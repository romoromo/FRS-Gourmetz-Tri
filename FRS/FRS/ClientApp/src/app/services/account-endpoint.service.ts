import { Injectable, Injector } from '@angular/core';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { EndpointFactory } from './endpoint-factory.service';
import { ConfigurationService } from './configuration.service';
import { HttpParamsOptions } from '@angular/common/http/src/params';
import { Filter, PagedResult } from '../models/sieve-filter.model';
import { WalletOperation } from '../models/userwallet.model';
import { RewardOperation } from '../models/userreward.model';


@Injectable()
export class AccountEndpoint extends EndpointFactory {

  private readonly _usersUrl: string = "/api/account/users";
  private readonly _usersRegisterUrl: string = "/api/account/users/register";
  private readonly _userByUserNameUrl: string = "/api/account/users/username";
  private readonly _currentUserUrl: string = "/api/account/users/me";
  private readonly _currentUserPreferencesUrl: string = "/api/account/users/me/preferences";
  private readonly _unblockUserUrl: string = "/api/account/users/unblock";
  private readonly _resetUserPasswordUrl: string = "/api/account/users/forgotpassword";
  private readonly _rolesUrl: string = "/api/account/roles";
  private readonly _roleByRoleNameUrl: string = "/api/account/roles/name";
  private readonly _permissionsUrl: string = "/api/account/permissions";
  private readonly _permissionsTreeUrl: string = "/api/account/permissionstree";
  private readonly _userPhonebooksUrl: string = "/api/account/phonebooks";
  private readonly _userVehiclesUrl: string = "/api/account/vehicles";
  private readonly _userCardIdsUrl: string = "/api/account/cardids";
  private readonly _configUrl: string = "/api/configuration";
  private readonly _walletTransactUrl: string = "/api/account/wallet/operation";
  private readonly _rewardTransactUrl: string = "/api/account/reward/operation";
  private readonly _walletUrl: string = "/api/account/wallet";

  get usersRegisterUrl() { return this.configurations.baseUrl + this._usersRegisterUrl; }
  get usersUrl() { return this.configurations.baseUrl + this._usersUrl; }
  get userByUserNameUrl() { return this.configurations.baseUrl + this._userByUserNameUrl; }
  get currentUserUrl() { return this.configurations.baseUrl + this._currentUserUrl; }
  get currentUserPreferencesUrl() { return this.configurations.baseUrl + this._currentUserPreferencesUrl; }
  get unblockUserUrl() { return this.configurations.baseUrl + this._unblockUserUrl; }
  get resetUserPasswordUrl() { return this.configurations.baseUrl + this._resetUserPasswordUrl; }
  get rolesUrl() { return this.configurations.baseUrl + this._rolesUrl; }
  get roleByRoleNameUrl() { return this.configurations.baseUrl + this._roleByRoleNameUrl; }
  get permissionsUrl() { return this.configurations.baseUrl + this._permissionsUrl; }
  get permissionsTreeUrl() { return this.configurations.baseUrl + this._permissionsTreeUrl; }
  get userPhonebooksUrl() { return this.configurations.baseUrl + this._userPhonebooksUrl; }
  get userVehiclesUrl() { return this.configurations.baseUrl + this._userVehiclesUrl; }
  get userCardIdsUrl() { return this.configurations.baseUrl + this._userCardIdsUrl; }
  get configUrl() { return this.configurations.baseUrl + this._configUrl; }
  get walletTransactUrl() { return this.configurations.baseUrl + this._walletTransactUrl; }
  get walletByUserUrl() { return this.configurations.baseUrl + this._walletUrl + '/user'; }
  get rewardTransactUrl() { return this.configurations.baseUrl + this._rewardTransactUrl; }

  constructor(http: HttpClient, public configurations: ConfigurationService, injector: Injector) {

    super(http, configurations, injector);
  }


