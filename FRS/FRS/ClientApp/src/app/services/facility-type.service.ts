import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';




import { CommonEndpoint } from './common-endpoint.service';
import { AuthService } from './auth.service';
import { FacilityType } from '../models/facility-type.model';
import { ConfigurationService } from './configuration.service';

export type FacilityTypesChangedOperation = "add" | "delete" | "modify";
export type FacilityTypesChangedEventArg = { facilityTypes: FacilityType[] | string[], operation: FacilityTypesChangedOperation };

@Injectable()
export class FacilityTypeService {

  public static readonly facilityTypeAddedOperation: FacilityTypesChangedOperation = "add";
  public static readonly facilityTypeDeletedOperation: FacilityTypesChangedOperation = "delete";
  public static readonly facilityTypeModifiedOperation: FacilityTypesChangedOperation = "modify";

  private _facilityTypesChanged = new Subject<FacilityTypesChangedEventArg>();

  private readonly _facilityTypeUrl: string = "/api/facilitytype";
  get facilityTypeUrl() { return this.configurations.baseUrl + this._facilityTypeUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private commonEndpoint: CommonEndpoint, private configurations: ConfigurationService) {

  }


  private onFacilityTypesChanged(facilityTypes: FacilityType[] | string[], op: FacilityTypesChangedOperation) {
    this._facilityTypesChanged.next({ facilityTypes: facilityTypes, operation: op });
  }


  onFacilityTypesCountChanged(facilityTypes: FacilityType[] | string[]) {
    return this.onFacilityTypesChanged(facilityTypes, FacilityTypeService.facilityTypeModifiedOperation);
  }


  getFacilityTypesChangedEvent(): Observable<FacilityTypesChangedEventArg> {
    return this._facilityTypesChanged.asObservable();
  }

  getFacilityTypes(page?: number, pageSize?: number) {

    return forkJoin(
      this.commonEndpoint.getPagedList<FacilityType[]>(this.facilityTypeUrl + '/facilitytypes/list', page, pageSize));
  }

  getFacilityTypeByInstitutionId(institutionId: string) {

    return this.commonEndpoint.getByInstitutionId<FacilityType[]>(this.facilityTypeUrl + '/facilitytypes/list', institutionId);
  }

  updateFacilityType(facilityType: FacilityType) {
    if (facilityType.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.facilityTypeUrl, facilityType, facilityType.id).pipe(
        tap(data => this.onFacilityTypesChanged([facilityType], FacilityTypeService.facilityTypeModifiedOperation)));
    }
    else {
      return this.commonEndpoint.getFacilityTypeByFacilityTypeNameEndpoint<FacilityType>(facilityType.name).pipe(
        mergeMap(foundFacility => {
          facilityType.id = foundFacility.id;
          return this.commonEndpoint.getUpdateEndpoint(this.facilityTypeUrl, facilityType, facilityType.id)
        }),
        tap(data => this.onFacilityTypesChanged([facilityType], FacilityTypeService.facilityTypeModifiedOperation)));
    }
  }


  newFacilityType(facilityType: FacilityType) {
    return this.commonEndpoint.getNewEndpoint<FacilityType>(this.facilityTypeUrl, facilityType).pipe<FacilityType>(
      tap(data => this.onFacilityTypesChanged([facilityType], FacilityTypeService.facilityTypeAddedOperation)));
  }


  deleteFacilityType(facilityOrFacilityId: string | FacilityType): Observable<FacilityType> {

    if (typeof facilityOrFacilityId === 'number' || facilityOrFacilityId instanceof Number ||
      typeof facilityOrFacilityId === 'string' || facilityOrFacilityId instanceof String) {
      return this.commonEndpoint.getDeleteEndpoint<FacilityType>(this.facilityTypeUrl, <string>facilityOrFacilityId).pipe<FacilityType>(
        tap(data => this.onFacilityTypesChanged([data], FacilityTypeService.facilityTypeDeletedOperation)));
    }
    else {

      if (facilityOrFacilityId.id) {
        return this.deleteFacilityType(facilityOrFacilityId.id);
      }
      else {
        return this.commonEndpoint.getFacilityTypeByFacilityTypeNameEndpoint<FacilityType>(facilityOrFacilityId.name).pipe<FacilityType>(
          mergeMap(facilityType => this.deleteFacilityType(facilityType.id)));
      }
    }
  }
}
