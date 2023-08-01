
export class DishCycleCalendar {

  constructor() {
  }

  public id: string;
  public label: string;
  public dishCycleId: string;
  public blockedDates: DishCycleCalendarBlockedDate[];
  public color: any;
  public rrule?: {
    freq: any;
    byweekday?: any;
  };
}

export class DishCycleCalendarBlockedDate {

  constructor() {
  }

  public id: string;
  public label: string;
  public dishCycleCalendarId: string;
  public effectiveDate: Date;
}
