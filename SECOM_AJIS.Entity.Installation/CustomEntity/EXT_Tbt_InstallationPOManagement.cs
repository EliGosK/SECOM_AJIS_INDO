using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Installation.MetaData;

namespace SECOM_AJIS.DataEntity.Installation
{
    [MetadataType(typeof(tbt_InstallationPOManagement_MetaData))]
    public partial class tbt_InstallationPOManagement
    {
        public List<DataEntity.Common.doMiscTypeCode> Currencies { get; set; }
        public string CheckChargeIEEmpName
        {
            get;
            set;
        }

        public string SubContractorName
        {
            get;
            set;
        }

        public string TxtCurrencyActualPOAmount
        {
            get;
            set;
        }
        public string TxtCurrencySubPOAmount
        {
            get;
            set;
        }
        public string TxtCurrencyLastInstallationFee
        {
            get;
            set;
        }
        public string TextUpdateLastInstallation
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.LastInstallationFee);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.LastInstallationFeeCurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

        public decimal? OldActualPOAmount { get; set; }
    }
}

namespace SECOM_AJIS.DataEntity.Installation.MetaData
{
    public class tbt_InstallationPOManagement_MetaData
    {

        [EmployeeMapping("CheckChargeIEEmpName")]
        public string CheckChargeIEEmpNo { get; set; }


    }
}
