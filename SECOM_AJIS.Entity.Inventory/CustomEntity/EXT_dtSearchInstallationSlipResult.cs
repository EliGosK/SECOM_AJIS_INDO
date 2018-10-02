using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Inventory.MetaData;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Inventory.MetaData;


namespace SECOM_AJIS.DataEntity.Inventory
{
    [MetadataType(typeof(dtSearchInstallationSlipResult_Meta))]
    public partial class dtSearchInstallationSlipResult
    {
        public string OfficeName { get; set; }
        public string SubContractorName { get; set; }

        private string _ContractCode = null;
        private string _ContractCodeShort = null;

        public string ContractCodeShort
        {
            get
            {
                if (_ContractCode != this.ContractCode)
                {
                    _ContractCode = this.ContractCode;
                    _ContractCodeShort = (new CommonUtil().ConvertContractCode(this.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT));
                }
                return _ContractCodeShort;
            }
        }

        public string ExpectedDateAndContractCode
        {
            get
            {
                return string.Format("(1){0}<br>(2){1}", CommonUtil.TextDate(this.SlipIssueDate), this.ContractCodeShort);
            }
        }

        public string ContractTargetAndPurchaserName
        {
            get
            {
                return string.Format("(1){0}<br>(2){1}", this.CustFullNameEN, this.CustFullNameLC);
            }
        }

        public string SiteName
        {
            get
            {
                return string.Format("(1){0}<br>(2){1}", this.SiteNameEN, this.SiteNameLC);
            }
        }

        public string OperationOfficeAndSubContractor
        {
            get
            {
                return string.Format("(1){0}<br>(2){1}", this.OfficeName, this.SubContractorName);
            }
        }

    }

}

namespace SECOM_AJIS.DataEntity.Inventory.MetaData
{
    public class dtSearchInstallationSlipResult_Meta
    {
        [LanguageMapping]
        public string OfficeName { get; set; }
        [LanguageMapping]
        public string SubContractorName { get; set; }
    }

}