  loadConfig<T>(): Observable<T> {
    return this.http.get<T>(this.configUrl, this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.loadConfig());
      }));
  }

  getUserEndpoint<T>(userId?: string): Observable<T> {
    let endpointUrl = userId ? `${this.usersUrl}/${userId}` : this.currentUserUrl;

    return this.http.get<T>(endpointUrl, this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getUserEndpoint(userId));
      }));
  }


  getUserByUserNameEndpoint<T>(userName: string): Observable<T> {
    let endpointUrl = `${this.userByUserNameUrl}/${userName}`;

    return this.http.get<T>(endpointUrl, this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getUserByUserNameEndpoint(userName));
      }));
  }


  getUsersEndpoint<T>(page?: number, pageSize?: number, institutionId?: string): Observable<T> {
    let endpointUrl = page && pageSize ? `${this.usersUrl}/${page}/${pageSize}` : `${this.usersUrl}?institutionId=${institutionId}`;

    return this.http.get<T>(endpointUrl, this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getUsersEndpoint(page, pageSize, institutionId));
      }));
  }

  getUsersSieveEndpoint<PagedResult>(filter?: Filter): Observable<PagedResult> {
    return this.getSieve<PagedResult>(this.usersUrl + '/sieve/list', filter);
  }

  getNewUserEndpoint<T>(userObject: any): Observable<T> {

    return this.http.post<T>(this.usersUrl, JSON.stringify(userObject), this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getNewUserEndpoint(userObject));
      }));
  }

  getRegisterUserEndpoint<T>(userObject: any): Observable<T> {

    return this.http.post<T>(this.usersRegisterUrl, JSON.stringify(userObject), this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getRegisterUserEndpoint(userObject));
      }));
  }

  getUpdateUserEndpoint<T>(userObject: any, userId?: string): Observable<T> {
    let endpointUrl = userId ? `${this.usersUrl}/${userId}` : this.currentUserUrl;

    return this.http.put<T>(endpointUrl, JSON.stringify(userObject), this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getUpdateUserEndpoint(userObject, userId));
      }));
  }

  getChangePasswordUserEndpoint<T>(userObject: any, userId?: string): Observable<T> {
    let endpointUrl = userId ? `${this.usersUrl}/${userId}/changepassword` : this.currentUserUrl;

    return this.http.put<T>(endpointUrl, JSON.stringify(userObject), this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getUpdateUserEndpoint(userObject, userId));
      }));
  }

  getPatchUpdateUserEndpoint<T>(patch: {}, userId?: string): Observable<T>
  getPatchUpdateUserEndpoint<T>(value: any, op: string, path: string, from?: any, userId?: string): Observable<T>
  getPatchUpdateUserEndpoint<T>(valueOrPatch: any, opOrUserId?: string, path?: string, from?: any, userId?: string): Observable<T> {
    let endpointUrl: string;
    let patchDocument: {};

    if (path) {
      endpointUrl = userId ? `${this.usersUrl}/${userId}` : this.currentUserUrl;
      patchDocument = from ?
        [{ "value": valueOrPatch, "path": path, "op": opOrUserId, "from": from }] :
        [{ "value": valueOrPatch, "path": path, "op": opOrUserId }];
    }
    else {
      endpointUrl = opOrUserId ? `${this.usersUrl}/${opOrUserId}` : this.currentUserUrl;
      patchDocument = valueOrPatch;
    }

    return this.http.patch<T>(endpointUrl, JSON.stringify(patchDocument), this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getPatchUpdateUserEndpoint(valueOrPatch, opOrUserId, path, from, userId));
      }));
  }


  getUserPreferencesEndpoint<T>(): Observable<T> {

    return this.http.get<T>(this.currentUserPreferencesUrl, this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getUserPreferencesEndpoint());
      }));
  }

  getUpdateUserPreferencesEndpoint<T>(configuration: string): Observable<T> {
    return this.http.put<T>(this.currentUserPreferencesUrl, JSON.stringify(configuration), this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getUpdateUserPreferencesEndpoint(configuration));
      }));
  }

  getUnblockUserEndpoint<T>(userId: string): Observable<T> {
    let endpointUrl = `${this.unblockUserUrl}/${userId}`;

    return this.http.put<T>(endpointUrl, null, this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getUnblockUserEndpoint(userId));
      }));
  }

  getResetUserEndpoint<T>(email: string): Observable<T> {
    let endpointUrl = `${this.resetUserPasswordUrl}`;

    return this.http.post<T>(endpointUrl, JSON.stringify(email), this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getResetUserEndpoint(email));
      }));
  }

  getDeleteUserEndpoint<T>(userId: string): Observable<T> {
    let endpointUrl = `${this.usersUrl}/${userId}`;

    return this.http.delete<T>(endpointUrl, this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getDeleteUserEndpoint(userId));
      }));
  }

  getTrueDeleteUserEndpoint<T>(userId: string): Observable<T> {
    let endpointUrl = `${this.usersUrl}/true/${userId}`;

    return this.http.delete<T>(endpointUrl, this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getTrueDeleteUserEndpoint(userId));
      }));
  }





  getRoleEndpoint<T>(roleId: string): Observable<T> {
    let endpointUrl = `${this.rolesUrl}/${roleId}`;

    return this.http.get<T>(endpointUrl, this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getRoleEndpoint(roleId));
      }));
  }


  getRoleByRoleNameEndpoint<T>(roleName: string): Observable<T> {
    let endpointUrl = `${this.roleByRoleNameUrl}/${roleName}`;

    return this.http.get<T>(endpointUrl, this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getRoleByRoleNameEndpoint(roleName));
      }));
  }



  getRolesEndpoint<T>(page?: number, pageSize?: number, institutionId?: string): Observable<T> {
    let endpointUrl = page && pageSize ? `${this.rolesUrl}/${page}/${pageSize}` : `${this.rolesUrl}?institutionId=${institutionId}`;

    return this.http.get<T>(endpointUrl, this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getRolesEndpoint(page, pageSize, institutionId));
      }));
  }

  getRolesSieveEndpoint<PagedResult>(filter?: Filter): Observable<PagedResult> {
    return this.getSieve<PagedResult>(this.rolesUrl + '/sieve/list', filter);
  }

  getNewRoleEndpoint<T>(roleObject: any): Observable<T> {

    return this.http.post<T>(this.rolesUrl, JSON.stringify(roleObject), this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getNewRoleEndpoint(roleObject));
      }));
  }

  getUpdateRoleEndpoint<T>(roleObject: any, roleId: string): Observable<T> {
    let endpointUrl = `${this.rolesUrl}/${roleId}`;

    return this.http.put<T>(endpointUrl, JSON.stringify(roleObject), this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getUpdateRoleEndpoint(roleObject, roleId));
      }));
  }

  getDeleteRoleEndpoint<T>(roleId: string): Observable<T> {
    let endpointUrl = `${this.rolesUrl}/${roleId}`;

    return this.http.delete<T>(endpointUrl, this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getDeleteRoleEndpoint(roleId));
      }));
  }


  getPermissionsEndpoint<T>(): Observable<T> {

    return this.http.get<T>(this.permissionsUrl, this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getPermissionsEndpoint());
      }));
  }

  getPermissionsTreeEndpoint<T>(): Observable<T> {

    return this.http.get<T>(this.permissionsTreeUrl, this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getPermissionsTreeEndpoint());
      }));
  }

  //phonebooks
  getUserPhonebooksEndpoint<T>(userId?: string, page?: number, pageSize?: number): Observable<T> {
    let endpointUrl = page && pageSize ? `${this.userPhonebooksUrl}/list/${page}/${pageSize}?userId=${userId}` : `${this.userPhonebooksUrl}/list?userId=${userId}`;

    return this.http.get<T>(endpointUrl, this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getUserPhonebooksEndpoint(userId, page, pageSize));
      }));
  }

  getNewUserPhonebookEndpoint<T>(userPhonebookObject: any): Observable<T> {

    return this.http.post<T>(this.userPhonebooksUrl, JSON.stringify(userPhonebookObject), this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getNewUserPhonebookEndpoint(userPhonebookObject));
      }));
  }

  getUpdateUserPhonebookEndpoint<T>(userPhonebookObject: any, userPhonebookId: string): Observable<T> {
    let endpointUrl = `${this.userPhonebooksUrl}/update/${userPhonebookId}`;

    return this.http.put<T>(endpointUrl, JSON.stringify(userPhonebookObject), this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getUpdateUserPhonebookEndpoint(userPhonebookObject, userPhonebookId));
      }));
  }

  getDeleteUserPhonebookEndpoint<T>(userPhonebookId: string): Observable<T> {
    let endpointUrl = `${this.userPhonebooksUrl}/delete/${userPhonebookId}`;

    return this.http.delete<T>(endpointUrl, this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getDeleteUserPhonebookEndpoint(userPhonebookId));
      }));
  }

  //vehicles
  getUserVehiclesEndpoint<T>(userId?: string, page?: number, pageSize?: number): Observable<T> {
    let endpointUrl = page && pageSize ? `${this.userVehiclesUrl}/list/${page}/${pageSize}?userId=${userId}` : `${this.userVehiclesUrl}/list?userId=${userId}`;

    return this.http.get<T>(endpointUrl, this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getUserPhonebooksEndpoint(userId, page, pageSize));
      }));
  }

  getNewUserVehicleEndpoint<T>(obj: any): Observable<T> {

    return this.http.post<T>(this.userVehiclesUrl, JSON.stringify(obj), this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getNewUserPhonebookEndpoint(obj));
      }));
  }

  getUpdateUserVehicleEndpoint<T>(obj: any, id: string): Observable<T> {
    let endpointUrl = `${this.userVehiclesUrl}/update/${id}`;

    return this.http.put<T>(endpointUrl, JSON.stringify(obj), this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getUpdateUserVehicleEndpoint(obj, id));
      }));
  }

  getDeleteUserVehicleEndpoint<T>(id: string): Observable<T> {
    let endpointUrl = `${this.userVehiclesUrl}/delete/${id}`;

    return this.http.delete<T>(endpointUrl, this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getDeleteUserVehicleEndpoint(id));
      }));
  }

  //card ids
  getUserCardIdsEndpoint<T>(userId?: string, page?: number, pageSize?: number): Observable<T> {
    let endpointUrl = page && pageSize ? `${this.userCardIdsUrl}/list/${page}/${pageSize}?userId=${userId}` : `${this.userCardIdsUrl}/list?userId=${userId}`;

    return this.http.get<T>(endpointUrl, this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getUserPhonebooksEndpoint(userId, page, pageSize));
      }));
  }

  getNewUserCardIdEndpoint<T>(obj: any): Observable<T> {

    return this.http.post<T>(this.userCardIdsUrl, JSON.stringify(obj), this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getNewUserPhonebookEndpoint(obj));
      }));
  }

  getNewUserCardIdActivateEndpoint<T>(obj: any): Observable<T> {

    return this.http.post<T>(this.userCardIdsUrl, JSON.stringify(obj), this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getNewUserPhonebookEndpoint(obj));
      }));
  }

  getUpdateUserCardIdEndpoint<T>(obj: any, id: string): Observable<T> {
    let endpointUrl = `${this.userCardIdsUrl}/update/${id}`;

    return this.http.put<T>(endpointUrl, JSON.stringify(obj), this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getUpdateUserCardIdEndpoint(obj, id));
      }));
  }

  getDeleteUserCardIdEndpoint<T>(id: string): Observable<T> {
    let endpointUrl = `${this.userCardIdsUrl}/delete/${id}`;

    return this.http.delete<T>(endpointUrl, this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getDeleteUserCardIdEndpoint(id));
      }));
  }

  getWalletByUserId<T>(userId: string): Observable<T> {
    let endpointUrl = `${this.walletByUserUrl}/${userId}`;

    return this.http.get<T>(endpointUrl, this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getWalletByUserId(userId));
      }));
  }

  getTransactWalletEndpoint<T>(model: WalletOperation): Observable<T> {
    let endpointUrl = `${this.walletTransactUrl}/${model.walletId}`;

    return this.http.put<T>(endpointUrl, JSON.stringify(model), this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getTransactWalletEndpoint(model,));
      }));
  }

  getTransactRewardEndpoint<T>(model: RewardOperation): Observable<T> {
    let endpointUrl = `${this.rewardTransactUrl}/${model.rewardId}`;

    return this.http.put<T>(endpointUrl, JSON.stringify(model), this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getTransactRewardEndpoint(model,));
      }));
  }

  getSieve<T>(url: string, filter: any): Observable<T> {
    const headers = this.getRequestHeaders();

    const httpParams: HttpParamsOptions = { fromObject: filter } as HttpParamsOptions;

    const options = { params: new HttpParams(httpParams), headers: headers.headers };

    return this.http.get<T>(url, options).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getSieve(url, filter));
      }));
  }
}
