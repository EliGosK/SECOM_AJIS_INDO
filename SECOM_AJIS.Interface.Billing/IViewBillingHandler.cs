using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.DataEntity.Billing;
using SECOM_AJIS.DataEntity.Contract;

namespace SECOM_AJIS.DataEntity.Billing
{
    public interface IViewBillingHandler
    {
        /// <summary>
        /// Get BillingBasic data for view
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strBillingOCC"></param>
        /// <param name="strBillingClientCode"></param>
        /// <param name="strBillingTargetCode"></param>
        /// <param name="strBillingCilentname"></param>
        /// <param name="strAddress"></param>
        /// <returns></returns>
        List<dtViewBillingBasic> GetViewBillingBasic(string strContractCode, string strBillingOCC, string strBillingClientCode, string strBillingTargetCode, string strBillingCilentname, string strAddress);
        /// <summary>
        /// Get BillingOccList for view
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        List<dtViewBillingOccList> GetViewBillingOccList(string strContractCode);
        /// <summary>
        /// Get BillingDetailList for view
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <returns></returns>
        List<dtViewBillingDetailList> GetViewBillingDetailList(string contractCode, string billingOCC);
        /// <summary>
        /// Get BillingDetailListOfLastInvoiceOCC for view
        /// </summary>
        /// <param name="invoiceNo"></param>
        /// <param name="billingClientCode"></param>
        /// <param name="billingTargetCode"></param>
        /// <param name="billingCilentname"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        List<dtViewBillingDetailListOfLastInvoiceOCC> GetViewBillingDetailListOfLastInvoiceOCC(string invoiceNo, string billingClientCode, string billingTargetCode, string billingCilentname, string address);
        /// <summary>
        /// Get DepositDetailInformation for view
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <returns></returns>
        List<dtViewDepositDetailInformation> GetViewDepositDetailInformation(string contractCode, string billingOCC);
        /// <summary>
        /// Get BillingBasicList for view
        /// </summary>
        /// <param name="billingClientCode"></param>
        /// <param name="billingTargetCode"></param>
        /// <param name="billingClientName"></param>
        /// <param name="address"></param>
        /// <param name="invoiceNo"></param>
        /// <returns></returns>
        List<dtViewBillingBasicList> GetViewBillingBasicList(string billingClientCode, string billingTargetCode, string billingClientName, string address, string invoiceNo, string taxIDNo);
        /// <summary>
        /// Get BillingInvoiceListOfLastInvoiceOcc for view
        /// </summary>
        /// <param name="billingClientCode"></param>
        /// <param name="billingTargetCode"></param>
        /// <param name="billingCilentname"></param>
        /// <param name="address"></param>
        /// <param name="invoiceNo"></param>
        /// <returns></returns>
        List<dtViewBillingInvoiceListOfLastInvoiceOcc> GetViewBillingInvoiceListOfLastInvoiceOcc(string billingClientCode, string billingTargetCode, string billingCilentname, string address, string invoiceNo, string taxIDNo);
        /// <summary>
        /// Get BillingTargetList for view
        /// </summary>
        /// <param name="billingClientCode"></param>
        /// <param name="billingTargetCode"></param>
        /// <param name="billingClientName"></param>
        /// <param name="address"></param>
        /// <param name="invoiceNo"></param>
        /// <returns></returns>
        List<doBillingTargetList> GetViewBillingTargetList(string billingClientCode, string billingTargetCode, string billingClientName, string address, string invoiceNo, string taxIDNo);
        /// <summary>
        /// Get BillingTargetData for search
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        List<dtBillingTargetData> GetViewBillingTargetDataForSearch(doBillingTargetDataSearchCondition cond);
        /// <summary>
        /// Get BillingDetailList by Target code
        /// </summary>
        /// <param name="billingTargetCode"></param>
        /// <returns></returns>
        List<dtViewBillingDetailList> GetViewBillingDetailListByTargetCode(string billingTargetCode);
        /// <summary>
        /// Get view billing occ list for deposit free
        /// </summary>
        /// <param name="strContractCode">Contract code</param>
        /// <returns></returns>
        List<dtViewBillingOccList> GetViewBillingOCCListForDepositFree(string strContractCode);
    }
}
