using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using WEventViewer.ViewModel;

namespace WEventViewer;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        DataContext = new MainWindowViewModel();
        InitializeComponent();
        WeakReferenceMessenger.Default.Register<MainWindow, OpenLogRequest>(this, async (recpient, req) =>
        {
            var vm = new OpenLogWindowViewModel();
            var dlg = new OpenLogWindow()
            {
                DataContext = vm
            };
            var ret = await dlg.ShowDialog<OpenLogWindowViewModel>(this);
            if(DataContext is MainWindowViewModel mwvm)
            {
                WeakReferenceMessenger.Default.Send<LoadLogMessage>(new(vm.LogName, vm.PathType));
            }
        });
    }
}