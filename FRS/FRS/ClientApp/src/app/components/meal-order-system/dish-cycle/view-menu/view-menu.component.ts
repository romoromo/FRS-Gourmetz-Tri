import { Component, ViewChild, Inject } from '@angular/core';

import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Cuisine } from 'src/app/models/meal-order/cuisine.model';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { Filter } from 'src/app/models/sieve-filter.model';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';
import { DishService } from 'src/app/services/meal-order/dish.service';
import { AccountService } from 'src/app/services/account.service';
import { Permission } from 'src/app/models/permission.model';
import { MenuService } from 'src/app/services/meal-order/menu.service';
import { Menu } from 'src/app/models/meal-order/menu.model';
import { FormControl } from '@angular/forms';
import { DishCycle, DishCycleScheduleSet } from 'src/app/models/meal-order/dish-cycle';


@Component({
  selector: 'view-dish-menu',
  templateUrl: './view-menu.component.html',
  styleUrls: ['./view-menu.component.css']
})
export class ViewDishCycleMenuComponent {
  public dishCycle: DishCycle;
  public day: number;
  public sets: DishCycleScheduleSet[] = [];
  private isSaving: boolean;
  public formResetToggle = true;
  private menus: Menu[] = [];
  public catererId: string;
  public searchForm: FormControl = new FormControl();
  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private menuService: MenuService, private accountService: AccountService, private dishService: DishService,
    public dialogRef: MatDialogRef<ViewDishCycleMenuComponent>, 
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.dishCycle) != typeof (undefined)) {
      this.catererId = data.catererId;
      this.dishCycle = data.dishCycle;
      this.day = data.day;

      this.getDishCycleMenu();
      //this.getMenus();
    }
  }

  //getMenus() {
  //  let filter = new Filter();
  //  let f = this.catererId ? '(CatererId)==' + this.catererId + ',' : '';
  //  filter.filters = f + '(IsActive)==true';
  //  this.menuService.getMenusByFilter(filter)
  //    .subscribe(results => {
  //      this.menus = results.pagedData;
  //    },
  //      error => {
  //        this.alertService.showStickyMessage("Get Error", `An error occured while retrieving menus.\r\n"`,
  //          MessageSeverity.error);
  //      })
  //}

  getDishCycleMenu() {
    this.dishService.getDishCycleScheduleSetDetails(this.dishCycle.id, this.day)
      .subscribe(results => {
        this.sets = results;
      },
        error => {
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving the records.\r\n"`,
            MessageSeverity.error);
        })
  }

  private cancel() {
    this.dialogRef.close();
  }


  private save() {
    this.isSaving = true;
    //this.alertService.startLoadingMessage("Saving changes...");
    //this.menuService.createPeriodMenus(this.periods).subscribe(response => this.saveSuccessHelper(response), error => this.saveFailedHelper(error));
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

  //addMenu(period: any, m: Menu) {
  //  if (!m || !period) return;
  //  let menu = new DishCycleScheduleMenu();
  //  menu.mealPeriodId = period.mealPeriodId;
  //  menu.menuId = m.id;
  //  menu.label = m.label;
  //  menu.code = m.code;
  //  if (!period.menus) period.menus = [];
  //  period.menus.push(menu);

  //  m = null;
  //  period.selectedMenu = null;
  //}

  //removeMenu(period: DishCycleSchedulePeriod, id: string) {
  //  let indx = period.menus.findIndex(f => f.menuId == id);
  //  if (indx > -1) {
  //    period.menus.splice(indx, 1);
  //  }
  //}

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
