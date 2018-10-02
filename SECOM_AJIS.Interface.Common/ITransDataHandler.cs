using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.DataEntity.Common
{
    public interface ITransDataHandler
    {
        /// <summary>
        /// To refresh user data in session (dsTrans)
        /// </summary>
        /// <param name="dsTrans"></param>
        /// <param name="EmpNo"></param>
        void RefreshUserData(dsTransDataModel dsTrans, string EmpNo);
        /// <summary>
        /// To refresh office data in session (dsTrans)
        /// </summary>
        /// <param name="dsTrans"></param>
        void RefreshOfficeData(dsTransDataModel dsTrans);
        /// <summary>
        /// To refresh permission data in session (dsTrans)
        /// </summary>
        /// <param name="dsTrans"></param>
        void RefreshPermissionData(dsTransDataModel dsTrans);
    }
}
