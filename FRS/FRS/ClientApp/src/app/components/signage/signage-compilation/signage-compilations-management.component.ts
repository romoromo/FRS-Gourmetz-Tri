import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, Inject } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { SignageCompilation } from '../../../models/signage-compilation.model';
import { Permission } from '../../../models/permission.model';
import { SignageCompilationEditorComponent } from "./signage-compilation-editor.component";
import { SignageCompilationService } from 'src/app/services/signage-compilation.service';
import { MatDialog } from '@angular/material';


@Component({
  selector: 'signage-compilations-management',
  templateUrl: './signage-compilations-management.component.html',
  styleUrls: ['./signage-compilations-management.component.css']
})
export class SignageCompilationsManagementComponent implements OnInit, AfterViewInit {
  columns: any[] = [];
  rows: SignageCompilation[] = [];
  rowsCache: SignageCompilation[] = [];
  allPermissions: Permission[] = [];
  editedSignageCompilation: SignageCompilation;
  sourceSignageCompilation: SignageCompilation;
  editingSignageCompilationName: { name: string };
  loadingIndicator: boolean;

  exportList: any = {};

  showEditor: boolean;

  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('signageCompilationEditor')
  signageCompilationEditor: SignageCompilationEditorComponent;
  header: string;
    keyword: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private signageCompilationService: SignageCompilationService) {
  }

  //openDialog(signageCompilation: SignageCompilation): void {
  //  const dialogRef = this.dialog.open(SignageCompilationEditorComponent, {
  //    data: { header: this.header, signageCompilation: signageCompilation },
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
      { prop: 'name', name: gT('signageCompilations.management.Name') },
      { prop: 'description', name: gT('signageCompilations.management.Description') },
      { name: '', width: 200, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
    ];

    if (!this.accountService.currentUser.institutionId || this.accountService.currentUser.institutionId == '0') {
      this.columns.splice(1, 0, { prop: 'institutionName', name: gT('roles.management.Institution'), width: 120 });
    }
    this.loadData();
  }





  ngAfterViewInit() {

    this.signageCompilationEditor.changesSavedCallback = () => {
      //this.addNewSignageCompilationToList();
      //this.editorModal.hide();
      this.showEditor = false;
      this.loadData();
    };

    this.signageCompilationEditor.changesCancelledCallback = () => {
      this.editedSignageCompilation = null;
      this.sourceSignageCompilation = null;
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

    this.signageCompilationService.export(ids)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let comps = results[0];

        let filename = `CompilationExports-${new Date().toLocaleString('sv-SE').replace(/-|:| /g, '')}.json`;

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

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve signage compilations from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  importClicked() {
    document.getElementById('file').click()
  }

  public uploadFinished = (event) => {
    let importFile = event ? event.dbPath : null;

    var r = confirm(`Are you sure you want to import from selected file?\nAny data with same name will be overwritten, and you cannot undo this action.`);
    if (r) {
      this.alertService.startLoadingMessage();
      this.loadingIndicator = true;
      this.signageCompilationService.import(importFile)
        .subscribe(results => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.loadData();

          this.alertService.showMessage("Import Success", "Import Success", MessageSeverity.success);
        },
          error => {
            this.alertService.stopLoadingMessage();
            this.loadingIndicator = false;

            this.alertService.showStickyMessage("Load Error", `Unable to import signage compilations to the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
              MessageSeverity.error);
          });
    }
  }

  addNewSignageCompilationToList() {
    if (this.sourceSignageCompilation) {
      Object.assign(this.sourceSignageCompilation, this.editedSignageCompilation);

      let sourceIndex = this.rowsCache.indexOf(this.sourceSignageCompilation, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rowsCache, sourceIndex, 0);

      sourceIndex = this.rows.indexOf(this.sourceSignageCompilation, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rows, sourceIndex, 0);

      this.editedSignageCompilation = null;
      this.sourceSignageCompilation = null;
    }
    else {
      let signageCompilation = new SignageCompilation();
      Object.assign(signageCompilation, this.editedSignageCompilation);
      this.editedSignageCompilation = null;

      let maxIndex = 0;
      for (let r of this.rowsCache) {
        if ((<any>r).index > maxIndex)
          maxIndex = (<any>r).index;
      }

      (<any>signageCompilation).index = maxIndex + 1;

      this.rowsCache.splice(0, 0, signageCompilation);
      this.rows.splice(0, 0, signageCompilation);
      this.rows = [...this.rows];
    }
  }




  loadData() {
    this.alertService.startLoadingMessage();
    this.loadingIndicator = true;

    this.signageCompilationService.getSignageCompilationByInstitutionId(this.accountService.currentUser.institutionId)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let signageCompilations = results[0];
        let permissions = results[1];

        signageCompilations.forEach((signageCompilation, index, signageCompilations) => {
          (<any>signageCompilation).index = index + 1;
        });


        this.rowsCache = [...signageCompilations];
        this.rows = signageCompilations;

        this.onSearchChanged(this.keyword);
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve signage compilations from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }


  onSearchChanged(value: string) {
    this.keyword = value;
    this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.name, r.description));
  }


  onEditorModalHidden() {
    this.editingSignageCompilationName = null;
    this.signageCompilationEditor.resetForm(true);
  }


  newSignageCompilation() {
    this.editingSignageCompilationName = null;
    this.sourceSignageCompilation = null;
    this.editedSignageCompilation = this.signageCompilationEditor.newSignageCompilation();
    this.header = 'New Signage Compilation';
    //this.openDialog(this.editedSignageCompilation);
    //this.editorModal.show();
    this.showEditor = true;
  }


  editSignageCompilation(row: SignageCompilation) {
    this.editingSignageCompilationName = { name: row.name };
    this.sourceSignageCompilation = row;
    this.editedSignageCompilation = this.signageCompilationEditor.editSignageCompilation(row);
    this.header = 'Edit Signage Compilation';
    //this.editorModal.show();
    //this.openDialog(this.editedSignageCompilation);
    this.showEditor = true;
  }

  deleteSignageCompilation(row: SignageCompilation) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" signage compilation?', DialogType.confirm, () => this.deleteSignageCompilationHelper(row));
  }


  deleteSignageCompilationHelper(row: SignageCompilation) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.signageCompilationService.deleteSignageCompilation(row)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.rowsCache = this.rowsCache.filter(item => item !== row)
        this.rows = this.rows.filter(item => item !== row)
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the signage compilation.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }


  get canCreateSignageCompilations() {
    return this.accountService.userHasPermission(Permission.createCompilationsPermission)
  }

  get canUpdateSignageCompilations() {
    return this.accountService.userHasPermission(Permission.updateCompilationsPermission)
  }

  get canDeleteSignageCompilations() {
    return this.accountService.userHasPermission(Permission.deleteCompilationsPermission)
  }

}
