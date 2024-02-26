import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { FaqDetailEditorComponent } from './faq-detail-editor.component';
import { FaqService } from 'src/app/services/meal-order/faq.service';
import { FaqDetail } from 'src/app/models/meal-order/faq-subject.model';


@Component({
  selector: 'faq-details-management',
  templateUrl: './faq-details-management.component.html',
  styleUrls: ['./faq-details-management.component.css']
})
export class FaqDetailsManagementComponent implements OnInit {
  columns: any[] = [];
  rows: FaqDetail[] = [];
  rowsCache: FaqDetail[] = [];
  allPermissions: Permission[] = [];
  editedFaqDetail: FaqDetail;
  sourceFaqDetail: FaqDetail;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('chargeByTemplate')
  chargeByTemplate: TemplateRef<any>;

  @ViewChild('flagTemplate')
  flagTemplate: TemplateRef<any>;

  @ViewChild('outletEditor')
  outletEditor: FaqDetailEditorComponent;

  @ViewChild('faqDetailsTable') table: any;

  header: string;
  @Input() isHideHeader: boolean;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private faqService: FaqService, public dialog: MatDialog) {
  }

  openDialog(faqDetail: FaqDetail): void {
    const dialogRef = this.dialog.open(FaqDetailEditorComponent, {
      data: { header: this.header, faqDetail: faqDetail },
      width: '600px',
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
      { prop: 'faqSubjectName', name: 'Subject' },
      { prop: 'faqSubjectDescription', name: 'Subject Desc' },
      { prop: 'label', name: 'Label' },
      { prop: 'description', name: 'Details' },
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
    this.filter.filters = '(IsActive)==true,(Label)@=' + this.keyword;
    
    this.faqService.getFaqDetailsByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let faqDetails = results.pagedData;

        faqDetails.forEach((faqDetail, index, faqDetails) => {
          (<any>faqDetail).index = index + 1;
        });


        this.rowsCache = [...faqDetails];
        this.rows = faqDetails;

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

  newFaqDetail() {
    this.header = 'New FAQ Detail';
    this.editedFaqDetail = new FaqDetail();
    this.openDialog(this.editedFaqDetail);
  }


  editFaqDetail(row: FaqDetail) {
    this.editedFaqDetail = row;
    this.header = 'Edit FAQ Detail';
    this.openDialog(this.editedFaqDetail);
  }

  deleteFaqDetail(row: FaqDetail) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.label + '\" ?', DialogType.confirm, () => this.deleteFaqDetailHelper(row));
  }


  deleteFaqDetailHelper(row: FaqDetail) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.faqService.deleteFaqDetail(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  get canManageFaqDetails() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtFaqDetailsPermission)
  }

}
