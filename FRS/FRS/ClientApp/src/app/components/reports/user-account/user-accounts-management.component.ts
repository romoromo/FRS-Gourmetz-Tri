import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDatepickerInputEvent, MatDialog } from '@angular/material';
import { AuthLog } from 'src/app/models/auth-log.model';
import { AuditService } from 'src/app/services/audit.service';
import { DateTimeOnlyPipe } from 'src/app/pipes/datetime.pipe';
import { Subscription } from 'rxjs';
import { SearchBoxComponent } from '../../controls/search-box.component';
import { saveAs } from 'file-saver';
import * as moment from 'moment';
import { DepartmentService } from 'src/app/services/department.service';
import { UserGroupService } from 'src/app/services/userGroup.service';
import { UserReportFilter } from 'src/app/models/user.model';


@Component({
  selector: 'user-accounts-report-management',
  templateUrl: './user-accounts-management.component.html',
  styleUrls: ['./user-accounts-management.component.css']
})
export class UserAccountsReportManagementComponent implements OnInit, OnDestroy {
  private subscription: Subscription = new Subscription();
  columns: any[] = [];
  rows: AuthLog[] = [];
  rowsCache: AuthLog[] = [];
  userGroups = [];
  departments = [];
  editingAuthLogName: { name: string };
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';
  start = new Date();
  end = new Date();
  reportFilter = new UserReportFilter();

