using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Entity.ExchangeRate;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM_AJIS.DataEntity.ExchangeRate.Handlers
{
    public class ExchangeRateHandler
    {
        ERDataEntities db = new ERDataEntities();
        string floatNumberFormat = "N2";

        public List<doExchangeRateForCalendar> GetAllExchangeRateForCalendar()
        {
            var results = db.tbt_RateConversion.Select(e => new
            {
                target_date = e.TargetDate,
                start = e.TargetDate,
                end = e.TargetDate,
                title = "",
                color = "",
                all_day = "1",
                bank_rate = e.BankRateRupiahPerDollar,
                tax_rate = e.TaxRateRupiahPerDollar
            });

            List<doExchangeRateForCalendar> resultList = new List<doExchangeRateForCalendar>();
            foreach (var item in results)
            {
                doExchangeRateForCalendar res = new doExchangeRateForCalendar();
                res.target_date = item.target_date.ToString("d-MMM-yyyy");
                res.all_day = item.all_day;
                res.bank_rate = !CommonUtil.IsNullOrEmpty(item.bank_rate) ? ((decimal)item.bank_rate).ToString(floatNumberFormat) : "";
                res.tax_rate = !CommonUtil.IsNullOrEmpty(item.tax_rate) ? ((decimal)item.tax_rate).ToString(floatNumberFormat) : "";
                res.is_today = (item.target_date.Date == DateTime.Now.Date);
                if (res.is_today)
                {
                    res.color = "#FFFF66";
                }
                else
                {
                    res.color = "#FFEEEE";
                }
                res.end = item.end.ToString("yyyy-MM-dd");
                res.start = item.start.ToString("yyyy-MM-dd");
                res.title = item.title;
                resultList.Add(res);
            }

            return resultList;
        }

        public tbt_RateConversion GetExchangeRateByTargetDate(DateTime? targetDate)
        {
            if (CommonUtil.IsNullOrEmpty(targetDate))
            {
                return null;
            }

            var results = db.tbt_RateConversion.Where(e => e.TargetDate == targetDate);
            if (results.Count() < 1)
            {
                return null;
            }

            return results.First();
        }

        public doExchangeRateForCalendar GetCurrentExchangeRate(DateTime? targetDate)
        {
            if (CommonUtil.IsNullOrEmpty(targetDate))
            {
                return null;
            }

            var results = db.tbt_RateConversion.Where(e => e.TargetDate <= targetDate).OrderByDescending(e => e.TargetDate).Select(e => new
            {
                target_date = e.TargetDate,
                start = e.TargetDate,
                end = e.TargetDate,
                title = "",
                color = "",
                all_day = "1",
                bank_rate = e.BankRateRupiahPerDollar,
                tax_rate = e.TaxRateRupiahPerDollar
            });
            if (results.Count() < 1)
            {
                return null;
            }
            var item = results.First();
            doExchangeRateForCalendar res = new doExchangeRateForCalendar();
            res.target_date = item.target_date.ToString("d-MMM-yyyy");
            res.all_day = item.all_day;
            res.bank_rate = !CommonUtil.IsNullOrEmpty(item.bank_rate) ? ((decimal)item.bank_rate).ToString(floatNumberFormat) : "";
            res.tax_rate = !CommonUtil.IsNullOrEmpty(item.tax_rate) ? ((decimal)item.tax_rate).ToString(floatNumberFormat) : "";
            res.is_today = (item.target_date.Date == DateTime.Now.Date);
            if (res.is_today)
            {
                res.color = "#FFFF66";
            }
            else
            {
                res.color = "#FFEEEE";
            }
            res.end = item.end.ToString("yyyy-MM-dd");
            res.start = item.start.ToString("yyyy-MM-dd");
            res.title = item.title;
            return res;
        }

        public tbt_RateConversion RegisterRate(NameValueCollection param)
        {
            DateTime targetDate = DateTime.ParseExact(param["targetDate"], "d/MMM/yyyy", null);

            var registered = db.tbt_RateConversion.Where(e => e.TargetDate == targetDate);
            tbt_RateConversion row = null;
            if (registered.Count() > 0)
            {
                row = UpdateRow(
                    registered.First(),
                    param["bankRateRupiah"],
                    param["taxRateRupiah"],
                    param["bankRateDollar"],
                    param["taxRateDollar"]
                );
            }
            else
            {
                row = CreateNewRow(
                    param["targetDate"],
                    param["bankRateRupiah"],
                    param["taxRateRupiah"],
                    param["bankRateDollar"],
                    param["taxRateDollar"]
                );
                db.tbt_RateConversion.Add(row);
            }
            
            db.SaveChanges();
            return row;
        }

        public tbt_RateConversion CreateNewRow(string targetDate, string bankRateRupiah, string taxRateRupiah, string bankRateDollar, string taxRateDollar)
        {
            tbt_RateConversion row = new tbt_RateConversion();
            
            row.TargetDate = DateTime.Parse(targetDate);
            
            if (!CommonUtil.IsNullOrEmpty(bankRateRupiah))
            {
                row.BankRateRupiahPerDollar = Math.Round(decimal.Parse(bankRateRupiah), 2, MidpointRounding.AwayFromZero);
            }
            if (!CommonUtil.IsNullOrEmpty(taxRateRupiah))
            {
                row.TaxRateRupiahPerDollar = Math.Round(decimal.Parse(taxRateRupiah), 2, MidpointRounding.AwayFromZero);
            }
            if (!CommonUtil.IsNullOrEmpty(bankRateDollar))
            {
                row.BankRateDollarPerRupiah = Math.Round(decimal.Parse(bankRateDollar), 9, MidpointRounding.AwayFromZero);
            }
            if (!CommonUtil.IsNullOrEmpty(taxRateDollar))
            {
                row.TaxkRateDollarPerRupiah = Math.Round(decimal.Parse(taxRateDollar), 9, MidpointRounding.AwayFromZero);
            }
            
            row.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            row.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            row.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            row.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            
            return row;
        }

        public tbt_RateConversion UpdateRow(tbt_RateConversion registerdRow, string bankRateRupiah, string taxRateRupiah, string bankRateDollar, string taxRateDollar)
        {
            if (!CommonUtil.IsNullOrEmpty(bankRateRupiah))
            {
                registerdRow.BankRateRupiahPerDollar = Math.Round(decimal.Parse(bankRateRupiah), 2, MidpointRounding.AwayFromZero);
            }
            if (!CommonUtil.IsNullOrEmpty(taxRateRupiah))
            {
                registerdRow.TaxRateRupiahPerDollar = Math.Round(decimal.Parse(taxRateRupiah), 2, MidpointRounding.AwayFromZero);
            }
            if (!CommonUtil.IsNullOrEmpty(bankRateDollar))
            {
                registerdRow.BankRateDollarPerRupiah = Math.Round(decimal.Parse(bankRateDollar), 9, MidpointRounding.AwayFromZero);
            }
            if (!CommonUtil.IsNullOrEmpty(taxRateDollar))
            {
                registerdRow.TaxkRateDollarPerRupiah = Math.Round(decimal.Parse(taxRateDollar), 9, MidpointRounding.AwayFromZero);
            }
            registerdRow.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            registerdRow.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

            return registerdRow;
        }
    }
}
