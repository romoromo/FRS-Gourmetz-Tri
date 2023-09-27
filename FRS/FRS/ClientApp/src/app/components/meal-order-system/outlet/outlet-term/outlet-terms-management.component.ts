import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { MatDialog } from '@angular/material';
import { DeliveryService } from 'src/app/services/meal-order/delivery.service';
import { Permission } from 'src/app/models/permission.model';
import { OutletTermEditorComponent } from './outlet-term-editor.component';
import { OutletTerm } from 'src/app/models/meal-order/outlet.model';
import { AppTranslationService } from 'src/app/services/app-translation.service';
import { Filter, PagedResult } from 'src/app/models/sieve-filter.model';
import { AccountService } from 'src/app/services/account.service';
import { AlertService, DialogType, MessageSeverity } from 'src/app/services/alert.service';
import { Utilities } from 'src/app/services/utilities';


@Component({
  selector: 'outlet-terms-management',
  templateUrl: './outlet-terms-management.component.html',
  styleUrls: ['./outlet-terms-management.component.css']
})
export class OutletTermsManagementComponent implements OnInit {
  columns: any[] = [];
  rows: OutletTerm[] = [];
  rowsCache: OutletTerm[] = [];
  allPermissions: Permission[] = [];
  editedOutletTerm: OutletTerm;
  sourceOutletTerm: OutletTerm;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @Input() isHideHeader: boolean;

  @Input() outletId: string;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('outletTermEditor')
  outletTermEditor: OutletTermEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private deliveryService: DeliveryService, public dialog: MatDialog) {
  }

  openDialog(outletTerm: OutletTerm): void {
    const dialogRef = this.dialog.open(OutletTermEditorComponent, {
      data: { header: this.header, outletTerm: outletTerm },
      width: '400px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData(null);
    });
  }

  initializeFilter() {
    this.filter = new Filter(1, 10);
    this.filter.sorts = 'label';
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
      { prop: 'label', name: 'Label', width: 200 },
      //{ prop: 'outletName', name: 'Outlet', width: 200 },
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
    let f = this.outletId ? '(OutletId)==' + this.outletId + ',' : '';
    this.filter.filters = f + '(IsActive)==true,(Label)@=' + this.keyword;
    
    this.deliveryService.getOutletTermsByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let outletTerms = results.pagedData;

        outletTerms.forEach((outletTerm, index, outletTerms) => {
          (<any>outletTerm).index = index + 1;
        });


        this.rowsCache = [...outletTerms];
        this.rows = outletTerms;

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

  newOutletTerm() {
    this.header = 'New Outlet Term';
    this.editedOutletTerm = new OutletTerm();
    this.editedOutletTerm.outletId = this.outletId;
    this.openDialog(this.editedOutletTerm);
  }


  editOutletTerm(row: OutletTerm) {
    this.editedOutletTerm = row;
    this.header = 'Edit Outlet Term';
    this.openDialog(this.editedOutletTerm);
  }

  deleteOutletTerm(row: OutletTerm) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\"?', DialogType.confirm, () => this.deleteOutletTermHelper(row));
  }


  deleteOutletTermHelper(row: OutletTerm) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.deliveryService.deleteOutletTerm(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the term.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  get canManageOutletTerms() {
    return true; //this.accountService.userHasPermission(Permission.manageOutletTermsPermission)
  }

}
