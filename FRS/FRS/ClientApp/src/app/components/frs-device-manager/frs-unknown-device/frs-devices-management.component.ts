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
import { EpaperDevice, EpaperTemplateLocation } from 'src/app/models/epaper.model';
import { EpaperDeviceEditorComponent } from './frs-device-editor.component';
import { EpaperService } from 'src/app/services/epaper.service';
import * as signalRCore from '@aspnet/signalr';
import { LocationService } from 'src/app/services/location.service';
import { ConfigurationService } from 'src/app/services/configuration.service';
//var oldSignalR: any = require('../../../assets/scripts/jquery.signalR-2.3.0.min.js');
//let $: JQueryStatic = (window as any)["jQuery"];

//import { SignalRModule } from 'ng2-signalr';
//import { IConnectionOptions, SignalR } from 'ng2-signalr';


export interface DialogData {
  header: string;
  epaperDevice: EpaperDevice;
  locations: Location[];
  institutions: Institution[];
  isApproval: boolean,
  data: any
}

@Component({
  selector: 'frs-devices-management',
  templateUrl: './frs-devices-management.component.html',
  styleUrls: ['./frs-devices-management.component.css']
})
export class EpaperDevicesManagementComponent implements OnInit, AfterViewInit, OnDestroy {
  columns: any[] = [];
  rows: EpaperDevice[] = [];
  rowsCache: EpaperDevice[] = [];
  allLocations: any = [];
  allInstitutions: Institution[] = [];
  editedEpaperDevice: EpaperDevice;
  sourceEpaperDevice: EpaperDevice;
  editingEpaperDeviceCode: { code: string };
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

  @ViewChild('epaperDeviceEditor')
  epaperDeviceEditor: EpaperDeviceEditorComponent;

  _route: ActivatedRoute;
  header: string;
  private connection: signalRCore.HubConnection;

  deviceInterval: any;
  locationInterval: any;
  ////signalR connection reference
  //private epaperConnection: SignalR;

