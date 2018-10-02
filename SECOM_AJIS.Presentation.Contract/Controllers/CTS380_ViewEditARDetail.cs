//*********************************
// Create by: Natthavat S.
// Create date: 13/DEC/2011
// Update date: 13/DEC/2011
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
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Models;
using CSI.WindsorHelper;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Presentation.Contract.Models;
using SECOM_AJIS.DataEntity.Quotation;
using System.Transactions;
using SECOM_AJIS.DataEntity.Master;
namespace SECOM_AJIS.Presentation.Contract.Controllers
{
    public partial class ContractController : BaseController
    {
        #region Authority

        /// <summary>
        /// Check user permission
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CTS380_Authority(CTS380_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            IARHandler arhandler = ServiceContainer.GetService<IARHandler>() as IARHandler;
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            try
            {
                res = ValidateAuthority_CTS380(res, param.pRequestNo, param);
                if (res.IsError)
                {
                    return Json(res);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<CTS380_ScreenParameter>("CTS380", param, res);
        }

        //public ActionResult CTS380_Authority(string param, string pRequestNo)
        //{
        //    ObjectResultData res = new ObjectResultData();
        //    IARHandler arhandler = ServiceContainer.GetService<IARHandler>() as IARHandler;
        //    ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
        //    CTS380_ScreenParameter sParam = new CTS380_ScreenParameter();

        //    try
        //    {
        //        //Dummie
        //        //pRequestNo = "202011A02021";

        //        if (CheckIsSuspending(res))
        //        {
        //            res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0049, null, null);
        //            return Json(res);
        //        }

        //        if (!CheckUserPermission(ScreenID.C_SCREEN_ID_VIEW_EDIT_AR))
        //        {
        //            res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0053, null, null);
        //            return Json(res);
        //        }

        //        if (string.IsNullOrEmpty(pRequestNo))
        //        {
        //            // Not Valid
        //            res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0011, new string[] { "Request No." }, null);
        //            return Json(res);
        //        }

        //        var arDat = arhandler.GetARDetail(pRequestNo);
        //        if ((arDat == null) || (arDat.dtAR == null))
        //        {
        //            // Not Found
        //            res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0011, new string[] { String.Format("Request No.: {0}", pRequestNo) }, null);
        //            return Json(res);
        //        }

        //        var arPermit = arhandler.HasARPermission(pRequestNo);
        //        if (!arPermit.ViewARDetailFlag)
        //        {
        //            res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0053, null, null);
        //            return Json(res);
        //        }

        //        sParam = new CTS380_ScreenParameter();
        //        sParam.RequestNo = pRequestNo;
        //    }
        //    catch (Exception ex)
        //    {
        //        res.AddErrorMessage(ex);
        //        return Json(res);
        //    }

        //    return InitialScreenEnvironment("CTS380", sParam);
        //}
        #endregion

        #region Action
        /// <summary>
        /// Initial screen
        /// </summary>
        /// <returns></returns>
        [Initialize("CTS380")]
        public ActionResult CTS380()
        {
            CTS380_ScreenParameter sParam = GetScreenObject<CTS380_ScreenParameter>();
            ViewBag.useKey = sParam.pRequestNo;
            ViewBag.AttachKey = GetCurrentKey();

            sParam.newAttachLst = new Dictionary<int, string>();
            sParam.delAttachLst = new Dictionary<int, string>();
            return View();
        }

        /// <summary>
        /// Initial person in charge grid schema
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS380_InitialAssignPersonInChargeGrid()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                List<CTS380_PICDat> dat = new List<CTS380_PICDat>();
                res.ResultData = CommonUtil.ConvertToXml<CTS380_PICDat>(dat, "Contract\\CTS380_AssignPIC", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Initial attach document grid schema
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS380_IntialGridAttachedDocList()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Contract\\CTS380_AttachedDocList", CommonUtil.GRID_EMPTY_TYPE.VIEW));
        }

        /// <summary>
        /// Retrieve attach document grid data
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS380_LoadGridAttachedDocList()
        {
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            ObjectResultData res = new ObjectResultData();

            try
            {
                CTS380_ScreenParameter sParam = GetScreenObject<CTS380_ScreenParameter>();

                List<dtAttachFileForGridView> lstAttachedName = new List<dtAttachFileForGridView>();

                if (sParam != null && sParam.pRequestNo != null) //Add by Jutarat A. on 07022013
                    lstAttachedName = commonhandler.GetAttachFileForGridView(sParam.pRequestNo);

                //if (Session["CTS330_AttachFile"] == null)
                //{
                //    lstAttachedName = commonhandler.GetAttachFileName(GetCurrentKey(), null, false);
                //    Session.Add("CTS330_AttachFile", lstAttachedName);
                //}
                //else
                //{
                //    lstAttachedName = (List<dtAttachFileNameID>)Session["CTS330_AttachFile"];
                //}

                res.ResultData = CommonUtil.ConvertToXml<dtAttachFileForGridView>(lstAttachedName, "Contract\\CTS380_AttachedDocList", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Validate and attach new document 
        /// </summary>
        /// <param name="fileSelect"></param>
        /// <param name="DocumentName"></param>
        /// <param name="sParam"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public ActionResult CTS380_AttachFile(HttpPostedFileBase fileSelect, string DocumentName, string sParam, string k)
        {
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            ObjectResultData res = new ObjectResultData();

            try
            {
                CTS380_ScreenParameter param = GetScreenObject<CTS380_ScreenParameter>(k);

                if (fileSelect == null)
                {
                    // File not select
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0050, null);
                }

                if (String.IsNullOrEmpty(DocumentName))
                {
                    // DocName is not input
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { CommonUtil.GetLabelFromResource("Contract", "CTS380", "lblDocumentName") });
                }

                byte[] fileData;

                using (BinaryReader reader = new BinaryReader(fileSelect.InputStream))
                {
                    var fList = commonhandler.GetAttachFileForGridView(sParam);

                    var filterDupItem = from a in fList where a.FileName.ToUpper().Equals(DocumentName.ToUpper() + Path.GetExtension(fileSelect.FileName).ToUpper()) select a;

                    if (filterDupItem.Count() > 0)
                    {
                        // Docname duplicate
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0115, new string[] { DocumentName });
                        //outmsg = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0115, new string[] { DocumentName });
                    }
                    else
                    {
                        fileData = reader.ReadBytes(fileSelect.ContentLength);

                        if (commonhandler.CanAttachFile(DocumentName, fileData.Length, Path.GetExtension(fileSelect.FileName), sParam, k))
                        {
                            DateTime currDate = DateTime.Now;
                            var attachedItem = commonhandler.InsertAttachFile(sParam
                            , DocumentName
                            , Path.GetExtension(fileSelect.FileName)
                            , fileData.Length
                            , fileData
                            , false);

                            param.newAttachLst.Add(attachedItem[0].AttachFileID, DocumentName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            if (res.IsError)
            {
                ViewBag.Message = res.Message.Message;
                ViewBag.MsgCode = res.Message.Code;
            }

            ViewBag.sKey = sParam;
            ViewBag.K = k;

            return View("CTS380_Upload");
        }

        /* Old Code
        /// <summary>
        /// Download exist attach document
        /// </summary>
        /// <param name="AttachID"></param>
        /// <returns></returns>
        public ActionResult CTS380_DownloadAttach(string AttachID)
        {
            ObjectResultData res = new ObjectResultData();
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            try
            {
                CTS380_ScreenParameter sParam = GetScreenObject<CTS380_ScreenParameter>();
                var downloadFileStream = commonhandler.GetAttachFileForDownload(int.Parse(AttachID), GetCurrentKey());
                var downloadFileName = commonhandler.GetTbt_AttachFile(sParam.pRequestNo, int.Parse(AttachID), null);
                //var downloadFileName = commonhandler.GetAttachFileName(sParam.strIncidentID, int.Parse(AttachID), null);
                //var attachFile = commonhandler.GetAttachFile(AttachmentModule.Incident, ReleateID, int.Parse(AttachID));
                //var fileNameLst = commonhandler.GetAttachFileName(ReleateID, int.Parse(AttachID), true);

                string fileName = downloadFileName[0].FileName;
                return File(downloadFileStream, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }*/

        //Add by Jutarat A. on 30012013
        ///// <summary>
        /// Download exist attach document
        /// </summary>
        /// <param name="AttachID"></param>
        public void CTS380_DownloadAttach(string AttachID)
        {
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            CTS380_ScreenParameter sParam = GetScreenObject<CTS380_ScreenParameter>();

            Stream downloadFileStream = commonhandler.GetAttachFileForDownload(int.Parse(AttachID), GetCurrentKey());
            List<tbt_AttachFile> downloadFileName = commonhandler.GetTbt_AttachFile(sParam.pRequestNo, int.Parse(AttachID), null);

            this.DownloadAllFile(downloadFileName[0].FileName, downloadFileStream);
        }
        //End Add

        /// <summary>
        /// Remove attach document
        /// </summary>
        /// <param name="AttachID"></param>
        /// <returns></returns>
        public ActionResult CTS380_RemoveAttach(string AttachID)
        {
            ObjectResultData res = new ObjectResultData();
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            try
            {
                CTS380_ScreenParameter sParam = GetScreenObject<CTS380_ScreenParameter>();
                int _attachID = int.Parse(AttachID);

                var lstAttachedName = commonhandler.GetAttachFileForGridView(sParam.pRequestNo);
                var targFile = from a in lstAttachedName where a.AttachFileID == _attachID select a;

                commonhandler.DeleteAttachFileByID(_attachID, sParam.pRequestNo);
                sParam.delAttachLst.Add(_attachID, targFile.First().FileName);

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Clear all attach document
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS380_ClearAttach()
        {
            ObjectResultData res = new ObjectResultData();
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            try
            {
                CTS380_ScreenParameter sParam = GetScreenObject<CTS380_ScreenParameter>();
                commonhandler.ClearTemporaryUploadFile(sParam.pRequestNo);
                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Initial attach document section
        /// </summary>
        /// <param name="sK"></param>
        /// <returns></returns>
        public ActionResult CTS380_Upload(string sK = "")
        {
            ViewBag.sKey = sK;
            ViewBag.K = GetCurrentKey();
            return View("CTS380_Upload");
        }

        /// <summary>
        /// Retrieve interaction type item for combobox
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS380_RetrieveInteractionCBB()
        {
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            IARHandler arhandler = ServiceContainer.GetService<IARHandler>() as IARHandler;
            ObjectResultData res = new ObjectResultData();
            CTS380_ScreenParameter sParam = GetScreenObject<CTS380_ScreenParameter>();

            try
            {
                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>();
                var arDat = arhandler.GetARDetail(sParam.pRequestNo);
                bool bEdit = false, bSpecialViewEdit = false;

                if ((arDat.dtAR != null) && (arDat.dtARRole != null) && (arDat.dtARRole.Count > 0))
                {
                    hasPermission_CTS380(sParam.pRequestNo, arDat.dtAR.ARStatus, ref res, out bSpecialViewEdit, out bEdit);
                    //res = new ObjectResultData();
                    if (!res.IsError)
                    {
                        var arRole = (arDat.dtARRole.Where(x => x.EmpNo == CommonUtil.dsTransData.dtUserData.EmpNo)).ToList();
                        if ((arDat.dtAR.ARStatus == ARStatus.C_AR_STATUS_RETURNED_REQUEST) && (arRole.Count > 0) && (arRole[0].ARRoleType == ARRole.C_AR_ROLE_REQUESTER))
                        {
                            miscs.Add(new doMiscTypeCode()
                            {
                                FieldName = MiscType.C_AR_INTERACTION_TYPE,
                                ValueCode = ARInteractionType.C_AR_INTERACTION_TYPE_APPROVAL_REQUEST
                            });

                            //Add by Jutarat A. on 05092013
                            miscs.Add(new doMiscTypeCode()
                            {
                                FieldName = MiscType.C_AR_INTERACTION_TYPE,
                                ValueCode = ARInteractionType.C_AR_INTERACTION_TYPE_AUDIT_REQUEST
                            });
                            //End Add
                        }
                        else if ((arDat.dtAR.ARStatus == ARStatus.C_AR_STATUS_WAIT_FOR_APPROVAL) && (arRole.Count > 0) && (arRole[0].ARRoleType == ARRole.C_AR_ROLE_APPROVER))
                        {
                            //Add by Jutarat A. on 29102012
                            miscs.Add(new doMiscTypeCode()
                            {
                                FieldName = MiscType.C_AR_INTERACTION_TYPE,
                                ValueCode = ARInteractionType.C_AR_INTERACTION_TYPE_APPROVAL_REQUEST
                            });
                            //End Add

                            miscs.Add(new doMiscTypeCode()
                            {
                                FieldName = MiscType.C_AR_INTERACTION_TYPE,
                                ValueCode = ARInteractionType.C_AR_INTERACTION_TYPE_AUDIT_REQUEST
                            });
                            miscs.Add(new doMiscTypeCode()
                            {
                                FieldName = MiscType.C_AR_INTERACTION_TYPE,
                                ValueCode = ARInteractionType.C_AR_INTERACTION_TYPE_RETURN_REQUEST
                            });
                            miscs.Add(new doMiscTypeCode()
                            {
                                FieldName = MiscType.C_AR_INTERACTION_TYPE,
                                ValueCode = ARInteractionType.C_AR_INTERACTION_TYPE_APPROVE
                            });
                            miscs.Add(new doMiscTypeCode()
                            {
                                FieldName = MiscType.C_AR_INTERACTION_TYPE,
                                ValueCode = ARInteractionType.C_AR_INTERACTION_TYPE_REJECT
                            });
                            miscs.Add(new doMiscTypeCode()
                            {
                                FieldName = MiscType.C_AR_INTERACTION_TYPE,
                                ValueCode = ARInteractionType.C_AR_INTERACTION_TYPE_INSTRUCTION
                            });
                        }
                        else if ((arDat.dtAR.ARStatus == ARStatus.C_AR_STATUS_AUDITING) && (arRole.Count > 0) && (arRole[0].ARRoleType == ARRole.C_AR_ROLE_AUDITOR))
                        {
                            miscs.Add(new doMiscTypeCode()
                            {
                                FieldName = MiscType.C_AR_INTERACTION_TYPE,
                                ValueCode = ARInteractionType.C_AR_INTERACTION_TYPE_AUDIT_REQUEST
                            });
                            miscs.Add(new doMiscTypeCode()
                            {
                                FieldName = MiscType.C_AR_INTERACTION_TYPE,
                                ValueCode = ARInteractionType.C_AR_INTERACTION_TYPE_RETURN_REQUEST
                            });
                            miscs.Add(new doMiscTypeCode()
                            {
                                FieldName = MiscType.C_AR_INTERACTION_TYPE,
                                ValueCode = ARInteractionType.C_AR_INTERACTION_TYPE_APPROVAL_REQUEST
                            });
                        }
                        else if (((arDat.dtAR.ARStatus == ARStatus.C_AR_STATUS_APPROVED) || (arDat.dtAR.ARStatus == ARStatus.C_AR_STATUS_INSTRUCTED) || (arDat.dtAR.ARStatus == ARStatus.C_AR_STATUS_REJECTED)) && (bSpecialViewEdit))
                        {
                            miscs.Add(new doMiscTypeCode()
                            {
                                FieldName = MiscType.C_AR_INTERACTION_TYPE,
                                ValueCode = ARInteractionType.C_AR_INTERACTION_TYPE_REGISTER_BY_ADMIN
                            });
                        }
                    }

                    res = new ObjectResultData();
                }

                var outlst = commonhandler.GetMiscTypeCodeList(miscs);
                if (outlst == null)
                    outlst = new List<doMiscTypeCode>();

                string display = "ValueCodeDisplay";

                res.ResultData = CommonUtil.CommonComboBox<doMiscTypeCode>("{BLANK_ID}", outlst, display, "ValueCode", null, true).ToString();
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Check status from selected interaction type when change select [InteractionType] button
        /// </summary>
        /// <param name="InteractionTypeCode"></param>
        /// <returns></returns>
        public ActionResult CTS380_InteractionTypeChange(string InteractionTypeCode)
        {
            IARHandler arhandler = ServiceContainer.GetService<IARHandler>() as IARHandler;
            ObjectResultData res = new ObjectResultData();
            CTS380_ScreenParameter sParam = GetScreenObject<CTS380_ScreenParameter>();

            try
            {
                CTS380_ARStatusReturn obj = new CTS380_ARStatusReturn();
                bool bEdit = false, bSpecialViewEdit = false;
                var arDat = arhandler.GetARDetail(sParam.pRequestNo);
                if (arDat.dtAR != null)
                {
                    hasPermission_CTS380(sParam.pRequestNo, arDat.dtAR.ARStatus, ref res, out bSpecialViewEdit, out bEdit);

                    if ((InteractionTypeCode == ARInteractionType.C_AR_INTERACTION_TYPE_REGISTER_BY_ADMIN) && (bSpecialViewEdit))
                    {
                        obj.IsEnable = true;
                    }
                    else if (InteractionTypeCode == ARInteractionType.C_AR_INTERACTION_TYPE_APPROVAL_REQUEST)
                    {
                        obj.ARStatus = ARStatus.C_AR_STATUS_WAIT_FOR_APPROVAL;
                    }
                    else if (InteractionTypeCode == ARInteractionType.C_AR_INTERACTION_TYPE_AUDIT_REQUEST)
                    {
                        obj.ARStatus = ARStatus.C_AR_STATUS_AUDITING;
                    }
                    else if (InteractionTypeCode == ARInteractionType.C_AR_INTERACTION_TYPE_APPROVE)
                    {
                        obj.ARStatus = ARStatus.C_AR_STATUS_APPROVED;
                    }
                    else if (InteractionTypeCode == ARInteractionType.C_AR_INTERACTION_TYPE_REJECT)
                    {
                        obj.ARStatus = ARStatus.C_AR_STATUS_REJECTED;
                    }
                    else if (InteractionTypeCode == ARInteractionType.C_AR_INTERACTION_TYPE_INSTRUCTION)
                    {
                        obj.ARStatus = ARStatus.C_AR_STATUS_INSTRUCTED;
                    }
                    else if (InteractionTypeCode == ARInteractionType.C_AR_INTERACTION_TYPE_RETURN_REQUEST)
                    {
                        obj.ARStatus = ARStatus.C_AR_STATUS_RETURNED_REQUEST;
                    }

                    res.ResultData = obj;
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Retrieve department item for combobox when change select [Office] drop down list on Assign person in charge information subsection
        /// </summary>
        /// <param name="OfficeCode"></param>
        /// <returns></returns>
        public ActionResult CTS380_OfficeChange(string OfficeCode)
        {
            ObjectResultData res = new ObjectResultData();
            IEmployeeMasterHandler emphandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
            IOfficeMasterHandler officehandler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler; //Add by jutarat A. on 20082012

            CTS380_ScreenParameter param = GetScreenObject<CTS380_ScreenParameter>(); //Add by jutarat A. on 20082012

            try
            {
                //Add by jutarat A. on 20082012
                if (String.IsNullOrEmpty(OfficeCode))
                {
                    res.ResultData = String.Empty;
                }
                else
                {
                    param.blnIncidentIsHeadOfficeFlag = officehandler.CheckHeadOffice(OfficeCode);
                    if (param.blnIncidentIsHeadOfficeFlag == false)
                    {
                        res.ResultData = false;
                    }
                    //End Add
                    else
                    {
                        var departLst = emphandler.GetBelongingDepartmentList(OfficeCode, null);
                        res.ResultData = CommonUtil.CommonComboBox<dtDepartment>("{BLANK_ID}"
                            , departLst, "DepartmentNameCode", "DepartmentCode"
                            , null, true).ToString();
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
        /// Retrieve AR role item for combobox when change select [Department] drop down list on Assign person in charge information subsection
        /// </summary>
        /// <param name="OfficeCode"></param>
        /// <param name="DepartmentCode"></param>
        /// <returns></returns>
        public ActionResult CTS380_DepartmentChange(string OfficeCode, string DepartmentCode)
        {
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            IARHandler arhandler = ServiceContainer.GetService<IARHandler>() as IARHandler;
            ObjectResultData res = new ObjectResultData();
            CTS380_ScreenParameter sParam = GetScreenObject<CTS380_ScreenParameter>();

            try
            {
                var arPermit = arhandler.HasARPermission(sParam.pRequestNo);
                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>();

                //Modify by Jutarat A. on 31082012
                //miscs.Add(new doMiscTypeCode()
                //{
                //    FieldName = MiscType.C_AR_ROLE,
                //    ValueCode = ARRole.C_AR_ROLE_CHIEF_OF_RELATED_OFFICE
                //});

                //if (arPermit.AssignApproverFlag)
                //{
                //    miscs.Add(new doMiscTypeCode()
                //    {
                //        FieldName = MiscType.C_AR_ROLE,
                //        ValueCode = ARRole.C_AR_ROLE_APPROVER
                //    });
                //}

                //if (arPermit.AssignAuditorFlag)
                //{
                //    miscs.Add(new doMiscTypeCode()
                //    {
                //        FieldName = MiscType.C_AR_ROLE,
                //        ValueCode = ARRole.C_AR_ROLE_AUDITOR
                //    });
                //}

                miscs.Add(new doMiscTypeCode()
                {
                    FieldName = MiscType.C_AR_ROLE,
                    ValueCode = "%"
                });

                var outlst = commonhandler.GetMiscTypeCodeList(miscs);
                if (outlst == null)
                    outlst = new List<doMiscTypeCode>();

                if (arPermit.AssignApproverFlag == false)
                {
                    foreach (doMiscTypeCode data in outlst)
                    {
                        if (data.ValueCode == ARRole.C_AR_ROLE_APPROVER)
                        {
                            outlst.Remove(data);
                            break;
                        }
                    }
                }

                if (arPermit.AssignAuditorFlag == false)
                {
                    foreach (doMiscTypeCode data in outlst)
                    {
                        if (data.ValueCode == ARRole.C_AR_ROLE_AUDITOR)
                        {
                            outlst.Remove(data);
                            break;
                        }
                    }
                }
                //End Modify

                string display = "ValueCodeDisplay";

                res.ResultData = CommonUtil.CommonComboBox<doMiscTypeCode>("{BLANK_ID}", outlst, display, "ValueCode", null, true).ToString();
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Retireve employee item for combobox when change select [AR Role] drop down list on Assign person in charge information subsection
        /// </summary>
        /// <param name="OfficeCode"></param>
        /// <param name="DepartmentCode"></param>
        /// <param name="ARRoleCode"></param>
        /// <returns></returns>
        public ActionResult CTS380_ARRoleChange(string OfficeCode, string DepartmentCode, string ARRoleCode)
        {
            IEmployeeMasterHandler emphandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
            ObjectResultData res = new ObjectResultData();

            CTS380_ScreenParameter param = GetScreenObject<CTS380_ScreenParameter>(); //Add by jutarat A. on 20082012

            try
            {
                var datTemp = emphandler.GetBelongingEmpList(OfficeCode
                    , param.blnIncidentIsHeadOfficeFlag == false ? null : DepartmentCode //DepartmentCode  //Modify by jutarat A. on 20082012
                    , null
                    //, ((ARRoleCode == ARRole.C_AR_ROLE_APPROVER) || (ARRoleCode == ARRole.C_AR_ROLE_AUDITOR)) ? FlagType.C_FLAG_ON : (bool?)null //FlagType.C_FLAG_OFF //Modify by jutarat A. on 17042013
                    , ((ARRoleCode == ARRole.C_AR_ROLE_APPROVER)) ? FlagType.C_FLAG_ON : (bool?)null //Modify by jutarat A. on 26042013
                    , null);

                //Add by jutarat A. on 21082012
                var datDist = (from t in datTemp
                               group t by new
                               {
                                   EmpNo = t.EmpNo,
                                   EmpFirstNameEN = t.EmpFirstNameEN,
                                   EmpLastNameEN = t.EmpLastNameEN,
                                   EmpFirstNameLC = t.EmpFirstNameLC,
                                   EmpLastNameLC = t.EmpLastNameLC
                               } into g
                               select g.FirstOrDefault());

                List<dtEmployeeBelonging> empLst = datDist.ToList<dtEmployeeBelonging>();
                //End Add

                CommonUtil.MappingObjectLanguage<dtEmployeeBelonging>(empLst);

                res.ResultData = CommonUtil.CommonComboBox<dtEmployeeBelonging>("{BLANK_ID}"
                    , empLst, "EmpFullNameWithCode", "EmpNo", null, true).ToString();
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Retrieve AR data for display
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS380_RetrieveARData()
        {
            ISiteMasterHandler sitehandler = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;
            IOfficeMasterHandler officehandler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
            IRentralContractHandler rentalhandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            ISaleContractHandler salehandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
            IARHandler arhandler = ServiceContainer.GetService<IARHandler>() as IARHandler;
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            ICustomerMasterHandler custhandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
            ObjectResultData res = new ObjectResultData();
            CTS380_ScreenParameter sParam = GetScreenObject<CTS380_ScreenParameter>();
            CommonUtil util = new CommonUtil();

            try
            {
                //Comment by Jutarat A. on 04042013 (Not Use)
                //string CustNameEN = "";
                //string CustNameLC = "";
                //string SiteNameEN = "";
                //string SiteNameLC = "";
                //string SiteAddressEN = "";
                //End Comment

                List<string> miscTypeName = new List<string>();
                miscTypeName.Add(MiscType.C_AR_TYPE);

                var miscList = commonhandler.GetMiscTypeCodeListByFieldName(miscTypeName);

                var arDat = arhandler.GetARDetail(sParam.pRequestNo);

                //Comment by Jutarat A. on 04042013 (Not Use)
                //if ((arDat.dtAR.ARRelavantType == ARRelevant.C_AR_RELEVANT_TYPE_CUSTOMER) && (!String.IsNullOrEmpty(arDat.dtAR.CustCode)))
                //{
                //    var custLst = custhandler.GetCustomer(arDat.dtAR.CustCode);
                //    if (custLst.Count == 1)
                //    {
                //        var tmpCust = custLst.First();
                //        CustNameEN = tmpCust.CustFullNameEN;
                //        CustNameLC = tmpCust.CustFullNameLC;
                //    }
                //}
                //else if ((arDat.dtAR.ARRelavantType == ARRelevant.C_AR_RELEVANT_TYPE_SITE) && (!String.IsNullOrEmpty(arDat.dtAR.SiteCode)))
                //{
                //    var siteLst = sitehandler.GetSite(arDat.dtAR.SiteCode, null);
                //    if (siteLst.Count == 1)
                //    {
                //        var tmpSite = siteLst.First();
                //        SiteNameEN = tmpSite.SiteNameEN;
                //        SiteNameLC = tmpSite.SiteNameLC;
                //        SiteAddressEN = tmpSite.AddressFullEN;
                //    }
                //}
                //End Comment

                CTS380_ScreenData obj = new CTS380_ScreenData();

                obj.ARInfo = new CTS380_ARInfo()
                {
                    //ARRelevantType = (arDat.dtAR.ARRelavantType == ARRelevant.C_AR_RELEVANT_TYPE_CUSTOMER) ? "1" :
                    //    (arDat.dtAR.ARRelavantType == ARRelevant.C_AR_RELEVANT_TYPE_SITE) ? "2" :
                    //    (arDat.dtAR.ARRelavantType == ARRelevant.C_AR_RELEVANT_TYPE_QUOTATION) ? "3" :
                    //    (arDat.dtAR.ARRelavantType == ARRelevant.C_AR_RELEVANT_TYPE_PROJECT) ? "4" :
                    //    (arDat.dtAR.ARRelavantType == ARRelevant.C_AR_RELEVANT_TYPE_CONTRACT) ? "5" : "",
                    ARRelevantType = arDat.dtAR.ARRelavantType,
                    RequestNo = arDat.dtAR.RequestNo,
                    ApproveNo = arDat.dtAR.ApproveNo,
                    CustomerCode = util.ConvertCustCode(arDat.dtAR.CustCode, CommonUtil.CONVERT_TYPE.TO_SHORT),

                    //Modify by Jutarat A. on 04042013
                    //CustomerNameEN = CustNameEN,
                    //CustomerNameLC = CustNameLC,
                    CustomerNameEN = arDat.dtAR.CustFullNameEN,
                    CustomerNameLC = arDat.dtAR.CustFullNameLC,
                    //End Modify

                    SiteCode = util.ConvertSiteCode(arDat.dtAR.SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT),

                    //Modify by Jutarat A. on 04042013
                    //SiteNameEN = SiteNameEN,
                    //SiteNameLC = SiteNameLC,
                    SiteNameEN = arDat.dtAR.SiteNameEN,
                    SiteNameLC = arDat.dtAR.SiteNameLC,
                    //End Modify

                    SiteAddressEN = arDat.dtAR.AddressFullEN,
                    ContractCode = util.ConvertContractCode(arDat.dtAR.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                    QuotationCode = util.ConvertQuotationTargetCode(arDat.dtAR.QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                    ProjectCode = arDat.dtAR.ProjectCode,
                    ProjectName = arDat.dtAR.ProjectName,
                    UserCode = arDat.dtAR.UserCode,
                    Requester = arDat.dtAR.RegistrantName,
                    Status = arDat.dtAR.ARStatusName,
                };

                if (arDat.dtAR.ARRelavantType == ARRelevant.C_AR_RELEVANT_TYPE_CONTRACT)
                {
                    var rentalDat = rentalhandler.GetRentalContractBasicForView(arDat.dtAR.ContractCode, null);
                    CommonUtil.MappingObjectLanguage<dtRentalContractBasicForView>(rentalDat);
                    if ((rentalDat != null) && (rentalDat.Count > 0))
                    {
                        var officeDat = officehandler.GetTbm_Office(rentalDat[0].OperationOfficeCode);

                        obj.RentalContract = new CTS380_RentalContract()
                        {
                            ContractCode = util.ConvertContractCode(rentalDat[0].ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                            AutoRenew = rentalDat[0].AutoRenewMonth.GetValueOrDefault().ToString(),
                            ContractDurationFrom = CommonUtil.TextDate(rentalDat[0].ContractStartDate),
                            ContractDurationTo = CommonUtil.TextDate(rentalDat[0].ContractEndDate),
                            ContractDurationMonth = rentalDat[0].ContractDurationMonth.GetValueOrDefault().ToString(),
                            FirstOperationDate = CommonUtil.TextDate(rentalDat[0].FirstSecurityStartDate),
                            LastChangeType = rentalDat[0].LastChangeTypeName,
                            LastOperationDate = CommonUtil.TextDate(rentalDat[0].LastChangeImplementDate),
                            MonthlyContractFee = CommonUtil.TextNumeric(rentalDat[0].OrderContractFee,2, "0.00"),
                            OldContractCode = util.ConvertContractCode(rentalDat[0].OldContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                            OperationOffice = (officeDat != null) || (officeDat.Count > 0) ? officeDat[0].OfficeName : "",
                            ProductName = rentalDat[0].ProductName,
                            SecurityType = rentalDat[0].SecurityTypeCode,
                            StartDealDate = CommonUtil.TextDate(rentalDat[0].FirstSecurityStartDate),
                            MonthlyContractFeeCurrencyType = string.IsNullOrEmpty(rentalDat[0].OrderContractFeeCurrencyType) ? CurrencyUtil.C_CURRENCY_LOCAL : rentalDat[0].OrderContractFeeCurrencyType,
                            MonthlyContractFeeUsd = CommonUtil.TextNumeric(rentalDat[0].OrderContractFeeUsd, 2, "0.00")
                        };

                    }
                    else
                    {
                        var saleDat = salehandler.GetSaleContractBasicForView(arDat.dtAR.ContractCode);
                        if ((saleDat != null) && (saleDat.Count > 0))
                        {
                            var officeDat = officehandler.GetTbm_Office(saleDat[0].OperationOfficeCode);

                            CommonUtil.MappingObjectLanguage<dtSaleContractBasicForView>(saleDat);

                            obj.SaleContract = new CTS380_SaleContract()
                            {
                                ContractCode = util.ConvertContractCode(saleDat[0].ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                                CompleteInstallDate = CommonUtil.TextDate(saleDat[0].InstallCompleteDate),
                                CustomerAcceptedDate = CommonUtil.TextDate(saleDat[0].CustAcceptanceDate),
                                ExpectedCompleteInstallDate = CommonUtil.TextDate(saleDat[0].ExpectedInstallCompleteDate),
                                ExpectedCustomerAcceptedDate = CommonUtil.TextDate(saleDat[0].ExpectedCustAcceptanceDate),
                                LastChangeType = saleDat[0].ChangeTypeName,
                                MaintainContractCode = util.ConvertContractCode(saleDat[0].MaintenanceContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                                ProcessMgmtStatus = saleDat[0].SaleProcessManageStatus,
                                ProductName = saleDat[0].ProductName,
                                SaleType = saleDat[0].SalesTypeName,
                                OperationOffice = (officeDat != null) || (officeDat.Count > 0) ? officeDat[0].OfficeName : "",
                            };
                        }
                    }
                }

                var arTypeTmp = from a in miscList where (a.FieldName == MiscType.C_AR_TYPE) && (a.ValueCode == arDat.dtAR.ARType) select a;
                var arTypeTitleTmp = arhandler.GetTbs_ARTypeTitle(arDat.dtAR.ARType, arDat.dtAR.ARTitleType);
                string arTypeTxt = "", arTitleTxt = "";

                if (arTypeTmp.Count() == 1)
                {
                    arTypeTxt = arTypeTmp.First().ValueDisplay;
                }

                if (arTypeTitleTmp.Count() == 1)
                {
                    CommonUtil.MappingObjectLanguage<tbs_ARTypeTitle>(arTypeTitleTmp);
                    arTitleTxt = arTypeTitleTmp.First().ARTitleName;
                }

                obj.ARDetail = new CTS380_ARDetail()
                {
                    ARPurpose = arDat.dtAR.ARPurpose,
                    ARSubTitle = arDat.dtAR.ARSubTitle,
                    ARTitle = arTitleTxt,
                    ARType = arTypeTxt,

                    //ContractARFee = (arDat.tbt_ARFeeAdjustment != null) ? arDat.tbt_ARFeeAdjustment.OrderContractFee.GetValueOrDefault() : 0,
                    //ContractQuotationFee = (arDat.tbt_ARFeeAdjustment != null) ? arDat.tbt_ARFeeAdjustment.NormalContractFee.GetValueOrDefault() : 0,
                    //DepositARFee = (arDat.tbt_ARFeeAdjustment != null) ? arDat.tbt_ARFeeAdjustment.OrderDepositFee.GetValueOrDefault() : 0,
                    //DepositQuotationFee = (arDat.tbt_ARFeeAdjustment != null) ? arDat.tbt_ARFeeAdjustment.NormalDepositFee.GetValueOrDefault() : 0,
                    //InstallARFee = (arDat.tbt_ARFeeAdjustment != null) ? arDat.tbt_ARFeeAdjustment.OrderInsatallationFee.GetValueOrDefault() : 0,
                    //InstallQuotationFee = (arDat.tbt_ARFeeAdjustment != null) ? arDat.tbt_ARFeeAdjustment.NormalInstallFee.GetValueOrDefault() : 0,
                    //InstallFeeARFee = (arDat.tbt_ARFeeAdjustment != null) ? arDat.tbt_ARFeeAdjustment.OrderSaleInstallationFee.GetValueOrDefault() : 0,
                    //InstallFeeQuotationFee = (arDat.tbt_ARFeeAdjustment != null) ? arDat.tbt_ARFeeAdjustment.NormalSaleInstallationFee.GetValueOrDefault() : 0,
                    //ProductPriceARFee = (arDat.tbt_ARFeeAdjustment != null) ? arDat.tbt_ARFeeAdjustment.OrderSaleProductPrice.GetValueOrDefault() : 0,
                    //ProductPriceQuotationFee = (arDat.tbt_ARFeeAdjustment != null) ? arDat.tbt_ARFeeAdjustment.NormalSaleProductPrice.GetValueOrDefault() : 0,

                    Deadline_Date = CommonUtil.TextDate(arDat.dtAR.DeadLine),
                    Deadline_Until = arDat.dtAR.DeadLineTime,
                    DueDate_Date = CommonUtil.TextDate(arDat.dtAR.DueDate),
                    DueDate_Time = CommonUtil.TextTime(arDat.dtAR.DueDateTime),
                    DueDateDeadlineType = (arDat.dtAR.DueDate.HasValue) ? "1" : "2",
                    ImportantFlag = arDat.dtAR.ImportanceFlag.GetValueOrDefault(),
                    ARStatus = arDat.dtAR.ARStatus //Add by Jutarat A. on 23082012
                };

                if (arDat.tbt_ARFeeAdjustment != null)
                {
                    #region Contract Quotation Fee

                    obj.ARDetail.ContractQuotationFeeCurrencyType = arDat.tbt_ARFeeAdjustment.NormalContractFeeCurrencyType;
                    if (obj.ARDetail.ContractQuotationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        obj.ARDetail.ContractQuotationFee = arDat.tbt_ARFeeAdjustment.NormalContractFeeUsd.GetValueOrDefault();
                    else
                        obj.ARDetail.ContractQuotationFee = arDat.tbt_ARFeeAdjustment.NormalContractFee.GetValueOrDefault();

                    #endregion
                    #region Contract AR Fee

                    obj.ARDetail.ContractARFeeCurrencyType = arDat.tbt_ARFeeAdjustment.OrderContractFeeCurrencyType;
                    if (obj.ARDetail.ContractARFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        obj.ARDetail.ContractARFee = arDat.tbt_ARFeeAdjustment.OrderContractFeeUsd.GetValueOrDefault();
                    else
                        obj.ARDetail.ContractARFee = arDat.tbt_ARFeeAdjustment.OrderContractFee.GetValueOrDefault();

                    #endregion
                    #region Deposit AR Fee

                    obj.ARDetail.DepositARFeeCurrencyType = arDat.tbt_ARFeeAdjustment.OrderDepositFeeCurrencyType;
                    if (obj.ARDetail.DepositARFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        obj.ARDetail.DepositARFee = arDat.tbt_ARFeeAdjustment.OrderDepositFeeUsd.GetValueOrDefault();
                    else
                        obj.ARDetail.DepositARFee = arDat.tbt_ARFeeAdjustment.OrderDepositFee.GetValueOrDefault();

                    #endregion
                    #region Deposit Quotation Fee

                    obj.ARDetail.DepositQuotationFeeCurrencyType = arDat.tbt_ARFeeAdjustment.NormalDepositFeeCurrencyType;
                    if (obj.ARDetail.DepositQuotationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        obj.ARDetail.DepositQuotationFee = arDat.tbt_ARFeeAdjustment.NormalDepositFeeUsd.GetValueOrDefault();
                    else
                        obj.ARDetail.DepositQuotationFee = arDat.tbt_ARFeeAdjustment.NormalDepositFee.GetValueOrDefault();

                    #endregion
                    #region Install AR Fee

                    obj.ARDetail.InstallARFeeCurrencyType = arDat.tbt_ARFeeAdjustment.OrderInsatallationFeeCurrencyType;
                    if (obj.ARDetail.InstallARFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        obj.ARDetail.InstallARFee = arDat.tbt_ARFeeAdjustment.OrderInsatallationFeeUsd.GetValueOrDefault();
                    else
                        obj.ARDetail.InstallARFee = arDat.tbt_ARFeeAdjustment.OrderInsatallationFee.GetValueOrDefault();

                    #endregion
                    #region Install Quotation Fee

                    obj.ARDetail.InstallQuotationFeeCurrencyType = arDat.tbt_ARFeeAdjustment.NormalInstallFeeCurrencyType;
                    if (obj.ARDetail.InstallQuotationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        obj.ARDetail.InstallQuotationFee = arDat.tbt_ARFeeAdjustment.NormalInstallFeeUsd.GetValueOrDefault();
                    else
                        obj.ARDetail.InstallQuotationFee = arDat.tbt_ARFeeAdjustment.NormalInstallFee.GetValueOrDefault();

                    #endregion
                    #region Install Fee AR Fee

                    obj.ARDetail.InstallFeeARFeeCurrencyType = arDat.tbt_ARFeeAdjustment.OrderSaleInstallationFeeCurrencyType;
                    if (obj.ARDetail.InstallFeeARFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        obj.ARDetail.InstallFeeARFee = arDat.tbt_ARFeeAdjustment.OrderSaleInstallationFeeUsd.GetValueOrDefault();
                    else
                        obj.ARDetail.InstallFeeARFee = arDat.tbt_ARFeeAdjustment.OrderSaleInstallationFee.GetValueOrDefault();

                    #endregion
                    #region Install Fee Quotation Fee

                    obj.ARDetail.InstallFeeQuotationFeeCurrencyType = arDat.tbt_ARFeeAdjustment.NormalSaleInstallationFeeCurrencyType;
                    if (obj.ARDetail.InstallFeeQuotationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        obj.ARDetail.InstallFeeQuotationFee = arDat.tbt_ARFeeAdjustment.NormalSaleInstallationFeeUsd.GetValueOrDefault();
                    else
                        obj.ARDetail.InstallFeeQuotationFee = arDat.tbt_ARFeeAdjustment.NormalSaleInstallationFee.GetValueOrDefault();

                    #endregion
                    #region Product Price AR Fee

                    obj.ARDetail.ProductPriceARFeeCurrencyType = arDat.tbt_ARFeeAdjustment.OrderSaleProductPriceCurrencyType;
                    if (obj.ARDetail.ProductPriceARFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        obj.ARDetail.ProductPriceARFee = arDat.tbt_ARFeeAdjustment.OrderSaleProductPriceUsd.GetValueOrDefault();
                    else
                        obj.ARDetail.ProductPriceARFee = arDat.tbt_ARFeeAdjustment.OrderSaleProductPrice.GetValueOrDefault();

                    #endregion
                    #region Product Price Quotation Fee

                    obj.ARDetail.ProductPriceQuotationFeeCurrencyType = arDat.tbt_ARFeeAdjustment.NormalSaleProductPriceCurrencyType;
                    if (obj.ARDetail.ProductPriceQuotationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        obj.ARDetail.ProductPriceQuotationFee = arDat.tbt_ARFeeAdjustment.NormalSaleProductPriceUsd.GetValueOrDefault();
                    else
                        obj.ARDetail.ProductPriceQuotationFee = arDat.tbt_ARFeeAdjustment.NormalSaleProductPrice.GetValueOrDefault();

                    #endregion
                }


                var requesterEmpNo = (from a in arDat.dtARRole where a.ARRoleType == ARRole.C_AR_ROLE_REQUESTER select a.EmpNo).FirstOrDefault();
                var arRoleDat = from a in arDat.dtARRole
                                select new CTS380_PICDat()
                                    {
                                        ARRoleCode = a.ARRoleType,
                                        ARRoleName = a.ARRoleTypeName,
                                        OfficeCode = a.OFFiceCode,
                                        OfficeName = a.OfficeName,
                                        DepartmentCode = a.DepartmentCode,
                                        DepartmentName = CommonUtil.TextCodeName(a.DepartmentCode, a.DepartmentName),
                                        EmployeeName = a.EmpFirstName + " " + a.EmpLastName,
                                        EmpNo = a.EmpNo,
                                        CanDelete = (
                                            a.ARRoleType != ARRole.C_AR_ROLE_REQUESTER
                                            && !(a.ARRoleType == ARRole.C_AR_ROLE_APPROVER && requesterEmpNo == CommonUtil.dsTransData.dtUserData.EmpNo)
                                        )
                                    };

                obj.ARRole = arRoleDat.ToList();

                string sAuditDetailHistory = "", sAuditDetailHistoryWithUpdate = "";
                createHistory_CTS380(arDat, out sAuditDetailHistory, out sAuditDetailHistoryWithUpdate);

                obj.ARDetail.AuditDetailHistory = sAuditDetailHistory;
                obj.ARDetail.AuditDetailHistoryWithUpdate = sAuditDetailHistoryWithUpdate;

                bool bCanSpecialViewEdit = false, bCanEdit = false;
                doARPermission doARPermissionData = hasPermission_CTS380(sParam.pRequestNo, arDat.dtAR.ARStatus, ref res, out bCanSpecialViewEdit, out bCanEdit);
                commonhandler.ClearTemporaryUploadFile(sParam.pRequestNo);

                obj.CanEdit = bCanEdit;
                obj.CanSpecialViewEdit = bCanSpecialViewEdit;

                sParam.newAttachLst = new Dictionary<int, string>();
                sParam.delAttachLst = new Dictionary<int, string>();

                obj.CanModPIC = false;
                //if (bCanSpecialViewEdit || 
                if (doARPermissionData != null && (doARPermissionData.AssignApproverFlag || doARPermissionData.AssignAuditorFlag)) //) //Modify by Jutarat A. on 12102012
                {
                    obj.CanModPIC = true;
                }

                //Add by Jutarat A. on 26042013
                bool isEnableAlarm = false;
                bool isEnableSale = false;
                SetEnableFeeAdjustment_CTS350(arDat.dtAR, ref isEnableAlarm, ref isEnableSale);

                obj.IsEnableAlarm = isEnableAlarm;
                obj.IsEnableSale = isEnableSale;
                //End Add

                sParam.ScreenData = obj; //Add by Jutarat A. on 24082012
                res.ResultData = obj;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Validate permission, require field, and business before edit AR
        /// </summary>
        /// <param name="regisObj"></param>
        /// <returns></returns>
        public ActionResult CTS380_ValidateData(CTS380_EntryData regisObj)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                CTS380_ScreenParameter sParam = GetScreenObject<CTS380_ScreenParameter>();
                res = ValidateAuthority_CTS380(res, sParam.pRequestNo);
                if (res.IsError)
                {
                    return Json(res);
                }

                res = ValidateEntryData_CTS380(res, sParam, regisObj);
                if (res.IsError)
                {
                    return Json(res);
                }

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Proceed edit AR data
        /// </summary>
        /// <param name="regisObj"></param>
        /// <returns></returns>
        public ActionResult CTS380_EditARData(CTS380_EntryData regisObj)
        {
            IARHandler arhandler = ServiceContainer.GetService<IARHandler>() as IARHandler;
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            IEmployeeMasterHandler emphandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
            CTS380_ScreenParameter sParam = GetScreenObject<CTS380_ScreenParameter>();
            ObjectResultData res = new ObjectResultData();

            try
            {
                res = ValidateAuthority_CTS380(res, sParam.pRequestNo);
                if (res.IsError)
                {
                    return Json(res);
                }

                res = ValidateEntryData_CTS380(res, sParam, regisObj);
                if (res.IsError)
                {
                    return Json(res);
                }

                using (TransactionScope scope = new TransactionScope())
                {
                    if (CheckIsSuspending(res))
                    {
                        res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0049, null, null);
                        return Json(res);
                    }

                    dsARDetailIn objIn = new dsARDetailIn();
                    tbt_AR currAR = arhandler.GetTbt_AR(sParam.pRequestNo)[0];
                    string newApproveNo = "", oldDueDate_Deadline_Type = "";
                    dsARDetail arDat = arhandler.GetARDetail(sParam.pRequestNo);
                    tbt_ARHistory newHistory = new tbt_ARHistory();
                    List<tbt_ARHistoryDetail> changeList = new List<tbt_ARHistoryDetail>();
                    List<tbt_ARRole> addList = new List<tbt_ARRole>();
                    List<tbt_ARRole> editList = new List<tbt_ARRole>();
                    List<int> delList = new List<int>();

                    List<tbt_ARFeeAdjustment> editARFeeList = new List<tbt_ARFeeAdjustment>(); //Add by Jutarat A. on 03042013

                    //Modify by Jutarat A. on 28082012
                    //if (((regisObj.InteractionType == ARInteractionType.C_AR_INTERACTION_TYPE_APPROVE)
                    //    || (regisObj.InteractionType == ARInteractionType.C_AR_INTERACTION_TYPE_REJECT)
                    //    || (regisObj.InteractionType == ARInteractionType.C_AR_INTERACTION_TYPE_INSTRUCTION))
                    //    && (String.IsNullOrEmpty(arDat.dtAR.ApproveNo)))
                    //{
                    //    newApproveNo = arhandler.GenerateARApproveNo(regisObj.InteractionType);
                    //}
                    if ((regisObj.StatusAfterUpdate == ARStatus.C_AR_STATUS_APPROVED)
                        || (regisObj.StatusAfterUpdate == ARStatus.C_AR_STATUS_REJECTED)
                        || (regisObj.StatusAfterUpdate == ARStatus.C_AR_STATUS_INSTRUCTED))
                    {
                        if (String.IsNullOrEmpty(arDat.dtAR.ApproveNo) || (regisObj.StatusAfterUpdate != arDat.dtAR.ARStatus))
                        {
                            newApproveNo = arhandler.GenerateARApproveNo(regisObj.StatusAfterUpdate);
                        }
                    }
                    //End Modify

                    oldDueDate_Deadline_Type = arDat.dtAR.DueDate.HasValue ? "1" : "2";

                    newHistory.AuditDetail = regisObj.AuditDetail;
                    newHistory.InteractionType = regisObj.InteractionType;
                    newHistory.RequestNo = arDat.dtAR.RequestNo;
                    currAR.hasRespondingDetailFlag = (!String.IsNullOrEmpty(regisObj.AuditDetail) || arDat.dtAR.hasRespondingDetailFlag.GetValueOrDefault());

                    foreach (var newAttachItem in sParam.newAttachLst)
                    {
                        changeList.Add(new tbt_ARHistoryDetail()
                        {
                            ChangeItemName = "Attached document",
                            ItemNewValue = newAttachItem.Value,
                            ItemOldValue = "Add"
                        });
                    }

                    foreach (var delAttachItem in sParam.delAttachLst)
                    {
                        changeList.Add(new tbt_ARHistoryDetail()
                        {
                            ChangeItemName = "Attached document",
                            ItemNewValue = "Delete",
                            ItemOldValue = delAttachItem.Value,
                        });
                    }

                    //Comment by Jutarat A. on 03092012
                    //if ((regisObj.DueDate_DeadlineType == oldDueDate_Deadline_Type) && (oldDueDate_Deadline_Type == "1"))
                    //{
                    //    if ((regisObj.DueDate_Date != arDat.dtAR.DueDate) || (regisObj.DueDate_Time != arDat.dtAR.DueDateTime))
                    //    {
                    //        changeList.Add(new tbt_ARHistoryDetail()
                    //        {
                    //            ChangeItemName = "DueDate",
                    //            ItemNewValue = CommonUtil.TextDate(regisObj.DueDate_Date) + " " + CommonUtil.TextTime(regisObj.DueDate_Time),
                    //            ItemOldValue = CommonUtil.TextDate(arDat.dtAR.DueDate) + " " + CommonUtil.TextTime(arDat.dtAR.DueDateTime),
                    //        });
                    //    }
                    //}
                    //else if ((regisObj.DueDate_DeadlineType == oldDueDate_Deadline_Type) && (oldDueDate_Deadline_Type == "2"))
                    //{
                    //    if ((regisObj.Deadline_Date != arDat.dtAR.DeadLine) || (regisObj.Deadline_Until != arDat.dtAR.DeadLineTime))
                    //    {
                    //        string newTime = "", oldTime = "";
                    //        List<doMiscTypeCode> miscs = new List<doMiscTypeCode>();
                    //        miscs.Add(new doMiscTypeCode()
                    //        {
                    //            FieldName = MiscType.C_DEADLINE_TIME_TYPE,
                    //            ValueCode = regisObj.Deadline_Until
                    //        });

                    //        miscs.Add(new doMiscTypeCode()
                    //        {
                    //            FieldName = MiscType.C_DEADLINE_TIME_TYPE,
                    //            ValueCode = arDat.dtAR.DeadLineTime
                    //        });

                    //        var outlst = commonhandler.GetMiscTypeCodeList(miscs);
                    //        CommonUtil.MappingObjectLanguage<doMiscTypeCode>(outlst);

                    //        newTime = (from a in outlst where a.ValueCode == regisObj.Deadline_Until select a).ToList()[0].ValueDisplay;
                    //        oldTime = (from a in outlst where a.ValueCode == arDat.dtAR.DeadLineTime select a).ToList()[0].ValueDisplay;

                    //        if ((outlst != null) && (outlst.Count == 2))
                    //        {
                    //            changeList.Add(new tbt_ARHistoryDetail()
                    //            {
                    //                ChangeItemName = "Deadline",
                    //                ItemNewValue = CommonUtil.TextDate(regisObj.Deadline_Date) + " " + newTime,
                    //                ItemOldValue = CommonUtil.TextDate(arDat.dtAR.DeadLine) + " " + oldTime,
                    //            });
                    //        }
                    //    }
                    //}
                    //else if ((oldDueDate_Deadline_Type == "1") && (regisObj.DueDate_DeadlineType == "2"))
                    //{
                    //    List<doMiscTypeCode> miscs = new List<doMiscTypeCode>();
                    //    miscs.Add(new doMiscTypeCode()
                    //    {
                    //        FieldName = MiscType.C_DEADLINE_TIME_TYPE,
                    //        ValueCode = regisObj.Deadline_Until
                    //    });

                    //    var outlst = commonhandler.GetMiscTypeCodeList(miscs);
                    //    CommonUtil.MappingObjectLanguage<doMiscTypeCode>(outlst);

                    //    if ((outlst != null) && (outlst.Count == 1))
                    //    {
                    //        changeList.Add(new tbt_ARHistoryDetail()
                    //        {
                    //            ChangeItemName = "Deadline",
                    //            ItemNewValue = "DeadLine " + CommonUtil.TextDate(regisObj.Deadline_Date) + " " + outlst[0].ValueDisplay,
                    //            ItemOldValue = "DueDate " + CommonUtil.TextDate(arDat.dtAR.DueDate) + " " + CommonUtil.TextTime(arDat.dtAR.DueDateTime),
                    //        });
                    //    }
                    //}
                    //else if ((oldDueDate_Deadline_Type == "2") && (regisObj.DueDate_DeadlineType == "1"))
                    //{
                    //    List<doMiscTypeCode> miscs = new List<doMiscTypeCode>();
                    //    miscs.Add(new doMiscTypeCode()
                    //    {
                    //        FieldName = MiscType.C_DEADLINE_TIME_TYPE,
                    //        ValueCode = arDat.dtAR.DeadLineTime
                    //    });

                    //    var outlst = commonhandler.GetMiscTypeCodeList(miscs);
                    //    CommonUtil.MappingObjectLanguage<doMiscTypeCode>(outlst);

                    //    if ((outlst != null) && (outlst.Count == 1))
                    //    {
                    //        changeList.Add(new tbt_ARHistoryDetail()
                    //        {
                    //            ChangeItemName = "DueDate",
                    //            ItemNewValue = "DueDate " + CommonUtil.TextDate(regisObj.DueDate_Date) + " " + CommonUtil.TextTime(regisObj.DueDate_Time),
                    //            ItemOldValue = "DeadLine " + CommonUtil.TextDate(arDat.dtAR.DeadLine) + " " + outlst[0].ValueDisplay,
                    //        });
                    //    }
                    //}
                    //End Comment

                    //Modify by Jutarat A. on 24082012
                    //currAR.ApproveNo = (!String.IsNullOrEmpty(newApproveNo)) ? newApproveNo : "";
                    if (String.IsNullOrEmpty(newApproveNo) == false)
                    {
                        currAR.ApproveNo = newApproveNo;
                        currAR.ApprovedDate = DateTime.Now; //Add by Jutarat A. on 05092012
                    }
                    else
                    {
                        if (regisObj.StatusAfterUpdate != arDat.dtAR.ARStatus)
                        {
                            currAR.ApproveNo = null;
                            currAR.ApprovedDate = null; //Add by Jutarat A. on 05092012
                        }
                    }
                    //End Modify

                    currAR.ARStatus = regisObj.StatusAfterUpdate;
                    currAR.InteractionType = regisObj.InteractionType;

                    //Comment by Jutarat A. on 05092012
                    //currAR.ApprovedDate = ((regisObj.InteractionType == ARInteractionType.C_AR_INTERACTION_TYPE_APPROVE)
                    //    || (regisObj.InteractionType == ARInteractionType.C_AR_INTERACTION_TYPE_REJECT)
                    //    || (regisObj.InteractionType == ARInteractionType.C_AR_INTERACTION_TYPE_INSTRUCTION)) ? (DateTime?)DateTime.Now : null;
                    //End Comment

                    //Comment by Jutarat A. on 03092012
                    //if (regisObj.DueDate_DeadlineType == "1")
                    //{
                    //    currAR.DueDate = regisObj.DueDate_Date;
                    //    currAR.DueDateTime = regisObj.DueDate_Time;
                    //    currAR.DeadLine = null;
                    //    currAR.DeadLineTime = null;
                    //}
                    //else
                    //{
                    //    currAR.DueDate = null;
                    //    currAR.DueDateTime = null;
                    //    currAR.DeadLine = regisObj.Deadline_Date;
                    //    currAR.DeadLineTime = regisObj.Deadline_Until;
                    //}
                    //End Comment

                    List<string> miscNameList = new List<string>();
                    miscNameList.Add(MiscType.C_AR_ROLE);
                    var miscListFull = commonhandler.GetMiscTypeCodeListByFieldName(miscNameList);

                    if (regisObj.HistoryList != null)
                    {
                        foreach (CTS380_HistoryLog item in regisObj.HistoryList)
                        {
                            if ((item.FunctionType == FunctionID.C_FUNC_ID_EDIT) || (item.FunctionType == FunctionID.C_FUNC_ID_DEL))
                            {
                                var currRoleItem = (from a in arDat.dtARRole where a.EmpNo == item.EmpNo select a).ToList();
                                var currEmp = emphandler.GetTbm_Employee(currRoleItem[0].EmpNo);
                                var currRoleName = (from a in miscListFull where a.ValueCode == currRoleItem[0].ARRoleType select a).ToList();

                                CommonUtil.MappingObjectLanguage<tbm_Employee>(currEmp);

                                if (item.FunctionType == FunctionID.C_FUNC_ID_DEL)
                                {
                                    var delItem = (from a in arDat.dtARRole where a.EmpNo == item.EmpNo select a).First();
                                    delList.Add(delItem.ARRoleID);

                                    CommonUtil.MappingObjectLanguage(delItem);

                                    changeList.Add(new tbt_ARHistoryDetail()
                                    {
                                        ChangeItemName = "Person in charge " + delItem.ARRoleTypeName,
                                        ItemOldValue = delItem.EmpFirstName + " " + delItem.EmpLastName,
                                        ItemNewValue = "Remove"
                                    });
                                }
                                else if (item.FunctionType == FunctionID.C_FUNC_ID_EDIT)
                                {
                                    var modRoleItem = (from a in regisObj.ARRole where a.EmpNo == item.EmpNo select a).ToList(); //Add by Jutarat A. on 23012013

                                    editList.Add(new tbt_ARRole
                                    {

                                        DepartmentCode = currRoleItem[0].DepartmentCode,
                                        EmpNo = currRoleItem[0].EmpNo,
                                        RequestNo = arDat.dtAR.RequestNo,
                                        ARRoleType = modRoleItem[0].ARRoleCode, //currRoleItem[0].ARRoleType, //Modify by Jutarat A. on 23012013
                                        OfficeCode = currRoleItem[0].OFFiceCode,
                                        ARRoleID = currRoleItem[0].ARRoleID, //Add by Jutarat A. on 23012013
                                        UpdateDate = currRoleItem[0].UpdateDate //Add by Jutarat A. on 23012013
                                    });

                                    changeList.Add(new tbt_ARHistoryDetail()
                                    {
                                        ChangeItemName = "Person in charge " + currRoleName[0].ValueDisplay,
                                        ItemOldValue = currEmp[0].EmpFullName,
                                        ItemNewValue = "Edit"
                                    });
                                }
                            }
                            else if (item.FunctionType == FunctionID.C_FUNC_ID_ADD)
                            {
                                var currRoleItem = (from a in regisObj.ARRole where a.EmpNo == item.EmpNo select a).ToList();
                                var currEmp = emphandler.GetTbm_Employee(currRoleItem[0].EmpNo);
                                var currRoleName = (from a in miscListFull where a.ValueCode == currRoleItem[0].ARRoleCode select a).ToList();

                                CommonUtil.MappingObjectLanguage<tbm_Employee>(currEmp);

                                addList.Add(new tbt_ARRole
                                {

                                    DepartmentCode = currRoleItem[0].DepartmentCode,
                                    EmpNo = currRoleItem[0].EmpNo,
                                    RequestNo = arDat.dtAR.RequestNo,
                                    ARRoleType = currRoleItem[0].ARRoleCode,
                                    OfficeCode = currRoleItem[0].OfficeCode,
                                });

                                changeList.Add(new tbt_ARHistoryDetail()
                                {
                                    ChangeItemName = "Person in charge " + currRoleName[0].ValueDisplay,
                                    ItemOldValue = "Add",
                                    ItemNewValue = currEmp[0].EmpFullName
                                });
                            }
                        }
                    }

                    //List<CTS380_HistoryLog> trueDelList = new List<CTS380_HistoryLog>();
                    //var currentRoleList = arDat.dtARRole;
                    //foreach (var item in regisObj.OriginList)
                    //{
                    //    //var filterItem = from a in entryDat.HistoryList where a.EmpNo == item.EmpNo select a;

                    //    if (item.FunctionType == FunctionID.C_FUNC_ID_DEL)
                    //    {
                    //        trueDelList.Add(item);

                    //        var currRoleItem = (from a in regisObj.ARRole where a.EmpNo == item.EmpNo select a).ToList();
                    //        var currEmp = emphandler.GetTbm_Employee(currRoleItem[0].EmpNo);
                    //        var currRoleName = (from a in miscListFull where a.ValueCode == currRoleItem[0].ARRoleCode select a).ToList();

                    //        var delItem = from a in currentRoleList where a.EmpNo == item.EmpNo select a;
                    //        delList.Add(delItem.First().ARRoleID);

                    //        CommonUtil.MappingObjectLanguage<tbm_Employee>(currEmp);
                    //        CommonUtil.MappingObjectLanguage<doMiscTypeCode>(currRoleName);

                    //        changeList.Add(new tbt_ARHistoryDetail()
                    //        {
                    //            ChangeItemName = "Person in charge " + currRoleName[0].ValueDisplay,
                    //            ItemOldValue = currEmp[0].EmpFullName,
                    //            ItemNewValue = "Remove"
                    //        });
                    //    }
                    //}

                    //regisObj.HistoryList.AddRange(trueDelList);

                    //Add by Jutarat A. on 04042013
                    if (arDat != null && arDat.tbt_ARFeeAdjustment != null)
                    {
                        tbt_ARFeeAdjustment editARFee = CommonUtil.CloneObject<tbt_ARFeeAdjustment, tbt_ARFeeAdjustment>(arDat.tbt_ARFeeAdjustment);
                        editARFee.NormalContractFee = regisObj.ContractFee_Quotation;
                        editARFee.OrderContractFee = regisObj.ContractFee_AR;
                        editARFee.NormalDepositFee = regisObj.Deposit_Quotation;
                        editARFee.OrderDepositFee = regisObj.Deposit_AR;
                        editARFee.NormalInstallFee = regisObj.Installation_Quotation;
                        editARFee.OrderInsatallationFee = regisObj.Installation_AR;

                        editARFee.NormalSaleProductPrice = regisObj.ProductPrice_Quotation;
                        editARFee.OrderSaleProductPrice = regisObj.ProductPrice_AR;
                        editARFee.NormalSaleInstallationFee = regisObj.InstallFee_Quotation;
                        editARFee.OrderSaleInstallationFee = regisObj.InstallFee_AR;

                        editARFeeList.Add(editARFee);
                    }
                    //End Add

                    objIn.tbt_AR = currAR;
                    objIn.tbt_ARHistory = newHistory;
                    objIn.tbt_ARHistoryDetail = changeList;
                    objIn.tbt_ARRoleAdd = addList;
                    objIn.tbt_ARRoleEdit = editList;
                    objIn.tbt_ARRoleDelete = delList;
                    objIn.tbt_ARFeeAdjustment = editARFeeList; //Add by Jutarat A. on 04042013

                    string resARStatus = "";

                    arhandler.UpdateARDetail(objIn, out resARStatus);

                    //Modify by Jutarat A. on 07092012
                    if (String.IsNullOrEmpty(sParam.MailTo) == false)
                    {
                        var msgEx = SendMail_CTS380(regisObj, arDat, resARStatus, sParam.MailTo, objIn.tbt_AR);

                        if (msgEx != null)
                            res.AddErrorMessage(msgEx);
                    }
                    //End Modify

                    res.ResultData = true;

                    scope.Complete();
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
        /// Validate user data permission
        /// </summary>
        /// <param name="pRequestNo"></param>
        /// <param name="arStatus"></param>
        /// <param name="res"></param>
        /// <param name="bSpecialViewEdit"></param>
        /// <param name="bEdit"></param>
        /// <returns></returns>
        private doARPermission hasPermission_CTS380(string pRequestNo, string arStatus, ref ObjectResultData res, out bool bSpecialViewEdit, out bool bEdit)
        {
            doARPermission arPermit = null;
            doARPermission doARPermissionData = null;

            try
            {
                IARHandler arhandler = ServiceContainer.GetService<IARHandler>() as IARHandler;
                bSpecialViewEdit = false;
                bEdit = false;

                //Modify by Jutarat A. on 05102012
                //if (CheckUserPermission(ScreenID.C_SCREEN_ID_VIEW_EDIT_AR, FunctionID.C_FUNC_ID_SPECIAL_VIEW_EDIT_AR))
                //{
                //    bSpecialViewEdit = true;
                //}

                //if (CheckUserPermission(ScreenID.C_SCREEN_ID_VIEW_EDIT_AR, FunctionID.C_FUNC_ID_EDIT))
                //{
                //    bEdit = true;
                //}
                CTS380_ScreenParameter param = GetScreenObject<CTS380_ScreenParameter>();
                if (param.ScreenData != null)
                {
                    bSpecialViewEdit = param.ScreenData.IsSpecialView;
                    bEdit = param.ScreenData.IsEditable;
                    arPermit = param.ScreenData.ARPermissionData;
                }
                //End Modify

                //if (bSpecialViewEdit || bEdit) //Comment by Jutarat A. on 16102012
                //{
                //Modify by Jutarat A. on 05102012
                if (arPermit == null)
                    arPermit = arhandler.HasARPermission(pRequestNo);
                //End Modify

                //Add by Jutarat A. on 12102012
                if (arPermit.EditARDetailFlag)
                {
                    bEdit = true;
                }
                else
                {
                    bEdit = false;
                }
                //End Add

                //if (arPermit.EditARDetailFlag) //Comment by Jutarat A. on 12102012
                //{
                var arRole = from a in arhandler.GetTbt_ARRole(pRequestNo) where a.EmpNo == CommonUtil.dsTransData.dtUserData.EmpNo select a;
                if ((arRole != null) && (arRole.Count() > 0))
                {
                    var arRoleObj = arRole.ToList()[0];
                    if ((arStatus == ARStatus.C_AR_STATUS_RETURNED_REQUEST) && (arRoleObj.ARRoleType != ARRole.C_AR_ROLE_REQUESTER))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3142, null, null);
                    }
                    else if ((arStatus == ARStatus.C_AR_STATUS_WAIT_FOR_APPROVAL) && (arRoleObj.ARRoleType != ARRole.C_AR_ROLE_APPROVER))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3143, null, null);
                    }
                    else if ((arStatus == ARStatus.C_AR_STATUS_AUDITING) && (arRoleObj.ARRoleType != ARRole.C_AR_ROLE_AUDITOR))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3144, null, null);
                    }
                    else if ((arStatus == ARStatus.C_AR_STATUS_APPROVED) && (!bSpecialViewEdit))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3145, null, null);
                    }
                    else if ((arStatus == ARStatus.C_AR_STATUS_INSTRUCTED) && (!bSpecialViewEdit))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3146, null, null);
                    }
                    else if ((arStatus == ARStatus.C_AR_STATUS_REJECTED) && (!bSpecialViewEdit))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3147, null, null);
                    }
                    else if ((arStatus != ARStatus.C_AR_STATUS_APPROVED)
                        && (arStatus != ARStatus.C_AR_STATUS_AUDITING)
                        && (arStatus != ARStatus.C_AR_STATUS_INSTRUCTED)
                        && (arStatus != ARStatus.C_AR_STATUS_REJECTED)
                        && (arStatus != ARStatus.C_AR_STATUS_RETURNED_REQUEST)
                        && (arStatus != ARStatus.C_AR_STATUS_WAIT_FOR_APPROVAL))
                    {
                        bSpecialViewEdit = false;
                        bEdit = false;
                    }

                    if (res.IsError)
                    {
                        bSpecialViewEdit = false;
                        bEdit = false;
                    }
                }
                else
                {
                    if ((arStatus == ARStatus.C_AR_STATUS_RETURNED_REQUEST))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3142, null, null);
                    }
                    else if ((arStatus == ARStatus.C_AR_STATUS_WAIT_FOR_APPROVAL))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3143, null, null);
                    }
                    else if ((arStatus == ARStatus.C_AR_STATUS_AUDITING))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3144, null, null);
                    }
                    else if ((arStatus == ARStatus.C_AR_STATUS_APPROVED) && (!bSpecialViewEdit))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3145, null, null);
                    }
                    else if ((arStatus == ARStatus.C_AR_STATUS_INSTRUCTED) && (!bSpecialViewEdit))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3146, null, null);
                    }
                    else if ((arStatus == ARStatus.C_AR_STATUS_REJECTED) && (!bSpecialViewEdit))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3147, null, null);
                    }
                    else if ((arStatus != ARStatus.C_AR_STATUS_APPROVED)
                       && (arStatus != ARStatus.C_AR_STATUS_AUDITING)
                       && (arStatus != ARStatus.C_AR_STATUS_INSTRUCTED)
                       && (arStatus != ARStatus.C_AR_STATUS_REJECTED)
                       && (arStatus != ARStatus.C_AR_STATUS_RETURNED_REQUEST)
                       && (arStatus != ARStatus.C_AR_STATUS_WAIT_FOR_APPROVAL))
                    {
                        bSpecialViewEdit = false;
                        bEdit = false;
                    }

                    if (res.IsError)
                    {
                        bSpecialViewEdit = false;
                        bEdit = false;
                    }
                }
                //}
                //else
                //{
                //    bEdit = false;
                //}

                doARPermissionData = arPermit;
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return doARPermissionData;
        }

        /// <summary>
        /// Generate history
        /// </summary>
        /// <param name="arDat"></param>
        /// <param name="normalText"></param>
        /// <param name="allText"></param>
        private void createHistory_CTS380(dsARDetail arDat, out string normalText, out string allText)
        {
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            string normaltxt_html = "<span class=\"historyTxt\">{0}</span><br />";
            string normaltxtindent_html = "<span class=\"historyTxtIndent\">{0}</span><br />";
            string normaltxtbr_html = "<br />";

            normalText = "";
            allText = "";

            var hisListSort = from a in arDat.tbt_ARHistory orderby a.CreateDate descending select a;

            foreach (var hisItem in hisListSort.ToList())
            {
                var currOffice = (from a in arDat.dtEmployeeOffice where a.EmpNo == hisItem.CreateBy select a).ToList();
                CommonUtil.MappingObjectLanguage<dtEmployeeOffice>(currOffice);

                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>();
                miscs.Add(new doMiscTypeCode()
                {
                    FieldName = MiscType.C_AR_INTERACTION_TYPE,
                    ValueCode = hisItem.InteractionType
                });

                var outlst = commonhandler.GetMiscTypeCodeList(miscs);

                string tmpText = String.Format("{0} : {1} : {2} : {3}"
                    , CommonUtil.TextDate(hisItem.CreateDate) + (hisItem.CreateDate != null ? " " + CommonUtil.TextTime(hisItem.CreateDate.Value.TimeOfDay) : "")
                    , (currOffice.Count > 0) ? currOffice[0].EmpFirstName + " " + currOffice[0].EmpLastName : ""
                    , (currOffice.Count > 0) ? currOffice[0].OfficeName : ""
                    , ((outlst != null) && (outlst.Count > 0)) ? outlst[0].ValueDisplay : "");

                normalText += normaltxt_html.Clone().ToString().Replace("{0}", tmpText);
                allText += normaltxt_html.Clone().ToString().Replace("{0}", tmpText);

                if (!String.IsNullOrEmpty(hisItem.AuditDetail))
                {
                    normalText += normaltxt_html.Clone().ToString().Replace("{0}", hisItem.AuditDetail);
                    allText += normaltxt_html.Clone().ToString().Replace("{0}", hisItem.AuditDetail);
                }

                var hisDetail = from a in arDat.tbt_ARHistoryDetail where a.ARHistoryID == hisItem.ARHistoryID select a;
                foreach (var detailItem in hisDetail.ToList())
                {
                    string subTxt = String.Format("{0} : {1} => {2}", detailItem.ChangeItemName, detailItem.ItemOldValue, detailItem.ItemNewValue);
                    allText += normaltxtindent_html.Clone().ToString().Replace("{0}", subTxt);
                }

                normalText += normaltxtbr_html;
                allText += normaltxtbr_html;
            }
        }

        /// <summary>
        /// Send email 
        /// </summary>
        /// <param name="entryObj"></param>
        /// <param name="arDat"></param>
        /// <param name="arStatus"></param>
        /// <returns></returns>
        private Exception SendMail_CTS380(CTS380_EntryData entryObj, dsARDetail arDat, string arStatus, string strMailTo, tbt_AR ARData) //Add strMailTo,ARData by Jutarat A. on 07092012
        {
            Exception resEx = null;

            try
            {
                IARHandler arhandler = ServiceContainer.GetService<IARHandler>() as IARHandler;
                IEmployeeMasterHandler emphandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                var markLst = from a in entryObj.ARRole where a.SendMail select a;
                //var sendLst = from a in arDat.dtARRole where markLst.Contains(a.EmpNo) select a;
                string mailTolst = "";

                //Modify by Jutarat A. on 07092012
                if (String.IsNullOrEmpty(strMailTo))
                {
                    List<dtEmployee> lstEmployee = emphandler.GetEmployee(null, null, null);
                    foreach (var item in markLst.ToList())
                    {
                        //var empRes = emphandler.GetEmployee(item.EmpNo, null, null);
                        List<dtEmployee> empRes = (from t in lstEmployee where t.EmpNo == item.EmpNo select t).ToList<dtEmployee>();
                        if (empRes != null)
                        {
                            if (mailTolst.Length > 0)
                            {
                                mailTolst += "; ";
                            }
                            mailTolst += empRes[0].EmailAddress;
                        }
                    }
                }
                else
                {
                    mailTolst = strMailTo;
                }
                //End Modify

                //Add by Jutarat A. on 19072013
                string strAuditDetailHistory = string.Empty;
                CTS380_ScreenParameter param = GetScreenObject<CTS380_ScreenParameter>();
                if (param.ScreenData.ARDetail != null)
                {
                    strAuditDetailHistory = param.ScreenData.ARDetail.AuditDetailHistoryWithUpdate;
                    if (String.IsNullOrEmpty(strAuditDetailHistory) == false)
                    {
                        strAuditDetailHistory = strAuditDetailHistory.Replace("<span class=\"historyTxt\">", "").Replace("<span class=\"historyTxtIndent\">", "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;").Replace("</span>", "");
                    }
                }
                //End Add

                if (markLst.Count() > 0)
                {
                    var arLinkEN = CommonUtil.GenerateCompleteURL("Contract", "CTS380", String.Format("pRequestNo={0}", arDat.dtAR.RequestNo), CommonValue.DEFAULT_SHORT_LANGUAGE_EN);
                    var arLinkLC = CommonUtil.GenerateCompleteURL("Contract", "CTS380", String.Format("pRequestNo={0}", arDat.dtAR.RequestNo), CommonValue.DEFAULT_SHORT_LANGUAGE_LC); //Add by Jutarat A. on 28092012

                    arhandler.SendAREmail(mailTolst, arStatus, arLinkEN, arLinkLC, ARData, strAuditDetailHistory); //Add arLinkLC,ARData,strAuditDetailHistory by Jutarat A. on 28092012
                }
            }
            catch (Exception ex)
            {
                resEx = ex;
            }

            return resEx;
        }


        private ObjectResultData ValidateEntryData_CTS380(ObjectResultData res, CTS380_ScreenParameter sParam, CTS380_EntryData regisObj)
        {
            try
            {
                List<string> controlLst = new List<string>()
                , labelList = new List<string>();

                if (String.IsNullOrEmpty(regisObj.StatusAfterUpdate))
                {
                    controlLst.Add("IncidentStatusAfterUpdate");
                    labelList.Add("lblStatusAfterUpdate");
                }

                //Add by Jutarat A. on 24082012
                if (sParam.ScreenData.CanSpecialViewEdit
                    && (sParam.ScreenData.ARDetail.ARStatus == ARStatus.C_AR_STATUS_APPROVED
                        || sParam.ScreenData.ARDetail.ARStatus == ARStatus.C_AR_STATUS_INSTRUCTED
                        || sParam.ScreenData.ARDetail.ARStatus == ARStatus.C_AR_STATUS_REJECTED))
                {
                    //No validate
                }
                //End Add
                else
                {
                    if (String.IsNullOrEmpty(regisObj.InteractionType))
                    {
                        controlLst.Add("InteractionType");
                        labelList.Add("lblInteractionType");
                    }

                    //Comment by Jutarat A. on 03092012
                    //if (regisObj.DueDate_DeadlineType == "1")
                    //{
                    //    if (!regisObj.DueDate_Date.HasValue || !regisObj.DueDate_Time.HasValue)
                    //    {
                    //        if (!regisObj.DueDate_Date.HasValue)
                    //            controlLst.Add("DueDate_Date");

                    //        if (!regisObj.DueDate_Time.HasValue)
                    //            controlLst.Add("DueDate_Time");

                    //        labelList.Add("lblDueDate_Date");
                    //    }
                    //}
                    //else if (regisObj.DueDate_DeadlineType == "2")
                    //{
                    //    if (!regisObj.Deadline_Date.HasValue || String.IsNullOrEmpty(regisObj.Deadline_Until))
                    //    {
                    //        if (!regisObj.Deadline_Date.HasValue)
                    //            controlLst.Add("Deadline_Date");

                    //        if (String.IsNullOrEmpty(regisObj.Deadline_Until))
                    //            controlLst.Add("Deadline_Until");

                    //        labelList.Add("lblDeadline_Date");
                    //    }
                    //}
                    //End Comment
                }

                if (labelList.Count > 0)
                {
                    var labelTextList = new List<string>();

                    foreach (var lblName in labelList)
                    {
                        labelTextList.Add(CommonUtil.GetLabelFromResource("Contract", "CTS380", lblName));
                    }

                    res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0007, new string[] { CommonUtil.TextList(labelTextList.ToArray()) }, controlLst.ToArray());
                }

                if (res.IsError)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return res;
                }

                //Add by Jutarat A. on 30082012
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

                //Validate business
                var approverLst = (from a in regisObj.ARRole where a.ARRoleCode == ARRole.C_AR_ROLE_APPROVER select a).ToList();

                //Cannot register only auditor, must register Approver at least
                var auditorLst = (from a in regisObj.ARRole where a.ARRoleCode == ARRole.C_AR_ROLE_AUDITOR select a).ToList();
                if (auditorLst != null && auditorLst.Count > 0)
                {
                    if (approverLst != null && approverLst.Count == 0)
                    {
                        res.AddErrorMessage("Contract", MessageUtil.MessageList.MSG3301, null, new string[] { "ARRoleCode" });
                        return res;
                    }
                }

                // This business is existing in register new AR
                if (approverLst != null && approverLst.Count == 0)
                {
                    res.AddErrorMessage("Contract", MessageUtil.MessageList.MSG3149, null, new string[] { "ARRoleCode" });
                    return res;
                }

                //Cannot register by 'Audit request', must register Auditor at least
                if (regisObj.StatusAfterUpdate == ARStatus.C_AR_STATUS_AUDITING)
                {
                    var auditLst = from a in regisObj.ARRole where a.ARRoleCode == ARRole.C_AR_ROLE_AUDITOR select a;
                    if (auditLst != null && auditLst.Count() == 0)
                    {
                        res.AddErrorMessage("Contract", MessageUtil.MessageList.MSG3303, null, new string[] { "ARRoleCode" });
                        return res;
                    }
                }
                //End Add

                if (regisObj.InteractionType == ARInteractionType.C_AR_INTERACTION_TYPE_APPROVE)
                {
                    if (approverLst == null || approverLst.Count <= 0 || approverLst[0].EmpNo != CommonUtil.dsTransData.dtUserData.EmpNo)
                    {
                        res.AddErrorMessage("Contract", MessageUtil.MessageList.MSG3312, null, new string[] { "InteractionType" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return res;
                    }
                }

                //Add by Jutarat A. on 06092012
                //Validate warning
                //Check specify e-mail address of person incharge
                IEmployeeMasterHandler emphandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                List<dtEmployee> lstEmployee = emphandler.GetEmployee(null, null, null);

                StringBuilder sbMailEmpName = new StringBuilder();
                StringBuilder sbMailTo = new StringBuilder();
                foreach (var item in regisObj.ARRole)
                {
                    if (item.SendMail)
                    {
                        List<dtEmployee> emp = (from t in lstEmployee where t.EmpNo == item.EmpNo select t).ToList<dtEmployee>();
                        if (emp == null || emp.Count == 0 || String.IsNullOrEmpty(emp[0].EmailAddress))
                        {
                            sbMailEmpName.AppendFormat("{0}, ", item.EmployeeName);
                        }
                        else
                        {
                            sbMailTo.AppendFormat("{0}; ", emp[0].EmailAddress);
                        }
                    }
                }

                if (sbMailEmpName.Length > 1)
                {
                    string strMailEmpName = sbMailEmpName.ToString().Substring(0, sbMailEmpName.Length - 2);
                    if (String.IsNullOrEmpty(strMailEmpName) == false)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3305, new string[] { strMailEmpName }, null);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                    }
                }

                if (sbMailTo.Length > 1)
                {
                    string strMailTo = sbMailTo.ToString().Substring(0, sbMailTo.Length - 2);
                    if (String.IsNullOrEmpty(strMailTo) == false)
                    {
                        sParam.MailTo = strMailTo;
                    }
                }
                //End Add

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return res;
        }

        /// <summary>
        /// Validate system suspend, permission, and parameter
        /// </summary>
        /// <param name="res"></param>
        /// <param name="pRequestNo"></param>
        /// <returns></returns>
        private ObjectResultData ValidateAuthority_CTS380(ObjectResultData res, string pRequestNo, CTS380_ScreenParameter param = null)
        {
            IARHandler arhandler = ServiceContainer.GetService<IARHandler>() as IARHandler;

            if (CheckIsSuspending(res))
            {
                res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0049, null, null);
                return res;
            }

            if (!CheckUserPermission(ScreenID.C_SCREEN_ID_VIEW_EDIT_AR))
            {
                res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0053, null, null);
                return res;
            }

            if (string.IsNullOrEmpty(pRequestNo))
            {
                // Not Valid
                res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0011, new string[] { "Request No." }, null);
                return res;
            }

            var arDat = arhandler.GetARDetail(pRequestNo);
            if ((arDat == null) || (arDat.dtAR == null))
            {
                // Not Found
                res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0011, new string[] { String.Format("Request No.: {0}", pRequestNo) }, null);
                return res;
            }

            //Modify by Jutarat A. on 05102012
            //var arPermit = arhandler.HasARPermission(pRequestNo);
            //if (!arPermit.ViewARDetailFlag)
            //{
            //    res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0053, null, null);
            //    return res;
            //}
            if (param != null)
            {
                bool bSpecialViewEdit = false;
                bool bEdit = false;

                if (CheckUserPermission(ScreenID.C_SCREEN_ID_VIEW_EDIT_AR, FunctionID.C_FUNC_ID_SPECIAL_VIEW_EDIT_AR))
                {
                    bSpecialViewEdit = true;
                }

                if (CheckUserPermission(ScreenID.C_SCREEN_ID_VIEW_EDIT_AR, FunctionID.C_FUNC_ID_EDIT))
                {
                    bEdit = true;
                }

                var arPermit = arhandler.HasARPermission(pRequestNo);
                //if (arPermit.ViewARDetailFlag == false && bSpecialViewEdit == false && bEdit == false)
                if (arPermit.ViewARDetailFlag == false && arPermit.EditARDetailFlag == false && bSpecialViewEdit == false) //Modify by Jutarat A. on 12102012
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053, null, null);
                }

                if (param.ScreenData == null)
                    param.ScreenData = new CTS380_ScreenData();

                param.ScreenData.IsSpecialView = bSpecialViewEdit;
                param.ScreenData.IsEditable = bEdit;
                param.ScreenData.ARPermissionData = arPermit;
            }
            //End Modify

            return res;
        }

        //Add by Jutarat A. on 26042013
        /// <summary>
        /// Set Enable Fee Adjustment Alarm and Sale
        /// </summary>
        /// <param name="arData"></param>
        /// <param name="isEnableAlarm"></param>
        /// <param name="isEnableSale"></param>
        private void SetEnableFeeAdjustment_CTS350(dtAR arData, ref bool isEnableAlarm, ref bool isEnableSale)
        {
            IARHandler arhandler = ServiceContainer.GetService<IARHandler>() as IARHandler;
            IQuotationHandler quotehandler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;

            try
            {
                if (arData == null || (String.IsNullOrEmpty(arData.ARTitleType)) || (String.IsNullOrEmpty(arData.ARType)))
                {
                    isEnableAlarm = false;
                    isEnableSale = false;
                }
                else
                {
                    var curType = arhandler.GetTbs_ARTypeTitle(arData.ARType, arData.ARTitleType);
                    if ((curType != null) && (curType[0].FeeAdjustmentFlag.GetValueOrDefault() == FlagType.C_FLAG_ON))
                    {
                        if (arData.ARRelavantType == ARRelevant.C_AR_RELEVANT_TYPE_CONTRACT)
                        {
                            if (arData.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                            {
                                isEnableAlarm = true;
                                isEnableSale = false;
                            }
                            else if (arData.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                            {
                                isEnableAlarm = false;
                                isEnableSale = true;
                            }
                            else
                            {
                                isEnableAlarm = true;
                                isEnableSale = true;
                            }
                        }
                        else if (arData.ARRelavantType == ARRelevant.C_AR_RELEVANT_TYPE_QUOTATION)
                        {
                            var quoteItem = quotehandler.GetTbt_QuotationTarget(new doGetQuotationDataCondition()
                            {
                                QuotationTargetCode = arData.QuotationTargetCode
                            });

                            if (quoteItem.Count == 1)
                            {
                                if (quoteItem[0].ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                                {
                                    isEnableAlarm = true;
                                    isEnableSale = false;
                                }
                                else if (quoteItem[0].ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                                {
                                    isEnableAlarm = false;
                                    isEnableSale = true;
                                }
                                else
                                {
                                    isEnableAlarm = true;
                                    isEnableSale = true;
                                }
                            }
                            else
                            {
                                isEnableAlarm = true;
                                isEnableSale = true;
                            }
                        }
                        else
                        {
                            isEnableAlarm = true;
                            isEnableSale = true;
                        }
                    }
                    else
                    {
                        isEnableAlarm = false;
                        isEnableSale = false;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //End Add

        #endregion
    }
}
