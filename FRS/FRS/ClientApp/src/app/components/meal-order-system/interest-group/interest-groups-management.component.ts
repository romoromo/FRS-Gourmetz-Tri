import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { InterestGroup } from 'src/app/models/meal-order/interest-group.model';
import { InterestGroupEditorComponent } from './interest-group-editor.component';
import { DishService } from 'src/app/services/meal-order/dish.service';
import { StudentService } from 'src/app/services/meal-order/student.service';
import { InterestGroupEmailComponent } from './interest-group-email.component';


@Component({
  selector: 'interest-groups-management',
  templateUrl: './interest-groups-management.component.html',
  styleUrls: ['./interest-groups-management.component.css']
})
export class InterestGroupsManagementComponent implements OnInit {
  columns: any[] = [];
  rows: InterestGroup[] = [];
  rowsCache: InterestGroup[] = [];
  allPermissions: Permission[] = [];
  editedInterestGroup: InterestGroup;
  sourceInterestGroup: InterestGroup;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @Input() isHideHeader: boolean;

  @Input() outletId: string;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('flagTemplate')
  flagTemplate: TemplateRef<any>;

  @ViewChild('interestGroupEditor')
  interestGroupEditor: InterestGroupEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private studentService: StudentService, public dialog: MatDialog) {
  }

  openDialog(interestGroup: InterestGroup): void {
    const dialogRef = this.dialog.open(InterestGroupEditorComponent, {
      data: { header: this.header, interestGroup: interestGroup },
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
    
    this.studentService.getInterestGroupsByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let interestGroups = results.pagedData;

        interestGroups.forEach((interestGroup, index, interestGroups) => {
          (<any>interestGroup).index = index + 1;
        });


        this.rowsCache = [...interestGroups];
        this.rows = interestGroups;

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

  newInterestGroup() {
    this.header = 'New Interest Group';
    this.editedInterestGroup = new InterestGroup();
    this.openDialog(this.editedInterestGroup);
  }


  editInterestGroup(row: InterestGroup) {
    this.editedInterestGroup = row;
    this.header = 'Edit Interest Group';
    this.openDialog(this.editedInterestGroup);
  }

  deleteInterestGroup(row: InterestGroup) {
    this.alertService.showDialog('Are you sure you want to delete \"' + row.name + '\"?', DialogType.confirm, () => this.deleteInterestGroupHelper(row));
  }


  deleteInterestGroupHelper(row: InterestGroup) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.studentService.deleteInterestGroup(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the Interest Group.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  sendEmail(row: InterestGroup) {
    const dialogRef = this.dialog.open(InterestGroupEmailComponent, {
      data: { header: "Send Email", id: row.id, outletId: this.outletId },
      width: '400px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
    });
  }

  get canManageInterestGroups() {
    return true; //this.accountService.userHasPermission(Permission.manageInterestGroupsPermission)
  }

}
