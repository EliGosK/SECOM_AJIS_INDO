using System;
using System.Web;
using System.Web.Mvc;
using SECOM_AJIS.Common.Controllers;
using System.Collections.Generic;

namespace SECOM_AJIS.Common.Models
{
    /// <summary>
    /// DO for message information
    /// </summary>
    [Serializable]
    public class MessageModel
    {
        #region Enum

        public enum MESSAGE_TYPE
        {
            WARNING = 0,
            INFORMATION,
            INFORMATION_OK,
            WARNING_DIALOG,
            WARNING_DIALOG_LIST
        }

        #endregion

        public MESSAGE_TYPE MessageType { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public string[] Controls { get; set; }

        public string ToJson
        {
            get
            {
                return SECOM_AJIS.Common.Util.CommonUtil.CreateJsonString(this);
            }
        }
    }
}
