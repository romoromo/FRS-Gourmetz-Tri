import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, ChangeDetectorRef, NgModule, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Permission } from '../../../models/permission.model';
import { Location } from '../../../models/location.model';
import { Institution } from '../../../models/institution.model';
import { ActivatedRoute } from '@angular/router';
import { DateOnlyPipe } from 'src/app/pipes/datetime.pipe';
import { MatDialog } from '@angular/material';
import { PIBTemplate, PIBDevice, PIBTemplateLocation } from 'src/app/models/department.model';
import { PIBDeviceEditorComponent } from './pib-device-editor.component';
import { PIBService } from 'src/app/services/pib.service';
import * as signalRCore from '@aspnet/signalr';
import { ConfigurationService } from 'src/app/services/configuration.service';
//var oldSignalR: any = require('../../../assets/scripts/jquery.signalR-2.3.0.min.js');
//let $: JQueryStatic = (window as any)["jQuery"];

//import { SignalRModule } from 'ng2-signalr';
//import { IConnectionOptions, SignalR } from 'ng2-signalr';


export interface DialogData {
  header: string;
  pibDevice: PIBDevice;
  locations: Location[];
  institutions: Institution[];
  isApproval: boolean,
  data: any
}

@Component({
  selector: 'pib-devices-management',
  templateUrl: './pib-devices-management.component.html',
  styleUrls: ['./pib-devices-management.component.css']
})
export class PIBDevicesManagementComponent implements OnInit, AfterViewInit, OnDestroy {
  columns: any[] = [];
  rows: PIBDevice[] = [];
  rowsCache: PIBDevice[] = [];
  allLocations: PIBTemplateLocation[] = [];
  allInstitutions: Institution[] = [];
  editedPIBDevice: PIBDevice;
  sourcePIBDevice: PIBDevice;
  editingPIBDeviceCode: { code: string };
  loadingIndicator: boolean;

  @Input()
  isApproval: boolean;

  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('statusTemplate')
  statusTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('pibDeviceEditor')
  pibDeviceEditor: PIBDeviceEditorComponent;

  _route: ActivatedRoute;
  header: string;
  private connection: signalRCore.HubConnection;

  deviceInterval: any;
  locationInterval: any;
  ////signalR connection reference
  //private pibConnection: SignalR;

