import { Component, ViewChild, Input, Inject } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { DeviceService } from '../../../services/device.service';
import { Device } from '../../../models/device.model';
import { Permission } from '../../../models/permission.model';
import { Location } from '../../../models/location.model';
import { Institution } from '../../../models/institution.model';
import { FormControl, Validators } from '@angular/forms';
import { LocType } from 'src/app/models/enums';
import { LocationService } from 'src/app/services/location.service';
import { SignagePublicationService } from '../../../services/signage-publication.service';
import { SignagePublication } from '../../../models/signage-publication.model';
import { ModuleService } from 'src/app/services/module.service';
import { Observable, Subscription } from 'rxjs';
import { Module } from 'src/app/models/module.model';
import { ModulePath } from 'src/app/helpers/enums';
import { EmsService } from '../../../services/ems.service';
import { MediaService } from 'src/app/services/media.service';
import { Filter } from 'src/app/models/sieve-filter.model';

@Component({
  selector: 'device-editor',
  templateUrl: './device-editor.component.html',
  styleUrls: ['./device-editor.component.css']
})
export class DeviceEditorComponent {
  private subscription: Subscription = new Subscription();
  private isEditMode = false;
  private isNewDevice = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingDeviceCode: string;
  private deviceEdit: Device = new Device();
  private currentModule: Module;
  private allLocations: any[] = [];
  private allInstitutions: Institution[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public searchForm: FormControl = new FormControl();
  public locationSearchForm: FormControl = new FormControl();
  fcLocations = new FormControl('', [Validators.required]);
  fcInstitution = new FormControl('', [Validators.required]);
  fcModule = new FormControl('', []);
  fcVolume = new FormControl('', [Validators.max(100), Validators.min(0)]);
  fcVideoVolume = new FormControl('', [Validators.max(100), Validators.min(0)]);
  fcBrightness = new FormControl('', [Validators.max(10), Validators.min(0)]);

  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;

  @ViewChild('locations')
  private locations;

  @ViewChild('locationsSelector')
  private locationsSelector;

  @ViewChild('institutions')
  private institutions;

  @ViewChild('institutionsSelector')
  private institutionsSelector;

  @Input()
  isViewOnly: boolean;

  isApproval: boolean;
  allPublications: SignagePublication[];
  public modules = [];
  public deviceTypes = [];
  private filteredLocations: any[] = [];
  private display_types: any[] = [];
  emses: any[] = [];
  medias: any[] = [];
  public ress = [
    { value: '', viewValue: '' },
    { value: '1024x768', viewValue: '1024 x 768' },
    { value: '1280x720', viewValue: '1280 x 720' },
    { value: '1920x1080', viewValue: '1920 x 1080' },
    { value: '3840x2160', viewValue: '3840 x 2160' }
  ];
  rotations =  ['','Left', 'Right', 'Normal', 'Inverted'];
  //public modules = [
  //  { name: 'Meeting Room Display', path: ModulePath.MeetingRoom },
  //  { name: 'Signage Display', path: ModulePath.SignageDisplay }
  //];
  constructor(private alertService: AlertService, private accountService: AccountService, private deviceService: DeviceService, private locationService: LocationService, signagePublicationService: SignagePublicationService,
    private moduleService: ModuleService, private emsService: EmsService, private mediaService: MediaService,

    public dialogRef: MatDialogRef<DeviceEditorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.device) != typeof (undefined)) {
      this.getModules();
      this.getDeviceTypes();
      if (data.device.id) {
        this.isApproval = data.isApproval;
        this.editDevice(data.device, data.locations, data.institutions);
      } else {
        this.newDevice(data.locations, data.institutions);
      }
    }

    //this.getEmses();
    //this.fcInstitution.valueChanges.subscribe(value => {
    //  this.changeInstitutions(value);
    //});

    //signagePublicationService.getSignagePublicationByInstitutionId(this.accountService.currentUser.institutionId)
    //  .subscribe(results => {
        
    //    this.allPublications = results[0];
        
    //  },
    //    error => {
          
