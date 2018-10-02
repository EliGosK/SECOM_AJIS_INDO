using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Contract.MetaData;
using SECOM_AJIS.DataEntity.Contract.Model;

namespace SECOM_AJIS.DataEntity.Contract
{

    public class doRegisterProjectData
    {
        public tbt_Project_CTS230 doTbt_Project { get; set; }
        public List<tbt_ProjectExpectedInstrumentDetails> doTbt_ProjectExpectedInstrumentDetail { get; set; }
        public List<tbt_ProjectOtherRalatedCompany> doTbt_ProjectOtherRalatedCompany { get; set; }
        public tbt_ProjectPurchaserCustomer doTbt_ProjectPurchaserCustomer { get; set; }
        public List<tbt_ProjectSupportStaffDetails> doTbt_ProjectSupportStaffDetails { get; set; }
        public List<tbt_ProjectSystemDetails> doTbt_ProjectSystemDetails { get; set; }
        

    }
}

