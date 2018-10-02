using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.Presentation.Common.Models;
using System.Xml;
using SECOM_AJIS.DataEntity.Billing;

namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {
        #region Authority

        /// <summary>
        /// Check screen authority and permission
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CMS021_Authority(FlowMenuScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                #region Check screen permission

                if (CheckUserPermission("CMS021", FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                #endregion
                #region Check contract code has in session

                //dsTransDataModel dsTrans = CommonUtil.dsTransData;
                //if (dsTrans.dtCommonSearch != null)
                //{
                //    if (CommonUtil.IsNullOrEmpty(dsTrans.dtCommonSearch.ContractCode) == false)
                //    {
                //        param.Condition = new FlowMenuCondition()
                //        {
                //            ContractCode = dsTrans.dtCommonSearch.ContractCode
                //        };
                //    }
                //}
                if (param.CommonSearch != null)
                {
                    if (CommonUtil.IsNullOrEmpty(param.CommonSearch.ContractCode) == false)
                    {
                        param.Condition = new FlowMenuCondition()
                        {
                            ContractCode = param.CommonSearch.ContractCode
                        };
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<FlowMenuScreenParameter>("CMS021", param, res);
        }

        #endregion
        #region Views

        /// <summary>
        /// Generate screen
        /// </summary>
        /// <returns></returns>
        [Initialize("CMS021")]
        public ActionResult CMS021()
        {
            try
            {
                FlowMenuScreenParameter param = GetScreenObject<FlowMenuScreenParameter>();
                if (param != null)
                {
                    if (param.Condition != null)
                        ViewBag.ContractCode = param.Condition.ContractCode;
                }

                ViewBag.ScreenID = "CMS021";
            }
            catch (Exception)
            {
            }

            return View();
        }

        #endregion
        #region Actions

        /// <summary>
        /// Getting flow menu id
        /// </summary>
        /// <param name="cond"></param>
        public void CMS021_SelectFlowMenuID(FlowMenu cond)
        {
            try
            {
                if (cond.LinkControl == FlowMenu.LINK_CONTROL.REGISTER_NEW_CONTRACT)
                {
                    cond.Controller = MessageUtil.MODULE_CONTRACT;
                    cond.ObjectID = "CTS020";
                    cond.SubObjectID = "0";
                }
                else if (cond.LinkControl == FlowMenu.LINK_CONTROL.CANCEL_CONTRACT_BEFORE_START)
                {
                    cond.Controller = MessageUtil.MODULE_CONTRACT;
                    cond.ObjectID = "CTS090";
                }
                else if (cond.LinkControl == FlowMenu.LINK_CONTROL.REGISTER_CONTRACT_CHANGE)
                {
                    cond.Controller = MessageUtil.MODULE_COMMON;
                    cond.ObjectID = "CMS026";
                    cond.PopupSubMenuID = "P04";
                    cond.SetSubController("CTS", MessageUtil.MODULE_CONTRACT);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}
