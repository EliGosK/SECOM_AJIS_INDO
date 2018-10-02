using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Common.Util.ConstantValue;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using System.Text.RegularExpressions;


namespace SECOM_AJIS.DataEntity.Master
{
    public class SupplierMasterHandler : BizMADataEntities, ISupplierMasterHandler
    {

        public tbm_Supplier GetSupplier(string strSupplierCode, string strSupplierName)
        {
            try
            {
                List<tbm_Supplier> obj = base.GetSupplier(strSupplierCode, strSupplierName);
                if (obj.Count > 0)
                {
                    CommonUtil.MappingObjectLanguage(obj);
                    return obj[0];
                }
                return new tbm_Supplier();

            }
            catch (Exception)
            {
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
            }


        }

    }
}