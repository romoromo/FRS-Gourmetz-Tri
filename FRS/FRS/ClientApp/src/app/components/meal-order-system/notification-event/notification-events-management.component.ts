import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { NotificationEventEditorComponent } from './notification-event-editor.component';
import { DeliveryService } from '../../../services/meal-order/delivery.service';
import { NotificationEvent } from 'src/app/models/meal-order/notification-event.model';
import { PaymentService } from 'src/app/services/meal-order/payment.service';
import { NotificationService } from 'src/app/services/notification.service';


@Component({
  selector: 'notification-events-management',
  templateUrl: './notification-events-management.component.html',
  styleUrls: ['./notification-events-management.component.css']
})
export class NotificationEventsManagementComponent implements OnInit {
  columns: any[] = [];
  rows: NotificationEvent[] = [];
  rowsCache: NotificationEvent[] = [];
  allPermissions: Permission[] = [];
  editedNotificationEvent: NotificationEvent;
  sourceNotificationEvent: NotificationEvent;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('flagTemplate')
  flagTemplate: TemplateRef<any>;

  @ViewChild('outletEditor')
  outletEditor: NotificationEventEditorComponent;

  @ViewChild('notificationEventsTable') table: any;

  header: string;
  @Input() isHideHeader: boolean;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private notificationService: NotificationService, public dialog: MatDialog) {
  }

  openDialog(notificationEvent: NotificationEvent): void {
    const dialogRef = this.dialog.open(NotificationEventEditorComponent, {
      data: { header: this.header, notificationEvent: notificationEvent },
      width: '400px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData(null);
    });
  }

  initializeFilter() {
    this.filter = new Filter(1, 10);
    this.filter.sorts = 'title';
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
      { prop: 'type', name: 'Type' },
      { prop: 'title', name: 'Title' },
      { prop: 'template', name: 'Template' },
      { name: '', width: 150, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
    ];
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
    this.filter.filters = '(IsActive)==true,(Title)@=' + this.keyword;
    
    this.notificationService.getNotificationEventsByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let notificationEvents = results.pagedData;

        notificationEvents.forEach((notificationEvent, index, notificationEvents) => {
          (<any>notificationEvent).index = index + 1;
        });


        this.rowsCache = [...notificationEvents];
        this.rows = notificationEvents;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve records from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  clearFilterAndPagedResult() {
    this.initializeFilter();
    this.initializePagedResult();
    this.table.offset = 0;
  }

  onSearchChanged(value: string) {
    this.keyword = value;
    this.clearFilterAndPagedResult();
    this.loadData(null);
  }

  newNotificationEvent() {
    this.header = 'New Notification Event';
    this.editedNotificationEvent = new NotificationEvent();
    this.openDialog(this.editedNotificationEvent);
  }


  editNotificationEvent(row: NotificationEvent) {
    this.editedNotificationEvent = row;
    this.header = 'Edit Notification Event';
    this.openDialog(this.editedNotificationEvent);
  }

  deleteNotificationEvent(row: NotificationEvent) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.title + '\" notification event?', DialogType.confirm, () => this.deleteNotificationEventHelper(row));
  }


  deleteNotificationEventHelper(row: NotificationEvent) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.notificationService.deleteNotificationEvent(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the notification event.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  get canManageNotificationEvents() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtNotificationEventsPermission)
  }

}
