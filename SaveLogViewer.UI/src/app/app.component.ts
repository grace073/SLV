import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  template: `
    <nav class="navbar navbar-expand-lg navbar-dark bg-dark">
      <div class="container-fluid">
        <a class="navbar-brand" href="#">SaveLog Viewer</a>
      </div>
    </nav>

    <div class="container-fluid mt-3">
      <div class="row">
        <div class="col-md-3">
          <app-savelog-list></app-savelog-list>
        </div>
        <div class="col-md-9">
          <router-outlet></router-outlet>
        </div>
      </div>
    </div>

    <div class="timeline-container">
      <app-log-timeline></app-log-timeline>
    </div>
  `,
  styles: [`
    .timeline-container {
      position: fixed;
      bottom: 0;
      left: 0;
      right: 0;
      height: 200px;
      background: #f8f9fa;
      border-top: 1px solid #dee2e6;
      padding: 1rem;
      z-index: 1000;
    }
  `]
})
export class AppComponent {
  title = 'SaveLog Viewer';
} 