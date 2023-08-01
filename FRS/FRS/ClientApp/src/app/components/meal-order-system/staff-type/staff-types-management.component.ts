import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { StaffType } from 'src/app/models/meal-order/staff-type.model';
import { StaffTypeEditorComponent } from './staff-type-editor.component';
import { UserService } from 'src/app/services/meal-order/user.service';


@Component({
  selector: 'staff-types-management',
  templateUrl: './staff-types-management.component.html',
  styleUrls: ['./staff-types-management.component.css']
})
export class StaffTypesManagementComponent implements OnInit {
  columns: any[] = [];
  rows: StaffType[] = [];
  rowsCache: StaffType[] = [];
  allPermissions: Permission[] = [];
  editedStaffType: StaffType;
  sourceStaffType: StaffType;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @Input() isHideHeader: boolean;

  @Input() outletId: string;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('staffTypeEditor')
  staffTypeEditor: StaffTypeEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private userService: UserService, public dialog: MatDialog) {
  }

  openDialog(staffType: StaffType): void {
    const dialogRef = this.dialog.open(StaffTypeEditorComponent, {
      data: { header: this.header, staffType: staffType },
      width: '400px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData(null);
    });
  }

  initializeFilter() {
    this.filter = new Filter(1, 10);
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
      { prop: 'name', name: gT('common.Name'), width: 200 },
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
    this.filter.filters = '(IsActive)==true,(Name)@=' + this.keyword + ',(InstitutionId)==' + this.accountService.currentUser.institutionId;
    
    this.userService.getStaffTypesByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let staffTypes = results.pagedData;

        staffTypes.forEach((staffType, index, staffTypes) => {
          (<any>staffType).index = index + 1;
        });


        this.rowsCache = [...staffTypes];
        this.rows = staffTypes;

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

  newStaffType() {
    this.header = 'New Staff Type';
    this.editedStaffType = new StaffType();
    this.openDialog(this.editedStaffType);
  }


  editStaffType(row: StaffType) {
    this.editedStaffType = row;
    this.header = 'Edit Staff Type';
    this.openDialog(this.editedStaffType);
  }

  deleteStaffType(row: StaffType) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" user type?', DialogType.confirm, () => this.deleteStaffTypeHelper(row));
  }


  deleteStaffTypeHelper(row: StaffType) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.userService.deleteStaffType(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the staff type.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  get canManageStaffTypes() {
    return true; //this.accountService.staffHasPermission(Permission.manageStaffTypesPermission)
  }

}
