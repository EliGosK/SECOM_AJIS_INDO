//*********************************
// Create by: Natthavat S.
// Create date: 17/Oct/2011
// Update date: 17/Oct/2011
//*********************************

using System;
using System.Web;
using System.Web.Mvc;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.DataEntity.Contract;
using System.Data.Objects;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Models;
using CSI.WindsorHelper;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Presentation.Contract.Models;
using SECOM_AJIS.DataEntity.Quotation;
using System.Transactions;
using SECOM_AJIS.DataEntity.Master;
namespace SECOM_AJIS.Presentation.Contract.Controllers
{
    public partial class ContractController : BaseController
    {
        #region Authority
        /// <summary>
        /// Check user’s permission
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CTS370_Authority(CTS370_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil comUtil = new CommonUtil();

            try
            {
                res = ValidateAuthority_CTS370(res);
                if (res.IsError)
                {
                    return Json(res);
                }

                //if ((param != null) && (!String.IsNullOrEmpty(param.strARRelevantCode)) && (!String.IsNullOrEmpty(param.strARRelevantType)))
                //{
                //    var cond = CreateCondition_CTS370(param);
                //    doRetrieveARListCondition rawCond = new doRetrieveARListCondition()
                //    {
                //        ARRelevantType = param.strARRelevantCode,
                //        ContractCode = cond.ContractCode,
                //        CustomerCode = cond.CustomerCode,
                //        SiteCode = cond.SiteCode,
                //        ProjectCode = cond.ProjectCode,
                //        DuedateDeadline = ((cond.DuedateDeadline == ARSearchDueDate.C_AR_SEARCH_DUEDATE_1MONTH) ? DateTime.Now.AddMonths(1) :
                //            ((cond.DuedateDeadline == ARSearchDueDate.C_AR_SEARCH_DUEDATE_2WEEKS) ? DateTime.Now.AddDays(14) :
                //            ((cond.DuedateDeadline == ARSearchDueDate.C_AR_SEARCH_DUEDATE_1WEEK) ? DateTime.Now.AddDays(7) : DateTime.Now))),
                //        ARStatus = cond.ARStatus,
                //        ARType = cond.ARType,
                //        QuotationCode = cond.QuotationCode
                //    };
                //    List<dtARListCTS370> rawDat;
                //    var condRes = RetrieveAR_CTS370(rawCond, out rawDat);
                //    if (condRes.Count == 0)
                //    {
                //        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                //        return Json(res);
                //    }
                //}

                //============================ CHECK AUTHORITY =====================================                
                ICustomerMasterHandler customerhandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
                ISiteMasterHandler sitehandler = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;
                ICommonContractHandler commoncontracthandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
                IProjectHandler projecthandler = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
                ISaleContractHandler salehandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                IRentralContractHandler rentalhandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                IQuotationHandler quotehandler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
                CommonUtil util = new CommonUtil();
                if (param.strARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_CUSTOMER)
                {
                    if (CommonUtil.IsNullOrEmpty(param.strARRelevantCode))
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006, new string[] { param.strARRelevantCode }, new string[] { "txtCustomerCode" });
                    }
                    else if (!customerhandler.CheckExistCustomerData(util.ConvertCustCode(param.strARRelevantCode, CommonUtil.CONVERT_TYPE.TO_LONG)))
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { param.strARRelevantCode }, new string[] { "txtCustomerCode" });
                    }
                    
                }
                else if (param.strARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_SITE)
                {
                    if (CommonUtil.IsNullOrEmpty(param.strARRelevantCode))
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006, new string[] { param.strARRelevantCode }, new string[] { "txtSiteCode" });
                    }
                    else if (!sitehandler.CheckExistSiteData(util.ConvertSiteCode(param.strARRelevantCode, CommonUtil.CONVERT_TYPE.TO_LONG), null))
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { param.strARRelevantCode }, new string[] { "txtSiteCode" });
                    }
                    
                }
                else if (param.strARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_CONTRACT)
                {
                    if (CommonUtil.IsNullOrEmpty(param.strARRelevantCode))
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006, new string[] { param.strARRelevantCode }, new string[] { "txtContractCode" });
                    }
                    else if (!commoncontracthandler.IsContractExistInRentalOrSale(util.ConvertContractCode(param.strARRelevantCode, CommonUtil.CONVERT_TYPE.TO_LONG)))
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { param.strARRelevantCode }, new string[] { "txtContractCode" });
                    }
                   
                }
                else if (param.strARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_PROJECT)
                {
                    var sRes = projecthandler.IsProjectExist((param.strARRelevantCode));

                    if (CommonUtil.IsNullOrEmpty(param.strARRelevantCode))
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006, new string[] { param.strARRelevantCode }, new string[] { "txtProjectCode" });
                    }
                    else if ((sRes.Count < 1) || !sRes[0].GetValueOrDefault())
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { param.strARRelevantCode }, new string[] { "txtProjectCode" });
                    }
                    
                }
                else if (param.strARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_QUOTATION)
                {

                    if (CommonUtil.IsNullOrEmpty(param.strARRelevantCode))
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006, null, new string[] { "txtQuotationCode" });
                    }
                    var qRes = quotehandler.GetTbt_QuotationTarget(new doGetQuotationDataCondition()
                    {
                        QuotationTargetCode = util.ConvertQuotationTargetCode(param.strARRelevantCode, CommonUtil.CONVERT_TYPE.TO_LONG)
                    });

                    if (qRes == null || qRes.Count == 0)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { param.strARRelevantCode }, new string[] { "txtQuotationCode" });
                    }
                }

                //==================================================================================
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<CTS370_ScreenParameter>("CTS370", param, res);
        }
        

        //public ActionResult CTS370_Authority(string strContractCode, string strARRelevantCode, string strARRelevantType)
        //{
        //    ObjectResultData res = new ObjectResultData();
        //    CommonUtil comUtil = new CommonUtil();
        //    CTS370_ScreenParameter param = null;

        //    try
        //    {
        //        // Check Permission
        //        if (!CheckUserPermission(ScreenID.C_SCREEN_ID_AR_LIST))
        //        {
        //            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
        //            return Json(res);
        //        }

        //        param = new CTS370_ScreenParameter()
        //        {
        //            strARRelevantCode = strARRelevantCode,
        //            strARRelevantType = strARRelevantType
        //        };

        //        //SetScreenParameter_CTS370(param);
        //    }
        //    catch (Exception ex)
        //    {
        //        res.AddErrorMessage(ex);
        //        return Json(res);
        //    }

        //    return InitialScreenEnvironment("CTS370", param);
        //}

        #endregion

        #region Action

        /// <summary>
        /// Initial screen
        /// </summary>
        /// <returns></returns>
        [Initialize("CTS370")]
        public ActionResult CTS370() // InitialState
        {
            return View();
        }

        /// <summary>
        /// Initial AR list's grid schema
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS370_InitialGridAR()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Contract\\CTS370_AR"));
        }

        /// <summary>
        /// Initial AR occurring site's grid schema
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS370_InitialGridOccurringSite()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Contract\\CTS370_OccurringSite"));
        }

        /// <summary>
        /// Initial AR occurring contract's grid schema
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS370_InitialGridOccurringContract()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Contract\\CTS370_OccurringContract"));
        }

        /// <summary>
        /// Initial screen default value
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS370_RetrieveInitialDefaultValue()
        {
            ObjectResultData result = new ObjectResultData();
            CTS370_DefaultValue res = new CTS370_DefaultValue();

            //res.DueDate = ARSearchDuedate.C_AR_SEARCH_DUEDATE_2WEEKS;
            res.DueDate = null;
            res.Status = ARSearchStatus.C_AR_SEARCH_STATUS_HANDLING;
            result.ResultData = res;

            return Json(result);
        }

        /// <summary>
        /// Checking parameter from another screen
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS370_CheckParameter()
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                CTS370_ScreenParameter sParam = GetScreenObject<CTS370_ScreenParameter>();
                //CTS370_ScreenParameter sParam = GetScreenObject_CTS370();

                if ((sParam != null) && (!String.IsNullOrEmpty(sParam.strARRelevantCode)) && (!String.IsNullOrEmpty(sParam.strARRelevantType)))
                {
                    res.ResultData = CreateCondition_CTS370(sParam, false);
                }
                //else
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001, null, null);
                //}
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Validate customer data and retrieve customer code in long version
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CTS370_RetrieveCustomerCode(CTS370_GetCodeCondition param)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                ISiteMasterHandler sitehandler = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;
                IRentralContractHandler rentralcontracthandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                ISaleContractHandler salehandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;

                CommonUtil util = new CommonUtil();

                if (!String.IsNullOrEmpty(param.SiteCode))
                {
                    doSiteSearchCondition sitecond = new doSiteSearchCondition()
                    {
                        SiteCode = util.ConvertSiteCode(param.SiteCode, CommonUtil.CONVERT_TYPE.TO_LONG)
                    };

                    var siteRes = sitehandler.GetSiteDataForSearch(sitecond);

                    if (siteRes.Count == 1)
                    {
                        res.ResultData = util.ConvertCustCode(siteRes[0].CustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    }
                    else
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001, null, null);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    }
                }
                else if (!(String.IsNullOrEmpty(param.ContractCode)))
                {
                    //var contractRentalRes = rentralcontracthandler.GetRentalContractBasicInformation(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                    var contractRentalRes = rentralcontracthandler.GetTbt_RentalContractBasic(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), null);

                    if (contractRentalRes.Count == 1)
                    {
                        res.ResultData = util.ConvertCustCode(contractRentalRes[0].ContractTargetCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    }
                    else
                    {
                        var contractSaleBasic = salehandler.GetTbt_SaleBasic(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), null, true);

                        if (contractSaleBasic.Count == 1)
                        {
                            res.ResultData = util.ConvertCustCode(contractSaleBasic[0].RealCustomerCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        }
                        else
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001, null, null);
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        }

                        //string lastOCC = salehandler.GetLastOCC(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                        //var contractSaleBasic = salehandler.GetSaleContractData(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), lastOCC);

                        //if ((contractSaleBasic != null) && (contractSaleBasic.dtTbt_SaleBasic != null))
                        //{
                        //    res.ResultData = util.ConvertCustCode(contractSaleBasic.dtTbt_SaleBasic.RealCustomerCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        //}
                        //else
                        //{
                        //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001, null, null);
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Validate site data and retrieve site code in long version
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CTS370_RetrieveSiteCode(CTS370_GetCodeCondition param)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                ISiteMasterHandler sitehandler = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;
                IRentralContractHandler rentralcontracthandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                ISaleContractHandler salehandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;

                CommonUtil util = new CommonUtil();

                if (!(String.IsNullOrEmpty(param.ContractCode)))
                {
                    var contractRentalRes = rentralcontracthandler.GetRentalContractBasicInformation(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG));

                    if (contractRentalRes.Count == 1)
                    {
                        res.ResultData = util.ConvertSiteCode(contractRentalRes[0].SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    }
                    else
                    {
                        string lastOCC = salehandler.GetLastOCC(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                        var contractSaleBasic = salehandler.GetSaleContractData(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), lastOCC);

                        if ((contractSaleBasic != null) && (contractSaleBasic.dtTbt_SaleBasic != null))
                        {
                            res.ResultData = util.ConvertSiteCode(contractSaleBasic.dtTbt_SaleBasic.SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        }
                        else
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001, null, null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Retrieve AR list grid header (code: name)
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CTS370_RetrieveGridHeader(CTS370_ARCondition param)
        {
            ObjectResultData res = new ObjectResultData();
            CTS370_GridHeader obj = new CTS370_GridHeader();
            ICustomerMasterHandler customerhandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
            ISiteMasterHandler sitehandler = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;
            ICommonContractHandler commoncontracthandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
            IProjectHandler projecthandler = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
            ISaleContractHandler salehandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
            IRentralContractHandler rentalhandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            IQuotationHandler quotehandler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;

            try
            {
                doRetrieveARListCondition doSearchcond = CreateCondition_CTS370(param);
                bool isValid = true;

                if (doSearchcond.ARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_CUSTOMER)
                {
                    if (CommonUtil.IsNullOrEmpty(doSearchcond.CustomerCode))
                    {
                        isValid = false;
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006, new string[] { param.CustomerCode }, new string[] { "txtCustomerCode" });
                    }
                    else if (!customerhandler.CheckExistCustomerData(doSearchcond.CustomerCode))
                    {
                        isValid = false;
                        //res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { param.CustomerCode }, new string[] { "txtCustomerCode" });
                    }
                    else
                    {
                        var dataLst = (from a in customerhandler.GetCustomer(doSearchcond.CustomerCode)
                                       select new CTS370_GridHeader()
                                       {
                                           Code = param.CustomerCode
                                           ,
                                           NameEN = a.CustFullNameEN
                                           ,
                                           NameJP = a.CustFullNameEN
                                           ,
                                           NameLC = a.CustFullNameLC
                                       }).ToList();

                        CommonUtil.MappingObjectLanguage<CTS370_GridHeader>(dataLst);

                        if (dataLst.Count == 1)
                        {
                            obj = dataLst[0];
                        }
                    }
                }
                else if (doSearchcond.ARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_SITE)
                {
                    if (CommonUtil.IsNullOrEmpty(doSearchcond.SiteCode))
                    {
                        isValid = false;
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006, new string[] { param.SiteCode }, new string[] { "txtSiteCode" });
                    }
                    else if (!sitehandler.CheckExistSiteData(doSearchcond.SiteCode, null))
                    {
                        isValid = false;
                        //res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { param.SiteCode }, new string[] { "txtSiteCode" });
                    }
                    else
                    {
                        var dataLst = (from a in sitehandler.GetSite(doSearchcond.SiteCode, null)
                                       select new CTS370_GridHeader()
                                       {
                                           Code = param.SiteCode
                                           ,
                                           NameEN = a.SiteNameEN
                                           ,
                                           NameJP = a.SiteNameEN
                                           ,
                                           NameLC = a.SiteNameLC
                                       }).ToList();

                        CommonUtil.MappingObjectLanguage<CTS370_GridHeader>(dataLst);

                        if (dataLst.Count == 1)
                        {
                            obj = dataLst[0];
                        }
                    }
                }
                else if (doSearchcond.ARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_CONTRACT)
                {
                    if (CommonUtil.IsNullOrEmpty(doSearchcond.ContractCode))
                    {
                        isValid = false;
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006, new string[] { param.ContractCode }, new string[] { "txtContractCode" });
                    }
                    else if ((!commoncontracthandler.IsContractExistInRentalOrSale(doSearchcond.ContractCode)))
                    {
                        isValid = false;
                        //res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { param.ContractCode }, new string[] { "txtContractCode" });
                    }
                    else
                    {
                        List<CTS370_GridHeader> dataLst = new List<CTS370_GridHeader>();

                        var saleLst = salehandler.GetTbt_SaleBasicForView(doSearchcond.ContractCode, null, null);

                        string trueContractCode = "";
                        var rentalLst_Code = rentalhandler.GetTbt_RentalContractBasic(doSearchcond.ContractCode, null);

                        if (rentalLst_Code.Count == 1)
                        {
                            trueContractCode = rentalLst_Code[0].ContractCode;
                        }

                        var rentalLst = rentalhandler.GetTbt_RentalContractBasicForView(trueContractCode);

                        if (saleLst.Count > 0)
                        {
                            dataLst = (from a in saleLst
                                       select new CTS370_GridHeader()
                                       {
                                           Code = param.ContractCode,
                                           NameEN = a.PurCust_CustFullNameEN,
                                           NameJP = a.PurCust_CustFullNameEN,
                                           NameLC = a.PurCust_CustFullNameLC
                                       }).ToList();
                            CommonUtil.MappingObjectLanguage<CTS370_GridHeader>(dataLst);
                        }
                        else if (rentalLst.Count > 0)
                        {
                            dataLst = (from a in rentalLst
                                       select new CTS370_GridHeader()
                                       {
                                           Code = param.ContractCode,
                                           NameEN = a.CustFullNameEN_Cust,
                                           NameJP = a.CustFullNameEN_Cust,
                                           NameLC = a.CustFullNameLC_Cust
                                       }).ToList();
                            CommonUtil.MappingObjectLanguage<CTS370_GridHeader>(dataLst);
                        }

                        if (dataLst.Count == 1)
                        {
                            obj = dataLst[0];
                        }
                    }
                }
                else if (doSearchcond.ARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_PROJECT)
                {
                    var sRes = projecthandler.IsProjectExist(doSearchcond.ProjectCode);

                    if (CommonUtil.IsNullOrEmpty(doSearchcond.ProjectCode))
                    {
                        isValid = false;
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006, new string[] { param.ProjectCode }, new string[] { "txtProjectCode" });
                    }
                    else if ((sRes.Count < 1) || !sRes[0].GetValueOrDefault())
                    {
                        isValid = false;
                        //res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { param.ProjectCode }, new string[] { "txtProjectCode" });
                    }
                    else
                    {
                        var dataLst = (from a in projecthandler.GetTbt_Project(doSearchcond.ProjectCode)
                                       select new CTS370_GridHeader()
                                       {
                                           Code = param.ProjectCode,
                                           Name = a.ProjectName
                                       }).ToList();

                        if (dataLst.Count == 1)
                        {
                            obj = dataLst[0];
                        }
                    }
                }
                else if (doSearchcond.ARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_QUOTATION)
                {

                    if (CommonUtil.IsNullOrEmpty(doSearchcond.QuotationCode))
                    {
                        isValid = false;
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006, null, new string[] { "txtQuotationCode" });
                    }
                    else
                    {
                        //var qRes = quotehandler.GetQuotationData(new doGetQuotationDataCondition()
                        //    {
                        //        QuotationTargetCode = doSearchcond.QuotationCode
                        //    });

                        var qRes = quotehandler.GetTbt_QuotationTarget(new doGetQuotationDataCondition()
                        {
                            QuotationTargetCode = doSearchcond.QuotationCode
                        });

                        //if ((qRes == null) || (qRes.dtTbt_QuotationCustomer == null) || (qRes.dtTbt_QuotationCustomer.Count < 1))
                        if (qRes == null || qRes.Count == 0)
                        {
                            isValid = false;
                            //5res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { param.QuotationCode }, new string[] { "txtQuotationCode" });
                        }
                        else
                        {
                            //var qRes2 = quotehandler.GetTbt_QuotationCustomer(new doGetQuotationDataCondition()
                            //{
                            //    QuotationTargetCode = doSearchcond.QuotationCode
                            //});
                            //var dataLst = (from a in qRes2
                            //               select new CTS370_GridHeader()
                            //               {
                            //                   Code = param.QuotationCode
                            //                   ,
                            //                   NameEN = a.CustFullNameEN
                            //                   ,
                            //                   NameLC = a.CustFullNameLC
                            //               }).ToList();

                            //CommonUtil.MappingObjectLanguage<CTS370_GridHeader>(dataLst);

                            //if (dataLst.Count == 1)
                            //{
                            //    obj = dataLst[0];
                            //}

                            var qRes2 = quotehandler.GetQuotationHeaderData(new doGetQuotationDataCondition()
                            {
                                QuotationTargetCode = doSearchcond.QuotationCode
                            });

                            obj = new CTS370_GridHeader()
                            {
                                Code = param.QuotationCode,
                                NameEN = qRes2.doContractTarget.CustFullNameEN,
                                NameJP = qRes2.doContractTarget.CustFullNameEN,
                                NameLC = qRes2.doContractTarget.CustFullNameLC
                            };

                            CommonUtil.MappingObjectLanguage(obj);
                        }
                    }
                }

                if (isValid)
                {
                    res.ResultData = obj;
                }
                //Comment by Jutarat A. on 27072012
                //else
                //{
                //    obj = new CTS370_GridHeader();
                //    res.ResultData = obj;
                //}
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Retrieve search result from criteria when click [Retrieve] from any section
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CTS370_RetrieveARResult(CTS370_ARCondition param)
        {
            ObjectResultData res = new ObjectResultData();
            doRetrieveARListCondition doSearchcond = new doRetrieveARListCondition();
            CTS370_SearchResult sres = new CTS370_SearchResult();
            dsTransDataModel dsTran;

            try
            {
                CTS370_ScreenParameter sParam = GetScreenObject<CTS370_ScreenParameter>();
                if (!String.IsNullOrEmpty(param.ARMode) && ((!String.IsNullOrEmpty(param.CustomerCode) || !String.IsNullOrEmpty(param.SiteCode) || !String.IsNullOrEmpty(param.ContractCode) || !String.IsNullOrEmpty(param.ProjectCode) || !String.IsNullOrEmpty(param.QuotationCode))))
                {
                    dsTran = CommonUtil.dsTransData;
                    doSearchcond = CreateCondition_CTS370(param);
                    List<dtARListCTS370> rawDat;
                    var resAR = RetrieveAR_CTS370(doSearchcond, out rawDat);

                    //if (resAR.Count > 0)
                    //{
                        //sres.DatCode = resAR[0].HeadCode;
                        //sres.DatName = String.IsNullOrEmpty(resAR[0].HeadName) ? "" : resAR[0].HeadName;

                        // Main AR
                        var resultXML = CommonUtil.ConvertToXml<CTS370_ARGridResult>(resAR, "Contract\\CTS370_AR", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                        sParam.strXmlResultAR = resultXML;
                        sres.SID = "1";
                        //sres.SID = dsTran.dtUserData.EmpNo + ".CTS370." + DateTime.Now.ToString("yyyyMMdd.HHmmssffff");
                        //Session.Add(sres.SID, resultXML);

                        // Site Occ
                        if (param.ARMode == ARRelevant.C_AR_RELEVANT_TYPE_CUSTOMER)
                        {
                            var resOcc = RetrieveOccurringSite_CTS370(param.ARMode, param.CustomerCode);
                            var resOccXML = CommonUtil.ConvertToXml<CTS370_AROccurringSiteGridResult>(resOcc, "Contract\\CTS370_OccurringSite", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                            sParam.strXmlResultOcc = resOccXML;
                            //Session.Add(sres.SID + ".occSite", resOccXML);
                        }

                        // Contract Occ
                        if (param.ARMode == ARRelevant.C_AR_RELEVANT_TYPE_SITE)
                        {
                            var resOcc = RetrieveOccurringContract_CTS370(param.ARMode, param.SiteCode);
                            var resOccXML = CommonUtil.ConvertToXml<CTS370_AROccurringContractGridResult>(resOcc, "Contract\\CTS370_OccurringContract", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                            sParam.strXmlResultOcc = resOccXML;
                            //Session.Add(sres.SID + ".occContract", resOccXML);
                        }

                        sres.SearchResult = true;
                    //}
                    //else
                    //{
                    //    sres.SearchResult = false;
                    //    sres.SID = string.Empty;
                    //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001, null, null);
                    //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    //}
                }
                else
                {
                    sres.SearchResult = false;
                    sres.SID = string.Empty;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006, null, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                }

                res.ResultData = sres;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Retrieve AR list from result
        /// </summary>
        /// <param name="sName"></param>
        /// <returns></returns>
        public ActionResult CTS370_RetrieveARData(String sName)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                CTS370_ScreenParameter sParam = GetScreenObject<CTS370_ScreenParameter>();
                res.ResultData = sParam.strXmlResultAR;
                //res.ResultData = Session[sName].ToString();
                Session.Remove(sName);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        
        /// <summary>
        /// Retrieve AR occurring list from result
        /// </summary>
        /// <param name="sName"></param>
        /// <param name="occType"></param>
        /// <returns></returns>
        public ActionResult CTS370_RetireveOccurringData(String sName, String occType)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                //string fullSName = sName;

                //if (occType == "1")
                //{
                //    fullSName = sName + ".occSite";
                //    res.ResultData = Session[fullSName].ToString();
                //    Session.Remove(fullSName);
                //}
                //else if (occType == "2")
                //{
                //    fullSName = sName + ".occContract";
                //    res.ResultData = Session[fullSName].ToString();
                //    Session.Remove(fullSName);
                //}

                CTS370_ScreenParameter sParam = GetScreenObject<CTS370_ScreenParameter>();
                res.ResultData = sParam.strXmlResultOcc;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        //public ActionResult CTS370_RetireveARData(CTS370_ARCondition param)
        //{
        //    ObjectResultData res = new ObjectResultData();
        //    doRetrieveARListCondition doSearchcond = new doRetrieveARListCondition();

        //    try
        //    {
        //        if (!String.IsNullOrEmpty(param.ARMode) && (!String.IsNullOrEmpty(param.CustomerCode) || !String.IsNullOrEmpty(param.SiteCode) || !String.IsNullOrEmpty(param.ContractCode) || !String.IsNullOrEmpty(param.ProjectCode) || !String.IsNullOrEmpty(param.QuotationCode)))
        //        {
        //            doSearchcond = CreateCondition_CTS370(param);
        //            var resultXML = CommonUtil.ConvertToXml<CTS370_ARGridResult>(RetrieveAR_CTS370(doSearchcond), "Contract\\CTS370_AR", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
        //            res.ResultData = resultXML;
        //        }
        //        else
        //        {
        //            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001, null, null);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.AddErrorMessage(ex);
        //    }

        //    //res.ResultData = true;

        //    return Json(res);
        //}

        //public ActionResult CTS370_RetireveOccurringData(CTS370_ARCondition param)
        //{
        //    ObjectResultData res = new ObjectResultData();
        //    doRetrieveARListCondition doSearchcond = new doRetrieveARListCondition();

        //    try
        //    {
        //        doSearchcond = CreateCondition_CTS370(param);

        //        if (doSearchcond.ARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_CUSTOMER)
        //        {
        //            res.ResultData = CommonUtil.ConvertToXml<CTS370_AROccurringSiteGridResult>(RetrieveOccurringSite_CTS370(doSearchcond), "Contract\\CTS370_OccurringSite", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
        //        }
        //        else if (doSearchcond.ARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_SITE)
        //        {
        //            res.ResultData = CommonUtil.ConvertToXml<CTS370_AROccurringContractGridResult>(RetrieveOccurringContract_CTS370(doSearchcond), "Contract\\CTS370_OccurringContract", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.AddErrorMessage(ex);
        //    }

        //    return Json(res);
        //}

        #endregion

        #region Method

        //private bool CheckUserPermission_CTS370()
        //{
        //    try
        //    {
        //        dsTransDataModel dsTrans = CommonUtil.dsTransData;
        //        if (dsTrans == null)
        //            return false;

        //        if (dsTrans.dtUserPermissionData != null)
        //        {
        //            var hasPermit = from a in dsTrans.dtUserPermissionData where a.Key.StartsWith(ScreenID.C_SCREEN_ID_AR_LIST) select a;
        //            return hasPermit.Count() > 0;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }

        //    return false;
        //}

        /// <summary>
        /// Create search criteria condition from parameter
        /// </summary>
        /// <param name="param"></param>
        /// <param name="needConvertCode"></param>
        /// <returns></returns>
        private CTS370_ARCondition CreateCondition_CTS370(CTS370_ScreenParameter param, bool needConvertCode = true)
        {
            CommonUtil util = new CommonUtil();

            CTS370_ARCondition newParam = new CTS370_ARCondition()
            {
                DuedateDeadline = null, //ARSearchDuedate.C_AR_SEARCH_DUEDATE_2WEEKS, //Modify by Jutarat A. on 05092012
                ARStatus = ARSearchStatus.C_AR_SEARCH_STATUS_HANDLING
            };

            if (param.strARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_CUSTOMER)
            {
                newParam.ARMode = param.strARRelevantType;
                //newParam.ARType = param.strARRelevantType;
                newParam.CustomerCode = param.strARRelevantCode;
                if (needConvertCode)
                {
                    newParam.CustomerCode = util.ConvertCustCode(param.strARRelevantCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                }
            }
            else if (param.strARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_SITE)
            {
                newParam.ARMode = param.strARRelevantType;
                //newParam.ARType = param.strARRelevantType;
                newParam.SiteCode = param.strARRelevantCode;
                if (needConvertCode)
                {
                    newParam.SiteCode = util.ConvertSiteCode(param.strARRelevantCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                }
            }
            else if (param.strARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_CONTRACT)
            {
                newParam.ARMode = param.strARRelevantType;
                //newParam.ARType = param.strARRelevantType;
                newParam.ContractCode = param.strARRelevantCode;
                if (needConvertCode)
                {
                    newParam.ContractCode = util.ConvertContractCode(param.strARRelevantCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                }
            }
            else if (param.strARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_PROJECT)
            {
                newParam.ARMode = param.strARRelevantType;
                //newParam.ARType = param.strARRelevantType;
                newParam.ProjectCode = param.strARRelevantCode;
            }
            else if (param.strARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_QUOTATION)
            {
                newParam.ARMode = param.strARRelevantType;
                //newParam.ARType = param.strARRelevantType;
                newParam.QuotationCode = param.strARRelevantCode;
                if (needConvertCode)
                {
                    newParam.QuotationCode = util.ConvertQuotationTargetCode(param.strARRelevantCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                }
            }

            return newParam;
        }

        /// <summary>
        /// Create search criteria condition from screen
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private doRetrieveARListCondition CreateCondition_CTS370(CTS370_ARCondition param)
        {
            doRetrieveARListCondition resCond = new doRetrieveARListCondition();
            CommonUtil util = new CommonUtil();

            if ((param != null) && (!String.IsNullOrEmpty(param.ARMode))
                    /*&& ((!String.IsNullOrEmpty(param.CustomerCode)) || (!String.IsNullOrEmpty(param.SiteCode))
                    || (!String.IsNullOrEmpty(param.ContractCode)) || (!String.IsNullOrEmpty(param.ProjectCode)) || (!String.IsNullOrEmpty(param.QuotationCode)))*/)
            {
                bool isValidData = false;
                if (param.ARMode == ARRelevant.C_AR_RELEVANT_TYPE_CUSTOMER) // Customer
                {
                    if (String.IsNullOrEmpty(param.CustomerCode))
                    {
                        ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006, null);
                    }

                    isValidData = true;
                    resCond = new doRetrieveARListCondition()
                    {
                        CustomerCode = util.ConvertCustCode(param.CustomerCode, CommonUtil.CONVERT_TYPE.TO_LONG),
                        ARRelevantType = ARRelevant.C_AR_RELEVANT_TYPE_CUSTOMER,
                        ARType = param.ARType,

                        //Modify by Jutarat A. on 05092012
                        //DuedateDeadline = ((param.DuedateDeadline == ARSearchDuedate.C_AR_SEARCH_DUEDATE_1MONTH) ? GetCurrentDateWithoutTime_CTS370().AddMonths(1) :
                        //    ((param.DuedateDeadline == ARSearchDuedate.C_AR_SEARCH_DUEDATE_2WEEKS) ? GetCurrentDateWithoutTime_CTS370().AddDays(14) :
                        //    ((param.DuedateDeadline == ARSearchDuedate.C_AR_SEARCH_DUEDATE_1WEEK) ? GetCurrentDateWithoutTime_CTS370().AddDays(7) : (DateTime?)null))),
                        DuedateDeadline = null,
                        //End Modify

                        ARStatus = param.ARStatus
                    };
                }
                else if (param.ARMode == ARRelevant.C_AR_RELEVANT_TYPE_SITE) // Site
                {
                    if (String.IsNullOrEmpty(param.SiteCode))
                    {
                        ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006, null);
                    }

                    isValidData = true;
                    resCond = new doRetrieveARListCondition()
                    {
                        SiteCode = util.ConvertSiteCode(param.SiteCode, CommonUtil.CONVERT_TYPE.TO_LONG),
                        ARRelevantType = ARRelevant.C_AR_RELEVANT_TYPE_SITE,
                        ARType = param.ARType,

                        //Modify by Jutarat A. on 05092012
                        //DuedateDeadline = ((param.DuedateDeadline == ARSearchDuedate.C_AR_SEARCH_DUEDATE_1MONTH) ? GetCurrentDateWithoutTime_CTS370().AddMonths(1) :
                        //    ((param.DuedateDeadline == ARSearchDuedate.C_AR_SEARCH_DUEDATE_2WEEKS) ? GetCurrentDateWithoutTime_CTS370().AddDays(14) :
                        //    ((param.DuedateDeadline == ARSearchDuedate.C_AR_SEARCH_DUEDATE_1WEEK) ? GetCurrentDateWithoutTime_CTS370().AddDays(7) : (DateTime?)null))),
                        DuedateDeadline = null,
                        //End Modify
                            
                        ARStatus = param.ARStatus
                    };
                }
                else if (param.ARMode == ARRelevant.C_AR_RELEVANT_TYPE_CONTRACT) // Contract
                {
                    if (String.IsNullOrEmpty(param.ContractCode))
                    {
                        ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006, null);
                    }

                    isValidData = true;
                    resCond = new doRetrieveARListCondition()
                    {
                        ContractCode = util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG),
                        UserCode = param.ContractCode,
                        ARRelevantType = ARRelevant.C_AR_RELEVANT_TYPE_CONTRACT,
                        ARType = param.ARType,

                        //Modify by Jutarat A. on 05092012
                        //DuedateDeadline = ((param.DuedateDeadline == ARSearchDuedate.C_AR_SEARCH_DUEDATE_1MONTH) ? GetCurrentDateWithoutTime_CTS370().AddMonths(1) :
                        //    ((param.DuedateDeadline == ARSearchDuedate.C_AR_SEARCH_DUEDATE_2WEEKS) ? GetCurrentDateWithoutTime_CTS370().AddDays(14) :
                        //    ((param.DuedateDeadline == ARSearchDuedate.C_AR_SEARCH_DUEDATE_1WEEK) ? GetCurrentDateWithoutTime_CTS370().AddDays(7) : (DateTime?)null))),
                        DuedateDeadline = null,
                        //End Modify

                        ARStatus = param.ARStatus
                    };
                }
                else if (param.ARMode == ARRelevant.C_AR_RELEVANT_TYPE_PROJECT) // Project
                {
                    if (String.IsNullOrEmpty(param.ProjectCode))
                    {
                        ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006, null);
                    }

                    isValidData = true;
                    resCond = new doRetrieveARListCondition()
                    {
                        ProjectCode = param.ProjectCode,
                        ARRelevantType = ARRelevant.C_AR_RELEVANT_TYPE_PROJECT,
                        ARType = param.ARType,

                        //Modify by Jutarat A. on 05092012
                        //DuedateDeadline = ((param.DuedateDeadline == ARSearchDuedate.C_AR_SEARCH_DUEDATE_1MONTH) ? GetCurrentDateWithoutTime_CTS370().AddMonths(1) :
                        //    ((param.DuedateDeadline == ARSearchDuedate.C_AR_SEARCH_DUEDATE_2WEEKS) ? GetCurrentDateWithoutTime_CTS370().AddDays(14) :
                        //    ((param.DuedateDeadline == ARSearchDuedate.C_AR_SEARCH_DUEDATE_1WEEK) ? GetCurrentDateWithoutTime_CTS370().AddDays(7) : (DateTime?)null))),
                        DuedateDeadline = null,
                        //End Modify

                        ARStatus = param.ARStatus
                    };
                }
                else if (param.ARMode == ARRelevant.C_AR_RELEVANT_TYPE_QUOTATION) // Quotation
                {
                    if (String.IsNullOrEmpty(param.QuotationCode))
                    {
                        ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006, null);
                    }

                    isValidData = true;
                    resCond = new doRetrieveARListCondition()
                    {
                        QuotationCode = util.ConvertQuotationTargetCode(param.QuotationCode, CommonUtil.CONVERT_TYPE.TO_LONG),
                        ARRelevantType = ARRelevant.C_AR_RELEVANT_TYPE_QUOTATION,
                        ARType = param.ARType,

                        //Modify by Jutarat A. on 05092012
                        //DuedateDeadline = ((param.DuedateDeadline == ARSearchDuedate.C_AR_SEARCH_DUEDATE_1MONTH) ? GetCurrentDateWithoutTime_CTS370().AddMonths(1) :
                        //    ((param.DuedateDeadline == ARSearchDuedate.C_AR_SEARCH_DUEDATE_2WEEKS) ? GetCurrentDateWithoutTime_CTS370().AddDays(14) :
                        //    ((param.DuedateDeadline == ARSearchDuedate.C_AR_SEARCH_DUEDATE_1WEEK) ? GetCurrentDateWithoutTime_CTS370().AddDays(7) : (DateTime?)null))),
                        DuedateDeadline = null,
                        //End Modify

                        ARStatus = param.ARStatus
                    };
                }
            }

            return resCond;
        }

        /// <summary>
        /// Retrieve search AR list
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="rawData"></param>
        /// <returns></returns>
        private List<CTS370_ARGridResult> RetrieveAR_CTS370(doRetrieveARListCondition cond, out List<dtARListCTS370> rawData)
        {
            List<CTS370_ARGridResult> result = new List<CTS370_ARGridResult>();
            IARHandler arhandler;
            CommonUtil util = new CommonUtil();
            List<dtARListCTS370> arResult;
            CommonUtil.LANGUAGE_LIST curLang;
            string strHeadCode = "";
            string strHeadName = "";

            rawData = new List<dtARListCTS370>();

            try
            {
                curLang = CommonUtil.CurrentLanguage(false);
                arhandler = ServiceContainer.GetService<IARHandler>() as IARHandler;

                arResult = arhandler.GetARList(cond);
                //arResult = CreateDummieARList_CTS370(cond);
                rawData = arResult;

                if (arResult.Count > 0)
                {
                    CommonUtil.MappingObjectLanguage<dtARListCTS370>(arResult);

                    if (cond.ARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_CUSTOMER)
                    {
                        strHeadCode = util.ConvertCustCode(arResult[0].CustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        strHeadName = arResult[0].CustFullName;
                    }
                    else if (cond.ARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_SITE)
                    {
                        strHeadCode = util.ConvertSiteCode(arResult[0].SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        strHeadName = arResult[0].SiteName;
                    }
                    else if (cond.ARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_CONTRACT)
                    {
                        strHeadCode = util.ConvertContractCode(arResult[0].ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        strHeadName = arResult[0].ContractTargetPurchaseCustFullName;
                    }
                    else if (cond.ARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_PROJECT)
                    {
                        strHeadCode = arResult[0].ProjectCode;
                        strHeadName = arResult[0].ProjectName;
                    }
                    else if (cond.ARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_QUOTATION)
                    {
                        strHeadCode = util.ConvertQuotationTargetCode(arResult[0].QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        strHeadName = arResult[0].QuotationTargetCustFullName;
                    }

                    result = PharseOutput_CTS370(arResult, strHeadCode, strHeadName);
                }
                //else
                //{
                //    ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001, null);
                //    //throw new Exception(Common.Util.MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, "MSG0001", null).Message);
                //}
            }
            catch (Exception)
            {
                
                throw;
            }


            return result;
        }

        /// <summary>
        /// Retrieve search AR occurring site list
        /// </summary>
        /// <param name="arRelevantType"></param>
        /// <param name="CustCode"></param>
        /// <returns></returns>
        private List<CTS370_AROccurringSiteGridResult> RetrieveOccurringSite_CTS370(string arRelevantType, string CustCode)
        {
            CommonUtil util = new CommonUtil();
            List<CTS370_AROccurringSiteGridResult> result = new List<CTS370_AROccurringSiteGridResult>();
            IARHandler arhandler = ServiceContainer.GetService<IARHandler>() as IARHandler;

            if (arRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_CUSTOMER)
            {
                //var arList = arhandler.GetARList(param);
                //var arList = CreateDummieARList_CTS370(param);
                //var occurringResult = arhandler.GetOccurringSiteList(arList);
                //var occurringResult = CreateDummieAROccSiteList_CTS370(arList);
                //result = RetrieveOccurringSite_CTS370(arList);
                var rawRes = arhandler.GetOccurringSiteList(util.ConvertCustCode(CustCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                result = PharseOutput_CTS370(rawRes);
            }

            return result;
        }

        //private List<CTS370_AROccurringSiteGridResult> RetrieveOccurringSite_CTS370(List<dtARListCTS370> rawDat)
        //{
        //    List<CTS370_AROccurringSiteGridResult> result = new List<CTS370_AROccurringSiteGridResult>();
        //    IARHandler arhandler = ServiceContainer.GetService<IARHandler>() as IARHandler;

        //    var occurringResult = new List<dtAROccSite>();

        //    if (rawDat.Count > 0)
        //    {
        //        var doList = from a in rawDat where !String.IsNullOrEmpty(a.CustCode) select a;
        //        occurringResult = arhandler.GetOccurringSiteList(doList.ToList());
        //    }

        //    //var occurringResult = CreateDummieAROccSiteList_CTS370(rawDat);
        //    result = PharseOutput_CTS370(occurringResult);

        //    return result;
        //}

        /// <summary>
        /// Retrieve search AR occurring contract list
        /// </summary>
        /// <param name="arRelevantType"></param>
        /// <param name="SiteCode"></param>
        /// <returns></returns>
        private List<CTS370_AROccurringContractGridResult> RetrieveOccurringContract_CTS370(string arRelevantType, string SiteCode)
        {
            CommonUtil util = new CommonUtil();
            List<CTS370_AROccurringContractGridResult> result = new List<CTS370_AROccurringContractGridResult>();
            IARHandler arhandler = ServiceContainer.GetService<IARHandler>() as IARHandler;

            if (arRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_SITE)
            {
                //var arList = arhandler.GetARList(param);
                //var arList = CreateDummieARList_CTS370(param);
                //var occurringResult = arhandler.GetOccurringContractList(arList);
                //var occurringResult = CreateDummieAROccContractList_CTS370(arList);
                //result = RetrieveOccurringContract_CTS370(arList);
                var rawRes = arhandler.GetOccurringContractList(util.ConvertSiteCode(SiteCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                result = PharseOutput_CTS370(rawRes);
            }

            return result;
        }

        //private List<CTS370_AROccurringContractGridResult> RetrieveOccurringContract_CTS370(List<dtARListCTS370> rawDat)
        //{
        //    List<CTS370_AROccurringContractGridResult> result = new List<CTS370_AROccurringContractGridResult>();
        //    IARHandler arhandler = ServiceContainer.GetService<IARHandler>() as IARHandler;

        //    var occurringResult = new List<dtAROccContract>();

        //    if (rawDat.Count > 0)
        //    {
        //        var doList = from a in rawDat where !String.IsNullOrEmpty(a.SiteCode) select a;
        //        occurringResult = arhandler.GetOccurringContractList(doList.ToList());
        //    }

        //    //var occurringResult = CreateDummieAROccContractList_CTS370(rawDat);
        //    result = PharseOutput_CTS370(occurringResult);

        //    return result;
        //}

        /// <summary>
        /// Create output display for each AR list item
        /// </summary>
        /// <param name="rawDat"></param>
        /// <param name="headCode"></param>
        /// <param name="headName"></param>
        /// <returns></returns>
        private List<CTS370_ARGridResult> PharseOutput_CTS370(List<dtARListCTS370> rawDat, string headCode, string headName)
        {
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<CTS370_ARGridResult> result = new List<CTS370_ARGridResult>();
            List<string> miscTypeNameLst = new List<string>();

            miscTypeNameLst.Add(MiscType.C_AR_STATUS);
            miscTypeNameLst.Add(MiscType.C_AR_TYPE);

            var miscTypeLst = commonhandler.GetMiscTypeCodeListByFieldName(miscTypeNameLst);

            if (rawDat != null)
            {
                CommonUtil.MappingObjectLanguage<dtARListCTS370>(rawDat);

                foreach (dtARListCTS370 item in rawDat)
                {
                    string arTypeName = "";
                    string arStatusname = "";

                    var tmpARType = from a in miscTypeLst where a.FieldName == MiscType.C_AR_TYPE && a.ValueCode == item.ARType select a;
                    var tmpARStatus = from a in miscTypeLst where a.FieldName == MiscType.C_AR_STATUS && a.ValueCode == item.ARStatus select a;

                    if (item.DueDateDeadLine.HasValue)
                    {
                        item.DueDateDeadLine = new DateTime(item.DueDateDeadLine.Value.Year
                            , item.DueDateDeadLine.Value.Month
                            , item.DueDateDeadLine.Value.Day
                            , 23, 59, 59);

                        TimeSpan DeadlineTime;
                        if (TimeSpan.TryParse(item.DueDateDeadLineTime, out DeadlineTime))
                        {
                            item.DueDateDeadLine = new DateTime(item.DueDateDeadLine.Value.Year
                            , item.DueDateDeadLine.Value.Month
                            , item.DueDateDeadLine.Value.Day
                            , DeadlineTime.Hours
                            , DeadlineTime.Minutes
                            , DeadlineTime.Seconds);
                        }
                    }

                    if (tmpARType.Count() > 0)
                    {
                        arTypeName = tmpARType.First().ValueDisplay;
                    }

                    if (tmpARStatus.Count() > 0)
                    {
                        arStatusname = tmpARStatus.First().ValueDisplay;
                    }

                    CTS370_ARGridResult tmp = new CTS370_ARGridResult()

                    //tmp.HeadCode = headCode;
                    //tmp.HeadName = headName;
                    //tmp.CompleteDate = (item.ApprovedDate.HasValue) ? item.ApprovedDate.Value.ToString("dd-MMM-yyyy") : String.Empty;
                    //tmp.ApproverName = (CommonUtil.CurrentLanguage(false) == CommonUtil.LANGUAGE_LIST.LANGUAGE_1) ? String.Format("{0} {1}", item.ApprEmpFirstNameEN, item.ApprEmpLastNameEN) : String.Format("{0} {1}", item.ApprEmpFirstNameLC, item.ApprEmpLastNameLC);
                    //tmp.ARFlag = item.ImportanceFlag.GetValueOrDefault() ? MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3220).Message : "";
                    //tmp.ARNo = item.RequestNo;
                    //tmp.ARID = item.RequestNo;
                    //tmp.ARStatus = (item.DueDateDeadLine.GetValueOrDefault() > DateTime.Now) ? MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3221).Message : (item.hasRespondingDetailFlag.GetValueOrDefault() ? MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3222).Message : "");
                    //tmp.ARType = (CommonUtil.CurrentLanguage(true) == CommonUtil.LANGUAGE_LIST.LANGUAGE_1) ? item.ARTypeNameEN : (CommonUtil.CurrentLanguage(true) == CommonUtil.LANGUAGE_LIST.LANGUAGE_2) ? item.ARTypeNameJP : item.ARTypeNameLC;
                    //tmp.DueDateDeadline = (item.DueDateDeadLine.HasValue && item.DueDateDeadLineTime.HasValue) ? String.Format("{0} : {1}", item.DueDateDeadLine.Value.ToString("dd-MMM-yyyy"), String.Format("{0:D2}.{1:D2}", item.DueDateDeadLineTime.Value.Hours, item.DueDateDeadLineTime.Value.Minutes)) : "-";
                    //tmp.RegisterDate = item.CreateDate.HasValue ? item.CreateDate.Value.ToString("dd-MMM-yyyy") : "";
                    //tmp.RequesterName = (CommonUtil.CurrentLanguage(false) == CommonUtil.LANGUAGE_LIST.LANGUAGE_1) ? String.Format("{0} {1}", item.ReqEmpFirstNameEN, item.ReqEmpLastNameEN) : String.Format("{0} {1}", item.ReqEmpFirstNameLC, item.ReqEmpLastNameLC);
                    //tmp.Status = ((CommonUtil.CurrentLanguage(true) == CommonUtil.LANGUAGE_LIST.LANGUAGE_1) ? item.ARStatusNameEN : (CommonUtil.CurrentLanguage(true) == CommonUtil.LANGUAGE_LIST.LANGUAGE_2) ? item.ARStatusNameJP : item.ARStatusNameLC);


                    {
                        HeadCode = headCode,
                        HeadName = headName,
                        CompleteDate = (item.ApprovedDate.HasValue) ? CommonUtil.TextDate(item.ApprovedDate) : String.Empty,
                        //ApproverName = String.Format("{0} {1}", item., item.ApprEmpLastName),
                        ApproverName = (!CommonUtil.IsNullOrEmpty(item.ApprEmpName)) ? item.ApprEmpName.Replace("^", "<br />") : "",
                        ARFlag = item.ImportanceFlag.GetValueOrDefault() ? "★" : "",
                        ARNo = item.RequestNo,
                        ARID = item.RequestNo,

                        //Modify by Jutarat A. on 04092012
                        //ARStatus = ((DateTime.Now > item.DueDateDeadLine.GetValueOrDefault())
                        //    && (item.ARStatusCode != ARStatus.C_AR_STATUS_INSTRUCTED)
                        //    && (item.ARStatusCode != ARStatus.C_AR_STATUS_REJECTED)
                        //    && (item.ARStatusCode != ARStatus.C_AR_STATUS_APPROVED)) ? "Over" : (!item.hasRespondingDetailFlag.GetValueOrDefault() ? "New" : ""),
                        ARStatus = (!item.hasRespondingDetailFlag.GetValueOrDefault() ? "New" : ""),
                        //End Modify

                        ARType = arTypeName,
                        DueDateDeadline = (item.DueDateDeadLine.HasValue && (item.DueDateDeadLineTime != null) ) ?  CommonUtil.TextDate(item.DueDateDeadLine) + "<br>" + item.DueDateDeadLineTime : "-",
                        RegisterDate = item.CreateDate.HasValue ? CommonUtil.TextDate(item.CreateDate) : "",
                        //RequesterName = String.Format("{0} {1}", item.ReqEmpFirstName, item.ReqEmpLastName),
                        RequesterName = item.ReqEmpName,
                        Status = arStatusname
                    };

                    result.Add(tmp);
                }
            }
            
            return result;
        }

        /// <summary>
        /// Create output display for each AR occurring site list item
        /// </summary>
        /// <param name="rawDat"></param>
        /// <returns></returns>
        private List<CTS370_AROccurringSiteGridResult> PharseOutput_CTS370(List<dtAROccSite> rawDat)
        {
            List<CTS370_AROccurringSiteGridResult> result = new List<CTS370_AROccurringSiteGridResult>();

            if (rawDat != null)
            {
                int cnt = 1;
                CommonUtil util = new CommonUtil();

                foreach (dtAROccSite item in rawDat)
                {
                    CTS370_AROccurringSiteGridResult tmp = new CTS370_AROccurringSiteGridResult()
                    {
                        Delay = (!item.CountDelayAR.HasValue || (item.CountDelayAR.Value == 0)) ? "-" : item.CountDelayAR.Value.ToString(),
                        AR = (!item.CountAR.HasValue || (item.CountAR.Value == 0)) ? "-" : item.CountAR.Value.ToString(),
                        No = cnt.ToString(),
                        SiteCode = util.ConvertSiteCode(item.SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                        SiteNameEN = item.SiteNameEN,
                        SiteNameLC = item.SiteNameLC
                    };

                    result.Add(tmp);

                    cnt++;
                }
            }

            return result;
        }

        /// <summary>
        /// Create output display for each AR occurring contract list item
        /// </summary>
        /// <param name="rawDat"></param>
        /// <returns></returns>
        private List<CTS370_AROccurringContractGridResult> PharseOutput_CTS370(List<dtAROccContract> rawDat)
        {
            List<CTS370_AROccurringContractGridResult> result = new List<CTS370_AROccurringContractGridResult>();

            if (rawDat != null)
            {
                int cnt = 1;
                CommonUtil util = new CommonUtil();

                foreach (dtAROccContract item in rawDat)
                {
                    CTS370_AROccurringContractGridResult tmp = new CTS370_AROccurringContractGridResult()
                    {
                        Delay = (!item.CountDelayAR.HasValue || (item.CountDelayAR.Value == 0)) ? "-" : item.CountDelayAR.Value.ToString(),
                        AR = (!item.CountAR.HasValue || (item.CountAR.Value == 0)) ? "-" : item.CountAR.Value.ToString(),
                        //No = cnt.ToString(),
                        ContractCode = util.ConvertContractCode(item.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                        ContractNameEN = item.ContractTargetFullNameEN,
                        ContractNameLC = item.ContractTargetFullNameLC
                    };

                    result.Add(tmp);

                    cnt++;
                }
            }

            return result;
        }

        /// <summary>
        /// Validate user permission
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        private ObjectResultData ValidateAuthority_CTS370(ObjectResultData res)
        {
            // Check Permission
            if (!CheckUserPermission(ScreenID.C_SCREEN_ID_AR_LIST))
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                return res;
            }

            return res;
        }

        /// <summary>
        /// Get current date without time
        /// </summary>
        /// <returns></returns>
        private DateTime GetCurrentDateWithoutTime_CTS370()
        {
            var currDate = DateTime.Now;
            return new DateTime(currDate.Year, currDate.Month, currDate.Day);
        }

        //private void SetScreenParameter_CTS370(CTS370_ScreenParameter obj)
        //{
        //    Session.Remove("CTS370_PARAM");
        //    Session.Add("CTS370_PARAM", obj);
        //}

        //private CTS370_ScreenParameter GetScreenObject_CTS370()
        //{
        //    CTS370_ScreenParameter obj = null;

        //    if (Session["CTS370_PARAM"] != null)
        //    {
        //        obj = (CTS370_ScreenParameter)Session["CTS370_PARAM"];
        //    }

        //    return obj;
        //}

        #endregion

        #region Dummie Data
        private List<dtARListCTS370> CreateDummieARList_CTS370(doRetrieveARListCondition cond, int amount = 7)
        {
            List<dtARListCTS370> result = new List<dtARListCTS370>();
            Random rand = new Random();
            CommonUtil util = new CommonUtil();
            IProjectHandler projecthandler = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            string dummieCode = "";
            string dummieNameLC = "", dummieNameEN = "";

            if (cond.ARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_CUSTOMER)
            {
                dummieCode = "C0000000125";
                dummieNameEN = "BANGKOK UNIVERSITY";
                dummieNameLC = "มหาวิทยาลัยกรุงเทพ";
            }
            else if (cond.ARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_SITE)
            {
                dummieCode = "S9999999999-0001";
                dummieNameEN = "CSI ASIA Co., LTD.";
                dummieNameLC = "บริษัท ซีเอสไอ เอเชีย จำกัด";
            }
            else if (cond.ARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_CONTRACT)
            {
                dummieCode = "N0000000001";
                dummieNameEN = "CSI ASIA Co., LTD.";
                dummieNameLC = "บริษัท ซีเอสไอ เอเชีย จำกัด";
            }
            else if (cond.ARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_PROJECT)
            {
                dummieCode = projecthandler.GenerateProjectCode();
                dummieNameEN = "Project Diva Extend";
                dummieNameLC = dummieNameEN;
            }
            else if (cond.ARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_QUOTATION)
            {
                dummieCode = "FN1234567";
                dummieNameEN = "CSI ASIA Co., LTD.";
                dummieNameLC = "บริษัท ซีเอสไอ เอเชีย จำกัด";
            }

            for (int i = 0; i < amount; i++)
            {
                dtARListCTS370 tmp = new dtARListCTS370()
                {
                    ApprEmpFirstNameEN = "Mr.XXXXX",
                    ApprEmpFirstNameLC = "นาย XXXXX",
                    ApprEmpLastNameEN = "YYYYYYY",
                    ApprEmpLastNameLC = "YYYYYYY",
                    ApprovedDate = (rand.Next(0, 2) == 1) ? DateTime.Now.AddDays(rand.Next(4, 8)) : (DateTime?)null,
                    ARRelavantType = cond.ARRelevantType,
                    ARType = ARType.C_AR_TYPE_REQUEST_FOR_PURCHASING_OFFICE_EQUIPMENT,
                    ARStatus = (cond.ARStatus == ARSearchStatus.C_AR_SEARCH_STATUS_COMPLETE) ? ARStatus.C_AR_STATUS_APPROVED : ARStatus.C_AR_STATUS_WAIT_FOR_APPROVAL,
                    ReqEmpFirstNameEN = "Mr.AAAAAA",
                    ReqEmpFirstNameLC = "นาย AAAAAA",
                    ReqEmpLastNameEN = "BBBBBBBB",
                    ReqEmpLastNameLC  = "BBBBBBBB",
                    RequestNo = rand.Next(100000, 999999).ToString() + "A" + rand.Next(100000, 999999).ToString(),
                    DueDateDeadLine = (rand.Next(0, 2) == 1) ? DateTime.Now.AddDays(rand.Next(4, 8)) : DateTime.Now.AddDays(rand.Next(-8, -4)),
                    CreateDate = DateTime.Now.AddDays(rand.Next(-16, -8)),
                    ContractCode = dummieCode,
                    ContractTargetPurchaseCustFullNameEN = dummieNameEN,
                    ContractTargetPurchaseCustFullNameLC = dummieNameLC,
                    CustCode = dummieCode,
                    CustFullNameEN = dummieNameEN,
                    CustFullNameLC = dummieNameLC,
                    SiteCode = dummieCode,
                    SiteNameEN = dummieNameEN,
                    SiteNameLC = dummieNameLC,
                    ProjectCode = dummieCode,
                    ProjectName = dummieNameEN,
                    QuotationTargetCode = dummieCode,
                    QuotationTargetCustFullNameEN = dummieNameEN,
                    QuotationTargetCustFullNameLC = dummieNameLC,
                    ImportanceFlag = (rand.Next(0, 2) == 1) ? true : false,
                    hasRespondingDetailFlag = (rand.Next(0, 2) == 1) ? true : false,
                };

                //tmp.DueDateDeadLineTime = tmp.DueDateDeadLine.Value.TimeOfDay;

                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                {
                    new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_AR_TYPE,
                        ValueCode = tmp.ARType
                    }
                };

                var artypename = commonhandler.GetMiscTypeCodeList(miscs);
                if (artypename.Count > 0)
                {
                    tmp.ARTypeNameEN = artypename[0].ValueDisplayEN;
                    tmp.ARTypeNameJP = artypename[0].ValueDisplayJP;
                    tmp.ARTypeNameLC = artypename[0].ValueDisplayLC;
                }

                miscs = new List<doMiscTypeCode>()
                {
                    new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_AR_STATUS,
                        ValueCode = tmp.ARStatus
                    }
                };

                var arstatusname = commonhandler.GetMiscTypeCodeList(miscs);
                if (arstatusname.Count > 0)
                {
                    tmp.ARStatusNameEN = arstatusname[0].ValueDisplayEN;
                    tmp.ARStatusNameJP = arstatusname[0].ValueDisplayJP;
                    tmp.ARStatusNameLC = arstatusname[0].ValueDisplayLC;
                }

                result.Add(tmp);
            }

            return result;
        }

        private List<dtAROccSite> CreateDummieAROccSiteList_CTS370(List<dtARListCTS370> rawDat, int amount = 0)
        {
            List<dtAROccSite> result = new List<dtAROccSite>();
            Random rand = new Random();

            for (int i = 0; i < amount; i++)
            {
                dtAROccSite temp = new dtAROccSite()
                {
                    CountDelayAR = rand.Next(0, 13),
                    CountAR = rand.Next(0, 21),
                    SiteCode = rand.Next(1, 9999).ToString("0000") + rand.Next(1, 9999).ToString("0000") + "-" + (i + 1).ToString("0000"),
                    SiteNameEN = "SiteName EN",
                    SiteNameLC = "SiteName LC"
                };

                result.Add(temp);
            }

            return result;
        }

        private List<dtAROccContract> CreateDummieAROccContractList_CTS370(List<dtARListCTS370> rawDat, int amount = 7)
        {
            List<dtAROccContract> result = new List<dtAROccContract>();
            Random rand = new Random();

            for (int i = 0; i < amount; i++)
            {
                dtAROccContract temp = new dtAROccContract()
                {
                    CountDelayAR = rand.Next(0, 13),
                    CountAR = rand.Next(0, 21),
                    ContractCode = "N" + rand.Next(1000, 9999).ToString() + rand.Next(111, 999).ToString(),
                    ContractTargetFullNameEN = "ContractName EN",
                    ContractTargetFullNameLC = "ContractName LC"
                };

                result.Add(temp);
            }

            return result;
        }
        #endregion
    }
}
