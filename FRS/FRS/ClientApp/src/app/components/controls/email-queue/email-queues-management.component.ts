import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { AssetService } from 'src/app/services/asset.service';
import { FileService } from 'src/app/services/file.service';
import { EmailQueue } from 'src/app/models/email-queue.model';
import { EmailQueueService } from 'src/app/services/emailqueue.service';
import { DateTimeOnlyPipe } from 'src/app/pipes/datetime.pipe';


@Component({
  selector: 'email-queues-management',
  templateUrl: './email-queues-management.component.html',
  styleUrls: ['./email-queues-management.component.css']
})
export class EmailQueuesManagementComponent implements OnInit {
  columns: any[] = [];
  rows: EmailQueue[] = [];
  rowsCache: EmailQueue[] = [];
  allPermissions: Permission[] = [];
  editedEmailQueue: EmailQueue;
  sourceEmailQueue: EmailQueue;
  editingEmailQueueName: { name: string };
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  subjects: string[] = [];

  @ViewChild('iconTemplate')
  iconTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;
  
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private fileService: FileService, private emailQueueService: EmailQueueService, public dialog: MatDialog) {
  }

  initializeFilter() {
    this.filter = new Filter(1, 10);
    this.filter.sorts = '-createdDate';
    this.filter.filters = '';
    this.filter.page = 1;

    this.subjects.push("Tappee Payment Invoice");
    this.subjects.push("Contact Us");
    this.subjects.push("Confirm your Account");

    
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
      { prop: 'toEmail', name: 'To' },
      { prop: 'fromEmail', name: 'From' },
      { prop: 'subject', name: 'Subject' },
      { prop: 'body', name: 'Body' },
      { prop: 'isSent', name: 'Is Sent' },
      { prop: 'isFailed', name: 'Is Failed' },
      { prop: 'sentDate', name: 'Sent On', pipe: new DateTimeOnlyPipe('en-SG') },
      { prop: 'createdDate', name: 'Created On', pipe: new DateTimeOnlyPipe('en-SG') }
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
    this.filter.filters = '(IsActive)==true,(FromEmail|ToEmail|Subject)@=' + this.keyword;
    
    this.emailQueueService.getEmailQueuesByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let pagedResults = results.pagedData;

        pagedResults.forEach((res, index, ress) => {
          (<any>res).index = index + 1;
        });


        this.rowsCache = [...pagedResults];
        this.rows = pagedResults;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve records from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }


  onSearchChanged(value: string) {
    //this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.name, r.description));
    this.keyword = value;
    this.loadData(null);
  }

  onSubjectChanged(value: any) {
    console.log(value);
    this.keyword = value.value;
    this.loadData(null);
  }
  get canManageEmailQueues() {
    return this.accountService.userHasPermission(Permission.viewEmailQueuesPermission)
  }

}
