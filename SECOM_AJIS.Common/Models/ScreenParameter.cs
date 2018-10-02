using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;
using System.Reflection;

namespace SECOM_AJIS.Common.Models
{
    /// <summary>
    /// DO for send parameter to controller
    /// </summary>
    [Serializable]
    public abstract class ScreenParameter
    {
        public class CommonSearchDo
        {
            public string ContractCode { get; set; }
            public string ProjectCode { get; set; }
        }

        public ScreenParameter()
        {
            this.BackStep = true;
        }

        [KeepSession]
        public string Key { get; set; }
        [KeepSession]
        public string Module { get; set; }
        [KeepSession]
        public string ScreenID { get; set; }
        [KeepSession]
        public string SubObjectID { get; set; }
        [KeepSession]
        public string CallerKey { get; set; }
        [KeepSession]
        public string CallerModule { get; set; }
        [KeepSession]
        public string CallerScreenID { get; set; }
        [KeepSession]
        public bool IsPopup { get; set; }
        [KeepSession]
        public bool IsLoaded { get; set; }
        [KeepSession]
        public bool BackStep { get; set; }

        public static ScreenParameter ResetScreenParameter(ScreenParameter param)
        {
            ScreenParameter nparam = (ScreenParameter)Activator.CreateInstance(param.GetType());
            if (nparam != null)
            {
                Dictionary<string, KeepSessionAttribute> sAttr =
                CommonUtil.CreateAttributeDictionary<KeepSessionAttribute>(param);
                foreach (KeyValuePair<string, KeepSessionAttribute> attr in sAttr)
                {
                    PropertyInfo prop = param.GetType().GetProperty(attr.Key);
                    if (prop != null)
                    {
                        if (prop.CanWrite == false)
                            continue;

                        object val = prop.GetValue(param, null);
                        if (val != null)
                        {
                            PropertyInfo nprop = nparam.GetType().GetProperty(attr.Key);
                            if (nprop != null)
                                nprop.SetValue(nparam, val, null);
                        }
                    }
                }
            }

            return nparam;
        }
    }
    public abstract class ScreenSearchParameter : ScreenParameter
    {
        [KeepSession]
        public CommonSearchDo CommonSearch { get; set; }
    }

    /// <summary>
    /// Attribute for keep data when refresh screen
    /// </summary>
    public class KeepSessionAttribute : Attribute
    {
    }
    
}
