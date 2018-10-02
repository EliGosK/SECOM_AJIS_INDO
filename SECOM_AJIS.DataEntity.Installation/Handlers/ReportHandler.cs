using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.Net.Sockets;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Models;
using CSI.WindsorHelper;
using System.IO;
using SECOM_AJIS.DataEntity.Common;

namespace SECOM_AJIS.DataEntity.Installation
{
    public class ReportHandler : BizISDataEntities, IReportHandler
    {
        public List<RPTNewRentalSlipDo> GetRptNewRentalSlipReport(string strSlipNo)
        {
            List<RPTNewRentalSlipDo> lstRptNewRetalSlipData = base.GetRptNewRetalSlipData(strSlipNo, ScreenID.C_SCREEN_ID_INSTALL_SLIP, MiscType.C_STOCK_OUT_TYPE); //Modify (Add C_SCREEN_ID_INSTALL_SLIP, C_STOCK_OUT_TYPE) by Jutarat A. on 21102013         

            return lstRptNewRetalSlipData;
        }

        public List<RPTNewRentalSlipDo> GetRptNewRentalSlipReport_TestData(string strSlipNo)
        {
            List<RPTNewRentalSlipDo> lstRptNewRetalSlipData = new List<RPTNewRentalSlipDo>();

            for (int i = 1; i <= 30; i++)
            {
                RPTNewRentalSlipDo doRpt = new RPTNewRentalSlipDo();
                
                doRpt.SlipIssueDate = DateTime.Now.ToString("dd-MMM-yyyy");
                doRpt.SlipNo = "00011100001";
                doRpt.ContractCode = "N00000015";
                doRpt.PlanCode = "000000001";
                doRpt.MaintenanceNo = "0001N20100001";
                doRpt.ExpectedInstrumentArrivalDate = DateTime.Now.ToString("dd-MMM-yyyy");
                doRpt.UserCode = "00005";
                doRpt.PreviousSlipNo = "-";
                doRpt.OfficeNameEN = "ดอนเมือง";
                doRpt.OtherSubContractor = 2;
                doRpt.SubContractorNameEN = "ผู้รับเหมาที่ 1";
                doRpt.SiteNameEN = "เอ็กเช้นจ์ ทาวเวอร์";
                doRpt.AddressFullEN = "338 ชั้น 33, ถนน สุขุมวิท แขวง คลองเตย เขต วัฒนา จังหวัด กรุงเทพ 10310";

                doRpt.InstrumentCode = "000000000" + i;
                doRpt.CurrentStockOutQty = 9999;
                doRpt.TotalStockOutQty = 9999;
                doRpt.ReturnQty = 9999;
                doRpt.ContractInstalledQty = 9999;
                doRpt.ContractCode = "N0000000001";
                doRpt.OperationOfficeCode = "4010";
                doRpt.SlipIssueOfficeCode = "3050";

                lstRptNewRetalSlipData.Add(doRpt);
            }

            return lstRptNewRetalSlipData;
        }

        public List<RPTChangeSlipDo> GetRptChangeSlipReport(string strSlipNo)
        {
            List<RPTChangeSlipDo> lstRptChangeSlipData = base.GetRptChangeSlipData(strSlipNo, ServiceType.C_SERVICE_TYPE_SALE, ServiceType.C_SERVICE_TYPE_RENTAL, MiscType.C_SALE_INSTALL_TYPE, MiscType.C_RENTAL_INSTALL_TYPE, ScreenID.C_SCREEN_ID_INSTALL_SLIP, MiscType.C_STOCK_OUT_TYPE); //Modify (Add C_SCREEN_ID_INSTALL_SLIP, C_STOCK_OUT_TYPE) by Jutarat A. on 21102013            

            return lstRptChangeSlipData;
        }

        public List<RPTChangeSlipDo> GetRptChangeSlipReport_TestData(string strSlipNo)
        {
            List<RPTChangeSlipDo> lstRptChangeSlipData = new List<RPTChangeSlipDo>();

            for (int i = 1; i <=30; i++)
            {
                RPTChangeSlipDo doRpt = new RPTChangeSlipDo();

                doRpt.SlipIssueDate = DateTime.Now.ToString("dd-MMM-yyyy");
                doRpt.SlipNo = "00011100001";
                doRpt.ContractCode = "N00000015";
                doRpt.PlanCode = "000000001";
                doRpt.MaintenanceNo = "0001N20100001";
                doRpt.ExpectedInstrumentArrivalDate = DateTime.Now.ToString("dd-MMM-yyyy");
                doRpt.UserCode = "00005";
                doRpt.PreviousSlipNo = "-";
                doRpt.OfficeNameEN = "ดอนเมือง";
                doRpt.OtherSubContractor = 2;
                doRpt.SubContractorNameEN = "ผู้รับเหมาที่ 1";
                doRpt.SiteNameEN = "เอ็กเช้นจ์ ทาวเวอร์";
                doRpt.AddressFullEN = "338 ชั้น 33, ถนน สุขุมวิท แขวง คลองเตย เขต วัฒนา จังหวัด กรุงเทพ 10310";
                doRpt.InstallationTypeName = "Change";

                doRpt.InstrumentCode = "000000000" + i;
                doRpt.CurrentStockOutQty = 9999;
                doRpt.TotalStockOutQty = 9999;
                doRpt.ReturnQty = 9999;
                doRpt.ContractInstalledQty = 9999;
                doRpt.ContractRemovedQty = 9999;
                doRpt.MoveQty = 9999;

                lstRptChangeSlipData.Add(doRpt);
            }

            return lstRptChangeSlipData;
        }

