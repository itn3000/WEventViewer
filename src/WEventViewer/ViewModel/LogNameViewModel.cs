using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WEventViewer.Model;
using R3;

namespace WEventViewer.ViewModel
{
    internal class LogNameViewModel : INotifyPropertyChanged, IDisposable
    {
        EventLogRepository EventLogRepository;
        public LogNameViewModel(): this(new EventLogRepository())
        {

        }
        List<string> Original = new List<string>();
        ObservableCollection<string> _LogNames = new ObservableCollection<string>();
        public ObservableCollection<string> LogNames => _LogNames;
        public LogNameViewModel(EventLogRepository eventLogRepository)
        {
            this.EventLogRepository = eventLogRepository;
            LoadLogNames = new AsyncRelayCommand(async () =>
            {
                if (EventLogRepository != null)
                {
                    Original.Clear();
                    LogNames.Clear();
                    await EventLogRepository.LoadLogNames(lst =>
                    {
                        Original.AddRange(lst);
                        foreach (var item in lst.Where(x => !string.IsNullOrEmpty(SearchString) ? x.Contains(SearchString) : true))
                        {
                            LogNames.Add(item);
                        }
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LogNames)));
                    });
                }
            });
            _SearchStringSubscription = Observable.EveryValueChanged(this, x => x.SearchString)
                .ThrottleLast(TimeSpan.FromMilliseconds(500))
                .Subscribe(searchstr =>
                {
                    LogNames.Clear();
                    foreach (var item in Original.Where(x => !string.IsNullOrEmpty(searchstr) ? x.Contains(searchstr, StringComparison.InvariantCultureIgnoreCase) : true))
                    {
                        LogNames.Add(item);
                    }
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LogNames)));
                });
        }
        public ICommand LoadLogNames;
        string _SearchString = string.Empty;
        IDisposable? _SearchStringSubscription;
        public string SearchString
        {
            get => _SearchString;
            set
            {
                if (_SearchString != value)
                {
                    _SearchString = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchString)));
                }
            }
        }
        private bool disposedValue;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _SearchStringSubscription?.Dispose();
                }

                // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、ファイナライザーをオーバーライドします
                // TODO: 大きなフィールドを null に設定します
                disposedValue = true;
            }
        }

        // // TODO: 'Dispose(bool disposing)' にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします
        // ~LogNameViewModel()
        // {
        //     // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
