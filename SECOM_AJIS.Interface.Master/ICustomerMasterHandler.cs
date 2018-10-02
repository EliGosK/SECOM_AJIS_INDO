using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.DataEntity.Common;

namespace SECOM_AJIS.DataEntity.Master
{
    public interface ICustomerMasterHandler
    {
        bool CheckDuplicateCustomer(doCustomer customer);
        bool CheckDuplicateCustomer_CustNameLC(doCustomer customer);
        bool CheckDuplicateCustomer_IDNo(doCustomer customer);
        void ManageCustomerInformation(string strCustCode);
        doCustomerTarget ManageCustomerTarget(doCustomerTarget cond);

        bool CheckExistCustomerData(string pchrCustCode);
        List<tbm_CustomerGroup> DeleteCustomerGroup(string xml_dtCustomerGroup);
        int InsertCustomerGroup(List<dtCustomeGroupData> groupList);
        List<tbm_Customer> UpdateCustomer(string custCode, string custStatus, Nullable<bool> importantFlag, string custNameEN, string custNameLC, string custFullNameEN, string custFullNameLC, string repPersonName, string contactPersonName, string sECOMContactPerson, string custTypeCode, string companyTypeCode, string financialMarketTypeCode, string businessTypeCode, string phoneNo, string faxNo, string iDNo, Nullable<bool> dummyIDFlag, string regionCode, string uRL, string memo, string alleyEN, string addressEN, string roadEN, string subDistrictEN, string addressFullEN, string addressLC, string alleyLC, string roadLC, string subDistrictLC, string addressFullLC, string districtCode, string provinceCode, string zipCode, Nullable<bool> deleteFlag, Nullable<System.DateTime> updateDate, string updateBy);
        List<tbm_Customer> CheckCustomer(string pchnvCustCode);
        List<tbm_Customer> InsertCustomer(string custCode, string custStatus, Nullable<bool> importantFlag, string custNameEN, string custNameLC, string custFullNameEN, string custFullNameLC, string repPersonName, string contactPersonName, string sECOMContactPerson, string custTypeCode, string companyTypeCode, string financialMarketTypeCode, string businessTypeCode, string phoneNo, string faxNo, string iDNo, Nullable<bool> dummyIDFlag, string regionCode, string uRL, string memo, string alleyEN, string addressEN, string roadEN, string subDistrictEN, string addressFullEN, string addressLC, string alleyLC, string roadLC, string subDistrictLC, string addressFullLC, string districtCode, string provinceCode, string zipCode, Nullable<bool> deleteFlag, Nullable<System.DateTime> createDate, string createBy, Nullable<System.DateTime> updateDate, string updateBy);
        int UpdateCustomerStatus(string c_CUST_STATUS_EXISTING_CUSTOMER, string c_CUST_STATUS_NEW_CUSTOMER, string pchnvstrCustCode, string updateBy, Nullable<System.DateTime> updateDate);

        List<tbm_Customer> UpdateCustomer(doCustomer customer);// with insert log
        List<tbm_Customer> InsertCustomer(doCustomer customer);// with insert log
        
        List<dtCustomerData> GetCustomerDataForSearch(doCustomerSearchCondition cond);
        List<dtCustomeGroupData> GetCustomeGroupData(string pchrCustomerCode ,string chvnGroupName);
        List<Nullable<int>> CheckExistCustomerGroup(string pchnvGroupCode, string pchnvCustCode);
        List<dtCustomerForView> GetCustomerForView(string chrCustCode, string pC_CUST_STATUS, string pC_CUST_TYPE, string pC_FINANCIAL_MARKET_TYPE);

        List<doCustomer> GetCustomer(string pchrCustCode);
        List<doCustomer> GetCustomerAll(string pchrCustCode);
        List<doCustomer> GetCustomerByLanguage(string pchrCustCode);
        List<dtCustomerGroupForView> GetCustomerGroup(string pchrGroupCode, string pchrCustCode);
        void ValidateCustomerData(doCustomer cust, bool isFullValidate = false);

        List<doCustomerList> GetCustomerList(List<tbm_Customer> lst);
        List<doCompanyType> GetCompanyType(string pchrstrCompanyTypeCode);

        List<doCustomerWithGroup> GetCustomerWithGroup(string pchrCustCode);

        bool CheckActiveCustomerData(string pchrCustCode);
    }
}
