
export class Waiver {

  constructor(id?: string, name?: string) {

    this.id = id;
    this.name = name;
  }

  public id: string;
  public name: string;
  public label: string;
  public amount: string;
  public institutionId: string;
  public transactionFeeId: string;
  public transactionFeePaymentType: string;
  public transactionFeeLabel: string;
}
