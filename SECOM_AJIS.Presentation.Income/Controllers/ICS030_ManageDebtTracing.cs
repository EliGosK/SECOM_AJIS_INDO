
//*********************************
// Create by: Waroon H.
// Create date: 29/Mar/2012
// Update date: 29/Mar/2012
//*********************************



using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Transactions;
using SECOM_AJIS.Presentation.Income.Models;

using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Installation;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Income;
using SECOM_AJIS.DataEntity.Contract;
using CSI.WindsorHelper;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.ActionFilters;

using System.ComponentModel;
using CrystalDecisions.Shared;
using CrystalDecisions.ReportSource;
using CrystalDecisions.CrystalReports.Engine;
using SECOM_AJIS.Presentation.Common;

namespace SECOM_AJIS.Presentation.Income.Controllers
{
    public partial class IncomeController : BaseController
    {
        /// <summary>
        /// Check screen authority and permission
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult ICS030_Authority(ICS030_ScreenParameter param)
        {

            ObjectResultData res = new ObjectResultData();
            //ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            
            //if (handlerCommon.IsSystemSuspending())
            //{
            //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
            //    return Json(res);
            //}

            if (!CheckUserPermission(ScreenID.C_SCREEN_ID_MANAGE_DEBT_TRACING, FunctionID.C_FUNC_ID_OPERATE))
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                return Json(res);
            }
 
