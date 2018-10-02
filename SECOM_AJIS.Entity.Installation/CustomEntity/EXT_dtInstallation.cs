using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using ConstantValue = SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Installation.MetaData;

namespace SECOM_AJIS.DataEntity.Installation
{
    /// <summary>
    /// Do Of installation
    /// </summary>
    [MetadataType(typeof(dtInstallation_MetaData))]
    public partial class dtInstallation
    {
        CommonUtil cm = new CommonUtil();
        public string InstallationStatusName { get; set; }

        [LanguageMapping]
        public string InstallationTypeName { get; set; }

        [LanguageMapping]
        public string contractOfficeName { get; set; }

        [LanguageMapping]
        public string SubContractorName { get; set; }

        public string SlipStatusName { get; set; }

        public string ManagementStatusName { get; set; }

        public string InstallationStatusMapping
        {
            get
            {
                return CommonUtil.TextCodeName(InstallationStatus, InstallationStatusName);
            }
        }
        public string SlipStatusMapping
        {
            get
            {
                return CommonUtil.TextCodeName(SlipStatus, SlipStatusName);
            }
        }
        public string ContractCode_Short
        {
            get
            {
                return cm.ConvertContractCode(ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string SiteCode_Short
        {
            get
            {
                return cm.ConvertSiteCode(SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }
        public string InstallationName
        {
            get
            {
                return string.Format("(1) {0}<br/>(2) {1}<br/>(3) {2}<br/>(4) {3}",
                String.IsNullOrEmpty(this.CustFullNameEN) ? "-" : this.CustFullNameEN,
                String.IsNullOrEmpty(this.CustFullNameLC) ? "-" : this.CustFullNameLC,
                String.IsNullOrEmpty(this.SiteNameEN) ? "-" : this.SiteNameEN,
                String.IsNullOrEmpty(this.SiteNameLC) ? "-" : this.SiteNameLC);
            }
        }
        public string SubContractorName_Text
        {
            get
            {
                return string.Format("(1) {0}<br/>(2) {1}",
                String.IsNullOrEmpty(this.SubContractorNameEN) ? "-" : this.SubContractorNameEN,
                String.IsNullOrEmpty(this.SubContractorNameLC) ? "-" : this.SubContractorNameLC);
            }
        }

        [LanguageMapping]
        public string OperationOfficeName { get; set; }

        public string ToJson {
            get {
                return CommonUtil.CreateJsonString(this);
            }
        }

        public string SalesmanEmpName1 { get; set; }

        public string SalesmanEmpName2 { get; set; }

        public string InstallationTypeName_Text
        {
            get
            {
                return (CommonUtil.IsNullOrEmpty(InstallationTypeName)) ? "-" : InstallationTypeName;
                
            }
        }
       
        public string InstallationStatusMapping_Text
        {
            get
            {
                return (CommonUtil.IsNullOrEmpty(InstallationStatusMapping)) ? "-" : InstallationStatusMapping;

            }
        }

        public string InstallationName_Text
        {
            get
            {
                return (CommonUtil.IsNullOrEmpty(InstallationName)) ? "-" : InstallationName;

            }
        }

        public string ContractCode_Short_Text
        {
            get
            {
                return (CommonUtil.IsNullOrEmpty(ContractCode_Short)) ? "-" : ContractCode_Short;

            }
        }

        public string SlipType_Text
        {
            get
            {
                return (CommonUtil.IsNullOrEmpty(SlipType)) ? "-" : SlipType;

            }
        }

        public string SlipStatusMapping_Text
        {
            get
            {
                return (CommonUtil.IsNullOrEmpty(SlipStatusMapping)) ? "-" : SlipStatusMapping;

            }
        }

        public string ManagementStatusName_Text
        {
            get
            {
                return (CommonUtil.IsNullOrEmpty(ManagementStatusName)) ? "-" : ManagementStatusName;

            }
        }



        public string CMS180_RegisterTextButton
        {
            get
            {
                string registerTextButton = string.Empty;
                if (ConstantValue.InstallationStatus.C_INSTALL_STATUS_INSTALL_NOT_REQUESTED == InstallationStatus)
                {
                    registerTextButton = ConstantValue.InstallationStatus.C_INSTALL_STATUS_REQUEST;
                }
                else if (ConstantValue.InstallationStatus.C_INSTALL_STATUS_INSTALL_REQUESTED_PO_NOT_REGISTERED == InstallationStatus)
                {
                    registerTextButton = ConstantValue.InstallationStatus.C_INSTALL_STATUS_PO;
                }
                else if (ConstantValue.InstallationStatus.C_INSTALL_STATUS_INSTALL_REQUESTED_PO_REGISTERED == InstallationStatus)
                {
                    if (string.IsNullOrEmpty(SlipStatus) == true)
                    {
                        registerTextButton = ConstantValue.InstallationStatus.C_INSTALL_STATUS_SLIP;
                    }
                    else
                    {
                        registerTextButton = ConstantValue.InstallationStatus.C_INSTALL_STATUS_COMPLETE;
                    }
                }
                else if (ConstantValue.InstallationStatus.C_INSTALL_STATUS_COMPLETED == InstallationStatus)
                {
                    registerTextButton = ConstantValue.InstallationStatus.C_INSTALL_STATUS_REGISTER;
                }
                else if (ConstantValue.InstallationStatus.C_INSTALL_STATUS_INSTALL_CANCELLED == InstallationStatus)
                {
                    registerTextButton = ConstantValue.InstallationStatus.C_INSTALL_STATUS_REGISTER;
                }
                else if (ContractStatus == ConstantValue.ContractStatus.C_CONTRACT_STATUS_CANCEL)
                {
                    if (ConstantValue.InstallationStatus.C_INSTALL_STATUS_COMPLETED != InstallationStatus
                            && ConstantValue.InstallationStatus.C_INSTALL_STATUS_INSTALL_CANCELLED != InstallationStatus
                            && ConstantValue.InstallationStatus.C_INSTALL_STATUS_INSTALL_NOT_REGISTERED != InstallationStatus
                            && ConstantValue.InstallationStatus.C_INSTALL_STATUS_NO_INSTALLATION != InstallationStatus
                            && ConstantValue.RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_ALL != InstallationStatus
                            && ConstantValue.SaleInstallationType.C_SALE_INSTALL_TYPE_REMOVE_ALL != InstallationStatus
                            )
                    {
                        registerTextButton = ConstantValue.InstallationStatus.C_INSTALL_STATUS_CANCEL;
                    }
                    else
                    {
                        registerTextButton = "-";
                    }
                }
                else
                {
                    registerTextButton = "-";
                }

                return registerTextButton;
            }
        }

        public bool CMS180_RegisterFlagButton
        {
            get
            {
                bool flag = true;

                if (ConstantValue.InstallationStatus.C_INSTALL_STATUS_INSTALL_REQUESTED_PO_REGISTERED == InstallationStatus)
                {
                    if (string.IsNullOrEmpty(this.SlipStatus) == false)
                    {
                        if (this.SlipStatus != ConstantValue.SlipStatus.C_SLIP_STATUS_NO_NEED_TO_STOCK_OUT && SlipStatus != ConstantValue.SlipStatus.C_SLIP_STATUS_STOCK_OUT)
                        {
                            flag = false;
                        }
                    }
                }
                else if (ConstantValue.InstallationStatus.C_INSTALL_STATUS_COMPLETED == InstallationStatus)
                {
                    flag = false;
                }
                else if (ConstantValue.InstallationStatus.C_INSTALL_STATUS_INSTALL_CANCELLED == InstallationStatus)
                {
                    flag = false;
                }


                return flag;
            }
        }

        public bool CMS180_ViewDetail
        {
            get
            {
                if (string.IsNullOrEmpty(SlipNo) == true)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}

namespace SECOM_AJIS.DataEntity.Installation.MetaData
{
    /// <summary>
    /// Do Of installation meta data
    /// </summary>
    public class dtInstallation_MetaData
    {
        [InstallationStatusMappingAttribute("InstallationStatusName")]
        public string InstallationStatus { get; set; }

        [SubContractorMappingAttribute("SubContractorName")]
        public string SubContractor { get; set; }

        [SlipStatusMappingAttribute("SlipStatusName")]
        public string SlipStatus { get; set; }

        //[InstallationTypeMappingAttribute("InstallationTypeName")]
        //public string InstallationType { get; set; }

        [ManagementStatusMappingAttribute("ManagementStatusName")]
        public string ManagementStatus { get; set; }

        [EmployeeMapping("SalesmanEmpName1")]
        public string SalesmanEmpNo1 { get; set; }

        [EmployeeMapping("SalesmanEmpName2")]
        public string SalesmanEmpNo2 { get; set; }

        


    }
}
