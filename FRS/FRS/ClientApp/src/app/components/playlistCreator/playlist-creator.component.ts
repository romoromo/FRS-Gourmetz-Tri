import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, Inject} from '@angular/core';
import { fadeInOut } from '../../services/animations';
import { ImageFileService } from 'src/app/services/image-file.service';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../services/alert.service';
import { AppTranslationService } from "../../services/app-translation.service";
import { AccountService } from '../../services/account.service';
import { Utilities } from '../../services/utilities';
import { Image } from '../../models/image.model';
import { Permission } from '../../models/permission.model';
import { PlaylistService } from "../../services/playlist.service";
import { Playlist, imageIndex } from '../../models/playlist.model';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material';
import { FormControl, Validators } from '@angular/forms';
import { UserSelectorComponent } from '../common/user-selector/user-selector.component';
import { MediaService } from '../../services/media.service';
import { Media } from '../../models/media.model';
import { MediaExtensionService } from '../../services/media-extension.service';
import { MediaExtension } from '../../models/media-extension.model';


@Component({
    selector: 'playlist-creator',
  templateUrl: './playlist-creator.component.html',
  styleUrls: ['./playlist-creator.component.css'],
    animations: [fadeInOut]
})

export class PlaylistCreatorComponent implements OnInit {


  public fileUploadResponse: { dbPath: '', fileId: null, fileName: '' };
  private filePath: string;
  loadingIndicator: boolean;

  private isSaving: boolean;
  private isNewPlaylist: boolean;
  private showValidationErrors: boolean = true;
  private editingPlaylistName: string;
  public playlistEdit: Playlist = new Playlist();

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;

  private selectedImageList: Image[] = [];
  
  imagePage: Image[] = [];
  imageAll: Image[] = [];

  allMedias: Media[];
  allMediaExtensions: MediaExtension[];

  public formResetToggle = true;

  search: string;
  mediaGroup: string = '';
  sort: string = '';
  fileType: string = '';
  ext: string = '';
  asc: boolean;

  animations = ['top', 'left', 'right', 'bottom', 'zoom', 'fadein'];

  data = [];
  page = 0;
  size = 9;
    originalPlaylist: Playlist;
    length: number;

  constructor(private fileService: ImageFileService, private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    public dialog: MatDialog, private playlistService: PlaylistService, private mediaService: MediaService,
    public dialogRef: MatDialogRef<PlaylistCreatorComponent>, private mediaExtensionService: MediaExtensionService,
    @Inject(MAT_DIALOG_DATA) public dt: any) {
    if (typeof (dt.playlist) != typeof (undefined)) {
      if (dt.playlist.id) {
        this.editPlaylist(dt.playlist);
      } else {
        this.newPlaylist();
      }
    }

    this.mediaService.getMedias()
      .subscribe(results => {
        this.allMedias = results;
      },
        error => {
        });

    this.mediaExtensionService.getMediaExtensionByInstitutionId(this.accountService.currentUser.institutionId)
      .subscribe(results => {
        this.allMediaExtensions = results[0];
      },
        error => {

        })
  }

  ngOnInit() {
    this.loadData();
    this.refresh();
  }

