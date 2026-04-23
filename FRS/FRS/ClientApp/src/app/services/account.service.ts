import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';


import { AccountEndpoint } from './account-endpoint.service';
import { AuthService } from './auth.service';
import { User } from '../models/user.model';
import { Role } from '../models/role.model';
import { Permission, PermissionNames, PermissionTree, PermissionValues } from '../models/permission.model';
import { UserEdit } from '../models/user-edit.model';
import { Facility } from '../models/facility.model';
import { DepartmentService } from './department.service';
import { Department } from '../models/department.model';
import { UserPhonebook } from '../models/userphonebook.model';
import * as coreSignalR from '@aspnet/signalr';
import { HubConnection } from '@aspnet/signalr';
import * as signalR from '@aspnet/signalr';
import { UserVehicle } from '../models/uservehicle.model';
import { UserCardId } from '../models/usercardid.model';
import { CommonFilter, Filter, PagedResult, RoleReportFilter } from '../models/sieve-filter.model';
import { WalletOperation } from '../models/userwallet.model';
import { RewardOperation } from '../models/userreward.model';
import { CommonEndpoint } from './common-endpoint.service';
import { ConfigurationService } from './configuration.service';

export type RolesChangedOperation = "add" | "delete" | "modify";
export type RolesChangedEventArg = { roles: Role[] | string[], operation: RolesChangedOperation };

@Injectable()
export class AccountService {

 public static readonly roleAddedOperation: RolesChangedOperation = "add";
  public static readonly roleDeletedOperation: RolesChangedOperation = "delete";
  public static readonly roleModifiedOperation: RolesChangedOperation = "modify";

  private _rolesChanged = new Subject<RolesChangedEventArg>();
  private connection: coreSignalR.HubConnection;
  private connections = {};
  private pibConnection: signalR.HubConnection;
  private readonly _accountUserUrl: string = "/api/account/users";
  get accountUserUrl() { return this.configurations.baseUrl + this._accountUserUrl; }

