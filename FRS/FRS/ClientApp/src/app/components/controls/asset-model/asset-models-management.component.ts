import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { AssetModel } from 'src/app/models/asset-model.model';
import { AssetModelEditorComponent } from './asset-model-editor.component';
import { AssetService } from 'src/app/services/asset.service';


@Component({
  selector: 'asset-models-management',
  templateUrl: './asset-models-management.component.html',
  styleUrls: ['./asset-models-management.component.css']
})
export class AssetModelsManagementComponent implements OnInit {
  columns: any[] = [];
  rows: AssetModel[] = [];
  rowsCache: AssetModel[] = [];
  allPermissions: Permission[] = [];
  editedAssetModel: AssetModel;
  sourceAssetModel: AssetModel;
  editingAssetModelName: { name: string };
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

  @ViewChild('assetModelEditor')
  assetModelEditor: AssetModelEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private assetService: AssetService, public dialog: MatDialog) {
  }

  openDialog(assetModel: AssetModel): void {
    const dialogRef = this.dialog.open(AssetModelEditorComponent, {
      data: { header: this.header, assetModel: assetModel },
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
      { prop: 'name', name: 'Model' },
      { prop: 'assetTypeName', name: 'Asset Type' },
      { prop: 'notes', name: 'Notes' },
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
    this.filter.filters = '(IsActive)==true,(Name|Model|Notes)@=' + this.keyword + ',(InstitutionId)==' + this.accountService.currentUser.institutionId;
    
    this.assetService.getAssetModelsByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let assetModels = results.pagedData;

        assetModels.forEach((assetModel, index, assetModels) => {
          (<any>assetModel).index = index + 1;
        });


        this.rowsCache = [...assetModels];
        this.rows = assetModels;

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
    this.editingAssetModelName = null;
    this.assetModelEditor.resetForm(true);
  }


  newAssetModel() {
    //this.editingAssetModelName = null;
    //this.sourceAssetModel = null;
    //this.editedAssetModel = this.assetModelEditor.newAssetModel();
    //this.editorModal.show();
    this.header = 'New Asset Model';
    this.editedAssetModel = new AssetModel();
    this.openDialog(this.editedAssetModel);
  }


  editAssetModel(row: AssetModel) {
    //this.editingAssetModelName = { name: row.name };
    //this.sourceAssetModel = row;
    this.editedAssetModel = row; //this.assetModelEditor.editAssetModel(row);
    //this.editorModal.show();

    this.header = 'Edit Asset Model';
    this.openDialog(this.editedAssetModel);
  }

  deleteAssetModel(row: AssetModel) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" asset Model?', DialogType.confirm, () => this.deleteAssetModelHelper(row));
  }


  deleteAssetModelHelper(row: AssetModel) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.assetService.deleteAssetModel(row.id)
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

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the asset model.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }


  get canManageAssetModels() {
    return this.accountService.userHasPermission(Permission.manageAssetModelsPermission)
  }

}
