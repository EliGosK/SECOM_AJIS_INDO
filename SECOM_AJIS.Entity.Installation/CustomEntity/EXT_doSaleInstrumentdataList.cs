using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Installation.MetaData;

namespace SECOM_AJIS.DataEntity.Installation
{
    public partial class doSaleInstrumentdataList
    {
        public string InstrumentName
        {
            get;
            set;
        }
        public decimal InstrumentPrice
        {
            get;
            set;
        }
    }
}