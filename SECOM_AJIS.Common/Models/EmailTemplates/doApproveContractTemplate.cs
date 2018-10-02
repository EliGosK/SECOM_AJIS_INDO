using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Common.Models.EmailTemplates
{
    /// <summary>
    /// DO for approve contract email
    /// </summary>
    public class doApproveContractEmailObject : ATemplateObject
    {
        public string ContractCode { get; set; }
        public string CustFullNameLC { get; set; }
        public string CustFullNameEN { get; set; }
        public string SiteNameLC { get; set; }
        public string SiteNameEN { get; set; }
        public string ProductNameLC { get; set; }
        public string ProductNameEN { get; set; }
        public string SalesmanEmpNameLCNo1 { get; set; }
        public string SalesmanEmpNameLCNo2 { get; set; }
        public string SalesmanEmpNameENNo1 { get; set; }
        public string SalesmanEmpNameENNo2 { get; set; }
        public string ContractOfficeNameLC { get; set; }
        public string ContractOfficeNameEN { get; set; }
        public string OperationOfficeNameEN { get; set; }
        public string OperationOfficeNameLC { get; set; }
    }
}
