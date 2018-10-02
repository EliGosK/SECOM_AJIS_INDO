using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.DataEntity.Master;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Models;
namespace SECOM_AJIS.DataEntity.Common
{
    class TransDataHandler : BizCMDataEntities, ITransDataHandler
    {
        /// <summary>
        /// Refresh user data of dsTrans object
        /// </summary>
        /// <param name="dsTrans"></param>
        /// <param name="EmpNo"></param>
        public void RefreshUserData(dsTransDataModel dsTrans, string EmpNo)
        {
            try
            {
                dsTrans.dtUserData = null;

                IEmployeeMasterHandler handEmp = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                List<dtEmployeeData> dtEmp = handEmp.GetUserData(EmpNo);
                if (dtEmp.Count > 0)
                    dsTrans.dtUserData = CommonUtil.CloneObject<dtEmployeeData, UserDataDo>(dtEmp[0]);

            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        ///  Refresh office data of dsTrans object
        /// </summary>
        /// <param name="dsTrans"></param>
        public void RefreshOfficeData(dsTransDataModel dsTrans)
        {
            try
            {
                #region Belonging User

                dsTrans.dtUserBelongingData = new List<UserBelongingData>();
                
                IEmployeeMasterHandler handEmp = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;

                List<dtUserBelonging> dtUserBelong = handEmp.getBelongingByEmpNo(dsTrans.dtUserData.EmpNo);
                foreach (dtUserBelonging usrb in dtUserBelong)
                {
                    UserBelongingData UsrBelong = new UserBelongingData(
                        usrb.OfficeCode, 
                        usrb.DepartmentCode, 
                        usrb.PositionCode);

                    dsTrans.dtUserBelongingData.Add(UsrBelong);
                }

                #endregion
                #region Office

                dsTrans.dtOfficeData = new List<OfficeDataDo>();

                IOfficeMasterHandler handOffice = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
                
                List<dtAuthorizeOffice> dtAuthorizedOffice = handOffice.GetAuthorizeOffice(CommonUtil.ConvertToXml_Store<dtUserBelonging>(dtUserBelong));
                dsTrans.dtOfficeData = CommonUtil.ConvertObjectbyLanguage<dtAuthorizeOffice, OfficeDataDo>(dtAuthorizedOffice, "OfficeName");

                #endregion
            }
            catch (Exception)
            {
                throw;
            }

        }
        /// <summary>
        ///  Refresh permission data of dsTrans object
        /// </summary>
        /// <param name="dsTrans"></param>
        public void RefreshPermissionData(dsTransDataModel dsTrans)
        {
            try
            {
                dsTrans.dtUserPermissionData = new Dictionary<string, UserPermissionDataDo>();

                string xmlDtBelonging = CommonUtil.ConvertToXml_Store<UserBelongingData>(dsTrans.dtUserBelongingData);
                List<dtUserPermission> dtUserPermiss = base.RefreshPermissionData(dsTrans.dtUserData.EmpNo, xmlDtBelonging);
                foreach (dtUserPermission i in dtUserPermiss)
                {
                    string key = dsTrans.GenerateKey(i.ObjectID, i.FuncionID.ToString());
                    dsTrans.dtUserPermissionData[key] = new UserPermissionDataDo(i.ObjectID, (int)i.FuncionID, i.ObjectTypeCode);
                }

                if (dsTrans.dtUserPermissionData.Count <= 0)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0033);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
