﻿
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
    public partial class dtSaleHistoryDigest
    {
        #region Primitive Properties

        public List<DataEntity.Common.doMiscTypeCode> Currencies { get; set; }
        public string TextTransferSalePrice
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.SalePrice);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.SalePriceCurrencyType);
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
        #endregion
    }
}