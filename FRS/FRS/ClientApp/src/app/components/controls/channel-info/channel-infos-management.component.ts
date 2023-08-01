import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { ChannelInfo } from 'src/app/models/channel-info.model';
import { ChannelInfoEditorComponent } from './channel-info-editor.component';
import { ChannelInfoService } from 'src/app/services/channel-info.service';
import { FileService } from 'src/app/services/file.service';


@Component({
  selector: 'channel-infos-management',
  templateUrl: './channel-infos-management.component.html',
  styleUrls: ['./channel-infos-management.component.css']
})
export class ChannelInfosManagementComponent implements OnInit {
  columns: any[] = [];
  rows: ChannelInfo[] = [];
  rowsCache: ChannelInfo[] = [];
  allPermissions: Permission[] = [];
  editedChannelInfo: ChannelInfo;
  sourceChannelInfo: ChannelInfo;
  editingChannelInfoName: { name: string };
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('channelInfoEditor')
  channelInfoEditor: ChannelInfoEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private fileService: FileService, private channelInfoService: ChannelInfoService, public dialog: MatDialog) {
  }

  openDialog(channelInfo: ChannelInfo): void {
    const dialogRef = this.dialog.open(ChannelInfoEditorComponent, {
      data: { header: this.header, channelInfo: channelInfo },
      width: '400px'
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
      { prop: 'number', name: 'Channel Id', width: 200 },
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
    
    this.channelInfoService.getChannelInfosByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let channelInfos = results.pagedData;

        channelInfos.forEach((channelInfo, index, channelInfos) => {
          (<any>channelInfo).index = index + 1;
        });


        this.rowsCache = [...channelInfos];
        this.rows = channelInfos;

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


  onEditorModalHidden() {
    this.editingChannelInfoName = null;
    this.channelInfoEditor.resetForm(true);
  }


  newChannelInfo() {
    //this.editingChannelInfoName = null;
    //this.sourceChannelInfo = null;
    //this.editedChannelInfo = this.channelInfoEditor.newChannelInfo();
    //this.editorModal.show();
    this.header = 'New Channel Info';
    this.editedChannelInfo = new ChannelInfo();
    this.openDialog(this.editedChannelInfo);
  }


  editChannelInfo(row: ChannelInfo) {
    //this.editingChannelInfoName = { name: row.name };
    //this.sourceChannelInfo = row;
    this.editedChannelInfo = row; //this.channelInfoEditor.editChannelInfo(row);
    //this.editorModal.show();

    this.header = 'Edit Channel Info';
    this.openDialog(this.editedChannelInfo);
  }

  deleteChannelInfo(row: ChannelInfo) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" channel info?', DialogType.confirm, () => this.deleteChannelInfoHelper(row));
  }


  deleteChannelInfoHelper(row: ChannelInfo) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.channelInfoService.deleteChannelInfo(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        //this.rowsCache = this.rowsCache.filter(item => item !== row)
        //this.rows = this.rows.filter(item => item !== row)
        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the channel info.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  getFileImage(path) {
    return this.fileService.getFile(path);
  }

  get canManageChannelInfos() {
    return true;// this.accountService.userHasPermission(Permission.manageChannelInfosPermission)
  }

}
