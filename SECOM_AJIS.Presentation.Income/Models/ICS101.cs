using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Common;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;
//using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Income;
using SECOM_AJIS.Presentation.Income.Models.MetaData;
namespace SECOM_AJIS.Presentation.Income.Models
{
    /// <summary>
    /// Parameter of ICS101 screen
    /// </summary>
    public class ICS101_ScreenParameter : ScreenParameter
    {
        // send data back to client
        [KeepSession]
        public List<doGetMoneyCollectionManagementInfo> doGetMoneyCollectionManagementInfo { set; get; }

        // send data from client to server
        public ICS101_RegisterData RegisterData { set; get; }
        public string DeleteReceiptNo { set; get; }
    }

    // register com sent data to server
    /// <summary>
    /// DO of Register Header
    /// </summary>
    public class ICS101_HeaderRegisterData
    {
        public DateTime? dtpExpectedCollectDateFrom { set; get; }
        public DateTime? dtpExpectedCollectDateTo { set; get; }
        public List<string> chklCollectionArea { set; get; }
    }

    /// <summary>
    /// DO of Register Detail (Section1)
    /// </summary>
    public class ICS101_DetailRegisterDataSection1
    {
        public string txtReceiptNo { set; get; }
        public DateTime? dtpReceiptDate { set; get; }
        public string txtBillingTargetCode { set; get; }
        public string txtBillingClientName { set; get; }
        public string txtBillingClientAddress { set; get; }
        public string txtReceiptAmount { set; get; }
        public string txtCollectionArea { set; get; }
        public DateTime? dtpExpectedCollectDate { set; get; }
        public string txtMemo { set; get; }
        public string rowid { set; get; }
    }

    /// <summary>
    /// DO for Register Data
    /// </summary>
    public class ICS101_RegisterData
    {
        public ICS101_HeaderRegisterData Header { set; get; }
        public List<ICS101_DetailRegisterDataSection1> Detail1 { set; get; }
        public List<ICS101_CSVGridData> doICS101_CSVGridData { set; get; }
    }

    // dummy class for CSV Popup Export file.
    /// <summary>
    /// DO for CSV report
    /// </summary>
    public class ICS101_CSVGridData 
    {
        public ICS101_CSVGridData(){}

