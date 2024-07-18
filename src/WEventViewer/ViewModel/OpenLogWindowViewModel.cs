using Avalonia.Data.Converters;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WEventViewer.ViewModel
{
    internal partial class PathTypeValueConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if(value is PathType ptype)
            {
                if(targetType ==  typeof(string))
                {
                    return ptype.ToString();
                }
                else if(targetType == typeof(int))
                {
                    return (int)ptype;
                }
            }
            throw new ArgumentException($"Convert: unsupported type({value?.GetType()})");
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if(value is string s)
            {
                if(s.Equals(nameof(PathType.LogName), StringComparison.OrdinalIgnoreCase))
                {
                    return PathType.LogName;
                }
                else if(s.Equals(nameof(PathType.FilePath), StringComparison.OrdinalIgnoreCase))
                {
                    return PathType.FilePath;
                }
            }
            else if(value is int i)
            {
                return (PathType)i;
            }
            throw new ArgumentException($"ConvertBack: unsupported type({value?.GetType()})");
        }
    }
    internal partial class OpenLogWindowViewModel : INotifyPropertyChanged
    {
        string _LogName = "";
        public string LogName { get => _LogName; set
            {
                _LogName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LogName)));
            }
        }
        PathType _PathType = PathType.LogName;
        public PathType PathType
        {
            get => _PathType;
            set
            {
                _PathType = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PathType)));
            }
        }
        public string A { get; set; } = "";
        public OpenLogWindowViewModel()
        {
            IsOk = false;
            OkCommand = new RelayCommand(() =>
            {
                IsOk = true;
                WeakReferenceMessenger.Default.Send<OpenDialogResultMessage>(new(true));
            }, () => true);
            CancelCommand = new RelayCommand(() => { 
                IsOk = false;
                WeakReferenceMessenger.Default.Send<OpenDialogResultMessage>(new(false));
            }, () => true);
        }
        public bool IsOk;
        public ICommand OkCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public event PropertyChangedEventHandler? PropertyChanged;
        [RelayCommand]
        public void OnSizeChanged(int WindowWidth)
        {
            LogNameWidth = WindowWidth;
        }
        public int LogNameWidth { get; set; }
    }
}
