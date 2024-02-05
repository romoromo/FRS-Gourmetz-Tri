import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, ChangeDetectorRef, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { BentoAsset } from 'src/app/models/meal-order/bento-asset.model';
import { DishService } from 'src/app/services/meal-order/dish.service';
import { StaffService } from '../../../services/meal-order/staff.service';
import { DeliveryService } from '../../../services/meal-order/delivery.service';
import { CartonAsset } from '../../../models/meal-order/carton-asset.model';
import { Dish } from '../../../models/meal-order/dish.model';


@Component({
  selector: 'dishing-processs-management',
  templateUrl: './dishing-processs-management.component.html',
  styleUrls: ['./dishing-processs-management.component.css']
})
export class DishingProcesssManagementComponent implements OnInit {
  columns: any[] = [];
  rows: any[] = [];
  map: any = {};
  allPermissions: Permission[] = [];
  editedDishingProcess: BentoAsset;
  sourceDishingProcess: BentoAsset;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  //@ViewChild('cartonInput')
  //cartonInput: ElementRef;

  @ViewChild('dishInput')
  dishInput: ElementRef;

  @ViewChild('bentoInput')
  bentoInput: ElementRef;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  header: string;

  //cartonAssets: CartonAsset[];
  bentoAssets: BentoAsset[];
  dishs: Dish[];

  //cartonAssetCode: any;
  dishCode: any;
  bentoAssetCode: any;
    bentoTimeout: NodeJS.Timer;
    autoAdd: any = true;
  //cartonTimeout: NodeJS.Timer;
  dishTimeout: NodeJS.Timer;
    dishSearchs: Dish[];
    bentoSearchs: BentoAsset[];

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService, private dishService: DishService, private ref: ChangeDetectorRef,
    private deliveryService: DeliveryService, public dialog: MatDialog) {

    //this.getCartonAssets();
    this.getBentoAssets();
    this.getDishs();
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
      //{ prop: 'cartonAssetCode', name: 'Carton Asset TID' },
      { prop: 'dishCode', name: 'Dish Code' },
      { prop: 'bentoAssetCode', name: 'Bento Asset TID' },
      { name: '', width: 150, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
    ];

    //if (!this.accountService.currentUser.institutionId || this.accountService.currentUser.institutionId == '0') {
    //  this.columns.splice(1, 0, { prop: 'institutionName', name: gT('roles.management.Institution'), width: 120 });
    //}
  }

  ngOnInit() {
    this.initializeFilter();
    this.initializePagedResult();
    this.initializeTableDefinition();
  }


  onSearchChanged(value: string) {
    this.keyword = value;
  }

  

  get canManageDishingProcesss() {
    return true; //this.accountService.userHasPermission(Permission.manageDishingProcesssPermission)
  }

  //cartonChanged() {
  //  clearTimeout(this.cartonTimeout)
  //  this.cartonTimeout = setTimeout(() => {
  //    if (this.autoAdd) this.addBento();
  //    this.dishInput.nativeElement.focus();
  //  }, 500);
  //}

  dishChanged() {
    clearTimeout(this.dishTimeout)
    this.dishTimeout = setTimeout(() => {
      this.dishSearchs = this.dishs.filter(x => x.code && this.dishCode && x.code.toLowerCase().includes(this.dishCode.toLowerCase()));

      if (this.autoAdd && this.dishSearchs.length == 1) {
        this.addBento();
        this.bentoInput.nativeElement.focus();
      }
    }, 500);
  }

  bentoChanged() {
    clearTimeout(this.bentoTimeout)
    this.bentoTimeout = setTimeout(() => {
      this.bentoSearchs = this.bentoAssets.filter(x => x.code && this.bentoAssetCode && x.code.toLowerCase().includes(this.bentoAssetCode.toLowerCase()));

      if (this.autoAdd && this.bentoSearchs.length == 1) {
        this.addBento();
        this.dishInput.nativeElement.focus();
      }
    }, 500);
  }

  addBento() {
    if (!this.dishCode || !this.bentoAssetCode) return; 

    if (!this.map[this.bentoAssetCode]) {
      let row = {
        //cartonAssetCode: this.cartonAssetCode,
        dishCode: this.dishCode,
        bentoAssetCode: this.bentoAssetCode
      };
      this.rows.push(row)
      this.map[this.bentoAssetCode] = row; 
    } else {
      //this.map[this.bentoAssetCode].cartonAssetCode = this.cartonAssetCode;
      this.map[this.bentoAssetCode].dishCode = this.dishCode;
      this.map[this.bentoAssetCode].isActive = true;
    }

    //let tempcarton = this.cartonAssetCode;
    let tempdish = this.dishCode;
    let tempbento = this.bentoAssetCode;

    //this.cartonAssetCode = '';
    this.dishCode = '';
    this.dishSearchs = [];
    this.bentoAssetCode = '';
    this.bentoSearchs = [];

    this.rows = [...this.rows];

    let bento = this.bentoAssets.find(x => x.code === tempbento);

    if (!bento) {
      this.getBentoAssets();
    }

    let dish = this.dishs.find(x => x.code === tempdish);

    if (!dish) {
      this.getDishs();
    }

    //let carton = this.cartonAssets.find(x => x.code === tempcarton);

    //if (!carton) {
    //  this.getCartonAssets();
    //}
  }

  removeBento(row) {
    this.rows = this.rows.filter((r) => r.bentoAssetCode != row.bentoAssetCode);
    this.map[this.bentoAssetCode] = null;
  }



  submit() {
    for (let row of this.rows) {
      row.message = '';
      row.isSuccess = false;

      let bento = this.bentoAssets.find(x => x.code === row.bentoAssetCode);

      if (!bento) {
        row.message = "No match bento asset TID";
        continue;
      }

      let dish = this.dishs.find(x => x.code === row.dishCode);

      if (!dish) {
        row.message = "No match dish code";
        continue;
      }

      bento.dishId = dish.id;

      //if (row.cartonAssetCode) {
        //let carton = this.cartonAssets.find(x => x.code === row.cartonAssetCode);

        //if (!carton) {
        //  row.message = "No match carton asset TID";
        //  continue;
        //}

        //bento.cartonAssetId = carton.id;
      //}

      this.deliveryService.updateBentoAsset(bento).subscribe(response => row.isSuccess = true, error => row.message = error);
    }
  }

  //getCartonAssets() {
  //  let filter = new Filter();
  //  filter.filters = '(IsActive)==true';
  //  this.deliveryService.getCartonAssetsByFilter(filter)
  //    .subscribe(results => {
  //      this.cartonAssets = results.pagedData;
  //    },
  //      error => {
  //        //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
  //        this.alertService.showStickyMessage("Get Error", `An error occured while retrieving carton assets.\r\n"`,
  //          MessageSeverity.error);
  //      })
  //}

  getBentoAssets() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.deliveryService.getBentoAssetsByFilter(filter)
      .subscribe(results => {
        this.bentoAssets = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving bento assets.\r\n"`,
            MessageSeverity.error);
        })
  }

  getDishs() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.dishService.getDishesByFilter(filter)
      .subscribe(results => {
        this.dishs = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving dishs.\r\n"`,
            MessageSeverity.error);
        })
  }

  get canManageDishing() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtDishingProcessPermission)
  }
}
