using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text;
using Microsoft.Identity.Client;

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

    public class SmtpOauth2Config
    {
        public string token_url { get;  set; }
        public string client_id { get;  set; }
        public string client_secret { get; set; }
        public string scope { get; set; }
        public string grant_type { get; set; }
        public string email_url { get; set; }
    }

    public class Oauth2TokenResponse
    {
        
        public string access_token { get; set; }
        public string token_type { get; set; }
        //public string expires_in { get; set; }
        //public string ext_expires_in { get; set; }

        public string error { get; set; }
        public string error_description { get; set; }
        //public List<int> error_codes { get; set; }
        //public string timestamp { get; set; }
        //public string trace_id { get; set; }
        //public string correlation_id { get; set; }
    }

    public class EmailAttachment
    {
        public string FileName { get; set; }
        public byte[] Stream { get; set; }
        public ContentType ContentType { get; set; }
    }
}
