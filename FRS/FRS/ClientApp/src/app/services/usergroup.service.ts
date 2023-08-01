import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';




import { CommonEndpoint } from './common-endpoint.service';
import { AuthService } from './auth.service';
import { UserGroup } from '../models/userGroup.model';
import { Filter, PagedResult } from '../models/sieve-filter.model';
import { ConfigurationService } from './configuration.service';

export type UserGroupsChangedOperation = "add" | "delete" | "modify";
export type UserGroupsChangedEventArg = { userGroups: UserGroup[] | string[], operation: UserGroupsChangedOperation };

@Injectable()
export class UserGroupService {

  public static readonly userGroupAddedOperation: UserGroupsChangedOperation = "add";
  public static readonly userGroupDeletedOperation: UserGroupsChangedOperation = "delete";
  public static readonly userGroupModifiedOperation: UserGroupsChangedOperation = "modify";

  private _userGroupsChanged = new Subject<UserGroupsChangedEventArg>();

  private readonly _userGroupUrl: string = "/api/userGroup";
  get userGroupUrl() { return this.configurations.baseUrl + this._userGroupUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private commonEndpoint: CommonEndpoint, protected configurations: ConfigurationService) {

  }


  private onUserGroupsChanged(userGroups: UserGroup[] | string[], op: UserGroupsChangedOperation) {
    this._userGroupsChanged.next({ userGroups: userGroups, operation: op });
  }


  onUserGroupsCountChanged(userGroups: UserGroup[] | string[]) {
    return this.onUserGroupsChanged(userGroups, UserGroupService.userGroupModifiedOperation);
  }


  getUserGroupsChangedEvent(): Observable<UserGroupsChangedEventArg> {
    return this._userGroupsChanged.asObservable();
  }

  getUserGroupById(userGroupId: string) {

    return this.commonEndpoint.getById<any>(this.userGroupUrl + '/get', userGroupId);
  }

  getUserGroupsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.userGroupUrl + '/userGroups/sieve/list', filter);
  }

  updateUserGroup(userGroup: UserGroup) {
    if (userGroup.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.userGroupUrl, userGroup, userGroup.id).pipe(
        tap(data => this.onUserGroupsChanged([userGroup], UserGroupService.userGroupModifiedOperation)));
    }
  }


  newUserGroup(userGroup: UserGroup) {
    return this.commonEndpoint.getNewEndpoint<UserGroup>(this.userGroupUrl, userGroup).pipe<UserGroup>(
      tap(data => this.onUserGroupsChanged([userGroup], UserGroupService.userGroupAddedOperation)));
  }


  deleteUserGroup(userGroupOrUserGroupId: string | UserGroup): Observable<UserGroup> {

    if (typeof userGroupOrUserGroupId === 'number' || userGroupOrUserGroupId instanceof Number ||
      typeof userGroupOrUserGroupId === 'string' || userGroupOrUserGroupId instanceof String) {
      return this.commonEndpoint.getDeleteEndpoint<UserGroup>(this.userGroupUrl, <string>userGroupOrUserGroupId).pipe<UserGroup>(
        tap(data => this.onUserGroupsChanged([data], UserGroupService.userGroupDeletedOperation)));
    }
    else {

      if (userGroupOrUserGroupId.id) {
        return this.deleteUserGroup(userGroupOrUserGroupId.id);
      }
    }
  }
}
