import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, ElementRef, Inject, ChangeDetectionStrategy, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Student } from 'src/app/models/meal-order/student.model';
import { StudentEditorComponent } from './student-editor.component';
import { StudentService } from 'src/app/services/meal-order/student.service';
import { HttpEvent, HttpEventType } from '@angular/common/http';
import { DeliveryService } from '../../../services/meal-order/delivery.service';
import { TokenOrdersManagementComponent } from './../token-order/token-orders-management.component';
import { StudentNotificationDialogComponent } from './notification-dialog.component';
import { getBaseUrl } from 'src/app/app.module';
import * as moment from 'moment';
import { saveAs } from 'file-saver';
import { Subscription } from 'rxjs';
import { ClassService } from '../../../services/meal-order/class.service';

@Component({
  selector: 'students-management',
  templateUrl: './students-management.component.html',
  styleUrls: ['./students-management.component.css']
})
export class StudentsManagementComponent implements OnInit, OnDestroy {

  private subscription: Subscription = new Subscription();
  @ViewChild('studentsTable') table: any;
  expanded: any = {};

  columns: any[] = [];
  rows: Student[] = [];
  rowsCache: Student[] = [];
  allPermissions: Permission[] = [];

  public batches = [];
  batchId: string;

  editedStudent: Student;
  sourceStudent: Student;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @Input() isHideHeader: boolean;

  @Input() outletId: string;

  @ViewChild('fileImport')
  fileImport: ElementRef;

  @ViewChild('fileStudentCardImport')
  fileStudentCardImport: ElementRef;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('fasTemplate')
  fasTemplate: TemplateRef<any>;

