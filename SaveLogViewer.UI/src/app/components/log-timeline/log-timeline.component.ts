import { Component, OnInit, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { Chart, ChartConfiguration } from 'chart.js';
import { SaveLogService, LogEntry } from '../../services/savelog.service';

@Component({
  selector: 'app-log-timeline',
  template: `
    <div class="timeline-controls mb-2">
      <div class="btn-group">
        <button class="btn btn-sm btn-outline-secondary" (click)="zoomIn()">
          <i class="bi bi-zoom-in"></i> Zoom In
        </button>
        <button class="btn btn-sm btn-outline-secondary" (click)="zoomOut()">
          <i class="bi bi-zoom-out"></i> Zoom Out
        </button>
        <button class="btn btn-sm btn-outline-secondary" (click)="resetZoom()">
          <i class="bi bi-arrow-counterclockwise"></i> Reset
        </button>
      </div>
    </div>
    <canvas #timelineCanvas></canvas>
  `,
  styles: [`
    :host {
      display: block;
      width: 100%;
      height: 100%;
    }
    .timeline-controls {
      position: absolute;
      top: 10px;
      right: 10px;
      z-index: 1;
    }
  `]
})
export class LogTimelineComponent implements OnInit, AfterViewInit {
  @ViewChild('timelineCanvas') timelineCanvas!: ElementRef<HTMLCanvasElement>;
  private chart: Chart | null = null;
  private logEntries: LogEntry[] = [];

  constructor(private saveLogService: SaveLogService) { }

  ngOnInit() {
    // Subscribe to log selection changes
  }

  ngAfterViewInit() {
    this.initializeChart();
  }

  private initializeChart() {
    const ctx = this.timelineCanvas.nativeElement.getContext('2d');
    if (!ctx) return;

    const config: ChartConfiguration = {
      type: 'scatter',
      data: {
        datasets: [
          {
            label: 'Error',
            data: [],
            backgroundColor: 'rgba(255, 99, 132, 0.5)',
            borderColor: 'rgba(255, 99, 132, 1)',
          },
          {
            label: 'Warning',
            data: [],
            backgroundColor: 'rgba(255, 206, 86, 0.5)',
            borderColor: 'rgba(255, 206, 86, 1)',
          },
          {
            label: 'Info',
            data: [],
            backgroundColor: 'rgba(54, 162, 235, 0.5)',
            borderColor: 'rgba(54, 162, 235, 1)',
          }
        ]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        scales: {
          x: {
            type: 'time',
            time: {
              unit: 'minute'
            },
            title: {
              display: true,
              text: 'Time'
            }
          },
          y: {
            title: {
              display: true,
              text: 'Severity'
            },
            ticks: {
              callback: function(value) {
                return ['Error', 'Warning', 'Info'][value as number];
              }
            }
          }
        },
        plugins: {
          tooltip: {
            callbacks: {
              label: (context) => {
                const entry = this.logEntries[context.dataIndex];
                return `${entry.message} (PID: ${entry.processId})`;
              }
            }
          }
        },
        onClick: (event, elements) => {
          if (elements.length > 0) {
            const index = elements[0].index;
            const entry = this.logEntries[index];
            // TODO: Implement click handling to show log details
            console.log('Selected log entry:', entry);
          }
        }
      }
    };

    this.chart = new Chart(ctx, config);
  }

  updateTimeline(entries: LogEntry[]) {
    if (!this.chart) return;

    this.logEntries = entries;
    const datasets = [[], [], []]; // Error, Warning, Info

    entries.forEach(entry => {
      const point = {
        x: entry.timestamp,
        y: this.getSeverityValue(entry.severity)
      };

      datasets[this.getSeverityValue(entry.severity)].push(point);
    });

    this.chart.data.datasets.forEach((dataset, index) => {
      dataset.data = datasets[index];
    });

    this.chart.update();
  }

  private getSeverityValue(severity: string): number {
    switch (severity.toLowerCase()) {
      case 'error': return 0;
      case 'warning': return 1;
      default: return 2; // Info
    }
  }

  zoomIn() {
    if (!this.chart) return;
    const options = this.chart.options;
    if (!options.scales?.x) return;

    // Implement zoom in logic
  }

  zoomOut() {
    if (!this.chart) return;
    const options = this.chart.options;
    if (!options.scales?.x) return;

    // Implement zoom out logic
  }

  resetZoom() {
    if (!this.chart) return;
    const options = this.chart.options;
    if (!options.scales?.x) return;

    // Reset to original zoom level
    this.chart.resetZoom();
  }
} 