import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';




import { CommonEndpoint } from './common-endpoint.service';
import { AuthService } from './auth.service';
import { SignageCompilation } from '../models/signage-compilation.model';
import { ConfigurationService } from './configuration.service';

export type SignageCompilationsChangedOperation = "add" | "delete" | "modify";
export type SignageCompilationsChangedEventArg = { signageCompilations: SignageCompilation[] | string[], operation: SignageCompilationsChangedOperation };

@Injectable()
export class SignageCompilationService {

  public static readonly signageCompilationAddedOperation: SignageCompilationsChangedOperation = "add";
  public static readonly signageCompilationDeletedOperation: SignageCompilationsChangedOperation = "delete";
  public static readonly signageCompilationModifiedOperation: SignageCompilationsChangedOperation = "modify";

  private _signageCompilationsChanged = new Subject<SignageCompilationsChangedEventArg>();

  private readonly _signageCompilationUrl: string = "/api/signagecompilation";
  get signageCompilationUrl() { return this.configurations.baseUrl + this._signageCompilationUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private commonEndpoint: CommonEndpoint, protected configurations: ConfigurationService) {

  }


  private onSignageCompilationsChanged(signageCompilations: SignageCompilation[] | string[], op: SignageCompilationsChangedOperation) {
    this._signageCompilationsChanged.next({ signageCompilations: signageCompilations, operation: op });
  }


  onSignageCompilationsCountChanged(signageCompilations: SignageCompilation[] | string[]) {
    return this.onSignageCompilationsChanged(signageCompilations, SignageCompilationService.signageCompilationModifiedOperation);
  }


  getSignageCompilationsChangedEvent(): Observable<SignageCompilationsChangedEventArg> {
    return this._signageCompilationsChanged.asObservable();
  }

  getSignageCompilations(page?: number, pageSize?: number) {

    return forkJoin(
      this.commonEndpoint.getPagedList<SignageCompilation[]>(this.signageCompilationUrl + '/signagecompilations/list', page, pageSize));
  }

  export(ids: string) {

    return forkJoin(
      this.commonEndpoint.get<SignageCompilation[]>(this.signageCompilationUrl + '/signagecompilations/export?ids=' + ids));
  }

  import(filename: string) {

    return this.commonEndpoint.get(this.signageCompilationUrl + '/signagecompilations/import?filename=' + filename);
  }

  getSignageCompilationByInstitutionId(institutionId: string) {

    return forkJoin(
      this.commonEndpoint.getByInstitutionId<SignageCompilation[]>(this.signageCompilationUrl + '/signagecompilations/list', institutionId));
  }

  updateSignageCompilation(signageCompilation: SignageCompilation) {
    if (signageCompilation.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.signageCompilationUrl, signageCompilation, signageCompilation.id).pipe(
        tap(data => this.onSignageCompilationsChanged([signageCompilation], SignageCompilationService.signageCompilationModifiedOperation)));
    }
  }


  newSignageCompilation(signageCompilation: SignageCompilation) {
    return this.commonEndpoint.getNewEndpoint<SignageCompilation>(this.signageCompilationUrl, signageCompilation).pipe<SignageCompilation>(
      tap(data => this.onSignageCompilationsChanged([signageCompilation], SignageCompilationService.signageCompilationAddedOperation)));
  }


  deleteSignageCompilation(signageCompilationorId: string | SignageCompilation): Observable<SignageCompilation> {

    if (typeof signageCompilationorId === 'number' || signageCompilationorId instanceof Number ||
      typeof signageCompilationorId === 'string' || signageCompilationorId instanceof String) {
      return this.commonEndpoint.getDeleteEndpoint<SignageCompilation>(this.signageCompilationUrl, <string>signageCompilationorId).pipe<SignageCompilation>(
        tap(data => this.onSignageCompilationsChanged([data], SignageCompilationService.signageCompilationDeletedOperation)));
    }
    else {
      if (signageCompilationorId.id) {
        return this.deleteSignageCompilation(signageCompilationorId.id);
      }
    }
  }
}
