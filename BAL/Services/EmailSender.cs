//using MailKit.Net.Smtp;
using MimeKit;
using System;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using Microsoft.Extensions.Options;
using Ical.Net;
using Ical.Net.DataTypes;
using Ical.Net.CalendarComponents;
using System.Collections.Generic;
using System.IO;
using BAL.Services.Interfaces;
using System.Linq;
using BAL.DTO;
using System.Net.Mail;
using DAL.Core.Logging;
using System.Net.Http;
using System.Net.Http.Json;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
using iTextSharp.text;
using System.Diagnostics;

namespace BAL.Services
{
    public class EmailSender : IEmailSender
    {
        private SmtpConfig _config;
        private SmtpOauth2Config _configOauth;
        private IApplicationSettingService _appSetting;
        private IEmailQueueService _emailQueue;
        private ILogger _logger;

        public EmailSender(IOptions<SmtpConfig> config, IOptions<SmtpOauth2Config> configOauth, IApplicationSettingService appSetting, IEmailQueueService emailQueue)
        {
            _config = config.Value;
            _configOauth = configOauth.Value;
            _appSetting = appSetting;
            _emailQueue = emailQueue;
            _logger = Logger.CreateLogger<EmailSender>();
        }


        /// <summary>
        /// This is changed from auto send to add the email to the email queue table
        /// </summary>
        /// <param name="recepientName"></param>
        /// <param name="recepientEmail"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="config"></param>
        /// <param name="isHtml"></param>
        /// <param name="attachments"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task<(bool success, string errorMsg)> SendEmailOldAsync(
            string recepientName,
            string recepientEmail,
            string subject,
            string body,
            SmtpConfig config = null,
            bool isHtml = true,
            List<EmailAttachment> attachments = null,
            string action = null)
        {
            if (config == null)
                config = _config;

            var email = new EmailQueueDTO
            {
                Action = action,
                FromEmail = _config.EmailAddress,
                FromName = _config.Name,
                Subject = subject,
                ToEmail = recepientEmail,
                ToName = recepientName,
                Body = body
            };

            var response = await _emailQueue.CreateEmailQueueAsync(email);

            //return (response.IsSuccess, response.Message);

            var from = new MailboxAddress(_config.Name, _config.EmailAddress);
            var to = new MailboxAddress(recepientName, recepientEmail);

            return await SendEmailAsync(from, new MailboxAddress[] { to }, subject, body, config, isHtml, attachments);
        }

        public async Task<(bool success, string errorMsg)> SendEmailAsync(
            string recepientName,
            string recepientEmail,
            string subject,
            string body,
            SmtpOauth2Config config = null,
            bool isHtml = true,
            List<EmailAttachment> attachments = null,
            string action = null)
        {
            try
            {
                if (config == null)
                    config = _configOauth;

                using var tokenClient = new HttpClient();
                var tokenBody = new Dictionary<string, string> {
                    { "client_id", config.client_id },
                    { "client_secret", config.client_secret },
                    { "scope", config.scope},
                    { "grant_type", config.grant_type },
                };
                var content = new FormUrlEncodedContent(tokenBody);
                var response = await tokenClient.PostAsync(config.token_url, content);
                //var tokenObj = await response.Content.ReadAsStringAsync();
                var tokenObj = await response.Content.ReadFromJsonAsync<Oauth2TokenResponse>();

                if (tokenObj == null) return (false, "Fetching token is failed.");
                if (!string.IsNullOrWhiteSpace(tokenObj.error) || string.IsNullOrWhiteSpace(tokenObj.access_token)) return (false, "Error No Token - " + await response.Content.ReadAsStringAsync());
        
                using var emailClient = new HttpClient();
                emailClient.DefaultRequestHeaders.Authorization
                         = new AuthenticationHeaderValue("Bearer", tokenObj.access_token);

                var jsonObj = JsonConvert.SerializeObject(new
                {
                    message =  new
                    {
                        subject = subject,
                        body = new
                        {
                            contentType = isHtml ? "HTML" : "Text",
                            content = body
                        },
                        toRecipients = new[] {
                            new {
                                emailAddress = new {
                                    address = recepientEmail
                                }
                            }
                        }
                    }
                });
                var stringContent = new StringContent(jsonObj, Encoding.UTF8, "application/json");
                var resp = await emailClient.PostAsync(config.email_url, stringContent);
                var emailContent = await resp.Content.ReadAsStringAsync();

                if(emailContent.Contains("error")) return (false, emailContent);

                return (true, "");
            }
            catch (Exception ex)
            {
                return (false, ex.ToString());
            }
        }

        public async Task<(bool success, string errorMsg)> SendEmailAsync(
            string senderName,
            string senderEmail,
            string recepientName,
            string recepientEmail,
            string subject,
            string body,
            SmtpOauth2Config config = null,
            bool isHtml = true,
            List<EmailAttachment> attachments = null)
        {
            return await SendEmailAsync(recepientName, recepientEmail, subject, body, config, isHtml, attachments);
        }



