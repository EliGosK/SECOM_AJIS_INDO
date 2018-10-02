//*********************************
// Create by: siripoj
// Create date: 3/Feb/2012
// Update date: 3/Feb/2012
//*****
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Transactions;

using CSI.WindsorHelper;

using SECOM_AJIS.DataEntity.Common;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.ActionFilters;

using SECOM_AJIS.DataEntity.Billing;
using SECOM_AJIS.Presentation.Billing.Models;

using SECOM_AJIS.DataEntity.Master;

namespace SECOM_AJIS.Presentation.Billing.Controllers
{
    public partial class BillingController : BaseController
    {
        #region Authority
        /// <summary>
        /// Check suspend, authority and resume 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult BLS010_Authority(BLS010_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                ///////// Check Suspending //////////
                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (chandler.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                /////// Check permission //////////
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_REGIST_BILL_TARGET, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }


            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<BLS010_ScreenParameter>("BLS010", param, res);
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initial screen BLS010
        /// </summary>
        /// <returns></returns>
        [Initialize("BLS010")]
        public ActionResult BLS010()
        {
            BLS010_ScreenParameter param = GetScreenObject<BLS010_ScreenParameter>();
            if (param != null)
            {
                //ViewBag.ContractProjectCode = param.ContractProjectCodeShort;
            }
            ViewBag.BillingOfficeCboValue = BillingOfficeWithSpecialCondition();
            return View();
        }

        #endregion

        #region Method
        /// <summary>
        /// Get billing office with special condition
        /// </summary>
        /// <returns></returns>
        public string BillingOfficeWithSpecialCondition()
        {
            List<tbm_Office> list = new List<tbm_Office>();
            try
            {
                IOfficeMasterHandler handler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
                // Get all office data
                list = handler.GetTbm_Office();
                List<tbm_Office> headOffice = (from p in list
                                               where p.OfficeCode == CommonUtil.dsTransData.dtUserData.MainOfficeCode
                                               select p).ToList<tbm_Office>();

                if (headOffice.Count > 0)
                {
                    //headOffice[0].OfficeLevel == 

                    // Comment by : Jirawat Jannet : 2016-08-16 
                    //if (headOffice[0].OfficeLevel == InventoryHeadOffice.C_OFFICELEVEL_HEAD) // if yes --> Authen cbo
                    //{
                    //    // Filter ==> BillingOfficeNormalCbo where ==> FunctionBilling <> C_FUNC_BILLING_NO
                    //    //list = (from p in list where p.FunctionBilling != FunctionBilling.C_FUNC_BILLING_NO select p).ToList<tbm_Office>();
                    //    return string.Empty;
                    //}
                    //else
                    //{
                    //    // list = headOffice;
                    //    return headOffice[0].OfficeCode;
                    //}
                    return headOffice[0].OfficeCode;
                }
                else
                {
                    list = new List<tbm_Office>();
                    return string.Empty;
                }

                // Language mappping
                //CommonUtil.MappingObjectLanguage<tbm_Office>(list);
            }
            catch
            {
                list = new List<tbm_Office>();
                return string.Empty;
            }
        }

        /// <summary>
        /// Set screen control
        /// </summary>
        /// <param name="doBillingClientData"></param>
        public void SetScreenControls(List<dtBillingClientData> doBillingClientData) 
        {
            //dtBillingClientData doBillingClient = doBillingClientData[0];

        }
        #endregion

        #region Event 
        /// <summary>
        /// Retrieve billing client data
        /// </summary>
        /// <param name="strBillingClientCode"></param>
        /// <returns></returns>
        public ActionResult BLS010_RetrieveData(string strBillingClientCode)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil comUtil = new CommonUtil();
            BLS010_ScreenParameter sParam = GetScreenObject<BLS010_ScreenParameter>();
            try
            {
                //Check mandatory
                if (String.IsNullOrEmpty(strBillingClientCode))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;                   
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                       ScreenID.C_SCREEN_ID_REGIST_BILL_TARGET,
                                       MessageUtil.MODULE_COMMON,
                                       MessageUtil.MessageList.MSG0007,
                                       new string[] { "lblBillingClientCode" },
                                         new string[] { "BillingClientCode" });
                    
