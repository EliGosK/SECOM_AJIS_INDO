using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using SECOM_AJIS.Common;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Common;
using CSI.WindsorHelper;

namespace SECOM_AJIS.DataEntity.Income
{
    /// <summary>
    /// Data object of Function for retrieving debt target data.
    /// </summary>
    public partial class doGetDebtTarget
    {
        [LanguageMapping]
        public string OfficeName { get; set; }

        public string AmountAllString
        {
            get
            {
                //return string.Format("{0}", this.AmountAll.HasValue ? this.AmountAll.Value.ToString("#,##0.00") : "0.00");
                return string.Format("{0}", this.AmountAll.ToString("#,##0.00"));
            }
        }

        public string DetailAllString
        {
            get
            {
                return string.Format("{0}", this.DetailAll.ToString("#,##0"));
            }
        }

        public string Amount2MonthString
        {
            get
            {
                return string.Format("{0}", this.Amount2Month.ToString("#,##0.00"));
            }
        }

        public string Detail2MonthString
        {
            get
            {
                return string.Format("{0}", this.Detail2Month.ToString("#,##0"));
            }
        }

        public string AmountAllCurrencyTypeName
        {
            get
            {
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(AmountAllCurrencyType);
            }
        }
        public string DetailAllCurrencyTypeName
        {
            get
            {
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(DetailAllCurrencyType);
            }
        }
        public string Amount2MonthCurrencyTypeName
        {
            get
            {
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(Amount2MonthCurrencyType);
            }
        }
        public string Detail2MonthCurrencyTypeName
        {
            get
            {
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(Detail2MonthCurrencyType);
            }
        }
    }
}