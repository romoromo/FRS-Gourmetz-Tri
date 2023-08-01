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
import { ContactUsDetail, ContactUsSubject } from 'src/app/models/meal-order/contact-us-subject.model';
import { EmailTemplate } from 'src/app/models/meal-order/email-template';

@Injectable()
export class EmailService {

  private readonly _contactUsSubjectUrl: string = "/api/email/contactussubjects";
  get contactUsSubjectUrl() { return this.configurations.baseUrl + this._contactUsSubjectUrl; }

  private readonly _contactUsDetailUrl: string = "/api/email/contactusdetails";
  get contactUsDetailUrl() { return this.configurations.baseUrl + this._contactUsDetailUrl; }

  private readonly _emailTemplateUrl: string = "/api/email/emailtemplates";
  get emailTemplateUrl() { return this.configurations.baseUrl + this._emailTemplateUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private accountEndpoint: AccountEndpoint, private commonEndpoint: CommonEndpoint, protected configurations: ConfigurationService) {

  }

  //contact us subject
  getContactUsSubjectById(contactUsSubjectId: string) {

    return this.commonEndpoint.getById<any>(this.contactUsSubjectUrl + '/get', contactUsSubjectId);
  }

  getContactUsSubjectsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.contactUsSubjectUrl + '/sieve/list', filter);
  }

  updateContactUsSubject(contactUsSubject: ContactUsSubject) {
    if (contactUsSubject.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.contactUsSubjectUrl, contactUsSubject, contactUsSubject.id);
    }
  }


  newContactUsSubject(contactUsSubject: ContactUsSubject) {
    return this.commonEndpoint.getNewEndpoint<ContactUsSubject>(this.contactUsSubjectUrl, contactUsSubject);
  }


  deleteContactUsSubject(contactUsSubjectOrContactUsSubjectId: string | ContactUsSubject): Observable<ContactUsSubject> {
    return this.commonEndpoint.getDeleteEndpoint<ContactUsSubject>(this.contactUsSubjectUrl, <string>contactUsSubjectOrContactUsSubjectId);
  }

  //contact us detail
  getContactUsDetailById(contactUsDetailId: string) {

    return this.commonEndpoint.getById<any>(this.contactUsDetailUrl + '/get', contactUsDetailId);
  }

  getContactUsDetailsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.contactUsDetailUrl + '/sieve/list', filter);
  }

  updateContactUsDetail(contactUsDetail: ContactUsDetail) {
    if (contactUsDetail.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.contactUsDetailUrl, contactUsDetail, contactUsDetail.id);
    }
  }


  newContactUsDetail(contactUsDetail: ContactUsDetail) {
    return this.commonEndpoint.getNewEndpoint<ContactUsDetail>(this.contactUsDetailUrl, contactUsDetail);
  }


  deleteContactUsDetail(contactUsDetailOrContactUsDetailId: string | ContactUsDetail): Observable<ContactUsDetail> {
    return this.commonEndpoint.getDeleteEndpoint<ContactUsDetail>(this.contactUsDetailUrl, <string>contactUsDetailOrContactUsDetailId);
  }

  //email template
  getEmailTemplateById(emailTemplateId: string) {

    return this.commonEndpoint.getById<any>(this.emailTemplateUrl + '/get', emailTemplateId);
  }

  getEmailTemplatesByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.emailTemplateUrl + '/sieve/list', filter);
  }

  updateEmailTemplate(emailTemplate: EmailTemplate) {
    if (emailTemplate.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.emailTemplateUrl, emailTemplate, emailTemplate.id);
    }
  }


  newEmailTemplate(emailTemplate: EmailTemplate) {
    return this.commonEndpoint.getNewEndpoint<EmailTemplate>(this.emailTemplateUrl, emailTemplate);
  }


  deleteEmailTemplate(emailTemplateOrEmailTemplateId: string | EmailTemplate): Observable<EmailTemplate> {
    return this.commonEndpoint.getDeleteEndpoint<EmailTemplate>(this.emailTemplateUrl, <string>emailTemplateOrEmailTemplateId);
  }

  sendTestEmail(templateId: string, email: string) {
    return this.commonEndpoint.get<any>(`${this.emailTemplateUrl}/sendtestemail?templateId=${templateId}&email=${email}`);
  }
}
