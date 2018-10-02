using SECOM_AJIS.Entity.ExchangeRate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SECOM_AJIS.Common.Util;
using System.Web.Mvc;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.ExchangeRate.ConstantValue;

namespace SECOM_AJIS.DataEntity.ExchangeRate.Handlers
{
    public class ConvertExchangeRateHandler
    {
        ERDataEntities db = new ERDataEntities();

        /// <summary>
        /// return converted Amount by bank rate
        /// </summary>
        /// <param name="baseDate"></param>
        /// <param name="convertType"></param>
        /// <param name="amount"></param>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        public decimal ConvertAmountByBankRate(DateTime baseDate, string convertType, decimal amount, ref double errorCode)
        {
            errorCode = RateCalcCode.C_NO_ERROR;
            decimal convertedAmount = 0;

            tbt_RateConversion rateRow = GetExchangeBankRate(baseDate, ref errorCode);
            if (errorCode != RateCalcCode.C_NO_ERROR && errorCode != RateCalcCode.C_ERROR_NO_RATE)
            {
                return 0;
            }

            if (convertType == RateCalcCode.C_CONVERT_TYPE_TO_DOLLAR && !CommonUtil.IsNullOrEmpty(rateRow.BankRateDollarPerRupiah))
            {// IRP to USD
                convertedAmount = amount * (decimal)rateRow.BankRateDollarPerRupiah;
            }
            else if (convertType == RateCalcCode.C_CONVERT_TYPE_TO_RPIAH && !CommonUtil.IsNullOrEmpty(rateRow.BankRateRupiahPerDollar))
            {// USD to IRP
                convertedAmount = amount * (decimal)rateRow.BankRateRupiahPerDollar;
            }
            else
            {
                errorCode = RateCalcCode.C_ERROR_OTHER;
            }

            if (errorCode != RateCalcCode.C_NO_ERROR && errorCode != RateCalcCode.C_ERROR_NO_RATE)
            {
                return 0;
            }
            
            CheckConcertedAmount(convertedAmount, ref errorCode);
            if (errorCode != RateCalcCode.C_NO_ERROR && errorCode != RateCalcCode.C_ERROR_NO_RATE)
            {
                return 0;
            }

            RoundConvertedAmount(ref convertedAmount, convertType, ref errorCode);
            if (errorCode != RateCalcCode.C_NO_ERROR && errorCode != RateCalcCode.C_ERROR_NO_RATE)
            {
                return 0;
            }

            return convertedAmount;
        }

        /// <summary>
        /// return converted Amount by tax rate
        /// </summary>
        /// <param name="baseDate"></param>
        /// <param name="convertType"></param>
        /// <param name="amount"></param>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        public decimal ConvertAmountByTaxRate(DateTime baseDate, string convertType, decimal amount, ref double errorCode)
        {
            tbt_RateConversion rateRow = GetExchangeTaxRate(baseDate, ref errorCode);
            decimal convertedAmount = 0;

            if (errorCode != RateCalcCode.C_NO_ERROR && errorCode != RateCalcCode.C_ERROR_NO_RATE)
            {
                return 0;
            }

            if (convertType == RateCalcCode.C_CONVERT_TYPE_TO_DOLLAR && !CommonUtil.IsNullOrEmpty(rateRow.BankRateDollarPerRupiah))
            {
                convertedAmount = amount * (decimal)rateRow.TaxkRateDollarPerRupiah;
            }
            else if (convertType == RateCalcCode.C_CONVERT_TYPE_TO_RPIAH && !CommonUtil.IsNullOrEmpty(rateRow.TaxRateRupiahPerDollar))
            {// USD to IRP
                convertedAmount = amount * (decimal)rateRow.TaxRateRupiahPerDollar;
            }
            else
            {
                errorCode = RateCalcCode.C_ERROR_OTHER;
            }

            if (errorCode != RateCalcCode.C_NO_ERROR && errorCode != RateCalcCode.C_ERROR_NO_RATE)
            {
                return 0;
            }

            CheckConcertedAmount(convertedAmount, ref errorCode);
            if (errorCode != RateCalcCode.C_NO_ERROR && errorCode != RateCalcCode.C_ERROR_NO_RATE)
            {
                return 0;
            }

            RoundConvertedAmount(ref convertedAmount, convertType, ref errorCode);
            if (errorCode != RateCalcCode.C_NO_ERROR && errorCode != RateCalcCode.C_ERROR_NO_RATE)
            {
                return 0;
            }
            return convertedAmount;
        }
        
