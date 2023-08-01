import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, Inject, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { BuildingService } from 'src/app/services/building.service';
import { MatDialog } from '@angular/material';
import { Utilities } from 'src/app/services/utilities';
import { Building } from 'src/app/models/building.model';
import { Permission } from 'src/app/models/permission.model';
import { AlertService, MessageSeverity, DialogType } from 'src/app/services/alert.service';
import { AppTranslationService } from 'src/app/services/app-translation.service';
import { AccountService } from 'src/app/services/account.service';
import { BuildingEditorComponent } from './building-editor.component';
import { Subscription } from 'rxjs';


@Component({
  selector: 'buildings-management',
  templateUrl: './buildings-management.component.html',
  styleUrls: ['./buildings-management.component.css']
})
export class BuildingsManagementComponent implements OnInit, OnDestroy, AfterViewInit {
  private subscription: Subscription = new Subscription();
  columns: any[] = [];
  rows: Building[] = [];
  rowsCache: Building[] = [];
  allPermissions: Permission[] = [];
  editedBuilding: Building;
  sourceBuilding: Building;
  editingBuildingName: { key: string };
  loadingIndicator: boolean;



  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('buildingEditor')
  buildingEditor: BuildingEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private buildingService: BuildingService, public dialog: MatDialog) {
  }

  openDialog(building: Building): void {
    const dialogRef = this.dialog.open(BuildingEditorComponent, {
      data: { header: this.header, building: building },
      width: '600px',
      disableClose: true
    });

    this.subscription.add(dialogRef.afterClosed().subscribe(result => {
      this.loadData();
    }));
  }

  ngOnInit() {

    let gT = (key: string) => this.translationService.getTranslation(key);

    this.columns = [
      { prop: "index", name: '#', width: 50, cellTemplate: this.indexTemplate, canAutoResize: false },
      { prop: 'code', name: gT('buildings.management.Code') },
      { prop: 'label', name: gT('buildings.management.Label') },
      { name: '', width: 150, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
    ];

    if (!this.accountService.currentUser.institutionId || this.accountService.currentUser.institutionId == '0') {
      this.columns.splice(1, 0, { prop: 'institutionName', name: gT('roles.management.Institution'), width: 120 });
    }
    this.loadData();
  }

  ngOnDestroy() {
    this.alertService.resetStickyMessage();
    this.subscription.unsubscribe();
  }



  ngAfterViewInit() {

    this.buildingEditor.changesSavedCallback = () => {
      this.addNewBuildingToList();
      //this.editorModal.hide();
    };

    this.buildingEditor.changesCancelledCallback = () => {
      this.editedBuilding = null;
      this.sourceBuilding = null;
      //this.editorModal.hide();
    };
  }


  addNewBuildingToList() {
    if (this.sourceBuilding) {
      Object.assign(this.sourceBuilding, this.editedBuilding);

      let sourceIndex = this.rowsCache.indexOf(this.sourceBuilding, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rowsCache, sourceIndex, 0);

      sourceIndex = this.rows.indexOf(this.sourceBuilding, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rows, sourceIndex, 0);

      this.editedBuilding = null;
      this.sourceBuilding = null;
    }
    else {
      let building = new Building();
      Object.assign(building, this.editedBuilding);
      this.editedBuilding = null;

      let maxIndex = 0;
      for (let r of this.rowsCache) {
        if ((<any>r).index > maxIndex)
          maxIndex = (<any>r).index;
      }

      (<any>building).index = maxIndex + 1;

      this.rowsCache.splice(0, 0, building);
      this.rows.splice(0, 0, building);
      this.rows = [...this.rows];
    }
  }




  loadData() {
    //this.alertService.startLoadingMessage();
    this.loadingIndicator = true;

    this.subscription.add(this.buildingService.getBuildingByInstitutionId(this.accountService.currentUser.institutionId)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let buildings = results[0];
        let permissions = results[1];

        buildings.forEach((building, index, buildings) => {
          (<any>building).index = index + 1;
        });


        this.rowsCache = [...buildings];
        this.rows = buildings;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve data from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }


  onSearchChanged(value: string) {
    this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.code, r.label));
  }


  onEditorModalHidden() {
    this.editingBuildingName = null;
    this.buildingEditor.resetForm(true);
  }


  newBuilding() {
    this.editingBuildingName = null;
    this.sourceBuilding = null;
    this.editedBuilding = this.buildingEditor.newBuilding();
    this.header = 'New';
    this.openDialog(this.editedBuilding);
    //this.editorModal.show();
  }


  editBuilding(row: Building) {
    this.editingBuildingName = { key: row.code };
    this.sourceBuilding = row;
    this.editedBuilding = this.buildingEditor.editBuilding(row);
    this.header = 'Edit';
    //this.editorModal.show();
    this.openDialog(this.editedBuilding);
  }

  deleteBuilding(row: Building) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.code + '\"?', DialogType.confirm, () => this.deleteBuildingHelper(row));
  }


  deleteBuildingHelper(row: Building) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.subscription.add(this.buildingService.deleteBuilding(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.rowsCache = this.rowsCache.filter(item => item !== row)
        this.rows = this.rows.filter(item => item !== row)
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the data.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }


  get canManageBuildings() {
    return true;//this.accountService.userHasPermission(Permission.manageBuildingPermission);
  }

}
