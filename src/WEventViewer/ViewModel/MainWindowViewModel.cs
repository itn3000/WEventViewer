using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows.Input;
using WEventViewer.Model;

namespace WEventViewer.ViewModel
{
    internal class MainWindowViewModel
    {
        record class OpenLogMessage(MainWindowViewModel vm);
        EventLogRepository? _EventLogRepository;
        public MainWindowViewModel(): this(null)
        {
        }
        public MainWindowViewModel(EventLogRepository repository)
        {
            OpenCommand = new RelayCommand(() =>
            {
                var ret = WeakReferenceMessenger.Default.Send<OpenLogMessage>(new(this));
            });
            _EventLogRepository = repository;
        }
        public ICommand OpenCommand;
    }
}
