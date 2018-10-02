﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Inventory.MetaData;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Inventory
{
    [MetadataType(typeof(dtInProcessReport_Meta))]
    public partial class dtInProcessReport
    {
        CommonUtil util = new CommonUtil();

        public string ContractCodeShort
        {
            get
            {
                return util.ConvertContractCode(this.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }        
        }

        public string SiteName { get; set; }

    }
}

namespace SECOM_AJIS.DataEntity.Inventory.MetaData
{
    public class dtInProcessReport_Meta
    {
        [LanguageMapping]
        public string SiteName { get; set; }
    }
}