using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Web.Mvc;
using PDFSplitMergeLib;

namespace SECOM_AJIS.Common.Util
{
    /// <summary>
    /// Path management
    /// </summary>
    public class PathUtil
    {
        public enum AppPath
        {
            PrintPDFFoxit,
            pdftk,
        }

        public enum PathName
        {
            PaymentDataFile,
            GeneratedReportPath,
            ReportTempatePath,
            ImageSignaturePath,
            AutoTransferFile,
            TemporaryAttachFilePath,
            AttachFilePath,
            ReportTempatePathOldCompany,
        }
        /// <summary>
        /// Get path
        /// </summary>
        /// <param name="pathName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetPathValue(PathName pathName, string fileName)
        {
            string pathValue = GetPathValue(pathName);

            string fullPath = string.IsNullOrEmpty(fileName) ? pathValue : Path.Combine(@pathValue, @fileName);

            FileInfo fileInfo = new FileInfo(fullPath);
            if (Directory.Exists(fileInfo.DirectoryName) == false)
            {
                Directory.CreateDirectory(fileInfo.DirectoryName);
            }

            return fullPath;
        }
        /// <summary>
        /// Get path
        /// </summary>
        /// <param name="pathName"></param>
        /// <returns></returns>
        public static string GetPathValue(PathName pathName)
        {
            string strRootPath = ConfigurationManager.AppSettings["RootPath"];
            string pathNameVal = ConfigurationManager.AppSettings[pathName.ToString()];

            if (string.IsNullOrEmpty(pathNameVal))
            {
                if (pathName == PathName.ReportTempatePathOldCompany)
                {
                    return GetPathValue(PathName.ReportTempatePath);
                }
                else
                {
                    throw new ConfigurationErrorsException(string.Format("Missing configuration for {0}", pathName.ToString()));
                }
            }
            else
            {
                string pathValue = Path.Combine(@strRootPath, @pathNameVal);

                if (Directory.Exists(pathValue) == false)
                {
                    Directory.CreateDirectory(pathValue);
                }

                return pathValue;
            }
        }
        /// <summary>
        /// Get temporary path
        /// </summary>
        /// <param name="tempFilename"></param>
        /// <returns></returns>
        public static string GetTemporaryPath(string tempFilename)
        {
            string path = Path.Combine(PathUtil.GetTempPath(), @tempFilename);
            return path;
        }
        /// <summary>
        /// Get list of file information
        /// </summary>
        /// <param name="DirPath"></param>
        /// <param name="Filter"></param>
        /// <returns></returns>
        public static FileInfo[] GetFilesFromDirectory(string DirPath, string Filter = "*.*")
        {
            FileInfo[] FileList = null;
            try
            {
                DirectoryInfo Dir = new DirectoryInfo(DirPath);
                FileList = Dir.GetFiles(Filter, SearchOption.TopDirectoryOnly);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return FileList;
        }
        /// <summary>
        /// Get directory
        /// </summary>
        /// <param name="DirPath"></param>
        /// <returns></returns>
        public static DirectoryInfo[] GetDirFromDirectory(string DirPath)
        {
            DirectoryInfo[] dirList = null;
            try
            {
                DirectoryInfo Dir = new DirectoryInfo(DirPath);
                dirList = Dir.GetDirectories();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dirList;
        }

        public static string GetTempPath()
        {
            string temppath = ConfigurationManager.AppSettings["TemporaryPath"];
            if (string.IsNullOrEmpty(temppath) || !Directory.Exists(temppath))
            {
                //temppath = Path.GetTempFileName();
                temppath = Path.GetTempPath();
            }

            return temppath;
        }

        public static string GetTempFileName(string filename = null)
        {
            string temppath = PathUtil.GetTempPath();
            string tempfilename = null;

            do
            {
                if (string.IsNullOrEmpty(filename))
                {
                    tempfilename = string.Format("{0}", Guid.NewGuid().ToString());
                }
                else
                {
                    string name = Path.GetFileNameWithoutExtension(filename);
                    string ext = Path.GetExtension(filename);
                    if (!string.IsNullOrEmpty(name))
                    {
                        tempfilename = name + "_";
                    }
                    tempfilename += Guid.NewGuid().ToString();
                    if (!string.IsNullOrEmpty(ext))
                    {
                        tempfilename += ext;
                    }
                }

                tempfilename = Path.Combine(temppath, tempfilename);
            }
            while (File.Exists(tempfilename));

            using (var fs = System.IO.File.Create(tempfilename))
            {
                fs.Close();
            }

            return tempfilename;
        }

        public static string GetAppPath(AppPath appPath)
        {
            string path = null;
            switch (appPath)
            {
                case AppPath.pdftk:
                    path = ConfigurationManager.AppSettings["pdftk"];

                    if (!string.IsNullOrEmpty(path) && !File.Exists(path))
                    {
                        path = Path.GetFullPath(path);
                    }

                    if (string.IsNullOrEmpty(path) || !File.Exists(path))
                    {
                        path = CommonUtil.WebPath + @"\bin\DLL\pdftk.exe";
                        if (!string.IsNullOrEmpty(path) && !File.Exists(path))
                        {
                            path = Path.GetFullPath(path);
                        }
                    }

                    if (string.IsNullOrEmpty(path) || !File.Exists(path))
                    {
                        path = "pdftk.exe";
                        if (!string.IsNullOrEmpty(path) && !File.Exists(path))
                        {
                            path = Path.GetFullPath(path);
                        }
                    }

                    break;
                case AppPath.PrintPDFFoxit:
                    path = ConfigurationManager.AppSettings["PrintPDFFoxit"];
                    break;
            }

            return path;
        }
    }
}