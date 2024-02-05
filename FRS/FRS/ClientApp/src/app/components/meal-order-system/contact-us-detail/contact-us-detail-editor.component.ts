import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { Filter } from 'src/app/models/sieve-filter.model';
import { ContactUsDetail } from 'src/app/models/meal-order/contact-us-subject.model';
import { EmailService } from 'src/app/services/meal-order/email.service';
import { Subscription } from 'rxjs';


@Component({
  selector: 'contact-us-detail-editor',
  templateUrl: './contact-us-detail-editor.component.html',
  styleUrls: ['./contact-us-detail-editor.component.css']
})
export class ContactUsDetailEditorComponent {
  private subscription: Subscription = new Subscription();
  private isNewContactUsDetail = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingContactUsDetailCode: string;
  private contactUsDetailEdit: ContactUsDetail = new ContactUsDetail();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;
  public catererId: string;
  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;
  public contactUsSubjects = [];

  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private emailService: EmailService, private accountService: AccountService,
    public dialogRef: MatDialogRef<ContactUsDetailEditorComponent>, private mealService: MealService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.contactUsDetail) != typeof (undefined)) {
      this.catererId = data.catererId;
      if (data.contactUsDetail.id) {
        this.editContactUsDetail(data.contactUsDetail);
      } else {
        this.newContactUsDetail();
      }
    }

    this.getContactUsSubjects();
  }

  getContactUsSubjects() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.subscription.add(this.emailService.getContactUsSubjectsByFilter(filter)
      .subscribe(results => {
        this.contactUsSubjects = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving records.\r\n"`,
            MessageSeverity.error);
        }));
  }

  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    

    if (this.isNewContactUsDetail) {
      this.emailService.newContactUsDetail(this.contactUsDetailEdit).subscribe(contactUsDetail => this.saveSuccessHelper(contactUsDetail), error => this.saveFailedHelper(error));
    }
    else {
      this.emailService.updateContactUsDetail(this.contactUsDetailEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }


  private saveSuccessHelper(contactUsDetail?: ContactUsDetail) {
    if (contactUsDetail)
      Object.assign(this.contactUsDetailEdit, contactUsDetail);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewContactUsDetail)
      this.alertService.showMessage("Success", `\"${this.contactUsDetailEdit.label}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to \"${this.contactUsDetailEdit.label}\" was saved successfully`, MessageSeverity.success);


    this.contactUsDetailEdit = new ContactUsDetail();
    this.resetForm();


    //if (!this.isNewContactUsDetail && this.accountService.currentUser.facilities.some(r => r == this.editingContactUsDetailCode))
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
    this.contactUsDetailEdit = new ContactUsDetail();

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


  newContactUsDetail() {
    this.isNewContactUsDetail = true;
    this.showValidationErrors = true;

    this.editingContactUsDetailCode = null;
    this.selectedValues = {};
    this.contactUsDetailEdit = new ContactUsDetail();
    return this.contactUsDetailEdit;
  }

  editContactUsDetail(contactUsDetail: ContactUsDetail) {
    if (contactUsDetail) {
      this.isNewContactUsDetail = false;
      this.showValidationErrors = true;

      this.editingContactUsDetailCode = contactUsDetail.label;
      this.selectedValues = {};
      this.contactUsDetailEdit = new ContactUsDetail();
      Object.assign(this.contactUsDetailEdit, contactUsDetail);

      return this.contactUsDetailEdit;
    }
    else {
      return this.newContactUsDetail();
    }
  }



  get canManageContactUsDetails() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtContactUsQuestionsPermission)
  }
}