  @ViewChild('searchbox') searchbox: SearchBoxComponent;

  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('userReportTable') table: any;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private departmentService: DepartmentService, private userGroupService: UserGroupService) {
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  loadUserGroups() {
    let filter = new Filter(-1, -1);
    filter.filters = '(IsActive)==true,(InstitutionId)==' + this.accountService.currentUser.institutionId;

    this.subscription.add(this.userGroupService.getUserGroupsByFilter(filter)
      .subscribe(results => {
        this.userGroups = results.pagedData;

      },
        error => {
          this.alertService.showMessage("Load Error", "Loading user groups from the server failed!", MessageSeverity.warn);
          this.alertService.logError(error);
        }));
  }

  loadDepartments() {
    this.subscription.add(this.departmentService.getDepartments(null, null, this.accountService.currentUser.institutionId)
      .subscribe(results => {
        this.departments = results;
      },
        error => {
          this.alertService.showMessage("Load Error", "Loading departments from the server failed!", MessageSeverity.warn);
          this.alertService.logError(error);
        }));
  }

  initializeFilter() {
    this.filter = new Filter(1, 10);
    this.filter.sorts = '-lastLoginTime';
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
      { prop: 'userName', name: 'User Name', width: 150, canAutoResize: false },
      { prop: 'designation', name: 'Designation' },
      { prop: 'department', name: 'Department' },
      { prop: 'userGroup', name: 'User Group', sortable: false },
      { prop: 'createdDate', name: 'Created On', pipe: new DateTimeOnlyPipe('en-SG') },
      { prop: 'lastLoginTime', name: 'Last Login On', pipe: new DateTimeOnlyPipe('en-SG') },
      { prop: 'inactiveDuration', name: 'Inactive Duration (Days ago)', sortable: false },
      { prop: 'deletedDate', name: 'Deactivated On', pipe: new DateTimeOnlyPipe('en-SG') },
      { prop: 'status', name: 'Status' },
      
    ];
  }

  clearFilterAndPagedResult() {
    this.initializeFilter();
    this.initializePagedResult();
    this.table.offset = 0;
  }

  ngOnInit() {
    this.loadDepartments();
    this.loadUserGroups();
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
    this.filter.filters = '(UserName|JobTitle)@=' + this.keyword;

    if (this.reportFilter.createdStart && !isNaN(this.reportFilter.createdStart.getTime()) &&
      this.reportFilter.createdEnd && !isNaN(this.reportFilter.createdEnd.getTime())) {
      this.filter.filters += ',(UserByCreateDateRange)==' + this.reportFilter.createdStart.toDateString() + '|' + this.reportFilter.createdEnd.toDateString();
    } else {
      if (this.reportFilter.createdStart && !isNaN(this.reportFilter.createdStart.getTime())) {
        this.filter.filters += ',(UserByCreateDateRange)==' + this.reportFilter.createdStart.toDateString();
      }

      if (this.reportFilter.createdEnd && !isNaN(this.reportFilter.createdEnd.getTime())) {
        this.filter.filters += ',(UserByCreateDateRange)==1901-01-01|' + this.reportFilter.createdEnd.toDateString();
      }
    }

    //last login
    if (this.reportFilter.lastLoginStart && !isNaN(this.reportFilter.lastLoginStart.getTime()) &&
      this.reportFilter.lastLoginEnd && !isNaN(this.reportFilter.lastLoginEnd.getTime())) {
      this.filter.filters += ',(UserByLastLoginDateRange)==' + this.reportFilter.lastLoginStart.toDateString() + '|' + this.reportFilter.lastLoginEnd.toDateString();
    } else {
      if (this.reportFilter.lastLoginStart && !isNaN(this.reportFilter.lastLoginStart.getTime())) {
        this.filter.filters += ',(UserByLastLoginDateRange)==' + this.reportFilter.lastLoginStart.toDateString();
      }

      if (this.reportFilter.lastLoginEnd && !isNaN(this.reportFilter.lastLoginEnd.getTime())) {
        this.filter.filters += ',(UserByLastLoginDateRange)==1901-01-01|' + this.reportFilter.lastLoginEnd.toDateString();
      }
    }

    //department
    if (this.reportFilter.departmentId) {
      this.filter.filters += ',(DepartmentId)==' + this.reportFilter.departmentId;
    }

    //user group
    if (this.reportFilter.userGroupId) {
      this.filter.filters += ',(UserByUserGroupId)==' + this.reportFilter.userGroupId;
    }

    //deleted date
    if (this.reportFilter.deletedDate && !isNaN(this.reportFilter.deletedDate.getTime())) {
      this.filter.filters += ',(DeletedDate)==' + this.reportFilter.deletedDate.toDateString();
    }

    //status
    if (typeof this.reportFilter.status != 'undefined') {
      this.filter.filters += ',(IsActive)==' + this.reportFilter.status;
    }

    this.accountService.getUserReportByFilter(this.filter)
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

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve records from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  onChangeDate(type: string, event: MatDatepickerInputEvent<Date>) {
    if (type == 'createdstart') {
      this.reportFilter.createdStart = new Date(event.value);
    }

    if (type == 'createdend') {
      this.reportFilter.createdEnd = new Date(event.value);
    }

    if (type == 'loginstart') {
      this.reportFilter.lastLoginStart = new Date(event.value);
    }

    if (type == 'loginend') {
      this.reportFilter.lastLoginEnd = new Date(event.value);
    }

    if (type == 'deleteddatepicker') {
      this.reportFilter.deletedDate = new Date(event.value);
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
    this.reportFilter = new UserReportFilter();
    this.reportFilter.createdStart = null;
    this.reportFilter.createdEnd = null;
    this.reportFilter.lastLoginStart = null;
    this.reportFilter.lastLoginEnd = null;
    this.reportFilter.deletedDate = null;
    this.keyword = '';
    this.clearFilterAndPagedResult();
    this.searchbox.clear();
  }

  downloadResults() {
    const fileName = moment().format('DDMMYYYY_hhmmss') + '_UserReport.xlsx';

    let f = new Filter(-1, -1);
    f.filters = this.filter.filters;
    f.sorts = this.filter.sorts;
    f.page = -1;
    this.accountService.downloadUserReport(f).subscribe(
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

  get canManageUserAccountReports() {
    return true; //this.accountService.userHasPermission(Permission.viewAuthLogsPermission)
  }

}
