//*********************************
// Create by: 
// Create date: /Jun/2010
// Update date: /Jun/2010
//*********************************

using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.Presentation.Contract.Models;
namespace SECOM_AJIS.Presentation.Contract.Controllers
{
    public partial class ContractController : BaseController
    {
        private const string CTS261_Screen = "CTS261";
        private const string CTS261_XML = "contract\\cts261";

        /// <summary>
        /// Authority screen CTS261
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CTS261_Authority(CTS261_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (CommonUtil.IsNullOrEmpty(param.strProjectCode))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0040);
                    return Json(res);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<CTS261_ScreenParameter>(CTS261_Screen, param, res);
        }

        /// <summary>
        /// Initial scrren CTS261
        /// </summary>
        /// <returns></returns>
        [Initialize(CTS261_Screen)]
        public ActionResult CTS261()
        {
            return View();
        }
        /// <summary>
        /// Initial grid project contract
        /// </summary>
        /// <returns></returns>
        public ActionResult InitialGrid_CTS261()
        {

            ObjectResultData res = new ObjectResultData();
            try
            {
                res.ResultData = CommonUtil.ConvertToXml<doProjectContractDetail>(null, CTS261_XML, CommonUtil.GRID_EMPTY_TYPE.VIEW);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Get contract detail data
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS261_GetContractDetail()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CTS261_ScreenParameter param = GetScreenObject<CTS261_ScreenParameter>();
                IProjectHandler ProjectHand = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;

                DataEntity.Common.ICommonHandler comHand = ServiceContainer.GetService<DataEntity.Common.ICommonHandler>() as DataEntity.Common.ICommonHandler;
                List<DataEntity.Common.doMiscTypeCode> currencies = comHand.GetMiscTypeCodeList(new List<DataEntity.Common.doMiscTypeCode>()
                                                    {
                                                        new DataEntity.Common.doMiscTypeCode()
                                                        {
                                                            FieldName = MiscType.C_CURRENCT,
                                                            ValueCode = "%"
                                                        }
                                                    });

                List<doProjectContractDetail> lstProjectContractDetail = new List<doProjectContractDetail>();
                if (param != null && param.strProjectCode != null)
                {
                    lstProjectContractDetail = ProjectHand.GetContractDetailList(param.strProjectCode, MiscType.C_DOC_AUDIT_RESULT);
                    CommonUtil.MappingObjectLanguage<doProjectContractDetail>(lstProjectContractDetail);

                    foreach(doProjectContractDetail pc in lstProjectContractDetail)
                    {
                        pc.Currencies = currencies;
                    }
                }
                res.ResultData = CommonUtil.ConvertToXml<doProjectContractDetail>(lstProjectContractDetail, CTS261_XML, CommonUtil.GRID_EMPTY_TYPE.VIEW);
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }

        }
    }
}
