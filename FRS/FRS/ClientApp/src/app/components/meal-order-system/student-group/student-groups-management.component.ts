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
import { MealPlanSummaryComponent } from './meal-plan/meal-plans-summary.component';
import { StudentGroupOrderSummaryComponent } from './individual-order/individual-orders-summary.component';
import * as moment from 'moment';
import { saveAs } from 'file-saver';
import { StudentWalletTopupComponent } from '../student/student-wallet-topup/student-wallet-topup.component';
import { StudentPointTopupComponent } from '../student/student-point-topup/student-point-topup.component';


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
  selectedStudentGroupId: string = ''

  public progress: number;
  public message: string;
  public filename: string;

  @Input() isHideHeader: boolean;

  @Input() outletId: string;

  @Input() mealCollectionType: string;

  @ViewChild('fileImport')
  fileImport: ElementRef;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('fasTemplate')
  fasTemplate: TemplateRef<any>;

  @ViewChild('totalStudents')
  totalStudents: TemplateRef<any>;

  @ViewChild('activeTemplate')
  activeTemplate: TemplateRef<any>;

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
      if (!result)
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
      { prop: 'type', name: 'Type' },
      {
        name: 'Students',
        cellTemplate: this.totalStudents,
        sortable: false
      },
      {
        name: 'Active',
        cellTemplate: this.activeTemplate
      },
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

    console.log('mealCollectionType',this.mealCollectionType);
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
      //this.loadData();
    });
  }

  openMealPlanSummary(row) {
    const dialogRef = this.dialog.open(MealPlanSummaryComponent, {
      data: { header: "Meal Order", group: row, outletId: this.outletId },
      width: '1200px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
    });
  }

  openStudentGroupSummary(row) {
    const dialogRef = this.dialog.open(StudentGroupOrderSummaryComponent, {
      data: { header: "Student Group Order", group: row, outletId: this.outletId, mealCollectionType: this.mealCollectionType },
      width: '1200px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
    });
  }

  getMealSessionName() {

  }

  get canManageStudents() {
    return this.accountService.userHasPermission(Permission.manageMOSOutletMgtStudentGroupsPermission)
  }

  exportListStudent(studentGroupId: number) {
    const fileName = moment().format('DDMMYYYY_hhmmss') + '_StudentList.xlsx';
    this.studentService.studentListExport(parseInt(this.outletId), studentGroupId).subscribe(
      data => {
        saveAs(data, fileName);
      },
      err => {
        alert("Problem while downloading the file.");
        console.error(err);
      }
    );
  }

  onImportClick(id: string, fileInput: HTMLInputElement) {
    this.selectedStudentGroupId = id;
    fileInput.click();
  }

  importedFile = (files) => {
    if (files.length === 0) {
      return;
    }

    if (!confirm(`Are you sure to import file '${files[0].name}'? \nImport cannot be undone after the file uploaded.`)) return;

    let fileToUpload = <File>files[0];
    const userId = this.accountService.currentUser.id;
    const formData = new FormData();
    formData.append('file', fileToUpload, fileToUpload.name);
    formData.append('studentGroupId', this.selectedStudentGroupId.toString());
    formData.append('userId', userId.toString());

    this.loadingIndicator = true;
    this.alertService.startLoadingMessage("Uploading...");
    this.studentService.importFileStudentGroup<HttpEvent<Object>>(formData)
      .subscribe(event => {
        if (event.type === HttpEventType.UploadProgress)
          this.progress = Math.round(100 * event.loaded / event.total);
        else if (event.type === HttpEventType.Response) {
          this.message = 'Upload success.';
          this.onUploadFinished(event.body);
          if (this.fileImport && this.fileImport.nativeElement) {
            this.fileImport.nativeElement.value = "";
          }
        }

        this.loadingIndicator = false;
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          if (this.fileImport && this.fileImport.nativeElement) {
            this.fileImport.nativeElement.value = "";
          }

          this.alertService.showStickyMessage("Import Error", `Unable to import the file to the server.`,
            MessageSeverity.error);
        });
  }

  onUploadFinished(response: any) {
    this.alertService.stopLoadingMessage();
    if (response.isSuccess) {
      this.alertService.showMessage(response.message);
      this.loadData();
    } else {
      this.alertService.showStickyMessage("Import Error", `Unable to import the file to the server.\r\nErrors: "${response.message}"`,
        MessageSeverity.error);
    }

  }

  addVoucher(studentGroupId: string) {
    this.alertService.showDialog('Enter Voucher Code:', DialogType.prompt, (val) => {
      if (!val) return;
      this.alertService.startLoadingMessage("Verifying...");
      this.studentService.addStudentVoucherByStudentGroup(studentGroupId, val)
        .subscribe(response => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;
          this.alertService.showMessage(response.message);
          if (response.data && response.data.length > 0) {
            const messageData = response.data.join("<br/><br/>")
            this.alertService.showStickyMessage("Voucher Info", messageData, MessageSeverity.info)
          }

        },
          error => {
            this.alertService.stopLoadingMessage();
            this.loadingIndicator = false;
            this.alertService.showStickyMessage("Voucher Error", `Unable to add voucher.`,
              MessageSeverity.error);
          });
    }, () => {
    });
  }

  addWalletTopup(studentId: string) {
    const dialogRef = this.dialog.open(StudentWalletTopupComponent, {
      data: {
        studentId: studentId,
        isStudentGroup: true
      },
      width: '350px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {

    });
  }

  addPointTopup(studentId: string) {
    const dialogRef = this.dialog.open(StudentPointTopupComponent, {
      data: {
        studentId: studentId,
        isStudentGroup: true
      },
      width: '350px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {

    });
  }


}
