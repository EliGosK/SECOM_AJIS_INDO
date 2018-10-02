using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Helpers;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Accounting;
using SECOM_AJIS.DataEntity.Accounting.Handlers;

namespace SECOM_AJIS.Presentation.Accounting.Helpers
{
    public static partial class ComboboxHelper
    {
        public static MvcHtmlString AccountingReportCombobox(this HtmlHelper helper, string id, object attribute = null, string firstElement = null)
        {
            AccountingHandler hand = new AccountingHandler();
            List<doAccountingDocumentList> lst = hand.getAccountingReportList();
            return CommonUtil.CommonComboBoxWithCustomFirstElement<doAccountingDocumentList>(id, lst, "DocumentNameENWithCode", "DocumentCode", firstElement, attribute);
        }
    }
}
