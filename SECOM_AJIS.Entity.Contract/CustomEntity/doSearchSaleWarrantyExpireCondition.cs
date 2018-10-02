using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class doSearchSaleWarrantyExpireCondition
    {
        public int ExpireWarrantyMonthFrom { get; set; }
        public int ExpireWarrantyYearFrom { get; set; }
        public int ExpireWarrantyMonthTo { get; set; }
        public int ExpireWarrantyYearTo { get; set; }
        public string OperationOfficeCode { get; set; }
        public string SaleContractOfficeCode { get; set; }
    }
}
