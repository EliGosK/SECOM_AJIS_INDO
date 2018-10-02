using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Contract.MetaData;

namespace SECOM_AJIS.DataEntity.Contract
{
    public partial class dtContractListGrp
    {
        CommonUtil c = new CommonUtil();
        public string ContractCodeShort
        {
            get { return c.ConvertContractCode(this.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT); }
        }

        public string ContractNameForShow
        {
            get { return "(1) " + this.CustNameEN + "<br>(2) " + this.CustNameLC; }
        }

        public string BranchNameForShow
        {
            get { return "(1) " + this.BranchNameEN + "<br>(2) " + this.BranchNameLC; }
        }

        public string AddressFullForShow
        {
            get { return "(1) " + this.AddressFullEN + "<br>(2) " + this.AddressFullLC; }
        }

        public List<DataEntity.Common.doMiscTypeCode> Currencies { get; set; }
        public string TextTransferPrice
        {
            get
            {
                if(this.PriceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    this.Price = this.PriceUsd;
                }

                string txt = CommonUtil.TextNumeric(this.Price);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.PriceCurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }
    }
}



