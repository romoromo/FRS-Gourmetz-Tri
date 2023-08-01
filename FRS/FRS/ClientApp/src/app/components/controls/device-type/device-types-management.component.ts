import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { DeviceType } from 'src/app/models/device-type.model';
import { DeviceTypeEditorComponent } from './device-type-editor.component';
import { DeviceService } from 'src/app/services/device.service';
import { FileService } from 'src/app/services/file.service';


@Component({
  selector: 'device-types-management',
  templateUrl: './device-types-management.component.html',
  styleUrls: ['./device-types-management.component.css']
})
export class DeviceTypesManagementComponent implements OnInit {
  columns: any[] = [];
  rows: DeviceType[] = [];
  rowsCache: DeviceType[] = [];
  allPermissions: Permission[] = [];
  editedDeviceType: DeviceType;
  sourceDeviceType: DeviceType;
  editingDeviceTypeName: { name: string };
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('deviceTypeEditor')
  deviceTypeEditor: DeviceTypeEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private fileService: FileService, private deviceService: DeviceService, public dialog: MatDialog) {
  }

  openDialog(deviceType: DeviceType): void {
    const dialogRef = this.dialog.open(DeviceTypeEditorComponent, {
      data: { header: this.header, deviceType: deviceType },
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
    
    this.deviceService.getDeviceTypesByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let deviceTypes = results.pagedData;

        deviceTypes.forEach((deviceType, index, deviceTypes) => {
          (<any>deviceType).index = index + 1;
        });


        this.rowsCache = [...deviceTypes];
        this.rows = deviceTypes;

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
    this.editingDeviceTypeName = null;
    this.deviceTypeEditor.resetForm(true);
  }


  newDeviceType() {
    //this.editingDeviceTypeName = null;
    //this.sourceDeviceType = null;
    //this.editedDeviceType = this.deviceTypeEditor.newDeviceType();
    //this.editorModal.show();
    this.header = 'New Device Type';
    this.editedDeviceType = new DeviceType();
    this.openDialog(this.editedDeviceType);
  }


  editDeviceType(row: DeviceType) {
    //this.editingDeviceTypeName = { name: row.name };
    //this.sourceDeviceType = row;
    this.editedDeviceType = row; //this.deviceTypeEditor.editDeviceType(row);
    //this.editorModal.show();

    this.header = 'Edit Device Type';
    this.openDialog(this.editedDeviceType);
  }

  deleteDeviceType(row: DeviceType) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" device type?', DialogType.confirm, () => this.deleteDeviceTypeHelper(row));
  }


  deleteDeviceTypeHelper(row: DeviceType) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.deviceService.deleteDeviceType(row.id)
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

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the device type.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  getFileImage(path) {
    return this.fileService.getFile(path);
  }

  get canManageDeviceTypes() {
    return this.accountService.userHasPermission(Permission.manageDeviceTypesPermission)
  }

}
