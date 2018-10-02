using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Billing
{
  public  class AdjustOnNextPeriod
    {
      public string AdjustType
      {
          get;
          set;
      }
      public Nullable<decimal> AdjustBillingPeriodAmount
      {
          get;
          set;
      }
      public Nullable<System.DateTime> AdjustBillingPeriodStartDate
      {
          get;
          set;
      }
      public Nullable<System.DateTime> AdjustBillingPeriodEndDate
      {
          get;
          set;
      }
    }
}
