import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, Inject, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { EmployeeScheduleService } from 'src/app/services/employee-schedule.service';
import { MatDialog } from '@angular/material';
import { Utilities } from 'src/app/services/utilities';
import { EmployeeSchedule } from 'src/app/models/employee-schedule.model';
import { Permission } from 'src/app/models/permission.model';
import { AlertService, MessageSeverity, DialogType } from 'src/app/services/alert.service';
import { AppTranslationService } from 'src/app/services/app-translation.service';
import { AccountService } from 'src/app/services/account.service';
import { EmployeeScheduleEditorComponent } from './employee-schedule-editor.component';
import { Subscription } from 'rxjs';
import { EmployeeScheduleAssignComponent } from './employee-schedule-assign.component';


@Component({
  selector: 'employee-schedules-management',
  templateUrl: './employee-schedules-management.component.html',
  styleUrls: ['./employee-schedules-management.component.css']
})
export class EmployeeSchedulesManagementComponent implements OnInit, OnDestroy, AfterViewInit {
  private subscription: Subscription = new Subscription();
  columns: any[] = [];
  rows: EmployeeSchedule[] = [];
  rowsCache: EmployeeSchedule[] = [];
  allPermissions: Permission[] = [];
  editedEmployeeSchedule: EmployeeSchedule;
  sourceEmployeeSchedule: EmployeeSchedule;
  editingEmployeeScheduleName: { key: string };
  loadingIndicator: boolean;



  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('employeeScheduleEditor')
  employeeScheduleEditor: EmployeeScheduleEditorComponent;

