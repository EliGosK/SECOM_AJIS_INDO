using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.Presentation.Contract.Models.MetaData;
using SECOM_AJIS.DataEntity.Master;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web;


namespace SECOM_AJIS.Presentation.Contract.Models
{
    /// <summary>
    /// DO of session parameter screen CTS230
    /// </summary>
    public class CTS230_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        public CTS230_InitialData InitialData { get; set; }
    }
    /// <summary>
    /// DO for retrieve customer
    /// </summary>
    public class CTS230RetrieveCustomer
    {
        public string CustCode { get; set; }
        public int CustType { get; set; }
    }
    /// <summary>
    /// DO for validate employee
    /// </summary>
    public class CTS230_doValidateEmpNo
    {
        public string EmpNo { get; set; }
        public string controls { get; set; }
        public bool isError { get { return EmpNo != null; } }
    }
    /// <summary>
    /// DO for initial data screen CTS230
    /// </summary>
    public class CTS230_InitialData
    {
        public doCustomer doCustomer { get; set; }
        public doRegisterProjectData doRegisterData { get; set; }
        public int ObjectTypeID { get; set; }

        public tbt_ProjectPurchaserCustomer doProjectPurchaserData
        {
            get
            {
                tbt_ProjectPurchaserCustomer ProjPurchaser = new tbt_ProjectPurchaserCustomer();

                ProjPurchaser = (CommonUtil.CloneObject<doCustomer, tbt_ProjectPurchaserCustomer>(this.doCustomer));
                return ProjPurchaser;

            }
            set { doProjectPurchaserData = value; }
        }
        public void SetObjectData(CTS230_InitialData initialData)
        {
            this.doProjectPurchaserData = initialData.doProjectPurchaserData;
        }
        public enum INITIAL_OBJECT
        {
            INITIAL_DATA = 0,
            CONTRACT_TARGET_DATA,
            REAL_CUSTOMER_DATA,
            QUOTATION_SITE_DATA
        }
        public INITIAL_OBJECT ObjectType
        {
            get
            {
                return (INITIAL_OBJECT)Enum.ToObject(typeof(INITIAL_OBJECT), this.ObjectTypeID);
            }
            set
            {
                this.ObjectTypeID = (int)value;
            }
        }
        public object GetObjectData(INITIAL_OBJECT ObjectType)
        {

            return null;
        }
    }
    /// <summary>
    /// DO of support staff
    /// </summary>
    public class SupportStaff
    {
        public string EmpFullName { get; set; }
        public string BelongingOfficeDepart { get; set; }

    }
    /// <summary>
    /// DO of support staff for show
    /// </summary>
    [MetadataType(typeof(CTS230_SupportStaff_MetaData))]
    public class CTS230_SupportStaff
    {
        public string StaffCode { get; set; }
        public string Remark { get; set; }
        public string[] lstStaffCode { get; set; }
    }
    /// <summary>
    /// DO of system product
    /// </summary>
    [MetadataType(typeof(CTS230_SystemProduct_Meta))]
    public class CTS230_SystemProduct
    {
        public string SystemProductName { get; set; }
        public string[] lstSystemProductName { get; set; }
    }
    /// <summary>
    /// DO of attach files
    /// </summary>
    [MetadataType(typeof(CTS230_AttachFile_MetaData))]
    public class CTS230_AttachFile
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }

        public string[] lstFileName { get; set; }
    }
    /// <summary>
    /// DO of Project related company/person
    /// </summary>
    [MetadataType(typeof(CTS230_AddProjectRelate_MetaData))]
    public class CTS230_ProjectRelateCompanyPerson
    {
        public string CompanyName { get; set; }
        public string Name { get; set; }
        public string ContractTelNo { get; set; }
        public string Remark { get; set; }
        public string[] lstCompanyName { get; set; }
        public string[] lstCompanyName_rowID { get; set; }
        public string row_id { get; set; }
    }
    /// <summary>
    /// DO of instrument
    /// </summary>
    [MetadataType(typeof(CTS230_AddInstrumentData_MetaData))]
    public class CTS230_AddInstrumentData
    {
        public string InstrumentCode { get; set; }
        public string InstrumentName { get; set; }
        public int? InstrumentQty { get; set; }
        public doInstrumentData dtNewInstrument { get; set; }

        public string[] lstInstrumentCode { get; set; }
    }
    [MetadataType(typeof(CTS230_ProjectPurchaseCustomer_Meta))]
    public class CTS230_ProjectPurchaseCustomer : tbt_ProjectPurchaserCustomer
    {

    }
}


namespace SECOM_AJIS.Presentation.Contract.Models.MetaData
{
    public class CTS230_ProjectPurchaseCustomer_Meta
    {
        public string CustCode { get; set; }
        [CodeNullOtherNotNull("CustCode", Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS230",
                        Parameter = "headerProjectPurchaser")]
        public string CustNameEN { get; set; }
        [CodeNullOtherNotNull("CustCode", Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS230",
                        Parameter = "headerProjectPurchaser")]
        public string CustNameLC { get; set; }
        [CodeNullOtherNotNull("CustCode", Controller = MessageUtil.MODULE_CONTRACT,
                              Screen = "CTS230",
                              Parameter = "headerProjectPurchaser")]
        public string CustTypeCode { get; set; }



    }
    public class CTS230RetrieveCustomer_Meta
    {          //Valid attribute
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS230",
                        Parameter = "lblCustomerCode",
                        ControlName = "CPCustCodeShort")]
        public string CustCode { get; set; }

    }
    public class CTS230_SystemProduct_Meta
    {          //Valid attribute
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS230",
                        Parameter = "lblSystemProductName",
                        ControlName = "SysProductName")]
        public string SystemProductName { get; set; }

    }
    public class CTS230_AddInstrumentData_MetaData
    {
        [NotNullOrEmpty(Module = MessageUtil.MODULE_COMMON, ControlName = "InstrumentCode", MessageCode = MessageUtil.MessageList.MSG0081, Screen = "CTS230", Parameter = "lblInstrumentCode")]
        public string InstrumentCode { get; set; }

    }
    public class CTS230_AttachFile_MetaData
    {
        [NotNullOrEmpty(ControlName = "AttachDocPath")]
        public string FilePath { get; set; }
        [NotNullOrEmpty(ControlName = "Docname")]
        public string FileName { get; set; }

    }
    public class CTS230_AddProjectRelate_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS230",
                Parameter = "lblCompanyName",
                ControlName = "ProjRelCompName")]
        public string CompanyName { get; set; }
    }

    public class CTS230_SupportStaff_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS230",
                Parameter = "lblSupportStaff",
               ControlName = "SupportStaffCode")]
        public string StaffCode { get; set; }
    }
}

