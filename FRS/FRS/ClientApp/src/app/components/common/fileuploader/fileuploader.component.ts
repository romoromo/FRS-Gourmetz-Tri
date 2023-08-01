import { Component, OnInit, Output, EventEmitter, Input, ViewChild, ElementRef } from '@angular/core';
import { HttpEventType, HttpClient, HttpEvent } from '@angular/common/http';
import { FileService } from 'src/app/services/file.service';

@Component({
  selector: 'file-upload',
  templateUrl: './fileuploader.component.html',
  styleUrls: ['./fileuploader.component.css']
})
export class FileUploadComponent implements OnInit {
  public progress: number;
  public message: string;
  public filename: string;

  @Input() fileTypes: string;
  @Input() showTxt: boolean;
  @Output() public onUploadFinished = new EventEmitter();
  @Input() resetAfterUpload: boolean;
  @Input() btnText: string;
  @Input() btnClass: string;
  @Input() btnClassContainer: string;
  @Input() fileName: string;
  @Input() useOriginalFilename: string;

  constructor(private http: HttpClient, private fileService: FileService) { }


  @ViewChild('file')
  fileEl: ElementRef;

  ngOnInit() {
  }

  public uploadFile = (files) => {
    if (files.length === 0) {
      return;
    }

    let fileToUpload = <File>files[0];
    const formData = new FormData();
    let name = fileToUpload.name;

    if (this.fileName && this.fileName.trim() != '') {
      let fnames = fileToUpload.name.split('.');
      if (fnames.length > 1) {
        name = this.fileName + '.' + fnames[1];
      }
    }
    formData.append('useOriginalFilename', !this.useOriginalFilename ? 'false' : this.useOriginalFilename);
    formData.append('file', fileToUpload, name);
    

    this.fileService.uploadFile<HttpEvent<Object>>(formData)
      .subscribe(event => {
        if (event.type === HttpEventType.UploadProgress)
          this.progress = Math.round(100 * event.loaded / event.total);
        else if (event.type === HttpEventType.Response) {
          this.message = 'Upload success.';
          //var fileUploadResponse: any = event.body;
          //this.filename = fileUploadResponse.fileName;
          this.onUploadFinished.emit(event.body);
          if (this.resetAfterUpload) {
            this.reset();
          }
        }
      });
  }

  public reset() {
    if (this.fileEl && this.fileEl.nativeElement) {
      this.fileEl.nativeElement.value = "";
    }

  }
}
