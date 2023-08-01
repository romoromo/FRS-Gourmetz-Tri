import { Address } from './address.model';
import { PaymentItem } from './payment-item.model';

export class TransactionOptions {
  public payerName: string;
  public payerEmail: string;
  public payerPhone: string;
  public shippingAddress: Address;
  public billingAddress: Address;
  public paymentItem: PaymentItem[];
}
