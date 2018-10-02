using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.DataEntity.Contract
{
    public partial class RPTContractReportDo
    {
        public string ContractCodeShort
        {
            get
            {
                CommonUtil c = new CommonUtil();
                return c.ConvertContractCode(ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string DocNoShort
        {
            get
            {
                CommonUtil c = new CommonUtil();
                return c.ConvertContractCode(DocNo, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        private string _imageSignatureFullPath = string.Empty;
        public string ImageSignatureFullPath
        {
            get
            {
                if (String.IsNullOrEmpty(ImageSignaturePath) == false)
                    _imageSignatureFullPath = ImageSignaturePath;

                return PathUtil.GetPathValue(PathUtil.PathName.ImageSignaturePath, _imageSignatureFullPath); //  ReportUtil.GetImageSignaturePath(_imageSignatureFullPath); //string.Format("{0}bin/{1}", CommonUtil.WebPath, ImageSignaturePath);
            }
        }

        public string PlanCodeShort
        {
            get
            {
                CommonUtil c = new CommonUtil();
                return c.ConvertQuotationTargetCode(this.PlanCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }


        public string ContractPeriodEN
        {
            get
            {
                int monthResult = ContractDurationMonth % 12;
                int yearResult = ContractDurationMonth / 12;

                if(monthResult == 0 && yearResult > 0)
                {
                    return yearResult + " year(s)";
                }
                else if (yearResult == 0 && monthResult > 0)
                {
                    return monthResult + "month(s)";
                }
                else if(monthResult > 0 && yearResult > 0)
                {
                    return yearResult + " year(s)" + monthResult + " month(s)";
                }
                else
                {
                    return null;
                }
            }
        }

        public string ContractPeriodLC
        {
            get
            {
                int monthResult = ContractDurationMonth % 12;
                int yearResult = ContractDurationMonth / 12;

                if(monthResult == 0 && yearResult > 0)
                { 
                    return yearResult + " tahun";
                }
                else if (yearResult == 0 && monthResult > 0)
                {
                    return monthResult + " bulan";
                }
                else if(monthResult > 0 && yearResult > 0)
                {
                    return yearResult + " tahun" + monthResult + " bulan";
                }
                else
                {
                    return null;
                }
            }
        }
        public string ContractRenewalEN
        {
            get
            {
                int monthResult = AutoRenewMonth % 12;
                int yearResult = AutoRenewMonth / 12;

                if (monthResult == 0 && yearResult > 0)
                {
                    return yearResult + " year(s)";
                }
                else if (yearResult == 0 && monthResult > 0)
                {
                    return monthResult + "month(s)";
                }
                else if (monthResult > 0 && yearResult > 0)
                {
                    return yearResult + " year(s)" + monthResult + " month(s)";
                }
                else
                {
                    return null;
                }
            }
        }
        
        public string ContractRenewalLC
        {
            get
            {
                int monthResult = AutoRenewMonth % 12;
                int yearResult = AutoRenewMonth / 12;

                if (monthResult == 0 && yearResult > 0)
                {
                    return yearResult + " tahun";
                }
                else if (yearResult == 0 && monthResult > 0)
                {
                    return monthResult + " bulan";
                }
                else if (monthResult > 0 && yearResult > 0)
                {
                    return yearResult + " tahun" + monthResult + " bulan";
                }
                else
                {
                    return null;
                }
            }
        }
        public string CurrencyOfTheMonthlyServiceFee
        {
            get
            {
                return CommonUtil.ContractOfTargetCurrency(ValueOfTheMonthlyServiceFee, ContractFeeCurrencyType);
            }
        }

        public decimal ValueOfTheMonthlyServiceFee
        {
            get
            {   
                return CommonUtil.ContractOfTargetFee(CommonUtil.TargetFee(ContractFeeCurrencyType, ContractFee, ContractFeeUsD));
            }
        }

        public string CurrencyOfSeculityDeposit
        {
            get
            {              
                return CommonUtil.ContractOfTargetCurrency(ValueOfSeculityDeposit, DepositFeeCurrencyType);
            }
        }

        public decimal ValueOfSeculityDeposit
        {
            get
            {
                return CommonUtil.ContractOfTargetFee(CommonUtil.TargetFee(DepositFeeCurrencyType, DepositFee, DepositFeeUsD));
            }
        }
        public string CurrencyOfApprovalOfQuotation
        {
            get
            {
                return CommonUtil.ContractOfTargetCurrency(ValueOfApprovalOfQuotation, InstallFee_ApproveContractCurrencyType);
            }
        }

        public decimal ValueOfApprovalOfQuotation
        {
            get
            {
                return CommonUtil.ContractOfTargetFee(CommonUtil.TargetFee(InstallFee_ApproveContractCurrencyType, InstallFee_ApproveContract, InstallFee_ApproveContractUsD));
            }
        }
        public string CurrecyOfCompleteInstallation
        {
            get
            {
                return CommonUtil.ContractOfTargetCurrency(ValueOfCompleteInstalation, InstallFee_CompleteInstallCurrencyType);
            }
        }

        public decimal ValueOfCompleteInstalation
        {
            get
            {
                return CommonUtil.ContractOfTargetFee(CommonUtil.TargetFee(InstallFee_CompleteInstallCurrencyType, InstallFee_CompleteInstall, InstallFee_CompleteInstallUsD));
            }
        }

        public string CurrencyOfCommencementDate
        {
            get
            {
                return CommonUtil.ContractOfTargetCurrency(ValueOfCommencementDate, InstallFee_StartServiceCurrencyType);
            }
        }

        public decimal ValueOfCommencementDate
        {
            get
            {
                return CommonUtil.ContractOfTargetFee(CommonUtil.TargetFee(InstallFee_StartServiceCurrencyType, InstallFee_StartService, InstallFee_StartServiceUsD));
            }
        }
        public string PaymentTermsEN
        {
            get
            {
                decimal fee = ValueOfTheMonthlyServiceFee;
                if(fee == 0)
                {
                    return "-";
                }
                else if(fee != 0)
                {
                    if(CreditTerm == 0)
                    {
                        return PaymentCycle + " month(s)" + "advance payment.";
                    }
                    else if(CreditTerm != 0)
                    {
                        return "Payment for " + PaymentCycle + " month(s) service period shall be made after " + CreditTerm + " month(s) service.";
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }
        public string PaymentTermsLC
        {
            get
            {
                decimal fee = ValueOfTheMonthlyServiceFee;
                if (fee == 0)
                {
                    return "-";
                }
                else if (fee != 0)
                {
                    if (CreditTerm == 0)
                    {
                        return "Pembayaran " + PaymentCycle + "bulan di muka.";
                    }
                    else if (CreditTerm != 0)
                    {
                        return "Pembayaran untuk periode " + PaymentCycle + " bulan dilakukan setelah " + CreditTerm + " bulan jasa.";
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public string SecurityDepositTextEN
        {
            get
            {
                string text = DepositFeePhaseNameEN;
                if(text == "-")
                {
                    return text;
                }
                else
                {
                    return "Based on " + text;
                }
            }
        }

        public string SecurityDepositTextLC
        {
            get
            {
                string text = DepositFeePhaseNameLC;
                if(text == "-")
                {
                    return text;
                }
                else
                {
                    return "Sesuai " + text;
                }
            }
        }
    }
}
