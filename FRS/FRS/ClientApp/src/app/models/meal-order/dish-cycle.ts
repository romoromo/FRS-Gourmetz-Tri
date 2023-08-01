import { Menu } from "../menu.model";
import { DishType } from "./dish-type.model";
import { Dish } from "./dish.model";
import { MealType } from "./meal-type.model";

export class DishCycle {

  constructor() {

  }

  public id: string;
  public label: string;
  public sequence: number;
  public dishTypeId: string;
  public dishTypeName: string;
  public startDate: Date;
  public endDate: Date;
  public numOfDays: number;
  public numOfSets: number;
  public outletProfileId: string;
  public catererId: string;
  public cycleType: string;
  public cycleTypeId: string;
  public mealTypeId: string;
  public dishType: DishType;
  public dishCyclePeriods: DishCyclePeriod[];
  public schedules: DishCycleSchedule[];
  public sets: DishCycleScheduleSet[];
  public blockedDates: DishCycleBlockedDate[];
  public outletDishBlockedDates: OutletDishBlockedDate[];
  public days: number[];
  public color: any;
  public isBlocked: boolean;
  public rrule?: {
    freq: any;
    byweekday?: any;
  };
}

export class DishCyclePeriod {

  constructor(id?: string, name?: string) {
  }

  public id: string;
  public dishCycleId: string;
  public mealPeriodId: string;
  public mealPeriodName: string;
  public sets: DishCycleScheduleSet[];
  public excludedMenus: OutletDishCyclePeriodMenu[];
}

export class OutletDishCyclePeriodMenu {
  public dishCyclePeriodId: string;
  public dishId: string;
  public outletId: string;
  public dishCycleScheduleSetId: string;
}

export class DishCycleSchedule {

  constructor(id?: string, name?: string) {
  }

  public id: string;
  public dishCycleId: string;
  public day: number;
  public details: DishCycleScheduleDetail[];
}

export class DishCycleScheduleDetail {

  constructor() {
  }

  public id: string;
  public dishCycleScheduleId: string;
  public label: string;
  public sequence: number;
  public menus: DishCycleScheduleDetailMenu[];
  public displayMenus: DishCycleScheduleDetailMenu[];
  public selectedDish: Dish;
  public selectedCycle: DishCycle;
}

export class OutletDishViewMenu {
  public outletId: string;
  public excludedMenus: OutletDishCyclePeriodMenu[];
}

export class DishCycleScheduleDetailMenu {

  constructor() {
  }

  public id: string;
  public dishCycleScheduleDetailId: string;
  public dishId: string;
  public dishLabel: string;
  public dishCode: string;
  public dishCycleId: string;
  public dishCycleLabel: string;
  public checked: boolean;
}


export class DishCycleScheduleSet {

  constructor(dishCycleId?: string, sequence?: number, label?: string, cycleType?: string, cycleTypeId?: string) {
    this.sequence = sequence;
    this.label = label;
    this.cycleType = cycleType;
    this.cycleTypeId = cycleTypeId;
    this.dishCycleId = dishCycleId;
  }

  public id: string;
  public sequence: number;
  public label: string;
  public cycleType: string;
  public cycleTypeId: string;
  public cycleTypeSequence: number;
  public cycleTypeLabel: string;
  public dishCycleId: string;
  public mealTypeId: string;
  public options: any[];
  public price: string;
  public menus: DishCycleScheduleDetailMenu[];
}

export class DishCycleBlockedDate {

  constructor() {
  }

  public dishCycleId: string;
  public effectiveDate: string;
  public label: string;
}

export class DishBlockUnblockDateModel {

  constructor() {
  }

  public blockedDates: DishCycleBlockedDate[];
  public isUnblock: boolean;
}

export class DishCycleScheduleMenu {

  constructor(id?: string, mealPeriodId?: string) {
  }

  public setId: string;
  public menuId: string;
  public label: string;
  public code: string;
  public checked: boolean;
  public menu: Menu;
  public mealTypes: MealType[];
}


export class OutletDishBlockedDate {

  constructor() {
  }

  public dishCycleId: string;
  public outletId: string;
  public effectiveDate: string;
  public label: string;
}

export class BlockUnblockOutletDishDateModel {

  constructor() {
  }

  public blockedDates: OutletDishBlockedDate[];
  public isUnblock: boolean;
}
