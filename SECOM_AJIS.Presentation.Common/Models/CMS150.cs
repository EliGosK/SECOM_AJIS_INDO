using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.CustomAttribute;
using CSI.WindsorHelper;

namespace SECOM_AJIS.Presentation.Common.Models
{
    /// <summary>
    /// Parameter for screen CMS150.
    /// </summary>
    public class CMS150_ScreenParameter : ScreenSearchParameter
    {
        [KeepSession]
        [NotNullOrEmpty]
        public string ContractCode { get; set; }
        [KeepSession]
        public string OCC { get; set; }
        [KeepSession]
        public string ContractTargetCode { get; set; }
        [KeepSession]
        public string PurchaserCustCode { get; set; }
        [KeepSession]
        public string RealCustomerCode { get; set; }
        [KeepSession]
        public string SiteCode { get; set; }

        [KeepSession]
        [NotNullOrEmpty]
        public string ServiceTypeCode { get; set; }
        [KeepSession]
        public string MATargetContractCode { get; set; }
        [KeepSession]
        public string ProductCode { get; set; }
        [KeepSession]
        public dsRentalBasicForHistDigestView dsRentBasicForHDView { get; set; }
        [KeepSession]
        public dsSaleBasicForHistDigestView dsSaleBasicForHDView { get; set; }
    }
    public class dsSaleBasicForHistDigestView
    {
        public List<dtTbt_SaleBasicForView> dtTbt_SaleBasicForView
        { get; set; }
        public List<doMiscTypeCode> dtMiscellaneousType
        { get; set; }
        public List<dtSaleHistoryDigest> dtSaleHistoryDigestList
        { get; set; }
        public List<View_dtSaleHistoryDigestList> View_dtSaleHistoryDigestList
        { get; set; }
    }
    public class dsRentalBasicForHistDigestView
    {
        public List<dtTbt_RentalContractBasicForView> dtTbt_RentalContractBasicForView
        { get; set; }
        public List<doMiscTypeCode> dtMiscellaneousType
        { get; set; }
        public List<dtRentalHistoryDigest> dtRentalHistoryDigest { get; set; }
        public List<View_dtRentalHistoryDigest> View_dtRentalHistoryDigest { get; set; }
    }
    public class View_dtSaleHistoryDigestList : dtSaleHistoryDigest
    {
        public string ChangeTypeName { get; set; }
        public string IncidentARTypeName { get; set; }
        public string ChangeType_View
        {
            get
            {

                if (this.ContractType == SECOM_AJIS.Common.Util.ConstantValue.ContractType.C_CONTRACT_TYPE_CONTACT)
                    return CommonUtil.TextCodeName( this.ChangeType.Trim() , this.ChangeTypeName );
                else if (this.ContractType == SECOM_AJIS.Common.Util.ConstantValue.ContractType.C_CONTRACT_TYPE_AR || this.ContractType == SECOM_AJIS.Common.Util.ConstantValue.ContractType.C_CONTRACT_TYPE_INCIDENT)
                    return CommonUtil.TextCodeName( this.IncidentARType.Trim()  , this.IncidentARTypeName );
                else
                    return "Contract type not allow.";
            }

        }
        public string ChangeIncidentARtype
        {
            get
            {
                if (!CommonUtil.IsNullOrEmpty(this.ContractType))
                    if (this.ContractType == SECOM_AJIS.Common.Util.ConstantValue.ContractType.C_CONTRACT_TYPE_CONTACT)
                        return this.ChangeType;
                    else if (this.ContractType == SECOM_AJIS.Common.Util.ConstantValue.ContractType.C_CONTRACT_TYPE_INCIDENT
                        || this.ContractType == SECOM_AJIS.Common.Util.ConstantValue.ContractType.C_CONTRACT_TYPE_AR)
                        return this.IncidentARType;

                return null;
            }
        }
        public string DueDate_ApproveNo
        {
            get
            {
                string DueDate= "(1) " + String.Format("{0:dd-MMM-yyyy}", this.OperationDueDate) +
                    "<br>(2) " + this.ApproveNo;
                return DueDate;
            }
        }
        public string ChangeType_ApproveNo
        {
            get
            {
                return "(1) " + this.ChangeType_View +
                    "<br>(2) " + this.ApproveNo;
            }
        }

