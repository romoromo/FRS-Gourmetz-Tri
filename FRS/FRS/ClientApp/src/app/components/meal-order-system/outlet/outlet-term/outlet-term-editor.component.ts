import { Component, ViewChild, Inject } from '@angular/core';

import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { DeliveryService } from 'src/app/services/meal-order/delivery.service';
import { Filter } from 'src/app/models/sieve-filter.model';
import { Permission } from 'src/app/models/permission.model';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';
import { OutletTerm } from 'src/app/models/meal-order/outlet.model';
import { AccountService } from 'src/app/services/account.service';


@Component({
  selector: 'outlet-term-editor',
  templateUrl: './outlet-term-editor.component.html',
  styleUrls: ['./outlet-term-editor.component.css']
})
export class OutletTermEditorComponent {

  private isNewOutletTerm = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingOutletTermName: string;
  private outletTermEdit: OutletTerm = new OutletTerm();
  private allPermissions: Permission[] = [];
  outlets = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private deliveryService: DeliveryService, private accountService: AccountService,
    public dialogRef: MatDialogRef<OutletTermEditorComponent>, 
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.outletTerm) != typeof (undefined)) {
      if (data.outletTerm.id) {
        this.editOutletTerm(data.outletTerm);
      } else {
        this.newOutletTerm();
      }
    }

    this.getOutletTerms();
  }

  getOutletTerms() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.deliveryService.getOutletTermsByFilter(filter)
      .subscribe(results => {
        this.outlets = results.pagedData;
      },
        error => {
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving terms.\r\n"`,
            MessageSeverity.error);
        })
  }

  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    if (this.isNewOutletTerm) {
      this.deliveryService.newOutletTerm(this.outletTermEdit).subscribe(outletTerm => this.saveSuccessHelper(outletTerm), error => this.saveFailedHelper(error));
    }
    else {
      this.deliveryService.updateOutletTerm(this.outletTermEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }


  private saveSuccessHelper(outletTerm?: OutletTerm) {
    if (outletTerm)
      Object.assign(this.outletTermEdit, outletTerm);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewOutletTerm)
      this.alertService.showMessage("Success", `Term \"${this.outletTermEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to term \"${this.outletTermEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.outletTermEdit = new OutletTerm();
    this.resetForm();


    //if (!this.isNewOutletTerm && this.accountService.currentUser.facilities.some(r => r == this.editingOutletTermName))
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
    this.outletTermEdit = new OutletTerm();

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


  newOutletTerm() {
    this.isNewOutletTerm = true;
    this.showValidationErrors = true;

    this.editingOutletTermName = null;
    this.selectedValues = {};
    this.outletTermEdit = new OutletTerm();

    return this.outletTermEdit;
  }

  editOutletTerm(outletTerm: OutletTerm) {
    if (outletTerm) {
      this.isNewOutletTerm = false;
      this.showValidationErrors = true;

      this.editingOutletTermName = outletTerm.name;
      this.selectedValues = {};
      this.outletTermEdit = new OutletTerm();
      Object.assign(this.outletTermEdit, outletTerm);

      return this.outletTermEdit;
    }
    else {
      return this.newOutletTerm();
    }
  }

  get canManageOutletTerms() {
    return true; //this.accountService.userHasPermission(Permission.manageOutletTermsPermission)
  }
}
