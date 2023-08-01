import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';




import { CommonEndpoint } from './common-endpoint.service';
import { AuthService } from './auth.service';
import { Module } from '../models/module.model';
import { ConfigurationService } from './configuration.service';

export type ModulesChangedOperation = "add" | "delete" | "modify";
export type ModulesChangedEventArg = { modules: Module[] | string[], operation: ModulesChangedOperation };

@Injectable()
export class ModuleService {

  public static readonly moduleAddedOperation: ModulesChangedOperation = "add";
  public static readonly moduleDeletedOperation: ModulesChangedOperation = "delete";
  public static readonly moduleModifiedOperation: ModulesChangedOperation = "modify";

  private _modulesChanged = new Subject<ModulesChangedEventArg>();

  private readonly _moduleUrl: string = "/api/module";
  get moduleUrl() { return this.configurations.baseUrl + this._moduleUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private commonEndpoint: CommonEndpoint, private configurations: ConfigurationService) {

  }


  private onModulesChanged(modules: Module[] | string[], op: ModulesChangedOperation) {
    this._modulesChanged.next({ modules: modules, operation: op });
  }


  onModulesCountChanged(modules: Module[] | string[]) {
    return this.onModulesChanged(modules, ModuleService.moduleModifiedOperation);
  }


  getModulesChangedEvent(): Observable<ModulesChangedEventArg> {
    return this._modulesChanged.asObservable();
  }

  getModuleById(moduleId: string) {

    return this.commonEndpoint.getById<any>(this.moduleUrl + '/get', moduleId);
  }

  getModules(page?: number, pageSize?: number) {
    return this.commonEndpoint.get<Module[]>(this.moduleUrl + '/modules/list');
  }

  updateModule(module: Module) {
    if (module.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.moduleUrl, module, module.id).pipe(
        tap(data => this.onModulesChanged([module], ModuleService.moduleModifiedOperation)));
    }
  }


  newModule(module: Module) {
    return this.commonEndpoint.getNewEndpoint<Module>(this.moduleUrl, module).pipe<Module>(
      tap(data => this.onModulesChanged([module], ModuleService.moduleAddedOperation)));
  }


  deleteModule(module: string | Module): Observable<Module> {

    if (typeof module === 'number' || module instanceof Number ||
      typeof module === 'string' || module instanceof String) {
      return this.commonEndpoint.getDeleteEndpoint<Module>(this.moduleUrl, <string>module).pipe<Module>(
        tap(data => this.onModulesChanged([data], ModuleService.moduleDeletedOperation)));
    }
    else {

      if (module.id) {
        return this.deleteModule(module.id);
      }
    }
  }
}
