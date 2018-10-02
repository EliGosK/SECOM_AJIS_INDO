using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.DataEntity.Income
{
    public class doMatchRReport
    {
        public DateTime? PaymentDate { get; set; }
        public string GroupName { get; set; }
        public string CreateBy { get; set; }
    }

    public class doHeaderReport
    {
        public string PaymentTransNo {get;set;}
        public string PaymentType {get;set;}
        public DateTime PaymentDate {get;set;}
        public decimal PaymentAmount {get;set;}
        public int? SecomAccountID {get;set;}
          
    }
    public class doMatchAccountCode
    {
        public doGetAccountCode AccountCodeInfo { get; set; }
        public decimal? ValueAmount { get; set; }
    }
}