    //    });
  }



  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }

  //private approve() {
  //  this.isSaving = true;
  //  this.alertService.startLoadingMessage("Processing approval...");
  //  this.deviceEdit.isApproved = true;
  //  this.deviceService.updateDevice(this.deviceEdit).subscribe(device => this.saveSuccessHelper(deviceEdit), error => this.saveFailedHelper(error));
  //}

  private save(isApproval?: boolean) {
    if (isApproval || (this.form.form.valid)) {
      this.isSaving = true;
      if (isApproval) {
        this.deviceEdit.isApproved = true;
        this.alertService.startLoadingMessage("Processing approval...");
      } else {
        this.alertService.startLoadingMessage("Saving changes...");
      }

      //if (this.allLocations) {
      //  let selectedLocation = this.allLocations.filter(e => e.id == this.deviceEdit.locationId);
      //  if (selectedLocation && selectedLocation.length > 0) {
      //    this.deviceEdit.locationName = selectedLocation[0].name;
      //  }
      //}

      if (this.isNewDevice) {
        this.deviceEdit.isApproved = false;
        this.deviceService.newDevice(this.deviceEdit).subscribe(device => this.saveSuccessHelper(device), error => this.saveFailedHelper(error));
      }
      else {
        this.deviceService.updateDevice(this.deviceEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
      }
    }
  }




  private saveSuccessHelper(device?: Device) {
    if (device)
      Object.assign(this.deviceEdit, device);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewDevice)
      this.alertService.showMessage("Success", `Device \"${this.deviceEdit.code}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to device \"${this.deviceEdit.code}\" was saved successfully`, MessageSeverity.success);


    this.deviceEdit = new Device();
    this.resetForm();


    //if (!this.isNewDevice && this.accountService.currentUser.devices.some(r => r == this.editingDeviceCode))
    //    this.refreshLoggedInUser();

    if (this.changesSavedCallback)
      this.changesSavedCallback();

    this.dialogRef.close();
  }


  private refreshLoggedInUser() {
    this.accountService.refreshLoggedInUser()
      .subscribe(user => { },
        error => {
          this.alertService.resetStickyMessage();
          this.alertService.showStickyMessage("Refresh failed", "An error occured while refreshing logged in user information from the server", MessageSeverity.error);
        });
  }



  private saveFailedHelper(error: any) {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage("Save Error", "The below errors occured while saving your changes:", MessageSeverity.error);
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);

    if (this.changesFailedCallback)
      this.changesFailedCallback();

    this.dialogRef.close();
  }


  private cancel() {
    this.deviceEdit = new Device();

    this.showValidationErrors = false;
    this.resetForm();

    this.alertService.resetStickyMessage();

    if (this.changesCancelledCallback)
      this.changesCancelledCallback();

    this.dialogRef.close();
  }

  resetForm(replace = false) {

    if (!replace) {
      this.form.reset();
    }
    else {
      this.formResetToggle = false;

      setTimeout(() => {
        this.formResetToggle = true;
      });
    }
  }


  newDevice(allLocations: Location[], allInstitutions: Institution[]) {
    this.isEditMode = true;
    this.isNewDevice = true;
    this.showValidationErrors = true;

    this.editingDeviceCode = null;
    //this.allLocations = allLocations;
    //this.allInstitutions = allInstitutions;
    //this.allLocations = allLocations;
    //this.buildingList = allLocations.filter(f => f.locationTypeName == LocType.Building);
    this.allInstitutions = allInstitutions;
    this.selectedValues = {};
    this.deviceEdit = new Device();
    this.deviceEdit.startDate = new Date();
    return this.deviceEdit;
  }

  editDevice(device: Device, allLocations: Location[], allInstitutions: Institution[]) {
    this.isEditMode = true;
    //this.allLocations = allLocations;
    //this.buildingList = allLocations.filter(f => f.locationTypeName == LocType.Building);
    this.allInstitutions = allInstitutions;

    if (device) {
      this.isNewDevice = false;
      this.showValidationErrors = true;

      this.editingDeviceCode = device.code;
      this.selectedValues = {};
      //this.setLocations(device, allLocations);
      //this.setInstitutions(device, allInstitutions);
      this.deviceEdit = new Device();
      Object.assign(this.deviceEdit, device);
      return this.deviceEdit;
    }
    else {
      return this.newDevice(allLocations, allInstitutions);
    }
  }

  buildingList: Location[];
  getRoomsByBuildingId(id) {
    let unsortedLocations = this.allLocations.filter(f => f.parentLocationId == id)
    unsortedLocations.sort((a, b) => (a.name < b.name ? -1 : 1));

    return unsortedLocations;
  }
  private setLocations(device: Device, allLocations?: Location[]) {

    //this.allLocations = allLocations ? [...allLocations] : [];

    //if (allLocations == null || this.allLocations.length != allLocations.length)
    //  setTimeout(() => this.locationsSelector.refresh());


  }

  private setInstitutions(device: Device, allInstitutions?: Institution[]) {

    this.allInstitutions = allInstitutions ? [...allInstitutions] : [];

    if (allInstitutions == null || this.allInstitutions.length != allInstitutions.length)
      setTimeout(() => this.institutionsSelector.refresh());

  }

  setIsApproval(isApproval: boolean) {
    this.isApproval = isApproval;
  }

  changePublications(id) {
    this.deviceEdit.publicationId = id;
  }

  changeInstitutions(id) {
    if (id) {
      let selectedInstitution = this.allInstitutions.filter(e => e.id == id);
      if (selectedInstitution && selectedInstitution.length > 0) {
        this.deviceEdit.institutionName = selectedInstitution[0].name;
      }

      if (this.currentModule.moduleParameters) {
        let selectedModule = this.currentModule.moduleParameters.filter(e => e.name == 'LOCATION_API');
        if (selectedModule && selectedModule.length > 0) {
          let uri = selectedModule[0].url + '?institutionId=' + id;
          this.getLocations(true, uri, false, null, true);
        }
      }
      //this.locationService.getLocationsByInstitutionId(id)
      //  .subscribe(results => {
      //    this.allLocations = results;
      //    var rooms = results.filter(f => f.locationTypeName == LocType.Room);
      //    this.buildingList = [];
      //    var bList = {};
      //    rooms.forEach(function (loc, i, array) {
      //      if (loc.parentLocation != null && loc.parentLocation.locationTypeName == LocType.Building) {
      //        if (!bList.hasOwnProperty(loc.parentLocation.id)) {
      //          bList[loc.parentLocation.id] = loc.parentLocation;
      //        }
      //      }
      //    });

      //    var keys = Object.keys(bList);

      //    this.buildingList = keys.map(function (v) { return bList[v]; });
      //  },
      //    error => {
      //      this.alertService.resetStickyMessage();
      //      this.alertService.showStickyMessage("getLocationsByInstitutionId failed", "An error occured while trying to get locations from the server", MessageSeverity.error);
      //    });
    }
  }

  filterLocations() {
    this.filteredLocations = this.deviceEdit.module_path_value == ModulePath.NurseCheckinCounter ? this.allLocations.filter(e => e.ward) : this.allLocations.filter(e => !e.ward);
  }

  getLocations(isShowMessage: boolean, url?: string, isPost?: boolean, params?: any, isFRS?: boolean) {
    if (isShowMessage) {
      this.alertService.startLoadingMessage("Loading locations...");
    }
    this.subscription.add(this.deviceService.getDataByUrl(url, isPost, params)
      .subscribe(results => {
        if (!isFRS) {
          this.allLocations = results.data;
          if (this.allLocations) {
            this.filterLocations();
          }
        } else {
          this.allLocations = results;
          var rooms = results.filter(f => f.locationTypeName == LocType.Room);
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
        }

        this.alertService.stopLoadingMessage();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.`,
            MessageSeverity.error);
        }));
  }

  onModuleChange(moduleId) {
    this.allLocations = [];
    //let uri = '';
    let selectedModule = this.modules.filter(e => e.id == moduleId);
    if (selectedModule && selectedModule.length > 0) {
      this.deviceEdit.module_path_value = selectedModule[0].route;
      this.currentModule = selectedModule[0];
      //uri = selectedModule[0].uri;
      this.getDropdownValuesFromUri();
    }
  }

  getModules() {
    this.moduleService.getModules()
      .subscribe(results => {
        this.modules = results;
        this.onModuleChange(this.deviceEdit.moduleId);
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving modules.\r\n"`,
            MessageSeverity.error);
        });
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

  getDropdownValuesFromUri() {
    if (this.deviceEdit.module_path_value == ModulePath.PibDisplay
      || this.deviceEdit.module_path_value == ModulePath.NurseCheckinCounter
      || this.deviceEdit.module_path_value == ModulePath.PatientCommunicatorDisplay
      || this.deviceEdit.module_path_value == ModulePath.Epaper) {
      //this.deviceEdit.isWard = this.deviceEdit.module_path_value == "/nursecheckincounter";
      //get hospital id first
      if (this.currentModule.moduleParameters) {
        let idHospital = this.currentModule.moduleParameters.filter(e => e.name == 'HospitalCode');
        if (idHospital && idHospital.length > 0) {
          let hospitalId = idHospital[0].url;
          let selectedModule = this.currentModule.moduleParameters.filter(e => e.name == 'LOCATION_API');
          if (selectedModule && selectedModule.length > 0) {
            let uri = selectedModule[0].url;
            //this.getLocations(true, uri);
            this.getLocations(true, uri, true, { hospitalCode: hospitalId, query: { paged: false, activeOnly: true } });
          }
        }

        let displayTypes = this.currentModule.moduleParameters.filter(e => e.name == 'DISPLAY_TYPES');
        if (displayTypes && displayTypes.length > 0) {
          let uri = displayTypes[0].url;
          this.getDisplayTypes(uri);
        }

        let emses = this.currentModule.moduleParameters.filter(e => e.name == 'EMS_API');
        if (emses && emses.length > 0) {
          let uri = emses[0].url;
          this.getEmses(uri);
        }

        let medias = this.currentModule.moduleParameters.filter(e => e.name == 'MEDIA_API');
        if (medias && medias.length > 0) {
          let uri = medias[0].url;
          this.getMedias(uri);
        }
      }

      if (this.deviceEdit.module_path_value == ModulePath.PatientCommunicatorDisplay) {
        let selectedModule = this.currentModule.moduleParameters.filter(e => e.name == 'MENU_API');
        if (selectedModule && selectedModule.length > 0) {
          let uri = selectedModule[0].url;
          this.getMenus(uri);
        }
      } else if (this.deviceEdit.module_path_value == ModulePath.PibDisplay) {
        if (this.currentModule.moduleParameters) {
          let selectedModule = this.currentModule.moduleParameters.filter(e => e.name == 'TEMPLATE_API');
          if (selectedModule && selectedModule.length > 0) {
            let uri = selectedModule[0].url;
            this.getTemplates(uri);
          }
        }
      }
    } else {
      //add FRS modules
      this.medias = [];
      this.emses = [];
      let selectedModule = this.currentModule.moduleParameters.filter(e => e.name == 'INSTITUTION_API');
      if (selectedModule && selectedModule.length > 0) {
        let uri = selectedModule[0].url;
        this.getInstitutions(uri);
      }

      if(this.deviceEdit.module_path_value == ModulePath.SignageDisplay) {
        if (this.currentModule.moduleParameters) {
          let selectedModule = this.currentModule.moduleParameters.filter(e => e.name == 'SIGNAGE_PUBLICATION_API');
          if (selectedModule && selectedModule.length > 0) {
            let uri = selectedModule[0].url;
            this.getPublications(uri);
          }
        }
      }

      let emses = this.currentModule.moduleParameters.filter(e => e.name == 'EMS_API');
      if (emses && emses.length > 0) {
        let uri = emses[0].url + '?institutionId=' + this.accountService.currentUser.institutionId;
        this.getEmses(uri);
      }

      let medias = this.currentModule.moduleParameters.filter(e => e.name == 'MEDIA_API');
      if (medias && medias.length > 0) {
        let uri = medias[0].url;
        this.getMedias(uri);
      }

      //this.applicationSetting.getApplicationSettingByKey(null, 'URL_FRS_SIGNAGE_PUBLICATION_API').subscribe(result => {
      //  uri = uri + result.value;
      //  this.getSignagePublications(uri);
      //}, error => { });


    }
  }

  menus: any[] = [];
  getMenus(url?: string) {
    this.subscription.add(this.deviceService.getDataByUrl(url)
      .subscribe(results => {
        this.menus = results;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving menu records.\r\n"`,
            MessageSeverity.error);
        }));
  }

  templates: any[] = [];
  getTemplates(url?: string) {
    this.subscription.add(this.deviceService.getDataByUrl(url)
      .subscribe(results => {
        this.templates = results;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving templates.\r\n"`,
            MessageSeverity.error);
        }));
  }

  getDataByUrl(component, url?: string) {
    this.subscription.add(this.deviceService.getDataByUrl(url)
      .subscribe(results => {
        component = results;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving the data.\r\n"`,
            MessageSeverity.error);
        }));
  }

  getPublications(url?: string) {
    this.subscription.add(this.deviceService.getDataByUrl(url)
      .subscribe(results => {
        this.allPublications = results;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving the publications.\r\n"`,
            MessageSeverity.error);
        }));
  }

  getInstitutions(url?: string) {
    this.subscription.add(this.deviceService.getDataByUrl(url)
      .subscribe(results => {
        this.allInstitutions = results;
        this.changeInstitutions(this.deviceEdit.institutionId);
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving the institutions.\r\n"`,
            MessageSeverity.error);
        }));
  }

  getDisplayTypes(url) {
    this.subscription.add(this.deviceService.getDataByUrl(url)
      .subscribe(results => {
        this.display_types = results;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving display types.\r\n"`,
            MessageSeverity.error);
        }));
  }


  getEmses(url) {
    let promise: Observable<any>;
    if (url) {
      promise = this.deviceService.getDataByUrl(url);
    } else {
      promise = this.emsService.getEmses(null, null, this.accountService.currentUser.institutionId)
    }

    this.subscription.add(promise
      .subscribe(results => {
        this.emses = results;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving EMS records.\r\n"`,
            MessageSeverity.error);
        }));
  }

  getMedias(url) {
    let promise: Observable<any>;
    if (url) {
      promise = this.deviceService.getDataByUrl(url);
    } else {
      promise = this.mediaService.getMedias(null, null, this.accountService.currentUser.institutionId)
    }

    this.subscription.add(promise
      .subscribe(results => {
        this.medias = results;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving Media records.\r\n"`,
            MessageSeverity.error);
        }));
  }

  get canManageDevices() {
    return this.accountService.userHasPermission(Permission.manageDevicesPermission)
  }

  get canApproveDevices() {
    return this.accountService.userHasPermission(Permission.approveDevicesPermission)
  }
}
