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
        private const string CMS300_Screen = "CMS300";

        /// <summary>
        /// Check parameter valid.
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CMS300_Authority(CMS300_ScreenParameter cond) // IN parameter: ContractCode , ServiceTypeCode
        {

            ObjectResultData res = new ObjectResultData();
            try
            {
                // Check parameter is OK ?
                //if ((ModelState.IsValid == false) && ( CommonUtil.IsNullOrEmpty(cond.ContractCode) == true || CommonUtil.IsNullOrEmpty(cond.ServiceTypeCode) == true))
                //{
                //    ValidatorUtil.BuildErrorMessage(res, this);
                //    return Json(res);
                //}
                if (CommonUtil.IsNullOrEmpty(cond.ContractCode))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0040);
                    return Json(res);
                }

                doContractInfoCondition param = new doContractInfoCondition();
                // Check parameter is OK ?
                //if (CommonUtil.IsNullOrEmpty(cond.ContractCode) == false && CommonUtil.IsNullOrEmpty(cond.ServiceTypeCode) == false) {
                if (CommonUtil.IsNullOrEmpty(cond.ContractCode) == false)
                {
                    param = new doContractInfoCondition()
                    {
                        ContractCode = cond.ContractCode,
                        ServiceTypeCode = cond.ServiceTypeCode
                    };
                }

                return InitialScreenEnvironment<CMS300_ScreenParameter>(CMS300_Screen, cond, res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }

        }

        /// <summary>
        /// Initial screen CMS300.
        /// </summary>
        /// <returns></returns>
        [Initialize(CMS300_Screen)]
        public ActionResult CMS300()
        {
            return View();
        }

        /// <summary>
        /// Get config for Change Customer History List table.
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS300_InitSiteGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS300"));
        }

        /// <summary>
        /// Search Customer Change History list by condition from caller screen.
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <param name="strCSCustCode"></param>
        /// <param name="strRCCustCode"></param>
        /// <param name="strSiteCode"></param>
        /// <returns></returns>
        public ActionResult CMS300_Search(string strContractCode, string strOCC, string strCSCustCode, string strRCCustCode, string strSiteCode)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {

                strContractCode = strContractCode == "" ? null : strContractCode;
                strOCC = strOCC == "" ? null : strOCC;
                strCSCustCode = strCSCustCode == "" ? null : strCSCustCode;
                strRCCustCode = strRCCustCode == "" ? null : strRCCustCode;
                strSiteCode = strSiteCode == "" ? null : strSiteCode;

                CommonUtil util = new CommonUtil();
                strContractCode = util.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                strCSCustCode = util.ConvertCustCode(strCSCustCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                strRCCustCode = util.ConvertCustCode(strRCCustCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                strSiteCode = util.ConvertSiteCode(strSiteCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                IViewContractHandler hand = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
                List<dtChangedCustHistList> list = hand.GetChangedCustHistList(strContractCode,
                    strOCC,
                    strCSCustCode,
                    strRCCustCode,
                    strSiteCode,
                    MiscType.C_CONTRACT_SIGNER_TYPE,
                    MiscType.C_CHANGE_NAME_REASON_TYPE);
                CommonUtil.MappingObjectLanguage<dtChangedCustHistList>(list);

                foreach (var i in list)
                {
                    i.CSCustCode = util.ConvertCustCode(i.CSCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    i.RCCustCode = util.ConvertCustCode(i.RCCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    i.SiteCode = util.ConvertCustCode(i.SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                }

                string xml = CommonUtil.ConvertToXml<dtChangedCustHistList>(list, "Common\\CMS300", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                res.ResultData = xml;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Search detail of Contract selected by user.
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="SequenceNo"></param>
        /// <returns></returns>
        public ActionResult CMS300_SearchDetail(string ContractCode, int? SequenceNo)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                IViewContractHandler hand = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
                List<dtChangedCustHistDetail> list = hand.GetChangedCustHistDetail(ContractCode,
                    SequenceNo,
                    MiscType.C_CONTRACT_SIGNER_TYPE,
                    MiscType.C_CUST_STATUS,
                    MiscType.C_CUST_TYPE,
                    MiscType.C_FINANCIAL_MARKET_TYPE,
                    MiscType.C_CHANGE_NAME_REASON_TYPE);
                CommonUtil.MappingObjectLanguage<dtChangedCustHistDetail>(list);

                if (list != null && list.Count != 0)
                {
                    if (list[0].SiteCode != null && list[0].SiteCode.Trim() == "")
                    {
                        list[0].SiteCode = null;
                    }

                    if (list[0].CSCustCode != null && list[0].CSCustCode.Trim() == "")
                    {
                        list[0].CSCustCode = null;
                    }

                    if (list[0].RCCustCode != null && list[0].RCCustCode.Trim() == "")
                    {
                        list[0].RCCustCode = null;
                    }

                    // Conver to short format
                    CommonUtil cm = new CommonUtil();
                    list[0].SiteCode = cm.ConvertSiteCode(list[0].SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    list[0].CSCustCode = cm.ConvertCustCode(list[0].CSCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    list[0].RCCustCode = cm.ConvertCustCode(list[0].RCCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);


                    res.ResultData = list[0];
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

    }
}
