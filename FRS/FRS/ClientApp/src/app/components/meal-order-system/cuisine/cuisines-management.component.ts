import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { Cuisine } from 'src/app/models/meal-order/cuisine.model';
import { CuisineEditorComponent } from './cuisine-editor.component';
import { DishService } from '../../../services/meal-order/dish.service';


@Component({
  selector: 'cuisines-management',
  templateUrl: './cuisines-management.component.html',
  styleUrls: ['./cuisines-management.component.css']
})
export class CuisinesManagementComponent implements OnInit {
  columns: any[] = [];
  rows: Cuisine[] = [];
  rowsCache: Cuisine[] = [];
  allPermissions: Permission[] = [];
  editedCuisine: Cuisine;
  sourceCuisine: Cuisine;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';
  @Input() isHideHeader: boolean;
  @Input() catererId: string;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('flagTemplate')
  flagTemplate: TemplateRef<any>;

  @ViewChild('cuisineEditor')
  cuisineEditor: CuisineEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private dishService: DishService, public dialog: MatDialog) {
  }

  openDialog(cuisine: Cuisine): void {
    const dialogRef = this.dialog.open(CuisineEditorComponent, {
      data: { header: this.header, cuisine: cuisine },
      width: '400px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData(null);
    });
  }

  initializeFilter() {
    this.filter = new Filter(1, 10);
    this.filter.sorts = 'name';
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
      { prop: 'name', name: 'Name' },
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
    this.filter.filters = '(IsActive)==true,(Name)@=' + this.keyword;
    
    this.dishService.getCuisinesByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let cuisines = results.pagedData;

        cuisines.forEach((cuisine, index, cuisines) => {
          (<any>cuisine).index = index + 1;
        });


        this.rowsCache = [...cuisines];
        this.rows = cuisines;

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

  newCuisine() {
    this.header = 'New Cuisine';
    this.editedCuisine = new Cuisine();
    this.openDialog(this.editedCuisine);
  }


  editCuisine(row: Cuisine) {
    this.editedCuisine = row;
    this.header = 'Edit Cuisine';
    this.openDialog(this.editedCuisine);
  }

  deleteCuisine(row: Cuisine) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\"?', DialogType.confirm, () => this.deleteCuisineHelper(row));
  }


  deleteCuisineHelper(row: Cuisine) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.dishService.deleteCuisine(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the cuisine.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  get canManageCuisines() {
    return true; //this.accountService.userHasPermission(Permission.manageCuisinesPermission)
  }

}
