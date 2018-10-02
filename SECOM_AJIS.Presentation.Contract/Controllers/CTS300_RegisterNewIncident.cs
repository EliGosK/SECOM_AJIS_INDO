//*********************************
// Create by: Natthavat 
// Create date: 1/Oct/2011
// Update date: 1/Oct/2011
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
using SECOM_AJIS.Common.Util;

using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Contract;
using CSI.WindsorHelper;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Presentation.Contract.Models;
using System.Transactions;
using SECOM_AJIS.Common.Models.EmailTemplates;

namespace SECOM_AJIS.Presentation.Contract.Controllers
{
    public partial class ContractController : BaseController
    {
        /// <summary>
        /// Check user’s permission
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CTS300_Authority(CTS300_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                res = ValidateAutority_CTS300(res);
                if (res.IsError)
                {
                    return Json(res);
                }

                res = ValidateParameter_CTS300(param, res);
                if (res.IsError)
                {
                    return Json(res);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<CTS300_ScreenParameter>("CTS300", param, res);
        }
        
        //public ActionResult CTS300_Authority(string strIncedentRevantType, string strIncedentRevantCode)
        //{
        //    ObjectResultData res = new ObjectResultData();
        //    CTS300_ScreenParameter scrParam = null;

        //    try
        //    {
        //        if (CheckIsSuspending(res))
        //        {
        //            if (!CheckUserPermission(ScreenID.C_SCREEN_ID_REGISTER_INCIDENT))
        //            {
        //                res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0053);
        //                return Json(res);
        //            }
        //        }

        //        scrParam = new CTS300_ScreenParameter()
        //        {
        //            strIncidentRelevantCode = strIncedentRevantCode,
        //            strIncidentRelevantType = strIncedentRevantType
        //        };

        //        //SetScreenParameter_CTS300(scrParam);
        //    }
        //    catch (Exception ex)
        //    {
        //        res.AddErrorMessage(ex);
        //    }

        //    if (CommonUtil.IsNullOrEmpty(strIncedentRevantType) == false)
        //    {
        //        //param = new doContractInfoCondition()
        //        //{
        //        //    ContractCode = strContractCode,
        //        //    OCC = strOCC 
        //        //};
        //    }

        //    return InitialScreenEnvironment("CTS300", scrParam);
        //}

        /// <summary>
        /// Initial screen
        /// </summary>
        /// <returns></returns>
        [Initialize("CTS300")]
        public ActionResult CTS300()
        {
            try
            {
                //ICommonHandler handler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                //doEmailProcess dtEmail = new doEmailProcess();
                //dtEmail.MailFrom = "narupon@csithai.com";
                //dtEmail.MailFromAlias = "ton";
                //dtEmail.MailTo = "lkback999@gmail.com;boomzat@gmail.com";
                //dtEmail.Message = "Test MVC for send email process.";
                //dtEmail.Subject = "MVC Test";

                //handler.SendMail(dtEmail);

                ViewBag.AttachKey = GetCurrentKey();
            }
            catch
            {

            }

            // tt for test

            return View();
        }

        /// <summary>
        /// Checking parameter from another screen
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS300_CheckParameter()
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                CTS300_ScreenParameter param = GetScreenObject<CTS300_ScreenParameter>();
                if (param != null)
                {
                    res.ResultData = param;
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Retrieve customer group list when retrieve customer data from code
        /// </summary>
        /// <param name="strCustomerCode"></param>
        /// <returns></returns>
        public ActionResult CTS300_GetCustomerGroup(string strCustomerCode)
        {
            try
            {
                CommonUtil c = new CommonUtil();
                strCustomerCode = c.ConvertCustCode(strCustomerCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                ICustomerMasterHandler handler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;

                List<dtCustomeGroupData> lsCustomerGroups = handler.GetCustomeGroupData(strCustomerCode, null);

                return Json(lsCustomerGroups);

            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Initial contract target grid schema
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS300_IntialGridContractTarget()
        {

            return Json(CommonUtil.ConvertToXml<object>(null, "Contract\\CTS300_ContractTarget", CommonUtil.GRID_EMPTY_TYPE.VIEW));
        }

        /// <summary>
        /// Initial customer group grid schema
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS300_IntialGridCustomerGroup()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Contract\\CTS300_CustomerGroup", CommonUtil.GRID_EMPTY_TYPE.VIEW));
        }

        /// <summary>
        /// Initial attach document grid schema
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS300_IntialGridAttachedDocList()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Contract\\CTS300_AttachedDocList", CommonUtil.GRID_EMPTY_TYPE.VIEW));
        }

        /// <summary>
        /// Initial person in charge grid schema
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS300_IntialGridAssignPIC()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Contract\\CTS300_AssignPIC", CommonUtil.GRID_EMPTY_TYPE.VIEW));
        }

        /// <summary>
        /// Retrieve project data from project code when click [Retrieve] button of Project on Incident Relevant Information section
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        public ActionResult CTS300_RetrieveProjectInfo(string strProjectCode)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                CTS300_ReturnData objRes = new CTS300_ReturnData();
                IProjectHandler projecthandper = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
                IViewContractHandler viewhandler = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
                ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                //var searchRes = projecthandper.GetProjectDataForSearch(new doSearchProjectCondition()
                //{
                //    ProjectCode = strProjectCode
                //});

                commonhandler.ClearTemporaryUploadFile(GetCurrentKey());

                var searchRes = viewhandler.GetProjectInfo(strProjectCode);

                if ((searchRes == null) || (searchRes.tbt_Project == null) || (searchRes.tbt_Project.Count == 0))
                {
                    res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0011, new string[] { strProjectCode }, new string[] { "ProjectCode" });
                }
                else
                {
                    dtProjectData obj = new dtProjectData()
                    {
                        ProjectCode = searchRes.tbt_Project[0].ProjectCode,
                        ProjectName = searchRes.tbt_Project[0].ProjectName,
                        ProjectAddress = searchRes.tbt_Project[0].ProjectAddress,
                        ProjectPurchaseNameEN = searchRes.dtProjectPurcheser.CustFullNameEN,
                        ProjectPurchaseNameLC = searchRes.dtProjectPurcheser.CustFullNameLC
                    };

                    objRes.SearchResult = obj;
                    objRes.HasPermissionInCharge = HasAuditPermission_CTS300();
                    res.ResultData = objRes;
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Retrieve customer data from customer code when click [Retrieve] button of Customer on Incident Relevant Information section
        /// </summary>
        /// <param name="strCustomerCode"></param>
        /// <returns></returns>
        public ActionResult CTS300_RetrieveCustomerInfo(string strCustomerCode)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                CTS300_ReturnData objRes = new CTS300_ReturnData();
                CTS300_CustomerData custDat = new CTS300_CustomerData();
                CommonUtil util = new CommonUtil();
                ICustomerMasterHandler custhandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
                ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                commonhandler.ClearTemporaryUploadFile(GetCurrentKey());

                var searchRes = custhandler.GetCustomerDataForSearch(new doCustomerSearchCondition()
                {
                    CustomerCode = util.ConvertCustCode(strCustomerCode, CommonUtil.CONVERT_TYPE.TO_LONG)
                });

                if (searchRes.Count != 1)
                {
                    res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0011, new string[] { strCustomerCode }, new string[] { "CustomerCode" });
                }
                else
                {
                    CommonUtil.MappingObjectLanguage<dtCustomerData>(searchRes);
                    custDat.CustCode = util.ConvertCustCode(searchRes.ToList()[0].CustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    custDat.AddressFullEN = searchRes.ToList()[0].AddressFullEN;
                    custDat.AddressFullLC = searchRes.ToList()[0].AddressFullLC;
                    custDat.BusinessTypeNameEN = searchRes.ToList()[0].BusinessTypeNameEN;
                    custDat.BusinessTypeNameJP = searchRes.ToList()[0].BusinessTypeNameJP;
                    custDat.BusinessTypeNameLC = searchRes.ToList()[0].BusinessTypeNameLC;
                    custDat.CustFullNameEN = searchRes.ToList()[0].CustFullNameEN;
                    custDat.CustFullNameLC = searchRes.ToList()[0].CustFullNameLC;
                    custDat.IDNo = searchRes.ToList()[0].IDNo;
                    custDat.PhoneNo = searchRes.ToList()[0].PhoneNo;
                    custDat.NationalityEN = searchRes.ToList()[0].NationalityEN;
                    custDat.NationalityJP = searchRes.ToList()[0].NationalityJP;
                    custDat.NationalityLC = searchRes.ToList()[0].NationalityLC;
                    custDat.URL = searchRes.ToList()[0].URL;
                    custDat.BusinessTypeName = searchRes.ToList()[0].BusinessTypeName;
                    custDat.Nationality = searchRes.ToList()[0].Nationality;
                    objRes.SearchResult = custDat;
                    objRes.HasPermissionInCharge = HasAuditPermission_CTS300();
                    res.ResultData = objRes;
                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Retrieve customer group list from customer code
        /// </summary>
        /// <param name="strCustomerCode"></param>
        /// <returns></returns>
        public ActionResult CTS300_RetrieveCustomerGroupGrid(string strCustomerCode)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                CommonUtil util = new CommonUtil();
                ICustomerMasterHandler custhandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;

                List<dtCustomeGroupData> searchRes = custhandler.GetCustomeGroupData(util.ConvertCustCode(strCustomerCode, CommonUtil.CONVERT_TYPE.TO_LONG), null);

                res.ResultData = CommonUtil.ConvertToXml<dtCustomeGroupData>(searchRes, "Contract\\CTS300_CustomerGroup", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Retrieve site data from site code when click [Retrieve] button of Site on Incident Relevant Information section
        /// </summary>
        /// <param name="strSiteCode"></param>
        /// <returns></returns>
        public ActionResult CTS300_RetrieveSiteInfo(string strSiteCode)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                CTS300_SiteData siteRes = new CTS300_SiteData();
                CTS300_ReturnData objRes = new CTS300_ReturnData();
                CommonUtil util = new CommonUtil();
                //ISiteMasterHandler sitehandler = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;
                IViewContractHandler viewcontracthandler = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
                ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                //var searchRes = sitehandler.GetSiteDataForSearch(new doSiteSearchCondition()
                //    {
                //        SiteCode = util.ConvertSiteCode(strSiteCode, CommonUtil.CONVERT_TYPE.TO_LONG),
                //    });

                commonhandler.ClearTemporaryUploadFile(GetCurrentKey());

                var searchRes = viewcontracthandler.GetSiteInfo(util.ConvertSiteCode(strSiteCode, CommonUtil.CONVERT_TYPE.TO_LONG));

                if ((searchRes == null) || (searchRes.doGetTbm_Site == null) || (searchRes.doGetTbm_Site.Count != 1))
                {
                    res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0011, new string[] { strSiteCode }, new string[] { "SiteCode" });
                }
                else
                {
                    CommonUtil.MappingObjectLanguage<tbm_BuildingUsage>(searchRes.tbm_BuildingUsage);
                    siteRes.SiteCode = util.ConvertSiteCode(searchRes.doGetTbm_Site[0].SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    siteRes.AddressFullEN = searchRes.doGetTbm_Site[0].AddressFullEN;
                    siteRes.AddressFullLC = searchRes.doGetTbm_Site[0].AddressFullLC;
                    siteRes.SiteNameEN = searchRes.doGetTbm_Site[0].SiteNameEN;
                    siteRes.SiteNameLC = searchRes.doGetTbm_Site[0].SiteNameLC;
                    siteRes.PhoneNo = searchRes.doGetTbm_Site[0].PhoneNo;

                    siteRes.Usage = (searchRes.tbm_BuildingUsage.Count > 0) ? searchRes.tbm_BuildingUsage[0].BuildingUsageName : "";

                    objRes.HasPermissionInCharge = HasAuditPermission_CTS300();
                    objRes.SearchResult = siteRes;
                    res.ResultData = objRes;
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Retrieve contract data from user code/contract code when click [Retrieve] button of User code/Contract on Incident Relevant Information section
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        public ActionResult CTS300_RetrieveContractInfo(string strContractCode)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                CommonUtil util = new CommonUtil();
                CTS300_ReturnData result = new CTS300_ReturnData();
                CTS300_ContractData resObj = new CTS300_ContractData();
                IRentralContractHandler rentralcontracthandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                ISaleContractHandler salecontracthandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                commonhandler.ClearTemporaryUploadFile(GetCurrentKey());

                var searchRentalRes = rentralcontracthandler.GetRentalContractBasicForView(util.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), strContractCode);
                if (searchRentalRes.Count != 1)
                {
                    var searchSaleRes = salecontracthandler.GetSaleContractBasicForView(util.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                    if (searchSaleRes.Count != 1)
                    {
                        res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0011, new string[] { strContractCode }, new string[] { "UserCode_ContractCode" });
                    }
                    else
                    {
                        CommonUtil.MappingObjectLanguage<dtSaleContractBasicForView>(searchSaleRes);

                        resObj.ContractCode = util.ConvertContractCode(searchSaleRes[0].ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        resObj.ContractFrom = "2";
                        resObj.UserCode = "-";
                        resObj.SiteName = CommonUtil.TextLineFormat(searchSaleRes[0].SiteNameEN, searchSaleRes[0].SiteNameLC);
                        resObj.ContractName = CommonUtil.TextLineFormat(searchSaleRes[0].PurchaserNameEN, searchSaleRes[0].PurchaserNameLC);

                        result.SearchResult = resObj;
                        result.HasPermissionInCharge = HasAuditPermission_CTS300();
                    }
                }
                else
                {
                    resObj.ContractCode = util.ConvertContractCode(searchRentalRes[0].ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    resObj.ContractFrom = "1";
                    resObj.UserCode = searchRentalRes[0].UserCode;
                    resObj.SiteName = CommonUtil.TextLineFormat(searchRentalRes[0].SiteNameEN, searchRentalRes[0].SiteNameLC);
                    resObj.ContractName = CommonUtil.TextLineFormat(searchRentalRes[0].ContractTargetNameEN, searchRentalRes[0].ContractTargetNameLC);

                    result.SearchResult = resObj;
                    result.HasPermissionInCharge = HasAuditPermission_CTS300();
                }

                res.ResultData = result;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Retrieve attach document list
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS300_LoadGridAttachedDocList()
        {
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            ObjectResultData res = new ObjectResultData();

            try
            {
                List<dtAttachFileForGridView> lstAttachedName = commonhandler.GetAttachFileForGridView(GetCurrentKey());
                res.ResultData = CommonUtil.ConvertToXml<dtAttachFileForGridView>(lstAttachedName, "Contract\\CTS300_AttachedDocList", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Validate and attach new document
        /// </summary>
        /// <param name="fileSelect"></param>
        /// <param name="DocumentName"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public ActionResult CTS300_AttachFile(HttpPostedFileBase fileSelect, string DocumentName, string k)
        {
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            ObjectResultData res = new ObjectResultData();

            try
            {
                if (fileSelect == null)
                {
                    // File not select
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0050, null);
                }

                if (String.IsNullOrEmpty(DocumentName))
                {
                    // DocName is not input
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { CommonUtil.GetLabelFromResource("Contract", "CTS300", "lblDocumentName") });
                }

                byte[] fileData;

                using (BinaryReader reader = new BinaryReader(fileSelect.InputStream))
                {
                    var fList = commonhandler.GetAttachFileForGridView(k);

                    var filterDupItem = from a in fList where a.FileName.ToUpper().Equals(DocumentName.ToUpper() + Path.GetExtension(fileSelect.FileName).ToUpper()) select a;

                    if (filterDupItem.Count() > 0)
                    {
                        // Docname duplicate
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0115, new string[] { DocumentName });
                        //outmsg = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0115, new string[] { DocumentName });
                    }
                    else
                    {
                        fileData = reader.ReadBytes(fileSelect.ContentLength);

                        if (commonhandler.CanAttachFile(DocumentName, fileData.Length, Path.GetExtension(fileSelect.FileName), k, k))
                        {
                            DateTime currDate = DateTime.Now;
                            commonhandler.InsertAttachFile(k
                            , DocumentName
                            , Path.GetExtension(fileSelect.FileName)
                            , fileData.Length
                            , fileData
                            , false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            if (res.IsError)
            {
                ViewBag.Message = res.Message.Message;
                ViewBag.MsgCode = res.Message.Code;
            }

            ViewBag.K = k;

            return View("CTS300_Upload");
        }

        /// <summary>
        /// Remove exist attach document
        /// </summary>
        /// <param name="AttachID"></param>
        /// <returns></returns>
        public ActionResult CTS300_RemoveAttach(string AttachID)
        {
            ObjectResultData res = new ObjectResultData();
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            try
            {
                int _attachID = int.Parse(AttachID);

                commonhandler.DeleteAttachFileByID(_attachID, GetCurrentKey());

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Clear all attach document
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS300_ClearAttach()
        {
            ObjectResultData res = new ObjectResultData();
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            try
            {
                commonhandler.ClearTemporaryUploadFile(GetCurrentKey());
                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Initial attach document section
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS300_Upload()
        {
            ViewBag.K = GetCurrentKey();
            return View("CTS300_Upload");
        }

        /// <summary>
        /// Retrieve incident received detail and reason type when change select [IncidentType] drop down list
        /// </summary>
        /// <param name="strIncidentType"></param>
        /// <returns></returns>
        public ActionResult CTS300_RetrieveReceivedDetailPatternAndReason(string strIncidentType)
        {
            ObjectResultData result = new ObjectResultData();
            CTS300_PatternAndReason obj = new CTS300_PatternAndReason();
            IIncidentHandler incidenthandler = ServiceContainer.GetService<IIncidentHandler>() as IIncidentHandler;

            try
            {
                var dat = incidenthandler.GetTbs_IncidentTypePattern(strIncidentType);
                if (dat.Count == 1)
                {
                    CommonUtil.MappingObjectLanguage<dtTbs_IncidentTypePattern>(dat);
                    obj.RecievedDetail = dat[0].IncidentTypeContent;
                }
                else
                {
                    obj.RecievedDetail = String.Empty;
                }

                var reason = incidenthandler.GetTbs_IncidentReasonType(strIncidentType);
                CommonUtil.MappingObjectLanguage<tbs_IncidentReasonType>(reason);
                if (reason.Count > 0)
                {
                    obj.HaveReasonType = true;
                    obj.ReasonType = CommonUtil.CommonComboBox<tbs_IncidentReasonType>("{BlankID}"
                        , reason
                        , "ReasonTypeName"
                        , "ReasonType"
                        , null
                        , true).ToString();
                }
                else
                {
                    obj.HaveReasonType = false;
                    obj.ReasonType = String.Empty;
                }

                result.ResultData = obj;
            }
            catch (Exception ex)
            {
                result.AddErrorMessage(ex);
            }

            return Json(result);
        }

        /// <summary>
        /// Retrieve department item when change select [Office] drop down list on Assign person in charge information subsection
        /// </summary>
        /// <param name="strOfficeCode"></param>
        /// <returns></returns>
        public ActionResult CTS300_RetrieveDepartmentComboBox(string strOfficeCode)
        {
            ObjectResultData result = new ObjectResultData();
            IEmployeeMasterHandler emphandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
            IOfficeMasterHandler officehandler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler; //Add by jutarat A. on 20082012

            CTS300_ScreenParameter param = GetScreenObject<CTS300_ScreenParameter>(); //Add by jutarat A. on 20082012

            try
            {
                if (String.IsNullOrEmpty(strOfficeCode))
                {
                    result.ResultData = String.Empty;
                }
                else
                {
                    //Add by jutarat A. on 20082012
                    param.blnIncidentIsHeadOfficeFlag = officehandler.CheckHeadOffice(strOfficeCode);
                    if (param.blnIncidentIsHeadOfficeFlag == false)
                    {
                        result.ResultData = false;
                    }
                    //End Add
                    else
                    {
                        var dat = emphandler.GetBelongingDepartmentList(strOfficeCode, null);
                        if (dat.Count == 0)
                        {
                            result.ResultData = String.Empty;
                        }
                        else
                        {
                            result.ResultData = CommonUtil.CommonComboBox<dtDepartment>("{BlankID}", dat.ToList(), "DepartmentNameCode", "DepartmentCode", null, true).ToString();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                result.AddErrorMessage(ex);
            }

            return Json(result);
        }

        /// <summary>
        /// Retrieve incident role when change select [Department] drop down list on Assign person in charge information subsection
        /// </summary>
        /// <param name="strOfficeCode"></param>
        /// <param name="strDepartmentCode"></param>
        /// <returns></returns>
        public ActionResult CTS300_RetrieveIncidentRoleComboBox(string strOfficeCode, string strDepartmentCode)
        {
            ObjectResultData result = new ObjectResultData();
            CTS300_ScreenParameter param = GetScreenObject<CTS300_ScreenParameter>(); //Add by jutarat A. on 20082012

            try
            {
                //Modify by jutarat A. on 20082012
                //if (string.IsNullOrEmpty(strDepartmentCode))
                //{
                //    result.ResultData = string.Empty;
                //}
                if (string.IsNullOrEmpty(strDepartmentCode) == false || param.blnIncidentIsHeadOfficeFlag == false)
                {
                    List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
                    try
                    {
                        List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                    {
                        new doMiscTypeCode()
                        {
                            FieldName = MiscType.C_INCIDENT_ROLE,
                            ValueCode = "%"
                        }
                    };

                        ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                        lst = hand.GetMiscTypeCodeList(miscs);
                    }
                    catch
                    {
                    }

                    if (lst == null)
                        lst = new List<doMiscTypeCode>();

                    string display = "ValueCodeDisplay";

                    result.ResultData = CommonUtil.CommonComboBox<doMiscTypeCode>("{BlankID}", lst, display, "ValueCode", null, true).ToString();
                }
                else
                {
                    result.ResultData = string.Empty;
                }
                //End Modify
            }
            catch (Exception ex)
            {
                result.AddErrorMessage(ex);
            }

            return Json(result);
        }

        /// <summary>
        /// Retrieve employee when change select [IncidentRole] drop down list on Assign person in charge information subsection
        /// </summary>
        /// <param name="strOfficeCode"></param>
        /// <param name="strDepartmentCode"></param>
        /// <param name="strIncidentRoleCode"></param>
        /// <returns></returns>
        public ActionResult CTS300_RetrieveEmployeeComboBox(string strOfficeCode, string strDepartmentCode, string strIncidentRoleCode)
        {
            ObjectResultData result = new ObjectResultData();
            IEmployeeMasterHandler emphandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;

            CTS300_ScreenParameter param = GetScreenObject<CTS300_ScreenParameter>(); //Add by jutarat A. on 20082012

            try
            {
                //var dat = emphandler.GetBelongingEmpList(strOfficeCode
                //    , strDepartmentCode
                //    , ((strIncidentRoleCode == IncidentRole.C_INCIDENT_ROLE_CHIEF) || (strIncidentRoleCode == IncidentRole.C_INCIDENT_ROLE_CONTROL_CHIEF)) ? true : (bool?)null
                //    , (bool?)null
                //    , ((strIncidentRoleCode == IncidentRole.C_INCIDENT_ROLE_CORRESPONDENT) || (strIncidentRoleCode == IncidentRole.C_INCIDENT_ROLE_ASSISTANT)) ? true : (bool?)null);

                //strOfficeCode = "1000";
                //strDepartmentCode = "0500";

                var datTemp = emphandler.GetBelongingEmpList(strOfficeCode
                    , param.blnIncidentIsHeadOfficeFlag == false ? null : strDepartmentCode //strDepartmentCode  //Modify by jutarat A. on 20082012
                    , ((strIncidentRoleCode == IncidentRole.C_INCIDENT_ROLE_CHIEF) || (strIncidentRoleCode == IncidentRole.C_INCIDENT_ROLE_CONTROL_CHIEF)) ? true : (bool?)null
                    , null
                    , ((strIncidentRoleCode == IncidentRole.C_INCIDENT_ROLE_CORRESPONDENT) || (strIncidentRoleCode == IncidentRole.C_INCIDENT_ROLE_ASSISTANT)) ? true : (bool?)null);

                //Add by jutarat A. on 21082012
                var datDist = (from t in datTemp
                           group t by new 
                           { 
                               EmpNo = t.EmpNo,
                               EmpFirstNameEN = t.EmpFirstNameEN,
                               EmpLastNameEN = t.EmpLastNameEN,
                               EmpFirstNameLC = t.EmpFirstNameLC,
                               EmpLastNameLC = t.EmpLastNameLC
                           } into g
                           select g.FirstOrDefault());

                List<dtEmployeeBelonging> dat = datDist.ToList<dtEmployeeBelonging>();
                //End Add

                CommonUtil.MappingObjectLanguage<dtEmployeeBelonging>(dat);

                if (param.blnIncidentIsHeadOfficeFlag) //Add by jutarat A. on 20082012
                {
                    if ((dat.Count < 1) && ((strIncidentRoleCode == IncidentRole.C_INCIDENT_ROLE_CHIEF) || (strIncidentRoleCode == IncidentRole.C_INCIDENT_ROLE_CONTROL_CHIEF)))
                    {
                        result.AddErrorMessage("Contract", MessageUtil.MessageList.MSG3184, null, new string[] { "IncidentRoleCode" });
                    }
                    else if ((dat.Count > 1) && ((strIncidentRoleCode == IncidentRole.C_INCIDENT_ROLE_CHIEF) || (strIncidentRoleCode == IncidentRole.C_INCIDENT_ROLE_CONTROL_CHIEF)))
                    {
                        result.AddErrorMessage("Contract", MessageUtil.MessageList.MSG3183, null, new string[] { "IncidentRoleCode" });
                    }
                    else if ((strIncidentRoleCode == IncidentRole.C_INCIDENT_ROLE_CHIEF) || (strIncidentRoleCode == IncidentRole.C_INCIDENT_ROLE_CONTROL_CHIEF))
                    {
                        result.ResultData = CommonUtil.CommonComboBox<dtEmployeeBelonging>("{BlankID}", dat.ToList(), "EmpFullNameWithCode", "EmpNo", null, false).ToString();
                    }
                    else
                    {
                        result.ResultData = CommonUtil.CommonComboBox<dtEmployeeBelonging>("{BlankID}", dat.ToList(), "EmpFullNameWithCode", "EmpNo", null, true).ToString();
                    }
                }
                //Add by jutarat A. on 20082012
                else
                {
                    result.ResultData = CommonUtil.CommonComboBox<dtEmployeeBelonging>("{BlankID}", dat.ToList(), "EmpFullNameWithCode", "EmpNo", null, true).ToString();
                }
                //End Add
            }
            catch (Exception ex)
            {
                result.AddErrorMessage(ex);
            }

            return Json(result);
        }

        /// <summary>
        /// Validate business before register when click [Register] button 
        /// </summary>
        /// <param name="dat"></param>
        /// <param name="HaveReasonType"></param>
        /// <returns></returns>
        public ActionResult CTS300_ValidateData(CTS300_RegisData dat, bool HaveReasonType)
        {
            ObjectResultData result = new ObjectResultData();

            try
            {
                result = ValidateAutority_CTS300(result);

                if (result.IsError)
                {
                    return Json(result);
                }

                //MapToIncidentRelevantType_CTS300(ref dat);
                result = ValidateData_CTS300(dat, HaveReasonType, true);
            }
            catch (Exception ex)
            {
                result.AddErrorMessage(ex);
            }

            return Json(result);
        }

        /// <summary>
        /// Proceed register new incident
        /// </summary>
        /// <param name="dat"></param>
        /// <param name="HaveReasonType"></param>
        /// <returns></returns>
        public ActionResult CTS300_RegisIncidentRelevant(CTS300_RegisData dat, bool HaveReasonType)
        {
            ObjectResultData result = new ObjectResultData();

            try
            {
                // Validate data (again)
                result = ValidateAutority_CTS300(result);
                if (result.IsError)
                {
                    return Json(result);
                }

                //MapToIncidentRelevantType_CTS300(ref dat);
                var validateRes = ValidateData_CTS300(dat, HaveReasonType);
                if (validateRes.IsError)
                {
                    return Json(validateRes);
                }

                // Register data
                var regisRes = RegisterIncident_CTS300(dat);
                result.ResultData = regisRes;
                if (regisRes.ErrorMessage != null)
                {
                    if (regisRes.ErrorMessage.Code == MessageUtil.MessageList.MSG3267.ToString())
                    {
                        if (result.MessageList == null)
                        {
                            result.AddErrorMessage("Contract", MessageUtil.MessageList.MSG3267, null, null);
                        }
                        else
                        {
                            result.MessageList.Add(regisRes.ErrorMessage);
                        }
                    }
                    else
                    {
                        throw new Exception(regisRes.ErrorMessage.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                result.AddErrorMessage(ex);
            }

            return Json(result);
        }

        //private bool CheckUserPermission_CTS300()
        //{
        //    try
        //    {
        //        dsTransDataModel dsTrans = CommonUtil.dsTransData;
        //        if (dsTrans == null)
        //            return false;

        //        if (dsTrans.dtUserPermissionData != null)
        //        {
        //            var hasPermit = from a in dsTrans.dtUserPermissionData where a.Key.StartsWith(ScreenID.C_SCREEN_ID_REGISTER_INCIDENT) select a;
        //            return hasPermit.Count() > 0;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }

        //    return false;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool HasAuditPermission_CTS300()
        {
            bool res = false;

            try
            {
                res = CheckUserPermission(ScreenID.C_SCREEN_ID_REGISTER_INCIDENT, FunctionID.C_FUNC_ID_SPECIAL_ADD_INCIDENT_ROLE);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return res;
        }

        //private void MapToIncidentRelevantType_CTS300(ref CTS300_RegisData dat)
        //{
            //if (dat.RelevantType == "1")
            //{
            //    dat.RelevantType = IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CUSTOMER;
            //}
            //else if (dat.RelevantType == "2")
            //{
            //    dat.RelevantType = IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_SITE;
            //}
            //else if (dat.RelevantType == "3")
            //{
            //    dat.RelevantType = IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_PROJECT;
            //}
            //else if (dat.RelevantType == "4")
            //{
            //    dat.RelevantType = IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CONTRACT;
            //}
        //}

        /// <summary>
        /// Validate require field and business from register data
        /// </summary>
        /// <param name="dat"></param>
        /// <param name="HaveReasonType"></param>
        /// <returns></returns>
        private ObjectResultData ValidateData_CTS300(CTS300_RegisData dat, bool HaveReasonType, bool isCheckForWarning = false)
        {
            ObjectResultData result = new ObjectResultData();

            try
            {
                List<string> controlLst = new List<string>()
                , labelList = new List<string>();

                // Check System Suspend
                if (CheckIsSuspending(result))
                {
                    result.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return result;
                }

                if (!dat.ReceivedDate.HasValue)
                {
                    controlLst.Add("ReceivedDate");
                    labelList.Add("lblReceivedDate");
                }

                if (!dat.ReceivedTime.HasValue)
                {
                    controlLst.Add("ReceivedTime");
                    labelList.Add("lblReceivedTime");
                }

                if (String.IsNullOrEmpty(dat.ReceivedMethod))
                {
                    controlLst.Add("ReceivedMethod");
                    labelList.Add("lblIncidentRelevantType");
                }

                if (String.IsNullOrEmpty(dat.ContactName))
                {
                    controlLst.Add("ContractName");
                    labelList.Add("lblContractName");
                }

                if (String.IsNullOrEmpty(dat.IncidentTitle))
                {
                    controlLst.Add("IncidentTitle");
                    labelList.Add("lblIncidentTitle");
                }

                if (String.IsNullOrEmpty(dat.IncidentType))
                {
                    controlLst.Add("IncidentTypeCode");
                    labelList.Add("lblIncidentType");
                }

                if (String.IsNullOrEmpty(dat.ReceivedDetail))
                {
                    controlLst.Add("RecivedDetail");
                    labelList.Add("lblRecivedDetail");
                }

                if (HaveReasonType)
                {
                    if (String.IsNullOrEmpty(dat.ReasonType))
                    {
                        controlLst.Add("ReasonTypeCode");
                        labelList.Add("lblReasonType");
                    }
                }

                if (dat.DueDateDeadLineType == "1")
                {
                    if (!dat.DueDate_Date.HasValue || !dat.DueDate_Time.HasValue)
                    {
                        if (!dat.DueDate_Date.HasValue)
                            controlLst.Add("DueDate");

                        if (!dat.DueDate_Time.HasValue)
                            controlLst.Add("ReceivedDueTime");

                        labelList.Add("rdoDueDate");
                    }
                } else if (dat.DueDateDeadLineType == "2")
                {
                    if (!dat.Deadline_Date.HasValue || String.IsNullOrEmpty(dat.Deadline_Until))
                    {
                        if (!dat.Deadline_Date.HasValue)
                            controlLst.Add("DeadLine");

                        if (String.IsNullOrEmpty(dat.Deadline_Until))
                            controlLst.Add("DeadLineTimeType");

                        labelList.Add("rdoDeadLine");
                    }
                }

                if (labelList.Count > 0)
                {
                    var labelTextList = new List<string>();

                    foreach (var lblName in labelList)
                    {
                        labelTextList.Add(CommonUtil.GetLabelFromResource("Contract", "CTS300", lblName));
                    }

                    result.AddErrorMessage("Common", MessageUtil.MessageList.MSG0007, new string[] { CommonUtil.TextList(labelTextList.ToArray(), ", ") }, controlLst.ToArray());
                }

                if (result.IsError)
                {
                    result.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return result;
                }

                if ((dat.InChargeList != null) && (dat.InChargeList.Count > 0))
                {
                    var filterControlChief = from a in dat.InChargeList where a.IncidentRoleCode == IncidentRole.C_INCIDENT_ROLE_CONTROL_CHIEF select a;
                    if ((filterControlChief != null) && (filterControlChief.Count() == 0))
                    {
                        // Alert No Control Cheif Specify
                        result.AddErrorMessage("Contract", MessageUtil.MessageList.MSG3276, null, null);
                        result.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    }
                }

                if (result.IsError)
                {
                    result.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return result;
                }

                // Get Attachment Doc

                //Add by Jutarat A. on 06092012
                //Validate warning
                //Check e-mail address of important incident person incharge
                if (isCheckForWarning)
                {
                    if (dat.IsImportance)
                    {
                        IEmployeeMasterHandler emphandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;

                        List<SECOM_AJIS.DataEntity.Master.dtEmailAddressForIncident> mailAddr = emphandler.GetEmailAddressForIncident(FlagType.C_FLAG_ON);
                        CommonUtil.MappingObjectLanguage<SECOM_AJIS.DataEntity.Master.dtEmailAddressForIncident>(mailAddr);

                        StringBuilder sbMailEmpName = new StringBuilder();
                        StringBuilder sbMailTo = new StringBuilder();
                        foreach (var mailItem in mailAddr)
                        {
                            if (String.IsNullOrEmpty(mailItem.EmailAddress))
                            {
                                sbMailEmpName.AppendFormat("{0} {1}, ", mailItem.EmpFirstName, mailItem.EmpLastName);
                            }
                            else
                            {
                                sbMailTo.AppendFormat("{0}; ", mailItem.EmailAddress);
                            }
                        }

                        if (sbMailEmpName.Length > 1)
                        {
                            string strMailEmpName = sbMailEmpName.ToString().Substring(0, sbMailEmpName.Length - 2);
                            if (String.IsNullOrEmpty(strMailEmpName) == false)
                            {
                                result.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3304, new string[] { strMailEmpName }, null);
                                result.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                            }
                        }

                        if (sbMailTo.Length > 1)
                        {
                            string strMailTo = sbMailTo.ToString().Substring(0, sbMailTo.Length - 2);
                            if (String.IsNullOrEmpty(strMailTo) == false)
                            {
                                CTS300_ScreenParameter param = GetScreenObject<CTS300_ScreenParameter>();
                                param.MailTo = strMailTo;
                            }
                        }
                    }
                }
                //End Add

                result.ResultData = true;
            }
            catch (Exception ex)
            {
                result.AddErrorMessage(ex);
            }

            return result;
        }

        /// <summary>
        /// Proceed register new incident
        /// </summary>
        /// <param name="dat"></param>
        /// <returns></returns>
        private CTS300_RegisResult RegisterIncident_CTS300(CTS300_RegisData dat)
        {
            bool result = true;
            CTS300_RegisResult resobj = new CTS300_RegisResult();
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            IIncidentHandler incidenthandler = ServiceContainer.GetService<IIncidentHandler>() as IIncidentHandler;
            IOfficeMasterHandler officehandler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
            CommonUtil util = new CommonUtil();
            List<tbt_Incident> incidentInserted = new List<tbt_Incident>();
            List<tbt_IncidentRole> incidentRoleInserted = new List<tbt_IncidentRole>();
            string strStatus = "";

            // Create dsIncident
            CTS300_ScreenParameter sParam = GetScreenObject<CTS300_ScreenParameter>();
            //List<tbt_AttachFile> attachList = commonhandler.GetAttachFile(GetCurrentKey());
            List<dsIncidentData> rawDat = new List<dsIncidentData>();

            // Insert Incident
            using (TransactionScope scope = new TransactionScope())
            {
                if (dat.RelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CONTRACT)
                {
                    var contractList = dat.ContractRelevant;
                    foreach (CTS300_ContractData item in contractList)
                    {
                        rawDat.Add(CreateDataObject_CTS300(dat, item, out strStatus));
                    }
                }
                else
                {
                    rawDat.Add(CreateDataObject_CTS300(dat, out strStatus));
                }

                foreach (dsIncidentData item in rawDat)
                {
                    //cntItem++;
                    var insertItem = incidenthandler.InsertTbt_Incident(item.tbt_Incident);
                    incidentInserted.Add(insertItem);
                    if (incidentInserted.Count > 0)
                    {
                        item.tbt_IncidentRole = AssignIncidentIDToRole_CTS300(item.tbt_IncidentRole, incidentInserted[incidentInserted.Count - 1].IncidentID);
                        incidentRoleInserted = incidenthandler.InsertTbt_IncidentRole(item.tbt_IncidentRole);

                        var attachFileList = commonhandler.GetAttachFile(GetCurrentKey());
                        if (attachFileList.Count > 0)
                        {
                            if (item.tbt_Incident.IncidentRelavantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CONTRACT)
                            {
                                commonhandler.CopyAttachFile(AttachmentModule.Incident, GetCurrentKey(), insertItem.IncidentID.ToString(), false);
                            }
                            else
                            {
                                commonhandler.UpdateFlagAttachFile(AttachmentModule.Incident, GetCurrentKey(), insertItem.IncidentID.ToString());
                            }
                        }

                        if (item.tbt_Incident.ImportanceFlag.GetValueOrDefault() == FlagType.C_FLAG_ON)
                        {
                            try
                            {
                                //Modify by Jutarat A. on 07092012
                                if (String.IsNullOrEmpty(sParam.MailTo) == false)
                                {
                                    SendMail_CTS300(insertItem.IncidentID, sParam.MailTo, dat.RelevantType, item); //Add strRelevantType, IncidentData by Jutarat A. on 16072013
                                }
                            }
                            catch (Exception ex)
                            {
                                resobj.ErrorMessage = MessageUtil.GetMessage("Contract", MessageUtil.MessageList.MSG3267, null);
                            }
                        }
                    }
                }

                // Clear Attach File
                commonhandler.ClearTemporaryUploadFile(GetCurrentKey());

                resobj.IsCompleted = result;
                resobj.IncidentID = ((incidentInserted.Count == 1) ? incidentInserted[0].IncidentID.ToString() : "");
                resobj.IncidentNo = ((incidentInserted.Count == 1) ? incidentInserted[0].IncidentNo : "");
                //resobj.RegisStatus = ((incidentInserted.Count == 1) ? strStatus : "");
                resobj.RegisStatus = strStatus;
                scope.Complete();
            }

            return resobj;
        }

        /// <summary>
        /// Mapping register incident data to entity
        /// </summary>
        /// <param name="dat"></param>
        /// <param name="strStatus"></param>
        /// <returns></returns>
        private dsIncidentData CreateDataObject_CTS300(CTS300_RegisData dat, out string strStatus)
        {
            return CreateDataObject_CTS300(dat, null, out strStatus);
        }

        /// <summary>
        /// Mapping register incident data (from contract data) to entity
        /// </summary>
        /// <param name="dat"></param>
        /// <param name="contractDat"></param>
        /// <param name="strStatus"></param>
        /// <returns></returns>
        private dsIncidentData CreateDataObject_CTS300(CTS300_RegisData dat, CTS300_ContractData contractDat, out string strStatus)
        {
            dsIncidentData result = new dsIncidentData();
            CommonUtil util = new CommonUtil();
            IIncidentHandler incidenthandler = ServiceContainer.GetService<IIncidentHandler>() as IIncidentHandler;
            IOfficeMasterHandler officehandler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            string strIncidentRelevantType = dat.RelevantType
                        , strIncidentRelevantCode = dat.RelevantCode
                        , strIncidentOffice = ""
                        , strIncidentStatus = ""
                        , strDepartmentCode = ""
                        , strIncidentNo = "";

            if ((dat.InChargeList != null) && (dat.InChargeList.Count > 0))
            {
                var chiefCnt = from a in dat.InChargeList where a.IncidentRoleCode == IncidentRole.C_INCIDENT_ROLE_CONTROL_CHIEF select a;
                if (chiefCnt.Count() == 1)
                {
                    var chiefItem = chiefCnt.ToList()[0];
                    if (strIncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CUSTOMER)
                    {
                        strIncidentRelevantCode = util.ConvertCustCode(strIncidentRelevantCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                        strIncidentOffice = chiefItem.OfficeCode;
                    }
                    else if (strIncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_SITE)
                    {
                        strIncidentRelevantCode = util.ConvertSiteCode(strIncidentRelevantCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                        strIncidentOffice = null;
                    }
                    else if (strIncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_PROJECT)
                    {
                        strIncidentOffice = chiefItem.OfficeCode;
                    }
                    else if (strIncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CONTRACT)
                    {
                        strIncidentRelevantCode = contractDat.ContractCode;
                        strIncidentRelevantCode = util.ConvertContractCode(strIncidentRelevantCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                        strIncidentOffice = null;
                    }

                    string[] incidentNo = incidenthandler.GenerateIncidentNo(strIncidentRelevantType, strIncidentRelevantCode, strIncidentOffice);
                    strIncidentNo = incidentNo[0];
                    strIncidentOffice = incidentNo[1];
                    strIncidentStatus = IncidentStatus.C_INCIDENT_STATUS_NEW_REGISTER;

                    if (officehandler.CheckHeadOffice(strIncidentOffice))
                    {
                        strDepartmentCode = chiefItem.DepartmentCode;
                    }
                }
            }
            else
            {
                strIncidentStatus = IncidentStatus.C_INCIDENT_STATUS_CONTROL_CHIEF_UNREGISTERED;
            }

            if ((dat.RelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CUSTOMER) && (dat.CustomerRelateType == "2"))
            {
                var custRelate = incidenthandler.GetSiteRelatedOfficeChief(strIncidentRelevantCode);
                foreach (var item in custRelate)
                {
                    CTS300_PersonIncharge picTmp = new CTS300_PersonIncharge()
                    {
                        EmpNo = item.EmpNo,
                        IncidentRoleCode = IncidentRole.C_INCIDENT_ROLE_CHIEF_OF_RELATED_OFFICE
                    };

                    if (dat.InChargeList == null)
                    {
                        dat.InChargeList = new List<CTS300_PersonIncharge>();
                    }

                    dat.InChargeList.Add(picTmp);
                }
            }
            else if ((dat.RelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_SITE) && (dat.SiteRelateType == "2"))
            {
                var siteRelate = incidenthandler.GetContractRelatedOfficeChief(strIncidentRelevantCode);
                foreach (var item in siteRelate)
                {
                    CTS300_PersonIncharge picTmp = new CTS300_PersonIncharge()
                    {
                        EmpNo = item.EmpNo,
                        IncidentRoleCode = IncidentRole.C_INCIDENT_ROLE_CHIEF_OF_RELATED_OFFICE
                    };

                    if (dat.InChargeList == null)
                    {
                        dat.InChargeList = new List<CTS300_PersonIncharge>();
                    }

                    dat.InChargeList.Add(picTmp);
                }
            }

            result.tbt_Incident = new tbt_Incident();
            result.tbt_IncidentRole = new List<tbt_IncidentRole>();

            if (dat.RelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CUSTOMER)
            {
                result.tbt_Incident.CustCode = util.ConvertCustCode(dat.RelevantCode, CommonUtil.CONVERT_TYPE.TO_LONG);
            }
            else if (dat.RelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_SITE)
            {
                result.tbt_Incident.SiteCode = util.ConvertSiteCode(dat.RelevantCode, CommonUtil.CONVERT_TYPE.TO_LONG);
            }
            else if (dat.RelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_PROJECT)
            {
                result.tbt_Incident.ProjectCode = dat.RelevantCode;
            }
            else if (dat.RelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CONTRACT)
            {
                result.tbt_Incident.ContractCode = util.ConvertContractCode(contractDat.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
            }

            result.tbt_Incident.IncidentNo = strIncidentNo;
            result.tbt_Incident.IncidentOfficeCode = strIncidentOffice;
            result.tbt_Incident.IncidentDepartmentCode = strDepartmentCode;
            result.tbt_Incident.IncidentStatus = strIncidentStatus;
            result.tbt_Incident.hasRespondingDetailFlag = FlagType.C_FLAG_OFF;
            result.tbt_Incident.IncidentRelavantType = strIncidentRelevantType;
            result.tbt_Incident.ReceivedDate = dat.ReceivedDate;
            result.tbt_Incident.ReceivedTime = dat.ReceivedTime;
            result.tbt_Incident.ReceivedMethod = dat.ReceivedMethod;
            result.tbt_Incident.ContactPerson = dat.ContactName;
            result.tbt_Incident.ContactPersonDep = dat.Department;
            result.tbt_Incident.IncidentTitle = dat.IncidentTitle;
            result.tbt_Incident.IncidentType = dat.IncidentType;
            result.tbt_Incident.ReasonType = dat.ReasonType;
            result.tbt_Incident.ImportanceFlag = dat.IsImportance;
            result.tbt_Incident.ConfidentialFlag = dat.IsSpecialInfo;
            result.tbt_Incident.ReceivedDetail = dat.ReceivedDetail;
            result.tbt_Incident.DueDate = dat.DueDate_Date;
            result.tbt_Incident.DueDateTime = dat.DueDate_Time;
            result.tbt_Incident.DeadLine = dat.Deadline_Date;
            result.tbt_Incident.DeadLineTime = dat.Deadline_Until;
            result.tbt_Incident.RelatedToAllSiteFlag = (dat.CustomerRelateType == "2") ? true : false;
            result.tbt_Incident.RelatedToAllContractFlag = (dat.SiteRelateType == "2") ? true : false;
            //strStatus = strIncidentStatus;
            strStatus = commonhandler.GetMiscDisplayValue(MiscType.C_INCIDENT_STATUS, strIncidentStatus);

            foreach (CTS300_PersonIncharge picItem in dat.InChargeList)
            {
                tbt_IncidentRole roleItem = new tbt_IncidentRole()
                {
                    DepartmentCode = picItem.DepartmentCode,
                    EmpNo = picItem.EmpNo,
                    OfficeCode = picItem.OfficeCode,
                    IncidentRoleType = picItem.IncidentRoleCode
                };

                result.tbt_IncidentRole.Add(roleItem);
            }

            return result;
        }

        /// <summary>
        /// Mapping incident id to incident role
        /// </summary>
        /// <param name="dat"></param>
        /// <param name="IncidentID"></param>
        /// <returns></returns>
        private List<tbt_IncidentRole> AssignIncidentIDToRole_CTS300(List<tbt_IncidentRole> dat, int IncidentID)
        {
            if (dat != null)
            {
                foreach (var item in dat)
                {
                    item.IncidentID = IncidentID;
                }
            }

            return dat;
        }

        /// <summary>
        /// Send notify mail
        /// </summary>
        /// <param name="incidentID"></param>
        /// <param name="strMailTo"></param>
        private void SendMail_CTS300(int incidentID, string strMailTo, string strRelevantType, dsIncidentData IncidentData) //Add strMailTo, strRelevantType, IncidentData by Jutarat A. on 07092012
        {
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            IEmployeeMasterHandler emphandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
            EmailTemplateUtil mailUtil = new EmailTemplateUtil(EmailTemplateName.C_EMAIL_TEMPLATE_NAME_INCIDENT_IMP);

            //var mailBody = commonhandler.GetTbt_EmailTemplate(EmailTemplateName.C_EMAIL_TEMPLATE_NAME_INCIDENT_IMP);
            
            //Modify by Jutarat A. on 07092012
            string allMailAddr = "";
            if (String.IsNullOrEmpty(strMailTo))
            {
                var mailAddr = emphandler.GetEmailAddressForIncident(FlagType.C_FLAG_ON);
                foreach (var mailItem in mailAddr)
                {
                    if (allMailAddr.Length > 0)
                    {
                        allMailAddr += "; ";
                    }
                    allMailAddr += mailItem.EmailAddress;
                }
            }
            else
            {
                allMailAddr = strMailTo;
            }
            //End Modify

            // dummie data
            //allMailAddr = "natthavat28@gmail.com; natthavat.s@me.com; natthavat@csithai.com";
            string incidentLinkEN = CommonUtil.GenerateCompleteURL("Contract", "CTS330", String.Format("strIncidentID={0}", incidentID.ToString()), CommonValue.DEFAULT_SHORT_LANGUAGE_EN);
            string incidentLinkLC = CommonUtil.GenerateCompleteURL("Contract", "CTS330", String.Format("strIncidentID={0}", incidentID.ToString()), CommonValue.DEFAULT_SHORT_LANGUAGE_LC); //Add by Jutarat A. on 28092012

            //if (mailBody.Count == 0)
            //{
            //    mailBody.Add(new tbs_EmailTemplate()
            //    {
            //        TemplateContent = "{1} is added.",
            //        TemplateSubject = "[CTS300]Hello World !"
            //    });
            //}
            doEmailWithURL templateObj = new doEmailWithURL();
            templateObj.ViewURL = incidentLinkEN;
            templateObj.ViewURLLC = incidentLinkLC; //Add by Jutarat A. on 28092012

            //Add by Jutarat A. on 16072013
            if (IncidentData != null && IncidentData.tbt_Incident != null)
            {
                CommonUtil comUtil = new CommonUtil();

                string strIncidentRelatedCode = string.Empty;
                if (strRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CUSTOMER)
                {
                    templateObj.IncidentRelatedCode = comUtil.ConvertCustCode(IncidentData.tbt_Incident.CustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                }
                else if (strRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_SITE)
                {
                    templateObj.IncidentRelatedCode = comUtil.ConvertSiteCode(IncidentData.tbt_Incident.SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                }
                else if (strRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_PROJECT)
                {
                    templateObj.IncidentRelatedCode = IncidentData.tbt_Incident.ProjectCode;
                }
                else if (strRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CONTRACT)
                {
                    templateObj.IncidentRelatedCode = comUtil.ConvertContractCode(IncidentData.tbt_Incident.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                }

                templateObj.IncidentTitle = IncidentData.tbt_Incident.IncidentTitle;
                templateObj.IncidentNo = IncidentData.tbt_Incident.IncidentNo;

                string strReceivedType = string.Empty;
                List<doMiscTypeCode> miscTypeCodeList = new List<doMiscTypeCode>()
                {
                    new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_INCIDENT_RECEIVED_METHOD,
                        ValueCode = IncidentData.tbt_Incident.ReceivedMethod
                    }
                };

                List<doMiscTypeCode> miscTypeList = commonhandler.GetMiscTypeCodeList(miscTypeCodeList);
                if (miscTypeList != null && miscTypeList.Count > 0)
                {
                    strReceivedType = miscTypeList[0].ValueDisplayEN;
                }

                templateObj.ContactDetail = String.Format("{0} {1} {2} {3} {4}"
                                            , CommonUtil.TextDate(IncidentData.tbt_Incident.ReceivedDate)
                                            , CommonUtil.TextTime(IncidentData.tbt_Incident.ReceivedTime)
                                            , strReceivedType
                                            , IncidentData.tbt_Incident.ContactPerson
                                            , IncidentData.tbt_Incident.ContactPersonDep);

                templateObj.ReceivedDetail = IncidentData.tbt_Incident.ReceivedDetail;
            }
            //End Add

            var mailTemplate = mailUtil.LoadTemplate(templateObj);

            doEmailProcess mailMsg = new doEmailProcess();
            mailMsg.MailTo = allMailAddr;
            mailMsg.MailFrom = CommonUtil.dsTransData.dtUserData.EmailAddress;
            mailMsg.MailFromAlias = null;
            mailMsg.Message = mailTemplate.TemplateContent;
            mailMsg.Subject = mailTemplate.TemplateSubject;
            mailMsg.IsBodyHtml = true; //Add by Jutarat A. on 16072013

            //Modify by Jutarat A. on 27092012
            //commonhandler.SendMail(mailMsg);
            SendMailObject obj = new SendMailObject();
            obj.EmailList = new List<doEmailProcess>();
            obj.EmailList.Add(mailMsg);

            System.Threading.Thread t = new System.Threading.Thread(SendMail);
            t.Start(obj);
            //End Modify
        }

        /// <summary>
        /// Validate system suspend and user permission
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        private ObjectResultData ValidateAutority_CTS300(ObjectResultData res)
        {
            //Check System Suspend
            if (CheckIsSuspending(res))
            {
                res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0049, null, null);
                return res;
            }

            if (!CheckUserPermission(ScreenID.C_SCREEN_ID_REGISTER_INCIDENT))
            {
                res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0053);
                return res;
            }

            return res;
        }

        /// <summary>
        /// Validate parameter
        /// </summary>
        /// <param name="param"></param>
        /// <param name="res"></param>
        /// <returns></returns>
        private ObjectResultData ValidateParameter_CTS300(CTS300_ScreenParameter param, ObjectResultData res)
        {
            CommonUtil util = new CommonUtil();
            ICustomerMasterHandler customerhandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
            ISiteMasterHandler sitehandler = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;
            IRentralContractHandler rentalhandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            ISaleContractHandler salehandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
            IProjectHandler projecthandler = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;

            if (!String.IsNullOrEmpty(param.strIncidentRelevantCode) && !string.IsNullOrEmpty(param.strIncidentRelevantType))
            {
                if (param.strIncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CUSTOMER)
                {
                    var custDat = customerhandler.GetCustomer(util.ConvertCustCode(param.strIncidentRelevantCode, CommonUtil.CONVERT_TYPE.TO_LONG));

                    if ((custDat == null) || (custDat.Count != 1))
                    {
                        res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0011, new string[] { param.strIncidentRelevantCode }, null);
                    }
                }
                else if (param.strIncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_SITE)
                {
                    var siteDat = sitehandler.GetSite(util.ConvertSiteCode(param.strIncidentRelevantCode, CommonUtil.CONVERT_TYPE.TO_LONG), null);

                    if ((siteDat == null) || (siteDat.Count != 1))
                    {
                        res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0011, new string[] { param.strIncidentRelevantCode }, null);
                    }
                }
                else if (param.strIncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CONTRACT)
                {
                    var rentalContractDat = rentalhandler.GetTbt_RentalContractBasic(util.ConvertContractCode(param.strIncidentRelevantCode, CommonUtil.CONVERT_TYPE.TO_LONG), null);
                    var saleContractDat = salehandler.GetTbt_SaleBasic(util.ConvertContractCode(param.strIncidentRelevantCode, CommonUtil.CONVERT_TYPE.TO_LONG), null, null);

                    if ((rentalContractDat != null) && (rentalContractDat.Count > 0))
                    {
                        // Data Exists
                    }
                    else
                    {
                        if ((saleContractDat != null) && (saleContractDat.Count > 0))
                        {
                            // Data Exists
                        }
                        else
                        {
                            res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0011, new string[] { param.strIncidentRelevantCode }, null);
                        }
                    }
                }
                else if (param.strIncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_PROJECT)
                {
                    var projectDat = projecthandler.GetTbt_Project(param.strIncidentRelevantCode);

                    if ((projectDat == null) || (projectDat.Count != 1))
                    {
                        res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0011, new string[] { param.strIncidentRelevantCode }, null);
                    }
                }
            }

            return res;
        }
        
        //private void SetScreenParameter_CTS300(CTS300_ScreenParameter obj)
        //{
        //    Session.Remove("CTS300_PARAM");
        //    Session.Add("CTS300_PARAM", obj);
        //}

        //private CTS300_ScreenParameter GetScreenObject_CTS300()
        //{
        //    CTS300_ScreenParameter obj = null;

        //    if (Session["CTS300_PARAM"] != null)
        //    {
        //        obj = (CTS300_ScreenParameter)Session["CTS300_PARAM"];
        //    }

        //    return obj;
        //}
    }
}