        public List<RPTRemoveSlipDo> GetRptRemoveSlipReport(string strSlipNo)
        {
            List<RPTRemoveSlipDo> lstRptRemoveSlipData = base.GetRptRemoveSlipData(strSlipNo, ServiceType.C_SERVICE_TYPE_SALE, ServiceType.C_SERVICE_TYPE_RENTAL, ScreenID.C_SCREEN_ID_INSTALL_SLIP, MiscType.C_STOCK_OUT_TYPE); //Modify (Add C_SCREEN_ID_INSTALL_SLIP, C_STOCK_OUT_TYPE) by Jutarat A. on 21102013           

            return lstRptRemoveSlipData;
        }

        public List<RPTRemoveSlipDo> GetRptRemoveSlipReport_TestData(string strSlipNo)
        {
            List<RPTRemoveSlipDo> lstRptRemoveSlipData = new List<RPTRemoveSlipDo>();

            for (int i = 1; i <= 25; i++)
            {
                RPTRemoveSlipDo doRpt = new RPTRemoveSlipDo();

                doRpt.SlipIssueDate = DateTime.Now.ToString("dd-MMM-yyyy");
                doRpt.SlipNo = "00011100001";
                doRpt.ContractCode = "N00000015";
                doRpt.PlanCode = "000000001";
                doRpt.MaintenanceNo = "0001N20100001";
                doRpt.UserCode = "00005";
                doRpt.PreviousSlipNo = "-";
                doRpt.OfficeNameLC = "ดอนเมือง";
                doRpt.OtherSubContractor = 2;
                doRpt.SubContractorNameLC = "ผู้รับเหมาที่ 1";
                doRpt.SiteNameLC = "เอ็กเช้นจ์ ทาวเวอร์";
                doRpt.AddressFullLC = "338 ชั้น 33, ถนน สุขุมวิท แขวง คลองเตย เขต วัฒนา จังหวัด กรุงเทพ 10310";

                doRpt.InstrumentCode = "000000000" + i;
                doRpt.AddRemovedQty = 9999;

                lstRptRemoveSlipData.Add(doRpt);
            }

            return lstRptRemoveSlipData;
        }

        public List<RPTPOSubPriceDo> GetRptPOSubPriceData(string strMaintenanceNo, string strSubcontractorCode)
        {
            List<RPTPOSubPriceDo> lstPOSubPriceData = base.GetRptPOSubPriceData(strMaintenanceNo, strSubcontractorCode, ServiceType.C_SERVICE_TYPE_SALE, ServiceType.C_SERVICE_TYPE_RENTAL, ServiceType.C_SERVICE_TYPE_PROJECT, MiscType.C_SALE_INSTALL_TYPE, MiscType.C_RENTAL_INSTALL_TYPE);
            return lstPOSubPriceData;
        }

        public List<RPTNewSaleSlipDo> GetRptNewSaleSlipReport(string strSlipNo)
        {
            List<RPTNewSaleSlipDo> lstRptNewSaleSlipData = base.GetRptNewSaleSlipData(strSlipNo, MiscType.C_SALE_INSTALL_TYPE, ScreenID.C_SCREEN_ID_INSTALL_SLIP, MiscType.C_STOCK_OUT_TYPE); //Modify (Add C_SCREEN_ID_INSTALL_SLIP, C_STOCK_OUT_TYPE) by Jutarat A. on 21102013            

            return lstRptNewSaleSlipData;
        }

        public List<RPTNewSaleSlipDo> GetRptNewSaleSlipReport_TestData(string strSlipNo)
        {
            List<RPTNewSaleSlipDo> lstRptNewSaleSlipData = new List<RPTNewSaleSlipDo>();

            for (int i = 1; i <= 30; i++)
            {
                RPTNewSaleSlipDo doRpt = new RPTNewSaleSlipDo();

                doRpt.SlipIssueDate = DateTime.Now.ToString("dd-MMM-yyyy");
                doRpt.SlipNo = "00011100001";
                doRpt.ContractCode = "N00000015";
                doRpt.MaintenanceNo = "0001N20100001";
                doRpt.ExpectedInstrumentArrivalDate = DateTime.Now.ToString("dd-MMM-yyyy");
                doRpt.PreviousSlipNo = "-";
                doRpt.OfficeNameEN = "ดอนเมือง";
                doRpt.OtherSubContractor = 2;
                doRpt.SubContractorNameEN = "ผู้รับเหมาที่ 1";
                doRpt.SiteNameEN = "เอ็กเช้นจ์ ทาวเวอร์";
                doRpt.AddressFullEN = "338 ชั้น 33, ถนน สุขุมวิท แขวง คลองเตย เขต วัฒนา จังหวัด กรุงเทพ 10310";
                doRpt.CustFullNameEN = "บริษัท ซี เอส ไอ ไทยแลนด์";
                doRpt.AddressFullEN1 = "338 ชั้น 33, ถนน สุขุมวิท แขวง คลองเตย เขต วัฒนา จังหวัด กรุงเทพ 10310";
                doRpt.ProductNameEN = "โทโมฮอก สอง";
                doRpt.InstallationTypeName = "New";

                doRpt.InstrumentCode = "000000000" + i;
                doRpt.CurrentStockOutQty = 9999;
                doRpt.TotalStockOutQty = 9999;
                doRpt.ReturnQty = 9999;
                doRpt.ContractInstalledQty = 9999;

                lstRptNewSaleSlipData.Add(doRpt);
            }

            return lstRptNewSaleSlipData;
        }

