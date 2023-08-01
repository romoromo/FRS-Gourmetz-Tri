import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, Inject, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { EmployeeDataService } from 'src/app/services/employee-data.service';
import { MatDialog } from '@angular/material';
import { Utilities } from 'src/app/services/utilities';
import { EmployeeData } from 'src/app/models/employee-data.model';
import { Permission } from 'src/app/models/permission.model';
import { AlertService, MessageSeverity, DialogType } from 'src/app/services/alert.service';
import { AppTranslationService } from 'src/app/services/app-translation.service';
import { AccountService } from 'src/app/services/account.service';
import { EmployeeDataEditorComponent } from './employee-data-editor.component';
import { Subscription } from 'rxjs';


@Component({
  selector: 'employee-datas-management',
  templateUrl: './employee-datas-management.component.html',
  styleUrls: ['./employee-datas-management.component.css']
})
export class EmployeeDatasManagementComponent implements OnInit, OnDestroy, AfterViewInit {
  private subscription: Subscription = new Subscription();
  columns: any[] = [];
  rows: EmployeeData[] = [];
  rowsCache: EmployeeData[] = [];
  allPermissions: Permission[] = [];
  editedEmployeeData: EmployeeData;
  sourceEmployeeData: EmployeeData;
  editingEmployeeDataName: { key: string };
  loadingIndicator: boolean;



  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('employeeDataEditor')
  employeeDataEditor: EmployeeDataEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private employeeDataService: EmployeeDataService, public dialog: MatDialog) {
  }

  openDialog(employeeData: EmployeeData): void {
    const dialogRef = this.dialog.open(EmployeeDataEditorComponent, {
      data: { header: this.header, employeeData: employeeData },
      width: '400px',
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
      { prop: 'code', name: gT('employeeDatas.management.Code') },
      { prop: 'name', name: gT('employeeDatas.management.Name') },
      { prop: 'employeeDesignationLabel', name: gT('employeeDatas.management.Designation') },
      { prop: 'displayName', name: gT('employeeDatas.management.DisplayName') },
      { name: '', width: 150, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
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

    this.employeeDataEditor.changesSavedCallback = () => {
      this.addNewEmployeeDataToList();
      //this.editorModal.hide();
    };

    this.employeeDataEditor.changesCancelledCallback = () => {
      this.editedEmployeeData = null;
      this.sourceEmployeeData = null;
      //this.editorModal.hide();
    };
  }


  addNewEmployeeDataToList() {
    if (this.sourceEmployeeData) {
      Object.assign(this.sourceEmployeeData, this.editedEmployeeData);

      let sourceIndex = this.rowsCache.indexOf(this.sourceEmployeeData, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rowsCache, sourceIndex, 0);

      sourceIndex = this.rows.indexOf(this.sourceEmployeeData, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rows, sourceIndex, 0);

      this.editedEmployeeData = null;
      this.sourceEmployeeData = null;
    }
    else {
      let employeeData = new EmployeeData();
      Object.assign(employeeData, this.editedEmployeeData);
      this.editedEmployeeData = null;

      let maxIndex = 0;
      for (let r of this.rowsCache) {
        if ((<any>r).index > maxIndex)
          maxIndex = (<any>r).index;
      }

      (<any>employeeData).index = maxIndex + 1;

      this.rowsCache.splice(0, 0, employeeData);
      this.rows.splice(0, 0, employeeData);
      this.rows = [...this.rows];
    }
  }




  loadData() {
    //this.alertService.startLoadingMessage();
    this.loadingIndicator = true;

    this.subscription.add(this.employeeDataService.getEmployeeDataByInstitutionId(this.accountService.currentUser.institutionId)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let employeeDatas = results[0];
        let permissions = results[1];

        employeeDatas.forEach((employeeData, index, employeeDatas) => {
          (<any>employeeData).index = index + 1;
        });


        this.rowsCache = [...employeeDatas];
        this.rows = employeeDatas;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve data from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }


  onSearchChanged(value: string) {
    this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.code, r.name));
  }


  onEditorModalHidden() {
    this.editingEmployeeDataName = null;
    this.employeeDataEditor.resetForm(true);
  }


  newEmployeeData() {
    this.editingEmployeeDataName = null;
    this.sourceEmployeeData = null;
    this.editedEmployeeData = this.employeeDataEditor.newEmployeeData();
    this.header = 'New';
    this.openDialog(this.editedEmployeeData);
    //this.editorModal.show();
  }


  editEmployeeData(row: EmployeeData) {
    this.editingEmployeeDataName = { key: row.code };
    this.sourceEmployeeData = row;
    this.editedEmployeeData = this.employeeDataEditor.editEmployeeData(row);
    this.header = 'Edit';
    //this.editorModal.show();
    this.openDialog(this.editedEmployeeData);
  }

  deleteEmployeeData(row: EmployeeData) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.code + '\"?', DialogType.confirm, () => this.deleteEmployeeDataHelper(row));
  }


  deleteEmployeeDataHelper(row: EmployeeData) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.subscription.add(this.employeeDataService.deleteEmployeeData(row.id)
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


  get canManageEmployeeDatas() {
    return true;//this.accountService.userHasPermission(Permission.manageEmployeeDataPermission);
  }

}
