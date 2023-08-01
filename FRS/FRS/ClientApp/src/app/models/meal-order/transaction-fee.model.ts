
export class TransactionFee {

  constructor(id?: string, name?: string) {

    this.id = id;
    this.name = name;
  }

  public id: string;
  public name: string;
  public paymentTypeId: string;
  public amount: number;
  public isFixed: boolean;
  public paymentTypeName: string;
  public transactionFeeDetails: TransactionFeeDetail[];
}

export class TransactionFeeDetail {

  constructor(id?: string, name?: string) {

    this.id = id;
    this.name = name;
  }

  public id: string;
  public name: string;
  public amount: number;
  public transactionFeeId: string;
}

