import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { Filter } from 'src/app/models/sieve-filter.model';
import { MealPeriod } from 'src/app/models/meal-order/meal-period.model';
import { StaffService } from '../../../services/meal-order/staff.service';
import { PaymentService } from '../../../services/meal-order/payment.service';
import { PaymentType } from 'src/app/models/meal-order/payment-type.model';


@Component({
  selector: 'payment-type-editor',
  templateUrl: './payment-type-editor.component.html',
  styleUrls: ['./payment-type-editor.component.css']
})
export class PaymentTypeEditorComponent {

  private isNewPaymentType = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingPaymentTypeCode: string;
  private paymentTypeEdit: PaymentType = new PaymentType();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;
  public catererId: string;
  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private paymentService: PaymentService, private accountService: AccountService,
    public dialogRef: MatDialogRef<PaymentTypeEditorComponent>, private mealService: MealService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.paymentType) != typeof (undefined)) {
      this.catererId = data.catererId;
      if (data.paymentType.id) {
        this.editPaymentType(data.paymentType);
      } else {
        this.newPaymentType();
      }
    }
  }


  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    

    if (this.isNewPaymentType) {
      this.paymentService.newPaymentType(this.paymentTypeEdit).subscribe(paymentType => this.saveSuccessHelper(paymentType), error => this.saveFailedHelper(error));
    }
    else {
      this.paymentService.updatePaymentType(this.paymentTypeEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }


  private saveSuccessHelper(paymentType?: PaymentType) {
    if (paymentType)
      Object.assign(this.paymentTypeEdit, paymentType);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewPaymentType)
      this.alertService.showMessage("Success", `\"${this.paymentTypeEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to \"${this.paymentTypeEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.paymentTypeEdit = new PaymentType();
    this.resetForm();


    //if (!this.isNewPaymentType && this.accountService.currentUser.facilities.some(r => r == this.editingPaymentTypeCode))
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
    this.paymentTypeEdit = new PaymentType();

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


  newPaymentType() {
    this.isNewPaymentType = true;
    this.showValidationErrors = true;

    this.editingPaymentTypeCode = null;
    this.selectedValues = {};
    this.paymentTypeEdit = new PaymentType();
    return this.paymentTypeEdit;
  }

  editPaymentType(paymentType: PaymentType) {
    if (paymentType) {
      this.isNewPaymentType = false;
      this.showValidationErrors = true;

      this.editingPaymentTypeCode = paymentType.name;
      this.selectedValues = {};
      this.paymentTypeEdit = new PaymentType();
      Object.assign(this.paymentTypeEdit, paymentType);

      return this.paymentTypeEdit;
    }
    else {
      return this.newPaymentType();
    }
  }



  get canManagePaymentTypes() {
    return true; //this.accountService.userHasPermission(Permission.managePaymentTypesPermission)
  }
}
