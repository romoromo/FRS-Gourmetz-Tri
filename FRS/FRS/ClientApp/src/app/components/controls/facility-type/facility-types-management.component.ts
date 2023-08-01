import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, Inject, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { FacilityType } from '../../../models/facility-type.model';
import { Permission } from '../../../models/permission.model';
import { FacilityTypeEditorComponent } from "./facility-type-editor.component";
import { FacilityTypeService } from 'src/app/services/facility-type.service';
import { MatDialog } from '@angular/material';
import { Subscription } from 'rxjs';


@Component({
  selector: 'facility-types-management',
  templateUrl: './facility-types-management.component.html',
  styleUrls: ['./facility-types-management.component.css']
})
export class FacilityTypesManagementComponent implements OnInit, AfterViewInit, OnDestroy {
  private subscription: Subscription = new Subscription();
  columns: any[] = [];
  rows: FacilityType[] = [];
  rowsCache: FacilityType[] = [];
  allPermissions: Permission[] = [];
  editedFacilityType: FacilityType;
  sourceFacilityType: FacilityType;
  editingFacilityTypeName: { name: string };
  loadingIndicator: boolean;



  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('facilityTypeEditor')
  facilityTypeEditor: FacilityTypeEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private facilityTypeService: FacilityTypeService, public dialog: MatDialog) {
  }

  openDialog(facilityType: FacilityType): void {
    const dialogRef = this.dialog.open(FacilityTypeEditorComponent, {
      data: { header: this.header, facilityType: facilityType },
      width: '400px'
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData();
    });
  }

  ngOnInit() {

    let gT = (key: string) => this.translationService.getTranslation(key);

    this.columns = [
      { prop: "index", name: '#', width: 50, cellTemplate: this.indexTemplate, canAutoResize: false },
      { prop: 'name', name: gT('facilityTypes.management.Name') },
      { prop: 'description', name: gT('facilityTypes.management.Description') },
      { name: '', cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
    ];

    if (!this.accountService.currentUser.institutionId || this.accountService.currentUser.institutionId == '0') {
      this.columns.splice(1, 0, { prop: 'institutionName', name: gT('roles.management.Institution'), width: 120 });
    }
    this.loadData();
  }





  ngAfterViewInit() {

    this.facilityTypeEditor.changesSavedCallback = () => {
      this.addNewFacilityTypeToList();
      //this.editorModal.hide();
    };

    this.facilityTypeEditor.changesCancelledCallback = () => {
      this.editedFacilityType = null;
      this.sourceFacilityType = null;
      //this.editorModal.hide();
    };
  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  addNewFacilityTypeToList() {
    if (this.sourceFacilityType) {
      Object.assign(this.sourceFacilityType, this.editedFacilityType);

      let sourceIndex = this.rowsCache.indexOf(this.sourceFacilityType, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rowsCache, sourceIndex, 0);

      sourceIndex = this.rows.indexOf(this.sourceFacilityType, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rows, sourceIndex, 0);

      this.editedFacilityType = null;
      this.sourceFacilityType = null;
    }
    else {
      let facility = new FacilityType();
      Object.assign(facility, this.editedFacilityType);
      this.editedFacilityType = null;

      let maxIndex = 0;
      for (let r of this.rowsCache) {
        if ((<any>r).index > maxIndex)
          maxIndex = (<any>r).index;
      }

      (<any>facility).index = maxIndex + 1;

      this.rowsCache.splice(0, 0, facility);
      this.rows.splice(0, 0, facility);
      this.rows = [...this.rows];
    }
  }




  loadData() {
    this.alertService.startLoadingMessage();
    this.loadingIndicator = true;

    this.subscription.add(this.facilityTypeService.getFacilityTypeByInstitutionId(this.accountService.currentUser.institutionId)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let facilityTypes = results;

        facilityTypes.forEach((facility, index, facilityTypes) => {
          (<any>facility).index = index + 1;
        });


        this.rowsCache = [...facilityTypes];
        this.rows = facilityTypes;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve facility types from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }


  onSearchChanged(value: string) {
    this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.name, r.description));
  }


  onEditorModalHidden() {
    this.editingFacilityTypeName = null;
    this.facilityTypeEditor.resetForm(true);
  }


  newFacilityType() {
    this.editingFacilityTypeName = null;
    this.sourceFacilityType = null;
    this.editedFacilityType = this.facilityTypeEditor.newFacilityType();
    this.header = 'New Facility Type';
    this.openDialog(this.editedFacilityType);
    //this.editorModal.show();
  }


  editFacilityType(row: FacilityType) {
    this.editingFacilityTypeName = { name: row.name };
    this.sourceFacilityType = row;
    this.editedFacilityType = this.facilityTypeEditor.editFacilityType(row);
    this.header = 'Edit Facility Type';
    //this.editorModal.show();
    this.openDialog(this.editedFacilityType);
  }

  deleteFacilityType(row: FacilityType) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" facility type?', DialogType.confirm, () => this.deleteFacilityTypeHelper(row));
  }


  deleteFacilityTypeHelper(row: FacilityType) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.subscription.add(this.facilityTypeService.deleteFacilityType(row)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.rowsCache = this.rowsCache.filter(item => item !== row)
        this.rows = this.rows.filter(item => item !== row)
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the facility type.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }


  get canManageFacilityTypes() {
    return this.accountService.userHasPermission(Permission.manageFacilityTypesPermission)
  }

}
