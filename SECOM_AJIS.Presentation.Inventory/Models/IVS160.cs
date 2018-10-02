using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Inventory;
using SECOM_AJIS.DataEntity.Common;

using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Presentation.Inventory.Models.MetaData;

namespace SECOM_AJIS.Presentation.Inventory.Models
{
    /// <summary>
    /// Parameter for screen IVS160.
    /// </summary>
    public class IVS160_ScreenParameter : ScreenParameter
    {
        private List<tbt_InventoryCheckingSlipDetail> _DetailList;
        public List<tbt_InventoryCheckingSlipDetail> DetailList
        {
            set
            {
                this._DetailList = value;
            }
            get
            {
                if (this._DetailList != null)
                {
                    int itemNo = 0;
                    int mod = 0;
                    int subtract = 0;
                    for (int i = 0; i < this._DetailList.Count; i++)
                    {
                        itemNo = (i + 1);
                        mod = itemNo % CommonValue.ROWS_PER_PAGE_FOR_INVENTORY_CHECKING;
                        subtract = itemNo / CommonValue.ROWS_PER_PAGE_FOR_INVENTORY_CHECKING;
                        if (itemNo < CommonValue.ROWS_PER_PAGE_FOR_INVENTORY_CHECKING)
                        {
                            this._DetailList[i].Page = 1;
                        }
                        else
                        {
                            this._DetailList[i].Page = (mod == 0 ? subtract : subtract + 1);
                        }

                        this._DetailList[i].SlipNo = this.SlipNo;
                        this._DetailList[i].CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        this._DetailList[i].CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        this._DetailList[i].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        this._DetailList[i].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    }
                }

                return this._DetailList;

            }
        }
        private List<dtCheckingDetailList> _DetailList_ForView;
        public List<dtCheckingDetailList> DetailList_ForView
        {
            set
            {
                this._DetailList_ForView = value;
            }
            get
            {
                if (this._DetailList_ForView != null)
                {
                    int itemNo = 0;
                    int mod = 0;
                    int subtract = 0;
                    for (int i = 0; i < this._DetailList_ForView.Count; i++)
                    {
                        itemNo = (i + 1);
                        mod = itemNo % CommonValue.ROWS_PER_PAGE_FOR_INVENTORY_CHECKING;
                        subtract = itemNo / CommonValue.ROWS_PER_PAGE_FOR_INVENTORY_CHECKING;
                        if (itemNo < CommonValue.ROWS_PER_PAGE_FOR_INVENTORY_CHECKING)
                        {
                            this._DetailList_ForView[i].Page = 1;
                        }
                        else
                        {
                            this._DetailList_ForView[i].Page = mod == 0 ? subtract : subtract + 1;
                        }
                    }
                }

                return this._DetailList_ForView;

            }
        }
        public int CurrentPage { set; get; }

        public DateTime? CheckingDate { set; get; }

        public string OfficeCode { set; get; }
        public string LocationCode { set; get; }
        public string AreaCode { get; set; }
        public string SlipNo { set; get; }

        [KeepSession]
        public string SlipNoReportPath { set; get; }
        [KeepSession]
        public string CheckingYearMonth { set; get; }
        [KeepSession]
        public DateTime? CheckingStartDate { set; get; }

    }

    /// <summary>
    /// DO for validate Checking Detail search condition.
    /// </summary>
    [MetadataType(typeof(CTS160_ScreenParameter_MetaData))]
    public class IVS160_SearchCondition : doGetCheckingDetailList
    {

    }

    /// <summary>
    /// DO for Add Data
    /// </summary>
    public class IVS160_AddData
    {

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY,
                     Screen = "IVS160",
                     Parameter = "lblInstrumentCode",
                     ControlName = "InstrumentCode")]
        public string InstrumentCode { set; get; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY,
                     Screen = "IVS160",
                     Parameter = "lblInstrumentArea",
                     ControlName = "InstrumentArea")]
        public string AreaCode { set; get; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY,
                     Screen = "IVS160",
                     Parameter = "lblShelfNo",
                     ControlName = "ShelfNo")]
        public string ShelfNo { set; get; }

        // extra field
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY,
                     Screen = "IVS160",
                     Parameter = "lblInstrumentArea",
                     ControlName = "InstrumentArea")]
        public string AreaCodeName { set; get; }
        public string AreaName { set; get; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY,
                     Screen = "IVS160",
                     Parameter = "lblInstrumentCode",
                     ControlName = "InstrumentCode")]
        public string InstrumentName { set; get; }
        public string key { get { return string.Format("{0}-{1}-{2}", this.InstrumentCode, this.AreaCode, this.ShelfNo); } }

    }

    /// <summary>
    /// DO of Page Information
    /// </summary>
    public class IVS160_PageInfo
    {
        public int CurrentPage { set; get; }
        public int TotalPage { set; get; }
        public int TotalItem { set; get; }
        public string TextPageInfo { get { return string.Format("Page {0} of {1} ({2} Item)", this.CurrentPage, this.TotalPage, this.TotalItem); } }

    }

    /// <summary>
    /// DO of Instrumet Information
    /// </summary>
    public class IVS160_InstrumetInfo
    {
        public string InstrumentCode { set; get; }
        public string InstrumentName { set; get; }
    }



}

namespace SECOM_AJIS.Presentation.Inventory.Models.MetaData
{
    public class CTS160_ScreenParameter_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY,
                     Screen = "IVS160",
                     Parameter = "lblOffice",
                     ControlName = "OfficeCode")]
        public string OfficeCode { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY,
                     Screen = "IVS160",
                     Parameter = "lblLocation",
                     ControlName = "LocationCode")]
        public string LocationCode { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY,
                     Screen = "IVS160",
                     Parameter = "headerInstrumentArea",
                     ControlName = "AreaCode")]
        public string AreaCode { get; set; }
    }
}
