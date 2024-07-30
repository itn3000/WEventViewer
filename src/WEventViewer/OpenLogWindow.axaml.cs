using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Messaging;
using Avalonia.Platform.Storage;
using WEventViewer.ViewModel;
using System.Collections.Generic;

namespace WEventViewer;

internal record class OpenDialogResultMessage(bool isOk);
public partial class OpenLogWindow : Window
{
    public OpenLogWindow()
    {
        InitializeComponent();
        WeakReferenceMessenger.Default.Register<OpenLogWindow, OpenDialogResultMessage>(this, (recipient, msg) =>
        {
            this.Close(msg.isOk);
        });
    }
    public async void OnLogOpenButtonClicked(object? sender, RoutedEventArgs routedEventArgs)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel != null && DataContext is OpenLogWindowViewModel vm)
        {
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
            {
                Title = "Open exported log file",
                AllowMultiple = false,
                SuggestedFileName = "*.evtx"
,            });
            if (files != null && files.Count != 0)
            {
                vm.LogName = files[0].Path.LocalPath;
            }
        }
    }
}