        public List<RPTInstallRequestDo> GetRptInstallationRequestData(string strMaintenanceNo)
        {
            List<RPTInstallRequestDo> lstRPTInstallRequest = base.GetRptInstallationRequestData(strMaintenanceNo, ServiceType.C_SERVICE_TYPE_SALE, ServiceType.C_SERVICE_TYPE_RENTAL, ServiceType.C_SERVICE_TYPE_PROJECT, MiscType.C_SALE_INSTALL_TYPE, MiscType.C_RENTAL_INSTALL_TYPE,MiscType.C_NEW_BLD_MGMT_FLAG,MiscType.C_BUILDING_TYPE,MiscType.C_PHONE_LINE_OWNER_TYPE);
            return lstRPTInstallRequest;
        }

        public List<RPTInstallSpecCompleteDo> GetRptInstallSpecCompleteData(string strMaintenanceNo, string strSubcontractorCode)
        {
            List<RPTInstallSpecCompleteDo> lstRPTInstallSpec = base.GetRptInstallSpecCompleteData(strMaintenanceNo, strSubcontractorCode, ServiceType.C_SERVICE_TYPE_SALE, ServiceType.C_SERVICE_TYPE_RENTAL, ServiceType.C_SERVICE_TYPE_PROJECT);
            return lstRPTInstallSpec;
        }

        public List<RPTIECheckSheetDo> GetRptIECheckSheetData(string strMaintenanceNo, string strSubcontractorCode)
        {
            List<RPTIECheckSheetDo> lstRPTIECheckSheet = base.GetRptIECheckSheetData(strMaintenanceNo, strSubcontractorCode, ServiceType.C_SERVICE_TYPE_SALE, ServiceType.C_SERVICE_TYPE_RENTAL, ServiceType.C_SERVICE_TYPE_PROJECT);
            return lstRPTIECheckSheet;
        }

        public List<RPTInstallCompleteDo> GetRptInstallCompleteConfirmData(string strSlipNo)
        {
            List<RPTInstallCompleteDo> lstRPTInstallComplete = base.GetRptInstallCompleteConfirmData(strSlipNo, ServiceType.C_SERVICE_TYPE_SALE, ServiceType.C_SERVICE_TYPE_RENTAL, MiscType.C_SALE_INSTALL_TYPE, MiscType.C_RENTAL_INSTALL_TYPE);
            return lstRPTInstallComplete;
        }

        public List<RPTAcceptInspecDo> GetRptAcceptInspecNocticeData(string strMaintenanceNo, string strSubcontractorCode)
        {
            List<RPTAcceptInspecDo> lstRPTAcceptInspecDo = base.GetRptAcceptInspecNocticeData(strMaintenanceNo, strSubcontractorCode, ServiceType.C_SERVICE_TYPE_SALE, ServiceType.C_SERVICE_TYPE_RENTAL, ServiceType.C_SERVICE_TYPE_PROJECT);
            return lstRPTAcceptInspecDo;
        }

        public List<RPTDeliveryConfirmDo> GetRptDeliveryConfirmData(string strSlipNo)
        {
            List<RPTDeliveryConfirmDo> lstRPTDeliveryConfirmDo = base.GetRptDeliveryConfirmData(strSlipNo,ConfigName.C_CONFIG_INSTALL_WARRANTY_COND);
            return lstRPTDeliveryConfirmDo;
        }

        public List<RptISR110InstallCompleteConfirmDo> GetRptISR110InstallCompleteConfirmData(string strSlipNo)
        {
            List<RptISR110InstallCompleteConfirmDo> lstRptISR110InstallCompleteConfirmDo = base.GetRptISR110InstallCompleteConfirmData(strSlipNo, ServiceType.C_SERVICE_TYPE_SALE, ServiceType.C_SERVICE_TYPE_RENTAL, MiscType.C_SALE_INSTALL_TYPE, MiscType.C_RENTAL_INSTALL_TYPE);
            return lstRptISR110InstallCompleteConfirmDo;
        }

        public List<RptSignatureDo> GetRptSignatureData(string strCode, string strPosition)
        {
            List<RptSignatureDo> lstRptSignatureDo = base.GetRptSignature(strCode, strPosition);
            return lstRptSignatureDo;
        }
    }
}
