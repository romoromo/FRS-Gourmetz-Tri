import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, Inject } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { BentoAsset } from 'src/app/models/meal-order/bento-asset.model';
import { BentoAssetEditorComponent } from './bento-asset-editor.component';
import { DishService } from 'src/app/services/meal-order/dish.service';
import { StaffService } from '../../../services/meal-order/staff.service';
import { DeliveryService } from '../../../services/meal-order/delivery.service';
import { BentoBoxType } from '../../../models/meal-order/bento-box-type.model';


@Component({
  selector: 'bento-assets-management',
  templateUrl: './bento-assets-management.component.html',
  styleUrls: ['./bento-assets-management.component.css']
})
export class BentoAssetsManagementComponent implements OnInit {
  columns: any[] = [];
  rows: BentoAsset[] = [];
  rowsCache: BentoAsset[] = [];
  allPermissions: Permission[] = [];
  editedBentoAsset: BentoAsset;
  sourceBentoAsset: BentoAsset;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('flagTemplate')
  flagTemplate: TemplateRef<any>;

  @ViewChild('bentoAssetEditor')
  bentoAssetEditor: BentoAssetEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private deliveryService: DeliveryService, public dialog: MatDialog) {
  }

  openDialog(bentoAsset: BentoAsset): void {
    const dialogRef = this.dialog.open(BentoAssetEditorComponent, {
      data: { header: this.header, bentoAsset: bentoAsset },
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
      { prop: 'code', name: 'Tag Id' },
      { prop: 'bentoBoxTypeCode', name: 'Bento Box Type' },
      { prop: 'storeInfoCode', name: 'Store' },
      { prop: 'dishCode', name: 'Dish Code' },
      { prop: 'dishLabel', name: 'Dish Label' },
      { prop: 'lastReturnTime', name: "Last Return" },
      { prop: 'updatedDate', name: "Last Update"},
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
    this.filter.filters = '(IsActive)==true,(Code)@=' + this.keyword + ',(InstitutionId)==' + this.accountService.currentUser.institutionId;
    
    this.deliveryService.getBentoAssetsByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let bentoAssets = results.pagedData;

        bentoAssets.forEach((bentoAsset, index, bentoAssets) => {
          (<any>bentoAsset).index = index + 1;
        });


        this.rowsCache = [...bentoAssets];
        this.rows = bentoAssets;

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

  newBentoAsset() {
    this.header = 'New Bento Asset';
    this.editedBentoAsset = new BentoAsset();
    this.openDialog(this.editedBentoAsset);
  }


  editBentoAsset(row: BentoAsset) {
    this.editedBentoAsset = row;
    this.header = 'Edit Bento Asset';
    this.openDialog(this.editedBentoAsset);
  }

  deleteBentoAsset(row: BentoAsset) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.code + '\" Bento Asset?', DialogType.confirm, () => this.deleteBentoAssetHelper(row));
  }


  deleteBentoAssetHelper(row: BentoAsset) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.deliveryService.deleteBentoAsset(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the Bento Assetcaterer.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  get canManageBentoAssets() {
    return true; //this.accountService.userHasPermission(Permission.manageBentoAssetsPermission)
  }

  newMultipleBentoAsset(): void {
    const dialogRef = this.dialog.open(NewMultipleBentoAsset, {
      width: '40vw',
    });

    dialogRef.afterClosed().subscribe(result => {
      setTimeout(() => this.loadData(null), 2000);
    });
  }

}

@Component({
  selector: 'new-multiple-bento-asset',
  templateUrl: 'new-multiple-bento-asset.html',
})
export class NewMultipleBentoAsset {

  bentoAssetCode: any;
  bentoBoxType: any;
  bentoBoxTypes : any;
    bentoTimeout: NodeJS.Timer;
  map: any = {};
  rows: any = [];


  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;

  constructor(private alertService: AlertService,
    public dialogRef: MatDialogRef<any>,
    @Inject(MAT_DIALOG_DATA) public data: any, public deliveryService: DeliveryService) {

    this.getBentoBoxTypes();
  }

  bentoChanged() {
    clearTimeout(this.bentoTimeout)
    this.bentoTimeout = setTimeout(() => {
      this.addBento();
    }, 500);
  }

  addBento() {
    if (!this.bentoAssetCode) return;

    if (!this.map[this.bentoAssetCode]) {
      let row = {
        code: this.bentoAssetCode,
        isActive: true
      };
      this.rows.push(row)
      this.map[this.bentoAssetCode] = row;
    } else {
      this.map[this.bentoAssetCode].code = this.bentoAssetCode;
      this.map[this.bentoAssetCode].isActive = true;
    }

    this.bentoAssetCode = '';
    this.rows = [...this.rows];
  }

  save() {
    if (!this.bentoBoxType && this.rows.length <= 0) return;

    let type = this.bentoBoxTypes.find(d => d.id == this.bentoBoxType);

    if (!type) return;

    type.bentoAssets = this.rows;

    this.deliveryService.updateBentoBoxType(type).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error)); 
  }

  private saveSuccessHelper() {
    this.alertService.stopLoadingMessage();

      this.alertService.showMessage("Success", `Changes to Bento asset was saved successfully`, MessageSeverity.success);


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

  getBentoBoxTypes() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.deliveryService.getBentoBoxTypesByFilter(filter)
      .subscribe(results => {
        this.bentoBoxTypes = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving bento box types.\r\n"`,
            MessageSeverity.error);
        })
  }
}
