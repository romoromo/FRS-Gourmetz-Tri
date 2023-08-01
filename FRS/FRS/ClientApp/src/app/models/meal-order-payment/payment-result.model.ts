import { FomoPaymentResponse } from './fomo-payment-response.model';
import { TransException } from './trans-exception.model';
 
export class PaymentResult {
  public isSuccessful: boolean;
  public isCancelled: boolean;
  public message: string;
  public statusCode: string;
  public responseStatus: string;
  public responseURI: string;
  public server: string;
  public paymentTime: Date;
  public fomoPaymentResponse: FomoPaymentResponse;
  public transException: TransException;
}

