//*********************************
// Create by: Narupon W. 
// Create date: 10/Oct/2011
// Update date: 10/Oct/2011
//*********************************



using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Xml;
using System.Transactions;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.ActionFilters;

using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Util;

using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Contract;
using CSI.WindsorHelper;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Presentation.Contract.Models;
using SECOM_AJIS.DataEntity.Billing;

using System.Diagnostics;

namespace SECOM_AJIS.Presentation.Contract.Controllers
{
    public partial class ContractController : BaseController
    {
        /// <summary>
        /// Check screen authority and permission
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CTS190_Authority(CTS190_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            //doContractInfoCondition param = new doContractInfoCondition();


            // Check permission
            if (!CheckUserPermission(ScreenID.C_SCREEN_ID_CP32, FunctionID.C_FUNC_ID_OPERATE))
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                return Json(res);
            }



            // Check system is syspend ?
            try
            {

                ICommonHandler handler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                bool isSuspend = handler.IsSystemSuspending();


                if (isSuspend)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

            }
            catch
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                return Json(res);
            }

            return InitialScreenEnvironment<CTS190_ScreenParameter>("CTS190", param, res);
        }
        /// <summary>
        /// Generate screen
        /// </summary>
        /// <returns></returns>
        [Initialize("CTS190")]
        public ActionResult CTS190()
        {
            return View();
        }
        /// <summary>
        /// Initial grid
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS190_IntialGridRecieveContractDoc()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Contract\\CTS190", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }
        /// <summary>
        /// Add received contract document
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CTS190_AddReceivedContractDoc(CTS190_ScreenParameter cond)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            CommonUtil cm = new CommonUtil();

            IMasterHandler handlerMaster = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
            List<dtDocumentTemplateByDocumentCode> listDocTemplate = new List<dtDocumentTemplateByDocumentCode>();

            string contracCode;
            string quotationTargetCode;

            try
            {
                ICommonContractHandler handler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
                List<dtContractDocHeader> list = new List<dtContractDocHeader>();

                contracCode = cm.ConvertContractCode(cond.ContractCode_QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                quotationTargetCode = cm.ConvertQuotationTargetCode(cond.ContractCode_QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG);


                // Check required field.
                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    if (res.IsError)
                        return Json(res);

                }

                // Business check 1
                if (cond.OCC_Alphabet == null)
                    cond.OCC_Alphabet = string.Empty;

                if (!(cond.OCC_Alphabet.ToUpper() == ParticularOCC.C_PARTICULAR_OCC_PO.ToUpper())) 
                      //|| cond.OCC_Alphabet.ToUpper() == ParticularOCC.C_PARTICULAR_OCC_START_CONFIRM_LETTER.ToUpper())) //Comment by Jutarat A. on 22042013
                {
                    //Then Check "Contract document OCC" field
                    // ---> Then show message at warning section (MSG3234: Please specify contract document occurrence)

                    if (CommonUtil.IsNullOrEmpty(cond.ContractDocOCC) == true)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3234, null, new string[] { "ContractDocOCC" });
                        return Json(res);
                    }

                }


                // Business check 2
                if (CommonUtil.IsNullOrEmpty(cond.ContractDocOCC) == true) // Not fill ContractDocOCC
                {
                    if (!(cond.OCC_Alphabet.ToUpper() == ParticularOCC.C_PARTICULAR_OCC_PO.ToUpper())) 
                          //|| cond.OCC_Alphabet.ToUpper() == ParticularOCC.C_PARTICULAR_OCC_START_CONFIRM_LETTER.ToUpper())) //Comment by Jutarat A. on 22042013
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                        return Json(res);
                    }

                    //#region Check billing basic data

                    //if (cond.OCC_Alphabet.ToUpper().CompareTo(ParticularOCC.C_PARTICULAR_OCC_MIN_BILLING_MEMO.ToUpper()) >= 0
                    //        && cond.OCC_Alphabet.ToUpper().CompareTo(ParticularOCC.C_PARTICULAR_OCC_MAX_BILLING_MEMO.ToUpper()) <= 0)
                    //{
                    //    string occ = cond.OCC_Alphabet.Trim();
                    //    if (occ.Length > 2)
                    //        occ = occ.Substring(occ.Length - 2);
                        
                    //    IBillingHandler bhandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                    //    doTbt_BillingBasic bs = bhandler.GetBillingBasicData(contracCode.Trim(), occ, null, null, null);
                    //    if (bs == null)
                    //    {
                    //        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3242);
                    //        return Json(res);
                    //    }
                    //}

                    //#endregion

                    list = handler.GetContractDocHeader(contracCode.Trim(), quotationTargetCode.Trim(), cond.OCC_Alphabet.Trim(), cond.ContractDocOCC);

                    if (list.Count == 0) // not found ! (ContractDoc Header)
                    {
                        //Comment by Jutarat A. on 22042013
                        //if (cond.OCC_Alphabet.ToUpper() == ParticularOCC.C_PARTICULAR_OCC_START_CONFIRM_LETTER.ToUpper())
                        //{
                        //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                        //    return Json(res);
                        //}
                        //End Comment

                        IRentralContractHandler handlerRental = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                        List<tbt_RentalContractBasic> rentalList = handlerRental.GetTbt_RentalContractBasic(contracCode, null);

                        if (rentalList.Count == 0)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                            return Json(res);
                        }
                        else
                        {
                            // Create dt !! 

                            dtContractDocHeader dt = new dtContractDocHeader();
                            dt.ContractCode = contracCode.ToUpper();
                            dt.OCC = cond.OCC_Alphabet.Trim().ToUpper();
                            if (cond.OCC_Alphabet.Trim().ToUpper() == ParticularOCC.C_PARTICULAR_OCC_PO.ToUpper())
                            {
                                dt.DocumentCode = DocumentCode.C_DOCUMENT_CODE_PO;
                            }
                            //else
                            //{
                            //    dt.DocumentCode = DocumentCode.C_DOCUMENT_CODE_CHANGE_PAYMENT_MEMO;
                            //}


                            listDocTemplate = handlerMaster.GetDocumentTemplateByDocumentCode(dt.DocumentCode);

                            if (listDocTemplate.Count > 0)
                            {
                                dt.DocumentNameEN = listDocTemplate[0].DocumentNameEN;
                                dt.DocumentNameJP = listDocTemplate[0].DocumentNameJP;
                                dt.DocumentNameLC = listDocTemplate[0].DocumentNameLC;
                            }

                            dt.ContractOfficeCode = rentalList[0].ContractOfficeCode;
                            dt.OperationOfficeCode = rentalList[0].OperationOfficeCode;
                            dt.DocStatus = ContractDocStatus.C_CONTRACT_DOC_STATUS_COLLECTED;
                            dt.DocAuditResult = cond.DocAuditResult;
                            dt.CollectDocDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                            // extra field
                            dt.ContractCode_QuotationTargetCode = contracCode.ToUpper();
                            dt.OCC_Alphabet = cond.OCC_Alphabet.Trim().ToUpper();


                            dt.IsCreateFlag = true;  //***
                            dt.IsContractFlag = true; //***

                            list.Clear();
                            list.Add(dt);
                        }


                    }
                    else  // Found !! (ContractDoc Header)
                    {
                        // Update dt !!

                        list[0].IsCreateFlag = false; // is updated
                        list[0].DocStatus = ContractDocStatus.C_CONTRACT_DOC_STATUS_COLLECTED;
                        list[0].DocAuditResult = cond.DocAuditResult;
                        list[0].CollectDocDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    }

                }
                else // ContractDocOCC is filled
                {
                    list = handler.GetContractDocHeader(contracCode.Trim(), quotationTargetCode.Trim(), cond.OCC_Alphabet.Trim(), cond.ContractDocOCC.Trim());

                    if (list.Count > 0)
                    {
                        // get Document templete for get "ReportFlag"
                        listDocTemplate = handlerMaster.GetDocumentTemplateByDocumentCode(list[0].DocumentCode);

                        if (listDocTemplate.Count == 0)
                        {
                            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0111, new string[] { "Document Template is not found." }, null);
                            return Json(res);
                        }

                        // Business check
                        if (CommonUtil.IsNullOrEmpty(list[0].ContractCode) == true)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3160);
                            return Json(res);
                        }
                        // Business check
                        if (listDocTemplate[0].ReportFlag == true)
                        {
                            if (list[0].DocStatus == ContractDocStatus.C_CONTRACT_DOC_STATUS_NOT_ISSUED || list[0].IssuedDate.HasValue == false)
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3179);
                                return Json(res);
                            }
                        }
                        else
                        {
                            if (list[0].DocStatus == ContractDocStatus.C_CONTRACT_DOC_STATUS_NOT_ISSUED)
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3179);
                                return Json(res);
                            }
                        }


                        // Update dt !!

                        list[0].IsCreateFlag = false; // is updated
                        list[0].DocStatus = ContractDocStatus.C_CONTRACT_DOC_STATUS_COLLECTED;
                        list[0].DocAuditResult = cond.DocAuditResult;
                        list[0].CollectDocDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;


                    }
                }


                if (list.Count > 0)
                {
                    // CheckDataAuthority
                    if (CTS190_CheckDataAuthority(list[0]) == false)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063);
                        return Json(res);
                    }
                }
                else
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                    return Json(res);
                }


                CommonUtil.MappingObjectLanguage<dtContractDocHeader>(list);
                MiscTypeMappingList miscList = new MiscTypeMappingList();
                miscList.AddMiscType(list.ToArray());

                ICommonHandler comHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                comHandler.MiscTypeMappingList(miscList);


                if (list.Count > 0)
                {
                    if (list[0].IsContractFlag.Value)
                    {
                        list[0].ContractCode_QuotationTargetCode = cm.ConvertContractCode(list[0].ContractCode_QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    }
                    else
                    {
                        list[0].ContractCode_QuotationTargetCode = cm.ConvertQuotationTargetCode(list[0].ContractCode_QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    }

                    string myContractCode = cm.ConvertContractCode(list[0].ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    string myOCC = list[0].OCC;
                    string myQuotationTargetCode = cm.ConvertQuotationTargetCode(list[0].QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    string myAlphabet = list[0].Alphabet;
                    
                    string myDocOCC = list[0].ContractDocOCC;
                    if (cond.OCC_Alphabet.ToUpper() == ParticularOCC.C_PARTICULAR_OCC_PO.ToUpper())
                        //|| cond.OCC_Alphabet.ToUpper() == ParticularOCC.C_PARTICULAR_OCC_START_CONFIRM_LETTER.ToUpper()) //Comment by Jutarat A. on 22042013
                        myDocOCC = "";

                    // Update my_checked_id_by_ct , my_checked_id_by_quo , my_checked_id
                    list[0].my_checked_id_by_ct = string.Format("{0}-{1}-{2}", myContractCode, myOCC, myDocOCC).ToUpper();
                    list[0].my_checked_id_by_quo = string.Format("{0}-{1}-{2}", myQuotationTargetCode, myAlphabet, myDocOCC).ToUpper();
                    list[0].my_checked_id = string.Format("{0}-{1}", list[0].my_checked_id_by_ct, list[0].my_checked_id_by_quo).ToUpper();
                    res.ResultData = list[0];

                }



                return Json(res);

            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }
        /// <summary>
        /// Save contract document receiving
        /// </summary>
        /// <param name="dtContractDocReceive"></param>
        /// <returns></returns>
        public ActionResult CTS190_SaveContractDocumentRecieving(List<dtContractDocHeader> dtContractDocReceive)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            CommonUtil cm = new CommonUtil();

            // Update CollectDocDate
            foreach (var item in dtContractDocReceive)
            {
                item.CollectDocDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            }

            try
            {

                // Check permission
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_CP32, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                // Check system is syspend ?
                ICommonHandler handler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                bool isSuspend = handler.IsSystemSuspending();

                if (isSuspend)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                if (dtContractDocReceive.Count == 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                    return Json(res);
                }

                int isComplete = 0;

                foreach (var item in dtContractDocReceive)
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        item.IsSaveComplete = false;

                        CTS190_Register(item);

                        item.IsSaveComplete = true;
                        scope.Complete();
                    }
                }


                // Check error result
                List<dtContractDocHeader> errorResult = (from p in dtContractDocReceive where p.IsSaveComplete = false select p).ToList<dtContractDocHeader>();
                // errorResult = list of incomplete (cannot save)

                List<dtContractDocHeader> okResult = (from p in dtContractDocReceive where p.IsSaveComplete = true select p).ToList<dtContractDocHeader>();
                // okResult = list of saved completely (can save)



                if (errorResult.Count > 0 && errorResult.Count == dtContractDocReceive.Count) // error all row
                {
                    isComplete = 0;
                }
                else if (errorResult.Count > 0 && errorResult.Count < dtContractDocReceive.Count) // error some row (some complete)
                {
                    isComplete = 1;

                    // Create string list of DocNo that cannot save
                    string[] strDocNo = new string[2];

                    // list of OK
                    for (int i = 0; i < okResult.Count; i++)
                    {
                        strDocNo[0] += okResult[i].DocNo;

                        if (i != okResult.Count - 1)
                        {
                            strDocNo[0] += ",";
                        }
                    }


                    // list of error
                    for (int i = 0; i < errorResult.Count; i++)
                    {
                        strDocNo[1] += errorResult[i].DocNo;

                        if (i != errorResult.Count - 1)
                        {
                            strDocNo[1] += ",";
                        }
                    }


                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3218, strDocNo);
                    res.ResultData = isComplete;
                    return Json(res);

                }
                else if (errorResult.Count == 0) // no error
                {
                    isComplete = 2;
                }

                res.ResultData = isComplete;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }
        /// <summary>
        /// Check data authority
        /// </summary>
        /// <param name="dtContractDocReceive"></param>
        /// <returns></returns>
        private bool CTS190_CheckDataAuthority(dtContractDocHeader dtContractDocReceive)
        {
            bool hasAuthority = false;

            // Old
            //List<OfficeDataDo> list = (from p in CommonUtil.dsTransData.dtOfficeData
            //                           where p.OfficeCode == dtContractDocReceive.ContractOfficeCode ||
            //                                 p.OfficeCode == dtContractDocReceive.OperationOfficeCode
            //                           select p).ToList<OfficeDataDo>();

            //hasAuthority = (list.Count > 0);

            IRentralContractHandler handlerRC = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            ISaleContractHandler handlerSale = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
            // get RCB
            List<tbt_RentalContractBasic> dtRCB = handlerRC.GetTbt_RentalContractBasic(dtContractDocReceive.ContractCode, null);
            // get Sale basic
            List<tbt_SaleBasic> dtSB = handlerSale.GetTbt_SaleBasic(dtContractDocReceive.ContractCode, null, true);


            List<OfficeDataDo> list = new List<OfficeDataDo>();

            if (dtRCB.Count > 0)
            {
                list = (from p in CommonUtil.dsTransData.dtOfficeData
                        where p.OfficeCode == dtRCB[0].ContractOfficeCode ||
                              p.OfficeCode == dtRCB[0].OperationOfficeCode
                        select p).ToList<OfficeDataDo>();

            }
            else if (dtSB.Count > 0)
            {
                list = (from p in CommonUtil.dsTransData.dtOfficeData
                        where p.OfficeCode == dtSB[0].ContractOfficeCode ||
                              p.OfficeCode == dtSB[0].OperationOfficeCode
                        select p).ToList<OfficeDataDo>();
            }

            hasAuthority = (list.Count > 0);

            return hasAuthority;
        }
        /// <summary>
        /// Register data
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private bool CTS190_Register(dtContractDocHeader dt)
        {
            List<tbt_ContractDocument> lsInsert = null; //Add by Jutarat A. on 12032013
            List<tbt_ContractDocument> dtUpdate = null; //Add by Jutarat A. on 12032013

            try
            {

                IContractDocumentHandler handlerCD = ServiceContainer.GetService<IContractDocumentHandler>() as IContractDocumentHandler;

                if (dt.IsCreateFlag)
                {
                    // **tt** Create dt tbt_ContractDocument and update some field follow DDS (in case create)
                    tbt_ContractDocument dtInsert = new tbt_ContractDocument();

                    dtInsert.ContractCode = dt.ContractCode;
                    dtInsert.OCC = dt.OCC;
                    dtInsert.DocumentCode = dt.DocumentCode;
                    dtInsert.ContractOfficeCode = dt.ContractOfficeCode;
                    dtInsert.OperationOfficeCode = dt.OperationOfficeCode;
                    dtInsert.DocStatus = dt.DocStatus;
                    dtInsert.DocAuditResult = dt.DocAuditResult;
                    dtInsert.CollectDocDate = dt.CollectDocDate;
                    dtInsert.IssuedDate = dt.IssuedDate;



                    // Generate new Doc OCC
                    string newDocOCC = "";
                    newDocOCC = handlerCD.GenerateDocOCC(dt.ContractCode, dt.OCC);

                    dtInsert.ContractDocOCC = newDocOCC;
                    dtInsert.DocNo = string.Format("{0}-{1}-{2}", dt.ContractCode, dt.OCC, newDocOCC);

                    // Keep back
                    dt.DocNo = string.Format("{0}-{1}-{2}", dt.ContractCode, dt.OCC, newDocOCC);

                    //List<tbt_ContractDocument> lsInsert = new List<tbt_ContractDocument>();
                    lsInsert = new List<tbt_ContractDocument>(); //Modify by Jutarat A. on 12032013
                    lsInsert.Add(dtInsert);

                    // Insert : InsertTbt_ContractDocument ** 
                    handlerCD.InsertTbt_ContractDocument(lsInsert);

                    // Call UpdateContractBasic
                    CTS190_UpdateContractBasicData(lsInsert);

                }
                else
                {
                    // **tt**  Get dt tbt_ContractDocument  from handler and update some field follow DDS (in case update)
                    //List<tbt_ContractDocument> dtUpdate = new List<tbt_ContractDocument>();
                    dtUpdate = new List<tbt_ContractDocument>(); //Modify by Jutarat A. on 12032013

                    if (dt.IsContractFlag.Value == true)
                    {
                        dtUpdate = handlerCD.GetContractDocHeaderByContractCode(dt.ContractCode, dt.OCC, dt.ContractDocOCC);
                    }
                    else
                    {
                        dtUpdate = handlerCD.GetContractDocHeaderByQuotationCode(dt.QuotationTargetCode, dt.Alphabet, dt.ContractDocOCC);
                    }

                    if (dtUpdate.Count == 0)
                    {
                        return false;
                    }


                    for (int i = 0; i < dtUpdate.Count; i++)
                    {
                        dtUpdate[i].DocStatus = dt.DocStatus;
                        dtUpdate[i].DocAuditResult = dt.DocAuditResult;
                        dtUpdate[i].CollectDocDate = dt.CollectDocDate;

                    }

                    if (dtUpdate.Count > 1)
                    {
                        dtUpdate.RemoveRange(1, dtUpdate.Count - 1);
                    }

                    // Keep back
                    dt.DocNo = dtUpdate[0].DocNo;



                    // Set Not user status
                    int isUpdated = handlerCD.SetNotUsedStatus(dtUpdate[0].ContractCode, dtUpdate[0].OCC, ContractDocStatus.C_CONTRACT_DOC_STATUS_NOT_USED, ContractDocStatus.C_CONTRACT_DOC_STATUS_COLLECTED, false, null);

                    // Update : UpdateTbt_ContractDocument **
                    List<tbt_ContractDocument> tt_Updated = handlerCD.UpdateTbt_ContractDocument(dtUpdate);

                    // Call UpdateContractBasic
                    CTS190_UpdateContractBasicData(dtUpdate);

                    // Update RentalSecurityBasic
                    CTS190_UpdateRentalSecurityBasic(dtUpdate);

                    // Update SalesBasic
                    CTS190_UpdateSaleBasic(dtUpdate);

                    // Update about PO (Recursive)
                    if (dt.DocumentCode == DocumentCode.C_DOCUMENT_CODE_CONTRACT_EN || dt.DocumentCode == DocumentCode.C_DOCUMENT_CODE_CONTRACT_TH)
                    {
                        List<tbt_ContractDocument> dtContractDoc = handlerCD.GetContractDocHeaderByContractCode(dt.ContractCode, ParticularOCC.C_PARTICULAR_OCC_PO, null);

                        if (dtContractDoc.Count == 0)
                        {
                            dtContractDocHeader dtPO = new dtContractDocHeader();
                            dtPO.IsCreateFlag = true;
                            dtPO.IsContractFlag = false;

                            dtPO.ContractCode = dtUpdate[0].ContractCode;
                            dtPO.OCC = ParticularOCC.C_PARTICULAR_OCC_PO;
                            dtPO.DocumentCode = DocumentCode.C_DOCUMENT_CODE_PO;
                            dtPO.ContractOfficeCode = dtUpdate[0].ContractOfficeCode;
                            dtPO.OperationOfficeCode = dtUpdate[0].OperationOfficeCode;
                            dtPO.IssuedDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            dtPO.DocStatus = ContractDocStatus.C_CONTRACT_DOC_STATUS_ISSUED;
                            dtPO.DocAuditResult = DocAuditResult.C_DOC_AUDIT_RESULT_NO_NEED_TO_RECEIVE;

                            // Recursive
                            CTS190_Register(dtPO);

                        }

                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                //Add by Jutarat A. on 12032013
                string source = "SIMS web application";
                string logName = "(CTS190)Application";
                string logDetail = ex.ToString();

                if (!EventLog.SourceExists(source))
                {
                    EventLog.CreateEventSource(source, logName);
                }

                if (lsInsert != null && lsInsert.Count > 0)
                {
                    logDetail = String.Format("{0} : {1}", ex.ToString(), CommonUtil.ConvertToXml_Store<tbt_ContractDocument>(lsInsert));
                }
                else if (dtUpdate != null && dtUpdate.Count > 0)
                {
                    logDetail = String.Format("{0} : {1}", ex.ToString(), CommonUtil.ConvertToXml_Store<tbt_ContractDocument>(dtUpdate));
                }

                EventLog objLog = new EventLog();
                objLog.Source = source;
                objLog.Log = logName;
                objLog.WriteEntry(logDetail, EventLogEntryType.Error);
                //End Add

                throw ex;
            }

        }
        /// <summary>
        /// Update contract basic data
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private bool CTS190_UpdateContractBasicData(List<tbt_ContractDocument> dt)
        {
            try
            {
                if (dt == null || dt.Count == 0)
                {
                    return false;
                }

                IRentralContractHandler handlerRC = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;

                // Get RCB
                List<tbt_RentalContractBasic> dtRCB = handlerRC.GetTbt_RentalContractBasic(dt[0].ContractCode, null);

                if (dtRCB.Count == 0)
                {
                    return false;
                }

                if (dt[0].DocumentCode == DocumentCode.C_DOCUMENT_CODE_PO)
                {
                    dtRCB[0].PODocAuditResult = dt[0].DocAuditResult;
                    dtRCB[0].PODocReceiveDate = dt[0].CollectDocDate;
                    
                }
                else if ((dt[0].DocumentCode == DocumentCode.C_DOCUMENT_CODE_CONTRACT_EN || dt[0].DocumentCode == DocumentCode.C_DOCUMENT_CODE_CONTRACT_TH)
                    && dt[0].OCC == ParticularOCC.C_PARTICULAR_OCC_CONTRACT_REP_BEF_START)
                {
                    dtRCB[0].ContractDocAuditResult = dt[0].DocAuditResult;
                    dtRCB[0].ContractDocReceiveDate = dt[0].CollectDocDate;
                }
                else if (dt[0].DocumentCode == DocumentCode.C_DOCUMENT_CODE_START_OPER_CONFIRM_LETTER)
                {
                    dtRCB[0].StartMemoAuditResult = dt[0].DocAuditResult;
                    dtRCB[0].StartMemoReceiveDate = dt[0].CollectDocDate;
                }

                dtRCB[0].AuditCollectionProcessDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                handlerRC.UpdateTbt_RentalContractBasicCore(dtRCB[0]);

                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// Update rental secuirty basic
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private bool CTS190_UpdateRentalSecurityBasic(List<tbt_ContractDocument> dt)
        {
            try
            {
                if (dt == null || dt.Count == 0)
                {
                    return false;
                }

                IRentralContractHandler handlerRC = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;

                // Get RSB
                List<tbt_RentalSecurityBasic> dtRSB = handlerRC.GetTbt_RentalSecurityBasic(dt[0].ContractCode, dt[0].OCC);

                if (dtRSB.Count == 0)
                {
                    return false;
                }

                if (dt[0].DocAuditResult == DocAuditResult.C_DOC_AUDIT_RESULT_OTHER)
                {
                    dtRSB[0].DocAuditResult = null;
                    dtRSB[0].DocumentCode = null;

                }
                else
                {
                    dtRSB[0].DocAuditResult = dt[0].DocAuditResult;
                    dtRSB[0].DocumentCode = dt[0].DocumentCode;

                }


                handlerRC.UpdateTbt_RentalSecurityBasic(dtRSB[0]);

                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// Update sale basic
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private bool CTS190_UpdateSaleBasic(List<tbt_ContractDocument> dt)
        {
            try
            {
                if (dt == null || dt.Count == 0)
                {
                    return false;
                }

                ISaleContractHandler handlerRC = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;

                // Get SaleBasic (SB)
                List<tbt_SaleBasic> dtSB = handlerRC.GetTbt_SaleBasic(dt[0].ContractCode, dt[0].OCC, null);

                if (dtSB.Count == 0)
                {
                    return false;
                }

                dtSB[0].DocReceiveDate = dt[0].CollectDocDate;
                dtSB[0].DocAuditResult = dt[0].DocAuditResult;
                dtSB[0].DocumentCode = dt[0].DocumentCode;


                handlerRC.UpdateTbt_SaleBasic(dtSB[0]);


                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }


}
