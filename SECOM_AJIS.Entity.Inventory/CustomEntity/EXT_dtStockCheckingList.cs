using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Inventory.MetaData;
using SECOM_AJIS.Common.Util;


namespace SECOM_AJIS.DataEntity.Inventory
{

    public partial class dtStockCheckingList
    {
        [LanguageMapping]
        public string OfficeName { get; set; }
        [LanguageMapping]
        public string LocationName { get; set; }
        [LanguageMapping]
        public string AreaName { get; set; }

        public string AreaCodeName { get { return CommonUtil.TextCodeName(this.AreaCode, this.AreaName); } }

        // GroupingKey
        public string GroupingKey
        {
            get
            {
                string lblOffice = CommonUtil.GetLabelFromResource("Inventory", "IVS170", "headerOffice");
                string lblLocation = CommonUtil.GetLabelFromResource("Inventory", "IVS170", "headerLocation");
                string lblAreaCode = CommonUtil.GetLabelFromResource("Inventory", "IVS170", "headerInstrumentArea");
                return string.Format("{0}: <b>{1}</b>   {2}: <b>{3}</b>   {4}: <b>{5}</b>", lblOffice, this.OfficeName, lblLocation, this.LocationName, lblAreaCode, this.AreaCodeName);
            }

        }


    }


}
