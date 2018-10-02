//*********************************
// Create by: Natthavat S. 
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
using SECOM_AJIS.DataEntity.Quotation;
using CSI.WindsorHelper;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Presentation.Contract.Models;
using System.Transactions;

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
        public ActionResult CTS350_Authority(CTS350_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                ////Check System Suspend
                //if (CheckIsSuspending(res))
                //{
                //    res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0049, null, null);
                //    return Json(res);
                //}

                ////Check Screen Permission
                //if (!CheckUserPermission(ScreenID.C_SCREEN_ID_REGISTER_AR))
                //{
                //    res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0053, null, null);
                //    return Json(res);
                //}

                res = ValidateAuthority_CTS350(res);
                if (res.IsError)
                {
                    return Json(res);
                }

                res = ValidateParameter_CTS350(param, res);
                if (res.IsError)
                {
                    return Json(res);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<CTS350_ScreenParameter>("CTS350", param, res);
        }

        //public ActionResult CTS350_Authority(string param, string strARRelevantType, string strARRelevantCode)
        //{
        //    ObjectResultData res = new ObjectResultData();
        //    CTS350_ScreenParameter sparam = null;

        //    try
        //    {
        //         //Check System Suspend
        //        if (CheckIsSuspending(res))
        //        {
        //            res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0049, null, null);
        //            return Json(res);
        //        }

        //         //Check Screen Permission
        //        if (!CheckUserPermission(ScreenID.C_SCREEN_ID_REGISTER_AR))
        //        {
        //            res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0053, null, null);
        //            return Json(res);
        //        }

        //        if (!String.IsNullOrEmpty(strARRelevantType) && !string.IsNullOrEmpty(strARRelevantCode))
        //        {
        //            sparam = new CTS350_ScreenParameter()
        //            {
        //                strARRelevantCode = strARRelevantCode,
        //                strARRelevantType = strARRelevantType
        //            };
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        res.AddErrorMessage(ex);
        //    }

        //    return InitialScreenEnvironment("CTS350", sparam, null);
        //}

        #endregion

        #region Action

        /// <summary>
        /// Initial screen
        /// </summary>
        /// <returns></returns>
        [Initialize("CTS350")]
        public ActionResult CTS350()
        {
            ViewBag.AttachKey = GetCurrentKey();
            return View();
        }

        /// <summary>
        /// Initial contract grid schema
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS350_InitialContractGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Contract\\CTS350_ContractTarget", CommonUtil.GRID_EMPTY_TYPE.VIEW));
        }

        /// <summary>
        /// Initial user groups schema
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS350_InitialUserGroupGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Contract\\CTS350_CustomerGroup"));
        }

        /// <summary>
        /// Initial person in charge grid schema
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS350_InitialAssignPersonInChargeGrid()
        {
            //ObjectResultData res = new ObjectResultData();

            //try
            //{

            //}
            //catch (Exception ex)
            //{
            //    res.AddErrorMessage(ex);
            //}

            //return Json(res);

            return Json(CommonUtil.ConvertToXml<object>(null, "Contract\\CTS350_AssignPIC", CommonUtil.GRID_EMPTY_TYPE.VIEW));
        }

        /// <summary>
        /// Initial attach document grid
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS350_IntialGridAttachedDocList()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Contract\\CTS350_AttachedDocList", CommonUtil.GRID_EMPTY_TYPE.VIEW));
        }

        /// <summary>
        /// Retrieve attach document grid data
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS350_LoadGridAttachedDocList()
        {
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            ObjectResultData res = new ObjectResultData();

            try
            {
                List<dtAttachFileForGridView> lstAttachedName = commonhandler.GetAttachFileForGridView(GetCurrentKey());
                res.ResultData = CommonUtil.ConvertToXml<dtAttachFileForGridView>(lstAttachedName, "Contract\\CTS350_AttachedDocList", CommonUtil.GRID_EMPTY_TYPE.VIEW);
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
        public ActionResult CTS350_AttachFile(HttpPostedFileBase fileSelect, string DocumentName, string k)
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

            return View("CTS350_Upload");
        }

        /// <summary>
        /// Remove exists attach document
        /// </summary>
        /// <param name="AttachID"></param>
        /// <returns></returns>
        public ActionResult CTS350_RemoveAttach(string AttachID)
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
        public ActionResult CTS350_ClearAttach()
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
        public ActionResult CTS350_Upload()
        {
            ViewBag.K = GetCurrentKey();
            return View("CTS350_Upload");
        }

        /// <summary>
        /// Retrieve ar title when change select [ARType] drop down list
        /// </summary>
        /// <param name="ARType"></param>
        /// <returns></returns>
        public ActionResult CTS350_RetrieveARTitle(string ARType)
        {
            IARHandler arhandler = ServiceContainer.GetService<IARHandler>() as IARHandler;
            ObjectResultData res = new ObjectResultData();
            CommonUtil util = new CommonUtil();

            try
            {
                List<tbs_ARTypeTitle> arRes = arhandler.GetTbs_ARTypeTitle(ARType, null);
                CommonUtil.MappingObjectLanguage<tbs_ARTypeTitle>(arRes);
                if (arRes.Count > 0)
                {
                    res.ResultData = CommonUtil.CommonComboBox<tbs_ARTypeTitle>("{BLANK_ID}", arRes
                        , "ARTitleName"
                        , "ARTitleType"
                        , null
                        , true).ToString();
                }
                else
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3227, null, new string[] { "ARType" });
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Retrieve ar purpose when change select [ARTitle] drop down list
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="contractLst"></param>
        /// <param name="strARType"></param>
        /// <param name="strARTitle"></param>
        /// <returns></returns>
        public ActionResult CTS350_RetrieveARPurpose(CTS350_RetrieveCondition cond, List<CTS350_TargetContract> contractLst, string strARType, string strARTitle)
        {
            IQuotationHandler quotehandler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            IARHandler arhandler = ServiceContainer.GetService<IARHandler>() as IARHandler;
            ObjectResultData res = new ObjectResultData();
            CTS350_ARPurposeResult obj = new CTS350_ARPurposeResult();
            CommonUtil util = new CommonUtil();

            try
            {
                //cond.ARRelevantType = ConvertARRelevantType_CTS350(cond.ARRelevantType);

                if ((String.IsNullOrEmpty(strARTitle)) || (String.IsNullOrEmpty(strARType)))
                {
                    obj.EnableGrid1 = false;
                    obj.EnableGrid2 = false;
                }
                else
                {
                    if ((strARTitle == ARTitle.C_AR_TITLE_OTHER) && (strARType == ARType.C_AR_TYPE_OTHER))
                    {
                        obj.ARPurpose = String.Empty;
                    }
                    else
                    {
                        List<tbs_ARTypePattern> lst = arhandler.GetTbs_ARTypePattern(strARType, strARTitle);
                        CommonUtil.MappingObjectLanguage<tbs_ARTypePattern>(lst);
                        if (lst.Count == 1)
                        {
                            obj.ARPurpose = lst[0].ARTypeContent;
                        }
                    }

                    var curType = arhandler.GetTbs_ARTypeTitle(strARType, strARTitle);
                    //if ((curType != null) && (curType[0].FeeAdjustmentFlag.GetValueOrDefault() == FlagType.C_FLAG_ON))
                    if ((curType != null && curType.Count() > 0) && (curType[0].FeeAdjustmentFlag.GetValueOrDefault() == FlagType.C_FLAG_ON)) //Modify by Jutarat A. on 25092013
                    {
                        if (cond.ARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_CONTRACT)
                        {
                            var cntRental = (from a in contractLst where a.ServiceType == ServiceType.C_SERVICE_TYPE_RENTAL select a);
                            var cntSale = (from a in contractLst where a.ServiceType == ServiceType.C_SERVICE_TYPE_SALE select a);

                            if (cntRental.Count() == contractLst.Count)
                            {
                                obj.EnableGrid1 = true;
                                obj.EnableGrid2 = false;
                            }
                            else if (cntSale.Count() == contractLst.Count)
                            {
                                obj.EnableGrid1 = false;
                                obj.EnableGrid2 = true;
                            }
                            else
                            {
                                obj.EnableGrid1 = true;
                                obj.EnableGrid2 = true;
                            }
                        }
                        else if (cond.ARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_QUOTATION)
                        {
                            var quoteItem = quotehandler.GetTbt_QuotationTarget(new doGetQuotationDataCondition()
                            {
                                QuotationTargetCode = util.ConvertQuotationTargetCode(cond.QuotationCode, CommonUtil.CONVERT_TYPE.TO_LONG)
                            });

                            if (quoteItem.Count == 1)
                            {
                                if (quoteItem[0].ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                                {
                                    obj.EnableGrid1 = true;
                                    obj.EnableGrid2 = false;
                                }
                                else if (quoteItem[0].ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                                {
                                    obj.EnableGrid1 = false;
                                    obj.EnableGrid2 = true;
                                }
                                else
                                {
                                    obj.EnableGrid1 = true;
                                    obj.EnableGrid2 = true;
                                }
                            }
                            else
                            {
                                obj.EnableGrid1 = true;
                                obj.EnableGrid2 = true;
                            }
                        }
                        else
                        {
                            obj.EnableGrid1 = true;
                            obj.EnableGrid2 = true;
                        }
                    }
                    else
                    {
                        obj.EnableGrid1 = false;
                        obj.EnableGrid2 = false;
                    }
                }

                res.ResultData = obj;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Checking parameter and retrieve AR data
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS350_RetrieveARDataFromParameter()
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil util = new CommonUtil();

            try
            {
                CTS350_ScreenParameter sParam = GetScreenObject<CTS350_ScreenParameter>();
                if ((sParam != null) && !String.IsNullOrEmpty(sParam.strARRelevantCode) && !String.IsNullOrEmpty(sParam.strARRelevantType))
                {
                    CTS350_RetrieveCondition condition = new CTS350_RetrieveCondition();

                    if (sParam.strARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_CUSTOMER)
                    {
                        condition.ARRelevantType = ARRelevant.C_AR_RELEVANT_TYPE_CUSTOMER;
                        condition.CustomerCode = sParam.strARRelevantCode;
                    } 
                    else if (sParam.strARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_SITE)
                    {
                        condition.ARRelevantType = ARRelevant.C_AR_RELEVANT_TYPE_SITE;
                        condition.SiteCode = sParam.strARRelevantCode;
                    }
                    else if (sParam.strARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_QUOTATION)
                    {
                        condition.ARRelevantType = ARRelevant.C_AR_RELEVANT_TYPE_QUOTATION;
                        condition.QuotationCode = sParam.strARRelevantCode;
                    }
                    else if (sParam.strARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_PROJECT)
                    {
                        condition.ARRelevantType = ARRelevant.C_AR_RELEVANT_TYPE_PROJECT;
                        condition.ProjectCode = sParam.strARRelevantCode;
                    }
                    else if (sParam.strARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_CONTRACT)
                    {
                        condition.ARRelevantType = ARRelevant.C_AR_RELEVANT_TYPE_CONTRACT;
                        condition.ContractCode = sParam.strARRelevantCode;
                    }

                    res.ResultData = condition;
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Retrieve department item for combobox when change select [Office] drop down list on Assign person in charge information subsection
        /// </summary>
        /// <param name="OfficeCode"></param>
        /// <returns></returns>
        public ActionResult CTS350_RetrieveDepartmentCBB(string OfficeCode)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil util = new CommonUtil();
            IEmployeeMasterHandler emphandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
            IOfficeMasterHandler officehandler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler; //Add by jutarat A. on 20082012

            CTS350_ScreenParameter param = GetScreenObject<CTS350_ScreenParameter>(); //Add by jutarat A. on 20082012

            try
            {
                if (String.IsNullOrEmpty(OfficeCode))
                {
                    res.ResultData = String.Empty;
                }
                else
                {
                    //Add by jutarat A. on 20082012
                    param.blnIncidentIsHeadOfficeFlag = officehandler.CheckHeadOffice(OfficeCode);
                    if (param.blnIncidentIsHeadOfficeFlag == false)
                    {
                        res.ResultData = false;
                    }
                    //End Add
                    else
                    {
                        var dat = emphandler.GetBelongingDepartmentList(OfficeCode, null);
                        if (dat.Count == 0)
                        {
                            res.ResultData = String.Empty;
                        }
                        else
                        {
                            res.ResultData = CommonUtil.CommonComboBox<dtDepartment>("{BLANK_ID}", dat.ToList(), "DepartmentNameCode", "DepartmentCode", null, true).ToString();
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
        /// Retrieve AR role item for combobox when change select [Department] drop down list on Assign person in charge information subsection
        /// </summary>
        /// <param name="OfficeCode"></param>
        /// <param name="DepartmentCode"></param>
        /// <returns></returns>
        public ActionResult CTS350_RetrieveARRoleCBB(string OfficeCode, string DepartmentCode)
        {
            ObjectResultData res = new ObjectResultData();
            CTS350_ScreenParameter param = GetScreenObject<CTS350_ScreenParameter>(); //Add by jutarat A. on 20082012

            try
            {
                //Modify by jutarat A. on 20082012
                //if (string.IsNullOrEmpty(DepartmentCode))
                //{
                //    res.ResultData = string.Empty;
                //}
                if (string.IsNullOrEmpty(DepartmentCode) == false || param.blnIncidentIsHeadOfficeFlag == false)
                {
                    List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
                    try
                    {
                        List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                    {
                        new doMiscTypeCode()
                        {
                            FieldName = MiscType.C_AR_ROLE,
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

                    res.ResultData = CommonUtil.CommonComboBox<doMiscTypeCode>("{BLANK_ID}", lst, display, "ValueCode", null, true).ToString();
                }
                else
                {
                    res.ResultData = string.Empty;
                }
                //End Modify
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Retrieve employee item for combobox when change select [AR Role] drop down list on Assign person in charge information subsection
        /// </summary>
        /// <param name="OfficeCode"></param>
        /// <param name="DepartmentCode"></param>
        /// <param name="ARRoleCode"></param>
        /// <returns></returns>
        public ActionResult CTS350_RetrieveEmployeeCBB(string OfficeCode, string DepartmentCode, string ARRoleCode)
        {
            IEmployeeMasterHandler emphandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
            ObjectResultData res = new ObjectResultData();

            CTS350_ScreenParameter param = GetScreenObject<CTS350_ScreenParameter>(); //Add by jutarat A. on 20082012

            try
            {
                //if (string.IsNullOrEmpty(OfficeCode) || string.IsNullOrEmpty(DepartmentCode) || string.IsNullOrEmpty(ARRoleCode))
                if (string.IsNullOrEmpty(OfficeCode) || string.IsNullOrEmpty(ARRoleCode)) //Modify by jutarat A. on 20082012
                {
                    res.ResultData = string.Empty;
                } else
                {
                    var datTemp = emphandler.GetBelongingEmpList(OfficeCode
                        , param.blnIncidentIsHeadOfficeFlag == false ? null : DepartmentCode //DepartmentCode  //Modify by jutarat A. on 20082012
                        , null
                        //, ((ARRoleCode == ARRole.C_AR_ROLE_APPROVER) || (ARRoleCode == ARRole.C_AR_ROLE_AUDITOR)) ? FlagType.C_FLAG_ON : (bool?)null //FlagType.C_FLAG_OFF //Modify by jutarat A. on 17042013
                        , ((ARRoleCode == ARRole.C_AR_ROLE_APPROVER)) ? FlagType.C_FLAG_ON : (bool?)null //Modify by jutarat A. on 26042013
                        , null);

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

                    List<dtEmployeeBelonging> empLst = datDist.ToList<dtEmployeeBelonging>();
                    //End Add

                    CommonUtil.MappingObjectLanguage<dtEmployeeBelonging>(empLst);

                    res.ResultData = CommonUtil.CommonComboBox<dtEmployeeBelonging>("{BLANK_ID}"
                        , empLst, "EmpFullNameWithCode", "EmpNo", null, true).ToString();
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Retrieve AR data when click any [Retrieve] button on AR Relevant Information section
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CTS350_RetrieveARData(CTS350_RetrieveCondition cond)
        {
            ObjectResultData res = new ObjectResultData();
            ICustomerMasterHandler custhandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
            IViewContractHandler viewcontracthandler = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
            IEmployeeMasterHandler emphandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            IOfficeMasterHandler officehandler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler; //Add by jutarat A. on 11102012

            CommonUtil util = new CommonUtil();
            CTS350_ARData dat = new CTS350_ARData();
            bool isValid = true;

            try
            {
                commonhandler.ClearTemporaryUploadFile(GetCurrentKey());

                //cond.ARRelevantType = ConvertARRelevantType_CTS350(cond.ARRelevantType);

                if (cond.ARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_CUSTOMER)
                {
                    var resCust = custhandler.GetCustomerDataForSearch(new doCustomerSearchCondition()
                    {
                        CustomerCode = util.ConvertCustCode(cond.CustomerCode, CommonUtil.CONVERT_TYPE.TO_LONG)
                    });

                    if (resCust.Count != 1)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { cond.CustomerCode }, new string[] { "CustomerCode" });
                    }
                    else
                    {
                        CommonUtil.MappingObjectLanguage<dtCustomerData>(resCust);

                        dat.CustomerData = new CTS350_ARData_Customer()
                        {
                            CustCode = util.ConvertCustCode(resCust[0].CustCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                            AddressFullEN = resCust[0].AddressFullEN,
                            AddressFullLC = resCust[0].AddressFullLC,
                            CustFullNameEN = resCust[0].CustFullNameEN,
                            CustFullNameLC = resCust[0].CustFullNameLC,
                            IDNo = resCust[0].IDNo,
                            PhoneNo = resCust[0].PhoneNo,
                            URL = resCust[0].URL,
                            BusinessTypeName = resCust[0].BusinessTypeName,
                            Nationality = resCust[0].Nationality,
                        };
                    }
                }
                else if (cond.ARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_SITE)
                {
                    var resSite = viewcontracthandler.GetSiteInfo(util.ConvertSiteCode(cond.SiteCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                    if ((resSite.doGetTbm_Site != null) && (resSite.doGetTbm_Site.Count == 1))
                    {
                        CommonUtil.MappingObjectLanguage<tbm_BuildingUsage>(resSite.tbm_BuildingUsage);

                        dat.SiteData = new CTS350_ARData_Site()
                        {
                            SiteCode = util.ConvertSiteCode(resSite.doGetTbm_Site[0].SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                            AddressFullEN = resSite.doGetTbm_Site[0].AddressFullEN,
                            AddressFullLC = resSite.doGetTbm_Site[0].AddressFullLC,
                            PhoneNo = resSite.doGetTbm_Site[0].PhoneNo,
                            SiteNameEN = resSite.doGetTbm_Site[0].SiteNameEN,
                            SiteNameLC = resSite.doGetTbm_Site[0].SiteNameLC,
                            Usage = (resSite.tbm_BuildingUsage.Count > 0) ? resSite.tbm_BuildingUsage[0].BuildingUsageName : ""
                        };
                    } else
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { cond.SiteCode }, new string[] { "SiteCode" });
                    }
                }
                else if (cond.ARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_QUOTATION)
                {
                    dat.QuotationData = RetrieveQuotationTargetInfo_CTS350(cond.QuotationCode);
                    if (dat.QuotationData == null)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { cond.QuotationCode }, new string[] { "QuotationCode" });
                    } else if (!String.IsNullOrEmpty(dat.QuotationData.ContractCode))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3074, new string[] { cond.QuotationCode }, new string[] { "QuotationCode" });
                    }
                }
                else if (cond.ARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_PROJECT)
                {
                    var projRes = viewcontracthandler.GetProjectInfo(cond.ProjectCode);
                    if ((projRes.tbt_Project == null) || (projRes.tbt_Project.Count != 1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { cond.ProjectCode }, new string[] { "ProjectCode" });
                    }
                    else
                    {
                        dat.ProjectData = new CTS350_ARData_Project()
                        {
                            ProjectCode = projRes.tbt_Project[0].ProjectCode,
                            ProjectName = projRes.tbt_Project[0].ProjectName,
                            ProjectAddress = projRes.tbt_Project[0].ProjectAddress,
                            ProjectPurchaseNameEN = projRes.dtProjectPurcheser.CustFullNameEN,
                            ProjectPurchaseNameLC = projRes.dtProjectPurcheser.CustFullNameLC
                        };
                    }
                }
                else if (cond.ARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_CONTRACT)
                {
                    dat.ContractData = RetrieveContractInfo_CTS350(cond.ContractCode
                        , util.ConvertContractCode(cond.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG));

                    if (dat.ContractData == null)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { cond.ContractCode }, new string[] { "UserCode_ContractCode" });
                    }
                }
                else
                {
                    isValid = false;
                }

                if (isValid && !res.IsError)
                {
                    var picRes = (from a in
                                      emphandler.GetBelonging(null, null, null, CommonUtil.dsTransData.dtUserData.EmpNo)
                                  where a.MainDepartmentFlag == FlagType.C_FLAG_ON
                                  select a).ToList();

                    List<doMiscTypeCode> miscs = new List<doMiscTypeCode>();
                    List<doMiscTypeCode> lst = new List<doMiscTypeCode>();

                    miscs.Add(new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_AR_ROLE,
                        ValueCode = ARRole.C_AR_ROLE_REQUESTER
                    });

                    lst = commonhandler.GetMiscTypeCodeList(miscs);
                    CommonUtil.MappingObjectLanguage<doMiscTypeCode>(lst);
                    CommonUtil.MappingObjectLanguage<dtBelonging>(picRes);

                    //Modify by jutarat A. on 11102012
                    if (picRes != null && picRes.Count > 0)
                    {
                        bool blnIncidentIsHeadOfficeFlag = officehandler.CheckHeadOffice(picRes[0].OfficeCode);
                        
                        dat.PersonInCharge = new CTS350_PersonInCharge()
                        {
                            OfficeCode = picRes[0].OfficeCode,
                            DepartmentCode = blnIncidentIsHeadOfficeFlag? picRes[0].DepartmentCode : null, //picRes[0].DepartmentCode,
                            DepartmentName = blnIncidentIsHeadOfficeFlag? picRes[0].DepartmentName : null, //picRes[0].DepartmentName,
                            ARRoleCode = ARRole.C_AR_ROLE_REQUESTER,
                            SendMail = false,
                            EmpNo = picRes[0].EmpNo,
                            CanRemove = false
                        };                        
                    }
                    //End Modify

                    dat.PersonInCharge.ARRoleName = lst[0].ValueDisplay;
                    dat.PersonInCharge.EmployeeName = String.Format("{0} {1}", picRes[0].EmpFirstName, picRes[0].EmpLastName);
                    dat.PersonInCharge.OfficeName = picRes[0].OfficeName;

                    res.ResultData = dat;
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Retrieve customer groups grid data when retrieve customer data
        /// </summary>
        /// <param name="strCustomerCode"></param>
        /// <returns></returns>
        public ActionResult CTS350_RetrieveCustomerGroup(string strCustomerCode)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                CommonUtil util = new CommonUtil();
                ICustomerMasterHandler custhandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;

                List<dtCustomeGroupData> searchRes = custhandler.GetCustomeGroupData(util.ConvertCustCode(strCustomerCode, CommonUtil.CONVERT_TYPE.TO_LONG), null);

                res.ResultData = CommonUtil.ConvertToXml<dtCustomeGroupData>(searchRes, "Contract\\CTS350_CustomerGroup", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Validate user authority, require field, and business before register
        /// </summary>
        /// <param name="regisObj"></param>
        /// <returns></returns>
        public ActionResult CTS350_ValidateData(CTS350_ARRegisForm regisObj)
        {
            ObjectResultData res = new ObjectResultData();

            List<string> controlLst = new List<string>()
            , labelList = new List<string>();

            try
            {
                res = ValidateAuthority_CTS350(res);

                if (res.IsError)
                {
                    return Json(res);
                }

                if (String.IsNullOrEmpty(regisObj.ARTypeCode))
                {
                    controlLst.Add("ARType");
                    labelList.Add("lblARType");
                }

                if (String.IsNullOrEmpty(regisObj.ARTitle))
                {
                    controlLst.Add("ARTitle");
                    labelList.Add("lblARTitle");
                }

                if (String.IsNullOrEmpty(regisObj.ARSubTitle))
                {
                    controlLst.Add("ARSubTitle");
                    labelList.Add("lblARSubTitle");
                }

                if (String.IsNullOrEmpty(regisObj.ARPurpose))
                {
                    controlLst.Add("ARPurpose");
                    labelList.Add("lblARPurpose");
                }

                //Comment by Jutarat A. on 03092012
                //if (regisObj.DueDate_Deadline_Type == "1")
                //{
                //    if (!regisObj.DueDate_Date.HasValue || !regisObj.DueDate_Time.HasValue)
                //    {
                //        if (!regisObj.DueDate_Date.HasValue)
                //            controlLst.Add("DueDate_Date");

                //        if (!regisObj.DueDate_Time.HasValue)
                //            controlLst.Add("DueDate_Time");

                //        labelList.Add("rdoDueDate");
                //    }
                //}
                //else if (regisObj.DueDate_Deadline_Type == "2")
                //{
                //    if (!regisObj.Deadline_Date.HasValue || String.IsNullOrEmpty(regisObj.Deadline_Until))
                //    {
                //        if (!regisObj.Deadline_Date.HasValue)
                //            controlLst.Add("Deadline_Date");

                //        if (String.IsNullOrEmpty(regisObj.Deadline_Until))
                //            controlLst.Add("Deadline_Until");

                //        labelList.Add("rdoDeadLine");
                //    }
                //}
                //End Comment

                if (labelList.Count > 0)
                {
                    var labelTextList = new List<string>();

                    foreach (var lblName in labelList)
                    {
                        labelTextList.Add(CommonUtil.GetLabelFromResource("Contract", "CTS350", lblName));
                    }

                    res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0007, new string[] { CommonUtil.TextList(labelTextList.ToArray(), ", ") }, controlLst.ToArray());
                }

                if (res.IsError)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }

                // Validate Approver
                var approverLst = from a in regisObj.PersonInChargeData where a.ARRoleCode == ARRole.C_AR_ROLE_APPROVER select a;

                //Add by Jutarat A. on 30082012
                //Cannot register only auditor, must register Approver at least
                var auditorLst = from a in regisObj.PersonInChargeData where a.ARRoleCode == ARRole.C_AR_ROLE_AUDITOR select a;
                if (auditorLst != null && auditorLst.Count() > 0)
                {
                    if (approverLst != null && approverLst.Count() == 0)
                    {
                        res.AddErrorMessage("Contract", MessageUtil.MessageList.MSG3301, null, new string[] { "ARRoleCode" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return Json(res);
                    }
                }
                //End Add

                if (approverLst != null && approverLst.Count() == 0)
                {
                    res.AddErrorMessage("Contract", MessageUtil.MessageList.MSG3149, null, new string[] { "ARRoleCode" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }


                //if (regisObj.DueDate_Deadline_Type == "1")
                //{
                //    if (!regisObj.DueDate_Date.HasValue || !regisObj.DueDate_Time.HasValue)
                //    {
                //        res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0007, new string[] { CommonUtil.GetLabelFromResource("Contract", "CTS350", "lblDueDate_DeadLine") }, new string[] { "DueDate_Date", "DueDate_Time" });
                //        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                //        return Json(res);
                //    }
                //}
                //else
                //{
                //    if (!regisObj.Deadline_Date.HasValue || String.IsNullOrEmpty(regisObj.Deadline_Until))
                //    {
                //        res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0007, new string[] { CommonUtil.GetLabelFromResource("Contract", "CTS350", "lblDueDate_DeadLine") }, new string[] { "Deadline_Date", "Deadline_Until" });
                //        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                //        return Json(res);
                //    }
                //}

                //Add by Jutarat A. on 06092012
                //Validate warning
                //Check specify e-mail address of person incharge
                IEmployeeMasterHandler emphandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                List<dtEmployee> lstEmployee = emphandler.GetEmployee(null, null, null);

                StringBuilder sbMailEmpName = new StringBuilder();
                StringBuilder sbMailTo = new StringBuilder();
                foreach (var item in regisObj.PersonInChargeData)
                {
                    if (item.SendMail)
                    {
                        List<dtEmployee> emp = (from t in lstEmployee where t.EmpNo == item.EmpNo select t).ToList<dtEmployee>();
                        if (emp == null || emp.Count == 0 || String.IsNullOrEmpty(emp[0].EmailAddress))
                        {
                            sbMailEmpName.AppendFormat("{0}, ", item.EmployeeName);
                        }
                        else
                        {
                            sbMailTo.AppendFormat("{0}; ", emp[0].EmailAddress);
                        }
                    }
                }

                if (sbMailEmpName.Length > 1)
                {
                    string strMailEmpName = sbMailEmpName.ToString().Substring(0, sbMailEmpName.Length - 2);
                    if (String.IsNullOrEmpty(strMailEmpName) == false)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3305, new string[] { strMailEmpName }, null);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                    }
                }

                if (sbMailTo.Length > 1)
                {
                    string strMailTo = sbMailTo.ToString().Substring(0, sbMailTo.Length - 2);
                    if (String.IsNullOrEmpty(strMailTo) == false)
                    {
                        CTS350_ScreenParameter param = GetScreenObject<CTS350_ScreenParameter>();
                        param.MailTo = strMailTo;
                    }
                }
                //End Add

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Register new AR
        /// </summary>
        /// <param name="regisObj"></param>
        /// <returns></returns>
        public ActionResult CTS350_RegisNewAR(CTS350_ARRegisForm regisObj)
        {
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            ObjectResultData res = new ObjectResultData();
            CTS350_RegisResult obj = null;

            try
            {
                res = ValidateAuthority_CTS350(res);
                if (res.IsError)
                {
                    return Json(res);
                }

                using (TransactionScope scope = new TransactionScope())
                {
                    //regisObj.ARRelevantType = ConvertARRelevantType_CTS350(regisObj.ARRelevantType);
                    if (regisObj.ARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_CONTRACT)
                    {
                        for (int i = 0; i < regisObj.ContractData.Count; i++)
                        {
                            var regRes = RegisARData_CTS350(regisObj, ref res, regisObj.ContractData[i]);
                            if (regisObj.ContractData.Count == 1)
                            {
                                obj = regRes;
                            }
                        }

                        // Clear Attach File
                        commonhandler.ClearTemporaryUploadFile(GetCurrentKey());

                        if (regisObj.ContractData.Count != 1)
                        {
                            obj = new CTS350_RegisResult();
                            obj.RegisStatus = ARStatus.C_AR_STATUS_WAIT_FOR_APPROVAL;
                        }
                    }
                    else
                    {
                        obj = RegisARData_CTS350(regisObj, ref res);

                        // Clear Attach File
                        commonhandler.ClearTemporaryUploadFile(GetCurrentKey());
                    }

                    List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                    {
                        new doMiscTypeCode()
                        {
                            FieldName = MiscType.C_AR_STATUS,
                            ValueCode = ARStatus.C_AR_STATUS_WAIT_FOR_APPROVAL
                        }
                    };

                    var txtRes = commonhandler.GetMiscTypeCodeList(miscs);
                    CommonUtil.MappingObjectLanguage<doMiscTypeCode>(txtRes);
                    obj.RegisStatus = txtRes[0].ValueDisplay;

                    scope.Complete();
                }

                res.ResultData = obj;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        #endregion

        #region Method

        //private string ConvertARRelevantType_CTS350(string strRawARRelevantType)
        //{
        //    if (strRawARRelevantType == "1")
        //    {
        //        return ARRelevant.C_AR_RELEVANT_TYPE_CUSTOMER;
        //    }
        //    else if (strRawARRelevantType == "2")
        //    {
        //        return ARRelevant.C_AR_RELEVANT_TYPE_SITE;
        //    }
        //    else if (strRawARRelevantType == "3")
        //    {
        //        return ARRelevant.C_AR_RELEVANT_TYPE_QUOTATION;
        //    }
        //    else if (strRawARRelevantType == "4")
        //    {
        //        return ARRelevant.C_AR_RELEVANT_TYPE_PROJECT;
        //    }
        //    else if (strRawARRelevantType == "5")
        //    {
        //        return ARRelevant.C_AR_RELEVANT_TYPE_CONTRACT;
        //    }

        //    return null;
        //}

        /// <summary>
        /// Retrieve quotation data
        /// </summary>
        /// <param name="strQuotationTargetCode"></param>
        /// <returns></returns>
        private CTS350_ARData_Quotation RetrieveQuotationTargetInfo_CTS350(string strQuotationTargetCode)
        {
            CommonUtil util = new CommonUtil();
            IQuotationHandler quothandler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            IOfficeMasterHandler officehandler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
            CTS350_ARData_Quotation res = null;

            var quoteObj = quothandler.GetTbt_QuotationTarget(new doGetQuotationDataCondition()
            {
                QuotationTargetCode = util.ConvertQuotationTargetCode(strQuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG)
            });

            var quoteHeader = quothandler.GetQuotationHeaderData(new doGetQuotationDataCondition()
            {
                QuotationTargetCode = util.ConvertQuotationTargetCode(strQuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG) ,
                TargetCodeTypeCode = CustPartType.C_CUST_PART_TYPE_CONTRACT_TARGET
            });

            //var quoteCusObj = quothandler.GetTbt_QuotationCustomer(new doGetQuotationDataCondition()
            //{
            //    QuotationTargetCode = util.ConvertQuotationTargetCode(strQuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG),
            //    TargetCodeTypeCode = CustPartType.C_CUST_PART_TYPE_CONTRACT_TARGET
            //});

            //var quoteSiteObj = quothandler.GetTbt_QuotationSite(new doGetQuotationDataCondition()
            //{
            //    QuotationTargetCode = util.ConvertQuotationTargetCode(strQuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG)
            //});

            if (quoteObj.Count > 0)
            {
                res = new CTS350_ARData_Quotation();

                var operateOfficeObj = officehandler.GetTbm_Office(quoteObj[0].OperationOfficeCode);
                var quoteOfficeObj = officehandler.GetTbm_Office(quoteObj[0].QuotationOfficeCode);

                if (CommonUtil.CurrentLanguage(true) == CommonUtil.LANGUAGE_LIST.LANGUAGE_1)
                {
                    res.OperationOffice = (operateOfficeObj.Count > 0) ? operateOfficeObj[0].OfficeNameEN : "";
                    res.QuotationOffice = (quoteOfficeObj.Count > 0) ? quoteOfficeObj[0].OfficeNameEN : "";
                }
                else if (CommonUtil.CurrentLanguage(true) == CommonUtil.LANGUAGE_LIST.LANGUAGE_2)
                {
                    res.OperationOffice = (operateOfficeObj.Count > 0) ? operateOfficeObj[0].OfficeNameJP : "";
                    res.QuotationOffice = (quoteOfficeObj.Count > 0) ? quoteOfficeObj[0].OfficeNameJP : "";
                }
                else
                {
                    res.OperationOffice = (operateOfficeObj.Count > 0) ? operateOfficeObj[0].OfficeNameLC : "";
                    res.QuotationOffice = (quoteOfficeObj.Count > 0) ? quoteOfficeObj[0].OfficeNameLC : "";
                }

                res.QuotationCode = util.ConvertQuotationTargetCode(quoteObj[0].QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                res.ContractCode = quoteObj[0].ContractCode;
                res.ContractTargetEN = (quoteHeader != null) ? quoteHeader.doContractTarget.CustFullNameEN : "";
                res.ContractTargetLC = (quoteHeader != null) ? quoteHeader.doContractTarget.CustFullNameLC : "";
                res.SiteAddressEN = (quoteHeader != null) ? quoteHeader.doQuotationSite.AddressFullEN : "";
                res.SiteAddressLC = (quoteHeader != null) ? quoteHeader.doQuotationSite.AddressFullLC : "";
                res.SiteNameEN = (quoteHeader != null) ? quoteHeader.doQuotationSite.SiteNameEN : "";
                res.SiteNameLC = (quoteHeader != null) ? quoteHeader.doQuotationSite.SiteNameLC : "";
            }

            return res;
        }

        /// <summary>
        /// Retrieve contract data
        /// </summary>
        /// <param name="strUserCode"></param>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        private CTS350_ARData_Contract RetrieveContractInfo_CTS350(string strUserCode, string strContractCode)
        {
            CTS350_ARData_Contract obj = null;
            CommonUtil util = new CommonUtil();
            IRentralContractHandler rentalhandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            ISaleContractHandler salehandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;

            var contRes = rentalhandler.GetRentalContractBasicForView(strContractCode, strUserCode);
            if (contRes.Count > 0)
            {
                obj = new CTS350_ARData_Contract()
                {
                    ContractCode = contRes[0].ContractCode,
                    ShortContractCode = util.ConvertContractCode(contRes[0].ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                    //ContractName = String.Format("(1) {0}<br />(2) {1}", contRes[0].ContractTargetNameEN, contRes[0].ContractTargetNameLC),
                    ContractName = CommonUtil.TextLineFormat(contRes[0].ContractTargetNameEN, contRes[0].ContractTargetNameLC),
                    SiteCode = contRes[0].SiteCode,
                    //SiteName = String.Format("(1) {0}<br />(2) {1}", contRes[0].SiteNameEN, contRes[0].SiteNameLC),
                    SiteName = CommonUtil.TextLineFormat(contRes[0].SiteNameEN, contRes[0].SiteNameLC),
                    UserCode = contRes[0].UserCode,
                    ServiceType = contRes[0].ServiceTypeCode
                };
            }
            else
            {
                var saleRes = salehandler.GetSaleContractBasicForView(strContractCode);
                if (saleRes.Count > 0)
                {
                    obj = new CTS350_ARData_Contract()
                    {
                        ContractCode = saleRes[0].ContractCode,
                        ShortContractCode = util.ConvertContractCode(saleRes[0].ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                        //ContractName = String.Format("(1) {0}<br />(2) {1}", saleRes[0].PurchaserNameEN, saleRes[0].PurchaserNameLC),
                        ContractName = CommonUtil.TextLineFormat(saleRes[0].PurchaserNameEN, saleRes[0].PurchaserNameLC),
                        SiteCode = saleRes[0].SiteCode,
                        //SiteName = String.Format("(1) {0}<br />(2) {1}", saleRes[0].SiteNameEN, saleRes[0].SiteNameLC),
                        SiteName = CommonUtil.TextLineFormat(saleRes[0].SiteNameEN, saleRes[0].SiteNameLC),
                        UserCode = "",
                        ServiceType = saleRes[0].ServiceTypeCode
                    };
                }
            }

            return obj;
        }

        /// <summary>
        /// Register new AR data
        /// </summary>
        /// <param name="entryDat"></param>
        /// <param name="objRes"></param>
        /// <param name="contractDat"></param>
        /// <returns></returns>
        private CTS350_RegisResult RegisARData_CTS350(CTS350_ARRegisForm entryDat, ref ObjectResultData objRes, CTS350_ARData_Contract contractDat = null)
        {
            CTS350_RegisResult res = null;
            CommonUtil util = new CommonUtil();
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            IARHandler arhandler = ServiceContainer.GetService<IARHandler>() as IARHandler;
            IOfficeMasterHandler officehandler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;

            try
            {
                List<CTS350_PersonInCharge> finalizePIC = null;

                string strARRelevantType = "", strARRelevantCode = "", strARReqNo = "", strAROffice = "", strARStatus = ""
                    , strDepartmentCode = "", strCustCode = null, strSiteCode = null, strQuoteCode = null, strProjectCode = null, strContractCode = null;

                bool? blRelatedToAllContractFlag = null, blRelatedToAllSiteFlag = null;

                var arTypeDat = arhandler.GetTbs_ARTypeTitle(entryDat.ARTypeCode, entryDat.ARTitle);

                CTS350_PersonInCharge[] tmpArr = new CTS350_PersonInCharge[entryDat.PersonInChargeData.Count];
                entryDat.PersonInChargeData.CopyTo(tmpArr);
                finalizePIC = tmpArr.ToList();

                if (contractDat != null)
                {
                    strARRelevantType = ARRelevant.C_AR_RELEVANT_TYPE_CONTRACT;
                    strARRelevantCode = contractDat.ContractCode;
                    strContractCode = contractDat.ContractCode;
                }
                else if (entryDat.ARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_CUSTOMER)
                {
                    strARRelevantType = entryDat.ARRelevantType;
                    strARRelevantCode = util.ConvertCustCode(entryDat.Condition.CustomerCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                    strCustCode = strARRelevantCode;
                }
                else if (entryDat.ARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_SITE)
                {
                    strARRelevantType = entryDat.ARRelevantType;
                    strARRelevantCode = util.ConvertSiteCode(entryDat.Condition.SiteCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                    strSiteCode = strARRelevantCode;
                }
                else if (entryDat.ARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_QUOTATION)
                {
                    strARRelevantType = entryDat.ARRelevantType;
                    strARRelevantCode = util.ConvertQuotationTargetCode(entryDat.Condition.QuotationCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                    strQuoteCode = strARRelevantCode;
                }
                else if (entryDat.ARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_PROJECT)
                {
                    strARRelevantType = entryDat.ARRelevantType;
                    strARRelevantCode = entryDat.Condition.ProjectCode;
                    strProjectCode = strARRelevantCode;
                }

                string[] argenRes = arhandler.GenerateARRequestNo(strARRelevantType, strARRelevantCode);
                if (argenRes != null && argenRes.Length > 0) //Add by Jutarat A. on 05032013
                {
                    strARReqNo = argenRes[0];
                    strAROffice = argenRes[1];
                }

                var auditRole = from a in entryDat.PersonInChargeData where a.ARRoleCode == ARRole.C_AR_ROLE_AUDITOR select a;

                if (auditRole!= null && auditRole.Count() > 0)
                {
                    strARStatus = ARStatus.C_AR_STATUS_AUDITING;
                }
                else
                {
                    strARStatus = ARStatus.C_AR_STATUS_WAIT_FOR_APPROVAL;
                }

                var headofficeRes = officehandler.CheckHeadOffice(strAROffice);

                if (headofficeRes)
                {
                    strDepartmentCode = CommonUtil.dsTransData.dtUserData.MainDepartmentCode;
                }

                if ((entryDat.ARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_CUSTOMER) && (entryDat.CustomerRelateType == "1"))
                {
                    blRelatedToAllSiteFlag = false;
                }
                else if ((entryDat.ARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_CUSTOMER) && (entryDat.CustomerRelateType == "2"))
                {
                    var officeLst = arhandler.GetSiteRelatedOfficeChief(strARReqNo);

                    if ((officeLst != null) && (officeLst.Count > 0))
                    {
                        finalizePIC.Add(new CTS350_PersonInCharge()
                        {
                            EmpNo = officeLst[0].EmpNo,
                            ARRoleCode = ARRole.C_AR_ROLE_CHIEF_OF_RELATED_OFFICE
                        });
                    }

                    blRelatedToAllSiteFlag = true;
                }
                else if ((entryDat.ARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_SITE) && (entryDat.SiteRelateType == "1"))
                {
                    blRelatedToAllContractFlag = false;
                }
                else if ((entryDat.ARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_SITE) && (entryDat.SiteRelateType == "2"))
                {
                    var officeLst = arhandler.GetContractRelatedOfficeChief(strARReqNo);

                    if ((officeLst != null) && (officeLst.Count > 0))
                    {
                        finalizePIC.Add(new CTS350_PersonInCharge()
                        {
                            EmpNo = officeLst[0].EmpNo,
                            ARRoleCode = ARRole.C_AR_ROLE_CHIEF_OF_RELATED_OFFICE
                        });
                    }

                    blRelatedToAllContractFlag = true;
                }

                tbt_AR objAR = new tbt_AR()
                {
                    ARDepartmentCode = strDepartmentCode,
                    AROfficeCode = strAROffice,
                    ARPurpose = entryDat.ARPurpose,
                    ARRelavantType = entryDat.ARRelevantType,
                    ARStatus = strARStatus,
                    ARTitleType = entryDat.ARTitle,
                    ARSubtitle = entryDat.ARSubTitle,
                    ARType = entryDat.ARTypeCode,

                    //DeadLine = (entryDat.DueDate_Deadline_Type == "2") ? entryDat.Deadline_Date : (DateTime?)null,
                    //DeadLineTime = (entryDat.DueDate_Deadline_Type == "2") ? entryDat.Deadline_Until : null,
                    //DueDate = (entryDat.DueDate_Deadline_Type == "1") ? entryDat.DueDate_Date : (DateTime?)null,
                    //DueDateTime = (entryDat.DueDate_Deadline_Type == "1") ? entryDat.DueDate_Time : (TimeSpan?)null,
                    DeadLine = (DateTime?)null,
                    DeadLineTime = null,
                    DueDate = DateTime.Now,
                    DueDateTime = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second),

                    ImportanceFlag = entryDat.IsImportance,
                    RelatedToAllContractFlag = blRelatedToAllContractFlag,
                    RelatedToAllSiteFlag = blRelatedToAllSiteFlag,
                    RequestNo = strARReqNo,
                    CustCode = strCustCode,
                    ContractCode = strContractCode,
                    SiteCode = strSiteCode,
                    QuotationTargetCode = strQuoteCode,
                    ProjectCode = strProjectCode,
                    hasRespondingDetailFlag = FlagType.C_FLAG_OFF
                };

                var insertARRes = arhandler.InsertTbt_AR(objAR);

                List<tbt_ARRole> objARRole = new List<tbt_ARRole>();
                foreach (var item in entryDat.PersonInChargeData)
                {
                    objARRole.Add(new tbt_ARRole()
                    {
                        ARRoleType = item.ARRoleCode,
                        DepartmentCode = item.DepartmentCode,
                        EmpNo = item.EmpNo,
                        OfficeCode = item.OfficeCode,
                        RequestNo = strARReqNo
                    });
                }

                var insertARRoleRes = arhandler.InsertTbt_ARRole(objARRole);

                if (arTypeDat != null && (arTypeDat.Count > 0) && arTypeDat[0].FeeAdjustmentFlag.GetValueOrDefault())
                {
                    tbt_ARFeeAdjustment objARFee = new tbt_ARFeeAdjustment()
                    {
                        //NormalContractFee = entryDat.ContractFee_Quotation,
                        //NormalDepositFee = entryDat.Deposit_Quotation,
                        //NormalInstallFee = entryDat.Installation_Quotation,
                        //NormalSaleInstallationFee = entryDat.InstallFee_Quotation,
                        //NormalSaleProductPrice = entryDat.ProductPrice_Quotation,
                        //OrderContractFee = entryDat.ContractFee_AR,
                        //OrderDepositFee = entryDat.Deposit_AR,
                        //OrderInsatallationFee = entryDat.Installation_AR,
                        //OrderSaleInstallationFee = entryDat.InstallFee_AR,
                        //OrderSaleProductPrice = entryDat.ProductPrice_AR,
                        RequestNo = strARReqNo
                    };

                    #region Normal Contract Fee

                    objARFee.NormalContractFeeCurrencyType = entryDat.ContractFee_QuotationCurrencyType;
                    if (objARFee.NormalContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        objARFee.NormalContractFeeUsd = entryDat.ContractFee_Quotation;
                    else
                        objARFee.NormalContractFee = entryDat.ContractFee_Quotation;

                    #endregion
                    #region Normal Install Fee

                    objARFee.NormalInstallFeeCurrencyType = entryDat.Installation_QuotationCurrencyType;
                    if (objARFee.NormalInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        objARFee.NormalInstallFeeUsd = entryDat.Installation_Quotation;
                    else
                        objARFee.NormalInstallFee = entryDat.Installation_Quotation;

                    #endregion
                    #region Normal Deposit Fee

                    objARFee.NormalDepositFeeCurrencyType = entryDat.Deposit_QuotationCurrencyType;
                    if (objARFee.NormalDepositFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        objARFee.NormalDepositFeeUsd = entryDat.Deposit_Quotation;
                    else
                        objARFee.NormalDepositFee = entryDat.Deposit_Quotation;

                    #endregion
                    #region Normal Sale Installation Fee

                    objARFee.NormalSaleInstallationFeeCurrencyType = entryDat.InstallFee_QuotationCurrencyType;
                    if (objARFee.NormalSaleInstallationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        objARFee.NormalSaleInstallationFeeUsd = entryDat.InstallFee_Quotation;
                    else
                        objARFee.NormalSaleInstallationFee = entryDat.InstallFee_Quotation;

                    #endregion
                    #region Normal Sale Product Price

                    objARFee.NormalSaleProductPriceCurrencyType = entryDat.ProductPrice_QuotationCurrencyType;
                    if (objARFee.NormalSaleProductPriceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        objARFee.NormalSaleProductPriceUsd = entryDat.ProductPrice_Quotation;
                    else
                        objARFee.NormalSaleProductPrice = entryDat.ProductPrice_Quotation;

                    #endregion
                    #region Order Contract Fee

                    objARFee.OrderContractFeeCurrencyType = entryDat.ContractFee_ARCurrencyType;
                    if (objARFee.OrderContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        objARFee.OrderContractFeeUsd = entryDat.ContractFee_AR;
                    else
                        objARFee.OrderContractFee = entryDat.ContractFee_AR;

                    #endregion
                    #region Order Deposit Fee

                    objARFee.OrderDepositFeeCurrencyType = entryDat.Deposit_ARCurrencyType;
                    if (objARFee.OrderDepositFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        objARFee.OrderDepositFeeUsd = entryDat.Deposit_AR;
                    else
                        objARFee.OrderDepositFee = entryDat.Deposit_AR;

                    #endregion
                    #region Order Installation Fee

                    objARFee.OrderInsatallationFeeCurrencyType = entryDat.Installation_ARCurrencyType;
                    if (objARFee.OrderInsatallationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        objARFee.OrderInsatallationFeeUsd = entryDat.Installation_AR;
                    else
                        objARFee.OrderInsatallationFee = entryDat.Installation_AR;

                    #endregion
                    #region Order Sale Installation Fee

                    objARFee.OrderSaleInstallationFeeCurrencyType = entryDat.InstallFee_ARCurrencyType;
                    if (objARFee.OrderSaleInstallationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        objARFee.OrderSaleInstallationFeeUsd = entryDat.InstallFee_AR;
                    else
                        objARFee.OrderSaleInstallationFee = entryDat.InstallFee_AR;

                    #endregion
                    #region Order Sale Product Price

                    objARFee.OrderSaleProductPriceCurrencyType = entryDat.ProductPrice_ARCurrencyType;
                    if (objARFee.OrderSaleProductPriceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        objARFee.OrderSaleProductPriceUsd = entryDat.ProductPrice_AR;
                    else
                        objARFee.OrderSaleProductPrice = entryDat.ProductPrice_AR;

                    #endregion

                    var insertARFeeRes = arhandler.InsertTbt_ARFeeAdjustment(objARFee);
                }

                var attachFileLst = commonhandler.GetAttachFile(GetCurrentKey());
                if (attachFileLst != null && attachFileLst.Count > 0)
                {
                    if (objAR.ARRelavantType == ARRelevant.C_AR_RELEVANT_TYPE_CONTRACT)
                    {
                        commonhandler.CopyAttachFile(AttachmentModule.AR, GetCurrentKey(), strARReqNo, false);
                    }
                    else
                    {
                        commonhandler.UpdateFlagAttachFile(AttachmentModule.AR, GetCurrentKey(), strARReqNo);
                    }
                }

                res = new CTS350_RegisResult();
                res.RequestNo = strARReqNo;
                res.RegisStatus = strARStatus;

                //Modify by Jutarat A. on 07092012
                CTS350_ScreenParameter sParam = GetScreenObject<CTS350_ScreenParameter>();
                if (String.IsNullOrEmpty(sParam.MailTo) == false)
                {
                    var ex = SendMail_CTS350(res, insertARRoleRes, entryDat.PersonInChargeData, sParam.MailTo, objAR);
                    if (ex != null)
                    {
                        objRes.AddErrorMessage(ex);
                    }
                }
                //End Modify
            }
            catch (Exception ex)
            {
                throw ex;
            }



            return res;
        }

        /// <summary>
        /// Proceed send email
        /// </summary>
        /// <param name="res"></param>
        /// <param name="arRole"></param>
        /// <param name="PICLst"></param>
        /// <returns></returns>
        private Exception SendMail_CTS350(CTS350_RegisResult res, List<tbt_ARRole> arRole, List<CTS350_PersonInCharge> PICLst, string strMailTo, tbt_AR ARData) //Add strMailTo,ARData by Jutarat A. on 07092012
        {
            Exception resEx = null;

            try
            {
                IARHandler arhandler = ServiceContainer.GetService<IARHandler>() as IARHandler;
                IEmployeeMasterHandler emphandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                var markLst = from a in PICLst where a.SendMail select a.EmpNo;
                var sendLst = from a in arRole where markLst.Contains(a.EmpNo) select a;
                string mailTolst = "";

                //Modify by Jutarat A. on 07092012
                if (String.IsNullOrEmpty(strMailTo))
                {
                    List<dtEmployee> lstEmployee = emphandler.GetEmployee(null, null, null);
                    foreach (var item in sendLst.ToList())
                    {
                        //var empRes = emphandler.GetEmployee(item.EmpNo, null, null);
                        List<dtEmployee> empRes = (from t in lstEmployee where t.EmpNo == item.EmpNo select t).ToList<dtEmployee>();
                        if (empRes != null)
                        {
                            if (mailTolst.Length > 0)
                            {
                                mailTolst += "; ";
                            }
                            mailTolst += empRes[0].EmailAddress;
                        }
                    }
                }
                else
                {
                    mailTolst = strMailTo;
                }
                //End Modify

                if (sendLst.Count() > 0)
                {
                    var arLinkEN = CommonUtil.GenerateCompleteURL("Contract", "CTS380", String.Format("pRequestNo={0}", res.RequestNo), CommonValue.DEFAULT_SHORT_LANGUAGE_EN);
                    var arLinkLC = CommonUtil.GenerateCompleteURL("Contract", "CTS380", String.Format("pRequestNo={0}", res.RequestNo), CommonValue.DEFAULT_SHORT_LANGUAGE_LC); //Add by Jutarat A. on 28092012

                    arhandler.SendAREmail(mailTolst, res.RegisStatus, arLinkEN, arLinkLC, ARData); //Add arLinkLC,ARData by Jutarat A. on 28092012
                }
            }
            catch (Exception ex)
            {
                resEx = ex;
            }

            return resEx;
        }

        /// <summary>
        /// Validate system suspend and user permission
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        private ObjectResultData ValidateAuthority_CTS350(ObjectResultData res)
        {
            //Check System Suspend
            if (CheckIsSuspending(res))
            {
                res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0049, null, null);
                return res;
            }

            //Check Screen Permission
            if (!CheckUserPermission(ScreenID.C_SCREEN_ID_REGISTER_AR))
            {
                res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0053, null, null);
                return res;
            }

            return res;
        }

        /// <summary>
        /// Validate parameter from another screen
        /// </summary>
        /// <param name="param"></param>
        /// <param name="res"></param>
        /// <returns></returns>
        private ObjectResultData ValidateParameter_CTS350(CTS350_ScreenParameter param, ObjectResultData res)
        {
            CommonUtil util = new CommonUtil();
            ICustomerMasterHandler customerhandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
            ISiteMasterHandler sitehandler = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;
            IRentralContractHandler rentalhandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            ISaleContractHandler salehandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
            IProjectHandler projecthandler = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
            IQuotationHandler quotationhandler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;

            if (!String.IsNullOrEmpty(param.strARRelevantCode) && !string.IsNullOrEmpty(param.strARRelevantType))
            {
                if (param.strARRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CUSTOMER)
                {
                    var custDat = customerhandler.GetCustomer(util.ConvertCustCode(param.strARRelevantCode, CommonUtil.CONVERT_TYPE.TO_LONG));

                    if ((custDat == null) || (custDat.Count != 1))
                    {
                        res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0011, new string[] { param.strARRelevantCode }, null);
                    }
                }
                else if (param.strARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_SITE)
                {
                    var siteDat = sitehandler.GetSite(util.ConvertSiteCode(param.strARRelevantCode, CommonUtil.CONVERT_TYPE.TO_LONG), null);

                    if ((siteDat == null) || (siteDat.Count != 1))
                    {
                        res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0011, new string[] { param.strARRelevantCode }, null);
                    }
                }
                else if (param.strARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_CONTRACT)
                {
                    var contractData = RetrieveContractInfo_CTS350(util.ConvertContractCode(param.strARRelevantCode, CommonUtil.CONVERT_TYPE.TO_LONG)
                        , util.ConvertContractCode(param.strARRelevantCode, CommonUtil.CONVERT_TYPE.TO_LONG));

                    if (contractData == null)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { param.strARRelevantCode });
                    }
                    
                    //var rentalContractDat = rentalhandler.GetTbt_RentalContractBasic(util.ConvertContractCode(param.strARRelevantCode, CommonUtil.CONVERT_TYPE.TO_LONG), null);
                    //var saleContractDat = salehandler.GetTbt_SaleBasic(util.ConvertContractCode(param.strARRelevantCode, CommonUtil.CONVERT_TYPE.TO_LONG), null, null);

                    //if ((rentalContractDat == null) || (rentalContractDat.Count != 1))
                    //{
                    //    res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0011, new string[] { param.strARRelevantCode }, null);
                    //}
                    //else if ((saleContractDat == null) || (saleContractDat.Count != 1))
                    //{
                    //    res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0011, new string[] { param.strARRelevantCode }, null);
                    //}
                }
                else if (param.strARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_PROJECT)
                {
                    var projectDat = projecthandler.GetTbt_Project(param.strARRelevantCode);

                    if ((projectDat == null) || (projectDat.Count != 1))
                    {
                        res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0011, new string[] { param.strARRelevantCode }, null);
                    }
                }
                else if (param.strARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_QUOTATION)
                {
                    var quotationDat = RetrieveQuotationTargetInfo_CTS350(param.strARRelevantCode);
                    if (quotationDat == null)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { param.strARRelevantCode }, null);
                    }
                    else if (!String.IsNullOrEmpty(quotationDat.ContractCode))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3074, new string[] { param.strARRelevantCode }, null);
                    }
                }
            }

            return res;
        }

        #endregion
    }
}
