import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, Inject } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { OutletClassRoster } from 'src/app/models/meal-order/outlet-class-roster.model';
import { OutletClassRosterEditorComponent } from './class-roster-editor.component';
import { DateOnlyPipe } from 'src/app/pipes/datetime.pipe';
import { ClassRosterFilter, Filter, PagedResult } from 'src/app/models/sieve-filter.model';
import { Permission } from 'src/app/models/permission.model';
import { AlertService, DialogType, MessageSeverity } from 'src/app/services/alert.service';
import { AppTranslationService } from 'src/app/services/app-translation.service';
import { AccountService } from 'src/app/services/account.service';
import { ClassService } from 'src/app/services/meal-order/class.service';
import { MealSession } from 'src/app/models/meal-order/meal-session.model';
import { Utilities } from 'src/app/services/utilities';
import { saveAs } from 'file-saver';
import * as moment from 'moment';

@Component({
  selector: 'class-rosters-management',
  templateUrl: './class-rosters-management.component.html',
  styleUrls: ['./class-rosters-management.component.css']
})
export class OutletClassRostersManagementComponent implements OnInit {
  columns: any[] = [];
  rows: OutletClassRoster[] = [];
  rowsCache: OutletClassRoster[] = [];
  allPermissions: Permission[] = [];
  editedOutletClassRoster: OutletClassRoster;
  sourceOutletClassRoster: OutletClassRoster;
  loadingIndicator: boolean;
  filter: ClassRosterFilter;
  pagedResult: PagedResult;
  keyword: string = '';
  @Input() isHideHeader: boolean;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('outletClassRosterEditor')
  outletClassRosterEditor: OutletClassRosterEditorComponent;
  header: string;

  dialogHeader: string;
  outletId: any;
  catererId: any;
  outletProfileId: any;
  private mealSession: MealSession;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private classService: ClassService, public dialog: MatDialog, public dialogRef: MatDialogRef<OutletClassRostersManagementComponent>, 
    @Inject(MAT_DIALOG_DATA) public data: any) {
    this.header = data ? data.header : 'Class Detailing';
    if (typeof (data.mealSession) != typeof (undefined)) {
      this.outletId = data.mealSession.outletId;
      this.catererId = data.mealSession.catererId;
      this.outletProfileId = data.outletProfileId;
      this.mealSession = data.mealSession;
    }
  }

  openDialog(outletClassRoster: OutletClassRoster): void {
    const dialogRef = this.dialog.open(OutletClassRosterEditorComponent, {
      data: { header: this.dialogHeader, outletClassRoster: outletClassRoster, catererId: this.catererId, mealSession: this.mealSession, outletProfileId: this.outletProfileId },
      width: '900px',
      disableClose: false
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData(null);
    });
  }

  initializeFilter() {
    this.filter = new ClassRosterFilter(1, 10);
    this.filter.sorts = 'startDate';
    this.filter.filters = '';
    this.filter.page = 1;
  }

  initializePagedResult() {
    this.pagedResult = new PagedResult();
    this.pagedResult.totalCount = 0;
    this.pagedResult.pagedData = [];
    this.pagedResult.filter = this.filter;
  }

  initializeTableDefinition() {
    let gT = (key: string) => this.translationService.getTranslation(key);

    this.columns = [
      { prop: 'label', name: 'Label' },
      { prop: 'startDate', name: 'Start', pipe: new DateOnlyPipe('en-SG') },
      { prop: 'endDate', name: 'End', pipe: new DateOnlyPipe('en-SG') },
      { name: '', width: 150, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
    ];
  }

  ngOnInit() {
    this.initializeFilter();
    this.initializePagedResult();
    this.initializeTableDefinition();
    this.loadData();
  }


  loadData(ev?: any) {
    this.alertService.startLoadingMessage();
    this.loadingIndicator = true;
    this.filter.pageSize = 10;

    if (ev) {
      this.filter.page = ev.offset + 1;
      if (ev.sorts) {
        this.filter.sorts = ev.sorts[0].dir == 'desc' ? '-' + ev.sorts[0].prop : ev.sorts[0].prop;
      }
    }

    if (!this.keyword) this.keyword = '';
    this.filter.filters = '(IsActive)==true,(Label)@=' + this.keyword + ',(MealSessionId)==' + this.mealSession.id;
    this.filter.catererId = this.catererId;
    this.filter.outletId = this.outletId;
    this.classService.getClassRostersByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let outletClassRosters = results.pagedData;

        outletClassRosters.forEach((outletClassRoster, index, outletClassRosters) => {
          (<any>outletClassRoster).index = index + 1;
        });


        this.rowsCache = [...outletClassRosters];
        this.rows = outletClassRosters;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve records from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }


  onSearchChanged(value: string) {
    this.keyword = value;
    this.loadData(null);
  }

  newOutletClassRoster() {
    this.dialogHeader = 'New Class Roster';
    this.editedOutletClassRoster = new OutletClassRoster();
    this.openDialog(this.editedOutletClassRoster);
  }


  editOutletClassRoster(row: OutletClassRoster) {
    this.editedOutletClassRoster = row;
    this.dialogHeader = 'Edit Class Roster';
    this.editedOutletClassRoster.catererId = this.catererId;
    this.openDialog(this.editedOutletClassRoster);
  }

  deleteOutletClassRoster(row: OutletClassRoster) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.label + '\" roster?', DialogType.confirm, () => this.deleteOutletClassRosterHelper(row));
  }


  deleteOutletClassRosterHelper(row: OutletClassRoster) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.classService.deleteClassRoster(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the class roster.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  private cancel() {
    this.alertService.resetStickyMessage();
    this.dialogRef.close();
  }

  downloadResults(row) {
    this.filter.classRosterId = row.id;
    this.filter.outletProfileId = this.outletProfileId;
    const fileName = moment().format('DDMMYYYY_hhmmss') + '_ClassRosters.xlsx';
    this.classService.downloadClassRoster(this.filter).subscribe(
      data => {
        console.log(data);
        saveAs(data, fileName);
      },
      err => {
        alert("Problem while downloading the file.");
        console.error(err);
      }
    );
  }

  get canManageOutletClassRosters() {
    return true; //this.accountService.userHasPermission(Permission.manageOutletClassRostersPermission)
  }

}