  @ViewChild('employeeScheduleAssign')
  employeeScheduleAssign: EmployeeScheduleAssignComponent;

  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private employeeScheduleService: EmployeeScheduleService, public dialog: MatDialog) {
  }

  openDialog(employeeSchedule: EmployeeSchedule): void {
    const dialogRef = this.dialog.open(EmployeeScheduleEditorComponent, {
      data: { header: this.header, employeeSchedule: employeeSchedule },
      width: '90vw',
      disableClose: true
    });

    this.subscription.add(dialogRef.afterClosed().subscribe(result => {
      this.loadData();
    }));
  }

  openAssignDialog(employeeSchedule: EmployeeSchedule): void {
    const dialogRef = this.dialog.open(EmployeeScheduleAssignComponent, {
      data: { header: this.header, employeeSchedule: employeeSchedule },
      width: '90vw',
      disableClose: true
    });

    this.subscription.add(dialogRef.afterClosed().subscribe(result => {
      this.loadData();
    }));
  }

  ngOnInit() {

    let gT = (key: string) => this.translationService.getTranslation(key);

    this.columns = [
      { prop: "index", name: '#', width: 50, cellTemplate: this.indexTemplate, canAutoResize: false },
      { prop: 'code', name: gT('employeeSchedules.management.Code'), width: 200 },
      { prop: 'label', name: gT('employeeSchedules.management.Label'), width: 200 },
      { name: '', width: 300, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
    ];

    if (!this.accountService.currentUser.institutionId || this.accountService.currentUser.institutionId == '0') {
      this.columns.splice(1, 0, { prop: 'institutionName', name: gT('roles.management.Institution'), width: 120 });
    }
    this.loadData();
  }

  ngOnDestroy() {
    this.alertService.resetStickyMessage();
    this.subscription.unsubscribe();
  }



  ngAfterViewInit() {

    this.employeeScheduleEditor.changesSavedCallback = () => {
      this.addNewEmployeeScheduleToList();
      //this.editorModal.hide();
    };

    this.employeeScheduleEditor.changesCancelledCallback = () => {
      this.editedEmployeeSchedule = null;
      this.sourceEmployeeSchedule = null;
      //this.editorModal.hide();
    };
  }


  addNewEmployeeScheduleToList() {
    if (this.sourceEmployeeSchedule) {
      Object.assign(this.sourceEmployeeSchedule, this.editedEmployeeSchedule);

      let sourceIndex = this.rowsCache.indexOf(this.sourceEmployeeSchedule, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rowsCache, sourceIndex, 0);

      sourceIndex = this.rows.indexOf(this.sourceEmployeeSchedule, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rows, sourceIndex, 0);

      this.editedEmployeeSchedule = null;
      this.sourceEmployeeSchedule = null;
    }
    else {
      let employeeSchedule = new EmployeeSchedule();
      Object.assign(employeeSchedule, this.editedEmployeeSchedule);
      this.editedEmployeeSchedule = null;

      let maxIndex = 0;
      for (let r of this.rowsCache) {
        if ((<any>r).index > maxIndex)
          maxIndex = (<any>r).index;
      }

      (<any>employeeSchedule).index = maxIndex + 1;

      this.rowsCache.splice(0, 0, employeeSchedule);
      this.rows.splice(0, 0, employeeSchedule);
      this.rows = [...this.rows];
    }
  }




  loadData() {
    //this.alertService.startLoadingMessage();
    this.loadingIndicator = true;

    this.subscription.add(this.employeeScheduleService.getEmployeeScheduleByInstitutionId(this.accountService.currentUser.institutionId)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let employeeSchedules = results[0];
        let permissions = results[1];

        employeeSchedules.forEach((employeeSchedule, index, employeeSchedules) => {
          (<any>employeeSchedule).index = index + 1;

          employeeSchedule.shifts.forEach((shift) => {
            try {
              shift.startTime = shift.startTime.split("T")[1];
              shift.endTime = shift.endTime.split("T")[1];
            } catch (ex) { }
          });

          employeeSchedule.shifts = employeeSchedule.shifts.sort((a, b) => a.startTime.localeCompare(b.startTime));
          employeeSchedule.locations = employeeSchedule.locations.sort((a, b) => a.name.localeCompare(b.name));
        });


        this.rowsCache = [...employeeSchedules];
        this.rows = employeeSchedules;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve data from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }


  onSearchChanged(value: string) {
    this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.code, r.label));
  }


  onEditorModalHidden() {
    this.editingEmployeeScheduleName = null;
    this.employeeScheduleEditor.resetForm(true);
  }


  newEmployeeSchedule() {
    this.editingEmployeeScheduleName = null;
    this.sourceEmployeeSchedule = null;
    this.editedEmployeeSchedule = this.employeeScheduleEditor.newEmployeeSchedule();
    this.header = 'New';
    this.openDialog(this.editedEmployeeSchedule);
    //this.editorModal.show();
  }


  editEmployeeSchedule(row: EmployeeSchedule) {
    this.editingEmployeeScheduleName = { key: row.code };
    this.sourceEmployeeSchedule = row;
    this.editedEmployeeSchedule = this.employeeScheduleEditor.editEmployeeSchedule(row);
    this.header = 'Edit';
    //this.editorModal.show();
    this.openDialog(this.editedEmployeeSchedule);
  }

  assignEmployeeSchedule(row: EmployeeSchedule) {
    this.editingEmployeeScheduleName = { key: row.code };
    this.sourceEmployeeSchedule = row;
    this.editedEmployeeSchedule = this.employeeScheduleAssign.editEmployeeSchedule(row);
    this.header = 'Edit';
    //this.editorModal.show();
    this.openAssignDialog(this.editedEmployeeSchedule);
  }

  deleteEmployeeSchedule(row: EmployeeSchedule) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.code + '\"?', DialogType.confirm, () => this.deleteEmployeeScheduleHelper(row));
  }


  deleteEmployeeScheduleHelper(row: EmployeeSchedule) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.subscription.add(this.employeeScheduleService.deleteEmployeeSchedule(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.rowsCache = this.rowsCache.filter(item => item !== row)
        this.rows = this.rows.filter(item => item !== row)
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the data.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }


  get canManageEmployeeSchedules() {
    return true;//this.accountService.userHasPermission(Permission.manageEmployeeSchedulePermission);
  }

}
