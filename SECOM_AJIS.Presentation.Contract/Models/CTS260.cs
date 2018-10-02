using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Presentation.Contract.Models.MetaData;

namespace SECOM_AJIS.Presentation.Contract.Models
{
    /// <summary>
    /// DO of session parameter screen CTS260
    /// </summary>
    [MetadataType(typeof(CTS260_MetaData))]
    public class CTS260_ScreenParameter : ScreenSearchParameter
    {
        [KeepSession]
        public string strProjectCode { get; set; }
    }
}
namespace SECOM_AJIS.Presentation.Contract.Models.MetaData
{
    public class CTS260_MetaData
    {
        [NotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG3216, Module = MessageUtil.MODULE_CONTRACT, Controller = MessageUtil.MODULE_CONTRACT,
                    Screen = "CTS260",
                    Parameter = "lblProjectCode")]
        public string strProjectCode { get; set; }
    }
}