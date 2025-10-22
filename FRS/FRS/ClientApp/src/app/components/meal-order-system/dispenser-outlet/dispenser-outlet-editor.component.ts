import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { DispenserOutlet } from 'src/app/models/meal-order/dispenser-outlet.model';
import { ClassService } from 'src/app/services/meal-order/class.service';
import { DeliveryService } from 'src/app/services/meal-order/delivery.service';
import { Filter } from 'src/app/models/sieve-filter.model';
import { TrayEditorComponent } from './tray-editor.component';


@Component({
  selector: 'dispenser-outlet-editor',
  templateUrl: './dispenser-outlet-editor.component.html',
  styleUrls: ['./dispenser-outlet-editor.component.css']
})
export class DispenserOutletEditorComponent {

  private isNewDispenserOutlet = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingDispenserCode: string;
  private dispenserOutletEdit: DispenserOutlet = new DispenserOutlet();
  private allPermissions: Permission[] = [];
  outlets = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private classService: ClassService, private accountService: AccountService,
    public dialogRef: MatDialogRef<DispenserOutletEditorComponent>, private deliveryService: DeliveryService,
    @Inject(MAT_DIALOG_DATA) public data: any, public dialog: MatDialog) {
    this.editDispenserOutlet(data.dispenserOutlet);

    this.getOutlets();

  }

  getOutlets() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.deliveryService.getOutletsSimpleByFilter(filter)
      .subscribe(results => {
        this.outlets = results.pagedData;
      },
        error => {
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving outlets.\r\n"`,
            MessageSeverity.error);
        })
  }

  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    this.dispenserOutletEdit.institutionId = this.accountService.currentUser.institutionId;
    if (this.isNewDispenserOutlet) {
      this.classService.newDispenserOutlet(this.dispenserOutletEdit).subscribe(dispenserOutlet => this.saveSuccessHelper(dispenserOutlet), error => this.saveFailedHelper(error));
    }
    else {
      this.classService.updateDispenserOutlet(this.dispenserOutletEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }


  private saveSuccessHelper(dispenserOutlet?: DispenserOutlet) {
    if (dispenserOutlet)
      Object.assign(this.dispenserOutletEdit, dispenserOutlet);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewDispenserOutlet)
      this.alertService.showMessage("Success", `Dispenser \"${this.dispenserOutletEdit.dispenserCode}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to dispenser \"${this.dispenserOutletEdit.dispenserCode}\" was saved successfully`, MessageSeverity.success);


    this.dispenserOutletEdit = new DispenserOutlet();
    this.resetForm();


    //if (!this.isNewClassLevel && this.accountService.currentUser.facilities.some(r => r == this.editingClassLevelName))
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
    this.dispenserOutletEdit = new DispenserOutlet();

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


  newDispenserOUtlet() {
    this.isNewDispenserOutlet = true;
    this.showValidationErrors = true;

    this.editingDispenserCode = null;
    this.selectedValues = {};
    this.dispenserOutletEdit = new DispenserOutlet();

    return this.dispenserOutletEdit;
  }

  editDispenserOutlet(dispenserOutlet: DispenserOutlet) {
    if (dispenserOutlet && dispenserOutlet.id) {
      this.isNewDispenserOutlet = false;
      this.showValidationErrors = true;

      this.editingDispenserCode = dispenserOutlet.dispenserCode;
      this.selectedValues = {};
      this.dispenserOutletEdit = new DispenserOutlet();
      Object.assign(this.dispenserOutletEdit, dispenserOutlet);
    }
    else {
      this.newDispenserOUtlet();
      Object.assign(this.dispenserOutletEdit, dispenserOutlet);
    }

    return this.dispenserOutletEdit;
  }

  get canManageClassLevels() {
    return this.accountService.userHasPermission(Permission.manageMOSOutletMgtClassLevelsPermission)
  }

  openDialogTrays(): void {
    const dialogRef = this.dialog.open(TrayEditorComponent, {
      width: '500px',
      disableClose: true,
      data: { outletId: this.dispenserOutletEdit.outletId }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        console.log('Trays Result : ', result);
      }
    });

  }
}
