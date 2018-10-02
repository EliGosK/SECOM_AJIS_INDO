using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Inventory
{
    public partial class doCSVMovingAssetAcc
    {

        [CSVMapping(HeaderName = "SlipNo", SequenceNo = 1, ValueOutputFormat = CSVMappingAttribute.eValueOutputFormat.Raw)]
        public string SlipNoCsv
        {
            get { return this.SlipNo; }
        }

        [CSVMapping(HeaderName = "SourceAccountCode", SequenceNo = 2, ValueOutputFormat = CSVMappingAttribute.eValueOutputFormat.Raw)]
        public string SourceAccountCodeCsv
        {
            get { return this.SourceAccountCode; }
        }

        [CSVMapping(HeaderName = "DestinationAccountCode", SequenceNo = 3, ValueOutputFormat = CSVMappingAttribute.eValueOutputFormat.Raw)]
        public string DestinationAccountCodeCsv
        {
            get { return this.DestinationAccountCode; }
        }

        [CSVMapping(HeaderName = "Amount", SequenceNo = 4, ValueOutputFormat = CSVMappingAttribute.eValueOutputFormat.Raw)]
        public Nullable<decimal> AmountCsv
        {
            get { return this.Amount; }
        }

    }
}
