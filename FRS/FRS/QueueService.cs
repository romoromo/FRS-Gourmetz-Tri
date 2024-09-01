using AutoMapper;
using DAL.Repositories;
using FRS.Controllers;
using FRS.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ServiceModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Xml.Linq;

public class QueueService : IQueueService
{
    //public string Test(string s)
    //{
    //	Console.WriteLine("Test Method Executed!");
    //	return s;
    //}

    //public void XmlMethod(XElement xml)
    //{
    //	Console.WriteLine(xml.ToString());
    //}
    private QueueTableMapController _queueController;

    private readonly IMapper _mapper;
    public QueueService(QueueTableMapController queueController, IMapper mapper)
    {
        _queueController = queueController;
        _mapper = mapper;
    }

    public GetTokenResponse GetToken(GetTokenParam tokenParam)
    {
        var res = new GetTokenResponse();

        var param = new Dictionary<string, string>();
        param.Add("username", tokenParam.Username);
        param.Add("password", tokenParam.Password);

        var result = (ObjectResult)_queueController.GetToken(param).Result;

        if (result.StatusCode == 200)
        {
            try
            {
                res.Token = result.Value.GetType().GetProperty("token").GetValue(result.Value, null).ToString();
                res.Expire = result.Value.GetType().GetProperty("expire").GetValue(result.Value, null).ToString();
            } catch (Exception ex)
            {
                res.ErrorDescription = ex.Message;
            }

            return res;
        }
        else
        {
            var errorCode = "";
            var errorMessage = "";

            try
            {
                errorCode = result.Value.GetType().GetProperty("ErrorCode").GetValue(result.Value, null).ToString();
                errorMessage = result.Value.GetType().GetProperty("ErrorMessage").GetValue(result.Value, null).ToString();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return new GetTokenResponse()
            {
                Status = "ERROR",
                ErrorCode = errorCode,
                ErrorDescription = errorMessage
            };
        }
    }

    public QueueServiceResponse CallQueue(QueueServiceParam queueParam)
    {
        Console.WriteLine(queueParam.ToString());

        var model = _mapper.Map<QueueLogViewModel>(queueParam);

        model.Queueid = queueParam.ClinicId + queueParam.TerminalId;
        model.QueueNo = queueParam.QueueNumber;
        model.Token = queueParam.Token;

        switch (queueParam.CallAction)
        {
            case "1":
                model.CallAction = QueueTableMapRepository.call;
                break;
            case "2":
                model.CallAction = QueueTableMapRepository.missedqueue;
                break;
            case "3":
                model.CallAction = QueueTableMapRepository.clear;
                break;
            case "4":
                model.CallAction = QueueTableMapRepository.silentcall;
                break;
        }

        model.isT = true;

        var result = (ObjectResult)_queueController.CallQueue(model).Result;


        if (result.StatusCode == 200)
        {
            return new QueueServiceResponse()
            {
                MessageId = queueParam.MessageId,
                Status = "SUCCESS"
            };
        }
        else
        {
            var errorCode = "";
            var errorMessage = "";

            try
            {
                errorCode = result.Value.GetType().GetProperty("ErrorCode").GetValue(result.Value, null).ToString();
                errorMessage = result.Value.GetType().GetProperty("ErrorMessage").GetValue(result.Value, null).ToString();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return new QueueServiceResponse()
            {
                MessageId = queueParam.MessageId,
                Status = "ERROR",
                ErrorCode = errorCode,
                ErrorDescription = errorMessage
            };
        }
    }
}
