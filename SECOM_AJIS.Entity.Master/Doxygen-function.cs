namespace SECOM_AJIS.DataEntity.Master
{
	partial class BizMADataEntities
	{
		/// \fn public virtual List<Nullable<int>> CheckActiveCustomer(string pcharCustCode)
		/// \brief (Call stored procedure: sp_MA_CheckActiveCustomer).

		/// \fn public virtual List<tbm_Customer> CheckCustomer(string pchnvCustCode)
		/// \brief (Call stored procedure: sp_MA_Customer).

		/// \fn public virtual List<Nullable<int>> CheckDuplicateCustomer(string c_CUST_STATUS_EXISTING_CUSTOMER, string pchrCustCode, string pchrIDNo, string pchrCustNameLC)
		/// \brief (Call stored procedure: sp_MA_CheckDuplicateCustomer).

		/// \fn public virtual List<Nullable<int>> CheckDuplicateCustomer_CustNameLC(string c_CUST_STATUS_EXISTING_CUSTOMER, string pchrCustCode, string pchrCustNameLC)
		/// \brief (Call stored procedure: sp_MA_CheckDuplicateCustomer_CustNameLC).

		/// \fn public virtual List<Nullable<int>> CheckDuplicateCustomer_IDNo(string c_CUST_STATUS_EXISTING_CUSTOMER, string pchrCustCode, string pchrIDNo)
		/// \brief (Call stored procedure: sp_MA_CheckDuplicateCustomer_IDNo).

		/// \fn public virtual List<Nullable<int>> CheckDuplicateGroup(string pGroupNameLC, string pGroupCode, Nullable<bool> pC_FLAG_OFF)
		/// \brief (Call stored procedure: sp_MA_CheckDuplicateGroup).

		/// \fn public virtual List<tbm_Shelf> CheckDuplicateShelf(string pShelfNo)
		/// \brief (Call stored procedure: sp_MA_CheckDuplicateShelf).

		/// \fn public virtual List<Nullable<int>> CheckDuplicateSite(string pSiteCode, string pCustCode, string pSiteNameLC, string pAddressFullLC)
		/// \brief (Call stored procedure: sp_MA_CheckDuplicateSite).

		/// \fn public virtual List<Nullable<int>> CheckExistActiveEmployee(string pchvEmpNo)
		/// \brief (Call stored procedure: sp_MA_CheckExistActiveEmployee).

		/// \fn public virtual List<Nullable<bool>> CheckExistBelonging(string pchvOfficeCode, string pchrDepartmentCode, string pchvEmpNo, Nullable<int> pintBelongingID)
		/// \brief (Call stored procedure: sp_MA_CheckExistBelonging).

		/// \fn public virtual List<Nullable<int>> CheckExistCustomer(string pchrCustCode)
		/// \brief (Call stored procedure: sp_MA_CheckExistCustomer).

		/// \fn public virtual List<Nullable<int>> CheckExistCustomerGroup(string pchnvGroupCode, string pchnvCustCode)
		/// \brief (Call stored procedure: sp_MA_CheckExistCustomerGroup).

		/// \fn public virtual List<Nullable<bool>> CheckExistEmployee(string pchvEmpNo)
		/// \brief (Call stored procedure: sp_MA_CheckExistEmployee).

		/// \fn public virtual List<Nullable<bool>> CheckExistEmpNo(string officeCode, string departmentCode, string positionCode, string empNo)
		/// \brief (Call stored procedure: sp_MA_CheckExistEmpNo).

		/// \fn public virtual List<Nullable<bool>> CheckExistInstrument(string pchvInstrumentCode)
		/// \brief (Call stored procedure: sp_MA_CheckExistInstrument).

		/// \fn public virtual List<tbm_InstrumentExpansion> CheckExistInstrumentExpansion(string pchvParentInstrumentCode, string pchvChildInstrumentCode)
		/// \brief (Call stored procedure: sp_MA_CheckExistInstrumentExpansion).

		/// \fn public virtual List<Nullable<bool>> CheckExistMainDepartmentFlag(string pchvEmpNo, Nullable<int> pintBelongingID)
		/// \brief (Call stored procedure: sp_MA_CheckExistMainDepartmentFlag).

		/// \fn public virtual List<Nullable<bool>> CheckExistMainDepartmentPersonInCharge(string officeCode, string departmentCode, Nullable<int> belongingID)
		/// \brief (Call stored procedure: sp_MA_CheckExistMainDepartmentPersonInCharge).

		/// \fn public virtual List<Nullable<bool>> CheckExistParentChild(string pchvInstrumentCode)
		/// \brief (Call stored procedure: sp_MA_CheckExistParentChild).

		/// \fn public virtual List<Nullable<int>> CheckExistParentInstrument(string pchvnInstrumentCode, string c_EXPANSION_TYPE_PARENT)
		/// \brief (Call stored procedure: sp_MA_CheckExistParentInstrument).

		/// \fn public virtual List<Nullable<bool>> CheckExistPermission(string officeCode, string departmentCode, string positionCode)
		/// \brief (Call stored procedure: sp_MA_CheckExistPermission).

		/// \fn public virtual List<Nullable<int>> CheckExistSite(string pchrSiteCode, string pchrCustCode)
		/// \brief (Call stored procedure: sp_MA_CheckExistSite).

		/// \fn public virtual List<CheckHeadOffice_Result> CheckHeadOffice(string strOfficeCode, string c_OFFICELEVEL_HEAD)
		/// \brief (Call stored procedure: sp_MA_CheckHeadOffice).

		/// \fn public virtual List<Nullable<int>> CheckInventoryHeadOffice(string officeCode, string c_FUNC_LOGISTIC_HQ)
		/// \brief (Call stored procedure: sp_MA_CheckInventoryHeadOffice).

		/// \fn public virtual List<Nullable<int>> CheckShelfDuplicateArea(string strInstrumentCode, string strAreaCode, string c_INV_SHELF_TYPE_NORMAL, Nullable<bool> c_FLAG_NO)
		/// \brief (Call stored procedure: sp_MA_CheckShelfDuplicateArea).

		/// \fn public virtual List<CheckUpdateDate> CheckUpdateDate(string siteCode)
		/// \brief (Call stored procedure: sp_MA_CheckUpdateDate).

		/// \fn public virtual List<tbm_CustomerGroup> ClearAllCustomerGroup(string custCode)
		/// \brief (Call stored procedure: sp_MA_ClearAllCustomerGroup).

		/// \fn public virtual List<tbm_PermissionDetail> CopyPermissionFromGroupToIndividual(string permissionGroupCode, string permissionIndividualCode, Nullable<System.DateTime> createDate, string createBy)
		/// \brief (Call stored procedure: sp_MA_CopyPermissionFromGroupToIndividual).

		/// \fn public virtual List<tbm_Belonging> DeleteAllBelonging(string pEmpNo)
		/// \brief (Call stored procedure: sp_MA_DeleteAllBelonging).

		/// \fn public virtual int DeleteAllInstrument(string instrumentCode)
		/// \brief (Call stored procedure: sp_MA_DeleteAllInstrumentExpansion).

		/// \fn public virtual List<tbm_Belonging> DeleteBelonging(Nullable<int> pintBelongingID)
		/// \brief (Call stored procedure: sp_MA_DeleteBelonging).

		/// \fn public virtual List<tbm_CustomerGroup> DeleteCustomerGroup(string xml_dtCustomerGroup)
		/// \brief (Call stored procedure: sp_MA_DeleteCustomerGroup).

		/// \fn public virtual List<tbm_Employee> DeleteEmployee(string pchvEmpNo, Nullable<bool> pbitDeleteFlag, string pchvUpdateBy, Nullable<System.DateTime> pdtmUpdateDate)
		/// \brief (Call stored procedure: sp_MA_DeleteEmployee).

		/// \fn public virtual List<tbm_InstrumentExpansion> DeleteInstrumentExpansion(string pchvnChildInstrumentCode, string pchvnInstrumentCode)
		/// \brief (Call stored procedure: sp_MA_DeleteInstrumentExpansion).

		/// \fn public virtual List<tbm_PermissionDetail> DeletePermissionDetail(string permissionGroupCode, string permissionIndividualCode)
		/// \brief (Call stored procedure: sp_MA_DeletePermissionDetail).

		/// \fn public virtual List<tbm_PermissionGroup> DeletePermissionGroup(string permissionGroupCode)
		/// \brief (Call stored procedure: sp_MA_DeletePermissionGroup).

		/// \fn public virtual List<tbm_PermissionIndividual> DeletePermissionIndividual(string permissionGroupCode, string permissionIndividualCode)
		/// \brief (Call stored procedure: sp_MA_DeletePermissionIndividual).

		/// \fn public virtual List<tbm_PermissionIndividualDetail> DeletePermissionIndividualDetail(string permissionGroupCode, string permissionIndividualCode)
		/// \brief (Call stored procedure: sp_MA_DeletePermissionIndividualDetail).

		/// \fn public virtual List<tbm_PermissionIndividualDetail> DeletePermissionIndividualDetailByEmpNo(string permissionGroupCode, string permissionIndividualCode, string empNoList)
		/// \brief (Call stored procedure: sp_MA_DeletePermissionIndividualDetailByEmpNo).

		/// \fn public virtual List<tbm_Site> DeleteSite(string pchnvSiteCode)
		/// \brief (Call stored procedure: sp_MA_DeleteSite).

		/// \fn public virtual List<tbm_CustomerGroup> DeleteTbm_CustomerGroup(string pGroupCode, string pCustCode)
		/// \brief (Call stored procedure: sp_MA_DeleteTbm_CustomerGroup).

		/// \fn public virtual List<string> GeneratePermissionGroupCode()
		/// \brief (Call stored procedure: sp_MA_GeneratePermissionGroupCode).

		/// \fn public virtual List<string> GeneratePermissionIndividualCode()
		/// \brief (Call stored procedure: sp_MA_GeneratePermissionIndividualCode).

		/// \fn public virtual List<Nullable<int>> GenerateSiteCode(string pcharCustCode)
		/// \brief (Call stored procedure: sp_MA_GenerateSiteCode).

		/// \fn public virtual List<tbm_Employee> GetActiveEmployee(string strEmpNo)
		/// \brief (Call stored procedure: sp_MA_GetActiveEmployee).

		/// \fn public virtual List<doActiveEmployeeList> GetActiveEmployeeList(string xml0)
		/// \brief (Call stored procedure: sp_MA_GetActiveEmployeeList).

		/// \fn public virtual List<tbm_Product> GetActiveProduct(string pcharProductCode, string pcharProductTypeCode)
		/// \brief (Call stored procedure: sp_MA_GetActiveProduct).

		/// \fn public virtual List<tbm_Office> GetAROffice()
		/// \brief (Call stored procedure: sp_MA_GetAROffice).

		/// \fn public virtual List<dtAuthorizeOffice> GetAuthorizeOffice(string xml_dtBelonging)
		/// \brief (Call stored procedure: sp_MA_GetAuthorizeOffice).

		/// \fn public virtual List<Nullable<System.DateTime>> GetAutoTransferDate(string bankCode, string autoTransferDate)
		/// \brief (Call stored procedure: sp_MA_GetAutoTransferDate).

		/// \fn public virtual List<dtBankBranch> GetBankBranch()
		/// \brief (Call stored procedure: sp_MA_GetBankBranch).

		/// \fn public virtual List<dtBelonging> GetBelonging(string pchvOfficeCode, string pchrDepartmentCode, string pchrPositionCode, string pchvEmpNo)
		/// \brief (Call stored procedure: sp_MA_GetBelonging).

		/// \fn public virtual List<dtUserBelonging> getBelongingByEmpNo(string pchvEmpNo)
		/// \brief (Call stored procedure: sp_MA_GetBelongingByEmpNo).

		/// \fn public virtual List<dtDepartment> GetBelongingDepartmentList(string strOfficeCode, string strDepCode)
		/// \brief (Call stored procedure: sp_MA_GetBelongingDepartmentList).

		/// \fn public virtual List<dtEmployeeBelonging> GetBelongingEmpList(string strOfficeCode, string strDepartmentCode, Nullable<bool> bChiefFlag, Nullable<bool> bApproverFlag, Nullable<bool> bCorrespondentFlag)
		/// \brief (Call stored procedure: sp_MA_GetBelongingEmpList).

		/// \fn public virtual List<dtBelongingEmpNo> GetBelongingEmpNoByOffice(string officeCode)
		/// \brief (Call stored procedure: sp_MA_GetBelongingEmpNoByOffice).

		/// \fn public virtual List<dtBelongingOffice> GetBelongingOfficeList(string pEmpNo)
		/// \brief (Call stored procedure: sp_MA_GetBelongingOfficeList).

		/// \fn public virtual List<Nullable<System.DateTime>> GetBelongingUpdateDate(Nullable<int> pintBelongingID)
		/// \brief (Call stored procedure: sp_MA_GetBelongingUpdateDate).

		/// \fn public virtual List<doBillingAddressNameEN> GetBillingAddressNameEN(string pchvnstrAutoComplete)
		/// \brief (Call stored procedure: sp_MA_GetBillingAddressNameEN).

		/// \fn public virtual List<doBillingAddressNameLC> GetBillingAddressNameLC(string pchvnstrAutoComplete)
		/// \brief (Call stored procedure: sp_MA_GetBillingAddressNameLC).

		/// \fn public virtual List<doBillingBranchNameEN> GetBillingBranchNameEN(string pchvnstrAutoComplete)
		/// \brief (Call stored procedure: sp_MA_GetBillingBranchNameEN).

		/// \fn public virtual List<doBillingBranchNameLC> GetBillingBranchNameLC(string pchvnstrAutoComplete)
		/// \brief (Call stored procedure: sp_MA_GetBillingBranchNameLC).

		/// \fn public virtual List<dtBillingClientData> GetBillingClient(string c_CUST_TYPE, string strBillingClientCode)
		/// \brief (Call stored procedure: sp_MA_GetBillingClient).

		/// \fn public virtual List<dtBillingClientAddress> GetBillingClientAddress(string pchvnstrAutoComplete)
		/// \brief (Call stored procedure: sp_MA_GetBillingClientAddress).

		/// \fn public virtual List<dtBillingClientDataForSearch> GetBillingClientDataForSearch(string pC_CUST_TYPE, string pchrBillingClientCode, string pchvnBillingClientName, string pchrCompanyTypeCode, string pchrRegionCode, string pchrBusinessTypeCode, string pchvnAddress, string pchvnPhoneNo, string xmlCustomerTypeCode)
		/// \brief (Call stored procedure: sp_MA_GetBillingClientDataForSearch).

		/// \fn public virtual List<dtBillingClientName> GetBillingClientName(string pchvnstrAutoComplete)
		/// \brief (Call stored procedure: sp_MA_GetBillingClientName).

		/// \fn public virtual List<doBillingNameEN> GetBillingNameEn(string pchvnstrAutoComplete)
		/// \brief (Call stored procedure: sp_MA_GetBillingNameEN).

		/// \fn public virtual List<doGetBillingNameLC> GetBillingNameLC(string pchvnstrAutoComplete)
		/// \brief (Call stored procedure: sp_MA_GetBillingNameLC).

		/// \fn public virtual List<tbm_BillingType> GetBillingTypeList(string billingServiceTypeCode)
		/// \brief (Call stored procedure: sp_MA_GetBillingTypeList).

		/// \fn public virtual List<tbm_BillingType> GetBillingTypeOneTimeList(string billingServiceTypeCode)
		/// \brief (Call stored procedure: sp_MA_GetBillingTypeOneTimeList).

		/// \fn public virtual List<doInstrumentExpansion> GetChildInstrument(string pchvnChildInstrumentCode, string c_LINE_UP_TYPE, string c_EXPANSION_TYPE_CHILD)
		/// \brief (Call stored procedure: sp_MA_GetChildInstrument).

		/// \fn public virtual List<doCompanyType> GetCompanyType(string pchrstrCompanyTypeCode)
		/// \brief (Call stored procedure: sp_MA_GetCompanyType).

		/// \fn public virtual List<doCustAddress> GetCustAddress(string pchvnstrAutoComplete)
		/// \brief (Call stored procedure: sp_MA_GetCustAddress).

		/// \fn public virtual List<doCustAddressEN> GetCustAddressEN(string pchvnstrAutoComplete)
		/// \brief (Call stored procedure: sp_MA_GetCustAddressEN).

		/// \fn public virtual List<doCustAddressLC> GetCustAddressLC(string pchvnstrAutoComplete)
		/// \brief (Call stored procedure: sp_MA_GetCustAddressLC).

		/// \fn public virtual List<doCustAlley> GetCustAlley(string pchvnstrAutoComplete)
		/// \brief (Call stored procedure: sp_MA_GetCustAlley).

		/// \fn public virtual List<doCustAlleyEN> GetCustAlleyEN(string pchvnstrAutoComplete)
		/// \brief (Call stored procedure: sp_MA_GetCustAlleyEN).

		/// \fn public virtual List<doCustAlleyLC> GetCustAlleyLC(string pchvnstrAutoComplete)
		/// \brief (Call stored procedure: sp_MA_GetCustAlleyLC).

		/// \fn public virtual List<dtCustName> GetCustName(string pLiveSearch)
		/// \brief (Call stored procedure: sp_MA_GetCustName).

		/// \fn public virtual List<dtCustNameEN> GetCustNameEN(string pchvnstrAutoComplete)
		/// \brief (Call stored procedure: sp_MA_GetCustNameEN).

		/// \fn public virtual List<dtCustNameLC> GetCustNameLC(string pchvnstrAutoComplete)
		/// \brief (Call stored procedure: sp_MA_GetCustNameLC).

		/// \fn public virtual List<dtCustomeGroupData> GetCustomeGroupData(string pchrCustomerCode, string chvnGroupName)
		/// \brief (Call stored procedure: sp_MA_GetCustomeGroupData).

		/// \fn public virtual List<doCustomer> GetCustomer(string pchrCustCode, string pcharC_CUST_STATUS, string pcharC_CUST_TYPE, string pcharC_FINANCIAL_MARKET_TYPE)
		/// \brief (Call stored procedure: sp_MA_GetCustomer).

		/// \fn public virtual List<doCustomer> GetCustomerAll(string pchrCustCode, string pcharC_CUST_STATUS, string pcharC_CUST_TYPE, string pcharC_FINANCIAL_MARKET_TYPE)
		/// \brief (Call stored procedure: sp_MA_GetCustomerAll).

		/// \fn public virtual List<dtCustomerData> GetCustomerDataForSearch(string pchrCustomerCode, string chvnCustomerName, string pchrCompanyTypeCode, string chvnIDNo, Nullable<bool> bitDummyIDFlag, string chrRegionCode, string chrBusinessTypeCode, string chvnAddress, string chvnAlley, string chvnRoad, string chvnSubDistrict, string chrDistrictCode, string chrProvinceCode, string chrZipCode, string chvnPhoneNo, string chvnGroupName, string pC_CUST_STATUS, string pC_CUST_TYPE, string pC_FINANCIAL_MARKET_TYPE, string xmlCustStatus, string xmlCustTypeCode)
		/// \brief (Call stored procedure: sp_MA_GetCustomerDataForSearch).

		/// \fn public virtual List<dtCustomerForView> GetCustomerForView(string chrCustCode, string pC_CUST_STATUS, string pC_CUST_TYPE, string pC_FINANCIAL_MARKET_TYPE)
		/// \brief (Call stored procedure: sp_MA_GetCustomerForView).

		/// \fn public virtual List<dtCustomerGroupForView> GetCustomerGroup(string pchrGroupCode, string pchrCustCode)
		/// \brief (Call stored procedure: sp_MA_GetCustomerGroup).

		/// \fn public virtual List<doCustomerList> GetCustomerList(string xml0)
		/// \brief (Call stored procedure: sp_MA_GetCustomerList).

		/// \fn public virtual List<doCustRoad> GetCustRoad(string pchvnstrAutoComplete)
		/// \brief (Call stored procedure: sp_MA_GetCustRoad).

		/// \fn public virtual List<doCustRoadEN> GetCustRoadEN(string pchvnstrAutoComplete)
		/// \brief (Call stored procedure: sp_MA_GetCustRoadEN).

		/// \fn public virtual List<doCustRoadLC> GetCustRoadLC(string pchvnstrAutoComplete)
		/// \brief (Call stored procedure: sp_MA_GetCustRoadLC).

		/// \fn public virtual List<doCustSubDistrict> GetCustSubDistrict(string pchvnstrAutoComplete)
		/// \brief (Call stored procedure: sp_MA_GetCustSubDistrict).

		/// \fn public virtual List<doCustSubDistrictEN> GetCustSubDistrictEN(string pchvnstrAutoComplete)
		/// \brief (Call stored procedure: sp_MA_GetCustSubDistrictEN).

		/// \fn public virtual List<doCustSubDistrictLC> GetCustSubDistrictLC(string pchvnstrAutoComplete)
		/// \brief (Call stored procedure: sp_MA_GetCustSubDistrictLC).

		/// \fn public virtual List<dtDocumentTemplateByDocumentCode> GetDocumentTemplateByDocumentCode(string pDocumentCode)
		/// \brief (Call stored procedure: sp_CM_GetDocumentTemplateByDocumentCode).

		/// \fn public virtual List<dtGetEmailAddress> GetEmailAddress(string pcharEmpFirstNameEN, string pcharEmailAddress, string pcharOfficeCode, string pcharDepartmentCode)
		/// \brief (Call stored procedure: sp_MA_GetEmailAddress).

		/// \fn public virtual List<dtEmailAddressForIncident> GetEmailAddressForIncident(Nullable<bool> c_FLAG_ON)
		/// \brief (Call stored procedure: sp_MA_GetEmailAddressForIncident).

		/// \fn public virtual List<dtEmployee> GetEmployee(string pchvEmpNo, string pchrnEmpFirstNameEN, Nullable<bool> pbitMainDepartmentFlag)
		/// \brief (Call stored procedure: sp_MA_GetEmployee).

		/// \fn public virtual List<string> GetEmployeeCode(string empNo)
		/// \brief (Call stored procedure: sp_MA_GetEmployeeCode).

		/// \fn public virtual List<dtEmployeeDetail> GetEmployeeDetail(string pchvEmpNo)
		/// \brief (Call stored procedure: sp_MA_GetEmployeeDetail).

		/// \fn public virtual List<tbm_Employee> GetEmployeeList(string xml0)
		/// \brief (Call stored procedure: sp_MA_GetEmployeeList).

		/// \fn public virtual List<dtEmployeeName> GetEmployeeName(string pchrnEmpFirstNameEN)
		/// \brief (Call stored procedure: sp_MA_GetEmployeeName).

		/// \fn public virtual List<dtEmpNo> GetEmployeeNameByEmpNo(string empNo)
		/// \brief (Call stored procedure: sp_MA_GetEmployeeNameByEmpNo).

		/// \fn public virtual List<dtEmployeeOffice> GetEmployeeOffice(string strEmpNo, Nullable<bool> blnMainDepartmentFlag)
		/// \brief (Call stored procedure: sp_MA_GetEmployeeOffice).

		/// \fn public virtual List<Nullable<System.DateTime>> GetEmployeeUpdateDate(string pchvEmpNo)
		/// \brief (Call stored procedure: sp_MA_GetEmployeeUpdateDate).

		/// \fn public virtual List<dtEmpNo> GetEmpNo(string permissionGroupCode, string permissionIndividualCode)
		/// \brief (Call stored procedure: sp_MA_GetEmpNo).

		/// \fn public virtual List<dtFunction> GetFunction(string permissionGroupCode, string permissionIndividualCode)
		/// \brief (Call stored procedure: sp_MA_GetFunction).

		/// \fn public virtual List<doFunctionBilling> GetFunctionBilling(string pchrC_FUNC_BILLING_NO)
		/// \brief (Call stored procedure: sp_MA_GetFunctionBilling).

		/// \fn public virtual List<dtOffice> GetFunctionLogistic(string c_FUNC_LOGISTIC_NO)
		/// \brief (Call stored procedure: sp_MA_GetFunctionLogistic).

		/// \fn public virtual List<dtOffice> GetFunctionQuotaion(string pchrC_FUNC_QUATATION_NO)
		/// \brief (Call stored procedure: sp_MA_GetFunctionQuotation).

		/// \fn public virtual List<dtOffice> GetFunctionSale(string pchrC_FUNC_SALE_NO)
		/// \brief (Call stored procedure: sp_MA_GetFunctionSale).

		/// \fn public virtual List<dtOffice> GetFunctionSecurity(string pchrC_FUNC_SECURITY_NO)
		/// \brief (Call stored procedure: sp_MA_GetFunctionSecurity).

		/// \fn public virtual List<doGroup> GetGroup(string pchvnGroupCode, string pchvnGroupName)
		/// \brief (Call stored procedure: sp_MA_GetGroup).

		/// \fn public virtual List<doGroupNameDataList> GetGroupNameDataList(string pchvnstrAutoComplete)
		/// \brief (Call stored procedure: sp_MA_GetGroupNameDataList).

		/// \fn public virtual List<tbm_Office> GetIncidentOffice()
		/// \brief (Call stored procedure: sp_MA_GetIncidentOffice).

		/// \fn public virtual List<dtInstrument> GetInstrument(string pchvInstrumentCode, string pchvInstrumentName, string pchrLineUpTypeCode, string pchvFieldName)
		/// \brief (Call stored procedure: sp_MA_GetInstrument).

		/// \fn public virtual List<doInstrumentData> GetInstrumentDataForSearch(string phvInstrumentCode, string phnvInstrumentName, string phnvMaker, string pchrSupplierCode, string pchrLineUpTypeCode, Nullable<int> instrumentFlag_1, Nullable<int> instrumentFlag_2, string expansionType_1, string expansionType_2, Nullable<int> saleFlag, Nullable<int> rentalFlag, string instrumentType_1, string instrumentType_2, string instrumentType_3, string c_LINE_UP_TYPE)
		/// \brief (Call stored procedure: sp_MA_GetInstrumentDataForSearch).

		/// \fn public virtual List<dtInstrumentDetail> GetInstrumentDetail(string pchvInstrumentCode, string pchvFieldName)
		/// \brief (Call stored procedure: sp_MA_GetInstrumentDetail).

		/// \fn public virtual List<doInstrumentExpansion> GetInstrumentExpansion(string pchvnInstrumentCode, string c_LINE_UP_TYPE)
		/// \brief (Call stored procedure: sp_MA_GetInstrumentExpansion).

		/// \fn public virtual List<doInstrumentData> GetInstrumentListForView(string xml0, string c_LINE_UP_TYPE)
		/// \brief (Call stored procedure: sp_MA_GetInstrumentListForView).

		/// \fn public virtual List<dtGetInstrumentMaker> GetInstrumentMaker(string pchnvStrAuto)
		/// \brief (Call stored procedure: sp_MA_GetInstrumentMaker).

		/// \fn public virtual List<doInstrumentName> GetInstrumentName(string pchvInstrumentName)
		/// \brief (Call stored procedure: sp_MA_GetInstrumentName).

		/// \fn public virtual List<Nullable<System.DateTime>> GetInstrumentUpdateDate(string pchvInstrumentCode)
		/// \brief (Call stored procedure: sp_MA_GetInstrumentUpdateDate).

		/// \fn public virtual List<tbm_Instrument> GetIntrumentList(string xml_dtIntrumentCode)
		/// \brief (Call stored procedure: sp_MA_GetIntrumentList).

		/// \fn public virtual List<doBelongingData> GetMainBelongingByEmpNo(string strEmpNo)
		/// \brief (Call stored procedure: sp_MA_GetMainBelongingByEmpNo).

		/// \fn public virtual List<doMonitoringInstrument> GetMonitoringInstrumentList(string xml0, string c_LINE_UP_TYPE, string c_INSTRUMENT_TYPE_MONITORING)
		/// \brief (Call stored procedure: sp_MA_GetMonitoringInstrumentList).

		/// \fn public virtual List<dtObjectFunction> GetObjectFunction(Nullable<int> moduleID)
		/// \brief (Call stored procedure: sp_MA_GetObjectFunction).

		/// \fn public virtual List<dtOfficeChief> GetOfficeChiefList(string strOfficeCode, Nullable<bool> c_FLAG_ON)
		/// \brief (Call stored procedure: sp_MA_GetOfficeChiefList).

		/// \fn public virtual List<doOfficeList> GetOfficeList(string xml0)
		/// \brief (Call stored procedure: sp_MA_GetOfficeList).

		/// \fn public virtual List<doParentGeneralInstrument> GetParentGeneralInstrumentList(string xml0, string c_LINE_UP_TYPE, string c_INSTRUMENT_TYPE_GENERAL, string c_EXPANSION_TYPE_PARENT, string c_LINE_UP_TYPE_STOP_SALE, string c_LINE_UP_TYPE_LOGICAL_DELETE)
		/// \brief (Call stored procedure: sp_MA_GetParentGeneralInstrumentList).

		/// \fn public virtual List<doParentInstrument> GetParentInstrument(string pchvnInstrumentCode, string c_EXPANSION_TYPE_PARENT)
		/// \brief (Call stored procedure: sp_MA_GetParentInstrument).

		/// \fn public virtual List<dtPermissionHeader> GetPermission(Nullable<bool> typeOffice, Nullable<bool> typeIndividual, string permissionGroupName, string officeCode, string department, string positionCode, string dtScreenFunction, string empNo, string c_PERMISSION_TYPE, string c_PERMISSION_TYPE_OFFICE, string c_PERMISSION_TYPE_INDIVIDUAL)
		/// \brief (Call stored procedure: sp_MA_GetPermission).

		/// \fn public virtual List<Nullable<System.DateTime>> GetPermissionDetailUpdateDate(string permissionGroupCode, string permissionIndividualCode)
		/// \brief (Call stored procedure: sp_MA_GetPermissionDetailUpdateDate).

		/// \fn public virtual List<string> GetPermissionGroupName(string empFirstNameEN)
		/// \brief (Call stored procedure: sp_MA_GetPermissionGroupName).

		/// \fn public virtual List<Nullable<System.DateTime>> GetPermissionGroupUpdateDate(string permissionGroupCode)
		/// \brief (Call stored procedure: sp_MA_GetPermissionGroupUpdatDate).

		/// \fn public virtual List<Nullable<System.DateTime>> GetPermissionIndividualDetailUpdateDate(string permissionGroupCode, string permissionIndividualCode)
		/// \brief (Call stored procedure: sp_MA_GetPermissionIndividualDetailUpdateDate).

		/// \fn public virtual List<Nullable<System.DateTime>> GetPermissionIndividualUpdateDate(string permissionGroupCode, string permissionIndividualCode)
		/// \brief (Call stored procedure: sp_MA_GetPermissionIndividualUpdateDate).

		/// \fn public virtual List<string> GetPositionCodeAtMaxPositionLevel()
		/// \brief (Call stored procedure: sp_MA_GetPositionCodeAtMaxPositionLevel).

		/// \fn public virtual List<dtProductName> GetProductName(string pchvnstrAutoComplete)
		/// \brief (Call stored procedure: sp_MA_GetProductName).

		/// \fn public virtual List<doSafetyStock> GetSafetyStock(string instrumentCode)
		/// \brief (Call stored procedure: sp_MA_GetSafetyStock).

		/// \fn public virtual List<tbm_SecomBankAccount> GetSecomBankAccountForAutoTransfer(string bankCode)
		/// \brief (Call stored procedure: sp_MA_GetSecomBankAccountForAutoTransfer).

		/// \fn public virtual List<doShelf> GetShelf(string shelfNo, string shelfName, string shelfTypeCode, string areaCode)
		/// \brief (Call stored procedure: sp_MA_GetShelf).

		/// \fn public virtual List<doShelfName> GetShelfName(string strLiveSearch)
		/// \brief (Call stored procedure: sp_MA_GetShelfName).

		/// \fn public virtual List<doSite> GetSite(string siteCode, string custCode)
		/// \brief (Call stored procedure: sp_MA_GetSite).

		/// \fn public virtual List<dtSiteAddress> GetSiteAddress(string pchvnstrAutoComplete)
		/// \brief (Call stored procedure: sp_MA_GetSiteAddress).

		/// \fn public virtual List<dtSiteAddressEN> GetSiteAddressEN(string pchvnstrAutoComplete)
		/// \brief (Call stored procedure: sp_MA_GetSiteAddressEN).

		/// \fn public virtual List<dtSiteAddressLC> GetSiteAddressLC(string pchvnstrAutoComplete)
		/// \brief (Call stored procedure: sp_MA_GetSiteAddressLC).

		/// \fn public virtual List<dtSiteAlley> GetSiteAlley(string pchvnstrAutoComplete)
		/// \brief (Call stored procedure: sp_MA_GetSiteAlley).

		/// \fn public virtual List<dtSiteAlleyEN> GetSiteAlleyEN(string pchvnstrAutoComplete)
		/// \brief (Call stored procedure: sp_MA_GetSiteAlleyEN).

		/// \fn public virtual List<dtSiteAlleyLC> GetSiteAlleyLC(string pchvnstrAutoComplete)
		/// \brief (Call stored procedure: sp_MA_GetSiteAlleyLC).

		/// \fn public virtual List<dtSiteData> GetSiteDataForSearch(string pC_CUST_STATUS, string pCustomerCode, string pCustomerName, string pSiteCode, string pSiteName, string pBuildingUsageCode, string pAddress, string pAlley, string pRoad, string pSubDistrict, string pDistrictCode, string pProvinceCode, string pZipCode, string pXmlCustStatus)
		/// \brief (Call stored procedure: sp_MA_GetSiteDataForSearch).

		/// \fn public virtual List<dtSiteName> GetSiteName(string pchvnstrAutoComplete)
		/// \brief (Call stored procedure: sp_MA_GetSiteName).

		/// \fn public virtual List<dtSiteNameEN> GetSiteNameEN(string pchvnstrAutoComplete)
		/// \brief (Call stored procedure: sp_MA_GetSiteNameEN).

		/// \fn public virtual List<dtSiteNameLC> GetSiteNameLC(string pchvnstrAutoComplete)
		/// \brief (Call stored procedure: sp_MA_GetSiteNameLC).

		/// \fn public virtual List<dtSiteRoad> GetSiteRoad(string pchvnstrAutoComplete)
		/// \brief (Call stored procedure: sp_MA_GetSiteRoad).

		/// \fn public virtual List<dtSiteRoadEN> GetSiteRoadEN(string pchvnstrAutoComplete)
		/// \brief (Call stored procedure: sp_MA_GetSiteRoadEN).

		/// \fn public virtual List<dtSiteRoadLC> GetSiteRoadLC(string pchvnstrAutoComplete)
		/// \brief (Call stored procedure: sp_MA_GetSiteRoadLC).

		/// \fn public virtual List<dtSiteSubDistrict> GetSiteSubDistrict(string pchvnstrAutoComplete)
		/// \brief (Call stored procedure: sp_MA_GetSiteSubDistrict).

		/// \fn public virtual List<dtSiteSubDistrictEN> GetSiteSubDistrictEN(string pchvnstrAutoComplete)
		/// \brief (Call stored procedure: sp_MA_GetSiteSubDistrictEN).

		/// \fn public virtual List<dtSiteSubDistrictLC> GetSiteSubDistrictLC(string pchvnstrAutoComplete)
		/// \brief (Call stored procedure: sp_MA_GetSiteSubDistrictLC).

		/// \fn public virtual List<doSubcontractor> GetSubcontractor(string subcontractorCode, string coCompanyCode, string installationTeam, string subcontractorName)
		/// \brief (Call stored procedure: sp_MA_GetSubcontractor).

		/// \fn public virtual List<doSubcontractorAddress> GetSubcontractorAddress(string strLiveSearch)
		/// \brief (Call stored procedure: sp_MA_GetSubcontractorAddress).

		/// \fn public virtual List<doSubcontractorAddressEN> GetSubcontractorAddressEN(string strLiveSearch)
		/// \brief (Call stored procedure: sp_MA_GetSubcontractorAddressEN).

		/// \fn public virtual List<doSubcontractorAddressLC> GetSubcontractorAddressLC(string strLiveSearch)
		/// \brief (Call stored procedure: sp_MA_GetSubcontractorAddressLC).

		/// \fn public virtual List<doSubcontractor> GetSubcontractorDetail(string subcontractorCode)
		/// \brief (Call stored procedure: sp_MA_GetSubcontractorDetail).

		/// \fn public virtual List<doSubcontractorName> GetSubcontractorName(string strLiveSearch)
		/// \brief (Call stored procedure: sp_MA_GetSubcontractorName).

		/// \fn public virtual List<doSubcontractorNameEN> GetSubcontractorNameEN(string strLiveSearch)
		/// \brief (Call stored procedure: sp_MA_GetSubcontractorNameEN).

		/// \fn public virtual List<doSubcontractorNameLC> GetSubcontractorNameLC(string strLiveSearch)
		/// \brief (Call stored procedure: sp_MA_GetSubcontractorNameLC).

		/// \fn public virtual List<tbm_Supplier> GetSupplier(string strSupplierCode, string strSupplierName)
		/// \brief (Call stored procedure: sp_MA_GetSupplier).

		/// \fn public virtual List<string> GetSupplierName(string strLiveSearch)
		/// \brief (Call stored procedure: sp_MA_GetSupplierName).

		/// \fn public virtual List<tbm_AutoTransferScheduleList> GetTbm_AutoTransferScheduleList(string bankCode, string autoTransferDate)
		/// \brief (Call stored procedure: sp_MA_GetTbm_AutoTransferScheduleList).

		/// \fn public virtual List<tbm_Bank> GetTbm_Bank()
		/// \brief (Call stored procedure: sp_MA_GetTbm_Bank).

		/// \fn public virtual List<tbm_BankBranch> GetTbm_BankBranch(string bankCode)
		/// \brief (Call stored procedure: sp_MA_GetTbm_BankBranch).

		/// \fn public virtual List<tbm_BillingClient> GetTbm_BillingClient(string billingClientCode)
		/// \brief (Call stored procedure: sp_MA_GetTbm_BillingClient).

		/// \fn public virtual List<tbm_BillingType> GetTbm_BillingType(string billingTypeCode)
		/// \brief (Call stored procedure: sp_MA_GetTbm_BillingType).

		/// \fn public virtual List<tbm_BuildingUsage> GetTbm_BiuldingUsage()
		/// \brief (Call stored procedure: sp_MA_GetTbm_BuildingUsage).

		/// \fn public virtual List<tbm_BusinessType> GetTbm_BusinessType()
		/// \brief (Call stored procedure: sp_MA_GetTbm_BusinessType).

		/// \fn public virtual List<tbm_Calendar> GetTbm_Calendar(Nullable<System.DateTime> onDate)
		/// \brief (Call stored procedure: sp_MA_GetTbm_Calendar).

		/// \fn public virtual List<tbm_CompanyType> GetTbm_CompanyType()
		/// \brief (Call stored procedure: sp_MA_GetTbm_CompanyType).

		/// \fn public virtual List<tbm_CreditCardCompany> GetTbm_CreditCardCompany()
		/// \brief (Call stored procedure: sp_MA_GetTbm_CreditCardCompany).

		/// \fn public virtual List<tbm_Customer> GetTbm_Customer(string custCode)
		/// \brief (Call stored procedure: sp_MA_GetTbm_Customer).

		/// \fn public virtual List<tbm_Department> GetTbm_Department()
		/// \brief (Call stored procedure: sp_MA_GetTbm_Department).

		/// \fn public virtual List<tbm_District> GetTbm_District(string provinceCode)
		/// \brief (Call stored procedure: sp_MA_GetTbm_District).

		/// \fn public virtual List<tbm_Employee> GetTbm_Employee(string pEmpNo)
		/// \brief (Call stored procedure: sp_MA_GetTbm_Employee).

		/// \fn public virtual List<tbm_Group> GetTbm_Group(string pGroupCode)
		/// \brief (Call stored procedure: sp_MA_GetTbm_Group).

		/// \fn public virtual List<tbm_Instrument> GetTbm_Instrument(string instrumentCode)
		/// \brief (Call stored procedure: sp_MA_GetTbm_Instrument).

		/// \fn public virtual List<tbm_InstrumentExpansion> GetTbm_InstrumentExpansion(string pInstrumentCode, string pChildInstrumentCode)
		/// \brief (Call stored procedure: sp_MA_GetTbm_InstrumentExpansion).

		/// \fn public virtual List<tbm_Module> GetTbm_Module()
		/// \brief (Call stored procedure: sp_MA_GetTbm_Module).

		/// \fn public virtual List<tbm_Object> GetTbm_Object()
		/// \brief (Call stored procedure: sp_MA_GetTbm_Object).

		/// \fn public virtual List<tbm_Office> GetTbm_Office(string pchrOfficeCode)
		/// \brief (Call stored procedure: sp_MA_GetTbm_Office).

		/// \fn public virtual List<tbm_Position> GetTbm_Position()
		/// \brief (Call stored procedure: sp_MA_GetTbm_Position).

		/// \fn public virtual List<tbm_Product> GetTbm_Product(string pchrProductCode, string pcharProductTypeCode)
		/// \brief (Call stored procedure: sp_MA_GetTbm_Product).

		/// \fn public virtual List<tbm_ProductFacility> GetTbm_ProductFacility(string pchrProductCode)
		/// \brief (Call stored procedure: sp_MA_GetTbm_ProductFacility).

		/// \fn public virtual List<tbm_ProductInstrument> GetTbm_ProductInstrument(string pchrProductCode)
		/// \brief (Call stored procedure: sp_MA_GetTbm_ProductInstrument).

		/// \fn public virtual List<tbm_Province> GetTbm_Province()
		/// \brief (Call stored procedure: sp_MA_GetTbm_Province).

		/// \fn public virtual List<tbm_Region> GetTbm_Region()
		/// \brief (Call stored procedure: sp_MA_GetTbm_Region).

		/// \fn public virtual List<tbm_SecomBankAccount> GetTbm_SecomBankAccount(Nullable<int> secomAccountID)
		/// \brief (Call stored procedure: sp_MA_GetTbm_SecomBankAccount).

		/// \fn public virtual List<tbm_Shelf> GetTbm_Shelf(string pShelfNo)
		/// \brief (Call stored procedure: sp_MA_GetTbm_Shelf).

		/// \fn public virtual List<doGetTbm_Site> GetTbm_Site(string pchrSiteCode)
		/// \brief (Call stored procedure: sp_MA_GetTbm_Site).

		/// \fn public virtual List<tbs_MiscellaneousTypeCode> GetTbs_MiscellaneousTypeCode(string c_CUST_TYPE)
		/// \brief (Call stored procedure: sp_MA_GetTbs_MiscellaneousTypeCode).

		/// \fn public virtual List<dtEmployeeData> GetUserData(string pchvEmpNo)
		/// \brief (Call stored procedure: sp_MA_GetUserData).

		/// \fn public virtual List<dtEmailAddress> GetUserEmailAddressDataList(string pchvEmployeeName, string pchvnEmailAddress, string pchvOfficeCode, string pchrDepartmentCode)
		/// \brief (Call stored procedure: sp_MA_GetUserEmailAddressDataList).

		/// \fn public virtual List<tbm_Belonging> InsertBelonging(string xmlBelonging)
		/// \brief (Call stored procedure: sp_MA_InsertBelonging).

		/// \fn public virtual List<tbm_BillingClient> InsertBillingClient(string billingClientCode, string nameEN, string nameLC, string fullNameEN, string fullNameLC, string branchNameEN, string branchNameLC, string custTypeCode, string companyTypeCode, string businessTypeCode, string phoneNo, string iDNo, string regionCode, string addressEN, string addressLC, Nullable<System.DateTime> createDate, string createBy, Nullable<System.DateTime> updateDate, string updateBy, Nullable<bool> deleteFlag)
		/// \brief (Call stored procedure: sp_MA_InsertBillingClient).

		/// \fn public virtual List<tbm_Customer> InsertCustomer(string custCode, string custStatus, Nullable<bool> importantFlag, string custNameEN, string custNameLC, string custFullNameEN, string custFullNameLC, string repPersonName, string contactPersonName, string sECOMContactPerson, string custTypeCode, string companyTypeCode, string financialMarketTypeCode, string businessTypeCode, string phoneNo, string faxNo, string iDNo, Nullable<bool> dummyIDFlag, string regionCode, string uRL, string memo, string alleyEN, string addressEN, string roadEN, string subDistrictEN, string addressFullEN, string addressLC, string alleyLC, string roadLC, string subDistrictLC, string addressFullLC, string districtCode, string provinceCode, string zipCode, Nullable<bool> deleteFlag, Nullable<System.DateTime> createDate, string createBy, Nullable<System.DateTime> updateDate, string updateBy)
		/// \brief (Call stored procedure: sp_MA_InsertCustomer).

		/// \fn public virtual List<tbm_CustomerGroup> InsertCustomerGroup(string xml_dtCustomerGroup)
		/// \brief (Call stored procedure: sp_MA_InsertCustomerGroup).

		/// \fn public virtual List<tbm_Employee> InsertEmployee(string xmlEmployee)
		/// \brief (Call stored procedure: sp_MA_InsertEmployee).

		/// \fn public virtual List<tbm_Group> InsertGroup(string pchrGroupCode, string pchvnGroupNameEN, string pchnvGroupNameLC, string pchnvMemo, string pchnvGroupOffice, string pchnvGroupEmpNo, Nullable<bool> pbitDeleteFlag, Nullable<System.DateTime> pdtmCreateDate, string pchnvCreateBy, Nullable<System.DateTime> pdtmUpdateDate, string pchnvUpdateBy)
		/// \brief (Call stored procedure: sp_MA_InsertGroup).

		/// \fn public virtual List<tbm_Instrument> InsertInstrument(string xmlInstrument)
		/// \brief (Call stored procedure: sp_MA_InsertInstrument).

		/// \fn public virtual List<tbm_InstrumentExpansion> InsertInstrumentExpansion(string pchnvInstrumentCode, string pchnvChildInstrumentCode, Nullable<System.DateTime> pdtmCreateDate, string pchnvCreateBy, Nullable<System.DateTime> pdtmUpdateDate, string pchnvUpdateBy)
		/// \brief (Call stored procedure: sp_MA_InsertInstrumentExpansion).

		/// \fn public virtual List<tbm_PermissionDetail> InsertPermissionDetailFromSelectedFunction(string permissionGroupCode, string objectFunction, string permissionIndividualCode, Nullable<System.DateTime> createDate, string createBy)
		/// \brief (Call stored procedure: sp_MA_InsertPermissionDetailFromSelectedFunction).

		/// \fn public virtual List<tbm_PermissionGroup> InsertPermissionGroup(string permissionGroupCode, string permissionGroupName, string officeCode, string departmentCode, string positionCode, Nullable<System.DateTime> createDate, string createBy)
		/// \brief (Call stored procedure: sp_MA_InsertPermissionGroup).

		/// \fn public virtual List<tbm_PermissionIndividual> InsertPermissionIndividual(string permissionGroupCode, string permissionIndividualCode, string permissionIndividualName, Nullable<System.DateTime> createDate, string createBy)
		/// \brief (Call stored procedure: sp_MA_InsertPermissionIndividual).

		/// \fn public virtual List<tbm_PermissionIndividualDetail> InsertPermissionIndividualDetail(string permissionGroupCode, string permissionIndividualCode, string empNo, Nullable<System.DateTime> createDate, string createBy)
		/// \brief (Call stored procedure: sp_MA_InsertPermissionIndividualDetail).

		/// \fn public virtual List<tbm_PermissionIndividualDetail> InsertPermissionIndividualDetailByEmpNo(string permissionGroupCode, string permissionIndividualCode, string empNoList, Nullable<System.DateTime> createDate, string createBy)
		/// \brief (Call stored procedure: sp_MA_InsertPermissionIndividualDetailByEmpNo).

		/// \fn public virtual List<tbm_SafetyStock> InsertSafetyStock(string xmlSafetyStock)
		/// \brief (Call stored procedure: sp_MA_InsertSafetyStock).

		/// \fn public virtual List<tbm_Shelf> InsertShelf(string xmlShelf)
		/// \brief (Call stored procedure: sp_MA_InsertShelf).

		/// \fn public virtual List<tbm_Site> InsertSite(string siteCode, string custCode, string siteNo, string siteNameEN, string siteNameLC, string sECOMContactPerson, string personInCharge, string phoneNo, string buildingUsageCode, string addressEN, string alleyEN, string roadEN, string subDistrictEN, string addressFullEN, string addressLC, string alleyLC, string roadLC, string subDistrictLC, string addressFullLC, string districtCode, string provinceCode, string zipCode, Nullable<System.DateTime> createDate, string createBy, Nullable<System.DateTime> updateDate, string updateBy)
		/// \brief (Call stored procedure: sp_MA_InsertSite).

		/// \fn public virtual List<doSubcontractor> InsertSubcontractor(string xmlSubcontractor)
		/// \brief (Call stored procedure: sp_MA_InsertSubcontractor).

		/// \fn public virtual List<Nullable<int>> IsUsedCustomer(string pcharCustCode)
		/// \brief (Call stored procedure: sp_MA_IsUsedCustomer).

		/// \fn public virtual List<Nullable<int>> IsUsedGroup(string pchvnstrGroupCode)
		/// \brief (Call stored procedure: sp_MA_IsUsedGroup).

		/// \fn public virtual List<dtManageCustomerInformation> ManageCustomerInformation(string pchvnstrCustCode, string c_CUST_STATUS_NEW_CUSTOMER)
		/// \brief (Call stored procedure: sp_MA_ManageCustomerInformation).

		/// \fn public virtual List<dtSiteRoadEN> SiteRoadEN(string pchvnstrAutoComplete)
		/// \brief (Call stored procedure: sp_MA_GetSiteRoadEN).

		/// \fn public virtual List<tbm_Belonging> UpdateBelonging(string xmlBelonging, Nullable<int> pintBelongingID)
		/// \brief (Call stored procedure: sp_MA_UpdateBelonging).

		/// \fn public virtual List<tbm_BillingClient> UpdateBillingClient(string xmlBillingClient)
		/// \brief (Call stored procedure: sp_MA_UpdateBillingClient).

		/// \fn public virtual List<tbm_Customer> UpdateCustomer(string custCode, string custStatus, Nullable<bool> importantFlag, string custNameEN, string custNameLC, string custFullNameEN, string custFullNameLC, string repPersonName, string contactPersonName, string sECOMContactPerson, string custTypeCode, string companyTypeCode, string financialMarketTypeCode, string businessTypeCode, string phoneNo, string faxNo, string iDNo, Nullable<bool> dummyIDFlag, string regionCode, string uRL, string memo, string alleyEN, string addressEN, string roadEN, string subDistrictEN, string addressFullEN, string addressLC, string alleyLC, string roadLC, string subDistrictLC, string addressFullLC, string districtCode, string provinceCode, string zipCode, Nullable<bool> deleteFlag, Nullable<System.DateTime> updateDate, string updateBy)
		/// \brief (Call stored procedure: sp_MA_UpdateCustomer).

		/// \fn public virtual int UpdateCustomerStatus(string c_CUST_STATUS_EXISTING_CUSTOMER, string c_CUST_STATUS_NEW_CUSTOMER, string pchnvstrCustCode, string updateBy, Nullable<System.DateTime> updateDate)
		/// \brief (Call stored procedure: sp_MA_UpdateCustomerStatus).

		/// \fn public virtual List<tbm_Employee> UpdateEmployee(string xmlEmployee)
		/// \brief (Call stored procedure: sp_MA_UpdateEmployee).

		/// \fn public virtual List<tbm_Group> UpdateGroup(string pchrGroupCode, string pchvnGroupNameEN, string pchnvGroupNameLC, string pchnvMemo, string pchnvGroupOffice, string pchnvGroupEmpNo, Nullable<bool> pbitDeleteFlag, Nullable<System.DateTime> pdtmUpdateDate, string pchnvUpdateBy)
		/// \brief (Call stored procedure: sp_MA_UpdateGroup).

		/// \fn public virtual List<tbm_Instrument> UpdateInstrument(string xmlInstrument)
		/// \brief (Call stored procedure: sp_MA_UpdateInstrument).

		/// \fn public virtual List<tbm_PermissionGroup> UpdatePermissionGroup(string permissionGroupCode, string permissionGroupName, Nullable<System.DateTime> updateDate, string updateBy)
		/// \brief (Call stored procedure: sp_MA_UpdatePermissionGroup).

		/// \fn public virtual List<tbm_PermissionIndividual> UpdatePermissionIndividual(string permissionGroupCode, string permissionIndividualCode, string permissionIndividualName, Nullable<System.DateTime> updateDate, string updateBy)
		/// \brief (Call stored procedure: sp_MA_UpdatePermissionIndividual).

		/// \fn public virtual List<tbm_SafetyStock> UpdateSafetyStock(string xmlSafetyStock)
		/// \brief (Call stored procedure: sp_MA_UpdateSafetyStock).

		/// \fn public virtual List<tbm_Shelf> UpdateShelf(string xmlShelf)
		/// \brief (Call stored procedure: sp_MA_UpdateShelf).

		/// \fn public virtual List<tbm_Site> UpdateSite(string siteCode, string custCode, string siteNo, string siteNameEN, string siteNameLC, string sECOMContactPerson, string personInCharge, string phoneNo, string buildingUsageCode, string addressEN, string alleyEN, string roadEN, string subDistrictEN, string addressFullEN, string addressLC, string alleyLC, string roadLC, string subDistrictLC, string addressFullLC, string districtCode, string provinceCode, string zipCode, Nullable<System.DateTime> updateDate, string updateBy)
		/// \brief (Call stored procedure: sp_MA_UpdateSite).

		/// \fn public virtual List<doSubcontractor> UpdateSubcontractor(string xmlSubcontractor)
		/// \brief (Call stored procedure: sp_MA_UpdateSubcontractor).


	}
}

