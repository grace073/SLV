using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SaveLogViewer.API.Models;
using SaveLogViewer.API.Services;

namespace SaveLogViewer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SaveLogController : ControllerBase
    {
        private readonly ISaveLogService _saveLogService;

        public SaveLogController(ISaveLogService saveLogService)
        {
            _saveLogService = saveLogService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SaveLog>>> GetAllSaveLogs()
        {
            var saveLogs = await _saveLogService.GetAllSaveLogsAsync();
            return Ok(saveLogs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SaveLog>> GetSaveLog(string id)
        {
            var saveLog = await _saveLogService.GetSaveLogByIdAsync(id);
            if (saveLog == null)
                return NotFound();

            return Ok(saveLog);
        }

        [HttpPost("upload")]
        public async Task<ActionResult<SaveLog>> UploadSaveLog(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var filePath = Path.GetTempFileName();
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var saveLog = await _saveLogService.UploadSaveLogAsync(filePath);
                return Ok(saveLog);
            }
            catch (Exception ex)
            {
                System.IO.File.Delete(filePath);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("search")]
        public async Task<ActionResult<IEnumerable<LogEntry>>> SearchLogs([FromBody] SearchCriteria criteria)
        {
            var results = await _saveLogService.SearchLogsAsync(criteria);
            return Ok(results);
        }

        [HttpGet("{id}/timeline")]
        public async Task<ActionResult<IEnumerable<LogEntry>>> GetTimeline(
            string id,
            [FromQuery] DateTime startTime,
            [FromQuery] DateTime endTime)
        {
            var results = await _saveLogService.GetTimelineDataAsync(id, startTime, endTime);
            return Ok(results);
        }

        [HttpPost("{id}/process")]
        public async Task<ActionResult> ProcessSaveLog(string id)
        {
            var success = await _saveLogService.ProcessSaveLogAsync(id);
            if (!success)
                return BadRequest("Failed to process savelog");

            return Ok();
        }
    }
} 