using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Inventory;
using SECOM_AJIS.DataEntity.Common;

namespace SECOM_AJIS.Presentation.Inventory.Models
{
    /// <summary>
    /// Parameter for screen IVS140.
    /// </summary>
    public class IVS140_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        public List<doOffice> HeaderOffice { set; get; }
        [KeepSession]
        public List<doMiscTypeCode> Miscellaneous { set; get; }
        public IVS140_RegisterData RegisterData { set; get; }
        public string SlipNo { set; get; }

        [KeepSession]
        public string SlipNoReportPath { set; get; }
    }

    /// <summary>
    /// DO of instrument for register checking.
    /// </summary>
    public class IVS140_RegisterData
    {
        public IVS140_HeaderData Header { set; get; }
        public List<IVS140_DetailData> Detail { set; get; }
    }

    public class IVS140_HeaderData
    {
        public string ApproveNo { set; get; }
        public string Memo { set; get; }
    }
    public class IVS140_DetailData : dtSearchInstrumentListResult
    {
        public string txtFixedReturnQtyID { set; get; }
        public string RowNo { get; set; }
    }
}
