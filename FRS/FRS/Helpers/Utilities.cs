using AspNet.Security.OpenIdConnect.Primitives;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FRS.Helpers
{
    public static class Utilities
    {
        static ILoggerFactory _loggerFactory;
        private const string REPORT_FOLDER = "temp";
        private const string REPORT_PDF_FOLDER = "pdf";
        private const string REPORT_XLS_FOLDER = "xls";
        static IConfiguration _configuration;
        public static void ConfigureLogger(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _loggerFactory = loggerFactory;
            _configuration = configuration;
        }


        public static ILogger CreateLogger<T>()
        {
            if (_loggerFactory == null)
            {
                throw new InvalidOperationException($"{nameof(ILogger)} is not configured. {nameof(ConfigureLogger)} must be called before use");
                //_loggerFactory = new LoggerFactory().AddConsole().AddDebug();
            }

            return _loggerFactory.CreateLogger<T>();
        }


        public static void QuickLog(string text, string filename)
        {
            string dirPath = Path.GetDirectoryName(filename);

            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            using (StreamWriter writer = File.AppendText(filename))
            {
                writer.WriteLine($"{DateTime.Now} - {text}");
            }
        }

        public static string DefaultProfilePhotoPath()
        {
            return (string)_configuration["AppSettings:DEFAULT_PROFILE_PHOTO_PATH"];
        }

        public static int GetUserId(ClaimsPrincipal user)
        {
            return Convert.ToInt32(user.FindFirst(OpenIdConnectConstants.Claims.Subject)?.Value?.Trim());
        }



        public static string[] GetRoles(ClaimsPrincipal identity)
        {
            return identity.Claims
                .Where(c => c.Type == OpenIdConnectConstants.Claims.Role)
                .Select(c => c.Value)
                .ToArray();
        }

        public static string GetReportPathXls()
        {
            string reportPath = _configuration["AppSettings:REPORT_PATH"];
            var path = Path.Combine(reportPath, REPORT_FOLDER, REPORT_XLS_FOLDER);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

        public static string GetRelativeReportPathPdf(string filename)
        {
            return "/" + REPORT_FOLDER + "/" + REPORT_PDF_FOLDER + "/" + filename;
        }

        public static long GetDirectorySize(this System.IO.DirectoryInfo directoryInfo, bool recursive = true)
        {
            try
            {
                var startDirectorySize = default(long);
                if (directoryInfo == null || !directoryInfo.Exists)
                    return startDirectorySize; //Return 0 while Directory does not exist.

                //Add size of files in the Current Directory to main size.
                foreach (var fileInfo in directoryInfo.GetFiles())
                    System.Threading.Interlocked.Add(ref startDirectorySize, fileInfo.Length);

                if (recursive) //Loop on Sub Direcotries in the Current Directory and Calculate it's files size.
                    System.Threading.Tasks.Parallel.ForEach(directoryInfo.GetDirectories(), (subDirectory) =>
                System.Threading.Interlocked.Add(ref startDirectorySize, GetDirectorySize(subDirectory, recursive)));

                return startDirectorySize;  //Return full Size of this Directory.
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static void TraverseCopyDirectory(string currentDirectory, DateTime filterDate, string destinationDirectory)
        {
            // Get all files and folders in the current directory
            string[] files = Directory.GetFiles(currentDirectory);
            string[] subDirectories = Directory.GetDirectories(currentDirectory);

            // Process each file in the current directory
            foreach (string file in files)
            {
                DateTime createdDate = File.GetCreationTime(file);
                DateTime updatedDate = File.GetLastWriteTime(file);

                if (createdDate >= filterDate || updatedDate >= filterDate)
                {
                    string fileName = Path.GetFileName(file);
                    string destinationFilePath = Path.Combine(destinationDirectory, fileName);
                    File.Copy(file, destinationFilePath, true);
                }
            }

            // Process each subdirectory in the current directory
            foreach (string subDirectory in subDirectories)
            {
                DateTime createdDate = Directory.GetCreationTime(subDirectory);

                DateTime updatedDate = Directory.GetLastWriteTime(subDirectory);

                if (createdDate >= filterDate || updatedDate >= filterDate)// || Directory.GetDirectories(subDirectory).Length > 0)
                {
                    string subDirectoryName = Path.GetFileName(subDirectory);
                    string destinationSubDirectoryPath = Path.Combine(destinationDirectory, subDirectoryName);
                    Directory.CreateDirectory(destinationSubDirectoryPath);

                    TraverseCopyDirectory(subDirectory, filterDate, destinationSubDirectoryPath);
                }
            }
        }
    }
}
