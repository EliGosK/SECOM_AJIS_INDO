//*********************************
// Create by: 
// Create date: /Jun/2010
// Update date: /Jun/2010
//*********************************

using System;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Master;
using CSI.WindsorHelper;
using SECOM_AJIS.Presentation.Master.Models;

namespace SECOM_AJIS.Presentation.Master.Controllers
{
    public partial class MasterController : BaseController
    {
        private const string MAS090_Screen = "MAS090";

        /// <summary>
        /// - Check user permission for screen MAS090.<br />
        /// - Check system suspending.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult MAS090_Authority(MAS090_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (!(CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_INSTRUMENT_INFO, FunctionID.C_FUNC_ID_VIEW) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_INSTRUMENT_INFO, FunctionID.C_FUNC_ID_ADD) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_INSTRUMENT_INFO, FunctionID.C_FUNC_ID_EDIT) == true
                    ))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                //Check system suspending
                res = checkSystemSuspending();
                if (res.IsError)
                {
                    return Json(res);
                }
                // Do in view
                //param.hasPermissionAdd = CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_INSTRUMENT_INFO, FunctionID.C_FUNC_ID_ADD);
                //param.hasPermissionEdit = CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_INSTRUMENT_INFO, FunctionID.C_FUNC_ID_EDIT);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<MAS090_ScreenParameter>("MAS090", param, res);
        }

        /// <summary>
        /// Initial screen.
        /// </summary>
        /// <returns></returns>
        [Initialize(MAS090_Screen)]
        public ActionResult MAS090()
        {
            MAS090_ScreenParameter MAS090Param = new MAS090_ScreenParameter();
            ViewBag.HasPermissionAdd = "";
            ViewBag.HasPermissionEdit = "";

            try
            {
                MAS090Param = GetScreenObject<MAS090_ScreenParameter>();
                ViewBag.HasPermissionAdd = CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_INSTRUMENT_INFO, FunctionID.C_FUNC_ID_ADD);
                ViewBag.HasPermissionEdit = CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_INSTRUMENT_INFO, FunctionID.C_FUNC_ID_EDIT);
                ViewBag.InstrumentTypeGeneral = InstrumentType.C_INST_TYPE_GENERAL;
            }
            catch
            {
            }

            return View();
        }

        /// <summary>
        /// Get config for instrument table.
        /// </summary>
        /// <returns></returns>
        public ActionResult MAS090_InitGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Master\\MAS090"));
        }

        /// <summary>
        /// Search instrument list.
        /// </summary>
        /// <param name="txtInstrumentCodeSearch"></param>
        /// <param name="txtInstrumentNameSearch"></param>
        /// <param name="cboLineUpTypeSearch"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MAS090_Search(string txtInstrumentCodeSearch, string txtInstrumentNameSearch, string cboLineUpTypeSearch)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                txtInstrumentCodeSearch = (txtInstrumentCodeSearch == "" ? null : txtInstrumentCodeSearch);
                txtInstrumentNameSearch = (txtInstrumentNameSearch == "" ? null : txtInstrumentNameSearch);
                cboLineUpTypeSearch = (cboLineUpTypeSearch == "" ? null : cboLineUpTypeSearch);

                if (txtInstrumentCodeSearch == null && txtInstrumentNameSearch == null && cboLineUpTypeSearch == null)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);
                }

                if (res.IsError)
                {
                    return Json(res);
                }

                IInstrumentMasterHandler hand = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                List<dtInstrument> list = hand.GetInstrument(txtInstrumentCodeSearch, txtInstrumentNameSearch, cboLineUpTypeSearch, MiscType.C_LINE_UP_TYPE);

                foreach (var item in list)
                {
                    if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                    {
                        item.LineUpTypeCode = item.LineUpTypeNameEN;
                    }
                    else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                    {
                        item.LineUpTypeCode = item.LineUpTypeNameJP;
                    }
                    else
                    {
                        item.LineUpTypeCode = item.LineUpTypeNameLC;
                    }
                }

                string xml = CommonUtil.ConvertToXml<dtInstrument>(list, "Master\\MAS090", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                res.ResultData = xml;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return null;
            }
        }

        /// <summary>
        /// Get detail of selected instrument.
        /// </summary>
        /// <param name="InstrumentCode"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MAS090_GetInstrumentDetail(string InstrumentCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IInstrumentMasterHandler hand = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                dtInstrumentDetail detail = null;
                List<dtInstrumentDetail> list = hand.GetInstrumentDetail(InstrumentCode, MiscType.C_LINE_UP_TYPE, CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US);
                if (list.Count > 0)
                {
                    detail = list[0];

                    MAS090_ScreenParameter MAS090Param = GetScreenObject<MAS090_ScreenParameter>();
                    if (detail.UpdateDate.HasValue)
                    {
                        MAS090Param.updateDate = detail.UpdateDate.Value;
                    }
                }

                res.ResultData = detail;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return null;
            }
        }

        /// <summary>
        /// Insert new instrument.<br />
        /// - Check system suspending.<br />
        /// - Validate require field.<br />
        /// - Check duplicate instrument.<br />
        /// - Insert instrument to database.
        /// </summary>
        /// <param name="instrument"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MAS090_InsertInstrument(MAS090_SaveInstrument instrument)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                #region Check system suspending

                res = checkSystemSuspending();
                if (res.IsError)
                    return Json(res);

                #endregion

                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

                #region Validate require field

                ValidatorUtil validator = new ValidatorUtil(this);
                if (!instrument.SaleFlag.HasValue)
                {
                    instrument.SaleFlag = false;
                }
                if (!instrument.RentalFlag.HasValue)
                {
                    instrument.RentalFlag = false;
                }
                if (!instrument.InstrumentFlag.HasValue)
                {
                    instrument.InstrumentFlag = false;
                }
                if (!instrument.ZeroBahtAssetFlag.HasValue)
                {
                    instrument.ZeroBahtAssetFlag = false;
                }
                if (!instrument.MaintenanceFlag.HasValue)
                {
                    instrument.MaintenanceFlag = false;
                }
                if (!instrument.ControllerFlag.HasValue)
                {
                    instrument.ControllerFlag = false;
                }

                if (InstrumentType.C_INST_TYPE_GENERAL.Equals(instrument.InstrumentTypeCode))
                {
                    if (instrument.LineUpTypeCode == null)
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_MASTER,
                            MAS090_Screen,
                            MessageUtil.MODULE_COMMON,
                            MessageUtil.MessageList.MSG0007,
                            "RLineUpTypeCode",
                            "lblLineUpType",
                            "LineUpTypeCode");
                    }
                    if (instrument.ExpansionTypeCode == null)
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_MASTER,
                            MAS090_Screen,
                            MessageUtil.MODULE_COMMON,
                            MessageUtil.MessageList.MSG0007,
                            "RExpansionTypeCode",
                            "lblExpansionType",
                            "ExpansionTypeCode");
                    }
                }

                ValidatorUtil.BuildErrorMessage(res, validator);
                if (res.IsError)
                    return Json(res);

                #endregion
                #region Check duplicate instrument information

                IInstrumentMasterHandler hand = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                List<bool?> listCheck = hand.CheckExistInstrument(instrument.InstrumentCode);
                if (listCheck.Count != 0 && listCheck[0].Value)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1019);
                    return Json(res);
                }

                #endregion



                // add by Jirawat Jannet on 216-12-23
                #region Set amount and currency type

                if (instrument.SaleUnitPriceCurrencyType == CurrencyUtil.C_CURRENCY_US)
                {
                    instrument.SaleUnitPriceUsd = instrument.SaleUnitPrice;
                    instrument.SaleUnitPrice = null;
                }
                else
                    instrument.SaleUnitPriceUsd = null;

                if (instrument.RentalUnitPriceCurrencyType == CurrencyUtil.C_CURRENCY_US)
                {
                    instrument.RentalUnitPriceUsd = instrument.RentalUnitPrice;
                    instrument.RentalUnitPrice = null;
                }
                else
                    instrument.RentalUnitPriceUsd = null;

                if (instrument.AddUnitPriceCurrencyType == CurrencyUtil.C_CURRENCY_US)
                {
                    instrument.AddUnitPriceUsd = instrument.AddUnitPrice;
                    instrument.AddUnitPrice = null;
                }
                else
                    instrument.AddUnitPriceUsd = null;

                if (instrument.RemoveUnitPriceCurrencyType == CurrencyUtil.C_CURRENCY_US)
                {
                    instrument.RemoveUnitPriceUsd = instrument.RemoveUnitPrice;
                    instrument.RemoveUnitPrice = null;
                }
                else
                    instrument.RemoveUnitPriceUsd = null;

                if (instrument.MoveUnitPriceCurrencyType == CurrencyUtil.C_CURRENCY_US)
                {
                    instrument.MoveUnitPriceUsd = instrument.MoveUnitPrice;
                    instrument.MoveUnitPrice = null;
                }
                else
                    instrument.MoveUnitPriceUsd = null;

                #endregion
                #region Insert instrument data

                instrument.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                instrument.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                instrument.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                instrument.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                tbm_Instrument inserted = null;
                List<tbm_Instrument> listInserted = hand.InsertInstrument(instrument);
                if (listInserted.Count > 0)
                {
                    inserted = listInserted[0];
                }

                res.ResultData = inserted;

                #endregion
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Update instrument.<br />
        /// - Check system suspending.<br />
        /// - Validate require field.<br />
        /// - Check change expansion type.<br />
        /// - Update instrument data in database.
        /// </summary>
        /// <param name="instrument"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MAS090_UpdateInstrument(MAS090_SaveInstrument instrument)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                #region Check system suspending

                res = checkSystemSuspending();
                if (res.IsError)
                    return Json(res);

                #endregion

                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

                #region Validate require field

                ValidatorUtil validator = new ValidatorUtil(this);
                if (!instrument.SaleFlag.HasValue)
                {
                    instrument.SaleFlag = false;
                }
                if (!instrument.RentalFlag.HasValue)
                {
                    instrument.RentalFlag = false;
                }
                if (!instrument.InstrumentFlag.HasValue)
                {
                    instrument.InstrumentFlag = false;
                }
                if (!instrument.ZeroBahtAssetFlag.HasValue)
                {
                    instrument.ZeroBahtAssetFlag = false;
                }
                if (!instrument.MaintenanceFlag.HasValue)
                {
                    instrument.MaintenanceFlag = false;
                }
                if (!instrument.ControllerFlag.HasValue)
                {
                    instrument.ControllerFlag = false;
                }

                if (InstrumentType.C_INST_TYPE_GENERAL.Equals(instrument.InstrumentTypeCode))
                {
                    if (instrument.LineUpTypeCode == null)
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_MASTER,
                            MAS090_Screen,
                            MessageUtil.MODULE_COMMON,
                            MessageUtil.MessageList.MSG0007,
                            "RLineUpTypeCode",
                            "lblLineUpType",
                            "LineUpTypeCode");
                    }
                    if (instrument.ExpansionTypeCode == null)
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_MASTER,
                            MAS090_Screen,
                            MessageUtil.MODULE_COMMON,
                            MessageUtil.MessageList.MSG0007,
                            "RExpansionTypeCode",
                            "lblExpansionType",
                            "ExpansionTypeCode");
                    }
                }

                ValidatorUtil.BuildErrorMessage(res, validator);
                if (res.IsError)
                    return Json(res);

                #endregion
                #region If Have change expansion type

                instrument.currentExpansionTypeCode = instrument.currentExpansionTypeCode == "" ? null : instrument.currentExpansionTypeCode;

                IInstrumentMasterHandler hand = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                if ((instrument.currentExpansionTypeCode == null && instrument.ExpansionTypeCode != null) ||
                    (instrument.ExpansionTypeCode != null && !instrument.currentExpansionTypeCode.Equals(instrument.ExpansionTypeCode)))
                {
                    List<bool?> listCheck = hand.CheckExistParentChild(instrument.InstrumentCode);
                    if (listCheck[0].Value)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1022);
                        return Json(res);
                    }
                }

                #endregion


                //instrument.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                //instrument.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                MAS090_ScreenParameter MAS090Param = GetScreenObject<MAS090_ScreenParameter>();
                instrument.UpdateDate = MAS090Param.updateDate;

                // add by Jirawat Jannet on 216-12-23
                #region Set amount and currency type

                if (instrument.SaleUnitPriceCurrencyType == CurrencyUtil.C_CURRENCY_US)
                {
                    instrument.SaleUnitPriceUsd = instrument.SaleUnitPrice;
                    instrument.SaleUnitPrice = null;
                }
                else
                    instrument.SaleUnitPriceUsd = null;

                if (instrument.RentalUnitPriceCurrencyType == CurrencyUtil.C_CURRENCY_US)
                {
                    instrument.RentalUnitPriceUsd = instrument.RentalUnitPrice;
                    instrument.RentalUnitPrice = null;
                }
                else
                    instrument.RentalUnitPriceUsd = null;

                if (instrument.AddUnitPriceCurrencyType == CurrencyUtil.C_CURRENCY_US)
                {
                    instrument.AddUnitPriceUsd = instrument.AddUnitPrice;
                    instrument.AddUnitPrice = null;
                }
                else
                    instrument.AddUnitPriceUsd = null;

                if (instrument.RemoveUnitPriceCurrencyType == CurrencyUtil.C_CURRENCY_US)
                {
                    instrument.RemoveUnitPriceUsd = instrument.RemoveUnitPrice;
                    instrument.RemoveUnitPrice = null;
                }
                else
                    instrument.RemoveUnitPriceUsd = null;

                if (instrument.MoveUnitPriceCurrencyType == CurrencyUtil.C_CURRENCY_US)
                {
                    instrument.MoveUnitPriceUsd = instrument.MoveUnitPrice;
                    instrument.MoveUnitPrice = null;
                }
                else
                    instrument.MoveUnitPriceUsd = null;

                #endregion

                List<tbm_Instrument> updatedList = hand.UpdateInstrument(instrument);
                if (updatedList.Count > 0)
                    res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get instrument name match to cond for auto complete textbox.
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MAS090_GetInatrumentName(string cond)
        {
            ObjectResultData res = new ObjectResultData();
            List<string> listInstName = new List<string>();
            try
            {
                IAutoCompleteHandler handler = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<doInstrumentName> lst = handler.GetInstrumentName(cond);

                foreach (var item in lst)
                {
                    listInstName.Add(item.InstrumentName);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            res.ResultData = listInstName.ToArray();
            return Json(res);
        }
    }
}

