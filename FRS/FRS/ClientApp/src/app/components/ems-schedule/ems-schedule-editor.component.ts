import { Component, ViewChild, Input } from '@angular/core';
import { fadeInOut } from '../../services/animations';

import { AlertService, MessageSeverity } from '../../services/alert.service';
import { AccountService } from "../../services/account.service";
import { EmsScheduleService } from '../../services/ems-schedule.service';
import { EmsSchedule } from '../../models/ems-schedule.model';
import { Permission } from '../../models/permission.model';
import { PIBTemplateLocation } from '../../models/department.model';
import { PIBService } from '../../services/pib.service';
import { EmsProfile } from '../../models/ems-profile.model';
import { EmsProfileService } from '../../services/ems-profile.service';
import { Ems } from '../../models/ems.model';
import { EmsService } from '../../services/ems.service';
import { LocationService } from '../../services/location.service';
import { Location } from '../../models/location.model';

@Component({
  selector: 'ems-schedule-editor',
  templateUrl: './ems-schedule-editor.component.html',
  styleUrls: ['./ems-schedule-editor.component.css'],
  animations: [fadeInOut]
})
export class EmsScheduleEditorComponent {

  private isEditMode = false;
  private isNewEmsSchedule = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingEmsScheduleName: string;
  private emsScheduleEdit: EmsSchedule = new EmsSchedule();
  private selectedValues: { [key: string]: boolean; } = {};

  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;
  public disabledTime = false;

  private allLocations: Location[] = [];
  private allProfiles: EmsProfile[] = [];
  private allEmsGroups: Ems[] = [];


  @ViewChild('f')
  private form;

  @ViewChild('facilities')
  private facilities;

  @ViewChild('locationsSelector')
  private locationsSelector;

  @ViewChild('allStartTimesSelector')
  private allStartTimesSelector;

  @ViewChild('allEndTimesSelector')
  private allEndTimesSelector;

  @ViewChild('startTime')
  private startTime;

  @ViewChild('endTime')
  private endTime;

  @Input()
  isViewOnly: boolean;

  constructor(private alertService: AlertService, private accountService: AccountService, private emsScheduleService: EmsScheduleService, private pibService: PIBService, private emsProfileService: EmsProfileService, private locationService: LocationService, private emsService: EmsService) {

    this.locationService.getLocationsByInstitutionId(this.accountService.currentUser.institutionId)
      .subscribe(results => {
        this.allLocations = results;
        this.alertService.stopLoadingMessage();
      },
        error => {
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.`,
            MessageSeverity.error);
        });

    this.emsProfileService.getAllEmsProfiles()
      .subscribe(results => {
        this.allProfiles = results;
        this.alertService.stopLoadingMessage();
      },
        error => {
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving ems profiles.`,
            MessageSeverity.error);
        });

    this.emsService.getEmses()
      .subscribe(results => {
        this.allEmsGroups = results;
        this.alertService.stopLoadingMessage();
      },
        error => {
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving ems groups.`,
            MessageSeverity.error);
        });
  }



  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {

    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");

    if (this.isNewEmsSchedule) {
      this.emsScheduleService.newEmsSchedule(this.emsScheduleEdit).subscribe(emsSchedule => this.saveSuccessHelper(emsSchedule), error => this.saveFailedHelper(error));
    }
    else {
      this.emsScheduleService.updateEmsSchedule(this.emsScheduleEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }




  private saveSuccessHelper(emsSchedule?: EmsSchedule) {
    if (emsSchedule) {
      Object.assign(this.emsScheduleEdit, emsSchedule);
    }

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewEmsSchedule)
      this.alertService.showMessage("Success", `EMS Schedule \"${this.emsScheduleEdit.label}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to ems schedule \"${this.emsScheduleEdit.label}\" was saved successfully`, MessageSeverity.success);


    this.emsScheduleEdit = new EmsSchedule();
    this.resetForm();


    //if (!this.isNewEmsSchedule && this.accountService.currentUser.emsSchedules.some(r => r == this.editingEmsScheduleName))
    //    this.refreshLoggedInUser();

    if (this.changesSavedCallback)
      this.changesSavedCallback();
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
    this.emsScheduleEdit = new EmsSchedule();

    this.showValidationErrors = false;
    this.resetForm();

    this.alertService.resetStickyMessage();

    if (this.changesCancelledCallback)
      this.changesCancelledCallback();
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

  public toggleTime() {
    setTimeout(() => this.allStartTimesSelector.refresh());
    setTimeout(() => this.allEndTimesSelector.refresh());
  }


  newEmsSchedule() {
    this.isEditMode = true;
    this.isNewEmsSchedule = true;
    this.showValidationErrors = true;
    this.disabledTime = false;
    this.editingEmsScheduleName = null;
    this.selectedValues = {};
    this.emsScheduleEdit = new EmsSchedule();

    return this.emsScheduleEdit;
  }

  editEmsSchedule(emsSchedule: EmsSchedule) {
    this.isEditMode = true;
    if (emsSchedule) {
      this.isNewEmsSchedule = false;
      this.showValidationErrors = true;
      this.selectedValues = {};
      this.emsScheduleEdit = new EmsSchedule();
      Object.assign(this.emsScheduleEdit, emsSchedule);
      
      
      return this.emsScheduleEdit;
    }
    else {
      return this.newEmsSchedule();
    }
  }

  get canManageEmsSchedules() {
    return true;// || this.accountService.userHasPermission(Permission.manageEmsSchedulesPermission)
  }
}
