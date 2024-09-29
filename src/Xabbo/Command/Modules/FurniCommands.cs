﻿using System.Text.RegularExpressions;

using Xabbo.Core;
using Xabbo.Core.Game;
using Xabbo.Core.GameData;
using Xabbo.Services.Abstractions;
using Xabbo.Configuration;
using Xabbo.Utility;
using Xabbo.Core.Messages.Outgoing;

namespace Xabbo.Command.Modules;

[CommandModule]
public sealed class FurniCommands(
    IConfigProvider<AppConfig> settingsProvider,
    IOperationManager operationManager,
    IGameDataManager gameData,
    ProfileManager profileManager,
    RoomManager roomManager) : CommandModule
{
    private readonly IConfigProvider<AppConfig> _settingsProvider = settingsProvider;
    private readonly IOperationManager _operationManager = operationManager;
    private readonly IGameDataManager _gameData = gameData;
    private readonly ProfileManager _profileManager = profileManager;
    private readonly RoomManager _roomManager = roomManager;

    [Command("furni", "f")]
    public async Task OnExecuteAsync(CommandArgs args)
    {
        if (args.Length < 1) return;
        string subCommand = args[0].ToLower();

        switch (subCommand)
        {
            case "s":
            case "show":
                await ShowFurniAsync(args); break;
            case "h":
            case "hide":
                await HideFurniAsync(args); break;
            case "p":
            case "pick":
            case "pickup":
                await PickupFurniAsync(args, false); break;
            case "e":
            case "eject":
                if (Session.Is(ClientType.Origins))
                {
                    ShowMessage("Origins does not support ejecting furni.");
                    return;
                }
                await PickupFurniAsync(args, true); break;
        }
    }

    private Task ShowFurniAsync(CommandArgs args)
    {
        IRoom? room = _roomManager.Room;
        if (room is not null)
        {
            string pattern = string.Join(' ', args.Skip(1));
            Regex regex = StringUtility.CreateWildcardRegex(pattern);
            foreach (IFurni furni in room.Furni)
            {
                if (furni.TryGetName(out string? name) &&
                    regex.IsMatch(name))
                {
                    _roomManager.ShowFurni(furni);
                }
            }
        }

        return Task.CompletedTask;
    }

    private Task HideFurniAsync(CommandArgs args)
    {
        IRoom? room = _roomManager.Room;
        if (room is not null)
        {
            string pattern = string.Join(" ", args.Skip(1));
            Regex regex = StringUtility.CreateWildcardRegex(pattern);
            foreach (IFurni furni in room.Furni)
            {
                if (furni.TryGetName(out string? name) &&
                    regex.IsMatch(name))
                {
                    _roomManager.HideFurni(furni);
                }
            }
        }

        return Task.CompletedTask;
    }

    private async Task PickupFurniAsync(CommandArgs args, bool eject)
    {
        IUserData? userData = _profileManager.UserData;
        IRoom? room = _roomManager.Room;

        if (userData is null || room is null)
        {
            ShowMessage(userData is null
                ? "User data is currently unavailable."
                : "Room state is unavailable, please re-enter the room."
            );
            return;
        }

        if (Session.Is(ClientType.Origins) && _roomManager.RightsLevel != RightsLevel.Owner)
        {
            ShowMessage("You must be the room owner to pick up furni.");
            return;
        }

        await _operationManager.RunAsync($"{(eject ? "eject" : "pickup")} furni", async ct =>
        {
            string pattern = string.Join(" ", args.Skip(1));
            if (string.IsNullOrWhiteSpace(pattern)) return;

            bool all = pattern.Equals("all", StringComparison.OrdinalIgnoreCase);
            Regex regex = StringUtility.CreateWildcardRegex(pattern);

            var allFurni = Session.Is(ClientType.Origins)
                ? room.Furni.ToArray()
                : room.Furni.Where(x =>
                    eject == (x.OwnerId != userData.Id)
                ).ToArray();

            var matched = all ? allFurni : allFurni.Where(furni =>
                furni.TryGetName(out string? name) &&
                (all || regex.IsMatch(name))
            ).ToArray();

            if (matched.Length == 0)
            {
                ShowMessage($"No furni to {(eject ? "eject" : "pick up")}.");
                return;
            }

            if (matched.Length == allFurni.Length && !all)
            {
                ShowMessage($"[Warning] Pattern matched all furni. Use '/{args.Command} {args[0]} all' to {(eject ? "eject" : "pick up")} all furni.");
                return;
            }

            int pickupInterval = Session.Is(ClientType.Origins)
                ? _settingsProvider.Value.Furni.PickupIntervalOrigins
                : _settingsProvider.Value.Furni.PickupInterval;

            int totalDelay = pickupInterval * matched.Length;
            string message = $"Picking up {matched.Length} furni...";
            if (totalDelay >= 2500)
                message += " Use /c to cancel.";
            ShowMessage(message);

            foreach (var furni in matched)
            {
                if (!Session.Is(ClientType.Origins) && eject == (furni.OwnerId == userData.Id)) continue;
                Ext.Send(new PickupItemMsg(furni.Type, furni.Id));
                await Task.Delay(pickupInterval, ct);
            }
        });
    }
}
