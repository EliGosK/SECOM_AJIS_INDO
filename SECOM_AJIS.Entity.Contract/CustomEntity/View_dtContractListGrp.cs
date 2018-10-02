using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class View_dtContractListGrp : dtContractListGrp
    {
        public string FullAddressEN_LC
        {
            get { return "(1)" + " " + this.AddressFullEN + "<br/>" + "(2)" + " " + this.AddressFullLC; }
        }
        public string ContractNameEN_LC
        {
            get { return "(1)" + " " + this.CustNameEN + "<br/>" + "(2)" + " " + this.CustNameLC; }
        }
        public string BranchNameEN_LC
        {
            get { return "(1)" + " " + this.BranchNameEN + "<br/>" + "(2)" + " " + this.BranchNameLC; }
        }
    }
}
