using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;
using System.IO;
using SECOM_AJIS.DataEntity.Billing;
using SECOM_AJIS.Presentation.Common;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.Presentation.Billing.Models
{
    //public class RPTInvoiceHeaderDO : DataEntity.Billing.dtRptInvoiceHeader { }
    //public class RPTInvoiceDetailDO : DataEntity.Billing.dtRptInvoiceDetail {C:\Project\CSI\SECOM-SIMS.INDO\SECOM_AJIS\SECOM_AJIS.Presentation.Billing\Models\ReportModel.cs }

    /// <summary>
    /// DO for stored data of Invoice report
    /// </summary>
    public class RPTInvoiceDO : DataEntity.Billing.dtRptInvoice { }

    public class RPTDocReceive : DataEntity.Billing.dtGetRptDocReceipt { }

    /// <summary>
    /// DO for stored data of Tax Invoice report
    /// </summary>
    public class RPTTaxInvoiceDO : DataEntity.Billing.dtRptTaxInvoice { }

    /// <summary>
    /// DO for stored data of Payment Form report
    /// </summary>
    public class RPTPaymentFormDO : DataEntity.Billing.dtRptPaymentForm { }
   
}
