using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.WebPages;
using System.Collections.Generic;
using System.Linq;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Installation;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.DataEntity.Billing;
using SECOM_AJIS.Common.Util.ConstantValue;


namespace SECOM_AJIS.Presentation.Installation.Helpers
{
    public static partial class ComboBoxHelper
    {
        /// <summary>
        /// Generate installation type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString InstallationTypeComboBox(this HtmlHelper helper, string id, string InstallType, object attribute = null)
        {
            List<tbs_MiscellaneousTypeCode> list = new List<tbs_MiscellaneousTypeCode>();
            string strDisplayName = "ValueDisplayEN";
            try
            {
                IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                list = handler.GetTbs_MiscellaneousTypeCode(InstallType);
            }
            catch
            {
                list = new List<tbs_MiscellaneousTypeCode>();
            }

            return CommonUtil.CommonComboBox<tbs_MiscellaneousTypeCode>(id, list, "ValueDisplay", "ValueCode", attribute);

        }

        /// <summary>
        /// Generate all installation type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="InstallType"></param>
        /// <param name="attribute"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString InstallationTypeAllComboBoxWithFirstElement(this HtmlHelper helper, string id, string InstallType, object attribute = null, string firstElement = null)
        {           
            string strDisplayName = "ValueDisplayEN";
            List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
            List<doMiscTypeCode> lst2 = new List<doMiscTypeCode>();
            try
            {
                

                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                {
                    new doMiscTypeCode()
                    {
                        FieldName = "RentalInstallationType",
                        ValueCode = "%"
                    }
                };

                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                lst = hand.GetMiscTypeCodeList(miscs);

                miscs = new List<doMiscTypeCode>()
                {
                    new doMiscTypeCode()
                    {
                        FieldName = "SaleInstallationType",
                        ValueCode = "%"
                    }
                };
                lst2 = hand.GetMiscTypeCodeList(miscs);

                foreach (doMiscTypeCode dtl in lst2)
                {
                    lst.Add(dtl);
                }
            }
            catch
            {
                
            }
            
            return CommonUtil.CommonComboBoxWithCustomFirstElement<doMiscTypeCode>(id, lst, "ValueDisplay", "ValueCode", firstElement, attribute, true);

        }

        /// <summary>
        /// Generate new telephone line owner combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString NewTelephoneLineOwnerComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            List<tbs_MiscellaneousTypeCode> list = new List<tbs_MiscellaneousTypeCode>();
            string strDisplayName = "ValueDisplayEN";
            try
            {
                IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                list = handler.GetTbs_MiscellaneousTypeCode(MiscType.C_PHONE_LINE_OWNER_TYPE);

                foreach (var item in list)
                {
                    item.ValueDisplayEN = CommonUtil.TextCodeName(item.ValueCode, item.ValueDisplayEN);
                    item.ValueDisplayJP = CommonUtil.TextCodeName(item.ValueCode, item.ValueDisplayJP);
                    item.ValueDisplayLC = CommonUtil.TextCodeName(item.ValueCode, item.ValueDisplayLC);

                }

                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    strDisplayName = "ValueDisplayEN";
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    strDisplayName = "ValueDisplayJP";
                }
                else
                {
                    strDisplayName = "ValueDisplayLC";
                }

            }
            catch
            {
                list = new List<tbs_MiscellaneousTypeCode>();
            }

