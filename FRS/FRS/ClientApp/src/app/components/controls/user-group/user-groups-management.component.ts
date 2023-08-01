import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { UserGroup } from '../../../models/userGroup.model';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { UserGroupEditorComponent } from "./user-group-editor.component";
import { UserGroupService } from 'src/app/services/userGroup.service';
import { MatDialog } from '@angular/material';


@Component({
  selector: 'user-groups-management',
  templateUrl: './user-groups-management.component.html',
  styleUrls: ['./user-groups-management.component.css']
})
export class UserGroupsManagementComponent implements OnInit {
  columns: any[] = [];
  rows: UserGroup[] = [];
  rowsCache: UserGroup[] = [];
  allPermissions: Permission[] = [];
  editedUserGroup: UserGroup;
  sourceUserGroup: UserGroup;
  editingUserGroupName: { name: string };
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

  @ViewChild('userGroupEditor')
  userGroupEditor: UserGroupEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private userGroupService: UserGroupService, public dialog: MatDialog) {
  }

  openDialog(userGroup: UserGroup): void {
    const dialogRef = this.dialog.open(UserGroupEditorComponent, {
      data: { header: this.header, userGroup: userGroup },
      width: '800px'
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
      //{ prop: "index", name: '#', width: 50, cellTemplate: this.indexTemplate, canAutoResize: false },
      { prop: 'name', name: gT('userGroups.management.Name') },
      { prop: 'description', name: gT('userGroups.management.Description') },
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
    this.filter.filters = '(IsActive)==true,(Name|Description)@=' + this.keyword + ',(InstitutionId)==' + this.accountService.currentUser.institutionId;
    
    this.userGroupService.getUserGroupsByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let userGroups = results.pagedData;

        userGroups.forEach((userGroup, index, userGroups) => {
          (<any>userGroup).index = index + 1;
        });


        this.rowsCache = [...userGroups];
        this.rows = userGroups;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve userGroups from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }


  onSearchChanged(value: string) {
    //this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.name, r.description));
    this.keyword = value;
    this.loadData(null);
  }


  onEditorModalHidden() {
    this.editingUserGroupName = null;
    this.userGroupEditor.resetForm(true);
  }


  newUserGroup() {
    //this.editingUserGroupName = null;
    //this.sourceUserGroup = null;
    //this.editedUserGroup = this.userGroupEditor.newUserGroup();
    //this.editorModal.show();
    this.header = 'New User Group';
    this.editedUserGroup = new UserGroup();
    this.openDialog(this.editedUserGroup);
  }


  editUserGroup(row: UserGroup) {
    //this.editingUserGroupName = { name: row.name };
    //this.sourceUserGroup = row;
    this.editedUserGroup = row; //this.userGroupEditor.editUserGroup(row);
    //this.editorModal.show();

    this.header = 'Edit User Group';
    this.openDialog(this.editedUserGroup);
  }

  deleteUserGroup(row: UserGroup) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" user group?', DialogType.confirm, () => this.deleteUserGroupHelper(row));
  }


  deleteUserGroupHelper(row: UserGroup) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.userGroupService.deleteUserGroup(row)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        //this.rowsCache = this.rowsCache.filter(item => item !== row)
        //this.rows = this.rows.filter(item => item !== row)
        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the userGroup.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }


  get canManageUserGroups() {
    return this.accountService.userHasPermission(Permission.manageUserGroupsPermission)
  }

}
