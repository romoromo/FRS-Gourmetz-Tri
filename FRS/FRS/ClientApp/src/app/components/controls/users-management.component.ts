import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, ViewEncapsulation, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../services/alert.service';
import { AppTranslationService } from "../../services/app-translation.service";
import { AccountService } from "../../services/account.service";
import { Utilities } from "../../services/utilities";
import { FRSHubConnections, User } from '../../models/user.model';
import { Role } from '../../models/role.model';
import { Permission } from '../../models/permission.model';
import { UserEdit } from '../../models/user-edit.model';
import { UserInfoComponent } from "./user-info.component";
import { Department } from 'src/app/models/department.model';
import { DepartmentService } from 'src/app/services/department.service';
import * as signalRCore from '@aspnet/signalr';
import { AuthService } from 'src/app/services/auth.service';
import { ConfigurationService } from 'src/app/services/configuration.service';
import { CommonFilter, Filter, PagedResult } from 'src/app/models/sieve-filter.model';
import { forEach } from '@angular/router/src/utils/collection';
import { Subscription } from 'rxjs';
import { ContactGroupService } from 'src/app/services/contactgroup.service';
import { isNumeric } from 'jquery';
import { RewardOperation } from 'src/app/models/userreward.model';

@Component({
  selector: 'users-management',
  templateUrl: './users-management.component.html',
  styleUrls: ['./users-management.component.css']
})
export class UsersManagementComponent implements OnInit, AfterViewInit, OnDestroy {
  private subscription: Subscription = new Subscription();
  columns: any[] = [];
  rows: User[] = [];
  rowsCache: User[] = [];
  editedUser: UserEdit;
  sourceUser: UserEdit;
  editingUserName: { name: string };
  loadingIndicator: boolean;
  forApproval = false;
  forOnline = false;
  searchText = '';
  allRoles: Role[] = [];
  allDepartments: Department[] = [];

  @ViewChild('userMgtTable') table: any;

  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('userNameTemplate')
  userNameTemplate: TemplateRef<any>;

  @ViewChild('rolesTemplate')
  rolesTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('userEditor')
  userEditor: UserInfoComponent;

  private connection: signalRCore.HubConnection;
  filter: CommonFilter;
  pagedResult: PagedResult;
  keyword: string = '';
  sieveFilters = new Map<string, string>();

  constructor(private alertService: AlertService, public connections: FRSHubConnections, private translationService: AppTranslationService, private authService: AuthService, private accountService: AccountService, private departmentService: DepartmentService,
    protected configurations: ConfigurationService, private contactGroupService: ContactGroupService  ) {

  }

  initializeFilter() {
    this.filter = new CommonFilter(1, 10);
    this.filter.sorts = 'userName';
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
      //{ prop: "index", name: '', width: 100, cellTemplate: this.indexTemplate, canAutoResize: false },
      //{ prop: 'jobTitle', name: gT('users.management.Title') },
      { prop: 'userName', name: gT('users.management.UserName'), cellTemplate: this.userNameTemplate },
      { prop: 'fullName', name: gT('users.management.FullName') },
      { prop: 'email', name: gT('users.management.Email') },
      { prop: 'departmentName', name: gT('users.management.Department') },
      { prop: 'roles', name: gT('users.management.Roles'), cellTemplate: this.rolesTemplate, sortable: false },
      { prop: 'phoneNumber', name: gT('users.management.PhoneNumber') }
    ];

    if (!this.accountService.currentUser.institutionId || this.accountService.currentUser.institutionId == '0') {
      this.columns.splice(1, 0, { prop: 'institutionCode', name: gT('users.management.Institution') },);
    }


