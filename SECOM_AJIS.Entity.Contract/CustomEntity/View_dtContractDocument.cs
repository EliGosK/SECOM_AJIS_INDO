using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class View_dtContractDocument : dtContractDocument
    {
        
        public string DocumentTypeName { set; get; }


        // DocStatusName (3L)
        public string DocStatusName_Extra { set; get; }
        public string DocStatusName_ExtraEN { get { return CommonUtil.TextCodeName(base.DocStatus, base.DocStatusNameEN); } }
        public string DocStatusName_ExtraJP {  get { return CommonUtil.TextCodeName(base.DocStatus ,base.DocStatusNameJP); } }
        public string DocStatusName_ExtraLC {  get { return CommonUtil.TextCodeName(base.DocStatus , base.DocStatusNameLC); } }


        // DocAuditResultName (3L)
        public string DocAuditResultName_Extra { set; get; }
        public string DocAuditResultName_ExtraEN { get { return CommonUtil.TextCodeName(base.DocAuditResult , base.DocAuditResultNameEN ); } }
        public string DocAuditResultName_ExtraJP { get { return CommonUtil.TextCodeName(base.DocAuditResult, base.DocAuditResultNameJP); } }
        public string DocAuditResultName_ExtraLC { get { return CommonUtil.TextCodeName(base.DocAuditResult, base.DocAuditResultNameLC); } }

        // ContractOfficeName (3L)
        public string ContractOfficeName_Extra { set; get; }
        public string ContractOfficeName_ExtraEN { get { return CommonUtil.TextCodeName(base.ContractOfficeCode, base.ContractOfficeNameEN); } }
        public string ContractOfficeName_ExtraJP { get { return CommonUtil.TextCodeName(base.ContractOfficeCode, base.ContractOfficeNameJP); } }
        public string ContractOfficeName_ExtraLC { get { return CommonUtil.TextCodeName(base.ContractOfficeCode, base.ContractOfficeNameLC); } }


        public string DocumentName { set; get; }

        // OperationOfficeName (3L)
        public string OperationOfficeName_Extra { set; get; }
        public string OperationOfficeName_ExtraEN { get { return CommonUtil.TextCodeName(base.OperationOfficeCode , base.OperationOfficeNameEN); } }
        public string OperationOfficeName_ExtraJP { get { return CommonUtil.TextCodeName(base.OperationOfficeCode, base.OperationOfficeNameJP); } }
        public string OperationOfficeName_ExtraLC { get { return CommonUtil.TextCodeName(base.OperationOfficeCode, base.OperationOfficeNameLC); } }


        // Saleman1 (2L)
        public string Saleman1 { set; get; }
        public string Saleman1EN { get { return CommonUtil.TextCodeName(base.NegotiationStaffEmpNo, base.NegotiationStaffEmpFirstNameEN + " " + base.NegotiationStaffEmpLastNameEN); } }
        public string Saleman1LC { get { return CommonUtil.TextCodeName(base.NegotiationStaffEmpNo , base.NegotiationStaffEmpFirstNameLC + " " + base.NegotiationStaffEmpLastNameLC); } }

       
    }
}
