//*********************************
// Create by: 
// Create date: /Jun/2010
// Update date: /Jun/2010
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
using SECOM_AJIS.Common.Models;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Contract.CustomEntity;
using SECOM_AJIS.Presentation.Contract.Models;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Common;

namespace SECOM_AJIS.Presentation.Contract.Controllers
{
    public partial class ContractController : BaseController
    {
        #region Authority

        /// <summary>
        /// Check screen authority and permission
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CTS030_Authority(CTS030_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                #region Check Permission

                IApprovalPermissionHandler handler = ServiceContainer.GetService<IApprovalPermissionHandler>() as IApprovalPermissionHandler;

                //bool bIsPermitted = handler.isPermittedIPAddress();
                bool bIsPermitted = isPermittedIPAddress();
                if (!bIsPermitted || !CheckUserPermission(ScreenID.C_SCREEN_ID_SEARCH_APPROVE, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                #endregion
                #region Check is Suspending

                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (chandler.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                #endregion
                #region Get approval status misc

                IMasterHandler mHand = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                List<tbs_MiscellaneousTypeCode> appvList = mHand.GetTbs_MiscellaneousTypeCode(MiscType.C_APPROVE_STATUS);
                CommonUtil.MappingObjectLanguage<tbs_MiscellaneousTypeCode>(appvList);
                param.approvalStatus = appvList;
                
                #endregion
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<CTS030_ScreenParameter>("CTS030", param, res);
        }

        #endregion
        #region Views

        /// <summary>
        /// Generate screen
        /// </summary>
        /// <param name="caller"></param>
        /// <returns></returns>
        [Initialize("CTS030")]
        public ActionResult CTS030(string caller)
        {
            try
            {
                CTS030_ScreenParameter CTS030Param = GetScreenObject<CTS030_ScreenParameter>();
                if (CTS030Param == null)
                    CTS030Param = new CTS030_ScreenParameter();

                #region Clear data if not return from child page

                if (CTS030Param.CallerScreenID != ScreenID.C_SCREEN_ID_FN99 //CTS010
                    && CTS030Param.CallerScreenID != ScreenID.C_SCREEN_ID_FQ99) //CTS020
                {
                    CTS030Param.data = null;
                }

                #endregion

                dsCTS030Data data = CTS030Param.data;
                if (data != null)
                {
                    //Convert result to current language
                    CommonUtil.MappingObjectLanguage<dtSearchDraftContractResult>(data.dtSearchResult);

                    //Clear isAuditBtnClick flag to clear session after come back from child page
                    CTS030Param.isAuditBtnClick = false;

                    CommonUtil c = new CommonUtil();
                    if (CommonUtil.IsNullOrEmpty(data.doSearchCondition.QuotationCode) == false)
                        ViewBag.QuotationCode = c.ConvertContractCode(data.doSearchCondition.QuotationCode, CommonUtil.CONVERT_TYPE.TO_SHORT);

                    ViewBag.Alphabet = data.doSearchCondition.Alphabet ?? "";
                    ViewBag.RegistrationDateFrom = (data.doSearchCondition.RegistrationDateFrom == null) ? "" : data.doSearchCondition.RegistrationDateFrom.Value.ToString("dd-MMM-yyyy");
                    ViewBag.RegistrationDateTo = (data.doSearchCondition.RegistrationDateTo == null) ? "" : data.doSearchCondition.RegistrationDateTo.Value.ToString("dd-MMM-yyyy");
                    ViewBag.Salesman1Code = data.doSearchCondition.Salesman1Code ?? "";
                    ViewBag.Salesman1Name = data.doSearchCondition.Salesman1Name ?? "";
                    ViewBag.ContractTargetName = data.doSearchCondition.ContractTargetName ?? "";
                    ViewBag.SiteName = data.doSearchCondition.SiteName ?? "";
                    ViewBag.ContractOfficeCode = (data.doSearchCondition.ContractOfficeCode == null || data.doSearchCondition.ContractOfficeCode.Contains(",")) ? "" : data.doSearchCondition.ContractOfficeCode;
                    ViewBag.OperationOfficeCode = (data.doSearchCondition.OperationOfficeCode == null || data.doSearchCondition.OperationOfficeCode.Contains(",")) ? "" : data.doSearchCondition.OperationOfficeCode;
                    ViewBag.CurrentIndex = CTS030Param.CurrentIndex;
                    ViewBag.CurrentSortColIndex = CTS030Param.CurrentSortColIndex;
                    ViewBag.CurrentSortType = CTS030Param.CurrentSortType;
                    ViewBag.HasSessionData = true;

                }
                else
                {
                    ViewBag.CurrentPage = 0;
                    ViewBag.HasSessionData = false;
                }
                ViewBag.AuditLabel = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, ScreenID.C_SCREEN_ID_SEARCH_APPROVE, "headerAudit");
                ViewBag.PageRow = CommonValue.ROWS_PER_PAGE_FOR_SEARCHPAGE;

                #region Update data

                UpdateScreenObject(CTS030Param);

                #endregion
            }
            catch (Exception)
            {
            }

            return View("CTS030");
        }

        #endregion
        #region Actions

        /// <summary>
        /// Initial grid
        /// </summary>
        /// <returns></returns>
        public ActionResult InitialGrid_CTS030()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Contract\\CTS030", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }
        /// <summary>
        /// Check required field
        /// </summary>
        /// <param name="QuotationCode"></param>
        /// <param name="Alphabet"></param>
        /// <returns></returns>
        public ActionResult CTS030_CheckReqField(string QuotationCode, string Alphabet)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                if (!CommonUtil.IsNullOrEmpty(Alphabet) && CommonUtil.IsNullOrEmpty(QuotationCode))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                        ScreenID.C_SCREEN_ID_SEARCH_APPROVE,
                                        MessageUtil.MODULE_COMMON,
                                        MessageUtil.MessageList.MSG0007,
                                        new string[] { "lblQuotationTargetCode" },
                                        new string[] { "QuotationCode" });
                    return Json(res);
                }

