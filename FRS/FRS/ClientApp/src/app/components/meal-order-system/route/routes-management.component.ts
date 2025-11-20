import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, PipeTransform } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { Route } from 'src/app/models/meal-order/route.model';
import { RouteEditorComponent } from './route-editor.component';
import { DeliveryService } from 'src/app/services/meal-order/delivery.service';
import * as moment from 'moment';


@Component({
  selector: 'routes-management',
  templateUrl: './routes-management.component.html',
  styleUrls: ['./routes-management.component.css']
})
export class RoutesManagementComponent implements OnInit {
  columns: any[] = [];
  rows: Route[] = [];
  rowsCache: Route[] = [];
  allPermissions: Permission[] = [];
  editedRoute: Route;
  sourceRoute: Route;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('flagTemplate')
  flagTemplate: TemplateRef<any>;

  @ViewChild('routeEditor')
  routeEditor: RouteEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private deliveryService: DeliveryService, public dialog: MatDialog) {
  }

  openDialog(route: Route): void {
    const dialogRef = this.dialog.open(RouteEditorComponent, {
      data: { header: this.header, route: route },
      width: '500px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData(null);
    });
  }

  initializeFilter() {
    this.filter = new Filter(1, 10);
    this.filter.sorts = 'label';
    this.filter.filters = '';
    this.filter.page = 1;
  }

  initializePagedResult() {
    this.pagedResult = new PagedResult();
    this.pagedResult.totalCount = 0;
    this.pagedResult.pagedData = [];
    this.pagedResult.filter = this.filter;
  }

  initializeTableDefinition() {
    let gT = (key: string) => this.translationService.getTranslation(key);

    this.columns = [
      { prop: 'label', name: 'Label' },
      { prop: 'details', name: 'Route Details' },
      { prop: 'pickup', name: 'Pickup Time', pipe: this.pipeTime() },
      { name: '', width: 150, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
    ];

    if (!this.accountService.currentUser.institutionId || this.accountService.currentUser.institutionId == '0') {
      this.columns.splice(1, 0, { prop: 'institutionName', name: gT('roles.management.Institution'), width: 120 });
    }
  }

  ngOnInit() {
    this.initializeFilter();
    this.initializePagedResult();
    this.initializeTableDefinition();
    this.loadData();
  }


  loadData(ev?: any) {
    this.alertService.startLoadingMessage();
    this.loadingIndicator = true;
    this.filter.pageSize = 10;

    if (ev) {
      this.filter.page = ev.offset + 1;
      if (ev.sorts) {
        this.filter.sorts = ev.sorts[0].dir == 'desc' ? '-' + ev.sorts[0].prop : ev.sorts[0].prop;
      }
    }

    if (!this.keyword) this.keyword = '';
    this.filter.filters = '(IsActive)==true,(Label)@=' + this.keyword + ',(InstitutionId)==' + this.accountService.currentUser.institutionId;
    
    this.deliveryService.getRoutesByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let routes = results.pagedData;

        routes.forEach((route, index, routes) => {
          (<any>route).index = index + 1;
        });


        this.rowsCache = [...routes];
        this.rows = routes;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve records from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }


  onSearchChanged(value: string) {
    this.keyword = value;
    this.loadData(null);
  }

  newRoute() {
    this.header = 'New Route';
    this.editedRoute = new Route();
    this.openDialog(this.editedRoute);
  }


  editRoute(row: Route) {
    this.editedRoute = row;
    this.header = 'Edit Route';
    this.openDialog(this.editedRoute);
  }

  deleteRoute(row: Route) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.label + '\" route?', DialogType.confirm, () => this.deleteRouteHelper(row));
  }


  deleteRouteHelper(row: Route) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.deliveryService.deleteRoute(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the route type.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  pipeTime(): PipeTransform {
    return {
      transform: (value) => {
        return moment(value).format('HH:mm')
      }
    }
  }

  get canManageRoutes() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtRoutesPermission)
  }

}
