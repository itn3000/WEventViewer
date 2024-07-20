﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Eventing;
using System.Diagnostics.Eventing.Reader;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using R3.Collections;

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
        static readonly DiagnosticListener _DS = new DiagnosticListener(nameof(EventLogRepositoryExtension));
        public static LogRecord ToLogRecord(this EventRecord record)
        {
            var keywords = record.Keywords;
            var keywordDisplayNames = GetKeywordsDisplayNames(record);
            var levelName = GetLevelDisplayName(record);
            var taskDisplayName = GetTaskDisplayName(record);
            var x = "hogehoge";
            var opcodeDisplayName = GetOpCodeDisplayName(record);
            return new LogRecord(record.ActivityId, record.Id, keywords, keywordDisplayNames, record.Level,
                levelName, record.LogName, record.MachineName, record.Opcode, opcodeDisplayName, record.ProcessId,
                record.Properties.Select(x => x.Value).ToArray(), record.ProviderId, record.ProviderName, record.Qualifiers, record.RecordId, record.RelatedActivityId,
                record.Task, taskDisplayName, record.ThreadId, record.TimeCreated, record.Version, record.FormatDescription());
        }
        static string[] GetKeywordsDisplayNames(EventRecord record)
        {
            try
            {
                return record.KeywordsDisplayNames.ToArray();
            }
            catch (EventLogException ex)
            {
                _DS.Write("Exception", new { Property = "KeyworkDisplayName", ex, record.LogName, record.ProviderName, record.Id });
                return Array.Empty<string>();
            }
        }
        static string GetLevelDisplayName(EventRecord record)
        {
            try
            {
                return record.LevelDisplayName;
            }
            catch (EventLogException ex)
            {
                _DS.Write("Exception", new { Property = "LevelDisplayName", ex, record.LogName, record.ProviderName, record.Id });
                return string.Empty;
            }
        }
        static string GetTaskDisplayName(EventRecord record)
        {
            try
            {
                return record.TaskDisplayName;
            }
            catch (EventLogException ex)
            {
                _DS.Write("Exception", new { Property = "TaskDisplayName", ex = ex, record.LogName, record.ProviderName, record.Id });
                return string.Empty;
            }
        }
        static string GetOpCodeDisplayName(EventRecord record)
        {
            try
            {
                return record.OpcodeDisplayName;
            }
            catch (EventLogException ex)
            {
                _DS.Write("Exception", new { Property = "OpcodeDisplayName", ex, record.LogName, record.ProviderName, record.Id });
                return string.Empty;
            }
        }
    }
    internal class EventLogRepository
    {
        RangedObservableCollection<LogRecord> records = [];
        public RangedObservableCollection<LogRecord> Records => records;
        public void Clear()
        {
            records.Clear();
        }
        static readonly DiagnosticListener _DS = new DiagnosticListener(nameof(EventLogRepository));
        public async Task Load(string logName, PathType pathType, string? query, CancellationToken token, Action<IList<LogRecord>> dispatch)
        {
            using var evreader = new EventLogReader(new EventLogQuery(logName, pathType, query));
            Clear();
            long count = 0;
            List<LogRecord> lst = [];
            await Task.Delay(10).ConfigureAwait(false);
            while (!token.IsCancellationRequested)
            {
                using var record = evreader.ReadEvent();
                try
                {
                    if (record == null)
                    {
                        break;
                    }
                    lst.Add(record.ToLogRecord());
                    count++;
                    if ((count & 0xff) == 0)
                    {
                        dispatch(lst);
                        lst.Clear();
                    }
                }
                catch (Exception ex)
                {
                    _DS.Write("Error", new { Exception = ex, LogRecordDescription = record.FormatDescription() });
                }
            }
            if (lst.Count > 0)
            {
                dispatch(lst);
            }
        }
    }
}
