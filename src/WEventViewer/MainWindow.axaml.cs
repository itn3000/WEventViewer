using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using WEventViewer.ViewModel;

namespace WEventViewer;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        WeakReferenceMessenger.Default.Register<MainWindow, OpenLogRequest>(this, async (recpient, req) =>
        {
            var vm = new OpenLogWindowViewModel();
            var dlg = new OpenLogWindow()
            {
                DataContext = vm
            };
            await dlg.ShowDialog(this);
            
        });
    }
}