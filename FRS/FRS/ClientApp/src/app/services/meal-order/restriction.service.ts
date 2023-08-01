import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';
import { AccountEndpoint } from '../account-endpoint.service';
import { AuthService } from '../auth.service';
import { CommonEndpoint } from '../common-endpoint.service';
import { ConfigurationService } from '../configuration.service';
import { Filter, PagedResult } from 'src/app/models/sieve-filter.model';
import { RestrictionType } from 'src/app/models/meal-order/restriction-type.model';
import { Restriction } from 'src/app/models/meal-order/restriction.model';

@Injectable()
export class RestrictionService {

  private readonly _restrictionUrl: string = "/api/restriction/restrictions";
  get restrictionUrl() { return this.configurations.baseUrl + this._restrictionUrl; }

  private readonly _restrictionTypeUrl: string = "/api/restriction/restrictiontypes";
  get restrictionTypeUrl() { return this.configurations.baseUrl + this._restrictionTypeUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private accountEndpoint: AccountEndpoint, private commonEndpoint: CommonEndpoint, protected configurations: ConfigurationService) {

  }

  //restriction Type
  getRestrictionTypeById(restrictionTypeId: string) {

    return this.commonEndpoint.getById<any>(this.restrictionTypeUrl + '/get', restrictionTypeId);
  }

  getRestrictionTypesByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.restrictionTypeUrl + '/sieve/list', filter);
  }

  updateRestrictionType(restrictionType: RestrictionType) {
    if (restrictionType.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.restrictionTypeUrl, restrictionType, restrictionType.id);
    }
  }


  newRestrictionType(restrictionType: RestrictionType) {
    return this.commonEndpoint.getNewEndpoint<RestrictionType>(this.restrictionTypeUrl, restrictionType);
  }


  deleteRestrictionType(restrictionTypeOrRestrictionTypeId: string | RestrictionType): Observable<RestrictionType> {
    return this.commonEndpoint.getDeleteEndpoint<RestrictionType>(this.restrictionTypeUrl, <string>restrictionTypeOrRestrictionTypeId);
  }


  //restriction
  getRestrictionById(restrictionModelId: string) {

    return this.commonEndpoint.getById<any>(this.restrictionUrl + '/get', restrictionModelId);
  }

  getRestrictionsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.restrictionUrl + '/sieve/list', filter);
  }

  updateRestriction(restrictionModel: Restriction) {
    if (restrictionModel.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.restrictionUrl, restrictionModel, restrictionModel.id);
    }
  }


  newRestriction(restrictionModel: Restriction) {
    return this.commonEndpoint.getNewEndpoint<Restriction>(this.restrictionUrl, restrictionModel);
  }


  deleteRestriction(restrictionModelOrRestrictionId: string | Restriction): Observable<Restriction> {
    return this.commonEndpoint.getDeleteEndpoint<Restriction>(this.restrictionUrl, <string>restrictionModelOrRestrictionId);
  }
}
