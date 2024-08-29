using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEventViewer.Model;

namespace WEventViewer.ViewModel
{
    internal class ViewModelFactoryServiceProvider(IServiceProvider provider) : IViewModelFactory
    {
        public MainWindowViewModel GetMainWindowViewModel()
        {
            return provider.GetRequiredService<MainWindowViewModel>();
        }
        public AboutViewModel GetAboutViewModel()
        {
            return provider.GetRequiredService<AboutViewModel>();
        }

        public DetailedLogViewModel GetDetailedLogViewModel(LogRecord record)
        {
            var vm = provider.GetRequiredService<DetailedLogViewModel>();
            vm.Initialize(record);
            return vm;
        }

        public ErrorWindowViewModel GetErrorViewWindowModel(string message)
        {
            var vm = provider.GetRequiredService<ErrorWindowViewModel>();
            vm.Message = message;
            return vm;
        }

        public LogNameViewModel GetLogNameViewModel()
        {
            return provider.GetRequiredService<LogNameViewModel>();
        }

        public OpenLogWindowViewModel GetOpenLogWindowViewMode()
        {
            return provider.GetRequiredService<OpenLogWindowViewModel>();
        }

        public ProviderNameWindowViewModel GetProviderNameWindowViewModel()
        {
            return provider.GetRequiredService<ProviderNameWindowViewModel>();
        }
    }
}
