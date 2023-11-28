import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';
import { CommonEndpoint } from '../common-endpoint.service';
import { AuthService } from '../auth.service';
import { ConfigurationService } from '../configuration.service';
import { OrderCancellationFilter, StudentOrderFilter, PagedResult } from 'src/app/models/sieve-filter.model';


@Injectable()
export class OrderService {

  private readonly _orderUrl: string = "/api/tokenorder";
  get orderUrl() { return this.configurations.baseUrl + this._orderUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private commonEndpoint: CommonEndpoint, protected configurations: ConfigurationService) {

  }

  getOrderCancellationsByFilter(filter: OrderCancellationFilter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.orderUrl + '/cancellations/sieve/list', filter);
  }

  cancelOrders(model): Observable<any> {
    return this.commonEndpoint.getNewEndpoint<any>(this.orderUrl + '/cancellations/cancel', model);
  }

  getStudentOrdersByFilter(filter: StudentOrderFilter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.orderUrl + '/students/sieve/list', filter);
  }

  amendOrder(model): Observable<any> {
    return this.commonEndpoint.getNewEndpoint<any>(this.orderUrl + '/students/amend', model);
  }

  createPrepaidOrder(model: any): Observable<any> {
    return this.commonEndpoint.getNewEndpoint<any>(this.orderUrl + '/students/prepaid', model);
  }
}
