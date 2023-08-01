import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';



import { CommonEndpoint } from './common-endpoint.service';
import { AccountEndpoint } from './account-endpoint.service';
import { AuthService } from './auth.service';
import { Facility } from '../models/facility.model';
import { FacilityType } from '../models/facility-type.model';
import { Institution } from '../models/institution.model';
import { ConfigurationService } from './configuration.service';

export type FacilitiesChangedOperation = "add" | "delete" | "modify";
export type FacilitiesChangedEventArg = { facilities: Facility[] | string[], operation: FacilitiesChangedOperation };

export type FacilityTypesChangedOperation = "add" | "delete" | "modify";
export type FacilityTypesChangedEventArg = { facilityTypes: FacilityType[] | string[], operation: FacilityTypesChangedOperation };

@Injectable()
export class FacilityService {

  public static readonly facilityAddedOperation: FacilitiesChangedOperation = "add";
  public static readonly facilityDeletedOperation: FacilitiesChangedOperation = "delete";
  public static readonly facilityModifiedOperation: FacilitiesChangedOperation = "modify";

  private _facilitiesChanged = new Subject<FacilitiesChangedEventArg>();


  private readonly _facilityUrl: string = "/api/facility";
  get facilityUrl() { return this.configurations.baseUrl + this._facilityUrl; }

  private readonly _institutionUrl: string = "/api/institution";
  get institutionUrl() { return this.configurations.baseUrl + this._institutionUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private accountEndpoint: AccountEndpoint, private commonEndpoint: CommonEndpoint, private configurations: ConfigurationService) {

  }


  private onFacilitiesChanged(facilities: Facility[] | string[], op: FacilitiesChangedOperation) {
    this._facilitiesChanged.next({ facilities: facilities, operation: op });
  }


  onFacilitiesUserCountChanged(facilities: Facility[] | string[]) {
    return this.onFacilitiesChanged(facilities, FacilityService.facilityModifiedOperation);
  }


  getFacilitiesChangedEvent(): Observable<FacilitiesChangedEventArg> {
    return this._facilitiesChanged.asObservable();
  }

  getFacilitiesAndFacilityTypes(page?: number, pageSize?: number, institutionId?: string) {

    return forkJoin(
      this.commonEndpoint.getFacilitiesEndpoint<Facility[]>(page, pageSize, institutionId),
      //this.commonEndpoint.getFacilityTypesEndpoint<FacilityType[]>(null, null, institutionId),
      this.commonEndpoint.getPagedList<Institution[]>(this.institutionUrl + '/institutions/list'));
  }

  getFacilitiesByInstitutionId(institutionId: string, page?: number, pageSize?: number) {

    return forkJoin(
      this.commonEndpoint.getFacilitiesEndpoint<Facility[]>(null, null, institutionId));
  }

  updateFacility(facility: Facility) {
    if (facility.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.facilityUrl, facility, facility.id).pipe(
        tap(data => this.onFacilitiesChanged([facility], FacilityService.facilityModifiedOperation)));
    }
    else {
      return this.commonEndpoint.getFacilityByFacilityNameEndpoint<Facility>(facility.name).pipe(
        mergeMap(foundFacility => {
          facility.id = foundFacility.id;
          return this.commonEndpoint.getUpdateEndpoint(this.facilityUrl, facility, facility.id)
        }),
        tap(data => this.onFacilitiesChanged([facility], FacilityService.facilityModifiedOperation)));
    }
  }


  newFacility(facility: Facility) {
    return this.commonEndpoint.getNewEndpoint<Facility>(this.facilityUrl, facility).pipe<Facility>(
      tap(data => this.onFacilitiesChanged([facility], FacilityService.facilityAddedOperation)));
  }


  deleteFacility(facilityOrFacilityId: string | Facility): Observable<Facility> {

    if (typeof facilityOrFacilityId === 'number' || facilityOrFacilityId instanceof Number ||
      typeof facilityOrFacilityId === 'string' || facilityOrFacilityId instanceof String) {
      return this.commonEndpoint.getDeleteEndpoint<Facility>(this.facilityUrl, <string>facilityOrFacilityId).pipe<Facility>(
        tap(data => this.onFacilitiesChanged([data], FacilityService.facilityDeletedOperation)));
    }
    else {

      if (facilityOrFacilityId.id) {
        return this.deleteFacility(facilityOrFacilityId.id);
      }
      else {
        return this.commonEndpoint.getFacilityByFacilityNameEndpoint<Facility>(facilityOrFacilityId.name).pipe<Facility>(
          mergeMap(facility => this.deleteFacility(facility.id)));
      }
    }
  }
}
