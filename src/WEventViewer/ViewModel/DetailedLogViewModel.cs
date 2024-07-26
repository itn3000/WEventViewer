using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using WEventViewer.Model;

namespace WEventViewer.ViewModel
{
    internal class DetailedLogViewModel : INotifyPropertyChanged
    {
        public DetailedLogViewModel() : this(null)
        {
        }
        LogRecord? logRecord;

        public event PropertyChangedEventHandler? PropertyChanged;

        public DetailedLogViewModel(LogRecord? logRecord)
        {
            this.logRecord = logRecord;
            if (logRecord != null && !string.IsNullOrEmpty(logRecord.XmlString))
            {
                var doc = XDocument.Parse(logRecord.XmlString);
                using var tw = new StringWriter();
                using var xw = new XmlTextWriter(tw);
                xw.Formatting = Formatting.Indented;
                doc.WriteTo(xw);
                _FormattedXmlString = tw.ToString();
            }
            else
            {
                _FormattedXmlString = string.Empty;
            }
            if(logRecord != null)
            {
                Items = new KeyValuePair<string, string?>[]
                {
                    new("Id", logRecord.Id.ToString()),
                    new("RecordId", logRecord.RecordId?.ToString()),
                    new("ActivityId", logRecord.ActivityId?.ToString()),
                    new("LogName", logRecord.LogName),
                    new("MachineName", logRecord.MachineName),
                    new("Level", string.IsNullOrEmpty(logRecord.LevelDisplayName) ? logRecord.Level?.ToString() : $"{logRecord.LevelDisplayName}({logRecord.Level})"),
                    new("TimeCreated", logRecord.TimeCreated?.ToString("o")),
                    new("ProviderName", logRecord.ProviderName),
                    new("OpCode", logRecord.OpCode?.ToString("x")),
                    new("Keywords", logRecord.Keywords?.ToString("x")),
                    new("Tasks", logRecord.Task?.ToString("x")),
                    new("Qualifiers", logRecord.Qualifiers?.ToString("x")),
                    new("Description", logRecord.Formatted),
                };
            }
        }
        public KeyValuePair<string, string?>[] Items { get; private set; } = Array.Empty<KeyValuePair<string, string?>>();
        string _FormattedXmlString;
        public string XmlString => _FormattedXmlString;
        public string? FormattedString => logRecord?.Formatted;
        public DateTime? TimeCreated => logRecord?.TimeCreated;
        public long? Keywords => logRecord?.Keywords;
        public byte? Level => logRecord?.Level;
        public string? LevelDisplayName => logRecord?.LevelDisplayName;
        public string? LogName => logRecord?.LogName;
        public string? MachineName => logRecord?.MachineName;
        public short? OpCode => logRecord?.OpCode;
        public int? ProcessId => logRecord?.ProcessId;
        public long? RecordId => logRecord?.RecordId;
        public string? ProviderName => logRecord?.ProviderName;
    }
}
