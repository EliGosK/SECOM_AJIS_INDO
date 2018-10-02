
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
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Common;
using CSI.WindsorHelper;

using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.Presentation.Common.Models;


namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {
        private const string CMS100_Screen = "CMS100";

        /// <summary>
        /// Check permission for access screen CMS100
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CMS100_Authority(CMS100_ScreenParameter param) // old: string GroupCode
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                //Mock Data
                //GroupCode = "G0000024";

                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_VIEW_CUSTOMER_GROUP, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                if (CommonUtil.IsNullOrEmpty(param.GroupCode))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0040);
                    return Json(res);
                }

                IGroupMasterHandler hand = ServiceContainer.GetService<IGroupMasterHandler>() as IGroupMasterHandler;
                doGroup doGrp = new doGroup();
                doGrp.GroupCode = param.GroupCode;
                List<doGroup> lst = hand.GetGroup(doGrp);
                if (lst.Count <= 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                    return Json(res);
                }

                param.GroupData = lst[0];
                param.GroupSummaryList = new List<dtGroupSummaryForShow>();
                param.CustomerList = new List<dtCustomerListGrp>();
                param.SiteList = new List<dtsiteListGrp>();
                param.ContractList = new List<dtContractListGrp>();

                return InitialScreenEnvironment<CMS100_ScreenParameter>(CMS100_Screen, param, res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Method for return view of screen CMS100
        /// </summary>
        /// <returns></returns>
        [Initialize(CMS100_Screen)]
        public ActionResult CMS100()
        {
            CMS100_ScreenParameter CMS100Param = new CMS100_ScreenParameter();
            try
            {
                CMS100Param = GetScreenObject<CMS100_ScreenParameter>();
                ViewBag.GroupCode = CMS100Param.GroupData.GroupCode;
                ViewBag.GroupNameEN = CMS100Param.GroupData.GroupNameEN;
                ViewBag.GroupNameLC = CMS100Param.GroupData.GroupNameLC;
                ViewBag.Memo = CMS100Param.GroupData.Memo;
            }
            catch
            {
            }

            ViewBag.OfficeInCharge = CommonUtil.TextCodeName(CMS100Param.GroupData.GroupOfficeCode, CMS100Param.GroupData.OfficeNameEN);
            ViewBag.PersonInCharge = CommonUtil.TextCodeName(CMS100Param.GroupData.GroupEmpNo, CMS100Param.GroupData.GroupEmpName);
            ViewBag.PageRow = CommonValue.ROWS_PER_PAGE_FOR_VIEWPAGE;
            ViewBag.AlarmPrefix = ContractPrefix.C_CONTRACT_PREFIX_ALARM;
            ViewBag.MaintPrefix = ContractPrefix.C_CONTRACT_PREFIX_MAINTAINENCE;
            ViewBag.GuardPrefix = ContractPrefix.C_CONTRACT_PREFIX_GUARD;
            ViewBag.SalePrefix = ContractPrefix.C_CONTRACT_PREFIX_SALE;

            ViewBag.CustomerListHeader = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, ScreenID.C_SCREEN_ID_VIEW_CUSTOMER_GROUP, "lblHeaderCustomerList");
            ViewBag.SiteListHeader = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, ScreenID.C_SCREEN_ID_VIEW_CUSTOMER_GROUP, "lblHeaderSiteList");
            ViewBag.ContractListHeader = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, ScreenID.C_SCREEN_ID_VIEW_CUSTOMER_GROUP, "lblHeaderContractList");

            ViewBag.ContractTargetDisplay = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, ScreenID.C_SCREEN_ID_VIEW_CUSTOMER_GROUP, "headerContractTaget");
            ViewBag.RealCustomerDisplay = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, ScreenID.C_SCREEN_ID_VIEW_CUSTOMER_GROUP, "headerRealCustomer");
            ViewBag.AlarmDisplay = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, ScreenID.C_SCREEN_ID_VIEW_CUSTOMER_GROUP, "headerAlarm");
            ViewBag.MaintenanceDisplay = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, ScreenID.C_SCREEN_ID_VIEW_CUSTOMER_GROUP, "headerMaintenance");
            ViewBag.GuardDisplay = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, ScreenID.C_SCREEN_ID_VIEW_CUSTOMER_GROUP, "headerGuard");
            ViewBag.SaleDisplay = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, ScreenID.C_SCREEN_ID_VIEW_CUSTOMER_GROUP, "headerSale");

            ViewBag.RoleTypeContractTarget = CustRoleType.C_CUST_ROLE_TYPE_CONTRACT_TARGET;
            ViewBag.RoleTypePurchaser = CustRoleType.C_CUST_ROLE_TYPE_PURCHASER;
            ViewBag.RoleTypeRealCust = CustRoleType.C_CUST_ROLE_TYPE_REAL_CUST;

            return View(CMS100_Screen);
        }
 

        #region InitialGrid
        /// <summary>
        /// Initial grid of screen CMS030 (customer grid)
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS100_InitialCustomerGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS100_CustomerList", CommonUtil.GRID_EMPTY_TYPE.VIEW));
        }
        
        /// <summary>
        /// Initial grid of screen CMS030 (site grid)
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS100_InitialSiteGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS100_SiteList", CommonUtil.GRID_EMPTY_TYPE.VIEW));
        }
        
        /// <summary>
        /// Initial grid of screen CMS030 (contract grid)
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS100_InitialContractGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS100_ContractList", CommonUtil.GRID_EMPTY_TYPE.VIEW));
        }
        #endregion

        /// <summary>
        /// Get customer group summary data
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS100_LoadGroupSummary()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CMS100_ScreenParameter CMS100Param = GetScreenObject<CMS100_ScreenParameter>();
                IViewContractHandler hand = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
                ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> tmpCurrencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                        {
                                                            new doMiscTypeCode()
                                                            {
                                                                FieldName = MiscType.C_CURRENCT,
                                                                ValueCode = "%"
                                                            }
                                                        }).ToList();

                List<dtGroupSummaryForShow> lst = hand.GetGroupSummaryForViewCustGrpData(CMS100Param.GroupCode);

                res.ResultData = CommonUtil.ConvertToXml<dtGroupSummaryForShow>(lst, "Common\\CMS100_Gsum", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Get customer data for view
        /// </summary>
        /// <param name="type"></param>
        /// <param name="contractPrefix"></param>
        /// <returns></returns>
        public ActionResult CMS100_LoadCustomer(string type, string contractPrefix)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CMS100_ScreenParameter CMS100Param = GetScreenObject<CMS100_ScreenParameter>();
                IViewContractHandler hand = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
                List<dtCustomerListGrp> lst = new List<dtCustomerListGrp>();
                if (type.Contains("CT"))
                {
                    if (type.Contains("Rental"))
                    {
                        lst = hand.GetCustomerListForViewCustGrp_CT_Rental(CMS100Param.GroupCode, contractPrefix);
                    }
                    else
                    {
                        lst = hand.GetCustomerListForViewCustGrp_CT_Sale(CMS100Param.GroupCode, contractPrefix);
                    }
                }
                else
                {
                    if (type.Contains("Rental"))
                    {
                        lst = hand.GetCustomerListForViewCustGrp_R_Rental(CMS100Param.GroupCode, contractPrefix);
                    }
                    else
                    {
                        lst = hand.GetCustomerListForViewCustGrp_R_Sale(CMS100Param.GroupCode, contractPrefix);
                    }
                }
                res.ResultData = CommonUtil.ConvertToXml<dtCustomerListGrp>(lst, "Common\\CMS100_CustomerList", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Get site data for view
        /// </summary>
        /// <param name="type"></param>
        /// <param name="contractPrefix"></param>
        /// <returns></returns>
        public ActionResult CMS100_LoadSite(string type, string contractPrefix)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CMS100_ScreenParameter CMS100Param = GetScreenObject<CMS100_ScreenParameter>();
                IViewContractHandler hand = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
                List<dtsiteListGrp> lst = new List<dtsiteListGrp>();
                if (type.Contains("CT"))
                {
                    if (type.Contains("Rental"))
                    {
                        lst = hand.GetSiteListForViewCustGrp_CT_Rental(CMS100Param.GroupCode, contractPrefix);
                    }
                    else
                    {
                        lst = hand.GetSiteListForViewCustGrp_CT_Sale(CMS100Param.GroupCode, contractPrefix);
                    }
                }
                else
                {
                    if (type.Contains("Rental"))
                    {
                        lst = hand.GetSiteListForViewCustGrp_R_Rental(CMS100Param.GroupCode, contractPrefix);
                    }
                    else
                    {
                        lst = hand.GetSiteListForViewCustGrp_R_Sale(CMS100Param.GroupCode, contractPrefix);
                    }
                }
                res.ResultData = CommonUtil.ConvertToXml<dtsiteListGrp>(lst, "Common\\CMS100_SiteList", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Get contract data for view
        /// </summary>
        /// <param name="type"></param>
        /// <param name="contractPrefix"></param>
        /// <returns></returns>
        public ActionResult CMS100_LoadContract(string type, string contractPrefix)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CMS100_ScreenParameter CMS100Param = GetScreenObject<CMS100_ScreenParameter>();
                IViewContractHandler hand = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
                ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> tmpCurrencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                        {
                                                            new doMiscTypeCode()
                                                            {
                                                                FieldName = MiscType.C_CURRENCT,
                                                                ValueCode = "%"
                                                            }
                                                        }).ToList();

                List<dtContractListGrp> lst = new List<dtContractListGrp>();
                if (type.Contains("CT"))
                {
                    if (type.Contains("Rental"))
                    {
                        lst = hand.GetContractListForViewCustGrp_CT_Rental(CMS100Param.GroupCode, contractPrefix);
                    }
                    else
                    {
                        lst = hand.GetContractListForViewCustGrp_CT_Sale(CMS100Param.GroupCode, contractPrefix);
                    }
                }
                else
                {
                    if (type.Contains("Rental"))
                    {
                        lst = hand.GetContractListForViewCustGrp_R_Rental(CMS100Param.GroupCode, contractPrefix);
                    }
                    else
                    {
                        lst = hand.GetContractListForViewCustGrp_R_Sale(CMS100Param.GroupCode, contractPrefix);
                    }
                }
                //Add Currency
                for (int i = 0; i < lst.Count(); i++)
                {
                    lst[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                }

                res.ResultData = CommonUtil.ConvertToXml<dtContractListGrp>(lst, "Common\\CMS100_ContractList", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        //private string[] CMS100_GetResourceHeader()
        //{
        //    #region Get Current Language
        //    string lang = CommonUtil.GetCurrentLanguage();
        //    if (lang == SECOM_AJIS.Common.Util.ConstantValue.CommonValue.DEFAULT_LANGUAGE_EN)
        //        lang = string.Empty;
        //    else
        //        lang = "." + lang;

        //    string resourcePath = string.Format("{0}{1}\\{2}\\{3}{4}.resx",
        //                                        CommonUtil.WebPath,
        //                                        SECOM_AJIS.Common.Util.ConstantValue.CommonValue.APP_GLOBAL_RESOURCE_FOLDER,
        //                                        "Common",
        //                                        "CMS100",
        //                                        lang);
        //    XmlDocument rDoc = new XmlDocument();
        //    rDoc.Load(resourcePath);
        //    #endregion
        //    #region Set display value
        //    XmlNode rNodeContractTarget = rDoc.SelectSingleNode(string.Format("root/data[@name=\"{0}\"]/value", "headerContractTaget"));
        //    String strContractTargetDisplay = rNodeContractTarget.InnerText ?? "";

        //    XmlNode rNodeRealCustomer = rDoc.SelectSingleNode(string.Format("root/data[@name=\"{0}\"]/value", "headerRealCustomer"));
        //    String strRealCustomerDisplay = rNodeRealCustomer.InnerText ?? "";

        //    XmlNode rNodeAlarm = rDoc.SelectSingleNode(string.Format("root/data[@name=\"{0}\"]/value", "headerAlarm"));
        //    String strAlarmDisplay = rNodeAlarm.InnerText ?? "";

        //    XmlNode rNodeMaintenance = rDoc.SelectSingleNode(string.Format("root/data[@name=\"{0}\"]/value", "headerMaintenance"));
        //    String strMaintenanceDisplay = rNodeMaintenance.InnerText ?? "";

        //    XmlNode rNodeGuard = rDoc.SelectSingleNode(string.Format("root/data[@name=\"{0}\"]/value", "headerGuard"));
        //    String strGuardDisplay = rNodeGuard.InnerText ?? "";

        //    XmlNode rNodeSale = rDoc.SelectSingleNode(string.Format("root/data[@name=\"{0}\"]/value", "headerSale"));
        //    String strSaleDisplay = rNodeSale.InnerText ?? "";
        //    #endregion
        //    #region Set return value
        //    string[] result = new string[6];
        //    result[0] = strContractTargetDisplay;
        //    result[1] = strRealCustomerDisplay;
        //    result[2] = strAlarmDisplay;
        //    result[3] = strMaintenanceDisplay;
        //    result[4] = strGuardDisplay;
        //    result[5] = strSaleDisplay;

        //    return result;
        //    #endregion
        //}

        #region Old Code
        //public ActionResult CMS100()
        //{
        //    IGroupMasterHandler hand = ServiceContainer.GetService<IGroupMasterHandler>() as IGroupMasterHandler;
        //    List<doGroup> lst = hand.GetGroup(new doGroup { GroupCode = "G0000017", GroupName = "" });
        //    ViewBag.GroupCode = lst[0].GroupCode;
        //    ViewBag.GNameEn = lst[0].GroupNameEN;
        //    ViewBag.GNameLc = lst[0].GroupNameLC;
        //    ViewBag.Memo = lst[0].Memo;
        //    ViewBag.OfficeInCharge = lst[0].GroupOfficeCode + ": " + lst[0].OfficeNameEN;
        //    ViewBag.PersonInChange = lst[0].GroupEmpNo + ": " + lst[0].GroupNameEN;
        //    return View();
        //}
        //public ActionResult CMS100_InitGroupSumGrid()
        //{
        //    return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS100_Gsum"));
        //}
        //public ActionResult CMS100_InitCustListGrid()
        //{
        //    return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS100_CustomerList"));
        //}
        //public ActionResult CMS100_GetGroupSum()
        //{
        //    try
        //    {
        //        string SiteCode = Request["GroupCode"];
        //        IViewContractHandler hand = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
        //        List<dtGroupSummary> lst = hand.GetGroupSummaryForViewCustGrpData(SiteCode);

        //        //string xml = CommonUtil.ConvertToXml<dtGroupSummary>(lst);
        //        return Json(lst);
        //    }
        //    catch (Exception ex)
        //    {


        //        //MessageModel msg = MessageUtil.GetMessage(ex);
        //        //return Json(msg);
        //        return null;
        //    }

        //}
        //#region GetContract
        //public ActionResult CMS100_GetContlist_CT_Rental(doCMS100_ViewCustomerGroup getCont)
        //{

        //    try
        //    {
        //        if (getCont.GroupCode != null)
        //        {

        //            IViewContractHandler hand = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
        //            List<dtContractListGrp> lst = hand.GetContractListForViewCustGrp_CT_Rental(getCont.GroupCode, getCont.StrPreFix);
        //            List<View_dtContractListGrp> lst2 = new List<View_dtContractListGrp>();
        //            foreach (dtContractListGrp dt in lst)
        //            {
        //                lst2.Add(CommonUtil.CloneObject<dtContractListGrp, View_dtContractListGrp>(dt));
        //            }
        //            string xml = CommonUtil.ConvertToXml<View_dtContractListGrp>(lst2, "Common\\CMS100_ContractList");
        //            return Json(xml);
        //        }
        //        return Json(null);
        //    }
        //    catch (Exception ex)
        //    {
        //        //MessageModel msg = MessageUtil.GetMessage(ex);
        //        //return Json(msg);
        //        return null;
        //    }

        //}
        //public ActionResult CMS100_GetContlist_CT_Sale(doCMS100_ViewCustomerGroup getCont)
        //{

        //    try
        //    {
        //        if (getCont.GroupCode != null)
        //        {

        //            IViewContractHandler hand = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
        //            List<dtContractListGrp> lst = hand.GetContractListForViewCustGrp_CT_Sale(getCont.GroupCode, getCont.StrPreFix);
        //            List<View_dtContractListGrp> lst2 = new List<View_dtContractListGrp>();
        //            foreach (dtContractListGrp dt in lst)
        //            {
        //                lst2.Add(CommonUtil.CloneObject<dtContractListGrp, View_dtContractListGrp>(dt));
        //            }
        //            string xml = CommonUtil.ConvertToXml<View_dtContractListGrp>(lst2, "Common\\CMS100_ContractList");
        //            return Json(xml);
        //        }
        //        return Json(null);
        //    }
        //    catch (Exception ex)
        //    {
        //        //MessageModel msg = MessageUtil.GetMessage(ex);
        //        //return Json(msg);
        //        return null;
        //    }

        //}
        //public ActionResult CMS100_GetContlist_R_Rental(doCMS100_ViewCustomerGroup getCont)
        //{

        //    try
        //    {
        //        if (getCont.GroupCode != null)
        //        {

        //            IViewContractHandler hand = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
        //            List<dtContractListGrp> lst = hand.GetContractListForViewCustGrp_R_Rental(getCont.GroupCode, getCont.StrPreFix);
        //            List<View_dtContractListGrp> lst2 = new List<View_dtContractListGrp>();
        //            foreach (dtContractListGrp dt in lst)
        //            {
        //                lst2.Add(CommonUtil.CloneObject<dtContractListGrp, View_dtContractListGrp>(dt));
        //            }
        //            string xml = CommonUtil.ConvertToXml<View_dtContractListGrp>(lst2, "Common\\CMS100_ContractList");
        //            return Json(xml);
        //        }
        //        return Json(null);
        //    }
        //    catch (Exception ex)
        //    {
        //        //MessageModel msg = MessageUtil.GetMessage(ex);
        //        //return Json(msg);
        //        return null;
        //    }

        //}
        //public ActionResult CMS100_GetContlist_R_Sale(doCMS100_ViewCustomerGroup getCont)
        //{

        //    try
        //    {
        //        if (getCont.GroupCode != null)
        //        {

        //            IViewContractHandler hand = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
        //            List<dtContractListGrp> lst = hand.GetContractListForViewCustGrp_R_Sale(getCont.GroupCode, getCont.StrPreFix);
        //            List<View_dtContractListGrp> lst2 = new List<View_dtContractListGrp>();
        //            foreach (dtContractListGrp dt in lst)
        //            {
        //                lst2.Add(CommonUtil.CloneObject<dtContractListGrp, View_dtContractListGrp>(dt));
        //            }
        //            string xml = CommonUtil.ConvertToXml<View_dtContractListGrp>(lst2, "Common\\CMS100_ContractList");
        //            return Json(xml);
        //        }
        //        return Json(null);
        //    }
        //    catch (Exception ex)
        //    {
        //        //MessageModel msg = MessageUtil.GetMessage(ex);
        //        //return Json(msg);
        //        return null;
        //    }
        //}
        //#endregion

        //#region GetCustomer
        //public ActionResult CMS100_GetCustlist_CT_Rental(doCMS100_ViewCustomerGroup getCust)
        //{

        //    try
        //    {
        //        if (getCust.GroupCode != null)
        //        {

        //            IViewContractHandler hand = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
        //            List<dtCustomerListGrp> lst = hand.GetCustomerListForViewCustGrp_CT_Rental(getCust.GroupCode, getCust.StrPreFix);
        //            List<View_dtCustomerListGrp> lst2 = new List<View_dtCustomerListGrp>();
        //            foreach (dtCustomerListGrp dt in lst)
        //            {
        //                lst2.Add(CommonUtil.CloneObject<dtCustomerListGrp, View_dtCustomerListGrp>(dt));
        //            }
        //            string xml = CommonUtil.ConvertToXml<View_dtCustomerListGrp>(lst2, "Common\\CMS100_CustomerList");
        //            return Json(xml);
        //        }
        //        return Json(null);
        //    }
        //    catch (Exception ex)
        //    {
        //        //MessageModel msg = MessageUtil.GetMessage(ex);
        //        //return Json(msg);
        //        return null;
        //    }

        //}
        //public ActionResult CMS100_GetCustlist_CT_Sale(doCMS100_ViewCustomerGroup getCust)
        //{

        //    try
        //    {
        //        if (getCust.GroupCode != null)
        //        {

        //            IViewContractHandler hand = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
        //            List<dtCustomerListGrp> lst = hand.GetCustomerListForViewCustGrp_CT_Sale(getCust.GroupCode, getCust.StrPreFix);
        //            List<View_dtCustomerListGrp> lst2 = new List<View_dtCustomerListGrp>();
        //            foreach (dtCustomerListGrp dt in lst)
        //            {
        //                lst2.Add(CommonUtil.CloneObject<dtCustomerListGrp, View_dtCustomerListGrp>(dt));
        //            }
        //            string xml = CommonUtil.ConvertToXml<View_dtCustomerListGrp>(lst2, "Common\\CMS100_CustomerList");
        //            return Json(xml);
        //        }
        //        return Json(null);
        //    }
        //    catch (Exception ex)
        //    {
        //        //MessageModel msg = MessageUtil.GetMessage(ex);
        //        //return Json(msg);
        //        return null;
        //    }

        //}
        //public ActionResult CMS100_GetCustlist_R_Rental(doCMS100_ViewCustomerGroup getCust)
        //{

        //    try
        //    {
        //        if (getCust.GroupCode != null)
        //        {

        //            IViewContractHandler hand = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
        //            List<dtCustomerListGrp> lst = hand.GetCustomerListForViewCustGrp_R_Rental(getCust.GroupCode, getCust.StrPreFix);
        //            List<View_dtCustomerListGrp> lst2 = new List<View_dtCustomerListGrp>();
        //            foreach (dtCustomerListGrp dt in lst)
        //            {
        //                lst2.Add(CommonUtil.CloneObject<dtCustomerListGrp, View_dtCustomerListGrp>(dt));
        //            }
        //            string xml = CommonUtil.ConvertToXml<View_dtCustomerListGrp>(lst2, "Common\\CMS100_CustomerList");
        //            return Json(xml);
        //        }
        //        return Json(null);
        //    }
        //    catch (Exception ex)
        //    {
        //        //MessageModel msg = MessageUtil.GetMessage(ex);
        //        //return Json(msg);
        //        return null;
        //    }

        //}
        //public ActionResult CMS100_GetCustlist_R_Sale(doCMS100_ViewCustomerGroup getCust)
        //{

        //    try
        //    {
        //        if (getCust.GroupCode != null)
        //        {

        //            IViewContractHandler hand = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
        //            List<dtCustomerListGrp> lst = hand.GetCustomerListForViewCustGrp_R_Sale(getCust.GroupCode, getCust.StrPreFix);
        //            List<View_dtCustomerListGrp> lst2 = new List<View_dtCustomerListGrp>();
        //            foreach (dtCustomerListGrp dt in lst)
        //            {
        //                lst2.Add(CommonUtil.CloneObject<dtCustomerListGrp, View_dtCustomerListGrp>(dt));
        //            }
        //            string xml = CommonUtil.ConvertToXml<View_dtCustomerListGrp>(lst2, "Common\\CMS100_CustomerList");
        //            return Json(xml);
        //        }
        //        return Json(null);
        //    }
        //    catch (Exception ex)
        //    {
        //        //MessageModel msg = MessageUtil.GetMessage(ex);
        //        //return Json(msg);
        //        return null;
        //    }

        //}
        //#endregion

        //#region GetSite
        //public ActionResult CMS100_GetSitelist_CT_Rental(doCMS100_ViewCustomerGroup getSite)
        //{

        //    try
        //    {
        //        if (getSite.GroupCode != null)
        //        {

        //            IViewContractHandler hand = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
        //            List<dtsiteListGrp> lst = hand.GetSiteListForViewCustGrp_CT_Rental(getSite.GroupCode, getSite.StrPreFix);
        //            List<View_dtSiteListGrp> lst2 = new List<View_dtSiteListGrp>();
        //            foreach (dtsiteListGrp dt in lst)
        //            {
        //                lst2.Add(CommonUtil.CloneObject<dtsiteListGrp, View_dtSiteListGrp>(dt));
        //            }
        //            string xml = CommonUtil.ConvertToXml<View_dtSiteListGrp>(lst2, "Common\\CMS100_SiteList");
        //            return Json(xml);
        //        }
        //        return Json(null);
        //    }
        //    catch (Exception ex)
        //    {
        //        //MessageModel msg = MessageUtil.GetMessage(ex);
        //        //return Json(msg);
        //        return null;
        //    }
        //}
        //public ActionResult CMS100_GetSitelist_CT_Sale(doCMS100_ViewCustomerGroup getSite)
        //{

        //    try
        //    {
        //        if (getSite.GroupCode != null)
        //        {

        //            IViewContractHandler hand = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
        //            List<dtsiteListGrp> lst = hand.GetSiteListForViewCustGrp_CT_Sale(getSite.GroupCode, getSite.StrPreFix);
        //            List<View_dtSiteListGrp> lst2 = new List<View_dtSiteListGrp>();
        //            foreach (dtsiteListGrp dt in lst)
        //            {
        //                lst2.Add(CommonUtil.CloneObject<dtsiteListGrp, View_dtSiteListGrp>(dt));
        //            }
        //            string xml = CommonUtil.ConvertToXml<View_dtSiteListGrp>(lst2, "Common\\CMS100_SiteList");
        //            return Json(xml);
        //        }
        //        return Json(null);
        //    }
        //    catch (Exception ex)
        //    {
        //        //MessageModel msg = MessageUtil.GetMessage(ex);
        //        //return Json(msg);
        //        return null;
        //    }
        //}
        //public ActionResult CMS100_GetSitelist_R_Rental(doCMS100_ViewCustomerGroup getSite)
        //{

        //    try
        //    {
        //        if (getSite.GroupCode != null)
        //        {

        //            IViewContractHandler hand = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
        //            List<dtsiteListGrp> lst = hand.GetSiteListForViewCustGrp_R_Rental(getSite.GroupCode, getSite.StrPreFix);
        //            List<View_dtSiteListGrp> lst2 = new List<View_dtSiteListGrp>();
        //            foreach (dtsiteListGrp dt in lst)
        //            {
        //                lst2.Add(CommonUtil.CloneObject<dtsiteListGrp, View_dtSiteListGrp>(dt));
        //            }
        //            string xml = CommonUtil.ConvertToXml<View_dtSiteListGrp>(lst2, "Common\\CMS100_SiteList");
        //            return Json(xml);
        //        }
        //        return Json(null);
        //    }
        //    catch (Exception ex)
        //    {
        //        //MessageModel msg = MessageUtil.GetMessage(ex);
        //        //return Json(msg);
        //        return null;
        //    }
        //}
        //public ActionResult CMS100_GetSitelist_R_Sale(doCMS100_ViewCustomerGroup getSite)
        //{

        //    try
        //    {
        //        if (getSite.GroupCode != null)
        //        {

        //            IViewContractHandler hand = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
        //            List<dtsiteListGrp> lst = hand.GetSiteListForViewCustGrp_R_Sale(getSite.GroupCode, getSite.StrPreFix);
        //            List<View_dtSiteListGrp> lst2 = new List<View_dtSiteListGrp>();
        //            foreach (dtsiteListGrp dt in lst)
        //            {
        //                lst2.Add(CommonUtil.CloneObject<dtsiteListGrp, View_dtSiteListGrp>(dt));
        //            }
        //            string xml = CommonUtil.ConvertToXml<View_dtSiteListGrp>(lst2, "Common\\CMS100_SiteList");
        //            return Json(xml);
        //        }
        //        return Json(null);
        //    }
        //    catch (Exception ex)
        //    {
        //        //MessageModel msg = MessageUtil.GetMessage(ex);
        //        //return Json(msg);
        //        return null;
        //    }
        //}
        //#endregion

        //public ActionResult CMS100_Convert(string strSiteCode)
        //{

        //    CommonUtil Convert = new CommonUtil();
        //    string SiteCode = Convert.ConvertSiteCode(strSiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
        //    return Json(SiteCode);
        //}

        //public ActionResult CMS100_ConvertContractCode(string strContractCode)
        //{
        //    CommonUtil Convert = new CommonUtil();
        //    string ContractCode = Convert.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
        //    return Json(ContractCode);
        //}

        //public ActionResult CMS100_ConvertCustCode(string strCustomerCode)
        //{
        //    CommonUtil Convert = new CommonUtil();
        //    string ContractCode = Convert.ConvertCustCode(strCustomerCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
        //    return Json(ContractCode);
        //}
        #endregion
    }
}
