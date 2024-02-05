import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { EmailService } from '../../../services/meal-order/email.service';
import { ContactUsSubject } from 'src/app/models/meal-order/contact-us-subject.model';


@Component({
  selector: 'contact-us-subject-editor',
  templateUrl: './contact-us-subject-editor.component.html',
  styleUrls: ['./contact-us-subject-editor.component.css']
})
export class ContactUsSubjectEditorComponent {

  private isNewContactUsSubject = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingContactUsSubjectCode: string;
  private contactUsSubjectEdit: ContactUsSubject = new ContactUsSubject();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;
  public catererId: string;
  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private emailService: EmailService, private accountService: AccountService,
    public dialogRef: MatDialogRef<ContactUsSubjectEditorComponent>, private mealService: MealService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.contactUsSubject) != typeof (undefined)) {
      this.catererId = data.catererId;
      if (data.contactUsSubject.id) {
        this.editContactUsSubject(data.contactUsSubject);
      } else {
        this.newContactUsSubject();
      }
    }
  }


  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    

    if (this.isNewContactUsSubject) {
      this.emailService.newContactUsSubject(this.contactUsSubjectEdit).subscribe(contactUsSubject => this.saveSuccessHelper(contactUsSubject), error => this.saveFailedHelper(error));
    }
    else {
      this.emailService.updateContactUsSubject(this.contactUsSubjectEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }


  private saveSuccessHelper(contactUsSubject?: ContactUsSubject) {
    if (contactUsSubject)
      Object.assign(this.contactUsSubjectEdit, contactUsSubject);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewContactUsSubject)
      this.alertService.showMessage("Success", `\"${this.contactUsSubjectEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to \"${this.contactUsSubjectEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.contactUsSubjectEdit = new ContactUsSubject();
    this.resetForm();


    //if (!this.isNewContactUsSubject && this.accountService.currentUser.facilities.some(r => r == this.editingContactUsSubjectCode))
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
    this.contactUsSubjectEdit = new ContactUsSubject();

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


  newContactUsSubject() {
    this.isNewContactUsSubject = true;
    this.showValidationErrors = true;

    this.editingContactUsSubjectCode = null;
    this.selectedValues = {};
    this.contactUsSubjectEdit = new ContactUsSubject();
    return this.contactUsSubjectEdit;
  }

  editContactUsSubject(contactUsSubject: ContactUsSubject) {
    if (contactUsSubject) {
      this.isNewContactUsSubject = false;
      this.showValidationErrors = true;

      this.editingContactUsSubjectCode = contactUsSubject.name;
      this.selectedValues = {};
      this.contactUsSubjectEdit = new ContactUsSubject();
      Object.assign(this.contactUsSubjectEdit, contactUsSubject);

      return this.contactUsSubjectEdit;
    }
    else {
      return this.newContactUsSubject();
    }
  }



  get canManageContactUsSubjects() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtContactUsSubjectsPermission)
  }
}
