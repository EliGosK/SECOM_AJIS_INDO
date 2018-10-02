using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Contract.MetaData;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Contract
{
    [MetadataType(typeof(tbt_DraftRentalContract_MetaData))]
    public partial class tbt_DraftRentalContract
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
                return CommonUtil.TextCodeName(this.QuotationTargetCodeShort, this.Alphabet, "-");
            }
        }

        public string ContractCodeShort
        {
            get
            {
                CommonUtil cmm = new CommonUtil();
                return cmm.ConvertContractCode(this.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
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
        public string DispatchTypeName { get; set; }
        public string DispatchTypeCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.DispatchTypeCode, this.DispatchTypeName);
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

        public string SalesmanEmpNameNo1 { get; set; }
        public string SalesmanEmpNameNo2 { get; set; }
        public string SalesSupporterEmpName { get; set; }
    }
}
namespace SECOM_AJIS.DataEntity.Contract.MetaData
{
    public class tbt_DraftRentalContract_MetaData
    {
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

        [EmployeeMapping("SalesmanEmpNameNo1")]
        public string SalesmanEmpNo1 { get; set; }
        [EmployeeMapping("SalesmanEmpNameNo2")]
        public string SalesmanEmpNo2 { get; set; }
        [EmployeeMapping("SalesSupporterEmpName")]
        public string SalesSupporterEmpNo { get; set; }
    }
}
