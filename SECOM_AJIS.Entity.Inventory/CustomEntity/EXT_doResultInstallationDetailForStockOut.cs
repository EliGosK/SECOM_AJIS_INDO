using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Inventory.MetaData;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Inventory.MetaData;

namespace SECOM_AJIS.DataEntity.Inventory
{
    [MetadataType(typeof(doResultInstallationDetailForStockOut_Meta))]
    public partial class doResultInstallationDetailForStockOut
    {

        public int NewInstSale { get; set; }
        public int NewInstSample { get; set; }
        public int NewInstRental { get; set; }
        public int SecondhandInstRental { get; set; }
        public string Remark { get; set; }

        public string ToJson
        {
            get
            {
                return CommonUtil.CreateJsonString(this);
            }
        }

    }

}
namespace SECOM_AJIS.DataEntity.Inventory.MetaData
{
    public class doResultInstallationDetailForStockOut_Meta
    {

    }

}

