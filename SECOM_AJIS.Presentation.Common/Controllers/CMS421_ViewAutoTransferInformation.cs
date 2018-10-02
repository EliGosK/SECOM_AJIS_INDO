using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;


using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Common;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.ActionFilters;
using CSI.WindsorHelper;

using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Presentation.Common.Models;

using SECOM_AJIS.DataEntity.Billing;

namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {
        /// <summary>
        /// Check suspend, authority and resume of CMS421
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CMS421_Authority(CMS421_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                // Check permission
                //if (CheckUserPermission(ScreenID.C_SCREEN_ID_VIEW_AUTO_TRANSFER_INFORMATION, FunctionID.C_FUNC_ID_VIEW) == false)
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                //    return Json(res);
                //}

                // is parameter OK ?
                if (CommonUtil.IsNullOrEmpty(param.ContractCode) || CommonUtil.IsNullOrEmpty(param.BillingOCC))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0040);
                    return Json(res);
                }

                // Check data exist
                CommonUtil cm = new CommonUtil();
                string strContractCode = cm.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                IBillingHandler handler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                List<dtTbt_AutoTransferBankAccountForView> listAutoTransferBankAccountData = handler.GetTbt_AutoTransferBankAccountForView(strContractCode, param.BillingOCC);
                if (listAutoTransferBankAccountData.Count == 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001); // data not found
                    return Json(res);
                }

                return InitialScreenEnvironment<CMS420_ScreenParameter>("CMS421", param, res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Initialize screen of CMS421
        /// </summary>
        /// <returns></returns>
        [Initialize("CMS421")]
        public ActionResult CMS421()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CMS421_ScreenParameter param = GetScreenObject<CMS421_ScreenParameter>();
                CommonUtil cm = new CommonUtil();

                string strContractCode = cm.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                IBillingHandler handler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                List<dtTbt_AutoTransferBankAccountForView> listAutoTransferBankAccountData = handler.GetTbt_AutoTransferBankAccountForView(strContractCode, param.BillingOCC);

                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                
                if (listAutoTransferBankAccountData.Count > 0)
                {
                    //Misc mapping
                    MiscTypeMappingList miscMapping = new MiscTypeMappingList();
                    miscMapping.AddMiscType(listAutoTransferBankAccountData.ToArray<dtTbt_AutoTransferBankAccountForView>());
                    handlerCommon.MiscTypeMappingList(miscMapping);
                }

                if (listAutoTransferBankAccountData.Count > 0)
                {
                    ViewBag.txtBillingCode = listAutoTransferBankAccountData[0].BillingCode_Short;
                    ViewBag.txtBillingClientCode = listAutoTransferBankAccountData[0].BillingClientCode_Short;
                    ViewBag.txtBillingClientNameEN = listAutoTransferBankAccountData[0].FullNameEN;
                    ViewBag.txtBillingClientNameLC = listAutoTransferBankAccountData[0].FullNameLC;
                    ViewBag.txtAutoTransferAccountName = listAutoTransferBankAccountData[0].AccountName;
                    ViewBag.txtBankBranch = listAutoTransferBankAccountData[0].BankNameEN + "/" + listAutoTransferBankAccountData[0].BankBranchNameEN;
                    ViewBag.txtAccountType = listAutoTransferBankAccountData[0].AccountTypeName;
                    ViewBag.txtAccountNo = listAutoTransferBankAccountData[0].AccountNo_ForView; // Edit by Narupon W. 28/05/2012
                    ViewBag.txtAutoTransferDate = listAutoTransferBankAccountData[0].AutoTransferDate;
                    ViewBag.txtLastAutoTransferResult = listAutoTransferBankAccountData[0].LastestResultName;
                }

                return View();
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }
    }
}
