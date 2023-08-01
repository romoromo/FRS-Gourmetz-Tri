import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Ems } from '../../../models/ems.model';
import { Permission } from '../../../models/permission.model';
import { EmsEditorComponent } from "./ems-editor.component";
import { EmsService } from 'src/app/services/ems.service';
import { MatDialog } from '@angular/material';


@Component({
  selector: 'emses-management',
  templateUrl: './emses-management.component.html',
  styleUrls: ['./emses-management.component.css']
})
export class EmsesManagementComponent implements OnInit, AfterViewInit {
  columns: any[] = [];
  rows: Ems[] = [];
  rowsCache: Ems[] = [];
  allPermissions: Permission[] = [];
  editedEms: Ems;
  sourceEms: Ems;
  editingEmsName: { name: string };
  loadingIndicator: boolean;



  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('emsEditor')
  emsEditor: EmsEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private emsService: EmsService, public dialog: MatDialog) {
  }

  openDialog(ems: Ems): void {
    const dialogRef = this.dialog.open(EmsEditorComponent, {
      data: { header: this.header, ems: ems },
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
      { prop: 'name', name: gT('emses.management.Name') },
      { prop: 'description', name: gT('emses.management.Description') },
      { name: '', width: 150, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
    ];

    this.loadData();
  }





  ngAfterViewInit() {

    this.emsEditor.changesSavedCallback = () => {
      this.addNewEmsToList();
      this.editorModal.hide();
    };

    this.emsEditor.changesCancelledCallback = () => {
      this.editedEms = null;
      this.sourceEms = null;
      this.editorModal.hide();
    };
  }


  addNewEmsToList() {
    if (this.sourceEms) {
      Object.assign(this.sourceEms, this.editedEms);

      let sourceIndex = this.rowsCache.indexOf(this.sourceEms, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rowsCache, sourceIndex, 0);

      sourceIndex = this.rows.indexOf(this.sourceEms, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rows, sourceIndex, 0);

      this.editedEms = null;
      this.sourceEms = null;
    }
    else {
      let facility = new Ems();
      Object.assign(facility, this.editedEms);
      this.editedEms = null;

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

    this.emsService.getEmses()
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let emses = results;

        emses.forEach((facility, index, emses) => {
          (<any>facility).index = index + 1;
        });


        this.rowsCache = [...emses];
        this.rows = emses;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve emses from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }


  onSearchChanged(value: string) {
    this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.name, r.description));
  }


  onEditorModalHidden() {
    this.editingEmsName = null;
    this.emsEditor.resetForm(true);
  }


  newEms() {
    this.editingEmsName = null;
    this.sourceEms = null;
    this.editedEms = this.emsEditor.newEms();
    //this.editorModal.show();
    this.header = 'New Ems';
    this.openDialog(this.editedEms);
  }


  editEms(row: Ems) {
    this.editingEmsName = { name: row.name };
    this.sourceEms = row;
    this.editedEms = this.emsEditor.editEms(row);
    //this.editorModal.show();

    this.header = 'Edit Ems';
    this.openDialog(this.editedEms);
  }

  deleteEms(row: Ems) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" ems?', DialogType.confirm, () => this.deleteEmsHelper(row));
  }


  deleteEmsHelper(row: Ems) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.emsService.deleteEms(row)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.rowsCache = this.rowsCache.filter(item => item !== row)
        this.rows = this.rows.filter(item => item !== row)
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the ems.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }


  get canManageEmses() {
    return true; //this.accountService.userHasPermission(Permission.manageEmsesPermission)
  }

}
