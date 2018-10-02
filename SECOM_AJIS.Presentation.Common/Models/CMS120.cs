using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.CustomAttribute;

using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Presentation.Common.Models.MetaData;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.DataEntity.Master;



namespace SECOM_AJIS.Presentation.Common.Models
{
    /// <summary>
    /// Screen parameter for CMS120
    /// </summary>
    public  class CMS120_ScreenParameter : ScreenSearchParameter
    {
        //============= In parameter=============//
        [KeepSession]
        public string strContractCode { get; set; }
        //============= In parameter=============//

        [KeepSession]
        [NotNullOrEmpty]
        public string ContractCode { get; set; }
        public string OCC { get; set; }
        public string ContractTargetCode { get; set; }
        public string PurchaserCustCode { get; set; }
        public string RealCustomerCode { get; set; }
        public string SiteCode { get; set; }

        [NotNullOrEmpty]
        public string ServiceTypeCode { get; set; }
        public string MATargetContractCode { get; set; }
        public string ProductCode { get; set; }

        // additional
        public string CSCustCode { get; set; }
        public string RCCustCode { get; set; }
        public string Mode { get; set; }

    }

    public class CMS120_ContractFeeDetail
    {
        public string Title { get; set; }
        public decimal? NormalAmt { get; set; }
        public decimal? OrderAmt { get; set; }
        public string DiscountName { get; set; }
        public string DiscountRate { get; set; }
        public string txtNormalAmt { get; set; }
        public string txtOrderAmt { get; set; }
        public string Currency1 { get; set; }
        public string Currency2 { get; set; }
    }
   
}



