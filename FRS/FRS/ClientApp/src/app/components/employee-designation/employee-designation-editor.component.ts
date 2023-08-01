import { Component, ViewChild, Inject, ElementRef, OnDestroy, OnInit } from '@angular/core';
import { fadeInOut } from '../../services/animations';
import { AccountService } from 'src/app/services/account.service';
import { Permission } from 'src/app/models/permission.model';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';
import { EmployeeDesignation } from 'src/app/models/employee-designation.model';
import { InstitutionService } from 'src/app/services/institution.service';
import { Utilities } from 'src/app/services/utilities';
import { Institution } from 'src/app/models/institution.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { EmployeeDesignationService } from 'src/app/services/employee-designation.service';
import { Subscription } from 'rxjs';


@Component({
  selector: 'employee-designation-editor',
  templateUrl: './employee-designation-editor.component.html',
  styleUrls: ['./employee-designation-editor.component.css'],
  animations: [fadeInOut]
})
export class EmployeeDesignationEditorComponent implements OnInit, OnDestroy{
  private subscription: Subscription = new Subscription();
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  public formResetToggle = true;
  private loadingIndicator = false;
  private isNewEmployeeDesignation = false;
  private originalEmployeeDesignation: EmployeeDesignation = new EmployeeDesignation();
  private employeeDesignationEdit: EmployeeDesignation = new EmployeeDesignation();
  private allInstitutions: Institution[] = [];
  private validation = { code: false, label: false};
  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;

  @ViewChild('f')
  private form;

  @ViewChild('code')
  private code;

  @ViewChild('label')
  private label;

  constructor(private alertService: AlertService, private accountService: AccountService, private institutionService: InstitutionService,
    private employeeDesignationService: EmployeeDesignationService,
    public dialogRef: MatDialogRef<EmployeeDesignationEditorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.employeeDesignation) != typeof (undefined)) {
      if (data.employeeDesignation.id) {
        this.editEmployeeDesignation(data.employeeDesignation);
      } else {
        this.newEmployeeDesignation();
      }
    }
  }

  ngOnInit() {
    this.alertService.resetStickyMessage();
  }

  ngOnDestroy() {
    this.alertService.resetStickyMessage();
    this.subscription.unsubscribe();
  }

  private save() {
      this.isSaving = true;
      this.alertService.startLoadingMessage("Saving changes...");


      if (!this.employeeDesignationEdit.id) {
        this.employeeDesignationService.newEmployeeDesignation(this.employeeDesignationEdit).subscribe(employeeDesignation => this.saveSuccessHelper(employeeDesignation), error => this.saveFailedHelper(error));
      }
      else {
        this.employeeDesignationService.updateEmployeeDesignation(this.employeeDesignationEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
      }
  }

  private saveSuccessHelper(employeeDesignation?: EmployeeDesignation) {
    if (employeeDesignation)
      Object.assign(this.employeeDesignationEdit, employeeDesignation);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewEmployeeDesignation) {
      this.alertService.showMessage("Success", `Data \"${this.employeeDesignationEdit.code}\" was created successfully`, MessageSeverity.success);
    }
    else {
      this.alertService.showMessage("Success", `Changes to data \"${this.employeeDesignationEdit.code}\" was saved successfully`, MessageSeverity.success);
    }

    if (this.changesSavedCallback)
      this.changesSavedCallback();

    this.dialogRef.close();
  }

  private saveFailedHelper(error: any) {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage("Save Error", "The below errors occured while saving your changes:", MessageSeverity.error);
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);

    //if (this.changesFailedCallback)
    //  this.changesFailedCallback();

    //this.dialogRef.close();
  }


  private cancel() {
    this.employeeDesignationEdit = new EmployeeDesignation();

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

  loadInstitutions() {
    this.institutionService.getInstitutions()
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.allInstitutions = results[0];

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve institutions from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  newEmployeeDesignation() {
    this.showValidationErrors = true;
    this.isNewEmployeeDesignation = true;
    this.employeeDesignationEdit = new EmployeeDesignation();
    return this.employeeDesignationEdit;
  }

  editEmployeeDesignation(employeeDesignation: EmployeeDesignation) {
    if (employeeDesignation) {
      this.isNewEmployeeDesignation = false;
      this.showValidationErrors = true;
      this.originalEmployeeDesignation = employeeDesignation;
      this.employeeDesignationEdit = new EmployeeDesignation();
      Object.assign(this.employeeDesignationEdit, employeeDesignation);

      return this.employeeDesignationEdit;
    }
    else {
      return this.newEmployeeDesignation();
    }
  }

  get canManageEmployeeDesignations() {
    return true;//this.accountService.userHasPermission(Permission.manageEmployeeDesignationPermission);
  }

}
