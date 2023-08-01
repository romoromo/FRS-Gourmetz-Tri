import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { DateAdapter, MatDatepickerInputEvent, MatDialog, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { MealTypeMenuDish, Menu, MenuDish } from 'src/app/models/meal-order/menu.model';
import { TokenOrder, TokenOrdered, TokenOrderDish, TokenAltDish, TokenOrderCombinedDish } from 'src/app/models/meal-order/token-order.model';
import { MenuService } from 'src/app/services/meal-order/menu.service';
import { Filter } from 'src/app/models/sieve-filter.model';
import { Dish } from 'src/app/models/meal-order/dish.model';
import { DishService } from 'src/app/services/meal-order/dish.service';
import { Student } from 'src/app/models/meal-order/student.model';
import { StudentService } from 'src/app/services/meal-order/student.service';
import * as moment from 'moment';
import { MAT_MOMENT_DATE_FORMATS } from '@angular/material-moment-adapter';
import { MomentUtcDateAdapter } from 'src/app/helpers/moment-utc-adapter';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { DeliveryService } from 'src/app/services/meal-order/delivery.service';
import { MealType, MealTypeDish } from 'src/app/models/meal-order/meal-type.model';
import { MealPeriod } from 'src/app/models/meal-order/meal-period.model';
import { MealSessionDetail } from 'src/app/models/meal-order/meal-session.model';
import { DishSelectorComponent } from '../dishes/dish-selector/dish-selector.component';

@Component({
  selector: 'token-order-editor',
  templateUrl: './token-order-editor.component.html',
  styleUrls: ['./token-order-editor.component.css'],
  providers: [
    { provide: MAT_DATE_LOCALE, useValue: 'en-SG' },
    { provide: MAT_DATE_FORMATS, useValue: MAT_MOMENT_DATE_FORMATS },
    { provide: DateAdapter, useClass: MomentUtcDateAdapter },
  ]
})


 

export class TokenOrderEditorComponent {

  private isNewOrder = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingOrderCode: string;
  private orderEdit: TokenOrder = new TokenOrder();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;
  transactionTime = new Date();
  deliveryDate = new Date();
  private students: Student[] = [];
  private periods: MealPeriod[] = [];
  private sessions: MealSessionDetail[] = [];
  private mealtypes: MealType[] = [];

  selectedStudent: Student;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;
  public catererId: string;
  public dishes: Dish[] = [];
  public altDishes: TokenAltDish[] = [];
  public storeId: string;

  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private menuService: MenuService, private accountService: AccountService,
    public dialogRef: MatDialogRef<TokenOrderEditorComponent>, private dishService: DishService, private studentService: StudentService, private mealService: MealService,
    @Inject(MAT_DIALOG_DATA) public data: any, public dialog: MatDialog, private deliveryService: DeliveryService,
    private dateAdapter: DateAdapter<Date>) {
    this.dateAdapter.setLocale('en-SG');

    if (typeof (data.student) != typeof (undefined)) {
      if (data.student) {
        this.selectedStudent = data.student;
      }
    }


    this.getStore();
    this.getMealTypes(data);
    this.getStudents();
    this.getPeriods();
    this.getMenuDishes();
    //this.getSessions();
  }

  getStudents() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.studentService.getStudentsByFilter(filter)
      .subscribe(results => {
        this.students = results.pagedData;
        console.log("students: ", this.students)
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving students.\r\n"`,
            MessageSeverity.error);
        })
  }

  getPeriods() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.mealService.getMealPeriodsByFilter(filter)
      .subscribe(results => {
        this.periods = results.pagedData;
        console.log("periods: ", this.periods)
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving meal periods.\r\n"`,
            MessageSeverity.error);
        })
  }

  getSessions() {
    if (this.deliveryDate) {
      var date = new Date(this.deliveryDate.getTime() - (this.deliveryDate.getTimezoneOffset() * 60000)).toJSON().split('T')
      console.log("delivery date: ", date[0])
      this.menuService.getStudentSessionsByFilter(this.selectedStudent.id, date[0])
        .subscribe(results => {
          this.sessions = results;
          console.log("sessions: ", this.sessions)
        },
          error => {
            //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            this.alertService.showStickyMessage("Get Error", `An error occured while retrieving meal sessions.\r\n"`,
              MessageSeverity.error);
          })
    }
  }

  getStore() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true,(outletId)==' + this.selectedStudent.outletId;
    this.deliveryService.getStoreInfosByFilter(filter)
      .subscribe(results => {
        if (results.pagedData.length > 0) {
          this.storeId = results.pagedData[0].id;
          this.catererId = results.pagedData[0].catererInfoId;
        } else {
          this.storeId = "0";
          this.catererId = "0";
        }
        console.log("store paged data: ", results.pagedData)
        console.log("store: ", this.storeId)
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving meal periods.\r\n"`,
            MessageSeverity.error);
        })
  }

  onChangeDate(type: string, event: MatDatepickerInputEvent<Date>) {
    this.deliveryDate = new Date(event.value);
    this.orderEdit.deliveryDate = new Date(event.value);
    this.getSessions();
  }

  selectToken(event, detail?: TokenOrdered) {
    console.log("event: ", event)
    console.log("order: ", detail)
    let token = this.mealtypes.find(x => x.id === event.value);
    if (token) {
      detail.price = token.price;
    }

    this.getCombinedDishes(detail);
    this.onChangeQty(event, detail);
  }

  selectCDish(event, detail?: TokenOrdered, selCDish?: TokenOrderCombinedDish) {
    console.log("event: ", event)
    console.log("order: ", detail)
    detail.selectedDishes = [];

    let cdish = detail.listCombinedDishes.find(x => x.setname === event.value);

    console.log("cdish: ", cdish)
    selCDish.menuQty = cdish.menus.length;
    cdish.menus.forEach(dish => {
      if (!detail.selectedDishes) detail.selectedDishes = [];

      let selDish = new TokenOrderDish();
      selDish.tokenOrderedId = detail.id;
      selDish.dishId = dish.dishId;

      detail.selectedDishes.push(selDish);
    })
  }

  onChangeCDQty(event, detail?: TokenOrdered, selCDish?: TokenOrderCombinedDish) {
    console.log("event: ", event)

    if (detail.selectedDishes) {

      detail.selectedDishes.forEach(dish => {
        dish.qty = selCDish.qty;
      })
    }

  }

  onChangeQty(event, detail?: TokenOrdered) {
    if (detail.qty && detail.price) {
      detail.subtotal = detail.qty * detail.price;
    } else {
      detail.subtotal = 0;
    }

    var sumAmount = 0;

    this.orderEdit.tokens.forEach(t => {
      sumAmount += t.subtotal;
    })

    this.orderEdit.totalAmount = sumAmount;
  }

  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");

    console.log("storeID = ", this.storeId)

    if (this.storeId != null && this.storeId != "0") {
      this.orderEdit.storeId = this.storeId;
    } else {
      this.noStoreHelper();
      return;
    }


    console.log("order: ", this.orderEdit)
    this.orderEdit.transactionTime = new Date();
    this.orderEdit.status = "pending";
    
    if (this.isNewOrder) {
      this.menuService.newTokenOrder(this.orderEdit).subscribe(order => this.saveSuccessHelper(order), error => this.saveFailedHelper(error));
    }
    else {
      this.menuService.updateTokenOrder(this.orderEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }


  private saveSuccessHelper(order?: TokenOrder) {
    if (order)
      Object.assign(this.orderEdit, order);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewOrder)
      this.alertService.showMessage("Success", `Order No \"${this.orderEdit.id}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to Order No \"${this.orderEdit.id}\" was saved successfully`, MessageSeverity.success);


    this.orderEdit = new TokenOrder();
    this.resetForm();


    //if (!this.isNewMenu && this.accountService.currentUser.facilities.some(r => r == this.editingMenuCode))
    //    this.refreshLoggedInUser();

    if (this.changesSavedCallback)
      this.changesSavedCallback();

    this.dialogRef.close();
  }

  private noStoreHelper() {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage("Save Error", "Current Outlet doesnt have any store linked to it. Please set up store for the Outlet", MessageSeverity.error);

    if (this.changesFailedCallback)
      this.changesFailedCallback();
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
    this.orderEdit = new TokenOrder();

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


  newOrder() {
    this.isNewOrder = true;
    this.showValidationErrors = true;

    this.editingOrderCode = null;
    this.selectedValues = {};
    this.orderEdit = new TokenOrder();
    this.orderEdit.profileId = this.selectedStudent.id;
    this.transactionTime = new Date();
    this.deliveryDate = new Date();
    this.deliveryDate.setUTCHours(0, 0, 0, 0);
    this.orderEdit.transactionTime = this.transactionTime;
    this.orderEdit.deliveryDate = this.deliveryDate;
    return this.orderEdit;
  }

  editOrder(order: TokenOrder) {
    if (order) {
      this.isNewOrder = false;
      this.showValidationErrors = true;

      console.log("order is: ", order)
      //this.editingMenuCode = menu.label;
      order.tokens.forEach(t => {
        let token = this.mealtypes.find(x => x.id === t.tokenId);
        if (token) {
          t.price = token.price;
        }

        if (t.price && t.qty) {
          t.subtotal = t.price * t.qty;
        }
      })
      this.selectedValues = {};
      this.orderEdit = new TokenOrder();
      Object.assign(this.orderEdit, order);
      this.transactionTime = this.orderEdit.transactionTime;
      this.deliveryDate = new Date(this.orderEdit.deliveryDate);

      this.getSessions();

      this.orderEdit.tokens.forEach(t => {
        this.getCombinedDishes(t);
      })

      return this.orderEdit;
    }
    else {
      return this.newOrder();
    }
  }

  getMealTypes(data: any) {
    let filter = new Filter();
    filter.sorts = 'name';
    let f = this.catererId ? '(CatererId)==' + this.catererId + ',' : '';
    filter.filters = f + '(IsActive)==true';
    this.mealService.getMealTypesByFilter(filter)
      .subscribe(results => {
        let mealTypes = results.pagedData;
        this.mealtypes = results.pagedData;
        if (typeof (data.tokenOrder) != typeof (undefined)) {
          if (data.tokenOrder.id) {
            this.editOrder(data.tokenOrder);
          } else {
            this.newOrder();
          }
        }
        //if (!this.menuEdit.mealTypeMenuDishes) {
        //  this.menuEdit.mealTypeMenuDishes = [];
        //}

        //let dishFilter = new Filter();
        //dishFilter.sorts = 'label';
        //dishFilter.filters = f + '(IsActive)==true';
        //this.dishService.getDishesByFilter(dishFilter)
        //  .subscribe(res => {
        //    let dishes = res.pagedData;
            
        //    mealTypes.forEach((mp: MealType, index, m) => {
        //      let mealTypeDish = new MealTypeMenuDish();
        //      mealTypeDish.menuDishes = [];
        //      mealTypeDish.mealTypeId = mp.id;
        //      mealTypeDish.mealTypeName = mp.name;
        //      dishes.forEach((d: Dish, index, dish) => {
        //        if (!this.menuEdit.menuDishes) this.menuEdit.menuDishes = [];
        //        let existingDishes = this.menuEdit.menuDishes.filter(e => e.mealTypeId == mp.id && e.dishId == d.id);
        //        if (existingDishes && existingDishes.length > 0) {
        //          let existingDish = existingDishes[0];
        //          existingDish.checked = true;
        //          mealTypeDish.menuDishes.push(existingDishes[0]);
        //        } else {
        //          let newMenuDish = new MenuDish();
        //          newMenuDish.dishId = d.id;
        //          newMenuDish.menuId = this.menuEdit.id;
        //          newMenuDish.mealTypeId = mp.id;
        //          newMenuDish.dishName = d.label;
        //          mealTypeDish.menuDishes.push(newMenuDish);
        //        }
        //      });

        //      this.menuEdit.mealTypeMenuDishes.push(mealTypeDish);
        //    },
        //      error => {
        //        this.alertService.showStickyMessage("Get Error", `An error occured while retrieving meal types.\r\n"`,
        //          MessageSeverity.error);
        //      });

        //  });
      },
        error => {
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving meal type.\r\n"`,
            MessageSeverity.error);
        })
  }

  getMenuDishes() {
    let filter = new Filter();
    let f = this.catererId ? '(CatererId)==' + this.catererId + ',' : '';
    filter.filters = f + '(IsActive)==true';
    this.dishService.getDishesByFilter(filter)
      .subscribe(results => {
        this.dishes = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving dishes.\r\n"`,
            MessageSeverity.error);
        })
  }

  getCombinedDishes(detail: TokenOrdered) {
    if (this.deliveryDate) {
      detail.listCombinedDishes = [];
      var deliveryDate = (new Date(this.deliveryDate.getTime() - (this.deliveryDate.getTimezoneOffset() * 60000)).toJSON().split('T'))[0]
      console.log("get combined dish: ", this.selectedStudent.outletId, this.catererId, detail.tokenId, deliveryDate, this.orderEdit.mealSessionDetailId)
      if (this.selectedStudent.outletId && this.orderEdit.mealSessionDetailId && detail.tokenId && deliveryDate && this.catererId) {

        this.dishService.getCombinedDish(this.selectedStudent.outletId, this.catererId, detail.tokenId, deliveryDate, this.orderEdit.mealSessionDetailId)
          .subscribe(results => {
            console.log("result combined dish list: ", results);
            detail.listCombinedDishes = results
          }
            , error => {
              //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
              this.alertService.showStickyMessage("Get Error", `An error occured while retrieving combined dishes.\r\n"`,
                MessageSeverity.error);

            })
      }
    }
  }

  //addDish(mealTypeId, mealTypeName: string) {
  //  let mealType = this.menuEdit.mealTypeMenuDishes.filter(e => e.mealTypeId == mealTypeId);
  //  let selectedDishes: MenuDish[] = [];
  //  if (mealType && mealType.length > 0) {
  //    selectedDishes = mealType[0].menuDishes.filter(e => e.checked);
  //  }
    
  //  const dialogRef = this.dialog.open(DishSelectorComponent, {
  //    data: { header: "Dishes", dishes: selectedDishes, catererId: this.catererId },
  //    width: '800px'
  //  });

  //  dialogRef.afterClosed().subscribe(result => {
  //    if (!result.isCancel) {
  //      let menuDishes: MenuDish[] = [];
  //      result.selectedDishes.forEach((d, index, ps) => {
  //        if (d.checked) {
  //          let dtDish = new MenuDish();
  //          dtDish.menuId = this.menuEdit.id;
  //          dtDish.dishId = d.id;
  //          dtDish.mealTypeId = mealTypeId;
  //          dtDish.checked = true;
  //          dtDish.dishName = d.label;
  //          menuDishes.push(dtDish);
  //        }
  //      });

  //      let mealTypeDishes = this.menuEdit.mealTypeMenuDishes.filter((cg) => cg.mealTypeId == mealTypeId);
  //      if (mealTypeDishes && mealTypeDishes.length > 0) {
  //        mealTypeDishes[0].menuDishes = menuDishes;
  //      }
  //    }
  //  });
  //}

  addDetail(order: TokenOrder) {
    if (!order.tokens) order.tokens = [];

    let tokenOrdered = new TokenOrdered();
    tokenOrdered.orderId = order.id;
    tokenOrdered.tokenAltDishes = [];

    let maxLength = this.dishes.length > 5 ? 5 : this.dishes.length

    for (let i = 0; i < maxLength; i++) {
      let tad = new TokenAltDish();
      tad.dishId = this.dishes[i].id;
      tad.tokenOrderedId = tokenOrdered.id;
      tokenOrdered.tokenAltDishes.push(tad);
    }

    order.tokens.push(tokenOrdered);
  }

  addDish(detail: TokenOrdered) {
    if (!detail.selectedDishes) detail.selectedDishes = [];

    let selDish = new TokenOrderDish();
    selDish.tokenOrderedId = detail.id;

    detail.selectedDishes.push(selDish);
  }

  addCombinedDish(detail: TokenOrdered) {
    if (!detail.selectedCombinedDishes) detail.selectedCombinedDishes = [];

    let selCDish = new TokenOrderCombinedDish();
    selCDish.tokenOrderedId = detail.id;

    detail.selectedCombinedDishes.push(selCDish);
  }

  removeToken(row: TokenOrdered) {

    this.orderEdit.tokens = this.orderEdit.tokens.filter(item => item !== row);
  }

  removeDish(token:TokenOrdered,row: TokenOrderDish) {
    token.selectedDishes = token.selectedDishes.filter(item => item !== row);
  }

  get canManageOrders() {
    return true; //this.accountService.userHasPermission(Permission.manageMenusPermission)
  }
}
