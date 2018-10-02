using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Master
{
    /// <summary>
    /// Do Of belonging employee no
    /// </summary>
    [MetadataType(typeof(dtBelongingEmpNo_MetaData))]
    public partial class dtBelongingEmpNo
    {
        public string EmpFirstName { get; set; }
        public string EmpLastName { get; set; }

        public string EmpValueCode {
            get {
                //return EmpNo + ":" + EmpFirstName + " " + EmpLastName;
                return CommonUtil.TextCodeName(this.EmpNo, this.EmpFirstName + " " + this.EmpLastName);
            }
        }
    }
    /// <summary>
    /// Do Of belonging employee no meta data
    /// </summary>
    public class dtBelongingEmpNo_MetaData
    {
        [LanguageMapping]
        public string EmpFirstName { get; set; }
        [LanguageMapping]
        public string EmpLastName { get; set; }
    }
}
