using CSI.WindsorHelper;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM_AJIS.Presentation.Common.Service
{
    // Add by jirawat jannete on 22016-12-07
    // รอ echange rate จาก indo ทำเพื่อ test อย่านำไปใช้
    public static class ExchangeRate
    {
        /// <summary>
        /// Convert price to anothor currency type : Case price variables is nullable
        /// </summary>
        /// <param name="price">Currenct price</param>
        /// <param name="fromCurrencyType">Currenct currency type</param>
        /// <param name="toCurrencyType">Currency type that want to convert to</param>
        /// <param name="defaultPrice">If there is error will set default price to this value.</param>
        /// <returns></returns>
        public static decimal? ConvertCurrencyPrice(this decimal? price, string fromCurrencyType, string toCurrencyType, decimal defaultPrice = 0)
        {
            double errorCode = 0;
            return price.ConvertCurrencyPrice(fromCurrencyType, toCurrencyType, DateTime.Now, ref errorCode, defaultPrice);
        }
        /// <summary>
        /// Convert price to anothor currency type : Case price variables is nullable
        /// </summary>
        /// <param name="price">Currenct price</param>
        /// <param name="fromCurrencyType">Currenct currency type</param>
        /// <param name="toCurrencyType">Currency type that want to convert to</param>
        /// <param name="targetDate">Date of exchange rate data</param>
        /// <param name="errorCode">Error code from converting.</param>
        /// <param name="defaultPrice">If there is error will set default price to this value.</param>
        /// <returns></returns>
        public static decimal? ConvertCurrencyPrice(this decimal? price, string fromCurrencyType, string toCurrencyType, DateTime targetDate, ref double errorCode, decimal? defaultPrice = null)
        {
            ICommonHandler commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            return commonHandler.ConvertCurrencyPrice(price, fromCurrencyType, toCurrencyType, targetDate, ref errorCode, defaultPrice);
        }

        /// <summary>
        /// Convert price to anothor currency type
        /// </summary>
        /// <param name="price">Currenct price</param>
        /// <param name="fromCurrencyType">Currenct currency type</param>
        /// <param name="toCurrencyType">Currency type that want to convert to</param>
        /// <param name="defaultPrice">If there is error will set default price to this value.</param>
        /// <returns></returns>
        public static decimal ConvertCurrencyPrice(this decimal price, string fromCurrencyType, string toCurrencyType)
        {
            double errorCode = 0;
            return price.ConvertCurrencyPrice(fromCurrencyType, toCurrencyType, DateTime.Now, ref errorCode);
        }
        /// <summary>
        /// Convert price to anothor currency type
        /// </summary>
        /// <param name="price">Currenct price</param>
        /// <param name="fromCurrencyType">Currenct currency type</param>
        /// <param name="toCurrencyType">Currency type that want to convert to</param>
        /// <param name="targetDate">Date of exchange rate data</param>
        /// <param name="errorCode">Error code from converting.</param>
        /// <param name="defaultPrice">If there is error will set default price to this value.</param>
        /// <returns></returns>
        public static decimal ConvertCurrencyPrice(this decimal price, string fromCurrencyType, string toCurrencyType, DateTime targetDate, ref double errorCode)
        {
            ICommonHandler commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            return commonHandler.ConvertCurrencyPrice(price, fromCurrencyType, toCurrencyType, targetDate, ref errorCode);
        }

    }
}