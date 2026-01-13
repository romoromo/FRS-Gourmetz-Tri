import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { PaymentService } from '../../../services/meal-order/payment.service';
import { CancelOrderRequest } from 'src/app/models/meal-order/cancel-order-request.model';
import { ConfigurationService } from 'src/app/services/configuration.service';
import { getBaseUrl } from 'src/app/app.module';


@Component({
  selector: 'cancel-order-request-editor',
  templateUrl: './cancel-order-request-editor.component.html',
  styleUrls: ['./cancel-order-request-editor.component.css']
})
export class CancelOrderRequestEditorComponent {

  private isNewCancelOrderRequest = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingCancelOrderRequestCode: string;
  private cancelOrderRequestEdit: CancelOrderRequest = new CancelOrderRequest();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;
  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;

  public isDisabled = false;

  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private paymentService: PaymentService, private accountService: AccountService,
    public dialogRef: MatDialogRef<CancelOrderRequestEditorComponent>, private mealService: MealService, private configurationService: ConfigurationService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.cancelOrderRequest) != typeof (undefined)) {
      this.editCancelOrderRequest(data.cancelOrderRequest);
    }

    this.isDisabled = data.isDisabled
  }


  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");

    this.paymentService.approveCancelRequest(this.cancelOrderRequestEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
  }


  private saveSuccessHelper(cancelOrderRequest?: CancelOrderRequest) {
    if (cancelOrderRequest)
      Object.assign(this.cancelOrderRequestEdit, cancelOrderRequest);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    this.alertService.showMessage("Success", `Changes are saved successfully`, MessageSeverity.success);


    this.cancelOrderRequestEdit = new CancelOrderRequest();
    this.resetForm();


    //if (!this.isNewCancelOrderRequest && this.accountService.currentUser.facilities.some(r => r == this.editingCancelOrderRequestCode))
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
    this.cancelOrderRequestEdit = new CancelOrderRequest();

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


  newCancelOrderRequest() {
    this.isNewCancelOrderRequest = true;
    this.showValidationErrors = true;

    this.selectedValues = {};
    this.cancelOrderRequestEdit = new CancelOrderRequest();
    return this.cancelOrderRequestEdit;
  }

  editCancelOrderRequest(cancelOrderRequest: CancelOrderRequest) {
    if (cancelOrderRequest) {
      this.isNewCancelOrderRequest = false;
      this.showValidationErrors = true;

      this.selectedValues = {};
      this.cancelOrderRequestEdit = new CancelOrderRequest();
      Object.assign(this.cancelOrderRequestEdit, cancelOrderRequest);

      return this.cancelOrderRequestEdit;
    }
    else {
      return this.newCancelOrderRequest();
    }
  }


  downloadFile(path) {

    var baseUrl = getBaseUrl();

    var url = new URL(baseUrl);

    var cleanUrl = url.origin;

    window.open(cleanUrl + '/Download/FileByPath?filePath=/' + encodeURIComponent(path), '_blank');
  }

  get canManageCancelOrderRequests() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtCancelRequestsPermission)
  }
}
