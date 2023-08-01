import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';




import { CommonEndpoint } from './common-endpoint.service';
import { AuthService } from './auth.service';
import { SignageComponent } from '../models/signage-component.model';
import { ConfigurationService } from './configuration.service';

export type SignageComponentsChangedOperation = "add" | "delete" | "modify";
export type SignageComponentsChangedEventArg = { signageComponents: SignageComponent[] | string[], operation: SignageComponentsChangedOperation };

@Injectable()
export class SignageComponentService {

  public static readonly signageComponentAddedOperation: SignageComponentsChangedOperation = "add";
  public static readonly signageComponentDeletedOperation: SignageComponentsChangedOperation = "delete";
  public static readonly signageComponentModifiedOperation: SignageComponentsChangedOperation = "modify";

  private _signageComponentsChanged = new Subject<SignageComponentsChangedEventArg>();

  private readonly _signageComponentUrl: string = "/api/signagecomponent";
  get signageComponentUrl() { return this.configurations.baseUrl + this._signageComponentUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private commonEndpoint: CommonEndpoint, protected configurations: ConfigurationService) {

  }


  private onSignageComponentsChanged(signageComponents: SignageComponent[] | string[], op: SignageComponentsChangedOperation) {
    this._signageComponentsChanged.next({ signageComponents: signageComponents, operation: op });
  }


  onSignageComponentsCountChanged(signageComponents: SignageComponent[] | string[]) {
    return this.onSignageComponentsChanged(signageComponents, SignageComponentService.signageComponentModifiedOperation);
  }


  getSignageComponentsChangedEvent(): Observable<SignageComponentsChangedEventArg> {
    return this._signageComponentsChanged.asObservable();
  }

  getSignageComponents(page?: number, pageSize?: number) {

    return forkJoin(
      this.commonEndpoint.getPagedList<SignageComponent[]>(this.signageComponentUrl + '/signagecomponents/list', page, pageSize));
  }

  exportSignageComponents(ids : string) {

    return forkJoin(
      this.commonEndpoint.get<SignageComponent[]>(this.signageComponentUrl + '/signagecomponents/export?ids=' + ids));
  }

  importSignageComponents(filename: string) {

    return this.commonEndpoint.get(this.signageComponentUrl + '/signagecomponents/import?filename=' + filename);
  }

  getSignageComponentByInstitutionId(institutionId: string) {

    return forkJoin(
      this.commonEndpoint.getByInstitutionId<SignageComponent[]>(this.signageComponentUrl + '/signagecomponents/list', institutionId));
  }

  updateSignageComponent(signageComponent: SignageComponent) {
    if (signageComponent.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.signageComponentUrl, signageComponent, signageComponent.id).pipe(
        tap(data => this.onSignageComponentsChanged([signageComponent], SignageComponentService.signageComponentModifiedOperation)));
    }
  }


  newSignageComponent(signageComponent: SignageComponent) {
    return this.commonEndpoint.getNewEndpoint<SignageComponent>(this.signageComponentUrl, signageComponent).pipe<SignageComponent>(
      tap(data => this.onSignageComponentsChanged([signageComponent], SignageComponentService.signageComponentAddedOperation)));
  }


  deleteSignageComponent(signageComponentorId: string | SignageComponent): Observable<SignageComponent> {

    if (typeof signageComponentorId === 'number' || signageComponentorId instanceof Number ||
      typeof signageComponentorId === 'string' || signageComponentorId instanceof String) {
      return this.commonEndpoint.getDeleteEndpoint<SignageComponent>(this.signageComponentUrl, <string>signageComponentorId).pipe<SignageComponent>(
        tap(data => this.onSignageComponentsChanged([data], SignageComponentService.signageComponentDeletedOperation)));
    }
    else {
      if (signageComponentorId.id) {
        return this.deleteSignageComponent(signageComponentorId.id);
      }
    }
  }
}
