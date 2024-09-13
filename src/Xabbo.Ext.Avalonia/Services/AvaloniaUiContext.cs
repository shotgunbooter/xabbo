﻿using System;
using System.Threading.Tasks;

using Avalonia.Threading;

using Xabbo.Ext.Services;

namespace Xabbo.Ext.Avalonia.Services;

public class AvaloniaUiContext : IUiContext
{
    public bool IsSynchronized => Dispatcher.UIThread.CheckAccess();
    public void Invoke(Action callback) => Dispatcher.UIThread.Invoke(callback);
    public Task InvokeAsync(Action callback) => Dispatcher.UIThread.InvokeAsync(callback).GetTask();
}
