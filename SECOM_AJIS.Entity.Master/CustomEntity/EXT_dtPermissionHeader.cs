using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Master.MetaData;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Master
{
    /// <summary>
    /// Do Of permission header
    /// </summary>
    [MetadataType(typeof(dtPermissionHeader_MetaData))]
    public partial class dtPermissionHeader
    {
        public string OfficeName { get; set; }
        public string PermissionType { get; set; }

        public string OfficeDepPosDisplay
        {
            get
            {
                return "(1) " + (OfficeName == null ? "-" : OfficeName)
                    + "<br/>(2) " + (DepartmentName == null ? "-" : DepartmentName)
                    + "<br/>(3) " + (PositionName == null ? "-" : PositionName);
            }
        }
    }
}

namespace SECOM_AJIS.DataEntity.Master.MetaData
{
    public class dtPermissionHeader_MetaData
    {
        [LanguageMapping]
        public string OfficeName { get; set; }
        [LanguageMapping]
        public string PermissionType { get; set; }
    }

}
