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
    // Using by Narupon
    partial class InventoryHandler : BizIVDataEntities, IInventoryHandler
    {
        public List<dtCheckingDetailList> GetCheckingDetailList(doGetCheckingDetailList cond)
        {
            try
            {
                List<dtCheckingDetailList> list = base.GetCheckingDetailList(cond.CheckingYearMonth, cond.OfficeCode, cond.LocationCode, cond.AreaCode, MiscType.C_INV_AREA);
                CommonUtil.MappingObjectLanguage<dtCheckingDetailList>(list);
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<dtStockCheckingList> GetStockCheckingList(doGetStockCheckingList cond)
        {
            try
            {
                List<dtStockCheckingList> list = base.GetStockCheckingList(cond.AreaCode, cond.CheckingYearMonth, cond.InstrumentCode, cond.InstrumentName, cond.LocationCode, cond.OfficeCode, cond.ShelfNoFrom, cond.ShelfNoTo, MiscType.C_INV_AREA, MiscType.C_INV_LOC);
                CommonUtil.MappingObjectLanguage<dtStockCheckingList>(list);
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
    }
}
