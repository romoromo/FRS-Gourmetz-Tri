import {
  Component,
  OnInit,
  AfterViewInit,
  TemplateRef,
  ViewChild,
  Input,
} from "@angular/core";
import { ModalDirective } from "ngx-bootstrap/modal";

import {
  AlertService,
  DialogType,
  MessageSeverity,
} from "../../../services/alert.service";
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from "../../../services/account.service";
import { Utilities } from "../../../services/utilities";
import { Filter, PagedResult } from "../../../models/sieve-filter.model";
import { Permission } from "../../../models/permission.model";
import { MatDialog } from "@angular/material";
import { ClassLevel } from "src/app/models/meal-order/class-level.model";
import { ClassLevelEditorComponent } from "./class-level-editor.component";
import { ClassService } from "src/app/services/meal-order/class.service";
import { ClassLevelSchedule } from "./class-level-schedule.components";

@Component({
  selector: "class-levels-management",
  templateUrl: "./class-levels-management.component.html",
  styleUrls: ["./class-levels-management.component.css"],
})
export class ClassLevelsManagementComponent implements OnInit {
  columns: any[] = [];
  rows: ClassLevel[] = [];
  rowsCache: ClassLevel[] = [];
  allPermissions: Permission[] = [];
  editedClassLevel: ClassLevel;
  sourceClassLevel: ClassLevel;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = "";

  @Input() isHideHeader: boolean;

  @Input() outletId: string;

  @Input() mealCollectionType: string | number;

  @ViewChild("actionsTemplate")
  actionsTemplate: TemplateRef<any>;

  @ViewChild("classLevelEditor")
  classLevelEditor: ClassLevelEditorComponent;
  header: string;
  constructor(
    private alertService: AlertService,
    private translationService: AppTranslationService,
    private accountService: AccountService,
    private classService: ClassService,
    public dialog: MatDialog
  ) {}

  openDialog(classLevel: ClassLevel): void {
    const dialogRef = this.dialog.open(ClassLevelEditorComponent, {
      data: {
        header: this.header,
        classLevel: classLevel,
        mealCollectionType: this.mealCollectionType,
      },
      width: "500px",
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe((result) => {
      this.loadData(null);
    });
  }

  initializeFilter() {
    this.filter = new Filter(1, 10);
    this.filter.sorts = "name";
    this.filter.filters = "";
    this.filter.page = 1;

    console.log("mealCollectionType : ", this.mealCollectionType);
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
      { prop: "name", name: gT("common.Name"), width: 200 },
      { prop: "outletName", name: "Outlet", width: 200 },
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

    if (
      !this.accountService.currentUser.institutionId ||
      this.accountService.currentUser.institutionId == "0"
    ) {
      this.columns.splice(1, 0, {
        prop: "institutionName",
        name: gT("roles.management.Institution"),
        width: 120,
      });
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
        this.filter.sorts =
          ev.sorts[0].dir == "desc" ? "-" + ev.sorts[0].prop : ev.sorts[0].prop;
      }
    }

    if (!this.keyword) this.keyword = "";
    let f = this.outletId ? "(OutletId)==" + this.outletId + "," : "";
    this.filter.filters =
      f +
      "(IsActive)==true,(Name)@=" +
      this.keyword +
      ",(InstitutionId)==" +
      this.accountService.currentUser.institutionId;

    this.classService.getClassLevelsByFilter(this.filter).subscribe(
      (results) => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let classLevels = results.pagedData;

        classLevels.forEach((classLevel, index, classLevels) => {
          (<any>classLevel).index = index + 1;
        });

        this.rowsCache = [...classLevels];
        this.rows = classLevels;
        console.log(this.rows);
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
    );
  }

  onSearchChanged(value: string) {
    this.keyword = value;
    this.loadData(null);
  }

  newClassLevel() {
    this.header = "New Class Level";
    this.editedClassLevel = new ClassLevel();
    this.editedClassLevel.outletId = this.outletId;
    this.openDialog(this.editedClassLevel);
  }

  editClassLevel(row: ClassLevel) {
    this.editedClassLevel = row;
    this.header = "Edit Class Level";
    this.openDialog(this.editedClassLevel);
  }

  deleteClassLevel(row: ClassLevel) {
    this.alertService.showDialog(
      'Are you sure you want to delete the "' + row.name + '" class level?',
      DialogType.confirm,
      () => this.deleteClassLevelHelper(row)
    );
  }

  deleteClassLevelHelper(row: ClassLevel) {
    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.classService.deleteClassLevel(row.id).subscribe(
      (results) => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
      (error) => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.alertService.showStickyMessage(
          "Delete Error",
          `An error occured while deleting the class level.\r\nError: "${Utilities.getHttpResponseMessage(
            error
          )}"`,
          MessageSeverity.error
        );
      }
    );
  }

  get canManageClassLevels() {
    return this.accountService.userHasPermission(
      Permission.manageMOSOutletMgtClassLevelsPermission
    );
  }

  openSchedule(row: ClassLevel) {
    const scheduleDialog = this.dialog.open(ClassLevelSchedule, {
      width: "1400px",
      data: {
        classLevelId: row.id,
        classLevelName: row.name,
        outletId: row.outletId,
      },
    });

    scheduleDialog.afterClosed().subscribe((result) => {
      console.log(result);
    });
  }
}
