using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Contract.Handlers;
using SECOM_AJIS.DataEntity.Contract.MetaData;
using SECOM_AJIS.DataEntity.Installation;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class UserControlHandler : BizCTDataEntities, IUserControlHandler
    {
        /// <summary>
        /// Get rental contract basic information to show in user control
        /// </summary>
        /// <param name="strContrancCode"></param>
        /// <returns></returns>
        public doRentalContractBasicInformation GetRentalContactBasicInformationData(string strContractCode)
        {
            try
            {
                string installStatus = "";
                doRentalContractBasicInformation doRentalContract = new doRentalContractBasicInformation();
                doRentalContract.ContractCode = strContractCode;
                CommonUtil.CheckMandatoryFiled(doRentalContract);

                List<doRentalContractBasicInformation> doRentalContractBasicInformation = base.GetRentalContractBasicInformation(strContractCode);
                ICommonHandler common = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IInstallationHandler installhandler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                List<doMiscTypeCode> lMiscTypeCode = new List<doMiscTypeCode>();
                List<doMiscTypeCode> viewMiscTypeCode = new List<doMiscTypeCode>();

                doMiscTypeCode dMiscTypeCode = new doMiscTypeCode();

                installStatus = installhandler.GetInstallationStatus(strContractCode);

                dMiscTypeCode.FieldName = SECOM_AJIS.Common.Util.ConstantValue.MiscType.C_INSTALL_STATUS;
                //dMiscTypeCode.ValueCode = "99"; //I will set default to 00 because the table installation still not create.
                dMiscTypeCode.ValueCode = installStatus;


                lMiscTypeCode.Add(dMiscTypeCode);
                lMiscTypeCode = common.GetMiscTypeCodeList(lMiscTypeCode);

                viewMiscTypeCode = common.GetMiscTypeCodeList(lMiscTypeCode);

                if (doRentalContractBasicInformation.Count != 0)
                {
                    if (viewMiscTypeCode != null && viewMiscTypeCode.Count >0)
                    {
                        doRentalContractBasicInformation[0].InstallationStatusCode = installStatus;
                        doRentalContractBasicInformation[0].InstallationStatusName = viewMiscTypeCode[0].ValueDisplay;
                    }

                    //List<doRentalContractBasicInformation> list = doRentalContractBasicInformation;
                    //CommonUtil.MappingObjectLanguage<doRentalContractBasicInformation>(list);
                    //doRentalContractBasicInformation[0].OperationOfficeName = list[0].OperationOfficeName;
                    CommonUtil.MappingObjectLanguage<doRentalContractBasicInformation>(doRentalContractBasicInformation);

                    MiscTypeMappingList miscList = new MiscTypeMappingList();
                    miscList.AddMiscType(doRentalContractBasicInformation[0]);

                    ICommonHandler comHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    comHandler.MiscTypeMappingList(miscList);

                    return doRentalContractBasicInformation[0];
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
        /// Get rental security basic information to show in user control
        /// </summary>
        /// <param name="strContrancCode"></param>
        /// <param name="occCode"></param>
        /// <returns></returns>
        public doRentalSecurityBasicInformation GetRentalSecurityBasicInformationData(string strContractCode,string occ)
        {
            try
            {
                doRentalSecurityBasicInformation  doRentalContract = new doRentalSecurityBasicInformation();
                doRentalContract.ContractCode = strContractCode;
                doRentalContract.OCC = occ;
                CommonUtil.CheckMandatoryFiled(doRentalContract);

                return base.GetRentalSecurityBasicInformation(strContractCode,occ)[0];
            }
            catch (Exception)
            {                
                throw;
            }
        }

        /// <summary>
        /// Get sale contract basic information to show in user control
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="occCode"></param>
        /// <returns></returns>
        public List<doSaleContractBasicInformation> GetSaleContractBasicInformationData(string contractCode, string occCode)
        {
            try
            {
                List<doSaleContractBasicInformation> doSaleBasicList = base.GetSaleContractBasicInformation(contractCode, FlagType.C_FLAG_ON, occCode);
                if (doSaleBasicList == null)
                {
                    doSaleBasicList = new List<doSaleContractBasicInformation>();
                }
                else
                {
                    //Add installation status
                    IInstallationHandler installHandler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                    string strInstallationStatusCode = installHandler.GetInstallationStatus(contractCode);
                    foreach(doSaleContractBasicInformation data in doSaleBasicList)
                    {
                        data.InstallationStatusCode = strInstallationStatusCode;
                    }

                    CommonUtil.MappingObjectLanguage<doSaleContractBasicInformation>(doSaleBasicList);

                    MiscTypeMappingList miscList = new MiscTypeMappingList();
                    miscList.AddMiscType(doSaleBasicList.ToArray()); 

                    ICommonHandler comHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    comHandler.MiscTypeMappingList(miscList);
                }                    

                return doSaleBasicList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
