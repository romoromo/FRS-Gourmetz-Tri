import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { DeliveryOrderNew } from 'src/app/models/meal-order/delivery-order-new.model';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { Filter } from 'src/app/models/sieve-filter.model';
import { MealPeriod } from 'src/app/models/meal-order/meal-period.model';
import { StaffService } from '../../../services/meal-order/staff.service';
import { DeliveryService } from '../../../services/meal-order/delivery.service';
import { CatererInfo } from '../../../models/meal-order/caterer-info.model';
import { DishService } from '../../../services/meal-order/dish.service';
import { StoreInfo } from '../../../models/meal-order/store-info.model';
import * as moment from 'moment';


@Component({
  selector: 'delivery-order-new-editor',
  templateUrl: './delivery-order-new-editor.component.html',
  styleUrls: ['./delivery-order-new-editor.component.css']
})
export class DeliveryOrderNewEditorComponent {

  private isNewDeliveryOrder = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingDeliveryOrderCode: string;
  private deliveryOrderEdit: DeliveryOrderNew = new DeliveryOrderNew();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;
  private periods: MealPeriod[] = [];
  private catererInfos: CatererInfo[] = [];
  private storeFroms: StoreInfo[] = [];
  private storeTos: StoreInfo[] = [];
  private storeCode = "";

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;
    cartonAssets: any;
    trackingStatuss: any;
    bentoAssets: any;
  dishs: any;


  cartonTimeout: NodeJS.Timer;
    cartonAssetCode: string;

