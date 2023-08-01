import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { EmailService } from '../../../services/meal-order/email.service';
import { EmailTemplate } from 'src/app/models/meal-order/email-template';


@Component({
  selector: 'email-template-editor',
  templateUrl: './email-template-editor.component.html',
  styleUrls: ['./email-template-editor.component.css']
})
export class EmailTemplateEditorComponent {

  private isNewEmailTemplate = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingEmailTemplateCode: string;
  private emailTemplateEdit: EmailTemplate = new EmailTemplate();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;
  public outletId: string;
  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private emailService: EmailService, private accountService: AccountService,
    public dialogRef: MatDialogRef<EmailTemplateEditorComponent>, private mealService: MealService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.emailTemplate) != typeof (undefined)) {
      this.outletId = data.outletId;
      if (data.emailTemplate.id) {
        this.editEmailTemplate(data.emailTemplate);
      } else {
        this.newEmailTemplate();
      }
    }
  }


  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    

    if (this.isNewEmailTemplate) {
      this.emailService.newEmailTemplate(this.emailTemplateEdit).subscribe(emailTemplate => this.saveSuccessHelper(emailTemplate), error => this.saveFailedHelper(error));
    }
    else {
      this.emailService.updateEmailTemplate(this.emailTemplateEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }


  private saveSuccessHelper(emailTemplate?: EmailTemplate) {
    if (emailTemplate)
      Object.assign(this.emailTemplateEdit, emailTemplate);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewEmailTemplate)
      this.alertService.showMessage("Success", `\"${this.emailTemplateEdit.subject}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to \"${this.emailTemplateEdit.subject}\" was saved successfully`, MessageSeverity.success);


    this.emailTemplateEdit = new EmailTemplate();
    this.resetForm();


    //if (!this.isNewEmailTemplate && this.accountService.currentUser.facilities.some(r => r == this.editingEmailTemplateCode))
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
    this.emailTemplateEdit = new EmailTemplate();

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


  newEmailTemplate() {
    this.isNewEmailTemplate = true;
    this.showValidationErrors = true;

    this.editingEmailTemplateCode = null;
    this.selectedValues = {};
    this.emailTemplateEdit = new EmailTemplate();
    this.emailTemplateEdit.outletId = this.outletId;
    this.emailTemplateEdit.fromName = 'FRS Team';
    this.emailTemplateEdit.fromEmail = 'donotreply@smv360.com';
    return this.emailTemplateEdit;
  }

  editEmailTemplate(emailTemplate: EmailTemplate) {
    if (emailTemplate) {
      this.isNewEmailTemplate = false;
      this.showValidationErrors = true;

      this.editingEmailTemplateCode = emailTemplate.subject;
      this.selectedValues = {};
      this.emailTemplateEdit = new EmailTemplate();
      Object.assign(this.emailTemplateEdit, emailTemplate);

      return this.emailTemplateEdit;
    }
    else {
      return this.newEmailTemplate();
    }
  }



  get canManageEmailTemplates() {
    return true; //this.accountService.userHasPermission(Permission.manageMOSOrderMgtEmailTemplatesPermission)
  }
}
