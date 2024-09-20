import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../services/alert.service';
import { AppTranslationService } from "../../services/app-translation.service";
import { AccountService } from '../../services/account.service';
import { Utilities } from '../../services/utilities';
import { Role } from '../../models/role.model';
import { Permission, PermissionTree } from '../../models/permission.model';
import { RoleEditorComponent } from "./role-editor.component";
import { Subscription } from 'rxjs';
import { CommonFilter, PagedResult } from 'src/app/models/sieve-filter.model';


@Component({
  selector: 'roles-management',
  templateUrl: './roles-management.component.html',
  styleUrls: ['./roles-management.component.css']
})
export class RolesManagementComponent implements OnInit, AfterViewInit, OnDestroy {
  private subscription: Subscription = new Subscription();
  columns: any[] = [];
  rows: Role[] = [];
  rowsCache: Role[] = [];
  allPermissions: Permission[] = [];
  allPermissionsTree: PermissionTree;
  editedRole: Role;
  sourceRole: Role;
  editingRoleName: { name: string };
  loadingIndicator: boolean;

  filter: CommonFilter;
  pagedResult: PagedResult;
  keyword: string = '';
  sieveFilters = new Map<string, string>();

  @ViewChild('roleMgtTable') table: any;

  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('roleEditor')
  roleEditor: RoleEditorComponent;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService) {
  }

  initializeFilter() {
    this.filter = new CommonFilter(1, 10);
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

  clearFilterAndPagedResult() {
    this.initializeFilter();
    this.initializePagedResult();
    this.table.offset = 0;
  }

  initializeTableDefinition() {
    let gT = (key: string) => this.translationService.getTranslation(key);

    this.columns = [
      /*{ prop: "index", name: '#', width: 50, cellTemplate: this.indexTemplate, canAutoResize: false, sortable: false },*/
      { prop: 'name', name: gT('roles.management.Name') },
      { prop: 'description', name: gT('roles.management.Description') },
      { prop: 'usersCount', name: gT('roles.management.Users'), sortable: false },
      { name: '', width: 150, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
    ];

    if (!this.accountService.currentUser.institutionId || this.accountService.currentUser.institutionId == '0') {
      this.columns.splice(1, 0, { prop: 'institutionCode', name: gT('users.management.Institution') },);
    }
  }

  initializeDefaultSieveFilter() {
    this.sieveFilters.set('(IsActive)==', 'true');
    this.sieveFilters.set('(InstitutionId)==', this.accountService.currentUser.institutionId);
  }

  ngOnInit() {
    this.initializeFilter();
    this.initializePagedResult();
    this.initializeTableDefinition();
    this.initializeDefaultSieveFilter();
    this.loadData(null, true);

  }

  ngAfterViewInit() {

    this.roleEditor.changesSavedCallback = () => {
      this.addNewRoleToList();
      this.editorModal.hide();
    };

    this.roleEditor.changesCancelledCallback = () => {
      this.editedRole = null;
      this.sourceRole = null;
      this.editorModal.hide();
    };
  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  addNewRoleToList() {
    if (this.sourceRole) {
      Object.assign(this.sourceRole, this.editedRole);

      let sourceIndex = this.rowsCache.indexOf(this.sourceRole, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rowsCache, sourceIndex, 0);

      sourceIndex = this.rows.indexOf(this.sourceRole, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rows, sourceIndex, 0);

      this.editedRole = null;
      this.sourceRole = null;
    }
    else {
      let role = new Role();
      Object.assign(role, this.editedRole);
      this.editedRole = null;

      let maxIndex = 0;
      for (let r of this.rowsCache) {
        if ((<any>r).index > maxIndex)
          maxIndex = (<any>r).index;
      }

      (<any>role).index = maxIndex + 1;

      this.rowsCache.splice(0, 0, role);
      this.rows.splice(0, 0, role);
      this.rows = [...this.rows];
    }
  }

  loadData(ev?: any, isFirstLoad?: boolean) {
    this.alertService.startLoadingMessage();
    this.loadingIndicator = true;
    this.filter.pageSize = 10;
    this.filter.institutionId = this.accountService.currentUser.institutionId;

    if (ev) {
      this.filter.page = ev.offset + 1;
      if (ev.sorts) {
        this.filter.sorts = ev.sorts[0].dir == 'desc' ? '-' + ev.sorts[0].prop : ev.sorts[0].prop;
      }
    }

    if (!this.sieveFilters) {
      this.initializeDefaultSieveFilter();
    }
    if (!this.keyword) this.keyword = '';


    let val = this.sieveFilters.has('(Name|Description)@=');
    if (val) {
      this.sieveFilters.delete('(Name|Description)@=');
    }

    if (this.keyword) {
      this.sieveFilters.set('(Name|Description)@=', this.keyword);
    }

    this.filter.filters = '';
    let indx = 0;
    this.sieveFilters.forEach((value: string, key: string) => {
      if (indx > 0) {
        this.filter.filters += ','
      }
      this.filter.filters += key + value;
      indx++;
    });
    this.subscription.add(this.accountService.getRolesAndPermissionsByFilters(this.filter).subscribe(results => {
      let pagedResult = results[0];
      let permissionsTree = results[1];
      this.alertService.stopLoadingMessage();
      this.loadingIndicator = false;
      this.pagedResult = pagedResult;
      let roles = pagedResult.pagedData;

      roles.forEach((role, index, roles) => {
        (<any>role).index = index + 1;
      });

      this.rowsCache = [...roles];
      this.rows = roles;

      this.allPermissionsTree = permissionsTree;

    }, error => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.alertService.showStickyMessage("Load Error", `Unable to retrieve roles from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
          MessageSeverity.error);
    }));
  }


  onSearchChanged(value: string) {
    //this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.name, r.description));
    this.keyword = value;
  }

  onSearch() {
    this.clearFilterAndPagedResult();
    this.loadData(null);
  }

  onEditorModalHidden() {
    this.editingRoleName = null;
    this.roleEditor.resetForm(true);
  }


  newRole() {
    this.editingRoleName = null;
    this.sourceRole = null;
    this.editedRole = this.roleEditor.newRole(this.allPermissions, this.allPermissionsTree);
    this.editedRole.institutionId = this.accountService.currentUser.institutionId;
    this.editorModal.show();
    this.roleEditor.loadTree();
  }


  editRole(row: Role) {
    this.editingRoleName = { name: row.name };
    this.sourceRole = row;
    this.editedRole = this.roleEditor.editRole(row, this.allPermissions, this.allPermissionsTree);
    this.editorModal.show();
    this.roleEditor.loadTree();
  }

  deleteRole(row: Role) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" role?', DialogType.confirm, () => this.deleteRoleHelper(row));
  }


  deleteRoleHelper(row: Role) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.subscription.add(this.accountService.deleteRole(row)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.rowsCache = this.rowsCache.filter(item => item !== row)
        this.rows = this.rows.filter(item => item !== row)
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the role.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }


  get canManageRoles() {
    return this.accountService.userHasPermission(Permission.manageRolesPermission)
  }

}
