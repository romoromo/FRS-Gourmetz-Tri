import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';
import { CommonEndpoint } from './common-endpoint.service';
import { AuthService } from './auth.service';
import { Filter, PagedResult, SalesOrderReportFilter, VoucherUtilisationReportFilter } from '../models/sieve-filter.model';
import { ConfigurationService } from './configuration.service';
import { AuditLog } from '../models/audit-log';


@Injectable()
export class ReportService {

  private readonly _reportUrl: string = "/api/report";
  get reportUrl() { return this.configurations.baseUrl + this._reportUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private commonEndpoint: CommonEndpoint, protected configurations: ConfigurationService) {

  }

  getCancelledOrdersByFilter(filter: SalesOrderReportFilter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.reportUrl + '/orders/cancellation', filter);
  }

  downloadCancelledOrdersReport(filter: Filter) {
    return this.commonEndpoint.getFile<any>(this.reportUrl + '/orders/cancellation/export', filter);
  }

  downloadFlattenCancelledOrdersReport(filter: Filter) {
    return this.commonEndpoint.getFile<any>(this.reportUrl + '/exportorders/flatten', filter);
  }

  getVoucherUtilisationsByFilter(filter: VoucherUtilisationReportFilter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.reportUrl + '/vouchers/ulisation', filter);
  }

  downloadVoucherUtilisationssReport(filter: VoucherUtilisationReportFilter) {
    return this.commonEndpoint.getFile<any>(this.reportUrl + '/vouchers/ulisation/export', filter);
  }
}
