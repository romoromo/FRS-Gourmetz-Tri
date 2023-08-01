using System;
using System.Collections.Generic;
using System.Text;
using Ical.Net;
using Ical.Net.DataTypes;
using Ical.Net.CalendarComponents;
using System.Threading.Tasks;
using MimeKit;
using System.Net.Mail;
using BAL.DTO;

namespace BAL.Services.Interfaces
{
    public interface IEmailSender
    {
        Task<(bool success, string errorMsg)> SendEmailAsync(MailboxAddress sender, MailboxAddress[] recepients, string subject, string body, SmtpConfig config = null, bool isHtml = true, List<EmailAttachment> attachments = null);
        Task<(bool success, string errorMsg)> SendEmailAsync(string recepientName, string recepientEmail, string subject, string body, SmtpConfig config = null, bool isHtml = true, List<EmailAttachment> attachments = null, string action = null);
        Task<(bool success, string errorMsg)> SendEmailAsync(string senderName, string senderEmail, string recepientName, string recepientEmail, string subject, string body, SmtpConfig config = null, bool isHtml = true, List<EmailAttachment> attachments = null);
        Calendar CreateCalendarEntry(DateTime? start, DateTime? end, string title, string description, string location, string organizerName, string organizerEmail);
    }
}
