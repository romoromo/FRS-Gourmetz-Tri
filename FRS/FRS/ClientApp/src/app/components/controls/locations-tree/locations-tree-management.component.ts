import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, Inject, ViewEncapsulation, AfterViewChecked, AfterContentChecked, ChangeDetectorRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { LocationService } from '../../../services/location.service';
import { DirectoryListingService } from '../../../services/directory-listing.service';
import { Utilities } from '../../../services/utilities';
import { Location, LocationTreeFilter, LocationType } from '../../../models/location.model';
import { Permission } from '../../../models/permission.model';
import { LocationTreeEditorComponent } from "./location-tree-editor.component";
import { Institution } from 'src/app/models/institution.model';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { TreeModel, NodeMenuItemAction, NodeEvent, Ng2TreeSettings, Tree } from 'ng2-tree';
import { MatDividerModule } from '@angular/material/divider';
import { Observable } from 'rxjs';
import { LocationTree } from 'src/app/models/location-tree.model';
import { AuthService } from 'src/app/services/auth.service';
import { LocType } from 'src/app/models/enums';
import { InstitutionService } from 'src/app/services/institution.service';
import { HttpEvent, HttpEventType } from '@angular/common/http';
import { forEach } from '@angular/router/src/utils/collection';
import { Filter } from 'src/app/models/sieve-filter.model';
import { ImageReferenceColorService } from 'src/app/services/image-reference-color.service';
import { DirectoryListing } from '../../../models/directory-listing.model';
export interface DialogData {
  header: string;
  location: Location;
  institutions: Institution[];
}

