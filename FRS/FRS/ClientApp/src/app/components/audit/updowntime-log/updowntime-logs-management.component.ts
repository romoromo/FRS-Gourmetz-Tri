import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDatepickerInputEvent, MatDialog } from '@angular/material';
import { UpDownTimeLog } from 'src/app/models/updowntime-log.model';
import { AuditService } from 'src/app/services/audit.service';
import { DateTimeOnlyPipe } from 'src/app/pipes/datetime.pipe';
import { Subscription } from 'rxjs';
import { SearchBoxComponent } from '../../controls/search-box.component';
import { saveAs } from 'file-saver';
import * as moment from 'moment';


@Component({
  selector: 'updowntime-logs-management',
  templateUrl: './updowntime-logs-management.component.html',
  styleUrls: ['./updowntime-logs-management.component.css']
})
export class UpDownTimeLogsManagementComponent implements OnInit, OnDestroy {
  private subscription: Subscription = new Subscription();
  columns: any[] = [];
  rows: UpDownTimeLog[] = [];
  rowsCache: UpDownTimeLog[] = [];
  editingUpDownTimeLogName: { name: string };
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

  @ViewChild('upDownTimeLogTable') table: any;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private upDownTimeLogService: AuditService) {
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  initializeFilter() {
    this.filter = new Filter(1, 10);
    this.filter.sorts = '-createdDate';
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
      { prop: 'deviceIdentifier', name: 'Device Identifier', canAutoResize: false },
      { prop: 'status', name: 'Status'},
      { prop: 'createdDate', name: 'Date', pipe: new DateTimeOnlyPipe('en-SG'), canAutoResize: false},
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
    this.filter.filters = '(DeviceIdentifier)@=' + this.keyword + ',(UpDownTimeDateRange)==' + this.start.toDateString() + '|' + this.end.toDateString();
    
    this.upDownTimeLogService.getUpDownTimeLogsByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;
        console.log(this.pagedResult);
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let upDownTimeLogs = results.pagedData;

        upDownTimeLogs.forEach((upDownTimeLog, index, upDownTimeLogs) => {
          (<any>upDownTimeLog).index = index + 1;
          (<any>upDownTimeLog).status = upDownTimeLog.isUp ? "UP" : "DOWN";
        });


        this.rowsCache = [...upDownTimeLogs];
        this.rows = upDownTimeLogs;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve upDownTime logs from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
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
  }

  clearFilters() {
    this.start = new Date();
    this.end = new Date();
    this.keyword = '';
    this.clearFilterAndPagedResult();
    this.searchbox.clear();
  }

  downloadResults() {
    const fileName = moment().format('DDMMYYYY_hhmmss') + '_UpDownTimeLogs.xlsx';

    this.upDownTimeLogService.downloadUpDownTimeLogReport(this.filter).subscribe(
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

  get canManageUpDownTimeLogs() {
    return this.accountService.userHasPermission(Permission.viewUpDownTimeLogsPermission)
  }

}
