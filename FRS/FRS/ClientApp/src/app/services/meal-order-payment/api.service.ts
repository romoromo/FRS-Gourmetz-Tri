import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
 
import { TokenOrderedPayment } from 'src/app/models/meal-order-payment/token-ordered.model';
import { Order } from 'src/app/models/meal-order-payment/order.model';
import { Address } from 'src/app/models/meal-order-payment/address.model';
import { Amount, PaymentItem } from 'src/app/models/meal-order-payment/payment-item.model';
import { TransactionOptions } from 'src/app/models/meal-order-payment/transaction-options.model';
import { PaymentResult } from 'src/app/models/meal-order-payment/payment-result.model';

import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpResponse, HttpErrorResponse } from '@angular/common/http';
import { Observable, of, throwError } from "rxjs";
import { catchError, map } from 'rxjs/operators';

import { OrderPaymentRequest } from 'src/app/models/meal-order-payment/order-payment-request';
import { OrderPaymentResponse } from 'src/app/models/meal-order-payment/order-payment-response';
import { AccountService } from '../../services/account.service';
import { AppTranslationService } from "../app-translation.service";


import { BodyPayLoad } from 'src/app/models/meal-order-payment/body-pay-load.model';

@Injectable({
  providedIn: 'root'
})


export class ApiService {


  bodyPayLoad: BodyPayLoad;

 constructor(private httpClient: HttpClient,
    private appTranslationService: AppTranslationService,
   public accountService: AccountService) { }




  ////// Calls API POST
  public getPaymentURLWithErrHd(tokenOrdered: TokenOrderedPayment, paymentMethod: string,
    outletURL: string, thankyouURL: string, notifyURL:string ) {

    var currentPaymentPageURL: string;

   /// mode of payment selected by user in payment page
   // let 1sourceOfFundsData: string[] = ['CARD'];

    //console.log('getPaymentURLWithErrHd / outletURL:' + outletURL);
    //console.log('getPaymentURLWithErrHd / thankyouURL:' + thankyouURL);
 

    let sourceOfFundsData: string[] = [];
 
    const headers = new HttpHeaders()
      .set('content-type', 'application/json')
      .set('accept', 'text/plain')
      .set('Access-Control-Allow-Origin', '*');

    const httpOptions =
    {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
      rejectUnauthorized: false
    };

    currentPaymentPageURL = window.location.href
    //  console.log("getPaymentURLWithErrHd / api current url " + window.location.href);

 
    var tokenOrdered2         = new TokenOrderedPayment();
    tokenOrdered2.id          = tokenOrdered.id;  // unique id for each trans passed to FOMO
    tokenOrdered2.subtotal    = tokenOrdered.subtotal;
    tokenOrdered2.tokenDesc   = tokenOrdered.tokenDesc;
    tokenOrdered2.tokenId     = tokenOrdered.tokenId
  //  console.log(" tokenOrdered2.tokenId  " +  tokenOrdered2.tokenId);

    var paymentMethod         = tokenOrdered.paymentMethod;
 
    // Set PaymentMethod 
    // If no payment found default to card payment
    if (typeof paymentMethod == 'undefined' || paymentMethod == null || paymentMethod == "")
    {
      sourceOfFundsData.push("CARD");
    }
    else
    {
       sourceOfFundsData.push(paymentMethod);
    }
 

    // For transOptions.
    var amountPayment       = new Amount();
    amountPayment.value     = "";
    amountPayment.currency  = "";

    let addressLineData: string[] = [''];

    var shipAddress           = new Address();
    shipAddress.addressLine   = addressLineData;
    shipAddress.city          = "";
    shipAddress.country       = "";
    shipAddress.organization  = "";
    shipAddress.phone         = "";
    shipAddress.postalCode    = "";
    shipAddress.recipient     = "";
    shipAddress.region        = "";
    shipAddress.dependentLocality = "";

    var paymentItem       = new PaymentItem();
    paymentItem.amount    = amountPayment;
    paymentItem.label     = "";
    paymentItem.quantity  = 0;
    paymentItem.sku       = "";

    let paymentItemList: PaymentItem[] = [paymentItem];

    var transactionOptions              = new TransactionOptions();
    transactionOptions.payerName        = "";
    transactionOptions.payerEmail       = "";
    transactionOptions.payerPhone       = "";
    transactionOptions.shippingAddress  = shipAddress;
    transactionOptions.billingAddress   = shipAddress;
    transactionOptions.paymentItem      = paymentItemList;
    // For transOptions.




    //////////////////////////////////////////////////
    // For Order.
    //////////////////////////////////////////////////
    var order           = new Order();
    order.mode          = "HOSTED";
    order.orderNo       = tokenOrdered.id.toString();
    order.tokenOrderId  = tokenOrdered.tokenId.toString();
    order.subject       = tokenOrdered.tokenDesc;
    order.description   = "";
    order.amount        = tokenOrdered.subtotal.toString();
    order.currencyCode  = "SGD";
    order.notifyUrl     = notifyURL;   //  outletURL;    //URL to notify order result update
    order.returnUrl     = thankyouURL;  //URL to return to after payment
    order.backUrl       = outletURL;    //URL to return to when user cancel the payment
    order.sourceOfFunds = sourceOfFundsData; 


    //console.log('getPaymentURLWithErrHd / order.orderNo         :' + order.orderNo);
    //console.log('getPaymentURLWithErrHd / order.tokenOrderId    :' + order.tokenOrderId );

    //////////////////////////////////////////////////
    // For Order.
    //////////////////////////////////////////////////


    order.transactionOptions = transactionOptions

    const body = JSON.stringify(order);
    //console.log('getPaymentURLWithErrHd/ body> ' + body);
    

    // Call API POST
    // Indicate response type format otherwise will get errors.
    //orig  return this.httpClient.post(`/api/Order`, body, { 'headers': headers, responseType: 'text', observe: 'response' });

    //console.log('getPaymentURLWithErrHd/  about to  call to post in API');

    return this.httpClient
     .post<PaymentResult>(`/api/Order`, body, { 'headers': headers, observe: 'response' })
      .pipe(
        catchError((err) => {
          //console.log('error caught in service')
          //console.error(err);
          //Handle the error here
          return throwError(err);    //Rethrow it back to component

        })
      )
 
   /* console.log('getPaymentURLWithErrHd/ Finished call to post in API');*/

  } // public getPaymentURLWithErrHd



