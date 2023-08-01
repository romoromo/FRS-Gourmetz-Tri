import { TransactionOptions } from './transaction-options.model';
  
export class Order {
  public mode: string;
  public orderNo: string;
  public tokenOrderId: string;
  public subject: string;
  public description: string;
  public amount: string;
  public currencyCode: string;
  public notifyUrl: string;
  public returnUrl: string;
  public backUrl: string;
  public sourceOfFunds: string[];
  public transactionOptions: TransactionOptions;
}
