//*********************************
// Create by: Jutarat A.
// Create date: 16/Nov/2011
// Update date: 16/Nov/2011
//*********************************

using System;
using System.Web;
using System.Web.Mvc;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.DataEntity.Contract;
using System.Data.Objects;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Presentation.Contract.Models;
using SECOM_AJIS.DataEntity.Quotation;
using System.Transactions;
using SECOM_AJIS.DataEntity.Master;

using System.Reflection;
using System.Data;

namespace SECOM_AJIS.Presentation.Contract.Controllers
{
    public partial class ContractController : BaseController
    {
        #region Authority

        /// <summary>
        /// Check system suspending and user’s permission
        /// </summary>
        /// <param name="sParam"></param>
        /// <returns></returns>
        public ActionResult CTS210_Authority(CTS210_ScreenParameter sParam)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                //CheckSystemStatus
                if (CheckIsSuspending(res) == true)
                    return Json(res);

                //Check screen permission
                if (CheckUserPermission(ScreenID.C_SCREEN_ID_ISSUE_CONTRACT_DOCUMENT, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                sParam = new CTS210_ScreenParameter();
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }

            return InitialScreenEnvironment<CTS210_ScreenParameter>("CTS210", sParam, res);
        }

        #endregion

        #region Action

        /// <summary>
        /// Initial screen
        /// </summary>
        /// <returns></returns>
        [Initialize("CTS210")]
        public ActionResult CTS210()
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                return View();
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Initial DocumentList grid
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS210_InitialGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Contract\\CTS210", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Get data to DocumentList grid
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CTS210_GetDocumentListData(CTS210_SpecifyContractCodeCondition cond)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            IContractDocumentHandler contDocHandler;
            dsContractDocForIssue dsContractDoc = new dsContractDocForIssue();
            List<dtContractDoc> gridDataList = new List<dtContractDoc>();
            CTS210_ValidateContractCondition validateContractCondition;

