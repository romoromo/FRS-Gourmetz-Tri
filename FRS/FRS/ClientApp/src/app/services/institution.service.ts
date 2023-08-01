import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';




import { CommonEndpoint } from './common-endpoint.service';
import { AuthService } from './auth.service';
import { Institution } from '../models/institution.model';
import { ConfigurationService } from './configuration.service';

export type InstitutionsChangedOperation = "add" | "delete" | "modify";
export type InstitutionsChangedEventArg = { institutions: Institution[] | string[], operation: InstitutionsChangedOperation };

@Injectable()
export class InstitutionService {

  public static readonly institutionAddedOperation: InstitutionsChangedOperation = "add";
  public static readonly institutionDeletedOperation: InstitutionsChangedOperation = "delete";
  public static readonly institutionModifiedOperation: InstitutionsChangedOperation = "modify";

  private _institutionsChanged = new Subject<InstitutionsChangedEventArg>();

  private readonly _institutionUrl: string = "/api/institution";
  get institutionUrl() { return this.configurations.baseUrl + this._institutionUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private commonEndpoint: CommonEndpoint, private configurations: ConfigurationService) {

  }


  private onInstitutionsChanged(institutions: Institution[] | string[], op: InstitutionsChangedOperation) {
    this._institutionsChanged.next({ institutions: institutions, operation: op });
  }


  onInstitutionsCountChanged(institutions: Institution[] | string[]) {
    return this.onInstitutionsChanged(institutions, InstitutionService.institutionModifiedOperation);
  }


  getInstitutionsChangedEvent(): Observable<InstitutionsChangedEventArg> {
    return this._institutionsChanged.asObservable();
  }

  getInstitutionById(institutionId: string) {

    return this.commonEndpoint.getByInstitutionId<any>(this.institutionUrl + '/get', institutionId);
  }

  getInstitutions(page?: number, pageSize?: number) {

    return forkJoin(
      this.commonEndpoint.getPagedList<Institution[]>(this.institutionUrl + '/institutions/list', page, pageSize));
  }

  updateInstitution(institution: Institution) {
    if (institution.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.institutionUrl, institution, institution.id).pipe(
        tap(data => this.onInstitutionsChanged([institution], InstitutionService.institutionModifiedOperation)));
    }
  }


  newInstitution(institution: Institution) {
    return this.commonEndpoint.getNewEndpoint<Institution>(this.institutionUrl, institution).pipe<Institution>(
      tap(data => this.onInstitutionsChanged([institution], InstitutionService.institutionAddedOperation)));
  }


  deleteInstitution(facilityOrFacilityId: string | Institution): Observable<Institution> {

    if (typeof facilityOrFacilityId === 'number' || facilityOrFacilityId instanceof Number ||
      typeof facilityOrFacilityId === 'string' || facilityOrFacilityId instanceof String) {
      return this.commonEndpoint.getDeleteEndpoint<Institution>(this.institutionUrl, <string>facilityOrFacilityId).pipe<Institution>(
        tap(data => this.onInstitutionsChanged([data], InstitutionService.institutionDeletedOperation)));
    }
    else {

      if (facilityOrFacilityId.id) {
        return this.deleteInstitution(facilityOrFacilityId.id);
      }
    }
  }
}
