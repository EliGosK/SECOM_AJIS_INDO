//*********************************
// Create by: Narupon W.
// Create date: /Jun/2011
// Update date: /Jun/2011
//*********************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Xml;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.ActionFilters;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Presentation.Common.Models;

using CSI.WindsorHelper;


namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {
        /// <summary>
        /// Check permission for access screen CMS240
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CMS240_Authority(CMS240_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_PURGE_LOG, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063);
                    return Json(res);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<CMS240_ScreenParameter>("CMS240", param, res);
        }

        /// <summary>
        /// Method for return view of screen CMS240
        /// </summary>
        /// <returns></returns>
        [Initialize("CMS240")]
        public ActionResult CMS240()
        {
            return View();
        }

        /// <summary>
        /// Initial grid of screen CMS240
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS240_InitialGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS240", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Get purge log status
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CMS240_GetStatus(CMS240_ScreenParameter cond)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            List<CMS240_PurgeLogDataDetail> listPurgeLogFailDetail = new List<CMS240_PurgeLogDataDetail>();

            CMS240_Status status = new CMS240_Status();

            try
            {
                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    if (res.IsError)
                        return Json(res);
                }

                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<DataEntity.Common.doSystemStatus> lstSystemStatus = hand.GetSystemStatus();
                bool bSuspendFlag = false;
                if (lstSystemStatus.Count > 0)
                {
                    bSuspendFlag = (lstSystemStatus[0].SuspendFlag == FlagType.C_FLAG_ON);
                }

                status.SuspendFlag = bSuspendFlag;
                status.MonthYear = cond.MonthYear;


                ILogHandler logHandler = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                List<dtTPL> lstLogHeader = logHandler.GetTbt_Purgelog(cond.MonthYear);


                List<dtMonthYear> lstLogMonthYear = logHandler.GetLogMonthYear();
                status.IsExistInTransLog = true;
                if (lstLogMonthYear.Count > 0)
                {
                    List<dtMonthYear> t = (from p in lstLogMonthYear where p.MonthYear.Value.CompareTo(cond.MonthYear.Value) == 0 
                                           select p).ToList<dtMonthYear>();

                    if (t.Count == 0) // 0 mean not exist
                    {
                        status.IsExistInTransLog =  false;
                    }

                    
                }


                if (lstLogHeader.Count > 0)
                {
                    //// Misc Mapping (#1)
                    //MiscTypeMappingList miscList = new MiscTypeMappingList();
                    //miscList.AddMiscType(lstLogHeader.ToArray());
                    //ICommonHandler comHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    //comHandler.MiscTypeMappingList(miscList);

                    // Misc Mapping (#2)
                    ICommonHandler handlerComm = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    List<doMiscTypeCode> MiscTypeCode = new List<doMiscTypeCode>();
                    List<string> lsFieldNames = new List<string>();
                    lsFieldNames.Add(MiscType.C_BATCH_STATUS);
                    List<doMiscTypeCode> MiscTypeList = handlerComm.GetMiscTypeCodeListByFieldName(lsFieldNames);

                    lstLogHeader[0].PurgeStatusName = handlerComm.GetMiscDisplayValue(MiscTypeList, MiscType.C_BATCH_STATUS, lstLogHeader[0].PurgeStatus);

                    // 1. Keep purge status , fail****
                    // 2. Create data list (PurgeLogDataDetail) from xml (column ErrorDescription)

                    bool isShowPurgeLogDataDetail = (lstLogHeader[0].PurgeStatus == BatchStatus.C_BATCH_STATUS_FAILED);

                    status.isShowPurgeLogDataDetail = isShowPurgeLogDataDetail;
                    status.PurgeStatusName = lstLogHeader[0].PurgeStatusName;
                    status.PurgeStatus = lstLogHeader[0].PurgeStatus;
                    status.xml = lstLogHeader[0].ErrorDescription;



                }

                UpdateScreenObject(status);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            res.ResultData = status;

            return Json(res);
        }

        /// <summary>
        /// Get purge log data
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS240_GetPurgeLogData()
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            List<CMS240_PurgeLogDataDetail> listPurgeLogFailDetail = new List<CMS240_PurgeLogDataDetail>();

            try
            {
                CMS240_Status cms240_stats_param = GetScreenObject<CMS240_Status>();

                // create data list from xml
                listPurgeLogFailDetail = CreatePurgeLogListFromXml(cms240_stats_param.xml);

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            res.ResultData = CommonUtil.ConvertToXml<CMS240_PurgeLogDataDetail>(listPurgeLogFailDetail, "Common\\CMS240", CommonUtil.GRID_EMPTY_TYPE.SEARCH);

            return Json(res);
        }

        /// <summary>
        /// Create purge log data list for xml format
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        private List<CMS240_PurgeLogDataDetail> CreatePurgeLogListFromXml(string xml)
        {
            List<CMS240_PurgeLogDataDetail> list = new List<CMS240_PurgeLogDataDetail>();
            CMS240_PurgeLogDataDetail detail = null;

            try
            {
                xml = string.Format("<PurgeData>{0}</PurgeData>", xml);
                XmlTextReader reader = new XmlTextReader(xml, XmlNodeType.Document, null);
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:

                            if (reader.HasAttributes) //filter only data element, exclude the root element
                            {
                                detail = new CMS240_PurgeLogDataDetail();

                                while (reader.MoveToNextAttribute())
                                {

                                    if (reader.Name == "TableName")
                                        detail.TableName = reader.Value;
                                    else
                                        detail.ErrorDescription = reader.Value;
                                }

                                list.Add(detail);
                            }
                            break;
                        case XmlNodeType.Text:
                            //do nothing
                            break;
                        case XmlNodeType.EndElement:
                            //do nothing
                            break;
                    }
                }
            }
            catch
            {
            }

            return list;
        }

        /// <summary>
        /// Perform purge log
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS240_PurgeLog()
        {
            ObjectResultData res = new ObjectResultData();

            try
            {

                CMS240_Status cms240_status_param = GetScreenObject<CMS240_Status>();

                //Write log for start purging log
                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteWindowLog(EventType.C_EVENT_TYPE_INFORMATION, "Purge log process is started", EventID.C_EVENT_ID_REPORT_PURGE_LOG);

                //Call CMP030: Purge log process 
                DateTime dtime = cms240_status_param.MonthYear.HasValue ? cms240_status_param.MonthYear.Value : DateTime.Now;
                List<tbt_PurgeLog> pl = hand.DeleteLog(cms240_status_param.MonthYear.Value);


                if (pl.Count > 0)
                {
                    if (pl[0].PurgeStatus == BatchStatus.C_BATCH_STATUS_SUCCEEDED)
                    {
                        cms240_status_param.IsPurgeSucceeded = true;
                        hand.WriteWindowLog(EventType.C_EVENT_TYPE_INFORMATION, "Purge log is finish",EventID.C_EVENT_ID_REPORT_PURGE_LOG);
                    }
                    else
                    {
                        cms240_status_param.IsPurgeSucceeded = false;
                        hand.WriteWindowLog(EventType.C_EVENT_TYPE_ERROR, "There are some error at purge log", EventID.C_EVENT_ID_REPORT_PURGE_LOG);
                    }


                    // update status
                    cms240_status_param.PurgeStatus = pl[0].PurgeStatus;
                    cms240_status_param.PurgeStatusName = "";
                  

                    // Misc Mapping (#2)
                    ICommonHandler handlerComm = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    List<doMiscTypeCode> MiscTypeCode = new List<doMiscTypeCode>();
                    List<string> lsFieldNames = new List<string>();
                    lsFieldNames.Add(MiscType.C_BATCH_STATUS);
                    List<doMiscTypeCode> MiscTypeList = handlerComm.GetMiscTypeCodeListByFieldName(lsFieldNames);

                    cms240_status_param.PurgeStatusName = handlerComm.GetMiscDisplayValue(MiscTypeList, MiscType.C_BATCH_STATUS, cms240_status_param.PurgeStatus);

                    
                }

                res.ResultData = cms240_status_param;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }


    }
}