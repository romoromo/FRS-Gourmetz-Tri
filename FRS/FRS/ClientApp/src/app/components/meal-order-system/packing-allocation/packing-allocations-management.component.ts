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
import { PackingAllocationEditorComponent } from './packing-allocation-editor.component';
import { StudentService } from 'src/app/services/meal-order/student.service';
import { HttpEvent, HttpEventType } from '@angular/common/http';
import { TokenOrdersBulkManagementComponent } from './../token-order-bulk/token-orders-bulk-management.component';
import { TokenLabel, TokenDishLabel, MealAllocation, PackingAllocation } from 'src/app/models/meal-order/token-order.model';
import { Route } from 'src/app/models/meal-order/route.model';
import { MenuService } from 'src/app/services/meal-order/menu.service';
import { DateOnlyPipe } from 'src/app/pipes/datetime.pipe';
import { DeliveryService } from 'src/app/services/meal-order/delivery.service';
import { PipeTransform } from '@angular/core';


@Component({
  selector: 'packing-allocations-management',
  templateUrl: './packing-allocations-management.component.html',
  styleUrls: ['./packing-allocations-management.component.css']
})
export class PackingAllocationssManagementComponent implements OnInit {
  columns: any[] = [];
  rows: PackingAllocation[] = [];
  rowsCache: PackingAllocation[] = [];
  allPermissions: Permission[] = [];
  editedAllocation: PackingAllocation;
  sourceAllocation: PackingAllocation;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';
  private routes: Route[] = [];
  start = new Date();

  @Input() isHideHeader: boolean;

  @Input() outletId: string;

  @ViewChild('fileImport')
  fileImport: ElementRef;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('fasTemplate')
  fasTemplate: TemplateRef<any>;

  @ViewChild('packingAllocationEditorComponent')
  packingAllocationEditorComponent: PackingAllocationEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private studentService: StudentService, public dialog: MatDialog, private menuService: MenuService, private deliveryService: DeliveryService) {
  }

  openDialog(allocation: PackingAllocation): void {
    const dialogRef = this.dialog.open(PackingAllocationEditorComponent, {
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
    this.filter.sorts = 'packingDate';
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
      { prop: 'packingDate', name: 'Packing Date', pipe: new DateOnlyPipe('en-SG') },
      { prop: 'routeId', name: 'Route', pipe: this.pipeRouteName() },
      //{ prop: 'name', name: 'Name' },
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
    this.getRoutes();
    //this.loadData();
  }

  getRoutes() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.deliveryService.getRoutesByFilter(filter)
      .subscribe(results => {
        this.routes = results.pagedData;
        this.loadData();
      },
        error => {
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving routes.\r\n"`,
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
    this.filter.filters = f + '(IsActive)==true,(PackingDate)==' + this.start.toDateString();
    //this.filter.filters = f + '(IsActive)==true';

    this.menuService.getPackingAllocationsByFilter(this.filter)
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

  pipeRouteName(): PipeTransform {
    return {
      transform: (value) => {
        let route = this.routes.find(x => x.id === value);
        if (route) {
          return route.label
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
    this.header = 'New Packing Allocations';
    this.editedAllocation = new PackingAllocation();
    this.editedAllocation.packingDate = this.start;
    this.openDialog(this.editedAllocation);
  }


  editAllocation(row: PackingAllocation) {
    this.editedAllocation = row;
    this.header = 'Edit Packing Allocation';
    this.openDialog(this.editedAllocation);
  }

  deleteAllocation(row: PackingAllocation) {
    this.alertService.showDialog('Are you sure you want to delete Packing Allocation?', DialogType.confirm, () => this.deleteAllocationHelper(row));
  }


  deleteAllocationHelper(row: PackingAllocation) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.menuService.deletePackingAllocation(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the Packing Allocation.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  get canManagePackingAllocations() {
    return this.accountService.userHasPermission(Permission.manageMOSOutletMgtPackingAllocationsPermission)
  }

}
