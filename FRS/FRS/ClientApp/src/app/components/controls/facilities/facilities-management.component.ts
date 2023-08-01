import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { FacilityService } from '../../../services/facility.service';
import { Utilities } from '../../../services/utilities';
import { Facility } from '../../../models/facility.model';
import { Permission } from '../../../models/permission.model';
import { FacilityEditorComponent } from "./facility-editor.component";
import { Institution } from 'src/app/models/institution.model';
import { MatDialog } from '@angular/material';
import { Subscription } from 'rxjs';

@Component({
  selector: 'facilities-management',
  templateUrl: './facilities-management.component.html',
  styleUrls: ['./facilities-management.component.css']
})
export class FacilitiesManagementComponent implements OnInit, AfterViewInit, OnDestroy {
  private subscription: Subscription = new Subscription();
  columns: any[] = [];
  rows: Facility[] = [];
  rowsCache: Facility[] = [];
  allInstitutions: Institution[] = [];
  editedFacility: Facility;
  sourceFacility: Facility;
  editingFacilityName: { name: string };
  loadingIndicator: boolean;



  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('facilityEditor')
  facilityEditor: FacilityEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private facilityService: FacilityService, public dialog: MatDialog) {
  }

  openDialog(facility: Facility, allInstitutions: Institution[]): void {
    const dialogRef = this.dialog.open(FacilityEditorComponent, {
      data: { header: this.header, facility: facility, allInstitutions: allInstitutions },
      width: '450px'
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData();
    });
  }

  ngOnInit() {

    let gT = (key: string) => this.translationService.getTranslation(key);

    this.columns = [
      { prop: "index", name: '#', width: 50, cellTemplate: this.indexTemplate, canAutoResize: false },
      { prop: 'name', name: gT('facilities.management.Name') },
      { prop: 'description', name: gT('facilities.management.Description') },
      { prop: 'link', name: gT('facilities.management.Link') },
      //{ prop: 'facilityTypeName', name: gT('facilities.management.FacilityType'), width: 80 },
      { name: '', cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
    ];

    if (!this.accountService.currentUser.institutionId || this.accountService.currentUser.institutionId == '0') {
      this.columns.splice(1, 0, { prop: 'institutionName', name: gT('roles.management.Institution'), width: 120 });
    }

    this.loadData();
  }





  ngAfterViewInit() {

    this.facilityEditor.changesSavedCallback = () => {
      this.addNewFacilityToList();
      //this.editorModal.hide();
    };

    this.facilityEditor.changesCancelledCallback = () => {
      this.editedFacility = null;
      this.sourceFacility = null;
      //this.editorModal.hide();
    };
  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  addNewFacilityToList() {
    if (this.sourceFacility) {
      Object.assign(this.sourceFacility, this.editedFacility);

      let sourceIndex = this.rowsCache.indexOf(this.sourceFacility, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rowsCache, sourceIndex, 0);

      sourceIndex = this.rows.indexOf(this.sourceFacility, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rows, sourceIndex, 0);

      this.editedFacility = null;
      this.sourceFacility = null;
    }
    else {
      let facility = new Facility();
      Object.assign(facility, this.editedFacility);
      this.editedFacility = null;

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

    this.subscription.add(this.facilityService.getFacilitiesAndFacilityTypes(null, null, this.accountService.currentUser.institutionId)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let facilities = results[0];
        let institutions = results[1];

        facilities.forEach((facility, index, facilities) => {
          (<any>facility).index = index + 1;
        });


        this.rowsCache = [...facilities];
        this.rows = facilities;

        this.allInstitutions = institutions;
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve facilities from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }


  onSearchChanged(value: string) {
    this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.name, r.description));
  }


  onEditorModalHidden() {
    this.editingFacilityName = null;
    this.facilityEditor.resetForm(true);
  }


  newFacility() {
    this.editingFacilityName = null;
    this.sourceFacility = null;
    this.editedFacility = this.facilityEditor.newFacility(this.allInstitutions);
    this.header = 'New Equipment';
    this.openDialog(this.editedFacility, this.allInstitutions);
    //this.editorModal.show();
  }


  editFacility(row: Facility) {
    this.editingFacilityName = { name: row.name };
    this.sourceFacility = row;
    this.editedFacility = this.facilityEditor.editFacility(row, this.allInstitutions);
    this.header = 'Edit Equipment';
    this.openDialog(this.editedFacility, this.allInstitutions);
    //this.editorModal.show();
  }

  deleteFacility(row: Facility) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" equipment?', DialogType.confirm, () => this.deleteFacilityHelper(row));
  }


  deleteFacilityHelper(row: Facility) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.facilityService.deleteFacility(row)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.rowsCache = this.rowsCache.filter(item => item !== row)
        this.rows = this.rows.filter(item => item !== row)
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the facility.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }


  get canManageFacilities() {
    return this.accountService.userHasPermission(Permission.manageFacilitiesPermission)
  }

}
