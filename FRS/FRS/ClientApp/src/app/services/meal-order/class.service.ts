import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';
import { AccountEndpoint } from '../account-endpoint.service';
import { AuthService } from '../auth.service';
import { CommonEndpoint } from '../common-endpoint.service';
import { ConfigurationService } from '../configuration.service';
import { ClassRosterFilter, Filter, PagedResult } from 'src/app/models/sieve-filter.model';
import { ClassLevel } from 'src/app/models/meal-order/class-level.model';
import { Class } from 'src/app/models/meal-order/class.model';
import { ClassBatch } from 'src/app/models/meal-order/class-batch.model';
import { OutletClassRoster } from 'src/app/models/meal-order/outlet-class-roster.model';
import { DispenserOutlet } from '../../models/meal-order/dispenser-outlet.model';
import { PlcModel } from '../../models/meal-order/plc.model';

@Injectable()
export class ClassService {

  private readonly _classUrl: string = "/api/class/classes";
  get classUrl() { return this.configurations.baseUrl + this._classUrl; }

  private readonly _classLevelUrl: string = "/api/class/classlevels";
  get classLevelUrl() { return this.configurations.baseUrl + this._classLevelUrl; }

  private readonly _classBatchUrl: string = "/api/class/classbatches";
  get classBatchUrl() { return this.configurations.baseUrl + this._classBatchUrl; }

  private readonly _classRosterUrl: string = "/api/class/classrosters";
  get classRosterUrl() { return this.configurations.baseUrl + this._classRosterUrl; }

  private readonly _dispenserOutletUrl: string = "/api/class/dispenseroutlets";
  get dispenserOutletUrl() { return this.configurations.baseUrl + this._dispenserOutletUrl; }

  private readonly _plcUrl: string = "/api/class/plc";
  get plcUrl() { return this.configurations.baseUrl + this._plcUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private accountEndpoint: AccountEndpoint, private commonEndpoint: CommonEndpoint, protected configurations: ConfigurationService) {

  }
  //class batch
  getClassBatchById(classBatchId: string) {

    return this.commonEndpoint.getById<any>(this.classBatchUrl + '/get', classBatchId);
  }

  getClassBatchesByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.classBatchUrl + '/sieve/list', filter);
  }

  updateClassBatch(classBatch: ClassBatch) {
    if (classBatch.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.classBatchUrl, classBatch, classBatch.id);
    }
  }


  newClassBatch(classBatch: ClassBatch) {
    return this.commonEndpoint.getNewEndpoint<ClassBatch>(this.classBatchUrl, classBatch);
  }


  deleteClassBatch(classBatchOrClassBatchId: string | ClassBatch): Observable<ClassBatch> {
    return this.commonEndpoint.getDeleteEndpoint<ClassBatch>(this.classBatchUrl, <string>classBatchOrClassBatchId);
  }

  //class level
  getClassLevelById(classLevelId: string) {

    return this.commonEndpoint.getById<any>(this.classLevelUrl + '/get', classLevelId);
  }

  getClassLevelsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.classLevelUrl + '/sieve/list', filter);
  }

  updateClassLevel(classLevel: ClassLevel) {
    if (classLevel.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.classLevelUrl, classLevel, classLevel.id);
    }
  }


  newClassLevel(classLevel: ClassLevel) {
    return this.commonEndpoint.getNewEndpoint<ClassLevel>(this.classLevelUrl, classLevel);
  }


  deleteClassLevel(classLevelOrClassLevelId: string | ClassLevel): Observable<ClassLevel> {
    return this.commonEndpoint.getDeleteEndpoint<ClassLevel>(this.classLevelUrl, <string>classLevelOrClassLevelId);
  }


  //class
  getClassById(classModelId: string) {

    return this.commonEndpoint.getById<any>(this.classUrl + '/get', classModelId);
  }

  getClassesByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.classUrl + '/sieve/list', filter);
  }

  updateClass(classModel: Class) {
    if (classModel.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.classUrl, classModel, classModel.id);
    }
  }


  newClass(classModel: Class) {
    return this.commonEndpoint.getNewEndpoint<Class>(this.classUrl, classModel);
  }


  deleteClass(classModelOrClassId: string | Class): Observable<Class> {
    return this.commonEndpoint.getDeleteEndpoint<Class>(this.classUrl, <string>classModelOrClassId);
  }

  //class roster
  getClassRosterById(id: string) {
    return this.commonEndpoint.getById<any>(this.classRosterUrl + '/get', id);
  }

  getClassRosterByOutletId(outletId: string, catererId: string, mealSessionId: string) {

    return this.commonEndpoint.get<any>(`${this.classRosterUrl}/${outletId}/${catererId}/${mealSessionId}`);
  }

  getClassRostersByFilter(filter: ClassRosterFilter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.classRosterUrl + '/sieve/list', filter);
  }

  updateClassRoster(model: OutletClassRoster) {
    if (model.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.classRosterUrl, model, model.id);
    }
  }


  newClassRoster(model: OutletClassRoster) {
    return this.commonEndpoint.getNewEndpoint<OutletClassRoster>(this.classRosterUrl, model);
  }


  deleteClassRoster(modelOrClassId: string | OutletClassRoster): Observable<OutletClassRoster> {
    return this.commonEndpoint.getDeleteEndpoint<OutletClassRoster>(this.classRosterUrl, <string>modelOrClassId);
  }

  downloadClassRoster(filter: Filter) {
    return this.commonEndpoint.getFile<any>(this.classRosterUrl + '/export', filter);
  }

  //class level
  getDispenserOutletById(dispenserOutletId: string) {

    return this.commonEndpoint.getById<any>(this.dispenserOutletUrl + '/get', dispenserOutletId);
  }

  getDispenserOutletsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.dispenserOutletUrl + '/sieve/list', filter);
  }

  updateDispenserOutlet(dispenserOutlet: DispenserOutlet) {
    if (dispenserOutlet.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.dispenserOutletUrl, dispenserOutlet, dispenserOutlet.id);
    }
  }


  newDispenserOutlet(dispenserOutlet: DispenserOutlet) {
    return this.commonEndpoint.getNewEndpoint<DispenserOutlet>(this.dispenserOutletUrl, dispenserOutlet);
  }


  deleteDispenserOutlet(dispenserOutletOrDispneserOutletId: string | DispenserOutlet): Observable<DispenserOutlet> {
    return this.commonEndpoint.getDeleteEndpoint<DispenserOutlet>(this.dispenserOutletUrl, <string>dispenserOutletOrDispneserOutletId);
  }

  getPLCByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.plcUrl + '/sieve/list', filter);
  }

  deletePLC(plcId: string | PlcModel): Observable<PlcModel> {
    return this.commonEndpoint.getDeleteEndpoint<PlcModel>(this.plcUrl, <string>plcId);
  }

  newPLC(plc: PlcModel) {
    return this.commonEndpoint.getNewEndpoint<PlcModel>(this.plcUrl, plc);
  }

  updatePLC(plc: PlcModel) {
    if (plc.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.plcUrl, plc, plc.id);
    }
  }

  getPeriodMealSession(outletId) {
    return this.commonEndpoint.get<any>(`${this.classLevelUrl}/periodmealsession?outletId=${outletId}`);
  }
}
