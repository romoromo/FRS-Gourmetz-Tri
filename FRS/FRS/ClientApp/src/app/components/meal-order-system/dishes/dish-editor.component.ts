import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Dish, DishComponent, DishDetail, DishPeriod, DishRestriction } from 'src/app/models/meal-order/dish.model';
import { DishService } from 'src/app/services/meal-order/dish.service';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { Filter } from 'src/app/models/sieve-filter.model';
import { MealPeriod } from 'src/app/models/meal-order/meal-period.model';
import { DishType } from 'src/app/models/meal-order/dish-type.model';
import { RestrictionService } from 'src/app/services/meal-order/restriction.service';
import { Restriction } from 'src/app/models/meal-order/restriction.model';
import { FileService } from 'src/app/services/file.service';
import { DeliveryService } from 'src/app/services/meal-order/delivery.service';
import { DishSelectorComponent } from './dish-selector/dish-selector.component';


@Component({
  selector: 'dish-editor',
  templateUrl: './dish-editor.component.html',
  styleUrls: ['./dish-editor.component.css']
})
export class DishEditorComponent {

  private isNewDish = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingDishCode: string;
  private dishEdit: Dish = new Dish();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;
  private periods: MealPeriod[] = [];
  private restrictions = [];
  private dishTypes: DishType[] = [];
  private bentoBoxTypes = [];
  private storeInfos = [];
  private cuisines = [];
  private subdishes: Dish[] = [];
  public fileUploadResponse: { dbPath: '', fileId: null, fileName: '' };
  public catererId: string;
  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;
    

