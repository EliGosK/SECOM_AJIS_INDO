using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.DataEntity.Installation
{
    public partial class RPTInstallRequestDo
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

        public string NameEN1
        {
            get;
            set;
        }
        public string NameEN2
        {
            get;
            set;
        }
        public string NameEN3
        {
            get;
            set;
        }
        public string NameEN4
        {
            get;
            set;
        }
        public string NameEN5
        {
            get;
            set;
        }
        public string Position1
        {
            get;
            set;
        }
        public string Position2
        {
            get;
            set;
        }
        public string Position3
        {
            get;
            set;
        }
        public string Position4
        {
            get;
            set;
        }
        public string Position5
        {
            get;
            set;
        }
    }
}
