
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Contract.MetaData;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Contract
{
    public partial class doProjectContractDetail
    {
        public List<DataEntity.Common.doMiscTypeCode> Currencies { get; set; }

        [LanguageMapping]
        public string ProductName { get; set; }
        public string ProductCodeName
        {
            get
            {
                string strProdCodeNam = CommonUtil.TextCodeName(this.ProductCode, this.ProductName).Trim();

                if (strProdCodeNam != "")
                    return strProdCodeNam;
                else
                    return "-";

            }

        }
        public string DocAuditCodeName
        {
            get
            {
                string strDocCodeNam = CommonUtil.TextCodeName(this.DocAuditResultCode, this.DocAuditResultName).Trim();

                if (strDocCodeNam != "")
                    return strDocCodeNam;
                else
                    return "-";
            }

        }
        public string ContractCode_Short
        {
            get
            {
                return new CommonUtil().ConvertContractCode(this.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }

        }
        public string Fee_View
        {
            get
            {
                string txt;
                if (this.FeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    txt = CommonUtil.TextNumeric(this.FeeUsd);
                else
                    txt = CommonUtil.TextNumeric(this.Fee);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == (this.FeeCurrencyType == null?
                        SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL : this.FeeCurrencyType));
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }

                return txt;
            }
        }
        [LanguageMapping]
        public string DocAuditResultName { get; set; }
    }

}

