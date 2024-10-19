import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { OrderPortalContent } from 'src/app/models/meal-order/order-portal-content.model';
import { OrderPortalContentEditorComponent } from './order-portal-content.component';
import { OrderPortalService } from 'src/app/services/order-portal.service';
import { DateOnlyPipe, DateTimeOnlyPipe } from 'src/app/pipes/datetime.pipe';


@Component({
  selector: 'order-portal-content-management',
  templateUrl: './order-portal-contents-management.component.html',
  styleUrls: ['./order-portal-contents-management.component.css']
})
export class OrderPortalContentsManagementComponent implements OnInit {
  columns: any[] = [];
  rows: OrderPortalContent[] = [];
  rowsCache: OrderPortalContent[] = [];
  allPermissions: Permission[] = [];
  editedOrderPortalContent: OrderPortalContent;
  sourceOrderPortalContent: OrderPortalContent;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('flagTemplate')
  flagTemplate: TemplateRef<any>;

  @ViewChild('outletEditor')
  outletEditor: OrderPortalContentEditorComponent;

  @ViewChild('portalContentsTable') table: any;

  header: string;
  @Input() isHideHeader: boolean;
  @Input() outletId: string;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private portalService: OrderPortalService, public dialog: MatDialog) {
  }

  openDialog(portalContent: OrderPortalContent): void {
    const dialogRef = this.dialog.open(OrderPortalContentEditorComponent, {
      data: { header: this.header, portalContent: portalContent, outletId: this.outletId },
      width: '800px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData(null);
    });
  }

  initializeFilter() {
    this.filter = new Filter(1, 10);
    this.filter.sorts = 'id';
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
      { prop: 'description', name: 'Description' },
      { prop: 'announcement', name: 'Announcement' },
      { prop: 'effectiveStartDate', name: 'Start', pipe: new DateOnlyPipe('en-SG') },
      { prop: 'effectiveEndDate', name: 'End', pipe: new DateOnlyPipe('en-SG') },
      { name: '', width: 150, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
    ];
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
    this.filter.filters = '(IsActive)==true,(OutletId)==' + (this.outletId ? this.outletId : '');
    
    this.portalService.getOrderPortalContentsByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let portalContents = results.pagedData;

        portalContents.forEach((portalContent, index, portalContents) => {
          (<any>portalContent).index = index + 1;
        });


        this.rowsCache = [...portalContents];
        this.rows = portalContents;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve records from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  clearFilterAndPagedResult() {
    this.initializeFilter();
    this.initializePagedResult();
    this.table.offset = 0;
  }

  onSearchChanged(value: string) {
    this.keyword = value;
    this.clearFilterAndPagedResult();
    this.loadData(null);
  }

  newOrderPortalContent() {
    this.header = 'New Content';
    this.editedOrderPortalContent = new OrderPortalContent();
    this.editedOrderPortalContent.outletId = this.outletId;
    this.openDialog(this.editedOrderPortalContent);
  }


  editOrderPortalContent(row: OrderPortalContent) {
    this.editedOrderPortalContent = row;
    this.header = 'Edit Portal Content';
    this.openDialog(this.editedOrderPortalContent);
  }

  deleteOrderPortalContent(row: OrderPortalContent) {
    this.alertService.showDialog('Are you sure you want to delete this content?', DialogType.confirm, () => this.deleteOrderPortalContentHelper(row));
  }


  deleteOrderPortalContentHelper(row: OrderPortalContent) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.portalService.deleteOrderPortalContent(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the content.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  get canManageOrderPortalContents() {
    return this.accountService.userHasPermission(Permission.manageMOSOutletMgtPortalContentsPermission)
  }

}
