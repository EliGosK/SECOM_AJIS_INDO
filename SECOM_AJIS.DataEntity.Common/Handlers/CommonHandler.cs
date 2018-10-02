using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;


using System.Net;
using System.Net.Mail;
using System.Net.Mime;


using System.Security.Cryptography;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Models;
using CSI.WindsorHelper;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.DataEntity.Master;
using System.Reflection;

using System.Configuration;
using System.IO;
using System.Data.Objects;
using System.Xml;
using System.Web;

using System.Diagnostics;
using System.Threading;
using System.Transactions;
using SECOM_AJIS.DataEntity.ExchangeRate.Handlers;
using SECOM_AJIS.DataEntity.ExchangeRate.ConstantValue;

namespace SECOM_AJIS.DataEntity.Common
{
    public class CommonHandler : BizCMDataEntities, ICommonHandler
    {


        #region Get Methods

        /// <summary>
        /// Process - get data of tbs_ProcessType
        /// </summary>
        /// <param name="pchrServiceTypeCode"></param>
        /// <param name="pchrProductTypeCode"></param>
        /// <returns></returns>
        public override List<tbs_ProductType> GetTbs_ProductType(string pchrServiceTypeCode, string pchrProductTypeCode)
        {
            try
            {
                List<tbs_ProductType> lst = base.GetTbs_ProductType(pchrServiceTypeCode, pchrProductTypeCode);
                if (lst == null)
                    lst = new List<tbs_ProductType>();
                lst = CommonUtil.ConvertObjectbyLanguage<tbs_ProductType, tbs_ProductType>(lst,
                            "ProductTypeName",
                            "ProvideServiceName");

                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Process - get system status (get configuration value by key)
        /// </summary>
        /// <param name="strConfigName"></param>
        /// <returns></returns>
        public string GetSystemStatusValue(string strConfigName)
        {
            List<doSystemConfig> lst = base.GetSystemConfig(strConfigName);
            if (lst.Count == 0)
            {
                return string.Empty;
            }
            else
            {
                return lst[0].ConfigValue;
            }
        }

        /// <summary>
        /// Prcess - get operation type data list
        /// </summary>
        /// <returns></returns>
        public List<doOperationType> GetOperationTypeList()
        {
            try
            {
                List<doOperationType> lst = this.GetOperationTypeList(SECOM_AJIS.Common.Util.ConstantValue.MiscType.C_OPERATION_TYPE);
                return CommonUtil.ConvertObjectbyLanguage<doOperationType, doOperationType>(lst, "OperationTypeName");
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Prcess - get miscellaneous type code 
        /// </summary>
        /// <param name="misc"></param>
        /// <returns></returns>
        public List<doMiscTypeCode> GetMiscTypeCodeList(List<doMiscTypeCode> misc)
        {
            try
            {
                string xml = CommonUtil.ConvertToXml_Store<doMiscTypeCode>(misc, "FieldName", "ValueCode");
                List<doMiscTypeCode> lst = this.GetMiscTypeCodeList(xml);

                if (lst == null)
                    lst = new List<doMiscTypeCode>();
                lst = CommonUtil.ConvertObjectbyLanguage<doMiscTypeCode, doMiscTypeCode>(lst, "ValueDisplay");

                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Prcess - miscellaneous type mapping
        /// </summary>
        /// <param name="miscLst"></param>
        public void MiscTypeMappingList(MiscTypeMappingList miscLst)
        {
            try
            {
                if (miscLst == null)
                    return;

                List<doMiscTypeCode> lst = this.GetMiscTypeCodeList(miscLst.GetMiscTypeList());
                if (lst.Count > 0)
                    miscLst.SetMiscTypeValue(lst);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Process - get miscellaneous type display value
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="strValueCode"></param>
        /// <returns></returns>
        public string GetMiscDisplayValue(string fieldName, string strValueCode)
        {
            string result = string.Empty;
            List<string> listFieldName = new List<string>();
            listFieldName.Add(fieldName);
            List<doMiscTypeCode> listMisc = GetMiscTypeCodeListByFieldName(listFieldName); // This result has language mapping already

            List<doMiscTypeCode> list = (from p in listMisc where p.ValueCode == strValueCode select p).ToList<doMiscTypeCode>();
            if (list.Count > 0)
            {
                result = list[0].ValueDisplay;
            }

            return result;
        }

        /// <summary>
        /// Process - get miscellaneous type display value
        /// </summary>
        /// <param name="listMisc"></param>
        /// <param name="fieldName"></param>
        /// <param name="strValueCode"></param>
        /// <returns></returns>
        public string GetMiscDisplayValue(List<doMiscTypeCode> listMisc, string fieldName, string strValueCode)
        {
            if (strValueCode == null)
                strValueCode = string.Empty;

            List<doMiscTypeCode> miscList =
                (from p in listMisc
                 where p.FieldName == fieldName && p.ValueCode.ToString() == strValueCode
                 select p).ToList<doMiscTypeCode>();

            string displayName = "";

            if (miscList.Count > 0)
            {
                displayName = miscList[0].ValueDisplay;
            }

            return displayName;
        }
        /// <summary>
        /// Process - get miscellaneous type code by field name
        /// </summary>
        /// <param name="fieldNames"></param>
        /// <returns></returns>
        public List<doMiscTypeCode> GetMiscTypeCodeListByFieldName(List<string> fieldNames)
        {
            List<doMiscTypeCode> miscList = new List<doMiscTypeCode>();
            if (fieldNames != null)
            {
                foreach (string field in fieldNames)
                {
                    miscList.Add(new doMiscTypeCode()
                    {
                        FieldName = field,
                        ValueCode = "%"
                    });
                }
            }

            return GetMiscTypeCodeList(miscList);
        }

        /// <summary>
        /// Process - Check status of system is suspend 
        /// </summary>
        /// <returns></returns>
        public bool IsSystemSuspending()
        {
            try
            {
                List<string> lst = this.IsSystemSuspending(SECOM_AJIS.Common.Util.ConstantValue.ConfigName.C_CONFIG_SUSPEND_FLAG);
                if (lst != null)
                {
                    if (lst.Count > 0)
                    {
                        if (lst[0] == "1")
                            return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Process - get employee data by condition
        /// </summary>
        /// <param name="lsEmployee"></param>
        /// <param name="empNo"></param>
        /// <returns></returns>
        public string GetEmplyeeDisplayCodeName(List<doEmpCodeName> lsEmployee, string empNo)
        {
            if (empNo == null)
                empNo = string.Empty;

            List<doEmpCodeName> employeeList =
                (from p in lsEmployee
                 where p.EmpNo.Trim() == empNo.Trim()
                 select p).ToList<doEmpCodeName>();

            string displayName = "";

            if (employeeList.Count > 0)
            {
                displayName = employeeList[0].EmpCodeName;
            }

            return displayName;
        }

        /// <summary>
        ///  Process - get all employee data
        /// </summary>
        /// <returns></returns>
        public List<doEmpCodeName> GetEmployeeCodeNameList()
        {
            try
            {

                List<doEmpCodeName> lst = base.GetEmpCodeName();

                if (lst == null)
                    lst = new List<doEmpCodeName>();
                lst = CommonUtil.ConvertObjectbyLanguage<doEmpCodeName, doEmpCodeName>(lst, "EmpCodeName");
                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Process - get running no.
        /// </summary>
        /// <param name="nameCode"></param>
        /// <returns></returns>
        public override List<doRunningNo> GetNextRunningCode(string nameCode, bool? isLockRow = false)
        {
            List<doRunningNo> list = base.GetNextRunningCode(nameCode, isLockRow);
            if (list.Count > 0)
            {
                if (list[0].RunningNo == "-1") // -1 = Running no. more than Max
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0104, nameCode);
                }
                else if (list[0].RunningNo == "-2")  // -2 = Running no. not register in tbs_RunningNo
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0136);
                }
            }

            return list;
        }

        #endregion
        #region Methods

        /// <summary>
        /// Validate email format
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        private  bool ValidateEmailAddress(string txt)
        {
            if (txt.Length == 0)
                throw new Exception("Email address is a required field");
            else
            {
                if (txt.IndexOf(".") == -1 || txt.IndexOf("@") == -1)
                {
                    throw new Exception("E-mail address must be valid e-mail");
                }
            }

            return true;
        }

        /// <summary>
        /// Process  - send e-mail
        /// </summary>
        /// <param name="doEmailProc"></param>
        /// <returns></returns>
        public DeliveryNotificationOptions SendMail(doEmailProcess doEmailProc)
        {
            try
            {

                int UseGmailSmtp = int.Parse(ConfigurationManager.AppSettings["UseGmailSmtp"]);
                string SmtpHost = ConfigurationManager.AppSettings["SmtpHost"];
                int SmtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
                // 2016.11 modify tanaka start
                //string AuthenticateUserName = ConfigurationManager.AppSettings["AuthenticateUserName"];
                //string AuthenticatePassword = ConfigurationManager.AppSettings["AuthenticatePassword"];
                // 2016.11 modify tanaka end
                string SECOM_SENDER = ConfigurationManager.AppSettings["SECOM_SENDER"];
                string SECOM_SENDER_ALIAS = ConfigurationManager.AppSettings["SECOM_SENDER_ALIAS"];

                ApplicationErrorException.CheckMandatoryField(doEmailProc);

                MailMessage message = new MailMessage();

                //Add by Jutarat A. on 20121130
                MailAddress MailFrom;
                if (String.IsNullOrEmpty(doEmailProc.MailFrom))
                    MailFrom = new MailAddress(SECOM_SENDER, SECOM_SENDER_ALIAS);
                else
                    MailFrom = new MailAddress(doEmailProc.MailFrom, SECOM_SENDER_ALIAS);
                //End Add

                message.IsBodyHtml = doEmailProc.IsBodyHtml ?? false; //Add by Jutarat A. on 15072013

                if (UseGmailSmtp == 1)
                {
                    //------- Send via gmail -------//

                    string[] tmpMailTo;
                    tmpMailTo = doEmailProc.MailTo.Split(';', ',');

                    foreach (string MailTo in tmpMailTo)
                    {
                        if ((!String.IsNullOrEmpty(MailTo)) && (ValidateEmailAddress(MailTo.Trim()) == true))
                        {
                            message.To.Add(MailTo.Trim());
                        }
                    }

                    message.From = MailFrom; //new MailAddress(SECOM_SENDER, SECOM_SENDER_ALIAS); //Modify by Jutarat A. on 20121130
                    message.Subject = doEmailProc.Subject;
                    message.Body = doEmailProc.Message;

                    SmtpClient client = new SmtpClient(SmtpHost, SmtpPort);
                    // 2016.11 modify tanaka start
                    //{
                    //    Credentials = new NetworkCredential(AuthenticateUserName, AuthenticatePassword),
                    //    EnableSsl = true
                    //};
                    // 2016.11 modify tanaka end

                    client.Send(message);
                }
                else
                {
                    // ------- Send via SECOM SMTP -------//

                    string[] tmpMailTo;
                    tmpMailTo = doEmailProc.MailTo.Split(';', ',');

                    tmpMailTo = ConfigurationManager.AppSettings["DEVELOPERS_MAIL_ADDRESS"].Split(';');
                    foreach (string MailTo in tmpMailTo)
                    {
                        if ((!String.IsNullOrEmpty(MailTo)) && (ValidateEmailAddress(MailTo.Trim()) == true))
                        {
                            message.To.Add(MailTo.Trim());
                        }
                    }
                    message.From = MailFrom; //new MailAddress(SECOM_SENDER, SECOM_SENDER_ALIAS); //Modify by Jutarat A. on 20121130
                    message.Subject = doEmailProc.Subject;
                    message.Body = doEmailProc.Message;

                    Attachment data;
                    if (doEmailProc.AttachFileList != null)
                    {
                        foreach (var item in doEmailProc.AttachFileList)
                        {
                            if (System.IO.File.Exists(item as string))
                            {
                                data = new Attachment(item as string, MediaTypeNames.Application.Octet);

                                // Add the file attachment to this e-mail message.
                                message.Attachments.Add(data);
                            }
                        }
                    }

                    //Send the message.
                    SmtpClient client = new SmtpClient(SmtpHost, SmtpPort);
                    // 2016.11 modify tanaka start
                    //if (!string.IsNullOrEmpty(AuthenticateUserName))
                    //{
                    //    client.UseDefaultCredentials = false;
                    //    client.Credentials = new System.Net.NetworkCredential(AuthenticateUserName, AuthenticatePassword);
                    //}
                    //else
                    //{
                    //    // Add credentials if the SMTP server requires them.
                    //    client.Credentials = CredentialCache.DefaultNetworkCredentials;
                    //}
                    // 2016.11 modify tanaka end

                    // Send
                    client.Send(message);
                }

                return message.DeliveryNotificationOptions;
            }
            catch (Exception ex)
            {
                //BizCMDataEntities entity = new BizCMDataEntities();
                //entity.WriteErrorLog("SendMail", ex.Message, ex.StackTrace, new DateTime(), "Send Mail");
                throw ex;
            }
        }
        /// <summary>
        /// Process - update system status
        /// </summary>
        /// <param name="bSuspendFlag"></param>
        /// <param name="bManualFlag"></param>
        /// <param name="UpdateBy"></param>
        /// <returns></returns>
        public Boolean UpdateSystemStatus(bool bSuspendFlag, bool bManualFlag, string UpdateBy)
        {
            try
            {
                string SQL_JOB_NAME = ConfigurationManager.AppSettings["SuspendResumeSystemJobName"];
                List<int?> lst = base.UpdateSystemStatus(bSuspendFlag, bManualFlag, UpdateBy, SQL_JOB_NAME);
                return lst[0] > 0 ? true : false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// Process - get system satus data list
        /// </summary>
        /// <returns></returns>
        public List<doSystemStatus> GetSystemStatus()
        {
            return base.CM_GetSystemStatus();
        }

        /// <summary>
        /// Generate check digit by runninn no.
        /// </summary>
        /// <param name="strRunningNo"></param>
        /// <returns></returns>
        public string GenerateCheckDigit(string strRunningNo)
        {
            // Akat K. 2011-10-27 : add mandatory check
            if (strRunningNo == null)
            {
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, "strRunningNo");
            }

            MD5 md = MD5.Create();
            byte[] inputByte = System.Text.Encoding.ASCII.GetBytes(strRunningNo);
            byte[] outputByte = md.ComputeHash(inputByte);
            return (outputByte[0] % 10).ToString();
        }

        /// <summary>
        /// Process - update system configuration value
        /// </summary>
        /// <param name="ConfigName"></param>
        /// <param name="ConfigValue"></param>
        /// <returns></returns>
        public bool UpdateSystemConfig(string ConfigName, string ConfigValue)
        {
            try
            {
                string SQL_JOB_NAME = ConfigurationManager.AppSettings["SuspendResumeSystemJobName"];
                List<int?> lst = base.UpdateSystemConfig(ConfigName, ConfigValue, CommonUtil.dsTransData.dtUserData.EmpNo, SQL_JOB_NAME);
                return lst[0] > 0 ? true : false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }


        #endregion
        
        // --------------------------------------------------------------------
        // Attach File Process (After Revise Version.)
        // Modified By: Narupon W., Natthavat S.
        // Revised Date: 20/02/2012
        // --------------------------------------------------------------------

        // General Method

        /// <summary>
        /// Validate file befefore uplaod file to system
        /// </summary>
        /// <param name="fullDocumentName"></param>
        /// <param name="fileSize"></param>
        /// <param name="fileType"></param>
        /// <param name="relatedID"></param>
        /// <param name="sessionID"></param>
        /// <returns></returns>
        public bool CanAttachFile(string fullDocumentName, int fileSize, string fileType, string relatedID, string sessionID = null)
        {
            bool res = false;

            List<dtAttachFileForGridView> currentFileList = GetAttachFileForGridView(relatedID);

            // Check File Name
            var invalidFile = from a in fullDocumentName where Path.GetInvalidFileNameChars().Contains(a) select a;
            if (invalidFile.Count() > 0)
            {
                // Invalid file name
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0125, null);
                //throw new Exception(MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0125).Message);
            }

            //var duplicateFile = from a in currentFileList where a.FileName.ToUpper() == fileAttach.FileName.ToUpper() select a;
            //if (duplicateFile.Count() > 0)
            //{
            //    // Duplicate file name

            //}

            var totalFileSize = fileSize + GetAccumulateFileSize(relatedID, sessionID).Sum(x => x.AccumulateFileSize);
            if (totalFileSize > AttachDocumentCondition.C_ATTACH_DOCUMENT_MAXIMUM_FILE_SIZE)
            {
                // File size over limit.
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0077, null);
                //throw new Exception(MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0077).Message);
            }

            var totalFileCount = currentFileList.Count + 1;
            if (totalFileCount > AttachDocumentCondition.C_ATTACH_DOCUMENT_MAXIMUM_NUMBER)
            {
                // File amount over limit.
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0076, null);
                //throw new Exception(MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0076).Message);
            }

            // Check file extension.
            string filePath = String.Format("{0}{1}", CommonUtil.WebPath, CommonValue.ALLOW_TYPE_FILE);
            XmlDocument configDoc = new XmlDocument();

            configDoc.Load(filePath);

            XmlNodeList extNode = configDoc.GetElementsByTagName("ext");
            List<String> extLst = new List<string>();

            foreach (XmlNode item in extNode)
            {
                extLst.Add(item.InnerText);
            }

            if (!extLst.Contains(fileType.ToUpper().Replace(".", String.Empty)))
            {
                // File extension not allow.
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0075, null);
                //throw new Exception(MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0075).Message);
            }

            res = true;

            return res;
        }
        /// <summary>
        /// Download attached file
        /// </summary>
        /// <param name="AttachFileID"></param>
        /// <param name="SessionId"></param>
        /// <returns></returns>
        public Stream GetAttachFileForDownload(int AttachFileID, string SessionId)
        {
            try
            {
                List<tbt_AttachFile> list = base.GetTbt_AttachFile(null, AttachFileID, null);

                if (list.Count > 0)
                {
                    // GetAttachmentFilePath() : \\192.168.33.2\FileServer\CSI\SECOM_TEST_DOCUMENT\Actual
                    // GetAttachmentFilePath() + FilePaht
                    // .FilePaht = "Incident\RelatedId-FileName.xlsx"

                    string filePath = string.Empty;

                    if (list[0].UploadCompleteFlag == true)
                    {
                        // Actual = GetAttachmentFilePath() + list[0].FilePaht
                        filePath = Path.Combine(GetAttachmentFilePath(), list[0].FilePath);
                    }
                    else
                    {
                        // Temporary = GetTemporaryAttachFilePath() + SessionId + list[0].FileName
                        string firstFilePath = Path.Combine(GetTemporaryAttachFilePath(), SessionId, list[0].FileName);
                        string secondFilePath = Path.Combine(GetTemporaryAttachFilePath(), list[0].RelatedID, list[0].FileName);

                        if (File.Exists(firstFilePath))
                        {
                            filePath = firstFilePath;
                        }
                        else if (File.Exists(secondFilePath))
                        {
                            filePath = secondFilePath;
                        }

                    }

                    byte[] f = File.ReadAllBytes(filePath);
                    return new MemoryStream(f);
                }
                else
                {
                    //ThrowErrorException(string module, MessageUtil.MessageList code, params string[] param)

                    return null;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// Copy directory
        /// </summary>
        /// <param name="SourcePath"></param>
        /// <param name="DestinationPath"></param>
        /// <param name="overwriteexisting"></param>
        /// <param name="strFileID"></param>
        /// <returns></returns>
        private bool CopyDirectory(string SourcePath, string DestinationPath, bool overwriteexisting, string strFileID = null) //Modify by Jutarat A. on 04022013 (Add strFileID)
        {
            bool ret = false;
            try
            {
                SourcePath = SourcePath.EndsWith(@"\") ? SourcePath : SourcePath + @"\";
                DestinationPath = DestinationPath.EndsWith(@"\") ? DestinationPath : DestinationPath + @"\";

                if (Directory.Exists(SourcePath))
                {
                    if (Directory.Exists(DestinationPath) == false)
                        Directory.CreateDirectory(DestinationPath);

                    string strFileName = string.Empty; //Add by Jutarat A. on 04022013
                    foreach (string fls in Directory.GetFiles(SourcePath))
                    {
                        FileInfo flinfo = new FileInfo(fls);
                        strFileName = String.IsNullOrEmpty(strFileID) ? flinfo.Name : String.Format("{0}-{1}", strFileID, flinfo.Name); //Add by Jutarat A. on 04022013

                        //flinfo.CopyTo(DestinationPath + flinfo.Name, overwriteexisting);
                        flinfo.CopyTo(DestinationPath + strFileName, overwriteexisting); //Modify by Jutarat A. on 04022013
                    }

                    strFileName = string.Empty; //Add by Jutarat A. on 04022013
                    foreach (string drs in Directory.GetDirectories(SourcePath))
                    {
                        DirectoryInfo drinfo = new DirectoryInfo(drs);
                        strFileName = String.IsNullOrEmpty(strFileID) ? drinfo.Name : String.Format("{0}-{1}", strFileID, drinfo.Name); //Add by Jutarat A. on 04022013

                        //if (CopyDirectory(drs, DestinationPath + drinfo.Name, overwriteexisting) == false)
                        if (CopyDirectory(drs, DestinationPath + strFileName, overwriteexisting) == false) //Modify by Jutarat A. on 04022013
                            ret = false;
                    }
                }
                ret = true;
            }
            catch (Exception ex)
            {
                ret = false;
            }
            return ret;
        }
        /// <summary>
        /// Get location full path of attached file
        /// </summary>
        /// <returns></returns>
        public string GetAttachmentFilePath()
        {
            //ICommonHandler handler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;


            // Edit by Narupon W. 9 May 2012
            //List<doSystemConfig> cfg = this.GetSystemConfig("AttachFilePath");
            //List<doSystemConfig> cfg = this.GetSystemConfig(ConfigName.C_CONFIG_ATTACH_FILE_PATH);
            //string path = cfg[0].ConfigValue;

            string path = PathUtil.GetPathValue(PathUtil.PathName.AttachFilePath);

            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }
        /// <summary>
        /// Get tempory attached file path
        /// </summary>
        /// <returns></returns>
        public string GetTemporaryAttachFilePath()
        {
            //ICommonHandler handler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            // Edit by Narupon W. 9 May 2012
            //List<doSystemConfig> cfg = this.GetSystemConfig("TemporaryAttachFilePath");
            //string path = cfg[0].ConfigValue;
            string path = PathUtil.GetPathValue(PathUtil.PathName.TemporaryAttachFilePath);

            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        // Transaction Method

        /// <summary>
        /// Register attached file to system
        /// </summary>
        /// <param name="relatedID"></param>
        /// <param name="fileName"></param>
        /// <param name="fileType"></param>
        /// <param name="fileSize"></param>
        /// <param name="fileBinary"></param>
        /// <param name="uploadCompleteFlag"></param>
        /// <returns></returns>
        public List<tbt_AttachFile> InsertAttachFile(string relatedID, string fileName, string fileType, Nullable<int> fileSize, byte[] fileBinary, Nullable<bool> uploadCompleteFlag)
        {
            List<tbt_AttachFile> result = new List<tbt_AttachFile>(); //Add by Jutarat A. on 04032013

            try
            {
                relatedID = String.IsNullOrEmpty(relatedID) ? string.Empty : relatedID.Trim(); //relatedID.Trim(); //Modify by Jutarat A. on 04032013
                fileType = String.IsNullOrEmpty(fileType) ? string.Empty : fileType.ToLower(); //fileType.ToLower(); //Modify by Jutarat A. on 04032013
                fileName = String.IsNullOrEmpty(fileName) ? string.Empty : fileName.Trim() + fileType; //fileName.Trim() + fileType; //Modify by Jutarat A. on 04032013

                if (fileName.Length > 100)
                {
                    string tmpName = Path.GetFileNameWithoutExtension(fileName).Trim();
                    string tmpExtension = Path.GetExtension(fileName).Trim().TrimEnd('.');

                    fileName = tmpName.Substring(0, 100 - tmpExtension.Length) + tmpExtension;
                    fileName = fileName.Trim().TrimEnd('.');
                    if (fileName.Length > 100)
                    {
                        fileName = fileName.Substring(0, 100);
                    }
                }

                DateTime currDate = DateTime.Now;
                string path;// = AttachDocumentCondition.C_CONFIG_TEMP_ATTACH_FILE_PATH;                        
                path = Path.Combine(GetTemporaryAttachFilePath(), relatedID); //String.Format("{0}{1}\\", GetTemporaryAttachFilePath(), relatedID);

                if (Directory.Exists(path) == false)
                {
                    Directory.CreateDirectory(path);
                }

                FileInfo fi = new FileInfo(fileName);

                using (var transaction = new TransactionScope())
                {
                    result = base.InsertAttachFile(relatedID, fi.Name, fileType, fileSize, null, uploadCompleteFlag, currDate, CommonUtil.dsTransData.dtUserData.EmpNo, currDate, CommonUtil.dsTransData.dtUserData.EmpNo);

                    if (result == null || result.Count <= 0)
                    {
                        throw new ApplicationException("Unable to insert tbt_AttachFile");
                    }

                    string tempFilePath = Path.Combine(path, result[0].FileName);
                    File.WriteAllBytes(tempFilePath, fileBinary);
                    
                    transaction.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }
        /// <summary>
        /// Copy attached file for temporary path ot acual path
        /// </summary>
        /// <param name="module"></param>
        /// <param name="relatedID"></param>
        /// <param name="newRelatedID"></param>
        /// <param name="deleteSource"></param>
        /// <returns></returns>
        public List<tbt_AttachFile> CopyAttachFile(string module, string relatedID, string newRelatedID, bool deleteSource = false)
        {
            module = module.Trim();
            relatedID = relatedID.Trim();
            newRelatedID = newRelatedID.Trim();

            //Modify by Jutarat A. on 04022013
            //string temp = String.Format("{0}{1}\\", GetTemporaryAttachFilePath(), relatedID);
            //string actual = String.Format("{0}\\{1}\\{2}", GetAttachmentFilePath(), module, newRelatedID);
            string temp = Path.Combine(GetTemporaryAttachFilePath(), relatedID);
            string actual = Path.Combine(GetAttachmentFilePath(), module);
            //End Modify

            //Directory.(temp, actual);
            List<tbt_AttachFile> list = base.CopyAttachFile(relatedID, newRelatedID, module);
            
            //CopyDirectory(temp, actual, true);
            CopyDirectory(temp, actual, true, newRelatedID); //Modify by Jutarat A. on 04022013
            if (deleteSource)
            {
                Directory.Delete(temp, true);
            }

            return list;
        }
        /// <summary>
        /// Delete attach file by id
        /// </summary>
        /// <param name="attachFileID"></param>
        /// <param name="sessionID"></param>
        /// <returns></returns>
        public int DeleteAttachFileByID(Nullable<int> attachFileID, string sessionID)
        {
            sessionID = sessionID.Trim();
            List<tbt_AttachFile> list = base.GetTbt_AttachFile(null, attachFileID, null);


            if (list.Count > 0)
            {
                string filename = list[0].FileName;
                string path = string.Empty;
                if (!(bool)list[0].UploadCompleteFlag) // New file -> temporary
                {
                    path = Path.Combine(GetTemporaryAttachFilePath(), list[0].RelatedID); //String.Format("{0}{1}\\", GetTemporaryAttachFilePath(), list[0].RelatedID);

                    File.Delete(Path.Combine(path, filename));
                    base.DeleteAttachFileByID(attachFileID);
                }
                else // Old file
                {
                    // Back up old attached file (that UploadCompleteFlag == 1) for delete when user press confrim botton.
                    base.BackUpAttachFile(attachFileID, sessionID);
                }


                return 1;
            }
            return 0;
        }

        /// <summary>
        /// Delete actual attached file by id
        /// </summary>
        /// <param name="attachFileID"></param>
        /// <returns></returns>
        public int DeleteActualAttachFileByID(Nullable<int> attachFileID)
        {

            List<tbt_AttachFile> list = base.GetTbt_AttachFile(null, attachFileID, null);

            if (list.Count > 0)
            {
                string filename = list[0].FileName;
                string path = string.Empty;
                
                path = Path.Combine(GetAttachmentFilePath(), list[0].RelatedID); //String.Format("{0}{1}\\", GetTemporaryAttachFilePath(), list[0].RelatedID);

                File.Delete(Path.Combine(path, filename));
                base.DeleteAttachFileByID(attachFileID);
                
                return 1;
            }
            return 0;
        }

        /// <summary>
        /// Delete Attach file temporary by name
        /// </summary>
        /// <param name="strSessionID"></param>
        /// <param name="strFileName"></param>
        /// <returns></returns>
        public bool DeleteAttachFileTemporaryByFileName(string strSessionID, string strFileName)
        {
            base.DeleteAttachFileTemporaryByFileName(strSessionID, strFileName);            
            return true;
        }

        /// <summary>
        /// Clear data and delate of temporaly upload file
        /// </summary>
        /// <param name="relatedID"></param>
        /// <returns></returns>
        public int ClearTemporaryUploadFile(string relatedID)
        {
            relatedID = relatedID.Trim();
            // Clare tbt_AttachFileTemporary by sessionID
            base.RemoveAttachFileTemporary(relatedID, false);

            // Clear Temp folder
            string temp = Path.Combine(GetTemporaryAttachFilePath(), relatedID);

            if (Directory.Exists(temp))
            {
                Directory.Delete(temp, true);
            }


            return 0;
        }
        /// <summary>
        /// Update UploadCompleFlag and copy temporaryfile to actual path when submit action.
        /// </summary>
        /// <param name="module"></param>
        /// <param name="relatedID"></param>
        /// <param name="newRelatedID"></param>
        /// <returns></returns>
        public int UpdateFlagAttachFile(string module, string relatedID, string newRelatedID)
        {
            // Action of UpdateFlagAttachFile 
            // 1. Move new file from Temp to Actual
            // 2. Update flag for new entries (file) -> tbt_AttachFile.UploadCompleteFlag
            // 3. In case edit -> old file is removed
            //      3.1 Get list of removed file for tbt_AttachFileTemporary
            //      3.2 Clare tbt_AttachFileTemporary by sessionID
            //      3.3 Remove physical file in Actual
            // 4. Clear Temp folder

            module = module.Trim();
            relatedID = relatedID.Trim();
            newRelatedID = newRelatedID.Trim();

            string temp = Path.Combine(GetTemporaryAttachFilePath(), relatedID); // String.Format("{0}{1}\\", GetTemporaryAttachFilePath(), relatedID);
            string actual = Path.Combine(GetAttachmentFilePath(), module); // String.Format("{0}{1}\\{2}", GetAttachmentFilePath(), module, newRelatedID);

            if (Directory.Exists(temp) == false)
            {
                Directory.CreateDirectory(temp);
            }

            string[] files = Directory.GetFiles(temp);

            if (Directory.Exists(actual) == false)
            {
                Directory.CreateDirectory(actual);
            }

            string sourceFilePath = string.Empty;
            string destFileName = string.Empty;
            string destFilePath = string.Empty;
            string entryFileName = string.Empty;
            FileInfo fileInfo;


           

            var fileList = GetTbt_AttachFile(relatedID, null, false);

            foreach (string f in files)
            {
                fileInfo = new FileInfo(f);

                var targFileLst = fileList.Where(x => x.FileName.ToUpper().Equals(fileInfo.Name.ToUpper()));
                if (targFileLst.Count() == 1)
                {
                    var targFile = targFileLst.First();
                    entryFileName = fileInfo.Name;
                    sourceFilePath = Path.Combine(temp, fileInfo.Name);
                    destFileName = string.Format("{0}-{1}", newRelatedID, fileInfo.Name); // ex. 1001-TestContract.doc
                    destFilePath = Path.Combine(actual, destFileName);
                    
                    //======= Teerapong 23/07/2012 ========
                    if (File.Exists(destFilePath))
                    {
                        DeleteAttachFileTemporaryByFileName(relatedID, fileInfo.Name);
                        File.Delete(destFilePath);
                    }
                    //=====================================
                    
                    File.Move(sourceFilePath, destFilePath);

                    base.UpdateFlagAttachFile(relatedID, newRelatedID, entryFileName, string.Format(@"{0}\{1}", module, destFileName), targFile.AttachFileID);
                }
            }

            // ===  3.1 ====

            // Get data from tbt_AttachFileTemporary
            List<dtAttachFileTemporary> list = base.GetTbt_AttachFileTemporary(relatedID);

            // Clare tbt_AttachFileTemporary by sessionID
            base.RemoveAttachFileTemporary(relatedID, true);

            // Remove physical file in Actual
            string removeFile = string.Empty;
            foreach (var item in list)
            {
                removeFile = Path.Combine(GetAttachmentFilePath(), item.FilePath);
                if (File.Exists(removeFile))
                {
                    File.Delete(removeFile);
                }

            }
            // ===== end 3.3 ===

            // Clear Temp folder
            if (Directory.Exists(temp))
            {
                Directory.Delete(temp, true);
            }



            return 0;
        }
        /// <summary>
        /// Get pop-up sub menu list
        /// </summary>
        /// <param name="popupSubmenuID"></param>
        /// <returns></returns>
        public override List<doPopupSubMenuList> GetPopupSubMenuList(string popupSubmenuID)
        {
            try
            {
                if (CommonUtil.IsNullOrEmpty(popupSubmenuID))
                {
                    throw ApplicationErrorException.ThrowErrorException(
                        MessageUtil.MODULE_COMMON,
                        MessageUtil.MessageList.MSG0007,
                        "PopupSubMenuID");
                }

                List<doPopupSubMenuList> lst = base.GetPopupSubMenuList(popupSubmenuID);
                CommonUtil.MappingObjectLanguage<doPopupSubMenuList>(lst);

                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Get account data of carry-over & Profit (sp_CM_GetAccountDataOfCarryOverAndProfit)
        /// </summary>
        /// <param name="StartTargetDate">start target date</param>
        /// <param name="EndTargetDate">end target date</param>
        /// <param name="FiveBusinessDate">five business date</param>
        /// <param name="ProductTypeCode">product type code</param>
        /// <returns></returns>
        public List<dtAccountDataOfCarryOverAndProfit> GetAccountDataOfCarryOverAndProfit(DateTime StartTargetDate, DateTime EndTargetDate, DateTime FiveBusinessDate, string ProductTypeCode)
        {
            List<dtAccountDataOfCarryOverAndProfit> result = base.GetAccountDataOfCarryOverAndProfit(
                StartTargetDate, EndTargetDate, FiveBusinessDate, ProductTypeCode
                , ProductType.C_PROD_TYPE_AL
                , ProductType.C_PROD_TYPE_ONLINE
                , ProductType.C_PROD_TYPE_BE
                , ProductType.C_PROD_TYPE_SG
                , ProductType.C_PROD_TYPE_MA
                , ProductType.C_PROD_TYPE_RENTAL_SALE
                , SECOM_AJIS.Common.Util.ConstantValue.GroupProductType.C_GROUP_PRODUCT_TYPE_N
                , SECOM_AJIS.Common.Util.ConstantValue.GroupProductType.C_GROUP_PRODUCT_TYPE_SG
                , SECOM_AJIS.Common.Util.ConstantValue.GroupProductType.C_GROUP_PRODUCT_TYPE_MA
                , BillingType.C_BILLING_TYPE_SERVICE
                , MiscType.C_RENTAL_CHANGE_TYPE);
            return result;
        }

        /// <summary>
        /// Insert manage account data of carry-over & profit information to the system (sp_CM_InsertTbt_ManageCarryOverProfit)
        /// </summary>
        /// <param name="xmlTbt_ManageCarryOverProfit"></param>
        /// <returns></returns>
        public override List<tbt_ManageCarryOverProfit> InsertTbt_ManageCarryOverProfit(string xmlTbt_ManageCarryOverProfit)
        {
            List<tbt_ManageCarryOverProfit> inserted = base.InsertTbt_ManageCarryOverProfit(xmlTbt_ManageCarryOverProfit);
            return inserted;
        }

        public void UpdateBillingHistoryOfManageCarryOverProfit(string reportYear, string reportMonth, string productType, string strEmpNo, DateTime dtDateTime)
        {
            base.UpdateBillingHistoryOfManageCarryOverProfit(reportYear, reportMonth, productType, (DateTime?)dtDateTime, strEmpNo);
        }


        /// <summary>
        /// Get manage account data of carry-over & Profit Process (sp_CM_GetTbt_ManageCarryOverProfit)
        /// </summary>
        /// <param name="reportYear">report year</param>
        /// <param name="reportMonth">report month</param>
        /// <returns></returns>
        public List<tbt_ManageCarryOverProfit> GetTbt_ManageCarryOverProfit(string reportYear, string reportMonth)
        {
            List<tbt_ManageCarryOverProfit> result = base.GetTbt_ManageCarryOverProfit(
                reportYear, reportMonth, null);
            return result;
        }

        public List<doResultManageCarryOverProfitForEdit> GetManageCarryOverProfitForEdit(string reportYear, string reportMonth, string productType, string contractCode, string billingOCC)
        {
            List<doResultManageCarryOverProfitForEdit> result = base.GetManageCarryOverProfitForEdit(reportYear, reportMonth, productType, contractCode, billingOCC, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
            return result;
        }

        public void UpdateTbtManageCarryOverProfit(string reportYear, string reportMonth, string contractCode, string billingOCC, Nullable<decimal> receiveAmount, Nullable<decimal> receiveAmountUsd, string receiveAmountCurrencyType, Nullable<decimal> incomeRentalFee, Nullable<decimal> incomeRentalFeeUsd, string incomeRentalFeeCurrencyType, Nullable<decimal> accumulatedReceiveAmount, Nullable<decimal> accumulatedReceiveAmountUsd, string accumulatedReceiveAmountCurrencyType, Nullable<decimal> accumulatedUnpaid, Nullable<decimal> accumulatedUnpaidUsd, string accumulatedUnpaidCurrencyType, Nullable<decimal> incomeVat, Nullable<decimal> incomeVatUsd, string incomeVatCurrencyType, Nullable<decimal> unpaidPeriod, Nullable<System.DateTime> incomeDate, string updateBy)
        {
            base.UpdateTbtManageCarryOverProfit(reportYear, reportMonth, contractCode, billingOCC, receiveAmount, receiveAmountUsd, receiveAmountCurrencyType, incomeRentalFee, incomeRentalFeeUsd, incomeRentalFeeCurrencyType, accumulatedReceiveAmount, accumulatedReceiveAmountUsd, accumulatedReceiveAmountCurrencyType, accumulatedUnpaid, accumulatedUnpaidUsd, accumulatedUnpaidCurrencyType, incomeVat, incomeVatUsd, incomeVatCurrencyType, unpaidPeriod, incomeDate, updateBy);
        }

        //Add by Jutarat A. on 17092013
        /// <summary>
        /// Validate and allocate Printing process
        /// </summary>
        /// <param name="strPrintingFlag"></param>
        /// <returns></returns>
        public bool AllocatePrintingProcess(string strPrintingFlag, string strScreenID, ref string strErrorMessage)
        {
            try
            {
                string PROCESS_NAME_READER = "Foxit Reader";
                string PROCESS_NAME_PORTABLE = "FoxitReaderPortable";
                ILogHandler logHandler = ServiceContainer.GetService<ILogHandler>() as ILogHandler;

                if (strPrintingFlag != PrintingFlag.C_PRINTING_FLAG_NO_PRINT)
                {
                    //Check "PrintingFlag"
                    List<doSystemConfig> doSystemConfigList = GetSystemConfig(ConfigName.C_CONFIG_PRINTING_FLAG);
                    if (doSystemConfigList != null && doSystemConfigList.Count > 0)
                    {
                        string currPrintFlag = doSystemConfigList[0].ConfigValue;
                        if (currPrintFlag != PrintingFlag.C_PRINTING_FLAG_NO_PRINT)
                        {
                            strErrorMessage = "This process was forced to finish because another printing service started.";
                            logHandler.WriteWindowLog(EventType.C_EVENT_TYPE_ERROR, strErrorMessage, EventID.C_EVENT_ID_INTERNAL_ERROR);

                            return false;
                        }
                        else
                        {
                            //Kill "Foxit Reader.exe" & "FoxitReaderPortable.exe" immediately without any message
                            string strForcedMessage = "Forced to finish FoxitReader process.";

                            Process[] ps1 = Process.GetProcessesByName(PROCESS_NAME_READER);
                            foreach (Process p1 in ps1)
                            {
                                if (p1.ProcessName == PROCESS_NAME_READER)
                                {
                                    Thread.Sleep(1000);
                                    if (!p1.HasExited)
                                    {
                                        p1.Kill();
                                        p1.Dispose();

                                        logHandler.WriteWindowLog(EventType.C_EVENT_TYPE_INFORMATION, strForcedMessage, EventID.C_EVENT_ID_INTERNAL_ERROR);
                                    }
                                }
                            }

                            Process[] ps2 = Process.GetProcessesByName(PROCESS_NAME_PORTABLE);
                            foreach (Process p2 in ps2)
                            {
                                if (p2.ProcessName == PROCESS_NAME_PORTABLE)
                                {
                                    Thread.Sleep(1000);
                                    if (!p2.HasExited)
                                    {
                                        p2.Kill();
                                        p2.Dispose();

                                        logHandler.WriteWindowLog(EventType.C_EVENT_TYPE_INFORMATION, strForcedMessage, EventID.C_EVENT_ID_INTERNAL_ERROR);
                                    }
                                }
                            }
                            //

                            //Update "PrintingFlag"
                            UpdateSystemConfig(ConfigName.C_CONFIG_PRINTING_FLAG, strPrintingFlag, strScreenID, null);

                            return true;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return false;
        }

        /// <summary>
        /// Reset Printing process (set PrintingFlag = C_PRINTING_FLAG_NO_PRINT)
        /// </summary>
        /// <returns></returns>
        public bool ResetPrintingProcess(string strScreenID)
        {
            try
            {
                string PROCESS_NAME_READER = "Foxit Reader";
                string PROCESS_NAME_PORTABLE = "FoxitReaderPortable";
                ILogHandler logHandler = ServiceContainer.GetService<ILogHandler>() as ILogHandler;

                //Kill "Foxit Reader.exe" & "FoxitReaderPortable.exe" immediately without any message
                string strForcedMessage = "Forced to finish FoxitReader process.";

                Process[] ps1 = Process.GetProcessesByName(PROCESS_NAME_READER);
                foreach (Process p1 in ps1)
                {
                    if (p1.ProcessName == PROCESS_NAME_READER)
                    {
                        Thread.Sleep(1000);
                        if (!p1.HasExited)
                        {
                            p1.Kill();
                            p1.Dispose();

                            logHandler.WriteWindowLog(EventType.C_EVENT_TYPE_INFORMATION, strForcedMessage, EventID.C_EVENT_ID_INTERNAL_ERROR);
                        }
                    }
                }

                Process[] ps2 = Process.GetProcessesByName(PROCESS_NAME_PORTABLE);
                foreach (Process p2 in ps2)
                {
                    if (p2.ProcessName == PROCESS_NAME_PORTABLE)
                    {
                        Thread.Sleep(1000);
                        if (!p2.HasExited)
                        {
                            p2.Kill();
                            p2.Dispose();

                            logHandler.WriteWindowLog(EventType.C_EVENT_TYPE_INFORMATION, strForcedMessage, EventID.C_EVENT_ID_INTERNAL_ERROR);
                        }
                    }
                }
                //

                //Update "PrintingFlag"
                UpdateSystemConfig(ConfigName.C_CONFIG_PRINTING_FLAG, PrintingFlag.C_PRINTING_FLAG_NO_PRINT, strScreenID, null);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Printing PDF
        /// </summary>
        /// <param name="strPathFilename"></param>
        public void PrintPDF(string strPathFilename)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = ConfigurationManager.AppSettings["PrintPDFFoxit"];
                process.StartInfo.Arguments = string.Format("/t \"{0}\" \"{1}\"", strPathFilename, ConfigurationManager.AppSettings["PrinterName"]);
                process.StartInfo.Verb = "Print";
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();

                int intPrintTimeOut = Convert.ToInt32(ConfigurationManager.AppSettings["PrintTimeOut"]);
                if (process.WaitForExit(intPrintTimeOut) == false) //Wait a maximum of 1 min for the process to finish
                {
                    if (!process.HasExited)
                    {
                        process.Kill();
                        process.Dispose();
                    }

                    throw new Exception("Print Timeout");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //End Add

        #region MiscellaneousType

        public string getCurrencyName(string currencyCode, string defaultValue = "", bool isThrowException = false)
        {
            try
            {
                List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                {
                    new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_CURRENCT,
                        ValueCode = "%"
                    }
                };

                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                lst = hand.GetMiscTypeCodeList(miscs).ToList();

                return lst.Where(m => m.ValueCode == currencyCode).Select(m => m.ValueDisplayEN).First();
            }
            catch (Exception ex)
            {
                if (isThrowException) throw ex;
                else return defaultValue;
            }
        }
        public string getCurrencyFullName(string currencyCode, string defaultValue = "", bool isThrowException = false)
        {
            try
            {
                List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                {
                    new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_CURRENCT,
                        ValueCode = "%"
                    }
                };

                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                lst = hand.GetMiscTypeCodeList(miscs).ToList();

                return lst.Where(m => m.ValueCode == currencyCode).Select(m => m.ValueDescription).First();
            }
            catch (Exception ex)
            {
                if (isThrowException) throw ex;
                else return defaultValue;
            }
        }

        #endregion

        // Add by Jirawat Jannet on 2016-12-06


        #region Convert currency price

        public decimal? ConvertCurrencyPrice(decimal? price, string fromCurrencyType, string toCurrencyType, DateTime TargetDate,ref double ErrorCode, decimal? defaultPrice = null)
        {
            if (price == null) return defaultPrice;

            return ConvertCurrencyPrice(price.Value, fromCurrencyType, toCurrencyType, TargetDate, ref ErrorCode);
        }

        public decimal ConvertCurrencyPrice(decimal price, string fromCurrencyType, string toCurrencyType, DateTime TargetDate, ref double ErrorCode)
        {
            if (string.IsNullOrEmpty(fromCurrencyType))
                fromCurrencyType = CurrencyUtil.C_CURRENCY_LOCAL;

            if (fromCurrencyType == toCurrencyType) return price;
            else
            {
                ConvertExchangeRateHandler handler = new ConvertExchangeRateHandler();

                if (fromCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL && toCurrencyType == CurrencyUtil.C_CURRENCY_US)
                    return handler.ConvertAmountByBankRate(TargetDate.Date, RateCalcCode.C_CONVERT_TYPE_TO_DOLLAR, price, ref ErrorCode);
                else if (fromCurrencyType == CurrencyUtil.C_CURRENCY_US && toCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                    return handler.ConvertAmountByBankRate(TargetDate.Date, RateCalcCode.C_CONVERT_TYPE_TO_RPIAH, price, ref ErrorCode);
                else
                    return handler.ConvertAmountByBankRate(TargetDate.Date, string.Empty, price, ref ErrorCode);
            }
        }

        #endregion
    }
}
