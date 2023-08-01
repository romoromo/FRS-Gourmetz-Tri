import { Component, ViewChild, Inject, AfterViewInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';

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
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';
import { AccountService } from 'src/app/services/account.service';
import { Permission } from 'src/app/models/permission.model';
import { DatatableComponent } from '@swimlane/ngx-datatable';

@Component({
  selector: 'menu-dish-editor',
  templateUrl: './dish-selector-editor.component.html',
  styleUrls: ['./dish-selector-editor.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    { provide: MAT_DATE_LOCALE, useValue: 'en-SG' },
    { provide: MAT_DATE_FORMATS, useValue: MAT_MOMENT_DATE_FORMATS },
    { provide: DateAdapter, useClass: MomentUtcDateAdapter },
  ]
})
export class MenuDishSelectorEditorComponent implements AfterViewInit{

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
  public rows: any[] = [];
  @ViewChild('f')
  private form;

  @ViewChild('ngxDatatable') ngxDatatable: DatatableComponent;

  constructor(private alertService: AlertService, private menuService: MenuService, private accountService: AccountService,
    public dialogRef: MatDialogRef<MenuDishSelectorEditorComponent>, private dishService: DishService, private mealService: MealService,
    @Inject(MAT_DIALOG_DATA) public data: any, public dialog: MatDialog, private cdr: ChangeDetectorRef,
    private dateAdapter: DateAdapter<Date>) {
    this.dateAdapter.setLocale('en-SG');
    if (typeof (data) != typeof (undefined)) {
      this.dishMealTypes = data.dishMealTypes;
      this.rows = data.rows;
      this.totalCount = data.totalCount;
      setTimeout(() => {
        this.rows == [...this.rows];
      }, 500);
      this.rows == [...this.rows];

    }
  }
    ngAfterViewInit(): void {
      let r = this.rows;
      this.rows = [];
      setTimeout(() => {
        this.rows = r;
        this.rows == [...this.rows];
        this.ngxDatatable.element.click();
        this.cdr.detectChanges();
      }, 500);
      this.rows == [...this.rows];
    }

  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.dialogRef.close({ isCancel: false, rows: this.rows });
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

  get canManageMenus() {
    return true; //this.accountService.userHasPermission(Permission.manageMenusPermission)
  }
}
