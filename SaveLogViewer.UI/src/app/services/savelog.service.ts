import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface SaveLog {
  id: string;
  fileName: string;
  uploadDate: Date;
  fileSize: number;
  isProcessed: boolean;
  processStatus: string;
}

export interface LogEntry {
  timestamp: Date;
  severity: string;
  processId: string;
  message: string;
  contextFolderId: string;
  taskflowId: string;
  studyUid: string;
  workflowId: string;
  isClientLog: boolean;
}

export interface SearchCriteria {
  saveLogId: string;
  contextFolderId?: string;
  studyUid?: string;
  workflowId?: string;
  processId?: string;
  startTime?: Date;
  endTime?: Date;
  searchText?: string;
  severity?: string[];
}

@Injectable({
  providedIn: 'root'
})
export class SaveLogService {
  private apiUrl = `${environment.apiUrl}/api/savelog`;

  constructor(private http: HttpClient) { }

  getAllSaveLogs(): Observable<SaveLog[]> {
    return this.http.get<SaveLog[]>(this.apiUrl);
  }

  getSaveLog(id: string): Observable<SaveLog> {
    return this.http.get<SaveLog>(`${this.apiUrl}/${id}`);
  }

  uploadSaveLog(file: File): Observable<SaveLog> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<SaveLog>(`${this.apiUrl}/upload`, formData);
  }

  searchLogs(criteria: SearchCriteria): Observable<LogEntry[]> {
    return this.http.post<LogEntry[]>(`${this.apiUrl}/search`, criteria);
  }

  getTimeline(id: string, startTime: Date, endTime: Date): Observable<LogEntry[]> {
    const params = {
      startTime: startTime.toISOString(),
      endTime: endTime.toISOString()
    };
    return this.http.get<LogEntry[]>(`${this.apiUrl}/${id}/timeline`, { params });
  }

  processSaveLog(id: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/process`, {});
  }
} 