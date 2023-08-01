
export class PaymentItem {
  public label: string;
  public amount: Amount;
  public sku: string;
  public quantity: number;
}


export class Amount {
  public currency: string;
  public value: string;
}