  ////signalR proxy reference
  //private proxy: SignalR.Hub.Proxy;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private epaperDeviceService: EpaperService, private route: ActivatedRoute, public dialog: MatDialog, private cd: ChangeDetectorRef,
    protected configurations: ConfigurationService,
    private locationService: LocationService) {
    this._route = route;

    //this.epaperConnection.hub.start().done(function () {
    //  console.log("Connected, transport = " + $.connection.hub.transport.name);
    //});

    //initialize connection
    //this.epaperConnection = $.connection;
    //this.epaperConnection.hub.url = 'http://119.73.206.36:89/signalr';

    ////to create proxy give your hub class name as parameter. IMPORTANT: notice that I followed camel casing in giving class name
    //this.proxy = this.epaperConnection.hub.createHubProxy('patientHub');
    ////$.connection.hub.qs = { "identifier": 1 };
    ////$.connection.hub.qs = { "bed": bed };
    //this.epaperConnection.hub.start()
    //  .done(function () {
    //    console.log("Connected, connection ID = " + this.epaperConnection.hub.id + ", transport = " + this.epaperConnection.hub.transport.name);
    //  })
    //  .fail(function () {
    //    console.log("Could not connect!");
    //  });

    //define a callback method for proxy
    //this.proxy.on('newPatientUpdate', (obj) => console.log(obj));

    //this.connection = new signalRCore.HubConnectionBuilder()
    //  .withUrl("/hub/frs")
    //  .build();
    this.connection = this.accountService.signalRConnection(this.configurations.baseUrl + "/hub/frs");
    if (this.connection != null) {
      this.connection.on("RefreshEpaperDeviceList", (device: EpaperDevice) => {
        //this.loadData();
        var rows = this.rows;
        this.rows.push(device);
        this.rows = [...this.rows]

        //this.rows = [];
        //this.rowsCache = [];
        //rows.push(device);
        //this.rows = rows;
        this.rowsCache = [...this.rows];
        this.cd.detectChanges();
        
      });
    }

    //this.connection.start().catch(err => {
    //  console.log(err);
    //});
  }

  openDialog(epaperDevice: EpaperDevice, isApproval?: boolean): void {
    const dialogRef = this.dialog.open(EpaperDeviceEditorComponent, {
      data: { header: this.header, epaperDevice: epaperDevice, locations: this.allLocations, institutions: this.allInstitutions, isApproval: isApproval},
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
      { prop: 'device_code', name: gT('devices.management.Code')},
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

    this.epaperDeviceEditor.changesSavedCallback = () => {
      this.addNewEpaperDeviceToList();
      //this.editorModal.hide();
    };

    this.epaperDeviceEditor.changesCancelledCallback = () => {
      this.editedEpaperDevice = null;
      this.sourceEpaperDevice = null;
      //this.editorModal.hide();
    };
  }

  ngOnDestroy() {
    clearInterval(this.locationInterval);
    clearInterval(this.deviceInterval);
  }
  addNewEpaperDeviceToList() {
    if (this.sourceEpaperDevice) {
      Object.assign(this.sourceEpaperDevice, this.editedEpaperDevice);

      let sourceIndex = this.rowsCache.indexOf(this.sourceEpaperDevice, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rowsCache, sourceIndex, 0);

      sourceIndex = this.rows.indexOf(this.sourceEpaperDevice, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rows, sourceIndex, 0);

      this.editedEpaperDevice = null;
      this.sourceEpaperDevice = null;
    }
    else {
      let epaperDevice = new EpaperDevice();
      Object.assign(epaperDevice, this.editedEpaperDevice);
      this.editedEpaperDevice = null;

      let maxIndex = 0;
      for (let r of this.rowsCache) {
        if ((<any>r).index > maxIndex)
          maxIndex = (<any>r).index;
      }

      (<any>epaperDevice).index = maxIndex + 1;

      this.rowsCache.splice(0, 0, epaperDevice);
      this.rows.splice(0, 0, epaperDevice);
      this.rows = [...this.rows];
    }
  }

  syncLocations() {
    this.alertService.startLoadingMessage("Synchronizing locations...");
    this.loadingIndicator = true;

    this.epaperDeviceService.syncLocations()
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
    this.locationService.getLocations(null, null, this.accountService.currentUser.institutionId)
      .subscribe(results => {
        this.allLocations = results;
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

    this.epaperDeviceService.getDevices()
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let epaperDevices = results[0];

        epaperDevices.forEach((epaperDevice, index, epaperDevices) => {
          (<any>epaperDevice).index = index + 1;
        });


        this.rowsCache = [...epaperDevices];
        this.rows = epaperDevices;

        
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve epaper devices from the server.`,
            MessageSeverity.error);
        });
  }


  onSearchChanged(value: string) {
    this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.device_code, r.device_label, r.mac_address, r.location_code, r.ip_address));
  }


  onEditorModalHidden() {
    this.editingEpaperDeviceCode = null;
    this.epaperDeviceEditor.resetForm(true);
  }


  newEpaperDevice() {
    this.editingEpaperDeviceCode = null;
    this.sourceEpaperDevice = null;
    this.editedEpaperDevice = this.epaperDeviceEditor.newEpaperDevice(this.allLocations);
    this.header = 'New Epaper Device';
    this.openDialog(this.editedEpaperDevice, false);
    //this.editorModal.show();
  }


  editEpaperDevice(row: EpaperDevice, isApproval?: boolean) {
    this.editingEpaperDeviceCode = { code: row.device_code };
    this.sourceEpaperDevice = row;
    this.editedEpaperDevice = this.epaperDeviceEditor.editEpaperDevice(row, this.allLocations);
    this.header = 'View Epaper Device';//row.device_status == 'Approved' ? 'View EpaperDevice' : 'Edit EpaperDevice';
    this.openDialog(this.editedEpaperDevice, isApproval);
    //this.editorModal.show();
  }

  deleteEpaperDevice(row: EpaperDevice) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.device_label + '\" epaper device?', DialogType.confirm, () => this.deleteEpaperDeviceHelper(row));
  }


  deleteEpaperDeviceHelper(row: EpaperDevice) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.epaperDeviceService.deleteEpaperDevice(row)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.rowsCache = this.rowsCache.filter(item => item !== row)
        this.rows = this.rows.filter(item => item !== row)
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the epaper device.`,
            MessageSeverity.error);
        });
  }


  get canManageEpaperDevices() {
    return this.accountService.userHasPermission(Permission.manageEpaperDevicesPermission)
  }
}
