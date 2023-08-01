import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Module } from '../../../models/module.model';
import { Permission } from '../../../models/permission.model';
import { ModuleEditorComponent } from "./module-editor.component";
import { ModuleService } from 'src/app/services/module.service';
import { MatDialog } from '@angular/material';
import { Subscription } from 'rxjs';


@Component({
  selector: 'modules-management',
  templateUrl: './modules-management.component.html',
  styleUrls: ['./modules-management.component.css']
})
export class ModulesManagementComponent implements OnInit, AfterViewInit, OnDestroy {
  private subscription: Subscription = new Subscription();
  columns: any[] = [];
  rows: Module[] = [];
  rowsCache: Module[] = [];
  allPermissions: Permission[] = [];
  editedModule: Module;
  sourceModule: Module;
  editingModuleName: { name: string };
  loadingIndicator: boolean;



  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('moduleEditor')
  moduleEditor: ModuleEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private moduleService: ModuleService, public dialog: MatDialog) {
  }

  openDialog(module: Module): void {
    const dialogRef = this.dialog.open(ModuleEditorComponent, {
      data: { header: this.header, module: module },
      width: '800px'
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData();
    });
  }

  ngOnInit() {

    let gT = (key: string) => this.translationService.getTranslation(key);

    this.columns = [
      { prop: "index", name: '#', width: 50, cellTemplate: this.indexTemplate, canAutoResize: false },
      { prop: 'name', name: gT('modules.management.Name') },
      { prop: 'route', name: gT('modules.management.Route') },
      { prop: 'uri', name: gT('modules.management.Uri') },
      { name: '', width: 150, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
    ];

    this.loadData();
  }





  ngAfterViewInit() {

    this.moduleEditor.changesSavedCallback = () => {
      this.addNewModuleToList();
      this.editorModal.hide();
    };

    this.moduleEditor.changesCancelledCallback = () => {
      this.editedModule = null;
      this.sourceModule = null;
      this.editorModal.hide();
    };
  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  addNewModuleToList() {
    if (this.sourceModule) {
      Object.assign(this.sourceModule, this.editedModule);

      let sourceIndex = this.rowsCache.indexOf(this.sourceModule, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rowsCache, sourceIndex, 0);

      sourceIndex = this.rows.indexOf(this.sourceModule, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rows, sourceIndex, 0);

      this.editedModule = null;
      this.sourceModule = null;
    }
    else {
      let facility = new Module();
      Object.assign(facility, this.editedModule);
      this.editedModule = null;

      let maxIndex = 0;
      for (let r of this.rowsCache) {
        if ((<any>r).index > maxIndex)
          maxIndex = (<any>r).index;
      }

      (<any>facility).index = maxIndex + 1;

      this.rowsCache.splice(0, 0, facility);
      this.rows.splice(0, 0, facility);
      this.rows = [...this.rows];
    }
  }




  loadData() {
    this.alertService.startLoadingMessage();
    this.loadingIndicator = true;

    this.subscription.add(this.moduleService.getModules()
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let modules = results;

        modules.forEach((facility, index, modules) => {
          (<any>facility).index = index + 1;
        });


        this.rowsCache = [...modules];
        this.rows = modules;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve modules from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }


  onSearchChanged(value: string) {
    this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.name, r.route, r.uri));
  }


  onEditorModalHidden() {
    this.editingModuleName = null;
    this.moduleEditor.resetForm(true);
  }


  newModule() {
    this.editingModuleName = null;
    this.sourceModule = null;
    this.editedModule = this.moduleEditor.newModule();
    //this.editorModal.show();
    this.header = 'New Module';
    this.openDialog(this.editedModule);
  }


  editModule(row: Module) {
    this.editingModuleName = { name: row.name };
    this.sourceModule = row;
    this.editedModule = this.moduleEditor.editModule(row);
    //this.editorModal.show();

    this.header = 'Edit Module';
    this.openDialog(this.editedModule);
  }

  deleteModule(row: Module) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" module?', DialogType.confirm, () => this.deleteModuleHelper(row));
  }


  deleteModuleHelper(row: Module) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.subscription.add(this.moduleService.deleteModule(row)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.rowsCache = this.rowsCache.filter(item => item !== row)
        this.rows = this.rows.filter(item => item !== row)
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the module.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }


  get canManageModules() {
    return true; //this.accountService.userHasPermission(Permission.manageModulesPermission)
  }

}
