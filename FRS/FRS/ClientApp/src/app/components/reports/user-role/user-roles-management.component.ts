import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { Utilities } from '../../../services/utilities';
import { CommonFilter, Filter, PagedResult, RoleReportFilter } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDatepickerInputEvent, MatDialog } from '@angular/material';
import { AuthLog } from 'src/app/models/auth-log.model';
import { AuditService } from 'src/app/services/audit.service';
import { DateOnlyPipe, DateTimeOnlyPipe } from 'src/app/pipes/datetime.pipe';
import { Subscription } from 'rxjs';
import { SearchBoxComponent } from '../../controls/search-box.component';
import { saveAs } from 'file-saver';
import * as moment from 'moment';
import { DepartmentService } from 'src/app/services/department.service';
import { UserGroupService } from 'src/app/services/userGroup.service';
import { UserReportFilter, UserRoleReportFilter } from 'src/app/models/user.model';
import { AccountService } from 'src/app/services/account.service';


@Component({
  selector: 'user-roles-report-management',
  templateUrl: './user-roles-management.component.html',
  styleUrls: ['./user-roles-management.component.css']
})
export class UserRolesReportManagementComponent implements OnInit, OnDestroy {
  private subscription: Subscription = new Subscription();
  columns = [];
  rows = [];
  rowsCache = [];
  userRoles = [];
  permissions = [];
  editingAuthLogName: { name: string };
  loadingIndicator: boolean;
  filter: RoleReportFilter;
  pagedResult: PagedResult;
  keyword: string = '';
  start = new Date();
  end = new Date();
  reportFilter = new UserRoleReportFilter();

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
    let filter = new CommonFilter(-1, -1);
    filter.filters = '(IsActive)==true,(InstitutionId)==' + this.accountService.currentUser.institutionId;

    this.subscription.add(this.accountService.getRolesAndPermissionListByFilters(filter)
      .subscribe(results => {
        let pagedResult = results[0];
        this.permissions = results[1];
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;
        this.userRoles = pagedResult.pagedData;
      },
        error => {
          this.alertService.showMessage("Load Error", "Loading user roles from the server failed!", MessageSeverity.warn);
          this.alertService.logError(error);
        }));
  }

  initializeFilter() {
    this.filter = new RoleReportFilter(1, 10);
    this.filter.sorts = 'name';
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
      { prop: "index", name: 'S/N', width: 50, cellTemplate: this.indexTemplate, canAutoResize: false },
      { prop: 'roleName', name: 'Role', width: 150, canAutoResize: false },
      { prop: 'permissions', name: 'Functions', sortable: false },
      { prop: 'createdDate', name: 'Created Date', pipe: new DateOnlyPipe('en-SG') },
      { prop: 'lastUpdatedDate', name: 'Last Updated Date', pipe: new DateOnlyPipe('en-SG') }      
    ];
  }

  clearFilterAndPagedResult() {
    this.initializeFilter();
    this.initializePagedResult();
    this.table.offset = 0;
  }

  ngOnInit() {
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
    this.filter.filters = '(IsActive)==true,(Name)@=' + this.keyword + ",(InstitutionId)==" + this.accountService.currentUser.institutionId;

    if (this.reportFilter.createdStart && !isNaN(this.reportFilter.createdStart.getTime()) &&
      this.reportFilter.createdEnd && !isNaN(this.reportFilter.createdEnd.getTime())) {
      this.filter.filters += ',(RoleByCreatedDateRange)==' + this.reportFilter.createdStart.toDateString() + '|' + this.reportFilter.createdEnd.toDateString();
    } else {
      if (this.reportFilter.createdStart && !isNaN(this.reportFilter.createdStart.getTime())) {
        this.filter.filters += ',(RoleByCreatedDateRange)==' + this.reportFilter.createdStart.toDateString();
      }

      if (this.reportFilter.createdEnd && !isNaN(this.reportFilter.createdEnd.getTime())) {
        this.filter.filters += ',(RoleByCreatedDateRange)==1901-01-01|' + this.reportFilter.createdEnd.toDateString();
      }
    }

    if (this.reportFilter.lastUpdatedStart && !isNaN(this.reportFilter.lastUpdatedStart.getTime()) &&
      this.reportFilter.lastUpdatedEnd && !isNaN(this.reportFilter.lastUpdatedEnd.getTime())) {
      this.filter.filters += ',(RoleByLastUpdatedDateRange)==' + this.reportFilter.lastUpdatedStart.toDateString() + '|' + this.reportFilter.lastUpdatedEnd.toDateString();
    } else {
      if (this.reportFilter.lastUpdatedStart && !isNaN(this.reportFilter.lastUpdatedStart.getTime())) {
        this.filter.filters += ',(RoleByLastUpdatedDateRange)==' + this.reportFilter.lastUpdatedStart.toDateString();
      }

      if (this.reportFilter.lastUpdatedEnd && !isNaN(this.reportFilter.lastUpdatedEnd.getTime())) {
        this.filter.filters += ',(RoleByLastUpdatedDateRange)==1901-01-01|' + this.reportFilter.lastUpdatedEnd.toDateString();
      }
    }

    //user role
    if (this.reportFilter.roleId) {
      this.filter.filters += ',(RoleById)==' + this.reportFilter.roleId;
    }

    if (!this.reportFilter.permissions) this.reportFilter.permissions = [];

    this.filter.permissions = this.reportFilter.permissions;
    this.accountService.getUserRoleReportByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;
        console.log(this.pagedResult);
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let rows = results.pagedData;

        rows.forEach((row, index, rows) => {
          (<any>row).index = index + 1;
        });


        this.rowsCache = [...rows];
        this.rows = rows;

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

    if (type == 'lastupdatedstart') {
      this.reportFilter.lastUpdatedStart = new Date(event.value);
    }

    if (type == 'lastupdatedend') {
      this.reportFilter.lastUpdatedEnd = new Date(event.value);
    }
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
    this.reportFilter = new UserRoleReportFilter();
    this.reportFilter.createdStart = null;
    this.reportFilter.createdEnd = null;
    this.reportFilter.lastUpdatedStart = null;
    this.reportFilter.lastUpdatedEnd = null;
    this.reportFilter.permissions = [];
    this.keyword = '';
    this.clearFilterAndPagedResult();
    this.searchbox.clear();
  }

  downloadResults() {
    const fileName = moment().format('DDMMYYYY_hhmmss') + '_RoleReport.xlsx';

    let f = new Filter(-1, -1);
    f.filters = this.filter.filters;
    f.sorts = this.filter.sorts;
    f.page = -1;
    this.accountService.downloadUserRoleReport(f).subscribe(
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

  get canManageUserRoleReports() {
    return true; //this.accountService.userHasPermission(Permission.viewAuthLogsPermission)
  }

}
