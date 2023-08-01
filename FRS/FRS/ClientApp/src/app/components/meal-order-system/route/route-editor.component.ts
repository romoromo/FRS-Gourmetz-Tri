import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Route, RouteNode } from 'src/app/models/meal-order/route.model';
import { DeliveryService } from 'src/app/services/meal-order/delivery.service';
import { MAT_MOMENT_DATE_FORMATS } from '@angular/material-moment-adapter';
import { MomentUtcDateAdapter } from 'src/app/helpers/moment-utc-adapter';
import { Filter } from 'src/app/models/sieve-filter.model';
import { DateAdapter, MatDatepickerInputEvent, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material';
import { StoreInfo } from '../../../models/meal-order/store-info.model';

import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';


@Component({
  selector: 'route-editor',
  templateUrl: './route-editor.component.html',
  styleUrls: ['./route-editor.component.css']
})
export class RouteEditorComponent {

  private isNewRoute = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingRouteLabel: string;
  private routeEdit: Route = new Route();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;
  public nodes = [];
  public selectedNodes = [];
  public selectedNode;

  start = new Date();

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private deliveryService: DeliveryService, private accountService: AccountService,
    public dialogRef: MatDialogRef<RouteEditorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.route) != typeof (undefined)) {
      if (data.route.id) {
        this.editRoute(data.route);
      } else {
        this.newRoute();
      }
    }
  }


  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }

  getStores() {
    console.log("get stores")
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.deliveryService.getStoreInfosByFilter(filter)
      .subscribe(results => {
        console.log("stores: ", results)
        this.nodes = results.pagedData;
        if (this.routeEdit.nodes != null) {
          this.routeEdit.nodes.forEach((rn, indexr) => {
            this.nodes.forEach((n, index) => {
              if (rn.storeId == n.id) {
                n.order = rn.order;
                n.interval = rn.interval;
                this.selectedNodes.push(n)
              }
            });
          });
        }
        console.log("selected Nodes", this.selectedNodes)
        this.selectedNodes.sort((a, b) => a.order - b.order)
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving interest groups.\r\n"`,
            MessageSeverity.error);
        })
  }


  addNode() {
    console.log("selected node: ", this.selectedNode)
    this.selectedNodes.push(this.selectedNode)
  }

  drop(event: CdkDragDrop<{ id: number, title: string, url: string }[]>) {
    moveItemInArray(this.selectedNodes, event.previousIndex, event.currentIndex);
    console.log("urutan: " + this.selectedNodes)
  }

  removeNode(node) {
    var index = this.selectedNodes.indexOf(node);
    this.selectedNodes.splice(index, 1);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    this.routeEdit.institutionId = this.accountService.currentUser.institutionId;
    console.log("route save:", this.routeEdit)

    this.routeEdit.nodes = [];
    this.selectedNodes.forEach((p, index, ps) => {
      let sr = new RouteNode();
      sr.routeId = this.routeEdit.id;
      sr.storeId = p.id;
      sr.order = index;
      sr.interval = p.interval;
      this.routeEdit.nodes.push(sr);

    });


    if (this.isNewRoute) {
      this.deliveryService.newRoute(this.routeEdit).subscribe(route => this.saveSuccessHelper(route), error => this.saveFailedHelper(error));
    }
    else {
      this.deliveryService.updateRoute(this.routeEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }


  private saveSuccessHelper(route?: Route) {
    if (route)
      Object.assign(this.routeEdit, route);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewRoute)
      this.alertService.showMessage("Success", `Route \"${this.routeEdit.label}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to route type \"${this.routeEdit.label}\" was saved successfully`, MessageSeverity.success);


    this.routeEdit = new Route();
    this.resetForm();


    //if (!this.isNewMealType && this.accountService.currentUser.facilities.some(r => r == this.editingMealTypeCode))
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
    this.routeEdit = new Route();

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


  newRoute() {
    this.isNewRoute = true;
    this.showValidationErrors = true;

    this.editingRouteLabel = null;
    this.selectedValues = {};
    this.start = new Date();
    this.routeEdit = new Route();

    this.getStores();

    return this.routeEdit;
  }

  editRoute(route: Route) {
    if (route) {
      console.log("route edit:", route)
      this.isNewRoute = false;
      this.showValidationErrors = true;

      this.editingRouteLabel = route.label;
      this.selectedValues = {};
      this.routeEdit = new Route();
      Object.assign(this.routeEdit, route);

      console.log('start: ', this.start)

      this.getStores();

      return this.routeEdit;
    }
    else {
      return this.newRoute();
    }
  }

  get canManageRoutes() {
    return true; //this.accountService.userHasPermission(Permission.manageMealTypesPermission)
  }
}
