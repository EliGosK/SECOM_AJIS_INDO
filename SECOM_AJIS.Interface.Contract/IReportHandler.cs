using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Contract
{
    public interface IReportHandler
    {
        /// <summary>
        /// To get CTR100 – Maintenance check-up slip report
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="ProductCode"></param>
        /// <param name="InstructionDate"></param>
        /// <param name="owner_pwd"></param>
        /// <returns></returns>
        string GetMaintenanceCheckupSlip(string ContractCode, string ProductCode, DateTime? InstructionDate, string owner_pwd = null);
        
        //string GetMaintenanceCheckupList(List<Object[]> MACheckupKey);

        /// <summary>
        /// To get CTR120 – Maintenance check-up list
        /// </summary>
        /// <param name="MACheckupList"></param>
        /// <returns></returns>
        string GetMaintenanceCheckupList(List<tbt_MaintenanceCheckup> MACheckupList);

        /// <summary>
        /// Get CTR110 - Maintenance check sheet report
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="ProductCode"></param>
        /// <param name="InstructionDate"></param>
        /// <param name="owner_pwd"></param>
        /// <returns></returns>
        string GetMaintenanceCompletionReport(string ContractCode, string ProductCode, DateTime? InstructionDate, string owner_pwd = null);
    }
}