            return InitialScreenEnvironment<ICS030_ScreenParameter>("ICS030", param, res);

        }
        /// <summary>
        /// Generate screen
        /// </summary>
        /// <returns></returns>
        [Initialize("ICS030")]
        public ActionResult ICS030()
        {

            ICS030_ScreenParameter param = GetScreenObject<ICS030_ScreenParameter>();
            List<doMiscTypeCode> lst = new List<doMiscTypeCode>();

            if (param != null)
            {
                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                {
                    new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_FLAG_DISPLAY,
                        ValueCode = "%"
                    }
                };

                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                lst = hand.GetMiscTypeCodeList(miscs);

                foreach (doMiscTypeCode l in lst)
                {
                    if (l.ValueCode == FlagDisplay.C_FLAG_DISPLAY_NO)
                    {
                        param.conNo = l.ValueDisplay;
                    }
                    if (l.ValueCode == FlagDisplay.C_FLAG_DISPLAY_YES)
                    {
                        param.conYes = l.ValueDisplay;
                    }
                }
            }
            return View();
        }

        /// <summary>
        /// Generate xml for initial debt actual table grid
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS030_InitialDebtActualTableGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Income\\ICS030_DebtActualTable", CommonUtil.GRID_EMPTY_TYPE.VIEW));
        }

        /// <summary>
        /// Generate xml for initial list of unpaid billing target by billing office grid
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS030_InitialListOfUnPaidBillingTargetByBillingOfficeGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Income\\ICS030_ListOfUnPaidBillingTargetByBillingOffice", CommonUtil.GRID_EMPTY_TYPE.VIEW));
        }

        /// <summary>
        /// Generate xml for initial list of upaid invoice by billing target grid
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS030_InitialListOfUnPaidInvoiceByBillingTargetGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Income\\ICS030_ListOfUnPaidInvoiceByBillingTarget", CommonUtil.GRID_EMPTY_TYPE.VIEW));
        }

        /// <summary>
        /// Generate xml for initial list of unpaid by billing target grid
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS030_InitialListOfUnPaidByBillingTargetGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Income\\ICS030_ListOfUnPaidByBillingTarget", CommonUtil.GRID_EMPTY_TYPE.VIEW));
        }

        /// <summary>
        /// Retrieve billing office dept summary information list of specific screen mode and search initial information
        /// </summary>
        /// <param name="data">Initaial criteria</param>
        /// <returns></returns>
        public ActionResult ICS030_LoadGetBillingOfficeDebtSummaryData(ICS030_RegisterData data)
        {
            ICS030_ScreenParameter param = GetScreenObject<ICS030_ScreenParameter>();
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            //  Add by Jirawat Jannet @ 2016-10-10
            #region Init new data model for uing new billing office debt summary table datas

            string localCurrency = MiscellaneousTypeCommon.getCurrencyName(CurrencyUtil.C_CURRENCY_LOCAL);
            string UsCurrency = MiscellaneousTypeCommon.getCurrencyName(CurrencyUtil.C_CURRENCY_US);

            #endregion

            try
            {
                if (param != null)
                {
                    //Comment by Jutarat A. on 06032012
                    //data.intMonth = data.RawdtpMonthYear.Month;
                    //data.intYear = data.RawdtpMonthYear.Year;

                    //IIncomeHandler iincomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                    //List<doGetBillingOfficeDebtSummary> _doGetBillingOfficeDebtSummaryList = iincomeHandler.GetBillingOfficeDebtSummaryList(data.intMonth, data.intYear);
                    //param.doGetBillingOfficeDebtSummaryList = _doGetBillingOfficeDebtSummaryList;
                    //param.doOfficeDataDo = CommonUtil.dsTransData.dtOfficeData;
                    //End Comment

                    //Add by Jutarat A. on 05032014
                    doTotalBillingOfficeDebt _doTotalBillingOfficeDebt = new doTotalBillingOfficeDebt();

                    // Add by Jirawat Jannet on 2016-10-10
                    doTotalBillingOfficeDebt _doTotalBillingOfficeDebtLocal = new doTotalBillingOfficeDebt();
                    doTotalBillingOfficeDebt _doTotalBillingOfficeDebtUs = new doTotalBillingOfficeDebt();
                    if (param.doNewGetBillingOfficeDebtSummaryList != null && param.doNewGetBillingOfficeDebtSummaryList.Count > 0)
                    {
                        _doTotalBillingOfficeDebt = (from t in param.doGetBillingOfficeDebtSummaryList
                                                     group t by t.GroupTotal into g
                                                     select new doTotalBillingOfficeDebt
                                                     {
                                                         TotalUnpaidAmount = g.Sum(p => p.UnpaidAmount),
                                                         TotalUnpaidAmount2Month = g.Sum(p => p.UnpaidAmount2Month),
                                                         TotalUnpaidAmount6Month = g.Sum(p => p.UnpaidAmount6Month),
                                                         TotalUnpaidDetail = g.Sum(p => p.UnpaidDetail),
                                                         TotalUnpaidDetail2Month = g.Sum(p => p.UnpaidDetail2Month),
                                                         TotalUnpaidDetail6Month = g.Sum(p => p.UnpaidDetail6Month),
                                                         TotalTargetAmountAll = g.Sum(p => p.TargetAmountAll),
                                                         TotalTargetAmount2Month = g.Sum(p => p.TargetAmount2Month),
                                                         TotalTargetDetailAll = g.Sum(p => p.TargetDetailAll),
                                                         TotalTargetDetail2Month = g.Sum(p => p.TargetDetail2Month)
                                                     }).FirstOrDefault<doTotalBillingOfficeDebt>();

                        // add by Jirawat Jannet
                        _doTotalBillingOfficeDebtLocal = (from t in param.doNewGetBillingOfficeDebtSummaryList
                                                          where t.Currency == localCurrency
                                                          group t by t.Currency into g
                                                          select new doTotalBillingOfficeDebt
                                                          {
                                                              Currency = g.Key,
                                                              TotalUnpaidAmount = g.Sum(p => p.UnpaidAmount),
                                                              TotalUnpaidAmount2Month = g.Sum(p => p.UnpaidAmount2Month),
                                                              TotalUnpaidAmount6Month = g.Sum(p => p.UnpaidAmount6Month),
                                                              TotalUnpaidDetail = g.Sum(p => p.UnpaidDetail),
                                                              TotalUnpaidDetail2Month = g.Sum(p => p.UnpaidDetail2Month),
                                                              TotalUnpaidDetail6Month = g.Sum(p => p.UnpaidDetail6Month),
                                                              TotalTargetAmountAll = g.Sum(p => p.TargetAmountAll),
                                                              TotalTargetAmount2Month = g.Sum(p => p.TargetAmount2Month),
                                                              TotalTargetDetailAll = g.Sum(p => p.TargetDetailAll),
                                                              TotalTargetDetail2Month = g.Sum(p => p.TargetDetail2Month)
                                                          }).FirstOrDefault<doTotalBillingOfficeDebt>();
                        // add by Jirawat Jannet
                        _doTotalBillingOfficeDebtUs = (from t in param.doNewGetBillingOfficeDebtSummaryList
                                                       where t.Currency == UsCurrency
                                                       group t by t.Currency into g
                                                       select new doTotalBillingOfficeDebt
                                                       {
                                                           Currency = g.Key,
                                                           TotalUnpaidAmount = g.Sum(p => p.UnpaidAmount),
                                                           TotalUnpaidAmount2Month = g.Sum(p => p.UnpaidAmount2Month),
                                                           TotalUnpaidAmount6Month = g.Sum(p => p.UnpaidAmount6Month),
                                                           TotalUnpaidDetail = g.Sum(p => p.UnpaidDetail),
                                                           TotalUnpaidDetail2Month = g.Sum(p => p.UnpaidDetail2Month),
                                                           TotalUnpaidDetail6Month = g.Sum(p => p.UnpaidDetail6Month),
                                                           TotalTargetAmountAll = g.Sum(p => p.TargetAmountAll),
                                                           TotalTargetAmount2Month = g.Sum(p => p.TargetAmount2Month),
                                                           TotalTargetDetailAll = g.Sum(p => p.TargetDetailAll),
                                                           TotalTargetDetail2Month = g.Sum(p => p.TargetDetail2Month)
                                                       }).FirstOrDefault<doTotalBillingOfficeDebt>();
                    }

                    param.doTotalBillingOfficeDebt = _doTotalBillingOfficeDebt;
                    param.doTotalBillingOfficeDebtLocal = _doTotalBillingOfficeDebtLocal; // add by Jirawat Jannet
                    param.doTotalBillingOfficeDebtUs = _doTotalBillingOfficeDebtUs; // add by Jirawat Jannet
                    //End Add
                }

                // Save RegisterData in session
                if (param != null)
                {
                    param.RegisterData = data;
                }
                // return "1" to js is every thing OK
                if (res.MessageList == null || res.MessageList.Count == 0)
                { res.ResultData = param; }
                else
                { res.ResultData = null; }
                return Json(res);
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        //Add by Jutarat A. on 05032014
        /// <summary>
        /// Get BillingOfficeDebtSummary list
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS030_LoadGetBillingOfficeDebtSummaryToGrid(ICS030_RegisterData data)
        {
            ICS030_ScreenParameter param = GetScreenObject<ICS030_ScreenParameter>();
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            IIncomeHandler iincomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
            List<doGetBillingOfficeDebtSummary> _doGetBillingOfficeDebtSummaryList = new List<doGetBillingOfficeDebtSummary>();
            List<OfficeDataDo> _doOfficeDataDo = new List<OfficeDataDo>();

            //  Add by Jirawat Jannet @ 2016-10-10
            #region Init new data model for uing new billing office debt summary table datas

            string localCurrency = MiscellaneousTypeCommon.getCurrencyName(CurrencyUtil.C_CURRENCY_LOCAL);
            string UsCurrency = MiscellaneousTypeCommon.getCurrencyName(CurrencyUtil.C_CURRENCY_US);

            List<ICS030_DebtActualTableData> _doDebtActualTabledatas = new List<ICS030_DebtActualTableData>();

            #endregion


            try
            {
                data.intMonth = data.RawdtpMonthYear.Month;
                data.intYear = data.RawdtpMonthYear.Year;

                _doGetBillingOfficeDebtSummaryList = iincomeHandler.GetBillingOfficeDebtSummaryList(data.intMonth, data.intYear);
                _doOfficeDataDo = CommonUtil.dsTransData.dtOfficeData;

                if (_doGetBillingOfficeDebtSummaryList != null && _doGetBillingOfficeDebtSummaryList.Count > 0)
                {
                    foreach (doGetBillingOfficeDebtSummary billingData in _doGetBillingOfficeDebtSummaryList)
                    {
                        string strDisableLinkOfficeFlag = "1";
                        for (int i = 0; i < _doOfficeDataDo.Count; i++)
                        {
                            if (billingData.BillingOfficeCode == _doOfficeDataDo[i].OfficeCode)
                            {
                                strDisableLinkOfficeFlag = "0";
                                break;
                            }
                        }

                        billingData.DisableLinkOfficeFlag = strDisableLinkOfficeFlag;
                    }

                    #region Initial new datas for this table

                    foreach (var item in _doGetBillingOfficeDebtSummaryList)
                    {
                        #region Local currency

                        // local currency line 1
                        _doDebtActualTabledatas.Add(new ICS030_DebtActualTableData()
                        {
                            BillingOffice = item.BillingOffice,
                            BillingOfficeCode = item.BillingOfficeCode,
                            DisableLinkOfficeFlag = item.DisableLinkOfficeFlag,
                            BillingOfficeName = item.BillingOfficeName,
                            Currency = localCurrency,
                            AllUnpaidActual = item.UnpaidAmountString,
                            AllUnpaidTarget = item.TargetAmountAllString,
                            AllUnpaidCompareTotarget = item.TargetAmountAllShow,
                            UnpaidOver2MonthActual = item.UnpaidAmount2MonthString,
                            UnpaidOver2MonthTarget = item.TargetAmount2MonthString,
                            UnpaidOver2MonthCompareTotarget = item.TargetAmount2MonthShow,
                            UnpaidOver6Month = item.UnpaidAmount6MonthString,

                            UnpaidAmount = item.UnpaidAmount,
                            UnpaidAmount2Month = item.UnpaidAmount2Month,
                            UnpaidAmount6Month = item.UnpaidAmount6Month,
                            UnpaidDetail = item.UnpaidDetail,
                            UnpaidDetail2Month = item.UnpaidDetail2Month,
                            UnpaidDetail6Month = item.UnpaidDetail6Month,
                            TargetAmountAll = item.TargetAmountAll,
                            TargetAmount2Month = item.TargetAmount2Month,
                            TargetDetailAll = item.TargetDetailAll,
                            TargetDetail2Month = item.TargetDetail2Month
                        });
                        // local currency line 2
                        _doDebtActualTabledatas.Add(new ICS030_DebtActualTableData()
                        {
                            BillingOffice = item.BillingOffice,
                            BillingOfficeCode = item.BillingOfficeCode,
                            DisableLinkOfficeFlag = item.DisableLinkOfficeFlag,
                            BillingOfficeName = item.BillingOfficeName,
                            Currency = localCurrency,
                            AllUnpaidActual = item.UnpaidDetailString,
                            AllUnpaidTarget = item.TargetDetailAllString,
                            AllUnpaidCompareTotarget = item.TargetDetailAllShow,
                            UnpaidOver2MonthActual = item.UnpaidDetail2MonthString,
                            UnpaidOver2MonthTarget = item.TargetDetail2MonthString,
                            UnpaidOver2MonthCompareTotarget = item.TargetDetail2MonthShow,
                            UnpaidOver6Month = item.UnpaidDetail6MonthString,

                            UnpaidAmount = 0,
                            UnpaidAmount2Month = 0,
                            UnpaidAmount6Month = 0,
                            UnpaidDetail = 0,
                            UnpaidDetail2Month = 0,
                            UnpaidDetail6Month = 0,
                            TargetAmountAll = 0,
                            TargetAmount2Month = 0,
                            TargetDetailAll = 0,
                            TargetDetail2Month = 0
                        });

                        #endregion

                        #region US Currency

                        // us currency line 1
                        _doDebtActualTabledatas.Add(new ICS030_DebtActualTableData()
                        {
                            BillingOffice = item.BillingOffice,
                            BillingOfficeCode = item.BillingOfficeCode,
                            DisableLinkOfficeFlag = item.DisableLinkOfficeFlag,
                            BillingOfficeName = item.BillingOfficeName,
                            Currency = UsCurrency,
                            AllUnpaidActual = item.UnpaidAmountUsdString,
                            AllUnpaidTarget = item.TargetAmountAllUsdString,
                            AllUnpaidCompareTotarget = item.TargetAmountAllUsdShow,
                            UnpaidOver2MonthActual = item.UnpaidAmount2MonthUsdString,
                            UnpaidOver2MonthTarget = item.TargetAmount2MonthUsdString,
                            UnpaidOver2MonthCompareTotarget = item.TargetAmount2MonthUsdShow,
                            UnpaidOver6Month = item.UnpaidAmount6MonthUsdString,

                            UnpaidAmount = item.UnpaidAmountUsd,
                            UnpaidAmount2Month = item.UnpaidAmount2MonthUsd,
                            UnpaidAmount6Month = item.UnpaidAmount6MonthUsd,
                            UnpaidDetail = item.UnpaidDetailUsd,
                            UnpaidDetail2Month = item.UnpaidDetail2MonthUsd,
                            UnpaidDetail6Month = item.UnpaidDetail6MonthUsd,
                            TargetAmountAll = item.TargetAmountAllUsd,
                            TargetAmount2Month = item.TargetAmount2MonthUsd,
                            TargetDetailAll = item.TargetDetailAllUsd,
                            TargetDetail2Month = item.TargetDetail2MonthUsd,
                        });
                        // us currency line 2
                        _doDebtActualTabledatas.Add(new ICS030_DebtActualTableData()
                        {
                            BillingOffice = item.BillingOffice,
                            BillingOfficeCode = item.BillingOfficeCode,
                            DisableLinkOfficeFlag = item.DisableLinkOfficeFlag,
                            BillingOfficeName = item.BillingOfficeName,
                            Currency = UsCurrency,
                            AllUnpaidActual = item.UnpaidDetailUsdString,
                            AllUnpaidTarget = item.TargetDetailAllUsdString,
                            AllUnpaidCompareTotarget = item.TargetDetailAllUsdShow,
                            UnpaidOver2MonthActual = item.UnpaidDetail2MonthUsdString,
                            UnpaidOver2MonthTarget = item.TargetDetail2MonthUsdString,
                            UnpaidOver2MonthCompareTotarget = item.TargetDetail2MonthUsdShow,
                            UnpaidOver6Month = item.UnpaidDetail6MonthUsdString,

                            UnpaidAmount = 0,
                            UnpaidAmount2Month = 0,
                            UnpaidAmount6Month = 0,
                            UnpaidDetail = 0,
                            UnpaidDetail2Month = 0,
                            UnpaidDetail6Month = 0,
                            TargetAmountAll = 0,
                            TargetAmount2Month = 0,
                            TargetDetailAll = 0,
                            TargetDetail2Month = 0
                        });

                        #endregion

                    }

                    #endregion

                }

                if (param != null)
                {
                    param.doGetBillingOfficeDebtSummaryList = _doGetBillingOfficeDebtSummaryList;
                    param.doNewGetBillingOfficeDebtSummaryList = _doDebtActualTabledatas;
                    param.doOfficeDataDo = _doOfficeDataDo;
                }
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
                return Json(res);
            }

            //res.ResultData = CommonUtil.ConvertToXml<doGetBillingOfficeDebtSummary>(_doGetBillingOfficeDebtSummaryList, "Income\\ICS030_DebtActualTable", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            res.ResultData = CommonUtil.ConvertToXml<ICS030_DebtActualTableData>(_doDebtActualTabledatas, "Income\\ICS030_DebtActualTable", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            return Json(res);
        }
        //End Add

        /// <summary>
        /// Retrieve billing target dept summary information list of specific screen mode and search criteria information
        /// </summary>
        /// <param name="data">Search criteria from link</param>
        /// <returns></returns>
        public ActionResult ICS030_LoadGetBillingTargetDebtSummaryByOfficeData(ICS030_RegisterData data)
        {
            ICS030_ScreenParameter param = GetScreenObject<ICS030_ScreenParameter>();
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            List<doGetBillingTargetDebtSummaryByOffice> _doGetBillingTargetDebtSummaryByOfficeList = new List<doGetBillingTargetDebtSummaryByOffice>();
            try
            {
                if (param != null)
                {
                    data.intMonth = data.RawdtpMonthYear.Month;
                    data.intYear = data.RawdtpMonthYear.Year;

                    IIncomeHandler iincomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                    _doGetBillingTargetDebtSummaryByOfficeList = iincomeHandler.GetBillingTargetDebtSummaryByOfficeList(data.strOfficeCode, data.intMonth, data.intYear);

                    foreach (var item in _doGetBillingTargetDebtSummaryByOfficeList)
                    {
                        if (item.DebtTracingRegisteredString == "Yes")
                            item.DebtTracingRegisteredGridFormat = param.conYes;
                        else
                            item.DebtTracingRegisteredGridFormat = param.conNo;

                        if (item.IncludeFirstFee == 1)
                            item.IncludeFirstFeeGridFormat = param.conYes;
                        else
                            item.IncludeFirstFeeGridFormat = param.conNo;
                    }
                    
                    // Save RegisterData in session
                    param.RegisterData = data;
                }
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
                return Json(res);
            }
            res.ResultData = CommonUtil.ConvertToXml<doGetBillingTargetDebtSummaryByOffice>(_doGetBillingTargetDebtSummaryByOfficeList, "Income\\ICS030_ListOfUnPaidBillingTargetByBillingOffice", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            return Json(res);
        }

        /// <summary>
        /// Retrieve unpaid invoice dept summary information list of specific screen mode and search criteria information
        /// </summary>
        /// <param name="data">Search criteria from link</param>
        /// <returns></returns>
        public ActionResult ICS030_LoadGetUnpaidInvoiceDebtSummaryByBillingTargetData(ICS030_RegisterData data)
        {
            ICS030_ScreenParameter param = GetScreenObject<ICS030_ScreenParameter>();
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            List<doGetUnpaidInvoiceDebtSummaryByBillingTarget> _doGetUnpaidInvoiceDebtSummaryByBillingTargetList = new List<doGetUnpaidInvoiceDebtSummaryByBillingTarget>();
            try
            {
                if (param != null)
                {
                    data.intMonth = data.RawdtpMonthYear.Month;
                    data.intYear = data.RawdtpMonthYear.Year;

                    CommonUtil comUtil = new CommonUtil();
                    IIncomeHandler iincomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                    _doGetUnpaidInvoiceDebtSummaryByBillingTargetList = iincomeHandler.GetUnpaidInvoiceDebtSummaryByBillingTargetList(
                        comUtil.ConvertContractCode(data.strBillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG), data.strOfficeCode); //Add (strOfficeCode) by Jutarat A. on 10042014

                    if (_doGetUnpaidInvoiceDebtSummaryByBillingTargetList != null)
                    {
                        foreach (var item in _doGetUnpaidInvoiceDebtSummaryByBillingTargetList)
                        {
                            if (item.TracingResultRegisteredString == "Yes")
                                item.DebtTracingRegisteredGridFormat = param.conYes;
                            else
                                item.DebtTracingRegisteredGridFormat = param.conNo;

                            if (item.IncludeFirstFee == 1)
                                item.IncludeFirstFeeGridFormat = param.conYes;
                            else
                                item.IncludeFirstFeeGridFormat = param.conNo;
                        }
                    }

                    // Save RegisterData in session
                    param.RegisterData = data;
                }
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
                return Json(res);
            }
            res.ResultData = CommonUtil.ConvertToXml<doGetUnpaidInvoiceDebtSummaryByBillingTarget>(_doGetUnpaidInvoiceDebtSummaryByBillingTargetList, "Income\\ICS030_ListOfUnPaidInvoiceByBillingTarget", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            return Json(res);
        }

        /// <summary>
        /// Retrieve unpaid detail dept summary information by billingtarget list of specific screen mode and search criteria information
        /// </summary>
        /// <param name="data">Search criteria from link</param>
        /// <returns></returns>
        public ActionResult ICS030_LoadGetUnpaidDetailDebtSummaryByBillingTargetData(ICS030_RegisterData data)
        {
            ICS030_ScreenParameter param = GetScreenObject<ICS030_ScreenParameter>();
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            List<doGetUnpaidDetailDebtSummary> _doGetUnpaidDetailDebtSummaryList = new List<doGetUnpaidDetailDebtSummary>();
            try
            {
                if (param != null)
                {
                    data.intMonth = data.RawdtpMonthYear.Month;
                    data.intYear = data.RawdtpMonthYear.Year;

                    CommonUtil comUtil = new CommonUtil();
                    IIncomeHandler iincomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                    _doGetUnpaidDetailDebtSummaryList = iincomeHandler.GetUnpaidDetailDebtSummaryByBillingTargetList(
                        comUtil.ConvertContractCode(data.strBillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG), data.strOfficeCode); //Add (strOfficeCode) by Jutarat A. on 11042014

                    foreach (var item in _doGetUnpaidDetailDebtSummaryList)
                    {
                        if (item.DebtTracingRegistered == 1)
                            item.DebtTracingRegisteredGridFormat = param.conYes;
                        else
                            item.DebtTracingRegisteredGridFormat = param.conNo;
                    }

                    // Save RegisterData in session
                    param.RegisterData = data;
                }
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
                return Json(res);
            }
            res.ResultData = CommonUtil.ConvertToXml<doGetUnpaidDetailDebtSummary>(_doGetUnpaidDetailDebtSummaryList, "Income\\ICS030_ListOfUnPaidByBillingTarget", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            return Json(res);
        }

        /// <summary>
        /// Retrieve unpaid detail dept summary information list by invoice of specific screen mode and search criteria information
        /// </summary>
        /// <param name="data">Search criteria from link</param>
        /// <returns></returns>
        public ActionResult ICS030_LoadGetUnpaidDetailDebtSummaryByInvoiceData(ICS030_RegisterData data)
        {
            ICS030_ScreenParameter param = GetScreenObject<ICS030_ScreenParameter>();
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            List<doGetUnpaidDetailDebtSummary> _doGetUnpaidDetailDebtSummaryList = new List<doGetUnpaidDetailDebtSummary>();
            try
            {
                if (param != null)
                {
                    data.intMonth = data.RawdtpMonthYear.Month;
                    data.intYear = data.RawdtpMonthYear.Year;

                    IIncomeHandler iincomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                    _doGetUnpaidDetailDebtSummaryList = iincomeHandler.GetUnpaidDetailDebtSummaryByInvoiceList(
                        data.strInvoiceNo, (int?)Convert.ToInt32(data.strInvoiceOCC), data.strOfficeCode); //Add (strOfficeCode) by Jutarat A. on 11042014

                    foreach (var item in _doGetUnpaidDetailDebtSummaryList)
                    {
                        if (item.DebtTracingRegistered == 1)
                            item.DebtTracingRegisteredGridFormat = param.conYes;
                        else
                            item.DebtTracingRegisteredGridFormat = param.conNo;
                    }

                    // Save RegisterData in session
                    param.RegisterData = data;
                }
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
                return Json(res);
            }
            res.ResultData = CommonUtil.ConvertToXml<doGetUnpaidDetailDebtSummary>(_doGetUnpaidDetailDebtSummaryList, "Income\\ICS030_ListOfUnPaidByBillingTarget", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            return Json(res);
        }

        /// <summary>
        /// Check ics032 popup screen authority and permission
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public ActionResult ICS030_CheckForICS032()
        {
            // 1 = ok
            // 2 = Suspending
            // 3 = no permission
            string ResultDataFlag = "1";
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                // System Suspend
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (handlerCommon.IsSystemSuspending())
                {
                    ResultDataFlag = "2";
                    res.ResultData = ResultDataFlag;
                    return Json(res);
                }

                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_DEBT_TRACING_INFO, FunctionID.C_FUNC_ID_OPERATE))
                {
                    ResultDataFlag = "3";
                    res.ResultData = ResultDataFlag;
                    return Json(res);
                }

                res.ResultData = ResultDataFlag;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }
    }
}
