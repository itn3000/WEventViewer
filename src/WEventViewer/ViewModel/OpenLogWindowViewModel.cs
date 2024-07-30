using Avalonia.Controls;
using Avalonia.Data.Converters;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
    public class PathTypeDefinition(PathType pathType, string displayName)
    {
        public PathType PathType => pathType;
        public string DisplayName => displayName;
    }
    internal partial class OpenLogWindowViewModel : INotifyPropertyChanged
    {
        private readonly static DiagnosticSource _DS = new DiagnosticListener("WEventView.OpenLogWindowViewMode");
        string _LogName = "";
        public string LogName { get => _LogName; set
            {
                _LogName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LogName)));
            }
        }
        //PathType _PathType = PathType.LogName;
        //public PathType PathType
        //{
        //    get => _PathType;
        //    set
        //    {
        //        _PathType = value;
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PathType)));
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnableFilePathOpenButton)));
        //    }
        //}
        PathTypeDefinition _CurrentSelected = _PathTypes[0];
        public PathTypeDefinition CurrentSelected
        {
            get => _CurrentSelected;
            set
            {
                _CurrentSelected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentSelected)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnableFilePathOpenButton)));
            }
        }

        [RelayCommand]
        void OnPathTypeChanged(Avalonia.Controls.SelectionChangedEventArgs evargs)
        {
            _DS.Write("OnPathTypeChanged", evargs);
        }
        static readonly PathTypeDefinition[] _PathTypes = [new PathTypeDefinition(PathType.LogName, "LogName"), new PathTypeDefinition(PathType.FilePath, "FilePath")];
        public IList<PathTypeDefinition> PathTypes
        {
            get => _PathTypes;
            private set { }
        }
        public string[] Hoge => ["a", "b", "c"];
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
        public bool IsEnableFilePathOpenButton => CurrentSelected != null ? CurrentSelected.PathType == PathType.FilePath : false;
        bool _UseRawQuery = false;
        public bool UseRawQuery
        {
            get => _UseRawQuery;
            set
            {
                var changed = _UseRawQuery != value;
                _UseRawQuery = value;
                if (changed)
                {
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs(nameof(UseRawQuery)));
                    }
                    if (_UseRawQuery)
                    {
                        UseBeginDate = false;
                        UseEndDate = false;
                        UseProviderNames = false;
                    }
                }
            }
        }
        public string RawQuery { get; set; } = string.Empty;
        public string QueryString => BuildQuery();
        string BuildQuery()
        {
            if (UseRawQuery)
            {
                return RawQuery;
            }
            else
            {
                var conditions = new List<string>();
                if (UseBeginDate)
                {
                    var d = BeginDateTime;
                    if (d != null)
                    {
                        conditions.Add($"TimeCreated[@SystemTime >= '{d:yyyy-MM-ddTHH:mm:ss}']");
                    }

                }
                if (UseEndDate)
                {
                    var d = EndDateTime;
                    if (d != null)
                    {
                        conditions.Add($"TimeCreated[@SystemTime <= '{d:yyyy-MM-ddTHH:mm:ss}']");
                    }
                }
                if (UseProviderNames && !string.IsNullOrEmpty(ProviderNames))
                {
                    var providerConditions = string.Join(" or ", ProviderNames.Split(',').Select(x => $"@Name = '{x.Trim()}'"));
                    conditions.Add($"Provider[{providerConditions}]");
                }
                if (conditions.Count > 0)
                {
                    return $"*[System[{string.Join(" and ", conditions)}]]";
                }
            }
            return string.Empty;
        }
        bool _UseTimeCreated = false;
        public bool UseTimeCreated
        {
            get => _UseTimeCreated;
            set
            {
                var changed = _UseTimeCreated != value;
                _UseTimeCreated = value;
                if (changed)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UseTimeCreated)));
                    if (UseRawQuery && value)
                    {
                        UseRawQuery = false;
                    }
                }
            }
        }
        bool _UseBeginDate = false;
        public bool UseBeginDate
        {
            get => _UseBeginDate;
            set
            {
                var changed = _UseBeginDate != value;
                _UseBeginDate = value;
                if (changed)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UseBeginDate)));
                    if (UseRawQuery && value)
                    {
                        UseRawQuery = false;
                    }
                }
            }
        }
        bool _UseEndDate = false;
        public bool UseEndDate
        {
            get => _UseEndDate;
            set
            {
                var changed = _UseEndDate != value;
                _UseEndDate = value;
                if (changed)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UseEndDate)));
                    if (UseRawQuery && value)
                    {
                        UseRawQuery = false;
                    }
                }
            }
        }
        public DateTime? BeginDateTime
        {
            get
            {
                if(BeginDate != null && DateTime.TryParse(BeginDate, out var d))
                {
                    if(BeginTime != null && DateTime.TryParse(BeginTime, out var t))
                    {
                        return new DateTime(d.Year, d.Month, d.Day, t.Hour, t.Minute, t.Second);
                    }
                    else
                    {
                        return d;
                    }
                }
                return null;
            }
        }
        public DateTime? EndDateTime
        {
            get
            {
                if (EndDate != null && DateTime.TryParse(EndDate, out var d))
                {
                    if (EndTime != null && DateTime.TryParse(EndTime, out var t))
                    {
                        return new DateTime(d.Year, d.Month, d.Day, t.Hour, t.Minute, t.Second);
                    }
                    else
                    {
                        return d;
                    }
                }
                return null;
            }
        }
        public string BeginDate { get; set; } = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
        public string BeginTime { get; set; } = "00:00:00";
        public string EndDate { get; set; } = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
        public string EndTime { get; set; } = "00:00:00";
        bool _UseProviderNames = false;
        public bool UseProviderNames
        {
            get => _UseProviderNames;
            set
            {
                var changed = _UseProviderNames != value;
                _UseProviderNames = value;
                if(changed)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs($"{nameof(UseProviderNames)}"));
                    if(UseRawQuery && value)
                    {
                        UseRawQuery = false;
                    }
                }
            }
        }
        public string ProviderNames { get; set; } = string.Empty;
    }
}
