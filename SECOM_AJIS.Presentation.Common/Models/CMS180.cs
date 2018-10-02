using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Installation;

using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.Presentation.Common.Models
{
    /// <summary>
    /// Screen parameter of CMS180
    /// </summary>
    public class CMS180_ScreenParameter : ScreenSearchParameter
    {
        [KeepSession]
        public string ContractCode { set; get; }
        [KeepSession]
        public string InstallationSlipNo { set; get; }
        [KeepSession]
        public string ServiceTypeCode { set; get; }

        public List<CMS180_SearchResultGridData> doSearchResultGridData { get; set; }
        public List<CMS180_SearchResultGridData> doResultCSVData { get; set; } //Add by Jutarat A. on 25062013
    }

    /// <summary>
    /// Inheritance do of installation for csv mapping
    /// </summary>
    public class CMS180_SearchResultGridData : dtInstallation
    {
        CommonUtil cm = new CommonUtil();

        [CSVMapping(Controller = MessageUtil.MODULE_COMMON,
                    Screen = "CMS180",
                    LabelName = "headerContractCode",
                    SequenceNo = 1)]
        public string ContractCodeCSV
        {
            get
            {
                return ContractCode_Short;
            }
        }

        [CSVMapping(Controller = MessageUtil.MODULE_COMMON,
                   Screen = "CMS180",
                   LabelName = "lblContractTargetPurchaserNameEnglish",
                   SequenceNo = 2)]
        public string CustFullNameENCSV
        {
            get
            {
                return CustFullNameEN;
            }
        }

        [CSVMapping(Controller = MessageUtil.MODULE_COMMON,
                   Screen = "CMS180",
                   LabelName = "lblContractTargetPurchaserNameLocal",
                   SequenceNo = 3)]
        public string CustFullNameLCCSV
        {
            get
            {
                return CustFullNameLC;
            }
        }

        [CSVMapping(Controller = MessageUtil.MODULE_COMMON,
                   Screen = "CMS180",
                   LabelName = "lblSiteCode",
                   SequenceNo = 4)]
        public string SiteCodeCSV
        {
            get
            {
                return SiteCode_Short;
            }
        }

        [CSVMapping(Controller = MessageUtil.MODULE_COMMON,
                   Screen = "CMS180",
                   LabelName = "lblPlanCode",
                   SequenceNo = 5)]
        public string PlanCodeCSV
        {
            get
            {
                return PlanCode;
            }
        }


        [CSVMapping(Controller = MessageUtil.MODULE_COMMON,
                   Screen = "CMS180",
                   LabelName = "lblOperationOffice",
                   SequenceNo = 6)]
        public string OperationOfficeCSV
        {
            get
            {
                return CommonUtil.TextCodeName(OperationOfficeCode,OperationOfficeName); 
            }
        }

        [CSVMapping(Controller = MessageUtil.MODULE_COMMON,
                  Screen = "CMS180",
                  LabelName = "lblContractOffice",
                  SequenceNo = 7)]
        public string ContractOfficeCSV
        {
            get
            {
                return CommonUtil.TextCodeName(ContractOfficeCode, contractOfficeName);
            }
        }

        [CSVMapping(Controller = MessageUtil.MODULE_COMMON,
                  Screen = "CMS180",
                  LabelName = "lblExpected",
                  SequenceNo = 8)]
        public string ExpectedOperationDateCSV
        {
            get
            {
                return CommonUtil.TextDate(ExpectedOperationDate);
            }
        }

        [CSVMapping(Controller = MessageUtil.MODULE_COMMON,
                 Screen = "CMS180",
                 LabelName = "lblsaleman1CSV",
                 SequenceNo = 9)]
        public string Saleman1CSV
        {
            get
            {
                 return CommonUtil.TextCodeName(SalesmanEmpNo1, SalesmanEmpName1);
            }
        }

        [CSVMapping(Controller = MessageUtil.MODULE_COMMON,
               Screen = "CMS180",
               LabelName = "lblsaleman2CSV",
               SequenceNo = 10)]
        public string Saleman2CSV
        {
            get
            {
                return CommonUtil.TextCodeName(SalesmanEmpNo2, SalesmanEmpName2);
            }
        }
        
    }
}
