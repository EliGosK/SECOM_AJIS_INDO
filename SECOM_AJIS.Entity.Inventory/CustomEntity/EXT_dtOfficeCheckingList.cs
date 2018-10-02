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
    public partial class dtOfficeCheckingList
    {
        [LanguageMapping]
        public string OfficeName { get; set; }

        [LanguageMapping]
        public string LocationName { get; set; }

        public string OfficeNameDisplay { get { return string.Format("{0} ({1})" , this.OfficeName ,this.LocationName); } }

        public string DownloadStatus {
            
            get {
                string downloaded = CommonUtil.GetLabelFromResource("Inventory", "IVS150", "lblDownloaded");
                string notDownloaded = CommonUtil.GetLabelFromResource("Inventory", "IVS150", "lblNotDownloaded");
                return this.DownloadDate.HasValue ? downloaded : notDownloaded;
            
            } 
        
        }
    }
}