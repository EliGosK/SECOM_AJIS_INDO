using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Xml;
using System.Linq;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.DataEntity.Master;

namespace SECOM_AJIS.Presentation.Contract.Helpers
{
    public static partial class ComboBoxHelper
    {
        //Add by Nutnicha C. 21/07/2011
        /// <summary>
        /// Generate approval status combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString ApprovalStatusComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            #region GetMisc
            //List<View_dtMiscTypeCode> lst = new List<View_dtMiscTypeCode>();
            //try
            //{
            //    List<dtMiscTypeCode> miscs = new List<dtMiscTypeCode>()
            //    {
            //        new dtMiscTypeCode()
            //        {
            //            FieldName = MiscType.C_APPROVE_STATUS,
            //            ValueCode = "%"
            //        }
            //    };

            //    ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            //    lst = hand.GetMiscTypeCodeListByLanguage(miscs);
            //}
            //catch
            //{
            //}

            //if (lst == null)
            //    lst = new List<View_dtMiscTypeCode>();
            #endregion

            ICommonHandler handlerComm = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<string> lsFieldNames = new List<string>();
            lsFieldNames.Add(MiscType.C_APPROVE_STATUS);
            List<doMiscTypeCode> MiscTypeList = handlerComm.GetMiscTypeCodeListByFieldName(lsFieldNames);
            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, MiscTypeList, "ValueCodeDisplay", "ValueCode", attribute);
        }

        //Add by Nutnicha C. 01/09/2011
        /// <summary>
        /// Generate subcontractor combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString SubContractorComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            ICommonContractHandler hand = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
            List<tbm_SubContractor> lst = hand.GetTbm_SubContractorData(null);
            MvcHtmlString mvc = CommonUtil.CommonComboBox<tbm_SubContractor>(id, lst, "SubContractorCodeName", "SubContractorCode", attribute);
            return mvc;
        }

        /// <summary>
        /// Generate subcontractor combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="firstElement"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString SubContractorComboBoxWithFirstElement(this HtmlHelper helper, string id, string firstElement, object attribute = null)
        {
            ICommonContractHandler hand = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
            List<tbm_SubContractor> lst = hand.GetTbm_SubContractorData(null);
            //MvcHtmlString mvc = CommonUtil.CommonComboBox<tbm_SubContractor>(id, lst, "SubContractorCodeName", "SubContractorCode", attribute);
            return CommonUtil.CommonComboBoxWithCustomFirstElement<tbm_SubContractor>(id, lst, "SubContractorCodeName", "SubContractorCode", firstElement, attribute);
        }

        //Add by Nutnicha C. 07/09/2011
        /// <summary>
        /// Generate related contract type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString RelatedContractTypeComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            #region Get Current Language
            string lang = CommonUtil.GetCurrentLanguage();
            if (lang == SECOM_AJIS.Common.Util.ConstantValue.CommonValue.DEFAULT_LANGUAGE_EN)
                lang = string.Empty;
            else
                lang = "." + lang;

            string resourcePath = string.Format("{0}{1}\\{2}\\{3}{4}.resx",
                                                CommonUtil.WebPath,
                                                SECOM_AJIS.Common.Util.ConstantValue.CommonValue.APP_GLOBAL_RESOURCE_FOLDER,
                                                "Contract",
                                                "CTS270",
                                                lang);
            XmlDocument rDoc = new XmlDocument();
            rDoc.Load(resourcePath);
            #endregion
            #region Set display value
            XmlNode rNodeAny = rDoc.SelectSingleNode(string.Format("root/data[@name=\"{0}\"]/value", "C_RELATED_CONTRACT_TYPE_ANY"));
            String strAnyDisplay = rNodeAny.InnerText ?? "";

            XmlNode rNodeAlarmPeriod = rDoc.SelectSingleNode(string.Format("root/data[@name=\"{0}\"]/value", "C_RELATED_CONTRACT_TYPE_ALARM_PERIOD"));
            String strAlarmPeriodDisplay = rNodeAlarmPeriod.InnerText ?? "";

            XmlNode rNodeSale = rDoc.SelectSingleNode(string.Format("root/data[@name=\"{0}\"]/value", "C_RELATED_CONTRACT_TYPE_SALE"));
            String strSaleDisplay = rNodeSale.InnerText ?? "";

            XmlNode rNodeSeparatedMA = rDoc.SelectSingleNode(string.Format("root/data[@name=\"{0}\"]/value", "C_RELATED_CONTRACT_TYPE_SEPARATED_MA"));
            String strSeparatedMADisplay = rNodeSeparatedMA.InnerText ?? "";
            #endregion
            #region Set combo item
            List<ItemValue<String>> ls = new List<ItemValue<String>>();
            ItemValue<String> item;

            item = new ItemValue<String>();
            item.Value = RelatedContractType.C_RELATED_CONTRACT_TYPE_ANY;
            item.Display = strAnyDisplay;
            ls.Add(item);

            item = new ItemValue<String>();
            item.Value = RelatedContractType.C_RELATED_CONTRACT_TYPE_ALARM_PERIOD;
            item.Display = strAlarmPeriodDisplay;
            ls.Add(item);

            item = new ItemValue<String>();
            item.Value = RelatedContractType.C_RELATED_CONTRACT_TYPE_SALE;
            item.Display = strSaleDisplay;
            ls.Add(item);

            item = new ItemValue<String>();
            item.Value = RelatedContractType.C_RELATED_CONTRACT_TYPE_SEPARATED_MA;
            item.Display = strSeparatedMADisplay;
            ls.Add(item);
            #endregion
            MvcHtmlString mvc = CommonUtil.CommonComboBox<ItemValue<String>>(id, ls, "Display", "Value", attribute, false);
            return mvc;
        }

        //Add by Songwut C. 01/09/2011
        /// <summary>
        /// Generate acquisition combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString AcquisitionComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            ICommonHandler handlerComm = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<string> lsFieldNames = new List<string>();
            lsFieldNames.Add(MiscType.C_ACQUISITION_TYPE);
            List<doMiscTypeCode> MiscTypeList = handlerComm.GetMiscTypeCodeListByFieldName(lsFieldNames);
            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, MiscTypeList, "ValueCodeDisplay", "ValueCode", attribute);
        }

        /// <summary>
        /// Generate project branch No. combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="ProjectCode"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString ProjectBranchNoComboBox(this HtmlHelper helper, string id, string ProjectCode, object attribute = null)
        {
            IProjectHandler projH = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
            List<doProjectBranch> lstBranch;
            lstBranch = projH.GetProjectStockOutBranch(ProjectCode);
            MvcHtmlString aa = CommonUtil.CommonComboBox<doProjectBranch>(id, lstBranch, "BranchNo", "ProjectCodeBranch", attribute, true);
            return aa;
        }

        // Add by Natthavat S. 26/10/2011
        /// <summary>
        /// Generate install process manage status combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString InstallProcessMgmtStatusComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            ICommonHandler handlerComm = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<string> lsFieldNames = new List<string>();
            lsFieldNames.Add(MiscType.C_SALE_PROC_MANAGE_STATUS);
            List<doMiscTypeCode> MiscTypeList = handlerComm.GetMiscTypeCodeListByFieldName(lsFieldNames);

            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, MiscTypeList, "ValueCodeDisplay", "ValueCode", attribute);
        }

        /// <summary>
        /// Generate install new & old building combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString InstallNewOldBuildingComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            ICommonHandler handlerComm = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<string> lsFieldNames = new List<string>();
            lsFieldNames.Add(MiscType.C_BUILDING_TYPE);
            List<doMiscTypeCode> MiscTypeList = handlerComm.GetMiscTypeCodeListByFieldName(lsFieldNames);

            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, MiscTypeList, "ValueCodeDisplay", "ValueCode", attribute);
        }

        ///// <summary>
        ///// Generate sale type combobox
        ///// </summary>
        ///// <param name="helper"></param>
        ///// <param name="id"></param>
        ///// <param name="attribute"></param>
        ///// <returns></returns>
        //public static MvcHtmlString SaleTypeComboBox(this HtmlHelper helper, string id, object attribute = null)
        //{
        //    ICommonHandler handlerComm = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
        //    List<string> lsFieldNames = new List<string>();
        //    lsFieldNames.Add(MiscType.C_SALE_TYPE);
        //    List<doMiscTypeCode> MiscTypeList = handlerComm.GetMiscTypeCodeListByFieldName(lsFieldNames);

        //    return CommonUtil.CommonComboBox<doMiscTypeCode>(id, MiscTypeList, "ValueCodeDisplay", "ValueCode", attribute);
        //}

        /// <summary>
        /// Generate doc audit result combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString DocAuditResultComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            ICommonHandler handlerComm = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<string> lsFieldNames = new List<string>();
            lsFieldNames.Add(MiscType.C_DOC_AUDIT_RESULT);
            List<doMiscTypeCode> MiscTypeList = handlerComm.GetMiscTypeCodeListByFieldName(lsFieldNames);

            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, MiscTypeList, "ValueCodeDisplay", "ValueCode", attribute);
        }

        /// <summary>
        /// Generate distribute type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString DistributeTypeComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            ICommonHandler handlerComm = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<string> lsFieldNames = new List<string>();
            lsFieldNames.Add(MiscType.C_DISTRIBUTED_TYPE);
            List<doMiscTypeCode> MiscTypeList = handlerComm.GetMiscTypeCodeListByFieldName(lsFieldNames);

            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, MiscTypeList, "ValueCodeDisplay", "ValueCode", attribute);
        }

        /// <summary>
        /// Generate purchase reason type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString PurchaseReasonTypeComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            ICommonHandler handlerComm = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<string> lsFieldNames = new List<string>();
            lsFieldNames.Add(MiscType.C_MOTIVATION_TYPE);
            List<doMiscTypeCode> MiscTypeList = handlerComm.GetMiscTypeCodeListByFieldName(lsFieldNames);

            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, MiscTypeList, "ValueCodeDisplay", "ValueCode", attribute);
        }

        /// <summary>
        /// Generate acquisition type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString AcquisitionTypeComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            ICommonHandler handlerComm = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<string> lsFieldNames = new List<string>();
            lsFieldNames.Add(MiscType.C_ACQUISITION_TYPE);
            List<doMiscTypeCode> MiscTypeList = handlerComm.GetMiscTypeCodeListByFieldName(lsFieldNames);

            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, MiscTypeList, "ValueCodeDisplay", "ValueCode", attribute);
        }

        // Add by Natthavat S. 03/11/2011
        /// <summary>
        /// Generate contract office combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString ContractOfficeCombo(this HtmlHelper helper, string id, object attribute = null)
        {
            IOfficeMasterHandler officehandler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
            var allOffice = officehandler.GetFunctionSale(FunctionSale.C_FUNC_SALE_NO);
            var rawResult = from a in allOffice
                            select new OfficeDataDo
                {
                    OfficeCode = a.OfficeCode,
                    OfficeName = String.Empty,
                    OfficeNameEN = a.OfficeNameEN,
                    OfficeNameJP = a.OfficeNameJP,
                    OfficeNameLC = a.OfficeNameLC
                };

            var result = CommonUtil.ConvertObjectbyLanguage<OfficeDataDo, OfficeDataDo>(rawResult.ToList(), "OfficeName");

            return CommonUtil.CommonComboBox<OfficeDataDo>(id, result.ToList(), "OfficeCodeName", "OfficeCode", attribute);
        }

        /// <summary>
        /// Generate operation office combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString OperationOfficeCombo_CTS150(this HtmlHelper helper, string id, object attribute = null)
        {
            IOfficeMasterHandler officehandler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
            var allOffice = officehandler.GetFunctionSecurity(FunctionSecurity.C_FUNC_SECURITY_NO);

            var rawResult = from a in allOffice
                            select new OfficeDataDo
                            {
                                OfficeCode = a.OfficeCode,
                                OfficeName = String.Empty,
                                OfficeNameEN = a.OfficeNameEN,
                                OfficeNameJP = a.OfficeNameJP,
                                OfficeNameLC = a.OfficeNameLC
                            };

            var result = CommonUtil.ConvertObjectbyLanguage<OfficeDataDo, OfficeDataDo>(rawResult.ToList(), "OfficeName");

            return CommonUtil.CommonComboBox<OfficeDataDo>(id, result.ToList(), "OfficeCodeName", "OfficeCode", attribute);
        }

        /// <summary>
        /// Generate reason combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString ReasonComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            IIncidentHandler handler = ServiceContainer.GetService<IIncidentHandler>() as IIncidentHandler;

            var lst = handler.GetTbs_IncidentReasonType(null);
            CommonUtil.MappingObjectLanguage<tbs_IncidentReasonType>(lst);

            return CommonUtil.CommonComboBox<tbs_IncidentReasonType>(id, lst
                , "ReasonTypeName"
                , "ReasonType", attribute);
        }

        /// <summary>
        /// Generate belonging office combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="empNo"></param>
        /// <returns></returns>
        public static MvcHtmlString BelongingOfficeComboBox(this HtmlHelper helper, string id, object attribute = null, string empNo = null)
        {
            //IEmployeeMasterHandler emphandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;

            //var officeLst = emphandler.GetBelongingOfficeList(null);
            //CommonUtil.MappingObjectLanguage<dtBelongingOffice>(officeLst);

            //return CommonUtil.CommonComboBox<dtBelongingOffice>(id, officeLst
            //    , "OfficeCodeName"
            //    , "OfficeCode", attribute, true);

            List<dtBelongingOffice> lst = new List<dtBelongingOffice>();
            List<dtBelongingOffice> vw_lst = new List<dtBelongingOffice>();
            try
            {
                IEmployeeMasterHandler handle = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                lst = handle.GetBelongingOfficeList(empNo);

                // Select language
                vw_lst = CommonUtil.ConvertObjectbyLanguage<dtBelongingOffice, dtBelongingOffice>(lst, "OfficeCodeName");

                vw_lst = (from p in vw_lst orderby p.OfficeCodeName ascending select p).ToList<dtBelongingOffice>();
            }
            catch
            {
                lst = new List<dtBelongingOffice>();
            }

            return CommonUtil.CommonComboBox<dtBelongingOffice>(id, vw_lst, "OfficeCodeName", "OfficeCode", attribute);
        }

        /// <summary>
        /// Generate AR status combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString ARStatusComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MvcHtmlString.Create(MiscTypeComboBoxWithFirstElement(helper, id, MiscType.C_AR_STATUS, "ALL", attribute, "ValueDisplay", true).ToString().Replace("<option value=\"1\">", "<option value=\"1\" selected=\"true\">"));
        }

        /// <summary>
        /// Generate payment method contract combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString PaymentMethodContractComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MvcHtmlString.Create(MiscTypeComboBoxWithFirstElement(helper, id, MiscType.C_PAYMENT_METHOD, null, attribute, "ValueCodeDisplay", true).ToString());
        }

        /// <summary>
        /// Generate misc type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="fieldName"></param>
        /// <param name="firstElement"></param>
        /// <param name="attribute"></param>
        /// <param name="display_field"></param>
        /// <param name="include_idx0"></param>
        /// <returns></returns>
        private static MvcHtmlString MiscTypeComboBoxWithFirstElement(this HtmlHelper helper, string id, string fieldName, string firstElement = null, object attribute = null, string display_field = null, bool include_idx0 = true)
        {
            List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
            try
            {
                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                {
                    new doMiscTypeCode()
                    {
                        FieldName = fieldName,
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
            if (display_field != null)
                display = display_field;

            return CommonUtil.CommonComboBoxWithCustomFirstElement<doMiscTypeCode>(id, lst, display, "ValueCode", firstElement, attribute, include_idx0);
        }


    }
}
