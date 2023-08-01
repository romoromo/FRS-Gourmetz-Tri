import { Component, ViewChild, Inject, OnInit, OnDestroy, ViewEncapsulation } from '@angular/core';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { DateAdapter, MatDatepickerInputEvent, MatDialog, MatDialogRef, MAT_DATE_FORMATS, MAT_DATE_LOCALE, MAT_DIALOG_DATA } from '@angular/material';
import { MenuService } from 'src/app/services/meal-order/menu.service';
import { CommonFilter, Filter } from 'src/app/models/sieve-filter.model';
import { ClassService } from 'src/app/services/meal-order/class.service';
import { ClassLevel } from 'src/app/models/meal-order/class-level.model';
import { Subscription } from 'rxjs';
import { UserService } from 'src/app/services/meal-order/user.service';
import { UserCardIdEditorComponent } from '../../controls/usercardid/usercardid-editor.component';
import { UserCardId } from 'src/app/models/usercardid.model';
import { RestrictionService } from 'src/app/services/meal-order/restriction.service';
import { DeliveryService } from 'src/app/services/meal-order/delivery.service';
import { FormControl } from '@angular/forms';
import { MenuGroup, MenuGroupClass, MenuGroupDishCycle } from 'src/app/models/meal-order/menu-group.model';
import { DishService } from 'src/app/services/meal-order/dish.service';
import { MAT_MOMENT_DATE_FORMATS } from '@angular/material-moment-adapter';
import { MomentUtcDateAdapter } from 'src/app/helpers/moment-utc-adapter';
import { DishCycle } from 'src/app/models/meal-order/dish-cycle';
import { Class } from 'src/app/models/meal-order/class.model';
//import { MenuSelectorComponent } from './menu-selector/menu-selector.component'


@Component({
  selector: 'menu-group-editor',
  templateUrl: './menu-group-editor.component.html',
  styleUrls: ['./menu-group-editor.component.css'],
  encapsulation: ViewEncapsulation.None,
  providers: [
    { provide: MAT_DATE_LOCALE, useValue: 'en-SG' },
    { provide: MAT_DATE_FORMATS, useValue: MAT_MOMENT_DATE_FORMATS },
    { provide: DateAdapter, useClass: MomentUtcDateAdapter },
  ]
})
export class MenuGroupEditorComponent implements OnInit, OnDestroy{
  private subscription: Subscription = new Subscription();
  private isNewGroup = false;
  private isChangePassword = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingMenuName: string;
  private menuGroupEdit: MenuGroup = new MenuGroup();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;
  private validation = { name: false, gender: false, classLevel: false, class: false, fas: false };
  private isAddAccount = false;
  start = new Date();
  end = new Date();
  private outletId;
  validDishCycle = false;
  public dishCycles: DishCycle[] = [];
  public selectedDishCycles: DishCycle[] = [];

  public classes: Class[] = [];
  public selectedClasses: Class[] = [];

