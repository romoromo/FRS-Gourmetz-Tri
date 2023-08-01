import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Permission } from '../../../models/permission.model';
import { Institution } from 'src/app/models/institution.model';
import { MatDialog } from '@angular/material';
import { UserVehicle } from 'src/app/models/uservehicle.model';
import { UserVehicleEditorComponent } from './uservehicle-editor.component';
import { Subscription } from 'rxjs';

@Component({
  selector: 'user-vehicle-management',
  templateUrl: './uservehicle-management.component.html',
  styleUrls: ['./uservehicle-management.component.css']
})
export class UserVehiclesManagementComponent implements OnInit, AfterViewInit, OnDestroy {
  private subscription: Subscription = new Subscription();
  columns: any[] = [];
  rows: UserVehicle[] = [];
  rowsCache: UserVehicle[] = [];

  loadingIndicator: boolean;

  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    public dialog: MatDialog,
    private ref: ChangeDetectorRef) {
  }

  ngOnInit() {

    let gT = (key: string) => this.translationService.getTranslation(key);

    this.columns = [
      { prop: "index", name: '#', width: 50, cellTemplate: this.indexTemplate, canAutoResize: false },
      { prop: 'plateNumber', name: gT('userVehicles.management.PlateNumber') },
      { prop: 'userName', name: gT('userVehicles.management.Owner') },
      { prop: 'vehicleStatus', name: gT('userVehicles.management.Status') },
      { name: '', width: 250, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
    ];

    this.loadData();
  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  addEditVehicle(userVehicle?: UserVehicle) {
    let header = userVehicle ? "Edit Vehicle" : "New Vehicle";
    if (!userVehicle) {
      userVehicle = new UserVehicle();
    }

    var dialog = this.dialog.open(UserVehicleEditorComponent, {
      data: { header: header, userVehicle: userVehicle, isUserInfo: false },
      width: '400px'
    });

    dialog.afterClosed().subscribe(result => {
      this.loadData();
    });
  }

  deleteVehicle(row: UserVehicle) {
    this.alertService.showDialog('Are you sure you want to delete vehicle with plate no. \"' + row.plateNumber + '\"?', DialogType.confirm, () => this.deleteUserVehicleHelper(row));
  }


  deleteUserVehicleHelper(row: UserVehicle) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.subscription.add(this.accountService.deleteUserVehicle(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.rowsCache = this.rowsCache.filter(item => item !== row)
        this.rows = this.rows.filter(item => item !== row)
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the vehicle.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }


  ngAfterViewInit() {

  }


  loadData() {
    this.alertService.startLoadingMessage();
    this.loadingIndicator = true;

    this.subscription.add(this.accountService.getUserVehicles(null, null)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let userVehicles = results;

        userVehicles.forEach((uv, index, userVehicle) => {
          (<any>uv).index = index + 1;
        });


        this.rowsCache = [...userVehicles];
        this.rows = userVehicles;
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve vehicles from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }


  onSearchChanged(value: string) {
    this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.plateNumber, r.vehicleStatus));
  }

  approveVehicle(row: UserVehicle) {
    this.alertService.showDialog('Are you sure you want to approve the vehicle with plate no. \"' + row.plateNumber + '\"?', DialogType.confirm, () => {
      row.isApprove = true;
      this.rows = [...this.rows];
      this.rowsCache = this.rows;
      this.subscription.add(this.accountService.updateUserVehicle(row).subscribe(response => this.saveSuccessHelper(row, response), error => this.saveFailedHelper(error)));
    });

    
  }

  disapproveVehicle(row: UserVehicle) {
    this.alertService.showDialog('Are you sure you want to revoke the vehicle with plate no. \"' + row.plateNumber + '\"?', DialogType.confirm, () => {
      row.isApprove = false;
      row.vehicleStatus = 'PENDING';
      this.rows = [...this.rows];
      this.rowsCache = this.rows;
      this.subscription.add(this.accountService.updateUserVehicle(row).subscribe(response => this.saveSuccessHelper(row, response, false), error => this.saveFailedHelper(error)));
    });


  }

  private saveSuccessHelper(vehicle: UserVehicle, response: any, isApproved: boolean = true) {
    this.alertService.stopLoadingMessage();
    vehicle.vehicleStatus = isApproved ? "APPROVED" : "PENDING";
    this.alertService.showMessage("Success", `"Vehicle with plate no. '${vehicle.plateNumber} has been ${(isApproved ? "approved" : "revoked")}!`, MessageSeverity.success);
    this.ref.detectChanges();
  }

  private saveFailedHelper(error: any) {
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage("Save Error", "The below errors occured while saving your changes:", MessageSeverity.error);
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);
  }

  get canManageUserVehicles() {
    return this.accountService.userHasPermission(Permission.manageUserVehiclesPermission)
  }

}
