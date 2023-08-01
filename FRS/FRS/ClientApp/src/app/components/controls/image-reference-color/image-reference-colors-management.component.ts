import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { ImageReferenceColor } from 'src/app/models/image-reference-color.model';
import { ImageReferenceColorEditorComponent } from './image-reference-color-editor.component';
import { ImageReferenceColorService } from 'src/app/services/image-reference-color.service';


@Component({
  selector: 'image-reference-colors-management',
  templateUrl: './image-reference-colors-management.component.html',
  styleUrls: ['./image-reference-colors-management.component.css']
})
export class ImageReferenceColorsManagementComponent implements OnInit {
  columns: any[] = [];
  rows: ImageReferenceColor[] = [];
  rowsCache: ImageReferenceColor[] = [];
  allPermissions: Permission[] = [];
  editedImageReferenceColor: ImageReferenceColor;
  sourceImageReferenceColor: ImageReferenceColor;
  editingImageReferenceColorName: { name: string };
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('colorTemplate')
  colorTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('imageReferenceColorEditor')
  imageReferenceColorEditor: ImageReferenceColorEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private imageReferenceColorService: ImageReferenceColorService, public dialog: MatDialog) {
  }

  openDialog(imageReferenceColor: ImageReferenceColor): void {
    const dialogRef = this.dialog.open(ImageReferenceColorEditorComponent, {
      data: { header: this.header, imageReferenceColor: imageReferenceColor },
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
      { prop: 'colorCode', name: 'Colour', width: 350, cellTemplate: this.colorTemplate },
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
    this.filter.filters = '(IsActive)==true,(Name)@=' + this.keyword + ',(InstitutionId)==' + this.accountService.currentUser.institutionId;
    
    this.imageReferenceColorService.getImageReferenceColorsByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let imageReferenceColors = results.pagedData;

        imageReferenceColors.forEach((imageReferenceColor, index, imageReferenceColors) => {
          (<any>imageReferenceColor).index = index + 1;
        });


        this.rowsCache = [...imageReferenceColors];
        this.rows = imageReferenceColors;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve image reference colours from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }


  onSearchChanged(value: string) {
    //this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.name, r.description));
    this.keyword = value;
    this.loadData(null);
  }


  onEditorModalHidden() {
    this.editingImageReferenceColorName = null;
    this.imageReferenceColorEditor.resetForm(true);
  }


  newImageReferenceColor() {
    //this.editingImageReferenceColorName = null;
    //this.sourceImageReferenceColor = null;
    //this.editedImageReferenceColor = this.imageReferenceColorEditor.newImageReferenceColor();
    //this.editorModal.show();
    this.header = 'New Image Reference Colour';
    this.editedImageReferenceColor = new ImageReferenceColor();
    this.openDialog(this.editedImageReferenceColor);
  }


  editImageReferenceColor(row: ImageReferenceColor) {
    //this.editingImageReferenceColorName = { name: row.name };
    //this.sourceImageReferenceColor = row;
    this.editedImageReferenceColor = row; //this.imageReferenceColorEditor.editImageReferenceColor(row);
    //this.editorModal.show();

    this.header = 'Edit Image Reference Colour';
    this.openDialog(this.editedImageReferenceColor);
  }

  deleteImageReferenceColor(row: ImageReferenceColor) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" Colour?', DialogType.confirm, () => this.deleteImageReferenceColorHelper(row));
  }


  deleteImageReferenceColorHelper(row: ImageReferenceColor) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.imageReferenceColorService.deleteImageReferenceColor(row)
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

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the image Reference Colour.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }


  get canManageImageReferenceColors() {
    return this.accountService.userHasPermission(Permission.manageImageReferenceColorsPermission)
  }

}
