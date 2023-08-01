import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, Inject, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { QueueTableMapService } from 'src/app/services/queue-table-map.service';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Utilities } from 'src/app/services/utilities';
import { QueueTableMap } from 'src/app/models/queue-table-map.model';
import { Permission } from 'src/app/models/permission.model';
import { AlertService, MessageSeverity, DialogType } from 'src/app/services/alert.service';
import { AppTranslationService } from 'src/app/services/app-translation.service';
import { AccountService } from 'src/app/services/account.service';
import { QueueTableMapEditorComponent } from './queue-table-map-editor.component';
import { Subscription } from 'rxjs';
import { AuthService } from '../../services/auth.service';
import { ConfigurationService } from 'src/app/services/configuration.service';
import { QueueTableMapGroup } from '../../models/queue-table-map-group.model';


@Component({
  selector: 'queue-table-maps-management',
  templateUrl: './queue-table-maps-management.component.html',
  styleUrls: ['./queue-table-maps-management.component.css']
})
export class QueueTableMapsManagementComponent implements OnInit, OnDestroy, AfterViewInit {
  private subscription: Subscription = new Subscription();
  columns: any[] = [];
  rows: QueueTableMapGroup[] = [];
  rowsCache: QueueTableMapGroup[] = [];
  allPermissions: Permission[] = [];
  editedQueueTableMap: QueueTableMap;
  sourceQueueTableMap: QueueTableMap;
  editingQueueTableMapName: { key: string };
  loadingIndicator: boolean;

