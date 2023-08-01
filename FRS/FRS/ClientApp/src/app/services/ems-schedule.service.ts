import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap, catchError } from 'rxjs/operators';



import { CommonEndpoint } from './common-endpoint.service';
import { AccountEndpoint } from './account-endpoint.service';
import { AuthService } from './auth.service';
import { EmsSchedule } from '../models/ems-schedule.model';
import { ConfigurationService } from './configuration.service';

export type EmsSchedulesChangedOperation = "add" | "delete" | "modify";
export type EmsSchedulesChangedEventArg = { emsSchedules: EmsSchedule[] | string[], operation: EmsSchedulesChangedOperation };


@Injectable()
export class EmsScheduleService {

  public static readonly emsScheduleAddedOperation: EmsSchedulesChangedOperation = "add";
  public static readonly emsScheduleDeletedOperation: EmsSchedulesChangedOperation = "delete";
  public static readonly emsScheduleModifiedOperation: EmsSchedulesChangedOperation = "modify";
  private readonly _emsScheduleUrl: string = "/api/emsschedule";

  private _emsSchedulesChanged = new Subject<EmsSchedulesChangedEventArg>();

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private accountEndpoint: AccountEndpoint, private commonEndpoint: CommonEndpoint,
    protected configurations: ConfigurationService ) {

  }

  get emsScheduleUrl() { return this._emsScheduleUrl; }

  private onEmsSchedulesChanged(emsSchedules: EmsSchedule[] | string[], op: EmsSchedulesChangedOperation) {
    this._emsSchedulesChanged.next({ emsSchedules: emsSchedules, operation: op });
  }


  onEmsSchedulesUserCountChanged(emsSchedules: EmsSchedule[] | string[]) {
    return this.onEmsSchedulesChanged(emsSchedules, EmsScheduleService.emsScheduleModifiedOperation);
  }


  getEmsSchedulesChangedEvent(): Observable<EmsSchedulesChangedEventArg> {
    return this._emsSchedulesChanged.asObservable();
  }



  getEmsSchedules(page?: number, pageSize?: number, institutionId?: string) {

    return forkJoin(
      this.commonEndpoint.getPagedList<EmsSchedule[]>(this.emsScheduleUrl + '/GetAllEmsSchedules', page, pageSize, true, { institutionId: institutionId }));
  }


  getAllEmsSchedules(page?: number, pageSize?: number, institutionId?: string) {

    return this.commonEndpoint.getPagedList<EmsSchedule[]>(this.emsScheduleUrl + '/get/emsschedules', page, pageSize, false, { institutionId: institutionId });
  }
  getEmsSchedule(id?: string) {
    return this.commonEndpoint.get<any>(this.emsScheduleUrl + '/get' + "?id=" + id);
  }

  //getEmsSchedules(page?: number, pageSize?: number) {
  //  return this.commonEndpoint.getPagedList<EmsSchedule[]>(this.emsScheduleUrl + '/GetAllEmsSchedules', page, pageSize, true);
  //}


  updateEmsSchedule(emsSchedule: EmsSchedule) {
    if (emsSchedule.id) {
      return this.commonEndpoint.getUpdateEndpoint<any>(this.emsScheduleUrl, emsSchedule, emsSchedule.id).pipe(
        tap(data => this.onEmsSchedulesChanged([emsSchedule], EmsScheduleService.emsScheduleModifiedOperation)));
    }
  }


  newEmsSchedule(emsSchedule: EmsSchedule) {
    return this.commonEndpoint.getNewEndpoint<EmsSchedule>(this.emsScheduleUrl, emsSchedule).pipe<EmsSchedule>(
      tap(data => this.onEmsSchedulesChanged([emsSchedule], EmsScheduleService.emsScheduleAddedOperation)));
  }


  deleteEmsSchedule(emsScheduleOrEmsScheduleId: string | EmsSchedule): Observable<EmsSchedule> {

    if (typeof emsScheduleOrEmsScheduleId === 'number' || emsScheduleOrEmsScheduleId instanceof Number ||
      typeof emsScheduleOrEmsScheduleId === 'string' || emsScheduleOrEmsScheduleId instanceof String
      || typeof emsScheduleOrEmsScheduleId == 'object') {
      var queryStrings = "";
      var id = "";

      if (typeof emsScheduleOrEmsScheduleId == 'object') {
        id = emsScheduleOrEmsScheduleId.id;
      } else {
        id = emsScheduleOrEmsScheduleId;
      }

      return this.commonEndpoint.getDeleteEndpoint<EmsSchedule>(this.emsScheduleUrl, id, queryStrings).pipe<EmsSchedule>(
        tap(data => this.onEmsSchedulesChanged([data], EmsScheduleService.emsScheduleDeletedOperation)));
    }
    //else {

    //  if (emsScheduleOrEmsScheduleId.id) {
    //    return this.deleteEmsSchedule(emsScheduleOrEmsScheduleId.id);
    //  }
    //}
  }

 
}