    if (this.canManageUsers)
      this.columns.push({ name: '', width: 150, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false });

  }

  initializeDefaultSieveFilter() {
    this.sieveFilters.set('(IsActive)==', 'true');
    this.sieveFilters.set('(InstitutionId)==', this.accountService.currentUser.institutionId);
    //this.sieveFilters = [];
    //this.sieveFilters = ['(IsActive)==true'];
    //this.sieveFilters.push('(InstitutionId)==' + this.accountService.currentUser.institutionId);
  }

  ngOnInit() {
    this.initializeFilter();
    this.initializePagedResult();
    this.initializeTableDefinition();
    this.initializeDefaultSieveFilter();
    this.loadData(null, true);
   
  }


  ngAfterViewInit() {

    this.userEditor.changesSavedCallback = () => {
      this.addNewUserToList();
      this.editorModal.hide();
      //this.loadData();
    };

    this.userEditor.changesCancelledCallback = () => {
      this.editedUser = null;
      this.sourceUser = null;
      this.editorModal.hide();
    };
  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  updateUserStatus(email: string, status: string) {
    if (this.connection != null) {
      this.connection.invoke('UpdateStatus', email, status)
        .catch(err => console.error(err));
    }
  }

  refilter() {

    this.forApproval = !this.forApproval;
    this.onFilterApprovedUsers();

    this.forOnline = !this.forOnline;
    this.onFilterOnlineUsers();

    this.onSearchChanged(this.searchText);
  }

  initUserHubConnection() {
    if (this.connections.userHubConnection == null || this.connections.userHubConnection.state !== signalRCore.HubConnectionState.Connected) {
      //let status = this.accountService.currentUser.status ? this.accountService.currentUser.status : 'ONLINE';
      this.connections.userHubConnection = this.authService.signalRConnection(this.configurations.baseUrl + `/hub/user?email=${this.accountService.currentUser.email}`, true, this.connections.userHubConnection);
      this.connection = this.connections.userHubConnection;
      if (this.connection != null) {
        this.connection.off("BroadcastUserStatusChange");
        this.connection.on("BroadcastUserStatusChange", (user) => {
          console.log('BroadcastUserStatusChange');
          console.log(user);
          let userCache = this.rowsCache.filter(item => item && item.email == user.email);
          if (userCache && userCache.length > 0) {
            //let indx = this.rowsCache.indexOf(userCache[0]);
            //this.rowsCache[indx].status = user.status;
            userCache[0].status = user.status;
            userCache[0].isConnected = user.isConnected;
            //this.rowsCache = [...this.rowsCache];
          }

          //let userRow = this.rows.filter(item => item && item.email == user.email);
          //if (userRow && userRow.length > 0) {
          //  userRow[0].status = user.status;
          //  userRow[0].isConnected = user.isConnected;

          //  this.rows = [...this.rows];
          //}

          this.filterAll();
        });

        this.connection.off("BroadcastAddedUser");
        this.connection.on("BroadcastAddedUser", (user) => {
          console.log('BroadcastAddedUser');
          console.log(user);

          if (user && user.id) {
            //this.sourceUser = null;
            //this.editedUser = user;
            //this.addNewUserToList();
            //this.rowsCache.push(user);
            //this.rows = this.rowsCache;
            //let maxIndex = 0;
            //for (let u of this.rowsCache) {
            //  if ((<any>u).index > maxIndex)
            //    maxIndex = (<any>u).index;
            //}

            //(<any>user).index = maxIndex + 1;

            //this.rowsCache.splice(0, 0, user);
            //this.rows.splice(0, 0, user);
            //this.rows = [...this.rows];
          }
        });

        this.connection.off("BroadcastUpdatedUser");
        this.connection.on("BroadcastUpdatedUser", (user) => {
          console.log('BroadcastUpdatedUser');
          console.log(user);

          if (user && user.id) {
            //this.sourceUser = user;
            //this.editedUser = user;
            //this.addNewUserToList();
            let userCache = this.rowsCache.filter(item => item && item.id == user.id);
            if (userCache && userCache.length > 0) {
              let indx = this.rowsCache.indexOf(userCache[0]);
              //this.rowsCache[indx] = user;
              this.rowsCache[indx] = user;
              this.rowsCache = [...this.rowsCache];
            }

            let userRow = this.rows.filter(item => item && item.id == user.id);
            if (userRow && userRow.length > 0) {
              let indx = this.rows.indexOf(userRow[0]);
              //this.rowsCache[indx] = user;
              this.rows[indx] = user;
              this.rows = [...this.rows];
            }

            this.filterAll();
          }

        });

        this.connection.off("BroadcastDeletedUser");
        this.connection.on("BroadcastDeletedUser", (user) => {
          console.log('BroadcastDeletedUser');
          console.log(user);
        });
      }
    } else {
      this.connection = this.connections.userHubConnection;
      if (this.connection != null) {
        this.connection.off("BroadcastUserStatusChange");
        this.connection.on("BroadcastUserStatusChange", (user) => {
          console.log('BroadcastUserStatusChange');
          console.log(user);
          let userCache = this.rowsCache.filter(item => item && item.email == user.email);
          if (userCache && userCache.length > 0) {
            //let indx = this.rowsCache.indexOf(userCache[0]);
            //this.rowsCache[indx].status = user.status;
            userCache[0].status = user.status;
            userCache[0].isConnected = user.isConnected;
            this.rowsCache = [...this.rowsCache];
          }

          let userRow = this.rows.filter(item => item && item.email == user.email);
          if (userRow && userRow.length > 0) {
            userRow[0].status = user.status;
            userRow[0].isConnected = user.isConnected;

            this.rows = [...this.rows];
          }

          this.filterAll();
        });

        this.connection.off("BroadcastAddedUser");
        this.connection.on("BroadcastAddedUser", (user) => {
          console.log('BroadcastAddedUser');
          console.log(user);

          if (user && user.id) {
            //let userCache = this.rowsCache.filter(item => item && item.id == user.id);
            //if (userCache && userCache.length > 0) {
            //  let indx = this.rowsCache.indexOf(userCache[0]);
            //  //this.rowsCache[indx] = user;
            //  userCache[0] = user;
            //}

            //this.rows = this.rowsCache;

            //this.sourceUser = null;
            //this.editedUser = user;
            //this.addNewUserToList();
            //this.rowsCache.push(user);
            //this.rows = this.rowsCache;
            //let maxIndex = 0;
            //for (let u of this.rowsCache) {
            //  if ((<any>u).index > maxIndex)
            //    maxIndex = (<any>u).index;
            //}

            //(<any>user).index = maxIndex + 1;

            //this.rowsCache.splice(0, 0, user);
            //this.rows.splice(0, 0, user);
            //this.rows = [...this.rows];
          }
        });

        this.connection.off("BroadcastUpdatedUser");
        this.connection.on("BroadcastUpdatedUser", (user) => {
          console.log('BroadcastUpdatedUser');
          console.log(user);

          if (user && user.id) {
            //this.sourceUser = user;
            //this.editedUser = user;
            //this.addNewUserToList();
            let userCache = this.rowsCache.filter(item => item && item.id == user.id);
            if (userCache && userCache.length > 0) {
              let indx = this.rowsCache.indexOf(userCache[0]);
              //this.rowsCache[indx] = user;
              this.rowsCache[indx] = user;
              this.rowsCache = [...this.rowsCache];
            }

            let userRow = this.rows.filter(item => item && item.id == user.id);
            if (userRow && userRow.length > 0) {
              let indx = this.rows.indexOf(userRow[0]);
              //this.rowsCache[indx] = user;
              this.rows[indx] = user;
              this.rows = [...this.rows];
            }

            this.filterAll();
          }

        });
        this.connection.off("BroadcastDeletedUser");
        this.connection.on("BroadcastDeletedUser", (user) => {
          console.log('BroadcastDeletedUser');
          console.log(user);
        });
      }
    }




  }

  addNewUserToList() {
    if (this.sourceUser) {
      Object.assign(this.sourceUser, this.editedUser);
      this.updateUserStatus(this.editedUser.email, this.editedUser.status);

      let sourceIndex = this.rowsCache.indexOf(this.sourceUser, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rowsCache, sourceIndex, 0);

      sourceIndex = this.rows.indexOf(this.sourceUser, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rows, sourceIndex, 0);

      this.editedUser = null;
      this.sourceUser = null;
    }
    else {
      let user = new User();
      Object.assign(user, this.editedUser);
      this.editedUser = null;

      let maxIndex = 0;
      for (let u of this.rowsCache) {
        if (u && ((<any>u).index > maxIndex))
          maxIndex = (<any>u).index;
      }

      (<any>user).index = maxIndex + 1;

      this.rowsCache.splice(0, 0, user);
      this.rows.splice(0, 0, user);
      this.rows = [...this.rows];
      this.updateUserStatus(user.email, user.status);
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

    
    let val = this.sieveFilters.has('(UserName|Email|PhoneNumber|departmentName)@=');
    if (val) {
      this.sieveFilters.delete('(UserName|Email|PhoneNumber|departmentName)@=');
    }

    if (this.keyword) {
      this.sieveFilters.set('(UserName|Email|PhoneNumber|departmentName)@=', this.keyword);
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

    if (this.canViewRoles) {
      if (isFirstLoad) {
        this.subscription.add(this.accountService.getUsersAndRolesByFilters(this.filter).subscribe(results => this.onDataLoadSuccessful(results[0], results[1], results[2]), error => this.onDataLoadFailed(error)));
      } else {
        this.subscription.add(this.accountService.getUsersByFilter(this.filter).subscribe(users => this.onDataLoadSuccessful(users, this.allRoles, this.allDepartments), error => this.onDataLoadFailed(error)));
      }
    }
    else {
      var department: Department[] = [];
      department.push(new Department(this.accountService.currentUser.departmentId, this.accountService.currentUser.departmentName));
      this.subscription.add(this.accountService.getUsersByFilter(this.filter).subscribe(users => this.onDataLoadSuccessful(users, this.accountService.currentUser.roles.map(x => new Role(x)), department), error => this.onDataLoadFailed(error)));
    }
  }

  loadDepartmets() {
    this.subscription.add(this.departmentService.getDepartments(1, 10, this.accountService.currentUser.institutionId)
      .subscribe(results => {
        this.allDepartments = results;
      },
        error => {
          this.alertService.showMessage("Load Error", "Loading departments from the server failed!", MessageSeverity.warn);
          this.alertService.logError(error);
        }));
  }

  onDataLoadSuccessful(pagedResult: PagedResult, roles: Role[], departments: Department[]) {
    this.alertService.stopLoadingMessage();
    this.loadingIndicator = false;
    this.pagedResult = pagedResult;
    let users = pagedResult.pagedData;

    users.forEach((user, index, users) => {
      (<any>user).index = index + 1;
    });

    this.rowsCache = [...users];
    this.rows = users;

    this.allRoles = roles;
    this.allDepartments = departments;

    this.initUserHubConnection();
    //this.updateUserStatus(this.accountService.currentUser.email);
  }


  onDataLoadFailed(error: any) {
    this.alertService.stopLoadingMessage();
    this.loadingIndicator = false;

    this.alertService.showStickyMessage("Load Error", `Unable to retrieve users from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
      MessageSeverity.error);
  }

  filterAll() {
    //this.rows = this.rowsCache;

    //if (this.searchText) {
    //  this.rows = this.rowsCache.filter(r => r && Utilities.searchArray(this.searchText, false, r.userName, r.fullName, r.email, r.phoneNumber, r.jobTitle, r.roles));
    //}

    //if (this.forApproval) {
    //  this.rows = this.rows.filter(r => !r.isEnabled);
    //}

    //if (this.forOnline) {
    //  this.rows = this.rows.filter(r => r.isConnected);
    //}

  }

  onSearchChanged(value: string) {
    
    //this.clearFilterAndPagedResult();
    this.keyword = value;
    //this.loadData(null);
    //this.rows = this.rowsCache.filter(r => r && Utilities.searchArray(value, false, r.userName, r.fullName, r.email, r.phoneNumber, r.jobTitle, r.roles));
    //this.searchText = value;
    //this.filterAll();
    //this.forOnline = !this.forOnline;
    //this.forApproval = !this.forApproval;
    //this.onFilterApprovedUsers();
    //this.onFilterOnlineUsers();
  }

  onSearch() {
    this.clearFilterAndPagedResult();
    this.loadData(null);
  }

  onFilterApprovedUsers() {
    this.forApproval = !this.forApproval;
    this.clearFilterAndPagedResult();

    let val = this.sieveFilters.has('(IsEnabled)==');
    if (val) {
      this.sieveFilters.delete('(IsEnabled)==');
    }

    if (this.forApproval) {
      this.sieveFilters.set('(IsEnabled)==', "false");
    }

    this.loadData(null, false);
    //this.filterAll();
    //this.forApproval = !this.forApproval;
    //if (this.forApproval) {
    //  this.rows = this.rows.filter(r => !r.isEnabled);
    //} else {
    //  //this.rows = this.rowsCache;
    //}
  }

  onFilterOnlineUsers() {
    this.clearFilterAndPagedResult();
    this.forOnline = !this.forOnline;

    let val = this.sieveFilters.has('(IsConnected)==');
    if (val) {
      this.sieveFilters.delete('(IsConnected)==');
    }

    if (this.forOnline) {
      this.sieveFilters.set('(IsConnected)==', "true");
    }
    this.loadData(null, false);
    //this.filterAll();
    //this.forOnline = !this.forOnline;
    //if (this.forOnline) {
    //  this.rows = this.rows.filter(r => r.isConnected);
    //} else {
    //  //this.rows = this.rowsCache;
    //}
  }

  onEditorModalHidden() {
    this.editingUserName = null;
    this.userEditor.resetForm(true);
  }


  newUser() {
    this.editingUserName = null;
    this.sourceUser = null;
    this.editedUser = this.userEditor.newUser(this.allRoles, this.allDepartments);
    this.editorModal.show();
  }


  editUser(row: UserEdit) {
    this.editingUserName = { name: row.userName };
    this.sourceUser = row;
    console.log(this.allRoles);
    this.editedUser = this.userEditor.editUser(row, this.allRoles, this.allDepartments);
    this.editorModal.show();
  }


  deleteUser(row: UserEdit) {
    this.alertService.showDialog('Are you sure you want to delete \"' + row.userName + '\"?', DialogType.confirm, () => this.deleteUserHelper(row));
  }

  trueDeleteUser(row: UserEdit) {
    this.alertService.showDialog('Are you sure you want to delete \"' + row.userName + '\"?', DialogType.confirm, () => this.trueDeleteUserHelper(row));
  }


  deleteUserHelper(row: UserEdit) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.subscription.add(this.accountService.deleteUser(row)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.rowsCache = this.rowsCache.filter(item => item !== row)
        this.rows = this.rows.filter(item => item !== row)
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the user.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }


  trueDeleteUserHelper(row: UserEdit) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.subscription.add(this.accountService.trueDeleteUser(row)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.rowsCache = this.rowsCache.filter(item => item !== row)
        this.rows = this.rows.filter(item => item !== row)
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the user.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }

  addRewardPoints(row: UserEdit): void {
    this.alertService.showDialog("How many points do you want to add?", DialogType.prompt, (val) => this.addRewardPointsHelper(row, val), () => this.addRewardPointsHelper(null, 0));
  }

  addRewardPointsHelper(row?: UserEdit, points?: any) {
    if (row) {
      if (isNumeric(points)) {
        let reward = new RewardOperation();
        if (row.userRewards && row.userRewards.length > 0) {
          reward.rewardId = row.userRewards[0].id;
          reward.concurrencyStamp = row.userRewards[0].concurrencyStamp;
          reward.transactionType = "CREDIT";
          reward.description = "ADD POINTS FROM USER MANAGEMENT";
          reward.amount = points;

          this.subscription.add(this.accountService.transactReward(reward)
            .subscribe(response => {
              if (response.isSuccess) {
                this.alertService.showMessage("Success", "Successfully added reward points!", MessageSeverity.success);
              } else {
                this.alertService.showMessage("Error", response.message, MessageSeverity.error);
              }
              row.userRewards[0].balance = response.data.balance;
              row.userRewards[0].concurrencyStamp = response.data.concurrencyStamp;

            },
              error => {
                this.alertService.stopLoadingMessage();
                this.loadingIndicator = false;

                this.alertService.showStickyMessage("Add Point Error", `An error occured while addting reward points the user.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
                  MessageSeverity.error);
              }));
        } else{
          this.alertService.showStickyMessage("Add Point Error", `An error occured while addting reward points the user.\r\nError: No reward record found for this user."`,
            MessageSeverity.error);
        }
      } else {
        this.alertService.showMessage("Error", "Please enter a number.", MessageSeverity.error);
      }
    }
  }

