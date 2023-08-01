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
import { Reservation } from '../models/reservation.model';
import { ConfigurationService } from './configuration.service';

export type FacilitiesChangedOperation = "add" | "delete" | "modify";
export type FacilitiesChangedEventArg = { facilities: Facility[] | string[], operation: FacilitiesChangedOperation };

export type FacilityTypesChangedOperation = "add" | "delete" | "modify";
export type FacilityTypesChangedEventArg = { facilityTypes: FacilityType[] | string[], operation: FacilityTypesChangedOperation };

@Injectable()
export class FacilityService {

  private readonly _calendarUrl: string = "/api/calendar";
  get calendarUrl() { return this.configurations.baseUrl + this._calendarUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private accountEndpoint: AccountEndpoint, private commonEndpoint: CommonEndpoint, protected configurations: ConfigurationService) {

  }

  getCalendarEvents(page?: number, pageSize?: number) {

    return forkJoin(
      this.commonEndpoint.getPagedList<Reservation[]>(this.calendarUrl, page, pageSize));
  }
}
