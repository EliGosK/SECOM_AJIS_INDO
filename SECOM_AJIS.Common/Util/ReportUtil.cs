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
    /// Report management
    /// </summary>
    public class ReportUtil
    {
        /// <summary>
        /// Convert stream data to bytes
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] StreamToBytes(Stream input)
        {


            Stream tmpStrem = input;

            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = tmpStrem.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        // Narupon W. Feb 28 ,2012 : comment -- not use ---
        //public static Stream MergeReports(IList<Stream> reportStream, string owner_pwd)
        //{
        //    List<byte[]> filesByte = new List<byte[]>();
        //    foreach (Stream stream in reportStream)
        //    {
        //        filesByte.Add(StreamToBytes(stream));
        //    }
        //    byte[] merge = PdfMerger.MergeFiles(filesByte, owner_pwd);
        //    Stream result = new MemoryStream(merge);
        //    return result;
        //}

        // Narupon W. Feb 28 ,2012 : comment -- not use ---
        //public static Stream MergeReports(List<byte[]> reportBytes, string owner_pwd)
        //{
        //    byte[] merge = PdfMerger.MergeFiles(reportBytes, owner_pwd);
        //    Stream result = new MemoryStream(merge);
        //    return result;
        //}

        /// <summary>
        /// Encrypt PDF
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <param name="outputFilePath"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public static bool EncryptPDF(string inputFilePath, string outputFilePath, string pwd)
        {
            bool result = false;
            //string essemblyPath = CommonUtil.WebPath + @"\bin\DLL\pdftk.exe";
            string essemblyPath = PathUtil.GetAppPath(PathUtil.AppPath.pdftk);

            try
            {
                //Modify by Jutarat A. on 21112013
                //if (File.Exists(inputFilePath) == false || File.Exists(essemblyPath) == false)
                //{
                //    return false;
                //}
                if (File.Exists(inputFilePath) == false)
                {
                    throw new Exception(string.Format("Can not encrypt file because the temporary file ({0}) does not exists.", inputFilePath));
                }
                else if (File.Exists(essemblyPath) == false)
                {
                    throw new Exception(string.Format("Can not encrypt file because the essembly file ({0}) does not exists.", essemblyPath));
                }
                //End Modify

                if (String.IsNullOrEmpty(pwd))
                {
                    pwd = DateTime.Now.ToString("yyyyMMddhhmmss");
                }

                // {0} output {1} owner_pw {2}
                string strCommandParameters = string.Format("{0} output {1} owner_pw {2} allow printing", inputFilePath, outputFilePath, pwd);
                ExecuteProcess(essemblyPath, strCommandParameters);

                //Add by Jutarat A. on 21112013
                if (File.Exists(outputFilePath) == false)
                {
                    FileInfo outputFile = new FileInfo(outputFilePath);
                    throw new Exception(string.Format("Can not copy file ({0}) to server ({1}).", outputFile.Name, outputFile.DirectoryName));
                }
                //End Add

                result = true;

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Merge PDF
        /// </summary>
        /// <param name="filePath1"></param>
        /// <param name="filePath2"></param>
        /// <param name="mergeOutputFilename"></param>
        /// <param name="bEncrypt"></param>
        /// <param name="encryptOutputFileName"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public static bool MergePDF(string filePath1, string filePath2, string mergeOutputFilename, bool bEncrypt = false, string encryptOutputFileName = null, string pwd = null)
        {
            bool result = false;
            //string essemblyPath = CommonUtil.WebPath + @"\bin\DLL\pdftk.exe";
            string essemblyPath = PathUtil.GetAppPath(PathUtil.AppPath.pdftk);

            try
            {
                if (File.Exists(filePath1) == false || File.Exists(filePath2) == false || File.Exists(essemblyPath) == false)
                {
                    return false;
                }

                string strCommandParameters = string.Format("{0} {1} cat output {2}", filePath1, filePath2, mergeOutputFilename);
                ExecuteProcess(essemblyPath, strCommandParameters);

                if (CommonUtil.IsNullOrEmpty(encryptOutputFileName) == true)
                {
                    string str = mergeOutputFilename.Replace(".pdf", "");
                    encryptOutputFileName = string.Format("{0}.Encrypt128.pdf", str);
                }
                if (CommonUtil.IsNullOrEmpty(pwd) == true)
                {
                    pwd = "foo";
                }

                if (bEncrypt)
                {
                    EncryptPDF(mergeOutputFilename, encryptOutputFileName, pwd);
                }

                result = true;
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Merge PDF
        /// </summary>
        /// <param name="sourceFiles"></param>
        /// <param name="mergeOutputFilename"></param>
        /// <param name="bEncrypt"></param>
        /// <param name="encryptOutputFileName"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public static bool MergePDF(string[] sourceFiles, string mergeOutputFilename, bool bEncrypt = false, string encryptOutputFileName = null, string pwd = null, bool isClearTempFile = false) //Modify by Jutarat A. on 14112012 (Add isClearTempFile)
        {
            bool result = false;
            //string essemblyPath = CommonUtil.WebPath + @"\bin\DLL\pdftk.exe";
            string essemblyPath = PathUtil.GetAppPath(PathUtil.AppPath.pdftk);
            int MaxLength = 8000;

            try
            {
                if (sourceFiles.Length == 0)
                {
                    return false;
                }

                for (int i = 0; i < sourceFiles.Length; i++)
                {
                    //if (!sourceFiles[i].Contains("\""))
                    //{
                    //    sourceFiles[i] = string.Format("\"{0}\"", sourceFiles[i]);
                    //}

                    if (File.Exists(sourceFiles[i]) == false)
                    {
                        return false;
                    }
                }

                //if (!mergeOutputFilename.Contains("\""))
                //{
                //    mergeOutputFilename = string.Format("\"{0}\"", mergeOutputFilename);
                //}

                //if (!string.IsNullOrEmpty(encryptOutputFileName))
                //{
                //    if (!encryptOutputFileName.Contains("\""))
                //    {
                //        encryptOutputFileName = string.Format("\"{0}\"", encryptOutputFileName);
                //    }
                //}


                string sourceFile = string.Join(" ", sourceFiles);
                bool isSplit = (sourceFile.Length > MaxLength);



                string strCommandParameters = string.Empty;

                if (sourceFiles.Length >= 2)
                {
                    // Merge file

                    if (isSplit)
                    {
                        List<List<string>> splitList = new List<List<string>>();
                        StringBuilder sb = new StringBuilder();

                        for (int i = 0; i < sourceFiles.Length; i++)
                        {
                            if (sb.Length + sourceFiles[i].Length + 3 >= MaxLength)
                            {

                                List<string> strs = new List<string>();
                                strs.Add(sb.ToString());
                                strs.Add(PathUtil.GetTempFileName(".pdf"));
                                splitList.Add(strs);
                                sb.Clear();
                            }

                            //sb.Append(" \"" + sourceFiles[i] + "\"");
                            sb.Append(" " + sourceFiles[i]);
                        }

                        if (sb.Length > 0)
                        {
                            List<string> strs = new List<string>();
                            strs.Add(sb.ToString());
                            strs.Add(PathUtil.GetTempFileName(".pdf"));
                            splitList.Add(strs);
                        }


                        string mergeSource = string.Empty;
                        foreach (var item in splitList)
                        {
                            strCommandParameters = string.Format("{0}  cat output {1}", item[0], item[1]);
                            ExecuteProcess(essemblyPath, strCommandParameters);
                            mergeSource += " " + item[1];
                        }

                        if (!string.IsNullOrEmpty(mergeSource))
                        {
                            strCommandParameters = string.Format("{0}  cat output {1}", mergeSource, mergeOutputFilename);
                            ExecuteProcess(essemblyPath, strCommandParameters);
                        }
                    }
                    else
                    {
                        strCommandParameters = string.Format("{0}  cat output {1}", sourceFile, mergeOutputFilename);
                        ExecuteProcess(essemblyPath, strCommandParameters);
                    }
                }


                if (CommonUtil.IsNullOrEmpty(encryptOutputFileName) == true)
                {
                    string str = mergeOutputFilename.Replace(".pdf", "");
                    encryptOutputFileName = string.Format("{0}.Encrypt128.pdf", str);
                }

                if (CommonUtil.IsNullOrEmpty(pwd) == true)
                {
                    pwd = "foo";
                }

                if (sourceFiles.Length >= 2)
                {
                    if (bEncrypt)
                    {
                        EncryptPDF(mergeOutputFilename, encryptOutputFileName, pwd);
                    }
                }
                else
                {
                    if (bEncrypt)
                    {
                        EncryptPDF(sourceFiles[0], encryptOutputFileName, pwd);
                    }
                    else
                    {
                        File.Copy(sourceFiles[0], mergeOutputFilename, true);
                    }

                }

                //Comment by Jutarat A. on 21112013 (Move to delete temp file when Login)
                ////Add by Jutarat A. on 14112012
                /*FileInfo fileInfo;
                foreach (string strFile in sourceFiles)
                {
                    fileInfo = new FileInfo(strFile);
                    fileInfo.Delete();

                    fileInfo = new FileInfo(strFile.Replace(".pdf", ""));
                    fileInfo.Delete();
                }

                if (isClearTempFile)
                {
                    fileInfo = new FileInfo(mergeOutputFilename);
                    fileInfo.Delete();

                    fileInfo = new FileInfo(mergeOutputFilename.Replace(".pdf", ""));
                    fileInfo.Delete();
                }*/
                ////End Add
                //End Comment

                result = true;
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Execute process
        /// </summary>
        /// <param name="essemblyPath"></param>
        /// <param name="strCommandParameters"></param>
        /// <returns></returns>
        private static bool ExecuteProcess(string essemblyPath, string strCommandParameters)
        {
            bool result = false;

            try
            {
                //Create process
                System.Diagnostics.Process pProcess = new System.Diagnostics.Process();

                //strCommand is path and file name of command to run
                pProcess.StartInfo.FileName = @essemblyPath; ;

                //strCommandParameters are parameters to pass to program
                pProcess.StartInfo.Arguments = strCommandParameters;

                pProcess.StartInfo.UseShellExecute = false;

                //Set output of program to be written to process output stream
                pProcess.StartInfo.RedirectStandardOutput = true;

                //Optional
                //pProcess.StartInfo.WorkingDirectory = strWorkingDirectory;

                pProcess.StartInfo.CreateNoWindow = true;

                //Start the process
                pProcess.Start();

                //Get program output
                string strOutput = pProcess.StandardOutput.ReadToEnd();

                //Wait for process to finish
                pProcess.WaitForExit();

                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        /// <summary>
        /// Clear temporary file
        /// </summary>
        /// <returns></returns>
        public static bool ClearTemporaryFile()
        {
            bool bResult = true;

            try
            {
                DeleteFile(PathUtil.GetTempPath(), "*.pdf");
                DeleteFile(PathUtil.GetTempPath(), "*.doc");
                DeleteFile(PathUtil.GetTempPath(), "*.docx");
                DeleteFile(PathUtil.GetTempPath(), "*.xls");
                DeleteFile(PathUtil.GetTempPath(), "*.xlsx");
                DeleteFile(PathUtil.GetTempPath(), "*.jpeg");
                DeleteFile(PathUtil.GetTempPath(), "*.jpg");
                DeleteFile(PathUtil.GetTempPath(), "*.bmp");
                DeleteFile(PathUtil.GetTempPath(), "*.png");
                DeleteFile(PathUtil.GetTempPath(), "*.gif");
                DeleteFile(PathUtil.GetTempPath(), "*.tmp"); //Add by Jutarat A. on 21112013

                // AttchedFile/TemporaryAttachFilePath
                DeleteDir(PathUtil.GetPathValue(PathUtil.PathName.TemporaryAttachFilePath));
                DeleteFile(PathUtil.GetPathValue(PathUtil.PathName.TemporaryAttachFilePath), "*.*");

                //DeleteFile(Path.GetTempPath(), "*.ppt");
                //DeleteFile(Path.GetTempPath(), "*.pptx");
                //DeleteFile(Path.GetTempPath(), "*.vsd");
                //DeleteFile(Path.GetTempPath(), "*.mpp");
                //DeleteFile(Path.GetTempPath(), "*.txt");
                //DeleteFile(Path.GetTempPath(), "*.rtf");
                //DeleteFile(Path.GetTempPath(), "*.tiff");
                //DeleteFile(Path.GetTempPath(), "*.zip");
                //DeleteFile(Path.GetTempPath(), "*.lzh");
                //DeleteFile(Path.GetTempPath(), "*.rar");
            }
            catch (Exception ex)
            {
                bResult = false;
            }

            return bResult;
        }
        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="DirPath"></param>
        /// <param name="Filter"></param>
        private static void DeleteFile(string DirPath, string Filter = "*.*")
        {
            try
            {
                var files = PathUtil.GetFilesFromDirectory(DirPath, Filter);

                var files2 = (from p in files where p.CreationTime.Date < DateTime.Now.AddDays(-1).Date select p).ToList<FileInfo>();

                foreach (var f in files2)
                {
                    f.Delete();
                }
            }
            catch { }
        }
        /// <summary>
        /// Delete directory
        /// </summary>
        /// <param name="DirPath"></param>
        private static void DeleteDir(string DirPath)
        {
            try
            {
                var dir = PathUtil.GetDirFromDirectory(DirPath);

                var dir2 = (from p in dir where p.CreationTime.Date < DateTime.Now.AddDays(-1).Date select p).ToList<DirectoryInfo>();

                foreach (var f in dir2)
                {
                    f.Delete(true);
                }
            }
            catch { }
        }
        /// <summary>
        /// Get total page of report
        /// </summary>
        /// <param name="pdfFilePath"></param>
        /// <returns></returns>
        public static int GetTotalPageCount(string pdfFilePath)
        {
            var doc = new PdfReader(pdfFilePath);
            var intTotalPages = doc.NumberOfPages;

            doc.Close();
            return intTotalPages;
        }
        /// <summary>
        /// Split page in PDF
        /// </summary>
        /// <param name="pdfSource"></param>
        /// <param name="pageRange"></param>
        /// <param name="pdfOutput"></param>
        public static bool PDFSplitPage(string pdfSource, int pageFrom, int pageTo, string pdfOutput)
        {
            try
            {
                //PDFSplitMerge pm = new PDFSplitMerge();
                //pm.AddPDFFromFileName(pdfSource);
                //pm.SelectPagesToExtract(0, pageRange);
                //pm.SaveToFile(pdfOutput);


                if (pageFrom == 0 || pageTo == 0)
                {
                    return false;
                }

                if (pageFrom > pageTo)
                {
                    return false;
                }


                //string essemblyPath = CommonUtil.WebPath + @"\bin\DLL\pdftk.exe";
                string essemblyPath = PathUtil.GetAppPath(PathUtil.AppPath.pdftk);

                if (File.Exists(pdfSource) == false || File.Exists(essemblyPath) == false)
                {
                    return false;
                }


                // A={0} cat A{1}-{2} output {3}
                string strCommandParameters = string.Format("A={0} cat A{1}-{2} output {3}", pdfSource, pageFrom, pageTo, pdfOutput);
                ExecuteProcess(essemblyPath, strCommandParameters);


                return true;


            }
            catch (Exception)
            {
                throw;
            }


        }
        /// <summary>
        /// Get report path
        /// </summary>
        /// <param name="reportFile"></param>
        /// <param name="serverPath"></param>
        /// <returns></returns>
        public static string GetReportPath(string reportFile, string serverPath)
        {
            string path = serverPath + "/bin/" + reportFile;
            return path;
        }

        // Currency to Thai word
        private static string s1 = "";
        private static string s2 = "";
        private static string s3 = "";
        private static string[] suffix = { "", "", "สิบ", "ร้อย", "พัน", "หมื่น", "แสน", "ล้าน" };
        private static string[] numSpeak = { "", "หนึ่ง", "สอง", "สาม", "สี่", "ห้า", "หก", "เจ็ด", "แปด", "เก้า" };

        /// <summary>
        /// Convert number to thai word
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string CurrencyToThaiWords(string number)
        {
            double dNumber = 0;

            try
            {
                dNumber = Convert.ToDouble(number);
            }
            catch
            {
            }

            return CurrencyToThaiWords(dNumber);
        }
        /// <summary>
        /// Convert number to thai word
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string CurrencyToThaiWords(double number)
        {
            string result;

            if (number == 0) return ("");

            SplitCurrency(number);
            result = "";
            if (s1.Length > 0)
            {
                result = result + Speak(s1) + "ล้าน";
            }
            if (s2.Length > 0)
            {
                result = result + Speak(s2) + "บาท";
            }
            if (s3.Length > 0)
            {
                result = result + SpeakStang(s3) + "สตางค์";
            }
            else
            {
                result = result + "ถ้วน";
            }
            return (result);
        }

        private static string Speak(string s)
        {
            int L, c;
            string result;

            if (s == "") return ("");

            result = "";
            L = s.Length;
            for (int i = 0; i < L; i++)
            {
                if ((s.Substring(i, 1) == "-"))
                {
                    result = result + "ติดลบ";
                }
                else
                {
                    c = System.Convert.ToInt32(s.Substring(i, 1));
                    if ((i == L - 1) && (c == 1))
                    {
                        if (L == 1)
                        {
                            return ("หนึ่ง");
                        }
                        if ((L > 1) && (s.Substring(L - 1, 1) == "0"))
                        {
                            result = result + "หนึ่ง";
                        }
                        else
                        {
                            result = result + "เอ็ด";
                        }
                    }
                    else if ((i == L - 2) && (c == 2))
                    {
                        result = result + "ยี่สิบ";
                    }
                    else if ((i == L - 2) && (c == 1))
                    {
                        result = result + "สิบ";
                    }
                    else
                    {
                        if (c != 0)
                        {
                            result = result + numSpeak[c] + suffix[L - i];
                        }
                    }
                }
            }
            return (result);
        }
        private static string SpeakStang(string s)
        {
            int L, c;
            string result;

            L = s.Length;

            if (L == 0) return ("");

            if (L == 1)
            {
                s = s + "0";
                L = 2;
            }
            if (L > 2)
            {
                s = s.Substring(0, 2);
                L = 2;
            }
            result = "";
            for (int i = 0; i < 2; i++)
            {
                c = Convert.ToInt32(s.Substring(i, 1));
                if ((i == L - 1) && (c == 1))
                {
                    if (Convert.ToInt32(s.Substring(0, 1)) == 0)
                        result = result + "หนึ่ง";
                    else
                        result = result + "เอ็ด";
                }
                else if ((i == L - 2) && (c == 2))
                {
                    result = result + "ยี่สิบ";
                }
                else if ((i == L - 2) && (c == 1))
                {
                    result = result + "สิบ";
                }
                else
                {
                    if (c != 0)
                    {
                        result = result + numSpeak[c] + suffix[L - i];
                    }
                }
            }

            return (result);
        }
        private static void SplitCurrency(double m)
        {
            string s;
            int L;
            int position;

            s = System.Convert.ToString(m);
            position = s.IndexOf(".");
            if ((position >= 0))
            {
                s1 = s.Substring(0, position);
                s3 = s.Substring(position + 1);
                if (s3 == "00")
                {
                    s3 = "";
                }
            }
            else
            {
                s1 = s;
                s3 = "";
            }
            L = s1.Length;
            if ((L > 6))
            {
                s2 = s1.Substring(L - 6);
                s1 = s1.Substring(0, L - 6);
            }
            else
            {
                s2 = s1;
                s1 = "";
            }

            if ((s1 != "") && (Convert.ToInt32(s1) == 0)) s1 = "";
            if ((s2 != "") && (Convert.ToInt32(s2) == 0)) s2 = "";
        }

        // Currency to English word
        private static String CURRENCY_UNIT = "Baht"; // "Dollar"
        private static String SUB_CURRENCY_UNIT = "Satang"; // "Cents"

        /// <summary>
        /// Convert number to english word
        /// </summary>
        /// <param name="numb"></param>
        /// <returns></returns>
        public static String NumericToEnlishWords(double numb)
        {
            //String num = numb.ToString();
            //return ChangeToWords(num, false).ToUpper();
            return ChangeToWords(string.Format("{0:0.00}", numb), false).ToUpper(); //Modify by Jutarat A. on 19122913
        }
        /// <summary>
        /// Convert number to english word
        /// </summary>
        /// <param name="numb"></param>
        /// <returns></returns>
        public static String NumericToEnglishWords(String numb)
        {
            //return ChangeToWords(numb, false).ToUpper();
            return NumericToEnlishWords(Convert.ToDouble(numb)); //Modify by Jutarat A. on 19122913
        }

        /// <summary>
        /// Convert number to english word
        /// </summary>
        /// <param name="numb"></param>
        /// <returns></returns>
        public static String CurrencyToEnglishWords(double numb)
        {
            //return ChangeToWords(numb.ToString(), true).ToUpper();
            return ChangeToWords(string.Format("{0:0.00}", numb), true); //Modify by Jutarat A. on 19122913
        }

        /// <summary>
        /// Convert number to english word
        /// </summary>
        /// <param name="numb"></param>
        /// <returns></returns>
        public static String CurrencyToEnglishWords(decimal numb, string currency)
        {
            decimal decval = numb % 1;
            decimal intval = numb - decval;
            string integertext = ChangeToWords(string.Format("{0:0}", intval), false).ToUpper();
            string dectext = decval.ToString().TrimEnd('0');
            int exp = (dectext.Contains('.') ? dectext.Split('.')[1].Length : 0);

            string currencytext = "";
            switch (currency)
            {
                case "THB": currencytext = "BAHT"; break;
                case "USD": currencytext = "DOLLAR"; break;
                case "EUR": currencytext = "EURO"; break;
                case "YEN": currencytext = "YEN"; break;
            }

            string result = string.Format("{0} {1}", integertext, currencytext).Trim();
            if (exp > 0)
            {
                if (exp < 2) exp = 2;
                result += string.Format(" AND {0:0}/{1:0}", decval * Convert.ToDecimal(Math.Pow(10, exp)), 100);
            }

            return result;
        }

        /// <summary>
        /// Convert number to english word
        /// </summary>
        /// <param name="numb"></param>
        /// <returns></returns>
        public static String CurrencyToEnglishWords(String numb)
        {
            //return ChangeToWords(numb, true).ToUpper();
            return CurrencyToEnglishWords(Convert.ToDouble(numb)); //Modify by Jutarat A. on 19122913
        }

        private static String ChangeToWords(String numb, bool isCurrency)
        {
            numb = numb.TrimStart('0');

            String val = "", wholeNo = numb, points = "", andStr = "", pointStr = "", currencyStr = "";
            //String endStr = (isCurrency) ? ("Only") : ("");
            string endStr = "";
            try
            {
                int decimalPlace = numb.IndexOf(".");
                if (decimalPlace > 0)
                {
                    wholeNo = numb.Substring(0, decimalPlace);
                    points = numb.Substring(decimalPlace + 1);
                    if (Convert.ToInt32(points) > 0)
                    {
                        andStr = (isCurrency) ? (" and") : (" point");// just to separate whole numbers from points/cents
                        //endStr = (isCurrency) ? (" " + SUB_CURRENCY_UNIT + " " + endStr) : ("");
                        //currencyStr = (isCurrency) ? (" " + CURRENCY_UNIT) : ("");
                        currencyStr = "";

                        //Modify by Jutarat A. on 18122013
                        //pointStr = TranslateSubCurrencyUnit(points);
                        if (isCurrency)
                        {
                            int intPoint = 0;
                            if (points.Length >= 2)
                                intPoint = Convert.ToInt32(points.Substring(0, 2));

                            pointStr = " " + TranslateWholeNumber(intPoint.ToString());
                        }
                        else
                        {
                            pointStr = TranslateSubCurrencyUnit(points);
                        }
                        //End Modify
                    }
                }
                val = String.Format("{0}{1}{2}{3}{4}", TranslateWholeNumber(wholeNo).Trim(), currencyStr, andStr, pointStr, endStr);
            }
            catch { ;}
            return val;
        }
        private static String TranslateWholeNumber(String number)
        {
            string word = "";
            try
            {
                bool beginsZero = false;//tests for 0XX
                bool isDone = false;//test if already translated
                double dblAmt = (Convert.ToDouble(number));
                //if ((dblAmt > 0) && number.StartsWith("0"))
                if (dblAmt > 0)
                {//test for zero or digit zero in a nuemric
                    beginsZero = number.StartsWith("0");

                    int numDigits = number.Length;
                    int pos = 0;//store digit grouping
                    String place = "";//digit grouping name:hundres,thousand,etc...
                    switch (numDigits)
                    {
                        case 1://ones' range
                            word = ones(number);
                            isDone = true;
                            break;
                        case 2://tens' range
                            word = tens(number);
                            isDone = true;
                            break;
                        case 3://hundreds' range
                            pos = (numDigits % 3) + 1;
                            place = " Hundred ";
                            break;
                        case 4://thousands' range
                        case 5:
                        case 6:
                            pos = (numDigits % 4) + 1;
                            place = " Thousand ";
                            break;
                        case 7://millions' range
                        case 8:
                        case 9:
                            pos = (numDigits % 7) + 1;
                            place = " Million ";
                            break;
                        case 10://Billions's range
                            pos = (numDigits % 10) + 1;
                            place = " Billion ";
                            break;
                        //add extra case options for anything above Billion...
                        default:
                            isDone = true;
                            break;
                    }
                    if (!isDone)
                    {//if transalation is not done, continue...(Recursion comes in now!!)


                        //word = TranslateWholeNumber(number.Substring(0, pos)) + place + TranslateWholeNumber(number.Substring(pos));

                        // tt
                        word = TranslateWholeNumber(number.Substring(0, pos).TrimStart('0')); //Fixed bug double 'AND'
                        if (number.Substring(0, pos) != "0")
                        {
                            word += place;
                        }
                        word += TranslateWholeNumber(number.Substring(pos));

                        //check for trailing zeros
                        if (beginsZero) word = " and " + word.Trim();
                    }
                    //ignore digit grouping names
                    if (word.Trim().Equals(place.Trim())) word = "";
                }

            }
            catch { ;}
            return word.Trim();
        }
        private static String tens(String digit)
        {
            int digt = Convert.ToInt32(digit);
            String name = null;
            switch (digt)
            {
                case 10:
                    name = "Ten";
                    break;
                case 11:
                    name = "Eleven";
                    break;
                case 12:
                    name = "Twelve";
                    break;
                case 13:
                    name = "Thirteen";
                    break;
                case 14:
                    name = "Fourteen";
                    break;
                case 15:
                    name = "Fifteen";
                    break;
                case 16:
                    name = "Sixteen";
                    break;
                case 17:
                    name = "Seventeen";
                    break;
                case 18:
                    name = "Eighteen";
                    break;
                case 19:
                    name = "Nineteen";
                    break;
                case 20:
                    name = "Twenty";
                    break;
                case 30:
                    name = "Thirty";
                    break;
                case 40:
                    //2013-12-26
                    //Edit By Supaset P.
                    //name = "Fourty";
                    name = "Forty";
                    break;
                case 50:
                    name = "Fifty";
                    break;
                case 60:
                    name = "Sixty";
                    break;
                case 70:
                    name = "Seventy";
                    break;
                case 80:
                    name = "Eighty";
                    break;
                case 90:
                    name = "Ninety";
                    break;
                default:
                    if (digt > 0)
                    {
                        name = tens(digit.Substring(0, 1) + "0") + " " + ones(digit.Substring(1));
                    }
                    break;
            }
            return name;
        }
        private static String ones(String digit)
        {
            int digt = Convert.ToInt32(digit);
            String name = "";
            switch (digt)
            {
                case 1:
                    name = "One";
                    break;
                case 2:
                    name = "Two";
                    break;
                case 3:
                    name = "Three";
                    break;
                case 4:
                    name = "Four";
                    break;
                case 5:
                    name = "Five";
                    break;
                case 6:
                    name = "Six";
                    break;
                case 7:
                    name = "Seven";
                    break;
                case 8:
                    name = "Eight";
                    break;
                case 9:
                    name = "Nine";
                    break;
            }
            return name;
        }
        private static String TranslateSubCurrencyUnit(String cents)
        {
            String cts = "", digit = "", engOne = "";
            for (int i = 0; i < cents.Length; i++)
            {
                digit = cents[i].ToString();
                if (digit.Equals("0"))
                {
                    engOne = "Zero";
                }
                else
                {
                    engOne = ones(digit);
                }
                cts += " " + engOne;
            }
            return cts;
        }

        public static string NumberToEndlishWords(int number)
        {
            if (number <= 0)
                return "";

            if (number < 0)
                return "minus " + NumberToEndlishWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToEndlishWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToEndlishWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToEndlishWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        }

    }

    // Narupon W. Feb 28 ,2012 : comment -- not use ---
    //public static class PdfMerger
    //{
    //    /// <summary>
    //    /// Merge pdf files.
    //    /// </summary>
    //    /// <param name="sourceFiles">PDF files being merged.</param>
    //    /// <returns></returns>
    //    public static byte[] MergeFiles(List<byte[]> sourceFiles, string owner_pwd)
    //    {
    //        Document document = new Document();
    //        MemoryStream output = new MemoryStream();
    //        try
    //        {
    //            try
    //            {
    //                // Initialize pdf writer
    //                PdfWriter writer = PdfWriter.GetInstance(document, output);
    //                writer.PageEvent = new PdfPageEvents();

    //                // Open document to write
    //                document.Open();
    //                PdfContentByte content = writer.DirectContent;

    //                byte[] byteArray = Encoding.ASCII.GetBytes(owner_pwd);

    //                // Iterate through all pdf documents
    //                for (int fileCounter = 0; fileCounter < sourceFiles.Count; fileCounter++)
    //                {
    //                    // Create pdf reader
    //                    PdfReader reader = new PdfReader(sourceFiles[fileCounter], byteArray);
    //                    int numberOfPages = reader.NumberOfPages;

    //                    // Iterate through all pages
    //                    for (int currentPageIndex = 1; currentPageIndex <= numberOfPages; currentPageIndex++)
    //                    {
    //                        // Determine page size for the current page
    //                        document.SetPageSize(reader.GetPageSizeWithRotation(currentPageIndex));
    //                        // Create page
    //                        document.NewPage();
    //                        PdfImportedPage importedPage = writer.GetImportedPage(reader, currentPageIndex);
    //                        // Determine page orientation
    //                        int pageOrientation = reader.GetPageRotation(currentPageIndex);
    //                        if ((pageOrientation == 90) || (pageOrientation == 270))
    //                        {
    //                            content.AddTemplate(importedPage, 0, -1f, 1f, 0, 0, reader.GetPageSizeWithRotation(currentPageIndex).Height);
    //                        }
    //                        else
    //                        {
    //                            content.AddTemplate(importedPage, 1f, 0, 0, 1f, 0, 0);
    //                        }
    //                    }
    //                }
    //            }
    //            catch (Exception exception)
    //            {
    //                throw new Exception("There has an unexpected exception occured during the pdf merging process.", exception);
    //            }
    //        }
    //        finally
    //        {
    //            document.Close();
    //        }
    //        return output.GetBuffer();
    //    }
    //}

    /// <summary>
    /// Implements custom page events.
    /// </summary>
    internal class PdfPageEvents : IPdfPageEvent
    {
        #region members

        private BaseFont _baseFont = null;
        private PdfContentByte _content;

        #endregion

        #region IPdfPageEvent Members

        public void OnOpenDocument(PdfWriter writer, Document document)
        {
            _baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            _content = writer.DirectContent;
        }

        public void OnStartPage(PdfWriter writer, Document document)
        {
        }

        public void OnEndPage(PdfWriter writer, Document document)
        {
            // Write header text
            string headerText = "";
            _content.BeginText();
            _content.SetFontAndSize(_baseFont, 8);
            _content.SetTextMatrix(GetCenterTextPosition(headerText, writer), writer.PageSize.Height - 10);
            _content.ShowText(headerText);
            _content.EndText();

            // Write footer text (page numbers)
            string text = "";//"Page " + writer.PageNumber;
            _content.BeginText();
            _content.SetFontAndSize(_baseFont, 8);
            _content.SetTextMatrix(GetCenterTextPosition(text, writer), 10);
            _content.ShowText(text);
            _content.EndText();
        }

        public void OnCloseDocument(PdfWriter writer, Document document)
        {
        }

        public void OnParagraph(PdfWriter writer, Document document, float paragraphPosition)
        {
        }

        public void OnParagraphEnd(PdfWriter writer, Document document, float paragraphPosition)
        {
        }

        public void OnChapter(PdfWriter writer, Document document, float paragraphPosition, Paragraph title)
        {
        }

        public void OnChapterEnd(PdfWriter writer, Document document, float paragraphPosition)
        {
        }

        public void OnSection(PdfWriter writer, Document document, float paragraphPosition, int depth, Paragraph title)
        {
        }

        public void OnSectionEnd(PdfWriter writer, Document document, float paragraphPosition)
        {
        }

        public void OnGenericTag(PdfWriter writer, Document document, Rectangle rect, string text)
        {
        }

        #endregion

        private float GetCenterTextPosition(string text, PdfWriter writer)
        {
            return writer.PageSize.Width / 2 - _baseFont.GetWidthPoint(text, 8) / 2;
        }
    }
}
