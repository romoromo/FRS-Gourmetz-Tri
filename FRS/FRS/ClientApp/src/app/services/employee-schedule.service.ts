import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';
import { CommonEndpoint } from './common-endpoint.service';
import { AuthService } from './auth.service';
import { EmployeeSchedule } from '../models/employee-schedule.model';
import { ConfigurationService } from './configuration.service';

export type EmployeeSchedulesChangedOperation = "add" | "delete" | "modify";
export type EmployeeSchedulesChangedEventArg = { employeeSchedules: EmployeeSchedule[] | string[], operation: EmployeeSchedulesChangedOperation };

@Injectable()
export class EmployeeScheduleService {

  public static readonly employeeScheduleAddedOperation: EmployeeSchedulesChangedOperation = "add";
  public static readonly employeeScheduleDeletedOperation: EmployeeSchedulesChangedOperation = "delete";
  public static readonly employeeScheduleModifiedOperation: EmployeeSchedulesChangedOperation = "modify";

  private _employeeSchedulesChanged = new Subject<EmployeeSchedulesChangedEventArg>();

  private readonly _employeeScheduleUrl: string = "/api/employeeschedule";
  get employeeScheduleUrl() { return this.configurations.baseUrl + this._employeeScheduleUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private commonEndpoint: CommonEndpoint, private configurations: ConfigurationService) {

  }


  private onEmployeeSchedulesChanged(employeeSchedules: EmployeeSchedule[] | string[], op: EmployeeSchedulesChangedOperation) {
    this._employeeSchedulesChanged.next({ employeeSchedules: employeeSchedules, operation: op });
  }


  onEmployeeSchedulesCountChanged(employeeSchedules: EmployeeSchedule[] | string[]) {
    return this.onEmployeeSchedulesChanged(employeeSchedules, EmployeeScheduleService.employeeScheduleModifiedOperation);
  }


  getEmployeeSchedulesChangedEvent(): Observable<EmployeeSchedulesChangedEventArg> {
    return this._employeeSchedulesChanged.asObservable();
  }

  getEmployeeSchedules(page?: number, pageSize?: number) {

    return forkJoin(
      this.commonEndpoint.getPagedList<EmployeeSchedule[]>(this.employeeScheduleUrl + '/list', page, pageSize));
  }

  getEmployeeScheduleByInstitutionId(institutionId: string) {

    return forkJoin(
      this.commonEndpoint.getByInstitutionId<EmployeeSchedule[]>(this.employeeScheduleUrl + '/list', institutionId));
  }

  getEmployeeScheduleByKey(institutionId: string, key: string) {

    return this.commonEndpoint.get<EmployeeSchedule>(this.employeeScheduleUrl + '/get?institutionId=' + institutionId + '&key=' + key);
  }

  updateEmployeeSchedule(employeeSchedule: EmployeeSchedule) {
    return this.commonEndpoint.getUpdateEndpoint(this.employeeScheduleUrl, employeeSchedule, employeeSchedule.id).pipe(
      tap(data => this.onEmployeeSchedulesChanged([employeeSchedule], EmployeeScheduleService.employeeScheduleModifiedOperation)));
  }

  newEmployeeSchedule(employeeSchedule: EmployeeSchedule) {
    return this.commonEndpoint.getNewEndpoint<EmployeeSchedule>(this.employeeScheduleUrl, employeeSchedule).pipe<EmployeeSchedule>(
      tap(data => this.onEmployeeSchedulesChanged([employeeSchedule], EmployeeScheduleService.employeeScheduleAddedOperation)));
  }


  deleteEmployeeSchedule(id): Observable<EmployeeSchedule> {
    return this.commonEndpoint.getDeleteEndpoint<EmployeeSchedule>(this.employeeScheduleUrl, <string>id).pipe<EmployeeSchedule>(
      tap(data => this.onEmployeeSchedulesChanged([data], EmployeeScheduleService.employeeScheduleDeletedOperation)));
  }
}
