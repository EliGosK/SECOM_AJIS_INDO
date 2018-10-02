using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Master;

namespace SECOM_AJIS.Controllers {

    //
    // GET: /SwtMAP031/

    public class SwtMAP031Controller : SwtCommonController {
        private static string BILLING_CLIENT_FIELD = "BillingClientCode,NameEN,NameLC,FullNameEN,FullNameLC,BranchNameEN,BranchNameLC,CustTypeCode,CustTypeNameEN,CustTypeNameJP,CustTypeNameLC,CompanyTypeCode,CompanyTypeNameEN,CompanyTypeNameLC,BusinessTypeCode,BusinessTypeNameEN,BusinessTypeNameJP,BusinessTypeNameLC,PhoneNo,IDNo,RegionCode,NationalityEN,NationalityJP,NationalityLC,AddressEN,AddressLC,CreateDate,CreateBy,UpdateDate,UpdateBy";

        public string index() {
            List<string> lst = new List<string>();
            lst.Add(Case1());
            lst.Add(Case2());
            lst.Add(Case3());
            lst.Add(Case4());

            string result = CommonUtil.TextList(lst.ToArray());
            result = result.Replace(", Case", "Case");

            return result;
        }

        ///<summary>
        ///Purpose:
        ///     Mandatory check  when billing client code is not specified.
        ///     
        ///Parameters:
        ///     strBillingClientCode: NULL
        ///         
        ///Expected:
        ///     MSG0007: "These field was required: strBillingClientCode."
        ///</summary>
        public string Case1() {
            IBillingMasterHandler target = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;
            string strBillingClientCode = null;
            string expected = "MSG0007";
            string actual = null;

            try {
                target.GetBillingClient(strBillingClientCode);
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 1, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Get billing client data when billing client code dose not exist
        ///     
        ///Parameters:
        ///     strBillingClientCode: 0000000007
        ///         
        ///Expected:
        ///     Return dtBillingClient is null
        ///</summary>
        public string Case2() {
            IBillingMasterHandler target = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;
            string strBillingClientCode = "0000000007";
            List<dtBillingClientData> expected = null;
            List<dtBillingClientData> actual = new List<dtBillingClientData>();

            try {
                actual = target.GetBillingClient(strBillingClientCode);
            } catch (ApplicationErrorException ex) {
                actual = new List<dtBillingClientData>();
            } catch (Exception ex) {
                actual = new List<dtBillingClientData>();
            }

            return string.Format(RESULT_FORMAT, 2, expected, actual, CompareResult_Object(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Get billing client data 
        ///     
        ///Parameters:
        ///     strBillingClientCode: 0000000036
        ///         
        ///Expected:
        ///     Expect test case 4
        ///</summary>
        public string Case3() {
            IBillingMasterHandler target = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;
            string strBillingClientCode = "0000000036";
            List<dtBillingClientData> expected = new List<dtBillingClientData>();
            List<dtBillingClientData> actual = null;

            dtBillingClientData billing = new dtBillingClientData();
            billing.BillingClientCode = "0000000036";
            billing.NameEN = "FUJI XEROX (THAILAND)";
            billing.NameLC = "ฟูจิ ซีร็อกซ์ (ประเทศไทย)";
            billing.FullNameEN = "FUJI XEROX (THAILAND) CO.,LTD.";
            billing.FullNameLC = "บริษัท ฟูจิ ซีร็อกซ์ (ประเทศไทย) จำกัด";
            billing.BranchNameEN = "Head office";
            billing.BranchNameLC = "สำนักงานใหญ่";
            billing.CustTypeCode = "0";
            billing.CustTypeNameEN = "Juristic";
            billing.CustTypeNameJP = "法人";
            billing.CustTypeNameLC = "JuristicLC";
            billing.CompanyTypeCode = "03";
            billing.CompanyTypeNameEN = "Limited company";
            billing.CompanyTypeNameLC = "บริษัทจำกัด";
            billing.BusinessTypeCode = "013";
            billing.BusinessTypeNameEN = "Electrical Equipment";
            billing.BusinessTypeNameJP = "電気機器";
            billing.BusinessTypeNameLC = "อุปกรณ์อิเล็กทรอนิกส์";
            billing.PhoneNo = "0868965423";
            billing.IDNo = "1234567890000";
            billing.RegionCode = "TH";
            billing.NationalityEN = "Thai";
            billing.NationalityJP = "タイ人";
            billing.NationalityLC = "คนไทย";
            billing.AddressEN = "123 SUNTOWERS  BUILDING A,23rd-26th FLOOR,VIBHAVADI-RANGSIT,CHOMPHON,JATUJAK,BANGKOK 10700";
            billing.AddressLC = "123 อาคารทานตะวัน A ชั้น 23-26 ถ.วิภาวดีรังสิต แขวงจอมพล เขตจตุจักร จ.กรุงเทพมหานคร 10700";
            billing.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            billing.CreateBy = "500576";
            billing.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            billing.UpdateBy = "500576";
            expected.Add(billing);
            CommonUtil.MappingObjectLanguage<dtBillingClientData>(expected);

            try {
                actual = target.GetBillingClient(strBillingClientCode);
            } catch (ApplicationErrorException ex) {
                actual = new List<dtBillingClientData>();
            } catch (Exception ex) {
                actual = new List<dtBillingClientData>();
            }

            return string.Format(RESULT_FORMAT, 3, expected[0].BillingClientCode, actual[0].BillingClientCode, CompareObjectList<dtBillingClientData>(expected, actual, BILLING_CLIENT_FIELD) ? "Pass" : "Fail");
        }

        ///<summary>
        ///Purpose:
        ///     Get billing client data (Max length all columns)
        ///     
        ///Parameters:
        ///     strBillingClientCode: 0000000179
        ///         
        ///Expected:
        ///     1. Expect test case 5
        ///     Note: At end character of text column will be 'z' or 'ฮ'
        ///     
        ///</summary>
        public string Case4() {
            IBillingMasterHandler target = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;
            string strBillingClientCode = "0000000179";
            List<dtBillingClientData> expected = new List<dtBillingClientData>();
            List<dtBillingClientData> actual = null;

            dtBillingClientData billing = new dtBillingClientData();
            billing.BillingClientCode = "0000000179";
            billing.NameEN = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxz";
            billing.NameLC = "กกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกฮ";
            billing.FullNameEN = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxzxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxz";
            billing.FullNameLC = "กกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกฮกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกฮ";
            billing.BranchNameEN = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxz";
            billing.BranchNameLC = "กกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกฮ";
            billing.CustTypeCode = "0";
            billing.CustTypeNameEN = "Juristic";
            billing.CustTypeNameJP = "法人";
            billing.CustTypeNameLC = "JuristicLC";
            billing.CompanyTypeCode = "03";
            billing.CompanyTypeNameEN = "Limited company";
            billing.CompanyTypeNameLC = "บริษัทจำกัด";
            billing.BusinessTypeCode = "005";
            billing.BusinessTypeNameEN = "Automotive Auto Parts";
            billing.BusinessTypeNameJP = "自動車産業・部品";
            billing.BusinessTypeNameLC = "ชิ้นส่วนรถยนต์";
            billing.PhoneNo = "12345678901234567890";
            billing.IDNo = "12345678901234567890";
            billing.RegionCode = "TH"; 
            billing.NationalityEN = "Thai";
            billing.NationalityJP = "タイ人";
            billing.NationalityLC = "คนไทย";
            billing.AddressEN = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxz";
            billing.AddressLC = "กกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกฮ";
            billing.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            billing.CreateBy = "500576";
            billing.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            billing.UpdateBy = "500576";
            expected.Add(billing);
            CommonUtil.MappingObjectLanguage<dtBillingClientData>(expected);

            try {
                actual = target.GetBillingClient(strBillingClientCode);
            } catch (ApplicationErrorException ex) {
                actual = new List<dtBillingClientData>();
            } catch (Exception ex) {
                actual = new List<dtBillingClientData>();
            }

            return string.Format(RESULT_FORMAT, 4, expected[0].BillingClientCode, actual[0].BillingClientCode, CompareObjectList<dtBillingClientData>(expected, actual, BILLING_CLIENT_FIELD) ? "Pass" : "Fail");
        }

    }
}