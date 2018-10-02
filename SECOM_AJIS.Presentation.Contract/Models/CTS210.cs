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
using SECOM_AJIS.Common.Util.ConstantValue;
using System.IO;

namespace SECOM_AJIS.Presentation.Contract.Models
{
    /// <summary>
    /// Parameter of CTS210 screen
    /// </summary>
    public class CTS210_ScreenParameter : ScreenParameter
    {
        public dsContractDocForIssue dsContractDoc { get; set; }
        public Stream StreamReport { get; set; }
    }

    /// <summary>
    /// Parameter for specify ContractCode condition
    /// </summary>
    public class CTS210_SpecifyContractCodeCondition
    { 
        public string ContractCode { get; set; }
        public string OCC { get; set; }
    }

    /// <summary>
    /// DO of DocumentList
    /// </summary>
    public class CTS210_DocumentListGridData : dtContractDoc
    {
        private string _generateDate = string.Empty;
        public string GenerateDate 
        {
            get
            {
                if (CreateDate != null)
                    _generateDate = string.Format("<div id='{0}'>", this.CreateDate.Value.ToString("yyyyMMdd")) + CommonUtil.TextDate(this.CreateDate) + "</div>";

                return _generateDate;
            }
        }

        private string _generateDateDisplay = string.Empty;
        public string GenerateDateDisplay
        {
            get
            {
                if (CreateDate != null)
                    _generateDateDisplay = CommonUtil.TextDate(this.CreateDate);

                return _generateDateDisplay;
            }
        }

        private string _occAlphabet = string.Empty;
		public string OccAlphabet 
        {
            get
            {
                if (String.IsNullOrEmpty(OCC) == false)
                    _occAlphabet = OCC;
                else
                    _occAlphabet = Alphabet;

                return _occAlphabet;
            }        
        }

        private string _issued = string.Empty;
        public string Issued 
        {
            get
            {
                if (IssuedDate != null)
                    _issued = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, ScreenID.C_SCREEN_ID_ISSUE_CONTRACT_DOCUMENT, "lblIssued");
                else
                    _issued = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, ScreenID.C_SCREEN_ID_ISSUE_CONTRACT_DOCUMENT, "lblNotIssued");

                return _issued;
            }        
        }

        public bool IsEnableDownload { get; set; }
     }

    /// <summary>
    /// DO for validate ContractDocument
    /// </summary>
    [MetadataType(typeof(CTS210_ValidateContractCondition_MetaData))]
    public class CTS210_ValidateContractCondition : tbt_ContractDocument
    {

    }
}

namespace SECOM_AJIS.Presentation.Contract.Models.MetaData
{
    public class CTS210_ValidateContractCondition_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
               Screen = "CTS210",
               Parameter = "lblContractQuotationTargetCode",
               ControlName = "txtContractQuotTgtCode")]
        public int ContractCode { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
               Screen = "CTS210",
               Parameter = "lblOccAlphabet",
               ControlName = "txtOccAlphabet")]
        public int OCC { get; set; }
    }
}

