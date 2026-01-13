import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, Inject } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { Menu } from 'src/app/models/meal-order/menu.model';
import { TokenOrderEditorComponent } from './token-order-editor.component';
import { MenuService } from 'src/app/services/meal-order/menu.service';
import { DateOnlyPipe } from 'src/app/pipes/datetime.pipe';
import { TokenOrder } from 'src/app/models/meal-order/token-order.model';
import { MealPeriod } from 'src/app/models/meal-order/meal-period.model';
import { Student } from 'src/app/models/meal-order/student.model';
import { StudentService } from 'src/app/services/meal-order/student.service';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { PipeTransform } from '@angular/core';
import { DateAdapter, MatDatepickerInputEvent, MatDialog, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { TokenOrderPaymentComponent } from '../token-order-payment/token-order-payment.component';



@Component({
  selector: 'token-orders-management',
  templateUrl: './token-orders-management.component.html',
  styleUrls: ['./token-orders-management.component.css']
})
export class TokenOrdersManagementComponent implements OnInit {
  columns: any[] = [];
  rows: TokenOrder[] = [];
  rowsCache: TokenOrder[] = [];
  allPermissions: Permission[] = [];
  editedOrder: TokenOrder;
  sourceOrder: TokenOrder;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';
  @Input() isHideHeader: boolean;
  private students: Student[] = [];
  private periods: MealPeriod[] = [];
  selectedStudent: Student;

  rowTokenOrderStatus;
  rowTokenOrderStatusFlag: boolean;
 
  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('flagTemplate')
  flagTemplate: TemplateRef<any>;

  @ViewChild('ordersTable') table: any;

  @ViewChild('tokenOrderEditor')
  tokenOrderEditor: TokenOrderEditorComponent;
  header: string;


  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private menuService: MenuService, public dialog: MatDialog, private studentService: StudentService, private mealService: MealService,
    public dialogRef: MatDialogRef<TokenOrdersManagementComponent>, @Inject(MAT_DIALOG_DATA) public data: any) {

    if (typeof (data.student) != typeof (undefined)) {
      if (data.student) {
        this.selectedStudent = data.student;
      }
    }
  }

  openDialog(tokenOrder: TokenOrder): void {
    const dialogRef = this.dialog.open(TokenOrderEditorComponent, {
      data: { header: this.header, tokenOrder: tokenOrder, student: this.selectedStudent },
      width: '1000px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData(null);
    });
  }

  initializeFilter() {
    this.filter = new Filter(1, 10);
    this.filter.sorts = 'transactionTime';
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
      { prop: 'transactionTime', name: 'Transaction Time', pipe: new DateOnlyPipe('en-SG') },
      { prop: 'profileId', name: 'Student', pipe: this.pipeStudentName() },
      { prop: 'periodId', name: 'Period', pipe: this.pipePeriodName() },
      { prop: 'status', name: 'Status' },
      { prop: 'remarks', name: 'Remarks' },
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
    this.getStudents();
    this.getPeriods();
  }

  getStudents() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.studentService.getSimpleStudentsByFilter(filter)
      .subscribe(results => {
        this.students = results.pagedData;
        console.log("students: ", this.students)
        this.loadData();
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving students.\r\n"`,
            MessageSeverity.error);
        })
  }

  getPeriods() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.mealService.getMealPeriodsByFilter(filter)
      .subscribe(results => {
        this.periods = results.pagedData;
        console.log("periods: ", this.periods)
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving meal periods.\r\n"`,
            MessageSeverity.error);
        })
  }


  loadData(ev?: any) {
    this.alertService.startLoadingMessage();
    this.loadingIndicator = true;
    this.table.offset = 0;
    this.filter.pageSize = 10;

    if (ev) {
      this.filter.page = ev.offset + 1;
      if (ev.sorts) {
        this.filter.sorts = ev.sorts[0].dir == 'desc' ? '-' + ev.sorts[0].prop : ev.sorts[0].prop;
      }
    }

    if (!this.keyword) this.keyword = '';
    let f = this.selectedStudent ? '(profileId)==' + this.selectedStudent.id + ',' : '';
    //let f = this.catererId ? '(CatererId)==' + this.catererId + ',' : '';
    //this.filter.filters = '(IsActive)==true,(Code|Label)@=' + this.keyword;
    this.filter.filters = f + '(IsActive)==true';

    console.log("filter: ", this.filter)
    
    this.menuService.getTokenOrdersByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let orders = results.pagedData;

        console.log("order: ", orders)

        orders.forEach((tokenOrder, index, orders) => {
          (<any>tokenOrder).index = index + 1;

        });

        this.rowsCache = [...orders];
        this.rows = orders;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve records from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }


  canMakeOrderPayment(row: TokenOrder, rowIndex:number)
  {
    let pagedMaxNumPages: number;
 
    pagedMaxNumPages = this.pagedResult.pagedData.length;

    //console.log("paged data : " + pagedMaxNumPages);
    //console.log("canMakeOrderPayment : " + row.id);

    if (row.status.toLowerCase().trim() == "pending")
    {
     // console.log("match found set flag : " + row.id);
      return true;
    }
    else
    {
     // console.log("no match set flag false : " + row.id);
      return false;
    }

  }


 

 



  onSearchChanged(value: string) {
    this.keyword = value;
    this.loadData(null);
  }

  newOrder() {
    this.header = 'New Order';
    this.editedOrder = new TokenOrder();
    this.openDialog(this.editedOrder);
  }


  editOrder(row: TokenOrder) {
    this.editedOrder = row;
    this.header = 'Edit Order';
    //this.editedOrder.catererId = this.catererId;
    this.openDialog(this.editedOrder);
  }

  deleteOrder(row: TokenOrder) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.id + '\" order?', DialogType.confirm, () => this.deleteOrderHelper(row));
  }


  deleteOrderHelper(row: TokenOrder) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.menuService.deleteTokenOrder(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the Order.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  pipeStudentName(): PipeTransform {
    return {
      transform: (value) => {
        let student = this.students.find(x => x.id === value);
        if (student) {
          return student.name
        } else {
          return ''
        }
      }
    }
  }

  pipePeriodName(): PipeTransform {
    return {
      transform: (value) => {
        let period = this.periods.find(x => x.id === value);
        if (period) {
          return period.name
        } else {
          return ''
        }
      }
    }
  }

  private cancel() {

    this.alertService.resetStickyMessage();

    this.dialogRef.close();
  }

  get canManageOrders() {
    return true; //this.accountService.userHasPermission(Permission.manageMenusPermission)
  }


  //
  payOrder(row: TokenOrder) {
   
    this.header = 'Payment for Order';
 
    const dialogRef = this.dialog.open(TokenOrderPaymentComponent, {
      data: { header: this.header, tokenOrder: row, student: this.selectedStudent },
      width: '1000px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData(null);
    });
 
  }





}
