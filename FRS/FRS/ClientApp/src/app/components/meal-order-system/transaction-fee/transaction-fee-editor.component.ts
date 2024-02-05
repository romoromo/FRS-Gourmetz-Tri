import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { Filter } from 'src/app/models/sieve-filter.model';
import { TransactionFee } from 'src/app/models/meal-order/transaction-fee.model';
import { PaymentService } from 'src/app/services/meal-order/payment.service';
import { Subscription } from 'rxjs';


@Component({
  selector: 'transaction-fee-editor',
  templateUrl: './transaction-fee-editor.component.html',
  styleUrls: ['./transaction-fee-editor.component.css']
})
export class TransactionFeeEditorComponent {
  private subscription: Subscription = new Subscription();
  private isNewTransactionFee = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingTransactionFeeCode: string;
  private transactionFeeEdit: TransactionFee = new TransactionFee();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;
  public catererId: string;
  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;
  public paymentTypes = [];

  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private paymentService: PaymentService, private accountService: AccountService,
    public dialogRef: MatDialogRef<TransactionFeeEditorComponent>, private mealService: MealService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.transactionFee) != typeof (undefined)) {
      this.catererId = data.catererId;
      if (data.transactionFee.id) {
        this.editTransactionFee(data.transactionFee);
      } else {
        this.newTransactionFee();
      }
    }

    this.getPaymentTypes();
  }

  getPaymentTypes() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.subscription.add(this.paymentService.getPaymentTypesByFilter(filter)
      .subscribe(results => {
        this.paymentTypes = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving payment types.\r\n"`,
            MessageSeverity.error);
        }));
  }

  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    

    if (this.isNewTransactionFee) {
      this.paymentService.newTransactionFee(this.transactionFeeEdit).subscribe(transactionFee => this.saveSuccessHelper(transactionFee), error => this.saveFailedHelper(error));
    }
    else {
      this.paymentService.updateTransactionFee(this.transactionFeeEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }


  private saveSuccessHelper(transactionFee?: TransactionFee) {
    if (transactionFee)
      Object.assign(this.transactionFeeEdit, transactionFee);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewTransactionFee)
      this.alertService.showMessage("Success", `\"${this.transactionFeeEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to \"${this.transactionFeeEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.transactionFeeEdit = new TransactionFee();
    this.resetForm();


    //if (!this.isNewTransactionFee && this.accountService.currentUser.facilities.some(r => r == this.editingTransactionFeeCode))
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
    this.transactionFeeEdit = new TransactionFee();

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


  newTransactionFee() {
    this.isNewTransactionFee = true;
    this.showValidationErrors = true;

    this.editingTransactionFeeCode = null;
    this.selectedValues = {};
    this.transactionFeeEdit = new TransactionFee();
    return this.transactionFeeEdit;
  }

  editTransactionFee(transactionFee: TransactionFee) {
    if (transactionFee) {
      this.isNewTransactionFee = false;
      this.showValidationErrors = true;

      this.editingTransactionFeeCode = transactionFee.name;
      this.selectedValues = {};
      this.transactionFeeEdit = new TransactionFee();
      Object.assign(this.transactionFeeEdit, transactionFee);

      return this.transactionFeeEdit;
    }
    else {
      return this.newTransactionFee();
    }
  }



  get canManageTransactionFees() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtTransactionFeesPermission)
  }
}
