using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEventViewer.Model;

namespace WEventViewer.ViewModel
{
    internal interface IViewModelFactory
    {
        MainWindowViewModel GetMainWindowViewModel();
        OpenLogWindowViewModel GetOpenLogWindowViewMode();
        ErrorWindowViewModel GetErrorViewWindowModel(string message);
        LogNameViewModel GetLogNameViewModel();
        DetailedLogViewModel GetDetailedLogViewModel(LogRecord record);
        AboutViewModel GetAboutViewModel();
        ProviderNameWindowViewModel GetProviderNameWindowViewModel();
    }
    class StubViewModelFactory : IViewModelFactory
    {
        public MainWindowViewModel GetMainWindowViewModel() => new MainWindowViewModel();
        public AboutViewModel GetAboutViewModel()
        {
            return new AboutViewModel();
        }

        public DetailedLogViewModel GetDetailedLogViewModel(LogRecord record)
        {
            return new DetailedLogViewModel(record);
        }

        public ErrorWindowViewModel GetErrorViewWindowModel(string message)
        {
            return new ErrorWindowViewModel();
        }

        public LogNameViewModel GetLogNameViewModel()
        {
            return new LogNameViewModel();
        }

        public OpenLogWindowViewModel GetOpenLogWindowViewMode()
        {
            throw new NotImplementedException();
        }

        public ProviderNameWindowViewModel GetProviderNameWindowViewModel()
        {
            throw new NotImplementedException();
        }
    }
}
