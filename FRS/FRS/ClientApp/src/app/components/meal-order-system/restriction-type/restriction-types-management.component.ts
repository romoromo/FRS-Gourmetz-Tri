import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { RestrictionType } from 'src/app/models/meal-order/restriction-type.model';
import { RestrictionTypeEditorComponent } from './restriction-type-editor.component';
import { RestrictionService } from 'src/app/services/meal-order/restriction.service';


@Component({
  selector: 'restriction-types-management',
  templateUrl: './restriction-types-management.component.html',
  styleUrls: ['./restriction-types-management.component.css']
})
export class RestrictionTypesManagementComponent implements OnInit {
  columns: any[] = [];
  rows: RestrictionType[] = [];
  rowsCache: RestrictionType[] = [];
  allPermissions: Permission[] = [];
  editedRestrictionType: RestrictionType;
  sourceRestrictionType: RestrictionType;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('flagTemplate')
  flagTemplate: TemplateRef<any>;

  @ViewChild('restrictionTypeEditor')
  restrictionTypeEditor: RestrictionTypeEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private restrictionService: RestrictionService, public dialog: MatDialog) {
  }

  openDialog(restrictionType: RestrictionType): void {
    const dialogRef = this.dialog.open(RestrictionTypeEditorComponent, {
      data: { header: this.header, restrictionType: restrictionType },
      width: '600px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
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
      { prop: 'isHandledEMR', name: 'Handled EMR', cellTemplate: this.flagTemplate },
      { prop: 'isDishTypeCustomisation', name: 'Dish Type Customisation', cellTemplate: this.flagTemplate },
      { prop: 'isSpecialCondition', name: 'Special Condition', cellTemplate: this.flagTemplate },
      { prop: 'isCarriedForward', name: 'Carried Forward', cellTemplate: this.flagTemplate },
      { prop: 'isMealFiltering', name: 'Meal Filtering', cellTemplate: this.flagTemplate },
      { prop: 'sequence', name: 'Sequence' },
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
    this.filter.filters = '(IsActive)==true,(Code|Label)@=' + this.keyword + ',(InstitutionId)==' + this.accountService.currentUser.institutionId;
    
    this.restrictionService.getRestrictionTypesByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let restrictionTypes = results.pagedData;

        restrictionTypes.forEach((restrictionType, index, restrictionTypes) => {
          (<any>restrictionType).index = index + 1;
        });


        this.rowsCache = [...restrictionTypes];
        this.rows = restrictionTypes;

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

  newRestrictionType() {
    this.header = 'New Restriction Type';
    this.editedRestrictionType = new RestrictionType();
    this.openDialog(this.editedRestrictionType);
  }


  editRestrictionType(row: RestrictionType) {
    this.editedRestrictionType = row;
    this.header = 'Edit Restriction Type';
    this.openDialog(this.editedRestrictionType);
  }

  deleteRestrictionType(row: RestrictionType) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.code + '\" restriction type?', DialogType.confirm, () => this.deleteRestrictionTypeHelper(row));
  }


  deleteRestrictionTypeHelper(row: RestrictionType) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.restrictionService.deleteRestrictionType(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the restriction type.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  get canManageRestrictionTypes() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtRestrictionTypesPermission)
  }

}
