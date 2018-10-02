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
    // GET: /SwtCMP210/

    public class SwtMAP021Controller : SwtCommonController {
        public string index() {
            List<string> lst = new List<string>();
            lst.Add(Case1());
            lst.Add(Case2());
            lst.Add(Case3());
            lst.Add(Case4());
            lst.Add(Case5());
            lst.Add(Case6());

            string result = CommonUtil.TextList(lst.ToArray());
            result = result.Replace(", Case", "Case");

            return result;
        }

        ///<summary>
        ///Purpose:
        ///     Mandatory check
        ///     
        ///Parameters:
        ///     strCustCode: NULL
        ///         
        ///Expected:
        ///     MSG0007: "These field was required: custCode."
        ///</summary>
        public string Case1() {
            ICustomerMasterHandler target = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
            string strCustCode = null;
            string expected = "MSG0007";
            string actual = null;

            try {
                target.GetCustomer(strCustCode);
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 1, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Get customer data when customer code dose not exist
        ///     
        ///Parameters:
        ///     strCustCode: C0000000000
        ///         
        ///Expected:
        ///     Return doCustomer is null
        ///</summary>
        public string Case2() {
            ICustomerMasterHandler target = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
            string strCustCode = "C0000000000";
            List<doCustomer> expected = null;
            List<doCustomer> actual = null;

            try {
                actual = target.GetCustomer(strCustCode);
            } catch (ApplicationErrorException ex) {
                actual = new List<doCustomer>();
            } catch (Exception ex) {
                actual = new List<doCustomer>();
            }

            return string.Format(RESULT_FORMAT, 2, expected, actual, CompareResult_Object(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Get customer data when customer data was deleted
        ///     
        ///Parameters:
        ///     strCustCode: C0000000032
        ///         
        ///Expected:
        ///     Return doCustomer is null
        ///</summary>
        public string Case3() {
            ICustomerMasterHandler target = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
            string strCustCode = "C0000000032";
            List<doCustomer> expected = null;
            List<doCustomer> actual = null;

            try {
                actual = target.GetCustomer(strCustCode);
            } catch (ApplicationErrorException ex) {
                actual = new List<doCustomer>();
            } catch (Exception ex) {
                actual = new List<doCustomer>();
            }

            return string.Format(RESULT_FORMAT, 3, expected, actual, CompareResult_Object(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Get customer data when customer has customer group
        ///
        ///Parameters:
        ///     strCustCode: C0000000014
        ///
        ///Expected:
        ///     Refer to: "SECOM-AJIS-STC.MAP021-GetCustomer" tab 'Expectation'
        ///
        ///</summary>
        public string Case4() {
            ICustomerMasterHandler target = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
            string strCustCode = "C0000000014";
            List<doCustomer> expected = null;
            List<doCustomer> actual = null;

            doCustomer customer = new doCustomer();
            customer.CustCode = "C0000000014";
			customer.CustStatus = "1";
			customer.CustStatusNameEN = "Existing customer";
			customer.CustStatusNameJP = "既存顧客";
			customer.CustStatusNameLC = "Existing customer";
			customer.ImportantFlag = true;
			customer.CustNameEN = "B-QUIK";
			customer.CustNameLC = "บี-ควิก";
			customer.CustFullNameEN = "B-QUIK COMPANY LIMITED";
			customer.CustFullNameLC = "บริษัท บี-ควิก จำกัด ";
			customer.RepPersonName = null;
			customer.ContactPersonName = null;
			customer.SECOMContactPerson = null;
			customer.CustTypeCode = "0";
			customer.CustTypeNameEN = "Juristic";
			customer.CustTypeNameJP = "法人";
			customer.CustTypeNameLC = "Juristic";
			customer.CompanyTypeCode = "03";
			customer.CompanyTypeNameEN = "Limited company";
			customer.CompanyTypeNameLC = "บริษัทจำกัด";
			customer.FinancialMarketTypeCode = "0";
			customer.FinancialMaketTypeNameEN = "None";
			customer.FinancialMaketTypeNameJP = "";
			customer.FinancialMaketTypeNameLC = "None";
			customer.BusinessTypeCode = "005";
			customer.BusinessTypeNameEN = "Automotive Auto Parts";
			customer.BusinessTypeNameJP = "自動車産業・部品";
			customer.BusinessTypeNameLC = "ชิ้นส่วนรถยนต์";
			customer.PhoneNo = "026200900";
			customer.FaxNo = null;
			customer.IDNo = "DMID000000004";
			customer.DummyIDFlag = true;
			customer.RegionCode = "TH";
			customer.NationalityEN = "Thai";
			customer.NationalityJP = "タイ人";
			customer.NationalityLC = "คนไทย";
			customer.URL = null;
			customer.Memo = null;
			customer.AddressEN = "253 16th FL.";
			customer.AlleyEN = "SUKHUMVIT 21 (ASOKE)";
			customer.RoadEN = "SUKHUMVIT";
			customer.SubDistrictEN = "KLONGTOEY NEUA";
			customer.AddressFullEN = "253 16th FL.,SUKHUMVIT 21 (ASOKE),KLONGTOEY NEUA,WATTANA,BANGKOK 10400";
			customer.AddressLC = "253";
			customer.AlleyLC = "สุขุมวิท 21 (อโศก)";
			customer.RoadLC = "สุขุมวิท";
			customer.SubDistrictLC = "คลองเตยเหนือ";
			customer.AddressFullLC = "253 ซ.สุขุมวิท 21 (อโศก) ถ.สุขุมวิท แขวงคลองเตยเหนือ เขตวัฒนา จ.กรุงเทพมหานคร 10400";
			customer.DistrictCode = "00049";
			customer.DistrictNameEN = "Vadhana";
			customer.DistrictNameLC = "วัฒนา";
			customer.ProvinceCode = "001";
			customer.ProvinceNameEN = "BANGKOK ";
			customer.ProvinceNameLC = "กรุงเทพมหานคร";
			customer.ZipCode = "10400";
			customer.DeleteFlag = false;
			customer.CreateDate = new DateTime(2011,7,13,12,9,0);
			customer.CreateBy = "440065";
            customer.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
			customer.UpdateBy = "440065";
            //customer.ValidateCustomerData = null;
            //customer.SiteCustCode = "S0000000014";

            List<dtCustomerGroupForView> groupList = new List<dtCustomerGroupForView>();
            dtCustomerGroupForView group = new dtCustomerGroupForView();
            group.CustCode = "C0000000014";
            group.GroupCode = "G0000017";
            group.GroupNameEN = "Sizzler group";
            group.GroupNameLC = "ซิสเลอร์ กรุ๊ป";
            group.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            group.CreateBy = "440065";
            group.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            group.UpdateBy = "440065";
            groupList.Add(group);

            group = new dtCustomerGroupForView();
            group.CustCode = "C0000000014";
            group.GroupCode = "G0000024";
            group.GroupNameEN = "Honda group";
            group.GroupNameLC = "ฮอนด้า กรุ๊ป";
            group.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            group.CreateBy = "440065";
            group.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            group.UpdateBy = "440065";
            groupList.Add(group);

            group = new dtCustomerGroupForView();
            group.CustCode = "C0000000014";
            group.GroupCode = "G0000038";
            group.GroupNameEN = "Toyota group";
            group.GroupNameLC = "โตโยต้า กรุ๊ป";
            group.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            group.CreateBy = "440065";
            group.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            group.UpdateBy = "440065";
            groupList.Add(group);

            group = new dtCustomerGroupForView();
            group.CustCode = "C0000000014";
            group.GroupCode = "G0000041";
            group.GroupNameEN = "Central pattana";
            group.GroupNameLC = "เซ็นทรัลพัฒนา";
            group.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            group.CreateBy = "440065";
            group.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            group.UpdateBy = "440065";
            groupList.Add(group);

            group = new dtCustomerGroupForView();
            group.CustCode = "C0000000014";
            group.GroupCode = "G0000056";
            group.GroupNameEN = "XEROX group";
            group.GroupNameLC = "ซีร็อกซ์ กรุ๊ป";
            group.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            group.CreateBy = "440065";
            group.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            group.UpdateBy = "440065";
            groupList.Add(group);

            try {
                actual = target.GetCustomer(strCustCode);
            } catch (ApplicationErrorException ex) {
                actual = new List<doCustomer>();
            } catch (Exception ex) {
                actual = new List<doCustomer>();
            }

            return string.Format(RESULT_FORMAT, 4, expected, actual, CompareResult_Object(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Get customer data when customer has not customer group
        ///     
        ///Parameters:
        ///     strCustCode: C0000000166
        ///         
        ///Expected:
        ///     Refer to: "SECOM-AJIS-STC.MAP021-GetCustomer" tab 'Expectation'
        ///</summary>
        public string Case5() {
            ICustomerMasterHandler target = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
            string strCustCode = "C0000000166";
            List<doCustomer> expected = null;
            List<doCustomer> actual = null;

            doCustomer customer = new doCustomer();
            customer.CustCode = "C0000000166";
            customer.CustStatus = "1";
            customer.CustStatusNameEN = "Existing customer";
            customer.CustStatusNameJP = "既存顧客";
            customer.CustStatusNameLC = "Existing customer";
            customer.ImportantFlag = false;
            customer.CustNameEN = "K.ANANCHA TRONGJITPHANICH";
            customer.CustNameLC = "คุณอาณัญชา ตรงจิตพานิช";
            customer.CustFullNameEN = "K.ANANCHA TRONGJITPHANICH";
            customer.CustFullNameLC = "คุณอาณัญชา ตรงจิตพานิช";
            customer.RepPersonName = null;
            customer.ContactPersonName = null;
            customer.SECOMContactPerson = null;
            customer.CustTypeCode = "1";
            customer.CustTypeNameEN = "Individual";
            customer.CustTypeNameJP = "個人";
            customer.CustTypeNameLC = "Individual";
            customer.CompanyTypeCode = null;
            customer.CompanyTypeNameEN = null;
            customer.CompanyTypeNameLC = null;
            customer.FinancialMarketTypeCode = "0";
            customer.FinancialMaketTypeNameEN = "None";
            customer.FinancialMaketTypeNameJP = "";
            customer.FinancialMaketTypeNameLC = "None";
            customer.BusinessTypeCode = "045";
            customer.BusinessTypeNameEN = "Trust Company";
            customer.BusinessTypeNameJP = "信託会社";
            customer.BusinessTypeNameLC = "บริษัททรัชต์";
            customer.PhoneNo = null;
            customer.FaxNo = null;
            customer.IDNo = "3118371001863";
            customer.DummyIDFlag = false;
            customer.RegionCode = "TH";
            customer.NationalityEN = "Thai";
            customer.NationalityJP = "タイ人";
            customer.NationalityLC = "คนไทย";
            customer.URL = null;
            customer.Memo = null;
            customer.AddressEN = "150/140 TOWN PLUS PETCHKASEM-BANGKHAE";
            customer.AlleyEN = null;
            customer.RoadEN = "PUTTHAMONTHON SAI 1";
            customer.SubDistrictEN = "BANGDUAN";
            customer.AddressFullEN = "150/140 TOWN PLUS PETCHKASEM-BANGKHAE, PUTTHAMONTHON SAI 1 RD., BANGDUAN, PHASI CHAROEN, BANGKOK 10160";
            customer.AddressLC = "150/140 ทาว์นพลัส เพชรเกษม-บางแค";
            customer.AlleyLC = null;
            customer.RoadLC = "พุทธมณฑล สาย 1";
            customer.SubDistrictLC = "บางด้วน";
            customer.AddressFullLC = "150/140 ทาวน์พลัส เพชรเกษม-บางแค ถ.พุทธมณฑล สาย 1 แขวงบางด้วน เขตภาษีเจริญ จ.กรุงเทพมหานคร 10160";
            customer.DistrictCode = "00032";
            customer.DistrictNameEN = "Phasi Charoen";
            customer.DistrictNameLC = "ภาษีเจริญ";
            customer.ProvinceCode = "001";
            customer.ProvinceNameEN = "BANGKOK ";
            customer.ProvinceNameLC = "กรุงเทพมหานคร";
            customer.ZipCode = "10160";
            customer.DeleteFlag = false;
            customer.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            customer.CreateBy = "440065";
            customer.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            customer.UpdateBy = "440065";
            //customer.ValidateCustomerData = "";
            //customer.SiteCustCode = "S0000000166";

            List<dtCustomerGroupForView> groupList = new List<dtCustomerGroupForView>();

            try {
                actual = target.GetCustomer(strCustCode);
            } catch (ApplicationErrorException ex) {
                actual = new List<doCustomer>();
            } catch (Exception ex) {
                actual = new List<doCustomer>();
            }

            return string.Format(RESULT_FORMAT, 5, expected, actual, CompareResult_Object(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Get customer data (Max length all columns)
        ///     
        ///Parameters:
        ///     strCustCode: C0000000189
        ///         
        ///Expected:
        ///     Note: At end character of text column will be 'z' or 'ฮ'
        ///</summary>
        public string Case6()
        {
            ICustomerMasterHandler target = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
            string strCustCode = "C0000000189";
            List<doCustomer> expected = null;
            List<doCustomer> actual = null;

            doCustomer customer = new doCustomer();
            customer.CustCode = "C0000000189";
            customer.CustStatus = "1";
            customer.CustStatusNameEN = "Existing customer";
            customer.CustStatusNameJP = "既存顧客";
            customer.CustStatusNameLC = "Existing customer";
            customer.ImportantFlag = false;
            customer.CustNameEN = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxz";
            customer.CustNameLC = "กกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกฮ";
            customer.CustFullNameEN = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxz";
            customer.CustFullNameLC = "กกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกฮ";
            customer.RepPersonName = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxz";
            customer.ContactPersonName = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxz";
            customer.SECOMContactPerson = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxz";
            customer.CustTypeCode = "1";
            customer.CustTypeNameEN = "Individual";
            customer.CustTypeNameJP = "個人";
            customer.CustTypeNameLC = "Individual";
            customer.CompanyTypeCode = "01";
            customer.CompanyTypeNameEN = "Registered ordinary partnership";
            customer.CompanyTypeNameLC = "ห้างหุ้นส่วนสามัญนิติบุคคล";
            customer.FinancialMarketTypeCode = "1";
            customer.FinancialMaketTypeNameEN = "SET";
            customer.FinancialMaketTypeNameJP = "";
            customer.FinancialMaketTypeNameLC = "SET";
            customer.BusinessTypeCode = "047";
            customer.BusinessTypeNameEN = "Other";
            customer.BusinessTypeNameJP = "その他";
            customer.BusinessTypeNameLC = "อื่นๆ";
            customer.PhoneNo = "12345678901234567890";
            customer.FaxNo = "12345678901234567890";
            customer.IDNo = "12345678901234567890";
            customer.DummyIDFlag = false;
            customer.RegionCode = "TH";
            customer.NationalityEN = "Thai";
            customer.NationalityJP = "タイ人";
            customer.NationalityLC = "คนไทย";
            customer.URL = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxz";
            customer.Memo = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxz"; customer.AddressEN = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxz"; customer.AlleyEN = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxz"; customer.RoadEN = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxz";
            customer.SubDistrictEN = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxz";
            customer.AddressFullEN = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxz";
            customer.AddressLC = "กกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกฮ";
            customer.AlleyLC = "กกกกกกกกกกกกกกฮ";
            customer.RoadLC = "กกกกกกกกกกกกกกฮ";
            customer.SubDistrictLC = "กกกกกกกกกกกกกกฮ";
            customer.AddressFullLC = "กกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกฮ";
            customer.DistrictCode = "00488";
            customer.DistrictNameEN = "Bang Bua Thong";
            customer.DistrictNameLC = "บางบัวทอง";
            customer.ProvinceCode = "035";
            customer.ProvinceNameEN = "NONTHABURI";
            customer.ProvinceNameLC = "นนทบุรี";
            customer.ZipCode = "11110";
            customer.DeleteFlag = false;
            customer.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            customer.CreateBy = "440065";
            customer.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            customer.UpdateBy = "440065";
            //customer.ValidateCustomerData = "";
            //customer.SiteCustCode = "";

            List<dtCustomerGroupForView> groupList = new List<dtCustomerGroupForView>();
            dtCustomerGroupForView group = new dtCustomerGroupForView();
            group.CustCode = "C0000000099";
            group.GroupCode = "G0000100";
            group.GroupNameEN = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxz";
            group.GroupNameLC = "กกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกฮ";
            group.CreateDate = new DateTime(2011, 10, 2, 12, 9, 0);
            group.CreateBy = "500576";
            group.UpdateDate = new DateTime(2011, 10, 2, 12, 9, 0);
            group.UpdateBy = "500576";
            groupList.Add(group);

            try {
                actual = target.GetCustomer(strCustCode);
            } catch (ApplicationErrorException ex) {
                actual = new List<doCustomer>();
            } catch (Exception ex) {
                actual = new List<doCustomer>();
            }

            return string.Format(RESULT_FORMAT, 6, expected, actual, CompareResult_Object(expected, actual));
        }

    }
}