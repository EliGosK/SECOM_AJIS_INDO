using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class doSearchProjectCondition
    {
		public string ProjectCode {get; set;}
        public string ContractCode {get; set;}
        public string ProjectName {get; set;}
        public string ProjectAddress {get; set;}
        public string PJPurchaseName {get; set;}
        public string Owner1Name {get; set;}
        public string PJManagementCompanyName {get; set;}
        public string OtherProjectRelatedPersonName {get; set;}
        public string ProductCode {get; set;}
        public string HeadSalesmanEmpName {get; set;}
        public string ProjectManagerEmpName { get; set; }
    }
}
