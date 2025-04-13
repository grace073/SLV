using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SaveLogViewer.API.Models;

namespace SaveLogViewer.API.Services
{
    public interface ISaveLogService
    {
        Task<IEnumerable<SaveLog>> GetAllSaveLogsAsync();
        Task<SaveLog> GetSaveLogByIdAsync(string id);
        Task<SaveLog> UploadSaveLogAsync(string filePath);
        Task<IEnumerable<LogEntry>> SearchLogsAsync(SearchCriteria criteria);
        Task<bool> ProcessSaveLogAsync(string id);
        Task<IEnumerable<LogEntry>> GetTimelineDataAsync(string saveLogId, DateTime startTime, DateTime endTime);
    }

    public class SearchCriteria
    {
        public string SaveLogId { get; set; }
        public string ContextFolderId { get; set; }
        public string StudyUid { get; set; }
        public string WorkflowId { get; set; }
        public string ProcessId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string SearchText { get; set; }
        public string[] Severity { get; set; }
    }
} 