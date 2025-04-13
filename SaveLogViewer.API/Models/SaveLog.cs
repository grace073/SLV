using System;
using System.Collections.Generic;

namespace SaveLogViewer.API.Models
{
    public class SaveLog
    {
        public string Id { get; set; }
        public string FileName { get; set; }
        public DateTime UploadDate { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public bool IsProcessed { get; set; }
        public string ProcessStatus { get; set; }
        public List<LogEntry> Entries { get; set; }
    }

    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string Severity { get; set; }
        public string ProcessId { get; set; }
        public string Message { get; set; }
        public string ContextFolderId { get; set; }
        public string TaskflowId { get; set; }
        public string StudyUid { get; set; }
        public string WorkflowId { get; set; }
        public bool IsClientLog { get; set; }
    }
} 