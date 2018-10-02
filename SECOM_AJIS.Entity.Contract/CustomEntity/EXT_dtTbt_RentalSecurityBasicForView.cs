using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Contract.MetaData;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.DataEntity.Contract
{
    /// <summary>
    /// Do Of table rental security basic for view
    /// </summary>
    public partial class dtTbt_RentalSecurityBasicForView
    {
        
        public List<DataEntity.Common.doMiscTypeCode> Currencies { get; set; }
        public string TextTransferNormalContractFee
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.NormalContractFee);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.NormalContractFeeCurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

        public string TextTransferOrderContractFee
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.OrderContractFee);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.OrderContractFeeCurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

        public string TextTransferNormalInstallFee
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.NormalInstallFee);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.NormalInstallFeeCurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

        public string TextTransferOrderInstallFee
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.OrderInstallFee);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.OrderInstallFeeCurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

        public string TextTransferNewBldMgmtCost
        {
            get
            {
                // add by Jirawat Jannet on 2016-12-28
                decimal amt = this.NewBldMgmtCostCurrencyType == CurrencyUtil.C_CURRENCY_US ? NewBldMgmtCostUsd ?? 0 : NewBldMgmtCost ?? 0;
                //string txt = CommonUtil.TextNumeric(this.NewBldMgmtCost); // comment by Jirawat Jannet on 2016-12-28
                string txt = CommonUtil.TextNumeric(amt);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.NewBldMgmtCostCurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

        public string TextTransferContractFeeOnStop
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.ContractFeeOnStop);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.ContractFeeOnStopCurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

        public string TextTransferInsuranceCoverageAmount
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.InsuranceCoverageAmount);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.InsuranceCoverageAmountCurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

        public string TextTransferMonthlyInsuranceFee
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.MonthlyInsuranceFee);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.MonthlyInsuranceFeeCurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

        public string TextTransferMaintenanceFee1
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.MaintenanceFee1);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.MaintenanceFee1CurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

        public string TextTransferAdditionalFee1
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.AdditionalFee1);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.AdditionalFee1CurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

        public string TextTransferAdditionalFee2
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.AdditionalFee2);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.AdditionalFee2CurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

        public string TextTransferAdditionalFee3
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.AdditionalFee3);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.AdditionalFee3CurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

        public string TextTransferOrderInstallFee_ApproveContract
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.OrderInstallFee_ApproveContract);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.OrderInstallFee_ApproveContractCurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

        public string TextTransferOrderInstallFee_CompleteInstall
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.OrderInstallFee_CompleteInstall);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.OrderInstallFee_CompleteInstallCurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

        public string TextTransferOrderInstallFee_StartService
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.OrderInstallFee_StartService);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.OrderInstallFee_StartServiceCurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

        public string TextTransferInstallFeePaidBySECOM
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.InstallFeePaidBySECOM);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.InstallFeePaidBySECOMCurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

        public string TextTransferInstallFeeRevenueBySECOM
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.InstallFeeRevenueBySECOM);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.InstallFeeRevenueBySECOMCurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

        public string ProductName { get; set; }

        public string DocumentName { get; set; }

        public string DocumentNoName { get; set; }
    }
}