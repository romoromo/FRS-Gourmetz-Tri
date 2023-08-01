import { Component, ViewChild, Inject, ElementRef, OnDestroy, OnInit } from '@angular/core';
import { fadeInOut } from '../../services/animations';
import { AccountService } from 'src/app/services/account.service';
import { Permission } from 'src/app/models/permission.model';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';
import { QueueTableMap } from 'src/app/models/queue-table-map.model';
import { InstitutionService } from 'src/app/services/institution.service';
import { Utilities } from 'src/app/services/utilities';
import { Institution } from 'src/app/models/institution.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { QueueTableMapService } from 'src/app/services/queue-table-map.service';
import { Subscription } from 'rxjs';
import { QueueTableMapGroup } from '../../models/queue-table-map-group.model';
import { JsonHubProtocol } from '@aspnet/signalr';
import { DeviceService } from '../../services/device.service';
import { Device } from '../../models/device.model';
import { FormControl } from '@angular/forms';


@Component({
  selector: 'queue-table-map-editor',
  templateUrl: './queue-table-map-editor.component.html',
  styleUrls: ['./queue-table-map-editor.component.css'],
  animations: [fadeInOut]
})
export class QueueTableMapEditorComponent implements OnInit, OnDestroy{
  private subscription: Subscription = new Subscription();
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  public formResetToggle = true;
  private loadingIndicator = false;
  private isNewQueueTableMap = false;
  private originalQueueTableMap: QueueTableMapGroup = new QueueTableMapGroup();
  private queueTableMapEdit: QueueTableMapGroup = new QueueTableMapGroup();
  private allInstitutions: Institution[] = [];
  private validation = { code: false, label: false};
  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;

  /** control for the MatSelect filter keyword */
  public searchDevice: FormControl = new FormControl();

  @ViewChild('f')
  private form;

  @ViewChild('code')
  private code;

  @ViewChild('label')
  private label;
  devices: any;
  selectedDevice: Device = null;
    selectedPairId: any = null;

  constructor(private alertService: AlertService, private accountService: AccountService, private institutionService: InstitutionService,
    private queueTableMapService: QueueTableMapService, private deviceService: DeviceService,
    public dialogRef: MatDialogRef<QueueTableMapEditorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.queueTableMap) != typeof (undefined)) {
      if (data.queueTableMap.id) {
        this.editQueueTableMap(data.queueTableMap);
      } else {
        this.newQueueTableMap();
      }
    }

    this.deviceService.getDevicesLocationsInstitutions(-1, -1, this.accountService.currentUser.institutionId)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;
        this.devices = results[0];
      },
        error => {
          
        });
  }

  add() {
    if (!this.selectedDevice) return;

    if (!this.queueTableMapEdit.list) this.queueTableMapEdit.list = [];

    let n: any = {
      deviceId: this.selectedDevice.code,
      deviceIdentifier: this.selectedDevice.id,
      stationId: this.selectedPairId,
      isActive: true
    }

    this.queueTableMapEdit.list.push(n);

    this.selectedDevice = null;
    this.selectedPairId = null;
  }

  getObjStr(obj) {
    return JSON.stringify(obj);
  }

  ngOnInit() {
    this.alertService.resetStickyMessage();
  }

  ngOnDestroy() {
    this.alertService.resetStickyMessage();
    this.subscription.unsubscribe();
  }

  private save() {
      this.isSaving = true;
      this.alertService.startLoadingMessage("Saving changes...");


    for (let q of this.queueTableMapEdit.list) {
      q.queueId = this.queueTableMapEdit.queueId;
      q.roomName = this.queueTableMapEdit.roomName;
      if (!q.id) {
        this.queueTableMapService.newQueueTableMap(q).subscribe(queueTableMap => this.saveSuccessHelper(queueTableMap), error => this.saveFailedHelper(error));
      }
      else {
        this.queueTableMapService.updateQueueTableMap(q).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
      }
    }
  }

  private saveSuccessHelper(queueTableMap?: QueueTableMap) {
    if (queueTableMap)
      Object.assign(this.queueTableMapEdit, queueTableMap);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewQueueTableMap) {
      this.alertService.showMessage("Success", `Data \"${this.queueTableMapEdit.queueId}\" was created successfully`, MessageSeverity.success);
    }
    else {
      this.alertService.showMessage("Success", `Changes to data \"${this.queueTableMapEdit.queueId}\" was saved successfully`, MessageSeverity.success);
    }

    if (this.changesSavedCallback)
      this.changesSavedCallback();

    this.dialogRef.close();
  }

  private saveFailedHelper(error: any) {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage("Save Error", "The below errors occured while saving your changes:", MessageSeverity.error);
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);

    //if (this.changesFailedCallback)
    //  this.changesFailedCallback();

    //this.dialogRef.close();
  }


  private cancel() {
    this.queueTableMapEdit = new QueueTableMapGroup();

    this.showValidationErrors = false;
    this.resetForm();

    this.alertService.resetStickyMessage();

    if (this.changesCancelledCallback)
      this.changesCancelledCallback();

    this.dialogRef.close();
  }

  resetForm(replace = false) {

    if (!replace) {
      this.form.reset();
    }
    else {
      this.formResetToggle = false;

      setTimeout(() => {
        this.formResetToggle = true;
      });
    }
  }

  loadInstitutions() {
    this.institutionService.getInstitutions()
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.allInstitutions = results[0];

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve institutions from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  newQueueTableMap() {
    this.showValidationErrors = true;
    this.isNewQueueTableMap = true;
    this.queueTableMapEdit = new QueueTableMapGroup();
    return this.queueTableMapEdit;
  }

  editQueueTableMap(queueTableMapGroup: QueueTableMapGroup) {
    if (queueTableMapGroup) {
      this.isNewQueueTableMap = false;
      this.showValidationErrors = true;
      this.originalQueueTableMap = queueTableMapGroup;
      this.queueTableMapEdit = new QueueTableMapGroup();
      Object.assign(this.queueTableMapEdit, queueTableMapGroup);

      return this.queueTableMapEdit;
    }
    else {
      return this.newQueueTableMap();
    }
  }

  get canManageQueueTableMaps() {
    return true;//this.accountService.userHasPermission(Permission.manageQueueTableMapPermission);
  }

}
