import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { EmsService } from "../../../services/ems.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Ems } from 'src/app/models/ems.model';


@Component({
  selector: 'ems-editor',
  templateUrl: './ems-editor.component.html',
  styleUrls: ['./ems-editor.component.css']
})
export class EmsEditorComponent {

  private isNewEms = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingEmsName: string;
  private emsEdit: Ems = new Ems();
  private allPermissions: Permission[] = [];
  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;



  constructor(private alertService: AlertService, private emsService: EmsService, private accountService: AccountService,
    public dialogRef: MatDialogRef<EmsEditorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.ems) != typeof (undefined)) {
      if (data.ems.id) {
        this.editEms(data.ems);
      } else {
        this.newEms();
      }
    }
  }



  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");

    if (this.isNewEms) {
      this.emsService.newEms(this.emsEdit).subscribe(ems => this.saveSuccessHelper(ems), error => this.saveFailedHelper(error));
    }
    else {
      this.emsService.updateEms(this.emsEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }




  private saveSuccessHelper(ems?: Ems) {
    if (ems)
      Object.assign(this.emsEdit, ems);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewEms)
      this.alertService.showMessage("Success", `Ems \"${this.emsEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to ems \"${this.emsEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.emsEdit = new Ems();
    this.resetForm();


    //if (!this.isNewEms && this.accountService.currentUser.facilities.some(r => r == this.editingEmsName))
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
    this.emsEdit = new Ems();

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


  newEms() {
    this.isNewEms = true;
    this.showValidationErrors = true;

    this.editingEmsName = null;
    this.emsEdit = new Ems();
    this.emsEdit.institutionId = this.accountService.currentUser.institutionId;
    return this.emsEdit;
  }

  editEms(ems: Ems) {
    if (ems) {
      this.isNewEms = false;
      this.showValidationErrors = true;

      this.editingEmsName = ems.name;
      this.emsEdit = new Ems();
      Object.assign(this.emsEdit, ems);

      return this.emsEdit;
    }
    else {
      return this.newEms();
    }
  }



  get canManageEmses() {
    return true; //this.accountService.userHasPermission(Permission.manageEmsesPermission)
  }
}
