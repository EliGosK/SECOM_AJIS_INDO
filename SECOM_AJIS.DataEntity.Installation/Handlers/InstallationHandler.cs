using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Contract;
using CSI.WindsorHelper;
using System.Reflection;
using System.Configuration;
using System.IO;
using System.Web.Mvc;


namespace SECOM_AJIS.DataEntity.Installation
{
    class InstallationHandler : BizISDataEntities, IInstallationHandler
    {

        public string GenerateInstallationMANo(doGenerateInstallationMANo cond)
        {
            try
            {
                string strPrefix = "";
                string strRunningNo = "";
                int RunningNo;
                DateTime CurrentDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                int Year = CommonUtil.dsTransData.dtOperationData.ProcessDateTime.Year;
                string CurrentUser = CommonUtil.dsTransData.dtUserData.EmpNo;

                if (cond.strServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                {
                    strPrefix = InstallationMANo.C_INSTALL_MA_NO_PREFIX_RENTAL;
                }
                else if (cond.strServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                {
                    strPrefix = InstallationMANo.C_INSTALL_MA_NO_PREFIX_SALE;
                }
                else if (cond.strServiceTypeCode == ServiceType.C_SERVICE_TYPE_PROJECT)
                {
                    strPrefix = InstallationMANo.C_INSTALL_MA_NO_PREFIX_PROJECT;
                }

                List<InstallationMARunningNo> ListResult = base.GetTbs_InstallationMARunningNo(cond.strOfficeCode, strPrefix, Year);
                if (ListResult.Count > 0)
                {
                    RunningNo = Convert.ToInt32(ListResult[0].RunningNo) + 1;

                    if (RunningNo > Convert.ToInt32(InstallationMANo.C_INSTALL_MA_NO_MAXIMUM))
                    {
                        //Throw Error
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5064);
                    }
                    else
                    {
                        //strRunningNo = RunningNo.ToString("0000");
                        int result = base.UpdateTbs_InstallationMARunningNo(cond.strOfficeCode, strPrefix, Year, RunningNo, CurrentDateTime, CurrentUser);
                    }
                }
                else
                {
                    RunningNo = Convert.ToInt32(InstallationMANo.C_INSTALL_MA_NO_MINIMUM);
                    int result = base.InsertTbs_InstallationMARunningNo(cond.strOfficeCode, strPrefix, Year, CurrentDateTime, CurrentUser, CurrentDateTime, CurrentUser, RunningNo);
                }
                return cond.strOfficeCode + strPrefix + Year.ToString() + RunningNo.ToString("0000");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string GenerateInstallationSlipNo(doGenerateInstallationSlipNoCond cond)
        {
            try
            {
                string ResultSlipNo = "";
                int CurrentYear = CommonUtil.dsTransData.dtOperationData.ProcessDateTime.Year;
                int CurrentMonth = CommonUtil.dsTransData.dtOperationData.ProcessDateTime.Month;
                DateTime CurrentDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                string CurrentUser = CommonUtil.dsTransData.dtUserData.EmpNo;
                //============================ Teerapong S. 04/10/2012 for solve error dead lock =========================
                //string strRunningNo = "";
                //int RunningNo;
                //DateTime CurrentDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                //int CurrentYear = CommonUtil.dsTransData.dtOperationData.ProcessDateTime.Year;
                //int CurrentMonth = CommonUtil.dsTransData.dtOperationData.ProcessDateTime.Month;
                //string CurrentUser = CommonUtil.dsTransData.dtUserData.EmpNo;

                //List<InstallationSlipRunningNo_Result> ListResult = base.GetTbs_InstallationSlipRunningNo(cond.strOfficeCode, cond.strSlipID, CurrentYear.ToString("0000"), CurrentMonth.ToString("00"));
                //if (ListResult.Count > 0)
                //{
                //    RunningNo = Convert.ToInt32(ListResult[0].RunningNo) + 1;

                //    if (RunningNo > Convert.ToInt32(InstallationSlipNo.C_INSTALL_SLIP_NO_MAXIMUM))
                //    {
                //        //Throw Error
                //        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5065);
                //    }
                //    else
                //    {
                //        //strRunningNo = RunningNo.ToString("000");
                //        int result = base.UpdateTbs_InstallationSlipRunningNo(cond.strOfficeCode, cond.strSlipID, CurrentYear.ToString("0000"), CurrentMonth.ToString("00"), RunningNo, CurrentDateTime, CurrentUser);
                //    }
                //}
                //else
                //{
                //    RunningNo = Convert.ToInt32(InstallationSlipNo.C_INSTALL_SLIP_NO_MINIMUM);
                //    int result = base.InsertTbs_InstallationSlipRunningNo(cond.strOfficeCode, cond.strSlipID, CurrentYear.ToString("0000"), CurrentMonth.ToString("00"), RunningNo, CurrentDateTime, CurrentUser, CurrentDateTime, CurrentUser);
                //}

                List<doGenerateInstallationSlipNo> doGenSlipNo = base.GenerateInstallationSlipNo(cond.strOfficeCode, cond.strSlipID, CurrentYear.ToString("0000"), CurrentMonth.ToString("00"), CurrentDateTime, CurrentUser);
                if (!CommonUtil.IsNullOrEmpty(doGenSlipNo) && doGenSlipNo.Count > 0)
                {
                    ResultSlipNo = cond.strOfficeCode + cond.strSlipID + doGenSlipNo[0].SlipYear + doGenSlipNo[0].SlipMonth + doGenSlipNo[0].RunningNo.Value.ToString("000"); //doGenSlipNo[0].RunningNo; //Modify by Jutarat A. on 25042013
                }
                //========================================================================================================

                //return cond.strOfficeCode + cond.strSlipID + CurrentYear.ToString("0000") + CurrentMonth.ToString("00") + RunningNo.ToString("000");
                return ResultSlipNo;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool CheckInstallationRegister(string strContractCode)
        {
            try
            {
                if (strContractCode != null)
                {
                    List<CheckInstallationRegistered_Result> result = base.CheckInstallationRegistered(strContractCode, InstallationStatus.C_INSTALL_STATUS_INSTALL_NOT_REQUESTED);
                    if (result.Count > 0)
                    {
                        return Convert.ToBoolean(result[0].blnInstallationRegistered);
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int InsertTbt_InstallationBasic(tbt_InstallationBasic doTbt_InstallationBasic)
        {

            try
            {
                if (doTbt_InstallationBasic != null)
                {
                    doTbt_InstallationBasic.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    doTbt_InstallationBasic.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    doTbt_InstallationBasic.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    doTbt_InstallationBasic.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                }
                List<tbt_InstallationBasic> dtInsertedTbt_InstallationBasic = base.InsertTbt_InstallationBasic(doTbt_InstallationBasic.ContractProjectCode, doTbt_InstallationBasic.OCC, doTbt_InstallationBasic.ServiceTypeCode, doTbt_InstallationBasic.InstallationStatus, doTbt_InstallationBasic.InstallationType
                                                                                                             , doTbt_InstallationBasic.PlanCode, doTbt_InstallationBasic.SlipNo, doTbt_InstallationBasic.MaintenanceNo, doTbt_InstallationBasic.OperationOfficeCode, doTbt_InstallationBasic.SecurityTypeCode, doTbt_InstallationBasic.ChangeReasonTypeCode
                                                                                                             , doTbt_InstallationBasic.NormalInstallFee, doTbt_InstallationBasic.BillingInstallFee, doTbt_InstallationBasic.InstallFeeBillingType, doTbt_InstallationBasic.NormalSaleProductPrice, doTbt_InstallationBasic.BillingSalePrice, doTbt_InstallationBasic.InstallationSlipProcessingDate
                                                                                                             , doTbt_InstallationBasic.InstallationCompleteDate, doTbt_InstallationBasic.InstallationCompleteProcessingDate, doTbt_InstallationBasic.InstallationBy, doTbt_InstallationBasic.SalesmanEmpNo1, doTbt_InstallationBasic.SalesmanEmpNo2, doTbt_InstallationBasic.ApproveNo1, doTbt_InstallationBasic.ApproveNo2
                                                                                                             , doTbt_InstallationBasic.InstallationStartDate, doTbt_InstallationBasic.InstallationFinishDate, doTbt_InstallationBasic.NormalContractFee, doTbt_InstallationBasic.BillingOCC, doTbt_InstallationBasic.CreateDate, doTbt_InstallationBasic.CreateBy, doTbt_InstallationBasic.UpdateDate, doTbt_InstallationBasic.UpdateBy
                                                                                                             , doTbt_InstallationBasic.NormalContractFeeUsd, doTbt_InstallationBasic.NormalContractFeeCurrencyType, doTbt_InstallationBasic.NormalInstallFeeUsd, doTbt_InstallationBasic.NormalInstallFeeCurrencyType, doTbt_InstallationBasic.BillingInstallFeeUsd, doTbt_InstallationBasic.BillingInstallFeeCurrencyType
                                                                                                             , doTbt_InstallationBasic.NormalSaleProductPriceUsd, doTbt_InstallationBasic.NormalSaleProductPriceCurrencyType);
                if (dtInsertedTbt_InstallationBasic.Count > 0)
                {
                    ILogHandler hand2 = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    doTransactionLog InsertIntrumentDetailLog = new doTransactionLog();
                    InsertIntrumentDetailLog.TransactionType = doTransactionLog.eTransactionType.Insert;
                    InsertIntrumentDetailLog.TableName = TableName.C_TBL_NAME_INS_BASIC;
                    InsertIntrumentDetailLog.TableData = CommonUtil.ConvertToXml(dtInsertedTbt_InstallationBasic);
                    hand2.WriteTransactionLog(InsertIntrumentDetailLog);
                    return dtInsertedTbt_InstallationBasic.Count;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int UpdateTbt_InstallationBasic(tbt_InstallationBasic doTbt_InstallationBasic)
        {

            try
            {
                if (doTbt_InstallationBasic != null)
                {
                    doTbt_InstallationBasic.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    doTbt_InstallationBasic.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                }
                List<tbt_InstallationBasic> dtInsertedTbt_InstallationBasic = base.UpdateTbt_InstallationBasic(doTbt_InstallationBasic.ContractProjectCode, doTbt_InstallationBasic.OCC, doTbt_InstallationBasic.ServiceTypeCode, doTbt_InstallationBasic.InstallationStatus, doTbt_InstallationBasic.InstallationType, doTbt_InstallationBasic.PlanCode, doTbt_InstallationBasic.SlipNo, doTbt_InstallationBasic.MaintenanceNo
                                                                                                              , doTbt_InstallationBasic.OperationOfficeCode, doTbt_InstallationBasic.SecurityTypeCode, doTbt_InstallationBasic.ChangeReasonTypeCode, doTbt_InstallationBasic.NormalInstallFee, doTbt_InstallationBasic.BillingInstallFee, doTbt_InstallationBasic.InstallFeeBillingType, doTbt_InstallationBasic.NormalSaleProductPrice, doTbt_InstallationBasic.BillingSalePrice
                                                                                                              , doTbt_InstallationBasic.InstallationSlipProcessingDate, doTbt_InstallationBasic.InstallationCompleteDate, doTbt_InstallationBasic.InstallationCompleteProcessingDate, doTbt_InstallationBasic.InstallationBy, doTbt_InstallationBasic.SalesmanEmpNo1, doTbt_InstallationBasic.SalesmanEmpNo2
                                                                                                              , doTbt_InstallationBasic.ApproveNo1, doTbt_InstallationBasic.ApproveNo2, doTbt_InstallationBasic.InstallationStartDate, doTbt_InstallationBasic.InstallationFinishDate, doTbt_InstallationBasic.NormalContractFee, doTbt_InstallationBasic.BillingOCC, doTbt_InstallationBasic.CreateDate, doTbt_InstallationBasic.CreateBy, doTbt_InstallationBasic.UpdateDate
                                                                                                              , doTbt_InstallationBasic.UpdateBy, doTbt_InstallationBasic.NormalInstallFeeUsd, doTbt_InstallationBasic.BillingInstallFeeUsd, doTbt_InstallationBasic.NormalSaleProductPriceUsd, doTbt_InstallationBasic.NormalContractFeeUsd, doTbt_InstallationBasic.NormalInstallFeeCurrencyType, doTbt_InstallationBasic.BillingInstallFeeCurrencyType
                                                                                                              , doTbt_InstallationBasic.NormalSaleProductPriceCurrencyType, doTbt_InstallationBasic.NormalContractFeeCurrencyType);
                if (dtInsertedTbt_InstallationBasic.Count > 0)
                {
                    ILogHandler hand2 = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    doTransactionLog InsertIntrumentDetailLog = new doTransactionLog();
                    InsertIntrumentDetailLog.TransactionType = doTransactionLog.eTransactionType.Update;
                    InsertIntrumentDetailLog.TableName = TableName.C_TBL_NAME_INS_BASIC;
                    InsertIntrumentDetailLog.TableData = CommonUtil.ConvertToXml(dtInsertedTbt_InstallationBasic);
                    hand2.WriteTransactionLog(InsertIntrumentDetailLog);
                    return dtInsertedTbt_InstallationBasic.Count;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int InsertTbt_InstallationManagement(tbt_InstallationManagement doTbt_InstallationManagement)
        {

            try
            {
                if (doTbt_InstallationManagement != null)
                {
                    doTbt_InstallationManagement.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    doTbt_InstallationManagement.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    doTbt_InstallationManagement.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    doTbt_InstallationManagement.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                }
                List<tbt_InstallationManagement> dtInsertedTbt_InstallationManagement = base.InsertTbt_InstallationManagement(doTbt_InstallationManagement.MaintenanceNo, doTbt_InstallationManagement.ContractProjectCode, doTbt_InstallationManagement.ManagementStatus, doTbt_InstallationManagement.ProposeInstallStartDate, doTbt_InstallationManagement.ProposeInstallCompleteDate
                                                                                                                            , doTbt_InstallationManagement.CustomerStaffBelonging, doTbt_InstallationManagement.CustomerStaffName, doTbt_InstallationManagement.CustomerStaffPhoneNo, doTbt_InstallationManagement.NewPhoneLineOpenDate, doTbt_InstallationManagement.NewConnectionPhoneNo
                                                                                                                            , doTbt_InstallationManagement.NewPhoneLineOwnerTypeCode, doTbt_InstallationManagement.IEStaffEmpNo1, doTbt_InstallationManagement.IEStaffEmpNo2, doTbt_InstallationManagement.IEManPower, doTbt_InstallationManagement.MaterialFee, doTbt_InstallationManagement.MaterialFeeUsd
                                                                                                                            , doTbt_InstallationManagement.MaterialFeeCurrencyType, doTbt_InstallationManagement.RequestMemo, doTbt_InstallationManagement.POMemo, doTbt_InstallationManagement.ChangeReasonCode, doTbt_InstallationManagement.ChangeReasonOther, doTbt_InstallationManagement.ChangeRequestorCode
                                                                                                                            , doTbt_InstallationManagement.ChangeRequestorOther, doTbt_InstallationManagement.NewBldMgmtFlag, doTbt_InstallationManagement.NewBldMgmtCost, doTbt_InstallationManagement.NewBldMgmtCostUsd, doTbt_InstallationManagement.NewBldMgmtCostCurrencyType, doTbt_InstallationManagement.CreateDate
                                                                                                                            , doTbt_InstallationManagement.CreateBy, doTbt_InstallationManagement.UpdateDate, doTbt_InstallationManagement.UpdateBy, doTbt_InstallationManagement.ApproveNo); //Add (ApproveNo) by Jutarat A. on 17042013
                if (dtInsertedTbt_InstallationManagement.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_INS_MA;
                    logData.TableData = CommonUtil.ConvertToXml(dtInsertedTbt_InstallationManagement);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                    return dtInsertedTbt_InstallationManagement.Count;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<tbt_InstallationBasic> GetTbt_InstallationBasicData(string strContractProjectCode)
        {

            try
            {
                List<tbt_InstallationBasic> dtInsertedTbt_InstallationBasic = base.GetTbt_InstallationBasic(strContractProjectCode);
                if (dtInsertedTbt_InstallationBasic.Count > 0)
                {
                    return dtInsertedTbt_InstallationBasic;
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

        public List<tbt_InstallationManagement> GetTbt_InstallationManagementData(string strMaintenanceNo)
        {

            try
            {
                List<tbt_InstallationManagement> dtTbt_InstallationManagement = base.GetTbt_InstallationManagement(strMaintenanceNo);
                if (dtTbt_InstallationManagement.Count > 0)
                {
                    return dtTbt_InstallationManagement;
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

        public List<tbt_InstallationPOManagement> GetTbt_InstallationPOManagementData(string strMaintenanceNo)
        {

            try
            {
                List<tbt_InstallationPOManagement> dtInsertedTbt_InstallationPOManagement = base.GetTbt_InstallationPOManagement(strMaintenanceNo);
                if (dtInsertedTbt_InstallationPOManagement.Count > 0)
                {
                    return dtInsertedTbt_InstallationPOManagement;
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

        public List<tbt_InstallationEmail> GetTbt_InstallationEmailData(string strMaintenanceNo)
        {

            try
            {
                List<tbt_InstallationEmail> dtInsertedTbt_InstallationEmail = base.GetTbt_InstallationEmail(strMaintenanceNo);
                if (dtInsertedTbt_InstallationEmail.Count > 0)
                {
                    return dtInsertedTbt_InstallationEmail;
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

        public bool CheckAllRemoval(string strContractCode)
        {
            try
            {

                if (strContractCode != null)
                {
                    List<CheckAllRemoval_Result> result = base.CheckAllRemoval(strContractCode, RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_ALL, SaleInstallationType.C_SALE_INSTALL_TYPE_REMOVE_ALL, InstallationStatus.C_INSTALL_STATUS_COMPLETED);
                    if (result.Count > 0)
                    {
                        return Convert.ToBoolean(result[0].blnAllRemoval);
                    }
                    else
                    {
                        return false;

                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int InsertTbt_InstallationMemo(tbt_InstallationMemo doTbt_InstallationMemo)
        {

            try
            {
                if (doTbt_InstallationMemo != null)
                {
                    doTbt_InstallationMemo.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    doTbt_InstallationMemo.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    doTbt_InstallationMemo.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    doTbt_InstallationMemo.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                }
                List<tbt_InstallationMemo> dtInsertedTbt_InstallationMemo = base.InsertTbt_InstallationMemo(doTbt_InstallationMemo.MemoID, doTbt_InstallationMemo.ContractProjectCode, doTbt_InstallationMemo.ReferenceID, doTbt_InstallationMemo.ObjectID
                                                                                                            , doTbt_InstallationMemo.Memo, doTbt_InstallationMemo.OfficeCode, doTbt_InstallationMemo.DepartmentCode, doTbt_InstallationMemo.CreateDate
                                                                                                            , doTbt_InstallationMemo.CreateBy, doTbt_InstallationMemo.UpdateDate, doTbt_InstallationMemo.UpdateBy);

                if (dtInsertedTbt_InstallationMemo.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_INS_MEMO;
                    logData.TableData = CommonUtil.ConvertToXml(dtInsertedTbt_InstallationMemo);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                    return dtInsertedTbt_InstallationMemo.Count;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int InsertTbt_InstallationEmail(tbt_InstallationEmail doTbt_InstallationEmail)
        {

            try
            {
                if (doTbt_InstallationEmail != null)
                {
                    doTbt_InstallationEmail.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    doTbt_InstallationEmail.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    doTbt_InstallationEmail.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    doTbt_InstallationEmail.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                }
                List<tbt_InstallationEmail> dtInsertedTbt_InstallationEmail = base.InsertTbt_InstallationEmail(doTbt_InstallationEmail.EmailID
                                                                                                                , doTbt_InstallationEmail.ReferenceID
                                                                                                                , doTbt_InstallationEmail.EmailNoticeTarget
                                                                                                                , doTbt_InstallationEmail.CreateDate
                                                                                                                , doTbt_InstallationEmail.CreateBy
                                                                                                                , doTbt_InstallationEmail.UpdateDate
                                                                                                                , doTbt_InstallationEmail.UpdateBy);
                if (dtInsertedTbt_InstallationEmail.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_INS_EMAIL;
                    logData.TableData = CommonUtil.ConvertToXml(dtInsertedTbt_InstallationEmail);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                    return dtInsertedTbt_InstallationEmail.Count;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int InsertTbt_InstallationPOManagement(tbt_InstallationPOManagement doTbt_InstallationPOManagement)
        {

            try
            {
                if (doTbt_InstallationPOManagement != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    doTbt_InstallationPOManagement.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                    doTbt_InstallationPOManagement.CreateBy = dsTrans.dtUserData.EmpNo;
                    doTbt_InstallationPOManagement.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                    doTbt_InstallationPOManagement.UpdateBy = dsTrans.dtUserData.EmpNo;

                }
                List<tbt_InstallationPOManagement> ListdoTbt_InstallationPOManagement = new List<tbt_InstallationPOManagement>();
                ListdoTbt_InstallationPOManagement.Add(doTbt_InstallationPOManagement);

                List<tbt_InstallationPOManagement> res = this.InsertTbt_InstallationPOManagement(CommonUtil.ConvertToXml_Store<tbt_InstallationPOManagement>(ListdoTbt_InstallationPOManagement));

                if (res.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_INS_PO_MA;
                    logData.TableData = CommonUtil.ConvertToXml(res);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }
                return res.Count;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int UpdateTbt_InstallationPOManagement(tbt_InstallationPOManagement doTbt_InstallationPOManagement)
        {

            try
            {
                if (doTbt_InstallationPOManagement != null)
                {
                    doTbt_InstallationPOManagement.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    doTbt_InstallationPOManagement.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                }
                List<tbt_InstallationPOManagement> ListdoTbt_InstallationPOManagement = new List<tbt_InstallationPOManagement>();
                ListdoTbt_InstallationPOManagement.Add(doTbt_InstallationPOManagement);

                List<tbt_InstallationPOManagement> dtUpdatedTbt_InstallationPOManagement = base.UpdateTbt_InstallationPOManagement(CommonUtil.ConvertToXml_Store<tbt_InstallationPOManagement>(ListdoTbt_InstallationPOManagement));
                if (dtUpdatedTbt_InstallationPOManagement.Count > 0)
                {
                    ILogHandler hand2 = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    doTransactionLog InsertIntrumentDetailLog = new doTransactionLog();
                    InsertIntrumentDetailLog.TransactionType = doTransactionLog.eTransactionType.Update;
                    InsertIntrumentDetailLog.TableName = TableName.C_TBL_NAME_INS_PO_MA;
                    InsertIntrumentDetailLog.TableData = CommonUtil.ConvertToXml(dtUpdatedTbt_InstallationPOManagement);
                    hand2.WriteTransactionLog(InsertIntrumentDetailLog);
                    return dtUpdatedTbt_InstallationPOManagement.Count;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int UpdateTbt_InstallationManagement(tbt_InstallationManagement doTbt_InstallationManagement)
        {

            try
            {
                if (doTbt_InstallationManagement != null)
                {
                    doTbt_InstallationManagement.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    doTbt_InstallationManagement.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                }
                List<tbt_InstallationManagement> dtUpdatedTbt_InstallationManagement = base.UpdateTbt_InstallationManagement(doTbt_InstallationManagement.MaintenanceNo, doTbt_InstallationManagement.ContractProjectCode, doTbt_InstallationManagement.ManagementStatus, doTbt_InstallationManagement.ProposeInstallStartDate, doTbt_InstallationManagement.ProposeInstallCompleteDate
                                                                                                                            , doTbt_InstallationManagement.CustomerStaffBelonging, doTbt_InstallationManagement.CustomerStaffName, doTbt_InstallationManagement.CustomerStaffPhoneNo, doTbt_InstallationManagement.NewPhoneLineOpenDate, doTbt_InstallationManagement.NewConnectionPhoneNo
                                                                                                                            , doTbt_InstallationManagement.NewPhoneLineOwnerTypeCode, doTbt_InstallationManagement.IEStaffEmpNo1, doTbt_InstallationManagement.IEStaffEmpNo2, doTbt_InstallationManagement.IEManPower, doTbt_InstallationManagement.MaterialFee, doTbt_InstallationManagement.MaterialFeeUsd
                                                                                                                            , doTbt_InstallationManagement.MaterialFeeCurrencyType, doTbt_InstallationManagement.RequestMemo, doTbt_InstallationManagement.POMemo, doTbt_InstallationManagement.ChangeReasonCode, doTbt_InstallationManagement.ChangeReasonOther, doTbt_InstallationManagement.ChangeRequestorCode
                                                                                                                            , doTbt_InstallationManagement.ChangeRequestorOther, doTbt_InstallationManagement.NewBldMgmtFlag, doTbt_InstallationManagement.NewBldMgmtCost, doTbt_InstallationManagement.NewBldMgmtCostUsd, doTbt_InstallationManagement.NewBldMgmtCostCurrencyType, doTbt_InstallationManagement.CreateDate
                                                                                                                            , doTbt_InstallationManagement.CreateBy, doTbt_InstallationManagement.UpdateDate, doTbt_InstallationManagement.UpdateBy, doTbt_InstallationManagement.ApproveNo); //Add (ApproveNo) by Jutarat A. on 17042013
                if (dtUpdatedTbt_InstallationManagement.Count > 0)
                {
                    ILogHandler hand2 = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    doTransactionLog InsertIntrumentDetailLog = new doTransactionLog();
                    InsertIntrumentDetailLog.TransactionType = doTransactionLog.eTransactionType.Update;
                    InsertIntrumentDetailLog.TableName = TableName.C_TBL_NAME_INS_MA;
                    InsertIntrumentDetailLog.TableData = CommonUtil.ConvertToXml(dtUpdatedTbt_InstallationManagement);
                    hand2.WriteTransactionLog(InsertIntrumentDetailLog);
                    return dtUpdatedTbt_InstallationManagement.Count;
                }
                else
                {
                    return -1;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public tbt_InstallationSlip GetTbt_InstallationSlipData(string strSlipNo)
        {

            try
            {
                List<tbt_InstallationSlip> dtSelectedTbt_InstallationSlip = base.GetTbt_InstallationSlip(strSlipNo);
                if (dtSelectedTbt_InstallationSlip.Count > 0)
                {
                    return dtSelectedTbt_InstallationSlip[0];
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

        public List<tbt_InstallationInstrumentDetails> GetTbt_InstallationInstrumentDetailsData(string strContractCode, string strInstrumentCode)
        {

            try
            {
                List<tbt_InstallationInstrumentDetails> dtSelectedTbt_InstallationInstrumentDetails = base.GetTbt_InstallationInstrumentDetails(strContractCode, strInstrumentCode);
                if (dtSelectedTbt_InstallationInstrumentDetails.Count > 0)
                {
                    return dtSelectedTbt_InstallationInstrumentDetails;
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

        public int InsertTbt_InstallationSlip(tbt_InstallationSlip doTbt_InstallationSlip)
        {

            try
            {
                if (doTbt_InstallationSlip != null)
                {
                    doTbt_InstallationSlip.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    doTbt_InstallationSlip.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    doTbt_InstallationSlip.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    doTbt_InstallationSlip.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                }

                //List<tbt_InstallationSlip> dtInsertedTbt_InstallationSlip = base.InsertTbt_InstallationSlip(doTbt_InstallationSlip.SlipNo, doTbt_InstallationSlip.ServiceTypeCode, doTbt_InstallationSlip.SlipStatus, doTbt_InstallationSlip.ChangeReasonCode, doTbt_InstallationSlip.InstallationType, doTbt_InstallationSlip.PlanCode, doTbt_InstallationSlip.CauseReason, doTbt_InstallationSlip.NormalContractFee, doTbt_InstallationSlip.NormalInstallFee, doTbt_InstallationSlip.InstallFeeBillingType, doTbt_InstallationSlip.BillingInstallFee, doTbt_InstallationSlip.BillingOCC, doTbt_InstallationSlip.PreviousSlipNo, doTbt_InstallationSlip.PreviousSlipStatus, doTbt_InstallationSlip.ContractCode, doTbt_InstallationSlip.SlipIssueDate, doTbt_InstallationSlip.SlipIssueOfficeCode, doTbt_InstallationSlip.StockOutDate, doTbt_InstallationSlip.StockOutOfficeCode, doTbt_InstallationSlip.ReturnReceiveDate, doTbt_InstallationSlip.ReturnReceiveOfficeCode, doTbt_InstallationSlip.ApproveNo1, doTbt_InstallationSlip.ApproveNo2, doTbt_InstallationSlip.ChangeContents, doTbt_InstallationSlip.ExpectedInstrumentArrivalDate, doTbt_InstallationSlip.StockOutTypeCode, doTbt_InstallationSlip.SlipType, doTbt_InstallationSlip.CreateDate, doTbt_InstallationSlip.CreateBy, doTbt_InstallationSlip.UpdateDate, doTbt_InstallationSlip.UpdateBy, doTbt_InstallationSlip.AdditionalStockOutOfficeCode, doTbt_InstallationSlip.SlipIssueFlag, doTbt_InstallationSlip.UnremoveApproveNo);
                List<tbt_InstallationSlip> dtInsertedTbt_InstallationSlip = base.InsertTbt_InstallationSlip(doTbt_InstallationSlip.SlipNo, doTbt_InstallationSlip.ServiceTypeCode, doTbt_InstallationSlip.SlipStatus, doTbt_InstallationSlip.ChangeReasonCode, doTbt_InstallationSlip.InstallationType, doTbt_InstallationSlip.PlanCode
                                                                                                           , doTbt_InstallationSlip.CauseReason, doTbt_InstallationSlip.NormalContractFee, doTbt_InstallationSlip.NormalContractFeeUsd, doTbt_InstallationSlip.NormalContractFeeCurrencyType, doTbt_InstallationSlip.NormalInstallFee
                                                                                                           , doTbt_InstallationSlip.NormalInstallFeeUsd, doTbt_InstallationSlip.NormalInstallFeeCurrencyType, doTbt_InstallationSlip.InstallFeeBillingType, doTbt_InstallationSlip.BillingInstallFee, doTbt_InstallationSlip.BillingInstallFeeUsd
                                                                                                           , doTbt_InstallationSlip.BillingInstallFeeCurrencyType, doTbt_InstallationSlip.OrderInstallFee, doTbt_InstallationSlip.OrderInstallFeeUsd, doTbt_InstallationSlip.OrderInstallFeeCurrencyType, doTbt_InstallationSlip.BillingOCC
                                                                                                           , doTbt_InstallationSlip.PreviousSlipNo, doTbt_InstallationSlip.PreviousSlipStatus, doTbt_InstallationSlip.ContractCode, doTbt_InstallationSlip.SlipIssueDate, doTbt_InstallationSlip.SlipIssueOfficeCode, doTbt_InstallationSlip.StockOutDate
                                                                                                           , doTbt_InstallationSlip.StockOutOfficeCode, doTbt_InstallationSlip.ReturnReceiveDate, doTbt_InstallationSlip.ReturnReceiveOfficeCode, doTbt_InstallationSlip.ApproveNo1, doTbt_InstallationSlip.ApproveNo2, doTbt_InstallationSlip.ChangeContents
                                                                                                           , doTbt_InstallationSlip.ExpectedInstrumentArrivalDate, doTbt_InstallationSlip.StockOutTypeCode, doTbt_InstallationSlip.SlipType, doTbt_InstallationSlip.CreateDate, doTbt_InstallationSlip.CreateBy, doTbt_InstallationSlip.UpdateDate, doTbt_InstallationSlip.UpdateBy
                                                                                                           , doTbt_InstallationSlip.AdditionalStockOutOfficeCode, doTbt_InstallationSlip.SlipIssueFlag, doTbt_InstallationSlip.UnremoveApproveNo); //Modify by Jutarat A. on 11042013
                if (dtInsertedTbt_InstallationSlip.Count > 0)
                {
                    ILogHandler hand2 = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    doTransactionLog InsertIntrumentDetailLog = new doTransactionLog();
                    InsertIntrumentDetailLog.TransactionType = doTransactionLog.eTransactionType.Insert;
                    InsertIntrumentDetailLog.TableName = TableName.C_TBL_NAME_INS_SLIP;
                    InsertIntrumentDetailLog.TableData = CommonUtil.ConvertToXml(dtInsertedTbt_InstallationSlip);
                    hand2.WriteTransactionLog(InsertIntrumentDetailLog);
                    return dtInsertedTbt_InstallationSlip.Count;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int InsertTbt_InstallationSlipDetails(tbt_InstallationSlipDetails doTbt_InstallationSlipDetails)
        {

            try
            {
                if (doTbt_InstallationSlipDetails != null)
                {
                    doTbt_InstallationSlipDetails.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    doTbt_InstallationSlipDetails.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    doTbt_InstallationSlipDetails.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    doTbt_InstallationSlipDetails.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                }
                List<tbt_InstallationSlipDetails> dtInsertedTbt_InstallationSlipDetails = base.InsertTbt_InstallationSlipDetails(doTbt_InstallationSlipDetails.SlipNo, doTbt_InstallationSlipDetails.InstrumentCode,
                                                                                                                            doTbt_InstallationSlipDetails.InstrumentTypeCode, doTbt_InstallationSlipDetails.ContractInstalledQty,
                                                                                                                            doTbt_InstallationSlipDetails.CurrentStockOutQty, doTbt_InstallationSlipDetails.TotalStockOutQty,
                                                                                                                            doTbt_InstallationSlipDetails.AddInstalledQty, doTbt_InstallationSlipDetails.ReturnQty,
                                                                                                                            doTbt_InstallationSlipDetails.AddRemovedQty, doTbt_InstallationSlipDetails.NotInstalledQty,
                                                                                                                            doTbt_InstallationSlipDetails.MoveQty, doTbt_InstallationSlipDetails.MAExchangeQty,
                                                                                                                            doTbt_InstallationSlipDetails.UnremovableQty, doTbt_InstallationSlipDetails.ReturnRemoveQty,
                                                                                                                            doTbt_InstallationSlipDetails.InstrumentPrice, doTbt_InstallationSlipDetails.CreateDate,
                                                                                                                            doTbt_InstallationSlipDetails.CreateBy, doTbt_InstallationSlipDetails.UpdateDate,
                                                                                                                            doTbt_InstallationSlipDetails.UpdateBy, doTbt_InstallationSlipDetails.PartialStockOutQty);
                if (dtInsertedTbt_InstallationSlipDetails.Count > 0)
                {
                    ILogHandler hand2 = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    doTransactionLog InsertIntrumentDetailLog = new doTransactionLog();
                    InsertIntrumentDetailLog.TransactionType = doTransactionLog.eTransactionType.Insert;
                    InsertIntrumentDetailLog.TableName = TableName.C_TBL_NAME_INS_SLIP_DET;
                    InsertIntrumentDetailLog.TableData = CommonUtil.ConvertToXml(dtInsertedTbt_InstallationSlipDetails);
                    hand2.WriteTransactionLog(InsertIntrumentDetailLog);
                    return dtInsertedTbt_InstallationSlipDetails.Count;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int UpdateTbt_InstallationSlip(tbt_InstallationSlip doTbt_InstallationSlip)
        {
            try
            {
                if (doTbt_InstallationSlip != null)
                {
                    doTbt_InstallationSlip.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    doTbt_InstallationSlip.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                }

                //List<tbt_InstallationSlip> dtInsertedTbt_InstallationSlip = base.UpdateTbt_InstallationSlip(doTbt_InstallationSlip.SlipNo, doTbt_InstallationSlip.ServiceTypeCode, doTbt_InstallationSlip.SlipStatus, doTbt_InstallationSlip.ChangeReasonCode, doTbt_InstallationSlip.InstallationType, doTbt_InstallationSlip.PlanCode, doTbt_InstallationSlip.CauseReason, doTbt_InstallationSlip.NormalContractFee, doTbt_InstallationSlip.NormalInstallFee, doTbt_InstallationSlip.InstallFeeBillingType, doTbt_InstallationSlip.BillingInstallFee, doTbt_InstallationSlip.BillingOCC, doTbt_InstallationSlip.PreviousSlipNo, doTbt_InstallationSlip.PreviousSlipStatus, doTbt_InstallationSlip.ContractCode, doTbt_InstallationSlip.SlipIssueDate, doTbt_InstallationSlip.SlipIssueOfficeCode, doTbt_InstallationSlip.StockOutDate, doTbt_InstallationSlip.StockOutOfficeCode, doTbt_InstallationSlip.ReturnReceiveDate, doTbt_InstallationSlip.ReturnReceiveOfficeCode, doTbt_InstallationSlip.ApproveNo1, doTbt_InstallationSlip.ApproveNo2, doTbt_InstallationSlip.ChangeContents, doTbt_InstallationSlip.ExpectedInstrumentArrivalDate, doTbt_InstallationSlip.StockOutTypeCode, doTbt_InstallationSlip.SlipType, doTbt_InstallationSlip.CreateDate, doTbt_InstallationSlip.CreateBy, doTbt_InstallationSlip.UpdateDate, doTbt_InstallationSlip.UpdateBy, doTbt_InstallationSlip.AdditionalStockOutOfficeCode, doTbt_InstallationSlip.SlipIssueFlag, doTbt_InstallationSlip.UnremoveApproveNo);
                List<tbt_InstallationSlip> dtInsertedTbt_InstallationSlip = base.UpdateTbt_InstallationSlip(doTbt_InstallationSlip.SlipNo, doTbt_InstallationSlip.ServiceTypeCode, doTbt_InstallationSlip.SlipStatus, doTbt_InstallationSlip.ChangeReasonCode, doTbt_InstallationSlip.InstallationType, doTbt_InstallationSlip.PlanCode
                                                                                                           , doTbt_InstallationSlip.CauseReason, doTbt_InstallationSlip.NormalContractFee, doTbt_InstallationSlip.NormalContractFeeUsd, doTbt_InstallationSlip.NormalContractFeeCurrencyType, doTbt_InstallationSlip.NormalInstallFee
                                                                                                           , doTbt_InstallationSlip.NormalInstallFeeUsd, doTbt_InstallationSlip.NormalInstallFeeCurrencyType, doTbt_InstallationSlip.InstallFeeBillingType, doTbt_InstallationSlip.BillingInstallFee, doTbt_InstallationSlip.BillingInstallFeeUsd
                                                                                                           , doTbt_InstallationSlip.BillingInstallFeeCurrencyType, doTbt_InstallationSlip.OrderInstallFee, doTbt_InstallationSlip.OrderInstallFeeUsd, doTbt_InstallationSlip.OrderInstallFeeCurrencyType, doTbt_InstallationSlip.BillingOCC
                                                                                                           , doTbt_InstallationSlip.PreviousSlipNo, doTbt_InstallationSlip.PreviousSlipStatus, doTbt_InstallationSlip.ContractCode, doTbt_InstallationSlip.SlipIssueDate, doTbt_InstallationSlip.SlipIssueOfficeCode, doTbt_InstallationSlip.StockOutDate
                                                                                                           , doTbt_InstallationSlip.StockOutOfficeCode, doTbt_InstallationSlip.ReturnReceiveDate, doTbt_InstallationSlip.ReturnReceiveOfficeCode, doTbt_InstallationSlip.ApproveNo1, doTbt_InstallationSlip.ApproveNo2, doTbt_InstallationSlip.ChangeContents
                                                                                                           , doTbt_InstallationSlip.ExpectedInstrumentArrivalDate, doTbt_InstallationSlip.StockOutTypeCode, doTbt_InstallationSlip.SlipType, doTbt_InstallationSlip.CreateDate, doTbt_InstallationSlip.CreateBy, doTbt_InstallationSlip.UpdateDate, doTbt_InstallationSlip.UpdateBy
                                                                                                           , doTbt_InstallationSlip.AdditionalStockOutOfficeCode, doTbt_InstallationSlip.SlipIssueFlag, doTbt_InstallationSlip.UnremoveApproveNo); //Modify by Jutarat A. on 11042013
                if (dtInsertedTbt_InstallationSlip.Count > 0)
                {
                    ILogHandler hand2 = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    doTransactionLog InsertIntrumentDetailLog = new doTransactionLog();
                    InsertIntrumentDetailLog.TransactionType = doTransactionLog.eTransactionType.Update;
                    InsertIntrumentDetailLog.TableName = TableName.C_TBL_NAME_INS_SLIP;
                    InsertIntrumentDetailLog.TableData = CommonUtil.ConvertToXml(dtInsertedTbt_InstallationSlip);
                    hand2.WriteTransactionLog(InsertIntrumentDetailLog);
                    return dtInsertedTbt_InstallationSlip.Count;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<tbt_InstallationSlipDetails> GetTbt_InstallationSlipDetailsData(string strSlipNo, string strInstrumentCode, string strInstrumentTypeCode)
        {

            try
            {
                List<tbt_InstallationSlipDetails> dtSelectedTbt_InstallationSlipDetails = base.GetTbt_InstallationSlipDetails(strSlipNo, strInstrumentCode, strInstrumentTypeCode);
                if (dtSelectedTbt_InstallationSlipDetails.Count > 0)
                {
                    return dtSelectedTbt_InstallationSlipDetails;
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

        //Add by Jutarat A. on 27052013
        /// <summary>
        /// Get Sale Instrument data
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="OCC"></param>
        /// <param name="SlipNo"></param>
        /// <param name="InstrumentTypeCode"></param>
        /// <param name="ChangeType"></param>
        /// <returns></returns>
        public List<doSaleInstrumentdataList> GetSaleInstrumentdataList(string ContractCode, string OCC, string SlipNo, string InstrumentTypeCode, string ChangeType = null, bool? InstallCompleteFlag = null, string SaleInstallType = null, string strSaleProcessManageStatus = null)
        {
            try
            {
                return base.GetSaleInstrumentdataList(ContractCode, OCC, SlipNo, InstrumentTypeCode, ChangeType, InstallCompleteFlag, SaleInstallType, strSaleProcessManageStatus, SaleChangeType.C_SALE_CHANGE_TYPE_ADD_SALE, SaleChangeType.C_SALE_CHANGE_TYPE_NEW_SALE, SaleInstallationType.C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE, SaleProcessManageStatus.C_SALE_PROCESS_STATUS_CANCEL);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //End Add

        //Add by Jutarat A. on 17062013
        /// <summary>
        /// Get Rental Instrument data
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="OCC"></param>
        /// <param name="SlipNo"></param>
        /// <param name="InstrumentTypeCode"></param>
        /// <param name="RentalInstallType"></param>
        /// <returns></returns>
        public List<doRentalInstrumentdataList> GetRentalInstrumentdataList(string ContractCode, string OCC, string SlipNo, string InstrumentTypeCode, string RentalInstallType = null)
        {
            try
            {
                return base.GetRentalInstrumentdataList(ContractCode, OCC, SlipNo, InstrumentTypeCode, RentalInstallType, RentalInstallationType.C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE, RentalInstallationType.C_RENTAL_INSTALL_TYPE_PERIODICAL_MAINTENANCE);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //End Add

        public List<doRentalFeeResult> GetRentalFee(string strContractCode)
        {
            try
            {
                return base.GetRentalFee(strContractCode, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int UpdateTbt_InstallationSlipDetails(tbt_InstallationSlipDetails doTbt_InstallationSlipDetails)
        {

            try
            {
                if (doTbt_InstallationSlipDetails != null)
                {
                    doTbt_InstallationSlipDetails.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    doTbt_InstallationSlipDetails.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                }
                List<tbt_InstallationSlipDetails> dtUpdatedTbt_InstallationSlipDetails = base.UpdateTbt_InstallationSlipDetails(doTbt_InstallationSlipDetails.SlipNo, doTbt_InstallationSlipDetails.InstrumentCode,
                                                                                                                            doTbt_InstallationSlipDetails.InstrumentTypeCode, doTbt_InstallationSlipDetails.ContractInstalledQty,
                                                                                                                            doTbt_InstallationSlipDetails.CurrentStockOutQty, doTbt_InstallationSlipDetails.TotalStockOutQty,
                                                                                                                            doTbt_InstallationSlipDetails.AddInstalledQty, doTbt_InstallationSlipDetails.ReturnQty,
                                                                                                                            doTbt_InstallationSlipDetails.AddRemovedQty, doTbt_InstallationSlipDetails.NotInstalledQty,
                                                                                                                            doTbt_InstallationSlipDetails.MoveQty, doTbt_InstallationSlipDetails.MAExchangeQty,
                                                                                                                            doTbt_InstallationSlipDetails.UnremovableQty, doTbt_InstallationSlipDetails.ReturnRemoveQty,
                                                                                                                            doTbt_InstallationSlipDetails.InstrumentPrice, doTbt_InstallationSlipDetails.CreateDate,
                                                                                                                            doTbt_InstallationSlipDetails.CreateBy, doTbt_InstallationSlipDetails.UpdateDate,
                                                                                                                            doTbt_InstallationSlipDetails.UpdateBy, doTbt_InstallationSlipDetails.PartialStockOutQty);
                if (dtUpdatedTbt_InstallationSlipDetails.Count > 0)
                {
                    ILogHandler hand2 = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    doTransactionLog InsertIntrumentDetailLog = new doTransactionLog();
                    InsertIntrumentDetailLog.TransactionType = doTransactionLog.eTransactionType.Update;
                    InsertIntrumentDetailLog.TableName = TableName.C_TBL_NAME_INS_SLIP_DET;
                    InsertIntrumentDetailLog.TableData = CommonUtil.ConvertToXml(dtUpdatedTbt_InstallationSlipDetails);
                    hand2.WriteTransactionLog(InsertIntrumentDetailLog);
                    return dtUpdatedTbt_InstallationSlipDetails.Count;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<tbt_InstallationHistory> UpdateTbt_InstallationHistory(List<tbt_InstallationHistory> lstHistory)
        {
            try
            {
                if (lstHistory == null || lstHistory.Count <= 0)
                {
                    return lstHistory;
                }

                foreach (var tmp in lstHistory)
                {
                    tmp.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    tmp.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                }

                var lst = base.UpdateTbt_InstallationHistory(CommonUtil.ConvertToXml_Store<tbt_InstallationHistory>(lstHistory));

                #region Log

                if (lst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Update,
                        TableName = TableName.C_TBL_NAME_INS_HIS,
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

        public List<tbt_InstallationInstrumentDetails> DeleteTbt_InstallationInstrumentDetail(string strContractCode, string strInstrumentCode)
        {
            try
            {
                //Delete data from DB
                List<tbt_InstallationInstrumentDetails> deletedList = base.DeleteTbt_InstallationInstrumentDetail(strContractCode, strInstrumentCode);

                //Insert Log
                if (deletedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                    logData.TableName = TableName.C_TBL_NAME_INS_INST;
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

        public List<tbt_InstallationBasic> DeleteTbt_InstallationBasic(string strContractProjectCode)
        {
            try
            {
                //Delete data from DB
                List<tbt_InstallationBasic> deletedList = base.DeleteTbt_InstallationBasic(strContractProjectCode);

                //Insert Log
                if (deletedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                    logData.TableName = TableName.C_TBL_NAME_INS_BASIC;
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

        public List<tbt_InstallationHistory> InsertTbt_InstallationHistory(tbt_InstallationHistory doTbt_InstallationHistory)
        {

            try
            {
                if (doTbt_InstallationHistory != null)
                {
                    doTbt_InstallationHistory.CreateDate = (doTbt_InstallationHistory.CreateDate ?? CommonUtil.dsTransData.dtOperationData.ProcessDateTime);
                    doTbt_InstallationHistory.CreateBy = (doTbt_InstallationHistory.CreateBy ?? CommonUtil.dsTransData.dtUserData.EmpNo);
                    doTbt_InstallationHistory.UpdateDate = (doTbt_InstallationHistory.UpdateDate ?? CommonUtil.dsTransData.dtOperationData.ProcessDateTime);
                    doTbt_InstallationHistory.UpdateBy = (doTbt_InstallationHistory.UpdateBy ?? CommonUtil.dsTransData.dtUserData.EmpNo);
                }
                List<tbt_InstallationHistory> dtInsertedTbt_InstallationHistory = base.InsertTbt_InstallationHistory(doTbt_InstallationHistory.ContractProjectCode
                                                                                                                    , doTbt_InstallationHistory.OCC
                                                                                                                    , doTbt_InstallationHistory.ServiceTypeCode
                                                                                                                    , doTbt_InstallationHistory.InstallationStatus
                                                                                                                    , doTbt_InstallationHistory.InstallationType
                                                                                                                    , doTbt_InstallationHistory.PlanCode
                                                                                                                    , doTbt_InstallationHistory.SlipNo
                                                                                                                    , doTbt_InstallationHistory.MaintenanceNo
                                                                                                                    , doTbt_InstallationHistory.OperationOfficeCode
                                                                                                                    , doTbt_InstallationHistory.SecurityTypeCode
                                                                                                                    , doTbt_InstallationHistory.ChangeReasonTypeCode
                                                                                                                    , doTbt_InstallationHistory.NormalInstallFee
                                                                                                                    , doTbt_InstallationHistory.BillingInstallFee
                                                                                                                    , doTbt_InstallationHistory.InstallFeeBillingType
                                                                                                                    , doTbt_InstallationHistory.NormalSaleProductPrice
                                                                                                                    , doTbt_InstallationHistory.BillingSalePrice
                                                                                                                    , doTbt_InstallationHistory.InstallationSlipProcessingDate
                                                                                                                    , doTbt_InstallationHistory.InstallationCompleteDate
                                                                                                                    , doTbt_InstallationHistory.InstallationCompleteProcessingDate
                                                                                                                    , doTbt_InstallationHistory.InstallationBy
                                                                                                                    , doTbt_InstallationHistory.SalesmanEmpNo1
                                                                                                                    , doTbt_InstallationHistory.SalesmanEmpNo2
                                                                                                                    , doTbt_InstallationHistory.ApproveNo1
                                                                                                                    , doTbt_InstallationHistory.ApproveNo2
                                                                                                                    , doTbt_InstallationHistory.InstallationStartDate
                                                                                                                    , doTbt_InstallationHistory.InstallationFinishDate
                                                                                                                    , doTbt_InstallationHistory.NormalContractFee
                                                                                                                    , doTbt_InstallationHistory.BillingOCC
                                                                                                                    , doTbt_InstallationHistory.CreateDate
                                                                                                                    , doTbt_InstallationHistory.CreateBy
                                                                                                                    , doTbt_InstallationHistory.UpdateDate
                                                                                                                    , doTbt_InstallationHistory.UpdateBy

                                                                                                                    );
                if (dtInsertedTbt_InstallationHistory.Count > 0)
                {
                    ILogHandler hand2 = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    doTransactionLog InsertIntrumentDetailLog = new doTransactionLog();
                    InsertIntrumentDetailLog.TransactionType = doTransactionLog.eTransactionType.Insert;
                    InsertIntrumentDetailLog.TableName = TableName.C_TBL_NAME_INS_HIS;
                    InsertIntrumentDetailLog.TableData = CommonUtil.ConvertToXml(dtInsertedTbt_InstallationHistory);
                    hand2.WriteTransactionLog(InsertIntrumentDetailLog);
                }

                return dtInsertedTbt_InstallationHistory;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int InsertTbt_InstallationHistoryDetail(tbt_InstallationHistoryDetails doTbt_InstallationHistoryDetail)
        {

            try
            {
                if (doTbt_InstallationHistoryDetail != null)
                {
                    doTbt_InstallationHistoryDetail.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    doTbt_InstallationHistoryDetail.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    doTbt_InstallationHistoryDetail.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    doTbt_InstallationHistoryDetail.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                }
                List<tbt_InstallationHistoryDetails> dtInsertedTbt_InstallationHistoryDetail = base.InsertTbt_InstallationHistoryDetail(doTbt_InstallationHistoryDetail.ContractCode
                                                                                                                        , doTbt_InstallationHistoryDetail.InstrumentCode
                                                                                                                        , doTbt_InstallationHistoryDetail.InstrumentTypeCode
                                                                                                                        , doTbt_InstallationHistoryDetail.ContractInstalledQty
                                                                                                                        , doTbt_InstallationHistoryDetail.ContractRemovedQty
                                                                                                                        , doTbt_InstallationHistoryDetail.ContractMovedQty
                                                                                                                        , doTbt_InstallationHistoryDetail.CreateDate
                                                                                                                        , doTbt_InstallationHistoryDetail.CreateBy
                                                                                                                        , doTbt_InstallationHistoryDetail.UpdateDate
                                                                                                                        , doTbt_InstallationHistoryDetail.UpdateBy
                                                                                                                    );
                if (dtInsertedTbt_InstallationHistoryDetail.Count > 0)
                {
                    ILogHandler hand2 = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    doTransactionLog InsertIntrumentDetailLog = new doTransactionLog();
                    InsertIntrumentDetailLog.TransactionType = doTransactionLog.eTransactionType.Insert;
                    InsertIntrumentDetailLog.TableName = TableName.C_TBL_NAME_INS_HIS_DET;
                    InsertIntrumentDetailLog.TableData = CommonUtil.ConvertToXml(dtInsertedTbt_InstallationHistoryDetail);
                    hand2.WriteTransactionLog(InsertIntrumentDetailLog);
                    return dtInsertedTbt_InstallationHistoryDetail.Count;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<tbt_InstallationMemo> DeleteTbt_InstallationMemo(string strContractProjectCode, string strReferenceID, string strObjectID)
        {
            try
            {
                //Delete data from DB
                List<tbt_InstallationMemo> deletedList = base.DeleteTbt_InstallationMemo(strContractProjectCode, strReferenceID, strObjectID);

                //Insert Log
                if (deletedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                    logData.TableName = TableName.C_TBL_NAME_INS_MEMO;
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

        public List<tbt_InstallationPOManagement> DeleteTbt_InstallationPOManagement(string strMaintenanceNo)
        {
            try
            {
                //Delete data from DB
                List<tbt_InstallationPOManagement> deletedList = base.DeleteTbt_InstallationPOManagement(strMaintenanceNo);

                //Insert Log
                if (deletedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                    logData.TableName = TableName.C_TBL_NAME_INS_PO_MA;
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

        public List<doSearchInstallManagementResult> SearchInstallationManagementList(doSearchInstallManageCriteria doCondition)
        {

            try
            {
                List<doSearchInstallManagementResult> dtSearch = base.SearchInstallationManagementList(
                    "1",
                    InstallationManagementStatus.C_INSTALL_MANAGE_STATUS_CANCELED,
                    doCondition.ContractCode,
                    doCondition.ProjectCode,
                    doCondition.InstallationType,
                    doCondition.IEStaffCode,
                    doCondition.SubcontractorCode,
                    doCondition.SubcontractorGroupName,
                    doCondition.ProposedInstallationCompleteDateFrom,
                    doCondition.ProposedInstallationCompleteDateTo,
                    doCondition.InstallationCompleteDateFrom,
                    doCondition.InstallationCompleteDateTo,
                    doCondition.InstallationStartDateFrom,
                    doCondition.InstallationStartDateTo,
                    doCondition.InstallationFinishDateFrom,
                    doCondition.InstallationFinishDateTo,
                    doCondition.SiteName,
                    doCondition.SiteAddress,
                    doCondition.OperationOfficeCode,
                    doCondition.InstallationManagementStatus,
                    doCondition.InstallationRequestDateFrom,
                    doCondition.InstallationRequestDateTo,
                    doCondition.ExpectedInstallationStartDateFrom,
                    doCondition.ExpectedInstallationStartDateTo,
                    doCondition.ExpectedInstallationFinishDateFrom,
                    doCondition.ExpectedInstallationFinishDateTo
                ); //Modify (Add InstallationRequestDateFrom, InstallationRequestDateTo) by Jutarat A. on 22102013
                if (dtSearch.Count > 0)
                {
                    return dtSearch;
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

        public List<dtRequestApproveInstallation> GetEmailForApprove()
        {

            try
            {
                List<dtRequestApproveInstallation> dtSelected = base.GetEmailForApprove();
                if (dtSelected.Count > 0)
                {
                    return dtSelected;
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

        public bool CheckCancelInstallationManagement(string strMaintenanceNo)
        {
            try
            {

                if (strMaintenanceNo != null)
                {
                    List<CheckCancelInstallationManagement_Result> result = base.CheckCancelInstallationManagement(strMaintenanceNo, InstallationManagementStatus.C_INSTALL_MANAGE_STATUS_CANCELED);
                    if (result.Count > 0)
                    {
                        return Convert.ToBoolean(result[0].blnCanceled);
                    }
                    else
                    {
                        return false;

                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<tbt_InstallationMemo> GetTbt_InstallationMemo(string strMaintenanceNo)
        {

            try
            {
                List<tbt_InstallationMemo> dtSelected = base.GetTbt_InstallationMemo(strMaintenanceNo);
                if (dtSelected.Count > 0)
                {
                    return dtSelected;
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

        public bool DeleteInstallationBasicData(string strContractCode)
        {
            try
            {
                if (CommonUtil.IsNullOrEmpty(strContractCode))
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "Contract code" });
                }
                if (strContractCode != null)
                {
                    List<tbt_InstallationBasic> doTbt_IntstallationBasic = base.GetTbt_InstallationBasic(strContractCode);
                    if (doTbt_IntstallationBasic != null && doTbt_IntstallationBasic.Count > 0)
                    {
                        if (doTbt_IntstallationBasic[0].InstallationStatus == InstallationStatus.C_INSTALL_STATUS_INSTALL_NOT_REGISTERED)
                        {
                            List<tbt_InstallationInstrumentDetails> doOutTbt_InstallationInstrumentDetails = DeleteTbt_InstallationInstrumentDetail(strContractCode, null);
                            List<tbt_InstallationBasic> doOutTbt_InstallationBasic = DeleteTbt_InstallationBasic(strContractCode);
                            //throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5058);
                        }

                    }
                    else
                    {
                        return false;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ReceiveReturnInstrument(string strSlipNo, string strReturnOfficecode)
        {
            try
            {
                if (CommonUtil.IsNullOrEmpty(strSlipNo))
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "ActionTypeCode" });
                }
                if (CommonUtil.IsNullOrEmpty(strReturnOfficecode))
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "Return office code" });
                }
                List<tbt_InstallationSlip> slipData = base.GetTbt_InstallationSlip(strSlipNo);
                if (slipData != null)
                {
                    slipData[0].SlipStatus = SlipStatus.C_SLIP_STATUS_RETURNED;
                    slipData[0].ReturnReceiveDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    slipData[0].ReturnReceiveOfficeCode = strReturnOfficecode;
                    int updatedRow = UpdateTbt_InstallationSlip(slipData[0]);
                    if (updatedRow > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateStockOutInstrument(string strSlipNo, bool blnStockOutFlag, string strStockOutOfficeCode, List<doInstrument> doInstrumentlist, DateTime? InstallSlipUpdateDate = null) //Modify by Jutarat A. on 27112012 (Add InstallSlipUpdateDate)
        {
            try
            {
                if (CommonUtil.IsNullOrEmpty(strSlipNo))
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "Slip no" });
                }
                if (CommonUtil.IsNullOrEmpty(blnStockOutFlag))
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "Stock out flag" });
                }
                if (CommonUtil.IsNullOrEmpty(strStockOutOfficeCode))
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "Stock out office code" });
                }
                if (blnStockOutFlag == FlagType.C_FLAG_ON)
                {
                    List<tbt_InstallationSlip> slipData = base.GetTbt_InstallationSlip(strSlipNo);
                    if (slipData != null)
                    {
                        slipData[0].SlipStatus = SlipStatus.C_SLIP_STATUS_STOCK_OUT;
                        slipData[0].StockOutDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        slipData[0].StockOutOfficeCode = strStockOutOfficeCode;

                        //Modify by Jutarat A. on 27112012
                        //slipData[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        //slipData[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        if (slipData.Count > 0 && InstallSlipUpdateDate != null)
                        {
                            if (DateTime.Compare(slipData[0].UpdateDate.GetValueOrDefault(), InstallSlipUpdateDate.GetValueOrDefault()) != 0)
                            {
                                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                            }
                        }
                        //End Modify

                        int updatedRow = UpdateTbt_InstallationSlip(slipData[0]);
                        if (updatedRow > 0)
                        {
                            foreach (doInstrument InstrumentData in doInstrumentlist)
                            {
                                List<tbt_InstallationSlipDetails> slipDetails = GetTbt_InstallationSlipDetails(strSlipNo, InstrumentData.InstrumentCode, null);
                                if (slipDetails != null)
                                {
                                    // change req 24/04/2012
                                    //slipDetails[0].PartialStockOutQty = 0;
                                    slipDetails[0].TotalStockOutQty = slipDetails[0].TotalStockOutQty + slipDetails[0].CurrentStockOutQty + InstrumentData.StockOutQty;
                                    slipDetails[0].CurrentStockOutQty = 0;
                                    slipDetails[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                    slipDetails[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                                    int updatedRow2 = UpdateTbt_InstallationSlipDetails(slipDetails[0]);
                                }
                            }

                            //============== Teerapong S. 14/08/2011 =================
                            List<tbt_InstallationSlipDetails> doTbt_InstallationSlipDetails = base.GetTbt_InstallationSlipDetails(slipData[0].SlipNo, null, null);
                            if (doTbt_InstallationSlipDetails != null && doTbt_InstallationSlipDetails.Count > 0)
                            {
                                foreach (tbt_InstallationSlipDetails slipDetail in doTbt_InstallationSlipDetails)
                                {
                                    if (slipDetail.CurrentStockOutQty > 0)
                                    {
                                        int? SumTotalStockOutQty = (slipDetail.TotalStockOutQty == null ? 0 : slipDetail.TotalStockOutQty) + (slipDetail.CurrentStockOutQty == null ? 0 : slipDetail.CurrentStockOutQty);
                                        slipDetail.TotalStockOutQty = SumTotalStockOutQty;
                                        slipDetail.CurrentStockOutQty = 0;
                                        UpdateTbt_InstallationSlipDetails(slipDetail);
                                    }
                                }
                            }
                            //========================================================

                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (blnStockOutFlag == FlagType.C_FLAG_OFF)
                {
                    List<tbt_InstallationSlip> slipData = base.GetTbt_InstallationSlip(strSlipNo);
                    if (slipData != null)
                    {
                        slipData[0].SlipStatus = SlipStatus.C_SLIP_STATUS_PARTIAL_STOCK_OUT;
                        slipData[0].StockOutDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        slipData[0].StockOutOfficeCode = strStockOutOfficeCode;

                        //Modify by Jutarat A. on 03122012
                        //slipData[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        //slipData[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        if (slipData.Count > 0 && InstallSlipUpdateDate != null)
                        {
                            if (DateTime.Compare(slipData[0].UpdateDate.GetValueOrDefault(), InstallSlipUpdateDate.GetValueOrDefault()) != 0)
                            {
                                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                            }
                        }
                        //End Modify

                        int updatedRow = UpdateTbt_InstallationSlip(slipData[0]);
                        if (updatedRow > 0)
                        {
                            //for (int i = 0; i < InstrumentCode.Length; i++)
                            //{
                            //    List<tbt_InstallationSlipDetails> slipDetails = GetTbt_InstallationSlipDetails(strSlipNo, InstrumentCode[i]);
                            //    if (slipDetails != null)
                            //    {
                            //        slipDetails[0].PartialStockOutQty = slipDetails[0].PartialStockOutQty + StockOutQty[i];                                
                            //        int updatedRow2 = UpdateTbt_InstallationSlipDetails(slipDetails[0]);
                            //    }
                            //}
                            foreach (doInstrument InstrumentData in doInstrumentlist)
                            {
                                List<tbt_InstallationSlipDetails> slipDetails = GetTbt_InstallationSlipDetails(strSlipNo, InstrumentData.InstrumentCode, null);
                                if (slipDetails != null)
                                {
                                    // change req 24/04/2012
                                    //slipDetails[0].PartialStockOutQty = slipDetails[0].PartialStockOutQty + InstrumentData.StockOutQty;
                                    slipDetails[0].CurrentStockOutQty = slipDetails[0].CurrentStockOutQty + InstrumentData.StockOutQty;

                                    slipDetails[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                    slipDetails[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                                    int updatedRow2 = UpdateTbt_InstallationSlipDetails(slipDetails[0]);
                                }
                            }
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool GenerateInstallationSlipDoc(string strContractCode, bool? isCheckSlipIssueFlag = true) //Add isCheckSlipIssueFlag by Jutarat A. on 25072013
        {
            try
            {
                IInstallationDocumentHandler docHand = ServiceContainer.GetService<IInstallationDocumentHandler>() as IInstallationDocumentHandler;

                if (CommonUtil.IsNullOrEmpty(strContractCode))
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "Contract code" });
                }
                List<tbt_InstallationBasic> dataBasic = base.GetTbt_InstallationBasic(strContractCode);
                if (dataBasic == null || dataBasic.Count <= 0 || dataBasic[0].SlipNo == null)
                {
                    return true;
                }

                List<tbt_InstallationSlip> dataSlip = base.GetTbt_InstallationSlip(dataBasic[0].SlipNo);

                //if (dataSlip[0].SlipStatus != SlipStatus.C_SLIP_STATUS_NO_NEED_TO_STOCK_OUT && dataSlip[0].SlipStatus != SlipStatus.C_SLIP_STATUS_NOT_STOCK_OUT)
                if (dataSlip != null && dataSlip.Count > 0
                        && dataSlip[0].SlipStatus != SlipStatus.C_SLIP_STATUS_NO_NEED_TO_STOCK_OUT
                        && dataSlip[0].SlipStatus != SlipStatus.C_SLIP_STATUS_NOT_STOCK_OUT //Modify by Jutarat A. on 07022013
                        && dataSlip[0].SlipStatus != SlipStatus.C_SLIP_STATUS_PARTIAL_STOCK_OUT //Add by Jutarat A. on 17062013
                        && dataSlip[0].SlipStatus != SlipStatus.C_SLIP_STATUS_STOCK_OUT) //Add by Jutarat A. on 24062013
                {
                    return true;
                }

                //Remove comment by Jutarat A. on 24072013
                ////Add by Phoomsak L. 2012-10-11 remove condition
                if (isCheckSlipIssueFlag == true) //Add by Jutarat A. on 25072013
                {
                    if (dataSlip[0].SlipIssueFlag != FlagType.C_FLAG_OFF)
                    {
                        return true;
                    }
                }

                if (dataBasic[0].InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_NEW || dataBasic[0].InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_CHANGEPLAN_NEW)
                {
                    //6.1.1.	Call		InstallationDocumentHandler.CreateISR010
                    //Parameter	doTbt_InstallationSlip.SlipNo
                    //Return		fsPDFFileISR010
                    docHand.CreateInstallationReport(dataBasic[0].SlipNo, DocumentCode.C_DOCUMENT_CODE_NEW_INSTALL_SLIP_RENTAL);
                }

                if (dataBasic[0].InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_CHANGE_WIRING ||
                    dataBasic[0].InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_CHANGEPLAN_AFTER_NEW ||
                    dataBasic[0].InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE ||
                    dataBasic[0].InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_MOVE ||
                    dataBasic[0].InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_PERIODICAL_MAINTENANCE ||
                    dataBasic[0].InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_PARTIAL ||
                    dataBasic[0].InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL ||
                    dataBasic[0].InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_CHANGE_WIRING ||
                    dataBasic[0].InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE ||
                    dataBasic[0].InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_MOVE ||
                    dataBasic[0].InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_PARTIAL_REMOVE
                    )
                {
                    //6.2.1.	Call		InstallationDocumentHandler.CreateISR020
                    //Parameter	doTbt_InstallationSlip.SlipNo
                    //Return		fsPDFFileISR020
                    docHand.CreateInstallationReport(dataBasic[0].SlipNo, DocumentCode.C_DOCUMENT_CODE_CHANGE_INSTALL_SLIP);
                }
                if (dataBasic[0].InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_ALL ||
                    dataBasic[0].InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_REMOVE_ALL)
                {
                    //6.3.1.	Call		InstallationDocumentHandler.CreateISR030
                    //Parameter	doTbt_InstallationSlip.SlipNo
                    //Return		fsPDFFileISR030
                    docHand.CreateInstallationReport(dataBasic[0].SlipNo, DocumentCode.C_DOCUMENT_CODE_REMOVAL_INSTALL_SLIP);
                }
                if (dataBasic[0].InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_NEW ||
                    dataBasic[0].InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_ADD)
                {
                    //6.4.1.	Call		InstallationDocumentHandler.CreateISR040
                    //Parameter	doTbt_InstallationSlip.SlipNo
                    //Return		fsPDFFileISR040
                    docHand.CreateInstallationReport(dataBasic[0].SlipNo, DocumentCode.C_DOCUMENT_CODE_NEW_INSTALL_SLIP_SALE);
                }

                //-- Add by Phoomsak L. 2012-10-11 add condition
                //if (dataSlip[0].SlipIssueFlag != FlagType.C_FLAG_ON) 
                if (dataSlip != null && dataSlip.Count > 0 && dataSlip[0].SlipIssueFlag != FlagType.C_FLAG_ON) //Modify by Jutarat A. on 07022013
                {
                    dataSlip[0].SlipIssueFlag = FlagType.C_FLAG_ON;
                    int updatedRpw = UpdateTbt_InstallationSlip(dataSlip[0]);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int InsertTbt_InstallationInstrumentDetails(tbt_InstallationInstrumentDetails doTbt_InstallationInstrumentDetails)
        {

            try
            {
                if (doTbt_InstallationInstrumentDetails != null)
                {
                    doTbt_InstallationInstrumentDetails.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    doTbt_InstallationInstrumentDetails.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    doTbt_InstallationInstrumentDetails.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    doTbt_InstallationInstrumentDetails.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                }
                List<tbt_InstallationInstrumentDetails> dtInsertedTbt_InstallationInstrumentDetails = base.InsertTbt_InstallationInstrumentDetails(doTbt_InstallationInstrumentDetails.ContractCode
                                                                                                                    , doTbt_InstallationInstrumentDetails.InstrumentCode, doTbt_InstallationInstrumentDetails.InstrumentTypeCode
                                                                                                                    , doTbt_InstallationInstrumentDetails.ContractInstalledQty, doTbt_InstallationInstrumentDetails.ContractRemovedQty
                                                                                                                    , doTbt_InstallationInstrumentDetails.ContractMovedQty, doTbt_InstallationInstrumentDetails.CreateDate, doTbt_InstallationInstrumentDetails.CreateBy, doTbt_InstallationInstrumentDetails.UpdateDate
                                                                                                                    , doTbt_InstallationInstrumentDetails.UpdateBy
                                                                                                                    );
                if (dtInsertedTbt_InstallationInstrumentDetails.Count > 0)
                {
                    ILogHandler hand2 = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    doTransactionLog InsertIntrumentDetailLog = new doTransactionLog();
                    InsertIntrumentDetailLog.TransactionType = doTransactionLog.eTransactionType.Insert;
                    InsertIntrumentDetailLog.TableName = TableName.C_TBL_NAME_INS_INST;
                    InsertIntrumentDetailLog.TableData = CommonUtil.ConvertToXml(dtInsertedTbt_InstallationInstrumentDetails);
                    hand2.WriteTransactionLog(InsertIntrumentDetailLog);
                    return dtInsertedTbt_InstallationInstrumentDetails.Count;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool GenerateInstallationBasic(doGenInstallationBasic doGenInstallationBasicData)
        {
            try
            {
                if (CommonUtil.IsNullOrEmpty(doGenInstallationBasicData) || CommonUtil.IsNullOrEmpty(doGenInstallationBasicData.ContractProjectCode))
                {
                    string LabelContractProjectCode = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_INSTALLATION, ScreenID.C_SCREEN_ID_INSTALL_SLIP, "lblContractProjectCode");
                    //Throw Error
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { LabelContractProjectCode });
                }

                tbt_InstallationBasic dataBasic = new tbt_InstallationBasic();
                dataBasic.ContractProjectCode = doGenInstallationBasicData.ContractProjectCode;
                dataBasic.OCC = doGenInstallationBasicData.OCC;
                dataBasic.ServiceTypeCode = doGenInstallationBasicData.ServiceTypeCode;
                dataBasic.InstallationStatus = InstallationStatus.C_INSTALL_STATUS_INSTALL_NOT_REGISTERED;
                dataBasic.InstallationType = doGenInstallationBasicData.InstallationType;
                dataBasic.OperationOfficeCode = doGenInstallationBasicData.OperationOfficeCode;
                dataBasic.SecurityTypeCode = doGenInstallationBasicData.SecurityTypeCode;
                dataBasic.NormalInstallFee = doGenInstallationBasicData.NormalInstallFee;
                dataBasic.ApproveNo1 = doGenInstallationBasicData.ApproveNo1;
                dataBasic.ApproveNo2 = doGenInstallationBasicData.ApproveNo2;
                if (doGenInstallationBasicData.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                {
                    dataBasic.NormalContractFee = doGenInstallationBasicData.NormalContractFee;
                }
                else if (doGenInstallationBasicData.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                {
                    dataBasic.NormalSaleProductPrice = doGenInstallationBasicData.NormalContractFee;
                }

                dataBasic.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                dataBasic.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                dataBasic.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                dataBasic.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                int InsertedCount = InsertTbt_InstallationBasic(dataBasic);

                foreach (tbt_InstallationInstrumentDetails instrumentDetail in doGenInstallationBasicData.doInstrumentDetails)
                {
                    InsertTbt_InstallationInstrumentDetails(instrumentDetail);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetInstallationStatus(string strContractProjectCode)
        {

            try
            {
                List<tbt_InstallationBasic> doTbt_InstallationBasic = base.GetTbt_InstallationBasic(strContractProjectCode);
                if (doTbt_InstallationBasic.Count > 0)
                {
                    return doTbt_InstallationBasic[0].InstallationStatus;
                }
                else
                {
                    return InstallationStatus.C_INSTALL_STATUS_NO_INSTALLATION;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string CheckInstallationDataToOpenScreenData(string strContractProjectCode)
        {
            try
            {
                List<doCheckInstallationDataToOpenScreen> doGetCheckData = base.CheckInstallationDataToOpenScreen(strContractProjectCode);
                if (doGetCheckData.Count > 0)
                {
                    return doGetCheckData[0].strInstallationMaintenanceNo;
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

        public int InsertTbt_InstallationAttachFile(tbt_InstallationAttachFile doTbt_AttachFile)
        {
            try
            {
                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                if (doTbt_AttachFile != null)
                {
                    doTbt_AttachFile.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    doTbt_AttachFile.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    doTbt_AttachFile.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    doTbt_AttachFile.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                }
                List<tbt_InstallationAttachFile> doInsertList = new List<tbt_InstallationAttachFile>();
                doInsertList.Add(doTbt_AttachFile);
                List<tbt_InstallationAttachFile> insertList = base.InsertTbt_InstallationAttachFile(CommonUtil.ConvertToXml_Store<tbt_InstallationAttachFile>(doInsertList));

                //Insert Log
                if (insertList.Count > 0)
                {

                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_INS_ATTH_FILE;
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

        public decimal GetNormalRemovalFee(string strContractCode)
        {
            try
            {
                List<doGetNormalRemovalFee> doRemovalFeeList = base.GetNormalRemovalFee(strContractCode, RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_ALL, SlipStatus.C_SLIP_STATUS_INSTALL_SLIP_CANCELED, SlipStatus.C_SLIP_STATUS_REPLACED);
                if (doRemovalFeeList.Count > 0)
                {
                    return (decimal)doRemovalFeeList[0].NormalInstallFee;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<tbt_InstallationAttachFile> DeleteTbt_InstallationAttachFile(Nullable<int> AttachFileID, string MaintenanceNo, string ObjectID)
        {
            try
            {
                //Delete data from DB
                List<tbt_InstallationAttachFile> deletedList = base.DeleteTbt_InstallationAttachFile(AttachFileID, MaintenanceNo, ObjectID);

                //Insert Log
                if (deletedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                    logData.TableName = TableName.C_TBL_NAME_INS_ATTH_FILE;
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

        public List<tbt_InstallationAttachFile> GetTbt_InstallationAttachFile(Nullable<int> AttachFileID, string MaintenanceNo, string ObjectID)
        {

            try
            {
                List<tbt_InstallationAttachFile> dtSelected = base.GetTbt_InstallationAttachFile(AttachFileID, MaintenanceNo, ObjectID);
                if (dtSelected.Count > 0)
                {
                    return dtSelected;
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

        public List<doInstallationDetailForCompleteInstallation> GetInstallationDetailForCompleteInstallation(string strInstallationSlipNo)
        {
            return base.GetInstallationDetailForCompleteInstallation(InstrumentType.C_INST_TYPE_GENERAL, strInstallationSlipNo);
        }

        public int InsertTbt_InstallationSlipExpansion(tbt_InstallationSlipExpansion doTbt_InstallationSlipExpansion)
        {

            try
            {
                if (doTbt_InstallationSlipExpansion != null)
                {
                    doTbt_InstallationSlipExpansion.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    doTbt_InstallationSlipExpansion.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    doTbt_InstallationSlipExpansion.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    doTbt_InstallationSlipExpansion.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                }

                List<tbt_InstallationSlipExpansion> ListdoTbt_InstallationSlipExpansion = new List<tbt_InstallationSlipExpansion>();
                ListdoTbt_InstallationSlipExpansion.Add(doTbt_InstallationSlipExpansion);

                List<tbt_InstallationSlipExpansion> res = this.InsertTbt_InstallationSlipExpansion(CommonUtil.ConvertToXml_Store<tbt_InstallationSlipExpansion>(ListdoTbt_InstallationSlipExpansion));

                if (res.Count > 0)
                {
                    ILogHandler hand2 = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    doTransactionLog InsertIntrumentDetailLog = new doTransactionLog();
                    InsertIntrumentDetailLog.TransactionType = doTransactionLog.eTransactionType.Insert;
                    InsertIntrumentDetailLog.TableName = TableName.C_TBL_NAME_INS_SLIP_DET;
                    InsertIntrumentDetailLog.TableData = CommonUtil.ConvertToXml(res);
                    hand2.WriteTransactionLog(InsertIntrumentDetailLog);
                    return res.Count;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Temp_CompleteInstallation_Rental(string RentalContractCode)
        {
            try
            {
                List<doPrepareCompleteInstallationData> doPrepareInstallComplete = base.Temp_CompleteInstallation_Rental(RentalContractCode);
                IContractHandler cHandler = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
                doCompleteInstallationData doComplete = new doCompleteInstallationData();
                if (doPrepareInstallComplete != null && doPrepareInstallComplete.Count > 0)
                {
                    if (doPrepareInstallComplete.Count > 0)
                    {
                        if (doPrepareInstallComplete[0].OrderInstallFee != null)
                            doComplete.BillingInstallationFee = (decimal)doPrepareInstallComplete[0].OrderInstallFee;
                        doComplete.BillingOCC = null;
                        doComplete.CompleteInstallationProcessFlag = true;
                        doComplete.ContractCode = doPrepareInstallComplete[0].ContractCode;
                        doComplete.IEInchargeEmpNo = doPrepareInstallComplete[0].IEInchargeNo;
                        doComplete.InstallationCompleteDate = (DateTime)doPrepareInstallComplete[0].InstallationCompleteDate;
                        doComplete.InstallationCompleteProcessDate = (DateTime)doPrepareInstallComplete[0].InstallationCompleteProcessDate;
                        doComplete.InstallationMemo = doPrepareInstallComplete[0].InstallationMemo;
                        doComplete.InstallationSlipNo = doPrepareInstallComplete[0].InstallationSlipNo;
                        doComplete.InstallationType = doPrepareInstallComplete[0].InstallationType;
                        if (doPrepareInstallComplete[0].NormalInstallFee != null)
                            doComplete.NormalInstallationFee = (decimal)doPrepareInstallComplete[0].NormalInstallFee;
                        doComplete.OCC = doPrepareInstallComplete[0].OCC;
                        doComplete.SECOMPaymentFee = 0;
                        doComplete.SECOMRevenueFee = 0;
                        doComplete.ServiceTypeCode = doPrepareInstallComplete[0].ServiceTypeCode;
                        doComplete.doInstrumentDetailsList = new List<doInstrumentDetails>();
                        //doComplete.doSubcontractorDetailsList;
                    }

                    foreach (doPrepareCompleteInstallationData prepareData in doPrepareInstallComplete)
                    {
                        doInstrumentDetails InstrumentDetail = new doInstrumentDetails();
                        InstrumentDetail.AddQty = prepareData.AddQty;
                        InstrumentDetail.InstrumentCode = prepareData.InstrumentCode;
                        InstrumentDetail.InstrumentTypeCode = prepareData.InstrumentTypeCode;
                        if (prepareData.InstallQty != null)
                            InstrumentDetail.InstrumentQty = (int)prepareData.InstallQty;
                        InstrumentDetail.RemoveQty = prepareData.RemoveQty;
                        doComplete.doInstrumentDetailsList.Add(InstrumentDetail);
                    }

                    cHandler.CompleteInstallation(doComplete);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Temp_CompleteInstallation_Sale(string SaleContractCode)
        {
            try
            {
                List<doPrepareCompleteInstallationData> doPrepareInstallComplete = base.Temp_CompleteInstallation_Sale(SaleContractCode);

                IContractHandler cHandler = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
                ISaleContractHandler sHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                doCompleteInstallationData doComplete = new doCompleteInstallationData();
                if (doPrepareInstallComplete != null && doPrepareInstallComplete.Count > 0)
                {
                    if (doPrepareInstallComplete.Count > 0)
                    {
                        if (doPrepareInstallComplete[0].OrderInstallFee != null)
                            doComplete.BillingInstallationFee = (decimal)doPrepareInstallComplete[0].OrderInstallFee;
                        doComplete.BillingOCC = null;
                        doComplete.CompleteInstallationProcessFlag = true;
                        doComplete.ContractCode = doPrepareInstallComplete[0].ContractCode;
                        doComplete.IEInchargeEmpNo = doPrepareInstallComplete[0].IEInchargeNo;
                        doComplete.InstallationCompleteDate = (DateTime)doPrepareInstallComplete[0].InstallationCompleteDate;
                        doComplete.InstallationCompleteProcessDate = (DateTime)doPrepareInstallComplete[0].InstallationCompleteProcessDate;
                        doComplete.InstallationMemo = doPrepareInstallComplete[0].InstallationMemo;
                        doComplete.InstallationSlipNo = doPrepareInstallComplete[0].InstallationSlipNo;
                        doComplete.InstallationType = doPrepareInstallComplete[0].InstallationType;
                        if (doPrepareInstallComplete[0].NormalInstallFee != null)
                            doComplete.NormalInstallationFee = (decimal)doPrepareInstallComplete[0].NormalInstallFee;
                        doComplete.OCC = doPrepareInstallComplete[0].OCC;
                        doComplete.SECOMPaymentFee = 0;
                        doComplete.SECOMRevenueFee = 0;
                        doComplete.ServiceTypeCode = doPrepareInstallComplete[0].ServiceTypeCode;
                        doComplete.doInstrumentDetailsList = new List<doInstrumentDetails>();
                        //doComplete.doSubcontractorDetailsList;
                    }

                    foreach (doPrepareCompleteInstallationData prepareData in doPrepareInstallComplete)
                    {
                        doInstrumentDetails InstrumentDetail = new doInstrumentDetails();
                        InstrumentDetail.AddQty = prepareData.AddQty;
                        InstrumentDetail.InstrumentCode = prepareData.InstrumentCode;
                        InstrumentDetail.InstrumentTypeCode = prepareData.InstrumentTypeCode;
                        if (prepareData.InstallQty != null)
                            InstrumentDetail.InstrumentQty = (int)prepareData.InstallQty;
                        InstrumentDetail.RemoveQty = prepareData.RemoveQty;
                        doComplete.doInstrumentDetailsList.Add(InstrumentDetail);
                    }

                    cHandler.CompleteInstallation(doComplete);

                    sHandler.UpdateCustomerAcceptance(doComplete.ContractCode,
                                                      doComplete.OCC,
                                                      DateTime.Now);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<doGetRemovalData> GetRemovalData(string strContractCode)
        {

            try
            {
                List<doGetRemovalData> doGetRemoval = base.GetRemovalData(strContractCode
                                                    , RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_ALL
                                                    , RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL
                                                    , InstallationStatus.C_INSTALL_STATUS_INSTALL_CANCELLED);

                return doGetRemoval;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CheckCanRegisterCP12(string strContractCode)
        {
            try
            {
                List<tbt_InstallationBasic> ListTbt_InstallationBasic = base.GetTbt_InstallationBasic(strContractCode);
                if (ListTbt_InstallationBasic == null || ListTbt_InstallationBasic.Count == 0)
                {

                }
                else
                {
                    tbt_InstallationBasic doTbt_InstallationBasic = ListTbt_InstallationBasic[0];
                    if (!CommonUtil.IsNullOrEmpty(doTbt_InstallationBasic.SlipNo))
                    {
                        List<tbt_InstallationSlip> doTbt_InstallationSlip = base.GetTbt_InstallationSlip(doTbt_InstallationBasic.SlipNo);
                        if (!CommonUtil.IsNullOrEmpty(doTbt_InstallationSlip) && doTbt_InstallationSlip[0].ChangeReasonCode == InstallChangeReason.C_INSTALL_CHANGE_REASON_SECOM)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5118);
                        }
                    }
                    if (doTbt_InstallationBasic.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE
                       || doTbt_InstallationBasic.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_MOVE
                       || doTbt_InstallationBasic.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_CHANGE_WIRING
                       || doTbt_InstallationBasic.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_PERIODICAL_MAINTENANCE
                       || doTbt_InstallationBasic.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_ALL
                       || doTbt_InstallationBasic.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_PARTIAL
                       || doTbt_InstallationBasic.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL)
                    {
                        ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                        doMiscTypeCode misc = new doMiscTypeCode();
                        misc.FieldName = MiscType.C_RENTAL_INSTALL_TYPE;
                        misc.ValueCode = doTbt_InstallationBasic.InstallationType;
                        List<doMiscTypeCode> ListMisc = new List<doMiscTypeCode>();
                        ListMisc.Add(misc);
                        List<doMiscTypeCode> doMiscTypeCode = handlerCommon.GetMiscTypeCodeList(ListMisc);
                        string displayInstallationType = "";
                        if (!CommonUtil.IsNullOrEmpty(doMiscTypeCode) && doMiscTypeCode.Count > 0)
                        {
                            displayInstallationType = doMiscTypeCode[0].ValueDisplay;
                        }
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5119, new string[] { displayInstallationType });
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string GetInstallationSlipNoForAcceptant(string pContractProjectCode, string pOCC)
        {
            var lst = base.GetInstallationSlipNoForAcceptant(pContractProjectCode, pOCC, SaleInstallationType.C_SALE_INSTALL_TYPE_ADD, SaleInstallationType.C_SALE_INSTALL_TYPE_NEW, InstallationStatus.C_INSTALL_STATUS_INSTALL_CANCELLED);
            return (lst != null && lst.Count > 0 ? lst[0] : null);
        }

        //No use
        //public bool CheckAllIEComplete(string strContractCode)
        //{
        //    bool bResult = false;

        //    try
        //    {
        //        List<bool?> bResultList = base.CheckAllIEComplete(strContractCode);
        //        if (bResultList != null && bResultList.Count > 0 && bResultList[0] != null)
        //        {
        //            bResult = bResultList[0].Value;
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        throw ex;
        //    }

        //    return bResult;
        //}

        #region Method Override
        //CMS180
        public List<dtInstallation> GetInstallationDataListForView(doSearchInstallationCondition cond)
        {
            try
            {
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IEmployeeMasterHandler ehandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;

                List<dtInstallation> result = base.GetInstallationDataListForView(ServiceType.C_SERVICE_TYPE_SALE,
                                                                                ServiceType.C_SERVICE_TYPE_RENTAL,
                                                                                InstallationStatus.C_INSTALL_STATUS_COMPLETED,
                                                                                RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_ALL,
                                                                                SaleInstallationType.C_SALE_INSTALL_TYPE_REMOVE_ALL,
                                                                                MiscType.C_SALE_INSTALL_TYPE,
                                                                                MiscType.C_RENTAL_INSTALL_TYPE,
                                                                                cond.ContractCode,
                                                                                cond.userCode,
                                                                                cond.planCode,
                                                                                cond.slipNo,
                                                                                cond.installationMaintenanceNo,
                                                                                cond.operationOffice,
                                                                                cond.salesmanEmpNo,
                                                                                cond.slipIssueDateFrom,
                                                                                cond.slipIssueDateTo,
                                                                                cond.contractTargetPurchaserName,
                                                                                cond.siteCode,
                                                                                cond.siteName,
                                                                                cond.siteAddress,
                                                                                cond.installationStatus,
                                                                                cond.slipStatus,
                                                                                cond.managementStatus,
                                                                                cond.slipNoNullFlag,
                                                                                cond.ViewFlag,
                                                                                cond.InstallationBy,
                                                                                cond.NotRegisteredYetSlipFlag,
                                                                                cond.NotRegisteredYetManagementFlag,
                                                                                cond.subContractorName
                                                                                );

                // Misc Mapping  
                MiscTypeMappingList miscMapping = new MiscTypeMappingList();
                miscMapping.AddMiscType(result.ToArray());
                handlerCommon.MiscTypeMappingList(miscMapping);

                //Mapping Employee

                EmployeeMappingList empLst = new EmployeeMappingList();
                empLst.AddEmployee(result.ToArray());
                ehandler.EmployeeListMapping(empLst);

                ///Mapping language
                CommonUtil.MappingObjectLanguage<dtInstallation>(result);


                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override List<dtInstallationHistoryForView> GetTbt_InstallationHistoryForView(string c_SERVICE_TYPE_SALE, string c_SERVICE_TYPE_RENTAL, string c_SALE_INSTALL_TYPE, string c_RENTAL_INSTALL_TYPE, string c_CHANGE_REASON_TYPE_CUSTOMER, string c_CHANGE_REASON_TYPE_SECOM, string c_CUSTOMER_REASON, string c_SECOM_REASON, string contractProjectCode, string maintenanceNo, string slipNo, string C_CURRENCY_LOCAL, string C_CURRENCY_US)
        {
            try
            {
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IEmployeeMasterHandler ehandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;

                List<dtInstallationHistoryForView> result = base.GetTbt_InstallationHistoryForView(c_SERVICE_TYPE_SALE, c_SERVICE_TYPE_RENTAL, c_SALE_INSTALL_TYPE, c_RENTAL_INSTALL_TYPE, c_CHANGE_REASON_TYPE_CUSTOMER, c_CHANGE_REASON_TYPE_SECOM, c_CUSTOMER_REASON, c_SECOM_REASON, contractProjectCode, maintenanceNo, slipNo, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);

                // Misc Mapping  
                MiscTypeMappingList miscMapping = new MiscTypeMappingList();
                miscMapping.AddMiscType(result.ToArray());
                handlerCommon.MiscTypeMappingList(miscMapping);

                //Mapping Employee

                EmployeeMappingList empLst = new EmployeeMappingList();
                empLst.AddEmployee(result.ToArray());
                ehandler.EmployeeListMapping(empLst);

                ///Mapping language
                CommonUtil.MappingObjectLanguage<dtInstallationHistoryForView>(result);


                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override List<dtInstallationMemoForView> GetTbt_InstallationMemoForView(string contractProjectCode, string maintenanceNo, string slipNo)
        {
            try
            {
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<dtInstallationMemoForView> result = base.GetTbt_InstallationMemoForView(contractProjectCode, maintenanceNo, slipNo);

                ///Mapping language
                CommonUtil.MappingObjectLanguage<dtInstallationMemoForView>(result);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<dtInstallation> GetInstallationDataListForCsvFile(doSearchInstallationCondition cond)
        {
            try
            {
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IEmployeeMasterHandler ehandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;

                List<dtInstallation> result = base.GetInstallationDataListForCsvFile(ServiceType.C_SERVICE_TYPE_SALE,
                                                                                ServiceType.C_SERVICE_TYPE_RENTAL,
                                                                                InstallationStatus.C_INSTALL_STATUS_COMPLETED,
                                                                                RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_ALL,
                                                                                SaleInstallationType.C_SALE_INSTALL_TYPE_REMOVE_ALL,
                                                                                MiscType.C_SALE_INSTALL_TYPE,
                                                                                MiscType.C_RENTAL_INSTALL_TYPE,
                                                                                cond.slipNoNullFlag,
                                                                                InstallationStatus.C_INSTALL_STATUS_INSTALL_NOT_REGISTERED);

                // Misc Mapping  
                MiscTypeMappingList miscMapping = new MiscTypeMappingList();
                miscMapping.AddMiscType(result.ToArray());
                handlerCommon.MiscTypeMappingList(miscMapping);

                //Mapping Employee

                EmployeeMappingList empLst = new EmployeeMappingList();
                empLst.AddEmployee(result.ToArray());
                ehandler.EmployeeListMapping(empLst);

                ///Mapping language
                CommonUtil.MappingObjectLanguage<dtInstallation>(result);


                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        public List<dtGetInstallationReport> GetInstallationReportExcelFile(doInstallationReport cond)
        {
            try
            {
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IEmployeeMasterHandler ehandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;

                List<dtGetInstallationReport> result = base.GetInstallationReport(cond.SubcontractorCode, cond.PaidDateFrom, cond.PaidDateTo);

                // Misc Mapping  
                MiscTypeMappingList miscMapping = new MiscTypeMappingList();
                miscMapping.AddMiscType(result.ToArray());
                handlerCommon.MiscTypeMappingList(miscMapping);

                //Mapping Employee

                EmployeeMappingList empLst = new EmployeeMappingList();
                empLst.AddEmployee(result.ToArray());
                ehandler.EmployeeListMapping(empLst);

                ///Mapping language
                CommonUtil.MappingObjectLanguage<dtGetInstallationReport>(result);


                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<dtGetInstallationReportMonthly> GetInstallationReportMonthlyExcelFile(doInstallationReportMonthly cond)
        {
            try
            {
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IEmployeeMasterHandler ehandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;

                List<dtGetInstallationReportMonthly> result = base.GetInstallationReportMonthly(
                    cond.ReportType,
                    cond.ReceiveDateFrom,
                    cond.ReceiveDateTo,
                    cond.CompleteDateFrom,
                    cond.CompleteDateTo,
                    cond.ExpectedStartDateFrom,
                    cond.ExpectedStartDateTo,
                    cond.ExpectedCompleteDateFrom,
                    cond.ExpectedCompleteDateTo,
                    cond.ContractCode,
                    cond.SiteName,
                    cond.SubContractorCode,
                    cond.ProductName,
                    cond.InstallationStatus,
                    cond.BuildingType
                );

                // Misc Mapping  
                MiscTypeMappingList miscMapping = new MiscTypeMappingList();
                miscMapping.AddMiscType(result.ToArray());
                handlerCommon.MiscTypeMappingList(miscMapping);

                //Mapping Employee

                EmployeeMappingList empLst = new EmployeeMappingList();
                empLst.AddEmployee(result.ToArray());
                ehandler.EmployeeListMapping(empLst);

                ///Mapping language
                CommonUtil.MappingObjectLanguage<dtGetInstallationReportMonthly>(result);


                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    public class InstallationReportData
    {
        public string DocumentCode { get; set; }
        public string ContractCode { get; set; }
        public string OperationOfficeCode { get; set; }
        public string SlipIssueOfficeCode { get; set; }
    }


}
