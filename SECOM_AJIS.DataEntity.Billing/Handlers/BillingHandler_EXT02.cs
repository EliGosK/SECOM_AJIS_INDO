using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net;

using System.Data;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
using System.IO;
using System.Globalization;

using System.Transactions;

using CSI.WindsorHelper;
using SECOM_AJIS.Common;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Billing;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util.ConstantValue;

using SECOM_AJIS.DataEntity.Billing.CustomEntity;
using SECOM_AJIS.DataEntity.Contract;

// Used by Narupon W.
namespace SECOM_AJIS.DataEntity.Billing
{
    public partial class BillingHandler : BizBLDataEntities, IBillingHandler
    {
        //BLP014 - calculate difference of monthly fee

        /// <summary>
        /// Process BLP014 - Calculate difference monthly fee
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <param name="changeDate"></param>
        /// <param name="monthlyBillingAmount"></param>
        /// <returns></returns>
        public AdjustOnNextPeriod CalculateDifferenceMonthlyFee(string contractCode, string billingOCC, DateTime changeDate, decimal monthlyBillingAmount, string callerObject)
        {
            var billingBasic = this.GetBillingBasic(contractCode, billingOCC, null, null, null, CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US);
            AdjustOnNextPeriod billingAdjustOnNextPeriod = null;

            if (billingBasic.Count == 0)
            {
                return null;
            }

            var iChangeDate = Convert.ToInt32(changeDate.ToString("yyyyMMdd"));
            var iLastBillingDate_plus2day = Convert.ToInt32(billingBasic[0].LastBillingDate.Value.AddDays(2).ToString("yyyyMMdd"));
            var iLastBillingDate = Convert.ToInt32(billingBasic[0].LastBillingDate.Value.ToString("yyyyMMdd"));


            // Case #1 : Change monthly fee
            if (monthlyBillingAmount > 0 || callerObject == ProcessID.C_PROCESS_ID_MANAGE_BILLING_BASIC_RESUME || callerObject == ProcessID.C_PROCESS_ID_MANAGE_BILLING_BASIC_STOP)
            {
                // Set start / end date
                DateTime dtBillingStartDate = changeDate;
                //DateTime dtBillingEndDate =  billingBasic[0].LastBillingDate.HasValue? billingBasic[0].LastBillingDate.Value.AddDays(1) : new DateTime(1,1,1);

                DateTime dtBillingEndDate = billingBasic[0].LastBillingDate.HasValue ? billingBasic[0].LastBillingDate.Value : new DateTime(1, 1, 1);


                if (iChangeDate <= iLastBillingDate)
                {
                    //decimal decNewBillingAmount = this.CalCulateBillingAmountPerHistory(dtBillingStartDate, dtBillingEndDate, monthlyBillingAmount, billingBasic[0].CalDailyFeeStatus);
                    //decimal decOldBillingAmount = this.CalculateBillingAmount(contractCode, billingOCC, billingBasic[0].CalDailyFeeStatus, dtBillingStartDate, dtBillingEndDate);
                    //decimal decDiffBillingAmount = 0;

                    //if (billingBasic[0].AdjustBillingPeriodAmount != null)
                    //{
                    //    decDiffBillingAmount = (decNewBillingAmount - decOldBillingAmount) + Convert.ToDecimal(billingBasic[0].AdjustBillingPeriodAmount);
                    //}
                    //else
                    //{
                    //    decDiffBillingAmount = (decNewBillingAmount - decOldBillingAmount);
                    //}

                    decimal decDiffMonthlyBillingAmount = monthlyBillingAmount - Convert.ToDecimal(billingBasic[0].MonthlyBillingAmount);
                    decimal decDiffBillingAmount = this.CalCulateBillingAmountPerHistory(dtBillingStartDate, dtBillingEndDate, decDiffMonthlyBillingAmount, billingBasic[0].CalDailyFeeStatus) + Convert.ToDecimal(billingBasic[0].AdjustBillingPeriodAmount);
                    decDiffBillingAmount = RoundUp(decDiffBillingAmount, 2); //Add by Jutarat A. on 27012014

                    if (monthlyBillingAmount > Convert.ToDecimal(billingBasic[0].MonthlyBillingAmount)) // In case increase monthly fee, generate billing detail of "Difference of contract fee"
                    {
                        if (decDiffBillingAmount > 0)
                        {
                            DateTime? autoTransferDate = null;
                            if (billingBasic[0].PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER || billingBasic[0].PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER)
                            {
                                autoTransferDate = this.GetNextAutoTransferDate(contractCode, billingOCC, billingBasic[0].PaymentMethod);

                                if (autoTransferDate.HasValue == false)
                                {
                                    billingBasic[0].PaymentMethod = PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER;
                                }
                            }

                            // Billing type
                            var billingTypeDetail = this.GetBillingTypeDetailContinues(contractCode, billingOCC, BillingTypeGroup.C_BILLING_TYPE_GROUP_CONTINUES);

                            // CREATE Billing detail
                            if (billingTypeDetail.Count > 0)
                            {
                                string strBillingTypeCode = this.GetBillingTypeDifferenceFee(billingTypeDetail[0].BillingTypeCode);
                                // Prepare BillingDetail (data object)
                                tbt_BillingDetail billingDetail = new tbt_BillingDetail()
                                {
                                    ContractCode = contractCode,
                                    BillingOCC = billingOCC,
                                    IssueInvDate = DateTime.Now,
                                    IssueInvFlag = true,
                                    BillingTypeCode = strBillingTypeCode,
                                    BillingAmount = decDiffBillingAmount,
                                    BillingStartDate = dtBillingStartDate,
                                    BillingEndDate = dtBillingEndDate,
                                    PaymentMethod = billingBasic[0].PaymentMethod,
                                    PaymentStatus = ((billingBasic[0].PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER || billingBasic[0].PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER) ?
                                                    PaymentStatus.C_PAYMENT_STATUS_DETAIL_AUTO_CREDIT :
                                                    PaymentStatus.C_PAYMENT_STATUS_DETAIL_BANK_COLLECT),
                                    AutoTransferDate = autoTransferDate,
                                    StartOperationDate = billingBasic[0].StartOperationDate

                                };

                                IRentralContractHandler rentalContralHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                                List<dtTbt_RentalContractBasicForView> dtRentalContract = rentalContralHandler.GetTbt_RentalContractBasicForView(contractCode);
                                if (dtRentalContract.Count > 0)
                                {
                                    billingDetail.ContractOCC = dtRentalContract[0].LastOCC;
                                }
                                else
                                {
                                    ISaleContractHandler saleHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                                    List<tbt_SaleBasic> dtSaleBasic = saleHandler.GetTbt_SaleBasic(contractCode, null, true);
                                    if (dtSaleBasic != null && dtSaleBasic.Count > 0)
                                    {
                                        billingDetail.ContractOCC = dtSaleBasic[0].OCC;
                                    }
                                }
                                billingDetail.ForceIssueFlag = false;
                                // CREATE
                                this.ManageBillingDetail(billingDetail);

                                billingAdjustOnNextPeriod = new AdjustOnNextPeriod(); // set AdjustOnNextPeriod to empty

                            }
                        }
                        else if (decDiffBillingAmount < 0)
                        {
                            billingAdjustOnNextPeriod = new AdjustOnNextPeriod()
                            {
                                AdjustType = AdjustType.C_ADJUST_TYPE_REDUCT,
                                AdjustBillingPeriodAmount = decDiffBillingAmount,
                                AdjustBillingPeriodStartDate = dtBillingStartDate,
                                AdjustBillingPeriodEndDate = dtBillingEndDate
                            };
                        }
                        else // decDiffBillingAmount == billingBasic[0].AdjustBillingPeriodAmount
                        {
                            billingAdjustOnNextPeriod = new AdjustOnNextPeriod(); // set AdjustOnNextPeriod to empty
                        }

                    }
                    else if (monthlyBillingAmount < Convert.ToDecimal(billingBasic[0].MonthlyBillingAmount)) // In case decrease monthly fee, calculate "Adjust-on-next-period amount"
                    {
                        if (decDiffBillingAmount > 0)
                        {
                            billingAdjustOnNextPeriod = new AdjustOnNextPeriod()
                                                        {
                                                            AdjustType = AdjustType.C_ADJUST_TYPE_ADD,
                                                            AdjustBillingPeriodAmount = decDiffBillingAmount,
                                                            AdjustBillingPeriodStartDate = dtBillingStartDate,
                                                            AdjustBillingPeriodEndDate = dtBillingEndDate
                                                        };
                        }
                        else if (decDiffBillingAmount < 0)
                        {
                            billingAdjustOnNextPeriod = new AdjustOnNextPeriod()
                                                        {
                                                            AdjustType = AdjustType.C_ADJUST_TYPE_REDUCT,
                                                            AdjustBillingPeriodAmount = decDiffBillingAmount,
                                                            AdjustBillingPeriodStartDate = dtBillingStartDate,
                                                            AdjustBillingPeriodEndDate = dtBillingEndDate
                                                        };
                        }
                        else
                        {
                            billingAdjustOnNextPeriod = new AdjustOnNextPeriod();
                        }

                    }
                }
            }
            else // Case #2 : Monthly billing amount = 0, and it's not during stop , cancel contract   --- 4.
            {


                if (iChangeDate >= iLastBillingDate_plus2day)
                {
                    // Set start / end date
                    DateTime dtBillingStartDate = billingBasic[0].LastBillingDate.HasValue ? billingBasic[0].LastBillingDate.Value.AddDays(1) : new DateTime(1,1,1);
                    DateTime dtBillingEndDate = changeDate.AddDays(-1); // tt

                    decimal decBillingAmount = this.CalculateBillingAmount(contractCode, billingOCC, billingBasic[0].CalDailyFeeStatus, dtBillingStartDate, dtBillingEndDate);

                    DateTime? autoTransferDate = null;
                    if (billingBasic[0].PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER || billingBasic[0].PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER)
                    {
                        autoTransferDate = this.GetNextAutoTransferDate(contractCode, billingOCC, billingBasic[0].PaymentMethod);

                        if (autoTransferDate.HasValue == false)
                        {
                            billingBasic[0].PaymentMethod = PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER;
                        }
                    }

                    // Billing type
                    var billingTypeDetail = this.GetBillingTypeDetailContinues(contractCode, billingOCC, BillingTypeGroup.C_BILLING_TYPE_GROUP_CONTINUES);

                    // CREATE Billing detail
                    tbt_BillingDetail billingDetail = new tbt_BillingDetail();
                    tbt_BillingDetail billingDetail_manage = new tbt_BillingDetail();
                    if (billingTypeDetail.Count > 0)
                    {
                        string strBillingTypeCode = this.GetBillingTypeDifferenceFee(billingTypeDetail[0].BillingTypeCode);
                        // Prepare BillingDetail (data object)
                        billingDetail = new tbt_BillingDetail()
                        {
                            ContractCode = contractCode,
                            BillingOCC = billingOCC,
                            IssueInvDate = DateTime.Now,
                            IssueInvFlag = true,
                            BillingTypeCode = strBillingTypeCode,
                            BillingAmount = decBillingAmount,
                            BillingStartDate = dtBillingStartDate,
                            BillingEndDate = dtBillingEndDate,
                            PaymentMethod = billingBasic[0].PaymentMethod,
                            PaymentStatus = ((billingBasic[0].PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER || billingBasic[0].PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER) ?
                                            PaymentStatus.C_PAYMENT_STATUS_DETAIL_AUTO_CREDIT :
                                            PaymentStatus.C_PAYMENT_STATUS_DETAIL_BANK_COLLECT),
                            AutoTransferDate = autoTransferDate,
                            StartOperationDate = billingBasic[0].StartOperationDate

                        };

                        IRentralContractHandler rentalContralHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                        List<dtTbt_RentalContractBasicForView> dtRentalContract = rentalContralHandler.GetTbt_RentalContractBasicForView(contractCode);
                        if (dtRentalContract.Count > 0)
                        {
                            billingDetail.ContractOCC = dtRentalContract[0].LastOCC;
                        }
                        else
                        {
                            ISaleContractHandler saleHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                            List<tbt_SaleBasic> dtSaleBasic = saleHandler.GetTbt_SaleBasic(contractCode, null, true);
                            if (dtSaleBasic != null && dtSaleBasic.Count > 0)
                            {
                                billingDetail.ContractOCC = dtSaleBasic[0].OCC;
                            }
                        }
                        billingDetail.ForceIssueFlag = false;
                        // CREATE !
                        billingDetail_manage = this.ManageBillingDetail(billingDetail);
                    }

                    // CREATE Invoice
                    tbt_Invoice newInvoice = new tbt_Invoice()
                    {
                        AutoTransferDate = autoTransferDate,
                        BillingTargetCode = billingBasic[0].BillingTargetCode,
                        BillingTypeCode = billingDetail.BillingTypeCode,
                        InvoicePaymentStatus = ((billingBasic[0].PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER || billingBasic[0].PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER) ?
                                            PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT :
                                            PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT),
                        IssueInvFlag = true,
                        PaymentMethod = billingBasic[0].PaymentMethod,
                        IssueInvDate = changeDate,
                    };

                    // CREATE !
                    List<tbt_BillingDetail> billingDetailList = new List<tbt_BillingDetail>();
                    billingDetailList.Add(billingDetail_manage);
                    this.ManageInvoiceByCommand(newInvoice, billingDetailList, false);

                }
                else if (iChangeDate <= iLastBillingDate)
                {
                    var invoice_betweenChangeDate = this.GetInvoiceOfChangeDate(contractCode, billingOCC, changeDate);

                    string[] statusList = {PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT ,
                                              PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT ,
                                              PaymentStatus.C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK ,
                                              PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK ,
                                              PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK ,
                                              PaymentStatus.C_PAYMENT_STATUS_COUNTER_BAL
                                             };

                    if (invoice_betweenChangeDate.Count > 0 && statusList.Contains(invoice_betweenChangeDate[0].InvoicePaymentStatus))
                    {

                        // Cancel Invoice !!

                        var header = GetTbt_Invoice(invoice_betweenChangeDate[0].InvoiceNo, null);  // null mean Lastest InvoiceOCC
                        List<tbt_BillingDetail> detail = new List<tbt_BillingDetail>();
                        if (header.Count > 0)
                        {
                            detail = this.GetTbt_BillingDetailOfInvoice(header[0].InvoiceNo, header[0].InvoiceOCC);

                            this.UpdateInvoicePaymentStatus(header[0], detail, PaymentStatus.C_PAYMENT_STATUS_CANCEL);
                        }

                    }
                }
            }

            if (callerObject != ProcessID.C_PROCESS_ID_MANAGE_BILLING_BASIC_CANCEL)
            {
                // Create billing history
                tbt_MonthlyBillingHistory billingHistory = new tbt_MonthlyBillingHistory()
                {
                    ContractCode = contractCode,
                    BillingOCC = billingOCC,
                    MonthlyBillingAmount = monthlyBillingAmount,
                    BillingStartDate = changeDate
                };
                this.CreateMonthlyBillingHistory(billingHistory);
            }


            return billingAdjustOnNextPeriod;
        }

