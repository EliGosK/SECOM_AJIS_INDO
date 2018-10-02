using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.DataEntity.Billing
{
    public partial class dtDownloadAutoTransferBankFile
    {
        [LanguageMapping]
        public string BankNameBankBranchName { get; set; }

        [LanguageMapping]
        public string BankBranchName { get; set; }

        public string ToJson { get { return CommonUtil.CreateJsonString(this); } }

    }
}
