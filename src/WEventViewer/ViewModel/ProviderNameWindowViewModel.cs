using Avalonia.Threading;
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
using ObservableCollections;

namespace WEventViewer.ViewModel
{
    internal class ProviderNameWindowViewModel : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        List<string> Original = new List<string>();
        public ObservableCollection<string> Providers { get; set; }
        public ICommand LoadCommand;
        EventLogRepository EventLogRepository { get; set; }
        public ProviderNameWindowViewModel() : this(new EventLogRepository())
        {

        }
        public void Dispose()
        {
            _SearchStringSubscription?.Dispose();
            _SearchStringSubscription = null;
        }
        public ProviderNameWindowViewModel(EventLogRepository eventLogRepository)
        {
            EventLogRepository = eventLogRepository;
            Providers = new();
            LoadCommand = new AsyncRelayCommand(async () =>
            {
                if (Providers != null)
                {
                    Providers.Clear();
                    Original.Clear();
                    await EventLogRepository.LoadProviderNames(lst =>
                    {
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            Original.AddRange(lst);
                            foreach (var item in lst.Where(x => !string.IsNullOrEmpty(_SearchString) ? x.Contains(_SearchString) : true))
                            {
                                Providers.Add(item);
                            }
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Providers)));
                        });
                    });
                }
            });
            _SearchStringSubscription = Observable.EveryValueChanged(this, x => x.SearchString)
                .ThrottleLast(TimeSpan.FromMilliseconds(500))
                .Subscribe(searchstr =>
                {
                    Providers.Clear();
                    foreach (var item in Original.Where(x => !string.IsNullOrEmpty(searchstr) ? x.Contains(searchstr, StringComparison.InvariantCultureIgnoreCase) : true))
                    {
                        Providers.Add(item);
                    }
                    //Providers.AddRange(Original.Where(x => !string.IsNullOrEmpty(searchstr) ? x.Contains(searchstr) : true));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Providers)));
                });
        }
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
    }
}
