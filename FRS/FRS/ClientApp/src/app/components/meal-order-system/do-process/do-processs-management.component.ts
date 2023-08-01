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
import { DeliveryOrder } from '../../../models/meal-order/delivery-order.model';


@Component({
  selector: 'do-processs-management',
  templateUrl: './do-processs-management.component.html',
  styleUrls: ['./do-processs-management.component.css']
})
export class DoProcesssManagementComponent implements OnInit {
  columns: any[] = [];
  rows: any[] = [];
  map: any = {};
  allPermissions: Permission[] = [];
  editedDoProcess: BentoAsset;
  sourceDoProcess: BentoAsset;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @ViewChild('cartonInput')
  cartonInput: ElementRef;

  @ViewChild('doInput')
  doInput: ElementRef;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  header: string;

  cartonAssets: CartonAsset[];
  deliveryOrders: DeliveryOrder[];

  cartonAssetCode: any;
  doNumber: any;

  autoAdd: any = true;
  cartonTimeout: NodeJS.Timer;
  doTimeout: NodeJS.Timer;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService, private dishService: DishService, private ref: ChangeDetectorRef,
    private deliveryService: DeliveryService, public dialog: MatDialog) {

    this.getCartonAssets();
    this.getDeliveryOrders();
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
      { prop: 'doNumber', name: 'DO Number' },
      { prop: 'cartonAssetCode', name: 'Carton Asset TID' },
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



  get canManageDoProcesss() {
    return true; //this.accountService.userHasPermission(Permission.manageDoProcesssPermission)
  }

  doChanged() {
    clearTimeout(this.doTimeout)
    this.doTimeout = setTimeout(() => {
      if (this.autoAdd) this.add();
      this.cartonInput.nativeElement.focus();
    }, 500);
  }

  cartonChanged() {
    clearTimeout(this.cartonTimeout)
    this.cartonTimeout = setTimeout(() => {
      if (this.autoAdd) this.add();
      this.doInput.nativeElement.focus();
    }, 500);
  }

  add() {
    if (!this.cartonAssetCode || !this.doNumber) return;

    if (!this.map[this.cartonAssetCode]) {
      let row = {
        cartonAssetCode: this.cartonAssetCode,
        doNumber: this.doNumber
      };
      this.rows.push(row)
      this.map[this.cartonAssetCode] = row;
    } else {
      this.map[this.cartonAssetCode].cartonAssetCode = this.cartonAssetCode;
      this.map[this.cartonAssetCode].doNumber = this.doNumber;
    }

    let tempdo = this.doNumber;
    let tempcarton = this.cartonAssetCode;

    this.cartonAssetCode = '';
    this.doNumber = '';
    this.rows = [...this.rows];

    let deliveryOrder = this.deliveryOrders.find(x => x.doNumber === tempdo);

    if (!deliveryOrder) {
      this.getDeliveryOrders();
    }

    let carton = this.cartonAssets.find(x => x.code === tempcarton);

    if (!carton) {
      this.getCartonAssets();
    }
  }

  remove(row) {
    this.rows = this.rows.filter((r) => r.cartonAssetCode != row.cartonAssetCode);
    this.map[this.cartonAssetCode] = null;
  }



  submit() {
    let map: any = {};

    for (let row of this.rows) {
      row.message = '';
      row.isSuccess = false;

      let deliveryOrder = this.deliveryOrders.find(x => x.doNumber === row.doNumber);

      if (!deliveryOrder) {
        row.message = "No match DO Number";
        continue;
      }

      let carton = this.cartonAssets.find(x => x.code === row.cartonAssetCode);

      if (!carton) {
        row.message = "No match carton asset TID";
        continue;
      }

      map[deliveryOrder.id] = deliveryOrder;

      if (!deliveryOrder.deliveryDetails) deliveryOrder.deliveryDetails = [];

      let cartondetail = deliveryOrder.deliveryDetails.find(x => x.cartonAssetId === carton.id);
      if (!cartondetail) {
        cartondetail = {
          isActive: 1,
          cartonAssetId: carton.id
        };

        deliveryOrder.deliveryDetails.push(cartondetail);
      }

      for (let bento of carton.bentoAssets) {
        if (!cartondetail.deliveryBentos) cartondetail.deliveryBentos = [];

        let bentodetail = cartondetail.deliveryBentos.find(x => x.bentoAssetId === bento.id);
        if (!bentodetail) {
          bentodetail = {
            isActive: 1,
            bentoAssetId: bento.id
          }

          cartondetail.deliveryBentos.push(bentodetail);
        }

        bentodetail.dishId = bento.dishId;
      }
    }

    for (let deliveryOrder of Object.values<DeliveryOrder>(map)) {
      this.deliveryService.updateDeliveryOrder(deliveryOrder).subscribe(response => {
        for (let row of this.rows.filter(x => x.doNumber === deliveryOrder.doNumber)) {
          row.isSuccess = true
        }
      }, error => {
          for (let row of this.rows.filter(x => x.doNumber === deliveryOrder.doNumber)) {
            row.message = error
          }
      });
    }
  }

  getCartonAssets() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.deliveryService.getCartonAssetsByFilter(filter)
      .subscribe(results => {
        this.cartonAssets = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving carton assets.\r\n"`,
            MessageSeverity.error);
        })
  }

  getDeliveryOrders() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.deliveryService.getDeliveryOrdersByFilter(filter)
      .subscribe(results => {
        this.deliveryOrders = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving carton assets.\r\n"`,
            MessageSeverity.error);
        })
  }
}
