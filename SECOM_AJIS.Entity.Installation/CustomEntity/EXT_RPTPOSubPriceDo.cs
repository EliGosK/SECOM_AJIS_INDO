using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util.ConstantValue;
using System.Globalization;

namespace SECOM_AJIS.DataEntity.Installation
{
    public partial class RPTPOSubPriceDo
    {
        public string DocumentNameEN
        {
            get;
            set;
        }

        public string DocumentVersion
        {
            get;
            set;
        }

        public string ContractCodeShort
        {
            get
            {
                CommonUtil cm = new CommonUtil();
                return cm.ConvertContractCode(this.ContractProjectCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string RPT_Date
        {
            get
            {
                return (this.registerDate.Date.Equals(new DateTime(1900, 1, 1)) ? "" : this.registerDate.ToString("dd/MMM/yyyy", CultureInfo.InvariantCulture));
            }
        }

        public decimal RPT_VatAmount
        {
            get
            {
                return this.ActualPOAmount * this.VatRate;
            }
        }

        public decimal RPT_Total
        {
            get
            {
                return this.ActualPOAmount + RPT_VatAmount;
            }
        }

    }
}
