import { Component, ViewChild, Inject, ElementRef, OnDestroy, OnInit } from '@angular/core';
import { fadeInOut } from '../../services/animations';
import { AccountService } from 'src/app/services/account.service';
import { Permission } from 'src/app/models/permission.model';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';
import { Building } from 'src/app/models/building.model';
import { InstitutionService } from 'src/app/services/institution.service';
import { Utilities } from 'src/app/services/utilities';
import { Institution } from 'src/app/models/institution.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { BuildingService } from 'src/app/services/building.service';
import { Subscription } from 'rxjs';
import { FloorService } from '../../services/floor.service';
import { Floor } from '../../models/floor.model';


@Component({
  selector: 'building-editor',
  templateUrl: './building-editor.component.html',
  styleUrls: ['./building-editor.component.css'],
  animations: [fadeInOut]
})
export class BuildingEditorComponent implements OnInit, OnDestroy {
  private subscription: Subscription = new Subscription();
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  public formResetToggle = true;
  private loadingIndicator = false;
  private isNewBuilding = false;
  private originalBuilding: Building = new Building();
  private buildingEdit: Building = new Building();
  private allInstitutions: Institution[] = [];
  private validation = { code: false, label: false };
  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;

  @ViewChild('f')
  private form;

  @ViewChild('code')
  private code;

  @ViewChild('label')
  private label;
    allFloors: Floor[];

  constructor(private alertService: AlertService, private accountService: AccountService, private institutionService: InstitutionService,
    private buildingService: BuildingService,
    private floorService: FloorService,
    public dialogRef: MatDialogRef<BuildingEditorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.building) != typeof (undefined)) {
      if (data.building.id) {
        this.editBuilding(data.building);
      } else {
        this.newBuilding();
      }
    }

    this.floorService.getFloorByInstitutionId(this.accountService.currentUser.institutionId)
      .subscribe(results => {
        
        this.allFloors = results[0];
      },
        error => {
         
        })
  }

  ngOnInit() {
    this.alertService.resetStickyMessage();
  }

  ngOnDestroy() {
    this.alertService.resetStickyMessage();
    this.subscription.unsubscribe();
  }

  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");


    if (!this.buildingEdit.id) {
      this.buildingService.newBuilding(this.buildingEdit).subscribe(building => this.saveSuccessHelper(building), error => this.saveFailedHelper(error));
    }
    else {
      this.buildingService.updateBuilding(this.buildingEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }

  private saveSuccessHelper(building?: Building) {
    if (building)
      Object.assign(this.buildingEdit, building);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewBuilding) {
      this.alertService.showMessage("Success", `Data \"${this.buildingEdit.code}\" was created successfully`, MessageSeverity.success);
    }
    else {
      this.alertService.showMessage("Success", `Changes to data \"${this.buildingEdit.code}\" was saved successfully`, MessageSeverity.success);
    }

    if (this.changesSavedCallback)
      this.changesSavedCallback();

    this.dialogRef.close();
  }

  private saveFailedHelper(error: any) {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage("Save Error", "The below errors occured while saving your changes:", MessageSeverity.error);
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);

    //if (this.changesFailedCallback)
    //  this.changesFailedCallback();

    //this.dialogRef.close();
  }

  add(selected: Floor) {
    if (!selected) return;
    if (!this.buildingEdit.floors) this.buildingEdit.floors = [];

    let a = this.buildingEdit.floors.filter(obj => obj.floorId === selected.id);

    if (a.length > 0) return;

    let f = new Floor();
    Object.assign(f, selected);
    f.order = 1;
    f.isActive = 1;

    if (this.buildingEdit.floors[0]) {
      let lastc = this.buildingEdit.floors[this.buildingEdit.floors.length - 1];
      f.order = lastc.order + 1;
    }

    f.floorId = f.id;
    f.id = "0";
    this.buildingEdit.floors.push(f);
  }

  sort() {
    if (!this.buildingEdit.floors) return;

    this.buildingEdit.floors = this.buildingEdit.floors.sort((a, b) => a.order - b.order);
  }


  private cancel() {
    this.buildingEdit = new Building();

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

  loadInstitutions() {
    this.institutionService.getInstitutions()
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.allInstitutions = results[0];

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve institutions from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  newBuilding() {
    this.showValidationErrors = true;
    this.isNewBuilding = true;
    this.buildingEdit = new Building();
    return this.buildingEdit;
  }

  editBuilding(building: Building) {
    if (building) {
      this.isNewBuilding = false;
      this.showValidationErrors = true;
      this.originalBuilding = building;
      this.buildingEdit = new Building();
      Object.assign(this.buildingEdit, building);

      this.sort();

      return this.buildingEdit;
    }
    else {
      return this.newBuilding();
    }
  }

  get canManageBuildings() {
    return true;//this.accountService.userHasPermission(Permission.manageBuildingPermission);
  }

}
