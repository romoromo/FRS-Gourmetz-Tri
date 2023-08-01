import { Component, ViewChild, Inject, ElementRef, OnDestroy, OnInit } from '@angular/core';
import { fadeInOut } from '../../services/animations';
import { AccountService } from 'src/app/services/account.service';
import { Permission } from 'src/app/models/permission.model';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';
import { Floor } from 'src/app/models/floor.model';
import { InstitutionService } from 'src/app/services/institution.service';
import { Utilities } from 'src/app/services/utilities';
import { Institution } from 'src/app/models/institution.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FloorService } from 'src/app/services/floor.service';
import { Subscription } from 'rxjs';


@Component({
  selector: 'floor-editor',
  templateUrl: './floor-editor.component.html',
  styleUrls: ['./floor-editor.component.css'],
  animations: [fadeInOut]
})
export class FloorEditorComponent implements OnInit, OnDestroy{
  private subscription: Subscription = new Subscription();
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  public formResetToggle = true;
  private loadingIndicator = false;
  private isNewFloor = false;
  private originalFloor: Floor = new Floor();
  private floorEdit: Floor = new Floor();
  private allInstitutions: Institution[] = [];
  private validation = { code: false, label: false};
  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;

  @ViewChild('f')
  private form;

  @ViewChild('code')
  private code;

  @ViewChild('label')
  private label;

  constructor(private alertService: AlertService, private accountService: AccountService, private institutionService: InstitutionService,
    private floorService: FloorService,
    public dialogRef: MatDialogRef<FloorEditorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.floor) != typeof (undefined)) {
      if (data.floor.id) {
        this.editFloor(data.floor);
      } else {
        this.newFloor();
      }
    }
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


      if (!this.floorEdit.id) {
        this.floorService.newFloor(this.floorEdit).subscribe(floor => this.saveSuccessHelper(floor), error => this.saveFailedHelper(error));
      }
      else {
        this.floorService.updateFloor(this.floorEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
      }
  }

  private saveSuccessHelper(floor?: Floor) {
    if (floor)
      Object.assign(this.floorEdit, floor);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewFloor) {
      this.alertService.showMessage("Success", `Data \"${this.floorEdit.code}\" was created successfully`, MessageSeverity.success);
    }
    else {
      this.alertService.showMessage("Success", `Changes to data \"${this.floorEdit.code}\" was saved successfully`, MessageSeverity.success);
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
    this.floorEdit = new Floor();

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

  newFloor() {
    this.showValidationErrors = true;
    this.isNewFloor = true;
    this.floorEdit = new Floor();
    return this.floorEdit;
  }

  editFloor(floor: Floor) {
    if (floor) {
      this.isNewFloor = false;
      this.showValidationErrors = true;
      this.originalFloor = floor;
      this.floorEdit = new Floor();
      Object.assign(this.floorEdit, floor);

      return this.floorEdit;
    }
    else {
      return this.newFloor();
    }
  }

  get canManageFloors() {
    return true;//this.accountService.userHasPermission(Permission.manageFloorPermission);
  }

}
