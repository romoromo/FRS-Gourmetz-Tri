import { Component, Inject, ViewChild } from "@angular/core";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material";
import { PlcModel } from "../../../../models/meal-order/plc.model";
import { Permission } from "../../../../models/permission.model";
import { AccountService } from "../../../../services/account.service";
import { AlertService, MessageSeverity } from "../../../../services/alert.service";
import { ClassService } from "../../../../services/meal-order/class.service";
import { DeliveryService } from "../../../../services/meal-order/delivery.service";

@Component({
  selector: 'plc-editor',
  templateUrl: './plc-editor.component.html',
  styleUrls: ['./plc-editor.component.css']
})
export class PlcEditorComponent {

  private isNewPlc = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingPlcCode: string;
  private plcEdit: PlcModel = new PlcModel();
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
    public dialogRef: MatDialogRef<PlcEditorComponent>, private deliveryService: DeliveryService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    this.editPLC(data.plc);
  }

  private saveSuccessHelper(plc?: PlcModel) {
    if (plc)
      Object.assign(this.plcEdit, plc);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewPlc)
      this.alertService.showMessage("Success", `PLC \"${this.plcEdit.ipAddress}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to PLC \"${this.plcEdit.ipAddress}\" was saved successfully`, MessageSeverity.success);


    this.plcEdit = new PlcModel();
    this.resetForm();

    if (this.changesSavedCallback)
      this.changesSavedCallback();

    this.dialogRef.close();
  }

  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    if (this.isNewPlc) {
      this.classService.newPLC(this.plcEdit).subscribe(plc => this.saveSuccessHelper(plc), error => this.saveFailedHelper(error));
    }
    else {
      this.classService.updatePLC(this.plcEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }

  private saveFailedHelper(error: any) {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage("Save Error", "The below errors occured while saving your changes:", MessageSeverity.error);
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);

    if (this.changesFailedCallback)
      this.changesFailedCallback();
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

  newPLC() {
    this.isNewPlc = true;
    this.showValidationErrors = true;

    this.editingPlcCode = null;
    this.selectedValues = {};
    this.plcEdit = new PlcModel();

    return this.plcEdit;
  }

  editPLC(plc: PlcModel) {
    if (plc && plc.id) {
      this.isNewPlc = false;
      this.showValidationErrors = true;

      this.editingPlcCode = plc.id;
      this.selectedValues = {};
      this.plcEdit = new PlcModel();
      Object.assign(this.plcEdit, plc);
    }
    else {
      this.newPLC();
      Object.assign(this.plcEdit, plc);
    }

    return this.plcEdit;
  }

}
