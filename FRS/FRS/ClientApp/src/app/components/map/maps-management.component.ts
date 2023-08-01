import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, Inject, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { MapService } from 'src/app/services/map.service';
import { MatDialog } from '@angular/material';
import { Utilities } from 'src/app/services/utilities';
import { MapModel } from 'src/app/models/map.model';
import { Permission } from 'src/app/models/permission.model';
import { AlertService, MessageSeverity, DialogType } from 'src/app/services/alert.service';
import { AppTranslationService } from 'src/app/services/app-translation.service';
import { AccountService } from 'src/app/services/account.service';
import { MapEditorComponent } from './map-editor.component';
import { Subscription } from 'rxjs';


@Component({
  selector: 'maps-management',
  templateUrl: './maps-management.component.html',
  styleUrls: ['./maps-management.component.css']
})
export class MapsManagementComponent implements OnInit, OnDestroy, AfterViewInit {
  private subscription: Subscription = new Subscription();
  columns: any[] = [];
  rows: MapModel[] = [];
  rowsCache: MapModel[] = [];
  allPermissions: Permission[] = [];
  editedMap: MapModel;
  sourceMap: MapModel;
  editingMapName: { key: string };
  loadingIndicator: boolean;



  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('mapEditor')
  mapEditor: MapEditorComponent;
  header = "Map Form";
    showEditor: boolean;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private mapService: MapService, public dialog: MatDialog) {
  }

  //openDialog(map: Map): void {
  //  const dialogRef = this.dialog.open(MapEditorComponent, {
  //    data: { header: this.header, map: map },
  //    width: '400px',
  //    disableClose: true
  //  });

  //  this.subscription.add(dialogRef.afterClosed().subscribe(result => {
  //    this.loadData();
  //  }));
  //}

  ngOnInit() {

    let gT = (key: string) => this.translationService.getTranslation(key);

    this.columns = [
      { prop: "index", name: '#', width: 50, cellTemplate: this.indexTemplate, canAutoResize: false },
      { prop: 'code', name: gT('maps.management.Code') },
      { prop: 'label', name: gT('maps.management.Label') },
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

    this.mapEditor.changesSavedCallback = () => {
      //this.addNewMapToList();
      //this.editorModal.hide();

      this.showEditor = false;
      this.loadData();
    };

    this.mapEditor.changesCancelledCallback = () => {
      this.editedMap = null;
      this.sourceMap = null;
      //this.editorModal.hide();

      this.showEditor = false;
    };
  }


  addNewMapToList() {
    if (this.sourceMap) {
      Object.assign(this.sourceMap, this.editedMap);

      let sourceIndex = this.rowsCache.indexOf(this.sourceMap, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rowsCache, sourceIndex, 0);

      sourceIndex = this.rows.indexOf(this.sourceMap, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rows, sourceIndex, 0);

      this.editedMap = null;
      this.sourceMap = null;
    }
    else {
      let map = new MapModel();
      Object.assign(map, this.editedMap);
      this.editedMap = null;

      let maxIndex = 0;
      for (let r of this.rowsCache) {
        if ((<any>r).index > maxIndex)
          maxIndex = (<any>r).index;
      }

      (<any>map).index = maxIndex + 1;

      this.rowsCache.splice(0, 0, map);
      this.rows.splice(0, 0, map);
      this.rows = [...this.rows];
    }
  }




  loadData() {
    //this.alertService.startLoadingMessage();
    this.loadingIndicator = true;

    this.subscription.add(this.mapService.getMapByInstitutionId(this.accountService.currentUser.institutionId)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let maps = results[0];
        let permissions = results[1];

        maps.forEach((map, index, maps) => {
          (<any>map).index = index + 1;
        });


        this.rowsCache = [...maps];
        this.rows = maps;

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
    this.editingMapName = null;
    this.mapEditor.resetForm(true);
  }


  newMap() {
    this.editingMapName = null;
    this.sourceMap = null;
    this.editedMap = this.mapEditor.newMap();
    //this.openDialog(this.editedMap);
    //this.editorModal.show();
    this.showEditor = true;
  }


  editMap(row: MapModel) {
    this.editingMapName = { key: row.code };
    this.sourceMap = row;
    this.editedMap = this.mapEditor.editMap(row);
    //this.editorModal.show();
    //this.openDialog(this.editedMap);
    this.showEditor = true;
  }

  deleteMap(row: MapModel) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.code + '\"?', DialogType.confirm, () => this.deleteMapHelper(row));
  }


  deleteMapHelper(row: MapModel) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.subscription.add(this.mapService.deleteMap(row.id)
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


  get canManageMaps() {
    return true;//this.accountService.userHasPermission(Permission.manageMapPermission);
  }

}
