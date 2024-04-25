import { Component, OnInit, OnDestroy, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { Subscription } from 'rxjs';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { Restriction } from 'src/app/models/meal-order/restriction.model';
import { RestrictionEditorComponent } from './restriction-editor.component';
import { RestrictionService } from 'src/app/services/meal-order/restriction.service';
import { SearchBoxComponent } from '../../controls/search-box.component';


@Component({
  selector: 'restrictions-management',
  templateUrl: './restrictions-management.component.html',
  styleUrls: ['./restrictions-management.component.css']
})
export class RestrictionsManagementComponent implements OnInit, OnDestroy {
  private subscription: Subscription = new Subscription();
  columns: any[] = [];
  rows: Restriction[] = [];
  rowsCache: Restriction[] = [];
  allPermissions: Permission[] = [];
  editedRestriction: Restriction;
  sourceRestriction: Restriction;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('flagTemplate')
  flagTemplate: TemplateRef<any>;

  @ViewChild('restrictionEditor')
  restrictionEditor: RestrictionEditorComponent;

  @ViewChild('searchbox') searchbox: SearchBoxComponent;

  @ViewChild('restrictionTable') table: any;

  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private restrictionService: RestrictionService, public dialog: MatDialog) {
  }

  openDialog(restriction: Restriction): void {
    const dialogRef = this.dialog.open(RestrictionEditorComponent, {
      data: { header: this.header, restriction: restriction },
      width: '600px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      if(!result || !result.isCancel)
      this.loadData(null);
    });
  }

  initializeFilter() {
    this.filter = new Filter(1, 10);
    this.filter.sorts = 'code';
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
      { prop: 'code', name: 'Code' },
      { prop: 'label', name: 'Label' },
      { prop: 'restrictionTypeLabel', name: 'Restriction Label' },
      { prop: 'isHighPriority', name: 'Set Patient as High Priority', cellTemplate: this.flagTemplate },
      { prop: 'isIncludeHighPriorityFiltering', name: 'Include in High Priority Filtering', cellTemplate: this.flagTemplate },
      { prop: 'isAvailableDateSpecific', name: 'Available as Date-Specific', cellTemplate: this.flagTemplate },
      { prop: 'isMealFiltering', name: 'Date-Specific Single Meal Period Only', cellTemplate: this.flagTemplate },
      { prop: 'sequence', name: 'Sequence'},
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

  ngOnDestroy() {
    this.alertService.resetStickyMessage();
    this.subscription.unsubscribe();
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
    this.filter.filters = '(IsActive)==true,(Code|Label)@=' + this.keyword;
    
    this.subscription.add(this.restrictionService.getRestrictionsByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let restrictions = results.pagedData;

        restrictions.forEach((restriction, index, restrictions) => {
          (<any>restriction).index = index + 1;
        });


        this.rowsCache = [...restrictions];
        this.rows = restrictions;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve records from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }


  onSearchChanged(value: string) {
    this.keyword = value;
    //this.loadData(null);
  }

  clearFilterAndPagedResult() {
    this.initializeFilter();
    this.initializePagedResult();
    this.table.offset = 0;
  }

  onSearch() {
    this.clearFilterAndPagedResult();
    this.loadData(null);
  }

  newRestriction() {
    this.header = 'New Restriction Level';
    this.editedRestriction = new Restriction();
    this.openDialog(this.editedRestriction);
  }


  editRestriction(row: Restriction) {
    this.editedRestriction = row;
    this.header = 'Edit Restriction Level';
    this.openDialog(this.editedRestriction);
  }

  deleteRestriction(row: Restriction) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.code + '\" restriction?', DialogType.confirm, () => this.deleteRestrictionHelper(row));
  }


  deleteRestrictionHelper(row: Restriction) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.restrictionService.deleteRestriction(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the restriction.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  get canManageRestrictions() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtRestrictionsPermission)
  }

}
