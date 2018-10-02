using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Contract
{
   public class View_dtSearchDraftContractResult:dtSearchDraftContractResult
    {
       CommonUtil c = new CommonUtil();

        //Quatation Code Column
        public string QuatationCodeShow
        {
           get
           {
               return c.ConvertQuotationTargetCode(this.QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT) + ":" + this.Alphabet;
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
            get { return "(1) " + this.ContractTargetCode + "<br/>" + "(2) " + this.ContractTargetNameEN + "<br/>" + "(3) " + this.ContractTargetNameLC; }
        }
        
        //Site Column
        public string SiteShow
        {
            get { return "(1) " + this.SiteCode + "<br/>" + "(2) " + this.SiteNameEN + "<br/>" + "(3) " + this.SiteNameLC; }
        }

        //Office Column
        public string ContractOfficeName { get; set; }
        public string OperationOfficeName { get; set; }
        public string OfficeShow
        {
            get { return "(1) " + this.ContractOfficeName + "<br/>" + "(2) " + this.OperationOfficeName; }
        }

        //Salesman1 Column
        public string Salesman1Name { get; set; }
        public string Salesman1Show
        {
            get { return "(1) " + this.Salesman1Code + "<br/>" + "(2) " + this.Salesman1Name; }
        }

        //Approve status Column
        public string ApprovalStatusName { get; set; }

        //Contract code column
        public string ContractCodeShow
        {
            get
            {
                if(String.Empty.Equals(this.ContractCode))
                    return "-";
                else
                    return c.ConvertContractCode(this.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string RegisterDateShow
        {
            get { return this.RegisterDate.Value.ToString("dd-MMM-yyyy"); }
        }

        public string ApproveContractDateShow
        {
            get { return this.ApproveContractDate.Value.ToString("dd-MMM-yyyy"); }
        }

        //public string ToJson
        //{
        //    get
        //    {
        //        return Common.Util.CommonUtil.CreateJsonString<View_tbm_Site>(this);
        //    }
        //}
    }
}
