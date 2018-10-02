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
        private const string CMS290_Screen = "CMS290";

        /// <summary>
        ///  Method for return view of screen CMS290
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CMS290_Authority(CMS290_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
           

            return InitialScreenEnvironment<CMS290_ScreenParameter>(CMS290_Screen, param, res);
        }

        /// <summary>
        /// Method for return view of screen CMS290
        /// </summary>
        /// <returns></returns>
        [Initialize(CMS290_Screen)]
        public ActionResult CMS290()
        {
            ViewBag.PageRow = CommonValue.ROWS_PER_PAGE_FOR_SEARCHPAGE;

            return View(CMS290_Screen);
        }
   
        /// <summary>
        /// Initial grid of screen CMS290
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS290_InitialGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS290", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Check condition that is required field
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CMS290_CheckReqField(CMS290_SearchCondition cond)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                //ApplicationErrorException.CheckMandatoryField(cond); //For Case AtLeast1FieldNotNullOrEmptyAttribute
                ValidatorUtil.BuildErrorMessage(res, this, new object[] { cond }); //AtLeast1FieldNotNullOrEmptyAttribute
                if (res.IsError)
                    return Json(res);

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        
        /// <summary>
        /// Get project data by search condition
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CMS290_Search(doSearchProjectCondition cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CommonUtil c = new CommonUtil();
                cond.ContractCode = c.ConvertContractCode(cond.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                
                IProjectHandler hand = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
                List<dtProjectData> list = hand.GetProjectDataForSearch(cond);

                res.ResultData = CommonUtil.ConvertToXml<dtProjectData>(list, "Common\\CMS290", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        #region Old Code
        //public ActionResult CMS290()
        //{
        //   return View();
        //}
        //public ActionResult GetProject_CMS290(doCMS290_SearchProject Search)
        //{
        //    ObjectResultData res = new ObjectResultData();
        //    try
        //    {
               
        //        List<MessageModel> msgLst = new List<MessageModel>();
        //        //string ProjectCode = Search.ProjectCode;
        //        //string ContractCode = Search.ContractCode;
        //        //string ProjectName = Search.ProjectName;
        //        //if (ProjectName == null)
        //        //    ProjectName = "%";
        //        //string ProRepAdd = Search.ProRepAdd;
        //        //if (ProRepAdd == null)
        //        //    ProRepAdd = "%";
        //        //string PJPurName = Search.PJPurName;
        //        //if (PJPurName == null)
        //        //    PJPurName = "%";
        //        //string PJOwe1 = Search.PJOwe1;
        //        //if (PJOwe1 == null)
        //        //    PJOwe1 = "%";
        //        //string PJManComName = Search.PJManComName;
        //        //if (PJManComName == null)
        //        //    PJManComName = "%";
        //        //string OthPJRelPerName = Search.OthPJRelPerName;
        //        //if (OthPJRelPerName == null)
        //        //    OthPJRelPerName = "%";
        //        //string SystemProduct = Search.SystemProduct;
        //        //string HeadSalemaneName = Search.HeadSalemaneName;
        //        //if (OthPJRelPerName == null)
        //        //    OthPJRelPerName = "%";
        //        //string PJMangerName = Search.PJMangerName;
        //        //if (PJMangerName == null)
        //        //    PJMangerName = "%";
        //        IProjectHandler hand = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
        //        List<dtProjectData> lst = hand.GetProjectDataForSearch(Search.ProjectCode, Search.ContractCode, Search.ProjectName, Search.ProRepAdd, Search.PJPurName, Search.PJOwe1, Search.PJManComName, Search.OthPJRelPerName, Search.SystemProduct, Search.HeadSalemaneName, Search.PJMangerName);
        //        List<View_dtProject> lst2 = new List<View_dtProject>();
        //        if(lst.Count > 0 && lst.Count <= 1000)
        //        {
        //            foreach (dtProjectData dt in lst)
        //            {
        //                lst2.Add(CommonUtil.CloneObject<dtProjectData, View_dtProject>(dt));
        //            }
        //            string xml = CommonUtil.ConvertToXml<View_dtProject>(lst2, "Common\\CMS290");
        //            return Json(xml);
        //        }
        //        else if (lst.Count > 1000)
        //        {
        //            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0052);
        //            return Json(res);
        //        }
        //        else
        //        {
        //            foreach (dtProjectData dt in lst)
        //            {
        //                lst2.Add(CommonUtil.CloneObject<dtProjectData, View_dtProject>(dt));
        //            }
        //            string xml = CommonUtil.ConvertToXml<View_dtProject>(lst2, "Common\\CMS290");
        //            return Json(xml);
        //        }

                
        //    }
        //    catch (Exception ex)
        //    {
        //        res.AddErrorMessage(ex);
        //        return Json(res);
        //    }

        //}
        #endregion
    }
}
