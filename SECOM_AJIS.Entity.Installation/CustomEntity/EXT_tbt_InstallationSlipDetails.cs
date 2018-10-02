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

    public partial class tbt_InstallationSlipDetails
    {
        CommonUtil cm = new CommonUtil();
        public int? ContractInstallAfterChange
        {
            get { return (ContractInstalledQty + AddInstalledQty - AddRemovedQty); }
        }

        public string InstrumentName { get; set; }
        public string ParentCode { get; set; }
        public bool IsUnremovable { get; set; }
        public bool IsParent { get; set; }
        public decimal RentalUnitPrice
        {
            get;
            set;
        }   
    }
}


