import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { CatererInfo } from 'src/app/models/meal-order/caterer-info.model';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { Filter } from 'src/app/models/sieve-filter.model';
import { MealPeriod } from 'src/app/models/meal-order/meal-period.model';
import { StaffService } from '../../../services/meal-order/staff.service';
import { DeliveryService } from '../../../services/meal-order/delivery.service';


@Component({
  selector: 'caterer-info-editor',
  templateUrl: './caterer-info-editor.component.html',
  styleUrls: ['./caterer-info-editor.component.css']
})
export class CatererInfoEditorComponent {

  private isNewCatererInfo = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingCatererInfoCode: string;
  private catererInfoEdit: CatererInfo = new CatererInfo();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;
  private periods: MealPeriod[] = [];

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private deliveryService: DeliveryService, private accountService: AccountService,
    public dialogRef: MatDialogRef<CatererInfoEditorComponent>, private mealService: MealService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.catererInfo) != typeof (undefined)) {
      if (data.catererInfo.id) {
        this.editCatererInfo(data.catererInfo);
      } else {
        this.newCatererInfo();
      }
    }
  }


  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    this.catererInfoEdit.institutionId = this.accountService.currentUser.institutionId;
    console.log(this.periods);
    let selectedPeriods = this.periods.filter(f => f.checked);
    

    if (this.isNewCatererInfo) {
      this.deliveryService.newCatererInfo(this.catererInfoEdit).subscribe(catererInfo => this.saveSuccessHelper(catererInfo), error => this.saveFailedHelper(error));
    }
    else {
      this.deliveryService.updateCatererInfo(this.catererInfoEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }


  private saveSuccessHelper(catererInfo?: CatererInfo) {
    if (catererInfo)
      Object.assign(this.catererInfoEdit, catererInfo);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewCatererInfo)
      this.alertService.showMessage("Success", `Caterer \"${this.catererInfoEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to caterer \"${this.catererInfoEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.catererInfoEdit = new CatererInfo();
    this.resetForm();


    //if (!this.isNewCatererInfo && this.accountService.currentUser.facilities.some(r => r == this.editingCatererInfoCode))
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
    this.catererInfoEdit = new CatererInfo();

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


  newCatererInfo() {
    this.isNewCatererInfo = true;
    this.showValidationErrors = true;

    this.editingCatererInfoCode = null;
    this.selectedValues = {};
    this.catererInfoEdit = new CatererInfo();

    return this.catererInfoEdit;
  }

  editCatererInfo(catererInfo: CatererInfo) {
    if (catererInfo) {
      this.isNewCatererInfo = false;
      this.showValidationErrors = true;

      this.editingCatererInfoCode = catererInfo.name;
      this.selectedValues = {};
      this.catererInfoEdit = new CatererInfo();
      Object.assign(this.catererInfoEdit, catererInfo);

      return this.catererInfoEdit;
    }
    else {
      return this.newCatererInfo();
    }
  }



  get canManageCatererInfos() {
    return true; //this.accountService.userHasPermission(Permission.manageCatererInfosPermission)
  }
}