  ////// Calls API POST
  public createResponseRecord(paymentResponse: PaymentResult, tokenOrderedCurrent: TokenOrderedPayment, newRequestRecordID: number) {
    var userID;
    var dateTime = new Date();
    var currentPaymentPageURL: string;

    const headers = new HttpHeaders()
      .set('content-type', 'application/json')
      .set('accept', 'text/plain')
      .set('Access-Control-Allow-Origin', '*');

    const httpOptions =
    {
      headers: new HttpHeaders({ 'ContentType': 'application/json' }),
      rejectUnauthorized: false
    };


    //////////////////////////////////////////////////////////
    /// For the new response record.....for out DB
    var orderPaymentResponse = new OrderPaymentResponse();
    orderPaymentResponse.paymentRequestId = newRequestRecordID.toString();
    orderPaymentResponse.userId = this.accountService.currentUser.id;
    orderPaymentResponse.institutionId = this.accountService.currentUser.institutionId; //// dummy value for testing.
    orderPaymentResponse.tokenOrderId = tokenOrderedCurrent.tokenId; // id for the token being paid
    orderPaymentResponse.isSuccessful = true;
    orderPaymentResponse.isCancelled = false;
    orderPaymentResponse.statusCode = paymentResponse.statusCode;
    orderPaymentResponse.responseStatus = paymentResponse.responseStatus;
    orderPaymentResponse.mode = paymentResponse.fomoPaymentResponse.mode;
    orderPaymentResponse.paymentTransactionId = paymentResponse.fomoPaymentResponse.id; // payment id from fomopay
    orderPaymentResponse.orderNo = paymentResponse.fomoPaymentResponse.orderNo; // our unique generated id for each request
    orderPaymentResponse.subject = paymentResponse.fomoPaymentResponse.subject;
    orderPaymentResponse.amount = paymentResponse.fomoPaymentResponse.amount;
    orderPaymentResponse.currencyCode = paymentResponse.fomoPaymentResponse.currencyCode;
    orderPaymentResponse.paymentUrl = paymentResponse.fomoPaymentResponse.url; // url for payment page.
    orderPaymentResponse.paymentDate = dateTime;

    /// For the new response record.....for out DB ..........................
    //////////////////////////////////////////////////////////



    // Save for call to manual token status update
    sessionStorage.setItem('fomoPayOrderId', orderPaymentResponse.paymentTransactionId); //
    sessionStorage.setItem('ourOrderNumber', orderPaymentResponse.orderNo); //
    // Save for call to manual token status update

 


    const requestBody = JSON.stringify(orderPaymentResponse);

    return this.httpClient
      .post<number>(`/api/Order/PostPaymentResponseRecord`, requestBody, { 'headers': headers, observe: 'response' })
      .pipe(
        catchError((err) => {
          //Handle the error here
          return throwError(err);    //Rethrow it back to component

        })
      )

    // console.log('createResponseRecord / line 249 end');

  }

