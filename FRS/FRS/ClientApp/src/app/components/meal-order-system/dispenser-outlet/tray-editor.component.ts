import { Component, Inject, Input, OnInit } from "@angular/core";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material";
import { PlcModel } from "../../../models/meal-order/plc.model";
import { TrayModel } from "../../../models/meal-order/TrayModel";
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

  messageErrors: any[] = [];
  isSaving = false;
  loadingIndicator = false;
  plcs: PlcModel[] = [];
  selectedPlc: PlcModel | null = null;
  filter: Filter;
  trayEdit: TrayModel = new TrayModel();

  @Input() outletId: string;

  constructor(
    public dialogRef: MatDialogRef<TrayEditorComponent>,
    private alertService: AlertService,
    private classService: ClassService,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.outletId = (data && typeof data.outletId !== 'undefined') ? data.outletId : null;
    this.filter = new Filter();
  }

  ngOnInit(): void {
    this.filter.page = -1;
    this.loadPlcs();
  }

  loadPlcs() {
    this.alertService.startLoadingMessage();
    this.loadingIndicator = true;

    let f = this.outletId ? `(OutletId)==${this.outletId},` : '';
    this.filter.filters = f + '(IsActive)==true';

    this.classService.getPLCByFilter(this.filter).subscribe({
      next: results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;
        this.plcs = results.pagedData || [];

        if (this.data && this.data.tray) {
          this.trayEdit = { ...this.data.tray };

          const existingPlc = this.plcs.find(x => x.id === this.trayEdit.plcId);
          if (existingPlc) {
            this.selectedPlc = existingPlc;
          }
        } else {
          // Default new tray
          this.trayEdit = new TrayModel();
        }
      },
      error: error => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;
        this.alertService.showStickyMessage(
          "Load Error",
          `Unable to retrieve PLC list from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
          MessageSeverity.error
        );
      }
    });
  }

  onPLCSelected(event: any) {
    const plcId = (event && event.value !== undefined) ? event.value : event;
    const selected = this.plcs.find(x => x.id === plcId);
    if (selected) {
      this.selectedPlc = selected;
      this.trayEdit.plcId = selected.id;
      this.trayEdit.framework = selected.framework;
      this.trayEdit.ipAddress = selected.ipAddress;
    }
  }

  save() {
    this.dialogRef.close(this.trayEdit);
  }

  cancel() {
    this.dialogRef.close(null);
  }
}
