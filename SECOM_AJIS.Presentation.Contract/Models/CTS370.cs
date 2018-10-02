using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Contract;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Contract.MetaData;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.Presentation.Contract.Models
{
    /// <summary>
    /// Parameter of screen
    /// </summary>
    public class CTS370_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        public string strARRelevantType { get; set; }

        [KeepSession]
        public string strARRelevantCode { get; set; }

        public string strXmlResultAR { get; set; }
        public string strXmlResultOcc { get; set; }
    }

    /// <summary>
    /// Grid header result
    /// </summary>
    public class CTS370_GridHeader
    {
        public string Code { get; set; }
        [LanguageMapping]
        public string Name { get; set; }

        public string NameEN { get; set; }
        public string NameLC { get; set; }
        public string NameJP { get; set; }
    }

    /// <summary>
    /// Retrieve code from screen
    /// </summary>
    public class CTS370_GetCodeCondition : ScreenParameter
    {
        public string SiteCode { get; set; }
        public string ContractCode { get; set; }
    }

    /// <summary>
    /// Condition for retrieve AR list
    /// </summary>
    public class CTS370_ARCondition : ScreenParameter
    {
        //public bool isInitial { get; set; }
        //public string GRIDNAME { get; set; }
        public string ARMode { get; set; }
        public string CustomerCode { get; set; }
        public string SiteCode { get; set; }
        public string ContractCode { get; set; }
        public string ProjectCode { get; set; }
        public string QuotationCode { get; set; }
        public string ARType { get; set; }
        public string DuedateDeadline { get; set; }
        public string ARStatus { get; set; }
    }

    /// <summary>
    /// Search result
    /// </summary>
    public class CTS370_SearchResult
    {
        //public string GRIDNAME { get; set; }
        public string DatName { get; set; }
        public string DatCode { get; set; }
        public bool SearchResult { get; set; }
        public string SID { get; set; }
    }

    /// <summary>
    /// AR occurring site list result
    /// </summary>
    public class CTS370_AROccurringSiteGridResult
    {
        public string No { get; set; }
        public string SiteCode { get; set; }
        public string SiteNameEN { get; set; }
        public string SiteNameLC { get; set; }
        public string AR { get; set; }
        public string Delay { get; set; }
    }

    /// <summary>
    /// AR occurring contract list result
    /// </summary>
    public class CTS370_AROccurringContractGridResult
    {
        public string No { get; set; }
        public string ContractCode { get; set; }
        public string ContractNameEN { get; set; }
        public string ContractNameLC { get; set; }
        public string AR { get; set; }
        public string Delay { get; set; }
    }

    /// <summary>
    /// AR list result
    /// </summary>
    public class CTS370_ARGridResult
    {
        public string HeadCode { get; set; }
        public string HeadName { get; set; }
        public string ARStatus { get; set; }
        public string ARFlag { get; set; }
        public string ARID { get; set; }
        public string ARNo { get; set; }
        public string DueDateDeadline { get; set; }
        public string ARType { get; set; }
        public string ApproverName { get; set; }
        public string RequesterName { get; set; }
        public string RegisterDate { get; set; }
        public string CompleteDate { get; set; }
        public string Status { get; set; }
    }

    /// <summary>
    /// Default value for screen
    /// </summary>
    public class CTS370_DefaultValue
    {
        public string DueDate { get; set; }
        public string Status { get; set; }
    }
}
