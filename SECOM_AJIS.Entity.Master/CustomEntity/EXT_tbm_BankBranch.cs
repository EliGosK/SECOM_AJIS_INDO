﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Master.MetaData;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Master
{
    /// <summary>
    /// Do Of table bank branch
    /// </summary>
    public partial class tbm_BankBranch
    {
        [LanguageMapping] 
        public string BankBranchName { get; set; }
       
    }
   
}


