import { Component, OnInit } from '@angular/core';
import { SaveLogService, SaveLog } from '../../services/savelog.service';

@Component({
  selector: 'app-savelog-list',
  template: `
    <div class="card">
      <div class="card-header">
        <h5 class="mb-0">SaveLogs</h5>
        <button class="btn btn-primary btn-sm mt-2" (click)="fileInput.click()">
          Upload New
        </button>
        <input #fileInput type="file" (change)="onFileSelected($event)" style="display: none">
      </div>
      <div class="card-body">
        <div class="list-group">
          <a *ngFor="let log of saveLogs"
             href="#"
             class="list-group-item list-group-item-action"
             [class.active]="selectedLog?.id === log.id"
             (click)="selectLog(log, $event)">
            <div class="d-flex w-100 justify-content-between">
              <h6 class="mb-1">{{log.fileName}}</h6>
              <small>{{log.uploadDate | date:'short'}}</small>
            </div>
            <p class="mb-1">{{formatFileSize(log.fileSize)}}</p>
            <small [class.text-success]="log.isProcessed"
                   [class.text-warning]="!log.isProcessed">
              {{log.processStatus}}
            </small>
          </a>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .card {
      height: calc(100vh - 100px);
      overflow-y: auto;
    }
    .card-header {
      position: sticky;
      top: 0;
      background: white;
      z-index: 1;
    }
  `]
})
export class SaveLogListComponent implements OnInit {
  saveLogs: SaveLog[] = [];
  selectedLog: SaveLog | null = null;

  constructor(private saveLogService: SaveLogService) { }

  ngOnInit() {
    this.loadSaveLogs();
  }

  loadSaveLogs() {
    this.saveLogService.getAllSaveLogs().subscribe(
      logs => this.saveLogs = logs
    );
  }

  selectLog(log: SaveLog, event: Event) {
    event.preventDefault();
    this.selectedLog = log;
    // TODO: Implement log selection handling
  }

  onFileSelected(event: Event) {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (file) {
      this.saveLogService.uploadSaveLog(file).subscribe(
        uploadedLog => {
          this.saveLogs = [...this.saveLogs, uploadedLog];
        }
      );
    }
  }

  formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
  }
} 