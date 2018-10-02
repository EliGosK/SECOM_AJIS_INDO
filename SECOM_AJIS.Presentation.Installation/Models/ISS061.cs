using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Common;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Common.MetaData;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Installation;
using SECOM_AJIS.DataEntity.Installation.MetaData;
using SECOM_AJIS.Presentation.Installation.Models;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.Presentation.Installation.Models
{
    /// <summary>
    /// DO for Validate Data
    /// </summary>
    public class ISS061_ValidateData
    {       

    }

    /// <summary>
    /// DO for Input Screen
    /// </summary>
    public class ISS061_ScreenInput
    {
      
    }

    /// <summary>
    /// DO for Register Start Resume Target
    /// </summary>
    public class ISS061_RegisterStartResumeTargetData  ///// FOR SENT VALUE TO SCREEN
    {
        public List<doSearchInstallManagementResult> doSearchData { get; set; }  
    } 

    /// <summary>
    /// Parameter of ICS061 screen
    /// </summary>
    public class ISS061_ScreenParameter : ScreenParameter
    {        
        
    }

}
