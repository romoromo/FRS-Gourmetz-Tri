import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { SortingArea } from 'src/app/models/meal-order/sorting-area.model';
import { SortingAreaEditorComponent } from './sorting-area-editor.component';
import { DeliveryService } from 'src/app/services/meal-order/delivery.service';
import { MatDialog } from '@angular/material';


@Component({
  selector: 'sorting-area-management',
  templateUrl: './sorting-area-management.component.html',
  styleUrls: ['./sorting-area-management.component.css']
})
export class SortingAreaManagementComponent implements OnInit {
  columns: any[] = [];
  rows: SortingArea[] = [];
  rowsCache: SortingArea[] = [];
  allPermissions: Permission[] = [];
  editedSortingArea: SortingArea;
  sourceSortingArea: SortingArea;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';
  @Input() isHideHeader: boolean;
  @Input() catererId: string;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('flagTemplate')
  flagTemplate: TemplateRef<any>;

  @ViewChild('SortingAreaEditor')
  SortingAreaEditor: SortingAreaEditorComponent;
  header: string;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private deliveryService: DeliveryService, public dialog: MatDialog) {
  }

  openDialog(SortingArea: SortingArea): void {
    const dialogRef = this.dialog.open(SortingAreaEditorComponent, {
      data: { header: this.header, SortingArea: SortingArea },
      width: '400px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData(null);
    });
  }

  initializeFilter() {
    this.filter = new Filter(1, 10);
    this.filter.sorts = 'code';
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
      { prop: 'description', name: 'Description' },
      { prop: 'routecolor', name: 'Route Color' },
      { prop: 'routedescription', name: 'Route Description' },
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
    this.filter.filters = '(IsActive)==true,(Code)@=' + this.keyword + ',(CatererInfoId)==' + this.catererId;
    
    this.deliveryService.getSortingAreaByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let SortingArea = results.pagedData;

        SortingArea.forEach((assetType, index, assetTypes) => {
          (<any>SortingArea).index = index + 1;
        });


        this.rowsCache = [...SortingArea];
        this.rows = SortingArea;

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

  newSortingArea() {
    this.header = 'New Sorting Area';
    this.editedSortingArea = new SortingArea();
    this.editedSortingArea.catererId = this.catererId;
    this.openDialog(this.editedSortingArea);
  }


  editSortingArea(row: SortingArea) {
    this.editedSortingArea = row;
    this.header = 'Edit Sorting Area';
    this.editedSortingArea.catererId = this.catererId;
    this.openDialog(this.editedSortingArea);
  }

  deleteSortingArea(row: SortingArea) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.code + '\"?', DialogType.confirm, () => this.deleteSortingAreaHelper(row));
  }


  deleteSortingAreaHelper(row: SortingArea) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.deliveryService.deleteSortingArea(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the SortingArea.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

}
