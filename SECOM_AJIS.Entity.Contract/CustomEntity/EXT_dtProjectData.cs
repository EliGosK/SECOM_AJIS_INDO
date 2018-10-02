using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Contract
{
    public partial class dtProjectData
    {
        CommonUtil c = new CommonUtil();

        public string ProjectCodeShow
        {
            get { return c.ConvertProjectCode(this.ProjectCode, CommonUtil.CONVERT_TYPE.TO_SHORT); }
        }

        public string ProjectNameAddressShow
        {
            get { return "(1) " + ProjectName + "<br/>(2) " + ProjectAddress; }
        }

        [LanguageMapping]
        public string ProjectPurchaseName { get; set; }
        [LanguageMapping]
        public string ProjectOwnerName { get; set; }

        public string ProjectPurchaseOwnerShow
        {
            get { return "(1) " + ProjectPurchaseName + "<br/>(2) " + ProjectOwnerName; }
        }

        [LanguageMapping]
        public string EmpFirstName { get; set; }
        [LanguageMapping]
        public string EmpLastName { get; set; }

        public string EmpNameShow
        {
            get { return this.EmpFirstName + " " + this.EmpLastName; }
        }

        [LanguageMapping]
        public string ManagerEmpFirstName { get; set; }
        [LanguageMapping]
        public string ManagerEmpLastName { get; set; }

        public string ManagerEmpShow
        {
            get { return this.ManagerEmpFirstName + " " + this.ManagerEmpLastName; }
        }

        public string PJManHeadSalesManShow
        {
            get { return "(1) " + ManagerEmpShow + "<br/>(2) " + EmpNameShow; }
        }

        [LanguageMapping]
        public string ProjectStatusName { get; set; }



        public string ToJson
        {
            get
            {
                return CommonUtil.CreateJsonString(this);
            }
        }
    }
}
