//*********************************
// Create by: Natthavat S.
// Create date: 25/Oct/2011
// Update date: 25/Oct/2011
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
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Models;
using CSI.WindsorHelper;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Presentation.Contract.Models;
using SECOM_AJIS.DataEntity.Quotation;
using System.Transactions;
using SECOM_AJIS.DataEntity.Master;
namespace SECOM_AJIS.Presentation.Contract.Controllers
{
    public partial class ContractController : BaseController
    {
        #region Authority

        /// <summary>
        /// Check user permission and system suspend
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CTS150_Authority(CTS150_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil comUtil = new CommonUtil();

            try
            {
                res = ValidateAuthority_CTS150(res);

                if (res.IsError)
                {
                    return Json(res);
                }
                else
                {
                    //dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    //param.strContractCode = dsTrans.dtCommonSearch.ContractCode;
                    if (param.CommonSearch != null)
                    {
                        if (CommonUtil.IsNullOrEmpty(param.CommonSearch.ContractCode) == false)
                            param.strContractCode = param.CommonSearch.ContractCode;
                    }

                    //     For Dummie
                    //if ((sParam == null) || (string.IsNullOrEmpty(sParam.strContractCode)))
                    //{
                    //    sParam = new CTS150_ScreenParameter();
                    //    sParam.strContractCode = "N0000000099";
                    //}

                    if (string.IsNullOrEmpty(param.strContractCode))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0147);
                        return Json(res);
                    }
                    else
                    {
                        CommonUtil cmmUtil = new CommonUtil();
                        ISaleContractHandler salehandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;

                        bool isExist = false;
                        List<bool?> lst = salehandler.IsContractExist(cmmUtil.ConvertContractCode(param.strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                        if (lst.Count > 0)
                        {
                            isExist = lst[0] != null ? lst[0].Value : false;
                        }
                        if (isExist == false)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0124);
                            return Json(res);
                        }
                    }

                    //sParam = new CTS150_ScreenParameter()
                    //{
                    //    strContractCode = strContractCode
                    //};

                    //SetScreenParameter_CTS150(sParam);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<CTS150_ScreenParameter>("CTS150", param, res);
        }

        //public ActionResult CTS150_Authority(string param, string strContractCode)
        //{
        //    ObjectResultData res = new ObjectResultData();
        //    CommonUtil comUtil = new CommonUtil();
        //    CTS150_ScreenParameter sParam = null;

        //    try
        //    {
        //        // Check Screen Permission

        //        if (CheckIsSuspending(res))
        //        {
        //            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
        //            return Json(res);
        //        }
        //        else if (CheckUserPermission(ScreenID.C_SCREEN_ID_REGISTER_CQ31, FunctionID.C_FUNC_ID_OPERATE) == false)
        //        {
        //            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
        //            return Json(res);
        //        }
        //        else
        //        {
                   
        //        //     For Dummie
        //            //if ((sParam == null) || (string.IsNullOrEmpty(sParam.strContractCode)))
        //            //{
        //            //    sParam = new CTS150_ScreenParameter();
        //            //    sParam.strContractCode = "N0000000099";
        //            //}

        //            if (string.IsNullOrEmpty(strContractCode))
        //            {
        //                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "ContractCode" });
        //                return Json(res);
        //            }

        //            sParam = new CTS150_ScreenParameter()
        //            {
        //                strContractCode = strContractCode
        //            };

        //            //SetScreenParameter_CTS150(sParam);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.AddErrorMessage(ex);
        //        return Json(res);
        //    }


        //    return InitialScreenEnvironment("CTS150", sParam);
        //}

        #endregion

        #region Action

        /// <summary>
        /// Initial screen
        /// </summary>
        /// <returns></returns>
        [Initialize("CTS150")]
        public ActionResult CTS150() // InitialState
        {
            return View();
        }

        /// <summary>
        /// Initial default value to screen
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS150_RetrieveInitialDefaultValue()
        {
            ObjectResultData res = new ObjectResultData();
            CTS150_DefaultValue result = new CTS150_DefaultValue();
            CommonUtil util = new CommonUtil();

            try
            {
                CTS150_ScreenParameter sParam = GetScreenObject<CTS150_ScreenParameter>();
                //CTS150_ScreenParameter sParam = GetScreenObject_CTS150();
                
                //if ((sParam == null) || (string.IsNullOrEmpty(sParam.strContractCode)))
                //{
                //    sParam = new CTS150_ScreenParameter();
                //    sParam.strContractCode = util.ConvertContractCode("N0000000099", CommonUtil.CONVERT_TYPE.TO_SHORT);
                //}

                result.C_ACQUISITION_TYPE_CUST = AcquisitionType.C_ACQUISITION_TYPE_CUST;
                result.C_ACQUISITION_TYPE_INSIDE_COMPANY = AcquisitionType.C_ACQUISITION_TYPE_INSIDE_COMPANY;
                result.C_ACQUISITION_TYPE_SUBCONTRACTOR = AcquisitionType.C_ACQUISITION_TYPE_SUBCONTRACTOR;

                result.C_DISTRIBUTED_TYPE_NOT_DISTRIBUTED = DistributeType.C_DISTRIBUTED_TYPE_NOT_DISTRIBUTED;
                result.C_DISTRIBUTED_TYPE_ORIGIN = DistributeType.C_DISTRIBUTED_TYPE_ORIGIN;

                result.ContractCode = sParam.strContractCode;

                res.ResultData = result;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Initial subcontractor grid schema
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS150_InitialSubcontractorGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Contract\\CTS150_Subcontractor"));
        }

        /// <summary>
        /// Initial instrument grid schema
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS150_InitialInstrumentDetail()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Contract\\CTS150_Instrument"));
        }

