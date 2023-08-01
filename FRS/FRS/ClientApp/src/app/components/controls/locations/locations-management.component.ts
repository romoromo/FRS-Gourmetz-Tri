import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, Inject } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { LocationService } from '../../../services/location.service';
import { Utilities } from '../../../services/utilities';
import { Location } from '../../../models/location.model';
import { Permission } from '../../../models/permission.model';
import { LocationEditorComponent } from "./location-editor.component";
import { Institution } from 'src/app/models/institution.model';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
export interface DialogData {
  header: string;
  location: Location;
  institutions: Institution[];
}

@Component({
  selector: 'locations-management',
  templateUrl: './locations-management.component.html',
  styleUrls: ['./locations-management.component.css']
})
export class LocationsManagementComponent implements OnInit, AfterViewInit {
  header: string;
  columns: any[] = [];
  rows: Location[] = [];
  rowsCache: Location[] = [];
  allInstitutions: Institution[] = [];
  editedLocation: Location;
  sourceLocation: Location;
  editingLocationName: { name: string };
  loadingIndicator: boolean;



  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('locationEditor')
  locationEditor: LocationEditorComponent;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private locationService: LocationService, public dialog: MatDialog) {
  }

  openDialog(location: Location): void {
    const dialogRef = this.dialog.open(LocationEditorComponent, {
      data: { header: this.header, location: location, institutions: this.allInstitutions }
    });

    dialogRef.afterClosed().subscribe(result => {
      
    });
  }


  ngOnInit() {

    let gT = (key: string) => this.translationService.getTranslation(key);

    this.columns = [
      { prop: "index", name: '#', width: 50, cellTemplate: this.indexTemplate, canAutoResize: false },
      { prop: 'name', name: gT('locations.management.Name') },
      { prop: 'description', name: gT('locations.management.Description') },
      { prop: 'capacity', name: gT('locations.management.Capacity') },
      { prop: 'facilityNames', name: gT('locations.management.Facilities') },
      { prop: 'institutionNames', name: gT('locations.management.Institution') },
      { name: '', width: 150, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
    ];

    this.loadData();
  }





  ngAfterViewInit() {

    this.locationEditor.changesSavedCallback = () => {
      this.addNewLocationToList();
      this.editorModal.hide();
    };

    this.locationEditor.changesCancelledCallback = () => {
      this.editedLocation = null;
      this.sourceLocation = null;
      this.editorModal.hide();
    };
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
    this.editedLocation = this.locationEditor.newLocation(this.allInstitutions);
    this.openDialog(this.editedLocation);
    //this.editorModal.show();
  }


  editLocation(row: Location) {
    
    this.editingLocationName = { name: row.name };
    this.sourceLocation = row;
    this.editedLocation = this.locationEditor.editLocation(row, this.allInstitutions);
    //this.editorModal.show();
    this.openDialog(this.editedLocation);
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


  get canManageLocations() {
    return this.accountService.userHasPermission(Permission.manageLocationsPermission)
  }

}
