using CSI.WindsorHelper;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM_AJIS.Presentation.Common
{
    // Create by Jirawat Jannet @ 2016-08-29
    public class MiscellaneousTypeUtil
    {
        /// <summary>
        /// Get currency name
        /// </summary>
        /// <param name="currencyCode">Currency code : Example  "1"</param>
        /// <param name="defaultValue">Default value if cannot find currency. (ถ้าต้องการใช้ default value ค่าของ isThrowException ต้องเป็น false) : Default: "-"</param>
        /// <param name="isThrowException">If want to throw exception send true</param>
        /// <returns></returns>
        public static string getCurrencyName(string currencyCode, string defaultValue = "-", bool isThrowException = false)
        {
            try
            {
                List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                {
                    new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_CURRENCT,
                        ValueCode = "%"
                    }
                };

                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                lst = hand.GetMiscTypeCodeList(miscs).ToList();

                return lst.Where(m => m.ValueCode == currencyCode).Select(m => m.ValueDisplayEN).First();
            }
            catch (Exception ex)
            {
                if (isThrowException) throw ex;
                else return defaultValue;
            }
        }
        public static string getCurrencyFullName(string currencyCode, string defaultValue = "-", bool isThrowException = false)
        {
            try
            {
                List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                {
                    new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_CURRENCT,
                        ValueCode = "%"
                    }
                };

                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                lst = hand.GetMiscTypeCodeList(miscs).ToList();

                return lst.Where(m => m.ValueCode == currencyCode).Select(m => m.ValueDescription).First();
            }
            catch (Exception ex)
            {
                if (isThrowException) throw ex;
                else return defaultValue;
            }
        }
    }
}