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
    public partial class dtSearchSaleWarrantyExpireResult
    {
        CommonUtil comUtil = new CommonUtil();

        [LanguageMapping]
        public string SaleContractOfficeName { get; set; }

        [LanguageMapping]
        public string SaleOperationOfficeName { get; set; }

        [LanguageMapping]
        public string SalesmanInChargeName { get; set; }

        [LanguageMapping]
        public string SaleProductName { get; set; }

        public string SaleContractCodeShort
        {
            get
            { 
                return comUtil.ConvertContractCode(SaleContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string MAContractCodeShort
        {
            get
            { 
                return comUtil.ConvertContractCode(MAContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string MASiteCodeShort
        {
            get
            {
                return comUtil.ConvertSiteCode(MASiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        string _SaleWarrantyFromDisplay = string.Empty;
        public string SaleWarrantyFromDisplay
        {
            get
            {
                if (SaleWarrantyFrom != null)
                    _SaleWarrantyFromDisplay = SaleWarrantyFrom.Value.ToString("MMM-yyyy");

                return _SaleWarrantyFromDisplay;
            }
        }

        string _SaleWarrantyToDisplay = string.Empty;
        public string SaleWarrantyToDisplay
        {
            get
            {
                if (SaleWarrantyTo != null)
                    _SaleWarrantyToDisplay = SaleWarrantyTo.Value.ToString("MMM-yyyy");

                return _SaleWarrantyToDisplay;
            }
        }
    }
}
