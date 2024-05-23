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

namespace WEventViewer.ViewModel
{
    class OpenLogRequest
    {
        public string? LogName { get; set; }
        public PathType PathType { get; set; }
    }
    record class LoadLogMessage(string logName, PathType pathType);
    internal class MainWindowViewModel
    {
        EventLogRepository? _EventLogRepository;
        public MainWindowViewModel()
        {
            _Progress = new Progress<long>(l => LogCount = l);
            _EventLogRepository = new EventLogRepository();
            _EventLogRepository.Records.CollectionChanged += Records_CollectionChanged;
            OpenCommand = new RelayCommand(() =>
            {
                WeakReferenceMessenger.Default.Send<OpenLogRequest>(new OpenLogRequest());
            });
            WeakReferenceMessenger.Default.Register<MainWindowViewModel, LoadLogMessage>(this, async (vm, msg) =>
            {
                await _EventLogRepository.Load(msg.logName, msg.pathType, null, default, _Progress);
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
}
