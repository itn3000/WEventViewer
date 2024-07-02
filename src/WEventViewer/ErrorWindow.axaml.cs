using Avalonia.Controls;
using CommunityToolkit.Mvvm.Messaging;
using WEventViewer.ViewModel;

namespace WEventViewer
{
    public partial class ErrorWindow : Window
    {

        public ErrorWindow()
        {
            InitializeComponent();
            WeakReferenceMessenger.Default.Register<ErrorWindowCloseMessage>(this, (recipient, msg) => this.Close());
        }
    }
}