resetPassword(row: UserEdit) {
  this.alertService.showDialog('Are you sure you want to reset the password for \"' + row.userName + '\"?', DialogType.confirm, () => this.resetPasswordHelper(row));
  }


  resetPasswordHelper(row: UserEdit) {

    this.alertService.startLoadingMessage("Resetting...");
    this.loadingIndicator = true;

    this.subscription.add(this.accountService.resetPassword(row.email)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        if (results && results.isSuccess) {
          this.alertService.showMessage('A link to reset the password has been sent to the email.');
        }
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Reset Password Error", `An error occured while resetting the password.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }


  uploadCSV() {
  }

  syncAd() {
    this.alertService.showDialog('Are you sure you want to sync user from Active Directory?', DialogType.confirm, () => this.syncAdHelper());
  }

  syncAdHelper() {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.subscription.add(this.contactGroupService.syncLdap()
      .subscribe(results => {
        this.alertService.showMessage('Users are now synced.');
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Sync Error", `An error occured while synchronizing the users.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }

  get canAssignRoles() {
    return this.accountService.userHasPermission(Permission.assignRolesPermission);
  }

  get canViewRoles() {
    return this.accountService.userHasPermission(Permission.viewRolesPermission)
  }

  get canManageUsers() {
    return this.accountService.userHasPermission(Permission.manageUsersPermission);
  }
}
