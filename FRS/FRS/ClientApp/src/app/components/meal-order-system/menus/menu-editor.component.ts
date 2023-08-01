import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { DateAdapter, MatDatepickerInputEvent, MatDialog, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { MealTypeMenuDish, Menu, MenuDish } from 'src/app/models/meal-order/menu.model';
import { MenuService } from 'src/app/services/meal-order/menu.service';
import { Filter } from 'src/app/models/sieve-filter.model';
import { Dish } from 'src/app/models/meal-order/dish.model';
import { DishService } from 'src/app/services/meal-order/dish.service';
import * as moment from 'moment';
import { MAT_MOMENT_DATE_FORMATS } from '@angular/material-moment-adapter';
import { MomentUtcDateAdapter } from 'src/app/helpers/moment-utc-adapter';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { MealType, MealTypeDish } from 'src/app/models/meal-order/meal-type.model';
import { Cuisine } from 'src/app/models/meal-order/cuisine.model';
import { DishSelectorComponent } from '../dishes/dish-selector/dish-selector.component';
import { MenuDishSelectorEditorComponent } from './dish-selector/dish-selector-editor.component';

@Component({
  selector: 'menu-editor',
  templateUrl: './menu-editor.component.html',
  styleUrls: ['./menu-editor.component.css'],
  providers: [
    { provide: MAT_DATE_LOCALE, useValue: 'en-SG' },
    { provide: MAT_DATE_FORMATS, useValue: MAT_MOMENT_DATE_FORMATS },
    { provide: DateAdapter, useClass: MomentUtcDateAdapter },
  ]
})
export class MenuEditorComponent {

  private isNewMenu = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingMenuCode: string;
  private menuEdit: Menu = new Menu();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;
  private dishes: Dish[] = [];
  private cuisines: Cuisine[] = [];
  start = new Date();
  end = new Date();

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;
  public catererId: string;

  public page: string;
  public totalCount: number;
  public dishMealTypes: any;
  public rows: any[];
  public mealTypes: any;
  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private menuService: MenuService, private accountService: AccountService,
    public dialogRef: MatDialogRef<MenuEditorComponent>, private dishService: DishService, private mealService: MealService,
    @Inject(MAT_DIALOG_DATA) public data: any, public dialog: MatDialog, 
    private dateAdapter: DateAdapter<Date>) {
    this.dateAdapter.setLocale('en-SG');
    if (typeof (data.menu) != typeof (undefined)) {
      this.catererId = data.catererId;
      if (data.menu.id) {
        this.editMenu(data.menu);
      } else {
        this.newMenu();
      }
    }

    this.getMealTypes();
    this.getCuisines();
    this.getMenuMealTypeDishes();
  }

  getMenuMealTypeDishes(ev?) {
    let menuId = this.menuEdit.id;
    let page = -1;//get everything //!ev ? 1 : ev.offset + 1;
    let pageSize = -1;
    this.menuService.getMenuMealTypesById(menuId, page ? page : 1, pageSize)
      .subscribe(results => {
        this.dishMealTypes = results;
        this.rows = results.rows;
        this.totalCount = results.totalCount;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving dishes.\r\n"`,
            MessageSeverity.error);
        })
  }

  getCuisines() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.dishService.getCuisinesByFilter(filter)
      .subscribe(results => {
        this.cuisines = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving cuisines.\r\n"`,
            MessageSeverity.error);
        })
  }

  onChangeDate(type: string, event: MatDatepickerInputEvent<Date>) {
    if (type == 'start') {
      this.start = new Date(event.value);
      this.menuEdit.startDate = new Date(event.value);
    }

    if (type == 'end') {
      this.end = new Date(event.value);
      this.menuEdit.endDate = new Date(event.value);
    }
  }

  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    this.rows
    //let selectedDishes = this.menuEdit.mealTypeMenuDishes.filter(f => f.checked);
    //console.log(selectedDishes);
    this.menuEdit.menuDishes = [];
    this.rows.forEach((p, index, ps) => {
      p.cells.forEach((d, i, ds) => {
        if (d.checked) {
          let dtDish = new MenuDish();
          dtDish.menuId = this.menuEdit.id;
          dtDish.dishId = d.dishId;
          dtDish.mealTypeId = d.mealTypeId;
          this.menuEdit.menuDishes.push(dtDish);
        }
      });
      
    });

    console.log(this.menuEdit.menuDishes);
    if (this.isNewMenu) {
      this.menuService.newMenu(this.menuEdit).subscribe(menu => this.saveSuccessHelper(menu), error => this.saveFailedHelper(error));
    }
    else {
      this.menuService.updateMenu(this.menuEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }


  private saveSuccessHelper(menu?: Menu) {
    if (menu)
      Object.assign(this.menuEdit, menu);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewMenu)
      this.alertService.showMessage("Success", `Menu \"${this.menuEdit.label}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to menu \"${this.menuEdit.label}\" was saved successfully`, MessageSeverity.success);


    this.menuEdit = new Menu();
    this.resetForm();


    //if (!this.isNewMenu && this.accountService.currentUser.facilities.some(r => r == this.editingMenuCode))
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
    this.menuEdit = new Menu();

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


  newMenu() {
    this.isNewMenu = true;
    this.showValidationErrors = true;

    this.editingMenuCode = null;
    this.selectedValues = {};
    this.menuEdit = new Menu();
    this.start = new Date();
    this.end = new Date();
    this.menuEdit.startDate = this.start;
    this.menuEdit.endDate = this.end;
    this.menuEdit.catererId = this.catererId;
    return this.menuEdit;
  }

  editMenu(menu: Menu) {
    if (menu) {
      this.isNewMenu = false;
      this.showValidationErrors = true;

      this.editingMenuCode = menu.label;
      this.selectedValues = {};
      this.menuEdit = new Menu();
      Object.assign(this.menuEdit, menu);
      this.start = this.menuEdit.startDate;
      this.end = this.menuEdit.endDate;
      return this.menuEdit;
    }
    else {
      return this.newMenu();
    }
  }

  getMealTypes() {
    let filter = new Filter();
    filter.sorts = 'name';
    let f = this.catererId ? '(CatererId)==' + this.catererId + ',' : '';
    filter.filters = f + '(IsActive)==true';
    this.mealService.getMealTypesByFilter(filter)
      .subscribe(results => {
        let mealTypes = results.pagedData;
        this.mealTypes = mealTypes;
        if (!this.menuEdit.mealTypeMenuDishes) {
          this.menuEdit.mealTypeMenuDishes = [];
        }

        let dishFilter = new Filter();
        dishFilter.sorts = 'label';
        dishFilter.filters = f + '(IsActive)==true';
        this.dishService.getDishesByFilter(dishFilter)
          .subscribe(res => {
            let dishes = res.pagedData;
            
            mealTypes.forEach((mp: MealType, index, m) => {
              let mealTypeDish = new MealTypeMenuDish();
              mealTypeDish.menuDishes = [];
              mealTypeDish.mealTypeId = mp.id;
              mealTypeDish.mealTypeName = mp.name;
              dishes.forEach((d: Dish, index, dish) => {
                if (!this.menuEdit.menuDishes) this.menuEdit.menuDishes = [];
                let existingDishes = this.menuEdit.menuDishes.filter(e => e.mealTypeId == mp.id && e.dishId == d.id);
                if (existingDishes && existingDishes.length > 0) {
                  let existingDish = existingDishes[0];
                  existingDish.checked = true;
                  mealTypeDish.menuDishes.push(existingDishes[0]);
                } else {
                  let newMenuDish = new MenuDish();
                  newMenuDish.dishId = d.id;
                  newMenuDish.menuId = this.menuEdit.id;
                  newMenuDish.mealTypeId = mp.id;
                  newMenuDish.dishName = d.label;
                  mealTypeDish.menuDishes.push(newMenuDish);
                }
              });

              this.menuEdit.mealTypeMenuDishes.push(mealTypeDish);
            },
              error => {
                this.alertService.showStickyMessage("Get Error", `An error occured while retrieving meal types.\r\n"`,
                  MessageSeverity.error);
              });

          });
      },
        error => {
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving sessions.\r\n"`,
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
        this.dishes.forEach((p, index, ps) => {
          (<any>p).checked = this.menuEdit.menuDishes != null && this.menuEdit.menuDishes.findIndex(f => f.dishId == p.id) > -1;
        });
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving dishes.\r\n"`,
            MessageSeverity.error);
        })
  }

  addDish(mealTypeId, mealTypeName: string) {
    let mealType = this.menuEdit.mealTypeMenuDishes.filter(e => e.mealTypeId == mealTypeId);
    let selectedDishes: MenuDish[] = [];
    if (mealType && mealType.length > 0) {
      selectedDishes = mealType[0].menuDishes.filter(e => e.checked);
    }
    
    const dialogRef = this.dialog.open(DishSelectorComponent, {
      data: { header: "Dishes", dishes: selectedDishes, catererId: this.catererId },
      width: '800px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result && !result.isCancel) {
        let menuDishes: MenuDish[] = [];
        result.selectedDishes.forEach((d, index, ps) => {
          if (d.checked) {
            let dtDish = new MenuDish();
            dtDish.menuId = this.menuEdit.id;
            dtDish.dishId = d.id;
            dtDish.mealTypeId = mealTypeId;
            dtDish.checked = true;
            dtDish.dishName = d.label;
            menuDishes.push(dtDish);
          }
        });

        let mealTypeDishes = this.menuEdit.mealTypeMenuDishes.filter((cg) => cg.mealTypeId == mealTypeId);
        if (mealTypeDishes && mealTypeDishes.length > 0) {
          mealTypeDishes[0].menuDishes = menuDishes;
        }
      }
    });
  }

  addDishBySelection() {
    const dialogRef = this.dialog.open(MenuDishSelectorEditorComponent, {
      data: { header: "Dishes", dishMealTypes: this.dishMealTypes, rows: this.rows, totalCount: this.totalCount },
      width: '800px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result && !result.isCancel) {
        this.menuEdit.mealTypeMenuDishes = [];
        this.mealTypes.forEach((mp: MealType, index, m) => {
          let mealTypeDish = new MealTypeMenuDish();
          mealTypeDish.menuDishes = [];
          mealTypeDish.mealTypeId = mp.id;
          mealTypeDish.mealTypeName = mp.name;
          let cells = [];
          result.rows.map(function (cls) {
            for (let c of cls.cells) {
              let dish = c;
              c.label = cls.dishLabel;
              cells.push(c);
            }
          });
          let dishes = cells.filter(e => e.mealTypeId == mp.id);
          dishes.forEach((d: any, index, dish) => {
            if (!this.menuEdit.menuDishes) this.menuEdit.menuDishes = [];
            if (d.checked) {
              let newMenuDish = new MenuDish();
              newMenuDish.dishId = d.dishId;
              newMenuDish.menuId = this.menuEdit.id;
              newMenuDish.mealTypeId = mp.id;
              newMenuDish.dishName = d.label;
              newMenuDish.checked = true;
              mealTypeDish.menuDishes.push(newMenuDish);
            }
            
          });

          this.menuEdit.mealTypeMenuDishes.push(mealTypeDish);
        },
          error => {
          });
      }   
    });
  }

  get canManageMenus() {
    return true; //this.accountService.userHasPermission(Permission.manageMenusPermission)
  }
}
