using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;
using Quartz.Impl;
using System.Diagnostics;
using System.Reflection;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.Common.Util
{
    /// <summary>
    /// Interface of batch process object
    /// </summary>
    public interface IBatchProcess
    {
       
        doBatchProcessResult WorkProcess(string UserId, DateTime BatchDate);
    }
}
