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

export class TransactionFilter extends Filter {
  public studentId: string;
  public transactionType: string;
  public transactionDate: string;
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
  public orderType: string;
  public isFas?: boolean;
  public keyword: string;
  public reportType: number;
  public studentGroupIds?: number[];
  public collectionStatuses?: string[];
  public outletId?: number[];
}

export class VoucherUtilisationReportFilter extends Filter {
  public reportDateFrom: string;
  public reportDateTo: string;
  public status: string;
  public keyword: string;
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

export class BentoAssetFilter extends Filter{
  constructor(page?: number, pageSize?: number) {
    super(page, pageSize);
  }
  public catererInfoId: string;
}

export class CartonAssetFilter extends Filter{
  constructor(page?: number, pageSize?: number) {
    super(page, pageSize);
  }
  public catererInfoId: string;
}

export class CatererAssetFilter extends Filter{
  constructor(page?: number, pageSize?: number) {
    super(page, pageSize);
  }
  public catererInfoId: string;
}

export class AssetCmpFilter extends Filter{
  constructor(page?: number, pageSize?: number) {
    super(page, pageSize);
  }
  public catererInfoId: string;
}

export class CancelOrderRequestFilter extends Filter{
  constructor(page?: number, pageSize?: number) {
    super(page, pageSize);
  }
  public catererInfoId: string;
}

export class WalletTransactionFilter extends Filter{
  public outletId?: number[];
  public classLevelIds: number[];

  public startDate: string;
  public endDate: string;
  public isFAS: boolean;
}

export class EWalletTransactionFilter extends Filter{
  public studentId: string;
  public transactionType: string;
  public transactionDate: string;
  public keyword: string;
  
  public outletId?: number[];
  public isFAS: boolean;
}

export class FASMonthlyBillingFilter extends Filter{
  public outletId?: number[];
  public classLevelIds: number[];

  public startDate: string;
  public endDate: string;
  public isFAS: boolean;
  public includeOffboarded?: boolean = false;
}

export class DetailedBasicWalletTopUpFilter extends Filter{
  public outletId?: number[];
  public classLevelIds: number[];

  public startDate: string;
  public endDate: string;
}
