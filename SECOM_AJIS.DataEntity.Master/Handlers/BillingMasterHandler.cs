using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Common;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Models;


namespace SECOM_AJIS.DataEntity.Master
{
    public class BillingMasterHandler : BizMADataEntities, IBillingMasterHandler
    {
        /// <summary>
        /// Manage billing client
        /// </summary>
        /// <param name="billingClient"></param>
        /// <returns></returns>
        public string ManageBillingClient(tbm_BillingClient billingClient)
        {
            var oldBillingClientCode = billingClient.BillingClientCode;
            try
            {
                //1.	Generate billing client code
                string strBillingClientCode = GenerateBillingClientCode();

                //2.	Insert billing client data
                billingClient.BillingClientCode = strBillingClientCode;
                InsertBillingClient(billingClient);

                return strBillingClientCode;
            }
            catch (Exception)
            {
                billingClient.BillingClientCode = oldBillingClientCode;
                throw;
            }
        }

        /// <summary>
        /// Generate billing client code
        /// </summary>
        /// <returns></returns>
        public string GenerateBillingClientCode()
        {
            string strBillingClientCode = string.Empty;
            try
            {
                //1. Get next running no
                ICommonHandler handler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doRunningNo> run = handler.GetNextRunningCode(NameCode.C_NAME_CODE_BILLING_CLIENT_CODE);

                if (run.Count > 0)
                {
                    string strRunningNo = run[0].RunningNo;

                    //2.	Generate check digit 
                    string strCheckDigit = handler.GenerateCheckDigit(strRunningNo);

                    //3.	Set strBillingClientCode =strRunningNo+ strCheckDigit
                    strBillingClientCode = strRunningNo + strCheckDigit;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return strBillingClientCode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="billingClient"></param>
        /// <returns></returns>
        public List<tbm_BillingClient> InsertBillingClient(tbm_BillingClient billingClient)
        {
            try
            {
                //1.	Check Mandatory Fields for input parameter.
                //BillingClientCode, CustTypeCode, NameEN, NameLC, AddressEN, AddressLC
                ApplicationErrorException.CheckMandatoryField<tbm_BillingClient, tbm_BillingClientCondition>(billingClient);

                dsTransDataModel dsTrans = CommonUtil.dsTransData;
                billingClient.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                billingClient.CreateBy = dsTrans.dtUserData.EmpNo;
                billingClient.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                billingClient.UpdateBy = dsTrans.dtUserData.EmpNo;

                List<tbm_BillingClient> insertList = this.InsertBillingClient(billingClient.BillingClientCode,
                                                                        billingClient.NameEN,
                                                                        billingClient.NameLC,
                                                                        billingClient.FullNameEN,
                                                                        billingClient.FullNameLC,
                                                                        billingClient.BranchNo,
                                                                        billingClient.BranchNameEN,
                                                                        billingClient.BranchNameLC,
                                                                        billingClient.CustTypeCode,
                                                                        billingClient.CompanyTypeCode,
                                                                        billingClient.BusinessTypeCode,
                                                                        billingClient.PhoneNo,
                                                                        billingClient.IDNo,
                                                                        billingClient.RegionCode,
                                                                        billingClient.AddressEN,
                                                                        billingClient.AddressLC,
                                                                        billingClient.CreateDate,
                                                                        billingClient.CreateBy,
                                                                        billingClient.UpdateDate,
                                                                        billingClient.UpdateBy,
                                                                        billingClient.DeleteFlag);
                //Insert Log
                if (insertList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_BILLING_CLIENT;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Insert billing client
        /// </summary>
        /// <param name="billingClientCode"></param>
        /// <returns></returns>
        public List<dtBillingClientData> GetBillingClient(string billingClientCode)
        {
            // Akat K. 2011-10-27 : add mandatory check
            if (billingClientCode == null)
            {
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, "siteCode");
            }

            List<dtBillingClientData> listBillingClientData;
            try
            {
                listBillingClientData = base.GetBillingClient(MiscType.C_CUST_TYPE, billingClientCode);

                CommonUtil.MappingObjectLanguage<dtBillingClientData>(listBillingClientData);

                return listBillingClientData;
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Get billing type data by billing type code condition
        /// </summary>
        /// <param name="BillingTypeCode"></param>
        /// <returns></returns>
        public tbm_BillingType GetTbm_BillingType(string BillingTypeCode)
        {
            try
            {
                List<tbm_BillingType> result = base.GetTbm_BillingType(BillingTypeCode);
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
        /// Get billing type data
        /// </summary>
        /// <returns></returns>
        public List<tbm_BillingType> GetTbm_BillingType()
        {
            try
            {
                List<tbm_BillingType> result = base.GetTbm_BillingType(null);
                if (result.Count > 0)
                {
                    return result;
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
        /// Get auto transfer date
        /// </summary>
        /// <param name="strBankCode"></param>
        /// <param name="strAutoTransferDate"></param>
        /// <returns></returns>
        public DateTime? GetAutoTransferDate(string strBankCode, string strAutoTransferDate)
        {
            List<DateTime?> dtNextAutoTransferDate = base.GetAutoTransferDate(strBankCode, strAutoTransferDate);
            if (dtNextAutoTransferDate.Count > 0)
            {
                return dtNextAutoTransferDate[0];
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// Get list of billing type onetime fee
        /// </summary>
        /// <param name="BillingServiceTypeCode"></param>
        /// <returns></returns>
        public List<tbm_BillingType> GetBillingTypeOneTimeListData(string BillingServiceTypeCode)
        {
            List<tbm_BillingType> result = base.GetBillingTypeOneTimeList(BillingServiceTypeCode);
            return result;
        }
    }
}
