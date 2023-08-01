import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { CartonType } from 'src/app/models/meal-order/carton-type.model';
import { CartonTypeEditorComponent } from './carton-type-editor.component';
import { DishService } from 'src/app/services/meal-order/dish.service';
import { StaffService } from '../../../services/meal-order/staff.service';
import { DeliveryService } from '../../../services/meal-order/delivery.service';


@Component({
  selector: 'carton-types-management',
  templateUrl: './carton-types-management.component.html',
  styleUrls: ['./carton-types-management.component.css']
})
export class CartonTypesManagementComponent implements OnInit {
  columns: any[] = [];
  rows: CartonType[] = [];
  rowsCache: CartonType[] = [];
  allPermissions: Permission[] = [];
  editedCartonType: CartonType;
  sourceCartonType: CartonType;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('flagTemplate')
  flagTemplate: TemplateRef<any>;

  @ViewChild('cartonTypeEditor')
  cartonTypeEditor: CartonTypeEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private deliveryService: DeliveryService, public dialog: MatDialog) {
  }

  openDialog(cartonType: CartonType): void {
    const dialogRef = this.dialog.open(CartonTypeEditorComponent, {
      data: { header: this.header, cartonType: cartonType },
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
      { prop: 'catererInfoName', name: 'Caterer' },
      { prop: 'code', name: 'Code' },
      { prop: 'details', name: 'Details' },
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
    
    this.deliveryService.getCartonTypesByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let cartonTypes = results.pagedData;

        cartonTypes.forEach((cartonType, index, cartonTypes) => {
          (<any>cartonType).index = index + 1;
        });


        this.rowsCache = [...cartonTypes];
        this.rows = cartonTypes;

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

  newCartonType() {
    this.header = 'New Carton type';
    this.editedCartonType = new CartonType();
    this.openDialog(this.editedCartonType);
  }


  editCartonType(row: CartonType) {
    this.editedCartonType = row;
    this.header = 'Edit Carton type';
    this.openDialog(this.editedCartonType);
  }

  deleteCartonType(row: CartonType) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.code + '\" Carton type?', DialogType.confirm, () => this.deleteCartonTypeHelper(row));
  }


  deleteCartonTypeHelper(row: CartonType) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.deliveryService.deleteCartonType(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the Carton type.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  get canManageCartonTypes() {
    return true; //this.accountService.userHasPermission(Permission.manageCartonTypesPermission)
  }

}
