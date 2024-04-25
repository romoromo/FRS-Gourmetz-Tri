import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { ClassBatch } from 'src/app/models/meal-order/class-batch.model';
import { ClassService } from 'src/app/services/meal-order/class.service';


@Component({
  selector: 'class-batch-editor',
  templateUrl: './class-batch-editor.component.html',
  styleUrls: ['./class-batch-editor.component.css']
})
export class ClassBatchEditorComponent {

  private isNewClassBatch = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingClassBatchName: string;
  private classBatchEdit: ClassBatch = new ClassBatch();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private classService: ClassService, private accountService: AccountService,
    public dialogRef: MatDialogRef<ClassBatchEditorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    this.editClassBatch(data.classBatch);
  }


  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    this.classBatchEdit.institutionId = this.accountService.currentUser.institutionId;
    if (this.isNewClassBatch) {
      this.classService.newClassBatch(this.classBatchEdit).subscribe(classBatch => this.saveSuccessHelper(classBatch), error => this.saveFailedHelper(error));
    }
    else {
      this.classService.updateClassBatch(this.classBatchEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }


  private saveSuccessHelper(classBatch?: ClassBatch) {
    if (classBatch)
      Object.assign(this.classBatchEdit, classBatch);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewClassBatch)
      this.alertService.showMessage("Success", `Class Batch \"${this.classBatchEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to class Batch \"${this.classBatchEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.classBatchEdit = new ClassBatch();
    this.resetForm();


    //if (!this.isNewClassBatch && this.accountService.currentUser.facilities.some(r => r == this.editingClassBatchName))
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
    this.classBatchEdit = new ClassBatch();

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


  newClassBatch() {
    this.isNewClassBatch = true;
    this.showValidationErrors = true;

    this.editingClassBatchName = null;
    this.selectedValues = {};
    this.classBatchEdit = new ClassBatch();

    return this.classBatchEdit;
  }

  editClassBatch(classBatch: ClassBatch) {
    if (classBatch && classBatch.id) {
      this.isNewClassBatch = false;
      this.showValidationErrors = true;

      this.editingClassBatchName = classBatch.name;
      this.selectedValues = {};
      this.classBatchEdit = new ClassBatch();
      Object.assign(this.classBatchEdit, classBatch);

      return this.classBatchEdit;
    }
    else {
      this.newClassBatch();
      Object.assign(this.classBatchEdit, classBatch);
      console.log(this.classBatchEdit);
      return this.classBatchEdit;
    }
  }

  get canManageClassBatches() {
    return this.accountService.userHasPermission(Permission.manageMOSOutletMgtClassBatchesPermission)
  }
}
