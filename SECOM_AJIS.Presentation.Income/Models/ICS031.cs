using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Common;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;
//using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Income;
using SECOM_AJIS.Presentation.Income.Models.MetaData;
namespace SECOM_AJIS.Presentation.Income.Models
{
    /// <summary>
    /// Parameter of ICS031 screen
    /// </summary>
    public class ICS031_ScreenParameter : ScreenParameter
    {
        // send data back to client
        [KeepSession]
        public List<doGetDebtTarget> doGetDebtTargetList { set; get; }
        public List<ICS031_DetailData> detailInputDatas { get; set; }
        // send data from client to server
        public ICS031_RegisterData RegisterData { set; get; }
    }

    //// register com sent data to server
    ///// <summary>
    ///// DO of Register Header 
    ///// </summary>
    //public class ICS031_HeaderRegisterData
    //{
    //}

    /// <summary>
    /// DO of Register Detail (Section1)
    /// </summary>
    public class ICS031_DetailRegisterDataSection1
    {
        public string txtBillingOfficeCode { set; get; }
        public string txtCurrency { get; set; }
        public string txtAmountAll { set; get; }
        public string txtAmount2Month { set; get; }
        public string txtDetailAll { set; get; }
        public string txtDetail2Month { set; get; }

    }

    /// <summary>
    /// DO for Register Data
    /// </summary>
    public class ICS031_RegisterData
    {
        //public ICS031_HeaderRegisterData Header { set; get; }
        public List<ICS031_DetailRegisterDataSection1> Detail1 { set; get; } 
    }

    public class ICS031_DetailData
    {
        public int No { get; set; }
        public string BillingOfficeCode { get; set; }
        public string BillingOfficeName { get; set; }
        public string CurrencyType { get; set; }
        public string CurrencyTypeName { get; set; }
        public decimal AllUnpaidTargetAmount { get; set; }
        public int AllUnpaidTargetBillingDetail { get; set; }
        public decimal UnpaidOverTargetAmount { get; set; }
        public int UnpaidOverBillingDetail { get; set; }
    }
}

