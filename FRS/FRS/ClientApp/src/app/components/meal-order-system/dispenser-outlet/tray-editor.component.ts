import { Component, Inject, Input, OnInit } from "@angular/core";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material";
import { PlcModel } from "../../../models/meal-order/plc.model";
import { TryModel } from "../../../models/meal-order/TryModel";
import { Filter } from "../../../models/sieve-filter.model";
import { AlertService, MessageSeverity } from "../../../services/alert.service";
import { ClassService } from "../../../services/meal-order/class.service";
import { Utilities } from "../../../services/utilities";

@Component({
  selector: 'app-tray-editor',
  templateUrl: './tray-editor.component.html',
  styleUrls: ['./tray-editor.component.css']
})

export class TrayEditorComponent implements OnInit {
  messageErrors: any[] = []
  isSaving: boolean = false;
  loadingIndicator: boolean = false;
  plcs: PlcModel[] = [];
  selectedPlc: PlcModel | null = null;
  filter: Filter;
  tryEdit: TryModel = new TryModel();

  @Input() outletId: string;

  constructor(public dialogRef: MatDialogRef<TrayEditorComponent>, private alertService: AlertService, private classService: ClassService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.outletId) != typeof (undefined)) {
      this.outletId = data.outletId;
    }    
    this.filter = new Filter();
  }

  ngOnInit(): void {
    this.filter.page = -1;
    this.loadPlcs();
  }

  loadPlcs() {
    this.alertService.startLoadingMessage();
    this.loadingIndicator = true;


    let f = this.outletId ? '(OutletId)==' + this.outletId + ',' : '';
    this.filter.filters = f + '(IsActive)==true';

    this.classService.getPLCByFilter(this.filter)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;
        this.plcs = results.pagedData || [];
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve records from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  onPLCSelected(event: any) {
    const plcId = (event && event.value !== undefined) ? event.value : event;
    const selectedRows = this.plcs.find(x => x.id == plcId);
    if (selectedRows) {
      this.selectedPlc = selectedRows;
      this.tryEdit.plcId = selectedRows.id;
      this.tryEdit.framework = selectedRows.framework;
      this.tryEdit.ipAddress = selectedRows.ipAddress;
    }
  }

  save() {
    this.dialogRef.close(this.selectedPlc);
  }

  cancel() {
    this.dialogRef.close(null);
  }
}
