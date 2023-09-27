import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult, UserActivityLogReportFilter } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDatepickerInputEvent, MatDialog } from '@angular/material';
import { AuditService } from 'src/app/services/audit.service';
import { DateTimeOnlyPipe } from 'src/app/pipes/datetime.pipe';
import { Subscription } from 'rxjs';
import { SearchBoxComponent } from '../../controls/search-box.component';
import { saveAs } from 'file-saver';
import * as moment from 'moment';
import { UserActivityLog } from 'src/app/models/audit-log';
import { UserActivityLogType } from 'src/app/models/enums';
import { UserActivityLogViewerComponent } from './user-activity-log-viewer.component';


@Component({
  selector: 'user-activity-logs-management',
  templateUrl: './user-activity-logs-management.component.html',
  styleUrls: ['./user-activity-logs-management.component.css']
})
export class UserActivityLogsManagementComponent implements OnInit, OnDestroy {
  private subscription: Subscription = new Subscription();
  columns: any[] = [];
  rows: UserActivityLog[] = [];
  rowsCache: UserActivityLog[] = [];
  reportTypes: any[] = [UserActivityLogType.Order, UserActivityLogType.Payment, UserActivityLogType.Roster];
  editingAuthLogName: { name: string };
  loadingIndicator: boolean;
  filter: UserActivityLogReportFilter;
  pagedResult: PagedResult;
  keyword: string = '';
  reportType: string = '';
  start = new Date();
  end = new Date();

  @ViewChild('searchbox') searchbox: SearchBoxComponent;

  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('authLogTable') table: any;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private authLogService: AuditService, public dialog: MatDialog) {
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  initializeFilter() {
    this.filter = new UserActivityLogReportFilter(1, 10);
    this.filter.sorts = '-username';
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
      { prop: 'groupId', name: 'Transaction', sortable: false  },
      { prop: 'eventDateTime', name: 'Date', pipe: new DateTimeOnlyPipe('en-SG'), sortable: false  },
      { prop: 'username', name: 'User Name', sortable: false  },
      { prop: 'remarks', name: 'Description', sortable: false },
      { name: '', width: 150, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
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
    //this.filter.filters = '(userName)@=' + this.keyword + ',(reportType)==' + (this.reportType == 'All' ? '' : this.reportType) + ',(UserActivityLogDateRange)==' + this.start.toDateString() + '|' + this.end.toDateString();

    this.filter.reportDateFrom = this.start.toDateString();
    this.filter.reportDateTo = this.end.toDateString();
    this.filter.reportType = (this.reportType == 'All' ? '' : this.reportType);
    this.filter.keyword = this.keyword;
    this.authLogService.getUserActivityLogsByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;
        console.log(this.pagedResult);
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let authLogs = results.pagedData;

        authLogs.forEach((authLog, index, authLogs) => {
          (<any>authLog).index = index + 1;
        });


        this.rowsCache = [...authLogs];
        this.rows = authLogs;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve logs from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  onChangeDate(type: string, event: MatDatepickerInputEvent<Date>) {
    if (type == 'start') {
      this.start = new Date(event.value);
    }

    if (type == 'end') {
      this.end = new Date(event.value);
    }

    //this.loadData();
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

  clearFilters() {
    this.start = new Date();
    this.end = new Date();
    this.keyword = '';
    this.clearFilterAndPagedResult();
    this.searchbox.clear();
  }

  viewAuditDetails(row) {
    const dialogRef = this.dialog.open(UserActivityLogViewerComponent, {
      data: { header: "Activity Log Details", data: row },
      width: '800px'
    });

    dialogRef.afterClosed().subscribe(result => {
      //this.loadData();
    });
  }

  downloadResults() {
    const fileName = moment().format('DDMMYYYY_hhmmss') + '_UserActivityLogs.xlsx';
    this.filter.page = null;
    this.filter.pageSize = null;
    this.authLogService.downloadFlattenUserActivityLogsReport(this.filter).subscribe(
      data => {
        console.log(data);
        saveAs(data, fileName);
      },
      err => {
        alert("Problem while downloading the file.");
        console.error(err);
      }
    );
  }

  get canManageAuthLogs() {
    return true;//this.accountService.userHasPermission(Permission.viewAuthLogsPermission)
  }

}
