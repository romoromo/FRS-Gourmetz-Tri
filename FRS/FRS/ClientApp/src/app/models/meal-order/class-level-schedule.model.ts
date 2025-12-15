import { ScheduleDay } from "../enums";

export class ClassLevelScheduleModel {
  constructor(id?: number, classLevelId?: number) {
    this.id = id;
    this.classLevelId = classLevelId;
  }
  id: number;
  classLevelId: number;
  schedules: ScheduleItemDto[];
}

export interface ScheduleItemDto {
  periodId: number;
  day: ScheduleDay;
  sessionId: number;
}
