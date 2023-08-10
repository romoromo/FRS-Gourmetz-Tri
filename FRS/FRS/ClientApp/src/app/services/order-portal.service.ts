import { Injectable } from '@angular/core';
import { Observable, interval } from 'rxjs';
import { map, flatMap, startWith } from 'rxjs/operators';

import { AuthService } from './auth.service';
import { Filter, PagedResult } from '../models/sieve-filter.model';
import { ConfigurationService } from './configuration.service';
import { CommonEndpoint } from './common-endpoint.service';
import { AccountEndpoint } from './account-endpoint.service';
import { OrderPortalContent } from '../models/meal-order/order-portal-content.model';



@Injectable()
export class OrderPortalService {


  private readonly _orderPortalContentUrl: string = "/api/orderportal/contents";
  get orderPortalContentUrl() { return this.configurations.baseUrl + this._orderPortalContentUrl; }


  constructor(private authService: AuthService,
    private accountEndpoint: AccountEndpoint, private commonEndpoint: CommonEndpoint, protected configurations: ConfigurationService  ) {

  }

  getOrderPortalContentFirst(outletId: string) {
    return this.commonEndpoint.get<any>(`${this.orderPortalContentUrl}/first?outletId=${outletId}`);
  }

  getOrderPortalContentById(id?: string) {
    return this.commonEndpoint.getById<any>(this.orderPortalContentUrl + '/get', id);
  }

  updateOrderPortalContent(orderPortalContent: OrderPortalContent) {
    if (orderPortalContent.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.orderPortalContentUrl, orderPortalContent, orderPortalContent.id);
    }
  }

  getOrderPortalContentsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.orderPortalContentUrl + '/sieve/list', filter);
  }
}
