import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { ImageReferenceType } from 'src/app/models/image-reference-type.model';
import { ImageReferenceTypeEditorComponent } from './image-reference-type-editor.component';
import { ImageReferenceTypeService } from 'src/app/services/image-reference-type.service';


@Component({
  selector: 'image-reference-types-management',
  templateUrl: './image-reference-types-management.component.html',
  styleUrls: ['./image-reference-types-management.component.css']
})
export class ImageReferenceTypesManagementComponent implements OnInit {
  columns: any[] = [];
  rows: ImageReferenceType[] = [];
  rowsCache: ImageReferenceType[] = [];
  allPermissions: Permission[] = [];
  editedImageReferenceType: ImageReferenceType;
  sourceImageReferenceType: ImageReferenceType;
  editingImageReferenceTypeName: { name: string };
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('imageReferenceTypeEditor')
  imageReferenceTypeEditor: ImageReferenceTypeEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private imageReferenceTypeService: ImageReferenceTypeService, public dialog: MatDialog) {
  }

  openDialog(imageReferenceType: ImageReferenceType): void {
    const dialogRef = this.dialog.open(ImageReferenceTypeEditorComponent, {
      data: { header: this.header, imageReferenceType: imageReferenceType },
      width: '400px'
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData(null);
    });
  }

  initializeFilter() {
    this.filter = new Filter(1, 10);
    this.filter.sorts = 'name';
    this.filter.filters = '';
    this.filter.page = 1;

    
  }

  initializePagedResult() {
    this.pagedResult = new PagedResult();
    this.pagedResult.totalCount = 0;
    this.pagedResult.pagedData = [];
    this.pagedResult.filter = this.filter;
  }

  initializeTableDefinition() {
    let gT = (key: string) => this.translationService.getTranslation(key);

    this.columns = [
      //{ prop: "index", name: '#', width: 50, cellTemplate: this.indexTemplate, canAutoResize: false },
      { prop: 'name', name: gT('common.Name'), width: 200 },
      { prop: 'description', name: gT('common.Description'), width: 350 },
      { name: '', width: 150, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
    ];

    if (!this.accountService.currentUser.institutionId || this.accountService.currentUser.institutionId == '0') {
      this.columns.splice(1, 0, { prop: 'institutionName', name: gT('roles.management.Institution'), width: 120 });
    }
  }

  ngOnInit() {
    this.initializeFilter();
    this.initializePagedResult();
    this.initializeTableDefinition();
    this.loadData();
  }


  loadData(ev?: any) {
    this.alertService.startLoadingMessage();
    this.loadingIndicator = true;
    this.filter.pageSize = 10;

    if (ev) {
      this.filter.page = ev.offset + 1;
      if (ev.sorts) {
        this.filter.sorts = ev.sorts[0].dir == 'desc' ? '-' + ev.sorts[0].prop : ev.sorts[0].prop;
      }
    }

    if (!this.keyword) this.keyword = '';
    this.filter.filters = '(IsActive)==true,(Name|Description)@=' + this.keyword + ',(InstitutionId)==' + this.accountService.currentUser.institutionId;
    
    this.imageReferenceTypeService.getImageReferenceTypesByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let imageReferenceTypes = results.pagedData;

        imageReferenceTypes.forEach((imageReferenceType, index, imageReferenceTypes) => {
          (<any>imageReferenceType).index = index + 1;
        });


        this.rowsCache = [...imageReferenceTypes];
        this.rows = imageReferenceTypes;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve image reference types from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }


  onSearchChanged(value: string) {
    //this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.name, r.description));
    this.keyword = value;
    this.loadData(null);
  }


  onEditorModalHidden() {
    this.editingImageReferenceTypeName = null;
    this.imageReferenceTypeEditor.resetForm(true);
  }


  newImageReferenceType() {
    //this.editingImageReferenceTypeName = null;
    //this.sourceImageReferenceType = null;
    //this.editedImageReferenceType = this.imageReferenceTypeEditor.newImageReferenceType();
    //this.editorModal.show();
    this.header = 'New Image Reference Type';
    this.editedImageReferenceType = new ImageReferenceType();
    this.openDialog(this.editedImageReferenceType);
  }


  editImageReferenceType(row: ImageReferenceType) {
    //this.editingImageReferenceTypeName = { name: row.name };
    //this.sourceImageReferenceType = row;
    this.editedImageReferenceType = row; //this.imageReferenceTypeEditor.editImageReferenceType(row);
    //this.editorModal.show();

    this.header = 'Edit Image Reference Type';
    this.openDialog(this.editedImageReferenceType);
  }

  deleteImageReferenceType(row: ImageReferenceType) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" imageReferenceType?', DialogType.confirm, () => this.deleteImageReferenceTypeHelper(row));
  }


  deleteImageReferenceTypeHelper(row: ImageReferenceType) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.imageReferenceTypeService.deleteImageReferenceType(row)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        //this.rowsCache = this.rowsCache.filter(item => item !== row)
        //this.rows = this.rows.filter(item => item !== row)
        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the image Reference Type.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }


  get canManageImageReferenceTypes() {
    return this.accountService.userHasPermission(Permission.manageImageReferenceTypesPermission)
  }

}
