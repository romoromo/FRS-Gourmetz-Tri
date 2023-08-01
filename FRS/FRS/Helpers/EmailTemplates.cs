using System;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace FRS.Helpers
{
    public static class EmailTemplates
    {
        static IHostingEnvironment _hostingEnvironment;
        static string testEmailTemplate;
        static string plainTextTestEmailTemplate;


        public static void Initialize(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public static string GetParticipantEmail(string subject, string description, string notes, string location, string recepientName, string bookedBy, string meetingDate, string urlResponse)
        {
            //if (testEmailTemplate == null)
                testEmailTemplate = ReadPhysicalFile("Helpers/Templates/ParticipantEmail.template");


            string emailMessage = testEmailTemplate
                .Replace("{subject}", subject)
                .Replace("{description}", description)
                .Replace("{location}", location)
                .Replace("{notes}", notes)
                .Replace("{participant}", recepientName)
                .Replace("{bookedBy}", bookedBy)
                .Replace("{meetingDate}", meetingDate)
                .Replace("{urlResponse}", urlResponse)
                .Replace("{from}", bookedBy);

            return emailMessage;
        }

        public static string GetConfirmationEmail(string orderPortalUrl, string email, string callbackUrl, string img)
        {
            //if (testEmailTemplate == null)
                testEmailTemplate = ReadPhysicalFile("Helpers/Templates/ConfirmEmail.template");


            string emailMessage = testEmailTemplate
                .Replace("{email}", email)
                .Replace("{callbackUrl}", callbackUrl)
                .Replace("{img_onboarding}", img)
                .Replace("{order_portal}", orderPortalUrl);

            return emailMessage;
        }

        public static string GetForgotPasswordEmail(string email, string callbackUrl, string img = "")
        {
            //if (testEmailTemplate == null)
            testEmailTemplate = ReadPhysicalFile("Helpers/Templates/ForgotPassword.template");


            string emailMessage = testEmailTemplate
                .Replace("{email}", email)
                .Replace("{callbackUrl}", callbackUrl)
                .Replace("{img_onboarding}", img);

            return emailMessage;
        }

        public static string GetAccountReadyEmail(string email, string url)
        {
            //if (testEmailTemplate == null)
                testEmailTemplate = ReadPhysicalFile("Helpers/Templates/AccountReadyEmail.template");


            string emailMessage = testEmailTemplate
                .Replace("{email}", email)
                .Replace("{url}", url);

            return emailMessage;
        }

        public static string GetTestEmail(string recepientName, DateTime testDate)
        {
            if (testEmailTemplate == null)
                testEmailTemplate = ReadPhysicalFile("Helpers/Templates/TestEmail.template");


            string emailMessage = testEmailTemplate
                .Replace("{user}", recepientName)
                .Replace("{testDate}", testDate.ToString());

            return emailMessage;
        }



        public static string GetPlainTextTestEmail(DateTime date)
        {
            if (plainTextTestEmailTemplate == null)
                plainTextTestEmailTemplate = ReadPhysicalFile("Helpers/Templates/PlainTextTestEmail.template");


            string emailMessage = plainTextTestEmailTemplate
                .Replace("{date}", date.ToString());

            return emailMessage;
        }

        public static string GetNoOrderNextWeek(string template, string name, string dateFrom, string dateTo, string dateCutOff)
        {
            //if (testEmailTemplate == null)
            testEmailTemplate = string.IsNullOrEmpty(template) ? ReadPhysicalFile("Helpers/Templates/ConfirmEmail.template") : template;


            string emailMessage = testEmailTemplate
                .Replace("{student_name}", name)
                .Replace("{date_from}", dateFrom)
                .Replace("{date_to}", dateTo)
                .Replace("{date_cutoff}", dateCutOff);

            return emailMessage;
        }

        public static string GetStudentsWithAbandonedCart1(string template, string url)
        {
            //if (testEmailTemplate == null)
            testEmailTemplate = string.IsNullOrEmpty(template) ? ReadPhysicalFile("Helpers/Templates/AbandonedCart1.template") : template;


            string emailMessage = testEmailTemplate
                .Replace("{url}", url);

            return emailMessage;
        }

        public static string GetStudentsWithAbandonedCart2(string template, string url)
        {
            //if (testEmailTemplate == null)
            testEmailTemplate = string.IsNullOrEmpty(template) ? ReadPhysicalFile("Helpers/Templates/AbandonedCart2.template") : template;


            string emailMessage = testEmailTemplate
                .Replace("{url}", url);

            return emailMessage;
        }

        public static string GetCancellationRequestApproved(string template, string deliveryDate, string sessionName, string orderNumber)
        {
            //if (testEmailTemplate == null)
            testEmailTemplate = string.IsNullOrEmpty(template) ? ReadPhysicalFile("Helpers/Templates/CancellationRequestApproved.template") : template;


            string emailMessage = testEmailTemplate
                .Replace("{delivery_date}", deliveryDate)
                .Replace("{session_name}", sessionName)
                .Replace("{items}", orderNumber);

            return emailMessage;
        }

        public static string GetCancellationRequestRejected(string template, string deliveryDate, string sessionName, string orderNumber)
        {
            //if (testEmailTemplate == null)
            testEmailTemplate = string.IsNullOrEmpty(template) ? ReadPhysicalFile("Helpers/Templates/CancellationRequestRejected.template") : template;


            string emailMessage = testEmailTemplate
                .Replace("{delivery_date}", deliveryDate)
                .Replace("{session_name}", sessionName)
                .Replace("{items}", orderNumber);

            return emailMessage;
        }

        public static string GetStudentsWithOrdersNotCollected(string template, string user, string date)
        {
            //if (testEmailTemplate == null)
            testEmailTemplate = string.IsNullOrEmpty(template) ? ReadPhysicalFile("Helpers/Templates/MissedCollection.template") : template;


            string emailMessage = testEmailTemplate
                .Replace("{user}", user)
                .Replace("{date}", date);

            return emailMessage;
        }

        private static string ReadPhysicalFile(string path)
        {
            if (_hostingEnvironment == null)
                throw new InvalidOperationException($"{nameof(EmailTemplates)} is not initialized");

            IFileInfo fileInfo = _hostingEnvironment.ContentRootFileProvider.GetFileInfo(path);

            if (!fileInfo.Exists)
                throw new FileNotFoundException($"Template file located at \"{path}\" was not found");

            using (var fs = fileInfo.CreateReadStream())
            {
                using (var sr = new StreamReader(fs))
                {
                    return sr.ReadToEnd();
                }
            }
        }
    }
}
