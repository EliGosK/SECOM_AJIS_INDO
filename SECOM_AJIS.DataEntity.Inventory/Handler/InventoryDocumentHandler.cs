
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Installation;
using CSI.WindsorHelper;
using System.Transactions;
using SECOM_AJIS.DataEntity.Quotation;
using System.IO;
using SpreadsheetLight;
using System.Drawing;


namespace SECOM_AJIS.DataEntity.Inventory
{
    partial class InventoryDocumentHandler : BizIVDataEntities, IInventoryDocumentHandler
    {
        #region Get data for generate report

        /// <summary>
        /// Getting data for create report IVR010 - IVR090, IVR120, IVR130, IVR180
        /// </summary>
        /// <param name="strInventorySlipNo"></param>
        /// <returns></returns>
        public List<doIVR> GetIVR(string strInventorySlipNo)
        {
            return base.GetIVR(MiscType.C_INV_STOCKIN_TYPE, MiscType.C_INV_LOC, MiscType.C_INV_AREA, strInventorySlipNo);
        }

        /// <summary>
        /// Getting data for create report IVR100.
        /// </summary>
        /// <param name="strInventorySlipNo"></param>
        /// <returns></returns>
        public List<doIVR100> GetIVR100(string strInventorySlipNo)
        {
            return base.GetIVR100(MiscType.C_INV_LOC, MiscType.C_INV_AREA, ConfigName.C_CONFIG_SUSPEND_FLAG, strInventorySlipNo);
        }

        /// <summary>
        /// Getting data for create report IVR110.
        /// </summary>
        /// <param name="strInventorySlipNo"></param>
        /// <returns></returns>
        public List<doIVR110> GetIVR110(string strInventorySlipNo)
        {
            return base.GetIVR110(MiscType.C_INV_LOC, MiscType.C_INV_AREA, ConfigName.C_CONFIG_SUSPEND_FLAG, strInventorySlipNo);
        }


        #endregion

        #region Geneate report

        // IVR010-Stock-in Slip (MK-02)
        public Stream GenerateIVR010(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime)
        {
            string strFilePath = GenerateIVR010FilePath(strInventorySlipNo, strInventorySlipIssueOffice, strEmpNo, dtDateTime);
            IDocumentHandler handlerDocument = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            return handlerDocument.GetDocumentReportFileStream(strFilePath);
        }
        #region IVR010 Deprecated, User IVR010B instead.
        //public string GenerateIVR010FilePath(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime)
        //{

        //    List<doIVR> ivr010 = base.GetIVR(MiscType.C_INV_STOCKIN_TYPE, MiscType.C_INV_LOC, MiscType.C_INV_AREA, strInventorySlipNo);

        //    if (ivr010.Count == 0)
        //    {
        //        return null;
        //    } //end if

        //    doDocumentDataGenerate doDocument = new doDocumentDataGenerate();
        //    doDocument.DocumentNo = strInventorySlipNo;
        //    doDocument.DocumentCode = ReportID.C_INV_REPORT_ID_STOCKIN;
        //    doDocument.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
        //    doDocument.DocumentData = ivr010;
        //    doDocument.OtherKey.InventorySlipIssueOffice = ivr010[0].DestinationOfficeCode;

        //    // Additional
        //    doDocument.EmpNo = strEmpNo;
        //    doDocument.ProcessDateTime = dtDateTime;

        //    string Memo = "";
        //    if (ivr010[0].Memo != null)
        //    {
        //        Memo = ivr010[0].Memo;
        //    }
        //    List<ReportParameterObject> listMainReportParam = new List<ReportParameterObject>();
        //    listMainReportParam.Add(new ReportParameterObject() { ParameterName = "SlipNo", Value = ivr010[0].SlipNo });
        //    listMainReportParam.Add(new ReportParameterObject() { ParameterName = "StockInType", Value = ivr010[0].StockInType });
        //    listMainReportParam.Add(new ReportParameterObject() { ParameterName = "MonthYearReport", Value = "(" + DateTime.Now.ToString("MMM. yyyy") + ")" });
        //    listMainReportParam.Add(new ReportParameterObject() { ParameterName = "Memo", Value = Memo });
        //    doDocument.MainReportParam = listMainReportParam;

        //    IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
        //    string strFilePath = "";

        //    strFilePath = documentHandler.GenerateDocumentFilePath(doDocument);

