using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net;

using System.Data;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
using System.IO;

using System.Transactions;

using SECOM_AJIS.Common;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util.ConstantValue;

using System.Configuration;

namespace SECOM_AJIS.DataEntity.Common
{
    public class LogHandler : BizCMDataEntities, ILogHandler
    {
        /// <summary>
        /// Process - write data to Windows Event Log
        /// </summary>
        /// <param name="eEventType"></param>
        /// <param name="strMessage"></param>
        public void WriteWindowLog(string eEventType, string strMessage,int EventID)
        {
            
            //====== Teerapong S. 3/10/2012
            //string logName = "Application";
            //string source = "SECOM-AJIS web application";
            string logName = ConfigurationManager.AppSettings["EventLogName"];
            string source = ConfigurationManager.AppSettings["EventSourceName"];

            EventLog objLog = new EventLog();

            try
            {
                if (!EventLog.SourceExists(source))
                {
                    EventLog.CreateEventSource(source, logName);
                }

                objLog.Source = source;
                objLog.Log = logName;

                //strMessage : If the message string is longer than 32766 bytes then error.
                byte[] byte_error = Encoding.ASCII.GetBytes(strMessage);

 
                if (byte_error.Length > 32766)
                {
                    strMessage = strMessage.Substring(0,25000);
                }

                if (eEventType == EventType.C_EVENT_TYPE_ERROR)
                {
                    objLog.WriteEntry(strMessage, EventLogEntryType.Error, EventID);
                }
                else if (eEventType == EventType.C_EVENT_TYPE_WARNING)
                {
                    objLog.WriteEntry(strMessage, EventLogEntryType.Warning, EventID);
                }
                else
                {
                    objLog.WriteEntry(strMessage, EventLogEntryType.Information, EventID);
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Process - transaction log
        /// </summary>
        /// <param name="doTrans"></param>
        /// <param name="empNo"></param>
        /// <param name="screenID"></param>
        /// <returns></returns>
        public bool WriteTransactionLog(doTransactionLog doTrans, string empNo=null, string screenID=null)
        {
            // Akat K. 2011-10-27 : add mandatory check
            List<string> messageParam = new List<string>();
            if (doTrans.TableData == null) {
                messageParam.Add("TableData");
            }
            if (doTrans.TableName == null) {
                messageParam.Add("TableName");
            }
            if (doTrans.TransactionType == null) {
                messageParam.Add("TransactionType");
            }
            if (messageParam.Count > 0) {
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, messageParam.ToArray<string>());
            }

            string xml = toXmlString(doTrans.TableData, doTrans.TransactionType.ToString().Substring(0, 1));



            List<int?> tmp;
            if (empNo == null)
                tmp = base.WriteTransactionLog(CommonUtil.dsTransData.dtOperationData.GUID, CommonUtil.dsTransData.dtTransHeader.ScreenID, doTrans.TableName, xml, CommonUtil.dsTransData.dtOperationData.ProcessDateTime, CommonUtil.dsTransData.dtUserData.EmpNo);
            else
            {
                //using in Batch Process CMS050
                Guid g;
                g = Guid.NewGuid();
                tmp = base.WriteTransactionLog(g.ToString()
                                                , screenID
                                                , doTrans.TableName
                                                , xml
                                                , DateTime.Now
                                                , empNo);
            }
                
            return tmp[0] > 0 ? true : false;


        }

        /// <summary>
        /// Process - delete transaction log
        /// </summary>
        /// <param name="dtMonthYear"></param>
        /// <returns></returns>
        public List<tbt_PurgeLog> DeleteLog(DateTime dtMonthYear)
        { 
            return base.DeleteLog(dtMonthYear,CommonUtil.dsTransData.dtUserData.EmpNo);
        }

        /// <summary>
        /// Convert data list (class format) to xml format
        /// </summary>
        /// <param name="strTableData"></param>
        /// <param name="strTranType"></param>
        /// <returns></returns>
        private string toXmlString(string strTableData, String strTranType)
        { 
            try
            {
                string xml = string.Empty;
                //if (dt != null)
                //{
                //    XmlDocument doc = new XmlDocument();
                //    doc.LoadXml("<TransData></TransData>");

                //    //PropertyInfo[] props = cType.GetProperties();

                //    foreach (DataRow dr in dt.Rows )
                //    {
                //        XmlNode node = doc.CreateNode(XmlNodeType.Element, "TableData", "");
                //        doc.ChildNodes[0].AppendChild(node);
                        
                //        foreach (DataColumn col in dt.Columns)
                //        {
                //            XmlNode iNode = doc.CreateNode(XmlNodeType.Element, col.ColumnName, "");
                //            if (dr[col.ColumnName] != null)
                //            {
                //                iNode.InnerText = dr[col.ColumnName].ToString();
                //            }
                //            node.AppendChild(iNode);
                //        }
                //    }

                //    // Insert Transaction Type
                //    XmlNode nTran = doc.CreateNode(XmlNodeType.Element, "TransType", "");
                //    doc.ChildNodes[0].AppendChild(nTran);
                //    XmlNode iTNode = doc.CreateNode(XmlNodeType.Element, "Type", "");
                //    iTNode.InnerText = strTranType;
                //    nTran.AppendChild(iTNode);

                //    StringWriter sw = new StringWriter();
                //    XmlTextWriter tx = new XmlTextWriter(sw);
                //    doc.WriteTo(tx);

                //    xml = sw.ToString();
                //}
                xml += "<Transaction><TransactionType>";
                xml += strTranType;
                xml += "</TransactionType>";
                xml += strTableData;
                xml += "</Transaction>";
                return xml;
            }
            catch(Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Write history of downloaded document log
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public  int WriteDocumentDownloadLog(doDocumentDownloadLog cond)
        {
            try
            {
                int iResult = -1000;

                ApplicationErrorException.CheckMandatoryField(cond);

                using (TransactionScope t = new TransactionScope())
                {
                    // do stuff... update, delete
                    iResult = base.WriteDocumentDownloadLog(cond.DocumentNo, cond.DocumentCode, cond.DownloadDate, cond.DownloadBy, cond.DocumentOCC);

                    t.Complete();

                }

                return iResult;
            }
            catch (Exception)
            {
                throw ;
            }
        }

        /// <summary>
        /// Search Foxit process and write window log
        /// </summary>
        /// <param name="objectID"></param>
        public void SearchFoxitProcess(string objectID) //Add by Jutarat A. on 20082013
        {
            try
            {
                string PROCESS_NAME_READER = "Foxit Reader";
                string PROCESS_NAME_PORTABLE = "FoxitReaderPortable";

                // Search 'Foxit Reader' process and output log if found																									
                Process[] ps1 = Process.GetProcessesByName(PROCESS_NAME_READER);
                foreach (Process p1 in ps1)
                {
                    if (p1.ProcessName == PROCESS_NAME_READER)
                    {
                        String strMessage = "The process " + PROCESS_NAME_READER +
                                                " has already been executed : " + objectID;
                        WriteWindowLog(EventType.C_EVENT_TYPE_ERROR, strMessage,
                                       EventID.C_EVENT_ID_INTERNAL_ERROR);
                    }
                }

                // Search 'FoxitReaderPortable' process and output log if found																									
                Process[] ps2 = Process.GetProcessesByName(PROCESS_NAME_PORTABLE);
                foreach (Process p2 in ps2)
                {
                    if (p2.ProcessName == PROCESS_NAME_PORTABLE)
                    {
                        String strMessage = "The process " + PROCESS_NAME_PORTABLE +
                                                " has already been executed : " + objectID;
                        WriteWindowLog(EventType.C_EVENT_TYPE_ERROR, strMessage,
                                       EventID.C_EVENT_ID_INTERNAL_ERROR);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }	

    }
}
