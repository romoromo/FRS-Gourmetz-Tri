import { Component, Input, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { PlcModel } from "../../../../models/meal-order/plc.model";
import { Permission } from "../../../../models/permission.model";
import { Filter, PagedResult } from "../../../../models/sieve-filter.model";
import { AlertService, DialogType, MessageSeverity } from "../../../../services/alert.service";
import { AppTranslationService } from "../../../../services/app-translation.service";
import { AccountService } from '../../../../services/account.service';
import { ClassService } from "../../../../services/meal-order/class.service";
import { MatDialog } from "@angular/material";
import { DispenserOutlet } from "../../../../models/meal-order/dispenser-outlet.model";
import { Utilities } from "../../../../services/utilities";
import { PlcEditorComponent } from "./plc-editor.component";
@Component({
  selector: 'plc-management',
  templateUrl: './plc-management.component.html',
  styleUrls: ['./plc-management.component.css']
})
export class PlcManagementComponent implements OnInit {
  columns: any[] = [];
  rows: PlcModel[] = [];
  rowsCache: PlcModel[] = [];
  allpermissions: Permission[] = [];
  editedPlc: PlcModel;
  sourcePlc: PlcModel;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @Input() isHideHeader: boolean;
  @Input() outletId: string;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('dispenserOutletEditor')
  plcEditor: PlcEditorComponent;
  header: string;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService, private classService: ClassService, public dialog: MatDialog) {
  }

  openDialog(plc: PlcModel): void {
    const dialogRef = this.dialog.open(PlcEditorComponent, {
      data: { header: this.header, plc: plc },
      width: '400px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData(null);
    });
  }

  initializeFilter() {
    this.filter = new Filter(1, 10);
    this.filter.sorts = 'IPAddress';
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
      { prop: 'ipAddress', name: 'IP Address', width: 200 },
      { prop: 'framework', name: 'Framework', width: 200 },
      { prop: 'totalNumber', name: 'Total Number', width: 200 },
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
    let f = this.outletId ? '(OutletId)==' + this.outletId + ',' : '';
    this.filter.filters = f + '(IsActive)==true,(IPAddress)@=' + this.keyword;

    this.classService.getPLCByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let plcs = results.pagedData;

        plcs.forEach((plc, index, plcs) => {
          (<any>plc).index = index + 1;
        });


        this.rowsCache = [...plcs];
        this.rows = plcs;

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

  newPLC() {
    this.header = 'New PLC';
    this.editedPlc = new PlcModel();
    this.editedPlc.outletId = this.outletId;
    this.openDialog(this.editedPlc);
  }


  editPLC(row: PlcModel) {
    this.editedPlc = row;
    this.header = 'Edit PLC';
    this.openDialog(this.editedPlc);
  }

  deletePLC(row: PlcModel) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.ipAddress + '\" PLC?', DialogType.confirm, () => this.deletePLCHelper(row));
  }


  deletePLCHelper(row: PlcModel) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.classService.deletePLC(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the PLC.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

}
