using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Collections;
using System.Windows.Input;
using WEventViewer.Model;
using System.Diagnostics.Eventing.Reader;
using System.Diagnostics.Eventing;
using System.Collections.ObjectModel;

namespace WEventViewer.ViewModel
{
    class OpenLogRequest(MainWindowViewModel vm, string logName, PathType pathType)
    {
        public string LogName => logName;
        public PathType PathType => pathType;
    }
    internal class MainWindowViewModel
    {
        EventLogRepository? _EventLogRepository;
        public MainWindowViewModel(): this(null)
        {
        }
        public MainWindowViewModel(EventLogRepository repository)
        {
            _Progress = new Progress<long>(l => LogCount = l);
            _EventLogRepository = repository;
            OpenCommand = new RelayCommand(async () =>
            {
                var ret = WeakReferenceMessenger.Default.Send<OpenLogRequest>(new OpenLogRequest(this, "", PathType.LogName));
                await _EventLogRepository.Load(ret.LogName, ret.PathType, null, default, _Progress);
            });
        }
        Progress<long> _Progress;
        public ICommand OpenCommand;
        long _LogCount;
        public long LogCount
        {
            get => _LogCount;
            set
            {
                _LogCount = value;

            }
        }
        public ObservableCollection<LogRecord> LogRecords;
    }
}
