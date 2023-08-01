import { Component, ViewChild, Input, ChangeDetectorRef, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../services/alert.service';
import { AccountService } from "../../services/account.service";
import { KioskSettingsService } from '../../services/kiosk-settings.service';
import { KioskSettings } from '../../models/kiosk-settings.model';
import { Permission } from '../../models/permission.model';
import { Playlist } from '../../models/playlist.model';
import { PlaylistService } from 'src/app/services/playlist.service';
import { AuthService } from 'src/app/services/auth.service';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormControl, Validators } from '@angular/forms';
import { FileService } from 'src/app/services/file.service';


@Component({
  selector: 'kiosk-settings-edit',
  templateUrl: './kiosk-settings-edit.component.html',
  styleUrls: ['./kiosk-settings-edit.component.css']
})
export class KioskSettingsEditComponent {

  private isEditMode = false;
  private isNewSettings = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingSettingsLabel: string;
  private settingsEdit: KioskSettings = new KioskSettings();
  private allPlaylists: Playlist[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  fcPlaylist = new FormControl('', [Validators.required]);
  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;

  @ViewChild('playlists')
  private playlists;

  @ViewChild('playlistsSelector')
  private playlistsSelector;

  @Input()
  isViewOnly: boolean;

  public fileUploadResponse: { dbPath: '', fileId: null, fileName: '' };


  fonts = [
    'American Typewriter',
    'Andalé Mono',
    'Arial Black',
    'Arial',
    'Avenir Next',
    'Bradley Hand',
    'Brush Script MT',
    'Comic Sans MS',
    'Courier',
    'Didot',
    'Franklin Gothic',
    'Georgia',
    'Impact',
    'Lucida Console',
    'Luminari',
    'Monaco',
    'Tahoma',
    'Times New Roman',
    'Trebuchet MS',
    'Verdana',
  ];

  constructor(private cdRef: ChangeDetectorRef, private alertService: AlertService, private accountService: AccountService, private kioskSettingsService: KioskSettingsService, private playlistService: PlaylistService,
    private authService: AuthService, private fileService: FileService,
    public dialogRef: MatDialogRef<KioskSettingsEditComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.settings) != typeof (undefined)) {
      this.allPlaylists = data.allPlaylists;
      console.log("playlist Data: ", data.allPlaylists)
      if (data.settings.id) {
        this.editKioskSettings(data.settings, data.allPlaylists);
      } else {
        this.newKioskSettings(data.allPlaylists);
      }
    }
  }



  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    console.log("saving...")
    if (this.form.form.valid) {
      this.isSaving = true;
      this.alertService.startLoadingMessage("Saving changes...");
      //this.settingsEdit.dir_font_type = this.settingsEdit.dirFontType;

      //this.settingsEdit.institutionId = parseInt(this.authService.currentUser.institutionId);
      if (this.isNewSettings) {
        console.log("save new: ", this.settingsEdit)
        this.kioskSettingsService.newKioskSettings(this.settingsEdit).subscribe(response => this.saveSuccessHelper(response), error => this.saveFailedHelper(error));
      }
      else {
        console.log("update setting")
        this.kioskSettingsService.updateKioskSettings(this.settingsEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
      }
    }
  }




  private saveSuccessHelper(settings?: KioskSettings) {
    if (settings)
      Object.assign(this.settingsEdit, settings);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewSettings)
      this.alertService.showMessage("Success", `Facility \"${this.settingsEdit.label}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to facility \"${this.settingsEdit.label}\" was saved successfully`, MessageSeverity.success);


    this.settingsEdit = new KioskSettings();
    this.resetForm();


    if (this.changesSavedCallback)
      this.changesSavedCallback();

    this.dialogRef.close();
  }


  private refreshLoggedInUser() {
    this.accountService.refreshLoggedInUser()
      .subscribe(user => { },
        error => {
          this.alertService.resetStickyMessage();
          this.alertService.showStickyMessage("Refresh failed", "An error occured while refreshing logged in user information from the server", MessageSeverity.error);
        });
  }



  private saveFailedHelper(error: any) {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage("Save Error", "The below errors occured while saving your changes:", MessageSeverity.error);
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);

    if (this.changesFailedCallback)
      this.changesFailedCallback();
  }


  private cancel() {
    this.settingsEdit = new KioskSettings();

    this.showValidationErrors = false;
    this.resetForm();

    this.alertService.resetStickyMessage();

    if (this.changesCancelledCallback)
      this.changesCancelledCallback();

    this.dialogRef.close();
  }

  resetForm(replace = false) {

    if (!replace) {
      this.form.reset();
    }
    else {
      this.formResetToggle = false;

      setTimeout(() => {
        this.formResetToggle = true;
      });
    }
  }


  newKioskSettings(allPlaylists: Playlist[]) {
    console.log("new kiosk Setting")
    this.isEditMode = true;
    this.isNewSettings = true;
    this.showValidationErrors = true;

    this.editingSettingsLabel = null;
    this.allPlaylists = allPlaylists;
    console.log("allplaylist new: ", allPlaylists)
    this.selectedValues = {};
    this.settingsEdit = new KioskSettings();

    return this.settingsEdit;
  }

  editKioskSettings(settings: KioskSettings, allPlaylists: Playlist[]) {
    console.log("update kiosk Settings")
    this.isEditMode = true;
    if (settings) {
      this.isNewSettings = false;
      this.showValidationErrors = true;
      this.allPlaylists = allPlaylists;

      this.editingSettingsLabel = settings.label;
      this.selectedValues = {};
      //this.setFacilityTypes(facility, allFacilityTypes);
      //this.setInstitutions(facility, allInstitutions);
      this.settingsEdit = new KioskSettings();
      Object.assign(this.settingsEdit, settings);

      return this.settingsEdit;
    }
    else {
      return this.newKioskSettings(allPlaylists);
    }
  }

  private setPLaylists(settings: KioskSettings, allPlaylists?: Playlist[]) {

    this.allPlaylists = allPlaylists ? [...allPlaylists] : [];

    if (allPlaylists == null || this.allPlaylists.length != allPlaylists.length)
      setTimeout(() => this.playlistsSelector.refresh());
  }

  changeKioskSettings(id) {
    var x = this.settingsEdit;
    var oldPlaylistId = this.settingsEdit.ssPlaylistId;
    this.settingsEdit.ssPlaylistId = "";
    this.playlistService.getPlaylists(id).subscribe(results => {
      this.allPlaylists = results;
      setTimeout(() => this.playlistsSelector.refresh());
      this.cdRef.detectChanges();
      this.settingsEdit.ssPlaylistId = oldPlaylistId;
    },
      error => {
      });



  }

  setssPlaylistId(id) {
    this.settingsEdit.ssPlaylistId = id;
  }

  get canManageKioskSettings() {
    return true;
  }

  public uploadFinished = (event) => {
    console.log("finish uploading... ", event)
    this.fileUploadResponse = event;
    this.settingsEdit.dir_font_type = this.fileUploadResponse ? this.fileUploadResponse.dbPath : null;
  }

  public uploadFinishedBalloon = (event) => {
    console.log("finish uploading... ", event)
    this.fileUploadResponse = event;
    this.settingsEdit.baloon_image = this.fileUploadResponse ? this.fileUploadResponse.dbPath : null;
  }

  public uploadFinishedArrow = (event) => {
    console.log("finish uploading... ", event)
    this.fileUploadResponse = event;
    this.settingsEdit.arrow_image = this.fileUploadResponse ? this.fileUploadResponse.dbPath : null;
  }

  getFileImage(path) {
    return this.fileService.getFile(path);
  }
}
