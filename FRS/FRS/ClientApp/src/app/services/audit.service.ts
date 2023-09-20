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
export class AuditService {

  private readonly _auditUrl: string = "/api/audit";
  get auditUrl() { return this.configurations.baseUrl + this._auditUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private commonEndpoint: CommonEndpoint, protected configurations: ConfigurationService) {

  }

  getAuthLogsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.auditUrl + '/authlogs/sieve/list', filter);
  }

  getUpDownTimeLogsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.auditUrl + '/updowntimelogs/sieve/list', filter);
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

  getUserActivityLogsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.auditUrl + '/useractivity/sieve/list', filter);
  }

  downloadAuthLogReport(filter: Filter) {
    return this.commonEndpoint.getFile<any>(this.auditUrl + '/exportauthlogs' , filter);
  }

  downloadUpDownTimeLogReport(filter: Filter) {
    return this.commonEndpoint.getFile<any>(this.auditUrl + '/exportupdowntimelogs', filter);
  }

  downloadDataLogReport(filter: Filter) {
    return this.commonEndpoint.getFile<any>(this.auditUrl + '/exportdatalogs', filter);
  }

  getExternalLoginLogsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.auditUrl + '/externalloginlogs/sieve/list', filter);
  }

  downloadExternalLoginLogsReport(filter: Filter) {
    return this.commonEndpoint.getFile<any>(this.auditUrl + '/externalloginlogs', filter);
  }

  getOrderLogsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.auditUrl + '/orderlogs/sieve/list', filter);
  }

  downloadOrderLogsReport(filter: Filter) {
    return this.commonEndpoint.getFile<any>(this.auditUrl + '/exportorders', filter);
  }

  downloadFlattenOrderLogsReport(filter: Filter) {
    return this.commonEndpoint.getFile<any>(this.auditUrl + '/exportorders/flatten', filter);
  }

  downloadFlattenUserActivityLogsReport(filter: Filter) {
    return this.commonEndpoint.getFile<any>(this.auditUrl + '/exportuseractivity/flatten', filter);
  }
}
