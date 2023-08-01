import { Component, OnInit, Output, EventEmitter, Input, Inject } from '@angular/core';
import { HttpEventType, HttpClient, HttpEvent } from '@angular/common/http';
import { DateAdapter, MatDatepickerInputEvent, MatDialog, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Utilities } from 'src/app/services/utilities';
import { Filter } from 'src/app/models/sieve-filter.model';
import { CatererInfo, CatererOutlet } from 'src/app/models/meal-order/caterer-info.model';
import { DeliveryService } from 'src/app/services/meal-order/delivery.service';
import { Outlet } from 'src/app/models/meal-order/outlet.model';
import { StoreInfo } from 'src/app/models/meal-order/store-info.model';
import { TokenOrder, TokenLabel, TokenDishLabel } from 'src/app/models/meal-order/token-order.model';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';
import * as moment from 'moment';
import { saveAs } from 'file-saver';
import { MenuService } from '../../../../services/meal-order/menu.service';
import { Dish } from '../../../../models/meal-order/dish.model';
import { MealType } from '../../../../models/meal-order/meal-type.model';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { DishService } from 'src/app/services/meal-order/dish.service';


@Component({
  selector: 'print-label',
  templateUrl: './print-label.component.html',
  styleUrls: ['./print-label.component.css']
})
export class PrintLabelComponent implements OnInit {
  stores: StoreInfo[];
  storesCache: StoreInfo[];
  selectedStores: StoreInfo[] = [];
  editOutlet: Outlet;
  selectedCaterers: CatererOutlet[];
  orderDate = moment();
  orders: TokenOrder[];
  token_count: TokenLabel[] = [];
  dishes: Dish[];
  tokens: MealType[];

  //dish_count: TokenDishLabel[] = [];

