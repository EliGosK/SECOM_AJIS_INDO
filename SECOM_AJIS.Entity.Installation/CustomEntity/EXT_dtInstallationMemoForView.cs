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

    public partial class dtInstallationMemoForView
    {
        CommonUtil cm = new CommonUtil();


        [LanguageMapping]
        public string ObjectName { get; set; }


        [LanguageMapping]
        public string OfficeName { get; set; }

        [LanguageMapping]
        public string EmpFirstName { get; set; }

        [LanguageMapping]
        public string EmpLastName { get; set; }

        //[LanguageMapping]
        //public string DepartmentName { get; set; }


        public string EmpFirstNameLastName
        {
            get
            {
                return string.Format("{0} {1}",
                String.IsNullOrEmpty(this.EmpFirstName) ? "" : this.EmpFirstName,
                String.IsNullOrEmpty(this.EmpLastName) ? "" : this.EmpLastName
                );
            }
        }

        public string Memo_Text
        {
            get
            {
                return string.Format("{0} : {1} : {2} : {3} : {4} : {5}",
                 String.IsNullOrEmpty(CommonUtil.TextDate(this.CreateDate)) ? "-" : CommonUtil.TextDate(this.CreateDate),
                 String.IsNullOrEmpty(EmpFirstNameLastName) ? "-" : EmpFirstNameLastName,
                 String.IsNullOrEmpty(OfficeName) ? "-" : OfficeName,
                 String.IsNullOrEmpty(this.DepartmentName) ? "-" : this.DepartmentName,
                 String.IsNullOrEmpty(ObjectName) ? "-" : ObjectName,
                 String.IsNullOrEmpty(this.Memo) ? "-" : this.Memo
                );
            }
        }
    }
}


