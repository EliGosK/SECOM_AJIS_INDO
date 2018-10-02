//*********************************
// Create by: Nattapong N.
// Create date: 25/Jul/2010
// Update date: 28/Jul/2010
//*********************************


using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.Presentation.Common.Models;
using System.Xml;
using SECOM_AJIS.DataEntity.Billing;
namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {
        /// <summary>
        /// Initial screen Home screen (CMS020)
        /// </summary>
        /// <returns></returns>
        [Initialize("CMS020")]
        public ActionResult CMS020()
        {
            //Initial Section  
            if (CommonUtil.dsTransData == null)
                return View(CMS010());

            try
            {
                string resourcePath = string.Format("{0}\\Content\\{1}.xml",
                                                            CommonUtil.WebPath,
                                                            "News");
                XmlDocument rDoc = new XmlDocument();
                rDoc.Load(resourcePath);

                XmlNode rNode = rDoc.SelectSingleNode("News/Content");
                if (rNode != null)
                    @ViewBag.News = rNode.InnerXml;
            }
            catch (Exception)
            {
            }

            return View();
        }

        /// <summary>
        /// Keep history of current user.<br />
        /// Clear transaction data.
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            dsTransDataModel dsTrans = CommonUtil.dsTransData;
            if (dsTrans != null)
            {
                ILoginHandler handLogin = ServiceContainer.GetService<ILoginHandler>() as ILoginHandler;
                handLogin.KeepHistory(CommonUtil.dsTransData.dtUserData.EmpNo, LogType.C_LOG_OUT);
                CommonUtil.dsTransData = null;
            }

            return Json(true);
        }

        /// <summary>
        /// Change current language and reload screen with selected language.
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        public void changeLanguageDsTransData(string lang)
        {
            try
            {
                if (!CommonUtil.IsNullOrEmpty(lang))
                {
                    dsTransDataModel dstrans = CommonUtil.dsTransData;

                    lang = lang.ToLower();
                    if (lang == CommonValue.DEFAULT_SHORT_LANGUAGE_EN)
                        dstrans.dtTransHeader.Language = CommonValue.DEFAULT_LANGUAGE_EN;
                    else if (lang == CommonValue.DEFAULT_SHORT_LANGUAGE_JP)
                        dstrans.dtTransHeader.Language = CommonValue.DEFAULT_LANGUAGE_JP;
                    else
                        dstrans.dtTransHeader.Language = CommonValue.DEFAULT_LANGUAGE_LC;

                    //Update Language
                    CommonUtil.MappingObjectLanguage<OfficeDataDo>(dstrans.dtOfficeData);

                    CommonUtil.dsTransData = dstrans;

                    //Update Language for menu list
                    CommonUtil.MappingObjectLanguage<MenuName>(CommonUtil.MenuNameList);
                    Session.Remove("Menu");

                    ScreenParameter param = GetScreenObject<object>() as ScreenParameter;
                    if (param != null)
                    {
                        ScreenParameter nparam = ScreenParameter.ResetScreenParameter(param);
                        if (nparam != null)
                        {
                            nparam.IsLoaded = false;
                            nparam.BackStep = true;
                            UpdateScreenObject(nparam);
                        }
                    }
                }

            }
            catch (Exception)
            {
            }
        }

        //public ActionResult CMS020_SearchBar_Authority()
        //{
        //    return InitialScreenEnvironment("CMS020_SearchBar");

        //}

        ////[Initialize("CMS020_SearchBar")]
        //public ActionResult CMS020_SearchBar()
        //{
        //    return View();
        //}

        /// <summary>
        /// Retrieve contract data from tbt_RentalContractBasic or tbt_SaleBasic.
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strUserCode"></param>
        /// <returns></returns>
        private dsContractData CMS020_RetrieveContract(string strContractCode, string strUserCode)
        {
            try
            {
                CommonUtil cm = new CommonUtil();
                strContractCode = cm.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                dsContractData dsContractData = new dsContractData();
                IRentralContractHandler RentHand = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                List<tbt_RentalContractBasic> dtRCB = RentHand.GetTbt_RentalContractBasic(strContractCode, strUserCode);

                if (dtRCB.Count == 0)
                {
                    ISaleContractHandler SaleHand = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                    List<tbt_SaleBasic> dtSB = SaleHand.GetTbt_SaleBasic(strContractCode, null, FlagType.C_FLAG_ON);

                    if (dtSB.Count > 0)
                    {
                        dsContractData.dtSB = dtSB;
                        dsContractData.dtRCB = null;

                    }
                }
                else
                {
                    dsContractData.dtSB = null;
                    dsContractData.dtRCB = dtRCB;
                }

                return dsContractData;
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// If user input ContractCode or UserCode retrieve contract data from tbt_RentalContractBasic or tbt_SaleBasic.<br />
        /// Else if user input ProjectCode retrieve project data from tbt_Project.<br />
        /// Else if user input InvoiceNo get billing details of last invoice OCC.
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="UserCode"></param>
        /// <param name="InvoiceNo"></param>
        /// <param name="ProjectCode"></param>
        /// <returns></returns>
        public ActionResult CMS020_Retrive(string ContractCode, string UserCode, string InvoiceNo, string ProjectCode)
        {
            CommonUtil cm = new CommonUtil();
            dtSearchBarData data = new dtSearchBarData();
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG;

            List<string> list = new List<string>();
            if (CommonUtil.IsNullOrEmpty(ContractCode) == false)
                list.Add(ContractCode);
            if (CommonUtil.IsNullOrEmpty(UserCode) == false)
                list.Add(UserCode);
            if (CommonUtil.IsNullOrEmpty(InvoiceNo) == false)
                list.Add(InvoiceNo);
            if (CommonUtil.IsNullOrEmpty(ProjectCode) == false)
                list.Add(ProjectCode);

            if (list.Count > 1)
            {
                // Input more than one criteria --> Retrun message
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0117);
                return Json(res);
            }
            else if (list.Count == 0)
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);
                return Json(res);
            }

            try
            {
                // Case goto CMS190
                if (CommonUtil.IsNullOrEmpty(ContractCode) == false || CommonUtil.IsNullOrEmpty(UserCode) == false)
                {
                    dsContractData contractData = new dsContractData();
                    contractData = CMS020_RetrieveContract(ContractCode, UserCode);

                    if (contractData.dtRCB == null && contractData.dtSB == null)
                    {
                        // Return message MSG0102 // This contract code does not exist. 
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0133);
                        return Json(res);
                    }

                    bool isRCBNull = true;
                    if (contractData.dtRCB != null)
                    {
                        if (CommonUtil.IsNullOrEmpty(UserCode) == false && contractData.dtRCB.Count > 1)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0157);
                            return Json(res);
                        }
                        else if (contractData.dtRCB.Count > 0)
                        {
                            data.ContractCode = cm.ConvertContractCode(contractData.dtRCB[0].ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                            data.ServiceType = contractData.dtRCB[0].ServiceTypeCode;
                            data.Mode = "CMS190";

                            isRCBNull = false;
                        }

                    }
                    if (isRCBNull == true)
                    {
                        if (contractData.dtSB != null)
                        {
                            if (contractData.dtSB.Count > 0)
                            {
                                data.ContractCode = cm.ConvertContractCode(contractData.dtSB[0].ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                                data.ServiceType = contractData.dtSB[0].ServiceTypeCode;
                                data.Mode = "CMS190";
                            }
                        }
                    }

                    //if (data.ContractCode != null)
                    //    CommonUtil.dsTransData.dtCommonSearch.ContractCode = data.ContractCode;
                }
                else if (CommonUtil.IsNullOrEmpty(ProjectCode) == false)
                {
                    IProjectHandler pHandler = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
                    List<tbt_Project> lst = pHandler.GetTbt_Project(ProjectCode);
                    if (lst.Count == 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0091, new string[] { ProjectCode });
                        return Json(res);
                    }

                    data.ProjectCode = ProjectCode;
                    data.Mode = "CTS260";

                    //CommonUtil.dsTransData.dtCommonSearch.ProjectCode = ProjectCode;
                }
                else if (CommonUtil.IsNullOrEmpty(InvoiceNo) == false)
                {
                    IViewBillingHandler bHandler = ServiceContainer.GetService<IViewBillingHandler>() as IViewBillingHandler;
                    List<dtViewBillingDetailListOfLastInvoiceOCC> lst = bHandler.GetViewBillingDetailListOfLastInvoiceOCC(InvoiceNo, null, null, null, null);
                    if (lst.Count == 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0150, new string[] { InvoiceNo });
                        return Json(res);
                    }

                    data.InvoiceNo = InvoiceNo;
                    data.Mode = "CMS450";

                    //CommonUtil.dsTransData.dtCommonSearch.InvoiceNo = InvoiceNo;
                }


            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }


            return Json(data);

        }

    }
}
