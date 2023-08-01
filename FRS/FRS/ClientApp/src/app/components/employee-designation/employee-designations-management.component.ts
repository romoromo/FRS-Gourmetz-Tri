import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, Inject, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { EmployeeDesignationService } from 'src/app/services/employee-designation.service';
import { MatDialog } from '@angular/material';
import { Utilities } from 'src/app/services/utilities';
import { EmployeeDesignation } from 'src/app/models/employee-designation.model';
import { Permission } from 'src/app/models/permission.model';
import { AlertService, MessageSeverity, DialogType } from 'src/app/services/alert.service';
import { AppTranslationService } from 'src/app/services/app-translation.service';
import { AccountService } from 'src/app/services/account.service';
import { EmployeeDesignationEditorComponent } from './employee-designation-editor.component';
import { Subscription } from 'rxjs';


@Component({
  selector: 'employee-designations-management',
  templateUrl: './employee-designations-management.component.html',
  styleUrls: ['./employee-designations-management.component.css']
})
export class EmployeeDesignationsManagementComponent implements OnInit, OnDestroy, AfterViewInit {
  private subscription: Subscription = new Subscription();
  columns: any[] = [];
  rows: EmployeeDesignation[] = [];
  rowsCache: EmployeeDesignation[] = [];
  allPermissions: Permission[] = [];
  editedEmployeeDesignation: EmployeeDesignation;
  sourceEmployeeDesignation: EmployeeDesignation;
  editingEmployeeDesignationName: { key: string };
  loadingIndicator: boolean;



  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('employeeDesignationEditor')
  employeeDesignationEditor: EmployeeDesignationEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private employeeDesignationService: EmployeeDesignationService, public dialog: MatDialog) {
  }

  openDialog(employeeDesignation: EmployeeDesignation): void {
    const dialogRef = this.dialog.open(EmployeeDesignationEditorComponent, {
      data: { header: this.header, employeeDesignation: employeeDesignation },
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
      { prop: 'code', name: gT('employeeDesignations.management.Code') },
      { prop: 'label', name: gT('employeeDesignations.management.Label') },
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

    this.employeeDesignationEditor.changesSavedCallback = () => {
      this.addNewEmployeeDesignationToList();
      //this.editorModal.hide();
    };

    this.employeeDesignationEditor.changesCancelledCallback = () => {
      this.editedEmployeeDesignation = null;
      this.sourceEmployeeDesignation = null;
      //this.editorModal.hide();
    };
  }


  addNewEmployeeDesignationToList() {
    if (this.sourceEmployeeDesignation) {
      Object.assign(this.sourceEmployeeDesignation, this.editedEmployeeDesignation);

      let sourceIndex = this.rowsCache.indexOf(this.sourceEmployeeDesignation, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rowsCache, sourceIndex, 0);

      sourceIndex = this.rows.indexOf(this.sourceEmployeeDesignation, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rows, sourceIndex, 0);

      this.editedEmployeeDesignation = null;
      this.sourceEmployeeDesignation = null;
    }
    else {
      let employeeDesignation = new EmployeeDesignation();
      Object.assign(employeeDesignation, this.editedEmployeeDesignation);
      this.editedEmployeeDesignation = null;

      let maxIndex = 0;
      for (let r of this.rowsCache) {
        if ((<any>r).index > maxIndex)
          maxIndex = (<any>r).index;
      }

      (<any>employeeDesignation).index = maxIndex + 1;

      this.rowsCache.splice(0, 0, employeeDesignation);
      this.rows.splice(0, 0, employeeDesignation);
      this.rows = [...this.rows];
    }
  }




  loadData() {
    //this.alertService.startLoadingMessage();
    this.loadingIndicator = true;

    this.subscription.add(this.employeeDesignationService.getEmployeeDesignationByInstitutionId(this.accountService.currentUser.institutionId)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let employeeDesignations = results[0];
        let permissions = results[1];

        employeeDesignations.forEach((employeeDesignation, index, employeeDesignations) => {
          (<any>employeeDesignation).index = index + 1;
        });


        this.rowsCache = [...employeeDesignations];
        this.rows = employeeDesignations;

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
    this.editingEmployeeDesignationName = null;
    this.employeeDesignationEditor.resetForm(true);
  }


  newEmployeeDesignation() {
    this.editingEmployeeDesignationName = null;
    this.sourceEmployeeDesignation = null;
    this.editedEmployeeDesignation = this.employeeDesignationEditor.newEmployeeDesignation();
    this.header = 'New';
    this.openDialog(this.editedEmployeeDesignation);
    //this.editorModal.show();
  }


  editEmployeeDesignation(row: EmployeeDesignation) {
    this.editingEmployeeDesignationName = { key: row.code };
    this.sourceEmployeeDesignation = row;
    this.editedEmployeeDesignation = this.employeeDesignationEditor.editEmployeeDesignation(row);
    this.header = 'Edit';
    //this.editorModal.show();
    this.openDialog(this.editedEmployeeDesignation);
  }

  deleteEmployeeDesignation(row: EmployeeDesignation) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.code + '\"?', DialogType.confirm, () => this.deleteEmployeeDesignationHelper(row));
  }


  deleteEmployeeDesignationHelper(row: EmployeeDesignation) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.subscription.add(this.employeeDesignationService.deleteEmployeeDesignation(row.id)
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


  get canManageEmployeeDesignations() {
    return true;//this.accountService.userHasPermission(Permission.manageEmployeeDesignationPermission);
  }

}
