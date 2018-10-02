//*********************************
// Create by: 
// Create date: /Jun/2010
// Update date: /Jun/2010
//*********************************

using System;
using System.Web;
using System.Web.Mvc;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.DataEntity.Contract;
using System.Data.Objects;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Presentation.Contract.Models;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Quotation;

namespace SECOM_AJIS.Presentation.Contract.Controllers
{
    public partial class ContractController : BaseController
    {
        #region Event
        
        /// <summary>
        /// Retrieve Quotation data when click [Retrieve] button in ‘Specify quotation’ section
        /// </summary>
        /// <param name="doRetrieveData"></param>
        /// <returns></returns>
        public ActionResult RetrieveClick_CTS062(CTS062_DORetrieveData doRetrieveData)
        {
            ObjectResultData res = new ObjectResultData();
           
            IInstrumentMasterHandler instrumentMasterHandler; 
            IQuotationHandler quotationHandler;
            ICommonContractHandler commonContractHandler;
            IBillingMasterHandler billingMasterHandler; 

            //dtTbt_Instrument dtTbtInstrument;
            dsQuotationData dsQuotation;
            doGetQuotationDataCondition doGetQuotationData;
            List<dtInstrument> listDTInstrument;
         
            CommonUtil comU = new CommonUtil();
            CTS062_ScreenParameter session;
            CTS062_RetrieveValid resObj = new CTS062_RetrieveValid();

            try
            {
                session = CTS062_GetImportData();
                resObj.CanRetrieve = true;
                comU = new CommonUtil();
                quotationHandler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
                instrumentMasterHandler = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                commonContractHandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
                billingMasterHandler = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;

                session.ListDTBillingClient = new List<dtBillingClientData>();

                //2.2 Load quotation
                doGetQuotationData = new doGetQuotationDataCondition();
                doGetQuotationData.QuotationTargetCode = comU.ConvertQuotationTargetCode(doRetrieveData.QuotationTargetCode,CommonUtil.CONVERT_TYPE.TO_LONG);
                doGetQuotationData.Alphabet = doRetrieveData.Alphabet;
                doGetQuotationData.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_SALE;
                doGetQuotationData.TargetCodeTypeCode = TargetCodeType.C_TARGET_CODE_TYPE_CONTRACT_CODE;
                doGetQuotationData.ContractFlag = true;
                dsQuotation = quotationHandler.GetQuotationData(doGetQuotationData);

                if (dsQuotation == null)
                {
                    //res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    //res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3002);
                    //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3002, "");
                    //return Json(res);    
                    resObj.CanRetrieve = false;
                    resObj.MessageCode = "MSG3002";
                }
                else
                {
                    session.DSQuotation = dsQuotation;
                    //2.3 Validate quotation for error //This validation is the same other screen
                    if (dsQuotation.dtTbt_QuotationBasic.LastOccNo != session.DSSaleContract.dtTbt_SaleBasic[0].OCC)
                    {
                        //res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        //res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3002);
                        //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3002, "");
                        //return Json(res);
                        resObj.CanRetrieve = false;
                        resObj.MessageCode = "MSG3002";
                    }

                    //2.4 Validate quotation for warning //This validation is the same other screen
                    foreach (var item in dsQuotation.dtTbt_QuotationInstrumentDetails)
                    {
                        listDTInstrument = instrumentMasterHandler.GetInstrument(dsQuotation.dtTbt_QuotationInstrumentDetails[0].InstrumentCode, null, null, null);
                        if (listDTInstrument[0].LineUpTypeCode == LineUpType.C_LINE_UP_TYPE_STOP_SALE || listDTInstrument[0].LineUpTypeCode == LineUpType.C_LINE_UP_TYPE_LOGICAL_DELETE)
                        {
                            //res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                            //res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3038);
                            //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3038, "");
                            //return Json(res);      
                            resObj.CanRetrieve = true;
                            resObj.MessageCode = "MSG3038";
                            break;
                        }
                    }

                    //--------------------------------------------------------

                    //2.6 Show product name (Do on GetProductName)

                    //2.7 Show sale type (Do on GetMiscellaneous)

                    //2.8 Show salesman name (Do on GetActiveEmployee)

                    //ทั้ง 2.6 2.7 2.8 จะโดนเรียกจาก Method GetChangePlanANDSpecifyDetail ตอนกด Retrieve

                    //---------------------------------------------------------

                    //2.10 Load billing temp of current OCC

                    session.ListBillingTemp = commonContractHandler.GetTbt_BillingTemp(session.DSSaleContract.dtTbt_SaleBasic[0].ContractCode, session.DOLastOCC.LastOCC);

                    //2.11 Initial billing client

                    foreach (var item in session.ListBillingTemp)
                    {
                        if (billingMasterHandler.GetBillingClient(item.BillingClientCode).Count() != 0)
                            session.ListDTBillingClient.Add(billingMasterHandler.GetBillingClient(item.BillingClientCode)[0]);
                    }
                }

                res.ResultData = resObj;

            }
            catch (Exception ex)
            {
                res.ResultData = false;
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Validate Business when click [Register] button in ‘Action button’ section
        /// </summary>
        /// <param name="doRegisterData"></param>
        /// <returns></returns>
        public ActionResult RegisterClick_CTS062(CTS062_DORegisterData doRegisterData) //12.5 Validate require fields (Do on model)
        {
            ObjectResultData res = new ObjectResultData();
            ObjectResultData resValidateBusiness = new ObjectResultData();

            ICommonHandler commonHandler;
            ISaleContractHandler saleContractHandler;
            dsSaleContractData dsSaleContract;
            List<tbt_BillingTemp> listDTBillingTemp;
            List<tbt_BillingTemp> listDTBillingTempCompleteInstallation;

            CTS062_ScreenParameter session;

            try
            {
                session = CTS062_GetImportData();
                commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                saleContractHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;

                //12.1 Check suspending
                if (commonHandler.IsSystemSuspending())
                {
                    //12.1.2 Not allow to continue operation if system issuspended
                    //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049, "");
                    res.ResultData = false;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);                
                }

                //12.6 Validate business
                resValidateBusiness = ValidateBusiness_CTS062(doRegisterData);
                if (resValidateBusiness.IsError)
                    return Json(resValidateBusiness);
                
                dsSaleContract = session.DSSaleContract;
                //12.2 Update contract data from quotation data
                saleContractHandler.MapFromQuotation(session.DSQuotation, ref dsSaleContract);

                //12.3 Map data from ‘Change plan’ section into dsSaleContract
                dsSaleContract.dtTbt_SaleBasic[0].ExpectedInstallCompleteDate = doRegisterData.ExpectedInstallCompleteDate;
                dsSaleContract.dtTbt_SaleBasic[0].ExpectedCustAcceptanceDate = doRegisterData.ExpectedCustAcceptanceDate;

                //dsSaleContract.dtTbt_SaleBasic[0].OrderProductPrice = decimal.Parse(doRegisterData.OrderProductPrice);
                //dsSaleContract.dtTbt_SaleBasic[0].OrderInstallFee = decimal.Parse(doRegisterData.OrderInstallFee);
                //dsSaleContract.dtTbt_SaleBasic[0].OrderSalePrice = decimal.Parse(doRegisterData.OrderSalePrice);

                //if (doRegisterData.BillingAmt_Acceptance != null && doRegisterData.BillingAmt_Acceptance != "")
                //    dsSaleContract.dtTbt_SaleBasic[0].BillingAmt_Acceptance = decimal.Parse(doRegisterData.BillingAmt_Acceptance);

                #region Normal Product Price

                dsSaleContract.dtTbt_SaleBasic[0].NormalProductPriceCurrencyType = session.DSQuotation.dtTbt_QuotationBasic.ProductPriceCurrencyType;
                dsSaleContract.dtTbt_SaleBasic[0].NormalProductPrice = session.DSQuotation.dtTbt_QuotationBasic.ProductPrice;
                dsSaleContract.dtTbt_SaleBasic[0].NormalProductPriceUsd = session.DSQuotation.dtTbt_QuotationBasic.ProductPriceUsd;

                #endregion
                #region Order Product Price

                dsSaleContract.dtTbt_SaleBasic[0].OrderProductPriceCurrencyType = doRegisterData.OrderProductPriceCurrencyType;
                if (dsSaleContract.dtTbt_SaleBasic[0].OrderProductPriceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    dsSaleContract.dtTbt_SaleBasic[0].OrderProductPrice = null;
                    dsSaleContract.dtTbt_SaleBasic[0].OrderProductPriceUsd = decimal.Parse(doRegisterData.OrderProductPrice);
                }
                else
                {
                    dsSaleContract.dtTbt_SaleBasic[0].OrderProductPrice = decimal.Parse(doRegisterData.OrderProductPrice);
                    dsSaleContract.dtTbt_SaleBasic[0].OrderProductPriceUsd = null;
                }

                #endregion
                #region Billing Amount for Approve Contract

                dsSaleContract.dtTbt_SaleBasic[0].BillingAmt_ApproveContractCurrencyType = doRegisterData.BillingAmt_ApproveContractCurrencyType;
                if (dsSaleContract.dtTbt_SaleBasic[0].BillingAmt_ApproveContractCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    dsSaleContract.dtTbt_SaleBasic[0].BillingAmt_ApproveContract = null;
                    dsSaleContract.dtTbt_SaleBasic[0].BillingAmt_ApproveContractUsd = decimal.Parse(doRegisterData.BillingAmt_ApproveContract);
                }
                else
                {
                    dsSaleContract.dtTbt_SaleBasic[0].BillingAmt_ApproveContract = decimal.Parse(doRegisterData.BillingAmt_ApproveContract);
                    dsSaleContract.dtTbt_SaleBasic[0].BillingAmt_ApproveContractUsd = null;
                }

                #endregion
                #region Billing Amount for Partial Fee

                dsSaleContract.dtTbt_SaleBasic[0].BillingAmt_PartialFeeCurrencyType = doRegisterData.BillingAmt_PartialFeeCurrencyType;
                if (dsSaleContract.dtTbt_SaleBasic[0].BillingAmt_PartialFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    dsSaleContract.dtTbt_SaleBasic[0].BillingAmt_PartialFee = null;
                    dsSaleContract.dtTbt_SaleBasic[0].BillingAmt_PartialFeeUsd = decimal.Parse(doRegisterData.BillingAmt_PartialFee);
                }
                else
                {
                    dsSaleContract.dtTbt_SaleBasic[0].BillingAmt_PartialFee = decimal.Parse(doRegisterData.BillingAmt_PartialFee);
                    dsSaleContract.dtTbt_SaleBasic[0].BillingAmt_PartialFeeUsd = null;
                }

                #endregion
                #region Billing Amount for Customer Acceptance

                dsSaleContract.dtTbt_SaleBasic[0].BillingAmt_AcceptanceCurrencyType = doRegisterData.BillingAmt_AcceptanceCurrencyType;
                if (dsSaleContract.dtTbt_SaleBasic[0].BillingAmt_AcceptanceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    dsSaleContract.dtTbt_SaleBasic[0].BillingAmt_Acceptance = null;
                    dsSaleContract.dtTbt_SaleBasic[0].BillingAmt_AcceptanceUsd = decimal.Parse(doRegisterData.BillingAmt_Acceptance);
                }
                else
                {
                    dsSaleContract.dtTbt_SaleBasic[0].BillingAmt_Acceptance = decimal.Parse(doRegisterData.BillingAmt_Acceptance);
                    dsSaleContract.dtTbt_SaleBasic[0].BillingAmt_AcceptanceUsd = null;
                }

                #endregion

                #region Normal Installation Fee

                dsSaleContract.dtTbt_SaleBasic[0].NormalInstallFeeCurrencyType = session.DSQuotation.dtTbt_QuotationBasic.InstallationFeeCurrencyType;
                dsSaleContract.dtTbt_SaleBasic[0].NormalInstallFee = session.DSQuotation.dtTbt_QuotationBasic.InstallationFee;
                dsSaleContract.dtTbt_SaleBasic[0].NormalInstallFeeUsd = session.DSQuotation.dtTbt_QuotationBasic.InstallationFeeUsd;

                #endregion
                #region Order Installation Fee

                dsSaleContract.dtTbt_SaleBasic[0].OrderInstallFeeCurrencyType = doRegisterData.OrderInstallFeeCurrencyType;
                if (dsSaleContract.dtTbt_SaleBasic[0].OrderInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    dsSaleContract.dtTbt_SaleBasic[0].OrderInstallFee = null;
                    dsSaleContract.dtTbt_SaleBasic[0].OrderInstallFeeUsd = decimal.Parse(doRegisterData.OrderInstallFee);
                }
                else
                {
                    dsSaleContract.dtTbt_SaleBasic[0].OrderInstallFee = decimal.Parse(doRegisterData.OrderInstallFee);
                    dsSaleContract.dtTbt_SaleBasic[0].OrderInstallFeeUsd = null;
                }

                #endregion
                #region Billing Amount Installation for Approve Contract

                dsSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_ApproveContractCurrencyType = doRegisterData.BillingAmtInstallation_ApproveContractCurrencyType;
                if (dsSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_ApproveContractCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    dsSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_ApproveContract = null;
                    dsSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_ApproveContractUsd = decimal.Parse(doRegisterData.BillingAmtInstallation_ApproveContract);
                }
                else
                {
                    dsSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_ApproveContract = decimal.Parse(doRegisterData.BillingAmtInstallation_ApproveContract);
                    dsSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_ApproveContractUsd = null;
                }

                #endregion
                #region Billing Amount Installation for Partial Fee

                dsSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_PartialFeeCurrencyType = doRegisterData.BillingAmtInstallation_PartialFeeCurrencyType;
                if (dsSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_PartialFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    dsSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_PartialFee = null;
                    dsSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_PartialFeeUsd = decimal.Parse(doRegisterData.BillingAmtInstallation_PartialFee);
                }
                else
                {
                    dsSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_PartialFee = decimal.Parse(doRegisterData.BillingAmtInstallation_PartialFee);
                    dsSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_PartialFeeUsd = null;
                }

                #endregion
                #region Billing Amount Installation for Customer Acceptance

                dsSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_AcceptanceCurrencyType = doRegisterData.BillingAmtInstallation_AcceptanceCurrencyType;
                if (dsSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_AcceptanceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    dsSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_Acceptance = null;
                    dsSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_AcceptanceUsd = decimal.Parse(doRegisterData.BillingAmtInstallation_Acceptance);
                }
                else
                {
                    dsSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_Acceptance = decimal.Parse(doRegisterData.BillingAmtInstallation_Acceptance);
                    dsSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_AcceptanceUsd = null;
                }

                #endregion

                dsSaleContract.dtTbt_SaleBasic[0].ConnectionFlag = doRegisterData.ConnectionFlag;
                dsSaleContract.dtTbt_SaleBasic[0].ConnectTargetCode = doRegisterData.ConnectTargetCode;
                dsSaleContract.dtTbt_SaleBasic[0].DistributedInstallTypeCode = doRegisterData.DistributedType;
                dsSaleContract.dtTbt_SaleBasic[0].DistributedOriginCode = doRegisterData.DistributedCode;
                dsSaleContract.dtTbt_SaleBasic[0].NegotiationStaffEmpNo1 = doRegisterData.NegotiationStaffEmpNo1;
                dsSaleContract.dtTbt_SaleBasic[0].NegotiationStaffEmpNo2 = doRegisterData.NegotiationStaffEmpNo2;

                dsSaleContract.dtTbt_SaleBasic[0].ApproveNo1 = doRegisterData.ApproveNo1;
                dsSaleContract.dtTbt_SaleBasic[0].ApproveNo2 = doRegisterData.ApproveNo2;
                dsSaleContract.dtTbt_SaleBasic[0].ApproveNo3 = doRegisterData.ApproveNo3;
                dsSaleContract.dtTbt_SaleBasic[0].ApproveNo4 = doRegisterData.ApproveNo4;
                dsSaleContract.dtTbt_SaleBasic[0].ApproveNo5 = doRegisterData.ApproveNo5;


                //   if (session.ListBillingTemp.FindAll(delegate(tbt_BillingTemp s) {
                //       return s.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_SALE_PRICE 
                //           && s.BillingTiming == BillingTiming.C_BILLING_TIMING_ACCEPTANCE; }).Count() != 0)
                //{
                // listDTBillingTempCompleteInstallation = session.ListBillingTemp.FindAll(delegate(tbt_BillingTemp s) {
                //           return s.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_SALE_PRICE 
                //           && s.BillingTiming == BillingTiming.C_BILLING_TIMING_ACCEPTANCE;
                //       });

                //       foreach (var item in listDTBillingTempCompleteInstallation)
                //       {
                //      item.BillingAmt = decimal.Parse(doRegisterData.BillingAmt_CompleteInstallation);
                //            item.PayMethod = doRegisterData.PaymethodCompleteInstallation;
                //       }
                //}

                List<tbt_BillingTemp> btmp = new List<tbt_BillingTemp>();

                #region Product Price for Approve

                btmp.Add(new tbt_BillingTemp()
                {
                    //BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_SALE_PRICE,
                    BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_PRODUCT_PRICE,
                    BillingTiming = BillingTiming.C_BILLING_TIMING_APPROVE_CONTRACT
                });

                #endregion
                #region Product Price for Partial Fee

                btmp.Add(new tbt_BillingTemp()
                {
                    //BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_SALE_PRICE,
                    BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_PRODUCT_PRICE,
                    BillingTiming = BillingTiming.C_BILLING_TIMING_PARTIAL
                });

                #endregion
                #region Product Price for Customer Acceptance

                btmp.Add(new tbt_BillingTemp()
                {
                    //BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_SALE_PRICE,
                    BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_PRODUCT_PRICE,
                    BillingTiming = BillingTiming.C_BILLING_TIMING_ACCEPTANCE
                });

                #endregion

                #region Installation Fee for Approve

                btmp.Add(new tbt_BillingTemp()
                {
                    BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE,
                    BillingTiming = BillingTiming.C_BILLING_TIMING_APPROVE_CONTRACT
                });

                #endregion
                #region Installation Fee for Partial Fee

                btmp.Add(new tbt_BillingTemp()
                {
                    BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE,
                    BillingTiming = BillingTiming.C_BILLING_TIMING_PARTIAL
                });

                #endregion
                #region Installation Fee for Customer Acceptance

                btmp.Add(new tbt_BillingTemp()
                {
                    BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE,
                    BillingTiming = BillingTiming.C_BILLING_TIMING_ACCEPTANCE
                });

                #endregion

                foreach (tbt_BillingTemp tmp in btmp)
                {
                    List<tbt_BillingTemp> billing = session.ListBillingTemp.FindAll(delegate (tbt_BillingTemp s)
                    {
                        return s.BillingType == tmp.BillingType
                                && s.BillingTiming == tmp.BillingTiming;
                    });
                    if (billing.Count() > 0)
                    {
                        string currencyType = null;
                        decimal? amt = null;
                        string method = null;
                        //if (tmp.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_SALE_PRICE)
                        if (tmp.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_PRODUCT_PRICE)
                        {
                            if (tmp.BillingTiming == BillingTiming.C_BILLING_TIMING_APPROVE_CONTRACT)
                            {
                                currencyType = doRegisterData.SalePrice_ApprovalCurrencyType;
                                amt = decimal.Parse(doRegisterData.SalePrice_Approval);
                            }
                            else if (tmp.BillingTiming == BillingTiming.C_BILLING_TIMING_PARTIAL)
                            {
                                currencyType = doRegisterData.SalePrice_PartialCurrencyType;
                                amt = decimal.Parse(doRegisterData.SalePrice_Partial);
                            }
                            else if (tmp.BillingTiming == BillingTiming.C_BILLING_TIMING_ACCEPTANCE)
                            {
                                currencyType = doRegisterData.SalePrice_AcceptanceCurrencyType;
                                amt = decimal.Parse(doRegisterData.SalePrice_Acceptance);
                                method = doRegisterData.SalePrice_PaymentMethod_Acceptance;
                            }
                        }
                        else
                        {
                            if (tmp.BillingTiming == BillingTiming.C_BILLING_TIMING_APPROVE_CONTRACT)
                            {
                                currencyType = doRegisterData.InstallationFee_ApprovalCurrencyType;
                                amt = decimal.Parse(doRegisterData.InstallationFee_Approval);
                            }
                            else if (tmp.BillingTiming == BillingTiming.C_BILLING_TIMING_PARTIAL)
                            {
                                currencyType = doRegisterData.InstallationFee_PartialCurrencyType;
                                amt = decimal.Parse(doRegisterData.InstallationFee_Partial);
                            }
                            else if (tmp.BillingTiming == BillingTiming.C_BILLING_TIMING_ACCEPTANCE)
                            {
                                currencyType = doRegisterData.InstallationFee_AcceptanceCurrencyType;
                                amt = decimal.Parse(doRegisterData.InstallationFee_Acceptance);
                                method = doRegisterData.InstallationFee_PaymentMethod_Acceptance;
                            }
                        }

                        foreach (tbt_BillingTemp item in billing)
                        {
                            item.BillingAmtCurrencyType = currencyType;
                            if (item.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            {
                                item.BillingAmt = null;
                                item.BillingAmtUsd = amt;
                            }
                            else
                            {
                                item.BillingAmt = amt;
                                item.BillingAmtUsd = null;
                            }
                            if (tmp.BillingTiming == BillingTiming.C_BILLING_TIMING_ACCEPTANCE)
                                item.PayMethod = method;
                        }
                    }
                }
                
                listDTBillingTemp = session.ListBillingTemp;

                //12.7 Change screen mode to ‘Confirm mode’ (Do on javascript)
            }
            catch (Exception ex)
            {
                res.ResultData = false;
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Update data to database when click [Confirm] button in ‘Action button’ section
        /// </summary>
        /// <param name="doRegisterData"></param>
        /// <returns></returns>
        public ActionResult ConfirmClick_CTS062(CTS062_DORegisterData doRegisterData)
        {
            decimal? normalSalePrice;
            decimal? orderSalePrice;

            ObjectResultData res = new ObjectResultData();
            ObjectResultData resValidateBusiness = new ObjectResultData();
            CommonUtil util = new CommonUtil();

            ICommonHandler commonHandler;
            ISaleContractHandler saleContractHandler;
            CTS062_ScreenParameter session;

            try
            {
                session = CTS062_GetImportData();
                commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                saleContractHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;

                //13.1 Check suspending
                if (commonHandler.IsSystemSuspending())
                {
                    //13.1.2 Not allow to continue operation if system is suspended
                    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3049, "");
                    return Json(res);
                }

                //13.2 Validate business
                resValidateBusiness = ValidateBusiness_CTS062(doRegisterData);
                if (resValidateBusiness.IsError)
                    return Json(resValidateBusiness);

                //13.3 Prepare data

                // Mapping update data to session
                session.DSSaleContract.dtTbt_SaleBasic[0].ExpectedInstallCompleteDate = doRegisterData.ExpectedInstallCompleteDate;
                session.DSSaleContract.dtTbt_SaleBasic[0].ExpectedCustAcceptanceDate = doRegisterData.ExpectedCustAcceptanceDate;

                //session.DSSaleContract.dtTbt_SaleBasic[0].NormalProductPrice = session.DSQuotation.dtTbt_QuotationBasic.ProductPrice;
                //session.DSSaleContract.dtTbt_SaleBasic[0].OrderProductPrice = decimal.Parse(doRegisterData.OrderProductPrice);
                //session.DSSaleContract.dtTbt_SaleBasic[0].NormalInstallFee = session.DSQuotation.dtTbt_QuotationBasic.InstallationFee;
                //session.DSSaleContract.dtTbt_SaleBasic[0].OrderInstallFee = decimal.Parse(doRegisterData.OrderInstallFee);
                //session.DSSaleContract.dtTbt_SaleBasic[0].NormalSalePrice = session.DSQuotation.dtTbt_QuotationBasic.ProductPrice + session.DSQuotation.dtTbt_QuotationBasic.InstallationFee;
                //session.DSSaleContract.dtTbt_SaleBasic[0].OrderSalePrice = decimal.Parse(doRegisterData.OrderSalePrice);
                //session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmt_Acceptance = decimal.Parse(doRegisterData.BillingAmt_Acceptance);

                #region Normal Product Price

                session.DSSaleContract.dtTbt_SaleBasic[0].NormalProductPriceCurrencyType = session.DSQuotation.dtTbt_QuotationBasic.ProductPriceCurrencyType;
                session.DSSaleContract.dtTbt_SaleBasic[0].NormalProductPrice = session.DSQuotation.dtTbt_QuotationBasic.ProductPrice;
                session.DSSaleContract.dtTbt_SaleBasic[0].NormalProductPriceUsd = session.DSQuotation.dtTbt_QuotationBasic.ProductPriceUsd;

                #endregion
                #region Order Product Price

                session.DSSaleContract.dtTbt_SaleBasic[0].OrderProductPriceCurrencyType = doRegisterData.OrderProductPriceCurrencyType;
                if (session.DSSaleContract.dtTbt_SaleBasic[0].OrderProductPriceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    session.DSSaleContract.dtTbt_SaleBasic[0].OrderProductPrice = null;
                    session.DSSaleContract.dtTbt_SaleBasic[0].OrderProductPriceUsd = decimal.Parse(doRegisterData.OrderProductPrice);
                }
                else
                {
                    session.DSSaleContract.dtTbt_SaleBasic[0].OrderProductPrice = decimal.Parse(doRegisterData.OrderProductPrice);
                    session.DSSaleContract.dtTbt_SaleBasic[0].OrderProductPriceUsd = null;
                }

                #endregion
                #region Billing Amount for Approve Contract

                session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmt_ApproveContractCurrencyType = doRegisterData.BillingAmt_ApproveContractCurrencyType;
                if (session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmt_ApproveContractCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmt_ApproveContract = null;
                    session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmt_ApproveContractUsd = decimal.Parse(doRegisterData.BillingAmt_ApproveContract);
                }
                else
                {
                    session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmt_ApproveContract = decimal.Parse(doRegisterData.BillingAmt_ApproveContract);
                    session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmt_ApproveContractUsd = null;
                }

                #endregion
                #region Billing Amount for Partial Fee

                session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmt_PartialFeeCurrencyType = doRegisterData.BillingAmt_PartialFeeCurrencyType;
                if (session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmt_PartialFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmt_PartialFee = null;
                    session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmt_PartialFeeUsd = decimal.Parse(doRegisterData.BillingAmt_PartialFee);
                }
                else
                {
                    session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmt_PartialFee = decimal.Parse(doRegisterData.BillingAmt_PartialFee);
                    session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmt_PartialFeeUsd = null;
                }

                #endregion
                #region Billing Amount for Customer Acceptance

                session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmt_AcceptanceCurrencyType = doRegisterData.BillingAmt_AcceptanceCurrencyType;
                if (session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmt_AcceptanceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmt_Acceptance = null;
                    session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmt_AcceptanceUsd = decimal.Parse(doRegisterData.BillingAmt_Acceptance);
                }
                else
                {
                    session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmt_Acceptance = decimal.Parse(doRegisterData.BillingAmt_Acceptance);
                    session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmt_AcceptanceUsd = null;
                }

                #endregion

                #region Normal Installation Fee

                session.DSSaleContract.dtTbt_SaleBasic[0].NormalInstallFeeCurrencyType = session.DSQuotation.dtTbt_QuotationBasic.InstallationFeeCurrencyType;
                session.DSSaleContract.dtTbt_SaleBasic[0].NormalInstallFee = session.DSQuotation.dtTbt_QuotationBasic.InstallationFee;
                session.DSSaleContract.dtTbt_SaleBasic[0].NormalInstallFeeUsd = session.DSQuotation.dtTbt_QuotationBasic.InstallationFeeUsd;

                #endregion
                #region Order Installation Fee

                session.DSSaleContract.dtTbt_SaleBasic[0].OrderInstallFeeCurrencyType = doRegisterData.OrderInstallFeeCurrencyType;
                if (session.DSSaleContract.dtTbt_SaleBasic[0].OrderInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    session.DSSaleContract.dtTbt_SaleBasic[0].OrderInstallFee = null;
                    session.DSSaleContract.dtTbt_SaleBasic[0].OrderInstallFeeUsd = decimal.Parse(doRegisterData.OrderInstallFee);
                }
                else
                {
                    session.DSSaleContract.dtTbt_SaleBasic[0].OrderInstallFee = decimal.Parse(doRegisterData.OrderInstallFee);
                    session.DSSaleContract.dtTbt_SaleBasic[0].OrderInstallFeeUsd = null;
                }

                #endregion
                #region Billing Amount Installation for Approve Contract

                session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_ApproveContractCurrencyType = doRegisterData.BillingAmtInstallation_ApproveContractCurrencyType;
                if (session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_ApproveContractCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_ApproveContract = null;
                    session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_ApproveContractUsd = decimal.Parse(doRegisterData.BillingAmtInstallation_ApproveContract);
                }
                else
                {
                    session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_ApproveContract = decimal.Parse(doRegisterData.BillingAmtInstallation_ApproveContract);
                    session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_ApproveContractUsd = null;
                }

                #endregion
                #region Billing Amount Installation for Partial Fee

                session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_PartialFeeCurrencyType = doRegisterData.BillingAmtInstallation_PartialFeeCurrencyType;
                if (session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_PartialFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_PartialFee = null;
                    session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_PartialFeeUsd = decimal.Parse(doRegisterData.BillingAmtInstallation_PartialFee);
                }
                else
                {
                    session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_PartialFee = decimal.Parse(doRegisterData.BillingAmtInstallation_PartialFee);
                    session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_PartialFeeUsd = null;
                }

                #endregion
                #region Billing Amount Installation for Customer Acceptance

                session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_AcceptanceCurrencyType = doRegisterData.BillingAmtInstallation_AcceptanceCurrencyType;
                if (session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_AcceptanceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_Acceptance = null;
                    session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_AcceptanceUsd = decimal.Parse(doRegisterData.BillingAmtInstallation_Acceptance);
                }
                else
                {
                    session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_Acceptance = decimal.Parse(doRegisterData.BillingAmtInstallation_Acceptance);
                    session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_AcceptanceUsd = null;
                }

                #endregion

                session.DSSaleContract.dtTbt_SaleBasic[0].ConnectionFlag = doRegisterData.ConnectionFlag;
                session.DSSaleContract.dtTbt_SaleBasic[0].ConnectTargetCode = util.ConvertContractCode(doRegisterData.ConnectTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                session.DSSaleContract.dtTbt_SaleBasic[0].DistributedInstallTypeCode = doRegisterData.DistributedType;
                //session.DSSaleContract.dtTbt_SaleBasic[0].SalesmanEmpNo1 = session.DSQuotation.dtTbt_QuotationBasic.SalesmanEmpCodeNameNo1;
                //session.DSSaleContract.dtTbt_SaleBasic[0].SalesmanEmpNo2 = session.DSQuotation.dtTbt_QuotationBasic.SalesmanEmpCodeNameNo2;
                //session.DSSaleContract.dtTbt_SaleBasic[0].SalesmanEmpNo3 = session.DSQuotation.dtTbt_QuotationBasic.SalesmanEmpCodeNameNo3;
                //session.DSSaleContract.dtTbt_SaleBasic[0].SalesmanEmpNo4 = session.DSQuotation.dtTbt_QuotationBasic.SalesmanEmpCodeNameNo4;
                //session.DSSaleContract.dtTbt_SaleBasic[0].SalesmanEmpNo5 = session.DSQuotation.dtTbt_QuotationBasic.SalesmanEmpCodeNameNo5;
                //session.DSSaleContract.dtTbt_SaleBasic[0].SalesmanEmpNo6 = session.DSQuotation.dtTbt_QuotationBasic.SalesmanEmpCodeNameNo6;
                //session.DSSaleContract.dtTbt_SaleBasic[0].SalesmanEmpNo7 = session.DSQuotation.dtTbt_QuotationBasic.SalesmanEmpCodeNameNo7;
                //session.DSSaleContract.dtTbt_SaleBasic[0].SalesmanEmpNo8 = session.DSQuotation.dtTbt_QuotationBasic.SalesmanEmpCodeNameNo8;
                //session.DSSaleContract.dtTbt_SaleBasic[0].SalesmanEmpNo9 = session.DSQuotation.dtTbt_QuotationBasic.SalesmanEmpCodeNameNo9;
                //session.DSSaleContract.dtTbt_SaleBasic[0].SalesmanEmpNo10 = session.DSQuotation.dtTbt_QuotationBasic.SalesmanEmpCodeNameNo10;

                session.DSSaleContract.dtTbt_SaleBasic[0].NegotiationStaffEmpNo1 = doRegisterData.NegotiationStaffEmpNo1;
                session.DSSaleContract.dtTbt_SaleBasic[0].NegotiationStaffEmpNo2 = doRegisterData.NegotiationStaffEmpNo2;

                session.DSSaleContract.dtTbt_SaleBasic[0].ApproveNo1 = doRegisterData.ApproveNo1;
                session.DSSaleContract.dtTbt_SaleBasic[0].ApproveNo2 = doRegisterData.ApproveNo2;
                session.DSSaleContract.dtTbt_SaleBasic[0].ApproveNo3 = doRegisterData.ApproveNo3;
                session.DSSaleContract.dtTbt_SaleBasic[0].ApproveNo4 = doRegisterData.ApproveNo4;
                session.DSSaleContract.dtTbt_SaleBasic[0].ApproveNo5 = doRegisterData.ApproveNo5;

                if (session.DSQuotation.dtTbt_QuotationMaintenanceLinkage != null)
                {
                    if (session.DSQuotation.dtTbt_QuotationMaintenanceLinkage.Count() != 0)
                        session.DSSaleContract.dtTbt_SaleBasic[0].MaintenanceContractFlag = true;
                    else
                        session.DSSaleContract.dtTbt_SaleBasic[0].MaintenanceContractFlag = false;
                }

                doRegisterData.DistributedCode = util.ConvertContractCode(doRegisterData.DistributedCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                normalSalePrice = session.DSSaleContract.dtTbt_SaleBasic[0].NormalSalePrice;
                orderSalePrice = session.DSSaleContract.dtTbt_SaleBasic[0].OrderSalePrice;
                
                if (normalSalePrice == null)
	                normalSalePrice = 0;
                
                if (orderSalePrice == null)
	                orderSalePrice = 0;

                session.DSSaleContract.dtTbt_SaleBasic[0].SaleAdjAmt = normalSalePrice - orderSalePrice;
                session.DSSaleContract.dtTbt_SaleBasic[0].ChangeImplementDate = null;

                //13.4 Perform save operation and 13.5 If there is no error Then Show success message by these criteria Message Code MSG3049 
                if (saleContractHandler.RegisterChangePlan(session.DSQuotation, session.DSSaleContract, session.ListBillingTemp))
                {
                    var contractinfo = saleContractHandler.GetTbt_SaleBasic(session.ScreenParameter.contractCode, null, true).FirstOrDefault();
                    if (contractinfo != null && contractinfo.ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START)
                    {
                        var viewinfo = saleContractHandler.GetTbt_SaleBasicForView(contractinfo.ContractCode, null, true).FirstOrDefault();
                        viewinfo = CommonUtil.ConvertObjectbyLanguage<dtTbt_SaleBasicForView, dtTbt_SaleBasicForView>(
                            viewinfo,
                            "PurCust_CustFullName",
                            "RealCust_CustName",
                            "RealCust_CustFullName",
                            "site_SiteName",
                            "Op_OfficeName"
                        );
                        CommonUtil.MappingObjectLanguage(viewinfo);
                        doChangePlanBeforeStartEmail templateObj = new doChangePlanBeforeStartEmail();
                        templateObj.ContractCode = viewinfo.ContractCode;
                        templateObj.CustCode = viewinfo.PurchaserCustCode;
                        templateObj.CustName = viewinfo.PurCust_CustFullName;
                        templateObj.SiteCode = viewinfo.SiteCode;
                        templateObj.SiteName = viewinfo.site_SiteName;
                        templateObj.OperationOfficeCode = viewinfo.OperationOfficeCode;
                        templateObj.OperationOfficeName = viewinfo.Op_OfficeName;
                        templateObj.CurrentDate = DateTime.Today;
                        if (CommonUtil.dsTransData != null && CommonUtil.dsTransData.dtUserData != null)
                        {
                            templateObj.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        }
                        var contracthandler = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
                        contracthandler.SendEmailChangePlanBeforeStart(templateObj);
                    }

                    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3049, "");
                }
            }
            catch (Exception ex)
            {
                res.ResultData = false;
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }        

        #endregion

        #region Method

        /// <summary>
        /// Initial screen
        /// </summary>
        /// <returns></returns>
        [Initialize("CTS062")]
        public ActionResult CTS062()
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                CTS062_ScreenParameter session = GetScreenObject<CTS062_ScreenParameter>();
                ViewBag.ContractCode = session.ScreenParameter.contractCode;
                ViewBag.ImportantFlag = false;
                InitialScreen_CTS062(ViewBag.ContractCode);

                return View("CTS062");
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Check system suspending, user’s permission and user’s authority of screen
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CTS062_Authority(CTS062_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                CommonUtil util = new CommonUtil();
                ICommonHandler commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                ISaleContractHandler saleHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;

                //1.1 Check suspending
                if (commonHandler.IsSystemSuspending())
                {
                    res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0049, new string[] { String.Format("Contract Code: {0}", param.ContractCode) }, null);
                    return Json(res);
                }

                //1.2 Check user's permission
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_CQ12_CHANGE_PLAN, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0053, null, null);
                    return Json(res);
                }

                // Check parameter


                if ((param == null)
                    || (String.IsNullOrEmpty(param.ContractCode)))
                {
                    //if (String.IsNullOrEmpty(CommonUtil.dsTransData.dtCommonSearch.ContractCode))
                    //{
                    //    // Not valid
                    //    //res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0011, new string[] { "Contract Code" }, null);
                    //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0147, null, null);
                    //    return Json(res);
                    //}
                    //else
                    //{
                    //    param.ContractCode = CommonUtil.dsTransData.dtCommonSearch.ContractCode;
                    //}
                    if (param.CommonSearch != null)
                    {
                        if (CommonUtil.IsNullOrEmpty(param.CommonSearch.ContractCode) == false)
                            param.ContractCode = param.CommonSearch.ContractCode;
                    }
                    if (String.IsNullOrEmpty(param.ContractCode))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0147, null, null);
                        return Json(res);
                    }
                }

                // Check is contact exists
                var contractObj = saleHandler.GetTbt_SaleBasic(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), null, true);
                if ((contractObj == null)
                    || (contractObj.Count == 0))
                {
                    // Not found
                    //res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0011, new string[] { String.Format("Contract Code: {0}", param.ContractCode) }, null);
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0124, null, null);
                    return Json(res);
                }

