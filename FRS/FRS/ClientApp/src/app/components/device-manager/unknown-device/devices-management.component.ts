import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { DeviceService } from '../../../services/device.service';
import { Utilities } from '../../../services/utilities';
import { Device } from '../../../models/device.model';
import { Permission } from '../../../models/permission.model';
import { DeviceEditorComponent } from "./device-editor.component";
import { Location } from '../../../models/location.model';
import { Institution } from '../../../models/institution.model';
import { ActivatedRoute } from '@angular/router';
import { DateOnlyPipe } from 'src/app/pipes/datetime.pipe';
import { MatDialog } from '@angular/material';
import { CommonFilter, PagedResult } from 'src/app/models/sieve-filter.model';
import { ConfigurationService } from 'src/app/services/configuration.service';
import { AuthService } from 'src/app/services/auth.service';
import { ApplicationSetting } from 'src/app/models/application-setting.model';
import { ApplicationSettingService } from 'src/app/services/application-setting.service';
import { DomSanitizer } from '@angular/platform-browser';
import { Subscription } from 'rxjs';

export interface DialogData {
  header: string;
  device: Device;
  locations: Location[];
  institutions: Institution[];
  isApproval: boolean,
  data: any
}

@Component({
  selector: 'devices-management',
  templateUrl: './devices-management.component.html',
  styleUrls: ['./devices-management.component.css']
})
export class DevicesManagementComponent implements OnInit, AfterViewInit, OnDestroy {
  private deviceListSub: Subscription = new Subscription();
  private deviceDeleteSub: Subscription = new Subscription();
  columns: any[] = [];
  rows: Device[] = [];
  rowsCache: Device[] = [];
  allLocations: Location[] = [];
  allInstitutions: Institution[] = [];
  editedDevice: Device;
  sourceDevice: Device;
  editingDeviceCode: { code: string };
  loadingIndicator: boolean;
  filter: CommonFilter;
  pagedResult: PagedResult;
  keyword: string = '';
  sieveFilters = new Map<string, string>();
  private signalRCoreconnection: signalR.HubConnection;

  @Input()
  isApproval: boolean;

  @ViewChild('deviceMgtTable') table: any;

  @ViewChild('statusDisplayTemplate')
  statusDisplayTemplate: TemplateRef<any>;


  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('codeTemplate')
  codeTemplate: TemplateRef<any>;

  @ViewChild('ipAddressTemplate')
  ipAddressTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('deviceEditor')
  deviceEditor: DeviceEditorComponent;

