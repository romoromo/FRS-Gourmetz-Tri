import { CommonFilter, Filter, PagedResult } from "./sieve-filter.model";

export class SignageDashboard {
  constructor() {
  }
  public unreachable: string;
  public asleep: string;
  public alive: string;
  public devices: any[];
  public pagedDevices: PagedResult;
}

export class SignageDashboardFilter{

  constructor() {
  }

  public status: string;
  public filter: Filter;
}
