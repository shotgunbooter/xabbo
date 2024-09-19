﻿using System;
using System.Reactive.Linq;
using System.Diagnostics;

using Avalonia.Media;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using FluentIcons.Common;
using FluentIcons.Avalonia.Fluent;

using IconSource = FluentAvalonia.UI.Controls.IconSource;

using Splat;

using Xabbo.Extension;
using Xabbo.Messages.Flash;

using Xabbo.Components;
using Xabbo.Services.Abstractions;
using Xabbo.Configuration;

namespace Xabbo.ViewModels;

public class GeneralPageViewModel : PageViewModel
{
    public override string Header => "General";
    public override IconSource? Icon { get; } = new SymbolIconSource { Symbol = Symbol.AppGeneric };

    private readonly IConfigProvider<AppConfig> _settingsProvider;
    private readonly IExtension _ext;

    public AppConfig Config => _settingsProvider.Value;

    public GeneralPageViewModel(IConfigProvider<AppConfig> settingsProvider, IExtension ext)
    {
        _settingsProvider = settingsProvider;
        _ext = ext;

        _settingsProvider
            .WhenAnyValue(x => x.Value)
            .Subscribe(_ => this.RaisePropertyChanged(nameof(AppConfig)));

        this.ObservableForProperty(x => x.TonerColor)
            .Sample(TimeSpan.FromMilliseconds(300))
            .Subscribe(change =>
            {
                var color = change.Value.ToHsl();
                Debug.WriteLine(color.ToString());
                _ext.Send(Out.SetRoomBackgroundColorData,
                    831085267, (int)Math.Round(color.H / 360 * 255), (int)Math.Round(color.S * 255), (int)Math.Round(color.L * 255));
            });
    }

    [Reactive] public Color TonerColor { get; set; } = Colors.Black;

    [Reactive] public bool IsRoomExpanded { get; set; } = true;
    [Reactive] public bool IsMovementExpanded { get; set; } = true;
    [Reactive] public bool IsClickToExpanded { get; set; } = true;
    [Reactive] public bool IsEntryExpanded { get; set; } = true;
    [Reactive] public bool IsChatExpanded { get; set; } = true;
    [Reactive] public bool IsMuteExpanded { get; set; } = true;
    [Reactive] public bool IsFurniExpanded { get; set; } = true;
    [Reactive] public bool IsHandItemExpanded { get; set; } = true;
    [Reactive] public bool IsAlertsExpanded { get; set; } = true;
    [Reactive] public bool IsFlashWindowExpanded { get; set; } = true;
    [Reactive] public bool IsMiscExpanded { get; set; } = true;

    [DependencyInjectionProperty] public ChatComponent? ChatComponent { get; internal set; }
    [DependencyInjectionProperty] public FurniActionsComponent? Furni { get; internal set; }
    [DependencyInjectionProperty] public AntiHandItemComponent? HandItem { get; internal set; }
    [DependencyInjectionProperty] public AntiIdleComponent? AntiIdle { get; internal set; }
    [DependencyInjectionProperty] public AntiTradeComponent? AntiTrade { get; internal set; }
    [DependencyInjectionProperty] public AntiTurnComponent? AntiTurn { get; internal set; }
    [DependencyInjectionProperty] public AntiTypingComponent? AntiTyping { get; internal set; }
    [DependencyInjectionProperty] public AntiWalkComponent? AntiWalk { get; internal set; }
    [DependencyInjectionProperty] public ClickThroughComponent? ClickThrough { get; internal set; }
    [DependencyInjectionProperty] public ClickToComponent? ClickTo { get; internal set; }
    [DependencyInjectionProperty] public RespectedComponent? Respected { get; internal set; }
    [DependencyInjectionProperty] public RoomEntryComponent? RoomEntry { get; internal set; }
    [DependencyInjectionProperty] public DoorbellComponent? Doorbell { get; internal set; }
    [DependencyInjectionProperty] public FlattenRoomComponent? Flatten { get; internal set; }
    [DependencyInjectionProperty] public AvatarOverlayComponent? Overlay { get; internal set; }
    [DependencyInjectionProperty] public AntiHcGiftNotificationComponent? AntiHcNotification { get; internal set; }
}
