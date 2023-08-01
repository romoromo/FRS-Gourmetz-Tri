import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { InterestGroup } from 'src/app/models/meal-order/interest-group.model';
import { DishService } from 'src/app/services/meal-order/dish.service';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { Filter } from 'src/app/models/sieve-filter.model';
import { MealPeriod } from 'src/app/models/meal-order/meal-period.model';
import { StudentService } from 'src/app/services/meal-order/student.service';


@Component({
  selector: 'interest-group-editor',
  templateUrl: './interest-group-editor.component.html',
  styleUrls: ['./interest-group-editor.component.css']
})
export class InterestGroupEditorComponent {

  private isNewInterestGroup = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingInterestGroupCode: string;
  private interestGroupEdit: InterestGroup = new InterestGroup();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private studentService: StudentService, private accountService: AccountService,
    public dialogRef: MatDialogRef<InterestGroupEditorComponent>, private mealService: MealService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.interestGroup) != typeof (undefined)) {
      if (data.interestGroup.id) {
        this.editInterestGroup(data.interestGroup);
      } else {
        this.newInterestGroup();
      }
    }
  }


  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    this.interestGroupEdit.institutionId = this.accountService.currentUser.institutionId;
    if (this.isNewInterestGroup) {
      this.studentService.newInterestGroup(this.interestGroupEdit).subscribe(interestGroup => this.saveSuccessHelper(interestGroup), error => this.saveFailedHelper(error));
    }
    else {
      this.studentService.updateInterestGroup(this.interestGroupEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }


  private saveSuccessHelper(interestGroup?: InterestGroup) {
    if (interestGroup)
      Object.assign(this.interestGroupEdit, interestGroup);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewInterestGroup)
      this.alertService.showMessage("Success", `Interest Group \"${this.interestGroupEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to Interest Group \"${this.interestGroupEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.interestGroupEdit = new InterestGroup();
    this.resetForm();


    //if (!this.isNewInterestGroup && this.accountService.currentUser.facilities.some(r => r == this.editingInterestGroupCode))
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
    this.interestGroupEdit = new InterestGroup();

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


  newInterestGroup() {
    this.isNewInterestGroup = true;
    this.showValidationErrors = true;

    this.editingInterestGroupCode = null;
    this.selectedValues = {};
    this.interestGroupEdit = new InterestGroup();

    return this.interestGroupEdit;
  }

  editInterestGroup(interestGroup: InterestGroup) {
    if (interestGroup) {
      this.isNewInterestGroup = false;
      this.showValidationErrors = true;

      this.editingInterestGroupCode = interestGroup.name;
      this.selectedValues = {};
      this.interestGroupEdit = new InterestGroup();
      Object.assign(this.interestGroupEdit, interestGroup);

      return this.interestGroupEdit;
    }
    else {
      return this.newInterestGroup();
    }
  }

  get canManageInterestGroups() {
    return true; //this.accountService.userHasPermission(Permission.manageInterestGroupsPermission)
  }
}
