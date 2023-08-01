import { Component, ViewChild, Inject } from '@angular/core';

import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Cuisine } from 'src/app/models/meal-order/cuisine.model';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { Filter, PagedResult } from 'src/app/models/sieve-filter.model';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';
import { DishService } from 'src/app/services/meal-order/dish.service';
import { AccountService } from 'src/app/services/account.service';
import { Permission } from 'src/app/models/permission.model';
import { MenuCycle, MenuCycleScheduleMenu, MenuCycleSchedulePeriod, OutletMenuCycleScheduleMenu } from 'src/app/models/meal-order/menu-cycle.model';
import { MenuService } from 'src/app/services/meal-order/menu.service';
import { MealTypeMenuDish, Menu, MenuDish, OutletMenuDish } from 'src/app/models/meal-order/menu.model';
import { forEach } from '@angular/router/src/utils/collection';
import { MealType } from 'src/app/models/meal-order/meal-type.model';
import { Observable } from 'rxjs';


@Component({
  selector: 'outlet-view-menu',
  templateUrl: './view-menu.component.html',
  styleUrls: ['./view-menu.component.css']
})
export class OutletViewMenuCycleMenuComponent {
  public menuCycle: MenuCycle;
  public day: number;
  public periods: MenuCycleSchedulePeriod[] = [];
  private isSaving: boolean;
  public formResetToggle = true;
  private menus: Menu[] = [];
  private mealTypes: MealType[] = [];
  private outletId: any;
  private catererId: any;
  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private menuService: MenuService, private accountService: AccountService,
    public dialogRef: MatDialogRef<OutletViewMenuCycleMenuComponent>, private mealService: MealService, 
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.menuCycle) != typeof (undefined)) {
      this.outletId = data.outletId;
      this.menuCycle = data.menuCycle;
      this.day = data.day;
      this.catererId = data.catererId;
      this.getMealTypes();
      this.getMenuCycleMenu();
      this.getMenus();
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

  
  async getMenuCycleMenu() {
    this.getMealTypes().subscribe(results => {
      let mealTypes = results.pagedData;
      this.menuService.getOutletMenuCycleSchedulePeriods(this.outletId, this.catererId, this.menuCycle.id, this.day)
        .subscribe(results => {
          this.periods = results;
          this.periods.forEach((period: MenuCycleSchedulePeriod, index, arr) => {
            if (!period.menus) period.menus = [];
            period.menus.forEach((menu: MenuCycleScheduleMenu, i, a) => {
              if (!period.outletMenus) period.outletMenus = [];
              menu.checked = !(period.outletMenus.findIndex(e => e.outletId == this.outletId && e.menuId == menu.menuId) > -1);

              //dishes
              menu.menu.mealTypeMenuDishes = [];
              mealTypes.forEach((mealType: MealType, m, x) => {
                let mtDish = new MealTypeMenuDish();
                mtDish.mealTypeId = mealType.id;
                mtDish.mealTypeName = mealType.name;
                mtDish.menuId = menu.menu.id;
                mtDish.menuDishes = menu.menu.menuDishes.filter(e => e.menuId == menu.menu.id && e.mealTypeId == mealType.id).map((e) => {
                  let md = new MenuDish();
                  Object.assign(md, e);
                  md.checked = (menu.checked && (!period.outletMenuDishes || (period.outletMenuDishes.findIndex(e => e.outletId == this.outletId && e.menuId == menu.menuId &&
                                e.mealTypeId == mealType.id && e.dishId == md.dishId) < 0)));
                  return md;
                }); 

                menu.menu.mealTypeMenuDishes.push(mtDish);
              });

              console.log(menu.menu.mealTypeMenuDishes);
            });
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
    if (menuDish.checked) menu.checked = true;
  }

  getMealTypes() {
    let filter = new Filter();
    filter.sorts = 'name';
    //let f = this.menuCycle.catererId ? '(CatererId)==' + this.menuCycle.catererId + ',' : '';
    filter.filters = '(MealTypeByOutletProfileId)==' + this.menuCycle.outletProfileId + ',(IsActive)==true';
    //filter.filters = f + '(IsActive)==true';
    return this.mealService.getMealTypesByFilter(filter);
  }

  private cancel() {
    this.dialogRef.close();
  }


  private save() {
    this.isSaving = true;
    //this.alertService.startLoadingMessage("Saving changes...");
    let outletPeriods: MenuCycleSchedulePeriod[] = [];
    this.periods.forEach((period: MenuCycleSchedulePeriod, index, arr) => {
      let outletPeriod = new MenuCycleSchedulePeriod();
      Object.assign(outletPeriod, period);

      if (!period.menus) period.menus = [];
      let outletMenuDishes = [];
      let outletMenus = [];
      period.menus.map((e) => {
        let outletMenu = new OutletMenuCycleScheduleMenu();
        Object.assign(outletMenu, e);
        outletMenu.outletId = this.outletId;
        if (!e.checked) {
          outletMenu.menu = null;
          outletMenus.push(outletMenu);
        }
        
        //outletMenu.menu.outletMenuDishes = [];
        e.menu.mealTypeMenuDishes.forEach((mealTypeMenuDish: MealTypeMenuDish, m, x) => {
          mealTypeMenuDish.menuDishes.forEach((menuDish: MenuDish, m, x) => {
            if (!menuDish.checked) {
              let outletMenuDish = new OutletMenuDish();
              outletMenuDish.outletId = this.outletId;
              outletMenuDish.mealTypeId = menuDish.mealTypeId;
              outletMenuDish.menuId = menuDish.menuId;
              outletMenuDish.dishId = menuDish.dishId;
              outletMenuDish.mealPeriodId = period.mealPeriodId;
              outletMenuDishes.push(outletMenuDish);
            }
          });
        });


      });

      outletPeriod.outletMenus = outletMenus;
      outletPeriod.outletMenuDishes = outletMenuDishes;
      outletPeriods.push(outletPeriod);
    });

    this.menuService.createOutletPeriodMenus(outletPeriods).subscribe(response => this.saveSuccessHelper(response), error => this.saveFailedHelper(error));
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

  addMenu(period: any, m: Menu) {
    if (!m || !period) return;
    let menu = new MenuCycleScheduleMenu();
    menu.mealPeriodId = period.mealPeriodId;
    menu.menuId = m.id;
    menu.label = m.label;
    if (!period.menus) period.menus = [];
    period.menus.push(menu);

    m = null;
    period.selectedMenu = null;
  }

  removeMenu(period: MenuCycleSchedulePeriod, id: string) {
    let indx = period.menus.findIndex(f => f.menuId == id);
    if (indx > -1) {
      period.menus.splice(indx, 1);
    }
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
