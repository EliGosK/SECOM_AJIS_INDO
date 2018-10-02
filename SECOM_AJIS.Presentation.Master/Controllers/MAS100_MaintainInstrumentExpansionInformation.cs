//*********************************
// Create by: Attawhit  Chuoosathan
// Create date: 23/Jun/2010
// Update date: 23/Jun/2010
//*********************************

using System;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Master;
using CSI.WindsorHelper;
using SECOM_AJIS.Presentation.Master.Models;
using SECOM_AJIS.DataEntity.Common;
using System.Transactions;
using System.Text;



namespace SECOM_AJIS.Presentation.Master.Controllers
{
    public partial class MasterController : BaseController
    {
        private const string MAS100_Screen = "MAS100";

        #region Authority

        /// <summary>
        /// - Check user permission for screen MAS100.<br />
        /// - Check system suspending.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult MAS100_Authority(MAS100_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (!(CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_INSTRUMENT_EXPANSION_INFO, FunctionID.C_FUNC_ID_VIEW) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_INSTRUMENT_EXPANSION_INFO, FunctionID.C_FUNC_ID_ADD) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_INSTRUMENT_EXPANSION_INFO, FunctionID.C_FUNC_ID_DEL) == true
                    ))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                //Check system suspending
                res = checkSystemSuspending();
                if (res.IsError)
                    return Json(res);
                // Do in view
                //param.HasAddPermission = CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_INSTRUMENT_EXPANSION_INFO, FunctionID.C_FUNC_ID_ADD);
                //param.HasDeletePermission = CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_INSTRUMENT_EXPANSION_INFO, FunctionID.C_FUNC_ID_DEL);
                //param.SearchInstrumentExpansion = new List<doInstrumentExpansion>();
                //param.AddInstrumentExpansion = new List<doInstrumentExpansion>();
                //param.DelInstrumentExpansion = new List<doInstrumentExpansion>();
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<MAS100_ScreenParameter>(MAS100_Screen, param, res);
        }
        
        #endregion

        #region View

        /// <summary>
        /// Initial screen.
        /// </summary>
        /// <returns></returns>
        [Initialize(MAS100_Screen)]
        public ActionResult MAS100()
        {
            MAS100_ScreenParameter MAS100Param = new MAS100_ScreenParameter();
            ViewBag.HasAddPermission = "";
            ViewBag.HasDeletePermission = "";
            ViewBag.PageRow = CommonValue.ROWS_PER_PAGE_FOR_VIEWPAGE;
            ViewBag.InstExpParent = ExpansionType.C_EXPANSION_TYPE_PARENT;
            ViewBag.InstExpChild = ExpansionType.C_EXPANSION_TYPE_CHILD;

            try
            {
                MAS100Param = GetScreenObject<MAS100_ScreenParameter>();
                ViewBag.HasAddPermission = CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_INSTRUMENT_EXPANSION_INFO, FunctionID.C_FUNC_ID_ADD);
                ViewBag.HasDeletePermission = CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_INSTRUMENT_EXPANSION_INFO, FunctionID.C_FUNC_ID_DEL);
                MAS100Param.SearchInstrumentExpansion = new List<doInstrumentExpansion>();
                MAS100Param.AddInstrumentExpansion = new List<doInstrumentExpansion>();
                MAS100Param.DelInstrumentExpansion = new List<doInstrumentExpansion>();
                ViewBag.PageRow = CommonValue.ROWS_PER_PAGE_FOR_VIEWPAGE;
                ViewBag.InstExpParent = ExpansionType.C_EXPANSION_TYPE_PARENT;
                ViewBag.InstExpChild = ExpansionType.C_EXPANSION_TYPE_CHILD;
            }
            catch 
            {
            }

            return View(MAS100_Screen);
        }
        #endregion

        /// <summary>
        /// Get config for instrument table.
        /// </summary>
        /// <returns></returns>
        public ActionResult InitialGrid_MAS100()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Master\\MAS100", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }

        /// <summary>
        /// Check require field.<br />
        /// - ParentInstrumentCode must not null or empty.<br />
        /// - Check exist ParentInstrumentCode.<br />
        /// - Get information of ParentInstrumentCode.
        /// </summary>
        /// <param name="ParentInstrumentCode"></param>
        /// <returns></returns>
        public ActionResult MAS100_CheckReqField(String ParentInstrumentCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (CommonUtil.IsNullOrEmpty(ParentInstrumentCode))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_MASTER,
                                        MAS100_Screen,
                                        MessageUtil.MODULE_MASTER, 
                                        MessageUtil.MessageList.MSG1040,
                                        new string[] { "lblParentinstrumentcode" }, 
                                        new string[] { "ParentInstrumentCode" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }

                //Check exist parent instrument
                IInstrumentMasterHandler hand = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                List<doParentInstrument> doParentInst = hand.GetParentInstrument(ParentInstrumentCode);
                if (doParentInst.Count <= 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_MASTER,
                                        MAS100_Screen,
                                        MessageUtil.MODULE_MASTER,
                                        MessageUtil.MessageList.MSG1043,
                                        new string[] { "lblParentinstrumentcode" },
                                        new string[] { "ParentInstrumentCode" });
                    return Json(res);
                }

                //Load data GetInstrumentExpansion
                List<doInstrumentExpansion> lst = hand.GetInstrumentExpansion(ParentInstrumentCode);

                MAS100_ScreenParameter MAS100Param = GetScreenObject<MAS100_ScreenParameter>();
                MAS100Param.ParentInstrumentCode = doParentInst[0].ParentInstruementCode;
                MAS100Param.ParentInstrumentName = doParentInst[0].ParentInstruementName;
                MAS100Param.SearchInstrumentExpansion = lst;

                //Clear AddInstrumentExpansion & DelInstrumentExpansion
                MAS100Param.AddInstrumentExpansion = new List<doInstrumentExpansion>();
                MAS100Param.DelInstrumentExpansion = new List<doInstrumentExpansion>();

                UpdateScreenObject(MAS100Param);

                res.ResultData = MAS100Param.ParentInstrumentName;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Retrieve parent instrument data.<br />
        /// - If ParentInstrumentCode is null or empty get information from session.<br />
        /// - If ParentInstrumentCode is not null or empty get information of ParentInstrumentCode.
        /// </summary>
        /// <param name="ParentInstrumentCode"></param>
        /// <returns></returns>
        public ActionResult MAS100_Retrieve(String ParentInstrumentCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                MAS100_ScreenParameter MAS100Param = GetScreenObject<MAS100_ScreenParameter>();
                List<doInstrumentExpansion> lst = new List<doInstrumentExpansion>();
                
                if (CommonUtil.IsNullOrEmpty(ParentInstrumentCode)) //from click retrieve
                {
                    lst = MAS100Param.SearchInstrumentExpansion;
                }
                else //from click search parent instrument
                {
                    //Check exist parent instrument
                    IInstrumentMasterHandler hand = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                    List<doParentInstrument> doParentInst = hand.GetParentInstrument(ParentInstrumentCode);
                    if (doParentInst.Count <= 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_MASTER,
                                        MAS100_Screen,
                                        MessageUtil.MODULE_MASTER,
                                        MessageUtil.MessageList.MSG1043,
                                        new string[] { "lblParentinstrument" },
                                        new string[] { "ParentInstrumentCode" });
                        return Json(res);
                    }

                    lst = hand.GetInstrumentExpansion(ParentInstrumentCode);
                    MAS100Param.ParentInstrumentCode = doParentInst[0].ParentInstruementCode;
                    MAS100Param.ParentInstrumentName = doParentInst[0].ParentInstruementName;
                    MAS100Param.SearchInstrumentExpansion = lst;

                    //Clear AddInstrumentExpansion & DelInstrumentExpansion
                    MAS100Param.AddInstrumentExpansion = new List<doInstrumentExpansion>();
                    MAS100Param.DelInstrumentExpansion = new List<doInstrumentExpansion>();

                    UpdateScreenObject(MAS100Param);
                }

                res.ResultData = CommonUtil.ConvertToXml<doInstrumentExpansion>(lst, "Master\\MAS100", CommonUtil.GRID_EMPTY_TYPE.INSERT);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Add child instrument.<br />
        /// - ChildInstrumentCode must not null or empty.<br />
        /// - Check is ChildInstrumentCode already register.<br />
        /// - Add to child instrument.
        /// </summary>
        /// <param name="ChildInstrumentCode"></param>
        /// <returns></returns>
        public ActionResult MAS100_Add(String ChildInstrumentCode)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                if (CommonUtil.IsNullOrEmpty(ChildInstrumentCode))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_MASTER,
                                        MAS100_Screen,
                                        MessageUtil.MODULE_COMMON,
                                        MessageUtil.MessageList.MSG0007,
                                        new string[] { "lblChildInstrumentCode" },
                                        new string[] { "ChildInstrumentCode" });
                    return Json(res);
                }

                MAS100_ScreenParameter MAS100Param = GetScreenObject<MAS100_ScreenParameter>();
                doInstrumentExpansion doInstSearch = MAS100Param.SearchInstrumentExpansion.Find(i => i.InstrumentCode.Trim().ToLower() == ChildInstrumentCode.Trim().ToLower());

                if (!CommonUtil.IsNullOrEmpty(doInstSearch))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_MASTER,
                                        MAS100_Screen,
                                        MessageUtil.MODULE_MASTER,
                                        MessageUtil.MessageList.MSG1026,
                                        new string[] { "lblChildInstrumentCode" },
                                        new string[] { "ChildInstrumentCode" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }
                else
                {
                    doInstrumentExpansion doInstDel = MAS100Param.DelInstrumentExpansion.Find(i => i.InstrumentCode.Trim().ToLower() == ChildInstrumentCode.Trim().ToLower());

                    if (!CommonUtil.IsNullOrEmpty(doInstDel))
                    {
                        MAS100Param.DelInstrumentExpansion.Remove(doInstDel);
                        MAS100Param.SearchInstrumentExpansion.Add(doInstDel);
                        UpdateScreenObject(MAS100Param);
                        res.ResultData = doInstDel;
                    }
                    else
                    {
                        IInstrumentMasterHandler hand = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                        List<doInstrumentExpansion> doInstAdd = hand.GetChildInstrument(ChildInstrumentCode);
                        if (doInstAdd.Count <= 0)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_MASTER,
                                        MAS100_Screen,
                                        MessageUtil.MODULE_MASTER,
                                        MessageUtil.MessageList.MSG1020,
                                        new string[] { "lblChildInstrumentCode" },
                                        new string[] { "ChildInstrumentCode" });
                            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                            return Json(res);
                        }
                        else
                        {
                            MAS100Param.AddInstrumentExpansion.Add(doInstAdd[0]);
                            MAS100Param.SearchInstrumentExpansion.Add(doInstAdd[0]);
                            UpdateScreenObject(MAS100Param);
                            res.ResultData = doInstAdd[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Remove selected child instrument.
        /// </summary>
        /// <param name="ChildInstrumentCode"></param>
        /// <returns></returns>
        public ActionResult MAS100_Remove(String ChildInstrumentCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                MAS100_ScreenParameter MAS100Param = GetScreenObject<MAS100_ScreenParameter>();
                doInstrumentExpansion doInstSearch = MAS100Param.SearchInstrumentExpansion.Find(i => i.InstrumentCode.Trim().ToLower() == ChildInstrumentCode.Trim().ToLower());
                MAS100Param.SearchInstrumentExpansion.Remove(doInstSearch);

                doInstrumentExpansion doInstAdd = MAS100Param.AddInstrumentExpansion.Find(i => i.InstrumentCode.Trim().ToLower() == ChildInstrumentCode.Trim().ToLower());

                if (!CommonUtil.IsNullOrEmpty(doInstAdd))
                    MAS100Param.AddInstrumentExpansion.Remove(doInstAdd);
                else
                    MAS100Param.DelInstrumentExpansion.Add(doInstSearch);

                UpdateScreenObject(MAS100Param);

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Update operation to database.<br />
        /// - Check is ChildInstrumentCode already register.<br />
        /// - Check system suspending.<br />
        /// - Add all added instrument to database.<br />
        /// - Remove all removed instrument from database.
        /// </summary>
        /// <param name="ChildInstrumentCode"></param>
        /// <returns></returns>
        public ActionResult MAS100_Confirm()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                MAS100_ScreenParameter MAS100Param = GetScreenObject<MAS100_ScreenParameter>();
                
                IInstrumentMasterHandler hand = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                if (MAS100Param.AddInstrumentExpansion.Count > 0)
                {
                    List<tbm_InstrumentExpansion>  list 
                        = hand.CheckExistInstrumentExpansion(MAS100Param.ParentInstrumentCode, MAS100Param.AddInstrumentExpansion);

                    if (list.Count > 0)
                    {
                        StringBuilder sbChildInstrumentCode = new StringBuilder("");
                        foreach (tbm_InstrumentExpansion inst in list)
                        {
                            sbChildInstrumentCode.AppendFormat("\'{0}\',", inst.ChildInstrumentCode);
                        }
                        string param = sbChildInstrumentCode.Remove(sbChildInstrumentCode.Length - 1, 1).ToString();

                        res.AddErrorMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1021, new string[] { param });
                        return Json(res);
                    }
                }

                res = this.checkSystemSuspending();
                if (res.IsError)
                    return Json(res);

                using (TransactionScope scope = new TransactionScope())
                {
                    //Insert instrument expansion data
                    foreach (doInstrumentExpansion doInst in MAS100Param.AddInstrumentExpansion)
                    {
                        tbm_InstrumentExpansion tbmInst = new tbm_InstrumentExpansion();
                        tbmInst.InstrumentCode = MAS100Param.ParentInstrumentCode;
                        tbmInst.ChildInstrumentCode = doInst.InstrumentCode;
                        hand.InsertInstrumentExpansion(tbmInst);
                    }

                    //Delete instrument expansion data
                    foreach (doInstrumentExpansion doInst in MAS100Param.DelInstrumentExpansion)
                    {
                        tbm_InstrumentExpansion tbmInst = new tbm_InstrumentExpansion();
                        tbmInst.InstrumentCode = MAS100Param.ParentInstrumentCode;
                        tbmInst.ChildInstrumentCode = doInst.InstrumentCode;
                        tbmInst.UpdateDate = doInst.UpdateDate;
                        tbmInst.UpdateBy = doInst.UpdateBy;
                        hand.DeleteInstrumentExpansion(tbmInst);
                    }

                    //clear data in session
                    MAS100Param.SearchInstrumentExpansion = new List<doInstrumentExpansion>();
                    MAS100Param.AddInstrumentExpansion = new List<doInstrumentExpansion>();
                    MAS100Param.DelInstrumentExpansion = new List<doInstrumentExpansion>();
                    UpdateScreenObject(MAS100Param);

                    //when finish with out error
                    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0046);

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        //------------------------------------------------------------------------------------------------------------

        #region Old Code
        //public ActionResult MAS100_Search()
        //{
        //    try
        //    {


        //        IInstrumentMasterHandler hand = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
        //        List<doInstrumentExpansion> lst = hand.GetInstruMentExpansion(Request["PInstrumentCode"], MiscType.C_LINE_UP_TYPE, "0").ToList();
        //        string xml = CommonUtil.ConvertToXml<doInstrumentExpansion>(lst);
        //        return Json(xml);
        //    }
        //    catch (Exception ex)
        //    {

        //        //MessageModel msg = MessageUtil.GetMessage(ex);
        //        //return Json(msg);
        //        return null;
        //    }
        //}
        //public JsonResult MAS100_AddDetail()
        //{
        //    try
        //    {

        //        IInstrumentMasterHandler hand = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
        //        List<doChildInstrument> lst = hand.GetChildInstrument(Request["CInstrumentCode"], MiscType.C_LINE_UP_TYPE, "0").ToList();
        //        string xml = CommonUtil.ConvertToXml<doChildInstrument>(lst);
        //        return Json(lst);

        //    }
        //    catch (Exception ex)
        //    {

        //        //MessageModel msg = MessageUtil.GetMessage(ex);
        //        //return Json(msg);
        //        return null;
        //    }
        //}
        //public ActionResult MAS100_Delete()
        //{
        //    string Delete = Request["DeleteArray"];
        //    string[] DeleteArrey = Delete.Split(',');
        //    try
        //    {
        //        IInstrumentMasterHandler hand = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
        //        for (int i = 1; i < DeleteArrey.Length; i++)
        //        {
        //            int lst = hand.DeleteInstrumentExpansion(DeleteArrey[i], DeleteArrey[0]);
        //        }
        //        //MessageModel msg = MessageUtil.GetMessage(MessageUtil.Common_MessageList.MSG0046);
        //        //msg.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
        //        //return Json(msg);
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        //MessageModel msg = MessageUtil.GetMessage(ex);
        //        //return Json(msg);
        //        return null;
        //    }
        //}
        //public ActionResult MAS100_InsertChild(List<tbm_InstrumentExpansion> lst)
        //{
        //    try
        //    {
        //        IInstrumentMasterHandler hand = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
        //        DateTime dt = DateTime.Now;
        //        string user = "test";
        //        hand.DeleteAllInstrument(lst[0].InstrumentCode);
        //        foreach (tbm_InstrumentExpansion Ins in lst)
        //        {
        //            hand.InsertInstrumentExpansion(Ins.InstrumentCode, Ins.ChildInstrumentCode
        //            , dt, user, dt, user);
        //        }
        //        //MessageModel msg = MessageUtil.GetMessage(MessageUtil.Common_MessageList.MSG0046);
        //        //msg.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
        //        //return Json(msg);
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        //MessageModel msg = MessageUtil.GetMessage(ex);
        //        //return Json(msg);
        //        return null;
        //    }
        //}
        #endregion
    }
}

