using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SaveLogViewer.API.Models;

namespace SaveLogViewer.API.Services
{
    public class SaveLogService : ISaveLogService
    {
        private readonly ILogger<SaveLogService> _logger;
        private readonly string _baseDirectory;

        public SaveLogService(ILogger<SaveLogService> logger)
        {
            _logger = logger;
            _baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SaveLogs");
            Directory.CreateDirectory(_baseDirectory);
        }

        public async Task<IEnumerable<SaveLog>> GetAllSaveLogsAsync()
        {
            var saveLogs = new List<SaveLog>();
            var directories = Directory.GetDirectories(_baseDirectory);

            foreach (var dir in directories)
            {
                var metadataFile = Path.Combine(dir, "metadata.json");
                if (File.Exists(metadataFile))
                {
                    var metadata = await File.ReadAllTextAsync(metadataFile);
                    var saveLog = System.Text.Json.JsonSerializer.Deserialize<SaveLog>(metadata);
                    saveLogs.Add(saveLog);
                }
            }

            return saveLogs;
        }

        public async Task<SaveLog> GetSaveLogByIdAsync(string id)
        {
            var metadataFile = Path.Combine(_baseDirectory, id, "metadata.json");
            if (!File.Exists(metadataFile))
                return null;

            var metadata = await File.ReadAllTextAsync(metadataFile);
            return System.Text.Json.JsonSerializer.Deserialize<SaveLog>(metadata);
        }

        public async Task<SaveLog> UploadSaveLogAsync(string filePath)
        {
            var id = Guid.NewGuid().ToString();
            var saveLog = new SaveLog
            {
                Id = id,
                FileName = Path.GetFileName(filePath),
                UploadDate = DateTime.UtcNow,
                FilePath = filePath,
                FileSize = new FileInfo(filePath).Length,
                IsProcessed = false,
                ProcessStatus = "Pending"
            };

            var saveLogDir = Path.Combine(_baseDirectory, id);
            Directory.CreateDirectory(saveLogDir);

            var metadataFile = Path.Combine(saveLogDir, "metadata.json");
            await File.WriteAllTextAsync(metadataFile, 
                System.Text.Json.JsonSerializer.Serialize(saveLog));

            // Trigger async processing
            _ = ProcessSaveLogAsync(id);

            return saveLog;
        }

        public async Task<bool> ProcessSaveLogAsync(string id)
        {
            var saveLog = await GetSaveLogByIdAsync(id);
            if (saveLog == null)
                return false;

            try
            {
                var extractPath = Path.Combine(_baseDirectory, id, "extracted");
                Directory.CreateDirectory(extractPath);

                // Extract the zip file
                await Task.Run(() => ZipFile.ExtractToDirectory(saveLog.FilePath, extractPath));

                // Process syngo.txt and other log files
                var files = Directory.GetFiles(extractPath, "*.*", SearchOption.AllDirectories);
                var entries = new List<LogEntry>();

                foreach (var file in files)
                {
                    if (Path.GetFileName(file).Equals("syngo.txt", StringComparison.OrdinalIgnoreCase))
                    {
                        entries.AddRange(await ParseCentralLogFile(file));
                    }
                    else if (Path.GetFileName(file).EndsWith(".txt"))
                    {
                        entries.AddRange(await ParseProcessLogFile(file));
                    }
                }

                saveLog.Entries = entries;
                saveLog.IsProcessed = true;
                saveLog.ProcessStatus = "Completed";

                await UpdateSaveLogMetadata(saveLog);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing savelog {Id}", id);
                saveLog.ProcessStatus = $"Error: {ex.Message}";
                await UpdateSaveLogMetadata(saveLog);
                return false;
            }
        }

        public async Task<IEnumerable<LogEntry>> SearchLogsAsync(SearchCriteria criteria)
        {
            var saveLog = await GetSaveLogByIdAsync(criteria.SaveLogId);
            if (saveLog?.Entries == null)
                return Array.Empty<LogEntry>();

            var query = saveLog.Entries.AsQueryable();

            if (!string.IsNullOrEmpty(criteria.ContextFolderId))
                query = query.Where(e => e.ContextFolderId == criteria.ContextFolderId);

            if (!string.IsNullOrEmpty(criteria.StudyUid))
                query = query.Where(e => e.StudyUid == criteria.StudyUid);

            if (!string.IsNullOrEmpty(criteria.WorkflowId))
                query = query.Where(e => e.WorkflowId == criteria.WorkflowId);

            if (!string.IsNullOrEmpty(criteria.ProcessId))
                query = query.Where(e => e.ProcessId == criteria.ProcessId);

            if (criteria.StartTime.HasValue)
                query = query.Where(e => e.Timestamp >= criteria.StartTime.Value);

            if (criteria.EndTime.HasValue)
                query = query.Where(e => e.Timestamp <= criteria.EndTime.Value);

            if (!string.IsNullOrEmpty(criteria.SearchText))
                query = query.Where(e => e.Message.Contains(criteria.SearchText));

            if (criteria.Severity?.Length > 0)
                query = query.Where(e => criteria.Severity.Contains(e.Severity));

            return query.ToList();
        }

        public async Task<IEnumerable<LogEntry>> GetTimelineDataAsync(string saveLogId, DateTime startTime, DateTime endTime)
        {
            var saveLog = await GetSaveLogByIdAsync(saveLogId);
            if (saveLog?.Entries == null)
                return Array.Empty<LogEntry>();

            return saveLog.Entries
                .Where(e => e.Timestamp >= startTime && e.Timestamp <= endTime)
                .OrderBy(e => e.Timestamp)
                .ToList();
        }

        private async Task<IEnumerable<LogEntry>> ParseCentralLogFile(string filePath)
        {
            var entries = new List<LogEntry>();
            var lines = await File.ReadAllLinesAsync(filePath);

            foreach (var line in lines)
            {
                if (TryParseLogEntry(line, out var entry))
                    entries.Add(entry);
            }

            return entries;
        }

        private async Task<IEnumerable<LogEntry>> ParseProcessLogFile(string filePath)
        {
            var entries = new List<LogEntry>();
            var lines = await File.ReadAllLinesAsync(filePath);
            var isClientLog = Path.GetFileName(filePath).Contains("client", StringComparison.OrdinalIgnoreCase);

            foreach (var line in lines)
            {
                if (TryParseLogEntry(line, out var entry))
                {
                    entry.IsClientLog = isClientLog;
                    entries.Add(entry);
                }
            }

            return entries;
        }

        private bool TryParseLogEntry(string line, out LogEntry entry)
        {
            entry = null;
            try
            {
                // This is a simplified parsing logic - you'll need to implement the actual parsing
                // based on your log format
                if (string.IsNullOrWhiteSpace(line))
                    return false;

                entry = new LogEntry
                {
                    Timestamp = DateTime.UtcNow, // Replace with actual parsing
                    Severity = "Info", // Replace with actual parsing
                    ProcessId = "0", // Replace with actual parsing
                    Message = line,
                    ContextFolderId = "", // Replace with actual parsing
                    TaskflowId = "", // Replace with actual parsing
                    StudyUid = "", // Replace with actual parsing
                    WorkflowId = "" // Replace with actual parsing
                };

                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task UpdateSaveLogMetadata(SaveLog saveLog)
        {
            var metadataFile = Path.Combine(_baseDirectory, saveLog.Id, "metadata.json");
            await File.WriteAllTextAsync(metadataFile,
                System.Text.Json.JsonSerializer.Serialize(saveLog));
        }
    }
} 