using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Master;

namespace SECOM_AJIS.Controllers
{
    //
    // GET: /SwtMAP020/
    // TODO Akat K. compare result all Case

    public class SwtMAP020Controller : SwtCommonController {
        public string index() {
            List<string> lst = new List<string>();
            lst.Add(Case1());
            lst.Add(Case2());
            lst.Add(Case3());
            lst.Add(Case4());
            lst.Add(Case5());
            lst.Add(Case6());
            lst.Add(Case7());
            lst.Add(Case8());
            lst.Add(Case9());
            lst.Add(Case10());
            lst.Add(Case11());
            lst.Add(Case12());
            lst.Add(Case13());
            lst.Add(Case14());
            lst.Add(Case15());

            string result = CommonUtil.TextList(lst.ToArray());
            result = result.Replace(", Case", "Case");

            return result;
        }

        ///<summary>
        ///Purpose:
        ///     Mandatory check 1 check mendatory field(Nocustomer group and site data)
        ///     
        ///Parameters:
        ///     Refer to : "SECOM-AJIS-STC.MAP020-ManageCustomerTarget" tab 'Test Data (Input DO)'
        ///         
        ///Expected:
        ///     MSG0007: "These field was required: CustNameEN, CustNameLC, CustTypeCode, BusinessTypeCode, RegionCode, AddressEN, RoadEN, SubDistrictEN, AddressLC, RoadLC, RoadLC, SubDistrictLC, DistrictCode, ProvinceCode"
        ///</summary>
        public string Case1() {
            ICustomerMasterHandler target = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
            doCustomerTarget param = new doCustomerTarget();
            param.doCustomer = new doCustomer();
            param.doCustomer.CustCode=null;
            param.doCustomer.CustStatus="0";
            param.doCustomer.ImportantFlag=null;
            param.doCustomer.CustNameEN=null;
            param.doCustomer.CustNameLC=null;
            param.doCustomer.CustFullNameEN=null;
            param.doCustomer.CustFullNameLC=null;
            param.doCustomer.RepPersonName=null;
            param.doCustomer.ContactPersonName=null;
            param.doCustomer.SECOMContactPerson=null;
            param.doCustomer.CustTypeCode=null;
            param.doCustomer.CompanyTypeCode=null;
            param.doCustomer.FinancialMarketTypeCode = null;
            param.doCustomer.BusinessTypeCode=null;
            param.doCustomer.PhoneNo="023946134";
            param.doCustomer.FaxNo="023456541";
            param.doCustomer.IDNo="1008002123455";
            param.doCustomer.DummyIDFlag=false;
            param.doCustomer.RegionCode=null;
            param.doCustomer.URL="http://www.csigroups.com/";
            param.doCustomer.Memo=null;
            param.doCustomer.AddressEN=null;
            param.doCustomer.AlleyEN=null;
            param.doCustomer.RoadEN=null;
            param.doCustomer.SubDistrictEN=null;
            param.doCustomer.AddressFullEN=null;
            param.doCustomer.AddressLC=null;
            param.doCustomer.AlleyLC=null;
            param.doCustomer.RoadLC=null;
            param.doCustomer.SubDistrictLC=null;
            param.doCustomer.AddressFullLC=null;
            param.doCustomer.DistrictCode=null;
            param.doCustomer.ProvinceCode=null;
            param.doCustomer.ZipCode="10210";
            param.doCustomer.DeleteFlag=false;
            //param.doCustomer.ValidateCustomerData=null;
            //param.doCustomer.SiteCustCode=null;
            param.dtCustomerGroup = null;
            param.doSite = null;

            string expected = "MSG0007";
            string actual = null;

            try {
                target.ManageCustomerTarget(param);
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 1, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Mandatory check 2 check mendatory field(No customer group and site data)
        ///     
        ///Parameters:
        ///     Refer to : "SECOM-AJIS-STC.MAP020-ManageCustomerTarget" tab 'Test Data (Input DO)'
        ///         
        ///Expected:
        ///     MSG0007: "These field was required: SiteNameEN, SiteNameLC, AddressEN, RoadEN, SubDistrictEN, AddressLC, AlleyLC, RoadLC, SubDistrictLC, DistrictCode, ProvinceCode"
        ///</summary>
        public string Case2() {
            ICustomerMasterHandler target = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
            doCustomerTarget param = new doCustomerTarget();
            param.doCustomer = new doCustomer();
            param.doSite = new doSite();

            param.doCustomer.CustCode=null;
            param.doCustomer.CustStatus="0";
            param.doCustomer.ImportantFlag=null;
            param.doCustomer.CustNameEN="C.S.I. Thailand";
            param.doCustomer.CustNameLC="ซีเอสไอ ไทยแลนด์";
            param.doCustomer.CustFullNameEN="C.S.I. Thailand CO., LTD.";
            param.doCustomer.CustFullNameLC="บริษัท ซีเอสไอ ไทยแลนด์ จำกัด";
            param.doCustomer.RepPersonName=null;
            param.doCustomer.ContactPersonName=null;
            param.doCustomer.SECOMContactPerson=null;
            param.doCustomer.CustTypeCode="0";
            param.doCustomer.CompanyTypeCode="03";
            param.doCustomer.FinancialMarketTypeCode="0";
            param.doCustomer.BusinessTypeCode="047";
            param.doCustomer.PhoneNo="023946134";
            param.doCustomer.FaxNo="023456541";
            param.doCustomer.IDNo="1008002123455";
            param.doCustomer.DummyIDFlag=false;
            param.doCustomer.RegionCode="TH";
            param.doCustomer.URL="http://www.csigroups.com/";
            param.doCustomer.Memo=null;
            param.doCustomer.AddressEN="SILOM COMPLEX TOWER, 28th FLOOR, NO.191";
            param.doCustomer.AlleyEN=null;
            param.doCustomer.RoadEN="SILOM";
            param.doCustomer.SubDistrictEN="SILOM";
            param.doCustomer.AddressFullEN="SILOM COMPLEX TOWER, 28th FLOOR, NO.191,SILOM,SILOM,BANGRAK,BANGKOK 10210";
            param.doCustomer.AddressLC="191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ";
            param.doCustomer.AlleyLC=null;
            param.doCustomer.RoadLC="สีลม";
            param.doCustomer.SubDistrictLC="สีลม";
            param.doCustomer.AddressFullLC="191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ ถ.สีลม แขวงสีลม เขตบางรัก จ.กรุงเทพมหานคร 10210";
            param.doCustomer.DistrictCode="00010";
            param.doCustomer.ProvinceCode="001";
            param.doCustomer.ZipCode="10210";
            param.doCustomer.DeleteFlag=false;
            //param.doCustomer.ValidateCustomerData=null
            //param.doCustomer.SiteCustCode=null
            param.dtCustomerGroup = null;
            param.doSite.SiteCode=null;
            param.doSite.CustCode=null;
            param.doSite.SiteNo=null;
            param.doSite.SiteNameEN=null;
            param.doSite.SiteNameLC=null;
            param.doSite.SECOMContactPerson=null;
            param.doSite.PersonInCharge=null;
            param.doSite.PhoneNo=null;
            param.doSite.BuildingUsageCode="019";
            param.doSite.AddressEN=null;
            param.doSite.AlleyEN=null;
            param.doSite.RoadEN=null;
            param.doSite.SubDistrictEN=null;
            param.doSite.AddressFullEN=null;
            param.doSite.AddressLC=null;
            param.doSite.AlleyLC=null;
            param.doSite.RoadLC=null;
            param.doSite.SubDistrictLC=null;
            param.doSite.AddressFullLC=null;
            param.doSite.DistrictCode="00010";
            param.doSite.ProvinceCode="001";
            param.doSite.ZipCode="10210";
            //param.doSite.ValidateSitetData=null;
						
            string expected = "MSG0007";
            string actual = null;

            try {
                target.ManageCustomerTarget(param);
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 2, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     New customer (No customer group data, No site data)
        ///     
        ///Parameters:
        ///     Refer to : "SECOM-AJIS-STC.MAP020-ManageCustomerTarget" tab 'Test Data (Input DO)'
        ///         
        ///Expected:
        ///     Refer to : "SECOM-AJIS-STC.MAP020-ManageCustomerTarget" tab ' expectation test case3
        ///     ///</summary>
        public string Case3() {
            ICustomerMasterHandler target = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
            doCustomerTarget param = new doCustomerTarget();
            param.doCustomer = new doCustomer();

            param.doCustomer.CustCode = null;
            param.doCustomer.CustStatus = "0";
            param.doCustomer.ImportantFlag = null;
            param.doCustomer.CustNameEN = "C.S.I. Thailand";
            param.doCustomer.CustNameLC = "ซีเอสไอ ไทยแลนด์";
            param.doCustomer.CustFullNameEN = "C.S.I. Thailand CO., LTD.";
            param.doCustomer.CustFullNameLC = "บริษัท ซีเอสไอ ไทยแลนด์ จำกัด";
            param.doCustomer.RepPersonName = null;
            param.doCustomer.ContactPersonName = null;
            param.doCustomer.SECOMContactPerson = null;
            param.doCustomer.CustTypeCode = "0";
            param.doCustomer.CompanyTypeCode = "03";
            param.doCustomer.FinancialMarketTypeCode = "0";
            param.doCustomer.BusinessTypeCode = "047";
            param.doCustomer.PhoneNo = "023946134";
            param.doCustomer.FaxNo = "023456541";
            param.doCustomer.IDNo = "1008002123455";
            param.doCustomer.DummyIDFlag = false;
            param.doCustomer.RegionCode = "TH";
            param.doCustomer.URL = "http://www.csigroups.com/";
            param.doCustomer.Memo = null;
            param.doCustomer.AddressEN = "SILOM COMPLEX TOWER, 28th FLOOR, NO.191";
            param.doCustomer.AlleyEN = null;
            param.doCustomer.RoadEN = "SILOM";
            param.doCustomer.SubDistrictEN = "SILOM";
            param.doCustomer.AddressFullEN = "SILOM COMPLEX TOWER, 28th FLOOR, NO.191,SILOM,SILOM,BANGRAK,BANGKOK 10210";
            param.doCustomer.AddressLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ";
            param.doCustomer.AlleyLC = null;
            param.doCustomer.RoadLC = "สีลม";
            param.doCustomer.SubDistrictLC = "สีลม";
            param.doCustomer.AddressFullLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ ถ.สีลม แขวงสีลม เขตบางรัก จ.กรุงเทพมหานคร 10210";
            param.doCustomer.DistrictCode = "00010";
            param.doCustomer.ProvinceCode = "001";
            param.doCustomer.ZipCode = "10210";
            param.doCustomer.DeleteFlag = false;
            //param.doCustomer.ValidateCustomerData=;
            //param.doCustomer.SiteCustCode=;
            param.dtCustomerGroup = null;
            param.doSite = null;

            string expected = null;
            string actual = null;

            try {
                target.ManageCustomerTarget(param);
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 3, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     New customer (Have customer group data, No site data)
        ///     
        ///Parameters:
        ///     Refer to : "SECOM-AJIS-STC.MAP020-ManageCustomerTarget" tab 'Test Data (Input DO)'
        ///     
        /// Precondition:
        ///     Delete data of expectation test case3 in DB
        ///         
        ///Expected:
        ///     Refer to : "SECOM-AJIS-STC.MAP020-ManageCustomerTarget" tab ' expectation test case4
        ///     ///</summary>
        public string Case4() {
            ICustomerMasterHandler target = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
            doCustomerTarget param = new doCustomerTarget();
            param.doCustomer = new doCustomer();
            param.dtCustomerGroup = new List<dtCustomerGroup>();

            param.doCustomer.CustCode=null;
            param.doCustomer.CustStatus="0";
            param.doCustomer.ImportantFlag=null;
            param.doCustomer.CustNameEN="C.S.I. Thailand";
            param.doCustomer.CustNameLC="ซีเอสไอ ไทยแลนด์";
            param.doCustomer.CustFullNameEN="C.S.I. Thailand CO., LTD.";
            param.doCustomer.CustFullNameLC="บริษัท ซีเอสไอ ไทยแลนด์ จำกัด";
            param.doCustomer.RepPersonName=null;
            param.doCustomer.ContactPersonName=null;
            param.doCustomer.SECOMContactPerson=null;
            param.doCustomer.CustTypeCode="0";
            param.doCustomer.CompanyTypeCode="03";
            param.doCustomer.FinancialMarketTypeCode="0";
            param.doCustomer.BusinessTypeCode="047";
            param.doCustomer.PhoneNo="023946134";
            param.doCustomer.FaxNo="023456541";
            param.doCustomer.IDNo="1008002123455";
            param.doCustomer.DummyIDFlag=false;
            param.doCustomer.RegionCode="TH";
            param.doCustomer.URL="http://www.csigroups.com/";
            param.doCustomer.Memo=null;
            param.doCustomer.AddressEN="SILOM COMPLEX TOWER, 28th FLOOR, NO.191";
            param.doCustomer.AlleyEN=null;
            param.doCustomer.RoadEN="SILOM";
            param.doCustomer.SubDistrictEN="SILOM";
            param.doCustomer.AddressFullEN="SILOM COMPLEX TOWER, 28th FLOOR, NO.191,SILOM,SILOM,BANGRAK,BANGKOK 10210";
            param.doCustomer.AddressLC="191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ";
            param.doCustomer.AlleyLC=null;
            param.doCustomer.RoadLC="สีลม";
            param.doCustomer.SubDistrictLC="สีลม";
            param.doCustomer.AddressFullLC="191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ ถ.สีลม แขวงสีลม เขตบางรัก จ.กรุงเทพมหานคร 10210";
            param.doCustomer.DistrictCode="00010";
            param.doCustomer.ProvinceCode="001";
            param.doCustomer.ZipCode="10210";
            param.doCustomer.DeleteFlag=false;
            //param.doCustomer.ValidateCustomerData=
            //param.doCustomer.SiteCustCode=
            dtCustomerGroup group = new dtCustomerGroup();
            group.CustCode=null;
            group.GroupCode="G0000024";
            param.dtCustomerGroup.Add(group);
            group = new dtCustomerGroup();
            group.CustCode = null;
            group.GroupCode = "G0000105";
            param.dtCustomerGroup.Add(group);
            param.doSite = null;

            string expected = null;
            string actual = null;

            try {
                target.ManageCustomerTarget(param);
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 4, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     New customer (Have customer group data, Have site data)
        ///     
        ///Parameters:
        ///     Refer to : "SECOM-AJIS-STC.MAP020-ManageCustomerTarget" tab 'Test Data (Input DO)'
        ///     
        /// Precondition:
        ///     Delete data of expectation test case4 in DB
        ///         
        ///Expected:
        ///     Refer to : "SECOM-AJIS-STC.MAP020-ManageCustomerTarget" tab ' expectation test case5
        ///     ///</summary>
        public string Case5() {
            ICustomerMasterHandler target = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
            doCustomerTarget param = new doCustomerTarget();
            param.doCustomer = new doCustomer();
            param.doSite = new doSite();
            param.dtCustomerGroup = new List<dtCustomerGroup>();

            param.doCustomer.CustCode=null;
            param.doCustomer.CustStatus="0";
            param.doCustomer.ImportantFlag=null;
            param.doCustomer.CustNameEN="C.S.I. Thailand";
            param.doCustomer.CustNameLC="ซีเอสไอ ไทยแลนด์";
            param.doCustomer.CustFullNameEN="C.S.I. Thailand CO., LTD.";
            param.doCustomer.CustFullNameLC="บริษัท ซีเอสไอ ไทยแลนด์ จำกัด";
            param.doCustomer.RepPersonName=null;
            param.doCustomer.ContactPersonName=null;
            param.doCustomer.SECOMContactPerson=null;
            param.doCustomer.CustTypeCode="0";
            param.doCustomer.CompanyTypeCode="03";
            param.doCustomer.FinancialMarketTypeCode="0";
            param.doCustomer.BusinessTypeCode="047";
            param.doCustomer.PhoneNo="023946134";
            param.doCustomer.FaxNo="023456541";
            param.doCustomer.IDNo="1008002123455";
            param.doCustomer.DummyIDFlag=false;
            param.doCustomer.RegionCode="TH";
            param.doCustomer.URL="http://www.csigroups.com/";
            param.doCustomer.Memo=null;
            param.doCustomer.AddressEN="SILOM COMPLEX TOWER, 28th FLOOR, NO.191";
            param.doCustomer.AlleyEN=null;
            param.doCustomer.RoadEN="SILOM";
            param.doCustomer.SubDistrictEN="SILOM";
            param.doCustomer.AddressFullEN="SILOM COMPLEX TOWER, 28th FLOOR, NO.191,SILOM,SILOM,BANGRAK,BANGKOK 10210";
            param.doCustomer.AddressLC="191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ";
            param.doCustomer.AlleyLC=null;
            param.doCustomer.RoadLC="สีลม";
            param.doCustomer.SubDistrictLC="สีลม";
            param.doCustomer.AddressFullLC="191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ ถ.สีลม แขวงสีลม เขตบางรัก จ.กรุงเทพมหานคร 10210";
            param.doCustomer.DistrictCode="00010";
            param.doCustomer.ProvinceCode="001";
            param.doCustomer.ZipCode="10210";
            param.doCustomer.DeleteFlag=false;
            //param.doCustomer.ValidateCustomerData=null;
            //param.doCustomer.SiteCustCode=null;
            dtCustomerGroup group = new dtCustomerGroup();	
            group.CustCode=null;
            group.GroupCode="G0000024";
            param.dtCustomerGroup.Add(group);
            group = new dtCustomerGroup();
            group.CustCode=null;
            group.GroupCode = "G0000105";
            param.dtCustomerGroup.Add(group);
            param.doSite.SiteCode=null;
            param.doSite.CustCode=null;
            param.doSite.SiteNo=null;
            param.doSite.SiteNameEN="C.S.I. INTERHOLDINGS CO., LTD.";
            param.doSite.SiteNameLC="บริษัท ซี.เอส.ไอ.อินเตอร์โฮลดิ้งส์ จำกัด";
            param.doSite.SECOMContactPerson=null;
            param.doSite.PersonInCharge=null;
            param.doSite.PhoneNo=null;
            param.doSite.BuildingUsageCode="019";
            param.doSite.AddressEN="SILOM COMPLEX TOWER,15th FL., NO.191";
            param.doSite.AlleyEN=null;
            param.doSite.RoadEN="SILOM";
            param.doSite.SubDistrictEN="SILOM";
            param.doSite.AddressFullEN="SILOM COMPLEX TOWER,15th FL., NO.191,SILOM,SILOM,BANGRAK,BANGKOK 10210";
            param.doSite.AddressLC="191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ";
            param.doSite.AlleyLC=null;
            param.doSite.RoadLC="สีลม";
            param.doSite.SubDistrictLC="สีลม";
            param.doSite.AddressFullLC="191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ ถ.สีลม แขวงสีลม เขตบางรัก จ.กรุงเทพมหานคร 10210";
            param.doSite.DistrictCode="00010";
            param.doSite.ProvinceCode="001";
            param.doSite.ZipCode="10210";
            //param.doSite.ValidateSitetData=null;

            string expected = null;
            string actual = null;

            try {
                target.ManageCustomerTarget(param);
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 5, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     New customer (No customer group data, Have site data)
        ///     
        ///Parameters:
        ///     Refer to : "SECOM-AJIS-STC.MAP020-ManageCustomerTarget" tab 'Test Data (Input DO)'
        ///     
        /// Precondition:
        ///     Delete data of expectation test case5 in DB
        ///         
        ///Expected:
        ///     Refer to : "SECOM-AJIS-STC.MAP020-ManageCustomerTarget" tab ' expectation test case6
        ///     ///</summary>
        public string Case6() {
            ICustomerMasterHandler target = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
            doCustomerTarget param = new doCustomerTarget();
            param.doCustomer = new doCustomer();
            param.doSite = new doSite();

            param.doCustomer.CustCode = null;
            param.doCustomer.CustStatus = "0";
            param.doCustomer.ImportantFlag = null;
            param.doCustomer.CustNameEN = "C.S.I. Thailand";
            param.doCustomer.CustNameLC = "ซีเอสไอ ไทยแลนด์";
            param.doCustomer.CustFullNameEN = "C.S.I. Thailand CO., LTD.";
            param.doCustomer.CustFullNameLC = "บริษัท ซีเอสไอ ไทยแลนด์ จำกัด";
            param.doCustomer.RepPersonName = null;
            param.doCustomer.ContactPersonName = null;
            param.doCustomer.SECOMContactPerson = null;
            param.doCustomer.CustTypeCode = "0";
            param.doCustomer.CompanyTypeCode = "03";
            param.doCustomer.FinancialMarketTypeCode = "0";
            param.doCustomer.BusinessTypeCode = "047";
            param.doCustomer.PhoneNo = "023946134";
            param.doCustomer.FaxNo = "023456541";
            param.doCustomer.IDNo = "1008002123455";
            param.doCustomer.DummyIDFlag = false;
            param.doCustomer.RegionCode = "TH";
            param.doCustomer.URL = "http://www.csigroups.com/";
            param.doCustomer.Memo = null;
            param.doCustomer.AddressEN = "SILOM COMPLEX TOWER, 28th FLOOR, NO.191";
            param.doCustomer.AlleyEN = null;
            param.doCustomer.RoadEN = "SILOM";
            param.doCustomer.SubDistrictEN = "SILOM";
            param.doCustomer.AddressFullEN = "SILOM COMPLEX TOWER, 28th FLOOR, NO.191,SILOM,SILOM,BANGRAK,BANGKOK 10210";
            param.doCustomer.AddressLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ";
            param.doCustomer.AlleyLC = null;
            param.doCustomer.RoadLC = "สีลม";
            param.doCustomer.SubDistrictLC = "สีลม";
            param.doCustomer.AddressFullLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ ถ.สีลม แขวงสีลม เขตบางรัก จ.กรุงเทพมหานคร 10210";
            param.doCustomer.DistrictCode = "00010";
            param.doCustomer.ProvinceCode = "001";
            param.doCustomer.ZipCode = "10210";
            param.doCustomer.DeleteFlag = false;
            //param.doCustomer.ValidateCustomerData=null;
            //param.doCustomer.SiteCustCode=null;
            param.doSite.SiteCode = null;
            param.doSite.CustCode = null;
            param.doSite.SiteNo = null;
            param.doSite.SiteNameEN = "C.S.I. INTERHOLDINGS CO., LTD.";
            param.doSite.SiteNameLC = "บริษัท ซี.เอส.ไอ.อินเตอร์โฮลดิ้งส์ จำกัด";
            param.doSite.SECOMContactPerson = null;
            param.doSite.PersonInCharge = null;
            param.doSite.PhoneNo = null;
            param.doSite.BuildingUsageCode = "019";
            param.doSite.AddressEN = "SILOM COMPLEX TOWER,15th FL., NO.191";
            param.doSite.AlleyEN = null;
            param.doSite.RoadEN = "SILOM";
            param.doSite.SubDistrictEN = "SILOM";
            param.doSite.AddressFullEN = "SILOM COMPLEX TOWER,15th FL., NO.191,SILOM,SILOM,BANGRAK,BANGKOK 10210";
            param.doSite.AddressLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ";
            param.doSite.AlleyLC = null;
            param.doSite.RoadLC = "สีลม";
            param.doSite.SubDistrictLC = "สีลม";
            param.doSite.AddressFullLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ ถ.สีลม แขวงสีลม เขตบางรัก จ.กรุงเทพมหานคร 10210";
            param.doSite.DistrictCode = "00010";
            param.doSite.ProvinceCode = "001";
            param.doSite.ZipCode = "10210";
            //param.doSite.ValidateSitetData=null;

            string expected = null;
            string actual = null;

            try {
                target.ManageCustomerTarget(param);
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 6, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     New customer (Dummy ID)
        ///     
        ///Parameters:
        ///     Refer to : "SECOM-AJIS-STC.MAP020-ManageCustomerTarget" tab 'Test Data (Input DO)'
        ///     
        /// Precondition:
        ///     Delete data of expectation test case6 in DB
        ///         
        ///Expected:
        ///     Refer to : "SECOM-AJIS-STC.MAP020-ManageCustomerTarget" tab expectation test case7
        ///
        ///</summary>
        public string Case7() {
            ICustomerMasterHandler target = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
            doCustomerTarget param = new doCustomerTarget();
            param.doCustomer = new doCustomer();
            param.doSite = new doSite();
            param.dtCustomerGroup = new List<dtCustomerGroup>();

            param.doCustomer.CustCode = null;
            param.doCustomer.CustStatus = "0";
            param.doCustomer.ImportantFlag = null;
            param.doCustomer.CustNameEN = "C.S.I. Thailand";
            param.doCustomer.CustNameLC = "ซีเอสไอ ไทยแลนด์";
            param.doCustomer.CustFullNameEN = "C.S.I. Thailand CO., LTD.";
            param.doCustomer.CustFullNameLC = "บริษัท ซีเอสไอ ไทยแลนด์ จำกัด";
            param.doCustomer.RepPersonName = null;
            param.doCustomer.ContactPersonName = null;
            param.doCustomer.SECOMContactPerson = null;
            param.doCustomer.CustTypeCode = "0";
            param.doCustomer.CompanyTypeCode = "03";
            param.doCustomer.FinancialMarketTypeCode = "0";
            param.doCustomer.BusinessTypeCode = "047";
            param.doCustomer.PhoneNo = "023946134";
            param.doCustomer.FaxNo = "023456541";
            param.doCustomer.IDNo = null;
            param.doCustomer.DummyIDFlag = true;
            param.doCustomer.RegionCode = "TH";
            param.doCustomer.URL = "http://www.csigroups.com/";
            param.doCustomer.Memo = null;
            param.doCustomer.AddressEN = "SILOM COMPLEX TOWER, 28th FLOOR, NO.191";
            param.doCustomer.AlleyEN = null;
            param.doCustomer.RoadEN = "SILOM";
            param.doCustomer.SubDistrictEN = "SILOM";
            param.doCustomer.AddressFullEN = "SILOM COMPLEX TOWER, 28th FLOOR, NO.191,SILOM,SILOM,BANGRAK,BANGKOK 10210";
            param.doCustomer.AddressLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ";
            param.doCustomer.AlleyLC = null;
            param.doCustomer.RoadLC = "สีลม";
            param.doCustomer.SubDistrictLC = "สีลม";
            param.doCustomer.AddressFullLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ ถ.สีลม แขวงสีลม เขตบางรัก จ.กรุงเทพมหานคร 10210";
            param.doCustomer.DistrictCode = "00010";
            param.doCustomer.ProvinceCode = "001";
            param.doCustomer.ZipCode = "10210";
            param.doCustomer.DeleteFlag = false;
            //param.doCustomer.ValidateCustomerData=null;
            //param.doCustomer.SiteCustCode=null;

            dtCustomerGroup group = new dtCustomerGroup();
            group.CustCode = null;
            group.GroupCode = "G0000024";
            param.dtCustomerGroup.Add(group);
            group = new dtCustomerGroup();
            group.CustCode = null;
            group.GroupCode = "G0000105";
            param.dtCustomerGroup.Add(group);

            param.doSite.SiteCode = null;
            param.doSite.CustCode = null;
            param.doSite.SiteNo = null;
            param.doSite.SiteNameEN = "C.S.I. INTERHOLDINGS CO., LTD.";
            param.doSite.SiteNameLC = "บริษัท ซี.เอส.ไอ.อินเตอร์โฮลดิ้งส์ จำกัด";
            param.doSite.SECOMContactPerson = null;
            param.doSite.PersonInCharge = null;
            param.doSite.PhoneNo = null;
            param.doSite.BuildingUsageCode = "019";
            param.doSite.AddressEN = "SILOM COMPLEX TOWER,15th FL., NO.191";
            param.doSite.AlleyEN = null;
            param.doSite.RoadEN = "SILOM";
            param.doSite.SubDistrictEN = "SILOM";
            param.doSite.AddressFullEN = "SILOM COMPLEX TOWER,15th FL., NO.191,SILOM,SILOM,BANGRAK,BANGKOK 10210";
            param.doSite.AddressLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ";
            param.doSite.AlleyLC = null;
            param.doSite.RoadLC = "สีลม";
            param.doSite.SubDistrictLC = "สีลม";
            param.doSite.AddressFullLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ ถ.สีลม แขวงสีลม เขตบางรัก จ.กรุงเทพมหานคร 10210";
            param.doSite.DistrictCode = "00010";
            param.doSite.ProvinceCode = "001";
            param.doSite.ZipCode = "10210";
            //param.doSite.ValidateSitetData=null;

            // Akat K. : to make sure that not effect to MSG0019
            CommonUtil.dsTransData.dtOperationData.ProcessDateTime = new DateTime(2011, 10, 31, 16, 0, 0);
            param.doCustomer.UpdateDate = new DateTime(2011, 10, 31, 16, 0, 0);
            param.doSite.UpdateDate = new DateTime(2011, 10, 31, 16, 0, 0);

            string expected = null;
            string actual = null;

            try {
                target.ManageCustomerTarget(param);
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 7, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Edit customer (All data not change)
        ///     
        ///Parameters:
        ///     Refer to : "SECOM-AJIS-STC.MAP020-ManageCustomerTarget" tab 'Test Data (Input DO)'
        ///         
        ///Expected:
        ///     No change data in DB
        ///
        ///</summary>
        public string Case8() {
            ICustomerMasterHandler target = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
            doCustomerTarget param = new doCustomerTarget();
            param.doCustomer = new doCustomer();
            param.doSite = new doSite();
            param.dtCustomerGroup = new List<dtCustomerGroup>();

            param.doCustomer.CustCode = "C0000000192";
            param.doCustomer.CustStatus = "0";
            param.doCustomer.ImportantFlag = null;
            param.doCustomer.CustNameEN = "C.S.I. Thailand";
            param.doCustomer.CustNameLC = "ซีเอสไอ ไทยแลนด์";
            param.doCustomer.CustFullNameEN = "C.S.I. Thailand CO., LTD.";
            param.doCustomer.CustFullNameLC = "บริษัท ซีเอสไอ ไทยแลนด์ จำกัด";
            param.doCustomer.RepPersonName = null;
            param.doCustomer.ContactPersonName = null;
            param.doCustomer.SECOMContactPerson = null;
            param.doCustomer.CustTypeCode = "0";
            param.doCustomer.CompanyTypeCode = "03";
            param.doCustomer.FinancialMarketTypeCode = "0";
            param.doCustomer.BusinessTypeCode = "047";
            param.doCustomer.PhoneNo = "023946134";
            param.doCustomer.FaxNo = "023456541";
            param.doCustomer.IDNo = "DMID000000005";
            param.doCustomer.DummyIDFlag = true;
            param.doCustomer.RegionCode = "TH";
            param.doCustomer.URL = "http://www.csigroups.com/";
            param.doCustomer.Memo = null;
            param.doCustomer.AddressEN = "SILOM COMPLEX TOWER, 28th FLOOR, NO.191";
            param.doCustomer.AlleyEN = null;
            param.doCustomer.RoadEN = "SILOM";
            param.doCustomer.SubDistrictEN = "SILOM";
            param.doCustomer.AddressFullEN = "SILOM COMPLEX TOWER, 28th FLOOR, NO.191,SILOM,SILOM,BANGRAK,BANGKOK 10210";
            param.doCustomer.AddressLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ";
            param.doCustomer.AlleyLC = null;
            param.doCustomer.RoadLC = "สีลม";
            param.doCustomer.SubDistrictLC = "สีลม";
            param.doCustomer.AddressFullLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ ถ.สีลม แขวงสีลม เขตบางรัก จ.กรุงเทพมหานคร 10210";
            param.doCustomer.DistrictCode = "00010";
            param.doCustomer.ProvinceCode = "001";
            param.doCustomer.ZipCode = "10210";
            param.doCustomer.DeleteFlag = false;
            //param.doCustomer.ValidateCustomerData=null;
            //param.doCustomer.SiteCustCode=null;
            param.doCustomer.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            param.doCustomer.UpdateDate = new DateTime(1, 1, 1, 1, 1, 1);

            dtCustomerGroup group = new dtCustomerGroup();
            group.CustCode = null;
            group.GroupCode = "G0000024";
            param.dtCustomerGroup.Add(group);
            group = new dtCustomerGroup();
            group.CustCode = null;
            group.GroupCode = "G0000105";
            param.dtCustomerGroup.Add(group);

            param.doSite.SiteCode = "S0000000192-0001";
            param.doSite.CustCode = null;
            param.doSite.SiteNo = "0001";
            param.doSite.SiteNameEN = "C.S.I. INTERHOLDINGS CO., LTD.";
            param.doSite.SiteNameLC = "บริษัท ซี.เอส.ไอ.อินเตอร์โฮลดิ้งส์ จำกัด";
            param.doSite.SECOMContactPerson = null;
            param.doSite.PersonInCharge = null;
            param.doSite.PhoneNo = null;
            param.doSite.BuildingUsageCode = "019";
            param.doSite.AddressEN = "SILOM COMPLEX TOWER,15th FL., NO.191";
            param.doSite.AlleyEN = null;
            param.doSite.RoadEN = "SILOM";
            param.doSite.SubDistrictEN = "SILOM";
            param.doSite.AddressFullEN = "SILOM COMPLEX TOWER,15th FL., NO.191,SILOM,SILOM,BANGRAK,BANGKOK 10210";
            param.doSite.AddressLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ";
            param.doSite.AlleyLC = null;
            param.doSite.RoadLC = "สีลม";
            param.doSite.SubDistrictLC = "สีลม";
            param.doSite.AddressFullLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ ถ.สีลม แขวงสีลม เขตบางรัก จ.กรุงเทพมหานคร 10210";
            param.doSite.DistrictCode = "00010";
            param.doSite.ProvinceCode = "001";
            param.doSite.ZipCode = "10210";
            //param.doSite.ValidateSitetData=null;

            // Akat K. : to make sure that not effect to MSG0019
            CommonUtil.dsTransData.dtOperationData.ProcessDateTime = new DateTime(2011, 10, 31, 16, 0, 0);
            param.doCustomer.UpdateDate = new DateTime(2011, 10, 31, 16, 0, 0);
            param.doSite.UpdateDate = new DateTime(2011, 10, 31, 16, 0, 0);

            string expected = null;
            string actual = null;

            try {
                target.ManageCustomerTarget(param);
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 8, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Edit customer (Change other info. except ID and use in other contract)
        ///     
        ///Parameters:
        ///     Refer to : "SECOM-AJIS-STC.MAP020-ManageCustomerTarget" tab 'Test Data (Input DO)'
        ///         
        ///Expected:
        ///     MSG1041: "Customer information is used in other contract."
        ///
        ///</summary>
        public string Case9()
        {
            ICustomerMasterHandler target = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
            doCustomerTarget param = new doCustomerTarget();
            param.doCustomer = new doCustomer();
            param.doSite = new doSite();
            param.dtCustomerGroup = new List<dtCustomerGroup>();

            param.doCustomer.CustCode = "C0000000192";
            param.doCustomer.CustStatus = "0";
            param.doCustomer.ImportantFlag = null;
            param.doCustomer.CustNameEN = "C.S.I. Thailand";
            param.doCustomer.CustNameLC = "ซีเอสไอ ไทยแลนด์";
            param.doCustomer.CustFullNameEN = "C.S.I. Thailand CO., LTD.";
            param.doCustomer.CustFullNameLC = "บริษัท ซีเอสไอ ไทยแลนด์ จำกัด";
            param.doCustomer.RepPersonName = "Patcharee T.";
            param.doCustomer.ContactPersonName = "Patcharee T.";
            param.doCustomer.SECOMContactPerson = "Pornsri W.";
            param.doCustomer.CustTypeCode = "0";
            param.doCustomer.CompanyTypeCode = "03";
            param.doCustomer.FinancialMarketTypeCode = "0";
            param.doCustomer.BusinessTypeCode = "047";
            param.doCustomer.PhoneNo = "023946134";
            param.doCustomer.FaxNo = "023456541";
            param.doCustomer.IDNo = "DMID000000005";
            param.doCustomer.DummyIDFlag = true;
            param.doCustomer.RegionCode = "TH";
            param.doCustomer.URL = "http://www.csigroups.com/";
            param.doCustomer.Memo = null;
            param.doCustomer.AddressEN = "SILOM COMPLEX TOWER, 28th FLOOR, NO.191";
            param.doCustomer.AlleyEN = null;
            param.doCustomer.RoadEN = "SILOM";
            param.doCustomer.SubDistrictEN = "SILOM";
            param.doCustomer.AddressFullEN = "SILOM COMPLEX TOWER, 28th FLOOR, NO.191,SILOM,SILOM,BANGRAK,BANGKOK 10210";
            param.doCustomer.AddressLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ";
            param.doCustomer.AlleyLC = null;
            param.doCustomer.RoadLC = "สีลม";
            param.doCustomer.SubDistrictLC = "สีลม";
            param.doCustomer.AddressFullLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ ถ.สีลม แขวงสีลม เขตบางรัก จ.กรุงเทพมหานคร 10210";
            param.doCustomer.DistrictCode = "00010";
            param.doCustomer.ProvinceCode = "001";
            param.doCustomer.ZipCode = "10210";
            param.doCustomer.DeleteFlag = false;
            //param.doCustomer.ValidateCustomerData=null;
            //param.doCustomer.SiteCustCode=null;

            dtCustomerGroup group = new dtCustomerGroup();
            group.CustCode = null;
            group.GroupCode = "G0000024";
            param.dtCustomerGroup.Add(group);
            group = new dtCustomerGroup();
            group.CustCode = null;
            group.GroupCode = "G0000105";
            param.dtCustomerGroup.Add(group);

            param.doSite.SiteCode = "S0000000192-0001";
            param.doSite.CustCode = null;
            param.doSite.SiteNo = "0001";
            param.doSite.SiteNameEN = "C.S.I. INTERHOLDINGS CO., LTD.";
            param.doSite.SiteNameLC = "บริษัท ซี.เอส.ไอ.อินเตอร์โฮลดิ้งส์ จำกัด";
            param.doSite.SECOMContactPerson = "Patcharee T.";
            param.doSite.PersonInCharge = "Patcharee T.";
            param.doSite.PhoneNo = "023946134";
            param.doSite.BuildingUsageCode = "019";
            param.doSite.AddressEN = "SILOM COMPLEX TOWER,15th FL., NO.191";
            param.doSite.AlleyEN = null;
            param.doSite.RoadEN = "SILOM";
            param.doSite.SubDistrictEN = "SILOM";
            param.doSite.AddressFullEN = "SILOM COMPLEX TOWER,15th FL., NO.191,SILOM,SILOM,BANGRAK,BANGKOK 10210";
            param.doSite.AddressLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ";
            param.doSite.AlleyLC = null;
            param.doSite.RoadLC = "สีลม";
            param.doSite.SubDistrictLC = "สีลม";
            param.doSite.AddressFullLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ ถ.สีลม แขวงสีลม เขตบางรัก จ.กรุงเทพมหานคร 10210";
            param.doSite.DistrictCode = "00010";
            param.doSite.ProvinceCode = "001";
            param.doSite.ZipCode = "10210";
            //param.doSite.ValidateSitetData=null;

            // Akat K. : to make sure that not effect to MSG0019
            CommonUtil.dsTransData.dtOperationData.ProcessDateTime = new DateTime(2011, 10, 31, 16, 0, 0);
            param.doCustomer.UpdateDate = new DateTime(2011, 10, 31, 16, 0, 0);
            param.doSite.UpdateDate = new DateTime(2011, 10, 31, 16, 0, 0);

            string expected = "MSG1041";
            string actual = null;

            try {
                target.ManageCustomerTarget(param);
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 9, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Edit customer (Change other info. except ID and no use in other contract)
        ///     
        ///Parameters:
        ///     Refer to : "SECOM-AJIS-STC.MAP020-ManageCustomerTarget" tab 'Test Data (Input DO)'
        ///         
        ///Precondition:
        ///     Delete row of contract code = 'N0000000001' in tbt_RentalContractBasic
        /// 
        ///Expected:
        ///     Refer to : "SECOM-AJIS-STC.MAP020-ManageCustomerTarget" tab expectation test case10
        ///
        ///</summary>
        public string Case10() {
            ICustomerMasterHandler target = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
            doCustomerTarget param = new doCustomerTarget();
            param.doCustomer = new doCustomer();
            param.doSite = new doSite();
            param.dtCustomerGroup = new List<dtCustomerGroup>();

            param.doCustomer.CustCode = "C0000000192";
            param.doCustomer.CustStatus = "0";
            param.doCustomer.ImportantFlag = null;
            param.doCustomer.CustNameEN = "C.S.I. Thailand";
            param.doCustomer.CustNameLC = "ซีเอสไอ ไทยแลนด์";
            param.doCustomer.CustFullNameEN = "C.S.I. Thailand CO., LTD.";
            param.doCustomer.CustFullNameLC = "บริษัท ซีเอสไอ ไทยแลนด์ จำกัด";
            param.doCustomer.RepPersonName = "Patcharee T.";
            param.doCustomer.ContactPersonName = "Patcharee T.";
            param.doCustomer.SECOMContactPerson = "Pornsri W.";
            param.doCustomer.CustTypeCode = "0";
            param.doCustomer.CompanyTypeCode = "03";
            param.doCustomer.FinancialMarketTypeCode = "0";
            param.doCustomer.BusinessTypeCode = "047";
            param.doCustomer.PhoneNo = "023946134";
            param.doCustomer.FaxNo = "023456541";
            param.doCustomer.IDNo = "DMID000000005";
            param.doCustomer.DummyIDFlag = true;
            param.doCustomer.RegionCode = "TH";
            param.doCustomer.URL = "http://www.csigroups.com/";
            param.doCustomer.Memo = null;
            param.doCustomer.AddressEN = "SILOM COMPLEX TOWER, 28th FLOOR, NO.191";
            param.doCustomer.AlleyEN = null;
            param.doCustomer.RoadEN = "SILOM";
            param.doCustomer.SubDistrictEN = "SILOM";
            param.doCustomer.AddressFullEN = "SILOM COMPLEX TOWER, 28th FLOOR, NO.191,SILOM,SILOM,BANGRAK,BANGKOK 10210";
            param.doCustomer.AddressLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ";
            param.doCustomer.AlleyLC = null;
            param.doCustomer.RoadLC = "สีลม";
            param.doCustomer.SubDistrictLC = "สีลม";
            param.doCustomer.AddressFullLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ ถ.สีลม แขวงสีลม เขตบางรัก จ.กรุงเทพมหานคร 10210";
            param.doCustomer.DistrictCode = "00010";
            param.doCustomer.ProvinceCode = "001";
            param.doCustomer.ZipCode = "10210";
            param.doCustomer.DeleteFlag = false;
            //param.doCustomer.ValidateCustomerData=null;
            //param.doCustomer.SiteCustCode=null;

            dtCustomerGroup group = new dtCustomerGroup();
            group.CustCode = null;
            group.GroupCode = "G0000024";
            param.dtCustomerGroup.Add(group);
            group = new dtCustomerGroup();
            group.CustCode = null;
            group.GroupCode = "G0000105";
            param.dtCustomerGroup.Add(group);

            param.doSite.SiteCode = "S0000000192-0001";
            param.doSite.CustCode = null;
            param.doSite.SiteNo = "0001";
            param.doSite.SiteNameEN = "C.S.I. INTERHOLDINGS CO., LTD.";
            param.doSite.SiteNameLC = "บริษัท ซี.เอส.ไอ.อินเตอร์โฮลดิ้งส์ จำกัด";
            param.doSite.SECOMContactPerson = "Patcharee T.";
            param.doSite.PersonInCharge = "Patcharee T.";
            param.doSite.PhoneNo = "023946134";
            param.doSite.BuildingUsageCode = "019";
            param.doSite.AddressEN = "SILOM COMPLEX TOWER,15th FL., NO.191";
            param.doSite.AlleyEN = null;
            param.doSite.RoadEN = "SILOM";
            param.doSite.SubDistrictEN = "SILOM";
            param.doSite.AddressFullEN = "SILOM COMPLEX TOWER,15th FL., NO.191,SILOM,SILOM,BANGRAK,BANGKOK 10210";
            param.doSite.AddressLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ";
            param.doSite.AlleyLC = null;
            param.doSite.RoadLC = "สีลม";
            param.doSite.SubDistrictLC = "สีลม";
            param.doSite.AddressFullLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ ถ.สีลม แขวงสีลม เขตบางรัก จ.กรุงเทพมหานคร 10210";
            param.doSite.DistrictCode = "00010";
            param.doSite.ProvinceCode = "001";
            param.doSite.ZipCode = "10210";
            //param.doSite.ValidateSitetData=null;

            // Akat K. : to make sure that not effect to MSG0019
            CommonUtil.dsTransData.dtOperationData.ProcessDateTime = new DateTime(2011, 10, 31, 16, 0, 0);
            param.doCustomer.UpdateDate = new DateTime(2011, 10, 31, 16, 0, 0);
            param.doSite.UpdateDate = new DateTime(2011, 10, 31, 16, 0, 0);

            string expected = null;
            string actual = null;

            try {
                target.ManageCustomerTarget(param);
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 10, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Edit customer (Change ID from dummy to actual)
        ///     
        ///Parameters:
        ///     Refer to : "SECOM-AJIS-STC.MAP020-ManageCustomerTarget" tab 'Test Data (Input DO)'
        ///         
        ///Expected:
        ///     Refer to : "SECOM-AJIS-STC.MAP020-ManageCustomerTarget" tab expectation test case11
        ///
        ///</summary>
        public string Case11() {
            ICustomerMasterHandler target = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
            doCustomerTarget param = new doCustomerTarget();
            param.doCustomer = new doCustomer();
            param.doSite = new doSite();
            param.dtCustomerGroup = new List<dtCustomerGroup>();

            param.doCustomer.CustCode = "C0000000192";
            param.doCustomer.CustStatus = "0";
            param.doCustomer.ImportantFlag = null;
            param.doCustomer.CustNameEN = "C.S.I. Thailand";
            param.doCustomer.CustNameLC = "ซีเอสไอ ไทยแลนด์";
            param.doCustomer.CustFullNameEN = "C.S.I. Thailand CO., LTD.";
            param.doCustomer.CustFullNameLC = "บริษัท ซีเอสไอ ไทยแลนด์ จำกัด";
            param.doCustomer.RepPersonName = "Patcharee T.";
            param.doCustomer.ContactPersonName = "Patcharee T.";
            param.doCustomer.SECOMContactPerson = "Pornsri W.";
            param.doCustomer.CustTypeCode = "0";
            param.doCustomer.CompanyTypeCode = "03";
            param.doCustomer.FinancialMarketTypeCode = "0";
            param.doCustomer.BusinessTypeCode = "047";
            param.doCustomer.PhoneNo = "023946134";
            param.doCustomer.FaxNo = "023456541";
            param.doCustomer.IDNo = "9876123476549";
            param.doCustomer.DummyIDFlag = false;
            param.doCustomer.RegionCode = "TH";
            param.doCustomer.URL = "http://www.csigroups.com/";
            param.doCustomer.Memo = null;
            param.doCustomer.AddressEN = "SILOM COMPLEX TOWER, 28th FLOOR, NO.191";
            param.doCustomer.AlleyEN = null;
            param.doCustomer.RoadEN = "SILOM";
            param.doCustomer.SubDistrictEN = "SILOM";
            param.doCustomer.AddressFullEN = "SILOM COMPLEX TOWER, 28th FLOOR, NO.191,SILOM,SILOM,BANGRAK,BANGKOK 10210";
            param.doCustomer.AddressLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ";
            param.doCustomer.AlleyLC = null;
            param.doCustomer.RoadLC = "สีลม";
            param.doCustomer.SubDistrictLC = "สีลม";
            param.doCustomer.AddressFullLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ ถ.สีลม แขวงสีลม เขตบางรัก จ.กรุงเทพมหานคร 10210";
            param.doCustomer.DistrictCode = "00010";
            param.doCustomer.ProvinceCode = "001";
            param.doCustomer.ZipCode = "10210";
            param.doCustomer.DeleteFlag = false;
            //param.doCustomer.ValidateCustomerData=null;
            //param.doCustomer.SiteCustCode=null;

            dtCustomerGroup group = new dtCustomerGroup();
            group.CustCode = null;
            group.GroupCode = "G0000024";
            param.dtCustomerGroup.Add(group);
            group = new dtCustomerGroup();
            group.CustCode = null;
            group.GroupCode = "G0000105";
            param.dtCustomerGroup.Add(group);

            param.doSite.SiteCode = "S0000000192-0001";
            param.doSite.CustCode = null;
            param.doSite.SiteNo = "0001";
            param.doSite.SiteNameEN = "C.S.I. INTERHOLDINGS CO., LTD.";
            param.doSite.SiteNameLC = "บริษัท ซี.เอส.ไอ.อินเตอร์โฮลดิ้งส์ จำกัด";
            param.doSite.SECOMContactPerson = "Patcharee T.";
            param.doSite.PersonInCharge = "Patcharee T.";
            param.doSite.PhoneNo = "023946134";
            param.doSite.BuildingUsageCode = "019";
            param.doSite.AddressEN = "SILOM COMPLEX TOWER,15th FL., NO.191";
            param.doSite.AlleyEN = null;
            param.doSite.RoadEN = "SILOM";
            param.doSite.SubDistrictEN = "SILOM";
            param.doSite.AddressFullEN = "SILOM COMPLEX TOWER,15th FL., NO.191,SILOM,SILOM,BANGRAK,BANGKOK 10210";
            param.doSite.AddressLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ";
            param.doSite.AlleyLC = null;
            param.doSite.RoadLC = "สีลม";
            param.doSite.SubDistrictLC = "สีลม";
            param.doSite.AddressFullLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ ถ.สีลม แขวงสีลม เขตบางรัก จ.กรุงเทพมหานคร 10210";
            param.doSite.DistrictCode = "00010";
            param.doSite.ProvinceCode = "001";
            param.doSite.ZipCode = "10210";
            //param.doSite.ValidateSitetData=null;

            // Akat K. : to make sure that not effect to MSG0019
            CommonUtil.dsTransData.dtOperationData.ProcessDateTime = new DateTime(2011, 10, 31, 16, 0, 0);
            param.doCustomer.UpdateDate = new DateTime(2011, 10, 31, 16, 0, 0);
            param.doSite.UpdateDate = new DateTime(2011, 10, 31, 16, 0, 0);

            string expected = null;
            string actual = null;

            try {
                target.ManageCustomerTarget(param);
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 11, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Edit customer (Change ID)
        ///     
        ///Parameters:
        ///     Refer to : "SECOM-AJIS-STC.MAP020-ManageCustomerTarget" tab 'Test Data (Input DO)'
        ///         
        ///Expected:
        ///     Refer to : "SECOM-AJIS-STC.MAP020-ManageCustomerTarget" tab expectation test case12
        ///
        ///</summary>
        public string Case12() {
            ICustomerMasterHandler target = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
            doCustomerTarget param = new doCustomerTarget();
            param.doCustomer = new doCustomer();
            param.doSite = new doSite();
            param.dtCustomerGroup = new List<dtCustomerGroup>();

            param.doCustomer.CustCode = "C0000000192";
            param.doCustomer.CustStatus = "0";
            param.doCustomer.ImportantFlag = null;
            param.doCustomer.CustNameEN = "C.S.I. Thailand";
            param.doCustomer.CustNameLC = "ซีเอสไอ ไทยแลนด์";
            param.doCustomer.CustFullNameEN = "C.S.I. Thailand CO., LTD.";
            param.doCustomer.CustFullNameLC = "บริษัท ซีเอสไอ ไทยแลนด์ จำกัด";
            param.doCustomer.RepPersonName = "Patcharee T.";
            param.doCustomer.ContactPersonName = "Patcharee T.";
            param.doCustomer.SECOMContactPerson = "Pornsri W.";
            param.doCustomer.CustTypeCode = "0";
            param.doCustomer.CompanyTypeCode = "03";
            param.doCustomer.FinancialMarketTypeCode = "0";
            param.doCustomer.BusinessTypeCode = "047";
            param.doCustomer.PhoneNo = "023946134";
            param.doCustomer.FaxNo = "023456541";
            param.doCustomer.IDNo = "1111111111111";
            param.doCustomer.DummyIDFlag = false;
            param.doCustomer.RegionCode = "TH";
            param.doCustomer.URL = "http://www.csigroups.com/";
            param.doCustomer.Memo = null;
            param.doCustomer.AddressEN = "SILOM COMPLEX TOWER, 28th FLOOR, NO.191";
            param.doCustomer.AlleyEN = null;
            param.doCustomer.RoadEN = "SILOM";
            param.doCustomer.SubDistrictEN = "SILOM";
            param.doCustomer.AddressFullEN = "SILOM COMPLEX TOWER, 28th FLOOR, NO.191,SILOM,SILOM,BANGRAK,BANGKOK 10210";
            param.doCustomer.AddressLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ";
            param.doCustomer.AlleyLC = null;
            param.doCustomer.RoadLC = "สีลม";
            param.doCustomer.SubDistrictLC = "สีลม";
            param.doCustomer.AddressFullLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ ถ.สีลม แขวงสีลม เขตบางรัก จ.กรุงเทพมหานคร 10210";
            param.doCustomer.DistrictCode = "00010";
            param.doCustomer.ProvinceCode = "001";
            param.doCustomer.ZipCode = "10210";
            param.doCustomer.DeleteFlag = false;
            //param.doCustomer.ValidateCustomerData=null;
            //param.doCustomer.SiteCustCode=null;

            dtCustomerGroup group = new dtCustomerGroup();
            group.CustCode = null;
            group.GroupCode = "G0000024";
            param.dtCustomerGroup.Add(group);
            group = new dtCustomerGroup();
            group.CustCode = null;
            group.GroupCode = "G0000105";
            param.dtCustomerGroup.Add(group);

            param.doSite.SiteCode = "S0000000192-0001";
            param.doSite.CustCode = null;
            param.doSite.SiteNo = "0001";
            param.doSite.SiteNameEN = "C.S.I. INTERHOLDINGS CO., LTD.";
            param.doSite.SiteNameLC = "บริษัท ซี.เอส.ไอ.อินเตอร์โฮลดิ้งส์ จำกัด";
            param.doSite.SECOMContactPerson = "Patcharee T.";
            param.doSite.PersonInCharge = "Patcharee T.";
            param.doSite.PhoneNo = "023946134";
            param.doSite.BuildingUsageCode = "019";
            param.doSite.AddressEN = "SILOM COMPLEX TOWER,15th FL., NO.191";
            param.doSite.AlleyEN = null;
            param.doSite.RoadEN = "SILOM";
            param.doSite.SubDistrictEN = "SILOM";
            param.doSite.AddressFullEN = "SILOM COMPLEX TOWER,15th FL., NO.191,SILOM,SILOM,BANGRAK,BANGKOK 10210";
            param.doSite.AddressLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ";
            param.doSite.AlleyLC = null;
            param.doSite.RoadLC = "สีลม";
            param.doSite.SubDistrictLC = "สีลม";
            param.doSite.AddressFullLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ ถ.สีลม แขวงสีลม เขตบางรัก จ.กรุงเทพมหานคร 10210";
            param.doSite.DistrictCode = "00010";
            param.doSite.ProvinceCode = "001";
            param.doSite.ZipCode = "10210";
            //param.doSite.ValidateSitetData=null;

            // Akat K. : to make sure that not effect to MSG0019
            CommonUtil.dsTransData.dtOperationData.ProcessDateTime = new DateTime(2011, 10, 31, 16, 0, 0);
            param.doCustomer.UpdateDate = new DateTime(2011, 10, 31, 16, 0, 0);
            param.doSite.UpdateDate = new DateTime(2011, 10, 31, 16, 0, 0);

            string expected = null;
            string actual = null;

            try {
                target.ManageCustomerTarget(param);
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 12, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Edit customer (Add customer group)
        ///     
        ///Parameters:
        ///     Refer to : "SECOM-AJIS-STC.MAP020-ManageCustomerTarget" tab 'Test Data (Input DO)'
        ///         
        ///Expected:
        ///     Refer to : "SECOM-AJIS-STC.MAP020-ManageCustomerTarget" tab expectation test case13
        ///
        ///</summary>
        public string Case13() {
            ICustomerMasterHandler target = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
            doCustomerTarget param = new doCustomerTarget();
            param.doCustomer = new doCustomer();
            param.doSite = new doSite();
            param.dtCustomerGroup = new List<dtCustomerGroup>();

            param.doCustomer.CustCode = "C0000000192";
            param.doCustomer.CustStatus = "0";
            param.doCustomer.ImportantFlag = null;
            param.doCustomer.CustNameEN = "C.S.I. Thailand";
            param.doCustomer.CustNameLC = "ซีเอสไอ ไทยแลนด์";
            param.doCustomer.CustFullNameEN = "C.S.I. Thailand CO., LTD.";
            param.doCustomer.CustFullNameLC = "บริษัท ซีเอสไอ ไทยแลนด์ จำกัด";
            param.doCustomer.RepPersonName = "Patcharee T.";
            param.doCustomer.ContactPersonName = "Patcharee T.";
            param.doCustomer.SECOMContactPerson = "Pornsri W.";
            param.doCustomer.CustTypeCode = "0";
            param.doCustomer.CompanyTypeCode = "03";
            param.doCustomer.FinancialMarketTypeCode = "0";
            param.doCustomer.BusinessTypeCode = "047";
            param.doCustomer.PhoneNo = "023946134";
            param.doCustomer.FaxNo = "023456541";
            param.doCustomer.IDNo = "9876123476549";
            param.doCustomer.DummyIDFlag = false;
            param.doCustomer.RegionCode = "TH";
            param.doCustomer.URL = "http://www.csigroups.com/";
            param.doCustomer.Memo = null;
            param.doCustomer.AddressEN = "SILOM COMPLEX TOWER, 28th FLOOR, NO.191";
            param.doCustomer.AlleyEN = null;
            param.doCustomer.RoadEN = "SILOM";
            param.doCustomer.SubDistrictEN = "SILOM";
            param.doCustomer.AddressFullEN = "SILOM COMPLEX TOWER, 28th FLOOR, NO.191,SILOM,SILOM,BANGRAK,BANGKOK 10210";
            param.doCustomer.AddressLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ";
            param.doCustomer.AlleyLC = null;
            param.doCustomer.RoadLC = "สีลม";
            param.doCustomer.SubDistrictLC = "สีลม";
            param.doCustomer.AddressFullLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ ถ.สีลม แขวงสีลม เขตบางรัก จ.กรุงเทพมหานคร 10210";
            param.doCustomer.DistrictCode = "00010";
            param.doCustomer.ProvinceCode = "001";
            param.doCustomer.ZipCode = "10210";
            param.doCustomer.DeleteFlag = false;
            //param.doCustomer.ValidateCustomerData=null;
            //param.doCustomer.SiteCustCode=null;

            dtCustomerGroup group = new dtCustomerGroup();
            group.CustCode = null;
            group.GroupCode = "G0000024";
            param.dtCustomerGroup.Add(group);
            group = new dtCustomerGroup();
            group.CustCode = null;
            group.GroupCode = "G0000105";
            param.dtCustomerGroup.Add(group);
            group = new dtCustomerGroup();
            group.CustCode = null;
            group.GroupCode = "G0000099";
            param.dtCustomerGroup.Add(group);

            param.doSite.SiteCode = "S0000000192-0001";
            param.doSite.CustCode = null;
            param.doSite.SiteNo = "0001";
            param.doSite.SiteNameEN = "C.S.I. INTERHOLDINGS CO., LTD.";
            param.doSite.SiteNameLC = "บริษัท ซี.เอส.ไอ.อินเตอร์โฮลดิ้งส์ จำกัด";
            param.doSite.SECOMContactPerson = "Patcharee T.";
            param.doSite.PersonInCharge = "Patcharee T.";
            param.doSite.PhoneNo = "023946134";
            param.doSite.BuildingUsageCode = "019";
            param.doSite.AddressEN = "SILOM COMPLEX TOWER,15th FL., NO.191";
            param.doSite.AlleyEN = null;
            param.doSite.RoadEN = "SILOM";
            param.doSite.SubDistrictEN = "SILOM";
            param.doSite.AddressFullEN = "SILOM COMPLEX TOWER,15th FL., NO.191,SILOM,SILOM,BANGRAK,BANGKOK 10210";
            param.doSite.AddressLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ";
            param.doSite.AlleyLC = null;
            param.doSite.RoadLC = "สีลม";
            param.doSite.SubDistrictLC = "สีลม";
            param.doSite.AddressFullLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ ถ.สีลม แขวงสีลม เขตบางรัก จ.กรุงเทพมหานคร 10210";
            param.doSite.DistrictCode = "00010";
            param.doSite.ProvinceCode = "001";
            param.doSite.ZipCode = "10210";
            //param.doSite.ValidateSitetData=null;

            // Akat K. : to make sure that not effect to MSG0019
            CommonUtil.dsTransData.dtOperationData.ProcessDateTime = new DateTime(2011, 10, 31, 16, 0, 0);
            param.doCustomer.UpdateDate = new DateTime(2011, 10, 31, 16, 0, 0);
            param.doSite.UpdateDate = new DateTime(2011, 10, 31, 16, 0, 0);

            string expected = null;
            string actual = null;

            try {
                target.ManageCustomerTarget(param);
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 13, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Edit customer (Remove any customer group)
        ///     
        ///Parameters:
        ///     Refer to : "SECOM-AJIS-STC.MAP020-ManageCustomerTarget" tab 'Test Data (Input DO)'
        ///         
        ///Expected:
        ///     Refer to : "SECOM-AJIS-STC.MAP020-ManageCustomerTarget" tab expectation test case14
        ///
        ///</summary>
        public string Case14() {
            ICustomerMasterHandler target = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
            doCustomerTarget param = new doCustomerTarget();
            param.doCustomer = new doCustomer();
            param.doSite = new doSite();
            param.dtCustomerGroup = new List<dtCustomerGroup>();

            param.doCustomer.CustCode = "C0000000192";
            param.doCustomer.CustStatus = "0";
            param.doCustomer.ImportantFlag = null;
            param.doCustomer.CustNameEN = "C.S.I. Thailand";
            param.doCustomer.CustNameLC = "ซีเอสไอ ไทยแลนด์";
            param.doCustomer.CustFullNameEN = "C.S.I. Thailand CO., LTD.";
            param.doCustomer.CustFullNameLC = "บริษัท ซีเอสไอ ไทยแลนด์ จำกัด";
            param.doCustomer.RepPersonName = "Patcharee T.";
            param.doCustomer.ContactPersonName = "Patcharee T.";
            param.doCustomer.SECOMContactPerson = "Pornsri W.";
            param.doCustomer.CustTypeCode = "0";
            param.doCustomer.CompanyTypeCode = "03";
            param.doCustomer.FinancialMarketTypeCode = "0";
            param.doCustomer.BusinessTypeCode = "047";
            param.doCustomer.PhoneNo = "023946134";
            param.doCustomer.FaxNo = "023456541";
            param.doCustomer.IDNo = "9876123476549";
            param.doCustomer.DummyIDFlag = false;
            param.doCustomer.RegionCode = "TH";
            param.doCustomer.URL = "http://www.csigroups.com/";
            param.doCustomer.Memo = null;
            param.doCustomer.AddressEN = "SILOM COMPLEX TOWER, 28th FLOOR, NO.191";
            param.doCustomer.AlleyEN = null;
            param.doCustomer.RoadEN = "SILOM";
            param.doCustomer.SubDistrictEN = "SILOM";
            param.doCustomer.AddressFullEN = "SILOM COMPLEX TOWER, 28th FLOOR, NO.191,SILOM,SILOM,BANGRAK,BANGKOK 10210";
            param.doCustomer.AddressLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ";
            param.doCustomer.AlleyLC = null;
            param.doCustomer.RoadLC = "สีลม";
            param.doCustomer.SubDistrictLC = "สีลม";
            param.doCustomer.AddressFullLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ ถ.สีลม แขวงสีลม เขตบางรัก จ.กรุงเทพมหานคร 10210";
            param.doCustomer.DistrictCode = "00010";
            param.doCustomer.ProvinceCode = "001";
            param.doCustomer.ZipCode = "10210";
            param.doCustomer.DeleteFlag = false;
            //param.doCustomer.ValidateCustomerData=null;
            //param.doCustomer.SiteCustCode=null;

            dtCustomerGroup group = new dtCustomerGroup();
            group.CustCode = null;
            group.GroupCode = "G0000105";
            param.dtCustomerGroup.Add(group);

            param.doSite.SiteCode = "S0000000192-0001";
            param.doSite.CustCode = null;
            param.doSite.SiteNo = "0001";
            param.doSite.SiteNameEN = "C.S.I. INTERHOLDINGS CO., LTD.";
            param.doSite.SiteNameLC = "บริษัท ซี.เอส.ไอ.อินเตอร์โฮลดิ้งส์ จำกัด";
            param.doSite.SECOMContactPerson = "Patcharee T.";
            param.doSite.PersonInCharge = "Patcharee T.";
            param.doSite.PhoneNo = "023946134";
            param.doSite.BuildingUsageCode = "019";
            param.doSite.AddressEN = "SILOM COMPLEX TOWER,15th FL., NO.191";
            param.doSite.AlleyEN = null;
            param.doSite.RoadEN = "SILOM";
            param.doSite.SubDistrictEN = "SILOM";
            param.doSite.AddressFullEN = "SILOM COMPLEX TOWER,15th FL., NO.191,SILOM,SILOM,BANGRAK,BANGKOK 10210";
            param.doSite.AddressLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ";
            param.doSite.AlleyLC = null;
            param.doSite.RoadLC = "สีลม";
            param.doSite.SubDistrictLC = "สีลม";
            param.doSite.AddressFullLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ ถ.สีลม แขวงสีลม เขตบางรัก จ.กรุงเทพมหานคร 10210";
            param.doSite.DistrictCode = "00010";
            param.doSite.ProvinceCode = "001";
            param.doSite.ZipCode = "10210";
            //param.doSite.ValidateSitetData=null;

            // Akat K. : to make sure that not effect to MSG0019
            CommonUtil.dsTransData.dtOperationData.ProcessDateTime = new DateTime(2011, 10, 31, 16, 0, 0);
            param.doCustomer.UpdateDate = new DateTime(2011, 10, 31, 16, 0, 0);
            param.doSite.UpdateDate = new DateTime(2011, 10, 31, 16, 0, 0);

            string expected = null;
            string actual = null;

            try {
                target.ManageCustomerTarget(param);
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 14, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Edit customer (Remove all customer group)
        ///     
        ///Parameters:
        ///     Refer to : "SECOM-AJIS-STC.MAP020-ManageCustomerTarget" tab 'Test Data (Input DO)'
        ///         
        ///Expected:
        ///     Refer to : "SECOM-AJIS-STC.MAP020-ManageCustomerTarget" tab expectation test case15
        ///
        ///</summary>
        public string Case15() {
            ICustomerMasterHandler target = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
            doCustomerTarget param = new doCustomerTarget();
            param.doCustomer = new doCustomer();
            param.doSite = new doSite();

            param.doCustomer.CustCode = "C0000000192";
            param.doCustomer.CustStatus = "0";
            param.doCustomer.ImportantFlag = null;
            param.doCustomer.CustNameEN = "C.S.I. Thailand";
            param.doCustomer.CustNameLC = "ซีเอสไอ ไทยแลนด์";
            param.doCustomer.CustFullNameEN = "C.S.I. Thailand CO., LTD.";
            param.doCustomer.CustFullNameLC = "บริษัท ซีเอสไอ ไทยแลนด์ จำกัด";
            param.doCustomer.RepPersonName = "Patcharee T.";
            param.doCustomer.ContactPersonName = "Patcharee T.";
            param.doCustomer.SECOMContactPerson = "Pornsri W.";
            param.doCustomer.CustTypeCode = "0";
            param.doCustomer.CompanyTypeCode = "03";
            param.doCustomer.FinancialMarketTypeCode = "0";
            param.doCustomer.BusinessTypeCode = "047";
            param.doCustomer.PhoneNo = "023946134";
            param.doCustomer.FaxNo = "023456541";
            param.doCustomer.IDNo = "9876123476549";
            param.doCustomer.DummyIDFlag = false;
            param.doCustomer.RegionCode = "TH";
            param.doCustomer.URL = "http://www.csigroups.com/";
            param.doCustomer.Memo = null;
            param.doCustomer.AddressEN = "SILOM COMPLEX TOWER, 28th FLOOR, NO.191";
            param.doCustomer.AlleyEN = null;
            param.doCustomer.RoadEN = "SILOM";
            param.doCustomer.SubDistrictEN = "SILOM";
            param.doCustomer.AddressFullEN = "SILOM COMPLEX TOWER, 28th FLOOR, NO.191,SILOM,SILOM,BANGRAK,BANGKOK 10210";
            param.doCustomer.AddressLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ";
            param.doCustomer.AlleyLC = null;
            param.doCustomer.RoadLC = "สีลม";
            param.doCustomer.SubDistrictLC = "สีลม";
            param.doCustomer.AddressFullLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ ถ.สีลม แขวงสีลม เขตบางรัก จ.กรุงเทพมหานคร 10210";
            param.doCustomer.DistrictCode = "00010";
            param.doCustomer.ProvinceCode = "001";
            param.doCustomer.ZipCode = "10210";
            param.doCustomer.DeleteFlag = false;
            //param.doCustomer.ValidateCustomerData=null;
            //param.doCustomer.SiteCustCode=null;

            param.doSite.SiteCode = "S0000000192-0001";
            param.doSite.CustCode = null;
            param.doSite.SiteNo = "0001";
            param.doSite.SiteNameEN = "C.S.I. INTERHOLDINGS CO., LTD.";
            param.doSite.SiteNameLC = "บริษัท ซี.เอส.ไอ.อินเตอร์โฮลดิ้งส์ จำกัด";
            param.doSite.SECOMContactPerson = "Patcharee T.";
            param.doSite.PersonInCharge = "Patcharee T.";
            param.doSite.PhoneNo = "023946134";
            param.doSite.BuildingUsageCode = "019";
            param.doSite.AddressEN = "SILOM COMPLEX TOWER,15th FL., NO.191";
            param.doSite.AlleyEN = null;
            param.doSite.RoadEN = "SILOM";
            param.doSite.SubDistrictEN = "SILOM";
            param.doSite.AddressFullEN = "SILOM COMPLEX TOWER,15th FL., NO.191,SILOM,SILOM,BANGRAK,BANGKOK 10210";
            param.doSite.AddressLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ";
            param.doSite.AlleyLC = null;
            param.doSite.RoadLC = "สีลม";
            param.doSite.SubDistrictLC = "สีลม";
            param.doSite.AddressFullLC = "191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ ถ.สีลม แขวงสีลม เขตบางรัก จ.กรุงเทพมหานคร 10210";
            param.doSite.DistrictCode = "00010";
            param.doSite.ProvinceCode = "001";
            param.doSite.ZipCode = "10210";
            //param.doSite.ValidateSitetData=null;

            // Akat K. : to make sure that not effect to MSG0019
            CommonUtil.dsTransData.dtOperationData.ProcessDateTime = new DateTime(2011, 10, 31, 16, 0, 0);
            param.doCustomer.UpdateDate = new DateTime(2011, 10, 31, 16, 0, 0);
            param.doSite.UpdateDate = new DateTime(2011, 10, 31, 16, 0, 0);

            string expected = null;
            string actual = null;

            try {
                target.ManageCustomerTarget(param);
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 15, expected, actual, CompareResult_String(expected, actual));
        }

    }
}