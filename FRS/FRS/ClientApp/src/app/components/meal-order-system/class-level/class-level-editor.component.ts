import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { ClassLevel } from 'src/app/models/meal-order/class-level.model';
import { ClassService } from 'src/app/services/meal-order/class.service';
import { DeliveryService } from 'src/app/services/meal-order/delivery.service';
import { Filter } from 'src/app/models/sieve-filter.model';


@Component({
  selector: 'class-level-editor',
  templateUrl: './class-level-editor.component.html',
  styleUrls: ['./class-level-editor.component.css']
})
export class ClassLevelEditorComponent {

  private isNewClassLevel = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingClassLevelName: string;
  private classLevelEdit: ClassLevel = new ClassLevel();
  private allPermissions: Permission[] = [];
  outlets = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private classService: ClassService, private accountService: AccountService,
    public dialogRef: MatDialogRef<ClassLevelEditorComponent>, private deliveryService: DeliveryService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    this.editClassLevel(data.classLevel);

    this.getOutlets();
  }

  getOutlets() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.deliveryService.getOutletsSimpleByFilter(filter)
      .subscribe(results => {
        this.outlets = results.pagedData;
      },
        error => {
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving outlets.\r\n"`,
            MessageSeverity.error);
        })
  }

  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    this.classLevelEdit.institutionId = this.accountService.currentUser.institutionId;
    if (this.isNewClassLevel) {
      this.classService.newClassLevel(this.classLevelEdit).subscribe(classLevel => this.saveSuccessHelper(classLevel), error => this.saveFailedHelper(error));
    }
    else {
      this.classService.updateClassLevel(this.classLevelEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }


  private saveSuccessHelper(classLevel?: ClassLevel) {
    if (classLevel)
      Object.assign(this.classLevelEdit, classLevel);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewClassLevel)
      this.alertService.showMessage("Success", `Class Level \"${this.classLevelEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to class Level \"${this.classLevelEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.classLevelEdit = new ClassLevel();
    this.resetForm();


    //if (!this.isNewClassLevel && this.accountService.currentUser.facilities.some(r => r == this.editingClassLevelName))
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
    this.classLevelEdit = new ClassLevel();

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


  newClassLevel() {
    this.isNewClassLevel = true;
    this.showValidationErrors = true;

    this.editingClassLevelName = null;
    this.selectedValues = {};
    this.classLevelEdit = new ClassLevel();

    return this.classLevelEdit;
  }

  editClassLevel(classLevel: ClassLevel) {
    if (classLevel && classLevel.id) {
      this.isNewClassLevel = false;
      this.showValidationErrors = true;

      this.editingClassLevelName = classLevel.name;
      this.selectedValues = {};
      this.classLevelEdit = new ClassLevel();
      Object.assign(this.classLevelEdit, classLevel);
    }
    else {
      this.newClassLevel();
      Object.assign(this.classLevelEdit, classLevel);
    }

    return this.classLevelEdit;
  }

  get canManageClassLevels() {
    return this.accountService.userHasPermission(Permission.manageMOSOutletMgtClassLevelsPermission)
  }
}
