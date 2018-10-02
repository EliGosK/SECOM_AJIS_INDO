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
using SECOM_AJIS.DataEntity.Inventory;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.Common.Util.ConstantValue;


namespace SECOM_AJIS.Presentation.Inventory.Helpers
{
    public static partial class ComboBoxHelper
    {
        /// <summary>
        /// Generate purchase order status combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString PurchaseOrderStatusCbo(this HtmlHelper helper, string id, object attribute = null)
        {
            ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<string> lst = new List<string>();
            lst.Add(MiscType.C_PURCHASE_ORDER_STATUS);
            List<doMiscTypeCode> dtPurchaseOrderType = comh.GetMiscTypeCodeListByFieldName(lst);

            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, dtPurchaseOrderType, "ValueCodeDisplay", "ValueCode", attribute, true, CommonUtil.eFirstElementType.All);

        }

        /// <summary>
        /// Generate transport type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="firstIdxType"></param>
        /// <returns></returns>
        public static MvcHtmlString TransportTypeCbo(this HtmlHelper helper, string id, object attribute = null, string firstIdxType = null)
        {
            ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<string> lst = new List<string>();
            lst.Add(MiscType.C_TRANSPORT_TYPE);
            List<doMiscTypeCode> dtTransportType = comh.GetMiscTypeCodeListByFieldName(lst);

            CommonUtil.eFirstElementType idx0_type = CommonUtil.eFirstElementType.All;
            if (firstIdxType == "Select")
                idx0_type = CommonUtil.eFirstElementType.Select;

            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, dtTransportType, "ValueCodeDisplay", "ValueCode", attribute, true, idx0_type);
        }

        /// <summary>
        /// Generate instrument area combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <param name="firstIdxType"></param>
        /// <returns></returns>
        public static MvcHtmlString InstrumentAreaCbo(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true, string firstIdxType = null)
        {
            ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<string> lst = new List<string>();
            lst.Add(MiscType.C_INV_AREA);
            List<doMiscTypeCode> dtInstrumentArea = comh.GetMiscTypeCodeListByFieldName(lst);

            CommonUtil.eFirstElementType idx0_type = CommonUtil.eFirstElementType.All;

            if (firstIdxType == "Select")
                idx0_type = CommonUtil.eFirstElementType.Select;

            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, dtInstrumentArea, "ValueCodeDisplay", "ValueCode", attribute, include_idx0, idx0_type);

        }

        /// <summary>
        /// Generate instrument area new combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString InstrumentAreaNewCbo(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true, string firstElement = null)
        {
            ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<string> lst = new List<string>();
            lst.Add(MiscType.C_INV_AREA);
            List<doMiscTypeCode> dtInstrumentArea = comh.GetMiscTypeCodeListByFieldName(lst);
            List<doMiscTypeCode> lstMatchMiscType = (from c in dtInstrumentArea
                                                     where
                                                         c.ValueCode == InstrumentArea.C_INV_AREA_NEW_SAMPLE ||
                                                         c.ValueCode == InstrumentArea.C_INV_AREA_NEW_DEMO ||
                                                         c.ValueCode == InstrumentArea.C_INV_AREA_NEW_RENTAL ||
                                                         c.ValueCode == InstrumentArea.C_INV_AREA_NEW_SALE
                                                     select c).ToList();

            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, lstMatchMiscType, "ValueCodeDisplay", "ValueCode", attribute);

        }

        /// <summary>
        /// Generate stockIn type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString StockInTypeCbo(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true, string firstElement = null)
        {
            ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<string> lst = new List<string>();
            lst.Add(MiscType.C_INV_STOCKIN_TYPE);
            List<doMiscTypeCode> dtStockIn = comh.GetMiscTypeCodeListByFieldName(lst);
            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, dtStockIn, "ValueCodeDisplay", "ValueCode", attribute, include_idx0, CommonUtil.eFirstElementType.All);
        }

        /// <summary>
        /// Generate register asset combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString RegisterAssetCbo(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true, string firstElement = null)
        {
            ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<string> lst = new List<string>();
            lst.Add(MiscType.C_INV_REGISTER_ASSET);
            List<doMiscTypeCode> dtResAsset = comh.GetMiscTypeCodeListByFieldName(lst);
            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, dtResAsset, "ValueCodeDisplay", "ValueCode", attribute, include_idx0, CommonUtil.eFirstElementType.All);
        }

        /// <summary>
        /// Generate purchase order type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString PurchaseOrderTypeCbo(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true, string firstElement = null)
        {
            ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<string> lst = new List<string>();
            lst.Add(MiscType.C_PURCHASE_ORDER_TYPE);
            List<doMiscTypeCode> dtMisc = comh.GetMiscTypeCodeListByFieldName(lst);
            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, dtMisc, "ValueCodeDisplay", "ValueCode", attribute);
        }

        /// <summary>
        /// Generate inventory location and pre-elimination combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="Location"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString InventoryLocationNPreEliminationCbo(this HtmlHelper helper, string id, string Location, object attribute = null, bool include_idx0 = true, string firstElement = null)
        {
            ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
            doMiscTypeCode Cond = new doMiscTypeCode();


            List<doMiscTypeCode> dtMisc = new List<doMiscTypeCode>();
            List<doMiscTypeCode> ResMisc = new List<doMiscTypeCode>();
            // lst.Add(MiscType.C_INV_LOC);


            if (Location == InstrumentLocation.C_INV_LOC_INSTOCK || Location == InstrumentLocation.C_INV_LOC_RETURNED)
            {
                Cond.FieldName = MiscType.C_INV_LOC;
                Cond.ValueCode = InstrumentLocation.C_INV_LOC_PRE_ELIMINATION;
                lst.Add(Cond);
                dtMisc = comh.GetMiscTypeCodeList(lst);
            }
            else
            {
                Cond.FieldName = MiscType.C_INV_LOC;
                Cond.ValueCode = "%";
                lst.Add(Cond);
                dtMisc = comh.GetMiscTypeCodeList(lst);

                ResMisc = (from c in dtMisc
                           where ((c.ValueCode == InstrumentLocation.C_INV_LOC_INSTOCK
                               || c.ValueCode == InstrumentLocation.C_INV_LOC_RETURNED
                               || c.ValueCode == InstrumentLocation.C_INV_LOC_PRE_ELIMINATION) &&

                               (Location == null || c.ValueCode != Location)
                               )
                           select c).ToList();
            }

            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, ResMisc, "ValueCodeDisplay", "ValueCode", attribute);


        }

        /// <summary>
        /// Generate inventory location checking combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="Location"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString InventoryLocationCheckingStockCbo(this HtmlHelper helper, string id, string Location, object attribute = null, bool include_idx0 = true, string firstElement = null)
        {
            ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<string> lst = new List<string>();
            List<doMiscTypeCode> dtMisc = new List<doMiscTypeCode>();
            List<doMiscTypeCode> ResMisc = new List<doMiscTypeCode>();
            lst.Add(MiscType.C_INV_LOC);
            dtMisc = comh.GetMiscTypeCodeListByFieldName(lst);


            //SELECT ValueCode, ValueDisplayEN, ValueDisplayLC, ValueDisplayJP 
            //FROM tbs_MiscellaneousTypeCode WHERE FieldName = C_INV_LOC 
            //AND ValueCode IN (C_INV_LOC_INSTOCK, C_INV_PRE_ELIMINATION, C_INV_REPAIRING) ORDER BY ValueDisplay


            ResMisc = (from p in dtMisc
                       where (p.ValueCode == InstrumentLocation.C_INV_LOC_INSTOCK ||
                              p.ValueCode == InstrumentLocation.C_INV_LOC_PRE_ELIMINATION ||
                              p.ValueCode == InstrumentLocation.C_INV_LOC_REPAIRING)
                       select p).ToList<doMiscTypeCode>();

            
            return CommonUtil.CommonComboBoxWithCustomFirstElement<doMiscTypeCode>(id, ResMisc, "ValueCodeDisplay", "ValueCode", firstElement, attribute, include_idx0);


        }

        /// <summary>
        /// Generate inventory location repair request combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="Location"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString InventoryLocationRepairRequestCbo(this HtmlHelper helper, string id, string Location, object attribute = null, bool include_idx0 = true, string firstElement = null)
        {
            ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<string> lst = new List<string>();
            List<doMiscTypeCode> dtMisc = new List<doMiscTypeCode>();
            List<doMiscTypeCode> ResMisc = new List<doMiscTypeCode>();
            lst.Add(MiscType.C_INV_LOC);
            dtMisc = comh.GetMiscTypeCodeListByFieldName(lst);

            ResMisc = (from c in dtMisc
                       where (c.ValueCode == InstrumentLocation.C_INV_LOC_INSTOCK ||
                           c.ValueCode == InstrumentLocation.C_INV_LOC_RETURNED)
                       select c).ToList<doMiscTypeCode>();

            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, ResMisc, "ValueCodeDisplay", "ValueCode", attribute, false);
        }

        /// <summary>
        /// Generate inventory location combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <returns></returns>
        public static MvcHtmlString InventoryLocationCbo(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = false)
        {
            var comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            var dtMisc = comh.GetMiscTypeCodeListByFieldName(new List<string>() { MiscType.C_INV_LOC });
            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, dtMisc, "ValueCodeDisplay", "ValueCode", attribute, include_idx0, CommonUtil.eFirstElementType.All);
        }

        /// <summary>
        /// Generate inventory location no loss WH combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <returns></returns>
        public static MvcHtmlString InventoryLocationNoLossWHComboBox(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = false)
        {
            var comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            var dtMisc = comh.GetMiscTypeCodeListByFieldName(new List<string>() { MiscType.C_INV_LOC })
                .Where(d =>
                    d.ValueCode != InstrumentLocation.C_INV_LOC_SOLD
                    && d.ValueCode != InstrumentLocation.C_INV_LOC_ELIMINATION
                    && d.ValueCode != InstrumentLocation.C_INV_LOC_VENDER
                    && d.ValueCode != InstrumentLocation.C_INV_LOC_LOSS
                    && d.ValueCode != InstrumentLocation.C_INV_LOC_SPECIAL
                )
                .OrderBy(d => d.ValueCodeDisplay)
                .ToList();
            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, dtMisc, "ValueCodeDisplay", "ValueCode", attribute, include_idx0, CommonUtil.eFirstElementType.All);
        }

        /// <summary>
        /// Generate invenotry area no lending combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <param name="firstIdxType"></param>
        /// <returns></returns>
        public static MvcHtmlString InvenotryAreaNoLendingCbo(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true, string firstIdxType = null, string[] filterAreaCode = null)
        {
            ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<string> lst = new List<string>();
            lst.Add(MiscType.C_INV_AREA); List<doMiscTypeCode> dtMisc = comh.GetMiscTypeCodeListByFieldName(lst);

            List<doMiscTypeCode> ResMisc = (from c in dtMisc where c.ValueCode != InstrumentArea.C_INV_AREA_SE_LENDING_DEMO select c).ToList<doMiscTypeCode>();

            if (filterAreaCode != null && filterAreaCode.Length > 0)
            {
                ResMisc = ResMisc.Where(d => filterAreaCode.Contains(d.ValueCode)).ToList();
            }

            CommonUtil.eFirstElementType idx0_type = CommonUtil.eFirstElementType.Select;

            if (firstIdxType == "All")
                idx0_type = CommonUtil.eFirstElementType.All;

            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, ResMisc, "ValueCodeDisplay", "ValueCode", attribute, include_idx0, idx0_type);
        }

        /// <summary>
        /// Generate inventory area combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <param name="firstIdxType"></param>
        /// <returns></returns>
        public static MvcHtmlString InventoryAreaCbo(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true, string firstIdxType = null)
        {
            ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<string> lst = new List<string>();
            lst.Add(MiscType.C_INV_AREA);
            List<doMiscTypeCode> dtMisc = comh.GetMiscTypeCodeListByFieldName(lst);

            CommonUtil.eFirstElementType idx0_type = CommonUtil.eFirstElementType.Select;

            if (firstIdxType == "All")
                idx0_type = CommonUtil.eFirstElementType.All;

            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, dtMisc, "ValueCodeDisplay", "ValueCode", attribute, include_idx0, idx0_type);
        }

        /// <summary>
        /// Generate inventory area srinakarin combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <param name="firstIdxType"></param>
        /// <returns></returns>
        public static MvcHtmlString InventoryAreaSrinakarinCbo(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true, string firstIdxType = null)
        {
            ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<string> lst = new List<string>();
            List<doMiscTypeCode> dtMisc = new List<doMiscTypeCode>();
            List<doMiscTypeCode> ResMisc = new List<doMiscTypeCode>();
            lst.Add(MiscType.C_INV_AREA);
            dtMisc = comh.GetMiscTypeCodeListByFieldName(lst);

            ResMisc = (from c in dtMisc
                       where (c.ValueCode == InstrumentArea.C_INV_AREA_NEW_SAMPLE ||
                           c.ValueCode == InstrumentArea.C_INV_AREA_NEW_DEMO ||
                           c.ValueCode == InstrumentArea.C_INV_AREA_SE_LENDING_DEMO ||
                           c.ValueCode == InstrumentArea.C_INV_AREA_SE_HANDLING_DEMO)
                       select c).ToList<doMiscTypeCode>();

            CommonUtil.eFirstElementType idx0_type = CommonUtil.eFirstElementType.Select;

            if (firstIdxType == "All")
                idx0_type = CommonUtil.eFirstElementType.All;

            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, ResMisc, "ValueCodeDisplay", "ValueCode", attribute, include_idx0, idx0_type);
        }

        /// <summary>
        /// Generate inventory area srinakarin no lending combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <param name="firstIdxType"></param>
        /// <returns></returns>
        public static MvcHtmlString InventoryAreaSrinakarinNoLendingCbo(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true, string firstIdxType = null)
        {
            ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<string> lst = new List<string>();
            List<doMiscTypeCode> dtMisc = new List<doMiscTypeCode>();
            List<doMiscTypeCode> ResMisc = new List<doMiscTypeCode>();
            lst.Add(MiscType.C_INV_AREA);
            dtMisc = comh.GetMiscTypeCodeListByFieldName(lst);

            ResMisc = (from c in dtMisc
                       where (c.ValueCode == InstrumentArea.C_INV_AREA_NEW_SAMPLE ||
                           c.ValueCode == InstrumentArea.C_INV_AREA_NEW_DEMO ||
                           c.ValueCode == InstrumentArea.C_INV_AREA_SE_HANDLING_DEMO)
                       select c).ToList<doMiscTypeCode>();

            CommonUtil.eFirstElementType idx0_type = CommonUtil.eFirstElementType.Select;

            if (firstIdxType == "All")
                idx0_type = CommonUtil.eFirstElementType.All;

            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, ResMisc, "ValueCodeDisplay", "ValueCode", attribute, include_idx0, idx0_type);
        }

        /// <summary>
        /// Generate inventory area depo combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <param name="firstIdxType"></param>
        /// <returns></returns>
        public static MvcHtmlString InventoryAreaDepoCbo(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true, string firstIdxType = null)
        {
            ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<string> lst = new List<string>();
            List<doMiscTypeCode> dtMisc = new List<doMiscTypeCode>();
            List<doMiscTypeCode> ResMisc = new List<doMiscTypeCode>();
            lst.Add(MiscType.C_INV_AREA);
            dtMisc = comh.GetMiscTypeCodeListByFieldName(lst);

            ResMisc = (from c in dtMisc
                       where (c.ValueCode == InstrumentArea.C_INV_AREA_NEW_SAMPLE ||
                              c.ValueCode == InstrumentArea.C_INV_AREA_NEW_SALE ||
                              c.ValueCode == InstrumentArea.C_INV_AREA_NEW_RENTAL ||
                              c.ValueCode == InstrumentArea.C_INV_AREA_SE_RENTAL)
                       select c).ToList<doMiscTypeCode>();

            CommonUtil.eFirstElementType idx0_type = CommonUtil.eFirstElementType.Select;

            if (firstIdxType == "All")
                idx0_type = CommonUtil.eFirstElementType.All;

            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, ResMisc, "ValueCodeDisplay", "ValueCode", attribute, include_idx0, idx0_type);
        }

        /// <summary>
        /// Generate instrument area combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString IVS270InstrumentAreaCbo(this HtmlHelper helper, string id, object attribute = null)
        {
            ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            var lstAll = comh.GetMiscTypeCodeListByFieldName(new List<string>() { MiscType.C_INV_AREA });
            var lstFilter = (
                from p in lstAll
                where p.ValueCode != InstrumentArea.C_INV_AREA_SE_LENDING_DEMO
                    && p.ValueCode != InstrumentArea.C_INV_AREA_SE_HANDLING_DEMO
                    && p.ValueCode != InstrumentArea.C_INV_AREA_NEW_DEMO
                select p
            ).ToList<doMiscTypeCode>();
            CommonUtil.MappingObjectLanguage<doMiscTypeCode>(lstFilter);
            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, lstFilter, "ValueCodeDisplay", "ValueCode", attribute, true, CommonUtil.eFirstElementType.All);
        }

        // InventoryTransferBufferCbo
        /// <summary>
        /// Generate inventory transfer buffer combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString InventoryTransferBufferCbo(this HtmlHelper helper, string id, object attribute = null)
        {
            ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<string> lst = new List<string>();
            List<doMiscTypeCode> dtMisc = new List<doMiscTypeCode>();
            List<doMiscTypeCode> ResMisc = new List<doMiscTypeCode>();
            lst.Add(MiscType.C_INV_LOC);
            dtMisc = comh.GetMiscTypeCodeListByFieldName(lst);

            ResMisc = (from c in dtMisc
                       where (c.ValueCode == InstrumentLocation.C_INV_LOC_INSTOCK ||
                           c.ValueCode == InstrumentLocation.C_INV_LOC_BUFFER)
                       select c).ToList<doMiscTypeCode>();

            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, ResMisc, "ValueCodeDisplay", "ValueCode", attribute, false);
        }

        /// <summary>
        /// Generate currency combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString CurrencyCbo(this HtmlHelper helper, string id, object attribute = null)
        {
            ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<string> lst = new List<string>();
            lst.Add(MiscType.C_CURRENCY_TYPE);
            List<doMiscTypeCode> dtMisc = comh.GetMiscTypeCodeListByFieldName(lst);
            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, dtMisc, "ValueCodeDisplay", "ValueCode", attribute);
        }

        /// <summary>
        /// Generate source office combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString SourceOfficeCbo(this HtmlHelper helper, string id, object attribute = null)
        {
            List<doOffice> lst = new List<doOffice>();
            IInventoryHandler invh = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
            if (CommonUtil.dsTransData.dtUserData.MainDepartmentCode == DepartmentMaster.C_DEPT_INVENTORY ||
                CommonUtil.dsTransData.dtUserData.MainDepartmentCode == DepartmentMaster.C_DEPT_PURCHASE)
            {
                lst = invh.GetInventoryOffice();
            }
            else
            {
                lst = invh.GetAuthorityOffice(CommonUtil.dsTransData.dtUserData.EmpNo);
                //foreach (OfficeDataDo i in CommonUtil.dsTransData.dtOfficeData)
                //{
                //    if (LogisticFunction.C_OFFICELOGISTIC_NONE.Equals(i.FunctionLogistic))
                //    {
                //        continue;
                //    }
                //    doOffice office = new doOffice();
                //    office.OfficeCode = i.OfficeCode;
                //    office.OfficeNameEN = i.OfficeNameEN;
                //    office.OfficeNameJP = i.OfficeNameJP;
                //    office.OfficeNameLC = i.OfficeNameLC;
                //    office.OfficeName = i.OfficeName;
                //    lst.Add(office);
                //}
            }

            lst = (from x in lst
                                  orderby x.OfficeCode
                                  select x).ToList();

            return CommonUtil.CommonComboBox<doOffice>(id, lst, "OfficeCodeName", "OfficeCode", attribute, false);
        }

        /// <summary>
        /// Generate destination office combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString DestinationOfficeCbo(this HtmlHelper helper, string id, object attribute = null)
        {
            List<doOffice> lst = new List<doOffice>();
            IInventoryHandler invh = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
            lst = invh.GetInventoryOffice();

            lst = (from x in lst
                   orderby x.OfficeCode
                   select x).ToList();

            return CommonUtil.CommonComboBox<doOffice>(id, lst, "OfficeCodeName", "OfficeCode", attribute, false);

            //return null;
        }

        /// <summary>
        /// Generate inventory office combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="firstElemType"></param>
        /// <returns></returns>
        public static MvcHtmlString InventoryOfficeCbo(this HtmlHelper helper, string id, object attribute = null, CommonUtil.eFirstElementType firstElemType = CommonUtil.eFirstElementType.Select)
        {
            IOfficeMasterHandler hMaster = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
            var lstAllOffice = hMaster.GetTbm_Office();

            var lst = new List<doOffice>();
            lst = (
                from p in lstAllOffice
                where !string.IsNullOrEmpty(p.FunctionLogistic) 
                    && p.FunctionLogistic != FunctionLogistic.C_FUNC_LOGISTIC_NO
                select new doOffice
                {
                    OfficeCode = p.OfficeCode,
                    OfficeName = p.OfficeName,
                    OfficeNameEN = p.OfficeNameEN,
                    OfficeNameJP = p.OfficeNameJP,
                    OfficeNameLC = p.OfficeNameLC
                }
            ).ToList<doOffice>();

            // Language mappping
            CommonUtil.MappingObjectLanguage<doOffice>(lst);

            return CommonUtil.CommonComboBox<doOffice>(id, lst, "OfficeCodeName", "OfficeCode", attribute, true, firstElemType);
        }

        /// <summary>
        /// Generate inventory office belong combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="firstElemType"></param>
        /// <returns></returns>
        public static MvcHtmlString InventoryOfficeBelongCbo(this HtmlHelper helper, string id, object attribute = null, CommonUtil.eFirstElementType firstElemType = CommonUtil.eFirstElementType.Select)
        {
            var lst = (
                from p in CommonUtil.dsTransData.dtOfficeData
                where !string.IsNullOrEmpty(p.FunctionLogistic) 
                    && p.FunctionLogistic != FunctionLogistic.C_FUNC_LOGISTIC_NO
                select new doOffice
                {
                    OfficeCode = p.OfficeCode,
                    OfficeName = p.OfficeName,
                    OfficeNameEN = p.OfficeNameEN,
                    OfficeNameJP = p.OfficeNameJP,
                    OfficeNameLC = p.OfficeNameLC
                }
            ).ToList<doOffice>();

            // Language mappping
            CommonUtil.MappingObjectLanguage<doOffice>(lst);

            return CommonUtil.CommonComboBox<doOffice>(id, lst, "OfficeCodeName", "OfficeCode", attribute, true, firstElemType);
        }

        /// <summary>
        /// Generate inventory office belong no head combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="firstElemType"></param>
        /// <returns></returns>
        public static MvcHtmlString InventoryOfficeBelongNoHeadCbo(this HtmlHelper helper, string id, object attribute = null, CommonUtil.eFirstElementType firstElemType = CommonUtil.eFirstElementType.Select)
        {
            var lst = (
                from p in CommonUtil.dsTransData.dtOfficeData
                where !string.IsNullOrEmpty(p.FunctionLogistic) 
                    && p.FunctionLogistic != FunctionLogistic.C_FUNC_LOGISTIC_NO 
                    && p.FunctionLogistic != FunctionLogistic.C_FUNC_LOGISTIC_HQ
                select new doOffice
                {
                    OfficeCode = p.OfficeCode,
                    OfficeName = p.OfficeName,
                    OfficeNameEN = p.OfficeNameEN,
                    OfficeNameJP = p.OfficeNameJP,
                    OfficeNameLC = p.OfficeNameLC
                }
            ).ToList<doOffice>();

            // Language mappping
            CommonUtil.MappingObjectLanguage<doOffice>(lst);

            return CommonUtil.CommonComboBox<doOffice>(id, lst, "OfficeCodeName", "OfficeCode", attribute, true, firstElemType);
        }

        /// <summary>
        /// Generate inventory office combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString IVS020InventoryOfficeCbo(this HtmlHelper helper, string id, object attribute = null)
        {
            IOfficeMasterHandler hMaster = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
            var lstAllOffice = hMaster.GetTbm_Office();
            var lstInvHeadOfficeCode = (
                from p in lstAllOffice
                where p.FunctionLogistic == FunctionLogistic.C_FUNC_LOGISTIC_HQ
                select p.OfficeCode
            ).ToList<string>();

            var bHasHeadOffice = (
                from p in CommonUtil.dsTransData.dtOfficeData
                where lstInvHeadOfficeCode.Contains(p.OfficeCode)
                select p
            ).Count() > 0;

            try
            {
                if (bHasHeadOffice && (from p in CommonUtil.dsTransData.dtUserBelongingData
                                       where p.DepartmentCode == DepartmentMaster.C_DEPT_PURCHASE
                                       select p).Count() > 0)
                {
                    return InventoryOfficeCbo(helper, id, attribute, CommonUtil.eFirstElementType.All);
                }
                else if (bHasHeadOffice && (from p in CommonUtil.dsTransData.dtUserBelongingData
                                            where p.DepartmentCode == DepartmentMaster.C_DEPT_INVENTORY
                                            select p).Count() > 0)
                {
                    return InventoryOfficeBelongCbo(helper, id, attribute, CommonUtil.eFirstElementType.All);
                }
                else
                {
                    return InventoryOfficeBelongNoHeadCbo(helper, id, attribute, CommonUtil.eFirstElementType.All);
                }
            }
            catch
            {
                var lst = new List<doOffice>();
                CommonUtil.MappingObjectLanguage<doOffice>(lst);
                return CommonUtil.CommonComboBox<doOffice>(id, lst, "OfficeCodeName", "OfficeCode", attribute, true);
            }
        }

        //public static MvcHtmlString IVS201InventoryOfficeCbo(this HtmlHelper helper, string id, object attribute = null)
        //{
        //    IOfficeMasterHandler hMaster = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
        //    var lstAllOffice = hMaster.GetTbm_Office();
        //    var lstInvHeadOfficeCode = (
        //        from p in lstAllOffice
        //        where p.FunctionLogistic == FunctionLogistic.C_FUNC_LOGISTIC_HQ
        //        select p.OfficeCode
        //    ).ToList<string>();

        //    var bHasHeadOffice = (
        //        from p in CommonUtil.dsTransData.dtOfficeData
        //        where lstInvHeadOfficeCode.Contains(p.OfficeCode)
        //        select p
        //    ).Count() > 0;

        //    try
        //    {
        //        if (bHasHeadOffice)
        //        {
        //            return InventoryOfficeCbo(helper, id, attribute, CommonUtil.eFirstElementType.All);
        //        }
        //        else
        //        {
        //            return InventoryOfficeBelongCbo(helper, id, attribute, CommonUtil.eFirstElementType.All);
        //        }
        //    }
        //    catch
        //    {
        //        var lst = new List<doOffice>();
        //        CommonUtil.MappingObjectLanguage<doOffice>(lst);
        //        return CommonUtil.CommonComboBox<doOffice>(id, lst, "OfficeCodeName", "OfficeCode", attribute, true, CommonUtil.eFirstElementType.All);
        //    }
        //}

        /// <summary>
        /// Generate inventory office combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString IVS220InventoryOfficeCbo(this HtmlHelper helper, string id, object attribute = null)
        {
            if (CommonUtil.dsTransData.dtUserData.MainDepartmentCode == DepartmentMaster.C_DEPT_PURCHASE)
            {
                return InventoryOfficeCbo(helper, id, attribute, CommonUtil.eFirstElementType.All);
            }
            else
            {
                var lst = (
                    from p in CommonUtil.dsTransData.dtOfficeData
                    where !string.IsNullOrEmpty(p.FunctionLogistic)
                        && p.FunctionLogistic != FunctionLogistic.C_FUNC_LOGISTIC_NO
                    select p
                ).ToList<OfficeDataDo>();

                CommonUtil.MappingObjectLanguage<OfficeDataDo>(lst);
                return CommonUtil.CommonComboBox<OfficeDataDo>(id, lst, "OfficeCodeName", "OfficeCode", attribute, true, CommonUtil.eFirstElementType.All);
            }

        }

        /// <summary>
        /// Generate operation office combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString IVS240OperationOfficeCbo(this HtmlHelper helper, string id, object attribute = null)
        {
            var lst = (
                from p in CommonUtil.dsTransData.dtOfficeData
                where p.FunctionSecurity != "0"
                select p
            ).ToList<OfficeDataDo>();

            CommonUtil.MappingObjectLanguage<OfficeDataDo>(lst);
            return CommonUtil.CommonComboBox<OfficeDataDo>(id, lst, "OfficeCodeName", "OfficeCode", attribute, true, CommonUtil.eFirstElementType.All);
        }

        /// <summary>
        /// Generate office combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString OfficeCbo(this HtmlHelper helper, string id, object attribute = null)
        {


            if (CommonUtil.dsTransData.dtUserData.MainDepartmentCode == DepartmentMaster.C_DEPT_INVENTORY ||
                CommonUtil.dsTransData.dtUserData.MainDepartmentCode == DepartmentMaster.C_DEPT_PURCHASE)
            {
                List<doOffice> lst = new List<doOffice>();

                IInventoryHandler invh = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                lst = invh.GetInventoryOffice();

                CommonUtil.MappingObjectLanguage<doOffice>(lst);

                return CommonUtil.CommonComboBox<doOffice>(id, lst, "OfficeCodeName", "OfficeCode", attribute, true);
            }
            else
            {
                List<tbm_Office> lst = new List<tbm_Office>();

                IOfficeMasterHandler hMaster = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
                var lstAllOffice = hMaster.GetTbm_Office();

                var lstTrans = new List<tbm_Office>();

                lstTrans = (
                        from p in CommonUtil.dsTransData.dtOfficeData
                        where p.FunctionLogistic != LogisticFunction.C_OFFICELOGISTIC_NONE
                        select new tbm_Office
                        {
                            OfficeCode = p.OfficeCode,
                            OfficeName = p.OfficeName,
                            OfficeNameEN = p.OfficeNameEN,
                            OfficeNameJP = p.OfficeNameJP,
                            OfficeNameLC = p.OfficeNameJP
                        }
                    ).ToList<tbm_Office>();

                foreach (tbm_Office i in lstTrans)
                {
                    List<tbm_Office> of = lstAllOffice.FindAll(delegate(tbm_Office smDo)
                    {
                        return (smDo.OfficeCode == i.OfficeCode);
                    });

                    if (of != null)
                    {
                        lst.AddRange(of);
                    }
                }

                CommonUtil.MappingObjectLanguage<tbm_Office>(lst);

                return CommonUtil.CommonComboBox<tbm_Office>(id, lst, "OfficeDisplay", "OfficeCode", attribute, true);
            }
        }

        /// <summary>
        /// Generate inventory checking year combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString InventoryCheckingYearCbo(this HtmlHelper helper, string id, object attribute = null)
        {
            IInventoryHandler handler = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
            List<dtCheckingYear> list = handler.GetCheckingYear();
            return CommonUtil.CommonComboBox<dtCheckingYear>(id, list, "CheckingYear", "CheckingYear", attribute, true);
        }

        /// <summary>
        /// Generate inventory checking year month combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString InventoryCheckingYearMonthCbo(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true, string firstElement = null)
        {
            IInventoryHandler handler = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
            List<tbt_InventoryCheckingSchedule> list = handler.GetTbt_InventoryCheckingSchedule(null);

            return CommonUtil.CommonComboBoxWithCustomFirstElement<tbt_InventoryCheckingSchedule>(id, list, "CheckingYearMonth", "CheckingYearMonth", firstElement, attribute, include_idx0);
        }

        /// <summary>
        /// Generate slip status combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <returns></returns>
        public static MvcHtmlString SlipStatusCbo(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true)
        {
            var comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            var dtMisc = comh.GetMiscTypeCodeListByFieldName(new List<string>() { MiscType.C_INV_SLIP_STATUS });
            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, dtMisc, "ValueCodeDisplay", "ValueCode", attribute, include_idx0, CommonUtil.eFirstElementType.All);
        }

        public static MvcHtmlString StockReportTypeCbo(this HtmlHelper helper, string id, object attribute = null)
        {
            ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            var lstAll = comh.GetMiscTypeCodeListByFieldName(new List<string>() { MiscType.C_STOCK_REPORT_TYPE });
            var lstFilter = lstAll.Where(d => d.ValueCode != "4").ToList<doMiscTypeCode>();
            CommonUtil.MappingObjectLanguage<doMiscTypeCode>(lstFilter);
            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, lstFilter, "ValueCodeDisplay", "ValueCode", attribute, false);
        }

        public static MvcHtmlString EliminateTransferTypeCbo(this HtmlHelper helper, string id, object attribute = null)
        {
            ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            var lstAll = comh.GetMiscTypeCodeListByFieldName(new List<string>() { MiscType.C_ELIMINATE_TRANSFER_TYPE });
            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, lstAll, "ValueCodeDisplay", "ValueCode", attribute, false);
        }

        public static MvcHtmlString ChangeAreaReportInventoryAreaCbo(this HtmlHelper helper, string id, object attribute = null)
        {
            ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<string> lst = new List<string>();
            lst.Add(MiscType.C_INV_AREA);
            List<doMiscTypeCode> dtMisc = comh.GetMiscTypeCodeListByFieldName(lst).Where(d => 
                d.ValueCode == InstrumentArea.C_INV_AREA_NEW_SALE
                || d.ValueCode == InstrumentArea.C_INV_AREA_NEW_RENTAL
                || d.ValueCode == InstrumentArea.C_INV_AREA_NEW_DEMO
                || d.ValueCode == InstrumentArea.C_INV_AREA_SE_LENDING_DEMO
            ).ToList();

            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, dtMisc, "ValueCodeDisplay", "ValueCode", attribute, true, CommonUtil.eFirstElementType.Select);
        }

        public static MvcHtmlString StockReportTypeInclType4Cbo(this HtmlHelper helper, string id, object attribute = null)
        {
            ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            var lstAll = comh.GetMiscTypeCodeListByFieldName(new List<string>() { MiscType.C_STOCK_REPORT_TYPE });
            var lstFilter = lstAll.ToList<doMiscTypeCode>();
            CommonUtil.MappingObjectLanguage<doMiscTypeCode>(lstFilter);
            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, lstFilter, "ValueCodeDisplay", "ValueCode", attribute, false);
        }

        public static MvcHtmlString UnitCbo(this HtmlHelper helper, string id, object attribute = null)
        {
            ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            var lstAll = comh.GetMiscTypeCodeListByFieldName(new List<string>() { MiscType.C_UNIT });
            var lstFilter = lstAll.ToList<doMiscTypeCode>();
            CommonUtil.MappingObjectLanguage<doMiscTypeCode>(lstFilter);
            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, lstFilter, "ValueDisplay", "ValueCode", attribute, true, CommonUtil.eFirstElementType.Select);
        }

        /// <summary>
        /// Generate transfer type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="firstIdxType"></param>
        /// <returns></returns>
        public static MvcHtmlString TransferTypeCbo(this HtmlHelper helper, string id, object attribute = null, string firstIdxType = null)
        {
            ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<string> lst = new List<string>();
            lst.Add(TransferType.C_INV_TRANSFERTYPE);
            List<doMiscTypeCode> dtTransportType = comh.GetMiscTypeCodeListByFieldName(lst);

            CommonUtil.eFirstElementType idx0_type = CommonUtil.eFirstElementType.All;
            if (firstIdxType == "Select")
                idx0_type = CommonUtil.eFirstElementType.Select;

            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, dtTransportType, "ValueCodeDisplay", "ValueCode", attribute, true, idx0_type);
        }

        public static MvcHtmlString InProcessPeriodCbo(this HtmlHelper helper, string id, object attribute = null)
        {
            List<SelectListItem> lstPeriod = new List<SelectListItem>();
            try
            {
                IInventoryHandler comh = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                var lstAll = comh.GetStockReport_InProcessPeriod();
                foreach (string period in lstAll)
                {
                    var date = DateTime.ParseExact(period, "yyyyMM", null);
                    string text = date.ToString("MMMM-yyyy");
                    lstPeriod.Add(new SelectListItem() { Selected = false, Text = text, Value = period });
                }
            }
            catch
            { }
            return CommonUtil.CommonComboBox<SelectListItem>(id, lstPeriod, "Text", "Value", attribute, false);
        }

        public static MvcHtmlString PhysicalPeriodCbo(this HtmlHelper helper, string id, object attribute = null)
        {
            List<SelectListItem> lstPeriod = new List<SelectListItem>();
            try
            {
                IInventoryHandler comh = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                var lstAll = comh.GetStockReport_PhysicalPeriod();
                foreach (string period in lstAll)
                {
                    var date = DateTime.ParseExact(period, "yyyyMM", null);
                    string text = date.ToString("MMMM-yyyy");
                    lstPeriod.Add(new SelectListItem() { Selected = false, Text = text, Value = period });
                }
            }
            catch
            { }
            return CommonUtil.CommonComboBox<SelectListItem>(id, lstPeriod, "Text", "Value", attribute, false);
        }

    }
}