import { Component, ViewChild, Inject } from '@angular/core';
import { fadeInOut } from '../../services/animations';
import { ImageFileService } from 'src/app/services/image-file.service';

import { AlertService, MessageSeverity } from '../../services/alert.service';
import { AccountService } from "../../services/account.service";
import { Image } from '../../models/image.model';
import { Permission } from '../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { ImageUploadComponent } from '../common/ImageUploader/imageuploader.component'
import { MediaService } from '../../services/media.service';
import { Media } from '../../models/media.model';

@Component({
  selector: 'image-uploads',
  templateUrl: './image-upload.component.html',
  styleUrls: ['./image-upload.component.css'],
  animations: [fadeInOut]
})
export class ImageUploadsComponent {


  public fileUploadResponse: { dbPath: '', fileId: null, fileName: '' };
  private filePath: string;
  public imageEdit: Image = new Image();
  private isSaving: boolean;
  private showValidationErrors: boolean = true;

  @ViewChild('imgUpload') imgUpload: ImageUploadComponent;
    allMedias: Media[];

  constructor(private fileService: ImageFileService, private alertService: AlertService, private accountService: AccountService, private mediaService: MediaService,
    @Inject(MAT_DIALOG_DATA) public data: any, public dialogRef: MatDialogRef<ImageUploadsComponent>) {
    if (data) this.imageEdit = data;

    this.mediaService.getMedias()
      .subscribe(results => {
        this.allMedias = results;
      },
        error => {
        });
  }

  public uploadFinished = (event) => {
    console.log("EVENT", event);
    this.fileUploadResponse = event;
    this.filePath = this.fileUploadResponse ? this.fileUploadResponse.dbPath : null;
    this.imageEdit.imageLocation = this.filePath;
  }

  getFileImage(path) {
    return this.fileService.getFile(path);
  }

  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    this.imageEdit.institutionId = this.accountService.currentUser.institutionId;
    if (this.imageEdit.id) this.fileService.updateImage(this.imageEdit).subscribe(image => this.saveSuccessHelper(image), error => this.saveFailedHelper(error));
    else this.fileService.newImage(this.imageEdit).subscribe(image => this.saveSuccessHelper(image), error => this.saveFailedHelper(error));
  }

  private saveSuccessHelper(image?: Image) {
    if (image)
      Object.assign(this.imageEdit, image);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    this.alertService.showMessage("Success", `Media \"${this.imageEdit.title}\" was created successfully`, MessageSeverity.success);


    this.imageEdit = new Image();
    this.filePath = '';
    if (this.imgUpload) this.imgUpload.reset();

    this.dialogRef.close();

    //if (!this.isNewDepartment && this.accountService.currentUser.facilities.some(r => r == this.editingDepartmentName))
    //    this.refreshLoggedInUser();
  }

  private cancel() {
    this.imageEdit = new Image();
    this.filePath = '';
    if (this.imgUpload) this.imgUpload.reset();

    this.dialogRef.close();
  }

  private saveFailedHelper(error: any) {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage("Save Error", "The below errors occured while saving your media:", MessageSeverity.error);
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);
  }

}
