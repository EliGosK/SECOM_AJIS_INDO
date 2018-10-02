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



namespace SECOM_AJIS.DataEntity.Billing
{
    public partial class BillingBatchProcessHandler : BizBLDataEntities
    {
        #region Get data
        /// <summary>
        /// Get billing detail data (by process)
        /// </summary>
        /// <param name="batchDate"></param>
        /// <returns></returns>
        public List<dtBillingDetailByProcess> GetBillingDetailByProcess(DateTime? batchDate)
        {
            return base.GetBillingDetailByProcess(PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER,
                                                  PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER,
                                                  PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER,
                                                  PaymentMethod.C_PAYMENT_METHOD_MESSENGER_TRANSFER,
                                                  PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT,
                                                  PaymentStatus.C_PAYMENT_STATUS_DETAIL_AUTO_CREDIT,
                                                  BillingType.C_BILLING_TYPE_SERVICE,
                                                  BillingType.C_BILLING_TYPE_MA,
                                                  BillingType.C_BILLING_TYPE_SG,
                                                  BillingType.C_BILLING_TYPE_DURING_STOP_SERVICE,
                                                  BillingType.C_BILLING_TYPE_DURING_STOP_MA,
                                                  BillingType.C_BILLING_TYPE_DURING_STOP_SG,
                                                  BillingTypeGroup.C_BILLING_TYPE_GROUP_CONTINUES,
                                                  batchDate
                                                );
        }

        /// <summary>
        /// Get billing detail data for create invoice
        /// </summary>
        /// <param name="BatchDate"></param>
        /// <returns></returns>
        public List<dtBillingDetailForCreateInvoice> GetBillingDetailForCreateInvoice(DateTime? BatchDate)
        {
            return base.GetBillingDetailForCreateInvoice(CustomerType.C_CUST_TYPE_JURISTIC,
                                                         PaymentStatus.C_PAYMENT_STATUS_CANCEL,
                                                         PaymentStatus.C_PAYMENT_STATUS_PAYMENT_MATCH_CANCELLED,
                                                         PaymentStatus.C_PAYMENT_STATUS_NOTE_FAIL,
                                                         PaymentStatus.C_PAYMENT_STATUS_POST_FAIL,
                                                         SeparateInvType.C_SEP_INV_CONTRACT_CODE_ASCE,
                                                         SeparateInvType.C_SEP_INV_SORT_ASCE,
                                                         SeparateInvType.C_SEP_INV_SORT_DESC,
                                                         SeparateInvType.C_SEP_INV_SAME_TYPE_CONTRACT_CODE_ASCE,
                                                         SeparateInvType.C_SEP_INV_SAME_TYPE_SORT_ASCE,
                                                         SeparateInvType.C_SEP_INV_SAME_TYPE_SORT_DESC,
                                                         BillingTypeGroup.C_BILLING_TYPE_GROUP_CONTINUES,
                                                         BillingTypeGroup.C_BILLING_TYPE_GROUP_DEPOSIT,
                                                         SeparateInvType.C_SEP_INV_EACH_CONTRACT,
                                                         BatchDate
                                                        );
        }

        #endregion

        #region Batch Process

        // BLP020

