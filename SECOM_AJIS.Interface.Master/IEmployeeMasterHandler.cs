using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Master
{
    public interface IEmployeeMasterHandler
    {
        #region Get Methods

        List<dtEmployeeOffice> GetEmployeeOffice(string strEmpNo, bool? blnMainDepartmentFlag);
        List<tbm_Employee> GetEmployeeList(List<tbm_Employee> emp);
        void EmployeeListMapping(EmployeeMappingList empLst);
        void ActiveEmployeeListMapping(EmployeeMappingList empLst);
        void CheckActiveEmployeeExist(EmployeeExistList empLst, bool clearData = false);
        List<dtEmployee> GetEmployee(string pchvEmpNo, string pchrnEmpFirstNameEN, Nullable<bool> pbitMainDepartmentFlag);
        List<dtEmployeeDetail> GetEmployeeDetail(string pchvEmpNo);
        List<tbm_Position> GetTbm_Position();
        List<string> GetPositionCodeAtMaxPositionLevel();
        List<dtBelonging> GetBelonging(string pchvOfficeCode, string pchrDepartmentCode, string pchrPositionCode, string pchvEmpNo);
        List<dtBelongingEmpNo> GetBelongingEmpNoByOffice(string officeCode);
        List<Nullable<System.DateTime>> GetBelongingUpdateDate(Nullable<int> pintBelongingID);
        List<Nullable<System.DateTime>> GetEmployeeUpdateDate(string pchvEmpNo);
        List<dtUserBelonging> getBelongingByEmpNo(string pchvEmpNo);
        List<dtEmployeeData> GetUserData(string pchvEmpNo);
        List<doActiveEmployeeList> GetActiveEmployeeList(List<tbm_Employee> emp);
        List<tbm_Employee> GetActiveEmployee(string strEmpNo);
        List<Nullable<bool>> CheckExistBelonging(string pchvOfficeCode, string pchrDepartmentCode, string pchvEmpNo, Nullable<int> pintBelongingID);
        List<Nullable<bool>> CheckExistEmployee(string pchvEmpNo);
        List<Nullable<bool>> CheckExistMainDepartmentFlag(string pchvEmpNo, Nullable<int> pintBelongingID);
        bool CheckExistActiveEmployee(string pchvEmpNo);
        List<dtEmpNo> GetEmployeeNameByEmpNo(string empNo);
        List<tbm_Employee> GetTbm_Employee(string strEmpNo);
        void checkBelongingUpdateDate(List<View_tbm_Belonging> checkList);
        List<dtBelongingOffice> GetBelongingOfficeList(string pEmpNo);
        List<dtGetEmailAddress> GetEmailAddress(string pcharEmpFirstNameEN, string pcharEmailAddress, string pcharOfficeCode, string pcharDepartmentCode);
        List<dtEmailAddressForIncident> GetEmailAddressForIncident(Nullable<bool> c_FLAG_ON);
        List<doBelongingData> GetMainBelongingByEmpNo(string strEmpNo);
        List<Nullable<bool>> CheckExistMainDepartmentPersonInCharge(string officeCode, string departmentCode, Nullable<int> belongingID);
        List<dtEmployeeBelonging> GetBelongingEmpList(string strOfficeCode, string strDepartmentCode, bool? bChiefFlag, bool? bApproveFlag, bool? bCorrespondentFlag);
        List<dtDepartment> GetBelongingDepartmentList(string strOfficeCode, string strDepartmentCode);
        List<dtOfficeChief> GetOfficeChiefList(string strOfficeCode);
        #endregion
        #region Insert Methods

        List<tbm_Belonging> InsertBelonging(string xmlBelonging);
        List<tbm_Employee> InsertEmployee(string xmlEmployee);
        
        #endregion
        #region Update Methods

        List<tbm_Employee> UpdateEmployee(tbm_Employee employee);
        List<tbm_Employee> DeleteEmployee(tbm_Employee employee);
        List<tbm_Belonging> DeleteBelonging(Nullable<int> pintBelongingID);
        List<tbm_Belonging> UpdateBelonging(string xmlBelonging, Nullable<int> pintBelongingID);
        List<tbm_Belonging> DeleteAllBelonging(string pEmpNo);

        #endregion

    }
}