  ////// Calls API POST
  public createRequestRecord(tokenOrdered: TokenOrderedPayment, paymentMethod: string,
    outletURL: string, thankyouURL: string, notifyURL: string  ) {

    var currentPaymentPageURL: string;


    let sourceOfFundsData: string[] = [];

    const headers = new HttpHeaders()
      .set('content-type', 'application/json')
      .set('accept', 'text/plain')
      .set('Access-Control-Allow-Origin', '*');

    const httpOptions =
    {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
      rejectUnauthorized: false
    };

    currentPaymentPageURL = window.location.href
    //  console.log("getPaymentURLWithErrHd / api current url " + window.location.href);


    var tokenOrdered2 = new TokenOrderedPayment();
    tokenOrdered2.id = tokenOrdered.id;  // unique id for each trans passed to FOMO
    tokenOrdered2.subtotal = tokenOrdered.subtotal;
    tokenOrdered2.tokenDesc = tokenOrdered.tokenDesc;
    tokenOrdered2.tokenId = tokenOrdered.tokenId


    var paymentMethod = tokenOrdered.paymentMethod;

    // Set PaymentMethod 
    // If no payment found default to card payment
    if (typeof paymentMethod == 'undefined' || paymentMethod == null || paymentMethod == "") {
      sourceOfFundsData.push("CARD");
    }
    else {
      sourceOfFundsData.push(paymentMethod);
    }


    // For transOptions.
    var amountPayment = new Amount();
    amountPayment.value = "";
    amountPayment.currency = "";

    let addressLineData: string[] = [''];

    var shipAddress = new Address();
    shipAddress.addressLine = addressLineData;
    shipAddress.city = "";
    shipAddress.country = "";
    shipAddress.organization = "";
    shipAddress.phone = "";
    shipAddress.postalCode = "";
    shipAddress.recipient = "";
    shipAddress.region = "";
    shipAddress.dependentLocality = "";

    var paymentItem = new PaymentItem();
    paymentItem.amount = amountPayment;
    paymentItem.label = this.appTranslationService.getTranslation('paymentPopup.Title');
    paymentItem.quantity = 1; // default for now.
    paymentItem.sku = tokenOrdered.tokenId.toString();

    let paymentItemList: PaymentItem[] = [paymentItem];

    var transactionOptions = new TransactionOptions();
    transactionOptions.payerName = "";
    transactionOptions.payerEmail = "";
    transactionOptions.payerPhone = "";
    transactionOptions.shippingAddress = shipAddress;
    transactionOptions.billingAddress = shipAddress;
    transactionOptions.paymentItem = paymentItemList;
    // For transOptions.




    //////////////////////////////////////////////////
    // For Order.
    //////////////////////////////////////////////////
    var order = new Order();
    order.mode = "HOSTED";
    order.orderNo = tokenOrdered.id.toString();
    order.tokenOrderId = tokenOrdered.tokenId.toString();
    order.subject = tokenOrdered.tokenDesc;
    order.description = "";
    order.amount = tokenOrdered.subtotal.toString();
    order.currencyCode = "SGD";
    order.notifyUrl = notifyURL;    //URL to notify order result update
    order.returnUrl = thankyouURL;  //URL to return to after payment
    order.backUrl = outletURL;    //URL to return to when user cancel the payment
    order.sourceOfFunds = sourceOfFundsData;


    //console.log('getPaymentURLWithErrHd / order.orderNo         :' + order.orderNo);
    //console.log('getPaymentURLWithErrHd / order.tokenOrderId    :' + order.tokenOrderId );

    //////////////////////////////////////////////////
    // For Order.
    //////////////////////////////////////////////////


    order.transactionOptions = transactionOptions

    const body = JSON.stringify(order);


    ///
    var orderPaymentRequest = new OrderPaymentRequest();
    let dateTime = new Date();

    //console.log('3 accountService=' + this.accountService.currentUser.id);
    //console.log('3 accountService=' + order.amount);

    orderPaymentRequest.userId = parseInt(this.accountService.currentUser.id);
    orderPaymentRequest.institutionId = parseInt(this.accountService.currentUser.institutionId);
    orderPaymentRequest.amount = order.amount;
    orderPaymentRequest.currencyCode = order.currencyCode;
    orderPaymentRequest.mode = order.mode;
    orderPaymentRequest.subject = order.subject;
    orderPaymentRequest.notifyUrl = order.notifyUrl;
    orderPaymentRequest.returnUrl = order.returnUrl;
    orderPaymentRequest.backUrl = order.backUrl;
    orderPaymentRequest.orderNo = order.orderNo;            // our unique generated id for each request
    orderPaymentRequest.tokenOrderId = order.tokenOrderId;  // id for the token being paid
    orderPaymentRequest.sourceOfFunds = order.sourceOfFunds[0];
    orderPaymentRequest.transactionDate = dateTime;
    // orderPaymentRequest.transactionOption = order.transactionOptions;

    const requestBody = JSON.stringify(orderPaymentRequest);

    return this.httpClient
      .post<number>(`/api/Order/PostPaymentRequestRecord`, requestBody, { 'headers': headers, observe: 'response' })
      .pipe(
        catchError((err) => {
          //Handle the error here
          return throwError(err);    //Rethrow it back to component

        })
      )

  }  //createResponseRecord