            return CommonUtil.CommonComboBox<tbs_MiscellaneousTypeCode>(id, list, strDisplayName, "ValueCode", attribute);

        }

        /// <summary>
        /// Generate cause reason combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="Reason"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString CauseReasonComboBox(this HtmlHelper helper, string id, string Reason, object attribute = null)
        {
            List<tbs_MiscellaneousTypeCode> list = new List<tbs_MiscellaneousTypeCode>();
            string strDisplayName = "ValueDisplayEN";
            try
            {
                IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                list = handler.GetTbs_MiscellaneousTypeCode(Reason);

                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    strDisplayName = "ValueDisplayEN";
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    strDisplayName = "ValueDisplayJP";
                }
                else
                {
                    strDisplayName = "ValueDisplayLC";
                }

            }
            catch
            {
                list = new List<tbs_MiscellaneousTypeCode>();
            }

            return CommonUtil.CommonComboBox<tbs_MiscellaneousTypeCode>(id, list, strDisplayName, "ValueCode", attribute);

        }

        /// <summary>
        /// Generate fee billing type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString FeeBillingTypeComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            List<tbs_MiscellaneousTypeCode> list = new List<tbs_MiscellaneousTypeCode>();
            string strDisplayName = "ValueDisplayEN";
            try
            {
                IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                list = handler.GetTbs_MiscellaneousTypeCode(MiscType.C_INSTALL_FEE_BILLING_TYPE);

                foreach (var item in list)
                {
                    item.ValueDisplayEN = CommonUtil.TextCodeName(item.ValueCode, item.ValueDisplayEN);
                    item.ValueDisplayJP = CommonUtil.TextCodeName(item.ValueCode, item.ValueDisplayJP);
                    item.ValueDisplayLC = CommonUtil.TextCodeName(item.ValueCode, item.ValueDisplayLC);

                }

                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    strDisplayName = "ValueDisplayEN";
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    strDisplayName = "ValueDisplayJP";
                }
                else
                {
                    strDisplayName = "ValueDisplayLC";
                }

            }
            catch
            {
                list = new List<tbs_MiscellaneousTypeCode>();
            }

            return CommonUtil.CommonComboBox<tbs_MiscellaneousTypeCode>(id, list, strDisplayName, "ValueCode", attribute);

        }

        /// <summary>
        /// Generate installation by combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString InstallationByComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            List<tbs_MiscellaneousTypeCode> list = new List<tbs_MiscellaneousTypeCode>();
            //string strDisplayName = "ValueDisplayEN";
            try
            {
                IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                list = handler.GetTbs_MiscellaneousTypeCode(MiscType.C_INSTALLATION_BY);
                CommonUtil.MappingObjectLanguage<tbs_MiscellaneousTypeCode>(list);
                //if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                //{
                //    strDisplayName = "ValueDisplayEN";
                //}
                //else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                //{
                //    strDisplayName = "ValueDisplayJP";
                //}
                //else
                //{
                //    strDisplayName = "ValueDisplayLC";
                //}

            }
            catch
            {
                list = new List<tbs_MiscellaneousTypeCode>();
            }

            return CommonUtil.CommonComboBox<tbs_MiscellaneousTypeCode>(id, list, "ValueCodeDisplay", "ValueCode", attribute);

        }

        /// <summary>
        /// Generate install slip output target combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString InstallSlipOutputTargetComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            List<dtOffice> list = new List<dtOffice>();
            dtOffice test = new dtOffice();

            string strDisplayName = "OfficeNameEN";
            try
            {
                IOfficeMasterHandler OMHandler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;

                list = OMHandler.GetFunctionLogistic();

                foreach (var item in list)
                {
                    item.OfficeNameEN = CommonUtil.TextCodeName(item.OfficeCode, item.OfficeNameEN);
                    item.OfficeNameJP = CommonUtil.TextCodeName(item.OfficeCode, item.OfficeNameJP);
                    item.OfficeNameLC = CommonUtil.TextCodeName(item.OfficeCode, item.OfficeNameLC);

                }

                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    strDisplayName = "OfficeNameEN";
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    strDisplayName = "OfficeNameJP";
                }
                else
                {
                    strDisplayName = "OfficeNameLC";
                }

            }
            catch
            {
                list = new List<dtOffice>();
            }

            return CommonUtil.CommonComboBox<dtOffice>(id, list, strDisplayName, "OfficeCode", attribute);

        }

        /// <summary>
        /// Generate stockout type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString StockOutTypeComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            List<tbs_MiscellaneousTypeCode> list = new List<tbs_MiscellaneousTypeCode>();
            //string strDisplayName = "ValueDisplayEN";
            try
            {
                IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                list = handler.GetTbs_MiscellaneousTypeCode(MiscType.C_STOCK_OUT_TYPE);
                CommonUtil.MappingObjectLanguage<tbs_MiscellaneousTypeCode>(list);
                //if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                //{
                //    strDisplayName = "ValueDisplayEN";
                //}
                //else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                //{
                //    strDisplayName = "ValueDisplayJP";
                //}
                //else
                //{
                //    strDisplayName = "ValueDisplayLC";
                //}

            }
            catch
            {
                list = new List<tbs_MiscellaneousTypeCode>();
            }

            return CommonUtil.CommonComboBox<tbs_MiscellaneousTypeCode>(id, list, "ValueCodeDisplay", "ValueCode", attribute);

        }

        /// <summary>
        /// Generate stockout origin additional combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString StockOutOriginAdditionalComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            List<dtOffice> list = new List<dtOffice>();
            string strDisplayName = "ValueDisplayEN";
            try
            {
                IOfficeMasterHandler handler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
                list = handler.GetFunctionLogistic();

                //foreach (var item in list)
                //{
                //    item.OfficeNameEN = CommonUtil.TextCodeName(item.OfficeCode, item.OfficeNameEN);
                //    item.OfficeNameJP = CommonUtil.TextCodeName(item.OfficeCode, item.OfficeNameJP);
                //    item.OfficeNameLC = CommonUtil.TextCodeName(item.OfficeCode, item.OfficeNameLC);

                //}

                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    strDisplayName = "OfficeNameEN";
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    strDisplayName = "OfficeNameJP";
                }
                else
                {
                    strDisplayName = "OfficeNameLC";
                }

            }
            catch
            {
                list = new List<dtOffice>();
            }

            return CommonUtil.CommonComboBox<dtOffice>(id, list, strDisplayName, "OfficeCode", attribute);

        }

        /// <summary>
        /// Generate complain combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString ComplainComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            List<tbs_MiscellaneousTypeCode> list = new List<tbs_MiscellaneousTypeCode>();
            string strDisplayName = "ValueDisplayEN";
            try
            {
                IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                list = handler.GetTbs_MiscellaneousTypeCode(MiscType.C_INSTALL_COMPLAIN);

                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    strDisplayName = "ValueDisplayEN";
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    strDisplayName = "ValueDisplayJP";
                }
                else
                {
                    strDisplayName = "ValueDisplayLC";
                }

            }
            catch
            {
                list = new List<tbs_MiscellaneousTypeCode>();
            }

            return CommonUtil.CommonComboBox<tbs_MiscellaneousTypeCode>(id, list, strDisplayName, "ValueCode", attribute);

        }

        /// <summary>
        /// Generate adjustment combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString AdjustMentComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            List<tbs_MiscellaneousTypeCode> list = new List<tbs_MiscellaneousTypeCode>();
            string strDisplayName = "ValueDisplayEN";
            try
            {
                IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                list = handler.GetTbs_MiscellaneousTypeCode(MiscType.C_INSTALL_ADJUSTMENT);

                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    strDisplayName = "ValueDisplayEN";
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    strDisplayName = "ValueDisplayJP";
                }
                else
                {
                    strDisplayName = "ValueDisplayLC";
                }

            }
            catch
            {
                list = new List<tbs_MiscellaneousTypeCode>();
            }

            return CommonUtil.CommonComboBox<tbs_MiscellaneousTypeCode>(id, list, strDisplayName, "ValueCode", attribute);

        }

        /// <summary>
        /// Generate adjustment contents combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString AdjustMentContentsComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            List<tbs_MiscellaneousTypeCode> list = new List<tbs_MiscellaneousTypeCode>();
            string strDisplayName = "ValueDisplayEN";
            try
            {
                IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                list = handler.GetTbs_MiscellaneousTypeCode(MiscType.C_INSTALL_ADJUST_CONTENTS);

                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    strDisplayName = "ValueDisplayEN";
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    strDisplayName = "ValueDisplayJP";
                }
                else
                {
                    strDisplayName = "ValueDisplayLC";
                }

            }
            catch
            {
                list = new List<tbs_MiscellaneousTypeCode>();
            }

            return CommonUtil.CommonComboBox<tbs_MiscellaneousTypeCode>(id, list, strDisplayName, "ValueCode", attribute);

        }

        /// <summary>
        /// Generate IE evaluation combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString IEEvaluationComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            List<tbs_MiscellaneousTypeCode> list = new List<tbs_MiscellaneousTypeCode>();
            string strDisplayName = "ValueDisplayEN";
            try
            {
                IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                list = handler.GetTbs_MiscellaneousTypeCode(MiscType.C_INSTALL_IE_EVALUATION);

                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    strDisplayName = "ValueDisplayEN";
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    strDisplayName = "ValueDisplayJP";
                }
                else
                {
                    strDisplayName = "ValueDisplayLC";
                }

            }
            catch
            {
                list = new List<tbs_MiscellaneousTypeCode>();
            }

            return CommonUtil.CommonComboBox<tbs_MiscellaneousTypeCode>(id, list, strDisplayName, "ValueCode", attribute);

        }

        /// <summary>
        /// Generate change reason code combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString ChangeReasonCodeComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            List<tbs_MiscellaneousTypeCode> list = new List<tbs_MiscellaneousTypeCode>();
            string strDisplayName = "ValueDisplayEN";
            try
            {
                IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                list = handler.GetTbs_MiscellaneousTypeCode(MiscType.C_INSTALL_BEFORE_CHANGE_REASON);

                foreach (var item in list)
                {
                    item.ValueDisplayEN = CommonUtil.TextCodeName(item.ValueCode, item.ValueDisplayEN);
                    item.ValueDisplayJP = CommonUtil.TextCodeName(item.ValueCode, item.ValueDisplayJP);
                    item.ValueDisplayLC = CommonUtil.TextCodeName(item.ValueCode, item.ValueDisplayLC);

                }

                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    strDisplayName = "ValueDisplayEN";
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    strDisplayName = "ValueDisplayJP";
                }
                else
                {
                    strDisplayName = "ValueDisplayLC";
                }

            }
            catch
            {
                list = new List<tbs_MiscellaneousTypeCode>();
            }

            return CommonUtil.CommonComboBox<tbs_MiscellaneousTypeCode>(id, list, strDisplayName, "ValueCode", attribute);

        }

        /// <summary>
        /// Generate change requester code combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString ChangeRequesterCodeComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            List<tbs_MiscellaneousTypeCode> list = new List<tbs_MiscellaneousTypeCode>();
            string strDisplayName = "ValueDisplayEN";
            try
            {
                IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                list = handler.GetTbs_MiscellaneousTypeCode(MiscType.C_INSTALL_BEFORE_CHANGE_REQUESTER);

                foreach (var item in list)
                {
                    item.ValueDisplayEN = CommonUtil.TextCodeName(item.ValueCode, item.ValueDisplayEN);
                    item.ValueDisplayJP = CommonUtil.TextCodeName(item.ValueCode, item.ValueDisplayJP);
                    item.ValueDisplayLC = CommonUtil.TextCodeName(item.ValueCode, item.ValueDisplayLC);

                }

                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    strDisplayName = "ValueDisplayEN";
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    strDisplayName = "ValueDisplayJP";
                }
                else
                {
                    strDisplayName = "ValueDisplayLC";
                }

            }
            catch
            {
                list = new List<tbs_MiscellaneousTypeCode>();
            }

            return CommonUtil.CommonComboBox<tbs_MiscellaneousTypeCode>(id, list, strDisplayName, "ValueCode", attribute);

        }

        /// <summary>
        /// Generate installation type stockout combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString InstallationTypeStockOutComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            List<tbs_MiscellaneousTypeCode> list = new List<tbs_MiscellaneousTypeCode>();

            try
            {
                IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;

                List<tbs_MiscellaneousTypeCode> lstRentalInstall = new List<tbs_MiscellaneousTypeCode>();
                List<tbs_MiscellaneousTypeCode> lstSaleInstall = new List<tbs_MiscellaneousTypeCode>();
                lstRentalInstall = handler.GetTbs_MiscellaneousTypeCode(MiscType.C_RENTAL_INSTALL_TYPE);
                lstSaleInstall = handler.GetTbs_MiscellaneousTypeCode(MiscType.C_SALE_INSTALL_TYPE);

                var arrValueCodeRetal = new string[] {
                    RentalInstallationType.C_RENTAL_INSTALL_TYPE_NEW, 
                    RentalInstallationType.C_RENTAL_INSTALL_TYPE_CHANGEPLAN_NEW, 
                    RentalInstallationType.C_RENTAL_INSTALL_TYPE_CHANGEPLAN_AFTER_NEW, 
                    RentalInstallationType.C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE,
                    RentalInstallationType.C_RENTAL_INSTALL_TYPE_PERIODICAL_MAINTENANCE
                };

                var arrValueCodeSale = new string[] {
                    SaleInstallationType.C_SALE_INSTALL_TYPE_NEW, 
                    SaleInstallationType.C_SALE_INSTALL_TYPE_ADD, 
                    SaleInstallationType.C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE
                };

                list.AddRange(
                    (
                        from t in lstRentalInstall
                        where arrValueCodeRetal.Contains(t.ValueCode)
                        select t
                    )
                    .Union(
                        from t in lstSaleInstall
                        where arrValueCodeSale.Contains(t.ValueCode)
                        select t
                    )
                );
            }
            catch
            {
                list = new List<tbs_MiscellaneousTypeCode>();
            }

            CommonUtil.MappingObjectLanguage<tbs_MiscellaneousTypeCode>(list);
            return CommonUtil.CommonComboBox<tbs_MiscellaneousTypeCode>(id, list, "ValueDisplay", "ValueCode", attribute, true, CommonUtil.eFirstElementType.All);

        }

        /// <summary>
        /// Generate subcontractor name combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString SubContractorNameComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            ICommonContractHandler hand = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
            List<tbm_SubContractor> lst = hand.GetTbm_SubContractorData(null);
            MvcHtmlString mvc = CommonUtil.CommonComboBox<tbm_SubContractor>(id, lst, "SubContractorName", "SubContractorCode", attribute);
            return mvc;
        }

        /// <summary>
        /// Generate installation history combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <returns></returns>
        public static MvcHtmlString InstallationHistoryComboBox(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true)
        {
            List<tbt_InstallationHistory> lst = new List<tbt_InstallationHistory>();
            try
            {
                IInstallationHandler handle = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                lst = handle.GetTbt_InstallationHistory(null, null, null);
                var sortedList = from p in lst
                                 orderby p.HistoryNo
                                 select p;

                lst = sortedList.ToList<tbt_InstallationHistory>();

                //lst = (from p in lst orderby p.DistrictName ascending select p).ToList<tbm_District>();
            }
            catch
            {
                lst = new List<tbt_InstallationHistory>();
            }

            return CommonUtil.CommonComboBox<tbt_InstallationHistory>(id, lst, "SlipNo", "SlipNo", attribute);
        }

        /// <summary>
        /// Generate install billing OCC combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="ContractCode"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <returns></returns>
        public static MvcHtmlString InstallBillingOCCComboBox(this HtmlHelper helper,string id, string ContractCode = null, object attribute = null, bool include_idx0 = true)
        {
            List<tbt_BillingBasic> lst = new List<tbt_BillingBasic>();
            try
            {
                IBillingHandler handle = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                lst = handle.GetTbt_BillingBasicListData(ContractCode);

                //var sortedList = from p in lst
                //                 orderby p.BillingOCC
                //                 select p;
                var sortedList = (from t in lst
                                  orderby t.BillingOCC
                                          group t by new
                                          {
                                              BillingOCC = t.BillingOCC
                                          } into g
                                          select g.FirstOrDefault());

                lst = sortedList.ToList<tbt_BillingBasic>();
            }
            catch
            {
                lst = new List<tbt_BillingBasic>();
            }

            return CommonUtil.CommonComboBox<tbt_BillingBasic>(id, lst, "BillingOCC", "BillingOCC", attribute);
        }

        public static MvcHtmlString InstallReportTypeCbo(this HtmlHelper helper, string id, string firstElement, object attribute = null)
        {

            List<tbs_MiscellaneousTypeCode> list = new List<tbs_MiscellaneousTypeCode>();
            string strDisplayName = "ValueDisplayEN";
            try
            {
                IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                list = handler.GetTbs_MiscellaneousTypeCode("InstallationReportType");

                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    strDisplayName = "ValueDisplayEN";
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    strDisplayName = "ValueDisplayJP";
                }
                else
                {
                    strDisplayName = "ValueDisplayLC";
                }

            }
            catch
            {
                list = new List<tbs_MiscellaneousTypeCode>();
            }
            return CommonUtil.CommonComboBoxWithCustomFirstElement<tbs_MiscellaneousTypeCode>(id, list, strDisplayName, "ValueCode", firstElement, attribute);
            
        }

        public static MvcHtmlString InstallTypeCboStatus(this HtmlHelper helper, string id, object attribute = null)
        {

            List<tbs_MiscellaneousTypeCode> list = new List<tbs_MiscellaneousTypeCode>();
            string strDisplayName = "ValueDisplayEN";
            try
            {
                IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                list = handler.GetTbs_MiscellaneousTypeCode("InstallationStatus");

                list = (from x in list
                        where x.ValueCode.Equals("04") || x.ValueCode.Equals("05")
                        select x).ToList();
                            

                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    strDisplayName = "ValueDisplayEN";
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    strDisplayName = "ValueDisplayJP";
                }
                else
                {
                    strDisplayName = "ValueDisplayLC";
                }

            }
            catch
            {
                list = new List<tbs_MiscellaneousTypeCode>();
            }

            return CommonUtil.CommonComboBox<tbs_MiscellaneousTypeCode>(id, list, strDisplayName, "ValueCode", attribute, false);
        }

        public static MvcHtmlString InstallTypeCbo(this HtmlHelper helper, string id, object attribute = null)
        {

            List<tbs_MiscellaneousTypeCode> list = new List<tbs_MiscellaneousTypeCode>();
            string strDisplayName = "ValueDisplayEN";
            try
            {
                IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                list = handler.GetTbs_MiscellaneousTypeCode("InstallationType");


                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    strDisplayName = "ValueDisplayEN";
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    strDisplayName = "ValueDisplayJP";
                }
                else
                {
                    strDisplayName = "ValueDisplayLC";
                }

            }
            catch
            {
                list = new List<tbs_MiscellaneousTypeCode>();
            }

            return CommonUtil.CommonComboBox<tbs_MiscellaneousTypeCode>(id, list, strDisplayName, "ValueCode", attribute, false);
        }

        public static MvcHtmlString ProductNameCbo(this HtmlHelper helper, string id, string firstElement, object attribute = null)
        {

            List<tbm_Product> list = new List<tbm_Product>();
            string strDisplayName = "ProductNameEN";
            try
            {
                IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                list = handler.GetTbm_Product(null,null);
                foreach (var items in list)
                {
                    items.ProductNameEN = items.ProductCode + " - " + items.ProductNameEN;
                    items.ProductNameJP = items.ProductCode + " - " + items.ProductNameJP;
                    items.ProductNameLC = items.ProductCode + " - " + items.ProductNameLC;
                }

                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    strDisplayName = "ProductNameEN";
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    strDisplayName = "ProductNameJP";
                }
                else
                {
                    strDisplayName = "ProductNameLC";
                }

            }
            catch
            {
                list = new List<tbm_Product>();
            }

            return CommonUtil.CommonComboBoxWithCustomFirstElement<tbm_Product>(id, list, strDisplayName, "ProductCode", firstElement, attribute);
           
        }

        public static MvcHtmlString InstallBuiildingType(this HtmlHelper helper, string id, object attribute = null)
        {

            List<tbs_MiscellaneousTypeCode> list = new List<tbs_MiscellaneousTypeCode>();
            string strDisplayName = "ValueDisplayEN";
            try
            {
                IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                list = handler.GetTbs_MiscellaneousTypeCode(MiscType.C_BUILDING_TYPE);
                
                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    strDisplayName = "ValueDisplayEN";
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    strDisplayName = "ValueDisplayJP";
                }
                else
                {
                    strDisplayName = "ValueDisplayLC";
                }
            }
            catch
            {
                list = new List<tbs_MiscellaneousTypeCode>();
            }

            return CommonUtil.CommonComboBox<tbs_MiscellaneousTypeCode>(id, list, strDisplayName, "ValueCode", attribute, true, CommonUtil.eFirstElementType.All);
        }

    }
}
