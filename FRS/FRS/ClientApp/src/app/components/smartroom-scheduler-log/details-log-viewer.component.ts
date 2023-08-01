import { Component, ViewChild, Input, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { AuditLog } from 'src/app/models/audit-log';
import { Filter, PagedResult } from 'src/app/models/sieve-filter.model';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';
import { AppTranslationService } from 'src/app/services/app-translation.service';
import { AuditService } from 'src/app/services/audit.service';
import { Utilities } from 'src/app/services/utilities';

@Component({
  selector: 'smartroom-scheduler-log-detail-viewer',
  templateUrl: './details-log-viewer.component.html',
  styleUrls: ['./details-log-viewer.component.css']
})
export class SmartRoomSchedulerDetailsViewerComponent implements OnInit{
  constructor(public dialogRef: MatDialogRef<SmartRoomSchedulerDetailsViewerComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
  }

  ngOnInit() {
  }

  private cancel() {
    this.dialogRef.close();
  }

  displayDetails(value) {
    return value ? value.replace(/(?:\r\n|\r|\n)/g, '<br>') : '';
  }
}
