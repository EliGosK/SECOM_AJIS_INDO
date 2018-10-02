using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Master;

namespace SECOM_AJIS.DataEntity.Quotation
{
    public class doLinkageSaleContractData
    {
        public string ContractCode { get; set; }
        public string ContractCodeShort
        {
            get
            {
                CommonUtil cmm = new CommonUtil();
                return cmm.ConvertContractCode(this.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string PlanCode { get; set; }
        public bool? SpecialInstallationFlag { get; set; }
        public string SpecialInstallationFlagText
        {
            get
            {
                if (SpecialInstallationFlag == true)
                    return CommonUtil.GetLabelFromResource(MessageUtil.MODULE_QUOTATION, "QUS030", "rdoSpecialInstall_Yes");
                return CommonUtil.GetLabelFromResource(MessageUtil.MODULE_QUOTATION, "QUS030", "rdoSpecialInstall_No");
            }
        }

        [EmployeeMapping("PlannerName")]
        public string PlannerEmpNo { get; set; }
        public string PlannerName { get; set; }
        public string PlannerCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.PlannerEmpNo, this.PlannerName);
            }
        }

        [EmployeeMapping("PlanCheckerName")]
        public string PlanCheckerEmpNo { get; set; }
        public string PlanCheckerName { get; set; }
        public string PlanCheckerCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.PlanCheckerEmpNo, this.PlanCheckerName);
            }
        }
        public DateTime? PlanCheckDate { get; set; }

        [EmployeeMapping("PlanApproverName")]
        public string PlanApproverEmpNo { get; set; }
        public string PlanApproverName { get; set; }
        public string PlanApproverCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.PlanApproverEmpNo, this.PlanApproverName);
            }
        }
        public DateTime? PlanApproveDate { get; set; }

        public decimal? SiteBuildingArea { get; set; }
        public decimal? SecurityAreaFrom { get; set; }
        public decimal? SecurityAreaTo {get; set;}

        [MainStructureTypeMapping("MainStructureTypeName")]
        public string MainStructureTypeCode { get; set; }
        public string MainStructureTypeName { get; set; }
        public string MainStructureTypeCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.MainStructureTypeCode, this.MainStructureTypeName);
            }
        }

        [BuildingTypeMapping("BuildingTypeName")]
        public string BuildingTypeCode { get; set; }
        public string BuildingTypeName { get; set; }
        public string BuildingTypeCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.BuildingTypeCode, this.BuildingTypeName);
            }
        }

        public Nullable<bool> NewBldMgmtFlag {get;set;}
        public string NewBldMgmtFlagText
        {
            get
            {
                if (NewBldMgmtFlag == true)
                    return CommonUtil.GetLabelFromResource(MessageUtil.MODULE_QUOTATION, "QUS030", "rdoNewBuildingMgmtTypeFlagNeed");
                return CommonUtil.GetLabelFromResource(MessageUtil.MODULE_QUOTATION, "QUS030", "rdoNewBuildingMgmtTypeFlagNoNeed");
            }
        }

        public Nullable<decimal> NewBldMgmtCost {get;set;}
        public String NewBldMgmtCostCurrencyType { get; set; }

        public List<doInstrumentDetail> SaleInstrumentDetailList { get; set; }
    }
}
