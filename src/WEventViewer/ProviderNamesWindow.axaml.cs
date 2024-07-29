using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using WEventViewer.ViewModel;

namespace WEventViewer;

public partial class ProviderNamesWindow : Window
{
    public ProviderNamesWindow()
    {
        InitializeComponent();
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Close();
    }

    private void Window_Loaded_1(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (this.DataContext is ProviderNameWindowViewModel vm)
        {
            vm.LoadCommand.Execute(null);
        }
    }
}