  _route: ActivatedRoute;
  scheme: string = 'http';
  port: string = '80';
  header: string;
  public currentPageLimit: number = 20;
  public pageLimit = [
    { value: 5 },
    { value: 10 },
    { value: 25 },
    { value: 50 },
    { value: 100 },
  ];


  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private deviceService: DeviceService, private route: ActivatedRoute, public dialog: MatDialog, private configurations: ConfigurationService,
    private authService: AuthService, private applicationSetting: ApplicationSettingService, private domSanitizer: DomSanitizer) {
    this._route = route;
    //this.applicationSetting.getApplicationSettingByKey(null, 'DEVICE_ADMIN_SCHEME').subscribe(result => {
    //  this.scheme = result.value;
    //}, error => { });

    //this.applicationSetting.getApplicationSettingByKey(null, 'DEVICE_ADMIN_PORT').subscribe(result => {
    //  this.port = result.value;
    //}, error => { });
    //this.route.params.subscribe(queryParams => {
    //  this.isApproval = queryParams["type"] == 'approval';
    //  this.clearFilterAndPagedResult();
    //  this.ngOnInit();
    //});
  }
  ngOnDestroy(): void {
    this.alertService.resetStickyMessage();
    if (this.deviceListSub && !this.deviceListSub.closed) {
      this.deviceListSub.unsubscribe();
    }

    if (this.deviceDeleteSub && !this.deviceDeleteSub.closed) {
      this.deviceDeleteSub.unsubscribe();
    }
  }

  public onLimitChange(limit: any): void {
    this.currentPageLimit = parseInt(limit, 10);
    this.clearFilterAndPagedResult();
    this.filter.pageSize = this.currentPageLimit;

    this.loadData(null);
  }


  openDialog(device: Device, isApproval?: boolean): void {
    const dialogRef = this.dialog.open(DeviceEditorComponent, {
      data: { header: this.header, device: device, locations: this.allLocations, institutions: this.allInstitutions, isApproval: isApproval },
      width: '600px'
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData();
    });
  }

  initializeFilter() {
    this.filter = new CommonFilter(1, 10);
    this.filter.sorts = 'code';
    this.filter.filters = '';
    this.filter.page = 1;
    this.filter.pageSize = 20;


  }

  initializePagedResult() {
    this.pagedResult = new PagedResult();
    this.pagedResult.totalCount = 0;
    this.pagedResult.pagedData = [];
    this.pagedResult.filter = this.filter;
  }

  initializeTableDefinition() {
    this.keyword = '';
    let gT = (key: string) => this.translationService.getTranslation(key);

    this.columns = [
      //{ prop: "index", name: '#', width: 50, cellTemplate: this.indexTemplate, canAutoResize: false },
      { prop: 'connection_status_display', name: '', cellTemplate: this.statusDisplayTemplate, sortable: false },
      { prop: 'linkCode', name: gT('devices.management.Code'), sortable: false },
      { prop: 'device_label', name: gT('devices.management.Label') },
      { code: 'ipAddress', name: gT('devices.management.IpAddress'), cellTemplate: this.ipAddressTemplate },
      { prop: 'locationName', name: gT('devices.management.Location') },
      { prop: 'macAddress', name: gT('devices.management.MacAddress') },
      { prop: 'serialNumber', name: gT('devices.management.SerialNumber') },
      //{ prop: 'startDate', name: gT('devices.management.StartDate'), width: 80, pipe: new DateOnlyPipe('en-SG') },
      //{ prop: 'endDate', name: gT('devices.management.EndDate'), width: 80, pipe: new DateOnlyPipe('en-SG') },
      { prop: 'status', name: gT('devices.management.Status') },
      { name: '', cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: true, sortable: false, draggable: false }
    ];
  }

  initializeDefaultSieveFilter() {
    this.sieveFilters.set('(IsActive)==', 'true');
    //this.sieveFilters.set('(InstitutionId)==', this.accountService.currentUser.institutionId);
  }

  initializeSignalR() {
    this.signalRCoreconnection = this.authService.signalRConnection(this.configurations.baseUrl + "/hub/frs", true, this.signalRCoreconnection);
    if (this.signalRCoreconnection != null) {
      this.signalRCoreconnection.on("RefreshPIBDeviceList", (deviceVM) => {
        console.log('Refreshing Device List');
        console.log(deviceVM);
        if (!this.rows) {
          this.rows = [];
        }

        //find the device id; add/update
        this.rows.forEach((device, index, devices) => {
          if (deviceVM.id == device.id) {
            this.rows[index].status_display = deviceVM.status_display;
            this.rows[index].connection_status_display = deviceVM.connection_status_display;
            this.rows[index].device_status = deviceVM.device_status;
          }
        });

        this.rows = [...this.rows];
        this.rowsCache = [...this.rows];
        console.log(this.rows);
      });
    }
  }

  clearFilterAndPagedResult() {
    this.initializeFilter();
    this.initializePagedResult();
    this.table.offset = 0;
  }

  init() {
    this.initializeFilter();
    this.initializePagedResult();
    this.initializeTableDefinition();
    this.initializeDefaultSieveFilter();
    this.initializeSignalR();
    this.loadData(null, true);
  }

  ngOnInit() {
    this.init();

    this.route.params.subscribe(queryParams => {
      this.isApproval = queryParams["type"] == 'approval';
      this.clearFilterAndPagedResult();
      this.init();
    });
  }

  ngAfterViewInit() {

    this.deviceEditor.changesSavedCallback = () => {
      this.addNewDeviceToList();
      //this.editorModal.hide();
    };

    this.deviceEditor.changesCancelledCallback = () => {
      this.editedDevice = null;
      this.sourceDevice = null;
      //this.editorModal.hide();
    };
  }


  addNewDeviceToList() {
    if (this.sourceDevice) {
      Object.assign(this.sourceDevice, this.editedDevice);

      let sourceIndex = this.rowsCache.indexOf(this.sourceDevice, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rowsCache, sourceIndex, 0);

      sourceIndex = this.rows.indexOf(this.sourceDevice, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rows, sourceIndex, 0);

      this.editedDevice = null;
      this.sourceDevice = null;
    }
    else {
      let device = new Device();
      Object.assign(device, this.editedDevice);
      this.editedDevice = null;

      let maxIndex = 0;
      for (let r of this.rowsCache) {
        if ((<any>r).index > maxIndex)
          maxIndex = (<any>r).index;
      }

      (<any>device).index = maxIndex + 1;

      this.rowsCache.splice(0, 0, device);
      this.rows.splice(0, 0, device);
      this.rows = [...this.rows];
    }
  }




  loadData(ev?: any, isFirstLoad?: boolean) {
    this.alertService.startLoadingMessage();
    this.loadingIndicator = true;

    this.filter.institutionId = this.accountService.currentUser.institutionId;

    if (ev) {
      this.filter.page = ev.offset + 1;
      if (ev.sorts) {
        this.filter.sorts = ev.sorts[0].dir == 'desc' ? '-' + ev.sorts[0].prop : ev.sorts[0].prop;
      }
    }

    if (!this.sieveFilters) {
      this.initializeDefaultSieveFilter();
    }
    if (!this.keyword) this.keyword = '';

    let val = this.sieveFilters.has('(Code|MacAddress|SerialNumber|IpAddress|device_label)@=');
    if (val) {
      this.sieveFilters.delete('(Code|MacAddress|SerialNumber|IpAddress|device_label)@=');
    }

    if (this.keyword) {
      this.sieveFilters.set('(Code|MacAddress|SerialNumber|IpAddress|device_label)@=', this.keyword);
    }

    this.sieveFilters.set('(DeviceIsApproval)==', String(this.isApproval));

    this.filter.filters = '';
    let indx = 0;
    this.sieveFilters.forEach((value: string, key: string) => {
      if (indx > 0) {
        this.filter.filters += ','
      }
      this.filter.filters += key + value;
      indx++;
    });

    if (this.deviceListSub && !this.deviceListSub.closed) {
      this.deviceListSub.unsubscribe();
    }

    this.deviceListSub = this.deviceService.getDevicesLocationsInstitutionsByFilter(this.filter)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;
        let pagedResult = results[0];
        this.pagedResult = pagedResult;
        let devices = pagedResult.pagedData;
        let locations = results[1];
        let institutions = results[2];

        devices.forEach((device, index, devices) => {
          (<any>device).index = index + 1;
          (<any>device).linkCode = `<a href="/display/${device.code}" target="_blank">${device.code}</a>`;
        });


        this.rowsCache = [...devices];
        this.rows = devices;
        //this.onSearchChanged(this.keyword);
        this.allLocations = locations;
        this.allInstitutions = institutions;
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve devices from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }


  onSearchChanged(value: string) {
    //this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.code, r.ipAddress));
    //this.keyword = value;
    //this.clearFilterAndPagedResult();
    this.keyword = value;
    //this.loadData(null);
  }

  onSearch() {
    this.clearFilterAndPagedResult();
    this.loadData(null);
  }

  onEditorModalHidden() {
    this.editingDeviceCode = null;
    this.deviceEditor.resetForm(true);
  }


  newDevice() {
    this.editingDeviceCode = null;
    this.sourceDevice = null;
    this.editedDevice = this.deviceEditor.newDevice(this.allLocations, this.allInstitutions);
    this.header = 'New Device';
    this.openDialog(this.editedDevice, false);
    //this.editorModal.show();
  }


  editDevice(row: Device, isApproval?: boolean) {
    this.editingDeviceCode = { code: row.code };
    this.sourceDevice = row;
    this.editedDevice = this.deviceEditor.editDevice(row, this.allLocations, this.allInstitutions);
    this.header = row.status == 'Approved' ? 'View Device' : 'Edit Device';
    this.openDialog(this.editedDevice, isApproval);
    //this.editorModal.show();
  }

  deleteDevice(row: Device) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.code + '\" device?', DialogType.confirm, () => this.deleteDeviceHelper(row));
  }


  deleteDeviceHelper(row: Device) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.deviceDeleteSub = this.deviceService.deleteDevice(row)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.rowsCache = this.rowsCache.filter(item => item !== row)
        this.rows = this.rows.filter(item => item !== row)
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the device.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  sanitize(row) {
    let url = (row.scheme ? row.scheme : 'http') + '://' + row.ipAddress;
    if (row.port) {
      url = url + ':' + row.port;
    }

    url = url + '/admin';
    return this.domSanitizer.bypassSecurityTrustUrl(url);
  }

  get canManageDevices() {
    return this.accountService.userHasPermission(Permission.manageDevicesPermission)
  }

  get canApproveDevices() {
    return this.accountService.userHasPermission(Permission.approveDevicesPermission)
  }
}
