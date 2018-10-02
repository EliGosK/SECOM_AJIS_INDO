using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Quotation.MetaData;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Master;

namespace SECOM_AJIS.DataEntity.Quotation
{
    [MetadataType(typeof(doQuotationTarget_MetaData))]
    public partial class doQuotationTarget
    {
        public string QuotationTargetCodeShort
        {
            get
            {
                SECOM_AJIS.Common.Util.CommonUtil cmm = new SECOM_AJIS.Common.Util.CommonUtil();
                return cmm.ConvertQuotationTargetCode(this.QuotationTargetCode, SECOM_AJIS.Common.Util.CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }
        public string OldContractCodeShort
        {
            get
            {
                SECOM_AJIS.Common.Util.CommonUtil cmm = new SECOM_AJIS.Common.Util.CommonUtil();
                return cmm.ConvertContractCode(this.OldContractCode, SECOM_AJIS.Common.Util.CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string ProductTypeCodeName
        {
            get
            {
                return SECOM_AJIS.Common.Util.CommonUtil.TextCodeName(this.ProductTypeCode, this.ProductTypeName);
            }
        }
        public string QuotationOfficeCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.QuotationOfficeCode, this.QuotationOfficeName);
            }
        }
        public string OperationOfficeCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.OperationOfficeCode, this.OperationOfficeName);
            }
        }
        public string AcquisitionTypeCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.AcquisitionTypeCode, this.AcquisitionTypeName);
            }
        }
        public string MotivationTypeCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.MotivationTypeCode, this.MotivationTypeName);
            }
        }
        public string QuotationStaffCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.QuotationStaffEmpNo, this.QuotationStaffName);
            }
        }
    }
}
namespace SECOM_AJIS.DataEntity.Quotation.MetaData
{
    public class doQuotationTarget_MetaData
    {
        [LanguageMapping]
        public string ProductTypeName { get; set; }
        [LanguageMapping]
        public string ProvideServiceName { get; set; }
        [LanguageMapping]
        public string QuotationOfficeName { get; set; }
        [LanguageMapping]
        public string OperationOfficeName { get; set; }
        [LanguageMapping]
        public string AcquisitionTypeName { get; set; }
        [LanguageMapping]
        public string MotivationTypeName { get; set; }
        [LanguageMapping]
        public string QuotationStaffName { get; set; }

        [OfficeMapping("QuotationOfficeName")]
        public string QuotationOfficeCode { get; set; }
        [OfficeMapping("OperationOfficeName")]
        public string OperationOfficeCode { get; set; }
        [AcquisitionTypeMapping("AcquisitionTypeName")]
        public string AcquisitionTypeCode { get; set; }
        [MotivationTypeMapping("MotivationTypeName")]
        public string MotivationTypeCode { get; set; }
        [EmployeeMapping("QuotationStaffName")]
        public string QuotationStaffEmpNo { get; set; }
    }
}