        /// <summary>
        /// Batch process BLP020 - Manage billing detail monthly fee process
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="BatchDate"></param>
        /// <returns></returns>
        public doBatchProcessResult BLP020_ManageBillingDetailMonthlyFeeProcess(string UserId, DateTime BatchDate)
        {
            doBatchProcessResult result = new doBatchProcessResult();
            BillingHandler billingHandler = new BillingHandler();
            
            DateTime dtBillingStartDate;
            DateTime dtBillingEndDate;
            bool bHasNextProcess = true;


            // Initial value of doBatchProcessResult
            result.Result = false;
            result.Total = 0;
            result.Complete = 0;
            result.Failed = 0;
            result.ErrorMessage = string.Empty;
            result.BatchUser = UserId;

            // Error list
            List<string> BillingBasic_ErrorList = new List<string>();

            while (bHasNextProcess)
            {

                var billingDetailList = this.GetBillingDetailByProcess(BatchDate);

                billingDetailList = (from p in billingDetailList
                                     where BillingBasic_ErrorList.Contains(string.Format("{0}--{1}--{2}", p.ContractCode, p.BillingOCC, (p.LastBillingDate.HasValue ? p.LastBillingDate.Value.ToString("yyyyMMdd") : ""))) == false
                                     select p).ToList<dtBillingDetailByProcess>();

                bHasNextProcess = (billingDetailList.Count > 0);


                if (billingDetailList.Count > 0)
                {
                    // Update total value
                    result.Total += billingDetailList.Count;

                    foreach (var doGenerate in billingDetailList)
                    {

                        using (TransactionScope scope = new TransactionScope())
                        {
                        try
                        {

                                // Assign value of dtBillingStartDate , dtBillingEndDate
                                dtBillingStartDate = doGenerate.LastBillingDate.Value.AddDays(1);
                                int iBillingCycle = Convert.ToInt32(doGenerate.BillingCycle);
                                if (CommonUtil.IsNullOrEmpty(doGenerate.AdjustEndDate))
                                {
                                    // dtBillingEndDate = dtBillingStartDate + doGen.BillingCycle (month)  - 1 day

                                    dtBillingEndDate = dtBillingStartDate.AddMonths(iBillingCycle).AddDays(-1);
                                }
                                else
                                {
                                    DateTime dtBillingEndDateOfPeriod = dtBillingStartDate.AddMonths(iBillingCycle).AddDays(-1);
                                    if (doGenerate.AdjustEndDate.Value > dtBillingEndDateOfPeriod)
                                    {
                                        dtBillingEndDate = dtBillingEndDateOfPeriod;
                                    }
                                    else
                                    {
                                        dtBillingEndDate = doGenerate.AdjustEndDate.Value;
                                    }
                                }

                                decimal decBillingAmount = billingHandler.CalculateBillingAmount(doGenerate.ContractCode,
                                                                                 doGenerate.BillingOCC,
                                                                                 doGenerate.CalDailyFeeStatus,
                                                                                 dtBillingStartDate,
                                                                                 dtBillingEndDate
                                                                           );
                                if (decBillingAmount == 0)
                                {
                                    // Keep error
                                    BillingBasic_ErrorList.Add(string.Format("{0}--{1}--{2}"
                                                            , doGenerate.ContractCode
                                                            , doGenerate.BillingOCC
                                                            , (doGenerate.LastBillingDate.HasValue ? doGenerate.LastBillingDate.Value.ToString("yyyyMMdd") : "")));


                                scope.Dispose();

                                result.Failed++;
                                    result.ErrorMessage += string.Format("ContractCode {0} BillingOCC {1} has Error : {2}\n", doGenerate.ContractCode, doGenerate.BillingOCC, "Billing amount = 0");

                                    continue; // go to next loop
                                }

                                decimal? decCalBillingAmount; //-----------------------*
                                decimal? decCalAdjustBillingPeriodAmount; //-----------------------* For Billing basic
                                decimal? decCalAdjustBillingAmount; //-----------------------* For Billing detail

                                if (Math.Abs(Convert.ToDecimal(doGenerate.AdjustBillingPeriodAmount)) > decBillingAmount
                                    && doGenerate.AdjustBillingPeriodAmount.HasValue
                                    && doGenerate.AdjustBillingPeriodAmount < 0)
                                {
                                    decCalBillingAmount = 0;
                                    decCalAdjustBillingPeriodAmount = decBillingAmount + Convert.ToDecimal(doGenerate.AdjustBillingPeriodAmount);
                                    decCalAdjustBillingAmount = decBillingAmount * (-1);
                                }
                                else
                                {
                                    decCalBillingAmount = decBillingAmount + Convert.ToDecimal(doGenerate.AdjustBillingPeriodAmount);
                                    decCalAdjustBillingPeriodAmount = null;
                                    decCalAdjustBillingAmount = Convert.ToDecimal(doGenerate.AdjustBillingPeriodAmount);
                                }


                                string tmpPaymentStatus = string.Empty;


                                // tmpPaymentStatus
                                if (doGenerate.PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER ||
                                    doGenerate.PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER)
                                {
                                    tmpPaymentStatus = PaymentStatus.C_PAYMENT_STATUS_DETAIL_AUTO_CREDIT;
                                }
                                else
                                {
                                    tmpPaymentStatus = PaymentStatus.C_PAYMENT_STATUS_DETAIL_BANK_COLLECT;
                                }

                                //Set First fee flag
                                //Add by Patcharee T. 21-Jun-2013
                                bool? bFirstFeeFlag = false;
                                if ((doGenerate.BillingTypeCode == BillingType.C_BILLING_TYPE_SERVICE
                                    || doGenerate.BillingTypeCode == BillingType.C_BILLING_TYPE_DIFF_SERVICE
                                    || doGenerate.BillingTypeCode == BillingType.C_BILLING_TYPE_MA
                                    || doGenerate.BillingTypeCode == BillingType.C_BILLING_TYPE_DIFF_MA
                                    || doGenerate.BillingTypeCode == BillingType.C_BILLING_TYPE_SG
                                    || doGenerate.BillingTypeCode == BillingType.C_BILLING_TYPE_DIFF_SG
                                    || doGenerate.BillingTypeCode == BillingType.C_BILLING_TYPE_MA_RESULT_BASE
                                    || doGenerate.BillingTypeCode == BillingType.C_BILLING_TYPE_DIFF_MA_RESULT_BASE)
                                    && (dtBillingStartDate == doGenerate.StartOperationDate))
                                {
                                    bFirstFeeFlag = true;
                                }
                                else
                                {
                                    bFirstFeeFlag = false;
                                }
                                //Add by Patcharee T. 21-Jun-2013

                                // PREPARE !
                               //ContractHandler icontract = new ContractHandler();
                                IContractHandler icontract = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
                                string lastOCCcontract = string.Empty; 
                                lastOCCcontract = (from x in icontract.GetContractLastOCC(doGenerate.ContractCode)
                                                     select x).First();
                                
                                tbt_BillingDetail billingDetailForSave = new tbt_BillingDetail()
                                {
                                    ContractCode = doGenerate.ContractCode,
                                    BillingOCC = doGenerate.BillingOCC,
                                    BillingDetailNo = 0, // ------------- it will get MAX()+1 (group by ContractCode ,BillingOCC) in Insert_procedure
                                    IssueInvDate = doGenerate.IssueInvDate,

                                    IssueInvFlag = true,
                                    BillingTypeCode = doGenerate.BillingTypeCode,

                                    BillingAmount = decCalBillingAmount,
                                    AdjustBillingAmount = decCalAdjustBillingAmount,
                                    BillingStartDate = dtBillingStartDate,
                                    BillingEndDate = dtBillingEndDate,
                                    PaymentMethod = doGenerate.PaymentMethod,
                                    PaymentStatus = tmpPaymentStatus,
                                    AutoTransferDate = doGenerate.AutoTransferDate,

                                    FirstFeeFlag = bFirstFeeFlag,       //Add by Patcharee T. 21-Jun-2013

                                    CreateDate = BatchDate,
                                    CreateBy = ProcessID.C_PROCESS_ID_MANAGE_BILLING_DETAIL_MONTHLY_FEE,
                                    UpdateDate = BatchDate,
                                    UpdateBy = ProcessID.C_PROCESS_ID_MANAGE_BILLING_DETAIL_MONTHLY_FEE,
                                    ForceIssueFlag = false,
                                    ContractOCC = lastOCCcontract

                                };
                                List<tbt_BillingDetail> billingDetailForSaveList = new List<tbt_BillingDetail>();
                                billingDetailForSaveList.Add(billingDetailForSave);

                                // Create Billing detail
                                billingHandler.InsertTbt_BillingDetail(CommonUtil.ConvertToXml_Store(billingDetailForSaveList));



                                // *** Update Billing basic (Header)

                                var billingBasicForUpdate = billingHandler.GetTbt_BillingBasic(doGenerate.ContractCode, doGenerate.BillingOCC, CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US);

                                // Update value
                                if (billingBasicForUpdate.Count > 0)
                                {
                                    billingBasicForUpdate[0].LastBillingDate = dtBillingEndDate;
                                    billingBasicForUpdate[0].AdjustEndDate = Convert.ToDateTime(doGenerate.AdjustEndDate).Date.CompareTo(dtBillingEndDate.Date) == 0 ? null : doGenerate.AdjustEndDate;
                                    billingBasicForUpdate[0].AdjustType = decCalAdjustBillingPeriodAmount.HasValue ? billingBasicForUpdate[0].AdjustType : null;
                                    billingBasicForUpdate[0].AdjustBillingPeriodAmount = decCalAdjustBillingPeriodAmount;
                                    billingBasicForUpdate[0].AdjustBillingPeriodStartDate = decCalAdjustBillingPeriodAmount.HasValue ? billingBasicForUpdate[0].AdjustBillingPeriodStartDate : null;
                                    billingBasicForUpdate[0].AdjustBillingPeriodEndDate = decCalAdjustBillingPeriodAmount.HasValue ? billingBasicForUpdate[0].AdjustBillingPeriodEndDate : null;
                                    billingBasicForUpdate[0].UpdateDate = BatchDate;
                                    billingBasicForUpdate[0].UpdateBy = ProcessID.C_PROCESS_ID_MANAGE_BILLING_DETAIL_MONTHLY_FEE;

                                    // UPDATE !
                                    billingHandler.UpdateTbt_BillingBasicNoLog(billingBasicForUpdate[0]);
                                }


                            scope.Complete();

                            result.Complete++;

                            }
                            catch (Exception ex)
                            {
                                // Keep error
                                BillingBasic_ErrorList.Add(string.Format("{0}--{1}--{2}"
                                                        , doGenerate.ContractCode
                                                        , doGenerate.BillingOCC
                                                        , (doGenerate.LastBillingDate.HasValue ? doGenerate.LastBillingDate.Value.ToString("yyyyMMdd") : "")));


                            scope.Dispose();

                            result.Failed++;
                                result.ErrorMessage += string.Format("ContractCode {0} BillingOCC {1} has Error : {2} {3}\n", doGenerate.ContractCode, doGenerate.BillingOCC, ex.Message, ex.InnerException != null ? ex.InnerException.Message : "");

                            }


                        }

                    }


                }

            }
            result.Result = !(result.Failed > 0);
            return result;

        }

