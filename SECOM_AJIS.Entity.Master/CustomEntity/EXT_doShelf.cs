using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Master.MetaData;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Models;

using SECOM_AJIS.DataEntity;

namespace SECOM_AJIS.DataEntity.Master
{
    /// <summary>
    /// Do Of shelf
    /// </summary>
    public partial class doShelf
    {
        
        public string ShelfTypeCodeName { get; set; }     

       
        public string AreaCodeName { get; set; }     

       

        //public string ToJson
        //{
        //    get { return CommonUtil.CreateJsonString(this); }
        //}
    }
}