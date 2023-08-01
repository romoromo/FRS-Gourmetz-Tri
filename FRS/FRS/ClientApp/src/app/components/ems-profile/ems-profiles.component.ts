import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../services/alert.service';
import { AppTranslationService } from "../../services/app-translation.service";
import { AccountService } from '../../services/account.service';
import { EmsProfileService } from '../../services/ems-profile.service';
import { Utilities } from '../../services/utilities';
import { EmsProfile } from '../../models/ems-profile.model';
import { Permission } from '../../models/permission.model';
import { EmsProfileEditorComponent } from "./ems-profile-editor.component";

import { fadeInOut } from '../../services/animations';
import { DateOnlyPipe } from '../../pipes/datetime.pipe';


@Component({
  selector: 'ems-profiles',
  templateUrl: './ems-profiles.component.html',
  styleUrls: ['./ems-profiles.component.css'],
  animations: [fadeInOut]
})
export class EmsProfilesComponent implements OnInit, AfterViewInit {
  columns: any[] = [];
  rows: EmsProfile[] = [];
  rowsCache: EmsProfile[] = [];
  editedEmsProfile: EmsProfile;
  sourceEmsProfile: EmsProfile;
  editingEmsProfileName: { shortDescription: string };
  loadingIndicator: boolean;
  @Input()
  viewDate: Date;


  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('emsProfileEditor')
  emsProfileEditor: EmsProfileEditorComponent;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private emsProfileService: EmsProfileService) {
  }


  ngOnInit() {

    let gT = (key: string) => this.translationService.getTranslation(key);

    this.columns = [
      { prop: "index", name: '#', width: 50, cellTemplate: this.indexTemplate, canAutoResize: false },
      { prop: 'code', name: gT('emsProfiles.management.Code') },
      { prop: 'label', name: gT('emsProfiles.management.Label') },
      { name: '', width: 100, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: true, sortable: false, draggable: false  }

    ];

    this.loadData();
  }





  ngAfterViewInit() {

    this.emsProfileEditor.changesSavedCallback = () => {
      this.addNewEmsProfileToList();
      this.editorModal.hide();
    };

    this.emsProfileEditor.changesCancelledCallback = () => {
      this.editedEmsProfile = null;
      this.sourceEmsProfile = null;
      this.editorModal.hide();
    };
  }


  addNewEmsProfileToList() {
    if (this.sourceEmsProfile) {
      Object.assign(this.sourceEmsProfile, this.editedEmsProfile);

      let sourceIndex = this.rowsCache.indexOf(this.sourceEmsProfile, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rowsCache, sourceIndex, 0);

      sourceIndex = this.rows.indexOf(this.sourceEmsProfile, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rows, sourceIndex, 0);

      this.editedEmsProfile = null;
      this.sourceEmsProfile = null;
    }
    else {
      let emsProfile = new EmsProfile();
      Object.assign(emsProfile, this.editedEmsProfile);
      this.editedEmsProfile = null;

      let maxIndex = 0;
      for (let r of this.rowsCache) {
        if ((<any>r).index > maxIndex)
          maxIndex = (<any>r).index;
      }

      (<any>emsProfile).index = maxIndex + 1;

      this.rowsCache.splice(0, 0, emsProfile);
      this.rows.splice(0, 0, emsProfile);
      this.rows = [...this.rows];
    }
  }




  loadData() {
    this.alertService.startLoadingMessage();
    this.loadingIndicator = true;

    this.emsProfileService.getEmsProfiles(null, null, this.accountService.currentUser.institutionId)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let emsProfiles = results[0];
        let locations = results[2];

        emsProfiles.forEach((emsProfile, index, emsProfiles) => {
          (<any>emsProfile).index = index + 1;
        });


        this.rowsCache = [...emsProfiles];
        this.rows = emsProfiles;
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve ems profiles from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }


  onSearchChanged(value: string) {
    this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.code, r.label));
  }


  onEditorModalHidden() {
    this.editingEmsProfileName = null;
    this.emsProfileEditor.resetForm(true);
  }


  newEmsProfile() {
    this.editingEmsProfileName = null;
    this.sourceEmsProfile = null;
    this.editedEmsProfile = this.emsProfileEditor.newEmsProfile();
    this.editorModal.show();
  }


  editEmsProfile(row: EmsProfile) {
    this.editingEmsProfileName = { shortDescription: row.code };
    this.sourceEmsProfile = row;
    this.editedEmsProfile = this.emsProfileEditor.editEmsProfile(row);
    this.editorModal.show();
  }

  deleteEmsProfile(row: EmsProfile) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.code + '\" ems profile?', DialogType.confirm, () => this.deleteEmsProfileHelper(row));
  }


  deleteEmsProfileHelper(row: EmsProfile) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.emsProfileService.deleteEmsProfile(row)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.rowsCache = this.rowsCache.filter(item => item !== row)
        this.rows = this.rows.filter(item => item !== row)
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the ems profile.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }


  get canManageEmsProfiles() {
    return true;// || this.accountService.userHasPermission(Permission.manageEmsProfilesPermission)
  }

}
