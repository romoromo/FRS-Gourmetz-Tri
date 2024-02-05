import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { OrderPortalService } from 'src/app/services/order-portal.service';
import { OrderPortalBanner, OrderPortalContent } from 'src/app/models/meal-order/order-portal-content.model';
import { FileService } from 'src/app/services/file.service';
import { getBaseUrl } from 'src/app/app.module';


@Component({
  selector: 'order-portal-content-editor',
  templateUrl: './order-portal-content.component.html',
  styleUrls: ['./order-portal-content.component.css']
})
export class OrderPortalContentEditorComponent {

  private isNewOrderPortalContent = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingOrderPortalContentCode: string;
  private portalContentEdit: OrderPortalContent = new OrderPortalContent();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;
  public outletId: string;
  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;
  public fileUploadResponse: { dbPath: '', fileId: null, fileName: '' };

  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private portalService: OrderPortalService, private accountService: AccountService,
    public dialogRef: MatDialogRef<OrderPortalContentEditorComponent>, private mealService: MealService, private fileService: FileService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.portalContent) != typeof (undefined)) {
      this.outletId = data.outletId;
      if (data.portalContent.id) {
        this.editOrderPortalContent(data.portalContent);
      } else {
        this.newOrderPortalContent();
      }
    }
  }


  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    

    if (this.isNewOrderPortalContent) {
      this.portalService.newOrderPortalContent(this.portalContentEdit).subscribe(portalContent => this.saveSuccessHelper(portalContent), error => this.saveFailedHelper(error));
    }
    else {
      this.portalService.updateOrderPortalContent(this.portalContentEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }


  private saveSuccessHelper(portalContent?: OrderPortalContent) {
    if (portalContent)
      Object.assign(this.portalContentEdit, portalContent);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewOrderPortalContent)
      this.alertService.showMessage("Success", `Content was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to the content was saved successfully`, MessageSeverity.success);


    this.portalContentEdit = new OrderPortalContent();
    this.resetForm();


    //if (!this.isNewOrderPortalContent && this.accountService.currentUser.facilities.some(r => r == this.editingOrderPortalContentCode))
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
    this.portalContentEdit = new OrderPortalContent();

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


  newOrderPortalContent() {
    this.isNewOrderPortalContent = true;
    this.showValidationErrors = true;

    this.editingOrderPortalContentCode = null;
    this.selectedValues = {};
    this.portalContentEdit = new OrderPortalContent();
    this.portalContentEdit.outletId = this.outletId;
    this.portalContentEdit.banners = [];
    return this.portalContentEdit;
  }

  editOrderPortalContent(portalContent: OrderPortalContent) {
    if (portalContent) {
      this.isNewOrderPortalContent = false;
      this.showValidationErrors = true;

      this.selectedValues = {};
      this.portalContentEdit = new OrderPortalContent();
      Object.assign(this.portalContentEdit, portalContent);

      return this.portalContentEdit;
    }
    else {
      return this.newOrderPortalContent();
    }
  }

  addBanner(compo) {
    let dc = new OrderPortalBanner();
    dc.orderPortalContentId = this.portalContentEdit.id;
    if (!this.portalContentEdit.banners) this.portalContentEdit.banners = [];
    this.portalContentEdit.banners.push(dc);
  }
  removeBanner(compo) {
    if (!this.portalContentEdit.banners) this.portalContentEdit.banners = [];
    const indx = this.portalContentEdit.banners.indexOf(compo);
    if (indx > -1)
      this.portalContentEdit.banners.splice(indx, 1);
  }

  public uploadBannerImageFinished = (event, banner) => {
    this.fileUploadResponse = event;
    banner.imageFilePath = this.fileUploadResponse ? this.fileUploadResponse.dbPath : null;
  }

  removePhoto(banner) {
    banner.imageFilePath = null;
    banner.imageFileName = null;
  }

  public uploadBannerFinished = (event, banner) => {
    this.fileUploadResponse = event;
    banner.fileName = event.fileName;
    banner.filePath = this.fileUploadResponse ? this.fileUploadResponse.dbPath : null;
  }

  getFileImage(path) {
    return this.fileService.getFile(path);
  }

  downloadFile(path) {
    window.open(getBaseUrl() + '/gateway/Download/FileByPath?filePath=/' + encodeURIComponent(path), '_blank');
  }

  get canManageOrderPortalContents() {
    return this.accountService.userHasPermission(Permission.manageMOSOutletMgtPortalContentsPermission)
  }
}
