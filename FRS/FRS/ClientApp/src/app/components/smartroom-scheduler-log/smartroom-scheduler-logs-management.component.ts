import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../services/alert.service';
import { AppTranslationService } from "../../services/app-translation.service";
import { AccountService } from '../../services/account.service';
import { Utilities } from '../../services/utilities';
import { Filter, PagedResult } from '../../models/sieve-filter.model';
import { Permission } from '../../models/permission.model';
import { MatDialog } from '@angular/material';
import { AuditService } from 'src/app/services/audit.service';
import { DateTimeOnlyPipe } from 'src/app/pipes/datetime.pipe';
import { Subscription } from 'rxjs';
import { SmartRoomSchedulerLog } from 'src/app/models/smartroom-scheduler-log';
import { SmartRoomSchedulerService } from 'src/app/services/smartroom.service';
import { SmartRoomSchedulerDetailsViewerComponent } from './details-log-viewer.component';


@Component({
  selector: 'smartroom-scheduler-logs-management',
  templateUrl: './smartroom-scheduler-logs-management.component.html',
  styleUrls: ['./smartroom-scheduler-logs-management.component.css']
})
export class SmartRoomSchedulerLogsManagementComponent implements OnInit, OnDestroy {
  private subscription: Subscription = new Subscription();
  columns: any[] = [];
  rows: SmartRoomSchedulerLog[] = [];
  rowsCache: SmartRoomSchedulerLog[] = [];
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('logTable') table: any;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private smartRoomSchedulerService: SmartRoomSchedulerService, public dialog: MatDialog) {
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  initializeFilter() {
    this.filter = new Filter(1, 10);
    this.filter.sorts = '-eventDateTime';
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
      //{ prop: "index", name: '#', width: 50, cellTemplate: this.indexTemplate, canAutoResize: false },
      { prop: 'eventDateTime', name: 'Date', pipe: new DateTimeOnlyPipe('en-SG'), flexGrow: 2 },
      { prop: 'status', name: 'Status', flexGrow: 1 },
      { prop: 'noRecordsAffected', name: 'Total No. Of Records', flexGrow: 1 },
      { prop: 'details', name: 'Details', cellTemplate: this.actionsTemplate, sortable: false, flexGrow: 1 },
    ];
  }

  clearFilterAndPagedResult() {
    this.initializeFilter();
    this.initializePagedResult();
    this.table.offset = 0;
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
    this.filter.filters = '(Status|details)@=' + this.keyword;
    
    this.smartRoomSchedulerService.getLogsByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;
        console.log(this.pagedResult);
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let logs = results.pagedData;

        logs.forEach((log, index, logs) => {
          (<any>log).index = index + 1;
        });


        this.rowsCache = [...logs];
        this.rows = logs;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve logs from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }


  onSearchChanged(value: string) {
    //this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.name, r.description));
    this.keyword = value;
    //this.loadData(null);
  }

  onSearch() {
    this.clearFilterAndPagedResult();
    this.loadData(null);
  }

  viewDetails(row) {
    const dialogRef = this.dialog.open(SmartRoomSchedulerDetailsViewerComponent, {
      data: { header: "Log Details", log: row },
      width: '400px'
    });

    dialogRef.afterClosed().subscribe(result => {
    });
  }

  get canManageSmartRoomSchedulerLogs() {
    return this.accountService.userHasPermission(Permission.viewSmartRoomSchedulerLogsPermission)
  }

}
