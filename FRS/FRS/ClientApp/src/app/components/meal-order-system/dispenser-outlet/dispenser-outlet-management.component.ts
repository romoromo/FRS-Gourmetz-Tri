import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { DispenserOutlet } from 'src/app/models/meal-order/dispenser-outlet.model';
import { DispenserOutletEditorComponent } from './dispenser-outlet-editor.component';
import { ClassService } from 'src/app/services/meal-order/class.service';


@Component({
  selector: 'dispenser-outlet-management',
  templateUrl: './dispenser-outlet-management.component.html',
  styleUrls: ['./dispenser-outlet-management.component.css']
})
export class DispenserOutletManagementComponent implements OnInit {
  columns: any[] = [];
  rows: DispenserOutlet[] = [];
  rowsCache: DispenserOutlet[] = [];
  allPermissions: Permission[] = [];
  editedDispenserOutlet: DispenserOutlet;
  sourceDispenserOutlet: DispenserOutlet;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @Input() isHideHeader: boolean;

  @Input() outletId: string;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('dispenserOutletEditor')
  dispenserOutletEditor: DispenserOutletEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private classService: ClassService, public dialog: MatDialog) {
  }

  openDialog(dispenserOutlet: DispenserOutlet): void {
    const dialogRef = this.dialog.open(DispenserOutletEditorComponent, {
      data: { header: this.header, dispenserOutlet: dispenserOutlet },
      width: '1000px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData(null);
    });
  }

  initializeFilter() {
    this.filter = new Filter(1, 10);
    this.filter.sorts = 'dispenserCode';
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
      { prop: 'outletName', name: 'Outlet', width: 200 },
      { prop: 'dispenserCode', name: 'Dispenser Code', width: 200 },
      { prop: 'counterName', name: 'Dispenser Name', width: 200 },
      { name: '', width: 150, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
    ];

    if (!this.accountService.currentUser.institutionId || this.accountService.currentUser.institutionId == '0') {
      this.columns.splice(1, 0, { prop: 'institutionName', name: gT('roles.management.Institution'), width: 120 });
    }
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
    let f = this.outletId ? '(OutletId)==' + this.outletId + ',' : '';
    this.filter.filters = f + '(IsActive)==true,(DispenserCode)@=' + this.keyword + ',(InstitutionId)==' + this.accountService.currentUser.institutionId;
    
    this.classService.getDispenserOutletsByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let dispenserOutlets = results.pagedData;

        dispenserOutlets.forEach((dispenserOutlet, index, dispenserOutlets) => {
          (<any>dispenserOutlet).index = index + 1;
        });


        this.rowsCache = [...dispenserOutlets];
        this.rows = dispenserOutlets;

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

  newDispenserOutlet() {
    this.header = 'New Dispenser';
    this.editedDispenserOutlet = new DispenserOutlet();
    this.editedDispenserOutlet.outletId = this.outletId;
    this.openDialog(this.editedDispenserOutlet);
  }


  editDispenserOutlet(row: DispenserOutlet) {
    this.editedDispenserOutlet = row; 
    this.header = 'Edit Dispenser';
    this.openDialog(this.editedDispenserOutlet);
  }

  deleteDispenserOutlet(row: DispenserOutlet) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.dispenserCode + '\" dispenser?', DialogType.confirm, () => this.deleteDispenserOutletHelper(row));
  }


  deleteDispenserOutletHelper(row: DispenserOutlet) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.classService.deleteDispenserOutlet(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the dispenser.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  get canManageClassLevels() { 
    return this.accountService.userHasPermission(Permission.manageMOSOutletMgtClassLevelsPermission)
  }

}
