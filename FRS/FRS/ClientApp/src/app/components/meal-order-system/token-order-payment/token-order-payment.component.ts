import { Component, ViewChild, OnInit, Input, Output, EventEmitter, Inject} from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
  
import { throwError } from 'rxjs/internal/observable/throwError';
import { Router } from '@angular/router';
import { FormBuilder, FormArray, ValidatorFn } from '@angular/forms';

import { AppTranslationService } from "../../../services/app-translation.service";

import { Location } from '@angular/common';

import { DateAdapter, MatDatepickerInputEvent, MatDialog, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { MAT_MOMENT_DATE_FORMATS } from '@angular/material-moment-adapter';
import { MomentUtcDateAdapter } from 'src/app/helpers/moment-utc-adapter';

import { ApiService } from '../../../services/meal-order-payment/api.service';

import { TokenOrderedPayment } from 'src/app/models/meal-order-payment/token-ordered.model';
import { TokenOrder, TokenOrdered } from 'src/app/models/meal-order/token-order.model';
import { PaymentResult } from 'src/app/models/meal-order-payment/payment-result.model';
import { FomoPaymentResponse } from 'src/app/models/meal-order-payment/fomo-payment-response.model';
import { PaymentOptions } from 'src/app/models/meal-order-payment/payment-options.model';
import { TokenOrderPaymentThankyouComponent } from './token-order-payment-thankyou/token-order-payment-thankyou.component';

import { PaymentDiscount } from 'src/app/models/meal-order-payment/payment-discount.model';
import { Student } from 'src/app/models/meal-order/student.model';

import { OrderPaymentRequest } from 'src/app/models/meal-order-payment/order-payment-request';
import { OrderPaymentResponse } from 'src/app/models/meal-order-payment/order-payment-response';
import { AccountService } from '../../../services/account.service';
 
import paymentDiscountData from 'src/assets/token-payment-appsettings.json';
import paymentListData from 'src/assets/token-payment-options.json';


import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { PaymentType } from 'src/app/models/meal-order/payment-type.model';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { PaymentService } from 'src/app/services/meal-order/payment.service';
import { Utilities } from '../../../services/utilities';


/*import { CookieService } from 'ngx-cookie-service';*/

@Component({
  selector: 'token-order-payment',
  templateUrl: './token-order-payment.component.html',
  styleUrls: ['./token-order-payment.component.css']
})





export class TokenOrderPaymentComponent implements OnInit
{

  private studentTokenOrder: TokenOrder;
  cookieValue: any;

  formdata;
  bodyData;
  imageSrc;
  imageAlt;
  displayOrderNumber  = this.appTranslationService.getTranslation('paymentPopup.NoOrderNumberFound');
  userMessage         = "";
  errorMessageDisplay = "";

  outletURL   = "";
  thankyouURL = ""; // Customer redirected to this page after successful payment
  notifyURL   = ""; // FOMOpay will use this send us when an order status changes eg credit card payment successful. 

  public href: string = "";

  paymentResult         : PaymentResult;
  fomoPaymentResponse   : FomoPaymentResponse;
  paymentRequestRecordId: number = 0;

  paymentList   : PaymentOptions[] = [];
  paymentOption : PaymentOptions;

  form: FormGroup;
  orders = [];

  ////
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';
  rows: PaymentType[] = [];
  paymentTypesList;


  constructor(private apiService: ApiService,
    private appTranslationService: AppTranslationService,
    private formBuilder: FormBuilder,
    private router: Router,
    private accountService: AccountService,
    public dialogRef: MatDialogRef<TokenOrderPaymentComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    public dialog: MatDialog,  
    private paymentService: PaymentService, private alertService: AlertService
  )

  {

 

    // //
    this.initializeFilter();
    this.initializePagedResult();
    this.loadPaymentTypeData();
    // //

    this.studentTokenOrder = new TokenOrder();
 
    if (data.tokenOrder !== undefined && data.tokenOrder !== null)
    {
      this.studentTokenOrder = Object.assign({}, data.tokenOrder);
      this.displayOrderNumber = this.getRandomInt(1, Number.MAX_SAFE_INTEGER);
      //console.log("========================================");
      //var date= new Date();
      //console.log("time="+ date);
      //console.log("JSON.stringify this.orderEdit =>  " + JSON.stringify(this.studentTokenOrder));
      //console.log("data.tokenOrder.id id=>");
      //console.log("cloneFood status =>" + this.studentTokenOrder.status);
      //console.log("cloneFood totalAmount=>" + this.studentTokenOrder.totalAmount);
      //console.log("cloneFood totalAmount=>" + this.studentTokenOrder.totalAmount);
      //console.log("========================================");
    }  //     if (data !== undefined && data !== null)



  }






  ///
 initializeFilter() {
    this.filter = new Filter(1, 10);
    this.filter.sorts = 'name';
    this.filter.filters = '';
    this.filter.page = 1;
  }


 initializePagedResult() {
    this.pagedResult = new PagedResult();
    this.pagedResult.totalCount = 0;
    this.pagedResult.pagedData = [];
    this.pagedResult.filter = this.filter;
  }

  //




 loadPaymentTypeData(ev?: any)
  {
  //  this.alertService.startLoadingMessage();
  //  this.loadingIndicator = true;
    this.filter.pageSize = 15;

   // console.log(" doing  loadPaymentTypeData line 147 " );

    if (ev)
    {
      this.filter.page = ev.offset + 1;
      if (ev.sorts) {
        this.filter.sorts = ev.sorts[0].dir == 'desc' ? '-' + ev.sorts[0].prop : ev.sorts[0].prop;
      }
    }

    if (!this.keyword) this.keyword = '';
    this.filter.filters = '(IsActive)==true,(Name)@=' + this.keyword;

    this.paymentService.getPaymentTypesByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        //this.alertService.stopLoadingMessage();
        //this.loadingIndicator = false;

        let paymentTypes = results.pagedData;
     

        paymentTypes.forEach((paymentType, index, paymentTypes) => {
          (<any>paymentType).index = index + 1;
        });

        this.paymentTypesList = paymentTypes;

        //for (var product of this.paymentTypesList) {
        //  console.log("Line 176: "+product.name);
        //}

        this.processPaymenTypes();;

        //this.rowsCache = [...paymentTypes];
        //this.rows = paymentTypes;

      },
        error => {
          //this.alertService.stopLoadingMessage();
          //this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve records from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });


   // console.log(" finish doing  loadPaymentTypeData line 147 ");

  }


  //

 
 SetupFormControls()
  {
    var tokenOrdered = new TokenOrdered();
 
    this.formdata = new FormGroup({
      paymentchosen: new FormControl("SELECT"),

    });

  }

 



//Redirect URL's
getReturnURLS()
{
    var currentURL        : string = "";
    var currentRouterPath : string = "";

 
    //1 eg http://localhost:56767/outlets
    if (window.location.href.length > 0) {
      this.outletURL = window.location.href;
      //console.log(' outletURL=' + this.outletURL);
    }

    //2 eg /outlets
    currentRouterPath = this.router.url;


    //3 eg http://localhost:56767
    currentURL = window.location.origin
   if (currentURL.length > 0)
   {
     this.thankyouURL = currentURL + "/" + "paymentthankyou";

     this.notifyURL = currentURL + "/" + "api/Order/PostProcessNotifyURL";
    }
 
    //console.log('2 this.outletURL =' + this.outletURL );
    //console.log('3 thankyouURL=' + this.thankyouURL);
    //console.log('3 thankyouURL=' + window.location.origin);




  } // end function



ngOnInit() {
    this.SetupFormControls();
  } // end function


ngAfterViewInit() {
  } // end function



processPaymenTypes()
  {
    var discountRate: number = 0;
    var orderQty: number = 0;
    var orderTotal: number = 0;
    var discountRate: number = 0;
    var key;

    var objPaymentData = JSON.stringify(paymentListData)
    var discountRateMap = new Map<string, number>();
    this.getReturnURLS();
    this.userMessage = "";

    // check object
    if (this.studentTokenOrder !== undefined && this.studentTokenOrder !== null)
    {

      discountRateMap = this.getDiscountRateInfo(discountRateMap);
      orderTotal      = this.studentTokenOrder.totalAmount;
 
      if (orderTotal > 0.50)
      {
 
       // console.log("  ngOnInit() Line 277:");
        for (var paymentType of this.paymentTypesList)
        {
           // console.log("  ngOnInit() Line 276:"+ paymentType.name);
            key = paymentType.name;
 

            ///  this.SetupFormControls();
            this.paymentResult        = new PaymentResult();
            this.fomoPaymentResponse  = new FomoPaymentResponse();

 
           // if (key.toUpperCase() == "ALIPAY" && value == true)
            if (key.toUpperCase() == "ALIPAY")
            {
              let paymentOption       = new PaymentOptions();
              paymentOption.isEnabled = true;
              paymentOption.imageSrc = 'assets/images/payments/alipayplus-logo.png';
              discountRate = discountRateMap.get('ALIPAY');  
              paymentOption.paymentDiscount = discountRate;
              paymentOption.paymentName = "AliPay";
              paymentOption.paymentCode = "ALIPAY";
              paymentOption.discountedPaymentAmount = this.formatDisplayAmount(orderTotal); // NB Not doing discounts off payment now.
              this.imageAlt = 'AliPay';
              this.paymentList.push(paymentOption);
            }

            if (key.toUpperCase() == "ATOME") 
            {
              let paymentOption2 = new PaymentOptions();
              paymentOption2.isEnabled = true;
              paymentOption2.imageSrc = 'assets/images/payments/atome-logo.png';
              discountRate = discountRateMap.get('ATOME');   
              paymentOption2.paymentDiscount = discountRate;
              paymentOption2.paymentName = "Atome";
              paymentOption2.paymentCode = "ATOME";
              paymentOption2.discountedPaymentAmount = this.formatDisplayAmount(orderTotal); // this.calculateDiscountedPrice(orderTotal, discountRate);
           
              this.imageAlt = 'Atome';
              this.paymentList.push(paymentOption2);
            }

            if (key.toUpperCase() == "CREDIT CARD")
            {
              console.log("Line 321 handling payment:"+paymentType.name);
              let paymentOption3 = new PaymentOptions();
              paymentOption3.isEnabled = true;
              paymentOption3.imageSrc = 'assets/images/payments/card-logo.png';
              discountRate = discountRateMap.get('CARD');  
              paymentOption3.paymentDiscount = discountRate;
              paymentOption3.paymentName = "Credit/Debit Card";
              paymentOption3.paymentCode = "CARD";
              paymentOption3.discountedPaymentAmount = this.formatDisplayAmount(orderTotal); // this.calculateDiscountedPrice(orderTotal, discountRate);

              this.imageAlt = 'Credit/Debit Card';
              this.paymentList.push(paymentOption3);
            }


            if (key.toUpperCase() == "DPT") 
            {
              let paymentOption4 = new PaymentOptions();
              paymentOption4.isEnabled = true;
              paymentOption4.imageSrc = 'assets/images/payments/dpt-logo.png';
              discountRate = discountRateMap.get('DPT');  
              paymentOption4.paymentDiscount = discountRate;
              paymentOption4.paymentName = "DPT";
              paymentOption4.paymentCode = "DPT";
              paymentOption4.discountedPaymentAmount = this.formatDisplayAmount(orderTotal); // this.calculateDiscountedPrice(orderTotal, discountRate);

              this.imageAlt = 'DPT';
              this.paymentList.push(paymentOption4);
            }

            if (key.toUpperCase() == "BROWSER")
            {
              let paymentOption5 = new PaymentOptions();
              paymentOption5.isEnabled = true;
              paymentOption5.imageSrc = 'assets/images/payments/googlepay-logo.png';
              discountRate = discountRateMap.get('BROWSER');   
              paymentOption5.paymentDiscount = discountRate;
              paymentOption5.paymentName = "Browser Payment"; // eg google pay, apple pay, microsoft pay etc
              paymentOption5.paymentCode = "BROWSER";
              paymentOption5.discountedPaymentAmount = this.formatDisplayAmount(orderTotal);// this.calculateDiscountedPrice(orderTotal, discountRate);

              this.paymentList.push(paymentOption5);
            }

            if (key.toUpperCase() == "GRABPAY") 
            {
              let paymentOption6 = new PaymentOptions();
              paymentOption6.isEnabled = true;
              paymentOption6.imageSrc = 'assets/images/payments/grabpay-logo.png';
              discountRate = discountRateMap.get('GRABPAY');   
              paymentOption6.paymentDiscount = discountRate;
              paymentOption6.paymentName = "GrabPay";
              paymentOption6.paymentCode = "GRABPAY";
              paymentOption6.discountedPaymentAmount = this.formatDisplayAmount(orderTotal);    // this.calculateDiscountedPrice(orderTotal, discountRate);

              this.paymentList.push(paymentOption6);
            }

            if (key.toUpperCase() == "NETSPAY")
            {
              let paymentOption7 = new PaymentOptions();
              paymentOption7.isEnabled = true;
              paymentOption7.imageSrc = 'assets/images/payments/netspay-logo.png';
              discountRate = discountRateMap.get('NETSPAY');  
              paymentOption7.paymentDiscount = discountRate;
              paymentOption7.paymentName = "NetsPay";
              paymentOption7.paymentCode = "NETSPAY";
              paymentOption7.discountedPaymentAmount = this.formatDisplayAmount(orderTotal); // this.calculateDiscountedPrice(orderTotal, discountRate);

              this.paymentList.push(paymentOption7);
            }

            if (key.toUpperCase() == "PAYNOW")
            {
              let paymentOption8 = new PaymentOptions();
              paymentOption8.isEnabled = true;
              paymentOption8.imageSrc = 'assets/images/payments/paynow-logo.png';
              discountRate = discountRateMap.get('PAYNOW');  
              paymentOption8.paymentDiscount = discountRate;
              paymentOption8.paymentName = "PayNow";
              paymentOption8.paymentCode = "PAYNOW";
              paymentOption8.discountedPaymentAmount = this.formatDisplayAmount(orderTotal);     // this.calculateDiscountedPrice(orderTotal, discountRate);

              this.paymentList.push(paymentOption8);
            }

            if (key.toUpperCase() == "PAYPAL")
            {
              let paymentOption9 = new PaymentOptions();
              paymentOption9.isEnabled = true;
              paymentOption9.imageSrc = 'assets/images/payments/paypal-logo.png';
              discountRate = discountRateMap.get('PAYPAL');   
              paymentOption9.paymentDiscount = discountRate;
              paymentOption9.paymentName = "PayPal";
              paymentOption9.paymentCode = "PAYPAL";
              paymentOption9.discountedPaymentAmount = this.formatDisplayAmount(orderTotal); //this.formatDisplayAmount(orderTotal);     // this.calculateDiscountedPrice(orderTotal, discountRate);

              this.paymentList.push(paymentOption9);
            }

            if (key.toUpperCase() == "SHOPEEPAY")
            {
              let paymentOption10 = new PaymentOptions();
              paymentOption10.isEnabled = true;
              paymentOption10.imageSrc = '../../assets/images/payments/shopeepay-logo.png';
              discountRate = discountRateMap.get('SHOPEEPAY'); 
              paymentOption10.paymentDiscount = discountRate;
              paymentOption10.paymentName = "ShopeePay";
              paymentOption10.paymentCode = "SHOPEEPAY";
              paymentOption10.discountedPaymentAmount = this.formatDisplayAmount(orderTotal); // this.calculateDiscountedPrice(orderTotal, discountRate);

              this.paymentList.push(paymentOption10);
            }

            if (key.toUpperCase() == "UNIONPAY" )
            {
              let paymentOption11 = new PaymentOptions();
              paymentOption11.isEnabled = true;
              paymentOption11.imageSrc = 'assets/images/payments/unionpay-logo.png';
              discountRate = discountRateMap.get('UNIONPAY');   
              paymentOption11.paymentDiscount = discountRate;
              paymentOption11.paymentName = "UnionPay";
              paymentOption11.paymentCode = "UNIONPAY";
              paymentOption11.discountedPaymentAmount = this.formatDisplayAmount(orderTotal); // this.calculateDiscountedPrice(orderTotal, discountRate);

              this.paymentList.push(paymentOption11);
            }

            if (key.toUpperCase() == "WECHATPAY")
            {
              let paymentOption12 = new PaymentOptions();
              paymentOption12.isEnabled = true;
              paymentOption12.imageSrc = 'assets/images/payments/wechatpay-logo.png';
              discountRate = discountRateMap.get('WECHATPAY');   
              paymentOption12.paymentDiscount = discountRate;
              paymentOption12.paymentName = "WeChatPay";
              paymentOption12.paymentCode = "WECHATPAY";
              paymentOption12.discountedPaymentAmount = this.formatDisplayAmount(orderTotal); // this.calculateDiscountedPrice(orderTotal, discountRate);

              this.paymentList.push(paymentOption12);
            }

         }  // for (var paymentType of paymentTypes) {


      }
      else
      {
         // this.userMessage = "Order total is below minimum order value or no data to display !";
          this.displayOrderNumber = "";
          this.userMessage = this.appTranslationService.getTranslation('paymentPopup.BelowMinOrderValue');

      }  //    if (orderTotal > 0.50)
     

    }   //    if (this.studentTokenOrder !== undefined && this.studentTokenOrder !== null)

 

 }  // ngOnInit

 
getCurrentAbsoluteSiteUrl(): string
{

    //console.log(window.location.href);
    //console.log(window.location.host);

    if (window
      && "location" in window
      && "protocol" in window.location
      && "pathname" in window.location
      && "host" in window.location) {
      return window.location.protocol + "//" + window.location.host + window.location.pathname;
    }

    return null;

 }


 
// Read json file and return discount rate
getDiscountRate(paymentMethod)
{
    var objData = JSON.stringify(paymentDiscountData)
    var discountRate: number = 0;

    const myArr = JSON.parse(objData);

  JSON.parse(objData, (key, value) =>
  {
      if (key == paymentMethod) {
        //console.log("found match paymentMethod: " + paymentMethod);
        discountRate = value;
      }

    });
    return discountRate;
 }


// Read json file and return discount rate data - 
getDiscountRateInfo(discountDataList)
 {
  var objData = JSON.stringify(paymentDiscountData)
 
  const myArr = JSON.parse(objData);

  JSON.parse(objData, (key, value) => {
    discountDataList.set(key, value);
  });

  return discountDataList;

}



calculateRevisedAmount(orderAmount, paymentMethod)
  {
    var discountRate    : number;
    var subTotalNew     : number = 0;
    var subTotalRounded;
 
    var discountRate  = this.getDiscountRate(paymentMethod);
    subTotalNew       = (orderAmount) - ((orderAmount) * discountRate);
    subTotalRounded   = subTotalNew.toFixed(2);

    return subTotalRounded;

}



calculateDiscountedPrice(orderTotalAmount, paymentDiscountRate)
{
  var subTotalNew   : number = 0;
  var subTotalRounded;

  subTotalNew     = (orderTotalAmount) - ((orderTotalAmount) * paymentDiscountRate);
  subTotalRounded = subTotalNew.toFixed(2);

  return subTotalRounded;

  }



formatDisplayAmount(orderTotalAmount)
{
  var subTotalRounded;
  subTotalRounded = orderTotalAmount.toFixed(2);
  return subTotalRounded;
}


// Call API with  data via input button click
onClickSubmitAction(eventButn, data)
{

  var paymentMethod: string;
  var currentPaymentPageURL: string;
  var newPaymentResponseRecordId;

  // console.log(' onClickSubmitAction ' + eventButn);
  // console.log(' data          ' + data);
  // console.log(" current url    " + window.location.href);

  // Get selected payment method.
  paymentMethod = eventButn.target.id;

  this.errorMessageDisplay = "";


  if (this.formdata.valid) {
    // Disable the payment buttons
    this.formdata.controls['paymentchosen'].disable();
    this.formdata.controls['paymentchosen'].markAsDirty();
    var dd = this.formdata.controls['paymentchosen'];

    // Highlight select payment button  
    var btnSelected = eventButn.target;
    btnSelected.style.backgroundColor = '#4800FF'; // dark purple colour
  }
  else {
    //console.log("not form submitted");
  }



  //console.log("onClickSubmitAction / totalAmount=>" + this.studentTokenOrder.totalAmount);
  //console.log("onClickSubmitAction / studentTokenOrder.id) =>" + this.studentTokenOrder.id);
  //console.log("onClickSubmitAction / studentTokenOrder.mealSessionDetailId =>" + this.studentTokenOrder.mealSessionDetailId);
  //console.log("onClickSubmitAction / studentTokenOrder.profileId =>" + this.studentTokenOrder.profileId);

  currentPaymentPageURL = window.location.href;
 
  let discountRate: number    = 0;
  let revisedSubTotal: number = 0;

  // Get revised SubTotal.
  revisedSubTotal = this.studentTokenOrder.totalAmount;  // this.calculateRevisedAmount(this.studentTokenOrder.totalAmount, paymentMethod)



   // Just for debug
   //temp for testing - min payment for fomopay is SGD 0.50
   // revisedSubTotal = 0.50;  
   // console.log('onClickSubmitAction / revisedSubTotal  :' + revisedSubTotal);
   //temp for testing
   //


  ////// Revised
  let tokenOrdered      = new TokenOrderedPayment();  
  let id: string        = this.displayOrderNumber;  
  let tokenId: string   = this.studentTokenOrder.id;
  let tokenDesc: string = "Token Meal Payment";  
  let subtotal: number = revisedSubTotal

  subtotal = 0;
 
 
  //console.debug("data submitted");
  //console.debug("id: " + id);
  //console.debug("tokenId: " + tokenId);
  //console.debug("tokenDesc: " + tokenDesc);
  //console.debug("subtotal: " + subtotal);
  //console.debug("revisedSubTotal: " + revisedSubTotal);

  tokenOrdered.id             = id;
  tokenOrdered.tokenId        = tokenId;
  tokenOrdered.tokenDesc      = tokenDesc;
  tokenOrdered.subtotal       = revisedSubTotal;
  tokenOrdered.paymentMethod  = paymentMethod.trim();

  // 1st call create db record
  this.SaveRequestRecord(tokenOrdered, paymentMethod, this.outletURL, this.thankyouURL, this.notifyURL);

 
  this.apiService.getPaymentURLWithErrHd(tokenOrdered, paymentMethod, this.outletURL, this.thankyouURL, this.notifyURL ).subscribe(
    (data) => {
      //console.log('(onClickSubmit) About to read data from post call');
      // console.log(data);
      // this.bodyData = data['body'];
      this.paymentResult = data.body;

      //console.log('Finished URL from API');
      //console.log('URL  :' + this.paymentResult.responseURI);
      //console.log('responseStatus  :' + this.paymentResult.responseStatus);
      //console.log('paymentTime  :' + this.paymentResult.paymentTime);
      //console.log('isSuccessful  :' + this.paymentResult.isSuccessful); 
      //console.log(' data.body  :' + JSON.stringify(data.body) );

      if (this.paymentResult !== undefined && this.paymentResult !== null) {

        if (this.paymentResult.responseURI !== undefined && this.paymentResult.responseURI !== null)
        {
          if (this.paymentResult.responseURI.length > 0)
          {

            // Save transactionID
            let fomoPaymentResponse = new FomoPaymentResponse
            fomoPaymentResponse = this.paymentResult.fomoPaymentResponse;

            // console.log('token component/ this.SavePaymentResponse method /line 562 about to call payment URL  :' + this.paymentRequestRecordId);
            // Save data before call api to be on safe side.
            newPaymentResponseRecordId = this.SavePaymentResponse(this.paymentResult, tokenOrdered, this.paymentRequestRecordId);
              //console.log('token component/onClickSubmitAction method/line 567 returned valued=' + newPaymentResponseRecordId);

              // Redirect user to the FOMOPay payment page (URL). 
            this.gotoPaymentPage(this.paymentResult.responseURI, false);

 



          }

        }
        else {
          this.errorMessageDisplay = this.appTranslationService.getTranslation('paymentPopup.ApologiesErrorOccurred');
        }

 
      }
      else {
        this.errorMessageDisplay = this.appTranslationService.getTranslation('paymentPopup.ApologiesErrorOccurred');
      }
        

    },
    // error => console.log('oops', error)
    error => this.errorHandler(error)
  );

} // onClickSubmit(data)


gotoPaymentPage(urlPage, newTab) {

  // redirect in same window/tab
  if (newTab == false) {
    window.location.href = urlPage;
  }
  else {
    // redirect in new tab
    window.open(urlPage, "_blank");
  }

 
}



// Error handling
errorHandler(error) {
  let errorMessage = '';
  if (error.error instanceof ErrorEvent) {
    // Get client-side error
    errorMessage = error.error.message;
    this.errorMessageDisplay = errorMessage;
  } else {
    // Get server-side error
    errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
    this.errorMessageDisplay = errorMessage;
  }
  // console.log(errorMessage);
  return throwError(() =>
  {
    this.errorMessageDisplay = errorMessage;
    return errorMessage;
  });
}

 
  // Generate random number for our trans
getRandomInt(min, max) {
    min = Math.ceil(min);
    max = Math.floor(max);
    return Math.floor(Math.random() * (max - min + 1)) + min;
  }



close() {
    this.dialogRef.close();
  }


 
  // 1st call method to save request set by this app to FOMOpay.
SaveRequestRecord(tokenOrdered: TokenOrderedPayment, paymentMethod: string, outletURL: string, thankyouURL: string, notifyURL:string) {
    // 1st  call create db record
  this.apiService.createRequestRecord(tokenOrdered, paymentMethod, outletURL, thankyouURL, notifyURL).subscribe(
      (data) => {
        this.paymentRequestRecordId = data.body;
        //console.log('createRequestRecord method line 515:  newPaymentRequestRecordId is  :' + this.paymentRequestRecordId);

        if (this.paymentRequestRecordId !== undefined && this.paymentRequestRecordId !== null) {
          //  console.log('createRequestRecord method: line 631 :' + this.paymentRequestRecordId);
        }
        else {
          this.errorMessageDisplay = this.appTranslationService.getTranslation('paymentPopup.ApologiesErrorOccurred');
        }
        //console.log('createRequestRecord method:line 567 :' + this.paymentRequestRecordId);

      },
      error => this.errorHandler(error)
    );


  }


  // 2nd call method to save response from FOMOpay.
SavePaymentResponse(paymentResponse: PaymentResult, tokenOrderedCurrent: TokenOrderedPayment, newRequestRecordID: number) {
    var paymentResponseRecordId = 0;

    this.apiService.createResponseRecord(paymentResponse, tokenOrderedCurrent, newRequestRecordID).subscribe(
      (data) => {
        paymentResponseRecordId = data.body;
        //console.log('SavePaymentResponse: line 589:' + data.body);
        if (paymentResponseRecordId !== undefined && paymentResponseRecordId !== null)
        {

 
          //console.log('line 618 token component/ SavePaymentResponse/ createResponseRecord method  returned value from API is  :' + paymentResponseRecordId);
        }
        else {
          this.errorMessageDisplay = this.appTranslationService.getTranslation('paymentPopup.ApologiesErrorOccurred');
        }
        return paymentResponseRecordId;

      },
      error => this.errorHandler(error)
    );


  }


 
}
