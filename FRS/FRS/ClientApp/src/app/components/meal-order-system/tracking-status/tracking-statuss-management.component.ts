import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { TrackingStatus } from 'src/app/models/meal-order/tracking-status.model';
import { TrackingStatusEditorComponent } from './tracking-status-editor.component';
import { DishService } from 'src/app/services/meal-order/dish.service';
import { StaffService } from '../../../services/meal-order/staff.service';
import { DeliveryService } from '../../../services/meal-order/delivery.service';


@Component({
  selector: 'tracking-statuss-management',
  templateUrl: './tracking-statuss-management.component.html',
  styleUrls: ['./tracking-statuss-management.component.css']
})
export class TrackingStatussManagementComponent implements OnInit {
  columns: any[] = [];
  rows: TrackingStatus[] = [];
  rowsCache: TrackingStatus[] = [];
  allPermissions: Permission[] = [];
  editedTrackingStatus: TrackingStatus;
  sourceTrackingStatus: TrackingStatus;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('flagTemplate')
  flagTemplate: TemplateRef<any>;

  @ViewChild('trackingStatusEditor')
  trackingStatusEditor: TrackingStatusEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private deliveryService: DeliveryService, public dialog: MatDialog) {
  }

  openDialog(trackingStatus: TrackingStatus): void {
    const dialogRef = this.dialog.open(TrackingStatusEditorComponent, {
      data: { header: this.header, trackingStatus: trackingStatus },
      width: '400px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData(null);
    });
  }

  initializeFilter() {
    this.filter = new Filter(1, 10);
    this.filter.sorts = 'status';
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
      { prop: 'status', name: 'Status' },
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
    this.filter.filters = '(IsActive)==true,(Status)@=' + this.keyword + ',(InstitutionId)==' + this.accountService.currentUser.institutionId;
    
    this.deliveryService.getTrackingStatussByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let trackingStatuss = results.pagedData;

        trackingStatuss.forEach((trackingStatus, index, trackingStatuss) => {
          (<any>trackingStatus).index = index + 1;
        });


        this.rowsCache = [...trackingStatuss];
        this.rows = trackingStatuss;

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

  newTrackingStatus() {
    this.header = 'New Tracking status';
    this.editedTrackingStatus = new TrackingStatus();
    this.openDialog(this.editedTrackingStatus);
  }


  editTrackingStatus(row: TrackingStatus) {
    this.editedTrackingStatus = row;
    this.header = 'Edit Tracking status';
    this.openDialog(this.editedTrackingStatus);
  }

  deleteTrackingStatus(row: TrackingStatus) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.status + '\" Tracking status?', DialogType.confirm, () => this.deleteTrackingStatusHelper(row));
  }


  deleteTrackingStatusHelper(row: TrackingStatus) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.deliveryService.deleteTrackingStatus(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the Tracking status.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  get canManageTrackingStatuss() {
    return true; //this.accountService.userHasPermission(Permission.manageTrackingStatussPermission)
  }

}
