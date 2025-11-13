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
import { Menu } from 'src/app/models/meal-order/menu.model';
import { BlockUnblockDateModel, BlockUnblockOutletDateModel, MenuCycle, MenuCycleBlockedDate, MenuCycleSchedulePeriod } from 'src/app/models/meal-order/menu-cycle.model';
import { MenuCycleCalendar } from 'src/app/models/meal-order/menu-cycle-calendar.model';
import { TokenOrder, TokenLabel, TokenDishLabel, MealAllocation, PackingAllocation } from 'src/app/models/meal-order/token-order.model';
import { OutletClassRoster, OutletClassRosterSchedule, OutletClassRosterScheduleByDay } from 'src/app/models/meal-order/outlet-class-roster.model';
import { MealSessionDetail } from 'src/app/models/meal-order/meal-session.model';
import { MenuGroup } from 'src/app/models/meal-order/menu-group.model';

@Injectable()
export class MenuService {

  private readonly _menuBaseUrl: string = "/api/menu";
  get menuBaseUrl() { return this.configurations.baseUrl + this._menuBaseUrl; }

  private readonly _menuUrl: string = "/api/menu/menus";
  get menuUrl() { return this.configurations.baseUrl + this._menuUrl; }

  private readonly _menuCycleUrl: string = "/api/menu/menucycles";
  get menuCycleUrl() { return this.configurations.baseUrl + this._menuCycleUrl; }

  private readonly _outletUrl: string = "/api/menu/outlets";
  get outletUrl() { return this.configurations.baseUrl + this._outletUrl; }

  private readonly _studentUrl: string = "/api/menu/students";
  get studentUrl() { return this.configurations.baseUrl + this._studentUrl; }

  private readonly _studentSessionUrl: string = "/api/menu/students/mealsessions";
  get studentSessionUrl() { return this.configurations.baseUrl + this._studentSessionUrl; }

  private readonly _outletSessionUrl: string = "/api/menu/outlets/mealsessions";
  get outletSessionUrl() { return this.configurations.baseUrl + this._outletSessionUrl; }

  private readonly _mealsessionsByCollectionType: string = "/api/menu/outlets/mealsessionsByCollectionType";
  get mealsessionsByCollectionTypeUrl() { return this.configurations.baseUrl + this._mealsessionsByCollectionType; }

  private readonly _menuCycleCalendarUrl: string = "/api/menu/menucyclecalendars";
  get menuCycleCalendarUrl() { return this.configurations.baseUrl + this._menuCycleCalendarUrl; }

  private readonly _tokenOrderUrl: string = "/api/TokenOrder/tokenorders";
  get tokenOrderUrl() { return this.configurations.baseUrl + this._tokenOrderUrl; }

  private readonly _tokenLabelUrl: string = "/api/TokenOrder/tokenlabels";
  get tokenLabelUrl() { return this.configurations.baseUrl + this._tokenLabelUrl; }

  private readonly _mealAllocationUrl: string = "/api/TokenOrder/mealallocations";
  get mealAllocationUrl() { return this.configurations.baseUrl + this._mealAllocationUrl; }

  private readonly _packingAllocationUrl: string = "/api/TokenOrder/packingallocations";
  get packingAllocationUrl() { return this.configurations.baseUrl + this._packingAllocationUrl; }

  private readonly _tokenOrderReportUrl: string = "/api/TokenOrder";
  get tokenOrderReportUrl() { return this.configurations.baseUrl + this._tokenOrderReportUrl; }

