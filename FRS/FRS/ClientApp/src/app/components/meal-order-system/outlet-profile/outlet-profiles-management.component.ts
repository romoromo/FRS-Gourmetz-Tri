import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { OutletProfile } from 'src/app/models/meal-order/outlet.model';
import { OutletProfileEditorComponent } from './outlet-profile-editor.component';
import { DeliveryService } from '../../../services/meal-order/delivery.service';


@Component({
  selector: 'outlet-profiles-management',
  templateUrl: './outlet-profiles-management.component.html',
  styleUrls: ['./outlet-profiles-management.component.css']
})
export class OutletProfilesManagementComponent implements OnInit {
  columns: any[] = [];
  rows: OutletProfile[] = [];
  rowsCache: OutletProfile[] = [];
  allPermissions: Permission[] = [];
  editedOutletProfile: OutletProfile;
  sourceOutletProfile: OutletProfile;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('flagTemplate')
  flagTemplate: TemplateRef<any>;

  @ViewChild('outletEditor')
  outletEditor: OutletProfileEditorComponent;

  @ViewChild('outletProfilesTable') table: any;

  header: string;
  @Input() isHideHeader: boolean;

  @Input() catererId: string;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private deliveryService: DeliveryService, public dialog: MatDialog) {
  }

  openDialog(outletProfile: OutletProfile): void {
    const dialogRef = this.dialog.open(OutletProfileEditorComponent, {
      data: { header: this.header, outletProfile: outletProfile, catererId: this.catererId },
      width: '400px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData(null);
    });
  }

  initializeFilter() {
    this.filter = new Filter(1, 10);
    this.filter.sorts = 'label';
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
    let f = this.catererId ? '(CatererId)==' + this.catererId + ',' : '';
    this.filter.filters = f + '(IsActive)==true,(Label)@=' + this.keyword;
    
    this.deliveryService.getOutletProfilesByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let outletProfiles = results.pagedData;

        outletProfiles.forEach((outletProfile, index, outletProfiles) => {
          (<any>outletProfile).index = index + 1;
        });


        this.rowsCache = [...outletProfiles];
        this.rows = outletProfiles;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve records from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  clearFilterAndPagedResult() {
    this.initializeFilter();
    this.initializePagedResult();
    this.table.offset = 0;
  }

  onSearchChanged(value: string) {
    this.keyword = value;
    this.clearFilterAndPagedResult();
    this.loadData(null);
  }

  newOutletProfile() {
    this.header = 'New Outlet Profile';
    this.editedOutletProfile = new OutletProfile();
    this.editedOutletProfile.catererId = this.catererId;
    this.openDialog(this.editedOutletProfile);
  }


  editOutletProfile(row: OutletProfile) {
    this.editedOutletProfile = row;
    this.header = 'Edit Outlet Profile';
    this.openDialog(this.editedOutletProfile);
  }

  deleteOutletProfile(row: OutletProfile) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.label + '\" outletProfile?', DialogType.confirm, () => this.deleteOutletProfileHelper(row));
  }


  deleteOutletProfileHelper(row: OutletProfile) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.deliveryService.deleteOutletProfile(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the outletProfile.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  get canManageOutletProfiles() {
    return true; //this.accountService.userHasPermission(Permission.manageOutletProfilesPermission)
  }

}