        public async Task<(bool success, string errorMsg)> SendEmailOldAsync(
            string senderName,
            string senderEmail,
            string recepientName,
            string recepientEmail,
            string subject,
            string body,
            SmtpConfig config = null,
            bool isHtml = true,
            List<EmailAttachment> attachments = null)
        {
            if (config == null)
                config = _config;

            if (string.IsNullOrEmpty(senderName))
            {
                senderName = config.Name;
            }

            if (string.IsNullOrEmpty(senderEmail))
            {
                senderEmail = config.EmailAddress;
            }

            var from = new MailboxAddress(senderName, senderEmail);
            var to = new MailboxAddress(recepientName, recepientEmail);

            return await SendEmailAsync(from, new MailboxAddress[] { to }, subject, body, config, isHtml, attachments);
        }



        public async Task<(bool success, string errorMsg)> SendEmailAsync(
            MailboxAddress sender,
            MailboxAddress[] recepients,
            string subject,
            string body,
            SmtpConfig config = null,
            bool isHtml = true,
            List<EmailAttachment> attachments = null)
        {
            MailMessage message = new MailMessage();

            //message.From.Add(sender);
            message.From = new MailAddress(config.EmailAddress, sender.Name ?? config.Name);

            var allowedRecepients = new List<MailboxAddress>();
            //check whitelist
            var whitelist = await _appSetting.GetApplicationSettingByKey("EMAIL_WHITELIST");
            if (whitelist != null && !string.IsNullOrEmpty(whitelist.Value))
            {
                var emails = whitelist.Value.Split(';').Select(e => e.Trim()).ToList();
                foreach (var recepient in recepients)
                {
                    if (emails.Any(e => e.ToLower() == recepient.Address.ToLower()))
                    {
                        //allowed
                        allowedRecepients.Add(recepient);
                    }
                }

                if (allowedRecepients.Count == 0)
                {
                    return (true, null);
                }
            }
            else
            {
                allowedRecepients = recepients.ToList();
            }

            foreach (var recipient in allowedRecepients)
            {
                message.To.Add(new MailAddress(recipient.Address));
            }

            //message.To.AddRange(allowedRecepients);
            message.Subject = subject;

            if (isHtml)
            {
                var builder = new BodyBuilder();
                builder.HtmlBody = body;
                if (attachments != null)
                {
                    foreach (var attachment in attachments)
                    {
                        builder.Attachments.Add(attachment.FileName, attachment.Stream);
                    }
                }
                //message.Body = builder.ToMessageBody();
                message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(body, null, System.Net.Mime.MediaTypeNames.Text.Html));
            }
            else
            {
                //message.Body = new TextPart("plain") { Text = body };
                message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(body, null, System.Net.Mime.MediaTypeNames.Text.Plain));
            }

            try
            {

                if (config == null)
                    config = _config;

                using (var client = new SmtpClient())
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential(config.Username, config.Password);
                    client.Port = config.Port;// 587; // 25 587
                    client.Host = config.Host;// "smtp.office365.com";
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.EnableSsl = config.UseSSL;

                    //if (!config.UseSSL)
                    //    client.ServerCertificateValidationCallback = (object sender2, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true;

                    //await client.ConnectAsync(config.Host, config.Port, config.UseSSL).ConfigureAwait(false);
                    //client.AuthenticationMechanisms.Remove("XOAUTH2");

                    //System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(config.Username, config.Password);
                    //client.Credentials = credentials;
                    client.Send(message);

                    //if (!string.IsNullOrWhiteSpace(config.Username))
                    //    await client.AuthenticateAsync(config.Username, config.Password).ConfigureAwait(false);

                    //await client.SendAsync(message).ConfigureAwait(false);
                    //await client.DisconnectAsync(true).ConfigureAwait(false);
                }

                foreach (var recipient in allowedRecepients)
                {
                    _logger.LogInformation(LoggingEvents.SEND_EMAIL, string.Format("Successfully sent the email to: {0}, Message: {1}", recipient?.Address, message?.Subject));
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                foreach (var recipient in allowedRecepients)
                {
                    _logger.LogError(LoggingEvents.SEND_EMAIL, string.Format("Failed to send the email to: {0}, Message: {1}", recipient?.Address, message?.Subject));
                }
                _logger.LogError(LoggingEvents.SEND_EMAIL, ex, "An error occurred while sending email");
                return (false, ex.Message);
            }
        }

        public Calendar CreateCalendarEntry(DateTime? start, DateTime? end, string title, string description, string location, string organizerName, string organizerEmail)
        {
            Calendar iCal = new Calendar();
            iCal.Method = "REQUEST";
            // Create the event, and add it to the iCalendar
            CalendarEvent evt = iCal.Create<CalendarEvent>();
            // Set information about the event
            evt.Start = new CalDateTime(start.Value);
            evt.End = new CalDateTime(end.Value); // This also sets the duration  
            evt.Description = description;
            evt.Location = location;
            evt.Summary = title;

            if (!string.IsNullOrEmpty(organizerEmail))
            {
                evt.Organizer = new Organizer()
                {
                    CommonName = organizerName,
                    Value = new Uri(string.Format("mailto:{0}", organizerEmail))
                };
            }
            // Create a reminder 24h before the event
            //Alarm reminder = new Alarm();
            //reminder.Action = AlarmAction.Display;
            //reminder.Trigger = new Trigger(new TimeSpan(-24, 0, 0));
            //evt.Alarms.Add(reminder);

            return iCal;
        }
    }
}
