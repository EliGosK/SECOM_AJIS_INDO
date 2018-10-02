using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class doSearchMACheckupCriteria
    {

        public string RelatedContractType { get; set; }
        public string OperationOffice { get; set; }
        public string ProductName { get; set; }
        public int CheckupInstructionMonthFrom { get; set; }
        public int CheckupInstructionYearFrom { get; set; }
        public int CheckupInstructionMonthTo { get; set; }
        public int CheckupInstructionYearTo { get; set; }
        public string SiteName { get; set; }
        public string UserCodeContractCode { get; set; }
        public string MAEmployeeName { get; set; }
        public string MACheckupNo { get; set; }
        public bool? HasCheckupResult { get; set; }
        public bool? HaveInstrumentMalfunction { get; set; }
        public bool? NeedToContactSalesman { get; set; }

        //Adds on
        public string UserCode { get; set; }
        public string ContractCode { get; set; }
        public DateTime InstructionDateFrom { get; set; }
        public DateTime InstructionDateTo { get; set; }
        public bool? MaintenanceCheckupSlipFlag { get; set; }
        public bool? MaintenanceCheckupListFlag { get; set; }
    }
}
