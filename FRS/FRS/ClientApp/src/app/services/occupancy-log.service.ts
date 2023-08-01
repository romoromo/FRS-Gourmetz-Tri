import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';
import { CommonEndpoint } from './common-endpoint.service';
import { AuthService } from './auth.service';
import { OccupancyLog } from '../models/occupancy-log.model';
import { ConfigurationService } from './configuration.service';

export type OccupancyLogsChangedOperation = "add" | "delete" | "modify";
export type OccupancyLogsChangedEventArg = { occupancyLogs: OccupancyLog[] | string[], operation: OccupancyLogsChangedOperation };

@Injectable()
export class OccupancyLogService {

  public static readonly occupancyLogAddedOperation: OccupancyLogsChangedOperation = "add";
  public static readonly occupancyLogDeletedOperation: OccupancyLogsChangedOperation = "delete";
  public static readonly occupancyLogModifiedOperation: OccupancyLogsChangedOperation = "modify";

  private _occupancyLogsChanged = new Subject<OccupancyLogsChangedEventArg>();

  private readonly _occupancyLogUrl: string = "/api/occupancylog";
  get occupancyLogUrl() { return this.configurations.baseUrl + this._occupancyLogUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private commonEndpoint: CommonEndpoint, private configurations: ConfigurationService) {

  }


  private onOccupancyLogsChanged(occupancyLogs: OccupancyLog[] | string[], op: OccupancyLogsChangedOperation) {
    this._occupancyLogsChanged.next({ occupancyLogs: occupancyLogs, operation: op });
  }


  onOccupancyLogsCountChanged(occupancyLogs: OccupancyLog[] | string[]) {
    return this.onOccupancyLogsChanged(occupancyLogs, OccupancyLogService.occupancyLogModifiedOperation);
  }


  getOccupancyLogsChangedEvent(): Observable<OccupancyLogsChangedEventArg> {
    return this._occupancyLogsChanged.asObservable();
  }

  getOccupancyLogs(page?: number, pageSize?: number) {

    return forkJoin(
      this.commonEndpoint.getPagedList<OccupancyLog[]>(this.occupancyLogUrl + '/list', page, pageSize));
  }

  getOccupancyLogByInstitutionId(institutionId: string) {

    return forkJoin(
      this.commonEndpoint.getByInstitutionId<OccupancyLog[]>(this.occupancyLogUrl + '/list', institutionId));
  }

  getDateStr(m: Date) {
    if (!m) return '';
    return m.getFullYear() + "-" +
      ("0" + (m.getMonth() + 1)).slice(-2) + "-" +
      ("0" + m.getDate()).slice(-2) + " " +
      ("0" + m.getHours()).slice(-2) + ":" +
      ("0" + m.getMinutes()).slice(-2) + ":" +
      ("0" + m.getSeconds()).slice(-2);
  }

  getOccupancyLogFilter(deviceId: string, sensorId: string, status: string, startTime: string, endTime: string) {

    return forkJoin(this.commonEndpoint.get<OccupancyLog[]>(this.occupancyLogUrl + '/listfilter?StartTime=' + (startTime || '') + '&EndTime=' + (endTime || '') + '&DeviceId=' + (deviceId || '') + '&SensorId=' + (sensorId || '') + '&Status=' + (status || '')));
  }

  getOccupancyLogByKey(institutionId: string, key: string) {

    return this.commonEndpoint.get<OccupancyLog>(this.occupancyLogUrl + '/get?institutionId=' + institutionId + '&key=' + key);
  }

  updateOccupancyLog(occupancyLog: OccupancyLog) {
    return this.commonEndpoint.getUpdateEndpoint(this.occupancyLogUrl, occupancyLog, occupancyLog.id).pipe(
      tap(data => this.onOccupancyLogsChanged([occupancyLog], OccupancyLogService.occupancyLogModifiedOperation)));
  }

  newOccupancyLog(occupancyLog: OccupancyLog) {
    return this.commonEndpoint.getNewEndpoint<OccupancyLog>(this.occupancyLogUrl, occupancyLog).pipe<OccupancyLog>(
      tap(data => this.onOccupancyLogsChanged([occupancyLog], OccupancyLogService.occupancyLogAddedOperation)));
  }


  deleteOccupancyLog(id): Observable<OccupancyLog> {
    return this.commonEndpoint.getDeleteEndpoint<OccupancyLog>(this.occupancyLogUrl, <string>id).pipe<OccupancyLog>(
      tap(data => this.onOccupancyLogsChanged([data], OccupancyLogService.occupancyLogDeletedOperation)));
  }
}