  @ViewChild('studentEditorComponent')
  studentEditorComponent: StudentEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService, private classService: ClassService,
    private studentService: StudentService, public dialog: MatDialog, public cdr: ChangeDetectorRef) {
  }

  openDialog(student: Student): void {
    const dialogRef = this.dialog.open(StudentEditorComponent, {
      data: { header: this.header, student: student, outletId: this.outletId },
      width: '800px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result) this.loadData(null);
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
      { prop: 'outletName', name: 'Outlet' },
      { prop: 'name', name: gT('common.Name') },
      { prop: 'classBatchName', name: 'Batch' },
      { prop: 'classLevelName', name: 'Class Level' },
      { prop: 'className', name: 'Class' },
      { prop: 'gender', name: 'Gender' },
      { name: 'Is FAS', cellTemplate: this.fasTemplate },
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

    this.getClassBatches();
  }

  ngOnDestroy() {
    this.alertService.resetStickyMessage();
    this.subscription.unsubscribe();
  }

  getClassBatches() {
    let filter = new Filter();
    let f = this.outletId ? '(OutletId)==' + this.outletId + ',' : '';
    filter.filters = f + '(IsActive)==true';
    filter.sorts = 'year';

    this.subscription.add(this.classService.getClassBatchesByFilter(filter)
      .subscribe(results => {
        this.batches = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving batches.\r\n"`,
            MessageSeverity.error);
        }));
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

    if (!this.keyword) {
      this.keyword = '';
    } else {
      this.keyword = this.keyword.replace(',', '^^^').replace('|', '***').replace('(', '@@@').replace(')', '###');
    }
    let f = this.outletId ? '(OutletId)==' + this.outletId + ',' : '';
    f += this.batchId ? '(ClassBatchId)==' + this.batchId + ',' : '';
    this.filter.filters = f + '(IsActive)==true,(StudentPostSearch)@=' + this.keyword;

    this.subscription.add(this.studentService.getStudentsByFilter(this.filter)
      .subscribe(results => {
        console.log("Filter", this.filter)

        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let students = results.pagedData;

        students.forEach((student, index, students) => {
          (<any>student).index = index + 1;
        });


        this.rowsCache = [...students];
        this.rows = students;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve records from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }


  onSearchChanged(value: string) {
    this.keyword = value;
    //this.loadData(null);
  }

  onSearch() {
    this.loadData(null);
  }

  newStudent() {
    this.header = 'New Student';
    this.editedStudent = new Student();
    this.openDialog(this.editedStudent);
  }


  editStudent(row: Student) {
    this.editedStudent = row;
    this.header = 'Edit Student';
    this.openDialog(this.editedStudent);
  }

  deleteStudent(row: Student) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" student?', DialogType.confirm, () => this.deleteStudentHelper(row));
  }


  deleteStudentHelper(row: Student) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.studentService.deleteStudent(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the student.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  addVoucher(row: Student) {
    this.alertService.showDialog('Enter Voucher Code:', DialogType.prompt, (val) => {
      if (!val) return;
      this.studentService.addStudentVoucher(row.id, val)
        .subscribe(response => {
          this.loadingIndicator = false;
          this.alertService.showMessage(response.message);
          this.loadData();
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

  triggerNotification(student: Student): void {
    const dialogRef = this.dialog.open(StudentNotificationDialogComponent, {
      data: { header: 'Trigger notification', student: student },
      width: '400px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
    });
  }

  public importedFile = (files) => {
    if (files.length === 0) {
      return;
    }

    if (!confirm(`Are you sure to import file '${files[0].name}'? \nImport cannot be undone after the file uploaded.`)) return;

    let fileToUpload = <File>files[0];
    const formData = new FormData();
    formData.append('file', fileToUpload, fileToUpload.name);

    this.loadingIndicator = true;
    this.alertService.startLoadingMessage("Uploading...");
    this.subscription.add(this.studentService.importFile<HttpEvent<Object>>(formData)
      .subscribe(event => {
        if (event.type === HttpEventType.UploadProgress) { }
        //this.progress = Math.round(100 * event.loaded / event.total);
        else if (event.type === HttpEventType.Response) {
          //this.message = 'Upload success.';
          //var fileUploadResponse: any = ;
          //this.filename = fileUploadResponse.fileName;
          this.onUploadFinished(event.body);
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
        }));
  }

  onUploadFinished(response: any) {
    this.alertService.stopLoadingMessage();
    if (response.isSuccess) {
      this.alertService.showMessage(response.message);
      this.loadData(null);
    } else {
      if (this.fileImport && this.fileImport.nativeElement) {
        this.fileImport.nativeElement.value = "";
      }
      this.alertService.showStickyMessage("Import Error", `Unable to import the file to the server.\r\nErrors: "${response.message}"`,
        MessageSeverity.error);
    }

  }

  public importedStudentCardFile = (files) => {
    if (files.length === 0) {
      return;
    }

    this.alertService.resetStickyMessage();
    if (!confirm(`Are you sure to import file '${files[0].name}'? \nImport cannot be undone after the file uploaded.`)) return;

    this.alertService.showDialog('Enter Batch Year:', DialogType.prompt, (val) => {
      if (isNaN(val)) return;
      let fileToUpload = <File>files[0];
      const formData = new FormData();
      formData.append('file', fileToUpload, fileToUpload.name);
      formData.append('outletId', this.outletId);
      formData.append('batch', val);

      this.loadingIndicator = true;
      this.alertService.startLoadingMessage("Uploading...");
      this.studentService.importStudentCardFile<HttpEvent<Object>>(formData)
        .subscribe(event => {
          if (event.type === HttpEventType.UploadProgress) { }
          //this.progress = Math.round(100 * event.loaded / event.total);
          else if (event.type === HttpEventType.Response) {
            //this.message = 'Upload success.';
            //var fileUploadResponse: any = ;
            //this.filename = fileUploadResponse.fileName;
            this.onUploadStudentCardFinished(event.body);
          }

          this.loadingIndicator = false;
        },
          error => {
            this.alertService.stopLoadingMessage();
            this.loadingIndicator = false;

            if (this.fileStudentCardImport && this.fileStudentCardImport.nativeElement) {
              this.fileStudentCardImport.nativeElement.value = "";
            }

            this.alertService.showStickyMessage("Import Error", `Unable to import the file to the server.`,
              MessageSeverity.error);
          });
    }, () => {
      if (this.fileStudentCardImport && this.fileStudentCardImport.nativeElement) {
        this.fileStudentCardImport.nativeElement.value = "";
      }
    });
  }

  onUploadStudentCardFinished(response: any) {
    this.alertService.stopLoadingMessage();
    if (this.fileStudentCardImport && this.fileStudentCardImport.nativeElement) {
      this.fileStudentCardImport.nativeElement.value = "";
    }

    if (response.isSuccess) {
      this.alertService.showMessage(response.message);
      this.loadData(null);
    } else {
      this.alertService.showMessage("Import Error", `Unable to import the file to the server.\r\nErrors: "${response.message}"`,
        MessageSeverity.error);
    }

  }

  downloadResults() {
    const fileName = moment().format('DDMMYYYY_hhmmss') + '_StudentReport.xlsx';

    let f = new Filter(-1, -1);
    f.filters = this.filter.filters;
    f.sorts = this.filter.sorts;
    f.page = -1;
    this.studentService.downloadReport(f).subscribe(
      data => {
        console.log(data);
        saveAs(data, fileName);
      },
      err => {
        alert("Problem while downloading the file.");
        console.error(err);
      }
    );
  }

  downloadTemplate() {
    let path = '/Resources/Templates/Student Import Template.xlsx';
    window.open(getBaseUrl() + '/gateway/Download/FileByPath?filePath=/' + encodeURIComponent(path), '_blank');
  }

  downloadTemplateWithStudent() {
    this.alertService.startLoadingMessage("Downloading...");
    const fileName = moment().format('DDMMYYYY_hhmmss') + '_Template with Student Information.xlsx';

    let f = new Filter(-1, -1);
    f.filters = this.filter.filters;
    f.sorts = this.filter.sorts;
    f.page = -1;
    this.studentService.downloadTemplateWithStudent(f).subscribe(
      data => {
        console.log(data);
        saveAs(data, fileName);

        this.alertService.stopLoadingMessage();
      },
      err => {
        this.alertService.stopLoadingMessage();
        alert("Problem while downloading the file.");
        console.error(err);
      }
    );

    //const fileName = 'YISS Student or Staff Import Template.xlsx';

    //let f = new Filter(-1, -1);
    //f.filters = this.filter.filters;
    //f.sorts = this.filter.sorts;
    //f.page = -1;
    //this.studentService.downloadTemplateStudentCardFile().subscribe(
    //  data => {
    //    console.log(data);
    //    saveAs(data, fileName);
    //  },
    //  err => {
    //    alert("Problem while downloading the file.");
    //    console.error(err);
    //  }
    //);
  }

  openTokenOrder(row) {
    const dialogRef = this.dialog.open(TokenOrdersManagementComponent, {
      data: { header: "Meal Order", student: row },
      width: '1200px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData();
    });
  }


  createAccount() {
    const dialogRef = this.dialog.open(CreateAccountMultiple, {
      data: { outletId: this.outletId },
      width: '60vw',
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result && !result.isCancel)
        setTimeout(() => this.loadData(null), 2000);
    });
  }

  openStudentCalendar(student) {
    window.open(getBaseUrl() + '/studentcalendars/' + student.id, '_blank');
  }



  // student orders
  toggleExpandRow(row) {
    console.log('Toggled Expand Row!', row);
    this.table.rowDetail.toggleExpandRow(row);
  }

  onDetailToggle(event) {
    console.log('Detail Toggled', event);
  }

  addWalletTopup(studentId: string) {
    this.alertService.showDialog('Enter Top up Amount:', DialogType.prompt, (val: string) => {
      if (!val) return;
      const normalized = val.replace(',', '.');
      const amount = parseFloat(normalized);
      if (isNaN(amount) || amount <= 0) {
        this.alertService.showStickyMessage(
          "Invalid Input",
          "Please enter a valid positive amount (numbers only, decimals allowed).",
          MessageSeverity.error
        );
        return;
      }

      this.alertService.startLoadingMessage("Processing top-up...");
      this.studentService.walletTopupForStudent(studentId, amount, this.accountService.currentUser.id)
        .subscribe({
          next: (response) => {
            this.alertService.stopLoadingMessage();
            this.loadingIndicator = false;
            this.alertService.showMessage(response.message);

            if (response.data && response.data.length > 0) {
              const messageData = response.data.join("<br/><br/>");
              this.alertService.showStickyMessage("Top-up Info", messageData, MessageSeverity.info);
            }
          },
          error: () => {
            this.alertService.stopLoadingMessage();
            this.loadingIndicator = false;
            this.alertService.showStickyMessage(
              "Wallet Top-up Error",
              "Unable to add amount.",
              MessageSeverity.error
            );
          }
        });
    }, () => {
      // Cancel callback
    });
  }

  addPointTopup(studentId: string) {
    this.alertService.showDialog('Enter Top up Point:', DialogType.prompt, (val: string) => {
      if (!val) return;
      const normalized = val.replace(',', '.');
      const amount = parseFloat(normalized);
      if (isNaN(amount) || amount <= 0) {
        this.alertService.showStickyMessage(
          "Invalid Input",
          "Please enter a valid positive point.",
          MessageSeverity.error
        );
        return;
      }

      this.alertService.startLoadingMessage("Processing top-up...");
      this.studentService.pointTopupForStudent(studentId, amount)
        .subscribe({
          next: (response) => {
            this.alertService.stopLoadingMessage();
            this.loadingIndicator = false;
            this.alertService.showMessage(response.message);

            if (response.data && response.data.length > 0) {
              const messageData = response.data.join("<br/><br/>");
              this.alertService.showStickyMessage("Top-up Info", messageData, MessageSeverity.info);
            }
          },
          error: () => {
            this.alertService.stopLoadingMessage();
            this.loadingIndicator = false;
            this.alertService.showStickyMessage(
              "Point Top-up Error",
              "Unable to add Point.",
              MessageSeverity.error
            );
          }
        });
    }, () => {
      // Cancel callback
    });
  }



  get canManageStudents() {
    return true; //this.accountService.userHasPermission(Permission.manageStudentsPermission)
  }

  get canImportStudents() {
    return this.accountService.userHasPermission(Permission.viewMOSOutletStudentMgtImportPermission)
  }

  get canAddStudent() {
    return this.accountService.userHasPermission(Permission.manageMOSOutletStudentMgtPermissionNew)
  }

  get canEditStudent() {
    return this.accountService.userHasPermission(Permission.manageMOSOutletStudentMgtPermissionEdit)
  }

  get canDeleteStudent() {
    return this.accountService.userHasPermission(Permission.manageMOSOutletStudentMgtPermissionDelete)
  }

  get canCreateStudentAccount() {
    return this.accountService.userHasPermission(Permission.manageMOSOutletStudentMgtPermissionCreateAccount)
  }

  get canManageStudentOrderMgt() {
    return this.accountService.userHasPermission(Permission.manageMOSOutletStudentMgtPermissionOrderMgt)
  }

  get canManageStudentVoucher() {
    return this.accountService.userHasPermission(Permission.manageMOSOutletStudentMgtPermissionVoucher)
  }

  get canManageStudentNotification() {
    return this.accountService.userHasPermission(Permission.manageMOSOutletStudentMgtPermissionNotification)
  }

  get canManageStudentCalendar() {
    return this.accountService.userHasPermission(Permission.manageMOSOutletStudentMgtPermissionCalendar)
  }

}


// TODO: Move this to another file
@Component({
  selector: 'create-account-multiple',
  templateUrl: 'create-account-multiple.html',
})
export class CreateAccountMultiple {

  bentoAssetCode: any;
  bentoBoxType: any;
  bentoBoxTypes: any;
  searchTimeout: NodeJS.Timer;
  map: any = {};
  rows: any = [];

  loadingIndicator: boolean;
  pagedResult: PagedResult;

  page: number;
  pageSize: number = 10;
  maxPage: number;
  prevPage: number;

  selected: Student[] = [];

  search: string = '';
  prevSearch
  outletId: string;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;

  @ViewChild('selectedContainer') private selectedContainer: ElementRef;

  constructor(private alertService: AlertService, private accountService: AccountService,
    public dialogRef: MatDialogRef<any>,
    @Inject(MAT_DIALOG_DATA) public data: any, public studentService: StudentService) {
    this.outletId = data.outletId;
    this.loadData();
  }

  ngOnInit() {

  }

  pageChanged(newValue) {
    if (this.prevPage !== newValue) {
      this.page = newValue;
      this.prevPage = this.page;

      this.loadData();
    }
  }

  searchChanged(newValue) {
    clearTimeout(this.searchTimeout)
    this.searchTimeout = setTimeout(() => {
      this.search = newValue;
      this.loadData();
    }, 500);
  }

  checkValue(value, row) {
    if (value) {
      this.selected.unshift(row);
      //try {
      //  this.selectedContainer.nativeElement.scrollTop = this.selectedContainer.nativeElement.scrollHeight + 1000;
      //} catch (err) { }
    } else {
      this.selected = this.selected.filter(s => s.id != row.id);
    }
  }

  checkSelected(value, row) {
    if (!value) {
      this.selected = this.selected.filter(s => s.id != row.id);
      let r = this.rows.find(s => s.id != row.id);
      if (r) r.checked = value;
    }
  }

  loadData() {
    console.log('OutletId', this.outletId);
    let f = this.outletId ? '(OutletId)==' + this.outletId + ',' : '';

    let filter: any = {
      filters: f + "(IsActive)==true,(Name)@=" + this.search,
      page: this.page || 1,
      pageSize: this.pageSize,
      sorts: "name",
      noAccount: true
    }

    this.studentService.getStudentsByFilter(filter)
      .subscribe(results => {

        this.pagedResult = results;

        this.page = this.page || this.pagedResult && this.pagedResult.filter && this.pagedResult.filter.page ? this.pagedResult.filter.page : 1;
        this.prevPage = this.page;
        this.maxPage = Math.ceil(this.pagedResult.totalCount / this.pageSize);

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let students = results.pagedData;

        this.rows = students;

        for (let row of this.rows) {
          if (this.selected.some(s => s.id === row.id)) row.checked = true;
        }

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve records from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });

  }


  save() {
    let ids = this.selected.map(s => s.id);

    this.studentService.createAccount(ids).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
  }

  private saveSuccessHelper() {
    this.alertService.stopLoadingMessage();

    this.alertService.showMessage("Success", `Accounts was created successfully`, MessageSeverity.success);


    if (this.changesSavedCallback)
      this.changesSavedCallback();

    this.dialogRef.close();
  }


  private saveFailedHelper(error: any) {

    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage("Save Error", "The below errors occured while saving your changes:", MessageSeverity.error);
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);

    if (this.changesFailedCallback)
      this.changesFailedCallback();
  }

  private cancel() {
    this.dialogRef.close({ isCancel: true });
  }
}