  constructor(private http: HttpClient, private alertService: AlertService, private deliveryService: DeliveryService, public dialog: MatDialog,
    public dialogRef: MatDialogRef<PrintLabelComponent>, private menuService: MenuService, private mealService: MealService, private dishService: DishService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.outlet) != typeof (undefined)) {
      this.editOutlet = data.outlet;
    }
  }

  ngOnInit() {
    this.getMenuDishes();
    this.getMealTypes();
    this.getTokenOrder();
  }

  loadData() {
    let filterCaterer = "";
    this.selectedCaterers.forEach((caterer, i) => {
      if (caterer.status == "APPROVED") {
        if (filterCaterer != "") {
          filterCaterer += '|'
        }
        filterCaterer += caterer.catererInfoId
      }
    });
    console.log("caterer filter = ", filterCaterer)

    //this.getStore(filterCaterer);
  }

  onChangeDate(type: string, event: MatDatepickerInputEvent<Date>) {
    console.log("event value: ", moment(event.value))
      this.orderDate = moment(event.value);
  }

  private cancel() {
    this.dialogRef.close({ isCancel: true });
  }

  onSearchChanged(value: string) {
    this.stores = this.storesCache.filter(r => Utilities.searchArray(value, false, r.name));
  }

  getOrderData() {
    console.log("clicked")
    this.getTokenOrder();
  }

  getMealTypes() {
    let filter = new Filter();
    filter.sorts = 'name';
    //let f = this.catererId ? '(CatererId)==' + this.catererId + ',' : '';
    filter.filters ='(IsActive)==true';
    this.mealService.getMealTypesByFilter(filter)
      .subscribe(results => {
        this.tokens = results.pagedData;
      },
        error => {
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving meal type.\r\n"`,
            MessageSeverity.error);
        })
  }

  getMenuDishes() {
    let filter = new Filter();
    //let f = this.catererId ? '(CatererId)==' + this.catererId + ',' : '';
    filter.filters ='(IsActive)==true';
    this.dishService.getDishesByFilter(filter)
      .subscribe(results => {
        this.dishes = results.pagedData;
      },
        error => {
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving dishes.\r\n"`,
            MessageSeverity.error);
        })
  }

  getTokenName(id) {
    let token = this.tokens.find(x => x.id === id);
    if (token) {
      return token.name
    } else {
      return ''
    }
  }

  getDishName(id) {
    let dish = this.dishes.find(x => x.id === id);
    if (dish) {
      return dish.label
    } else {
      return ''
    }
  }

  getDishCode(id) {
    let dish = this.dishes.find(x => x.id === id);
    if (dish) {
      return dish.code
    } else {
      return ''
    }
  }

  

  onChangeQty(event, detail?: TokenDishLabel, token?: TokenLabel) {
    if (detail.a_qty > 0) {
      detail.t_qty = detail.o_qty + detail.a_qty;
    } else {
      detail.t_qty = detail.o_qty + detail.p_qty;
    }

    token.qty_tdishes = 0;

    token.dishes.forEach(d => {
      token.qty_tdishes += d.t_qty;

    })
  }

  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }



  getTokenOrder() {
    var insideFilter = new Filter();

    this.token_count = [];

    var strDate = this.orderDate.format().split('T');
    insideFilter.filters = '(IsActive)==true,(outletId)==' + this.editOutlet.id + ',(DeliveryDate)==' + strDate[0];
    console.log("filters: ", insideFilter.filters)

    this.menuService.getTokenOrdersByFilter(insideFilter).subscribe(results => {
      this.orders = results.pagedData;

      console.log("order : ",this.orders)

      //Order Loop
      this.orders.forEach(o => {

        //Token Loop
        o.tokens.forEach(t => {

          var tokenIndex = this.token_count.findIndex(x => x.token_id === t.tokenId)
          if (tokenIndex < 0) {
            var token = new TokenLabel;
            token.token_id = t.tokenId;
            token.token_name = this.getTokenName(t.tokenId);
            console.log("combined dish: ", t.selectedCombinedDishes.length)
            if (t.selectedCombinedDishes.length > 0) {
              console.log("menuqty: ", t.selectedCombinedDishes[0].menuQty)
              token.qty = t.qty * t.selectedCombinedDishes[0].menuQty;
            } else {
              token.qty = t.qty;
            }
            token.qty_menus = t.qty;
            token.qty_dishes = 0;
            token.qty_pdishes = 0;
            token.qty_tdishes = 0;
            token.deliveryDate = strDate[0];
            token.dishes = [];
            this.token_count.push(token);
            tokenIndex = this.token_count.findIndex(x => x.token_id === t.tokenId)
          } else {
            if (t.selectedCombinedDishes.length > 0) {
              this.token_count[tokenIndex].qty += (t.qty * t.selectedCombinedDishes[0].menuQty);
            } else {
              this.token_count[tokenIndex].qty += t.qty;
            }
            this.token_count[tokenIndex].qty_menus += t.qty;
          }

          // Selected dishes loop
          t.selectedDishes.forEach(d => {
            var dishIndex = this.token_count[tokenIndex].dishes.findIndex(y => y.dish_id === d.dishId)
            if (dishIndex < 0) {
              var dish = new TokenDishLabel;
              dish.token_id = t.tokenId;
              dish.token_name = this.getTokenName(t.tokenId);
              dish.dish_id = d.dishId;
              dish.dish_name = this.getDishName(d.dishId);
              dish.dish_code = this.getDishCode(d.dishId);
              dish.o_qty = d.qty;
              this.token_count[tokenIndex].dishes.push(dish);
            } else {
              this.token_count[tokenIndex].dishes[dishIndex].o_qty += d.qty;
            }
            this.token_count[tokenIndex].qty_dishes += d.qty;

          })

        })

      })

      //count proposed allocation
      this.token_count.forEach(t => {
        t.qty_pdishes = 0;
        t.qty_tdishes = 0;
        t.dishes.forEach(d => {
          console.log("hasil awal: ", d.o_qty / t.qty_dishes * (t.qty - t.qty_dishes))
          var p_allocation = Math.ceil(d.o_qty / t.qty_dishes * (t.qty - t.qty_dishes))
          console.log("ceiling: ", p_allocation)
          d.p_qty = p_allocation;
          d.t_qty = d.o_qty + d.p_qty;
          t.qty_pdishes += p_allocation;
          t.qty_tdishes += d.t_qty;

        })

      })


      console.log("token count: ", this.token_count);

    }, error => { });
  }

  save() {
    console.log("download Label");
    this.downloadLabel();
  }

  downloadLabel() {
    const fileName = moment().format('DDMMYYYY_hhmmss') + '_OrderLabel.pdf';

    console.log("token sent: ", this.token_count)

    this.menuService.downloadOrderLabel(this.token_count).subscribe(
      data => {
        console.log(data);
        saveAs(data, fileName);
      },
      err => {
        alert("Problem while downloading the file.");
        console.error(err);
      }
    );
  }


}
