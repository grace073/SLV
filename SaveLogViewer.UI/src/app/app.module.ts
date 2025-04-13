import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { AppComponent } from './app.component';
import { SaveLogListComponent } from './components/savelog-list/savelog-list.component';
import { LogTimelineComponent } from './components/log-timeline/log-timeline.component';
import { SaveLogService } from './services/savelog.service';

@NgModule({
  declarations: [
    AppComponent,
    SaveLogListComponent,
    LogTimelineComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    NgbModule,
    RouterModule.forRoot([])
  ],
  providers: [SaveLogService],
  bootstrap: [AppComponent]
})
export class AppModule { } 