import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { PaymentService } from '../../../services/meal-order/payment.service';
import { Voucher, VoucherMealPeriod } from 'src/app/models/meal-order/voucher.model';
import { Filter } from 'src/app/models/sieve-filter.model';
import { Subscription } from 'rxjs';
import { MealPeriod } from 'src/app/models/meal-order/meal-period.model';
import { DeliveryService } from 'src/app/services/meal-order/delivery.service';


@Component({
  selector: 'voucher-editor',
  templateUrl: './voucher-editor.component.html',
  styleUrls: ['./voucher-editor.component.css']
})
export class VoucherEditorComponent {
  private subscription: Subscription = new Subscription();
  private isNewVoucher = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingVoucherCode: string;
  private voucherEdit: Voucher = new Voucher();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;
  public catererId: string;
  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;
  private periods: MealPeriod[] = [];
  private outletProfiles = [];

  @ViewChild('f')
  private form;
  voucherTypes: any;

  constructor(private alertService: AlertService, private paymentService: PaymentService, private accountService: AccountService,
    public dialogRef: MatDialogRef<VoucherEditorComponent>, private mealService: MealService, private deliveryService: DeliveryService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.voucher) != typeof (undefined)) {
      if (data.voucher.id) {
        this.editVoucher(data.voucher);
      } else {
        this.newVoucher();
      }
    }
    this.getOutletProfiles();;
    this.getVoucherTypes();
  }

  getVoucherTypes() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.subscription.add(this.paymentService.getVoucherTypesByFilter(filter)
      .subscribe(results => {
        this.voucherTypes = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving voucher types.\r\n"`,
            MessageSeverity.error);
        }));
  }

  getOutletProfiles() {
    let filter = new Filter();
    let f = this.catererId ? '(CatererId)==' + this.catererId + ',' : '';
    filter.filters = f + '(IsActive)==true';
    this.deliveryService.getOutletProfilesByFilter(filter)
      .subscribe(results => {
        this.outletProfiles = results.pagedData;
        this.getPeriods(false, this.voucherEdit.outletProfileId);
      },
        error => {
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving outlets.\r\n"`,
            MessageSeverity.error);
        })
  }

  getPeriods(isClearValue: boolean, outletProfileId?: string) {
    if (outletProfileId) {
      //if (isClearValue) this.dishCycleEdit.outletProfileId = '';
      let filter = new Filter();
      filter.sorts = 'sequence';
      filter.filters = '(IsActive)==true,(OutletProfileId)==' + outletProfileId;

      this.mealService.getMealPeriodsByFilter(filter)
        .subscribe(results => {
          this.periods = results.pagedData;
          this.periods.forEach((p, index, ps) => {
            (<any>p).checked = this.voucherEdit.voucherMealPeriods != null && this.voucherEdit.voucherMealPeriods.findIndex(f => f.mealPeriodId == p.id) > -1;
          });
        },
          error => {
          })
    } else {

    }
  }

  onOutletChange(outletProfileId?: string) {
    this.getPeriods(true, outletProfileId);
  }

  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");

    this.voucherEdit.voucherMealPeriods = [];
    this.periods.forEach((p, index, ps) => {
      if (p.checked) {
        let period = new VoucherMealPeriod();
        period.VoucherId = this.voucherEdit.id;
        period.mealPeriodId = p.id;
        this.voucherEdit.voucherMealPeriods.push(period);
      }

    });

    if (this.isNewVoucher) {
      this.paymentService.newVoucher(this.voucherEdit).subscribe(voucher => this.saveSuccessHelper(voucher), error => this.saveFailedHelper(error));
    }
    else {
      this.paymentService.updateVoucher(this.voucherEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }


  private saveSuccessHelper(voucher?: Voucher) {
    if (voucher)
      Object.assign(this.voucherEdit, voucher);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewVoucher)
      this.alertService.showMessage("Success", `\"${this.voucherEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to \"${this.voucherEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.voucherEdit = new Voucher();
    this.resetForm();


    //if (!this.isNewVoucher && this.accountService.currentUser.facilities.some(r => r == this.editingVoucherCode))
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
    this.voucherEdit = new Voucher();

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


  newVoucher() {
    this.isNewVoucher = true;
    this.showValidationErrors = true;

    this.editingVoucherCode = null;
    this.selectedValues = {};
    this.voucherEdit = new Voucher();
    return this.voucherEdit;
  }

  editVoucher(voucher: Voucher) {
    if (voucher) {
      this.isNewVoucher = false;
      this.showValidationErrors = true;

      this.editingVoucherCode = voucher.name;
      this.selectedValues = {};
      this.voucherEdit = new Voucher();
      Object.assign(this.voucherEdit, voucher);

      return this.voucherEdit;
    }
    else {
      return this.newVoucher();
    }
  }

  onUsageQuantityChange(newQty: number) {
    this.voucherEdit.usageQuantityUsed = 0;
  }



  get canManageVouchers() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtVouchersPermission)
  }
}
