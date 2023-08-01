import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { PaymentService } from '../../../services/meal-order/payment.service';
import { Waiver } from 'src/app/models/meal-order/waiver.model';
import { Filter } from 'src/app/models/sieve-filter.model';
import { Subscription } from 'rxjs';


@Component({
  selector: 'waiver-editor',
  templateUrl: './waiver-editor.component.html',
  styleUrls: ['./waiver-editor.component.css']
})
export class WaiverEditorComponent {
  private subscription: Subscription = new Subscription();
  private isNewWaiver = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingWaiverCode: string;
  private waiverEdit: Waiver = new Waiver();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;
  public catererId: string;
  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;
  private transactionFees = [];

  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private paymentService: PaymentService, private accountService: AccountService,
    public dialogRef: MatDialogRef<WaiverEditorComponent>, private mealService: MealService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.waiver) != typeof (undefined)) {
      this.catererId = data.catererId;
      if (data.waiver.id) {
        this.editWaiver(data.waiver);
      } else {
        this.newWaiver();
      }
    }

    this.getTransactionFees();
  }

  getTransactionFees() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.subscription.add(this.paymentService.getTransactionFeesByFilter(filter)
      .subscribe(results => {
        this.transactionFees = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving voucher types.\r\n"`,
            MessageSeverity.error);
        }));
  }

  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    

    if (this.isNewWaiver) {
      this.paymentService.newWaiver(this.waiverEdit).subscribe(waiver => this.saveSuccessHelper(waiver), error => this.saveFailedHelper(error));
    }
    else {
      this.paymentService.updateWaiver(this.waiverEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }


  private saveSuccessHelper(waiver?: Waiver) {
    if (waiver)
      Object.assign(this.waiverEdit, waiver);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewWaiver)
      this.alertService.showMessage("Success", `\"${this.waiverEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to \"${this.waiverEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.waiverEdit = new Waiver();
    this.resetForm();


    //if (!this.isNewWaiver && this.accountService.currentUser.facilities.some(r => r == this.editingWaiverCode))
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
    this.waiverEdit = new Waiver();

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


  newWaiver() {
    this.isNewWaiver = true;
    this.showValidationErrors = true;

    this.editingWaiverCode = null;
    this.selectedValues = {};
    this.waiverEdit = new Waiver();
    return this.waiverEdit;
  }

  editWaiver(waiver: Waiver) {
    if (waiver) {
      this.isNewWaiver = false;
      this.showValidationErrors = true;

      this.editingWaiverCode = waiver.name;
      this.selectedValues = {};
      this.waiverEdit = new Waiver();
      Object.assign(this.waiverEdit, waiver);

      return this.waiverEdit;
    }
    else {
      return this.newWaiver();
    }
  }



  get canManageWaivers() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtWaiversPermission)
  }
}
