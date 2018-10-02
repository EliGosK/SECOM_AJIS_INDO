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
    // GET: /SwtMAP030/

    public class SwtMAP030Controller : SwtCommonController {
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
        ///     Mandatory check (No billing client data)
        ///     
        ///Parameters:
        ///     Refer to: "SECOM-AJIS-STC.MAP030-ManageBillingClient" tab 'Test Data(Input DO)'
        ///         
        ///Expected:
        ///     Refer to: "SECOM-AJIS-STC.MAP030-ManageBillingClient" tab 'Expect Result'
        ///</summary>
        public string Case1() {
            IBillingMasterHandler target = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;
            tbm_BillingClient param = new tbm_BillingClient();
            string expected = null;
            string actual = null;

            try {
                target.ManageBillingClient(param);
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 1, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///    New billing client
        ///     
        ///Parameters:
        ///     Refer to: "SECOM-AJIS-STC.MAP030-ManageBillingClient" tab 'Test Data(Input DO)'
        ///         
        ///Expected:
        ///     Refer to: "SECOM-AJIS-STC.MAP030-ManageBillingClient" tab 'Expect Result'
        ///</summary>
        public string Case2() {
            IBillingMasterHandler target = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;
            tbm_BillingClient param = new tbm_BillingClient();
            string expected = null;
            string actual = null;

            param.BillingClientCode=null;
            param.NameEN="C.S.I. INTERHOLDINGS";
            param.NameLC="ซีเอสไอ อินเตอร์โฮลดิ้ง";
            param.FullNameEN="C.S.I. INTERHOLDINGS CO., LTD.";
            param.FullNameLC="บริษัท ซีเอสไอ อินเตอร์โฮลดิ้ง จำกัด";
            param.BranchNameEN="Head office";
            param.BranchNameLC="สำนักงานใหญ่";
            param.CustTypeCode="0";
            //param.CustTypeName=null;
            param.CompanyTypeCode="03";
            //param.CompanyTypeName=null;
            param.BusinessTypeCode="047";
            param.BusinessTypeName=null;
            param.PhoneNo="021234567";
            param.IDNo="1234567890123";
            param.RegionCode="TH";
            param.Nationality=null;
            param.AddressEN="SILOM COMPLEX TOWER, 28th FLOOR, NO.191,SILOM,SILOM,BANGRAK,BANGKOK 10210";
            param.AddressLC="191 อาคารสีลมคอมเพล็กซ์ ชั้น 28 เอฟ ถ.สีลม แขวงสีลม เขตบางรัก จ.กรุงเทพมหานคร 10210";
            param.DeleteFlag = false;

            try {
                target.ManageBillingClient(param);
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 2, expected, actual, CompareResult_String(expected, actual));
        }
        ///<summary>
        ///Purpose:
        ///    New billing client (max length)
        ///     
        ///Parameters:
        ///     Refer to: "SECOM-AJIS-STC.MAP030-ManageBillingClient" tab 'Test Data(Input DO)'
        ///         
        ///Expected:
        ///     Refer to: "SECOM-AJIS-STC.MAP030-ManageBillingClient" tab 'Expect Result'
        ///</summary>
        public string Case3() {
            IBillingMasterHandler target = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;
            tbm_BillingClient param = new tbm_BillingClient();
            string expected = null;
            string actual = null;

            param.BillingClientCode=null;
            param.NameEN="xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxz";
            param.NameLC="กกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกฮ";
            param.FullNameEN="xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxzxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxz";
            param.FullNameLC="กกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกฮกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกฮ";
            param.BranchNameEN="xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxz";
            param.BranchNameLC="กกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกฮ";
            param.CustTypeCode="0";
            //param.CustTypeName=null;
            param.CompanyTypeCode="03";
            //param.CompanyTypeName=null;
            param.BusinessTypeCode="005";
            param.BusinessTypeName=null;
            param.PhoneNo="12345678901234567890";
            param.IDNo="12345678901234567890";
            param.RegionCode="TH";
            param.Nationality=null;
            param.AddressEN="xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxz";
            param.AddressLC="กกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกกฮ";
            param.DeleteFlag=false;

            try {
                target.ManageBillingClient(param);
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 3, expected, actual, CompareResult_String(expected, actual));
        }
    }
}