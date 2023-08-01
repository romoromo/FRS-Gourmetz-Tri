import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { HttpEventType, HttpClient, HttpEvent } from '@angular/common/http';
import { ImageFileService } from 'src/app/services/image-file.service';
import { ThemePalette } from '@angular/material/core';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { AlertService, MessageSeverity } from '../../../services/alert.service';

@Component({
  selector: 'image-upload',
  templateUrl: './imageuploader.component.html',
  styleUrls: ['./imageuploader.component.css']
})
export class ImageUploadComponent implements OnInit {
  public progress: number;
  public message: string;
  public filename: string;

  @Input() showTxt: boolean;
  @Output() public onUploadFinished = new EventEmitter();

  constructor(private http: HttpClient, private fileService: ImageFileService, private alertService: AlertService) { }

  ngOnInit() {
  }

  public uploadFile = (files) => {
    if (files.length === 0) {
      return;
    }

    let fileToUpload = <File>files[0];
    const formData = new FormData();
    formData.append('file', fileToUpload, fileToUpload.name);

    this.fileService.uploadFile<HttpEvent<Object>>(formData)
      .subscribe(event => {
        if (event.type === HttpEventType.UploadProgress)
          this.progress = Math.round(100 * event.loaded / event.total);
        else if (event.type === HttpEventType.Response) {
          this.message = 'Upload success.';
          //var fileUploadResponse: any = event.body;
          //this.filename = fileUploadResponse.fileName;
          this.onUploadFinished.emit(event.body);
        }
      }, error => {
          if (error && error.error && error.error[""]) {
            this.message = "Upload Failed";
            for (let str of error.error[""]) {
              this.alertService.showStickyMessage("Upload Failed", str, MessageSeverity.error);
            }
          }
      });
  }

  public reset() {
    this.progress = 0;
  }
}