        public string txtSalePrice
        {
            get
            {
                return "(1) " + this.TextTransferSalePrice +
                    "<br>(2) " + this.TextTransferOrderInstallFee;
            }
        }
    }
    public class View_dtRentalHistoryDigest : dtRentalHistoryDigest
    {
        public string ChangeIncidentARtype
        {
            get
            {
                if (!CommonUtil.IsNullOrEmpty(this.ContractType))
                    if (this.ContractType == SECOM_AJIS.Common.Util.ConstantValue.ContractType.C_CONTRACT_TYPE_CONTACT)
                        return this.ChangeType;
                    else if (this.ContractType == SECOM_AJIS.Common.Util.ConstantValue.ContractType.C_CONTRACT_TYPE_INCIDENT
                        || this.ContractType == SECOM_AJIS.Common.Util.ConstantValue.ContractType.C_CONTRACT_TYPE_AR)
                        return this.IncidentARType;

                return null;
            }
        }
        public string ChangeTypeName { get; set; }
        public string IncidentARTypeName { get; set; }
        public string DocAuditResultName { get; set; }
        public string DocumentName { get; set; }
        public string DocumentName_View
        {
            get
            {
                if (CommonUtil.IsNullOrEmpty(this.DocumentName))
                    return "-";
                else
                    return this.DocumentName;
            }
        }
        public string ChangeType_View
        {
            get
            {

                if (this.ContractType == SECOM_AJIS.Common.Util.ConstantValue.ContractType.C_CONTRACT_TYPE_CONTACT)
                    return CommonUtil.TextCodeName( this.ChangeType , this.ChangeTypeName );
                else if (this.ContractType == SECOM_AJIS.Common.Util.ConstantValue.ContractType.C_CONTRACT_TYPE_AR || this.ContractType == SECOM_AJIS.Common.Util.ConstantValue.ContractType.C_CONTRACT_TYPE_INCIDENT)
                    return CommonUtil.TextCodeName( this.IncidentARType  ,this.IncidentARTypeName );
                else
                    return "Contract type not allow.";
            }

        }
        public string FEE_View
        {
            get
            {
                ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> tmpCurrencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                        {
                                                            new doMiscTypeCode()
                                                            {
                                                                FieldName = MiscType.C_CURRENCT,
                                                                ValueCode = "%"
                                                            }
                                                        }).ToList();
                this.Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                string ContractFee = null;
                if (CommonUtil.IsNullOrEmpty(this.ContractFee) || this.ContractFee == 0)
                    ContractFee = "0.00";
                else
                    ContractFee = Convert.ToDecimal(this.ContractFee).ToString("#,####.00");

                string OrderFee = null;
                if (CommonUtil.IsNullOrEmpty(this.OrderInstallFee) || this.OrderInstallFee == 0)
                    OrderFee = "0.00";
                else
                    OrderFee = Convert.ToDecimal(this.OrderInstallFee).ToString("#,####.00");

                return "(1) " + TextTransferAmountContractFee +
                    "<br>(2) " + TextTransferAmountOrderInstallFee;

            }
        }
        public string DocAudit_View
        {
            get
            {
                if (!CommonUtil.IsNullOrEmpty(this.DocAuditResult) && this.DocAuditResult != "-")
                    return this.DocAuditResult.Trim() + ":" + this.DocAuditResultName;
                else
                    return "-";
            }
        }
        public string DueDate_ApproveNo
        {
            get
            {
                return "(1) " + String.Format("{0:dd-MMM-yyyy}", this.OperationDueDate) +
                    "<br>(2) " + this.ApproveNo;
            }
        }
        public string ChangeType_ApproveNo
        {
            get
            {
                return "(1) " + this.ChangeType_View +
                    "<br>(2) " + this.ApproveNo;
            }
        }

        //public string ContractCode_Short { get { return new CommonUtil().ConvertContractCode(this.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT); } }
        //public string ContractTargetCustCode_Short { get { return new CommonUtil().ConvertContractCode(this.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT); } }
        //public string RealCustomerCustCode_Short { get { return new CommonUtil().ConvertContractCode(this.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT); } }
        //public string SiteCode_Short { get { return new CommonUtil().ConvertSiteCode(this., CommonUtil.CONVERT_TYPE.TO_SHORT); } }

    }
}
