import { Component, ViewChild, Input } from '@angular/core';
import { fadeInOut } from '../../services/animations';

import { AlertService, MessageSeverity } from '../../services/alert.service';
import { AccountService } from "../../services/account.service";
import { EmsProfileService } from '../../services/ems-profile.service';
import { EmsProfile } from '../../models/ems-profile.model';
import { Permission } from '../../models/permission.model';
import { ModuleService } from '../../services/module.service';
//import { PatientCommunicatorService } from '../../services/patient-communicator.service';
//import { CommunicatorRootMenu } from '../../models/communicator-menu.model';

interface Resolution {
  value: string;
  viewValue: string;
}

@Component({
  selector: 'ems-profile-editor',
  templateUrl: './ems-profile-editor.component.html',
  styleUrls: ['./ems-profile-editor.component.css'],
  animations: [fadeInOut]
})
export class EmsProfileEditorComponent {

  private isEditMode = false;
  private isNewEmsProfile = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingEmsProfileName: string;
  private emsProfileEdit: EmsProfile = new EmsProfile();
  private selectedValues: { [key: string]: boolean; } = {};

  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;
  public disabledTime = false;

  //menus: CommunicatorRootMenu[] = [];

  public modules: any;

  public ress: Resolution[] = [
    { value: '', viewValue: '' },
    { value: '1024x768', viewValue: '1024 x 768' },
    { value: '1280x720', viewValue: '1280 x 720' },
    { value: '1920x1080', viewValue: '1920 x 1080' },
    { value: '3840x2160', viewValue: '3840 x 2160' }
  ];

  rotations = ['', 'Left', 'Right', 'Normal', 'Inverted'];

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
    //moduleService: any;

  constructor(private alertService: AlertService, private accountService: AccountService, private emsProfileService: EmsProfileService, private moduleService: ModuleService
    //private menuService: PatientCommunicatorService
  ) {
    this.getMenus();

    this.moduleService.getModules()
      .subscribe(results => {
        this.modules = results;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving modules.\r\n"`,
            MessageSeverity.error);
        });
  }



  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {

    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");

    if (this.isNewEmsProfile) {
      this.emsProfileService.newEmsProfile(this.emsProfileEdit).subscribe(emsProfile => this.saveSuccessHelper(emsProfile), error => this.saveFailedHelper(error));
    }
    else {
      this.emsProfileService.updateEmsProfile(this.emsProfileEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }


  getMenus() {
    //this.menuService.getRootMenus(null, null)
    //  .subscribe(results => {
    //    this.menus = results;
    //  },
    //    error => {
    //      //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
    //      this.alertService.showStickyMessage("Get Error", `An error occured while retrieving menu records.\r\n"`,
    //        MessageSeverity.error);
    //    });
  }

  private saveSuccessHelper(emsProfile?: EmsProfile) {
    if (emsProfile) {
      Object.assign(this.emsProfileEdit, emsProfile);
    }

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewEmsProfile)
      this.alertService.showMessage("Success", `Ems Profile \"${this.emsProfileEdit.code}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to ems profile \"${this.emsProfileEdit.code}\" was saved successfully`, MessageSeverity.success);


    this.emsProfileEdit = new EmsProfile();
    this.resetForm();


    //if (!this.isNewEmsProfile && this.accountService.currentUser.emsProfiles.some(r => r == this.editingEmsProfileName))
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
    this.emsProfileEdit = new EmsProfile();

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


  newEmsProfile() {
    this.isEditMode = true;
    this.isNewEmsProfile = true;
    this.showValidationErrors = true;
    this.disabledTime = false;
    this.editingEmsProfileName = null;
    this.selectedValues = {};
    this.emsProfileEdit = new EmsProfile();

    return this.emsProfileEdit;
  }

  editEmsProfile(emsProfile: EmsProfile) {
    this.isEditMode = true;
    if (emsProfile) {
      this.isNewEmsProfile = false;
      this.showValidationErrors = true;
      this.selectedValues = {};
      this.emsProfileEdit = new EmsProfile();
      Object.assign(this.emsProfileEdit, emsProfile);
      
      
      return this.emsProfileEdit;
    }
    else {
      return this.newEmsProfile();
    }
  }

  get canManageEmsProfiles() {
    return true;// || this.accountService.userHasPermission(Permission.manageEmsProfilesPermission)
  }
}