            try
            {
                //Validate required field
                validateContractCondition = CommonUtil.CloneObject<CTS210_SpecifyContractCodeCondition, CTS210_ValidateContractCondition>(cond);
                ValidatorUtil.BuildErrorMessage(res, new object[] { validateContractCondition }, null, false);
                if (res.IsError)
                    return Json(res);

                //Get contract document data
                CommonUtil comUtil = new CommonUtil();
                string strContractCode = comUtil.ConvertContractCode(cond.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                string strQuotationTargetCode = comUtil.ConvertQuotationTargetCode(cond.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                contDocHandler = ServiceContainer.GetService<IContractDocumentHandler>() as IContractDocumentHandler;
                dsContractDoc = contDocHandler.GetContractDocForIssue(strContractCode, strQuotationTargetCode, cond.OCC);

                //Check existing of returned data
                if (dsContractDoc != null && dsContractDoc.dtContractDocList != null && dsContractDoc.dtContractDocList.Count == 0)
                {
                    //res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);

                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;

                    //Check existing contract document
                    if (contDocHandler.IsContractDocExist(strContractCode, strQuotationTargetCode, null, null))
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { cond.OCC }, new string[] { "txtOccAlphabet" });
                    else
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { cond.ContractCode }, new string[] { "txtContractQuotTgtCode" });

                    return Json(res);
                }
                else
                {
                    List<dtContractDoc> contactDocTemp = (from t in dsContractDoc.dtContractDocList
                                                          orderby t.CreateDate
                                                          select t).ToList<dtContractDoc>();

                    dsContractDoc.dtContractDocList = contactDocTemp;
                }

                List<CTS210_DocumentListGridData> resultGridData = new List<CTS210_DocumentListGridData>();
                if (dsContractDoc.dtContractDocList != null && dsContractDoc.dtContractDocList.Count > 0)
                {
                    resultGridData = CommonUtil.ClonsObjectList<dtContractDoc, CTS210_DocumentListGridData>(dsContractDoc.dtContractDocList);
                    SetEnableDownload_CTS210(resultGridData);
                }

                CTS210_ScreenParameter sParam = GetScreenObject<CTS210_ScreenParameter>();
                if (sParam.dsContractDoc == null)
                    sParam.dsContractDoc = new dsContractDocForIssue();

                sParam.dsContractDoc = dsContractDoc;
                UpdateScreenObject(sParam);

                res.ResultData = CommonUtil.ConvertToXml<CTS210_DocumentListGridData>(resultGridData, "Contract\\CTS210", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get data of Contract Information
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS210_GetContractInfo()
        {
            ObjectResultData res = new ObjectResultData();
            List<dtRentalContractBasicForView> dtRentalContractBasicForViewList;
            List<dtSaleContractBasicForView> dtSaleContractBasicForViewList;

            try
            {
                CTS210_ScreenParameter sParam = GetScreenObject<CTS210_ScreenParameter>();
                if (sParam.dsContractDoc != null)
                {
                    dtRentalContractBasicForViewList = sParam.dsContractDoc.dtRentalContractBasicForViewList;
                    dtSaleContractBasicForViewList = sParam.dsContractDoc.dtSaleContractBasicForViewList;

                    dtRentalContractBasicForView dtContractInfo = new dtRentalContractBasicForView();
                    if (dtRentalContractBasicForViewList != null && dtRentalContractBasicForViewList.Count > 0)
                    {
                        dtContractInfo.ContractTargetNameEN = dtRentalContractBasicForViewList[0].ContractTargetNameEN;
                        dtContractInfo.ContractTargetNameLC = dtRentalContractBasicForViewList[0].ContractTargetNameLC;
                        dtContractInfo.SiteNameEN = dtRentalContractBasicForViewList[0].SiteNameEN;
                        dtContractInfo.SiteNameLC = dtRentalContractBasicForViewList[0].SiteNameLC;

                        res.ResultData = dtContractInfo;
                    }
                    else if (dtSaleContractBasicForViewList != null && dtSaleContractBasicForViewList.Count > 0)
                    {
                        dtContractInfo.ContractTargetNameEN = dtSaleContractBasicForViewList[0].ProductNameEN;
                        dtContractInfo.ContractTargetNameLC = dtSaleContractBasicForViewList[0].ProductNameLC;
                        dtContractInfo.SiteNameEN = dtSaleContractBasicForViewList[0].SiteNameEN;
                        dtContractInfo.SiteNameLC = dtSaleContractBasicForViewList[0].SiteNameLC;

                        res.ResultData = dtContractInfo;
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
        /// Generate contract document when click [Download] button on Document result list at ‘Document list/Specify document’ section
        /// </summary>
        /// <param name="iDocID"></param>
        /// <returns></returns>
        public ActionResult CTS210_DownloadDocumentReport(int iDocID)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                //Check suspending
                if (CheckIsSuspending(res) == true)
                    return Json(res);

                //Check screen permission
                if (CheckUserPermission(ScreenID.C_SCREEN_ID_ISSUE_CONTRACT_DOCUMENT, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }


                CTS210_ScreenParameter sParam = GetScreenObject<CTS210_ScreenParameter>();
                if (sParam.dsContractDoc != null)
                {
                    if (sParam.dsContractDoc.dtContractDocList != null)
                    {
                        foreach (dtContractDoc data in sParam.dsContractDoc.dtContractDocList)
                        {
                            if (data.DocID == iDocID)
                            {
                                if (data.IssuedDate == null)
                                {
                                    sParam.StreamReport = DownloadDocument_CTS210(data.DocID, data.DocNo, data.DocumentCode, data.UpdateDate);
                                    if (sParam.StreamReport != null)
                                        res.ResultData = true;
                                }
                                else
                                {
                                    sParam.StreamReport = ReDownloadDocument_CTS210(data.DocNo, data.DocumentCode, data.DocumentType); //Modify (Add strDocumentType) by Jutarat A. on 11112013
                                    if (sParam.StreamReport != null)
                                        res.ResultData = false;
                                }
                            }
                        }

                        UpdateScreenObject(sParam);
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
        /// Reload data to DocumentList grid
        /// </summary>
        /// <param name="iDocID"></param>
        /// <returns></returns>
        public ActionResult CTS210_RefreshDocumentReport(int iDocID)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                CTS210_ScreenParameter sParam = GetScreenObject<CTS210_ScreenParameter>();
                if (sParam.dsContractDoc != null)
                {
                    if (sParam.dsContractDoc.dtContractDocList != null)
                    {
                        foreach (dtContractDoc data in sParam.dsContractDoc.dtContractDocList)
                        {
                            if (data.DocID == iDocID)
                            {
                                if (data.IssuedDate == null)
                                {
                                    data.IssuedDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                    data.DocStatus = ContractDocStatus.C_CONTRACT_DOC_STATUS_ISSUED;
                                }
                            }
                        }

                        UpdateScreenObject(sParam);

                        List<CTS210_DocumentListGridData> resultGridData = CommonUtil.ClonsObjectList<dtContractDoc, CTS210_DocumentListGridData>(sParam.dsContractDoc.dtContractDocList);
                        SetEnableDownload_CTS210(resultGridData);

                        res.ResultData = CommonUtil.ConvertToXml<CTS210_DocumentListGridData>(resultGridData, "Contract\\CTS210", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
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
        /// Open PDF file of contract document
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public ActionResult CTS210_OpenContractDocument(string k)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                CTS210_ScreenParameter sParam = GetScreenObject<CTS210_ScreenParameter>(k);
                if (sParam.StreamReport != null)
                {
                    return File(sParam.StreamReport, "application/pdf");
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        #endregion

        #region Method

        /// <summary>
        /// Set enable/disable [Download] button on Document result list at ‘Document list/Specify document’ section
        /// </summary>
        /// <param name="resultGridData"></param>
        private void SetEnableDownload_CTS210(List<CTS210_DocumentListGridData> resultGridData)
        {
            bool bIsEnable;
            foreach (CTS210_DocumentListGridData data in resultGridData)
            {
                bIsEnable = true;

                if (data.ReportFlag == null || data.ReportFlag == FlagType.C_FLAG_OFF)
                {
                    bIsEnable = false;
                }

                if (CommonUtil.dsTransData.ContainsPermission(data.DocumentCode, FunctionID.C_FUNC_ID_DOWNLOAD) == false)
                {
                    bIsEnable = false;
                }

                data.IsEnableDownload = bIsEnable;
            }
        }

        /// <summary>
        /// Download document process
        /// </summary>
        /// <param name="iDocID"></param>
        /// <param name="strDocNo"></param>
        /// <param name="strDocumentCode"></param>
        /// <param name="dLastUpdateDate"></param>
        /// <returns></returns>
        private Stream DownloadDocument_CTS210(int iDocID, string strDocNo, string strDocumentCode, DateTime? dLastUpdateDate)
        {
            Stream stream = null;
            IContractDocumentHandler contDocHandler;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    //Call method for creating report
                    contDocHandler = ServiceContainer.GetService<IContractDocumentHandler>() as IContractDocumentHandler;
                    stream = contDocHandler.CreateContractReport(iDocID, strDocNo, strDocumentCode);

                    //Update document status
                    List<tbt_ContractDocument> contDocList = contDocHandler.UpdateDocumentStatus(ContractDocStatus.C_CONTRACT_DOC_STATUS_ISSUED, iDocID, dLastUpdateDate);

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return stream;
        }

        /// <summary>
        /// Redownload document process
        /// </summary>
        /// <param name="strDocNo"></param>
        /// <param name="strDocumentCode"></param>
        /// <returns></returns>
        private Stream ReDownloadDocument_CTS210(string strDocNo, string strDocumentCode, string strDocumentType) //Modify (Add strDocumentType) by Jutarat A. on 11112013
        {
            Stream stream = null;
            //ICommonHandler commonHandler; //Comment by Jutarat A. on 11112013 (No use)
            IDocumentHandler docHandler;

            try
            {

                //Modify by Jutarat A. on 11112013
                //Call method for getting report
                /*commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<tbt_DocumentList> documentList = commonHandler.GetTbt_DocumentList(strDocNo, strDocumentCode);
                documentList = (from t in documentList
                                orderby t.DocumentOCC descending
                                select t).ToList<tbt_DocumentList>();

                if (documentList != null & documentList.Count > 0)
                {
                    docHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
                    stream = docHandler.GetDocumentReportFileStream(documentList[0].FilePath);
                }*/
                doDocumentDataCondition cond = new doDocumentDataCondition();
                cond.DocumentNo = strDocNo;
                cond.DocumentCode = strDocumentCode;
                cond.DocumentType = strDocumentType;

                docHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
                List<dtDocumentData> documentList = docHandler.GetDocumentDataList(cond, false);
                if (documentList != null & documentList.Count > 0)
                {
                    stream = docHandler.GetDocumentReportFileStream(documentList[0].FilePath);
                }
                //End Modify
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return stream;
        }

        #endregion
    }

}
