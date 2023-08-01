import { Component, ViewChild, Input, ChangeDetectorRef, ViewEncapsulation, Inject, OnInit, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { DateAdapter, MatDialog, MatDialogRef, MAT_DATE_FORMATS, MAT_DATE_LOCALE, MAT_DIALOG_DATA } from '@angular/material';
import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { LocationService } from '../../../services/location.service';
import { ImageReference, Location } from '../../../models/location.model';
import { Permission } from '../../../models/permission.model';
import { Institution } from 'src/app/models/institution.model';
import { DirectoryListing } from 'src/app/models/directory-listing.model';
import { FacilityService } from 'src/app/services/facility.service';
import { Facility } from '../../../models/facility.model';
import { FormControl, Validators } from '@angular/forms';
import { TreeModel } from 'ng2-tree';
import { LocationTree } from 'src/app/models/location-tree.model';
import { AuthService } from 'src/app/services/auth.service';
import { FileService } from 'src/app/services/file.service';
import { ImageViewerComponent } from '../../common/image-viewer/image-viewer.component';
import { FacilityTypeService } from 'src/app/services/facility-type.service';
import { FacilityType } from 'src/app/models/facility-type.model';
import { ConfigurationService } from 'src/app/services/configuration.service';
import { ImageReferenceTypeService } from 'src/app/services/image-reference-type.service';
import { Filter } from 'src/app/models/sieve-filter.model';
import { ImageReferenceColorService } from 'src/app/services/image-reference-color.service';
import { MAT_MOMENT_DATE_FORMATS } from '@angular/material-moment-adapter';
import { MomentUtcDateAdapter } from 'src/app/helpers/moment-utc-adapter';
import { LocationAsset } from 'src/app/models/asset.model';
import { AssetService } from 'src/app/services/asset.service';
import { AssetSelectorComponent } from '../../common/asset-selector/asset-selector.component';
import { getBaseUrl } from 'src/app/app.module';


@Component({
  selector: 'location-tree-editor',
  templateUrl: './location-tree-editor.component.html',
  styleUrls: ['./location-tree-editor.component.css'],
  providers: [
    { provide: MAT_DATE_LOCALE, useValue: 'en-SG' },
    { provide: MAT_DATE_FORMATS, useValue: MAT_MOMENT_DATE_FORMATS },
    { provide: DateAdapter, useClass: MomentUtcDateAdapter },
  ]
})
export class LocationTreeEditorComponent implements OnChanges {

  private isEditMode = false;
  private isNewLocation = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingLocationName: string;
  private locationEdit: Location = new Location();
  private originalLocation: Location = new Location();
  private allInstitutions: Institution[] = [];
  private allDirs: DirectoryListing[] = [];
  private facilitiesList: Facility[] = [];
  private facilityTypesList: FacilityType[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  private imageRefSort: boolean = false;
  private assetSort: boolean = false;
  facilities = new FormControl('', [Validators.required]);
  facilityTypes = new FormControl('');
  institutions = new FormControl('', [Validators.required]);
  public fileUploadResponse: { dbPath: '', fileId: null, fileName: '' };
  public formResetToggle = true;
  public imageReferenceTypes = [];
  public imageReferenceColors = [];
  public assetTypes = [];
  public assetModels = [];
  public colorCodes = ['Red', 'Yellow', 'Blue', 'Orange', 'Green', 'Violet', 'Brown', 'Gray', 'Black', 'Pink'];
  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;

  @ViewChild('f')
  private form;

  @ViewChild('locationTypes')
  private locationTypes;

  @ViewChild('locationTypesSelector')
  private locationTypesSelector;

  @ViewChild('institutionsSelector')
  private institutionsSelector;

  @ViewChild('directorySelector')
  private directorySelector;

  @Input()
  isViewOnly: boolean;

  @Input()
  location: Location;

  @Output()
  savedLocation = new EventEmitter<Location>();

  @Output()
  cancelChanges = new EventEmitter<Location>();

  filter: Filter;

  savedCompleted(location: Location) {
    this.imageRefSort = false;
    this.savedLocation.emit(location);
  }

  cancelChangeCompleted() {
    this.cancelChanges.emit(this.location);
  }

  @Output()
  renamedLocation = new EventEmitter<string>();

  renamedLocationComplete(txt) {
    this.renamedLocation.emit(txt);
  }

  rootLocation: LocationTree;
  oopNodeController: any;

  @ViewChild('location')
  locationName: any;

  constructor(private cdRef: ChangeDetectorRef, private alertService: AlertService, private accountService: AccountService, private locationService: LocationService, private facilityService: FacilityService,
    private facilityTypeService: FacilityTypeService, public dialogRef: MatDialogRef<LocationTreeEditorComponent>, private configurationService: ConfigurationService,
    private fileService: FileService, @Inject(MAT_DIALOG_DATA) public data: any, public dialog: MatDialog, private imageTypeReferenceService: ImageReferenceTypeService, private imageReferenceColorService: ImageReferenceColorService,
    private assetService: AssetService,
    private dateAdapter: DateAdapter<Date>) {
    this.dateAdapter.setLocale('en-SG');
    this.loadFacilities();
    this.loadFacilityTypes();
    this.loadImageReferenceTypes();
    this.loadImageReferenceColors();
    this.loadAssetTypes();
    this.loadAssetModels();
  }

  ngOnChanges(): void {
    this.clearFilters();
    if (this.locationName) {

      this.locationName.nativeElement.focus();
    }
  }

  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }

  renameNode(ev) {
    console.log(ev);

    this.renamedLocationComplete(ev.target.value);
  }
  private save() {
    //check assets are filled out correctly
    if (this.locationEdit.locationAssets) {
      let exist = this.locationEdit.locationAssets.findIndex(e => !e.assetModelId || !e.serialNumber);
      if (exist > -1) {
        this.showErrorAlert('Assets Section', 'Please fill out required fields.');
        return;
      }
      
    }

    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");

    if (!this.locationEdit.id) {
      //this.locationEdit.institutionId = this.accountService.currentUser.institutionId;
      this.locationService.newLocation(this.locationEdit).subscribe(location => this.saveSuccessHelper(location), error => this.saveFailedHelper(error));
    }
    else {
      this.locationService.updateLocation(this.locationEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }

  private saveSuccessHelper(location?: Location) {
    if (location)
      Object.assign(this.locationEdit, location);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    this.savedCompleted(this.locationEdit);
    if (this.isNewLocation) {
      this.alertService.showMessage("Success", `Location \"${this.locationEdit.name}\" was created successfully`, MessageSeverity.success);
    }
    else {
      this.alertService.showMessage("Success", `Changes to location \"${this.locationEdit.name}\" was saved successfully`, MessageSeverity.success);
    }


    //this.locationEdit = new Location();
    //this.resetForm();

    if (this.changesSavedCallback)
      this.changesSavedCallback();
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
  }


  private cancel() {
    if (this.originalLocation && this.originalLocation.name == 'New') {
      this.savedCompleted(this.originalLocation);
      this.resetForm();
    }

    this.locationEdit = this.originalLocation;

    this.showValidationErrors = false;
    //

    this.alertService.resetStickyMessage();

    if (this.changesCancelledCallback)
      this.changesCancelledCallback();

    this.cancelChangeCompleted();
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

  setDisabledSelection(institution) {
    return this.rootLocation.value == institution.name;
  }

  newLocation(allInstitutions: Institution[], allDirs: DirectoryListing[], tempName?: string) {
    this.isEditMode = true;
    this.isNewLocation = true;
    this.showValidationErrors = true;

    this.editingLocationName = null;
    this.setInstitutions(allInstitutions);
    this.setDirs(allDirs);
    this.selectedValues = {};
    this.locationEdit = new Location();
    this.locationEdit.name = tempName;
    this.locationEdit.locationAssets = [];
    return this.locationEdit;
  }

  editLocation(location: Location, allInstitutions: Institution[], rootLocation: LocationTree, allDirs: DirectoryListing[]) {
    this.isEditMode = true;
    this.rootLocation = rootLocation;
    if (location) {
      this.isNewLocation = false;
      this.showValidationErrors = true;

      this.originalLocation = location;
      this.editingLocationName = location.name;
      this.selectedValues = {};
      this.setInstitutions(allInstitutions);
      this.setDirs(allDirs);
      this.locationEdit = new Location();
      console.log(location);
      Object.assign(this.locationEdit, location);
      console.log(this.locationEdit);
      this.ngOnChanges();
      return this.locationEdit;
    }
    else {
      this.ngOnChanges();
      return this.newLocation(allInstitutions,allDirs);
    }
  }

  private setInstitutions(allInstitutions: Institution[]) {

    this.allInstitutions = allInstitutions ? [...allInstitutions] : [];

    if (allInstitutions == null || this.allInstitutions.length != allInstitutions.length)
      setTimeout(() => this.institutionsSelector.refresh());
  }

  private setDirs(allDirs: DirectoryListing[]) {

    this.allDirs = allDirs ? [...allDirs] : [];

    if (allDirs == null || this.allDirs.length != allDirs.length)
      setTimeout(() => this.directorySelector.refresh());
  }

  loadFacilities() {

    this.facilityService.getFacilitiesAndFacilityTypes(null, null, this.accountService.currentUser.institutionId)
      .subscribe(results => {
        let facilities = results[0];
        this.facilitiesList = facilities;
      },
        error => {
          this.alertService.resetStickyMessage();
          this.alertService.showStickyMessage("getFacilitiesByInstitutionId failed", "An error occured while trying to get equipments from the server", MessageSeverity.error);
        });
  }

  loadFacilityTypes() {

    this.facilityTypeService.getFacilityTypeByInstitutionId(this.accountService.currentUser.institutionId)
      .subscribe(results => {
        this.facilityTypesList = results;
      },
        error => {
          this.alertService.resetStickyMessage();
          this.alertService.showStickyMessage("getFacilityTypesByInstitutionId failed", "An error occured while trying to get facility types from the server", MessageSeverity.error);
        });
  }

  loadImageReferenceTypes() {
    this.filter = new Filter(-1, -10);
    this.filter.sorts = 'Name';
    this.filter.filters = '';
    this.filter.page = 0;
    this.filter.pageSize = 0;
    this.filter.filters = '(IsActive)==true';

    this.imageTypeReferenceService.getImageReferenceTypesByFilter(this.filter)
      .subscribe(results => {
        this.imageReferenceTypes = results.pagedData;
      },
        error => {
          this.alertService.resetStickyMessage();
          this.alertService.showStickyMessage("getImageReferenceTypesByFilter failed", "An error occured while trying to get image reference types from the server", MessageSeverity.error);
        });
  }

  loadImageReferenceColors() {
    this.filter = new Filter(-1, -10);
    this.filter.sorts = 'Name';
    this.filter.filters = '';
    this.filter.page = 0;
    this.filter.pageSize = 0;
    this.filter.filters = '(IsActive)==true';

    this.imageReferenceColorService.getImageReferenceColorsByFilter(this.filter)
      .subscribe(results => {
        this.imageReferenceColors = results.pagedData;
      },
        error => {
          this.alertService.resetStickyMessage();
          this.alertService.showStickyMessage("getImageReferenceColorsByFilter failed", "An error occured while trying to get image reference colors from the server", MessageSeverity.error);
        });
  }

  loadAssetTypes() {
    this.filter = new Filter();
    this.filter.sorts = 'Name';
    this.filter.filters = '';
    this.filter.page = 0;
    this.filter.pageSize = 0;
    this.filter.filters = '(IsActive)==true,(InstitutionId)==' + this.accountService.currentUser.institutionId;

    this.assetService.getAssetTypesByFilter(this.filter)
      .subscribe(results => {
        this.assetTypes = results.pagedData;
      },
        error => {
          this.alertService.resetStickyMessage();
          this.alertService.showStickyMessage("getAssetTypesByFilter failed", "An error occured while trying to get asset types from the server", MessageSeverity.error);
        });
  }

  loadAssetModels() {
    this.filter = new Filter();
    this.filter.sorts = 'Name';
    this.filter.filters = '';
    this.filter.page = 0;
    this.filter.pageSize = 0;
    this.filter.filters = '(IsActive)==true,(InstitutionId)==' + this.accountService.currentUser.institutionId;

    this.assetService.getAssetModelsByFilter(this.filter)
      .subscribe(results => {
        this.assetModels = results.pagedData;
      },
        error => {
          this.alertService.resetStickyMessage();
          this.alertService.showStickyMessage("getAssetModelsByFilter failed", "An error occured while trying to get asset model from the server", MessageSeverity.error);
        });
  }

  changeAssetType(id, row) {
    row.assetModelId = '';
  }

  getAssetTypeName(id, row) {
    if (!this.assetModels) this.assetModels = [];
    let modelIndx = this.assetModels.findIndex(e => e.id == id);
    if (modelIndx > -1) {
      if (!this.assetTypes) this.assetTypes = [];
      let typeIndx = this.assetTypes.findIndex(e => e.id == this.assetModels[modelIndx].assetTypeId);

      return this.assetTypes[typeIndx].name;
    }

    return '';
  }

  changeInstitutions(id) {
    this.facilityService.getFacilitiesByInstitutionId(id)
      .subscribe(results => {
        let facilities = results[0];
        this.facilitiesList = facilities;
      },
        error => {
          this.alertService.resetStickyMessage();
          this.alertService.showStickyMessage("getFacilitiesByInstitutionId failed", "An error occured while trying to get equipments from the server", MessageSeverity.error);
        });
  }

  setLocationTypeId(id) {
    this.locationEdit.locationTypeId = id;
  }

  public uploadFinished = (event) => {
    this.fileUploadResponse = event;
    this.locationEdit.filePath = this.fileUploadResponse ? this.fileUploadResponse.dbPath : null;
  }

  public addImageReferenceRecord() {
    if (!this.locationEdit.imageReferences) {
      this.locationEdit.imageReferences = [];
    }

    this.locationEdit.imageReferences.splice(0, 0, new ImageReference(this.locationEdit.id, '', '', 0, null, '', new Date()));

    //this.locationEdit.imageReferences.push(new ImageReference(this.locationEdit.id, '', '', 0, null, '', new Date()));
  }

  public addImageReference = (event, ir) => {
    let filePath = event ? event.dbPath : null;
    let originalFileName = event ? event.originalFileName : null;
    //if (!this.locationEdit.imageReferences) {
    //  this.locationEdit.imageReferences = [];
    //}

    ir.filePath = filePath;
    ir.fileName = originalFileName;
    //this.locationEdit.imageReferences.push(new ImageReference(this.locationEdit.id, filePath, '', 0, null, '', new Date()));
    //this.locationEdit.imageReferences.push({
    //  locationId: this.locationEdit.id,
    //  fileId: 0,
    //  filePath: filePath,
    //  fileName: null,
    //  remarks: '',
    //});
    //console.log(this.locationEdit.imageReferences);
  }

  downloadFile(path) {
    window.open(getBaseUrl() + '/Download/FileByPath?filePath=/' + encodeURIComponent(path), '_blank');
  }

  getDomainUrl() {
    return this.configurationService.config.domainUrl;
  }

  viewImage(path, isDownload) {
    let fileType = this.getFileType(path);
    if (fileType == 'pdf') {
      this.downloadFile(path);
      //|| fileType == 'ppt' || fileType == 'pptx' || fileType == 'doc' || fileType == 'docx') {
      //window.open('https://docs.google.com/viewerng/viewer?url=' + this.getFileImage(path), '_blank');
    } else if (fileType == 'ppt' || fileType == 'pptx' || fileType == 'doc' || fileType == 'docx') {
      //path = '\\Resources\\Images\\8a6e4f0f-5a92-4fcb-a55a-927f601b107ed64d305b-4ffb-4c2f-9067-aee606b500a1pptexamples & test (2).ppt';
      if (path[0] == "\\" || path[0] == "/") {
        path = path.substring(1, path.length);
      }
      console.log(this.getDomainUrl() + "/" + path);
      let src = encodeURIComponent(this.getDomainUrl() + "/" + path);

      window.open('https://view.officeapps.live.com/op/embed.aspx?src=' + src, '_blank');
    } else {
      const dialogRef = this.dialog.open(ImageViewerComponent, {
        data: {
          fileType: fileType, //this.getFileType(path) == 'pdf',
          thumbnailFilePath: path,
          fullImageFilePath: path,
          width: '400',
          height: '600',
          lensWidth: '400',
          lensHeight: '400'
        },
        width: '800px',
        //height: '100vh'
        //maxWidth: '100vw',
      });
    }

  }

  deleteImage(path) {
    this.alertService.showDialog('Are you sure you want to delete this row?', DialogType.confirm, () => {
      let indx = this.locationEdit.imageReferences.findIndex(e => e.filePath == path);
      if (indx > -1) {
        this.locationEdit.imageReferences.splice(indx, 1);
      }
    });

  }


  getFileImage(path) {
    return this.fileService.getFile(path);
  }

  getFileType(path) {
    var ext = path.split('.').pop();
    return ext ? ext : '';
  }

  getImageThumbnail(path) {
    let fType = this.getFileType(path);
    if (fType == 'pdf') {
      return "Resources\\Default\\pdficon.png";
    } else if (fType == 'ppt' || fType == 'pptx') {
      return "Resources\\Default\\ppticon.png";
    } else if (fType == 'doc' || fType == 'docx') {
      return "Resources\\Default\\wordicon.png";
    } else {
      return this.fileService.getFile(path);
    }

  }

  getColorById(id) {
    let code = '';

    if (this.imageReferenceColors) {
      let color = this.imageReferenceColors.filter(e => e.id == id);
      if (color && color.length > 0) {
        code = color[0].colorCode;
      }
    }

    return code;
  }

  fType: any = '';
  fColor: any = '';

  clearFilters() {
    this.fType = '';
    this.fColor = '';

    if (this.locationEdit.imageReferences) {
      this.locationEdit.imageReferences.forEach((ir, index, l) => {
        ir.hidden = false;
      });
    }
  }

  applyFilters() {
    if (this.locationEdit.imageReferences) {
      this.locationEdit.imageReferences.forEach((ir, index, l) => {
        ir.hidden = false;
        if (this.fType && ir.imageReferenceTypeId != this.fType) {
          ir.hidden = true;
        }

        if (this.fColor && ir.imageReferenceColorId != this.fColor) {
          ir.hidden = true;
        }
      });

      this.locationEdit.imageReferences = [...this.locationEdit.imageReferences];
    }
  }


  sortBy(prop: string) {
    this.imageRefSort = !this.imageRefSort;
    if (this.locationEdit.imageReferences) {
      if (this.imageRefSort) {
        return this.locationEdit.imageReferences.sort((a, b) => a[prop] > b[prop] ? 1 : a[prop] === b[prop] ? 0 : -1);
      }
      return this.locationEdit.imageReferences.sort((a, b) => a[prop] < b[prop] ? 1 : a[prop] === b[prop] ? 0 : -1);
    }
  }

  addAssetFromSelector() {
    let assets = [];
    if (!this.locationEdit.locationAssets) this.locationEdit.locationAssets = [];
      this.locationEdit.locationAssets.forEach((ir, index, l) => {
        ir.assetId = ir.assetId;
        ir.locationId = this.locationEdit.id;
        assets.push(ir);
    });

    const dialogRef = this.dialog.open(AssetSelectorComponent, {
      data: { header: "Assets", assets: assets },
      disableClose: true,
      width: '800px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result.isCancel) {
        console.log(result.selectedAssets);
        let locationAssets = [];
        if (result.selectedAssets) {
          result.selectedAssets.forEach((ir, index, l) => {
            ir.assetId = ir.assetId;
            ir.locationId = this.locationEdit.id;
            ir.id = 0;
            locationAssets.push(ir);
          });
        }
        this.locationEdit.locationAssets = locationAssets;
      }
    });
  }

  public addAssetRecord() {
    if (!this.locationEdit.locationAssets) {
      this.locationEdit.locationAssets = [];
    }

    let asset = new LocationAsset();
    asset.locationId = this.locationEdit.id;
    //asset.purchaseDate = asset.warrantyStart = asset.warrantyEnd = new Date();
    this.locationEdit.locationAssets.splice(0, 0, asset);
  }

  deleteAsset(asset) {
    this.alertService.showDialog('Are you sure you want to delete this row?', DialogType.confirm, () => {
      let indx = this.locationEdit.locationAssets.findIndex(e => e.assetId == asset.id);
      if (indx > -1) {
        this.locationEdit.locationAssets.splice(indx, 1);
      } else {
        indx = this.locationEdit.locationAssets.findIndex(e => e.serialNumber == asset.serialNumber);
        if (indx > -1) {
          this.locationEdit.locationAssets.splice(indx, 1);
        }
      }
    });

  }

  get canManageLocations() {
    return this.accountService.userHasPermission(Permission.manageLocationsPermission)
  }
}
