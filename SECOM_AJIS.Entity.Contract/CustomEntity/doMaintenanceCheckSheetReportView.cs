using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class doMaintenanceCheckSheetReportView
    {
        private doMaintenanceCheckSheetReport _main;
        private doMaintenanceCheckSheetReport _m1;
        private doMaintenanceCheckSheetReport _m2;
        private doMaintenanceCheckSheetReport _r1;

        public doMaintenanceCheckSheetReportView()
        {
            _m1 = new doMaintenanceCheckSheetReport();
        }
        public doMaintenanceCheckSheetReportView(doMaintenanceCheckSheetReport main, doMaintenanceCheckSheetReport m1) : this(main, m1, null, null) { }
        public doMaintenanceCheckSheetReportView(doMaintenanceCheckSheetReport main, doMaintenanceCheckSheetReport m1, doMaintenanceCheckSheetReport m2) : this(main, m1, m2, null) { }
        public doMaintenanceCheckSheetReportView(doMaintenanceCheckSheetReport main, doMaintenanceCheckSheetReport m1, doMaintenanceCheckSheetReport m2, doMaintenanceCheckSheetReport r1)
        {
            _main = (main ?? m1 ?? m2 ?? r1);
            if (_main == null)
            {
                throw new ArgumentNullException("all parameters cannot be null.");
            }
            _m1 = m1;
            _m2 = m2;
            _r1 = r1;
        }

        public string ContractCode
        {
            get { return new CommonUtil().ConvertContractCode(_main.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT); }
        }

        public string UserCode
        {
            get { return _main.UserCode; }
        }

        public string MaintenanceMonth
        {
            get { return _main.MaintenanceMonth; }
        }

        public string MaintenanceYear
        {
            get { return _main.MaintenanceYear; }
        }

        public string ContractTargetNameEN
        {
            get { return _main.ContractTargetNameEN; }
        }

        public string ContractTargetNameLC
        {
            get { return _main.ContractTargetNameLC; }
        }

        public string SiteNameEN
        {
            get { return _main.SiteNameEN; }
        }

        public string SiteNameLC
        {
            get { return _main.SiteNameLC; }
        }

        public string SecurityTypeCode
        {
            get { return _main.SecurityTypeCode; }
        }

        public string NextMaintenanceMonth
        {
            get { return _main.NextMaintenanceMonth; }
        }

        public string NextMaintenanceYear
        {
            get { return _main.NextMaintenanceYear; }
        }

        public string Type
        {
            get { return _main.Type; }
        }

        public string CheckupNo
        {
            get { return _main.CheckupNo; }
        }

        public string OCC
        {
            get { return _main.OCC; }
        }

        public string QuotationTargetCode
        {
            get { return new CommonUtil().ConvertQuotationTargetCode(_main.QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT); }
        }

        public string QuotationAlphabet
        {
            get { return _main.QuotationAlphabet; }
        }

        public string ContractOfficeCode
        {
            get { return _main.ContractOfficeCode; }
        }

        public string OperationOfficeCode
        {
            get { return _main.OperationOfficeCode; }
        }

        public string ContractTargetCustCode
        {
            get { return new CommonUtil().ConvertCustCode(_main.ContractTargetCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT); }
        }

        public string InstrumentCodeM1
        {
            get { return (_m1 == null ? null : _m1.InstrumentCode); }
        }

        public string InstrumentCodeM2
        {
            get { return (_m2 == null ? null : _m2.InstrumentCode); }
        }

        public string InstrumentCodeR1
        {
            get { return (_r1 == null ? null : _r1.InstrumentCode); }
        }

        public int QtyM1
        {
            get { return (_m1 == null ? 0 : (_m1.Qty ?? 0)); }
        }

        public int QtyM2
        {
            get { return (_m2 == null ? 0 : (_m2.Qty ?? 0)); }
        }

        public int QtyR1
        {
            get { return (_r1 == null ? 0 : (_r1.Qty ?? 0)); }
        }

        public int RowsPerPage
        {
            get { return _main.RowsPerPage; }
        }

        public string OperationOfficeNameEN
        {
            get
            {
                return _main.OperationOfficeNameEN;
            }
        }

        public string OperationOfficeNameJP
        {
            get
            {
                return _main.OperationOfficeNameJP;
            }
        }

        public string OperationOfficeNameLC
        {
            get
            {
                return _main.OperationOfficeNameLC;
            }
        }
    }

    public static class doMaintenanceCheckSheetReportViewUtil
    {
        private static int ROWS_PER_PAGE = 23;

        public static List<doMaintenanceCheckSheetReportView> ToReportViewList(this List<doMaintenanceCheckSheetReport> data)
        {
            var lstView = new List<doMaintenanceCheckSheetReportView>();

            if (data != null)
            {
                doMaintenanceCheckSheetReport main = data.FirstOrDefault();

                if (main != null)
                {
                    var lstM = data.Where(d => d.Type == "M").OrderBy(d => d.InstrumentCode).ToList(); //Maintenance Instrument
                    var lstR = data.Where(d => d.Type == "R").OrderBy(d => d.InstrumentCode).ToList(); //Replace Instrument

                    int rowsperpage = (main.RowsPerPage > 0 ? main.RowsPerPage : ROWS_PER_PAGE);
                    decimal totalrows = Math.Max(Math.Ceiling(lstM.Count / 2m), lstR.Count);
                    if (totalrows <= 0)
                    {
                        totalrows = rowsperpage;
                    }

                    decimal totalpages = Math.Ceiling(totalrows / rowsperpage);

                    for (int page = 0; page < totalpages; page++)
                    {
                        for (int i = 0; i < rowsperpage; i++)
                        {
                            int idxM1, idxM2, idxR1;
                            idxM1 = (page * rowsperpage * 2) + i;
                            idxM2 = (page * rowsperpage * 2) + rowsperpage + i;
                            idxR1 = i;

                            doMaintenanceCheckSheetReport m1, m2, r1;
                            m1 = (idxM1 >= lstM.Count ? null : lstM[idxM1]);
                            m2 = (idxM2 >= lstM.Count ? null : lstM[idxM2]);
                            r1 = (idxR1 >= lstR.Count ? null : lstR[idxR1]);

                            lstView.Add(new doMaintenanceCheckSheetReportView(main, m1, m2, r1));
                        }
                    }
                }
            }

            return lstView;
        }
    }
}
