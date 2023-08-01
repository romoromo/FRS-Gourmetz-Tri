import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, Inject, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { ApplicationSettingService } from 'src/app/services/application-setting.service';
import { MatDialog } from '@angular/material';
import { Utilities } from 'src/app/services/utilities';
import { ApplicationSetting } from 'src/app/models/application-setting.model';
import { Permission } from 'src/app/models/permission.model';
import { AlertService, MessageSeverity, DialogType } from 'src/app/services/alert.service';
import { AppTranslationService } from 'src/app/services/app-translation.service';
import { AccountService } from 'src/app/services/account.service';
import { ApplicationSettingEditorComponent } from './application-setting-editor.component';
import { Subscription } from 'rxjs';


@Component({
  selector: 'application-settings-management',
  templateUrl: './application-settings-management.component.html',
  styleUrls: ['./application-settings-management.component.css']
})
export class ApplicationSettingsManagementComponent implements OnInit, OnDestroy, AfterViewInit {
  private subscription: Subscription = new Subscription();
  columns: any[] = [];
  rows: ApplicationSetting[] = [];
  rowsCache: ApplicationSetting[] = [];
  allPermissions: Permission[] = [];
  editedApplicationSetting: ApplicationSetting;
  sourceApplicationSetting: ApplicationSetting;
  editingApplicationSettingName: { key: string };
  loadingIndicator: boolean;



  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('applicationSettingEditor')
  applicationSettingEditor: ApplicationSettingEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private applicationSettingService: ApplicationSettingService, public dialog: MatDialog) {
  }

  openDialog(applicationSetting: ApplicationSetting): void {
    const dialogRef = this.dialog.open(ApplicationSettingEditorComponent, {
      data: { header: this.header, applicationSetting: applicationSetting },
      width: '400px',
      disableClose: true
    });

    this.subscription.add(dialogRef.afterClosed().subscribe(result => {
      this.loadData();
    }));
  }

  ngOnInit() {

    let gT = (key: string) => this.translationService.getTranslation(key);

    this.columns = [
      { prop: "index", name: '#', width: 50, cellTemplate: this.indexTemplate, canAutoResize: false },
      { prop: 'key', name: gT('applicationSettings.management.Key') },
      { prop: 'value', name: gT('applicationSettings.management.Value') },
      { prop: 'description', name: gT('applicationSettings.management.Description') },
      { name: '', width: 150, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
    ];

    if (!this.accountService.currentUser.institutionId || this.accountService.currentUser.institutionId == '0') {
      this.columns.splice(1, 0, { prop: 'institutionName', name: gT('roles.management.Institution'), width: 120 });
    }
    this.loadData();
  }

  ngOnDestroy() {
    this.alertService.resetStickyMessage();
    this.subscription.unsubscribe();
  }



  ngAfterViewInit() {

    this.applicationSettingEditor.changesSavedCallback = () => {
      this.addNewApplicationSettingToList();
      //this.editorModal.hide();
    };

    this.applicationSettingEditor.changesCancelledCallback = () => {
      this.editedApplicationSetting = null;
      this.sourceApplicationSetting = null;
      //this.editorModal.hide();
    };
  }


  addNewApplicationSettingToList() {
    if (this.sourceApplicationSetting) {
      Object.assign(this.sourceApplicationSetting, this.editedApplicationSetting);

      let sourceIndex = this.rowsCache.indexOf(this.sourceApplicationSetting, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rowsCache, sourceIndex, 0);

      sourceIndex = this.rows.indexOf(this.sourceApplicationSetting, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rows, sourceIndex, 0);

      this.editedApplicationSetting = null;
      this.sourceApplicationSetting = null;
    }
    else {
      let applicationSetting = new ApplicationSetting();
      Object.assign(applicationSetting, this.editedApplicationSetting);
      this.editedApplicationSetting = null;

      let maxIndex = 0;
      for (let r of this.rowsCache) {
        if ((<any>r).index > maxIndex)
          maxIndex = (<any>r).index;
      }

      (<any>applicationSetting).index = maxIndex + 1;

      this.rowsCache.splice(0, 0, applicationSetting);
      this.rows.splice(0, 0, applicationSetting);
      this.rows = [...this.rows];
    }
  }




  loadData() {
    //this.alertService.startLoadingMessage();
    this.loadingIndicator = true;

    this.subscription.add(this.applicationSettingService.getApplicationSettingByInstitutionId(this.accountService.currentUser.institutionId)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let applicationSettings = results[0];
        let permissions = results[1];

        applicationSettings.forEach((applicationSetting, index, applicationSettings) => {
          (<any>applicationSetting).index = index + 1;
        });


        this.rowsCache = [...applicationSettings];
        this.rows = applicationSettings;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve application setting from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }


  onSearchChanged(value: string) {
    this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.key, r.value));
  }


  onEditorModalHidden() {
    this.editingApplicationSettingName = null;
    this.applicationSettingEditor.resetForm(true);
  }


  newApplicationSetting() {
    this.editingApplicationSettingName = null;
    this.sourceApplicationSetting = null;
    this.editedApplicationSetting = this.applicationSettingEditor.newApplicationSetting();
    this.header = 'New Application Setting';
    this.openDialog(this.editedApplicationSetting);
    //this.editorModal.show();
  }


  editApplicationSetting(row: ApplicationSetting) {
    this.editingApplicationSettingName = { key: row.key };
    this.sourceApplicationSetting = row;
    this.editedApplicationSetting = this.applicationSettingEditor.editApplicationSetting(row);
    this.header = 'Edit Application Setting';
    //this.editorModal.show();
    this.openDialog(this.editedApplicationSetting);
  }

  deleteApplicationSetting(row: ApplicationSetting) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.key + '\" application setting?', DialogType.confirm, () => this.deleteApplicationSettingHelper(row));
  }


  deleteApplicationSettingHelper(row: ApplicationSetting) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.subscription.add(this.applicationSettingService.deleteApplicationSetting(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.rowsCache = this.rowsCache.filter(item => item !== row)
        this.rows = this.rows.filter(item => item !== row)
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the application setting.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }


  get canManageApplicationSettings() {
    return true;//this.accountService.userHasPermission(Permission.manageApplicationSettingPermission);
  }

}
