import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog, MatDatepickerInputEvent } from '@angular/material';
import { Student } from 'src/app/models/meal-order/student.model';
import { StudentGroup } from 'src/app/models/meal-order/student-group.model';
import { PrintAllocationComponent } from './print-allocation.component';
import { StudentService } from 'src/app/services/meal-order/student.service';
import { HttpEvent, HttpEventType } from '@angular/common/http';
import { TokenOrdersBulkManagementComponent } from './../token-order-bulk/token-orders-bulk-management.component';
import { TokenLabel, TokenDishLabel, MealAllocation } from 'src/app/models/meal-order/token-order.model';
import { MenuService } from 'src/app/services/meal-order/menu.service';

import { MealService } from 'src/app/services/meal-order/meal.service';
import { MealSessionDetail, MealSession } from 'src/app/models/meal-order/meal-session.model';
import { PipeTransform } from '@angular/core';
import * as moment from 'moment';


@Component({
  selector: 'meal-allocations-management',
  templateUrl: './meal-allocations-management.component.html',
  styleUrls: ['./meal-allocations-management.component.css']
})
export class MealAllocationssManagementComponent implements OnInit {
  columns: any[] = [];
  rows: MealAllocation[] = [];
  rowsCache: MealAllocation[] = [];
  allPermissions: Permission[] = [];
  editedAllocation: MealAllocation;
  sourceAllocation: MealAllocation;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';
  private sessions: MealSessionDetail[] = [];
  allSessions: MealSession[] = [];
  start = new Date();

  @Input() isHideHeader: boolean;

  @Input() outletId: string;

  @ViewChild('fileImport')
  fileImport: ElementRef;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('fasTemplate')
  fasTemplate: TemplateRef<any>;

  @ViewChild('printAllocationComponent')
  printAllocationComponent: PrintAllocationComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private studentService: StudentService, public dialog: MatDialog, private menuService: MenuService, private mealService: MealService) {
  }

  openDialog(allocation: MealAllocation): void {
    const dialogRef = this.dialog.open(PrintAllocationComponent, {
      data: { header: this.header, allocation: allocation, outletId: this.outletId },
      width: '1000px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData(null);
    });
  }

  initializeFilter() {
    this.filter = new Filter(1, 10);
    this.filter.sorts = 'deliveryDate';
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
      { prop: 'deliveryDate', name: 'Delivery Date' },
      //{ prop: '', name: 'Name' },
      { prop: 'mealSessionId', name: 'Session', pipe: this.pipeSessionName() },
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
    this.getAllSessions();
    //this.loadData();
  }

  getAllSessions() {
    let filter = new Filter();
    //let f = this.catererId ? '(CatererId)==' + this.catererId + ',' : '';
    filter.filters = '(IsActive)==true';
    this.mealService.getMealSessionsByFilter(filter)
      .subscribe(results => {
        this.allSessions = results.pagedData;
        console.log("all sessions: ", this.allSessions);
        this.allSessions.forEach((session, index) => {
          session.details.forEach((detail, index) => {
            this.sessions.push(detail);
          })
        })
        console.log("all session detail: ", this.sessions)
        this.loadData();
      },
        error => {
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving meal type.\r\n"`,
            MessageSeverity.error);
        })
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
    this.filter.filters = f + '(IsActive)==true,(DeliveryDate)==' + this.start.toDateString();
    //this.filter.filters = f + '(IsActive)==true';

    this.menuService.getMealAllocationsByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let allocations = results.pagedData;

        allocations.forEach((allocation, index, students) => {
          (<any>allocation).index = index + 1;
        });


        this.rowsCache = [...allocations];
        this.rows = allocations;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve records from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  pipeSessionName(): PipeTransform {
    return {
      transform: (value) => {
        let session = this.sessions.find(x => x.id === value);
        if (session) {
          return session.name
        } else {
          return ''
        }
      }
    }
  }

  onChangeDate(type: string, event: MatDatepickerInputEvent<Date>) {
    if (type == 'start') {
      this.start = new Date(event.value);
    }

    this.loadData();

    //this.loadData();
  }


  onSearchChanged(value: string) {
    this.keyword = value;
    this.loadData(null);
  }

  newAllocation() {
    this.header = 'New Meal Allocations';
    this.editedAllocation = new MealAllocation();
    this.openDialog(this.editedAllocation);
  }


  editAllocation(row: MealAllocation) {
    this.editedAllocation = row;
    this.header = 'Edit Meal Allocation';
    this.openDialog(this.editedAllocation);
  }

  deleteAllocation(row: MealAllocation) {
    this.alertService.showDialog('Are you sure you want to delete Meal Allocation?', DialogType.confirm, () => this.deleteAllocationHelper(row));
  }


  deleteAllocationHelper(row: MealAllocation) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.menuService.deleteMealAllocation(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the Meal Allocation.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  get canManageMealAllocations() {
    return this.accountService.userHasPermission(Permission.manageMOSOutletMgtMealAllocationsPermission)
  }

}
