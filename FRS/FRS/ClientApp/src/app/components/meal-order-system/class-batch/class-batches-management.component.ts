import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { ClassBatch } from 'src/app/models/meal-order/class-batch.model';
import { ClassBatchEditorComponent } from './class-batch-editor.component';
import { ClassService } from 'src/app/services/meal-order/class.service';


@Component({
  selector: 'class-batches-management',
  templateUrl: './class-batches-management.component.html',
  styleUrls: ['./class-batches-management.component.css']
})
export class ClassBatchesManagementComponent implements OnInit {
  columns: any[] = [];
  rows: ClassBatch[] = [];
  rowsCache: ClassBatch[] = [];
  allPermissions: Permission[] = [];
  editedClassBatch: ClassBatch;
  sourceClassBatch: ClassBatch;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @Input() isHideHeader: boolean;

  @Input() outletId: string;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('classBatchEditor')
  classBatchEditor: ClassBatchEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private classService: ClassService, public dialog: MatDialog) {
  }

  openDialog(classBatch: ClassBatch): void {
    const dialogRef = this.dialog.open(ClassBatchEditorComponent, {
      data: { header: this.header, classBatch: classBatch },
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
      { prop: 'name', name: gT('common.Name'), width: 200 },
      { prop: 'year', name: 'Year', width: 200 },
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
    this.filter.filters = '(IsActive)==true,(Name)@=' + this.keyword + ',(InstitutionId)==' + this.accountService.currentUser.institutionId;
    
    this.classService.getClassBatchesByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let classBatches = results.pagedData;

        classBatches.forEach((classBatch, index, classBatches) => {
          (<any>classBatch).index = index + 1;
        });


        this.rowsCache = [...classBatches];
        this.rows = classBatches;

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

  newClassBatch() {
    this.header = 'New Class Batch';
    this.editedClassBatch = new ClassBatch();
    this.openDialog(this.editedClassBatch);
  }


  editClassBatch(row: ClassBatch) {
    this.editedClassBatch = row;
    this.header = 'Edit Class Batch';
    this.openDialog(this.editedClassBatch);
  }

  deleteClassBatch(row: ClassBatch) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" class batch?', DialogType.confirm, () => this.deleteClassBatchHelper(row));
  }


  deleteClassBatchHelper(row: ClassBatch) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.classService.deleteClassBatch(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the class batch.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  get canManageClassBatches() {
    return true; //this.accountService.userHasPermission(Permission.manageClassBatchesPermission)
  }

}
