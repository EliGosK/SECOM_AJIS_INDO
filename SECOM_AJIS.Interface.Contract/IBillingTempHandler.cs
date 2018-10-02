using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Contract
{
    public interface IBillingTempHandler
    {
        /// <summary>
        /// Insert billing temp data
        /// </summary>
        /// <param name="billing"></param>
        /// <returns></returns>
        List<tbt_BillingTemp> InsertBillingTemp(tbt_BillingTemp billing);

        /// <summary>
        /// Update billing temp data by billing client code and office
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strBillingClientCode"></param>
        /// <param name="strBillingOfficeCode"></param>
        /// <param name="strBillingOCC"></param>
        /// <param name="strBillingTargetCode"></param>
        /// <returns></returns>
        List<tbt_BillingTemp> UpdateBillingTempByBillingClientAndOffice(
            string strContractCode, string strBillingClientCode, string strBillingOfficeCode,
            string strBillingOCC, string strBillingTargetCode);

        /// <summary>
        /// For change billing target from CTS130: CP-16 change customer name and address
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOldBillingClientCode"></param>
        /// <param name="strOldBillingOfficeCode"></param>
        /// <param name="strOldBillingTargetCode"></param>
        /// <param name="strNewBillingClientCode"></param>
        /// <param name="strNewBillingOfficeCode"></param>
        /// <param name="strNewBillingTargetCode"></param>
        /// <returns></returns>
        List<tbt_BillingTemp> UpdateBillingTempByBillingTarget(
            string strContractCode, string strOldBillingClientCode, string strOldBillingOfficeCode,
            string strOldBillingTargetCode, string strNewBillingClientCode, string strNewBillingOfficeCode,
            string strNewBillingTargetCode);

        /// <summary>
        /// Update billing temp data by key
        /// </summary>
        /// <param name="billing"></param>
        /// <returns></returns>
        List<tbt_BillingTemp> UpdateBillingTempByKey(tbt_BillingTemp billing);

        /// <summary>
        /// Delete billing temp data by contract code
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        List<tbt_BillingTemp> DeleteBillingTempByContractCode(string strContractCode);

        /// <summary>
        /// Delete billing temp data by contract code and occ
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <returns></returns>
        List<tbt_BillingTemp> DeleteBillingTempByContractCodeOCC(string strContractCode, string strOCC);

        /// <summary>
        /// Delete billing temp data by key
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <param name="iSequenceNo"></param>
        /// <returns></returns>
        List<tbt_BillingTemp> DeleteBillingTempByKey(string strContractCode, string strOCC, int iSequenceNo);

        /// <summary>
        /// Get billing basic data from billing temp 
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="OCC"></param>
        /// <param name="BillingTypeList"></param>
        /// <param name="BillingTimingList"></param>
        /// <returns></returns>
        List<doBillingTempBasic> GetBillingBasicData(string ContractCode, string OCC, List<string> BillingTypeList, List<string> BillingTimingList);

        /// <summary>
        /// Get billing detail data from billing temp
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="OCC"></param>
        /// <param name="BillingTypeList"></param>
        /// <param name="BillingTimingList"></param>
        /// <returns></returns>
        List<doBillingTempDetail> GetBillingDetailData(string ContractCode, string OCC, List<string> BillingTypeList, List<string> BillingTimingList);

        /// <summary>
        /// Get billing temp from specified fee type
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="occ"></param>
        /// <param name="billingType"></param>
        /// <param name="billingTiming"></param>
        /// <returns></returns>
        List<tbt_BillingTemp> GetFee(string contractCode, string occ, string billingType, string billingTiming);

        /// <summary>
        /// Update flag in billing temp when already send to billing module
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="SequenceNo"></param>
        /// <param name="OCC"></param>
        /// <returns></returns>
        List<tbt_BillingTemp> UpdateSendFlag(string ContractCode, int SequenceNo, string OCC); //Add OCC by Jutarat A. on 05102012

        /// <summary>
        /// Get billing temp data
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <returns></returns>
        List<tbt_BillingTemp> GetTbt_BillingTemp(string strContractCode, string strOCC);

        /// <summary>
        /// Delete data in billing temp when data is send
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <returns></returns>
        List<tbt_BillingTemp> DeleteAllSendData(string ContractCode);
    }
}
