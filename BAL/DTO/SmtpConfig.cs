using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text;

namespace BAL.DTO
{
    public class SmtpConfig
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool UseSSL { get; set; }

        public string Name { get; set; }
        public string Username { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public bool UseDefaultCrendential { get; set; }
    }

    public class EmailAttachment
    {
        public string FileName { get; set; }
        public byte[] Stream { get; set; }
        public ContentType ContentType { get; set; }
    }
}