  ////signalR proxy reference
  //private proxy: SignalR.Hub.Proxy;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private pibDeviceService: PIBService, private route: ActivatedRoute, public dialog: MatDialog, private cd: ChangeDetectorRef,
    private configurationService: ConfigurationService  ) {
    this._route = route;

    //this.pibConnection.hub.start().done(function () {
    //  console.log("Connected, transport = " + $.connection.hub.transport.name);
    //});

    //initialize connection
    //this.pibConnection = $.connection;
    //this.pibConnection.hub.url = 'http://119.73.206.36:89/signalr';

    ////to create proxy give your hub class name as parameter. IMPORTANT: notice that I followed camel casing in giving class name
    //this.proxy = this.pibConnection.hub.createHubProxy('patientHub');
    ////$.connection.hub.qs = { "identifier": 1 };
    ////$.connection.hub.qs = { "bed": bed };
    //this.pibConnection.hub.start()
    //  .done(function () {
    //    console.log("Connected, connection ID = " + this.pibConnection.hub.id + ", transport = " + this.pibConnection.hub.transport.name);
    //  })
    //  .fail(function () {
    //    console.log("Could not connect!");
    //  });

    //define a callback method for proxy
    //this.proxy.on('newPatientUpdate', (obj) => console.log(obj));

    //this.connection = new signalRCore.HubConnectionBuilder()
    //  .withUrl("/hub/frs")
    //  .build();
    this.connection = this.accountService.signalRConnection(this.configurationService.baseUrl + "/hub/frs", true);
    if (this.connection != null) {
      this.connection.on("RefreshPIBDeviceList", (device: PIBDevice) => {
        console.log(device);

        //this.loadData();
        if (!this.rows) {
          this.rows = [];
        }

        //find the device id; add/update
        let isNew = true;
        this.rows.forEach((pibdevice, index, devices) => {
          if (device.id == pibdevice.id) {
            pibdevice = device;
            this.rows[index] = device;
            isNew = false;
          }
        });

        if (isNew) {
          this.rows.push(device);
        }

        this.rows = [...this.rows]

        //this.rows = [];
        //this.rowsCache = [];
        //rows.push(device);
        //this.rows = rows;
        this.rowsCache = [...this.rows];
        //this.cd.detectChanges();

      });
    }

    //this.connection.start().catch(err => {
    //  console.log(err);
    //});
  }

  openDialog(pibDevice: PIBDevice, isApproval?: boolean): void {
    const dialogRef = this.dialog.open(PIBDeviceEditorComponent, {
      data: { header: this.header, pibDevice: pibDevice, locations: this.allLocations, institutions: this.allInstitutions, isApproval: isApproval},
      width: '600px'
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData();
    });
  }
  ngOnInit() {

    let gT = (key: string) => this.translationService.getTranslation(key);

    this.columns = [
      { prop: "index", name: '#', width: 50, cellTemplate: this.indexTemplate, canAutoResize: false },
      { prop: 'device_code', name: gT('devices.management.Code') },
      { prop: 'device_label', name: gT('devices.management.Label') },
      { prop: 'ip_address', name: gT('devices.management.IpAddress') },
      { prop: 'location_code', name: gT('devices.management.Location') },
      { prop: 'mac_address', name: gT('devices.management.MacAddress') },
      //{ prop: 'status_display', name: gT('devices.management.Status'), width: 80 },
      { name: gT('devices.management.Status'), code: '', width: 50, cellTemplate: this.statusTemplate, resizeable: false, canAutoResize: true, sortable: false, draggable: false },
      { code: '', width: 150, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: true, sortable: false, draggable: false }
    ];
    var component = this;
    this.deviceInterval = setInterval(() => { component.loadData(); }, 60000);
    this.locationInterval = setInterval(() => { component.getLocations(false); }, 60000);
    this.loadData();
    this.getLocations(true);
    
  }

  ngAfterViewInit() {

    this.pibDeviceEditor.changesSavedCallback = () => {
      this.addNewPIBDeviceToList();
      //this.editorModal.hide();
    };

    this.pibDeviceEditor.changesCancelledCallback = () => {
      this.editedPIBDevice = null;
      this.sourcePIBDevice = null;
      //this.editorModal.hide();
    };
  }

  ngOnDestroy() {
    clearInterval(this.locationInterval);
    clearInterval(this.deviceInterval);
  }
  addNewPIBDeviceToList() {
    if (this.sourcePIBDevice) {
      Object.assign(this.sourcePIBDevice, this.editedPIBDevice);

      let sourceIndex = this.rowsCache.indexOf(this.sourcePIBDevice, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rowsCache, sourceIndex, 0);

      sourceIndex = this.rows.indexOf(this.sourcePIBDevice, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rows, sourceIndex, 0);

      this.editedPIBDevice = null;
      this.sourcePIBDevice = null;
    }
    else {
      let pibDevice = new PIBDevice();
      Object.assign(pibDevice, this.editedPIBDevice);
      this.editedPIBDevice = null;

      let maxIndex = 0;
      for (let r of this.rowsCache) {
        if ((<any>r).index > maxIndex)
          maxIndex = (<any>r).index;
      }

      (<any>pibDevice).index = maxIndex + 1;

      this.rowsCache.splice(0, 0, pibDevice);
      this.rows.splice(0, 0, pibDevice);
      this.rows = [...this.rows];
    }
  }

  syncLocations() {
    this.alertService.startLoadingMessage("Synchronizing locations...");
    this.loadingIndicator = true;

    this.pibDeviceService.syncLocations()
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;
        console.log(results);
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to sync locations from the server.`,
            MessageSeverity.error);
        });
  }

  getLocations(isShowMessage: boolean) {
    if (isShowMessage) {
      this.alertService.startLoadingMessage("Loading locations...");
    }
    this.pibDeviceService.getLocations()
      .subscribe(results => {
        this.allLocations = results[0];
        this.alertService.stopLoadingMessage();
      },
        error => {
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.`,
            MessageSeverity.error);
        });
  }

  loadData() {
    this.alertService.startLoadingMessage();
    this.loadingIndicator = true;

    this.pibDeviceService.getDevices()
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let pibDevices = results[0];

        pibDevices.forEach((pibDevice, index, pibDevices) => {
          (<any>pibDevice).index = index + 1;
        });


        this.rowsCache = [...pibDevices];
        this.rows = pibDevices;

        
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve pib devices from the server.`,
            MessageSeverity.error);
        });
  }


  onSearchChanged(value: string) {
    this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.device_code, r.device_label, r.mac_address, r.location_code, r.ip_address));
  }


  onEditorModalHidden() {
    this.editingPIBDeviceCode = null;
    this.pibDeviceEditor.resetForm(true);
  }


  newPIBDevice() {
    this.editingPIBDeviceCode = null;
    this.sourcePIBDevice = null;
    this.editedPIBDevice = this.pibDeviceEditor.newPIBDevice(this.allLocations);
    this.header = 'New PIB Device';
    this.openDialog(this.editedPIBDevice, false);
    //this.editorModal.show();
  }


  editPIBDevice(row: PIBDevice, isApproval?: boolean) {
    this.editingPIBDeviceCode = { code: row.device_code };
    this.sourcePIBDevice = row;
    this.editedPIBDevice = this.pibDeviceEditor.editPIBDevice(row, this.allLocations);
    this.header = 'View PIB Device';//row.device_status == 'Approved' ? 'View PIBDevice' : 'Edit PIBDevice';
    this.openDialog(this.editedPIBDevice, isApproval);
    //this.editorModal.show();
  }

  deletePIBDevice(row: PIBDevice) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.device_label + '\" pib device?', DialogType.confirm, () => this.deletePIBDeviceHelper(row));
  }


  deletePIBDeviceHelper(row: PIBDevice) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.pibDeviceService.deletePIBDevice(row)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.rowsCache = this.rowsCache.filter(item => item !== row)
        this.rows = this.rows.filter(item => item !== row)
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the pib device.`,
            MessageSeverity.error);
        });
  }


  get canManagePIBDevices() {
    return true; //this.accountService.userHasPermission(Permission.managePIBDevicesPermission)
  }
}
