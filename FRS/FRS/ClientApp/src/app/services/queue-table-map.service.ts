import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';
import { CommonEndpoint } from './common-endpoint.service';
import { AuthService } from './auth.service';
import { QueueTableMap } from '../models/queue-table-map.model';
import { ConfigurationService } from './configuration.service';

export type QueueTableMapsChangedOperation = "add" | "delete" | "modify";
export type QueueTableMapsChangedEventArg = { queueTableMaps: QueueTableMap[] | string[], operation: QueueTableMapsChangedOperation };

@Injectable()
export class QueueTableMapService {

  public static readonly queueTableMapAddedOperation: QueueTableMapsChangedOperation = "add";
  public static readonly queueTableMapDeletedOperation: QueueTableMapsChangedOperation = "delete";
  public static readonly queueTableMapModifiedOperation: QueueTableMapsChangedOperation = "modify";

  private _queueTableMapsChanged = new Subject<QueueTableMapsChangedEventArg>();

  private readonly _queueTableMapUrl: string = "/api/queuetablemap";
  get queueTableMapUrl() { return this.configurations.baseUrl + this._queueTableMapUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private commonEndpoint: CommonEndpoint, private configurations: ConfigurationService) {

  }


  private onQueueTableMapsChanged(queueTableMaps: QueueTableMap[] | string[], op: QueueTableMapsChangedOperation) {
    this._queueTableMapsChanged.next({ queueTableMaps: queueTableMaps, operation: op });
  }


  onQueueTableMapsCountChanged(queueTableMaps: QueueTableMap[] | string[]) {
    return this.onQueueTableMapsChanged(queueTableMaps, QueueTableMapService.queueTableMapModifiedOperation);
  }


  getQueueTableMapsChangedEvent(): Observable<QueueTableMapsChangedEventArg> {
    return this._queueTableMapsChanged.asObservable();
  }

  getQueueTableMaps(page?: number, pageSize?: number) {

    return forkJoin(
      this.commonEndpoint.getPagedList<QueueTableMap[]>(this.queueTableMapUrl + '/list', page, pageSize));
  }

  getQueueTableMapByInstitutionId(institutionId: string) {

    return forkJoin(
      this.commonEndpoint.getByInstitutionId<QueueTableMap[]>(this.queueTableMapUrl + '/list', institutionId));
  }

  getQueueTableMapByKey(institutionId: string, key: string) {

    return this.commonEndpoint.get<QueueTableMap>(this.queueTableMapUrl + '/get?institutionId=' + institutionId + '&key=' + key);
  }

  updateQueueTableMap(queueTableMap: QueueTableMap) {
    return this.commonEndpoint.getUpdateEndpoint(this.queueTableMapUrl, queueTableMap, queueTableMap.id).pipe(
      tap(data => this.onQueueTableMapsChanged([queueTableMap], QueueTableMapService.queueTableMapModifiedOperation)));
  }

  newQueueTableMap(queueTableMap: QueueTableMap) {
    return this.commonEndpoint.getNewEndpoint<QueueTableMap>(this.queueTableMapUrl, queueTableMap).pipe<QueueTableMap>(
      tap(data => this.onQueueTableMapsChanged([queueTableMap], QueueTableMapService.queueTableMapAddedOperation)));
  }


  deleteQueueTableMap(id): Observable<QueueTableMap> {
    return this.commonEndpoint.getDeleteEndpoint<QueueTableMap>(this.queueTableMapUrl, <string>id).pipe<QueueTableMap>(
      tap(data => this.onQueueTableMapsChanged([data], QueueTableMapService.queueTableMapDeletedOperation)));
  }

  sendQueueSimulator(data: any) {
    return this.commonEndpoint.sendQueueSimulator<any>(this.queueTableMapUrl + '/callqueue', data);
  }

  getToken(username, password) {
    return this.commonEndpoint.sendQueueSimulator<any>(this.queueTableMapUrl + '/gettoken', { username: username, password: password});
  }
}
