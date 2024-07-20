using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using WEventViewer.ViewModel;

namespace WEventViewer;

internal record class OpenErrorLogWindow(string message);
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
            var ret = await dlg.ShowDialog<bool>(this);
            if(ret && DataContext is MainWindowViewModel mwvm)
            {
                WeakReferenceMessenger.Default.Send<LoadLogMessage>(new(vm.LogName, vm.CurrentSelected.PathType));
            }
        });
        WeakReferenceMessenger.Default.Register<MainWindow, OpenErrorLogWindow>(this, async (mw, msg) =>
        {
            var vm = new ErrorWindowViewModel(msg.message);
            var dlg = new ErrorWindow() { DataContext = vm };
            await dlg.ShowDialog(mw);
        });
        WeakReferenceMessenger.Default.Register<MainWindow, MainWindowCloseMessage>(this, (w, msg) => w.Close());
    }

    private void Window_SizeChanged(object? sender, Avalonia.Controls.SizeChangedEventArgs e)
    {
        if (DataContext is MainWindowViewModel mwvm)
        {
            mwvm.CurrentWindowHeight = this.Height;
        }
    }
}