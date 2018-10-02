using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Common;

using SECOM_AJIS.Common.Util.ConstantValue;
using System.Reflection;
using CSI.WindsorHelper;
using System.Diagnostics;
using SECOM_AJIS.Common.Models;
using System.Globalization;

namespace SECOM_AJIS.DataEntity.Master
{
    public class MasterHandler : BizMADataEntities, IMasterHandler
    {
        #region Override Methods

        public override List<tbm_Department> GetTbm_Department()
        {
            try
            {
                return base.GetTbm_Department();
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
            
        }
        public override List<tbm_BuildingUsage> GetTbm_BiuldingUsage()
        {
            try
            {
                List<tbm_BuildingUsage> lst = base.GetTbm_BiuldingUsage();
                if (lst.Count > 0)
                    CommonUtil.MappingObjectLanguage<tbm_BuildingUsage>(lst);
                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public override List<tbm_BusinessType> GetTbm_BusinessType()
        {
            try
            {
                List<tbm_BusinessType> lst = base.GetTbm_BusinessType();
                if (lst.Count > 0)
                    CommonUtil.MappingObjectLanguage<tbm_BusinessType>(lst);
                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public override List<tbm_CompanyType> GetTbm_CompanyType()
        {
            try
            {
                return base.GetTbm_CompanyType();
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }
        public override List<tbm_Region> GetTbm_Region()
        {
            try
            {
                List<tbm_Region> lst = base.GetTbm_Region();
                if (lst.Count > 0)
                    CommonUtil.MappingObjectLanguage<tbm_Region>(lst);
                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public override List<tbm_Customer> GetTbm_Customer(string custCode)
        {
            try
            {
                return base.GetTbm_Customer(custCode);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public override List<doGetTbm_Site> GetTbm_Site(string siteCode)
        {
            try
            {
                return base.GetTbm_Site(siteCode);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public override List<doCompanyType> GetCompanyType(string pchrstrCompanyTypeCode)
        {
            try
            {
                List<doCompanyType> lst = base.GetCompanyType(pchrstrCompanyTypeCode);
                if (lst.Count > 0)
                    CommonUtil.MappingObjectLanguage<doCompanyType>(lst);
                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public override List<tbm_Province> GetTbm_Province()
        {
            try
            {
                List<tbm_Province> lst = base.GetTbm_Province();
                if (lst.Count > 0)
                    CommonUtil.MappingObjectLanguage<tbm_Province>(lst);
                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public override List<tbm_District> GetTbm_District(string provinceCode)
        {
            try
            {
                List<tbm_District> lst = base.GetTbm_District(provinceCode);
                if (lst.Count > 0)
                    CommonUtil.MappingObjectLanguage<tbm_District>(lst);
                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public override List<tbm_Object> GetTbm_Object() {
            try
            {
                List<tbm_Object> lst = base.GetTbm_Object();
                if (lst.Count > 0)
                    CommonUtil.MappingObjectLanguage<tbm_Object>(lst);
                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public override List<tbm_Bank> GetTbm_Bank()
        {
            try
            {
                List<tbm_Bank> lst = base.GetTbm_Bank();
                //CommonUtil.MappingObjectLanguage<tbm_Bank>(lst);
                return lst;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public override List<tbm_BankBranch> GetTbm_BankBranch(string strBankCode)
        {
            try
            {
                List<tbm_BankBranch> lst = base.GetTbm_BankBranch(strBankCode);                
                CommonUtil.MappingObjectLanguage<tbm_BankBranch>(lst);
                return lst;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public List<tbm_SecomBankAccount> GetTbm_SecomBankAccount(int? secomeAccountID)
        {
            List<tbm_SecomBankAccount> lst = base.GetTbm_SecomBankAccount(secomeAccountID);
            CommonUtil.MappingObjectLanguage<tbm_SecomBankAccount>(lst);
            return lst;
        }

        public override List<tbm_CreditCardCompany> GetTbm_CreditCardCompany()
        {
            try
            {
                List<tbm_CreditCardCompany> lst =  base.GetTbm_CreditCardCompany();               
                CommonUtil.MappingObjectLanguage<tbm_CreditCardCompany>(lst);
                return lst;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public override List<tbm_AutoTransferScheduleList> GetTbm_AutoTransferScheduleList(string BankCode, string AutoTransferDateNumber)
        {
            try
            {
                List<tbm_AutoTransferScheduleList> lst = base.GetTbm_AutoTransferScheduleList(BankCode, AutoTransferDateNumber);
                CommonUtil.MappingObjectLanguage<tbm_AutoTransferScheduleList>(lst);
                return lst;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        

        #endregion
        #region Methods

        public List<tbm_BuildingUsage> GetSiteBuildingUsage(string strSiteBuildingUsageCode)
        {
            var res = from a in this.GetTbm_BiuldingUsage() where a.BuildingUsageCode == strSiteBuildingUsageCode select a;

            return res.ToList();
        }

        public List<dtBillingClientDataForSearch> GetBillingClientDataForSearch(doBillingClientSearchCondition cond)
        {
            try
            {
                return base.GetBillingClientDataForSearch(MiscType.C_CUST_TYPE,
                                                           cond.BillingClientCode,
                                                           cond.BillingClientName,
                                                           cond.CompanyTypeCode,
                                                           cond.RegionCode,
                                                           cond.BusinessTypeCode,
                                                           cond.Address,
                                                           cond.TelephoneNo,
                                                           cond.CustomerTypeCode); // xmlCustomerTypeCode

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<tbm_Employee> GetActiveEmployee(string strEmpNo)
        {
            try
            {
                return base.GetActiveEmployee(strEmpNo);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public List<View_tbm_Group> GetDorp_Group()
        {
            try
            {
                List<tbm_Group> result = base.GetTbm_Group(null);
                List<View_tbm_Group> lst = new List<View_tbm_Group>();
                View_tbm_Group group = new View_tbm_Group();


                for (int i = 0; i < result.Count; i++)
                {
                    group = CommonUtil.CloneObject<tbm_Group, View_tbm_Group>(result[i]);
                    lst.Add(group);

                    if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                    {
                        lst[i].GroupCodeName = CommonUtil.TextCodeName(lst[i].GroupCode, lst[i].GroupNameEN);
                    }
                    else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                    {
                        lst[i].GroupCodeName = CommonUtil.TextCodeName(lst[i].GroupCode, lst[i].GroupNameEN);
                    }
                    else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_LC)
                    {
                        lst[i].GroupCodeName = CommonUtil.TextCodeName(lst[i].GroupCode, lst[i].GroupNameLC);
                    }
                }

                //List<View_tbm_Group> lst = CommonUtil.CloneObject<tbm_Group, View_tbm_Group>(result);

               
                return lst;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        
        public void CreateAddressFull(doCustomer cust)
        {
            try
            {
                if (cust == null)
                    return;

                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                doCustomerAddress custAddress = new doCustomerAddress();
                MiscTypeMappingList miscLst = new MiscTypeMappingList();
                miscLst.AddMiscType(custAddress);
                chandler.MiscTypeMappingList(miscLst);

                #region EN


                string[] addrEN = new string[]
                {
                    cust.AddressEN,
                    CommonUtil.TextWithPrefixSuffix(cust.AlleyEN, custAddress.AlleyPrefixEN, custAddress.AlleySuffixEN),
                    CommonUtil.TextWithPrefixSuffix(cust.RoadEN, custAddress.RoadPrefixEN, custAddress.RoadSuffixEN),
                    CommonUtil.TextWithPrefixSuffix(cust.SubDistrictEN, custAddress.SubDistrictPrefixEN, custAddress.SubDistrictSuffixEN),
                    CommonUtil.TextWithPrefixSuffix(cust.DistrictNameEN, custAddress.DistrictPrefixEN, custAddress.DistrictSuffixEN),
                    CommonUtil.TextWithPrefixSuffix(cust.ProvinceNameEN, custAddress.ProvincePrefixEN, custAddress.ProvinceSuffixEN),
                    cust.ZipCode
                };
                cust.AddressFullEN = CommonUtil.TextList(addrEN, " ");

                #endregion
                #region LC

                string[] addrLC = new string[]
                {
                    cust.AddressLC,
                    CommonUtil.TextWithPrefixSuffix(cust.AlleyLC, custAddress.AlleyPrefixLC, custAddress.AlleySuffixLC),
                    CommonUtil.TextWithPrefixSuffix(cust.RoadLC, custAddress.RoadPrefixLC, custAddress.RoadSuffixLC),
                    CommonUtil.TextWithPrefixSuffix(cust.SubDistrictLC, custAddress.SubDistrictPrefixLC, custAddress.SubDistrictSuffixLC),
                    CommonUtil.TextWithPrefixSuffix(cust.DistrictNameLC, custAddress.DistrictPrefixLC, custAddress.DistrictSuffixLC),
                    CommonUtil.TextWithPrefixSuffix(cust.ProvinceNameLC, custAddress.ProvincePrefixLC, custAddress.ProvinceSuffixLC),
                    cust.ZipCode
                };
                cust.AddressFullLC = CommonUtil.TextList(addrLC, " ");

                #endregion
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool CheckShelfDuplicateArea(string strInstrumentCode, string strAreaCode)
        {
            try
            {
                int chkShelf = Convert.ToInt32(base.CheckShelfDuplicateArea(strInstrumentCode, strAreaCode, ShelfType.C_INV_SHELF_TYPE_NORMAL, FlagType.C_FLAG_ON)[0]);
                return chkShelf == 1;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        //Add by Jutarat A. on 09042013
        /// <summary>
        /// Get Normal shelf no. that has duplicate area and instrument
        /// </summary>
        /// <param name="strInstrumentCode"></param>
        /// <param name="strAreaCode"></param>
        /// <returns></returns>
        public string GetShelfNoDuplicateArea(string strInstrumentCode, string strAreaCode)
        {
            string strDuplicateShelfNo = string.Empty;

            try
            {
                List<string> duplicateShelfNoList = base.GetShelfNoDuplicateArea(strInstrumentCode, strAreaCode, ShelfType.C_INV_SHELF_TYPE_NORMAL, FlagType.C_FLAG_ON);
                if (duplicateShelfNoList != null && duplicateShelfNoList.Count > 0)
                    strDuplicateShelfNo = duplicateShelfNoList[0];

                return strDuplicateShelfNo;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        //End Add

        //public List<tbm_BillingType> GetTbm_BillingType()
        //{
        //    try
        //    {
        //        List<tbm_BillingType> tbm_BillingTypeList = base.GetTbm_BillingType();
        //        if (tbm_BillingTypeList == null)
        //        {
        //            tbm_BillingTypeList = new List<tbm_BillingType>();
        //        }
        //        else
        //        {
        //            CommonUtil.MappingObjectLanguage<tbm_BillingType>(tbm_BillingTypeList);
        //        }

        //        return tbm_BillingTypeList;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        public List<tbm_SecomBankAccount> GetSecomBankAccountForAutoTransfer(string strBankCode)
        {
            try
            {
                List<tbm_SecomBankAccount> lst = base.GetSecomBankAccountForAutoTransfer(strBankCode);

                if (lst != null)
                {
                    return lst;
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


        public List<doSECOMAccount> GetSECOMAccount()
        {
            List<doSECOMAccount> lst = base.GetSECOMAccount();
            if (lst != null)
            {
                //Language Mapping
                CommonUtil.MappingObjectLanguage<doSECOMAccount>(lst);

                //Sorting by current language
                CultureInfo culture = null;
                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_EN);
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_EN);  // JP -> EN, There are 2 languages for doSECOMAccount data object.
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_LC)
                {
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_LC);
                }
                lst = lst.OrderBy(p => p.Text, StringComparer.Create(culture, false)).ToList();
            }
            return lst;
        }

        public List<doSECOMAccount> GetSECOMAccountAutoTransfer()
        {
            List<doSECOMAccount> lst = base.GetSECOMAccountAutoTransfer();
            if (lst != null)
            {
                //Language Mapping
                CommonUtil.MappingObjectLanguage<doSECOMAccount>(lst);

                //Sorting by current language
                CultureInfo culture = null;
                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_EN);
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_EN);  // JP -> EN, There are 2 languages for doSECOMAccount data object.
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_LC)
                {
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_LC);
                }
                lst = lst.OrderBy(p => p.Text, StringComparer.Create(culture, false)).ToList();
            }
            return lst;
        }
        
        public List<doSECOMAccount> GetSECOMAccountBankTransfer()
        {
            List<doSECOMAccount> lst = base.GetSECOMAccountBankTransfer();
            if (lst != null)
            {
                //Language Mapping
                CommonUtil.MappingObjectLanguage<doSECOMAccount>(lst);

                //Sorting by current language
                CultureInfo culture = null;
                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_EN);
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_EN);  // JP -> EN, There are 2 languages for doSECOMAccount data object.
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_LC)
                {
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_LC);
                }
                lst = lst.OrderBy(p => p.Text, StringComparer.Create(culture, false)).ToList();
            }
            return lst;
        }

        public List<doSECOMAccount> GetSECOMAccountDummyTransfer()
        {
            List<doSECOMAccount> lst = base.GetSECOMAccountDummyTransfer();
            if (lst != null)
            {
                //Language Mapping
                CommonUtil.MappingObjectLanguage<doSECOMAccount>(lst);

                //Sorting by current language
                CultureInfo culture = null;
                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_EN);
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_EN);  // JP -> EN, There are 2 languages for doSECOMAccount data object.
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_LC)
                {
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_LC);
                }
                lst = lst.OrderBy(p => p.Text, StringComparer.Create(culture, false)).ToList();
            }
            return lst;
        }
        #endregion
    }
    
}
