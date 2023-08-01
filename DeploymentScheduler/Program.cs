using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentScheduler
{
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
                eventLog.WriteEntry("Deployment Scheduler started", EventLogEntryType.Information);

                //back up files first
                string backupDirectory = ConfigurationManager.AppSettings["BACKUP_DIRECTORY"];
                string appPath = ConfigurationManager.AppSettings["APPLICATION_DIRECTORY"];
                string publishPath = ConfigurationManager.AppSettings["PUBLISH_DIRECTORY"];

                //check if there are new files in the publish folder, else just skip
                if (System.IO.Directory.Exists(publishPath))
                {
                    if (System.IO.Directory.GetFiles(publishPath).Length > 0)
                    {
                        if (System.IO.Directory.Exists(appPath))
                        {
                            string newBackupDirectory = System.IO.Path.Combine(backupDirectory, DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                            if (!System.IO.Directory.Exists(newBackupDirectory))
                            {
                                System.IO.Directory.CreateDirectory(newBackupDirectory);
                            }

                            string[] files = System.IO.Directory.GetFiles(appPath);

                            // Copy the files and overwrite destination files if they already exist.
                            foreach (string s in files)
                            {
                                // Use static Path methods to extract only the file name from the path.
                                string fileName = System.IO.Path.GetFileName(s);
                                string destFile = System.IO.Path.Combine(newBackupDirectory, fileName);
                                System.IO.File.Copy(s, destFile, true);
                            }

                            //backup done. Start publishing the files
                            if (System.IO.Directory.Exists(publishPath))
                            {
                                if (System.IO.Directory.GetFiles(publishPath).Length > 0)
                                {
                                    Process cmd = new Process();
                                    cmd.StartInfo.FileName = "cmd.exe";
                                    cmd.StartInfo.RedirectStandardInput = true;
                                    cmd.StartInfo.RedirectStandardOutput = true;
                                    cmd.StartInfo.CreateNoWindow = true;
                                    cmd.StartInfo.UseShellExecute = false;
                                    cmd.Start();

                                    string command = string.Format("cd {0}&FRS.deploy.cmd /Y", publishPath);
                                    cmd.StandardInput.WriteLine(command);
                                    cmd.StandardInput.Flush();
                                    cmd.StandardInput.Close();
                                    cmd.WaitForExit();
                                    eventLog.WriteEntry(cmd.StandardOutput.ReadToEnd(), EventLogEntryType.Information);
                                    eventLog.WriteEntry("Publish Completed.", EventLogEntryType.Information);
                                    //remove published files

                                    System.IO.Directory.Delete(publishPath, true);
                                    System.IO.Directory.CreateDirectory(publishPath);

                                }
                            }
                            else
                            {
                                eventLog.WriteEntry(string.Format("PUBLISH_DIRECTORY - {0} does not exist!", publishPath), EventLogEntryType.Error);
                            }
                        }
                        else
                        {
                            eventLog.WriteEntry(string.Format("APPLICATION_DIRECTORY - {0} does not exist!", appPath), EventLogEntryType.Error);
                        }
                    }
                }
            }
        }
    }
}
