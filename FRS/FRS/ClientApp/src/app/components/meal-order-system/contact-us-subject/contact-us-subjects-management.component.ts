import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { ContactUsSubjectEditorComponent } from './contact-us-subject-editor.component';
import { DeliveryService } from '../../../services/meal-order/delivery.service';
import { ContactUsSubject } from 'src/app/models/meal-order/contact-us-subject.model';
import { EmailService } from 'src/app/services/meal-order/email.service';


@Component({
  selector: 'contact-us-subjects-management',
  templateUrl: './contact-us-subjects-management.component.html',
  styleUrls: ['./contact-us-subjects-management.component.css']
})
export class ContactUsSubjectsManagementComponent implements OnInit {
  columns: any[] = [];
  rows: ContactUsSubject[] = [];
  rowsCache: ContactUsSubject[] = [];
  allPermissions: Permission[] = [];
  editedContactUsSubject: ContactUsSubject;
  sourceContactUsSubject: ContactUsSubject;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('flagTemplate')
  flagTemplate: TemplateRef<any>;

  @ViewChild('outletEditor')
  outletEditor: ContactUsSubjectEditorComponent;

  @ViewChild('contactUsSubjectsTable') table: any;

  header: string;
  @Input() isHideHeader: boolean;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private emailService: EmailService, public dialog: MatDialog) {
  }

  openDialog(contactUsSubject: ContactUsSubject): void {
    const dialogRef = this.dialog.open(ContactUsSubjectEditorComponent, {
      data: { header: this.header, contactUsSubject: contactUsSubject },
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
      { prop: 'description', name: 'Description' },
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
    
    this.emailService.getContactUsSubjectsByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let contactUsSubjects = results.pagedData;

        contactUsSubjects.forEach((contactUsSubject, index, contactUsSubjects) => {
          (<any>contactUsSubject).index = index + 1;
        });


        this.rowsCache = [...contactUsSubjects];
        this.rows = contactUsSubjects;

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

  newContactUsSubject() {
    this.header = 'New Subject';
    this.editedContactUsSubject = new ContactUsSubject();
    this.openDialog(this.editedContactUsSubject);
  }


  editContactUsSubject(row: ContactUsSubject) {
    this.editedContactUsSubject = row;
    this.header = 'Edit Subject';
    this.openDialog(this.editedContactUsSubject);
  }

  deleteContactUsSubject(row: ContactUsSubject) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" subject?', DialogType.confirm, () => this.deleteContactUsSubjectHelper(row));
  }


  deleteContactUsSubjectHelper(row: ContactUsSubject) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.emailService.deleteContactUsSubject(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the subject.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  get canManageContactUsSubjects() {
    return true; //this.accountService.userHasPermission(Permission.manageMOSOrderMgtContactUsSubjectsPermission)
  }

}
