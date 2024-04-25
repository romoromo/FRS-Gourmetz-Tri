import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { OutletSimple, OutletProfile } from 'src/app/models/meal-order/outlet.model';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { Filter } from 'src/app/models/sieve-filter.model';
import { MealPeriod } from 'src/app/models/meal-order/meal-period.model';
import { StaffService } from '../../../services/meal-order/staff.service';
import { DeliveryService } from '../../../services/meal-order/delivery.service';


@Component({
  selector: 'outlet-editor',
  templateUrl: './outlet-editor.component.html',
  styleUrls: ['./outlet-editor.component.css']
})
export class OutletEditorComponent {

  private isNewOutlet = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingOutletCode: string;
  private outletEdit: OutletSimple = new OutletSimple();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;
  private outletProfiles: OutletProfile[] = [];

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private deliveryService: DeliveryService, private accountService: AccountService,
    public dialogRef: MatDialogRef<OutletEditorComponent>, private mealService: MealService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.outlet) != typeof (undefined)) {
      if (data.outlet.id) {
        this.editOutlet(data.outlet);
      } else {
        this.newOutlet();
      }
    }

    this.getOutletProfiles();
  }

  getOutletProfiles() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.deliveryService.getOutletProfilesByFilter(filter)
      .subscribe(results => {
        this.outletProfiles = results.pagedData;
      },
        error => {
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving the records.\r\n"`,
            MessageSeverity.error);
        })
  }

  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");

    if (this.isNewOutlet) {
      this.outletEdit.createdBy = this.accountService.currentUser.id;
      this.deliveryService.newOutlet(this.outletEdit).subscribe(outlet => this.saveSuccessHelper(outlet), error => this.saveFailedHelper(error));
    }
    else {
      this.deliveryService.updateOutlet(this.outletEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }


  private saveSuccessHelper(outlet?: OutletSimple) {
    if (outlet)
      Object.assign(this.outletEdit, outlet);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewOutlet)
      this.alertService.showMessage("Success", `Caterer \"${this.outletEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to caterer \"${this.outletEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.outletEdit = new OutletSimple();
    this.resetForm();


    //if (!this.isNewOutlet && this.accountService.currentUser.facilities.some(r => r == this.editingOutletCode))
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
    this.outletEdit = new OutletSimple();

    this.showValidationErrors = false;
    this.resetForm();

    this.alertService.resetStickyMessage();

    if (this.changesCancelledCallback)
      this.changesCancelledCallback();

    this.dialogRef.close(true);
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


  newOutlet() {
    this.isNewOutlet = true;
    this.showValidationErrors = true;

    this.editingOutletCode = null;
    this.selectedValues = {};
    this.outletEdit = new OutletSimple();

    return this.outletEdit;
  }

  editOutlet(outlet: OutletSimple) {
    if (outlet) {
      this.isNewOutlet = false;
      this.showValidationErrors = true;

      this.editingOutletCode = outlet.name;
      this.selectedValues = {};
      this.outletEdit = new OutletSimple();
      Object.assign(this.outletEdit, outlet);

      return this.outletEdit;
    }
    else {
      return this.newOutlet();
    }
  }



  get canManageOutlets() {
    return true; //this.accountService.userHasPermission(Permission.manageOutletsPermission)
  }
}
