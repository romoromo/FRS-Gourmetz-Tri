import { Component, OnInit, OnDestroy, TemplateRef, ViewChild, Input, AfterViewInit, Output, EventEmitter } from '@angular/core';
import { MatDatepickerInputEvent, MatDialog } from '@angular/material';
import { SignageDashboard, SignageDashboardFilter } from 'src/app/models/dashboard.model';
import { AccountService } from 'src/app/services/account.service';
import { AlertService, DialogType, MessageSeverity } from 'src/app/services/alert.service';
import { AppTranslationService } from 'src/app/services/app-translation.service';
import { AuthService } from 'src/app/services/auth.service';
import { ConfigurationService } from 'src/app/services/configuration.service';
import { DashboardService } from 'src/app/services/dashboard.service';
import { LocationService } from 'src/app/services/location.service';
import { Location } from 'src/app/models/location.model';
import { Filter } from 'src/app/models/sieve-filter.model';
import { LocType } from 'src/app/models/enums';
import { SignagePublication } from 'src/app/models/signage-publication.model';
import { SignagePublicationService } from 'src/app/services/signage-publication.service';
import { Device, DevicePushMessage } from 'src/app/models/device.model';
import { DeviceService } from 'src/app/services/device.service';
import { Utilities } from 'src/app/services/utilities';
import { FormControl } from '@angular/forms';
import { pairwise, startWith } from 'rxjs/operators';
import { ApplicationSettingService } from 'src/app/services/application-setting.service';
import { DomSanitizer } from '@angular/platform-browser';
import { DevicePushMessageDialog } from '../push-message-dialog/push-message-dialog.component';
import { setTime } from 'ngx-bootstrap/chronos/utils/date-setters';
import { Subscription } from 'rxjs';
import { Permission } from 'src/app/models/permission.model';
import { ChannelInfoService } from '../../../../services/channel-info.service';

