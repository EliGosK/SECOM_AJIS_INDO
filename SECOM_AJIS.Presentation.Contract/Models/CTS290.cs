using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Contract;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Presentation.Contract.Models.MetaData;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.Presentation.Contract.Models
{
    /// <summary>
    /// Parameter of CTS290 screen
    /// </summary>
    public class CTS290_ScreenParameter : ScreenParameter
    {
        public List<CTS290_SearchResultGridData> doSearchResultGridData { get; set; }
        public List<dtSearchSaleWarrantyExpireResult> dtSearchSaleWarrantyExpireResultData { get; set; }
    }

    /// <summary>
    /// DO of SaleWarrantyExpire grid
    /// </summary>
    public class CTS290_SearchResultGridData : dtSearchSaleWarrantyExpireResult
    {
        [CSVMapping(Controller = MessageUtil.MODULE_CONTRACT,
                    Screen = "CTS290",
                    LabelName = "headerContractCodeOCC", 
                    SequenceNo = 1)]
        public string ContractCodeAndOCC
        {
            get
            {
                return String.Format("{0} / {1} /<br/> {2}", SaleContractCodeShort, SaleContractOCC, MAContractCodeShort);
            }
        }

        private string _saleWarrantyPeriod = string.Empty;

        [CSVMapping(Controller = MessageUtil.MODULE_CONTRACT,
                    Screen = "CTS290",
                    LabelName = "headerSaleWarrantyPeriod", 
                    SequenceNo = 2)]
        public string SaleWarrantyPeriod
        {
            get
            {
                string strWarrantyFrom = SaleWarrantyFrom == null ? string.Empty : SaleWarrantyFromDisplay;
                string strWarrantyTo = SaleWarrantyTo == null ? string.Empty : SaleWarrantyToDisplay;

                if (String.IsNullOrEmpty(strWarrantyFrom) == false)
                {
                    _saleWarrantyPeriod = String.Format("{0} - <br/>{1}", strWarrantyFrom, strWarrantyTo);        
                }

                return _saleWarrantyPeriod;
            }
        }

        [CSVMapping(Controller = MessageUtil.MODULE_CONTRACT,
                    Screen = "CTS290",
                    LabelName = "headerOfficeInChargeName",
                    SequenceNo = 3)]
        public string OfficeNameAndInChargeName
        {
            get
            {
                return String.Format("{0} /<br/>{1}", SaleContractOfficeName, SalesmanInChargeName);
            }
        }

        [CSVMapping(Controller = MessageUtil.MODULE_CONTRACT,
                    Screen = "CTS290",
                    LabelName = "headerSiteCodeOptOfficeName",
                    SequenceNo = 4)]
        public string SiteCodeAndOptOfficeName
        {
            get
            {
                return String.Format("{0} /<br/>{1}", MASiteCodeShort, SaleOperationOfficeName);
            }
        }

        [CSVMapping(Controller = MessageUtil.MODULE_CONTRACT,
                    Screen = "CTS290",
                    LabelName = "headerRealCustomerName", 
                    SequenceNo = 5)]
        public string RealCustomerName
        {
            get
            {
                //string strCustomerNameEN = String.IsNullOrEmpty(MARealCustomerNameEN) ? string.Empty : String.Format("(1) {0} ", MARealCustomerNameEN);
                //string strCustomerNameLC = String.IsNullOrEmpty(MARealCustomerNameLC) ? string.Empty : String.Format("<br/>(2) {0}", MARealCustomerNameLC);
                //return String.Format("{0}{1}", strCustomerNameEN, strCustomerNameLC);

                return CommonUtil.TextLineFormat(MARealCustomerNameEN, MARealCustomerNameLC);
            }
        }

        [CSVMapping(Controller = MessageUtil.MODULE_CONTRACT,
                    Screen = "CTS290",
                    LabelName = "headerSiteName", 
                    SequenceNo = 6)]
        public string SiteName
        {
            get
            {
                //string strSiteNameEN = String.IsNullOrEmpty(MASiteNameEN) ? string.Empty : String.Format("(1) {0} ", MASiteNameEN);
                //string strSiteNameLC = String.IsNullOrEmpty(MASiteNameLC) ? string.Empty : String.Format("<br/>(2) {0}", MASiteNameLC);
                //return String.Format("{0}{1}", strSiteNameEN, strSiteNameLC);

                return CommonUtil.TextLineFormat(MASiteNameEN, MASiteNameLC);
            }
        }

        [CSVMapping(Controller = MessageUtil.MODULE_CONTRACT,
                    Screen = "CTS290",
                    LabelName = "headerProductName",
                    SequenceNo = 7)]
        public string ProductName
        {
            get { return this.SaleProductName; }
        }
    }

    /// <summary>
    /// DO of SaleWarrantyExpire CSV
    /// </summary>
    public class CTS290_SearchResultCSVData
    {
        CommonUtil comUtil = new CommonUtil();

        [CSVMapping(HeaderName = "Sale contract no.", SequenceNo = 1)]
        public string SaleContractCodeShort { get; set; }

        [CSVMapping(HeaderName = "OCC", SequenceNo = 2)]
        public string SaleContractOCC { get; set; }

        [CSVMapping(HeaderName = "MA contract no.", SequenceNo = 3)]
        public string MAContractCodeShort { get; set; }

        [CSVMapping(HeaderName = "Warranty period (from)", SequenceNo = 4)]
        public string SaleWarrantyFromDisplay { get; set; }

        [CSVMapping(HeaderName = "Warranty period (to)", SequenceNo = 5)]
        public string SaleWarrantyToDisplay { get; set; }

        [CSVMapping(HeaderName = "Sales contract office", SequenceNo = 6)]
        public string SaleContractOfficeName { get; set; }

        [CSVMapping(HeaderName = "Salesman in charge", SequenceNo = 7)]
        public string SalesmanInChargeName { get; set; }

        [CSVMapping(HeaderName = "Premise's code", SequenceNo = 8)]
        public string MASiteCodeShort { get; set; }

        [CSVMapping(HeaderName = "Operation office", SequenceNo = 9)]
        public string SaleOperationOfficeName { get; set; }

        [CSVMapping(HeaderName = "Real customer (End user) name (English)", SequenceNo = 10)]
        public string MARealCustomerNameEN { get; set; }

        [CSVMapping(HeaderName = "Real customer (End user) name (Local)", SequenceNo = 11)]
        public string MARealCustomerNameLC { get; set; }

        [CSVMapping(HeaderName = "Premise's name (English)", SequenceNo = 12)]
        public string MASiteNameEN { get; set; }

        [CSVMapping(HeaderName = "Premise's name (Local)", SequenceNo = 13)]
        public string MASiteNameLC { get; set; }

        [CSVMapping(HeaderName = "Product name", SequenceNo = 14)]
        public string SaleProductName { get; set; }

        [CSVMapping(HeaderName = "Customer name (English)", SequenceNo = 15)]
        public string SalePurchaserNameEN { get; set; }

        [CSVMapping(HeaderName = "Customer name (Local)", SequenceNo = 16)]
        public string SalePurchaserNameLC { get; set; }


        public string SalePurchaserCustCode { get; set; }

        [CSVMapping(HeaderName = "Customer code", SequenceNo = 17)]
        public string SalePurchaserCustCodeShort
        {
            get
            {
                return comUtil.ConvertCustCode(SalePurchaserCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }


        public string MARealCustomerCustCode { get; set; }

        [CSVMapping(HeaderName = "Real customer customer code", SequenceNo = 18)]
        public string MARealCustomerCustCodeShort
        {
            get
            {
                return comUtil.ConvertCustCode(MARealCustomerCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        //[CSVMapping(HeaderName = "Normal sale price", SequenceNo = 19)]
        //public Nullable<decimal> NormalSalePrice { get; set; }

        //[CSVMapping(HeaderName = "Order sale price", SequenceNo = 20)]
        //public Nullable<decimal> OrderSalePrice { get; set; }

        //[CSVMapping(HeaderName = "Project code", SequenceNo = 21)]
        //public string ProjectCode { get; set; }

        //[CSVMapping(HeaderName = "Plan code", SequenceNo = 22)]
        //public string PlanCode { get; set; }

        [CSVMapping(HeaderName = "Normal product price (Rp.)", SequenceNo = 19)]
        public Nullable<decimal> NormalProductPrice { get; set; }
        [CSVMapping(HeaderName = "Normal product price (US$)", SequenceNo = 20)]
        public Nullable<decimal> NormalProductPriceUsd { get; set; }
        [CSVMapping(HeaderName = "Order product price (Rp.)", SequenceNo = 21)]
        public Nullable<decimal> OrderProductPrice { get; set; }
        [CSVMapping(HeaderName = "Order product price (US$)", SequenceNo = 22)]
        public Nullable<decimal> OrderProductPriceUsd { get; set; }

        [CSVMapping(HeaderName = "Normal installation fee (Rp.)", SequenceNo = 23)]
        public Nullable<decimal> NormalInstallFee { get; set; }
        [CSVMapping(HeaderName = "Normal installation fee (US$)", SequenceNo = 24)]
        public Nullable<decimal> NormalInstallFeeUsd { get; set; }
        [CSVMapping(HeaderName = "Order installation fee (Rp.)", SequenceNo = 25)]
        public Nullable<decimal> OrderInstallFee { get; set; }
        [CSVMapping(HeaderName = "Order installation fee (US$)", SequenceNo = 26)]
        public Nullable<decimal> OrderInstallFeeUsd { get; set; }

        [CSVMapping(HeaderName = "Project code", SequenceNo = 27)]
        public string ProjectCode { get; set; }
        [CSVMapping(HeaderName = "Plan code", SequenceNo = 28)]
        public string PlanCode { get; set; }
    }

    /// <summary>
    /// DO for validate SaleWarrantyExpire Period
    /// </summary>
    [MetadataType(typeof(CTS290_ValidateSaleWarrantyExpirePeriod_MetaData))]
    public class CTS290_ValidateSaleWarrantyExpirePeriod : doSearchSaleWarrantyExpireCondition
    {

    }
}

namespace SECOM_AJIS.Presentation.Contract.Models.MetaData
{
    public class CTS290_ValidateSaleWarrantyExpirePeriod_MetaData
    {
        [NotNullOrEmpty(ControlName="WarrantyMonthFrom")]
        public int ExpireWarrantyMonthFrom { get; set; }

        [NotNullOrEmpty(ControlName = "WarrantyYearFrom")]
        public int ExpireWarrantyYearFrom { get; set; }

        [NotNullOrEmpty(ControlName = "WarrantyMonthTO")]
        public int ExpireWarrantyMonthTo { get; set; }

        [NotNullOrEmpty(ControlName = "WarrantyYearTO")]
        public int ExpireWarrantyYearTo { get; set; }
    }
}