        //    return strFilePath;
        //}
        #endregion
        public string GenerateIVR010FilePath(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime)
        {

            var ivr011 = base.GetIVR010(
                MiscType.C_INV_STOCKIN_TYPE,
                MiscType.C_INV_LOC,
                MiscType.C_INV_AREA,
                MiscType.C_PURCHASE_ORDER_STATUS,
                MiscType.C_TRANSPORT_TYPE,
                MiscType.C_CURRENCY_TYPE,
                strInventorySlipNo
            );

            if (ivr011.Count == 0)
            {
                return null;
            } //end if

            doDocumentDataGenerate doDocument = new doDocumentDataGenerate();
            doDocument.DocumentNo = strInventorySlipNo;
            doDocument.DocumentCode = ReportID.C_INV_REPORT_ID_STOCKIN;
            doDocument.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
            doDocument.DocumentData = ivr011;
            doDocument.OtherKey.InventorySlipIssueOffice = ivr011[0].DestinationOfficeCode;

            // Additional
            doDocument.EmpNo = strEmpNo;
            doDocument.ProcessDateTime = dtDateTime;

            string Memo = "";
            if (ivr011[0].Memo != null)
            {
                Memo = ivr011[0].Memo;
            }
            List<ReportParameterObject> listMainReportParam = new List<ReportParameterObject>();
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "SlipNo", Value = ivr011[0].SlipNo });
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "StockInType", Value = ivr011[0].StockInType });
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "MonthYearReport", Value = "(" + DateTime.Now.ToString("MMM. yyyy") + ")" });
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "Memo", Value = Memo });
            doDocument.MainReportParam = listMainReportParam;

            IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
            string strFilePath = "";

            strFilePath = documentHandler.GenerateDocumentFilePath(doDocument);

            return strFilePath;
        }

        // IVR020-Expected elimination slip (MK-98)
        public Stream GenerateIVR020(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime)
        {
            string strFilePath = GenerateIVR020FilePath(strInventorySlipNo, strInventorySlipIssueOffice, strEmpNo, dtDateTime);
            IDocumentHandler handlerDocument = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            return handlerDocument.GetDocumentReportFileStream(strFilePath);

        }
        public string GenerateIVR020FilePath(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime)
        {

            List<doIVR> ivr020 = base.GetIVR(MiscType.C_INV_STOCKIN_TYPE, MiscType.C_INV_LOC, MiscType.C_INV_AREA, strInventorySlipNo);

            if (ivr020.Count == 0)
            {
                return null;
            } //end if

            doDocumentDataGenerate doDocument = new doDocumentDataGenerate();
            doDocument.DocumentNo = strInventorySlipNo;
            doDocument.DocumentCode = ReportID.C_INV_REPORT_ID_PRE_ELIMINATION;
            doDocument.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
            doDocument.DocumentData = ivr020;
            doDocument.OtherKey.InventorySlipIssueOffice = ivr020[0].SourceOfficeCode;

            // Additional
            doDocument.EmpNo = strEmpNo;
            doDocument.ProcessDateTime = dtDateTime;

            string Memo = "";
            if (ivr020[0].Memo != null)
            {
                Memo = ivr020[0].Memo;
            }

            IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
            List<ReportParameterObject> listMainReportParam = new List<ReportParameterObject>();
            List<tbm_DocumentTemplate> dLst = documentHandler.GetDocumentTemplateByDocumentCode(ReportID.C_INV_REPORT_ID_PRE_ELIMINATION);
            if (dLst.Count > 0)
            {
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "DocVersion", Value = dLst[0].DocumentVersion });
            }
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "SlipNo", Value = ivr020[0].SlipNo });
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "SlipIssueDate", Value = ivr020[0].SlipIssueDate });
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "ApproveNo", Value = ivr020[0].ApproveNo });
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "SourceLocationName", Value = ivr020[0].SourceLocationName });
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "DestLocationName", Value = ivr020[0].DestLocationName });
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "MonthYearReport", Value = "(" + DateTime.Now.ToString("MMM. yyyy") + ")" });
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "Memo", Value = Memo });
            doDocument.MainReportParam = listMainReportParam;

            string strFilePath = string.Empty;

            strFilePath = documentHandler.GenerateDocumentFilePath(doDocument);

            return strFilePath;
        }

        // IVR030-Elimination slip (MK-99)
        public Stream GenerateIVR030(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime)
        {
            string strFilePath = GenerateIVR030FilePath(strInventorySlipNo, strInventorySlipIssueOffice, strEmpNo, dtDateTime);
            IDocumentHandler handlerDocument = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            return handlerDocument.GetDocumentReportFileStream(strFilePath);

        }
        public string GenerateIVR030FilePath(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime)
        {

            List<byte[]> filesByte = new List<byte[]>();
            IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            List<doIVR> ivr030 = base.GetIVR(MiscType.C_INV_STOCKIN_TYPE, MiscType.C_INV_LOC, MiscType.C_INV_AREA, strInventorySlipNo);

            if (ivr030.Count == 0)
            {
                return null;
            } //end if

            string Memo = "";
            if (ivr030[0].Memo != null)
            {
                Memo = ivr030[0].Memo;
            }

            List<ReportParameterObject> listMainReportParam = new List<ReportParameterObject>();
            List<tbm_DocumentTemplate> dLst = documentHandler.GetDocumentTemplateByDocumentCode(ReportID.C_INV_REPORT_ID_ELIMINATION);
            if (dLst.Count > 0)
            {
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "DocVersion", Value = dLst[0].DocumentVersion });
            }
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "SlipNo", Value = ivr030[0].SlipNo });
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "SlipIssueDate", Value = ivr030[0].SlipIssueDate });
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "ApproveNo", Value = ivr030[0].ApproveNo });
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "MonthYearReport", Value = "(" + DateTime.Now.ToString("MMM. yyyy") + ")" });
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "Memo", Value = Memo });

            doDocumentDataGenerate doDocumentA = new doDocumentDataGenerate();
            doDocumentA.DocumentNo = strInventorySlipNo;
            doDocumentA.DocumentCode = ReportID.C_INV_REPORT_ID_ELIMINATION;
            doDocumentA.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
            doDocumentA.DocumentData = ivr030;
            doDocumentA.OtherKey.InventorySlipIssueOffice = ivr030[0].SourceOfficeCode;
            doDocumentA.MainReportParam = listMainReportParam;

            // Additional
            doDocumentA.EmpNo = strEmpNo;
            doDocumentA.ProcessDateTime = dtDateTime;


            doDocumentDataGenerate doDocumentB = new doDocumentDataGenerate();
            doDocumentB.DocumentNo = strInventorySlipNo;
            doDocumentB.DocumentCode = ReportID.C_INV_REPORT_ID_ELIMINATION_B;
            doDocumentB.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
            doDocumentB.DocumentData = ivr030;
            doDocumentB.OtherKey.InventorySlipIssueOffice = ivr030[0].SourceOfficeCode;
            doDocumentB.MainReportParam = listMainReportParam;

            List<doDocumentDataGenerate> slaveDoc = new List<doDocumentDataGenerate>();
            slaveDoc.Add(doDocumentB);

            string strFilePath = documentHandler.GenerateDocumentFilePath(doDocumentA, slaveDoc);
            return strFilePath;
        }

        // IVR040-Transfer between warehouse and branch slip (MK-06)
        public Stream GenerateIVR040(string strInventorySlipNo, string strEmpNo, DateTime dtDateTime)
        {
            string strFilePath = GenerateIVR040FilePath(strInventorySlipNo, strEmpNo, dtDateTime);
            IDocumentHandler handlerDocument = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            return handlerDocument.GetDocumentReportFileStream(strFilePath);

        }
        public string GenerateIVR040FilePath(string strInventorySlipNo, string strEmpNo, DateTime dtDateTime)
        {

            List<byte[]> filesByte = new List<byte[]>();
            IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            List<doIVR> ivr040 = base.GetIVR(MiscType.C_INV_STOCKIN_TYPE, MiscType.C_INV_LOC, MiscType.C_INV_AREA, strInventorySlipNo);

            if (ivr040.Count == 0)
            {
                return null;
            } //end if

            string Memo = "";
            if (ivr040[0].Memo != null)
            {
                Memo = ivr040[0].Memo;
            }

            List<ReportParameterObject> listMainReportParam = new List<ReportParameterObject>();
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "SlipNo", Value = ivr040[0].SlipNo });
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "SlipIssueDate", Value = ivr040[0].SlipIssueDate });
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "ApproveNo", Value = ivr040[0].ApproveNo });
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "SourceOfficeName", Value = ivr040[0].SourceOfficeName });
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "DestOfficeName", Value = ivr040[0].DestOfficeName });
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "MonthYearReport", Value = "(" + DateTime.Now.ToString("MMM. yyyy") + ")" });
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "Memo", Value = Memo });

            doDocumentDataGenerate doDocumentA = new doDocumentDataGenerate();
            doDocumentA.DocumentNo = strInventorySlipNo;
            doDocumentA.DocumentCode = ReportID.C_INV_REPORT_ID_TRANSFER_OFFICE;
            doDocumentA.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
            doDocumentA.DocumentData = ivr040;
            doDocumentA.OtherKey.InventorySlipIssueOffice = ivr040[0].SourceOfficeCode;
            doDocumentA.MainReportParam = listMainReportParam;

            // Additional
            doDocumentA.EmpNo = strEmpNo;
            doDocumentA.ProcessDateTime = dtDateTime;

            doDocumentDataGenerate doDocumentB = new doDocumentDataGenerate();
            doDocumentB.DocumentNo = strInventorySlipNo;
            doDocumentB.DocumentCode = ReportID.C_INV_REPORT_ID_TRANSFER_OFFICE_B;
            doDocumentB.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
            doDocumentB.DocumentData = ivr040;
            doDocumentB.OtherKey.InventorySlipIssueOffice = ivr040[0].SourceOfficeCode;
            doDocumentB.MainReportParam = listMainReportParam;

            List<doDocumentDataGenerate> slaveDoc = new List<doDocumentDataGenerate>();
            slaveDoc.Add(doDocumentB);

            string strFilePath = documentHandler.GenerateDocumentFilePath(doDocumentA, slaveDoc);
            return strFilePath;
        }

        // IVR050-Transfer instrument within warehouse slip (MK-50)
        public Stream GenerateIVR050(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime)
        {
            string strFilePath = GenerateIVR050FilePath(strInventorySlipNo, strInventorySlipIssueOffice, strEmpNo, dtDateTime);
            IDocumentHandler handlerDocument = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            return handlerDocument.GetDocumentReportFileStream(strFilePath);

        }
        public string GenerateIVR050FilePath(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime)
        {

            List<doIVR> ivr050 = base.GetIVR(MiscType.C_INV_STOCKIN_TYPE, MiscType.C_INV_LOC, MiscType.C_INV_AREA, strInventorySlipNo);

            if (ivr050.Count == 0)
            {
                return null;
            } //end if

            doDocumentDataGenerate doDocument = new doDocumentDataGenerate();
            doDocument.DocumentNo = strInventorySlipNo;
            doDocument.DocumentCode = ReportID.C_INV_REPORT_ID_TRANSFER_WITHIN_WH;
            doDocument.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
            doDocument.DocumentData = ivr050;
            doDocument.OtherKey.InventorySlipIssueOffice = ivr050[0].SourceOfficeCode;

            string Memo = "";
            if (ivr050[0].Memo != null)
            {
                Memo = ivr050[0].Memo;
            }

            List<ReportParameterObject> listMainReportParam = new List<ReportParameterObject>();
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "SlipNo", Value = ivr050[0].SlipNo });
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "SlipIssueDate", Value = ivr050[0].SlipIssueDate });
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "ApproveNo", Value = ivr050[0].ApproveNo });
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "SourceOfficeName", Value = ivr050[0].SourceOfficeName });
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "SourceAreaName", Value = ivr050[0].SourceAreaName });
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "DestAreaName", Value = ivr050[0].DestAreaName });
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "MonthYearReport", Value = "(" + DateTime.Now.ToString("MMM. yyyy") + ")" });
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "Memo", Value = Memo });
            doDocument.MainReportParam = listMainReportParam;

            // Additional
            doDocument.EmpNo = strEmpNo;
            doDocument.ProcessDateTime = dtDateTime;

            IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
            string strFilePath = null;

            strFilePath = documentHandler.GenerateDocumentFilePath(doDocument);

            return strFilePath;
        }

        // IVR060-Special stock-out slip (MK-19)
        public Stream GenerateIVR060(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime)
        {
            string strFilePath = GenerateIVR060FilePath(strInventorySlipNo, strInventorySlipIssueOffice, strEmpNo, dtDateTime);
            IDocumentHandler handlerDocument = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            return handlerDocument.GetDocumentReportFileStream(strFilePath);

        }
        public string GenerateIVR060FilePath(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime)
        {

            List<byte[]> filesByte = new List<byte[]>();
            IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            List<doIVR> ivr060 = base.GetIVR(MiscType.C_INV_STOCKIN_TYPE, MiscType.C_INV_LOC, MiscType.C_INV_AREA, strInventorySlipNo);

            if (ivr060.Count == 0)
            {
                return null;
            } //end if

            string Memo = "";
            if (ivr060[0].Memo != null)
            {
                Memo = ivr060[0].Memo;
            }

            List<ReportParameterObject> listMainReportParam = new List<ReportParameterObject>();
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "SlipNo", Value = ivr060[0].SlipNo });
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "SlipIssueDate", Value = ivr060[0].SlipIssueDate });
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "ApproveNo", Value = ivr060[0].ApproveNo });
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "MonthYearReport", Value = "(" + DateTime.Now.ToString("MMM. yyyy") + ")" });
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "Memo", Value = Memo });

            doDocumentDataGenerate doDocumentA = new doDocumentDataGenerate();
            doDocumentA.DocumentNo = strInventorySlipNo;
            doDocumentA.DocumentCode = ReportID.C_INV_REPORT_ID_SPECIAL_STOCKOUT;
            doDocumentA.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
            doDocumentA.DocumentData = ivr060;
            doDocumentA.OtherKey.InventorySlipIssueOffice = ivr060[0].SourceOfficeCode;
            doDocumentA.MainReportParam = listMainReportParam;

            // Additional
            doDocumentA.EmpNo = strEmpNo;
            doDocumentA.ProcessDateTime = dtDateTime;

            doDocumentDataGenerate doDocumentB = new doDocumentDataGenerate();
            doDocumentB.DocumentNo = strInventorySlipNo;
            doDocumentB.DocumentCode = ReportID.C_INV_REPORT_ID_SPECIAL_STOCKOUT_B;
            doDocumentB.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
            doDocumentB.DocumentData = ivr060;
            doDocumentB.OtherKey.InventorySlipIssueOffice = ivr060[0].SourceOfficeCode;
            doDocumentB.MainReportParam = listMainReportParam;

            List<doDocumentDataGenerate> slaveDoc = new List<doDocumentDataGenerate>();
            slaveDoc.Add(doDocumentB);

            string strFilePath = documentHandler.GenerateDocumentFilePath(doDocumentA, slaveDoc);
            return strFilePath;
        }

        // IVR070-Repair request Slip (MK-17)
        public Stream GenerateIVR070(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime)
        {
            string strFilePath = GenerateIVR070FilePath(strInventorySlipNo, strInventorySlipIssueOffice, strEmpNo, dtDateTime);
            IDocumentHandler handlerDocument = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            return handlerDocument.GetDocumentReportFileStream(strFilePath);

        }
        public string GenerateIVR070FilePath(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime)
        {
            IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            List<doIVR> ivr070 = GetIVR(strInventorySlipNo);
            List<tbm_DocumentTemplate> dLst = documentHandler.GetDocumentTemplateByDocumentCode(ReportID.C_INV_REPORT_ID_REPAIR_REQUEST);

            if (ivr070.Count == 0)
            {
                return null;
            } //end if            

            List<ReportParameterObject> listMainReportParam = new List<ReportParameterObject>();
            if (dLst.Count > 0)
            {
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "DocVersion", Value = dLst[0].DocumentVersion });
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "TemplateName", Value = dLst[0].DocumentNameEN });
            }

            doDocumentDataGenerate doDocumentA = new doDocumentDataGenerate();
            doDocumentA.DocumentNo = strInventorySlipNo;
            doDocumentA.DocumentCode = ReportID.C_INV_REPORT_ID_REPAIR_REQUEST;
            doDocumentA.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
            doDocumentA.DocumentData = ivr070;
            doDocumentA.OtherKey.InventorySlipIssueOffice = ivr070[0].SourceOfficeCode;
            doDocumentA.MainReportParam = listMainReportParam;
            doDocumentA.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            doDocumentA.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;

            // Additional
            doDocumentA.EmpNo = strEmpNo;
            doDocumentA.ProcessDateTime = dtDateTime;

            doDocumentDataGenerate doDocumentB = new doDocumentDataGenerate();
            doDocumentB.DocumentNo = strInventorySlipNo;
            doDocumentB.DocumentCode = ReportID.C_INV_REPORT_ID_REPAIR_REQUEST_B;
            doDocumentB.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
            doDocumentB.DocumentData = ivr070;
            doDocumentB.OtherKey.InventorySlipIssueOffice = ivr070[0].SourceOfficeCode;
            doDocumentB.MainReportParam = listMainReportParam;
            doDocumentB.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            doDocumentB.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;

            List<doDocumentDataGenerate> slaveDoc = new List<doDocumentDataGenerate>();
            slaveDoc.Add(doDocumentB);

            string slipNoReportPath = documentHandler.GenerateDocumentFilePath(doDocumentA, slaveDoc);
            return slipNoReportPath;
        }

        // IVR080-Repair return Slip (MK-18)
        public Stream GenerateIVR080(string strInventorySlipNo, string strEmpNo, DateTime dtDateTime)
        {
            string strFilePath = GenerateIVR080FilePath(strInventorySlipNo, strEmpNo, dtDateTime);
            IDocumentHandler handlerDocument = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            return handlerDocument.GetDocumentReportFileStream(strFilePath);

        }
        public string GenerateIVR080FilePath(string strInventorySlipNo, string strEmpNo, DateTime dtDateTime)
        {
            IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            List<doIVR> ivr080 = base.GetIVR(MiscType.C_INV_STOCKIN_TYPE, MiscType.C_INV_LOC, MiscType.C_INV_AREA, strInventorySlipNo);
            List<tbm_DocumentTemplate> dLst = documentHandler.GetDocumentTemplateByDocumentCode(ReportID.C_INV_REPORT_ID_REPAIR_RETURN);

            if (ivr080.Count == 0)
            {
                return null;
            } //end if

            List<ReportParameterObject> listMainReportParam = new List<ReportParameterObject>();
            if (dLst.Count > 0)
            {
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "DocVersion", Value = dLst[0].DocumentVersion });
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "TemplateName", Value = dLst[0].DocumentNameEN });
            }

            doDocumentDataGenerate doDocumentA = new doDocumentDataGenerate();
            doDocumentA.DocumentNo = strInventorySlipNo;
            doDocumentA.DocumentCode = ReportID.C_INV_REPORT_ID_REPAIR_RETURN;
            doDocumentA.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
            doDocumentA.DocumentData = ivr080;
            doDocumentA.OtherKey.InventorySlipIssueOffice = ivr080[0].SourceOfficeCode;
            doDocumentA.MainReportParam = listMainReportParam;
            doDocumentA.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            doDocumentA.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;

            // Additional
            doDocumentA.EmpNo = strEmpNo;
            doDocumentA.ProcessDateTime = dtDateTime;

            doDocumentDataGenerate doDocumentB = new doDocumentDataGenerate();
            doDocumentB.DocumentNo = strInventorySlipNo;
            doDocumentB.DocumentCode = ReportID.C_INV_REPORT_ID_REPAIR_RETURN_B;
            doDocumentB.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
            doDocumentB.DocumentData = ivr080;
            doDocumentB.OtherKey.InventorySlipIssueOffice = ivr080[0].SourceOfficeCode;
            doDocumentB.MainReportParam = listMainReportParam;
            doDocumentB.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            doDocumentB.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;

            List<doDocumentDataGenerate> slaveDoc = new List<doDocumentDataGenerate>();
            slaveDoc.Add(doDocumentB);

            string slipNoReportPath = documentHandler.GenerateDocumentFilePath(doDocumentA, slaveDoc);
            return slipNoReportPath;
        }

        // IVR090-Checking returned Slip (MK-44)
        public Stream GenerateIVR090(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime)
        {
            string strFilePath = GenerateIVR090FilePath(strInventorySlipNo, strInventorySlipIssueOffice, strEmpNo, dtDateTime);
            IDocumentHandler handlerDocument = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            return handlerDocument.GetDocumentReportFileStream(strFilePath);

        }
        public string GenerateIVR090FilePath(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime)
        {
            IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            List<doIVR> ivr090 = base.GetIVR(MiscType.C_INV_STOCKIN_TYPE, MiscType.C_INV_LOC, MiscType.C_INV_AREA, strInventorySlipNo);
            List<tbm_DocumentTemplate> dLst = documentHandler.GetDocumentTemplateByDocumentCode(ReportID.C_INV_REPORT_ID_CHECKING_RETURNED);

            if (ivr090.Count == 0)
            {
                return null;
            } //end if

            doDocumentDataGenerate doDocument = new doDocumentDataGenerate();
            doDocument.DocumentNo = strInventorySlipNo;
            doDocument.DocumentCode = ReportID.C_INV_REPORT_ID_CHECKING_RETURNED;
            doDocument.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
            doDocument.DocumentData = ivr090;
            doDocument.OtherKey.InventorySlipIssueOffice = ivr090[0].SourceOfficeCode;
            doDocument.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            doDocument.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;

            // Additional
            doDocument.EmpNo = strEmpNo;
            doDocument.ProcessDateTime = dtDateTime;

            List<ReportParameterObject> listMainReportParam = new List<ReportParameterObject>();
            if (dLst.Count > 0)
            {
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "DocVersion", Value = dLst[0].DocumentVersion });
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "TemplateName", Value = dLst[0].DocumentNameEN });
            }
            doDocument.MainReportParam = listMainReportParam;

            string slipNoReportPath = documentHandler.GenerateDocumentFilePath(doDocument);
            return slipNoReportPath;
        }

        // IVR100-Checking instrument slip
        public Stream GenerateIVR100(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtProcessTime)
        {
            string strFilePath = GenerateIVR100FilePath(strInventorySlipNo, strInventorySlipIssueOffice, strEmpNo, dtProcessTime);
            IDocumentHandler handlerDocument = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            return handlerDocument.GetDocumentReportFileStream(strFilePath);

        }
        public string GenerateIVR100FilePath(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtProcessTime)
        {
            var ivr100 = GetIVR100(strInventorySlipNo);

            if (ivr100.Count == 0)
            {
                return null;
            }

            doDocumentDataGenerate doDocument = new doDocumentDataGenerate();
            doDocument.DocumentNo = strInventorySlipNo;
            doDocument.DocumentCode = ReportID.C_INV_REPORT_ID_CHECKING_INSTRUMENT;
            doDocument.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
            doDocument.DocumentData = ivr100;
            doDocument.OtherKey.InventorySlipIssueOffice = strInventorySlipIssueOffice;
            doDocument.OtherKey.MonthYear = DateTime.ParseExact(ivr100[0].CheckingYearMonth, "yyyyMM", System.Globalization.DateTimeFormatInfo.CurrentInfo);

            // Additional
            doDocument.EmpNo = strEmpNo;
            doDocument.ProcessDateTime = dtProcessTime;

            List<ReportParameterObject> listMainReportParam = new List<ReportParameterObject>();
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "MonthYearReport", Value = "(" + DateTime.Now.ToString("MMM. yyyy") + ")" });
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "RowsPerPage", Value = CommonValue.ROWS_PER_PAGE_FOR_INVENTORY_CHECKING });
            doDocument.MainReportParam = listMainReportParam;

            IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            string strFilePath = documentHandler.GenerateDocumentFilePath(doDocument);

            return strFilePath;
        }

        // IVR110-Checking instrument slip
        public Stream GenerateIVR110(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime)
        {
            string strFilePath = GenerateIVR110FilePath(strInventorySlipNo, strInventorySlipIssueOffice, strEmpNo, dtDateTime);
            IDocumentHandler handlerDocument = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            return handlerDocument.GetDocumentReportFileStream(strFilePath);

        }
        public string GenerateIVR110FilePath(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime)
        {
            var ivr110 = GetIVR110(strInventorySlipNo);

            if (ivr110.Count == 0)
            {
                return null;
            }

            doDocumentDataGenerate doDocument = new doDocumentDataGenerate();
            doDocument.DocumentNo = strInventorySlipNo;
            doDocument.DocumentCode = ReportID.C_INV_REPORT_ID_CHECKING_INSTRUMENT_RESULT;
            doDocument.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
            doDocument.DocumentData = ivr110;
            doDocument.OtherKey.InventorySlipIssueOffice = strInventorySlipIssueOffice;
            //doDocument.OtherKey.MonthYear = (ivr110[0].CheckingYearMonth ?? dtDateTime);
            doDocument.OtherKey.MonthYear = DateTime.ParseExact(ivr110[0].CheckingYearMonth, "yyyyMM", System.Globalization.DateTimeFormatInfo.CurrentInfo); //Modify by Jutarat A. on 27022013

            // Additional
            doDocument.EmpNo = strEmpNo;
            doDocument.ProcessDateTime = dtDateTime;

            List<ReportParameterObject> listMainReportParam = new List<ReportParameterObject>();
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "MonthYearReport", Value = "(" + DateTime.Now.ToString("MMM. yyyy") + ")" });
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "RowsPerPage", Value = CommonValue.ROWS_PER_PAGE_FOR_INVENTORY_CHECKING });
            doDocument.MainReportParam = listMainReportParam;

            IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            string strFilePath = documentHandler.GenerateDocumentFilePath(doDocument);

            return strFilePath;
        }

        // IVR120-Register adjust Slip (MK-91)
        public Stream GenerateIVR120(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime)
        {
            string strFilePath = GenerateIVR120FilePath(strInventorySlipNo, strInventorySlipIssueOffice, strEmpNo, dtDateTime);
            IDocumentHandler handlerDocument = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            return handlerDocument.GetDocumentReportFileStream(strFilePath);

        }
        public string GenerateIVR120FilePath(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime)
        {
            IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            List<doIVR> ivr120 = base.GetIVR(MiscType.C_INV_STOCKIN_TYPE, MiscType.C_INV_LOC, MiscType.C_INV_AREA, strInventorySlipNo);
            List<tbm_DocumentTemplate> dLst = documentHandler.GetDocumentTemplateByDocumentCode(ReportID.C_INV_REPORT_ID_TRANSFER_BUFFER);

            if (ivr120.Count == 0)
            {
                return null;
            } //end if

            doDocumentDataGenerate doDocument = new doDocumentDataGenerate();
            doDocument.DocumentNo = strInventorySlipNo;
            doDocument.DocumentCode = ReportID.C_INV_REPORT_ID_TRANSFER_BUFFER;
            doDocument.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
            doDocument.DocumentData = ivr120;
            doDocument.OtherKey.InventorySlipIssueOffice = ivr120[0].SourceOfficeCode;
            doDocument.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            doDocument.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;

            // Additional
            doDocument.EmpNo = strEmpNo;
            doDocument.ProcessDateTime = dtDateTime;

            List<ReportParameterObject> listMainReportParam = new List<ReportParameterObject>();
            if (dLst.Count > 0)
            {
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "DocVersion", Value = dLst[0].DocumentVersion });
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "TemplateName", Value = dLst[0].DocumentNameEN });
            }
            doDocument.MainReportParam = listMainReportParam;

            string slipNoReportPath = documentHandler.GenerateDocumentFilePath(doDocument);
            return slipNoReportPath;
        }

        // IVR130-Fix stock adjust Slip (MK-92)
        public Stream GenerateIVR130(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime)
        {
            string strFilePath = GenerateIVR130FilePath(strInventorySlipNo, strInventorySlipIssueOffice, strEmpNo, dtDateTime);
            IDocumentHandler handlerDocument = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            return handlerDocument.GetDocumentReportFileStream(strFilePath);

        }
        public string GenerateIVR130FilePath(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime)
        {
            IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            List<doIVR> ivr130 = base.GetIVR(MiscType.C_INV_STOCKIN_TYPE, MiscType.C_INV_LOC, MiscType.C_INV_AREA, strInventorySlipNo);
            List<tbm_DocumentTemplate> dLst = documentHandler.GetDocumentTemplateByDocumentCode(ReportID.C_INV_REPORT_ID_FIX_ADJUSTMENT);

            if (ivr130.Count == 0)
            {
                return null;
            } //end if

            doDocumentDataGenerate doDocument = new doDocumentDataGenerate();
            doDocument.DocumentNo = strInventorySlipNo;
            doDocument.DocumentCode = ReportID.C_INV_REPORT_ID_FIX_ADJUSTMENT;
            doDocument.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
            doDocument.DocumentData = ivr130;
            doDocument.OtherKey.InventorySlipIssueOffice = ivr130[0].SourceOfficeCode;
            doDocument.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            doDocument.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;

            // Additional
            doDocument.EmpNo = strEmpNo;
            doDocument.ProcessDateTime = dtDateTime;

            List<ReportParameterObject> listMainReportParam = new List<ReportParameterObject>();
            if (dLst.Count > 0)
            {
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "DocVersion", Value = dLst[0].DocumentVersion });
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "TemplateName", Value = dLst[0].DocumentNameEN });
            }
            doDocument.MainReportParam = listMainReportParam;

            string slipNoReportPath = documentHandler.GenerateDocumentFilePath(doDocument);
            return slipNoReportPath;
        }

        // IVR140 IVR141 IVR142 IVR143
        public Stream GenerateIVR140(DateTime dtReportDateTime, string strEmpNo, DateTime dtDateTime)
        {
            string strFilePath = GenerateIVR140FilePath(dtReportDateTime, strEmpNo, dtDateTime);
            IDocumentHandler handlerDocument = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            return handlerDocument.GetDocumentReportFileStream(strFilePath);

        }
        public string GenerateIVR140FilePath(DateTime dtReportDateTime, string strEmpNo, DateTime dtDateTime)
        {
            List<byte[]> filesByte = new List<byte[]>();

            List<ReportParameterObject> listMainReportParam;
            List<tbm_DocumentTemplate> dLst;
            List<ReportParameterObject> listSubReportDataSource;
            List<doDocumentDataGenerate> slaveDoc = new List<doDocumentDataGenerate>();

            IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            string monthYear = dtReportDateTime.ToString("yyyyMM");
            List<doOffice> headOffice = this.GetInventoryHeadOffice();
            List<RPTdoIVR140> ivr140 = base.GetIVR140(monthYear, MiscType.C_INV_AREA);
            if (ivr140.Count != 0)
            {

                doDocumentDataGenerate doDocument140 = new doDocumentDataGenerate();
                doDocument140.DocumentNo = monthYear;
                doDocument140.DocumentCode = ReportID.C_INV_REPORT_ID_SUMMARY_IN_OUT;
                doDocument140.DocumentTemplateCode = ReportID.C_INV_REPORT_ID_SUMMARY_IN_OUT;
                doDocument140.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
                doDocument140.DocumentData = ivr140;
                doDocument140.OtherKey.InventorySlipIssueOffice = headOffice[0].OfficeCode;
                doDocument140.OtherKey.MonthYear = dtReportDateTime;

                // Additional
                doDocument140.EmpNo = strEmpNo;
                doDocument140.ProcessDateTime = dtDateTime;

                listMainReportParam = new List<ReportParameterObject>();
                dLst = documentHandler.GetDocumentTemplateByDocumentCode(ReportID.C_INV_REPORT_ID_SUMMARY_IN_OUT);
                if (dLst.Count > 0)
                {
                    listMainReportParam.Add(new ReportParameterObject() { ParameterName = "DocVersion", Value = dLst[0].DocumentVersion });
                }
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "MonthReport", Value = "(" + dtReportDateTime.ToString("MMM. yyyy") + ")" });
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "Group", Value = "Headquarter" });
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "MonthYearReport", Value = dtReportDateTime.ToString("MMMM / yyyy") });
                doDocument140.MainReportParam = listMainReportParam;

                listSubReportDataSource = new List<ReportParameterObject>();
                listSubReportDataSource.Add(new ReportParameterObject() { SubReportName = "stockInOutHead", Value = ivr140 });
                doDocument140.SubReportDataSource = listSubReportDataSource;

                slaveDoc.Add(doDocument140);
            } //end if

            List<RPTdoIVR141> ivr141 = base.GetIVR141(monthYear, MiscType.C_INV_AREA);
            if (ivr141.Count != 0)
            {
                doDocumentDataGenerate doDocument141 = new doDocumentDataGenerate();
                doDocument141.DocumentNo = monthYear;
                doDocument141.DocumentCode = ReportID.C_INV_REPORT_ID_SUMMARY_IN_OUT;
                doDocument141.DocumentTemplateCode = ReportID.C_INV_REPORT_ID_SUMMARY_IN_OUT_OFFICE;
                doDocument141.DocumentCode = ReportID.C_INV_REPORT_ID_SUMMARY_IN_OUT;
                doDocument141.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
                doDocument141.DocumentData = ivr141;
                doDocument141.OtherKey.InventorySlipIssueOffice = headOffice[0].OfficeCode;

                // Additional
                doDocument141.EmpNo = strEmpNo;
                doDocument141.ProcessDateTime = dtDateTime;

                listMainReportParam = new List<ReportParameterObject>();
                dLst = documentHandler.GetDocumentTemplateByDocumentCode(ReportID.C_INV_REPORT_ID_SUMMARY_IN_OUT_OFFICE);
                if (dLst.Count > 0)
                {
                    listMainReportParam.Add(new ReportParameterObject() { ParameterName = "DocVersion", Value = dLst[0].DocumentVersion });
                }
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "MonthReport", Value = "(" + dtReportDateTime.ToString("MMM. yyyy") + ")" });
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "Group", Value = "Office" });
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "MonthYearReport", Value = dtReportDateTime.ToString("MMMM / yyyy") });
                doDocument141.MainReportParam = listMainReportParam;

                listSubReportDataSource = new List<ReportParameterObject>();
                listSubReportDataSource.Add(new ReportParameterObject() { SubReportName = "stockInOutDetail", Value = ivr141 });
                doDocument141.SubReportDataSource = listSubReportDataSource;

                slaveDoc.Add(doDocument141);
            } //end if

            List<RPTdoIVR142> ivr142 = base.GetIVR142(monthYear, MiscType.C_INV_AREA);
            if (ivr142.Count != 0)
            {
                doDocumentDataGenerate doDocument142 = new doDocumentDataGenerate();
                doDocument142.DocumentNo = monthYear;
                doDocument142.DocumentCode = ReportID.C_INV_REPORT_ID_SUMMARY_IN_OUT;
                doDocument142.DocumentTemplateCode = ReportID.C_INV_REPORT_ID_SUMMARY_IN_OUT_WIP_GROUP;
                doDocument142.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
                doDocument142.DocumentData = ivr142;
                doDocument142.OtherKey.InventorySlipIssueOffice = headOffice[0].OfficeCode;

                // Additional
                doDocument142.EmpNo = strEmpNo;
                doDocument142.ProcessDateTime = dtDateTime;

                listMainReportParam = new List<ReportParameterObject>();
                dLst = documentHandler.GetDocumentTemplateByDocumentCode(ReportID.C_INV_REPORT_ID_SUMMARY_IN_OUT_WIP_GROUP);
                if (dLst.Count > 0)
                {
                    listMainReportParam.Add(new ReportParameterObject() { ParameterName = "DocVersion", Value = dLst[0].DocumentVersion });
                }
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "MonthReport", Value = "(" + dtReportDateTime.ToString("MMM. yyyy") + ")" });
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "Group", Value = "WIP Group" });
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "MonthYearReport", Value = dtReportDateTime.ToString("MMMM / yyyy") });
                doDocument142.MainReportParam = listMainReportParam;

                listSubReportDataSource = new List<ReportParameterObject>();
                listSubReportDataSource.Add(new ReportParameterObject() { SubReportName = "stockInOutWIP", Value = ivr142 });
                doDocument142.SubReportDataSource = listSubReportDataSource;

                slaveDoc.Add(doDocument142);
            } //end if

            List<RPTdoIVR143> ivr143 = base.GetIVR143(monthYear, MiscType.C_INV_AREA);
            if (ivr143.Count != 0)
            {
                doDocumentDataGenerate doDocument143 = new doDocumentDataGenerate();
                doDocument143.DocumentNo = monthYear;
                doDocument143.DocumentCode = ReportID.C_INV_REPORT_ID_SUMMARY_IN_OUT;
                doDocument143.DocumentTemplateCode = ReportID.C_INV_REPORT_ID_SUMMARY_IN_OUT_USER;
                doDocument143.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
                doDocument143.DocumentData = ivr143;
                doDocument143.OtherKey.InventorySlipIssueOffice = headOffice[0].OfficeCode;

                // Additional
                doDocument143.EmpNo = strEmpNo;
                doDocument143.ProcessDateTime = dtDateTime;

                listMainReportParam = new List<ReportParameterObject>();
                dLst = documentHandler.GetDocumentTemplateByDocumentCode(ReportID.C_INV_REPORT_ID_SUMMARY_IN_OUT_USER);
                if (dLst.Count > 0)
                {
                    listMainReportParam.Add(new ReportParameterObject() { ParameterName = "DocVersion", Value = dLst[0].DocumentVersion });
                }
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "MonthReport", Value = "(" + dtReportDateTime.ToString("MMM. yyyy") + ")" });
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "Group", Value = "User" });
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "MonthYearReport", Value = dtReportDateTime.ToString("MMMM / yyyy") });
                doDocument143.MainReportParam = listMainReportParam;

                listSubReportDataSource = new List<ReportParameterObject>();
                listSubReportDataSource.Add(new ReportParameterObject() { SubReportName = "stockInOutUser", Value = ivr143 });
                doDocument143.SubReportDataSource = listSubReportDataSource;

                slaveDoc.Add(doDocument143);
            } //end if

            if (slaveDoc.Count == 0)
            {
                return null;
            }

            doDocumentDataGenerate maindoc = slaveDoc[0];
            slaveDoc.RemoveAt(0);

            string strFilePath = documentHandler.GenerateDocumentFilePath(maindoc, slaveDoc);
            return strFilePath;
        }



        // IVR150-Summary inventory asset report
        public Stream GenerateIVR150(string monthYearGenerate, string strEmpNo, DateTime dtDateTime)
        {
            string strFilePath = GenerateIVR150FilePath(monthYearGenerate, strEmpNo, dtDateTime);
            IDocumentHandler handlerDocument = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            return handlerDocument.GetDocumentReportFileStream(strFilePath);

        }
        public string GenerateIVR150FilePath(string monthYearGenerate, string strEmpNo, DateTime dtDateTime)
        {
            //string monthYearGenerate = dtReportDateTime.ToString("yyyyMM");
            List<RPTdoIVR150> ivr150 = base.GetIVR150(monthYearGenerate);
            List<doOffice> headOffice = this.GetInventoryHeadOffice();

            if (ivr150.Count == 0)
            {
                return null;
            } //end if

            doDocumentDataGenerate doDocument = new doDocumentDataGenerate();
            doDocument.DocumentNo = monthYearGenerate;
            doDocument.DocumentCode = ReportID.C_INV_REPORT_ID_SUMMARY_ASSET;
            doDocument.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
            doDocument.DocumentData = ivr150;
            doDocument.OtherKey.InventorySlipIssueOffice = headOffice[0].OfficeCode;
            doDocument.OtherKey.MonthYear = DateTime.ParseExact(monthYearGenerate, "yyyyMM", System.Globalization.CultureInfo.CurrentCulture);

            // Additional
            doDocument.EmpNo = strEmpNo;
            doDocument.ProcessDateTime = dtDateTime;

            // TODO: (Akat) Client must set monthYear to this function (GenerateIVR150FilePath)
            //doDocument.OtherKey.MonthYear = monthYear;

            IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
            List<tbm_DocumentTemplate> dLst = documentHandler.GetDocumentTemplateByDocumentCode(ReportID.C_INV_REPORT_ID_SUMMARY_ASSET);

            List<ReportParameterObject> listMainReportParam = new List<ReportParameterObject>();
            if (dLst.Count > 0)
            {
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "DocVersion", Value = dLst[0].DocumentVersion });
            }
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "MonthReport", Value = "(" + DateTime.Now.ToString("MMM. yyyy") + ")" });
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "MonthYearReport", Value = DateTime.Now.ToString("MMMM / yyyy") });


            doDocument.MainReportParam = listMainReportParam;

            string strFilePath = documentHandler.GenerateDocumentFilePath(doDocument);

            return strFilePath;
        }


        // IVR170-Picking list Slip (MK-22)
        public Stream GenerateIVR170(string strPickingListNo, string strEmpNo, DateTime dtDateTime)
        {
            //Modify by Jutarat A. on 04122012
            //string strFilePath = GenerateIVR170FilePath(strPickingListNo, strEmpNo, dtDateTime);
            //IDocumentHandler handlerDocument = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
            //return handlerDocument.GetDocumentReportFileStream(strFilePath);

            doDocumentDataGenerate doDocument = CreateDocumentDataIVR170(strPickingListNo, strEmpNo, dtDateTime);
            if (doDocument == null)
                return null;

            IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
            return documentHandler.GenerateDocument(doDocument);
            //End Modify
        }
        public string GenerateIVR170FilePath(string strPickingListNo, string strEmpNo, DateTime dtDateTime)
        {
            //Comment by Jutarat A. on 04122012 (Move to CreateDocumentDataIVR170())
            #region Old code
            //IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            //List<doIVR170> ivr170 = base.GetIVR170(strPickingListNo, MiscType.C_INV_AREA);

            //List<tbm_DocumentTemplate> dLst = documentHandler.GetDocumentTemplateByDocumentCode(ReportID.C_INV_REPORT_ID_PICKING_LIST);

            //if (ivr170.Count == 0)
            //{
            //    return null;
            //} //end if

            //doDocumentDataGenerate doDocument = new doDocumentDataGenerate();
            //doDocument.DocumentNo = strPickingListNo;
            //doDocument.DocumentCode = ReportID.C_INV_REPORT_ID_PICKING_LIST;
            //doDocument.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
            //doDocument.DocumentData = ivr170;
            //doDocument.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            //doDocument.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;

            //List<doOffice> headOffice = this.GetInventoryHeadOffice();
            //if (headOffice != null && headOffice.Count > 0)
            //{
            //    doDocument.OtherKey.InventorySlipIssueOffice = headOffice[0].OfficeCode;
            //}

            //// Additional
            //doDocument.EmpNo = strEmpNo;
            //doDocument.ProcessDateTime = dtDateTime;

            //List<ReportParameterObject> listMainReportParam = new List<ReportParameterObject>();
            //listMainReportParam.Add(new ReportParameterObject() { ParameterName = "SlipIssueDate", Value = DateTime.Now.ToString("dd-MMM-yyyy") });
            //if (dLst.Count > 0)
            //{
            //    listMainReportParam.Add(new ReportParameterObject() { ParameterName = "DocVersion", Value = dLst[0].DocumentVersion });
            //    listMainReportParam.Add(new ReportParameterObject() { ParameterName = "TemplateName", Value = dLst[0].DocumentNameEN });
            //}
            //doDocument.MainReportParam = listMainReportParam;

            //string slipNoReportPath = documentHandler.GenerateDocumentFilePath(doDocument);
            //return slipNoReportPath;
            #endregion
            //End Comment

            //Add by Jutarat A. on 04122012
            doDocumentDataGenerate doDocument = CreateDocumentDataIVR170(strPickingListNo, strEmpNo, dtDateTime);
            if (doDocument == null)
                return null;

            IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
            return documentHandler.GenerateDocumentFilePath(doDocument);
            //End Add
        }

        //Add by Jutarat A. on 04122012
        private doDocumentDataGenerate CreateDocumentDataIVR170(string strPickingListNo, string strEmpNo, DateTime dtDateTime)
        {
            IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            List<doIVR170> ivr170 = base.GetIVR170(strPickingListNo, MiscType.C_INV_AREA);

            List<tbm_DocumentTemplate> dLst = documentHandler.GetDocumentTemplateByDocumentCode(ReportID.C_INV_REPORT_ID_PICKING_LIST);

            if (ivr170.Count == 0)
                return null;

            doDocumentDataGenerate doDocument = new doDocumentDataGenerate();
            doDocument.DocumentNo = strPickingListNo;
            doDocument.DocumentCode = ReportID.C_INV_REPORT_ID_PICKING_LIST;
            doDocument.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
            doDocument.DocumentData = ivr170;
            doDocument.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            doDocument.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;

            List<doOffice> headOffice = this.GetInventoryHeadOffice();
            if (headOffice != null && headOffice.Count > 0)
            {
                doDocument.OtherKey.InventorySlipIssueOffice = headOffice[0].OfficeCode;
            }

            // Additional
            doDocument.EmpNo = strEmpNo;
            doDocument.ProcessDateTime = dtDateTime;

            List<ReportParameterObject> listMainReportParam = new List<ReportParameterObject>();
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "SlipIssueDate", Value = DateTime.Now.ToString("dd-MMM-yyyy") });
            if (dLst.Count > 0)
            {
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "DocVersion", Value = dLst[0].DocumentVersion });
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "TemplateName", Value = dLst[0].DocumentNameEN });
            }
            doDocument.MainReportParam = listMainReportParam;

            return doDocument;
        }
        //End Add

        // IVR180-Stock out Slip (MK-22)
        public Stream GenerateIVR180(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime)
        {
            string strFilePath = GenerateIVR180FilePath(strInventorySlipNo, strInventorySlipIssueOffice, strEmpNo, dtDateTime);
            IDocumentHandler handlerDocument = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            return handlerDocument.GetDocumentReportFileStream(strFilePath);

        }
        public string GenerateIVR180FilePath(string strInventorySlipNo, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime)
        {
            IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            //Modify by Jutarat A. on 22102013
            /*List<doIVR> ivr180tmp = base.GetIVR(MiscType.C_INV_STOCKIN_TYPE, MiscType.C_INV_LOC, MiscType.C_INV_AREA, strInventorySlipNo);

            List<doIVR> ivr180 = (
                from d in ivr180tmp
                group d by new
                {
                    SlipNo = d.SlipNo,
                    InstallationSlipNo = d.InstallationSlipNo,
                    SlipIssueDate = d.SlipIssueDate,
                    SourceAreaName = d.SourceAreaName,
                    DestAreaName = d.DestAreaName,
                    InstrumentCode = d.InstrumentCode,
                    InstrumentName = d.InstrumentName
                } into gShelf
                orderby gShelf.Key.InstrumentCode, gShelf.Key.SourceAreaName
                select new doIVR()
                {
                    SlipNo = gShelf.Key.SlipNo,
                    SlipIssueDate = gShelf.Key.SlipIssueDate,
                    InstallationSlipNo = gShelf.Key.InstallationSlipNo,
                    SourceAreaName = gShelf.Key.SourceAreaName,
                    DestAreaName = gShelf.Key.DestAreaName,
                    InstrumentCode = gShelf.Key.InstrumentCode,
                    InstrumentName = gShelf.Key.InstrumentName,
                    SourceOfficeCode = gShelf.First().SourceOfficeCode,
                    TransferQty = gShelf.Sum(g => g.TransferQty)
                    // Akat K. Add
                    ,ProjectCode = gShelf.First().ProjectCode
                }
            ).ToList();*/

            List<doIVR180> ivr180tmp = base.GetIVR180(strInventorySlipNo, MiscType.C_INV_LOC, MiscType.C_INV_AREA);

            List<doIVR180> ivr180 = (
                from d in ivr180tmp
                group d by new
                {
                    SlipNo = d.SlipNo,
                    InstallationSlipNo = d.InstallationSlipNo,
                    SlipIssueDate = d.SlipIssueDate,
                    SourceAreaName = d.SourceAreaName,
                    DestAreaName = d.DestAreaName,
                    InstrumentCode = d.InstrumentCode,
                    InstrumentName = d.InstrumentName,
                    ContractCode = d.ContractCode,
                    OperationOfficeName = d.OperationOfficeName,
                    SiteName = d.SiteName,
                    SubContractorName = d.SubContractorName,
                    StockInDate = d.StockInDate,
                    StockOutDate = d.StockOutDate,
                } into gShelf
                orderby gShelf.Key.InstrumentCode, gShelf.Key.SourceAreaName
                select new doIVR180()
                {
                    SlipNo = gShelf.Key.SlipNo,
                    SlipIssueDate = gShelf.Key.SlipIssueDate,
                    InstallationSlipNo = gShelf.Key.InstallationSlipNo,
                    SourceAreaName = gShelf.Key.SourceAreaName,
                    DestAreaName = gShelf.Key.DestAreaName,
                    InstrumentCode = gShelf.Key.InstrumentCode,
                    InstrumentName = gShelf.Key.InstrumentName,
                    SourceOfficeCode = gShelf.First().SourceOfficeCode,
                    TransferQty = gShelf.Sum(g => g.TransferQty),
                    ProjectCode = gShelf.First().ProjectCode,
                    ContractCode = gShelf.Key.ContractCode,
                    OperationOfficeName = gShelf.Key.OperationOfficeName,
                    SiteName = gShelf.Key.SiteName,
                    SubContractorName = gShelf.Key.SubContractorName,
                    StockInDate = gShelf.Key.StockInDate,
                    StockOutDate = gShelf.Key.StockOutDate,
                }
            ).ToList();
            //End Modify

            List<tbm_DocumentTemplate> dLst = documentHandler.GetDocumentTemplateByDocumentCode(ReportID.C_INV_REPORT_ID_STOCKOUT);
            if (ivr180.Count == 0)
            {
                return null;
            } //end if

            doDocumentDataGenerate doDocument = new doDocumentDataGenerate();
            doDocument.DocumentNo = strInventorySlipNo;
            doDocument.DocumentCode = ReportID.C_INV_REPORT_ID_STOCKOUT;
            doDocument.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
            doDocument.DocumentData = ivr180;
            doDocument.OtherKey.InventorySlipIssueOffice = ivr180[0].SourceOfficeCode;
            doDocument.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            doDocument.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;

            // Additional
            doDocument.EmpNo = strEmpNo;
            doDocument.ProcessDateTime = dtDateTime;

            List<ReportParameterObject> listMainReportParam = new List<ReportParameterObject>();
            if (dLst.Count > 0)
            {
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "DocVersion", Value = dLst[0].DocumentVersion });
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "TemplateName", Value = dLst[0].DocumentNameEN });
            }
            doDocument.MainReportParam = listMainReportParam;

            var childReport = new List<doDocumentDataGenerate>();

            doDocumentDataGenerate doDocumentIVR181 = null;
            List<doIVR181> ivr181 = base.GetIVR181(strInventorySlipNo, MiscType.C_INV_LOC, MiscType.C_INV_AREA);
            List<tbm_DocumentTemplate> dLstIVR181 = documentHandler.GetDocumentTemplateByDocumentCode(ReportID.C_INV_REPORT_ID_STOCKOUT_B);
            if (ivr181.Count > 0)
            {
                doDocumentIVR181 = new doDocumentDataGenerate();
                doDocumentIVR181.DocumentNo = strInventorySlipNo;
                doDocumentIVR181.DocumentCode = ReportID.C_INV_REPORT_ID_STOCKOUT_B;
                doDocumentIVR181.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
                doDocumentIVR181.DocumentData = ivr181;
                doDocumentIVR181.OtherKey.InventorySlipIssueOffice = ivr181[0].SourceOfficeCode;
                doDocumentIVR181.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doDocumentIVR181.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;

                // Additional
                doDocumentIVR181.EmpNo = strEmpNo;
                doDocumentIVR181.ProcessDateTime = dtDateTime;

                List<ReportParameterObject> lstParam = new List<ReportParameterObject>();
                if (dLstIVR181.Count > 0)
                {
                    lstParam.Add(new ReportParameterObject() { ParameterName = "DocVersion", Value = dLstIVR181[0].DocumentVersion });
                    lstParam.Add(new ReportParameterObject() { ParameterName = "TemplateName", Value = dLstIVR181[0].DocumentNameEN });
                }
                doDocumentIVR181.MainReportParam = lstParam;

                childReport.Add(doDocumentIVR181);
            }

            string slipNoReportPath = documentHandler.GenerateDocumentFilePath(doDocument, childReport);
            return slipNoReportPath;
        }



        // IVR190-Purchase order JPN
        public Stream GenerateIVR190(string strPurchaseOrderNo, string strOfficeCode, string strEmpNo, DateTime dtDateTime)
        {
            string strFilePath = GenerateIVR190FilePath(strPurchaseOrderNo, strOfficeCode, strEmpNo, dtDateTime);
            IDocumentHandler handlerDocument = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            return handlerDocument.GetDocumentReportFileStream(strFilePath);

        }
        public string GenerateIVR190FilePath(string strPurchaseOrderNo, string strOfficeCode, string strEmpNo, DateTime dtDateTime)
        {
            IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            List<doIVR190> ivr190 = base.GetIVR190(
                strPurchaseOrderNo,
                InventoryHeadOffice.C_OFFICELEVEL_HEAD,
                DepartmentMaster.C_DEPT_PURCHASE,
                FlagType.C_FLAG_ON,
                CurrencyType.C_CURRENCY_TYPE_THB,
                CurrencyType.C_CURRENCY_TYPE_USD,
                CurrencyType.C_CURRENCY_TYPE_EUR,
                CurrencyType.C_CURRENCY_TYPE_YEN);

            List<tbm_DocumentTemplate> dLst = documentHandler.GetDocumentTemplateByDocumentCode(ReportID.C_INV_REPORT_ID_PURCHASE_ORDER_CHN);

            if (ivr190.Count == 0)
            {
                return null;
            } //end if

            doDocumentDataGenerate doDocument = new doDocumentDataGenerate();
            doDocument.DocumentNo = strPurchaseOrderNo;
            doDocument.DocumentCode = ReportID.C_INV_REPORT_ID_PURCHASE_ORDER_CHN;
            doDocument.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
            doDocument.DocumentData = ivr190;
            doDocument.OtherKey.InventorySlipIssueOffice = strOfficeCode;
            doDocument.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            doDocument.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;

            // Additional
            doDocument.EmpNo = strEmpNo;
            doDocument.ProcessDateTime = dtDateTime;

            int totalOrder = 0;
            decimal totalAmount = 0;
            decimal grandTotalAmount = 0;
            string textCharge = " ";
            string charge = " ";
            bool currencyUS = false;
            foreach (var i in ivr190)
            {
                totalOrder += i.OrderQty;
                totalAmount += i.Amount;

                if (i.Currency == CurrencyType.@C_CURRENCY_TYPE_THB)
                {
                    i.Currency = "THB฿";
                }
                else if (i.Currency == CurrencyType.@C_CURRENCY_TYPE_USD)
                {
                    i.Currency = "US$";
                    currencyUS = true;
                }
                else if (i.Currency == CurrencyType.@C_CURRENCY_TYPE_EUR)
                {
                    i.Currency = "EUR€";
                }
                else if (i.Currency == CurrencyType.@C_CURRENCY_TYPE_YEN)
                {
                    i.Currency = "YEN¥";
                }

                //if (i.AccountName.Length > 35)
                if (String.IsNullOrEmpty(i.AccountName) == false && i.AccountName.Length > 35) //Modify by Jutarat A. on 04022013
                {
                    i.AccountName = i.AccountName.Substring(0, 35);
                }
            }

            grandTotalAmount = totalAmount;
            if (currencyUS)
            {
                textCharge = "Handling charge (US$50 must be added to the sub-total, if the purchasing amount is less than US$800 or equivalent.)";

                charge = "0.00";
                if (totalAmount <= 800)
                {
                    charge = "50.00";
                    grandTotalAmount += 50;
                }
            }

            List<ReportParameterObject> listMainReportParam = new List<ReportParameterObject>();
            if (dLst.Count > 0)
            {
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "DocVersion", Value = dLst[0].DocumentVersion });
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "TemplateName", Value = dLst[0].DocumentNameEN });

                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "totalOrder", Value = totalOrder });
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "totalAmount", Value = totalAmount });
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "grandTotalAmount", Value = grandTotalAmount });
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "textCharge", Value = textCharge });
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "charge", Value = charge });

            }
            doDocument.MainReportParam = listMainReportParam;

            string slipNoReportPath = documentHandler.GenerateDocumentFilePath(doDocument);
            return slipNoReportPath;
        }


        // IVR191-Purchase order DOM
        public Stream GenerateIVR191(string strPurchaseOrderNo, string strOfficeCode, string strEmpNo, DateTime dtDateTime)
        {
            string strFilePath = GenerateIVR191FilePath(strPurchaseOrderNo, strOfficeCode, strEmpNo, dtDateTime);
            IDocumentHandler handlerDocument = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            return handlerDocument.GetDocumentReportFileStream(strFilePath);

        }
        public string GenerateIVR191FilePath(string strPurchaseOrderNo, string strOfficeCode, string strEmpNo, DateTime dtDateTime)
        {
            int MAX_ROW = 15;
            int LAST_IDX = 14;

            IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            List<doIVR191> ivr191 = base.GetIVR191(strPurchaseOrderNo, ConfigName.C_VAT_THB, Instrument.C_UNIT_PCS, InventoryHeadOffice.C_OFFICELEVEL_HEAD, DepartmentMaster.C_DEPT_PURCHASE, FlagType.C_FLAG_ON);

            List<tbm_DocumentTemplate> dLst = documentHandler.GetDocumentTemplateByDocumentCode(ReportID.C_INV_REPORT_ID_PURCHASE_ORDER_DOM);

            if (ivr191.Count == 0)
            {
                return null;
            } //end if

            foreach (var i in ivr191)
            {
                i.OrderQtyShow = i.OrderQty == 0 ? "-" : i.OrderQty.ToString("#,##0");
                i.UnitPriceShow = i.UnitPrice == 0 ? "-" : i.UnitPrice.ToString("#,##0.00");
                i.TotalPricePerRowShow = i.TotalPricePerRow == 0 ? "-" : i.TotalPricePerRow.ToString("#,##0.00");
            }

            if (ivr191.Count < MAX_ROW)
            {
                for (int i = ivr191.Count; i < MAX_ROW; i++)
                {
                    ivr191.Add(new doIVR191
                    {
                        PurchaseOrderNo = " ",
                        OrderDate = " ",
                        SupplierNameLC = " ",
                        AddressFullLC = " ",
                        PhoneNo = " ",
                        FaxNo = " ",
                        InstrumentCode = " ",
                        OrderQty = 0,
                        Unit = " ",
                        UnitPrice = 0,
                        TotalPricePerRow = 0,
                        TotalPrice = 0,
                        VatRate = " ",
                        VatPrice = 0,
                        TotalPriceIncludeVat = 0,
                        Memo = " ",
                        AdjustDueDate = " ",
                        PersonInCharge = " ",
                    });
                }
            }

            ivr191[LAST_IDX].TotalPrice = ivr191[0].TotalPrice;
            ivr191[LAST_IDX].VatPrice = ivr191[0].VatPrice;
            ivr191[LAST_IDX].TotalPriceIncludeVat = ivr191[0].TotalPriceIncludeVat;
            ivr191[LAST_IDX].Memo = ivr191[0].Memo;
            ivr191[LAST_IDX].AdjustDueDate = ivr191[0].AdjustDueDate;
            ivr191[LAST_IDX].PersonInCharge = ivr191[0].PersonInCharge;

            doDocumentDataGenerate doDocument = new doDocumentDataGenerate();
            doDocument.DocumentNo = strPurchaseOrderNo;
            doDocument.DocumentCode = ReportID.C_INV_REPORT_ID_PURCHASE_ORDER_DOM;
            doDocument.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
            doDocument.DocumentData = ivr191;
            doDocument.OtherKey.InventorySlipIssueOffice = strOfficeCode;
            doDocument.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            doDocument.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;

            // Additional
            doDocument.EmpNo = strEmpNo;
            doDocument.ProcessDateTime = dtDateTime;

            List<ReportParameterObject> listMainReportParam = new List<ReportParameterObject>();
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "PurchaseOrderNo", Value = "" });
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "OrderDate", Value = "" });
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "SupplierName", Value = "" });
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "SupplierAddress", Value = "" });
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "SupplierPhoneNo", Value = "" });
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "SupplierFaxNo", Value = "" });
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "VatRate", Value = 0.00 });
            listMainReportParam.Add(new ReportParameterObject() { ParameterName = "VatPrice", Value = 0.00 });

            if (dLst.Count > 0)
            {
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "DocVersion", Value = dLst[0].DocumentVersion });
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "TemplateName", Value = dLst[0].DocumentNameEN });
            }
            doDocument.MainReportParam = listMainReportParam;

            string slipNoReportPath = documentHandler.GenerateDocumentFilePath(doDocument);
            return slipNoReportPath;
        }

        // IVR210-Returned Slip (MK-43)
        public Stream GenerateIVR210(string strInventorySlipNoList, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime)
        {
            string strFilePath = GenerateIVR210FilePath(strInventorySlipNoList, strInventorySlipIssueOffice, strEmpNo, dtDateTime);
            IDocumentHandler handlerDocument = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            return handlerDocument.GetDocumentReportFileStream(strFilePath);

        }
        public string GenerateIVR210FilePath(string strInventorySlipNoList, string strInventorySlipIssueOffice, string strEmpNo, DateTime dtDateTime)
        {
            if (string.IsNullOrEmpty(strInventorySlipNoList))
                return null;

            var lstInvSlipNo = strInventorySlipNoList.Split(',').ToList();
            if (lstInvSlipNo == null || lstInvSlipNo.Count == 0)
                return null;

            IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
            List<tbm_DocumentTemplate> dLst = documentHandler.GetDocumentTemplateByDocumentCode(ReportID.C_INV_REPORT_ID_RETURNED);

            doDocumentDataGenerate doDocumentMain = null;
            List<doDocumentDataGenerate> lstdoDocumentSlave = new List<doDocumentDataGenerate>();

            foreach (var slipno in lstInvSlipNo)
            {
                var slip = base.GetTbt_InventorySlip(slipno, null);
                if (slip == null)
                    return null;

                List<doIVR210> ivr210 = base.GetIVR210(slipno, MiscType.C_INV_LOC);

                if (ivr210.Count == 0)
                {
                    return null;
                }

                doDocumentDataGenerate doDocument = new doDocumentDataGenerate();
                doDocument.DocumentNo = slip[0].InstallationSlipNo;
                doDocument.DocumentCode = ReportID.C_INV_REPORT_ID_RETURNED;
                doDocument.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
                doDocument.DocumentData = ivr210;
                doDocument.OtherKey.InventorySlipIssueOffice = ivr210[0].SourceOfficeCode;
                doDocument.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doDocument.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;

                // Additional
                doDocument.EmpNo = strEmpNo;
                doDocument.ProcessDateTime = dtDateTime;

                List<ReportParameterObject> listMainReportParam = new List<ReportParameterObject>();
                if (dLst.Count > 0)
                {
                    listMainReportParam.Add(new ReportParameterObject() { ParameterName = "DocVersion", Value = dLst[0].DocumentVersion });
                    listMainReportParam.Add(new ReportParameterObject() { ParameterName = "TemplateName", Value = dLst[0].DocumentNameEN });
                }
                doDocument.MainReportParam = listMainReportParam;

                if (doDocumentMain == null)
                {
                    doDocumentMain = doDocument;
                }
                else
                {
                    lstdoDocumentSlave.Add(doDocument);
                }
            }

            string slipNoReportPath;

            if (lstdoDocumentSlave.Count == 0)
            {
                slipNoReportPath = documentHandler.GenerateDocumentFilePath(doDocumentMain);
            }
            else
            {
                slipNoReportPath = documentHandler.GenerateDocumentFilePath(doDocumentMain, lstdoDocumentSlave);
            }

            return slipNoReportPath;
        }


        public bool GenerateInventoryAccountData(DateTime dtDateGenerate, string strEmpNo, DateTime dtProcessTime)
        {
            bool bResult = true;

            string strMainDepartmentCode = null;
            string strEmpFirstNameEN = null;
            string strEmpLastNameEN = null;
            string strDepartmentName = null;

            IEmployeeMasterHandler srvEmployee = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
            var lstEmpInfo = srvEmployee.GetEmployeeOffice(strEmpNo, true);
            if (lstEmpInfo != null && lstEmpInfo.Count > 0)
            {
                strMainDepartmentCode = lstEmpInfo[0].DepartmentCode;
                strEmpFirstNameEN = lstEmpInfo[0].EmpFirstNameEN;
                strEmpLastNameEN = lstEmpInfo[0].EmpLastNameEN;

                IMasterHandler srvMaster = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                List<tbm_Department> lstDeptInfo = srvMaster.GetTbm_Department().Where(d => d.DepartmentCode == strMainDepartmentCode).ToList();
                if (lstDeptInfo != null && lstDeptInfo.Count > 0)
                {
                    strDepartmentName = lstDeptInfo[0].DepartmentName;
                }
            }


            string strCommonFileHeader = string.Join(",",
                dtProcessTime.ToString(@"dd\/MM\/yyyy HH:mm:ss"),
                strEmpNo,
                strEmpFirstNameEN,
                strEmpLastNameEN,
                strDepartmentName
            );
            IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
            ICommonHandler commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            // Generate ACC010
            {
                const string strReportName = "ACC010";
                string strFileName = string.Format("{0}{1}.csv", strReportName, dtDateGenerate.ToString("yyyyMM"));
                string strFileHeader = string.Join(",", strCommonFileHeader, strReportName, dtDateGenerate.ToString(@"yyyy\/MM"));

                List<doCSVInvDepreciationAcc> lstReportDetail = base.GetExportInvDepreciationAcc(dtDateGenerate);
                if (lstReportDetail == null && lstReportDetail.Count <= 0)
                {
                    bResult = false;
                }
                else
                {
                    List<tbt_DocumentList> lstCheckDoc = commonHandler.GetTbt_DocumentList(strFileName, ReportID.C_INV_REPORT_ID_ACCOUNT_DEPRECIATION);
                    if (lstCheckDoc == null || lstCheckDoc.Count <= 0)
                    {

                        StringBuilder sbContent = new StringBuilder();
                        sbContent.AppendLine(strFileHeader);
                        sbContent.AppendLine();

                        var qStartType = (from d in lstReportDetail group d by d.StartType into g select g);
                        foreach (var gStartType in qStartType)
                        {
                            string strReportDetail = CSVReportUtil.GenerateCSVData<doCSVInvDepreciationAcc>(gStartType.ToList(), false, true);
                            sbContent.AppendLine(strReportDetail);
                            sbContent.AppendLine();
                        }

                        doDocumentDataGenerate doDocument = new doDocumentDataGenerate();
                        doDocument.DocumentNo = strFileName;
                        doDocument.DocumentCode = ReportID.C_INV_REPORT_ID_ACCOUNT_DEPRECIATION;
                        doDocument.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
                        doDocument.DocumentData = sbContent.ToString();
                        doDocument.OtherKey.MonthYear = dtDateGenerate;

                        // Additional
                        doDocument.EmpNo = strEmpNo;
                        doDocument.ProcessDateTime = dtProcessTime;

                        documentHandler.GenerateTextReportFilePath(doDocument, dtProcessTime, strFileName, sbContent.ToString());
                        //documentHandler.GenerateTextReportFilePath(null, dtProcessTime, strFileName, sbContent.ToString());
                    }
                }
            }

            // Generate ACC020
            {
                const string strReportName = "ACC020";
                string strFileName = string.Format("{0}{1}.csv", strReportName, dtDateGenerate.ToString("yyyyMM"));
                string strFileHeader = string.Join(",", strCommonFileHeader, strReportName, dtDateGenerate.ToString(@"yyyy\/MM"));

                List<doCSVassetAmountAcc> lstReportDetail = this.GetExportAssetAmountAcc(
                    InventoryAccountCode.C_INV_ACCOUNT_CODE_SALE,
                    InventoryAccountCode.C_INV_ACCOUNT_CODE_SPECIAL,
                    InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK,
                    InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTALLED,
                    InventoryAccountCode.C_INV_ACCOUNT_CODE_ELIMINATE,
                    InventoryAccountCode.C_INV_ACCOUNT_CODE_ADJUST
                );

                if (lstReportDetail == null && lstReportDetail.Count <= 0)
                {
                    bResult = false;
                }
                else
                {
                    List<tbt_DocumentList> lstCheckDoc = commonHandler.GetTbt_DocumentList(strFileName, ReportID.C_INV_REPORT_ID_ACCOUNT_ASSETAMOUNT);
                    if (lstCheckDoc == null || lstCheckDoc.Count <= 0)
                    {
                        string strReportDetail = CSVReportUtil.GenerateCSVData<doCSVassetAmountAcc>(lstReportDetail, false, true);

                        StringBuilder sbContent = new StringBuilder();
                        sbContent.AppendLine(strFileHeader);
                        sbContent.AppendLine();
                        sbContent.AppendLine(strReportDetail);
                        sbContent.AppendLine();

                        doDocumentDataGenerate doDocument = new doDocumentDataGenerate();
                        doDocument.DocumentNo = strFileName;
                        doDocument.DocumentCode = ReportID.C_INV_REPORT_ID_ACCOUNT_ASSETAMOUNT;
                        doDocument.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
                        doDocument.DocumentData = sbContent.ToString();
                        doDocument.OtherKey.MonthYear = dtDateGenerate;

                        // Additional
                        doDocument.EmpNo = strEmpNo;
                        doDocument.ProcessDateTime = dtProcessTime;

                        documentHandler.GenerateTextReportFilePath(doDocument, dtProcessTime, strFileName, sbContent.ToString());
                        //documentHandler.GenerateTextReportFilePath(null, dtProcessTime, strFileName, sbContent.ToString());
                    }
                }
            }

            // Generate ACC030
            {
                const string strReportName = "ACC030";
                string strFileName = string.Format("{0}{1}.csv", strReportName, dtDateGenerate.ToString("yyyyMM"));
                string strFileHeader = string.Join(",", strCommonFileHeader, strReportName, dtDateGenerate.ToString(@"yyyy\/MM"));

                List<doCSVMovingAssetAcc> lstReportDetail = this.ExportMovingAssetAcc(MiscType.C_INV_ACCOUNT_CODE, dtDateGenerate);
                if (lstReportDetail == null && lstReportDetail.Count <= 0)
                {
                    bResult = false;
                }
                else
                {
                    List<tbt_DocumentList> lstCheckDoc = commonHandler.GetTbt_DocumentList(strFileName, ReportID.C_INV_REPORT_ID_ACCOUNT_MOVINGASSET);
                    if (lstCheckDoc == null || lstCheckDoc.Count <= 0)
                    {
                        string strReportDetail = CSVReportUtil.GenerateCSVData<doCSVMovingAssetAcc>(lstReportDetail, false, true);

                        StringBuilder sbContent = new StringBuilder();
                        sbContent.AppendLine(strFileHeader);
                        sbContent.AppendLine();
                        sbContent.AppendLine(strReportDetail);
                        sbContent.AppendLine();

                        doDocumentDataGenerate doDocument = new doDocumentDataGenerate();
                        doDocument.DocumentNo = strFileName;
                        doDocument.DocumentCode = ReportID.C_INV_REPORT_ID_ACCOUNT_MOVINGASSET;
                        doDocument.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
                        doDocument.DocumentData = sbContent.ToString();
                        doDocument.OtherKey.MonthYear = dtDateGenerate;

                        // Additional
                        doDocument.EmpNo = strEmpNo;
                        doDocument.ProcessDateTime = dtProcessTime;

                        documentHandler.GenerateTextReportFilePath(doDocument, dtProcessTime, strFileName, sbContent.ToString());
                        //documentHandler.GenerateTextReportFilePath(null, dtProcessTime, strFileName, sbContent.ToString());
                    }
                }
            }

            // Generate ACC040
            {
                const string strReportName = "ACC040";
                string strFileName = string.Format("{0}{1}.csv", strReportName, dtDateGenerate.ToString("yyyyMM"));
                string strFileHeader = string.Join(",", strCommonFileHeader, strReportName, dtDateGenerate.ToString(@"yyyy\/MM"));

                List<doCSVOtherFinancialAcc> lstReportDetail = this.OtherFinancialAcc(
                    TransferType.C_INV_TRANSFERTYPE_STOCKIN_PURCHASE,
                    TransferType.C_INV_TRANSFERTYPE_STOCKIN_SPECIAL,
                    TransferType.C_INV_TRANSFERTYPE_ELIMINATION,
                    TransferType.C_INV_TRANSFERTYPE_STOCKOUT_SPECIAL,
                    TransferType.C_INV_TRANSFERTYPE_FIX_ADJUSTMENT,
                    dtDateGenerate
                );
                if (lstReportDetail == null && lstReportDetail.Count <= 0)
                {
                    bResult = false;
                }
                else
                {
                    List<tbt_DocumentList> lstCheckDoc = commonHandler.GetTbt_DocumentList(strFileName, ReportID.C_INV_REPORT_ID_ACCOUNT_FINANCE);
                    if (lstCheckDoc == null || lstCheckDoc.Count <= 0)
                    {
                        string strReportDetail = CSVReportUtil.GenerateCSVData<doCSVOtherFinancialAcc>(lstReportDetail, false, true);

                        StringBuilder sbContent = new StringBuilder();
                        sbContent.AppendLine(strFileHeader);
                        sbContent.AppendLine();
                        sbContent.AppendLine(strReportDetail);
                        sbContent.AppendLine();

                        doDocumentDataGenerate doDocument = new doDocumentDataGenerate();
                        doDocument.DocumentNo = strFileName;
                        doDocument.DocumentCode = ReportID.C_INV_REPORT_ID_ACCOUNT_FINANCE;
                        doDocument.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
                        doDocument.DocumentData = sbContent.ToString();
                        doDocument.OtherKey.MonthYear = dtDateGenerate;

                        // Additional
                        doDocument.EmpNo = strEmpNo;
                        doDocument.ProcessDateTime = dtProcessTime;

                        documentHandler.GenerateTextReportFilePath(doDocument, dtProcessTime, strFileName, sbContent.ToString());
                        //documentHandler.GenerateTextReportFilePath(null, dtProcessTime, strFileName, sbContent.ToString());
                    }
                }
            }

            return bResult;

        }

        #endregion

        #region Generate account report

        public string GenerateIVS280InReport(string reportType, List<dtInReportDetail> data)
        {
            const string TEMPLATE_NAME = "IVS280.xlsx";
            const string WSNAME_DETAIL = "Detail";
            const string WSNAME_SUMMARY = "Summary";
            const string WSNAME_Working = "Sheet1";

            const int ROW_HEADER = 1;
            const int COL_HEADER_TITLE = 1;

            const int ROW_GRPHDR = 2;
            const int COL_GRPHDR_SLIPNO = 2;
            const int COL_GRPHDR_STOCKINDATE = 4;
            const int COL_GRPHDR_VOUCHERID = 6;
            const int COL_GRPHDR_VOUCHERDATE = 8;

            const int ROW_GRPHDR2 = 3;
            const int COL_GRPHDR2_SUPPLIER = 2;
            const int COL_GRPHDR2_INVOICENO = 4;
            const int COL_GRPHDR2_TRANSFERTYPE = 6;

            const int ROW_GRPHDR3 = 4;
            const int COL_GRPHDR3_MEMO = 2;

            const int ROW_TBLHDR = 5;
            const int COL_TBLHDR_INSTRUMENTCODE = 2;
            const int COL_TBLHDR_INSTRUMENTCODE_END = 6;

            const int ROW_DTL = 6;
            const int COL_DTL_NO = 1;
            const int COL_DTL_INSTRUMENTCODE = 2;
            const int COL_DTL_INSTRUMENTCODE_END = 6;
            const int COL_DTL_QTY = 7;
            const int COL_DTL_COST = 8;
            const int COL_DTL_TOTAL = 9;

            const int ROW_SUBTTL = 7;
            const int COL_SUBTTL_INSTRUMENTCODE = 2;
            const int COL_SUBTTL_INSTRUMENTCODE_END = 6;
            const int COL_SUBTTL_QTY = 7;
            const int COL_SUBTTL_TOTAL = 9;

            const int COL_MIN = 1;
            const int COL_MAX = 10;

            const int ROW_REPORT_TYPE_TITLE = 5;
            const int COL_REPORT_TYPE_TITLE = 2;

            string strReportTempletePath = PathUtil.GetPathValue(PathUtil.PathName.ReportTempatePath, TEMPLATE_NAME);
            if (!File.Exists(strReportTempletePath))
            {
                throw new FileNotFoundException("Template file not found.", TEMPLATE_NAME);
            }

            string strOutputPath = PathUtil.GetTempFileName(".xlsx");
            using (SLDocument docTemp = new SLDocument(strReportTempletePath))
            using (SLDocument doc = new SLDocument(strReportTempletePath))
            {
                doc.AddWorksheet(WSNAME_Working);
                docTemp.SelectWorksheet(WSNAME_DETAIL);
                doc.SelectWorksheet(WSNAME_Working);

                for (int i = COL_MIN; i <= COL_MAX; i++)
                {
                    doc.SetColumnWidth(i, docTemp.GetColumnWidth(i));
                }

                int rowindex = 1;

                doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_HEADER, COL_MIN, ROW_HEADER, COL_MAX, rowindex, 1);
                doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_HEADER));
                doc.SetCellValue(ROW_HEADER, COL_HEADER_TITLE, string.Format(
                    doc.GetCellValueAsString(ROW_HEADER, COL_HEADER_TITLE),
                    data.Min(d => d.SlipNo),
                    data.Max(d => d.SlipNo),
                    (reportType == "1" ? "MERCHANDISE" : "USER EQUIPMENT")
                ));
                rowindex++;

                var qGroupBySlip = (
                    from d in data
                    group d by new { d.TransferType, d.SlipNo, d.StockInDate } into grp
                    orderby grp.Key.TransferType, grp.Key.SlipNo, grp.Key.StockInDate
                    select grp
                );

                foreach (var slip in qGroupBySlip)
                {
                    var firstobj = slip.First();

                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_GRPHDR, COL_MIN, ROW_GRPHDR, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_GRPHDR));
                    doc.SetCellValue(rowindex, COL_GRPHDR_SLIPNO, slip.Key.SlipNo);
                    doc.SetCellValue(rowindex, COL_GRPHDR_STOCKINDATE, (slip.Key.StockInDate == null ? "" : slip.Key.StockInDate.Value.ToString("dd/MM/yyyy")));
                    doc.SetCellValue(rowindex, COL_GRPHDR_VOUCHERID, firstobj.VoucherID);
                    doc.SetCellValue(rowindex, COL_GRPHDR_VOUCHERDATE, (firstobj.VoucherDate == null ? "" : firstobj.VoucherDate.Value.ToString("dd/MM/yyyy")));
                    rowindex++;

                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_GRPHDR2, COL_MIN, ROW_GRPHDR2, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_GRPHDR2));
                    doc.SetCellValue(rowindex, COL_GRPHDR2_SUPPLIER, firstobj.SupplierNameEN);
                    doc.SetCellValue(rowindex, COL_GRPHDR2_INVOICENO, firstobj.InvoiceNo);
                    doc.SetCellValue(rowindex, COL_GRPHDR2_TRANSFERTYPE, slip.Key.TransferType);
                    rowindex++;

                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_GRPHDR3, COL_MIN, ROW_GRPHDR3, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_GRPHDR3));
                    doc.SetCellValue(rowindex, COL_GRPHDR3_MEMO, firstobj.Memo);
                    rowindex++;

                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_TBLHDR, COL_MIN, ROW_TBLHDR, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_TBLHDR));
                    doc.MergeWorksheetCells(rowindex, COL_TBLHDR_INSTRUMENTCODE, rowindex, COL_TBLHDR_INSTRUMENTCODE_END);
                    doc.SetCellValue(ROW_REPORT_TYPE_TITLE, COL_REPORT_TYPE_TITLE, string.Format(
                        doc.GetCellValueAsString(ROW_REPORT_TYPE_TITLE, COL_REPORT_TYPE_TITLE),
                        (reportType == "1" ? "MERCHANDISE" : "USER EQUIPMENT")
                    )); rowindex++;

                    int firstrow, lastrow;
                    firstrow = lastrow = rowindex;
                    int lineno = 0;
                    foreach (var detail in slip.OrderBy(d => d.InstrumentCode))
                    {
                        lineno++;

                        doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_DTL, COL_MIN, ROW_DTL, COL_MAX, rowindex, 1);
                        doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_DTL));
                        doc.MergeWorksheetCells(rowindex, COL_DTL_INSTRUMENTCODE, rowindex, COL_DTL_INSTRUMENTCODE_END);
                        doc.SetCellValue(rowindex, COL_DTL_NO, lineno);
                        doc.SetCellValue(rowindex, COL_DTL_INSTRUMENTCODE, detail.InstrumentCode);
                        doc.SetCellValue(rowindex, COL_DTL_QTY, (detail.Qty ?? 0));
                        doc.SetCellValue(rowindex, COL_DTL_COST, detail.CostUsD);
                        doc.SetCellValue(rowindex, COL_DTL_TOTAL, (detail.TotalUsD ?? 0));
                        rowindex++;

                        lastrow = firstrow + lineno - 1;
                    }

                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_SUBTTL, COL_MIN, ROW_SUBTTL, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_SUBTTL));
                    doc.MergeWorksheetCells(rowindex, COL_SUBTTL_INSTRUMENTCODE, rowindex, COL_SUBTTL_INSTRUMENTCODE_END);
                    doc.SetCellValue(rowindex, COL_SUBTTL_QTY, string.Format(
                        "=SUM({0}:{1})",
                        SLConvert.ToCellReference(firstrow, COL_SUBTTL_QTY),
                        SLConvert.ToCellReference(lastrow, COL_SUBTTL_QTY)
                    ));
                    doc.SetCellValue(rowindex, COL_SUBTTL_TOTAL, string.Format(
                        "=SUM({0}:{1})",
                        SLConvert.ToCellReference(firstrow, COL_SUBTTL_TOTAL),
                        SLConvert.ToCellReference(lastrow, COL_SUBTTL_TOTAL)
                    ));
                    rowindex++;

                    //Blank Row for group separator.
                    rowindex++;
                }

                doc.DeleteWorksheet(WSNAME_DETAIL);
                doc.DeleteWorksheet(WSNAME_SUMMARY);

                doc.SaveAs(strOutputPath);
                docTemp.CloseWithoutSaving();
            }

            return strOutputPath;
        }

        public string GenerateIVS280InReportSummary(string reportType, List<dtInReportDetail> data)
        {
            const string TEMPLATE_NAME = "IVS280.xlsx";
            const string WSNAME_DETAIL = "Detail";
            const string WSNAME_SUMMARY = "Summary";

            const int ROW_HEADER = 1;
            const int COL_HEADER_TITLE = 1;

            const int ROW_TBLHDR = 2;
            const int COL_TBLHDR_INSTRUMENTCODE = 2;
            const int COL_TBLHDR_INSTRUMENTCODE_END = 6;

            const int ROW_DTL = 3;
            const int COL_DTL_NO = 1;
            const int COL_DTL_INSTRUMENTCODE = 2;
            const int COL_DTL_INSTRUMENTCODE_END = 6;
            const int COL_DTL_QTY = 7;
            const int COL_DTL_COST = 8;
            const int COL_DTL_TOTAL = 9;

            const int ROW_TTL = 4;
            const int COL_TTL_INSTRUMENTCODE = 2;
            const int COL_TTL_INSTRUMENTCODE_END = 6;
            const int COL_TTL_QTY = 7;
            const int COL_TTL_TOTAL = 9;

            const int COL_MIN = 1;
            const int COL_MAX = 10;

            const int ROW_REPORT_TYPE_TITLE = 2;
            const int COL_REPORT_TYPE_TITLE = 2;

            string strReportTempletePath = PathUtil.GetPathValue(PathUtil.PathName.ReportTempatePath, TEMPLATE_NAME);
            if (!File.Exists(strReportTempletePath))
            {
                throw new FileNotFoundException("Template file not found.", TEMPLATE_NAME);
            }

            string strOutputPath = PathUtil.GetTempFileName(".xlsx");
            using (SLDocument docTemp = new SLDocument(strReportTempletePath))
            using (SLDocument doc = new SLDocument(strReportTempletePath))
            {
                var qSummaryTransferType = (
                    from d in data
                    group d by new
                    {
                        IsSpecial = (d.TransferType == "Special"),
                        OrderKey = (d.TransferType == "Special" ? 1 : 0)
                    } into grp
                    orderby grp.Key.OrderKey
                    select grp
                );

                foreach (var grpTransferType in qSummaryTransferType)
                {
                    var qSummary = (
                        from d in grpTransferType
                        group d by new { d.InstrumentCode } into grp
                        orderby grp.Key.InstrumentCode
                        select grp
                    );

                    string wsName = (grpTransferType.Key.IsSpecial ? "Special" : "Normal");
                    doc.AddWorksheet(wsName);
                    docTemp.SelectWorksheet(WSNAME_SUMMARY);
                    doc.SelectWorksheet(wsName);

                    for (int i = COL_MIN; i <= COL_MAX; i++)
                    {
                        doc.SetColumnWidth(i, docTemp.GetColumnWidth(i));
                    }

                    int rowindex = 1;

                    doc.CopyCellFromWorksheet(WSNAME_SUMMARY, ROW_HEADER, COL_MIN, ROW_HEADER, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_HEADER));
                    doc.SetCellValue(ROW_HEADER, COL_HEADER_TITLE, string.Format(
                        doc.GetCellValueAsString(ROW_HEADER, COL_HEADER_TITLE),
                        data.Min(d => d.SlipNo),
                        data.Max(d => d.SlipNo),
                        (reportType == "1" ? "MERCHANDISE" : "USER EQUIPMENT"),
                        (grpTransferType.Key.IsSpecial ? "(SPECIAL)" : "")
                    ));
                    rowindex++;

                    doc.CopyCellFromWorksheet(WSNAME_SUMMARY, ROW_TBLHDR, COL_MIN, ROW_TBLHDR, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_TBLHDR));
                    doc.MergeWorksheetCells(rowindex, COL_TBLHDR_INSTRUMENTCODE, rowindex, COL_TBLHDR_INSTRUMENTCODE_END);
                    doc.SetCellValue(ROW_REPORT_TYPE_TITLE, COL_REPORT_TYPE_TITLE, string.Format(
                        doc.GetCellValueAsString(ROW_REPORT_TYPE_TITLE, COL_REPORT_TYPE_TITLE),
                        (reportType == "1" ? "MERCHANDISE" : "USER EQUIPMENT")
                    ));
                    rowindex++;

                    int firstrow, lastrow;
                    firstrow = lastrow = rowindex;
                    int lineno = 0;
                    foreach (var detail in qSummary)
                    {
                        lineno++;

                        doc.CopyCellFromWorksheet(WSNAME_SUMMARY, ROW_DTL, COL_MIN, ROW_DTL, COL_MAX, rowindex, 1);
                        doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_DTL));
                        doc.MergeWorksheetCells(rowindex, COL_DTL_INSTRUMENTCODE, rowindex, COL_DTL_INSTRUMENTCODE_END);
                        doc.SetCellValue(rowindex, COL_DTL_NO, lineno);
                        doc.SetCellValue(rowindex, COL_DTL_INSTRUMENTCODE, detail.Key.InstrumentCode);

                        int qty = detail.Sum(d => d.Qty ?? 0);
                        decimal total = detail.Sum(d => d.TotalUsD ?? 0);

                        doc.SetCellValue(rowindex, COL_DTL_QTY, qty);
                        doc.SetCellValue(rowindex, COL_DTL_COST, string.Format(
                            "={0}/{1}",
                            SLConvert.ToCellReference(rowindex, COL_DTL_TOTAL),
                            SLConvert.ToCellReference(rowindex, COL_DTL_QTY)
                        ));
                        doc.SetCellValue(rowindex, COL_DTL_TOTAL, total);
                        rowindex++;

                        lastrow = firstrow + lineno - 1;
                    }

                    doc.CopyCellFromWorksheet(WSNAME_SUMMARY, ROW_TTL, COL_MIN, ROW_TTL, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_TTL));
                    doc.MergeWorksheetCells(rowindex, COL_TTL_INSTRUMENTCODE, rowindex, COL_TTL_INSTRUMENTCODE_END);
                    doc.SetCellValue(rowindex, COL_TTL_QTY, string.Format(
                        "=SUM({0}:{1})",
                        SLConvert.ToCellReference(firstrow, COL_TTL_QTY),
                        SLConvert.ToCellReference(lastrow, COL_TTL_QTY)
                    ));
                    doc.SetCellValue(rowindex, COL_TTL_TOTAL, string.Format(
                        "=SUM({0}:{1})",
                        SLConvert.ToCellReference(firstrow, COL_TTL_TOTAL),
                        SLConvert.ToCellReference(lastrow, COL_TTL_TOTAL)
                    ));
                }

                doc.DeleteWorksheet(WSNAME_DETAIL);
                doc.DeleteWorksheet(WSNAME_SUMMARY);
                doc.SelectWorksheet(doc.GetWorksheetNames().First());

                doc.SaveAs(strOutputPath);
                docTemp.CloseWithoutSaving();
            }

            return strOutputPath;
        }

        public string GenerateIVS281OutReport(string reportType, List<dtOutReportDetail> data)
        {
            const string TEMPLATE_NAME = "IVS281.xlsx";
            const string WSNAME_DETAIL = "Detail";
            const string WSNAME_SUMMARY = "Summary";
            const string WSNAME_Working = "Sheet1";

            const int ROW_HEADER = 1;
            const int COL_HEADER_TITLE = 1;

            const int ROW_GRPHDR1 = 2;
            const int COL_GRPHDR1_SLIPNO = 2;
            const int COL_GRPHDR1_STOCKOUTDATE = 4;
            const int COL_GRPHDR1_CUSTNAME = 6;

            const int ROW_GRPHDR2 = 3;
            const int COL_GRPHDR2_CONTRACTCODE = 2;
            const int COL_GRPHDR2_OPERATEDATE = 4;
            const int COL_GRPHDR2_SITENAME = 6;

            const int ROW_GRPHDR3 = 4;
            const int COL_GRPHDR3_MEMO = 2;
            const int COL_GRPHDR3_TRANSFERTYPE = 9;

            const int ROW_TBLHDR = 5;
            const int COL_TBLHDR_INSTRUMENTCODE = 2;
            const int COL_TBLHDR_INSTRUMENTCODE_END = 6;

            const int ROW_DTL = 6;
            const int COL_DTL_NO = 1;
            const int COL_DTL_INSTRUMENTCODE = 2;
            const int COL_DTL_INSTRUMENTCODE_END = 6;
            const int COL_DTL_QTY = 7;
            const int COL_DTL_COST = 8;
            const int COL_DTL_TOTAL = 9;

            const int ROW_SUBTTL = 7;
            const int COL_SUBTTL_INSTRUMENTCODE = 2;
            const int COL_SUBTTL_INSTRUMENTCODE_END = 6;
            const int COL_SUBTTL_QTY = 7;
            const int COL_SUBTTL_TOTAL = 9;

            const int COL_MIN = 1;
            const int COL_MAX = 10;

            const int ROW_REPORT_TYPE_TITLE = 5;
            const int COL_REPORT_TYPE_TITLE = 2;


            var commonUtil = new CommonUtil();

            string strReportTempletePath = PathUtil.GetPathValue(PathUtil.PathName.ReportTempatePath, TEMPLATE_NAME);
            if (!File.Exists(strReportTempletePath))
            {
                throw new FileNotFoundException("Template file not found.", TEMPLATE_NAME);
            }

            string strOutputPath = PathUtil.GetTempFileName(".xlsx");
            using (SLDocument docTemp = new SLDocument(strReportTempletePath))
            using (SLDocument doc = new SLDocument(strReportTempletePath))
            {
                doc.AddWorksheet(WSNAME_Working);
                docTemp.SelectWorksheet(WSNAME_DETAIL);
                doc.SelectWorksheet(WSNAME_Working);

                for (int i = COL_MIN; i <= COL_MAX; i++)
                {
                    doc.SetColumnWidth(i, docTemp.GetColumnWidth(i));
                }

                int rowindex = 1;

                doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_HEADER, COL_MIN, ROW_HEADER, COL_MAX, rowindex, 1);
                doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_HEADER));
                doc.SetCellValue(ROW_HEADER, COL_HEADER_TITLE, string.Format(
                    doc.GetCellValueAsString(ROW_HEADER, COL_HEADER_TITLE),
                    data.Min(d => d.SlipNo),
                    data.Max(d => d.SlipNo),
                    (reportType == "1" ? "MERCHANDISE" : "USER EQUIPMENT")
                ));
                rowindex++;

                var qGroupBySlip = (
                    from d in data
                    group d by new
                    {
                        d.TransferType,
                        d.SlipNo,
                        d.StockOutDate,
                    } into grp
                    orderby grp.Key.TransferType, grp.Key.SlipNo, grp.Key.StockOutDate
                    select grp
                );

                foreach (var slip in qGroupBySlip)
                {
                    var firstobj = slip.First();

                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_GRPHDR1, COL_MIN, ROW_GRPHDR1, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_GRPHDR1));
                    doc.SetCellValue(rowindex, COL_GRPHDR1_SLIPNO, slip.Key.SlipNo);
                    doc.SetCellValue(rowindex, COL_GRPHDR1_STOCKOUTDATE, (slip.Key.StockOutDate == null ? "" : slip.Key.StockOutDate.Value.ToString("dd/MM/yyyy")));
                    doc.SetCellValue(rowindex, COL_GRPHDR1_CUSTNAME, firstobj.CustNameEN);
                    rowindex++;

                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_GRPHDR2, COL_MIN, ROW_GRPHDR2, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_GRPHDR2));
                    doc.SetCellValue(rowindex, COL_GRPHDR2_CONTRACTCODE, commonUtil.ConvertContractCode(firstobj.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT));
                    doc.SetCellValue(rowindex, COL_GRPHDR2_OPERATEDATE, (firstobj.OperateDate == null ? "" : firstobj.OperateDate.Value.ToString("dd/MM/yyyy")));
                    doc.SetCellValue(rowindex, COL_GRPHDR2_SITENAME, firstobj.SiteName);
                    rowindex++;

                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_GRPHDR3, COL_MIN, ROW_GRPHDR3, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_GRPHDR3));
                    doc.SetCellValue(rowindex, COL_GRPHDR3_MEMO, firstobj.Memo);
                    doc.SetCellValue(rowindex, COL_GRPHDR3_TRANSFERTYPE, slip.Key.TransferType);
                    rowindex++;

                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_TBLHDR, COL_MIN, ROW_TBLHDR, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_TBLHDR));
                    doc.MergeWorksheetCells(rowindex, COL_TBLHDR_INSTRUMENTCODE, rowindex, COL_TBLHDR_INSTRUMENTCODE_END);
                    doc.SetCellValue(ROW_REPORT_TYPE_TITLE, COL_REPORT_TYPE_TITLE, string.Format(
                        doc.GetCellValueAsString(ROW_REPORT_TYPE_TITLE, COL_REPORT_TYPE_TITLE),
                        (reportType == "1" ? "MERCHANDISE" : "USER EQUIPMENT")
                    ));
                    rowindex++;

                    int firstrow, lastrow;
                    firstrow = lastrow = rowindex;
                    int lineno = 0;
                    foreach (var detail in slip.OrderBy(d => d.InstrumentCode))
                    {
                        lineno++;

                        doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_DTL, COL_MIN, ROW_DTL, COL_MAX, rowindex, 1);
                        doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_DTL));
                        doc.MergeWorksheetCells(rowindex, COL_DTL_INSTRUMENTCODE, rowindex, COL_DTL_INSTRUMENTCODE_END);
                        doc.SetCellValue(rowindex, COL_DTL_NO, lineno);
                        doc.SetCellValue(rowindex, COL_DTL_INSTRUMENTCODE, detail.InstrumentCode);
                        doc.SetCellValue(rowindex, COL_DTL_QTY, (detail.Qty ?? 0));
                        doc.SetCellValue(rowindex, COL_DTL_COST, detail.Cost);
                        doc.SetCellValue(rowindex, COL_DTL_TOTAL, (detail.Total ?? 0));
                        rowindex++;

                        lastrow = firstrow + lineno - 1;
                    }

                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_SUBTTL, COL_MIN, ROW_SUBTTL, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_SUBTTL));
                    doc.MergeWorksheetCells(rowindex, COL_SUBTTL_INSTRUMENTCODE, rowindex, COL_SUBTTL_INSTRUMENTCODE_END);
                    doc.SetCellValue(rowindex, COL_SUBTTL_QTY, string.Format(
                        "=SUM({0}:{1})",
                        SLConvert.ToCellReference(firstrow, COL_SUBTTL_QTY),
                        SLConvert.ToCellReference(lastrow, COL_SUBTTL_QTY)
                    ));
                    doc.SetCellValue(rowindex, COL_SUBTTL_TOTAL, string.Format(
                        "=SUM({0}:{1})",
                        SLConvert.ToCellReference(firstrow, COL_SUBTTL_TOTAL),
                        SLConvert.ToCellReference(lastrow, COL_SUBTTL_TOTAL)
                    ));
                    rowindex++;

                    //Blank Row for group separator.
                    rowindex++;
                }

                doc.DeleteWorksheet(WSNAME_DETAIL);
                doc.DeleteWorksheet(WSNAME_SUMMARY);

                doc.SaveAs(strOutputPath);
                docTemp.CloseWithoutSaving();
            }

            return strOutputPath;
        }

        public string GenerateIVS281OutReportSummary(string reportType, List<dtOutReportDetail> data)
        {
            const string TEMPLATE_NAME = "IVS281.xlsx";
            const string WSNAME_DETAIL = "Detail";
            const string WSNAME_SUMMARY = "Summary";

            const int ROW_HEADER = 1;
            const int COL_HEADER_TITLE = 1;

            const int ROW_TBLHDR = 2;
            const int COL_TBLHDR_INSTRUMENTCODE = 2;
            const int COL_TBLHDR_INSTRUMENTCODE_END = 6;

            const int ROW_DTL = 3;
            const int COL_DTL_NO = 1;
            const int COL_DTL_INSTRUMENTCODE = 2;
            const int COL_DTL_INSTRUMENTCODE_END = 6;
            const int COL_DTL_QTY = 7;
            const int COL_DTL_COST = 8;
            const int COL_DTL_TOTAL = 9;

            const int ROW_TTL = 4;
            const int COL_TTL_INSTRUMENTCODE = 2;
            const int COL_TTL_INSTRUMENTCODE_END = 6;
            const int COL_TTL_QTY = 7;
            const int COL_TTL_TOTAL = 9;

            const int COL_MIN = 1;
            const int COL_MAX = 10;

            const int ROW_REPORT_TYPE_TITLE = 2;
            const int COL_REPORT_TYPE_TITLE = 2;

            string strReportTempletePath = PathUtil.GetPathValue(PathUtil.PathName.ReportTempatePath, TEMPLATE_NAME);
            if (!File.Exists(strReportTempletePath))
            {
                throw new FileNotFoundException("Template file not found.", TEMPLATE_NAME);
            }

            string strOutputPath = PathUtil.GetTempFileName(".xlsx");
            using (SLDocument docTemp = new SLDocument(strReportTempletePath))
            using (SLDocument doc = new SLDocument(strReportTempletePath))
            {
                var qSummaryTransferType = (
                    from d in data
                    group d by new
                    {
                        IsSpecial = (d.TransferType == "Special"),
                        OrderKey = (d.TransferType == "Special" ? 1 : 0)
                    } into grp
                    orderby grp.Key.OrderKey
                    select grp
                );

                foreach (var grpTransferType in qSummaryTransferType)
                {
                    var qSummary = (
                        from d in grpTransferType
                        group d by new { d.InstrumentCode } into grp
                        orderby grp.Key.InstrumentCode
                        select grp
                    );

                    string wsName = (grpTransferType.Key.IsSpecial ? "Special" : "Normal");
                    doc.AddWorksheet(wsName);
                    docTemp.SelectWorksheet(WSNAME_SUMMARY);
                    doc.SelectWorksheet(wsName);

                    for (int i = COL_MIN; i <= COL_MAX; i++)
                    {
                        doc.SetColumnWidth(i, docTemp.GetColumnWidth(i));
                    }

                    int rowindex = 1;

                    doc.CopyCellFromWorksheet(WSNAME_SUMMARY, ROW_HEADER, COL_MIN, ROW_HEADER, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_HEADER));
                    doc.SetCellValue(ROW_HEADER, COL_HEADER_TITLE, string.Format(
                        doc.GetCellValueAsString(ROW_HEADER, COL_HEADER_TITLE),
                        data.Min(d => d.SlipNo),
                        data.Max(d => d.SlipNo),
                        (reportType == "1" ? "MERCHANDISE" : "USER EQUIPMENT"),
                        (grpTransferType.Key.IsSpecial ? "(SPECIAL)" : "")
                    ));
                    rowindex++;

                    doc.CopyCellFromWorksheet(WSNAME_SUMMARY, ROW_TBLHDR, COL_MIN, ROW_TBLHDR, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_TBLHDR));
                    doc.MergeWorksheetCells(rowindex, COL_TBLHDR_INSTRUMENTCODE, rowindex, COL_TBLHDR_INSTRUMENTCODE_END);
                    doc.SetCellValue(ROW_REPORT_TYPE_TITLE, COL_REPORT_TYPE_TITLE, string.Format(
                        doc.GetCellValueAsString(ROW_REPORT_TYPE_TITLE, COL_REPORT_TYPE_TITLE),
                        (reportType == "1" ? "MERCHANDISE" : "USER EQUIPMENT")
                    ));
                    rowindex++;

                    int firstrow, lastrow;
                    firstrow = lastrow = rowindex;
                    int lineno = 0;
                    foreach (var detail in qSummary)
                    {
                        lineno++;

                        doc.CopyCellFromWorksheet(WSNAME_SUMMARY, ROW_DTL, COL_MIN, ROW_DTL, COL_MAX, rowindex, 1);
                        doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_DTL));
                        doc.MergeWorksheetCells(rowindex, COL_DTL_INSTRUMENTCODE, rowindex, COL_DTL_INSTRUMENTCODE_END);
                        doc.SetCellValue(rowindex, COL_DTL_NO, lineno);
                        doc.SetCellValue(rowindex, COL_DTL_INSTRUMENTCODE, detail.Key.InstrumentCode);

                        int qty = detail.Sum(d => d.Qty ?? 0);
                        decimal total = detail.Sum(d => d.TotalUsD ?? 0);

                        doc.SetCellValue(rowindex, COL_DTL_QTY, qty);
                        doc.SetCellValue(rowindex, COL_DTL_COST, string.Format(
                            "={0}/{1}",
                            SLConvert.ToCellReference(rowindex, COL_DTL_TOTAL),
                            SLConvert.ToCellReference(rowindex, COL_DTL_QTY)
                        ));
                        doc.SetCellValue(rowindex, COL_DTL_TOTAL, total);
                        rowindex++;

                        lastrow = firstrow + lineno - 1;
                    }

                    doc.CopyCellFromWorksheet(WSNAME_SUMMARY, ROW_TTL, COL_MIN, ROW_TTL, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_TTL));
                    doc.MergeWorksheetCells(rowindex, COL_TTL_INSTRUMENTCODE, rowindex, COL_TTL_INSTRUMENTCODE_END);
                    doc.SetCellValue(rowindex, COL_TTL_QTY, string.Format(
                        "=SUM({0}:{1})",
                        SLConvert.ToCellReference(firstrow, COL_TTL_QTY),
                        SLConvert.ToCellReference(lastrow, COL_TTL_QTY)
                    ));
                    doc.SetCellValue(rowindex, COL_TTL_TOTAL, string.Format(
                        "=SUM({0}:{1})",
                        SLConvert.ToCellReference(firstrow, COL_TTL_TOTAL),
                        SLConvert.ToCellReference(lastrow, COL_TTL_TOTAL)
                    ));
                }

                doc.DeleteWorksheet(WSNAME_DETAIL);
                doc.DeleteWorksheet(WSNAME_SUMMARY);
                doc.SelectWorksheet(doc.GetWorksheetNames().First());

                doc.SaveAs(strOutputPath);
                docTemp.CloseWithoutSaving();
            }

            return strOutputPath;
        }

        public string GenerateIVS282ReturnReport(string reportType, List<dtReturnReportDetail> data)
        {
            const string TEMPLATE_NAME = "IVS282.xlsx";
            const string WSNAME_DETAIL = "Detail";
            const string WSNAME_SUMMARY = "Summary";
            const string WSNAME_Working = "Sheet1";

            const int ROW_HEADER = 1;
            const int COL_HEADER_TITLE = 1;

            const int ROW_GRPHDR1 = 2;
            const int COL_GRPHDR1_SLIPNO = 2;
            const int COL_GRPHDR1_RETURNDATE = 4;
            const int COL_GRPHDR1_CUSTNAME = 6;

            const int ROW_GRPHDR2 = 3;
            const int COL_GRPHDR2_CONTRACTCODE = 2;
            const int COL_GRPHDR2_OPERATEDATE = 4;

            const int ROW_TBLHDR = 4;
            const int COL_TBLHDR_INSTRUMENTCODE = 2;
            const int COL_TBLHDR_INSTRUMENTCODE_END = 6;

            const int ROW_DTL = 5;
            const int COL_DTL_NO = 1;
            const int COL_DTL_INSTRUMENTCODE = 2;
            const int COL_DTL_INSTRUMENTCODE_END = 6;
            const int COL_DTL_QTY = 7;
            const int COL_DTL_COST = 8;
            const int COL_DTL_TOTAL = 9;

            const int ROW_SUBTTL = 6;
            const int COL_SUBTTL_INSTRUMENTCODE = 2;
            const int COL_SUBTTL_INSTRUMENTCODE_END = 6;
            const int COL_SUBTTL_QTY = 7;
            const int COL_SUBTTL_TOTAL = 9;

            const int COL_MIN = 1;
            const int COL_MAX = 10;

            const int ROW_REPORT_TYPE_TITLE = 4;
            const int COL_REPORT_TYPE_TITLE = 2;


            var commonUtil = new CommonUtil();

            string strReportTempletePath = PathUtil.GetPathValue(PathUtil.PathName.ReportTempatePath, TEMPLATE_NAME);
            if (!File.Exists(strReportTempletePath))
            {
                throw new FileNotFoundException("Template file not found.", TEMPLATE_NAME);
            }

            string strOutputPath = PathUtil.GetTempFileName(".xlsx");
            using (SLDocument docTemp = new SLDocument(strReportTempletePath))
            using (SLDocument doc = new SLDocument(strReportTempletePath))
            {
                doc.AddWorksheet(WSNAME_Working);
                docTemp.SelectWorksheet(WSNAME_DETAIL);
                doc.SelectWorksheet(WSNAME_Working);

                for (int i = COL_MIN; i <= COL_MAX; i++)
                {
                    doc.SetColumnWidth(i, docTemp.GetColumnWidth(i));
                }

                int rowindex = 1;

                doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_HEADER, COL_MIN, ROW_HEADER, COL_MAX, rowindex, 1);
                doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_HEADER));
                doc.SetCellValue(ROW_HEADER, COL_HEADER_TITLE, string.Format(
                    doc.GetCellValueAsString(ROW_HEADER, COL_HEADER_TITLE),
                    data.Min(d => d.SlipNo),
                    data.Max(d => d.SlipNo),
                    (reportType == "1" ? "MERCHANDISE" : "USER EQUIPMENT")
                ));
                rowindex++;

                var qGroupBySlip = (
                    from d in data
                    group d by new
                    {
                        d.SlipNo,
                        ReturnDate = d.StockOutDate,
                        CustName = d.CustNameLC,
                        d.ContractCode,
                        d.OperateDate,
                    } into grp
                    orderby grp.Key.SlipNo, grp.Key.ReturnDate
                    select grp
                );

                foreach (var slip in qGroupBySlip)
                {
                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_GRPHDR1, COL_MIN, ROW_GRPHDR1, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_GRPHDR1));
                    doc.SetCellValue(rowindex, COL_GRPHDR1_SLIPNO, slip.Key.SlipNo);
                    doc.SetCellValue(rowindex, COL_GRPHDR1_RETURNDATE, (slip.Key.ReturnDate == null ? "" : slip.Key.ReturnDate.Value.ToString("dd/MM/yyyy")));
                    doc.SetCellValue(rowindex, COL_GRPHDR1_CUSTNAME, slip.Key.CustName);
                    rowindex++;

                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_GRPHDR2, COL_MIN, ROW_GRPHDR2, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_GRPHDR2));
                    doc.SetCellValue(rowindex, COL_GRPHDR2_CONTRACTCODE, commonUtil.ConvertContractCode(slip.Key.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT));
                    doc.SetCellValue(rowindex, COL_GRPHDR2_OPERATEDATE, (slip.Key.OperateDate == null ? "" : slip.Key.OperateDate.Value.ToString("dd/MM/yyyy")));
                    rowindex++;

                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_TBLHDR, COL_MIN, ROW_TBLHDR, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_TBLHDR));
                    doc.MergeWorksheetCells(rowindex, COL_TBLHDR_INSTRUMENTCODE, rowindex, COL_TBLHDR_INSTRUMENTCODE_END);
                    doc.SetCellValue(ROW_REPORT_TYPE_TITLE, COL_REPORT_TYPE_TITLE, string.Format(
                        doc.GetCellValueAsString(ROW_REPORT_TYPE_TITLE, COL_REPORT_TYPE_TITLE),
                        (reportType == "1" ? "MERCHANDISE" : "USER EQUIPMENT")
                    ));
                    rowindex++;

                    int firstrow, lastrow;
                    firstrow = lastrow = rowindex;
                    int lineno = 0;
                    foreach (var detail in slip.OrderBy(d => d.InstrumentCode))
                    {
                        lineno++;

                        doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_DTL, COL_MIN, ROW_DTL, COL_MAX, rowindex, 1);
                        doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_DTL));
                        doc.MergeWorksheetCells(rowindex, COL_DTL_INSTRUMENTCODE, rowindex, COL_DTL_INSTRUMENTCODE_END);
                        doc.SetCellValue(rowindex, COL_DTL_NO, lineno);
                        doc.SetCellValue(rowindex, COL_DTL_INSTRUMENTCODE, detail.InstrumentCode);
                        doc.SetCellValue(rowindex, COL_DTL_QTY, (detail.Qty ?? 0));
                        doc.SetCellValue(rowindex, COL_DTL_COST, detail.CostUsD);
                        doc.SetCellValue(rowindex, COL_DTL_TOTAL, (detail.TotalUsD ?? 0));
                        rowindex++;

                        lastrow = firstrow + lineno - 1;
                    }

                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_SUBTTL, COL_MIN, ROW_SUBTTL, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_SUBTTL));
                    doc.MergeWorksheetCells(rowindex, COL_SUBTTL_INSTRUMENTCODE, rowindex, COL_SUBTTL_INSTRUMENTCODE_END);
                    doc.SetCellValue(rowindex, COL_SUBTTL_QTY, string.Format(
                        "=SUM({0}:{1})",
                        SLConvert.ToCellReference(firstrow, COL_SUBTTL_QTY),
                        SLConvert.ToCellReference(lastrow, COL_SUBTTL_QTY)
                    ));
                    doc.SetCellValue(rowindex, COL_SUBTTL_TOTAL, string.Format(
                        "=SUM({0}:{1})",
                        SLConvert.ToCellReference(firstrow, COL_SUBTTL_TOTAL),
                        SLConvert.ToCellReference(lastrow, COL_SUBTTL_TOTAL)
                    ));
                    rowindex++;

                    //Blank Row for group separator.
                    rowindex++;
                }

                doc.DeleteWorksheet(WSNAME_DETAIL);
                doc.DeleteWorksheet(WSNAME_SUMMARY);

                doc.SaveAs(strOutputPath);
                docTemp.CloseWithoutSaving();
            }

            return strOutputPath;
        }

        public string GenerateIVS282ReturnReportSummary(string reportType, List<dtReturnReportDetail> data)
        {
            const string TEMPLATE_NAME = "IVS282.xlsx";
            const string WSNAME_DETAIL = "Detail";
            const string WSNAME_SUMMARY = "Summary";
            const string WSNAME_Working = "Sheet1";

            const int ROW_HEADER = 1;
            const int COL_HEADER_TITLE = 1;

            const int ROW_TBLHDR = 2;
            const int COL_TBLHDR_INSTRUMENTCODE = 2;
            const int COL_TBLHDR_INSTRUMENTCODE_END = 6;

            const int ROW_DTL = 3;
            const int COL_DTL_NO = 1;
            const int COL_DTL_INSTRUMENTCODE = 2;
            const int COL_DTL_INSTRUMENTCODE_END = 6;
            const int COL_DTL_QTY = 7;
            const int COL_DTL_COST = 8;
            const int COL_DTL_TOTAL = 9;

            const int ROW_TTL = 4;
            const int COL_TTL_INSTRUMENTCODE = 2;
            const int COL_TTL_INSTRUMENTCODE_END = 6;
            const int COL_TTL_QTY = 7;
            const int COL_TTL_TOTAL = 9;

            const int COL_MIN = 1;
            const int COL_MAX = 10;

            const int ROW_REPORT_TYPE_TITLE = 2;
            const int COL_REPORT_TYPE_TITLE = 2;


            string strReportTempletePath = PathUtil.GetPathValue(PathUtil.PathName.ReportTempatePath, TEMPLATE_NAME);
            if (!File.Exists(strReportTempletePath))
            {
                throw new FileNotFoundException("Template file not found.", TEMPLATE_NAME);
            }

            string strOutputPath = PathUtil.GetTempFileName(".xlsx");
            using (SLDocument docTemp = new SLDocument(strReportTempletePath))
            using (SLDocument doc = new SLDocument(strReportTempletePath))
            {
                doc.AddWorksheet(WSNAME_Working);
                docTemp.SelectWorksheet(WSNAME_SUMMARY);
                doc.SelectWorksheet(WSNAME_Working);

                for (int i = COL_MIN; i <= COL_MAX; i++)
                {
                    doc.SetColumnWidth(i, docTemp.GetColumnWidth(i));
                }

                int rowindex = 1;

                doc.CopyCellFromWorksheet(WSNAME_SUMMARY, ROW_HEADER, COL_MIN, ROW_HEADER, COL_MAX, rowindex, 1);
                doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_HEADER));
                doc.SetCellValue(ROW_HEADER, COL_HEADER_TITLE, string.Format(
                    doc.GetCellValueAsString(ROW_HEADER, COL_HEADER_TITLE),
                    data.Min(d => d.SlipNo),
                    data.Max(d => d.SlipNo),
                    (reportType == "1" ? "MERCHANDISE" : "USER EQUIPMENT")
                ));
                rowindex++;

                var qSummary = (
                    from d in data
                    group d by new { d.InstrumentCode } into grp
                    orderby grp.Key.InstrumentCode
                    select grp
                );

                doc.CopyCellFromWorksheet(WSNAME_SUMMARY, ROW_TBLHDR, COL_MIN, ROW_TBLHDR, COL_MAX, rowindex, 1);
                doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_TBLHDR));
                doc.MergeWorksheetCells(rowindex, COL_TBLHDR_INSTRUMENTCODE, rowindex, COL_TBLHDR_INSTRUMENTCODE_END);
                doc.SetCellValue(ROW_REPORT_TYPE_TITLE, COL_REPORT_TYPE_TITLE, string.Format(
                    doc.GetCellValueAsString(ROW_REPORT_TYPE_TITLE, COL_REPORT_TYPE_TITLE),
                    (reportType == "1" ? "MERCHANDISE" : "USER EQUIPMENT")
                ));
                rowindex++;

                int firstrow, lastrow;
                firstrow = lastrow = rowindex;
                int lineno = 0;
                foreach (var detail in qSummary)
                {
                    lineno++;

                    doc.CopyCellFromWorksheet(WSNAME_SUMMARY, ROW_DTL, COL_MIN, ROW_DTL, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_DTL));
                    doc.MergeWorksheetCells(rowindex, COL_DTL_INSTRUMENTCODE, rowindex, COL_DTL_INSTRUMENTCODE_END);
                    doc.SetCellValue(rowindex, COL_DTL_NO, lineno);
                    doc.SetCellValue(rowindex, COL_DTL_INSTRUMENTCODE, detail.Key.InstrumentCode);

                    int qty = detail.Sum(d => d.Qty ?? 0);
                    decimal total = detail.Sum(d => d.TotalUsD ?? 0);

                    doc.SetCellValue(rowindex, COL_DTL_QTY, qty);
                    doc.SetCellValue(rowindex, COL_DTL_COST, string.Format(
                        "={0}/{1}",
                        SLConvert.ToCellReference(rowindex, COL_DTL_TOTAL),
                        SLConvert.ToCellReference(rowindex, COL_DTL_QTY)
                    ));
                    doc.SetCellValue(rowindex, COL_DTL_TOTAL, total);
                    rowindex++;

                    lastrow = firstrow + lineno - 1;
                }

                doc.CopyCellFromWorksheet(WSNAME_SUMMARY, ROW_TTL, COL_MIN, ROW_TTL, COL_MAX, rowindex, 1);
                doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_TTL));
                doc.MergeWorksheetCells(rowindex, COL_TTL_INSTRUMENTCODE, rowindex, COL_TTL_INSTRUMENTCODE_END);
                doc.SetCellValue(rowindex, COL_TTL_QTY, string.Format(
                    "=SUM({0}:{1})",
                    SLConvert.ToCellReference(firstrow, COL_TTL_QTY),
                    SLConvert.ToCellReference(lastrow, COL_TTL_QTY)
                ));
                doc.SetCellValue(rowindex, COL_TTL_TOTAL, string.Format(
                    "=SUM({0}:{1})",
                    SLConvert.ToCellReference(firstrow, COL_TTL_TOTAL),
                    SLConvert.ToCellReference(lastrow, COL_TTL_TOTAL)
                ));


                doc.DeleteWorksheet(WSNAME_DETAIL);
                doc.DeleteWorksheet(WSNAME_SUMMARY);

                doc.SaveAs(strOutputPath);
                docTemp.CloseWithoutSaving();
            }

            return strOutputPath;
        }

        public string GenerateIVS283MovementReport(string reportType, List<dtMovementReport> data)
        {
            const string TEMPLATE_NAME = "IVS283.xlsx";
            const string WSNAME_DETAIL = "Detail";
            const string WSNAME_Working = "Sheet1";

            const int ROW_HEADER = 1;
            const int COL_HEADER_TITLE = 1;

            const int ROW_GRPHDR = 2;
            const int COL_GRPHDR_INSTRUMENTCODE = 2;

            const int ROW_TBLHDR = 3;

            const int ROW_DTL = 4;
            const int COL_DTL_NO = 1;
            const int COL_DTL_DATE = 2;
            const int COL_DTL_IN = 3;
            const int COL_DTL_OUT = 4;
            const int COL_DTL_RETURN = 5;
            const int COL_DTL_BALQTY = 6;
            const int COL_DTL_COST = 7;
            const int COL_DTL_TCOST = 8;
            const int COL_DTL_BALCOST = 9;
            const int COL_DTL_SLIP = 10;

            const int ROW_SUBTTL = 5;
            const int COL_SUBTTL_IN = 3;
            const int COL_SUBTTL_OUT = 4;
            const int COL_SUBTTL_RETURN = 5;
            const int COL_SUBTTL_BALQTY = 6;
            const int COL_SUBTTL_BALCOST = 9;

            const int COL_MIN = 1;
            const int COL_MAX = 10;

            var commonUtil = new CommonUtil();

            string strReportTempletePath = PathUtil.GetPathValue(PathUtil.PathName.ReportTempatePath, TEMPLATE_NAME);
            if (!File.Exists(strReportTempletePath))
            {
                throw new FileNotFoundException("Template file not found.", TEMPLATE_NAME);
            }

            string strOutputPath = PathUtil.GetTempFileName(".xlsx");
            using (SLDocument docTemp = new SLDocument(strReportTempletePath))
            using (SLDocument doc = new SLDocument(strReportTempletePath))
            {
                doc.AddWorksheet(WSNAME_Working);
                docTemp.SelectWorksheet(WSNAME_DETAIL);
                doc.SelectWorksheet(WSNAME_Working);

                for (int i = COL_MIN; i <= COL_MAX; i++)
                {
                    doc.SetColumnWidth(i, docTemp.GetColumnWidth(i));
                }

                int rowindex = 1;

                doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_HEADER, COL_MIN, ROW_HEADER, COL_MAX, rowindex, 1);
                doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_HEADER));
                doc.SetCellValue(ROW_HEADER, COL_HEADER_TITLE, string.Format(
                    doc.GetCellValueAsString(ROW_HEADER, COL_HEADER_TITLE),
                    DateTime.Today.ToString("MMMM yyyy").ToUpper(),
                    (reportType == "1" ? "MERCHANDISE" : "USER EQUIPMENT")
                ));
                rowindex++;

                var qGroupByInstrument = (
                    from d in data
                    group d by new
                    {
                        d.InstrumentCode
                    } into grp
                    orderby grp.Key.InstrumentCode
                    select grp
                );

                foreach (var instrument in qGroupByInstrument)
                {
                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_GRPHDR, COL_MIN, ROW_GRPHDR, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_GRPHDR));
                    doc.SetCellValue(rowindex, COL_GRPHDR_INSTRUMENTCODE, instrument.Key.InstrumentCode);
                    rowindex++;

                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_TBLHDR, COL_MIN, ROW_TBLHDR, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_TBLHDR));
                    rowindex++;

                    int firstrow, lastrow;
                    firstrow = lastrow = rowindex;
                    int lineno = 0;
                    foreach (var detail in instrument.OrderBy(d => d.RNum))
                    {
                        lineno++;

                        doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_DTL, COL_MIN, ROW_DTL, COL_MAX, rowindex, 1);
                        doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_DTL));

                        doc.SetCellValue(rowindex, COL_DTL_NO, lineno);
                        doc.SetCellValue(rowindex, COL_DTL_DATE, (detail.TransferDate == null ? "" : detail.TransferDate.Value.ToString("dd/MM/yyyy")));
                        doc.SetCellValue(rowindex, COL_DTL_IN, (detail.QtyIn ?? 0));
                        doc.SetCellValue(rowindex, COL_DTL_OUT, (detail.QtyOut ?? 0));
                        doc.SetCellValue(rowindex, COL_DTL_RETURN, (detail.QtyReturn ?? 0));
                        doc.SetCellValue(rowindex, COL_DTL_COST, (detail.CostUsD ?? 0));

                        if (lineno == 1)
                        {
                            doc.SetCellValue(rowindex, COL_DTL_BALQTY, detail.MoveType * (detail.TransferQty ?? 0));
                            doc.SetCellValue(rowindex, COL_DTL_COST, (detail.CostUsD ?? 0));
                            doc.SetCellValue(rowindex, COL_DTL_TCOST, detail.MoveType * (detail.TCostUsD ?? 0));
                            doc.SetCellValue(rowindex, COL_DTL_BALCOST, detail.MoveType * (detail.TCostUsD ?? 0));
                        }
                        else
                        {
                            doc.SetCellValue(rowindex, COL_DTL_BALQTY, string.Format("={0}+{1}-{2}+{3}"
                                , SLConvert.ToCellReference(rowindex - 1, COL_DTL_BALQTY)
                                , SLConvert.ToCellReference(rowindex, COL_DTL_IN)
                                , SLConvert.ToCellReference(rowindex, COL_DTL_OUT)
                                , SLConvert.ToCellReference(rowindex, COL_DTL_RETURN)
                            ));

                            if (detail.MoveType == 1)
                            {
                                doc.SetCellValue(rowindex, COL_DTL_TCOST, (detail.TCostUsD ?? 0));
                            }
                            else
                            {
                                doc.SetCellValue(rowindex, COL_DTL_TCOST, -(detail.TCostUsD ?? 0));
                            }

                            doc.SetCellValue(rowindex, COL_DTL_BALCOST, string.Format("={0}+{1}"
                                , SLConvert.ToCellReference(rowindex - 1, COL_DTL_BALCOST)
                                , SLConvert.ToCellReference(rowindex, COL_DTL_TCOST)
                            ));
                        }

                        doc.SetCellValue(rowindex, COL_DTL_SLIP, detail.SlipNo);
                        rowindex++;

                        lastrow = firstrow + lineno - 1;
                    }

                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_SUBTTL, COL_MIN, ROW_SUBTTL, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_SUBTTL));
                    doc.SetCellValue(rowindex, COL_SUBTTL_IN, string.Format(
                        "=SUM({0}:{1})",
                        SLConvert.ToCellReference(firstrow, COL_SUBTTL_IN),
                        SLConvert.ToCellReference(lastrow, COL_SUBTTL_IN)
                    ));
                    doc.SetCellValue(rowindex, COL_SUBTTL_OUT, string.Format(
                        "=SUM({0}:{1})",
                        SLConvert.ToCellReference(firstrow, COL_SUBTTL_OUT),
                        SLConvert.ToCellReference(lastrow, COL_SUBTTL_OUT)
                    ));
                    doc.SetCellValue(rowindex, COL_SUBTTL_RETURN, string.Format(
                        "=SUM({0}:{1})",
                        SLConvert.ToCellReference(firstrow, COL_SUBTTL_RETURN),
                        SLConvert.ToCellReference(lastrow, COL_SUBTTL_RETURN)
                    ));
                    doc.SetCellValue(rowindex, COL_SUBTTL_BALQTY, string.Format("={0}", SLConvert.ToCellReference(lastrow, COL_SUBTTL_BALQTY)));
                    doc.SetCellValue(rowindex, COL_SUBTTL_BALCOST, string.Format("={0}", SLConvert.ToCellReference(lastrow, COL_SUBTTL_BALCOST)));
                    rowindex++;

                    //Blank Row for group separator.
                    rowindex++;
                }

                doc.DeleteWorksheet(WSNAME_DETAIL);

                doc.SaveAs(strOutputPath);
                docTemp.CloseWithoutSaving();
            }

            return strOutputPath;
        }

        public string GenerateIVS284InprocessToInstallReport(string reportType, List<dtInprocessToInstallReportDetail> data, doIVS284SearchCondition searchParam)
        {
            const string TEMPLATE_NAME = "IVS284.xlsx";
            const string WSNAME_DETAIL = "Detail";
            const string WSNAME_SUMMARY = "Summary";
            const string WSNAME_Working = "Sheet1";

            const int ROW_HEADER = 1;
            const int COL_HEADER_TITLE = 1;

            const int ROW_GRPHDR = 2;
            const int COL_GRPHDR_CONTRACTCODE = 2;
            const int COL_GRPHDR_OCC = 4;
            const int COL_GRPHDR_CHANGETYPE = 6;

            const int ROW_GRPHDR2 = 3;
            const int COL_GRPHDR2_CUSTNAME = 2;

            const int ROW_GRPHDR3 = 4;
            const int COL_GRPHDR3_SITE = 2;

            const int ROW_GRPHDR4 = 5;
            const int COL_GRPHDR4_OPERATEDATE = 2;
            const int COL_GRPHDR4_COMPLETEDATE = 4;
            const int COL_GRPHDR4_TRANSFERDATE = 6;

            const int ROW_TBLHDR = 6;
            const int COL_TBLHDR_INSTRUMENTCODE = 2;
            const int COL_TBLHDR_INSTRUMENTCODE_END = 4;

            const int ROW_DTL = 7;
            const int COL_DTL_NO = 1;
            const int COL_DTL_INSTRUMENTCODE = 2;
            const int COL_DTL_INSTRUMENTCODE_END = 4;
            const int COL_DTL_QTY = 5;
            const int COL_DTL_COST = 6;
            const int COL_DTL_TOTAL = 7;

            const int ROW_SUBTTL = 8;
            const int COL_SUBTTL_INSTRUMENTCODE = 2;
            const int COL_SUBTTL_INSTRUMENTCODE_END = 4;
            const int COL_SUBTTL_QTY = 5;
            const int COL_SUBTTL_TOTAL = 7;

            const int COL_MIN = 1;
            const int COL_MAX = 10;

            const int ROW_REPORT_TYPE_TITLE = 6;
            const int COL_REPORT_TYPE_TITLE = 2;

            string strReportTempletePath = PathUtil.GetPathValue(PathUtil.PathName.ReportTempatePath, TEMPLATE_NAME);
            if (!File.Exists(strReportTempletePath))
            {
                throw new FileNotFoundException("Template file not found.", TEMPLATE_NAME);
            }

            string strOutputPath = PathUtil.GetTempFileName(".xlsx");
            using (SLDocument docTemp = new SLDocument(strReportTempletePath))
            using (SLDocument doc = new SLDocument(strReportTempletePath))
            {
                doc.AddWorksheet(WSNAME_Working);
                docTemp.SelectWorksheet(WSNAME_DETAIL);
                doc.SelectWorksheet(WSNAME_Working);

                for (int i = COL_MIN; i <= COL_MAX; i++)
                {
                    doc.SetColumnWidth(i, docTemp.GetColumnWidth(i));
                }

                int rowindex = 1;

                doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_HEADER, COL_MIN, ROW_HEADER, COL_MAX, rowindex, 1);
                doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_HEADER));
                doc.SetCellValue(ROW_HEADER, COL_HEADER_TITLE, string.Format(
                    doc.GetCellValueAsString(ROW_HEADER, COL_HEADER_TITLE),
                    (reportType == "1" ? "MERCHANDISE" : "USER EQUIPMENT")
                ));
                rowindex++;

                var qGroupByContractOCC = (
                    from d in data
                    group d by new { d.TransferDate, d.ContractCodeShort, d.OCC } into grp
                    orderby grp.Key.TransferDate, grp.Key.ContractCodeShort, grp.Key.OCC
                    select grp
                );

                foreach (var contract in qGroupByContractOCC)
                {
                    var firstobj = contract.First();

                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_GRPHDR, COL_MIN, ROW_GRPHDR, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_GRPHDR));
                    doc.SetCellValue(rowindex, COL_GRPHDR_CONTRACTCODE, contract.Key.ContractCodeShort);
                    doc.SetCellValue(rowindex, COL_GRPHDR_OCC, contract.Key.OCC);
                    doc.SetCellValue(rowindex, COL_GRPHDR_CHANGETYPE, firstobj.ChangeType);
                    rowindex++;

                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_GRPHDR2, COL_MIN, ROW_GRPHDR2, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_GRPHDR2));
                    doc.SetCellValue(rowindex, COL_GRPHDR2_CUSTNAME, firstobj.CustNameEN);
                    rowindex++;

                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_GRPHDR3, COL_MIN, ROW_GRPHDR3, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_GRPHDR3));
                    doc.SetCellValue(rowindex, COL_GRPHDR3_SITE, firstobj.SiteName);
                    rowindex++;

                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_GRPHDR4, COL_MIN, ROW_GRPHDR4, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_GRPHDR4));
                    doc.SetCellValue(rowindex, COL_GRPHDR4_OPERATEDATE, (firstobj.OperateDate == null ? "" : firstobj.OperateDate.Value.ToString("dd/MM/yyyy")));
                    doc.SetCellValue(rowindex, COL_GRPHDR4_COMPLETEDATE, (firstobj.InstallCompleteDate == null ? "" : firstobj.InstallCompleteDate.Value.ToString("dd/MM/yyyy")));
                    doc.SetCellValue(rowindex, COL_GRPHDR4_TRANSFERDATE, (firstobj.TransferDate == null ? "" : firstobj.TransferDate.Value.ToString("dd/MM/yyyy")));
                    rowindex++;

                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_TBLHDR, COL_MIN, ROW_TBLHDR, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_TBLHDR));
                    doc.MergeWorksheetCells(rowindex, COL_TBLHDR_INSTRUMENTCODE, rowindex, COL_TBLHDR_INSTRUMENTCODE_END);
                    doc.SetCellValue(ROW_REPORT_TYPE_TITLE, COL_REPORT_TYPE_TITLE, string.Format(
                        doc.GetCellValueAsString(ROW_REPORT_TYPE_TITLE, COL_REPORT_TYPE_TITLE),
                        (reportType == "1" ? "MERCHANDISE" : "USER EQUIPMENT")
                    ));
                    rowindex++;

                    int firstrow, lastrow;
                    firstrow = lastrow = rowindex;
                    int lineno = 0;
                    foreach (var detail in contract.OrderBy(d => d.InstrumentCode))
                    {
                        lineno++;

                        doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_DTL, COL_MIN, ROW_DTL, COL_MAX, rowindex, 1);
                        doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_DTL));
                        doc.MergeWorksheetCells(rowindex, COL_DTL_INSTRUMENTCODE, rowindex, COL_DTL_INSTRUMENTCODE_END);
                        doc.SetCellValue(rowindex, COL_DTL_NO, lineno);
                        doc.SetCellValue(rowindex, COL_DTL_INSTRUMENTCODE, detail.InstrumentCode);
                        doc.SetCellValue(rowindex, COL_DTL_QTY, (detail.Qty ?? 0));
                        doc.SetCellValue(rowindex, COL_DTL_COST, detail.UnitPriceUsD);
                        doc.SetCellValue(rowindex, COL_DTL_TOTAL, (detail.AmountUsD ?? 0));
                        rowindex++;

                        lastrow = firstrow + lineno - 1;
                    }

                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_SUBTTL, COL_MIN, ROW_SUBTTL, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_SUBTTL));
                    doc.MergeWorksheetCells(rowindex, COL_SUBTTL_INSTRUMENTCODE, rowindex, COL_SUBTTL_INSTRUMENTCODE_END);
                    doc.SetCellValue(rowindex, COL_SUBTTL_QTY, string.Format(
                        "=SUM({0}:{1})",
                        SLConvert.ToCellReference(firstrow, COL_SUBTTL_QTY),
                        SLConvert.ToCellReference(lastrow, COL_SUBTTL_QTY)
                    ));
                    doc.SetCellValue(rowindex, COL_SUBTTL_TOTAL, string.Format(
                        "=SUM({0}:{1})",
                        SLConvert.ToCellReference(firstrow, COL_SUBTTL_TOTAL),
                        SLConvert.ToCellReference(lastrow, COL_SUBTTL_TOTAL)
                    ));
                    rowindex++;

                    //Blank Row for group separator.
                    rowindex++;
                }

                doc.DeleteWorksheet(WSNAME_DETAIL);
                doc.DeleteWorksheet(WSNAME_SUMMARY);

                doc.SaveAs(strOutputPath);
                docTemp.CloseWithoutSaving();
            }

            return strOutputPath;
        }

        public string GenerateIVS284InprocessToInstallReportSummary(string reportType, List<dtInprocessToInstallReport> data, doIVS284SearchCondition searchParam)
        {
            const string TEMPLATE_NAME = "IVS284.xlsx";
            const string WSNAME_DETAIL = "Detail";
            const string WSNAME_SUMMARY = "Summary";
            const string WSNAME_Working = "Sheet1";

            const int ROW_HEADER1 = 1;
            const int COL_HEADER1_TITLE = 1;
            const int COL_HEADER1_TITLE_END = 9;

            const int ROW_HEADER2 = 2;
            const int COL_HEADER2_DATEFROMTO = 1;
            const int COL_HEADER2_DATEFROMTO_END = 10;

            const int ROW_TBLHDR = 3;

            const int ROW_DTL = 4;
            const int COL_DTL_CONTRACTCODESHORT = 1;
            const int COL_DTL_OCC = 2;
            const int COL_DTL_CHANGETYPE = 3;
            const int COL_DTL_OPERATEDATE = 4;
            const int COL_DTL_COMPLETEDATE = 5;
            const int COL_DTL_CUSTNAME = 6;
            const int COL_DTL_SITENAME = 7;
            const int COL_DTL_TRANSFERDATE = 8;
            const int COL_DTL_QTY = 9;
            const int COL_DTL_AMOUNT = 10;

            const int ROW_SUBTTL = 5;
            const int COL_SUBTTL_TOTALLABEL = 1;
            const int COL_SUBTTL_TOTALLABEL_END = 8;
            const int COL_SUBTTL_QTY = 9;
            const int COL_SUBTTL_AMOUNT = 10;

            const int COL_MIN = 1;
            const int COL_MAX = 11;

            var commonUtil = new CommonUtil();

            string strReportTempletePath = PathUtil.GetPathValue(PathUtil.PathName.ReportTempatePath, TEMPLATE_NAME);
            if (!File.Exists(strReportTempletePath))
            {
                throw new FileNotFoundException("Template file not found.", TEMPLATE_NAME);
            }

            string strOutputPath = PathUtil.GetTempFileName(".xlsx");
            using (SLDocument docTemp = new SLDocument(strReportTempletePath))
            using (SLDocument doc = new SLDocument(strReportTempletePath))
            {
                doc.AddWorksheet(WSNAME_Working);
                docTemp.SelectWorksheet(WSNAME_SUMMARY);
                doc.SelectWorksheet(WSNAME_Working);

                for (int i = COL_MIN; i <= COL_MAX; i++)
                {
                    doc.SetColumnWidth(i, docTemp.GetColumnWidth(i));
                }

                int rowindex = 1;

                doc.CopyCellFromWorksheet(WSNAME_SUMMARY, ROW_HEADER1, COL_MIN, ROW_HEADER1, COL_MAX, rowindex, 1);
                doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_HEADER1));
                doc.SetCellValue(ROW_HEADER1, COL_HEADER1_TITLE, string.Format(
                    doc.GetCellValueAsString(ROW_HEADER1, COL_HEADER1_TITLE),
                    (reportType == "1" ? "MERCHANDISE" : "USER EQUIPMENT")
                ));
                doc.MergeWorksheetCells(rowindex, COL_HEADER1_TITLE, rowindex, COL_HEADER1_TITLE_END);
                rowindex++;

                doc.CopyCellFromWorksheet(WSNAME_SUMMARY, ROW_HEADER2, COL_MIN, ROW_HEADER2, COL_MAX, rowindex, 1);
                doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_HEADER2));
                doc.MergeWorksheetCells(rowindex, COL_HEADER2_DATEFROMTO, rowindex, COL_HEADER2_DATEFROMTO_END);
                    doc.SetCellValue(rowindex, COL_HEADER2_DATEFROMTO, "");
                if (searchParam != null && !string.IsNullOrEmpty(searchParam.YearMonth))
                {
                    DateTime dtTmp;
                    if (DateTime.TryParseExact(searchParam.YearMonth, "yyyyMM", null, System.Globalization.DateTimeStyles.None, out dtTmp)) {
                        doc.SetCellValue(rowindex, COL_HEADER2_DATEFROMTO, dtTmp.ToString("MMMM-yyyy"));
                }
                }
                rowindex++;

                doc.CopyCellFromWorksheet(WSNAME_SUMMARY, ROW_TBLHDR, COL_MIN, ROW_TBLHDR, COL_MAX, rowindex, 1);
                doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_TBLHDR));
                rowindex++;

                int firstrow, lastrow;
                firstrow = lastrow = rowindex;
                int lineno = 0;
                foreach (var detail in data.OrderBy(d => d.ChangeType))
                {
                    lineno++;

                    doc.CopyCellFromWorksheet(WSNAME_SUMMARY, ROW_DTL, COL_MIN, ROW_DTL, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_DTL));
                    //doc.SetCellValue(rowindex, COL_DTL_NO, lineno);
                    doc.SetCellValue(rowindex, COL_DTL_CONTRACTCODESHORT, detail.ContractCodeShort);
                    doc.SetCellValue(rowindex, COL_DTL_OCC, detail.OCC);
                    doc.SetCellValue(rowindex, COL_DTL_CHANGETYPE, detail.ChangeType);
                    doc.SetCellValue(rowindex, COL_DTL_OPERATEDATE, (detail.OperateDate == null ? "" : detail.OperateDate.Value.ToString("dd/MM/yyyy")));
                    doc.SetCellValue(rowindex, COL_DTL_COMPLETEDATE, (detail.InstallCompleteDate == null ? "" : detail.InstallCompleteDate.Value.ToString("dd/MM/yyyy")));
                    doc.SetCellValue(rowindex, COL_DTL_CUSTNAME, detail.CustNameEN);
                    doc.SetCellValue(rowindex, COL_DTL_SITENAME, detail.SiteName);
                    doc.SetCellValue(rowindex, COL_DTL_TRANSFERDATE, (detail.TransferDate == null ? "" : detail.TransferDate.Value.ToString("dd/MM/yyyy")));
                    doc.SetCellValue(rowindex, COL_DTL_QTY, (detail.Qty ?? 0));
                    doc.SetCellValue(rowindex, COL_DTL_AMOUNT, (detail.AmountUsD ?? 0));
                    rowindex++;

                    lastrow = firstrow + lineno - 1;
                }

                doc.CopyCellFromWorksheet(WSNAME_SUMMARY, ROW_SUBTTL, COL_MIN, ROW_SUBTTL, COL_MAX, rowindex, 1);
                doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_SUBTTL));
                doc.MergeWorksheetCells(rowindex, COL_SUBTTL_TOTALLABEL, rowindex, COL_SUBTTL_TOTALLABEL_END);
                doc.SetCellValue(rowindex, COL_SUBTTL_QTY, string.Format(
                    "=SUM({0}:{1})",
                    SLConvert.ToCellReference(firstrow, COL_SUBTTL_QTY),
                    SLConvert.ToCellReference(lastrow, COL_SUBTTL_QTY)
                ));
                doc.SetCellValue(rowindex, COL_SUBTTL_AMOUNT, string.Format(
                    "=SUM({0}:{1})",
                    SLConvert.ToCellReference(firstrow, COL_SUBTTL_AMOUNT),
                    SLConvert.ToCellReference(lastrow, COL_SUBTTL_AMOUNT)
                ));
                rowindex++;

                doc.DeleteWorksheet(WSNAME_DETAIL);
                doc.DeleteWorksheet(WSNAME_SUMMARY);

                doc.SaveAs(strOutputPath);
                docTemp.CloseWithoutSaving();
            }

            return strOutputPath;
        }

        public string GenerateIVS285PhysicalReport(string reportType, List<dtPhysicalReport> data)
        {
            const string TEMPLATE_NAME = "IVS285.xlsx";
            const string WSNAME_DETAIL = "Detail";
            const string WSNAME_Working = "Sheet1";

            const int ROW_HEADER = 1;
            const int COL_HEADER_TITLE = 1;

            const int ROW_TBLHDR = 2;

            const int ROW_DTL = 3;
            const int COL_DTL_NO = 1;
            const int COL_DTL_INSTRUMENTCODE = 2;
            const int COL_DTL_QTY = 3;
            const int COL_DTL_COST = 4;
            const int COL_DTL_TOTAL = 5;

            const int ROW_SUBTTL = 4;
            const int COL_SUBTTL_QTY = 3;
            const int COL_SUBTTL_TOTAL = 5;

            const int COL_MIN = 1;
            const int COL_MAX = 5;

            var commonUtil = new CommonUtil();

            string strReportTempletePath = PathUtil.GetPathValue(PathUtil.PathName.ReportTempatePath, TEMPLATE_NAME);
            if (!File.Exists(strReportTempletePath))
            {
                throw new FileNotFoundException("Template file not found.", TEMPLATE_NAME);
            }

            string strOutputPath = PathUtil.GetTempFileName(".xlsx");
            using (SLDocument docTemp = new SLDocument(strReportTempletePath))
            using (SLDocument doc = new SLDocument(strReportTempletePath))
            {
                doc.AddWorksheet(WSNAME_Working);
                docTemp.SelectWorksheet(WSNAME_DETAIL);
                doc.SelectWorksheet(WSNAME_Working);

                for (int i = COL_MIN; i <= COL_MAX; i++)
                {
                    doc.SetColumnWidth(i, docTemp.GetColumnWidth(i));
                }

                int rowindex = 1;

                doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_HEADER, COL_MIN, ROW_HEADER, COL_MAX, rowindex, 1);
                doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_HEADER));
                doc.SetCellValue(ROW_HEADER, COL_HEADER_TITLE, string.Format(
                    doc.GetCellValueAsString(ROW_HEADER, COL_HEADER_TITLE),
                    DateTime.Today.ToString("MMMM yyyy").ToUpper(),
                    (reportType == "1" ? "MERCHANDISE" : reportType == "2" ? "USER EQUIPMENT" : "DEMO AND SPARE")
                ));
                rowindex++;

                doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_TBLHDR, COL_MIN, ROW_TBLHDR, COL_MAX, rowindex, 1);
                doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_TBLHDR));
                rowindex++;

                int firstrow, lastrow;
                firstrow = lastrow = rowindex;
                int lineno = 0;
                foreach (var detail in data)
                {
                    lineno++;

                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_DTL, COL_MIN, ROW_DTL, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_DTL));
                    doc.SetCellValue(rowindex, COL_DTL_NO, lineno);
                    doc.SetCellValue(rowindex, COL_DTL_INSTRUMENTCODE, detail.InstrumentCode);
                    doc.SetCellValue(rowindex, COL_DTL_QTY, (detail.Qty ?? 0));
                    doc.SetCellValue(rowindex, COL_DTL_COST, (detail.CostUsD ?? 0));
                    doc.SetCellValue(rowindex, COL_DTL_TOTAL, (detail.TotalUsD ?? 0));
                    rowindex++;

                    lastrow = firstrow + lineno - 1;
                }

                doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_SUBTTL, COL_MIN, ROW_SUBTTL, COL_MAX, rowindex, 1);
                doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_SUBTTL));
                doc.SetCellValue(rowindex, COL_SUBTTL_QTY, string.Format(
                    "=SUM({0}:{1})",
                    SLConvert.ToCellReference(firstrow, COL_SUBTTL_QTY),
                    SLConvert.ToCellReference(lastrow, COL_SUBTTL_QTY)
                ));
                doc.SetCellValue(rowindex, COL_SUBTTL_TOTAL, string.Format(
                    "=SUM({0}:{1})",
                    SLConvert.ToCellReference(firstrow, COL_SUBTTL_TOTAL),
                    SLConvert.ToCellReference(lastrow, COL_SUBTTL_TOTAL)
                ));
                rowindex++;

                doc.DeleteWorksheet(WSNAME_DETAIL);

                doc.SaveAs(strOutputPath);
                docTemp.CloseWithoutSaving();
            }

            return strOutputPath;
        }

        public string GenerateIVS286InProcessReport(string reportType, List<dtInProcessReportDetail> data, doIVS286SearchCondition searchParam)
        {
            const string TEMPLATE_NAME = "IVS286.xlsx";
            const string WSNAME_DETAIL = "Detail";
            const string WSNAME_SUMMARY = "Summary";
            const string WSNAME_Working = "Sheet1";

            const int ROW_HEADER = 1;
            const int COL_HEADER_TITLE = 1;

            const int ROW_GRPHDR = 2;
            const int COL_GRPHDR_CONTRACTCODE = 2;
            const int COL_GRPHDR_TYPE = 4;

            const int ROW_GRPHDR2 = 3;
            const int COL_GRPHDR2_CUSTNAME = 2;

            const int ROW_GRPHDR3 = 4;
            const int COL_GRPHDR3_SITE = 2;

            const int ROW_TBLHDR = 5;
            const int COL_TBLHDR_INSTRUMENTCODE = 2;
            const int COL_TBLHDR_INSTRUMENTCODE_END = 6;

            const int ROW_DTL = 6;
            const int COL_DTL_NO = 1;
            const int COL_DTL_INSTRUMENTCODE = 2;
            const int COL_DTL_INSTRUMENTCODE_END = 6;
            const int COL_DTL_QTY = 7;
            const int COL_DTL_COST = 8;
            const int COL_DTL_TOTAL = 9;

            const int ROW_SUBTTL = 7;
            const int COL_SUBTTL_INSTRUMENTCODE = 2;
            const int COL_SUBTTL_INSTRUMENTCODE_END = 6;
            const int COL_SUBTTL_QTY = 7;
            const int COL_SUBTTL_TOTAL = 9;

            const int COL_MIN = 1;
            const int COL_MAX = 10;

            string strReportTempletePath = PathUtil.GetPathValue(PathUtil.PathName.ReportTempatePath, TEMPLATE_NAME);
            if (!File.Exists(strReportTempletePath))
            {
                throw new FileNotFoundException("Template file not found.", TEMPLATE_NAME);
            }

            string strOutputPath = PathUtil.GetTempFileName(".xlsx");
            using (SLDocument docTemp = new SLDocument(strReportTempletePath))
            using (SLDocument doc = new SLDocument(strReportTempletePath))
            {
                doc.AddWorksheet(WSNAME_Working);
                docTemp.SelectWorksheet(WSNAME_DETAIL);
                doc.SelectWorksheet(WSNAME_Working);

                for (int i = COL_MIN; i <= COL_MAX; i++)
                {
                    doc.SetColumnWidth(i, docTemp.GetColumnWidth(i));
                }

                int rowindex = 1;

                doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_HEADER, COL_MIN, ROW_HEADER, COL_MAX, rowindex, 1);
                doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_HEADER));
                doc.SetCellValue(ROW_HEADER, COL_HEADER_TITLE, string.Format(
                    doc.GetCellValueAsString(ROW_HEADER, COL_HEADER_TITLE),
                    (reportType == "1" ? "MERCHANDISE" : "USER EQUIPMENT")
                ));
                rowindex++;

                var qGroupByContract = (
                    from d in data
                    group d by new { d.ContractCodeShort, d.ContractType, d.Customer, d.SiteName } into grp
                    orderby grp.Key.ContractCodeShort
                    select grp
                );

                foreach (var contract in qGroupByContract)
                {
                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_GRPHDR, COL_MIN, ROW_GRPHDR, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_GRPHDR));
                    doc.SetCellValue(rowindex, COL_GRPHDR_CONTRACTCODE, contract.Key.ContractCodeShort);
                    doc.SetCellValue(rowindex, COL_GRPHDR_TYPE, contract.Key.ContractType);
                    rowindex++;

                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_GRPHDR2, COL_MIN, ROW_GRPHDR2, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_GRPHDR2));
                    doc.SetCellValue(rowindex, COL_GRPHDR2_CUSTNAME, contract.Key.Customer);
                    rowindex++;

                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_GRPHDR3, COL_MIN, ROW_GRPHDR3, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_GRPHDR3));
                    doc.SetCellValue(rowindex, COL_GRPHDR3_SITE, contract.Key.SiteName);
                    rowindex++;

                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_TBLHDR, COL_MIN, ROW_TBLHDR, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_TBLHDR));
                    doc.MergeWorksheetCells(rowindex, COL_TBLHDR_INSTRUMENTCODE, rowindex, COL_TBLHDR_INSTRUMENTCODE_END);
                    doc.SetCellValue(rowindex, COL_TBLHDR_INSTRUMENTCODE, string.Format(
                       doc.GetCellValueAsString(rowindex, COL_TBLHDR_INSTRUMENTCODE),
                       (reportType == "1" ? "MERCHANDISE" : "USER EQUIPMENT")
                    ));
                    rowindex++;

                    int firstrow, lastrow;
                    firstrow = lastrow = rowindex;
                    int lineno = 0;
                    foreach (var detail in contract.OrderBy(d => d.InstrumentCode))
                    {
                        lineno++;

                        doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_DTL, COL_MIN, ROW_DTL, COL_MAX, rowindex, 1);
                        doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_DTL));
                        doc.MergeWorksheetCells(rowindex, COL_DTL_INSTRUMENTCODE, rowindex, COL_DTL_INSTRUMENTCODE_END);
                        doc.SetCellValue(rowindex, COL_DTL_NO, lineno);
                        doc.SetCellValue(rowindex, COL_DTL_INSTRUMENTCODE, detail.InstrumentCode);
                        doc.SetCellValue(rowindex, COL_DTL_QTY, (detail.Qty ?? 0));
                        doc.SetCellValue(rowindex, COL_DTL_COST, detail.UnitPriceUsD);
                        doc.SetCellValue(rowindex, COL_DTL_TOTAL, (detail.AmountUsD ?? 0));
                        rowindex++;

                        lastrow = firstrow + lineno - 1;
                    }

                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_SUBTTL, COL_MIN, ROW_SUBTTL, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_SUBTTL));
                    doc.MergeWorksheetCells(rowindex, COL_SUBTTL_INSTRUMENTCODE, rowindex, COL_SUBTTL_INSTRUMENTCODE_END);
                    doc.SetCellValue(rowindex, COL_SUBTTL_QTY, string.Format(
                        "=SUM({0}:{1})",
                        SLConvert.ToCellReference(firstrow, COL_SUBTTL_QTY),
                        SLConvert.ToCellReference(lastrow, COL_SUBTTL_QTY)
                    ));
                    doc.SetCellValue(rowindex, COL_SUBTTL_TOTAL, string.Format(
                        "=SUM({0}:{1})",
                        SLConvert.ToCellReference(firstrow, COL_SUBTTL_TOTAL),
                        SLConvert.ToCellReference(lastrow, COL_SUBTTL_TOTAL)
                    ));
                    rowindex++;

                    //Blank Row for group separator.
                    rowindex++;
                }

                doc.DeleteWorksheet(WSNAME_DETAIL);
                doc.DeleteWorksheet(WSNAME_SUMMARY);

                doc.SaveAs(strOutputPath);
                docTemp.CloseWithoutSaving();
            }

            return strOutputPath;
        }

        public string GenerateIVS286InProcessReportSummary(string reportType, List<dtInProcessReport> data, doIVS286SearchCondition searchParam)
        {
            const string TEMPLATE_NAME = "IVS286.xlsx";
            const string WSNAME_DETAIL = "Detail";
            const string WSNAME_SUMMARY = "Summary";
            const string WSNAME_Working = "Sheet1";

            const int ROW_HEADER1 = 1;
            const int COL_HEADER1_TITLE = 1;
            const int COL_HEADER1_TITLE_END = 5;

            const int ROW_HEADER2 = 2;
            const int COL_HEADER2_PROCESSDATE = 1;
            const int COL_HEADER2_PROCESSDATE_END = 6;

            const int ROW_TBLHDR = 3;

            const int ROW_DTL = 4;
            const int COL_DTL_CONTRACTCODESHORT = 1;
            const int COL_DTL_CONTRACTTYPE = 2;
            const int COL_DTL_CUSTNAME = 3;
            const int COL_DTL_SITE = 4;
            const int COL_DTL_QTY = 5;
            const int COL_DTL_AMOUNT = 6;

            const int ROW_SUBTTL = 5;
            const int COL_SUBTTL_TOTALLABEL = 1;
            const int COL_SUBTTL_TOTALLABEL_END = 4;
            const int COL_SUBTTL_QTY = 5;
            const int COL_SUBTTL_AMOUNT = 6;

            const int COL_MIN = 1;
            const int COL_MAX = 6;

            var commonUtil = new CommonUtil();

            string strReportTempletePath = PathUtil.GetPathValue(PathUtil.PathName.ReportTempatePath, TEMPLATE_NAME);
            if (!File.Exists(strReportTempletePath))
            {
                throw new FileNotFoundException("Template file not found.", TEMPLATE_NAME);
            }

            string strOutputPath = PathUtil.GetTempFileName(".xlsx");
            using (SLDocument docTemp = new SLDocument(strReportTempletePath))
            using (SLDocument doc = new SLDocument(strReportTempletePath))
            {
                doc.AddWorksheet(WSNAME_Working);
                docTemp.SelectWorksheet(WSNAME_SUMMARY);
                doc.SelectWorksheet(WSNAME_Working);

                for (int i = COL_MIN; i <= COL_MAX; i++)
                {
                    doc.SetColumnWidth(i, docTemp.GetColumnWidth(i));
                }

                int rowindex = 1;

                doc.CopyCellFromWorksheet(WSNAME_SUMMARY, ROW_HEADER1, COL_MIN, ROW_HEADER1, COL_MAX, rowindex, 1);
                doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_HEADER1));
                doc.SetCellValue(ROW_HEADER1, COL_HEADER1_TITLE, string.Format(
                    doc.GetCellValueAsString(ROW_HEADER1, COL_HEADER1_TITLE),
                    (reportType == "1" ? "MERCHANDISE" : "USER EQUIPMENT")
                ));
                doc.MergeWorksheetCells(rowindex, COL_HEADER1_TITLE, rowindex, COL_HEADER1_TITLE_END);
                rowindex++;

                doc.CopyCellFromWorksheet(WSNAME_SUMMARY, ROW_HEADER2, COL_MIN, ROW_HEADER2, COL_MAX, rowindex, 1);
                doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_HEADER2));
                doc.MergeWorksheetCells(rowindex, COL_HEADER2_PROCESSDATE, rowindex, COL_HEADER2_PROCESSDATE_END);
                if (searchParam == null || searchParam.ProcessDate == null)
                {
                    doc.SetCellValue(rowindex, COL_HEADER2_PROCESSDATE, "");
                }
                else
                {
                    try
                    {
                        var date = DateTime.ParseExact(searchParam.ProcessDate, "yyyyMM", null);
                        doc.SetCellValue(rowindex, COL_HEADER2_PROCESSDATE, date.ToString("MMMM-yyyy"));
                    }
                    catch { }
                }
                rowindex++;

                doc.CopyCellFromWorksheet(WSNAME_SUMMARY, ROW_TBLHDR, COL_MIN, ROW_TBLHDR, COL_MAX, rowindex, 1);
                doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_TBLHDR));
                rowindex++;

                int firstrow, lastrow;
                firstrow = lastrow = rowindex;
                int lineno = 0;
                foreach (var detail in data)
                {
                    lineno++;

                    doc.CopyCellFromWorksheet(WSNAME_SUMMARY, ROW_DTL, COL_MIN, ROW_DTL, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_DTL));
                    doc.SetCellValue(rowindex, COL_DTL_CONTRACTCODESHORT, detail.ContractCodeShort);
                    doc.SetCellValue(rowindex, COL_DTL_CONTRACTTYPE, detail.ContractType);
                    doc.SetCellValue(rowindex, COL_DTL_CUSTNAME, detail.Customer);
                    doc.SetCellValue(rowindex, COL_DTL_SITE, detail.SiteName);
                    doc.SetCellValue(rowindex, COL_DTL_QTY, (detail.Qty ?? 0));
                    doc.SetCellValue(rowindex, COL_DTL_AMOUNT, (detail.AmountUsD ?? 0));
                    rowindex++;

                    lastrow = firstrow + lineno - 1;
                }

                doc.CopyCellFromWorksheet(WSNAME_SUMMARY, ROW_SUBTTL, COL_MIN, ROW_SUBTTL, COL_MAX, rowindex, 1);
                doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_SUBTTL));
                doc.MergeWorksheetCells(rowindex, COL_SUBTTL_TOTALLABEL, rowindex, COL_SUBTTL_TOTALLABEL_END);
                doc.SetCellValue(rowindex, COL_SUBTTL_QTY, string.Format(
                    "=SUM({0}:{1})",
                    SLConvert.ToCellReference(firstrow, COL_SUBTTL_QTY),
                    SLConvert.ToCellReference(lastrow, COL_SUBTTL_QTY)
                ));
                doc.SetCellValue(rowindex, COL_SUBTTL_AMOUNT, string.Format(
                    "=SUM({0}:{1})",
                    SLConvert.ToCellReference(firstrow, COL_SUBTTL_AMOUNT),
                    SLConvert.ToCellReference(lastrow, COL_SUBTTL_AMOUNT)
                ));
                rowindex++;

                doc.DeleteWorksheet(WSNAME_DETAIL);
                doc.DeleteWorksheet(WSNAME_SUMMARY);

                doc.SaveAs(strOutputPath);
                docTemp.CloseWithoutSaving();
            }

            return strOutputPath;
        }

        public string GenerateIVS287StockListReport(string reportType, List<dtStockListReport> data)
        {
            const string TEMPLATE_NAME = "IVS287.xlsx";
            const string WSNAME_DETAIL = "Detail";
            const string WSNAME_Working = "Sheet1";

            const int ROW_HEADER = 1;
            const int COL_HEADER = 1;

            const int ROW_TBLHDR = 2;
            const int COL_TBLHDR_INSTRUMENTCODE = 2;

            const int ROW_DTL = 3;
            const int COL_DTL_NO = 1;
            const int COL_DTL_INSTRUMENTCODE = 2;
            const int COL_DTL_QTY = 3;
            const int COL_DTL_COST = 4;
            const int COL_DTL_TOTAL = 5;

            const int ROW_SUBTTL = 4;
            const int COL_SUBTTL_QTY = 3;
            const int COL_SUBTTL_TOTAL = 5;

            const int COL_MIN = 1;
            const int COL_MAX = 5;

            var commonUtil = new CommonUtil();

            string strReportTempletePath = PathUtil.GetPathValue(PathUtil.PathName.ReportTempatePath, TEMPLATE_NAME);
            if (!File.Exists(strReportTempletePath))
            {
                throw new FileNotFoundException("Template file not found.", TEMPLATE_NAME);
            }

            string strOutputPath = PathUtil.GetTempFileName(".xlsx");
            using (SLDocument docTemp = new SLDocument(strReportTempletePath))
            using (SLDocument doc = new SLDocument(strReportTempletePath))
            {
                doc.AddWorksheet(WSNAME_Working);
                docTemp.SelectWorksheet(WSNAME_DETAIL);
                doc.SelectWorksheet(WSNAME_Working);

                for (int i = COL_MIN; i <= COL_MAX; i++)
                {
                    doc.SetColumnWidth(i, docTemp.GetColumnWidth(i));
                }

                int rowindex = 1;

                doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_HEADER, COL_MIN, ROW_HEADER, COL_MAX, rowindex, 1);
                doc.SetCellValue(ROW_HEADER, COL_HEADER, string.Format(
                    doc.GetCellValueAsString(ROW_HEADER, COL_HEADER),
                    (reportType == "1" ? "MERCHANDISE" : "USER EQUIPMENT")
                ));
                doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_HEADER));
                rowindex++;

                doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_TBLHDR, COL_MIN, ROW_TBLHDR, COL_MAX, rowindex, 1);
                doc.SetCellValue(ROW_TBLHDR, COL_TBLHDR_INSTRUMENTCODE, string.Format(
                    doc.GetCellValueAsString(ROW_TBLHDR, COL_TBLHDR_INSTRUMENTCODE),
                    (reportType == "1" ? "MERCHANDISE" : "USER EQUIPMENT")
                ));
                doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_TBLHDR));
                rowindex++;

                int firstrow, lastrow;
                firstrow = lastrow = rowindex;
                int lineno = 0;
                foreach (var detail in data)
                {
                    lineno++;

                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_DTL, COL_MIN, ROW_DTL, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_DTL));
                    doc.SetCellValue(rowindex, COL_DTL_NO, lineno);
                    doc.SetCellValue(rowindex, COL_DTL_INSTRUMENTCODE, detail.InstrumentCode);
                    doc.SetCellValue(rowindex, COL_DTL_QTY, (detail.Qty ?? 0));
                    doc.SetCellValue(rowindex, COL_DTL_COST, (detail.CostUsD ?? 0));
                    doc.SetCellValue(rowindex, COL_DTL_TOTAL, (detail.TotalUsD ?? 0));
                    rowindex++;

                    lastrow = firstrow + lineno - 1;
                }

                doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_SUBTTL, COL_MIN, ROW_SUBTTL, COL_MAX, rowindex, 1);
                doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_SUBTTL));
                doc.SetCellValue(rowindex, COL_SUBTTL_QTY, string.Format(
                    "=SUM({0}:{1})",
                    SLConvert.ToCellReference(firstrow, COL_SUBTTL_QTY),
                    SLConvert.ToCellReference(lastrow, COL_SUBTTL_QTY)
                ));
                doc.SetCellValue(rowindex, COL_SUBTTL_TOTAL, string.Format(
                    "=SUM({0}:{1})",
                    SLConvert.ToCellReference(firstrow, COL_SUBTTL_TOTAL),
                    SLConvert.ToCellReference(lastrow, COL_SUBTTL_TOTAL)
                ));
                rowindex++;

                doc.DeleteWorksheet(WSNAME_DETAIL);

                doc.SaveAs(strOutputPath);
                docTemp.CloseWithoutSaving();
            }

            return strOutputPath;
        }

        public string GenerateIVS288ChangeAreaReport(List<dtChangeAreaReportDetail> data, doIVS288SearchCondition searchParam)
        {
            const string TEMPLATE_NAME = "IVS288.xlsx";
            const string WSNAME_DETAIL = "Detail";
            const string WSNAME_SUMMARY = "Summary";
            const string WSNAME_Working = "Sheet1";

            const int ROW_HEADER = 1;
            const int COL_HEADER_TITLE = 1;

            const int ROW_GRPHDR1 = 2;
            const int COL_GRPHDR1_SLIPNO = 2;
            const int COL_GRPHDR1_TRANSFERDATE = 4;
            const int COL_GRPHDR1_CUSTNAME = 6;

            const int ROW_GRPHDR2 = 3;
            const int COL_GRPHDR2_CONTRACTCODE = 2;
            const int COL_GRPHDR2_APPROVENO = 4;

            const int ROW_TBLHDR = 4;
            const int COL_TBLHDR_INSTRUMENTCODE = 2;
            const int COL_TBLHDR_INSTRUMENTCODE_END = 6;

            const int ROW_DTL = 5;
            const int COL_DTL_NO = 1;
            const int COL_DTL_INSTRUMENTCODE = 2;
            const int COL_DTL_INSTRUMENTCODE_END = 6;
            const int COL_DTL_QTY = 7;
            const int COL_DTL_COST = 8;
            const int COL_DTL_TOTAL = 9;

            const int ROW_SUBTTL = 6;
            const int COL_SUBTTL_INSTRUMENTCODE = 2;
            const int COL_SUBTTL_INSTRUMENTCODE_END = 6;
            const int COL_SUBTTL_QTY = 7;
            const int COL_SUBTTL_TOTAL = 9;

            const int COL_MIN = 1;
            const int COL_MAX = 10;

            var commonUtil = new CommonUtil();

            string strReportTempletePath = PathUtil.GetPathValue(PathUtil.PathName.ReportTempatePath, TEMPLATE_NAME);
            if (!File.Exists(strReportTempletePath))
            {
                throw new FileNotFoundException("Template file not found.", TEMPLATE_NAME);
            }

            string strOutputPath = PathUtil.GetTempFileName(".xlsx");
            using (SLDocument docTemp = new SLDocument(strReportTempletePath))
            using (SLDocument doc = new SLDocument(strReportTempletePath))
            {
                doc.AddWorksheet(WSNAME_Working);
                docTemp.SelectWorksheet(WSNAME_DETAIL);
                doc.SelectWorksheet(WSNAME_Working);

                for (int i = COL_MIN; i <= COL_MAX; i++)
                {
                    doc.SetColumnWidth(i, docTemp.GetColumnWidth(i));
                }

                int rowindex = 1;

                doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_HEADER, COL_MIN, ROW_HEADER, COL_MAX, rowindex, 1);
                doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_HEADER));
                doc.SetCellValue(ROW_HEADER, COL_HEADER_TITLE, string.Format(
                    doc.GetCellValueAsString(ROW_HEADER, COL_HEADER_TITLE),
                    searchParam.SourceAreaCode,
                    searchParam.DestinationAreaCode
                ));
                rowindex++;

                var qGroupBySlip = (
                    from d in data
                    group d by new
                    {
                        d.SlipNo,
                        d.TransferDate,
                        CustName = d.CustNameEN,
                        d.ContractCode,
                        d.ApproveNo,
                    } into grp
                    orderby grp.Key.SlipNo, grp.Key.TransferDate
                    select grp
                );

                foreach (var slip in qGroupBySlip)
                {
                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_GRPHDR1, COL_MIN, ROW_GRPHDR1, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_GRPHDR1));
                    doc.SetCellValue(rowindex, COL_GRPHDR1_SLIPNO, slip.Key.SlipNo);
                    doc.SetCellValue(rowindex, COL_GRPHDR1_TRANSFERDATE, (slip.Key.TransferDate == null ? "" : slip.Key.TransferDate.Value.ToString("dd/MM/yyyy")));
                    doc.SetCellValue(rowindex, COL_GRPHDR1_CUSTNAME, slip.Key.CustName);
                    rowindex++;

                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_GRPHDR2, COL_MIN, ROW_GRPHDR2, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_GRPHDR2));
                    doc.SetCellValue(rowindex, COL_GRPHDR2_CONTRACTCODE, commonUtil.ConvertContractCode(slip.Key.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT));
                    doc.SetCellValue(rowindex, COL_GRPHDR2_APPROVENO, slip.Key.ApproveNo);
                    rowindex++;

                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_TBLHDR, COL_MIN, ROW_TBLHDR, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_TBLHDR));
                    doc.MergeWorksheetCells(rowindex, COL_TBLHDR_INSTRUMENTCODE, rowindex, COL_TBLHDR_INSTRUMENTCODE_END);
                    rowindex++;

                    int firstrow, lastrow;
                    firstrow = lastrow = rowindex;
                    int lineno = 0;
                    foreach (var detail in slip.OrderBy(d => d.InstrumentCode))
                    {
                        lineno++;

                        doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_DTL, COL_MIN, ROW_DTL, COL_MAX, rowindex, 1);
                        doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_DTL));
                        doc.MergeWorksheetCells(rowindex, COL_DTL_INSTRUMENTCODE, rowindex, COL_DTL_INSTRUMENTCODE_END);
                        doc.SetCellValue(rowindex, COL_DTL_NO, lineno);
                        doc.SetCellValue(rowindex, COL_DTL_INSTRUMENTCODE, detail.InstrumentCode);
                        doc.SetCellValue(rowindex, COL_DTL_QTY, (detail.Qty ?? 0));
                        doc.SetCellValue(rowindex, COL_DTL_COST, (detail.CostUsD ?? 0));
                        doc.SetCellValue(rowindex, COL_DTL_TOTAL, (detail.TotalUsD ?? 0));
                        rowindex++;

                        lastrow = firstrow + lineno - 1;
                    }

                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_SUBTTL, COL_MIN, ROW_SUBTTL, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_SUBTTL));
                    doc.MergeWorksheetCells(rowindex, COL_SUBTTL_INSTRUMENTCODE, rowindex, COL_SUBTTL_INSTRUMENTCODE_END);
                    doc.SetCellValue(rowindex, COL_SUBTTL_QTY, string.Format(
                        "=SUM({0}:{1})",
                        SLConvert.ToCellReference(firstrow, COL_SUBTTL_QTY),
                        SLConvert.ToCellReference(lastrow, COL_SUBTTL_QTY)
                    ));
                    doc.SetCellValue(rowindex, COL_SUBTTL_TOTAL, string.Format(
                        "=SUM({0}:{1})",
                        SLConvert.ToCellReference(firstrow, COL_SUBTTL_TOTAL),
                        SLConvert.ToCellReference(lastrow, COL_SUBTTL_TOTAL)
                    ));
                    rowindex++;

                    //Blank Row for group separator.
                    rowindex++;
                }

                doc.DeleteWorksheet(WSNAME_DETAIL);
                doc.DeleteWorksheet(WSNAME_SUMMARY);

                doc.SaveAs(strOutputPath);
                docTemp.CloseWithoutSaving();
            }

            return strOutputPath;
        }

        public string GenerateIVS288ChangeAreaReportSummary(List<dtChangeAreaReportDetail> data, doIVS288SearchCondition searchParam)
        {
            const string TEMPLATE_NAME = "IVS288.xlsx";
            const string WSNAME_DETAIL = "Detail";
            const string WSNAME_SUMMARY = "Summary";
            const string WSNAME_Working = "Sheet1";

            const int ROW_HEADER = 1;
            const int COL_HEADER_TITLE = 1;

            const int ROW_TBLHDR = 2;
            const int COL_TBLHDR_INSTRUMENTCODE = 2;
            const int COL_TBLHDR_INSTRUMENTCODE_END = 6;

            const int ROW_DTL = 3;
            const int COL_DTL_NO = 1;
            const int COL_DTL_INSTRUMENTCODE = 2;
            const int COL_DTL_INSTRUMENTCODE_END = 6;
            const int COL_DTL_QTY = 7;
            const int COL_DTL_COST = 8;
            const int COL_DTL_TOTAL = 9;

            const int ROW_TTL = 4;
            const int COL_TTL_INSTRUMENTCODE = 2;
            const int COL_TTL_INSTRUMENTCODE_END = 6;
            const int COL_TTL_QTY = 7;
            const int COL_TTL_TOTAL = 9;

            const int COL_MIN = 1;
            const int COL_MAX = 10;

            string strReportTempletePath = PathUtil.GetPathValue(PathUtil.PathName.ReportTempatePath, TEMPLATE_NAME);
            if (!File.Exists(strReportTempletePath))
            {
                throw new FileNotFoundException("Template file not found.", TEMPLATE_NAME);
            }

            string strOutputPath = PathUtil.GetTempFileName(".xlsx");
            using (SLDocument docTemp = new SLDocument(strReportTempletePath))
            using (SLDocument doc = new SLDocument(strReportTempletePath))
            {
                doc.AddWorksheet(WSNAME_Working);
                docTemp.SelectWorksheet(WSNAME_SUMMARY);
                doc.SelectWorksheet(WSNAME_Working);

                for (int i = COL_MIN; i <= COL_MAX; i++)
                {
                    doc.SetColumnWidth(i, docTemp.GetColumnWidth(i));
                }

                int rowindex = 1;

                doc.CopyCellFromWorksheet(WSNAME_SUMMARY, ROW_HEADER, COL_MIN, ROW_HEADER, COL_MAX, rowindex, 1);
                doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_HEADER));
                doc.SetCellValue(ROW_HEADER, COL_HEADER_TITLE, string.Format(
                    doc.GetCellValueAsString(ROW_HEADER, COL_HEADER_TITLE),
                    searchParam.SourceAreaCode,
                    searchParam.DestinationAreaCode
                ));
                rowindex++;

                var qSummary = (
                    from d in data
                    group d by new { d.InstrumentCode } into grp
                    orderby grp.Key.InstrumentCode
                    select grp
                );

                doc.CopyCellFromWorksheet(WSNAME_SUMMARY, ROW_TBLHDR, COL_MIN, ROW_TBLHDR, COL_MAX, rowindex, 1);
                doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_TBLHDR));
                doc.MergeWorksheetCells(rowindex, COL_TBLHDR_INSTRUMENTCODE, rowindex, COL_TBLHDR_INSTRUMENTCODE_END);
                rowindex++;

                int firstrow, lastrow;
                firstrow = lastrow = rowindex;
                int lineno = 0;
                foreach (var detail in qSummary)
                {
                    lineno++;

                    doc.CopyCellFromWorksheet(WSNAME_SUMMARY, ROW_DTL, COL_MIN, ROW_DTL, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_DTL));
                    doc.MergeWorksheetCells(rowindex, COL_DTL_INSTRUMENTCODE, rowindex, COL_DTL_INSTRUMENTCODE_END);
                    doc.SetCellValue(rowindex, COL_DTL_NO, lineno);
                    doc.SetCellValue(rowindex, COL_DTL_INSTRUMENTCODE, detail.Key.InstrumentCode);

                    int qty = detail.Sum(d => d.Qty ?? 0);
                    decimal total = detail.Sum(d => d.TotalUsD ?? 0);

                    doc.SetCellValue(rowindex, COL_DTL_QTY, qty);
                    doc.SetCellValue(rowindex, COL_DTL_COST, string.Format(
                        "={0}/{1}",
                        SLConvert.ToCellReference(rowindex, COL_DTL_TOTAL),
                        SLConvert.ToCellReference(rowindex, COL_DTL_QTY)
                    ));
                    doc.SetCellValue(rowindex, COL_DTL_TOTAL, total);
                    rowindex++;

                    lastrow = firstrow + lineno - 1;
                }

                doc.CopyCellFromWorksheet(WSNAME_SUMMARY, ROW_TTL, COL_MIN, ROW_TTL, COL_MAX, rowindex, 1);
                doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_TTL));
                doc.MergeWorksheetCells(rowindex, COL_TTL_INSTRUMENTCODE, rowindex, COL_TTL_INSTRUMENTCODE_END);
                doc.SetCellValue(rowindex, COL_TTL_QTY, string.Format(
                    "=SUM({0}:{1})",
                    SLConvert.ToCellReference(firstrow, COL_TTL_QTY),
                    SLConvert.ToCellReference(lastrow, COL_TTL_QTY)
                ));
                doc.SetCellValue(rowindex, COL_TTL_TOTAL, string.Format(
                    "=SUM({0}:{1})",
                    SLConvert.ToCellReference(firstrow, COL_TTL_TOTAL),
                    SLConvert.ToCellReference(lastrow, COL_TTL_TOTAL)
                ));


                doc.DeleteWorksheet(WSNAME_DETAIL);
                doc.DeleteWorksheet(WSNAME_SUMMARY);

                doc.SaveAs(strOutputPath);
                docTemp.CloseWithoutSaving();
            }

            return strOutputPath;
        }

        public string GenerateIVS289EliminateReport(List<dtEliminateReportDetail> data, doIVS289SearchCondition searchParam)
        {
            const string TEMPLATE_NAME = "IVS289.xlsx";
            const string WSNAME_DETAIL = "Detail";
            const string WSNAME_SUMMARY = "Summary";
            const string WSNAME_Working = "Sheet1";

            const int ROW_HEADER = 1;
            const int COL_HEADER_TITLE = 1;

            const int ROW_GRPHDR1 = 2;
            const int COL_GRPHDR1_SLIPNO = 2;
            const int COL_GRPHDR1_STOCKOUTDATE = 4;

            const int ROW_TBLHDR = 3;
            const int COL_TBLHDR_INSTRUMENTCODE = 2;
            const int COL_TBLHDR_INSTRUMENTCODE_END = 6;

            const int ROW_DTL = 4;
            const int COL_DTL_NO = 1;
            const int COL_DTL_INSTRUMENTCODE = 2;
            const int COL_DTL_INSTRUMENTCODE_END = 6;
            const int COL_DTL_QTY = 7;
            const int COL_DTL_COST = 8;
            const int COL_DTL_TOTAL = 9;

            const int ROW_SUBTTL = 5;
            const int COL_SUBTTL_INSTRUMENTCODE = 2;
            const int COL_SUBTTL_INSTRUMENTCODE_END = 6;
            const int COL_SUBTTL_QTY = 7;
            const int COL_SUBTTL_TOTAL = 9;

            const int COL_MIN = 1;
            const int COL_MAX = 10;

            var commonUtil = new CommonUtil();

            string strReportTempletePath = PathUtil.GetPathValue(PathUtil.PathName.ReportTempatePath, TEMPLATE_NAME);
            if (!File.Exists(strReportTempletePath))
            {
                throw new FileNotFoundException("Template file not found.", TEMPLATE_NAME);
            }

            string strOutputPath = PathUtil.GetTempFileName(".xlsx");
            using (SLDocument docTemp = new SLDocument(strReportTempletePath))
            using (SLDocument doc = new SLDocument(strReportTempletePath))
            {
                doc.AddWorksheet(WSNAME_Working);
                docTemp.SelectWorksheet(WSNAME_DETAIL);
                doc.SelectWorksheet(WSNAME_Working);

                for (int i = COL_MIN; i <= COL_MAX; i++)
                {
                    doc.SetColumnWidth(i, docTemp.GetColumnWidth(i));
                }

                int rowindex = 1;

                doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_HEADER, COL_MIN, ROW_HEADER, COL_MAX, rowindex, 1);
                doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_HEADER));
                doc.SetCellValue(ROW_HEADER, COL_HEADER_TITLE, string.Format(
                    doc.GetCellValueAsString(ROW_HEADER, COL_HEADER_TITLE),
                    searchParam.TransferTypeText
                ));
                rowindex++;

                var qGroupBySlip = (
                    from d in data
                    group d by new
                    {
                        d.SlipNo,
                        d.StockOutDate,
                    } into grp
                    orderby grp.Key.SlipNo, grp.Key.StockOutDate
                    select grp
                );

                foreach (var slip in qGroupBySlip)
                {
                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_GRPHDR1, COL_MIN, ROW_GRPHDR1, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_GRPHDR1));
                    doc.SetCellValue(rowindex, COL_GRPHDR1_SLIPNO, slip.Key.SlipNo);
                    doc.SetCellValue(rowindex, COL_GRPHDR1_STOCKOUTDATE, (slip.Key.StockOutDate == null ? "" : slip.Key.StockOutDate.Value.ToString("dd/MM/yyyy")));
                    rowindex++;

                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_TBLHDR, COL_MIN, ROW_TBLHDR, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_TBLHDR));
                    doc.MergeWorksheetCells(rowindex, COL_TBLHDR_INSTRUMENTCODE, rowindex, COL_TBLHDR_INSTRUMENTCODE_END);
                    rowindex++;

                    int firstrow, lastrow;
                    firstrow = lastrow = rowindex;
                    int lineno = 0;
                    foreach (var detail in slip.OrderBy(d => d.InstrumentCode))
                    {
                        lineno++;

                        doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_DTL, COL_MIN, ROW_DTL, COL_MAX, rowindex, 1);
                        doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_DTL));
                        doc.MergeWorksheetCells(rowindex, COL_DTL_INSTRUMENTCODE, rowindex, COL_DTL_INSTRUMENTCODE_END);
                        doc.SetCellValue(rowindex, COL_DTL_NO, lineno);
                        doc.SetCellValue(rowindex, COL_DTL_INSTRUMENTCODE, detail.InstrumentCode);
                        doc.SetCellValue(rowindex, COL_DTL_QTY, (detail.Qty ?? 0));
                        doc.SetCellValue(rowindex, COL_DTL_COST, detail.CostUsD);
                        doc.SetCellValue(rowindex, COL_DTL_TOTAL, (detail.TotalUsD ?? 0));
                        rowindex++;

                        lastrow = firstrow + lineno - 1;
                    }

                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_SUBTTL, COL_MIN, ROW_SUBTTL, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_SUBTTL));
                    doc.MergeWorksheetCells(rowindex, COL_SUBTTL_INSTRUMENTCODE, rowindex, COL_SUBTTL_INSTRUMENTCODE_END);
                    doc.SetCellValue(rowindex, COL_SUBTTL_QTY, string.Format(
                        "=SUM({0}:{1})",
                        SLConvert.ToCellReference(firstrow, COL_SUBTTL_QTY),
                        SLConvert.ToCellReference(lastrow, COL_SUBTTL_QTY)
                    ));
                    doc.SetCellValue(rowindex, COL_SUBTTL_TOTAL, string.Format(
                        "=SUM({0}:{1})",
                        SLConvert.ToCellReference(firstrow, COL_SUBTTL_TOTAL),
                        SLConvert.ToCellReference(lastrow, COL_SUBTTL_TOTAL)
                    ));
                    rowindex++;

                    //Blank Row for group separator.
                    rowindex++;
                }

                doc.DeleteWorksheet(WSNAME_DETAIL);
                doc.DeleteWorksheet(WSNAME_SUMMARY);

                doc.SaveAs(strOutputPath);
                docTemp.CloseWithoutSaving();
            }

            return strOutputPath;
        }

        public string GenerateIVS289EliminateReportSummary(List<dtEliminateReportDetail> data, doIVS289SearchCondition searchParam)
        {
            const string TEMPLATE_NAME = "IVS289.xlsx";
            const string WSNAME_DETAIL = "Detail";
            const string WSNAME_SUMMARY = "Summary";
            const string WSNAME_Working = "Sheet1";

            const int ROW_HEADER = 1;
            const int COL_HEADER_TITLE = 1;

            const int ROW_TBLHDR = 2;
            const int COL_TBLHDR_INSTRUMENTCODE = 2;
            const int COL_TBLHDR_INSTRUMENTCODE_END = 6;

            const int ROW_DTL = 3;
            const int COL_DTL_NO = 1;
            const int COL_DTL_INSTRUMENTCODE = 2;
            const int COL_DTL_INSTRUMENTCODE_END = 6;
            const int COL_DTL_QTY = 7;
            const int COL_DTL_COST = 8;
            const int COL_DTL_TOTAL = 9;

            const int ROW_TTL = 4;
            const int COL_TTL_INSTRUMENTCODE = 2;
            const int COL_TTL_INSTRUMENTCODE_END = 6;
            const int COL_TTL_QTY = 7;
            const int COL_TTL_TOTAL = 9;

            const int COL_MIN = 1;
            const int COL_MAX = 10;

            string strReportTempletePath = PathUtil.GetPathValue(PathUtil.PathName.ReportTempatePath, TEMPLATE_NAME);
            if (!File.Exists(strReportTempletePath))
            {
                throw new FileNotFoundException("Template file not found.", TEMPLATE_NAME);
            }

            string strOutputPath = PathUtil.GetTempFileName(".xlsx");
            using (SLDocument docTemp = new SLDocument(strReportTempletePath))
            using (SLDocument doc = new SLDocument(strReportTempletePath))
            {
                doc.AddWorksheet(WSNAME_Working);
                docTemp.SelectWorksheet(WSNAME_SUMMARY);
                doc.SelectWorksheet(WSNAME_Working);

                for (int i = COL_MIN; i <= COL_MAX; i++)
                {
                    doc.SetColumnWidth(i, docTemp.GetColumnWidth(i));
                }

                int rowindex = 1;

                doc.CopyCellFromWorksheet(WSNAME_SUMMARY, ROW_HEADER, COL_MIN, ROW_HEADER, COL_MAX, rowindex, 1);
                doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_HEADER));
                doc.SetCellValue(ROW_HEADER, COL_HEADER_TITLE, string.Format(
                    doc.GetCellValueAsString(ROW_HEADER, COL_HEADER_TITLE),
                    searchParam.TransferTypeText
                ));
                rowindex++;

                var qSummary = (
                    from d in data
                    group d by new { d.InstrumentCode } into grp
                    orderby grp.Key.InstrumentCode
                    select grp
                );

                doc.CopyCellFromWorksheet(WSNAME_SUMMARY, ROW_TBLHDR, COL_MIN, ROW_TBLHDR, COL_MAX, rowindex, 1);
                doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_TBLHDR));
                doc.MergeWorksheetCells(rowindex, COL_TBLHDR_INSTRUMENTCODE, rowindex, COL_TBLHDR_INSTRUMENTCODE_END);
                rowindex++;

                int firstrow, lastrow;
                firstrow = lastrow = rowindex;
                int lineno = 0;
                foreach (var detail in qSummary)
                {
                    lineno++;

                    doc.CopyCellFromWorksheet(WSNAME_SUMMARY, ROW_DTL, COL_MIN, ROW_DTL, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_DTL));
                    doc.MergeWorksheetCells(rowindex, COL_DTL_INSTRUMENTCODE, rowindex, COL_DTL_INSTRUMENTCODE_END);
                    doc.SetCellValue(rowindex, COL_DTL_NO, lineno);
                    doc.SetCellValue(rowindex, COL_DTL_INSTRUMENTCODE, detail.Key.InstrumentCode);

                    int qty = detail.Sum(d => d.Qty ?? 0);
                    decimal total = detail.Sum(d => d.TotalUsD ?? 0);

                    doc.SetCellValue(rowindex, COL_DTL_QTY, qty);
                    doc.SetCellValue(rowindex, COL_DTL_COST, string.Format(
                        "={0}/{1}",
                        SLConvert.ToCellReference(rowindex, COL_DTL_TOTAL),
                        SLConvert.ToCellReference(rowindex, COL_DTL_QTY)
                    ));
                    doc.SetCellValue(rowindex, COL_DTL_TOTAL, total);
                    rowindex++;

                    lastrow = firstrow + lineno - 1;
                }

                doc.CopyCellFromWorksheet(WSNAME_SUMMARY, ROW_TTL, COL_MIN, ROW_TTL, COL_MAX, rowindex, 1);
                doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_TTL));
                doc.MergeWorksheetCells(rowindex, COL_TTL_INSTRUMENTCODE, rowindex, COL_TTL_INSTRUMENTCODE_END);
                doc.SetCellValue(rowindex, COL_TTL_QTY, string.Format(
                    "=SUM({0}:{1})",
                    SLConvert.ToCellReference(firstrow, COL_TTL_QTY),
                    SLConvert.ToCellReference(lastrow, COL_TTL_QTY)
                ));
                doc.SetCellValue(rowindex, COL_TTL_TOTAL, string.Format(
                    "=SUM({0}:{1})",
                    SLConvert.ToCellReference(firstrow, COL_TTL_TOTAL),
                    SLConvert.ToCellReference(lastrow, COL_TTL_TOTAL)
                ));


                doc.DeleteWorksheet(WSNAME_DETAIL);
                doc.DeleteWorksheet(WSNAME_SUMMARY);

                doc.SaveAs(strOutputPath);
                docTemp.CloseWithoutSaving();
            }

            return strOutputPath;
        }

        public string GenerateIVS290BufferLossReport(List<dtBufferLossReportDetail> data)
        {
            const string TEMPLATE_NAME = "IVS290.xlsx";
            const string WSNAME_DETAIL = "Detail";
            const string WSNAME_SUMMARY = "Summary";
            const string WSNAME_Working = "Sheet1";

            const int ROW_HEADER = 1;
            const int COL_HEADER_TITLE = 1;

            const int ROW_GRPHDR1 = 2;
            const int COL_GRPHDR1_SLIPNO = 2;
            const int COL_GRPHDR1_STOCKOUTDATE = 4;

            const int ROW_TBLHDR = 3;
            const int COL_TBLHDR_INSTRUMENTCODE = 2;
            const int COL_TBLHDR_INSTRUMENTCODE_END = 6;

            const int ROW_DTL = 4;
            const int COL_DTL_NO = 1;
            const int COL_DTL_INSTRUMENTCODE = 2;
            const int COL_DTL_INSTRUMENTCODE_END = 6;
            const int COL_DTL_QTY = 7;
            const int COL_DTL_COST = 8;
            const int COL_DTL_TOTAL = 9;

            const int ROW_SUBTTL = 5;
            const int COL_SUBTTL_INSTRUMENTCODE = 2;
            const int COL_SUBTTL_INSTRUMENTCODE_END = 6;
            const int COL_SUBTTL_QTY = 7;
            const int COL_SUBTTL_TOTAL = 9;

            const int COL_MIN = 1;
            const int COL_MAX = 10;

            var commonUtil = new CommonUtil();

            string strReportTempletePath = PathUtil.GetPathValue(PathUtil.PathName.ReportTempatePath, TEMPLATE_NAME);
            if (!File.Exists(strReportTempletePath))
            {
                throw new FileNotFoundException("Template file not found.", TEMPLATE_NAME);
            }

            string strOutputPath = PathUtil.GetTempFileName(".xlsx");
            using (SLDocument docTemp = new SLDocument(strReportTempletePath))
            using (SLDocument doc = new SLDocument(strReportTempletePath))
            {
                doc.AddWorksheet(WSNAME_Working);
                docTemp.SelectWorksheet(WSNAME_DETAIL);
                doc.SelectWorksheet(WSNAME_Working);

                for (int i = COL_MIN; i <= COL_MAX; i++)
                {
                    doc.SetColumnWidth(i, docTemp.GetColumnWidth(i));
                }

                int rowindex = 1;

                doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_HEADER, COL_MIN, ROW_HEADER, COL_MAX, rowindex, 1);
                doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_HEADER));
                doc.SetCellValue(ROW_HEADER, COL_HEADER_TITLE, doc.GetCellValueAsString(ROW_HEADER, COL_HEADER_TITLE));
                rowindex++;

                var qGroupBySlip = (
                    from d in data
                    group d by new
                    {
                        d.SlipNo,
                        d.StockOutDate,
                    } into grp
                    orderby grp.Key.SlipNo, grp.Key.StockOutDate
                    select grp
                );

                foreach (var slip in qGroupBySlip)
                {
                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_GRPHDR1, COL_MIN, ROW_GRPHDR1, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_GRPHDR1));
                    doc.SetCellValue(rowindex, COL_GRPHDR1_SLIPNO, slip.Key.SlipNo);
                    doc.SetCellValue(rowindex, COL_GRPHDR1_STOCKOUTDATE, (slip.Key.StockOutDate == null ? "" : slip.Key.StockOutDate.Value.ToString("dd/MM/yyyy")));
                    rowindex++;

                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_TBLHDR, COL_MIN, ROW_TBLHDR, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_TBLHDR));
                    doc.MergeWorksheetCells(rowindex, COL_TBLHDR_INSTRUMENTCODE, rowindex, COL_TBLHDR_INSTRUMENTCODE_END);
                    rowindex++;

                    int firstrow, lastrow;
                    firstrow = lastrow = rowindex;
                    int lineno = 0;
                    foreach (var detail in slip.OrderBy(d => d.InstrumentCode))
                    {
                        lineno++;

                        doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_DTL, COL_MIN, ROW_DTL, COL_MAX, rowindex, 1);
                        doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_DTL));
                        doc.MergeWorksheetCells(rowindex, COL_DTL_INSTRUMENTCODE, rowindex, COL_DTL_INSTRUMENTCODE_END);
                        doc.SetCellValue(rowindex, COL_DTL_NO, lineno);
                        doc.SetCellValue(rowindex, COL_DTL_INSTRUMENTCODE, detail.InstrumentCode);
                        doc.SetCellValue(rowindex, COL_DTL_QTY, (detail.Qty ?? 0));
                        doc.SetCellValue(rowindex, COL_DTL_COST, (detail.CostUsD ?? 0));
                        doc.SetCellValue(rowindex, COL_DTL_TOTAL, (detail.TotalUsD ?? 0));
                        rowindex++;

                        lastrow = firstrow + lineno - 1;
                    }

                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_SUBTTL, COL_MIN, ROW_SUBTTL, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_SUBTTL));
                    doc.MergeWorksheetCells(rowindex, COL_SUBTTL_INSTRUMENTCODE, rowindex, COL_SUBTTL_INSTRUMENTCODE_END);
                    doc.SetCellValue(rowindex, COL_SUBTTL_QTY, string.Format(
                        "=SUM({0}:{1})",
                        SLConvert.ToCellReference(firstrow, COL_SUBTTL_QTY),
                        SLConvert.ToCellReference(lastrow, COL_SUBTTL_QTY)
                    ));
                    doc.SetCellValue(rowindex, COL_SUBTTL_TOTAL, string.Format(
                        "=SUM({0}:{1})",
                        SLConvert.ToCellReference(firstrow, COL_SUBTTL_TOTAL),
                        SLConvert.ToCellReference(lastrow, COL_SUBTTL_TOTAL)
                    ));
                    rowindex++;

                    //Blank Row for group separator.
                    rowindex++;
                }

                doc.DeleteWorksheet(WSNAME_DETAIL);
                doc.DeleteWorksheet(WSNAME_SUMMARY);

                doc.SaveAs(strOutputPath);
                docTemp.CloseWithoutSaving();
            }

            return strOutputPath;
        }

        public string GenerateIVS290BufferLossReportSummary(List<dtBufferLossReportDetail> data)
        {
            const string TEMPLATE_NAME = "IVS290.xlsx";
            const string WSNAME_DETAIL = "Detail";
            const string WSNAME_SUMMARY = "Summary";
            const string WSNAME_Working = "Sheet1";

            const int ROW_HEADER = 1;
            const int COL_HEADER_TITLE = 1;

            const int ROW_TBLHDR = 2;
            const int COL_TBLHDR_INSTRUMENTCODE = 2;
            const int COL_TBLHDR_INSTRUMENTCODE_END = 6;

            const int ROW_DTL = 3;
            const int COL_DTL_NO = 1;
            const int COL_DTL_INSTRUMENTCODE = 2;
            const int COL_DTL_INSTRUMENTCODE_END = 6;
            const int COL_DTL_QTY = 7;
            const int COL_DTL_COST = 8;
            const int COL_DTL_TOTAL = 9;

            const int ROW_TTL = 4;
            const int COL_TTL_INSTRUMENTCODE = 2;
            const int COL_TTL_INSTRUMENTCODE_END = 6;
            const int COL_TTL_QTY = 7;
            const int COL_TTL_TOTAL = 9;

            const int COL_MIN = 1;
            const int COL_MAX = 10;

            string strReportTempletePath = PathUtil.GetPathValue(PathUtil.PathName.ReportTempatePath, TEMPLATE_NAME);
            if (!File.Exists(strReportTempletePath))
            {
                throw new FileNotFoundException("Template file not found.", TEMPLATE_NAME);
            }

            string strOutputPath = PathUtil.GetTempFileName(".xlsx");
            using (SLDocument docTemp = new SLDocument(strReportTempletePath))
            using (SLDocument doc = new SLDocument(strReportTempletePath))
            {
                doc.AddWorksheet(WSNAME_Working);
                docTemp.SelectWorksheet(WSNAME_SUMMARY);
                doc.SelectWorksheet(WSNAME_Working);

                for (int i = COL_MIN; i <= COL_MAX; i++)
                {
                    doc.SetColumnWidth(i, docTemp.GetColumnWidth(i));
                }

                int rowindex = 1;

                doc.CopyCellFromWorksheet(WSNAME_SUMMARY, ROW_HEADER, COL_MIN, ROW_HEADER, COL_MAX, rowindex, 1);
                doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_HEADER));
                doc.SetCellValue(ROW_HEADER, COL_HEADER_TITLE, doc.GetCellValueAsString(ROW_HEADER, COL_HEADER_TITLE));
                rowindex++;

                var qSummary = (
                    from d in data
                    group d by new { d.InstrumentCode } into grp
                    orderby grp.Key.InstrumentCode
                    select grp
                );

                doc.CopyCellFromWorksheet(WSNAME_SUMMARY, ROW_TBLHDR, COL_MIN, ROW_TBLHDR, COL_MAX, rowindex, 1);
                doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_TBLHDR));
                doc.MergeWorksheetCells(rowindex, COL_TBLHDR_INSTRUMENTCODE, rowindex, COL_TBLHDR_INSTRUMENTCODE_END);
                rowindex++;

                int firstrow, lastrow;
                firstrow = lastrow = rowindex;
                int lineno = 0;
                foreach (var detail in qSummary)
                {
                    lineno++;

                    doc.CopyCellFromWorksheet(WSNAME_SUMMARY, ROW_DTL, COL_MIN, ROW_DTL, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_DTL));
                    doc.MergeWorksheetCells(rowindex, COL_DTL_INSTRUMENTCODE, rowindex, COL_DTL_INSTRUMENTCODE_END);
                    doc.SetCellValue(rowindex, COL_DTL_NO, lineno);
                    doc.SetCellValue(rowindex, COL_DTL_INSTRUMENTCODE, detail.Key.InstrumentCode);

                    int qty = detail.Sum(d => d.Qty ?? 0);
                    decimal total = detail.Sum(d => d.TotalUsD ?? 0);

                    doc.SetCellValue(rowindex, COL_DTL_QTY, qty);
                    doc.SetCellValue(rowindex, COL_DTL_COST, string.Format(
                        "={0}/{1}",
                        SLConvert.ToCellReference(rowindex, COL_DTL_TOTAL),
                        SLConvert.ToCellReference(rowindex, COL_DTL_QTY)
                    ));
                    doc.SetCellValue(rowindex, COL_DTL_TOTAL, total);
                    rowindex++;

                    lastrow = firstrow + lineno - 1;
                }

                doc.CopyCellFromWorksheet(WSNAME_SUMMARY, ROW_TTL, COL_MIN, ROW_TTL, COL_MAX, rowindex, 1);
                doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_TTL));
                doc.MergeWorksheetCells(rowindex, COL_TTL_INSTRUMENTCODE, rowindex, COL_TTL_INSTRUMENTCODE_END);
                doc.SetCellValue(rowindex, COL_TTL_QTY, string.Format(
                    "=SUM({0}:{1})",
                    SLConvert.ToCellReference(firstrow, COL_TTL_QTY),
                    SLConvert.ToCellReference(lastrow, COL_TTL_QTY)
                ));
                doc.SetCellValue(rowindex, COL_TTL_TOTAL, string.Format(
                    "=SUM({0}:{1})",
                    SLConvert.ToCellReference(firstrow, COL_TTL_TOTAL),
                    SLConvert.ToCellReference(lastrow, COL_TTL_TOTAL)
                ));


                doc.DeleteWorksheet(WSNAME_DETAIL);
                doc.DeleteWorksheet(WSNAME_SUMMARY);

                doc.SaveAs(strOutputPath);
                docTemp.CloseWithoutSaving();
            }

            return strOutputPath;
        }

        // IVR192-Purchase order
        public Stream GenerateIVR192(string strPurchaseOrderNo, string strOfficeCode, string strEmpNo, DateTime dtDateTime)
        {
            string strFilePath = GenerateIVR192FilePath(strPurchaseOrderNo, strOfficeCode, strEmpNo, dtDateTime);
            IDocumentHandler handlerDocument = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            return handlerDocument.GetDocumentReportFileStream(strFilePath);
        }
        public string GenerateIVR192FilePath(string strPurchaseOrderNo, string strOfficeCode, string strEmpNo, DateTime dtDateTime)
        {
            IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            var ivr192 = base.GetIVR192(strPurchaseOrderNo);

            List<tbm_DocumentTemplate> dLst = documentHandler.GetDocumentTemplateByDocumentCode(ReportID.C_INV_REPORT_ID_PURCHASE_ORDER);

            if (ivr192.Count == 0)
            {
                return null;
            } //end if

            //{
            //    var tmp = new List<doIVR192>();
            //    for (int i = 0; i < 50; i++)
            //    {
            //        tmp.Add(ivr192.First());

            //        doDocumentDataGenerate doDocumentTmp = new doDocumentDataGenerate();
            //        doDocumentTmp.DocumentNo = strPurchaseOrderNo;
            //        doDocumentTmp.DocumentCode = ReportID.C_INV_REPORT_ID_PURCHASE_ORDER;
            //        doDocumentTmp.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
            //        doDocumentTmp.DocumentData = tmp;
            //        doDocumentTmp.OtherKey.InventorySlipIssueOffice = strOfficeCode;
            //        doDocumentTmp.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            //        doDocumentTmp.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;

            //        // Additional
            //        doDocumentTmp.EmpNo = strEmpNo;
            //        doDocumentTmp.ProcessDateTime = dtDateTime;

            //        List<ReportParameterObject> listMainReportParamTmp = new List<ReportParameterObject>();
            //        if (dLst.Count > 0)
            //        {
            //            listMainReportParamTmp.Add(new ReportParameterObject() { ParameterName = "DocVersion", Value = dLst[0].DocumentVersion });
            //            listMainReportParamTmp.Add(new ReportParameterObject() { ParameterName = "TemplateName", Value = dLst[0].DocumentNameEN });
            //        }
            //        doDocumentTmp.MainReportParam = listMainReportParamTmp;

            //        string tmpPath = documentHandler.GenerateDocumentFilePath(doDocumentTmp);
            //    }
            //}

            doDocumentDataGenerate doDocument = new doDocumentDataGenerate();
            doDocument.DocumentNo = strPurchaseOrderNo;
            doDocument.DocumentCode = ReportID.C_INV_REPORT_ID_PURCHASE_ORDER;
            doDocument.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
            doDocument.DocumentData = ivr192;
            doDocument.OtherKey.InventorySlipIssueOffice = strOfficeCode;
            doDocument.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            doDocument.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;

            // Additional
            doDocument.EmpNo = strEmpNo;
            doDocument.ProcessDateTime = dtDateTime;

            List<ReportParameterObject> listMainReportParam = new List<ReportParameterObject>();
            if (dLst.Count > 0)
            {
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "DocVersion", Value = dLst[0].DocumentVersion });
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "TemplateName", Value = dLst[0].DocumentNameEN });
            }
            doDocument.MainReportParam = listMainReportParam;

            string slipNoReportPath = documentHandler.GenerateDocumentFilePath(doDocument);
            return slipNoReportPath;
        }

        public string GenerateIVS170StockTakingResult(List<dtStockCheckingList> data, doGetStockCheckingList cond)
        {
            const string TEMPLATE_NAME = "IVS170.xlsx";
            const string WSNAME_DETAIL = "Detail";
            const string WSNAME_Working = "Sheet1";

            const int COL_MIN = 1;
            const int COL_MAX = 10;

            const int ROW_HEADER1 = 1;
            const int ROW_HEADER2 = 2;
            const int COL_HEADER2_YEARMONTH = 2;
            const int ROW_HEADER3 = 3;
            const int COL_HEADER3_OFFICE = 2;
            const int COL_HEADER3_LOCATION = 5;
            const int ROW_HEADER4 = 4;
            const int COL_HEADER4_AREA = 2;
            const int ROW_HEADER5 = 5;
            const int COL_HEADER5_SHELF = 2;
            const int ROW_HEADER6 = 6;
            const int COL_HEADER6_INSTRUMENTCODE = 2;
            const int COL_HEADER6_INSTRUMENTNAME = 5;

            const int ROW_TBLHDR1 = 7;
            const int ROW_TBLHDR2 = 8;
            const int COL_TBLHDR2_OFFICE = 2;
            const int COL_TBLHDR2_LOCATION = 5;
            const int COL_TBLHDR2_AREA = 8;
            const int ROW_TBLHDR3 = 9;

            const int ROW_DTL = 10;
            const int COL_DTL_INSTRUMENTCODE = 1;
            const int COL_DTL_INSTRUMENTNAME = 2;
            const int COL_DTL_INSTRUMENTNAME_END = 4;
            const int COL_DTL_SHELF = 5;
            const int COL_DTL_LOGICALQTY = 6;
            const int COL_DTL_ACTUALQTY = 7;
            const int COL_DTL_DIFFQTY = 8;
            const int COL_DTL_LASTCHECKINGDATE = 9;

            string strReportTempletePath = PathUtil.GetPathValue(PathUtil.PathName.ReportTempatePath, TEMPLATE_NAME);
            if (!File.Exists(strReportTempletePath))
            {
                throw new FileNotFoundException("Template file not found.", TEMPLATE_NAME);
            }

            string strOutputPath = PathUtil.GetTempFileName(".xlsx");
            using (SLDocument docTemp = new SLDocument(strReportTempletePath))
            using (SLDocument doc = new SLDocument(strReportTempletePath))
            {
                doc.AddWorksheet(WSNAME_Working);
                docTemp.SelectWorksheet(WSNAME_DETAIL);
                doc.SelectWorksheet(WSNAME_Working);

                for (int i = COL_MIN; i <= COL_MAX; i++)
                {
                    doc.SetColumnWidth(i, docTemp.GetColumnWidth(i));
                }

                int rowindex = 1;

                doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_HEADER1, COL_MIN, ROW_HEADER1, COL_MAX, rowindex, 1);
                doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_HEADER1));
                rowindex++;

                doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_HEADER2, COL_MIN, ROW_HEADER2, COL_MAX, rowindex, 1);
                doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_HEADER2));
                doc.SetCellValue(rowindex, COL_HEADER2_YEARMONTH, cond.CheckingYearMonth);
                rowindex++;

                doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_HEADER3, COL_MIN, ROW_HEADER3, COL_MAX, rowindex, 1);
                doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_HEADER3));
                doc.SetCellValue(rowindex, COL_HEADER3_OFFICE, cond.OfficeText);
                doc.SetCellValue(rowindex, COL_HEADER3_LOCATION, cond.LocationText);
                rowindex++;

                doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_HEADER4, COL_MIN, ROW_HEADER4, COL_MAX, rowindex, 1);
                doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_HEADER4));
                doc.SetCellValue(rowindex, COL_HEADER4_AREA, cond.AreaText);
                rowindex++;

                doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_HEADER5, COL_MIN, ROW_HEADER5, COL_MAX, rowindex, 1);
                doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_HEADER5));
                if (!string.IsNullOrEmpty(cond.ShelfNoFrom) || !string.IsNullOrEmpty(cond.ShelfNoTo))
                {
                    doc.SetCellValue(rowindex, COL_HEADER5_SHELF, cond.ShelfNoFrom + " ~ " + cond.ShelfNoTo);
                }
                rowindex++;

                doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_HEADER6, COL_MIN, ROW_HEADER6, COL_MAX, rowindex, 1);
                doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_HEADER6));
                doc.SetCellValue(rowindex, COL_HEADER6_INSTRUMENTCODE, cond.InstrumentCode);
                doc.SetCellValue(rowindex, COL_HEADER6_INSTRUMENTNAME, cond.InstrumentName);
                rowindex++;

                var qGroupList = (
                    from d in data
                    group d by new { d.GroupingKey, d.OfficeName, d.LocationName, d.AreaCodeName } into grp
                    orderby grp.Key.GroupingKey
                    select grp
                );

                foreach (var qGrouping in qGroupList)
                {
                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_TBLHDR1, COL_MIN, ROW_TBLHDR1, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_TBLHDR1));
                    rowindex++;

                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_TBLHDR2, COL_MIN, ROW_TBLHDR2, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_TBLHDR2));
                    doc.SetCellValue(rowindex, COL_TBLHDR2_OFFICE, qGrouping.Key.OfficeName);
                    doc.SetCellValue(rowindex, COL_TBLHDR2_LOCATION, qGrouping.Key.LocationName);
                    doc.SetCellValue(rowindex, COL_TBLHDR2_AREA, qGrouping.Key.AreaCodeName);
                    rowindex++;

                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_TBLHDR3, COL_MIN, ROW_TBLHDR3, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_TBLHDR3));
                    doc.MergeWorksheetCells(rowindex, COL_DTL_INSTRUMENTNAME, rowindex, COL_DTL_INSTRUMENTNAME_END);
                    rowindex++;

                    int firstrow, lastrow;
                    firstrow = lastrow = rowindex;
                    int lineno = 0;
                    foreach (var detail in qGrouping)
                    {
                        lineno++;

                        doc.CopyCellFromWorksheet(WSNAME_DETAIL, ROW_DTL, COL_MIN, ROW_DTL, COL_MAX, rowindex, 1);
                        doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_DTL));
                        doc.MergeWorksheetCells(rowindex, COL_DTL_INSTRUMENTNAME, rowindex, COL_DTL_INSTRUMENTNAME_END);
                        doc.SetCellValue(rowindex, COL_DTL_INSTRUMENTCODE, detail.InstrumentCode);
                        doc.SetCellValue(rowindex, COL_DTL_INSTRUMENTNAME, detail.InstrumentName);
                        doc.SetCellValue(rowindex, COL_DTL_SHELF, detail.ShelfNo);
                        doc.SetCellValue(rowindex, COL_DTL_LOGICALQTY, (detail.InstrumentQty ?? 0));
                        doc.SetCellValue(rowindex, COL_DTL_ACTUALQTY, (detail.CheckingQty ?? 0));
                        doc.SetCellValue(rowindex, COL_DTL_DIFFQTY, (detail.DiffQty ?? 0));
                        if ((detail.DiffQty ?? 0) != 0)
                        {
                            var style = doc.GetCellStyle(rowindex, COL_DTL_DIFFQTY);
                            style.Font.FontColor = ((detail.DiffQty ?? 0) > 0 ? Color.Green : Color.Red);
                            doc.SetCellStyle(rowindex, COL_DTL_DIFFQTY, style);
                        }
                        doc.SetCellValue(rowindex, COL_DTL_LASTCHECKINGDATE, (detail.UpdateDate == null ? "" : detail.UpdateDate.Value.ToString("dd-MM-yyyy")));
                        rowindex++;

                        lastrow = firstrow + lineno - 1;
                    }
                }

                doc.DeleteWorksheet(WSNAME_DETAIL);

                doc.SaveAs(strOutputPath);
                docTemp.CloseWithoutSaving();
            }

            return strOutputPath;
        }

        #endregion

        #region Private

        private List<doOffice> GetInventoryHeadOffice()
        {
            try
            {
                //Modified by Non A. 15/Feb/2012 : HeadOffice should always return only 1 doOffice.
                var lstTmp = base.GetInventoryHeadOffice(InventoryHeadOffice.C_OFFICLOGISTIC_HEAD);
                if (lstTmp != null && lstTmp.Capacity > 1)
                {
                    for (int i = 1; i < lstTmp.Count; i++)
                    {
                        lstTmp.RemoveAt(i);
                    }
                }
                return lstTmp;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

    }
}