  constructor(private alertService: AlertService, private dishService: DishService, private accountService: AccountService,
    public dialogRef: MatDialogRef<DishEditorComponent>, private fileService: FileService, private mealService: MealService, private restrictionService: RestrictionService,
    private deliveryService: DeliveryService, public dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.dish) != typeof (undefined)) {
      this.catererId = data.catererId;
      if (data.dish.id) {
        this.editDish(data.dish);
      } else {
        this.newDish();
      }
    }

    this.getMealPeriods();
    this.getDishTypes();
    this.getRestrictions();
    this.getBentoBoxTypes();
    this.getCuisines();
    this.getStoreInfos();
  }


  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");

    if (this.dishEdit.dishComponents && this.dishEdit.dishComponents.length > 0) {
      this.dishEdit.dishComponents = this.dishEdit.dishComponents.filter(f => !f.editMode);
    }

    let selectedPeriods = this.periods.filter(f => f.checked);
    this.dishEdit.dishPeriods = [];
    this.periods.forEach((p, index, ps) => {
      if (p.checked) {
        let dtPeriod = new DishPeriod();
        dtPeriod.dishId = this.dishEdit.id;
        dtPeriod.periodId = p.id;
        this.dishEdit.dishPeriods.push(dtPeriod);
      }
      
    });

  

    let selectedRestrictions = this.restrictions.filter(f => f.checked);
    this.dishEdit.restrictions = [];
    this.restrictions.forEach((p, index, ps) => {
      if (p.checked) {
        let sr = new DishRestriction();
        sr.dishId = this.dishEdit.id;
        sr.restrictionId = p.id;
        this.dishEdit.restrictions.push(sr);
      }

    });

    if (this.isNewDish) {
      this.dishService.newDish(this.dishEdit).subscribe(dish => this.saveSuccessHelper(dish), error => this.saveFailedHelper(error));
    }
    else {
      this.dishService.updateDish(this.dishEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }


  private saveSuccessHelper(dish?: Dish) {
    if (dish)
      Object.assign(this.dishEdit, dish);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewDish)
      this.alertService.showMessage("Success", `Dish \"${this.dishEdit.label}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to dish \"${this.dishEdit.label}\" was saved successfully`, MessageSeverity.success);


    this.dishEdit = new Dish();
    this.resetForm();


    //if (!this.isNewDish && this.accountService.currentUser.facilities.some(r => r == this.editingDishCode))
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
    this.dishEdit = new Dish();

    this.showValidationErrors = false;
    this.resetForm();

    this.alertService.resetStickyMessage();

    if (this.changesCancelledCallback)
      this.changesCancelledCallback();

    this.dialogRef.close(true);
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


  newDish() {
    this.isNewDish = true;
    this.showValidationErrors = true;

    this.editingDishCode = null;
    this.selectedValues = {};
    this.dishEdit = new Dish();
    this.dishEdit.isEnabled = true;
    this.dishEdit.catererId = this.catererId;

    if (!this.dishEdit.code) {
      this.getDishCode();
    }
    return this.dishEdit;
  }

  editDish(dish: Dish) {
    if (dish) {
      this.isNewDish = false;
      this.showValidationErrors = true;

      this.editingDishCode = dish.label;
      this.selectedValues = {};
      this.dishEdit = new Dish();
      Object.assign(this.dishEdit, dish);


      if (!this.dishEdit.code) {
        this.getDishCode();
      }

      return this.dishEdit;
    }
    else {
      return this.newDish();
    }
  }

  getDishCode() {
    this.dishService.generateDishCode(this.catererId)
      .subscribe(results => {
        console.log(results);
        this.dishEdit.code = results.code;
      },
        error => {
        })
  }

  getMealPeriods() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.mealService.getMealPeriodsByFilter(filter)
      .subscribe(results => {
        this.periods = results.pagedData;
        this.periods.forEach((p, index, ps) => {
          (<any>p).checked = this.dishEdit.dishPeriods != null && this.dishEdit.dishPeriods.findIndex(f=> f.periodId == p.id) > -1;
        });
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving cperiods.\r\n"`,
            MessageSeverity.error);
        })
  }

  getRestrictions() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.restrictionService.getRestrictionsByFilter(filter)
      .subscribe(results => {
        this.restrictions = results.pagedData;
        this.restrictions.forEach((p, index, ps) => {
          (<any>p).checked = this.dishEdit.restrictions != null && this.dishEdit.restrictions.findIndex(f => f.restrictionId == p.id) > -1;
        });
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving restrictions.\r\n"`,
            MessageSeverity.error);
        })
  }

  getDishTypes() {
    let filter = new Filter();
    let f = this.catererId ? '(CatererId)==' + this.catererId + ',' : '';
    filter.filters = f + '(IsActive)==true';
    this.dishService.getDishTypesByFilter(filter)
      .subscribe(results => {
        this.dishTypes = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving dish types.\r\n"`,
            MessageSeverity.error);
        })
  }

  getBentoBoxTypes() {
    let filter = new Filter();
    let f = this.catererId ? '(CatererInfoId)==' + this.catererId + ',' : '';
    filter.filters = f + '(IsActive)==true';
    this.deliveryService.getBentoBoxTypesByFilter(filter)
      .subscribe(results => {
        this.bentoBoxTypes = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving bento box types.\r\n"`,
            MessageSeverity.error);
        })
  }

  getStoreInfos() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true,(storeType)@=Kitchen';
    this.deliveryService.getStoreInfosByFilter(filter)
      .subscribe(results => {
        this.storeInfos = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving bento box types.\r\n"`,
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

  public uploadFinished = (event) => {
    this.fileUploadResponse = event;
    this.dishEdit.filePath = this.fileUploadResponse ? this.fileUploadResponse.dbPath : null;
  }

  public productionPictureUploadFinished = (event) => {
    this.fileUploadResponse = event;
    this.dishEdit.productionPicturePath = this.fileUploadResponse ? this.fileUploadResponse.dbPath : null;
  }

  getFileImage(path) {
    return this.fileService.getFile(path);
  }

  removePhoto() {
    this.dishEdit.filePath = null;
    this.dishEdit.fileId = null;
    this.dishEdit.fileName = null;
  }

  removeProductionPhoto() {
    this.dishEdit.productionPicturePath = null;
    this.dishEdit.productionPictureId = null;
    this.dishEdit.productionPictureName = null;
  }

  addSubdish() {
    const dialogRef = this.dialog.open(DishSelectorComponent, {
      data: { header: "Sub Dishes", dishes: this.dishEdit.subDishes, catererId: this.catererId },
      width: '500px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result.isCancel) {
        this.dishEdit.subDishes = [];
        result.selectedDishes.forEach((p, index, ps) => {
          if (p.checked) {
            let dt = new DishDetail();
            dt.parentDishId = this.dishEdit.id;
            dt.dishId = p.id;
            dt.code = p.code;
            dt.label = p.label;
            dt.cookedWeight = p.cookedWeight;
            this.dishEdit.subDishes.push(dt);
          }

        });
      }
    });
  }

  removeSubdish(dish) {
    if (dish.dishId) {
      this.dishEdit.subDishes = this.dishEdit.subDishes.filter(item => item.dishId !== dish.dishId);
    }
  }

  saveDishComponent(item: DishComponent) {
    if (item && item.category && item.component
      && item.quantity && item.sapProductCode && item.weight) {
      item.editMode = false;
    }
    else {
      alert('Please enter required values for the component.');
    }
  }

  addComponent(compo) {
    let dc = new DishComponent();
    dc.dishId = this.dishEdit.id;
    dc.editMode = true;
    this.dishEdit.dishComponents.push(dc);
  }
  removeComponent(compo) {
    const indx = this.dishEdit.dishComponents.indexOf(compo);
    if (indx > -1)
      this.dishEdit.dishComponents.splice(indx, 1);
  }

  get canManageDishes() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtCatererDishesMenu)
  }
}
