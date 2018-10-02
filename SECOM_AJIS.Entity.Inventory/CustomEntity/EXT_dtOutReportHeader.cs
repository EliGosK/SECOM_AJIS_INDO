using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Inventory.MetaData;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Inventory
{
    [MetadataType(typeof(dtOutReportHeader_Meta))]
    public partial class dtOutReportHeader
    {
        CommonUtil util = new CommonUtil();

        public string ContractCodeShort
        {
            get
            {
                return util.ConvertContractCode(this.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }        
        }

        public string CustName
        {
            get
            {
                return this.CustNameEN;
            }
        }

        public string SiteName { get; set; }
    }
}

namespace SECOM_AJIS.DataEntity.Inventory.MetaData
{
    public class dtOutReportHeader_Meta
    {
        [LanguageMapping]
        public string SiteName { get; set; }
    }
}
