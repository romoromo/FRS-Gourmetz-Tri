
export class Voucher {

  constructor(id?: string, name?: string) {

    this.id = id;
    this.name = name;
  }

  public id: string;
  public name: string;
  public institutionId: string;
  public voucherTypeId: string;
  public outletProfileId: string;
  public startDateTime: Date;
  public endDateTime: Date;
  public discountType: string;
  public discountAmount: number;
  public minimumBasketPrice: number;
  public usageQuantity: number;
  public usageQuantityUsed: number;
  public maxDistribution: number;
  public isDisplayAllPages: boolean;
  public voucherMealPeriods: VoucherMealPeriod[];
}

export class VoucherMealPeriod {

  constructor(id?: string, name?: string) {
  }

  public id: string;
  public VoucherId: string;
  public mealPeriodId: string;
  public mealPeriodName: string;
}

export class VoucherUtilisation {

  constructor(id?: string, name?: string) {
  }

  public id: string;
  public total: string;
  public voucherName: string;
  public voucherCode: string;
  public voucherAmount: number;
  public discountType: string;
  public validityStartDate: Date;
  public validityEndDate: Date;
  public studentName: string;
  public className: string;
  public utilisedDate?: Date;
  public invoiceNumber: string;
  public discount: number;
  public voucherStatus: string;

}
