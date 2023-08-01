import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, Inject, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { OccupancyLogService } from 'src/app/services/occupancy-log.service';
import { MatDialog } from '@angular/material';
import { Utilities } from 'src/app/services/utilities';
import { OccupancyLog } from 'src/app/models/occupancy-log.model';
import { Permission } from 'src/app/models/permission.model';
import { AlertService, MessageSeverity, DialogType } from 'src/app/services/alert.service';
import { AppTranslationService } from 'src/app/services/app-translation.service';
import { AccountService } from 'src/app/services/account.service';
import { OccupancyLogEditorComponent } from './occupancy-log-editor.component';
import { Subscription } from 'rxjs';


@Component({
  selector: 'occupancy-logs-management',
  templateUrl: './occupancy-logs-management.component.html',
  styleUrls: ['./occupancy-logs-management.component.css']
})
export class OccupancyLogsManagementComponent implements OnInit, OnDestroy, AfterViewInit {
  private subscription: Subscription = new Subscription();
  columns: any[] = [];
  rows: OccupancyLog[] = [];
  rowsCache: OccupancyLog[] = [];
  allPermissions: Permission[] = [];
  editedOccupancyLog: OccupancyLog;
  sourceOccupancyLog: OccupancyLog;
  editingOccupancyLogName: { key: string };
  loadingIndicator: boolean;



  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('occupancyLogEditor')
  occupancyLogEditor: OccupancyLogEditorComponent;
  header: string;
    deviceId: string;
    sensorId: string;
    status: string;
    startTime: string;
    endTime: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private occupancyLogService: OccupancyLogService, public dialog: MatDialog, private ref: ChangeDetectorRef) {

    
  }

  openDialog(occupancyLog: OccupancyLog): void {
    const dialogRef = this.dialog.open(OccupancyLogEditorComponent, {
      data: { header: this.header, occupancyLog: occupancyLog },
      width: '400px',
      disableClose: true
    });

    this.subscription.add(dialogRef.afterClosed().subscribe(result => {
      this.loadData();
    }));
  }

  getDateStr(m: Date) {
    if (!m) return '';
    return m.getFullYear() + "-" +
      ("0" + (m.getMonth() + 1)).slice(-2) + "-" +
      ("0" + m.getDate()).slice(-2)
      + "T" + "00:00";
  }

  ngOnInit() {
    this.startTime = this.getDateStr(new Date());
    //this.startTime.setHours(0, 0, 0, 0);

    let d = new Date();
    d.setDate(d.getDate() + 1);

    this.endTime = this.getDateStr(d);
    

    this.ref.detectChanges();
    this.ref.markForCheck();

    let gT = (key: string) => this.translationService.getTranslation(key);

    this.columns = [
      { prop: "index", name: '#', width: 50, cellTemplate: this.indexTemplate, canAutoResize: false },
      { prop: 'datetime', name: gT('occupancyLogs.management.Datetime') },
      { prop: 'deviceId', name: gT('occupancyLogs.management.DeviceId') },
      { prop: 'sensorId', name: gT('occupancyLogs.management.SensorId') },
      { prop: 'status', name: gT('occupancyLogs.management.Status') },
      { name: '', width: 150, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
    ];

    if (!this.accountService.currentUser.institutionId || this.accountService.currentUser.institutionId == '0') {
      this.columns.splice(1, 0, { prop: 'institutionName', name: gT('roles.management.Institution'), width: 120 });
    }
    this.loadData();
  }

  ngOnDestroy() {
    this.alertService.resetStickyMessage();
    this.subscription.unsubscribe();
  }



  ngAfterViewInit() {

    this.occupancyLogEditor.changesSavedCallback = () => {
      this.addNewOccupancyLogToList();
      //this.editorModal.hide();
    };

    this.occupancyLogEditor.changesCancelledCallback = () => {
      this.editedOccupancyLog = null;
      this.sourceOccupancyLog = null;
      //this.editorModal.hide();
    };
  }


  addNewOccupancyLogToList() {
    if (this.sourceOccupancyLog) {
      Object.assign(this.sourceOccupancyLog, this.editedOccupancyLog);

      let sourceIndex = this.rowsCache.indexOf(this.sourceOccupancyLog, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rowsCache, sourceIndex, 0);

      sourceIndex = this.rows.indexOf(this.sourceOccupancyLog, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rows, sourceIndex, 0);

      this.editedOccupancyLog = null;
      this.sourceOccupancyLog = null;
    }
    else {
      let occupancyLog = new OccupancyLog();
      Object.assign(occupancyLog, this.editedOccupancyLog);
      this.editedOccupancyLog = null;

      let maxIndex = 0;
      for (let r of this.rowsCache) {
        if ((<any>r).index > maxIndex)
          maxIndex = (<any>r).index;
      }

      (<any>occupancyLog).index = maxIndex + 1;

      this.rowsCache.splice(0, 0, occupancyLog);
      this.rows.splice(0, 0, occupancyLog);
      this.rows = [...this.rows];
    }
  }




  loadData() {
    //this.alertService.startLoadingMessage();
    this.loadingIndicator = true;

    this.subscription.add(this.occupancyLogService.getOccupancyLogFilter(this.deviceId, this.sensorId, this.status, this.startTime, this.endTime)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let occupancyLogs = results[0];
        let permissions = results[1];

        occupancyLogs.forEach((occupancyLog, index, occupancyLogs) => {
          (<any>occupancyLog).index = index + 1;
        });


        this.rowsCache = [...occupancyLogs];
        this.rows = occupancyLogs;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve data from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }


  onSearchChanged(value: string) {
    this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.datetime, r.deviceId, r.sensorId, r.status));
  }


  onEditorModalHidden() {
    this.editingOccupancyLogName = null;
    this.occupancyLogEditor.resetForm(true);
  }


  newOccupancyLog() {
    this.editingOccupancyLogName = null;
    this.sourceOccupancyLog = null;
    this.editedOccupancyLog = this.occupancyLogEditor.newOccupancyLog();
    this.header = 'New';
    this.openDialog(this.editedOccupancyLog);
    //this.editorModal.show();
  }


  editOccupancyLog(row: OccupancyLog) {
    this.editingOccupancyLogName = { key: row.datetime.toString() };
    this.sourceOccupancyLog = row;
    this.editedOccupancyLog = this.occupancyLogEditor.editOccupancyLog(row);
    this.header = 'Edit';
    //this.editorModal.show();
    this.openDialog(this.editedOccupancyLog);
  }

  deleteOccupancyLog(row: OccupancyLog) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.datetime + '\"?', DialogType.confirm, () => this.deleteOccupancyLogHelper(row));
  }


  deleteOccupancyLogHelper(row: OccupancyLog) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.subscription.add(this.occupancyLogService.deleteOccupancyLog(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.rowsCache = this.rowsCache.filter(item => item !== row)
        this.rows = this.rows.filter(item => item !== row)
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the data.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }


  get canManageOccupancyLogs() {
    return true;//this.accountService.userHasPermission(Permission.manageOccupancyLogPermission);
  }

}
