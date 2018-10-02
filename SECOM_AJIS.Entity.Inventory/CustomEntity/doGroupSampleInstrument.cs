﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Inventory.MetaData;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Inventory
{
    public partial class doGroupSampleInstrument
    {
        public string ObjectID { get; set; }
        public string LotNo { get; set; }
        private bool bTransferType = true;
        public bool TransferType
        {
            get
            {
                return bTransferType;
            }
            set
            {
                bTransferType = value;
            }
        }
    }



}