import { Component, ViewChild, Inject } from "@angular/core";

import { AlertService, MessageSeverity } from "../../../services/alert.service";
import { AccountService } from "../../../services/account.service";
import { Permission } from "../../../models/permission.model";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material";
import { MealService } from "src/app/services/meal-order/meal.service";
import { Filter } from "src/app/models/sieve-filter.model";
import { DishService } from "../../../services/meal-order/dish.service";
import { FileService } from "src/app/services/file.service";
import { SortingArea } from "src/app/models/meal-order/sorting-area.model";
import { DeliveryService } from "src/app/services/meal-order/delivery.service";
import { Subscription } from "rxjs";
import { Route } from "src/app/models/meal-order/route.model";

@Component({
  selector: "sorting-area-editor",
  templateUrl: "./sorting-area-editor.component.html",
  styleUrls: ["./sorting-area-editor.component.css"],
})
export class SortingAreaEditorComponent {
  private subscription: Subscription = new Subscription();
  private routes: Route[] = [];
  private isNewSortingArea = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingSortingAreaCode: string;
  private sortingAreaEdit: SortingArea = new SortingArea();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean } = {};
  public formResetToggle = true;
  public fileUploadResponse: { dbPath: ""; fileId: null; fileName: "" };

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;

  @ViewChild("f")
  private form;

  private catererInfoId: string;

  constructor(
    private alertService: AlertService,
    private deliveryService: DeliveryService,
    private accountService: AccountService,
    public dialogRef: MatDialogRef<SortingAreaEditorComponent>,
    private mealService: MealService,
    private fileService: FileService,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    if (typeof data.SortingArea != typeof undefined) {
      if (data.SortingArea.catererId)
        this.catererInfoId = data.SortingArea.catererId;

      if (data.SortingArea.id) {
        this.editSortingArea(data.SortingArea);
      } else {
        this.newSortingArea();
      }
    }

    this.getRoutes();
  }

  getRoutes() {
    let filter = new Filter();
    filter.filters = "(IsActive)==true";
    this.subscription.add(
      this.deliveryService.getRoutesByFilter(filter).subscribe(
        (results) => {
          this.routes = results.pagedData;
        },
        (error) => {
          this.alertService.showStickyMessage(
            "Get Error",
            `An error occured while retrieving caterer types.\r\n"`,
            MessageSeverity.error
          );
        }
      )
    );
  }

  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }

  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");

    if (this.isNewSortingArea) {
      this.deliveryService.newSortingArea(this.sortingAreaEdit).subscribe(
        (sortingArea) => this.saveSuccessHelper(sortingArea),
        (error) => this.saveFailedHelper(error)
      );
    } else {
      this.deliveryService.updateSortingArea(this.sortingAreaEdit).subscribe(
        (response) => this.saveSuccessHelper(),
        (error) => this.saveFailedHelper(error)
      );
    }
  }

  private saveSuccessHelper(sortingArea?: SortingArea) {
    if (sortingArea) Object.assign(this.sortingAreaEdit, sortingArea);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewSortingArea)
      this.alertService.showMessage(
        "Success",
        `\"${this.sortingAreaEdit.code}\" was created successfully`,
        MessageSeverity.success
      );
    else
      this.alertService.showMessage(
        "Success",
        `Changes to \"${this.sortingAreaEdit.code}\" was saved successfully`,
        MessageSeverity.success
      );

    this.sortingAreaEdit = new SortingArea();
    this.resetForm();

    if (this.changesSavedCallback) this.changesSavedCallback();

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

    if (this.changesFailedCallback) this.changesFailedCallback();
  }

  private cancel() {
    this.sortingAreaEdit = new SortingArea();

    this.showValidationErrors = false;
    this.resetForm();

    this.alertService.resetStickyMessage();

    if (this.changesCancelledCallback) this.changesCancelledCallback();

    this.dialogRef.close();
  }

  resetForm(replace = false) {
    if (!replace) {
      this.form.reset();
    } else {
      this.formResetToggle = false;

      setTimeout(() => {
        this.formResetToggle = true;
      });
    }
  }

  newSortingArea() {
    this.isNewSortingArea = true;
    this.showValidationErrors = true;

    this.editingSortingAreaCode = null;
    this.selectedValues = {};
    this.sortingAreaEdit = new SortingArea();
    this.sortingAreaEdit.catererId = this.catererInfoId;
    return this.sortingAreaEdit;
  }

  editSortingArea(sortingArea: SortingArea) {
    if (sortingArea) {
      this.isNewSortingArea = false;
      this.showValidationErrors = true;

      this.editingSortingAreaCode = sortingArea.code;
      this.selectedValues = {};
      this.sortingAreaEdit = new SortingArea();
      Object.assign(this.sortingAreaEdit, sortingArea);
      this.sortingAreaEdit.catererId = this.catererInfoId;
      return this.sortingAreaEdit;
    } else {
      return this.newSortingArea();
    }
  }

  onRouteSelected(routeId: any): void {
    const selectedRoute = this.routes.find((route) => route.id === routeId);

    if (selectedRoute) {
      this.sortingAreaEdit.routeColor = selectedRoute.color;
      this.sortingAreaEdit.routeDetail = selectedRoute.details;
    } else {
      this.sortingAreaEdit.routeColor = null;
      this.sortingAreaEdit.routeDetail = null;
    }
  }

  get canManageSortingAreas() {
    return true; //this.accountService.userHasPermission(Permission.manageSortingAreasPermission)
  }
}
