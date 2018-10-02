using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO for rental change type
    /// </summary>
    public class RentalChangeType
    {
        public static string C_RENTAL_CHANGE_TYPE_ALTERNATIVE_START { get; private set; }
        public static string C_RENTAL_CHANGE_TYPE_APPROVE { get; private set; }
        public static string C_RENTAL_CHANGE_TYPE_CANCEL { get; private set; }
        public static string C_RENTAL_CHANGE_TYPE_CANCEL_BEFORE_START { get; private set; }
        public static string C_RENTAL_CHANGE_TYPE_CHANGE_FEE { get; private set; }
        //public static string C_RENTAL_CHANGE_TYPE_CHANGE_CONTRACT_FEE { get; private set; }
        public static string C_RENTAL_CHANGE_TYPE_CHANGE_INSTRU_DURING_STOP { get; private set; }
        public static string C_RENTAL_CHANGE_TYPE_END_CONTRACT { get; private set; }
        public static string C_RENTAL_CHANGE_TYPE_MODIFY_INSTRUMENT_QTY { get; private set; }
        public static string C_RENTAL_CHANGE_TYPE_NEW_START { get; private set; }
        public static string C_RENTAL_CHANGE_TYPE_PLAN_CHANGE { get; private set; }
        public static string C_RENTAL_CHANGE_TYPE_STOP { get; private set; }
        public static string C_RENTAL_CHANGE_TYPE_TERMINATED { get; private set; }
        public static string C_RENTAL_CHANGE_TYPE_CHANGE_EXPECTED_OPR_DATE { get; private set; }
        public static string C_RENTAL_CHANGE_TYPE_RESUME { get; private set; }

        public static string C_RENTAL_CHANGE_TYPE_CHANGE_NAME { get; private set; }
        public static string C_RENTAL_CHANGE_TYPE_CHANGE_NAME_DURING_STOP { get; private set; }
        public static string C_RENTAL_CHANGE_TYPE_REMOVE_ALL { get; private set; }

        public static string C_RENTAL_CHANGE_TYPE_REMOVE_INSTRU_DURING_STOP { get; private set; }
        public static string C_RENTAL_CHANGE_TYPE_EXCHANGE_INSTRU_AT_MA { get; private set; }
        public static string C_RENTAL_CHANGE_TYPE_MOVE_INSTRU { get; private set; }
        public static string C_RENTAL_CHANGE_TYPE_MODIFY_QTY_DURING_STOP { get; private set; }
        public static string C_RENTAL_CHANGE_TYPE_CHANGE_FEE_DURING_STOP { get; private set; }
        public static string C_RENTAL_CHANGE_TYPE_CHANGE_WIRING { get; private set; } //Add by Jutarat A. on 21052013
    }
}
