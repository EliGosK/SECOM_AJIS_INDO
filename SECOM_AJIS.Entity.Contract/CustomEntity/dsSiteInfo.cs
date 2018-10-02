using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SECOM_AJIS.DataEntity.Master;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class dsSiteInfo
    {
        public List<doGetTbm_Site> doGetTbm_Site { get; set; }
        public List<tbm_BuildingUsage> tbm_BuildingUsage { get; set; }
    }
}
