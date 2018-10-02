//*********************************
// Create by: Narupon W.
// Create date: 5/Jul/2010
// Update date: 5/Jul/2010
//*********************************


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
using System.Diagnostics;

namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {
        /// <summary>
        /// Check permission for access screen CMS050
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CMS050_Authority(CMS050_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_RUN_BATCH, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<CMS050_ScreenParameter>("CMS050", param, res);
        }

        /// <summary>
        /// Method for return view of screen CMS050
        /// </summary>
        /// <returns></returns>
        [Initialize("CMS050")]
        public ActionResult CMS050()
        {
            return View();
        }

        /// <summary>
        ///  Initial grid of screen CMS050
        /// </summary>
        /// <returns></returns>
        public ActionResult InitialGrid_CMS050()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS050"));
        }

        /// <summary>
        /// Get all batch process status
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS050_SearchResponse()
        {
            List<dtBatchProcess> list = new List<dtBatchProcess>();
            List<View_dtBatchProcess> nlst = new List<View_dtBatchProcess>();
            ObjectResultData res = new ObjectResultData();

            try
            {

                ILogHandler handler = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                list = handler.GetBatchProcessDataList(ConfigName.C_CONFIG_SUSPEND_FLAG,
                                                                            MiscType.C_BATCH_STATUS,
                                                                            MiscType.C_BATCH_LAST_RESULT,
                                                                            BatchStatus.C_BATCH_STATUS_PROCESSING,
                                                                            FlagType.C_FLAG_ON,
                                                                            FlagType.C_FLAG_OFF);



                // Select by language
                nlst = CommonUtil.ConvertObjectbyLanguage<dtBatchProcess, View_dtBatchProcess>(list, "BatchStatusName", "BatchLastResultName");

                // tt
                if (BatchProcessUtil.CheckJobRunning("SECOM_AJIS_BatchAll") == true)
                {
                    foreach (var item in nlst)
                    {
                        item.EnableRun = false;
                    }
                }

            }
            catch (Exception ex)
            {

                nlst = new List<View_dtBatchProcess>();
                res.AddErrorMessage(ex);
                return Json(res);
            }

            res.ResultData = CommonUtil.ConvertToXml<View_dtBatchProcess>(nlst, "Common\\CMS050", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            return Json(res);

        }

        /// <summary>
        ///  Method for execute batch process (each)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult CMS050_Runbatch(doBatchProcess data)
        {

            List<string> lsRunBatchFailed = new List<string>();
            List<int?> list = new List<int?>();


            try
            {
                IBatchProcessHandler handler = ServiceContainer.GetService<IBatchProcessHandler>() as IBatchProcessHandler;

                if (data != null)
                {

                    if (data.isRunAll == false) // Run selected batch
                    {

                        data.BatchDate = DateTime.Now;
                        data.BatchUser = CommonUtil.dsTransData.dtUserData.EmpNo;
                        handler.RunProcess(data);


                    }
                    else // ...
                    {

                    }
                }


                return Json(1);

            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res);
            }


        }

        /// <summary>
        /// Method for execute all batch process
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS050_RunbatchAll()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (BatchProcessUtil.CheckJobRunning("SECOM_AJIS_BatchAll"))
                {
                    // btnRunBatchAll
                    string lblRunBatchAll = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, "CMS050", "btnRunBatchAll");
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0031, new string[] { lblRunBatchAll });
                    return Json(res);
                }
                IBatchProcessHandler handler = ServiceContainer.GetService<IBatchProcessHandler>() as IBatchProcessHandler;
                handler.RunProcessAll();

                return Json(1);
            }
            catch (Exception ex)
            {

                res.AddErrorMessage(ex);
                return Json(res);
            }

        }

    }
}
