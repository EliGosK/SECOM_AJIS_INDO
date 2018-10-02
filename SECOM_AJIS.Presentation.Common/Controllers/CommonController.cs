//*********************************
// Create by: 
// Create date: /Jun/2010
// Update date: /Jun/2010
//*********************************


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Xml;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using CSI.WindsorHelper;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Models.EmailTemplates;
using SECOM_AJIS.Presentation.Common.Helpers;

namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {
        public ActionResult TestUpload_Authority(TestCommon_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            return InitialScreenEnvironment<object>("TestUpload" ,param ,res);
        }
        
        private ActionResult GetChangeTypeCombo(string filter) {
            try {
                string strDisplay = "ValueDisplay";
                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>();
                if (filter == null || MiscType.C_ALL_CHANGE_TYPE.Equals(filter)) {
                    miscs.Add(
                        new doMiscTypeCode() {
                            FieldName = MiscType.C_RENTAL_CHANGE_TYPE,
                            ValueCode = "%"
                        });
                    miscs.Add(
                        new doMiscTypeCode() {
                            FieldName = MiscType.C_SALE_CHANGE_TYPE,
                            ValueCode = "%"
                        });
                } else {
                    miscs.Add(
                        new doMiscTypeCode() {
                            FieldName = filter,
                            ValueCode = "%"
                        });
                }

                List<doMiscTypeCode> MiscLock = hand.GetMiscTypeCodeList(miscs);

                foreach (doMiscTypeCode i in MiscLock)
                    i.ValueDisplay = i.ValueCode + ':' + i.ValueDisplay;

                //MessageModel select = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0113);
                string strSelect = CommonUtil.GetLabelFromResource("Common", "CMS030", "lblComboboxSelect");

                doMiscTypeCode first = new doMiscTypeCode();
                first.ValueCode = "";
                first.ValueDisplay = strSelect;
                MiscLock.Insert(0, first);

                ComboBoxModel cboModel = new ComboBoxModel();
                cboModel.SetList<doMiscTypeCode>(MiscLock, strDisplay, "ValueCode", false);

                return Json(cboModel);
            } catch (Exception ex) {
               // return Json(MessageUtil.GetMessage(ex));
                return null;
            }
        }

        private ActionResult GetChangeTypeFirstElementAllCombo(string filter)
        {
            try
            {
                string strDisplay = "ValueDisplay";
                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>();
                if (filter == null || MiscType.C_ALL_CHANGE_TYPE.Equals(filter))
                {
                    miscs.Add(
                        new doMiscTypeCode()
                        {
                            FieldName = MiscType.C_RENTAL_CHANGE_TYPE,
                            ValueCode = "%"
                        });
                    miscs.Add(
                        new doMiscTypeCode()
                        {
                            FieldName = MiscType.C_SALE_CHANGE_TYPE,
                            ValueCode = "%"
                        });
                }
                else
                {
                    miscs.Add(
                        new doMiscTypeCode()
                        {
                            FieldName = filter,
                            ValueCode = "%"
                        });
                }

                List<doMiscTypeCode> MiscLock = hand.GetMiscTypeCodeList(miscs);

                foreach (doMiscTypeCode i in MiscLock)
                    i.ValueDisplay = i.ValueCode + ':' + i.ValueDisplay;

                //MessageModel select = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0113);
                string strSelect = CommonUtil.GetLabelFromResource("Common", "CMS030", "lblComboboxSelect");

                //doMiscTypeCode first = new doMiscTypeCode();
                //first.ValueCode = "";
                //first.ValueDisplay = strSelect;
                //MiscLock.Insert(0, first);

                ComboBoxModel cboModel = new ComboBoxModel();
                cboModel.SetList<doMiscTypeCode>(MiscLock, strDisplay, "ValueCode", true,CommonUtil.eFirstElementType.All);

                return Json(cboModel);
            }
            catch (Exception ex)
            {
                // return Json(MessageUtil.GetMessage(ex));
                return null;
            }
        }

        [HttpPost]
        public ActionResult GetAllChangeTypeCombo(string filter)
        {
            return this.GetChangeTypeCombo(MiscType.C_ALL_CHANGE_TYPE);
        }
        [HttpPost]
        public ActionResult GetRentalChangeTypeCombo(string filter)
        {
            return this.GetChangeTypeCombo(MiscType.C_RENTAL_CHANGE_TYPE);
        }
        [HttpPost]
        public ActionResult GetSaleChangeTypeCombo(string filter)
        {
            return this.GetChangeTypeCombo(MiscType.C_SALE_CHANGE_TYPE);
        }
        [HttpPost]
        public ActionResult GetAllChangeTypeFirstElementAllCombo(string filter)
        {
            return this.GetChangeTypeFirstElementAllCombo(MiscType.C_ALL_CHANGE_TYPE);
        }
        [HttpPost]
        public ActionResult GetRentalChangeTypeFirstElementAllCombo(string filter)
        {
            return this.GetChangeTypeFirstElementAllCombo(MiscType.C_RENTAL_CHANGE_TYPE);
        }
        [HttpPost]
        public ActionResult GetSaleChangeTypeFirstElementAllCombo(string filter)
        {
            return this.GetChangeTypeFirstElementAllCombo(MiscType.C_SALE_CHANGE_TYPE);
        }
        [HttpPost]
        public ActionResult GetPersonInChargeCombo(string filter) {
            filter = filter == "" ? null : filter;
            try {
                List<dtBelongingEmpNo> result = new List<dtBelongingEmpNo>();
                string strDisplayName = "EmpValueCode";
                try {
                    if (filter != null) {
                        IEmployeeMasterHandler handler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                        result = handler.GetBelongingEmpNoByOffice(filter);
                        CommonUtil.MappingObjectLanguage<dtBelongingEmpNo>(result);
                    }
                } catch {
                    result = new List<dtBelongingEmpNo>();
                }

                ComboBoxModel cboModel = new ComboBoxModel();
                cboModel.SetList<dtBelongingEmpNo>(result, strDisplayName, "EmpNo");

                return Json(cboModel);
            } catch (Exception ex) {
                return null;
            }
        }

        [HttpPost]
        public ActionResult GetPersonInChargeFirstElementAllCombo(string filter)
        {
            filter = filter == "" ? null : filter;
            try
            {
                List<dtBelongingEmpNo> result = new List<dtBelongingEmpNo>();
                string strDisplayName = "EmpValueCode";
                try
                {
                    if (filter != null)
                    {
                        IEmployeeMasterHandler handler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                        result = handler.GetBelongingEmpNoByOffice(filter);
                        CommonUtil.MappingObjectLanguage<dtBelongingEmpNo>(result);
                    }
                }
                catch
                {
                    result = new List<dtBelongingEmpNo>();
                }

                ComboBoxModel cboModel = new ComboBoxModel();
                cboModel.SetList<dtBelongingEmpNo>(result, strDisplayName, "EmpNo",true,CommonUtil.eFirstElementType.All);

                return Json(cboModel);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public ActionResult TestGenDocReport()
        {
            doDocumentDataGenerate doc = new doDocumentDataGenerate();
            doc.DocumentNo = DateTime.Now.Millisecond.ToString();
            doc.DocumentCode = "BLR010";
            IDocumentHandler handler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
            Stream stream = handler.GenerateDocument(doc);
            
            return File(stream, "application/pdf"); 
        }
        public ActionResult TestGetDocReport()
        {

            IDocumentHandler handler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
            List<tbt_DocumentReports> list = handler.GetDocumentReportsList("570","001", "BLR010");
            Stream stream=null;
            if (list.Count > 0)
            {
                stream = new MemoryStream(list[0].FileBinary);
            }
            return File(stream, "application/pdf");
        }
        [Initialize("TestUpload")]
        public ActionResult TestUpload()
        {
            return View();
        }
        
        public ActionResult Upload()
        {
            ICommonHandler handler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<tbt_AttachFile> list = handler.GetAttachFile(Session.SessionID);

            ViewBag.AttachFileList = list;
            return View();
        }
        public ActionResult DeleteFile(int id)
        {
            ICommonHandler handler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            handler.DeleteAttachFileByID(id, Session.SessionID);
            List<tbt_AttachFile> list = handler.GetAttachFile(Session.SessionID);
            ViewBag.AttachFileList = list;
            return View("Upload");
        }
        public ActionResult CopyAttachFile(string newId="TestCopy")
        {
            ICommonHandler handler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            handler.CopyAttachFile(AttachmentModule.Contract, Session.SessionID, newId);
            List<tbt_AttachFile> list = handler.GetAttachFile(Session.SessionID);

            ViewBag.AttachFileList = list;
            return View("Upload");
        }
        public ActionResult UpdateFlag()
        {
            ICommonHandler handler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            handler.UpdateFlagAttachFile(AttachmentModule.Project, Session.SessionID, "Test");
            return View("Upload");
        }

        public ActionResult CancelUpload()
        {
            ICommonHandler handler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            handler.ClearTemporaryUploadFile(Session.SessionID);
            return View("Upload");
        }

        public ActionResult TestUploadFile(HttpPostedFileBase uploadedFile,string TempRelatedID)
        {
            if (uploadedFile!=null)
            {
                int fileSize = uploadedFile.ContentLength;
             
                    byte[] data;
                    using (BinaryReader reader = new BinaryReader(uploadedFile.InputStream))
                    {
                        data = reader.ReadBytes(uploadedFile.ContentLength);
                    }
                    string fileType = Path.GetExtension(uploadedFile.FileName);

                    ICommonHandler handler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    List<tbt_AttachFile> attach= handler.InsertAttachFile(TempRelatedID,
                                                                          uploadedFile.FileName , 
                                                                          fileType,
                                                                          fileSize,
                                                                          data, 
                                                                          false);
                    
                    List<tbt_AttachFile> list=handler.GetAttachFile(TempRelatedID);

                    ViewBag.AttachFileList=list;
                
                
                

            }
            return View("Upload");
        }

        public ActionResult TestSendMail_Authority(TestCommon_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            return InitialScreenEnvironment<object>("TestSendMail", param, res);
        }

        [Initialize("TestSendMail")]
        public ActionResult TestSendMail()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                ICommonHandler handler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                //doEmailProcess dtEmail = new doEmailProcess();
                ////dtEmail.MailFrom = "narupon@csithai.com";
                ////dtEmail.MailFromAlias = "Narupon";
                //dtEmail.MailTo = "anusas@csithai.com ; phoomsak@csithai.com ; boomzat@gmail.com ; narupon@csithai.com ; secomajis@gmail.com  ; csidev1@secom.co.th";
                //dtEmail.Message = "This is test send email process from SECOM" + Environment.NewLine + Environment.NewLine
                //                + "ทดสอบการส่งอีกเมลจาก SECOM" + Environment.NewLine + Environment.NewLine
                //                + "これは、テストがセコムからのメール送信処理です。" + Environment.NewLine + Environment.NewLine 
                //                + "Time stamp :" + DateTime.Now.ToString()
                //                ;

                //dtEmail.Subject = "SECOM-AJIS Test send mail";

                //handler.SendMail(dtEmail);


                string strEmailTemplateName = EmailTemplateName.C_EMAIL_TEMPLATE_NAME_AR_RETURNED;

                EmailTemplateUtil mailUtil = new EmailTemplateUtil(strEmailTemplateName);
                
                doEmailWithURL templateObj = new doEmailWithURL();
                templateObj.ViewURL = "-LinkEN-";
                templateObj.ViewURLLC = "-LinkLC-";

                templateObj.ARRelatedCode = "-ARRelatedCode-";
                templateObj.ARRequestNo = "-ARRequestNo-";
                templateObj.ARTypeEN = "-ARTypeEN-";
                templateObj.ARTypeLC = "-ARTypeLC-";
                templateObj.ARTitleEN = "-ARTitleEN-";
                templateObj.ARTitleLC = "-ARTitleLC-";
                templateObj.ARSubtitle = "-ARSubtitle-";
                templateObj.ARPurpose = "-ARPurpose-";

                var mailTemplate = mailUtil.LoadTemplate(templateObj);

                doEmailProcess mailMsg = new doEmailProcess();
                mailMsg.MailFrom = "jutarat@csithai.com";
                mailMsg.MailFromAlias = "Jutarat";
                mailMsg.MailTo = "jutarat@csithai.com";
                mailMsg.Subject = mailTemplate.TemplateSubject;
                mailMsg.Message = mailTemplate.TemplateContent;

                handler.SendMail(mailMsg);


                ViewBag.SendEmailResult = "Send e-mail is completed.";

            }
            catch (Exception ex)
            {
                
                res.AddErrorMessage(ex);
                ViewBag.SendEmailResult = "Send e-mail is failed.";
                
            }

            return View(); 
        }

        
    }


    public class TestCommon_ScreenParameter : ScreenParameter
    {

    }
}
