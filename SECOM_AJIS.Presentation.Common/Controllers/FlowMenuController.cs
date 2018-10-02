using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Util;
using System.Web.Mvc;
using SECOM_AJIS.Presentation.Common.Models;
using SECOM_AJIS.Common.Models;
using System.Reflection;
using SECOM_AJIS.DataEntity.Contract;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.Presentation.Common.Models
{
    /// <summary>
    /// DO for intial screen
    /// </summary>
    public class FlowMenuScreenParameter : ScreenSearchParameter
    {
        [KeepSession]
        public FlowMenuCondition Condition { get; set; }
        /// <summary>
        /// DO for sale contract data
        /// </summary>
        public doSaleContractDataForFlowMenu doSaleContractDataForFlowMenu { get; set; }
        /// <summary>
        /// DO for rental contract data
        /// </summary>
        public doRentalContractDataForFlowMenu doRentalContractDataForFlowMenu { get; set; }
    }
    /// <summary>
    /// DO for flow menu
    /// </summary>
    public class FlowMenu
    {
        public class SubControllerDo
        {
            public string ModulePrefix { get; set; }
            public string SubController { get; set; }
        }

        public enum LINK_CONTROL
        {
            REGISTER_QUOTATION,
            REGISTER_NEW_CONTRACT,
            REGISTER_GENERATE_CONTRACT_DOCUMENT,
            APPROVE_CONTRACT,
            REGISTER_CONTRACT_DOCUMENT_RECEIVING,
            CHANGE_CONTRACT_FEE,
            CHANGE_NAME_AND_ADDRESS,
            REGISTER_CHANGE_BILLING_BASIC,
            REGISTER_STOP_SERVICE,
            REGISTER_CANCEL_CONTRACT,
            REGISTER_CANCEL_CONTRACT_AFTER_START,
            REGISTER_INSTALLATION_REQUEST,
            REGISTER_INSTALLATION_MANAGEMENT,
            REGISTER_GENERATED_INSTALLATION_SLIP,
            REGISTER_STOCK_OUT_INSTRUMENT,
            REGISTER_CONTRACT_CHANGE,
            REGISTER_CHANGE_INSTALLATION_SLIP,
            REGISTER_COMPLETE_INSTALLATION,
            REGISTER_START_SERVICE,
            REGISTER_CUSTOMER_ACCEPTANCE,
            CUSTOMER_CORRESPONDENT_SUPPORT,
            AR,
            CANCEL_UN_OPERATED_CONTRACT,
            CANCEL_INSTALLATION,
            CANCEL_CONTRACT_BEFORE_START,
            SEARCH_QUOTATION_INFORMATION,
            SEARCH_VIEW_CONTRACT_CUSTOMER_INFORMATION,
            VIEW_INSTALLATION_INFORMATION,
            VIEW_INVENTORY_INFORMATION,
            VIEW_BILLING_INFORMATION,
            SEARCH_INSTALLATION_MANAGEMENT
        }

        public string LinkID { get; set; }
        public LINK_CONTROL LinkControl
        {
            get
            {
                LINK_CONTROL link;

                string id = this.LinkID;
                if (CommonUtil.IsNullOrEmpty(id) == false)
                {
                    if (id.EndsWith("_2"))
                        id = id.Substring(0, id.Length - 2);
                }

                Enum.TryParse<LINK_CONTROL>(id, out link);
                return link;
            }
        }

        public string Controller { get; set; }
        public List<SubControllerDo> SubControllerList { get; set; }
        public string ObjectID { get; set; }
        public string SubObjectID { get; set; }
        public string PopupSubMenuID { get; set; }

        public List<string> Parameters { get; set; }
        public List<string> Values { get; set; }

        public void SetParameter(string parameter, string value)
        {
            if (this.Parameters == null)
            {
                this.Parameters = new List<string>();
                this.Values = new List<string>();
            }
            this.Parameters.Add(parameter);
            this.Values.Add(value);
        }
        public void SetSubController(string modulePrefix, string subController)
        {
            if (this.SubControllerList == null)
                this.SubControllerList = new List<SubControllerDo>();
            this.SubControllerList.Add(new SubControllerDo()
            {
                ModulePrefix = modulePrefix,
                SubController = subController
            });
        }
    }
    /// <summary>
    /// DO for getting data
    /// </summary>
    public class FlowMenuCondition
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_COMMON,
                        Screen = "FlowMenu",
                        Parameter = "lblContractCode",
                        ControlName = "ContractCode")]
        public string ContractCode { get; set; }
        public string ContractCodeLong
        {
            get
            {
                CommonUtil cmm = new CommonUtil();
                return cmm.ConvertContractCode(this.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
            }
        }
    }
}
namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {
        /// <summary>
        /// Retrieve flow menu data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult RetrieveFlowMenu(FlowMenuCondition cond)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                #region Validate required field

                ValidatorUtil.BuildErrorMessage(res, this);
                if (res.IsError == true)
                    return Json(res);

                #endregion
                #region Get basic information

                FlowMenuScreenParameter param = GetScreenObject<FlowMenuScreenParameter>();
                if (param != null)
                {
                    IViewContractHandler chandler = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;

                    if (param.ScreenID == "CMS021")
                    {
                        List<doSaleContractDataForFlowMenu> lst = chandler.GetSaleContractDataForFlowMenu(cond.ContractCodeLong);
                        if (lst.Count == 0)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0124,
                                null,
                                new string[] { "ContractCode" });
                            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                            return Json(res);
                        }

                        param.doSaleContractDataForFlowMenu = lst[0];
                        res.ResultData = param.doSaleContractDataForFlowMenu;
                    }
                    else
                    {
                        List<doRentalContractDataForFlowMenu> lst = chandler.GetRentalContractDataForFlowMenu(cond.ContractCodeLong);
                        if (lst.Count == 0)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0124,
                                null,
                                new string[] { "ContractCode" });
                            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                            return Json(res);
                        }

                        param.doRentalContractDataForFlowMenu = lst[0];
                        res.ResultData = param.doRentalContractDataForFlowMenu;
                    }

                    param.Condition = cond;
                        
                    #region Update search session in dsTrans

                    CommonUtil.dsTransData.dtCommonSearch.ContractCode = cond.ContractCode;

                    #endregion
                }

                #endregion
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Getting flow menu id
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult SelectFlowMenuID(FlowMenu cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                cond.SubObjectID = "0";

                FlowMenuScreenParameter param = GetScreenObject<object>() as FlowMenuScreenParameter;
                if (param != null)
                {
                    string methodName = string.Format("{0}_SelectFlowMenuID", param.ScreenID);
                    MethodInfo method = this.GetType().GetMethod(methodName);
                    if (method != null)
                        method.Invoke(this, new object[] { cond });
                }

                if (cond.ObjectID == null)
                {
                    cond.Controller = MessageUtil.MODULE_COMMON;
                    cond.ObjectID = "CMS026";

                    switch (cond.LinkControl)
                    {
                        case FlowMenu.LINK_CONTROL.REGISTER_QUOTATION:
                            {
                                cond.PopupSubMenuID = "P01";
                                cond.SetSubController("QUS", MessageUtil.MODULE_QUOTATION);
                            } break;
                        case FlowMenu.LINK_CONTROL.APPROVE_CONTRACT:
                            {
                                cond.PopupSubMenuID = "P02";
                                cond.SetSubController("CTS", MessageUtil.MODULE_CONTRACT);
                            } break;
                        case FlowMenu.LINK_CONTROL.REGISTER_STOCK_OUT_INSTRUMENT:
                            {
                                cond.PopupSubMenuID = "P03";
                                cond.SetSubController("IVS", MessageUtil.MODULE_INVENTORY);
                            } break;
                        case FlowMenu.LINK_CONTROL.REGISTER_CONTRACT_CHANGE:
                            {
                                cond.PopupSubMenuID = "P12";
                                cond.SetSubController("CTS", MessageUtil.MODULE_CONTRACT);
                            } break;
                        case FlowMenu.LINK_CONTROL.CUSTOMER_CORRESPONDENT_SUPPORT:
                            {
                                cond.PopupSubMenuID = "P05";
                                cond.SetSubController("CTS", MessageUtil.MODULE_CONTRACT);

                                if (param.Condition != null)
                                {
                                    cond.SetParameter("strIncidentRelevantType", IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CONTRACT);
                                    cond.SetParameter("strIncidentRelevantCode", param.Condition.ContractCode);
                                }
                            } break;
                        case FlowMenu.LINK_CONTROL.AR:
                            {
                                cond.PopupSubMenuID = "P06";
                                cond.SetSubController("CTS", MessageUtil.MODULE_CONTRACT);

                                if (param.Condition != null)
                                {
                                    cond.SetParameter("strARRelevantType", ARRelevant.C_AR_RELEVANT_TYPE_CONTRACT);
                                    cond.SetParameter("strARRelevantCode", param.Condition.ContractCode);
                                }
                            } break;
                        case FlowMenu.LINK_CONTROL.SEARCH_QUOTATION_INFORMATION:
                            {
                                cond.Controller = MessageUtil.MODULE_QUOTATION;
                                cond.ObjectID = "QUS010";
                            } break;
                        case FlowMenu.LINK_CONTROL.SEARCH_VIEW_CONTRACT_CUSTOMER_INFORMATION:
                            {
                                cond.PopupSubMenuID = "P08";
                                cond.SetSubController("CMS", MessageUtil.MODULE_COMMON);
                            } break;
                        case FlowMenu.LINK_CONTROL.VIEW_INSTALLATION_INFORMATION:
                            {
                                cond.PopupSubMenuID = "P09";
                                cond.SetSubController("CMS", MessageUtil.MODULE_COMMON);
                                cond.SetSubController("ISS", MessageUtil.MODULE_INSTALLATION); 
                            } break;
                        case FlowMenu.LINK_CONTROL.VIEW_INVENTORY_INFORMATION:
                            {
                                cond.PopupSubMenuID = "P10";
                                cond.SetSubController("IVS", MessageUtil.MODULE_INVENTORY);
                            } break;
                        case FlowMenu.LINK_CONTROL.VIEW_BILLING_INFORMATION:
                            {
                                cond.PopupSubMenuID = "P11";
                                cond.SetSubController("CMS", MessageUtil.MODULE_COMMON);
                            } break;

                        case FlowMenu.LINK_CONTROL.REGISTER_GENERATE_CONTRACT_DOCUMENT:
                            {
                                cond.Controller = MessageUtil.MODULE_CONTRACT;
                                cond.ObjectID = "CTS160";
                            } break;
                        case FlowMenu.LINK_CONTROL.REGISTER_CONTRACT_DOCUMENT_RECEIVING:
                            {
                                cond.Controller = MessageUtil.MODULE_CONTRACT;
                                cond.ObjectID = "CTS190";
                            } break;
                        case FlowMenu.LINK_CONTROL.CHANGE_CONTRACT_FEE:
                            {
                                cond.Controller = MessageUtil.MODULE_CONTRACT;
                                cond.ObjectID = "CTS053";
                            } break;
                        case FlowMenu.LINK_CONTROL.CHANGE_NAME_AND_ADDRESS:
                            {
                                cond.Controller = MessageUtil.MODULE_CONTRACT;
                                cond.ObjectID = "CTS130";
                            } break;
                        case FlowMenu.LINK_CONTROL.REGISTER_CHANGE_BILLING_BASIC:
                            {
                                cond.Controller = MessageUtil.MODULE_BILLING;
                                cond.ObjectID = "BLS040";
                            } break;
                        case FlowMenu.LINK_CONTROL.REGISTER_STOP_SERVICE:
                            {
                                cond.Controller = MessageUtil.MODULE_CONTRACT;
                                cond.ObjectID = "CTS100";
                            } break;
                        case FlowMenu.LINK_CONTROL.REGISTER_CANCEL_CONTRACT:
                            {
                                cond.Controller = MessageUtil.MODULE_CONTRACT;
                                cond.ObjectID = "CTS110";
                            } break;
                        case FlowMenu.LINK_CONTROL.REGISTER_CANCEL_CONTRACT_AFTER_START:
                            {
                                cond.Controller = MessageUtil.MODULE_CONTRACT;
                                cond.ObjectID = "CTS110";
                                cond.SubObjectID = "1";
                            } break;
                        case FlowMenu.LINK_CONTROL.REGISTER_INSTALLATION_REQUEST:
                            {
                                cond.Controller = MessageUtil.MODULE_INSTALLATION;
                                cond.ObjectID = "ISS010";
                            } break;
                        case FlowMenu.LINK_CONTROL.REGISTER_INSTALLATION_MANAGEMENT:
                            {
                                cond.Controller = MessageUtil.MODULE_INSTALLATION;
                                cond.ObjectID = "ISS050";
                            } break;
                        case FlowMenu.LINK_CONTROL.REGISTER_GENERATED_INSTALLATION_SLIP:
                            {
                                cond.Controller = MessageUtil.MODULE_INSTALLATION;
                                cond.ObjectID = "ISS030";
                            } break;
                        case FlowMenu.LINK_CONTROL.REGISTER_CHANGE_INSTALLATION_SLIP:
                            {
                                cond.Controller = MessageUtil.MODULE_INSTALLATION;
                                cond.ObjectID = "ISS030";
                            } break;
                        case FlowMenu.LINK_CONTROL.REGISTER_COMPLETE_INSTALLATION:
                            {
                                cond.Controller = MessageUtil.MODULE_INSTALLATION;
                                cond.ObjectID = "ISS060";
                            } break;
                        case FlowMenu.LINK_CONTROL.REGISTER_START_SERVICE:
                            {
                                cond.Controller = MessageUtil.MODULE_CONTRACT;
                                cond.ObjectID = "CTS070";
                            } break;
                        case FlowMenu.LINK_CONTROL.REGISTER_CUSTOMER_ACCEPTANCE:
                            {
                                cond.Controller = MessageUtil.MODULE_BILLING;
                                cond.ObjectID = "BLS070";

                                if (param.doSaleContractDataForFlowMenu != null)
                                {
                                    cond.SetParameter("OCC", param.doSaleContractDataForFlowMenu.OCC);
                                    cond.SetParameter("ProcessType", InvoiceProcessType.C_INV_PROCESS_TYPE_ISSUE_SALE);
                                }
                            } break;
                        case FlowMenu.LINK_CONTROL.CANCEL_UN_OPERATED_CONTRACT:
                            {
                                cond.Controller = MessageUtil.MODULE_CONTRACT;
                                cond.ObjectID = "CTS055";
                            } break;
                        case FlowMenu.LINK_CONTROL.CANCEL_INSTALLATION:
                            {
                                cond.Controller = MessageUtil.MODULE_INSTALLATION;
                                cond.ObjectID = "ISS070";
                            } break;
                        case FlowMenu.LINK_CONTROL.CANCEL_CONTRACT_BEFORE_START:
                            {
                                cond.Controller = MessageUtil.MODULE_CONTRACT;
                                cond.ObjectID = "CTS110";
                            } break;
                        case FlowMenu.LINK_CONTROL.SEARCH_INSTALLATION_MANAGEMENT:
                            {
                                cond.Controller = MessageUtil.MODULE_INSTALLATION;
                                cond.ObjectID = "ISS080";
                            } break;
                    }
                }

                res.ResultData = cond;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
    }
}
