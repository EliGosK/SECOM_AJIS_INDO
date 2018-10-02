//*********************************
// Create by: Narupon W.
// Create date: /Jun/2011
// Update date: /Jun/2011
//*********************************

using System;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.ActionFilters;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Presentation.Common.Models;

using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Contract;
using CSI.WindsorHelper;

using SECOM_AJIS.Common.Util.ConstantValue;



using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {
        /// <summary>
        /// Check permission for access screen CMS131
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CMS131_Authority(CMS131_ScreenParameter param) // IN parameter: string strContractCode, string strOCC
        {
            ObjectResultData res = new ObjectResultData();


            // Check parameter is OK ?
            if (CommonUtil.IsNullOrEmpty(param.strContractCode) == false && CommonUtil.IsNullOrEmpty(param.strOCC) == false)
            {
                param.ContractCode = param.strContractCode;
                param.OCC = param.strOCC;
            }
            else
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0040);
                return Json(res);
            }

            // Check exist data
            try
            {
                CommonUtil c = new CommonUtil();
                string ContractCode = c.ConvertContractCode(param.strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);


                IRentralContractHandler handler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                List<dtTbt_RentalContractBasicForView> dtRentalContract = handler.GetTbt_RentalContractBasicForView(ContractCode);

                if (dtRentalContract.Count == 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                    return Json(res);
                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }


            return InitialScreenEnvironment<CMS131_ScreenParameter>("CMS131", param, res);
        }

        /// <summary>
        /// Method for return view of screen CMS131
        /// </summary>
        /// <returns></returns>
        [Initialize("CMS131")]
        public ActionResult CMS131()
        {
            string strContractCode = "";
            string strOCC = "";

            try
            {
                CMS131_ScreenParameter param = GetScreenObject<CMS131_ScreenParameter>();
                strContractCode = param.ContractCode;
                strOCC = param.OCC;
            }
            catch
            {
            }

            ViewBag.ContractCode = strContractCode;
            ViewBag.Occurrence = strOCC;

            CommonUtil c = new CommonUtil();
            strContractCode = c.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);


            List<dtTbt_RentalContractBasicForView> vw_dtRentalContract = new List<dtTbt_RentalContractBasicForView>();
            List<dtTbt_RentalSecurityBasicForView> vw_dtRentalSecurity = new List<dtTbt_RentalSecurityBasicForView>();

            try
            {
                IRentralContractHandler handler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                List<dtTbt_RentalContractBasicForView> dtRentalContract = handler.GetTbt_RentalContractBasicForView(strContractCode);
                List<dtTbt_RentalSecurityBasicForView> dtRentalSecurity = handler.GetTbt_RentalSecurityBasicForView(strContractCode, CommonUtil.IsNullOrEmpty(strOCC) == true ? null : strOCC);


                // Select language
                vw_dtRentalContract = CommonUtil.ConvertObjectbyLanguage<dtTbt_RentalContractBasicForView, dtTbt_RentalContractBasicForView>(dtRentalContract, "Quo_OfficeName", "Con_OfficeName", "Op_OfficeName");
                vw_dtRentalSecurity = CommonUtil.ConvertObjectbyLanguage<dtTbt_RentalSecurityBasicForView, dtTbt_RentalSecurityBasicForView>(dtRentalSecurity, "NegStaff1_EmpFirstName",
                                                                                                                                                                "NegStaff1_EmpLastName",
                                                                                                                                                                "NegStaff2_EmpFirstName",
                                                                                                                                                                "NegStaff2_EmpLastName");

                if (vw_dtRentalContract.Count > 0)
                {
                    string txtContractOffice = CommonUtil.TextCodeName(vw_dtRentalContract[0].ContractOfficeCode, vw_dtRentalContract[0].Con_OfficeName);
                    ViewBag.txtContractOffice = CommonUtil.IsNullOrEmpty(txtContractOffice) == true ? "-" : txtContractOffice;
                    string txtOperationOffice = CommonUtil.TextCodeName(vw_dtRentalContract[0].OperationOfficeCode, vw_dtRentalContract[0].Op_OfficeName);
                    ViewBag.txtOperationOffice = CommonUtil.IsNullOrEmpty(txtOperationOffice) == true ? "-" : txtOperationOffice;
                }

                if (vw_dtRentalSecurity.Count > 0)
                {
                    string txtNegotiationStaff1 = CommonUtil.TextCodeName(vw_dtRentalSecurity[0].NegotiationStaffEmpNo1, string.Format("{0} {1}", vw_dtRentalSecurity[0].NegStaff1_EmpFirstName, vw_dtRentalSecurity[0].NegStaff1_EmpLastName));
                    ViewBag.txtNegotiationStaff1 = CommonUtil.IsNullOrEmpty(txtNegotiationStaff1) == true ? "-" : txtNegotiationStaff1;
                    string txtNegotiationStaff2 = CommonUtil.TextCodeName(vw_dtRentalSecurity[0].NegotiationStaffEmpNo2, string.Format("{0} {1}", vw_dtRentalSecurity[0].NegStaff2_EmpFirstName, vw_dtRentalSecurity[0].NegStaff2_EmpLastName));
                    ViewBag.txtNegotiationStaff2 = CommonUtil.IsNullOrEmpty(txtNegotiationStaff2) == true ? "-" : txtNegotiationStaff2;
                }

                return View();
            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res);
            }



        }

        /// <summary>
        /// Initial grid of screen CMS131
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS131_InitialGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS131"));
        }

        /// <summary>
        /// Get contract document data by contract code , OCC
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <returns></returns>
        public ActionResult CMS131_SearchResponse(string strContractCode, string strOCC)
        {
            CommonUtil c = new CommonUtil();

            ObjectResultData res = new ObjectResultData();
            List<View_dtContractDocument> nlst = new List<View_dtContractDocument>();


            try
            {
                strContractCode = c.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                IViewContractHandler handler = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
                List<dtContractDocument> list = handler.GetContractDocDataListForView(strContractCode, strOCC, null, null);


                foreach (dtContractDocument item in list)
                {
                    nlst.Add(CommonUtil.CloneObject<dtContractDocument, View_dtContractDocument>(item));
                }



                // Select by language
                nlst = CommonUtil.ConvertObjectbyLanguage<View_dtContractDocument, View_dtContractDocument>(nlst,
                                                                                                            "DocumentTypeName",
                                                                                                            "DocStatusName_Extra",
                                                                                                            "DocAuditResultName_Extra",
                                                                                                            "ContractOfficeName_Extra",
                                                                                                            "DocumentName",
                                                                                                            "OperationOfficeName_Extra",
                                                                                                            "Saleman1"
                                                                                                            );



                //return Json(CommonUtil.ConvertToXml<View_dtContractDocument>(nlst, "Common\\CMS131"));

            }
            catch (Exception ex)
            {
                nlst = new List<View_dtContractDocument>();
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            res.ResultData = CommonUtil.ConvertToXml<View_dtContractDocument>(nlst, "Common\\CMS131");
            return Json(res);

        }


    }
}