  loadData() {
    this.alertService.startLoadingMessage();
    this.loadingIndicator = true;

    this.fileService.getImages(-1, -1, this.accountService.currentUser.institutionId, '', this.accountService.currentUser.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let result_data = results;

        result_data.forEach((image, index, result_data) => {
          (<any>image).index = index + 1;
        });

        this.imageAll = result_data;

        this.refresh();

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve images from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  refresh() {
    this.page = 0;
    this.getData();
  }

  getData(obj?) {
    if (obj && !isNaN(obj.pageIndex) && !isNaN(obj.pageSize)) {
      this.page = obj.pageIndex;
      this.size = obj.pageSize;
    }

    let startingIndex = this.page * this.size;
    let endingIndex = startingIndex + this.size;

    this.imagePage = this.imageAll.slice(0);;

    if (this.search)
      this.imagePage = this.imagePage.filter(i => (i.title + ' ' + i.description + ' ' + i.tags).toLowerCase().includes(this.search.toLowerCase()));

    if (this.mediaGroup)
      this.imagePage = this.imagePage.filter(i => i.mediaId == this.mediaGroup);

    if (this.fileType)
      this.imagePage = this.imagePage.filter(i => this.fileType == '1' ? !i.isVideo : (this.fileType == '2' ? i.isVideo : true));

    if (this.ext) {
      this.imagePage = this.imagePage.filter(i => i.imageLocation.endsWith(this.ext));
    }

    if (this.sort)
      this.imagePage = this.imagePage.sort((a, b) => {
        let result = 0;

        if (this.sort == '1') result = 1;
        else if (this.sort == '2') result = b.title.localeCompare(a.title);
        else if (this.sort == '3') result = b.description.localeCompare(a.description);

        if (this.asc) result *= -1;

        return result;
      });

    this.length = this.imagePage.length;

    this.imagePage = this.imagePage.filter((img, i) => {
      return i >= startingIndex && i < endingIndex;
    });
  }

  public uploadFinished = (event) => {
    this.fileUploadResponse = event;
    this.filePath = this.fileUploadResponse ? this.fileUploadResponse.dbPath : null;
  }

  getFileImage(path) {
    return this.fileService.getFile(path);
  }

  drop(event: CdkDragDrop<{ id: number,title: string, url: string }[]>) {
    moveItemInArray(this.selectedImageList, event.previousIndex, event.currentIndex);
    console.log("urutan: " + this.selectedImageList)
  }

  addImage(image) {
    image.animation = image.animation || '';
    this.selectedImageList.push(image);
  }

  removeImage(image) {
    var index = this.selectedImageList.indexOf(image);
    this.selectedImageList.splice(index, 1);
  }

  loadSelectedImages() {
    let localPE = this.playlistEdit;
    //get selected image from db according to index
    var selImage: Image[] = [];
    localPE.imageIds.sort((a, b) => a.imgIndex - b.imgIndex);
    console.log("the playlistEdit after sort: ", localPE.imageIds)

    this.fileService.getImages(-1, -1, this.accountService.currentUser.institutionId)
      .subscribe((results) => {
        localPE.imageIds.forEach((imgId, index) => {
          results.forEach((image, jndex) => {
            if (+image.id == imgId.imageId) {
              image.duration = imgId.duration;
              image.animation = imgId.animation || '';
              selImage.push(image)
            }
          });
        });
        this.selectedImageList = selImage;
      },
        error => {
          this.alertService.resetStickyMessage();
          this.alertService.showStickyMessage("getImages failed", "An error occured while trying to get images from the server", MessageSeverity.error);
        });

    //this.fileService.getImages(-1, -1, this.accountService.currentUser.institutionId)
    //  .subscribe((results) => {
    //    this.selectedImageList = results.filter(function (image) {
    //      var match = localPE.imageIds.indexOf(+image.id);
    //      //console.log("playlistEdit: ", localPE.imageIds);
    //      //console.log("image id: " + image.id + "indexof: " + match);
    //      return localPE.imageIds.indexOf(+image.id) !== -1;
    //    });
    //  },
    //    error => {
    //      this.alertService.resetStickyMessage();
    //      this.alertService.showStickyMessage("getImages failed", "An error occured while trying to get images from the server", MessageSeverity.error);
    //    });
    
  }

  private save() {
    this.isSaving = true;
    var selectedImageIds: imageIndex[] = [];;
    this.selectedImageList.forEach((image, index, result_data) => {
      selectedImageIds.push(new imageIndex(+image.id, index, image.duration, image.animation));
    });
    console.log("image list saving: ", this.selectedImageList);
    this.playlistEdit.imageIds = selectedImageIds;
    console.log("image list id in object: ", this.playlistEdit.imageIds);
    this.alertService.startLoadingMessage("Saving changes...");
    if (this.isNewPlaylist) {
      this.playlistService.newPlaylist(this.playlistEdit).subscribe(playlist => this.saveSuccessHelper(playlist), error => this.saveFailedHelper(error));
    }
    else {
      this.playlistService.updatePlaylist(this.playlistEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }




  private saveSuccessHelper(playlist?: Playlist) {
    if (playlist)
      Object.assign(this.playlistEdit, playlist);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewPlaylist)
      this.alertService.showMessage("Success", `Playlist \"${this.playlistEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to playlist \"${this.playlistEdit.name}\" was saved successfully`, MessageSeverity.success);


    if (this.changesSavedCallback)
      this.changesSavedCallback();

    this.dialogRef.close();

    //this.playlistEdit = new Playlist();

    //if (!this.isNewContactGroup && this.accountService.currentUser.facilities.some(r => r == this.editingContactGroupName))
    //    this.refreshLoggedInUser();
  }


  private saveFailedHelper(error: any) {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage("Save Error", "The below errors occured while saving your changes:", MessageSeverity.error);
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);

  }

  private cancel() {
    this.playlistEdit = new Playlist();

    this.showValidationErrors = false;

    this.alertService.resetStickyMessage();

    if (this.changesCancelledCallback)
      this.changesCancelledCallback();

    this.dialogRef.close();
  }

  newPlaylist() {
    this.showValidationErrors = true;
    this.isNewPlaylist = true;
    this.playlistEdit = new Playlist();
    this.playlistEdit.institutionId = this.accountService.currentUser.institutionId;
    return this.playlistEdit;
  }

  editPlaylist(playlist: Playlist) {
    if (playlist) {
      this.isNewPlaylist = false;
      this.showValidationErrors = true;
      this.originalPlaylist = playlist;
      this.playlistEdit = new Playlist();
      Object.assign(this.playlistEdit, playlist);

      this.loadSelectedImages();

      return this.playlistEdit;
    }
    else {
      return this.newPlaylist();
    }
  }

}
