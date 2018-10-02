using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM_AJIS
{
    // Create by Jirawat Jannet @ 2016-10-03
    /// <summary>
    /// Contain convert function
    /// </summary>
    public static class ConvertUtil
    {
        /// <summary>
        /// Convert data type
        /// </summary>
        /// <typeparam name="T">Target data type</typeparam>
        /// <param name="value"></param>
        /// <param name="isThrowException">True: Throw exception when convert error, False: Not throw error</param>
        /// <param name="defaultValue">Default value when cannot convert data</param>
        /// <returns></returns>
        public static T ConvertTo<T>(this object value, bool isThrowException = false, T defaultValue = default(T))
        {
            try
            {
                if (value != null)
                {
                    defaultValue = (T)Convert.ChangeType(value, typeof(T));
                }
                //return (T)Convert.ChangeType(value, typeof(T));
            }
            catch (Exception ex)
            {
                if (isThrowException)
                {
                    throw ex;
                }
            }
            return defaultValue;
        }
    }
}
