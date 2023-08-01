import { Component, ViewChild, Inject, ElementRef, OnDestroy, OnInit } from '@angular/core';
import { fadeInOut } from '../../services/animations';
import { AccountService } from 'src/app/services/account.service';
import { Permission } from 'src/app/models/permission.model';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';
import { EmployeeSchedule } from 'src/app/models/employee-schedule.model';
import { InstitutionService } from 'src/app/services/institution.service';
import { Utilities } from 'src/app/services/utilities';
import { Institution } from 'src/app/models/institution.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { EmployeeScheduleService } from 'src/app/services/employee-schedule.service';
import { Subscription } from 'rxjs';
import { EmployeeScheduleShift } from '../../models/employee-schedule-shift.model';
import { LocationService } from '../../services/location.service';
import { EmployeeDataService } from '../../services/employee-data.service';
import { Location } from '../../models/location.model';
import { EmployeeData } from 'src/app/models/employee-data.model';


@Component({
  selector: 'employee-schedule-assign',
  templateUrl: './employee-schedule-assign.component.html',
  animations: [fadeInOut]
})
export class EmployeeScheduleAssignComponent implements OnInit, OnDestroy{
  private subscription: Subscription = new Subscription();
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  public formResetToggle = true;
  private loadingIndicator = false;
  private isNewEmployeeSchedule = false;
  private originalEmployeeSchedule: EmployeeSchedule = new EmployeeSchedule();
  private employeeScheduleEdit: EmployeeSchedule = new EmployeeSchedule();
  private allInstitutions: Institution[] = [];
  private validation = { code: false, label: false};
  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;

  public day: any = '1';

  public clickedLocationId: any;
  public clickedShiftId: any;
  public clickedDay: any;

  @ViewChild('f')
  private form;

  @ViewChild('code')
  private code;

  @ViewChild('radioGroup')
  private radioGroup;

  @ViewChild('label')
  private label;
    allLocations: Location[];
  selected: any;
  selectedCover: any;
    allEmployeeDatas: EmployeeData[];

  constructor(private alertService: AlertService, private accountService: AccountService, private institutionService: InstitutionService,
    private employeeScheduleService: EmployeeScheduleService, private locationService: LocationService, private employeeDataService: EmployeeDataService,
    public dialogRef: MatDialogRef<EmployeeScheduleAssignComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.employeeSchedule) != typeof (undefined)) {
      if (data.employeeSchedule.id) {
        this.editEmployeeSchedule(data.employeeSchedule);
      } else {
        this.newEmployeeSchedule();
      }
    }

    this.locationService.getLocationsByInstitutionId(this.accountService.currentUser.institutionId)
        .subscribe(results => {
          this.allLocations = results;
          
        },
          error => {
            
          });

    this.employeeDataService.getEmployeeDataByInstitutionId(this.accountService.currentUser.institutionId)
      .subscribe(results => {
        this.allEmployeeDatas = results[0];

      },
        error => {
        });

    this.day = new Date().getDay() + '';
  }

  clickedCell(locationId, shiftId, day) {
    this.clickedLocationId = locationId;
    this.clickedShiftId = shiftId;
    this.clickedDay = day;

    if (!this.employeeScheduleEdit.infos) this.employeeScheduleEdit.infos = [];

    if (!this.employeeScheduleEdit.infos.some(e => e.locationId == locationId && e.shiftId == shiftId && e.day == day)) {
      this.employeeScheduleEdit.infos.push({
        locationId : locationId,
        shiftId : shiftId,
        day : day
      });
    }
  }

  addCover(selected, slot) {
    if (!selected) return;

    slot.coveringEmployeeDataId = selected.id;
    slot.coverName = selected.name;
    slot.cover = false;

    this.selectedCover = null;
  }

  addEmployee(selected, locationId, shiftId, day) {
    if (!selected) return;
    if (!this.employeeScheduleEdit.slots) this.employeeScheduleEdit.slots = [];

    if (this.employeeScheduleEdit.slots.some(e => e.isActive && e.employeeDataId === selected.id && e.locationId == locationId && e.shiftId == shiftId && e.day == day)) {
      this.alertService.showStickyMessage("Error", "Employee is already added", MessageSeverity.error);
      return;
    }

    let s: any = {};
    s.isActive = 1;

    s.employeeDataId = selected.id;
    s.name = selected.name;
    s.locationId = locationId;
    s.shiftId = shiftId;
    s.day = day;

    this.employeeScheduleEdit.slots.push(s);

    this.selected = null;
  }

  addShift() {
    if (!this.employeeScheduleEdit.shifts) this.employeeScheduleEdit.shifts = [];

    let s = new EmployeeScheduleShift();
    s.isActive = 1;

    this.employeeScheduleEdit.shifts.push(s);
  }

  addLocation(selected: any) {
    if (!selected) return;
    if (!this.employeeScheduleEdit.locations) this.employeeScheduleEdit.locations = [];

    if (this.employeeScheduleEdit.locations.some(e => e.locationId === selected.id && e.isActive)) {
      this.alertService.showStickyMessage("Error", "Location is already added", MessageSeverity.error);
      return;
    }

    let c: any = {};
    Object.assign(c, selected);
    c.isActive = 1;

    c.locationId = c.id;
    c.id = "0";
    this.employeeScheduleEdit.locations.push(c);

    this.selected = null;
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


      if (!this.employeeScheduleEdit.id) {
        this.employeeScheduleService.newEmployeeSchedule(this.employeeScheduleEdit).subscribe(employeeSchedule => this.saveSuccessHelper(employeeSchedule), error => this.saveFailedHelper(error));
      }
      else {
        this.employeeScheduleService.updateEmployeeSchedule(this.employeeScheduleEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
      }
  }

  private saveSuccessHelper(employeeSchedule?: EmployeeSchedule) {
    if (employeeSchedule)
      Object.assign(this.employeeScheduleEdit, employeeSchedule);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewEmployeeSchedule) {
      this.alertService.showMessage("Success", `Data \"${this.employeeScheduleEdit.code}\" was created successfully`, MessageSeverity.success);
    }
    else {
      this.alertService.showMessage("Success", `Changes to data \"${this.employeeScheduleEdit.code}\" was saved successfully`, MessageSeverity.success);
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
    this.employeeScheduleEdit = new EmployeeSchedule();

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

  newEmployeeSchedule() {
    this.showValidationErrors = true;
    this.isNewEmployeeSchedule = true;
    this.employeeScheduleEdit = new EmployeeSchedule();
    return this.employeeScheduleEdit;
  }

  editEmployeeSchedule(employeeSchedule: EmployeeSchedule) {
    if (employeeSchedule) {
      this.isNewEmployeeSchedule = false;
      this.showValidationErrors = true;
      this.originalEmployeeSchedule = employeeSchedule;
      this.employeeScheduleEdit = new EmployeeSchedule();
      Object.assign(this.employeeScheduleEdit, employeeSchedule);

      return this.employeeScheduleEdit;
    }
    else {
      return this.newEmployeeSchedule();
    }
  }

  get canManageEmployeeSchedules() {
    return true;//this.accountService.userHasPermission(Permission.manageEmployeeSchedulePermission);
  }

}
