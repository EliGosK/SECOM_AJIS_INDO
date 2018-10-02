using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Inventory
{
    public partial class doCSVOtherFinancialAcc
    {

        [CSVMapping(HeaderName = "SlipNo", SequenceNo = 1, ValueOutputFormat = CSVMappingAttribute.eValueOutputFormat.Raw)]
        public string SlipNoCsv
        {
            get { return this.SlipNo; }
        }

        [CSVMapping(HeaderName = "DestinationAreaCode", SequenceNo = 2, ValueOutputFormat = CSVMappingAttribute.eValueOutputFormat.Raw)]
        public string DestinationAreaCodeCsv
        {
            get { return this.DestinationAreaCode; }
        }

        [CSVMapping(HeaderName = "TransferTypeCode", SequenceNo = 3, ValueOutputFormat = CSVMappingAttribute.eValueOutputFormat.Raw)]
        public string TransferTypeCodeCsv
        {
            get { return this.TransferTypeCode; }
        }

        [CSVMapping(HeaderName = "Amount", SequenceNo = 4, ValueOutputFormat = CSVMappingAttribute.eValueOutputFormat.Raw)]
        public Nullable<decimal> AmountCsv
        {
            get { return this.Amount; }
        }

    }
}
