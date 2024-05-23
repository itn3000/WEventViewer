using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Messaging;

namespace WEventViewer;

public partial class OpenLogWindow : Window
{
    internal record class OpenDialogResultMessage(bool isOk);
    public OpenLogWindow()
    {
        InitializeComponent();
        WeakReferenceMessenger.Default.Register<OpenLogWindow, OpenDialogResultMessage>(this, (recipient, msg) =>
        {
            if(msg.isOk)
            {
                
            }
        });
    }
}