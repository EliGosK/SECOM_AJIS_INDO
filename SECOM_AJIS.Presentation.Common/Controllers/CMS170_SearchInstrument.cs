using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.Presentation.Common.Models;
using System.Linq;


namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {
        private const string CMS170_Screen = "CMS170";


        /// <summary>
        /// Check permission for access screen CMS170
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CMS170_Authority(CMS170_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                // Parameter is OK ?
                if (param.InputData == null)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0040);
                    return Json(res);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }

            return InitialScreenEnvironment<CMS170_ScreenParameter>("CMS170", param, res);
        }


        /// <summary>
        /// Method for return view of screen CMS170
        /// </summary>
        /// <returns></returns>
        [Initialize(CMS170_Screen)]
        public ActionResult CMS170()
        {
            CMS170_ScreenParameter CMS170Param = new CMS170_ScreenParameter();
            try
            {
                CMS170Param = GetScreenObject<CMS170_ScreenParameter>();
                doInstrumentParam CMS170Input = CMS170Param.InputData;

                if (CMS170Input != null)
                {
                    ViewBag.bExpTypeHas = CMS170Input.bExpTypeHas;
                    ViewBag.bExpTypeNo = CMS170Input.bExpTypeNo;
                    ViewBag.bProdTypeSale = CMS170Input.bProdTypeSale;
                    ViewBag.bProdTypeAlarm = CMS170Input.bProdTypeAlarm;
                    ViewBag.bInstTypeGen = CMS170Input.bInstTypeGen;
                    ViewBag.bInstTypeMonitoring = CMS170Input.bInstTypeMonitoring;
                    ViewBag.bInstTypeMat = CMS170Input.bInstTypeMat;

                    if (CMS170Input.bExpTypeHas || CMS170Input.bExpTypeNo)
                    {
                        ViewBag.DisableExpType = "True";
                        CMS170Param.DisableExpType = true;
                    }
                    else
                    {
                        ViewBag.DisableExpType = "False";
                        CMS170Param.DisableExpType = false;
                    }

                    if (CMS170Input.bProdTypeSale || CMS170Input.bProdTypeAlarm)
                    {
                        ViewBag.DisableProdType = "True";
                        CMS170Param.DisableProdType = true;
                    }
                    else
                    {
                        ViewBag.DisableProdType = "False";
                        CMS170Param.DisableProdType = false;
                    }

                    if (CMS170Input.bInstTypeGen || CMS170Input.bInstTypeMonitoring || CMS170Input.bInstTypeMat)
                    {
                        ViewBag.DisableInstType = "True";
                        CMS170Param.DisableInstType = true;
                    }
                    else
                    {
                        ViewBag.DisableInstType = "False";
                        CMS170Param.DisableInstType = false;
                    }

                }

                UpdateScreenObject(CMS170Param);
            }
            catch
            {
            }



            return View(CMS170_Screen);
        }

        /// <summary>
        /// Initial grid of screen CMS170
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS170_InitialGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS170", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Check seach condition that is required field
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CMS170_CheckReqField(CMS170_SearchCondition cond)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                #region Removed check intrument flag & expansion type & product type
                //if (cond.InstFlagMain == false && cond.InstFlagOption == false)
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_COMMON,
                //                        CMS170_Screen,
                //                        MessageUtil.MODULE_COMMON,
                //                        MessageUtil.MessageList.MSG0079,
                //                        new string[] { "lblInstruFlag" },
                //                        new string[] { "cms170_InstFlagMain", "cms170_InstFlagOption" });
                //}

                //if (cond.ExpTypeHas == false && cond.ExpTypeNo == false)
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_COMMON,
                //                        CMS170_Screen,
                //                        MessageUtil.MODULE_COMMON,
                //                        MessageUtil.MessageList.MSG0079,
                //                        new string[] { "lblExpanFlag" },
                //                        new string[] { "cms170_ExpTypeHas", "cms170_ExpTypeNo" });
                //}

                //if (cond.ProdTypeAlarm == false && cond.ProdTypeSale == false)
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_COMMON,
                //                        CMS170_Screen,
                //                        MessageUtil.MODULE_COMMON,
                //                        MessageUtil.MessageList.MSG0079,
                //                        new string[] { "lblProductType" },
                //                        new string[] { "cms170_ProdTypeSale", "cms170_ProdTypeAlarm" });
                //}

                #endregion

                if (cond.InstTypeGen == false && cond.InstTypeMon == false && cond.InstTypeMat == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON,
                                        CMS170_Screen,
                                        MessageUtil.MODULE_COMMON,
                                        MessageUtil.MessageList.MSG0079,
                                        new string[] { "lblInstruType" },
                                        new string[] { "cms170_InstTypeGen", "cms170_InstTypeMon", "cms170_InstTypeMat" });
                }

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
        /// Get instrument data list by search condition
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CMS170_Search(CMS170_SearchCondition cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IInstrumentMasterHandler hand = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> tmpCurrencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                        {
                                                            new doMiscTypeCode()
                                                            {
                                                                FieldName = MiscType.C_CURRENCT,
                                                                ValueCode = "%"
                                                            }
                                                        }).ToList();
                doInstrumentSearchCondition searchCon = new doInstrumentSearchCondition();

                //Start: Prepare search condition----------------------------------------------------------
                //Set parameter value for partial matching search textbox
                searchCon.InstrumentCode = (cond.InstrumentCode != null) ? cond.InstrumentCode.Replace('*', '%') : null;
                searchCon.InstrumentName = (cond.InstrumentName != null) ? cond.InstrumentName.Replace('*', '%') : null;
                searchCon.Maker = (cond.Maker != null) ? cond.Maker.Replace('*', '%') : null;

                searchCon.SupplierCode = cond.SupplierCode;
                searchCon.LineUpTypeCode = cond.LineUpTypeCode;

                searchCon.InstrumentFlag = new List<int?>();
                if (cond.InstFlagMain)
                    searchCon.InstrumentFlag.Add(1);
                if (cond.InstFlagOption)
                    searchCon.InstrumentFlag.Add(0);

                searchCon.ExpansionType = new List<string>();
                if (cond.ExpTypeHas)
                    searchCon.ExpansionType.Add(ExpansionType.C_EXPANSION_TYPE_PARENT);
                if (cond.ExpTypeNo)
                    searchCon.ExpansionType.Add(ExpansionType.C_EXPANSION_TYPE_CHILD);

                if (cond.ProdTypeSale)
                    searchCon.SaleFlag = 1;
                if (cond.ProdTypeAlarm)
                    searchCon.RentalFlag = 1;

                searchCon.InstrumentType = new List<string>();
                if (cond.InstTypeGen)
                    searchCon.InstrumentType.Add(InstrumentType.C_INST_TYPE_GENERAL);
                if (cond.InstTypeMon)
                    searchCon.InstrumentType.Add(InstrumentType.C_INST_TYPE_MONITOR);
                if (cond.InstTypeMat)
                    searchCon.InstrumentType.Add(InstrumentType.C_INST_TYPE_MATERIAL);
                //End: Prepare search condition----------------------------------------------------------

                List<doInstrumentData> list = hand.GetInstrumentDataForSearch(searchCon);
                for (int i = 0; i < list.Count(); i++)
                {
                    list[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                }

                res.ResultData = CommonUtil.ConvertToXml<doInstrumentData>(list, "Common\\CMS170", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        #region Old Code
        //public ActionResult Common170_Search(doCMS170_SearchInstrument Search)
        //{
        //    ObjectResultData res = new ObjectResultData();
        //    List<MessageModel> msgLst = new List<MessageModel>();
        //    string InstruCode = Search.InstumentCode;

        //    if( InstruCode == "")
        //        InstruCode = "%";
        //    string InstruName = Search.InstumentName;

        //    if (InstruName == "")
        //        InstruName = "%";

        //    string Maker = Search.Maker;

        //    if (Maker == "")
        //        Maker = "%";

        //    string InstruType1 = Search.InstrumentType_1;
        //    string InstruType2 = Search.InstrumentType_2;
        //    string InstruType3 = Search.InstrumentType_3;
        //    string InstrumentFlag_1 = Search.InstrumentFlag_1;
        //    string InstrumentFlag_2 = Search.InstrumentFlag_2;
        //    string ExpantionType_1 = Search.ExpantionType_1;
        //    string ExpantionType_2 = Search.ExpantionType_2;
        //    string Sale = Search.ProductType_1;
        //    string Rental = Search.ProductType_2;
        //    if (InstrumentFlag_1 == null && InstrumentFlag_2 == null )
        //    {
        //        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0079);	
        //        return Json(res);
        //    }


        //    if (Sale == null && Rental == null)
        //    {
        //        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0079);
        //        return Json(res);
        //    }
        //    else
        //    {
        //        if (Sale == "0")
        //            Sale = "1";
        //        if (Sale == null )
        //            Sale = "0";
        //        if (Rental == null)
        //            Rental = "0";
        //    }
        //    if (ExpantionType_1 == null && ExpantionType_2 == null)
        //    {
        //        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0079);
        //        return Json(res);
        //    }
        //    else
        //    {
        //        if (ExpantionType_1 == null)
        //            ExpantionType_1 = "%";
        //        if (ExpantionType_2 == null)
        //            ExpantionType_2 = "%";
        //    }

        //    if (InstruType1 == null && InstruType2 == null && InstruType3 == null)
        //    {
        //        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0079);
        //        return Json(res);
        //    }
        //    else
        //    {
        //        if (InstruType1 == null)
        //            InstruType1 = "%";
        //        if (InstruType2 == null)
        //            InstruType2 = "%";
        //        if (InstruType3 == null)
        //            InstruType3 = "%";
        //    }
        //    string LineUp = Search.LineUpType;
        //    string SupplierCode = Search.SupplierType;
        //    if (SupplierCode == "")
        //        SupplierCode = null;
        //    if (LineUp == "")
        //        LineUp = null;
        //    try 
        //    {
        //           IInstrumentMasterHandler hand = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
        //           doInstrumentSearchCondition con = new doInstrumentSearchCondition();
        //           con.InstrumentType = new List<string>(){InstruType1, InstruType2, InstruType3};
        //           int InstrumantFalg1 = Convert.ToInt32(InstrumentFlag_1);
        //           int InstrumantFalg2 = Convert.ToInt32(InstrumentFlag_2);
        //           con.InstrumentFlag = new List<int?>() { InstrumantFalg1, InstrumantFalg2 };
        //           con.InstrumentCode = InstruCode;
        //           con.InstrumentName = InstruName;
        //           con.Maker = Maker;
        //           con.ExpansionType = new List<string>() { ExpantionType_1,ExpantionType_2};
        //           con.SaleFlag = Convert.ToInt32(Sale);
        //           con.RentalFlag = Convert.ToInt32(Rental);
        //           con.SupplierCode = SupplierCode;
        //           con.LineUpTypeCode = LineUp;

        //           List<doInstrumentData> lst = hand.GetInstrumentDataForSearch(con);
        //           if (lst.Count > 0 && lst.Count <= 1000)
        //           {
        //               string xml = CommonUtil.ConvertToXml<doInstrumentData>(lst, "Common\\CMS170");
        //               return Json(xml);
        //           }
        //           else if (lst.Count > 1000)
        //           {
        //               res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0052);
        //               return Json(res);
        //           }
        //           else
        //           {
        //               res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
        //               return Json(res);
        //           }
        //    }
        //    catch(Exception ex)
        //    {
        //        res.AddErrorMessage(ex);
        //        return Json(res);
        //    }

        //   // return null;
        //}
        public ActionResult GetInstrumentName(string cond)
        {
            try
            {
                IAutoCompleteHandler hand = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<doInstrumentName> lst = hand.GetInstrumentName(cond);
                List<string> strList = new List<string>();

                foreach (var l in lst)
                {
                    strList.Add(l.InstrumentName);
                }

                return Json(strList.ToArray());
            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        public ActionResult GetMaker(string cond)
        {
            try
            {
                IAutoCompleteHandler hand = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<dtGetInstrumentMaker> lst = hand.GetInstrumentMaker(cond);
                List<string> strList = new List<string>();

                foreach (var l in lst)
                {
                    strList.Add(l.Maker);
                }

                return Json(strList.ToArray());
            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }
        #endregion

    }
}
