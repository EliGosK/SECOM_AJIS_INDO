using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Master.MetaData;

namespace SECOM_AJIS.DataEntity.Master
{
    /// <summary>
    /// Do Of instrument
    /// </summary>
    [MetadataType(typeof(doInstrumentData_MetaData))]
    public partial class doInstrumentData
    {
        public string LineUpTypeCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.LineUpTypeCode, this.LineUpTypeName);
            }
        }

        public string ToJson
        {
            get
            {
                return CommonUtil.CreateJsonString(this);
            }
        }
        public List<DataEntity.Common.doMiscTypeCode> Currencies { get; set; }
        public string TextTransferSaleUnitPrice
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.SaleUnitPrice);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.SaleUnitPriceCurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

        public string TextTransferRentalUnitPrice
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.RentalUnitPrice);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.RentalUnitPriceCurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

        public string TextTransferAddUnitPrice
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.AddUnitPrice);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.AddUnitPriceCurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

    }
}
namespace SECOM_AJIS.DataEntity.Master.MetaData
{  
    /// <summary>
    /// Do Of instrument meta data
    /// </summary>
    public class doInstrumentData_MetaData
    {
        [LanguageMapping]
        public string LineUpTypeName { get; set; }
    }
}