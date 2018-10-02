using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Contract.MetaData;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Contract
{
    [MetadataType(typeof(tbt_DraftSaleContract_MetaData))]
    public partial class tbt_DraftSaleContract
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

        public string ConnectTargetCodeShort
        {
            get
            {
                CommonUtil cmm = new CommonUtil();
                return cmm.ConvertContractCode(this.ConnectTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string ProductName { get; set; }

        public string SalesmanEmpNameNo1 { get; set; }
        public string SalesmanEmpNameNo2 { get; set; }
        public string SalesmanEmpNameNo3 { get; set; }
        public string SalesmanEmpNameNo4 { get; set; }
        public string SalesmanEmpNameNo5 { get; set; }
        public string SalesmanEmpNameNo6 { get; set; }
        public string SalesmanEmpNameNo7 { get; set; }
        public string SalesmanEmpNameNo8 { get; set; }
        public string SalesmanEmpNameNo9 { get; set; }
        public string SalesmanEmpNameNo10 { get; set; }
    }
}
namespace SECOM_AJIS.DataEntity.Contract.MetaData
{
    public class tbt_DraftSaleContract_MetaData
    {
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
    }
}
