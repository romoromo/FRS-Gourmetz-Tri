import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Department } from '../../../models/department.model';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { DepartmentEditorComponent } from "./department-editor.component";
import { DepartmentService } from 'src/app/services/department.service';
import { MatDialog } from '@angular/material';


@Component({
  selector: 'departments-management',
  templateUrl: './departments-management.component.html',
  styleUrls: ['./departments-management.component.css']
})
export class DepartmentsManagementComponent implements OnInit {
  columns: any[] = [];
  rows: Department[] = [];
  rowsCache: Department[] = [];
  allPermissions: Permission[] = [];
  editedDepartment: Department;
  sourceDepartment: Department;
  editingDepartmentName: { name: string };
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('departmentEditor')
  departmentEditor: DepartmentEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private departmentService: DepartmentService, public dialog: MatDialog) {
  }

  openDialog(department: Department): void {
    const dialogRef = this.dialog.open(DepartmentEditorComponent, {
      data: { header: this.header, department: department },
      width: '400px'
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData(null);
    });
  }

  initializeFilter() {
    this.filter = new Filter(1, 10);
    this.filter.sorts = 'name';
    this.filter.filters = '';
    this.filter.page = 1;

    
  }

  initializePagedResult() {
    this.pagedResult = new PagedResult();
    this.pagedResult.totalCount = 0;
    this.pagedResult.pagedData = [];
    this.pagedResult.filter = this.filter;
  }

  initializeTableDefinition() {
    let gT = (key: string) => this.translationService.getTranslation(key);

    this.columns = [
      //{ prop: "index", name: '#', width: 50, cellTemplate: this.indexTemplate, canAutoResize: false },
      { prop: 'name', name: gT('departments.management.Name') },
      { prop: 'description', name: gT('departments.management.Description') },
      { name: '', width: 150, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
    ];

    if (!this.accountService.currentUser.institutionId || this.accountService.currentUser.institutionId == '0') {
      this.columns.splice(1, 0, { prop: 'institutionName', name: gT('roles.management.Institution'), width: 120 });
    }
  }

  ngOnInit() {
    this.initializeFilter();
    this.initializePagedResult();
    this.initializeTableDefinition();
    this.loadData();
  }


  loadData(ev?: any) {
    this.alertService.startLoadingMessage();
    this.loadingIndicator = true;
    this.filter.pageSize = 10;

    if (ev) {
      this.filter.page = ev.offset + 1;
      if (ev.sorts) {
        this.filter.sorts = ev.sorts[0].dir == 'desc' ? '-' + ev.sorts[0].prop : ev.sorts[0].prop;
      }
    }

    if (!this.keyword) this.keyword = '';
    this.filter.filters = '(IsActive)==true,(Name|Description)@=' + this.keyword + ',(InstitutionId)==' + this.accountService.currentUser.institutionId;
    
    this.departmentService.getDepartmentsByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let departments = results.pagedData;

        departments.forEach((department, index, departments) => {
          (<any>department).index = index + 1;
        });


        this.rowsCache = [...departments];
        this.rows = departments;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve departments from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }


  onSearchChanged(value: string) {
    //this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.name, r.description));
    this.keyword = value;
    this.loadData(null);
  }


  onEditorModalHidden() {
    this.editingDepartmentName = null;
    this.departmentEditor.resetForm(true);
  }


  newDepartment() {
    //this.editingDepartmentName = null;
    //this.sourceDepartment = null;
    //this.editedDepartment = this.departmentEditor.newDepartment();
    //this.editorModal.show();
    this.header = 'New Department';
    this.editedDepartment = new Department();
    this.openDialog(this.editedDepartment);
  }


  editDepartment(row: Department) {
    //this.editingDepartmentName = { name: row.name };
    //this.sourceDepartment = row;
    this.editedDepartment = row; //this.departmentEditor.editDepartment(row);
    //this.editorModal.show();

    this.header = 'Edit Department';
    this.openDialog(this.editedDepartment);
  }

  deleteDepartment(row: Department) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" department?', DialogType.confirm, () => this.deleteDepartmentHelper(row));
  }


  deleteDepartmentHelper(row: Department) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.departmentService.deleteDepartment(row)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        //this.rowsCache = this.rowsCache.filter(item => item !== row)
        //this.rows = this.rows.filter(item => item !== row)
        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the department.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }


  get canManageDepartments() {
    return this.accountService.userHasPermission(Permission.manageDepartmentsPermission)
  }

}
