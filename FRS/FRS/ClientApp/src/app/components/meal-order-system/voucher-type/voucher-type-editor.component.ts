import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { PaymentService } from '../../../services/meal-order/payment.service';
import { VoucherType } from 'src/app/models/meal-order/voucher-type.model';


@Component({
  selector: 'voucher-type-editor',
  templateUrl: './voucher-type-editor.component.html',
  styleUrls: ['./voucher-type-editor.component.css']
})
export class VoucherTypeEditorComponent {

  private isNewVoucherType = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingVoucherTypeCode: string;
  private voucherTypeEdit: VoucherType = new VoucherType();
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
    public dialogRef: MatDialogRef<VoucherTypeEditorComponent>, private mealService: MealService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.voucherType) != typeof (undefined)) {
      this.catererId = data.catererId;
      if (data.voucherType.id) {
        this.editVoucherType(data.voucherType);
      } else {
        this.newVoucherType();
      }
    }
  }


  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    

    if (this.isNewVoucherType) {
      this.paymentService.newVoucherType(this.voucherTypeEdit).subscribe(voucherType => this.saveSuccessHelper(voucherType), error => this.saveFailedHelper(error));
    }
    else {
      this.paymentService.updateVoucherType(this.voucherTypeEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }


  private saveSuccessHelper(voucherType?: VoucherType) {
    if (voucherType)
      Object.assign(this.voucherTypeEdit, voucherType);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewVoucherType)
      this.alertService.showMessage("Success", `\"${this.voucherTypeEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to \"${this.voucherTypeEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.voucherTypeEdit = new VoucherType();
    this.resetForm();


    //if (!this.isNewVoucherType && this.accountService.currentUser.facilities.some(r => r == this.editingVoucherTypeCode))
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
    this.voucherTypeEdit = new VoucherType();

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


  newVoucherType() {
    this.isNewVoucherType = true;
    this.showValidationErrors = true;

    this.editingVoucherTypeCode = null;
    this.selectedValues = {};
    this.voucherTypeEdit = new VoucherType();
    return this.voucherTypeEdit;
  }

  editVoucherType(voucherType: VoucherType) {
    if (voucherType) {
      this.isNewVoucherType = false;
      this.showValidationErrors = true;

      this.editingVoucherTypeCode = voucherType.name;
      this.selectedValues = {};
      this.voucherTypeEdit = new VoucherType();
      Object.assign(this.voucherTypeEdit, voucherType);

      return this.voucherTypeEdit;
    }
    else {
      return this.newVoucherType();
    }
  }



  get canManageVoucherTypes() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtVoucherTypesPermission)
  }
}
