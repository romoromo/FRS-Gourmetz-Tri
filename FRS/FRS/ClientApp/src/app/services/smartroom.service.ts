import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';
import { CommonEndpoint } from './common-endpoint.service';
import { AuthService } from './auth.service';
import { Filter, PagedResult } from '../models/sieve-filter.model';
import { ConfigurationService } from './configuration.service';
import { AuditLog } from '../models/audit-log';


@Injectable()
export class SmartRoomSchedulerService {

  private readonly _auditUrl: string = "/api/smartRoomScheduler";
  get auditUrl() { return this.configurations.baseUrl + this._auditUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private commonEndpoint: CommonEndpoint, protected configurations: ConfigurationService) {

  }

  getLogsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.auditUrl + '/logs/sieve/list', filter);
  }

  getDataLogsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.auditUrl + '/datalogs/sieve/list', filter);
  }

  getDataLogDetailsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.auditUrl + '/datalogs/details/sieve/list', filter);
  }

  getDataLogDetailsById(id: string) {
    return this.commonEndpoint.getById<AuditLog[]>(this.auditUrl + '/datalogs', id);
  }

}
