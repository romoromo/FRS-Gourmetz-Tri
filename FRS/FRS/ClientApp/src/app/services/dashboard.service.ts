import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap, catchError } from 'rxjs/operators';



import { CommonEndpoint } from './common-endpoint.service';
import { AccountEndpoint } from './account-endpoint.service';
import { AuthService } from './auth.service';
import { Reservation, CalendarFilter } from '../models/reservation.model';
import { LocationService } from './location.service';
import { ConfigurationService } from './configuration.service';
import { CalendarEvent } from 'calendar-utils';
import { DeviceFilter, Device } from '../models/device.model';
import { PIBDevice } from '../models/department.model';
import { SignageDashboard, SignageDashboardFilter } from '../models/dashboard.model';
import { Filter } from '../models/sieve-filter.model';


@Injectable()
export class DashboardService {

  private readonly _dashboardUrl: string = "/api/dashboard";

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private accountEndpoint: AccountEndpoint, private commonEndpoint: CommonEndpoint, private locationService: LocationService,
    protected configurations: ConfigurationService ) {

  }

  get dashboardUrl() { return this.configurations.baseUrl + this._dashboardUrl; }

   getUpcomingEvents(pageSize?: number, filter?: CalendarFilter) {
     return this.commonEndpoint.getPagedList<Reservation[]>(this.dashboardUrl + '/upcomingevents/' + pageSize, null, null, true, filter);
  }

  getInProgressEvents(pageSize?: number, filter?: CalendarFilter) {
    return this.commonEndpoint.getPagedList<Reservation[]>(this.dashboardUrl + '/inprogressevents/' + pageSize, null, null, true, filter);
  }

  getDeviceForApproval(pageNumber?: number,pageSize?: number, filter?: DeviceFilter) {
    return this.commonEndpoint.getPagedList<Device[]>(this.dashboardUrl + '/devices', pageNumber, pageSize, true, filter);
  }

  getPIBDeviceStatusList(pageNumber?: number, pageSize?: number, filter?: DeviceFilter) {
    return this.commonEndpoint.getPagedList<PIBDevice[]>(this.dashboardUrl + '/pibdevices', pageNumber, pageSize, true, filter);
  }

  getSignageDashboard(filter?: SignageDashboardFilter) {
    return this.commonEndpoint.get<any>(this.dashboardUrl + '/signage/dashboard/', true, filter);
  }

  
}
