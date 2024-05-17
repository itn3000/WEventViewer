using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Eventing;
using System.Diagnostics.Eventing.Reader;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WEventViewer.Model
{
    internal record class LogRecord(Guid? ActivityId,
        int Id,
        long? Keywords,
        string[]? KeywordsDisplayNames,
        byte? Level,
        string? LevelDisplayName,
        string LogName,
        string MachineName,
        short? OpCode,
        string? OpcodeDisplayName,
        int? ProcessId,
        object[] Properties,
        Guid? ProviderId,
        string ProviderName,
        int? Qualifiers,
        long? RecordId,
        Guid? RelatedActivityId,
        int? Task,
        string? TaskDisplayName,
        int? ThreadId,
        DateTime? TimeCreated,
        byte? Version,
        string Formatted);
    internal static class EventLogRepositoryExtension
    {
        public static LogRecord ToLogRecord(this EventRecord record)
        {
            return new LogRecord(record.ActivityId, record.Id, record.Keywords, record.KeywordsDisplayNames.ToArray(), record.Level,
                record.LevelDisplayName, record.LogName, record.MachineName, record.Opcode, record.OpcodeDisplayName, record.ProcessId,
                record.Properties.Select(x => x.Value).ToArray(), record.ProviderId, record.ProviderName, record.Qualifiers, record.RecordId, record.RelatedActivityId,
                record.Task, record.TaskDisplayName, record.ThreadId, record.TimeCreated, record.Version, record.FormatDescription());
        }
    }
    internal class EventLogRepository
    {
        List<LogRecord> records = [];
        public IEnumerable<LogRecord> Records => records;
        public async Task Load(string logName, PathType pathType, string? query, CancellationToken token, IProgress<long> progress)
        {
            var lst = new List<LogRecord>();
            using var evreader = new EventLogReader(new EventLogQuery(logName, pathType, query));
            long count = 0;
            while(!token.IsCancellationRequested)
            {
                await Task.Yield();
                using var record = evreader.ReadEvent();
                if(record == null)
                {
                    break;
                }
                lst.Add(record.ToLogRecord());
                count++;
                if((count & 0xff) == 0)
                {
                    progress?.Report(count);
                }
            }
            progress?.Report(count);
            while(!token.IsCancellationRequested)
            {
                var old = records;
                var exchanged = Interlocked.CompareExchange(ref records, lst, old);
                if(exchanged == old)
                {
                    break;
                }
            }
        }
    }
}
