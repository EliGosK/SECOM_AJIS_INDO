using System;
using System.Collections.Generic;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.DataEntity.Master;
using System.Linq;

// Used by Siripoj
namespace SECOM_AJIS.DataEntity.Billing
{
    public partial class BillingHandler : BizBLDataEntities, IBillingHandler
    {
        #region Custom Math method

        /// <summary>
        /// Math - Custom round up 
        /// </summary>
        /// <param name="dec"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public decimal RoundUp(decimal dec, int digits)
        {

            decimal result = 0;

            if (digits > 0)
            {
                // 10^digit
                decimal M1 = Convert.ToDecimal(Math.Pow(10, digits));
                decimal M2 = Convert.ToDecimal(Math.Pow(10, digits + 1));

                // == old ==
                // Math.Ceiling(Math.Floor( 2.010900001*1000)/1000*100)/100
                //result = Math.Ceiling(Math.Floor(dec * M2) / M2 * M1) / M1;

                // == new ==
                result = Math.Round((Math.Floor(dec * M2) / M2 * M1) + 0.01M) / M1;
            }
            else
            {
                result = Math.Ceiling(dec);
            }

            return result;


        }
        #endregion

        #region BLP010 Manage Billing Basic
        /// <summary>
        /// To create/ update billing basic and billing target if there are not existing in DB when contract send command 
        /// </summary>
        /// <param name="doBillingTempBasicList"></param>
        /// <returns></returns>
        public List<doBillingTempBasic> ManageBillingBasic(List<doBillingTempBasic> doBillingTempBasicList)
        {
            try
            {
                bool iResult;
                string strBillingTypeGroup = null;
                AdjustOnNextPeriod doAdjustOnNextPeriod = new AdjustOnNextPeriod();
                IBillingMasterHandler hand = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;
                IRentralContractHandler handContract = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;

                foreach (doBillingTempBasic btbl in doBillingTempBasicList)
                {
                    string strMappingBillingType = null;
                    //1.1
                    if (CommonUtil.IsNullOrEmpty(btbl.ContractCode))
                    {
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "ContractCode" });
                    }

                    //1.2   GetBillingBasic ----*
                    doTbt_BillingBasic doBillingBasic = new doTbt_BillingBasic();
                    List<doTbt_BillingBasic> lstBasic = base.GetBillingBasic(btbl.ContractCode,
                                                                        btbl.BillingOCC,
                                                                        btbl.BillingTargetCode,
                                                                        btbl.BillingClientCode,
                                                                        btbl.BillingOfficeCode,
                                                                        null, null);
                    if (lstBasic != null && lstBasic.Count > 0)
                    {
                        doBillingBasic = lstBasic[0];
                    }
                    else
                    {
                        doBillingBasic = null;
                    }

                    //1.3   strMappingBillingType ----*                 
                    //if (btbl.ContractBillingType != ContractBillingType.C_CONTRACT_BILLING_TYPE_SALE_PRICE)
                    if (btbl.ContractBillingType != ContractBillingType.C_CONTRACT_BILLING_TYPE_PRODUCT_PRICE)
                    {

                        strMappingBillingType = MappingBillingType(btbl.ContractBillingType, btbl.ContractTiming.ToString(), btbl.ProductTypeCode);
                        tbm_BillingType doTbm_BillingType = new tbm_BillingType();
                        //billingMasterHandler = new BillingMasterHandler();

                        if (string.IsNullOrEmpty(strMappingBillingType))
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "strMappingBillingType" });
                        }

                        doTbm_BillingType = hand.GetTbm_BillingType(strMappingBillingType);

