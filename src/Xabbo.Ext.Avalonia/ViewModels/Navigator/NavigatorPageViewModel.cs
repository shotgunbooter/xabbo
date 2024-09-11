﻿using IconSource = FluentAvalonia.UI.Controls.IconSource;

using FluentIcons.Common;
using FluentIcons.Avalonia.Fluent;

namespace Xabbo.Ext.Avalonia.ViewModels;

public class NavigatorPageViewModel : PageViewModel
{
    public override string Header => "Navigator";
    public override IconSource? Icon { get; } = new SymbolIconSource { Symbol = Symbol.Building };

    public NavigatorPageViewModel() { }
}
