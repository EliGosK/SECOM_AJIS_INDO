using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.Common.Helpers
{
    public static partial class CustomControlHelper
    {
        /// <summary>
        /// Generate empty div in case of load section
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        public static MvcHtmlString EmptyDIVforLoadData(this HtmlHelper helper, string id, string header)
        {
            var divBuilder = new TagBuilder("div");
            divBuilder.MergeAttribute("id", id);

            var divFormBuilder = new TagBuilder("div");
            divFormBuilder.MergeAttribute("class", "main-table");
            divFormBuilder.MergeAttribute("style", "display:none;");

            var divRowBuilder = new TagBuilder("div");
            divRowBuilder.MergeAttribute("class", "table-header");
            divRowBuilder.InnerHtml = string.Format("{0}&nbsp;<img src=\"../../Content/images/loading.gif\" />", header);
            divFormBuilder.InnerHtml = divRowBuilder.ToString(TagRenderMode.Normal);

            var divFormDesBuilder = new TagBuilder("div");

            divBuilder.InnerHtml = divFormBuilder.ToString(TagRenderMode.Normal) +
                                    divFormDesBuilder.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(divBuilder.ToString(TagRenderMode.Normal));
        }
        /// <summary>
        /// Generate empty div in case of load section
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        public static MvcHtmlString EmptyDIVforLoadData2(this HtmlHelper helper, string id)
        {
            var divBuilder = new TagBuilder("div");
            divBuilder.MergeAttribute("id", "div" + id);
            divBuilder.MergeAttribute("style", "display:none;");
            return MvcHtmlString.Create(divBuilder.ToString(TagRenderMode.Normal));
        }
        /// <summary>
        /// Generate sub-menu
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="lstMenu"></param>
        /// <returns></returns>
        public static MvcHtmlString CreateSubMenu(this HtmlHelper helper, List<Menu> lstMenu)
        {
            var bld = new StringBuilder();
            int maxleng = 0 ;
            int counter = 0;

            if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
            {
                maxleng =  CommonValue.MENU_MAX_LENGTH_EN;
            }
            else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
            {
                maxleng = CommonValue.MENU_MAX_LENGTH_JP;
            }
            else
            {
                maxleng = CommonValue.MENU_MAX_LENGTH_LC;
            }



            var ulBuilder = new TagBuilder("ul");
            foreach (var memu in lstMenu)
            {
                if (memu.SubMenu == null) // No child
                {
                    //if ( memu.Name == null)
                    //{
                    //    continue;
                    //}

                    string name = (CommonUtil.IsNullOrEmpty(memu.Name) ? "(No name)" : memu.Name);
                    name = name.Trim().PadRight(maxleng, ' ');
                    

                    // <li class="current"><a>Level one menu @c2.ToString() </a> </li>
                    var liBuilder = new TagBuilder("li");
                    liBuilder.AddCssClass("current");


                    string linkText = "";
                    string href = "";

                    if (memu.ContractType > 0)
                    {
                        // <li class='current'>@Html.ActionLink(j.Name, j.ContractType.ToString())</li>

                        linkText = name.Substring(0, maxleng);
                        href = memu.ContractType.ToString();

                        liBuilder.InnerHtml = System.Web.Mvc.Html.LinkExtensions.ActionLink(helper, linkText, href).ToString();
                        
                    }
                    else
                    {
                        //<li class='current'>@Html.ActionLink(j.Name, j.Action + "/" + j.SubObject, j.Controller, null, new { id = j.Action })</li>

                        linkText = name.Substring(0, maxleng);
                        href = string.Format("{0}/{1}", memu.Action, memu.SubObject);

                        /*
                        if (name.Trim().Length > maxleng)
                        {
                            liBuilder.InnerHtml = System.Web.Mvc.Html.LinkExtensions.ActionLink(helper, linkText, href, memu.Controller, null, new { id = memu.Action, title = memu.Name }).ToString();
                        }
                        else
                        {
                            liBuilder.InnerHtml = System.Web.Mvc.Html.LinkExtensions.ActionLink(helper, linkText, href, memu.Controller, null, new { id = memu.Action}).ToString();
                        }

                        */

                        liBuilder.InnerHtml = System.Web.Mvc.Html.LinkExtensions.ActionLink(helper, linkText, href, memu.Controller, null, new { id = memu.Action }).ToString();
                        

                    }


                    ulBuilder.InnerHtml += liBuilder.ToString(TagRenderMode.Normal);
                }
                else // has child
                {
                    if (memu.SubMenu.Count > 0)
                    {
                        //if (memu.Name == null)
                        //{
                        //    continue;
                        //}

                        string name = (CommonUtil.IsNullOrEmpty(memu.Name) ? "(No name)" : memu.Name);
                        name = name.Trim().PadRight(maxleng, ' ');

                        //  <li class="current"><a>Level one menu @c2.ToString()  #</a>
                        var liBuilder = new TagBuilder("li");

                        //if (counter == 0)
                        //{
                            liBuilder.AddCssClass("current");
                        //}
                        
                        liBuilder.AddCssClass("has_submenu");

                        var aBuilder = new TagBuilder("a");
                        //aBuilder.MergeAttribute("title", memu.Name);

                        aBuilder.InnerHtml = name.Substring(0, maxleng) ;

                        liBuilder.InnerHtml = aBuilder.ToString(TagRenderMode.Normal);

                        // Recursive
                        bld.Append(CreateSubMenu(helper, (List<Menu>)memu.SubMenu));

                        liBuilder.InnerHtml += bld.ToString();


                        ulBuilder.InnerHtml += liBuilder.ToString(TagRenderMode.Normal);

                        counter++;
                    }
                }


            }


            return MvcHtmlString.Create(ulBuilder.ToString(TagRenderMode.Normal));
        }
    }
}
