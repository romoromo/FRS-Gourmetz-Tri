import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { InstitutionService } from "../../../services/institution.service";
import { Institution } from '../../../models/institution.model';
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';


@Component({
  selector: 'institution-editor',
  templateUrl: './institution-editor.component.html',
  styleUrls: ['./institution-editor.component.css']
})
export class InstitutionEditorComponent {

  private isNewInstitution = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingInstitutionName: string;
  private institutionEdit: Institution = new Institution();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  private restrictions = ['DAILY', 'WEEKLY', 'MONTHLY', 'YEARLY'];
  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;



  constructor(private alertService: AlertService, private institutionService: InstitutionService, private accountService: AccountService,
    public dialogRef: MatDialogRef<InstitutionEditorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.institution) != typeof (undefined)) {
      if (data.institution.id) {
        this.editInstitution(data.institution);
      } else {
        this.newInstitution();
      }
    }
  }



  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");

    if (this.isNewInstitution) {
      this.institutionService.newInstitution(this.institutionEdit).subscribe(institution => this.saveSuccessHelper(institution), error => this.saveFailedHelper(error));
    }
    else {
      this.institutionService.updateInstitution(this.institutionEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }




  private saveSuccessHelper(institution?: Institution) {
    if (institution)
      Object.assign(this.institutionEdit, institution);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewInstitution)
      this.alertService.showMessage("Success", `Facility Type \"${this.institutionEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to institution \"${this.institutionEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.institutionEdit = new Institution();
    this.resetForm();


    //if (!this.isNewInstitution && this.accountService.currentUser.facilities.some(r => r == this.editingInstitutionName))
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
    this.institutionEdit = new Institution();

    this.showValidationErrors = false;
    this.resetForm();

    this.alertService.resetStickyMessage();

    if (this.changesCancelledCallback)
      this.changesCancelledCallback();

    this.dialogRef.close();
  }

  private toggleGroup(groupName: string) {
    let firstMemberValue: boolean;

    this.allPermissions.forEach(p => {
      if (p.groupName != groupName)
        return;

      if (firstMemberValue == null)
        firstMemberValue = this.selectedValues[p.value] == true;

      this.selectedValues[p.value] = !firstMemberValue;
    });
  }


  private getSelectedPermissions() {
    return this.allPermissions.filter(p => this.selectedValues[p.value] == true);
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


  newInstitution() {
    this.isNewInstitution = true;
    this.showValidationErrors = true;

    this.editingInstitutionName = null;
    this.selectedValues = {};
    this.institutionEdit = new Institution();

    return this.institutionEdit;
  }

  editInstitution(institution: Institution) {
    if (institution) {
      this.isNewInstitution = false;
      this.showValidationErrors = true;

      this.editingInstitutionName = institution.name;
      this.selectedValues = {};
      this.institutionEdit = new Institution();
      Object.assign(this.institutionEdit, institution);

      return this.institutionEdit;
    }
    else {
      return this.newInstitution();
    }
  }



  get canManageInstitutions() {
    return this.accountService.userHasPermission(Permission.manageInstitutionsPermission)
  }
}