  public searchForm: FormControl = new FormControl();
  groupId: any;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private menuService: MenuService, private dishService: DishService, private accountService: AccountService, private classService: ClassService,
    private userService: UserService, private restrictionService: RestrictionService, private deliveryService: DeliveryService,
    public dialogRef: MatDialogRef<MenuGroupEditorComponent>, public dialog: MatDialog, 
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.outletId) != typeof (undefined)) {
      this.outletId = data.outletId;
    }
    if (typeof (data.group) != typeof (undefined) && data.group.id) {
      this.editGroup(data.group);
    } else {
      this.newGroup();
    }

    this.getDishCycles();
    this.getClasses();
  }

  ngOnInit() {
    this.alertService.resetStickyMessage();
  }

  ngOnDestroy() {
    this.alertService.resetStickyMessage();
    this.subscription.unsubscribe();
  }

  getDishCycles() {
    let filter = new Filter();
    let f = this.outletId ? '(InOutletId)==' + this.outletId + ',' : '';
    filter.filters = f + '(IsActive)==true';
    this.dishService.getDishCyclesByFilter(filter)
      .subscribe(results => {
         let allDishCycles = results.pagedData;
        this.dishCycles = [];
        this.selectedDishCycles = [];
        allDishCycles.forEach((d, i) => {
          if (this.menuGroupEdit.menuGroupDishCycles) {
            let indx = this.menuGroupEdit.menuGroupDishCycles.findIndex(e => e.dishCycleId == d.id);
            if (indx > -1) {
              this.selectedDishCycles.push(d);
            } else {
              this.dishCycles.push(d);
            }
          } else {
            this.dishCycles.push(d);
          }
        });
      },
        error => {
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving dish cycles.\r\n"`,
            MessageSeverity.error);
        })
  }

  getClasses() {
    let filter = new Filter();
    filter.filters = this.outletId ? '(classOutletId)==' + this.outletId + ',' : '';
    filter.filters = filter.filters + '(IsActive)==true';
    this.classService.getClassesByFilter(filter)
      .subscribe(results => {
        let allClasses = results.pagedData;
        this.classes = [];
        this.selectedClasses = [];
        allClasses.forEach((d, i) => {
          if (this.menuGroupEdit.classes) {
            let indx = this.menuGroupEdit.classes.findIndex(e => e.classId == d.id);
            if (indx > -1) {
              this.selectedClasses.push(d);
            } else {
              this.classes.push(d);
            }
          } else {
            this.classes.push(d);
          }
        });
      },
        error => {
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving dish cycles.\r\n"`,
            MessageSeverity.error);
        })
  }

  selectDishCycle(dishCycle){
    //remove dishCycle from the left and add to the selected dish cycle
    if (this.dishCycles) {
      let selectedIndex = this.dishCycles.findIndex(e => e.id == dishCycle.id);
      if (!this.selectedDishCycles) this.selectedDishCycles = [];
      this.selectedDishCycles.push(dishCycle);

      this.dishCycles.splice(selectedIndex, 1);
      this.validDishCycle = false;
    }
  }

  removeSelectDishCycle(dishCycle) {
    //remove dishCycle from the left and add to the selected dish cycle
    if (this.selectedDishCycles) {
      let selectedIndex = this.selectedDishCycles.findIndex(e => e.id == dishCycle.id);
      if (!this.dishCycles) this.dishCycles = [];
      this.dishCycles.push(dishCycle);

      this.selectedDishCycles.splice(selectedIndex, 1);
    }
  }

  selectClass(c) {
    //remove class from the left and add to the selected class
    if (this.classes) {
      let selectedIndex = this.classes.findIndex(e => e.id == c.id);
      if (!this.selectedClasses) this.selectedClasses = [];
      this.selectedClasses.push(c);

      this.classes.splice(selectedIndex, 1);
    }
  }

  removeSelectClass(c) {
    //remove class from the left and add to the selected class
    if (this.selectedClasses) {
      let selectedIndex = this.selectedClasses.findIndex(e => e.id == c.id);
      if (!this.classes) this.classes = [];
      this.classes.push(c);

      this.selectedClasses.splice(selectedIndex, 1);
    }
  }

  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    if (!this.selectedDishCycles || this.selectedDishCycles.length < 1 ) {
      this.validDishCycle = true;
      return false;
    }

    this.menuGroupEdit.outletId = this.outletId;
    this.menuGroupEdit.menuGroupDishCycles = [];
    this.menuGroupEdit.startDate = this.start;
    this.menuGroupEdit.endDate = this.end;
    this.selectedDishCycles.forEach((d, i) => {
      let dishCycle = new MenuGroupDishCycle();
      dishCycle.dishCycleId = d.id;
      dishCycle.menuGroupId = this.menuGroupEdit.id;
      this.menuGroupEdit.menuGroupDishCycles.push(dishCycle);
    });

    this.menuGroupEdit.classes = [];
    this.selectedClasses.forEach((d, i) => {
      let c = new MenuGroupClass();
      c.classId = d.id;
      c.menuGroupId = this.menuGroupEdit.id;
      this.menuGroupEdit.classes.push(c);
    });

    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");

    if (this.isNewGroup) {
      this.menuService.newMenuGroup(this.menuGroupEdit).subscribe(group => this.saveSuccessHelper(group), error => this.saveFailedHelper(error));
    }
    else {
      this.menuService.updateMenuGroup(this.menuGroupEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }

  invalidate() {
    return false;
  }

  private saveSuccessHelper(group?: MenuGroup) {
    if (group)
      Object.assign(this.menuGroupEdit, group);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewGroup)
      this.alertService.showMessage("Success", `Menu group \"${this.menuGroupEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to menu group \"${this.menuGroupEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.menuGroupEdit = new MenuGroup();
    this.resetForm();

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
    this.menuGroupEdit = new MenuGroup();

    this.showValidationErrors = false;
    this.resetForm();

    this.alertService.resetStickyMessage();

    if (this.changesCancelledCallback)
      this.changesCancelledCallback();

    this.dialogRef.close();
  }

  resetForm(replace = false) {
    this.isChangePassword = false;
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


  newGroup() {
    this.isNewGroup = true;
    this.editingMenuName = null;
    this.selectedValues = {};
    this.menuGroupEdit = new MenuGroup();
    return this.menuGroupEdit;
  }

  editGroup(group: MenuGroup) {
    if (group) {
      this.isNewGroup = false;
      this.showValidationErrors = true;

      this.selectedValues = {};
      this.menuGroupEdit = new MenuGroup();
      Object.assign(this.menuGroupEdit, group);
      this.start = this.menuGroupEdit.startDate;
      this.end = this.menuGroupEdit.endDate;
      return this.menuGroupEdit;
    }
    else {
      return this.newGroup();
    }
  }

  onChangeDate(type: string, event: MatDatepickerInputEvent<Date>) {
    if (type == 'start') {
      this.start = new Date(event.value);
      this.menuGroupEdit.startDate = new Date(event.value);
    }
    if (type == 'end') {
      this.end = new Date(event.value);
      this.menuGroupEdit.endDate = new Date(event.value);
    }
  }

  //addMenuSel() {
  //  const dialogRef = this.dialog.open(MenuSelectorComponent, {
  //    data: { header: "Menus", outletId: this.outletId, dishCycles: this.menuGroupEdit.sgdetails },
  //    width: '800px'
  //  });

  //  dialogRef.afterClosed().subscribe(result => {
  //    if (!result.isCancel) {
  //      console.log("early selstore:", this.menuGroupEdit.sgdetails);
  //      console.log("change selstore:", result.selectedMenus);
  //      this.menuGroupEdit.sgdetails = [];
  //      result.selectedMenus.forEach(menu => {
  //        var detail = new MenuGroupDetail();
  //        detail.menuId = menu.id;
  //        detail.menuGroupId = this.menuGroupEdit.id;
  //        this.menuGroupEdit.sgdetails.push(detail);
  //      })
  //      console.log("final sgdetails: ", this.menuGroupEdit)
  //    }
  //  });
  //}

  //getName(id) {
  //  let menu = this.dishCycles.find(x => x.id === id);
  //  if (menu) {
  //    return menu.name
  //  } else {
  //    return '';
  //  }
  //}

  get canManageMenuGroups() {
    return true; //this.accountService.userHasPermission(Permission.manageMenusPermission)
  }
}