        // BLP030

        /// <summary>
        /// Batch process BLP030 - Manage invoice process
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="BatchDate"></param>
        /// <returns></returns>
        public doBatchProcessResult BLP030_ManageInvoiceProcess(string UserId, DateTime BatchDate)
        {

            doBatchProcessResult result = new doBatchProcessResult();
            BillingHandler billingHandler = new BillingHandler();
            ICommonHandler commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            BillingDocumentHandler billingDocumentHandler = new BillingDocumentHandler();

            var billingDetailList = this.GetBillingDetailForCreateInvoice(BatchDate);

            var invoiceGroupList = (from p in billingDetailList select p.InvoiceGroup).Max();

            string strNewInvoiceNo = string.Empty;


            // Initial value of doBatchProcessResult
            result.Result = false;
            result.Total = invoiceGroupList == null ? 0 : (int)invoiceGroupList.Value; //(int)invoiceGroupList.Value; //Modify by Jutarat A. on 28062013
            result.Complete = 0;
            result.Failed = 0;
            result.ErrorMessage = string.Empty;
            result.BatchUser = UserId;

            //for (int invoiceGroupLabel = 1; invoiceGroupLabel <= (int)invoiceGroupList.Value; invoiceGroupLabel++)
            for (int invoiceGroupLabel = 1; invoiceGroupLabel <= result.Total; invoiceGroupLabel++) //Modify by Jutarat A. on 28062013
            {
                var member = (from p in billingDetailList where p.InvoiceGroup == invoiceGroupLabel select p).ToList<dtBillingDetailForCreateInvoice>();


                using (TransactionScope scope = new TransactionScope())
                {
                try
                {
                        // Get new Invoice No.
                        strNewInvoiceNo = billingHandler.GetNextRunningNoByTypeMonthYear(RunningType.C_RUNNING_TYPE_INVOICE, UserId, member[0].IssueInvDate.Value/*BatchDate*/); //Edit by Patcharee T. For get invoice no. in month of InvoiceDate 11-Jun-2013

                        string strInvoicePaymentStatus = string.Empty; //---------*

                        if (member[0].PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER ||
                            member[0].PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER)
                        {
                            strInvoicePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT;
                        }
                        else
                        {
                            strInvoicePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT;
                        }



                        // Update value of strNewInvoiceNo , strInvoicePaymentStatus -------****
                        foreach (var item in member)
                        {
                            if (item.InvoiceGroup == invoiceGroupLabel)
                            {
                                item.InvoiceNo = strNewInvoiceNo;
                                item.PaymentStatus = strInvoicePaymentStatus;

                                // Edit by Narupon W. 11 May 2012
                                item.InvoiceOCC = 1;
                                item.UpdateDate = BatchDate;
                                item.UpdateBy = ProcessID.C_PROCESS_ID_MANAGE_INVOICE;


                            }


                        }

                        // Prepare tbt_invoice
                        tbt_Invoice invoice = new tbt_Invoice()
                        {
                            InvoiceNo = strNewInvoiceNo,
                            InvoiceOCC = 1,
                            IssueInvDate = member[0].IssueInvDate,
                            AutoTransferDate = member[0].AutoTransferDate,
                            BillingTargetCode = member[0].BillingTargetCode,
                            //BillingTypeCode = member[0].BillingTypeCode,
                            BillingTypeCode = null,

                            InvoiceAmount = member[0].SumBillingAmount,
                            InvoiceAmountUsd = member[0].SumBillingAmountUsd, // Add by Jirawat J. 13/12/2016
                            InvoiceAmountCurrencyType = member[0].BillingAmountCurrencyType, // Add by Jirawat J. 13/12/2016

                            VatRate = Convert.ToDecimal(member[0].VatRate), //----
                            VatAmount = billingHandler.RoundUp((member[0].SumBillingAmount ?? 0M) * (member[0].VatRate ?? 0M), 2),  //----
                            VatAmountUsd = billingHandler.RoundUp((member[0].SumBillingAmountUsd ?? 0M) * (member[0].VatRate ?? 0M), 2),  //Add by Jirawat J. 13/12/2016
                            VatAmountCurrencyType = member[0].BillingAmountCurrencyType,  //Add by Jirawat J. 13/12/2016

                            WHTRate = Convert.ToDecimal(member[0].WhtRate),
                            WHTAmount = billingHandler.RoundUp((member[0].SumBillingAmount ?? 0M) * (member[0].WhtRate ?? 0M), 2),
                            WHTAmountUsd = billingHandler.RoundUp((member[0].SumBillingAmountUsd ?? 0M) * (member[0].WhtRate ?? 0M), 2), //Add by Jirawat J. 13/12/2016
                            WHTAmountCurrencyType = member[0].BillingAmountCurrencyType,  //Add by Jirawat J. 13/12/2016

                            InvoicePaymentStatus = strInvoicePaymentStatus,
                            IssueInvFlag = (member[0].IssueInvFlag == true && member[0].InvFormatType != InvFormatType.C_INV_FORMAT_SPECIFIC)
                                            ? true : false, //old--> member[0].IssueInvFlag,
                            FirstIssueInvDate = null,
                            FirstIssueInvFlag = null, // old--> false,
                            PaymentMethod = member[0].PaymentMethod,
                            CreateDate = BatchDate,
                            CreateBy = ProcessID.C_PROCESS_ID_MANAGE_INVOICE,
                            UpdateDate = BatchDate,
                            UpdateBy = ProcessID.C_PROCESS_ID_MANAGE_INVOICE,

                            PaidAmountIncVat = null, //Add by Jirawat J. 13/12/2016
                            PaidAmountIncVatUsd = null, //Add by Jirawat J. 13/12/2016
                            PaidAmountIncVatCurrencyType = null, //Add by Jirawat J. 13/12/2016

                            RegisteredWHTAmount = null, //Add by Jirawat J. 13/12/2016
                            RegisteredWHTAmountUsd = null, //Add by Jirawat J. 13/12/2016
                            RegisteredWHTAmountCurrencyType = null //Add by Jirawat J. 13/12/2016
                        };
                        List<tbt_Invoice> invoiceList = new List<tbt_Invoice>();
                        invoiceList.Add(invoice);

                        // Create Invoice !!
                        var Invoice_inserted = base.InsertTbt_Invoice(CommonUtil.ConvertToXml_Store(invoiceList));

                        #region Comment by Juatarat A. on 06032013 (Move)
                        /*//------ Update Invoice No. to tbt_BillingDetail ----
                        base.UpdateBillingDetailByBatch(CommonUtil.ConvertToXml_Store(member));

                        // === Create deposit fee ==
                        tbt_DepositFee depositFee = new tbt_DepositFee();
                        List<tbt_DepositFee> depositFee_list = new List<tbt_DepositFee>();
                        foreach (var item in member)
                        {
                            if (item.BillingTypeGroup == BillingTypeGroup.C_BILLING_TYPE_GROUP_DEPOSIT)
                            {
                                depositFee = new tbt_DepositFee()
                                {
                                    ContractCode = item.ContractCode,
                                    BillingOCC = item.BillingOCC,
                                    DepositFeeNo = 0,
                                    ProcessDate = BatchDate,
                                    DepositStatus = DepositStatus.C_DEPOSIT_STATUS_ISSUE_INVOICE,
                                    ProcessAmount = (item.SumBillingAmount ?? 0) + (billingHandler.RoundUp((member[0].SumBillingAmount ?? 0M) * (member[0].VatRate ?? 0M), 2)),
                                    ReceivedFee = null,
                                    InvoiceNo = strNewInvoiceNo,
                                    ReceiptNo = null,
                                    CreditNoteNo = null

                                };

                                depositFee_list.Add(depositFee);
                            }
                        }

                        if (depositFee_list.Count > 0)
                        {
                            // CREATE !!!
                            InsertTbt_Depositfee(CommonUtil.ConvertToXml_Store(depositFee_list));

                        }

                        //*** Generate Invoice report (BLR010)
                        if (member[0].InvFormatType != InvFormatType.C_INV_FORMAT_SPECIFIC)
                        {
                            billingDocumentHandler.GenerateBLR010FilePath(strNewInvoiceNo, ProcessID.C_PROCESS_ID_MANAGE_INVOICE, BatchDate);
                        }*/
                        //End Comment
                        #endregion

                        //-------- Create Tax Invoice ----
                        string strTaxInvoiceNo = string.Empty; //Add by Jutarat A. on 05062013
                        if (member[0].IssueReceiptTiming == IssueRecieptTime.C_ISSUE_REC_TIME_SAME_INV ||
                            member[0].BillingTypeGroup == BillingTypeGroup.C_BILLING_TYPE_GROUP_SALE)
                        {

                            DateTime? tmpTaxInvoiceDate = null;
                            if (member[0].BillingTypeGroup == BillingTypeGroup.C_BILLING_TYPE_GROUP_SALE)
                            {
                                tmpTaxInvoiceDate = member[0].IssueInvDate;
                            }
                            else if (member[0].PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER ||
                                     member[0].PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER)
                            {
                                tmpTaxInvoiceDate = member[0].AutoTransferDate;
                            }
                            else
                            {
                                tmpTaxInvoiceDate = member[0].IssueInvDate;  // ???
                            }

                            // Get Tax Invoice No.
                            strTaxInvoiceNo = billingHandler.GetNextRunningNoByTypeMonthYear(RunningType.C_RUNNING_TYPE_TAX_INVOICE, UserId, tmpTaxInvoiceDate.Value); //Edit by Patcharee T. For get Tax invoice no. in month of taxInvoiceDate 11-Jun-2013

                            // Prepare
                            tbt_TaxInvoice taxInvoice = new tbt_TaxInvoice()
                            {
                                TaxInvoiceNo = strTaxInvoiceNo,
                                InvoiceNo = strNewInvoiceNo,
                                InvoiceOCC = 1,
                                TaxInvoiceDate = tmpTaxInvoiceDate,
                                TaxInvoiceCanceledFlag = false,
                                TaxInvoiceIssuedFlag = (member[0].IssueInvFlag == true && member[0].InvFormatType != InvFormatType.C_INV_FORMAT_SPECIFIC)
                                                         ? true : false,// old --> member[0].IssueInvFlag,
                                CreateDate = BatchDate,
                                CreateBy = ProcessID.C_PROCESS_ID_MANAGE_INVOICE,
                                UpdateDate = BatchDate,
                                UpdateBy = ProcessID.C_PROCESS_ID_MANAGE_INVOICE

                            };
                            List<tbt_TaxInvoice> taxInvoiceList = new List<tbt_TaxInvoice>();
                            taxInvoiceList.Add(taxInvoice);

                            // CREATE 
                            var TaxInvoice_inserted = base.InsertTbt_TaxInvoice(CommonUtil.ConvertToXml_Store(taxInvoiceList));


                            #region Comment by Juatarat A. on 05062013 (Move)
                            //*** Generate tax invoice report (BLR020)
                            /*if (member[0].InvFormatType != InvFormatType.C_INV_FORMAT_SPECIFIC)
                            {
                                billingDocumentHandler.GenerateBLR020FilePath(strTaxInvoiceNo, ProcessID.C_PROCESS_ID_MANAGE_INVOICE, BatchDate);
                            }*/
                            //End Comment
                            #endregion

                        }

                        //Add by Juatarat A. on 06032013 (Move)
                        //------ Update Invoice No. to tbt_BillingDetail ----
                        base.UpdateBillingDetailByBatch(CommonUtil.ConvertToXml_Store(member));



                    // Begin add by Jirawat J. on 2016-12-13 
                    // Set InvoiceIssueList 
                    decimal? billingAmt = 0;
                    decimal? billingAmtUsd = 0;

                    double errorCode = 0;

                    if ((member[0].SumBillingAmountUsd ?? 0M) > 0)
                    {
                        billingAmt = commonHandler.ConvertCurrencyPrice(member[0].SumBillingAmountUsd, CurrencyUtil.C_CURRENCY_US, CurrencyUtil.C_CURRENCY_LOCAL, DateTime.Now, ref errorCode, null);
                        billingAmtUsd = member[0].SumBillingAmountUsd.Value;
                    }
                    else
                    {
                        billingAmt = member[0].SumBillingAmount;
                        billingAmtUsd = null;
                    }

                    tbt_InvoiceIssueList invoiceIssue = new tbt_InvoiceIssueList()
                    {
                        IssueInvDate = member[0].IssueInvDate.Value,
                        InvoiceNo = strNewInvoiceNo,
                        IssueInvDateMonth = member[0].IssueInvDate.Value.ToString("MM"),
                        IssueInvDateYear = member[0].IssueInvDate.Value.ToString("yyyy"),
                        IDNo = member[0].IDNo,
                        RealBillingClientNameEN = member[0].RealBillingClientNameEN,
                        RealBillingClientAddressEN = member[0].RealBillingClientAddressEN,
                        ProductNameEN = member[0].ProductNameEN,

                        BillingAmount = billingAmt,
                        BillingAmountUsd = billingAmtUsd,

                        CreateDate = BatchDate,
                        CreateBy = ProcessID.C_PROCESS_ID_MANAGE_INVOICE,
                        UpdateDate = BatchDate,
                        UpdateBy = ProcessID.C_PROCESS_ID_MANAGE_INVOICE
                    };

                    List<tbt_InvoiceIssueList> invoiceIssueList = new List<tbt_InvoiceIssueList>();
                    invoiceIssueList.Add(invoiceIssue);

                    // Create Invoice issue list !! 
                    var InvoiceIssue_inserted = base.InsertTbt_InvoiceIssueList(CommonUtil.ConvertToXml_Store(invoiceIssueList));

                    // End add by Jirawat J. on 2016-12-13




                    // === Create deposit fee ==
                    tbt_Depositfee depositFee = new tbt_Depositfee();
                        List<tbt_Depositfee> depositFee_list = new List<tbt_Depositfee>();
                        foreach (var item in member)
                        {
                            if (item.BillingTypeGroup == BillingTypeGroup.C_BILLING_TYPE_GROUP_DEPOSIT)
                            {
                                depositFee = new tbt_Depositfee()
                                {
                                    ContractCode = item.ContractCode,
                                    BillingOCC = item.BillingOCC,
                                    DepositFeeNo = 0,
                                    ProcessDate = BatchDate,
                                    DepositStatus = DepositStatus.C_DEPOSIT_STATUS_ISSUE_INVOICE,
                                    //ProcessAmount = (item.SumBillingAmount ?? 0) + (billingHandler.RoundUp((member[0].SumBillingAmount ?? 0M) * (member[0].VatRate ?? 0M), 2)),
                                    ProcessAmount = (item.SumBillingAmount ?? 0), //Modify by Jutarat A. on 20122013
                                    ReceivedFee = null,
                                    InvoiceNo = strNewInvoiceNo,
                                    ReceiptNo = null,
                                    CreditNoteNo = null,
                                    //Add by Jutarat A. on 22082013
                                    CreateDate = BatchDate,
                                    CreateBy = ProcessID.C_PROCESS_ID_MANAGE_INVOICE,
                                    UpdateDate = BatchDate,
                                    UpdateBy = ProcessID.C_PROCESS_ID_MANAGE_INVOICE,
                                    ProcessAmountUsd = (item.SumBillingAmountUsd ?? 0),             //Modify by Pachara S. on 20170315
                                    ProcessAmountCurrencyType = item.BillingAmountCurrencyType,     //Modify by Pachara S. on 20170315
                                    //End Add
                                };

                                depositFee_list.Add(depositFee);
                            }
                        }

                        if (depositFee_list.Count > 0)
                        {
                            // CREATE !!!
                            InsertTbt_Depositfee(CommonUtil.ConvertToXml_Store(depositFee_list));

                        }

                        //*** Generate Invoice report (BLR010)
                        if (member[0].InvFormatType != InvFormatType.C_INV_FORMAT_SPECIFIC)
                        {
                            billingDocumentHandler.GenerateBLR010FilePath(strNewInvoiceNo, ProcessID.C_PROCESS_ID_MANAGE_INVOICE, BatchDate);
                        }

                        // Comment by Jirawat Janet on 2016-12-27
                        //if (member[0].IssueReceiptTiming == IssueRecieptTime.C_ISSUE_REC_TIME_SAME_INV ||
                        //    member[0].BillingTypeGroup == BillingTypeGroup.C_BILLING_TYPE_GROUP_SALE)
                        //{
                        //    //*** Generate tax invoice report (BLR020)
                        //    if (member[0].InvFormatType != InvFormatType.C_INV_FORMAT_SPECIFIC)
                        //    {
                        //        if (String.IsNullOrEmpty(strTaxInvoiceNo) == false)
                        //        {
                        //            // Comment by Jirawat Jannet 
                        //            //billingDocumentHandler.GenerateBLR020FilePath(strTaxInvoiceNo, ProcessID.C_PROCESS_ID_MANAGE_INVOICE, BatchDate);
                        //            //throw new Exception("กำลังดำเนินการแก้ไข report BLR020");
                        //        }
                        //    }
                        //}
                    //End Add

                    scope.Complete();

                    result.Complete++;
                    }
                    catch (Exception ex)
                    {
                    scope.Dispose();

                    result.Failed++;
                        result.ErrorMessage += string.Format("ContractCode {0} BillingOCC {1} Billing detail {2} has Error : {2} {3}\n", member[0].ContractCode, member[0].BillingOCC, member[0].BillingDetailNo, ex.Message, ex.InnerException != null ? ex.InnerException.Message : "");

                        ObjectResultData.WriteTableErrorLog(DateTime.Now, ex); //Add by Jutarat A. on 15112012
                    }
                }
            }

            result.Result = !(result.Failed > 0);

            return result;

        }

