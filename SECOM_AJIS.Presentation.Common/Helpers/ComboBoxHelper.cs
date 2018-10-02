using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.WebPages;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Master;

using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.Presentation.Common.Helpers
{
    public static partial class ComboBoxHelper
    {
        /// <summary>
        /// Generate product type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString ProductTypeComboBox(this HtmlHelper helper, string id, object attribute = null, string firstElement = null)
        {
            List<tbs_ProductType> lstProductType = new List<tbs_ProductType>();
            try
            {
                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                lstProductType = hand.GetTbs_ProductType(null, null);
            }
            catch
            {
                lstProductType = new List<tbs_ProductType>();
            }
            return CommonUtil.CommonComboBoxWithCustomFirstElement<tbs_ProductType>(id, lstProductType, "ProductTypeCodeName", "ProductTypeCode", firstElement, attribute);

        }
        /// <summary>
        /// Generate system product combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString SystemProduct(this HtmlHelper helper, string id, object attribute = null, string firstElement = null)
        {
            string Display = "ValueDisplay";
            IMasterHandler hand = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
            List<tbm_Product> lst = hand.GetTbm_Product(null, null);
            List<View_tbm_Product> lst2 = new List<View_tbm_Product>();

            foreach (tbm_Product i in lst)
            {
                lst2.Add(CommonUtil.CloneObject<tbm_Product, View_tbm_Product>(i));
            }

            CommonUtil.MappingObjectLanguage<View_tbm_Product>(lst2);

            foreach (View_tbm_Product i2 in lst2)
            {
                i2.ValueDisplay = CommonUtil.TextCodeName(i2.ProductCode, i2.ProductName);//i2.ProductCode + " : " + i2.ProductName;

            }
            return CommonUtil.CommonComboBoxWithCustomFirstElement<View_tbm_Product>(id, lst2, Display, "ProductCode", firstElement, attribute);
        }
        /// <summary>
        /// Generate document type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString DocumentTypeCombo(this HtmlHelper helper, string id, object attribute = null, string firstElement = null)
        {

            List<dtDocumentType> list = new List<dtDocumentType>();


            List<string> lsObjectID = new List<string>();

            if (CommonUtil.dsTransData != null)
            {
                foreach (var item in CommonUtil.dsTransData.dtUserPermissionData.Values)
                {
                    lsObjectID.Add(item.ObjectID);
                }
            }


            string strObjectIDList = CommonUtil.CreateCSVString(lsObjectID);



            try
            {
                IDocumentHandler handler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

                list = handler.GetDocumentTypeDataList(MiscType.C_DOCUMENT_TYPE, strObjectIDList).ToList();

                // Select language

                list = CommonUtil.ConvertObjectbyLanguage<dtDocumentType, dtDocumentType>(list, "DocumentTypeCodeName");


            }
            catch (Exception)
            {
                list = new List<dtDocumentType>();
            }

            // Edit by Narupon W. 9 May 2012
            //return CommonUtil.CommonComboBox<dtDocumentType>(id, list, "DocumentTypeCodeName", "DocumentType", attribute);

            // return CommonUtil.CommonComboBoxWithCustomFirstElement<tbm_DocumentTemplate>(id, lst, strDisplayName, "DocumentCode", firstElement, attribute);

            return CommonUtil.CommonComboBoxWithCustomFirstElement<dtDocumentType>(id, list, "DocumentTypeCodeName", "DocumentType", firstElement, attribute);

        }
        /// <summary>
        /// Generate document name combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="documentType"></param>
        /// <param name="attribute"></param>
        /// <param name="bReportFlag"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString DocumentNameComboBox(this HtmlHelper helper, string id, string documentType, object attribute = null, bool? bReportFlag = null, string firstElement = null)
        {
            List<tbm_DocumentTemplate> lst = new List<tbm_DocumentTemplate>();
            string strDisplayName = "DocumentNameEN";
            try
            {
                IDocumentHandler handler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
                lst = handler.GetTbm_DocumentTemplate(documentType, bReportFlag);

                // 1. Connect string DocumentCode:DocumentName
                // 2. Select language

                //foreach (var item in lst)
                //{
                //    item.DocumentNameEN = CommonUtil.TextCodeName(item.DocumentCode, item.DocumentNameEN);
                //    item.DocumentNameJP = CommonUtil.TextCodeName(item.DocumentCode, item.DocumentNameJP);
                //    item.DocumentNameLC = CommonUtil.TextCodeName(item.DocumentCode, item.DocumentNameLC);

                //}

                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    strDisplayName = "DocumentNameEN";
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    strDisplayName = "DocumentNameJP";
                }
                else
                {
                    strDisplayName = "DocumentNameLC";
                }

            }
            catch
            {
                lst = new List<tbm_DocumentTemplate>();
            }

            //return CommonUtil.CommonComboBox<tbm_DocumentTemplate>(id, lst, strDisplayName, "DocumentCode", attribute);
            return CommonUtil.CommonComboBoxWithCustomFirstElement<tbm_DocumentTemplate>(id, lst, strDisplayName, "DocumentCode", firstElement, attribute);
        }

        /// <summary>
        /// Generate document name combobox for Maintain Contract
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="documentType"></param>
        /// <param name="attribute"></param>
        /// <param name="bReportFlag"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString DocumentNameMaintainContractComboBox(this HtmlHelper helper, string id, string documentType, object attribute = null, bool? bReportFlag = null, string firstElement = null)
        {
            IDocumentHandler handler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
            List<tbm_DocumentTemplate> lst = handler.GetTbm_DocumentTemplate(documentType, bReportFlag);

            if (lst != null && lst.Count > 0)
            {
                lst = (from t in lst
                       where t.DocumentCode != DocumentCode.C_DOCUMENT_CODE_COVER_LETTER
                       select t).ToList<tbm_DocumentTemplate>();

                foreach (var item in lst)
                {
                    item.DocumentNameEN = CommonUtil.TextCodeName(item.DocumentCode, item.DocumentNameEN);
                    item.DocumentNameJP = CommonUtil.TextCodeName(item.DocumentCode, item.DocumentNameJP);
                    item.DocumentNameLC = CommonUtil.TextCodeName(item.DocumentCode, item.DocumentNameLC);
                }
            }

            string strDisplayName = "DocumentNameEN";
            if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
            {
                strDisplayName = "DocumentNameEN";
            }
            else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
            {
                strDisplayName = "DocumentNameJP";
            }
            else
            {
                strDisplayName = "DocumentNameLC";
            }

            return CommonUtil.CommonComboBoxWithCustomFirstElement<tbm_DocumentTemplate>(id, lst, strDisplayName, "DocumentCode", firstElement, attribute);
        }

        /// <summary>
        /// Generate office combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString AllOfficeCombo(this HtmlHelper helper, string id, object attribute = null)
        {

            string strDisplayMember = "OfficeCodeName";
            string strValueMember = "OfficeCode";
            List<OfficeDataDo> nLst = new List<OfficeDataDo>();
            if (CommonUtil.dsTransData != null)
            {
                List<OfficeDataDo> list = CommonUtil.dsTransData.dtOfficeData;
                nLst = CommonUtil.ClonsObjectList<OfficeDataDo, OfficeDataDo>(list);
                CommonUtil.MappingObjectLanguage<OfficeDataDo>(nLst);
            }

            return CommonUtil.CommonComboBox<OfficeDataDo>(id, nLst, strDisplayMember, strValueMember, attribute);
        }
        /// <summary>
        /// Contract Office : select dsTransData.OfficeData[]  where FunctionSale = C_FUNC_SALE_YES
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns>CboContraceOffice</returns>
        public static MvcHtmlString ContractOfficeCombo(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true)
        {
            // select dsTransData.OfficeData[]  where FunctionSale = C_FUNC_SALE_YES  
            // and put to CboContractOffice

            List<OfficeDataDo> vw_list = new List<OfficeDataDo>();

            try
            {
                List<OfficeDataDo> list = new List<OfficeDataDo>();
                if (CommonUtil.dsTransData != null)
                {
                    // use linq : select dsTransData.OfficeData[]  where FunctionSale = C_FUNC_SALE_YES 
                    list = (from t in CommonUtil.dsTransData.dtOfficeData
                            where t.FunctionSale == FunctionSale.C_FUNC_SALE_YES
                            select t).ToList<OfficeDataDo>();
                }
                vw_list = CommonUtil.ClonsObjectList<OfficeDataDo, OfficeDataDo>(list);
                CommonUtil.MappingObjectLanguage<OfficeDataDo>(vw_list);
            }
            catch
            {
            }

            string strDisplayMember = "OfficeCodeName";
            string strValueMember = "OfficeCode";

            return CommonUtil.CommonComboBox<OfficeDataDo>(id, vw_list, strDisplayMember, strValueMember, attribute, include_idx0);
        }
        /// <summary>
        /// Contrace Ofice : select dsTransData.OfficeData[]  where FunctionSale = C_FUNC_SALE_YES
        /// With specified first element
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns>CboContraceOffice</returns>
        public static MvcHtmlString ContractOfficeComboWithFirstElement(this HtmlHelper helper, string id, string firstElement, object attribute = null)
        {
            // select dsTransData.OfficeData[]  where FunctionSale = C_FUNC_SALE_YES  
            // and put to CboContractOffice

            List<OfficeDataDo> list = new List<OfficeDataDo>();
            List<OfficeDataDo> vw_list = new List<OfficeDataDo>();

            if (CommonUtil.dsTransData != null)
            {
                // use linq : select dsTransData.OfficeData[]  where FunctionSale = C_FUNC_SALE_YES 
                list = (from t in CommonUtil.dsTransData.dtOfficeData
                        where t.FunctionSale == FunctionSale.C_FUNC_SALE_YES
                        select t).ToList<OfficeDataDo>();
            }


            vw_list = CommonUtil.ClonsObjectList<OfficeDataDo, OfficeDataDo>(list);
            CommonUtil.MappingObjectLanguage<OfficeDataDo>(vw_list);

            string strDisplayMember = "OfficeCodeName";
            string strValueMember = "OfficeCode";

            //return CommonUtil.CommonComboBox<OfficeDataDo>(id, vw_list, strDisplayMember, strValueMember, attribute, include_idx0);
            return CommonUtil.CommonComboBoxWithCustomFirstElement<OfficeDataDo>(id, vw_list, strDisplayMember, strValueMember, firstElement, attribute);
        }
        /// <summary>
        /// Billing Office : select dsTransData.OfficeData[]  where FunctionBilling = C_FUNC_BILLING_YES
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString BillingOfficeCombo(this HtmlHelper helper, string id, object attribute = null)
        {

            // select dsTransData.OfficeData[]  where FunctionBilling = C_FUNC_BILLING_YES  
            // and put to CboBillingOffice

            List<OfficeDataDo> vw_list = new List<OfficeDataDo>();
            List<OfficeDataDo> list = new List<OfficeDataDo>();

            if (CommonUtil.dsTransData != null)
            {
                // use linq : select dsTransData.OfficeData[]  where FunctionBilling = C_FUNC_BILLING_YES
                list = (from t in CommonUtil.dsTransData.dtOfficeData
                        where t.FunctionBilling == FunctionBilling.C_FUNC_BILLING_YES
                        select t).ToList<OfficeDataDo>();
            }


            vw_list = CommonUtil.ClonsObjectList<OfficeDataDo, OfficeDataDo>(list);
            CommonUtil.MappingObjectLanguage<OfficeDataDo>(vw_list);

            string strDisplayMember = "OfficeCodeName";
            string strValueMember = "OfficeCode";

            return CommonUtil.CommonComboBox<OfficeDataDo>(id, vw_list, strDisplayMember, strValueMember, attribute);
        }
        /// <summary>
        /// Issued Office : select dsTransData.OfficeData[]  where FunctionSecurity = C_FUNC_SECURITY_ALL
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString OperationOfficeCombo(this HtmlHelper helper, string id, object attribute = null)
        {

            // select dsTransData.OfficeData[]  where FunctionSecurity = C_FUNC_SECURITY_ALL
            // and put to CboOperationOffice

            List<OfficeDataDo> list = new List<OfficeDataDo>();
            List<OfficeDataDo> vw_list = new List<OfficeDataDo>();

            if (CommonUtil.dsTransData != null)
            {
                // use linq : select dsTransData.OfficeData[]  where FunctionSecurity = C_FUNC_SECURITY_ALL
                list = (from t in CommonUtil.dsTransData.dtOfficeData
                        where t.FunctionSecurity != FunctionSecurity.C_FUNC_SECURITY_NO
                        select t).ToList<OfficeDataDo>();
            }


            vw_list = CommonUtil.ClonsObjectList<OfficeDataDo, OfficeDataDo>(list);
            CommonUtil.MappingObjectLanguage<OfficeDataDo>(vw_list);

            string strDisplayMember = "OfficeCodeName";
            string strValueMember = "OfficeCode";


            return CommonUtil.CommonComboBox<OfficeDataDo>(id, vw_list, strDisplayMember, strValueMember, attribute);
        }
        /// <summary>
        /// Generate line-up type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_1index"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString LineUpTypeComboBox(this HtmlHelper helper, string id, object attribute = null, bool include_1index = true, string firstElement = null)
        {
            string strDisplay = "ValueDisplay";
            //IOfficeMasterHandler hand = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;

            ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<doMiscTypeCode> lstMiscTypeCode = new List<doMiscTypeCode>();
            doMiscTypeCode MiscTypeCode = new doMiscTypeCode();
            MiscTypeCode.FieldName = MiscType.C_LINE_UP_TYPE;
            // For FIX EXCEPTION while testing "%"
            MiscTypeCode.ValueCode = "%";
            lstMiscTypeCode.Add(MiscTypeCode);
            List<doMiscTypeCode> MiscLock = hand.GetMiscTypeCodeList(lstMiscTypeCode);

            foreach (doMiscTypeCode i in MiscLock)
                i.ValueDisplay = CommonUtil.TextCodeName(i.ValueCode, i.ValueDisplay);
            return CommonUtil.CommonComboBoxWithCustomFirstElement<doMiscTypeCode>(id, MiscLock, strDisplay, "ValueCode", firstElement, attribute, include_1index);
        }
        /// <summary>
        /// Generate instrument type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_1idx"></param>
        /// <returns></returns>
        public static MvcHtmlString InstrumentTypeComboBox(this HtmlHelper helper, string id, object attribute = null, bool include_1idx = true)
        {
            string strDisplay = "ValueDisplay";
            //IOfficeMasterHandler hand = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;

            ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<doMiscTypeCode> lstMiscTypeCode = new List<doMiscTypeCode>();
            doMiscTypeCode MiscTypeCode = new doMiscTypeCode();
            MiscTypeCode.FieldName = MiscType.C_INSTRUMENT_TYPE;
            // For FIX EXCEPTION while testing
            MiscTypeCode.ValueCode = "%";
            lstMiscTypeCode.Add(MiscTypeCode);
            List<doMiscTypeCode> MiscLock = hand.GetMiscTypeCodeList(lstMiscTypeCode);

            foreach (doMiscTypeCode i in MiscLock)
                i.ValueDisplay = CommonUtil.TextCodeName(i.ValueCode, i.ValueDisplay);
            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, MiscLock, strDisplay, "ValueCode", attribute, include_1idx);
        }
        /// <summary>
        /// Generate expansion type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_1idx"></param>
        /// <returns></returns>
        public static MvcHtmlString ExpansionTypeComboBox(this HtmlHelper helper, string id, object attribute = null, bool include_1idx = true)
        {
            string strDisplay = "ValueDisplay";
            //IOfficeMasterHandler hand = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;

            ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<doMiscTypeCode> lstMiscTypeCode = new List<doMiscTypeCode>();
            doMiscTypeCode MiscTypeCode = new doMiscTypeCode();
            MiscTypeCode.FieldName = MiscType.C_EXPANSION_TYPE;
            // For FIX EXCEPTION while testing
            MiscTypeCode.ValueCode = "%";
            lstMiscTypeCode.Add(MiscTypeCode);
            List<doMiscTypeCode> MiscLock = hand.GetMiscTypeCodeList(lstMiscTypeCode);

            foreach (doMiscTypeCode i in MiscLock)
                i.ValueDisplay = CommonUtil.TextCodeName(i.ValueCode, i.ValueDisplay);
            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, MiscLock, strDisplay, "ValueCode", attribute, include_1idx);
        }
        /// <summary>
        /// Generate supplier combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString SupplierComboBox(this HtmlHelper helper, string id, object attribute = null, string firstElement = null)
        {
            //IOfficeMasterHandler hand = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
            ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<SECOM_AJIS.DataEntity.Common.tbm_Supplier> supplier = hand.GetTbm_SupplierCode();

            CommonUtil.MappingObjectLanguage<SECOM_AJIS.DataEntity.Common.tbm_Supplier>(supplier);

            foreach (SECOM_AJIS.DataEntity.Common.tbm_Supplier i in supplier)
                i.SupplierName = CommonUtil.TextCodeName(i.SupplierCode, i.SupplierName);
            return CommonUtil.CommonComboBoxWithCustomFirstElement<SECOM_AJIS.DataEntity.Common.tbm_Supplier>(id, supplier, "SupplierName", "SupplierCode", firstElement, attribute);
        }
        /// <summary>
        /// ChangType Combobox for CMS150 :Nattapong N
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString ChangeTypeComboBoxCMS150(this HtmlHelper helper, string id, string serviceType, object attribute = null)
        {
            // RenlMisc 
            ICommonHandler handCom = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<doMiscTypeCode> MiscTypeCode = new List<doMiscTypeCode>();

            MiscTypeCode.Add(new doMiscTypeCode());

            if (serviceType == ServiceType.C_SERVICE_TYPE_RENTAL)
                MiscTypeCode[0].FieldName = MiscType.C_RENTAL_CHANGE_TYPE;
            else
                MiscTypeCode[0].FieldName = MiscType.C_SALE_CHANGE_TYPE;

            MiscTypeCode[0].ValueCode = "%";


            List<doMiscTypeCode> MiscTypeResult = handCom.GetMiscTypeCodeList(MiscTypeCode);

            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, MiscTypeResult, "ValueCodeDisplay", "ValueCode", attribute, false);
        }
        /// <summary>
        /// Generate incident AR combobox for CMS150
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString IncidentARComboBoxCMS150(this HtmlHelper helper, string id, object attribute = null)
        {
            ICommonHandler handCom = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<doMiscTypeCode> MiscTypeCode = new List<doMiscTypeCode>();
            for (int i = 0; i < 2; i++)
                MiscTypeCode.Add(new doMiscTypeCode());
            MiscTypeCode[0].FieldName = MiscType.C_INCIDENT_TYPE;
            MiscTypeCode[0].ValueCode = "%";
            MiscTypeCode[1].FieldName = MiscType.C_AR_TYPE;
            MiscTypeCode[1].ValueCode = "%";
            List<doMiscTypeCode> MiscTypeResult = handCom.GetMiscTypeCodeList(MiscTypeCode);

            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, MiscTypeResult, "ValueCodeDisplay", "ValueCode", attribute, false);
        }
        /// <summary>
        /// Generate acquisition combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString AcquisitionComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
            try
            {
                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                {
                    new doMiscTypeCode()
                    {
                        FieldName = SECOM_AJIS.Common.Util.ConstantValue.MiscType.C_ACQUISITION_TYPE,
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

            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, lst, "ValueCodeDisplay", "ValueCode", attribute);
        }
        /// <summary>
        /// Generate motivation combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString MotivationComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
            try
            {
                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                {
                    new doMiscTypeCode()
                    {
                        FieldName = SECOM_AJIS.Common.Util.ConstantValue.MiscType.C_MOTIVATION_TYPE,
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

            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, lst, "ValueCodeDisplay", "ValueCode", attribute);
        }
        /// <summary>
        /// Generate main structure type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString MainStructureTypeComboBox(this HtmlHelper helper, string id, string value, object attribute = null)
        {
            List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
            try
            {
                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                {
                    new doMiscTypeCode()
                    {
                        FieldName = SECOM_AJIS.Common.Util.ConstantValue.MiscType.C_MAIN_STRUCTURE_TYPE,
                        ValueCode = "%"
                    }
                };

                if (value != null)
                    miscs[0].ValueCode = value;

                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                lst = hand.GetMiscTypeCodeList(miscs);
            }
            catch
            {
            }

            if (lst == null)
                lst = new List<doMiscTypeCode>();

            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, lst, "ValueCodeDisplay", "ValueCode", attribute);
        }
        /// <summary>
        /// Generate building type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString BuildingTypeComboBox(this HtmlHelper helper, string id, string value, object attribute = null)
        {
            List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
            try
            {
                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                {
                    new doMiscTypeCode()
                    {
                        FieldName = SECOM_AJIS.Common.Util.ConstantValue.MiscType.C_BUILDING_TYPE,
                        ValueCode = "%"
                    }
                };

                if (value != null)
                    miscs[0].ValueCode = value;

                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                lst = hand.GetMiscTypeCodeList(miscs);
            }
            catch
            {
            }

            if (lst == null)
                lst = new List<doMiscTypeCode>();

            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, lst, "ValueCodeDisplay", "ValueCode", attribute);
        }
        /// <summary>
        /// Generate log month-year combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString LogMonthYear(this HtmlHelper helper, string id, object attribute = null)
        {
            ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
            List<dtMonthYear> lst = hand.GetLogMonthYear();

            return CommonUtil.CommonComboBox<dtMonthYear>(id, lst, "strDisplay", "MonthYear", attribute, true);
        }
        /// <summary>
        /// Generate role type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <returns></returns>
        public static MvcHtmlString RoleTypeComboBox(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_CUST_ROLE_TYPE, attribute, null, include_idx0);
        }
        /// <summary>
        /// Generate rold type combobox (include "ALL" in index 0)
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <returns></returns>
        public static MvcHtmlString RoleTypeComboBoxWithFirstElement(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true)
        {
            return MiscTypeComboBoxWithFirstElement(helper, id, MiscType.C_CUST_ROLE_TYPE, "ALL", attribute, null, include_idx0);
        }
        /// <summary>
        /// Generate change type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="changeTypeFilter"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString ChangeTypeComboBox(this HtmlHelper helper, string id, string changeTypeFilter, object attribute = null, bool include_idx0 = true, string firstElement = null)
        {
            List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
            try
            {
                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>();

                if (changeTypeFilter == null || MiscType.C_ALL_CHANGE_TYPE.Equals(changeTypeFilter))
                {
                    miscs.Add(
                        new doMiscTypeCode()
                        {
                            FieldName = MiscType.C_RENTAL_CHANGE_TYPE,
                            ValueCode = "%"
                        });
                    miscs.Add(
                        new doMiscTypeCode()
                        {
                            FieldName = MiscType.C_SALE_CHANGE_TYPE,
                            ValueCode = "%"
                        });
                }
                else
                {
                    miscs.Add(
                        new doMiscTypeCode()
                        {
                            FieldName = changeTypeFilter,
                            ValueCode = "%"
                        });
                }

                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                lst = hand.GetMiscTypeCodeList(miscs);

                lst = (from p in lst orderby p.ValueCode ascending select p).ToList<doMiscTypeCode>();
            }
            catch { }

            if (lst == null)
                lst = new List<doMiscTypeCode>();

            string display = "ValueCodeDisplay";

            return CommonUtil.CommonComboBoxWithCustomFirstElement<doMiscTypeCode>(id, lst, display, "ValueCode", firstElement, attribute, include_idx0);
        }
        /// <summary>
        /// Generate change reason type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <returns></returns>
        public static MvcHtmlString ChangeReasonTypeComboBox(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_CHANGE_REASON_TYPE, attribute, null, include_idx0);
        }
        /// <summary>
        /// Generate change name reason type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <returns></returns>
        public static MvcHtmlString ChangeNameReasonTypeComboBox(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_CHANGE_NAME_REASON_TYPE, attribute, null, include_idx0);
        }
        /// <summary>
        /// Generate process management combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <returns></returns>
        public static MvcHtmlString ProcessManagementComboBox(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_SALE_PROC_MANAGE_STATUS, attribute, null, include_idx0);
        }
        /// <summary>
        /// Generate process manangement (include "ALL" in index 0)
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <returns></returns>
        public static MvcHtmlString ProcessManagementFirstElementAllComboBox(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true)
        {
            return MiscTypeComboBoxWithFirstElement(helper, id, MiscType.C_SALE_PROC_MANAGE_STATUS, "ALL", attribute, null, include_idx0);
        }
        /// <summary>
        /// Generate start stype combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <returns></returns>
        public static MvcHtmlString StartTypeComboBox(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_START_TYPE, attribute, null, include_idx0);
        }
        /// <summary>
        /// Generate start type combobox (include "ALL" in index 0)
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <returns></returns>
        public static MvcHtmlString StartTypeFirstElementAllComboBox(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true)
        {
            return MiscTypeComboBoxWithFirstElement(helper, id, MiscType.C_START_TYPE, "ALL", attribute, null, include_idx0);
        }
        /// <summary>
        /// Generate installation type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <returns></returns>
        public static MvcHtmlString InstallationTypeComboBox(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_INSTALL_TYPE, attribute, null, include_idx0);
        }
        /// <summary>
        /// Generate dispatch type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString DispatchTypeComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_DISPATCH_TYPE, attribute);
        }
        /// <summary>
        /// Generate phone line type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString PhoneLineTypeComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_PHONE_LINE_TYPE, attribute);
        }
        /// <summary>
        /// Generate phone line owner type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString PhoneLineOwnerTypeComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_PHONE_LINE_OWNER_TYPE, attribute);
        }
        /// <summary>
        /// Generate maintenance cycle combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString MaintenanceCycleComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_MA_CYCLE, attribute, "ValueDisplay", false);
        }
        /// <summary>
        /// Generate insurance type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString InsuranceTypeComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_INSURANCE_TYPE, attribute, null, false);
        }
        /// <summary>
        /// Generate maintenance target product type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString MaintenanceTargetProductTypeComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_MA_TARGET_PROD_TYPE, attribute);
        }
        /// <summary>
        /// Generate maintenance type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString MaintenanceTypeComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_MA_TYPE, attribute);
        }
        /// <summary>
        /// Generate maintenance fee type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString MaintenanceFeeTypeComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_MA_FEE_TYPE, attribute, null, false);
        }
        /// <summary>
        /// Generate number of date combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString NumberOfDateComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_NUM_OF_DATE, attribute);
        }
        /// <summary>
        /// Generate sentry guard area type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString SentryGuardAreaTypeComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_SG_AREA_TYPE, attribute);
        }
        /// <summary>
        /// Generate sentry guard type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString SentryGuardTypeComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_SG_TYPE, attribute);
        }
        /// <summary>
        /// Generate billing timimg type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString BillingTimingTypeComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_BILLING_TIMING, attribute);
        }
        /// <summary>
        /// Generate payment method combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="display_field"></param>
        /// <returns></returns>
        public static MvcHtmlString PaymentMethodComboBox(this HtmlHelper helper, string id, object attribute = null, string display_field = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_PAYMENT_METHOD, attribute, display_field);
        }
        /// <summary>
        /// Generate debt tracing payment method combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="display_field"></param>
        /// <returns></returns>
        public static MvcHtmlString DebtTracingPaymentMethodComboBox(this HtmlHelper helper, string id, object attribute = null, string display_field = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_DEBT_TRACING_PAYMENT_METHOD, attribute, display_field);
        }
        /// <summary>
        /// Generate payment method have condition combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="HaveAutoTranfer"></param>
        /// <param name="HaveCreditCard"></param>
        /// <param name="strDisplay"></param>
        /// <param name="include_index0"></param>
        /// <returns></returns>
        public static MvcHtmlString PaymentMethodHaveConditionComboBox(this HtmlHelper helper, string id, object attribute = null, bool HaveAutoTranfer = true, bool HaveCreditCard = true, string strDisplay = null, bool include_index0 = true)
        {
            List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
            try
            {
                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                {
                    new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_PAYMENT_METHOD,
                        ValueCode = "%"
                    }
                };

                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                lst = hand.GetMiscTypeCodeList(miscs);


                if (HaveAutoTranfer != true)
                {
                    lst = (from t in lst
                           where t.ValueCode != PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER
                           select t).ToList<doMiscTypeCode>();

                }
                if (HaveCreditCard != true)
                {
                    lst = (from t in lst
                           where t.ValueCode != PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER
                           select t).ToList<doMiscTypeCode>();
                }

            }
            catch
            {
            }

            if (lst == null)
                lst = new List<doMiscTypeCode>();


            if (strDisplay == null)
            {
                strDisplay = "ValueCodeDisplay";
            }

            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, lst, strDisplay, "ValueCode", attribute, include_index0);
        }
        /// <summary>
        /// Generate billing timimg type for register contract combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString BillingTimingTypeForRegisterContractComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
            try
            {
                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                {
                    new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_BILLING_TIMING,
                        ValueCode = BillingTiming.C_BILLING_TIMING_APPROVE_CONTRACT
                    },
                    new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_BILLING_TIMING,
                        ValueCode = BillingTiming.C_BILLING_TIMING_COMPLETE_INSTALLATION
                    },
                    new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_BILLING_TIMING,
                        ValueCode = BillingTiming.C_BILLING_TIMING_START_SERVICE
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

            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, lst, "ValueCodeDisplay", "ValueCode", attribute);
        }
        /// <summary>
        /// Generate billing timimg type for register contract combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="productTypeCode"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString BillingTimingTypeForRegisterContractComboBox(this HtmlHelper helper, string id, string productTypeCode, object attribute = null)
        {
            List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
            try
            {
                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>();
                miscs.Add(new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_BILLING_TIMING,
                        ValueCode = BillingTiming.C_BILLING_TIMING_APPROVE_CONTRACT
                    });

                if (productTypeCode == ProductType.C_PROD_TYPE_SALE
                    || productTypeCode == ProductType.C_PROD_TYPE_AL
                    || productTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
                {
                    miscs.Add(new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_BILLING_TIMING,
                        ValueCode = BillingTiming.C_BILLING_TIMING_COMPLETE_INSTALLATION
                    });
                }

                miscs.Add(new doMiscTypeCode()
                {
                    FieldName = MiscType.C_BILLING_TIMING,
                    ValueCode = BillingTiming.C_BILLING_TIMING_START_SERVICE
                });

                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                lst = hand.GetMiscTypeCodeList(miscs);
            }
            catch
            {
            }

            if (lst == null)
                lst = new List<doMiscTypeCode>();

            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, lst, "ValueCodeDisplay", "ValueCode", attribute);
        }
        /// <summary>
        /// Generate misc type combobox (include first element)
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
        /// <summary>
        /// Generate misc type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="fieldName"></param>
        /// <param name="attribute"></param>
        /// <param name="display_field"></param>
        /// <param name="include_idx0"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        private static MvcHtmlString MiscTypeComboBox(this HtmlHelper helper, string id, string fieldName, object attribute = null, string display_field = null, bool include_idx0 = true, string firstElement = null)
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
                lst = hand.GetMiscTypeCodeList(miscs).ToList(); 
            }
            catch
            {
            }

            if (lst == null)
                lst = new List<doMiscTypeCode>();

            string display = "ValueCodeDisplay";
            if (display_field != null)
                display = display_field;

            // Edit by Narupon W. 9 May 2012
            //return CommonUtil.CommonComboBox<doMiscTypeCode>(id, lst, display, "ValueCode", attribute, include_idx0 );
            return CommonUtil.CommonComboBoxWithCustomFirstElement<doMiscTypeCode>(id, lst, display, "ValueCode", firstElement, attribute, include_idx0);
        }
        /// <summary>
        /// Generate payment method type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <returns></returns>
        public static MvcHtmlString PaymentMethodTypeComboBox(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_PAYMENT_METHOD, attribute, null, include_idx0);
        }
        /// <summary>
        /// Generate stop cancel  reason combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString StopCancelReasonComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_STOP_CANCEL_REASON_TYPE, attribute);
        }
        /// <summary>
        /// Generate contract signer type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString ContractSignerTypeComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_CONTRACT_SIGNER_TYPE, attribute);
        }
        /// <summary>
        /// Generate billing cycle combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString BillingCycleComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_BILLING_CYCLE, attribute, "ValueDisplay", false);
        }
        /// <summary>
        /// Generate calculation daily fee combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString CalculationDailyFeeComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_CALC_DAILY_FEE_TYPE, attribute, null, false);
        }
        /// <summary>
        /// Generate incident role combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString IncidentRoleComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
            try
            {
                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>() {
                    new doMiscTypeCode() {
                        FieldName = MiscType.C_INCIDENT_ROLE,
                        ValueCode = IncidentRole.C_INCIDENT_ROLE_CHIEF
                    }
                };
                miscs.Add(new doMiscTypeCode()
                {
                    FieldName = MiscType.C_INCIDENT_ROLE,
                    ValueCode = IncidentRole.C_INCIDENT_ROLE_CORRESPONDENT
                });

                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                lst = hand.GetMiscTypeCodeList(miscs);
            }
            catch { }

            if (lst == null)
                lst = new List<doMiscTypeCode>();

            string display = "ValueCodeDisplay";

            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, lst, display, "ValueCode", attribute, false);
        }
        /// <summary>
        /// Generate incident due-date (deadline) combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString IncidentDueDate_DeadLineComboBox(this HtmlHelper helper, string id, object attribute = null, string firstElement = null)
        {
            return MiscTypeComboBoxWithFirstElement(helper, id, MiscType.C_INCIDENT_SEARCH_DUEDATE, firstElement, attribute);
        }
        /// <summary>
        /// Generate indident due-date (deadline) combobox for CTS320
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString IncidentDueDate_DeadLineComboBoxCTS320(this HtmlHelper helper, string id, object attribute = null)
        {
            return MvcHtmlString.Create(MiscTypeComboBoxWithFirstElement(helper, id, MiscType.C_INCIDENT_SEARCH_DUEDATE, "ALL", attribute, "ValueCodeDisplay", true).ToString().Replace("<option value=\"2\">", "<option value=\"2\" selected=\"true\">"));
        }
        /// <summary>
        /// Generate incident status combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString IncidentStatusComboBox(this HtmlHelper helper, string id, object attribute = null, string firstElement = null)
        {
            return MiscTypeComboBoxWithFirstElement(helper, id, MiscType.C_INCIDENT_STATUS, firstElement, attribute);
        }
        /// <summary>
        /// Generate incident search status combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString IncidentSearchStatusComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MvcHtmlString.Create(MiscTypeComboBoxWithFirstElement(helper, id, MiscType.C_INCIDENT_SEARCH_STATUS, "ALL", attribute, "ValueCodeDisplay", true).ToString().Replace("<option value=\"1\">", "<option value=\"1\" selected=\"true\">"));
        }
        /// <summary>
        /// Generate incident period combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString IncidentPeriodComboBox(this HtmlHelper helper, string id, object attribute = null, string firstElement = null)
        {
            //return MiscTypeComboBox(helper, id, MiscType.C_INCIDENT_SEARCH_PERIOD, attribute);
            return MiscTypeComboBoxWithFirstElement(helper, id, MiscType.C_INCIDENT_SEARCH_PERIOD, firstElement, attribute);
        }
        /// <summary>
        /// Generate incident type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString IncidentTypeComboBox(this HtmlHelper helper, string id, object attribute = null, string firstElement = null)
        {
            //return MiscTypeComboBox(helper, id, MiscType.C_INCIDENT_TYPE, attribute);
            return MiscTypeComboBoxWithFirstElement(helper, id, MiscType.C_INCIDENT_TYPE, firstElement, attribute);
        }
        /// <summary>
        /// Generate incident type combobox for CTS320
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString IncidentTypeComboBoxCTS320(this HtmlHelper helper, string id, object attribute = null)
        {
            return MvcHtmlString.Create(MiscTypeComboBoxWithFirstElement(helper, id, MiscType.C_INCIDENT_TYPE, "ALL", attribute, "ValueCodeDisplay", true).ToString());

        }
        /// <summary>
        /// Generate deadline time type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString DeadLineTimeTypeComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_DEADLINE_TIME_TYPE, attribute);
        }
        /// <summary>
        /// Generate fee type cancel contract combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString FeeTypeCancelContractComboBox(this HtmlHelper helper, string id, object attribute = null, string strProductTypeCode = null, bool? blFirstInstallCompleteFlag = null)
        {
            //List<tbm_BillingType> vw_list = new List<tbm_BillingType>();
            //IMasterHandler masHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;

            //string[] BillingTypeCodeList = {                                         
            //                                 BillingType.C_BILLING_TYPE_DEPOSIT_FEE ,
            //                                 BillingType.C_BILLING_TYPE_CONTRACT_FEE,
            //                                 BillingType.C_BILLING_TYPE_REMOVAL_INSTALLATION_FEE ,
            //                                 BillingType.C_BILLING_TYPE_CANCEL_CONTRACT_FEE ,
            //                                 BillingType.C_BILLING_TYPE_MAINTENANCE_FEE_ONE ,
            //                                 BillingType.C_BILLING_TYPE_CHANGE_INSTALLATION_FEE ,
            //                                 BillingType.C_BILLING_TYPE_CARD_FEE ,
            //                                 BillingType.C_BILLING_TYPE_OTHER_FEE
            //                                };

            //List<tbm_BillingType> list = (from t in masHandler.GetTbm_BillingType()
            //                              where BillingTypeCodeList.Contains<string>(t.BillingTypeCode)
            //                              select t).ToList<tbm_BillingType>();

            //List<tbm_BillingType> listOrder = new List<tbm_BillingType>();
            //foreach (var item in BillingTypeCodeList)
            //{
            //    listOrder.Add(list.FindAll(delegate(tbm_BillingType s) { return s.BillingTypeCode == item; })[0]);
            //}

            //vw_list = CommonUtil.ConvertObjectbyLanguage<tbm_BillingType, tbm_BillingType>(listOrder, "BillingTypeName");
            //return CommonUtil.CommonComboBox<tbm_BillingType>(id, vw_list, "BillingTypeCodeName", "BillingTypeCode", attribute);

            //Modify by Jutarat A. on 19072012
            //string[] BillingTypeCodeList = {                                         
            //                                 ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE,
            //                                 ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE,
            //                                 ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE,
            //                                 ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE,
            //                                 ContractBillingType.C_CONTRACT_BILLING_TYPE_CANCEL_CONTRACT_FEE,
            //                                 ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE,
            //                                 ContractBillingType.C_CONTRACT_BILLING_TYPE_CARD_FEE,
            //                                 ContractBillingType.C_CONTRACT_BILLING_TYPE_OTHER_FEE
            //                                };
            string[] BillingTypeCodeList = null;
            if (String.IsNullOrEmpty(strProductTypeCode) == false)
            {
                if (strProductTypeCode == ProductType.C_PROD_TYPE_AL || strProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
                {
                    if (blFirstInstallCompleteFlag == true)
                    {
                        BillingTypeCodeList = new string[] {                                         
                                                 ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE,
                                                 ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE,
                                                 ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE,
                                                 ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE,
                                                 ContractBillingType.C_CONTRACT_BILLING_TYPE_CANCEL_CONTRACT_FEE,
                                                 ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE,
                                                 ContractBillingType.C_CONTRACT_BILLING_TYPE_CARD_FEE,
                                                 ContractBillingType.C_CONTRACT_BILLING_TYPE_OTHER_FEE
                                                };
                    }
                    else
                    {
                        BillingTypeCodeList = new string[] {                                         
                                                     ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE,
                                                     ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE,
                                                     ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE,
                                                     ContractBillingType.C_CONTRACT_BILLING_TYPE_CANCEL_CONTRACT_FEE,
                                                     ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE,
                                                     ContractBillingType.C_CONTRACT_BILLING_TYPE_CARD_FEE,
                                                     ContractBillingType.C_CONTRACT_BILLING_TYPE_OTHER_FEE
                                                    };
                    }
                }
                else
                {
                    BillingTypeCodeList = new string[] {                                         
                                                 ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE,
                                                 ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE,
                                                 ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE,
                                                 ContractBillingType.C_CONTRACT_BILLING_TYPE_CANCEL_CONTRACT_FEE,
                                                 ContractBillingType.C_CONTRACT_BILLING_TYPE_CARD_FEE,
                                                 ContractBillingType.C_CONTRACT_BILLING_TYPE_OTHER_FEE
                                                };
                }
            }
            else
            {
                BillingTypeCodeList = new string[] {                                         
                                                 ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE,
                                                 ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE,
                                                 ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE,
                                                 ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE,
                                                 ContractBillingType.C_CONTRACT_BILLING_TYPE_CANCEL_CONTRACT_FEE,
                                                 ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE,
                                                 ContractBillingType.C_CONTRACT_BILLING_TYPE_CARD_FEE,
                                                 ContractBillingType.C_CONTRACT_BILLING_TYPE_OTHER_FEE
                                                };
            }
            //End Modify

            List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
            try
            {
                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                {
                    new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_CONTRACT_BILLING_TYPE,
                        ValueCode = "%"
                    }
                };

                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                lst = hand.GetMiscTypeCodeList(miscs);
            }
            catch
            { }

            if (lst == null)
                lst = new List<doMiscTypeCode>();

            List<doMiscTypeCode> listOrder = new List<doMiscTypeCode>();

            if (BillingTypeCodeList != null)
            {
                foreach (var item in BillingTypeCodeList)
                {
                    listOrder.Add(lst.FindAll(delegate(doMiscTypeCode s) { return s.ValueCode == item; })[0]);
                }
            }

            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, listOrder, "ValueCodeDisplay", "ValueCode", attribute, true);
        }
        /// <summary>
        /// Generate handling type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString HandlingTypeComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_HANDLING_TYPE, attribute);
        }
        /// <summary>
        /// Generate AR summary period combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString ARSummaryPeriodComboBox(this HtmlHelper helper, string id, object attribute = null, string firstElement = null)
        {
            List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
            try
            {
                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>() {
                    new doMiscTypeCode() {
                        FieldName = MiscType.C_AR_SUMMARY_PERIOD,
                        ValueCode = "%"
                    }
                };

                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                lst = hand.GetMiscTypeCodeList(miscs);
            }
            catch { }

            if (lst == null)
                lst = new List<doMiscTypeCode>();

            return CommonUtil.CommonComboBoxWithCustomFirstElement<doMiscTypeCode>(id, lst, "ValueCodeDisplay", "ValueCode", firstElement, attribute);
        }
        /// <summary>
        /// Generate incident summary period combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString IncidentSummaryPeriodComboBox(this HtmlHelper helper, string id, object attribute = null, string firstElement = null)
        {
            return MiscTypeComboBoxWithFirstElement(helper, id, MiscType.C_INCIDENT_SUMMARY_PERIOD, firstElement, attribute);
        }
        /// <summary>
        /// Generate AR role combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="includeIdx0"></param>
        /// <returns></returns>
        public static MvcHtmlString ARRoleComboBox(this HtmlHelper helper, string id, object attribute = null, bool includeIdx0 = true)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_AR_ROLE, attribute, null, includeIdx0);
        }
        /// <summary>
        /// Generate AR search status combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString ARSearchStatusComboBox(this HtmlHelper helper, string id, object attribute = null, string firstElement = null)
        {
            return MiscTypeComboBoxWithFirstElement(helper, id, MiscType.C_AR_SEARCH_STATUS, firstElement, attribute);
        }
        /// <summary>
        /// Generate AR search period combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString ARSearchPeriodComboBox(this HtmlHelper helper, string id, object attribute = null, string firstElement = null)
        {
            return MiscTypeComboBoxWithFirstElement(helper, id, MiscType.C_AR_SEARCH_PERIOD, firstElement, attribute);
        }

        /// <summary>
        /// Generate AR search period combobox (No DueDate MiscType)
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString ARSearchPeriodNoDueDateComboBox(this HtmlHelper helper, string id, object attribute = null, string firstElement = null)
        {
            List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
            try
            {
                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                {
                    new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_AR_SEARCH_PERIOD,
                        ValueCode = "%"
                    }
                };

                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                lst = hand.GetMiscTypeCodeList(miscs);

                foreach (doMiscTypeCode data in lst)
                {
                    if (data.ValueCode == ARSearchPeriod.C_AR_SEARCH_PERIOD_DUEDATE)
                    {
                        lst.Remove(data);
                        break;
                    }
                }
            }
            catch
            {
            }

            if (lst == null)
                lst = new List<doMiscTypeCode>();

            string display = "ValueCodeDisplay";
            return CommonUtil.CommonComboBoxWithCustomFirstElement<doMiscTypeCode>(id, lst, display, "ValueCode", firstElement, attribute, true);
        }

        /// <summary>
        /// Generate AR type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString ARTypeComboBox(this HtmlHelper helper, string id, object attribute = null, string firstElement = null)
        {
            //return MiscTypeComboBox(helper, id, MiscType.C_AR_TYPE, attribute);
            return MiscTypeComboBoxWithFirstElement(helper, id, MiscType.C_AR_TYPE, firstElement, attribute);
        }
        /// <summary>
        /// Generate document audit result combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString DocumnetAuditResultComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_DOC_AUDIT_RESULT, attribute);
        }
        /// <summary>
        /// Generate sale type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString SaleTypeComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_SALE_TYPE, attribute);
        }
        /// <summary>
        /// Generate sub contract or level combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString SubcontractorLevelComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_SUBCONTRACTOR_LEVEL, attribute);
        }
        /// <summary>
        /// Generate sub contractor skill combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString SubcontractorSkillComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_SUBCONTRACTOR_SKILL_LEVEL, attribute);
        }
        /// <summary>
        /// Generate AR type combobox for CTS370
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString ARTypeComboBoxCTS370(this HtmlHelper helper, string id, object attribute = null)
        {
            return MvcHtmlString.Create(MiscTypeComboBoxWithFirstElement(helper, id, MiscType.C_AR_TYPE, "ALL", attribute, "ValueCodeDisplay", true).ToString());
        }
        /// <summary>
        /// Generate AR due-date (deadline) combobox for CTS370
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString ARDueDate_DeadLineComboBoxCTS370(this HtmlHelper helper, string id, object attribute = null)
        {
            return MvcHtmlString.Create(MiscTypeComboBoxWithFirstElement(helper, id, MiscType.C_AR_SEARCH_DUEDATE, "ALL", attribute, "ValueCodeDisplay", true).ToString().Replace("<option value=\"2\">", "<option value=\"2\" selected=\"true\">"));
        }
        /// <summary>
        /// Generate AR search status combobox for CTS370
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString ARSearchStatusComboBoxCTS370(this HtmlHelper helper, string id, object attribute = null)
        {
            return MvcHtmlString.Create(MiscTypeComboBoxWithFirstElement(helper, id, MiscType.C_AR_SEARCH_STATUS, "ALL", attribute, "ValueCodeDisplay", true).ToString().Replace("<option value=\"1\">", "<option value=\"1\" selected=\"true\">"));
        }
        /// <summary>
        /// Generate incident role combobox (include "ALL" in index 0)
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString IncidentRoleAllComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBoxWithFirstElement(helper, id, MiscType.C_INCIDENT_ROLE, null, attribute, "ValueDisplay", true);
        }
        /// <summary>
        /// Generate contract document language combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString ContractDocumentLanguageComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_DOC_LANGUAGE, attribute);
        }
        /// <summary>
        /// Generate document name combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString DocumentNameComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_RENTAL_COVER_LETTER_DOC_CODE, attribute);
        }
        /// <summary>
        /// Generate rental document name combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString RentalDocumentNameComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_RENTAL_COVER_LETTER_DOC_CODE, attribute);
        }
        /// <summary>
        /// Generate sale document name combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString SaleDocumentNameComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_SALE_COVER_LETTER_DOC_CODE, attribute);
        }
        /// <summary>
        /// Generate instrument area combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString InstrumentAreaComboBox(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true, string firstElement = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_INV_AREA, attribute, null, include_idx0, firstElement);
        }
        /// <summary>
        /// Generate instrument area combobox (include first element)
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="index0Display"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString InstrumentAreaComboBoxWithFirstElement(this HtmlHelper helper, string id, string index0Display, object attribute = null)
        {
            //return MiscTypeComboBox(helper, id, MiscType.C_INV_AREA, attribute);
            return MiscTypeComboBoxWithFirstElement(helper, id, MiscType.C_INV_AREA, index0Display, attribute, "ValueDisplay", true);
        }
        /// <summary>
        /// Generate shelf type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="index0Display"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString ShelfTypeComboBox(this HtmlHelper helper, string id, string index0Display, object attribute = null)
        {
            //return MiscTypeComboBox(helper, id, MiscType.C_INV_SHELF_TYPE, attribute);
            return MiscTypeComboBoxWithFirstElement(helper, id, MiscType.C_INV_SHELF_TYPE, index0Display, attribute, "ValueDisplay", true);
        }
        /// <summary>
        /// Generate issue invoice timing combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString IssueInvoiceTimingComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_ISSUE_INV_TIME, attribute, "ValueDisplay");
            //return MiscTypeComboBox(helper, id, MiscType.C_ISSUE_INV_TIME, attribute);
        }
        /// <summary>
        /// Generate issue invoice date combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString IssueInvoiceDateComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_ISSUE_INV_DATE, attribute, "ValueDisplay");
            //return MiscTypeComboBox(helper, id, MiscType.C_ISSUE_INV_DATE, attribute);
        }
        /// <summary>
        /// Generate invoice format combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString InvoiceFormatComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_INV_FORMAT, attribute, "ValueDisplay");
            //return MiscTypeComboBox(helper, id, MiscType.C_INV_FORMAT, attribute);
        }
        /// <summary>
        /// Generate signature type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString SignatureTypeComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_SIG_TYPE, attribute, "ValueDisplay");
            //return MiscTypeComboBox(helper, id, MiscType.C_SIG_TYPE, attribute);
        }
        /// <summary>
        /// Generate display language combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString DisplayLanguageComboBox(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true) //Modify (Add include_idx0) by Jutarat A. on 18122013
        {
            return MiscTypeComboBox(helper, id, MiscType.C_DOC_LANGUAGE, attribute, "ValueDisplay", include_idx0); //Modify (Add include_idx0) by Jutarat A. on 18122013
            //return MiscTypeComboBox(helper, id, MiscType.C_DOC_LANGUAGE, attribute);
        }
        /// <summary>
        /// Generate show payment due-date combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString ShowPaymentDuedateComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_SHOW_DUEDATE, attribute, "ValueDisplay");
            //return MiscTypeComboBox(helper, id, MiscType.C_SHOW_DUEDATE, attribute);
        }
        /// <summary>
        /// Generate issue receipt timing combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString IssueReceiptTiminngComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_ISSUE_REC_TIME, attribute, "ValueDisplay");
            //return MiscTypeComboBox(helper, id, MiscType.C_ISSUE_REC_TIME, attribute);
        }
        /// <summary>
        /// Generate auto transfer account combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString AutoTransferAccountComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_SHOW_BANK_ACC, attribute, "ValueDisplay");
            //return MiscTypeComboBox(helper, id, MiscType.C_SHOW_BANK_ACC, attribute);
        }
        /// <summary>
        /// Generate auto transfer with deduction combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString AutoTransferWithDeductionTypeComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_DEDUCT_TYPE, attribute, "ValueDisplay");
            //return MiscTypeComboBox(helper, id, MiscType.C_DEDUCT_TYPE, attribute);
        }
        /// <summary>
        /// Generate print issue date combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString PrintIssueDateComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_SHOW_ISSUE_DATE, attribute, "ValueDisplay");
            //return MiscTypeComboBox(helper, id, MiscType.C_SHOW_ISSUE_DATE, attribute);
        }
        /// <summary>
        /// Generate seprate invoice type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString SeparateInvoiceTypeComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_SEP_INV, attribute, "ValueDisplay");
            //return MiscTypeComboBox(helper, id, MiscType.C_SEP_INV, attribute);
        }
        /// <summary>
        /// Generate account type code combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString AccountTypeCodeComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_ACCOUNT_TYPE, attribute, "ValueDisplay");
            //return MiscTypeComboBox(helper, id, MiscType.C_ACCOUNT_TYPE, attribute, "ValueDisplay");
        }
        /// <summary>
        /// Generate auto transfer date code combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString AutoTransferDateCodeComboBox(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true, string firstElement = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_AUTO_TRANSFER_DATE, attribute, "ValueDisplay", include_idx0, firstElement);
            //return MiscTypeComboBox(helper, id, MiscType.C_AUTO_TRANSFER_DATE, attribute, "ValueDisplay");
        }
        /// <summary>
        /// Generate credit card type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString CreditCardTypeComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_CREDIT_CARD_TYPE, attribute, "ValueDisplay");
        }
        /// <summary>
        /// Generate credit card company type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString CreditCardCompanyTypeComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            //return MiscTypeComboBox(helper, id, MiscType.C_CREDIT_CARD_COMPANY, attribute, "ValueDisplay");
            return MiscTypeComboBox(helper, id, MiscType.C_CREDIT_CARD_COMPANY, attribute);
        }
        /// <summary>
        /// Generate rental installation type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <returns></returns>
        public static MvcHtmlString RentalInstallationTypeComboBox(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_RENTAL_INSTALL_TYPE, attribute, null, include_idx0);
        }
        /// <summary>
        /// Generate adjust type code combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString AdjustTypeCodeComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_ADJUST_TYPE, attribute, "ValueDisplay");
            //return MiscTypeComboBox(helper, id, MiscType.C_INSTALL_ADJUSTMENT, attribute);
        }
        /// <summary>
        /// Generate billing detail invoice format combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString BillingDetailIinvoiceFormatComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_BILLING_INV_FORMAT, attribute);
        }
        /// <summary>
        /// Generate issue invoice combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString IssueInvoiceComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_ISSUE_INV, attribute);
        }
        /// <summary>
        /// Generate issue invoice combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString IssueInvoiceComboBoxForBLS070(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_ISSUE_INV, attribute, "ValueDisplay", false);
        }
        /// <summary>
        /// Generate used payment combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString UsedPaymentComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_USED_PAYMENT_METHOD, attribute);
        }
        /// <summary>
        /// Generate used payment combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString UsedPaymentComboBoxForBLS070(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_USED_PAYMENT_METHOD, attribute, "ValueDisplay", false);
        }
        /// <summary>
        /// Generate payment type combobox
        /// </summary>9
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="firstElement"></param>
        /// <param name="include_idx0"></param>
        /// <returns></returns>
        public static MvcHtmlString PaymentTypeComboBox(this HtmlHelper helper, string id, object attribute = null, string firstElement = null, bool include_idx0 = true)
        {
            //return MiscTypeComboBox(helper, id, MiscType.C_PAYMENT_TYPE, attribute);
            return MiscTypeComboBoxWithFirstElement(helper, id, MiscType.C_PAYMENT_TYPE, firstElement, attribute, null, include_idx0);
        }

        //Add by Jutarat A. on 06082013
        public static MvcHtmlString PaymentTypeExceptCreditNoteDecreasedComboBox(this HtmlHelper helper, string id, object attribute = null, string firstElement = null, bool include_idx0 = true)
        {
            List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
            try
            {
                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>() {
                    new doMiscTypeCode() {
                        FieldName = MiscType.C_PAYMENT_TYPE,
                        ValueCode = "%"
                    }
                };

                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                lst = hand.GetMiscTypeCodeList(miscs).Where(d => d.ValueCode != PaymentType.C_PAYMENT_TYPE_CREDITNOTE_DECREASED).ToList();

            }
            catch { }

            if (lst == null)
                lst = new List<doMiscTypeCode>();

            return CommonUtil.CommonComboBoxWithCustomFirstElement<doMiscTypeCode>(id, lst, "ValueCodeDisplay", "ValueCode", firstElement, attribute, include_idx0);
        }
        //End Add

        /// <summary>
        /// Generate payment type exc credit note combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="firstElement"></param>
        /// <param name="include_idx0"></param>
        /// <returns></returns>
        public static MvcHtmlString PaymentTypeExcCreditNoteComboBox(this HtmlHelper helper, string id, object attribute = null, string firstElement = null, bool include_idx0 = true)
        {
            string fieldName = MiscType.C_PAYMENT_TYPE;
            string display_field = null;

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
                lst = hand.GetMiscTypeCodeList(miscs).Where(d => d.ValueCode != PaymentType.C_PAYMENT_TYPE_CREDITNOTE_DECREASED
                                                            && d.ValueCode != PaymentType.C_PAYMENT_TYPE_CREDITNOTE_REFUND).ToList();
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
        /// <summary>
        /// Generate payment type exc credit note decreased combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="firstElement"></param>
        /// <param name="include_idx0"></param>
        /// <returns></returns>
        public static MvcHtmlString PaymentTypeExcCreditNoteDecreasedComboBox(this HtmlHelper helper, string id, object attribute = null, string firstElement = null, bool include_idx0 = true)
        {
            string fieldName = MiscType.C_PAYMENT_TYPE;
            string display_field = null;

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
                lst = hand.GetMiscTypeCodeList(miscs).Where(d => d.ValueCode != PaymentType.C_PAYMENT_TYPE_CREDITNOTE_DECREASED).ToList();
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
        /// <summary>
        /// Generate payment status search combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="firstElement"></param>
        /// <param name="include_idx0"></param>
        /// <returns></returns>
        public static MvcHtmlString PaymentStatusSearchComboBox(this HtmlHelper helper, string id, object attribute = null, string firstElement = null, bool include_idx0 = true)
        {
            return MiscTypeComboBoxWithFirstElement(helper, id, MiscType.C_PAYMENT_STATUS_SEARCH, firstElement, attribute, "ValueDisplay", include_idx0);
        }
        /// <summary>
        /// Generate payment status search combobox (only unmateched payment)
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="firstElement"></param>
        /// <param name="include_idx0"></param>
        /// <returns></returns>
        public static MvcHtmlString PaymentStatusSearchOnlyUnmatechedComboBox(this HtmlHelper helper, string id, object attribute = null, string firstElement = null, bool include_idx0 = true)
        {
            List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
            try
            {
                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>() {
                    new doMiscTypeCode() {
                        FieldName = MiscType.C_PAYMENT_STATUS_SEARCH,
                        ValueCode = "%"
                    }
                };

                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                lst = hand.GetMiscTypeCodeList(miscs).Where(d => d.ValueCode == PaymentStatusSearch.C_PAYMENT_STATUS_SERACH_ALL_UNMATCHED_PAYMENT).ToList();

            }
            catch { }

            if (lst == null)
                lst = new List<doMiscTypeCode>();

            return CommonUtil.CommonComboBoxWithCustomFirstElement<doMiscTypeCode>(id, lst, "ValueDisplay", "ValueCode", firstElement, attribute, include_idx0);
        }
        /// <summary>
        /// Generate correction reason combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString CorrectionReasonComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_CORRECTION_REASON, attribute, "ValueDisplay");
        }
        /// <summary>
        /// Generate instrument location combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString InstrumentLocationComboBox(this HtmlHelper helper, string id, object attribute = null, string firstElement = null)
        {
            return MiscTypeComboBoxWithFirstElement(helper, id, MiscType.C_INV_LOC, firstElement, attribute);
        }


        /// <summary>
        /// Generate Year combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString GetYearOfCarryOverProfitComboBox(this HtmlHelper helper, string id, object attribute = null, string firstElement = null)
        {
            List<doYearOfCarryOverProfit> lstYear = new List<doYearOfCarryOverProfit>();
            try
            {
                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                lstYear = hand.GetYearOfCarryOverProfit();
            }
            catch
            {
                lstYear = new List<doYearOfCarryOverProfit>();
            }

            return CommonUtil.CommonComboBoxWithCustomFirstElement<doYearOfCarryOverProfit>(id, lstYear, "ReportYear", "ReportYear", firstElement, attribute);

        }

        /// <summary>
        /// Generate Month combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString GetMonthOfCarryOverProfitComboBox(this HtmlHelper helper, string id, object attribute = null, string firstElement = null)
        {
            DateTime month = new DateTime(2000, 1, 1, 0, 0, 0);
            List<ItemValue<string>> ls = new List<ItemValue<string>>();

            for (int i = 0; i < 12; i++)
            {
                ls.Add(new ItemValue<string>()
                {
                    Value = string.Format("{0:00}", i + 1),
                    Display = month.ToString("MMMM")
                });

                month = month.AddMonths(1);
            }

            return CommonUtil.CommonComboBoxWithCustomFirstElement<ItemValue<string>>(id, ls, "Display", "Value", firstElement, attribute);

        }

        public static MvcHtmlString GetManageCarryOverProfitProductType(this HtmlHelper helper, string id, object attribute = null, string firstElement = null)
        {
            List<ItemValue<string>> ls = new List<ItemValue<string>>();
            ls.Add(new ItemValue<string>() { Display = "MA", Value = "MA" });
            ls.Add(new ItemValue<string>() { Display = "N", Value = "N" });
            ls.Add(new ItemValue<string>() { Display = "SG", Value = "SG" });

            return CommonUtil.CommonComboBoxWithCustomFirstElement<ItemValue<string>>(id, ls, "Display", "Value", firstElement, attribute);
        }

        /// <summary>
        /// Generate payment method combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="display_field"></param>
        /// <returns></returns>
        public static MvcHtmlString ChequeReturnReasonComboBox(this HtmlHelper helper, string id, object attribute = null, string display_field = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_CHEQUE_RETURN_REASON, attribute, display_field);
        }

        public static MvcHtmlString CMS490DocTypeCbo(this HtmlHelper helper, string id, object attribute = null, string display_field = null, string firstElement = null)
        {
            //List<tbs_MiscellaneousTypeCode> list = new List<tbs_MiscellaneousTypeCode>();
            //string strDisplayName = "ValueDisplayEN";
            //try
            //{
            //    IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
            //    list = handler.GetTbs_MiscellaneousTypeCode("DocType");


            //    if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
            //    {
            //        strDisplayName = "ValueDisplayEN";
            //    }
            //    else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
            //    {
            //        strDisplayName = "ValueDisplayJP";
            //    }
            //    else
            //    {
            //        strDisplayName = "ValueDisplayLC";
            //    }

            //}
            //catch
            //{
            //    list = new List<tbs_MiscellaneousTypeCode>();
            //}

            //return CommonUtil.CommonComboBox<tbs_MiscellaneousTypeCode>(id, list, strDisplayName, "ValueCode", attribute, false);

            List<tbm_DocumentTemplate> lst = new List<tbm_DocumentTemplate>();
            string strDisplayName = "DocumentNameEN";
            try
            {
                IDocumentHandler handler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
                lst = handler.GetTbm_DocumentTemplate("04", true);

                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    strDisplayName = "DocumentNameEN";
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    strDisplayName = "DocumentNameJP";
                }
                else
                {
                    strDisplayName = "DocumentNameLC";
                }

            }
            catch
            {
                lst = new List<tbm_DocumentTemplate>();
            }
            List<OfficeDataDo> list = new List<OfficeDataDo>();
            if (CommonUtil.dsTransData != null)
            {
                // use linq : select dsTransData.OfficeData[]  where FunctionSale = C_FUNC_SALE_YES 
                list = (from t in CommonUtil.dsTransData.dtOfficeData
                        where t.FunctionSale == FunctionSale.C_FUNC_SALE_YES
                        select t).ToList<OfficeDataDo>();
            }
            lst = (from x in lst
                   where x.DocumentCode == "BLR010"
                    || x.DocumentCode == "BLR020"
                    || x.DocumentCode == "ICR010"
                    || x.DocumentCode == "ICR020"
                   select x).ToList<tbm_DocumentTemplate>();
            //tbm_DocumentTemplate newlst = (from x in lst
            //                                where x.DocumentCode == "BLR010" 
            //                                            || x.DocumentCode == "BLR020"
            //                                            || x.DocumentCode == "ICR010"
            //                                            || x.DocumentCode == "ICR020"
            //                                           select x;
            //return CommonUtil.CommonComboBox<tbm_DocumentTemplate>(id, lst, strDisplayName, "DocumentCode", attribute);
            return CommonUtil.CommonComboBoxWithCustomFirstElement<tbm_DocumentTemplate>(id, lst, strDisplayName, "DocumentCode", firstElement, attribute);
        }

        /// <summary>
        /// Generate currency combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="currency"></param>
        /// <param name="attribute"></param>
        /// <param name="currencyType"></param>
        /// <param name="required"></param>
        /// <returns></returns>
        public static MvcHtmlString CurrencyCombobox(this HtmlHelper helper, string id, string currency = "1", object attribute = null, TextBoxHelper.MULTIPLE_CURRENCY_TYPE currencyType = TextBoxHelper.MULTIPLE_CURRENCY_TYPE.COMBOBOX, bool required = false)
        {
            bool isViewMode = false;
            if (attribute != null)
            {
                PropertyInfo prop = attribute.GetType().GetProperty("isViewMode");
                if (prop != null)
                {
                    bool mode = false;
                    if (bool.TryParse(prop.GetValue(attribute, null).ToString(), out mode))
                        isViewMode = mode;
                }
            }

            List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
            List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                {
                    new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_CURRENCT,
                        ValueCode = "%"
                    }
                };

            ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            lst = hand.GetMiscTypeCodeList(miscs).ToList();

            Dictionary<string, string> currencies = new Dictionary<string, string>();
            foreach (var l in lst)
            {
                currencies.Add(l.ValueCode, l.ValueDisplayEN);
            }

            string txtUnit = string.Empty;

            if (isViewMode)
            {
                txtUnit = string.Format("<span>{0}</span>", currencies[currency]);
            }
            else
            {
                if (currencyType == TextBoxHelper.MULTIPLE_CURRENCY_TYPE.COMBOBOX)
                {
                    var selectBuilder = new TagBuilder("select");
                    selectBuilder.MergeAttribute("id", id);
                    selectBuilder.MergeAttribute("name", id);

                    if (attribute != null)
                        CommonUtil.SetHtmlTagAttribute(selectBuilder, attribute);

                    foreach (string key in currencies.Keys)
                    {
                        var optionBuilder = new TagBuilder("option");
                        optionBuilder.MergeAttribute("value", key);
                        optionBuilder.InnerHtml = currencies[key];

                        string tt = optionBuilder.ToString(TagRenderMode.Normal);
                        if (currency == key)
                        {
                            string chk = "<option ";
                            int idx = tt.IndexOf(chk);
                            if (idx >= 0)
                                tt = tt.Substring(0, chk.Length) + "selected=\"true\" " + tt.Substring(chk.Length);
                        }

                        selectBuilder.InnerHtml += tt;
                    }

                    txtUnit = selectBuilder.ToString(TagRenderMode.Normal);
                }
                else
                {
                    TagBuilder spanBuilder = new TagBuilder("span");
                    spanBuilder.MergeAttribute("id", id + "CurrencyType");
                    spanBuilder.MergeAttribute("name", id + "CurrencyType");
                    spanBuilder.AddCssClass("label-currency");

                    if (attribute != null)
                        CommonUtil.SetHtmlTagAttribute(spanBuilder, attribute);

                    foreach (string key in currencies.Keys)
                    {
                        var ispanBuilder = new TagBuilder("span");
                        ispanBuilder.SetInnerText(currencies[key]);
                        ispanBuilder.MergeAttribute("data-type", key);

                        if (currency != key)
                            ispanBuilder.MergeAttribute("style", "display:none;");

                        spanBuilder.InnerHtml += ispanBuilder.ToString(TagRenderMode.Normal);
                    }

                    txtUnit = spanBuilder.ToString(TagRenderMode.Normal);
                }
            }


            string div = string.Format("<div class=\"combo-unit\">{0}</div>", txtUnit);
            string divr = string.Empty;
            if (required == true)
                divr = "&nbsp;<span class=\"label-remark\">*</span>";

            txtUnit = string.Format("{0}{1}",
                div, divr);

            return MvcHtmlString.Create(txtUnit);
        }
    }
}