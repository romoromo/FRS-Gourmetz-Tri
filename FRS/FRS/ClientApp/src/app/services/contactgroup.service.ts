import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';




import { CommonEndpoint } from './common-endpoint.service';
import { AuthService } from './auth.service';
import { ContactGroup } from '../models/contactgroup.model';
import { ConfigurationService } from './configuration.service';

export type ContactGroupsChangedOperation = "add" | "delete" | "modify";
export type ContactGroupsChangedEventArg = { contactgroups: ContactGroup[] | string[], operation: ContactGroupsChangedOperation };

@Injectable()
export class ContactGroupService {

  public static readonly contactgroupAddedOperation: ContactGroupsChangedOperation = "add";
  public static readonly contactgroupDeletedOperation: ContactGroupsChangedOperation = "delete";
  public static readonly contactgroupModifiedOperation: ContactGroupsChangedOperation = "modify";

  private _contactgroupsChanged = new Subject<ContactGroupsChangedEventArg>();

  private readonly _contactgroupUrl: string = "/api/contactgroup";
  get contactgroupUrl() { return this.configurations.baseUrl + this._contactgroupUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private commonEndpoint: CommonEndpoint, protected configurations: ConfigurationService) {

  }


  private onContactGroupsChanged(contactgroups: ContactGroup[] | string[], op: ContactGroupsChangedOperation) {
    this._contactgroupsChanged.next({ contactgroups: contactgroups, operation: op });
  }


  onContactGroupsCountChanged(contactgroups: ContactGroup[] | string[]) {
    return this.onContactGroupsChanged(contactgroups, ContactGroupService.contactgroupModifiedOperation);
  }


  getContactGroupsChangedEvent(): Observable<ContactGroupsChangedEventArg> {
    return this._contactgroupsChanged.asObservable();
  }

  getContactGroupById(contactgroupId: string) {

    return this.commonEndpoint.getById<ContactGroup>(this.contactgroupUrl + '/get', contactgroupId);
  }

  getContactGroups(page?: number, pageSize?: number, institutionId?: string, isSimple?: boolean) {
    return this.commonEndpoint.getPagedList<ContactGroup[]>(this.contactgroupUrl + '/contactgroups/list?institutionId=' + institutionId + '&isSimple=' + isSimple, page, pageSize);
  }

  updateContactGroup(contactgroup: ContactGroup) {
    if (contactgroup.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.contactgroupUrl, contactgroup, contactgroup.id).pipe(
        tap(data => this.onContactGroupsChanged([contactgroup], ContactGroupService.contactgroupModifiedOperation)));
    }
  }


  newContactGroup(contactgroup: ContactGroup) {
    return this.commonEndpoint.getNewEndpoint<ContactGroup>(this.contactgroupUrl, contactgroup).pipe<ContactGroup>(
      tap(data => this.onContactGroupsChanged([contactgroup], ContactGroupService.contactgroupAddedOperation)));
  }


  deleteContactGroup(contactgroupOrContactGroupId: string | ContactGroup): Observable<ContactGroup> {

    if (typeof contactgroupOrContactGroupId === 'number' || contactgroupOrContactGroupId instanceof Number ||
      typeof contactgroupOrContactGroupId === 'string' || contactgroupOrContactGroupId instanceof String) {
      return this.commonEndpoint.getDeleteEndpoint<ContactGroup>(this.contactgroupUrl, <string>contactgroupOrContactGroupId).pipe<ContactGroup>(
        tap(data => this.onContactGroupsChanged([data], ContactGroupService.contactgroupDeletedOperation)));
    }
    else {

      if (contactgroupOrContactGroupId.id) {
        return this.deleteContactGroup(contactgroupOrContactGroupId.id);
      }
    }
  }

  syncLdap() {
    return this.commonEndpoint.get<any>(this.contactgroupUrl + '/ldap/sync');
  }
}
