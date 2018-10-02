using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Contract;

namespace SECOM_AJIS.Controllers {

    //
    // GET: /SwtMAP031/

    public class SwtMAP099Controller : SwtCommonController {
        private static string BILLING_CLIENT_FIELD = "BillingClientCode,NameEN,NameLC,FullNameEN,FullNameLC,BranchNameEN,BranchNameLC,CustTypeCode,CustTypeNameEN,CustTypeNameJP,CustTypeNameLC,CompanyTypeCode,CompanyTypeNameEN,CompanyTypeNameLC,BusinessTypeCode,BusinessTypeNameEN,BusinessTypeNameJP,BusinessTypeNameLC,PhoneNo,IDNo,RegionCode,NationalityEN,NationalityJP,NationalityLC,AddressEN,AddressLC,CreateDate,CreateBy,UpdateDate,UpdateBy";

        public string index() {
            List<string> lst = new List<string>();
            lst.Add(Case1());
        

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
            IMaintenanceHandler target = ServiceContainer.GetService<IMaintenanceHandler>() as IMaintenanceHandler;
          
            string expected = "ttt";
            string actual = null;

            tbt_MaintenanceCheckup doInsert = new tbt_MaintenanceCheckup() { ContractCode = "N0000000008", ProductCode = "009", InstructionDate = DateTime.Now , Remark="ฟหกด"};

            try {
                target.InsertTbt_MaintenanceCheckup(doInsert);
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 1, expected, actual, CompareResult_String(expected, actual));
        }

        

    }
}