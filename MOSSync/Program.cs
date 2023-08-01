using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MOSSync
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        static async Task MainAsync(string[] args)
        {
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Application";
                eventLog.WriteEntry("MOSSync started", EventLogEntryType.Information);


                string url = string.Empty;
                string fullpath = string.Empty;
                try
                {
                    string apiUrl = ConfigurationManager.AppSettings["ZipApiUrl"];
                    string relativePath = ConfigurationManager.AppSettings["RelativePath"];
                    string fname = ConfigurationManager.AppSettings["Filename"];
                    string syncType = ConfigurationManager.AppSettings["SyncType"];
                    DateTime? from = null;

                    if (!string.IsNullOrEmpty(syncType))
                    {
                        var days = Convert.ToInt32(ConfigurationManager.AppSettings["Days"]);
                        // filter delta by date
                        from = DateTime.Now.AddDays(days * -1);
                        apiUrl = string.Format("{0}?path={1}&fname={2}&from={3}", apiUrl, relativePath, fname, from.Value.ToString("yyyy-MM-dd HH:mm"));
                    }

                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    using (var apiClient = new HttpClient())
                    {
                        apiClient.Timeout = TimeSpan.FromHours(24);
                        apiClient.DefaultRequestHeaders.Accept.Clear();
                        apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        if (!string.IsNullOrEmpty(apiUrl))
                        {
                            try
                            {
                                HttpResponseMessage response = await apiClient.GetAsync(apiUrl);

                                if (response != null && response.Content != null)
                                {
                                    var content = await response.Content.ReadAsStringAsync();
                                    var resp = JsonConvert.DeserializeObject<BaseOperationResponse>(content);

                                    if (resp.IsSuccess)
                                    {
                                        url = ConfigurationManager.AppSettings["DownloadUrl"];
                                        string filename = "products.zip";

                                        if (resp.Data != null && !string.IsNullOrEmpty(Convert.ToString(resp.Data)))
                                        {
                                            filename = Convert.ToString(resp.Data);
                                            var folderName = ConfigurationManager.AppSettings["UnzippedFolder"];
                                            url = string.Format("{0}?folder={1}&filename={2}&latest=true", url, folderName, filename);
                                        }

                                        string destinationPath = ConfigurationManager.AppSettings["MOS_ZipResourceFilePath"];

                                        if (!Directory.Exists(destinationPath))
                                            Directory.CreateDirectory(destinationPath);

                                        fullpath = Path.Combine(destinationPath, filename);

                                        var client = new WebClient();
                                        ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                                        client.DownloadFile(url, fullpath);
                                        eventLog.WriteEntry("MOSSync is successful.", EventLogEntryType.Information);

                                        string unzipDirectory = ConfigurationManager.AppSettings["MOS_UnzipResourceFilePath"];
                                        ExtractZipFileToDirectory(fullpath, unzipDirectory, true);

                                        //after unzipping, copy the files
                                        string destinationDirectory = ConfigurationManager.AppSettings["DestinationFolder"];
                                        TraverseCopyDirectory(unzipDirectory, destinationDirectory);
                                    }
                                    else
                                    {
                                        eventLog.WriteEntry(string.Format("MOSSync Error: {0}", resp.Message), EventLogEntryType.Error);
                                    }
                                }
                                else
                                {
                                    eventLog.WriteEntry("MOSSync Error: Response received but empty or null.", EventLogEntryType.Error);
                                }

                            }
                            catch (Exception ex)
                            {
                                eventLog.WriteEntry(string.Format("MOSSync Error: An error occurred while calling {0}. Message: {1} Error: {2}", apiUrl, ex.Message, ex.StackTrace), EventLogEntryType.Error);
                            }
                        }
                        else
                        {
                            eventLog.WriteEntry("MOSSync Error: Argument missing. Please provide Api Url in Task Scheduler", EventLogEntryType.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    eventLog.WriteEntry(string.Format("MOSSync Error: An error occurred while calling {0}. Message: {1} Error: {2} : Fullpath: {3}", url, ex.Message, ex.StackTrace, fullpath), EventLogEntryType.Error);
                }
            }
        }

        public static void ExtractZipFileToDirectory(string sourceZipFilePath, string destinationDirectoryName, bool overwrite)
        {
            using (var archive = ZipFile.Open(sourceZipFilePath, ZipArchiveMode.Read))
            {
                if (!overwrite)
                {
                    archive.ExtractToDirectory(destinationDirectoryName);
                    return;
                }

                DirectoryInfo di = Directory.CreateDirectory(destinationDirectoryName);
                string destinationDirectoryFullPath = di.FullName;

                foreach (ZipArchiveEntry file in archive.Entries)
                {
                    string fullPath = Path.GetFullPath(Path.Combine(destinationDirectoryName, file.FullName));

                    if (Path.GetFileName(fullPath).Length != 0)
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                        // The boolean parameter determines whether an existing file that has the same name as the destination file should be overwritten
                        file.ExtractToFile(fullPath, true);
                    }

                    //string completeFileName = Path.GetFullPath(Path.Combine(destinationDirectoryFullPath, file.FullName));

                    //if (!completeFileName.StartsWith(destinationDirectoryFullPath, StringComparison.OrdinalIgnoreCase))
                    //{
                    //    throw new IOException("Trying to extract file outside of destination directory. See this link for more info: https://snyk.io/research/zip-slip-vulnerability");
                    //}

                    //if (file.Name == "")
                    //{// Assuming Empty for Directory
                    //    Directory.CreateDirectory(Path.GetDirectoryName(completeFileName));
                    //    continue;
                    //}
                    //file.ExtractToFile(completeFileName, true);
                }
            }
        }

        public static void TraverseCopyDirectory(string currentDirectory, string destinationDirectory)
        {
            // Get all files and folders in the current directory
            string[] files = Directory.GetFiles(currentDirectory);
            string[] subDirectories = Directory.GetDirectories(currentDirectory);

            // Process each file in the current directory
            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                string destinationFilePath = Path.Combine(destinationDirectory, fileName);

                bool overwrite = false, skip = false;
                if (File.Exists(destinationFilePath))
                {
                    DateTime updatedDate = File.GetLastWriteTime(file);
                    DateTime destUpdatedDate = File.GetLastWriteTime(destinationFilePath);
                    if (updatedDate >= destUpdatedDate)
                    {
                        overwrite = true;
                    }
                    else
                    {
                        skip = true;
                    }
                }

                if(!skip)
                    File.Copy(file, destinationFilePath, overwrite);
            }

            // Process each subdirectory in the current directory
            foreach (string subDirectory in subDirectories)
            {
                string subDirectoryName = Path.GetFileName(subDirectory);
                string destinationSubDirectoryPath = Path.Combine(destinationDirectory, subDirectoryName);
                Directory.CreateDirectory(destinationSubDirectoryPath);

                TraverseCopyDirectory(subDirectory, destinationSubDirectoryPath);
            }
        }
    }

    public class BaseOperationResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
