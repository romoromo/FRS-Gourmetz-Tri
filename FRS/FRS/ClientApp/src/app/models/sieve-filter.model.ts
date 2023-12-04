import { extend } from "jquery";

export class Filter {

  constructor(page?: number, pageSize?: number) {
  }

  public sorts: string;
  public filters: string;
  public page: number;
  public pageSize: number;
}

export class PagedResult {

  constructor() {
  }

  public filter: Filter;
  public totalCount: number;
  public pagedData: any;
}

export class CommonFilter extends Filter {
  constructor(page?: number, pageSize?: number) {
    super(page, pageSize);
  }

  public institutionId: string;
}

export class ServiceContractFilter {
  constructor(includeAssets, filter) {
    this.includeAssets = includeAssets;
    this.filter = filter;
  }

  public includeAssets: boolean;
  public filter: Filter;    
}

export class ClassRosterFilter extends Filter {
  public catererId: string;
  public outletId: string;
  public classRosterId: string;
  public outletProfileId: string;
}

export class OrderCancellationFilter extends Filter {
  public invoiceNumber: string;
  public orderNumber: string;
  public outletId: string;
  public keyword: string;
}

export class StudentOrderFilter extends Filter {
  public invoiceNumber: string;
  public orderNumber: string;
  public outletId: string;
  public keyword: string;
  public studentId: string;
}

export class SalesOrderReportFilter extends Filter {
  public reportDateFrom: string;
  public reportDateTo: string;
  public status: string;
  public isFas?: boolean;
  public keyword: string;
  public reportType: number;
  public studentGroupId?: number;
}

export class RoleReportFilter extends Filter {
  public permissions: string[];
}

export class OrderCancellationReportFilter extends Filter {
  public reportDateFrom: string;
  public reportDateTo: string;
}

export class UserActivityLogReportFilter extends Filter {
  public reportDateFrom: string;
  public reportDateTo: string;
  public keyword: string;
  public reportType: string;
}
