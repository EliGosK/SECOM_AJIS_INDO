

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
        /// Check suspend, authority and resume of CMS410
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CMS410_Authority(CMS410_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                // Check permission
                if (CheckUserPermission(ScreenID.C_SCREEN_ID_VIEW_BILLING_TARGET_INFORMATION, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                // is parameter OK ?
                if (CommonUtil.IsNullOrEmpty(param.BillingTargetCode))
                {
                    //res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0040);
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0154);
                    return Json(res);
                }

                // Check data exist
                CommonUtil cm = new CommonUtil();
                string strBillingTargetCode = cm.ConvertBillingTargetCode(param.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                IBillingHandler handler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                List<dtTbt_BillingTargetForView> billingTargetData = handler.GetTbt_BillingTargetForView(strBillingTargetCode, MiscType.C_CUST_TYPE);
                if (billingTargetData != null)
                {
                    if (billingTargetData.Count == 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001); // data not found
                        return Json(res);
                    }
                }
                return InitialScreenEnvironment<CMS410_ScreenParameter>("CMS410", param, res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Initial screen CMS410
        /// </summary>
        /// <returns></returns>
        [Initialize("CMS410")]
        public ActionResult CMS410()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CMS410_ScreenParameter param = GetScreenObject<CMS410_ScreenParameter>();
                CommonUtil cm = new CommonUtil();

                string strBillingTargetCode_short = param.BillingTargetCode;
                string strBillingTargetCode = cm.ConvertBillingTargetCode(strBillingTargetCode_short, CommonUtil.CONVERT_TYPE.TO_LONG);

                IBillingHandler handler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                List<dtTbt_BillingTargetForView> billingTargetData = handler.GetTbt_BillingTargetForView(strBillingTargetCode, MiscType.C_CUST_TYPE);
                
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                if (billingTargetData != null)
                {
                    if (billingTargetData.Count > 0)
                    {
                        //Language mapping
                        CommonUtil.MappingObjectLanguage<dtTbt_BillingTargetForView>(billingTargetData);

                        //Misc mapping
                        MiscTypeMappingList miscMapping = new MiscTypeMappingList();
                        miscMapping.AddMiscType(billingTargetData.ToArray<dtTbt_BillingTargetForView>());
                        handlerCommon.MiscTypeMappingList(miscMapping);

                        ViewBag.BillingTargetCode = strBillingTargetCode_short;
                        ViewBag.txtBillingTargetCode = strBillingTargetCode_short;
                        ViewBag.txtBillingOffice = CommonUtil.TextCodeName(billingTargetData[0].BillingOfficeCode, billingTargetData[0].OfficeName);
                        ViewBag.txtCustomerType = CommonUtil.TextCodeName(billingTargetData[0].CustTypeCode, billingTargetData[0].CustTypeName);
                        ViewBag.txtNameEN = billingTargetData[0].FullNameEN;
                        ViewBag.txtBranchNameEN = billingTargetData[0].BranchNameEN;
                        ViewBag.txtAddressEN = billingTargetData[0].AddressEN;
                        ViewBag.txtNameLC = billingTargetData[0].FullNameLC;
                        ViewBag.txtBranchNameLC = billingTargetData[0].BranchNameLC;
                        ViewBag.txtAddressLC = billingTargetData[0].AddressLC;
                        ViewBag.txtNationality = billingTargetData[0].Nationality;
                        ViewBag.txtPhoneNo = billingTargetData[0].PhoneNo;
                        ViewBag.txtIDNo = billingTargetData[0].IDNo;
                        ViewBag.txtBusinessType = billingTargetData[0].BusinessTypeName;
                        ViewBag.txtContactPersonName = billingTargetData[0].ContactPersonName;
                        ViewBag.txtMemo = billingTargetData[0].Memo;
                        ViewBag.txtIssueInvoiceTiming = string.Format("{0} {1}", billingTargetData[0].IssueInvTimeName, billingTargetData[0].IssueInvMonth.HasValue ? billingTargetData[0].IssueInvMonth.Value.ToString() : ""); ;
                        string IssueInvDate = string.Empty;
                        if (billingTargetData[0].IssueInvDate.HasValue)
                        {
                            if (billingTargetData[0].IssueInvDate == 1)
                            {
                                IssueInvDate = string.Format("{0}st", billingTargetData[0].IssueInvDate.ToString());
                            }
                            else if (billingTargetData[0].IssueInvDate == 2)
                            {
                                IssueInvDate = string.Format("{0}nd", billingTargetData[0].IssueInvDate.ToString());
                            }
                            else
                            {
                                IssueInvDate = string.Format("{0}th", billingTargetData[0].IssueInvDate.ToString());
                            }
                        }

                        ViewBag.txtIssueInvoiceDate = IssueInvDate;
                        ViewBag.txtInvoiceFormat = billingTargetData[0].InvFormatTypeName;
                        ViewBag.txtSignatureType = billingTargetData[0].SignatureTypeName;
                        ViewBag.txtShowPaymentDueDate = billingTargetData[0].ShowDueDateName;
                        ViewBag.txtIssueTaxInvoiceReceiptTiming = billingTargetData[0].IssueReceiptTimingName;
                        ViewBag.txtShowAutoTransferAccount = billingTargetData[0].ShowAccTypeName;
                        ViewBag.txtAutoTransferWhtDeductionType = billingTargetData[0].WhtDeductionTypeName;
                        ViewBag.txtDisplayLanguage = billingTargetData[0].DocLanguageName;
                        ViewBag.txtPrintIssueDate = billingTargetData[0].ShowIssueDateName;
                        ViewBag.txtSeparateInvoiceType = billingTargetData[0].SeparateInvTypeName;
                        ViewBag.txtSupplementaryInvoiceAddress = billingTargetData[0].SuppleInvAddress;

                        ViewBag.PayByChequeFlag = billingTargetData[0].PayByChequeFlag.HasValue == false ? false : billingTargetData[0].PayByChequeFlag.Value;
                        ViewBag.PrintInvoiceWHTFlag = billingTargetData[0].ShowInvWHTFlag.HasValue == false ? false : billingTargetData[0].ShowInvWHTFlag.Value;

                        ViewBag.txtRealBillingClientNameEN = billingTargetData[0].RealBillingClientNameEN;
                        ViewBag.txtRealBillingClientAddressEN = billingTargetData[0].RealBillingClientAddressEN;
                        ViewBag.txtRealBillingClientNameLC = billingTargetData[0].RealBillingClientNameLC;
                        ViewBag.txtRealBillingClientAddressLC = billingTargetData[0].RealBillingClientAddressLC;
                    }
                    else
                    {
                        ViewBag.PayByChequeFlag = false;
                        ViewBag.PrintInvoiceWHTFlag = false;
                    }
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
