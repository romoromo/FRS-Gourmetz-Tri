import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';




import { CommonEndpoint } from './common-endpoint.service';
import { AuthService } from './auth.service';
import { KioskSettings } from '../models/kiosk-settings.model';
import { ConfigurationService } from './configuration.service';

//export type ContactGroupsChangedOperation = "add" | "delete" | "modify";
//export type ContactGroupsChangedEventArg = { contactgroups: ContactGroup[] | string[], operation: ContactGroupsChangedOperation };

@Injectable()
export class KioskSettingsService {

  //public static readonly contactgroupAddedOperation: ContactGroupsChangedOperation = "add";
  //public static readonly contactgroupDeletedOperation: ContactGroupsChangedOperation = "delete";
  //public static readonly contactgroupModifiedOperation: ContactGroupsChangedOperation = "modify";

  //private _contactgroupsChanged = new Subject<ContactGroupsChangedEventArg>();

  private readonly _kioskSettingsUrl: string = "/api/kioskSettings";
  get kioskSettingsUrl() { return this.configurations.baseUrl + this._kioskSettingsUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private commonEndpoint: CommonEndpoint, private configurations: ConfigurationService) {

  }


  //private onContactGroupsChanged(contactgroups: ContactGroup[] | string[], op: ContactGroupsChangedOperation) {
  //  this._contactgroupsChanged.next({ contactgroups: contactgroups, operation: op });
  //}


  //onContactGroupsCountChanged(contactgroups: ContactGroup[] | string[]) {
  //  return this.onContactGroupsChanged(contactgroups, ContactGroupService.contactgroupModifiedOperation);
  //}


  //getContactGroupsChangedEvent(): Observable<ContactGroupsChangedEventArg> {
  //  return this._contactgroupsChanged.asObservable();
  //}

  getKioskSettingsById(kioskSettingsId: string) {

    return this.commonEndpoint.getById<any>(this.kioskSettingsUrl, kioskSettingsId);
  }

  getKioskSettings(page?: number, pageSize?: number, institutionId?: string) {
    return this.commonEndpoint.getPagedList<KioskSettings[]>(this.kioskSettingsUrl + '/kioskSettings/list?institutionId=' + institutionId, page, pageSize);
  }

  updateKioskSettings(kioskSettings: KioskSettings) {
    if (kioskSettings.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.kioskSettingsUrl, kioskSettings, kioskSettings.id);
    }
  }


  newKioskSettings(kioskSettings: KioskSettings) {
    return this.commonEndpoint.getNewEndpoint<KioskSettings>(this.kioskSettingsUrl, kioskSettings);
  }


  deletekioskSettings(kioskSettingsOrKioskSettingsId: string | KioskSettings): Observable<KioskSettings> {

    if (typeof kioskSettingsOrKioskSettingsId === 'number' || kioskSettingsOrKioskSettingsId instanceof Number ||
      typeof kioskSettingsOrKioskSettingsId === 'string' || kioskSettingsOrKioskSettingsId instanceof String) {
      return this.commonEndpoint.getDeleteEndpoint<KioskSettings>(this.kioskSettingsUrl, <string>kioskSettingsOrKioskSettingsId);
    }
    else {

      if (kioskSettingsOrKioskSettingsId.id) {
        return this.deletekioskSettings(kioskSettingsOrKioskSettingsId.id);
      }
    }
  }
}
