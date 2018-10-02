using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Common;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class View_dtContractSameSite : dtContractsSameSite
    {
        public string ProductName { get; set; }
        public string LastChangeTypeName { get; set; }
        public string CalContractStatus
        {
            get
            {
                string strCalStatus = null;

                if (this.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE
                           && (this.LastChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_NEW_SALE
                           || this.LastChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_ADD_SALE))
                {
                    if (this.ContractStatus == SECOM_AJIS.Common.Util.ConstantValue.ContractStatus.C_CONTRACT_STATUS_BEF_START)
                    {
                        strCalStatus = this.LastChangeTypeName + "-" + CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, "CMS280", "lblUnoperated");
                    }
                    else if (this.ContractStatus == SECOM_AJIS.Common.Util.ConstantValue.ContractStatus.C_CONTRACT_STATUS_AFTER_START
                    || this.ContractStatus == SECOM_AJIS.Common.Util.ConstantValue.ContractStatus.C_CONTRACT_STATUS_STOPPING
                    || this.ContractStatus == SECOM_AJIS.Common.Util.ConstantValue.ContractStatus.C_CONTRACT_STATUS_END)
                    {
                        strCalStatus = this.LastChangeTypeName + "-" + CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, "CMS280", "lblOperated");
                    }
                    else if (this.ContractStatus == SECOM_AJIS.Common.Util.ConstantValue.ContractStatus.C_CONTRACT_STATUS_CANCEL
                       || this.ContractStatus == SECOM_AJIS.Common.Util.ConstantValue.ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL)
                    {
                        strCalStatus = this.LastChangeTypeName + "-" + CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, "CMS280", "lblCancel");
                    }
                }
                else
                {
                    strCalStatus = this.LastChangeTypeName;
                }
                return strCalStatus;
            }
        }

        public string ContTarPurcName
        {

            get
            {
                return "(1) " + this.CustNameEN + "<br>(2) " + this.CustNameLC;

            }

        }

        public string OperationDateText
        {
            get
            {
                return CommonUtil.TextDate(this.OperationDate);
            }
        }
        public string LastChangeDateText
        {
            get
            {
                return CommonUtil.TextDate(this.LastChangeDate);
            }
        }

        public string Text_Price
        {
            get
            {
                ICommonHandler handler = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                if (string.IsNullOrEmpty(this.PriceCurrencyType)) this.PriceCurrencyType = CurrencyUtil.C_CURRENCY_LOCAL;
                string currencyName = handler.getCurrencyName(this.PriceCurrencyType);

                return string.Format("{0} {1}", currencyName, (Price ?? 0).ToString("N2"));
            }
        }

    }
}
