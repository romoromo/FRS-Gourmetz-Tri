import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { DishService } from '../../../services/meal-order/dish.service';
import { CatererAssetType } from 'src/app/models/meal-order/asset-type.model';
import { CatererAsssetTypeEditorComponent } from './caterer-asset-type-editor.component';


@Component({
  selector: 'caterer-asset-type-management',
  templateUrl: './caterer-asset-type-management.component.html',
  styleUrls: ['./caterer-asset-type-management.component.css']
})
export class CatererAsssetTypeManagementComponent implements OnInit {
  columns: any[] = [];
  rows: CatererAssetType[] = [];
  rowsCache: CatererAssetType[] = [];
  allPermissions: Permission[] = [];
  editedAssetType: CatererAssetType;
  sourceAssetType: CatererAssetType;
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

  @ViewChild('AssetTypeEditor')
  AssetTypeEditor: CatererAsssetTypeEditorComponent;
  header: string;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private dishService: DishService, public dialog: MatDialog) {
  }

  openDialog(AssetType: CatererAssetType): void {
    const dialogRef = this.dialog.open(CatererAsssetTypeEditorComponent, {
      data: { header: this.header, AssetType: AssetType },
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
    
    this.dishService.getAssetTypeByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let AssetType = results.pagedData;

        AssetType.forEach((assetType, index, assetTypes) => {
          (<any>AssetType).index = index + 1;
        });


        this.rowsCache = [...AssetType];
        this.rows = AssetType;

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

  newAssetType() {
    this.header = 'New Asset Type';
    this.editedAssetType = new CatererAssetType();
    this.editedAssetType.catererInfoId = this.catererId;
    this.openDialog(this.editedAssetType);
  }


  editAssetType(row: CatererAssetType) {
    this.editedAssetType = row;
    this.header = 'Edit Asset Type';
    this.editedAssetType.catererInfoId = this.catererId;
    this.openDialog(this.editedAssetType);
  }

  deleteAssetType(row: CatererAssetType) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.code + '\"?', DialogType.confirm, () => this.deleteAssetTypeHelper(row));
  }


  deleteAssetTypeHelper(row: CatererAssetType) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.dishService.deleteAssetType(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the AssetType.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

}
