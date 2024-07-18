using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Messaging;

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

    private void Binding(object? sender, Avalonia.Controls.SizeChangedEventArgs e)
    {
    }
}