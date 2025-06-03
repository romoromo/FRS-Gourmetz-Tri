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
import { DishType } from 'src/app/models/meal-order/dish-type.model';
import { Dish } from 'src/app/models/meal-order/dish.model';
import { Cuisine } from 'src/app/models/meal-order/cuisine.model';
import { BlockUnblockOutletDishDateModel, DishBlockUnblockDateModel, DishCycle, DishCyclePeriod, OutletDishCyclePeriodMenu, OutletDishViewMenu } from 'src/app/models/meal-order/dish-cycle';
import { DishCycleCalendar } from 'src/app/models/meal-order/dish-cycle-calendar.model';

@Injectable()
export class DishService {

  private readonly _cuisineUrl: string = "/api/dish/cuisines";
  get cuisineUrl() { return this.configurations.baseUrl + this._cuisineUrl; }

  private readonly _dishTypeUrl: string = "/api/dish/dishtypes";
  get dishTypeUrl() { return this.configurations.baseUrl + this._dishTypeUrl; }

  private readonly _dishUrl: string = "/api/dish/dishes";
  get dishUrl() { return this.configurations.baseUrl + this._dishUrl; }

  private readonly _dishCycleUrl: string = "/api/dish/dishcycles";
  get dishCycleUrl() { return this.configurations.baseUrl + this._dishCycleUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private accountEndpoint: AccountEndpoint, private commonEndpoint: CommonEndpoint, protected configurations: ConfigurationService) {

  }

  //cuisine
  getCuisineById(cuisineId: string) {

    return this.commonEndpoint.getById<any>(this.cuisineUrl + '/get', cuisineId);
  }

  getCuisinesByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.cuisineUrl + '/sieve/list', filter);
  }

  updateCuisine(cuisine: Cuisine) {
    if (cuisine.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.cuisineUrl, cuisine, cuisine.id);
    }
  }


  newCuisine(cuisine: Cuisine) {
    return this.commonEndpoint.getNewEndpoint<Cuisine>(this.cuisineUrl, cuisine);
  }


  deleteCuisine(cuisineOrCuisineId: string | Cuisine): Observable<Cuisine> {
    return this.commonEndpoint.getDeleteEndpoint<Cuisine>(this.cuisineUrl, <string>cuisineOrCuisineId);
  }

  //dish Type
  getDishTypeById(dishTypeId: string) {

    return this.commonEndpoint.getById<any>(this.dishTypeUrl + '/get', dishTypeId);
  }

  getDishTypesByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.dishTypeUrl + '/sieve/list', filter);
  }

  getDishTypesByActiveDishCycles(outletId, orderDate, orderDateTo?) {
    return this.commonEndpoint.get<DishType[]>(`${this.dishTypeUrl}/by-active-dishcycles?outletId=${outletId}&deliveryDate=${orderDate}&deliveryDateTo=${orderDateTo}`);
  }

  updateDishType(dishType: DishType) {
    if (dishType.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.dishTypeUrl, dishType, dishType.id);
    }
  }


  newDishType(dishType: DishType) {
    return this.commonEndpoint.getNewEndpoint<DishType>(this.dishTypeUrl, dishType);
  }


  deleteDishType(dishTypeOrDishTypeId: string | DishType): Observable<DishType> {
    return this.commonEndpoint.getDeleteEndpoint<DishType>(this.dishTypeUrl, <string>dishTypeOrDishTypeId);
  }


  //dish
  getDishById(dishId: string) {

    return this.commonEndpoint.getById<any>(this.dishUrl + '/get', dishId);
  }

  getDishesByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.dishUrl + '/sieve/list', filter);
  }

  updateDish(dish: Dish) {
    if (dish.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.dishUrl, dish, dish.id);
    }
  }


  newDish(dish: Dish) {
    return this.commonEndpoint.getNewEndpoint<Dish>(this.dishUrl, dish);
  }


  deleteDish(dishOrDishId: string | Dish): Observable<Dish> {
    return this.commonEndpoint.getDeleteEndpoint<Dish>(this.dishUrl, <string>dishOrDishId);
  }

  generateDishCode(catererId: string) {
    return this.commonEndpoint.get<any>(this.dishUrl + '/generatecode?catererId=' + catererId);
  }

  dishExport(filter: Filter) {
    return this.commonEndpoint.getFile<any>(this.dishUrl + `/sieve/list/export`, filter);
  }

  //dish cycles
  getDishCycleById(dishCycleId: string) {

    return this.commonEndpoint.getById<any>(this.dishCycleUrl + '/get', dishCycleId);
  }

  getDishCyclesSimpleByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.dishCycleUrl + '/simple/sieve/list', filter);
  }

  getDishCyclesByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.dishCycleUrl + '/sieve/list', filter);
  }

  getDishCycleScheduleSetDetails(id, day, outletId?: string) {
    return this.commonEndpoint.get<any>(`${this.dishCycleUrl}/getScheduleSetDetails/${id}/${day}?outletId=${outletId}`);
  }

  getDishCyclesByStudent(id, orderDate, sessionId) {
    return this.commonEndpoint.get<any>(`${this.dishCycleUrl}/student/${id}?orderDate=${orderDate}&mealSessionId=${sessionId}`);
  }

  createOutletDishCyclePeriodMenus(model: OutletDishViewMenu) {
    return this.commonEndpoint.getNewEndpoint<any>(`${this.dishCycleUrl}/outlets/scheduleperiods`, model);
  }

  updateDishCycle(dishCycle: DishCycle) {
    if (dishCycle.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.dishCycleUrl, dishCycle, dishCycle.id);
    }
  }


  newDishCycle(dishCycle: DishCycle) {
    return this.commonEndpoint.getNewEndpoint<DishCycle>(this.dishCycleUrl, dishCycle);
  }


  deleteDishCycle(dishOrDishCycleId: string | DishCycle): Observable<DishCycle> {
    return this.commonEndpoint.getDeleteEndpoint<DishCycle>(this.dishCycleUrl, <string>dishOrDishCycleId);
  }

  blockunblockdate(model: DishBlockUnblockDateModel) {
    return this.commonEndpoint.getNewEndpoint<DishCycleCalendar>(this.dishCycleUrl + '/blockunblockdate', model);
  }

  getOutletDishCycles(outletId, catererId) {
    return this.commonEndpoint.get<DishCycle[]>(this.dishCycleUrl + '/outlets/list?outletId=' + outletId + '&catererId=' + catererId);
  }

  getOutletDishCyclePeriods(dishCycleId) {
    return this.commonEndpoint.get<DishCyclePeriod[]>(this.dishCycleUrl + '/outlets/periods/list?dishCycleId=' + dishCycleId);
  }

  blockunblockOutletDishDate(model: BlockUnblockOutletDishDateModel) {
    return this.commonEndpoint.getNewEndpoint<any>(this.dishCycleUrl + '/outlets/blockunblockdate', model);
  }

  //combined dish

  getCombinedDish(outletId, catererId, mealTypeId, orderDate, sessionId) {
    return this.commonEndpoint.get<any>(this.dishUrl + '/get/' + outletId + '?catererId=' + catererId + '&mealTypeId=' + mealTypeId + '&orderDate=' + orderDate + '&sessionId=' + sessionId);
  }
}
