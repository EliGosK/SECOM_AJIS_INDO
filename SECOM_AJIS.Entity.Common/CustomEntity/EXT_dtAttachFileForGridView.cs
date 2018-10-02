using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
namespace SECOM_AJIS.DataEntity.Common
{
    public partial class dtAttachFileForGridView
    {
        public string RelatedIDAndFileName
        {
            get
            {
                return string.Format("({0}) {1}", this.RelatedID, this.FileName);
            }
        }
    }

}
