import { Component, Inject } from "@angular/core";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material";
import { DAYS } from "src/app/helpers/enums";
import { ScheduleDay } from "src/app/models/enums";
import {
  SaveClassLevelScheduleDto,
  ScheduleItemDto,
  ScheduleRowModel,
} from "src/app/models/meal-order/class-level-schedule.model";
import { ClassService } from "src/app/services/meal-order/class.service";
import { MealService } from "src/app/services/meal-order/meal.service";

@Component({
  selector: "class-level-schedule",
  templateUrl: "./class-level-schedule.components.html",
  styleUrls: ["./class-level-schedule.components.css"],
})
export class ClassLevelSchedule {
  public formResetToggle = true;
  days = DAYS;
  mealPeriods: any[] = [];
  scheduleRows: ScheduleRowModel[] = [];
  classLevelName: string;
  classLevelId: number;
  isSaving = false;

  constructor(
    public dialogRef: MatDialogRef<ClassLevelSchedule>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private mealService: MealService,
    private classService: ClassService
  ) {
    const outletId = data ? data.outletId : null;
    this.classLevelName = data.classLevelName;
    this.classLevelId = data.classLevelId;

    this.loadMealPeriods(outletId);
  }

  loadMealPeriods(outletId: string) {
    this.mealService.getMealSessionLite(outletId).subscribe((r) => {
      this.mealPeriods = (r || []).map((p: any) => ({
        ...p,
        details: p.details || [],
      }));

      console.log("mealPeriods", this.mealPeriods);
      this.buildRows();
      this.loadExistingSchedule();
    });
  }

  buildRows() {
    this.scheduleRows = this.mealPeriods.map((p) => ({
      periodId: p.id,
      periodName: p.name,
      sessions: {},
    }));
  }

  loadExistingSchedule() {
    this.classService
      .getClassLevelSchedule(this.classLevelId)
      .subscribe((existing) => {
        if (!existing || !existing.schedules) return;

        existing.schedules.forEach((item) => {
          const row = this.scheduleRows.find((r) => r.periodId === item.periodId);
          if (!row) return;

          row.sessions[item.day] = item.sessionId;
        });
      });
  }

  save() {
    this.isSaving = true;
    const schedules: ScheduleItemDto[] = [];

    this.scheduleRows.forEach((row) => {
      this.days.forEach((d) => {
        const sessionId = row.sessions[d.enum];
        if (sessionId) {
          schedules.push({
            periodId: row.periodId,
            day: d.enum,
            sessionId,
          });
        }
      });
    });

    const payload: SaveClassLevelScheduleDto = {
      classLevelId: this.classLevelId,
      schedules,
    };

    this.classService.saveClassLevelSchedule(payload).subscribe({
      next: () => {
        this.isSaving = false;
        this.dialogRef.close(true);
      },
      error: () => {
        this.isSaving = false;
      },
    });
  }

  cancel() {
    this.dialogRef.close(false);
  }
}
