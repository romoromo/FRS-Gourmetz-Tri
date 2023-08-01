import { Component, ViewChild, Input, Inject } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { Location } from '../../../models/location.model';
import { Institution } from '../../../models/institution.model';
import { FormControl, Validators } from '@angular/forms';
import { LocType } from 'src/app/models/enums';
import { LocationService } from 'src/app/services/location.service';
import { PIBDevice, PIBTemplate, PIBTemplateLocation } from 'src/app/models/department.model';
import { PIBService } from 'src/app/services/pib.service';

@Component({
  selector: 'pib-device-editor',
  templateUrl: './pib-device-editor.component.html',
  styleUrls: ['./pib-device-editor.component.css']
})
export class PIBDeviceEditorComponent {

  private isEditMode = false;
  private isNewPIBDevice = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingPIBDeviceCode: string;
  private pibDeviceEdit: PIBDevice = new PIBDevice();
  private allLocations: PIBTemplateLocation[] = [];
  private selectedValues: { [key: string]: boolean; } = {};

  fcLocation = new FormControl('', [Validators.required] );

  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;

  @ViewChild('locations')
  private locations;

  @ViewChild('locationsSelector')
  private locationsSelector;

  @Input()
  isViewOnly: boolean;

  constructor(private alertService: AlertService, private accountService: AccountService, private pibDeviceService: PIBService, private locationService: LocationService,
    public dialogRef: MatDialogRef<PIBDeviceEditorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.pibDevice) != typeof (undefined)) {

      if (!data.locations || data.locations.length == 0) {
        this.getLocations(true);
      }

      if (data.pibDevice.id) {
        this.editPIBDevice(data.pibDevice, data.locations);
      } else {
        this.newPIBDevice(data.locations);
      }
    }
  }



  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }

  private save() {
    if (this.form.form.valid && this.fcLocation.valid) {
      this.isSaving = true;
      this.alertService.startLoadingMessage("Saving changes...");
      var locations = this.allLocations.filter(f => f.location_id == this.pibDeviceEdit.location_id);
      if (locations && locations.length > 0) {
        this.pibDeviceEdit.location_code = locations[0].code;
      }
      if (this.isNewPIBDevice) {
        this.pibDeviceService.newPIBDevice(this.pibDeviceEdit).subscribe(pibDevice => this.saveSuccessHelper(pibDevice), error => this.saveFailedHelper(error));
      }
      else {
        this.pibDeviceService.updatePIBDevice(this.pibDeviceEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
      }
    }
  }




  private saveSuccessHelper(pibDevice?: PIBDevice) {
    if (pibDevice)
      Object.assign(this.pibDeviceEdit, pibDevice);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewPIBDevice)
      this.alertService.showMessage("Success", `PIBDevice \"${this.pibDeviceEdit.device_code}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to pibDevice \"${this.pibDeviceEdit.device_code}\" was saved successfully`, MessageSeverity.success);


    this.pibDeviceEdit = new PIBDevice();
    this.resetForm();


    //if (!this.isNewPIBDevice && this.accountService.currentUser.pibDevices.some(r => r == this.editingPIBDeviceCode))
    //    this.refreshLoggedInUser();

    if (this.changesSavedCallback)
      this.changesSavedCallback();

    this.dialogRef.close();
  }


  private refreshLoggedInUser() {
    this.accountService.refreshLoggedInUser()
      .subscribe(user => { },
        error => {
          this.alertService.resetStickyMessage();
          this.alertService.showStickyMessage("Refresh failed", "An error occured while refreshing logged in user information from the server", MessageSeverity.error);
        });
  }



  private saveFailedHelper(error: any) {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage("Save Error", "The below errors occured while saving your changes:", MessageSeverity.error);
    //this.alertService.showStickyMessage(error, null, MessageSeverity.error);

    if (this.changesFailedCallback)
      this.changesFailedCallback();

    //this.dialogRef.close();
  }


  private cancel() {
    this.pibDeviceEdit = new PIBDevice();

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


  newPIBDevice(allLocations: PIBTemplateLocation[]) {
    this.isEditMode = true;
    this.isNewPIBDevice = true;
    this.showValidationErrors = true;

    this.editingPIBDeviceCode = null;
    this.allLocations = allLocations;
    this.selectedValues = {};
    this.pibDeviceEdit = new PIBDevice();
    return this.pibDeviceEdit;
  }

  editPIBDevice(pibDevice: PIBDevice, allLocations: PIBTemplateLocation[]) {
    this.isEditMode = true;
    this.allLocations = allLocations;
    if (pibDevice) {
      this.isNewPIBDevice = false;
      this.showValidationErrors = true;

      this.editingPIBDeviceCode = pibDevice.device_code;
      this.selectedValues = {};
      this.pibDeviceEdit = new PIBDevice();
      Object.assign(this.pibDeviceEdit, pibDevice);

      return this.pibDeviceEdit;
    }
    else {
      return this.newPIBDevice(allLocations);
    }
  }

  getLocations(isShowMessage: boolean) {
    if (isShowMessage) {
      this.alertService.startLoadingMessage("Loading locations...");
    }
    this.pibDeviceService.getLocations()
      .subscribe(results => {
        this.allLocations = results[0];
        this.alertService.stopLoadingMessage();
      },
        error => {
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.`,
            MessageSeverity.error);
        });
  }

  get canManagePIBDevices() {
    return true; //this.accountService.userHasPermission(Permission.managePIBDevicesPermission)
  }
}
