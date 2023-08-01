import { Component, ViewChild, Input, ChangeDetectorRef, ViewEncapsulation, Inject } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { LocationService } from '../../../services/location.service';
import { Location } from '../../../models/location.model';
import { Permission } from '../../../models/permission.model';
import { Institution } from 'src/app/models/institution.model';
import { FacilityService } from 'src/app/services/facility.service';
import { Facility } from '../../../models/facility.model';
import { FormControl, Validators } from '@angular/forms';
import { FileService } from 'src/app/services/file.service';


@Component({
  selector: 'location-editor',
  templateUrl: './location-editor.component.html',
  styleUrls: ['./location-editor.component.css']
})
export class LocationEditorComponent {

  private isEditMode = false;
  private isNewLocation = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingLocationName: string;
  private locationEdit: Location = new Location();
  private allInstitutions: Institution[] = [];
  private facilitiesList: Facility[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public fileUploadResponse: { dbPath: '', fileId: null, fileName: '' };
  facilities = new FormControl('', [Validators.required]);
  institutions = new FormControl('', [Validators.required]);

  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;

  @ViewChild('f')
  private form;

  @ViewChild('locationTypes')
  private locationTypes;

  @ViewChild('locationTypesSelector')
  private locationTypesSelector;

  @ViewChild('institutionsSelector')
  private institutionsSelector;

  @Input()
  isViewOnly: boolean;

  constructor(private cdRef: ChangeDetectorRef, private alertService: AlertService, private accountService: AccountService, private locationService: LocationService, private facilityService: FacilityService,
    public dialogRef: MatDialogRef<LocationEditorComponent>,
    private fileService: FileService, @Inject(MAT_DIALOG_DATA) public data: any) {

    if (typeof (data.location) != typeof (undefined)) {
      if (data.location.id) {
        this.editLocation(data.location, data.institutions);
      } else {
        this.newLocation(data.institutions);
      }

      this.loadFacilities();
      //this.institutions.valueChanges.subscribe(value => {
      //  this.changeInstitutions(value);
      //});
    }


  }



  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");


    if (this.isNewLocation) {
      //this.locationEdit.institutionId = this.accountService.currentUser.institutionId;
      this.locationService.newLocation(this.locationEdit).subscribe(location => this.saveSuccessHelper(location), error => this.saveFailedHelper(error));
    }
    else {
      this.locationService.updateLocation(this.locationEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }

  private saveSuccessHelper(location?: Location) {
    if (location)
      Object.assign(this.locationEdit, location);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewLocation)
      this.alertService.showMessage("Success", `Location \"${this.locationEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to location \"${this.locationEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.locationEdit = new Location();

    this.resetForm();


    //if (!this.isNewLocation && this.accountService.currentUser.locations.some(r => r == this.editingLocationName))
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
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);

    if (this.changesFailedCallback)
      this.changesFailedCallback();
  }


  private cancel() {
    this.locationEdit = new Location();
    
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


  newLocation(allInstitutions: Institution[]) {
    this.isEditMode = true;
    this.isNewLocation = true;
    this.showValidationErrors = true;

    this.editingLocationName = null;
    this.setInstitutions(allInstitutions);
    this.selectedValues = {};
    this.locationEdit = new Location();
    //this.locationEdit.institutionId = this.accountService.currentUser.institutionId;
    return this.locationEdit;
  }

  editLocation(location: Location, allInstitutions: Institution[]) {
    this.isEditMode = true;
    if (location) {
      this.isNewLocation = false;
      this.showValidationErrors = true;

      this.editingLocationName = location.name;
      this.selectedValues = {};
      this.setInstitutions(allInstitutions);
      this.locationEdit = new Location();
      Object.assign(this.locationEdit, location);

      return this.locationEdit;
    }
    else {
      return this.newLocation(allInstitutions);
    }
  }

  private setInstitutions(allInstitutions: Institution[]) {

    this.allInstitutions = allInstitutions ? [...allInstitutions] : [];

    if (allInstitutions == null || this.allInstitutions.length != allInstitutions.length)
      setTimeout(() => this.institutionsSelector.refresh());
  }

  loadFacilities() {
    this.facilityService.getFacilitiesAndFacilityTypes(null, null, this.accountService.currentUser.institutionId)
      .subscribe(results => {
        let facilities = results[0];
        this.facilitiesList = facilities;
      },
        error => {
          this.alertService.resetStickyMessage();
          this.alertService.showStickyMessage("getFacilitiesByInstitutionId failed", "An error occured while trying to get equipments from the server", MessageSeverity.error);
        });
  }


  changeInstitutions(id) {
    this.facilityService.getFacilitiesByInstitutionId(id)
      .subscribe(results => {
        let facilities = results[0];
        this.facilitiesList = facilities;
      },
        error => {
          this.alertService.resetStickyMessage();
          this.alertService.showStickyMessage("getFacilitiesByInstitutionId failed", "An error occured while trying to get equipments from the server", MessageSeverity.error);
        });
  }

  setLocationTypeId(id) {
    this.locationEdit.locationTypeId = id;
  }

  public uploadFinished = (event) => {
    this.fileUploadResponse = event;
    this.locationEdit.filePath = this.fileUploadResponse ? this.fileUploadResponse.dbPath : null;
  }

  getFileImage(path) {
    return this.fileService.getFile(path);
  }

  get canManageLocations() {
    return this.accountService.userHasPermission(Permission.manageLocationsPermission)
  }
}