@Component({
  selector: 'signage-dashboard-container',
  templateUrl: './signage-dashboard-container.component.html',
  styleUrls: ['./signage-dashboard-container.component.css']
})
export class SignageDashboardContainerComponent implements OnInit, AfterViewInit, OnDestroy {
  private subscription: Subscription = new Subscription();
  loadingIndicator: boolean;
  show_no_results = true;
  private locations: Location[] = [];
  private rooms: Location[] = [];
  public dashboard: SignageDashboard;
  private signalRCoreconnection: signalR.HubConnection;
  public sgnFilter: SignageDashboardFilter;
  private keyword: string;
  private location_id: string;
  private status: string;
  locationIds: string[];
  allPublications: SignagePublication[];
  pubControl = new FormControl();
  scheme: string = 'http';
  port: string = '80';
  public ress = [
    { value: '', viewValue: '' },
    { value: '1024x768', viewValue: '1024 x 768' },
    { value: '1280x720', viewValue: '1280 x 720' },
    { value: '1920x1080', viewValue: '1920 x 1080' },
    { value: '3840x2160', viewValue: '3840 x 2160' }
  ];
  rotations = ['', 'Left', 'Right', 'Normal', 'Inverted'];
  public deviceTypes = [];
  public searchForm: FormControl = new FormControl();
  @Output()
  dateChange: EventEmitter<MatDatepickerInputEvent<any>>;
  deviceTypeId: string;
    allChannelInfos: [];
    channelId: any;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService, private dashboardService: DashboardService,
    private authService: AuthService, private configurations: ConfigurationService, private locationService: LocationService,
    private signagePublicationService: SignagePublicationService, private channelInfoService: ChannelInfoService, private deviceService: DeviceService, private applicationSetting: ApplicationSettingService,
    private domSanitizer: DomSanitizer, public dialog: MatDialog  ) {

    //this.applicationSetting.getApplicationSettingByKey(null, 'DEVICE_ADMIN_SCHEME').subscribe(result => {
    //  this.scheme = result.value;
    //}, error => { });

    //this.applicationSetting.getApplicationSettingByKey(null, 'DEVICE_ADMIN_PORT').subscribe(result => {
    //  this.port = result.value;
    //}, error => { });
  }
    ngOnDestroy(): void {
      if (this.subscription) {
        this.subscription.unsubscribe();
      }
    }

  ngOnInit() {
    console.log('On INIT');
    this.getFilters();
    this.initializeSignalR();
    this.applyFilters();
    this.getPublications();

    this.getChannelInfos();
  }

  ngAfterViewInit() {

  }

  initFilter() {
    if (!this.sgnFilter) {
      this.sgnFilter = new SignageDashboardFilter();
      this.sgnFilter.filter = new Filter();
    }
    
    this.sgnFilter.filter.filters = '(IsActive)==true,(moduleRoute)==/signagedisplay';
    this.sgnFilter.filter.page = 1;
    if (!this.sgnFilter.filter.pageSize || this.sgnFilter.filter.pageSize < 1) {
      this.sgnFilter.filter.pageSize = 10;
    }
    
  }

  addFilters() {
    if (this.keyword) {
      this.sgnFilter.filter.filters = this.sgnFilter.filter.filters +
        ',(Code | MacAddress | SerialNumber | IpAddress | locationName | device_label)@=' + this.keyword;
    }

    this.locationIds = [];

    if (this.location_id || this.location_id == "0") {
      if (this.location_id == "0") {
        //All locations
        this.locationIds = this.rooms.map(function (v) { return v.id; });
      } else {
        this.locationIds = this.getRoomsByBuildingId(this.location_id).map(function (v) { return v.id; });

        if (!this.locationIds || this.locationIds.length == 0) {
          this.locationIds = [];
          this.locationIds.push(this.location_id);
        }
      } 

      //let locFilter = ',(FindDeviceWithLocationIdIncludeParent)==' + this.locationIds.join('|');
      let locFilter = ',(LocationId)==' + this.locationIds.join('|');
      this.sgnFilter.filter.filters = this.sgnFilter.filter.filters + locFilter;
     
    } else {
      this.sgnFilter.filter.filters = this.sgnFilter.filter.filters +
        ',(LocationId)==-1';
    }

    let deviceTypeIds = [];

    if (this.deviceTypeId || this.deviceTypeId == "0") {
      if (this.deviceTypeId == "0") {
        //All device types
        deviceTypeIds = this.deviceTypes.map(function (v) { return v.id; });
      } else {
        deviceTypeIds.push(this.deviceTypeId);
      } 
      let deviceTypeFilter = ',(DeviceTypeId)==' + deviceTypeIds.join('|');
      this.sgnFilter.filter.filters = this.sgnFilter.filter.filters + deviceTypeFilter;
    } 

    this.sgnFilter.status = this.status;
    //this.sgnFilter.institutionId = this.accountService.currentUser.institutionId;
  }

  clearFilters() {
    
    this.initFilter();
    this.location_id = '';
    this.keyword = '';
    this.status = '';
    this.deviceTypeId = '';
    this.addFilters();
    this.loadData();
  }

  applyFilters(obj?: any) {
    this.initFilter();

    if (obj) {
      this.sgnFilter.filter.page = obj.pageIndex + 1;
      this.sgnFilter.filter.pageSize = obj.pageSize;
    }
    
    this.addFilters();
    this.loadData();
  }

  openPushMessageDialog() {
    const dialogRef = this.dialog.open(DevicePushMessageDialog, {
      data: { header: "Bulk Push Messages", sgnFilter: this.sgnFilter },
      width: '400px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result && result.isSuccess) {
        this.applyFilters();
      }
    });
  }

  bulkReboot() {
    this.alertService.showDialog('You are about to reboot the devices on the list. Do you wish to proceed?', DialogType.confirm, () => this.bulkRebootDeviceHelper());
  }

  bulkRebootDeviceHelper() {
    this.alertService.startLoadingMessage("Rebooting devices...");
    this.deviceService.bulkReboot(this.sgnFilter)
      .subscribe(results => {
        console.log(results);
        this.alertService.stopLoadingMessage();
        if (results.isSuccess) {
          this.alertService.showMessage("Successfully rebooted all devices!", "", MessageSeverity.success);
        } else {
          this.alertService.showMessage(results.message, "", MessageSeverity.error);
        }
      },
        error => {
          this.alertService.stopLoadingMessage();

          this.alertService.showStickyMessage("Reboot Error", `An error occured while rebooting the devices.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  bulkRefresh() {
    this.alertService.showDialog('You are about to refresh the display of the devices on the list. Do you wish to proceed?', DialogType.confirm, () => this.bulkRefreshDeviceHelper());
  }

  bulkRefreshDeviceHelper() {
    this.alertService.startLoadingMessage("Refreshing display...");
    this.deviceService.bulkRefresh(this.sgnFilter)
      .subscribe(results => {
        console.log(results);
        this.alertService.stopLoadingMessage();
        if (results.isSuccess) {
          this.alertService.showMessage("Successfully refreshed all devices!", "", MessageSeverity.success);
        } else {
          this.alertService.showMessage(results.message, "", MessageSeverity.error);
        }
      },
        error => {
          this.alertService.stopLoadingMessage();

          this.alertService.showStickyMessage("Refresh Error", `An error occured while refreshing the devices.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }
  initializeSignalR() {
    this.signalRCoreconnection = this.authService.signalRConnection(this.configurations.baseUrl + "/hub/frs", true, this.signalRCoreconnection);
    if (this.signalRCoreconnection != null) {
      this.signalRCoreconnection.on("RefreshPIBDeviceList", (deviceVM) => {
        console.log('Refreshing Device List');
        this.applyFilters();

      });
    }
  }

  buildingList: Location[];
  getRoomsByBuildingId(id) {
    let unsortedLocations = this.locations.filter(f => f.parentLocationId == id)
    unsortedLocations.sort((a, b) => (a.name < b.name ? -1 : 1));

    return unsortedLocations;
  }

  getFilters() {
    //get locations
    
    this.subscription.add(this.locationService.getLocationsByUser(this.accountService.currentUser.id, false)
      .subscribe(results => {
        this.locations = results;
        console.log('getFilters');
        var rooms = results.filter(f => f.locationTypeName == LocType.Room);//.filter(f => f.locationTypeName == LocType.Room);
        this.rooms = rooms;
        this.buildingList = [];
        var bList = {};
        rooms.forEach(function (loc, i, array) {
          if (loc.parentLocation != null && loc.parentLocation.locationTypeName == LocType.Building) {
            if (!bList.hasOwnProperty(loc.parentLocation.id)) {
              bList[loc.parentLocation.id] = loc.parentLocation;
            }
          }
        });

        var keys = Object.keys(bList);
        this.buildingList = keys.map(function (v) { return bList[v]; });
      },
        error => {
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.`,
            MessageSeverity.error);
        }));

    this.getDeviceTypes();
  }

  loadData() {
    this.subscription.add(this.dashboardService.getSignageDashboard(this.sgnFilter)
      .subscribe(results => {
        this.show_no_results = false;
        this.dashboard = results;
        if (!this.dashboard || !this.dashboard.pagedDevices || this.dashboard.pagedDevices.totalCount == 0) {
          this.show_no_results = true;
        } else {
          this.dashboard.pagedDevices.pagedData.forEach((device, ind, devices) => {
            device.assignedPublicationId = device.publicationId;
          });
        }
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving the report.`,
          //  MessageSeverity.error);
        }));

  }

  getDeviceTypes() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true,(InstitutionId)==' + this.accountService.currentUser.institutionId;
    this.deviceService.getDeviceTypesByFilter(filter)
      .subscribe(results => {
        this.deviceTypes = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving device types.\r\n"`,
            MessageSeverity.error);
        });
  }

  getPublications() {
    this.subscription.add(this.signagePublicationService.getSignagePublications(null, null)
      .subscribe(results => {
        this.allPublications = results[0];
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving the publications.\r\n"`,
            MessageSeverity.error);
        }));
  }

  getChannelInfos() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.subscription.add(this.channelInfoService.getChannelInfosByFilter(filter)
      .subscribe(results => {
        this.allChannelInfos = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving the publications.\r\n"`,
            MessageSeverity.error);
        }));
  }

  changePublication(row: any) {
    if (row.assignedPublicationId != row.publicationId) {
      this.alertService.showDialog('You are about to change the publication of \"' + row.device_label + '\". Do you wish to proceed?', DialogType.confirm, () => this.updatePublicationHelper(row),
        () => { row.assignedPublicationId = row.publicationId });
    }
  }

  updatePublicationHelper(row: any) {
    row.publicationId = row.assignedPublicationId;
    this.alertService.startLoadingMessage("Updating...");
    this.loadingIndicator = true;

    this.subscription.add(this.deviceService.updateDevice(row)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.alertService.showMessage("Successfully changed!", "", MessageSeverity.success);
        this.loadingIndicator = false;
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Update Error", `An error occured while updating the device.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }

  reboot(row: any) {
    this.alertService.showDialog('You are about to reboot \"' + row.device_label + '\". Do you wish to proceed?', DialogType.confirm, () => this.rebootDeviceHelper(row));
  }


  rebootDeviceHelper(row: any) {

    this.alertService.startLoadingMessage("Rebooting...");
    this.loadingIndicator = true;

    this.subscription.add(this.deviceService.reboot(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;
        this.alertService.showMessage("Success", `\"${row.device_label}\" was rebooted`, MessageSeverity.success);
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Error", `An error occured while trying to reboot the device.`,
            MessageSeverity.error);
        }));
  }

  channelup(row: any) {
    this.alertService.showDialog('You are about to do channel up \"' + row.device_label + '\". Do you wish to proceed?', DialogType.confirm, () => this.channelupDeviceHelper(row));
  }


  channelupDeviceHelper(row: any) {

    this.alertService.startLoadingMessage("Channel up...");
    this.loadingIndicator = true;

    this.subscription.add(this.deviceService.channelup(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;
        this.alertService.showMessage("Success", `\"${row.device_label}\" was channel up`, MessageSeverity.success);
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Error", `An error occured while trying to channel up the device.`,
            MessageSeverity.error);
        }));
  }

  changechannel(row: any) {
    if (!row.channelId) return;
    this.alertService.showDialog('You are about to change the channel \"' + row.device_label + '\". Do you wish to proceed?', DialogType.confirm, () => this.changechannelDeviceHelper(row));
  }


  changechannelDeviceHelper(row: any) {

    this.alertService.startLoadingMessage("Changing the channel...");
    this.loadingIndicator = true;

    this.subscription.add(this.deviceService.changechannel(row.id, row.channelId)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;
        this.alertService.showMessage("Success", `\"${row.device_label}\" channel was changed`, MessageSeverity.success);
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Error", `An error occured while trying to change the channel.`,
            MessageSeverity.error);
        }));
  }

  channeldown(row: any) {
    this.alertService.showDialog('You are about to do channel down \"' + row.device_label + '\". Do you wish to proceed?', DialogType.confirm, () => this.channeldownDeviceHelper(row));
  }


  channeldownDeviceHelper(row: any) {

    this.alertService.startLoadingMessage("Channel down...");
    this.loadingIndicator = true;

    this.subscription.add(this.deviceService.channeldown(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;
        this.alertService.showMessage("Success", `\"${row.device_label}\" was channel down`, MessageSeverity.success);
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Error", `An error occured while trying to channel down of the device.`,
            MessageSeverity.error);
        }));
  }

  screenon(row: any) {
    this.alertService.showDialog('You are about to do screen on \"' + row.device_label + '\". Do you wish to proceed?', DialogType.confirm, () => this.screenonDeviceHelper(row));
  }


  screenonDeviceHelper(row: any) {

    this.alertService.startLoadingMessage("Screen on...");
    this.loadingIndicator = true;

    this.subscription.add(this.deviceService.screenon(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;
        this.alertService.showMessage("Success", `\"${row.device_label}\" was screen on`, MessageSeverity.success);
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Error", `An error occured while trying to screen on of the device.`,
            MessageSeverity.error);
        }));
  }

  screenoff(row: any) {
    this.alertService.showDialog('You are about to do screen off \"' + row.device_label + '\". Do you wish to proceed?', DialogType.confirm, () => this.screenoffDeviceHelper(row));
  }


  screenoffDeviceHelper(row: any) {

    this.alertService.startLoadingMessage("Screen off...");
    this.loadingIndicator = true;

    this.subscription.add(this.deviceService.screenoff(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;
        this.alertService.showMessage("Success", `\"${row.device_label}\" was screen off`, MessageSeverity.success);
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Error", `An error occured while trying to screen off of the device.`,
            MessageSeverity.error);
        }));
  }

  refresh(row: any) {
    this.alertService.showDialog('You are about to refresh \"' + row.device_label + '\". Do you wish to proceed?', DialogType.confirm, () => this.refreshDeviceHelper(row));
  }


  refreshDeviceHelper(row: any) {

    this.alertService.startLoadingMessage("Refreshing...");
    this.loadingIndicator = true;

    this.subscription.add(this.deviceService.refresh(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;
        this.alertService.showMessage("Success", `\"${row.device_label}\" was refreshed`, MessageSeverity.success);
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Error", `An error occured while trying to refresh the device.`,
            MessageSeverity.error);
        }));
  }

  pushMessages(row: any) {
    row.publicationId = row.assignedPublicationId;
    this.alertService.startLoadingMessage("Pushing messages...");
    this.loadingIndicator = true;

    this.subscription.add(this.deviceService.updateDevice(row)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.alertService.showMessage("Pushed!", "", MessageSeverity.success);
        this.loadingIndicator = false;
        this.applyFilters();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Update Error", `An error occured while pushing the messages.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }

  editDevice(device) {
    device.isEdit = true;
    if (!device.orig) {
      //device.orig = new Device();
      device.orig = { rotation: device.rotation, resolution: device.resolution, brightness: device.brightness, volume: device.volume };
    }
  }

  cancelEditDevice(device) {
    device.isEdit = false;
    if (device.orig) {
      device.rotation = device.orig.rotation;
      device.resolution = device.orig.resolution;
      device.brightness = device.orig.brightness;
      device.volume = device.orig.volume;
    }
  }

  saveDevice(row) {
    this.alertService.startLoadingMessage("Updating...");
    this.loadingIndicator = true;
    this.subscription.add(this.deviceService.updateDevice(row)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.alertService.showMessage("Successfully changed!", "", MessageSeverity.success);
        this.loadingIndicator = false;
        row.isEdit = false;
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Update Error", `An error occured while updating the device.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }

  sanitize(row) {
    //let url = (row.scheme ? row.scheme : 'http') + '://' + row.ipAddress + ':' + (row.port ? row.port : '80') + '/admin';
    let url = (row.scheme ? row.scheme : 'http') + '://' + row.ipAddress;
    if (row.port) {
      url = url + ':' + row.port;
    }

    url = url + '/admin';

    return this.domSanitizer.bypassSecurityTrustUrl(url);
  }

  screenshot(row) {
    //let url = (row.scheme ? row.scheme : 'http') + '://' + row.ipAddress + ':' + (row.port ? row.port : '80') + '/admin';
    let url = (row.scheme ? row.scheme : 'http') + '://' + row.ipAddress;
    if (row.port) {
      url = url + ':' + row.port;
    }

    url = url + '/admin/download/SSimg';

    return this.domSanitizer.bypassSecurityTrustUrl(url);
  }

  get canPushMessage() {
    return this.accountService.userHasPermission(Permission.pushmessageSignageControlPermission);
  }

  get canPreview() {
    return this.accountService.userHasPermission(Permission.previewSignageControlPermission); 
  }

  get canReboot() {
    return this.accountService.userHasPermission(Permission.rebootSignageControlPermission);
  }

  get canRefresh() {
    return this.accountService.userHasPermission(Permission.refreshSignageControlPermission);
  }

  get canScreenshot() {
    return this.accountService.userHasPermission(Permission.screenshotSignageControlPermission);
  }

  get canChangePublication() {
    return this.accountService.userHasPermission(Permission.publicationSignageControlPermission);
  }

  get canChangeDeviceInformation() {
    return this.accountService.userHasPermission(Permission.deviceinfoSignageControlPermission);
  }

}


