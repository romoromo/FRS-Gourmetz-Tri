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
import { MealType } from 'src/app/models/meal-order/meal-type.model';
import { MealPeriod } from 'src/app/models/meal-order/meal-period.model';
import { MealSession } from 'src/app/models/meal-order/meal-session.model';

@Injectable()
export class MealService {

  private readonly _mealTypeUrl: string = "/api/meal/mealtypes";
  get mealTypeUrl() { return this.configurations.baseUrl + this._mealTypeUrl; }

  private readonly _mealPeriodUrl: string = "/api/meal/mealperiods";
  get mealPeriodUrl() { return this.configurations.baseUrl + this._mealPeriodUrl; }

  private readonly _mealSessionUrl: string = "/api/meal/mealsessions";
  get mealSessionUrl() { return this.configurations.baseUrl + this._mealSessionUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private accountEndpoint: AccountEndpoint, private commonEndpoint: CommonEndpoint, protected configurations: ConfigurationService) {

  }

  //meal Type
  getMealTypeById(mealTypeId: string) {

    return this.commonEndpoint.getById<any>(this.mealTypeUrl + '/get', mealTypeId);
  }

  getMealTypesByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.mealTypeUrl + '/sieve/list', filter);
  }

  updateMealType(mealType: MealType) {
    if (mealType.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.mealTypeUrl, mealType, mealType.id);
    }
  }


  newMealType(mealType: MealType) {
    return this.commonEndpoint.getNewEndpoint<MealType>(this.mealTypeUrl, mealType);
  }


  deleteMealType(mealTypeOrMealTypeId: string | MealType): Observable<MealType> {
    return this.commonEndpoint.getDeleteEndpoint<MealType>(this.mealTypeUrl, <string>mealTypeOrMealTypeId);
  }


  //meal period
  getMealPeriodById(mealPeriodModelId: string) {

    return this.commonEndpoint.getById<any>(this.mealPeriodUrl + '/get', mealPeriodModelId);
  }

  getMealPeriodsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.mealPeriodUrl + '/sieve/list', filter);
  }

  updateMealPeriod(mealPeriodModel: MealPeriod) {
    if (mealPeriodModel.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.mealPeriodUrl, mealPeriodModel, mealPeriodModel.id);
    }
  }


  newMealPeriod(mealPeriodModel: MealPeriod) {
    return this.commonEndpoint.getNewEndpoint<MealPeriod>(this.mealPeriodUrl, mealPeriodModel);
  }


  deleteMealPeriod(mealPeriodModelOrMealPeriodId: string | MealPeriod): Observable<MealPeriod> {
    return this.commonEndpoint.getDeleteEndpoint<MealPeriod>(this.mealPeriodUrl, <string>mealPeriodModelOrMealPeriodId);
  }

  //meal session
  getMealSessionById(mealSessionModelId: string) {

    return this.commonEndpoint.getById<any>(this.mealSessionUrl + '/get', mealSessionModelId);
  }

  getMealPeriodSessions(outletId: string, catererId: string, outletProfileId: string) {
    return this.commonEndpoint.get<any>(`${this.mealSessionUrl}/mealperiod?outletId=${outletId}&catererId=${catererId}&outletProfileId=${outletProfileId}`);
  }

  getAllMealPeriodSessions(outletId: string, catererId: string, outletProfileId: string) {
    return this.commonEndpoint.get<any>(`${this.mealSessionUrl}/mealperiod/all?outletId=${outletId}&catererId=${catererId}&outletProfileId=${outletProfileId}`);
  }

  getMealSessionsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.mealSessionUrl + '/sieve/list', filter);
  }

  getMealSessionsSimpleByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.mealSessionUrl + '/simple/sieve/list', filter);
  }

  updateMealSession(mealSessionModel: MealSession) {
    if (mealSessionModel.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.mealSessionUrl, mealSessionModel, mealSessionModel.id);
    }
  }


  newMealSession(mealSessionModel: MealSession) {
    return this.commonEndpoint.getNewEndpoint<MealSession>(this.mealSessionUrl, mealSessionModel);
  }


  deleteMealSession(mealSessionModelOrMealSessionId: string | MealSession): Observable<MealSession> {
    return this.commonEndpoint.getDeleteEndpoint<MealSession>(this.mealSessionUrl, <string>mealSessionModelOrMealSessionId);
  }

  bulkMealSession(mealSessions: MealSession[]) {
    return this.commonEndpoint.getNewEndpoint<any>(this.mealSessionUrl + '/bulkcreate', mealSessions);
  }

  getFasMealOrderSummary(outletId: string, storeId: string, mealSessionId: string, deliveryDate: Date) {
    return this.commonEndpoint.get<any>(`${this.mealSessionUrl}/fas/ordersummary?outletId=${outletId}&storeId=${storeId}&mealSessionId=${mealSessionId}&deliveryDate=${deliveryDate}`);
  }

  getMealSessionLite(outletId: string) {
    return this.commonEndpoint.get<any>(`${this.mealSessionUrl}/outlet-lite?outletId=${outletId}`);
  }
}
