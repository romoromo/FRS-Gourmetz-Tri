import {
  Component,
  OnInit,
  OnDestroy,
  AfterViewInit,
  TemplateRef,
  ViewChild,
  Input,
  Inject,
} from "@angular/core";
import { Subscription } from "rxjs";
import { SearchBoxComponent } from "../../controls/search-box.component";
import { ModalDirective } from "ngx-bootstrap/modal";

import {
  AlertService,
  DialogType,
  MessageSeverity,
} from "../../../services/alert.service";
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from "../../../services/account.service";
import { Utilities } from "../../../services/utilities";
import {
  AssetComponentFilter,
  Filter,
  PagedResult,
} from "../../../models/sieve-filter.model";
import { Permission } from "../../../models/permission.model";
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from "@angular/material";
import { DishService } from "src/app/services/meal-order/dish.service";
import { StaffService } from "../../../services/meal-order/staff.service";
import { DeliveryService } from "../../../services/meal-order/delivery.service";
import { saveAs } from "file-saver";
import * as moment from "moment";
import { AssetComponent } from "src/app/models/meal-order/asset-component.model";
import { AssetComponentEditorComponent } from "./asset-component-editor.component";

@Component({
  selector: "asset-component-management",
  templateUrl: "./asset-component-management.component.html",
  styleUrls: ["./asset-component-management.component.css"],
})
export class AssetComponentManagementComponent implements OnInit, OnDestroy {
  private subscription: Subscription = new Subscription();
  columns: any[] = [];
  rows: AssetComponent[] = [];
  rowsCache: AssetComponent[] = [];
  allPermissions: Permission[] = [];
  editedAssetComponent: AssetComponent;
  sourceAssetComponent: AssetComponent;
  loadingIndicator: boolean;
  filter: AssetComponentFilter;
  pagedResult: PagedResult;
  keyword: string = "";

  @ViewChild("actionsTemplate")
  actionsTemplate: TemplateRef<any>;

  @ViewChild("flagTemplate")
  flagTemplate: TemplateRef<any>;

  @ViewChild("catererAssetEditor")
  catererAssetEditor: AssetComponentEditorComponent;

  @ViewChild("searchbox") searchbox: SearchBoxComponent;

  @ViewChild("catererAssetTable") table: any;

  @Input() catererId: string;

  header: string;
  constructor(
    private alertService: AlertService,
    private translationService: AppTranslationService,
    private accountService: AccountService,
    private deliveryService: DeliveryService,
    public dialog: MatDialog
  ) {}

  openDialog(catererAsset: AssetComponent): void {
    const dialogRef = this.dialog.open(AssetComponentEditorComponent, {
      data: { header: this.header, catererAsset: catererAsset },
      width: "400px",
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (!result || !result.isCancel) this.loadData(null);
    });
  }

  openQrDialog(asset: AssetComponent): void {
    const dialogRef = this.dialog.open(AssetComponentEditorComponent, {
      data: `${asset.id}`,
      width: "90vw",
    });

    dialogRef.afterClosed().subscribe((result) => {
      this.loadData(null);
    });
  }

  initializeFilter() {
    this.filter = new AssetComponentFilter(1, 10);
    this.filter.sorts = "id";
    this.filter.filters = "";
    this.filter.page = 1;
    this.filter.catererInfoId = this.catererId;
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
      { prop: "catererAssetCode", name: "Asset" },
      { prop: "description", name: "Description" },
      { prop: "qty", name: "Qty" },
      { prop: "remarks", name: "Remarks" },
      {
        name: "",
        width: 150,
        cellTemplate: this.actionsTemplate,
        resizeable: false,
        canAutoResize: false,
        sortable: false,
        draggable: false,
      },
    ];
  }

  ngOnInit() {
    this.initializeFilter();
    this.initializePagedResult();
    this.initializeTableDefinition();
    this.loadData();
  }

  ngOnDestroy() {
    this.alertService.resetStickyMessage();
    this.subscription.unsubscribe();
  }

  loadData(ev?: any) {
    this.alertService.startLoadingMessage();
    this.loadingIndicator = true;
    this.filter.pageSize = 10;

    if (ev) {
      this.filter.page = ev.offset + 1;
      if (ev.sorts) {
        this.filter.sorts =
          ev.sorts[0].dir == "desc" ? "-" + ev.sorts[0].prop : ev.sorts[0].prop;
      }
    }

    if (!this.keyword) this.keyword = "";
    this.filter.filters = "(IsActive)==true";

    this.subscription.add(
      this.deliveryService.getAssetComponentByFilter(this.filter).subscribe(
        (results) => {
          this.pagedResult = results;

          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          let catererAssets = results.pagedData;

          catererAssets.forEach((catererAsset, index, catererAssets) => {
            (<any>catererAsset).index = index + 1;
          });

          this.rowsCache = [...catererAssets];
          this.rows = catererAssets;
        },
        (error) => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage(
            "Load Error",
            `Unable to retrieve records from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(
              error
            )}"`,
            MessageSeverity.error
          );
        }
      )
    );
  }

  onSearchChanged(value: string) {
    this.keyword = value;
    this.loadData(null);
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

  newAssetComponent() {
    this.header = "New Asset Component";
    this.editedAssetComponent = new AssetComponent();
    this.editedAssetComponent.catererId = this.catererId;
    this.openDialog(this.editedAssetComponent);
  }

  editAssetComponent(row: AssetComponent) {
    this.editedAssetComponent = row;
    this.header = "Edit Asset Component";
    this.editedAssetComponent.catererId = this.catererId;
    this.openDialog(this.editedAssetComponent);
  }

  deleteAssetComponent(row: AssetComponent) {
    this.alertService.showDialog(
      'Are you sure you want to delete the "' +
        row.description +
        '" Asset Component?',
      DialogType.confirm,
      () => this.deleteAssetComponentHelper(row)
    );
  }

  deleteAssetComponentHelper(row: AssetComponent) {
    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.deliveryService.deleteAssetComponent(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the Asset Component.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }
}
