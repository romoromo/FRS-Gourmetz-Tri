import { Component, ViewChild, Inject, OnInit} from '@angular/core';
import { fadeInOut } from '../../services/animations';
import { ImageFileService } from 'src/app/services/image-file.service';

import { AlertService, MessageSeverity, DialogType } from '../../services/alert.service';
import { AccountService } from "../../services/account.service";
import { Image } from '../../models/image.model';
import { Permission } from '../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material';
import { ImageUploadComponent } from '../common/ImageUploader/imageuploader.component'
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { AppTranslationService } from "../../services/app-translation.service";
import { Utilities } from '../../services/utilities';
import { PlaylistService } from "../../services/playlist.service";
import { Playlist, imageIndex } from '../../models/playlist.model';
import { FormControl, Validators } from '@angular/forms';
import { UserSelectorComponent } from '../common/user-selector/user-selector.component';
import { ImageUploadsComponent} from "./image-upload.component";
import { MediaService } from '../../services/media.service';
import { Media } from '../../models/media.model';
import { MediaExtensionService } from '../../services/media-extension.service';
import { MediaExtension } from '../../models/media-extension.model';

@Component({
    selector: 'image-lists',
    templateUrl: './image-list.component.html',
    styleUrls: ['./image-list.component.css'],
    animations: [fadeInOut]
})
export class ImageListsComponent implements OnInit  {
  public fileUploadResponse: { dbPath: '', fileId: null, fileName: '' };
  private filePath: string;
  private imageEdit: Image = new Image();
  private isSaving: boolean;
  private showValidationErrors: boolean = true;

  loadingIndicator: boolean;
  private isNewPlaylist: boolean;
  private editingPlaylistName: string;

  private selectedImageList: Image[] = [];

  imagePage: Image[] = [];
  imageAll: Image[] = [];

  allMedias: Media[];
  allMediaExtensions: MediaExtension[];

  search: string;
  mediaGroup: string = '';
  sort: string = '';
  fileType: string = '';
  ext: string = '';
  asc: boolean;

  data = [];
  page = 0;
  size = 24;

  @ViewChild('imgUpload') imgUpload: ImageUploadComponent;
    length: number;

  constructor(private fileService: ImageFileService, private alertService: AlertService, private accountService: AccountService, public dialog: MatDialog, public mediaService: MediaService, private mediaExtensionService: MediaExtensionService) {
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
  }

  getFileImage(path) {
    return this.fileService.getFile(path);
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

    let filtered = false;

    if (this.search) {
      this.imagePage = this.imagePage.filter(i => (i.title + ' ' + i.description + ' ' + i.tags).toLowerCase().includes(this.search.toLowerCase()));
      filtered = true;
    }

    if (this.mediaGroup) {
      this.imagePage = this.imagePage.filter(i => i.mediaId == this.mediaGroup);
      filtered = true;
    }

    if (this.fileType) {
      this.imagePage = this.imagePage.filter(i => this.fileType == '1' ? !i.isVideo : (this.fileType == '2' ? i.isVideo : true));
      filtered = true;
    }

    if (this.ext) {
      this.imagePage = this.imagePage.filter(i => i.imageLocation.endsWith(this.ext));
      filtered = true;
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

    console.log("INDEX", startingIndex, endingIndex);

    if (!filtered) this.imagePage = [];
  }

  addImage(image) {
    this.selectedImageList.push(image);
  }

  delete() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Deleting image...");
    this.selectedImageList.forEach((image, index, result_data) => {
      this.deleteDb(image.id, index == (this.selectedImageList.length -1));
      //this.removeImage(image);
    });
  }

  removeImage(image) {
    var index = this.selectedImageList.indexOf(image);
    this.selectedImageList.splice(index, 1);
  }

  private deleteDb(imageId,last: boolean) {
    console.log("image delete id: ", imageId);
    this.fileService.deleteImage(imageId).subscribe(response => this.saveSuccessHelper(last), error => this.saveFailedHelper(error));
  }

  private deleteDbConfirm(imageId, last: boolean) {
    if (confirm('Are you sure you want to delete?')) {
      this.deleteDb(imageId, last);
    }
  }


  private saveSuccessHelper(last?: boolean) {

    if (last) {

      this.isSaving = false;
      this.alertService.stopLoadingMessage();
      this.showValidationErrors = false;

      this.alertService.showMessage("Success", `Success Delete`, MessageSeverity.success);
      this.selectedImageList = [];
      this.loadData();
    }
  }


  private saveFailedHelper(error: any) {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage("Delete Error", "The below errors occured while deleting image:", MessageSeverity.error);
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);

  }

  newImage() {
    //this.editingDepartmentName = null;
    //this.sourceDepartment = null;
    //this.editedDepartment = this.departmentEditor.newDepartment();
    ////this.editorModal.show();
    //this.header = 'New Department';
    this.openDialog({});
  }

  editImage(image) {
    this.openDialog(image);
  }

  openDialog(data): void {
    const dialogRef = this.dialog.open(ImageUploadsComponent, {
      data: data,
      width: '80vw',
      height: '90vh',
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData();
    });
  }

}
