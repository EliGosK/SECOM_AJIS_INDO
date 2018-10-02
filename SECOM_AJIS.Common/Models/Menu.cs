using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.Common.Models
{
    /// <summary>
    /// DO for menu
    /// </summary>
    public class Menu
    {
        public Menu(string nName, string nAction, string nController, string subObject)
        {
            this.Name = nName;
            this.Action = nAction;
            this.Controller = nController;
        }
        public Menu() { }

        [LanguageMapping]
        public string Name { get; set; }
        public string NameEN { get; set; }
        public string NameJP { get; set; }
        public string NameLC { get; set; }
        public string Action { get; set; }
        public string ObjectDescription { get; set; }
        public string SubObject { get; set; }
        public string Target { get; set; }

        private bool blnVisible=true;
        public bool isVisible
        {
            get { if (this.blnVisible) { return true; } else { return false; } }
            set { this.blnVisible = value; }
        }
        
        public string Controller { get; set; }
        public List<Menu> SubMenu { get; set; }

        public int ContractType { get; set; }

        public string MenumKey
        {
            get
            {
                string key = this.Action;
                if (CommonUtil.IsNullOrEmpty(this.SubObject))
                    key += ";0";
                else
                    key += ";" + this.SubObject;

                return key;
            }
        }
    }
    /// <summary>
    /// DO for menu
    /// </summary>
    public class MenuName 
    {
        
        public string ObjectID { get; set; }
        public string SubObjectID { get; set; }

        [LanguageMapping]
        public string ObjectName { get; set; }
        public string ObjectNameEN { get; set; }
        public string ObjectNameJP { get; set; }
        public string ObjectNameLC { get; set; }

        [LanguageMapping]
        public string ObjectDescription { get; set; }
        public string ObjectDescriptionEN { get; set; }
        public string ObjectDescriptionJP { get; set; }
        public string ObjectDescriptionLC { get; set; }

        [LanguageMapping]
        public string ObjectAbbrName { get; set; }
        public string ObjectAbbrNameEN { get; set; }
        public string ObjectAbbrNameJP { get; set; }
        public string ObjectAbbrNameLC { get; set; }

        public string ObjectDisplay { 
            get 
            { 
                return CommonUtil.TextCodeName(this.ObjectID, this.ObjectName); 
            }
        }
    }

}
