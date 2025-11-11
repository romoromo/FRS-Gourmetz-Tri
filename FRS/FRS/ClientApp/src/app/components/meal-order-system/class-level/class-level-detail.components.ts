import { Component, Inject, OnInit } from "@angular/core";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material";
import { ClassLevelDetail } from "src/app/models/meal-order/class-level.model";
import { MealService } from "src/app/services/meal-order/meal.service";

@Component({
  selector: "class-level-detail",
  templateUrl: "./class-level-detail.components.html",
  styleUrls: ["./class-level-detail.components.css"],
})
export class ClassLevelDetailComponent {
  mealPeriods: any[];
  mealSessions: any[];

  selectedPeriodId: string;
  selectedSessionId: string;
  existingDetails = [];
  constructor(
    public dialogRef: MatDialogRef<ClassLevelDetailComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private mealService: MealService
  ) {
    this.existingDetails = data.existingDetails || [];
    const outletId = data.classLevel ? data.classLevel.outletId : null;
    this.getMealSessionLite(outletId);
  }

getMealSessionLite(outletId: string) {
  this.mealService.getMealSessionLite(outletId).subscribe((r) => {
    this.mealPeriods = (r || []).map((p: any) => ({
      ...p,
      details: p.details || [],
    }));

    const existing = (this.existingDetails || []).map((d: any) => ({
        periodId: Number(d.periodId ? d.periodId : d["periodId"]),
      }));

    console.log('mealPeriods', this.mealPeriods);
    console.log('existing details (normalized)', existing);

    if (existing.length > 0) {
      this.mealPeriods = this.mealPeriods
          .map((period) => {
            const filteredDetails = (period.details || []).filter(
              (session: any) => {
                const pid = Number(period.id);
                const used = existing.some((e: any) => e.periodId === pid);

                return !used;
              }
            );

            return { ...period, details: filteredDetails };
          })
          .filter((period) => (period.details || []).length > 0);
    }

    if (this.selectedPeriodId) {
      this.getMealSessionDetailLite(this.selectedPeriodId);
    }
  });
}

  getMealSessionDetailLite(mealSessionId: string) {
    const mealSession = this.mealPeriods.find((ms) => ms.id === mealSessionId);
    this.mealSessions = mealSession ? mealSession.details : [];
  }

  onChangeMealSession(event: any) {
    this.getMealSessionDetailLite(event.value);
  }

  save() {
    if (!this.selectedPeriodId || !this.selectedSessionId) return;

    const period = this.mealPeriods.find((p) => p.id === this.selectedPeriodId);
    const session = this.mealSessions.find(
      (s) => s.id === this.selectedSessionId
    );

    const detail = {
      periodId: String(period.id),
      periodName: period.name,
      sessionId: String(session.id),
      sessionName: session.name,
    };

    this.dialogRef.close(detail);
  }

  cancel() {
    this.dialogRef.close(null);
  }
}