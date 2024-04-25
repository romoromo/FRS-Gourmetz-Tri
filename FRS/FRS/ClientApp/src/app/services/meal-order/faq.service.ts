import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';
import { AccountEndpoint } from '../account-endpoint.service';
import { AuthService } from '../auth.service';
import { CommonEndpoint } from '../common-endpoint.service';
import { ConfigurationService } from '../configuration.service';
import {  Filter, PagedResult } from 'src/app/models/sieve-filter.model';
import { FaqDetail, FaqSubject } from 'src/app/models/meal-order/faq-subject.model';

@Injectable()
export class FaqService {

  private readonly _faqSubjectUrl: string = "/api/faq/subjects";
  get faqSubjectUrl() { return this.configurations.baseUrl + this._faqSubjectUrl; }

  private readonly _faqDetailUrl: string = "/api/faq/details";
  get faqDetailUrl() { return this.configurations.baseUrl + this._faqDetailUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private accountEndpoint: AccountEndpoint, private commonEndpoint: CommonEndpoint, protected configurations: ConfigurationService) {

  }

  //faq subject
  getFaqSubjectById(faqSubjectId: string) {

    return this.commonEndpoint.getById<any>(this.faqSubjectUrl + '/get', faqSubjectId);
  }

  getFaqSubjectsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.faqSubjectUrl + '/sieve/list', filter);
  }

  updateFaqSubject(faqSubject: FaqSubject) {
    if (faqSubject.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.faqSubjectUrl, faqSubject, faqSubject.id);
    }
  }


  newFaqSubject(faqSubject: FaqSubject) {
    return this.commonEndpoint.getNewEndpoint<FaqSubject>(this.faqSubjectUrl, faqSubject);
  }


  deleteFaqSubject(faqSubjectOrFaqSubjectId: string | FaqSubject): Observable<FaqSubject> {
    return this.commonEndpoint.getDeleteEndpoint<FaqSubject>(this.faqSubjectUrl, <string>faqSubjectOrFaqSubjectId);
  }

  //faq detail
  getFaqDetailById(faqDetailId: string) {

    return this.commonEndpoint.getById<any>(this.faqDetailUrl + '/get', faqDetailId);
  }

  getFaqDetailsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.faqDetailUrl + '/sieve/list', filter);
  }

  updateFaqDetail(faqDetail: FaqDetail) {
    if (faqDetail.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.faqDetailUrl, faqDetail, faqDetail.id);
    }
  }


  newFaqDetail(faqDetail: FaqDetail) {
    return this.commonEndpoint.getNewEndpoint<FaqDetail>(this.faqDetailUrl, faqDetail);
  }


  deleteFaqDetail(faqDetailOrFaqDetailId: string | FaqDetail): Observable<FaqDetail> {
    return this.commonEndpoint.getDeleteEndpoint<FaqDetail>(this.faqDetailUrl, <string>faqDetailOrFaqDetailId);
  }

  orderFaqDetail(faqDetailId: string, isAsc: boolean): Observable<any> {
    return this.commonEndpoint.get<any>(`${this.faqDetailUrl}/order/${faqDetailId}?isAsc=${isAsc}`);
  }

  orderFaqSubject(id: string, isAsc: boolean): Observable<any> {
    return this.commonEndpoint.get<any>(`${this.faqSubjectUrl}/order/${id}?isAsc=${isAsc}`);
  }
}
