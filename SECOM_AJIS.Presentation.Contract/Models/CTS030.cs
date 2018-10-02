using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Contract.CustomEntity;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.Presentation.Contract.Models.MetaData;

namespace SECOM_AJIS.Presentation.Contract.Models
{
    //public class CTS030_CheckReqField
    //{
    //    [RelateObject("QuotationCode", ControlName = "Alphabet", RelateControlName = "QuotationCode")]
    //    public string Alphabet { get; set; }
    //    public string QuotationCode { get; set; }
    //}
    /// <summary>
    /// DO for validate condition
    /// </summary>
    [MetadataType(typeof(CTS030_Search_MetaData))]
    public class CTS030_Search : doSearchDraftContractCondition
    {
    }
    /// <summary>
    /// DO for CTS030 screen
    /// </summary>
    public class dsCTS030Data
    {
        private doSearchDraftContractCondition _doSearchCondition;
        private List<dtSearchDraftContractResult> _dtSearchResultList;

        public doSearchDraftContractCondition doSearchCondition
        {
            get { return this._doSearchCondition; }
            set { this._doSearchCondition = value; }
        }
        public List<dtSearchDraftContractResult> dtSearchResult
        {
            get { return this._dtSearchResultList; }
            set { this._dtSearchResultList = value; }
        }
    }
    /// <summary>
    /// DO for initial screen
    /// </summary>
    public class CTS030_ScreenParameter : ScreenParameter
    {
        public bool isAuditBtnClick { get; set; }
        public int CurrentIndex { get; set; }
        public int CurrentSortColIndex { get; set; }
        public string CurrentSortType { get; set; }
        public dsCTS030Data data { get; set; }

        [KeepSession]
        public List<tbs_MiscellaneousTypeCode> approvalStatus { get; set; }
    }
    /// <summary>
    /// DO for check result item
    /// </summary>
    public class CTS030_CheckResultItem
    {
        public string KeyIndex { get; set; }
    }
}

namespace SECOM_AJIS.Presentation.Contract.Models.MetaData
{
    /// <summary>
    /// Metadata for CTS030_Search DO
    /// </summary>
    public class CTS030_Search_MetaData
    {
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string QuotationCode { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        //[RelateObject("RegistrationDateTo", ControlName = "RegistrationDateFrom", RelateControlName = "RegistrationDateTo")]
        public DateTime? RegistrationDateFrom { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public DateTime? RegistrationDateTo { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string Salesman1Code { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string Salesman1Name { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string ContractTargetName { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string SiteName { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string ContractOfficeCode { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string OperationOfficeCode { get; set; }
    }
}
