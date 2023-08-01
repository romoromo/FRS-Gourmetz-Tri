import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Institution } from '../../../models/institution.model';
import { Permission } from '../../../models/permission.model';
import { InstitutionEditorComponent } from "./institution-editor.component";
import { InstitutionService } from 'src/app/services/institution.service';
import { MatDialog } from '@angular/material';
import { Subscription } from 'rxjs';


@Component({
  selector: 'institutions-management',
  templateUrl: './institutions-management.component.html',
  styleUrls: ['./institutions-management.component.css']
})
export class InstitutionsManagementComponent implements OnInit, AfterViewInit, OnDestroy {
  private subscription: Subscription = new Subscription();
  columns: any[] = [];
  rows: Institution[] = [];
  rowsCache: Institution[] = [];
  allPermissions: Permission[] = [];
  editedInstitution: Institution;
  sourceInstitution: Institution;
  editingInstitutionName: { name: string };
  loadingIndicator: boolean;



  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('institutionEditor')
  institutionEditor: InstitutionEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private institutionService: InstitutionService, public dialog: MatDialog) {
  }

  openDialog(institution: Institution): void {
    const dialogRef = this.dialog.open(InstitutionEditorComponent, {
      data: { header: this.header, institution: institution },
      width: '400px'
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData();
    });
  }

  ngOnInit() {

    let gT = (key: string) => this.translationService.getTranslation(key);

    this.columns = [
      { prop: "index", name: '#', width: 50, cellTemplate: this.indexTemplate, canAutoResize: false },
      { prop: 'name', name: gT('institutions.management.Name'), width: 200 },
      { prop: 'description', name: gT('institutions.management.Description'), width: 350 },
      { name: '', width: 150, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
    ];

    this.loadData();
  }





  ngAfterViewInit() {

    this.institutionEditor.changesSavedCallback = () => {
      this.addNewInstitutionToList();
      this.editorModal.hide();
    };

    this.institutionEditor.changesCancelledCallback = () => {
      this.editedInstitution = null;
      this.sourceInstitution = null;
      this.editorModal.hide();
    };
  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  addNewInstitutionToList() {
    if (this.sourceInstitution) {
      Object.assign(this.sourceInstitution, this.editedInstitution);

      let sourceIndex = this.rowsCache.indexOf(this.sourceInstitution, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rowsCache, sourceIndex, 0);

      sourceIndex = this.rows.indexOf(this.sourceInstitution, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rows, sourceIndex, 0);

      this.editedInstitution = null;
      this.sourceInstitution = null;
    }
    else {
      let facility = new Institution();
      Object.assign(facility, this.editedInstitution);
      this.editedInstitution = null;

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

    this.subscription.add(this.institutionService.getInstitutions()
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let institutions = results[0];
        let permissions = results[1];

        institutions.forEach((facility, index, institutions) => {
          (<any>facility).index = index + 1;
        });


        this.rowsCache = [...institutions];
        this.rows = institutions;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve institutions from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }


  onSearchChanged(value: string) {
    this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.name, r.description));
  }


  onEditorModalHidden() {
    this.editingInstitutionName = null;
    this.institutionEditor.resetForm(true);
  }


  newInstitution() {
    this.editingInstitutionName = null;
    this.sourceInstitution = null;
    this.editedInstitution = this.institutionEditor.newInstitution();
    //this.editorModal.show();
    this.header = 'New Institution';
    this.openDialog(this.editedInstitution);
  }


  editInstitution(row: Institution) {
    this.editingInstitutionName = { name: row.name };
    this.sourceInstitution = row;
    this.editedInstitution = this.institutionEditor.editInstitution(row);
    //this.editorModal.show();

    this.header = 'Edit Institution';
    this.openDialog(this.editedInstitution);
  }

  deleteInstitution(row: Institution) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" institution?', DialogType.confirm, () => this.deleteInstitutionHelper(row));
  }


  deleteInstitutionHelper(row: Institution) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.subscription.add(this.institutionService.deleteInstitution(row)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.rowsCache = this.rowsCache.filter(item => item !== row)
        this.rows = this.rows.filter(item => item !== row)
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the institution.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }


  get canManageInstitutions() {
    return this.accountService.userHasPermission(Permission.manageInstitutionsPermission)
  }

}
