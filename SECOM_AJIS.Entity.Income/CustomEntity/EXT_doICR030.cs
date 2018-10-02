using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using SECOM_AJIS.Common;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Income.MetaData;

namespace SECOM_AJIS.DataEntity.Income
{
    /// <summary>
    /// Data ojbect of Notice1 and Notice2 reports.
    /// </summary>
    public partial class doICR030
    {

        public string RPT_SignatureImageFullPath
        {
            get
            {
                string fullPath = string.Empty;

                if (!string.IsNullOrEmpty(this.SignatureImageFileName))
                {
                    fullPath = PathUtil.GetPathValue(PathUtil.PathName.ImageSignaturePath, this.SignatureImageFileName);
                }

                return fullPath;
            }
        }

    }
}

