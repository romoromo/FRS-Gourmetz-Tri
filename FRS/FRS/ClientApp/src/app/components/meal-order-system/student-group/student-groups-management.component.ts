import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { Student } from 'src/app/models/meal-order/student.model';
import { StudentGroup } from 'src/app/models/meal-order/student-group.model';
import { StudentGroupEditorComponent } from './student-group-editor.component';
import { StudentService } from 'src/app/services/meal-order/student.service';
import { HttpEvent, HttpEventType } from '@angular/common/http';
import { TokenOrdersBulkManagementComponent } from './../token-order-bulk/token-orders-bulk-management.component';


@Component({
  selector: 'student-groups-management',
  templateUrl: './student-groups-management.component.html',
  styleUrls: ['./student-groups-management.component.css']
})
export class StudentGroupsManagementComponent implements OnInit {
  columns: any[] = [];
  rows: StudentGroup[] = [];
  rowsCache: StudentGroup[] = [];
  allPermissions: Permission[] = [];
  editedGroup: StudentGroup;
  sourceGroup: StudentGroup;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @Input() isHideHeader: boolean;

  @Input() outletId: string;

  @ViewChild('fileImport')
  fileImport: ElementRef;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('fasTemplate')
  fasTemplate: TemplateRef<any>;

  @ViewChild('studentEditorComponent')
  studentEditorComponent: StudentGroupEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private studentService: StudentService, public dialog: MatDialog) {
  }

  openDialog(group: StudentGroup): void {
    const dialogRef = this.dialog.open(StudentGroupEditorComponent, {
      data: { header: this.header, group: group, outletId: this.outletId },
      width: '1000px',
      disableClose: true
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
      { prop: 'code', name: 'Code' },
      { prop: 'name', name: 'Name' },
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
    let f = this.outletId ? '(OutletId)==' + this.outletId + ',' : '';
    this.filter.filters = f + '(IsActive)==true,(Name)@=' + this.keyword;
    
    this.studentService.getStudentGroupsByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let groups = results.pagedData;

        groups.forEach((group, index, students) => {
          (<any>group).index = index + 1;
        });


        this.rowsCache = [...groups];
        this.rows = groups;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve records from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }


  onSearchChanged(value: string) {
    this.keyword = value;
    this.loadData(null);
  }

  newGroup() {
    this.header = 'New Student Group';
    this.editedGroup = new StudentGroup();
    this.openDialog(this.editedGroup);
  }


  editGroup(row: StudentGroup) {
    this.editedGroup = row;
    this.header = 'Edit Student Group';
    this.openDialog(this.editedGroup);
  }

  deleteGroup(row: StudentGroup) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" student group?', DialogType.confirm, () => this.deleteGroupHelper(row));
  }


  deleteGroupHelper(row: StudentGroup) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.studentService.deleteStudentGroup(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the student group.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  openTokenOrder(row) {
    const dialogRef = this.dialog.open(TokenOrdersBulkManagementComponent, {
      data: { header: "Meal Order", group: row },
      width: '1200px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData();
    });
  }

  get canManageStudents() {
    return true;
  }

}
