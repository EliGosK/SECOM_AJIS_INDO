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
    // GET: /SwtCMP070/

    public class SwtMAP022Controller : SwtCommonController {
        private static string SITE_FIELD = "SiteCode,CustCode,SiteNo,SiteNameEN,SiteNameLC,SECOMContactPerson,PersonInCharge,PhoneNo,BuildingUsageCode,AddressEN,AlleyEN,RoadEN,SubDistrictEN,AddressFullEN,AddressLC,AlleyLC,RoadLC,SubDistrictLC,AddressFullLC,DistrictCode,ProvinceCode,ZipCode,CreateDate,CreateBy,UpdateDate,UpdateBy";

        public string index() {
            List<string> lst = new List<string>();
            lst.Add(Case1());
            lst.Add(Case2());
            lst.Add(Case3());

            string result = CommonUtil.TextList(lst.ToArray());
            result = result.Replace(", Case", "Case");

            return result;
        }

        ///<summary>
        ///Purpose:
        ///     Mandatory check when site code is not specified.
        ///     
        ///Parameters:
        ///     siteCode: NULL
        ///         
        ///Expected:
        ///     MSG0007: "These field was required: siteCode."
        ///</summary>
        public string Case1() {
            ISiteMasterHandler target = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;
            string siteCode = null;
            string custCode = null;
            string expected = "MSG0007";
            string actual = null;

            try {
                target.GetSite(siteCode, custCode);
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 1, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Get site data when site code dose not exist
        ///     
        ///Parameters:
        ///     siteCode: S0000000014
        ///         
        ///Expected:
        ///     MSG2006: "Customer code not found, S0000000014."
        ///</summary>
        public string Case2() {
            ISiteMasterHandler target = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;
            string siteCode = "S0000000014";
            string custCode = null;
            string expected = "MSG2006";
            string actual = null;

            try {
                target.GetSite(siteCode, custCode);
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 2, expected, actual, CompareResult_String(expected, actual));
        }


        ///<summary>
        ///Purpose:
        ///     Get site data
        ///     
        ///Parameters:
        ///     siteCode: S0000000014-0001
        ///         
        ///Expected:
        ///     Refer to : "SECOM-AJIS-STC.MAP022-GetSite" sheet 'Expectation'
        ///</summary>
        public string Case3() {
            ISiteMasterHandler target = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;
            string siteCode = "S0000000014-0001";
            string custCode = null;

            List<doSite> expected = new List<doSite>();
            List<doSite> actual = null;
            //00007	001	10700	NULL	440065	NULL	440065
            doSite site = new doSite();
            site.SiteCode = "S0000000014-0001";
            site.CustCode = "C0000000014";
            site.SiteNo = "0001";
            site.SiteNameEN = "B-QUIK CO.,LTD.  (CARREFOUR RAMA 2 ROAD BRANCH)";
            site.SiteNameLC = "บริษัท บี-ควิค จำกัด (สาขาคาร์ฟูร์ พระราม2)";
            site.SECOMContactPerson = null;
            site.PersonInCharge = null;
            site.PhoneNo = null;
            site.BuildingUsageCode = "023";
            site.AddressEN = "189/1 MOO 6";
            site.AlleyEN = null;
            site.RoadEN = "RAMA 2";
            site.SubDistrictEN = "SAMAE-DAM";
            site.AddressFullEN = "189/1 MOO 6, RAMA 2, SAMAE-DAM, BANGKHUNTIEN, BANGKOK 10700";
            site.AddressLC = "189/1 หมู่ 6";
            site.AlleyLC = null;
            site.RoadLC = "พระราม 2";
            site.SubDistrictLC = "แสมดำ";
            site.AddressFullLC = "189/1 หมู่ 6 ถ.พระราม 2 แขวงแสมดำ เขตบางขุนเทียน จ.กรุงเทพมหานคร 10700";
            site.DistrictCode = "00007";
            site.ProvinceCode = "001";
            site.ZipCode = "10700";
            site.CreateDate = null;
            site.CreateBy = "440065";
            site.UpdateDate = null;
            site.UpdateBy = "440065";
            expected.Add(site);

            try {
                actual = target.GetSite(siteCode, custCode);
            } catch (ApplicationErrorException ex) {
                actual = null;
            } catch (Exception ex) {
                actual = null;
            }

            return string.Format(RESULT_FORMAT, 3, expected[0].SiteCode, actual[0].SiteCode, CompareObjectList<doSite>(expected, actual, SITE_FIELD) ? "Pass" : "Fail");
        }
    }
}