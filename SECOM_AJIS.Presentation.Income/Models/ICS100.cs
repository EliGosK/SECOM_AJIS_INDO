using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Common;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Income;
using SECOM_AJIS.Presentation.Income.Models.MetaData;
namespace SECOM_AJIS.Presentation.Income.Models
{
    /// <summary>
    /// Parameter of ICS100 screen
    /// </summary>
    public class ICS100_ScreenParameter : ScreenParameter
    {
        // send data back to client
        [KeepSession]
        public doReceipt doReceipt { set; get; }
        public tbt_MoneyCollectionInfo _dotbt_MoneyCollectionInfo { set; get; }

        // send data from client to server
        public ICS100_RegisterData RegisterData { set; get; }
    }
    
    /// <summary>
    /// DO of Register Header
    /// </summary>
    public class ICS100_HeaderRegisterData
    {
        public string txtReceiptNo { set; get; }
    }

    /// <summary>
    /// DO of Register Detail (Section1)
    /// </summary>
    public class ICS100_DetailRegisterDataSection1
    {

        public string txtReceiptNo { set; get; }
        public DateTime? dtpReceiptDate { set; get; }
        public string txtBillingTargetCode { set; get; }
        public string txtBillingClientName { set; get; }
        public string txtBillingClientAddress { set; get; }
        public string txtReceiptAmount { set; get; }
        public string txtReceiptAmountCurrencyType { set; get; }
        public string txtCollectionArea { set; get; }
        public DateTime? dtpExpectedCollectDate { set; get; }
        public string txtMemo { set; get; }
        public string rowid { set; get; }

    }

    /// <summary>
    /// DO for Register Data
    /// </summary>
    public class ICS100_RegisterData
    {
        public ICS100_HeaderRegisterData Header { set; get; }
        public List<ICS100_DetailRegisterDataSection1> Detail1 { set; get; }
    }

    /// <summary>
    /// DO for check add Data
    /// </summary>
    public class ICS100_CheckAddData
    {
        public string txtCollectionArea { set; get; }
        public DateTime? dtpExpectedCollectDate { set; get; }
        public bool bolCheckDuplicate { set; get; }
        public string txtReceiptNo { set; get; }
    }


}
