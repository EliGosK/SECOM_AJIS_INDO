using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Inventory
{
    public partial class doCSVassetAmountAcc
    {
        [CSVMapping(HeaderName = "AccountCode", SequenceNo = 1, ValueOutputFormat = CSVMappingAttribute.eValueOutputFormat.Raw)]
        public string AccountCodeCsv
        {
            get { return this.AccountCode; }
        }

        [CSVMapping(HeaderName = "AreaCode", SequenceNo = 2, ValueOutputFormat = CSVMappingAttribute.eValueOutputFormat.Raw)]
        public string AreaCodeCsv
        {
            get { return this.AreaCode; }
        }

        [CSVMapping(HeaderName = "TotalSum", SequenceNo = 3, ValueOutputFormat = CSVMappingAttribute.eValueOutputFormat.Raw)]
        public Nullable<decimal> TotalSumCsv
        {
            get { return this.TotalSum; }
        }
    }
}
