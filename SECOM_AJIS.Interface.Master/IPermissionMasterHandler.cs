using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Master
{
    public interface IPermissionMasterHandler
    {
        List<dtObjectFunction> GetObjectFunction(Nullable<int> moduleID);
        List<dtFunction> GetFunction(string permissionGroupCode, string permissionIndividualCode);
        List<dtEmpNo> GetEmpNo(string permissionGroupCode, string permissionIndividualCode);
        List<Nullable<bool>> CheckExistEmpNo(string officeCode, string departmentCode, string positionCode, string empNo);
        List<Nullable<bool>> CheckExistPermission(string officeCode, string departmentCode, string positionCode);
        List<dtPermissionHeader> GetPermission(doPermission condition);

        /// <summary>
        /// Add new permission type office.<br />
        /// - Generate permission group code.<br />
        /// - Insert permission group.<br />
        /// - Write transaction log.<br />
        /// - Insert permission detail.<br />
        /// - Write transaction log.
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        List<tbm_PermissionGroup> AddPermissionTypeOffice(doPermission permission);

        /// <summary>
        /// Add new permission type individual.<br />
        /// - Generate permission individual code.<br />
        /// - Insert permission individual.<br />
        /// - Write transaction log.<br />
        /// - Insert permission individual detail(employee number for apply permission).<br />
        /// - Write transaction log.<br />
        /// - Insert permission detail.<br />
        /// - Write transaction log.
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        List<tbm_PermissionIndividual> AddPermissionTypeIndividual(doPermission permission);

        /// <summary>
        /// Edit permission type office.<br />
        /// - Check permission group's update date.<br />
        /// - Update permission group.<br />
        /// - Write transaction log.<br />
        /// - Delete permission detail.<br />
        /// - Write transaction log.<br />
        /// - Insert permission detail.<br />
        /// - Write transaction log.
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        List<tbm_PermissionGroup> EditPermissionTypeOffice(doPermission permission);

        /// <summary>
        /// Edit permission type individual.<br />
        /// - Check permission individual's update date.<br />
        /// - Update permission individual.<br />
        /// - Write transaction log.<br />
        /// - Delete permission individual detail.<br />
        /// - Wtite transaction log.<br />
        /// - Delete permission detail.<br />
        /// - Write transaction log.<br />
        /// - Insert permission detail.<br />
        /// - Write transaction log.
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        List<tbm_PermissionIndividual> EditPermissionTypeIndividual(doPermission permission);

        /// <summary>
        /// Delete permission type office.<br />
        /// - Check permission group's update date.<br />
        /// - Delete permission individual detail.<br />
        /// - Write transaction log.<br />
        /// - Delete permission individual.<br />
        /// - Write transaction log.<br />
        /// - Delete permission detail.<br />
        /// - Write transaction log.<br />
        /// - Delete permission group.<br />
        /// - Write transaction log.
        /// </summary>
        /// <param name="permissionGroupCode"></param>
        /// <param name="updateDate"></param>
        /// <returns></returns>
        List<tbm_PermissionGroup> DeletePermissionTypeOffice(string permissionGroupCode, DateTime updateDate);


        /// <summary>
        /// Delete permission type individual.<br />
        /// - Check permission individual's update date.<br />
        /// - Delete permission individual detail.<br />
        /// - Write transaction log.<br />
        /// - Delete permission individual.<br />
        /// - Write transaction log.<br />
        /// - Delete permission detail.<br />
        /// - Write transaction log.
        /// </summary>
        /// <param name="permissionGroupCode"></param>
        /// <param name="permissionIndividualCode"></param>
        /// <param name="updateDate"></param>
        /// <returns></returns>
        List<tbm_PermissionDetail> DeletePermissionTypeIndividual(string permissionGroupCode, string permissionIndividualCode, DateTime updateDate);

        List<tbm_PermissionDetail> CopyPermissionFromGroupToIndividual(string permissionGroupCode, string permissionIndividualCode, Nullable<System.DateTime> createDate, string createBy);
    }
}
