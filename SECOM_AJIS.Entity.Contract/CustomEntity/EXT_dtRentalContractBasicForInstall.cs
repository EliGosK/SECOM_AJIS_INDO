using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.DataEntity.Contract
{
    public partial class dtRentalContractBasicForInstall
    {       

        [LanguageMapping]
        public string BuildingTypeName { get; set; }

        public string Salesman1
        {
            get
            {
                string lang = CommonUtil.GetCurrentLanguage();
                string strSalesman1 = "";
                if (lang == SECOM_AJIS.Common.Util.ConstantValue.CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    strSalesman1 = this.SalesmanEN1;
                }
                else if (lang == SECOM_AJIS.Common.Util.ConstantValue.CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    strSalesman1 = this.SalesmanEN1;
                }
                else if (lang == SECOM_AJIS.Common.Util.ConstantValue.CommonValue.DEFAULT_LANGUAGE_LC)
                {
                    strSalesman1 = this.SalesmanLC1;
                }
                return strSalesman1;                
            }
        }

        public string Salesman2
        {
            get
            {
                string lang = CommonUtil.GetCurrentLanguage();
                string strSalesman2 = "";
                if (lang == SECOM_AJIS.Common.Util.ConstantValue.CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    strSalesman2 = this.SalesmanEN2;
                }
                else if (lang == SECOM_AJIS.Common.Util.ConstantValue.CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    strSalesman2 = this.SalesmanEN2;
                }
                else if (lang == SECOM_AJIS.Common.Util.ConstantValue.CommonValue.DEFAULT_LANGUAGE_LC)
                {
                    strSalesman2 = this.SalesmanLC2;
                }
                return strSalesman2;
            }
        }
       
    }
}
