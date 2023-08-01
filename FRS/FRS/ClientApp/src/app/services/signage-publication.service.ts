import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';




import { CommonEndpoint } from './common-endpoint.service';
import { AuthService } from './auth.service';
import { SignagePublication } from '../models/signage-publication.model';
import { ConfigurationService } from './configuration.service';

export type SignagePublicationsChangedOperation = "add" | "delete" | "modify";
export type SignagePublicationsChangedEventArg = { signagePublications: SignagePublication[] | string[], operation: SignagePublicationsChangedOperation };

@Injectable()
export class SignagePublicationService {

  public static readonly signagePublicationAddedOperation: SignagePublicationsChangedOperation = "add";
  public static readonly signagePublicationDeletedOperation: SignagePublicationsChangedOperation = "delete";
  public static readonly signagePublicationModifiedOperation: SignagePublicationsChangedOperation = "modify";

  private _signagePublicationsChanged = new Subject<SignagePublicationsChangedEventArg>();

  private readonly _signagePublicationUrl: string = "/api/signagepublication";
  get signagePublicationUrl() { return this.configurations.baseUrl + this._signagePublicationUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private commonEndpoint: CommonEndpoint, protected configurations: ConfigurationService) {

  }


  private onSignagePublicationsChanged(signagePublications: SignagePublication[] | string[], op: SignagePublicationsChangedOperation) {
    this._signagePublicationsChanged.next({ signagePublications: signagePublications, operation: op });
  }


  onSignagePublicationsCountChanged(signagePublications: SignagePublication[] | string[]) {
    return this.onSignagePublicationsChanged(signagePublications, SignagePublicationService.signagePublicationModifiedOperation);
  }


  getSignagePublicationsChangedEvent(): Observable<SignagePublicationsChangedEventArg> {
    return this._signagePublicationsChanged.asObservable();
  }

  getSignagePublication(id: string) {

    return this.commonEndpoint.get<SignagePublication>(this.signagePublicationUrl + '/signagepublication/' + id);
  }

  getSignagePublications(page?: number, pageSize?: number) {

    return forkJoin(
      this.commonEndpoint.getPagedList<SignagePublication[]>(this.signagePublicationUrl + '/signagepublications/list', page, pageSize));
  }

  getSignagePublicationByInstitutionId(institutionId: string) {

    return forkJoin(
      this.commonEndpoint.getByInstitutionId<SignagePublication[]>(this.signagePublicationUrl + '/signagepublications/list', institutionId));
  }

  updateSignagePublication(signagePublication: SignagePublication) {
    if (signagePublication.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.signagePublicationUrl, signagePublication, signagePublication.id).pipe(
        tap(data => this.onSignagePublicationsChanged([signagePublication], SignagePublicationService.signagePublicationModifiedOperation)));
    }
  }


  newSignagePublication(signagePublication: SignagePublication) {
    return this.commonEndpoint.getNewEndpoint<SignagePublication>(this.signagePublicationUrl, signagePublication).pipe<SignagePublication>(
      tap(data => this.onSignagePublicationsChanged([signagePublication], SignagePublicationService.signagePublicationAddedOperation)));
  }


  deleteSignagePublication(signagePublicationorId: string | SignagePublication): Observable<SignagePublication> {

    if (typeof signagePublicationorId === 'number' || signagePublicationorId instanceof Number ||
      typeof signagePublicationorId === 'string' || signagePublicationorId instanceof String) {
      return this.commonEndpoint.getDeleteEndpoint<SignagePublication>(this.signagePublicationUrl, <string>signagePublicationorId).pipe<SignagePublication>(
        tap(data => this.onSignagePublicationsChanged([data], SignagePublicationService.signagePublicationDeletedOperation)));
    }
    else {
      if (signagePublicationorId.id) {
        return this.deleteSignagePublication(signagePublicationorId.id);
      }
    }
  }
}