  private signalRCoreconnection: signalR.HubConnection;



  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('queueTableMapEditor')
  queueTableMapEditor: QueueTableMapEditorComponent;
  header: string;
  groups: QueueTableMapGroup[];
    editedQueueTableMapGroup: QueueTableMapGroup;
    sourceQueueTableMapGroup: QueueTableMapGroup;
    token: any;
    expire: any;
    keyword: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private queueTableMapService: QueueTableMapService, public dialog: MatDialog, private authService: AuthService,
    private configurationService: ConfigurationService) {
  }

  openDialog(queueTableMap: QueueTableMapGroup): void {
    const dialogRef = this.dialog.open(QueueTableMapEditorComponent, {
      data: { header: this.header, queueTableMap: queueTableMap },
      width: '80vw',
      disableClose: true
    });

    this.subscription.add(dialogRef.afterClosed().subscribe(result => {
      this.loadData();
    }));
  }

  ngOnInit() {

    let gT = (key: string) => this.translationService.getTranslation(key);

    this.columns = [
      { prop: "index", name: '#', width: 50, cellTemplate: this.indexTemplate, canAutoResize: false },
      { prop: 'queueId', name: gT('queueTableMaps.management.QueueId'), },
      { prop: 'roomName', name: 'Room Name', },
      { prop: 'devices', name: 'Device Id',  },
      { prop: 'sts', name: 'Pair Id',  },

      { prop: 'cds', name: 'Current Display',  },
      { prop: 'lcs', name: 'Last Call',  },
      { prop: 'lrs', name: 'Last Return',  },
      //{ prop: 'stationId', name: gT('queueTableMaps.management.StationId'), width: 200 },
      //{ prop: 'currentDisplay', name: gT('queueTableMaps.management.CurrentDisplay'), width: 200 },
      //{ prop: 'lastCall', name: gT('queueTableMaps.management.LastCall'), width: 200 },
      //{ prop: 'lastReturn', name: gT('queueTableMaps.management.LastReturn'), width: 200 },

      { name: '', width: 150,  cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: true, sortable: false, draggable: false }
    ];

    if (!this.accountService.currentUser.institutionId || this.accountService.currentUser.institutionId == '0') {
      this.columns.splice(1, 0, { prop: 'institutionName', name: gT('roles.management.Institution'), width: 120 });
    }

    this.signalRCoreconnection = this.authService.signalRConnection(this.configurationService.baseUrl + "/hub/queue?device_id=" + "MonitoringTable", true, this.signalRCoreconnection);
    if (this.signalRCoreconnection != null) {
      this.signalRCoreconnection.on("RefreshTable", () => {
        this.loadData();
      });
    }

    this.loadData();
  }

  ngOnDestroy() {
    this.alertService.resetStickyMessage();
    this.subscription.unsubscribe();
  }



  ngAfterViewInit() {

    this.queueTableMapEditor.changesSavedCallback = () => {
      this.loadData();
      //this.editorModal.hide();
    };

    this.queueTableMapEditor.changesCancelledCallback = () => {
      this.editedQueueTableMap = null;
      this.sourceQueueTableMap = null;
      //this.editorModal.hide();
    };
  }


  //addNewQueueTableMapToList() {
  //  if (this.sourceQueueTableMap) {
  //    Object.assign(this.sourceQueueTableMap, this.editedQueueTableMap);

  //    let sourceIndex = this.rowsCache.indexOf(this.sourceQueueTableMap, 0);
  //    if (sourceIndex > -1)
  //      Utilities.moveArrayItem(this.rowsCache, sourceIndex, 0);

  //    sourceIndex = this.rows.indexOf(this.sourceQueueTableMap, 0);
  //    if (sourceIndex > -1)
  //      Utilities.moveArrayItem(this.rows, sourceIndex, 0);

  //    this.editedQueueTableMap = null;
  //    this.sourceQueueTableMap = null;
  //  }
  //  else {
  //    let queueTableMap = new QueueTableMap();
  //    Object.assign(queueTableMap, this.editedQueueTableMap);
  //    this.editedQueueTableMap = null;

  //    let maxIndex = 0;
  //    for (let r of this.rowsCache) {
  //      if ((<any>r).index > maxIndex)
  //        maxIndex = (<any>r).index;
  //    }

  //    (<any>queueTableMap).index = maxIndex + 1;

  //    this.rowsCache.splice(0, 0, queueTableMap);
  //    this.rows.splice(0, 0, queueTableMap);
  //    this.rows = [...this.rows];
  //  }
  //}




  loadData() {
    //this.alertService.startLoadingMessage();
    this.loadingIndicator = true;

    this.subscription.add(this.queueTableMapService.getQueueTableMapByInstitutionId(this.accountService.currentUser.institutionId)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let queueTableMaps = results[0];
        let permissions = results[1];

        queueTableMaps.forEach((queueTableMap, index, queueTableMaps) => {
          (<any>queueTableMap).index = index + 1;
        });

        this.groups = this.getGroups(queueTableMaps);


        this.rowsCache = [...this.groups];
        this.rows = this.groups;

        this.onSearchChanged(this.keyword);

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve data from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }

  getGroups(queueTableMaps: QueueTableMap[]): QueueTableMapGroup[] {
    let result: QueueTableMapGroup[] = [];

    const unique = [...new Set(queueTableMaps.map(item => item.queueId))];

    for (let [index, q] of unique.entries()) {
      let r: any = {
        index: index + 1,
        queueId: q,
        id: q,
        list: queueTableMaps.filter(item => item.queueId == q)
      }

      r.roomName = r.list && r.list[0] ? r.list[0].roomName : '';

      r.devices = r.list.map(function (elem) {
        return elem.deviceId || "-";
      }).join(` <hr> `);

      r.sts = r.list.map(function (elem) {
        return elem.stationId || "-";
      }).join(" <hr> ");

      r.cds = r.list.map(function (elem) {
        return elem.currentDisplay || "-";
      }).join(" <hr> ");

      r.cds = r.list.map(function (elem) {
        return elem.currentDisplay || "-";
      }).join(" <hr> ");

      r.lcs = r.list.map(function (elem) {
        return elem.lastCall || "-";
      }).join(" <hr> ");

      r.lrs = r.list.map(function (elem) {
        return elem.lastReturn || "-";
      }).join(" <hr> ");

      result.push(r);
    }


    return result;
  }


  onSearchChanged(value: string) {
    this.keyword = value;
    this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.queueId, r.devices));
  }


  onEditorModalHidden() {
    this.editingQueueTableMapName = null;
    this.queueTableMapEditor.resetForm(true);
  }


  newQueueTableMap() {
    this.editingQueueTableMapName = null;
    this.sourceQueueTableMapGroup = null;
    this.editedQueueTableMapGroup = this.queueTableMapEditor.newQueueTableMap();
    this.header = 'New';
    this.openDialog(this.editedQueueTableMapGroup);
    //this.editorModal.show();
  }


  editQueueTableMap(row: QueueTableMapGroup) {
    this.editingQueueTableMapName = { key: row.queueId };
    this.sourceQueueTableMapGroup = row;
    this.editedQueueTableMapGroup = this.queueTableMapEditor.editQueueTableMap(row);
    this.header = 'Edit';
    //this.editorModal.show();
    this.openDialog(this.editedQueueTableMapGroup);
  }

  deleteQueueTableMap(row: QueueTableMapGroup) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.queueId + '\"?', DialogType.confirm, () => this.deleteQueueTableMapHelper(row));
  }


  deleteQueueTableMapHelper(row: QueueTableMapGroup) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    for (let q of row.list) {
      this.subscription.add(this.queueTableMapService.deleteQueueTableMap(q.id)
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
  }

  openSimulator(queueTableMap: QueueTableMap): void {
    const dialogRef = this.dialog.open(QueueTableMapSimulator, {
      width: '80vw',
      data: { Queueid: queueTableMap.queueId, token: this.token, expire: this.expire }
    });

    dialogRef.afterClosed().subscribe(result => {
      this.token = result.token;
      this.expire = result.expire;
      if (!result || !result.Queueid) return;

      this.queueTableMapService.sendQueueSimulator(result)
        .subscribe(r => {
          this.loadData();
        },
          error => {
            this.alertService.stopLoadingMessage();
            this.loadingIndicator = false;

            this.alertService.showStickyMessage("Error", `${Utilities.getHttpResponseMessage(error)}`,
              MessageSeverity.error);
          });
    });
  }


  get canManageQueueTableMaps() {
    return true;//this.accountService.userHasPermission(Permission.manageQueueTableMapPermission);
  }
}

