import { MealType } from "./meal-type.model";
import { Class } from "./class.model";
import { MealSession } from "./meal-session.model";

export class OutletClassRoster {

  constructor(id?: string) {

    this.id = id;
  }

  public id: string;
  public label: string;
  public startDate: Date;
  public endDate?: Date;
  public institutionId: string;
  public mealSessionId: string;
  public outletProfileId: string;
  public outletProfileName: string;
  public catererId: string;
  public outletId: string;
  public schedules: OutletClassRosterSchedule[];
  public mealSession: MealSession;
  public days: number[];
  public color: any;
  public isBlocked: boolean;
  public rrule?: {
    freq: any;
    byweekday?: any;
  };
}

export class OutletClassRosterSchedule {

  constructor(id?: string, name?: string) {
  }

  public id: string;
  public outletClassRosterId: string;
  public outletClassRoster: OutletClassRoster;
  public day: number;
  public periods: OutletClassRosterSchedulePeriod[];
  public hasClass: boolean;
  public color: any;
  public rrule?: {
    freq: any;
    byweekday?: any;
  };
}

export class OutletClassRosterSchedulePeriod {

  constructor(id?: string, name?: string) {
  }

  public id: string;
  public classCycleScheduleId: string;
  public outletClassRosterScheduleId: string;
  public mealSessionDetailId: string;
  public mealSessionDetailName: string;
  public mealSessionDetailStartDate: Date;
  public mealSessionDetailEndDate: Date;
  public outletClassRosterStartDate: Date;
  public outletClassRosterEndDate: Date;
  public startDate: Date;
  public endDate: Date;
  public classes: OutletClassRosterScheduleClass[];
  public outletClasses: OutletOutletClassRosterScheduleClass[];
  public color: any;
  public rrule?: {
    freq: any;
    byweekday?: any;
  };
}

export class OutletClassRosterScheduleClass {

  constructor(id?: string, mealSessionId?: string) {
  }

  public mealSessionDetailId: string;

  public outletClassRosterSchedulePeriodId: string;
  public classId: string;
  public name: string;
  public code: string;
  public checked: boolean;
  public class: Class;
  public mealTypes: MealType[];
}

export class OutletOutletClassRosterScheduleClass extends OutletClassRosterScheduleClass {
  public outletId: string;
}

export class OutletClassRosterScheduleByDay {
  public day: number;
  public periods: OutletClassRosterSchedulePeriod[];
  public schedules: OutletClassRosterSchedule[];
  public color: any;
  public rrule?: {
    freq: any;
    byweekday?: any;
  };
}
