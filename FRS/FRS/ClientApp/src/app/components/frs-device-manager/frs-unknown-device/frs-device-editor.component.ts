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
import { EpaperDevice, EpaperTemplateLocation } from 'src/app/models/epaper.model';
import { EpaperService } from 'src/app/services/epaper.service';

@Component({
  selector: 'frs-device-editor',
  templateUrl: './frs-device-editor.component.html',
  styleUrls: ['./frs-device-editor.component.css']
})
export class EpaperDeviceEditorComponent {

  private isEditMode = false;
  private isNewEpaperDevice = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingEpaperDeviceCode: string;
  private epaperDeviceEdit: EpaperDevice = new EpaperDevice();
  private allLocations: any = [];
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

  constructor(private alertService: AlertService, private accountService: AccountService, private epaperDeviceService: EpaperService, private locationService: LocationService,
    public dialogRef: MatDialogRef<EpaperDeviceEditorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.epaperDevice) != typeof (undefined)) {

      if (!data.locations || data.locations.length == 0) {
        this.getLocations(true);
      }

      if (data.epaperDevice.id) {
        this.editEpaperDevice(data.epaperDevice, data.locations);
      } else {
        this.newEpaperDevice(data.locations);
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
      var locations = this.allLocations.filter(f => f.id == this.epaperDeviceEdit.location_id);
      if (locations && locations.length > 0) {
        this.epaperDeviceEdit.location_code = locations[0].name;
      }
      if (this.isNewEpaperDevice) {
        this.epaperDeviceService.newEpaperDevice(this.epaperDeviceEdit).subscribe(epaperDevice => this.saveSuccessHelper(epaperDevice), error => this.saveFailedHelper(error));
      }
      else {
        this.epaperDeviceService.updateEpaperDevice(this.epaperDeviceEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
      }
    }
  }




  private saveSuccessHelper(epaperDevice?: EpaperDevice) {
    if (epaperDevice)
      Object.assign(this.epaperDeviceEdit, epaperDevice);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewEpaperDevice)
      this.alertService.showMessage("Success", `EpaperDevice \"${this.epaperDeviceEdit.device_code}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to epaperDevice \"${this.epaperDeviceEdit.device_code}\" was saved successfully`, MessageSeverity.success);


    this.epaperDeviceEdit = new EpaperDevice();
    this.resetForm();


    //if (!this.isNewEpaperDevice && this.accountService.currentUser.epaperDevices.some(r => r == this.editingEpaperDeviceCode))
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
    this.epaperDeviceEdit = new EpaperDevice();

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


  newEpaperDevice(allLocations: Location[]) {
    this.isEditMode = true;
    this.isNewEpaperDevice = true;
    this.showValidationErrors = true;

    this.editingEpaperDeviceCode = null;
    this.allLocations = allLocations;
    this.selectedValues = {};
    this.epaperDeviceEdit = new EpaperDevice();
    return this.epaperDeviceEdit;
  }

  editEpaperDevice(epaperDevice: EpaperDevice, allLocations: Location[]) {
    this.isEditMode = true;
    this.allLocations = allLocations;
    if (epaperDevice) {
      this.isNewEpaperDevice = false;
      this.showValidationErrors = true;

      this.editingEpaperDeviceCode = epaperDevice.device_code;
      this.selectedValues = {};
      this.epaperDeviceEdit = new EpaperDevice();
      Object.assign(this.epaperDeviceEdit, epaperDevice);

      return this.epaperDeviceEdit;
    }
    else {
      return this.newEpaperDevice(allLocations);
    }
  }

  getLocations(isShowMessage: boolean) {
    if (isShowMessage) {
      this.alertService.startLoadingMessage("Loading locations...");
    }

    this.locationService.getLocations(null, null, this.accountService.currentUser.institutionId)
      .subscribe(results => {
        this.allLocations = results;
        this.alertService.stopLoadingMessage();
      },
        error => {
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.`,
            MessageSeverity.error);
        });
  }

  get canManageEpaperDevices() {
    return this.accountService.userHasPermission(Permission.manageEpaperDevicesPermission)
  }
}
