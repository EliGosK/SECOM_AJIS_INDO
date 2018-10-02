using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Inventory.MetaData;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Inventory.MetaData;


namespace SECOM_AJIS.DataEntity.Inventory
{
    [MetadataType(typeof(dtResultInventorySlipIVS230_Meta))]
    public partial class dtResultInventorySlipIVS230
    {
        public string TransferType { get; set; }
        public string SlipStatus { get; set; }
        public string SourceOfficeName { get; set; }
        public string DestOfficeName { get; set; }
        public string SourceLocationName { get; set; }
        public string DestLocationName { get; set; }
        public string EmpFirstName { get; set; }
        public string EmpLastName { get; set; }

        public string StockOutDateText
        {
            get
            {
                return CommonUtil.TextDate(this.StockOutDate);
            }
        }

        public string StockInDateText
        {
            get
            {
                return CommonUtil.TextDate(this.StockInDate);
            }
        }

        public string EmpFullName 
        {
            get
            {
                return string.Format("{0} : {1} {2}", this.EmpNo, this.EmpFirstName, this.EmpLastName);
            }
        }
        
        public string ToJson
        {
            get
            {
                return CommonUtil.CreateJsonString(this);
            }
        }

    }

}

namespace SECOM_AJIS.DataEntity.Inventory.MetaData
{
    public class dtResultInventorySlipIVS230_Meta
    {
        [LanguageMapping]
        public string TransferType { get; set; }
        [LanguageMapping]
        public string SlipStatus { get; set; }
        [LanguageMapping]
        public string SourceOfficeName { get; set; }
        [LanguageMapping]
        public string DestOfficeName { get; set; }
        [LanguageMapping]
        public string SourceLocationName { get; set; }
        [LanguageMapping]
        public string DestLocationName { get; set; }
        [LanguageMapping]
        public string EmpFirstName { get; set; }
        [LanguageMapping]
        public string EmpLastName { get; set; }
    }

}
