import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { ContactUsDetailEditorComponent } from './contact-us-detail-editor.component';
import { EmailService } from 'src/app/services/meal-order/email.service';
import { ContactUsDetail } from 'src/app/models/meal-order/contact-us-subject.model';


@Component({
  selector: 'contact-us-details-management',
  templateUrl: './contact-us-details-management.component.html',
  styleUrls: ['./contact-us-details-management.component.css']
})
export class ContactUsDetailsManagementComponent implements OnInit {
  columns: any[] = [];
  rows: ContactUsDetail[] = [];
  rowsCache: ContactUsDetail[] = [];
  allPermissions: Permission[] = [];
  editedContactUsDetail: ContactUsDetail;
  sourceContactUsDetail: ContactUsDetail;
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
  outletEditor: ContactUsDetailEditorComponent;

  @ViewChild('contactUsDetailsTable') table: any;

  header: string;
  @Input() isHideHeader: boolean;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private emailService: EmailService, public dialog: MatDialog) {
  }

  openDialog(contactUsDetail: ContactUsDetail): void {
    const dialogRef = this.dialog.open(ContactUsDetailEditorComponent, {
      data: { header: this.header, contactUsDetail: contactUsDetail },
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
      { prop: 'contactUsSubjectName', name: 'Subject' },
      { prop: 'contactUsSubjectDescription', name: 'Subject Desc' },
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
    this.filter.filters = '(IsActive)==true,(Label)@=' + this.keyword;
    
    this.emailService.getContactUsDetailsByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let contactUsDetails = results.pagedData;

        contactUsDetails.forEach((contactUsDetail, index, contactUsDetails) => {
          (<any>contactUsDetail).index = index + 1;
        });


        this.rowsCache = [...contactUsDetails];
        this.rows = contactUsDetails;

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

  newContactUsDetail() {
    this.header = 'New Contact Us Detail';
    this.editedContactUsDetail = new ContactUsDetail();
    this.openDialog(this.editedContactUsDetail);
  }


  editContactUsDetail(row: ContactUsDetail) {
    this.editedContactUsDetail = row;
    this.header = 'Edit Contact Us Detail';
    this.openDialog(this.editedContactUsDetail);
  }

  deleteContactUsDetail(row: ContactUsDetail) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.label + '\" ?', DialogType.confirm, () => this.deleteContactUsDetailHelper(row));
  }


  deleteContactUsDetailHelper(row: ContactUsDetail) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.emailService.deleteContactUsDetail(row.id)
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

  get canManageContactUsDetails() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtContactUsQuestionsPermission)
  }

}
