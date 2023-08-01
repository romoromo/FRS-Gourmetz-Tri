import { MealPeriod } from "./meal-period.model";
import { OutletClassRoster } from "./outlet-class-roster.model";

export class MealSession {

  constructor(id?: string, name?: string) {

    this.id = id;
    this.name = name;
  }

  public id: string;
  public name: string;
  public sequence: number;
  public institutionId: string;
  public outletId: string;
  public outletName: string;
  public mealPeriodId: string;
  public mealPeriodName: string;
  public startDate: Date;
  public endDate?: Date;
  public checked: boolean;
  public catererId: string;
  public isActive: boolean;
  public details: MealSessionDetail[];
  public classRosters: OutletClassRoster[];
}

export class MealSessionDetail {

  constructor(id?: string, name?: string) {

    this.id = id;
    this.name = name;
  }

  public id: string;
  public name: string;
  public sequence: number;
  public mealSessionId: string;
  public mealSessionName: string;
  public startDate: Date;
  public endDate?: Date;
  public routeTime?: Date;
  public overheadTime?: Date;
  public calSourceTime?: Date;
  public routeId?: string;
  public routeName?: string;
  public checked: boolean;
  public sTime: string;
  public eTime: string;
  public rTime: string;
  public oTime: string;
  public cTime: string;
  public isActive: boolean;
  public routeInterval: number;
  public overheadInterval: number;
}

export class MealSessionMealPeriod {
  public mealPeriod: MealPeriod;
  public mealSessions: MealSession[];
}