  //////////////////////////////////////////////////////////////////////////////////////


  ////// Calls API POST - manually call the NotifyURLmanual method.
  public updateOrderStatus(fOMOPaymentId: string, ourPaymentOrderNo: string)
  {

    var currentPaymentPageURL: string;

    //console.log('getPaymentURLWithErrHd / outletURL:' + outletURL);
    //console.log('getPaymentURLWithErrHd / thankyouURL:' + thankyouURL);


    const headers = new HttpHeaders()
      .set('content-type', 'application/json')
      .set('accept', 'text/plain')
      .set('Access-Control-Allow-Origin', '*');

    const httpOptions =
    {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
      rejectUnauthorized: false
    };

 
   
    var bodyPayLoad = new BodyPayLoad();
    bodyPayLoad.OrderId = fOMOPaymentId;
    bodyPayLoad.OrderNo = ourPaymentOrderNo;
    bodyPayLoad.TransactionId = "";  // not used
    bodyPayLoad.TransactionNo = "";  // not used


    console.log('api/ updateOrderStatus api method / FOMOPaymentId         :' + fOMOPaymentId);
    console.log('api/ updateOrderStatus api method / OurPaymentOrderNo    :' + ourPaymentOrderNo);
 

    const body = JSON.stringify(bodyPayLoad);
    //console.log('getPaymentURLWithErrHd/ body> ' + body);
 
    //api/controller/method  
    return this.httpClient
      .post<any>(`/api/Order/PostProcessNotifyURLManualUpdate`, body, { 'headers': headers, observe: 'response' })
        .pipe(
            map(result => {
                if (result)
                {
                    console.log('PostProcessNotifyURLManualUpdate - no errors ');
                }  
            }),
        catchError((err) => {
          //console.log('error caught in service')
          //console.error(err);
          //Handle the error here
          return throwError(err);    //Rethrow it back to component

        })
      )

    /* console.log('getPaymentURLWithErrHd/ Finished call to post in API');*/
 

  } // public getPaymentURLWithErrHd


 

}


