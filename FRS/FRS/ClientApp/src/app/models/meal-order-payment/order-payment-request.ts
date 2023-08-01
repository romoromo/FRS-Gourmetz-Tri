import { PaymentItem } from './payment-item.model';
import { TransactionOptions } from './transaction-options.model';

export class OrderPaymentRequest {
  public userId: number;
  public institutionId: number;
  public discountId: number;
  public priceId: number;
  public amount: string;
  public currencyCode: string;
  public mode: string;
  public subject: string;
  public notifyUrl: string;
  public returnUrl: string;
  public backUrl: string;
  public orderNo: string;       // our unique generated id for each request
  public tokenOrderId: string;  // id for the token being paid
  public sourceOfFunds: string;
  public transactionDate: Date;
 // public transactionOption: TransactionOptions;
  
}

 
