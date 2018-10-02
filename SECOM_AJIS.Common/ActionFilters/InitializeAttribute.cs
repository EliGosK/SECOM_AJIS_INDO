using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using System.Collections.Generic;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Controllers;
using System.Web.Routing;

namespace SECOM_AJIS.Common.ActionFilters
{
    public class InitializeAttribute : ActionFilterAttribute
    {
        private string ScreenID { get; set; }
        public bool IsPopupScreen { get; set; }

        public InitializeAttribute(string ScreenID)
        {
            this.ScreenID = ScreenID;
        }

        /// <summary>
        /// Event on action executing for initial screen information
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                if (CheckURLAction(filterContext) == false)
                    return;
                dsTransDataModel dsTrans = CommonUtil.dsTransData;
                if (dsTrans == null)
                    return;
                BaseController bController = filterContext.Controller as BaseController;
                
                #region Init Menu
                
                InitialMenu(filterContext);

                #endregion
                #region Screen ID

                //dsTrans.dtTransHeader.ScreenID = ScreenID;
                filterContext.Controller.ViewBag.ScreenCode = ScreenID;

                #endregion
                #region Screen Name

                string SubObjectID = "0";
                if (bController != null)
                {
                    ScreenParameter param = bController.GetScreenObject<object>() as ScreenParameter;
                    if (param != null)
                        SubObjectID = param.SubObjectID;
                }

                string strScreenName = null;
                //List<MenuName> mm = CommonUtil.MenuNameList;

                // Edit by Narupon W. : 1/Jan/2012
                //List<MenuName> mm = CommonUtil.dsTransData.dtMenuNameList;

                List<MenuName> mm = new List<MenuName>();
                if (CommonUtil.dsTransData != null)
                {
                    if (CommonUtil.dsTransData.dtMenuNameList != null)
                    {
                        mm = CommonUtil.dsTransData.dtMenuNameList;
                    }
                    else
                    {
                        mm = CommonUtil.MenuNameList;
                    }
                }
                else
                {
                    mm = CommonUtil.MenuNameList;
                }


                if (!CommonUtil.IsNullOrEmpty(mm))
                {
                    IEnumerable<MenuName> ScrNameIEnum = 
                        ( from c in mm 
                          where c.ObjectID == ScreenID 
                                && c.SubObjectID == SubObjectID
                          select c);
                    if (ScrNameIEnum != null && ScrNameIEnum.ToList().Count > 0)
                    {
                        CommonUtil.MappingObjectLanguage(ScrNameIEnum.ToList()[0]);
                        strScreenName = ScrNameIEnum.ToList()[0].ObjectName;
                    }
                }
                filterContext.Controller.ViewBag.ScreenName = strScreenName;

                #endregion
                #region Common search

                if (bController != null)
                {
                    ScreenSearchParameter param = bController.GetScreenObject<object>() as ScreenSearchParameter;
                    if (param != null)
                    {
                        if (param.CommonSearch != null)
                        {
                            filterContext.Controller.ViewBag.SearchContractCode = param.CommonSearch.ContractCode;
                            filterContext.Controller.ViewBag.SearchProjectCode = param.CommonSearch.ProjectCode;
                        }
                    }
                }

                #endregion

                // Change title to {Screen Name} [SECOM-AJIS]
                ////filterContext.Controller.ViewBag.Title = CommonUtil.TextCodeName(ScreenID, strScreenName, "-");
                //filterContext.Controller.ViewBag.Title = String.Format("{0} [SECOM-AJIS]", strScreenName);
                filterContext.Controller.ViewBag.Title = String.Format("{0} [SIMS]", strScreenName); //Modify by Jutarat A. on 21022013

