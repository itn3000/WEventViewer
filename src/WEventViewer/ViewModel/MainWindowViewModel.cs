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
using Avalonia.Threading;
using Avalonia.Input;
using Microsoft.Extensions.DependencyInjection;


namespace WEventViewer.ViewModel
{
    class OpenLogRequest
    {
        public string? LogName { get; set; }
        public PathType PathType { get; set; }
    }
    record class LoadLogMessage(string logName, PathType pathType, string query);
    record class MainWindowCloseMessage();
    record class OpenDetailedLogMessage();
    internal class MainWindowViewModel : ObservableRecipient
    {
        EventLogRepository _EventLogRepository;
        Task LoadTask;
        public MainWindowViewModel(): this(new EventLogRepository()) { }
        public MainWindowViewModel(EventLogRepository eventLogRepository)
        {
            LoadTask = Task.CompletedTask;
            _EventLogRepository = eventLogRepository;
            _Progress = new Progress<long>(l =>
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    OnPropertyChanged(nameof(LogCount));
                    OnPropertyChanged(nameof(LogRecords));
                });
            });
            OpenCommand = new RelayCommand(() =>
            {
                LoadStatus = "Loading";
                OnPropertyChanged(nameof(LoadStatus));
                WeakReferenceMessenger.Default.Send<OpenLogRequest>(new OpenLogRequest());
            }, () => LoadTask == null || LoadTask.IsCompleted);
            WeakReferenceMessenger.Default.Register<MainWindowViewModel, LoadLogMessage>(this, (vm, msg) =>
            {
                LoadTask = _EventLogRepository.Load(msg.logName, msg.pathType, msg.query, default, (lst) =>
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        _EventLogRepository.Records.AddRange(lst);
                        OnPropertyChanged(nameof(LogCount));
                        OnPropertyChanged(nameof(LogRecords));
                    });
                })
                    .ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                        {
                            Dispatcher.UIThread.Invoke(() => WeakReferenceMessenger.Default.Send(new OpenErrorLogWindow(t.Exception.ToString())));
                        }
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            LoadStatus = "Complete";
                            OnPropertyChanged(nameof(LoadStatus));
                        });
                    });
            });
            CloseCommand = new RelayCommand(() => WeakReferenceMessenger.Default.Send(new MainWindowCloseMessage()));
            LoadStatus = string.Empty;
        }

        Progress<long> _Progress;
        public ICommand OpenCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }
        long _LogCount;
        public long LogCount
        {
            get
            {
                if (_EventLogRepository != null && _EventLogRepository.Records != null)
                {
                    return _EventLogRepository.Records.Count;
                }
                else
                {
                    return 0;
                }
            }
        }
        public RangedObservableCollection<LogRecord> LogRecords => _EventLogRepository.Records;
        public string LoadStatus { get; private set; }
        double _CurrentWindowHeight;
        public double CurrentWindowHeight
        {
            get => _CurrentWindowHeight;
            set
            {
                _CurrentWindowHeight = value;
                OnPropertyChanged(nameof(CurrentWindowHeight));
                OnPropertyChanged(nameof(ScrollViewerHeight));
                OnPropertyChanged(nameof(LogViewMaxHeight));
            }
        }
        public double LogViewMaxHeight => ScrollViewerHeight - 40;
        public double ScrollViewerHeight => CurrentWindowHeight - 160;
    }
    public class MyRecord(string a, int b)
    {
        public string A => a;
        public int B => b;
    }
}