        /// <summary>
        /// Process - get billing detail by invoice
        /// </summary>
        /// <param name="invoiceNo"></param>
        /// <param name="invoiceOCC"></param>
        /// <returns></returns>
        public List<tbt_BillingDetail> GetTbt_BillingDetailOfInvoice(string invoiceNo, int? invoiceOCC)
        {
            return base.GetTbt_BillingDetailOfInvoice(invoiceNo, invoiceOCC, PaymentStatus.C_PAYMENT_STATUS_CANCEL, PaymentStatus.C_PAYMENT_STATUS_PAYMENT_MATCH_CANCELLED, PaymentStatus.C_PAYMENT_STATUS_NOTE_FAIL, PaymentStatus.C_PAYMENT_STATUS_POST_FAIL);
        }

        /// <summary>
        /// Process - get download auto transfer bank file
        /// </summary>
        /// <param name="secomAccountID"></param>
        /// <param name="autoTransferDateFrom"></param>
        /// <param name="autoTransferDateTo"></param>
        /// <param name="generateDateFrom"></param>
        /// <param name="generateDateTo"></param>
        /// <returns></returns>
        public List<dtDownloadAutoTransferBankFile> GetDownloadAutoTransferBankFile(int? secomAccountID, DateTime? autoTransferDateFrom, DateTime? autoTransferDateTo, DateTime? generateDateFrom, DateTime? generateDateTo)
        {
            var list = base.GetDownloadAutoTransferBankFile(secomAccountID, autoTransferDateFrom, autoTransferDateTo, generateDateFrom, generateDateTo);

            CommonUtil.MappingObjectLanguage<dtDownloadAutoTransferBankFile>(list);

            CultureInfo culture = null;
            if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
            {
                culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_EN);
            }
            else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
            {
                culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_EN); // if has 2 language JP => EN
            }
            else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_LC)
            {
                culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_LC);
            }

            if (culture != null)
            {
                list = list.OrderByDescending(p => p.AutoTransferDate).ThenBy(n => n.BankBranchName, StringComparer.Create(culture, false)).ThenBy(m => m.BankNameBankBranchName, StringComparer.Create(culture, false)).ToList<dtDownloadAutoTransferBankFile>();
            }

            return list;
        }

        /// <summary>
        /// Process - update monthly billing amount
        /// </summary>
        /// <param name="billingBasicList"></param>
        /// <returns></returns>
        public List<tbt_BillingBasic> UpdateMonthlyBillingAmount(List<tbt_BillingBasic> billingBasicList)
        {
            string xml_doTbtBillingBasic = CommonUtil.ConvertToXml_Store(billingBasicList);
            List<tbt_BillingBasic> result = base.UpdateMonthlyBillingAmount(xml_doTbtBillingBasic);

            //Insert Log
            if (result.Count > 0)
            {

                doTransactionLog logData = new doTransactionLog();
                logData.TransactionType = doTransactionLog.eTransactionType.Update;
                logData.TableName = TableName.C_TBL_NAME_BILLING_BASIC;
                logData.TableData = CommonUtil.ConvertToXml(result);
                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);
            }

            return result;
        }

        /// <summary>
        /// Get Difference contract billing type
        /// </summary>
        /// <param name="BillingTypeCode"></param>
        /// <returns></returns>
        public string GetBillingTypeDifferenceFee(string BillingTypeCode)
        {
            try
            {

                string strMappingDiffBillingType = null;

                if (BillingTypeCode == BillingType.C_BILLING_TYPE_SERVICE)
                {
                    strMappingDiffBillingType = BillingType.C_BILLING_TYPE_DIFF_SERVICE;
                }
                else if (BillingTypeCode == BillingType.C_BILLING_TYPE_SG)
                {
                    strMappingDiffBillingType = BillingType.C_BILLING_TYPE_DIFF_SG;
                }
                else if (BillingTypeCode == BillingType.C_BILLING_TYPE_MA)
                {
                    strMappingDiffBillingType = BillingType.C_BILLING_TYPE_DIFF_MA;
                }
                else if (BillingTypeCode == BillingType.C_BILLING_TYPE_MA_RESULT_BASE)
                {
                    strMappingDiffBillingType = BillingType.C_BILLING_TYPE_DIFF_MA_RESULT_BASE;
                }
                else
                {
                    strMappingDiffBillingType = null;
                }
                return strMappingDiffBillingType;
            }
            catch (Exception)
            {
                throw;
            }

        }

    }

}
