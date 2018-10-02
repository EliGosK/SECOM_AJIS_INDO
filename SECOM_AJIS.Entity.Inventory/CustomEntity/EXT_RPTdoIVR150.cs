using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Inventory
{
    public partial class RPTdoIVR150
    {
        public string ControlItemShow {
            get
            {
                switch (ControlItem)
                {
                    case 1	:	return	"Carry-forward of previous period";
                    case 2	:	return	"Purchasing stock-in";
                    case 3	:	return	"Special stock-in";
                    case 4	:	return	"Stock-out equipment for MA (to office)";
                    case 5	:	return	"Return equipment for MA (from office)";
                    case 6	:	return	"Stock-out sale equipment";
                    case 7	:	return	"Stock-out new rental equipment for installation";
                    case 8	:	return	"Stock-out equipment for MA";
                    case 9	:	return	"Stock-out equipment for change installation";
                    case 10	:	return	"Returning not-used equipment";
                    case 11	:	return	"Sale billing agreement";
                    case 12	:	return	"New start service & complete installation after operation";
                    case 13	:	return	"Returning removal equipment (change/removal installation, MA exchange)";
                    case 14	:	return	"Special stock-out";
                    case 15	:	return	"Pre-disposal";
                    case 16	:	return	"Unremovable equipment";
                    case 17	:	return	"Repairing request";
                    case 18	:	return	"Repairing return";
                    case 19	:	return	"Fix returned equipment";
                    case 20	:	return	"Total plus of current period";
                    case 21	:	return	"Total minus of current period";
                    case 22	:	return	"Carry-forward of current period before check stock";
                    case 23	:	return	"Lack qty of  stocktaking";
                    case 24	:	return	"Over qty of  stocktaking";
                    case 25	:	return	"Modify equipment qty by real investigation (+)";
                    case 26	:	return	"Modify equipment qty by real investigation (-)";
                    case 27	:	return	"Carry-forward of current period after adjust stocktacking";
                }
                return "None";
            }
        }

        public string UserWHShow
        {
            get
            {
                return UserWH == 0 ? "-" : UserWH.ToString("#,##0");
            }
        }

        public string WIPWHArea0Show
        {
            get
            {
                return WIPWHArea0 == 0 ? "-" : WIPWHArea0.ToString("#,##0");
            }
        }

        public string WIPWHArea1Show
        {
            get
            {
                return WIPWHArea1 == 0 ? "-" : WIPWHArea1.ToString("#,##0");
            }
        }

        public string WIPWHArea2Show
        {
            get
            {
                return WIPWHArea2 == 0 ? "-" : WIPWHArea2.ToString("#,##0");
            }
        }

        public string WIPWHArea3Show
        {
            get
            {
                return WIPWHArea3 == 0 ? "-" : WIPWHArea3.ToString("#,##0");
            }
        }

        public string NewHQReturnedWHShow
        {
            get
            {
                return NewHQReturnedWH == 0 ? "-" : NewHQReturnedWH.ToString("#,##0");
            }
        }

        public string NewHQInstockWHShow
        {
            get
            {
                return NewHQInstockWH == 0 ? "-" : NewHQInstockWH.ToString("#,##0");
            }
        }

        public string NewOfficeOfficeWHShow
        {
            get
            {
                return NewOfficeOfficeWH == 0 ? "-" : NewOfficeOfficeWH.ToString("#,##0");
            }
        }

        public string NewTransferringTransferWHShow
        {
            get
            {
                return NewTransferringTransferWH == 0 ? "-" : NewTransferringTransferWH.ToString("#,##0");
            }
        }

        public string NewTransferringWaitingReturnWHShow
        {
            get
            {
                return NewTransferringWaitingReturnWH == 0 ? "-" : NewTransferringWaitingReturnWH.ToString("#,##0");
            }
        }

        public string NewRepairingRepairRequestWHShow
        {
            get
            {
                return NewRepairingRepairRequestWH == 0 ? "-" : NewRepairingRepairRequestWH.ToString("#,##0");
            }
        }

        public string NewRepairingRepairingWHShow
        {
            get
            {
                return NewRepairingRepairingWH == 0 ? "-" : NewRepairingRepairingWH.ToString("#,##0");
            }
        }

        public string NewRepairingRepairReturnWHShow
        {
            get
            {
                return NewRepairingRepairReturnWH == 0 ? "-" : NewRepairingRepairReturnWH.ToString("#,##0");
            }
        }

        public string SecondhandHQReturnedWHShow
        {
            get
            {
                return SecondhandHQReturnedWH == 0 ? "-" : SecondhandHQReturnedWH.ToString("#,##0");
            }
        }

        public string SecondhandHQInstockWHShow
        {
            get
            {
                return SecondhandHQInstockWH == 0 ? "-" : SecondhandHQInstockWH.ToString("#,##0");
            }
        }

        public string SecondhandOfficeOfficeWHShow
        {
            get
            {
                return SecondhandOfficeOfficeWH == 0 ? "-" : SecondhandOfficeOfficeWH.ToString("#,##0");
            }
        }

        public string SecondhandOfficeSNRWHShow
        {
            get
            {
                return SecondhandOfficeSNRWH == 0 ? "-" : SecondhandOfficeSNRWH.ToString("#,##0");
            }
        }

        public string SecondhandTransferringTransferWHShow
        {
            get
            {
                return SecondhandTransferringTransferWH == 0 ? "-" : SecondhandTransferringTransferWH.ToString("#,##0");
            }
        }

        public string SecondhandReturnWaitingReturnWHShow
        {
            get
            {
                return SecondhandReturnWaitingReturnWH == 0 ? "-" : SecondhandReturnWaitingReturnWH.ToString("#,##0");
            }
        }

        public string SecondhandReturnReturnWIPWHShow
        {
            get
            {
                return SecondhandReturnReturnWIPWH == 0 ? "-" : SecondhandReturnReturnWIPWH.ToString("#,##0");
            }
        }

        public string SecondhandRepairingRepairRequestWHShow
        {
            get
            {
                return SecondhandRepairingRepairRequestWH == 0 ? "-" : SecondhandRepairingRepairRequestWH.ToString("#,##0");
            }
        }

        public string SecondhandRepairingRepairingWHShow
        {
            get
            {
                return SecondhandRepairingRepairingWH == 0 ? "-" : SecondhandRepairingRepairingWH.ToString("#,##0");
            }
        }

        public string SecondhandRepairingRepairReturnWHShow
        {
            get
            {
                return SecondhandRepairingRepairReturnWH == 0 ? "-" : SecondhandRepairingRepairReturnWH.ToString("#,##0");
            }
        }
    }
}