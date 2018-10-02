using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Common
{
    /// <summary>
    /// DO for collect all search condition in screen CMS070
    /// </summary>
    public class doSearchInfoCondition
    {
		//CMS070_SearchCode
        public string GroupCode { get; set; }
        public string CustomerCode { get; set; }
        public string SiteCode { get; set; }
        public string ContractCode { get; set; }
        public string UserCode { get; set; }
        public string PlanCode { get; set; }
        public string ProjectCode { get; set; }

        //CMS070_SearchContract
        public bool? SearchContractAll { get; set; }
        public bool? SearchContractRental { get; set; }
        public bool? SearchContractSale { get; set; }

        //CMS070_SearchCondition_Real
        public string CustomerName { get; set; }
        public string Branchename { get; set; }
        public string GroupName { get; set; }
        public string CustomerRole { get; set; }
        public string CustomerStatusNew { get; set; }
        public string CustomerStatusExist { get; set; }
        public string CustTypeJuristic { get; set; }
        public string CompanyType { get; set; }
        public string CustTypeIndividual { get; set; }
        public string CustTypeAssociation { get; set; }
        public string CustTypePublicOffice { get; set; }
        public string CustTypeOther { get; set; }
        public string CustomerIDNo { get; set; }
        public string CustomerNatioality { get; set; }
        public string CustomerBusinessType { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerSoi { get; set; }
        public string CustomerZipCode { get; set; }
        public string CustomerRoad { get; set; }
        public string CustomerTelephone { get; set; }
        public string CustomerTumbol { get; set; }
        public string CustomerFax { get; set; }
        public string CustomerJangwat { get; set; }
        public string CustomerAmper { get; set; }

        //CMS070_SearchCondition_Site
        public string SiteName { get; set; }
        public string SiteAddress { get; set; }
        public string SiteSoi { get; set; }
        public string SiteZipCode { get; set; }
        public string SiteRoad { get; set; }
        public string SiteTelephone { get; set; }
        public string SiteTambol { get; set; }
        public string SiteJangwat { get; set; }
        public string SiteAmper { get; set; }

        //CMS070_SearchCondition_Contact
        public DateTime? OperationDateFrom { get; set; }
        public DateTime? OperationDateTo { get; set; }
        public DateTime? StopDateFrom { get; set; }
        public DateTime? StopDateTo { get; set; }
        public DateTime? CancelDateFrom { get; set; }
        public DateTime? CancelDateTo { get; set; }
        public DateTime? CustAcceptDateFrom { get; set; }
        public DateTime? CustAcceptDateTo { get; set; }
        public DateTime? CompleteDateFrom { get; set; }
        public DateTime? CompleteDateTo { get; set; }
        public string ContractOffice { get; set; }
        public string OperationOffice { get; set; }
        public string SaleEmpNo { get; set; }
        public string SaleName { get; set; }
        public string ProductName { get; set; }
        public string ChangeType { get; set; }
        public string ProcessStatus { get; set; }
        public string StartType { get; set; }

        /*
         * All Field for Reference
        CMS070_SearchCode
        string GroupCode
        string CustomerCode
        string SiteCode
        ContractCode
        UserCode
        PlanCode
        ProjectCode

        CMS070_SearchContract
        SearchContractAll
        SearchContractRental
        SearchContractSale

        CMS070_SearchCondition_Real
        CustomerName
        Branchename
        GroupName
        CustomerRole
        CustomerStatusNew
        CustomerStatusExist
        CustTypeJuristic
        CompanyType
        CustTypeIndividual
        CustTypeAssociation
        CustTypePublicOffice
        CustTypeOther
        CustomerIDNo
        CustomerNatioality
        CustomerBusinessType
        CustomerAddress
        CustomerSoi
        CustomerZipCode
        CustomerRoad
        CustomerTelephone
        CustomerTumbol
        CustomerFax
        CustomerJangwat
        CustomerAmper

        CMS070_SearchCondition_Site
        SiteName
        SiteAddress
        SiteSoi
        SiteZipCode
        SiteRoad
        SiteTelephone
        SiteTambol
        SiteJangwat
        SiteAmper

        CMS070_SearchCondition_Contact
        OperationDateFrom
        OperationDateTo
        CustAcceptDateFrom
        CustAcceptDateTo
        CompleteDateFrom
        CompleteDateTo
        ContractOffice
        OperationOffice
        SaleEmpNo
        SaleName
        ProductName
        ChangeType
        ProcessStatus
        StartType
        */
    }
}
