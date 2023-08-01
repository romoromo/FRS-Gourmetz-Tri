using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace LdapSyncJob
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        

        static async Task MainAsync(string[] args)
        {
            //args[0] = http://119.73.206.38/api/ContactGroup/ldap/sync
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Application";
                eventLog.WriteEntry("LdapSyncJob started", EventLogEntryType.Information);
                string apiUrl = ConfigurationManager.AppSettings["API_URL"]; ;
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromHours(24);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (!string.IsNullOrEmpty(apiUrl))
                    {
                        try
                        {
                            var response = await client.GetAsync(apiUrl);
                            //response.EnsureSuccessStatusCode();
                            if(response != null && response.Content != null)
                            {
                                var content = await response.Content.ReadAsStringAsync();
                                var results = JObject.Parse(content);
                                eventLog.WriteEntry(content, EventLogEntryType.Information);
                            }
                            else
                            {
                                eventLog.WriteEntry("Response received but empty or null.", EventLogEntryType.Information);
                            }
                            
                        }
                        catch (Exception ex)
                        {
                            eventLog.WriteEntry(string.Format("An error occurred while calling {0}. Message: {1} Error: {2}", apiUrl, ex.Message, ex.StackTrace), EventLogEntryType.Error);
                        }
                    }
                    else
                    {
                        eventLog.WriteEntry("Argument missing. Please provide Api Url in Task Scheduler", EventLogEntryType.Error);
                    }
                }
            }
        }
    }
}
