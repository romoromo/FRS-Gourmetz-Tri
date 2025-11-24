import { Component, OnInit, OnDestroy, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { Subscription } from 'rxjs';
import { SearchBoxComponent } from '../../controls/search-box.component';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { BentoBoxType } from 'src/app/models/meal-order/bento-box-type.model';
import { BentoBoxTypeEditorComponent } from './bento-box-type-editor.component';
import { DishService } from 'src/app/services/meal-order/dish.service';
import { StaffService } from '../../../services/meal-order/staff.service';
import { DeliveryService } from '../../../services/meal-order/delivery.service';


@Component({
  selector: 'bento-box-types-management',
  templateUrl: './bento-box-types-management.component.html',
  styleUrls: ['./bento-box-types-management.component.css']
})
export class BentoBoxTypesManagementComponent implements OnInit, OnDestroy {
  private subscription: Subscription = new Subscription();
  columns: any[] = [];
  rows: BentoBoxType[] = [];
  rowsCache: BentoBoxType[] = [];
  allPermissions: Permission[] = [];
  editedBentoBoxType: BentoBoxType;
  sourceBentoBoxType: BentoBoxType;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('flagTemplate')
  flagTemplate: TemplateRef<any>;

  @ViewChild('bentoBoxTypeEditor')
  bentoBoxTypeEditor: BentoBoxTypeEditorComponent;

  @ViewChild('searchbox') searchbox: SearchBoxComponent;

  @ViewChild('bentoBoxTypeTable') table: any;

  @Input() catererId: string;

  header: string;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private deliveryService: DeliveryService, public dialog: MatDialog) {
  }

  openDialog(bentoBoxType: BentoBoxType): void {
    const dialogRef = this.dialog.open(BentoBoxTypeEditorComponent, {
      data: { header: this.header, bentoBoxType: bentoBoxType },
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
      { prop: 'details', name: 'Details' },
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
    this.filter.filters = '(IsActive)==true,(Code)@=' + this.keyword + ',(InstitutionId)==' + this.accountService.currentUser.institutionId + ',(CatererInfoId)==' + this.catererId;
    
    this.subscription.add(this.deliveryService.getBentoBoxTypesByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let bentoBoxTypes = results.pagedData;

        bentoBoxTypes.forEach((bentoBoxType, index, bentoBoxTypes) => {
          (<any>bentoBoxType).index = index + 1;
        });


        this.rowsCache = [...bentoBoxTypes];
        this.rows = bentoBoxTypes;

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

  newBentoBoxType() {
    this.header = 'New Bento box type';
    this.editedBentoBoxType = new BentoBoxType();
    this.editedBentoBoxType.catererInfoId = this.catererId;
    this.openDialog(this.editedBentoBoxType);
  }


  editBentoBoxType(row: BentoBoxType) {
    this.editedBentoBoxType = row;
    this.editedBentoBoxType.catererInfoId = this.catererId;
    this.header = 'Edit Bento box type';
    this.openDialog(this.editedBentoBoxType);
  }

  deleteBentoBoxType(row: BentoBoxType) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.code + '\" Bento box type?', DialogType.confirm, () => this.deleteBentoBoxTypeHelper(row));
  }


  deleteBentoBoxTypeHelper(row: BentoBoxType) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.deliveryService.deleteBentoBoxType(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the Bento box type.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  get canManageBentoBoxTypes() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtBentoBoxTypesPermission)
  }

}