                        /* Modify Siripoj 22/02/2012 */
                        if (doTbm_BillingType != null)
                        {
                            strBillingTypeGroup = doTbm_BillingType.BillingTypeGroup;
                        }
                    }

                    //1.4
                    if (!CommonUtil.IsNullOrEmpty(doBillingBasic))
                    {
                        //1.4.1
                        btbl.BillingOCC = doBillingBasic.BillingOCC;
                        btbl.BillingTargetCode = doBillingBasic.BillingTargetCode;
                        btbl.BillingClientCode = doBillingBasic.BillingClientCode;
                        btbl.BillingOfficeCode = doBillingBasic.BillingOfficeCode;
                        //1.4.2                  
                        bool pass = false;   
                        if(doBillingBasic.MonthlyBillingAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            if(Convert.ToDecimal(doBillingBasic.MonthlyBillingAmountUsd) != Convert.ToDecimal(btbl.BillingAmountUsd))
                            {
                                pass = true;
                            }
                        }
                        else 
                        {
                            if (Convert.ToDecimal(doBillingBasic.MonthlyBillingAmount) != Convert.ToDecimal(btbl.BillingAmount))
                            {
                                pass = true;
                            }
                        }
                        if (strBillingTypeGroup == BillingTypeGroup.C_BILLING_TYPE_GROUP_CONTINUES
                            && btbl.ContractTiming != Convert.ToInt32(SendToBillingTiming.C_CONTRACT_TIMING_BEFORE_START)
                            && pass)
                        {

                            //1.4.2.1
                            if (btbl.ContractTiming == Convert.ToInt32(SendToBillingTiming.C_CONTRACT_TIMING_AFTER_START_CHANGE_DURING_STOP))
                            {
                                doBillingBasic.MonthlyFeeBeforeStop = btbl.BillingAmount;
                                doBillingBasic.MonthlyFeeBeforeStopUsd = btbl.BillingAmountUsd;
                                doBillingBasic.MonthlyFeeBeforeStopCurrencyType = btbl.BillingAmountCurrencyType;
                            }
                            //1.4.2.2
                            else if (btbl.ContractTiming == Convert.ToInt32(SendToBillingTiming.C_CONTRACT_TIMING_AFTER_START_CHANGE_PLAN)
                                || btbl.ContractTiming == Convert.ToInt32(SendToBillingTiming.C_CONTRACT_TIMING_AFTER_START_CHANGE_FEE))
                            {
                                if (doBillingBasic.StopBillingFlag == false)
                                {
                                    doAdjustOnNextPeriod = CalculateDifferenceMonthlyFee(btbl.ContractCode, btbl.BillingOCC, Convert.ToDateTime(btbl.ChangeFeeDate), Convert.ToDecimal(btbl.BillingAmount), ProcessID.C_PROCESS_ID_MANAGE_BILLING_BASIC);
                                    doBillingBasic.ResultBasedMaintenanceFlag = false;

                                    // edit by Jirawat Jannet on 2016-12-20
                                    doBillingBasic.MonthlyBillingAmountCurrencyType = btbl.BillingAmountCurrencyType;

                                    if (btbl.BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                                    {
                                        doBillingBasic.MonthlyBillingAmountUsd = btbl.BillingAmountUsd;
                                        doBillingBasic.MonthlyBillingAmount = null;
                                    }
                                    else
                                    {
                                        doBillingBasic.MonthlyBillingAmount = btbl.BillingAmount;
                                        doBillingBasic.MonthlyBillingAmountUsd = null;
                                    }


                                    if ((btbl.BillingAmount <= 0 && btbl.BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                                        || (btbl.BillingAmountUsd <= 0 && btbl.BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_US))
                                    {
                                        doBillingBasic.StopBillingFlag = true;
                                    }
                                    else if ((btbl.BillingAmount > 0 && btbl.BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                                        || (btbl.BillingAmountUsd > 0 && btbl.BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_US))
                                    {
                                        doBillingBasic.StopBillingFlag = false;
                                    }

                                    if (doAdjustOnNextPeriod != null)
                                    {
                                        doBillingBasic.AdjustType = doAdjustOnNextPeriod.AdjustType;
                                        doBillingBasic.AdjustBillingPeriodAmount = doAdjustOnNextPeriod.AdjustBillingPeriodAmount;
                                        doBillingBasic.AdjustBillingPeriodStartDate = doAdjustOnNextPeriod.AdjustBillingPeriodStartDate;
                                        doBillingBasic.AdjustBillingPeriodEndDate = doAdjustOnNextPeriod.AdjustBillingPeriodEndDate;
                                    }
                                }
                                else if (doBillingBasic.StopBillingFlag == true 
                                    && ((btbl.BillingAmount ?? 0) > 0 || (btbl.BillingAmountUsd ?? 0) > 0))
                                {
                                    //doBillingBasic.MonthlyBillingAmount = btbl.BillingAmount;
                                    //doBillingBasic.MonthlyBillingAmountUsd = btbl.BillingAmountUsd;
                                    //doBillingBasic.MonthlyBillingAmountCurrencyType = btbl.BillingAmountCurrencyType;

                                    // add by Jirawat Jannet on 2016-12-20
                                    doBillingBasic.MonthlyBillingAmountCurrencyType = btbl.BillingAmountCurrencyType;

                                    if (btbl.BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                                    {
                                        doBillingBasic.MonthlyBillingAmountUsd = btbl.BillingAmountUsd;
                                        doBillingBasic.MonthlyBillingAmount = null;
                                    }
                                    else
                                    {
                                        doBillingBasic.MonthlyBillingAmount = btbl.BillingAmount;
                                        doBillingBasic.MonthlyBillingAmountUsd = null;
                                    }

                                    doBillingBasic.StopBillingFlag = false;
                                    if (doBillingBasic.LastBillingDate > Convert.ToDateTime(btbl.ChangeFeeDate).AddDays(-1))
                                    {
                                        doAdjustOnNextPeriod = CalculateDifferenceMonthlyFee(btbl.ContractCode, btbl.BillingOCC, Convert.ToDateTime(btbl.ChangeFeeDate), Convert.ToDecimal(btbl.BillingAmount), ProcessID.C_PROCESS_ID_MANAGE_BILLING_BASIC);
                                        if (doAdjustOnNextPeriod != null)
                                        {
                                            doBillingBasic.AdjustType = doAdjustOnNextPeriod.AdjustType;
                                            doBillingBasic.AdjustBillingPeriodAmount = doAdjustOnNextPeriod.AdjustBillingPeriodAmount;
                                            doBillingBasic.AdjustBillingPeriodStartDate = doAdjustOnNextPeriod.AdjustBillingPeriodStartDate;
                                            doBillingBasic.AdjustBillingPeriodEndDate = doAdjustOnNextPeriod.AdjustBillingPeriodEndDate;
                                        }
                                    }
                                    else
                                    {
                                        doBillingBasic.LastBillingDate = Convert.ToDateTime(btbl.ChangeFeeDate).AddDays(-1);

                                        //Create monthly  billing history
                                        tbt_MonthlyBillingHistory doTbt_BillingHistory = new tbt_MonthlyBillingHistory();
                                        doTbt_BillingHistory.ContractCode = doBillingBasic.ContractCode;
                                        doTbt_BillingHistory.BillingOCC = doBillingBasic.BillingOCC;
                                        //doTbt_BillingHistory.HistoryNo = null;


                                        //doTbt_BillingHistory.MonthlyBillingAmount = btbl.BillingAmount;
                                        //doTbt_BillingHistory.MonthlyBillingAmountUsd = btbl.BillingAmountUsd;
                                        //doTbt_BillingHistory.MonthlyBillingAmountCurrencyType = btbl.BillingAmountCurrencyType;

                                        // add by Jirawat Jannet on 2016-12-20
                                        doTbt_BillingHistory.MonthlyBillingAmountCurrencyType = btbl.BillingAmountCurrencyType;

                                        if (btbl.BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                                        {
                                            doTbt_BillingHistory.MonthlyBillingAmountUsd = btbl.BillingAmountUsd;
                                            doTbt_BillingHistory.MonthlyBillingAmount = null;
                                        }
                                        else
                                        {
                                            doTbt_BillingHistory.MonthlyBillingAmount = btbl.BillingAmount;
                                            doTbt_BillingHistory.MonthlyBillingAmountUsd = null;
                                        }

                                        doTbt_BillingHistory.BillingStartDate = btbl.ChangeFeeDate;
                                        doTbt_BillingHistory.BillingEndDate = null;
                                        CreateMonthlyBillingHistory(doTbt_BillingHistory);
                                    }



                                }

                            }
                            else
                            // in case of before new installation complete, or when start service
                            // Timing: Approve contract or before new installation complete or start service
                            {
                                //doBillingBasic.MonthlyBillingAmount = btbl.BillingAmount;
                                //doBillingBasic.MonthlyBillingAmountUsd = btbl.BillingAmountUsd;
                                //doBillingBasic.MonthlyBillingAmountCurrencyType = btbl.BillingAmountCurrencyType;

                                // add by Jirawat Jannet on 2016-12-20
                                doBillingBasic.MonthlyBillingAmountCurrencyType = btbl.BillingAmountCurrencyType;

                                if (btbl.BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                                {
                                    doBillingBasic.MonthlyBillingAmountUsd = btbl.BillingAmountUsd;
                                    doBillingBasic.MonthlyBillingAmount = null;
                                }
                                else
                                {
                                    doBillingBasic.MonthlyBillingAmount = btbl.BillingAmount;
                                    doBillingBasic.MonthlyBillingAmountUsd = null;
                                }

                            }

                            // in case this billing basic is not rental
                            if (doBillingBasic.BillingCycle == BillingCycle.C_BILLING_CYCLE_DEFAULT_ONETIME && Convert.ToDecimal(btbl.BillingAmount) > 0)
                            {
                                // Get rental contract basic data
                                List<tbt_RentalContractBasic> doTbt_RentalContractBasic = handContract.GetTbt_RentalContractBasic(btbl.ContractCode, null);

                                // get Billing Basic Of Contract for check devide contract fee
                                List<BillingBasicList> doBillingBasicOfContractList = GetBillingBasicListData(btbl.ContractCode, MiscType.C_CUST_TYPE);
                                if (doBillingBasicOfContractList != null)
                                {
                                    //Filtering MonthlyBillingAmount > 0
                                    doBillingBasicOfContractList = doBillingBasicOfContractList.Where(d => d.MonthlyBillingAmount.GetValueOrDefault(0) > 0 
                                    || d.MonthlyBillingAmountUsd.GetValueOrDefault(0) > 0).ToList() ;
                                    if (doBillingBasicOfContractList != null && doBillingBasicOfContractList.Count == 1)
                                    {

                                        doBillingBasic.BillingCycle = doBillingBasicOfContractList[0].BillingCycle;
                                        doBillingBasic.CreditTerm = doBillingBasicOfContractList[0].CreditTerm;
                                        doBillingBasic.CalDailyFeeStatus = doBillingBasicOfContractList[0].CalDailyFeeStatus;
                                    }
                                    else
                                    {
                                        doBillingBasic.BillingCycle = BillingCycle.C_BILLING_CYCLE_DEFAULT_SERVICE;
                                        doBillingBasic.CreditTerm = CreditTerm.C_CREDIT_TERM_DEFAULT;
                                        doBillingBasic.CalDailyFeeStatus = CalculationDailyFeeType.C_CALC_DAILY_FEE_TYPE_CALENDAR;
                                    }
                                }
                                else
                                {
                                    doBillingBasic.BillingCycle = BillingCycle.C_BILLING_CYCLE_DEFAULT_SERVICE;
                                    doBillingBasic.CreditTerm = CreditTerm.C_CREDIT_TERM_DEFAULT;
                                    doBillingBasic.CalDailyFeeStatus = CalculationDailyFeeType.C_CALC_DAILY_FEE_TYPE_CALENDAR;
                                }
                                doBillingBasic.ResultBasedMaintenanceFlag = false;
                                doBillingBasic.StartOperationDate = (doTbt_RentalContractBasic != null && doTbt_RentalContractBasic.Count > 0) ? doTbt_RentalContractBasic[0].FirstSecurityStartDate : null;
                            }

                            //1.4.2.4
                            tbt_BillingBasic basic = CommonUtil.CloneObject<doTbt_BillingBasic, tbt_BillingBasic>(doBillingBasic);
                            int iRowCount = UpdateTbt_BillingBasic(basic);
                        }
                        //Add by Patcharee for CRC 2.26             
                        else if (strBillingTypeGroup == BillingTypeGroup.C_BILLING_TYPE_GROUP_CONTINUES
                            && (btbl.ContractTiming == Convert.ToInt32(SendToBillingTiming.C_CONTRACT_TIMING_AFTER_START_CHANGE_PLAN) || btbl.ContractTiming == Convert.ToInt32(SendToBillingTiming.C_CONTRACT_TIMING_AFTER_START_CHANGE_FEE))
                            && Convert.ToDecimal(doBillingBasic.MonthlyBillingAmount) == Convert.ToDecimal(btbl.BillingAmount)
                            && doBillingBasic.StopBillingFlag == false)
                        {
                            //Create monthly  billing history
                            tbt_MonthlyBillingHistory doTbt_BillingHistory = new tbt_MonthlyBillingHistory();
                            doTbt_BillingHistory.ContractCode = doBillingBasic.ContractCode;
                            doTbt_BillingHistory.BillingOCC = doBillingBasic.BillingOCC;
                            //doTbt_BillingHistory.HistoryNo = null;
                            doTbt_BillingHistory.MonthlyBillingAmount = btbl.BillingAmount;
                            doTbt_BillingHistory.MonthlyBillingAmountUsd = btbl.BillingAmountUsd;
                            doTbt_BillingHistory.MonthlyBillingAmountCurrencyType = btbl.BillingAmountCurrencyType;

                            doTbt_BillingHistory.BillingStartDate = btbl.ChangeFeeDate;
                            doTbt_BillingHistory.BillingEndDate = null;
                            CreateMonthlyBillingHistory(doTbt_BillingHistory);
                        }
                        //1.4.3
                        else if (strMappingBillingType == BillingType.C_BILLING_TYPE_MA_RESULT_BASE)
                        {
                            // Set doBillingBasic.ResultBasedMaintenanceFlag
                            if (btbl.BillingAmount <= 0 || btbl.BillingAmountUsd <= 0)
                            {
                                doBillingBasic.ResultBasedMaintenanceFlag = false;
                            }
                            else if (btbl.BillingAmount > 0 || btbl.BillingAmountUsd > 0)
                            {
                                doBillingBasic.ResultBasedMaintenanceFlag = true;
                            }
                            //Set doBillingBasic.MonthlyBillingAmount
                            //doBillingBasic.MonthlyBillingAmount = btbl.BillingAmount;
                            //doBillingBasic.MonthlyBillingAmountUsd = btbl.BillingAmountUsd;
                            //doBillingBasic.MonthlyBillingAmountCurrencyType = btbl.BillingAmountCurrencyType;

                            // add by Jirawat Jannet on 2016-12-20
                            doBillingBasic.MonthlyBillingAmountCurrencyType = btbl.BillingAmountCurrencyType;

                            if (btbl.BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                            {
                                doBillingBasic.MonthlyBillingAmountUsd = btbl.BillingAmountUsd;
                                doBillingBasic.MonthlyBillingAmount = null;
                            }
                            else
                            {
                                doBillingBasic.MonthlyBillingAmount = btbl.BillingAmount;
                                doBillingBasic.MonthlyBillingAmountUsd = null;
                            }



                            tbt_BillingBasic basic = CommonUtil.CloneObject<doTbt_BillingBasic, tbt_BillingBasic>(doBillingBasic);
                            int iRowCount = UpdateTbt_BillingBasic(basic);
                        }
                        //1.4.4 

                        //if (btbl.ContractBillingType != ContractBillingType.C_CONTRACT_BILLING_TYPE_SALE_PRICE)
                        if (strBillingTypeGroup == BillingTypeGroup.C_BILLING_TYPE_GROUP_CONTINUES && Convert.ToDecimal(btbl.BillingAmount) > 0)
                        {
                            //1.4.3.1
                            tbt_BillingTypeDetail doBillingTypeDetail = new tbt_BillingTypeDetail();
                            doBillingTypeDetail = GetTbt_BillingTypeDetailData(btbl.ContractCode, btbl.BillingOCC, strMappingBillingType);
                            //1.4.3.2
                            if (CommonUtil.IsNullOrEmpty(doBillingTypeDetail))
                            {
                                doBillingTypeDetail = new tbt_BillingTypeDetail();
                                doBillingTypeDetail.ContractCode = btbl.ContractCode;
                                doBillingTypeDetail.BillingOCC = btbl.BillingOCC;
                                doBillingTypeDetail.BillingTypeCode = strMappingBillingType;
                                doBillingTypeDetail.InvoiceDescriptionEN = null;
                                doBillingTypeDetail.InvoiceDescriptionLC = null;
                                doBillingTypeDetail.IssueInvoiceFlag = true;
                                doBillingTypeDetail.ProductCode = null;
                                doBillingTypeDetail.ProductTypeCode = btbl.ProductTypeCode;
                                iResult = CreateBillingTypeDetail(doBillingTypeDetail);
                            }
                        }

                    }
                    //1.5
                    else
                    {
                        //1.5.1
                        if (CommonUtil.IsNullOrEmpty(btbl.BillingTargetCode))
                        {
                            //1.5.1.1
                            if (!CommonUtil.IsNullOrEmpty(btbl.BillingClientCode)
                                && !CommonUtil.IsNullOrEmpty(btbl.BillingOfficeCode))
                            {
                                tbt_BillingTarget doTbt_BillingTarget = new tbt_BillingTarget();
                                var dtbillingClient = hand.GetBillingClient(btbl.BillingClientCode);
                                //doTbt_BillingTarget = GetTbt_BillingTarget(null, btbl.BillingClientCode, btbl.BillingOfficeCode);
                                //if (doTbt_BillingTarget == null)
                                //{
                                doTbt_BillingTarget = new tbt_BillingTarget();
                                doTbt_BillingTarget.BillingClientCode = btbl.BillingClientCode;
                                doTbt_BillingTarget.BillingOfficeCode = btbl.BillingOfficeCode;
                                if (dtbillingClient != null && dtbillingClient.Count > 0)
                                {
                                    doTbt_BillingTarget.RealBillingClientNameEN = dtbillingClient[0].FullNameEN;
                                    doTbt_BillingTarget.RealBillingClientNameLC = dtbillingClient[0].FullNameLC;
                                    doTbt_BillingTarget.RealBillingClientAddressEN = dtbillingClient[0].AddressEN;
                                    doTbt_BillingTarget.RealBillingClientAddressLC = dtbillingClient[0].AddressLC;
                                    if (dtbillingClient[0].CustTypeCode == CustomerType.C_CUST_TYPE_JURISTIC && !CommonUtil.IsNullOrEmpty(dtbillingClient[0].IDNo))
                                    {
                                        doTbt_BillingTarget.WhtDeductionType = DeductType.C_DEDUCT_TYPE_DEDUCT;
                                    }
                                }
                                doTbt_BillingTarget.DocLanguage = btbl.DocLanguage; //Add by Jutarat A. on 20122013

                                btbl.BillingTargetCode = CreateBillingTarget(doTbt_BillingTarget);
                                //}
                                //else
                                //{
                                //    btbl.BillingTargetCode = doTbt_BillingTarget.BillingTargetCode;
                                //}
                            }
                            //1.5.1.2
                            else
                            {
                                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "BillingClientCode", "BillingOfficeCode" });
                            }
                        }
                        //1.5.2 create billing basic
                        tbt_BillingBasic doCreateBillingBasic = new tbt_BillingBasic();
                        //1.5.2.2
                        if (strBillingTypeGroup != BillingTypeGroup.C_BILLING_TYPE_GROUP_CONTINUES)
                        {
                            doCreateBillingBasic.ContractCode = btbl.ContractCode;
                            doCreateBillingBasic.BillingOCC = null;
                            doCreateBillingBasic.BillingTargetCode = btbl.BillingTargetCode;
                            doCreateBillingBasic.PreviousBillingTargetCode = null;
                            doCreateBillingBasic.DebtTracingOfficeCode = btbl.BillingOfficeCode;
                            if (strMappingBillingType == BillingType.C_BILLING_TYPE_MA_RESULT_BASE)
                            {
                                doCreateBillingBasic.ResultBasedMaintenanceFlag = true;


                                //doCreateBillingBasic.MonthlyBillingAmount = btbl.BillingAmount;
                                //doCreateBillingBasic.MonthlyBillingAmountUsd = btbl.BillingAmountUsd;
                                //doCreateBillingBasic.MonthlyBillingAmountCurrencyType = btbl.BillingAmountCurrencyType;

                                // add by Jirawat Jannet on 2016-12-20
                                doCreateBillingBasic.MonthlyBillingAmountCurrencyType = btbl.BillingAmountCurrencyType;

                                if (btbl.BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                                {
                                    doCreateBillingBasic.MonthlyBillingAmountUsd = btbl.BillingAmountUsd;
                                    doCreateBillingBasic.MonthlyBillingAmount = null;
                                }
                                else
                                {
                                    doCreateBillingBasic.MonthlyBillingAmount = btbl.BillingAmount;
                                    doCreateBillingBasic.MonthlyBillingAmountUsd = null;
                                }
                            }
                            else
                            {
                                doCreateBillingBasic.ResultBasedMaintenanceFlag = false;
                                doCreateBillingBasic.MonthlyBillingAmount = null;
                                doCreateBillingBasic.MonthlyBillingAmountUsd = null;
                                doCreateBillingBasic.MonthlyBillingAmountCurrencyType = null;
                            }
                            doCreateBillingBasic.StartOperationDate = null;

                            //if (btbl.PaymentMethod == PaymentMethodType.C_PAYMENT_METHOD_MESSENGER)
                            //{
                            //    doCreateBillingBasic.PaymentMethod = btbl.PaymentMethod;
                            //}
                            //else
                            //{
                            //    doCreateBillingBasic.PaymentMethod = PaymentMethodType.C_PAYMENT_METHOD_BANK_TRANSFER;
                            //}
                            doCreateBillingBasic.PaymentMethod = btbl.PaymentMethod; // Modified By Pachara S. 
                            doCreateBillingBasic.BillingCycle = BillingCycle.C_BILLING_CYCLE_DEFAULT_ONETIME;
                            doCreateBillingBasic.CreditTerm = CreditTerm.C_CREDIT_TERM_DEFAULT;
                            doCreateBillingBasic.CalDailyFeeStatus = CalculationDailyFeeType.C_CALC_DAILY_FEE_TYPE_CALENDAR;
                            doCreateBillingBasic.LastBillingDate = null;
                            doCreateBillingBasic.CarefulFlag = false;
                            doCreateBillingBasic.AdjustEndDate = null;
                            doCreateBillingBasic.SortingType = null;
                            doCreateBillingBasic.StopBillingFlag = true;
                            doCreateBillingBasic.MonthlyFeeBeforeStop = null;
                            doCreateBillingBasic.BalanceDeposit = null;
                            doCreateBillingBasic.VATUnchargedFlag = null;
                            doCreateBillingBasic.ChangeDate = null;
                            doCreateBillingBasic.ApproveNo = null;
                            doCreateBillingBasic.DocReceiving = null;
                            doCreateBillingBasic.AdjustType = null;
                            doCreateBillingBasic.AdjustBillingPeriodAmount = null;
                            doCreateBillingBasic.AdjustBillingPeriodStartDate = null;
                            doCreateBillingBasic.AdjustBillingPeriodEndDate = null;
                        }
                        //1.5.2.3
                        else if (btbl.ContractTiming == SendToBillingTiming.C_CONTRACT_TIMING_APPROVE)
                        {
                            doCreateBillingBasic.ContractCode = btbl.ContractCode;
                            doCreateBillingBasic.BillingOCC = null;
                            doCreateBillingBasic.BillingTargetCode = btbl.BillingTargetCode;
                            doCreateBillingBasic.PreviousBillingTargetCode = null;
                            doCreateBillingBasic.DebtTracingOfficeCode = btbl.BillingOfficeCode;
                            doCreateBillingBasic.ResultBasedMaintenanceFlag = strMappingBillingType == BillingType.C_BILLING_TYPE_MA_RESULT_BASE ? true : false;

                            doCreateBillingBasic.StartOperationDate = null;

                            //doCreateBillingBasic.MonthlyBillingAmount = btbl.BillingAmount;
                            //doCreateBillingBasic.MonthlyBillingAmountUsd = btbl.BillingAmountUsd;
                            //doCreateBillingBasic.MonthlyBillingAmountCurrencyType = btbl.BillingAmountCurrencyType;

                            // add by Jirawat Jannet on 2016-12-20
                            doCreateBillingBasic.MonthlyBillingAmountCurrencyType = btbl.BillingAmountCurrencyType;

                            if (btbl.BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                            {
                                doCreateBillingBasic.MonthlyBillingAmountUsd = btbl.BillingAmountUsd;
                                doCreateBillingBasic.MonthlyBillingAmount = null;
                            }
                            else
                            {
                                doCreateBillingBasic.MonthlyBillingAmount = btbl.BillingAmount;
                                doCreateBillingBasic.MonthlyBillingAmountUsd = null;
                            }


                            //if (btbl.PaymentMethod == PaymentMethodType.C_PAYMENT_METHOD_MESSENGER)
                            //{
                            //    doCreateBillingBasic.PaymentMethod = btbl.PaymentMethod;
                            //}
                            //else
                            //{
                            //    doCreateBillingBasic.PaymentMethod = PaymentMethodType.C_PAYMENT_METHOD_BANK_TRANSFER;
                            //}
                            doCreateBillingBasic.PaymentMethod = btbl.PaymentMethod; // Modified By Pachara S. 
                            doCreateBillingBasic.BillingCycle = btbl.BillingCycle;
                            doCreateBillingBasic.CreditTerm = btbl.CreditTerm;
                            doCreateBillingBasic.CalDailyFeeStatus = btbl.CalculationDailyFee;
                            doCreateBillingBasic.LastBillingDate = null;
                            doCreateBillingBasic.CarefulFlag = false;
                            doCreateBillingBasic.AdjustEndDate = null;
                            doCreateBillingBasic.SortingType = null;
                            doCreateBillingBasic.StopBillingFlag = true;
                            doCreateBillingBasic.MonthlyFeeBeforeStop = null;
                            doCreateBillingBasic.BalanceDeposit = null;
                            doCreateBillingBasic.VATUnchargedFlag = null;
                            doCreateBillingBasic.ChangeDate = null;
                            doCreateBillingBasic.ApproveNo = null;
                            doCreateBillingBasic.DocReceiving = null;
                            doCreateBillingBasic.AdjustType = null;
                            doCreateBillingBasic.AdjustBillingPeriodAmount = null;
                            doCreateBillingBasic.AdjustBillingPeriodStartDate = null;
                            doCreateBillingBasic.AdjustBillingPeriodEndDate = null;
                        }
                        //1.5.2.4
                        else
                        {
                            List<BillingBasicList> doBillingBasicOfContractList = new List<BillingBasicList>();
                            doBillingBasicOfContractList = GetBillingBasicListData(btbl.ContractCode, MiscType.C_CUST_TYPE);

                            //Filtering MonthlyBillingAmount > 0
                            doBillingBasicOfContractList = doBillingBasicOfContractList
                                .Where(d => d.MonthlyBillingAmount.GetValueOrDefault(0) > 0 
                                                || d.MonthlyBillingAmountUsd.GetValueOrDefault(0) > 0).ToList();

                            if (doBillingBasicOfContractList != null && doBillingBasicOfContractList.Count == 1)
                            {
                                doCreateBillingBasic.ContractCode = btbl.ContractCode;
                                doCreateBillingBasic.BillingOCC = null;
                                doCreateBillingBasic.BillingTargetCode = btbl.BillingTargetCode;
                                doCreateBillingBasic.PreviousBillingTargetCode = null;
                                doCreateBillingBasic.DebtTracingOfficeCode = btbl.BillingOfficeCode;
                                doCreateBillingBasic.ResultBasedMaintenanceFlag = strMappingBillingType == BillingType.C_BILLING_TYPE_MA_RESULT_BASE ? true : false;

                                // Edit by Narupon W. 
                                if (btbl.ContractTiming <= SendToBillingTiming.C_CONTRACT_TIMING_BEFORE_START)
                                {
                                    doCreateBillingBasic.StartOperationDate = null;
                                }
                                else if (btbl.ContractTiming >= SendToBillingTiming.C_CONTRACT_TIMING_START)
                                {
                                    doCreateBillingBasic.StartOperationDate = doBillingBasicOfContractList[0].StartOperationDate;
                                }

                                //doCreateBillingBasic.MonthlyBillingAmount = btbl.BillingAmount;
                                // Edit by Patcharee T.
                                if (btbl.ContractTiming == SendToBillingTiming.C_CONTRACT_TIMING_BEFORE_START)
                                {
                                    doCreateBillingBasic.MonthlyBillingAmount = null;
                                    doCreateBillingBasic.MonthlyBillingAmountUsd = null;
                                    doCreateBillingBasic.MonthlyBillingAmountCurrencyType = null;
                                }
                                else
                                {
                                    //doCreateBillingBasic.MonthlyBillingAmount = btbl.BillingAmount;
                                    //doCreateBillingBasic.MonthlyBillingAmountUsd = btbl.BillingAmountUsd;
                                    //doCreateBillingBasic.MonthlyBillingAmountCurrencyType = btbl.BillingAmountCurrencyType;

                                    // add by Jirawat Jannet on 2016-12-20
                                    doCreateBillingBasic.MonthlyBillingAmountCurrencyType = btbl.BillingAmountCurrencyType;

                                    if (btbl.BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                                    {
                                        doCreateBillingBasic.MonthlyBillingAmountUsd = btbl.BillingAmountUsd;
                                        doCreateBillingBasic.MonthlyBillingAmount = null;
                                    }
                                    else
                                    {
                                        doCreateBillingBasic.MonthlyBillingAmount = btbl.BillingAmount;
                                        doCreateBillingBasic.MonthlyBillingAmountUsd = null;
                                    }
                                }

                                //if (btbl.PaymentMethod == PaymentMethodType.C_PAYMENT_METHOD_MESSENGER)
                                //{
                                //    doCreateBillingBasic.PaymentMethod = btbl.PaymentMethod;
                                //}
                                //else
                                //{
                                //    doCreateBillingBasic.PaymentMethod = PaymentMethodType.C_PAYMENT_METHOD_BANK_TRANSFER;
                                //}
                                doCreateBillingBasic.PaymentMethod = btbl.PaymentMethod; // Modified By Pachara S. 
                                doCreateBillingBasic.BillingCycle = doBillingBasicOfContractList[0].BillingCycle;
                                doCreateBillingBasic.CreditTerm = doBillingBasicOfContractList[0].CreditTerm;
                                doCreateBillingBasic.CalDailyFeeStatus = doBillingBasicOfContractList[0].CalDailyFeeStatus;
                                if (btbl.ContractTiming <= SendToBillingTiming.C_CONTRACT_TIMING_BEFORE_START)
                                {
                                    doCreateBillingBasic.LastBillingDate = null;
                                }
                                else if (btbl.ContractTiming >= SendToBillingTiming.C_CONTRACT_TIMING_START)
                                {
                                    doCreateBillingBasic.LastBillingDate = Convert.ToDateTime(btbl.ChangeFeeDate).AddDays(-1);
                                }
                                doCreateBillingBasic.CarefulFlag = false;
                                doCreateBillingBasic.AdjustEndDate = null;
                                doCreateBillingBasic.SortingType = null;
                                if (btbl.ContractTiming <= SendToBillingTiming.C_CONTRACT_TIMING_BEFORE_START)
                                {
                                    doCreateBillingBasic.StopBillingFlag = true;
                                }
                                else if (btbl.ContractTiming >= SendToBillingTiming.C_CONTRACT_TIMING_START)
                                {
                                    doCreateBillingBasic.StopBillingFlag = false;
                                }
                                doCreateBillingBasic.MonthlyFeeBeforeStop = null;
                                doCreateBillingBasic.BalanceDeposit = null;
                                doCreateBillingBasic.VATUnchargedFlag = null;
                                doCreateBillingBasic.ChangeDate = null;
                                doCreateBillingBasic.ApproveNo = null;
                                doCreateBillingBasic.DocReceiving = null;
                                doCreateBillingBasic.AdjustType = null;
                                doCreateBillingBasic.AdjustBillingPeriodAmount = null;
                                doCreateBillingBasic.AdjustBillingPeriodStartDate = null;
                                doCreateBillingBasic.AdjustBillingPeriodEndDate = null;
                            }
                            else
                            {
                                doCreateBillingBasic.ContractCode = btbl.ContractCode;
                                doCreateBillingBasic.BillingOCC = null;
                                doCreateBillingBasic.BillingTargetCode = btbl.BillingTargetCode;
                                doCreateBillingBasic.PreviousBillingTargetCode = null;
                                doCreateBillingBasic.DebtTracingOfficeCode = btbl.BillingOfficeCode;
                                doCreateBillingBasic.ResultBasedMaintenanceFlag = strMappingBillingType == BillingType.C_BILLING_TYPE_MA_RESULT_BASE ? true : false; ;


                                // Edit by Narupon W. 
                                if (btbl.ContractTiming <= SendToBillingTiming.C_CONTRACT_TIMING_BEFORE_START)
                                {
                                    doCreateBillingBasic.StartOperationDate = null;
                                }
                                else if (btbl.ContractTiming >= SendToBillingTiming.C_CONTRACT_TIMING_START)
                                {
                                    //doCreateBillingBasic.StartOperationDate = doBillingBasicOfContractList.Count > 0 ? doBillingBasicOfContractList[0].StartOperationDate : null;
                                    doCreateBillingBasic.StartOperationDate = (doBillingBasicOfContractList != null && doBillingBasicOfContractList.Count > 0) ? doBillingBasicOfContractList[0].StartOperationDate : null;
                                }


                                //doCreateBillingBasic.MonthlyBillingAmount = btbl.BillingAmount;
                                // Edit by Patcharee T.
                                if (btbl.ContractTiming == SendToBillingTiming.C_CONTRACT_TIMING_BEFORE_START)
                                {
                                    doCreateBillingBasic.MonthlyBillingAmount = null;
                                    doCreateBillingBasic.MonthlyBillingAmountUsd = null;
                                    doCreateBillingBasic.MonthlyBillingAmountCurrencyType = null;
                                }
                                else
                                {
                                    //doCreateBillingBasic.MonthlyBillingAmount = btbl.BillingAmount;
                                    //doCreateBillingBasic.MonthlyBillingAmountUsd = btbl.BillingAmountUsd;
                                    //doCreateBillingBasic.MonthlyBillingAmountCurrencyType = btbl.BillingAmountCurrencyType;

                                    // add by Jirawat Jannet on 2016-12-20
                                    doCreateBillingBasic.MonthlyBillingAmountCurrencyType = btbl.BillingAmountCurrencyType;

                                    if (btbl.BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                                    {
                                        doCreateBillingBasic.MonthlyBillingAmountUsd = btbl.BillingAmountUsd;
                                        doCreateBillingBasic.MonthlyBillingAmount = null;
                                    }
                                    else
                                    {
                                        doCreateBillingBasic.MonthlyBillingAmount = btbl.BillingAmount;
                                        doCreateBillingBasic.MonthlyBillingAmountUsd = null;
                                    }
                                }

                                //if (btbl.PaymentMethod == PaymentMethodType.C_PAYMENT_METHOD_MESSENGER)
                                //{
                                //    doCreateBillingBasic.PaymentMethod = btbl.PaymentMethod;
                                //}
                                //else
                                //{
                                //    doCreateBillingBasic.PaymentMethod = PaymentMethodType.C_PAYMENT_METHOD_BANK_TRANSFER;
                                //}
                                doCreateBillingBasic.PaymentMethod = btbl.PaymentMethod; // Modified By Pachara S. 
                                doCreateBillingBasic.BillingCycle = BillingCycle.C_BILLING_CYCLE_DEFAULT_SERVICE;
                                doCreateBillingBasic.CreditTerm = CreditTerm.C_CREDIT_TERM_DEFAULT;
                                doCreateBillingBasic.CalDailyFeeStatus = CalculationDailyFeeType.C_CALC_DAILY_FEE_TYPE_CALENDAR;
                                if (btbl.ContractTiming <= SendToBillingTiming.C_CONTRACT_TIMING_BEFORE_START)
                                {
                                    doCreateBillingBasic.LastBillingDate = null;
                                }
                                else if (btbl.ContractTiming >= SendToBillingTiming.C_CONTRACT_TIMING_START)
                                {
                                    doCreateBillingBasic.LastBillingDate = Convert.ToDateTime(btbl.ChangeFeeDate).AddDays(-1);
                                }
                                doCreateBillingBasic.CarefulFlag = false;
                                doCreateBillingBasic.AdjustEndDate = null;
                                doCreateBillingBasic.SortingType = null;
                                if (btbl.ContractTiming <= SendToBillingTiming.C_CONTRACT_TIMING_BEFORE_START)
                                {
                                    doCreateBillingBasic.StopBillingFlag = true;
                                }
                                if (btbl.ContractTiming >= SendToBillingTiming.C_CONTRACT_TIMING_START)
                                {
                                    doCreateBillingBasic.StopBillingFlag = false;
                                }
                                doCreateBillingBasic.MonthlyFeeBeforeStop = null;
                                doCreateBillingBasic.BalanceDeposit = null;
                                doCreateBillingBasic.VATUnchargedFlag = null;
                                doCreateBillingBasic.ChangeDate = null;
                                doCreateBillingBasic.ApproveNo = null;
                                doCreateBillingBasic.DocReceiving = null;
                                doCreateBillingBasic.AdjustType = null;
                                doCreateBillingBasic.AdjustBillingPeriodAmount = null;
                                doCreateBillingBasic.AdjustBillingPeriodStartDate = null;
                                doCreateBillingBasic.AdjustBillingPeriodEndDate = null;
                            }
                        }
                        //1.5.2.5 create billing basic
                        btbl.BillingOCC = CreateBillingBasic(doCreateBillingBasic);
                        //1.5.3
                        if ((strBillingTypeGroup == BillingTypeGroup.C_BILLING_TYPE_GROUP_CONTINUES) 
                            && (Convert.ToDecimal(btbl.BillingAmount) > 0|| Convert.ToDecimal(btbl.BillingAmountUsd) > 0)
                            && (btbl.ContractTiming == SendToBillingTiming.C_CONTRACT_TIMING_AFTER_START_CHANGE_PLAN
                                || btbl.ContractTiming == SendToBillingTiming.C_CONTRACT_TIMING_AFTER_START_CHANGE_DURING_STOP
                                || btbl.ContractTiming == SendToBillingTiming.C_CONTRACT_TIMING_AFTER_START_CHANGE_FEE))
                        {
                            //1.5.3.1
                            bool bResult = false;
                            tbt_MonthlyBillingHistory doTbt_BillingHistory = new tbt_MonthlyBillingHistory();
                            doTbt_BillingHistory.ContractCode = btbl.ContractCode;
                            doTbt_BillingHistory.BillingOCC = btbl.BillingOCC;
                            //doTbt_BillingHistory.HistoryNo = null;

                            //doTbt_BillingHistory.MonthlyBillingAmount = btbl.BillingAmount;
                            //doTbt_BillingHistory.MonthlyBillingAmountUsd = btbl.BillingAmountUsd;
                            //doTbt_BillingHistory.MonthlyBillingAmountCurrencyType = btbl.BillingAmountCurrencyType;

                            // add by Jirawat Jannet on 2016-12-20
                            doTbt_BillingHistory.MonthlyBillingAmountCurrencyType = btbl.BillingAmountCurrencyType;

                            if (btbl.BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                            {
                                doTbt_BillingHistory.MonthlyBillingAmountUsd = btbl.BillingAmountUsd;
                                doTbt_BillingHistory.MonthlyBillingAmount = null;
                            }
                            else
                            {
                                doTbt_BillingHistory.MonthlyBillingAmount = btbl.BillingAmount;
                                doTbt_BillingHistory.MonthlyBillingAmountUsd = null;
                            }

                            doTbt_BillingHistory.BillingStartDate = btbl.ChangeFeeDate;
                            doTbt_BillingHistory.BillingEndDate = null;
                            bResult = CreateMonthlyBillingHistory(doTbt_BillingHistory);
                        }
                        //1.5.4
                        tbt_BillingTypeDetail doBillingTypeDetail = new tbt_BillingTypeDetail();
                        //if (btbl.ContractBillingType != ContractBillingType.C_CONTRACT_BILLING_TYPE_SALE_PRICE)
                        if (strBillingTypeGroup == BillingTypeGroup.C_BILLING_TYPE_GROUP_CONTINUES && (Convert.ToDecimal(btbl.BillingAmount) > 0 || Convert.ToDecimal(btbl.BillingAmountUsd) > 0))
                        {

                            doBillingTypeDetail.ContractCode = btbl.ContractCode;
                            doBillingTypeDetail.BillingOCC = btbl.BillingOCC;
                            doBillingTypeDetail.BillingTypeCode = strMappingBillingType;
                            doBillingTypeDetail.InvoiceDescriptionEN = null;
                            doBillingTypeDetail.InvoiceDescriptionLC = null;
                            doBillingTypeDetail.IssueInvoiceFlag = true;
                            //doBillingTypeDetail.ProductCode = null;//btbl.ProductCode;
                            doBillingTypeDetail.ProductTypeCode = btbl.ProductTypeCode;
                            doBillingTypeDetail.BillingTypeGroup = strBillingTypeGroup;
                            iResult = CreateBillingTypeDetail(doBillingTypeDetail);
                        }
                    }
                }
                return doBillingTempBasicList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// To get billing basic data
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strBillingOCC"></param>
        /// <param name="strBillingTargetCode"></param>
        /// <param name="strBillingClientCode"></param>
        /// <param name="strBillingOfficeCode"></param>
        /// <returns></returns>
        public doTbt_BillingBasic GetBillingBasicData(string strContractCode, string strBillingOCC, string strBillingTargetCode, string strBillingClientCode, string strBillingOfficeCode)
        {
            try
            {
                List<doTbt_BillingBasic> result = base.GetBillingBasic(strContractCode, strBillingOCC, strBillingTargetCode, strBillingClientCode, strBillingOfficeCode
                    , null, null);
                if (result.Count > 0)
                {
                    return result[0];
                }
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Mapping billing type from contract to billing
        /// </summary>
        /// <param name="BillingTypeCode"></param>
        /// <param name="BillingTimingType"></param>
        /// <param name="ProductTypeCode"></param>
        /// <returns></returns>
        public string MappingBillingType(string BillingTypeCode, string BillingTimingType, string ProductTypeCode)
        {
            try
            {

                string strMappingBillingType = null;
                if (BillingTypeCode == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE)
                {
                    if (ProductTypeCode == ProductType.C_PROD_TYPE_AL || ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE || ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
                    {
                        strMappingBillingType = BillingType.C_BILLING_TYPE_SERVICE;
                    }
                    else if (ProductTypeCode == ProductType.C_PROD_TYPE_BE || ProductTypeCode == ProductType.C_PROD_TYPE_SG)
                    {
                        strMappingBillingType = BillingType.C_BILLING_TYPE_SG;
                    }
                }
                else if (BillingTypeCode == ContractBillingType.C_CONTRACT_BILLING_TYPE_STOP_FEE)
                {
                    if (ProductTypeCode == ProductType.C_PROD_TYPE_AL || ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE || ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
                    {
                        strMappingBillingType = BillingType.C_BILLING_TYPE_DURING_STOP_SERVICE;
                    }
                    else if (ProductTypeCode == ProductType.C_PROD_TYPE_MA)
                    {
                        strMappingBillingType = BillingType.C_BILLING_TYPE_DURING_STOP_MA;
                    }
                    else if (ProductTypeCode == ProductType.C_PROD_TYPE_BE || ProductTypeCode == ProductType.C_PROD_TYPE_SG)
                    {
                        strMappingBillingType = BillingType.C_BILLING_TYPE_DURING_STOP_SG;
                    }
                }
                else if (BillingTypeCode == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_CON)
                {
                    strMappingBillingType = BillingType.C_BILLING_TYPE_MA;
                }
                else if (BillingTypeCode == ContractBillingType.C_CONTRACT_BILLING_TYPE_CANCEL_CONTRACT_FEE)
                {
                    strMappingBillingType = BillingType.C_BILLING_TYPE_CANCEL_CONTRACT;
                }
                //else if (BillingTypeCode == ContractBillingType.C_CONTRACT_BILLING_TYPE_SALE_PRICE)
                else if (BillingTypeCode == ContractBillingType.C_CONTRACT_BILLING_TYPE_PRODUCT_PRICE)
                {
                    if (BillingTimingType == BillingTiming.C_BILLING_TIMING_APPROVE_CONTRACT)
                    {
                        // 2017.02.15 modify matsuda start
                        //strMappingBillingType = BillingType.C_BILLING_TYPE_SALE_PRICE_DOWN;
                        strMappingBillingType = BillingType.C_BILLING_TYPE_SALE_PRICE;
                        // 2017.02.15 modify matsuda end
                    }
                    else if (BillingTimingType == BillingTiming.C_BILLING_TIMING_PARTIAL)
                    {
                        // 2017.02.15 modify matsuda start
                        //strMappingBillingType = BillingType.C_BILLING_TYPE_SALE_PRICE_PARTIAL;
                        strMappingBillingType = BillingType.C_BILLING_TYPE_SALE_PRICE_DOWN;
                        // 2017.02.15 modify matsuda end
                    }
                }
                else if (BillingTypeCode == ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE)
                {
                    strMappingBillingType = BillingType.C_BILLING_TYPE_DEPOSIT;
                }
                else if (BillingTypeCode == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE)
                {
                    if (ProductTypeCode == ProductType.C_PROD_TYPE_SALE)
                    {
                        strMappingBillingType = BillingType.C_BILLING_TYPE_INSTALL;
                    }
                    else
                    {
                        strMappingBillingType = BillingType.C_BILLING_TYPE_INSTRUMENT_SETUP;
                    }
                }
                else if (BillingTypeCode == ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE)
                {

                    if (ProductTypeCode == ProductType.C_PROD_TYPE_SALE)
                    {
                        strMappingBillingType = BillingType.C_BILLING_TYPE_INSTALL_SALE;
                    }
                    else
                    {
                        strMappingBillingType = BillingType.C_BILLING_TYPE_CHANGE_INSTALL;
                    }
                }
                else if (BillingTypeCode == ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE)
                {
                    strMappingBillingType = BillingType.C_BILLING_TYPE_REMOVAL_INSTALL;
                }
                else if (BillingTypeCode == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_ONE)
                {
                    strMappingBillingType = BillingType.C_BILLING_TYPE_MA_RESULT_BASE;
                }
                else
                {
                    strMappingBillingType = null;
                }
                return strMappingBillingType;
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// To update tbt_BillingBasic
        /// </summary>
        /// <param name="billingBasic"></param>
        /// <returns></returns>
        public int UpdateTbt_BillingBasic(tbt_BillingBasic billingBasic)
        {
            try
            {
                //set updateDate and updateBy
                billingBasic.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                billingBasic.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_BillingBasic> tbt_BillingBasic = new List<tbt_BillingBasic>();
                tbt_BillingBasic.Add(billingBasic);
                List<tbt_BillingBasic> updatedList = base.UpdateTbt_BillingBasicData(CommonUtil.ConvertToXml_Store<tbt_BillingBasic>(tbt_BillingBasic));

                //Insert Log
                if (updatedList.Count > 0)
                {

                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Update;
                    logData.TableName = TableName.C_TBL_NAME_BILLING_BASIC;
                    logData.TableData = CommonUtil.ConvertToXml(updatedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return updatedList.Count;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //Add by Jutarat A. on 07052013
        /// <summary>
        /// To update list of tbt_BillingBasic
        /// </summary>
        /// <param name="billingBasicList"></param>
        /// <returns></returns>
        public List<tbt_BillingBasic> UpdateTbt_BillingBasic(List<tbt_BillingBasic> billingBasicList)
        {
            try
            {
                //set updateDate and updateBy
                if (billingBasicList != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    foreach (tbt_BillingBasic billingBasic in billingBasicList)
                    {
                        billingBasic.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                        billingBasic.UpdateBy = dsTrans.dtUserData.EmpNo;
                    }
                }

                List<tbt_BillingBasic> updatedList = base.UpdateTbt_BillingBasicData(CommonUtil.ConvertToXml_Store<tbt_BillingBasic>(billingBasicList));

                //Insert Log
                if (updatedList.Count > 0)
                {

                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Update;
                    logData.TableName = TableName.C_TBL_NAME_BILLING_BASIC;
                    logData.TableData = CommonUtil.ConvertToXml(updatedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return updatedList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //End Add

        /// <summary>
        /// To update tbt_BillingBasic without write transaction log
        /// </summary>
        /// <param name="billingBasic"></param>
        /// <returns></returns>
        public int UpdateTbt_BillingBasicNoLog(tbt_BillingBasic billingBasic)
        {
            try
            {
                ////set updateDate and updateBy
                //billingBasic.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                //billingBasic.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_BillingBasic> tbt_BillingBasic = new List<tbt_BillingBasic>();
                tbt_BillingBasic.Add(billingBasic);
                List<tbt_BillingBasic> updatedList = base.UpdateTbt_BillingBasicData(CommonUtil.ConvertToXml_Store<tbt_BillingBasic>(tbt_BillingBasic));

                return updatedList.Count;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// To get data tbt_BillingTypeDetail
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="BillingOCC"></param>
        /// <param name="BillingTypeCode"></param>
        /// <returns></returns>
        public tbt_BillingTypeDetail GetTbt_BillingTypeDetailData(string ContractCode, string BillingOCC, string BillingTypeCode)
        {
            try
            {
                List<tbt_BillingTypeDetail> result = base.GetTbt_BillingTypeDetail(ContractCode, BillingOCC, BillingTypeCode);
                if (result.Count > 0)
                {
                    return result[0];
                }
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// To create billing type detail of billing basic 
        /// </summary>
        /// <param name="doBTD"></param>
        /// <returns></returns>
        public bool CreateBillingTypeDetail(tbt_BillingTypeDetail doBTD)
        {
            try
            {
                tbm_Product doTbm_Product = new tbm_Product();
                List<string> lstField = new List<string>();

                if (CommonUtil.IsNullOrEmpty(doBTD.ContractCode))
                {
                    //lstField.Add("ContractCode");
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "ContractCode" });
                }
                if (CommonUtil.IsNullOrEmpty(doBTD.BillingOCC))
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "BillingOCC" });
                    //lstField.Add("BillingOCC");
                }
                if (CommonUtil.IsNullOrEmpty(doBTD.BillingTypeCode))
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "BillingTypeCode" });
                    //lstField.Add("BillingTypeCode");

                }

                if ((doBTD.BillingTypeCode == BillingType.C_BILLING_TYPE_SALE_PRODUCT_PRICE
                    || doBTD.BillingTypeCode == BillingType.C_BILLING_TYPE_SALE_PRICE
                    || doBTD.BillingTypeCode == BillingType.C_BILLING_TYPE_SALE_PRICE_DOWN
                    || doBTD.BillingTypeCode == BillingType.C_BILLING_TYPE_SALE_PRICE_PARTIAL)
                    && (!CommonUtil.IsNullOrEmpty(doBTD.ProductCode)))
                {
                    IMasterHandler hand = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                    List<tbm_Product> lst = hand.GetTbm_Product(doBTD.ProductCode, null);
                    if (lst != null && lst.Count > 0)
                    {
                        doTbm_Product = lst[0];
                    }
                    //2.2.1
                    if (doBTD.BillingTypeCode == BillingType.C_BILLING_TYPE_SALE_PRODUCT_PRICE)
                    {
                        doBTD.InvoiceDescriptionEN = BillingType.C_BILLING_TYPE_SALE_PRODUCT_PRICE_PREFIX_EN + " " + doTbm_Product.ProductNameEN;
                        doBTD.InvoiceDescriptionLC = BillingType.C_BILLING_TYPE_SALE_PRODUCT_PRICE_PREFIX_LC + " " + doTbm_Product.ProductNameLC;
                    }
                    //2.2.2
                    else if (doBTD.BillingTypeCode == BillingType.C_BILLING_TYPE_SALE_PRICE)
                    {
                        doBTD.InvoiceDescriptionEN = BillingType.C_BILLING_TYPE_SALE_PRICE_PREFIX_EN + " " + doTbm_Product.ProductNameEN;
                        doBTD.InvoiceDescriptionLC = BillingType.C_BILLING_TYPE_SALE_PRICE_PREFIX_LC + " " + doTbm_Product.ProductNameLC;
                    }
                    //2.2.3
                    else if (doBTD.BillingTypeCode == BillingType.C_BILLING_TYPE_SALE_PRICE_DOWN)
                    {
                        doBTD.InvoiceDescriptionEN = BillingType.C_BILLING_TYPE_SALE_PRICE_DOWN_SUBFIX_EN + " " + doTbm_Product.ProductNameEN;
                        doBTD.InvoiceDescriptionLC = BillingType.C_BILLING_TYPE_SALE_PRICE_DOWN_SUBFIX_LC + " " + doTbm_Product.ProductNameLC;
                    }
                    //2.2.4
                    else if (doBTD.BillingTypeCode == BillingType.C_BILLING_TYPE_SALE_PRICE_PARTIAL)
                    {
                        doBTD.InvoiceDescriptionEN = BillingType.C_BILLING_TYPE_SALE_PRICE_PARTIAL_SUBFIX_EN + " " + doTbm_Product.ProductNameEN;
                        doBTD.InvoiceDescriptionLC = BillingType.C_BILLING_TYPE_SALE_PRICE_PARTIAL_SUBFIX_LC + " " + doTbm_Product.ProductNameLC;
                    }
                }

                // === CREATE Billing Type Detail !!
                int iRowCount = InsertTbt_BillingTypeDetail(doBTD);


                //4.
                if (doBTD.BillingTypeGroup == BillingTypeGroup.C_BILLING_TYPE_GROUP_CONTINUES)
                {
                    tbt_BillingTypeDetail doDiffBillingTypeDetail = new tbt_BillingTypeDetail();
                    doDiffBillingTypeDetail.ContractCode = doBTD.ContractCode;
                    doDiffBillingTypeDetail.BillingOCC = doBTD.BillingOCC;
                    doDiffBillingTypeDetail.BillingTypeCode = null;
                    doDiffBillingTypeDetail.InvoiceDescriptionEN = null;
                    doDiffBillingTypeDetail.InvoiceDescriptionLC = null;
                    doDiffBillingTypeDetail.IssueInvoiceFlag = true;
                    doDiffBillingTypeDetail.ProductCode = doBTD.ProductCode;
                    //4.3.3.1
                    if (doBTD.ProductTypeCode == ProductType.C_PROD_TYPE_AL
                        || doBTD.ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE
                        || doBTD.ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
                    {
                        doDiffBillingTypeDetail.BillingTypeCode = BillingType.C_BILLING_TYPE_DIFF_SERVICE;
                    }
                    else if (doBTD.ProductTypeCode == ProductType.C_PROD_TYPE_BE
                       || doBTD.ProductTypeCode == ProductType.C_PROD_TYPE_SG)
                    {
                        doDiffBillingTypeDetail.BillingTypeCode = BillingType.C_BILLING_TYPE_DIFF_SG;
                    }
                    else if (doBTD.ProductTypeCode == ProductType.C_PROD_TYPE_MA)
                    {
                        doDiffBillingTypeDetail.BillingTypeCode = BillingType.C_BILLING_TYPE_DIFF_MA;
                    }
                    //4.4
                    iRowCount = InsertTbt_BillingTypeDetail(doDiffBillingTypeDetail);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Ton insert data to tbt_BillingTypeDetail
        /// </summary>
        /// <param name="dotbt_BillingTypeDetail"></param>
        /// <returns></returns>
        public int InsertTbt_BillingTypeDetail(tbt_BillingTypeDetail dotbt_BillingTypeDetail)
        {
            try
            {
                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                dotbt_BillingTypeDetail.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                dotbt_BillingTypeDetail.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                dotbt_BillingTypeDetail.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                dotbt_BillingTypeDetail.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_BillingTypeDetail> doInsertList = new List<tbt_BillingTypeDetail>();
                doInsertList.Add(dotbt_BillingTypeDetail);
                List<tbt_BillingTypeDetail> insertList = base.InsertTbt_BillingTypeDetailData(CommonUtil.ConvertToXml_Store<tbt_BillingTypeDetail>(doInsertList));

                //Insert Log
                if (insertList.Count > 0)
                {

                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_BILLING_TYPE_DETAIL;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList.Count;
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// To get data of tbt_BillingTarget
        /// </summary>
        /// <param name="BillingTargetCode"></param>
        /// <param name="BillingClientCode"></param>
        /// <param name="BillingOfficeCode"></param>
        /// <returns></returns>
        public tbt_BillingTarget GetTbt_BillingTargetData(string BillingTargetCode, string BillingClientCode, string BillingOfficeCode)
        {
            try
            {
                List<tbt_BillingTarget> result = base.GetTbt_BillingTarget(BillingTargetCode, BillingClientCode, BillingOfficeCode);
                if (result.Count > 0)
                {
                    return result[0];
                }
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// To generate billing target code and insert billing target information 
        /// </summary>
        /// <param name="doBLTG"></param>
        /// <returns></returns>
        public string CreateBillingTarget(tbt_BillingTarget doBLTG)
        {
            try
            {
                string strBillingTargetCode = null;
                List<string> lst = new List<string>();
                if (CommonUtil.IsNullOrEmpty(doBLTG.BillingClientCode))
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "BillingClientCode" });
                    //lst.Add("BillingClientCode");
                }
                if (CommonUtil.IsNullOrEmpty(doBLTG.BillingOfficeCode))
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "BillingOfficeCode" });
                    //lst.Add("BillingOfficeCode");
                }

                tbt_BillingTarget tmpBillingTarget = new tbt_BillingTarget();
                tmpBillingTarget = GetTbt_BillingTargetData(null, doBLTG.BillingClientCode, doBLTG.BillingOfficeCode);
                //3.
                if (tmpBillingTarget == null)
                {
                    lst.Clear();

                    lst = base.GenerateBillingTargetNo(doBLTG.BillingClientCode);
                    string strBillingTargetNo = null;
                    if (lst != null && lst.Count > 0)
                    {
                        strBillingTargetNo = lst[0].ToString();
                    }
                    doBLTG.BillingTargetNo = (Convert.ToInt32(strBillingTargetNo) + 1).ToString("000");
                    doBLTG.BillingTargetCode = doBLTG.BillingClientCode + "-" + doBLTG.BillingTargetNo;
                    strBillingTargetCode = doBLTG.BillingTargetCode;
                    //3.2
                    if (CommonUtil.IsNullOrEmpty(doBLTG.IssueInvTime))
                    {
                        doBLTG.IssueInvTime = IssueInvTime.C_ISSUE_INV_TIME_BEFORE;
                    }
                    if (CommonUtil.IsNullOrEmpty(doBLTG.IssueInvMonth))
                    {
                        doBLTG.IssueInvMonth = 1;
                    }
                    if (CommonUtil.IsNullOrEmpty(doBLTG.IssueInvDate))
                    {
                        doBLTG.IssueInvDate = Convert.ToInt32(IssueInvDate.C_ISSUE_INV_DATE_01);
                    }
                    if (CommonUtil.IsNullOrEmpty(doBLTG.InvFormatType))
                    {
                        doBLTG.InvFormatType = InvFormatType.C_INV_FORMAT_FOLLOW_BILLING;
                    }
                    if (CommonUtil.IsNullOrEmpty(doBLTG.SignatureType))
                    {
                        //doBLTG.SignatureType = SigType.C_SIG_TYPE_NO;
                        doBLTG.SignatureType = SigType.C_SIG_TYPE_HAVE; //Modify by Jutarat A. on 16102013
                    }
                    if (CommonUtil.IsNullOrEmpty(doBLTG.DocLanguage))
                    {
                        doBLTG.DocLanguage = ReportDocLanguage.C_DOC_LANG_LOCAL;
                    }
                    if (CommonUtil.IsNullOrEmpty(doBLTG.ShowDueDate))
                    {
                        doBLTG.ShowDueDate = ShowDueDate.C_SHOW_DUEDATE_7;
                    }
                    if (CommonUtil.IsNullOrEmpty(doBLTG.IssueReceiptTiming))
                    {
                        doBLTG.IssueReceiptTiming = IssueRecieptTime.C_ISSUE_REC_TIME_AFTER_PAYMENT;
                    }
                    if (CommonUtil.IsNullOrEmpty(doBLTG.ShowAccType))
                    {
                        doBLTG.ShowAccType = ShowBankAccType.C_SHOW_BANK_ACC_SHOW;
                    }
                    if (CommonUtil.IsNullOrEmpty(doBLTG.WhtDeductionType))
                    {
                        doBLTG.WhtDeductionType = DeductType.C_DEDUCT_TYPE_NOT_DEDUCT;
                    }
                    if (CommonUtil.IsNullOrEmpty(doBLTG.ShowIssueDate))
                    {
                        doBLTG.ShowIssueDate = ShowIssueDate.C_SHOW_ISSUE_DATE_CHRISTIAN;
                    }
                    if (CommonUtil.IsNullOrEmpty(doBLTG.PayByChequeFlag))
                    {
                        doBLTG.PayByChequeFlag = false;
                    }
                    if (CommonUtil.IsNullOrEmpty(doBLTG.ShowInvWHTFlag))
                    {
                        doBLTG.ShowInvWHTFlag = false;
                    }
                    if (CommonUtil.IsNullOrEmpty(doBLTG.SeparateInvType))
                    {
                        doBLTG.SeparateInvType = SeparateInvType.C_SEP_INV_EACH_CONTRACT;
                    }
                    //if (CommonUtil.IsNullOrEmpty(doBLTG.SuppleInvAddress))
                    //{
                    //    doBLTG.SuppleInvAddress = null;
                    //}

                    //3.3
                    InsertTbt_BillingTarget(doBLTG);
                    strBillingTargetCode = doBLTG.BillingTargetCode;
                }
                else
                {
                    strBillingTargetCode = tmpBillingTarget.BillingTargetCode;
                }

                return strBillingTargetCode;
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// To insert data to tbt_BillingTarget
        /// </summary>
        /// <param name="doTbt_BillingTarget"></param>
        /// <returns></returns>
        public int InsertTbt_BillingTarget(tbt_BillingTarget doTbt_BillingTarget)
        {
            try
            {
                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                doTbt_BillingTarget.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doTbt_BillingTarget.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                doTbt_BillingTarget.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doTbt_BillingTarget.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_BillingTarget> doInsertList = new List<tbt_BillingTarget>();
                doInsertList.Add(doTbt_BillingTarget);
                List<tbt_BillingTarget> insertList = base.InsertTbt_BillingTargetData(CommonUtil.ConvertToXml_Store<tbt_BillingTarget>(doInsertList));

                //Insert Log
                if (insertList.Count > 0)
                {

                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_BILLING_TARGET;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList.Count;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// To generate billing basic code and insert billing basic information 
        /// </summary>
        /// <param name="doTbt_BillingBasic"></param>
        /// <returns></returns>
        public string CreateBillingBasic(tbt_BillingBasic doTbt_BillingBasic)
        {
            try
            {
                string strBillingOCC = null;
                List<string> lst = new List<string>();
                if (CommonUtil.IsNullOrEmpty(doTbt_BillingBasic.ContractCode))
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "ContractCode" });
                }
                if (CommonUtil.IsNullOrEmpty(doTbt_BillingBasic.BillingTargetCode))
                {
                    //lst.Add("BillingTargetCode");
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "BillingTargetCode" });
                }
                //2.
                doTbt_BillingBasic doBillingBasic = new doTbt_BillingBasic();
                List<doTbt_BillingBasic> lstBasic = base.GetBillingBasic(doTbt_BillingBasic.ContractCode, null, doTbt_BillingBasic.BillingTargetCode, null, null, null, null);
                if (lstBasic != null && lstBasic.Count > 0)
                {
                    doBillingBasic = lstBasic[0];
                }
                else
                {
                    doBillingBasic = null;
                }
                //doBillingBasic = GetBillingBasicData(doTbt_BillingBasic.ContractCode, null, doTbt_BillingBasic.BillingTargetCode, null, null);
                //3.
                if (doBillingBasic == null)
                {
                    //getBillingOOC
                    lst.Clear();
                    lst = base.GetBillingOCC(doTbt_BillingBasic.ContractCode);
                    if (lst != null && lst.Count > 0)
                    {
                        doTbt_BillingBasic.BillingOCC = (Convert.ToInt32(lst[0]) + 1).ToString("00");
                        strBillingOCC = doTbt_BillingBasic.BillingOCC;
                    }
                    if (doTbt_BillingBasic.CarefulFlag == null)
                    {
                        doTbt_BillingBasic.CarefulFlag = false;
                    }
                    if (doTbt_BillingBasic.VATUnchargedFlag == null)
                    {
                        doTbt_BillingBasic.VATUnchargedFlag = false;
                    }
                    //3.2
                    int iRowcount = InsertTbt_BillingBasic(doTbt_BillingBasic);
                }
                //4.
                else
                {
                    strBillingOCC = doTbt_BillingBasic.BillingOCC;
                }
                return strBillingOCC;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// To insert data to tbt_BillingBasic
        /// </summary>
        /// <param name="doBillingBasic"></param>
        /// <returns></returns>
        public int InsertTbt_BillingBasic(tbt_BillingBasic doBillingBasic)
        {
            try
            {
                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                doBillingBasic.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doBillingBasic.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                doBillingBasic.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doBillingBasic.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_BillingBasic> doInsertList = new List<tbt_BillingBasic>();
                doInsertList.Add(doBillingBasic);
                List<tbt_BillingBasic> insertList = base.InsertTbt_BillingBasicData(CommonUtil.ConvertToXml_Store<tbt_BillingBasic>(doInsertList));

                //Insert Log
                if (insertList.Count > 0)
                {

                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_BILLING_BASIC;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList.Count;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// To update billing end date of lasted monthly billing history and insert new monthly billing history 
        /// </summary>
        /// <param name="doTbt_MonthlyBillingHistory"></param>
        /// <returns></returns>
        public bool CreateMonthlyBillingHistory(tbt_MonthlyBillingHistory doTbt_MonthlyBillingHistory)
        {
            try
            {
                //1.
                int iRowCount = 0;
                List<string> lst = new List<string>();
                if (CommonUtil.IsNullOrEmpty(doTbt_MonthlyBillingHistory.ContractCode))
                {
                    //lst.Add("ContractCode");
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "ContractCode" });
                }
                if (CommonUtil.IsNullOrEmpty(doTbt_MonthlyBillingHistory.BillingOCC))
                {
                    //lst.Add("BillingOOC");
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "BillingOCC" });
                }
                //2.            
                tbt_MonthlyBillingHistory doLastedMonthlyBillingHistory = new tbt_MonthlyBillingHistory();
                doLastedMonthlyBillingHistory = GetLastBillingHistoryData(doTbt_MonthlyBillingHistory.ContractCode, doTbt_MonthlyBillingHistory.BillingOCC);

                if (doLastedMonthlyBillingHistory != null)
                {
                    //2.2.1

                    if (doTbt_MonthlyBillingHistory.BillingStartDate.Value.Date > doLastedMonthlyBillingHistory.BillingStartDate.Value.Date)
                    {
                        //2.2.1.1
                        doLastedMonthlyBillingHistory.BillingEndDate = Convert.ToDateTime(doTbt_MonthlyBillingHistory.BillingStartDate).AddDays(-1);
                        iRowCount = UpdateTbt_MonthlyBillingHistory(doLastedMonthlyBillingHistory);

                        //2.2.1.2
                        doTbt_MonthlyBillingHistory.HistoryNo = doLastedMonthlyBillingHistory.HistoryNo + 1;
                    }
                    //2.2.2
                    else // doTbt_MonthlyBillingHistory.BillingStartDate and doLastedMonthlyBillingHistory. BillingStartDate  has same date
                    {
                        //2.2.2.1
                        //doLastedMonthlyBillingHistory.MonthlyBillingAmount = doTbt_MonthlyBillingHistory.MonthlyBillingAmount;
                        doLastedMonthlyBillingHistory.BillingEndDate = doLastedMonthlyBillingHistory.BillingStartDate;
                        iRowCount = UpdateTbt_MonthlyBillingHistory(doLastedMonthlyBillingHistory);
                        doTbt_MonthlyBillingHistory.HistoryNo = doLastedMonthlyBillingHistory.HistoryNo + 1;

                    }
                }
                else
                {
                    doTbt_MonthlyBillingHistory.HistoryNo = 1;
                }

                //3.1
                InsertTbt_MonthlyBillingHistory(doTbt_MonthlyBillingHistory);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// To get last monthly billing history of billing basic
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="BillingOCC"></param>
        /// <returns></returns>
        public tbt_MonthlyBillingHistory GetLastBillingHistoryData(string ContractCode, string BillingOCC)
        {
            try
            {
                List<string> lst = new List<string>();
                if (CommonUtil.IsNullOrEmpty(ContractCode))
                {
                    //lst.Add("ContractCode");
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "ContractCode" });
                }
                if (CommonUtil.IsNullOrEmpty(BillingOCC))
                {
                    //lst.Add("BillingOOC");
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "BillingOCC" });
                }
                List<tbt_MonthlyBillingHistory> result = base.GetLastBillingHistory(ContractCode, BillingOCC,CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US);
                if (result.Count > 0)
                {
                    return result[0];
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// To update tbt_MonthlyBillingHistory
        /// </summary>
        /// <param name="dolast"></param>
        /// <returns></returns>
        public int UpdateTbt_MonthlyBillingHistory(tbt_MonthlyBillingHistory dolast)
        {

            try
            {
                //set updateDate and updateBy
                dolast.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                dolast.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_MonthlyBillingHistory> monthlyBillingHistoryList = new List<tbt_MonthlyBillingHistory>();
                monthlyBillingHistoryList.Add(dolast);
                List<tbt_MonthlyBillingHistory> updatedList = base.UpdateTbt_MonthlyBillingHistoryData(CommonUtil.ConvertToXml_Store<tbt_MonthlyBillingHistory>(monthlyBillingHistoryList));

                //Insert Log
                if (updatedList.Count > 0)
                {

                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Update;
                    logData.TableName = TableName.C_TBL_NAME_MONTHLY_HISTORY;
                    logData.TableData = CommonUtil.ConvertToXml(updatedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return updatedList.Count;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// To update tbt_MonthlyBillingHistory
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public List<tbt_MonthlyBillingHistory> UpdateTbt_MonthlyBillingHistoryData(List<tbt_MonthlyBillingHistory> data)
        {

            try
            {

                List<tbt_MonthlyBillingHistory> updatedList = base.UpdateTbt_MonthlyBillingHistoryData(CommonUtil.ConvertToXml_Store<tbt_MonthlyBillingHistory>(data));

                //Insert Log
                if (updatedList.Count > 0)
                {

                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Update;
                    logData.TableName = TableName.C_TBL_NAME_MONTHLY_HISTORY;
                    logData.TableData = CommonUtil.ConvertToXml(updatedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return updatedList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// To insert tbt_MonthlyBillingHistory
        /// </summary>
        /// <param name="doInsertMonthlyBillingHistory"></param>
        /// <returns></returns>
        public int InsertTbt_MonthlyBillingHistory(tbt_MonthlyBillingHistory doInsertMonthlyBillingHistory)
        {
            try
            {
                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                doInsertMonthlyBillingHistory.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsertMonthlyBillingHistory.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                doInsertMonthlyBillingHistory.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsertMonthlyBillingHistory.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_MonthlyBillingHistory> doInsertList = new List<tbt_MonthlyBillingHistory>();
                doInsertList.Add(doInsertMonthlyBillingHistory);
                List<tbt_MonthlyBillingHistory> insertList = base.InsertTbt_MonthlyBillingHistory(CommonUtil.ConvertToXml_Store<tbt_MonthlyBillingHistory>(doInsertList));

                //Insert Log
                if (insertList.Count > 0)
                {

                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_MONTHLY_HISTORY;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList.Count;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region BLP011 Manage Billing Detail & Manage Billing Detail By Contract
        /// <summary>
        /// To create billing detail when billing send command
        /// </summary>
        /// <param name="doBillingDetail"></param>
        /// <returns></returns>
        public tbt_BillingDetail ManageBillingDetail(tbt_BillingDetail doBillingDetail)
        {

            tbt_BillingDetail dtBillingDetail = new tbt_BillingDetail();

            try
            {
                bool? bFirstFeeFlag = false;

                IBillingMasterHandler hand = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;
                //1.
                int iRowCount = 0;
                List<string> lst = new List<string>();
                if (CommonUtil.IsNullOrEmpty(doBillingDetail.ContractCode))
                {
                    //lst.Add("ContractCode");
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "ContractCode" });
                }
                if (CommonUtil.IsNullOrEmpty(doBillingDetail.BillingOCC))
                {
                    //lst.Add("BillingOCC");
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "BillingOCC" });
                }


                //3.
                tbt_BillingDetail doTbt_BillingDetail = new tbt_BillingDetail();
                //4.
                if (doBillingDetail.FirstFeeFlag == null)
                {
                    if ((doBillingDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_SERVICE
                        || doBillingDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_DIFF_SERVICE
                        || doBillingDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_MA
                        || doBillingDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_DIFF_MA
                        || doBillingDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_SG
                        || doBillingDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_DIFF_SG
                        || doBillingDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_MA_RESULT_BASE
                        || doBillingDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_DIFF_MA_RESULT_BASE)
                        && (doBillingDetail.BillingStartDate == doBillingDetail.StartOperationDate))
                    {
                        bFirstFeeFlag = true;
                    }
                    else if ((doBillingDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_DEPOSIT
                        || doBillingDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_INSTRUMENT_SETUP
                        || doBillingDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_CHANGE_INSTALL)
                        && (doBillingDetail.BillingStartDate <= doBillingDetail.StartOperationDate
                            || doBillingDetail.StartOperationDate == null))
                    {
                        bFirstFeeFlag = true;
                    }
                    else
                    {
                        bFirstFeeFlag = false;
                    }
                }
                else
                {
                    bFirstFeeFlag = doBillingDetail.FirstFeeFlag;
                }

                // check over Digit before create tbt_invoice

                if (doBillingDetail.BillingAmount > (decimal?)999999999999.99)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_BILLING, MessageUtil.MessageList.MSG6083);
                }

                //5.
                tbt_BillingBasic doTbt_BillingBasic = new tbt_BillingBasic();
                doTbt_BillingDetail.ContractCode = doBillingDetail.ContractCode;
                doTbt_BillingDetail.BillingOCC = doBillingDetail.BillingOCC;

                // Edit by Narupon W. March 22, 2012 => BillingDetailNo is setted in insert_procedure by get MAX()+1 (group by ContractCode, BillingOCC)
                doTbt_BillingDetail.BillingDetailNo = 0;//GetBillingDetailNoData(doBillingDetail.ContractCode, doBillingDetail.BillingOCC);
                doTbt_BillingDetail.InvoiceNo = doBillingDetail.InvoiceNo;
                doTbt_BillingDetail.InvoiceOCC = doBillingDetail.InvoiceOCC;
                doTbt_BillingDetail.IssueInvDate = doBillingDetail.IssueInvDate;
                doTbt_BillingDetail.IssueInvFlag = doBillingDetail.IssueInvFlag;
                doTbt_BillingDetail.BillingTypeCode = doBillingDetail.BillingTypeCode;
                doTbt_BillingDetail.BillingAmount = doBillingDetail.BillingAmount;
                doTbt_BillingDetail.BillingAmountCurrencyType = doBillingDetail.BillingAmountCurrencyType; // add by Jirawat Jannet
                doTbt_BillingDetail.BillingAmountUsd = doBillingDetail.BillingAmountUsd; // add by Jirawat Jannet

                doTbt_BillingDetail.AdjustBillingAmountCurrencyType = doBillingDetail.AdjustBillingAmountCurrencyType;
                doTbt_BillingDetail.AdjustBillingAmount = doBillingDetail.AdjustBillingAmount;
                doTbt_BillingDetail.AdjustBillingAmountUsd = doBillingDetail.AdjustBillingAmountUsd;

                doTbt_BillingDetail.BillingStartDate = doBillingDetail.BillingStartDate;
                doTbt_BillingDetail.BillingEndDate = doBillingDetail.BillingEndDate;
                doTbt_BillingDetail.PaymentMethod = CommonUtil.IsNullOrEmpty(doBillingDetail.PaymentMethod) ? PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER : doBillingDetail.PaymentMethod;
                doTbt_BillingDetail.PaymentStatus = doBillingDetail.PaymentStatus;
                doTbt_BillingDetail.AutoTransferDate = doBillingDetail.AutoTransferDate;
                doTbt_BillingDetail.FirstFeeFlag = bFirstFeeFlag;
                doTbt_BillingDetail.DelayedMonth = doBillingDetail.DelayedMonth;
                doTbt_BillingDetail.ContractOCC = doBillingDetail.ContractOCC;
                doTbt_BillingDetail.ForceIssueFlag = doBillingDetail.ForceIssueFlag;
                //6.
                //iRowCount = CreateTbt_BillingDetail(doTbt_BillingDetail);

                var billingDetail_inserted = CreateTbt_BillingDetail(doTbt_BillingDetail);

                if (billingDetail_inserted.Count > 0)
                {
                    dtBillingDetail = billingDetail_inserted[0];
                }

                //7.
                tbm_BillingType doTbm_BillingType = new tbm_BillingType();
                if (CommonUtil.IsNullOrEmpty(doBillingDetail.BillingTypeGroup))
                {
                    doTbm_BillingType = hand.GetTbm_BillingType(doBillingDetail.BillingTypeCode);
                    if (doTbm_BillingType != null)
                    {
                        doBillingDetail.BillingTypeGroup = doTbm_BillingType.BillingTypeGroup;
                    }
                }

                //8.             
                if (doBillingDetail.BillingTypeGroup == BillingTypeGroup.C_BILLING_TYPE_GROUP_CONTINUES)
                {
                    //8.1
                    List<tbt_BillingBasic> result = base.GetTbt_BillingBasic(doBillingDetail.ContractCode, doBillingDetail.BillingOCC);
                    if (result.Count > 0)
                    {
                        doTbt_BillingBasic = result[0];
                    }
                    else
                    {
                        doTbt_BillingBasic = null;
                    }
                    //doTbt_BillingBasic = GetTbt_BillingBasic(doBillingDetail.ContractCode, doBillingDetail.BillingOCC);         
                    //8.2
                    if (doTbt_BillingBasic.LastBillingDate < doBillingDetail.BillingEndDate
                            || (doTbt_BillingBasic.LastBillingDate == null && doBillingDetail.BillingEndDate != null))
                    {
                        //8.2.1
                        doTbt_BillingBasic.LastBillingDate = doBillingDetail.BillingEndDate;
                        //8.2.2
                        if (doTbt_BillingBasic.AdjustEndDate == doBillingDetail.BillingEndDate)
                        {
                            doTbt_BillingBasic.AdjustEndDate = null;
                        }

                        //8.2.3
                        iRowCount = UpdateTbt_BillingBasic(doTbt_BillingBasic);
                    }
                }
                //9.
                return dtBillingDetail;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //Add by Jutarat A. on 07052013
        /// <summary>
        /// To create list of billing detail when billing send command
        /// </summary>
        /// <param name="doBillingDetailList"></param>
        /// <returns></returns>
        public List<tbt_BillingDetail> ManageBillingDetail(List<tbt_BillingDetail> doBillingDetailList)
        {
            List<tbt_BillingDetail> addBillingDetailList = new List<tbt_BillingDetail>();
            List<tbt_BillingDetail> resultBillingDetailList = new List<tbt_BillingDetail>();
            List<tbt_BillingBasic> updateBillingBasicList = new List<tbt_BillingBasic>();

            try
            {
                bool? bFirstFeeFlag = false;
                IBillingMasterHandler hand = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;

                foreach (var billingDetail in doBillingDetailList)
                {
                    bFirstFeeFlag = false;

                    //1.
                    List<string> lst = new List<string>();
                    if (CommonUtil.IsNullOrEmpty(billingDetail.ContractCode))
                    {
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "ContractCode" });
                    }
                    if (CommonUtil.IsNullOrEmpty(billingDetail.BillingOCC))
                    {
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "BillingOCC" });
                    }

                    //3.
                    tbt_BillingDetail doTbt_BillingDetail = new tbt_BillingDetail();

                    //4.
                    if (billingDetail.FirstFeeFlag == null)
                    {
                        if ((billingDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_SERVICE
                            || billingDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_DIFF_SERVICE
                            || billingDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_MA
                            || billingDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_DIFF_MA
                            || billingDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_SG
                            || billingDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_DIFF_SG
                            || billingDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_MA_RESULT_BASE
                            || billingDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_DIFF_MA_RESULT_BASE)
                            && (billingDetail.BillingStartDate == billingDetail.StartOperationDate))
                        {
                            bFirstFeeFlag = true;
                        }
                        else if ((billingDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_DEPOSIT
                            || billingDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_INSTRUMENT_SETUP
                            || billingDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_CHANGE_INSTALL)
                            && (billingDetail.BillingStartDate <= billingDetail.StartOperationDate
                                || billingDetail.StartOperationDate == null))
                        {
                            bFirstFeeFlag = true;
                        }
                        else
                        {
                            bFirstFeeFlag = false;
                        }
                    }
                    else
                    {
                        bFirstFeeFlag = billingDetail.FirstFeeFlag;
                    }

                    // check over Digit before create tbt_invoice
                    if (billingDetail.BillingAmount > (decimal?)999999999999.99)
                    {
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_BILLING, MessageUtil.MessageList.MSG6083);
                    }

                    //5.
                    tbt_BillingBasic doTbt_BillingBasic = new tbt_BillingBasic();
                    doTbt_BillingDetail.ContractCode = billingDetail.ContractCode;
                    doTbt_BillingDetail.BillingOCC = billingDetail.BillingOCC;

                    // Edit by Narupon W. March 22, 2012 => BillingDetailNo is setted in insert_procedure by get MAX()+1 (group by ContractCode, BillingOCC)
                    doTbt_BillingDetail.BillingDetailNo = 0;//GetBillingDetailNoData(doBillingDetail.ContractCode, doBillingDetail.BillingOCC);
                    doTbt_BillingDetail.InvoiceNo = billingDetail.InvoiceNo;
                    doTbt_BillingDetail.InvoiceOCC = billingDetail.InvoiceOCC;
                    doTbt_BillingDetail.IssueInvDate = billingDetail.IssueInvDate;
                    doTbt_BillingDetail.IssueInvFlag = billingDetail.IssueInvFlag;
                    doTbt_BillingDetail.BillingTypeCode = billingDetail.BillingTypeCode;

                    doTbt_BillingDetail.BillingAmountCurrencyType = billingDetail.BillingAmountCurrencyType;
                    doTbt_BillingDetail.BillingAmount = billingDetail.BillingAmount;
                    doTbt_BillingDetail.BillingAmountUsd = billingDetail.BillingAmountUsd;

                    doTbt_BillingDetail.AdjustBillingAmountCurrencyType = billingDetail.AdjustBillingAmountCurrencyType;
                    doTbt_BillingDetail.AdjustBillingAmount = billingDetail.AdjustBillingAmount;
                    doTbt_BillingDetail.AdjustBillingAmountUsd = billingDetail.AdjustBillingAmountUsd;

                    doTbt_BillingDetail.BillingStartDate = billingDetail.BillingStartDate;
                    doTbt_BillingDetail.BillingEndDate = billingDetail.BillingEndDate;
                    doTbt_BillingDetail.PaymentMethod = CommonUtil.IsNullOrEmpty(billingDetail.PaymentMethod) ? PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER : billingDetail.PaymentMethod;
                    doTbt_BillingDetail.PaymentStatus = billingDetail.PaymentStatus;
                    doTbt_BillingDetail.AutoTransferDate = billingDetail.AutoTransferDate;
                    doTbt_BillingDetail.FirstFeeFlag = bFirstFeeFlag;
                    doTbt_BillingDetail.DelayedMonth = billingDetail.DelayedMonth;
                    doTbt_BillingDetail.ContractOCC = billingDetail.ContractOCC;
                    doTbt_BillingDetail.ForceIssueFlag = billingDetail.ForceIssueFlag;

                    addBillingDetailList.Add(doTbt_BillingDetail);

                    //7.
                    tbm_BillingType doTbm_BillingType = new tbm_BillingType();
                    if (CommonUtil.IsNullOrEmpty(billingDetail.BillingTypeGroup))
                    {
                        doTbm_BillingType = hand.GetTbm_BillingType(billingDetail.BillingTypeCode);
                        if (doTbm_BillingType != null)
                        {
                            billingDetail.BillingTypeGroup = doTbm_BillingType.BillingTypeGroup;
                        }
                    }

                    //8.             
                    if (billingDetail.BillingTypeGroup == BillingTypeGroup.C_BILLING_TYPE_GROUP_CONTINUES)
                    {
                        //8.1
                        List<tbt_BillingBasic> result = base.GetTbt_BillingBasic(billingDetail.ContractCode, billingDetail.BillingOCC);
                        if (result.Count > 0)
                        {
                            doTbt_BillingBasic = result[0];
                        }
                        else
                        {
                            doTbt_BillingBasic = null;
                        }

                        //8.2
                        if (doTbt_BillingBasic.LastBillingDate < billingDetail.BillingEndDate)
                        {
                            //8.2.1
                            doTbt_BillingBasic.LastBillingDate = billingDetail.BillingEndDate;
                            //8.2.2
                            if (doTbt_BillingBasic.AdjustEndDate == billingDetail.BillingEndDate)
                            {
                                doTbt_BillingBasic.AdjustEndDate = null;
                            }

                            //8.2.3
                            updateBillingBasicList.Add(doTbt_BillingBasic);
                        }
                    }
                }

                //6.
                if (addBillingDetailList != null && addBillingDetailList.Count > 0)
                    resultBillingDetailList = CreateTbt_BillingDetail(addBillingDetailList);

                if (updateBillingBasicList != null && updateBillingBasicList.Count > 0)
                    UpdateTbt_BillingBasic(updateBillingBasicList);

                //9.
                return resultBillingDetailList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //End Add

        /// <summary>
        /// To insert data to tbt_BillingDetail
        /// </summary>
        /// <param name="billingDetail"></param>
        /// <returns></returns>
        public List<tbt_BillingDetail> CreateTbt_BillingDetail(tbt_BillingDetail billingDetail)
        {
            try
            {
                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                billingDetail.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                billingDetail.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                billingDetail.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                billingDetail.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_BillingDetail> doInsertList = new List<tbt_BillingDetail>();
                doInsertList.Add(billingDetail);
                List<tbt_BillingDetail> insertList = base.InsertTbt_BillingDetail(CommonUtil.ConvertToXml_Store<tbt_BillingDetail>(doInsertList));


                //Insert Log
                if (insertList.Count > 0)
                {

                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_BILLING_DETAIL;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Add by Jutarat A. on 30042013
        /// <summary>
        /// To insert data to list of tbt_BillingDetail
        /// </summary>
        /// <param name="billingDetail"></param>
        /// <returns></returns>
        public List<tbt_BillingDetail> CreateTbt_BillingDetail(List<tbt_BillingDetail> billingDetailList)
        {
            try
            {
                if (billingDetailList != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    foreach (tbt_BillingDetail billingDetail in billingDetailList)
                    {
                        billingDetail.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                        billingDetail.CreateBy = dsTrans.dtUserData.EmpNo;
                        billingDetail.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                        billingDetail.UpdateBy = dsTrans.dtUserData.EmpNo;
                    }
                }

                List<tbt_BillingDetail> insertList = base.InsertTbt_BillingDetail(CommonUtil.ConvertToXml_Store<tbt_BillingDetail>(billingDetailList));

                //Insert Log
                if (insertList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_BILLING_DETAIL;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        //End Add

        /// <summary>
        /// To delete data of tbt_BillingDetail
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strBillingOCC"></param>
        /// <param name="intBillingDetailNo"></param>
        /// <returns></returns>
        public List<tbt_BillingDetail> DeleteTbt_BillingDetail(string strContractCode, string strBillingOCC, int intBillingDetailNo)
        {
            try
            {
                //Delete data from DB
                List<tbt_BillingDetail> deletedList = base.DeleteTbt_BillingDetailData(strContractCode, strBillingOCC, intBillingDetailNo);

                //Insert Log
                if (deletedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                    logData.TableName = TableName.C_TBL_NAME_BILLING_DETAIL;
                    logData.TableData = CommonUtil.ConvertToXml(deletedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return deletedList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// To update tbt_BillingDetail
        /// </summary>
        /// <param name="billingDetail"></param>
        /// <returns></returns>
        public int Updatetbt_BillingDetail(tbt_BillingDetail billingDetail, DateTime? dtUpdateDate = null) //Modify (Add dtUpdateDate) by Jutarat A. on 25112013
        {
            try
            {
                //Add by Jutarat A. on 25112013
                if (dtUpdateDate != null)
                {
                    List<tbt_BillingDetail> billingDatialList = this.GetTbt_BillingDetailData(billingDetail.ContractCode, billingDetail.BillingOCC, billingDetail.BillingDetailNo);
                    if (billingDatialList != null && billingDatialList.Count > 0)
                    {
                        if (DateTime.Compare(dtUpdateDate.GetValueOrDefault(), billingDatialList[0].UpdateDate.GetValueOrDefault()) != 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                        }
                    }
                }
                //End Add

                //set updateDate and updateBy
                billingDetail.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                billingDetail.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_BillingDetail> tbt_BillingDetail = new List<tbt_BillingDetail>();
                tbt_BillingDetail.Add(billingDetail);
                List<tbt_BillingDetail> updatedList = base.UpdateTbt_BillingDetailData(CommonUtil.ConvertToXml_Store<tbt_BillingDetail>(tbt_BillingDetail));

                //Insert Log
                if (updatedList.Count > 0)
                {

                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Update;
                    logData.TableName = TableName.C_TBL_NAME_BILLING_DETAIL;
                    logData.TableData = CommonUtil.ConvertToXml(updatedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return updatedList.Count;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //Add by Jutarat A. on 29042013
        /// <summary>
        /// To update list of tbt_BillingDetail
        /// </summary>
        /// <param name="billingDetailList"></param>
        /// <returns></returns>
        public List<tbt_BillingDetail> Updatetbt_BillingDetail(List<tbt_BillingDetail> billingDetailList, bool isCheckUpdateDate = false) //Modify (Add isCheckUpdateDate) by Jutarat A. on 25112013
        {
            try
            {
                //Add by Jutarat A. on 25112013
                if (isCheckUpdateDate)
                {
                    foreach (tbt_BillingDetail billingDetail in billingDetailList)
                    {
                        List<tbt_BillingDetail> billingData = this.GetTbt_BillingDetailData(billingDetail.ContractCode, billingDetail.BillingOCC, billingDetail.BillingDetailNo);
                        if (billingData != null && billingData.Count > 0)
                        {
                            if (DateTime.Compare(billingDetail.UpdateDate.GetValueOrDefault(), billingData[0].UpdateDate.GetValueOrDefault()) != 0)
                            {
                                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                            }
                        }
                    }
                }
                //End Add

                //set updateDate and updateBy
                if (billingDetailList != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    foreach (tbt_BillingDetail billingDetail in billingDetailList)
                    {
                        billingDetail.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                        billingDetail.UpdateBy = dsTrans.dtUserData.EmpNo;
                    }
                }

                List<tbt_BillingDetail> updatedList = base.UpdateTbt_BillingDetailData(CommonUtil.ConvertToXml_Store<tbt_BillingDetail>(billingDetailList));

                //Insert Log
                if (updatedList.Count > 0)
                {

                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Update;
                    logData.TableName = TableName.C_TBL_NAME_BILLING_DETAIL;
                    logData.TableData = CommonUtil.ConvertToXml(updatedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return updatedList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //End Add

        //public List<tbt_BillingDetail> GetTbt_BillingDetailData(string strContractCode, string strBillingOCC, int intBillingDetailNo)
        //{
        //    return GetTbt_BillingDetailData(strContractCode, strBillingOCC, (int?)intBillingDetailNo);
        //}

        //public List<tbt_BillingDetail> GetTbt_BillingDetailData(string strContractCode, string strBillingOCC, int? intBillingDetailNo)
        //{
        //    try
        //    {
        //        List<tbt_BillingDetail> result = base.GetTbt_BillingDetailData(strContractCode, strBillingOCC, intBillingDetailNo);
        //        if (result.Count > 0)
        //        {
        //            return result;
        //        }
        //        else
        //            return null;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public tbt_BillingBasic GetTbt_BillingBasic(string ContractCode, string BillingOCC)
        //{
        //    try
        //    {
        //        List<tbt_BillingBasic> result = base.GetTbt_BillingBasicData(ContractCode, BillingOCC);
        //        if (result.Count > 0)
        //        {
        //            return result[0];
        //        }
        //        else
        //            return null;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        /// <summary>
        /// To create billing detail when contract send command
        /// </summary>
        /// <param name="doBillingTempDetailListData"></param>
        /// <returns></returns>
        public List<doBillingTempDetail> ManageBillingDetailByContract(List<doBillingTempDetail> doBillingTempDetailListData)
        {
            try
            {
                string strMappingBillingType = null;
                tbt_BillingBasic doTbt_BillingBasic = new tbt_BillingBasic();
                tbt_BillingTypeDetail doTbt_BillingTypeDetail = new tbt_BillingTypeDetail();
                tbm_BillingType doTbm_BillingType = new tbm_BillingType();
                IBillingMasterHandler hand = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;
                bool iResult = false;
                bool bFirstFeeFlag = false;
                DateTime? dtIssueInvDate = null;
                DateTime? dtAutotransferDate = null;

                foreach (doBillingTempDetail doBillingTempDetailList in doBillingTempDetailListData)
                {
                    //1.1                    
                    List<string> lst = new List<string>();
                    if (CommonUtil.IsNullOrEmpty(doBillingTempDetailList.ContractCode))
                    {
                        //lst.Add("ContractCode");
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "ContractCode" });
                    }
                    if (CommonUtil.IsNullOrEmpty(doBillingTempDetailList.BillingOCC))
                    {
                        //lst.Add("BillingOOC");
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "BillingOCC" });
                    }
                }
                foreach (doBillingTempDetail doBillingTempDetailList in doBillingTempDetailListData)
                {

                    //1.2
                    List<tbt_BillingBasic> result = base.GetTbt_BillingBasic(doBillingTempDetailList.ContractCode, doBillingTempDetailList.BillingOCC);
                    if (result.Count > 0)
                    {
                        doTbt_BillingBasic = result[0];
                    }
                    else
                    {
                        doTbt_BillingBasic = new tbt_BillingBasic(); //null;
                    }
                    //doTbt_BillingBasic = GetTbt_BillingBasic(doBillingTempDetailList.ContractCode, doBillingTempDetailList.BillingOCC);
                    //1.3
                    strMappingBillingType = MappingBillingType(doBillingTempDetailList.ContractBillingType, doBillingTempDetailList.BillingTiming, doBillingTempDetailList.ProductTypeCode);

                    if (string.IsNullOrEmpty(strMappingBillingType))
                    {
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "strMappingBillingType" });
                    }

                    //1.4                    
                    doTbt_BillingTypeDetail = GetTbt_BillingTypeDetailData(doBillingTempDetailList.ContractCode, doBillingTempDetailList.BillingOCC, strMappingBillingType);
                    //1.5
                    if (doTbt_BillingTypeDetail == null)
                    {
                        //1.5.1
                        doTbm_BillingType = hand.GetTbm_BillingType(strMappingBillingType);
                        //1.5.2
                        doTbt_BillingTypeDetail = new tbt_BillingTypeDetail();
                        doTbt_BillingTypeDetail.ContractCode = doBillingTempDetailList.ContractCode;
                        doTbt_BillingTypeDetail.BillingOCC = doBillingTempDetailList.BillingOCC;
                        doTbt_BillingTypeDetail.BillingTypeCode = strMappingBillingType;
                        doTbt_BillingTypeDetail.InvoiceDescriptionEN = null;
                        doTbt_BillingTypeDetail.InvoiceDescriptionLC = null;
                        doTbt_BillingTypeDetail.IssueInvoiceFlag = true;
                        doTbt_BillingTypeDetail.ProductCode = doBillingTempDetailList.ProductCode;
                        doTbt_BillingTypeDetail.ProductTypeCode = doBillingTempDetailList.ProductTypeCode;
                        if (doTbm_BillingType != null)
                        {
                            doTbt_BillingTypeDetail.BillingTypeGroup = doTbm_BillingType.BillingTypeGroup;
                        }
                        iResult = CreateBillingTypeDetail(doTbt_BillingTypeDetail);
                    }
                    //1.6
                    tbt_BillingDetail doTbt_BillingDetail = new tbt_BillingDetail();
                    //1.7
                    if ((strMappingBillingType == BillingType.C_BILLING_TYPE_SERVICE
                          || strMappingBillingType == BillingType.C_BILLING_TYPE_DIFF_SERVICE
                          || strMappingBillingType == BillingType.C_BILLING_TYPE_MA
                          || strMappingBillingType == BillingType.C_BILLING_TYPE_DIFF_MA
                          || strMappingBillingType == BillingType.C_BILLING_TYPE_SG
                          || strMappingBillingType == BillingType.C_BILLING_TYPE_DIFF_SG
                          || strMappingBillingType == BillingType.C_BILLING_TYPE_MA_RESULT_BASE
                          || strMappingBillingType == BillingType.C_BILLING_TYPE_DIFF_MA_RESULT_BASE)
                          && (doBillingTempDetailList.BillingDate == doTbt_BillingBasic.StartOperationDate))
                    {
                        bFirstFeeFlag = true;
                    }
                    else if ((strMappingBillingType == BillingType.C_BILLING_TYPE_DEPOSIT
                        || strMappingBillingType == BillingType.C_BILLING_TYPE_INSTRUMENT_SETUP
                        || strMappingBillingType == BillingType.C_BILLING_TYPE_CHANGE_INSTALL)
                        && (doBillingTempDetailList.BillingDate <= doTbt_BillingBasic.StartOperationDate
                            || doTbt_BillingBasic.StartOperationDate == null))
                    {
                        bFirstFeeFlag = true;
                    }
                    else if ((strMappingBillingType == BillingType.C_BILLING_TYPE_SALE_PRICE
                        || strMappingBillingType == BillingType.C_BILLING_TYPE_SALE_PRICE_PARTIAL
                        || strMappingBillingType == BillingType.C_BILLING_TYPE_SALE_PRICE_DOWN)
                        && doBillingTempDetailList.OCC == OCCType.C_FIRST_SALE_CONTRACT_OCC)
                    {
                        bFirstFeeFlag = true;
                    }
                    else
                    {
                        bFirstFeeFlag = false;
                    }
                    //1.8
                    if (doBillingTempDetailList.PaymentMethod == PaymentMethodType.C_PAYMENT_METHOD_AUTO_TRANFER
                        || doBillingTempDetailList.PaymentMethod == PaymentMethodType.C_PAYMENT_METHOD_CREDIT_CARD)
                    {
                        dtAutotransferDate = GetNextAutoTransferDate(doBillingTempDetailList.ContractCode, doBillingTempDetailList.BillingOCC, doBillingTempDetailList.PaymentMethod);
                        // Edit by Patcharee T. Jul 25, 2012 
                        if (dtAutotransferDate == null)
                        {
                            doBillingTempDetailList.PaymentMethod = PaymentMethodType.C_PAYMENT_METHOD_BANK_TRANSFER;
                            dtIssueInvDate = DateTime.Now;
                        }
                        else
                        {
                            if (dtAutotransferDate.Value.AddDays(-30) < DateTime.Now)
                            {
                                dtIssueInvDate = DateTime.Now;
                            }
                            else
                            {
                                dtIssueInvDate = dtAutotransferDate.Value.AddDays(-30); // issue invoice 30 days before auto-transfer date
                            }
                        }
                    }
                    else
                    {
                        dtIssueInvDate = DateTime.Now;
                    }
                    //1.9
                    doTbt_BillingDetail.ContractCode = doBillingTempDetailList.ContractCode;
                    doTbt_BillingDetail.BillingOCC = doBillingTempDetailList.BillingOCC;

                    // Edit by Narupon W. March 22, 2012 => BillingDetailNo is setted in insert_procedure  by get MAX()+1 (group by ContractCode, BillingOCC)
                    doTbt_BillingDetail.BillingDetailNo = 0;// GetBillingDetailNoData(doBillingTempDetailList.ContractCode, doBillingTempDetailList.BillingOCC);
                    doTbt_BillingDetail.InvoiceNo = null;
                    doTbt_BillingDetail.InvoiceOCC = null;
                    if (strMappingBillingType != BillingType.C_BILLING_TYPE_SALE_PRICE_PARTIAL)
                    {
                        doTbt_BillingDetail.IssueInvDate = dtIssueInvDate;
                    }
                    else
                    {
                        doTbt_BillingDetail.IssueInvDate = null; // issue invoice of partial fee by CF-13
                    }
                    doTbt_BillingDetail.IssueInvFlag = true;
                    doTbt_BillingDetail.BillingTypeCode = strMappingBillingType;

                    doTbt_BillingDetail.BillingAmountCurrencyType = doBillingTempDetailList.BillingAmountCurrencyType;
                    doTbt_BillingDetail.BillingAmount = doBillingTempDetailList.BillingAmount;
                    doTbt_BillingDetail.BillingAmountUsd = doBillingTempDetailList.BillingAmountUsd;

                    doTbt_BillingDetail.AdjustBillingAmountCurrencyType = null;
                    doTbt_BillingDetail.AdjustBillingAmount = null;
                    doTbt_BillingDetail.AdjustBillingAmountUsd = null;

                    doTbt_BillingDetail.BillingStartDate = doBillingTempDetailList.BillingDate;
                    doTbt_BillingDetail.BillingEndDate = null;
                    doTbt_BillingDetail.PaymentMethod = CommonUtil.IsNullOrEmpty(doBillingTempDetailList.PaymentMethod) ? PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER : doBillingTempDetailList.PaymentMethod; // Edit by Patcharee T. Jul 25, 2012 
                    if (doBillingTempDetailList.PaymentMethod == PaymentMethodType.C_PAYMENT_METHOD_AUTO_TRANFER
                        || doBillingTempDetailList.PaymentMethod == PaymentMethodType.C_PAYMENT_METHOD_CREDIT_CARD)
                    {
                        doTbt_BillingDetail.PaymentStatus = PaymentStatus.C_PAYMENT_STATUS_DETAIL_AUTO_CREDIT;
                    }
                    else
                    {
                        doTbt_BillingDetail.PaymentStatus = PaymentStatus.C_PAYMENT_STATUS_DETAIL_BANK_COLLECT;
                    }
                    if (doBillingTempDetailList.PaymentMethod == PaymentMethodType.C_PAYMENT_METHOD_AUTO_TRANFER
                        || doBillingTempDetailList.PaymentMethod == PaymentMethodType.C_PAYMENT_METHOD_CREDIT_CARD)
                    {
                        doTbt_BillingDetail.AutoTransferDate = dtAutotransferDate;
                    }
                    else
                    {
                        doTbt_BillingDetail.AutoTransferDate = null;
                    }
                    doTbt_BillingDetail.FirstFeeFlag = bFirstFeeFlag;
                    doTbt_BillingDetail.DelayedMonth = null;
                    doTbt_BillingDetail.ContractOCC = doBillingTempDetailList.OCC;
                    doTbt_BillingDetail.ForceIssueFlag = false;

                    if (string.IsNullOrEmpty(doTbt_BillingDetail.ContractOCC))
                    {
                        IRentralContractHandler rentalContralHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                        List<dtTbt_RentalContractBasicForView> dtRentalContract = rentalContralHandler.GetTbt_RentalContractBasicForView(doTbt_BillingDetail.ContractCode);
                        if (dtRentalContract.Count > 0)
                        {
                            doTbt_BillingDetail.ContractOCC = dtRentalContract[0].LastOCC;
                        }
                        else
                        {
                            ISaleContractHandler saleHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                            List<tbt_SaleBasic> dtSaleBasic = saleHandler.GetTbt_SaleBasic(doTbt_BillingDetail.ContractCode, null, true);
                            if (dtSaleBasic != null && dtSaleBasic.Count > 0)
                            {
                                doTbt_BillingDetail.ContractOCC = dtSaleBasic[0].OCC;
                            }
                        }
                    }

                    //1.10
                    CreateTbt_BillingDetail(doTbt_BillingDetail);
                }
                return doBillingTempDetailListData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get next auto transfer date
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strBillingOCC"></param>
        /// <param name="strPaymentMethod"></param>
        /// <returns></returns>
        public DateTime? GetNextAutoTransferDate(string strContractCode, string strBillingOCC, string strPaymentMethod)
        {
            try
            {
                //1.
                tbt_AutoTransferBankAccount doTbt_AutoTransferBankAccount = new tbt_AutoTransferBankAccount();
                tbt_CreditCard doTbt_CreditCard = new tbt_CreditCard();
                string strBankCode = null;
                string iAutoTransferDate = null;

                List<string> lst = new List<string>();
                if (CommonUtil.IsNullOrEmpty(strContractCode))
                {
                    //lst.Add("ContractCode");
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "ContractCode" });
                }
                if (CommonUtil.IsNullOrEmpty(strBillingOCC))
                {
                    //lst.Add("BillingOOC");
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "BillingOCC" });
                }
                //2.
                if (strPaymentMethod == PaymentMethodType.C_PAYMENT_METHOD_AUTO_TRANFER)
                {
                    doTbt_AutoTransferBankAccount = GetTbt_AutoTransferBankAccountData(strContractCode, strBillingOCC);
                    if (doTbt_AutoTransferBankAccount != null)
                    {
                        iAutoTransferDate = doTbt_AutoTransferBankAccount.AutoTransferDate;
                        strBankCode = doTbt_AutoTransferBankAccount.BankCode;
                    }
                    else
                    {
                        return null;
                    }
                }
                //3.
                else if (strPaymentMethod == PaymentMethodType.C_PAYMENT_METHOD_CREDIT_CARD)
                {
                    //3.1
                    doTbt_CreditCard = GetTbt_CreditCardData(strContractCode, strBillingOCC);
                    //3.2
                    if (doTbt_CreditCard != null)
                    {
                        iAutoTransferDate = null;
                        strBankCode = doTbt_CreditCard.CreditCardCompanyCode;
                    }
                    else
                    {
                        return null;
                    }
                }
                //4.
                else
                {
                    return null;
                }
                //5.
                List<DateTime?> dtNextAutoTransferDate = base.GetAutoTransferDate(strBankCode, iAutoTransferDate);
                if (dtNextAutoTransferDate.Count > 0)
                {
                    return dtNextAutoTransferDate[0];
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// To get data of tbt_AutoTransferBankAccount
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="BillingOCC"></param>
        /// <returns></returns>
        public tbt_AutoTransferBankAccount GetTbt_AutoTransferBankAccountData(string ContractCode, string BillingOCC)
        {
            try
            {
                List<tbt_AutoTransferBankAccount> result = base.GetTbt_AutoTransferBankAccount(ContractCode, BillingOCC);
                if (result.Count > 0)
                {
                    return result[0];
                }
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get data of tbt_CreditCard
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="BillingOCC"></param>
        /// <returns></returns>
        public tbt_CreditCard GetTbt_CreditCardData(string ContractCode, string BillingOCC)
        {
            try
            {
                List<tbt_CreditCard> result = base.GetTbt_CreditCard(ContractCode, BillingOCC);
                if (result.Count > 0)
                {
                    return result[0];
                }
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// To get Billing detail No.
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strBillingOCC"></param>
        /// <returns></returns>
        public int GetBillingDetailNoData(string strContractCode, string strBillingOCC)
        {
            try
            {
                List<Nullable<int>> lstInt = new List<Nullable<int>>();
                int iBillingDetailNo = 0;
                lstInt = base.GetBillingDetailNo(strContractCode, strBillingOCC);
                if (lstInt.Count > 0)
                {
                    iBillingDetailNo = Convert.ToInt32(lstInt[0]) + 1;
                }
                return iBillingDetailNo;
            }
            catch (Exception)
            {
                throw;
            }

        }
        #endregion

        #region BLP012 ManageBillingBasicForStart
        /// <summary>
        /// To update billing basic when contract send command for start service
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="dtStartServiceDate"></param>
        /// <param name="dtAdjustDate"></param>
        /// <returns></returns>
        public bool ManageBillingBasicForStart(string strContractCode, DateTime? dtStartServiceDate, DateTime? dtAdjustDate)
        {
            try
            {
                List<tbt_BillingBasic> doTbt_BillingBasicListData = base.GetTbt_BillingBasicListData(strContractCode);
                tbt_MonthlyBillingHistory doTbt_BillingHistory = new tbt_MonthlyBillingHistory();
                decimal dBillngAmount = 0;
                DateTime? dtAutoTransferDate = null;
                BillingTypeDetail doBillingTypeDetail = new BillingTypeDetail();

                foreach (tbt_BillingBasic doTbt_BillingBasicList in doTbt_BillingBasicListData)
                {
                    //2.1
                    if ((Convert.ToDecimal(doTbt_BillingBasicList.MonthlyBillingAmount) > 0|| Convert.ToDecimal(doTbt_BillingBasicList.MonthlyBillingAmountUsd) > 0) && dtStartServiceDate.HasValue && (doTbt_BillingBasicList.ResultBasedMaintenanceFlag ?? false) == false)
                    {
                        //2.1.1
                        doTbt_BillingBasicList.StartOperationDate = dtStartServiceDate;
                        doTbt_BillingBasicList.AdjustEndDate = dtAdjustDate;
                        doTbt_BillingBasicList.StopBillingFlag = false;
                        doTbt_BillingBasicList.LastBillingDate = dtStartServiceDate.Value.AddDays(-1);

                        //2.1.2
                        UpdateTbt_BillingBasic(doTbt_BillingBasicList);

                        //2.2
                        doTbt_BillingHistory.ContractCode = doTbt_BillingBasicList.ContractCode;
                        doTbt_BillingHistory.BillingOCC = doTbt_BillingBasicList.BillingOCC;
                        //doTbt_BillingHistory.HistoryNo = null;
                        doTbt_BillingHistory.MonthlyBillingAmount = doTbt_BillingBasicList.MonthlyBillingAmount;
                        doTbt_BillingHistory.MonthlyBillingAmountUsd = doTbt_BillingBasicList.MonthlyBillingAmountUsd;
                        doTbt_BillingHistory.MonthlyBillingAmountCurrencyType = doTbt_BillingBasicList.MonthlyBillingAmountCurrencyType;
                        doTbt_BillingHistory.BillingStartDate = dtStartServiceDate;
                        doTbt_BillingHistory.BillingEndDate = null;
                        CreateMonthlyBillingHistory(doTbt_BillingHistory);
                        //2.3
                        DateTime? dtBillingEndDate = DateTime.Now;
                        if (dtAdjustDate == null)
                        {
                            if (!CommonUtil.IsNullOrEmpty(doTbt_BillingBasicList.BillingCycle))
                            {
                                if (strContractCode.StartsWith("MA"))
                                {
                                    dtBillingEndDate = dtStartServiceDate.Value
                                        .AddDays(1 - dtStartServiceDate.Value.Day) //Start by the begin of month
                                        .AddMonths(doTbt_BillingBasicList.BillingCycle.Value) //Add billing cycle months
                                        .AddDays(dtStartServiceDate.Value.Day - 2); //Add day before the billing start.
                                }
                                else
                                {
                                    dtBillingEndDate = Convert.ToDateTime(dtStartServiceDate).AddMonths(Convert.ToInt32(doTbt_BillingBasicList.BillingCycle) - 1);
                                    dtBillingEndDate = CommonUtil.LastDayOfMonthFromDateTime(Convert.ToDateTime(dtBillingEndDate).Month, Convert.ToDateTime(dtBillingEndDate).Year);
                                }
                            }
                        }
                        else
                        {
                            dtBillingEndDate = dtAdjustDate;
                        }
                        //2.4
                        //call CalBillingAmount();
                        dBillngAmount = CalculateBillingAmount(doTbt_BillingBasicList.ContractCode, doTbt_BillingBasicList.BillingOCC, doTbt_BillingBasicList.CalDailyFeeStatus, dtStartServiceDate, dtBillingEndDate);
                        //2.5
                        if (doTbt_BillingBasicList.PaymentMethod == PaymentMethodType.C_PAYMENT_METHOD_AUTO_TRANFER
                            || doTbt_BillingBasicList.PaymentMethod == PaymentMethodType.C_PAYMENT_METHOD_CREDIT_CARD)
                        {
                            dtAutoTransferDate = GetNextAutoTransferDate(doTbt_BillingBasicList.ContractCode, doTbt_BillingBasicList.BillingOCC, doTbt_BillingBasicList.PaymentMethod);
                            if (dtAutoTransferDate == null)
                            {
                                doTbt_BillingBasicList.PaymentMethod = PaymentMethodType.C_PAYMENT_METHOD_BANK_TRANSFER;
                                dtAutoTransferDate = null;
                            }
                        }
                        //2.6.1              
                        doBillingTypeDetail = GetBillingTypeDetailContinuesData(doTbt_BillingBasicList.ContractCode, doTbt_BillingBasicList.BillingOCC, BillingTypeGroup.C_BILLING_TYPE_GROUP_CONTINUES);
                        //2.6.2
                        if (doBillingTypeDetail == null)
                        {
                            return false;
                        }
                        //2.7
                        tbt_BillingDetail doBillingDetail = new tbt_BillingDetail();
                        doBillingDetail.ContractCode = doTbt_BillingBasicList.ContractCode;
                        doBillingDetail.BillingOCC = doTbt_BillingBasicList.BillingOCC;
                        //doBillingDetail.BillingDetailNo = null;
                        doBillingDetail.InvoiceNo = null;
                        doBillingDetail.InvoiceOCC = null;
                        doBillingDetail.IssueInvDate = DateTime.Now;
                        doBillingDetail.IssueInvFlag = true;
                        doBillingDetail.BillingTypeCode = doBillingTypeDetail.BillingTypeCode;

                        string dBillingAmountCurrncyType = CalculateBillingAmountCurrencyType(doTbt_BillingBasicList.ContractCode, doTbt_BillingBasicList.BillingOCC, doTbt_BillingBasicList.CalDailyFeeStatus, dtStartServiceDate, dtBillingEndDate);
                        if (dBillingAmountCurrncyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            doBillingDetail.BillingAmount = null;
                            doBillingDetail.BillingAmountUsd = dBillngAmount;
                            doBillingDetail.BillingAmountCurrencyType = dBillingAmountCurrncyType;
                        }
                        else
                        {
                        doBillingDetail.BillingAmount = dBillngAmount;
                            doBillingDetail.BillingAmountUsd = null;
                            doBillingDetail.BillingAmountCurrencyType = dBillingAmountCurrncyType;
                        }
                        doBillingDetail.AdjustBillingAmount = null;


                        doBillingDetail.BillingStartDate = dtStartServiceDate;
                        doBillingDetail.BillingEndDate = dtBillingEndDate;
                        doBillingDetail.PaymentMethod = doTbt_BillingBasicList.PaymentMethod;
                        if (doTbt_BillingBasicList.PaymentMethod == PaymentMethodType.C_PAYMENT_METHOD_AUTO_TRANFER
                            || doTbt_BillingBasicList.PaymentMethod == PaymentMethodType.C_PAYMENT_METHOD_CREDIT_CARD)
                        {
                            doBillingDetail.PaymentStatus = PaymentStatus.C_PAYMENT_STATUS_DETAIL_AUTO_CREDIT;
                        }
                        else
                        {
                            doBillingDetail.PaymentStatus = PaymentStatus.C_PAYMENT_STATUS_DETAIL_BANK_COLLECT;
                        }

                        doBillingDetail.AutoTransferDate = dtAutoTransferDate;
                        doBillingDetail.FirstFeeFlag = null;
                        doBillingDetail.DelayedMonth = null;
                        doBillingDetail.StartOperationDate = dtStartServiceDate;

                        IRentralContractHandler rentalContralHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                        List<dtTbt_RentalContractBasicForView> dtRentalContract = rentalContralHandler.GetTbt_RentalContractBasicForView(doTbt_BillingBasicList.ContractCode);
                        if (dtRentalContract.Count > 0)
                        {
                            doBillingDetail.ContractOCC = dtRentalContract[0].LastOCC;
                        }
                        else
                        {
                            ISaleContractHandler saleHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                            List<tbt_SaleBasic> dtSaleBasic = saleHandler.GetTbt_SaleBasic(doTbt_BillingBasicList.ContractCode, null, true);
                            if (dtSaleBasic != null && dtSaleBasic.Count > 0)
                            {
                                doBillingDetail.ContractOCC = dtSaleBasic[0].OCC;
                            }
                        }
                        doBillingDetail.ForceIssueFlag = false;
                        //2.7.2
                        ManageBillingDetail(doBillingDetail);

                    }
                    else
                    {
                        doTbt_BillingBasicList.StartOperationDate = dtStartServiceDate;
                        doTbt_BillingBasicList.StopBillingFlag = true;
                        UpdateTbt_BillingBasic(doTbt_BillingBasicList);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// Calculate billing amount from billing history 
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strBillingOCC"></param>
        /// <param name="strCalDailyFee"></param>
        /// <param name="dtStartDate"></param>
        /// <param name="dtEndDate"></param>
        /// <returns></returns>
        public decimal CalculateBillingAmount(string strContractCode, string strBillingOCC, string strCalDailyFee, DateTime? dtStartDate, DateTime? dtEndDate)
        {
            try
            {
                //1.
                List<string> lst = new List<string>();
                List<tbt_MonthlyBillingHistory> doTbt_MonthlyBillingHistoryList = new List<tbt_MonthlyBillingHistory>();

                if (CommonUtil.IsNullOrEmpty(strContractCode))
                {
                    //lst.Add("ContractCode");
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "ContractCode" });
                }
                if (CommonUtil.IsNullOrEmpty(strBillingOCC))
                {
                    //lst.Add("BillingOOC");
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "BillinbgOCC" });
                }
                if (CommonUtil.IsNullOrEmpty(strCalDailyFee))
                {
                    //lst.Add("CalDailyFee");
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "CalDailyFee" });
                }
                if (CommonUtil.IsNullOrEmpty(dtStartDate))
                {
                    //lst.Add("StartDate");
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "StartDate" });
                }
                if (CommonUtil.IsNullOrEmpty(dtEndDate))
                {
                    //lst.Add("EndDate");
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "EndDate" });
                }
                //2.
                doTbt_MonthlyBillingHistoryList = base.GetBillingHistoryPeriodList(strContractCode, strBillingOCC, dtStartDate, dtEndDate);
                //3.
                DateTime? dtFrom;
                DateTime? dtTo;
                decimal decAccumulateAmount = 0;
                //4.
                dtFrom = dtStartDate;
                decAccumulateAmount = 0;
                //5.
                int iRow = 0;
                foreach (tbt_MonthlyBillingHistory doTbt_MonthlyBillingHistory in doTbt_MonthlyBillingHistoryList)
                {
                    //Checck Currency
                    if(doTbt_MonthlyBillingHistory.MonthlyBillingAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        doTbt_MonthlyBillingHistory.MonthlyBillingAmount = doTbt_MonthlyBillingHistory.MonthlyBillingAmountUsd;
                    }

                    //5.1
                    if (iRow.Equals(doTbt_MonthlyBillingHistoryList.Count - 1))
                    {
                        dtTo = dtEndDate;
                    }
                    else
                    {
                        dtTo = Convert.ToDateTime(doTbt_MonthlyBillingHistory.BillingEndDate);
                    }
                    //5.3 
                    decAccumulateAmount = decAccumulateAmount + CalCulateBillingAmountPerHistory(dtFrom, dtTo, Convert.ToDecimal(doTbt_MonthlyBillingHistory.MonthlyBillingAmount), strCalDailyFee);
                    //5.4
                    dtFrom = Convert.ToDateTime(dtTo).AddDays(1);
                    iRow += 1;
                }
                //return decAccumulateAmount;
                return RoundUp(decAccumulateAmount, 2); //Modify by Jutarat A. on 27012014
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public string CalculateBillingAmountCurrencyType(string strContractCode, string strBillingOCC, string strCalDailyFee, DateTime? dtStartDate, DateTime? dtEndDate)
        {
            List<tbt_MonthlyBillingHistory> doTbt_MonthlyBillingHistoryList = new List<tbt_MonthlyBillingHistory>();
            doTbt_MonthlyBillingHistoryList = base.GetBillingHistoryPeriodList(strContractCode, strBillingOCC, dtStartDate, dtEndDate);
            return doTbt_MonthlyBillingHistoryList[0].MonthlyBillingAmountCurrencyType;
        }

        /// <summary>
        /// Calculate billing amount per one billing history by depend on calculation fee status such as calendar, 30.4 and other
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="BillingAmount"></param>
        /// <param name="CalDailyFeeStatus"></param>
        /// <returns></returns>
        public decimal CalCulateBillingAmountPerHistory(DateTime? FromDate, DateTime? ToDate, decimal BillingAmount, string CalDailyFeeStatus)
        {
            decimal decTotalAmount = 0;
            int intBeginNoOfDay = 0;
            int intMidNoOfMonth = 0;
            int intEndNoOfDay = 0;

            int intNoOfMonth = 0;
            int intNoOfDay = 0;

            int iLastDayOfMonth_From = DateTime.DaysInMonth(Convert.ToDateTime(FromDate).Year, Convert.ToDateTime(FromDate).Month);  //CommonUtil.LastDayOfMonthFromDateTime(Convert.ToDateTime(FromDate).Month, Convert.ToDateTime(FromDate).Year).Day;
            int iLastDayOfMonth_To = DateTime.DaysInMonth(Convert.ToDateTime(ToDate).Year, Convert.ToDateTime(ToDate).Month); //CommonUtil.LastDayOfMonthFromDateTime(Convert.ToDateTime(ToDate).Month, Convert.ToDateTime(ToDate).Year).Day;

            DateTime adjToDate = ToDate.Value.AddDays(-FromDate.Value.Day + 1);
            DateTime adjFromDate = FromDate.Value.AddDays(-FromDate.Value.Day + 1);

            //2.
            if (CalDailyFeeStatus == CalculationDailyFeeType.C_CALC_DAILY_FEE_TYPE_CALENDAR)
            {

                int i = string.Compare(CommonUtil.LastDayOfMonthFromDateTime(Convert.ToDateTime(adjFromDate).Month, Convert.ToDateTime(adjFromDate).Year).ToString("yyyyMMdd"), adjToDate.ToString("yyyyMMdd"));
                if (i == 0) // Date between From and To == 1 Month
                {
                    decTotalAmount = BillingAmount;
                }

                else if (i > 0) // Date between From and To < 1 Month // (in case not equal 1 month)
                {
                    if (FromDate.Value.Month == ToDate.Value.Month) // dateFrom , dataTo is in same month // then calculate with same rate
                    {
                        intBeginNoOfDay = CalculateDayDifference(ToDate, FromDate);

                        //decTotalAmount = RoundUp(BillingAmount / iLastDayOfMonth_From, 2) * intBeginNoOfDay;
                        decTotalAmount = (BillingAmount / iLastDayOfMonth_From) * intBeginNoOfDay; //Modify by Jutarat A. on 24012014
                    }
                    else //dateFrom , dataTo not same month // so separate to 2 period and calculate with 2 rate/day of each month
                    {
                        // first period
                        int first_period = (CommonUtil.LastDayOfMonthFromDateTime(FromDate).Day - FromDate.Value.Day) + 1;
                        //decimal totalAmount_first_period = RoundUp(BillingAmount / iLastDayOfMonth_From, 2) * first_period;
                        decimal totalAmount_first_period = (BillingAmount / iLastDayOfMonth_From) * first_period; //Modify by Jutarat A. on 24012014

                        // second period
                        int second_period = ToDate.Value.Day;
                        //decimal totalAmount_second_period = RoundUp(BillingAmount / iLastDayOfMonth_To, 2) * second_period;
                        decimal totalAmount_second_period = (BillingAmount / iLastDayOfMonth_To) * second_period; //Modify by Jutarat A. on 24012014

                        // sum
                        decTotalAmount = totalAmount_first_period + totalAmount_second_period;

                    }


                }

                else // Date between From and To > 1 Month
                {

                    intBeginNoOfDay = Convert.ToDateTime(FromDate).Day == 1 ? 0 : CalculateDayDifference((CommonUtil.LastDayOfMonthFromDateTime(Convert.ToDateTime(FromDate).Month, Convert.ToDateTime(FromDate).Year)), FromDate);

                    intMidNoOfMonth = CalculateFullMonthDiff(FromDate, ToDate);

                    intEndNoOfDay = Convert.ToDateTime(ToDate).Day == CommonUtil.LastDayOfMonthFromDateTime(ToDate).Day ? 0 : Convert.ToDateTime(ToDate).Day;

                    //Modify by Jutarat A. on 24012014
                    /*decTotalAmount = RoundUp(BillingAmount / iLastDayOfMonth_From, 2) * intBeginNoOfDay +
                                  (intMidNoOfMonth * BillingAmount) +
                                 RoundUp(BillingAmount / iLastDayOfMonth_To, 2) * intEndNoOfDay;*/
                    decTotalAmount = (BillingAmount / iLastDayOfMonth_From) * intBeginNoOfDay +
                                  (intMidNoOfMonth * BillingAmount) +
                                  (BillingAmount / iLastDayOfMonth_To) * intEndNoOfDay;
                    //End Modify
                }

            }
            else
            {

                //3.2
                if (string.Compare(Convert.ToDateTime(adjFromDate).AddMonths(1).ToString("yyyyMMdd"), Convert.ToDateTime(adjToDate).ToString("yyyyMMdd")) > 0)
                {
                    intNoOfDay = CalculateDayDifference(adjToDate, adjFromDate);
                    if (intNoOfDay == DateTime.DaysInMonth(adjFromDate.Year, adjFromDate.Month))
                    {
                        intNoOfMonth = 1;
                        intNoOfDay = 0;
                    }

                    else // i==0
                    {
                        intNoOfMonth = 0;
                    }
                }
                //3.3
                else
                {
                    //3.3.1
                    if (Convert.ToDateTime(adjToDate).Day < Convert.ToDateTime(adjFromDate).Day)
                    {
                        intNoOfMonth = CalculateMonthDifference(adjFromDate, adjToDate) - 1;
                    }
                    else
                    {
                        //intNoOfMonth = Number of month of (ToDate - FromDate) 
                        intNoOfMonth = CalculateMonthDifference(adjFromDate, adjToDate);
                    }

                    //3.3.2
                    //intNoOfDay = Number of day of (ToDate – (FromDate + @intNoOfMonth)) + 1  
                    intNoOfDay = CalculateDayDifference(adjToDate, Convert.ToDateTime(adjFromDate).AddMonths(intNoOfMonth));
                    if (intNoOfDay == DateTime.DaysInMonth(adjToDate.Year, adjToDate.Month))
                    {
                        intNoOfMonth += 1;
                        intNoOfDay = 0;
                    }
                    //else
                    //{
                    //    intNoOfDay += 1;
                    //}

                }

                //3.4
                // decTotalAmount =	[@intNoOfMonth x BillingAmount] + [MATH.ROUNDUP((@intNoOfDay/30.4),2) x BillingAmount]

                //decTotalAmount = (intNoOfMonth * BillingAmount) + RoundUp(BillingAmount / 30.4M, 2) * intNoOfDay;
                decTotalAmount = (intNoOfMonth * BillingAmount) + (BillingAmount / 30.4M) * intNoOfDay; //Modify by Jutarat A. on 24012014


            }
            return decTotalAmount;
        }

        /// <summary>
        /// To calculate month difference 
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public int CalculateMonthDifference(DateTime? startDate, DateTime? endDate)
        {
            if (!CommonUtil.IsNullOrEmpty(startDate) && !CommonUtil.IsNullOrEmpty(endDate))
            {
                int monthsApart = 12 * (Convert.ToDateTime(startDate).Year - Convert.ToDateTime(endDate).Year) + Convert.ToDateTime(startDate).Month - Convert.ToDateTime(endDate).Month;
                return Math.Abs(monthsApart);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Calculate difference beteween datafrom and dateto and return number of full month in this period
        /// </summary>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public int CalculateFullMonthDiff(DateTime? dateFrom, DateTime? dateTo)
        {
            if (dateFrom.HasValue && dateFrom.HasValue)
            {
                DateTime d1 = Convert.ToDateTime(dateFrom);
                DateTime d2 = Convert.ToDateTime(dateTo);

                if (d1.Day != 1)
                    d1 = d1.AddDays(-d1.Day + 1).AddMonths(1);
                d2 = d2.AddDays(1);
                if (d2.Day != 1)
                    d2 = d2.AddDays(-d2.Day + 1);

                return (((d2.Year * 12) + d2.Month) - ((d1.Year * 12) + d1.Month));
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        ///  To calculate date difference return number of days
        /// </summary>
        /// <param name="toDate"></param>
        /// <param name="fromDate"></param>
        /// <returns></returns>
        public int CalculateDayDifference(DateTime? toDate, DateTime? fromDate)
        {
            if (!CommonUtil.IsNullOrEmpty(toDate) && !CommonUtil.IsNullOrEmpty(fromDate))
            {
                TimeSpan span = Convert.ToDateTime(toDate).Subtract(Convert.ToDateTime(fromDate));
                return Math.Abs(span.Days + 1);
            }
            else
            {
                return 0;
            }


        }

        /// <summary>
        /// To get continues billing type by billing code
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="BillingOCC"></param>
        /// <param name="C_BILLING_TYPE_GROUP_CONTINUES"></param>
        /// <returns></returns>
        public BillingTypeDetail GetBillingTypeDetailContinuesData(string ContractCode, string BillingOCC, string C_BILLING_TYPE_GROUP_CONTINUES)
        {
            try
            {
                List<BillingTypeDetail> result = base.GetBillingTypeDetailContinues(ContractCode, BillingOCC, C_BILLING_TYPE_GROUP_CONTINUES);
                if (result.Count > 0)
                {
                    return result[0];
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region BLP013 ManageBillingBasicForResume
        /// <summary>
        /// To update billing basic when contract send command for resume service
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="dtResumeDate"></param>
        /// <returns></returns>
        public bool ManageBillingBasicForResume(string strContractCode, DateTime dtResumeDate)
        {
            List<BillingBasicList> doTbt_BillingBasicListData = new List<BillingBasicList>();
            AdjustOnNextPeriod doAdjustOnNextPeriod = new AdjustOnNextPeriod();
            tbt_BillingDetail doBillingDetail = new tbt_BillingDetail();
            BillingTypeDetail doBillingTypeDetail = new BillingTypeDetail();

            doTbt_BillingBasicListData = GetBillingBasicListData(strContractCode, MiscType.C_CUST_TYPE);
            foreach (BillingBasicList doTbt_BillingBasicList in doTbt_BillingBasicListData)
            {

                if (doTbt_BillingBasicList.MonthlyFeeBeforeStop.HasValue)
                {
                    //2.1
                    if (Convert.ToDecimal(doTbt_BillingBasicList.MonthlyBillingAmount) != Convert.ToDecimal(doTbt_BillingBasicList.MonthlyFeeBeforeStop)
                        || Convert.ToDecimal(doTbt_BillingBasicList.MonthlyBillingAmountUsd) != Convert.ToDecimal(doTbt_BillingBasicList.MonthlyFeeBeforeStop))
                    {
                        //2.1.1
                        doAdjustOnNextPeriod = CalculateDifferenceMonthlyFee(doTbt_BillingBasicList.ContractCode,
                                                                             doTbt_BillingBasicList.BillingOCC,
                                                                             dtResumeDate,
                                                                             Convert.ToDecimal(doTbt_BillingBasicList.MonthlyFeeBeforeStop),
                                                                             ProcessID.C_PROCESS_ID_MANAGE_BILLING_BASIC_RESUME); // edit by Narupon W.
                        //2.1.2
                        if (doAdjustOnNextPeriod != null)
                        {
                            doTbt_BillingBasicList.AdjustType = doAdjustOnNextPeriod.AdjustType;
                            doTbt_BillingBasicList.AdjustBillingPeriodAmount = doAdjustOnNextPeriod.AdjustBillingPeriodAmount;
                            doTbt_BillingBasicList.AdjustBillingPeriodStartDate = doAdjustOnNextPeriod.AdjustBillingPeriodStartDate;
                            doTbt_BillingBasicList.AdjustBillingPeriodEndDate = doAdjustOnNextPeriod.AdjustBillingPeriodEndDate;
                        }
                        //2.1.3
                        if (Convert.ToDecimal(doTbt_BillingBasicList.MonthlyBillingAmount) == 0
                            && doTbt_BillingBasicList.LastBillingDate < dtResumeDate)
                        {
                            //2.1.3.1
                            doBillingTypeDetail = GetBillingTypeDetailContinuesData(doTbt_BillingBasicList.ContractCode, doTbt_BillingBasicList.BillingOCC, BillingTypeGroup.C_BILLING_TYPE_GROUP_CONTINUES);
                            //2.1.3.2
                            if (doBillingTypeDetail == null)
                            {
                                return false;
                            }
                            //2.1.3.3
                            doBillingDetail.ContractCode = doTbt_BillingBasicList.ContractCode;
                            doBillingDetail.BillingOCC = doTbt_BillingBasicList.BillingOCC;
                            doBillingDetail.InvoiceNo = null;
                            doBillingDetail.InvoiceOCC = null;
                            doBillingDetail.IssueInvDate = DateTime.Now;
                            doBillingDetail.IssueInvFlag = false;
                            doBillingDetail.BillingTypeCode = doBillingTypeDetail.BillingTypeCode;
                            doBillingDetail.BillingAmount = 0;
                            doBillingDetail.AdjustBillingAmount = null;
                            doBillingDetail.BillingStartDate = (Convert.ToDateTime(doTbt_BillingBasicList.LastBillingDate)).AddDays(1);
                            doBillingDetail.BillingEndDate = dtResumeDate.AddDays(-1);
                            doBillingDetail.PaymentMethod = doTbt_BillingBasicList.PaymentMethod;
                            if (doTbt_BillingBasicList.PaymentMethod == PaymentMethodType.C_PAYMENT_METHOD_AUTO_TRANFER
                                || doTbt_BillingBasicList.PaymentMethod == PaymentMethodType.C_PAYMENT_METHOD_CREDIT_CARD)
                            {
                                doBillingDetail.PaymentStatus = PaymentStatus.C_PAYMENT_STATUS_DETAIL_AUTO_CREDIT;
                            }
                            else
                            {
                                doBillingDetail.PaymentStatus = PaymentStatus.C_PAYMENT_STATUS_DETAIL_BANK_COLLECT;
                            }
                            doBillingDetail.AutoTransferDate = null;
                            doBillingDetail.FirstFeeFlag = null;
                            doBillingDetail.DelayedMonth = null;
                            doBillingDetail.StartOperationDate = doTbt_BillingBasicList.StartOperationDate;
                            
                            IRentralContractHandler rentalContralHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                            List<dtTbt_RentalContractBasicForView> dtRentalContract = rentalContralHandler.GetTbt_RentalContractBasicForView(doTbt_BillingBasicList.ContractCode);
                            if (dtRentalContract.Count > 0)
                            {
                                doBillingDetail.ContractOCC = dtRentalContract[0].LastOCC;
                            }
                            else
                            {
                                ISaleContractHandler saleHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                                List<tbt_SaleBasic> dtSaleBasic = saleHandler.GetTbt_SaleBasic(doTbt_BillingBasicList.ContractCode, null, true);
                                if (dtSaleBasic != null && dtSaleBasic.Count > 0)
                                {
                                    doBillingDetail.ContractOCC = dtSaleBasic[0].OCC;
                                }
                            }
                            doBillingDetail.ForceIssueFlag = false;
                            ManageBillingDetail(doBillingDetail);
                        }
                    }
                    //2.2
                    if (Convert.ToDecimal(doTbt_BillingBasicList.MonthlyBillingAmount) > 0 || Convert.ToDecimal(doTbt_BillingBasicList.MonthlyBillingAmountUsd) > 0)
                    {
                        doTbt_BillingBasicList.MonthlyBillingAmount = doTbt_BillingBasicList.MonthlyFeeBeforeStop;
                        doTbt_BillingBasicList.MonthlyFeeBeforeStop = null;
                    }
                    else
                    {
                        doTbt_BillingBasicList.MonthlyBillingAmount = doTbt_BillingBasicList.MonthlyFeeBeforeStop;
                        doTbt_BillingBasicList.StopBillingFlag = false;
                        doTbt_BillingBasicList.MonthlyFeeBeforeStop = null;
                        doTbt_BillingBasicList.LastBillingDate = dtResumeDate.AddDays(-1);
                    }
                    //2.3
                    tbt_BillingBasic BillingBasic = CommonUtil.CloneObject<BillingBasicList, tbt_BillingBasic>(doTbt_BillingBasicList);
                    UpdateTbt_BillingBasic(BillingBasic);
                }


            }
            return true;
        }


        #endregion

        #region BLP015 ManageBillingBasicForStop
        /// <summary>
        /// To update billing basic when contract send command for stop service
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="dtStopDate"></param>
        /// <param name="dStopFee"></param>
        /// <returns></returns>
        public bool ManageBillingBasicForStop(string strContractCode, DateTime dtStopDate, decimal dStopFee)
        {
            List<tbt_BillingBasic> doTbt_BillingBasicListData = base.GetTbt_BillingBasicListData(strContractCode);
            AdjustOnNextPeriod doAdjustOnNextPeriod = new AdjustOnNextPeriod();

            if (doTbt_BillingBasicListData.Count == 0)
            {
                return false;
            }

            if (doTbt_BillingBasicListData[0].StopBillingFlag == false)
            {
                foreach (tbt_BillingBasic doTbt_BillingBasicList in doTbt_BillingBasicListData)
                {
                    if (doTbt_BillingBasicList.StopBillingFlag == false)
                    {
                        //2.1               
                        if (Convert.ToDecimal(doTbt_BillingBasicList.MonthlyBillingAmount) != dStopFee || Convert.ToDecimal(doTbt_BillingBasicList.MonthlyBillingAmountUsd) != dStopFee)
                        {
                            //2.1.1                   
                            doAdjustOnNextPeriod = CalculateDifferenceMonthlyFee(doTbt_BillingBasicList.ContractCode,
                                                                                 doTbt_BillingBasicList.BillingOCC,
                                                                                 dtStopDate,
                                                                                 dStopFee,
                                                                                 ProcessID.C_PROCESS_ID_MANAGE_BILLING_BASIC_STOP);

                            //2.1.2
                            if (doAdjustOnNextPeriod != null)
                            {
                                doTbt_BillingBasicList.AdjustType = doAdjustOnNextPeriod.AdjustType;
                                doTbt_BillingBasicList.AdjustBillingPeriodAmount = doAdjustOnNextPeriod.AdjustBillingPeriodAmount;
                                doTbt_BillingBasicList.AdjustBillingPeriodStartDate = doAdjustOnNextPeriod.AdjustBillingPeriodStartDate;
                                doTbt_BillingBasicList.AdjustBillingPeriodEndDate = doAdjustOnNextPeriod.AdjustBillingPeriodEndDate;
                            }
                        }

                        //2.2.1
                        decimal? tempBillingAmount = null;
                        if (dStopFee > 0)
                        {
                            //2.2.1.1
                            tempBillingAmount = doTbt_BillingBasicList.MonthlyBillingAmountUsd ?? doTbt_BillingBasicList.MonthlyBillingAmount;
                            doTbt_BillingBasicList.MonthlyBillingAmount = dStopFee;
                            doTbt_BillingBasicList.MonthlyFeeBeforeStop = tempBillingAmount;
                        }
                        //2.2.2
                        else
                        {
                            tempBillingAmount = doTbt_BillingBasicList.MonthlyBillingAmountUsd ?? doTbt_BillingBasicList.MonthlyBillingAmount;
                            doTbt_BillingBasicList.MonthlyBillingAmount = dStopFee;
                            doTbt_BillingBasicList.StopBillingFlag = true;
                            doTbt_BillingBasicList.MonthlyFeeBeforeStop = tempBillingAmount;
                        }
                        UpdateTbt_BillingBasic(doTbt_BillingBasicList);
                    }


                }
            }


            return true;
        }
        #endregion

        #region BLP016 ManageBillingBasicForCancel
        /// <summary>
        /// To update billing basic when contract send command for cancel service
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="dtCancelDate"></param>
        /// <returns></returns>
        public bool ManageBillingBasicForCancel(string strContractCode, DateTime dtCancelDate)
        {
            List<BillingBasicList> doBillingBasicListData = new List<BillingBasicList>();
            AdjustOnNextPeriod doAdjustOnNextPeriod = new AdjustOnNextPeriod();
            doBillingBasicListData = GetBillingBasicListData(strContractCode, MiscType.C_CUST_TYPE);
            foreach (BillingBasicList doBillingBasicList in doBillingBasicListData)
            {
                if (Convert.ToDecimal(doBillingBasicList.MonthlyBillingAmount) > 0 && doBillingBasicList.StopBillingFlag == false) // Edit by Patcharee T. Jul 25, 2012
                {
                    doAdjustOnNextPeriod = CalculateDifferenceMonthlyFee(
                                                                            doBillingBasicList.ContractCode,
                                                                            doBillingBasicList.BillingOCC,
                                                                            dtCancelDate,
                                                                            0M,
                                                                            ProcessID.C_PROCESS_ID_MANAGE_BILLING_BASIC_CANCEL);
                }
                //Create monthly billing history 
                if (Convert.ToDecimal(doBillingBasicList.MonthlyFeeBeforeStop) != Convert.ToDecimal(doBillingBasicList.MonthlyBillingAmount))
                {
                    tbt_MonthlyBillingHistory doTbt_BillingHistory = new tbt_MonthlyBillingHistory();
                    doTbt_BillingHistory.ContractCode = doBillingBasicList.ContractCode;
                    doTbt_BillingHistory.BillingOCC = doBillingBasicList.BillingOCC;
                    //doTbt_BillingHistory.HistoryNo = null;
                    doTbt_BillingHistory.MonthlyBillingAmount = 0; //doBillingBasicList.MonthlyFeeBeforeStop; //Modify by Jutarat A. on 28032014
                    doTbt_BillingHistory.BillingStartDate = dtCancelDate;
                    doTbt_BillingHistory.BillingEndDate = null;
                    CreateMonthlyBillingHistory(doTbt_BillingHistory);
                }

                //Update billing basic
                doBillingBasicList.MonthlyBillingAmount = doBillingBasicList.MonthlyFeeBeforeStop == null ? doBillingBasicList.MonthlyBillingAmount : doBillingBasicList.MonthlyFeeBeforeStop;
                doBillingBasicList.StopBillingFlag = true;
                doBillingBasicList.MonthlyFeeBeforeStop = null;
                tbt_BillingBasic basic = CommonUtil.CloneObject<BillingBasicList, tbt_BillingBasic>(doBillingBasicList);
                UpdateTbt_BillingBasic(basic);

            }
            return true;
        }
        #endregion

        #region BLP017 ManageBillingBasicForChangNameAndAddress
        /// <summary>
        /// To update billing basic and billing target if there are not existing in DB when contract send command for change name and address
        /// </summary>
        /// <param name="doBillingTempBasicListData"></param>
        /// <returns></returns>
        public List<doBillingTempBasic> ManageBillingBasicForChangeNameAndAddress(List<doBillingTempBasic> doBillingTempBasicListData)
        {

            tbt_BillingTarget doTbt_BillingTarget = new tbt_BillingTarget();
            tbt_BillingBasic doTbt_BillingBasic = new tbt_BillingBasic();
            IBillingMasterHandler masterHandler = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;
            List<string> lst = new List<string>();
            string strBillingTargetCode = null;
            foreach (doBillingTempBasic doBillingTempBasicList in doBillingTempBasicListData)
            {
                strBillingTargetCode = null;
                if (CommonUtil.IsNullOrEmpty(doBillingTempBasicList.ContractCode))
                {
                    //lst.Add("ContractCode");
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "ContractCode" });
                }
                if (CommonUtil.IsNullOrEmpty(doBillingTempBasicList.BillingOCC))
                {
                    //lst.Add("BillingOCC");
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "BillingOCC" });
                }
                List<tbt_BillingBasic> result = base.GetTbt_BillingBasic(doBillingTempBasicList.ContractCode, doBillingTempBasicList.BillingOCC);
                if (result.Count > 0)
                {
                    doTbt_BillingBasic = result[0];
                }
                else
                {
                    doTbt_BillingBasic = null;
                }
                // doTbt_BillingBasic = GetTbt_BillingBasic(doBillingTempBasicList.ContractCode, doBillingTempBasicList.BillingOCC);
                //1.3
                if (!CommonUtil.IsNullOrEmpty(doBillingTempBasicList.BillingTargetCode))
                {
                    //doTbt_BillingTarget = GetTbt_BillingTargetData(doBillingTempBasicList.BillingTargetCode, null, null);
                    strBillingTargetCode = doBillingTempBasicList.BillingTargetCode;
                }
                //1.4
                else if (!CommonUtil.IsNullOrEmpty(doBillingTempBasicList.BillingClientCode)
                    && !CommonUtil.IsNullOrEmpty(doBillingTempBasicList.BillingOfficeCode))
                {
                    //1.4.1
                    doTbt_BillingTarget = GetTbt_BillingTargetData(null, doBillingTempBasicList.BillingClientCode, doBillingTempBasicList.BillingOfficeCode);

                    var dtbillingClient = masterHandler.GetBillingClient(doBillingTempBasicList.BillingClientCode);
                    //1.4.2

                    if (doTbt_BillingTarget == null)
                    {
                        doTbt_BillingTarget = new tbt_BillingTarget();
                        doTbt_BillingTarget.BillingTargetCode = null;
                        doTbt_BillingTarget.BillingTargetNo = null;
                        doTbt_BillingTarget.BillingClientCode = doBillingTempBasicList.BillingClientCode;
                        doTbt_BillingTarget.BillingOfficeCode = doBillingTempBasicList.BillingOfficeCode;
                        doTbt_BillingTarget.ContactPersonName = null;
                        doTbt_BillingTarget.IssueInvTime = null;
                        doTbt_BillingTarget.IssueInvMonth = null;
                        doTbt_BillingTarget.IssueInvDate = null;
                        doTbt_BillingTarget.SeparateInvType = null;
                        doTbt_BillingTarget.InvFormatType = null;
                        doTbt_BillingTarget.SignatureType = null;
                        doTbt_BillingTarget.ShowInvWHTFlag = null;
                        doTbt_BillingTarget.ShowDueDate = null;
                        doTbt_BillingTarget.DocLanguage = null;
                        doTbt_BillingTarget.SuppleInvAddress = null;
                        doTbt_BillingTarget.Memo = null;
                        if (dtbillingClient != null && dtbillingClient.Count > 0)
                        {
                            doTbt_BillingTarget.RealBillingClientNameEN = dtbillingClient[0].FullNameEN;
                            doTbt_BillingTarget.RealBillingClientNameLC = dtbillingClient[0].FullNameLC;
                            doTbt_BillingTarget.RealBillingClientAddressEN = dtbillingClient[0].AddressEN;
                            doTbt_BillingTarget.RealBillingClientAddressLC = dtbillingClient[0].AddressLC;
                        }

                        strBillingTargetCode = CreateBillingTarget(doTbt_BillingTarget);

                        doBillingTempBasicList.BillingTargetCode = strBillingTargetCode; //Add by Jutarat A. on 24072012
                    }
                    //1.4.3
                    else
                    {
                        strBillingTargetCode = doTbt_BillingTarget.BillingTargetCode;
                        doBillingTempBasicList.BillingTargetCode = doTbt_BillingTarget.BillingTargetCode; // Add by Narupon W. 3/May/2012
                    }
                }
                //1.5
                else
                {
                    if (CommonUtil.IsNullOrEmpty(doTbt_BillingTarget.BillingTargetCode))
                    {
                        //lst.Add("BillingTargetCode");
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "BillingTargetCode" });
                    }
                    if (CommonUtil.IsNullOrEmpty(doTbt_BillingTarget.BillingClientCode))
                    {
                        //lst.Add("BillingClientCode");
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "BillingClientCode" });
                    }
                    if (CommonUtil.IsNullOrEmpty(doTbt_BillingTarget.BillingOfficeCode))
                    {
                        //lst.Add("BillingOfficeCode");
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "BillingOfficeCode" });
                    }
                }
                //1.6
                if (doTbt_BillingBasic != null)
                {
                    string strPreviousBillingTargetCode = doTbt_BillingBasic.BillingTargetCode;
                    doTbt_BillingBasic.BillingTargetCode = strBillingTargetCode;
                    doTbt_BillingBasic.PreviousBillingTargetCode = strPreviousBillingTargetCode;
                    UpdateTbt_BillingBasic(doTbt_BillingBasic);
                }
            }
            return doBillingTempBasicListData;
        }
        #endregion

        #region BLP070 GetTbt_BillingTargetForView

        /// <summary>
        /// Get Billing target data for view (call store sp_BL_GetTbt_BillingTargetForView)
        /// </summary>
        /// <param name="strBillingTargetCode">Billing target code</param>
        /// <param name="C_CUST_TYPE">Cust type</param>
        /// <returns></returns>
        public dtTbt_BillingTargetForView GetTbt_BillingTargetForViewData(string strBillingTargetCode, string C_CUST_TYPE)
        {
            try
            {
                List<dtTbt_BillingTargetForView> result = base.GetTbt_BillingTargetForView(strBillingTargetCode, C_CUST_TYPE);
                CommonUtil.MappingObjectLanguage<dtTbt_BillingTargetForView>(result);

                if (result.Count > 0)
                {
                    return result[0];
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region BLS080 GetBillingBasicList
        /// <summary>
        /// Get list of billing basic by ContractCode
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="C_CUST_TYPE"></param>
        /// <returns></returns>
        public List<BillingBasicList> GetBillingBasicListData(string ContractCode, string C_CUST_TYPE)
        {
            try
            {
                if (String.IsNullOrEmpty(C_CUST_TYPE))
                    C_CUST_TYPE = MiscType.C_CUST_TYPE;

                List<BillingBasicList> result = base.GetBillingBasicList(ContractCode, C_CUST_TYPE);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region BLS020 Edit Billing target

        /// <summary>
        /// To update data to tbt_BillingTarget
        /// </summary>
        /// <param name="billingTarget"></param>
        /// <returns></returns>
        public int UpdateTbt_BillingTarget(tbt_BillingTarget billingTarget)
        {
            try
            {
                //set updateDate and updateBy
                billingTarget.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                billingTarget.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_BillingTarget> tbt_BillingTargetList = new List<tbt_BillingTarget>();
                tbt_BillingTargetList.Add(billingTarget);
                List<tbt_BillingTarget> updatedList = base.UpdateTbt_BillingTarget(CommonUtil.ConvertToXml_Store<tbt_BillingTarget>(tbt_BillingTargetList));

                //Insert Log
                if (updatedList.Count > 0)
                {

                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Update;
                    logData.TableName = TableName.C_TBL_NAME_BILLING_TARGET;
                    logData.TableData = CommonUtil.ConvertToXml(updatedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return updatedList.Count;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region BLS031 AutoTransfer
        /// <summary>
        /// To insert data to tbt__AutoTransferBankAccount
        /// </summary>
        /// <param name="doTbtAutoTransferBankAccountList"></param>
        /// <returns></returns>
        public int InsertTbt_AutoTransferBankAccountData(List<tbt_AutoTransferBankAccount> doTbtAutoTransferBankAccountList)
        {
            try
            {
                if (doTbtAutoTransferBankAccountList != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    foreach (tbt_AutoTransferBankAccount AutoTransferBankAccountData in doTbtAutoTransferBankAccountList)
                    {
                        AutoTransferBankAccountData.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                        AutoTransferBankAccountData.CreateBy = dsTrans.dtUserData.EmpNo;
                        AutoTransferBankAccountData.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                        AutoTransferBankAccountData.UpdateBy = dsTrans.dtUserData.EmpNo;
                    }
                }

                List<tbt_AutoTransferBankAccount> res = this.InsertTbt_AutoTransferBankAccount(
                    CommonUtil.ConvertToXml_Store<tbt_AutoTransferBankAccount>(doTbtAutoTransferBankAccountList));

                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Insert,
                    TableName = TableName.C_TBL_NAME_AUTO_BANK,
                    TableData = CommonUtil.ConvertToXml(res)
                };

                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);

                #endregion

                return res.Count;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region BLS032 Creditcard
        /// <summary>
        /// To insert data to tbt_CreditCard
        /// </summary>
        /// <param name="doTbtCreditCardList"></param>
        /// <returns></returns>
        public int InsertTbt_CreditCard(List<tbt_CreditCard> doTbtCreditCardList)
        {
            try
            {
                if (doTbtCreditCardList != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    foreach (tbt_CreditCard AutoTransferBankAccountData in doTbtCreditCardList)
                    {
                        AutoTransferBankAccountData.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                        AutoTransferBankAccountData.CreateBy = dsTrans.dtUserData.EmpNo;
                        AutoTransferBankAccountData.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                        AutoTransferBankAccountData.UpdateBy = dsTrans.dtUserData.EmpNo;
                    }
                }

                List<tbt_CreditCard> res = this.InsertTbt_CreditCard(
                    CommonUtil.ConvertToXml_Store<tbt_CreditCard>(doTbtCreditCardList));

                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Insert,
                    TableName = TableName.C_TBL_NAME_CREDIT_CARD,
                    TableData = CommonUtil.ConvertToXml(res)
                };

                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);

                #endregion

                return res.Count;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region BLS071 Select Billing Detail
        /// <summary>
        /// Get billing detail for combine (call store sp_BL_GetBillingDetailForCombine)
        /// </summary>
        /// <param name="BillingTargetCode"></param>
        /// <returns></returns>
        public override List<doBillingDetail> GetBillingDetailForCombine(string BillingTargetCode, string billingTypeCode, string c_CURRENCY_LOCAL, string c_CURRENCY_US, string c_CURRENCY)
        {
            try
            {
                List<doBillingDetail> result = base.GetBillingDetailForCombine(BillingTargetCode, billingTypeCode,CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US, c_CURRENCY);

                if (result.Count > 0)
                {
                    CommonUtil.MappingObjectLanguage<doBillingDetail>(result);
                    return result;
                }
                else
                    return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region BLS040
        /// <summary>
        /// To delete data from tbt_BillingTypeDetail
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strBillingOCC"></param>
        /// <param name="strBillingTypeCode"></param>
        /// <returns></returns>
        public List<tbt_BillingTypeDetail> DeleteTbt_BillingTypeDetail(string strContractCode, string strBillingOCC, string strBillingTypeCode)
        {
            try
            {
                //Delete data from DB
                List<tbt_BillingTypeDetail> deletedList = base.DeleteTbt_BillingTypeDetailData(strContractCode, strBillingOCC, strBillingTypeCode);

                //Insert Log
                if (deletedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                    logData.TableName = TableName.C_TBL_NAME_BILLING_TYPE_DETAIL;
                    logData.TableData = CommonUtil.ConvertToXml(deletedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return deletedList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// To delete data from tbt_CreditCard
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strBillingOCC"></param>
        /// <returns></returns>
        public List<tbt_CreditCard> DeleteTbt_CreditCard(string strContractCode, string strBillingOCC)
        {
            try
            {
                //Delete data from DB
                List<tbt_CreditCard> deletedList = base.DeleteTbt_CreditCard(strContractCode, strBillingOCC);

                //Insert Log
                if (deletedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                    logData.TableName = TableName.C_TBL_NAME_CREDIT_CARD;
                    logData.TableData = CommonUtil.ConvertToXml(deletedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return deletedList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// To delete data from tbt_AutoTransferBankAccount
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strBillingOCC"></param>
        /// <returns></returns>
        public List<tbt_AutoTransferBankAccount> DeleteTbt_AutoTransferBankAccount(string strContractCode, string strBillingOCC)
        {
            try
            {
                //Delete data from DB
                List<tbt_AutoTransferBankAccount> deletedList = base.DeleteTbt_autoTransferBankAccountdata(strContractCode, strBillingOCC);

                //Insert Log
                if (deletedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                    logData.TableName = TableName.C_TBL_NAME_AUTO_BANK;
                    logData.TableData = CommonUtil.ConvertToXml(deletedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return deletedList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// To update data to tbt_BillingTypeDetail
        /// </summary>
        /// <param name="lstUpdate"></param>
        /// <returns></returns>
        public List<tbt_BillingTypeDetail> UpdateTbt_BillingTypeDetail(List<tbt_BillingTypeDetail> lstUpdate)
        {
            try
            {
                List<tbt_BillingTypeDetail> lst = base.UpdateTbt_BillingTypeDetailData(CommonUtil.ConvertToXml_Store<tbt_BillingTypeDetail>(lstUpdate));

                #region Log

                if (lst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Update,
                        TableName = TableName.C_TBL_NAME_BILLING_TYPE_DETAIL,
                        TableData = CommonUtil.ConvertToXml(lst)
                    };

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                #endregion

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// To update data to tbt_CreditCard
        /// </summary>
        /// <param name="lstUpdate"></param>
        /// <returns></returns>
        public List<tbt_CreditCard> UpdateTbt_CreditCard(List<tbt_CreditCard> lstUpdate)
        {
            try
            {
                List<tbt_CreditCard> lst = base.UpdateTbt_CreditCard(CommonUtil.ConvertToXml_Store<tbt_CreditCard>(lstUpdate));

                #region Log

                if (lst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Update,
                        TableName = TableName.C_TBL_NAME_CREDIT_CARD,
                        TableData = CommonUtil.ConvertToXml(lst)
                    };

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                #endregion

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// To update data to tbt_AutoTransferBankAccount
        /// </summary>
        /// <param name="lstUpdate"></param>
        /// <returns></returns>
        public List<tbt_AutoTransferBankAccount> UpdateTbt_AutoTransferBankAccount(List<tbt_AutoTransferBankAccount> lstUpdate)
        {
            try
            {
                List<tbt_AutoTransferBankAccount> lst = base.UpdateTbt_AutoTransferBankAccountData(CommonUtil.ConvertToXml_Store<tbt_AutoTransferBankAccount>(lstUpdate));

                #region Log

                if (lst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Update,
                        TableName = TableName.C_TBL_NAME_AUTO_BANK,
                        TableData = CommonUtil.ConvertToXml(lst)
                    };

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                #endregion

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public List<doTbt_MonthlyBillingHistoryList> GetBillingHistoryList(string ContractCode, string BillingOCC)
        //{
        //    try
        //    {
        //        List<doTbt_MonthlyBillingHistoryList> result = base.GetBillingHistoryListData(ContractCode, BillingOCC);
        //        if (result.Count > 0)
        //        {
        //            return result;
        //        }
        //        else
        //            return null;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public List<doBillingTypeDetailList> GetBillingTypeDetailList(string ContractCode, string BillingOCC)
        //{
        //    try
        //    {
        //        List<doBillingTypeDetailList> result = base.GetBillingTypeDetailList(ContractCode, BillingOCC);
        //        return result;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}


        #endregion

        #region Method Override
        /// <summary>
        /// Get Tbt_CreditCardForView
        /// </summary>
        /// <param name="contractCode">Contract code</param>
        /// <param name="billingOCC">Billing OCC</param>
        /// <returns></returns>
        public override List<dtTbt_CreditCardForView> GetTbt_CreditCardForView(string contractCode, string billingOCC)
        {
            ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            var list = base.GetTbt_CreditCardForView(contractCode, billingOCC);

            // Misc Mapping  
            MiscTypeMappingList miscMapping = new MiscTypeMappingList();
            miscMapping.AddMiscType(list.ToArray());
            handlerCommon.MiscTypeMappingList(miscMapping);

            CommonUtil.MappingObjectLanguage<dtTbt_CreditCardForView>(list);
            return list;
        }

        /// <summary>
        /// To get Billing basic data for view
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <returns></returns>
        public override List<dtTbt_BillingBasicForView> GetTbt_BillingBasicForView(string contractCode, string billingOCC)
        {
            ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            var list = base.GetTbt_BillingBasicForView(contractCode, billingOCC);

            //// Misc Mapping  
            //MiscTypeMappingList miscMapping = new MiscTypeMappingList();
            //miscMapping.AddMiscType(list.ToArray());
            //handlerCommon.MiscTypeMappingList(miscMapping);

            CommonUtil.MappingObjectLanguage<dtTbt_BillingBasicForView>(list);
            return list;
        }

        /// <summary>
        /// To get Billing target data for view
        /// </summary>
        /// <param name="billingTargetCode"></param>
        /// <param name="c_CUST_TYPE"></param>
        /// <returns></returns>
        public override List<dtTbt_BillingTargetForView> GetTbt_BillingTargetForView(string billingTargetCode, string c_CUST_TYPE)
        {
            ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            var list = base.GetTbt_BillingTargetForView(billingTargetCode, c_CUST_TYPE);

            //// Misc Mapping  
            //MiscTypeMappingList miscMapping = new MiscTypeMappingList();
            //miscMapping.AddMiscType(list.ToArray());
            //handlerCommon.MiscTypeMappingList(miscMapping);

            CommonUtil.MappingObjectLanguage<dtTbt_BillingTargetForView>(list);
            return list;
        }

        #endregion

        /// <summary>
        /// To get last working day
        /// </summary>
        /// <param name="checkingDate"></param>
        /// <returns></returns>
        public DateTime GetLastWorkingDay(DateTime? checkingDate)
        {
            var list = base.GetLastWorkingDay(checkingDate);
            DateTime returnDate = DateTime.Now;

            if (list.Count > 0)
            {
                returnDate = Convert.ToDateTime(list[0]);
            }

            return returnDate;
        }

        /// <summary>
        /// To get list of billing detail that Issue invoice (Auto transfer,Credit card)
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <returns></returns>
        public List<tbt_BillingDetail> GetBillingDetailAutoTransferList(string contractCode, string billingOCC)
        {
            return base.GetBillingDetailAutoTransferList(contractCode, billingOCC, PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT, PaymentStatus.C_PAYMENT_STATUS_DETAIL_AUTO_CREDIT, null, null);

        }

        /// <summary>
        /// To get Auto transfer bank account data for view
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <returns></returns>
        public List<dtAutoTransferBankAccountForView> GetAutoTransferBankAccountForView(string contractCode, string billingOCC)
        {

            var result = base.GetAutoTransferBankAccountForView(contractCode, billingOCC, MiscType.C_ACCOUNT_TYPE, MiscType.C_SHOW_AUTO_TRANSFER_RESULT);
            CommonUtil.MappingObjectLanguage<dtAutoTransferBankAccountForView>(result);

            return result;
        }

        /// <summary>
        /// To get credit card data for view
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <returns></returns>
        public List<dtCreditCardForView> GetCreditCardForView(string contractCode, string billingOCC)
        {
            var result = base.GetCreditCardForView(contractCode, billingOCC, MiscType.C_CREDIT_CARD_TYPE);
            CommonUtil.MappingObjectLanguage<dtCreditCardForView>(result);

            return result;
        }

        //Add B y Sommai P., Nov 11, 2013
        /// <summary>
        /// get billing basic by credit note no.
        /// </summary>
        /// <param name="CreditNoteNo"></param>
        /// <returns></returns>
        public tbt_BillingBasic GetBillingBasicByCreditNoteNo(string CreditNoteNo)
        {
            try
            {

                tbt_BillingBasic result = base.GetBillingBasicByCreditNoteNo(CreditNoteNo).FirstOrDefault();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Insert cancel refund deposit fee transaction after cancel credit note.
        /// </summary>
        public bool InsertDepositFeeCancelRefund(string ContractCode, string BillingOCC, string CreditNoteNo, decimal CancelAmount, decimal CancelAmountUsd, string CancelAmountCurrencyType)
        {
            try
            {
                tbt_Depositfee doDepositFee = new tbt_Depositfee()
                {
                    ContractCode = ContractCode,
                    BillingOCC = BillingOCC,
                    DepositFeeNo = 0,  //Max running + 1 Move logic to sp_BL_InsertTbt_Depositfee
                    ProcessDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                    DepositStatus = DepositStatus.C_DEPOSIT_STATUS_CANCEL_REFUND,
                    ProcessAmount = CancelAmount,
                    ProcessAmountUsd = CancelAmountUsd,
                    ProcessAmountCurrencyType = CancelAmountCurrencyType,
                    ReceivedFee = null,
                    InvoiceNo = null,
                    ReceiptNo = null,
                    CreditNoteNo = CreditNoteNo,
                    SlideBillingCode = null,
                    RevenueNo = null,
                    CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                    CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                    UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                    UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime
                };

                var result = CreateTbt_Depositfee(doDepositFee);
                return result.Count > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //End Add

        // Akat K. 2014-05-21 Update Billing Basic
        /// <summary>
        /// get billing basic by credit note no.
        /// </summary>
        /// <param name="CreditNoteNo"></param>
        /// <returns></returns>
        public tbt_BillingBasic UpdateDebtTracingOffice(string billingTargetCode, string billingOfficeCode)
        {
            try
            {
                tbt_BillingBasic result = base.UpdateDebtTracingOffice(
                    billingTargetCode, billingOfficeCode,
                    CommonUtil.dsTransData.dtUserData.EmpNo,
                    CommonUtil.dsTransData.dtOperationData.ProcessDateTime).FirstOrDefault();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<tbt_BillingBasic> GetTbt_BillingBasic(string contractCode, string billingOCC, string c_CURRENCY_LOCAL, string c_CURRENCY_US)
        {
            throw new NotImplementedException();
        }

        public List<dtBillingBasicForRentalList> GetBillingBasicForRentalList(string contractCode)
        {
            return base.GetBillingBasicForRentalList(contractCode, CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US);
        }

        public List<doGetBalanceDepositByBillingCode> GetBalanceDepositByBillingCode(string strContractCode, string strBillingOCC)
        {
            return this.GetBalanceDepositByBillingCode(strContractCode, strBillingOCC, CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US);
        }

        //public List<doTbt_BillingBasic> GetBillingBasic(string contractCode, string billingOCC, string billingTargetCode, string billingClientCode, string billingOfficeCode)
        //{
        //    throw new NotImplementedException();
        //}

        //public List<doTbt_MonthlyBillingHistoryList> GetBillingHistoryList(string ContractCode, string BillingOCC)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
