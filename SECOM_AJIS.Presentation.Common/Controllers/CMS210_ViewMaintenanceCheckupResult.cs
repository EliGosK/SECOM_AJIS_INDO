//*********************************
// Create by: Narupon W.
// Create date: /Jun/2011
// Update date: /Jun/2011
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

using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Contract;
using CSI.WindsorHelper;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Presentation.Common.Models;

namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {
        /// <summary>
        ///  Check permission for access screen CMS210
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CMS210_Authority(CMS210_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (CommonUtil.IsNullOrEmpty(param.ContractCode) == true
                    || CommonUtil.IsNullOrEmpty(param.ServiceTypeCode) == true
                    || CommonUtil.IsNullOrEmpty(param.MATargetContractCode) == true
                    || CommonUtil.IsNullOrEmpty(param.ProductCode) == true
                    )
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0040);
                    return Json(res);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<CMS210_ScreenParameter>("CMS210", param, res);
        }

        /// <summary>
        /// Method for return view of screen CMS210
        /// </summary>
        /// <returns></returns>
        [Initialize("CMS210")]
        public ActionResult CMS210()
        {
            CMS210_ScreenParameter cond = new CMS210_ScreenParameter();

            try
            {
                cond = GetScreenObject<CMS210_ScreenParameter>();
               
            }
            catch
            {
            }

            // Keep ServiceTypeCode
            ViewBag.ServiceTypeCode = cond.ServiceTypeCode;

            CommonUtil c = new CommonUtil();
            cond.ContractCode = c.ConvertContractCode(cond.ContractCode ,CommonUtil.CONVERT_TYPE.TO_LONG);
            cond.MATargetContractCode = c.ConvertContractCode(cond.MATargetContractCode,CommonUtil.CONVERT_TYPE.TO_LONG);
            cond.ContractTargetCode = c.ConvertCustCode(cond.ContractTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG);

            try
            {
                IRentralContractHandler handlerR = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                ISaleContractHandler handlerS = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;

                List<dtTbt_RentalContractBasicForView> dtRentalContract = new List<dtTbt_RentalContractBasicForView>();
                List<dtTbt_SaleBasicForView> dtSaleContract = new List<dtTbt_SaleBasicForView>();
                ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> tmpCurrencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                        {
                                                            new doMiscTypeCode()
                                                            {
                                                                FieldName = MiscType.C_CURRENCT,
                                                                ValueCode = "%"
                                                            }
                                                        }).ToList();

                if (cond.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                {
                    dtRentalContract = handlerR.GetTbt_RentalContractBasicForView(cond.MATargetContractCode);

                    //Add Currency to List
                    for (int i = 0; i < dtRentalContract.Count(); i++)
                    {
                        dtRentalContract[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                    }

                    foreach (var item in dtRentalContract)
                    {
                        item.ContractCode = c.ConvertContractCode(item.ContractCode ,CommonUtil.CONVERT_TYPE.TO_SHORT);
                    }


                    if (dtRentalContract.Count > 0)
                    {
                        ViewBag.txtContractCode =  dtRentalContract[0].ContractCode  ;
                        ViewBag.txtContract_target_name_English =  dtRentalContract[0].CustFullNameEN_Cust  ;
                        ViewBag.txtContract_target_address_English =  dtRentalContract[0].AddressFullEN_Cust  ;
                        ViewBag.txtSite_name_English =  dtRentalContract[0].SiteNameEN_Site  ;
                        ViewBag.Site_address_English =  dtRentalContract[0].AddressFullEN_Site  ;
                        ViewBag.txtContract_target_name_Local =  dtRentalContract[0].CustFullNameLC_Cust  ;
                        ViewBag.txtContract_target_address_Local =  dtRentalContract[0].AddressFullLC_Cust  ;
                        ViewBag.txtSite_name_Local =  dtRentalContract[0]. SiteNameLC_Site ;
                        ViewBag.txtSite_address_Local = dtRentalContract[0].AddressFullLC_Site;

                        ViewBag.txtRentalAttachImportanceFlag = dtRentalContract[0].SpecialCareFlag;
                    }


                }
                else if (cond.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                {
                    dtSaleContract = handlerS.GetTbt_SaleBasicForView(cond.MATargetContractCode, null, null);

                    //Add Currency to List
                    for (int i = 0; i < dtSaleContract.Count(); i++)
                    {
                        dtSaleContract[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                    }

                    foreach (var item in dtSaleContract)
                    {
                        item.ContractCode = c.ConvertContractCode(item.ContractCode ,CommonUtil.CONVERT_TYPE.TO_SHORT);
                    }

                    if (dtSaleContract.Count  > 0)
                    {
                        ViewBag.txtContractCode = dtSaleContract[0].ContractCode;
                        ViewBag.txtContract_target_name_English = dtSaleContract[0].PurCust_CustFullNameEN;
                        ViewBag.txtContract_target_address_English = dtSaleContract[0].AddressFullEN_PurCust;
                        ViewBag.txtSite_name_English = dtSaleContract[0].site_SiteNameEN;
                        ViewBag.Site_address_English = dtSaleContract[0].AddressFullEN_site;
                        ViewBag.txtContract_target_name_Local = dtSaleContract[0].PurCust_CustFullNameLC;
                        ViewBag.txtContract_target_address_Local = dtSaleContract[0].AddressFullLC_PurCust;
                        ViewBag.txtSite_name_Local = dtSaleContract[0].site_SiteNameLC;
                        ViewBag.txtSite_address_Local = dtSaleContract[0].AddressFullLC_site;

                        ViewBag.txtSaleAttachImportanceFlag = dtSaleContract[0].SpecialCareFlag;
                    }
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
        /// Initial grid of screen CMS210
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS210_InitialGrid()
        {
            return Json(CommonUtil.ConvertToXml<View_dtMaintCheckUpResultList>(null, "Common\\CMS210", CommonUtil.GRID_EMPTY_TYPE.VIEW));
        }

        /// <summary>
        /// Get maintenance check-up result by search condition
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CMS210_GetMaintCheckUpResultList(doContractInfoCondition cond)
        {
            CommonUtil c = new CommonUtil();

            List<View_dtMaintCheckUpResultList> nlst = new List<View_dtMaintCheckUpResultList>();

            ObjectResultData res = new ObjectResultData();

            try
            {
                cond.ContractCode = c.ConvertContractCode(cond.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                cond.MATargetContractCode = c.ConvertContractCode(cond.MATargetContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);


                IViewContractHandler handler = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
                List<dtMaintCheckUpResultList> list = handler.GetMaintCheckUpResultList(cond.ContractCode ,cond.MATargetContractCode ,cond.ProductCode );


                list = CommonUtil.ConvertObjectbyLanguage<dtMaintCheckUpResultList, dtMaintCheckUpResultList>(list,
                                            "SubContractorName",
                                            "MaintEmpFirstName",
                                            "MaintEmpLastName");

                                           

                // clone object to View
                foreach (dtMaintCheckUpResultList l in list)
                {
                    nlst.Add(CommonUtil.CloneObject<dtMaintCheckUpResultList, View_dtMaintCheckUpResultList>(l));
                }

            }
            catch (Exception ex)
            {
                nlst = new List<View_dtMaintCheckUpResultList>();
                res.AddErrorMessage(ex);
            }

            res.ResultData = CommonUtil.ConvertToXml<View_dtMaintCheckUpResultList>(nlst, "Common\\CMS210",CommonUtil.GRID_EMPTY_TYPE.VIEW);
            return Json(res);

        }
       
    }

        

}
