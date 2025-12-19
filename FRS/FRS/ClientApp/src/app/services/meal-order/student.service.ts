import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';
import { AccountEndpoint } from '../account-endpoint.service';
import { AuthService } from '../auth.service';
import { CommonEndpoint } from '../common-endpoint.service';
import { ConfigurationService } from '../configuration.service';
import { Filter, PagedResult } from 'src/app/models/sieve-filter.model';
import { Student } from 'src/app/models/meal-order/student.model';
import { StudentGroup, StudentGroupDetail } from 'src/app/models/meal-order/student-group.model';
import { InterestGroup } from 'src/app/models/meal-order/interest-group.model';
import { Class } from 'src/app/models/meal-order/class.model';

@Injectable()
export class StudentService {

  private readonly _studentUrl: string = "/api/student/students";
  get studentUrl() { return this.configurations.baseUrl + this._studentUrl; }

  private readonly _studentGroupUrl: string = "/api/student/studentgroups";
  get studentGroupUrl() { return this.configurations.baseUrl + this._studentGroupUrl; }

  private readonly _voucherUrl: string = "/api/student/vouchers";
  get voucherUrl() { return this.configurations.baseUrl + this._voucherUrl; }

  private readonly _outletTermUrl: string = "/api/student/outlets/terms";
  get outletTermUrl() { return this.configurations.baseUrl + this._outletTermUrl; }

  private readonly _walletUrl: string = "/api/student/wallet";
  get walletUrl() { return this.configurations.baseUrl + this._walletUrl; }

  private readonly _pointUrl: string = "/api/student/point";
  get pointUrl() { return this.configurations.baseUrl + this._pointUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private accountEndpoint: AccountEndpoint, private commonEndpoint: CommonEndpoint, protected configurations: ConfigurationService) {

  }

