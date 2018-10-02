using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Installation.MetaData;

namespace SECOM_AJIS.DataEntity.Installation
{

    public partial class dtInstallationSlipDetailsForView
    {
        CommonUtil cm = new CommonUtil();        

        public string InstrumentCode_Text
        {
            get
            {
                return (CommonUtil.IsNullOrEmpty(InstrumentCode)) ? "-" : InstrumentCode;

            }
        }

        public string InstrumentName_Text
        {
            get
            {
                return (CommonUtil.IsNullOrEmpty(InstrumentName)) ? "-" : InstrumentName;

            }
        }

        public Nullable<int> ContractInstalledQty_Numeric
        {
            get
            {
                return (CommonUtil.IsNullOrEmpty(ContractInstalledQty)) ? 0 : ContractInstalledQty;

            }
        }

        public Nullable<int> AddInstalledQty_Numeric
        {
            get
            {
                return (CommonUtil.IsNullOrEmpty(AddInstalledQty)) ? 0 : AddInstalledQty;

            }
        }

        public Nullable<int> ReturnQty_Numeric
        {
            get
            {
                return (CommonUtil.IsNullOrEmpty(ReturnQty)) ? 0 : ReturnQty;

            }
        }

        public Nullable<int> AddRemovedQty_Numeric
        {
            get
            {
                return (CommonUtil.IsNullOrEmpty(AddRemovedQty)) ? 0 : AddRemovedQty;

            }
        }

        public int? ContractInstallAfterChange
        {
            //get { return (ContractInstalledQty + AddInstalledQty - AddRemovedQty); }
            get { return (ContractInstalledQty ?? 0)+ (AddInstalledQty ?? 0) - (ReturnQty ?? 0) - (AddRemovedQty ?? 0); } //Modify by Jutarat A. on 31012013
        }

        public Nullable<int> ContractInstallAfterChange_Numeric
        {
            get
            {
                return (CommonUtil.IsNullOrEmpty(ContractInstallAfterChange)) ? 0 : ContractInstallAfterChange;

            }
        }

        public Nullable<int> MoveQty_Numeric
        {
            get
            {
                return (CommonUtil.IsNullOrEmpty(MoveQty)) ? 0 : MoveQty;

            }
        }

        public Nullable<int> MAExchangeQty_Numeric
        {
            get
            {
                return (CommonUtil.IsNullOrEmpty(MAExchangeQty)) ? 0 : MAExchangeQty;

            }
        }

        public Nullable<int> TotalStockOutQty_Numeric
        {
            get
            {
                //Modify by Jutarat A. on 25122013
                //int? TotalStockOutQty = this.TotalStockOutQty ?? 0 + this.CurrentStockOutQty ?? 0;
                //return (CommonUtil.IsNullOrEmpty(TotalStockOutQty)) ? 0 : TotalStockOutQty;
                int? intTotalStockOutQty = (this.TotalStockOutQty ?? 0) + (this.CurrentStockOutQty ?? 0);
                return (CommonUtil.IsNullOrEmpty(intTotalStockOutQty)) ? 0 : intTotalStockOutQty;
                //End Modify
            }
        }
        
        
    }
}


