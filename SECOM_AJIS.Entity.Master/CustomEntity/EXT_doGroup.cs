using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Master.MetaData;

namespace SECOM_AJIS.DataEntity.Master
{
    /// <summary>
    /// Do Of group
    /// </summary>
    public partial class doGroup
    {
        [LanguageMapping]
        public string GroupName { get; set; }

        [LanguageMapping]
        public string OfficeName { get; set; }
        
        [LanguageMapping]
        public string GroupEmpName { get; set; }

        public string GroupNameShow
        {
            get { return "(1) " + this.GroupNameEN + "<br/>(2) " + this.GroupNameLC; }
        }

        //public string ToJson
        //{
        //    get { return CommonUtil.CreateJsonString(this); }
        //}
    }
}