  private readonly _accountRoleUrl: string = "/api/account/roles";
  get accountRoleUrl() { return this.configurations.baseUrl + this._accountRoleUrl; }
  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private accountEndpoint: AccountEndpoint, private departmentService: DepartmentService, private commonEndpoint: CommonEndpoint,
    public configurations: ConfigurationService  ) {

  }

  loadConfig() {
    return this.accountEndpoint.loadConfig<any>();
  }

  getUser(userId?: string) {
    return this.accountEndpoint.getUserEndpoint<User>(userId);
  }

  getUserAndRoles(userId?: string, institutionId?: string) {
    return forkJoin(
      this.accountEndpoint.getUserEndpoint<User>(userId),
      this.accountEndpoint.getRolesEndpoint<Role[]>(null, null, institutionId),
      this.departmentService.getDepartments(null, null, institutionId),
      this.accountEndpoint.getUserPhonebooksEndpoint<UserPhonebook[]>());
  }

  getUsers(page?: number, pageSize?: number, institutionId?: string) {

    return this.accountEndpoint.getUsersEndpoint<User[]>(page, pageSize, institutionId);
  }

  getUsersAndRoles(page?: number, pageSize?: number, institutionId?: string) {

    return forkJoin(
      this.accountEndpoint.getUsersEndpoint<User[]>(page, pageSize, institutionId),
      this.accountEndpoint.getRolesEndpoint<Role[]>(null, null, institutionId),
      this.departmentService.getDepartments(null, null, institutionId),
      this.accountEndpoint.getUserPhonebooksEndpoint<UserPhonebook[]>());
  }

  getUsersByFilter(userFilter: CommonFilter) {

    return this.accountEndpoint.getUsersSieveEndpoint<PagedResult>(userFilter);
  }

  getUsersAndRolesByFilters(userFilter: CommonFilter) {

    return forkJoin(
      this.accountEndpoint.getUsersSieveEndpoint<PagedResult>(userFilter),
      this.accountEndpoint.getRolesEndpoint<Role[]>(null, null, userFilter.institutionId),
      this.departmentService.getDepartments(null, null, userFilter.institutionId),
      this.accountEndpoint.getUserPhonebooksEndpoint<UserPhonebook[]>());
  }

  updateUser(user: UserEdit) {
    if (user.id) {
      return this.accountEndpoint.getUpdateUserEndpoint<User>(user, user.id);
    }
    else {
      return this.accountEndpoint.getUserByUserNameEndpoint<User>(user.userName).pipe<User>(
        mergeMap(foundUser => {
          user.id = foundUser.id;
          return this.accountEndpoint.getUpdateUserEndpoint<User>(user, user.id)
        }));
    }
  }

  changePassowrd(userId: string, currentPassword: string, newPassword) {
    return this.accountEndpoint.getUpdateUserEndpoint<any>({ currentPassword: currentPassword, newPassword: newPassword }, userId);
  }

  newUser(user: UserEdit) {
    return this.accountEndpoint.getNewUserEndpoint<User>(user);
  }

  registerUser(user: UserEdit) {
    return this.accountEndpoint.getRegisterUserEndpoint<User>(user);
  }

  getUserPreferences() {
    return this.accountEndpoint.getUserPreferencesEndpoint<string>();
  }

  updateUserPreferences(configuration: string) {
    return this.accountEndpoint.getUpdateUserPreferencesEndpoint(configuration);
  }


  deleteUser(userOrUserId: string | UserEdit): Observable<User> {

    if (typeof userOrUserId === 'string' || userOrUserId instanceof String ||
      typeof userOrUserId === 'number' || userOrUserId instanceof Number) {
      return this.accountEndpoint.getDeleteUserEndpoint<User>(<string>userOrUserId).pipe<User>(
        tap(data => this.onRolesUserCountChanged(data.roles)));
    }
    else {

      if (userOrUserId.id) {
        return this.deleteUser(userOrUserId.id);
      }
      else {
        return this.accountEndpoint.getUserByUserNameEndpoint<User>(userOrUserId.userName).pipe<User>(
          mergeMap(user => this.deleteUser(user.id)));
      }
    }
  }

  trueDeleteUser(userOrUserId: string | UserEdit): Observable<User> {

    if (typeof userOrUserId === 'string' || userOrUserId instanceof String ||
      typeof userOrUserId === 'number' || userOrUserId instanceof Number) {
      return this.accountEndpoint.getTrueDeleteUserEndpoint<User>(<string>userOrUserId).pipe<User>(
        tap(data => this.onRolesUserCountChanged(data.roles)));
    }
    else {

      if (userOrUserId.id) {
        return this.trueDeleteUser(userOrUserId.id);
      }
      else {
        return this.accountEndpoint.getUserByUserNameEndpoint<User>(userOrUserId.userName).pipe<User>(
          mergeMap(user => this.trueDeleteUser(user.id)));
      }
    }
  }

  resetPassword(userId: string) {
    return this.accountEndpoint.getResetUserEndpoint<any>(userId);
  }


  unblockUser(userId: string) {
    return this.accountEndpoint.getUnblockUserEndpoint(userId);
  }

  changePassword(userEdit: UserEdit, userId: string) {
    return this.accountEndpoint.getChangePasswordUserEndpoint(userEdit, userId);
  }
  
  getUserReportByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.accountUserUrl + '/report/sieve/list', filter);
  }

  downloadUserReport(filter: Filter) {
    return this.commonEndpoint.getFile<any>(this.accountUserUrl + '/report/export', filter);
  }

  //role report
  getUserRoleReportByFilter(filter: RoleReportFilter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.accountRoleUrl + '/report/sieve/list', filter);
  }

  downloadUserRoleReport(filter: Filter) {
    return this.commonEndpoint.getFile<any>(this.accountRoleUrl + '/report/export', filter);
  }

  getWallet(userId) {

    return this.accountEndpoint.getWalletByUserId<any>(userId);
  }

  transactWallet(model: WalletOperation) {
    return this.accountEndpoint.getTransactWalletEndpoint<any>(model);
  }

  transactReward(model: RewardOperation) {
    return this.accountEndpoint.getTransactRewardEndpoint<any>(model);
  }

  userHasPermission(permissionValue: PermissionValues): boolean {
    return this.permissions.some(p => p == permissionValue);
  }

  userHasPermissions(permissionValues: PermissionValues[]): boolean {
    let isAllowed = true; //default allow menu as some have no acl yet
    if (permissionValues) {
      for (let pv of permissionValues) {
        isAllowed = this.permissions.some(p => p == pv);
        if (!isAllowed) break;
      }
    }
    

    return isAllowed;
  }

  refreshLoggedInUser() {
    return this.authService.refreshLogin();
  }




  getRoles(page?: number, pageSize?: number, institutionId?: string) {

    return this.accountEndpoint.getRolesEndpoint<Role[]>(page, pageSize, institutionId);
  }

  getRolesAndPermissionsByFilters(filter: CommonFilter) {

    return forkJoin(
      this.accountEndpoint.getRolesSieveEndpoint<PagedResult>(filter),
      this.accountEndpoint.getPermissionsTreeEndpoint<any>());
  }

  getRolesAndPermissionListByFilters(filter: CommonFilter) {

    return forkJoin(
      this.accountEndpoint.getRolesSieveEndpoint<PagedResult>(filter),
      this.accountEndpoint.getPermissionsEndpoint<any>());
  }

  getRolesAndPermissions(page?: number, pageSize?: number, institutionId?: string) {

    return forkJoin(
      this.accountEndpoint.getPermissionsEndpoint<Permission[]>(),
      this.accountEndpoint.getPermissionsTreeEndpoint<any>());
  }


  updateRole(role: Role) {
    if (role.id) {
      return this.accountEndpoint.getUpdateRoleEndpoint(role, role.id).pipe(
        tap(data => this.onRolesChanged([role], AccountService.roleModifiedOperation)));
    }
    else {
      return this.accountEndpoint.getRoleByRoleNameEndpoint<Role>(role.name).pipe(
        mergeMap(foundRole => {
          role.id = foundRole.id;
          return this.accountEndpoint.getUpdateRoleEndpoint(role, role.id)
        }),
        tap(data => this.onRolesChanged([role], AccountService.roleModifiedOperation)));
    }
  }


  newRole(role: Role) {
    return this.accountEndpoint.getNewRoleEndpoint<Role>(role).pipe<Role>(
      tap(data => this.onRolesChanged([role], AccountService.roleAddedOperation)));
  }


  deleteRole(roleOrRoleId: string | Role): Observable<Role> {

    if (typeof roleOrRoleId === 'number' || typeof roleOrRoleId === 'string' || roleOrRoleId instanceof String) {
      return this.accountEndpoint.getDeleteRoleEndpoint<Role>(<string>roleOrRoleId).pipe<Role>(
        tap(data => this.onRolesChanged([data], AccountService.roleDeletedOperation)));
    }
    else {

      if (roleOrRoleId.id) {
        return this.deleteRole(roleOrRoleId.id);
      }
      else {
        return this.accountEndpoint.getRoleByRoleNameEndpoint<Role>(roleOrRoleId.name).pipe<Role>(
          mergeMap(role => this.deleteRole(role.id)));
      }
    }
  }

  getPermissions() {

    return this.accountEndpoint.getPermissionsEndpoint<Permission[]>();
  }


  private onRolesChanged(roles: Role[] | string[], op: RolesChangedOperation) {
    this._rolesChanged.next({ roles: roles, operation: op });
  }


  onRolesUserCountChanged(roles: Role[] | string[]) {
    return this.onRolesChanged(roles, AccountService.roleModifiedOperation);
  }


  getRolesChangedEvent(): Observable<RolesChangedEventArg> {
    return this._rolesChanged.asObservable();
  }



  get permissions(): PermissionValues[] {
    return this.authService.userPermissions;
  }

  get currentUser() {
    return this.authService.currentUser;
  }

  signalRConnection(url: string, skipAuth?: boolean): coreSignalR.HubConnection {
    let connection: coreSignalR.HubConnection;
    console.log(this.connections);
    //if (this.connections.hasOwnProperty(url)) {
    //  connection = this.connections[url];
    //}
    //else {
    if (this.authService.currentUser != null || skipAuth) {
      connection = new coreSignalR.HubConnectionBuilder()
        .withUrl(url, {
          transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling
        })
        .configureLogging(coreSignalR.LogLevel.Trace)
        .configureLogging({
          log: function (logLevel, message) {
            console.log(url + " - " + new Date().toISOString() + ": " + message);
          }
        })
        .build();

      this.connections[url] = connection;
    }
    //}

    if (connection.state !== coreSignalR.HubConnectionState.Connected) {
      connection.start().catch(err => {
        console.log(err);
      });
    }
    return connection;
  }

  disconnectSignalRConnection(connection: coreSignalR.HubConnection, url: string) {
    if (connection != null) {
      connection.stop().catch(err => {
        console.log(err);
      });

      if (this.connections.hasOwnProperty(url)) {
        //connection = this.connections[url];
        delete this.connections[url];
      }

    }
  }

  get pibSignalRConnection(): HubConnection {
    if (this.authService.currentUser != null) {
      if (this.pibConnection == null) {
        this.pibConnection = new signalR.HubConnectionBuilder()
          .withUrl("http://localhost:91/signalr/patientHub", {
            transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling
          })
          .build();

      }
      if (this.pibConnection.state !== signalR.HubConnectionState.Connected) {
        this.pibConnection.start().catch(err => {
          console.log(err);
        });
      }

      return this.pibConnection;
    }
    return null;
  }

  getUserPhonebooks(userId?: string, page?: number, pageSize?: number) {

    return this.accountEndpoint.getUserPhonebooksEndpoint<UserPhonebook[]>(userId, page, pageSize);
  }

  updateUserPhonebook(userPhonebook: UserPhonebook) {
    if (userPhonebook.id) {
      return this.accountEndpoint.getUpdateUserPhonebookEndpoint(userPhonebook, userPhonebook.id);
    }
  }


  newUserPhonebook(userPhonebook: UserPhonebook) {
    return this.accountEndpoint.getNewUserPhonebookEndpoint<UserPhonebook>(userPhonebook);
  }


  deleteUserPhonebook(userPhonebookOrUserPhonebookId: string | UserPhonebook): Observable<UserPhonebook> {
    return this.accountEndpoint.getDeleteUserPhonebookEndpoint<UserPhonebook>(<string>userPhonebookOrUserPhonebookId);
  }

  getUserVehicles(userId?: string, page?: number, pageSize?: number) {

    return this.accountEndpoint.getUserVehiclesEndpoint<UserVehicle[]>(userId, page, pageSize);
  }

  newUserVehicle(vehicle: UserVehicle) {
    return this.accountEndpoint.getNewUserVehicleEndpoint<UserVehicle>(vehicle);
  }

  updateUserVehicle(vehicle: UserVehicle) {
    if (vehicle.id) {
      return this.accountEndpoint.getUpdateUserVehicleEndpoint<UserVehicle>(vehicle, vehicle.id);
    }
  }

  deleteUserVehicle(id: string | UserVehicle): Observable<UserVehicle> {
    return this.accountEndpoint.getDeleteUserVehicleEndpoint<UserVehicle>(<string>id);
  }

  getUserCardIds(userId?: string, page?: number, pageSize?: number) {

    return this.accountEndpoint.getUserCardIdsEndpoint<UserCardId[]>(userId, page, pageSize);
  }

  newUserCardId(cardId: UserCardId) {
    return this.accountEndpoint.getNewUserCardIdEndpoint<UserCardId>(cardId);
  }

  updateUserCardId(cardId: UserCardId) {
    if (cardId.id) {
      return this.accountEndpoint.getUpdateUserCardIdEndpoint<UserVehicle>(cardId, cardId.id);
    }
  }

  deleteUserCardId(id: string | UserCardId): Observable<UserCardId> {
    return this.accountEndpoint.getDeleteUserCardIdEndpoint<UserCardId>(<string>id);
  }
}