        private static string l_DateTimeColumnFormat = "dd-MMM-yyyy";
        private string _ReceiptNo;
        [CSVMapping(Controller = MessageUtil.MODULE_INCOME,
                    Screen = "ICS101",
                    LabelName = "csvHeaderReceiptNo", // Mapping with Language Resource File
                    SequenceNo = 1)]
        public string ReceiptNo
        {
            get
            {
                return String.Format("{0}", _ReceiptNo); 
            }
            set {_ReceiptNo = value;}
        }
        private string _ReceiptDate;
        [CSVMapping(Controller = MessageUtil.MODULE_INCOME,
                    Screen = "ICS101",
                    LabelName = "csvHeaderReceiptDate", // Mapping with Language Resource File
                    SequenceNo = 2)]
        public string ReceiptDate
        {
            get
            {
                return String.Format("{0}", Convert.ToDateTime(_ReceiptDate).ToString(l_DateTimeColumnFormat));
            }
            set {  _ReceiptDate = value; }
        }
        private string _BillingTargetCode;
        [CSVMapping(Controller = MessageUtil.MODULE_INCOME,
                    Screen = "ICS101",
                    LabelName = "csvHeaderBillingTargetCode", // Mapping with Language Resource File
                    SequenceNo = 3)]
        public string BillingTargetCode
        {
            get
            {
                return String.Format("{0}", _BillingTargetCode);
            }
            set {  _BillingTargetCode = value; }
        }
        private string _BillingClientNameEN;
        [CSVMapping(Controller = MessageUtil.MODULE_INCOME,
                    Screen = "ICS101",
                    LabelName = "csvHeaderBillingClientNameEN", // Mapping with Language Resource File
                    SequenceNo = 4)]
        public string BillingClientNameEN
        {
            get
            {
                return String.Format("{0}", _BillingClientNameEN);
            }
            set { _BillingClientNameEN  = value; }
        }
        private string _BillingClientNameLC;
        [CSVMapping(Controller = MessageUtil.MODULE_INCOME,
                    Screen = "ICS101",
                    LabelName = "csvHeaderBillingClientNameLC", // Mapping with Language Resource File
                    SequenceNo = 5)]
        public string BillingClientNameLC
        {
            get
            {
                return String.Format("{0}", _BillingClientNameLC);
            }
            set { _BillingClientNameLC  = value; }
        }
        private string _BillingClientAddressEN;
        [CSVMapping(Controller = MessageUtil.MODULE_INCOME,
                    Screen = "ICS101",
                    LabelName = "csvHeaderBillingClientAddressEN", // Mapping with Language Resource File
                    SequenceNo = 6)]
        public string BillingClientAddressEN
        {
            get
            {
                return String.Format("{0}", _BillingClientAddressEN);
            }
            set { _BillingClientAddressEN = value; }
        }
        private string _BillingClientAddressLC;
        [CSVMapping(Controller = MessageUtil.MODULE_INCOME,
                    Screen = "ICS101",
                    LabelName = "csvHeaderBillingClientAddressLC", // Mapping with Language Resource File
                    SequenceNo = 7)]
        public string BillingClientAddressLC
        {
            get
            {
                return String.Format("{0}", _BillingClientAddressLC);
            }
            set {  _BillingClientAddressLC = value; }
        }
        private string _ReceiptAmount;
        [CSVMapping(Controller = MessageUtil.MODULE_INCOME,
                    Screen = "ICS101",
                    LabelName = "csvHeaderReceiptAmount", // Mapping with Language Resource File
                    SequenceNo = 8,
                    ValueOutputFormat = CSVMappingAttribute.eValueOutputFormat.Text)]
        public string ReceiptAmount
        {
            get
            {
                return String.Format("{0}", _ReceiptAmount);
            }
            set { _ReceiptAmount  = value; }
        }
        private string _CollectionAreaName;
        [CSVMapping(Controller = MessageUtil.MODULE_INCOME,
                    Screen = "ICS101",
                    LabelName = "csvHeaderCollectionAreaName", // Mapping with Language Resource File
                    SequenceNo = 9)]
        public string CollectionAreaName
        {
            get
            {
                return String.Format("{0}", _CollectionAreaName);
            }
            set { _CollectionAreaName  = value; }
        }
        private string _ExpectedCollectDate;
        [CSVMapping(Controller = MessageUtil.MODULE_INCOME,
                    Screen = "ICS101",
                    LabelName = "csvHeaderExpectedCollectDate", // Mapping with Language Resource File
                    SequenceNo = 10)]
        public string ExpectedCollectDate
        {
            get
            {
                return String.Format("{0}", Convert.ToDateTime(_ExpectedCollectDate).ToString(l_DateTimeColumnFormat));
            }
            set { _ExpectedCollectDate = value; }
        }
        private string _Memo;
        [CSVMapping(Controller = MessageUtil.MODULE_INCOME,
                    Screen = "ICS101",
                    LabelName = "csvHeaderMemo", // Mapping with Language Resource File
                    SequenceNo = 11)]
        public string Memo
        {
            get
            {
                return String.Format("{0}", _Memo);
            }
            set { _Memo = value; }
        }
        //-----------------------


    }

    /// <summary>
    /// DO for get Money Collection Management Information
    /// </summary>
    public class ICS101_doGetMoneyCollectionManagementInfo : doGetMoneyCollectionManagementInfo
    {
        public string BillingTargetCodeShort
        {
            get
            {
                CommonUtil cmm = new CommonUtil();
                return cmm.ConvertBillingTargetCode(this.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }
        public string BillingClientName
        {
            get
            {
                return CommonUtil.TextLineFormat(this.BillingClientNameEN, this.BillingClientNameLC);
            }
        }
        public string BillingClientAddress
        {
            get
            {
                return CommonUtil.TextLineFormat(this.BillingClientAddressEN, this.BillingClientAddressLC);
            }
        }
        public string CollectionAreaCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.CollectionAreaCode, this.CollectionAreaName);
            }
        }
        public string ReceiptDateText
        {
            get
            {
                return CommonUtil.TextDate(this.ReceiptDate);
            }
        }
        public string ExpectedCollectDateText
        {
            get
            {
                return CommonUtil.TextDate(this.ExpectedCollectDate);
            }
        }

    }
}
