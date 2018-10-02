using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.DataEntity.Billing;

namespace SECOM_AJIS.DataEntity.Contract
{
    public interface IBillingInterfaceHandler
    {
        /// <summary>
        /// Send billing basic data of changing contract fee to billing module 
        /// </summary>
        /// <param name="billingList"></param>
        /// <returns></returns>
        bool SendBilling_ChangeFee(List<doBillingTempBasic> billingList);

        /// <summary>
        /// Send billing basic data of changing name to billing module
        /// </summary>
        /// <param name="billingList"></param>
        /// <returns></returns>
        bool SendBilling_ChangeName(List<doBillingTempBasic> billingList);

        /// <summary>
        /// Send billing basic data and billing detail of approve contract to billing module
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <returns></returns>
        bool SendBilling_RentalApprove(string ContractCode);

        /// <summary>
        /// Send billing basic data and billing detail of cancel contract to billing module
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="CancelDate"></param>
        /// <param name="doBillingTempBasicCancel"></param>
        /// <param name="doBillingTempDetailCancel"></param>
        /// <param name="blnCompleteInstallFlag"></param>
        /// <returns></returns>
        bool SendBilling_RentalCancel(string ContractCode, DateTime? CancelDate, doBillingTempBasic doBillingTempBasicCancel, doBillingTempDetail doBillingTempDetailCancel, bool? blnCompleteInstallFlag);

        /// <summary>
        /// Send billing basic data and billing detail of change plan contract to billing module
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="OCC"></param>
        /// <returns></returns>
        bool SendBilling_RentalChangePlan(string ContractCode, string OCC);

        /// <summary>
        /// Send billing basic data and billing detail of complete installation to billing module
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <returns></returns>
        bool SendBilling_RentalCompleteInstall(string ContractCode, DateTime CompleteInstallationDate);

        /// <summary>
        /// Send resume service command to billing module
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="ResumeDate"></param>
        /// <returns></returns>
        bool SendBilling_ResumeService(string ContractCode, DateTime? ResumeDate);

        /// <summary>
        /// Send billing basic data and billing detail of approve contract to billing module
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="SaleOCC"></param>
        /// <returns></returns>
        bool SendBilling_SaleApprove(string ContractCode, string SaleOCC);

        /// <summary>
        /// Send billing detail data of complete installation to billing module
        /// </summary>
        /// <param name="doBillingTempDetailData"></param>
        /// <returns></returns>
        bool SendBilling_SaleCompleteInstall(doBillingTempDetail doBillingTempDetailData);
        //void SendBilling_SaleCustAccept(string ContractCode, string SaleOCC); //No use
        /// <summary>
        /// Send billing detail data of one time to billing module
        /// </summary>
        /// <param name="doBillingTempDetailData"></param>
        /// <returns></returns>
        bool SendBilling_OnetimeFee(doBillingTempDetail doBillingTempDetailData);

        /// <summary>
        ///Send start service command to billing module
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="StartDate"></param>
        /// <param name="AdjustBillingTermEndDate"></param>
        /// <returns></returns>
        bool SendBilling_StartService(string ContractCode, DateTime? StartDate, DateTime? AdjustBillingTermEndDate, string strStartType);

        /// <summary>
        /// Send stop service command to billing module
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="StopDate"></param>
        /// <param name="StopFee"></param>
        /// <returns></returns>
        bool SendBilling_StopService(string ContractCode, DateTime? StopDate, decimal? StopFee);

        /// <summary>
        /// Send billing detail when register result base of maintenance contract to billing module
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="BillingOCC"></param>
        /// <param name="ResultBaseFee"></param>
        /// <returns></returns>
        bool SendBilling_MAResultBase(string ContractCode, string BillingOCC, decimal? ResultBaseFee);
        //List<tbt_BillingBasic> GetBillingBasicByContractCode(string ContractCode); //No use

        /// <summary>
        /// Getting billing target data from billing module
        /// </summary>
        /// <param name="billingTargetCode"></param>
        /// <returns></returns>
        List<tbt_BillingTarget> GetBillingTarget(string billingTargetCode);

        /// <summary>
        /// Getting billing temp from billing module
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="occCode"></param>
        /// <returns></returns>
        List<tbt_BillingTemp> GetBillingBasicAsBillingTemp(string ContractCode, string occCode);

        /// <summary>
        /// Getting billing target detail data from billing module
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        List<doBillingTargetDetail> GetBillingTargetDetailByContractCode(string strContractCode);

        
    }
}
