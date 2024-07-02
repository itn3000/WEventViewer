using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;
using WEventViewer.Model;
using System.Diagnostics.Eventing.Reader;
using System.Diagnostics.Eventing;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Avalonia;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;


namespace WEventViewer.ViewModel
{
    class OpenLogRequest
    {
        public string? LogName { get; set; }
        public PathType PathType { get; set; }
    }
    record class LoadLogMessage(string logName, PathType pathType);
    internal class MainWindowViewModel : ObservableRecipient
    {
        EventLogRepository _EventLogRepository;
        public MainWindowViewModel()
        {
            _Progress = new Progress<long>(l =>
            {
                _LogCount = l;
            });
            _EventLogRepository = new EventLogRepository();
            _EventLogRepository.Records.CollectionChanged += Records_CollectionChanged;
            OpenCommand = new RelayCommand(() =>
            {
                WeakReferenceMessenger.Default.Send<OpenLogRequest>(new OpenLogRequest());
            });
            WeakReferenceMessenger.Default.Register<MainWindowViewModel, LoadLogMessage>(this, async (vm, msg) =>
            {
                try
                {
                    await _EventLogRepository.Load(msg.logName, msg.pathType, null, default, _Progress);
                }
                catch (Exception ex)
                {
                    WeakReferenceMessenger.Default.Send(new OpenErrorLogWindow(ex.ToString()));
                }
            });
        }

        private void Records_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if(_EventLogRepository != null)
            {
                LogCount = _EventLogRepository.Records.Count;
            }
        }

        Progress<long> _Progress;
        public ICommand OpenCommand { get; private set; }
        long _LogCount;
        public long LogCount
        {
            get => _LogCount;
            set
            {
                _LogCount = value;

            }
        }
        public ObservableCollection<LogRecord> LogRecords => _EventLogRepository.Records;
    }
    public class MyRecord(string a, int b)
    {
        public string A => a;
        public int B => b;
    }
}
