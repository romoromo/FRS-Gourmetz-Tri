
export class OrderPaymentResponse {
  public paymentRequestId : string;
  public userId: string;
  public institutionId: string;
  public isSuccessful: Boolean;
  public isCancelled: Boolean;
  public isConfirmedPaid: Boolean;
  public statusCode: string;
  public responseStatus: string;
  public mode: string;
  public paymentTransactionId: string;    // from fomopay
  public orderNo: string;                 // our unique generate id for request
  public tokenOrderId: string;            // id for the token being paid
  public subject: string;
  public amount: string;
  public currencyCode: string;
  public paymentUrl: string;
  public paymentDate: Date;
}

