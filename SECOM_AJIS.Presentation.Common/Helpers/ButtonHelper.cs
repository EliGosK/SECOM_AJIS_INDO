using System;
using System.Web.WebPages;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Contract;
using CSI.WindsorHelper;

namespace SECOM_AJIS.Presentation.Common.Helpers
{
    public static class ButtonHelper
    {
        public static MvcHtmlString OperationTypeCheckList(this HtmlHelper helper,
                                                            string id,
                                                            string[] check_val = null,
                                                            object attribute = null)
        {
            List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
            try
            {
                ICommonHandler handler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (handler != null)
                {
                    lst = handler.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                    {
                        new doMiscTypeCode()
                        {
                            FieldName =  SECOM_AJIS.Common.Util.ConstantValue.MiscType.C_OPERATION_TYPE,
                            ValueCode = "%"
                        }
                    });
                }
           }
            catch
            {

            }

            return CommonUtil.CommonCheckButtonList<doMiscTypeCode>(id, null, lst, "ValueCodeDisplay", "ValueCode", false, check_val, attribute);
        }



        public static MvcHtmlString ContractsSameSiteLinkList(this HtmlHelper helper,
                                                 string id,
                                                 string header,
                                                 string strSiteCode,
                                                 bool isHorizontal = false,
                                                 object attribute = null ,
                                                 string strContracatCode = null)
        {

            CommonUtil c = new CommonUtil();
            strSiteCode = c.ConvertSiteCode(strSiteCode, CommonUtil.CONVERT_TYPE.TO_LONG);
            strContracatCode = c.ConvertContractCode(strContracatCode ,CommonUtil.CONVERT_TYPE.TO_LONG);

            List<TestModel> lst = new List<TestModel>();
            
            try
            {
                // Rental
                IViewContractHandler handler = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
                List<dtContractsSameSite> list = handler.GetContractsSameSiteList(strSiteCode, strContracatCode);


                // convert to short code format
                foreach (var item in list)
                {
                    item.ContractCode = c.ConvertContractCode(item.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                }

                string strCode = "";
                string strScreenID = "";
                string strDisplay = "";
                foreach (var item in list)
                {
                    //strScreenID = item.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL ? "CMS"

                    if (item.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                    {
                        strScreenID = "CMS120" ;
                    }
                    else if ((item.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE))
                    {
                        strScreenID = "CMS160" ;
                    }

                    strCode = string.Format("{0}--{1}",strScreenID ,item.ContractCode);
                    strDisplay = string.Format("{0}: {1}" , item.ContractCode , item.ProductCode);

                    lst.Add(new TestModel() { Code = strCode, DisplayName = strDisplay });
                }
            }
            catch
            {
                lst = new List<TestModel>();
            }

            return CommonUtil.CommonLinkList<TestModel>(id, header, lst, "DisplayName", "Code", isHorizontal, attribute);


        }

    }
}
