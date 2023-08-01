import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, Inject } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { SignagePublication } from '../../../models/signage-publication.model';
import { Permission } from '../../../models/permission.model';
import { SignagePublicationEditorComponent } from "./signage-publication-editor.component";
import { SignagePublicationService } from 'src/app/services/signage-publication.service';
import { MatDialog } from '@angular/material';


@Component({
  selector: 'signage-publications-management',
  templateUrl: './signage-publications-management.component.html',
  styleUrls: ['./signage-publications-management.component.css']
})
export class SignagePublicationsManagementComponent implements OnInit, AfterViewInit {
  columns: any[] = [];
  rows: SignagePublication[] = [];
  rowsCache: SignagePublication[] = [];
  allPermissions: Permission[] = [];
  editedSignagePublication: SignagePublication;
  sourceSignagePublication: SignagePublication;
  editingSignagePublicationName: { name: string };
  loadingIndicator: boolean;

  showEditor: boolean;

  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('signagePublicationEditor')
  signagePublicationEditor: SignagePublicationEditorComponent;
  header: string;
    keyword: string;
  pending: boolean;
  status: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private signagePublicationService: SignagePublicationService) {
  }

  //openDialog(signagePublication: SignagePublication): void {
  //  const dialogRef = this.dialog.open(SignagePublicationEditorComponent, {
  //    data: { header: this.header, signagePublication: signagePublication },
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
      { prop: 'name', name: gT('signagePublications.management.Name') },
      { prop: 'description', name: gT('signagePublications.management.Description') },
      { prop: 'createdByName', name: gT('signagePublications.management.CreatedBy') },
      { prop: 'createdTime', name: "Created Date" },
      { prop: 'approvedByUser', name: "Approved By" },
      { prop: 'approvedTime', name: "Approved Date" },
      { prop: 'rejectedByUser', name: "Rejected By" },
      { prop: 'rejectedTime', name: "Rejected Date" },
      { prop: 'status', name: 'Status' },
      { prop: 'remarkspan', name: "Remark" },
      //{ prop: 'linkPreview', name: '', width: 50 },
      { name: '', width: 150, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
    ];

    if (!this.accountService.currentUser.institutionId || this.accountService.currentUser.institutionId == '0') {
      this.columns.splice(1, 0, { prop: 'institutionName', name: gT('roles.management.Institution'), width: 120 });
    }
    this.loadData();
  }





  ngAfterViewInit() {

    this.signagePublicationEditor.changesSavedCallback = () => {
      //this.addNewSignagePublicationToList();
      //this.editorModal.hide();
      this.showEditor = false;
      this.loadData();
    };

    this.signagePublicationEditor.changesCancelledCallback = () => {
      this.editedSignagePublication = null;
      this.sourceSignagePublication = null;
      //this.editorModal.hide();
      this.showEditor = false;
    };
  }


  addNewSignagePublicationToList() {
    if (this.sourceSignagePublication) {
      Object.assign(this.sourceSignagePublication, this.editedSignagePublication);

      let sourceIndex = this.rowsCache.indexOf(this.sourceSignagePublication, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rowsCache, sourceIndex, 0);

      sourceIndex = this.rows.indexOf(this.sourceSignagePublication, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rows, sourceIndex, 0);

      this.editedSignagePublication = null;
      this.sourceSignagePublication = null;
    }
    else {
      let signagePublication = new SignagePublication();
      Object.assign(signagePublication, this.editedSignagePublication);
      this.editedSignagePublication = null;

      let maxIndex = 0;
      for (let r of this.rowsCache) {
        if ((<any>r).index > maxIndex)
          maxIndex = (<any>r).index;
      }

      (<any>signagePublication).index = maxIndex + 1;

      this.rowsCache.splice(0, 0, signagePublication);
      this.rows.splice(0, 0, signagePublication);
      this.rows = [...this.rows];
    }
  }




  loadData() {
    this.alertService.startLoadingMessage();
    this.loadingIndicator = true;

    this.signagePublicationService.getSignagePublicationByInstitutionId(this.accountService.currentUser.institutionId)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let signagePublications = results[0];
        let permissions = results[1];

        signagePublications.forEach((signagePublication, index, signagePublications) => {
          (<any>signagePublication).index = index + 1;
          //(<any>signagePublication).linkPreview = `<a href="/signagedisplay/serverpreview/${signagePublication.id}" target="_blank" style="white-space: nowrap;">Preview</a>`;
          (<any>signagePublication).linkPreview = `/signagedisplay/serverpreview/${signagePublication.id}`;

          (<any>signagePublication).remarkspan = `<span title="${signagePublication.remark}">${signagePublication.remark ? signagePublication.remark.substring(0, 10) + '...' : ''}</span>`;

          (<any>signagePublication).createdTime = new Date((<any>signagePublication).createdDate).toLocaleString('sv-SE');
          if (signagePublication.approvedDate && signagePublication.approvedDate != "0001-01-01T00:00:00" && signagePublication.approved) (<any>signagePublication).approvedTime = new Date((<any>signagePublication).approvedDate).toLocaleString('sv-SE');

          if (signagePublication.rejectedDate && signagePublication.rejectedDate != "0001-01-01T00:00:00" && signagePublication.rejected) (<any>signagePublication).rejectedTime = new Date((<any>signagePublication).rejectedDate).toLocaleString('sv-SE');

          (<any>signagePublication).exporthistory = `/api/signagepublication/exportapprovedhistory?publicationId=${signagePublication.id}`;

          (<any>signagePublication).status = signagePublication.approved ? 'Approved' : (signagePublication.rejected ? 'Rejected' : 'Pending');

          if (signagePublication.approved) signagePublication.approvedByUser = signagePublication.approvedByName;

          if (signagePublication.rejected) signagePublication.rejectedByUser = signagePublication.rejectedByName;

          signagePublication.schedules.forEach((schedule) => {
            try {
              schedule.startTime = schedule.startTime.split("T")[1];
              schedule.endTime = schedule.endTime.split("T")[1];
            } catch (ex) { }
          });
        });


        this.rowsCache = [...signagePublications];
        this.rows = signagePublications;

        this.onSearchChanged(this.keyword);
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve signage publications from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }


  onSearchChanged(value: string, status?: string) {
    this.keyword = value;
    this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.name, r.description));

    if (status == 'rejected') this.rows = this.rows.filter(r => r.rejected);
    if (status == 'approved') this.rows = this.rows.filter(r => r.approved);
    if (status == 'pending') this.rows = this.rows.filter(r => !r.approved && !r.rejected);
  }


  onEditorModalHidden() {
    this.editingSignagePublicationName = null;
    this.signagePublicationEditor.resetForm(true);
  }


  newSignagePublication() {
    this.editingSignagePublicationName = null;
    this.sourceSignagePublication = null;
    this.editedSignagePublication = this.signagePublicationEditor.newSignagePublication();
    this.header = 'New Signage Publication';
    //this.openDialog(this.editedSignagePublication);
    //this.editorModal.show();
    this.showEditor = true;
  }


  editSignagePublication(row: SignagePublication) {
    this.editingSignagePublicationName = { name: row.name };
    this.sourceSignagePublication = row;
    this.editedSignagePublication = this.signagePublicationEditor.editSignagePublication(row);
    this.header = 'Edit Signage Publication';
    //this.editorModal.show();
    //this.openDialog(this.editedSignagePublication);
    this.showEditor = true;
  }

  deleteSignagePublication(row: SignagePublication) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" signage publication?', DialogType.confirm, () => this.deleteSignagePublicationHelper(row));
  }


  deleteSignagePublicationHelper(row: SignagePublication) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.signagePublicationService.deleteSignagePublication(row)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.rowsCache = this.rowsCache.filter(item => item !== row)
        this.rows = this.rows.filter(item => item !== row)
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the signage publication.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }


  get canCreateSignagePublications() {
    return this.accountService.userHasPermission(Permission.createPublicationsPermission)
  }

  get canUpdateSignagePublications() {
    return this.accountService.userHasPermission(Permission.updatePublicationsPermission)
  }

  get canDeleteSignagePublications() {
    return this.accountService.userHasPermission(Permission.deletePublicationsPermission)
  }

}
