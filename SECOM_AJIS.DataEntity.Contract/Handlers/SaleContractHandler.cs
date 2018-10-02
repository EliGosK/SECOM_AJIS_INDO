using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using System.Transactions;
using SECOM_AJIS.DataEntity.Contract.Handlers;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Quotation;
using SECOM_AJIS.DataEntity.Inventory;
using SECOM_AJIS.DataEntity.Installation;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class SaleContractHandler : BizCTDataEntities, ISaleContractHandler
    {
        /// <summary>
        /// Get sale basic for view data
        /// </summary>
        /// <param name="pchvContractCode"></param>
        /// <param name="pchrOCC"></param>
        /// <param name="pLatestOCCFlag"></param>
        /// <returns></returns>
        List<dtTbt_SaleBasicForView> ISaleContractHandler.GetTbt_SaleBasicForView(string pchvContractCode, string pchrOCC, bool? pLatestOCCFlag)
        {
            List<dtTbt_SaleBasicForView> doSaleBasicList = base.GetTbt_SaleBasicForView(pchvContractCode, pchrOCC, pLatestOCCFlag);
            if (doSaleBasicList != null && doSaleBasicList.Count > 0)
            {
                MiscTypeMappingList miscList = new MiscTypeMappingList();
                miscList.AddMiscType(doSaleBasicList.ToArray());

                ICommonHandler comHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                comHandler.MiscTypeMappingList(miscList);
            }

            //return base.GetTbt_SaleBasicForView(pchvContractCode, pchrOCC, pLatestOCCFlag);
            return doSaleBasicList;
        }

        /// <summary>
        /// Get sale basic data
        /// </summary>
        /// <param name="contractCode"></param>
        /// <returns></returns>
        public List<tbt_SaleBasic> GetTbt_SaleBasic(string contractCode)
        {
            return base.GetTbt_SaleBasic(contractCode, null, null);
        }

        /// <summary>
        /// Get sale basic data
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="OCC"></param>
        /// <returns></returns>
        public List<tbt_SaleBasic> GetTbt_SaleBasic(string contractCode, string OCC)
        {
            return base.GetTbt_SaleBasic(contractCode, OCC, null);
        }

        /// <summary>
        /// To get sale contract data with accumulated instrument qty
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="OCC"></param>
        /// <returns></returns>
        public doSaleContractData GetSaleContractData(string contractCode, string OCC)
        {
            try
            {
                doSaleContractData dsDo = null;

                string rOCC = OCC;
                if (rOCC == null)
                {
                    List<string> OCCLst = this.GetSaleLastOCC(contractCode);
                    if (OCCLst != null)
                    {
                        if (OCCLst.Count > 0)
                            rOCC = OCCLst[0];
                    }
                }

                /* --- Get sale basic data --- */
                List<tbt_SaleBasic> sLst = this.GetTbt_SaleBasic(contractCode, rOCC);
                if (sLst != null)
                {
                    if (sLst.Count > 0)
                    {
                        dsDo = new doSaleContractData();
                        dsDo.dtTbt_SaleBasic = sLst[0];

                        /* --- Get instrument details --- */
                        dsDo.dtTbt_SaleInstrumentDetails = this.GetSaleInstrumentDetails(contractCode, rOCC);
                    }
                }
                return dsDo;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get sale contract basic information
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        public List<dtSaleContractBasicForView> GetSaleContractBasicForView(string strContractCode)
        {
            //return this.GetSaleContractBasicForView(strContractCode, MiscType.C_SALE_PROC_MANAGE_STATUS, MiscType.C_SALE_TYPE, MiscType.C_SALE_CHANGE_TYPE, FlagType.C_FLAG_ON);
            return this.GetSaleContractBasicForView(strContractCode, MiscType.C_SALE_PROC_MANAGE_STATUS, MiscType.C_SALE_TYPE, MiscType.C_SALE_CHANGE_TYPE, FlagType.C_FLAG_ON, ContractStatus.C_CONTRACT_STATUS_CANCEL, ContractStatus.C_CONTRACT_STATUS_END, ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL);
        }

        /// <summary>
        /// To check linkage sale contract
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="quotationTargetCode"></param>
        /// <returns></returns>
        public doSaleContractData CheckLinkageSaleContract(string contractCode, string quotationTargetCode)
        {
            try
            {
                //1. Get sale contract data
                doSaleContractData dsData = this.GetSaleContractData(contractCode, null);

                //2. Get relation data
                List<tbt_RelationType> relationTypeList 
                    = base.CheckRelationType(contractCode
                                            , ContractStatus.C_CONTRACT_STATUS_END
                                            , ContractStatus.C_CONTRACT_STATUS_CANCEL
                                            , ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL
                                            , quotationTargetCode
                                            , RelationType.C_RELATION_TYPE_SALE);
                if (relationTypeList != null && relationTypeList.Count > 0)
                {
                    CommonUtil c = new CommonUtil();
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3120,
                        c.ConvertContractCode(contractCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                        c.ConvertContractCode(relationTypeList[0].ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT));
                }

                //3.	Check existing data
                CommonUtil cmm = new CommonUtil();
                string contractCodeShort = cmm.ConvertContractCode(contractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);

                if (dsData != null && dsData.dtTbt_SaleBasic != null)
                {
                    string strContractStatus = (dsData.dtTbt_SaleBasic.ContractStatus != null) ? dsData.dtTbt_SaleBasic.ContractStatus.Trim() : dsData.dtTbt_SaleBasic.ContractStatus;

                    //if (strContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START)
                    //    throw ApplicationErrorException.ThrowErrorException(
                    //        MessageUtil.MODULE_CONTRACT, 
                    //        MessageUtil.MessageList.MSG3119,
                    //        contractCodeShort);

                    if (strContractStatus == ContractStatus.C_CONTRACT_STATUS_CANCEL)
                        throw ApplicationErrorException.ThrowErrorException(
                            MessageUtil.MODULE_CONTRACT, 
                            MessageUtil.MessageList.MSG3105,
                            contractCodeShort);
                }
                else
                {
                    throw ApplicationErrorException.ThrowErrorException(
                        MessageUtil.MODULE_COMMON, 
                        MessageUtil.MessageList.MSG0093,
                        contractCodeShort);
                }

                return dsData;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// To generate contract occurrence
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        public string GenerateContractOCC(string strContractCode)
        {
            try
            {
                //1.	Call method to get last contract occurrence of sale contract
                string strLastOCC = this.GetLastOCC(strContractCode);

                //1.1.	Check strLastOCC and set strOCC
                string strOCC = string.Empty;
                if (strLastOCC == null)
                    strOCC = OCCType.C_FIRST_SALE_CONTRACT_OCC;
                else
                {
                    decimal dLastOCC = Decimal.Parse(strLastOCC); //9965
                    decimal dOCC = Math.Round(dLastOCC / 10, 0, MidpointRounding.AwayFromZero); // Round(996.5) => 997
                    int iOCC = (Decimal.ToInt32(dOCC) * 10) - 10; //997*10-10 = 9960
                    strOCC = iOCC.ToString().PadLeft(4, '0');

                    if (strOCC == "0000")
                    {
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3026);
                    }
                }

                //2.	Return strOCC
                return strOCC;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// To generate contract counter of sale contract
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        public int GenerateContractCounter(string strContractCode)
        {
            try
            {
                //1.	Get last contract counter number
                List<int?> lastCounterNo = base.GetLastContractCounterNo(strContractCode);

                //2.	check iLastCounter
                int iCounter = 0;
                if (lastCounterNo.Count <= 0 || lastCounterNo[0] == null)
                {
                    iCounter = 1;
                }
                else if (lastCounterNo[0] == 99)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3024);
                }
                else
                {
                    iCounter = lastCounterNo[0].Value + 1;
                }

                //3.	return iCounter
                return iCounter;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// To get last OCC of sale contract
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        public string GetLastOCC(string strContractCode)
        {
            try
            {
                List<string> lastOCC = base.GetLastOCCs(strContractCode);

                if (lastOCC.Count > 0 && lastOCC[0] != null && lastOCC[0] != string.Empty)
                    return lastOCC[0];
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Update sale basic data
        /// </summary>
        /// <param name="dotbt_SaleBasic"></param>
        /// <returns></returns>
        public List<tbt_SaleBasic> UpdateTbt_SaleBasic(tbt_SaleBasic dotbt_SaleBasic)
        {
            try
            {
                //Check whether this record is the most updated data
                List<tbt_SaleBasic> sList = this.GetTbt_SaleBasic(dotbt_SaleBasic.ContractCode, dotbt_SaleBasic.OCC);
                if (DateTime.Compare(sList[0].UpdateDate.Value, dotbt_SaleBasic.UpdateDate.Value) != 0)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                }

                //set updateDate and updateBy
                dotbt_SaleBasic.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                dotbt_SaleBasic.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_SaleBasic> tbt_SaleBasicList = new List<tbt_SaleBasic>();
                tbt_SaleBasicList.Add(dotbt_SaleBasic);
                List<tbt_SaleBasic> updatedList = base.UpdateTbt_SaleBasic(CommonUtil.ConvertToXml_Store<tbt_SaleBasic>(tbt_SaleBasicList));

                //Insert Log
                if (updatedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Update;
                    logData.TableName = TableName.C_TBL_NAME_SALE_BASIC;
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
        /// Update sale instrument data
        /// </summary>
        /// <param name="dotbt_SaleInstrumentDetails"></param>
        /// <returns></returns>
        public List<tbt_SaleInstrumentDetails> UpdateTbt_SaleInstrumentDetails(tbt_SaleInstrumentDetails dotbt_SaleInstrumentDetails)
        {
            try
            {
                //Check whether this record is the most updated data
                //List<tbt_SaleInstrumentDetails> sList = this.GetTbt_SaleInstrumentDetails(dotbt_SaleInstrumentDetails.ContractCode, dotbt_SaleInstrumentDetails.OCC);
                //if (sList[0].UpdateDate.Value.ToLongDateString() != dotbt_SaleInstrumentDetails.UpdateDate.Value.ToLongDateString())
                //{
                //    if (DateTime.Compare(sList[0].UpdateDate.Value, dotbt_SaleInstrumentDetails.UpdateDate.Value) != 0)
                //    {
                //        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                //    }
                //}

                //set updateDate and updateBy
                dotbt_SaleInstrumentDetails.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                dotbt_SaleInstrumentDetails.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_SaleInstrumentDetails> tbt_SaleInstrumentDetailsList = new List<tbt_SaleInstrumentDetails>();
                tbt_SaleInstrumentDetailsList.Add(dotbt_SaleInstrumentDetails);
                List<tbt_SaleInstrumentDetails> updatedList = base.UpdateTbt_SaleInstrumentDetails(CommonUtil.ConvertToXml_Store<tbt_SaleInstrumentDetails>(tbt_SaleInstrumentDetailsList));

                //Insert Log
                if (updatedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Update;
                    logData.TableName = TableName.C_TBL_NAME_SALE_INST_DET;
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
        /// Calling from billing module when registering customer acceptance
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strSaleOCC"></param>
        /// <param name="dtpCustomerAcceptanceDate"></param>
        public void UpdateCustomerAcceptance(string strContractCode, string strSaleOCC, DateTime? dtpCustomerAcceptanceDate)
        {
            try
            {
                //1.	Check mandatory data strContractCode, strSaleOCC and dtpCustomerAcceptanceDate are require fields
                doUpdateCustomerAcceptanceData doCust = new doUpdateCustomerAcceptanceData();
                doCust.ContractCode = strContractCode;
                doCust.OCC = strSaleOCC;
                doCust.Date = dtpCustomerAcceptanceDate;
                ApplicationErrorException.CheckMandatoryField(doCust);

                //3.	Get data in sale basic table
                List<tbt_SaleBasic> doOutTbt_SaleBasic = this.GetTbt_SaleBasic(strContractCode, strSaleOCC);

                //set data to doTbt_SaleBasic
                if (doOutTbt_SaleBasic.Count > 0 && doOutTbt_SaleBasic[0] != null)
                {
                    tbt_SaleBasic doSaleBasic = doOutTbt_SaleBasic[0];
                    doSaleBasic.ContractCode = strContractCode;
                    doSaleBasic.OCC = strSaleOCC;
                    if (doSaleBasic.SaleProcessManageStatus == SaleProcessManageStatus.C_SALE_PROCESS_STATUS_COMPLETE_NOTACCEPT)
                        doSaleBasic.SaleProcessManageStatus = SaleProcessManageStatus.C_SALE_PROCESS_STATUS_COMPLETE_ACCEPT;
                    else if (doSaleBasic.SaleProcessManageStatus == SaleProcessManageStatus.C_SALE_PROCESS_STATUS_SHIP)
                        doSaleBasic.SaleProcessManageStatus = SaleProcessManageStatus.C_SALE_PROCESS_STATUS_ACCEPT_NOTCOMPLETE;
                    doSaleBasic.ContractStatus = ContractStatus.C_CONTRACT_STATUS_AFTER_START;
                    doSaleBasic.CustAcceptanceDate = dtpCustomerAcceptanceDate;
                    doSaleBasic.WarranteeFrom = dtpCustomerAcceptanceDate;
                    doSaleBasic.WarranteeTo = dtpCustomerAcceptanceDate.Value.AddYears(1).AddDays(-1);
                    
                    //doSaleBasic.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    //doSaleBasic.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                    using (TransactionScope scope = new TransactionScope())
                    {
                        //5.	Update data in sale basic table
                        this.UpdateTbt_SaleBasic(doSaleBasic);

                        if (doSaleBasic.SaleProcessManageStatus == SaleProcessManageStatus.C_SALE_PROCESS_STATUS_COMPLETE_ACCEPT)
                        {
                            IInventoryHandler invHandler = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                            invHandler.UpdateCustomerAcceptance(
                                doSaleBasic.ContractCode,
                                doSaleBasic.OCC,
                                doSaleBasic.CustAcceptanceDate);
                        }

                        IBillingTempHandler btHandler = ServiceContainer.GetService<IBillingTempHandler>() as IBillingTempHandler;
                        btHandler.DeleteBillingTempByContractCodeOCC(
                            doSaleBasic.ContractCode,
                                doSaleBasic.OCC);

                        scope.Complete();
                    }
                }
                else
                {
                    CommonUtil cmm = new CommonUtil();
                    throw ApplicationErrorException.ThrowErrorException(
                        MessageUtil.MODULE_COMMON, 
                        MessageUtil.MessageList.MSG0011, 
                        cmm.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT));
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Update entire contract
        /// </summary>
        /// <param name="dsSaleContract"></param>
        public void UpdateEntireContract(dsSaleContractData dsSaleContract)
        {
            try
            {
                foreach (var item in dsSaleContract.dtTbt_SaleBasic)
                {
                    UpdateTbt_SaleBasic(item);
                }

                var delInstrumentLst = base.DeleteTbt_SaleInstrumentDetails_ByContractCodeOCC(dsSaleContract.dtTbt_SaleBasic[0].ContractCode
                    , dsSaleContract.dtTbt_SaleBasic[0].OCC);

                //Insert Log
                if (delInstrumentLst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                    logData.TableName = TableName.C_TBL_NAME_SALE_INST_DET;
                    logData.TableData = CommonUtil.ConvertToXml(delInstrumentLst);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                foreach (var item in dsSaleContract.dtTbt_SaleInstrumentDetails)
                {
                    InsertTbt_SaleInstrumentDetails(item);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Update data in case new/add sale and will insert new occurrence in case other installation type.
        /// </summary>
        /// <param name="doComplete"></param>
        public void CompleteInstallation(doCompleteInstallationData doComplete)
        {
            try
            {
                //Validation ContractCode, Installation Type, Installation Slip No. and Complete Process Flag
                ApplicationErrorException.CheckMandatoryField<doCompleteInstallationData, doCompleteSaleCompleteInstallation>(doComplete);

                //2. Get sale contract data
                //2.1 Get alst OCC and check contract code exists
                //2.1.1 Get last OCC
                string strLastOCC = this.GetLastOCC(doComplete.ContractCode);

                //2.1.2 if strLastOCC is null
                if (strLastOCC == null)
                {
                    CommonUtil c = new CommonUtil();
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, c.ConvertContractCode(doComplete.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT));
                }

                //3.Check installation type
                //3.1 In case installation type is new or add
                if (doComplete.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_NEW ||
                    doComplete.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_ADD)
                {
                    using (TransactionScope scope = new TransactionScope()) //Add by Jutarat A. on 26022013
                    {
                        //3.1.1 Get sale basic data
                        List<tbt_SaleBasic> doTbt_SaleBasic = this.GetTbt_SaleBasic(doComplete.ContractCode, strLastOCC);

                        //3.1.2 Prepare data object before save
                        tbt_SaleBasic saleDo = doTbt_SaleBasic[0];
                        saleDo.ContractCode = doComplete.ContractCode;
                        saleDo.OCC = strLastOCC;
                        saleDo.ContractStatus = ContractStatus.C_CONTRACT_STATUS_AFTER_START;

                        if (saleDo.SaleProcessManageStatus == SaleProcessManageStatus.C_SALE_PROCESS_STATUS_SHIP
                            || saleDo.SaleProcessManageStatus == SaleProcessManageStatus.C_SALE_PROCESS_STATUS_GENCODE) //Add by Jutarat A. on 11092013
                            saleDo.SaleProcessManageStatus = SaleProcessManageStatus.C_SALE_PROCESS_STATUS_COMPLETE_NOTACCEPT;
                        else if (saleDo.SaleProcessManageStatus == SaleProcessManageStatus.C_SALE_PROCESS_STATUS_ACCEPT_NOTCOMPLETE)
                            saleDo.SaleProcessManageStatus = SaleProcessManageStatus.C_SALE_PROCESS_STATUS_COMPLETE_ACCEPT;

                        saleDo.InstallationCompleteFlag = FlagType.C_FLAG_ON;
                        saleDo.InstallCompleteDate = doComplete.InstallationCompleteDate;
                        saleDo.InstallCompleteProcessDate = doComplete.InstallationCompleteProcessDate;
                        saleDo.IEInchargeEmpNo = doComplete.IEInchargeEmpNo;
                        saleDo.InstallationCompleteEmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;
                        saleDo.InstallationSlipNo = doComplete.InstallationSlipNo;
                        saleDo.InstallationTypeCode = doComplete.InstallationType;

                        //saleDo.InstallFeePaidBySECOM = doComplete.SECOMPaymentFee;
                        if(doComplete.NormalInstallationFee > doComplete.BillingInstallationFee)
                        {
                           saleDo.InstallFeePaidBySECOM = doComplete.NormalInstallationFee-doComplete.BillingInstallationFee;  
                        }
                        else
                        {
                           saleDo.InstallFeePaidBySECOM = 0;
                        }
                        //saleDo.InstallFeeRevenueBySECOM = doComplete.SECOMRevenueFee;
                        if (doComplete.BillingInstallationFee > doComplete.NormalInstallationFee)
                        {
                            saleDo.InstallFeeRevenueBySECOM = doComplete.BillingInstallationFee - doComplete.NormalInstallationFee;
                        }
                        else
                        {
                            saleDo.InstallFeeRevenueBySECOM = 0;
                        }
                        saleDo.NewAddInstallCompleteProcessDate = doComplete.InstallationCompleteProcessDate;
                        saleDo.NewAddInstallCompleteEmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;
                        saleDo.ChangeImplementDate = doComplete.InstallationCompleteDate;

                        //3.1.3 Update data to sale basic data
                        this.UpdateTbt_SaleBasic(saleDo);
                        if (doComplete.doSubcontractorDetailsList != null && doComplete.doSubcontractorDetailsList.Count > 0)
                        {
                            foreach (doSubcontractorDetails dataSubCondetail in doComplete.doSubcontractorDetailsList)
                            {
                                this.InsertTbt_SaleInstSubcontractor(doComplete.ContractCode, doComplete.OCC, dataSubCondetail.SubcontractorCode);
                            }
                        }

                        scope.Complete(); //Add by Jutarat A. on 26022013
                    }
                    
                }
                //3.2 In case installation is other
                else
                {
                    //3.2.1 Get sale basic table
                    dsSaleContractData dsEntireContract = this.GetEntireContract(doComplete.ContractCode, strLastOCC);
                    if (dsEntireContract != null)
                    {
                        //3.2.2 Generate sale occurance
                        string strNewOCC = this.GenerateContractOCC(doComplete.ContractCode);

                        //3.2.3 Prepare data object before save
                        tbt_SaleBasic saleDo = CommonUtil.CloneObject<tbt_SaleBasic, tbt_SaleBasic>(dsEntireContract.dtTbt_SaleBasic[0]);
                        saleDo.ContractCode = doComplete.ContractCode;
                        saleDo.OCC = strNewOCC;

                        if(doComplete.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_REMOVE_ALL)
                        {
		                      saleDo.ContractStatus  = ContractStatus.C_CONTRACT_STATUS_CANCEL;
                        }
                       
                        saleDo.SaleProcessManageStatus = null;

                        if (doComplete.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE)
                            saleDo.ChangeType = SaleChangeType.C_SALE_CHANGE_TYPE_EXCHANGE_INSTR;
                        else if (doComplete.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_MOVE)
                            saleDo.ChangeType = SaleChangeType.C_SALE_CHANGE_TYPE_MOVE_INSTR;
                        else if (doComplete.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_PARTIAL_REMOVE)
                            saleDo.ChangeType = SaleChangeType.C_SALE_CHANGE_TYPE_REMOVE_INSTR_PARTIAL;
                        else if (doComplete.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_REMOVE_ALL)
                            saleDo.ChangeType = SaleChangeType.C_SALE_CHANGE_TYPE_REMOVE_INSTR_ALL;
                        //Add by Jutarat A. on 21052013
                        else if (doComplete.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_CHANGE_WIRING)
                            saleDo.ChangeType = SaleChangeType.C_SALE_CHANGE_TYPE_CHANGE_WIRING;
                        //End Add

                        saleDo.InstallationCompleteFlag = FlagType.C_FLAG_ON;
                        saleDo.InstallCompleteDate = doComplete.InstallationCompleteDate;
                        saleDo.InstallCompleteProcessDate = doComplete.InstallationCompleteProcessDate;
                        saleDo.IEInchargeEmpNo = doComplete.IEInchargeEmpNo;
                        saleDo.InstallationCompleteEmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;
                        saleDo.InstallationSlipNo = doComplete.InstallationSlipNo;
                        saleDo.InstallationTypeCode = doComplete.InstallationType;
                        saleDo.NormalProductPrice = null;
                        saleDo.NormalInstallFee = doComplete.NormalInstallationFee;
                        saleDo.NormalSalePrice= null;
                        saleDo.OrderProductPrice= null;
                        saleDo.OrderInstallFee = doComplete.BillingInstallationFee;
                        saleDo.OrderSalePrice= null;
                        saleDo.BillingAmt_ApproveContract= null;
                        saleDo.BillingAmt_PartialFee= null;
                        saleDo.BillingAmt_Acceptance= null;
                        //saleDo.InstallFeePaidBySECOM = doComplete.SECOMPaymentFee;
                        //saleDo.InstallFeeRevenueBySECOM = doComplete.SECOMRevenueFee;
                        if (doComplete.NormalInstallationFee > doComplete.BillingInstallationFee)
                        {
                            saleDo.InstallFeePaidBySECOM = doComplete.NormalInstallationFee - doComplete.BillingInstallationFee;
                        }
                        else
                        {
                            saleDo.InstallFeePaidBySECOM = 0;
                        }
                        if (doComplete.BillingInstallationFee > doComplete.NormalInstallationFee)
                        {
                            saleDo.InstallFeeRevenueBySECOM = doComplete.BillingInstallationFee - doComplete.NormalInstallationFee;
                        }
                        else
                        {
                            saleDo.InstallFeeRevenueBySECOM = 0;
                        }
                        saleDo.ChangeImplementDate = doComplete.InstallationCompleteDate;
                        dsEntireContract.dtTbt_SaleBasic[0] = saleDo;

                        dsEntireContract.dtTbt_SaleInstrumentDetails = new List<tbt_SaleInstrumentDetails>();
                        if (doComplete.doInstrumentDetailsList != null && doComplete.doInstrumentDetailsList.Count > 0)
                        {
                            foreach (doInstrumentDetails instDetail in doComplete.doInstrumentDetailsList)
                            {
                                tbt_SaleInstrumentDetails saleDetailDo = new tbt_SaleInstrumentDetails();
                                saleDetailDo.ContractCode = doComplete.ContractCode;
                                saleDetailDo.OCC = strNewOCC;
                                saleDetailDo.InstrumentCode = instDetail.InstrumentCode;
                                saleDetailDo.InstrumentTypeCode = InstrumentType.C_INST_TYPE_GENERAL;
                                saleDetailDo.InstrumentQty = instDetail.AddQty - instDetail.RemoveQty;
                                dsEntireContract.dtTbt_SaleInstrumentDetails.Add(saleDetailDo);
                            }
                        }
                        using (TransactionScope scope = new TransactionScope())
                        {
                            //3.2.4 Insert data to sale basic table and instrument table
                            this.InsertEntireContract(dsEntireContract);

                            //Add by Jutarat A. on 27022014 (Move)
                            List<tbt_SaleBasic> sList = this.GetTbt_SaleBasic(doComplete.ContractCode, strLastOCC);
                            List<tbt_SaleBasic> dsResult;
                            if (sList.Count > 0)
                            {
                                //update tbt_salebasic
                                sList[0].LatestOCCFlag = FlagType.C_FLAG_OFF;
                                dsResult = this.UpdateTbt_SaleBasic(sList[0]);
                            }
                            //End Add

                            //3.2.5 Insert data to subcontractor table
                            if (doComplete.doSubcontractorDetailsList != null && doComplete.doSubcontractorDetailsList.Count > 0)
                            {
                                foreach (doSubcontractorDetails doInst in doComplete.doSubcontractorDetailsList)
                                {
                                    this.InsertTbt_SaleInstSubcontractor(doComplete.ContractCode, strNewOCC, doInst.SubcontractorCode);
                                }
                            }
                            //Comment by Jutarat A. on 27022014 (Move)
                            //List<tbt_SaleBasic> sList = this.GetTbt_SaleBasic(doComplete.ContractCode, strLastOCC);
                            //List<tbt_SaleBasic> dsResult;
                            //if (sList.Count > 0)
                            //{
                            //    //update tbt_salebasic
                            //    sList[0].LatestOCCFlag = FlagType.C_FLAG_OFF;
                            //    dsResult = this.UpdateTbt_SaleBasic(sList[0]);
                            //}
                            //End Comment

                            //3.2.6 Check need to send billing or not
                            if (doComplete.BillingOCC != null && doComplete.BillingOCC != string.Empty && doComplete.BillingInstallationFee > 0)
                            {
                                //3.2.6.1 Prepare data object before sending to Billing
                                doBillingTempDetail billingDo = new doBillingTempDetail();
                                billingDo.ContractCode = doComplete.ContractCode;
                                billingDo.BillingOCC = doComplete.BillingOCC;
                                if (doComplete.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_REMOVE_ALL)
                                {
                                    billingDo.ContractBillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE;
                                }
                                else
                                {
                                    billingDo.ContractBillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE;
                                }
                                billingDo.BillingDate = doComplete.InstallationCompleteProcessDate;
                                billingDo.BillingAmount = doComplete.BillingInstallationFee;
                                billingDo.OCC = strNewOCC;
                                //3.2.6.2 Call send data to billing module
                                IBillingInterfaceHandler hand = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
                                hand.SendBilling_SaleCompleteInstall(billingDo);
                            }

                            //In case remove all, update quotation data
                            if (doComplete.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_REMOVE_ALL)
                            {
                                doUpdateQuotationData doUpdateQuotation;
                                IQuotationHandler guotHandler;
                                guotHandler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;

                                doUpdateQuotation = new doUpdateQuotationData();
                                doUpdateQuotation.QuotationTargetCode = doComplete.ContractCode;
                                doUpdateQuotation.Alphabet = null;
                                doUpdateQuotation.LastUpdateDate = DateTime.MinValue; //null;
                                doUpdateQuotation.ContractCode = doComplete.ContractCode;
                                doUpdateQuotation.ActionTypeCode = ActionType.C_ACTION_TYPE_CANCEL;
                                int iUpdateQuotRowCount = guotHandler.UpdateQuotationData(doUpdateQuotation);
                            }

                            scope.Complete();
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Update process magement status to approve contract status in sale basic data.
        /// </summary>
        /// <param name="doComplete"></param>
        public void CancelInstallation(doCompleteInstallationData doComplete)
        {
            try
            {
                //Validation ContractCode and OCC
                ApplicationErrorException.CheckMandatoryField<doCompleteInstallationData, doCompleteSaleCancelInstallation>(doComplete);

                //2. Update data in sale basic table
                //2.1 Get sale basic table
                List<tbt_SaleBasic> doTbt_SaleBasic = this.GetTbt_SaleBasic(doComplete.ContractCode, doComplete.OCC);

                //2.2 Check current sale process management status
                if (doTbt_SaleBasic.Count > 0)
                {
                    if (doTbt_SaleBasic[0].SaleProcessManageStatus != SaleProcessManageStatus.C_SALE_PROCESS_STATUS_SHIP)
                    {
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3107);
                    }
                    else
                    {
                        //2.3 Prepare data object before save
                        doTbt_SaleBasic[0].SaleProcessManageStatus = SaleProcessManageStatus.C_SALE_PROCESS_STATUS_GENCODE;

                        //2.4 Update data to sale basic table
                        this.UpdateTbt_SaleBasic(doTbt_SaleBasic[0]);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Getting all part of specified contract for creaing a new occurrence or else
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <returns></returns>
        public dsSaleContractData GetEntireContract(string strContractCode, string strOCC)
        {
            dsSaleContractData dsResult = new dsSaleContractData();
            try
            {
                //1. Get Sale basic 
                List<tbt_SaleBasic> saleBasicList = new List<tbt_SaleBasic>();
                if (strOCC == null || strOCC == String.Empty)
                    saleBasicList = this.GetTbt_SaleBasic(strContractCode, null, true); //use lastest occ
                else
                    saleBasicList = this.GetTbt_SaleBasic(strContractCode, strOCC);

                if (saleBasicList != null && saleBasicList.Count > 0)
                {
                    //Keep result in dsSaleContractData
                    dsResult.dtTbt_SaleBasic = saleBasicList;

                    //2.	Get SaleInstrumentDetails
                    strOCC = (strOCC == null || strOCC == String.Empty) ? saleBasicList[0].OCC : strOCC;
                    List<tbt_SaleInstrumentDetails> instDetailList = this.GetTbt_SaleInstrumentDetails(strContractCode, strOCC);
                    dsResult.dtTbt_SaleInstrumentDetails = instDetailList;
                }
                else
                {
                    dsResult = null;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return dsResult;
        }

        /// <summary>
        /// Insert entire contract data
        /// </summary>
        /// <param name="saleContractData"></param>
        /// <returns></returns>
        public dsSaleContractData InsertEntireContract(dsSaleContractData dsData)
        {
            dsSaleContractData dsResult = new dsSaleContractData();
            try
            {
                string strContractCode = dsData.dtTbt_SaleBasic[0].ContractCode;
                string strOCC = dsData.dtTbt_SaleBasic[0].OCC;

                List<tbt_SaleBasic> sList = this.GetTbt_SaleBasic(strContractCode, strOCC);
                if (sList.Count > 0)
                    //update tbt_salebasic
                    dsResult.dtTbt_SaleBasic = this.UpdateTbt_SaleBasic(dsData.dtTbt_SaleBasic[0]);
                else
                    //insert tbt_salebasic
                    dsResult.dtTbt_SaleBasic = this.InsertTbt_SaleBasic(dsData.dtTbt_SaleBasic[0]);

                //insert tbt_saleinstrumentdetails
                dsResult.dtTbt_SaleInstrumentDetails = new List<tbt_SaleInstrumentDetails>();
                foreach (tbt_SaleInstrumentDetails saleDetail in dsData.dtTbt_SaleInstrumentDetails)
                {
                    List<tbt_SaleInstrumentDetails> saleDetailList = this.InsertTbt_SaleInstrumentDetails(saleDetail);
                    dsResult.dtTbt_SaleInstrumentDetails.AddRange(saleDetailList);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return dsResult;
        }

        /// <summary>
        /// Insert sale basic data
        /// </summary>
        /// <param name="doInsert"></param>
        /// <returns></returns>
        public List<tbt_SaleBasic> InsertTbt_SaleBasic(tbt_SaleBasic doInsert)
        {
            try
            {
                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                doInsert.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                doInsert.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_SaleBasic> doInsertList = new List<tbt_SaleBasic>();
                doInsertList.Add(doInsert);
                List<tbt_SaleBasic> insertList = base.InsertTbt_SaleBasic(CommonUtil.ConvertToXml_Store<tbt_SaleBasic>(doInsertList));

                //Insert Log
                if (insertList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_SALE_BASIC;
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

        /// <summary>
        /// Insert sale instrument detail data
        /// </summary>
        /// <param name="doInsert"></param>
        /// <returns></returns>
        public List<tbt_SaleInstrumentDetails> InsertTbt_SaleInstrumentDetails(tbt_SaleInstrumentDetails doInsert)
        {
            try
            {
                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                doInsert.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                doInsert.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_SaleInstrumentDetails> doInsertList = new List<tbt_SaleInstrumentDetails>();
                doInsertList.Add(doInsert);
                List<tbt_SaleInstrumentDetails> insertList = base.InsertTbt_SaleInstrumentDetails(CommonUtil.ConvertToXml_Store<tbt_SaleInstrumentDetails>(doInsertList));

                //Insert Log
                if (insertList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_SALE_INST_DET;
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

        /// <summary>
        /// Insert sale instrument subcontractor
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <param name="strSubcontractorCode"></param>
        /// <returns></returns>
        public List<tbt_SaleInstSubcontractor> InsertTbt_SaleInstSubcontractor(string strContractCode, String strOCC, string strSubcontractorCode)
        {
            List<tbt_SaleInstSubcontractor> insertList
                = base.InsertTbt_SaleInstSubcontractor(strContractCode, strOCC, strSubcontractorCode
                    , CommonUtil.dsTransData.dtOperationData.ProcessDateTime, CommonUtil.dsTransData.dtUserData.EmpNo);

            //Insert Log
            if (insertList.Count > 0)
            {
                doTransactionLog logData = new doTransactionLog();
                logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                logData.TableName = TableName.C_TBL_NAME_SALE_INST_SUB;
                logData.TableData = CommonUtil.ConvertToXml(insertList);
                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);
            }

            return insertList;
        }

        /// <summary>
        /// For registering change expected installation complete date of sale contract
        /// </summary>
        /// <param name="dsSaleContract"></param>
        /// <returns></returns>
        public bool RegisterChangeExpectedInstallationCompleteDate(dsSaleContractData dsSaleContract)
        {
            int iNewCounter;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    dsSaleContract.dtTbt_SaleBasic[0].LastChangeProcessDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    iNewCounter = GenerateContractCounter(dsSaleContract.dtTbt_SaleBasic[0].ContractCode);
                    dsSaleContract.dtTbt_SaleBasic[0].CounterNo = iNewCounter;
                    UpdateEntireContract(dsSaleContract);

#if !ROUND1
                    //Call update inventory process

                    IInventoryHandler invenhandler = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                    invenhandler.ChangeExpectedStartServiceDate(new SECOM_AJIS.DataEntity.Inventory.doBooking()
                    {
                        ContractCode = dsSaleContract.dtTbt_SaleBasic[0].ContractCode,
                        ExpectedStartServiceDate = dsSaleContract.dtTbt_SaleBasic[0].ExpectedInstallCompleteDate.GetValueOrDefault()
                    });

#endif
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        /// <summary>
        /// For register change plan of sale contract
        /// </summary>
        /// <param name="dsQuotation"></param>
        /// <param name="dsSaleContract"></param>
        /// <param name="listBillingTemp"></param>
        /// <returns></returns>
        public bool RegisterChangePlan(dsQuotationData dsQuotation, dsSaleContractData dsSaleContract, List<tbt_BillingTemp> listBillingTemp)
        {
            int contractCounter;
            bool bLockQuotationResult;

            BillingTempHandler billingTempHandler;
            IQuotationHandler quotationHandler;
            doUpdateQuotationData doUpdateQuotation;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    billingTempHandler = new BillingTempHandler();
                    quotationHandler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;

                    dsSaleContract.dtTbt_SaleBasic[0].LastChangeProcessDate = DateTime.Now;
                    contractCounter = GenerateContractCounter(dsSaleContract.dtTbt_SaleBasic[0].ContractCode);
                    dsSaleContract.dtTbt_SaleBasic[0].CounterNo = contractCounter;
                    UpdateEntireContract(dsSaleContract);

                    foreach (var item in listBillingTemp)
                    {
                        billingTempHandler.UpdateBillingTempByKey(item);
                    }

                    bLockQuotationResult = quotationHandler.LockQuotation(dsSaleContract.dtTbt_SaleBasic[0].QuotationTargetCode,
                                           dsSaleContract.dtTbt_SaleBasic[0].Alphabet, LockStyle.C_LOCK_STYLE_BACKWARD);

                    doUpdateQuotation = new doUpdateQuotationData();
                    doUpdateQuotation.QuotationTargetCode = dsSaleContract.dtTbt_SaleBasic[0].QuotationTargetCode;
                    doUpdateQuotation.Alphabet = dsSaleContract.dtTbt_SaleBasic[0].Alphabet;
                    doUpdateQuotation.ContractCode = dsSaleContract.dtTbt_SaleBasic[0].ContractCode;
                    doUpdateQuotation.ActionTypeCode = ActionType.C_ACTION_TYPE_CHANGE;
                    quotationHandler.UpdateQuotationData(doUpdateQuotation);

                    #region Rebooking before first installation complete
                    IInstallationHandler installationhandler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                    IInventoryHandler invenhandler = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

                    var installHist = installationhandler.GetTbt_InstallationHistory(dsSaleContract.dtTbt_SaleBasic[0].ContractCode, null, null);
                    var currentBooking = invenhandler.GetTbt_InventoryBooking(dsSaleContract.dtTbt_SaleBasic[0].ContractCode);

                    if ((installHist == null || installHist.Count <= 0) &&
                        currentBooking != null && currentBooking.Count > 0)
                    {
                        var bookingInstruments = (
                            from qd in dsQuotation.dtTbt_QuotationInstrumentDetails
                            group qd by new { qd.InstrumentCode } into grpByInst
                            select new
                            {
                                grpByInst.Key.InstrumentCode,
                                InstrumentQty = grpByInst.Sum(d => d.InstrumentQty),
                            }
                        ).ToList();

                        doBooking booking = new doBooking()
                        {
                            ContractCode = dsSaleContract.dtTbt_SaleBasic[0].ContractCode,
                            ExpectedStartServiceDate = dsSaleContract.dtTbt_SaleBasic[0].ExpectedCustAcceptanceDate ?? currentBooking[0].ExpectedStartServiceDate.Value,
                            blnExistContractCode = true,
                            blnFirstInstallCompleteFlag = true,
                            InstrumentCode = (from inst in bookingInstruments orderby inst.InstrumentCode select inst.InstrumentCode).ToList(),
                            InstrumentQty = (from inst in bookingInstruments orderby inst.InstrumentCode select inst.InstrumentQty ?? 0).ToList()
                        };

                        var previousBooking = invenhandler.GetTbt_InventoryBookingDetail(booking.ContractCode, null);
                        invenhandler.CancelBooking(booking);
                        invenhandler.NewBooking(booking);
                        var newBookingDtl = invenhandler.GetTbt_InventoryBookingDetail(booking.ContractCode, null);
                        foreach (var bookingDtl in newBookingDtl)
                        {
                            bookingDtl.StockOutQty = (
                                from d in previousBooking
                                where d.InstrumentCode == bookingDtl.InstrumentCode
                                select d.StockOutQty
                            ).FirstOrDefault();

                            if ((bookingDtl.BookingQty ?? 0) < (bookingDtl.StockOutQty ?? 0))
                            {
                                bookingDtl.BookingQty = bookingDtl.StockOutQty;
                            }
                        }

                        var lstUpdateBookingDtl = newBookingDtl.Where(d => (d.StockOutQty ?? 0) != 0).ToList();
                        if (lstUpdateBookingDtl != null && lstUpdateBookingDtl.Count > 0)
                        {
                            invenhandler.UpdateTbt_InventoryBookingDetail(lstUpdateBookingDtl);
                        }
                    }
                    #endregion

                    scope.Complete();
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Replace contract data with quotation data. Using when create contract or change contract.
        /// This method can be run on client.
        /// </summary>
        /// <param name="dsQuotation"></param>
        /// <param name="dsSaleContract"></param>
        public void MapFromQuotation(dsQuotationData dsQuotation, ref dsSaleContractData dsSaleContract)
        {
            try
            {
                //1.Update dtTbt_SaleBasic
                if (dsQuotation.dtTbt_QuotationMaintenanceLinkage != null)
                {
                    if (dsQuotation.dtTbt_QuotationMaintenanceLinkage.Count() != 0)
                        dsSaleContract.dtTbt_SaleBasic[0].MaintenanceContractFlag = FlagType.C_FLAG_ON;
                    else
                        dsSaleContract.dtTbt_SaleBasic[0].MaintenanceContractFlag = FlagType.C_FLAG_OFF;
                }
                else
                    dsSaleContract.dtTbt_SaleBasic[0].MaintenanceContractFlag = FlagType.C_FLAG_OFF;

                dsSaleContract.dtTbt_SaleBasic[0].QuotationOfficeCode = dsQuotation.dtTbt_QuotationTarget.QuotationOfficeCode;
                dsSaleContract.dtTbt_SaleBasic[0].OperationOfficeCode = dsQuotation.dtTbt_QuotationTarget.OperationOfficeCode;
                //dsSaleContract.dtTbt_SaleBasic[0].SalesmanEmpNo1 = dsQuotation.dtTbt_QuotationBasic.SalesmanEmpNo1;
                //dsSaleContract.dtTbt_SaleBasic[0].SalesmanEmpNo2 = dsQuotation.dtTbt_QuotationBasic.SalesmanEmpNo2;
                //dsSaleContract.dtTbt_SaleBasic[0].SalesmanEmpNo3 = dsQuotation.dtTbt_QuotationBasic.SalesmanEmpNo3;
                //dsSaleContract.dtTbt_SaleBasic[0].SalesmanEmpNo4 = dsQuotation.dtTbt_QuotationBasic.SalesmanEmpNo4;
                //dsSaleContract.dtTbt_SaleBasic[0].SalesmanEmpNo5 = dsQuotation.dtTbt_QuotationBasic.SalesmanEmpNo5;
                //dsSaleContract.dtTbt_SaleBasic[0].SalesmanEmpNo6 = dsQuotation.dtTbt_QuotationBasic.SalesmanEmpNo6;
                //dsSaleContract.dtTbt_SaleBasic[0].SalesmanEmpNo7 = dsQuotation.dtTbt_QuotationBasic.SalesmanEmpNo7;
                //dsSaleContract.dtTbt_SaleBasic[0].SalesmanEmpNo8 = dsQuotation.dtTbt_QuotationBasic.SalesmanEmpNo8;
                //dsSaleContract.dtTbt_SaleBasic[0].SalesmanEmpNo9 = dsQuotation.dtTbt_QuotationBasic.SalesmanEmpNo9;
                //dsSaleContract.dtTbt_SaleBasic[0].SalesmanEmpNo10 = dsQuotation.dtTbt_QuotationBasic.SalesmanEmpNo10;
                dsSaleContract.dtTbt_SaleBasic[0].QuotationStaffEmpNo = dsQuotation.dtTbt_QuotationTarget.QuotationStaffEmpNo;
                dsSaleContract.dtTbt_SaleBasic[0].NormalProductPrice = dsQuotation.dtTbt_QuotationBasic.ProductPrice;
                dsSaleContract.dtTbt_SaleBasic[0].NormalInstallFee = dsQuotation.dtTbt_QuotationBasic.InstallationFee;
                dsSaleContract.dtTbt_SaleBasic[0].NormalSalePrice = dsQuotation.dtTbt_QuotationBasic.ProductPrice + dsQuotation.dtTbt_QuotationBasic.InstallationFee;
                dsSaleContract.dtTbt_SaleBasic[0].QuotationTargetCode = dsQuotation.dtTbt_QuotationBasic.QuotationTargetCode;
                dsSaleContract.dtTbt_SaleBasic[0].Alphabet = dsQuotation.dtTbt_QuotationBasic.Alphabet;
                dsSaleContract.dtTbt_SaleBasic[0].CreateDate = dsQuotation.dtTbt_QuotationBasic.CreateDate;
                dsSaleContract.dtTbt_SaleBasic[0].BidGuaranteeAmount1 = dsQuotation.dtTbt_QuotationBasic.BidGuaranteeAmount1;
                dsSaleContract.dtTbt_SaleBasic[0].BidGuaranteeAmount2 = dsQuotation.dtTbt_QuotationBasic.BidGuaranteeAmount2;
                dsSaleContract.dtTbt_SaleBasic[0].PlanCode = dsQuotation.dtTbt_QuotationBasic.PlanCode;
                dsSaleContract.dtTbt_SaleBasic[0].SpecialInstallationFlag = dsQuotation.dtTbt_QuotationBasic.SpecialInstallationFlag;
                dsSaleContract.dtTbt_SaleBasic[0].PlannerEmpNo = dsQuotation.dtTbt_QuotationBasic.PlannerEmpNo;
                dsSaleContract.dtTbt_SaleBasic[0].PlanCheckerEmpNo = dsQuotation.dtTbt_QuotationBasic.PlanCheckerEmpNo;
                dsSaleContract.dtTbt_SaleBasic[0].PlanCheckDate = dsQuotation.dtTbt_QuotationBasic.PlanCheckDate;
                dsSaleContract.dtTbt_SaleBasic[0].PlanApproverEmpNo = dsQuotation.dtTbt_QuotationBasic.PlanApproverEmpNo;
                dsSaleContract.dtTbt_SaleBasic[0].PlanApproveDate = dsQuotation.dtTbt_QuotationBasic.PlanApproveDate;
                dsSaleContract.dtTbt_SaleBasic[0].SiteBuildingArea = dsQuotation.dtTbt_QuotationBasic.SiteBuildingArea;
                dsSaleContract.dtTbt_SaleBasic[0].SecurityAreaFrom = dsQuotation.dtTbt_QuotationBasic.SecurityAreaFrom;
                dsSaleContract.dtTbt_SaleBasic[0].SecurityAreaTo = dsQuotation.dtTbt_QuotationBasic.SecurityAreaTo;
                dsSaleContract.dtTbt_SaleBasic[0].MainStructureTypeCode = dsQuotation.dtTbt_QuotationBasic.MainStructureTypeCode;
                dsSaleContract.dtTbt_SaleBasic[0].BuildingTypeCode = dsQuotation.dtTbt_QuotationBasic.BuildingTypeCode;

                //2.Update dtTbt_SaleInstrumentDetails by remove all exiting rows and insert all rows from quotation instead
                //2.1.Update from dtTbt_QuotationInstrumentDetails

                List<tbt_SaleInstrumentDetails> newInstDetail = new List<tbt_SaleInstrumentDetails>();

                foreach (var quoteInstDetail in dsQuotation.dtTbt_QuotationInstrumentDetails)
                {
                    newInstDetail.Add(new tbt_SaleInstrumentDetails()
                    {
                        InstrumentCode = quoteInstDetail.InstrumentCode,
                        InstrumentTypeCode = InstrumentType.C_INST_TYPE_GENERAL,
                        InstrumentQty = quoteInstDetail.InstrumentQty,
                        ContractCode = dsSaleContract.dtTbt_SaleBasic[0].ContractCode,
                        OCC = dsSaleContract.dtTbt_SaleBasic[0].OCC,
                        CreateBy = dsSaleContract.dtTbt_SaleBasic[0].CreateBy,
                        CreateDate = dsSaleContract.dtTbt_SaleBasic[0].CreateDate,
                        UpdateBy = dsSaleContract.dtTbt_SaleBasic[0].UpdateBy,
                        UpdateDate = dsSaleContract.dtTbt_SaleBasic[0].UpdateDate
                    });
                }
                
                //dsSaleContract.dtTbt_SaleInstrumentDetails[0].InstrumentTypeCode = InstrumentType.C_INST_TYPE_GENERAL;
                //if (dsQuotation.dtTbt_QuotationInstrumentDetails != null)
                //{
                //    if (dsQuotation.dtTbt_QuotationInstrumentDetails.Count() != 0)
                //    {
                //        dsSaleContract.dtTbt_SaleInstrumentDetails[0].InstrumentCode = dsQuotation.dtTbt_QuotationInstrumentDetails[0].InstrumentCode;
                //        dsSaleContract.dtTbt_SaleInstrumentDetails[0].InstrumentQty = dsQuotation.dtTbt_QuotationInstrumentDetails[0].InstrumentQty;
                //    }
                //}

                //2.2.Update from dtTbt_QuotationFacilityDetails

                foreach (var quoteFacilityDetail in dsQuotation.dtTbt_QuotationFacilityDetails)
                {
                    newInstDetail.Add(new tbt_SaleInstrumentDetails()
                    {
                        InstrumentCode = quoteFacilityDetail.FacilityCode,
                        InstrumentTypeCode = InstrumentType.C_INST_TYPE_MONITOR,
                        InstrumentQty = quoteFacilityDetail.FacilityQty,
                        ContractCode = dsSaleContract.dtTbt_SaleBasic[0].ContractCode,
                        OCC = dsSaleContract.dtTbt_SaleBasic[0].OCC,
                        CreateBy = dsSaleContract.dtTbt_SaleBasic[0].CreateBy,
                        CreateDate = dsSaleContract.dtTbt_SaleBasic[0].CreateDate,
                        UpdateBy = dsSaleContract.dtTbt_SaleBasic[0].UpdateBy,
                        UpdateDate = dsSaleContract.dtTbt_SaleBasic[0].UpdateDate
                    });
                }

                //if (dsQuotation.dtTbt_QuotationFacilityDetails != null)
                //{
                //    if (dsQuotation.dtTbt_QuotationFacilityDetails.Count() != 0)
                //    {
                //        dsSaleContract.dtTbt_SaleInstrumentDetails[0].InstrumentCode = dsQuotation.dtTbt_QuotationFacilityDetails[0].FacilityCode;
                //        dsSaleContract.dtTbt_SaleInstrumentDetails[0].InstrumentQty = dsQuotation.dtTbt_QuotationFacilityDetails[0].FacilityQty;
                //        dsSaleContract.dtTbt_SaleInstrumentDetails[0].InstrumentTypeCode = InstrumentType.C_INST_TYPE_MONITOR;
                //    }
                //}

                dsSaleContract.dtTbt_SaleInstrumentDetails = newInstDetail;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Get sale basic information for display on Installation page
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strBuildingType"></param>
        /// <returns></returns>
        public dtSaleBasic GetSaleBasicDataForInstall(string strContractCode, string strBuildingType)
        {
            try
            {
                List<dtSaleBasic> dtSaleBasicData = base.GetSaleBasicForInstall(strContractCode, strBuildingType,
                    SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL,
                    SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
                if (dtSaleBasicData.Count != 0)
                {
                    return dtSaleBasicData[0];
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
        /// For register CQ-31
        /// </summary>
        /// <param name="contract"></param>
        public void RegisterCQ31(dsSaleContractData contract)
        {
            using (TransactionScope trans = new TransactionScope())
            {
                contract.dtTbt_SaleBasic[0].DataCorrectionProcessDate = DateTime.Now;
                contract.dtTbt_SaleBasic[0].DataCorrectionProcessEmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;

                this.UpdateEntireContract(contract);

                trans.Complete();
            }
        }

        /// <summary>
        /// Get instrument installed before quantity
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        public List<dtInstrumentInstalledBefore> GetInstrumentInstalledBefore(string strContractCode)
        {
            return base.GetInstrumentInstalledBefore(strContractCode);
        }

        /// <summary>
        /// Get instrument additional installed quantity
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        public List<dtInstrumentAdditionalInstalled> GetInstrumentAdditionalInstalled(string strContractCode)
        {
            return base.GetInstrumentAdditionalInstalled(strContractCode);
        }

        /// <summary>
        /// In case installation type is new sale or add sale, if customer accpetance is registered, return true
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        public bool CheckCanReplaceInstallSlip(string strContractCode)
        {
            try
            {

                if (strContractCode != null)
                {
                    List<CheckCanReplaceInstallSlip_Result> result = base.CheckCanReplaceInstallSlip(strContractCode, SaleChangeType.C_SALE_CHANGE_TYPE_NEW_SALE,SaleChangeType.C_SALE_CHANGE_TYPE_ADD_SALE);
                    if (result.Count > 0)
                    {
                        return Convert.ToBoolean(result[0].blnCanReplaceInstallSlip);
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

        /// <summary>
        /// Get sale instrument detail data
        /// </summary>
        /// <param name="pchrContractCode"></param>
        /// <param name="pchrOCC"></param>
        /// <returns></returns>
        public List<dsSaleInstrumentDetails> GetSaleInstrumentDetails(string pchrContractCode, string pchrOCC)
        {
            return this.GetSaleInstrumentDetails(pchrContractCode, pchrOCC, InstrumentType.C_INST_TYPE_GENERAL, SaleChangeType.C_SALE_CHANGE_TYPE_CANCEL);
        }

        /// <summary>
        /// Get sale basic data for issue invoice 
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="OCC"></param>
        /// <returns></returns>
        public List<doGetSaleDataForIssueInvoice> GetSaleDataForIssueInvoice(string ContractCode, string OCC)
        {
            try
            {
                var result = base.GetSaleDataForIssueInvoice(ContractCode
                                                            , OCC
                                                            , BillingTiming.C_BILLING_TIMING_ACCEPTANCE
                                                            , BillingTemp.C_BILLINGTEMP_FLAG_KEEP
                                                            , SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL
                                                            , SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get linkage sale basic data
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        public List<tbt_SaleBasic> GetLinkageSaleContractData(string strContractCode)
        {
            try
            {
                List<tbt_SaleBasic> result = base.GetLinkageSaleContract(strContractCode, RelationType.C_RELATION_TYPE_SALE);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
