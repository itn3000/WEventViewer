using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace WEventViewer;

public partial class HelpWindow : Window
{
    public HelpWindow()
    {
        InitializeComponent();
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }
}