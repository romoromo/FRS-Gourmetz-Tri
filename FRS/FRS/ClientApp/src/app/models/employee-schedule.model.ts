import { EmployeeScheduleShift } from "./employee-schedule-shift.model";

export class EmployeeSchedule {
  public id: string;
  public code: string;
  public label: string;

  public effectiveDate: string;
  public ineffectiveDate: string;

  shifts: EmployeeScheduleShift[];
  locations: any[];
  slots: any[];
  infos: any[];
}