        // BLP040

        /// <summary>
        /// Batch process BLP040 - Batch generate auto transfer bank file process
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="BatchDate"></param>
        /// <returns></returns>
        public doBatchProcessResult BLP040_BatchGenerateAutoTransferBankFileProcess(string UserId, DateTime BatchDate)
        {

            int iTotal = 0;
            int iComplete = 0;
            int iFail = 0;
            int iTotalTransaction = 0;
            decimal? iTotalAmount = 0;

            string strErrorMsg = string.Empty;
            string strFileName = string.Empty;
            string strPath = string.Empty;

            bool bolGenFileFlagError = false;

            try
            {
                BillingHandler billingHandler = new BillingHandler();

                doBatchProcessResult _doBatchProcessResult = new doBatchProcessResult();
                _doBatchProcessResult.BatchUser = UserId;

                List<doGetBankAutoTransferDateForGen> _doGetBankAutoTransferDateForGenList = billingHandler.GetBankAutoTransferDateForGenList();
                List<tbm_Bank> _dotbm_BankList = new List<tbm_Bank>();
                tbm_Bank _dotbm_Bank = new tbm_Bank();
                IMasterHandler masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;

                // CSV Enging
                WriteCSVFile egWriteCSV = new WriteCSVFile();
                doHeaderBatchGenerateAutoTransferBankFile _doHeader = new doHeaderBatchGenerateAutoTransferBankFile();
                List<doHeaderBatchGenerateAutoTransferBankFile> _doHeaderList = new List<doHeaderBatchGenerateAutoTransferBankFile>();
                List<tbm_SecomBankAccount> _dotbm_SecomBankAccount = new List<tbm_SecomBankAccount>();

                doBodyBatchGenerateAutoTransferBankFile _doDetail = new doBodyBatchGenerateAutoTransferBankFile();
                List<doBodyBatchGenerateAutoTransferBankFile> _doDetailList = new List<doBodyBatchGenerateAutoTransferBankFile>();
                List<doGetInvoiceForGenBankFile> _doGetInvoiceForGenBankFileList = new List<doGetInvoiceForGenBankFile>();

                doFooterBatchGenerateAutoTransferBankFile _doFooter = new doFooterBatchGenerateAutoTransferBankFile();
                List<doFooterBatchGenerateAutoTransferBankFile> _doFooterList = new List<doFooterBatchGenerateAutoTransferBankFile>();

                if (_doGetBankAutoTransferDateForGenList != null)
                {

                    iTotal = _doGetBankAutoTransferDateForGenList.Count;
                    iComplete = 0;
                    iFail = 0;
                    strErrorMsg = string.Empty;


                    // 1 bank 1 file
                    foreach (doGetBankAutoTransferDateForGen _doGetBankAutoTransferDateForGen in _doGetBankAutoTransferDateForGenList)
                    {
                        bolGenFileFlagError = false;
                        _doHeaderList.Clear();
                        _doDetailList.Clear();
                        _doFooterList.Clear();

                        // temp use this handle is duplicate both name and strod procedure master handle combo
                        _dotbm_BankList = masterHandler.GetTbm_Bank();
                        foreach (tbm_Bank _dotbm_Banktemp in _dotbm_BankList)
                        {
                            if (_dotbm_Banktemp.BankCode == _doGetBankAutoTransferDateForGen.BankCode)
                            {
                                _dotbm_Bank = _dotbm_Banktemp;
                            }
                        }

                        // Edit by Narupon W. 9 May 2012
                        //strFileName = _dotbm_Bank.BankNameAbbr + "-" + Convert.ToDateTime(System.DateTime.Now).ToString("ddMMyyyy").Replace(",", "") + ".CSV";

                        strFileName = string.Format("{0}-{1}-{2}.CSV", _dotbm_Bank.BankNameAbbr, DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.ToString("HHmmss"));
                        strPath = PathUtil.GetPathValue(PathUtil.PathName.AutoTransferFile, strFileName); //ReportUtil.GetGeneratedReportPath(strFileName);

                        egWriteCSV.IfExitDeleteFile(strPath);

                        iTotalTransaction = 0;
                        iTotalAmount = 0;
                        DateTime AutoTransferDate = DateTime.Now;
                        int iAutoTransferDate = Convert.ToInt32(_doGetBankAutoTransferDateForGen.AutoTransferDate);
                        if (iAutoTransferDate < DateTime.Now.Day)
                        {
                            AutoTransferDate = DateTime.Now.AddMonths(1).AddDays(-DateTime.Now.Day + iAutoTransferDate);
                        }
                        else
                        {
                            AutoTransferDate = DateTime.Now.AddDays(-DateTime.Now.Day + iAutoTransferDate);
                        }


                        //_doHeader
                        //_doGetInvoiceForGenBankFileList
                        _dotbm_SecomBankAccount = masterHandler.GetSecomBankAccountForAutoTransfer(_dotbm_Bank.BankCode);

                        if (_dotbm_SecomBankAccount != null)
                        {
                            if (_dotbm_SecomBankAccount.Count > 0)
                            {
                                _doHeader.Col1 = _dotbm_SecomBankAccount[0].AccountNo;

                                // Edit by Narupon W. 
                                //_doHeader.Col2 = _dotbm_Bank.BankNameEN;
                                _doHeader.Col2 = _dotbm_Bank.BankNameAbbr;



                                // Edit by Narupon W. 9 May 2012
                                //_doHeader.Col3 = _doGetBankAutoTransferDateForGen.AutoTransferDate;
                                _doHeader.Col3 = AutoTransferDate.ToString("ddMMyyyy");

                                _doHeaderList.Add(_doHeader);
                            }
                        }


                        egWriteCSV.ExportToCSVFile<doHeaderBatchGenerateAutoTransferBankFile>(strPath, _doHeaderList);


                        _doGetInvoiceForGenBankFileList = billingHandler.GetInvoiceForGenBankFile(
                                                                            _doGetBankAutoTransferDateForGen.BankCode
                                                                            , _doGetBankAutoTransferDateForGen.AutoTransferDate);

                        if (_doGetInvoiceForGenBankFileList != null)
                        {
                            foreach (doGetInvoiceForGenBankFile _doGetInvoiceForGenBankFile in _doGetInvoiceForGenBankFileList)
                            {
                                _doDetail = new doBodyBatchGenerateAutoTransferBankFile();
                                _doDetail.Row = _doGetInvoiceForGenBankFile.Row;
                                _doDetail.InvoiceNo = _doGetInvoiceForGenBankFile.InvoiceNo;
                                _doDetail.FullNameEN = _doGetInvoiceForGenBankFile.FullNameEN;
                                _doDetail.FullNameLC = _doGetInvoiceForGenBankFile.FullNameLC;
                                _doDetail.AccountNo = _doGetInvoiceForGenBankFile.AccountNo;

                                //_doDetail.InvoiceAmount = _doGetInvoiceForGenBankFile.InvoiceAmount;
                                _doDetail.InvoiceAmount = (_doGetInvoiceForGenBankFile.InvoiceAmount ?? 0) - _doGetInvoiceForGenBankFile.CreditAmountIncVAT; //Modify by Jutarat A. on 15112013

                                _doDetail.WHTAmount = _doGetInvoiceForGenBankFile.WHTAmount;
                                //_doDetail.AutoTransferDate = _doGetInvoiceForGenBankFile.AutoTransferDate;

                                _doDetailList.Add(_doDetail);

                                iTotalTransaction = iTotalTransaction + 1;

                                //iTotalAmount = iTotalAmount + _doGetInvoiceForGenBankFile.InvoiceAmount;
                                iTotalAmount = iTotalAmount + ((_doGetInvoiceForGenBankFile.InvoiceAmount ?? 0) - _doGetInvoiceForGenBankFile.CreditAmountIncVAT); //Modify by Jutarat A. on 15112013
                            }

                            egWriteCSV.ExportToCSVFile<doBodyBatchGenerateAutoTransferBankFile>(strPath, _doDetailList);
                        }


                        //_doFooter
                        _doFooter.Col1 = iTotalTransaction;
                        _doFooter.Col2 = iTotalAmount;
                        _doFooterList.Add(_doFooter);
                        egWriteCSV.ExportToCSVFile<doFooterBatchGenerateAutoTransferBankFile>(strPath, _doFooterList);

                        tbt_ExportAutoTransfer _dotbt_ExportAutoTransfer = new tbt_ExportAutoTransfer();
                        _dotbt_ExportAutoTransfer.ExportAutoTransferID = 0;
                        _dotbt_ExportAutoTransfer.BankCode = _doGetBankAutoTransferDateForGen.BankCode;
                        _dotbt_ExportAutoTransfer.BankBranchCode = _dotbm_SecomBankAccount[0].BankBranchCode;
                        _dotbt_ExportAutoTransfer.GenerateDate = _doGetBankAutoTransferDateForGen.GenerateDate;
                        _dotbt_ExportAutoTransfer.AutoTransferDate = AutoTransferDate;//_doGetBankAutoTransferDateForGen.GenerateDate;//_doGetBankAutoTransferDateForGen.AutoTransferDate;
                        //_dotbt_ExportAutoTransfer.FilePath = strPath;
                        _dotbt_ExportAutoTransfer.FileName = strFileName;

                        _dotbt_ExportAutoTransfer.CreateBy = ProcessID.C_PROCESS_ID_GEN_AUTO_FILE;
                        _dotbt_ExportAutoTransfer.CreateDate = BatchDate;
                        _dotbt_ExportAutoTransfer.UpdateBy = ProcessID.C_PROCESS_ID_GEN_AUTO_FILE;
                        _dotbt_ExportAutoTransfer.UpdateDate = BatchDate;

                        if (!(billingHandler.CreateTbt_ExportAutoTransferNoLog(_dotbt_ExportAutoTransfer) > 0))
                        {
                            // insert error
                            iFail = iFail + 1;
                            strErrorMsg = strErrorMsg + " insert log error ";
                            bolGenFileFlagError = true;
                        };

                        // loop again for update invoice status
                        if (_doGetInvoiceForGenBankFileList != null)
                        {
                            foreach (doGetInvoiceForGenBankFile _doGetInvoiceForGenBankFile in _doGetInvoiceForGenBankFileList)
                            {
                                _doDetail.InvoiceNo = _doGetInvoiceForGenBankFile.InvoiceNo;
                                if (!(billingHandler.UpdateInvoicePaymentStatusNolog(
                                    _doGetInvoiceForGenBankFile.InvoiceNo
                                    , PaymentStatus.C_PAYMENT_STATUS_GEN_AUTO_CREDIT
                                    , ProcessID.C_PROCESS_ID_GEN_AUTO_FILE
                                    , BatchDate
                                    , strFileName)))
                                {
                                    // error here
                                    iFail = iFail + 1;
                                    strErrorMsg = strErrorMsg + " Update Invoice Status Error - invoice no :" + _doGetInvoiceForGenBankFile.InvoiceNo;
                                    bolGenFileFlagError = true;
                                };
                            }
                        }

                        if (!(bolGenFileFlagError))
                        {
                            iComplete = iComplete + 1;
                        }
                        // foreach loop 1 bank 1 file
                    }
                }
                if (iFail > 0)
                {
                    _doBatchProcessResult.Result = FlagType.C_FLAG_OFF;
                }
                else
                {
                    _doBatchProcessResult.Result = FlagType.C_FLAG_ON;
                }
                _doBatchProcessResult.BatchStatus = null;
                _doBatchProcessResult.Total = iTotal;
                _doBatchProcessResult.Complete = iComplete;
                _doBatchProcessResult.Failed = iFail;
                _doBatchProcessResult.ErrorMessage = strErrorMsg;

                return _doBatchProcessResult;
            }
            catch (Exception Ex)
            {
                throw;
            }
        }


