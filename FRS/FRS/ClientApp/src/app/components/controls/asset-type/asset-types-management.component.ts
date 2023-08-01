import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { AssetType } from 'src/app/models/asset-type.model';
import { AssetTypeEditorComponent } from './asset-type-editor.component';
import { AssetService } from 'src/app/services/asset.service';
import { FileService } from 'src/app/services/file.service';


@Component({
  selector: 'asset-types-management',
  templateUrl: './asset-types-management.component.html',
  styleUrls: ['./asset-types-management.component.css']
})
export class AssetTypesManagementComponent implements OnInit {
  columns: any[] = [];
  rows: AssetType[] = [];
  rowsCache: AssetType[] = [];
  allPermissions: Permission[] = [];
  editedAssetType: AssetType;
  sourceAssetType: AssetType;
  editingAssetTypeName: { name: string };
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @ViewChild('iconTemplate')
  iconTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('assetTypeEditor')
  assetTypeEditor: AssetTypeEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private fileService: FileService, private assetService: AssetService, public dialog: MatDialog) {
  }

  openDialog(assetType: AssetType): void {
    const dialogRef = this.dialog.open(AssetTypeEditorComponent, {
      data: { header: this.header, assetType: assetType },
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
      { prop: 'name', name: gT('common.Name'), width: 200 },
      { name: 'Icon', width: 200, cellTemplate: this.iconTemplate },
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
    
    this.assetService.getAssetTypesByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let assetTypes = results.pagedData;

        assetTypes.forEach((assetType, index, assetTypes) => {
          (<any>assetType).index = index + 1;
        });


        this.rowsCache = [...assetTypes];
        this.rows = assetTypes;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve records from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }


  onSearchChanged(value: string) {
    //this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.name, r.description));
    this.keyword = value;
    this.loadData(null);
  }


  onEditorModalHidden() {
    this.editingAssetTypeName = null;
    this.assetTypeEditor.resetForm(true);
  }


  newAssetType() {
    //this.editingAssetTypeName = null;
    //this.sourceAssetType = null;
    //this.editedAssetType = this.assetTypeEditor.newAssetType();
    //this.editorModal.show();
    this.header = 'New Asset Type';
    this.editedAssetType = new AssetType();
    this.openDialog(this.editedAssetType);
  }


  editAssetType(row: AssetType) {
    //this.editingAssetTypeName = { name: row.name };
    //this.sourceAssetType = row;
    this.editedAssetType = row; //this.assetTypeEditor.editAssetType(row);
    //this.editorModal.show();

    this.header = 'Edit Asset Type';
    this.openDialog(this.editedAssetType);
  }

  deleteAssetType(row: AssetType) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" asset type?', DialogType.confirm, () => this.deleteAssetTypeHelper(row));
  }


  deleteAssetTypeHelper(row: AssetType) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.assetService.deleteAssetType(row.id)
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

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the asset type.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  getFileImage(path) {
    return this.fileService.getFile(path);
  }

  get canManageAssetTypes() {
    return this.accountService.userHasPermission(Permission.manageAssetTypesPermission)
  }

}
