import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { EmailTemplateEditorComponent } from './email-template-editor.component';
import { EmailService } from 'src/app/services/meal-order/email.service';
import { EmailTemplate } from 'src/app/models/meal-order/email-template';


@Component({
  selector: 'email-templates-management',
  templateUrl: './email-templates-management.component.html',
  styleUrls: ['./email-templates-management.component.css']
})
export class EmailTemplatesManagementComponent implements OnInit {
  columns: any[] = [];
  rows: EmailTemplate[] = [];
  rowsCache: EmailTemplate[] = [];
  allPermissions: Permission[] = [];
  editedEmailTemplate: EmailTemplate;
  sourceEmailTemplate: EmailTemplate;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('flagTemplate')
  flagTemplate: TemplateRef<any>;

  @ViewChild('outletEditor')
  outletEditor: EmailTemplateEditorComponent;

  @ViewChild('emailTemplatesTable') table: any;

  header: string;
  @Input() isHideHeader: boolean;
  @Input() outletId: string;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private emailService: EmailService, public dialog: MatDialog) {
  }

  openDialog(emailTemplate: EmailTemplate): void {
    const dialogRef = this.dialog.open(EmailTemplateEditorComponent, {
      data: { header: this.header, emailTemplate: emailTemplate, outletId: this.outletId },
      width: '700px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData(null);
    });
  }

  initializeFilter() {
    this.filter = new Filter(1, 10);
    this.filter.sorts = 'subject';
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
      { prop: 'subject', name: 'Subject' },
      { prop: 'body', name: 'Body' },
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
    this.filter.filters = '(IsActive)==true,(Subject)@=' + this.keyword + ',(OutletId)==' + (this.outletId ? this.outletId : '');
    
    this.emailService.getEmailTemplatesByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let emailTemplates = results.pagedData;

        emailTemplates.forEach((emailTemplate, index, emailTemplates) => {
          (<any>emailTemplate).index = index + 1;
        });


        this.rowsCache = [...emailTemplates];
        this.rows = emailTemplates;

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

  newEmailTemplate() {
    this.header = 'New Template';
    this.editedEmailTemplate = new EmailTemplate();
    this.editedEmailTemplate.outletId = this.outletId;
    this.openDialog(this.editedEmailTemplate);
  }


  editEmailTemplate(row: EmailTemplate) {
    this.editedEmailTemplate = row;
    this.header = 'Edit Template';
    this.openDialog(this.editedEmailTemplate);
  }

  deleteEmailTemplate(row: EmailTemplate) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.subject + '\" template?', DialogType.confirm, () => this.deleteEmailTemplateHelper(row));
  }


  deleteEmailTemplateHelper(row: EmailTemplate) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.emailService.deleteEmailTemplate(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the template.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  sentTestEmail(row: EmailTemplate) {
    this.alertService.showDialog('Enter email:', DialogType.prompt, (val) => {
      if (!val) return;
      this.emailService.sendTestEmail(row.id, val)
        .subscribe(results => {
          this.alertService.stopLoadingMessage();
          this.alertService.showMessage("Success", `Emails sent!`, MessageSeverity.success);
        },
          error => {
            this.alertService.stopLoadingMessage();
            this.alertService.showStickyMessage("Sending Error", `An error occured while sending emails.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
              MessageSeverity.error);
          });
    }, () => {
    });
  }

  get canManageEmailTemplates() {
    return this.accountService.userHasPermission(Permission.manageMOSOutletMgtEmailTemplatesPermission)
  }

}
