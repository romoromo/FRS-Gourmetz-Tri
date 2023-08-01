import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap, catchError } from 'rxjs/operators';



import { CommonEndpoint } from './common-endpoint.service';
import { AccountEndpoint } from './account-endpoint.service';
import { AuthService } from './auth.service';
import { EmsProfile } from '../models/ems-profile.model';
import { ConfigurationService } from './configuration.service';

export type EmsProfilesChangedOperation = "add" | "delete" | "modify";
export type EmsProfilesChangedEventArg = { emsProfiles: EmsProfile[] | string[], operation: EmsProfilesChangedOperation };


@Injectable()
export class EmsProfileService {

  public static readonly emsProfileAddedOperation: EmsProfilesChangedOperation = "add";
  public static readonly emsProfileDeletedOperation: EmsProfilesChangedOperation = "delete";
  public static readonly emsProfileModifiedOperation: EmsProfilesChangedOperation = "modify";
  private readonly _emsProfileUrl: string = "/api/emsprofile";

  private _emsProfilesChanged = new Subject<EmsProfilesChangedEventArg>();

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private accountEndpoint: AccountEndpoint, private commonEndpoint: CommonEndpoint,
    protected configurations: ConfigurationService ) {

  }

  get emsProfileUrl() { return this._emsProfileUrl; }

  private onEmsProfilesChanged(emsProfiles: EmsProfile[] | string[], op: EmsProfilesChangedOperation) {
    this._emsProfilesChanged.next({ emsProfiles: emsProfiles, operation: op });
  }


  onEmsProfilesUserCountChanged(emsProfiles: EmsProfile[] | string[]) {
    return this.onEmsProfilesChanged(emsProfiles, EmsProfileService.emsProfileModifiedOperation);
  }


  getEmsProfilesChangedEvent(): Observable<EmsProfilesChangedEventArg> {
    return this._emsProfilesChanged.asObservable();
  }



  getEmsProfiles(page?: number, pageSize?: number, institutionId?: string) {

    return forkJoin(
      this.commonEndpoint.getPagedList<EmsProfile[]>(this.emsProfileUrl + '/GetAllEmsProfiles', page, pageSize, true, { institutionId: institutionId }));
  }


  getAllEmsProfiles(page?: number, pageSize?: number, institutionId?: string) {

    return this.commonEndpoint.getPagedList<EmsProfile[]>(this.emsProfileUrl + '/get/emsprofiles', page, pageSize, false, { institutionId: institutionId });
  }
  getEmsProfile(id?: string) {
    return this.commonEndpoint.get<any>(this.emsProfileUrl + '/get' + "?id=" + id);
  }

  //getEmsProfiles(page?: number, pageSize?: number) {
  //  return this.commonEndpoint.getPagedList<EmsProfile[]>(this.emsProfileUrl + '/GetAllEmsProfiles', page, pageSize, true);
  //}


  updateEmsProfile(emsProfile: EmsProfile) {
    if (emsProfile.id) {
      return this.commonEndpoint.getUpdateEndpoint<any>(this.emsProfileUrl, emsProfile, emsProfile.id).pipe(
        tap(data => this.onEmsProfilesChanged([emsProfile], EmsProfileService.emsProfileModifiedOperation)));
    }
  }


  newEmsProfile(emsProfile: EmsProfile) {
    return this.commonEndpoint.getNewEndpoint<EmsProfile>(this.emsProfileUrl, emsProfile).pipe<EmsProfile>(
      tap(data => this.onEmsProfilesChanged([emsProfile], EmsProfileService.emsProfileAddedOperation)));
  }


  deleteEmsProfile(emsProfileOrEmsProfileId: string | EmsProfile): Observable<EmsProfile> {

    if (typeof emsProfileOrEmsProfileId === 'number' || emsProfileOrEmsProfileId instanceof Number ||
      typeof emsProfileOrEmsProfileId === 'string' || emsProfileOrEmsProfileId instanceof String
      || typeof emsProfileOrEmsProfileId == 'object') {
      var queryStrings = "";
      var id = "";

      if (typeof emsProfileOrEmsProfileId == 'object') {
        id = emsProfileOrEmsProfileId.id;
      } else {
        id = emsProfileOrEmsProfileId;
      }

      return this.commonEndpoint.getDeleteEndpoint<EmsProfile>(this.emsProfileUrl, id, queryStrings).pipe<EmsProfile>(
        tap(data => this.onEmsProfilesChanged([data], EmsProfileService.emsProfileDeletedOperation)));
    }
    //else {

    //  if (emsProfileOrEmsProfileId.id) {
    //    return this.deleteEmsProfile(emsProfileOrEmsProfileId.id);
    //  }
    //}
  }

 
}