                    return Json(res);
                }
               
                //Retrieve billing client data
                SECOM_AJIS.DataEntity.Master.IBillingMasterHandler handler = ServiceContainer.GetService<SECOM_AJIS.DataEntity.Master.IBillingMasterHandler>() as SECOM_AJIS.DataEntity.Master.IBillingMasterHandler;
                sParam.doBillingClientList = handler.GetBillingClient( comUtil.ConvertBillingClientCode(strBillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                
                //set data on screen
                if (sParam.doBillingClientList.Count > 0)
                {
                    sParam.doBillingClientList[0].BillingClientCode = comUtil.ConvertBillingClientCode(sParam.doBillingClientList[0].BillingClientCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    res.ResultData = sParam.doBillingClientList[0];
                    //SetScreenControls(sParam.doBillingClientList);
                }
                else
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;                    
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                      ScreenID.C_SCREEN_ID_REGIST_BILL_TARGET,
                                      MessageUtil.MODULE_MASTER,
                                      MessageUtil.MessageList.MSG1058,
                                      new string[] { "lblBillingClientCode" },
                                        new string[] { "BillingClientCode" });
                    return Json(res);
                }
               
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Return mapping language of billing client data
        /// </summary>
        /// <param name="doBillingClientData"></param>
        /// <returns></returns>
        public ActionResult BLS010_ReturnMappingLanguage(dtBillingClientData doBillingClientData)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil comUtil = new CommonUtil();
            BLS010_ScreenParameter sParam = GetScreenObject<BLS010_ScreenParameter>();
            try
            {
                if (doBillingClientData != null)
                {
                    List<dtBillingClientData> listBillingClientData = new List<dtBillingClientData>();
                    listBillingClientData.Add(doBillingClientData);
                    //listBillingClientData = base.GetBillingClient(MiscType.C_CUST_TYPE, billingClientCode);
                    CommonUtil.MappingObjectLanguage<dtBillingClientData>(listBillingClientData);
                    sParam.doBillingClientList = listBillingClientData;
                    res.ResultData = sParam.doBillingClientList[0];
                }                                               
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Get temp billing client for return billing client list
        /// </summary>
        /// <returns></returns>
        public ActionResult BLS010_GetTempBillingClientData()
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                BLS010_ScreenParameter sParam = GetScreenObject<BLS010_ScreenParameter>();
                if (sParam.doBillingClientList == null)
                {
                   // sParam.doBillingClientList = new List<dtBillingClientData>();
                    res.ResultData = null;
                    return Json(res);
                }
                if (sParam.doBillingClientList.Count > 0)
                {
                    res.ResultData = sParam.doBillingClientList[0];
                }
              
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Register billing target
        /// </summary>
        /// <param name="doTbtBillingTarget"></param>
        /// <param name="doBillingClientDdata"></param>
        /// <returns></returns>
        public ActionResult BLS010_RegisterData(tbt_BillingTarget doTbtBillingTarget,dtBillingClientData doBillingClientDdata)
        {
          
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            ValidatorUtil validator = new ValidatorUtil();
            CommonUtil comUtil = new CommonUtil();            
            try
            {
                BLS010_ScreenParameter sParam = GetScreenObject<BLS010_ScreenParameter>();
                BLS010_ScreenInputValidate obj = new BLS010_ScreenInputValidate();
                IBillingHandler handler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                List<string> lstParam = new List<string>();
                List<string> lstControl = new List<string>();

                ///////// Check Suspending //////////
                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (chandler.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }
              
                /////// Check permission //////////
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_REGIST_BILL_TARGET, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                //if (sParam.doBillingClientList == null)
                //{
                //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                //    //res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "lblBillingClientCode" }, new string[] { "lblBillingClientCode" });
                //    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                //                     ScreenID.C_SCREEN_ID_REGIST_BILL_TARGET,
                //                     MessageUtil.MODULE_COMMON,
                //                     MessageUtil.MessageList.MSG0007,
                //                     new string[] { "lblBillingClientCode" },
                //                      new string[] { "lblBillingClientCode" });
                //    return Json(res);
                //}              
                ////////////////5.2 Check mandatory
                //if (CommonUtil.IsNullOrEmpty(sParam.doBillingClientList[0].BillingClientCode))
                //{
                //    if (CommonUtil.IsNullOrEmpty(sParam.doBillingClientList[0].NameEN))
                //    {
                //        lstParam.Add("lblNameEnglish");
                //        lstControl.Add("FullNameEN");
                //    }
                //    if (CommonUtil.IsNullOrEmpty(sParam.doBillingClientList[0].NameLC))
                //    {
                //        lstParam.Add("lblNameLocal");
                //        lstControl.Add("FullNameEN");
                //    }
                //    if (CommonUtil.IsNullOrEmpty(sParam.doBillingClientList[0].CustTypeCode))
                //    {
                //        lstParam.Add("lblCustTypeCode");
                //        lstControl.Add("CustTypeName");
                        
                //    }
                //}

                // Modify by siripoj 06-06-12
                if (CommonUtil.IsNullOrEmpty(doBillingClientDdata.BillingClientCode))
                {
                    if (CommonUtil.IsNullOrEmpty(doBillingClientDdata.NameEN))
                    {
                        lstParam.Add("lblNameEnglish");
                        lstControl.Add("FullNameEN");
                    }
                    // 2017.02.15 delete matsuda start
                    //if (CommonUtil.IsNullOrEmpty(doBillingClientDdata.NameLC))
                    //{
                    //    lstParam.Add("lblNameLocal");
                    //    lstControl.Add("FullNameLC");
                    //}
                    // 2017.02.15 delete matsuda end
                    if (CommonUtil.IsNullOrEmpty(doBillingClientDdata.CustTypeCode))
                    {
                        lstParam.Add("lblCustTypeCode");
                        lstControl.Add("CustTypeName");

                    }
                }

                if (CommonUtil.IsNullOrEmpty(doTbtBillingTarget.BillingOfficeCode))
                {
                    lstParam.Add("lblBillingOffice");
                    lstControl.Add("BillingOfficeCode");
                }
                if (lstParam.Count > 0 && lstControl.Count>0)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                   // res.AddErrorMessage(MessageUtil.MODULE_BILLING, MessageUtil.MessageList.MSG0007, lstParam.ToArray(), lstControl.ToArray());
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                      ScreenID.C_SCREEN_ID_REGIST_BILL_TARGET,
                                      MessageUtil.MODULE_COMMON,
                                      MessageUtil.MessageList.MSG0007,
                                      lstParam.ToArray(),
                                       lstControl.ToArray());
                    return Json(res); 
                }

                //Clone Data 
                //obj = CommonUtil.CloneObject<dtBillingClientData, BLS010_ScreenInputValidate>(sParam.doBillingClientList[0]);
                //obj.BillingOfficeCode = doTbtBillingTarget.BillingOfficeCode;

                ////////////////5.2 Check mandatory
                //ValidatorUtil.BuildErrorMessage(res, this, new object[] { obj });
                //if (res.IsError)
                //{

                //    return Json(res);
                //}
                ///////////////5.3 Validate Business
                 //doTbtBillingTarget = CommonUtil.CloneObject<dtBillingClientData, tbt_BillingTarget>(sParam.doBillingClientList[0]);
                
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                
                // Modify by Siripoj 06-06-12
                //doTbtBillingTarget.BillingClientCode = sParam.doBillingClientList[0].BillingClientCode;
                doTbtBillingTarget.BillingClientCode = doBillingClientDdata.BillingClientCode;
                
                if (CommonUtil.IsNullOrEmpty(doTbtBillingTarget.IssueInvMonth))
                {
                    doTbtBillingTarget.IssueInvMonth = 0;
                }
                if (doTbtBillingTarget.IssueInvMonth > 60)
                {
                    
                    //res.AddErrorMessage(MessageUtil.MODULE_BILLING, MessageUtil.MessageList.MSG6001, new string[] { "IssueInvTime" }, new string[] { "IssueInvTime" });
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                      ScreenID.C_SCREEN_ID_REGIST_BILL_TARGET,
                                      MessageUtil.MODULE_BILLING,
                                      MessageUtil.MessageList.MSG6001,
                                      new string[] { "lblIssueInvoiceTiming" },
                                        new string[] { "IssueInvMonth" });
                    return Json(res);                    
                }
                if (!CommonUtil.IsNullOrEmpty(doTbtBillingTarget.BillingClientCode))
                {
                    List<tbt_BillingTarget> lst = new List<tbt_BillingTarget>();
                    lst = handler.GetTbt_BillingTarget(null, comUtil.ConvertBillingClientCode(doTbtBillingTarget.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG), doTbtBillingTarget.BillingOfficeCode);
                    if (lst.Count > 0)
                    {
                        doTbtBillingTarget = lst[0];
                    }
                    else
                    {
                        doTbtBillingTarget = null;
                    }
                    //doTbtBillingTarget = handler.GetTbt_BillingTarget(null, comUtil.ConvertBillingClientCode(doTbtBillingTarget.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG), doTbtBillingTarget.BillingOfficeCode);
                    if (doTbtBillingTarget != null)
                    {                                              
                        res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                    ScreenID.C_SCREEN_ID_REGIST_BILL_TARGET,
                                    MessageUtil.MODULE_BILLING,
                                    MessageUtil.MessageList.MSG6002,
                                    new string[] { comUtil.ConvertBillingTargetCode(doTbtBillingTarget.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT)},
                                      new string[] { "BillingClientCodeView" });
                        return Json(res);
                    }                   
                }
                res.ResultData = true;
                sParam.doBillingClient = doBillingClientDdata;
                sParam.doTbt_BillingTarget = doTbtBillingTarget;

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Set screen parameter of billing target information
        /// </summary>
        /// <param name="doBillingClientData"></param>
        /// <returns></returns>
        public ActionResult BLS010_SetScreenParameterBillingtargetInfo(dtBillingClientData doBillingClientData)
        {
            ObjectResultData res = new ObjectResultData();
            ValidatorUtil validator = new ValidatorUtil();
            try
            {
                BLS010_ScreenParameter sParam = GetScreenObject<BLS010_ScreenParameter>();
                if (sParam.doBillingClientList != null)
                {
                    sParam.doBillingClientList.Clear();
                    sParam.doBillingClientList.Add(doBillingClientData);  
                }
                                     
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Confirm billing target data
        /// </summary>
        /// <param name="doTbmBillingClient"></param>
        /// <param name="doTbtBillingTarget"></param>
        /// <returns></returns>
        public ActionResult BLS010_ConfirmBillingTargetData(SECOM_AJIS.DataEntity.Master.tbm_BillingClient doTbmBillingClient, tbt_BillingTarget doTbtBillingTarget)
        //public ActionResult BLS010_ConfirmBillingTargetData(BLS010_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil comUtil = new CommonUtil();
            IBillingHandler handler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
            SECOM_AJIS.DataEntity.Master.IBillingMasterHandler handlerMaster = ServiceContainer.GetService<SECOM_AJIS.DataEntity.Master.IBillingMasterHandler>() as SECOM_AJIS.DataEntity.Master.IBillingMasterHandler;
            string strBillingClientCode = null;
            string strBillingTargetCode = null;

            BLS010_ScreenParameter sParam = GetScreenObject<BLS010_ScreenParameter>();
           // tbm_BillingClient doTbmBillingClient = param.doTbmBillingClientParam;
           // tbt_BillingTarget doTbtBillingTarget = param.doTbt_BillingTarget;
            try
            {

                
                ///////// Check Suspending //////////
                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (chandler.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }


                /////// Check permission //////////
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_REGIST_BILL_TARGET, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                //clone tbtBillingClientData to tbm_billingcliebtData
                doTbmBillingClient = CommonUtil.CloneObject<SECOM_AJIS.DataEntity.Master.dtBillingClientData, SECOM_AJIS.DataEntity.Master.tbm_BillingClient>(sParam.doBillingClient);
                doTbmBillingClient.BillingClientCode = comUtil.ConvertBillingClientCode(doTbmBillingClient.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                doTbtBillingTarget.BillingClientCode = comUtil.ConvertBillingClientCode(doTbtBillingTarget.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                
                //validatebusiness
                tbt_BillingTarget tmpTbtBillingTarget = new tbt_BillingTarget();
                if (!CommonUtil.IsNullOrEmpty(doTbmBillingClient.BillingClientCode))
                {
                    List<tbt_BillingTarget> lst = new List<tbt_BillingTarget>();
                    lst = handler.GetTbt_BillingTarget(null, doTbmBillingClient.BillingClientCode, doTbtBillingTarget.BillingOfficeCode);
                    if (lst.Count > 0)
                    {
                        tmpTbtBillingTarget = lst[0];
                    }
                    else
                    {
                        tmpTbtBillingTarget = null;
                    }
                    

                    if (tmpTbtBillingTarget != null)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                       
                        res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                  ScreenID.C_SCREEN_ID_REGIST_BILL_TARGET,
                                  MessageUtil.MODULE_BILLING,
                                  MessageUtil.MessageList.MSG6002,
                                  new string[] { comUtil.ConvertBillingTargetCode( tmpTbtBillingTarget.BillingTargetCode ,CommonUtil.CONVERT_TYPE.TO_SHORT) },
                                    null);
                        return Json(res);
                    }    
                }


                var tmpdoTbmBillingClient = CommonUtil.CloneObject<tbm_BillingClient, tbm_BillingClient>(doTbmBillingClient);
                var tmpdoTbtBillingTarget = CommonUtil.CloneObject<tbt_BillingTarget, tbt_BillingTarget>(doTbtBillingTarget);

                //register new billing target
                using (TransactionScope scope = new TransactionScope())
                {

                //7.3.4
                if (CommonUtil.IsNullOrEmpty(tmpdoTbtBillingTarget.BillingClientCode))
                    {
                        strBillingClientCode = handlerMaster.ManageBillingClient(tmpdoTbmBillingClient);
                        tmpdoTbtBillingTarget.BillingClientCode = strBillingClientCode;
                    }

                    //7.3.5 create billing target
                    strBillingTargetCode = handler.CreateBillingTarget(tmpdoTbtBillingTarget);
                    tmpdoTbtBillingTarget.BillingTargetCode = comUtil.ConvertBillingClientCode(strBillingTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    tmpdoTbmBillingClient.BillingClientCode = comUtil.ConvertBillingClientCode(tmpdoTbmBillingClient.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                scope.Complete();

                doTbmBillingClient = CommonUtil.CloneObject<tbm_BillingClient, tbm_BillingClient>(tmpdoTbmBillingClient);
                    doTbtBillingTarget = CommonUtil.CloneObject<tbt_BillingTarget, tbt_BillingTarget>(tmpdoTbtBillingTarget);
                }
                doTbtBillingTarget.BillingClientCode = comUtil.ConvertBillingClientCode(doTbtBillingTarget.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                sParam.doTbt_BillingTarget = doTbtBillingTarget;
                res.ResultData = doTbtBillingTarget;
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION_OK;
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0046, null, null);
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Reset session of billing client list and billing target
        /// </summary>
        /// <returns></returns>
        public ActionResult BLS010_ResetSession()
        {
            ObjectResultData res = new ObjectResultData();
            
            try
            {
                BLS010_ScreenParameter sParam = GetScreenObject<BLS010_ScreenParameter>();
                sParam.doBillingClientList = null;
                sParam.doTbt_BillingTarget = null;
                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        #endregion



    }
}