                filterContext.Controller.ViewBag.UserName = dsTrans.dtUserData.EmpFullName;
                filterContext.Controller.ViewBag.ROWS_PER_PAGE_FOR_SEARCHPAGE = CommonValue.ROWS_PER_PAGE_FOR_SEARCHPAGE;
                filterContext.Controller.ViewBag.ROWS_PER_PAGE_FOR_VIEWPAGE = CommonValue.ROWS_PER_PAGE_FOR_VIEWPAGE;
                filterContext.Controller.ViewBag.ROWS_PER_PAGE_FOR_INVENTORY_CHECKING = CommonValue.ROWS_PER_PAGE_FOR_INVENTORY_CHECKING;
                              

                /* --- Update Data --- */
                CommonUtil.dsTransData = dsTrans;

                base.OnActionExecuting(filterContext);
            }
            catch(Exception)
            {
            }
        }
        /// <summary>
        /// Intial menum section
        /// </summary>
        /// <param name="filterContext"></param>
        private void InitialMenu(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;
            if (ctx.Session["Menu"] == null) { 
                string filePath = string.Format("{0}\\menu.xml", CommonUtil.WebPath);
                XmlDocument hDoc = new XmlDocument();
                hDoc.Load(filePath);

                if (hDoc.ChildNodes.Count >= 1)
                {
                    List<Menu> ListMenu = new List<Menu>();
               
                    //List<MenuName> mm = CommonUtil.MenuNameList;

                    //Change by Narupon W. : 1/Jan/2012
                    //Get MenuList from dsTransData (instead of CommonUtil.MenuNameList)
                    List<MenuName> mm = new List<MenuName>();
                    if (CommonUtil.dsTransData != null)
                    {
                        if (CommonUtil.dsTransData.dtMenuNameList != null)
                        {
                            mm = CommonUtil.dsTransData.dtMenuNameList;
                        }
                        else
                        {
                            mm = CommonUtil.MenuNameList;
                        }
                    }
                    else
                    {
                        mm = CommonUtil.MenuNameList;
                    }
                    
               

                    if (mm == null)
                        mm = new List<MenuName>();
                
                    foreach (XmlNode node in hDoc.ChildNodes[0].ChildNodes)
                    {
                        if (node.Name == "MenuHeader")
                        {
                            Menu MenuHeader = new Menu();

                            IEnumerable<MenuName> mmc = (from c in mm where c.ObjectID == node.Attributes["id"].Value select c);
                            if (mmc != null 
                                && mmc.ToList().Count > 0)
                            {
                                //MenuHeader.NameEN = mmc.ToList()[0].ObjectNameEN;
                                //MenuHeader.NameJP = mmc.ToList()[0].ObjectNameJP;
                                //MenuHeader.NameLC = mmc.ToList()[0].ObjectNameLC;

                                // Charnge map menu name from objectName --> objectAbbrName (Narupon W.)
                                MenuHeader.NameEN = CommonUtil.IsNullOrEmpty(mmc.ToList()[0].ObjectAbbrNameEN) ? mmc.ToList()[0].ObjectAbbrNameEN : mmc.ToList()[0].ObjectAbbrNameEN;
                                MenuHeader.NameJP = CommonUtil.IsNullOrEmpty(mmc.ToList()[0].ObjectAbbrNameJP) ? mmc.ToList()[0].ObjectAbbrNameEN : mmc.ToList()[0].ObjectAbbrNameJP;
                                MenuHeader.NameLC = CommonUtil.IsNullOrEmpty(mmc.ToList()[0].ObjectAbbrNameLC) ? mmc.ToList()[0].ObjectAbbrNameEN : mmc.ToList()[0].ObjectAbbrNameLC;

                                CommonUtil.MappingObjectLanguage(MenuHeader);
                            }
                            else
                            {
                                MenuHeader.NameEN = node.Attributes["NameEN"].Value;
                                MenuHeader.NameJP = node.Attributes["NameJP"].Value;
                                MenuHeader.NameLC = node.Attributes["NameLC"].Value;
                                CommonUtil.MappingObjectLanguage(MenuHeader);
                            }

                            if (node.Attributes["target"] != null)
                            {
                                MenuHeader.Target = node.Attributes["target"].Value;
                            }

                            if (node.Attributes["action"] != null && node.Attributes["controller"] != null)
                            {
                                string mSubObject = "0";
                                if (node.Attributes["subobject"] != null)
                                    mSubObject = node.Attributes["subobject"].Value;

                                MenuHeader.Action = node.Attributes["action"].Value;
                                MenuHeader.Controller = node.Attributes["controller"].Value;
                                MenuHeader.SubObject = mSubObject;
                            }
                            else if (node.Attributes["action"] != null)
                            {
                                MenuHeader.Action = node.Attributes["action"].Value;
                            }

                            if (node.ChildNodes.Count > 0)
                            {
                                List<Menu> lstMenuList = new List<Menu>();

                                foreach (XmlNode i in node.ChildNodes)
                                {

                                    if (i.Name == "MenuList")
                                    {
                                        string mSubObject = "0";
                                        if (i.Attributes["subobject"] != null)
                                            mSubObject = i.Attributes["subobject"].Value;
                                    
                                        Menu MenuList = new Menu();
                                        IEnumerable<MenuName> mml = (
                                            from c in mm 
                                            where c.ObjectID == i.Attributes["id"].Value
                                                    && c.SubObjectID == mSubObject
                                            select c);
                                        if (mml != null && mml.ToList().Count > 0)
                                        {
                                            //MenuList.NameEN = mml.ToList()[0].ObjectNameEN;
                                            //MenuList.NameJP = mml.ToList()[0].ObjectNameJP;
                                            //MenuList.NameLC = mml.ToList()[0].ObjectNameLC;

                                            // Charnge map menu name from objectName --> objectAbbrName (Narupon W.)
                                            MenuList.NameEN = CommonUtil.IsNullOrEmpty(mml.ToList()[0].ObjectAbbrNameEN) ? mml.ToList()[0].ObjectNameEN : mml.ToList()[0].ObjectAbbrNameEN;
                                            MenuList.NameJP = CommonUtil.IsNullOrEmpty(mml.ToList()[0].ObjectAbbrNameJP) ? mml.ToList()[0].ObjectNameEN : mml.ToList()[0].ObjectAbbrNameJP;
                                            MenuList.NameLC = CommonUtil.IsNullOrEmpty(mml.ToList()[0].ObjectAbbrNameLC) ? mml.ToList()[0].ObjectNameEN : mml.ToList()[0].ObjectAbbrNameLC;

                                            CommonUtil.MappingObjectLanguage(MenuList);
                                        }
                                        else
                                        {
                                            XmlAttribute a = i.Attributes["NameEN"];
                                            if (a != null)
                                            {
                                                MenuList.NameEN = i.Attributes["NameEN"].Value;
                                                MenuList.NameJP = i.Attributes["NameJP"].Value;
                                                MenuList.NameLC = i.Attributes["NameLC"].Value;
                                                CommonUtil.MappingObjectLanguage(MenuList);
                                            }

                                        }

                                        if (i.Attributes["ContractType"] != null)
                                        {
                                            int type = -1;
                                            if (int.TryParse(i.Attributes["ContractType"].Value, out type))
                                                MenuList.ContractType = type;
                                        }

                                        if (node.Attributes["target"] != null)
                                        {
                                            MenuList.Target = node.Attributes["target"].Value;
                                        }

                                        if (i.Attributes["action"] != null && i.Attributes["controller"] != null)
                                        {
                                            MenuList.Action = i.Attributes["action"].Value;
                                            MenuList.Controller = i.Attributes["controller"].Value;
                                            MenuList.SubObject = mSubObject;
                                        }
                                        else if (i.Attributes["action"] != null)
                                        {
                                            MenuList.Action = i.Attributes["action"].Value;
                                        }

                                        if (i.ChildNodes.Count > 0)
                                        {
                                            List<Menu> Subs = new List<Menu>();
                                            foreach (XmlNode s in i.ChildNodes)
                                            {
                                                if (s.Name == "SubMenu")
                                                {
                                                    string subObject = "0";
                                                    if (s.Attributes["subobject"] != null)
                                                        subObject = s.Attributes["subobject"].Value;

                                                    Menu Sub = new Menu();
                                                    IEnumerable<MenuName> mmSub = (
                                                        from c in mm 
                                                        where c.ObjectID == s.Attributes["id"].Value
                                                                && c.SubObjectID == subObject
                                                        select c );

                                                    if (mmSub != null && mmSub.ToList().Count > 0)
                                                    {
                                                        //Sub.NameEN = mmSub.ToList()[0].ObjectNameEN;
                                                        //Sub.NameJP = mmSub.ToList()[0].ObjectNameJP;
                                                        //Sub.NameLC = mmSub.ToList()[0].ObjectNameLC;

                                                        // Charnge map menu name from objectName --> objectAbbrName (Narupon W.)
                                                        Sub.NameEN = CommonUtil.IsNullOrEmpty(mmSub.ToList()[0].ObjectAbbrNameEN) ? mmSub.ToList()[0].ObjectNameEN : mmSub.ToList()[0].ObjectAbbrNameEN;
                                                        Sub.NameJP = CommonUtil.IsNullOrEmpty(mmSub.ToList()[0].ObjectAbbrNameJP) ? mmSub.ToList()[0].ObjectNameEN : mmSub.ToList()[0].ObjectAbbrNameJP;
                                                        Sub.NameLC = CommonUtil.IsNullOrEmpty(mmSub.ToList()[0].ObjectAbbrNameLC) ? mmSub.ToList()[0].ObjectNameEN : mmSub.ToList()[0].ObjectAbbrNameLC;

                                                        Sub.ObjectDescription = mmSub.ToList()[0].ObjectDescription;
                                                        CommonUtil.MappingObjectLanguage(Sub);
                                                    }
                                                
                                                    Sub.Action = s.Attributes["action"].Value;
                                                    Sub.Controller = s.Attributes["controller"].Value;
                                                    Sub.SubObject = subObject;
                                            

                                                    Subs.Add(Sub);
                                                }
                                            }
                                            MenuList.SubMenu = Subs;
                                        }
                                        lstMenuList.Add(MenuList);
                                    }
                                }
                                MenuHeader.SubMenu = lstMenuList;
                            }
                            ListMenu.Add(MenuHeader);
                        }
                    }
                    ctx.Session["Menu"] = ListMenu;
                    
                }
            }
            filterContext.Controller.ViewBag.lstMenu = ctx.Session["Menu"];
        }
        /// <summary>
        /// Check URL is correct format
        /// </summary>
        /// <param name="filterContext"></param>
        /// <returns></returns>
        private bool CheckURLAction(ActionExecutingContext filterContext)
        {
            if (this.ScreenID != SECOM_AJIS.Common.Util.ConstantValue.ScreenID.C_SCREEN_ID_MAIN)
            {
                bool isFoundKey = false;
                bool isPopup = false;

                BaseController bController = filterContext.Controller as BaseController;
                if (bController != null)
                {
                    ScreenParameter param = bController.GetScreenObject<object>() as ScreenParameter;
                    if (param != null)
                    {
                        if (CommonUtil.IsNullOrEmpty(param.Key) == false && param.IsLoaded == false)
                        {
                            isFoundKey = true;
                            param.IsLoaded = true;
                        }

                        isPopup = param.IsPopup;
                    }
                }
                
                if (isFoundKey == false)
                {
                    string url = CommonUtil.GenerateURL("Common", SECOM_AJIS.Common.Util.ConstantValue.ScreenID.C_SCREEN_ID_MAIN);
                    if (isPopup == true)
                    {
                        RedirectObject o = new RedirectObject();
                        o.URL = url;

                        JsonResult jRes = new JsonResult();
                        jRes.Data = o;
                        filterContext.Result = jRes;
                    }
                    else
                    {
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                        {
                            controller = "Common",
                            action = SECOM_AJIS.Common.Util.ConstantValue.ScreenID.C_SCREEN_ID_MAIN
                        }));
                    }

                    return false;
                }
            }

            return true;
        }
    }
}
