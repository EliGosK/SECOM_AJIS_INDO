using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Quotation.MetaData;

namespace SECOM_AJIS.DataEntity.Quotation
{
    [MetadataType(typeof(tbt_QuotationBasic_MetaData))]
    public partial class tbt_QuotationBasic
    {
        public string QuotationTargetCodeShort
        {
            get
            {
                CommonUtil cmm = new CommonUtil();
                return cmm.ConvertQuotationTargetCode(this.QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }
        public string QuotationTargetCodeFull
        {
            get
            {
                return CommonUtil.TextCodeName(this.QuotationTargetCodeShort,this.Alphabet, "-");
            }
        }

        public string ProductName { get; set; }
        public string ProductCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.ProductCode, this.ProductName);
            }
        }

        public string PlannerName { get; set; }
        public string PlannerCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.PlannerEmpNo, this.PlannerName);
            }
        }
        public string PlanCheckerName { get; set; }
        public string PlanCheckerCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.PlanCheckerEmpNo, this.PlanCheckerName);
            }
        }
        public string PlanApproverName { get; set; }
        public string PlanApproverCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.PlanApproverEmpNo, this.PlanApproverName);
            }
        }
        public string SalesmanEmpNameNo1 { get; set; }
        public string SalesmanEmpCodeNameNo1
        {
            get
            {
                return CommonUtil.TextCodeName(this.SalesmanEmpNo1, this.SalesmanEmpNameNo1);
            }
        }
        public string SalesmanEmpNameNo2 { get; set; }
        public string SalesmanEmpCodeNameNo2
        {
            get
            {
                return CommonUtil.TextCodeName(this.SalesmanEmpNo2, this.SalesmanEmpNameNo2);
            }
        }
        public string SalesmanEmpNameNo3 { get; set; }
        public string SalesmanEmpCodeNameNo3
        {
            get
            {
                return CommonUtil.TextCodeName(this.SalesmanEmpNo3, this.SalesmanEmpNameNo3);
            }
        }
        public string SalesmanEmpNameNo4 { get; set; }
        public string SalesmanEmpCodeNameNo4
        {
            get
            {
                return CommonUtil.TextCodeName(this.SalesmanEmpNo4, this.SalesmanEmpNameNo4);
            }
        }
        public string SalesmanEmpNameNo5 { get; set; }
        public string SalesmanEmpCodeNameNo5
        {
            get
            {
                return CommonUtil.TextCodeName(this.SalesmanEmpNo5, this.SalesmanEmpNameNo5);
            }
        }
        public string SalesmanEmpNameNo6 { get; set; }
        public string SalesmanEmpCodeNameNo6
        {
            get
            {
                return CommonUtil.TextCodeName(this.SalesmanEmpNo6, this.SalesmanEmpNameNo6);
            }
        }
        public string SalesmanEmpNameNo7 { get; set; }
        public string SalesmanEmpCodeNameNo7
        {
            get
            {
                return CommonUtil.TextCodeName(this.SalesmanEmpNo7, this.SalesmanEmpNameNo7);
            }
        }
        public string SalesmanEmpNameNo8 { get; set; }
        public string SalesmanEmpCodeNameNo8
        {
            get
            {
                return CommonUtil.TextCodeName(this.SalesmanEmpNo8, this.SalesmanEmpNameNo8);
            }
        }
        public string SalesmanEmpNameNo9 { get; set; }
        public string SalesmanEmpCodeNameNo9
        {
            get
            {
                return CommonUtil.TextCodeName(this.SalesmanEmpNo9, this.SalesmanEmpNameNo9);
            }
        }
        public string SalesmanEmpNameNo10 { get; set; }
        public string SalesmanEmpCodeNameNo10
        {
            get
            {
                return CommonUtil.TextCodeName(this.SalesmanEmpNo10, this.SalesmanEmpNameNo10);
            }
        }
        public string SalesSupporterEmpName { get; set; }
        public string SalesSupporterEmpCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.SalesSupporterEmpNo, this.SalesSupporterEmpName);
            }
        }

        public string MainStructureTypeName { get; set; }
        public string MainStructureTypeCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.MainStructureTypeCode, this.MainStructureTypeName);
            }
        }

        public string BuildingTypeName { get; set; }
        public string BuildingTypeCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.BuildingTypeCode, this.BuildingTypeName);
            }
        }

        public string DispatchTypeName { get; set; }
        public string DispatchTypeCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.DispatchTypeCode, this.DispatchTypeName);
            }
        }
        
        public string ContractTransferStatusName { get; set; }
        public string ContractTransferStatusCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.ContractTransferStatus, this.ContractTransferStatusName);
            }
        }

        public string PhoneLineTypeName1 { get; set; }
        public string PhoneLineTypeCodeName1
        {
            get
            {
                return CommonUtil.TextCodeName(this.PhoneLineTypeCode1, this.PhoneLineTypeName1);
            }
        }

        public string PhoneLineTypeName2 { get; set; }
        public string PhoneLineTypeCodeName2
        {
            get
            {
                return CommonUtil.TextCodeName(this.PhoneLineTypeCode2, this.PhoneLineTypeName2);
            }
        }

        public string PhoneLineTypeName3 { get; set; }
        public string PhoneLineTypeCodeName3
        {
            get
            {
                return CommonUtil.TextCodeName(this.PhoneLineTypeCode3, this.PhoneLineTypeName3);
            }
        }

        public string PhoneLineOwnerTypeName1 { get; set; }
        public string PhoneLineOwnerTypeCodeName1
        {
            get
            {
                return CommonUtil.TextCodeName(this.PhoneLineOwnerTypeCode1, this.PhoneLineOwnerTypeName1);
            }
        }

        public string PhoneLineOwnerTypeName2 { get; set; }
        public string PhoneLineOwnerTypeCodeName2
        {
            get
            {
                return CommonUtil.TextCodeName(this.PhoneLineOwnerTypeCode2, this.PhoneLineOwnerTypeName2);
            }
        }

        public string PhoneLineOwnerTypeName3 { get; set; }
        public string PhoneLineOwnerTypeCodeName3
        {
            get
            {
                return CommonUtil.TextCodeName(this.PhoneLineOwnerTypeCode3, this.PhoneLineOwnerTypeName3);
            }
        }

        public string InsuranceTypeName { get; set; }
        public string InsuranceTypeCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.InsuranceTypeCode, this.InsuranceTypeName);
            }
        }

        public string SentryGuardAreaTypeName { get; set; }
        public string SentryGuardAreaTypeCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.SentryGuardAreaTypeCode, this.SentryGuardAreaTypeName);
            }
        }

        public string MaintenanceTargetProductTypeName { get; set; }
        public string MaintenanceTargetProductTypeCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.MaintenanceTargetProductTypeCode, this.MaintenanceTargetProductTypeName);
            }
        }

        public string MaintenanceTypeName { get; set; }
        public string MaintenanceTypeCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.MaintenanceTypeCode, this.MaintenanceTypeName);
            }
        }

        public string MaintenanceCycleName { get; set; }

        public string SaleOnlineContractCodeShort
        {
            get
            {
                CommonUtil cmm = new CommonUtil();
                return cmm.ConvertContractCode(this.SaleOnlineContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }
    }

    [MetadataType(typeof(tbt_QuotationBasic_MetaData))]
    public class InsertQuotationBasicCondition : tbt_QuotationBasic
    {
    }
}
namespace SECOM_AJIS.DataEntity.Quotation.MetaData
{
    public class tbt_QuotationBasic_MetaData
    {
        [EmployeeMapping("PlannerName")]
        public string PlannerEmpNo { get; set; }
        [EmployeeMapping("PlanCheckerName")]
        public string PlanCheckerEmpNo { get; set; }
        [EmployeeMapping("PlanApproverName")]
        public string PlanApproverEmpNo { get; set; }
        [EmployeeMapping("SalesmanEmpNameNo1")]
        public string SalesmanEmpNo1 { get; set; }
        [EmployeeMapping("SalesmanEmpNameNo2")]
        public string SalesmanEmpNo2 { get; set; }
        [EmployeeMapping("SalesmanEmpNameNo3")]
        public string SalesmanEmpNo3 { get; set; }
        [EmployeeMapping("SalesmanEmpNameNo4")]
        public string SalesmanEmpNo4 { get; set; }
        [EmployeeMapping("SalesmanEmpNameNo5")]
        public string SalesmanEmpNo5 { get; set; }
        [EmployeeMapping("SalesmanEmpNameNo6")]
        public string SalesmanEmpNo6 { get; set; }
        [EmployeeMapping("SalesmanEmpNameNo7")]
        public string SalesmanEmpNo7 { get; set; }
        [EmployeeMapping("SalesmanEmpNameNo8")]
        public string SalesmanEmpNo8 { get; set; }
        [EmployeeMapping("SalesmanEmpNameNo9")]
        public string SalesmanEmpNo9 { get; set; }
        [EmployeeMapping("SalesmanEmpNameNo10")]
        public string SalesmanEmpNo10 { get; set; }
        [EmployeeMapping("SalesSupporterEmpName")]
        public string SalesSupporterEmpNo { get; set; }

