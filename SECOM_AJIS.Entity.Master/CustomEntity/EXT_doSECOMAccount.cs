using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Master.MetaData;

namespace SECOM_AJIS.DataEntity.Master
{
    /// <summary>
    /// Do Of SECOM bank account
    /// </summary>
    public partial class doSECOMAccount
    {
        public string TextEN
        {
            get
            {
                return string.Format("{0} / {1}", this.BankNameEN, this.BankBranchNameEN);
            }
        }
        public string TextLC
        {
            get
            {
                return string.Format("{0} / {1}", this.BankNameLC, this.BankBranchNameLC);
            }
        }
        public string TextJP
        {
            get
            {
                return this.TextEN;
            }

        }

        public string BankNameJP
        {
            get
            {
                return this.BankNameEN;
            }
        }

        public string BankBranchNameJP
        {
            get
            {
                return this.BankBranchNameEN;
            }
        }

        [LanguageMapping]
        public string Text
        {
            get;
            set;
        }
        
        [LanguageMapping]
        public string BankName
        {
            get;
            set;
        }

        [LanguageMapping]
        public string BankBranchName
        {
            get;
            set;
        }
    }
}