import {
  Component,
  OnInit,
  Output,
  EventEmitter,
  Input,
  Inject,
} from "@angular/core";
import { HttpEventType, HttpClient, HttpEvent } from "@angular/common/http";
import {
  DateAdapter,
  MatDatepickerInputEvent,
  MatDialog,
  MAT_DATE_FORMATS,
  MAT_DATE_LOCALE,
} from "@angular/material";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material";
import { Utilities } from "src/app/services/utilities";
import { Filter } from "src/app/models/sieve-filter.model";
import {
  CatererInfo,
  CatererOutlet,
} from "src/app/models/meal-order/caterer-info.model";
import { DeliveryService } from "src/app/services/meal-order/delivery.service";
import { Outlet } from "src/app/models/meal-order/outlet.model";
import { StoreInfo } from "src/app/models/meal-order/store-info.model";
import {
  TokenOrder,
  TokenLabel,
  TokenDishLabel,
  MealAllocation,
} from "src/app/models/meal-order/token-order.model";
import { AlertService, MessageSeverity } from "src/app/services/alert.service";
import * as moment from "moment";
import { saveAs } from "file-saver";
import { MenuService } from "../../../services/meal-order/menu.service";
import { Dish } from "../../../models/meal-order/dish.model";
import { MealType } from "../../../models/meal-order/meal-type.model"; 
import { Route } from "../../../models/meal-order/route.model";

import { MealService } from "src/app/services/meal-order/meal.service";
import { DishService } from "src/app/services/meal-order/dish.service";
import {
  MealSessionDetail,
  MealSession,
} from "src/app/models/meal-order/meal-session.model";

import { DishSelectorComponent } from "../dishes/dish-selector/dish-selector.component";
import { ClassService } from "../../../services/meal-order/class.service";
import { EBADF } from "constants";

@Component({
  selector: "print-allocation",
  templateUrl: "./print-allocation.component.html",
  styleUrls: ["./print-allocation.component.css"],
})
export class PrintAllocationComponent implements OnInit {
  stores: StoreInfo[];
  storesCache: StoreInfo[];
  selectedStores: StoreInfo[] = [];
  editOutlet: Outlet;
  outletId: string;
  sessions: MealSessionDetail[] = [];
  allSessions: MealSession[] = [];
  selectedCaterers: CatererOutlet[];
  orderDate;
  orders: TokenOrder[];
  token_count: TokenLabel[] = [];
  dishes: Dish[];
  tokens: MealType[];
  allocation: MealAllocation;
  isSaving = false;
  isLoading = false;
  filterLoading = true;
  isNewAllocation = false;
  cantPrint = true;
  cantSave = true;
  selectedPeriodName = "";
  selectedPeriodId: string | null = null;
  periodPlaceholder = "Meal Period have not been setup";
  mealPeriods: any[];
  mealSessions: any[];
  routes = [];
  newRoutes: Route[] = [];
  //dish_count: TokenDishLabel[] = [];

  constructor(
    private http: HttpClient,
    private alertService: AlertService,
    private deliveryService: DeliveryService,
    public dialog: MatDialog,
    public dialogRef: MatDialogRef<PrintAllocationComponent>,
    private menuService: MenuService,
    private mealService: MealService,
    private dishService: DishService,
    public classService: ClassService,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    if (typeof data.outletId != typeof undefined) {
      this.outletId = data.outletId;
    }
    if (typeof data.allocation != typeof undefined) {
      if (data.allocation.id) {
        this.cantPrint = false;
        this.cantSave = false;
        this.editAllocation(data.allocation);
      } else {
        this.cantPrint = true;
        this.cantSave = true;
        this.newAllocation();
      }
    }
  }

  ngOnInit() {
    this.getRoutes();
    this.getMenuDishes();
    this.getMealTypes();
    this.getPeriodsByDate();
    //this.getTokenOrder();
  }

