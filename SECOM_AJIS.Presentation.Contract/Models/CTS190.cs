using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Quotation;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Presentation.Contract.Models.MetaData;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.Presentation.Contract.Models
{
    /// <summary>
    /// DO for initial screen and validate condition
    /// </summary>
    [MetadataType(typeof(CTS190_ScreenParameter_MetaData))]
    public class CTS190_ScreenParameter : ScreenParameter
    {
        public string ContractCode_QuotationTargetCode { set; get; }
        public string OCC_Alphabet { set; get; }
        public string ContractDocOCC { set; get; }
        public string DocAuditResult { set; get; }
    }
}

namespace SECOM_AJIS.Presentation.Contract.Models.MetaData
{
    /// <summary>
    /// Metadata for CTS190_ScreenParameter DO
    /// </summary>
    public class CTS190_ScreenParameter_MetaData
    {

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                     Screen = "CTS190",
                     Parameter = "lblContractCodeQuotationTargetCode",
                     ControlName = "ContractCode_QuotationTargetCode")]
        public string ContractCode_QuotationTargetCode { get; set; }


        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                     Screen = "CTS190",
                     Parameter = "lblOCC_Alphabet",
                     ControlName = "OCC_Alphabet")]
        public string OCC_Alphabet { get; set; }


        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                     Screen = "CTS190",
                     Parameter = "lblDocAuditResult",
                     ControlName = "DocAuditResult")]
        public string DocAuditResult { get; set; }

        
    }
}
