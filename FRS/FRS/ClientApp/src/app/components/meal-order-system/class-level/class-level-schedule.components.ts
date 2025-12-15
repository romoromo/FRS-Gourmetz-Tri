import { Component, Inject } from "@angular/core";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material";
import { ClassLevelScheduleModel } from "src/app/models/meal-order/class-level-schedule.model";
import { MealService } from "src/app/services/meal-order/meal.service";

export const DAYS = [
  { key: "monday", label: "Monday" },
  { key: "tuesday", label: "Tuesday" },
  { key: "wednesday", label: "Wednesday" },
  { key: "thursday", label: "Thursday" },
  { key: "friday", label: "Friday" },
  { key: "saturday", label: "Saturday" },
  { key: "sunday", label: "Sunday" },
];

@Component({
  selector: "class-level-schedule",
  templateUrl: "./class-level-schedule.components.html",
  styleUrls: ["./class-level-schedule.components.css"],
})
export class ClassLevelSchedule {
  public formResetToggle = true;
  mealPeriods: any[];
  mealSessions: any[];
  scheduleRows: ClassLevelScheduleModel[];
  days = DAYS;
  classLevelName: string;
  classLevelId: number;
  isEdit = false;

  constructor(
    public dialogRef: MatDialogRef<ClassLevelSchedule>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private mealService: MealService
  ) {
    const outletId = data ? data.outletId : null;
    this.classLevelName = data.classLevelName;
    this.classLevelId = data.classLevelId;
    
    this.getMealSessionLite(outletId);
  }

  getMealSessionLite(outletId: string) {
    this.mealService.getMealSessionLite(outletId).subscribe((r) => {
      this.mealPeriods = (r || []).map((p: any) => ({
        ...p,
        details: p.details || [],
      }));
      console.log("mealPeriods", this.mealPeriods);
      this.buildScheduleRows();
    });
  }

  buildScheduleRows() {
    this.scheduleRows = this.mealPeriods.map((p) => ({
      periodId: p.id,
      periodName: p.name,
      sessions: this.days.reduce((acc, d) => {
        acc[d.key] = null;
        return acc;
      }, {} as any),
    }));
    console.log(this.scheduleRows);
  }

  cancel() {
    this.dialogRef.close(null);
  }

  save() {
    const payload = this.scheduleRows.map((row) => ({
      classLevelId: this.data.classLevelId,
      periodId: row.periodId,
      sessionMonday: row.sessions.monday,
      sessionTuesday: row.sessions.tuesday,
      sessionWednesday: row.sessions.wednesday,
      sessionThursday: row.sessions.thursday,
      sessionFriday: row.sessions.friday,
      sessionSaturday: row.sessions.saturday,
      sessionSunday: row.sessions.sunday,
    }));

    console.log("SAVE PAYLOAD", payload);
    this.dialogRef.close(null);
  }
}
