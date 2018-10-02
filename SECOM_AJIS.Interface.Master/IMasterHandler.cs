using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.DataEntity.Common;

namespace SECOM_AJIS.DataEntity.Master
{
    public interface IMasterHandler
    {
        List<tbm_Group> GetTbm_Group(string pGroupCode);

        /// <summary>
        /// Get all group and transform to view mode.
        /// </summary>
        /// <returns></returns>
        List<View_tbm_Group> GetDorp_Group();

        List<tbm_Department> GetTbm_Department();

        /// <summary>
        /// Get building usage and mapping language.
        /// </summary>
        /// <returns></returns>
        List<tbm_BuildingUsage> GetTbm_BiuldingUsage();

        /// <summary>
        /// Get business type and mapping language.
        /// </summary>
        /// <returns></returns>
        List<tbm_BusinessType> GetTbm_BusinessType();

        List<tbm_CompanyType> GetTbm_CompanyType();

        /// <summary>
        /// Get region and mapping language.
        /// </summary>
        /// <returns></returns>
        List<tbm_Region> GetTbm_Region();

        /// <summary>
        /// Get province and mapping language.
        /// </summary>
        /// <returns></returns>
        List<tbm_Province> GetTbm_Province();

        /// <summary>
        /// Get district and mapping language.
        /// </summary>
        /// <param name="provinceCode"></param>
        /// <returns></returns>
        List<tbm_District> GetTbm_District(string provinceCode);

        List<tbm_Customer> GetTbm_Customer(string custCode);
        List<doGetTbm_Site> GetTbm_Site(string siteCode);
        List<tbs_MiscellaneousTypeCode> GetTbs_MiscellaneousTypeCode(string c_CUST_TYPE);
        List<dtBillingClientDataForSearch> GetBillingClientDataForSearch(doBillingClientSearchCondition cond);

        /// <summary>
        /// Get company type and mapping language.
        /// </summary>
        /// <param name="pchrstrCompanyTypeCode"></param>
        /// <returns></returns>
        List<doCompanyType> GetCompanyType(string pchrstrCompanyTypeCode);

        List<tbm_Product> GetTbm_Product(string pchrProductCode, string pcharProductTypeCode);

        /// <summary>
        /// Get object(screen) and mapping language.
        /// </summary>
        /// <returns></returns>
        List<tbm_Object> GetTbm_Object();

        List<tbm_Module> GetTbm_Module();
        List<tbm_Employee> GetActiveEmployee(string strEmpNo);

        /// <summary>
        /// Combind alley, road, sub district, district and province to full address for both English and local.
        /// </summary>
        /// <param name="cust"></param>
        /// <returns></returns>
        void CreateAddressFull(doCustomer cust);

       // List<tbm_BillingType> GetTbm_BillingType();

        /// <summary>
        /// Get building usege from given strSiteBuildingUsageCode.
        /// </summary>
        /// <param name="strSiteBuildingUsageCode"></param>
        /// <returns></returns>
        List<tbm_BuildingUsage> GetSiteBuildingUsage(string strSiteBuildingUsageCode);

        List<dtDocumentTemplateByDocumentCode> GetDocumentTemplateByDocumentCode(string pDocumentCode);
        List<tbm_Calendar> GetTbm_Calendar(Nullable<System.DateTime> onDate);
        List<tbm_Bank> GetTbm_Bank();

        /// <summary>
        /// Get bank branch data and mapping language.
        /// </summary>
        /// <param name="strBankCode"></param>
        /// <returns></returns>
        List<tbm_BankBranch> GetTbm_BankBranch(string strBankCode);

        /// <summary>
        /// Get secom bank account and mapping language.
        /// </summary>
        /// <param name="secomAccountID"></param>
        /// <returns></returns>
        List<tbm_SecomBankAccount> GetTbm_SecomBankAccount(int? secomAccountID);

        /// <summary>
        /// Get credit card company and mapping language.
        /// </summary>
        /// <returns></returns>
        List<tbm_CreditCardCompany> GetTbm_CreditCardCompany();

        List<tbm_CustomerGroup> DeleteTbm_CustomerGroup(string pGroupCode, string pCustCode);

        /// <summary>
        /// Get auto transfer schedule list and mapping language.
        /// </summary>
        /// <param name="BankCode"></param>
        /// <param name="AutoTransferDateNumber"></param>
        /// <returns></returns>
        List<tbm_AutoTransferScheduleList> GetTbm_AutoTransferScheduleList(string BankCode, string AutoTransferDateNumber);
        
        List<dtBankBranch> GetBankBranch();
        List<tbm_InstrumentExpansion> GetTbm_InstrumentExpansion(string pInstrumentCode, string pChildInstrumentCode);


        #region Master II
        /// <summary>
        /// Check duplicate area and instrument on normal shelf (sp_MA_CheckShelfDuplicateArea
        /// </summary>
        /// <param name="strInstrumentCode"></param>
        /// <param name="strAreaCode"></param>
        /// <returns></returns>
        bool CheckShelfDuplicateArea(string strInstrumentCode, string strAreaCode);

        /// <summary>
        /// Get Normal shelf no. that has duplicate area and instrument
        /// </summary>
        /// <param name="strInstrumentCode"></param>
        /// <param name="strAreaCode"></param>
        /// <returns></returns>
        string GetShelfNoDuplicateArea(string strInstrumentCode, string strAreaCode); //Add by Jutarat A. on 09042013

        /// <summary>
        /// Get secom bank account for auto transfer (sp_MA_GetSecomBankAccountForAutoTransfer)
        /// </summary>
        /// <param name="strBankCode"></param>
        /// <returns></returns>
        List<tbm_SecomBankAccount> GetSecomBankAccountForAutoTransfer(string strBankCode);
        #endregion

        /// <summary>
        /// Get SECOM bank account.
        /// </summary>
        /// <returns></returns>
        List<doSECOMAccount> GetSECOMAccount();

        /// <summary>
        /// Get SECOM bank account for auto transfer payment.
        /// </summary>
        /// <returns></returns>
        List<doSECOMAccount> GetSECOMAccountAutoTransfer();
        /// <summary>
        /// Get SECOM bank account for bank transfer payment.
        /// </summary>
        /// <returns></returns>
        List<doSECOMAccount> GetSECOMAccountBankTransfer();
        /// <summary>
        /// Get SECOM bank account for dummy payment. 
        /// </summary>
        /// <returns></returns>
        List<doSECOMAccount> GetSECOMAccountDummyTransfer();
    }
}
