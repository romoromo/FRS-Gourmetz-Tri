using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
 

namespace SMV.FOMOPay.LoggerHelper
{
    public static class EventLogger
    {

        /// <summary>
        /// Writes simple emtry in event viewer under the appliccation sectipn.
        /// </summary>
        /// <param name="ExceptionMessage"></param>
        /// <param name="EntryType"></param>
        public static void CreateEventEntry(string ExceptionMessage
                                            , EventLogEntryType EntryType   )
        {
            string prefix = "";

            // Create eventlog entry under Windows Logs> Appliation
            using (EventLog eventLog = new EventLog("Application"))
            {
                prefix = "FRS FOMO Payment API:";
                // Use this quick fix to remove 
                // "The description for Event ID 0 from source Application cannot be
                // found..." under the event log>windows Log>Application> General Tab
                eventLog.Source  = ".NET Runtime";

                ExceptionMessage = prefix +"  "+ ExceptionMessage;

               // eventLog.WriteEntry(ExceptionMessage, EntryType);
                eventLog.WriteEntry(ExceptionMessage, EntryType,  1000);

            }
 
        }


        /// <summary>
        /// Reads entries from event log from selected section.
        /// </summary>
        /// <returns></returns>
        public static string ReadEventEntry(string EventEntrySection)
        {
            int countEntries   = 0;
            EventLog eventLog  = new EventLog();
            string EventMesage = "";

            eventLog.Log = EventEntrySection; // "Application";  

            foreach (EventLogEntry entry in eventLog.Entries)
            {
 
                try
                {
                    //Write your custom code here
                    Console.WriteLine("found" + entry);
                    Console.WriteLine("found" + entry.EventID);
                    EventMesage = entry.EventID.ToString();

                    countEntries = countEntries + 1;

                    if (countEntries > 1)
                    {
                        break;
                        //return EventMesage;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex);
                }
 
            } // for each

            return EventMesage;


        }









    }
}
