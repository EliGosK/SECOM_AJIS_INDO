using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.CustomAttribute;

using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Presentation.Common.Models.MetaData;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.Presentation.Common.Models
{
    /// <summary>
    /// Parameter for screen CMS080.
    /// </summary>
    public class CMS080_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        public string strCustomerCode { get; set; }

        [KeepSession]
        public string strCustomerCode_short { get; set; }

        [KeepSession]
        public string strCustomerRole { get; set; }
        [KeepSession]
        public bool hasPermission_CMS100 { get; set; }
        [KeepSession]
        public bool hasPermission_CMS280 { get; set; }
    }
}
