using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Master
{
    public partial class tbm_Position
    {
        public string PositionCodeTrim
        {
            get {
                return PositionCode.Trim();
            }
        }
    }
}
