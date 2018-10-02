using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.DataEntity.Billing
{
    public partial class dtAutoTransferBankAccountForView
    {

        [LanguageMapping]
        public string BankName { get; set; }

        [LanguageMapping]
        public string BankBranchName { get; set; }

        [LanguageMapping]
        public string AccountTypeName { get; set; }

        [LanguageMapping]
        public string LastestResultName { get; set; }

        public string AutoTransferDateForView
        {

            get
            {

                string str = this.AutoTransferDate;
                if (str == "1")
                {
                    str = string.Format("{0}st", str);
                }
                else if (str == "2")
                {
                    str = string.Format("{0}nd", str);
                }
                else if (str == "3")
                {
                    str = string.Format("{0}rd", str);
                }
                else if (string.IsNullOrEmpty(str) == false)
                {
                    str = string.Format("{0}th", str);
                }

                return str;
            }
        }

    }
}