@Component({
  selector: 'locations-tree-management',
  templateUrl: './locations-tree-management.component.html',
  styleUrls: ['./locations-tree-management.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class LocationsTreeManagementComponent implements OnInit, AfterViewInit, AfterViewChecked, AfterContentChecked {
  header: string;
  columns: any[] = [];
  rows: Location[] = [];
  rowsCache: Location[] = [];
  public imageReferenceColors = [];
  colourCodefilter: Filter;
  filter: LocationTreeFilter;
  allInstitutions: Institution[] = [];
  allFilterInstitutions: Institution[] = [];
  editedLocation: Location;
  sourceLocation: Location;
  selectedNode: Location;
  editingLocationName: { name: string };
  loadingIndicator: boolean;
  institutionId: any = null;
  currentNodeId = '';
  locationTypes: LocationType[] = [];
  isTreeLoaded: boolean = false;
  tree: TreeModel;
  originalTree: LocationTree;
  rootLocation: LocationTree;
  treeSettings: Ng2TreeSettings = {
    rootIsVisible: false
  };

  treeKeyword: string;
  public progress: number;
  public message: string;
  public filename: string;
  fileTypes: string;
  showTxt: boolean;

  private allDirs: DirectoryListing[] = [];

  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('locationEditor')
  locationEditor: LocationTreeEditorComponent;

  @ViewChild('treeComponent') treeComponent: TreeModel;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private locationService: LocationService, private direcoryListingService: DirectoryListingService, private authService: AuthService, public dialog: MatDialog, private institutionService: InstitutionService,
    private changeDetector: ChangeDetectorRef, private imageReferenceColorService: ImageReferenceColorService) {
  }

  ngAfterContentChecked(): void {
    //console.log('ngAfterContentChecked');
    //if (this.tree.children && this.tree.children.length > 0) {
    //  var oopNodeController = this.treeComponent.getControllerByNodeId(this.tree.children[0].id);
    //  console.log(oopNodeController);
    //  if (oopNodeController) oopNodeController.collapse();
    //}
  }

  ngAfterViewChecked(): void {
    //console.log('ngAfterViewChecked');
    //this.changeDetector.detectChanges();

    if (this.tree && this.tree.children && this.tree.children.length > 0 && !this.isTreeLoaded) {
      var oopNodeController = this.treeComponent.getControllerByNodeId(this.tree.children[0].id);
      //console.log(oopNodeController);
      if (oopNodeController) {
        //oopNodeController.collapse();
        this.isTreeLoaded = true;
        if (!this.treeKeyword && this.tree.children[0].children && this.tree.children[0].children.length > 0) {
          this.collapseAllParents(this.tree.children[0].children);
        }

      }
    }
  }

  collapseAllParents(children) {
    for (let c of children) {
      if (c.children && c.children.length > 0) {
        this.collapseAllParents(c.children);
      }
      var oopNodeController = this.treeComponent.getControllerByNodeId(c.id);
      //console.log(oopNodeController);
      if (oopNodeController) {
        oopNodeController.collapse();
      }

    }
  }

  searchTree() {
    if (this.treeKeyword) {
      this.filterTree();
    } else {
      this.loadTree(this.institutionId);
    }
  }

  filterTree() {
    let treeResult = new LocationTree(0, '', '', []);
    for (let c of this.originalTree.children) {
      let p = Object.assign({}, c);
      if (c.children.length > 0) {
        p.children = [];
        this.getFilteredNodes(c.children, p, this.treeKeyword);
      }

      if (p.children && p.children.length > 0) {
        treeResult.children.push(p);
      }
    }

    this.tree = treeResult;
    //console.log(treeResult);
    this.tree.settings = {
      'static': true,
      'rightMenu': true,
      //'isCollapsedOnInit': true,
      'leftMenu': true,
      'cssClasses': {
        'expanded': 'fa fa-caret-down fa-sm',
        'collapsed': 'fa fa-caret-right fa-sm',
        'leaf': 'fa fa-sm',
        'empty': 'fa fa-caret-right disabled'
      },
      'templates': {
        'node': '<i class="fa fa-bank fa-sm"></i> ',
        'leaf': '<i class="fa fa-building-o fa-sm"></i> ',
        'leftMenu': '<i class="fa fa-navicon fa-sm"></i> '
      }
    };

    if (this.changeDetector) {
      this.changeDetector.detectChanges();
    }

    //if (this.tree.children && this.tree.children.length > 0) {
    //  this.collapseAllParents(this.tree.children[0].children);
    //}
  }

  getFilteredNodes(children: LocationTree[], parent: LocationTree, keyword: string) {
    //let nodes: LocationTree[] = [];
    for (let c of children) {
      if (c.children && c.children.length > 0) {
        let p = Object.assign({}, c);
        p.children = [];
        let n = this.getFilteredNodes(c.children, p, keyword);
        p = n;
        if (p.children.length > 0) {
          if (parent.children.findIndex(f => f.value == c.value) < 0) {
            parent.children.push(p);
          }
        }

      }
      if (c.value && c.value.toLowerCase().indexOf(keyword.toLowerCase()) !== -1) {
        if (parent.children.findIndex(f => f.value == c.value) < 0) {
          parent.children.push(c);
        }

      }
    }

    return parent;
  }

  openDialog(location: Location): void {
    const dialogRef = this.dialog.open(LocationTreeEditorComponent, {
      data: { header: this.header, location: location, institutions: this.allInstitutions, allDirs: this.allDirs }
    });

    dialogRef.afterClosed().subscribe(result => {

    });
  }


  ngOnInit() {
    this.loadInstitutions();
    this.loadImageReferenceColors();
    this.institutionId = this.accountService.currentUser.institutionId;
    this.filter = new LocationTreeFilter();
    this.filter.institutionId = this.accountService.currentUser.institutionId;
    this.filter.currentUserInstitutionId = this.accountService.currentUser.institutionId;
    if (this.institutionId) {
      this.loadTree(this.institutionId);
    }
    this.loadDirs();
    this.loadData();
  }

  loadImageReferenceColors() {
    this.colourCodefilter = new Filter(-1, -10);
    this.colourCodefilter.sorts = 'Name';
    this.colourCodefilter.filters = '';
    this.colourCodefilter.page = 0;
    this.colourCodefilter.pageSize = 0;
    this.colourCodefilter.filters = '(IsActive)==true';

    this.imageReferenceColorService.getImageReferenceColorsByFilter(this.colourCodefilter)
      .subscribe(results => {
        this.imageReferenceColors = results.pagedData;
      },
        error => {
          this.alertService.resetStickyMessage();
          this.alertService.showStickyMessage("getImageReferenceColorsByFilter failed", "An error occured while trying to get image reference colors from the server", MessageSeverity.error);
        });
  }

  loadInstitutions() {
    this.institutionService.getInstitutions()
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let institutions = results[0];
        this.allFilterInstitutions = institutions;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve institutions from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  loadDirs() {
    this.direcoryListingService.getDirectoryListings()
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let dirs = results[0];
        this.allDirs = dirs;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve directorys from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  changeInstitutions(event) {
    if (event && event.source.value) {
      this.institutionId = event.source.value
      this.loadTree(this.institutionId);
    }
  }


  loadTree(id, setOrigOnly?: boolean, filter?: LocationTreeFilter) {
    if (!setOrigOnly) {
      this.tree = null;
      this.alertService.startLoadingMessage();
      this.loadingIndicator = true;
      this.currentNode = null;
    }
    if (!filter) {
      this.filter = new LocationTreeFilter();
      this.filter.institutionId = id ? id : this.accountService.currentUser.institutionId;
      this.filter.currentUserInstitutionId = this.filter.institutionId;
    }

    this.locationService.getLocationTree(this.filter)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        //Object.assign(this.tree, results);

        //this.tree.children = [];
        //results.children.forEach((t, index, trees) => {
        //  var tm: TreeModel = t;
        //  if (t.type == "Hospital") {
        //    tm.settings = {
        //      'cssClasses': {
        //        'expanded': 'fa fa-caret-down fa-sm',
        //        'collapsed': 'fa fa-caret-right fa-sm',
        //        'leaf': 'fa fa-sm',
        //        'empty': 'fa fa-caret-right disabled'
        //      },
        //      'templates': {
        //        'node': '<i class="fa fa-bank fa-sm"></i>',
        //        'leaf': '<i class="fa fa-building-o fa-sm" ></i>',
        //        'leftMenu': '<i class="fa fa-navicon fa-sm" ></i>'
        //      }
        //    }
        //  } else if (t.type == "Building") {
        //    tm.settings = {
        //      'cssClasses': {
        //        'expanded': 'fa fa-caret-down fa-sm',
        //        'collapsed': 'fa fa-caret-right fa-sm',
        //        'leaf': 'fa fa-sm',
        //        'empty': 'fa fa-caret-right disabled'
        //      },
        //      'templates': {
        //        'node': '<i class=""fa fa-building-o fa-sm""></i>',
        //        'leaf': '<i class=""fa fa-building-o fa-sm"" ></i>',
        //        'leftMenu': '<i class=""fa fa-navicon fa-sm"" ></i>'
        //      }
        //    }
        //  }
        //  else {
        //    tm.settings = {
        //      'cssClasses': {
        //        'expanded': 'fa fa-caret-down fa-sm',
        //        'collapsed': 'fa fa-caret-right fa-sm',
        //        'leaf': 'fa fa-sm',
        //        'empty': 'fa fa-caret-right disabled'
        //      },
        //      'templates': {
        //        'leaf': '<i class=""fa fa-hospital-o fa-sm"" ></i>',
        //        'leftMenu': '<i class=""fa fa-navicon fa-sm"" ></i>'
        //      }
        //    }
        //  }

        //  this.tree.children.push(tm);
        //});

        if (results && results.children) {
          this.originalTree = results;
          if (!setOrigOnly) {
            this.rootLocation = results.children[0];
            this.originalTree = results;
            this.tree = results;
            this.tree.settings = {
              'static': true,
              'rightMenu': true,
              //'isCollapsedOnInit': true,
              'leftMenu': true,
              'cssClasses': {
                'expanded': 'fa fa-caret-down fa-sm',
                'collapsed': 'fa fa-caret-right fa-sm',
                'leaf': 'fa fa-sm',
                'empty': 'fa fa-caret-right disabled'
              },
              'templates': {
                'node': '<i class="fa fa-bank fa-sm"></i> ',
                'leaf': '<i class="fa fa-building-o fa-sm"></i> ',
                'leftMenu': '<i class="fa fa-navicon fa-sm"></i> '
              }
            };

            if (this.changeDetector) {
              this.changeDetector.detectChanges();
            }

            if (this.tree.children && this.tree.children.length > 0) {
              this.collapseAllParents(this.tree.children[0].children);
            }
          }
        }
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve locations from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  currentNode: TreeModel;
  addNode() {
    if (this.currentNode) {
      var oopNodeController = this.treeComponent.getControllerByNodeId(this.currentNode.id);
      let uniqueId = Date.now().toString(36) + Math.random().toString(36).substring(2);
      let newNode: TreeModel = {
        value: 'New',
        id: uniqueId
      };

      if (this.currentNode.node.type == 'Hospital') {
        newNode.children = [];
      }

      this.currentNodeId = uniqueId;
      oopNodeController.addChild(newNode);
      //oopNodeController.tree.markAsBeingRenamed();
      var locInstitutionId = this.authService.currentUser.institutionId ? this.authService.currentUser.institutionId : this.institutionId;
      var location = new Location();
      location.name = newNode.value.toString();
      location.institutionId = locInstitutionId;
      location.capacity = 0;
      location.institutionIds = [];
      location.institutionIds.push(parseInt(locInstitutionId));

      if (this.currentNode.node.type != LocType.Room) {
        location.parentLocationId = this.currentNode.node.id;
        location.locationTypeName = this.currentNode.node.type == LocType.Building ? LocType.Room : LocType.Building;//this.currentNode.node.type == LocType.Hospital ? LocType.Building : LocType.Hospital;
      } else {
        location.parentLocationId = this.currentNode.node.parentId;
        location.locationTypeName = LocType.Room;
      }


      location.locationTypeId = this.getLocationTypeByName(location.locationTypeName);
      this.locationEditor.editLocation(location, this.allInstitutions, this.rootLocation, this.allDirs);
    }
  }

  renameNode(newName) {
    if (this.currentNode) {
      var oopNodeController = this.treeComponent.getControllerByNodeId(this.currentNode.id);
      oopNodeController.rename(newName);
    }
  }

  removeNode() {
    if (this.currentNode) {
      this.alertService.showDialog('Are you sure you want to delete the \"' + this.currentNode.value + '\" ?', DialogType.confirm, () => {
        this.alertService.startLoadingMessage("Deleting...");
        this.loadingIndicator = true;

        this.locationService.deleteLocation(this.currentNode.id.toString())
          .subscribe(results => {
            this.alertService.stopLoadingMessage();
            this.loadingIndicator = false;

            var oopNodeController = this.treeComponent.getControllerByNodeId(this.currentNode.id);
            oopNodeController.remove();
            this.loadTree(this.institutionId, true);
            this.currentNode = null;
          },
            error => {
              this.alertService.stopLoadingMessage();
              this.loadingIndicator = false;

              this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the menu.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
                MessageSeverity.error);
            });

      });
    }
  }

  handleSelected(event, isManual?: boolean) {
    console.log('selected');
    var oopNodeController = this.treeComponent.getControllerByNodeId(event.node.id);
    this.currentNode = event.node;
    if (isManual) {
      this.currentNodeId = event.node.id;
    } else {
      this.currentNodeId = event.node.node.id;
    }

    this.locationService.getLocationById(this.currentNodeId).subscribe(location => {
      this.alertService.stopLoadingMessage();
      this.loadingIndicator = false;
      this.selectedNode = location;
      this.locationEditor.editLocation(location, this.allInstitutions, this.rootLocation, this.allDirs);

    },
      error => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.alertService.showStickyMessage("Load Error", `Unable to retrieve location from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
          MessageSeverity.error);
      });
  }

  saveCompleted(event) {
    //this.loadTree(this.institutionId);
    //this.currentNode.id = event.id;
    //this.currentNode.value = event.name;
    //this.currentNodeId = event.id;
    var oopNodeController = this.treeComponent.getControllerByNodeId(this.currentNodeId);
    if (oopNodeController) {
      if (this.currentNodeId != event.id) {
        oopNodeController.changeNodeId(event.id);
      }

      oopNodeController.select();
      this.currentNode = oopNodeController.tree.node;
      this.currentNodeId = event.id;
      this.handleSelected(oopNodeController.tree, true);
      this.loadTree(this.institutionId, true, this.filter);
    }
  }

  cancelChangeCompleted(event) {
    var oopNodeController = this.treeComponent.getControllerByNodeId(this.currentNodeId);
    if (oopNodeController) {
      oopNodeController.rename(event.name);
      oopNodeController.unselect();
      oopNodeController.select();
    }
  }

  renamedLocation(txt) {
    var oopNodeController = this.treeComponent.getControllerByNodeId(this.currentNodeId);
    if (oopNodeController) {
      oopNodeController.rename(txt);
    }
  }

  getLocationTypeByName(name: string) {
    var types = this.locationTypes.filter(r => Utilities.searchArray(name, false, r.name));
    if (types && types.length > 0) {
      return types[0].id;
    }

    return null;
  }

  ngAfterViewInit() {

    //if (this.tree.children && this.tree.children.length > 0) {
    //  var oopNodeController = this.treeComponent.getControllerByNodeId(this.tree.children[0].id);
    //  console.log(oopNodeController);
    //  if (oopNodeController) oopNodeController.collapse();
    //}


    //var oopNodeController = this.treeComponent.getControllerByNodeId(this.tree.children[0].id);
    //oopNodeController.select();

    if (this.locationEditor) {

      this.locationEditor.changesSavedCallback = () => {
        //this.addNewLocationToList();
        //this.editorModal.hide();
      };

      this.locationEditor.changesCancelledCallback = () => {
        //this.editedLocation = null;
        //this.sourceLocation = null;
        //this.editorModal.hide();
      };
    }
  }


  addNewLocationToList() {
    if (this.sourceLocation) {
      Object.assign(this.sourceLocation, this.editedLocation);

      let sourceIndex = this.rowsCache.indexOf(this.sourceLocation, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rowsCache, sourceIndex, 0);

      sourceIndex = this.rows.indexOf(this.sourceLocation, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rows, sourceIndex, 0);

      this.editedLocation = null;
      this.sourceLocation = null;
    }
    else {
      let location = new Location();
      Object.assign(location, this.editedLocation);
      this.editedLocation = null;

      let maxIndex = 0;
      for (let r of this.rowsCache) {
        if ((<any>r).index > maxIndex)
          maxIndex = (<any>r).index;
      }

      (<any>location).index = maxIndex + 1;

      this.rowsCache.splice(0, 0, location);
      this.rows.splice(0, 0, location);
      this.rows = [...this.rows];
    }
  }




  loadData() {
    this.alertService.startLoadingMessage();
    this.loadingIndicator = true;

    this.locationService.getLocationsAndInstitutions(null, null, this.accountService.currentUser.institutionId)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let locations = results[0];
        let institutions = results[1];

        locations.forEach((location, index, locations) => {
          (<any>location).index = index + 1;
        });


        this.rowsCache = [...locations];
        this.rows = locations;

        this.allInstitutions = institutions;
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve locations from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });

    this.locationService.getLocationTypes()
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.locationTypes = results;
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve location types from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  onSearchChanged(value: string) {
    this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.name, r.description));
  }


  onEditorModalHidden() {
    this.editingLocationName = null;
    this.locationEditor.resetForm(true);
  }


  newLocation() {
    this.editingLocationName = null;
    this.sourceLocation = null;
    this.editedLocation = this.locationEditor.newLocation(this.allInstitutions, this.allDirs);
    this.openDialog(this.editedLocation);
    //this.editorModal.show();
  }


  editLocation(row: Location) {

    this.editingLocationName = { name: row.name };
    this.sourceLocation = row;
    this.editedLocation = this.locationEditor.editLocation(row, this.allInstitutions, this.rootLocation, this.allDirs);
  }

  deleteLocation(row: Location) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" location?', DialogType.confirm, () => this.deleteLocationHelper(row));
  }


  deleteLocationHelper(row: Location) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.locationService.deleteLocation(row)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.rowsCache = this.rowsCache.filter(item => item !== row)
        this.rows = this.rows.filter(item => item !== row)
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the location.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
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
    this.locationService.importFile<HttpEvent<Object>>(formData)
      .subscribe(event => {
        if (event.type === HttpEventType.UploadProgress)
          this.progress = Math.round(100 * event.loaded / event.total);
        else if (event.type === HttpEventType.Response) {
          this.message = 'Upload success.';
          //var fileUploadResponse: any = ;
          //this.filename = fileUploadResponse.fileName;
          this.onUploadFinished(event.body);
        }

        this.loadingIndicator = false;
      });
  }

  onUploadFinished(response: any) {
    this.alertService.stopLoadingMessage();
    if (response.isSuccess) {
      this.alertService.showMessage(response.message);
      this.loadTree(null);
    } else {
      this.alertService.showStickyMessage("Import Error", `Unable to import the file to the server.\r\nErrors: "${response.message}"`,
        MessageSeverity.error);
    }

  }

  syncSmartRooms() {
    this.locationService.syncSmartRoomResources(this.accountService.currentUser.institutionId).subscribe(response => {
      this.alertService.stopLoadingMessage();
      this.loadingIndicator = false;
      if (response && response.isSuccess) {
        this.alertService.showMessage("Successful Sync", response.message, MessageSeverity.success);
        this.loadTree(null);
      } else {
        this.alertService.showStickyMessage("Sync Error", `Unable to sync smart room locations.\r\nErrors: "${response.message}"`,
          MessageSeverity.error);
      }

    },
      error => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.alertService.showStickyMessage("Sync Error", `Unable to sync smart room locations.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
          MessageSeverity.error);
      });
  }

  syncSmartRoomSchedules() {
    this.locationService.syncSmartRoomSchedules(this.accountService.currentUser.institutionId).subscribe(response => {
      this.alertService.stopLoadingMessage();
      this.loadingIndicator = false;
      if (response && response.isSuccess) {
        this.alertService.showMessage("Successful Sync", response.message, MessageSeverity.success);
        this.loadTree(null);
      } else {
        this.alertService.showStickyMessage("Sync Error", `Unable to sync smart room schedules.\r\nErrors: "${response.message}"`,
          MessageSeverity.error);
      }

    },
      error => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.alertService.showStickyMessage("Sync Error", `Unable to sync smart room schedules.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
          MessageSeverity.error);
      });
  }

  export() {
    this.gotoUrl(`/api/location/exporttree?currentUserInstitutionId=${this.accountService.currentUser.institutionId}&institutionId=${this.accountService.currentUser.institutionId}`);
  }
  gotoUrl(url) {
    window.open(url, '_blank');
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

  getColorNameById(id) {
    let code = '';

    if (this.imageReferenceColors) {
      let color = this.imageReferenceColors.filter(e => e.id == id);
      if (color && color.length > 0) {
        code = color[0].name;
      }
    }

    return code;
  }

  applyFilters() {
    if (!this.filter.keyword) {
      this.filter.keyword = '';
    }
    this.loadTree('', false, this.filter);
  }

  clearFilters() {
    this.filter = new LocationTreeFilter();
    this.filter.institutionId = this.accountService.currentUser.institutionId;
    this.filter.currentUserInstitutionId = this.filter.institutionId;
    this.applyFilters();
  }

  downloadResults() {
    //this.cleanFilter();
    let url = `/api/location/exporttree?currentUserInstitutionId=${this.filter.currentUserInstitutionId}&institutionId=${this.filter.institutionId}&imageReferenceColorId=${this.filter.imageReferenceColorId}&keyword=${this.filter.keyword}&isBookingOnly=${this.filter.isBookingOnly}`;
    this.gotoUrl(url);
  }

  //cleanFilter() {
  //  if (!this.filter.keyword) {
  //    this.filter.keyword = '';
  //  }
  //  if (!this.filter.imageReferenceColorId || this.filter.imageReferenceColorId == "undefined") {
  //    this.filter.imageReferenceColorId = '';
  //  }
  //  if (!this.filter.institutionId || this.filter.institutionId == "undefined") {
  //    this.filter.institutionId = '';
  //  }
  //}

  get canManageLocations() {
    return this.accountService.userHasPermission(Permission.manageLocationsPermission)
  }

}
