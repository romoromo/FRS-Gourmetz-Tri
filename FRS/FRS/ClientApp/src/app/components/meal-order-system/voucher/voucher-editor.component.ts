import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { PaymentService } from '../../../services/meal-order/payment.service';
import { Voucher, VoucherMealPeriod } from 'src/app/models/meal-order/voucher.model';
import { Filter } from 'src/app/models/sieve-filter.model';
import { Subscription } from 'rxjs';
import { MealPeriod } from 'src/app/models/meal-order/meal-period.model';
import { DeliveryService } from 'src/app/services/meal-order/delivery.service';
import { VoucherDishComponent } from './voucher-dish/vouhcer-dish.component';


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
  private minEndDate: string = '';
  selectedDishes: any[] = [];

  @ViewChild('f')
  private form;
  voucherTypes: any;

  constructor(private alertService: AlertService, private paymentService: PaymentService, private accountService: AccountService,
    public dialogRef: MatDialogRef<VoucherEditorComponent>, private mealService: MealService, private deliveryService: DeliveryService,
    public dialog: MatDialog,
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

    if (this.voucherEdit.endDateTime && this.voucherEdit.startDateTime && this.voucherEdit.endDateTime < this.voucherEdit.startDateTime) {
      this.alertService.stopLoadingMessage();
      this.alertService.showStickyMessage("Save Error", "End date cannot be earlier than start date.", MessageSeverity.error);
      this.isSaving = false;
      return; // Prevent submission
    }

    if (this.voucherEdit.usageQuantity < this.voucherEdit.usageQuantityUsed) {
      this.alertService.stopLoadingMessage();
      this.alertService.showStickyMessage("Save Error", "Quantities Allocated cannot be less than Quantities Assigned Out.", MessageSeverity.error);
      this.isSaving = false;
      return; // Prevent submission
    }

    this.voucherEdit.voucherMealPeriods = [];
    this.periods.forEach((p, index, ps) => {
      if (p.checked) {
        let period = new VoucherMealPeriod();
        period.VoucherId = this.voucherEdit.id;
        period.mealPeriodId = p.id;
        this.voucherEdit.voucherMealPeriods.push(period);
      }

    });
    this.voucherEdit.voucherDishes = [...this.selectedDishes]
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
    this.voucherEdit.usageQuantityUsed = 0
    return this.voucherEdit;
  }

  editVoucher(voucher: Voucher) {
    if (voucher) {
      this.isNewVoucher = false;
      this.showValidationErrors = true;

      this.editingVoucherCode = voucher.name;
      this.selectedValues = {};
      this.voucherEdit = new Voucher();
      if (!this.voucherEdit.usageQuantityUsed) {
        this.voucherEdit.usageQuantityUsed = 0
      }
      Object.assign(this.voucherEdit, voucher);

      this.setMinEndDate();

      voucher.voucherDishes.forEach(dish => {
        var detail = {
          dishId: dish.dishId,
          dishName: dish.dishName,
          dishCode: dish.dishCode
        }
        this.selectedDishes.push(detail);
      })

      return this.voucherEdit;
    }
    else {
      return this.newVoucher();
    }
  }

  setMinEndDate() {
    console.log('start date', this.voucherEdit.startDateTime.toString());
    this.minEndDate = this.voucherEdit.startDateTime.toString();
  }

  //#region Dish
  openDialogDish(): void {
    const dialogRef = this.dialog.open(VoucherDishComponent, {
      data: { selectedDishes: this.selectedDishes },
      width: '1000px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result.isCancel) {
        this.selectedDishes = []
        result.selectedData.forEach(data => {
          var detail = {
            dishId: data.id,
            dishName: data.label,
            dishCode: data.code
          }
          this.selectedDishes.push(detail)
        })
      }
    });

  }

  deleteDish(row: any, index: number) {
    this.alertService.showDialog('Are you sure you want to remove the \"' + row.dishName + '\"?', DialogType.confirm, () => this.deleteDishHelper(index));
  }


  deleteDishHelper(index: number) {
    if (this.selectedDishes)
      this.selectedDishes.splice(index, 1);
  }
  //#endregion

  get canManageVouchers() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtVouchersPermission)
  }
}
