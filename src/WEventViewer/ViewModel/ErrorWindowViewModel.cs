using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WEventViewer.ViewModel
{
    internal record class ErrorWindowCloseMessage();
    internal class ErrorWindowViewModel
    {
        public ErrorWindowViewModel() : this("")
        {
        }
        public ErrorWindowViewModel(string message)
        {
            Message = message;
            CloseCommand = new RelayCommand(() =>
            {
                WeakReferenceMessenger.Default.Send<ErrorWindowCloseMessage>(new());
            });
        }
        public string Message { get; set; }
        public ICommand CloseCommand { get; set; }
    }
}