        [MainStructureTypeMapping("MainStructureTypeName")]
        public string MainStructureTypeCode { get; set; }
        [ContractTransferStatusMapping("ContractTransferStatusName")]
        public string ContractTransferStatus { get; set; }
        [BuildingTypeMapping("BuildingTypeName")]
        public string BuildingTypeCode { get; set; }
        [DispatchTypeMapping("DispatchTypeName")]
        public string DispatchTypeCode { get; set; }
        [PhoneLineTypeMapping("PhoneLineTypeName1")]
        public string PhoneLineTypeCode1 { get; set; }
        [PhoneLineTypeMapping("PhoneLineTypeName2")]
        public string PhoneLineTypeCode2 { get; set; }
        [PhoneLineTypeMapping("PhoneLineTypeName3")]
        public string PhoneLineTypeCode3 { get; set; }
        [PhoneLineOwnerTypeMapping("PhoneLineOwnerTypeName1")]
        public string PhoneLineOwnerTypeCode1 { get; set; }
        [PhoneLineOwnerTypeMapping("PhoneLineOwnerTypeName2")]
        public string PhoneLineOwnerTypeCode2 { get; set; }
        [PhoneLineOwnerTypeMapping("PhoneLineOwnerTypeName3")]
        public string PhoneLineOwnerTypeCode3 { get; set; }
        [InsuranceTypeMapping("InsuranceTypeName")]
        public string InsuranceTypeCode { get; set; }
        [SentryGuardAreaTypeMapping("SentryGuardAreaTypeName")]
        public string SentryGuardAreaTypeCode { get; set; }
        [MaintenanceTargetProductTypeMapping("MaintenanceTargetProductTypeName")]
        public string MaintenanceTargetProductTypeCode { get; set; }
        [MaintenanceTypeMapping("MaintenanceTypeName")]
        public string MaintenanceTypeCode { get; set; }
        [MaintenanceCycleMapping("MaintenanceCycleName")]
        public Nullable<int> MaintenanceCycle { get; set; }

        [MaxTextLength(20)]
        public string PlanCode { get; set; }
        [MaxTextLength(13)]
        public string SecurityTypeCode { get; set; }
        [MaxTextLength(500)]
        public string FacilityMemo { get; set; }
        [MaxTextLength(500)]
        public string MaintenanceMemo { get; set; }

        [MaxTextLength(15)]
        public string ApproveNo1 { get; set; }
        [MaxTextLength(15)]
        public string ApproveNo2 { get; set; }
        [MaxTextLength(15)]
        public string ApproveNo3 { get; set; }
        [MaxTextLength(15)]
        public string ApproveNo4 { get; set; }
        [MaxTextLength(15)]
        public string ApproveNo5 { get; set; }

        [MaxTextLength(15)]
        public string AdditionalApproveNo1 { get; set; }
        [MaxTextLength(15)]
        public string AdditionalApproveNo2 { get; set; }
        [MaxTextLength(15)]
        public string AdditionalApproveNo3 { get; set; }
    }
    public class InsertQuotationBasicCondition_MetaData
    {
        [NotNullOrEmpty]
        public string Alphabet { get; set; }
        [NotNullOrEmpty]
        public string QuotationTargetCode { get; set; }
    }
}
