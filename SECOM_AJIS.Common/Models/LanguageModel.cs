using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Web.Mvc;

namespace SECOM_AJIS.Common.Models
{
    /// <summary>
    /// DO for language information
    /// </summary>
    public class LanguageModel
    {
        private string strUrl;
        private string strActionName;
        private string strControllerName;
        private RouteValueDictionary rvRouteValues;
        private bool bIsSelected;
        
        public string Url
        { 
            get { return this.strUrl; }
            set { this.strUrl = value; }
        }
        public string ActionName 
        { 
            get { return this.strActionName;}
            set { this.strActionName = value;}
        }
        public string ControllerName 
        { 
            get { return this.strControllerName; }
            set { this.strControllerName = value; }
        }
        public RouteValueDictionary RouteValues 
        { 
            get { return this.rvRouteValues; }
            set { this.rvRouteValues = value; }
        }
        public bool IsSelected
        { 
            get { return this.bIsSelected;}
            set { this.bIsSelected = value; }
        }
        public MvcHtmlString HtmlSafeUrl
        {
            get  
            {  
                return MvcHtmlString.Create(this.strUrl); 
            }
        }
    }
}
