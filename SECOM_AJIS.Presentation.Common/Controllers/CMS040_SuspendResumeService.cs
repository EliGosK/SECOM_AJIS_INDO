//*********************************
// Create by: 
// Create date: /Jun/2010
// Update date: /Jun/2010
//*********************************


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Xml;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.ActionFilters;

using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;

using SECOM_AJIS.DataEntity.Common;
using CSI.WindsorHelper;
using SECOM_AJIS.Presentation.Common.Models;

namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {
        /// <summary>
        /// Check permission for access screen CMS040
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CMS040_Authority(CMS040_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_SUSPEND_RESUME, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<CMS040_ScreenParameter>("CMS040", param, res);
        }

        /// <summary>
        /// Method for return view of screen CMS040
        /// </summary>
        /// <returns></returns>
        [Initialize("CMS040")]
        public ActionResult CMS040()
        {
            try
            {
                ICommonHandler handler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                var list = handler.GetSystemStatus();

                //list[0].SuspendFlag
                //list[0].ResumeServiceDateTime
                //list[0].SuspendServiceDateTime



                string strOnline = CommonUtil.GetLabelFromResource("Common", "CMS040", "lblOnline"); ;
                string strOffline = CommonUtil.GetLabelFromResource("Common", "CMS040", "lblOffline"); 

                ViewBag.SystemStatus = (list[0].SuspendFlag == true) ? strOffline : strOnline;
                ViewBag.SystemStatusName = (list[0].SuspendFlag == true) ? "OFFLINE" : "ONLINE";
                ViewBag.NextResumeServiceDateTime = ((DateTime)list[0].ResumeServiceDateTime).ToString("dd-MMM-yyyy HH:mm:ss");
                ViewBag.NextSuspendServiceDateTime = ((DateTime)list[0].SuspendServiceDateTime).ToString("dd-MMM-yyyy HH:mm:ss");

                // for set default at combobox NextResumeServiceTime , NextSuspendServiceTime
                DateTime t = DateTime.Now;
                DateTime datNSPST = ((DateTime)list[0].SuspendServiceDateTime);
                DateTime datNRST = ((DateTime)list[0].ResumeServiceDateTime) ;
                
                DateTime datNextSuspendServiceTime = new DateTime(t.Year, t.Month, t.Day, datNSPST.Hour, datNSPST.Minute, 0);
                DateTime datNextResumeServiceTime = new DateTime(t.Year, t.Month, t.Day, datNRST.Hour, datNRST.Minute, 0);
                
                ViewBag.NextSuspendServiceTime = datNextSuspendServiceTime.ToString();
                ViewBag.NextResumeServiceTime = datNextResumeServiceTime.ToString();

                return View();
            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res);
            }

        }

        // Resume/Suspend Now

        /// <summary>
        /// Method for Resume/Suspend system immediately
        /// </summary>
        /// <param name="UpdateType"></param>
        /// <returns></returns>
        public ActionResult CMS040_UpdateSystemStatus(string UpdateType)
        {
            
            bool bResult = false;
            try
            {
                ICommonHandler handler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                if (UpdateType == "SUSPEND")
                {
                   bResult = handler.UpdateSystemStatus(true, true ,CommonUtil.dsTransData.dtUserData.EmpNo);
                }
                else if (UpdateType == "RESUME")
                {
                    bResult = handler.UpdateSystemStatus(false, true, CommonUtil.dsTransData.dtUserData.EmpNo);
                }


                List<doSystemStatus> l = new List<doSystemStatus>();
                doSystemStatus item = new doSystemStatus();

                var list = handler.GetSystemStatus();

                if (list.Count > 0)
                {



                    string strOnline = CommonUtil.GetLabelFromResource("Common", "CMS040", "lblOnline"); ;
                    string strOffline = CommonUtil.GetLabelFromResource("Common", "CMS040", "lblOffline"); 


                    item.CompleteFlag = bResult;
                    item.UpdateType = UpdateType;
                    item.SystemStatus = (list[0].SuspendFlag == true) ? "Offline" : "Online";
                    item.SystemStatusDisplayName = (list[0].SuspendFlag == true) ? strOffline : strOnline;
                    item.NextResumeServiceDateTime = ((DateTime)list[0].ResumeServiceDateTime).ToString("dd-MMM-yyyy HH:mm:ss");
                    item.NextSuspendServiceDateTime = ((DateTime)list[0].SuspendServiceDateTime).ToString("dd-MMM-yyyy HH:mm:ss");
                    l.Add(item);
                }

                return Json(l);
            }
            catch (Exception ex)
            {

                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res); 
            }

        }

    
        /// <summary>
        /// Method for update Resume/Suspend system schedule
        /// </summary>
        /// <param name="UpdateType"></param>
        /// <param name="ServiceUpdateTime"></param>
        /// <returns></returns>
        public ActionResult CMS040_UpdateSystemConfig(string UpdateType , DateTime ServiceUpdateTime)
        {
            
            string strServiceUpdateTime = ServiceUpdateTime.ToString("HH:mm"); // What's type of format ?? --> "HH:mm" 
            bool bResult = false;

            ObjectResultData res = new ObjectResultData();
            
            try
            {
                ICommonHandler handler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                var currentStatus= handler.GetSystemStatus();
                string strNextResumeTime = ((DateTime)currentStatus[0].ResumeServiceDateTime).ToString("HH:mm");
                string strNextSuspendTime = ((DateTime)currentStatus[0].SuspendServiceDateTime).ToString("HH:mm");

                if (UpdateType == "SUSPEND")
                {
                    if (strServiceUpdateTime == strNextResumeTime)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0029);
                        return Json(res);
                    }

                    bResult = handler.UpdateSystemConfig(ConfigName.C_CONFIG_SUSPEND_SERVICE_TIME, strServiceUpdateTime);
                }
                else if (UpdateType == "RESUME")
                {
                    if (strServiceUpdateTime == strNextSuspendTime)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0030);
                        return Json(res);
                    }

                    bResult = handler.UpdateSystemConfig(ConfigName.C_CONFIG_RESUME_SERVICE_TIME, strServiceUpdateTime);
                }

                List<doSystemStatus> l = new List<doSystemStatus>();
                doSystemStatus item = new doSystemStatus();

                var list = handler.GetSystemStatus();

                if (list.Count > 0)
                {
                    string strOnline = CommonUtil.GetLabelFromResource("Common", "CMS040", "lblOnline"); ;
                    string strOffline = CommonUtil.GetLabelFromResource("Common", "CMS040", "lblOffline"); 

                    item.CompleteFlag = bResult;
                    item.UpdateType = UpdateType;
                    item.SystemStatus = (list[0].SuspendFlag == true) ? "Offline" : "Online";
                    item.SystemStatusDisplayName = (list[0].SuspendFlag == true) ? strOffline : strOnline;
                    item.NextResumeServiceDateTime = ((DateTime)list[0].ResumeServiceDateTime).ToString("dd-MMM-yyyy HH:mm:ss");
                    item.NextSuspendServiceDateTime = ((DateTime)list[0].SuspendServiceDateTime).ToString("dd-MMM-yyyy HH:mm:ss");

                    
                    l.Add(item);
                }

                return Json(l);
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
                return Json(res);
            }

        }
    }
}
