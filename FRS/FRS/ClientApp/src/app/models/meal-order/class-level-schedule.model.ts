import { ScheduleDay } from "../enums";

export interface ScheduleRowModel {
  periodId: number;
  periodName: string;
  sessions: {
    [ScheduleDay.Monday]?: number;
    [ScheduleDay.Tuesday]?: number;
    [ScheduleDay.Wednesday]?: number;
    [ScheduleDay.Thursday]?: number;
    [ScheduleDay.Friday]?: number;
    [ScheduleDay.Saturday]?: number;
    [ScheduleDay.Sunday]?: number;
  };
}

export interface ScheduleItemDto {
  periodId: number;
  day: ScheduleDay;
  sessionId: number;
}

export interface SaveClassLevelScheduleDto {
  classLevelId: number;
  schedules: ScheduleItemDto[];
}