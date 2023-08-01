import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { DateAdapter, MatDatepickerInputEvent, MatDialog, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material';
import { AuditService } from 'src/app/services/audit.service';
import { AuditLog } from 'src/app/models/audit-log';
import { DataLogViewerComponent } from './data-log-viewer.component';
import { DateOnlyPipe, DateTimeOnlyPipe } from 'src/app/pipes/datetime.pipe';
import { Subscription } from 'rxjs';
import { SearchBoxComponent } from '../../controls/search-box.component';
import { saveAs } from 'file-saver';
import * as moment from 'moment';

@Component({
  selector: 'data-logs-management',
  templateUrl: './data-logs-management.component.html',
  styleUrls: ['./data-logs-management.component.css']
})
export class DataLogsManagementComponent implements OnInit, OnDestroy {
  private subscription: Subscription = new Subscription();
  columns: any[] = [];
  rows: AuditLog[] = [];
  rowsCache: AuditLog[] = [];
  editingAuditLogName: { name: string };
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';
  start = new Date();
  end = new Date();

  @ViewChild('searchbox') searchbox: SearchBoxComponent;

  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('dataLogTable') table: any;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private auditService: AuditService, public dialog: MatDialog) {
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
      { prop: 'userName', name: 'User Name' },
      { prop: 'logType', name: 'Type' },
      { prop: 'eventDateTime', name: 'Date', pipe: new DateTimeOnlyPipe('en-SG') },
      { prop: 'tableName', name: 'Table' },
      { prop: 'recordId', name: 'Record ID' },
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
    console.log(ev);
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
    this.filter.filters = '(UserName|RecordId|TableName)@=' + this.keyword + ',(AuditDataDateRange)==' + this.start.toDateString() + '|' + this.end.toDateString();

    this.subscription.add(this.auditService.getDataLogsByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

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

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve audit logs from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }

  onChangeDate(type: string, event: MatDatepickerInputEvent<Date>) {
    if (type == 'start') {
      this.start = new Date(event.value);
    }

    if (type == 'end') {
      this.end = new Date(event.value);
    }

    this.loadData();
  }

  onSearchChanged(value: string) {
    //this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.name, r.description));
    this.keyword = value;
    //this.loadData(null);
  }

  onSearch() {
    this.clearFilterAndPagedResult();
    this.loadData(null);
    console.log('onSearch');
  }

  clearFilters() {
    this.start = new Date();
    this.end = new Date();
    this.keyword = '';
    this.clearFilterAndPagedResult();
    this.searchbox.clear();
  }

  viewAuditDetails(row) {
    const dialogRef = this.dialog.open(DataLogViewerComponent, {
      data: { header: "Audit Log Details", id: row.id },
      width: '600px'
    });

    dialogRef.afterClosed().subscribe(result => {
      //this.loadData();
    });
  }

  downloadResults() {
    const fileName = moment().format('DDMMYYYY_hhmmss') + '_DataLogs.xlsx';

    this.auditService.downloadDataLogReport(this.filter).subscribe(
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

  get canManageAuditLogs() {
    return this.accountService.userHasPermission(Permission.viewDataLogsPermission)
  }

}
