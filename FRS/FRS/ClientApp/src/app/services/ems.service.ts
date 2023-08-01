import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';




import { CommonEndpoint } from './common-endpoint.service';
import { AuthService } from './auth.service';
import { Ems } from '../models/ems.model';

export type EmsesChangedOperation = "add" | "delete" | "modify";
export type EmsesChangedEventArg = { emses: Ems[] | string[], operation: EmsesChangedOperation };

@Injectable()
export class EmsService {

  public static readonly emsAddedOperation: EmsesChangedOperation = "add";
  public static readonly emsDeletedOperation: EmsesChangedOperation = "delete";
  public static readonly emsModifiedOperation: EmsesChangedOperation = "modify";

  private _emsesChanged = new Subject<EmsesChangedEventArg>();

  private readonly _emsUrl: string = "/api/ems";
  get emsUrl() { return this._emsUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private commonEndpoint: CommonEndpoint) {

  }


  private onEmsesChanged(emses: Ems[] | string[], op: EmsesChangedOperation) {
    this._emsesChanged.next({ emses: emses, operation: op });
  }


  onEmsesCountChanged(emses: Ems[] | string[]) {
    return this.onEmsesChanged(emses, EmsService.emsModifiedOperation);
  }


  getEmsesChangedEvent(): Observable<EmsesChangedEventArg> {
    return this._emsesChanged.asObservable();
  }

  getEmsById(emsId: string) {

    return this.commonEndpoint.getById<any>(this.emsUrl + '/get', emsId);
  }

  getEmses(page?: number, pageSize?: number, institutionId?: string) {
    return this.commonEndpoint.get<Ems[]>(this.emsUrl + '/emses/list?institutionId=' + institutionId);
  }

  updateEms(ems: Ems) {
    if (ems.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.emsUrl, ems, ems.id).pipe(
        tap(data => this.onEmsesChanged([ems], EmsService.emsModifiedOperation)));
    }
  }


  newEms(ems: Ems) {
    return this.commonEndpoint.getNewEndpoint<Ems>(this.emsUrl, ems).pipe<Ems>(
      tap(data => this.onEmsesChanged([ems], EmsService.emsAddedOperation)));
  }


  deleteEms(ems: string | Ems): Observable<Ems> {

    if (typeof ems === 'number' || ems instanceof Number ||
      typeof ems === 'string' || ems instanceof String) {
      return this.commonEndpoint.getDeleteEndpoint<Ems>(this.emsUrl, <string>ems).pipe<Ems>(
        tap(data => this.onEmsesChanged([data], EmsService.emsDeletedOperation)));
    }
    else {

      if (ems.id) {
        return this.deleteEms(ems.id);
      }
    }
  }
}
