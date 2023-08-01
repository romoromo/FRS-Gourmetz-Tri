import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, Inject } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Asset } from 'src/app/models/asset.model';
import { AssetEditorComponent } from './asset-editor.component';
import { AssetService } from 'src/app/services/asset.service';
import { FileService } from 'src/app/services/file.service';
import { DateOnlyPipe } from 'src/app/pipes/datetime.pipe';
import { HttpEvent, HttpEventType } from '@angular/common/http';
import { InstitutionService } from '../../../services/institution.service';


@Component({
  selector: 'assets-management',
  templateUrl: './assets-management.component.html',
  styleUrls: ['./assets-management.component.css']
})
export class AssetsManagementComponent implements OnInit {
  columns: any[] = [];
  rows: Asset[] = [];
  rowsCache: Asset[] = [];
  allPermissions: Permission[] = [];
  editedAsset: Asset;
  sourceAsset: Asset;
  editingAssetName: { name: string };
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  public progress: number;
  public message: string;
  public filename: string;
  fileTypes: string;
  showTxt: boolean;

  public allInstitutions = [];
  public institutionId = '';

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('assetEditor')
  assetEditor: AssetEditorComponent;
  header: string;
    selectToggle: boolean = false;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService, private institutionService: InstitutionService,
    private fileService: FileService, private assetService: AssetService, public dialog: MatDialog) {

    this.loadInstitutions();
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
    this.assetService.importFile<HttpEvent<Object>>(formData)
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
      },
        error => {
          this.alertService.stopLoadingMessage();

          this.alertService.showStickyMessage("Upload Error", `Upload error.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  onUploadFinished() {
    this.alertService.showStickyMessage("Upload Success", `Upload Success`,
      MessageSeverity.success);
    //const currentRoute = this.router.url;

    //this.router.navigateByUrl('/', { skipLocationChange: true }).then(() => {
    //this.router.navigate([currentRoute]); // navigate to same route
    //}); 
    this.loadData();
  }

  openDialog(asset: Asset): void {
    const dialogRef = this.dialog.open(AssetEditorComponent, {
      data: { header: this.header, asset: asset },
      width: '400px'
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData(null);
    });
  }

  openQrDialog(asset: Asset): void {
    const dialogRef = this.dialog.open(AssetQrCodeComponent, {
      data: `extract : ${asset.assetTypeName}   
   model : ${asset.assetModelName || null}   
   SN : ${asset.serialNumber}   
   start : ${asset.warrantyStart || null}   
   end date : ${asset.warrantyEnd || ''}`,
      width: '90vw'
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData(null);
    });
  }

  initializeFilter() {
    this.filter = new Filter(1, 10);
    this.filter.sorts = 'serialNumber';
    this.filter.filters = '';
    this.filter.page = 0;

    
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
      { prop: 'serialNumber', name: 'Serial No.' },
      { prop: 'assetModelName', name: 'Asset Model' },
      { prop: 'assetTypeName', name: 'Asset Type' },
      { prop: 'purchaseDate', name: 'Purchase Date', pipe: new DateOnlyPipe('en-SG') },
      { prop: 'warrantyStart', name: 'Warranty Start', pipe: new DateOnlyPipe('en-SG') },
      { prop: 'warrantyEnd', name: 'Warranty End', pipe: new DateOnlyPipe('en-SG') },
      { prop: 'locationName', name: 'Location' },
      { prop: 'institutionName', name: 'Institution' },
      { name: '', width: 150, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
    ];
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
      this.filter.page = ev.offset;
      if (ev.sorts) {
        this.filter.sorts = ev.sorts[0].dir == 'desc' ? '-' + ev.sorts[0].prop : ev.sorts[0].prop;
      }
    }

    if (!this.keyword) this.keyword = '';
    this.filter.filters = '(IsActive)==true,(SerialNumber|assetModelName|assetTypeName|locationName)@=' + this.keyword + ',(InstitutionId)==' + this.institutionId;
    
    this.assetService.getAssetsByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let assets = results.pagedData;

        assets.forEach((asset, index, assets) => {
          (<any>asset).index = index + 1;
        });


        this.rowsCache = [...assets];
        this.rows = assets;

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
    this.editingAssetName = null;
    this.assetEditor.resetForm(true);
  }


  newAsset() {
    //this.editingAssetName = null;
    //this.sourceAsset = null;
    //this.editedAsset = this.assetEditor.newAsset();
    //this.editorModal.show();
    this.header = 'New Asset';
    this.editedAsset = new Asset();
    this.openDialog(this.editedAsset);
  }


  editAsset(row: Asset) {
    //this.editingAssetName = { name: row.name };
    //this.sourceAsset = row;
    this.editedAsset = row; //this.assetEditor.editAsset(row);
    //this.editorModal.show();

    this.header = 'Edit Asset';
    this.openDialog(this.editedAsset);
  }

  deleteAsset(row: Asset) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.serialNumber + '\" asset?', DialogType.confirm, () => this.deleteAssetHelper(row));
  }


  deleteAssetHelper(row: Asset) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.assetService.deleteAsset(row.id)
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

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the asset.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  loadInstitutions() {
    this.institutionService.getInstitutions()
      .subscribe(results => {
        this.alertService.stopLoadingMessage();

        this.allInstitutions = results[0];

      },
        error => {
          this.alertService.stopLoadingMessage();

          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving institutions.`,
            MessageSeverity.error);
        });
  }

  selectAll() {
    this.selectToggle = !this.selectToggle;
    for (let r of this.rows) {
      r.selected = this.selectToggle;
    }
  }

  deleteSelected() {
    this.alertService.showDialog('Are you sure you want to delete the selected assets?', DialogType.confirm, () => {
      for (let r of this.rows) {
        if (r.selected) this.deleteAssetHelper(r);
      }
    });
  }

  get canManageAssets() {
    return this.accountService.userHasPermission(Permission.manageAssetsPermission)
  }

}

@Component({
  template: `<div [style.width]="100" [style.text-align]="'center'">
          <qr-code [value]="data" [size] = "512" [level] = "'H'" > </qr-code> <br />

          <button type="button" mat-button (click)="cancel()" ><i class='fa fa-times'></i> {{ 'Close' }}</button>
    </div>`
})
export class AssetQrCodeComponent {
  constructor(
    public dialogRef: MatDialogRef<AssetQrCodeComponent>,
    @Inject(MAT_DIALOG_DATA) public data: string,
  ) { }

  private cancel() {
    this.dialogRef.close();
  }
}