                res.ResultData = true;

                #region Clear data in session when click on search button

                CTS030_ScreenParameter CTS030Param = GetScreenObject<CTS030_ScreenParameter>();
                CTS030Param.data = null;
                UpdateScreenObject(CTS030Param); //clear search data

                #endregion
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Load search result to grid
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CTS030_Search(CTS030_Search cond)
        {
            List<dtSearchDraftContractResult> list = new List<dtSearchDraftContractResult>();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                ValidatorUtil.BuildErrorMessage(res, this, new object[] { cond }); //AtLeast1FieldNotNullOrEmptyAttribute
                if (res.IsError)
                    return Json(res);

                CTS030_ScreenParameter CTS030Param = GetScreenObject<CTS030_ScreenParameter>();
                if (CTS030Param == null)
                    CTS030Param = new CTS030_ScreenParameter();

                dsCTS030Data data = CTS030Param.data;
                if (data == null)
                {
                    //Save search condition value
                    dsCTS030Data dsData = new dsCTS030Data();
                    dsData.doSearchCondition = cond;

                    //Set default to some search condition
                    CommonUtil c = new CommonUtil();
                    cond.QuotationCode = c.ConvertQuotationTargetCode(cond.QuotationCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                    cond.ApproveContractStatus = ApprovalStatus.C_APPROVE_STATUS_WAITFORAPPROVE;

                    //Query for draft contract
                    IDraftContractHandler hand = ServiceContainer.GetService<IDraftContractHandler>() as IDraftContractHandler;
                    list = hand.SearchDraftContractList(cond);

                    //Save search result list
                    dsData.dtSearchResult = list;

                    //Save condition and result into session
                    CTS030Param.data = dsData;
                    UpdateScreenObject(CTS030Param);
                }
                else
                {
                    list = data.dtSearchResult;
                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            res.ResultData = CommonUtil.ConvertToXml<dtSearchDraftContractResult>(list, "Contract\\CTS030", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            return Json(res);
        }
        /// <summary>
        /// Update button status to session
        /// </summary>
        /// <param name="CurrentIndex"></param>
        /// <param name="list"></param>
        /// <param name="CurrentSortColIndex"></param>
        /// <param name="CurrentSortType"></param>
        /// <returns></returns>
        public ActionResult CTS030_SetAuditBtnClickFlag(int CurrentIndex, List<CTS030_CheckResultItem> list, int CurrentSortColIndex, string CurrentSortType)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                CTS030_ScreenParameter CTS030Param = GetScreenObject<CTS030_ScreenParameter>();
                CTS030Param.isAuditBtnClick = true;
                CTS030Param.CurrentIndex = CurrentIndex;
                CTS030Param.CurrentSortColIndex = CurrentSortColIndex;
                CTS030Param.CurrentSortType = CurrentSortType;

                List<dtSearchDraftContractResult> nSort = new List<dtSearchDraftContractResult>();
                foreach (CTS030_CheckResultItem ma in list)
                {
                    foreach (dtSearchDraftContractResult r in CTS030Param.data.dtSearchResult)
                    {
                        if (r.KeyIndex == ma.KeyIndex)
                        {
                            nSort.Add(r);
                            break;
                        }
                    }
                }
                CTS030Param.data.dtSearchResult = nSort;

                UpdateScreenObject(CTS030Param);
                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        #endregion
        #region Methods

        /// <summary>
        /// Clear data in session
        /// </summary>
        public void CTS030_ClearSession()
        {
            try
            {
                CTS030_ScreenParameter CTS030Param = GetScreenObject<CTS030_ScreenParameter>();
                if (!CTS030Param.isAuditBtnClick)
                    UpdateScreenObject(null);
            }
            catch (Exception)
            {
            }
        }
        /// <summary>
        /// Update selected data to session
        /// </summary>
        /// <param name="callerSessionKey"></param>
        /// <param name="quotationTargetCode"></param>
        /// <param name="contractCode"></param>
        /// <param name="approvalStatusCode"></param>
        public void CTS030_UpdateDataFromChildPage(string callerSessionKey, string quotationTargetCode, string contractCode, string approvalStatusCode)
        {
            try
            {
                CTS030_ScreenParameter CTS030Param = GetScreenObject<CTS030_ScreenParameter>(callerSessionKey);
                if (CTS030Param != null)
                {
                    if (CTS030Param.data != null)
                    {
                        var objects = from l in CTS030Param.data.dtSearchResult
                                      where l.QuotationTargetCode == quotationTargetCode
                                      select l;

                        foreach (dtSearchDraftContractResult obj in objects)
                        {
                            obj.ContractCode = contractCode;
                            obj.ApprovalStatusCode = approvalStatusCode;

                            if (!CommonUtil.IsNullOrEmpty(approvalStatusCode))
                            {
                                //Convert approvalStatusName to current language
                                CommonUtil.MappingObjectLanguage<tbs_MiscellaneousTypeCode>(CTS030Param.approvalStatus);
                                tbs_MiscellaneousTypeCode misc = CTS030Param.approvalStatus.Find(i => i.ValueCode == approvalStatusCode);
                                if (!CommonUtil.IsNullOrEmpty(misc))
                                {
                                    obj.ApprovalStatusName = misc.ValueDisplay;
                                    obj.ApprovalStatusNameEN = misc.ValueDisplayEN;
                                    obj.ApprovalStatusNameJP = misc.ValueDisplayJP;
                                    obj.ApprovalStatusNameLC = misc.ValueDisplayLC;
                                }
                            }
                        }

                        CTS030Param.IsLoaded = false;

                        ScreenParameter oparam = (ScreenParameter)GetScreenObject<object>();
                        if (oparam != null)
                        {
                            CTS030Param.CallerScreenID = oparam.ScreenID;
                            CTS030Param.CallerModule = oparam.Module;
                            CTS030Param.CallerKey = oparam.Key;
                            CTS030Param.BackStep = true;
                        }

                        UpdateScreenObject(CTS030Param, callerSessionKey);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        /// <summary>
        /// Check IP address is permitted
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult isPermittedIPAddress(string cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                //IApprovalPermissionHandler handler = ServiceContainer.GetService<IApprovalPermissionHandler>() as IApprovalPermissionHandler;
                //bool bIsPermitted = handler.isPermittedIPAddress();
                //return Json(bIsPermitted);
                return Json(isPermittedIPAddress());
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }
        /// <summary>
        /// Check IP address is permitted
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public bool isPermittedIPAddress()
        {
            try
            {
                //1.	Load the list of permitted IP Address from config file
                //--Load permitted ip list from config file
                //string filePath = string.Format("{0}{1}\\{2}.xml",
                //                                CommonUtil.WebPath,
                //                                CommonValue.PERMITTED_IPADDR_FOLDER,
                //                                CommonValue.PERMITTED_IPADDR_FILE);
                //XmlDocument xmldoc = new XmlDocument();
                //xmldoc.Load(filePath);

                //string proxy = null;
                //XmlNodeList pxmlnode = xmldoc.GetElementsByTagName("proxy");
                //if (pxmlnode.Count > 0)
                //{
                //    proxy = pxmlnode[0].InnerText;
                //}

                //XmlNodeList xmlnode = xmldoc.GetElementsByTagName("ipaddress");
                //List<String> permittedIPList = new List<string>();
                //for (int i = 0; i < xmlnode.Count; i++)
                //{
                //    permittedIPList.Add(xmlnode[i].InnerText);
                //}
                //permittedIPList.Add("127.0.0.1");

                //string addr = this.HttpContext.Request.UserHostAddress;
                //string hf = this.HttpContext.Request.Headers["X-Forwarded-For"];
                //if (addr != proxy)
                //{
                //    foreach (String permittedIP in permittedIPList)
                //    {
                //        if (addr == permittedIP)
                //            return true;
                //    }
                //}
                //else if (hf != null)
                //{
                //    foreach (String permittedIP in permittedIPList)
                //    {
                //        if (hf.IndexOf(permittedIP) >= 0)
                //            return true;
                //    }
                //}

                //return false;
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
