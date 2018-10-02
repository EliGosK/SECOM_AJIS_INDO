﻿using System;
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
    /// DO for inital screen
    /// </summary>
    public class CTS040_ScreenParameter : ScreenParameter
    {
    }
    //public class CTS040_CheckReqField
    //{
    //    //[RelateObject("QuotationCode", ControlName = "Alphabet", RelateControlName = "QuotationCode")]
    //    //public string Alphabet { get; set; }
    //    //public string QuotationCode { get; set; }

    //    //[CodeNotNullOtherNotNull("Alphabet", Controller= MessageUtil.MODULE_CONTRACT, Screen = "CTS040", Parameter MessageCode = MessageUtil.MessageList.MSG0007, ControlName = "QuotationCode")]
    //    //public string QuotationCode { get; set; }
    //    //public string Alphabet { get; set; }
    //}
    /// <summary>
    /// DO for validate condition
    /// </summary>
    [MetadataType(typeof(CTS040_Search_MetaData))]
    public class CTS040_Search : doSearchDraftContractCondition
    {
    }
}

namespace SECOM_AJIS.Presentation.Contract.Models.MetaData
{
    /// <summary>
    /// Metadata for CTS040_Search DO
    /// </summary>
    public class CTS040_Search_MetaData
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
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string ApproveContractStatus { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        //[RelateObject("ApproveDateTo", ControlName = "ApproveDateFrom", RelateControlName = "ApproveDateTo")]
        public DateTime? ApproveDateFrom { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public DateTime? ApproveDateTo { get; set; }
    }
}