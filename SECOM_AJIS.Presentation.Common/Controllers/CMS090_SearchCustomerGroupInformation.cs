//*********************************
// Create by: 
// Create date: /Jun/2010
// Update date: /Jun/2010
//*********************************


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Xml;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.ActionFilters;

using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Common;
using CSI.WindsorHelper;

using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.Presentation.Common.Models;

namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {
        private const string CMS090_Screen = "CMS090";

        /// <summary>
        /// Check user permission for screen CMS090.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ActionResult CMS090_Authority(CMS090_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_SEARCH_CUST_GROUP, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                if (res.IsError)
                    return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<CMS090_ScreenParameter>("CMS090", param, res);
        }

        /// <summary>
        /// Initial screen CMS090
        /// </summary>
        /// <returns></returns>
        [Initialize(CMS090_Screen)]
        public ActionResult CMS090(string strCustomerCode, string strCustomerRole)
        {
            //CMS090_ScreenParameter param = GetScreenObject<CMS090_ScreenParameter>();
            //ViewBag.HasPermissionCMS100 = param.hasPermission_CMS100;
            
            return View();
        }

        /// <summary>
        /// Get config for Customer Group List table.
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS090_InitGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS090"));
        }

        /// <summary>
        /// Validate search criteria.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public ActionResult CMS090_ValidateSearch(doCMS090_SearchCondition condition) {
            ObjectResultData res = new ObjectResultData();
            try {

                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                ValidatorUtil.BuildErrorMessage(res, this, new object[] { condition });

                if (!res.IsError) {
                    res.ResultData = "P";
                }

            } catch (Exception ex) {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Search Customer Group by search condition.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public ActionResult CMS090_Search(doCMS090_SearchCondition condition)
        {
            ObjectResultData res = new ObjectResultData();
            try {
                IViewContractHandler hand = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
                List<dtGroupList> list = hand.GetGroupListForSearchCustGrp(
                                            condition.GroupCode, 
                                            condition.GroupName, 
                                            condition.OfficeInCharge, 
                                            condition.PersonInCharge, 
                                            condition.NumberOfCustomerFrom, 
                                            condition.NumberOfCustomerTo, 
                                            condition.NumberOfSiteFrom, 
                                            condition.NumberOfSiteTo,
                                            ContractStatus.C_CONTRACT_STATUS_BEF_START,
                                            ContractStatus.C_CONTRACT_STATUS_CANCEL,
                                            ContractStatus.C_CONTRACT_STATUS_AFTER_START,
                                            FlagType.C_FLAG_ON);
                CommonUtil.MappingObjectLanguage<dtGroupList>(list);

                List<View_dtGroupList> result = new List<View_dtGroupList>();
                foreach (var i in list) {
                    //4.4.3	ConvToShortGroupCode()
                    View_dtGroupList item = new View_dtGroupList();
                    item.GroupCode = i.GroupCode;
                    item.CountCust = i.CountCust;
                    item.CountSite = i.CountSite;
                    item.GroupNameDisplay = "(1) " + i.GroupNameEN + "<br/>(2) " + i.GroupNameLC;

                    item.OfficeInCharge = i.OfficeName;
                    item.PersonInCharge = i.EmpFirstName + " " + i.EmpLastName;

                    result.Add(item);
                }

                string xml = CommonUtil.ConvertToXml<View_dtGroupList>(result, "Common\\CMS090", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                res.ResultData = xml;
            } catch (Exception ex) {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

    }
}
