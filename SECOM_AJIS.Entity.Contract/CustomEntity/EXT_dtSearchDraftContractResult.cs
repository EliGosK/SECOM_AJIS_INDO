using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.DataEntity.Contract
{
    public partial class dtSearchDraftContractResult
    {
        CommonUtil c = new CommonUtil();

        //Quatation Code Column
        public string QuatationCodeShow
        {
           get
           {
               return c.ConvertQuotationTargetCode(this.QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT) + "<br/>-" + this.Alphabet;
           }
        }

        //Quatation Target Code Column
        public string QuotationTargetCodeShow
        {
            get
            {
                return c.ConvertQuotationTargetCode(this.QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }


        //Contract Target/Purchaser Column
        public string ContractTargatShow
        {
            get { return "(1) " + c.ConvertCustCode(this.ContractTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT) + "<br/>" + "(2) " + this.ContractTargetNameEN + "<br/>" + "(3) " + this.ContractTargetNameLC; }
        }
        
        //Site Column
        public string SiteShow
        {
            get { return "(1) " + c.ConvertSiteCode(this.SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT) + "<br/>" + "(2) " + this.SiteNameEN + "<br/>" + "(3) " + this.SiteNameLC; }
        }

        //Office Column
        [LanguageMapping]
        public string ContractOfficeName { get; set; }
        [LanguageMapping]
        public string OperationOfficeName { get; set; }
        public string OfficeShow
        {
            get { return "(1) " + this.ContractOfficeName + "<br/>" + "(2) " + this.OperationOfficeName; }
        }

        //Salesman1 Column
        [LanguageMapping]
        public string Salesman1Name { get; set; }
        public string Salesman1Show
        {
            get { return "(1) " + this.Salesman1Code + "<br/>" + "(2) " + this.Salesman1Name; }
        }

        //Contract code column
        public string ContractCodeShow
        {
            get
            {
                //if(String.Empty.Equals(this.ContractCode))
                if(CommonUtil.IsNullOrEmpty(this.ContractCode))
                    return "-";
                else
                    return c.ConvertContractCode(this.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string RegisterDateShow
        {
            get 
            {
                return (RegisterDate.HasValue) ? this.RegisterDate.Value.ToString("dd-MMM-yyyy") : string.Empty;
            }
        }

        //Approve status Column  
        [LanguageMapping]
        public string ApprovalStatusName { get; set; } 

        public string ApproveContractDateShow
        {

            get
            {
                return (ApproveContractDate.HasValue) ? this.ApproveContractDate.Value.ToString("dd-MMM-yyyy") : string.Empty;
            }
        }

        //Register date FN-99/FQ-99 and CP-01 Column
        public string RegistrationDateFNFQCPShow
        {
            get { return "(1) " + RegisterDateShow + "<br/>" + "(2) " + ApproveContractDateShow; }
        }


        public string ServiceTypeCode
        {
            get
            {
                if (this.ProductTypeCodeName == "rental")
                    return ServiceType.C_SERVICE_TYPE_RENTAL;
                else
                    return ServiceType.C_SERVICE_TYPE_SALE;
            }
        }

        //public string ToJson
        //{
        //    get
        //    {
        //        return Common.Util.CommonUtil.CreateJsonString<View_tbm_Site>(this);
        //    }
        //}

        public string KeyIndex
        {
            get
            {
                return string.Format("{0};{1}", this.QuotationCode, this.Alphabet);
            }
        }
    }
}
