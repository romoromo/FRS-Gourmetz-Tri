using AutoMapper;
using BAL.DTO.MealOrder;
using BAL.Services.Interfaces;
using BAL.Services.Interfaces;
using BAL.Services.Interfaces.MealOrder;
using BAL.Services.MealOrder;
using DAL;
using DAL.Core;
using DAL.Core.Logging;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;
using FRS.Controllers;
using FRS.Helpers;
using FRS.Middleware;
using FRS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NodaTime.Calendars;
using Org.BouncyCastle.Crypto.Operators;
using RestSharp;
using SMV.FOMOPay.CommonHelper;
using SMV.FOMOPay.LoggerHelper;
using SMV.FOMOPay.MessageHelper;
using SMV.FOMOPay.Model;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using LoggingEvents = DAL.Core.Logging.LoggingEvents;

namespace MealOrderPayments.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ILogger _logger;
        private string _paymentResultMessage = "";
        private readonly IConfiguration _configuration;
        private IUnitOfWork _unitOfWork;


        private ITokenOrderService _service;
        private IPaymentService _paymentService;
        private IStudentService _studentService;
        private IStudentWalletService _walletService;
        private readonly IEmailSender _emailSender;
        private readonly IMapper _mapper;


        public OrderController(IConfiguration configuration, IUnitOfWork unitOfWork, ITokenOrderService service, IPaymentService paymentService, IEmailSender emailSender, IStudentService studentService, IMapper mapper, IStudentWalletService walletService)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;

            _service = service;
            _paymentService = paymentService;
            _studentService = studentService;
            _emailSender = emailSender;
            _mapper = mapper;
            _logger = Utilities.CreateLogger<OrderController>();
            _walletService = walletService;
        }




        //////////////////



        //////////////////



        /////////////////////////////////////////////////////////////////////////////////
        ///                     Main Private methods
        /////////////////////////////////////////////////////////////////////////////////
        ///
        private string getBasicCreditals()
        {
            string basicCreditals = "";
            string basicCreditialRaw = "";
            Boolean useDummyCreditials = false;

            if (useDummyCreditials == true)
            {
                basicCreditialRaw = _configuration["FOMOPayMerchantDevelopment:MidKey"] + ":"
                    + _configuration["FOMOPayMerchantDevelopment:PSKKey"];
            }
            else
            {
                basicCreditialRaw = _configuration["FOMOPayMerchantProduction:MidKey"] + ":"
                    + _configuration["FOMOPayMerchantProduction:PSKKey"];
            }

            // Convert string to Base64 encoded string
            basicCreditals = HelperMethod.EncodeTo64(basicCreditialRaw);

            return basicCreditals;
        }

        ///// <summary>
        ///// Convert payment type to string array as required by API.
        ///// </summary>
        ///// <param name="choosenPayment"></param>
        ///// <returns></returns>
        //private string[] GetPaymentList(String choosenPayment)
        //{
        //    String[] sourceOfFunds = new string[1];
        //    IDictionary<String, Boolean> livePaymentList;


        //    livePaymentList = new Dictionary<String, Boolean>();
        //    foreach (var paymentItem in _configuration.GetSection("LiveSourceOfFunds").GetChildren())
        //    {
        //        var key = paymentItem.Key;
        //        Boolean value = Convert.ToBoolean(paymentItem.Value);
        //        livePaymentList.Add(key, value);
        //    }

        //    if (livePaymentList.Count > 0)
        //    {
        //        if (livePaymentList.ContainsKey(choosenPayment) == true)
        //        {
        //            sourceOfFunds[0] = choosenPayment;
        //        }
        //    }

        //    livePaymentList = null;

        //    return sourceOfFunds;

        //}

        /// <summary>
        /// Min order amount is SGD 0.50 as required by stripe.
        /// https://docs.stripe.com/currencies#minimum-and-maximum-charge-amounts
        /// </summary>
        /// <returns></returns>
        private static long GetMinOrderValueStripe()
        {
            return 50;
        }

        /// <summary>
        /// Min order amount is SGD 0.50 as required by fomopay.
        /// </summary>
        /// <returns></returns>
        private decimal GetMinOrderValue()
        {
            String minOrderVal = "";
            Decimal minOrderValue = 0;
            minOrderVal = _configuration["FOMOPaySettings:MinPaymentAmount"];
            minOrderValue = GetOrderAmount(minOrderVal);

            return minOrderValue;

        }


        private decimal GetOrderAmount(String passedValues)
        {
            string[] figuresValues;
            String orderStrAmount = "0";
            decimal orderAmount = 0;
            var test = "";
            try
            {
                if (passedValues.Length > 0)
                {
                    orderAmount = Convert.ToDecimal(passedValues);
                }

            }
            catch (OverflowException)
            {
                // Console.WriteLine("{0} is out of range of the Decimal type.", orderAmount);
                // Just log error 
                EventLogger.CreateEventEntry("Order amount value is not read.", EventLogEntryType.Warning);
            }

            return orderAmount;
        }


        private void ProcessMessage(Boolean isSuccessResult)
        {
            if (isSuccessResult == true)
            {
                _paymentResultMessage = TransactionSuccess.PaymentSuccess;
            }
            else
            {
                _paymentResultMessage = TransactionFailure.PaymentError;
            }
        }


        private void ProcessFailedMessage(String UserFriendlyMessage)
        {
            if (UserFriendlyMessage.Length > 0)
            {
                _paymentResultMessage = TransactionSuccess.PaymentSuccess;
            }

        }


        private string MinOrderPrefix()
        {
            string message = _configuration.GetSection("FOMOPaySettings:DefaultCurrency").Value + " " + GetMinOrderValue().ToString();

            return message;
        }


        private TransException ProcessError(RestResponse fomoRestResponse)
        {
            TransException transExceptionData = null;

            if (fomoRestResponse != null)
            {
                transExceptionData = new TransException();
                transExceptionData.content = fomoRestResponse.Content;
                transExceptionData.hiResult = fomoRestResponse.ErrorException.HResult;
                transExceptionData.helpDescription = "";
                transExceptionData.helpLink = fomoRestResponse.ErrorException.HelpLink;
                transExceptionData.message = fomoRestResponse.ErrorException.Message;
                transExceptionData.source = fomoRestResponse.ErrorException.Source;
                transExceptionData.statusCode = fomoRestResponse.StatusCode.ToString();
                transExceptionData.ResponseStatus = fomoRestResponse.ResponseStatus.ToString();
                transExceptionData.statusDescription = fomoRestResponse.StatusDescription;
                transExceptionData.ResponseAbsoluteURL = fomoRestResponse.ResponseUri.AbsoluteUri;
                transExceptionData.URLPort = fomoRestResponse.ResponseUri.Port;
                transExceptionData.dateLogged = DateTime.Now;
            }

            return transExceptionData;

        }


        /// <summary>
        ///  Calls the external API to get payment details.
        ///  Called in the POST.
        /// </summary>
        /// <param name="custOrder"></param>
        /// <param name="cancelToken"></param>
        /// <returns></returns>
        [HttpPost, HttpPut]
        private async Task<ActionResult<PaymentResult>> ProcessPaymentAsync(SMV.FOMOPay.Model.Order custOrder, CancellationToken cancelToken)
        {
            string authorizationString = "";
            string apiDestination = "";
            string baseFOMOPayUR = "";

            FomoPaymentResponse fomoPaymentResponse = null;
            OrderResponse orderRes = null;
            RestResponse paymentResponse;
            PaymentResult paymentResult;
            TransException transException;

            paymentResult = new PaymentResult();
            transException = new TransException();
            fomoPaymentResponse = new FomoPaymentResponse();



            // Cancel may occur via navigating away from a page or timeout ....
            if (cancelToken.IsCancellationRequested == false)
            {
                authorizationString = _configuration.GetSection("FOMOPaySettings:AuthorizationType").Value + " " + getBasicCreditals();
                baseFOMOPayUR = _configuration.GetSection("FOMOPaySettings:BaseFOMOPayURL").Value;
                apiDestination = _configuration.GetSection("FOMOPaySettings:Orders").Value;

                //RestClient should be thread-safe
                var client = new RestClient(baseFOMOPayUR);
                var request = new RestRequest(apiDestination, Method.Post);

                request.AddHeader("Access-Control-Allow-Origin", _configuration.GetSection("FOMOPayAPIHeader:AccessControlAllowOrigin").Value);
                request.AddHeader("Access-Control-Allow-Methods", _configuration.GetSection("FOMOPayAPIHeader:AccessControlAllowMethods").Value);
                request.AddHeader("Accept", _configuration.GetSection("FOMOPayAPIHeader:Accept").Value);
                request.AddHeader("Referer", _configuration.GetSection("FOMOPayAPIHeader:Referer").Value);
                request.AddHeader("Authorization", authorizationString);

                request.RequestFormat = DataFormat.Json;

                request.AddBody(JsonConvert.SerializeObject(custOrder));

                // Main part of method
                paymentResponse = await client.ExecutePostAsync<RestResponse>(request, cancelToken);

                //   Console.WriteLine(paymentResponse.Content);

                fomoPaymentResponse = JsonConvert.DeserializeObject<FomoPaymentResponse>(paymentResponse.Content);

                // Add manually as FOMO doesnt deal with it.
                // if (fomoPaymentResponse is not null) core 6
                if (fomoPaymentResponse != null)
                {
                    fomoPaymentResponse.tokenOrderId = custOrder.tokenOrderId;
                }


                //// NB Normally, RestSharp doesn't throw an exception if the
                // request fails (for ExecutePostAsync and related commands)
                if (paymentResponse.IsSuccessful == true)
                {

                    paymentResult.isSuccessful = true;
                    paymentResult.paymentTime = DateTime.Now;
                    paymentResult.responseStatus = paymentResponse.ResponseStatus.ToString();
                    paymentResult.message = paymentResponse.Content;


                    //deserialize result  OrderResponse
                    orderRes = JsonConvert.DeserializeObject<OrderResponse>(paymentResponse.Content); ;

                    paymentResult.fomoPaymentResponse = fomoPaymentResponse;
                    paymentResult.responseURI = orderRes.url;
                    paymentResult.statusCode = paymentResponse.StatusCode.ToString();

                    //
                    paymentResponse = null;
                    fomoPaymentResponse = null;
                    request = null;
                    orderRes = null;

                    //returns http 200 status code plus our redirect URL
                    return Ok(paymentResult);
                }
                else
                {
                    //If there is a network transport error (network is down, failed DNS lookup, etc),
                    //or any kind of server error (except 404) RestResponse.ResponseStatus will be
                    //set to Error. In this case we can reytry payment but with PUT. 
                    //Multiple POST requests with the same orde rNo will result in HTTP 409 error.
                    //PUT with the same requesting JSON.

                    //Retry once with PUT if not a 404/409 error.
                    if ((paymentResponse.ResponseStatus == ResponseStatus.Error) &
                          ((paymentResponse.StatusCode != HttpStatusCode.NotFound) || (paymentResponse.StatusCode != HttpStatusCode.Conflict)))
                    {
                        paymentResponse = null;
                        paymentResponse = await client.ExecutePutAsync<RestResponse>(request, cancelToken);

                        if (paymentResponse.IsSuccessful == true)
                        {
                            paymentResult.isCancelled = false;
                            paymentResult.isSuccessful = true;
                            paymentResult.paymentTime = DateTime.Now;
                            paymentResult.responseStatus = paymentResponse.ResponseStatus.ToString();
                            paymentResult.statusCode = paymentResponse.StatusCode.ToString();

                            //deserialize result  OrderResponse
                            orderRes = JsonConvert.DeserializeObject<OrderResponse>(paymentResponse.Content);

                            paymentResult.fomoPaymentResponse = fomoPaymentResponse;
                            paymentResult.responseURI = orderRes.url;

                            paymentResponse = null;
                            fomoPaymentResponse = null;
                            request = null;
                            orderRes = null;

                            //returns http 200 status code plus our redirect URL
                            return Ok(paymentResult);
                        }
                        else
                        {
                            // not successful
                            transException = ProcessError(paymentResponse);

                            // NB isSuccessful/isCancelled default set to false in class contructor
                            // hence no need to set here.
                            paymentResult.transException = transException;
                            paymentResult.responseStatus = Convert.ToString(paymentResponse.ResponseStatus);


                            //Clean up
                            paymentResponse = null;
                            request = null;
                            transException = null;

                            EventLogger.CreateEventEntry("BadRequest:" + transException, EventLogEntryType.Warning);

                            return BadRequest(paymentResult);

                        }

                    }

                    // At this point Post or PUT failed hence should have some data to send to client
                    paymentResult.statusCode = paymentResponse.StatusCode.ToString();

                    transException = ProcessError(paymentResponse);
                    // NB isSuccessful/isCancelled default set to false in class contructor
                    // hence no need to set here.
                    paymentResult.transException = transException;
                    paymentResult.responseStatus = Convert.ToString(paymentResponse.ResponseStatus);

                    paymentResponse = null;
                    request = null;
                    transException = null;

                    EventLogger.CreateEventEntry("BadRequest:" + transException, EventLogEntryType.Warning);

                    return BadRequest(paymentResult);
                }


            } // IsCancellationRequested==false
            else
            {
                // Handle cancel request.
                paymentResult.message = "Request Cancelled By Customer/System";
                EventLogger.CreateEventEntry("Request Cancelled By Customer/System", EventLogEntryType.Warning);
                paymentResult.isCancelled = true;

                transException = new TransException();
                transException.content = default;
                transException.hiResult = default;
                transException.helpDescription = default;
                transException.helpLink = default;
                transException.message = default;
                transException.source = default;
                transException.statusCode = default;
                transException.ResponseStatus = default;
                transException.statusDescription = default;
                transException.ResponseAbsoluteURL = default;
                transException.URLPort = default;
                transException.dateLogged = DateTime.Now;
                paymentResult.transException = transException;

                paymentResponse = null;
                transException = null;

                return Ok(paymentResult);
            }

        }










        /////////////////////////////////////////////////////////////////////////////////
        ///                     End Main Private methods
        /////////////////////////////////////////////////////////////////////////////////



        /////////////////////////////////////////////////////////////////////////////////
        ///                     Main API Methods 
        /////////////////////////////////////////////////////////////////////////////////

        [Route("[action]")]
        [ProducesResponseType(400)]  // Bad request status response 
        [ProducesResponseType(201)]  // Created success status response - TODO add return type for createed  rec id
        //// Save our request response record in database.
        ////
        public async Task<IActionResult> PostPaymentResponseRecord([FromBody] TokenPaymentResponseViewModel tokenPaymentResponse)
        {
            long lnResult = 4521;
            long lnResult2 = 99;
            long newRecordID = 0;

            if (ModelState.IsValid)
            {

                if (tokenPaymentResponse == null)
                    return BadRequest($"{nameof(tokenPaymentResponse)} cannot be null");

                var type = _mapper.Map<TokenPaymentResponse>(tokenPaymentResponse);
                var result = await _unitOfWork.ProcessedPayments.CreateAsync(type);

                newRecordID = Convert.ToInt64(result.Data);

                if (result.IsSuccess)
                {
                    return Ok(newRecordID);
                }
            }

            return BadRequest();

        }


        [Route("[action]")]
        [ProducesResponseType(400)]  // bad request status response 
        [ProducesResponseType(201)]  // Created success status response - TODO add return type for createed  rec id
        //// POST api/<OrderController>
        //// Save our request record in database.
        ////
        public async Task<IActionResult> PostPaymentRequestRecord([FromBody] TokenPaymentRequestViewModel tokenPaymentRequest)
        {
            long lnResult = 4521;
            long lnResult2 = 99;
            long newPaymentRequestID = 0;

            //// 
            if (ModelState.IsValid)
            {

                if (tokenPaymentRequest == null)
                    return BadRequest($"{nameof(tokenPaymentRequest)} cannot be null");

                var type = _mapper.Map<TokenPaymentRequest>(tokenPaymentRequest);
                var result = await _unitOfWork.SubmittedPayments.CreateAsync(type);

                long newRecordID = Convert.ToInt64(result.Data);

                if (result.IsSuccess)
                {
                    return Ok(newRecordID);
                }
            }

            return BadRequest();

        }


        /// <summary>
        /// Calculates HMACSHA256Hash   using key and generated message values.
        /// </summary>
        /// <param name="theMessagePassed"></param>
        /// <param name="strPSKKey"></param>
        /// <returns></returns>
        private static string CalcHMACSHA256Hash(string theMessagePassed, string strPSKKey)
        {

            string result = string.Empty;
            var encode = Encoding.Default;

            byte[]
            hashed = encode.GetBytes(theMessagePassed),
            salt = encode.GetBytes(strPSKKey);
            HMACSHA256 hasher = new HMACSHA256(salt);
            byte[] byteHashedText = hasher.ComputeHash(hashed);
            result = string.Join("", byteHashedText.ToList().Select(b => b.ToString("X2")).ToArray());

            // Pass results back in lowercase
            if (result.Trim().Length > 0)
            {
                result = result.ToLower();
            }

            return result;
        }





        private void WriteToEventLog(string strPrefix, string strErrorMessage)
        {
            EventLogger.CreateEventEntry(strPrefix + ":" + strErrorMessage, EventLogEntryType.Warning);
        }





        /// <summary>
        /// We have to do a query on the order status and update our database table.
        /// </summary>
        /// <param name="sessionId">stripe session id</param>
        /// <param name="fromWebhook">from webhook</param>
        /// <returns>order status</returns>
        private string QueryOrderStatusStripe(string sessionId, bool fromWebhook = false)
        {
            //  Quick check ...
            if (!String.IsNullOrEmpty(sessionId))
            {
                try
                {
                    // Set your secret key. Remember to switch to your live secret key in production.
                    // See your keys here: https://dashboard.stripe.com/apikeys
                    StripeConfiguration.ApiKey = _configuration.GetSection("StripeConfiguration:ApiKey").Value;
                    Console.WriteLine("Fulfilling Checkout Session " + sessionId);

                    // TODO: Make this function safe to run multiple times,
                    // even concurrently, with the same session ID

                    // TODO: Make sure fulfillment hasn't already been
                    // performed for this Checkout Session

                    // Retrieve the Checkout Session from the API with line_items expanded
                    var options = new SessionGetOptions
                    {
                        Expand = new List<string> { "line_items" },
                    };

                    var service = new SessionService();
                    var checkoutSession = service.Get(sessionId, options);

                    // Check the Checkout Session's payment_status property
                    // to determine if fulfillment should be performed
                    //if (checkoutSession.PaymentStatus != "unpaid")
                    //{
                    if (checkoutSession != null)
                    {
                        if (checkoutSession.PaymentStatus != "unpaid")
                        {
                            return "SUCCESS";
                        }
                    }
                } catch (Exception ex)
                {
                    WriteToEventLog("Exception", ex.Message);
                } finally
                {
                }
            }
            return "";
        }

        /// <summary>
        /// We have to do a query on the order status and update our database table.
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        private String QueryOrderStatus(string orderID)
        {
            string authorizationString = string.Empty;
            string apiDestination = string.Empty;
            string baseFOMOPayUR = string.Empty;
            string strOrderStatus = string.Empty;
            string strQueryOrderURL = string.Empty;

            RestClient client;
            RestRequest request;

            QueryOrderResponse queryOrderResponse = null;
            RestResponse getRequestResponse;
            PaymentResult paymentResult;


            paymentResult = new PaymentResult();
            queryOrderResponse = new QueryOrderResponse();

            //  Quick check ...
            if (!String.IsNullOrEmpty(orderID))
            {

                try
                {
                    authorizationString = _configuration.GetSection("FOMOPaySettings:AuthorizationType").Value + " " + getBasicCreditals();
                    baseFOMOPayUR = _configuration.GetSection("FOMOPaySettings:BaseFOMOPayURL").Value;
                    apiDestination = _configuration.GetSection("FOMOPaySettings:Orders").Value;

                    strQueryOrderURL = apiDestination + "/" + orderID;

                    //RestClient should be thread-safe
                    client = new RestClient(baseFOMOPayUR);
                    request = new RestRequest(strQueryOrderURL, Method.Get);

                    request.AddHeader("Access-Control-Allow-Origin", _configuration.GetSection("FOMOPayAPIHeader:AccessControlAllowOrigin").Value);
                    request.AddHeader("Access-Control-Allow-Methods", _configuration.GetSection("FOMOPayAPIHeader:AccessControlAllowMethods").Value);
                    request.AddHeader("Accept", _configuration.GetSection("FOMOPayAPIHeader:Accept").Value);
                    request.AddHeader("Referer", _configuration.GetSection("FOMOPayAPIHeader:Referer").Value);
                    request.AddHeader("Authorization", authorizationString);

                    request.RequestFormat = DataFormat.Json;

                    // Main part of method
                    getRequestResponse = client.ExecuteGet<RestResponse>(request);

                    queryOrderResponse = JsonConvert.DeserializeObject<QueryOrderResponse>(getRequestResponse.Content);

                    //// NB Normally, RestSharp doesn't throw an exception if the
                    // request fails (for ExecutePostAsync and related commands)
                    if (getRequestResponse.IsSuccessful == true)
                    {
                        strOrderStatus = queryOrderResponse.Status;
                        paymentResult.responseStatus = getRequestResponse.ResponseStatus.ToString();
                    }
                    //else
                    //{
                    //    strOrderStatus = orderID + " " + getRequestResponse.Content;
                    //}
                }
                catch (Exception ex)
                {
                    WriteToEventLog("Exception", ex.Message);
                }
                finally
                {
                    // Do clean up code....
                    queryOrderResponse = null;
                    getRequestResponse = null;
                    paymentResult = null;
                    client = null;
                    request = null;

                }

            }  // if ...

            //returns order status
            return strOrderStatus;

        }

        private QueryOrderResponse QueryOrderData(string orderID)
        {
            string authorizationString = string.Empty;
            string apiDestination = string.Empty;
            string baseFOMOPayUR = string.Empty;
            QueryOrderResponse strOrderStatus = null;
            string strQueryOrderURL = string.Empty;

            RestClient client;
            RestRequest request;

            QueryOrderResponse queryOrderResponse = null;
            RestResponse getRequestResponse;
            PaymentResult paymentResult;


            paymentResult = new PaymentResult();
            queryOrderResponse = new QueryOrderResponse();

            //  Quick check ...
            if (!String.IsNullOrEmpty(orderID))
            {

                try
                {
                    authorizationString = _configuration.GetSection("FOMOPaySettings:AuthorizationType").Value + " " + getBasicCreditals();
                    baseFOMOPayUR = _configuration.GetSection("FOMOPaySettings:BaseFOMOPayURL").Value;
                    apiDestination = _configuration.GetSection("FOMOPaySettings:Orders").Value;

                    strQueryOrderURL = apiDestination + "/" + orderID;

                    //RestClient should be thread-safe
                    client = new RestClient(baseFOMOPayUR);
                    request = new RestRequest(strQueryOrderURL, Method.Get);

                    request.AddHeader("Access-Control-Allow-Origin", _configuration.GetSection("FOMOPayAPIHeader:AccessControlAllowOrigin").Value);
                    request.AddHeader("Access-Control-Allow-Methods", _configuration.GetSection("FOMOPayAPIHeader:AccessControlAllowMethods").Value);
                    request.AddHeader("Accept", _configuration.GetSection("FOMOPayAPIHeader:Accept").Value);
                    request.AddHeader("Referer", _configuration.GetSection("FOMOPayAPIHeader:Referer").Value);
                    request.AddHeader("Authorization", authorizationString);

                    request.RequestFormat = DataFormat.Json;

                    // Main part of method
                    getRequestResponse = client.ExecuteGet<RestResponse>(request);

                    queryOrderResponse = JsonConvert.DeserializeObject<QueryOrderResponse>(getRequestResponse.Content);

                    //// NB Normally, RestSharp doesn't throw an exception if the
                    // request fails (for ExecutePostAsync and related commands)
                    if (getRequestResponse.IsSuccessful == true)
                    {
                        strOrderStatus = queryOrderResponse;
                        paymentResult.responseStatus = getRequestResponse.ResponseStatus.ToString();
                    }
                    //else
                    //{
                    //    strOrderStatus = orderID + " " + getRequestResponse.Content;
                    //}
                }
                catch (Exception ex)
                {
                    WriteToEventLog("Exception", ex.Message);
                }
                finally
                {
                    // Do clean up code....
                    queryOrderResponse = null;
                    getRequestResponse = null;
                    paymentResult = null;
                    client = null;
                    request = null;

                }

            }  // if ...

            //returns order status
            return strOrderStatus;

        }

        private static string DecodeUrlString(string url)
        {
            string newUrl;
            while ((newUrl = Uri.EscapeDataString(url)) != url)
                url = newUrl;
            return newUrl;
        }

        private List<QueryOrderResponse> QueryOrderList(string filter, string range, string sort)
        {
            string authorizationString = string.Empty;
            string apiDestination = string.Empty;
            string baseFOMOPayUR = string.Empty;
            List<QueryOrderResponse> strOrderStatus = null;
            string strQueryOrderURL = string.Empty;

            RestClient client;
            RestRequest request;

            List<QueryOrderResponse> queryOrderResponse = null;
            RestResponse getRequestResponse;
            PaymentResult paymentResult;


            paymentResult = new PaymentResult();
            queryOrderResponse = new List<QueryOrderResponse>();

            //  Quick check ...
            //if (!String.IsNullOrEmpty(paramss))
            //{

            try
            {
                authorizationString = _configuration.GetSection("FOMOPaySettings:AuthorizationType").Value + " " + getBasicCreditals();
                baseFOMOPayUR = _configuration.GetSection("FOMOPaySettings:BaseFOMOPayURL").Value;
                apiDestination = _configuration.GetSection("FOMOPaySettings:Orders").Value;


                filter = HttpUtility.UrlEncode(filter);
                range = HttpUtility.UrlEncode(range);
                sort = HttpUtility.UrlEncode(sort);

                strQueryOrderURL = apiDestination + $"?filter={filter}&range={range}&sort={sort}";

                //strQueryOrderURL = DecodeUrlString(strQueryOrderURL);


                //RestClient should be thread-safe
                client = new RestClient(baseFOMOPayUR);
                request = new RestRequest(strQueryOrderURL, Method.Get);

                request.AddHeader("Access-Control-Allow-Origin", _configuration.GetSection("FOMOPayAPIHeader:AccessControlAllowOrigin").Value);
                request.AddHeader("Access-Control-Allow-Methods", _configuration.GetSection("FOMOPayAPIHeader:AccessControlAllowMethods").Value);
                request.AddHeader("Accept", _configuration.GetSection("FOMOPayAPIHeader:Accept").Value);
                request.AddHeader("Referer", _configuration.GetSection("FOMOPayAPIHeader:Referer").Value);
                request.AddHeader("Authorization", authorizationString);

                request.RequestFormat = DataFormat.Json;

                // Main part of method
                getRequestResponse = client.ExecuteGet<RestResponse>(request);

                queryOrderResponse = JsonConvert.DeserializeObject<List<QueryOrderResponse>>(getRequestResponse.Content);

                //// NB Normally, RestSharp doesn't throw an exception if the
                // request fails (for ExecutePostAsync and related commands)
                if (getRequestResponse.IsSuccessful == true)
                {
                    strOrderStatus = queryOrderResponse;
                    paymentResult.responseStatus = getRequestResponse.ResponseStatus.ToString();
                }
                //else
                //{
                //    strOrderStatus = orderID + " " + getRequestResponse.Content;
                //}
            }
            catch (Exception ex)
            {
                WriteToEventLog("Exception", ex.Message);
            }
            finally
            {
                // Do clean up code....
                queryOrderResponse = null;
                getRequestResponse = null;
                paymentResult = null;
                client = null;
                request = null;

            }

            //}  // if ...

            //returns order status
            return strOrderStatus;

        }



        ///// <summary>
        ///// Create a method that can be awaited, but does not return any value.
        ///// In this case we don't need to anything after querying payment and 
        ///// updating database. Hence mark method as Task. rather than very undersiable
        ///// void.
        ///// </summary>
        ///// <param name="objBodyPayload"></param>
        ///// <returns></returns>
        //public async Task QueryAndUpdatePaymentStatus()
        //{
        //    try
        //    {
        //        List<PaymentDTO> payments = await _paymentService.GetCreatedPaymentsAsync();

        //        foreach (var p in payments)
        //        {
        //            var strOrderStatus = QueryOrderStatus(p.PaymentNumber);

        //            if (!String.IsNullOrEmpty(strOrderStatus))
        //            {
        //                if (p.Status != strOrderStatus)
        //                {
        //                    p.Status = strOrderStatus;

        //                    var result = await _paymentService.UpdatePaymentAsync(p);
        //                    if (result.IsSuccess && p.Status == "SUCCESS")
        //                    {
        //                        foreach (var t in p.TokenOrders)
        //                        {
        //                            var dt = await this._service.GetTokenOrderByIdAsync(t.Id);

        //                            if (dt != null)
        //                            {
        //                                if (p.Status == "SUCCESS") dt.Status = "paid";
        //                                var r = await this._service.UpdateTokenOrderAsync(dt);
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteToEventLog("FRS Exception", ex.Message);
        //    }
        //    finally
        //    {
        //    }

        //}







        /// <summary>
        /// Update the token order status. Make it a return type Task as 
        /// no need to return any value to caller.
        /// </summary>
        /// <param name="orderBodyPayload"></param>
        /// <param name="strUpdateStatus"></param>
        /// <returns></returns>
        // private async Task<TokenPaymentResponse> UpdateOrder(BodyPayload orderBodyPayload, string strUpdateStatus)
        private async Task<int> UpdateOrder(BodyPayload orderBodyPayload, string strUpdateStatus)
        {
            string strTransacionOrderId = string.Empty;
            string strError = string.Empty;
            var result2 = string.Empty;
            long paymentResponseRecID = 0;
            UpdateTokenOrderDTO updateTokenOrder = null;
            TokenPaymentResponse objTokenPaymentResponse;
            int tokenOrderID = 0;

            if (ModelState.IsValid)
            {
                try
                {
                    if (orderBodyPayload != null)
                    {
                        // The FOMOPay generated order id really 
                        strTransacionOrderId = orderBodyPayload.OrderId.Trim(); // TransactionId.Trim();

                        if (strTransacionOrderId.Length > 0)
                        {
                            // Create new object UpdateTokenOrderStatus
                            updateTokenOrder = new UpdateTokenOrderDTO();
                            updateTokenOrder.PaymentTransactionId = strTransacionOrderId;
                            updateTokenOrder.StatusCode = strUpdateStatus;

                            // Do a check ..
                            if (updateTokenOrder != null)
                            {
                                //var result1 = await _unitOfWork.ProcessedPayments.GetByIdAsync(strTransacionOrderId);

                                var result = await _unitOfWork.ProcessedPayments.UpdateAsync(strTransacionOrderId, strUpdateStatus);

                                if (result is object)
                                {

                                    Boolean isSuccess = result.IsSuccess;
                                    if (isSuccess == true)
                                    {
                                        objTokenPaymentResponse = (TokenPaymentResponse)result.Data;

                                        if (objTokenPaymentResponse is object)
                                        {
                                            tokenOrderID = objTokenPaymentResponse.TokenOrderId;
                                        }

                                    }

                                }


                            } // if (updateTokenOrder!=null)

                        } // if (strTransacionOrderId.Length>0)

                    }
                    else
                    {
                        strError = "{nameof(orderBodyPayload)} cannot be null";
                        WriteToEventLog("Error", strError);
                    }

                }
                catch (Exception ex)
                {
                    WriteToEventLog("FRS Exception", ex.Message);
                }
                finally
                {
                    orderBodyPayload = null;
                    updateTokenOrder = null;
                }

            }

            return tokenOrderID;

        }





        /// <summary>
        /// Linked to the NotifyURL  To set up client side
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedStatus"></param>
        /// <returns></returns>
        private async Task<IActionResult> UpdateTokenOrder(int tokenOrderID, string updatedStatus)
        {
            if (ModelState.IsValid)
            {
                //if (model == null)
                //    return BadRequest($"{nameof(model)} cannot be null");

                //if (model.Id == 0)
                //    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetTokenOrderByIdAsync(tokenOrderID);


                if (dto == null)
                {
                    return NotFound(tokenOrderID);
                }
                else
                {
                    dto.Status = updatedStatus;
                }


                var result = await this._service.UpdateTokenOrderAsync(dto);
                if (result.IsSuccess)
                    return NoContent();

                // AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }



        /// <summary>
        /// Create a method that can be awaited, but does not return any value.
        /// In this case we don't need to anything after querying payment and 
        /// updating database. Hence mark method as Task. rather than very undesirable
        /// void.
        /// </summary>
        /// <param name="objBodyPayload"></param>
        /// <returns></returns>
        private async Task QueryAndUpdateOrderStatus(BodyPayload objBodyPayload)
        {
            string strOrderId = string.Empty;
            string strOrderNo = string.Empty;
            string strTransactionId = string.Empty;
            string strTransactionNo = string.Empty;
            string strOrderStatus = string.Empty;
            long paymentResponseRecordID = 0;
            int tokenPaymentOrderID = 0;
            string paymentStatus = "";

            // First query fomopay to get order status
            try
            {

                strOrderId = objBodyPayload.OrderId;
                strOrderNo = objBodyPayload.OrderNo;
                strTransactionId = objBodyPayload.TransactionId;
                strTransactionNo = objBodyPayload.TransactionNo;

                ////Test data only below
                //strOrderId = "";
                //strOrderNo = "";
                //strTransactionId = "";
                //strTransactionNo = "";

                // We have send a api GET request to FOMOPay see what the order status is 
                // actually has been updated to.
                strOrderStatus = QueryOrderStatus(strOrderId);

                if (!String.IsNullOrEmpty(strOrderStatus))
                {

                    if (strOrderStatus.ToLower() == "success")
                    {
                        //Now Uupdate db table with updated status
                        tokenPaymentOrderID = await UpdateOrder(objBodyPayload, strOrderStatus);

                        if (tokenPaymentOrderID > 0)
                        {
                            // Since status is success set flag.
                            paymentStatus = "paid";
                            // Updare status Main token order table...i 
                            var updateResult = await UpdateTokenOrder(tokenPaymentOrderID, paymentStatus);
                        }


                    }



                }

            }
            catch (Exception ex)
            {
                WriteToEventLog("FRS Exception", ex.Message);
            }
            finally
            {
                // Do clean up code....
                objBodyPayload = null;
            }

        }



        /// <summary>  for devug only
        /// We have received a notificaiton from FOMOpay about our order.
        /// We don't really know how long (after our initial POST order) before this method is called 
        /// by FOMOPay. But they said should be almost straigght away.
        /// PostProcessNotifyURL needs to be public URL, hence need to decorate with
        /// [AllowAnonymous]
        /// The content type is "application/json" and the request body is a raw JSON string 
        /// (not a JSON object).
        /// </summary>
        /// <param name="rawJsonData"></param>
        /// <returns></returns>
        [Route("[action]")] // need to enable this when using tesing with POSTMAN as well
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(400)]  // Bad request status response 
        [ProducesResponseType(200)]  // Sucess send after conforming request is valid and authenicated
        public async Task<IActionResult> PostProcessNotifyURL([FromBody] JObject rawJsonData)
        {
            string token = string.Empty;
            string strOurCreditial = "";
            StringValues extractedValue = default(StringValues);
            string strValue = default(StringValues);
            Boolean blnExtactedOk = false;
            Boolean blnWithinTimeLimit = false;
            Boolean blnRequestValid = false;
            Boolean blnExtractedNonceValid = false;
            Boolean blnFirstCheckValid = false;
            Boolean blnSuccess = false;
            int intNonceLength = 0;
            int intTimeSpan;

            string strExtractedVersion = string.Empty;
            string strExtractedCredential = string.Empty;
            string strExtractedNonce = string.Empty;
            string strExtractedTimeStamp = string.Empty;
            string strExtractedSignature = string.Empty;
            string strExtractedBodyPayload = string.Empty;
            string strOurMessage = string.Empty;
            string strRequiredSignature = string.Empty;
            string strOurPSKKey = string.Empty;
            string strCalculatedSignature = string.Empty;
            string strNotifyURLNonceTimeSpan = string.Empty;
            string strReferer = string.Empty;

            DateTime dtExtractedTimeStamp;
            DateTime dtTimeOffsetLower;
            DateTime dtTimeOffsetUpper;
            DateTime dtCurrentTime;
            long unixTimeStamp = 0;
            string unixTimeStampStr = "";
            string fomopayData;
            Dictionary<string, string> headersDict = null;

            var requestCurrent = Request;
            var headers = requestCurrent.Headers;

            // API uses Content-Type header to select a formatter ie (as sent by FOMOPay)
            // application/json. API will read value rawJsonData from the [FromBody],
            // and will populate in string format.
            // var extractedBodyPayload = rawJsonData.Trim(); ;

            // Copy headers into dictionary and make the keys lowercase 
            // FOMOpay may supply them in either lower or uppercase
            // Dictionary<string, string> headersDict3 = headers.ToDictionary(a => a.Key.ToLower(), a => string.Join("", a.Value));

            if (headers.ContentLength > 0)
            {
                fomopayData = headers["x-fomopay-authorization"];
                fomopayData = fomopayData.ToLower();
                fomopayData = fomopayData.Replace("fomopay1-hmac-sha256", ""); // don't need this.

                headersDict = fomopayData.Split(',')
                    .Select(s => s.Split('='))
                    .ToDictionary(a => a[0].Trim().ToLower(), a => a[1].Trim().ToLower());

            }

            /// represents 300 seconds as specified in fomppay apu doc
            strNotifyURLNonceTimeSpan = _configuration.GetSection("FOMOPaySettings:NotifyURLNonceTimeSpan").Value;
            blnSuccess = int.TryParse(strNotifyURLNonceTimeSpan, out intTimeSpan);

            strExtractedBodyPayload = JsonConvert.SerializeObject(rawJsonData);

            // Temp 
            // WriteToEventLog("Exception", "PostProcessNotifyURL line 850. Json body"+ strExtractedBodyPayload);
            // Temp 

            // This is not really used but maybe useful later on...
            if (headersDict.ContainsKey("referer"))
            {
                blnExtactedOk = headersDict.TryGetValue("referer", out strReferer);
            }

            //////////////////////////////////////////////
            /// Extract the main fields from the header
            //////////////////////////////////////////////

            // version
            if (headersDict.ContainsKey("version"))
            {
                blnExtactedOk = headersDict.TryGetValue("version", out strExtractedVersion);
                if (blnExtactedOk == true)
                {
                    strExtractedVersion = strExtractedVersion.Trim();
                }
            }
            // Credential
            if (headersDict.ContainsKey("credential"))
            {
                blnExtactedOk = headersDict.TryGetValue("credential", out strExtractedCredential);
                if (blnExtactedOk == true)
                {
                    strExtractedCredential = strExtractedCredential.Trim();
                }
            }

            //nonce
            if (headersDict.ContainsKey("nonce"))
            {
                blnExtactedOk = headersDict.TryGetValue("nonce", out strValue);
                if (blnExtactedOk == true)
                {
                    strExtractedNonce = strValue.ToString().Trim();
                    intNonceLength = strExtractedNonce.Length;
                    if ((intNonceLength >= 16) && (intNonceLength <= 64))
                    {
                        blnExtractedNonceValid = true;
                    }

                }
            }

            //Timestamp
            if (headersDict.ContainsKey("timestamp"))
            {
                blnExtactedOk = headersDict.TryGetValue("timestamp", out unixTimeStampStr);
                if (blnExtactedOk == true)
                {
                    // dtExtractedTimeStamp = Convert.ToDateTime(strExtractedTimeStamp);
                    // unixTimeStamp = ((DateTimeOffset)dtExtractedTimeStamp).ToUnixTimeSeconds();
                    unixTimeStampStr = unixTimeStampStr.ToString().Trim();
                }
            }

            dtCurrentTime = DateTime.Now;
            dtTimeOffsetLower = dtCurrentTime.AddSeconds(-intTimeSpan);
            dtTimeOffsetUpper = dtCurrentTime.AddSeconds(intTimeSpan);

            if ((dtCurrentTime >= dtTimeOffsetLower) && (dtCurrentTime <= dtTimeOffsetUpper))
            {
                blnWithinTimeLimit = true;
            }

            //Signature
            if (headersDict.ContainsKey("signature"))
            {
                blnExtactedOk = headersDict.TryGetValue("signature", out strExtractedSignature);
                if (blnExtactedOk == true)
                {
                    strExtractedSignature = strExtractedSignature.Trim();
                }
            }


            // temp
            //blnWithinTimeLimit = true;
            // temp

            // NB we need to 2 checks as specifiled in fomopay api doc

            ///////////////////////////////////////////
            // 1st Check - if valid state
            ///////////////////////////////////////////


            // Midkey is just a series of numbers.
            strOurCreditial = _configuration.GetSection("FOMOPayMerchantProduction:MidKey").Value;

            if ((strExtractedVersion == "1.1") && (strOurCreditial == strExtractedCredential) &&
                (blnExtractedNonceValid == true) && (blnWithinTimeLimit == true))
            {
                // Valid request
                blnFirstCheckValid = true;
            }
            else
            {
                // Reject request
                return BadRequest();   // rem this during testing  code.
            }


            // debug only
            // blnFirstCheckValid = true;
            // debug only

            ///////////////////////////////////////////
            //  Part 2 check messages signatures 
            ///////////////////////////////////////////


            strOurPSKKey = _configuration.GetSection("FOMOPayMerchantProduction:PSKKey").Value;
            if (blnFirstCheckValid == true)
            {
                // Form our message
                strOurMessage = strExtractedBodyPayload + unixTimeStampStr + strExtractedNonce;

                // calculate signature
                strCalculatedSignature = CalcHMACSHA256Hash(strOurMessage, strOurPSKKey);

                // temp JUST FOR test data for the CODE
                //unixTimeStamp = 1577808000;
                //strExtractedNonce = "b39c7ec8fa58be1041eb3921c9ceb98b";
                //string strFormedMessage = strExtractedBodyPayload + unixTimeStamp + strExtractedNonce;
                //strOurPSKKey = "E00F270DE323E2B187532D8E4B306EB2841AF0BFF08132BAB7F0E62BED6419BB";
                //strCalculatedSignature = CalcHMACSHA256Hash(strFormedMessage, strOurPSKKey);
                //strExtractedSignature = "596ecb8f2636ff88eea7b4d4b4841ae822eaa4f1eea9cb1ce1da2953c9db0b05";
                // temp JUST FOR test for the CODE

                // debug only
                //strCalculatedSignature = "";
                //strExtractedSignature = "";
                // debug only

                // If match, we have valid request
                if (strCalculatedSignature == strExtractedSignature)
                {
                    BodyPayload objBodyPayload = JsonConvert.DeserializeObject<BodyPayload>(strExtractedBodyPayload);

                    // temp JUST FOR test for the CODE
                    // objBodyPayload.TransactionId = "100500020220916439138838"; /// temp test
                    // temp JUST FOR test for the CODE


                    // Temp 
                    // WriteToEventLog("Exception", "PostProcessNotifyURL line 1045" + strExtractedBodyPayload);
                    // Temp 

                    // calll asynch method to get query order status from fomopay & update db
                    await QueryAndUpdateOrderStatus(objBodyPayload);

                    // While awaiting QueryAndUpdateOrderStatus we return 
                    // ok status back to the caller ie
                    // the FOMOPay system.
                    return Ok();
                }
                else
                {
                    // Otherwise reject
                    WriteToEventLog("Exception", "Badrequest in PostProcessNotifyURL. Signatures not match ");
                    return BadRequest();
                }

            }

            WriteToEventLog("Exception", "Badrequest in PostProcessNotifyURL ");
            return BadRequest(); //  return bad request 

        }




        /// <summary>  for devug only
        /// We have received a notificaiton from FOMOpay about our order.
        /// We don't really know how long (after our initial POST order) before this method is called 
        /// by FOMOPay. But they said should be almost straigght away.
        /// PostProcessNotifyURL needs to be public URL, hence need to decorate with
        /// [AllowAnonymous]
        /// The content type is "application/json" and the request body is a raw JSON string 
        /// (not a JSON object).
        /// </summary>
        /// <param name="rawJsonData"></param>
        /// <returns></returns>
        [Route("[action]")] // need to enable this when using tesing with POSTMAN as well
        [HttpPost]
        [ProducesResponseType(400)]  // Bad request status response 
        [ProducesResponseType(200)]  // Sucess send after conforming request is valid and authenicated
        public async Task<IActionResult> PostProcessNotifyURLManualUpdate([FromBody] JObject rawJsonData)
        {
            string token = string.Empty;
            string strOurCreditial = "";
            StringValues extractedValue = default(StringValues);
            string strValue = default(StringValues);
            Boolean blnExtactedOk = false;
            Boolean blnWithinTimeLimit = false;
            Boolean blnRequestValid = false;
            Boolean blnExtractedNonceValid = false;
            Boolean blnFirstCheckValid = false;
            Boolean blnSuccess = false;
            int intNonceLength = 0;
            int intTimeSpan;

            string strExtractedVersion = string.Empty;
            string strExtractedCredential = string.Empty;
            string strExtractedNonce = string.Empty;
            string strExtractedTimeStamp = string.Empty;
            string strExtractedSignature = string.Empty;
            string strExtractedBodyPayload = string.Empty;
            string strOurMessage = string.Empty;
            string strRequiredSignature = string.Empty;
            string strOurPSKKey = string.Empty;
            string strCalculatedSignature = string.Empty;
            string strNotifyURLNonceTimeSpan = string.Empty;
            string strReferer = string.Empty;

            DateTime dtExtractedTimeStamp;
            DateTime dtTimeOffsetLower;
            DateTime dtTimeOffsetUpper;
            DateTime dtCurrentTime;
            long unixTimeStamp = 0;
            string unixTimeStampStr = "";
            string fomopayData;
            Dictionary<string, string> headersDict = null;

            var requestCurrent = Request;
            var headers = requestCurrent.Headers;

            strExtractedBodyPayload = JsonConvert.SerializeObject(rawJsonData).Trim();

            if (strExtractedBodyPayload.Length > 0)
            {


                BodyPayload objBodyPayload = JsonConvert.DeserializeObject<BodyPayload>(strExtractedBodyPayload);

                // temp JUST FOR test for the CODE
                // objBodyPayload.TransactionId = "100500020220916439138838"; /// temp test
                // temp JUST FOR test for the CODE


                // Temp 
                WriteToEventLog("Exception", "PostProcessNotifyURLManualUpdate line 1247" + strExtractedBodyPayload);
                // Temp 

                // calll asynch method to get query order status from fomopay & update db
                await QueryAndUpdateOrderStatus(objBodyPayload);

                // While awaiting QueryAndUpdateOrderStatus we return 
                // ok status back to the caller ie
                // the FOMOPay system.
                return Ok();


            }

            WriteToEventLog("Exception", "Badrequest in PostProcessNotifyURL ");
            return BadRequest(); //  return bad request 

        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> NotifyStripe()
        {
            // Use the secret provided by Stripe CLI for local testing
            // or your webhook endpoint's secret.
            var secret = _configuration.GetSection("StripeConfiguration:WebhookSecret").Value;
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], secret);
                // TODO what events do we need to listen?
                // currently only listening for CheckoutSessionCompleted.
                // If on SDK version < 46, use class Events instead of EventTypes
                if (stripeEvent.Type == Events.CheckoutSessionCompleted)
                {
                    var sess = stripeEvent.Data.Object as Stripe.Checkout.Session;
                    var options = new SessionGetOptions
                    {
                        Expand = new List<string> { "line_items" },
                    };
                    var service = new SessionService();
                    var checkoutSession = service.Get(sess.Id, options);

                    // Check the Checkout Session's payment_status property
                    // to determine if fulfillment should be performed
                    if (checkoutSession != null)
                    {
                        if (checkoutSession.PaymentStatus != "unpaid")
                        {
                            var first = checkoutSession.LineItems.First();
                            if (first != null)
                            {
                                var custom = first.Description;
                                if (custom.ToUpper().Equals("TOP UP WALLET"))
                                {
                                    await QueryAndUpdateWalletPaymentStatusStripe(sess.Id, true);
                                } else
                                {
                                    await QueryAndUpdatePaymentStatusStripe(sess.Id, true);
                                }
                                return Ok();
                            } else
                            {
                                return BadRequest("display items is null");
                            }
                        } else
                        {
                            return BadRequest("payment is unpaid");
                        }
                    } else
                    {
                        return BadRequest("checkout session is null");
                    }
                } else
                {
                    EventLogger.CreateEventEntry("Not the stripe event type we check", EventLogEntryType.Error);
                    return BadRequest();
                }
            } catch (StripeException sec)
            {
                EventLogger.CreateEventEntry(sec.ToString(), EventLogEntryType.Error);
                return BadRequest();
            }
        }

        /// <summary>
        /// PostStripe
        /// </summary>
        /// <param name="custTransaction"></param>
        /// <returns></returns>
        /// POST api/<OrderController>
        [Route("[action]")]
        [HttpPost]
        public async Task<PaymentResult> PostStripe(SMV.FOMOPay.Model.Order order)
        {
            StripeConfiguration.ApiKey = _configuration.GetSection("StripeConfiguration:ApiKey").Value;
            // validate total here
            long amt = 0;
            try
            {
                var culture = CultureInfo.CreateSpecificCulture("en-SG");
                var damt = decimal.Parse(order.amount, culture);
                // old stripe api versions cant use decimal
                damt *= 100;
                amt = (long)damt;
            } catch (Exception ex)
            {
                //TODO want to fail here
                // For internal use
                //Debug.WriteLine("Exception occured:" + ex);

                // Show user friendly error message
                ProcessMessage(false);
                var erm = "Exception occurred in OrderController PostStripe method of API";
                EventLogger.CreateEventEntry(erm + "." + ex.Message, EventLogEntryType.Error);
                throw new Exception(erm, ex);
            }
            if (amt < GetMinOrderValueStripe())
            {
                var z = _configuration.GetSection("UserMessages:MinPaymentError").Value + MinOrderPrefix();
                return ProcessBadRequest(order, z);
            }
            if (order.orderNo.Length == 0)
            {
                var z = _configuration.GetSection("UserMessages:NoOrderNumberError").Value;
                return ProcessBadRequest(order, z);
            }
            Dictionary<string, string> mappings = new Dictionary<string, string>();
            //mappings.Add(db, "stripe_enum");
            mappings.Add("CARD", "card");
            //https://docs.stripe.com/payments/grabpay/accept-a-payment?payment-ui=checkout#enable-grabpay-as-a-payment-method
            mappings.Add("GRABPAY", "grabpay");
            //https://docs.stripe.com/payments/paynow/accept-a-payment?payment-ui=checkout#enable-paynow-as-a-payment-method
            mappings.Add("PAYNOW", "paynow");

            string pmCode = order.sourceOfFunds.Last();
            List<string> paymentMethods = new List<string>();
            if (pmCode != null)
            {
                // map sof pm to stripe pm here.
                if (mappings.TryGetValue(pmCode, out string found))
                {
                    paymentMethods.Add(found);
                }
            }

            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                  new SessionLineItemOptions
                  {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                      UnitAmount = amt,
                      Currency = order.currencyCode,
                      ProductData = new SessionLineItemPriceDataProductDataOptions
                      {
                        Name = order.subject, //subject: 'Token Meal Payment',
                        Description = order.description,// description: `Payment for ${student.name}`,
                        Metadata = new Dictionary<string, string>
                        {
                            { "orderNo", order.orderNo }
                        }
                      },
                    },
                    //Amount = amt,
                    //Currency = order.currencyCode,
                    Quantity = 1,
                    // TODO probably can do something more explicit to differentiate
                    // between cart and topup wallet here?
                    //Name = order.subject, //subject: 'Token Meal Payment', / 'Top Up Wallet'
                    //Description = order.description, // description: `Payment for ${student.name}`, 
                  },
                },
                Mode = "payment",
                //SuccessUrl = order.returnUrl,
                SuccessUrl = order.returnUrl + "?&session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = order.backUrl,
                PaymentMethodTypes = paymentMethods
            };

            var service = new SessionService();
            Session session = service.Create(options);
            // old Stripe API version cant use `Session.Url` directly. 
            // see https://github.com/stripe/stripe-dotnet/blob/master/CHANGELOG.md#39540---2021-06-16
            try
            {
                PaymentResult ret = new PaymentResult();
                var rawjsonstring = session.StripeResponse.Content;
                var splitted = rawjsonstring.Split(",");
                var uri = splitted.Where(s => s.Contains("checkout.stripe.com")).First();
                var s2 = uri.Split(": ");
                var uri2 = s2[1].Replace('\"', ' ').Trim();
                ret.responseURI = uri2;
                // TODO this is dirty. fix it later.
                // because we dont want to break paymentresult, reuse this for now.
                ret.fomoPaymentResponse = new FomoPaymentResponse();
                ret.fomoPaymentResponse.id = session.Id;
                // I hope this will be helpful when debugging frontend
                ret.fomoPaymentResponse.description = "Manually created. This is created in PostStripe";
                ret.isSuccessful = true;
                return ret;
            } catch (Exception ex)
            {
                var z = ex.Message;
                EventLogger.CreateEventEntry(z, EventLogEntryType.Error);
                return ProcessBadRequest(order, z);
            }
        }

        /// <summary>
        /// Main method 
        /// </summary>
        /// <param name="custTransaction"></param>
        /// <returns></returns>
        /// POST api/<OrderController>
        [HttpPost]
        public PaymentResult Post(SMV.FOMOPay.Model.Order custTransaction)
        {
            String redirectToPaymentURL = "";
            String paymentSelected = "";
            String minOrderMessage = "";
            Decimal discountedPayment = 0;
            Decimal orderValue = 0;
            string errorMessage = "";
            string returnOrderMessage = "";

            Transaction custTransactionData = null;
            TransactionOptions transactionOptions = null;
            PaymentItem[] paymentItems = null;
            Amount paymentAmount = null;
            SMV.FOMOPay.Model.Address shipAddrs = null;
            PaymentDiscount[] paymentDiscountList = null;
            bool successfulConvert = false;
            PaymentResult dataReturnedObject = default;

            String[] selectedPayment;

            string settings = "";
            string tests = "";

            //initializes to non-null; can be reassigned a value of any type
            Object dataReturnedResult = new object();
            ActionResult<PaymentResult> dataReturned;
            Type dataReturnedResultType;

            var cancellationTokenSource = new CancellationTokenSource();
            //cancellationTokenSource.CancelAfter(1);
            var cancellationToken = cancellationTokenSource.Token;
            FomoPaymentResponse objFomoPaymentResponse;
            SMV.FOMOPay.Model.Order cusOrder = new SMV.FOMOPay.Model.Order();

            cusOrder.mode = custTransaction.mode;
            cusOrder.orderNo = custTransaction.orderNo;
            cusOrder.tokenOrderId = custTransaction.tokenOrderId;
            cusOrder.subject = custTransaction.subject;
            cusOrder.description = custTransaction.description;
            cusOrder.amount = custTransaction.amount;
            cusOrder.currencyCode = custTransaction.currencyCode;
            cusOrder.notifyUrl = custTransaction.notifyUrl;
            cusOrder.returnUrl = custTransaction.returnUrl;
            cusOrder.backUrl = custTransaction.backUrl;
            cusOrder.sourceOfFunds = custTransaction.sourceOfFunds;
            cusOrder.transactionOptions = custTransaction.transactionOptions;

            settings = _configuration.GetSection("ApplicationSettings:DefaultPayment").Value;

            try
            {


                //  
                if (custTransaction != null)
                {

                    if ((custTransaction.orderNo != null) && (custTransaction.orderNo != ""))
                    {
                        /// Check for min order amount set by FOMOPay. 
                        orderValue = GetOrderAmount(custTransaction.amount);
                        if (orderValue > GetMinOrderValue())
                        {

                            // Should be  CARD, GrabPay,etc Single value only.
                            if (custTransaction.sourceOfFunds.Length > 0)
                            {
                                paymentSelected = custTransaction.sourceOfFunds[0];
                            }

                            successfulConvert = Decimal.TryParse(custTransaction.amount, out discountedPayment);
                            //if (successfulConvert)
                            //{
                            //    Console.WriteLine($"Converted '{custTransaction.amount}' ok");
                            //}
                            //else
                            //{
                            //    Console.WriteLine($"Attempted conversion of '{custTransaction.amount ?? "<null>"}' failed.");
                            //}


                            if ((custTransaction.orderNo.Length > 0) &&
                                  (paymentSelected != null) && discountedPayment > 0)
                            {


                                if (paymentSelected.Length > 0)
                                {

                                    // sourceFunds = new string[12] { "CARD", "PAYNOW", "NETSPAY", "GRABPAY", "WECHATPAY", "ALIPAY", "UNIONPAY", "SHOPEEPAY", "ATOME", "DPT", "BROWSER", "PAYPAL" };
                                    custTransaction.mode = _configuration.GetSection("FOMOPaySettings:PaymentMode").Value;

                                    // NB:We can't change currency used in the FOMO payment page ourselves.
                                    // Need to contact FOMOPay. Currently can only use SGD.
                                    custTransaction.currencyCode = _configuration.GetSection("FOMOPaySettings:DefaultCurrency").Value;


                                    var asynchRedirectPayment = ProcessPaymentAsync(custTransaction, cancellationToken);

                                    //Need to ref to asynch otherwise won't complete async
                                    //redirect.
                                    var asynchResult = asynchRedirectPayment.Result;

                                    if (asynchRedirectPayment.IsCompletedSuccessfully == true)
                                    {
                                        dataReturned = asynchRedirectPayment.Result;
                                        dataReturnedResult = dataReturned.Result;
                                        dataReturnedResultType = dataReturnedResult.GetType();

                                        if (dataReturnedResultType.Name.ToUpper() == "OKOBJECTRESULT")
                                        {
                                            if (dataReturned != null)
                                            {

                                                var okObjectResult = (OkObjectResult)dataReturned.Result;
                                                dataReturnedObject = (PaymentResult)okObjectResult.Value;
                                                redirectToPaymentURL = dataReturnedObject.responseURI;

                                                // We should have successful api call to FOMO
                                                // and hence a payment URl etc ...
                                                if (redirectToPaymentURL.Length > 1)
                                                {
                                                    //  Order status update .....
                                                    //objFomoPaymentResponse = (FomoPaymentResponse)dataReturnedObject.fomoPaymentResponse;
                                                    //QueryAndUpdateOrderStatus(objFomoPaymentResponse);
                                                    //

                                                    ProcessMessage(true);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ProcessMessage(false);

                                            // Just extra check
                                            if (dataReturned != null)
                                            {
                                                // Should be BadRequestObjectResult
                                                dataReturnedObject = new PaymentResult();

                                                var badRequestObjectResult = (BadRequestObjectResult)dataReturned.Result;
                                                dataReturnedObject = (PaymentResult)badRequestObjectResult.Value;

                                                // Return cust order info when we have unsuccessful or
                                                // unprocessed payment
                                                custTransactionData = null;
                                            }


                                        } //if (dataReturnedResultType.Name.ToUpper() == "OKOBJECTRESULT")
                                    } //  if (asynchRedirectPayment.IsCompletedSuccessfully == true)

                                } // if (paymentSelected.Length>0)


                            } //if ((custTransaction.customerOrder.orderNo.Length > 0) &&   (paymentSelected != null))


                        }  //if (orderValue > GetMinOrderValue() )   
                        else
                        {

                            //custTransactionData = GetDummyOrder();
                            custTransactionData = null;

                            minOrderMessage = _configuration.GetSection("UserMessages:MinPaymentError").Value + MinOrderPrefix();
                            dataReturnedObject = ProcessBadRequest(custTransaction, minOrderMessage);


                        } //   if (orderValue > GetMinOrderValue())

                    }
                    else // no order number
                    {

                        // unsuccessful result.
                        custTransactionData = null;

                        returnOrderMessage = _configuration.GetSection("UserMessages:NoOrderNumberError").Value;
                        dataReturnedObject = ProcessBadRequest(custTransaction, returnOrderMessage);

                    } //if (custTransaction.customerOrder.orderNo != null)


                }
                else   // no order data
                {
                    // unsuccessful result.
                    custTransactionData = null;

                    returnOrderMessage = _configuration.GetSection("UserMessages:NoOrderDataError").Value;
                    dataReturnedObject = ProcessBadRequest(custTransaction, returnOrderMessage);

                }

            }
            catch (Exception ex)
            {

                // For internal use
                //Debug.WriteLine("Exception occured:" + ex);

                // Show user friendly error message
                ProcessMessage(false);

                errorMessage = "Exception occurred in OrderController POST method of API";

                EventLogger.CreateEventEntry(errorMessage + "." + ex.Message, EventLogEntryType.Error);

                throw new Exception(errorMessage, ex);

            }
            finally
            {
                // cleanup.
                transactionOptions = null;
                paymentItems = null;
                paymentAmount = null;
                shipAddrs = null;
                dataReturned = null;
                dataReturnedResultType = null;
                cusOrder = null;
                dataReturnedResultType = null;
                cancellationTokenSource.Dispose();

            }


            /// set  
            return dataReturnedObject;

        }



        private PaymentResult ProcessBadRequest(SMV.FOMOPay.Model.Order customerRequestData, String returnMessage)
        {
            PaymentResult dataReturnedDataObject;
            string prefix;

            string jsonObject = JsonConvert.SerializeObject(customerRequestData, Formatting.None);
            prefix = "Request data: ";

            dataReturnedDataObject = new PaymentResult();
            dataReturnedDataObject.message = returnMessage + ". " + prefix + jsonObject;
            dataReturnedDataObject.paymentTime = DateTime.Now;

            return dataReturnedDataObject;

        }



        private TransException ProcessException(Exception exception)
        {
            TransException transExceptionData = null;

            if (exception != null)
            {
                transExceptionData = new TransException();
                //transExceptionData.content        = ;
                transExceptionData.hiResult = exception.HResult;
                transExceptionData.helpDescription = "";
                transExceptionData.message = exception.Message;
                transExceptionData.source = exception.Source;
                transExceptionData.dateLogged = DateTime.Now;
                //transExceptionData.statusCode     = "";
                //transExceptionData.ResponseStatus = "";
                //transExceptionData.statusDescription      = "";
                // transExceptionData.ResponseAbsoluteURL   = "";
                ///transExceptionData.URLPort               = "";
            }

            return transExceptionData;

        }

        [HttpGet("checkstatus/{id}")] // need to enable this when using tesing with POSTMAN
        [AllowAnonymous]
        [ProducesResponseType(400)]  // Bad request status response 
        [ProducesResponseType(200)]  // Sucess send after conforming request is valid and authenicated
        public IActionResult GetOrderStatus(string id)
        {
            return Ok(QueryOrderStatus(id));
        }

        [HttpGet("checkdata/{id}")] // need to enable this when using tesing with POSTMAN
        [AllowAnonymous]
        [ProducesResponseType(400)]  // Bad request status response 
        [ProducesResponseType(200)]  // Sucess send after conforming request is valid and authenicated
        public IActionResult GetOrderData(string id)
        {
            return Ok(QueryOrderData(id));
        }

        [HttpGet("checklist")] // need to enable this when using tesing with POSTMAN
        [AllowAnonymous]
        [ProducesResponseType(400)]  // Bad request status response 
        [ProducesResponseType(200)]  // Sucess send after conforming request is valid and authenicated
        public IActionResult GetOrderList(string filter, string range, string sort)
        {
            return Ok(QueryOrderList(filter, range, sort));
        }



        /// <summary>
        /// We have to do a query on the order status and update our database table.
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        //[Route("[action]")] // need to enable this when using tesing with POSTMAN
        //[AllowAnonymous]
        //[HttpGet]
        //[ProducesResponseType(400)]  // Bad request status response 
        //[ProducesResponseType(200)]  // Sucess send after conforming request is valid and authenicated
        //private String QueryOrderStatus(string orderID)
        //{
        //    string authorizationString = string.Empty;
        //    string apiDestination = string.Empty;
        //    string baseFOMOPayUR = string.Empty;
        //    string strOrderStatus = string.Empty;
        //    string strQueryOrderURL = string.Empty;

        //    RestClient client;
        //    RestRequest request;

        //    QueryOrderResponse queryOrderResponse = null;
        //    RestResponse getRequestResponse;
        //    PaymentResult paymentResult;


        //    paymentResult = new PaymentResult();
        //    queryOrderResponse = new QueryOrderResponse();

        //    //  Quick check ...
        //    if (!String.IsNullOrEmpty(orderID))
        //    {

        //        try
        //        {
        //            authorizationString = _configuration.GetSection("FOMOPaySettings:AuthorizationType").Value + " " + getBasicCreditals();
        //            baseFOMOPayUR = _configuration.GetSection("FOMOPaySettings:BaseFOMOPayURL").Value;
        //            apiDestination = _configuration.GetSection("FOMOPaySettings:Orders").Value;

        //            strQueryOrderURL = apiDestination + "/" + orderID;

        //            //RestClient should be thread-safe
        //            client = new RestClient(baseFOMOPayUR);
        //            request = new RestRequest(strQueryOrderURL, Method.Get);

        //            request.AddHeader("Access-Control-Allow-Origin", _configuration.GetSection("FOMOPayAPIHeader:AccessControlAllowOrigin").Value);
        //            request.AddHeader("Access-Control-Allow-Methods", _configuration.GetSection("FOMOPayAPIHeader:AccessControlAllowMethods").Value);
        //            request.AddHeader("Accept", _configuration.GetSection("FOMOPayAPIHeader:Accept").Value);
        //            request.AddHeader("Referer", _configuration.GetSection("FOMOPayAPIHeader:Referer").Value);
        //            request.AddHeader("Authorization", authorizationString);

        //            request.RequestFormat = DataFormat.Json;

        //            // Main part of method
        //            getRequestResponse = client.ExecuteGet<RestResponse>(request);

        //            queryOrderResponse = JsonConvert.DeserializeObject<QueryOrderResponse>(getRequestResponse.Content);

        //            //// NB Normally, RestSharp doesn't throw an exception if the
        //            // request fails (for ExecutePostAsync and related commands)
        //            if (getRequestResponse.IsSuccessful == true)
        //            {
        //                strOrderStatus = queryOrderResponse.Status;
        //                paymentResult.responseStatus = getRequestResponse.ResponseStatus.ToString();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            WriteToEventLog("Exception", ex.Message);
        //        }
        //        finally
        //        {
        //            // Do clean up code....
        //            queryOrderResponse = null;
        //            getRequestResponse = null;
        //            paymentResult = null;
        //            client = null;
        //            request = null;

        //        }

        //    }  // if ...

        //    //returns order status
        //    return strOrderStatus;

        //}

        public async Task SendInvoice(PaymentDTO p)
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format(@"<p> <b>Invoice Number:</b>{0}</p>
                                    <table>
                                      <thead>
                                        <tr>
                                          <td colspan=""4"" style=""text-align:center;padding:5px""><b>Dishes</b></td>
                                        </tr>
                                      </thead>
                                      <tbody>", p.InvoiceNumber));

            foreach (var d in p.TokenOrders)
            {
                var label = d.Tokens?[0].SelectedDishes?[0].DishLabel;
                var price = d.TotalAmount;

                sb.AppendLine("<tr>");
                sb.AppendLine(string.Format("<td style=\"padding: 5px;\">{0}</td>", label));
                sb.AppendLine(string.Format("<td style=\"text-align: right;padding:5px;\" >{0:n2}</td>", price));
                sb.AppendLine(string.Format("<td style=\"padding: 5px;\">{0}</td>", d.DeliveryDate));
                sb.AppendLine(string.Format("<td style=\"padding: 5px;\">{0}</td>", d.MealSessionDetailName));
                sb.AppendLine("</tr>");

            }

            foreach (var d in p.MealPlanOrders)
            {
                var label = d.StudentGroupCode;
                var price = d.TotalAmount;

                sb.AppendLine("<tr>");
                sb.AppendLine(string.Format("<td style=\"padding: 5px;\">{0}</td>", label));
                sb.AppendLine(string.Format("<td style=\"text-align: right;padding:5px;\" >{0:n2}</td>", price));
                sb.AppendLine("</tr>");

            }

            sb.AppendLine("<tr>");
            sb.AppendLine(string.Format("<td style=\"padding: 5px;\">{0}</td>", "Price"));
            sb.AppendLine(string.Format("<td style=\"text-align: right;padding:5px;\" >{0}</td>", p.subtotal));
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr>");
            sb.AppendLine(string.Format("<td style=\"padding: 5px;\">{0}</td>", "Discount:"));
            sb.AppendLine(string.Format("<td style=\"text-align: right;padding:5px;\" >{0}</td>", p.discount));
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr>");
            sb.AppendLine(string.Format("<td style=\"padding: 5px;\">{0}</td>", "GST Inclusive:"));
            sb.AppendLine(string.Format("<td style=\"text-align: right;padding:5px;\" >{0}</td>", p.gst));
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr>");
            sb.AppendLine(string.Format("<td style=\"padding: 5px;\">{0}</td>", "Subtotal"));
            sb.AppendLine(string.Format("<td style=\"text-align: right;padding:5px;\" >{0}</td>", p.subdisctotal));
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr>");
            sb.AppendLine(string.Format("<td style=\"padding: 5px;\">{0}</td>", "Transaction Fee:"));
            sb.AppendLine(string.Format("<td style=\"text-align: right;padding:5px;\" >{0}</td>", p.transactionFee));
            sb.AppendLine("</tr>");

            if (p.fixedTransactionFee > 0)
            {
                sb.AppendLine("<tr>");
                sb.AppendLine(string.Format("<td style=\"padding: 5px;\">{0}</td>", "Fixed Transaction Fee:"));
                sb.AppendLine(string.Format("<td style=\"text-align: right;padding:5px;\" >{0}</td>", p.fixedTransactionFee));
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("<tr>");
            sb.AppendLine(string.Format("<td style=\"padding: 5px;\"><b>{0}</b></td>", "Total"));
            sb.AppendLine(string.Format("<td style=\"text-align: right;padding:5px;\" ><b>{0}</b></td>", p.total));
            sb.AppendLine("</tr>");
            sb.AppendLine("</tbody>");
            sb.AppendLine("</table>");
            sb.AppendLine("<div><p>Gourmetz Pte Ltd<br />GST Registration No: </p></div>");


            var isSuccess = await _emailSender.SendEmailAsync(p.name, p.email, "GOe Payment Invoice", sb.ToString());
        }

        public async Task SendWalletInvoice(WalletPaymentDTO p)
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format(@"<p> <b>Invoice Number:</b>{0}</p>
                                    <table>
                                      <thead>
                                        <tr>
                                          <td colspan=""4"" style=""text-align:center;padding:5px""></td>
                                        </tr>
                                      </thead>
                                      <tbody>", p.InvoiceNumber));

          

            sb.AppendLine("<tr>");
            sb.AppendLine(string.Format("<td style=\"padding: 5px;\">{0}</td>", "Amount"));
            sb.AppendLine(string.Format("<td style=\"text-align: right;padding:5px;\" >{0}</td>", p.amount));
            sb.AppendLine("</tr>");

            

            sb.AppendLine("<tr>");
            sb.AppendLine(string.Format("<td style=\"padding: 5px;\">{0}</td>", "Transaction Fee:"));
            sb.AppendLine(string.Format("<td style=\"text-align: right;padding:5px;\" >{0}</td>", p.transactionFee));
            sb.AppendLine("</tr>");

            if (p.fixedTransactionFee > 0)
            {
                sb.AppendLine("<tr>");
                sb.AppendLine(string.Format("<td style=\"padding: 5px;\">{0}</td>", "Fixed Transaction Fee:"));
                sb.AppendLine(string.Format("<td style=\"text-align: right;padding:5px;\" >{0}</td>", p.fixedTransactionFee));
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("<tr>");
            sb.AppendLine(string.Format("<td style=\"padding: 5px;\"><b>{0}</b></td>", "Total"));
            sb.AppendLine(string.Format("<td style=\"text-align: right;padding:5px;\" ><b>{0}</b></td>", p.total));
            sb.AppendLine("</tr>");
            sb.AppendLine("</tbody>");
            sb.AppendLine("</table>");
            sb.AppendLine("<div><p>Gourmetz Pte Ltd<br />GST Registration No: </p></div>");


            var isSuccess = await _emailSender.SendEmailAsync(p.name, p.email, "GOe Payment Invoice", sb.ToString());
        }

        /// <summary>
        /// Only call this from webhook where we know for sure this is stripe.
        /// if you need to call from other place, see 
        /// other method named similarly without the stripe word.
        /// </summary>
        /// <param name="sessionId">stripe session id</param>
        /// <returns>use await for this method.</returns>
        public async Task QueryAndUpdatePaymentStatusStripe(string sessionId, bool successAlready = false)
        {
            if (sessionId != null) // this is called from webhook
            {
                var sos = "";
                if (successAlready)
                {
                    sos = "SUCCESS";
                } else
                {
                    sos = QueryOrderStatusStripe(sessionId, true);
                }
                List<PaymentDTO> payments = await _paymentService.GetCreatedPaymentsAsync();
                var pement = payments.Where(x => x.fomoid == sessionId);
                if (!pement.IsNullOrEmpty())
                {
                    var p = pement.First();
                    if (p != null)
                    {
                        var updated = false;
                        var tokenOrderUpdated = false;
                        var emailSent = false;
                        var strOrderStatus = "";

                        try
                        {
                            strOrderStatus = String.IsNullOrWhiteSpace(p.fomoid) ? "" : sos;
                            if (!String.IsNullOrEmpty(strOrderStatus))
                            {
                                if (p.Status != strOrderStatus)
                                {
                                    p.Status = strOrderStatus;

                                    if (!p.invoiceSent && p.Status == "SUCCESS")
                                    {
                                        await SendInvoice(p);
                                        emailSent = true;
                                        p.invoiceSent = true;
                                    }

                                    await _paymentService.UpdatePaymentAsync(p);
                                }

                                foreach (var t in p.TokenOrders)
                                {
                                    if (t.Status != "paid" && p.Status == "SUCCESS" && t.Status != "cancelled")
                                    {
                                        var dt = await this._service.GetTokenOrderByIdAsync(t.Id);

                                        if (dt != null)
                                        {
                                            dt.Status = "paid";
                                            var r = await this._service.UpdateTokenOrderAsync(dt);

                                            tokenOrderUpdated = true;
                                        }
                                    }
                                }

                                foreach (var t in p.MealPlanOrders)
                                {
                                    if (t.Status != "paid" && p.Status == "SUCCESS" && t.Status != "cancelled")
                                    {
                                        var dt = await this._service.GetMealPlanOrderByIdAsync(t.Id);

                                        if (dt != null)
                                        {
                                            dt.Status = "paid";
                                            var r = await this._service.UpdateMealPlanOrderAsync(dt);

                                            tokenOrderUpdated = true;

                                            if (t.StudentGroupId.HasValue && t.ProfileId.HasValue) await _studentService.CreateOrUpdateStudentGroupDetailAsync(t.StudentGroupId.Value, t.ProfileId.Value, true);
                                        }
                                    }
                                }
                            }
                        } catch (Exception ex)
                        {
                            _logger.LogError(LoggingEvents.APPLICATION_ERROR, ex, $"Error when update payment status. Payment Id {p.Id}, status {p.Status}, status from fomo {strOrderStatus}, payment update {updated}, token order updated {tokenOrderUpdated}, email sent {emailSent}");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Create a method that can be awaited, but does not return any value.
        /// In this case we don't need to anything after querying payment and 
        /// updating database. Hence mark method as Task. rather than very undersiable
        /// void.
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns></returns>
        public async Task QueryAndUpdatePaymentStatus(int? studentId = null)
        {
            try
            {
                List<PaymentDTO> payments = await _paymentService.GetCreatedPaymentsAsync(studentId);

                foreach (var p in payments)
                {
                    var updated = false;
                    var tokenOrderUpdated = false;
                    var emailSent = false;
                    var strOrderStatus = "";

                    try
                    {
                        var qos = await QueryOrderStatusIndependent(p.PaymentTypeId, p.fomoid);
                        strOrderStatus = String.IsNullOrWhiteSpace(p.fomoid) ? "" : qos;

                        if (!String.IsNullOrEmpty(strOrderStatus))
                        {
                            if (p.Status != strOrderStatus)
                            {
                                p.Status = strOrderStatus;

                                if (!p.invoiceSent && p.Status == "SUCCESS")
                                {
                                    await SendInvoice(p);
                                    emailSent = true;
                                    p.invoiceSent = true;
                                }

                                await _paymentService.UpdatePaymentAsync(p);
                            }

                            foreach (var t in p.TokenOrders)
                            {
                                if (t.Status != "paid" && p.Status == "SUCCESS" && t.Status != "cancelled")
                                {
                                    var dt = await this._service.GetTokenOrderByIdAsync(t.Id);

                                    if (dt != null)
                                    {
                                        dt.Status = "paid";
                                        var r = await this._service.UpdateTokenOrderAsync(dt);

                                        tokenOrderUpdated = true;
                                    }
                                }
                            }

                            foreach (var t in p.MealPlanOrders)
                            {
                                if (t.Status != "paid" && p.Status == "SUCCESS" && t.Status != "cancelled")
                                {
                                    var dt = await this._service.GetMealPlanOrderByIdAsync(t.Id);

                                    if (dt != null)
                                    {
                                        dt.Status = "paid";
                                        var r = await this._service.UpdateMealPlanOrderAsync(dt);

                                        tokenOrderUpdated = true;

                                        if (t.StudentGroupId.HasValue && t.ProfileId.HasValue) await _studentService.CreateOrUpdateStudentGroupDetailAsync(t.StudentGroupId.Value, t.ProfileId.Value, true);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(LoggingEvents.APPLICATION_ERROR, ex, $"Error when update payment status. Payment Id {p.Id}, status {p.Status}, status from fomo {strOrderStatus}, payment update {updated}, token order updated {tokenOrderUpdated}, email sent {emailSent}");
                    }
                }

                List<TokenOrderDTO> tOrders = await _service.GetUnupdatedTokenOrdersAsync();

                foreach (var t in tOrders)
                {
                    try
                    {

                        t.Status = "paid";
                        var r = await this._service.UpdateTokenOrderAsync(t);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(LoggingEvents.APPLICATION_ERROR, ex, $"Error when update token order status. Token Order ID {t.Id}, status {t.Status}");
                    }
                }

                List<MealPlanOrderDTO> mealPlanOrders = await _service.GetUnupdatedMealPlanOrdersAsync();

                foreach (var t in mealPlanOrders)
                {
                    try
                    {

                        t.Status = "paid";
                        var r = await this._service.UpdateMealPlanOrderAsync(t);

                        if(t.StudentGroupId.HasValue && t.ProfileId.HasValue) await _studentService.CreateOrUpdateStudentGroupDetailAsync(t.StudentGroupId.Value, t.ProfileId.Value, true);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(LoggingEvents.APPLICATION_ERROR, ex, $"Error when update MealPlan order status. MealPlan Order ID {t.Id}, status {t.Status}");
                    }
                }
            }
            catch (Exception ex)
            {
                WriteToEventLog("FRS Exception", ex.Message);

                _logger.LogError(LoggingEvents.APPLICATION_ERROR, ex, "Error when update payment and token order status.");
            }
            finally
            {
            }

        }

        public async Task QueryAndUpdateWalletPaymentStatusStripe(string sessionId, bool successAlready = false)
        {
            if (sessionId != null) // this is called from webhook
            {
                var sos = "";
                if (successAlready)
                {
                    sos = "SUCCESS";
                } else
                {
                    sos = QueryOrderStatusStripe(sessionId, true);
                }
                try
                {
                    List<WalletPaymentDTO> payments = await _paymentService.GetCreatedWalletPaymentsAsync();
                    var pement = payments.Where(x => x.fomoid == sessionId);
                    foreach (var p in pement)
                    {
                        var updated = false;
                        var tokenOrderUpdated = false;
                        var emailSent = false;
                        var strOrderStatus = "";

                        try
                        {
                            strOrderStatus = String.IsNullOrWhiteSpace(p.fomoid) ? "" : sos;

                            if (!String.IsNullOrEmpty(strOrderStatus))
                            {
                                if (p.Status != strOrderStatus)
                                {
                                    p.Status = strOrderStatus;

                                    if (!p.invoiceSent && p.Status == "SUCCESS")
                                    {
                                        await this._walletService.TopupWalletBalanceByStudentIdAsync(p.StudentId.Value, Decimal.ToDouble(p.amount), p.UserId.Value, WalletType.BASIC);
                                        await SendWalletInvoice(p);
                                        emailSent = true;
                                        p.invoiceSent = true;
                                    }

                                    await _paymentService.UpdateWalletPaymentAsync(p);
                                }
                            }
                        } catch (Exception ex)
                        {
                            _logger.LogError(LoggingEvents.APPLICATION_ERROR, ex, $"Error when update payment status. Payment Id {p.Id}, status {p.Status}, status from fomo {strOrderStatus}, payment update {updated}, token order updated {tokenOrderUpdated}, email sent {emailSent}");
                        }
                    }
                } catch (Exception ex)
                {
                    WriteToEventLog("FRS Exception", ex.Message);

                    _logger.LogError(LoggingEvents.APPLICATION_ERROR, ex, "Error when update payment and token order status.");
                } finally
                {
                }
            }
        }

        /// <summary>
        /// query order status independent of the payment type.
        /// </summary>
        /// <param name="paymentTypeId"></param>
        /// <param name="fomoid"></param>
        /// <returns>order status</returns>
        private async Task<string> QueryOrderStatusIndependent(int? paymentTypeId, string fomoid)
        {
            // we cant use stripe anymore.
            // use another thing, like fomoid stripe pattern.
            // TODO verify that actual stripe prod checkout session id actually starts with cs_
            bool fromStripe = fomoid.StartsWith("cs_") || fomoid.StartsWith("cs_test_");
            string qos;
            if (fromStripe)
            {
                qos = QueryOrderStatusStripe(fomoid);
            } else
            {
                qos = QueryOrderStatus(fomoid);
            }
            return qos;
        }

        /// <summary>
        /// Create a method that can be awaited, but does not return any value.
        /// In this case we don't need to anything after querying payment and 
        /// updating database. Hence mark method as Task. rather than very undersiable
        /// void.
        /// </summary>
        /// <param name="objBodyPayload"></param>
        /// <returns></returns>
        public async Task QueryAndUpdateWalletPaymentStatus(int? studentId = null)
        {
            try
            {
                List<WalletPaymentDTO> payments = await _paymentService.GetCreatedWalletPaymentsAsync(studentId);

                foreach (var p in payments)
                {
                    var updated = false;
                    var tokenOrderUpdated = false;
                    var emailSent = false;
                    var strOrderStatus = "";

                    try
                    {
                        var qos = await QueryOrderStatusIndependent(p.PaymentTypeId, p.fomoid);
                        strOrderStatus = String.IsNullOrWhiteSpace(p.fomoid) ? "" : qos;

                        if (!String.IsNullOrEmpty(strOrderStatus))
                        {
                            if (p.Status != strOrderStatus)
                            {
                                p.Status = strOrderStatus;

                                if (!p.invoiceSent && p.Status == "SUCCESS")
                                {
                                    await this._walletService.TopupWalletBalanceByStudentIdAsync(p.StudentId.Value, Decimal.ToDouble(p.amount), p.UserId.Value,WalletType.BASIC);
                                    await SendWalletInvoice(p);
                                    emailSent = true;
                                    p.invoiceSent = true;
                                }

                                await _paymentService.UpdateWalletPaymentAsync(p);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(LoggingEvents.APPLICATION_ERROR, ex, $"Error when update payment status. Payment Id {p.Id}, status {p.Status}, status from fomo {strOrderStatus}, payment update {updated}, token order updated {tokenOrderUpdated}, email sent {emailSent}");
                    }
                }
            }
            catch (Exception ex)
            {
                WriteToEventLog("FRS Exception", ex.Message);

                _logger.LogError(LoggingEvents.APPLICATION_ERROR, ex, "Error when update payment and token order status.");
            }
            finally
            {
            }

        }

        [HttpGet("notifypayment/{id}")]// need to enable this when using tesing with POSTMAN
        [AllowAnonymous]
        [ProducesResponseType(400)]  // Bad request status response 
        [ProducesResponseType(200)]  // Sucess send after conforming request is valid and authenicated
        public async Task<IActionResult> Notify(string orderID)
        {
            try
            {
                await QueryAndUpdatePaymentStatus();

                return Ok();
            }
            catch (Exception ex) { }

            return BadRequest();
        }

        [HttpGet("notifywalletpayment/{id}")]// need to enable this when using tesing with POSTMAN
        [AllowAnonymous]
        [ProducesResponseType(400)]  // Bad request status response 
        [ProducesResponseType(200)]  // Sucess send after conforming request is valid and authenicated
        public async Task<IActionResult> NotifyWallet(string orderID)
        {
            try
            {
                await QueryAndUpdateWalletPaymentStatus();

                return Ok();
            }
            catch (Exception ex) { }

            return BadRequest();
        }
    }
}
