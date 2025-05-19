import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';
import { AccountEndpoint } from '../account-endpoint.service';
import { AuthService } from '../auth.service';
import { CommonEndpoint } from '../common-endpoint.service';
import { ConfigurationService } from '../configuration.service';
import { Filter, PagedResult } from 'src/app/models/sieve-filter.model';
import { PaymentType } from 'src/app/models/meal-order/payment-type.model';
import { TransactionFee } from 'src/app/models/meal-order/transaction-fee.model';
import { VoucherType } from 'src/app/models/meal-order/voucher-type.model';
import { Voucher } from 'src/app/models/meal-order/voucher.model';
import { Waiver } from 'src/app/models/meal-order/waiver.model';
import { CancelOrderRequest } from 'src/app/models/meal-order/cancel-order-request.model';

@Injectable()
export class PaymentService {

  private readonly _paymentTypeUrl: string = "/api/payment/paymenttypes";
  get paymentTypeUrl() { return this.configurations.baseUrl + this._paymentTypeUrl; }

  private readonly _transactionFeeUrl: string = "/api/payment/transactionfees";
  get transactionFeeUrl() { return this.configurations.baseUrl + this._transactionFeeUrl; }

  private readonly _voucherTypeUrl: string = "/api/payment/vouchertypes";
  get voucherTypeUrl() { return this.configurations.baseUrl + this._voucherTypeUrl; }

  private readonly _voucherUrl: string = "/api/payment/vouchers";
  get voucherUrl() { return this.configurations.baseUrl + this._voucherUrl; }

  private readonly _waiverUrl: string = "/api/payment/waivers";
  get waiverUrl() { return this.configurations.baseUrl + this._waiverUrl; }

  private readonly _cancelOrderRequestUrl: string = "/api/tokenorder/tokenorders/cancelorder";
  get cancelOrderRequestUrl() { return this.configurations.baseUrl + this._cancelOrderRequestUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private accountEndpoint: AccountEndpoint, private commonEndpoint: CommonEndpoint, protected configurations: ConfigurationService) {

  }
  //payment type
  getPaymentTypeById(paymentTypeId: string) {

    return this.commonEndpoint.getById<any>(this.paymentTypeUrl + '/get', paymentTypeId);
  }

  getPaymentTypesByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.paymentTypeUrl + '/sieve/list', filter);
  }

  updatePaymentType(paymentType: PaymentType) {
    if (paymentType.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.paymentTypeUrl, paymentType, paymentType.id);
    }
  }


  newPaymentType(paymentType: PaymentType) {
    return this.commonEndpoint.getNewEndpoint<PaymentType>(this.paymentTypeUrl, paymentType);
  }


  deletePaymentType(paymentTypeOrPaymentTypeId: string | PaymentType): Observable<PaymentType> {
    return this.commonEndpoint.getDeleteEndpoint<PaymentType>(this.paymentTypeUrl, <string>paymentTypeOrPaymentTypeId);
  }

  //transaction fee
  getTransactionFeeById(transactionFeeId: string) {

    return this.commonEndpoint.getById<any>(this.transactionFeeUrl + '/get', transactionFeeId);
  }

  getTransactionFeesByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.transactionFeeUrl + '/sieve/list', filter);
  }

  updateTransactionFee(transactionFee: TransactionFee) {
    if (transactionFee.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.transactionFeeUrl, transactionFee, transactionFee.id);
    }
  }


  newTransactionFee(transactionFee: TransactionFee) {
    return this.commonEndpoint.getNewEndpoint<TransactionFee>(this.transactionFeeUrl, transactionFee);
  }


  deleteTransactionFee(transactionFeeOrTransactionFeeId: string | TransactionFee): Observable<TransactionFee> {
    return this.commonEndpoint.getDeleteEndpoint<TransactionFee>(this.transactionFeeUrl, <string>transactionFeeOrTransactionFeeId);
  }

  //voucher type
  getVoucherTypeById(voucherTypeId: string) {

    return this.commonEndpoint.getById<any>(this.voucherTypeUrl + '/get', voucherTypeId);
  }

  getVoucherTypesByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.voucherTypeUrl + '/sieve/list', filter);
  }

  updateVoucherType(voucherType: VoucherType) {
    if (voucherType.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.voucherTypeUrl, voucherType, voucherType.id);
    }
  }


  newVoucherType(voucherType: VoucherType) {
    return this.commonEndpoint.getNewEndpoint<VoucherType>(this.voucherTypeUrl, voucherType);
  }


  deleteVoucherType(voucherTypeOrVoucherTypeId: string | VoucherType): Observable<VoucherType> {
    return this.commonEndpoint.getDeleteEndpoint<VoucherType>(this.voucherTypeUrl, <string>voucherTypeOrVoucherTypeId);
  }


  //voucher 
  getVoucherById(voucherId: string) {

    return this.commonEndpoint.getById<any>(this.voucherUrl + '/get', voucherId);
  }

  getVouchersByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.voucherUrl + '/sieve/list', filter);
  }

  getVoucherStudent(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.voucherUrl + '/sieve/student-list', filter);
  }

  updateVoucher(voucher: Voucher) {
    if (voucher.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.voucherUrl, voucher, voucher.id);
    }
  }


  newVoucher(voucher: Voucher) {
    return this.commonEndpoint.getNewEndpoint<Voucher>(this.voucherUrl, voucher);
  }


  deleteVoucher(voucherOrVoucherId: string | Voucher): Observable<Voucher> {
    return this.commonEndpoint.getDeleteEndpoint<Voucher>(this.voucherUrl, <string>voucherOrVoucherId);
  }

  deleteVoucherStudent(id: string): Observable<any> {
    return this.commonEndpoint.getDeleteEndpoint<any>(`${this.voucherUrl}/student`, id);
  }

  //waiver
  getWaiverById(waiverId: string) {

    return this.commonEndpoint.getById<any>(this.waiverUrl + '/get', waiverId);
  }

  getWaiversByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.waiverUrl + '/sieve/list', filter);
  }

  updateWaiver(waiver: Waiver) {
    if (waiver.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.waiverUrl, waiver, waiver.id);
    }
  }


  newWaiver(waiver: Waiver) {
    return this.commonEndpoint.getNewEndpoint<Waiver>(this.waiverUrl, waiver);
  }


  deleteWaiver(waiverOrWaiverId: string | Waiver): Observable<Waiver> {
    return this.commonEndpoint.getDeleteEndpoint<Waiver>(this.waiverUrl, <string>waiverOrWaiverId);
  }

  //cancel order request
  getCancelOrderRequestById(cancelOrderRequestId: string) {

    return this.commonEndpoint.getById<any>(this.cancelOrderRequestUrl + '/get', cancelOrderRequestId);
  }

  getCancelOrderRequestsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.cancelOrderRequestUrl + '/sieve/list', filter);
  }

  approveCancelRequest(cancelOrderRequest: CancelOrderRequest) {
    return this.commonEndpoint.getNewEndpoint(this.cancelOrderRequestUrl + '/approval', cancelOrderRequest);
  }
}
