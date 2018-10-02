//*********************************
// Create by: Narupon W.
// Create date: 30/Jun/2010
// Update date: 30/Jun/2010
//*********************************


using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Reflection;


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
        /// Check permission for access screen CMS250.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CMS250_Authority(CMS250_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
   
            return InitialScreenEnvironment<CMS250_ScreenParameter>("CMS250", param, res);
        }

        /// <summary>
        ///  Method for return view of screen CMS250
        /// </summary>
        /// <returns></returns>
        [Initialize("CMS250")]
        public ActionResult CMS250()
        {
            return View();
        }

        /// <summary>
        /// Initial grid of screen CMS250
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS250_InitialGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS250"));
        }

        /// <summary>
        /// Get customer data by search condition
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CMS250_SearchResponse(doCustomerSearchCondition cond)
        {

            CommonUtil c = new CommonUtil();

            List<View_dtCustomerData2> nlst = new List<View_dtCustomerData2>();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            
            // Concate string Customer status with commar separate. like ,xx,yy,zz,  
            List<string> lstCustStatus = new List<string>();
            lstCustStatus.Add(cond.chkExistCustomer);
            lstCustStatus.Add(cond.chkNewCustomer);
            cond.CustStatus = CommonUtil.CreateCSVString(lstCustStatus);

            // Concate string CustomerTypeCode with commar separate. like ,xx,yy,zz, 
            List<string> lstCustomerTypeCode = new List<string>();
            lstCustomerTypeCode.Add(cond.chkJuristic);
            lstCustomerTypeCode.Add(cond.chkIndividual);
            lstCustomerTypeCode.Add(cond.chkAssociation);
            lstCustomerTypeCode.Add(cond.chkPublicOffice);
            lstCustomerTypeCode.Add(cond.chkOther);
            cond.CustomerTypeCode = CommonUtil.CreateCSVString(lstCustomerTypeCode);


            try
            {

                cond.CustomerCode = c.ConvertCustCode(cond.CustomerCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                if (cond.CustStatus == string.Empty )
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0054);
                }

                if (cond.CustomerTypeCode == string.Empty)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0055);
                }
                else
                {
                    //if (cond.Counter == 0)
                    //{
                    //    res.ResultData = CommonUtil.ConvertToXml<View_dtCustomerData>(nlst, "Common\\CMS250",CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                    //    return Json(res);
                    //}

                    ICustomerMasterHandler handler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
                    List<dtCustomerData> list = handler.GetCustomerDataForSearch(cond);


                    foreach (dtCustomerData l in list)
                    {
                        l.CustCode = c.ConvertCustCode(l.CustCode ,CommonUtil.CONVERT_TYPE.TO_SHORT);
                        nlst.Add(CommonUtil.CloneObject<dtCustomerData, View_dtCustomerData2>(l));
                    }
                }


            }
            catch (Exception ex)
            {
                nlst = new List<View_dtCustomerData2>();
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
                
            }

            res.ResultData = CommonUtil.ConvertToXml<View_dtCustomerData2>(nlst, "Common\\CMS250",CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            return Json(res);

        }

        /// <summary>
        /// Get customer group data by customer (code , name)
        /// </summary>
        /// <param name="strCustomerCode"></param>
        /// <param name="strGroupName"></param>
        /// <returns></returns>
        public ActionResult CMS250_GetCustomerGroup(string strCustomerCode, string strGroupName)
        {
            if (strGroupName == string.Empty)
            {
                strGroupName = null;
            }

            try
            {
                CommonUtil c = new CommonUtil();
                strCustomerCode = c.ConvertCustCode(strCustomerCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                ICustomerMasterHandler handler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;

                List<dtCustomeGroupData> lsCustomerGroups = handler.GetCustomeGroupData(strCustomerCode, strGroupName);

                 return Json(lsCustomerGroups);

            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res); 
            }
        }
        
    }
}
