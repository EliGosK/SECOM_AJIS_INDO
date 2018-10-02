using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Master;
using CSI.WindsorHelper;
using System.Transactions;


namespace SECOM_AJIS.DataEntity.Inventory
{
    // Using by Natthavat
    partial class InventoryHandler : BizIVDataEntities, IInventoryHandler
    {
        public List<doResultIVS200> GetIVS200(doGetIVS200 cond)
        {
            return base.GetIVS200(cond.OfficeCode
                , InstrumentLocation.C_INV_LOC_INSTOCK
                , InstrumentArea.C_INV_AREA_NEW_SAMPLE
                , InstrumentArea.C_INV_AREA_NEW_RENTAL
                , InstrumentArea.C_INV_AREA_NEW_SALE
                , cond.InstrumentCode
                , cond.InstrumentName
                , ConfigName.C_CONFIG_WILDCARD
                , cond.HaveOrder
                , cond.BelowSafety
                , cond.Minus);
        }

        public List<doInventoryBookingDetail> GetIVS200_Detail(doGetIVS200_Detail cond)
        {
            var result = base.GetTbtInventoryBookingDetailForIV200(cond.InstrumentCode);
            CommonUtil.MappingObjectLanguage(result);
            return result;
        }
    }
}