        /// <summary>
        /// Retrieve employee name from code when type employee code from screen
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CTS150_RetrieveEmployeeByCode(CTS150_SearchByCode cond)
        {
            ObjectResultData res = new ObjectResultData();
            IEmployeeMasterHandler emphandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
            CTS150_SearchResultByCode result = new CTS150_SearchResultByCode();

            try
            {
                var empList = emphandler.GetActiveEmployee(cond.Code);
                if (empList.Count == 1)
                {
                    result.Code = empList[0].EmpNo;
                    result.Name = empList[0].EmpFullName;

                    res.ResultData = result;
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Retrieve instrument name and maker from code when click [Search instrument] button
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CTS150_RetrieveInstrumentByCode(CTS150_SearchByCode cond)
        {
            ObjectResultData res = new ObjectResultData();
            IInstrumentMasterHandler instruhandler = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
            CTS150_SearchResultByCode result = new CTS150_SearchResultByCode();

            try
            {
                var instrulist = instruhandler.GetInstrumentDataForSearch(new doInstrumentSearchCondition()
                {
                    InstrumentCode = cond.Code,
                });

                if (instrulist.Count == 1)
                {
                    result.Code = instrulist[0].InstrumentCode;
                    result.Name = instrulist[0].InstrumentName;
                    result.Maker = instrulist[0].Maker;

                    res.ResultData = result;
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        
        /// <summary>
        /// Retrieve contract data to display when click [Retrieve] button in ‘Specify sale occurrence’ section
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CTS150_RetrieveAllContractInformation(CTS150_Condition cond)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil util = new CommonUtil();

            try
            {
                res = RetrieveContractDetail_CTS150(util.ConvertContractCode(cond.strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), cond.strOccurrenceCode);

                //if (String.IsNullOrEmpty(cond.strContractCode) || String.IsNullOrEmpty(cond.strOccurrenceCode))
                //{
                //    res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0007, new string[] { cond.strContractCode + ", " + cond.strOccurrenceCode }, null);
                //    return Json(res);
                //}
                //else
                //{
                //    res = RetrieveContractDetail_CTS150(util.ConvertContractCode(cond.strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), cond.strOccurrenceCode);
                    
                //}
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Retrieve subcontractor data to display and initial when retrieve contract data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CTS150_RetrieveSubcontractorData(CTS150_Condition cond)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil util = new CommonUtil();

            try
            {
                res = RetrieveSubcontractorGridData_CTS150(util.ConvertContractCode(cond.strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), cond.strOccurrenceCode);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Retrieve instrument data to display and initial when retrieve contract data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CTS150_RetrieveInstrumentData(CTS150_Condition cond)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil util = new CommonUtil();

            try
            {
                res = RetrieveInstrumentGridData_CTS150(util.ConvertContractCode(cond.strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), cond.strOccurrenceCode);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Validate business before register when click [Register] button 
        /// </summary>
        /// <param name="dat"></param>
        /// <returns></returns>
        public ActionResult CTS150_ValidateBusiness(CTS150_CQ31Change dat)
        {
            ObjectResultData result = new ObjectResultData();

            try
            {
                result = ValidateAuthority_CTS150(result);
                if (result.IsError)
                {
                    return Json(result);
                }

                result.ResultData = ValidateBusiness_CS150(dat);
            }
            catch (Exception ex)
            {
                result.AddErrorMessage(ex);
            }

            return Json(result);
        }

        /// <summary>
        /// Proceed maintain sale contract data after passing validate business
        /// </summary>
        /// <param name="dat"></param>
        /// <returns></returns>
        public ActionResult CTS150_RegisterCQ31(CTS150_CQ31Change dat)
        {
            ObjectResultData result = new ObjectResultData();
            ISaleContractHandler salehandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
            CTS150_ValidateResult validateRes = new CTS150_ValidateResult();

            try
            {
                result = ValidateAuthority_CTS150(result);
                if (result.IsError)
                {
                    return Json(result);
                }

                validateRes = ValidateBusiness_CS150(dat);

                if (validateRes.IsValid)
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        dsSaleContractData destData = GetUpdateContractData_CTS150(dat);
                        salehandler.RegisterCQ31(destData);

                        ICommonContractHandler commonContractHandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
                        commonContractHandler.UpdateOperationOffice(destData.dtTbt_SaleBasic[0].ContractCode, destData.dtTbt_SaleBasic[0].OperationOfficeCode);

                        scope.Complete();

                        validateRes.IsValid = true;
                    }
                }
            }
            catch (Exception ex)
            {
                validateRes.IsValid = false;
                result.AddErrorMessage(ex);
            }

            result.ResultData = validateRes;

            return Json(result);
        }

        /// <summary>
        /// Add new instrument to list when click [Add] button
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CTS150_AddInstrument(CTS150_SearchByCode cond)
        {
            ObjectResultData res = new ObjectResultData();
            
            try
            {
                IInstrumentMasterHandler instruhandler = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                List<doInstrumentData> instrulist = instruhandler.GetInstrumentDataForSearch(new doInstrumentSearchCondition()
                {
                    InstrumentCode = cond.Code,
                });

                if (instrulist.Count == 1)
                {
                    if (instrulist[0].InstrumentTypeCode != InstrumentType.C_INST_TYPE_GENERAL)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0014, new string[] { cond.Code });
                        return Json(res);
                    }
                    if (instrulist[0].ExpansionTypeCode != ExpansionType.C_EXPANSION_TYPE_PARENT)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0015, new string[] { cond.Code });
                        return Json(res);
                    }
                    if (instrulist[0].SaleFlag != true)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0016, new string[] { cond.Code });
                        return Json(res);
                    }
                    if (instrulist[0].LineUpTypeCode == LineUpType.C_LINE_UP_TYPE_STOP_SALE
                        || instrulist[0].LineUpTypeCode == LineUpType.C_LINE_UP_TYPE_LOGICAL_DELETE)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0086, new string[] { cond.Code });
                        return Json(res);
                    }
                }

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        #endregion

        #region Method

        /// <summary>
        /// Get updated contract object from screen data
        /// </summary>
        /// <param name="dat"></param>
        /// <returns></returns>
        private dsSaleContractData GetUpdateContractData_CTS150(CTS150_CQ31Change dat)
        {
            dsSaleContractData result = null;
            CommonUtil util = new CommonUtil();
            ISaleContractHandler salehandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
            IInstrumentMasterHandler insthandler = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;

            result = salehandler.GetEntireContract(util.ConvertContractCode(dat.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), dat.OCCCode);

            // Update
            result.dtTbt_SaleBasic[0].AcquisitionTypeCode = dat.AcquisitionType;
            result.dtTbt_SaleBasic[0].ApproveNo1 = dat.ContractApproveNo1;
            result.dtTbt_SaleBasic[0].ApproveNo2 = dat.ContractApproveNo2;
            result.dtTbt_SaleBasic[0].ApproveNo3 = dat.ContractApproveNo3;
            result.dtTbt_SaleBasic[0].ApproveNo4 = dat.ContractApproveNo4;
            result.dtTbt_SaleBasic[0].ApproveNo5 = dat.ContractApproveNo5;
            //result.dtTbt_SaleBasic[0].BidGuaranteeAmount1 = dat.BidGuaranteeAmount1;
            //result.dtTbt_SaleBasic[0].BidGuaranteeAmount2 = dat.BidGuaranteeAmount2;
            result.dtTbt_SaleBasic[0].BidGuaranteeReturnDate1 = dat.BidGuaranteeReturnDate1;
            result.dtTbt_SaleBasic[0].BidGuaranteeReturnDate2 = dat.BidGuaranteeReturnDate2;
            result.dtTbt_SaleBasic[0].ContractOfficeCode = dat.ContractOfficeCode;
            result.dtTbt_SaleBasic[0].InstallCompleteDate = dat.ChangeCompleteInstallDate;
            //result.dtTbt_SaleBasic[0].NormalInstallFee = dat.ChangeNormalInstallFee; //Comment by Jutarat A. on 21022013 (Map by ChangeType)
            result.dtTbt_SaleBasic[0].InstallationCompleteEmpNo = dat.CompleteRegistrantCode;
            result.dtTbt_SaleBasic[0].CustAcceptanceDate = dat.CustomerAcceptDate;
            result.dtTbt_SaleBasic[0].DeliveryDocReceiveDate = dat.DeliveryDocRecieveDate;
            result.dtTbt_SaleBasic[0].DistributedOriginCode = util.ConvertContractCode(dat.DistributedOriginCode, CommonUtil.CONVERT_TYPE.TO_LONG);
            result.dtTbt_SaleBasic[0].DistributedInstallTypeCode = dat.DistributedType;
            result.dtTbt_SaleBasic[0].DocAuditResult = dat.DocAuditResult;
            result.dtTbt_SaleBasic[0].DocReceiveDate = dat.DocRecieveDate;
            result.dtTbt_SaleBasic[0].ExpectedInstallCompleteDate = dat.ExpectedCompleteInstallDate;
            result.dtTbt_SaleBasic[0].ExpectedCustAcceptanceDate = dat.ExpectedCustomerAcceptDate;
            result.dtTbt_SaleBasic[0].IEInchargeEmpNo = dat.IEInCargeCode;
            result.dtTbt_SaleBasic[0].InstrumentStockOutDate = dat.InstrumentStockOutDate;
            result.dtTbt_SaleBasic[0].IntroducerCode = dat.IntroducerCode;
            result.dtTbt_SaleBasic[0].NegotiationStaffEmpNo1 = dat.NegotiationStaffCode;
            result.dtTbt_SaleBasic[0].NewBldMgmtCost = dat.NewBuildingMgmtCost;
            result.dtTbt_SaleBasic[0].NewBldMgmtFlag = (dat.NewBuildingMgmtType == "1") ? true : false;
            result.dtTbt_SaleBasic[0].BuildingTypeCode = dat.NewOldBuilding;

            string strOnlineContractCode = String.IsNullOrEmpty(dat.OnlineContractCode) == false? dat.OnlineContractCode.ToUpper() : dat.OnlineContractCode; //Add by Jutarat A. on 22082012
            result.dtTbt_SaleBasic[0].ConnectTargetCode = util.ConvertContractCode(strOnlineContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

            result.dtTbt_SaleBasic[0].OperationOfficeCode = dat.OperationOfficeCode;
            //result.dtTbt_SaleBasic[0].OrderInstallFee = dat.OrderInstallFee; //Comment by Jutarat A. on 21022013 (Map by ChangeType)
            result.dtTbt_SaleBasic[0].SaleProcessManageStatus = dat.ProcessMgmtStatus;
            result.dtTbt_SaleBasic[0].MotivationTypeCode = dat.PurchaseReasonType;
            result.dtTbt_SaleBasic[0].SalesOfficeCode = dat.SalesOfficeCode;
            result.dtTbt_SaleBasic[0].SalesType = dat.SaleType;
            result.dtTbt_SaleBasic[0].SubcontractInstallCompleteDate = dat.SubcontractCompleteInstallDate;
            result.dtTbt_SaleBasic[0].WarranteeFrom = dat.WaranteePeriodFrom;
            result.dtTbt_SaleBasic[0].WarranteeTo = dat.WaranteePeriodTo;

            //Comment by Jutarat A. on 21022013 (Map by ChangeType)
            //result.dtTbt_SaleBasic[0].OrderProductPrice = dat.BillingProductPrice;
            //result.dtTbt_SaleBasic[0].NormalProductPrice = dat.NormalProductPrice;
            //result.dtTbt_SaleBasic[0].OrderInstallFee = dat.OrderInstallFee;
            //result.dtTbt_SaleBasic[0].NormalInstallFee = dat.NormalInstallFee;
            //End Comment
             
            result.dtTbt_SaleBasic[0].SalesmanEmpNo1 = dat.SalesmanCode[0];
            result.dtTbt_SaleBasic[0].SalesmanEmpNo2 = dat.SalesmanCode[1];
            result.dtTbt_SaleBasic[0].SalesmanEmpNo3 = dat.SalesmanCode[2];
            result.dtTbt_SaleBasic[0].SalesmanEmpNo4 = dat.SalesmanCode[3];
            result.dtTbt_SaleBasic[0].SalesmanEmpNo5 = dat.SalesmanCode[4];
            result.dtTbt_SaleBasic[0].SalesmanEmpNo6 = dat.SalesmanCode[5];
            result.dtTbt_SaleBasic[0].SalesmanEmpNo7 = dat.SalesmanCode[6];
            result.dtTbt_SaleBasic[0].SalesmanEmpNo8 = dat.SalesmanCode[7];
            result.dtTbt_SaleBasic[0].SalesmanEmpNo9 = dat.SalesmanCode[8];
            result.dtTbt_SaleBasic[0].SalesmanEmpNo10 = dat.SalesmanCode[9];

            result.dtTbt_SaleBasic[0].QuotationNo = dat.QuotationNo;

            #region Bid Guarantee Amount 1

            result.dtTbt_SaleBasic[0].BidGuaranteeAmount1CurrencyType = dat.BidGuaranteeAmount1CurrencyType;
            if (result.dtTbt_SaleBasic[0].BidGuaranteeAmount1CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
            {
                result.dtTbt_SaleBasic[0].BidGuaranteeAmount1 = null;
                result.dtTbt_SaleBasic[0].BidGuaranteeAmount1Usd = dat.BidGuaranteeAmount1;
            }
            else
            {
                result.dtTbt_SaleBasic[0].BidGuaranteeAmount1 = dat.BidGuaranteeAmount1;
                result.dtTbt_SaleBasic[0].BidGuaranteeAmount1Usd = null;
            }

            #endregion
            #region Bid Guarantee Amount 2

            result.dtTbt_SaleBasic[0].BidGuaranteeAmount2CurrencyType = dat.BidGuaranteeAmount2CurrencyType;
            if (result.dtTbt_SaleBasic[0].BidGuaranteeAmount2CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
            {
                result.dtTbt_SaleBasic[0].BidGuaranteeAmount2 = null;
                result.dtTbt_SaleBasic[0].BidGuaranteeAmount2Usd = dat.BidGuaranteeAmount2;
            }
            else
            {
                result.dtTbt_SaleBasic[0].BidGuaranteeAmount2 = dat.BidGuaranteeAmount2;
                result.dtTbt_SaleBasic[0].BidGuaranteeAmount2Usd = null;
            }

            #endregion

            result.dtTbt_SaleBasic[0].PaymentDateIncentive = dat.PaymentDateIncentive;

            //Add by Jutarat A. on 21022013 (Map by ChangeType)
            if ((result.dtTbt_SaleBasic[0].ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_NEW_SALE)
                || (result.dtTbt_SaleBasic[0].ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_ADD_SALE)
                || (result.dtTbt_SaleBasic[0].ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_CANCEL))
            {
                #region Billing Product Price

                result.dtTbt_SaleBasic[0].OrderProductPriceCurrencyType = dat.BillingProductPriceCurrencyType;
                if (result.dtTbt_SaleBasic[0].OrderProductPriceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    result.dtTbt_SaleBasic[0].OrderProductPrice = null;
                    result.dtTbt_SaleBasic[0].OrderProductPriceUsd = dat.BillingProductPrice; 
                }
                else
                {
                    result.dtTbt_SaleBasic[0].OrderProductPrice = dat.BillingProductPrice;
                    result.dtTbt_SaleBasic[0].OrderProductPriceUsd = null;
                }

                #endregion
                #region Normal Product Price

                result.dtTbt_SaleBasic[0].NormalProductPriceCurrencyType = dat.NormalProductPriceCurrencyType;
                if (result.dtTbt_SaleBasic[0].NormalProductPriceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    result.dtTbt_SaleBasic[0].NormalProductPrice = null;
                    result.dtTbt_SaleBasic[0].NormalProductPriceUsd = dat.NormalProductPrice; 
                }
                else
                {
                    result.dtTbt_SaleBasic[0].NormalProductPrice = dat.NormalProductPrice;
                    result.dtTbt_SaleBasic[0].NormalProductPriceUsd = null;
                }

                #endregion
                #region Order Install Fee

                result.dtTbt_SaleBasic[0].OrderInstallFeeCurrencyType = dat.BillingInstallFeeCurrencyType;
                if (result.dtTbt_SaleBasic[0].OrderInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    result.dtTbt_SaleBasic[0].OrderInstallFee = null;
                    result.dtTbt_SaleBasic[0].OrderInstallFeeUsd = dat.BillingInstallFee; 
                }
                else
                {
                    result.dtTbt_SaleBasic[0].OrderInstallFee = dat.BillingInstallFee;
                    result.dtTbt_SaleBasic[0].OrderInstallFeeUsd = null;
                }

                #endregion
                #region Normal Install Fee

                result.dtTbt_SaleBasic[0].NormalInstallFeeCurrencyType = dat.NormalInstallFeeCurrencyType;
                if (result.dtTbt_SaleBasic[0].NormalInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    result.dtTbt_SaleBasic[0].NormalInstallFee = null;
                    result.dtTbt_SaleBasic[0].NormalInstallFeeUsd = dat.NormalInstallFee; 
                }
                else
                {
                    result.dtTbt_SaleBasic[0].NormalInstallFee = dat.NormalInstallFee;
                    result.dtTbt_SaleBasic[0].NormalInstallFeeUsd = null;
                }

                #endregion
                
                //result.dtTbt_SaleBasic[0].OrderSalePrice = (dat.BillingProductPrice ?? 0) + (dat.BillingInstallFee ?? 0);
                //result.dtTbt_SaleBasic[0].NormalSalePrice = (dat.NormalProductPrice ?? 0) + (dat.NormalInstallFee ?? 0);
            }
            else if ((result.dtTbt_SaleBasic[0].ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_EXCHANGE_INSTR)
                    || (result.dtTbt_SaleBasic[0].ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_MOVE_INSTR)
                    || (result.dtTbt_SaleBasic[0].ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_REMOVE_INSTR_ALL)
                    || (result.dtTbt_SaleBasic[0].ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_REMOVE_INSTR_PARTIAL)
                    || (result.dtTbt_SaleBasic[0].ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_CHANGE_WIRING)) //Add by Jutarat A. on 21052013
            {
                #region Normal Install Fee

                result.dtTbt_SaleBasic[0].NormalInstallFeeCurrencyType = dat.ChangeNormalInstallFeeCurrencyType;
                if (result.dtTbt_SaleBasic[0].NormalInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    result.dtTbt_SaleBasic[0].NormalInstallFee = null;
                    result.dtTbt_SaleBasic[0].NormalInstallFeeUsd = dat.ChangeNormalInstallFee;
                }
                else
                {
                    result.dtTbt_SaleBasic[0].NormalInstallFee = dat.ChangeNormalInstallFee;
                    result.dtTbt_SaleBasic[0].NormalInstallFeeUsd = null;
                }

                #endregion
                #region Order Install Fee

                result.dtTbt_SaleBasic[0].OrderInstallFeeCurrencyType = dat.OrderInstallFeeCurrencyType;
                if (result.dtTbt_SaleBasic[0].OrderInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    result.dtTbt_SaleBasic[0].OrderInstallFee = null;
                    result.dtTbt_SaleBasic[0].OrderInstallFeeUsd = dat.OrderInstallFee; 
                }
                else
                {
                    result.dtTbt_SaleBasic[0].OrderInstallFee = dat.OrderInstallFee;
                    result.dtTbt_SaleBasic[0].OrderInstallFeeUsd = null;
                }

                #endregion
                #region Install Fee Paid by SECOM

                result.dtTbt_SaleBasic[0].InstallFeePaidBySECOMCurrencyType = dat.SecomPaymentCurrencyType;
                if (result.dtTbt_SaleBasic[0].InstallFeePaidBySECOMCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    result.dtTbt_SaleBasic[0].InstallFeePaidBySECOM = null;
                    result.dtTbt_SaleBasic[0].InstallFeePaidBySECOMUsd = dat.SecomPayment;
                }
                else
                {
                    result.dtTbt_SaleBasic[0].InstallFeePaidBySECOM = dat.SecomPayment;
                    result.dtTbt_SaleBasic[0].InstallFeePaidBySECOMUsd = null;
                }

                #endregion
                #region Install Fee Revenue by SECOM

                result.dtTbt_SaleBasic[0].InstallFeeRevenueBySECOMCurrencyType = dat.SecomRevenueCurrencyType;
                if (result.dtTbt_SaleBasic[0].InstallFeeRevenueBySECOMCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    result.dtTbt_SaleBasic[0].InstallFeeRevenueBySECOM = null;
                    result.dtTbt_SaleBasic[0].InstallFeeRevenueBySECOMUsd = dat.SecomRevenue;
                }
                else
                {
                    result.dtTbt_SaleBasic[0].InstallFeeRevenueBySECOM = dat.SecomRevenue;
                    result.dtTbt_SaleBasic[0].InstallFeeRevenueBySECOMUsd = null;
                }

                #endregion
            }
            //End Add

            List<tbt_SaleInstrumentDetails> updateList = new List<tbt_SaleInstrumentDetails>();

            if (dat.InstrumentDetail != null)
            {
                foreach (var item in dat.InstrumentDetail)
                {
                    var destLst = insthandler.GetInstrumentDataForSearch(new doInstrumentSearchCondition()
                    {
                        InstrumentCode = item.Code,
                    });

                    if (destLst.Count == 1)
                    {
                        tbt_SaleInstrumentDetails tmpItem = new tbt_SaleInstrumentDetails()
                        {
                            ContractCode = util.ConvertContractCode(dat.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG),
                            CreateBy = destLst[0].CreateBy,
                            CreateDate = destLst[0].CreateDate,
                            InstrumentCode = item.Code,
                            InstrumentQty = item.Total,
                            InstrumentTypeCode = destLst[0].InstrumentTypeCode,
                            OCC = dat.OCCCode,
                            UpdateBy = destLst[0].UpdateBy,
                            UpdateDate = destLst[0].UpdateDate
                        };

                        updateList.Add(tmpItem);
                    }
                }
            }

            result.dtTbt_SaleInstrumentDetails.Clear();
            result.dtTbt_SaleInstrumentDetails.AddRange(updateList);

            return result;
        }

        /// <summary>
        /// Validate require field and business
        /// </summary>
        /// <param name="dat"></param>
        /// <returns></returns>
        private CTS150_ValidateResult ValidateBusiness_CS150(CTS150_CQ31Change dat)
        {
            CTS150_ValidateResult result = new CTS150_ValidateResult();
            CommonUtil util = new CommonUtil();
            IEmployeeMasterHandler emphandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
            ISaleContractHandler salehandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
            IInstrumentMasterHandler insthandler = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
            IRentralContractHandler rentalhandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;

            result.IsValid = true;

            //Comment by Jutarat A. on 26022013 (Move)
            //// Validate Required Field
            //if (String.IsNullOrEmpty(dat.SalesmanCode[0]))
            //{
            //    result.InvalidMessage = MessageUtil.GetMessage("Common", MessageUtil.MessageList.MSG0007, new string[] { CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, ScreenID.C_SCREEN_ID_REGISTER_CQ31, "lblSalesman") + " 1" }).Message;
            //    result.InvalidControl = new string[] { "SalesmanCode1" };
            //    result.IsValid = false;
            //    return result;
            //}

            //if (!emphandler.CheckExistActiveEmployee(dat.SalesmanCode[0]))
            //{
            //    result.InvalidMessage = MessageUtil.GetMessage("Common", MessageUtil.MessageList.MSG0095, new string[] { dat.SalesmanCode[0] }).Message;
            //    result.InvalidControl = new string[] { "SalesmanCode1" };
            //    result.IsValid = false;
            //    return result;
            //}
            //End Comment

            // Validate Business
            if ((dat.ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_NEW_SALE)
                || (dat.ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_ADD_SALE)
                || (dat.ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_CANCEL))
            {
                //Add by Jutarat A. on 26022013 (Move)
                // Validate Required Field
                if (String.IsNullOrEmpty(dat.SalesmanCode[0]))
                {
                    result.InvalidMessage = MessageUtil.GetMessage("Common", MessageUtil.MessageList.MSG0007, new string[] { CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, ScreenID.C_SCREEN_ID_REGISTER_CQ31, "lblSalesman") + " 1" }).Message;
                    result.InvalidControl = new string[] { "SalesmanCode1" };
                    result.IsValid = false;
                    return result;
                }

                if (!emphandler.CheckExistActiveEmployee(dat.SalesmanCode[0]))
                {
                    result.InvalidMessage = MessageUtil.GetMessage("Common", MessageUtil.MessageList.MSG0095, new string[] { dat.SalesmanCode[0] }).Message;
                    result.InvalidControl = new string[] { "SalesmanCode1" };
                    result.IsValid = false;
                    return result;
                }
                //End Add

                if ((dat.DistributedType == DistributeType.C_DISTRIBUTE_TYPE_TARGET) && (String.IsNullOrEmpty(dat.DistributedOriginCode)))
                {
                    result.InvalidMessage = MessageUtil.GetMessage("Contract", MessageUtil.MessageList.MSG3062).Message;
                    result.InvalidControl = new string[] { "DistributedOriginCode" };
                    result.IsValid = false;
                    return result;
                }
                else if (dat.DistributedType == DistributeType.C_DISTRIBUTE_TYPE_TARGET)
                {
                    var distList = salehandler.GetTbt_SaleBasic(util.ConvertContractCode(dat.DistributedOriginCode, CommonUtil.CONVERT_TYPE.TO_LONG), null, true);
                    if (distList.Count != 1)
                    {
                        result.InvalidMessage = MessageUtil.GetMessage("Contract", MessageUtil.MessageList.MSG3063, dat.DistributedOriginCode).Message;
                        result.InvalidControl = new string[] { "DistributedOriginCode" };
                        result.IsValid = false;
                        return result;
                    }
                }

                if (dat.PaymentDateIncentive != null)
                {
                    DateTime currentDate = DateTime.Now;
                    currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day);

                    if (dat.PaymentDateIncentive.Value.CompareTo(currentDate) > 0)
                    {
                        result.InvalidMessage = MessageUtil.GetMessage("Contract", MessageUtil.MessageList.MSG3350).Message;
                        result.InvalidControl = new string[] { "PaymentDateIncentive" };
                        result.IsValid = false;
                        return result;
                    }
                }
            }

            if (dat.CustomerAcceptDate.HasValue && (dat.CustomerAcceptDate.GetValueOrDefault() > DateTime.Now))
            {
                var distList = salehandler.GetTbt_SaleBasic(util.ConvertContractCode(dat.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), dat.OCCCode, false);
                if (distList.Count != 1)
                {
                    result.InvalidMessage = MessageUtil.GetMessage("Common", MessageUtil.MessageList.MSG0009, new string[] { CommonUtil.GetLabelFromResource("Contract", ScreenID.C_SCREEN_ID_REGISTER_CQ31, "lblCustAccept") }).Message;
                    result.InvalidControl = new string[] { "CustomerAcceptDate" };
                    result.IsValid = false;
                    return result;
                }
            }

            //Add by jutarat A. on 20082012
            //4.3	Validate Connection contract code
            if (String.IsNullOrEmpty(dat.OnlineContractCode) == false)
            {
                //4.3.1
                bool isValid = true;
                char[] arrCode = dat.OnlineContractCode.ToUpper().ToCharArray();
                if (arrCode.Length >= 1)
                {
                    if (arrCode[0] == 'Q')
                        isValid = false;
                }

                if (arrCode.Length >= 2)
                {
                    if ((arrCode[0] == 'M' && arrCode[1] == 'A') || (arrCode[0] == 'S' && arrCode[1] == 'G'))
                        isValid = false;
                }

                if (isValid == false)
                {
                    result.InvalidMessage = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3263, null).Message;
                    result.InvalidControl = new string[] { "OnlineContractCode" };
                    result.IsValid = false;
                    return result;
                }


                //4.3.2	
                string strConnectContractCodeLong = util.ConvertContractCode(dat.OnlineContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                List<tbt_RentalContractBasic> dtTbt_RentalContractBasic = rentalhandler.GetTbt_RentalContractBasic(strConnectContractCodeLong, null);
                if (dtTbt_RentalContractBasic == null || dtTbt_RentalContractBasic.Count < 1)
                {
                    result.InvalidMessage = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0118, new string[] { dat.OnlineContractCode }).Message;
                    result.InvalidControl = new string[] { "OnlineContractCode" };
                    result.IsValid = false;
                    return result;
                }
            }
            //End Add

            bool isSequent = true;
            int itemCnt = 0;
            foreach (string salemanCode in dat.SalesmanCode)
            {
                itemCnt++;

                if (itemCnt > 1) // except first row cause of already check above.
                {
                    if (!isSequent)
                    {
                        // this must be null or empty only !
                        if (!String.IsNullOrEmpty(salemanCode))
                        {
                            result.InvalidMessage = MessageUtil.GetMessage("Contract", MessageUtil.MessageList.MSG3164).Message;
                            result.InvalidControl = new string[] { "SalesmanCode" + itemCnt };
                            result.IsValid = false;
                            return result;
                        }
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(salemanCode))
                        {
                            // found empty row, from now on all row must be empty !
                            isSequent = false;
                        }
                        else
                        {
                            if (!emphandler.CheckExistActiveEmployee(salemanCode))
                            {
                                result.InvalidMessage = MessageUtil.GetMessage("Common", MessageUtil.MessageList.MSG0095, new string[] { salemanCode }).Message;
                                result.InvalidControl = new string[] { "SalesmanCode" + itemCnt };
                                result.IsValid = false;
                                return result;
                            }
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(dat.IEInCargeCode))
            {
                if (!emphandler.CheckExistActiveEmployee(dat.IEInCargeCode))
                {
                    result.InvalidMessage = MessageUtil.GetMessage("Common", MessageUtil.MessageList.MSG0095, new string[] { CommonUtil.GetLabelFromResource("Contract", ScreenID.C_SCREEN_ID_REGISTER_CQ31, "lblIEInCharge") }).Message;
                    result.InvalidControl = new string[] { "IEInChargeCode" };
                    result.IsValid = false;
                    return result;
                }
            }

            if (CommonUtil.IsNullOrEmpty(dat.NegotiationStaffCode) == false)
            {
                if (emphandler.CheckExistActiveEmployee(dat.NegotiationStaffCode) == false)
                {
                    result.InvalidMessage = MessageUtil.GetMessage("Common", MessageUtil.MessageList.MSG0095, new string[] { dat.NegotiationStaffCode }).Message;
                    result.InvalidControl = new string[] { "NegotiationStaffCode" };
                    result.IsValid = false;
                    return result;
                }
            }
            if (CommonUtil.IsNullOrEmpty(dat.CompleteRegistrantCode) == false)
            {
                if (emphandler.CheckExistActiveEmployee(dat.CompleteRegistrantCode) == false)
                {
                    result.InvalidMessage = MessageUtil.GetMessage("Common", MessageUtil.MessageList.MSG0095, new string[] { dat.CompleteRegistrantCode }).Message;
                    result.InvalidControl = new string[] { "CompleteRegistrantCode" };
                    result.IsValid = false;
                    return result;
                }
            }

            if (dat.CustomerAcceptDate != null
                && dat.WaranteePeriodFrom != null
               )
            {
                if (dat.WaranteePeriodFrom.GetValueOrDefault() < dat.CustomerAcceptDate.GetValueOrDefault())
                {
                    result.InvalidMessage = MessageUtil.GetMessage("Contract", MessageUtil.MessageList.MSG3165, null).Message;
                    result.InvalidControl = new string[] { "WaranteePeriodFrom" };
                    result.IsValid = false;
                    return result;
                }
            }

            if (dat.WaranteePeriodFrom != null
                && dat.WaranteePeriodTo != null
               )
            {
                if (dat.WaranteePeriodTo.GetValueOrDefault() <= dat.WaranteePeriodFrom.GetValueOrDefault())
                {
                    result.InvalidMessage = MessageUtil.GetMessage("Contract", MessageUtil.MessageList.MSG3166, null).Message;
                    result.InvalidControl = new string[] { "WaranteePeriodFrom", "WaranteePeriodTo" };
                    result.IsValid = false;
                    return result;
                }
            }

            if (dat.InstrumentDetail != null)
            {
                foreach (CTS150_InstrumentDetail instItem in dat.InstrumentDetail)
                {
                    var res = insthandler.GetInstrumentDataForSearch(new doInstrumentSearchCondition()
                    {
                        InstrumentCode = instItem.Code
                    });

                    if (res.Count != 1)
                    {
                        result.InvalidMessage = MessageUtil.GetMessage("Common", MessageUtil.MessageList.MSG0082, new string[] { instItem.Code }).Message;
                        //result.InvalidControl = new string[] { "WaranteePeriodFrom", "WaranteePeriodTo" };
                        result.IsValid = false;
                        return result;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Retrieve contract data to display on screen
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="occCode"></param>
        /// <returns></returns>
        private ObjectResultData RetrieveContractDetail_CTS150 (string contractCode, string occCode)
        {
            ObjectResultData result = new ObjectResultData();
            ISaleContractHandler salehandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
            ISiteMasterHandler sitehandler = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;
            ICustomerMasterHandler custhandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
            IEmployeeMasterHandler emphandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
            IRentralContractHandler contracthandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;

            dsSaleContractData contData = salehandler.GetEntireContract(contractCode, occCode);
            dtTbt_RentalMaintenanceDetailsForView maintenData = null;
            dtTbt_RentalSecurityBasicForView securData = null;
            doCustomer custData = null;
            dtSiteData siteData = null;
            List<tbm_Employee> empData = new List<tbm_Employee>();

            if ((contData != null) && (contData.dtTbt_SaleBasic != null) && (contData.dtTbt_SaleBasic.Count == 1))
            {
                var custList = custhandler.GetCustomer(contData.dtTbt_SaleBasic[0].PurchaserCustCode);
                if (custList.Count == 1)
                {
                    custData = custList[0];
                    var siteList = sitehandler.GetSiteDataForSearch(new doSiteSearchCondition()
                    {
                        SiteCode = contData.dtTbt_SaleBasic[0].SiteCode
                    });

                    if (siteList.Count == 1)
                    {
                        siteData = siteList[0];
                    }
                }

                var securList = contracthandler.GetTbt_RentalSecurityBasicForView(contractCode, occCode);
                if (securList.Count == 1)
                {
                    securData = securList[0];
                }

                var maList = contracthandler.GetTbt_RentalMaintenanceDetailsForView(contractCode, occCode);
                if (maList.Count == 1)
                {
                    maintenData = maList[0];
                }

                //if ((custData == null) || (siteData == null)
                //    || (!(contData.dtTbt_SaleBasic.MaintenanceContractFlag.GetValueOrDefault() && ((securData != null) && (maintenData != null))))
                //    /* || (contData.dtTbt_SaleInstrumentDetails == null)*/)
                //{
                //    result.AddErrorMessage("Common", MessageUtil.MessageList.MSG0106, new string[] { occCode }, new string[] { "OccurrenceCode" });
                //}
                //else
                {
                    if (maintenData == null)
                        maintenData = new dtTbt_RentalMaintenanceDetailsForView();

                    if (securData == null)
                        securData = new dtTbt_RentalSecurityBasicForView();

                    if (siteData == null)
                        siteData = new dtSiteData();

                    var tmpEmp1 = emphandler.GetActiveEmployee(contData.dtTbt_SaleBasic[0].SalesmanEmpNo1);
                    var tmpEmp2 = emphandler.GetActiveEmployee(contData.dtTbt_SaleBasic[0].SalesmanEmpNo2);
                    var tmpEmp3 = emphandler.GetActiveEmployee(contData.dtTbt_SaleBasic[0].SalesmanEmpNo3);
                    var tmpEmp4 = emphandler.GetActiveEmployee(contData.dtTbt_SaleBasic[0].SalesmanEmpNo4);
                    var tmpEmp5 = emphandler.GetActiveEmployee(contData.dtTbt_SaleBasic[0].SalesmanEmpNo5);
                    var tmpEmp6 = emphandler.GetActiveEmployee(contData.dtTbt_SaleBasic[0].SalesmanEmpNo6);
                    var tmpEmp7 = emphandler.GetActiveEmployee(contData.dtTbt_SaleBasic[0].SalesmanEmpNo7);
                    var tmpEmp8 = emphandler.GetActiveEmployee(contData.dtTbt_SaleBasic[0].SalesmanEmpNo8);
                    var tmpEmp9 = emphandler.GetActiveEmployee(contData.dtTbt_SaleBasic[0].SalesmanEmpNo9);
                    var tmpEmp10 = emphandler.GetActiveEmployee(contData.dtTbt_SaleBasic[0].SalesmanEmpNo10);
                    var tmpEmpIE = emphandler.GetActiveEmployee(contData.dtTbt_SaleBasic[0].IEInchargeEmpNo);
                    var tmpEmpNego = emphandler.GetActiveEmployee(contData.dtTbt_SaleBasic[0].NegotiationStaffEmpNo1);
                    var tmpCompInst = emphandler.GetActiveEmployee(contData.dtTbt_SaleBasic[0].InstallationCompleteEmpNo);

                    empData.Add((tmpEmp1.Count == 1) ? tmpEmp1[0] : null);
                    empData.Add((tmpEmp2.Count == 1) ? tmpEmp2[0] : null);
                    empData.Add((tmpEmp3.Count == 1) ? tmpEmp3[0] : null);
                    empData.Add((tmpEmp4.Count == 1) ? tmpEmp4[0] : null);
                    empData.Add((tmpEmp5.Count == 1) ? tmpEmp5[0] : null);
                    empData.Add((tmpEmp6.Count == 1) ? tmpEmp6[0] : null);
                    empData.Add((tmpEmp7.Count == 1) ? tmpEmp7[0] : null);
                    empData.Add((tmpEmp8.Count == 1) ? tmpEmp8[0] : null);
                    empData.Add((tmpEmp9.Count == 1) ? tmpEmp9[0] : null);
                    empData.Add((tmpEmp10.Count == 1) ? tmpEmp10[0] : null);
                    empData.Add((tmpEmpIE.Count == 1) ? tmpEmpIE[0] : null);
                    empData.Add((tmpEmpNego.Count == 1) ? tmpEmpNego[0] : null);
                    empData.Add((tmpCompInst.Count == 1) ? tmpCompInst[0] : null);

                    result.ResultData = PharseOutput_CTS150(contData, maintenData, securData, custData, siteData, empData);
                }
            }
            else
            {
                result.AddErrorMessage("Common", MessageUtil.MessageList.MSG0106, new string[] { occCode }, new string[] { "OccurrenceCode" });
            }

            return result;
        }

        /// <summary>
        /// Retrieve subcontractor data to initial and display on screen
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="occCode"></param>
        /// <returns></returns>
        private ObjectResultData RetrieveSubcontractorGridData_CTS150(string contractCode, string occCode)
        {
            ObjectResultData result = new ObjectResultData();
            ISaleContractHandler salehandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;

            List<dtTbt_SaleInstSubcontractorListForView> rawData = salehandler.GetTbt_SaleInstSubcontractorListForView(contractCode, occCode, null);
            if (rawData == null)
                rawData = new List<dtTbt_SaleInstSubcontractorListForView>();

            result.ResultData = CommonUtil.ConvertToXml<CTS150_SubContractorResult>(PharseOutput_CTS150(rawData), "Contract\\CTS150_Subcontractor", CommonUtil.GRID_EMPTY_TYPE.VIEW);

            return result;
        }

        /// <summary>
        /// Retrieve instrument data to initial and display on screen
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="occCode"></param>
        /// <returns></returns>
        private ObjectResultData RetrieveInstrumentGridData_CTS150(string contractCode, string occCode)
        {
            ObjectResultData result = new ObjectResultData();
            ISaleContractHandler salehandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;

            dsSaleContractData contract = salehandler.GetEntireContract(contractCode, occCode);
            List<tbt_SaleInstrumentDetails> rawData = new List<tbt_SaleInstrumentDetails>();

            if ((contract != null) 
                && (contract.dtTbt_SaleInstrumentDetails != null))
            {
                rawData = contract.dtTbt_SaleInstrumentDetails;
            }

            result.ResultData = CommonUtil.ConvertToXml<CTS150_InstrumentResult>(PharseOutput_CTS150(rawData), "Contract\\CTS150_Instrument", CommonUtil.GRID_EMPTY_TYPE.INSERT);

            return result;
        }

        /// <summary>
        /// Create output contract data to display
        /// </summary>
        /// <param name="rawData"></param>
        /// <param name="maintenData"></param>
        /// <param name="securData"></param>
        /// <param name="custData"></param>
        /// <param name="siteData"></param>
        /// <param name="salesData"></param>
        /// <returns></returns>
        private CTS150_ContractDetailResult PharseOutput_CTS150(dsSaleContractData rawData, dtTbt_RentalMaintenanceDetailsForView maintenData, dtTbt_RentalSecurityBasicForView securData, doCustomer custData, dtSiteData siteData, List<tbm_Employee> salesData)
        {
            CommonUtil util = new CommonUtil();
            IProductMasterHandler producthandler = ServiceContainer.GetService<IProductMasterHandler>() as IProductMasterHandler;
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            List<string> fieldNameList = new List<string>();
            var prodObj = producthandler.GetTbm_Product(rawData.dtTbt_SaleBasic[0].ProductCode, null);
            CommonUtil.MappingObjectLanguage<tbm_Product>(prodObj);
            fieldNameList.Add(MiscType.C_SALE_CHANGE_TYPE);
            fieldNameList.Add(MiscType.C_MA_TYPE);
            fieldNameList.Add(MiscType.C_SALE_INSTALL_TYPE);

            var miscList = commonhandler.GetMiscTypeCodeListByFieldName(fieldNameList);
            var changeTypeItem = miscList.Where(x => x.ValueCode == rawData.dtTbt_SaleBasic[0].ChangeType && x.FieldName == MiscType.C_SALE_CHANGE_TYPE).ToList();
            var maintainTypeItem = miscList.Where(x => x.ValueCode == maintenData.MaintenanceTypeCode && x.FieldName == MiscType.C_MA_TYPE).ToList();
            var installTyupeItem = miscList.Where(x => x.ValueCode == rawData.dtTbt_SaleBasic[0].InstallationTypeCode && x.FieldName == MiscType.C_SALE_INSTALL_TYPE).ToList();

            CTS150_ContractDetailResult result = new CTS150_ContractDetailResult()
            {
                ChangeTypeCode = changeTypeItem[0].ValueCode,
                PurchaseCode = util.ConvertCustCode(rawData.dtTbt_SaleBasic[0].PurchaserCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                EndUserCode = util.ConvertCustCode(rawData.dtTbt_SaleBasic[0].RealCustomerCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                SiteCode = util.ConvertSiteCode(rawData.dtTbt_SaleBasic[0].SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                SiteNameEN = siteData.SiteNameEN,
                SiteNameLC = siteData.SiteNameLC,
                SiteAddressEN = siteData.AddressFullEN,
                SiteAddressLC = siteData.AddressFullLC,
                PurchaseNameEN = custData.CustFullNameEN,
                PurchaseNameLC = custData.CustFullNameLC,
                PurchaseAddressEN = custData.AddressFullEN,
                PurchaseAddressLC = custData.AddressFullLC,
                //ChangeType = rawData.dtTbt_SaleBasic[0].ChangeType,
                ChangeType = (changeTypeItem.Count == 1) ? CommonUtil.TextCodeName(changeTypeItem[0].ValueCode, changeTypeItem[0].ValueDisplay) : "",
                IsImportant = custData.ImportantFlag.GetValueOrDefault(),
                ProductName = (prodObj.Count == 1) ? CommonUtil.TextCodeName(prodObj[0].ProductCode, prodObj[0].ProductName) : "",
                ApproveContractDate = CommonUtil.TextDate(rawData.dtTbt_SaleBasic[0].FirstContractDate),
                ContractOffice = rawData.dtTbt_SaleBasic[0].ContractOfficeCode,
                SaleType = rawData.dtTbt_SaleBasic[0].SalesType,
                OperationOffice = rawData.dtTbt_SaleBasic[0].OperationOfficeCode,
                PlanCode = rawData.dtTbt_SaleBasic[0].PlanCode,
                SalesOffice = rawData.dtTbt_SaleBasic[0].SalesOfficeCode,
                RawQuotationCode = util.ConvertQuotationTargetCode(rawData.dtTbt_SaleBasic[0].QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                RawAlphabet = rawData.dtTbt_SaleBasic[0].Alphabet,
                QuotationCode = util.ConvertQuotationTargetCode(rawData.dtTbt_SaleBasic[0].QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT) + "-" + rawData.dtTbt_SaleBasic[0].Alphabet,

                QuotationNo = rawData.dtTbt_SaleBasic[0].QuotationNo,

                ProjectCode = util.ConvertProjectCode(rawData.dtTbt_SaleBasic[0].ProjectCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                DocAuditResult = rawData.dtTbt_SaleBasic[0].DocAuditResult,
                DistributedType = rawData.dtTbt_SaleBasic[0].DistributedInstallTypeCode,
                DocRecieveDate = CommonUtil.TextDate(rawData.dtTbt_SaleBasic[0].DocReceiveDate),
                DistributedOriginCode = util.ConvertContractCode(rawData.dtTbt_SaleBasic[0].DistributedOriginCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                OnlineContractCode = util.ConvertContractCode(rawData.dtTbt_SaleBasic[0].ConnectTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT),

                //ProductBillingAmount = CommonUtil.TextNumeric(rawData.dtTbt_SaleBasic[0].OrderProductPrice),
                //ProductNormalAmount = CommonUtil.TextNumeric(rawData.dtTbt_SaleBasic[0].NormalProductPrice),
                //InstallBillingAmount = CommonUtil.TextNumeric(rawData.dtTbt_SaleBasic[0].OrderInstallFee),
                //InstallNormalAmount = CommonUtil.TextNumeric(rawData.dtTbt_SaleBasic[0].NormalInstallFee),
                //BidGuaranteeAmount1 = CommonUtil.TextNumeric(rawData.dtTbt_SaleBasic[0].BidGuaranteeAmount1),
                //BidGuaranteeAmount2 = CommonUtil.TextNumeric(rawData.dtTbt_SaleBasic[0].BidGuaranteeAmount2),

                BidGuaranteeReturnDate1 = CommonUtil.TextDate(rawData.dtTbt_SaleBasic[0].BidGuaranteeReturnDate1),
                BidGuaranteeReturnDate2 = CommonUtil.TextDate(rawData.dtTbt_SaleBasic[0].BidGuaranteeReturnDate2),

                ContractApproveNo1 = rawData.dtTbt_SaleBasic[0].ApproveNo1,
                ContractApproveNo2 = rawData.dtTbt_SaleBasic[0].ApproveNo2,
                ContractApproveNo3 = rawData.dtTbt_SaleBasic[0].ApproveNo3,
                ContractApproveNo4 = rawData.dtTbt_SaleBasic[0].ApproveNo4,
                ContractApproveNo5 = rawData.dtTbt_SaleBasic[0].ApproveNo5,
                PurchaseReasonType = rawData.dtTbt_SaleBasic[0].MotivationTypeCode,
                AcquisitionType = rawData.dtTbt_SaleBasic[0].AcquisitionTypeCode,
                IntroducerCode = rawData.dtTbt_SaleBasic[0].IntroducerCode,

                SalesmanCode1 = rawData.dtTbt_SaleBasic[0].SalesmanEmpNo1,
                SalesmanCode2 = rawData.dtTbt_SaleBasic[0].SalesmanEmpNo2,
                SalesmanCode3 = rawData.dtTbt_SaleBasic[0].SalesmanEmpNo3,
                SalesmanCode4 = rawData.dtTbt_SaleBasic[0].SalesmanEmpNo4,
                SalesmanCode5 = rawData.dtTbt_SaleBasic[0].SalesmanEmpNo5,
                SalesmanCode6 = rawData.dtTbt_SaleBasic[0].SalesmanEmpNo6,
                SalesmanCode7 = rawData.dtTbt_SaleBasic[0].SalesmanEmpNo7,
                SalesmanCode8 = rawData.dtTbt_SaleBasic[0].SalesmanEmpNo8,
                SalesmanCode9 = rawData.dtTbt_SaleBasic[0].SalesmanEmpNo9,
                SalesmanCode10 = rawData.dtTbt_SaleBasic[0].SalesmanEmpNo10,

                SalesmanName1 = (salesData[0] != null) ? salesData[0].EmpFullName : String.Empty,
                SalesmanName2 = (salesData[1] != null) ? salesData[1].EmpFullName : String.Empty,
                SalesmanName3 = (salesData[2] != null) ? salesData[2].EmpFullName : String.Empty,
                SalesmanName4 = (salesData[3] != null) ? salesData[3].EmpFullName : String.Empty,
                SalesmanName5 = (salesData[4] != null) ? salesData[4].EmpFullName : String.Empty,
                SalesmanName6 = (salesData[5] != null) ? salesData[5].EmpFullName : String.Empty,
                SalesmanName7 = (salesData[6] != null) ? salesData[6].EmpFullName : String.Empty,
                SalesmanName8 = (salesData[7] != null) ? salesData[7].EmpFullName : String.Empty,
                SalesmanName9 = (salesData[8] != null) ? salesData[8].EmpFullName : String.Empty,
                SalesmanName10 = (salesData[9] != null) ? salesData[9].EmpFullName : String.Empty,

                ProcessManagementStatus = rawData.dtTbt_SaleBasic[0].SaleProcessManageStatus,
                InstallSlipNo = rawData.dtTbt_SaleBasic[0].InstallationSlipNo,
                ExpectedCompleteInstallDate = CommonUtil.TextDate(rawData.dtTbt_SaleBasic[0].ExpectedInstallCompleteDate),
                ExpectedCustomerAccept = CommonUtil.TextDate(rawData.dtTbt_SaleBasic[0].ExpectedCustAcceptanceDate),
                InstrumentStockOutDate = CommonUtil.TextDate(rawData.dtTbt_SaleBasic[0].InstrumentStockOutDate),
                SubcontractCompleteInstallDate = CommonUtil.TextDate(rawData.dtTbt_SaleBasic[0].SubcontractInstallCompleteDate),
                CompleteInstallDate = CommonUtil.TextDate(rawData.dtTbt_SaleBasic[0].InstallCompleteDate),
                DeliveryDocRecieveDate = CommonUtil.TextDate(rawData.dtTbt_SaleBasic[0].DeliveryDocReceiveDate),
                CustomerAcceptDate = CommonUtil.TextDate(rawData.dtTbt_SaleBasic[0].CustAcceptanceDate),
                IEInChargeCode = rawData.dtTbt_SaleBasic[0].IEInchargeEmpNo,
                IEInChargeName = (salesData[10] != null) ? salesData[10].EmpFullName : string.Empty,
                NewOldBuilding = rawData.dtTbt_SaleBasic[0].BuildingTypeCode,
                NewBuildMgmtType = rawData.dtTbt_SaleBasic[0].NewBldMgmtFlag.GetValueOrDefault(),
                NewBuildingMgmtCost = CommonUtil.TextNumeric(rawData.dtTbt_SaleBasic[0].NewBldMgmtCost),

                MaintenanceContractCode = util.ConvertContractCode(rawData.dtTbt_SaleBasic[0].ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                MaintenanceTargetProduct = CommonUtil.TextCodeName(maintenData.MaintenanceTargetProductTypeCode, maintenData.MaintenanceTargetProductType),
                //MaintenanceType = maintenData.MaintenanceTypeCode
                MaintenanceType = (maintainTypeItem.Count == 1) ? CommonUtil.TextCodeName(maintainTypeItem[0].ValueCode, maintainTypeItem[0].ValueDisplay) : "",
                MaintenanceCycle = securData.MaintenanceCycle.GetValueOrDefault().ToString(),
                MaintenanceStartMonth = maintenData.MaintenanceContractStartMonth.GetValueOrDefault().ToString(),
                MaintenanceStartYear = maintenData.MaintenanceContractStartYear.GetValueOrDefault().ToString(),
                MaintenanceFeeType = maintenData.MaintenanceFeeTypeCode,
                MaintenanceMemo = maintenData.MaintenanceMemo,
                ContractDuration = securData.ContractDurationMonth.GetValueOrDefault().ToString(),
                EndContractDate = CommonUtil.TextDate(securData.ContractEndDate),
                AutoRenew = securData.AutoRenewMonth.GetValueOrDefault().ToString(),
                StartMaintenanceDate = CommonUtil.TextDate(rawData.dtTbt_SaleBasic[0].StartMaintenanceDate),
                EndMaintenanceDate = CommonUtil.TextDate(rawData.dtTbt_SaleBasic[0].EndMaintenanceDate),

                //InstallationType = CommonUtil.TextCodeName(rawData.dtTbt_SaleBasic[0].InstallationTypeCode, ""),
                InstallationType = (installTyupeItem.Count == 1) ? CommonUtil.TextCodeName(rawData.dtTbt_SaleBasic[0].InstallationTypeCode, installTyupeItem[0].ValueDisplay) : "",
                //NormalInstallFee = rawData.dtTbt_SaleBasic[0].NormalInstallFee.GetValueOrDefault().ToString("#,##0.00"),
                //OrderInstallFee = rawData.dtTbt_SaleBasic[0].OrderInstallFee.GetValueOrDefault().ToString("#,##0.00"),

                //SecomPayment = rawData.dtTbt_SaleBasic[0].InstallFeePaidBySECOM.GetValueOrDefault().ToString("#,##0.00"),
                //SecomRevenue = rawData.dtTbt_SaleBasic[0].InstallFeeRevenueBySECOM.GetValueOrDefault().ToString("#,##0.00"),

                ChangeApproveNo1 = rawData.dtTbt_SaleBasic[0].ApproveNo1,
                NegotiationStaffCode = rawData.dtTbt_SaleBasic[0].NegotiationStaffEmpNo1,
                NegotiationStaffName = (salesData[11] != null) ? salesData[11].EmpFullName : string.Empty,
                ChangeApproveNo2 = rawData.dtTbt_SaleBasic[0].ApproveNo2,
                CompleteRegistrantCode = rawData.dtTbt_SaleBasic[0].InstallationCompleteEmpNo,
                CompleteRegistrantName = (salesData[12] != null) ? salesData[12].EmpFullName : string.Empty,

                WaranteePeriodFrom = CommonUtil.TextDate(rawData.dtTbt_SaleBasic[0].WarranteeFrom),
                WaranteePeriodTo = CommonUtil.TextDate(rawData.dtTbt_SaleBasic[0].WarranteeTo),

                HasMA = rawData.dtTbt_SaleBasic[0].MaintenanceContractFlag.GetValueOrDefault(),

                PaymentDateIncentive = CommonUtil.TextDate(rawData.dtTbt_SaleBasic[0].PaymentDateIncentive)
            };

            #region Order Product Price

            result.ProductBillingAmountCurrencyType = rawData.dtTbt_SaleBasic[0].OrderProductPriceCurrencyType;

            if (rawData.dtTbt_SaleBasic[0].OrderProductPriceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                result.ProductBillingAmount = CommonUtil.TextNumeric(rawData.dtTbt_SaleBasic[0].OrderProductPriceUsd);
            else
                result.ProductBillingAmount = CommonUtil.TextNumeric(rawData.dtTbt_SaleBasic[0].OrderProductPrice);

            #endregion
            #region Normal Product Price

            result.ProductNormalAmountCurrencyType = rawData.dtTbt_SaleBasic[0].NormalProductPriceCurrencyType;

            if (rawData.dtTbt_SaleBasic[0].NormalProductPriceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                result.ProductNormalAmount = CommonUtil.TextNumeric(rawData.dtTbt_SaleBasic[0].NormalProductPriceUsd);
            else
                result.ProductNormalAmount = CommonUtil.TextNumeric(rawData.dtTbt_SaleBasic[0].NormalProductPrice);

            #endregion
            #region Order Install Fee

            result.InstallBillingAmountCurrencyType = rawData.dtTbt_SaleBasic[0].OrderInstallFeeCurrencyType;

            if (rawData.dtTbt_SaleBasic[0].OrderInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                result.InstallBillingAmount = CommonUtil.TextNumeric(rawData.dtTbt_SaleBasic[0].OrderInstallFeeUsd);
            else
                result.InstallBillingAmount = CommonUtil.TextNumeric(rawData.dtTbt_SaleBasic[0].OrderInstallFee);

            #endregion
            #region Normal Install Fee

            result.InstallNormalAmountCurrencyType = rawData.dtTbt_SaleBasic[0].NormalInstallFeeCurrencyType;

            if (rawData.dtTbt_SaleBasic[0].NormalInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                result.InstallNormalAmount = CommonUtil.TextNumeric(rawData.dtTbt_SaleBasic[0].NormalInstallFeeUsd);
            else
                result.InstallNormalAmount = CommonUtil.TextNumeric(rawData.dtTbt_SaleBasic[0].NormalInstallFee);

            #endregion
            #region Bid Guarantee Amount 1

            result.BidGuaranteeAmount1CurrencyType = rawData.dtTbt_SaleBasic[0].BidGuaranteeAmount1CurrencyType;

            if (rawData.dtTbt_SaleBasic[0].BidGuaranteeAmount1CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                result.BidGuaranteeAmount1 = CommonUtil.TextNumeric(rawData.dtTbt_SaleBasic[0].BidGuaranteeAmount1Usd);
            else
                result.BidGuaranteeAmount1 = CommonUtil.TextNumeric(rawData.dtTbt_SaleBasic[0].BidGuaranteeAmount1);

            #endregion
            #region Bid Guarantee Amount 2

            result.BidGuaranteeAmount2CurrencyType = rawData.dtTbt_SaleBasic[0].BidGuaranteeAmount2CurrencyType;

            if (rawData.dtTbt_SaleBasic[0].BidGuaranteeAmount2CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                result.BidGuaranteeAmount2 = CommonUtil.TextNumeric(rawData.dtTbt_SaleBasic[0].BidGuaranteeAmount2Usd);
            else
                result.BidGuaranteeAmount2 = CommonUtil.TextNumeric(rawData.dtTbt_SaleBasic[0].BidGuaranteeAmount2);

            #endregion

            #region Normal Install Fee

            result.NormalInstallFeeCurrencyType = rawData.dtTbt_SaleBasic[0].NormalInstallFeeCurrencyType;

            if (rawData.dtTbt_SaleBasic[0].NormalInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                result.NormalInstallFee = CommonUtil.TextNumeric(rawData.dtTbt_SaleBasic[0].NormalInstallFeeUsd);
            else
                result.NormalInstallFee = CommonUtil.TextNumeric(rawData.dtTbt_SaleBasic[0].NormalInstallFee);

            #endregion
            #region Order Install Fee

            result.OrderInstallFeeCurrencyType = rawData.dtTbt_SaleBasic[0].OrderInstallFeeCurrencyType;

            if (rawData.dtTbt_SaleBasic[0].OrderInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                result.OrderInstallFee = CommonUtil.TextNumeric(rawData.dtTbt_SaleBasic[0].OrderInstallFeeUsd);
            else
                result.OrderInstallFee = CommonUtil.TextNumeric(rawData.dtTbt_SaleBasic[0].OrderInstallFee);

            #endregion
            #region Install Fee Paid by SECOM

            result.SecomPaymentCurrencyType = rawData.dtTbt_SaleBasic[0].InstallFeePaidBySECOMCurrencyType;

            if (rawData.dtTbt_SaleBasic[0].InstallFeePaidBySECOMCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                result.SecomPayment = CommonUtil.TextNumeric(rawData.dtTbt_SaleBasic[0].InstallFeePaidBySECOMUsd);
            else
                result.SecomPayment = CommonUtil.TextNumeric(rawData.dtTbt_SaleBasic[0].InstallFeePaidBySECOM);

            #endregion
            #region Install Fee Revenue by SECOM

            result.SecomRevenueCurrencyType = rawData.dtTbt_SaleBasic[0].InstallFeeRevenueBySECOMCurrencyType;

            if (rawData.dtTbt_SaleBasic[0].InstallFeeRevenueBySECOMCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                result.SecomRevenue = CommonUtil.TextNumeric(rawData.dtTbt_SaleBasic[0].InstallFeeRevenueBySECOMUsd);
            else
                result.SecomRevenue = CommonUtil.TextNumeric(rawData.dtTbt_SaleBasic[0].InstallFeeRevenueBySECOM);

            #endregion
            
            if ((rawData.dtTbt_SaleBasic[0].ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_NEW_SALE) 
                || (rawData.dtTbt_SaleBasic[0].ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_ADD_SALE)
                || (rawData.dtTbt_SaleBasic[0].ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_CANCEL))
            {
                result.ViewMode = 1;
            }
            else if (rawData.dtTbt_SaleBasic[0].ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_CHANGE_NAME)
            {
                result.ViewMode = 2;
            }
            else if ((rawData.dtTbt_SaleBasic[0].ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_EXCHANGE_INSTR)
                || (rawData.dtTbt_SaleBasic[0].ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_MOVE_INSTR)
                || (rawData.dtTbt_SaleBasic[0].ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_REMOVE_INSTR_ALL)
                || (rawData.dtTbt_SaleBasic[0].ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_REMOVE_INSTR_PARTIAL)
                || (rawData.dtTbt_SaleBasic[0].ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_CHANGE_WIRING)) //Add by Jutarat A. on 21052013
            {
                result.ViewMode = 3;
            }

            return result;
        }

        /// <summary>
        /// Create subcontractor data to display
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        private List<CTS150_SubContractorResult> PharseOutput_CTS150(List<dtTbt_SaleInstSubcontractorListForView> rawData)
        {
            List<CTS150_SubContractorResult> result = new List<CTS150_SubContractorResult>();

            foreach (var item in rawData)
            {
                CTS150_SubContractorResult tmpItem = new CTS150_SubContractorResult()
                {
                    SubcontractorCode = item.SubcontractorCode,
                    SubContractorNameEN = item.SubContractorNameEN,
                    SubContractorNameLC = item.SubContractorNameLC
                };

                result.Add(tmpItem);
            }

            return result;
        }

        /// <summary>
        /// Create instrument data to display
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        private List<CTS150_InstrumentResult> PharseOutput_CTS150(List<tbt_SaleInstrumentDetails> rawData)
        {
            List<CTS150_InstrumentResult> result = new List<CTS150_InstrumentResult>();
            IInstrumentMasterHandler insthandler = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;

            //if (rawData.Count == 0)
            //{
            //    Random rand = new Random();
            //    for (int i = 0; i < 12; i++)
            //    {
            //        int randQty = rand.Next(-10, 10);
            //        CTS150_InstrumentResult tmpItem = new CTS150_InstrumentResult()
            //        {
            //            InstrumentCode = i.ToString(),
            //            InstrumentName = i.ToString() + " Name",
            //            MakerName = i + " Maker",
            //            QtyAdded = (randQty > 0) ? randQty.ToString() : "0",
            //            QtyRemoved = (randQty < 0) ? Math.Abs(randQty).ToString() : "0",
            //            QtyTotal = randQty.ToString()
            //        };

            //        result.Add(tmpItem);
            //    }
            //}
            //else
            {

                foreach (var item in rawData)
                {
                    var tmpInst = insthandler.GetInstrumentDataForSearch(new doInstrumentSearchCondition()
                    {
                        InstrumentCode = item.InstrumentCode,
                    });

                    CTS150_InstrumentResult tmpItem = new CTS150_InstrumentResult()
                    {
                        InstrumentCode = item.InstrumentCode,
                        InstrumentName = (tmpInst.Count == 1) ? tmpInst[0].InstrumentName : String.Empty,
                        MakerName = (tmpInst.Count == 1) ? tmpInst[0].Maker : String.Empty,
                        QtyAdded = (item.InstrumentQty.GetValueOrDefault() > 0) ? item.InstrumentQty.GetValueOrDefault().ToString() : "0",
                        QtyRemoved = (item.InstrumentQty.GetValueOrDefault() < 0) ? Math.Abs(item.InstrumentQty.GetValueOrDefault()).ToString() : "0",
                        QtyTotal = item.InstrumentQty.GetValueOrDefault().ToString()
                    };

                    result.Add(tmpItem);
                }
            }

            return result;
        }

        /// <summary>
        /// Checking system suspend and user permission
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        private ObjectResultData ValidateAuthority_CTS150(ObjectResultData res)
        {
            if (CheckIsSuspending(res))
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                return res;
            }
            else if (CheckUserPermission(ScreenID.C_SCREEN_ID_REGISTER_CQ31, FunctionID.C_FUNC_ID_OPERATE) == false)
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                return res;
            }

            return res;
        }

        //private void SetScreenParameter_CTS150(CTS150_ScreenParameter obj)
        //{
        //    Session.Remove("CTS150_PARAM");
        //    Session.Add("CTS150_PARAM", obj);
        //}

        //private CTS150_ScreenParameter GetScreenObject_CTS150()
        //{
        //    CTS150_ScreenParameter obj = null;

        //    if (Session["CTS150_PARAM"] != null)
        //    {
        //        obj = (CTS150_ScreenParameter)Session["CTS150_PARAM"];
        //    }

        //    return obj;
        //}
        #endregion
    }
}
