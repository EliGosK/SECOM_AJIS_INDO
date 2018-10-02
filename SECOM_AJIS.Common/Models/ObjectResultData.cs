using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using System.Web;
using System.Web.Mvc;
using NLog;


namespace SECOM_AJIS.Common.Models
{
    /// <summary>
    /// DO for send result to screen
    /// </summary>
    [Serializable]
    public class ObjectResultData
    {
        #region Variables

        public List<MessageModel> MessageList { get; private set; }
        public MessageModel.MESSAGE_TYPE MessageType { get; set; }
        public object ResultData { get; set; }

        //[NonSerialized]
        //private List<Exception> _ExceptionList;

        #endregion
        #region Constructors

        public ObjectResultData()
        {
            this.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
        }

        #endregion
        #region Methods
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public void AddErrorMessage(string module, MessageUtil.MessageList code, string[] param = null, string[] controls = null)
        {
            try
            {
                if (MessageList == null)
                    MessageList = new List<MessageModel>();
                MessageModel msg = MessageUtil.GetMessage(module, code, param);
                msg.Controls = controls;

                MessageList.Add(msg);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void AddErrorMessage(string controller, string screen, string module, MessageUtil.MessageList code, string[] param = null, string[] controls = null)
        {
            try
            {
                if (MessageList == null)
                    MessageList = new List<MessageModel>();

                MessageModel msg = null;
                if (CommonUtil.IsNullOrEmpty(controller) || CommonUtil.IsNullOrEmpty(screen))
                    msg = MessageUtil.GetMessage(module, code, param);
                else
                    msg = MessageUtil.GetMessageForScreen(controller, screen, module, code, param);

                msg.Controls = controls;

                MessageList.Add(msg);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static void WriteTableErrorLog(DateTime dt, Exception ex)
        {
            Logger logger = LogManager.GetCurrentClassLogger();
            Exception logEx = ex;

            while (logEx.InnerException != null)
            {
                logEx = logEx.InnerException;
            }
            var logEventInfo = new LogEventInfo(LogLevel.Error, "databaselog", "logMessage");
            logEventInfo.Properties["ErrorDescription"] = logEx.Message;
            logEventInfo.Properties["CreateDate"] = dt;
            string userID = "SYSTEM";
            try
            {
                userID=CommonUtil.dsTransData.dtUserData.EmpNo;
            }
            catch (Exception)
            {

            }
            logEventInfo.Properties["UserID"] = userID;
            logEventInfo.Exception = ex;
            
            logger.Log(logEventInfo);

        }

        private bool IsExceptionCanAutoRetry(Exception ex, ref bool bIsDbError)
        {
            const string ERROR_DEADLOCK = "deadlocked on lock resources with another process and has been chosen as the deadlock victim. Rerun the transaction.";
            const string ERROR_COMMAND_TIMEOUT = "Timeout expired.  The timeout period elapsed prior to completion of the operation or the server is not responding.";
            const string ERROR_TRANSACTION_TIMEOUT = "Transaction Timeout";

            try
            {
                HttpContext http = HttpContext.Current;
                object objRetryCount = null;
                if (http != null)
                {
                    if (http.Request.RequestContext.RouteData.DataTokens.TryGetValue("retry_count", out objRetryCount))
                    {
                        if ((int)objRetryCount == SECOM_AJIS.Common.ActionFilters.AutoRetryAttribute.MAX_RETRY)
                        {
                            bIsDbError = false;
                            return false;
                        }
                    }
                }

                Exception exTemp = ex;
                while (exTemp != null && !string.IsNullOrEmpty(exTemp.Message))
                {
                    if (exTemp.Message.Contains(ERROR_DEADLOCK)
                        || exTemp.Message.Contains(ERROR_COMMAND_TIMEOUT)
                        || exTemp.Message == ERROR_TRANSACTION_TIMEOUT
                    )
                    {
                        System.Diagnostics.Debug.WriteLine("[Autoretry] {0} SID:{1} ErrorMessage:{2} RetryCount:{3}", DateTime.Now, (HttpContext.Current != null ? HttpContext.Current.Session.SessionID : null), exTemp.Message, objRetryCount);
                        bIsDbError = false;
                        return true;
                    }

                    exTemp = exTemp.InnerException;
                }
                return false;
            }
            catch (Exception)
            {
                //Do nothing
                bIsDbError = false;
                return false;
            }
        }

        public void AddErrorMessage(Exception ex)
        {
            // Edit by Narupon W.
            // Write WindowsEventLog when database connection is disconnected.
            bool dbError = false;
            const string DB_DISCONNECT_ERROR_MESSAGE = "provider: TCP Provider, error: 0";
            const string ERROR_MESSAGE = "Database connection is disconnected.";
            const string ERROR_NUM_MESSAGE = "Arithmetic overflow error converting varchar to data type numeric.";

            if (ex is System.Data.EntityCommandExecutionException)
            {
                if (ex.Message.Contains(DB_DISCONNECT_ERROR_MESSAGE))
                {
                    // write WindowsEventLog (Type -> Error)
                    // Message = Database connection is disconnected
                    CommonUtil.WriteWindowLog(EventType.C_EVENT_TYPE_ERROR, ERROR_MESSAGE,EventID.C_EVENT_ID_INTERNAL_ERROR);
                    dbError = true;
                }
                else
                {
                    if (ex.InnerException != null)
                    {
                        if (ex.InnerException.Message.Contains(ERROR_NUM_MESSAGE))
                        {
                            ex = ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0152);
                        }
                        else if (ex.InnerException.Message.Contains(DB_DISCONNECT_ERROR_MESSAGE))
                        {
                            // write WindowsEventLog (Type -> Error)
                            // Message = Database connection is disconnected
                            CommonUtil.WriteWindowLog(EventType.C_EVENT_TYPE_ERROR, ERROR_MESSAGE, EventID.C_EVENT_ID_INTERNAL_ERROR);
                            dbError = true;
                        }
                    }
                }
            }

            if (IsExceptionCanAutoRetry(ex, ref dbError))
            {
                CommonUtil.WriteWindowLog(EventType.C_EVENT_TYPE_ERROR, ex.Message, EventID.C_EVENT_ID_INTERNAL_ERROR);
                this.AutoRetry = true;
            }

            MessageModel errorMessage;

            try
            {
                if (ex != null)
                {
                    ApplicationErrorException aEx = null;
                    if (ex is ApplicationErrorException)
                        aEx = ex as ApplicationErrorException;
                    else if (ex.InnerException is ApplicationErrorException)
                        aEx = ex.InnerException as ApplicationErrorException;
                    var errorDateTime = DateTime.Now;
                    if (aEx != null)
                    {
                        if (aEx.ErrorResult != null)
                            this.MessageList = aEx.ErrorResult.MessageList;
                        else
                        {
                            if (!dbError && !this.AutoRetry)
                            {
                                WriteTableErrorLog(errorDateTime, ex);

                                // Write to windows log // Narupon 3-sep-2012
                                CommonUtil.WriteWindowLog(EventType.C_EVENT_TYPE_ERROR, ex.Message, EventID.C_EVENT_ID_INTERNAL_ERROR);
                            }


                            //Narupon W.
                            //Get message MSG0111: Internal error , Please contact system administrator.
                            //errorMessage = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0111, new string[] { ex.Message });
                            if (this.AutoRetry)
                            {
                                errorMessage = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0159, new string[] { "<br/>ERROR:" + ex.Message });
                            }
                            else
                            {
                                errorMessage = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0111, new string[] { "<br/>ERROR:" + ex.Message });
                            }

                            this.MessageList = new List<MessageModel>();
                            this.MessageList.Add(new MessageModel()
                            {
                                Code = CommonValue.SYSTEM_MESSAGE_CODE,
                                //Message = ex.Message
                                Message = string.Format("{0} ({1})", errorMessage.Message, errorDateTime.ToString("yyyy-MM-dd HH:mm:ss")) // old: yyyyMMdd-HHmmss
                                // SELECT CONVERT(VARCHAR(100), CreateDate, 120) FROM dbo.tbt_ErrorLog -- yyyy-MM-dd HH:mm:ss
                            });
                        }
                        this.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    }
                    else
                    {

                        if (!dbError && !this.AutoRetry)
                        {
                            WriteTableErrorLog(errorDateTime, ex);

                            // Write to windows log // Narupon 3-sep-2012
                            CommonUtil.WriteWindowLog(EventType.C_EVENT_TYPE_ERROR, ex.Message, EventID.C_EVENT_ID_INTERNAL_ERROR);
                        }


                        //Narupon W.
                        //Get message MSG0111: Internal error , Please contact system administrator.
                        //errorMessage = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0111, new string[] { logEx.Message });
                        errorMessage = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0111, new string[] { "<br/>ERROR:" + ex.Message });

                        this.MessageList = new List<MessageModel>();
                        this.MessageList.Add(new MessageModel()
                        {
                            Code = CommonValue.SYSTEM_MESSAGE_CODE,
                            //Message = logEx.Message
                            Message = string.Format("{0} ({1})", errorMessage.Message, errorDateTime.ToString("yyyy-MM-dd HH:mm:ss")) // old: yyyyMMdd-HHmmss
                            // SELECT CONVERT(VARCHAR(100), CreateDate, 120) FROM dbo.tbt_ErrorLog -- yyyy-MM-dd HH:mm:ss
                        });
                        this.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            //finally
            //{
            //    if (ex != null)
            //    {
            //        this.ExceptionList.Add(ex);
            //    }
            //}
        }

        private MessageModel ToWarningMessage()
        {
            MessageModel msg = new MessageModel();
            msg.Code = CommonValue.SYSTEM_MESSAGE_CODE;
            msg.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            List<string> ctrlLst = new List<string>();
            if (this.MessageList != null)
            {
                TagBuilder ul = new TagBuilder("ul");
                foreach (MessageModel imsg in this.MessageList)
                {
                    TagBuilder li = new TagBuilder("li");
                    li.InnerHtml = imsg.Message;
                    ul.InnerHtml += li.ToString(TagRenderMode.Normal);

                    if (imsg.Controls != null)
                    {
                        if (imsg.Controls.Length > 0)
                            ctrlLst.AddRange(imsg.Controls);
                    }
                }

                msg.Message = ul.ToString(TagRenderMode.Normal);
                if (ctrlLst.Count > 0)
                    msg.Controls = ctrlLst.ToArray();
            }

            return msg;
        }

        #endregion
        #region Properties

        public bool IsError
        {
            get
            {
                return this.Message != null;
            }
        }
        public bool HasResultData
        {
            get
            {
                return this.ResultData != null;
            }
        }
        public MessageModel Message
        {
            get
            {
                bool isNullMsg = true;
                if (this.MessageList != null)
                {
                    if (this.MessageList.Count > 0)
                        isNullMsg = false;
                }
                if (isNullMsg == false)
                {
                    if (this.MessageType == MessageModel.MESSAGE_TYPE.WARNING)
                        return ToWarningMessage();
                    else
                    {
                        MessageModel msg = MessageList[0];
                        msg.MessageType = this.MessageType;
                        return msg;
                    }
                }

                return null;
            }
        }
        public string ToJson
        {
            get
            {
                return null;// CommonUtil.CreateJsonString(this);
            }
        }
        public bool AutoRetry { get; set; }

        //[field:NonSerialized]
        //public List<Exception> ExceptionList
        //{
        //    get
        //    {
        //        if (_ExceptionList == null)
        //        {
        //            _ExceptionList = new List<Exception>();
        //        }
        //        return _ExceptionList;
        //    }
        //}

        #endregion
    }
}
