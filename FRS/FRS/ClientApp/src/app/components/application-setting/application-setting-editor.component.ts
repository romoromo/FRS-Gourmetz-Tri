import { Component, ViewChild, Inject, ElementRef, OnDestroy, OnInit } from '@angular/core';
import { fadeInOut } from '../../services/animations';
import { AccountService } from 'src/app/services/account.service';
import { Permission } from 'src/app/models/permission.model';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';
import { ApplicationSetting } from 'src/app/models/application-setting.model';
import { InstitutionService } from 'src/app/services/institution.service';
import { Utilities } from 'src/app/services/utilities';
import { Institution } from 'src/app/models/institution.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { ApplicationSettingService } from 'src/app/services/application-setting.service';
import { Subscription } from 'rxjs';


@Component({
  selector: 'application-setting-editor',
  templateUrl: './application-setting-editor.component.html',
  styleUrls: ['./application-setting-editor.component.css'],
  animations: [fadeInOut]
})
export class ApplicationSettingEditorComponent implements OnInit, OnDestroy{
  private subscription: Subscription = new Subscription();
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  public formResetToggle = true;
  private loadingIndicator = false;
  private isNewApplicationSetting = false;
  private originalApplicationSetting: ApplicationSetting = new ApplicationSetting();
  private applicationSettingEdit: ApplicationSetting = new ApplicationSetting();
  private allInstitutions: Institution[] = [];
  private validation = { key: false, value: false, description: false };
  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;

  @ViewChild('f')
  private form;

  @ViewChild('key')
  private key;

  @ViewChild('value')
  private value;

  @ViewChild('description')
  private description;

  constructor(private alertService: AlertService, private accountService: AccountService, private institutionService: InstitutionService,
    private applicationSettingService: ApplicationSettingService,
    public dialogRef: MatDialogRef<ApplicationSettingEditorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.applicationSetting) != typeof (undefined)) {
      if (data.applicationSetting.id) {
        this.editApplicationSetting(data.applicationSetting);
      } else {
        this.newApplicationSetting();
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

  private validate() {
    this.validation = { key: false, value: false, description: false };
    if (this.applicationSettingEdit.key && this.applicationSettingEdit.key.trim().length == 0) {
      this.validation.key = true;
      this.key.control.setErrors({});
    }

    if (this.applicationSettingEdit.value && this.applicationSettingEdit.value.trim().length == 0) {
      this.validation.value = true;
      this.value.control.setErrors({});
    }

    if (this.applicationSettingEdit.description && this.applicationSettingEdit.description.trim().length == 0) {
      this.validation.description = true;
      this.description.control.setErrors({});
    }

    if (this.validation.key || this.validation.value || this.validation.description) {
      this.showValidationErrors = true;
      return false;
    }

    return true;
  }

  private save() {
      this.isSaving = true;
      this.alertService.startLoadingMessage("Saving changes...");


      if (!this.applicationSettingEdit.id) {
        this.applicationSettingService.newApplicationSetting(this.applicationSettingEdit).subscribe(applicationSetting => this.saveSuccessHelper(applicationSetting), error => this.saveFailedHelper(error));
      }
      else {
        this.applicationSettingService.updateApplicationSetting(this.applicationSettingEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
      }
  }

  private saveSuccessHelper(applicationSetting?: ApplicationSetting) {
    if (applicationSetting)
      Object.assign(this.applicationSettingEdit, applicationSetting);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewApplicationSetting) {
      this.alertService.showMessage("Success", `Application Setting \"${this.applicationSettingEdit.key}\" was created successfully`, MessageSeverity.success);
    }
    else {
      this.alertService.showMessage("Success", `Changes to application setting \"${this.applicationSettingEdit.key}\" was saved successfully`, MessageSeverity.success);
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
    this.applicationSettingEdit = new ApplicationSetting();

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

  newApplicationSetting() {
    this.showValidationErrors = true;
    this.isNewApplicationSetting = true;
    this.applicationSettingEdit = new ApplicationSetting();
    this.applicationSettingEdit.institutionId = this.accountService.currentUser.institutionId;
    return this.applicationSettingEdit;
  }

  editApplicationSetting(applicationSetting: ApplicationSetting) {
    if (applicationSetting) {
      this.isNewApplicationSetting = false;
      this.showValidationErrors = true;
      this.originalApplicationSetting = applicationSetting;
      this.applicationSettingEdit = new ApplicationSetting();
      Object.assign(this.applicationSettingEdit, applicationSetting);

      return this.applicationSettingEdit;
    }
    else {
      return this.newApplicationSetting();
    }
  }

  get canManageApplicationSettings() {
    return true;//this.accountService.userHasPermission(Permission.manageApplicationSettingPermission);
  }

}
