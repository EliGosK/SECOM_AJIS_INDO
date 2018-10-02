using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class View_dtMaintCheckUpResultList : dtMaintCheckUpResultList
    {
        private string ReplaceDoubleQuote(object txtVal)
        {
            if (txtVal != null)
            {
                if (txtVal.GetType() == typeof(string))
                {
                    txtVal = (txtVal == null) ? txtVal : ((string)txtVal).Replace("\"", "\\\"");
                }
                else if (txtVal.GetType() == typeof(bool))
                {
                    txtVal = txtVal.ToString();
                }
                else
                {
                    txtVal = txtVal.ToString();
                }
            }

            return (string)txtVal;

        }

        string srtObject = "";

        // Create JSON object
        public string Object
        {
            get
            {

                srtObject = "{" +

                            "\"InstructionDate\":\"" + (this.InstructionDate != null ? this.InstructionDate.ToString("MMM-yyyy") : "" )+ "\"," +
                            "\"ExpectedMaintenanceDate\":\"" + ( this.ExpectedMaintenanceDate != null ? this.ExpectedMaintenanceDate.Value.ToString("dd-MMM-yyyy") : "" )+ "\"," +
                            "\"MaintenanceDate\":\"" + (this.MaintenanceDate != null ? this.MaintenanceDate.Value.ToString("dd-MMM-yyyy") : "" ) + "\"," +
                            "\"SubcontractName\":\"" + ReplaceDoubleQuote(this.SubContractorName) + "\"," +
                            "\"PICName\":\"" + ReplaceDoubleQuote(this.PICName) + "\"," +
                            "\"MaintenamceEmployee\":\"" + CommonUtil.TextCodeName(this.MaintEmpNo, string.Format("{0} {1}", this.MaintEmpFirstName, this.MaintEmpLastName)) + "\"," +
                            "\"UsageTime\":\"" + this.UsageTime + "\"," +
                            "\"InstrumentMalfunctionFlag\":\"" + (this.InstrumentMalfunctionFlag != null ? (this.InstrumentMalfunctionFlag.Value == true ? "1" : "0") : "0") + "\"," +
                            "\"NeedSalesmanFlag\":\"" + (this.NeedSalesmanFlag != null ? (this.NeedSalesmanFlag.Value == true ? "1" : "0") : "0" )+ "\"," +
                            "\"Location\":\"" + ReplaceDoubleQuote(this.Location) + "\"," +
                            "\"MalfunctionDetail\":\"" + ReplaceDoubleQuote(this.MalfunctionDetail) + "\"," +
                            "\"Remark\":\"" + ReplaceDoubleQuote(this.Remark) + "\"" +

                             "}";
                

                return srtObject;
            }
        }

        public string MaintenanceEmployeeName { get { return string.Format("{0} {1}", this.MaintEmpFirstName, this.MaintEmpLastName); } }
        public string HaveInstrumentMalfinction { get { return this.InstrumentMalfunctionFlag != null ? (this.InstrumentMalfunctionFlag.Value == true ? CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, "CMS210", "lblYes") : CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, "CMS210", "lblNo")) : ""; } }
        public string YearMonth { get { return (this.InstructionDate != null ? string.Format("<div id='{0}'>", this.InstructionDate.ToString("yyyyMM")) + this.InstructionDate.ToString("MMM-yyyy") + "</div>" : ""); } }
    
    }
}