  //outlet terms
  getOutletTermsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.outletTermUrl + '/sieve/list', filter);
  }

  //student level
  getStudentById(studentId: string) {

    return this.commonEndpoint.getById<any>(this.studentUrl + '/get', studentId);
  }

  getStudentsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.studentUrl + '/sieve/list', filter);
  }

  updateStudent(student: Student) {
    if (student.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.studentUrl, student, student.id);
    }
  }


  newStudent(student: Student) {
    return this.commonEndpoint.getNewEndpoint<Student>(this.studentUrl, student);
  }


  deleteStudent(studentOrStudentId: string | Student): Observable<Student> {
    return this.commonEndpoint.getDeleteEndpoint<Student>(this.studentUrl, <string>studentOrStudentId);
  }

  importFile<T>(data: any): Observable<T> {
    return this.commonEndpoint.importFile(this.studentUrl + '/import', data);
  }

  importStudentCardFile<T>(data: any): Observable<T> {
    return this.commonEndpoint.importFile(this.studentUrl + '/cards/import', data);
  }

  downloadTemplateStudentCardFile<T>(): Observable<T> {
    return this.commonEndpoint.getFile<any>(this.studentUrl + '/cards/import/template', {});
  }

  downloadTemplateWithStudent(filter: Filter) {
    return this.commonEndpoint.getFile<any>(this.studentUrl + '/cards/import/template-with-details', filter);
  }

  createAccount(ids: string[]) {
    return this.commonEndpoint.get(this.studentUrl + '/createaccount', true, ids);
  }

  downloadReport(filter: Filter) {
    return this.commonEndpoint.getFile<any>(this.studentUrl + '/report/export', filter);
  }

  transferClassStudent(classData: any, originClassId: number) {
    if (classData.classId) {
      return this.commonEndpoint.getTransferClassStudentEndpoint(this.studentUrl, classData, originClassId);
    }
  }

  //interest group

  private readonly _interestGroupUrl: string = "/api/student/interestgroups";
  get interestGroupUrl() { return this.configurations.baseUrl + this._interestGroupUrl; }

  getInterestGroupById(interestGroupId: string) {

    return this.commonEndpoint.getById<any>(this.interestGroupUrl + '/get', interestGroupId);
  }

  getInterestGroupsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.interestGroupUrl + '/sieve/list', filter);
  }

  updateInterestGroup(interestGroup: InterestGroup) {
    if (interestGroup.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.interestGroupUrl, interestGroup, interestGroup.id);
    }
  }


  newInterestGroup(interestGroup: InterestGroup) {
    return this.commonEndpoint.getNewEndpoint<InterestGroup>(this.interestGroupUrl, interestGroup);
  }


  deleteInterestGroup(interestGroupOrInterestGroupId: string | InterestGroup): Observable<InterestGroup> {
    return this.commonEndpoint.getDeleteEndpoint<InterestGroup>(this.interestGroupUrl, <string>interestGroupOrInterestGroupId);
  }

  sendEmailToStudents(templateId: string, id: string) {
    return this.commonEndpoint.get<any>(`${this.interestGroupUrl}/sendemail/${id}/${templateId}`);
  }


  //Student Group

  getStudentGroupById(id: string) {

    return this.commonEndpoint.getById<any>(this.studentGroupUrl + '/get', id);
  }

  getStudentGroupsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.studentGroupUrl + '/sieve/list', filter);
  }

  getStudentGroupsSimpleByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.studentGroupUrl + '/simple/list', filter);
  }

  updateStudentGroup(group: StudentGroup) {
    if (group.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.studentGroupUrl, group, group.id);
    }
  }


  newStudentGroup(group: StudentGroup) {
    return this.commonEndpoint.getNewEndpoint<StudentGroup>(this.studentGroupUrl, group);
  }


  deleteStudentGroup(studentGroupOrStudentGroupId: string | StudentGroup): Observable<StudentGroup> {
    return this.commonEndpoint.getDeleteEndpoint<StudentGroup>(this.studentGroupUrl, <string>studentGroupOrStudentGroupId);
  }

  addStudentVoucher(studentId: string, code: string) {
    return this.commonEndpoint.get<any>(this.voucherUrl + '/add?studentId=' + studentId + '&code=' + code);
  }

  generateDishCode(catererId: string) {
    return this.commonEndpoint.get<any>(this.studentGroupUrl + '/generatecode?catererId=' + catererId);
  }

  studentListExport(outletId: number, studentGroupId: number) {
    return this.commonEndpoint.getFile<any>(this.studentUrl + `/list/export/${outletId}/${studentGroupId}`);
  }

  importFileStudentGroup<T>(data: any): Observable<T> {
    return this.commonEndpoint.importFile(this.studentUrl + '/import/studentgroup', data);
  }

  addStudentVoucherByStudentGroup(studentGroupId: string, code: string) {
    return this.commonEndpoint.get<any>(this.voucherUrl + `/addbystudentgroup/${studentGroupId}/${code}`);
  }

  getStudentWalletTransactionsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.walletUrl + '/transactions/sieve/list', filter);
  }

  downloadWalletTransactions(filter: Filter){
    return this.commonEndpoint.getFile<any>(this.walletUrl + '/exportwallettransaction' , filter);
  }

  walletTopupForStudentGroup(studentGroupId: string, amount: number, userId: string,type:string) {
    const param = {
      studentGroupId: studentGroupId,
      amount: amount,
      userId: userId,
      type: type
    }
    return this.commonEndpoint.getNewEndpoint<any>(this.walletUrl + `/studentgroup`, param);
  }

  walletTopupForStudent(studentId: string, amount: number, userId: string,type:string) {
    const param = {
      studentGroupId: studentId,
      amount: amount,
      userid: userId,
      type: type
    }
    return this.commonEndpoint.getNewEndpoint<any>(this.walletUrl + `/student`, param);
  }

  getStudentPointTransactionsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.pointUrl + '/transactions/sieve/list', filter);
  }

  pointTopupForStudentGroup(studentGroupId: string, amount: number, userId: string,type:string) {
    const param = {
      studentGroupId: studentGroupId,
      amount: amount,
      userId,
      type
    }
    return this.commonEndpoint.getNewEndpoint<any>(this.pointUrl + `/studentgroup`, param);
  }

  pointTopupForStudent(studentId: string, amount: number, userId: string,type:string) {
    const param = {
      studentGroupId: studentId,
      amount: amount,
      userId,
      type
    }
    return this.commonEndpoint.getNewEndpoint<any>(this.pointUrl + `/student`, param);
  }

  offBoardingStudent(studentId: string, userId: string) {
    const param = {
      studentGroupId: studentId,
      userId: userId
    }
    return this.commonEndpoint.getNewEndpoint<any>(this.walletUrl + `/student-off-boarding`, param);
  }
}
