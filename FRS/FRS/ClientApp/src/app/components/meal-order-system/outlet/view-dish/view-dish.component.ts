import { Component, ViewChild, Inject } from '@angular/core';

import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Cuisine } from 'src/app/models/meal-order/cuisine.model';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { Filter, PagedResult } from 'src/app/models/sieve-filter.model';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';
import { DishService } from 'src/app/services/meal-order/dish.service';
import { AccountService } from 'src/app/services/account.service';
import { Permission } from 'src/app/models/permission.model';
import { MenuService } from 'src/app/services/meal-order/menu.service';
import { MealTypeMenuDish, Menu, MenuDish, OutletMenuDish } from 'src/app/models/meal-order/menu.model';
import { forEach } from '@angular/router/src/utils/collection';
import { MealType } from 'src/app/models/meal-order/meal-type.model';
import { Observable } from 'rxjs';
import { DishCycle, DishCyclePeriod, DishCycleScheduleDetailMenu, DishCycleScheduleSet, OutletDishCyclePeriodMenu, OutletDishViewMenu } from 'src/app/models/meal-order/dish-cycle';


@Component({
  selector: 'outlet-view-dish',
  templateUrl: './view-dish.component.html',
  styleUrls: ['./view-dish.component.css']
})
export class OutletViewDishCycleMenuComponent {
  public dishCycle: DishCycle;
  public day: number;
  public periods: DishCyclePeriod[] = [];
  private isSaving: boolean;
  public formResetToggle = true;
  private menus: Menu[] = [];
  private mealTypes: MealType[] = [];
  private outletId: any;
  private catererId: any;
  public sets: DishCycleScheduleSet[] = [];
  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private menuService: MenuService, private accountService: AccountService,
    public dialogRef: MatDialogRef<OutletViewDishCycleMenuComponent>, private mealService: MealService, private dishService: DishService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.dishCycle) != typeof (undefined)) {
      this.outletId = data.outletId;
      this.dishCycle = data.dishCycle;
      this.day = data.day;
      this.catererId = data.catererId;
      this.getDishCycleMenu();
      //this.getMenus();
    }
  }

  getMenus() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.menuService.getMenusByFilter(filter)
      .subscribe(results => {
        this.menus = results.pagedData;
      },
        error => {
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving menus.\r\n"`,
            MessageSeverity.error);
        })
  }

  getDishCycleMenu() {
    this.dishService.getOutletDishCyclePeriods(this.dishCycle.id)
      .subscribe(periods => {
        if (!periods) periods = [];
        this.dishCycle.dishCyclePeriods = periods;
        this.dishService.getDishCycleScheduleSetDetails(this.dishCycle.id, this.day, this.outletId)
          .subscribe(results => {
            const res = results ? [...results] : [];
            this.dishCycle.dishCyclePeriods.forEach((period: DishCyclePeriod, index, arr) => {
              const sets = [];
              res.forEach((set: DishCycleScheduleSet, i, a) => {
                let s = new DishCycleScheduleSet();
                Object.assign(s, set);
                s.menus = [];
                set.menus.forEach((menu: DishCycleScheduleDetailMenu, x, ar) => {
                  let m = new DishCycleScheduleDetailMenu();
                  Object.assign(m, menu);
                  m.checked = !(period.excludedMenus.findIndex(e => e.dishCyclePeriodId == period.id && e.outletId == this.outletId && e.dishCycleScheduleSetId == set.id && e.dishId == menu.dishId) > -1);
                  s.menus.push(m);
                });
                sets.push(s);
              });
              period.sets = [...sets];
            });
          },
            error => {
              this.alertService.showStickyMessage("Get Error", `An error occured while retrieving the records.\r\n"`,
                MessageSeverity.error);
            })
      },
        error => {
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving the records.\r\n"`,
            MessageSeverity.error);
        });
  }
  
  //async getDishCycleMenu() {
  //  this.getMealTypes().subscribe(results => {
  //    let mealTypes = results.pagedData;
  //    this.menuService.getOutletDishCyclePeriods(this.outletId, this.catererId, this.dishCycle.id, this.day)
  //      .subscribe(results => {
  //        this.periods = results;
  //        this.periods.forEach((period: DishCyclePeriod, index, arr) => {
  //          if (!period.menus) period.menus = [];
  //          period.menus.forEach((menu: DishCycleScheduleMenu, i, a) => {
  //            if (!period.outletMenus) period.outletMenus = [];
  //            menu.checked = !(period.outletMenus.findIndex(e => e.outletId == this.outletId && e.menuId == menu.menuId) > -1);

  //            //dishes
  //            menu.menu.mealTypeMenuDishes = [];
  //            mealTypes.forEach((mealType: MealType, m, x) => {
  //              let mtDish = new MealTypeMenuDish();
  //              mtDish.mealTypeId = mealType.id;
  //              mtDish.mealTypeName = mealType.name;
  //              mtDish.menuId = menu.menu.id;
  //              mtDish.menuDishes = menu.menu.menuDishes.filter(e => e.menuId == menu.menu.id && e.mealTypeId == mealType.id).map((e) => {
  //                let md = new MenuDish();
  //                Object.assign(md, e);
  //                md.checked = (menu.checked && (!period.outletMenuDishes || (period.outletMenuDishes.findIndex(e => e.outletId == this.outletId && e.menuId == menu.menuId &&
  //                              e.mealTypeId == mealType.id && e.dishId == md.dishId) < 0)));
  //                return md;
  //              }); 

  //              menu.menu.mealTypeMenuDishes.push(mtDish);
  //            });

  //            console.log(menu.menu.mealTypeMenuDishes);
  //          });
  //        });
  //      },
  //        error => {
  //          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving the records.\r\n"`,
  //            MessageSeverity.error);
  //        })
  //  },
  //    error => {
  //      this.alertService.showStickyMessage("Get Error", `An error occured while retrieving the records.\r\n"`,
  //        MessageSeverity.error);
  //    });
    
  //}

  selectDeselectAllDishes(menu) {
    if (menu.menu) {
      menu.menu.mealTypeMenuDishes.forEach((mealTypeMenuDish: MealTypeMenuDish, index, arr) => {
        mealTypeMenuDish.menuDishes.forEach((menuDish: MenuDish, i, a) => {
          menuDish.checked = menu.checked;
        });
      });
    }
  }

  selectMenu(menuDish, menu) {
    //if (menuDish.checked) menu.checked = true;
  }

  getMealTypes() {
    let filter = new Filter();
    filter.sorts = 'name';
    //let f = this.dishCycle.catererId ? '(CatererId)==' + this.dishCycle.catererId + ',' : '';
    filter.filters = '(MealTypeByOutletProfileId)==' + this.dishCycle.outletProfileId + ',(IsActive)==true';
    //filter.filters = f + '(IsActive)==true';
    return this.mealService.getMealTypesByFilter(filter);
  }

  private cancel() {
    this.dialogRef.close();
  }


  private save() {
    this.isSaving = true;
    //this.alertService.startLoadingMessage("Saving changes...");
    let excludedMenus: OutletDishCyclePeriodMenu[] = []
    this.dishCycle.dishCyclePeriods.forEach((period: DishCyclePeriod, index, arr) => {
      period.sets.forEach((set: DishCycleScheduleSet, i, a) => {
        set.menus.forEach((menu: DishCycleScheduleDetailMenu, x, ar) => {
          if (!menu.checked) {
            let m = new OutletDishCyclePeriodMenu();
            m.dishCyclePeriodId = period.id;
            m.dishCycleScheduleSetId = set.id;
            m.dishId = menu.dishId;
            m.outletId = this.outletId;

            excludedMenus.push(m);
          }
        });
      });
    });
    let model = new OutletDishViewMenu();
    model.outletId = this.outletId;
    model.excludedMenus = excludedMenus;
    this.dishService.createOutletDishCyclePeriodMenus(model).subscribe(response => this.saveSuccessHelper(response), error => this.saveFailedHelper(error));
  }


  private saveSuccessHelper(response: any) {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();

    if (response.isSuccess)
      this.alertService.showMessage("Success", `Successfully saved.`, MessageSeverity.success);
    else
      this.alertService.showMessage("Error", response.message, MessageSeverity.error);


    this.resetForm();

    this.dialogRef.close();
  }

  private saveFailedHelper(error: any) {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage("Save Error", "The below errors occured while saving your changes:", MessageSeverity.error);
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);

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

}
