
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
