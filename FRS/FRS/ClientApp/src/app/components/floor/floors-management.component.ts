import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, Inject, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { FloorService } from 'src/app/services/floor.service';
import { MatDialog } from '@angular/material';
import { Utilities } from 'src/app/services/utilities';
import { Floor } from 'src/app/models/floor.model';
import { Permission } from 'src/app/models/permission.model';
import { AlertService, MessageSeverity, DialogType } from 'src/app/services/alert.service';
import { AppTranslationService } from 'src/app/services/app-translation.service';
import { AccountService } from 'src/app/services/account.service';
import { FloorEditorComponent } from './floor-editor.component';
import { Subscription } from 'rxjs';


@Component({
  selector: 'floors-management',
  templateUrl: './floors-management.component.html',
  styleUrls: ['./floors-management.component.css']
})
export class FloorsManagementComponent implements OnInit, OnDestroy, AfterViewInit {
  private subscription: Subscription = new Subscription();
  columns: any[] = [];
  rows: Floor[] = [];
  rowsCache: Floor[] = [];
  allPermissions: Permission[] = [];
  editedFloor: Floor;
  sourceFloor: Floor;
  editingFloorName: { key: string };
  loadingIndicator: boolean;



  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('floorEditor')
  floorEditor: FloorEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private floorService: FloorService, public dialog: MatDialog) {
  }

  openDialog(floor: Floor): void {
    const dialogRef = this.dialog.open(FloorEditorComponent, {
      data: { header: this.header, floor: floor },
      width: '400px',
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
      { prop: 'code', name: gT('floors.management.Code') },
      { prop: 'label', name: gT('floors.management.Label') },
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

    this.floorEditor.changesSavedCallback = () => {
      this.addNewFloorToList();
      //this.editorModal.hide();
    };

    this.floorEditor.changesCancelledCallback = () => {
      this.editedFloor = null;
      this.sourceFloor = null;
      //this.editorModal.hide();
    };
  }


  addNewFloorToList() {
    if (this.sourceFloor) {
      Object.assign(this.sourceFloor, this.editedFloor);

      let sourceIndex = this.rowsCache.indexOf(this.sourceFloor, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rowsCache, sourceIndex, 0);

      sourceIndex = this.rows.indexOf(this.sourceFloor, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rows, sourceIndex, 0);

      this.editedFloor = null;
      this.sourceFloor = null;
    }
    else {
      let floor = new Floor();
      Object.assign(floor, this.editedFloor);
      this.editedFloor = null;

      let maxIndex = 0;
      for (let r of this.rowsCache) {
        if ((<any>r).index > maxIndex)
          maxIndex = (<any>r).index;
      }

      (<any>floor).index = maxIndex + 1;

      this.rowsCache.splice(0, 0, floor);
      this.rows.splice(0, 0, floor);
      this.rows = [...this.rows];
    }
  }




  loadData() {
    //this.alertService.startLoadingMessage();
    this.loadingIndicator = true;

    this.subscription.add(this.floorService.getFloorByInstitutionId(this.accountService.currentUser.institutionId)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let floors = results[0];
        let permissions = results[1];

        floors.forEach((floor, index, floors) => {
          (<any>floor).index = index + 1;
        });


        this.rowsCache = [...floors];
        this.rows = floors;

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
    this.editingFloorName = null;
    this.floorEditor.resetForm(true);
  }


  newFloor() {
    this.editingFloorName = null;
    this.sourceFloor = null;
    this.editedFloor = this.floorEditor.newFloor();
    this.header = 'New';
    this.openDialog(this.editedFloor);
    //this.editorModal.show();
  }


  editFloor(row: Floor) {
    this.editingFloorName = { key: row.code };
    this.sourceFloor = row;
    this.editedFloor = this.floorEditor.editFloor(row);
    this.header = 'Edit';
    //this.editorModal.show();
    this.openDialog(this.editedFloor);
  }

  deleteFloor(row: Floor) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.code + '\"?', DialogType.confirm, () => this.deleteFloorHelper(row));
  }


  deleteFloorHelper(row: Floor) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.subscription.add(this.floorService.deleteFloor(row.id)
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


  get canManageFloors() {
    return true;//this.accountService.userHasPermission(Permission.manageFloorPermission);
  }

}
