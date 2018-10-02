using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Util;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Quotation;
using SECOM_AJIS.DataEntity.Installation;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class CommonContractHandler : BizCTDataEntities, ICommonContractHandler
    {
        //public override List<dtTbt_BillingTempListForView> GetTbt_BillingTempListForView(string pContractCode, string pOCC)
        //{
        //    return base.GetTbt_BillingTempListForView(pContractCode, pOCC);
        //}

        /// <summary>
        /// Get billing temp from both billing temp table and billing basic
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <returns></returns>
        public List<tbt_BillingTemp> GetTbt_BillingTargetForEditing(string strContractCode, string strOCC)
        {
            IBillingInterfaceHandler billinginterfacehandler = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
            List<tbt_BillingTemp> resLst = new List<tbt_BillingTemp>();

            List<tbt_BillingTemp> localBilling = GetTbt_BillingTemp(strContractCode, strOCC);
            // filter data
            localBilling = (from a in localBilling
                            where
                                ((a.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE)
                                || (a.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE)
                                || (a.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE)
                                || (a.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_STOP_FEE)
                                //|| (a.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_SALE_PRICE)
                                || (a.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_PRODUCT_PRICE)
                                || (a.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_ONE))
                            select a).ToList();

            List<tbt_BillingTemp> mainBilling = billinginterfacehandler.GetBillingBasicAsBillingTemp(strContractCode, strOCC);

            //Modify by Jutarat A. on 16072012
            //For merge localBilling and mainBilling
            //resLst.AddRange(localBilling);

            //var diffSet = from a in mainBilling
            //              where
            //                  localBilling.Where(x =>
            //                  (x.BillingOCC != a.BillingOCC)
            //                  && (x.BillingType == a.BillingType)).Count() == 0
            //              select a;

            //if (diffSet.Count() > 0)
            //{
            //    int maxSeq = localBilling.Max(x => x.SequenceNo);

            //    foreach (var item in diffSet)
            //    {
            //        item.SequenceNo = ++maxSeq;
            //        //item.FromBillingModule = true;
            //    }

            //    resLst.AddRange(diffSet.ToList());
            //}
            var lstBillingBasic = localBilling.Union(mainBilling).ToList();
            var uniqueBillingBasic = (from t in lstBillingBasic
                                      group t by new
                                      {
                                          ContractCode = t.ContractCode,
                                          BillingOCC = t.BillingOCC,
                                          BillingClientCode = t.BillingClientCode,
                                          BillingTargetCode = t.BillingTargetCode,
                                          BillingOfficeCode = t.BillingOfficeCode
                                      } into g
                                      select g.FirstOrDefault()).OrderBy(p => p.BillingOCC);

            resLst = uniqueBillingBasic.ToList<tbt_BillingTemp>();
            //End Modify

            return resLst;
        }

        //public List<tbt_BillingTemp> GetTbt_BillingTargetForEditing(string strContractCode, string strOCC)
        //{
        //    tbt_BillingTemp tbtBillingTemp;
        //    List<tbt_BillingTemp> listBillingTemp;

        //    try
        //    {
        //        //ข้อมูลส่วนเเรกจะเป็นข้อมูลของ BEFORE START ทำการดึงจาก BILLINGTEMP
        //        listBillingTemp = this.GetBillingTargetForEditing(strContractCode, strOCC,
        //                          ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE,
        //                          ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE,
        //                          ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_CON,
        //                          ContractBillingType.C_CONTRACT_BILLING_TYPE_STOP_FEE,
        //                          ContractBillingType.C_CONTRACT_BILLING_TYPE_SALE_PRICE);

        //        //ข้อมูลส่วนที่สองจะเป็นข้อมูลของ AFTER START ทำการดึงจาก BILLINGBASIC เเต่เนื่องจาก Table ยังไม่ถูกสร้างจะทำการจำลองไว้สำหรับเทส AFTER START
        //        tbtBillingTemp = new tbt_BillingTemp();
        //        tbtBillingTemp.ContractCode = "N0000000002";
        //        tbtBillingTemp.SequenceNo = 1;
        //        tbtBillingTemp.OCC = "0001";
        //        tbtBillingTemp.BillingOCC = "";
        //        tbtBillingTemp.BillingClientCode = "0000000002";
        //        tbtBillingTemp.BillingOfficeCode = "1000";
        //        tbtBillingTemp.BillingTargetCode = "";
        //        tbtBillingTemp.BillingAmt = 6000;
        //        tbtBillingTemp.PayMethod = "3";
        //        tbtBillingTemp.BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE;
        //        tbtBillingTemp.BillingTiming = null;

        //        //Merge after start and before start
        //        //listBillingTemp.Add(tbtBillingTemp);

        //        return listBillingTemp;

        //    }
        //    catch (Exception ex)
        //    {                
        //        throw;
        //    }
        //}

        /// <summary>
        /// Get sub contractor data
        /// </summary>
        /// <param name="pSubContractorCode"></param>
        /// <returns></returns>
        public List<tbm_SubContractor> GetTbm_SubContractorData(string pSubContractorCode)
        {
            try
            {
                List<tbm_SubContractor> lst = base.GetTbm_SubContractor(pSubContractorCode);
                if (lst == null)
                    lst = new List<tbm_SubContractor>();
                else
                    CommonUtil.MappingObjectLanguage<tbm_SubContractor>(lst);

                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get the records from Tbt_RelationType which link from the specified contract code
        /// </summary>
        /// <param name="pSubContractorCode"></param>
        /// <param name="paramOCC"></param>
        /// <param name="paramRelationType"></param>
        /// <returns></returns>
        public List<tbt_RelationType> GetContractLinkageRelation(string pSubContractorCode, string paramOCC, string paramRelationType)
        {
            try
            {
                return base.GetTbt_GetContractLinkageRelation(pSubContractorCode, paramOCC, paramRelationType);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Delete contract email
        /// </summary>
        /// <param name="contractEmailID"></param>
        /// <param name="empNo"></param>
        /// <returns></returns>
        public List<tbt_ContractEmail> DeleteTbt_ContractEmail(int contractEmailID, string empNo = null)
        {
            try
            {
                //Delete data from DB
                List<tbt_ContractEmail> deletedList = base.DeleteTbt_ContractEmail(contractEmailID);

                //Insert Log
                if (deletedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                    logData.TableName = TableName.C_TBL_NAME_CON_EMAIL;
                    logData.TableData = CommonUtil.ConvertToXml(deletedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData, empNo);
                }

                return deletedList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Delete the contract email of specified type which not sent
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="emailType"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public List<tbt_ContractEmail> DeleteTbt_ContractEmailUnsentContractEmail(string contractCode, string emailType, bool flag)
        {
            try
            {
                //Delete data from DB
                List<tbt_ContractEmail> deletedList = base.DeleteTbt_ContractEmail_UnsentContractEmail(contractCode, emailType, flag);

                //Insert Log
                if (deletedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                    logData.TableName = TableName.C_TBL_NAME_CON_EMAIL;
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
        /// Update contract email
        /// </summary>
        /// <param name="doUpdate"></param>
        /// <returns></returns>
        public List<tbt_ContractEmail> UpdateTbt_ContractEmail(tbt_ContractEmail doUpdate)
        {
            bool isBatch = false;
            try
            {
                //Check whether this record is the most updated data
                List<tbt_ContractEmail> rList = this.GetTbt_ContractEmail(doUpdate.ContractEmailID);
                //if (rList[0].UpdateDate != doUpdate.UpdateDate)
                if (DateTime.Compare(rList[0].UpdateDate.Value, doUpdate.UpdateDate.Value) != 0)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                }

                //set updateDate and updateBy

                try
                {
                    doUpdate.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    doUpdate.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                }
                catch (Exception) //Call by Batch
                {
                    isBatch = true;
                    doUpdate.UpdateDate = DateTime.Now;
                }


                List<tbt_ContractEmail> doUpdateList = new List<tbt_ContractEmail>();
                doUpdateList.Add(doUpdate);
                List<tbt_ContractEmail> updatedList = base.UpdateTbt_ContractEmail(CommonUtil.ConvertToXml_Store<tbt_ContractEmail>(doUpdateList));

                //Insert Log
                if (updatedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Update;
                    logData.TableName = TableName.C_TBL_NAME_CON_EMAIL;
                    logData.TableData = CommonUtil.ConvertToXml(updatedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    if (isBatch)
                        hand.WriteTransactionLog(logData, doUpdate.UpdateBy, "CMS050");
                    else
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
        /// Insert relation type
        /// </summary>
        /// <param name="doRelationType"></param>
        /// <returns></returns>
        public List<tbt_RelationType> InsertTbt_RelationType(tbt_RelationType doRelationType)
        {
            try
            {
                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                doRelationType.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doRelationType.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                doRelationType.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doRelationType.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_RelationType> doInsertList = new List<tbt_RelationType>();
                doInsertList.Add(doRelationType);
                List<tbt_RelationType> insertList = base.InsertTbt_RelationType(CommonUtil.ConvertToXml_Store<tbt_RelationType>(doInsertList));

                //Insert Log
                if (insertList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_RELATION_TYPE;
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
        /// Insert relation type (list)
        /// </summary>
        /// <param name="relationTypeList"></param>
        /// <returns></returns>
        public int InsertTbt_RelationType(List<tbt_RelationType> relationTypeList)
        {
            try
            {
                if (relationTypeList != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    foreach (tbt_RelationType relationType in relationTypeList)
                    {
                        relationType.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                        relationType.CreateBy = dsTrans.dtUserData.EmpNo;
                        relationType.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                        relationType.UpdateBy = dsTrans.dtUserData.EmpNo;
                    }
                }

                List<tbt_RelationType> res = this.InsertTbt_RelationType(
                    CommonUtil.ConvertToXml_Store<tbt_RelationType>(relationTypeList));

                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Insert,
                    TableName = TableName.C_TBL_NAME_RELATION_TYPE,
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

        /// <summary>
        /// Update relation type
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="relationTypeList"></param>
        /// <returns></returns>
        public int UpdateTbt_RelationType(string ContractCode, List<tbt_RelationType> relationTypeList)
        {
            try
            {
                //List<tbt_RelationType> res = this.DeleteTbt_RelationType(ContractCode);

                //#region Log

                //doTransactionLog logData = new doTransactionLog()
                //{
                //    TransactionType = doTransactionLog.eTransactionType.Delete,
                //    TableName = TableName.C_TBL_NAME_RELATION_TYPE,
                //    TableData = CommonUtil.ConvertToXml(res)
                //};

                //ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                //hand.WriteTransactionLog(logData);

                //#endregion
                List<tbt_RelationType> res = DeleteTbtRelationType(ContractCode);

                return InsertTbt_RelationType(relationTypeList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Delete relation type
        /// </summary>
        /// <param name="pchrContractCode"></param>
        /// <returns></returns>
        public List<tbt_RelationType> DeleteTbtRelationType(string contractCode)
        {
            try
            {
                //Delete data from DB
                List<tbt_RelationType> deletedList = base.DeleteTbt_RelationType(contractCode);

                //Insert Log
                if (deletedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                    logData.TableName = TableName.C_TBL_NAME_RELATION_TYPE;
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
        /// To generate doRelationType from list of MA target contract. Using for update relation type from quotation
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="MATargetContract"></param>
        /// <param name="BeforeStartFlag"></param>
        /// <returns></returns>
        public List<tbt_RelationType> GenerateMaintenanceRelationType(string ContractCode, List<string> MATargetContract, bool BeforeStartFlag = false)
        {
            try
            {
                List<tbt_RelationType> lst = new List<tbt_RelationType>();

                List<tbt_SaleBasic> mLst = new List<tbt_SaleBasic>();
                if (MATargetContract != null)
                {
                    foreach (string code in MATargetContract)
                    {
                        mLst.Add(new tbt_SaleBasic()
                        {
                            ContractCode = code,
                            OCC = "0001"
                        });
                    }
                    List<doMaintenanceRelationType> mtLst = this.GenerateMaintenanceRelationType(CommonUtil.ConvertToXml_Store<tbt_SaleBasic>(mLst, "ContractCode", "OCC"), BeforeStartFlag);
                    if (mtLst.Count > 0)
                    {

                        foreach (string code in MATargetContract)
                        {
                            string RelatedOCC = null;
                            foreach (doMaintenanceRelationType ma in mtLst)
                            {
                                if (ma.ContractCode == code)
                                {
                                    RelatedOCC = ma.LastOCC;
                                    break;
                                }
                            }
                            if (RelatedOCC != null)
                            {
                                lst.Add(new tbt_RelationType()
                                {
                                    ContractCode = ContractCode,
                                    OCC = null,
                                    RelatedContractCode = code,
                                    RelatedOCC = RelatedOCC,
                                    RelationType = RelationType.C_RELATION_TYPE_MA
                                });
                            }
                        }
                    }
                }

                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Insert contract customer history
        /// </summary>
        /// <param name="docLst"></param>
        /// <returns></returns>
        public List<tbt_ContractCustomerHistory> InsertTbt_ContractCustomerHistory(List<tbt_ContractCustomerHistory> docLst)
        {
            try
            {
                if (docLst != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    foreach (tbt_ContractCustomerHistory doc in docLst)
                    {
                        doc.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                        doc.CreateBy = dsTrans.dtUserData.EmpNo;
                        doc.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                        doc.UpdateBy = dsTrans.dtUserData.EmpNo;
                    }
                }

                return this.InsertTbt_ContractCustomerHistory(CommonUtil.ConvertToXml_Store<tbt_ContractCustomerHistory>(docLst));
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Check exist contract code (Both rental and sale) 
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        public bool IsContractExistInRentalOrSale(string strContractCode)
        {
            bool bResult = false;

            IRentralContractHandler rentralContractHandler;
            ISaleContractHandler saleContractHandler;
            List<tbt_RentalContractBasic> listRentalContractBasic_Code, listRentalContractBasic_UserCode;
            List<tbt_SaleBasic> listSaleBasic;

            try
            {
                rentralContractHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                saleContractHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;

                listRentalContractBasic_Code = rentralContractHandler.GetTbt_RentalContractBasic(strContractCode, null);
                listRentalContractBasic_UserCode = rentralContractHandler.GetTbt_RentalContractBasic(null, strContractCode);

                if ((listRentalContractBasic_Code != null && listRentalContractBasic_Code.Count > 0) || (listRentalContractBasic_UserCode != null && listRentalContractBasic_UserCode.Count > 0))
                {
                    bResult = true;
                }
                else
                {
                    listSaleBasic = saleContractHandler.GetTbt_SaleBasic(strContractCode, null, FlagType.C_FLAG_ON);
                    if (listSaleBasic != null && listSaleBasic.Count > 0)
                    {
                        bResult = true;
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return bResult;
        }

        /// <summary>
        /// Get service type code and product type code by contract code or project code
        /// </summary>
        /// <param name="strCode"></param>
        /// <returns></returns>
        public List<doServiceProductTypeCode> GetServiceProductTypeCode(string strCode)
        {
            try
            {
                List<doServiceProductTypeCode> lst = base.GetServiceProductTypeCode(strCode, ServiceType.C_SERVICE_TYPE_PROJECT);
                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Check installation complete remove all 
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        public bool IsCompleteRemoveAll(string strContractCode)
        {
            var res = base.IsCompleteRemoveAll(strContractCode, RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_ALL);
            return (res.Count == 1)
                ? res[0].GetValueOrDefault()
                : false;
        }
        
        public void UpdateOperationOffice(string contractCode, string operationOfficeCode)
        {
            try
            {
                if (string.IsNullOrEmpty(contractCode) || string.IsNullOrEmpty(operationOfficeCode))
                {
                    return;
                }

                ISaleContractHandler saleHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                var lstSales = saleHandler.GetTbt_SaleBasic(contractCode, null, null);
                if (lstSales != null && lstSales.Count > 0)
                {
                    foreach (var sale in lstSales)
                    {
                        sale.OperationOfficeCode = operationOfficeCode;
                        saleHandler.UpdateTbt_SaleBasic(sale);
                    }
                }

                IQuotationHandler quotationHandler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;

                var lstQuotationTarget = quotationHandler.GetTbt_QuotationTargetByContractCode(contractCode);
                if (lstQuotationTarget != null && lstQuotationTarget.Count > 0)
                {
                    foreach (var q in lstQuotationTarget)
                    {
                        quotationHandler.UpdateQuotationTarget(new doUpdateQuotationTargetData()
                        {
                            QuotationTargetCode = q.QuotationTargetCode,
                            ContractTransferStatus = q.ContractTransferStatus,
                            ContractCode = q.ContractCode,
                            TransferDate = q.TransferDate,
                            TransferAlphabet = q.TransferAlphabet,
                            LastAlphabet = q.LastAlphabet,
                            QuotationOfficeCode = q.QuotationTargetCode,
                            OperationOfficeCode = operationOfficeCode,
                        });
                    }
                }

                IInstallationHandler installHandler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;

                var lstInstallationBasic = installHandler.GetTbt_InstallationBasicData(contractCode);
                if (lstInstallationBasic != null && lstInstallationBasic.Count > 0)
                {
                    foreach (var ib in lstInstallationBasic)
                    {
                        ib.OperationOfficeCode = operationOfficeCode;
                        installHandler.UpdateTbt_InstallationBasic(ib);
                    }
                }

                var lstInstallationHistory = installHandler.GetTbt_InstallationHistory(contractCode, null, null);
                if (lstInstallationHistory != null && lstInstallationHistory.Count > 0)
                {
                    foreach (var ih in lstInstallationHistory)
                    {
                        ih.OperationOfficeCode = operationOfficeCode;
                    }

                    installHandler.UpdateTbt_InstallationHistory(lstInstallationHistory);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