  loadData() {
    let filterCaterer = "";
    this.selectedCaterers.forEach((caterer, i) => {
      if (caterer.status == "APPROVED") {
        if (filterCaterer != "") {
          filterCaterer += "|";
        }
        filterCaterer += caterer.catererInfoId;
      }
    });
    console.log("caterer filter = ", filterCaterer);

    //this.getStore(filterCaterer);
  }

  onChangeDate(type: string, event: MatDatepickerInputEvent<Date>) {
    this.filterLoading = true;
    console.log("event value: ", moment(event.value));
    this.orderDate = new Date(event.value);
    this.allocation.deliveryDate = this.orderDate;
    //this.getSessions();
    this.getPeriodsByDate();

    if (this.allocation.mealSessionId) {
      console.log("allocation Id: ", this.allocation);
      //this.getTokenOrder();
      this.allocation.mealSessionId = null;
    }
  }

  onChangePackingTime(event: any) {
    this.allocation.packingTime = event;
    console.log("packing time: ", this.allocation.packingTime);
    this.cantSave = false;
  }


  sessionSelect() {
    if (this.allocation.mealSessionId) {
      let sessionDetailSelected = this.sessions.find(
        (x) => x.id === this.allocation.mealSessionId
      );
      let sessionSelected = this.allSessions.find(
        (x) => x.id === sessionDetailSelected.mealSessionId
      );

      let routeSelected = this.routes.find(
        (x) => x.id === sessionDetailSelected.routeId
      );

      this.allocation.outletId = sessionSelected.mealPeriodId;
      this.allocation.timePacked = routeSelected.pickup;
      this.allocation.color = routeSelected.color;
      this.selectedPeriodName = sessionDetailSelected.name;

      console.log("Allocation : ", this.allocation);
    }
  }

  onChangeSessions() {
    this.sessionSelect();
    if (this.allocation.mealSessionId) {
      console.log("allocation Id: ", this.allocation);
      this.getTokenOrder();
    }
  }

  onChangePeriod(event: any) {
    const selectedPeriodId = event.value;
    this.selectedPeriodId = selectedPeriodId;
    this.mealSessions = this.sessions.filter(
      (s) => s.mealPeriodId === selectedPeriodId
    );
    this.allocation.mealSessionId = null;
  }

  getAllSessions() {
    let filter = new Filter();
    //let f = this.catererId ? '(CatererId)==' + this.catererId + ',' : '';
    filter.filters = "(IsActive)==true";
    this.mealService.getMealSessionsSimpleByFilter(filter).subscribe(
      (results) => {
        this.allSessions = results.pagedData;
        console.log("all sessions: ", this.allSessions);
        if (this.allocation.mealSessionId) {
          this.sessionSelect();
        }
        this.filterLoading = false;
      },
      (error) => {
        this.filterLoading = false;
        this.alertService.showStickyMessage(
          "Get Error",
          `An error occured while retrieving meal type.\r\n"`,
          MessageSeverity.error
        );
      }
    );
  }

