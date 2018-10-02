using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Contract.MetaData;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Contract
{
    [MetadataType(typeof(tbt_RentalSecurityBasic_MetaData))]
    public partial class tbt_RentalSecurityBasic
    {
        CommonUtil comUtil = new CommonUtil();

        public string ProductTypeCode { get; set; }
        public string PlannerName { get; set; }
        public string PlanCheckerName { get; set; }
        public string PlanApproverName { get; set; }

        public string ContractCodeShort
        {
            get
            {
                return comUtil.ConvertContractCode(ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string QuotationTargetCodeShort
        {
            get
            {
                return comUtil.ConvertQuotationTargetCode(QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string DocAuditResultName { get; set; }
        public string DocAuditResultCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.DocAuditResult, this.DocAuditResultName);
            }
        }
    }
}
namespace SECOM_AJIS.DataEntity.Contract.MetaData
{
    public class tbt_RentalSecurityBasic_MetaData
    {
        [EmployeeExist]
        [EmployeeMapping("PlannerName")]
        public string PlannerEmpNo { get; set; }
        [EmployeeExist]
        [EmployeeMapping("PlanCheckerName")]
        public string PlanCheckerEmpNo { get; set; }
        [EmployeeExist]
        [EmployeeMapping("PlanApproverName")]
        public string PlanApproverEmpNo { get; set; }

        [DocAuditResultMapping("DocAuditResultName")]
        public string DocAuditResult { get; set; }
    }
}
