
export class MenuCycleCalendar {

  constructor() {
  }

  public id: string;
  public label: string;
  public menuCycleId: string;
  public blockedDates: MenuCycleCalendarBlockedDate[];
  public color: any;
  public rrule?: {
    freq: any;
    byweekday?: any;
  };
}

export class MenuCycleCalendarBlockedDate {

  constructor() {
  }

  public id: string;
  public label: string;
  public menuCycleCalendarId: string;
  public effectiveDate: Date;
}
