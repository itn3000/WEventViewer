using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using WEventViewer.ViewModel;

namespace WEventViewer;

public partial class LogNameWindow : Window
{
    public LogNameWindow()
    {
        InitializeComponent();
    }
    private void CloseButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Close();
    }

    private void Window_Loaded_1(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (this.DataContext is LogNameViewModel vm)
        {
            vm.LoadLogNames.Execute(null);
        }
    }
}