  private readonly _menuGroupuUrl: string = "/api/menu/menugroups";
  get menuGroupUrl() { return this.configurations.baseUrl + this._menuGroupuUrl; }
  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private accountEndpoint: AccountEndpoint, private commonEndpoint: CommonEndpoint, protected configurations: ConfigurationService) {

  }

  //menu
  getMenuById(menuId: string) {

    return this.commonEndpoint.getById<any>(this.menuUrl + '/get', menuId);
  }

  getMenuMealTypesById(menuId: string, page: number, pageSize: number) {

    return this.commonEndpoint.get<any>(`${this.menuBaseUrl}/mealtypedishes/get/${menuId}/${page}/${pageSize}`);
  }

  getMenusByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.menuUrl + '/sieve/list', filter);
  }

  updateMenu(menu: Menu) {
    if (menu.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.menuUrl, menu, menu.id);
    }
  }


  newMenu(menu: Menu) {
    return this.commonEndpoint.getNewEndpoint<Menu>(this.menuUrl, menu);
  }


  deleteMenu(menuOrMenuId: string | Menu): Observable<Menu> {
    return this.commonEndpoint.getDeleteEndpoint<Menu>(this.menuUrl, <string>menuOrMenuId);
  }

  //menuCycle
  getMenuCycleById(menuCycleId: string) {

    return this.commonEndpoint.getById<any>(this.menuCycleUrl + '/get', menuCycleId);
  }

  getMenuCyclesByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.menuCycleUrl + '/sieve/list', filter);
  }

  getOutletMenuCyclesByFilter(outletId, catererId) {
    return this.commonEndpoint.get<MenuCycle[]>(this.outletUrl + '/list?outletId=' + outletId + '&catererId=' + catererId);
  }

  getStudentMenuCyclesByFilter(studentId, outletId, catererId) {
    return this.commonEndpoint.get<OutletClassRoster[]>(`${this.studentUrl}/list?studentId=${studentId}&outletId=${outletId}&catererId=${catererId}`);
  }

  updateMenuCycle(menuCycle: MenuCycle) {
    if (menuCycle.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.menuCycleUrl, menuCycle, menuCycle.id);
    }
  }

  getStudentSessionsByFilter(studentId, orderDate) {
    return this.commonEndpoint.get<MealSessionDetail[]>(`${this.studentSessionUrl}?studentId=${studentId}&orderDate=${orderDate}`);
  }

  getOutletSessionsByFilter(outletId, orderDate, orderDateTo?) {
    return this.commonEndpoint.get<MealSessionDetail[]>(`${this.outletSessionUrl}?outletId=${outletId}&orderDate=${orderDate}&orderDateTo=${orderDateTo}`);
  }

  getMealsessionsByCollectionType(outletId, orderDate, orderDateTo?) {
    return this.commonEndpoint.get<MealSessionDetail[]>(`${this.mealsessionsByCollectionTypeUrl}?outletId=${outletId}&orderDate=${orderDate}&orderDateTo=${orderDateTo}`);
  }

  getMealsessionsFromMealAllocation(outletId, orderDate){
    return this.commonEndpoint.get<MealSessionDetail[]>(`${this.configurations.baseUrl}/api/menu/outlets/mealsessionsFromMealAllocation?outletId=${outletId}&orderDate=${orderDate}`);
  }

  newMenuCycle(menuCycle: MenuCycle) {
    return this.commonEndpoint.getNewEndpoint<MenuCycle>(this.menuCycleUrl, menuCycle);
  }


  deleteMenuCycle(menuCycleOrMenuCycleId: string | MenuCycle): Observable<MenuCycle> {
    return this.commonEndpoint.getDeleteEndpoint<MenuCycle>(this.menuCycleUrl, <string>menuCycleOrMenuCycleId);
  }

  blockunblockdate(model: BlockUnblockDateModel) {
    return this.commonEndpoint.getNewEndpoint<MenuCycleCalendar>(this.menuCycleUrl + '/blockunblockdate', model);
  }

  blockunblockOutletDate(model: BlockUnblockOutletDateModel) {
    return this.commonEndpoint.getNewEndpoint<any>(this.outletUrl + '/blockunblockdate', model);
  }

  createPeriodMenus(model: MenuCycleSchedulePeriod[]) {
    return this.commonEndpoint.getNewEndpoint<any>(this.menuCycleUrl + '/scheduleperiods', model);
  }

  createOutletPeriodMenus(model: MenuCycleSchedulePeriod[]) {
    return this.commonEndpoint.getNewEndpoint<any>(this.outletUrl + '/scheduleperiods', model);
  }

  getOutletMenuCycleSchedulePeriods(outletId: string, catererId: string, menuCycleId: string, day) {
    return this.commonEndpoint.get<any>(this.outletUrl + '/scheduleperiods/list/' + outletId + '/' + catererId + '/' + menuCycleId + '/' + day);
  }

  getMenuCycleSchedulePeriods(menuCycleId: string, day) {
    return this.commonEndpoint.get<any>(this.menuCycleUrl + '/scheduleperiods/list/' + menuCycleId + '/' + day);
  }

  //menuCycleCalendar
  getMenuCycleCalendarById(menuCycleCalendarId: string) {

    return this.commonEndpoint.getById<any>(this.menuCycleCalendarUrl + '/get', menuCycleCalendarId);
  }

  getMenuCycleCalendarsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.menuCycleCalendarUrl + '/sieve/list', filter);
  }

  updateMenuCycleCalendar(menuCycleCalendar: MenuCycleCalendar) {
    if (menuCycleCalendar.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.menuCycleCalendarUrl, menuCycleCalendar, menuCycleCalendar.id);
    }
  }


  newMenuCycleCalendar(menuCycleCalendar: MenuCycleCalendar) {
    return this.commonEndpoint.getNewEndpoint<MenuCycleCalendar>(this.menuCycleCalendarUrl, menuCycleCalendar);
  }


  deleteMenuCycleCalendar(menuCycleCalendarOrMenuCycleCalendarId: string | MenuCycleCalendar): Observable<MenuCycleCalendar> {
    return this.commonEndpoint.getDeleteEndpoint<MenuCycleCalendar>(this.menuCycleCalendarUrl, <string>menuCycleCalendarOrMenuCycleCalendarId);
  }

  //tokenOrder

  getTokenOrderById(id: string) {

    return this.commonEndpoint.getById<any>(this.tokenOrderUrl + '/get', id);
  }

  getTokenOrdersByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.tokenOrderUrl + '/sieve/list', filter);
  }

  updateTokenOrder(tokenOrder: TokenOrder) {
    if (tokenOrder.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.tokenOrderUrl, tokenOrder, tokenOrder.id);
    }
  }


  newTokenOrder(tokenOrder: TokenOrder) {
    return this.commonEndpoint.getNewEndpoint<TokenOrder>(this.tokenOrderUrl, tokenOrder);
  }


  deleteTokenOrder(tokenOrderOrTokenOrderId: string | TokenOrder): Observable<TokenOrder> {
    return this.commonEndpoint.getDeleteEndpoint<TokenOrder>(this.tokenOrderUrl, <string>tokenOrderOrTokenOrderId);
  }

  bulkFasTokenOrder(outletId: string, storeId: string, deliveryDate: string, deliveryDateTo: string, dishTypeId: string, mealSessionId: string, createdBy: string, isClearOrder: boolean) {
    return this.commonEndpoint.get<any>(`${this.tokenOrderUrl}/fas?outletId=${outletId}&storeId=${storeId}&deliveryDate=${deliveryDate}&deliveryDateTo=${deliveryDateTo}&dishTypeId=${dishTypeId}&mealSessionId=${mealSessionId}&createdBy=${createdBy}&clear=${isClearOrder}`);
  }

  getFasTokenOrderSummary(outletId: string, storeId: string, deliveryDate: string, deliveryDateTo: string) {
    return this.commonEndpoint.get<any>(`${this.tokenOrderUrl}/fas/ordersummary?outletId=${outletId}&storeId=${storeId}&deliveryDate=${deliveryDate}&deliveryDateTo=${deliveryDateTo}`);
  }

  updateOrderSession() {
    console.log("updateOrderSession")
    return this.commonEndpoint.get<any>(this.tokenOrderReportUrl + '/updateOrderSession',true);
  }

  //donwload report

  downloadOrderReport(filter: Filter) {
    return this.commonEndpoint.getFile<any>(this.tokenOrderReportUrl + '/orderreport', filter);
  }

  //donwload delivery report
  downloadDeliveryOrderReport(filter: Filter) {
    return this.commonEndpoint.getFile<any>(this.tokenOrderReportUrl + '/deliveryorderreport', filter);
  }

  //download delivery allocation
  downloadDeliveryAllocationReport(filter: Filter) {
    return this.commonEndpoint.getFile<any>(this.tokenOrderReportUrl + '/deliveryallocationreport', filter);
  }

  downloadMealSummaryReport(filter: Filter) {
    return this.commonEndpoint.getFile<any>(this.tokenOrderReportUrl + '/mealsummaryreport', filter);
  }


  downloadOrderLabel(tokenLabel: TokenLabel[]) {
    return this.commonEndpoint.getFile<any>(this.tokenOrderReportUrl + '/orderlabel', tokenLabel);
  }

  //getOrderWithCurrentSession

  getOrderwithCurrentSession(filter: Filter) {
    return this.commonEndpoint.getSieve<any>(this.tokenOrderReportUrl + '/ordersession', filter);
  }

  //tokenLabel

  getTokenLabelById(id: string) {

    return this.commonEndpoint.getById<any>(this.tokenLabelUrl + '/get', id);
  }

  getTokenLabelsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.tokenLabelUrl + '/sieve/list', filter);
  }

  updateTokenLabel(tokenLabel: TokenLabel) {
    if (tokenLabel.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.tokenLabelUrl, tokenLabel, tokenLabel.id);
    }
  }


  newTokenLabel(tokenLabel: TokenLabel) {
    return this.commonEndpoint.getNewEndpoint<TokenLabel>(this.tokenLabelUrl, tokenLabel);
  }


  deleteTokenLabel(tokenLabelOrTokenLabelId: string | TokenLabel): Observable<TokenLabel> {
    return this.commonEndpoint.getDeleteEndpoint<TokenLabel>(this.tokenLabelUrl, <string>tokenLabelOrTokenLabelId);
  }

  //Meal Allocation

  getMealAllocationById(id: string) {

    return this.commonEndpoint.getById<any>(this.mealAllocationUrl + '/get', id);
  }

  getMealAllocationsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.mealAllocationUrl + '/sieve/list', filter);
  }

  updateMealAllocation(allocation: MealAllocation) {
    if (allocation.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.mealAllocationUrl, allocation, allocation.id);
    }
  }


  newMealAllocation(allocation: MealAllocation) {
    return this.commonEndpoint.getNewEndpoint<MealAllocation>(this.mealAllocationUrl, allocation);
  }


  deleteMealAllocation(mealAllocationOrMealAllocationId: string | MealAllocation): Observable<MealAllocation> {
    return this.commonEndpoint.getDeleteEndpoint<MealAllocation>(this.mealAllocationUrl, <string>mealAllocationOrMealAllocationId);
  }


  //Packing Allocation

  getPackingAllocationById(id: string) {

    return this.commonEndpoint.getById<any>(this.packingAllocationUrl + '/get', id);
  }

  getPackingAllocationsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.packingAllocationUrl + '/sieve/list', filter);
  }

  updatePackingAllocation(allocation: PackingAllocation) {
    if (allocation.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.packingAllocationUrl, allocation, allocation.id);
    }
  }


  newPackingAllocation(allocation: PackingAllocation) {
    return this.commonEndpoint.getNewEndpoint<PackingAllocation>(this.packingAllocationUrl, allocation);
  }


  deletePackingAllocation(packingAllocationOrPackingAllocationId: string | PackingAllocation): Observable<PackingAllocation> {
    return this.commonEndpoint.getDeleteEndpoint<PackingAllocation>(this.packingAllocationUrl, <string>packingAllocationOrPackingAllocationId);
  }

  //menu group
  getMenuGroupById(menuGroupId: string) {

    return this.commonEndpoint.getById<any>(this.menuGroupUrl + '/get', menuGroupId);
  }

  getMenuGroupsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.menuGroupUrl + '/sieve/list', filter);
  }

  getMenuGroupsSimpleByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.menuGroupUrl + '/simple/sieve/list', filter);
  }

  updateMenuGroup(menuGroup: MenuGroup) {
    if (menuGroup.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.menuGroupUrl, menuGroup, menuGroup.id);
    }
  }


  newMenuGroup(menuGroup: MenuGroup) {
    return this.commonEndpoint.getNewEndpoint<MenuGroup>(this.menuGroupUrl, menuGroup);
  }


  deleteMenuGroup(menuGroupOrMenuGroupId: string | MenuGroup): Observable<MenuGroup> {
    return this.commonEndpoint.getDeleteEndpoint<MenuGroup>(this.menuGroupUrl, <string>menuGroupOrMenuGroupId);
  }

  getMenuGroupDishCyclesByOutletId(outletId: string) {

    return this.commonEndpoint.get<any>(`${this.menuGroupUrl}/dishcycles/sieve/list?sorts=label&filters=(IsActive)==true,(InOutletId)==${outletId}`);
  }

  getMenuGroupActiveDishCycles(studentId: string) {

    return this.commonEndpoint.get<any>(this.menuGroupUrl + '/activedishcycles?studentId=' + studentId);
  }
  // meal plan
  bulkMealPlanOrder(studentGroupId: string, outletId: string, storeId: string, deliveryDate: string, deliveryDateTo: string, dishTypeId: string, mealSessionId: string, createdBy: string, isClearOrder: boolean) {
    return this.commonEndpoint.get<any>(`${this.tokenOrderUrl}/mealplan?studentGroupId=${studentGroupId}&outletId=${outletId}&storeId=${storeId}&deliveryDate=${deliveryDate}&deliveryDateTo=${deliveryDateTo}&dishTypeId=${dishTypeId}&mealSessionId=${mealSessionId}&createdBy=${createdBy}&clear=${isClearOrder}`);
  }

  getMealPlanSummary(studentGroupId: string, outletId: string, storeId: string, deliveryDate: string, deliveryDateTo: string, mealSessionId: string) {
    return this.commonEndpoint.get<any>(`${this.tokenOrderUrl}/mealplan/ordersummary?studentGroupId=${studentGroupId}&outletId=${outletId}&storeId=${storeId}&deliveryDate=${deliveryDate}&deliveryDateTo=${deliveryDateTo}&mealSessionId=${mealSessionId}`);
  }

  // individual order
  bulkStudentGroupOrder(studentGroupId: string, outletId: string, storeId: string, deliveryDate: string, deliveryDateTo: string, dishTypeId: string, mealSessionId: string, createdBy: string, isClearOrder: boolean, type: string) {
    return this.commonEndpoint.get<any>(`${this.tokenOrderUrl}/studentgroup/order?studentGroupId=${studentGroupId}&outletId=${outletId}&storeId=${storeId}&deliveryDate=${deliveryDate}&deliveryDateTo=${deliveryDateTo}&dishTypeId=${dishTypeId}&mealSessionId=${mealSessionId}&createdBy=${createdBy}&clear=${isClearOrder}&type=${type}`);
  }

  getStudentGroupOrderSummary(studentGroupId: string, outletId: string, storeId: string, deliveryDate: string, deliveryDateTo: string, mealSessionId: string, type: string) {
    return this.commonEndpoint.get<any>(`${this.tokenOrderUrl}/studentgroup/ordersummary?studentGroupId=${studentGroupId}&outletId=${outletId}&storeId=${storeId}&deliveryDate=${deliveryDate}&deliveryDateTo=${deliveryDateTo}&mealSessionId=${mealSessionId}&type=${type}`);
  }
}