  getRoutes() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.deliveryService.getRoutesByFilter(filter)
      .subscribe(results => {
        this.routes = results.pagedData;
        this.getAllSessions();
      },
        error => {
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving routes.\r\n"`,
            MessageSeverity.error);
        })
  }

  private cancel() {
    this.dialogRef.close({ isCancel: true });
  }

  onSearchChanged(value: string) {
    this.stores = this.storesCache.filter((r) =>
      Utilities.searchArray(value, false, r.name)
    );
  }

  getOrderData() {
    console.log("clicked");
    this.getTokenOrder();
  }

  getMealTypes() {
    let filter = new Filter();
    filter.sorts = "name";
    //let f = this.catererId ? '(CatererId)==' + this.catererId + ',' : '';
    filter.filters = "(IsActive)==true";
    this.mealService.getMealTypesByFilter(filter).subscribe(
      (results) => {
        this.tokens = results.pagedData;
      },
      (error) => {
        this.alertService.showStickyMessage(
          "Get Error",
          `An error occured while retrieving meal type.\r\n"`,
          MessageSeverity.error
        );
      }
    );
  }

  getMenuDishes() {
    let filter = new Filter();
    //let f = this.catererId ? '(CatererId)==' + this.catererId + ',' : '';
    filter.filters = "(IsActive)==true";
    this.dishService.getDishesByFilter(filter).subscribe(
      (results) => {
        this.dishes = results.pagedData;
      },
      (error) => {
        this.alertService.showStickyMessage(
          "Get Error",
          `An error occured while retrieving dishes.\r\n"`,
          MessageSeverity.error
        );
      }
    );
  }

  getPeriodsByDate() {
    this.classService.getPeriodMealSession(this.outletId).subscribe(
      (results) => {
        this.sessions = results;
        console.log("sessions1: ", this.sessions);

        this.mealPeriods = Array.from(
          new Map(
            results.map((item) => [
              item.mealPeriodId,
              {
                mealPeriodId: item.mealPeriodId,
                mealPeriodName: item.mealPeriodName,
              },
            ])
          ).values()
        );

        if (!this.isNewAllocation && this.allocation.mealSessionId) {
          const selectedSession = this.sessions.find(
            (s) => s.id === this.allocation.mealSessionId
          );
          if (selectedSession) {
            this.selectedPeriodId = selectedSession.mealPeriodId;
            // auto-select period based on session
            this.mealSessions = this.sessions.filter(
              (s) => s.mealPeriodId === selectedSession.mealPeriodId
            );
          }
        }

        if (!this.sessions || this.sessions.length === 0) {
          this.periodPlaceholder = "Meal Period have not been setup";
        } else {
          this.periodPlaceholder = "Select a Meal Period";
        }

        if (!this.isNewAllocation) {
          console.log("edit allocation: ", this.token_count)
          this.token_count.forEach((t) => {
            if (t.timePacked == null) {
              let sessionDetailSelected = this.sessions.find(
                (x) => x.id === this.allocation.mealSessionId
              );

              console.log("session selected: ", sessionDetailSelected)

              console.log("routes: ", this.routes)

              let routeSelected = this.routes.find(
                (x) => x.id === sessionDetailSelected.routeId
              );

              console.log("route selected: ", routeSelected)

              t.timePacked = routeSelected.pickup;
              t.color = routeSelected.color;
            }
          });

          console.log("token_count: ", this.token_count )
        }

        this.filterLoading = false;
      },
      (error) => {
        this.filterLoading = false;
        //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
        this.alertService.showStickyMessage(
          "Get Error",
          `An error occured while retrieving meal sessions.\r\n"`,
          MessageSeverity.error
        );
      }
    );
  }

  getTokenName(id) {
    if (!this.tokens || this.tokens.length === 0) return '';
    let token = this.tokens.find((x) => x.id === id);
    if (token) {
      return token.name;
    } else {
      return "";
    }
  }

  getDishName(id) {
    let dish = this.dishes.find((x) => x.id === id);
    if (dish) {
      //return dish.label + ' (' + dish.id + ')';
      return dish.label;
    } else {
      return "";
    }
  }

  getDishCode(id) {
    let dish = this.dishes.find((x) => x.id === id);
    if (dish) {
      return dish.code;
    } else {
      return "";
    }
  }

  onChangeQty(event, detail?: TokenDishLabel, token?: TokenLabel) {
    if (detail.a_qty > 0) {
      detail.t_qty = detail.o_qty + detail.a_qty;
    } else {
      detail.t_qty = detail.o_qty + detail.p_qty;
    }

    token.qty_tdishes = 0;

    token.dishes.forEach((d) => {
      token.qty_tdishes += d.t_qty;
    });
  }

  public showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }

  getTokenOrder() {
    this.isLoading = true;
    var insideFilter = new Filter();

    this.token_count = [];

    var strDate = moment(this.orderDate).format().split("T");
    let f = this.allocation.mealSessionId
      ? "(OrderCurrentSession)==" + this.allocation.mealSessionId + ","
      : "";
    insideFilter.filters =
      f +
      "(IsActive)==true,(outletId)==" +
      this.outletId +
      ",(DeliveryDate)==" +
      strDate[0];
    insideFilter.filters += ",(Status)==paid";
    console.log("filters: ", insideFilter.filters);

    this.menuService.getOrderwithCurrentSession(insideFilter).subscribe(
      (results) => {
        console.log("results: ", results);
        this.orders = results;

        console.log("order : ", this.orders);

        //Order Loop
        this.orders.forEach((o) => {
          console.log("order: ", o);

          //Token Loop
          o.tokens.forEach((t) => {
            if (t.qty == 0) t.qty = 1;

            var tokenIndex = this.token_count.findIndex(
              (x) => x.token_id === t.tokenId
            );
            if (tokenIndex < 0) {
              var token = new TokenLabel();
              token.order_id = o.id;
              token.token_id = t.tokenId;
              token.meal_allocation_id = this.allocation.id;
              token.timePacked = this.allocation.timePacked;
              token.color = this.allocation.color;
              token.token_name = this.getTokenName(t.tokenId);
              console.log("combined dish: ", t.selectedCombinedDishes.length);
              if (t.selectedCombinedDishes.length > 0) {
                console.log("menuqty: ", t.selectedCombinedDishes[0].menuQty);
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
              tokenIndex = this.token_count.findIndex(
                (x) => x.token_id === t.tokenId
              );
            } else {
              if (t.selectedCombinedDishes.length > 0) {
                this.token_count[tokenIndex].qty +=
                  t.qty * t.selectedCombinedDishes[0].menuQty;
              } else {
                this.token_count[tokenIndex].qty += t.qty;
              }
              this.token_count[tokenIndex].qty_menus += t.qty;
            }

            // Selected dishes loop
            t.selectedDishes.forEach((d) => {
              var dishIndex = this.token_count[tokenIndex].dishes.findIndex(
                (y) => y.dish_id === d.dishId
              );
              if (dishIndex < 0) {
                var dish = new TokenDishLabel();
                //dish.token_lable_id = t.id;
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
            });
          });
        });

        //count proposed allocation
        this.token_count.forEach((t) => {
          t.qty_pdishes = 0;
          t.qty_tdishes = 0;

          t.dishes.sort((a, b) => a.dish_name.localeCompare(b.dish_name));

          t.dishes.forEach((d) => {
            console.log(
              "hasil awal: ",
              (d.o_qty / t.qty_dishes) * (t.qty - t.qty_dishes)
            );
            var p_allocation = Math.ceil(
              (d.o_qty / t.qty_dishes) * (t.qty - t.qty_dishes)
            );
            console.log("ceiling: ", p_allocation);
            d.p_qty = p_allocation;
            d.t_qty = d.o_qty + d.p_qty;
            t.qty_pdishes += p_allocation;
            t.qty_tdishes += d.t_qty;
          });
        });



        this.isLoading = false;
        console.log("token count: ", this.token_count);
      },
      (error) => {
        this.isLoading = false;
      }
    );
  }

  save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    this.allocation.outletId = this.outletId;
    this.allocation.tokens = this.token_count;
    this.allocation.deliveryDate = new Date(
      this.orderDate.getTime() - this.orderDate.getTimezoneOffset() * 60000
    );
    console.log("saving: ", this.allocation);


    if (this.isNewAllocation) {
      this.menuService.newMealAllocation(this.allocation).subscribe(
        (allocation) => this.saveSuccessHelper(allocation),
        (error) => this.saveFailedHelper(error)
      );
    } else {
      this.menuService.updateMealAllocation(this.allocation).subscribe(
        (response) => this.saveSuccessHelper(),
        (error) => this.saveFailedHelper(error)
      );
    }
  }

  private saveSuccessHelper(allocation?: MealAllocation) {
    if (allocation) Object.assign(this.allocation, allocation);

    this.cantPrint = false;
    this.isSaving = false;
    this.alertService.stopLoadingMessage();

    if (this.isNewAllocation)
      this.alertService.showMessage(
        "Success",
        `Meal Allocation was created successfully`,
        MessageSeverity.success
      );
    else
      this.alertService.showMessage(
        "Success",
        `Changes to Meal Allocations was saved successfully`,
        MessageSeverity.success
      );

    this.allocation = new MealAllocation();

    this.dialogRef.close();
  }

  private saveFailedHelper(error: any) {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage(
      "Save Error",
      "The below errors occured while saving your changes:",
      MessageSeverity.error
    );
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);
  }

  newAllocation() {
    this.isNewAllocation = true;

    this.allocation = new MealAllocation();
    //this.orderDate = moment().toDate();
    //this.orderDate.setHours(0, 0, 0, 0);
    //this.allocation.deliveryDate = this.orderDate;

    return this.allocation;
  }

  editAllocation(allocation: MealAllocation) {
    this.cantPrint = false;
    if (allocation) {
      this.isNewAllocation = false;

      console.log("allocation inside: ", allocation);

      this.allocation = new MealAllocation();
      Object.assign(this.allocation, allocation);
      this.orderDate = new Date(this.allocation.deliveryDate);
      this.token_count = this.allocation.tokens;

      return this.allocation;
    } else {
      return this.allocation;
    }
  }

  print() {
    console.log("download Label");
    this.downloadLabel();
  }

  downloadLabel() {
    this.isLoading = true;
    const fileName = moment().format("DDMMYYYY_hhmmss") + "_OrderLabel.pdf";

    console.log("token sent: ", this.token_count);

    this.menuService.downloadOrderLabel(this.token_count).subscribe(
      (data) => {
        console.log(data);
        this.isLoading = false;
        saveAs(data, fileName);
      },
      (err) => {
        this.isLoading = false;
        alert("Problem while downloading the file.");
        console.error(err);
      }
    );
  }

  addDish() {
    const dialogRef = this.dialog.open(DishSelectorComponent, {
      width: "1000px",
      data: { header: "Sub Dishes", dishes: this.dishes, catererId: "" },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (!result.isCancel) {
        var strDate = moment(this.orderDate).format().split("T");

        result.selectedDishes.forEach((selectedDish, index, ps) => {
          if (selectedDish.checked) {
            console.log("selected: ", selectedDish);
            console.log("token", this.tokens);
            console.log("token_count", this.token_count);

            if (!this.token_count || this.token_count.length === 0) {
              // Initialize from this.tokens
              this.token_count = this.tokens.map((token) => {
                const newToken = new TokenLabel();
                newToken.token_id = token.id;
                newToken.token_name = token.name;
                newToken.qty = 0;
                newToken.qty_menus = 0;
                newToken.qty_dishes = 0;
                newToken.qty_tdishes = 0;
                newToken.deliveryDate = strDate[0];
                newToken.timePacked = this.allocation.timePacked;
                newToken.color = this.allocation.color;
                newToken.dishes = [];
                return newToken;
              });
              console.log(
                "Initialized token_count from tokens:",
                this.token_count
              );
            }

            this.token_count.forEach((token) => {
              // Find if the dish already exists for this token
              const dishIndex = token.dishes.findIndex(
                (d) => d.dish_id === selectedDish.id
              );

              if (dishIndex < 0) {
                // New dish — create a new TokenDishLabel entry
                const dish = new TokenDishLabel();
                dish.token_id = token.token_id;
                dish.token_name = this.getTokenName(token.token_id);
                dish.dish_id = selectedDish.id;
                dish.dish_name = this.getDishName(selectedDish.id);
                dish.dish_code = this.getDishCode(selectedDish.id);
                dish.o_qty = selectedDish.qty ? selectedDish.qty : 0;
                dish.t_qty = 0;
                token.dishes.push(dish);
              } else {
                // Existing dish — increase its ordered quantity
                token.dishes[dishIndex].o_qty += selectedDish.qty
                  ? selectedDish.qty
                  : 0;
              }

              // Update total quantities per token
              token.qty_dishes = token.dishes.reduce(
                (sum, d) => sum + (d.o_qty || 0),
                0
              );
              token.qty_tdishes = token.dishes.reduce(
                (sum, d) => sum + (d.t_qty || 0),
                0
              );
            });
          }
        });

        console.log("token", this.tokens);
        console.log("token_count", this.token_count);
      }
    });
  }
}
