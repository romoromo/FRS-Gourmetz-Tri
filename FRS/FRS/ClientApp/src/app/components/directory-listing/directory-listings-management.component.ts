import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, Inject, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { DirectoryListingService } from 'src/app/services/directory-listing.service';
import { MatDialog } from '@angular/material';
import { Utilities } from 'src/app/services/utilities';
import { DirectoryListing } from 'src/app/models/directory-listing.model';
import { Permission } from 'src/app/models/permission.model';
import { AlertService, MessageSeverity, DialogType } from 'src/app/services/alert.service';
import { AppTranslationService } from 'src/app/services/app-translation.service';
import { AccountService } from 'src/app/services/account.service';
import { DirectoryListingEditorComponent } from './directory-listing-editor.component';
import { Subscription } from 'rxjs';
import { HttpEventType, HttpClient, HttpEvent } from '@angular/common/http';
import { Router } from '@angular/router';


@Component({
  selector: 'directory-listings-management',
  templateUrl: './directory-listings-management.component.html',
  styleUrls: ['./directory-listings-management.component.css']
})
export class DirectoryListingsManagementComponent implements OnInit, OnDestroy, AfterViewInit {
  private subscription: Subscription = new Subscription();
  columns: any[] = [];
  rows: DirectoryListing[] = [];
  rowsCache: DirectoryListing[] = [];
  allPermissions: Permission[] = [];
  editedDirectoryListing: DirectoryListing;
  sourceDirectoryListing: DirectoryListing;
  editingDirectoryListingName: { key: string };
  loadingIndicator: boolean;

  public progress: number;
  public message: string;
  public filename: string;
  fileTypes: string;
  showTxt: boolean;

  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('directoryListingEditor')
  directoryListingEditor: DirectoryListingEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService, private router: Router,
    private directoryListingService: DirectoryListingService, public dialog: MatDialog) {
  }

  public importedFile = (files) => {
    if (files.length === 0) {
      return;
    }

    if (!confirm(`Are you sure to import file '${files[0].name}'? \nImport cannot be undone after the file uploaded.`)) return;

    let fileToUpload = <File>files[0];
    const formData = new FormData();
    formData.append('file', fileToUpload, fileToUpload.name);

    this.loadingIndicator = true;
    this.alertService.startLoadingMessage("Uploading...");
    this.directoryListingService.importFile<HttpEvent<Object>>(formData)
      .subscribe(event => {
        if (event.type === HttpEventType.UploadProgress)
          this.progress = Math.round(100 * event.loaded / event.total);
        else if (event.type === HttpEventType.Response) {
          this.message = 'Upload success.';
          //var fileUploadResponse: any = event.body;
          //this.filename = fileUploadResponse.fileName;
          this.onUploadFinished();
        }

        this.loadingIndicator = false;
      });
  }

  onUploadFinished() {
    //const currentRoute = this.router.url;

    //this.router.navigateByUrl('/', { skipLocationChange: true }).then(() => {
      //this.router.navigate([currentRoute]); // navigate to same route
    //}); 
    this.loadData();
  }

  openDialog(directoryListing: DirectoryListing): void {
    const dialogRef = this.dialog.open(DirectoryListingEditorComponent, {
      data: { header: this.header, directoryListing: directoryListing },
      width: '80vw',
      disableClose: true
    });

    this.subscription.add(dialogRef.afterClosed().subscribe(result => {
      //this.loadData();
    }));
  }

  ngOnInit() {

    let gT = (key: string) => this.translationService.getTranslation(key);

    this.columns = [
      { prop: "index", name: '#', width: 50, cellTemplate: this.indexTemplate, canAutoResize: false },
      { prop: "id", name: 'id', width: 50, canAutoResize: false },
      { prop: 'code', name: gT('directoryListings.management.Code') },
      { prop: 'label', name: gT('directoryListings.management.Label') },
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

    this.directoryListingEditor.changesSavedCallback = () => {
      //this.addNewDirectoryListingToList();
      //this.editorModal.hide();
      this.loadData();
    };

    this.directoryListingEditor.changesCancelledCallback = () => {
      this.editedDirectoryListing = null;
      this.sourceDirectoryListing = null;
      //this.editorModal.hide();
    };
  }


  addNewDirectoryListingToList() {
    if (this.sourceDirectoryListing) {
      Object.assign(this.sourceDirectoryListing, this.editedDirectoryListing);

      let sourceIndex = this.rowsCache.indexOf(this.sourceDirectoryListing, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rowsCache, sourceIndex, 0);

      sourceIndex = this.rows.indexOf(this.sourceDirectoryListing, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rows, sourceIndex, 0);

      this.editedDirectoryListing = null;
      this.sourceDirectoryListing = null;
    }
    else {
      let directoryListing = new DirectoryListing();
      Object.assign(directoryListing, this.editedDirectoryListing);
      this.editedDirectoryListing = null;

      let maxIndex = 0;
      for (let r of this.rowsCache) {
        if ((<any>r).index > maxIndex)
          maxIndex = (<any>r).index;
      }

      (<any>directoryListing).index = maxIndex + 1;

      this.rowsCache.splice(0, 0, directoryListing);
      this.rows.splice(0, 0, directoryListing);
      this.rows = [...this.rows];
    }
  }




  loadData() {
    //this.alertService.startLoadingMessage();
    this.loadingIndicator = true;

    this.subscription.add(this.directoryListingService.getDirectoryListingByInstitutionId(this.accountService.currentUser.institutionId)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let directoryListings = results[0];
        let permissions = results[1];

        directoryListings.forEach((directoryListing, index, directoryListings) => {
          (<any>directoryListing).index = index + 1;
        });


        this.rowsCache = [...directoryListings];
        this.rows = directoryListings;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve data from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }


  onSearchChanged(value: string) {
    this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.code, r.label));
  }


  onEditorModalHidden() {
    this.editingDirectoryListingName = null;
    this.directoryListingEditor.resetForm(true);
  }


  newDirectoryListing() {
    this.editingDirectoryListingName = null;
    this.sourceDirectoryListing = null;
    this.editedDirectoryListing = this.directoryListingEditor.newDirectoryListing();
    this.header = 'New';
    this.openDialog(this.editedDirectoryListing);
    //this.editorModal.show();
  }


  editDirectoryListing(row: DirectoryListing) {
    this.editingDirectoryListingName = { key: row.code };
    this.sourceDirectoryListing = row;
    this.editedDirectoryListing = this.directoryListingEditor.editDirectoryListing(row);
    this.header = 'Edit';
    //this.editorModal.show();
    this.openDialog(this.editedDirectoryListing);
  }

  deleteDirectoryListing(row: DirectoryListing) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.code + '\"?', DialogType.confirm, () => this.deleteDirectoryListingHelper(row));
  }


  deleteDirectoryListingHelper(row: DirectoryListing) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.subscription.add(this.directoryListingService.deleteDirectoryListing(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.rowsCache = this.rowsCache.filter(item => item !== row)
        this.rows = this.rows.filter(item => item !== row)
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the data.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }


  get canManageDirectoryListings() {
    return true;//this.accountService.userHasPermission(Permission.manageDirectoryListingPermission);
  }

}
