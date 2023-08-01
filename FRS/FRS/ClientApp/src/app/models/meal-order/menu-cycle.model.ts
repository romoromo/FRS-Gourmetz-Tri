import { MealType } from "./meal-type.model";
import { MealTypeMenuDish, Menu, OutletMenuDish } from "./menu.model";

export class MenuCycle {

  constructor(id?: string) {

    this.id = id;
  }

  public id: string;
  public label: string;
  public startDate: Date;
  public endDate?: Date;
  public institutionId: string;
  public outletProfileId: string;
  public outletProfileName: string;
  public catererId: string;
  public schedules: MenuCycleSchedule[];
  public blockedDates: MenuCycleBlockedDate[];
  public outletBlockedDates: OutletBlockedDate[];
  public days: number[];
  public color: any;
  public isBlocked: boolean;
  public rrule?: {
    freq: any;
    byweekday?: any;
  };
}

export class MenuCycleSchedule {

  constructor(id?: string, name?: string) {
  }

  public id: string;
  public menuCycleId: string;
  public day: number;
  public periods: MenuCycleSchedulePeriod[];
}

export class MenuCycleSchedulePeriod {

  constructor(id?: string, name?: string) {
  }

  public menuCycleScheduleId: string;
  public mealPeriodId: string;
  public menus: MenuCycleScheduleMenu[];
  public outletMenus: OutletMenuCycleScheduleMenu[];
  public outletMenuDishes: OutletMenuDish[];
}

export class MenuCycleScheduleMenu {

  constructor(id?: string, mealPeriodId?: string) {
  }

  public mealPeriodId: string;
  public menuId: string;
  public label: string;
  public code: string;
  public checked: boolean;
  public menu: Menu;
  public mealTypes: MealType[];
}

export class OutletMenuCycleScheduleMenu extends MenuCycleScheduleMenu {
  public outletId: string;
}

export class MenuCycleBlockedDate {

  constructor() {
  }

  public menuCycleId: string;
  public effectiveDate: string;
  public label: string;
}

export class BlockUnblockDateModel {

  constructor() {
  }

  public blockedDates: MenuCycleBlockedDate[];
  public isUnblock: boolean;
}

export class OutletBlockedDate {

  constructor() {
  }

  public menuCycleId: string;
  public outletId: string;
  public effectiveDate: string;
  public label: string;
}

export class BlockUnblockOutletDateModel {

  constructor() {
  }

  public blockedDates: OutletBlockedDate[];
  public isUnblock: boolean;
}
