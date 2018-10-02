using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.Common.Models
{
    /// <summary>
    /// DO for generate combobox
    /// </summary>
    [Serializable]
    public class ComboBoxModel
    {
        private List<ComboBoxOptionModel> objLst;

        public List<ComboBoxOptionModel> List
        {
            get { return this.objLst; }
        }


        private const string COMBO_FIRSTELEMTXT_SELECT = "SELECT";
        private const string COMBO_FIRSTELEMTXT_ALL = "ALL";
        public void SetList<T>(List<T> lst, string display, string value, bool include_idx0 = true, CommonUtil.eFirstElementType idx0_type = CommonUtil.eFirstElementType.Select) where T : class
        {
            if (lst == null)
            {
                this.objLst = null;
            }
            else
            {
                this.objLst = new List<ComboBoxOptionModel>();
                if (include_idx0)
                {
                    //MessageModel select = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0113);
                    string strFirstElemText = string.Empty;

                    if (idx0_type == CommonUtil.eFirstElementType.Select)
                    {
                        strFirstElemText = CommonUtil.GetLabelFromResource("Common", "CommonResources", "lblComboboxSelect");
                    }
                    if (idx0_type == CommonUtil.eFirstElementType.Short)
                    {
                        strFirstElemText = CommonUtil.GetLabelFromResource("Common", "CommonResources", "lblComboboxCUSTOM_SELECT");
                    }
                    else if (idx0_type == CommonUtil.eFirstElementType.All)
                    {
                        strFirstElemText = CommonUtil.GetLabelFromResource("Common", "CommonResources", "lblComboboxAll");
                    }

                    ComboBoxOptionModel foModel = new ComboBoxOptionModel()
                    {

                        Display = strFirstElemText ,   //select.Message,
                        Value = ""
                    };
                    objLst.Add(foModel);
                }

                foreach (T et in lst)
                {
                    PropertyInfo propD = et.GetType().GetProperty(display);
                    PropertyInfo propV = et.GetType().GetProperty(value);
                    if (propD != null && propV != null)
                    {
                        if (propD.GetValue(et, null) != null
                            && propV.GetValue(et, null) != null)
                        {
                            ComboBoxOptionModel oModel = new ComboBoxOptionModel()
                            {
                                Display = propD.GetValue(et, null).ToString(),
                                Value = propV.GetValue(et, null).ToString()
                            };
                            objLst.Add(oModel);
                        }
                    }
                }
            }
        }
    }
    /// <summary>
    /// DO for option in combobox
    /// </summary>
    [Serializable]
    public class ComboBoxOptionModel
    {
        private string strDisplay;
        private string strValue;

        public string Display
        {
            get { return this.strDisplay; }
            set { this.strDisplay = value; }
        }
        public string Value
        {
            get { return this.strValue; }
            set { this.strValue = value; }
        }
    }
    /// <summary>
    /// DO for combobox value
    /// </summary>
    public class ComboObjectModel
    {
        public string Code { get; set; }
        public string DisplayName { get; set; }
    }
}
