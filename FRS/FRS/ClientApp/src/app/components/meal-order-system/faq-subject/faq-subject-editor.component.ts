import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { FaqService } from '../../../services/meal-order/faq.service';
import { FaqSubject } from 'src/app/models/meal-order/faq-subject.model';


@Component({
  selector: 'faq-subject-editor',
  templateUrl: './faq-subject-editor.component.html',
  styleUrls: ['./faq-subject-editor.component.css']
})
export class FaqSubjectEditorComponent {

  private isNewFaqSubject = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingFaqSubjectCode: string;
  private faqSubjectEdit: FaqSubject = new FaqSubject();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;
  public catererId: string;
  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private faqService: FaqService, private accountService: AccountService,
    public dialogRef: MatDialogRef<FaqSubjectEditorComponent>, private mealService: MealService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.faqSubject) != typeof (undefined)) {
      this.catererId = data.catererId;
      if (data.faqSubject.id) {
        this.editFaqSubject(data.faqSubject);
      } else {
        this.newFaqSubject();
      }
    }
  }


  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    

    if (this.isNewFaqSubject) {
      this.faqService.newFaqSubject(this.faqSubjectEdit).subscribe(faqSubject => this.saveSuccessHelper(faqSubject), error => this.saveFailedHelper(error));
    }
    else {
      this.faqService.updateFaqSubject(this.faqSubjectEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }


  private saveSuccessHelper(faqSubject?: FaqSubject) {
    if (faqSubject)
      Object.assign(this.faqSubjectEdit, faqSubject);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewFaqSubject)
      this.alertService.showMessage("Success", `\"${this.faqSubjectEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to \"${this.faqSubjectEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.faqSubjectEdit = new FaqSubject();
    this.resetForm();


    //if (!this.isNewFaqSubject && this.accountService.currentUser.facilities.some(r => r == this.editingFaqSubjectCode))
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
    this.faqSubjectEdit = new FaqSubject();

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


  newFaqSubject() {
    this.isNewFaqSubject = true;
    this.showValidationErrors = true;

    this.editingFaqSubjectCode = null;
    this.selectedValues = {};
    this.faqSubjectEdit = new FaqSubject();
    return this.faqSubjectEdit;
  }

  editFaqSubject(faqSubject: FaqSubject) {
    if (faqSubject) {
      this.isNewFaqSubject = false;
      this.showValidationErrors = true;

      this.editingFaqSubjectCode = faqSubject.name;
      this.selectedValues = {};
      this.faqSubjectEdit = new FaqSubject();
      Object.assign(this.faqSubjectEdit, faqSubject);

      return this.faqSubjectEdit;
    }
    else {
      return this.newFaqSubject();
    }
  }



  get canManageFaqSubjects() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtFaqSubjectsPermission)
  }
}
