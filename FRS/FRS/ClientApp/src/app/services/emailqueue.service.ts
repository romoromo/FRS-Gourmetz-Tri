import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';
import { CommonEndpoint } from './common-endpoint.service';
import { AuthService } from './auth.service';
import { Filter, PagedResult } from '../models/sieve-filter.model';
import { ConfigurationService } from './configuration.service';
import { EmailQueue } from '../models/email-queue.model';

export type EmailQueuesChangedOperation = "add" | "delete" | "modify";
export type EmailQueuesChangedEventArg = { emailQueues: EmailQueue[] | string[], operation: EmailQueuesChangedOperation };

@Injectable()
export class EmailQueueService {

  public static readonly emailQueueAddedOperation: EmailQueuesChangedOperation = "add";
  public static readonly emailQueueDeletedOperation: EmailQueuesChangedOperation = "delete";
  public static readonly emailQueueModifiedOperation: EmailQueuesChangedOperation = "modify";

  private _emailQueuesChanged = new Subject<EmailQueuesChangedEventArg>();

  private readonly _emailQueueUrl: string = "/api/emailqueue/emailqueue";
  get emailQueueUrl() { return this.configurations.baseUrl + this._emailQueueUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private commonEndpoint: CommonEndpoint, protected configurations: ConfigurationService) {

  }
  
  getEmailQueueById(emailQueueId: string) {

    return this.commonEndpoint.getById<any>(this.emailQueueUrl + '/get', emailQueueId);
  }

  getEmailQueuesByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.emailQueueUrl + '/sieve/list', filter);
  }

  updateEmailQueue(emailQueue: EmailQueue) {
    if (emailQueue.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.emailQueueUrl, emailQueue, emailQueue.id);
    }
  }


  newEmailQueue(emailQueue: EmailQueue) {
    return this.commonEndpoint.getNewEndpoint<EmailQueue>(this.emailQueueUrl, emailQueue);
  }


  deleteEmailQueue(emailQueueOrEmailQueueId: string | EmailQueue): Observable<EmailQueue> {
    return this.commonEndpoint.getDeleteEndpoint<EmailQueue>(this.emailQueueUrl, <string>emailQueueOrEmailQueueId);
  }
}