        #endregion
    }

    #region Class process

    #region BLP020_ManageBillingDetailMonthlyFee

    public class BLP020_ManageBillingDetailMonthlyFee : IBatchProcess
    {
        public doBatchProcessResult WorkProcess(string UserId, DateTime BatchDate)
        {
            BillingBatchProcessHandler batch = new BillingBatchProcessHandler();
            return batch.BLP020_ManageBillingDetailMonthlyFeeProcess(UserId, BatchDate);
        }
    }

    #endregion

    #region BLP030_ManageInvoice

    public class BLP030_ManageInvoice : IBatchProcess
    {
        public doBatchProcessResult WorkProcess(string UserId, DateTime BatchDate)
        {
            BillingBatchProcessHandler batch = new BillingBatchProcessHandler();
            return batch.BLP030_ManageInvoiceProcess(UserId, BatchDate);
        }
    }

    #endregion

    #region BLP040_BatchGenerateAutoTransferBankFile

    public class BLP040_BatchGenerateAutoTransferBankFile : IBatchProcess
    {
        public doBatchProcessResult WorkProcess(string UserId, DateTime BatchDate)
        {
            BillingBatchProcessHandler batch = new BillingBatchProcessHandler();
            return batch.BLP040_BatchGenerateAutoTransferBankFileProcess(UserId, BatchDate);
        }
    }

    #endregion

    #endregion
}
