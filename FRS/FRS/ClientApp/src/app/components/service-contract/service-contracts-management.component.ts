import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../services/alert.service';
import { AppTranslationService } from "../../services/app-translation.service";
import { AccountService } from '../../services/account.service';
import { Utilities } from '../../services/utilities';
import { Filter, PagedResult, ServiceContractFilter } from '../../models/sieve-filter.model';
import { Permission } from '../../models/permission.model';
import { MatDatepickerInputEvent, MatDialog } from '@angular/material';
import { ServiceContract } from 'src/app/models/service-contract.model';
import { ServiceContractEditorComponent } from './service-contract-editor.component';
import { ServiceContractService } from 'src/app/services/service-contract.service';
import { FileService } from 'src/app/services/file.service';
import { AssetSelectorComponent } from '../common/asset-selector/asset-selector.component';
import { DateOnlyPipe } from 'src/app/pipes/datetime.pipe';
import { SearchBoxComponent } from '../controls/search-box.component';
import * as moment from 'moment';
import { saveAs } from 'file-saver';

@Component({
  selector: 'service-contracts-management',
  templateUrl: './service-contracts-management.component.html',
  styleUrls: ['./service-contracts-management.component.css']
})
export class ServiceContractsManagementComponent implements OnInit {
  columns: any[] = [];
  rows: ServiceContract[] = [];
  rowsCache: ServiceContract[] = [];
  allPermissions: Permission[] = [];
  editedServiceContract: ServiceContract;
  sourceServiceContract: ServiceContract;
  editingServiceContractName: { name: string };
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';
  start = new Date();
  end = new Date();

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('serviceContractEditor')
  serviceContractEditor: ServiceContractEditorComponent;

  @ViewChild('searchbox') searchbox: SearchBoxComponent;

  @ViewChild('serviceContractTable') table: any;

  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private fileService: FileService, private serviceContractService: ServiceContractService, public dialog: MatDialog) {
  }

  openDialog(serviceContract: ServiceContract): void {
    const dialogRef = this.dialog.open(ServiceContractEditorComponent, {
      data: { header: this.header, serviceContract: serviceContract },
      width: '800px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData(null);
    });
  }

  initializeFilter() {
    this.filter = new Filter(1, 10);
    this.filter.sorts = 'identifier';
    this.filter.filters = '';
    this.filter.page = 1;

    this.end.setDate(this.start.getDate() + 7);
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
      { prop: 'identifier', name: 'Contract No.', width: 200 },
      { prop: 'reference', name: 'Reference' },
      { prop: 'coverage', name: 'Coverage' },
      { prop: 'startDate', name: 'Start Date', pipe: new DateOnlyPipe('en-SG') },
      { prop: 'endDate', name: 'End Date', pipe: new DateOnlyPipe('en-SG') },
      //{ prop: 'details', name: 'Details' },
      { prop: 'renewalAlert', name: 'Renewal Alert (in Days)' },
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
    this.filter.filters = '(IsActive)==true,(Identifier|Reference|Coverage|Details)@=' + this.keyword + ',(ServiceContractDateRange)==' + this.start.toDateString() + '|' + this.end.toDateString();
    
    this.serviceContractService.getServiceContractsByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let serviceContracts = results.pagedData;

        serviceContracts.forEach((serviceContract, index, serviceContracts) => {
          (<any>serviceContract).index = index + 1;
        });


        this.rowsCache = [...serviceContracts];
        this.rows = serviceContracts;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve records from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  onChangeDate(type: string, event: MatDatepickerInputEvent<Date>) {
    if (type == 'start') {
      this.start = new Date(event.value);
    }

    if (type == 'end') {
      this.end = new Date(event.value);
    }

    this.loadData();
  }

  clearFilterAndPagedResult() {
    this.initializeFilter();
    this.initializePagedResult();
    this.table.offset = 0;
  }

  onSearch() {
    this.clearFilterAndPagedResult();
    this.loadData(null);
  }

  clearFilters() {
    this.start = new Date();
    this.end = new Date();
    this.keyword = '';
    this.clearFilterAndPagedResult();
    this.searchbox.clear();
  }

  onSearchChanged(value: string) {
    //this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.name, r.description));
    this.keyword = value;
    this.loadData(null);
  }


  onEditorModalHidden() {
    this.editingServiceContractName = null;
    this.serviceContractEditor.resetForm(true);
  }


  newServiceContract() {
    //this.editingServiceContractName = null;
    //this.sourceServiceContract = null;
    //this.editedServiceContract = this.serviceContractEditor.newServiceContract();
    //this.editorModal.show();
    this.header = 'New Service Contract';
    this.editedServiceContract = new ServiceContract();
    this.editedServiceContract.renewalAlert = 7;
    this.editedServiceContract.serviceContractAssets = [];
    this.openDialog(this.editedServiceContract);
  }


  editServiceContract(row: ServiceContract) {
    //this.editingServiceContractName = { name: row.name };
    //this.sourceServiceContract = row;
    this.editedServiceContract = row; //this.serviceContractEditor.editServiceContract(row);
    //this.editorModal.show();

    this.header = 'Edit Service Contract';
    this.openDialog(this.editedServiceContract);
  }

  deleteServiceContract(row: ServiceContract) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.identifier + '\" service contract?', DialogType.confirm, () => this.deleteServiceContractHelper(row));
  }


  deleteServiceContractHelper(row: ServiceContract) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.serviceContractService.deleteServiceContract(row.id)
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

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the service contract.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  downloadResults() {
    this.alertService.showDialog('Do you want to include the list of assets?', DialogType.confirm, () => this.proceedExport(true), () => this.proceedExport(false), 'Yes', 'No');

    
  }

  proceedExport(includeAssets) {
    this.alertService.startLoadingMessage("Exporting...");
    const fileName = moment().format('DDMMYYYY_hhmmss') + '_ServiceContracts.xlsx';
    let f = new Filter();
    f.page = 1;
    f.filters = this.filter.filters;
    f.sorts = this.filter.sorts;
    let serviceContractFilter = new ServiceContractFilter(includeAssets, f);
    this.serviceContractService.downloadServiceContractReport(serviceContractFilter).subscribe(
      data => {
        console.log(data);
        this.alertService.stopLoadingMessage();
        saveAs(data, fileName);
      },
      err => {
        this.alertService.stopLoadingMessage();
        alert("Problem while downloading the file.");

        console.error(err);
      }
    );
  }

  getFileImage(path) {
    return this.fileService.getFile(path);
  }

  get canManageServiceContracts() {
    return this.accountService.userHasPermission(Permission.manageServiceContractsPermission)
  }

}
