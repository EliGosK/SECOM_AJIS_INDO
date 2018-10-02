using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Installation.MetaData;

namespace SECOM_AJIS.DataEntity.Installation
{
    [MetadataType(typeof(dtInstallationHistoryForView_MetaData))]
    public partial class dtInstallationHistoryForView
    {
        CommonUtil cm = new CommonUtil();
        public string InstallationStatusName { get; set; }

        [LanguageMapping]
        public string InstallationTypeName { get; set; }

        [LanguageMapping]
        public string CauseReasonName { get; set; }

        public string SalesmanEmpName1 { get; set; }

        public string SalesmanEmpName2 { get; set; }

        public string InstallFeeBillingTypeName { get; set; }

     

        public string InstallationFeeBillingFlag
        {
            get {
                return CommonUtil.TextCodeName(InstallFeeBillingType, InstallFeeBillingTypeName);
            }
        }

        public string InstallationTypeCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(InstallationType, InstallationTypeName);
            }
        }

        public string SlipIssueDate_Text
        {
            get
            {
                return CommonUtil.TextDate(SlipIssueDate);
            }
        }

        public string StockOutDate_Text
        {
            get
            {
                return CommonUtil.TextDate(StockOutDate);
            }
        }

        public string InstallationCompleteDate_Text
        {
            get
            {
                return CommonUtil.TextDate(InstallationCompleteDate);
            }
        }

        public string ReturnReceiveDate_Text
        {
            get
            {
                return CommonUtil.TextDate(ReturnReceiveDate);
            }
        }

        public string NormalContractFeeNumeric
        {
            get
            {
                return CommonUtil.TextNumeric(NormalContractFee);

            }
        }

        public string NormalInstallFeeNumeric
        {
            get
            {
                return CommonUtil.TextNumeric(NormalInstallFee);

            }
        }
        public string BillingInstallFeeNumeric
        {
            get
            {
                return CommonUtil.TextNumeric(BillingInstallFee);

            }
        }
        public string SecomPaymentNumeric
        {
            get
            {
                return CommonUtil.TextNumeric(SecomPayment);

            }
        }
        public string SecomRevenueNumeric
        {
            get
            {
                return CommonUtil.TextNumeric(SecomRevenue);

            }
        }
    }
}

namespace SECOM_AJIS.DataEntity.Installation.MetaData
{
    public class dtInstallationHistoryForView_MetaData
    {
        [InstallFeeBillingTypeMappingAttribute("InstallFeeBillingTypeName")]
        public string InstallFeeBillingType { get; set; }


        [EmployeeMapping("SalesmanEmpName1")]
        public string SalesmanEmpNo1 { get; set; }

        [EmployeeMapping("SalesmanEmpName2")]
        public string SalesmanEmpNo2 { get; set; }

        [InstallationStatusMappingAttribute("InstallationStatusName")]
        public string InstallationStatus { get; set; }
    }
}
