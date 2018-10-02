using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Contract
{
    public partial class tbt_BillingTemp
    {
        CommonUtil comUtil = new CommonUtil();

        public string ContractCodeShort
        {
            get
            {
                return comUtil.ConvertContractCode(ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public virtual string BillingTargetCodeShort
        {
            get
            {
                return comUtil.ConvertBillingTargetCode(BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public virtual string BillingClientCodeShort
        {
            get
            {
                return comUtil.ConvertBillingClientCode(BillingClientCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }
    }

    [MetadataType(typeof(tbt_BillingTemp_Insert_MetaData))]
    public class tbt_BillingTemp_Insert : tbt_BillingTemp
    {

    }

    [MetadataType(typeof(tbt_BillingTemp_Update_MetaData))]
    public class tbt_BillingTemp_Update : tbt_BillingTemp
    {

    }

    [MetadataType(typeof(tbt_BillingTemp_UpdateBillingClientAndOffice_MetaData))]
    public class tbt_BillingTemp_UpdateBillingClientAndOffice : tbt_BillingTemp
    {

    }

    [MetadataType(typeof(tbt_BillingTemp_ClientOfficeCode_MetaData))]
    public class tbt_BillingTemp_ClientOfficeCode : tbt_BillingTemp
    {

    }

    public class tbt_BillingTemp_Insert_MetaData
    {
        [NotNullOrEmpty]
        public string ContractCode { get; set; }
        [NotNullOrEmpty]
        public string OCC { get; set; }
        [NotNullOrEmpty]
        public string BillingType { get; set; }
        [NotNullOrEmpty]
        public string BillingAmt { get; set; }
        [NotNullOrEmpty]
        public string PayMethod { get; set; }
    }

    public class tbt_BillingTemp_Update_MetaData
    {
        [NotNullOrEmpty]
        public string ContractCode { get; set; }
        [NotNullOrEmpty]
        public string OCC { get; set; }
        [NotNullOrEmpty]
        public string SequenceNo { get; set; }
        [NotNullOrEmpty]
        public string BillingType { get; set; }
        [NotNullOrEmpty]
        public string BillingAmt { get; set; }
        [NotNullOrEmpty]
        public string PayMethod { get; set; }
    }

    public class tbt_BillingTemp_ClientOfficeCode_MetaData
    {
        [NotNullOrEmpty]
        public string BillingClientCode { get; set; }

        [NotNullOrEmpty]
        public string BillingOfficeCode { get; set; }
    }

    public class tbt_BillingTemp_UpdateBillingClientAndOffice_MetaData
    {
        [NotNullOrEmpty]
        public string ContractCode { get; set; }
        [NotNullOrEmpty]
        public string BillingClientCode { get; set; }
        [NotNullOrEmpty]
        public string BillingOfficeCode { get; set; }
        [NotNullOrEmpty]
        public string BillingOCC { get; set; }
        [NotNullOrEmpty]
        public string BillingTargetCode { get; set; }
    }


}
