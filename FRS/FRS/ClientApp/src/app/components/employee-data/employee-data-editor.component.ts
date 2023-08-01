import { Component, ViewChild, Inject, ElementRef, OnDestroy, OnInit } from '@angular/core';
import { fadeInOut } from '../../services/animations';
import { AccountService } from 'src/app/services/account.service';
import { Permission } from 'src/app/models/permission.model';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';
import { EmployeeData } from 'src/app/models/employee-data.model';
import { InstitutionService } from 'src/app/services/institution.service';
import { Utilities } from 'src/app/services/utilities';
import { Institution } from 'src/app/models/institution.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { EmployeeDataService } from 'src/app/services/employee-data.service';
import { Subscription } from 'rxjs';
import { EmployeeDesignationService } from '../../services/employee-designation.service';


@Component({
  selector: 'employee-data-editor',
  templateUrl: './employee-data-editor.component.html',
  styleUrls: ['./employee-data-editor.component.css'],
  animations: [fadeInOut]
})
export class EmployeeDataEditorComponent implements OnInit, OnDestroy{
  private subscription: Subscription = new Subscription();
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  public formResetToggle = true;
  private loadingIndicator = false;
  private isNewEmployeeData = false;
  private originalEmployeeData: EmployeeData = new EmployeeData();
  private employeeDataEdit: EmployeeData = new EmployeeData();
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
    employeeDesignations: any;

  constructor(private alertService: AlertService, private accountService: AccountService, private institutionService: InstitutionService,
    private employeeDataService: EmployeeDataService, private employeeDesignationService: EmployeeDesignationService,
    public dialogRef: MatDialogRef<EmployeeDataEditorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.employeeData) != typeof (undefined)) {
      if (data.employeeData.id) {
        this.editEmployeeData(data.employeeData);
      } else {
        this.newEmployeeData();
      }
    }

    if (!this.employeeDesignations) this.employeeDesignationService.getEmployeeDesignationByInstitutionId(this.accountService.currentUser.institutionId)
      .subscribe(results => {
        this.employeeDesignations = results[0];
      },
        error => {
        });
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


      if (!this.employeeDataEdit.id) {
        this.employeeDataService.newEmployeeData(this.employeeDataEdit).subscribe(employeeData => this.saveSuccessHelper(employeeData), error => this.saveFailedHelper(error));
      }
      else {
        this.employeeDataService.updateEmployeeData(this.employeeDataEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
      }
  }

  private saveSuccessHelper(employeeData?: EmployeeData) {
    if (employeeData)
      Object.assign(this.employeeDataEdit, employeeData);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewEmployeeData) {
      this.alertService.showMessage("Success", `Data \"${this.employeeDataEdit.code}\" was created successfully`, MessageSeverity.success);
    }
    else {
      this.alertService.showMessage("Success", `Changes to data \"${this.employeeDataEdit.code}\" was saved successfully`, MessageSeverity.success);
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
    this.employeeDataEdit = new EmployeeData();

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

  newEmployeeData() {
    this.showValidationErrors = true;
    this.isNewEmployeeData = true;
    this.employeeDataEdit = new EmployeeData();
    return this.employeeDataEdit;
  }

  editEmployeeData(employeeData: EmployeeData) {
    if (employeeData) {
      this.isNewEmployeeData = false;
      this.showValidationErrors = true;
      this.originalEmployeeData = employeeData;
      this.employeeDataEdit = new EmployeeData();
      Object.assign(this.employeeDataEdit, employeeData);

      return this.employeeDataEdit;
    }
    else {
      return this.newEmployeeData();
    }
  }

  get canManageEmployeeDatas() {
    return true;//this.accountService.userHasPermission(Permission.manageEmployeeDataPermission);
  }

}
