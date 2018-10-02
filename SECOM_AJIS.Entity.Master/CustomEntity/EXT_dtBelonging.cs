using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Master.MetaData;

namespace SECOM_AJIS.DataEntity.Master
{
    /// <summary>
    /// Do Of belonging
    /// </summary>
    [MetadataType(typeof(dtBelonging_MetaData))]
    public partial class dtBelonging
    {
        public string OfficeName { get; set; }

        public string OfficeDisplay {
            get {
                return CommonUtil.TextCodeName(OfficeCode, OfficeName);
            }
        }
        public string DepartmentDisplay {
            get {
                return CommonUtil.TextCodeName(DepartmentCode, DepartmentName);
            }
        }
        public string PositionDisplay {
            get {
                return CommonUtil.TextCodeName(PositionCode, PositionName);
            }
        }

        [LanguageMapping]
        public string EmpFirstName { get; set; }
        [LanguageMapping]
        public string EmpLastName { get; set; }
    }
}
namespace SECOM_AJIS.DataEntity.Master.MetaData
{
    /// <summary>
    /// Do Of belonging meta data
    /// </summary>
    public class dtBelonging_MetaData
    {
        [LanguageMapping]
        public string OfficeName { get; set; }
    }
}
