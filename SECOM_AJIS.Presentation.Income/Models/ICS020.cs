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
using System.Web;
using System.IO;
using System.Net.Mime;

namespace SECOM_AJIS.Presentation.Income.Models
{
    /// <summary>
    /// Import payment process type
    /// </summary>
    public enum ICS020_ImportPaymentProcess
    {
        AutoTransfer = 0,
        BankTransfer
    }

    /// <summary>
    /// Data object for ICS_020 screen
    /// </summary>
    public class ICS020_ScreenParameter : ScreenParameter
    {
        public ICS020_ImportPaymentProcess? SelectProcess { get; set; }
        public int? SECOMAccountID { get; set; }
        public Guid? ImportID { get; set; }
        public string CsvFilePath { get; set; }
        public string CsvFileName { get; set; }
        public string CurrencyType { get; set; }
        public List<tbt_tmpImportContent> ContentData { get; set; }
    }
}
