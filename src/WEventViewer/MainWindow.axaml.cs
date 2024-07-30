using Avalonia.Controls;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using WEventViewer.Model;
using WEventViewer.ViewModel;

namespace WEventViewer;

internal record class OpenErrorLogWindow(string message);
public partial class MainWindow : Window
{
    DiagnosticListener _DS = new DiagnosticListener(nameof(MainWindow));
    IServiceProvider? serviceProvider;
    public MainWindow() : this(null) { }
    public MainWindow(IServiceProvider? serviceProvider)
    {
        DataContext = serviceProvider != null ? serviceProvider.GetService<MainWindowViewModel>() : new MainWindowViewModel();
        this.serviceProvider = serviceProvider;
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
                WeakReferenceMessenger.Default.Send<LoadLogMessage>(new(vm.LogName, vm.CurrentSelected.PathType, vm.QueryString));
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

    private void DataGrid_DoubleTapped_1(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        _DS.Write("OnDataGridDoubleTapped", new { e.Source, e.Pointer });
        if(sender is DataGrid dataGrid)
        {
            if (dataGrid.SelectedItem is LogRecord record)
            {
                var vm = new DetailedLogViewModel(record);
                var w = new DetailedLogMessageWIndow()
                {
                    DataContext = vm,
                };
                w.Show(this);
            }
        }
    }

    private void PrintProviderClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _DS.Write("OnPrintProviderClick", new { e.Source, t = e.GetType() });
        if (serviceProvider != null)
        {
            var vm = serviceProvider.GetService<ProviderNameWindowViewModel>();
            var w = new ProviderNamesWindow() { DataContext = vm };
            w.Show();
        }

    }

    private void PrintLogNamesClick(object? sender, RoutedEventArgs e)
    {
        if (serviceProvider != null)
        {
            var vm = serviceProvider.GetService<LogNameViewModel>();
            var w = new LogNameWindow() { DataContext = vm };
            w.Show();
        }
    }

    private void AboutClick(object? sender, RoutedEventArgs e)
    {
        if (serviceProvider != null)
        {
            var vm = serviceProvider.GetService<AboutViewModel>();
            var w = new AboutWindow() { DataContext = vm };
            w.Show(this);
        }
    }

    private async void CopyAsXmlClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _DS.Write("CopyAsXmlClicked", new { sender, e });
        var rootelem = new XElement("Events");
        foreach (var item in this.LogDataGrid.SelectedItems.OfType<LogRecord>())
        {
            if(item.XmlString == null)
            {
                continue;
            }
            rootelem.Add(XElement.Parse(item.XmlString));
        }
        var xmlstr = rootelem.ToString(SaveOptions.None);
        await Clipboard.SetTextAsync(xmlstr);
    }
}