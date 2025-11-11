import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { Class } from 'src/app/models/meal-order/class.model';
import { ClassEditorComponent } from './class-editor.component';
import { ClassService } from 'src/app/services/meal-order/class.service';
import { ClassTransferComponent } from './class-transfer/class-transfer.component';
import { MealCollectionTypeList } from 'src/app/helpers/enums';

@Component({
  selector: 'classes-management',
  templateUrl: './classes-management.component.html',
  styleUrls: ['./classes-management.component.css']
})
export class ClassesManagementComponent implements OnInit {
  columns: any[] = [];
  rows: Class[] = [];
  rowsCache: Class[] = [];
  allPermissions: Permission[] = [];
  editedClass: Class;
  sourceClass: Class;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @Input() isHideHeader: boolean;

  @Input() outletId: string;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('classModelEditor')
  classModelEditor: ClassEditorComponent;
  header: string;

  @ViewChild('mealCollectionTypeTemplate')
  mealCollectionTypeTemplate: TemplateRef<any>;

  mealCollectionTypes = MealCollectionTypeList;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private classService: ClassService, public dialog: MatDialog) {
  }

  openDialog(classModel: Class): void {
    const dialogRef = this.dialog.open(ClassEditorComponent, {
      data: { header: this.header, classModel: classModel, outletId: this.outletId },
      width: '800px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData(null);
    });
  }

  openDialogTransfer(classModel: Class): void {
    const dialogRef = this.dialog.open(ClassTransferComponent, {
      data: { classModel: classModel, outletId: this.outletId },
      width: '500px',
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
      { prop: 'classLevelName', name: 'Class Level', width: 200 },
      {
        prop: 'mealCollectionType',
        name: 'Meal Colection',
        cellTemplate: this.mealCollectionTypeTemplate
      },
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
    let f = this.outletId ? '(classOutletId)==' + this.outletId + ',' : '';
    this.filter.filters = f + '(IsActive)==true,(Name)@=' + this.keyword;
    
    this.classService.getClassesByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let classModels = results.pagedData;

        classModels.forEach((classModel, index, classModels) => {
          (<any>classModel).index = index + 1;
        });


        this.rowsCache = [...classModels];
        this.rows = classModels;

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

  newClass() {
    this.header = 'New Class';
    this.editedClass = new Class();
    this.openDialog(this.editedClass);
  }


  editClass(row: Class) {
    this.editedClass = row;
    this.header = 'Edit Class';
    this.openDialog(this.editedClass);
  }

  deleteClass(row: Class) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" class?', DialogType.confirm, () => this.deleteClassHelper(row));
  }


  deleteClassHelper(row: Class) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.classService.deleteClass(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the class.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  get canManageClasses() {
    return this.accountService.userHasPermission(Permission.manageMOSOutletMgtClassesPermission)
  }

  getMealCollectionTypeLabel(id: number): string {
    const type = this.mealCollectionTypes.find(t => t.id == id);
    return type ? type.label : '-';
  }

}
