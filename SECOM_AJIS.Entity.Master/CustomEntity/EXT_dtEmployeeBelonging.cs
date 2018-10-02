using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Master
{
    /// <summary>
    /// Do Of employee belonging
    /// </summary>
    public partial class dtEmployeeBelonging
    {
        public string EmpFullNameEN
        {
            get
            {
                return String.Format("{0} {1}", EmpFirstNameEN, EmpLastNameEN);
            }
        }
        public string EmpFullNameLC
        {
            get
            {
                return String.Format("{0} {1}", EmpFirstNameLC, EmpLastNameLC);
            }
        }
        public string EmpFullName
        {
            get
            {
                return String.Format("{0} {1}", EmpFirstName, EmpLastName);
            }
        }
        public string EmpFullNameWithCode
        {
            get
            {
                return CommonUtil.TextCodeName(EmpNo, EmpFullName);
            }
        }

        [LanguageMapping]
        public string EmpFirstName { get; set; }
        [LanguageMapping]
        public string EmpLastName { get; set; }
    }
}
