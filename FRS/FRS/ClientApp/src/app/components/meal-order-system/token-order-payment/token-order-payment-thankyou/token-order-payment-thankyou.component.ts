import { Component, ViewChild, OnInit, Input, Output, EventEmitter, Inject } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';

import { throwError } from 'rxjs/internal/observable/throwError';
import { Router } from '@angular/router';
import { FormBuilder, FormArray, ValidatorFn } from '@angular/forms';
import { DateAdapter, MatDatepickerInputEvent, MatDialog, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { MAT_MOMENT_DATE_FORMATS } from '@angular/material-moment-adapter';
import { MomentUtcDateAdapter } from 'src/app/helpers/moment-utc-adapter';

import { AppTranslationService } from "../../../../services/app-translation.service";
import { ApiService } from '../../../../services/meal-order-payment/api.service';

/*import { CookieService } from 'ngx-cookie-service';*/
  
@Component({
  selector: 'app-token-order-payment-thankyou',
  templateUrl: './token-order-payment-thankyou.component.html',
  styleUrls: ['./token-order-payment-thankyou.component.css']
})
export class TokenOrderPaymentThankyouComponent implements OnInit
{

  cookieValue: any;
  fomoPayOrderId: any;
  ourOrderNumber: any;
  sessstorage: any;
  errorMessageDisplay: string;

  constructor(private apiService: ApiService,
    public dialogRef: MatDialogRef<TokenOrderPaymentThankyouComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    public dialog: MatDialog 
  )
  {
 

   // this.fomoPayOrderId = sessionStorage.getItem('fomoPayOrderId');
   // this.ourOrderNumber = sessionStorage.getItem('ourOrderNumber');


  }

  ngOnInit()
  {

    this.fomoPayOrderId = "";
    this.ourOrderNumber = "";

    this.fomoPayOrderId = sessionStorage.getItem('fomoPayOrderId'); //
    this.ourOrderNumber = sessionStorage.getItem('ourOrderNumber'); //

    // for test only - actual values saved in session vars in file api.service.ts 
    //this.fomoPayOrderId = "100500220221201278597654";
    //this.ourOrderNumber = "6794151467160136";
    // for test only
 

    if (this.fomoPayOrderId != null && this.ourOrderNumber!= null)
    {

      //  this.fomoPayOrderId = sessionStorage.getItem('fomoPayOrderId'); //
      //  this.ourOrderNumber = sessionStorage.getItem('ourOrderNumber'); //
 
        this.updateOrderStatus();
      }
      else
      {
          console.log('PaymentThankyou module: No/Missing id values to update token order ststus');
      }
 

  }


  updateOrderStatus()
  {
    var updateResult: boolean;

     // Check we have a valid value ie non-null & not undefined
    if ((this.fomoPayOrderId != null) && (this.ourOrderNumber != null))
     {
       // call method
      this.apiService.updateOrderStatus(this.fomoPayOrderId, this.ourOrderNumber).subscribe(
          (data) =>
          {
              //console.log('updateOrderStatus method ');
              this.clearStoredID();
          },
          // error => this.errorHandler(error)
          error => this.clearUp(error)
      );

 
     }

 }



 clearUp(error)
 {
     this.clearStoredID();
     this.errorHandler(error);
}

clearStoredID()
{
    // clear vars after update/error
    console.log('clearing value fomoPayOrderId :' + this.fomoPayOrderId);
    console.log('clearing value ourOrderNumber :' + this.ourOrderNumber);

    this.fomoPayOrderId = "";
    this.ourOrderNumber = "";

    sessionStorage.setItem('fomoPayOrderId', ""); //
    sessionStorage.setItem('ourOrderNumber', ""); //

}




  //Redirect URL's
  returnBack() {
    var outletURL: string = "";
    var baseURL  : string = "";
 
    //1 eg http://localhost:56767
    baseURL = window.location.origin;

    if (baseURL.length > 0) {
      outletURL = baseURL + "/" + "outlets";
    }
 
    window.location.href = outletURL;

  } // end function


  close()
  {
    this.dialogRef.close();
  }

  // Error handling
  errorHandler(error) {
    let errorMessage = '';
    if (error.error instanceof ErrorEvent) {
      // Get client-side error
      errorMessage = error.error.message;
     // this.errorMessageDisplay = errorMessage;
      console.log(errorMessage);

    } else {
      // Get server-side error
      errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
      this.errorMessageDisplay = errorMessage;
      console.log(this.errorMessageDisplay );
    }
    // console.log(errorMessage);
    return throwError(() => {
      this.errorMessageDisplay = errorMessage;
      console.log(this.errorMessageDisplay);
      return errorMessage;
    });
  }




}
