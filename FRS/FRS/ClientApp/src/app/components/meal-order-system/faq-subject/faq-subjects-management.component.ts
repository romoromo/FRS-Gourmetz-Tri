import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { FaqSubjectEditorComponent } from './faq-subject-editor.component';
import { DeliveryService } from '../../../services/meal-order/delivery.service';
import { FaqSubject } from 'src/app/models/meal-order/faq-subject.model';
import { FaqService } from 'src/app/services/meal-order/faq.service';


@Component({
  selector: 'faq-subjects-management',
  templateUrl: './faq-subjects-management.component.html',
  styleUrls: ['./faq-subjects-management.component.css']
})
export class FaqSubjectsManagementComponent implements OnInit {
  columns: any[] = [];
  rows: FaqSubject[] = [];
  rowsCache: FaqSubject[] = [];
  allPermissions: Permission[] = [];
  editedFaqSubject: FaqSubject;
  sourceFaqSubject: FaqSubject;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('flagTemplate')
  flagTemplate: TemplateRef<any>;

  @ViewChild('outletEditor')
  outletEditor: FaqSubjectEditorComponent;

  @ViewChild('faqSubjectsTable') table: any;
  expanded: any = {};

  header: string;
  @Input() isHideHeader: boolean;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private faqService: FaqService, public dialog: MatDialog) {
  }

  openDialog(faqSubject: FaqSubject): void {
    const dialogRef = this.dialog.open(FaqSubjectEditorComponent, {
      data: { header: this.header, faqSubject: faqSubject },
      width: '400px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData(null);
    });
  }

  initializeFilter() {
    this.filter = new Filter(1, 10);
    this.filter.sorts = 'order';
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
      { prop: 'name', name: 'Group Header', sortable: false },
      { prop: 'description', name: 'Description', sortable: false },
      { name: '', width: 250, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
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
    
    this.faqService.getFaqSubjectsByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let faqSubjects = results.pagedData;

        faqSubjects.forEach((faqSubject, index, faqSubjects) => {
          (<any>faqSubject).index = index + 1;
        });


        this.rowsCache = [...faqSubjects];
        this.rows = faqSubjects;

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

  newFaqSubject() {
    this.header = 'New FAQ Group Header';
    this.editedFaqSubject = new FaqSubject();
    this.openDialog(this.editedFaqSubject);
  }


  editFaqSubject(row: FaqSubject) {
    this.editedFaqSubject = row;
    this.header = 'Edit FAQ Group Header';
    this.openDialog(this.editedFaqSubject);
  }

  deleteFaqSubject(row: FaqSubject) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" FAQ group header?', DialogType.confirm, () => this.deleteFaqSubjectHelper(row));
  }


  deleteFaqSubjectHelper(row: FaqSubject) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.faqService.deleteFaqSubject(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the FAQ group header.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  toggleExpandRow(row) {
    console.log('Toggled Expand Row!', row);
    this.table.rowDetail.toggleExpandRow(row);
  }

  orderFaq(id: string, asc: boolean) {
    let currentRow = this.rows.find(e => e.id == id);
    let order = currentRow.order;

    if (asc) {
      if (order < 1) return;

      //let previousRow = this.rows.find(e => e.order < currentRow.order);

      let previousRow = this.rows
        .filter(row => row.order < currentRow.order)
        .reduce((prev, curr) => prev && prev.order > curr.order ? prev : curr, null);

      if (previousRow == null) {
        previousRow = this.rows
          .filter(row => row.id < currentRow.id)
          .reduce((prev, curr) => prev && prev.id > curr.id ? prev : curr, null);
      }

      if (previousRow != null) {
        currentRow.order = previousRow.order;
        previousRow.order = order;
      }

    } else {
      if (order >= this.rows.length) return;

      //let nextRow = this.rows.find(e => e.order > currentRow.order);
      let nextRow = this.rows
        .filter(row => row.order > currentRow.order)
        .reduce((prev, curr) => prev && prev.order < curr.order ? prev : curr, null);

      if (nextRow == null) {
        nextRow = this.rows
          .filter(row => row.id > currentRow.id)
          .reduce((prev, curr) => prev && prev.id < curr.id ? prev : curr, null);
      }

      if (nextRow != null) {
        currentRow.order = nextRow.order;
        nextRow.order = order;
      }
    }


    this.faqService.orderFaqSubject(id, asc)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.rows.sort((a, b) => a.order - b.order);
        this.rows == [...this.rows];
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Sorting Error", `An error occured while sorting.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  get canManageFaqSubjects() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtFaqSubjectsPermission)
  }

}
