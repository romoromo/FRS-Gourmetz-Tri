import { Component, ViewChild, Input, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { UserActivityLog } from 'src/app/models/audit-log';
import { Filter, PagedResult } from 'src/app/models/sieve-filter.model';
import { DateTimeOnlyPipe } from 'src/app/pipes/datetime.pipe';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';
import { AppTranslationService } from 'src/app/services/app-translation.service';
import { AuditService } from 'src/app/services/audit.service';
import { Utilities } from 'src/app/services/utilities';

@Component({
  selector: 'user-activity-log-viewer',
  templateUrl: './user-activity-log-viewer.component.html',
  styleUrls: ['./user-activity-log-viewer.component.css']
})
export class UserActivityLogViewerComponent implements OnInit{

  private id: string;
  private details: UserActivityLog[];
  columns: any[] = [];
  rows: UserActivityLog[] = [];
  rowsCache: UserActivityLog[] = [];
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @ViewChild('dataLogDetailsTable') table: any;

  constructor(private auditService: AuditService, private translationService: AppTranslationService,
    private alertService: AlertService, 
    public dialogRef: MatDialogRef<UserActivityLogViewerComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.data) != typeof (undefined)) {
      console.log(data.data);
      this.initializeFilter();
      this.initializePagedResult();
      this.initializeTableDefinition();
      this.id = data.data.groupId;

      this.rowsCache = [...data.data.details];
      this.rows = data.data.details;
      this.rows = [...this.rows];
      //this.loadData();
    }
  }

  ngOnInit() {
    this.initializeTableDefinition();
  }

  initializeFilter() {
    this.filter = new Filter(1, 10);
    this.filter.sorts = 'propertyName';
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
      { prop: 'eventDateTime', name: 'Date', pipe: new DateTimeOnlyPipe('en-SG'), sortable: false, width: 200 },
      //{ prop: 'logType', name: 'Type', width: 100 },
      { prop: 'detailRemarks', name: 'Remarks', width: 300 },
      //{ prop: 'propertyName', name: 'Field' },nope
      //{ prop: 'originalValue', name: 'OriginalValue' },
      //{ prop: 'newValue', name: 'New Value' },
    ];
  }

  clearFilterAndPagedResult() {
    this.initializeFilter();
    this.initializePagedResult();
    this.table.offset = 0;
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
    this.filter.filters = '(PropertyName|OriginalValue|NewValue)@=' + this.keyword + ',(AuditLogId)==' + this.id;

    this.auditService.getDataLogDetailsByFilter(this.filter)
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
        this.rows = [...this.rows];
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve audit log details from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }


  onSearchChanged(value: string) {
    this.keyword = value;
  }

  onSearch() {
    this.clearFilterAndPagedResult();
    this.loadData(null);
  }

  private cancel() {
    this.dialogRef.close();
  }
  //loadData(id) {
  //  this.auditService.getDataLogDetailsById(this.id)
  //    .subscribe(results => {
  //      this.details = results;
  //    },
  //      error => {
  //        this.alertService.stopLoadingMessage();

  //        this.alertService.showStickyMessage("Load Error", `Unable to retrieve details from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
  //          MessageSeverity.error);
  //      });
  //}
}
