import { Component, ViewChild, Input, ChangeDetectorRef, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { FacilityService } from '../../../services/facility.service';
import { Facility } from '../../../models/facility.model';
import { Permission } from '../../../models/permission.model';
import { Institution } from 'src/app/models/institution.model';
import { FacilityTypeService } from 'src/app/services/facility-type.service';
import { FileService } from 'src/app/services/file.service';
import { AuthService } from 'src/app/services/auth.service';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormControl, Validators } from '@angular/forms';


@Component({
  selector: 'facility-editor',
  templateUrl: './facility-editor.component.html',
  styleUrls: ['./facility-editor.component.css']
})
export class FacilityEditorComponent {

  private isEditMode = false;
  private isNewFacility = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingFacilityName: string;
  private facilityEdit: Facility = new Facility();
  private allInstitutions: Institution[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  fcFacilityType = new FormControl('', [Validators.required]);
  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;

  @ViewChild('facilityTypes')
  private facilityTypes;

  @ViewChild('facilityTypesSelector')
  private facilityTypesSelector;

  @ViewChild('institutionsSelector')
  private institutionsSelector;

  @Input()
  isViewOnly: boolean;

  public fileUploadResponse: { dbPath: '', fileId: null, fileName: '' };
  constructor(private cdRef: ChangeDetectorRef, private alertService: AlertService, private accountService: AccountService, private facilityService: FacilityService, private facilityTypeService: FacilityTypeService,
    private fileService: FileService, private authService: AuthService,
    public dialogRef: MatDialogRef<FacilityEditorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.facility) != typeof (undefined)) {
      this.allInstitutions = data.allInstitutions;
      if (data.facility.id) {
        this.editFacility(data.facility, data.allInstitutions);
      } else {
        this.newFacility(data.allInstitutions);
      }
    }
  }



  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    if (this.form.form.valid) {
      this.isSaving = true;
      this.alertService.startLoadingMessage("Saving changes...");

      this.facilityEdit.filePath = this.fileUploadResponse ? this.fileUploadResponse.dbPath : null;
      this.facilityEdit.institutionId = parseInt(this.authService.currentUser.institutionId);
      if (this.isNewFacility) {
        this.facilityService.newFacility(this.facilityEdit).subscribe(facility => this.saveSuccessHelper(facility), error => this.saveFailedHelper(error));
      }
      else {
        this.facilityService.updateFacility(this.facilityEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
      }
    }
  }




  private saveSuccessHelper(facility?: Facility) {
    if (facility)
      Object.assign(this.facilityEdit, facility);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewFacility)
      this.alertService.showMessage("Success", `Facility \"${this.facilityEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to facility \"${this.facilityEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.facilityEdit = new Facility();
    this.resetForm();


    //if (!this.isNewFacility && this.accountService.currentUser.facilities.some(r => r == this.editingFacilityName))
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
    this.facilityEdit = new Facility();

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


  newFacility(allInstitutions: Institution[]) {
    this.isEditMode = true;
    this.isNewFacility = true;
    this.showValidationErrors = true;

    this.editingFacilityName = null;
    this.allInstitutions = allInstitutions;
    this.selectedValues = {};
    this.facilityEdit = new Facility();

    return this.facilityEdit;
  }

  editFacility(facility: Facility, allInstitutions: Institution[]) {
    this.isEditMode = true;
    if (facility) {
      this.isNewFacility = false;
      this.showValidationErrors = true;

      this.editingFacilityName = facility.name;
      this.selectedValues = {};
      //this.setFacilityTypes(facility, allFacilityTypes);
      //this.setInstitutions(facility, allInstitutions);
      this.facilityEdit = new Facility();
      Object.assign(this.facilityEdit, facility);

      return this.facilityEdit;
    }
    else {
      return this.newFacility(allInstitutions);
    }
  }

  private setInstitutions(facility: Facility, allInstitutions: Institution[]) {

    this.allInstitutions = allInstitutions ? [...allInstitutions] : [];

    if (allInstitutions == null || this.allInstitutions.length != allInstitutions.length)
      setTimeout(() => this.institutionsSelector.refresh());
  }

  public uploadFinished = (event) => {
    this.fileUploadResponse = event;
    this.facilityEdit.filePath = this.fileUploadResponse ? this.fileUploadResponse.dbPath : null;
  }

  getFileImage(path) {
    return this.fileService.getFile(path);
  }

  get canManageFacilities() {
    return this.accountService.userHasPermission(Permission.manageFacilitiesPermission)
  }
}