@Component({
  selector: 'queue-table-map-simulator',
  templateUrl: 'queue-table-map-simulator.html',
})
export class QueueTableMapSimulator {

  token: any;
    password: any;
    username: any;
  expire: any;

  

  constructor(private alertService: AlertService,
    public dialogRef: MatDialogRef<QueueTableMapSimulator>,
    @Inject(MAT_DIALOG_DATA) public data: any, private queueTableMapService: QueueTableMapService) {

    data.RoomInfo = data.RoomInfo || "RoomInfo";
    data.ServingInfo = data.ServingInfo || "ServingInfo";
    data.QueueNo = data.QueueNo || "A1234";
    data.DoctorInfo = data.DoctorInfo || "DoctorInfo";
    data.AssistantInfo = data.AssistantInfo || "AssistantInfo";
    data.CallAction = data.CallAction || "call";
    data.Timestamp = data.Timestamp || new Date().toLocaleString('sv-SE').replace(' ', 'T');
    this.token = data.token;
    this.expire = data.expire;
  }

  ngOnInit() {
    this.token = "not-needed";
  }

  getToken(username, password) {
    this.queueTableMapService.getToken(username, password)
      .subscribe(r => {
        this.token = r.token;
        this.expire = r.expire;
        if (!this.data) this.data = {};

        this.data.token = r.token;
        this.data.expire = r.expire;
      },
        error => {
          this.alertService.stopLoadingMessage();

          this.alertService.showStickyMessage("Error", `${Utilities.getHttpResponseMessage(error)}`,
            MessageSeverity.error);
        });
  }

  onNoClick(): void {
    this.dialogRef.close({ token: this.data.token, expire: this.data.expire });
  }

}
