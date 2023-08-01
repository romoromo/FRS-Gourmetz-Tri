using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GenericAPITaskScheduler
{
    /// <summary>
    /// This console application can be used to directly call a PUBLIC api through Task Scheduler. API Url can be set in the scheduled task's arguments.
    /// Default URL to be called is set in the config file setting API_URL
    /// Writes all logs in the Event Viewer
    /// NOTE: Only GET and POST are supported now
    /// <param name="args">args[0] is the API url, args[1] is the REQUEST TYPE, args[2] is the POST body</param>
    /// <remarks>Set the task scheduler arguments separated by space example: "http://localhost/api/SmartRoomScheduler/scheduler/execute" "GET"</remarks>
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        static async Task MainAsync(string[] args)
        {
            //Debugger.Launch(); //TODO: uncomment if you want to debug the app
            //args[0] = http://119.73.206.38/api/SmartRoomScheduler/scheduler/execute or any url (API_URL = default URL)
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Application";
                eventLog.WriteEntry("Scheduler started", EventLogEntryType.Information);
                string apiUrl = args != null && args.Count() > 0 ? args[0] : ConfigurationManager.AppSettings["API_URL"];
                string type = "GET";
                string body = "";
                if (args != null)
                {
                    if(args.Length > 1)
                    {
                        type = args[1].ToUpper();
                    }

                    if (args.Length > 2)
                    {
                        body = args[2];
                    }
                }

                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromHours(24);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (!string.IsNullOrEmpty(apiUrl))
                    {
                        try
                        {
                            HttpResponseMessage response = null;

                            if(type == "POST")
                            {
                                response = await client.PostAsync(apiUrl, new StringContent(body, Encoding.UTF8, "application/json"));
                            }
                            else
                            {
                                response = await client.GetAsync(apiUrl);
                            }
                            
                            //response.EnsureSuccessStatusCode();
                            if (response != null && response.Content != null)
                            {
                                var content = await response.Content.ReadAsStringAsync();
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
