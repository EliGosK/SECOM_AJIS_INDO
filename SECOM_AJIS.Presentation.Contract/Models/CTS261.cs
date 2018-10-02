using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Presentation.Contract.Models.MetaData;

namespace SECOM_AJIS.Presentation.Contract.Models
{
    /// <summary>
    /// DO of session parameter screen CTS261
    /// </summary>
    [MetadataType(typeof(doCTS261Condition_MetaData))]
    public class CTS261_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        public string strProjectCode { get; set; }
    }
}
namespace SECOM_AJIS.Presentation.Contract.Models.MetaData
{
    public class doCTS261Condition_MetaData
    {
        [NotNullOrEmpty]
        public string strProjectCode { get; set; }
    }
}