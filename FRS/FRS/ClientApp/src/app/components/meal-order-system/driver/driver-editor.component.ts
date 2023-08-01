import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Driver } from 'src/app/models/meal-order/driver.model';
import { DeliveryService } from 'src/app/services/meal-order/delivery.service';


@Component({
  selector: 'driver-editor',
  templateUrl: './driver-editor.component.html',
  styleUrls: ['./driver-editor.component.css']
})
export class DriverEditorComponent {

  private isNewDriver = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingDriverName: string;
  private driverEdit: Driver = new Driver();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private deliveryService: DeliveryService, private accountService: AccountService,
    public dialogRef: MatDialogRef<DriverEditorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.driver) != typeof (undefined)) {
      if (data.driver.id) {
        this.editDriver(data.driver);
      } else {
        this.newDriver();
      }
    }
  }


  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    this.driverEdit.institutionId = this.accountService.currentUser.institutionId;
    if (this.isNewDriver) {
      this.deliveryService.newDriver(this.driverEdit).subscribe(driver => this.saveSuccessHelper(driver), error => this.saveFailedHelper(error));
    }
    else {
      this.deliveryService.updateDriver(this.driverEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }


  private saveSuccessHelper(driver?: Driver) {
    if (driver)
      Object.assign(this.driverEdit, driver);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewDriver)
      this.alertService.showMessage("Success", `Driver \"${this.driverEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to driver type \"${this.driverEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.driverEdit = new Driver();
    this.resetForm();


    //if (!this.isNewMealType && this.accountService.currentUser.facilities.some(r => r == this.editingMealTypeCode))
    //    this.refreshLoggedInUser();

    if (this.changesSavedCallback)
      this.changesSavedCallback();

    this.dialogRef.close();
  }


  private saveFailedHelper(error: any) {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage("Save Error", "The below errors occured while saving your changes:", MessageSeverity.error);
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);

    if (this.changesFailedCallback)
      this.changesFailedCallback();
  }


  private cancel() {
    this.driverEdit = new Driver();

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


  newDriver() {
    this.isNewDriver = true;
    this.showValidationErrors = true;

    this.editingDriverName = null;
    this.selectedValues = {};
    this.driverEdit = new Driver();

    return this.driverEdit;
  }

  editDriver(driver: Driver) {
    if (driver) {
      this.isNewDriver = false;
      this.showValidationErrors = true;

      this.editingDriverName = driver.name;
      this.selectedValues = {};
      this.driverEdit = new Driver();
      Object.assign(this.driverEdit, driver);

      return this.driverEdit;
    }
    else {
      return this.newDriver();
    }
  }

  get canManageDrivers() {
    return true; //this.accountService.userHasPermission(Permission.manageMealTypesPermission)
  }
}
