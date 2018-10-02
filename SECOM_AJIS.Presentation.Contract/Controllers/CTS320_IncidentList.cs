//*********************************
// Create by: Natthavat S.
// Create date: 06/Oct/2011
// Update date: 06/Oct/2011
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
        public ActionResult CTS320_Authority(CTS320_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil comUtil = new CommonUtil();

            try
            {
                res = ValidateAuthority_CTS320(res);
                if (res.IsError)
                {
                    return Json(res);
                }

                ICustomerMasterHandler customerhandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
                ISiteMasterHandler sitehandler = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;
                ICommonContractHandler commoncontracthandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
                IProjectHandler projecthandler = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
                ISaleContractHandler salehandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                IRentralContractHandler rentalhandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                IQuotationHandler quotehandler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
                CommonUtil util = new CommonUtil();
                if (param.strIncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CUSTOMER)
                {
                    if (CommonUtil.IsNullOrEmpty(param.strIncidentRelevantCode))
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006, new string[] { param.strIncidentRelevantCode }, new string[] { "txtCustomerCode" });
                    }
                    else if (!customerhandler.CheckExistCustomerData(util.ConvertCustCode(param.strIncidentRelevantCode, CommonUtil.CONVERT_TYPE.TO_LONG)))
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { param.strIncidentRelevantCode }, new string[] { "txtCustomerCode" });
                    }

                }
                else if (param.strIncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_SITE)
                {
                    if (CommonUtil.IsNullOrEmpty(param.strIncidentRelevantCode))
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006, new string[] { param.strIncidentRelevantCode }, new string[] { "txtSiteCode" });
                    }
                    else if (!sitehandler.CheckExistSiteData(util.ConvertSiteCode(param.strIncidentRelevantCode, CommonUtil.CONVERT_TYPE.TO_LONG), null))
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { param.strIncidentRelevantCode }, new string[] { "txtSiteCode" });
                    }

                }
                else if (param.strIncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CONTRACT)
                {
                    if (CommonUtil.IsNullOrEmpty(param.strIncidentRelevantCode))
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006, new string[] { param.strIncidentRelevantCode }, new string[] { "txtContractCode" });
                    }
                    else if (!commoncontracthandler.IsContractExistInRentalOrSale(util.ConvertContractCode(param.strIncidentRelevantCode, CommonUtil.CONVERT_TYPE.TO_LONG)))
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { param.strIncidentRelevantCode }, new string[] { "txtContractCode" });
                    }

                }
                else if (param.strIncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_PROJECT)
                {
                    var sRes = projecthandler.IsProjectExist((param.strIncidentRelevantCode));

                    if (CommonUtil.IsNullOrEmpty(param.strIncidentRelevantCode))
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006, new string[] { param.strIncidentRelevantCode }, new string[] { "txtProjectCode" });
                    }
                    else if ((sRes.Count < 1) || !sRes[0].GetValueOrDefault())
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { param.strIncidentRelevantCode }, new string[] { "txtProjectCode" });
                    }

                }


                //if ((param != null) && (!String.IsNullOrEmpty(param.strIncidentRelevantCode)) && (!String.IsNullOrEmpty(param.strIncidentRelevantType)))
                //{
                //    var cond = CreateCondition_CTS320(param);
                //    doRetrieveIncidentListCondition rawCond = new doRetrieveIncidentListCondition()
                //    {
                //        IncidentRelevantType = param.strIncidentRelevantType,
                //        ContractCode = comUtil.ConvertContractCode(cond.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG),
                //        CustomerCode = comUtil.ConvertCustCode(cond.CustomerCode, CommonUtil.CONVERT_TYPE.TO_LONG),
                //        SiteCode = comUtil.ConvertSiteCode(cond.SiteCode, CommonUtil.CONVERT_TYPE.TO_LONG),
                //        ProjectCode = cond.ProjectCode,
                //        DuedateDeadline = ((cond.DuedateDeadline == IncidentSearchDuedate.C_INCIDENT_SEARCH_DUEDATE_1MONTH) ? DateTime.Now.AddMonths(1) :
                //            ((cond.DuedateDeadline == IncidentSearchDuedate.C_INCIDENT_SEARCH_DUEDATE_2WEEKS) ? DateTime.Now.AddDays(14) :
                //            ((cond.DuedateDeadline == IncidentSearchDuedate.C_INCIDENT_SEARCH_DUEDATE_1WEEK) ? DateTime.Now.AddDays(7) : DateTime.Now))),
                //        IncidentStatus = cond.IncidentStatus,
                //        IncidentType = cond.IncidentType
                //    };
                //    var condRes = RetrieveIncident_CTS320(rawCond);
                //    if (condRes.Count == 0)
                //    {
                //        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                //        return Json(res);
                //    }
                //}
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<CTS320_ScreenParameter>("CTS320", param, res);
        }
        

        //public ActionResult CTS320_Authority(string strIncidentRelevantCode, string strIncidentRelevantType)
        //{
        //    ObjectResultData res = new ObjectResultData();
        //    CommonUtil comUtil = new CommonUtil();
        //    CTS320_ScreenParameter param = null;

        //    try
        //    {
        //        // Check Permission
        //        if (!CheckUserPermission(ScreenID.C_SCREEN_ID_INCIDENT_LIST))
        //        {
        //            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
        //            return Json(res);
        //        }

        //        param = new CTS320_ScreenParameter()
        //        {
        //            strIncidentRelevantCode = strIncidentRelevantCode,
        //            strIncidentRelevantType = strIncidentRelevantType
        //        };

        //        //SetScreenParameter_CTS320(param);
        //    }
        //    catch (Exception ex)
        //    {
        //        res.AddErrorMessage(ex);
        //        return Json(res);
        //    }

        //    return InitialScreenEnvironment("CTS320", param);
        //}

        #endregion

        #region Action

        /// <summary>
        /// Initial screen
        /// </summary>
        /// <returns></returns>
        [Initialize("CTS320")]
        public ActionResult CTS320() // InitialState
        {
            return View();
        }

        /// <summary>
        /// Initial incident list's grid schema
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS320_InitialGridIncident()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Contract\\CTS320_Incident"));
        }

        /// <summary>
        /// Initial incident occurring site's grid schema
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS320_InitialGridOccurringSite()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Contract\\CTS320_OccurringSite"));
        }

        /// <summary>
        /// Initial incident occurring contract's grid schema
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS320_InitialGridOccurringContract()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Contract\\CTS320_OccurringContract"));
        }

        /// <summary>
        /// Initial screen default value
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS320_RetrieveInitialDefaultValue()
        {
            ObjectResultData result = new ObjectResultData();
            CTS320_DefaultValue res = new CTS320_DefaultValue();

            res.DueDate = IncidentSearchDuedate.C_INCIDENT_SEARCH_DUEDATE_2WEEKS;
            res.Status = IncidentSearchStatus.C_INCIDENT_SEARCH_STATUS_HANDLING;
            result.ResultData = res;

            return Json(result);
        }

        /// <summary>
        /// Checking parameter from another screen
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS320_CheckParameter()
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                CTS320_ScreenParameter sParam = GetScreenObject<CTS320_ScreenParameter>();
                //CTS320_ScreenParameter sParam = GetScreenObject_CTS320();

                if ((sParam != null) && (!String.IsNullOrEmpty(sParam.strIncidentRelevantCode)) && (!String.IsNullOrEmpty(sParam.strIncidentRelevantType)))
                {
                    res.ResultData = CreateCondition_CTS320(sParam);
                }
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
        public ActionResult CTS320_RetrieveCustomerCode(CTS320_GetCodeCondition param)
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
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001, null, null);
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
                        var saleObj = salehandler.GetTbt_SaleBasic(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG)
                            , null, true);

                        if (saleObj.Count == 1)
                        {
                            res.ResultData = util.ConvertCustCode(saleObj[0].RealCustomerCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        } else
                        {
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001, null, null);
                        }

                        //string lastOCC = salehandler.GetLastOCC(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                        //var contractSaleBasic = salehandler.GetSaleContractData(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), lastOCC);
                        //salehandler.GetTbt_SaleBasic(
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
        public ActionResult CTS320_RetrieveSiteCode(CTS320_GetCodeCondition param)
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
        /// Retrieve incident list from criteria when click [Retrieve] from any section
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CTS320_RetireveIncidentData(CTS320_IncidentCondition param)
        {
            ObjectResultData res = new ObjectResultData();
            CTS320_ViewResult result = new CTS320_ViewResult();
            doRetrieveIncidentListCondition doSearchcond = new doRetrieveIncidentListCondition();

            ICustomerMasterHandler customerhandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
            ISiteMasterHandler sitehandler = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;
            ICommonContractHandler commoncontracthandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
            IProjectHandler projecthandler = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;

            try
            {
                doSearchcond = CreateCondition_CTS320(param);
                bool isValid = true;

                
                
                // Check Existing Data
                if (doSearchcond.IncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CUSTOMER)
                {
                    if (CommonUtil.IsNullOrEmpty(doSearchcond.CustomerCode))
                    {
                        isValid = false;
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006, null, new string[] { "txtCustomerCode" });
                    }
                    else if (!customerhandler.CheckExistCustomerData(doSearchcond.CustomerCode))
                    {
                        isValid = false;
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { param.CustomerCode }, new string[] { "txtCustomerCode" });
                    }
                }
                else if (doSearchcond.IncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_SITE)
                {
                    if (CommonUtil.IsNullOrEmpty(doSearchcond.SiteCode))
                    {
                        isValid = false;
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006, null, new string[] { "txtSiteCode" });
                    }
                    else if (!sitehandler.CheckExistSiteData(doSearchcond.SiteCode, null))
                    {
                        isValid = false;
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { param.SiteCode }, new string[] { "txtSiteCode" });
                    }
                }
                else if (doSearchcond.IncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CONTRACT)
                {
                    if (CommonUtil.IsNullOrEmpty(doSearchcond.ContractCode))
                    {
                        isValid = false;
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006, null, new string[] { "txtContractCode" });
                    }
                    else if (!commoncontracthandler.IsContractExistInRentalOrSale(doSearchcond.ContractCode))
                    {
                        isValid = false;
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { param.ContractCode }, new string[] { "txtContractCode" });
                    }
                }
                else if (doSearchcond.IncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_PROJECT)
                {
                    var sRes = projecthandler.IsProjectExist(doSearchcond.ProjectCode);
                    if (CommonUtil.IsNullOrEmpty(doSearchcond.ProjectCode))
                    {
                        isValid = false;
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006, null, new string[] { "txtProjectCode" });
                    }
                    else if ((sRes.Count < 1) || !sRes[0].GetValueOrDefault())
                    {
                        isValid = false;
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { param.ProjectCode }, new string[] { "txtProjectCode" });
                    }
                } 
                
                if (isValid)
                {
                    var resLst = RetrieveIncident_CTS320(doSearchcond);
                    
                    //===================== TRS Check Permission toshow xxxxx data ======================
                    // Check Screen Permission
                    IIncidentHandler incidenthandler = ServiceContainer.GetService<IIncidentHandler>() as IIncidentHandler;
                    

                    //if (!CheckUserPermission(ScreenID.C_SCREEN_ID_VIEW_EDIT_INCIDENT) && resLst != null)
                    //{
                    //    foreach (CTS320_IncidentGridResult data in resLst)
                    //    {
                    //        data.DueDateDeadline = "XXXXX";
                    //        data.IncidentType = "XXXXX";
                    //        data.ControlChief = "XXXXX";
                    //        data.Correspondent = "XXXXX";
                    //        data.RegisterDate = "XXXXX";
                    //        data.CompleteDate = "XXXXX";
                    //        data.Status = "XXXXX";
                    //    }
                    //}
                    //else
                    //{
                        foreach (CTS320_IncidentGridResult data in resLst)
                        {
                            var incidentPermit = incidenthandler.HasIncidentPermission(Convert.ToInt32(data.IncidentID));
                            var incidentItem = incidenthandler.GetIncidentDetail(Convert.ToInt32(data.IncidentID));
                            bool isAdmin = HasAdminPermission_CTS320();
                            //if (!incidentPermit.ViewConfidentialIncidentFlag && (incidentItem.dtIncident.ConfidentialFlag == FlagType.C_FLAG_ON))
                            if (((incidentItem.dtIncident.ConfidentialFlag == FlagType.C_FLAG_ON) && (isAdmin || (incidentPermit.ViewConfidentialIncidentFlag)))
                            || (incidentItem.dtIncident.ConfidentialFlag != FlagType.C_FLAG_ON))
                            {

                            }
                            else
                            {
                                data.DueDateDeadline = "XXXXX";
                                data.IncidentType = "XXXXX";
                                data.ControlChief = "XXXXX";
                                data.Correspondent = "XXXXX";
                                data.RegisterDate = "XXXXX";
                                data.CompleteDate = "XXXXX";
                                data.Status = "XXXXX";
                            }
                            
                        }
                    //}
                    //===================================================================================

                    var resultXML = CommonUtil.ConvertToXml<CTS320_IncidentGridResult>(resLst, "Contract\\CTS320_Incident", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                    res.ResultData = resultXML;

                    //if (resLst.Count > 0)
                    //{
                    //    var resultXML = CommonUtil.ConvertToXml<CTS320_IncidentGridResult>(resLst, "Contract\\CTS320_Incident", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                    //    res.ResultData = resultXML;
                    //}
                    //else
                    //{
                    //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                    //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    //}
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                if (res.Message.Code == "MSG0006")
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            }

            return Json(res);
        }

        /// <summary>
        /// Retrieve incident occurring data from criteria
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CTS320_RetireveOccurringData(CTS320_IncidentCondition param)
        {
            ObjectResultData res = new ObjectResultData();
            CTS320_ViewResult result = new CTS320_ViewResult();
            doRetrieveIncidentListCondition doSearchcond = new doRetrieveIncidentListCondition();

            try
            {
                doSearchcond = CreateCondition_CTS320(param);

                if (doSearchcond.IncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CUSTOMER)
                {
                    var resObj = RetrieveOccurringSite_CTS320(doSearchcond);
                    //if (resObj != null)
                        res.ResultData = CommonUtil.ConvertToXml<CTS320_IncidentOccurringSiteGridResult>(resObj, "Contract\\CTS320_OccurringSite", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                }
                else if (doSearchcond.IncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_SITE)
                {
                    var resObj = RetrieveOccurringContract_CTS320(doSearchcond);
                    //if (resObj != null)
                        res.ResultData = CommonUtil.ConvertToXml<CTS320_IncidentOccurringContractGridResult>(resObj, "Contract\\CTS320_OccurringContract", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Retrieve incident list grid header (code: name)
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CTS320_RetrieveResultGridHeader(CTS320_IncidentCondition param)
        {
            ObjectResultData res = new ObjectResultData();
            CTS320_GridHeader obj = new CTS320_GridHeader();
            doRetrieveIncidentListCondition doSearchcond = new doRetrieveIncidentListCondition();

            ICustomerMasterHandler customerhandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
            ISiteMasterHandler sitehandler = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;
            ICommonContractHandler commoncontracthandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
            IProjectHandler projecthandler = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
            ISaleContractHandler salehandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
            IRentralContractHandler rentalhandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;

            try
            {
                doSearchcond = CreateCondition_CTS320(param);
                bool isValid = true;

                // Check Existing Data
                if (doSearchcond.IncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CUSTOMER)
                {
                    if (!customerhandler.CheckExistCustomerData(doSearchcond.CustomerCode))
                    {
                        isValid = false;
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { param.CustomerCode }, new string[] { "txtCustomerCode" });
                    }
                    else
                    {
                        var dataLst = (from a in customerhandler.GetCustomer(doSearchcond.CustomerCode) select new CTS320_GridHeader()
                            {
                                Code = param.CustomerCode
                                , NameEN = a.CustFullNameEN
                                , NameJP = a.CustFullNameEN
                                , NameLC = a.CustFullNameLC
                            }).ToList();

                        CommonUtil.MappingObjectLanguage<CTS320_GridHeader>(dataLst);

                        if (dataLst.Count == 1)
                        {
                            obj = dataLst[0];
                        }
                    }
                }
                else if (doSearchcond.IncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_SITE)
                {
                    if (!sitehandler.CheckExistSiteData(doSearchcond.SiteCode, null))
                    {
                        isValid = false;
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { param.SiteCode }, new string[] { "txtSiteCode" });
                    }
                    else
                    {
                        var dataLst = (from a in sitehandler.GetSite(doSearchcond.SiteCode, null)
                                       select new CTS320_GridHeader()
                                       {
                                           Code = param.SiteCode
                                           ,
                                           NameEN = a.SiteNameEN
                                           ,
                                           NameJP = a.SiteNameEN
                                           ,
                                           NameLC = a.SiteNameLC
                                       }).ToList();

                        CommonUtil.MappingObjectLanguage<CTS320_GridHeader>(dataLst);

                        if (dataLst.Count == 1)
                        {
                            obj = dataLst[0];
                        }
                    }
                }
                else if (doSearchcond.IncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CONTRACT)
                {
                    if (!commoncontracthandler.IsContractExistInRentalOrSale(doSearchcond.ContractCode))
                    {
                        isValid = false;
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { param.ContractCode }, new string[] { "txtContractCode" });
                    } else
                    {
                        List<CTS320_GridHeader> dataLst = new List<CTS320_GridHeader>();

                        var saleLst = salehandler.GetTbt_SaleBasicForView(doSearchcond.ContractCode, null, null);

                        string trueContractCode = "";
                        var rentalLst_Code = rentalhandler.GetTbt_RentalContractBasic(doSearchcond.ContractCode, null);
                        var rentalLst_UserCode = rentalhandler.GetTbt_RentalContractBasic(null, doSearchcond.ContractCode);

                        if (rentalLst_Code.Count == 1)
                        {
                            trueContractCode = rentalLst_Code[0].ContractCode;
                        }

                        var rentalLst = rentalhandler.GetTbt_RentalContractBasicForView(trueContractCode);

                        if (saleLst.Count > 0)
                        {
                            dataLst = (from a in saleLst
                                           select new CTS320_GridHeader()
                                           {
                                               Code = param.ContractCode,
                                               NameEN = a.PurCust_CustFullNameEN,
                                               NameJP = a.PurCust_CustFullNameEN,
                                               NameLC = a.PurCust_CustFullNameLC
                                           }).ToList();
                            CommonUtil.MappingObjectLanguage<CTS320_GridHeader>(dataLst);
                        } else if (rentalLst.Count > 0)
                        {
                            dataLst = (from a in rentalLst
                                           select new CTS320_GridHeader()
                                           {
                                               Code = param.ContractCode,
                                               NameEN = a.CustFullNameEN_Cust,
                                               NameJP = a.CustFullNameEN_Cust,
                                               NameLC = a.CustFullNameLC_Cust
                                           }).ToList();
                            CommonUtil.MappingObjectLanguage<CTS320_GridHeader>(dataLst);
                        }

                        if (dataLst.Count == 1)
                        {
                            obj = dataLst[0];
                        }
                    }
                }
                else if (doSearchcond.IncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_PROJECT)
                {
                    var sRes = projecthandler.IsProjectExist(doSearchcond.ProjectCode);
                    if ((sRes.Count < 1) || !sRes[0].GetValueOrDefault())
                    {
                        isValid = false;
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { param.ProjectCode }, new string[] { "txtProjectCode" });
                    }
                    else
                    {
                        var dataLst = (from a in projecthandler.GetTbt_Project(doSearchcond.ProjectCode)
                                       select new CTS320_GridHeader()
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

                if (isValid)
                {
                    res.ResultData = obj;
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        

        #endregion

        #region Method

        //private bool CheckUserPermission_CTS320()
        //{
        //    try
        //    {
        //        dsTransDataModel dsTrans = CommonUtil.dsTransData;
        //        if (dsTrans == null)
        //            return false;

        //        if (dsTrans.dtUserPermissionData != null)
        //        {
        //            var hasPermit = from a in dsTrans.dtUserPermissionData where a.Key.StartsWith(ScreenID.C_SCREEN_ID_INCIDENT_LIST) select a;
        //            return hasPermit.Count() > 0;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }

        //    return false;
        //}

        /// <summary>
        /// Checking user admin permission
        /// </summary>
        /// <returns></returns>
        private bool HasAdminPermission_CTS320()
        {
            return CheckUserPermission(ScreenID.C_SCREEN_ID_INCIDENT_LIST, FunctionID.C_FUNC_ID_SPECIAL_VIEW_CONFIDENTIAL);
        }

        /// <summary>
        /// Create search criteria condition from parameter
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private CTS320_IncidentCondition CreateCondition_CTS320(CTS320_ScreenParameter param)
        {
            CommonUtil util = new CommonUtil();

            CTS320_IncidentCondition newParam = new CTS320_IncidentCondition()
            {
                DuedateDeadline =  IncidentSearchDuedate.C_INCIDENT_SEARCH_DUEDATE_2WEEKS,
                IncidentStatus = IncidentSearchStatus.C_INCIDENT_SEARCH_STATUS_HANDLING
            };

            if (param.strIncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CUSTOMER)
            {
                newParam.IncidentMode = "1";
                newParam.IncidentType = null;
                //newParam.CustomerCode = util.ConvertCustCode(param.strIncidentRelevantCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                newParam.CustomerCode = param.strIncidentRelevantCode;
            }
            else if (param.strIncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_SITE)
            {
                newParam.IncidentMode = "2";
                newParam.IncidentType = null;
                //newParam.SiteCode = util.ConvertSiteCode(param.strIncidentRelevantCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                newParam.SiteCode = param.strIncidentRelevantCode;
            }
            else if (param.strIncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CONTRACT)
            {
                newParam.IncidentMode = "3";
                newParam.IncidentType = null;
                //newParam.ContractCode = util.ConvertContractCode(param.strIncidentRelevantCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                newParam.ContractCode = param.strIncidentRelevantCode;
            }
            else if (param.strIncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_PROJECT)
            {
                newParam.IncidentMode = "4";
                newParam.IncidentType = null;
                newParam.ProjectCode = param.strIncidentRelevantCode;
            }

            return newParam;
        }

        /// <summary>
        /// Create search criteria condition from screen
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private doRetrieveIncidentListCondition CreateCondition_CTS320(CTS320_IncidentCondition param)
        {
            doRetrieveIncidentListCondition resCond = new doRetrieveIncidentListCondition();
            CommonUtil util = new CommonUtil();

            if ((param != null) && (!String.IsNullOrEmpty(param.IncidentMode))
                    && ((!String.IsNullOrEmpty(param.CustomerCode)) || (!String.IsNullOrEmpty(param.SiteCode))
                    || (!String.IsNullOrEmpty(param.ContractCode)) || (!String.IsNullOrEmpty(param.ProjectCode))))
            {
                bool isValidData = false;

                if ((param.IncidentMode == "1") ) // Customer
                {
                    isValidData = true;
                    resCond = new doRetrieveIncidentListCondition()
                    {
                        CustomerCode = util.ConvertCustCode(param.CustomerCode, CommonUtil.CONVERT_TYPE.TO_LONG),
                        IncidentRelevantType = IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CUSTOMER,
                        IncidentType = param.IncidentType,
                        DuedateDeadline = (param.DuedateDeadline != null) ? ((param.DuedateDeadline == IncidentSearchDuedate.C_INCIDENT_SEARCH_DUEDATE_1MONTH) ? GetCurrentDateWithoutTime_CTS320().AddMonths(1) :
                            ((param.DuedateDeadline == IncidentSearchDuedate.C_INCIDENT_SEARCH_DUEDATE_2WEEKS) ? GetCurrentDateWithoutTime_CTS320().AddDays(14) :
                            ((param.DuedateDeadline == IncidentSearchDuedate.C_INCIDENT_SEARCH_DUEDATE_1WEEK) ? GetCurrentDateWithoutTime_CTS320().AddDays(7) : GetCurrentDateWithoutTime_CTS320()))) : (DateTime?)null,
                        IncidentStatus = param.IncidentStatus
                    };
                }
                else if ((param.IncidentMode == "2") ) // Site
                {
                    isValidData = true;
                    resCond = new doRetrieveIncidentListCondition()
                    {
                        SiteCode = util.ConvertSiteCode(param.SiteCode, CommonUtil.CONVERT_TYPE.TO_LONG),
                        IncidentRelevantType = IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_SITE,
                        IncidentType = param.IncidentType,
                        DuedateDeadline = (param.DuedateDeadline != null) ? ((param.DuedateDeadline == IncidentSearchDuedate.C_INCIDENT_SEARCH_DUEDATE_1MONTH) ? GetCurrentDateWithoutTime_CTS320().AddMonths(1) :
                            ((param.DuedateDeadline == IncidentSearchDuedate.C_INCIDENT_SEARCH_DUEDATE_2WEEKS) ? GetCurrentDateWithoutTime_CTS320().AddDays(14) :
                            ((param.DuedateDeadline == IncidentSearchDuedate.C_INCIDENT_SEARCH_DUEDATE_1WEEK) ? GetCurrentDateWithoutTime_CTS320().AddDays(7) : GetCurrentDateWithoutTime_CTS320()))) : (DateTime?)null,
                        IncidentStatus = param.IncidentStatus
                    };
                }
                else if ((param.IncidentMode == "3") ) // Contract
                {
                    isValidData = true;
                    resCond = new doRetrieveIncidentListCondition()
                    {
                        ContractCode = util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG),
                        IncidentRelevantType = IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CONTRACT,
                        IncidentType = param.IncidentType,
                        DuedateDeadline = (param.DuedateDeadline != null) ? ((param.DuedateDeadline == IncidentSearchDuedate.C_INCIDENT_SEARCH_DUEDATE_1MONTH) ? GetCurrentDateWithoutTime_CTS320().AddMonths(1) :
                            ((param.DuedateDeadline == IncidentSearchDuedate.C_INCIDENT_SEARCH_DUEDATE_2WEEKS) ? GetCurrentDateWithoutTime_CTS320().AddDays(14) :
                            ((param.DuedateDeadline == IncidentSearchDuedate.C_INCIDENT_SEARCH_DUEDATE_1WEEK) ? GetCurrentDateWithoutTime_CTS320().AddDays(7) : GetCurrentDateWithoutTime_CTS320()))) : (DateTime?)null,
                        IncidentStatus = param.IncidentStatus
                    };
                }
                else if ((param.IncidentMode == "4") ) // Project
                {
                    isValidData = true;
                    resCond = new doRetrieveIncidentListCondition()
                    {
                        ProjectCode = param.ProjectCode,
                        IncidentRelevantType = IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_PROJECT,
                        IncidentType = param.IncidentType,
                        DuedateDeadline = (param.DuedateDeadline != null) ? ((param.DuedateDeadline == IncidentSearchDuedate.C_INCIDENT_SEARCH_DUEDATE_1MONTH) ? GetCurrentDateWithoutTime_CTS320().AddMonths(1) :
                            ((param.DuedateDeadline == IncidentSearchDuedate.C_INCIDENT_SEARCH_DUEDATE_2WEEKS) ? GetCurrentDateWithoutTime_CTS320().AddDays(14) :
                            ((param.DuedateDeadline == IncidentSearchDuedate.C_INCIDENT_SEARCH_DUEDATE_1WEEK) ? GetCurrentDateWithoutTime_CTS320().AddDays(7) : GetCurrentDateWithoutTime_CTS320()))) : (DateTime?)null,
                        IncidentStatus = param.IncidentStatus
                    };
                }
            }

            return resCond;
        }

        /// <summary>
        /// Retrieve search incident list
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private List<CTS320_IncidentGridResult> RetrieveIncident_CTS320(doRetrieveIncidentListCondition param)
        {
            List<CTS320_IncidentGridResult> result = new List<CTS320_IncidentGridResult>();
            IIncidentHandler incidentHandler = null;
            CommonUtil util = new CommonUtil();
            List<bool> permissionList = new List<bool>();
            List<dtIncidentListCTS320> incidentResult;
            CommonUtil.LANGUAGE_LIST curLang;
            string strHeadCode = "";
            string strHeadName = "";

            try
            {
                curLang = CommonUtil.CurrentLanguage(false);
                incidentHandler = ServiceContainer.GetService<IIncidentHandler>() as IIncidentHandler;

                incidentResult = incidentHandler.GetIncidentList(param);
                permissionList = ValidatePermission_CTS320(incidentResult);

                //incidentResult = CreateDummyIncidentList(param);
                //permissionList = CreateDummyPermission_CTS320(incidentResult);

                if (incidentResult.Count > 0)
                {
                    //if (param.IncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CUSTOMER)
                    //{
                    //    strHeadCode = util.ConvertCustCode(incidentResult[0].CustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    //    strHeadName = (curLang == CommonUtil.LANGUAGE_LIST.LANGUAGE_1) ? incidentResult[0].CustFullNameEN : incidentResult[0].CustFullNameLC;
                    //}
                    //else if (param.IncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_SITE)
                    //{
                    //    strHeadCode = util.ConvertSiteCode(incidentResult[0].SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    //    strHeadName = (curLang == CommonUtil.LANGUAGE_LIST.LANGUAGE_1) ? incidentResult[0].SiteNameEN : incidentResult[0].SiteNameLC;
                    //}
                    //else if (param.IncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CONTRACT)
                    //{
                    //    strHeadCode = util.ConvertContractCode(incidentResult[0].ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    //    strHeadName = (curLang == CommonUtil.LANGUAGE_LIST.LANGUAGE_1) ? incidentResult[0].ContractTargetPurchaseCustFullNameEN : incidentResult[0].ContractTargetPurchaseCustFullNameLC;
                    //}
                    //else if (param.IncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_PROJECT)
                    //{
                    //    strHeadCode = incidentResult[0].ProjectCode;
                    //    strHeadName = incidentResult[0].ProjectName;
                    //} 

                    result = PharseOutput_CTS320(incidentResult, permissionList, strHeadCode, strHeadName);
                }
                //else
                //{
                //    throw new Exception(Common.Util.MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, "MSG0001", null).Message);
                //}
            }
            catch (Exception)
            {
                
                throw;
            }

            return result;
        }

        /// <summary>
        /// Retrieve search incident occurring site list
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private List<CTS320_IncidentOccurringSiteGridResult> RetrieveOccurringSite_CTS320(doRetrieveIncidentListCondition param)
        {
            List<CTS320_IncidentOccurringSiteGridResult> result = new List<CTS320_IncidentOccurringSiteGridResult>();
            IIncidentHandler incidenthandler = ServiceContainer.GetService<IIncidentHandler>() as IIncidentHandler;

            if (param.IncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CUSTOMER)
            {
                //var incidentList = incidenthandler.GetIncidentList(param);

                var occurringResult = incidenthandler.GetOccurringSiteList(param.CustomerCode);
                result = PharseOutput_CTS320(occurringResult);
            }

            return result;
        }

        /// <summary>
        /// Retrieve search incident occurring contract list
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private List<CTS320_IncidentOccurringContractGridResult> RetrieveOccurringContract_CTS320(doRetrieveIncidentListCondition param)
        {
            List<CTS320_IncidentOccurringContractGridResult> result = new List<CTS320_IncidentOccurringContractGridResult>();
            IIncidentHandler incidenthandler = ServiceContainer.GetService<IIncidentHandler>() as IIncidentHandler;

            if (param.IncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_SITE)
            {
                var occurringResult = incidenthandler.GetOccurringContractList(param.SiteCode);
                //var occurringResult = CreateDummyOccurringContractList(param);
                result = PharseOutput_CTS320(occurringResult);
            }

            return result;
        }

        //private CTS320_ViewResult RetrieveIncident_CTS320(doRetrieveIncidentListCondition param)
        //{
        //    CTS320_ViewResult result = new CTS320_ViewResult();
        //    IIncidentHandler incidentHandler = null;
        //    List<dtIncidentListCTS320> incidentResult = new List<dtIncidentListCTS320>();
        //    CommonUtil util = new CommonUtil();
        //    CommonUtil.LANGUAGE_LIST curLang;
        //    List<bool> permissionList = new List<bool>();

        //    try
        //    {
        //        curLang = CommonUtil.CurrentLanguage(false);
        //        incidentHandler = ServiceContainer.GetService<IIncidentHandler>() as IIncidentHandler;

        //        //incidentResult = incidentHandler.GetIncidentList(param);
        //        permissionList = ValidatePermission_CTS320(incidentResult);

        //        incidentResult = CreateDummyIncidentList(param);
        //        permissionList = CreateDummyPermission_CTS320(incidentResult);
        //        result.xmlIncidentGrid = CommonUtil.ConvertToXml<CTS320_IncidentGridResult>(PharseOutput_CTS320(incidentResult, permissionList), "Contract//CTS320_Incident");
        //        result.IncidentMode = (param.IncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CUSTOMER) ? "1" :
        //            (param.IncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_SITE) ? "2" :
        //            (param.IncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CONTRACT) ? "3" : "4";

        //        if (incidentResult.Count > 0)
        //        {
        //            if (param.IncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CUSTOMER)
        //            {
        //                //var occurringResult = incidentHandler.GetOccurringSiteList(incidentResult);
        //                var occurringResult = CreateDummyOccurringSiteList(param);
        //                result.xmlOccurringGrid = CommonUtil.ConvertToXml<CTS320_IncidentOccurringSiteGridResult>(PharseOutput_CTS320(occurringResult), "Contract//CTS320_OccurringSite");
        //            }
        //            else if (param.IncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_SITE)
        //            {
        //                //var occurringResult = incidentHandler.GetOccurringContractList(incidentResult);
        //                var occurringResult = CreateDummyOccurringContractList(param);
        //                result.xmlOccurringGrid = CommonUtil.ConvertToXml<CTS320_IncidentOccurringContractGridResult>(PharseOutput_CTS320(occurringResult), "Contract//CTS320_OccurringContract");
        //            }

        //            if (param.IncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CUSTOMER)
        //            {
        //                result.strHeadCode = util.ConvertCustCode(incidentResult[0].CustCode, CommonUtil.CONVERT_TYPE.TO_LONG);
        //                result.strHeadName = (curLang == CommonUtil.LANGUAGE_LIST.LANGUAGE_1) ? incidentResult[0].CustFullNameEN : incidentResult[0].CustFullNameLC;
        //            }
        //            else if (param.IncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_SITE)
        //            {
        //                result.strHeadCode = util.ConvertSiteCode(incidentResult[0].SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
        //                result.strHeadName = (curLang == CommonUtil.LANGUAGE_LIST.LANGUAGE_1) ? incidentResult[0].SiteNameEN : incidentResult[0].SiteNameLC;
        //            }
        //            else if (param.IncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CONTRACT)
        //            {
        //                result.strHeadCode = util.ConvertContractCode(incidentResult[0].ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
        //                result.strHeadName = (curLang == CommonUtil.LANGUAGE_LIST.LANGUAGE_1) ? incidentResult[0].ContractTargetPurchaseCustFullNameEN : incidentResult[0].ContractTargetPurchaseCustFullNameLC;
        //            }
        //            else if (param.IncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_PROJECT)
        //            {
        //                result.strHeadCode = util.ConvertProjectCode(incidentResult[0].ProjectCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
        //                result.strHeadName = incidentResult[0].ProjectName;
        //            } 

        //            result.IsValid = true;
        //        }
        //        else
        //        {
        //            throw new Exception(Common.Util.MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, "MSG0001", null).Message);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }

        //    return result;
        //}

        /// <summary>
        /// Validate user permission from incident list
        /// </summary>
        /// <param name="rawDat"></param>
        /// <returns></returns>
        private List<bool> ValidatePermission_CTS320(List<dtIncidentListCTS320> rawDat)
        {
            List<bool> result = new List<bool>();
            bool isAdmin = HasAdminPermission_CTS320();
            IIncidentHandler incidenthandler = ServiceContainer.GetService<IIncidentHandler>() as IIncidentHandler;

            foreach (dtIncidentListCTS320 item in rawDat)
            {
                doHasIncidentPermission incidentpermit = incidenthandler.HasIncidentPermission(item.IncidentID);

                //if (((item.ConfidentialFlag.GetValueOrDefault() == FlagType.C_FLAG_ON) &&
                //    (isAdmin || (incidentpermit.ViewConfidentialIncidentFlag)))
                //    || (!incidentpermit.ViewConfidentialIncidentFlag))
                //{
                //    result.Add(true);
                //}
                //else
                //{
                //    result.Add(false);
                //}
                if ( ((item.ConfidentialFlag.GetValueOrDefault() == FlagType.C_FLAG_ON) && (isAdmin || (incidentpermit.ViewConfidentialIncidentFlag)))
                    || (item.ConfidentialFlag.GetValueOrDefault() != FlagType.C_FLAG_ON))
                {
                    result.Add(true);
                }
                else
                {
                    result.Add(false);
                }
            }

            return result;
        }

        /// <summary>
        /// Create output display for each incident list item
        /// </summary>
        /// <param name="rawDat"></param>
        /// <param name="permitDat"></param>
        /// <param name="headCode"></param>
        /// <param name="headName"></param>
        /// <returns></returns>
        private List<CTS320_IncidentGridResult> PharseOutput_CTS320(List<dtIncidentListCTS320> rawDat, List<bool> permitDat, string headCode, string headName)
        {
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<CTS320_IncidentGridResult> result = new List<CTS320_IncidentGridResult>();

            List<string> miscFieldName = new List<string>();
            miscFieldName.Add(MiscType.C_DEADLINE_TIME_TYPE);
            miscFieldName.Add(MiscType.C_INCIDENT_STATUS);
            miscFieldName.Add(MiscType.C_INCIDENT_TYPE);

            var miscTypeCodeList = commonhandler.GetMiscTypeCodeListByFieldName(miscFieldName);

            if ((rawDat != null) && (permitDat != null) && (rawDat.Count == permitDat.Count))
            {
                for (int i = 0; i < rawDat.Count; i++)
                {
                    var item = rawDat[i];
                    var validItem = from a in result where a.IncidentNo == item.IncidentNo select a;

                    if (validItem.Count() == 0)
                    {
                        CommonUtil.MappingObjectLanguage(item);

                        //TimeSpan? duedateTime = (item.DueDateDeadLineTime.HasValue ? item.DueDateDeadLineTime.Value.TimeOfDay : (TimeSpan?)null);
                        TimeSpan? duedateTime = item.DueDateTime;
                        string deadlineTime = null;
                        string incidentStatusName = null;
                        string incidentTypeName = null;

                        if (!String.IsNullOrEmpty(item.DeadLineTime))
                        {
                            var displayTime = miscTypeCodeList.Where(x => x.ValueCode == item.DeadLineTime && x.FieldName == MiscType.C_DEADLINE_TIME_TYPE);
                            if (displayTime.Count() == 1)
                            {
                                deadlineTime = displayTime.First().ValueDisplay;
                            }
                        }

                        var statusName = miscTypeCodeList.Where(x => x.FieldName == MiscType.C_INCIDENT_STATUS && x.ValueCode == item.IncidentStatus);
                        if (statusName.Count() == 1)
                        {
                            incidentStatusName = statusName.First().ValueDisplay;
                        }

                        var typeName = miscTypeCodeList.Where(x => x.FieldName == MiscType.C_INCIDENT_TYPE && x.ValueCode == item.IncidentType);
                        if (typeName.Count() == 1)
                        {
                            incidentTypeName = typeName.First().ValueDisplay;
                        }

                        if (item.DueDateDeadLine.HasValue)
                        {
                            item.DueDateDeadLine = new DateTime(item.DueDateDeadLine.Value.Year
                                , item.DueDateDeadLine.Value.Month
                                , item.DueDateDeadLine.Value.Day
                                , 23, 59, 59);

                            if (item.DueDateTime.HasValue)
                            {
                                item.DueDateDeadLine = new DateTime(item.DueDateDeadLine.Value.Year
                                , item.DueDateDeadLine.Value.Month
                                , item.DueDateDeadLine.Value.Day
                                , item.DueDateTime.Value.Hours
                                , item.DueDateTime.Value.Minutes
                                , item.DueDateTime.Value.Seconds);
                            }
                        }

                        CTS320_IncidentGridResult tmp = new CTS320_IncidentGridResult()
                        {
                            HeadCode = headCode,
                            HeadName = headName,
                            CompleteDate = (permitDat[i]) ? (item.CompletedDate.HasValue) ? CommonUtil.TextDate(item.CompletedDate) : "-" : "XXXXX",
                            //ControlChief = (permitDat[i]) ? String.Format("{0} {1}", item.ConChiefEmpFirstName.Replace("^", "<br />"), item.ConChiefEmpLastName.Replace("^", "")) : "XXXXX",
                            //Correspondent = (permitDat[i]) ? String.Format("{0} {1}", item.CorrEmpFirstName.Replace("^", "<br />"), item.CorrEmpLastName.Replace("^", "")) : "XXXXX",
                            ControlChief = (permitDat[i]) ? (!CommonUtil.IsNullOrEmpty(item.ConChiefEmpName)) ? item.ConChiefEmpName.Replace("^", "<br />") : "" : "XXXXX",
                            Correspondent = (permitDat[i]) ? (!CommonUtil.IsNullOrEmpty(item.CorrEmpName)) ? item.CorrEmpName.Replace("^", "<br />") : "" : "XXXXX",
                            DueDateDeadline = (permitDat[i])
                                ? (item.DueDateDeadLine.HasValue && duedateTime.HasValue)
                                    ? String.Format("{0} <br /> {1}", CommonUtil.TextDate(item.DueDateDeadLine), CommonUtil.TextTime(duedateTime))
                                    : (item.DueDateDeadLine.HasValue && !String.IsNullOrEmpty(deadlineTime))
                                        ? String.Format("{0} <br /> {1}", CommonUtil.TextDate(item.DueDateDeadLine), deadlineTime)
                                        : "-"
                                : "XXXXX",
                            IncidentType = (permitDat[i]) ? incidentTypeName : "XXXXX",
                            IncidentFlag = item.ImportanceFlag.GetValueOrDefault() ? "★" : "",
                            IncidentID = item.IncidentID.ToString(),
                            IncidentNo = (String.IsNullOrEmpty(item.IncidentNo)) ? CommonUtil.GetLabelFromResource("Contract", "CTS320", "lblNA") : item.IncidentNo,
                            IncidentStatus = (DateTime.Now > (item.DueDateDeadLine.GetValueOrDefault()) && (item.IncidentStatus != IncidentStatus.C_INCIDENT_STATUS_COMPLETE)) ? "Over" : (!item.hasRespondingDetailFlag.GetValueOrDefault() ? "New" : ""),
                            RegisterDate = (permitDat[i]) ? item.ReceivedDate.HasValue ? CommonUtil.TextDate(item.ReceivedDate) : "" : "XXXXX",
                            Status = (permitDat[i]) ? String.Format("{0}<br />{1}", incidentStatusName
                            , CommonUtil.TextDate(item.LastUpdateDate)) : "XXXXX",
                        };

                        result.Add(tmp);
                    } else
                    {
                        if (permitDat[i])
                        {
                            validItem.ToList()[0].Correspondent += "<br />" + String.Format("{0} {1}", item.CorrEmpFirstName, item.CorrEmpLastName);
                        }
                        else
                        {
                            validItem.ToList()[0].Correspondent = "XXXXX";
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Create output display for each incident occurring site list item
        /// </summary>
        /// <param name="rawDat"></param>
        /// <returns></returns>
        private List<CTS320_IncidentOccurringSiteGridResult> PharseOutput_CTS320(List<dtIncidentOccSite> rawDat)
        {
            List<CTS320_IncidentOccurringSiteGridResult> result = new List<CTS320_IncidentOccurringSiteGridResult>();

            if (rawDat != null)
            {
                int cnt = 1;
                CommonUtil util = new CommonUtil();

                foreach (dtIncidentOccSite item in rawDat)
	            {
                    CTS320_IncidentOccurringSiteGridResult tmp = new CTS320_IncidentOccurringSiteGridResult()
                    {
                        Delay = (!item.CountDelayIncident.HasValue || (item.CountDelayIncident.Value == 0)) ? "-" : item.CountDelayIncident.Value.ToString(),
                        Incident = (!item.CountIncident.HasValue || (item.CountIncident.Value == 0)) ? "-" : item.CountIncident.Value.ToString(),
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
        /// Create output display for each incident occurring contract list item
        /// </summary>
        /// <param name="rawDat"></param>
        /// <returns></returns>
        private List<CTS320_IncidentOccurringContractGridResult> PharseOutput_CTS320(List<dtIncidentOccContract> rawDat)
        {
            List<CTS320_IncidentOccurringContractGridResult> result = new List<CTS320_IncidentOccurringContractGridResult>();

            if (rawDat != null)
            {
                int cnt = 1;
                CommonUtil util = new CommonUtil();

                foreach (dtIncidentOccContract item in rawDat)
                {
                    CTS320_IncidentOccurringContractGridResult tmp = new CTS320_IncidentOccurringContractGridResult()
                    {
                        Delay = (!item.CountDelayIncident.HasValue || (item.CountDelayIncident.Value == 0)) ? "-" : item.CountDelayIncident.Value.ToString(),
                        Incident = (!item.CountIncident.HasValue || (item.CountIncident.Value == 0)) ? "-" : item.CountIncident.Value.ToString(),
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
        private ObjectResultData ValidateAuthority_CTS320(ObjectResultData res)
        {
            if (!CheckUserPermission(ScreenID.C_SCREEN_ID_INCIDENT_LIST))
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
        private DateTime GetCurrentDateWithoutTime_CTS320()
        {
            var currDate = DateTime.Now;
            return new DateTime(currDate.Year, currDate.Month, currDate.Day);
        }

        //private void SetScreenParameter_CTS320(CTS320_ScreenParameter obj)
        //{
        //    Session.Remove("CTS320_PARAM");
        //    Session.Add("CTS320_PARAM", obj);
        //}

        //private CTS320_ScreenParameter GetScreenObject_CTS320()
        //{
        //    CTS320_ScreenParameter obj = null;

        //    if (Session["CTS320_PARAM"] != null)
        //    {
        //        obj = (CTS320_ScreenParameter)Session["CTS320_PARAM"];
        //    }

        //    return obj;
        //}

        #endregion

        #region Dummy Data

        //private List<bool> CreateDummyPermission_CTS320(List<dtIncidentListCTS320> rawDat)
        //{
        //    List<bool> result = new List<bool>();
        //    Random rand = new Random();

        //    foreach (var item in rawDat)
        //    {
        //        result.Add((rand.Next(0, 2) == 1) ? true : false);
        //    }

        //    return result;
        //}

        //private List<dtIncidentListCTS320> CreateDummyIncidentList(doRetrieveIncidentListCondition cont, int amount = 7)
        //{
        //    List<dtIncidentListCTS320> result = new List<dtIncidentListCTS320>();
        //    IContractHandler contracthandler = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
        //    ICustomerMasterHandler customerhandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
        //    ISiteMasterHandler sitehandler = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;
        //    IProjectHandler projecthandler = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
        //    ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
        //    Random rand = new Random();

        //    string dummieCode = "";
        //    string dummieNameLC = "", dummieNameEN = "";

        //    if (cont.IncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CUSTOMER)
        //    {
        //        dummieCode = "C0000000125";
        //        dummieNameEN = "BANGKOK UNIVERSITY";
        //        dummieNameLC = "มหาวิทยาลัยกรุงเทพ";

        //        var firstIncidentNo = "";

        //        for (int i = 0; i < amount; i++)
        //        {
        //            dtIncidentListCTS320 tmpdat = new dtIncidentListCTS320()
        //            {
        //                AsstEmpFirstNameEN = "AsstEmpFirstNameEN",
        //                AsstEmpFirstNameLC = "AsstEmpFirstNameLC",
        //                AsstEmpLastNameEN = "AsstEmpLastNameEN",
        //                AsstEmpLastNameLC = "AsstEmpLastNameLC",
        //                AsstEmpNo = "111222",
        //                ChiefEmpFirstNameEN = "ChiefEmpFirstNameEN",
        //                ChiefEmpFirstNameLC = "ChiefEmpFirstNameLC",
        //                ChiefEmpLastNameEN = "ChiefEmpLastNameEN",
        //                ChiefEmpLastNameLC = "ChiefEmpLastNameLC",
        //                ChiefEmpNo = "333444",
        //                CompletedDate = (rand.Next(0, 2) == 1) ? (DateTime?)null : DateTime.Now.AddDays(rand.Next(-5, 5)),
        //                ConChiefEmpFirstNameEN = "ConChiefEmpFirstNameEN",
        //                ConChiefEmpFirstNameLC = "ConChiefEmpFirstNameLC",
        //                ConChiefEmpLastNameEN = "ConChiefEmpLastNameEN",
        //                ConChiefEmpLastNameLC = "ConChiefEmpLastNameLC",
        //                ConChiefEmpNo = "555666",
        //                ConfidentialFlag = false,
        //                ContactPerson = "ContractPerson",
        //                ContactPersonDep = "DEV-DNA",
        //                ContractCode = "777888",
        //                ContractTargetPurchaseCustFullNameEN = "ContractTargetPurchaseCustFullNameEN",
        //                ContractTargetPurchaseCustFullNameLC = "ContractTargetPurchaseCustFullNameLC",
        //                CorrEmpFirstNameEN = "CorrEmpFirstNameEN",
        //                CorrEmpFirstNameLC = "CorrEmpFirstNameLC",
        //                CorrEmpLastNameEN = "CorrEmpLastNameEN",
        //                CorrEmpLastNameLC = "CorrEmpLastNameLC",
        //                CorrEmpNo = "999000",
        //                CustCode = dummieCode,
        //                CustFullNameEN = dummieNameEN,
        //                CustFullNameLC = dummieNameLC,
        //                DeadLineTime = "09:00",
        //                DueDateDeadLine = DateTime.Now.AddDays(rand.Next(-2, 5)),
        //                DueDateDeadLineTime = DateTime.Now,
        //                DueDateTime = new TimeSpan(13, 12, 11),
        //                hasRespondingDetailFlag = (rand.Next(0, 2) == 1) ? true : false,
        //                ImportanceFlag = (rand.Next(0, 2) == 1) ? true : false,
        //                IncidentID = i,
        //                IncidentNo = rand.Next(100000, 999999).ToString() + rand.Next(100000, 999999).ToString(),
        //                IncidentType = IncidentType.C_INCIDENT_TYPE_CLAIM,
        //                IncidentTypeNameEN = "",
        //                IncidentTypeNameJP = "",
        //                IncidentTypeNameLC = "",
        //                ReceivedDate = DateTime.Now.AddDays(rand.Next(-5, -2)),
        //                IncidentStatusCode = (cont.IncidentStatus == IncidentSearchStatus.C_INCIDENT_SEARCH_STATUS_COMPLETE) ? IncidentStatus.C_INCIDENT_STATUS_COMPLETE : IncidentStatus.C_INCIDENT_STATUS_WAIT_FOR_COMPLETE_APPROVAL,
        //            };

        //            if (i == 0)
        //            {
        //                firstIncidentNo = tmpdat.IncidentNo;
        //            }

        //            if ((i + 1) == amount)
        //            {
        //                tmpdat.IncidentNo = firstIncidentNo;
        //            }

        //            List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
        //            {
        //                new doMiscTypeCode()
        //                {
        //                    FieldName = MiscType.C_INCIDENT_TYPE,
        //                    ValueCode = tmpdat.IncidentType
        //                }
        //            };

        //            var incidenttypename = commonhandler.GetMiscTypeCodeList(miscs);
        //            if (incidenttypename.Count > 0)
        //            {
        //                tmpdat.IncidentTypeNameEN = incidenttypename[0].ValueDisplayEN;
        //                tmpdat.IncidentTypeNameJP = incidenttypename[0].ValueDisplayJP;
        //                tmpdat.IncidentTypeNameLC = incidenttypename[0].ValueDisplayLC;
        //            }

        //            miscs = new List<doMiscTypeCode>()
        //            {
        //                new doMiscTypeCode()
        //                {
        //                    FieldName = MiscType.C_INCIDENT_STATUS,
        //                    ValueCode = tmpdat.IncidentStatusCode
        //                }
        //            };

        //            var incidentstatusname = commonhandler.GetMiscTypeCodeList(miscs);
        //            if (incidentstatusname.Count > 0)
        //            {
        //                tmpdat.IncidentStatusNameEN = incidentstatusname[0].ValueDisplayEN;
        //                tmpdat.IncidentStatusNameJP = incidentstatusname[0].ValueDisplayJP;
        //                tmpdat.IncidentStatusNameLC = incidentstatusname[0].ValueDisplayLC;
        //            }

        //            result.Add(tmpdat);
        //        }
        //    }
        //    else if (cont.IncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_SITE)
        //    {
        //        dummieCode = "S9999999999-0001";
        //        dummieNameEN = "CSI ASIA Co., LTD.";
        //        dummieNameLC = "บริษัท ซีเอสไอ เอเชีย จำกัด";

        //        for (int i = 0; i < amount; i++)
        //        {
        //            dtIncidentListCTS320 tmpdat = new dtIncidentListCTS320()
        //            {
        //                AsstEmpFirstNameEN = "AsstEmpFirstNameEN",
        //                AsstEmpFirstNameLC = "AsstEmpFirstNameLC",
        //                AsstEmpLastNameEN = "AsstEmpLastNameEN",
        //                AsstEmpLastNameLC = "AsstEmpLastNameLC",
        //                AsstEmpNo = "111222",
        //                ChiefEmpFirstNameEN = "ChiefEmpFirstNameEN",
        //                ChiefEmpFirstNameLC = "ChiefEmpFirstNameLC",
        //                ChiefEmpLastNameEN = "ChiefEmpLastNameEN",
        //                ChiefEmpLastNameLC = "ChiefEmpLastNameLC",
        //                ChiefEmpNo = "333444",
        //                CompletedDate = null,
        //                ConChiefEmpFirstNameEN = "ConChiefEmpFirstNameEN",
        //                ConChiefEmpFirstNameLC = "ConChiefEmpFirstNameLC",
        //                ConChiefEmpLastNameEN = "ConChiefEmpLastNameEN",
        //                ConChiefEmpLastNameLC = "ConChiefEmpLastNameLC",
        //                ConChiefEmpNo = "555666",
        //                ConfidentialFlag = false,
        //                ContactPerson = "ContractPerson",
        //                ContactPersonDep = "DEV-DNA",
        //                ContractCode = "777888",
        //                ContractTargetPurchaseCustFullNameEN = "ContractTargetPurchaseCustFullNameEN",
        //                ContractTargetPurchaseCustFullNameLC = "ContractTargetPurchaseCustFullNameLC",
        //                CorrEmpFirstNameEN = "CorrEmpFirstNameEN",
        //                CorrEmpFirstNameLC = "CorrEmpFirstNameLC",
        //                CorrEmpLastNameEN = "CorrEmpLastNameEN",
        //                CorrEmpLastNameLC = "CorrEmpLastNameLC",
        //                CorrEmpNo = "999000",
        //                //CustCode = dummieCode,
        //                //CustFullNameEN = dummieNameEN,
        //                //CustFullNameLC = dummieNameLC,
        //                SiteCode = dummieCode,
        //                SiteNameEN = dummieNameEN,
        //                SiteNameLC = dummieNameLC,
        //                DeadLineTime = "09:00",
        //                DueDateDeadLine = DateTime.Now.AddDays(rand.Next(-2, 5)),
        //                DueDateDeadLineTime = DateTime.Now,
        //                DueDateTime = new TimeSpan(13, 12, 11),
        //                hasRespondingDetailFlag = (rand.Next(0, 2) == 1) ? true : false,
        //                ImportanceFlag = (rand.Next(0, 2) == 1) ? true : false,
        //                IncidentID = i,
        //                IncidentNo = rand.Next(100000, 999999).ToString() + rand.Next(100000, 999999).ToString(),
        //                IncidentType = IncidentType.C_INCIDENT_TYPE_CLAIM,
        //                ReceivedDate = DateTime.Now.AddDays(rand.Next(-5, -2)),
        //            };

        //            result.Add(tmpdat);
        //        }
        //    }
        //    else if (cont.IncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CONTRACT)
        //    {
        //        dummieCode = "N0000000001";
        //        dummieNameEN = "CSI ASIA Co., LTD.";
        //        dummieNameLC = "บริษัท ซีเอสไอ เอเชีย จำกัด";

        //        for (int i = 0; i < amount; i++)
        //        {
        //            dtIncidentListCTS320 tmpdat = new dtIncidentListCTS320()
        //            {
        //                AsstEmpFirstNameEN = "AsstEmpFirstNameEN",
        //                AsstEmpFirstNameLC = "AsstEmpFirstNameLC",
        //                AsstEmpLastNameEN = "AsstEmpLastNameEN",
        //                AsstEmpLastNameLC = "AsstEmpLastNameLC",
        //                AsstEmpNo = "111222",
        //                ChiefEmpFirstNameEN = "ChiefEmpFirstNameEN",
        //                ChiefEmpFirstNameLC = "ChiefEmpFirstNameLC",
        //                ChiefEmpLastNameEN = "ChiefEmpLastNameEN",
        //                ChiefEmpLastNameLC = "ChiefEmpLastNameLC",
        //                ChiefEmpNo = "333444",
        //                CompletedDate = null,
        //                ConChiefEmpFirstNameEN = "ConChiefEmpFirstNameEN",
        //                ConChiefEmpFirstNameLC = "ConChiefEmpFirstNameLC",
        //                ConChiefEmpLastNameEN = "ConChiefEmpLastNameEN",
        //                ConChiefEmpLastNameLC = "ConChiefEmpLastNameLC",
        //                ConChiefEmpNo = "555666",
        //                ConfidentialFlag = false,
        //                ContactPerson = "ContractPerson",
        //                ContactPersonDep = "DEV-DNA",
        //                ContractCode = dummieCode,
        //                ContractTargetPurchaseCustFullNameEN = dummieNameEN,
        //                ContractTargetPurchaseCustFullNameLC = dummieNameLC,
        //                CorrEmpFirstNameEN = "CorrEmpFirstNameEN",
        //                CorrEmpFirstNameLC = "CorrEmpFirstNameLC",
        //                CorrEmpLastNameEN = "CorrEmpLastNameEN",
        //                CorrEmpLastNameLC = "CorrEmpLastNameLC",
        //                CorrEmpNo = "999000",

        //                DeadLineTime = "09:00",
        //                DueDateDeadLine = DateTime.Now.AddDays(rand.Next(-2, 5)),
        //                DueDateDeadLineTime = DateTime.Now,
        //                DueDateTime = new TimeSpan(13, 12, 11),
        //                hasRespondingDetailFlag = (rand.Next(0, 2) == 1) ? true : false,
        //                ImportanceFlag = (rand.Next(0, 2) == 1) ? true : false,
        //                IncidentID = i,
        //                IncidentNo = rand.Next(100000, 999999).ToString() + rand.Next(100000, 999999).ToString(),
        //                IncidentType = IncidentType.C_INCIDENT_TYPE_CLAIM,
        //                ReceivedDate = DateTime.Now.AddDays(rand.Next(-5, -2)),
        //            };

        //            result.Add(tmpdat);
        //        }
        //    }
        //    else if (cont.IncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_PROJECT)
        //    {
        //        dummieCode = projecthandler.GenerateProjectCode();
        //        dummieNameEN = "Project Diva";
        //        dummieNameLC = dummieNameEN;

        //        for (int i = 0; i < amount; i++)
        //        {
        //            dtIncidentListCTS320 tmpdat = new dtIncidentListCTS320()
        //            {
        //                AsstEmpFirstNameEN = "AsstEmpFirstNameEN",
        //                AsstEmpFirstNameLC = "AsstEmpFirstNameLC",
        //                AsstEmpLastNameEN = "AsstEmpLastNameEN",
        //                AsstEmpLastNameLC = "AsstEmpLastNameLC",
        //                AsstEmpNo = "111222",
        //                ChiefEmpFirstNameEN = "ChiefEmpFirstNameEN",
        //                ChiefEmpFirstNameLC = "ChiefEmpFirstNameLC",
        //                ChiefEmpLastNameEN = "ChiefEmpLastNameEN",
        //                ChiefEmpLastNameLC = "ChiefEmpLastNameLC",
        //                ChiefEmpNo = "333444",
        //                CompletedDate = null,
        //                ConChiefEmpFirstNameEN = "ConChiefEmpFirstNameEN",
        //                ConChiefEmpFirstNameLC = "ConChiefEmpFirstNameLC",
        //                ConChiefEmpLastNameEN = "ConChiefEmpLastNameEN",
        //                ConChiefEmpLastNameLC = "ConChiefEmpLastNameLC",
        //                ConChiefEmpNo = "555666",
        //                ConfidentialFlag = false,
        //                ContactPerson = "ContractPerson",
        //                ContactPersonDep = "DEV-DNA",
        //                ContractCode = "777888",
        //                ContractTargetPurchaseCustFullNameEN = "ContractTargetPurchaseCustFullNameEN",
        //                ContractTargetPurchaseCustFullNameLC = "ContractTargetPurchaseCustFullNameLC",
        //                CorrEmpFirstNameEN = "CorrEmpFirstNameEN",
        //                CorrEmpFirstNameLC = "CorrEmpFirstNameLC",
        //                CorrEmpLastNameEN = "CorrEmpLastNameEN",
        //                CorrEmpLastNameLC = "CorrEmpLastNameLC",
        //                CorrEmpNo = "999000",
        //                ProjectCode = dummieCode,
        //                ProjectName = dummieNameEN,
        //                DeadLineTime = "09:00",
        //                DueDateDeadLine = DateTime.Now.AddDays(rand.Next(-2, 5)),
        //                DueDateDeadLineTime = DateTime.Now,
        //                DueDateTime = new TimeSpan(13, 12, 11),
        //                hasRespondingDetailFlag = (rand.Next(0, 2) == 1) ? true : false,
        //                ImportanceFlag = (rand.Next(0, 2) == 1) ? true : false,
        //                IncidentID = i,
        //                IncidentNo = rand.Next(100000, 999999).ToString() + rand.Next(100000, 999999).ToString(),
        //                IncidentType = IncidentType.C_INCIDENT_TYPE_CLAIM,
        //                ReceivedDate = DateTime.Now.AddDays(rand.Next(-5, -2)),
        //            };

        //            result.Add(tmpdat);
        //        }
        //    }

        //    return result;
        //}

        //private List<dtIncidentOccSite> CreateDummyOccurringSiteList(doRetrieveIncidentListCondition cont, int amount = 7)
        //{
        //    List<dtIncidentOccSite> result = new List<dtIncidentOccSite>();
        //    Random rand = new Random();

        //    for (int i = 0; i < amount; i++)
        //    {
        //        dtIncidentOccSite temp = new dtIncidentOccSite()
        //        {
        //            CountDelayIncident = rand.Next(0, 13),
        //            CountIncident = rand.Next(0, 21),
        //            SiteCode = rand.Next(1, 9999).ToString("0000") + rand.Next(1, 9999).ToString("0000") + "-" + (i + 1).ToString("0000"),
        //            SiteNameEN = "SiteName EN",
        //            SiteNameLC = "SiteName LC"
        //        };

        //        result.Add(temp);
        //    }

        //    return result;
        //}

        //private List<dtIncidentOccContract> CreateDummyOccurringContractList(doRetrieveIncidentListCondition cont, int amount = 7)
        //{
        //    List<dtIncidentOccContract> result = new List<dtIncidentOccContract>();
        //    Random rand = new Random();

        //    for (int i = 0; i < amount; i++)
        //    {
        //        dtIncidentOccContract temp = new dtIncidentOccContract()
        //        {
        //            CountDelayIncident = rand.Next(0, 13),
        //            CountIncident = rand.Next(0, 21),
        //            ContractCode = "N" + rand.Next(1000, 9999).ToString() + rand.Next(111, 999).ToString(),
        //            ContractTargetFullNameEN = "ContractName EN",
        //            ContractTargetFullNameLC = "ContractName LC"
        //        };

        //        result.Add(temp);
        //    }

        //    return result;
        //}

        #endregion
    }
}
