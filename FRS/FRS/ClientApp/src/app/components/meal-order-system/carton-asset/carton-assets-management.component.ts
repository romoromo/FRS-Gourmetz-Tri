import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { CartonAsset } from 'src/app/models/meal-order/carton-asset.model';
import { CartonAssetEditorComponent } from './carton-asset-editor.component';
import { DishService } from 'src/app/services/meal-order/dish.service';
import { StaffService } from '../../../services/meal-order/staff.service';
import { DeliveryService } from '../../../services/meal-order/delivery.service';
import { saveAs } from 'file-saver';


@Component({
  selector: 'carton-assets-management',
  templateUrl: './carton-assets-management.component.html',
  styleUrls: ['./carton-assets-management.component.css']
})
export class CartonAssetsManagementComponent implements OnInit {
  columns: any[] = [];
  rows: CartonAsset[] = [];
  rowsCache: CartonAsset[] = [];
  allPermissions: Permission[] = [];
  editedCartonAsset: CartonAsset;
  sourceCartonAsset: CartonAsset;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('flagTemplate')
  flagTemplate: TemplateRef<any>;

  @ViewChild('cartonAssetEditor')
  cartonAssetEditor: CartonAssetEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private deliveryService: DeliveryService, public dialog: MatDialog) {
  }

  openDialog(cartonAsset: CartonAsset): void {
    const dialogRef = this.dialog.open(CartonAssetEditorComponent, {
      data: { header: this.header, cartonAsset: cartonAsset },
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
      { prop: 'cartonTypeCode', name: 'Carton Type' },
      { prop: 'storeInfoCode', name: 'Store' },
      { prop: 'updatedDate', name: 'Last Update' },
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
    
    this.deliveryService.getCartonAssetsByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let cartonAssets = results.pagedData;

        cartonAssets.forEach((cartonAsset, index, cartonAssets) => {
          (<any>cartonAsset).index = index + 1;
        });


        this.rowsCache = [...cartonAssets];
        this.rows = cartonAssets;

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

  newCartonAsset() {
    this.header = 'New Carton Asset';
    this.editedCartonAsset = new CartonAsset();
    this.openDialog(this.editedCartonAsset);
  }


  editCartonAsset(row: CartonAsset) {
    this.editedCartonAsset = row;
    this.header = 'Edit Carton Asset';
    this.openDialog(this.editedCartonAsset);
  }

  deleteCartonAsset(row: CartonAsset) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.code + '\" Carton Asset?', DialogType.confirm, () => this.deleteCartonAssetHelper(row));
  }


  deleteCartonAssetHelper(row: CartonAsset) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.deliveryService.deleteCartonAsset(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the Carton Assetcaterer.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  get canManageCartonAssets() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtCartonAssetsPermission)
  }

  printCCLabel(row: CartonAsset) {
    const fileName = row.code + '_CartonContentLabel.pdf';

    this.deliveryService.printCCLabel(row.id).subscribe(
      data => {
        console.log(data);
        saveAs(data, fileName);
      },
      err => {
        alert("Carton is Empty");
        console.error(err);
      }
    );
  }

}
