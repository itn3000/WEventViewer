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


namespace WEventViewer.ViewModel
{
    class OpenLogRequest
    {
        public string? LogName { get; set; }
        public PathType PathType { get; set; }
    }
    record class LoadLogMessage(string logName, PathType pathType);
    record class MainWindowCloseMessage();
    internal class MainWindowViewModel : ObservableRecipient
    {
        EventLogRepository _EventLogRepository;
        Task LoadTask;
        public MainWindowViewModel()
        {
            LoadTask = Task.CompletedTask;
            _Progress = new Progress<long>(l =>
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    OnPropertyChanged(nameof(LogCount));
                    OnPropertyChanged(nameof(LogRecords));
                });
            });
            _EventLogRepository = new EventLogRepository();
            //_EventLogRepository.Records.CollectionChanged;
            OpenCommand = new RelayCommand(() =>
            {
                LoadStatus = "Loading";
                OnPropertyChanged(nameof(LoadStatus));
                WeakReferenceMessenger.Default.Send<OpenLogRequest>(new OpenLogRequest());
            }, () => LoadTask == null || LoadTask.IsCompleted);
            WeakReferenceMessenger.Default.Register<MainWindowViewModel, LoadLogMessage>(this, (vm, msg) =>
            {
                LoadTask = _EventLogRepository.Load(msg.logName, msg.pathType, null, default, (lst) =>
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        foreach (var item in lst)
                        {
                            _EventLogRepository.Records.Add(item);
                        }
                        OnPropertyChanged(nameof(LogCount));
                        OnPropertyChanged(nameof(LogRecords));
                    });
                })
                    .ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                        {
                            WeakReferenceMessenger.Default.Send(new OpenErrorLogWindow(t.Exception.ToString()));
                        }
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            LoadStatus = "Complete";
                            OnPropertyChanged(nameof(LoadStatus));
                        });
                    });
            });
            CloseCommand = new RelayCommand(() => WeakReferenceMessenger.Default.Send(new MainWindowCloseMessage()));
        }

        private void Records_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            //OnPropertyChanged(nameof(LogCount));
            //OnPropertyChanged(nameof(LogRecords));
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
        public ObservableCollection<LogRecord> LogRecords => _EventLogRepository.Records;
        public string LoadStatus { get; private set; }
    }
    public class MyRecord(string a, int b)
    {
        public string A => a;
        public int B => b;
    }
}
