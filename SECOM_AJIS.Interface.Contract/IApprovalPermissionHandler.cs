using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Contract
{
    public interface IApprovalPermissionHandler
    {
        /// <summary>
        /// Check the permitted IP Address of client for approve draft contract
        /// </summary>
        /// <returns></returns>
        bool isPermittedIPAddress();
    }
}
