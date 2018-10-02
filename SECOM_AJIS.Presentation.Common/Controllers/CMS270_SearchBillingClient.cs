//*********************************
// Create by: Narupon W.
// Create date: /Jun/2011
// Update date: /Jun/2011
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
using SECOM_AJIS.Presentation.Common.Models;

namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {
        /// <summary>
        /// Check permission for access screen CMS270
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CMS270_Authority(CMS270_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            return InitialScreenEnvironment<CMS270_ScreenParameter>("CMS270", param, res);
        }

        /// <summary>
        ///  Method for return view of screen CMS270
        /// </summary>
        /// <returns></returns>
        [Initialize("CMS270")]
        public ActionResult CMS270()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS270_InitialGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS270"));
        }

        public ActionResult CMS270_SearchResponse(doBillingClientSearchCondition cond)
        {
            List<dtBillingClientDataForSearch> list = new List<dtBillingClientDataForSearch>();
            List<View_dtBillingClientData> nlst = new List<View_dtBillingClientData>();

            ObjectResultData res = new ObjectResultData();


            // Concate string CustomerTypeCode with commar separate. like ,xx,yy,zz, 
            List<string> lstCustomerTypeCode = new List<string>();
            lstCustomerTypeCode.Add(cond.chkJuristic);
            lstCustomerTypeCode.Add(cond.chkIndividual);
            lstCustomerTypeCode.Add(cond.chkAssociation);
            lstCustomerTypeCode.Add(cond.chkPublicOffice);
            lstCustomerTypeCode.Add(cond.chkOther);
            cond.CustomerTypeCode = CommonUtil.CreateCSVString(lstCustomerTypeCode);

            
            CommonUtil c = new CommonUtil();
            

            try
            {

                cond.BillingClientCode = c.ConvertBillingClientCode(cond.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                if (cond.CustomerTypeCode == string.Empty)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0055);
                }
                else
                {
                    //if (cond.Counter == 0)
                    //{
                    //    res.ResultData = CommonUtil.ConvertToXml<View_dtBillingClientData>(nlst, "Common\\CMS270", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                    //    return Json(res);
                    //}


                    IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                    list = handler.GetBillingClientDataForSearch(cond);

                    foreach (dtBillingClientDataForSearch l in list)
                    {
                        nlst.Add(CommonUtil.CloneObject<dtBillingClientDataForSearch, View_dtBillingClientData>(l));
                    }
                }

            }
            catch (Exception ex)
            {
                nlst = new List<View_dtBillingClientData>();
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
                
            }

            res.ResultData = CommonUtil.ConvertToXml<View_dtBillingClientData>(nlst, "Common\\CMS270", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            return Json(res);

        }

    }
}
