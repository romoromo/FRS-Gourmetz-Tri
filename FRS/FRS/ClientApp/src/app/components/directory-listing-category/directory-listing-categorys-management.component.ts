import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, Inject, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { DirectoryListingCategoryService } from 'src/app/services/directory-listing-category.service';
import { MatDialog } from '@angular/material';
import { Utilities } from 'src/app/services/utilities';
import { DirectoryListingCategory } from 'src/app/models/directory-listing-category.model';
import { Permission } from 'src/app/models/permission.model';
import { AlertService, MessageSeverity, DialogType } from 'src/app/services/alert.service';
import { AppTranslationService } from 'src/app/services/app-translation.service';
import { AccountService } from 'src/app/services/account.service';
import { DirectoryListingCategoryEditorComponent } from './directory-listing-category-editor.component';
import { Subscription } from 'rxjs';


@Component({
  selector: 'directory-listing-categorys-management',
  templateUrl: './directory-listing-categorys-management.component.html',
  styleUrls: ['./directory-listing-categorys-management.component.css']
})
export class DirectoryListingCategorysManagementComponent implements OnInit, OnDestroy, AfterViewInit {
  private subscription: Subscription = new Subscription();
  columns: any[] = [];
  rows: DirectoryListingCategory[] = [];
  rowsCache: DirectoryListingCategory[] = [];
  allPermissions: Permission[] = [];
  editedDirectoryListingCategory: DirectoryListingCategory;
  sourceDirectoryListingCategory: DirectoryListingCategory;
  editingDirectoryListingCategoryName: { key: string };
  loadingIndicator: boolean;



  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('directoryListingCategoryEditor')
  directoryListingCategoryEditor: DirectoryListingCategoryEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private directoryListingCategoryService: DirectoryListingCategoryService, public dialog: MatDialog) {
  }

  openDialog(directoryListingCategory: DirectoryListingCategory): void {
    const dialogRef = this.dialog.open(DirectoryListingCategoryEditorComponent, {
      data: { header: this.header, directoryListingCategory: directoryListingCategory },
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
      { prop: 'code', name: gT('directoryListingCategorys.management.Code') },
      { prop: 'label', name: gT('directoryListingCategorys.management.Label') },
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

    this.directoryListingCategoryEditor.changesSavedCallback = () => {
      this.addNewDirectoryListingCategoryToList();
      //this.editorModal.hide();
    };

    this.directoryListingCategoryEditor.changesCancelledCallback = () => {
      this.editedDirectoryListingCategory = null;
      this.sourceDirectoryListingCategory = null;
      //this.editorModal.hide();
    };
  }


  addNewDirectoryListingCategoryToList() {
    if (this.sourceDirectoryListingCategory) {
      Object.assign(this.sourceDirectoryListingCategory, this.editedDirectoryListingCategory);

      let sourceIndex = this.rowsCache.indexOf(this.sourceDirectoryListingCategory, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rowsCache, sourceIndex, 0);

      sourceIndex = this.rows.indexOf(this.sourceDirectoryListingCategory, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rows, sourceIndex, 0);

      this.editedDirectoryListingCategory = null;
      this.sourceDirectoryListingCategory = null;
    }
    else {
      let directoryListingCategory = new DirectoryListingCategory();
      Object.assign(directoryListingCategory, this.editedDirectoryListingCategory);
      this.editedDirectoryListingCategory = null;

      let maxIndex = 0;
      for (let r of this.rowsCache) {
        if ((<any>r).index > maxIndex)
          maxIndex = (<any>r).index;
      }

      (<any>directoryListingCategory).index = maxIndex + 1;

      this.rowsCache.splice(0, 0, directoryListingCategory);
      this.rows.splice(0, 0, directoryListingCategory);
      this.rows = [...this.rows];
    }
  }




  loadData() {
    //this.alertService.startLoadingMessage();
    this.loadingIndicator = true;

    this.subscription.add(this.directoryListingCategoryService.getDirectoryListingCategoryByInstitutionId(this.accountService.currentUser.institutionId)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let directoryListingCategorys = results[0];
        let permissions = results[1];

        directoryListingCategorys.forEach((directoryListingCategory, index, directoryListingCategorys) => {
          (<any>directoryListingCategory).index = index + 1;
        });


        this.rowsCache = [...directoryListingCategorys];
        this.rows = directoryListingCategorys;

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
    this.editingDirectoryListingCategoryName = null;
    this.directoryListingCategoryEditor.resetForm(true);
  }


  newDirectoryListingCategory() {
    this.editingDirectoryListingCategoryName = null;
    this.sourceDirectoryListingCategory = null;
    this.editedDirectoryListingCategory = this.directoryListingCategoryEditor.newDirectoryListingCategory();
    this.header = 'New';
    this.openDialog(this.editedDirectoryListingCategory);
    //this.editorModal.show();
  }


  editDirectoryListingCategory(row: DirectoryListingCategory) {
    this.editingDirectoryListingCategoryName = { key: row.code };
    this.sourceDirectoryListingCategory = row;
    this.editedDirectoryListingCategory = this.directoryListingCategoryEditor.editDirectoryListingCategory(row);
    this.header = 'Edit';
    //this.editorModal.show();
    this.openDialog(this.editedDirectoryListingCategory);
  }

  deleteDirectoryListingCategory(row: DirectoryListingCategory) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.code + '\"?', DialogType.confirm, () => this.deleteDirectoryListingCategoryHelper(row));
  }


  deleteDirectoryListingCategoryHelper(row: DirectoryListingCategory) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.subscription.add(this.directoryListingCategoryService.deleteDirectoryListingCategory(row.id)
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


  get canManageDirectoryListingCategorys() {
    return true;//this.accountService.userHasPermission(Permission.manageDirectoryListingCategoryPermission);
  }

}