  constructor(private alertService: AlertService, private deliveryService: DeliveryService, private dishService: DishService, private accountService: AccountService,
    public dialogRef: MatDialogRef<DeliveryOrderNewEditorComponent>, private mealService: MealService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.deliveryOrder) != typeof (undefined)) {
      if (data.deliveryOrder.id) {
        this.editDeliveryOrder(data.deliveryOrder);
      } else {
        this.newDeliveryOrder();
      }
    }

    this.getCatererInfos();
    this.getCartonAssets();
    this.getTrackingStatuss();
    this.getBentoAssets();
    this.getDishs();
    this.getStoreFroms();
    this.getStoreTos();
  }


  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    this.deliveryOrderEdit.institutionId = this.accountService.currentUser.institutionId;
    var fromStoreCode = "";
    this.storeFroms.forEach(sf => {
      if (sf.id == this.deliveryOrderEdit.fromStoreId) {
        fromStoreCode = sf.code;
      }
    });
    //if (!this.deliveryOrderEdit.doNumber || this.deliveryOrderEdit.doNumber == '') {
    //  this.deliveryOrderEdit.doNumber = fromStoreCode + moment().format('YYMMDDHHmm')
    //}
    console.log(this.deliveryOrderEdit);
    let selectedPeriods = this.periods.filter(f => f.checked);
    

    if (this.isNewDeliveryOrder) {
      this.deliveryService.newDeliveryOrderNew(this.deliveryOrderEdit).subscribe(deliveryOrder => this.saveSuccessHelper(deliveryOrder), error => this.saveFailedHelper(error));
    }
    else {
      this.deliveryService.updateDeliveryOrderNew(this.deliveryOrderEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }


  private saveSuccessHelper(deliveryOrder?: DeliveryOrderNew) {
    if (deliveryOrder)
      Object.assign(this.deliveryOrderEdit, deliveryOrder);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewDeliveryOrder)
      this.alertService.showMessage("Success", `Delivery order \"${this.deliveryOrderEdit.doNumber}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to Delivery order \"${this.deliveryOrderEdit.doNumber}\" was saved successfully`, MessageSeverity.success);


    this.deliveryOrderEdit = new DeliveryOrderNew();
    this.resetForm();


    //if (!this.isNewDeliveryOrder && this.accountService.currentUser.facilities.some(r => r == this.editingDeliveryOrderCode))
    //    this.refreshLoggedInUser();

    if (this.changesSavedCallback)
      this.changesSavedCallback();

    this.dialogRef.close();
  }


  private saveFailedHelper(error: any) {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage("Save Error", "The below errors occured while saving your changes:", MessageSeverity.error);
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);

    if (this.changesFailedCallback)
      this.changesFailedCallback();
  }


  private cancel() {
    this.deliveryOrderEdit = new DeliveryOrderNew();

    this.showValidationErrors = false;
    this.resetForm();

    this.alertService.resetStickyMessage();

    if (this.changesCancelledCallback)
      this.changesCancelledCallback();

    this.dialogRef.close();
  }

  resetForm(replace = false) {

    if (!replace) {
      this.form.reset();
    }
    else {
      this.formResetToggle = false;

      setTimeout(() => {
        this.formResetToggle = true;
      });
    }
  }


  newDeliveryOrder() {
    this.isNewDeliveryOrder = true;
    this.showValidationErrors = true;

    this.editingDeliveryOrderCode = null;
    this.selectedValues = {};
    this.deliveryOrderEdit = new DeliveryOrderNew();

    return this.deliveryOrderEdit;
  }

  editDeliveryOrder(deliveryOrder: DeliveryOrderNew) {
    if (deliveryOrder) {
      this.isNewDeliveryOrder = false;
      this.showValidationErrors = true;

      this.editingDeliveryOrderCode = deliveryOrder.doNumber;
      this.selectedValues = {};
      this.deliveryOrderEdit = new DeliveryOrderNew();
      Object.assign(this.deliveryOrderEdit, deliveryOrder);

      return this.deliveryOrderEdit;
    }
    else {
      return this.newDeliveryOrder();
    }
  }

  cartonChanged() {
    clearTimeout(this.cartonTimeout)
    this.cartonTimeout = setTimeout(() => {
      this.addCarton();
      this.cartonAssetCode = '';
    }, 500);
  }

  addCarton() {

    let carton = this.cartonAssets.find(x => x.code === this.cartonAssetCode);

    if (!carton) {
      alert("No match carton asset TID");
      return;
    }

    if (!this.deliveryOrderEdit.deliveryDetails) this.deliveryOrderEdit.deliveryDetails = [];

    let cartondetail = this.deliveryOrderEdit.deliveryDetails.find(x => x.cartonAssetId === carton.id);
    if (!cartondetail) {
      cartondetail = {
        isActive: 1,
        cartonAssetId: carton.id
      };

      this.deliveryOrderEdit.deliveryDetails.push(cartondetail);
    }

    cartondetail.dishId = carton.dishId;
    cartondetail.qty = carton.qty;

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

      //bentodetail.dishId = bento.dishId;
    }
  }

  getCatererInfos() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.deliveryService.getCatererInfosByFilter(filter)
      .subscribe(results => {
        this.catererInfos = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving caterers.\r\n"`,
            MessageSeverity.error);
        })
  }

  getStoreFroms() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true,(storeType)@=Kitchen';
    this.deliveryService.getStoreInfosByFilter(filter)
      .subscribe(results => {
        this.storeFroms = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving store info.\r\n"`,
            MessageSeverity.error);
        })
  }

  getStoreTos() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true,(storeType)@=Inventory';
    this.deliveryService.getStoreInfosByFilter(filter)
      .subscribe(results => {
        this.storeTos = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving store info types.\r\n"`,
            MessageSeverity.error);
        })
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

  getTrackingStatuss() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.deliveryService.getTrackingStatussByFilter(filter)
      .subscribe(results => {
        this.trackingStatuss = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving tracking status.\r\n"`,
            MessageSeverity.error);
        })
  }

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

  addDetail(order) {
    if (!order.deliveryDetails) order.deliveryDetails = [];

    order.deliveryDetails.push({ isActive: 1 });
  }

  addBento(detail) {
    if (!detail.deliveryBentos) detail.deliveryBentos = [];

    detail.deliveryBentos.push({ isActive: 1 });
  }


  padTo2Digits(num: number) {
  return num.toString().padStart(2, '0');
  }

  formatTime(date: Date) {
    if (date != null) {
      console.log("date: ", date);
      console.log("date format: ", moment(date).format('HH:mm'));
      return moment(date).format('HH:mm');
    }

    return "";
  }



  get canManageDeliveryOrders() {
    return true; //this.accountService.userHasPermission(Permission.manageDeliveryOrdersPermission)
  }
}
