using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.DataEntity.Installation
{
    public partial class RPTInstallCompleteDo
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
                return cm.ConvertContractCode(this.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }
        public string NameEN1
        {
            get;
            set;
        }
        public string Position1
        {
            get;
            set;
        }
        public string NameEN2
        {
            get;
            set;
        }
        public string Position2
        {
            get;
            set;
        }
    }
}
