using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Contract.MetaData;

using SECOM_AJIS.Common.Models.EmailTemplates;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class doChangePlanBeforeStartEmail : ATemplateObject
    {
        public string ContractCode { get; set; }
        public string ContractCodeDisplay
        {
            get
            {
                return new CommonUtil().ConvertContractCode(this.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }
        public string CustCode { get; set; }
        public string CustCodeDisplay
        {
            get
            {
                return new CommonUtil().ConvertCustCode(this.CustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }
        public string CustName { get; set; }
        public string SiteCode { get; set; }
        public string SiteCodeDisplay
        {
            get
            {
                return new CommonUtil().ConvertSiteCode(this.SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }
        public string SiteName { get; set; }
        public string OperationOfficeCode { get; set; }
        public string OperationOfficeName { get; set; }
        public DateTime? CurrentDate { get; set; }
        public string CurrentDateDisplay
        {
            get
            {
                return CommonUtil.TextDate(this.CurrentDate);
            }
        }
        public string CreateBy { get; set; }
    }
}
