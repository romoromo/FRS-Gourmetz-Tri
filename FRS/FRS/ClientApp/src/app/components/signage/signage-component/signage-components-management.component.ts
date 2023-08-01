import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, Inject, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { SignageComponent } from '../../../models/signage-component.model';
import { Permission } from '../../../models/permission.model';
import { SignageComponentEditorComponent } from "./signage-component-editor.component";
import { SignageComponentService } from 'src/app/services/signage-component.service';
import { MatDialog } from '@angular/material';


@Component({
  selector: 'signage-components-management',
  templateUrl: './signage-components-management.component.html',
  styleUrls: ['./signage-components-management.component.css']
})
export class SignageComponentsManagementComponent implements OnInit, AfterViewInit {
  columns: any[] = [];
  rows: SignageComponent[] = [];
  rowsCache: SignageComponent[] = [];
  allPermissions: Permission[] = [];
  editedSignageComponent: SignageComponent;
  sourceSignageComponent: SignageComponent;
  editingSignageComponentName: { name: string };
  loadingIndicator: boolean;

  exportList: any = {};

  showEditor: boolean;

  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('signageComponentEditor')
  signageComponentEditor: SignageComponentEditorComponent;
  header: string;
    importFile: any;
    keyword: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private signageComponentService: SignageComponentService) {
  }

  //openDialog(signageComponent: SignageComponent): void {
  //  const dialogRef = this.dialog.open(SignageComponentEditorComponent, {
  //    data: { header: this.header, signageComponent: signageComponent },
  //    width: '400px'
  //  });

  //  dialogRef.afterClosed().subscribe(result => {
  //    this.loadData();
  //  });
  //}

  ngOnInit() {

    let gT = (key: string) => this.translationService.getTranslation(key);

    this.columns = [
      { prop: "index", name: '#', width: 50, cellTemplate: this.indexTemplate, canAutoResize: false },
      { prop: 'name', name: gT('signageComponents.management.Name') },
      { prop: 'description', name: gT('signageComponents.management.Description') },
      { prop: 'createdByName', name: 'Created By' },
      { prop: 'lastModified', name: 'Last Modified' },
      { name: '', width: 200, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
    ];

    if (!this.accountService.currentUser.institutionId || this.accountService.currentUser.institutionId == '0') {
      this.columns.splice(1, 0, { prop: 'institutionName', name: gT('roles.management.Institution'), width: 120 });
    }
    this.loadData();
  }





  ngAfterViewInit() {

    this.signageComponentEditor.changesSavedCallback = () => {
      //this.addNewSignageComponentToList();
      //this.editorModal.hide();
      this.showEditor = false;
      this.loadData();
    };

    this.signageComponentEditor.changesCancelledCallback = () => {
      this.editedSignageComponent = null;
      this.sourceSignageComponent = null;
      //this.editorModal.hide();
      this.showEditor = false;
    };
  }

  addToExport(row) {
    if (!this.exportList[row.id]) this.exportList[row.id] = {};

    this.exportList[row.id].included = true;
    this.exportList[row.id].id = row.id;
  }

  removeFromExport(row) {
    if (this.exportList[row.id]) {
      this.exportList[row.id].included = false;
    }
  }

  numberOfExports() {
    return (<any>Object.values(this.exportList)).filter(i => i.included).length;
  }

  export() {
    let ids = (<any>Object.values(this.exportList)).filter(i => i.included).map(i => i.id).join();

    this.alertService.startLoadingMessage();
    this.loadingIndicator = true;

    this.signageComponentService.exportSignageComponents(ids)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let comps = results[0];

        let filename = `ComponentExports-${new Date().toLocaleString('sv-SE').replace(/-|:| /g, '')}.json`;

        var blob = new Blob([JSON.stringify(comps, null, 4)], { type: 'text/plain;charset=utf-8;' });
        if (navigator.msSaveBlob) { // IE 10+
          navigator.msSaveBlob(blob, filename);
        } else {
          var link = document.createElement("a");
          if (link.download !== undefined) { // feature detection
            // Browsers that support HTML5 download attribute
            var url = URL.createObjectURL(blob);
            link.setAttribute("href", url);
            link.setAttribute("download", filename);
            link.style.visibility = 'hidden';
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
          }
        }

        this.exportList = {};
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve signage components from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  importClicked() {
      document.getElementById('file').click()
  }

  public uploadFinished = (event) => {
    this.importFile = event ? event.dbPath : null;

    var r = confirm(`Are you sure you want to import from selected file?\nAny data with same name will be overwritten, and you cannot undo this action.`);
    if (r) {
      this.alertService.startLoadingMessage();
      this.loadingIndicator = true;
      this.signageComponentService.importSignageComponents(this.importFile)
        .subscribe(results => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.loadData();

          this.alertService.showMessage("Import Success", "Import Success", MessageSeverity.success);
        },
          error => {
            this.alertService.stopLoadingMessage();
            this.loadingIndicator = false;

            this.alertService.showStickyMessage("Load Error", `Unable to import signage components to the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
              MessageSeverity.error);
          });
    }
  }

  addNewSignageComponentToList() {
    if (this.sourceSignageComponent) {
      Object.assign(this.sourceSignageComponent, this.editedSignageComponent);

      let sourceIndex = this.rowsCache.indexOf(this.sourceSignageComponent, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rowsCache, sourceIndex, 0);

      sourceIndex = this.rows.indexOf(this.sourceSignageComponent, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rows, sourceIndex, 0);

      this.editedSignageComponent = null;
      this.sourceSignageComponent = null;
    }
    else {
      let signageComponent = new SignageComponent();
      Object.assign(signageComponent, this.editedSignageComponent);
      this.editedSignageComponent = null;

      let maxIndex = 0;
      for (let r of this.rowsCache) {
        if ((<any>r).index > maxIndex)
          maxIndex = (<any>r).index;
      }

      (<any>signageComponent).index = maxIndex + 1;

      this.rowsCache.splice(0, 0, signageComponent);
      this.rows.splice(0, 0, signageComponent);
      this.rows = [...this.rows];
    }
  }




  loadData() {
    this.alertService.startLoadingMessage();
    this.loadingIndicator = true;

    this.signageComponentService.getSignageComponentByInstitutionId(this.accountService.currentUser.institutionId)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let signageComponents = results[0];
        let permissions = results[1];

        signageComponents.forEach((signageComponent, index, signageComponents) => {
          (<any>signageComponent).index = index + 1;
          (<any>signageComponent).lastModified = new Date((<any>signageComponent).updatedDate).toLocaleString('sv-SE');
        });


        this.rowsCache = [...signageComponents];
        this.rows = signageComponents;

        this.onSearchChanged(this.keyword);
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve signage components from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }


  onSearchChanged(value: string) {
    this.keyword = value;
    this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.name, r.description));
  }


  onEditorModalHidden() {
    this.editingSignageComponentName = null;
    this.signageComponentEditor.resetForm(true);
  }


  newSignageComponent() {
    this.editingSignageComponentName = null;
    this.sourceSignageComponent = null;
    this.editedSignageComponent = this.signageComponentEditor.newSignageComponent();
    this.header = 'New Signage Component';
    //this.openDialog(this.editedSignageComponent);
    //this.editorModal.show();
    this.showEditor = true;
  }


  editSignageComponent(row: SignageComponent) {
    this.editingSignageComponentName = { name: row.name };
    this.sourceSignageComponent = row;
    this.editedSignageComponent = this.signageComponentEditor.editSignageComponent(row);
    this.header = 'Edit Signage Component';
    //this.editorModal.show();
    //this.openDialog(this.editedSignageComponent);
    this.showEditor = true;
  }

  deleteSignageComponent(row: SignageComponent) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" signage component?', DialogType.confirm, () => this.deleteSignageComponentHelper(row));
  }


  deleteSignageComponentHelper(row: SignageComponent) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.signageComponentService.deleteSignageComponent(row)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.rowsCache = this.rowsCache.filter(item => item !== row)
        this.rows = this.rows.filter(item => item !== row)
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the signage component.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }


  get canCreateSignageComponents() {
    return this.accountService.userHasPermission(Permission.createComponentsPermission)
  }

  get canUpdateSignageComponents() {
    return this.accountService.userHasPermission(Permission.updateComponentsPermission)
  }

  get canDeleteSignageComponents() {
    return this.accountService.userHasPermission(Permission.deleteComponentsPermission)
  }

}
