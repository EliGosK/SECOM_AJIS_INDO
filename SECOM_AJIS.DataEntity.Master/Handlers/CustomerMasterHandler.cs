using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.DataEntity.Common;

using SECOM_AJIS.Common.Util.ConstantValue;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.Common.Models;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SECOM_AJIS.DataEntity.Master
{
    public class CustomerMasterHandler : BizMADataEntities, ICustomerMasterHandler
    {
        /// <summary>
        /// Check duplicate customer
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public bool CheckDuplicateCustomer(doCustomer customer)
        {
            try
            {
                doCustomer cust = CommonUtil.CloneObject<doCustomer, doCustomer>(customer);
                if (cust != null)
                {
                    if (CommonUtil.IsNullOrEmpty(cust.CustCode) == false
                        || CommonUtil.IsNullOrEmpty(cust.CustNameLC) == false)
                    {
                        for (int i = 32; i <= 125; i++)
                        {
                            string key = null;
                            if ((i >= 32 && i <= 46)
                                    || (i >= 58 && i <= 64)
                                    || (i >= 91 && i <= 96)
                                    || (i >= 123 && i <= 125))
                            {
                                key = Char.ConvertFromUtf32(i);
                            }
                            if (CommonUtil.IsNullOrEmpty(key) == false)
                            {
                                if (CommonUtil.IsNullOrEmpty(cust.CustCode) == false)
                                    cust.CustCode = cust.CustCode.Replace(key, "");
                                if (CommonUtil.IsNullOrEmpty(cust.CustNameLC) == false)
                                    cust.CustNameLC = cust.CustNameLC.Replace(key, "");
                            }
                        }
                    }

                    List<int?> lst = base.CheckDuplicateCustomer(CustomerStatus.C_CUST_STATUS_EXIST,
                                                    cust.CustCode,
                                                    cust.IDNo,
                                                    cust.CustNameLC);
                    if (lst.Count > 0)
                    {
                        if (lst[0] == 1)
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1003);
                        else if (lst[0] == 2)
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1004);
                    }
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Check duplicate customer (customer name LOCAL)
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public bool CheckDuplicateCustomer_CustNameLC(doCustomer customer)
        {
            try
            {
                doCustomer cust = CommonUtil.CloneObject<doCustomer, doCustomer>(customer);
                if (cust != null)
                {
                    //32  AND 46 	= 	space !"#$%&'()*+,-.
                    //58  AND 64 	= 	:;<=>?@
                    //91  AND 96 	= 	[\]^_`
                    //123 AND 125 	= 	{|}

                    if (CommonUtil.IsNullOrEmpty(cust.CustNameLC) == false)
                        cust.CustNameLC = Regex.Replace(cust.CustNameLC, "[ !\"#$%&'()*+,-.:;<=>?@[\\]^_`{|}]", "");

                    List<int?> lst = base.CheckDuplicateCustomer_CustNameLC(
                                        CustomerStatus.C_CUST_STATUS_EXIST,
                                        cust.CustCode, cust.CustNameLC);

                    if (lst.Count > 0 && (lst[0] == 1))
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Check duplicate customer (ID No)
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public bool CheckDuplicateCustomer_IDNo(doCustomer customer)
        {
            try
            {
                doCustomer cust = CommonUtil.CloneObject<doCustomer, doCustomer>(customer);

                List<int?> lst = base.CheckDuplicateCustomer_IDNo(
                                    CustomerStatus.C_CUST_STATUS_EXIST,
                                    cust.CustCode, cust.IDNo);

                if (lst.Count > 0 && (lst[0] == 1))
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Manage Customer Information
        /// </summary>
        /// <param name="strCustCode"></param>
        public void ManageCustomerInformation(string strCustCode)
        {
            // Akat K. 2011-10-27 : add mandatory check
            if (strCustCode == null)
            {
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, "strCustCode");
            }

            try
            {
                List<dtManageCustomerInformation> info = base.ManageCustomerInformation(strCustCode, CustomerStatus.C_CUST_STATUS_NEW);
                if (info.Count > 0)
                    UpdateCustomerStatus(CustomerStatus.C_CUST_STATUS_EXIST, CustomerStatus.C_CUST_STATUS_NEW, strCustCode,
                        CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Getting customer data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public List<dtCustomerData> GetCustomerDataForSearch(doCustomerSearchCondition cond)
        {
            try
            {
                return base.GetCustomerDataForSearch(cond.CustomerCode,
                                                     cond.CustomerName,
                                                     cond.CompanyTypeCode,
                                                     cond.IDNo,
                                                     cond.DummyIDFlag,
                                                     cond.RegionCode,
                                                     cond.BusinessTypeCode,
                                                     cond.Address,
                                                     cond.Alley,
                                                     cond.Road,
                                                     cond.SubDistrict,
                                                     cond.DistrictCode,
                                                     cond.ProvinceCode,
                                                     cond.ZipCode,
                                                     cond.TelephoneNo,
                                                     cond.GroupName,
                                                     MiscType.C_CUST_STATUS,
                                                     MiscType.C_CUST_TYPE,
                                                     MiscType.C_FINANCIAL_MARKET_TYPE,
                                                     cond.CustStatus,//xmlCustStatus, 
                                                     cond.CustomerTypeCode); // xmlCustTypeCode
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// Get customer for view
        /// </summary>
        /// <param name="chrCustCode"></param>
        /// <param name="pC_CUST_STATUS"></param>
        /// <param name="pC_CUST_TYPE"></param>
        /// <param name="pC_FINANCIAL_MARKET_TYPE"></param>
        /// <returns></returns>
        public override List<dtCustomerForView> GetCustomerForView(string chrCustCode, string pC_CUST_STATUS, string pC_CUST_TYPE, string pC_FINANCIAL_MARKET_TYPE)
        {
            try
            {
                return base.GetCustomerForView(chrCustCode, pC_CUST_STATUS, pC_CUST_TYPE, pC_FINANCIAL_MARKET_TYPE);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// Generate customer code
        /// </summary>
        /// <returns></returns>
        public string GenerateCustomerCode()
        {
            try
            {
                ICommonHandler handler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doRunningNo> run = handler.GetNextRunningCode(NameCode.C_NAME_CODE_CUSTOMER_CODE);
                if (run.Count > 0)
                {
                    string digit = handler.GenerateCheckDigit(run[0].RunningNo);
                    return CommonValue.C_CUST_CODE_PREFIX + run[0].RunningNo + digit;
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        /// <summary>
        /// Generate site code
        /// </summary>
        /// <param name="CustCode"></param>
        /// <param name="site"></param>
        /// <returns></returns>
        public bool GenerateSiteCode(string CustCode, doSite site)
        {
            try
            {
                List<int?> lst = this.GenerateSiteCode(CustCode);
                if (lst.Count > 0)
                {
                    string siteCode = CommonValue.C_SITE_CODE_PREFIX + CustCode.Substring(CustCode.Length - 10, 10);

                    // Akat K. change from PadRight to PadLeft
                    //site.SiteNo = (lst[0] + 1).ToString().PadRight(4, '0');
                    site.SiteNo = (lst[0] + 1).ToString().PadLeft(4, '0');
                    site.SiteCode = string.Format("{0}-{1}", siteCode, site.SiteNo);

                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Generate dummy ID
        /// </summary>
        /// <returns></returns>
        public string GenerateDummyID()
        {
            try
            {
                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doRunningNo> run = hand.GetNextRunningCode(NameCode.C_NAME_CODE_DUMMY_ID);
                if (run.Count > 0)
                    return CommonValue.C_DUMMY_ID_PREFIX + run[0].RunningNo;

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        /// <summary>
        /// Insert customer
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public List<tbm_Customer> InsertCustomer(doCustomer customer)
        {
            try
            {
                dsTransDataModel dsTrans = CommonUtil.dsTransData;
                customer.CustStatus = CustomerStatus.C_CUST_STATUS_NEW;
                customer.ImportantFlag = false;
                customer.DummyIDFlag = customer.DummyIDFlag == null ? false : customer.DummyIDFlag;
                customer.DeleteFlag = false;
                customer.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                customer.CreateBy = dsTrans.dtUserData.EmpNo;
                customer.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                customer.UpdateBy = dsTrans.dtUserData.EmpNo;

                List<tbm_Customer> lst = this.InsertCustomer(customer.CustCode,
                                                                customer.CustStatus,
                                                                customer.ImportantFlag,
                                                                customer.CustNameEN,
                                                                customer.CustNameLC,
                                                                customer.CustFullNameEN,
                                                                customer.CustFullNameLC,
                                                                customer.RepPersonName,
                                                                customer.ContactPersonName,
                                                                customer.SECOMContactPerson,
                                                                customer.CustTypeCode,
                                                                customer.CompanyTypeCode,
                                                                customer.FinancialMarketTypeCode,
                                                                customer.BusinessTypeCode,
                                                                customer.PhoneNo,
                                                                customer.FaxNo,
                                                                customer.IDNo,
                                                                customer.DummyIDFlag,
                                                                customer.RegionCode,
                                                                customer.URL,
                                                                customer.Memo,
                                                                customer.AlleyEN,
                                                                customer.AddressEN,
                                                                customer.RoadEN,
                                                                customer.SubDistrictEN,
                                                                customer.AddressFullEN,
                                                                customer.AddressLC,
                                                                customer.AlleyLC,
                                                                customer.RoadLC,
                                                                customer.SubDistrictLC,
                                                                customer.AddressFullLC,
                                                                customer.DistrictCode,
                                                                customer.ProvinceCode,
                                                                customer.ZipCode,
                                                                customer.DeleteFlag,
                                                                customer.CreateDate,
                                                                customer.CreateBy,
                                                                customer.UpdateDate,
                                                                customer.UpdateBy);
                //Insert Log
                if (lst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_CUSTOMER;
                    logData.TableData = CommonUtil.ConvertToXml(lst);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        /// <summary>
        /// Update customer
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public List<tbm_Customer> UpdateCustomer(doCustomer customer)
        {
            try
            {
                //Check whether this record is the most updated data
                List<tbm_Customer> sList = this.GetTbm_Customer(customer.CustCode);

                if (sList == null || sList.Count == 0 || sList[0] == null || sList[0].UpdateDate == null
                    || customer.UpdateDate == null || !customer.UpdateDate.HasValue)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "UpdateDate" });
                }

                if (DateTime.Compare(sList[0].UpdateDate.Value, customer.UpdateDate.Value) != 0)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                }

                dsTransDataModel dsTrans = CommonUtil.dsTransData;
                //customer.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                //customer.UpdateBy = dsTrans.dtUserData.EmpNo;

                List<tbm_Customer> lst = this.UpdateCustomer(customer.CustCode,
                                                                customer.CustStatus,
                                                                customer.ImportantFlag,
                                                                customer.CustNameEN,
                                                                customer.CustNameLC,
                                                                customer.CustFullNameEN,
                                                                customer.CustFullNameLC,
                                                                customer.RepPersonName,
                                                                customer.ContactPersonName,
                                                                customer.SECOMContactPerson,
                                                                customer.CustTypeCode,
                                                                customer.CompanyTypeCode,
                                                                customer.FinancialMarketTypeCode,
                                                                customer.BusinessTypeCode,
                                                                customer.PhoneNo,
                                                                customer.FaxNo,
                                                                customer.IDNo,
                                                                customer.DummyIDFlag,
                                                                customer.RegionCode,
                                                                customer.URL,
                                                                customer.Memo,
                                                                customer.AlleyEN,
                                                                customer.AddressEN,
                                                                customer.RoadEN,
                                                                customer.SubDistrictEN,
                                                                customer.AddressFullEN,
                                                                customer.AddressLC,
                                                                customer.AlleyLC,
                                                                customer.RoadLC,
                                                                customer.SubDistrictLC,
                                                                customer.AddressFullLC,
                                                                customer.DistrictCode,
                                                                customer.ProvinceCode,
                                                                customer.ZipCode,
                                                                customer.DeleteFlag,
                                                                dsTrans.dtOperationData.ProcessDateTime,
                                                                dsTrans.dtUserData.EmpNo);

                //Insert Log
                if (lst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Update;
                    logData.TableName = TableName.C_TBL_NAME_CUSTOMER;
                    logData.TableData = CommonUtil.ConvertToXml(lst);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        /// <summary>
        /// Delete customer group
        /// </summary>
        /// <param name="groupList"></param>
        /// <returns></returns>
        public int DeleteCustomerGroup(List<dtCustomeGroupData> groupList)
        {
            try
            {
                foreach (dtCustomeGroupData group in groupList)
                {
                    ApplicationErrorException.CheckMandatoryField<dtCustomeGroupData, dtCustomerGroupDataCondition>(group);
                }

                List<tbm_CustomerGroup> lst = this.DeleteCustomerGroup(CommonUtil.ConvertToXml_Store<dtCustomeGroupData>(groupList));
                return lst.Count;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        /// <summary>
        /// Insert customer group
        /// </summary>
        /// <param name="groupList"></param>
        /// <returns></returns>
        public int InsertCustomerGroup(List<dtCustomeGroupData> groupList)
        {
            try
            {
                List<tbm_CustomerGroup> gLst = CommonUtil.ClonsObjectList<dtCustomeGroupData, tbm_CustomerGroup>(groupList);

                dsTransDataModel dsTrans = CommonUtil.dsTransData;
                foreach (tbm_CustomerGroup group in gLst)
                {
                    ApplicationErrorException.CheckMandatoryField<tbm_CustomerGroup, dtCustomerGroupDataCondition>(group);

                    group.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                    group.CreateBy = dsTrans.dtUserData.EmpNo;
                    group.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                    group.UpdateBy = dsTrans.dtUserData.EmpNo;
                }

                List<tbm_CustomerGroup> lst = InsertCustomerGroup(CommonUtil.ConvertToXml_Store<tbm_CustomerGroup>(gLst));
                return lst.Count;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Manage customer target
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public doCustomerTarget ManageCustomerTarget(doCustomerTarget cond)
        {
            try
            {
                // Akat K. check mandatory field
                List<string> messageParam = new List<string>();
                if (cond.doCustomer.CustNameEN == null)
                {
                    messageParam.Add("CustNameEN");
                }
                //if (cond.doCustomer.CustNameLC == null)
                //{
                //    messageParam.Add("CustNameLC");
                //}
                if (cond.doCustomer.CustTypeCode == null)
                {
                    messageParam.Add("CustTypeCode");
                }
                if (cond.doCustomer.BusinessTypeCode == null)
                {
                    messageParam.Add("BusinessTypeCode");
                }
                if (cond.doCustomer.RegionCode == null)
                {
                    messageParam.Add("RegionCode");
                }
                if (cond.doCustomer.AddressEN == null)
                {
                    messageParam.Add("AddressEN");
                }
                //if (cond.doCustomer.RoadEN == null) {
                //    messageParam.Add("RoadEN");
                //}
                if (cond.doCustomer.SubDistrictEN == null)
                {
                    messageParam.Add("SubDistrictEN");
                }
                //if (cond.doCustomer.AddressLC == null)
                //{
                //    messageParam.Add("AddressLC");
                //}
                //if (cond.doCustomer.RoadLC == null) {
                //    messageParam.Add("RoadLC");
                //}
                //if (cond.doCustomer.SubDistrictLC == null)
                //{
                //    messageParam.Add("SubDistrictLC");
                //}
                if (cond.doCustomer.DistrictCode == null)
                {
                    messageParam.Add("DistrictCode");
                }
                if (cond.doCustomer.ProvinceCode == null)
                {
                    messageParam.Add("ProvinceCode");
                }
                if (messageParam.Count > 0)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, messageParam.ToArray<string>());
                }

                if (CommonUtil.IsNullOrEmpty(cond.doCustomer.CustCode))
                {
                    cond.doCustomer.CustCode = this.GenerateCustomerCode();
                    if (cond.doCustomer.DummyIDFlag == true)
                        cond.doCustomer.IDNo = this.GenerateDummyID();
                    List<tbm_Customer> nLst = this.InsertCustomer(cond.doCustomer);
                    if (nLst.Count > 0)
                    {
                        cond.doCustomer.CustStatus = nLst[0].CustStatus;

                        ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                        List<doMiscTypeCode> mLst = hand.GetMiscTypeCodeList(new List<doMiscTypeCode>() 
                                        { 
                                            new doMiscTypeCode() 
                                            { 
                                                FieldName = MiscType.C_CUST_STATUS,
                                                ValueCode = cond.doCustomer.CustStatus
                                            }
                                        });
                        if (mLst.Count > 0)
                        {
                            cond.doCustomer.CustStatusName = mLst[0].ValueDisplay;
                        }
                    }

                    if (cond.doSite != null)
                        cond.doSite.SiteCode = null;
                }
                else
                {
                    List<doCustomer> custLst = this.GetCustomerByLanguage(cond.doCustomer.CustCode);
                    if (custLst.Count > 0)
                    {
                        bool isChanged = false;
                        PropertyInfo[] props = cond.doCustomer.GetType().GetProperties();
                        foreach (PropertyInfo prop in props)
                        {
                            if (prop.CanWrite == false
                                || prop.PropertyType != typeof(string))
                                continue;

                            string val1 = (string)prop.GetValue(cond.doCustomer, null);

                            PropertyInfo cprop = custLst[0].GetType().GetProperty(prop.Name);
                            if (cprop != null)
                            {
                                string val2 = (string)cprop.GetValue(custLst[0], null);
                                if (val1 != val2)
                                {
                                    isChanged = true;
                                    break;
                                }
                            }
                        }
                        if (isChanged)
                        {
                            if (cond.doCustomer.IDNo != custLst[0].IDNo)
                            {
                                // Akat K. : modify follow DDS
                                //if (cond.doCustomer.DummyIDFlag == true
                                //    && custLst[0].DummyIDFlag == false)
                                if (custLst[0].DummyIDFlag == true
                                    && cond.doCustomer.DummyIDFlag == false)
                                {
                                    this.UpdateCustomer(cond.doCustomer);
                                }
                                else
                                {
                                    cond.doCustomer.CustCode = this.GenerateCustomerCode();
                                    List<tbm_Customer> nLst = this.InsertCustomer(cond.doCustomer);
                                    if (nLst.Count > 0)
                                    {
                                        cond.doCustomer.CustStatus = nLst[0].CustStatus;

                                        ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                                        List<doMiscTypeCode> mLst = hand.GetMiscTypeCodeList(new List<doMiscTypeCode>() 
                                        { 
                                            new doMiscTypeCode() 
                                            { 
                                                FieldName = MiscType.C_CUST_STATUS,
                                                ValueCode = cond.doCustomer.CustStatus
                                            }
                                        });
                                        if (mLst.Count > 0)
                                        {
                                            cond.doCustomer.CustStatusName = mLst[0].ValueDisplay;
                                        }
                                    }

                                    if (cond.doSite != null)
                                        cond.doSite.SiteCode = null;
                                }
                            }
                            else
                            {
                                List<int?> lst = this.IsUsedCustomer(cond.doCustomer.CustCode);
                                if (lst.Count > 0)
                                {
                                    if (lst[0] > 0)
                                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1041);
                                }

                                this.UpdateCustomer(cond.doCustomer);
                            }
                        }
                    }
                }
                if (cond.dtCustomerGroup != null)
                {
                    if (cond.dtCustomerGroup.Count > 0)
                    {
                        List<dtCustomeGroupData> lst = CommonUtil.ClonsObjectList<dtCustomerGroup, dtCustomeGroupData>(cond.dtCustomerGroup);
                        foreach (dtCustomeGroupData group in lst)
                        {
                            group.CustCode = cond.doCustomer.CustCode;
                        }

                        this.DeleteCustomerGroup(lst);
                        this.InsertCustomerGroup(lst);
                    }
                }
                // Akat K. : if input none of CustomerGroup > clear all remain CustomerGroup
                if (cond.dtCustomerGroup == null || cond.dtCustomerGroup.Count == 0)
                {
                    base.ClearAllCustomerGroup(cond.doCustomer.CustCode);
                }

                if (cond.doSite != null)
                {
                    cond.doSite.CustCode = cond.doCustomer.CustCode;

                    ISiteMasterHandler iHand = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;

                    if (CommonUtil.IsNullOrEmpty(cond.doSite.SiteCode))
                    {
                        if (GenerateSiteCode(cond.doCustomer.CustCode, cond.doSite))
                        {
                            iHand.InsertSite(cond.doSite);
                        }
                    }
                    //else
                    //    iHand.UpdateSite(cond.doSite);
                }

                return cond;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Getting customer data
        /// </summary>
        /// <param name="pchrCustCode"></param>
        /// <returns></returns>
        public List<doCustomer> GetCustomer(string pchrCustCode)
        {
            try
            {
                List<doCustomer> lst = this.GetCustomer(pchrCustCode,
                                            SECOM_AJIS.Common.Util.ConstantValue.MiscType.C_CUST_STATUS,
                                            SECOM_AJIS.Common.Util.ConstantValue.MiscType.C_CUST_TYPE,
                                            SECOM_AJIS.Common.Util.ConstantValue.MiscType.C_FINANCIAL_MARKET_TYPE);

                if (lst == null)
                    lst = new List<doCustomer>();
                else
                    CommonUtil.MappingObjectLanguage<doCustomer>(lst);

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Getting customer data
        /// </summary>
        /// <param name="pchrCustCode"></param>
        /// <returns></returns>
        public List<doCustomer> GetCustomerAll(string pchrCustCode)
        {
            try
            {
                List<doCustomer> lst = this.GetCustomerAll(pchrCustCode,
                                            SECOM_AJIS.Common.Util.ConstantValue.MiscType.C_CUST_STATUS,
                                            SECOM_AJIS.Common.Util.ConstantValue.MiscType.C_CUST_TYPE,
                                            SECOM_AJIS.Common.Util.ConstantValue.MiscType.C_FINANCIAL_MARKET_TYPE);

                if (lst == null)
                    lst = new List<doCustomer>();
                else
                    CommonUtil.MappingObjectLanguage<doCustomer>(lst);

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Getting customer data by language
        /// </summary>
        /// <param name="pchrCustCode"></param>
        /// <returns></returns>
        public List<doCustomer> GetCustomerByLanguage(string pchrCustCode)
        {
            try
            {
                return CommonUtil.ConvertObjectbyLanguage<doCustomer, doCustomer>(
                                    this.GetCustomer(pchrCustCode),
                                    "CustStatusName",
                                    "CustTypeName",
                                    "CompanyTypeName",
                                    "FinancialMaketTypeName",
                                    "BusinessTypeName",
                                    "Nationality");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        /// <summary>
        /// Getting customer group data
        /// </summary>
        /// <param name="pchrCustCode"></param>
        /// <returns></returns>
        public List<doCustomerWithGroup> GetCustomerWithGroup(string pchrCustCode)
        {
            try
            {
                List<doCustomerWithGroup> custLst = new List<doCustomerWithGroup>();

                List<doCustomer> lst = this.GetCustomerByLanguage(pchrCustCode);
                if (lst.Count > 0)
                {
                    foreach (doCustomer c in lst)
                    {
                        doCustomerWithGroup cust = CommonUtil.CloneObject<doCustomer, doCustomerWithGroup>(c);
                        cust.CustomerGroupData = this.GetCustomeGroupData(pchrCustCode, null);
                        if (cust.CustomerGroupData.Count == 1)
                        {
                            if (CommonUtil.IsNullOrEmpty(cust.CustomerGroupData[0].GroupCode))
                                cust.CustomerGroupData = new List<dtCustomeGroupData>();
                        }

                        custLst.Add(cust);
                    }
                }

                return custLst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        /// <summary>
        /// Check exist customer code
        /// </summary>
        /// <param name="pchrCustCode"></param>
        /// <returns></returns>
        public bool CheckExistCustomerData(string pchrCustCode)
        {
            try
            {
                List<int?> lst = this.CheckExistCustomer(pchrCustCode);
                if (lst != null)
                {
                    if (lst.Count > 0)
                        return lst[0] == 1;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        /// <summary>
        /// Check active customer code
        /// </summary>
        /// <param name="pchrCustCode"></param>
        /// <returns></returns>
        public bool CheckActiveCustomerData(string pchrCustCode)
        {
            try
            {
                List<int?> lst = this.CheckActiveCustomer(pchrCustCode);
                if (lst != null)
                {
                    if (lst.Count > 0)
                        return lst[0] == 1;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        /// <summary>
        /// Validate customer data
        /// </summary>
        /// <param name="cust"></param>
        /// <param name="isFullValidate"></param>
        public void ValidateCustomerData(doCustomer cust, bool isFullValidate = false)
        {
            IMasterHandler mhandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
            ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            doCompanyType cType = null;
            cust.ValidateCustomerData = true;
            try
            {
                #region Set Misc Data

                MiscTypeMappingList miscList = new MiscTypeMappingList();
                miscList.AddMiscType(cust);
                chandler.MiscTypeMappingList(miscList);

                #endregion
                #region Company Type

                List<doCompanyType> clst = mhandler.GetCompanyType(cust.CompanyTypeCode);
                if (clst.Count > 0)
                {
                    cType = clst[0];
                    cust.CompanyTypeName = clst[0].CompanyTypeName;
                }

                #endregion
                #region Nationality Data

                if (CommonUtil.IsNullOrEmpty(cust.RegionCode) == false)
                {
                    List<tbm_Region> nlst = mhandler.GetTbm_Region();
                    if (nlst.Count > 0)
                    {
                        foreach (tbm_Region r in nlst)
                        {
                            if (cust.RegionCode == r.RegionCode)
                            {
                                cust.Nationality = r.Nationality;
                                break;
                            }
                        }
                    }
                }

                #endregion
                #region BusinessType Data

                if (CommonUtil.IsNullOrEmpty(cust.BusinessTypeCode) == false)
                {
                    List<tbm_BusinessType> blst = mhandler.GetTbm_BusinessType();
                    if (blst.Count > 0)
                    {
                        foreach (tbm_BusinessType b in blst)
                        {
                            if (cust.BusinessTypeCode == b.BusinessTypeCode)
                            {
                                cust.BusinessTypeName = b.BusinessTypeName;
                                break;
                            }
                        }
                    }
                }

                #endregion
                #region Province Data

                if (CommonUtil.IsNullOrEmpty(cust.ProvinceCode) == false)
                {
                    List<tbm_Province> plst = mhandler.GetTbm_Province();
                    if (plst.Count > 0)
                    {
                        foreach (tbm_Province pv in plst)
                        {
                            if (cust.ProvinceCode == pv.ProvinceCode)
                            {
                                cust.ProvinceNameEN = pv.ProvinceNameEN;
                                cust.ProvinceNameLC = pv.ProvinceNameLC;
                                break;
                            }
                        }
                    }
                }

                #endregion
                #region District

                if (CommonUtil.IsNullOrEmpty(cust.DistrictCode) == false)
                {
                    List<tbm_District> dlst = mhandler.GetTbm_District(cust.ProvinceCode);
                    if (dlst.Count > 0)
                    {
                        foreach (tbm_District d in dlst)
                        {
                            if (cust.ProvinceCode == d.ProvinceCode
                                && cust.DistrictCode == d.DistrictCode)
                            {
                                cust.DistrictNameEN = d.DistrictNameEN;
                                cust.DistrictNameLC = d.DistrictNameLC;
                                break;
                            }
                        }
                    }
                }

                #endregion

                if (CommonUtil.IsNullOrEmpty(cust.CustTypeName) || cust.CustTypeCode != CustomerType.C_CUST_TYPE_JURISTIC)
                    cust.CompanyTypeName = null;
                if (CommonUtil.IsNullOrEmpty(cust.CompanyTypeName) || cust.CompanyTypeCode != CompanyType.C_COMPANY_TYPE_PUBLIC_CO_LTD)
                    cust.FinancialMaketTypeName = null;

                if (CommonUtil.IsNullOrEmpty(cust.CustCode))
                {
                    if (isFullValidate)
                        ApplicationErrorException.CheckMandatoryField<doCustomer, ValidateCustomer_Full>(cust);
                    else if (CommonUtil.IsNullOrEmpty(cust.CustCode) == true)
                        ApplicationErrorException.CheckMandatoryField<doCustomer, ValidateCustomer_CodeNull>(cust);
                    else
                        ApplicationErrorException.CheckMandatoryField<doCustomer, ValidateCustomer>(cust);
                }
            }
            catch
            {
                cust.ValidateCustomerData = false;
            }
            try
            {
                if (cType == null)
                    cType = new doCompanyType();

                cust.CustFullNameEN =
                        CommonUtil.TextList(new string[] {  cType.CustNamePrefixEN, 
                                                            cust.CustNameEN, 
                                                            cType.CustNameSuffixEN }, " ");
                cust.CustFullNameLC =
                    CommonUtil.TextList(new string[] {  cType.CustNamePrefixLC, 
                                                            cust.CustNameLC, 
                                                            cType.CustNameSuffixLC }, " ");

                mhandler.CreateAddressFull(cust);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Getting customer list data
        /// </summary>
        /// <param name="lst"></param>
        /// <returns></returns>
        public List<doCustomerList> GetCustomerList(List<tbm_Customer> lst)
        {
            try
            {
                return this.GetCustomerList(SECOM_AJIS.Common.Util.CommonUtil.ConvertToXml_Store<tbm_Customer>(lst, "CustCode"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
