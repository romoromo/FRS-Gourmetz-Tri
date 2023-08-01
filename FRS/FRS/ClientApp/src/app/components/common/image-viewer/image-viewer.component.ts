import { Component, OnInit, Output, EventEmitter, Input, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { DomSanitizer } from '@angular/platform-browser';
import { ConfigurationService } from 'src/app/services/configuration.service';
import { FileService } from 'src/app/services/file.service';

@Component({
  selector: 'image-viewer',
  templateUrl: './image-viewer.component.html',
  styleUrls: ['./image-viewer.component.css']
})
export class ImageViewerComponent implements OnInit {
  @Input() thumbnailFilePath: string;
  @Input() fullImageFilePath: string;
  @Input() width: string;
  @Input() height: string;
  @Input() lensWidth: string;
  @Input() lensHeight: string;
  @Input() fileType: string;

  constructor(public dialogRef: MatDialogRef<ImageViewerComponent>, @Inject(MAT_DIALOG_DATA) public data: any, private fileService: FileService, private domSanitizer: DomSanitizer,
  private configurationService: ConfigurationService) {
    if (typeof (data) != typeof (undefined)) {
      this.thumbnailFilePath = this.getFileImage(data.thumbnailFilePath);
      this.fullImageFilePath = this.getFileImage(data.fullImageFilePath);
      this.width = data.width;
      this.height = data.height;
      this.lensWidth = data.lensWidth;
      this.lensHeight = data.lensHeight;
      this.fileType = data.fileType;
    }
  }

  ngOnInit() {
  }

  getBaseUrl() {
    return this.configurationService.baseUrl;
  }

  getDomainUrl() {
    return this.configurationService.config.domainUrl;
  }

  getFileImage(path) {
    return this.fileService.getFile(path);
  }

  getDocSrc(path) {
    //path = 'Resources\\Images\\b08a4cc0-8bfc-4ce7-b57b-ac355cc0099dpptexamples.ppt';
    if (path[0] != "/") {
      path = "/" + path;
    }
    console.log(this.getDomainUrl() + path);
    let src = encodeURIComponent(this.getDomainUrl() + path);
    
    let url = 'https://view.officeapps.live.com/op/embed.aspx?src=' + src ;//'https://docs.google.com/viewerng/viewer?url='
    //let safeSrc = this.domSanitizer.bypassSecurityTrustResourceUrl(url);
    return url;
  }

  private cancel() {
    this.dialogRef.close();
  }
}