        /// <summary>
        /// Get current exchange bank rate row.
        /// </summary>
        /// <param name="baseDate"></param>
        /// <param name="getJustBefore"></param>
        /// <returns></returns>
        public tbt_RateConversion GetExchangeBankRate(DateTime baseDate, ref double errorCode)
        {
            tbt_RateConversion rate = GetRateOnTargetDate(baseDate, ref errorCode);
            if (CommonUtil.IsNullOrEmpty(rate))
            {
                errorCode = RateCalcCode.C_ERROR_NO_RATE;
                rate = GetJustBeforeBankRateByTargetDate(baseDate, ref errorCode);
            }
            
            return rate;
        }

        /// <summary>
        /// Get current exchange tax rate row.
        /// </summary>
        /// <param name="baseDate"></param>
        /// <param name="getJustBefore"></param>
        /// <returns></returns>
        public tbt_RateConversion GetExchangeTaxRate(DateTime baseDate, ref double errorCode)
        {
            tbt_RateConversion rate = GetRateOnTargetDate(baseDate, ref errorCode);
            if (CommonUtil.IsNullOrEmpty(rate))
            {
                errorCode = RateCalcCode.C_ERROR_NO_RATE;
                rate = GetJustBeforeTaxRateByTargetDate(baseDate, ref errorCode);
            }
            return rate;
        }

        /// <summary>
        /// Get exchange rate row on base date
        /// </summary>
        /// <param name="baseDate"></param>
        /// <returns></returns>
        public tbt_RateConversion GetRateOnTargetDate(DateTime baseDate, ref double errorCode)
        {
            var rate = db.tbt_RateConversion.Where(e => e.TargetDate == baseDate);
            if (rate.Count() < 1)
            {
                return null;
            }

            return rate.First();
        }
        
        /// <summary>
        /// Get just before exchange bank rate
        /// </summary>
        /// <param name="baseDate"></param>
        /// <returns></returns>
        public tbt_RateConversion GetJustBeforeBankRateByTargetDate(DateTime baseDate, ref double errorCode)
        {
            var rate = db.tbt_RateConversion.Where(e => e.TargetDate < baseDate && e.BankRateRupiahPerDollar != null).OrderByDescending(e => e.TargetDate);
            if (rate.Count() < 1)
            {
                errorCode = RateCalcCode.C_ERROR_OTHER;
                return null;
            }

            return rate.First();
        }

        /// <summary>
        /// Get just before exchange tax rate
        /// </summary>
        /// <param name="baseDate"></param>
        /// <returns></returns>
        public tbt_RateConversion GetJustBeforeTaxRateByTargetDate(DateTime baseDate, ref double errorCode)
        {
            var rate = db.tbt_RateConversion.Where(e => e.TargetDate <= baseDate && e.TaxRateRupiahPerDollar != null).OrderByDescending(e => e.TargetDate);
            if (rate.Count() < 1)
            {
                errorCode = RateCalcCode.C_ERROR_OTHER;
                return null;
            }

            return rate.First();
        }

        public void RoundConvertedAmount(ref decimal convertedAmount, string convertType, ref double errorCode)
        {
            if (convertType == RateCalcCode.C_CONVERT_TYPE_TO_DOLLAR)
            {// IRP to USD
                convertedAmount = Math.Round(convertedAmount, 9, MidpointRounding.AwayFromZero);
            }
            else if (convertType == RateCalcCode.C_CONVERT_TYPE_TO_RPIAH)
            {// USD to IRP
                convertedAmount = Math.Round(convertedAmount, 2, MidpointRounding.AwayFromZero);
            }
            else
            {
                errorCode = RateCalcCode.C_ERROR_OTHER;
            }
        }

        public void CheckConcertedAmount(decimal convertedAmount, ref double errorCode)
        {
            if (convertedAmount >= 100000000000000)
            {
                errorCode = RateCalcCode.C_ERROR_OVER_DIGIT;
            }
        }
    }
}
