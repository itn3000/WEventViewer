using Avalonia.Controls;
using Avalonia.Interactivity;

namespace WEventViewer
{
    public partial class DetailedLogMessageWIndow : Window
    {
        public DetailedLogMessageWIndow()
        {
            InitializeComponent();
        }
        void OnCloseClicked(object? sender, RoutedEventArgs routedEventArgs)
        {
            this.Close();
        }
    }
}
