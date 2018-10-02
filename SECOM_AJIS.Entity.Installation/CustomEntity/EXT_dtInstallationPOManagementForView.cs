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

    public partial class dtInstallationPOManagementForView
    {
        CommonUtil cm = new CommonUtil();

        public string SubcontractorCode_Text
        {
            get
            {
                return (CommonUtil.IsNullOrEmpty(SubcontractorCode)) ? "-" : SubcontractorCode;                
            }
        }

        public string SubContractorNameEN_Text
        {
            get
            {
                return (CommonUtil.IsNullOrEmpty(SubContractorNameEN)) ? "-" : SubContractorNameEN;
            }
        }

        public string SubContractorNameLC_Text
        {
            get
            {
                return (CommonUtil.IsNullOrEmpty(SubContractorNameLC)) ? "-" : SubContractorNameLC;
            }
        }
        
    }
}