                string lastOCC = saleHandler.GetLastOCC(contractObj[0].ContractCode);
                var dsSaleContract = saleHandler.GetEntireContract(contractObj[0].ContractCode, lastOCC);

                if ((dsSaleContract != null)
                    && (dsSaleContract.dtTbt_SaleBasic != null)
                    && (dsSaleContract.dtTbt_SaleBasic.Count != 0))
                {
                    /*
                    if (CommonUtil.dsTransData.dtOfficeData.FindAll(delegate(OfficeDataDo s) { return s.OfficeCode == dsSaleContract.dtTbt_SaleBasic[0].ContractOfficeCode; }).Count == 0)
                    {
                        res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0063, null, null);
                        return Json(res);
                    }

                    if (CommonUtil.dsTransData.dtOfficeData.FindAll(delegate(OfficeDataDo s) { return s.OfficeCode == dsSaleContract.dtTbt_SaleBasic[0].OperationOfficeCode; }).Count == 0)
                    {
                        res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0063, null, null);
                        return Json(res);
                    }
                     * */

                    var existsContarctOffice = CommonUtil.dsTransData.dtOfficeData.Where(x => x.OfficeCode == dsSaleContract.dtTbt_SaleBasic[0].ContractOfficeCode);
                    var existsOperateOffice = CommonUtil.dsTransData.dtOfficeData.Where(x => x.OfficeCode == dsSaleContract.dtTbt_SaleBasic[0].OperationOfficeCode);

                    if ((existsContarctOffice.Count() <= 0) && (existsOperateOffice.Count() <= 0))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063, null, null);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return Json(res);
                    }

                    if (dsSaleContract.dtTbt_SaleBasic[0].InstallationCompleteFlag == FlagType.C_FLAG_ON)
                    {
                        res.AddErrorMessage("Contract", MessageUtil.MessageList.MSG3058, null, null);
                        return Json(res);
                    }

                    //if (dsSaleContract.dtTbt_SaleBasic[0].InstallationCompleteFlag == FlagType.C_FLAG_ON)
                    //{
                    //    res.AddErrorMessage("Contract", MessageUtil.MessageList.MSG3058, null, null);
                    //    return Json(res);
                    //}

                    if (((dsSaleContract.dtTbt_SaleBasic[0].ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_NEW_SALE)
                        || (dsSaleContract.dtTbt_SaleBasic[0].ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_ADD_SALE))
                        && ((dsSaleContract.dtTbt_SaleBasic[0].SaleProcessManageStatus == SaleProcessManageStatus.C_SALE_PROCESS_STATUS_ACCEPT_NOTCOMPLETE)
                        || (dsSaleContract.dtTbt_SaleBasic[0].SaleProcessManageStatus == SaleProcessManageStatus.C_SALE_PROCESS_STATUS_CANCEL)
                        || (dsSaleContract.dtTbt_SaleBasic[0].SaleProcessManageStatus == SaleProcessManageStatus.C_SALE_PROCESS_STATUS_COMPLETE_ACCEPT)
                        || (dsSaleContract.dtTbt_SaleBasic[0].SaleProcessManageStatus == SaleProcessManageStatus.C_SALE_PROCESS_STATUS_COMPLETE_NOTACCEPT)))
                    {
                        res.AddErrorMessage("Contract", MessageUtil.MessageList.MSG3058, null, null);
                        return Json(res);
                    }
                }
                else
                {
                    res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0011, new string[] { String.Format("Contract Code: {0}", param.ContractCode) }, null);
                    return Json(res);
                }

                InitialScreenSession_CTS062(param);
                //session.ContractCode = param.ContractCode;
                param.ScreenParameter.contractCode = util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                param.ContractCode = param.ScreenParameter.contractCode;

                return InitialScreenEnvironment<CTS061_ScreenParameter>("CTS062", param, res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        //public ActionResult CTS062_Authority(string contractCode)
        //{
        //    CommonUtil util = new CommonUtil();
        //    CTS062_ScreenParameter session = InitialScreenSession_CTS062();
        //    session.ScreenParameter.contractCode = util.ConvertContractCode(contractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
        //    return InitialScreenEnvironment("CTS062", session);
        //}

        /// <summary>
        /// Initial screen
        /// </summary>
        /// <param name="contractCode"></param>
        /// <returns></returns>
        public ActionResult InitialScreen_CTS062(string contractCode)
        {
            string lastOCC;
            dsSaleContractData dsSaleContract;
            IUserControlHandler userControlHandler;
            ISaleContractHandler saleContractHandler;
            ICommonHandler commonHandler;

            List<doSaleContractBasicInformation> listDOSaleContractBasicInformation;
            List<string> listFieldName;
            List<doMiscTypeCode> listMistTypeCode;
            List<doMiscTypeCode> listMistTypeCodeNew;

            ObjectResultData res = new ObjectResultData();
            CTS062_ScreenParameter session;

            try
            {
                session = CTS062_GetImportData();
                userControlHandler = ServiceContainer.GetService<IUserControlHandler>() as IUserControlHandler;
                saleContractHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                
                HasAuthority_CTS062(contractCode);
                if (ViewBag.Permission == true && ViewBag.IsSystemSuspending == false && (ViewBag.HasAuthorityContract == true || ViewBag.HasAuthorityOperation == true))
                {
                    //1.1 Get contract data
                    //1.1.1 Get last OCC
                    lastOCC = saleContractHandler.GetLastOCC(contractCode);
                    session.DOLastOCC.LastOCC = lastOCC;
                    ViewBag.LastOCC = lastOCC;

                    //1.1.2 Get entire contract data
                    dsSaleContract = saleContractHandler.GetEntireContract(contractCode, lastOCC);
                    session.DSSaleContract = dsSaleContract;

                    //1.2.1 Installatoin of last OCC must not completed
                    if ((dsSaleContract.dtTbt_SaleBasic[0].ChangeType != SaleChangeType.C_SALE_CHANGE_TYPE_NEW_SALE && dsSaleContract.dtTbt_SaleBasic[0].ChangeType != SaleChangeType.C_SALE_CHANGE_TYPE_ADD_SALE) &&
                        (dsSaleContract.dtTbt_SaleBasic[0].ChangeType == SaleProcessManageStatus.C_SALE_PROCESS_STATUS_ACCEPT_NOTCOMPLETE ||
                         dsSaleContract.dtTbt_SaleBasic[0].ChangeType == SaleProcessManageStatus.C_SALE_PROCESS_STATUS_CANCEL ||
                         dsSaleContract.dtTbt_SaleBasic[0].ChangeType == SaleProcessManageStatus.C_SALE_PROCESS_STATUS_COMPLETE_ACCEPT ||
                         dsSaleContract.dtTbt_SaleBasic[0].ChangeType == SaleProcessManageStatus.C_SALE_PROCESS_STATUS_COMPLETE_NOTACCEPT))
                        ViewBag.IsLastOCCCompleted = false;
                    else
                        ViewBag.IsLastOCCCompleted = true;

                    //1.3 Get data for ucSaleContractBasicInformation
                    listDOSaleContractBasicInformation = userControlHandler.GetSaleContractBasicInformationData(contractCode, lastOCC);

                    //1.4 Get payment method
                    listFieldName = new List<string>();
                    listMistTypeCode = new List<doMiscTypeCode>();
                    listMistTypeCodeNew = new List<doMiscTypeCode>();

                    listFieldName.Add(MiscType.C_PAYMENT_METHOD);
                    listMistTypeCode = commonHandler.GetMiscTypeCodeListByFieldName(listFieldName);

                    foreach (var item in listMistTypeCode)
                    {
                        if (item.ValueCode == MethodType.C_PAYMENT_METHOD_BANK_TRANSFER ||
                            item.ValueCode == MethodType.C_PAYMENT_METHOD_AUTO_TRANSFER ||
                            item.ValueCode == MethodType.C_PAYMENT_METHOD_CREDIT_CARD ||
                            item.ValueCode == MethodType.C_PAYMENT_METHOD_MESSENGER)
                            listMistTypeCodeNew.Add(item);
                    }

                    session.ListDOMiscTypeCode = listMistTypeCodeNew;

                    //1.5 Bind data to screen items
                    InitialScreenData_CTS062(listDOSaleContractBasicInformation, lastOCC);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Initial data of screen
        /// </summary>
        /// <param name="listDOSaleContractBasicInformation"></param>
        /// <param name="lastOCC"></param>
        public void InitialScreenData_CTS062(List<doSaleContractBasicInformation> listDOSaleContractBasicInformation, string lastOCC)
        {
            CTS062_ScreenParameter session;
            CommonUtil comU;

            try
            {
                session = CTS062_GetImportData();
                comU = new CommonUtil();
                ISaleContractHandler saleContractHandler;
                ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (listDOSaleContractBasicInformation.Count() != 0)
                {
                    saleContractHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;

                    ViewBag.ContractCode = comU.ConvertContractCode(listDOSaleContractBasicInformation[0].ContractCode,CommonUtil.CONVERT_TYPE.TO_SHORT);
                    ViewBag.PurchaserCustCode = comU.ConvertCustCode(listDOSaleContractBasicInformation[0].PurchaserCustCode,CommonUtil.CONVERT_TYPE.TO_SHORT);
                    ViewBag.RealCustomerCustCode = comU.ConvertCustCode(listDOSaleContractBasicInformation[0].RealCustomerCustCode.ToString(), CommonUtil.CONVERT_TYPE.TO_SHORT);
                    ViewBag.SiteCode = comU.ConvertSiteCode(listDOSaleContractBasicInformation[0].SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    ViewBag.PurchaserNameEN = listDOSaleContractBasicInformation[0].PurchaserNameEN;
                    ViewBag.PurchaserAddressEN = listDOSaleContractBasicInformation[0].PurchaserAddressEN;
                    ViewBag.SiteNameEN = listDOSaleContractBasicInformation[0].SiteNameEN;
                    ViewBag.SiteAddressEN = listDOSaleContractBasicInformation[0].SiteAddressEN;
                    ViewBag.PurchaserNameLC = listDOSaleContractBasicInformation[0].PurchaserNameLC;
                    ViewBag.PurchaserAddressLC = listDOSaleContractBasicInformation[0].PurchaserAddressLC;
                    ViewBag.SiteNameLC = listDOSaleContractBasicInformation[0].SiteNameLC;
                    ViewBag.SiteAddressLC = listDOSaleContractBasicInformation[0].SiteAddressLC;

                    if (listDOSaleContractBasicInformation[0].InstallationStatusCodeName != "" && listDOSaleContractBasicInformation[0].InstallationStatusCodeName != null)
                        ViewBag.InstallationStatusCodeName = listDOSaleContractBasicInformation[0].InstallationStatusCodeName;

                    ViewBag.OperationOfficeName = CommonUtil.TextCodeName(listDOSaleContractBasicInformation[0].OperationOfficeCode, listDOSaleContractBasicInformation[0].OperationOfficeName);
                    //ViewBag.QuotationTargetCode = comU.ConvertQuotationTargetCode(session.DSSaleContract.dtTbt_SaleBasic[0].QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    ViewBag.QuotationTargetCode = comU.ConvertContractCode(listDOSaleContractBasicInformation[0].ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    //ViewBag.SalesType = session.DSSaleContract.dtTbt_SaleBasic[0].SalesType.ToString();

                    List<doMiscTypeCode> miscs = new List<doMiscTypeCode>();
                    miscs.Add(new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_SALE_TYPE,
                        ValueCode = session.DSSaleContract.dtTbt_SaleBasic[0].SalesType
                    });

                    var outlst = commonhandler.GetMiscTypeCodeList(miscs);
                    if (outlst.Count == 1)
                    {
                        ViewBag.SalesType = outlst[0].ValueCodeDisplay;
                    }
                    else
                    {
                        ViewBag.SalesType = "";
                    }

                    if (listDOSaleContractBasicInformation[0].PurchaserCustomerImportant == null)
                        ViewBag.ImportantFlag = false;
                    else
                        ViewBag.ImportantFlag = (bool)listDOSaleContractBasicInformation[0].PurchaserCustomerImportant;

                    ViewBag.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_SALE;
                    ViewBag.TargetCodeType = TargetCodeType.C_TARGET_CODE_TYPE_CONTRACT_CODE;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Initial session of screen
        /// </summary>
        /// <returns></returns>
        public void InitialScreenSession_CTS062(CTS062_ScreenParameter importData)
        {
            try
            {
                importData.DSSaleContract = new dsSaleContractData();
                importData.ListBillingTemp = new List<tbt_BillingTemp>();
                importData.ListDTBillingClient = new List<dtBillingClientData>();
                importData.DOValidateBusiness = new CTS062_DOValidateBusiness();
                importData.DOLastOCC = new CTS062_DOLastOCC();
                importData.ScreenParameter = new CTS062_Parameter();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get data to ChangePlan grid
        /// </summary>
        /// <returns></returns>
        public ActionResult GetChangePlanGrid_CTS062()
        {
            ObjectResultData res = new ObjectResultData();  
            CTS062_DOChangePlan doChangePlanProduct;
            CTS062_DOChangePlan doChangePlanInstallation;
            CTS062_DOChangePlan doChangePlanSale;
            List<CTS062_DOChangePlan> listDOChangePlan;

            CTS062_ScreenParameter session;

            try
            {
                session = CTS062_GetImportData();
                listDOChangePlan = new List<CTS062_DOChangePlan>();

                if (session.DSSaleContract.dtTbt_SaleBasic != null)
	            {                   
                    if (session.DSSaleContract.dtTbt_SaleBasic.Count() != 0)
	                {
		                //Porduct-------------------------------------------------------------------------------
                        doChangePlanProduct = new CTS062_DOChangePlan();
                        //doChangePlanProduct.ID = "Product price (1)"; //จิงๆจะทำการใช้ javascript ไปดึง resource ไฟล์มาเพราะจะมีการเปลี่ยนภาษาด้วย
                        doChangePlanProduct.ID = CommonUtil.GetLabelFromResource("Contract", "CTS062", "lblProductPrice");

                        //if (session.DSSaleContract.dtTbt_SaleBasic[0].NormalProductPrice != null)
                        //    doChangePlanProduct.NormalPrice = CommonUtil.TextNumeric(session.DSQuotation.dtTbt_QuotationBasic.ProductPrice);

                        //if (session.DSSaleContract.dtTbt_SaleBasic[0].OrderProductPrice != null)
                        //    doChangePlanProduct.OrderPrice = CommonUtil.TextNumeric(session.DSSaleContract.dtTbt_SaleBasic[0].OrderProductPrice);

                        #region Normal Product Price

                        doChangePlanProduct.NormalPriceCurrencyType = session.DSQuotation.dtTbt_QuotationBasic.ProductPriceCurrencyType;
                        if (session.DSQuotation.dtTbt_QuotationBasic.ProductPriceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            doChangePlanProduct.NormalPrice = CommonUtil.TextNumeric(session.DSQuotation.dtTbt_QuotationBasic.ProductPriceUsd);
                        else
                            doChangePlanProduct.NormalPrice = CommonUtil.TextNumeric(session.DSQuotation.dtTbt_QuotationBasic.ProductPrice);

                        #endregion
                        #region Order Product Price

                        doChangePlanProduct.OrderPriceCurrencyType = session.DSSaleContract.dtTbt_SaleBasic[0].OrderProductPriceCurrencyType;
                        if (session.DSSaleContract.dtTbt_SaleBasic[0].OrderProductPriceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            doChangePlanProduct.OrderPrice = CommonUtil.TextNumeric(session.DSSaleContract.dtTbt_SaleBasic[0].OrderProductPriceUsd);
                        else
                            doChangePlanProduct.OrderPrice = CommonUtil.TextNumeric(session.DSSaleContract.dtTbt_SaleBasic[0].OrderProductPrice);

                        #endregion
                        #region Product Price Billing at Approval Fee

                        doChangePlanProduct.BillingAmt_ApproveContractCurrencyType = session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmt_ApproveContractCurrencyType;
                        if (session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmt_ApproveContractCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            doChangePlanProduct.BillingAmt_ApproveContract = CommonUtil.TextNumeric(session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmt_ApproveContractUsd);
                        else
                            doChangePlanProduct.BillingAmt_ApproveContract = CommonUtil.TextNumeric(session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmt_ApproveContract);

                        #endregion
                        #region Product Price Billing at Partial Fee

                        doChangePlanProduct.BillingAmt_PartialFeeCurrencyType = session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmt_PartialFeeCurrencyType;
                        if (session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmt_PartialFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            doChangePlanProduct.BillingAmt_PartialFee = CommonUtil.TextNumeric(session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmt_PartialFeeUsd);
                        else
                            doChangePlanProduct.BillingAmt_PartialFee = CommonUtil.TextNumeric(session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmt_PartialFee);

                        #endregion
                        #region Product Price Billing at Customet Acceptance

                        doChangePlanProduct.BillingAmt_AcceptanceCurrencyType = session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmt_AcceptanceCurrencyType;
                        if (session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmt_AcceptanceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            doChangePlanProduct.BillingAmt_Acceptance = CommonUtil.TextNumeric(session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmt_AcceptanceUsd);
                        else
                            doChangePlanProduct.BillingAmt_Acceptance = CommonUtil.TextNumeric(session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmt_Acceptance);

                        #endregion
                                              
                        listDOChangePlan.Add(doChangePlanProduct);

                        //Installation----------------------------------------------------------------------------------------
                        doChangePlanInstallation = new CTS062_DOChangePlan();
                        //doChangePlanInstallation.ID = "Installation fee (2)"; //จิงๆจะทำการใช้ javascript ไปดึง resource ไฟล์มาเพราะจะมีการเปลี่ยนภาษาด้วย
                        doChangePlanInstallation.ID = CommonUtil.GetLabelFromResource("Contract", "CTS062", "lblInstallationFee");

                        //if (session.DSQuotation.dtTbt_QuotationBasic.ProductPrice.HasValue)
                        //    doChangePlanInstallation.NormalPrice = CommonUtil.TextNumeric(session.DSQuotation.dtTbt_QuotationBasic.InstallationFee);

                        //if (session.DSQuotation.dtTbt_QuotationBasic.InstallationFee.HasValue)
                        //    doChangePlanInstallation.OrderPrice = CommonUtil.TextNumeric(session.DSQuotation.dtTbt_QuotationBasic.InstallationFee);

                        #region Normal Installation Fee

                        doChangePlanInstallation.NormalPriceCurrencyType = session.DSQuotation.dtTbt_QuotationBasic.InstallationFeeCurrencyType;
                        if (session.DSQuotation.dtTbt_QuotationBasic.InstallationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            doChangePlanInstallation.NormalPrice = CommonUtil.TextNumeric(session.DSQuotation.dtTbt_QuotationBasic.InstallationFeeUsd);
                        else
                            doChangePlanInstallation.NormalPrice = CommonUtil.TextNumeric(session.DSQuotation.dtTbt_QuotationBasic.InstallationFee);

                        #endregion
                        #region Order Installation Fee

                        doChangePlanInstallation.OrderPriceCurrencyType = session.DSSaleContract.dtTbt_SaleBasic[0].OrderInstallFeeCurrencyType;
                        if (session.DSSaleContract.dtTbt_SaleBasic[0].OrderInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            doChangePlanInstallation.OrderPrice = CommonUtil.TextNumeric(session.DSSaleContract.dtTbt_SaleBasic[0].OrderInstallFeeUsd);
                        else
                            doChangePlanInstallation.OrderPrice = CommonUtil.TextNumeric(session.DSSaleContract.dtTbt_SaleBasic[0].OrderInstallFee);

                        #endregion
                        #region Installation Fee Billing at Approval

                        doChangePlanInstallation.BillingAmt_ApproveContractCurrencyType = session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_ApproveContractCurrencyType;
                        if (session.DSSaleContract.dtTbt_SaleBasic[0].OrderInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            doChangePlanInstallation.BillingAmt_ApproveContract = CommonUtil.TextNumeric(session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_ApproveContractUsd);
                        else
                            doChangePlanInstallation.BillingAmt_ApproveContract = CommonUtil.TextNumeric(session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_ApproveContract);

                        #endregion
                        #region Installation Fee Billing at Partial Fee

                        doChangePlanInstallation.BillingAmt_PartialFeeCurrencyType = session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_PartialFeeCurrencyType;
                        if (session.DSSaleContract.dtTbt_SaleBasic[0].OrderInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            doChangePlanInstallation.BillingAmt_PartialFee = CommonUtil.TextNumeric(session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_PartialFeeUsd);
                        else
                            doChangePlanInstallation.BillingAmt_PartialFee = CommonUtil.TextNumeric(session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_PartialFee);

                        #endregion
                        #region Installation Fee Billing at Customer Acceptance

                        doChangePlanInstallation.BillingAmt_AcceptanceCurrencyType = session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_AcceptanceCurrencyType;
                        if (session.DSSaleContract.dtTbt_SaleBasic[0].OrderInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            doChangePlanInstallation.BillingAmt_Acceptance = CommonUtil.TextNumeric(session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_AcceptanceUsd);
                        else
                            doChangePlanInstallation.BillingAmt_Acceptance = CommonUtil.TextNumeric(session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmtInstallation_Acceptance);

                        #endregion

                        listDOChangePlan.Add(doChangePlanInstallation);

                        ////Sale----------------------------------------------------------------------------------------
                        //doChangePlanSale = new CTS062_DOChangePlan();
                        ////doChangePlanSale.ID = "Sale price (1) + (2) "; //จิงๆจะทำการใช้ javascript ไปดึง resource ไฟล์มาเพราะจะมีการเปลี่ยนภาษาด้วย
                        //doChangePlanSale.ID = CommonUtil.GetLabelFromResource("Contract", "CTS062", "lblSalePrice");
                        //if (session.DSQuotation.dtTbt_QuotationBasic.ProductPrice.HasValue
                        //    || session.DSQuotation.dtTbt_QuotationBasic.InstallationFee.HasValue)
                        //    doChangePlanSale.NormalPrice = CommonUtil.TextNumeric(session.DSQuotation.dtTbt_QuotationBasic.ProductPrice.GetValueOrDefault()
                        //        + session.DSQuotation.dtTbt_QuotationBasic.InstallationFee.GetValueOrDefault());
                        //else
                        //{
                        //    doChangePlanSale.NormalPrice = String.Empty;
                        //}

                        //if (session.DSSaleContract.dtTbt_SaleBasic[0].OrderInstallFee != null)
                        //    doChangePlanSale.OrderPrice = CommonUtil.TextNumeric(session.DSSaleContract.dtTbt_SaleBasic[0].OrderSalePrice);

                        //if (session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmt_ApproveContract != null)
                        //    doChangePlanSale.BillingAmt_ApproveContract = CommonUtil.TextNumeric(session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmt_ApproveContract);

                        //if (session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmt_PartialFee != null)
                        //    doChangePlanSale.PartialFee = CommonUtil.TextNumeric(session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmt_PartialFee);

                        //if (session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmt_Acceptance != null)
                        //    doChangePlanSale.BillingAmt_Acceptance = CommonUtil.TextNumeric(session.DSSaleContract.dtTbt_SaleBasic[0].BillingAmt_Acceptance);
                        
                        //listDOChangePlan.Add(doChangePlanSale);

                        res.ResultData = listDOChangePlan;
	                }                
                }
            }
            catch (Exception ex)
            {
               res.AddErrorMessage(ex);                
            }

            return Json(res);
        }

        /// <summary>
        /// Get detail of ChangePlanANDSpecify data
        /// </summary>
        /// <returns></returns>
        public ActionResult GetChangePlanANDSpecifyDetail_CTS062()
        {            
            ObjectResultData res = new ObjectResultData();
            IMasterHandler masterHandler;
            IEmployeeMasterHandler employeeHandler;
            ICommonContractHandler commonContractHandler; 

            CTS062_DOChangePlanDetail doChangePlanDetail;
            CTS062_ScreenParameter session;

            CommonUtil comU;

            try
            {
                session = CTS062_GetImportData();
                comU = new CommonUtil();
                masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                employeeHandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                commonContractHandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
                
                doChangePlanDetail = new CTS062_DOChangePlanDetail();
                doChangePlanDetail.ProductName = GetProductName_CTS062();
                doChangePlanDetail.Alphabet = session.DSQuotation.dtTbt_QuotationBasic.Alphabet;
                doChangePlanDetail.ExpectedInstallCompleteDate = session.DSSaleContract.dtTbt_SaleBasic[0].ExpectedInstallCompleteDate;
                doChangePlanDetail.ExpectedCustAcceptanceDate = session.DSSaleContract.dtTbt_SaleBasic[0].ExpectedCustAcceptanceDate;

                doChangePlanDetail.ConnectionFlag = session.DSSaleContract.dtTbt_SaleBasic[0].ConnectionFlag;
                doChangePlanDetail.ConnectTargetCode = session.DSSaleContract.dtTbt_SaleBasic[0].ConnectTargetCode;

                doChangePlanDetail.DistributedType = session.DSSaleContract.dtTbt_SaleBasic[0].DistributedInstallTypeCode;
                if (session.DSSaleContract.dtTbt_SaleBasic[0].DistributedOriginCode != null && session.DSSaleContract.dtTbt_SaleBasic[0].DistributedOriginCode != "")                
                    doChangePlanDetail.DistributedCode = comU.ConvertContractCode(session.DSSaleContract.dtTbt_SaleBasic[0].DistributedOriginCode,CommonUtil.CONVERT_TYPE.TO_SHORT);
                
                doChangePlanDetail.SalesmanEmpNo1 = session.DSSaleContract.dtTbt_SaleBasic[0].SalesmanEmpNo1;
                doChangePlanDetail.SalesmanEmpNo2 = session.DSSaleContract.dtTbt_SaleBasic[0].SalesmanEmpNo2;
                doChangePlanDetail.SalesmanEmpNo3 = session.DSSaleContract.dtTbt_SaleBasic[0].SalesmanEmpNo3;
                doChangePlanDetail.SalesmanEmpNo4 = session.DSSaleContract.dtTbt_SaleBasic[0].SalesmanEmpNo4;
                doChangePlanDetail.SalesmanEmpNo5 = session.DSSaleContract.dtTbt_SaleBasic[0].SalesmanEmpNo5;
                doChangePlanDetail.SalesmanEmpNo6 = session.DSSaleContract.dtTbt_SaleBasic[0].SalesmanEmpNo6;
                doChangePlanDetail.SalesmanEmpNo7 = session.DSSaleContract.dtTbt_SaleBasic[0].SalesmanEmpNo7;
                doChangePlanDetail.SalesmanEmpNo8 = session.DSSaleContract.dtTbt_SaleBasic[0].SalesmanEmpNo8;
                doChangePlanDetail.SalesmanEmpNo9 = session.DSSaleContract.dtTbt_SaleBasic[0].SalesmanEmpNo9;
                doChangePlanDetail.SalesmanEmpNo10 = session.DSSaleContract.dtTbt_SaleBasic[0].SalesmanEmpNo10;

                //doChangePlanDetail.NegotiationStaffEmpNo1 = session.DSSaleContract.dtTbt_SaleBasic[0].NegotiationStaffEmpNo1;
                //doChangePlanDetail.NegotiationStaffEmpNo2 = session.DSSaleContract.dtTbt_SaleBasic[0].NegotiationStaffEmpNo2;

                doChangePlanDetail.ApproveNo1 = session.DSSaleContract.dtTbt_SaleBasic[0].ApproveNo1;
                doChangePlanDetail.ApproveNo2 = session.DSSaleContract.dtTbt_SaleBasic[0].ApproveNo2;
                doChangePlanDetail.ApproveNo3 = session.DSSaleContract.dtTbt_SaleBasic[0].ApproveNo3;
                doChangePlanDetail.ApproveNo4 = session.DSSaleContract.dtTbt_SaleBasic[0].ApproveNo4;
                doChangePlanDetail.ApproveNo5 = session.DSSaleContract.dtTbt_SaleBasic[0].ApproveNo5;

                if (employeeHandler.GetEmployeeNameByEmpNo(doChangePlanDetail.SalesmanEmpNo1).Count() != 0)
                {
                    if (doChangePlanDetail.SalesmanEmpNo1 != null)                    
                        doChangePlanDetail.SalesmanEmpName1 = employeeHandler.GetEmployeeNameByEmpNo(doChangePlanDetail.SalesmanEmpNo1)[0].EmployeeNameDisplay;
                }

                if (employeeHandler.GetEmployeeNameByEmpNo(doChangePlanDetail.SalesmanEmpNo2).Count() != 0)
                {
                    if (doChangePlanDetail.SalesmanEmpNo2 != null)
                        doChangePlanDetail.SalesmanEmpName2 = employeeHandler.GetEmployeeNameByEmpNo(doChangePlanDetail.SalesmanEmpNo2)[0].EmployeeNameDisplay;
                }

                if (employeeHandler.GetEmployeeNameByEmpNo(doChangePlanDetail.SalesmanEmpNo3).Count() != 0)
                {
                    if (doChangePlanDetail.SalesmanEmpNo3 != null)
                        doChangePlanDetail.SalesmanEmpName3 = employeeHandler.GetEmployeeNameByEmpNo(doChangePlanDetail.SalesmanEmpNo3)[0].EmployeeNameDisplay;
                }

                if (employeeHandler.GetEmployeeNameByEmpNo(doChangePlanDetail.SalesmanEmpNo4).Count() != 0)
                {
                    if (doChangePlanDetail.SalesmanEmpNo4 != null)
                        doChangePlanDetail.SalesmanEmpName4 = employeeHandler.GetEmployeeNameByEmpNo(doChangePlanDetail.SalesmanEmpNo4)[0].EmployeeNameDisplay;
                }

                if (employeeHandler.GetEmployeeNameByEmpNo(doChangePlanDetail.SalesmanEmpNo5).Count() != 0)
                {
                    if (doChangePlanDetail.SalesmanEmpNo5 != null)
                        doChangePlanDetail.SalesmanEmpName5 = employeeHandler.GetEmployeeNameByEmpNo(doChangePlanDetail.SalesmanEmpNo5)[0].EmployeeNameDisplay;
                }

                if (employeeHandler.GetEmployeeNameByEmpNo(doChangePlanDetail.SalesmanEmpNo6).Count() != 0)
                {
                    if (doChangePlanDetail.SalesmanEmpNo6 != null)
                        doChangePlanDetail.SalesmanEmpName6 = employeeHandler.GetEmployeeNameByEmpNo(doChangePlanDetail.SalesmanEmpNo6)[0].EmployeeNameDisplay;
                }

                if (employeeHandler.GetEmployeeNameByEmpNo(doChangePlanDetail.SalesmanEmpNo7).Count() != 0)
                {
                    if (doChangePlanDetail.SalesmanEmpNo7 != null)
                        doChangePlanDetail.SalesmanEmpName7 = employeeHandler.GetEmployeeNameByEmpNo(doChangePlanDetail.SalesmanEmpNo7)[0].EmployeeNameDisplay;
                }

                if (employeeHandler.GetEmployeeNameByEmpNo(doChangePlanDetail.SalesmanEmpNo8).Count() != 0)
                {
                    if (doChangePlanDetail.SalesmanEmpNo8 != null)
                        doChangePlanDetail.SalesmanEmpName8 = employeeHandler.GetEmployeeNameByEmpNo(doChangePlanDetail.SalesmanEmpNo8)[0].EmployeeNameDisplay;
                }

                if (employeeHandler.GetEmployeeNameByEmpNo(doChangePlanDetail.SalesmanEmpNo9).Count() != 0)
                {
                    if (doChangePlanDetail.SalesmanEmpNo9 != null)
                        doChangePlanDetail.SalesmanEmpName9 = employeeHandler.GetEmployeeNameByEmpNo(doChangePlanDetail.SalesmanEmpNo9)[0].EmployeeNameDisplay;
                }

                if (employeeHandler.GetEmployeeNameByEmpNo(doChangePlanDetail.SalesmanEmpNo10).Count() != 0)
                {
                    if (doChangePlanDetail.SalesmanEmpNo10 != null)
                        doChangePlanDetail.SalesmanEmpName10 = employeeHandler.GetEmployeeNameByEmpNo(doChangePlanDetail.SalesmanEmpNo10)[0].EmployeeNameDisplay;
                }

                //if (employeeHandler.GetEmployeeNameByEmpNo(doChangePlanDetail.NegotiationStaffEmpNo1).Count() != 0)
                //    doChangePlanDetail.NegotiationStaffEmpName1 = employeeHandler.GetEmployeeNameByEmpNo(doChangePlanDetail.NegotiationStaffEmpNo1)[0].EmployeeNameDisplay;

                //if (employeeHandler.GetEmployeeNameByEmpNo(doChangePlanDetail.NegotiationStaffEmpNo2).Count() != 0)
                //    doChangePlanDetail.NegotiationStaffEmpName2 = employeeHandler.GetEmployeeNameByEmpNo(doChangePlanDetail.NegotiationStaffEmpNo2)[0].EmployeeNameDisplay;

                res.ResultData = doChangePlanDetail;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get detail of BillingTarget information
        /// </summary>
        /// <returns></returns>
        public ActionResult GetBillingTargetInformationDetail_CTS062()
        {
            ObjectResultData res = new ObjectResultData();
            CTS062_DOBillingTargetDetailData doBillingTargetDetail;

            CTS062_ScreenParameter session;
            CommonUtil comU;

            try
            {
                session = CTS062_GetImportData();
                doBillingTargetDetail = new CTS062_DOBillingTargetDetailData();

                comU = new CommonUtil();

                if (session.ListBillingTemp.Count() != 0)
                {
                    doBillingTargetDetail.BillingTargetCodeDetail = comU.ConvertBillingTargetCode(session.ListBillingTemp[0].BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    doBillingTargetDetail.BillingOCC = session.ListBillingTemp[0].BillingOCC;
                    doBillingTargetDetail.BillingOffice = session.ListBillingTemp[0].BillingOfficeCode;
                }

                if (session.ListDTBillingClient.Count() != 0)
                {
                    doBillingTargetDetail.BillingClientCodeDetail = comU.ConvertBillingClientCode(session.ListDTBillingClient[0].BillingClientCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    doBillingTargetDetail.FullNameEN = session.ListDTBillingClient[0].FullNameEN;
                    doBillingTargetDetail.BranchNameEN = session.ListDTBillingClient[0].BranchNameEN;
                    doBillingTargetDetail.AddressEN = session.ListDTBillingClient[0].AddressEN;
                    doBillingTargetDetail.FullNameLC = session.ListDTBillingClient[0].FullNameLC;
                    doBillingTargetDetail.BranchNameLC = session.ListDTBillingClient[0].BranchNameLC;
                    doBillingTargetDetail.AddressLC = session.ListDTBillingClient[0].AddressLC;
                    doBillingTargetDetail.Nationality = session.ListDTBillingClient[0].NationalityEN; //Wait for edit to Nationality which follow culture from billingTemp;
                    doBillingTargetDetail.PhoneNo = session.ListDTBillingClient[0].PhoneNo;
                    doBillingTargetDetail.BusinessType = session.ListDTBillingClient[0].BusinessTypeNameEN; //Wait for edit to BusinessTypeName which follow culture from billingTemp;
                    doBillingTargetDetail.IDNo = session.ListDTBillingClient[0].IDNo;                    
                }
          
                return Json(doBillingTargetDetail);
            }
            catch (Exception ex)
            {
                res.ResultData = false;
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Get data to BillingTarget Information detail grid
        /// </summary>
        /// <returns></returns>
        public ActionResult GetBillingTargetInformationDetailGrid_CTS062()
        {
            decimal total = 0;
            ObjectResultData res = new ObjectResultData();
            CTS062_DOBillingTargetDetailGridData doBillingTargetDetailGrid;
            List<CTS062_DOBillingTargetDetailGridData> listDOBillingTargetDetailGrid;

            CTS062_ScreenParameter session;
            
            try
            {
                session = CTS062_GetImportData();
                listDOBillingTargetDetailGrid = new List<CTS062_DOBillingTargetDetailGridData>();

                //--------------------------------------------------------------------

                List<tbt_BillingTemp> btmp = new List<tbt_BillingTemp>();

                #region Product Price for Approve

                btmp.Add(new tbt_BillingTemp()
                {
                    //BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_SALE_PRICE,
                    BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_PRODUCT_PRICE,
                    BillingTiming = BillingTiming.C_BILLING_TIMING_APPROVE_CONTRACT
                });

                #endregion
                #region Product Price for Partial Fee

                btmp.Add(new tbt_BillingTemp()
                {
                    //BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_SALE_PRICE,
                    BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_PRODUCT_PRICE,
                    BillingTiming = BillingTiming.C_BILLING_TIMING_PARTIAL
                });

                #endregion
                #region Product Price for Customer Acceptance

                btmp.Add(new tbt_BillingTemp()
                {
                    //BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_SALE_PRICE,
                    BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_PRODUCT_PRICE,
                    BillingTiming = BillingTiming.C_BILLING_TIMING_ACCEPTANCE
                });

                #endregion
                
                #region Installation Fee for Approve

                btmp.Add(new tbt_BillingTemp()
                {
                    BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE,
                    BillingTiming = BillingTiming.C_BILLING_TIMING_APPROVE_CONTRACT
                });

                #endregion
                #region Installation Fee for Partial Fee

                btmp.Add(new tbt_BillingTemp()
                {
                    BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE,
                    BillingTiming = BillingTiming.C_BILLING_TIMING_PARTIAL
                });

                #endregion
                #region Installation Fee for Customer Acceptance

                btmp.Add(new tbt_BillingTemp()
                {
                    BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE,
                    BillingTiming = BillingTiming.C_BILLING_TIMING_ACCEPTANCE
                });

                #endregion

                foreach (tbt_BillingTemp tmp in btmp)
                {
                    doBillingTargetDetailGrid = new CTS062_DOBillingTargetDetailGridData();
                    
                    List<tbt_BillingTemp> billing = session.ListBillingTemp.FindAll(delegate (tbt_BillingTemp s)
                    {
                        return s.BillingType == tmp.BillingType
                               && s.BillingTiming == tmp.BillingTiming;
                    });
                    if (billing.Count() != 0)
                    {
                        doBillingTargetDetailGrid.AmountCurrencyType = billing[0].BillingAmtCurrencyType;
                        if (doBillingTargetDetailGrid.AmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            doBillingTargetDetailGrid.Amount = CommonUtil.TextNumeric(billing[0].BillingAmtUsd);
                        else
                            doBillingTargetDetailGrid.Amount = CommonUtil.TextNumeric(billing[0].BillingAmt);

                        if (tmp.BillingTiming == BillingTiming.C_BILLING_TIMING_ACCEPTANCE)
                            doBillingTargetDetailGrid.PayMethod = billing[0].PayMethod;
                    }
                    else
                    {
                        doBillingTargetDetailGrid.AmountCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;
                        doBillingTargetDetailGrid.Amount = CommonUtil.TextNumeric(0);

                        if (tmp.BillingTiming == BillingTiming.C_BILLING_TIMING_ACCEPTANCE)
                            doBillingTargetDetailGrid.PayMethod = PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER;
                    }

                    listDOBillingTargetDetailGrid.Add(doBillingTargetDetailGrid);
                }

                res.ResultData = listDOBillingTargetDetailGrid;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get data of ProductName
        /// </summary>
        /// <returns></returns>
        public string GetProductName_CTS062()
        {
            string productName = "";
            ObjectResultData res = new ObjectResultData();
            IMasterHandler masterHandler;
            List<tbm_Product> listProduct;
            List<CTS062_DOProductName> listDOProductName;
            CTS062_ScreenParameter session;

            try
            {
                //2.6 Show product name 
                session = CTS062_GetImportData();
                masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                listProduct = masterHandler.GetTbm_Product(session.DSQuotation.dtTbt_QuotationBasic.ProductCode, null);
                listDOProductName = CommonUtil.ClonsObjectList<tbm_Product, CTS062_DOProductName>(listProduct);
                CommonUtil.MappingObjectLanguage(listDOProductName);

                if (listDOProductName.Count() != 0)
                    productName = listDOProductName[0].ProductName.ToString();

                return productName;
            }
            catch (Exception ex)
            {
            }

            return productName;
        }

        /// <summary>
        /// Get data of Miscellaneous
        /// </summary>
        /// <returns></returns>
        public string GetMiscellaneous_CTS062()
        {
            string saleType = "";
            ObjectResultData res = new ObjectResultData();
            ICommonHandler commonHandler;
            List<doMiscTypeCode> listMistTypeCodeNew;
            List<doMiscTypeCode> listDOMiscTypeCode;
            List<string> listFieldName;

            CTS062_ScreenParameter session;

            try
            {
                //2.6 Show product name 
                listFieldName = new List<string>();
                session = CTS062_GetImportData();
                commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                listFieldName.Add(MiscType.C_SALE_TYPE);
                listDOMiscTypeCode = commonHandler.GetMiscTypeCodeListByFieldName(listFieldName);
                listMistTypeCodeNew = new List<doMiscTypeCode>();
                if (listDOMiscTypeCode.Count() != 0)
                    saleType = listDOMiscTypeCode[0].ValueDisplay;

                return saleType;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return saleType;
        }

        /// <summary>
        /// Get data of Employee
        /// </summary>
        /// <param name="doEmployee"></param>
        /// <returns></returns>
        public ActionResult GetActiveEmployee_CTS062(CTS062_DOEmployee doEmployee)
        {
            ObjectResultData res = new ObjectResultData();
            IMasterHandler masterHandler;
            IEmployeeMasterHandler employeeHandler;
            List<tbm_Employee> listEmployee;
            List<dtEmpNo> listEmpNo;

            try
            {
                masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                employeeHandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;

                listEmployee = masterHandler.GetActiveEmployee(doEmployee.EmpNo);

                if (listEmployee.Count() == 0)
                {
                    //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0012, "");
                    return Json(res);
                }

                listEmpNo = employeeHandler.GetEmployeeNameByEmpNo(doEmployee.EmpNo);
                if (listEmpNo.Count() != 0)
                    doEmployee.EmpName = listEmpNo[0].EmployeeNameDisplay;

                return Json(doEmployee);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Get data to PaymentMethod ComboBox
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetComboBoxPaymentMethod_CTS062(string id)
        {
            List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
            CTS062_ScreenParameter session;

            try
            {
                session = CTS062_GetImportData();
                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                {
                    new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_PAYMENT_METHOD,
                        ValueCode = "%"
                    }
                };

                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                lst = hand.GetMiscTypeCodeList(session.ListDOMiscTypeCode);
            }
            catch
            {
            }

            if (lst == null)
                lst = new List<doMiscTypeCode>();

            string display = "ValueCodeDisplay";
            return Json(CommonUtil.CommonComboBox<doMiscTypeCode>(id, lst, display, "ValueCode", null, true).ToString());
        }

        /// <summary>
        /// Reset data of screen
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetData_CTS062()
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil util = new CommonUtil();

            ISaleContractHandler salehandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
            IUserControlHandler usercontrolhandler = ServiceContainer.GetService<IUserControlHandler>() as IUserControlHandler;

            try
            {
                CTS062_ScreenParameter session = GetScreenObject<CTS062_ScreenParameter>();
                InitialScreen_CTS062(session.ContractCode);
                session = GetScreenObject<CTS062_ScreenParameter>();

                string lastOCC = salehandler.GetLastOCC(session.ContractCode);
                string expectedInstallCompleteDate = CommonUtil.TextDate(salehandler.GetTbt_SaleBasic(session.DSSaleContract.dtTbt_SaleBasic[0].ContractCode, lastOCC, true)[0].ExpectedInstallCompleteDate);
                var saleContractInfo = usercontrolhandler.GetSaleContractBasicInformationData(session.ContractCode, null);

                CTS062_ScreenOutputObject outObj = new CTS062_ScreenOutputObject()
                {
                    CanOperate = true,
                    ContractCode = session.ContractCode,
                    ContractCodeShort = util.ConvertContractCode(session.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                    ExpectedInstallCompleteDate = expectedInstallCompleteDate,
                    InstallationStatusCode = saleContractInfo[0].InstallationStatusCode,
                    InstallationStatusCodeName = saleContractInfo[0].InstallationStatusCodeName,
                    LastOCC = lastOCC,
                    ImportantFlag = saleContractInfo[0].PurchaserCustomerImportant.GetValueOrDefault(),
                    OperationOfficeName = CommonUtil.TextCodeName(saleContractInfo[0].OperationOfficeCode, saleContractInfo[0].OperationOfficeName),
                    PurchaserAddressEN = saleContractInfo[0].PurchaserAddressEN,
                    PurchaserAddressLC = saleContractInfo[0].PurchaserAddressLC,
                    PurchaserCustCode = util.ConvertCustCode(saleContractInfo[0].PurchaserCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                    PurchaserNameEN = saleContractInfo[0].PurchaserNameEN,
                    PurchaserNameLC = saleContractInfo[0].PurchaserNameLC,
                    RealCustomerCustCode = util.ConvertCustCode(saleContractInfo[0].RealCustomerCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                    SiteAddressEN = saleContractInfo[0].SiteAddressEN,
                    SiteAddressLC = saleContractInfo[0].SiteAddressLC,
                    SiteCode = util.ConvertSiteCode(saleContractInfo[0].SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                    SiteNameEN = saleContractInfo[0].SiteNameEN,
                    SiteNameLC = saleContractInfo[0].SiteNameLC,
                    ServiceTypeCode = ServiceType.C_SERVICE_TYPE_SALE,
                    TargetCodeType = TargetCodeType.C_TARGET_CODE_TYPE_CONTRACT_CODE
                };

                res.ResultData = outObj;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        #endregion

        #region Validation

        /// <summary>
        /// Check screen has authority
        /// </summary>
        /// <param name="contractCode"></param>
        public void HasAuthority_CTS062(string contractCode)
        {
            ObjectResultData res = new ObjectResultData();
            ICommonHandler commonHandler;
            ISaleContractHandler saleContractHandler;
            List<tbt_SaleBasic> listSaleBasic;
            dsSaleContractData dsSaleContract;

            string lastOCC;

            try
            {
                commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                saleContractHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;

                lastOCC = saleContractHandler.GetLastOCC(contractCode);
                ViewBag.LastOCC = lastOCC;

                //1.1.2 Get entire contract data
                dsSaleContract = saleContractHandler.GetEntireContract(contractCode, lastOCC);

                //1.1 Check suspending
                if (commonHandler.IsSystemSuspending())
                    ViewBag.IsSystemSuspending = true;
                else
                    ViewBag.IsSystemSuspending = false;

                //1.2 Check user's permission
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_CQ12_CHANGE_PLAN, FunctionID.C_FUNC_ID_OPERATE))
                    ViewBag.Permission = false;
                else
                    ViewBag.Permission = true;

                if (dsSaleContract != null)
                {
                    if (dsSaleContract.dtTbt_SaleBasic.Count != 0)
                    {
                        if (CommonUtil.dsTransData.dtOfficeData.FindAll(delegate(OfficeDataDo s) { return s.OfficeCode == dsSaleContract.dtTbt_SaleBasic[0].ContractOfficeCode; }).Count == 0)
                            ViewBag.HasAuthorityContract = false;
                        else
                            ViewBag.HasAuthorityContract = true;

                        if (CommonUtil.dsTransData.dtOfficeData.FindAll(delegate(OfficeDataDo s) { return s.OfficeCode == dsSaleContract.dtTbt_SaleBasic[0].OperationOfficeCode; }).Count == 0)
                            ViewBag.HasAuthorityOperation = false;
                        else
                            ViewBag.HasAuthorityOperation = true;
                    }
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
        }

        /// <summary>
        /// Validate Business of screen
        /// </summary>
        /// <param name="doRegister"></param>
        /// <returns></returns>
        public ObjectResultData ValidateBusiness_CTS062(CTS062_DORegisterData doRegister)
        {
            ObjectResultData res = new ObjectResultData();                        
            ISaleContractHandler saleContractHandler; 
            IEmployeeMasterHandler employeeMasterHandler;
            IMasterHandler masterHandler;
            IEmployeeMasterHandler employeeHandler;
            List<tbt_SaleBasic> listSaleBasic;
            List<tbm_Employee> listEmployee;
            List<string> listSequenceApproveNo;
            CommonUtil util = new CommonUtil();

            try
            {
                listSequenceApproveNo = new List<string>();
                saleContractHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                employeeMasterHandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                employeeHandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                IRentralContractHandler rentalhandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;

                // Validate Sequencial Approve No

                //if (doRegister.BillingAmt_Acceptance == null || doRegister.BillingAmt_Acceptance == "")
                //    doRegister.BillingAmt_Acceptance = "0";

                //if (doRegister.BillingAmt_ApproveContract == null || doRegister.BillingAmt_ApproveContract == "")
                //    doRegister.BillingAmt_ApproveContract = "0";

                //if (doRegister.BillingAmt_CompleteInstallation == null || doRegister.BillingAmt_CompleteInstallation == "")
                //    doRegister.BillingAmt_CompleteInstallation = "0";

                //if (doRegister.NormalSalePrice == null || doRegister.NormalSalePrice == "")
                //    doRegister.NormalSalePrice = "0";

                //if (doRegister.OrderInstallFee == null || doRegister.OrderInstallFee == "")
                //    doRegister.OrderInstallFee = "0";

                //if (doRegister.OrderProductPrice == null || doRegister.OrderProductPrice == "")
                //    doRegister.OrderProductPrice = "0";

                //if (doRegister.OrderSalePrice == null || doRegister.OrderSalePrice == "")
                //    doRegister.OrderSalePrice = "0";

                //if (doRegister.PartialFee == null || doRegister.PartialFee == "")
                //    doRegister.PartialFee = "0";

                ////Protect cast error
                //if (doRegister.OrderSalePrice == null || doRegister.OrderSalePrice == "")
                //    doRegister.OrderSalePrice = "0";

                //if (doRegister.BillingAmt_ApproveContract == null || doRegister.BillingAmt_ApproveContract == "")
                //    doRegister.BillingAmt_ApproveContract = "0";

                //if (doRegister.PartialFee == null || doRegister.PartialFee == "")
                //    doRegister.PartialFee = "0";

                //if (doRegister.BillingAmt_Acceptance == null || doRegister.BillingAmt_Acceptance == "")
                //    doRegister.BillingAmt_Acceptance = "0";

                if (CommonUtil.IsNullOrEmpty(doRegister.OrderProductPrice))
                    doRegister.OrderProductPrice = "0";
                if (CommonUtil.IsNullOrEmpty(doRegister.BillingAmt_ApproveContract))
                    doRegister.BillingAmt_ApproveContract = "0";
                if (CommonUtil.IsNullOrEmpty(doRegister.BillingAmt_PartialFee))
                    doRegister.BillingAmt_PartialFee = "0";
                if (CommonUtil.IsNullOrEmpty(doRegister.BillingAmt_Acceptance))
                    doRegister.BillingAmt_Acceptance = "0";

                if (CommonUtil.IsNullOrEmpty(doRegister.OrderInstallFee))
                    doRegister.OrderInstallFee = "0";
                if (CommonUtil.IsNullOrEmpty(doRegister.BillingAmtInstallation_ApproveContract))
                    doRegister.BillingAmtInstallation_ApproveContract = "0";
                if (CommonUtil.IsNullOrEmpty(doRegister.BillingAmtInstallation_PartialFee))
                    doRegister.BillingAmtInstallation_PartialFee = "0";
                if (CommonUtil.IsNullOrEmpty(doRegister.BillingAmtInstallation_Acceptance))
                    doRegister.BillingAmtInstallation_Acceptance = "0";


                if (CommonUtil.IsNullOrEmpty(doRegister.SalePrice_Approval))
                    doRegister.SalePrice_Approval = "0";
                if (CommonUtil.IsNullOrEmpty(doRegister.SalePrice_Partial))
                    doRegister.SalePrice_Partial = "0";
                if (CommonUtil.IsNullOrEmpty(doRegister.SalePrice_Acceptance))
                    doRegister.SalePrice_Acceptance = "0";

                if (CommonUtil.IsNullOrEmpty(doRegister.InstallationFee_Approval))
                    doRegister.InstallationFee_Approval = "0";
                if (CommonUtil.IsNullOrEmpty(doRegister.InstallationFee_Partial))
                    doRegister.InstallationFee_Partial = "0";
                if (CommonUtil.IsNullOrEmpty(doRegister.InstallationFee_Acceptance))
                    doRegister.InstallationFee_Acceptance = "0";

                //4.1 ValidateBusiness



                ////4.1.1	Summary of billing price must equal to order sale price
                //if (decimal.Parse(doRegister.OrderSalePrice) != 
                //    (decimal.Parse(doRegister.BillingAmt_ApproveContract) + decimal.Parse(doRegister.PartialFee) + decimal.Parse(doRegister.BillingAmt_Acceptance)))
                //{
                //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                //    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3059, "");
                //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3059);
                //    return res;                    
                //}

                ////4.1.2	Sale price must equal to product price + installation fee
                //if (decimal.Parse(doRegister.OrderSalePrice) != (decimal.Parse(doRegister.OrderProductPrice) + decimal.Parse(doRegister.OrderInstallFee)))
                //{
                //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                //    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3235, "");
                //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3235);
                //    return res;
                //}

                ////4.1.3	If txtNormalSalePrice > 0 And txtOrderSalePrice = 0 And txtApproveNo1 is empty
                //if (decimal.Parse(doRegister.NormalSalePrice) > 0 && decimal.Parse(doRegister.OrderSalePrice) == 0 && (doRegister.ApproveNo1 == null || doRegister.ApproveNo1 == ""))
                //{
                //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                //    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3065, "");
                //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3065);
                //    return res;
                //}

                ////4.1.4	If txtNormalSalePrice > 0 And txtOrderSalePrice <= 30% of txtNormalSalePrice And txtApproveNo1 is empty
                //decimal thirtyPercent = 0;
                //thirtyPercent = (decimal.Parse(doRegister.NormalSalePrice) * 30) / 100;
                //if (decimal.Parse(doRegister.NormalSalePrice) > 0 && decimal.Parse(doRegister.OrderSalePrice) <= thirtyPercent && (doRegister.ApproveNo1 == null || doRegister.ApproveNo1 == ""))
                //{
                //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                //    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3066, "");
                //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3066);
                //    return res;
                //}

                ////4.1.5	If txtNormalSalePrice > 0 And txtOrderSalePrice >= 1000% of txtNormalSalePrice And txtApproveNo1 is empty
                //decimal oneThousandPercent = 0;
                //oneThousandPercent = (decimal.Parse(doRegister.NormalSalePrice) * 1000) / 100;
                //if (decimal.Parse(doRegister.NormalSalePrice) > 0 && decimal.Parse(doRegister.OrderSalePrice) >= oneThousandPercent && (doRegister.ApproveNo1 == null && doRegister.ApproveNo1 == ""))
                //{
                //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                //    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3066, "");
                //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3066);
                //    return res;
                //}

                ////4.1.2	When normal sale price is not equal to order sale price, Approve no. must registered
                //if ((doRegister.NormalSalePrice != doRegister.OrderSalePrice) && (doRegister.ApproveNo1 == null || doRegister.ApproveNo1 == ""))
                //{
                //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                //    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3060, "");
                //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3060);
                //    return res;
                //}

                decimal summmary = 0;
                CTS062_ScreenParameter session = CTS062_GetImportData();

                #region Product Price

                if (session.DSQuotation.dtTbt_QuotationBasic.ProductPriceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    //if (session.DSQuotation.dtTbt_QuotationBasic.ProductPrice.GetValueOrDefault() == 0
                    //&& decimal.Parse(doRegister.OrderProductPrice) > 0
                    //&& CommonUtil.IsNullOrEmpty(doRegister.ApproveNo1))
                    //{
                    //    //res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    //    //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3322, "");
                    //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3322,
                    //                            null, new string[] { "ApproveNo1" });
                    //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    //    res.ResultData = false;
                    //    return res;
                    //}

                    if (session.DSQuotation.dtTbt_QuotationBasic.ProductPriceUsd > 0
                        && decimal.Parse(doRegister.OrderProductPrice) == 0
                        && CommonUtil.IsNullOrEmpty(doRegister.ApproveNo1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3322,
                                                null, new string[] { "OrderProductPrice" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.ResultData = false;
                        return res;
                    }                    
                }
                else
                {
                    //if (session.DSQuotation.dtTbt_QuotationBasic.ProductPriceUsd.GetValueOrDefault() == 0
                    //    && decimal.Parse(doRegister.OrderProductPrice) > 0
                    //    && CommonUtil.IsNullOrEmpty(doRegister.ApproveNo1))
                    //{
                    //    //res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    //    //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3322, "");
                    //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3322,
                    //                            null, new string[] { "ApproveNo1" });
                    //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    //    res.ResultData = false;
                    //    return res;
                    //}     

                    if (session.DSQuotation.dtTbt_QuotationBasic.ProductPrice > 0
                                && decimal.Parse(doRegister.OrderProductPrice) == 0
                                && CommonUtil.IsNullOrEmpty(doRegister.ApproveNo1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3322,
                                                null, new string[] { "OrderProductPrice" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.ResultData = false;
                        return res;
                    }
                }

                bool skipop = false;
                summmary = 0;
                if (doRegister.OrderProductPriceCurrencyType == doRegister.BillingAmt_ApproveContractCurrencyType)
                    summmary += decimal.Parse(doRegister.BillingAmt_ApproveContract);
                else
                {
                    if (decimal.Parse(doRegister.BillingAmt_ApproveContract) > 0)
                        skipop = true;
                }

                if (doRegister.OrderProductPriceCurrencyType == doRegister.BillingAmt_PartialFeeCurrencyType)
                    summmary += decimal.Parse(doRegister.BillingAmt_PartialFee);
                else
                {
                    if (decimal.Parse(doRegister.BillingAmt_PartialFee) > 0)
                        skipop = true;
                }

                if (doRegister.OrderProductPriceCurrencyType == doRegister.BillingAmt_AcceptanceCurrencyType)
                    summmary += decimal.Parse(doRegister.BillingAmt_Acceptance);
                else
                {
                    if (decimal.Parse(doRegister.BillingAmt_Acceptance) > 0)
                        skipop = true;
                }

                if (skipop == false
                    && decimal.Parse(doRegister.OrderProductPrice) != summmary)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3323,
                                                null, new string[] { "OrderProductPrice" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.ResultData = false;
                    return res;
                }

                if (session.DSQuotation.dtTbt_QuotationBasic.ProductPriceCurrencyType == doRegister.OrderProductPriceCurrencyType)
                {
                    if (session.DSQuotation.dtTbt_QuotationBasic.ProductPriceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        if (session.DSQuotation.dtTbt_QuotationBasic.ProductPriceUsd != decimal.Parse(doRegister.OrderProductPrice)
                            && CommonUtil.IsNullOrEmpty(doRegister.ApproveNo1))
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3324,
                                                null, new string[] { "ApproveNo1" });
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                            res.ResultData = false;
                            return res;
                        }
                    }
                    else
                    {
                        if (session.DSQuotation.dtTbt_QuotationBasic.ProductPrice != decimal.Parse(doRegister.OrderProductPrice)
                            && CommonUtil.IsNullOrEmpty(doRegister.ApproveNo1))
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3324,
                                                null, new string[] { "ApproveNo1" });
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                            res.ResultData = false;
                            return res;
                        }
                    }
                }

                if (decimal.Parse(doRegister.BillingAmt_Acceptance) > 0
                    && CommonUtil.IsNullOrEmpty(doRegister.SalePrice_PaymentMethod_Acceptance))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3327, null, new string[] { "SalePrice_PaymentMethod_Acceptance" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.ResultData = false;
                    return res;
                }



                #endregion
                #region Installation Fee

                if (session.DSQuotation.dtTbt_QuotationBasic.InstallationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    //if (session.DSQuotation.dtTbt_QuotationBasic.InstallationFee.GetValueOrDefault() == 0
                    //    && decimal.Parse(doRegister.OrderInstallFee) > 0
                    //    && CommonUtil.IsNullOrEmpty(doRegister.ApproveNo1))
                    //{
                    //    //res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    //    //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3331, "");
                    //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3329,
                    //        null, new string[] { "ApproveNo1" });
                    //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    //    res.ResultData = false;
                    //    return res;
                    //}

                    if (session.DSQuotation.dtTbt_QuotationBasic.InstallationFeeUsd > 0
                                && decimal.Parse(doRegister.OrderInstallFee) == 0
                                && CommonUtil.IsNullOrEmpty(doRegister.ApproveNo1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3329,
                                                null, new string[] { "OrderInstallFee" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.ResultData = false;
                        return res;
                    }
                }
                else
                {
                    //if (session.DSQuotation.dtTbt_QuotationBasic.InstallationFeeUsd.GetValueOrDefault() == 0
                    //    && decimal.Parse(doRegister.OrderInstallFee) > 0
                    //    && CommonUtil.IsNullOrEmpty(doRegister.ApproveNo1))
                    //{
                    //    //res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    //    //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3322, "");
                    //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3329,
                    //        null, new string[] { "ApproveNo1" });
                    //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    //    res.ResultData = false;
                    //    return res;
                    //}

                    if (session.DSQuotation.dtTbt_QuotationBasic.InstallationFee > 0
                                    && decimal.Parse(doRegister.OrderInstallFee) == 0
                                    && CommonUtil.IsNullOrEmpty(doRegister.ApproveNo1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3329,
                                                null, new string[] { "OrderInstallFee" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.ResultData = false;
                        return res;
                    }
                }

                bool skipoi = false;
                summmary = 0;
                if (doRegister.OrderInstallFeeCurrencyType == doRegister.BillingAmtInstallation_ApproveContractCurrencyType)
                    summmary += decimal.Parse(doRegister.BillingAmtInstallation_ApproveContract);
                else
                {
                    if (decimal.Parse(doRegister.BillingAmtInstallation_ApproveContract) > 0)
                        skipoi = true;
                }

                if (doRegister.OrderInstallFeeCurrencyType == doRegister.BillingAmtInstallation_PartialFeeCurrencyType)
                    summmary += decimal.Parse(doRegister.BillingAmtInstallation_PartialFee);
                else
                {
                    if (decimal.Parse(doRegister.BillingAmtInstallation_PartialFee) > 0)
                        skipoi = true;
                }

                if (doRegister.OrderInstallFeeCurrencyType == doRegister.BillingAmtInstallation_AcceptanceCurrencyType)
                    summmary += decimal.Parse(doRegister.BillingAmtInstallation_Acceptance);
                else
                {
                    if (decimal.Parse(doRegister.BillingAmtInstallation_Acceptance) > 0)
                        skipoi = true;
                }

                if (skipoi == false
                    && decimal.Parse(doRegister.OrderInstallFee) != summmary)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3330,
                                                null, new string[] { "OrderInstallFee" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.ResultData = false;
                    return res;
                }

                if (session.DSQuotation.dtTbt_QuotationBasic.InstallationFeeCurrencyType == doRegister.OrderInstallFeeCurrencyType)
                {
                    if (session.DSQuotation.dtTbt_QuotationBasic.InstallationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        if (session.DSQuotation.dtTbt_QuotationBasic.InstallationFeeUsd != decimal.Parse(doRegister.OrderInstallFee)
                            && CommonUtil.IsNullOrEmpty(doRegister.ApproveNo1))
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3331,
                                null, new string[] { "ApproveNo1" });
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                            res.ResultData = false;
                            return res;
                        }
                    }
                    else
                    {
                        if (session.DSQuotation.dtTbt_QuotationBasic.InstallationFee != decimal.Parse(doRegister.OrderInstallFee)
                            && CommonUtil.IsNullOrEmpty(doRegister.ApproveNo1))
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3331,
                                null, new string[] { "ApproveNo1" });
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                            res.ResultData = false;
                            return res;
                        }
                    }
                }

                if (decimal.Parse(doRegister.BillingAmtInstallation_Acceptance) > 0
                    && CommonUtil.IsNullOrEmpty(doRegister.InstallationFee_PaymentMethod_Acceptance))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3334, null, new string[] { "InstallationFee_PaymentMethod_Acceptance" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.ResultData = false;
                    return res;
                }

                #endregion

                //4.2 Validate expected installation complete date
                //DateTime future = new DateTime(DateTime.Now.Year + 3, DateTime.Now.Month, DateTime.Now.Day);
                DateTime future = DateTime.Today.AddYears(3);
                if (doRegister.ExpectedInstallCompleteDate > future)
	            {
                    //res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3206, "");
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3206);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.ResultData = false;
                    return res;
	            }

                if (doRegister.ExpectedCustAcceptanceDate > future)
                {
                    //res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3208, "");
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3208);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.ResultData = false;
                    return res;
                }

                if (doRegister.DistributedType == DistributeType.C_DISTRIBUTE_TYPE_TARGET && (doRegister.DistributedCode == null || doRegister.DistributedCode == ""))
	            {
                    //res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3062, "");
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3062);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.ResultData = false;
                    return res;
	            }

                if (doRegister.ConnectionFlag.GetValueOrDefault())
                {
                    string longRentalContractCode = "";
                    longRentalContractCode = util.ConvertContractCode(doRegister.ConnectTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                    var contractDat = rentalhandler.GetTbt_RentalContractBasic(longRentalContractCode, null);
                    if (contractDat.Count == 1)
                    {
                        if (contractDat[0].ProductTypeCode != ProductType.C_PROD_TYPE_ONLINE)
                        {
                            //res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                            //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3280, new string[] { doRegister.ConnectTargetCode });
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3280, new string[] { doRegister.ConnectTargetCode });
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                            res.ResultData = false;
                            return res;
                        }
                    }
                    else
                    {
                        //res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3279, new string[] { doRegister.ConnectTargetCode });
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3279, new string[] { doRegister.ConnectTargetCode });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.ResultData = false;
                        return res;
                    }
                }

                if (doRegister.DistributedType == DistributeType.C_DISTRIBUTE_TYPE_TARGET)
                {
                    string longCode = "";
                    try
                    {
                        longCode = util.ConvertContractCode(doRegister.DistributedCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                    }
                    catch (Exception ex)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.AddErrorMessage(ex);
                        res.ResultData = false;
                        //res.ResultData = res.Message;
                        return res;
                    }

                    listSaleBasic = saleContractHandler.GetTbt_SaleBasic(longCode, null, null);

                    if (listSaleBasic != null)
                    {
                        if (listSaleBasic.Count == 0)
                        {
                            //res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                            //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3063, "");
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3063);
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                            res.ResultData = false;
                            return res;    
                        }
                    }
                    else
                    {
                        //res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3063, "");
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3063);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.ResultData = false;
                        return res;
                    }
                }
              
                listEmployee = masterHandler.GetActiveEmployee(doRegister.NegotiationStaffEmpNo1);
                if (listEmployee.Count() == 0)
                {
                    string[] param = { doRegister.NegotiationStaffEmpNo1 };
                    //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0012, param);
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0012, param);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.ResultData = false;
                    return res;
                }

                if (!String.IsNullOrEmpty(doRegister.NegotiationStaffEmpNo2))
                {
                    listEmployee = masterHandler.GetActiveEmployee(doRegister.NegotiationStaffEmpNo2);
                    if (listEmployee.Count() == 0)
                    {
                        string[] param = { doRegister.NegotiationStaffEmpNo2 };
                        //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0012, param);
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0012, param);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.ResultData = false;
                        return res;
                    }
                }

                if (doRegister.NegotiationStaffEmpNo1 == doRegister.NegotiationStaffEmpNo2)
                {
                    string[] param = { doRegister.NegotiationStaffEmpNo2 };
                    //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3273, param);
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3273, param);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.ResultData = false;
                    return res;
                }
                                
                listSequenceApproveNo.Add(doRegister.ApproveNo1);
                listSequenceApproveNo.Add(doRegister.ApproveNo2);
                listSequenceApproveNo.Add(doRegister.ApproveNo3);
                listSequenceApproveNo.Add(doRegister.ApproveNo4);
                listSequenceApproveNo.Add(doRegister.ApproveNo5);

                for (int i = 0; i <= listSequenceApproveNo.Count - 1; i++)
                {
                    if (!string.IsNullOrEmpty(listSequenceApproveNo[i]))
                    {
                        if (i != 0)
                        {
                            for (int j = i - 1; j >= 0; j--)
                            {
                                if (String.IsNullOrEmpty(listSequenceApproveNo[j]))
                                {
                                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                                    //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3009, "");
                                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3009);
                                    res.ResultData = false;
                                    return res;
                                }
                            }
                        }
                    }
                }

                //if (decimal.Parse(doRegister.BillingAmt_Acceptance) > 0 &&
                //   (doRegister.PaymethodCompleteInstallation != PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER && 
                //    doRegister.PaymethodCompleteInstallation != PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER) &&
                //   (doRegister.ApproveNo1 == null || doRegister.ApproveNo1 == ""))
                //{
                //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                //    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3064, "");
                //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3064, null);
                //}

                if (decimal.Parse(doRegister.BillingAmt_Acceptance) > 0
                            && doRegister.SalePrice_PaymentMethod_Acceptance != PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER
                            && doRegister.SalePrice_PaymentMethod_Acceptance != PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER
                            && CommonUtil.IsNullOrEmpty(doRegister.ApproveNo1))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3328, null, new string[] { "ApproveNo1" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.ResultData = false;
                    return res;
                }
                if (decimal.Parse(doRegister.BillingAmtInstallation_Acceptance) > 0
                            && doRegister.InstallationFee_PaymentMethod_Acceptance != PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER
                            && doRegister.InstallationFee_PaymentMethod_Acceptance != PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER
                            && CommonUtil.IsNullOrEmpty(doRegister.ApproveNo1))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3328, null, new string[] { "ApproveNo1" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.ResultData = false;
                    return res;
                }

                if (session.DSQuotation.dtTbt_QuotationBasic.ProductPriceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    if (session.DSQuotation.dtTbt_QuotationBasic.ProductPriceUsd > 0
                        && decimal.Parse(doRegister.OrderProductPrice) == 0)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3337);
                    }
                    
                }
                else
                {
                    if (session.DSQuotation.dtTbt_QuotationBasic.ProductPrice > 0
                        && decimal.Parse(doRegister.OrderProductPrice) == 0)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3337);
                    }
                }

                if (session.DSQuotation.dtTbt_QuotationBasic.InstallationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    if (session.DSQuotation.dtTbt_QuotationBasic.InstallationFeeUsd > 0
                        && decimal.Parse(doRegister.OrderInstallFee) == 0)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3338);
                    }
                    
                }
                else
                {
                    if (session.DSQuotation.dtTbt_QuotationBasic.InstallationFee > 0
                        && decimal.Parse(doRegister.OrderInstallFee) == 0)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3338);
                    }
                }

                if (session.DSQuotation.dtTbt_QuotationBasic.ProductPriceCurrencyType == doRegister.OrderProductPriceCurrencyType)
                {
                    if (session.DSQuotation.dtTbt_QuotationBasic.ProductPriceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        var percentOfNormalProductPrice_30 = session.DSQuotation.dtTbt_QuotationBasic.ProductPriceUsd.GetValueOrDefault() * 0.3M;
                        var percentOfNormalProductPrice_1000 = session.DSQuotation.dtTbt_QuotationBasic.ProductPriceUsd.GetValueOrDefault() * 10M;
                        if ((session.DSQuotation.dtTbt_QuotationBasic.ProductPriceUsd.GetValueOrDefault() > 0)
                            && (decimal.Parse(doRegister.OrderProductPrice) > 0)
                            && ((decimal.Parse(doRegister.OrderProductPrice) <= percentOfNormalProductPrice_30)
                                   || (decimal.Parse(doRegister.OrderProductPrice) >= percentOfNormalProductPrice_1000))
                            && CommonUtil.IsNullOrEmpty(doRegister.ApproveNo1))
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3362, null, null);
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                        }
                    }
                    else
                    {
                        var percentOfNormalProductPrice_30 = session.DSQuotation.dtTbt_QuotationBasic.ProductPrice.GetValueOrDefault() * 0.3M;
                        var percentOfNormalProductPrice_1000 = session.DSQuotation.dtTbt_QuotationBasic.ProductPrice.GetValueOrDefault() * 10M;
                        if ((session.DSQuotation.dtTbt_QuotationBasic.ProductPrice.GetValueOrDefault() > 0)
                            && (decimal.Parse(doRegister.OrderProductPrice) > 0)
                            && ((decimal.Parse(doRegister.OrderProductPrice) <= percentOfNormalProductPrice_30)
                                   || (decimal.Parse(doRegister.OrderProductPrice) >= percentOfNormalProductPrice_1000))
                            && CommonUtil.IsNullOrEmpty(doRegister.ApproveNo1))
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3362, null, null);
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                        }
                    }
                }
                else
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3364, null, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                }

                if (doRegister.OrderProductPriceCurrencyType != doRegister.BillingAmt_ApproveContractCurrencyType)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3365, null, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                }
                if (doRegister.OrderProductPriceCurrencyType != doRegister.BillingAmt_PartialFeeCurrencyType)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3366, null, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                }
                if (doRegister.OrderProductPriceCurrencyType != doRegister.BillingAmt_AcceptanceCurrencyType)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3367, null, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                }


                if (session.DSQuotation.dtTbt_QuotationBasic.InstallationFeeCurrencyType == doRegister.OrderInstallFeeCurrencyType)
                {
                    if (session.DSQuotation.dtTbt_QuotationBasic.InstallationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        var percentOfNormalInstall_30 = session.DSQuotation.dtTbt_QuotationBasic.InstallationFeeUsd.GetValueOrDefault() * 0.3M;
                        var percentOfNormalInstall_1000 = session.DSQuotation.dtTbt_QuotationBasic.InstallationFeeUsd.GetValueOrDefault() * 10.0M;
                        if ((session.DSQuotation.dtTbt_QuotationBasic.InstallationFeeUsd.GetValueOrDefault() > 0)
                            && (decimal.Parse(doRegister.OrderInstallFee) > 0)
                            && ((decimal.Parse(doRegister.OrderInstallFee) <= percentOfNormalInstall_30)
                                   || (decimal.Parse(doRegister.OrderInstallFee) >= percentOfNormalInstall_1000))
                            && CommonUtil.IsNullOrEmpty(doRegister.ApproveNo1))
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3363, null, null);
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                        }
                    }
                    else
                    {
                        var percentOfNormalInstall_30 = session.DSQuotation.dtTbt_QuotationBasic.InstallationFee.GetValueOrDefault() * 0.3M;
                        var percentOfNormalInstall_1000 = session.DSQuotation.dtTbt_QuotationBasic.InstallationFee.GetValueOrDefault() * 10.0M;
                        if ((session.DSQuotation.dtTbt_QuotationBasic.InstallationFee.GetValueOrDefault() > 0)
                            && (decimal.Parse(doRegister.OrderInstallFee) > 0)
                            && ((decimal.Parse(doRegister.OrderInstallFee) <= percentOfNormalInstall_30)
                                   || (decimal.Parse(doRegister.OrderInstallFee) >= percentOfNormalInstall_1000))
                            && CommonUtil.IsNullOrEmpty(doRegister.ApproveNo1))
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3363, null, null);
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                        }
                    }
                }
                else
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3368, null, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                }

                if (doRegister.OrderInstallFeeCurrencyType != doRegister.BillingAmtInstallation_ApproveContractCurrencyType)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3369, null, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                }
                if (doRegister.OrderInstallFeeCurrencyType != doRegister.BillingAmtInstallation_PartialFeeCurrencyType)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3370, null, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                }
                if (doRegister.OrderInstallFeeCurrencyType != doRegister.BillingAmtInstallation_AcceptanceCurrencyType)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3371, null, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                }

                
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                res.ResultData = false;
            }

            return res;
        }

        /// <summary>
        /// Validate retrieved data
        /// </summary>
        /// <param name="doRetrieveData"></param>
        /// <returns></returns>
        public ActionResult ValidateRetrieve_CTS062(CTS062_DORetrieveData doRetrieveData)
        {
            ObjectResultData res = new ObjectResultData();
            if (ModelState.IsValid == false)
            {
                res.ResultData = false;
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                ValidatorUtil.BuildErrorMessage(res, this);
                return Json(res);
            }
            return Json(res);
        }

        /// <summary>
        /// Validate required field
        /// </summary>
        /// <param name="doRegisterData"></param>
        /// <returns></returns>
        public ActionResult ValidateRequireField_CTS062(CTS062_DORegisterData doRegisterData)
        {
            ObjectResultData res = new ObjectResultData();

            string additionError = "";
            List<string> additionControls = new List<string>();
            //decimal tmpAmt = -1;

            //decimal.TryParse(doRegisterData.BillingAmt_CompleteInstallation, out tmpAmt);

            //if ((tmpAmt > 0) && (String.IsNullOrEmpty(doRegisterData.PaymethodCompleteInstallation)))
            //{
            //    if (additionError.Length > 0)
            //    {
            //        additionError += ", ";
            //    }

            //    additionError += CommonUtil.GetLabelFromResource("Contract", "CTS062", "lblCustomerAcceptance");
            //    additionControls.Add("PaymethodCompleteInstallation");
            //}

            if (ModelState.IsValid == false)
            {
                ValidatorUtil.BuildErrorMessage(res, this);

                if (res.IsError)
                {
                    if (additionError.Length > 0)
                    {
                        res.Message.Message.Replace(": ", ": " + additionError);
                        res.Message.Controls = res.Message.Controls.Concat(additionControls.ToArray()).ToArray();
                    }

                    res.ResultData = false;
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    //ValidatorUtil.BuildErrorMessage(res, this);
                    return Json(res);
                }
            }
            else
            {
                if (additionError.Length > 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007
                    , new string[] { additionError }, additionControls.ToArray());

                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.ResultData = false;
                    return Json(res);
                }
            }
            return Json(res);
        }

        #endregion

        #region Session

        /// <summary>
        /// Get import data from screen
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private CTS062_ScreenParameter CTS062_GetImportData(string key = null)
        {
            try
            {
                return GetScreenObject<CTS062_ScreenParameter>(key);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Set import data to screen
        /// </summary>
        /// <param name="import"></param>
        /// <param name="key"></param>
        private void CTS062_SetImportData(CTS062_ScreenParameter import, string key = null)
        {
            try
            {
                UpdateScreenObject(import, key);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// Clear session of screen
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS062_ClearSession()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                UpdateScreenObject(null);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        #endregion
    }
}
