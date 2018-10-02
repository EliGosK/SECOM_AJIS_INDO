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

    //public class ISS080_ValidateData
    //{       

    //}

    //public class ISS080_ScreenInput
    //{
      
    //}
    /// <summary>
    /// DO for send data to show in screen
    /// </summary>
    public class ISS080_RegisterStartResumeTargetData  ///// FOR SENT VALUE TO SCREEN
    {
        public List<doSearchInstallManagementResult> doSearchData { get; set; }  
    } 
    /// <summary>
    /// DO of session parameter for screen ISS080
    /// </summary>
    public class ISS080_ScreenParameter : ScreenParameter
    {        
        
    }

}
