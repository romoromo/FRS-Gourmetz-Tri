import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { BentoBoxType } from 'src/app/models/meal-order/bento-box-type.model';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { Filter } from 'src/app/models/sieve-filter.model';
import { MealPeriod } from 'src/app/models/meal-order/meal-period.model';
import { StaffService } from '../../../services/meal-order/staff.service';
import { DeliveryService } from '../../../services/meal-order/delivery.service';
import { CatererInfo } from '../../../models/meal-order/caterer-info.model';


@Component({
  selector: 'bento-box-type-editor',
  templateUrl: './bento-box-type-editor.component.html',
  styleUrls: ['./bento-box-type-editor.component.css']
})
export class BentoBoxTypeEditorComponent {

  private isNewBentoBoxType = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingBentoBoxTypeCode: string;
  private bentoBoxTypeEdit: BentoBoxType = new BentoBoxType();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;
  private periods: MealPeriod[] = [];
  private catererInfos: CatererInfo[] = [];

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private deliveryService: DeliveryService, private accountService: AccountService,
    public dialogRef: MatDialogRef<BentoBoxTypeEditorComponent>, private mealService: MealService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.bentoBoxType) != typeof (undefined)) {
      if (data.bentoBoxType.id) {
        this.editBentoBoxType(data.bentoBoxType);
      } else {
        this.newBentoBoxType();
      }
    }

    this.getCatererInfos();
  }


  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    this.bentoBoxTypeEdit.institutionId = this.accountService.currentUser.institutionId;
    console.log(this.periods);
    let selectedPeriods = this.periods.filter(f => f.checked);
    

    if (this.isNewBentoBoxType) {
      this.deliveryService.newBentoBoxType(this.bentoBoxTypeEdit).subscribe(bentoBoxType => this.saveSuccessHelper(bentoBoxType), error => this.saveFailedHelper(error));
    }
    else {
      this.deliveryService.updateBentoBoxType(this.bentoBoxTypeEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }

  public uploadFinished = (event) => {
    this.bentoBoxTypeEdit.picture = event ? event.dbPath : null;

    if (this.bentoBoxTypeEdit.picture) this.bentoBoxTypeEdit.picture = this.bentoBoxTypeEdit.picture.replace(/\\/g, '/');
  }


  private saveSuccessHelper(bentoBoxType?: BentoBoxType) {
    if (bentoBoxType)
      Object.assign(this.bentoBoxTypeEdit, bentoBoxType);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewBentoBoxType)
      this.alertService.showMessage("Success", `Bento box type \"${this.bentoBoxTypeEdit.code}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to Bento box type \"${this.bentoBoxTypeEdit.code}\" was saved successfully`, MessageSeverity.success);


    this.bentoBoxTypeEdit = new BentoBoxType();
    this.resetForm();


    //if (!this.isNewBentoBoxType && this.accountService.currentUser.facilities.some(r => r == this.editingBentoBoxTypeCode))
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
    this.bentoBoxTypeEdit = new BentoBoxType();

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


  newBentoBoxType() {
    this.isNewBentoBoxType = true;
    this.showValidationErrors = true;

    this.editingBentoBoxTypeCode = null;
    this.selectedValues = {};
    this.bentoBoxTypeEdit = new BentoBoxType();

    return this.bentoBoxTypeEdit;
  }

  editBentoBoxType(bentoBoxType: BentoBoxType) {
    if (bentoBoxType) {
      this.isNewBentoBoxType = false;
      this.showValidationErrors = true;

      this.editingBentoBoxTypeCode = bentoBoxType.code;
      this.selectedValues = {};
      this.bentoBoxTypeEdit = new BentoBoxType();
      Object.assign(this.bentoBoxTypeEdit, bentoBoxType);

      return this.bentoBoxTypeEdit;
    }
    else {
      return this.newBentoBoxType();
    }
  }

  getCatererInfos() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.deliveryService.getCatererInfosByFilter(filter)
      .subscribe(results => {
        this.catererInfos = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving caterers.\r\n"`,
            MessageSeverity.error);
        })
  }



  get canManageBentoBoxTypes() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtBentoBoxTypesPermission)
  }
}
