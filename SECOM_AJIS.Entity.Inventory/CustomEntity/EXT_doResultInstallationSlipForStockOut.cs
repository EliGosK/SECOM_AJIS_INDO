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
    [MetadataType(typeof(doResultInstallationSlipForStockOut_Meta))]
    public partial class doResultInstallationSlipForStockOut
    {
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

        public string CustFullName { get; set; }
        public string InstallationTypeName { get; set; }
        public string OfficeName { get; set; }
        public string SiteName { get; set; }

        public string CustFullNameDisplay
        {
            get 
            {
                return string.Format("(1) {0}<br>(2) {1}", this.CustFullName, this.SiteName);
            }
        }

        public string SlipIssueDateDisplay
        {
            get
            {
                return CommonUtil.TextDate(this.SlipIssueDate);
            }
        }

        public string ToJson 
        {
            get
            {
                return CommonUtil.CreateJsonString(this);
            }
        }

    }

}
namespace SECOM_AJIS.DataEntity.Inventory.MetaData
{
    public class doResultInstallationSlipForStockOut_Meta
    {
        [LanguageMapping]
        public string CustFullName { get; set; }
        [LanguageMapping]
        public string InstallationTypeName { get; set; }
        [LanguageMapping]
        public string OfficeName { get; set; }
        [LanguageMapping]
        public string SiteName { get; set; }
    }

}

