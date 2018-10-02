using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.Common.Models
{
    /// <summary>
    /// DO for correct all user's information
    /// </summary>
    [Serializable]
    public class dsTransDataModel
    {
        private TransHeaderDo _dtTransHeader;
        private Dictionary<string, UserPermissionDataDo> _dtUserPermissionData;
        private List<OfficeDataDo> _dtOfficeData;
        private List<UserBelongingData> _dtUserBelongingData;
        private OperationDataDo _dtOperationData;
        private UserDataDo _dtUserData;
        private CommonSearchDo _dtCommonSearch;


        // Add by Narupon : 1/Jan/2012
        private List<MenuName> _dtMenuNameList;

        // Add by Narupon : 1/Jan/2012
        public List<MenuName> dtMenuNameList
        {
            get { return this._dtMenuNameList; }
            set { this._dtMenuNameList = value; }
        
        }

        public TransHeaderDo dtTransHeader
        {
            get { return this._dtTransHeader; }
            set { this._dtTransHeader = value; }
        }
        public Dictionary<string,UserPermissionDataDo> dtUserPermissionData
        {
            get { return this._dtUserPermissionData; }
            set { this._dtUserPermissionData = value; }
        }
        public List<OfficeDataDo> dtOfficeData
        {
            get { return this._dtOfficeData; }
            set { this._dtOfficeData = value; }
        }
        public OperationDataDo dtOperationData
        {
            get { return this._dtOperationData; }
            set { this._dtOperationData = value; }
        }
        public UserDataDo dtUserData
        {
            get { return this._dtUserData; }
            set { this._dtUserData = value; }
        }
        public List<UserBelongingData> dtUserBelongingData
        {
            get { return this._dtUserBelongingData; }
            set { this._dtUserBelongingData = value; }
        }

        public bool ContainsPermission(string ObjectID, string FunctionID)
        {
            return dtUserPermissionData.ContainsKey(GenerateKey(ObjectID, FunctionID));
        }
        public bool ContainsPermission(string ObjectID)
        {
              return dtUserPermissionData.ContainsKey(GenerateKey(ObjectID, SECOM_AJIS.Common.Util.ConstantValue.FunctionID.C_FUNC_ID_OPERATE));
        }

        public string GenerateKey(string ObjectID,string FunctionID)
        {
            return ObjectID + "." + FunctionID;  
        }

        public CommonSearchDo dtCommonSearch
        {
            get { return this._dtCommonSearch; }
            set { this._dtCommonSearch = value; }
        }
    
    }
    /// <summary>
    /// DO for beloging user
    /// </summary>
    public class UserBelongingData
    {
        private string strOfficeCode; public string OfficeCode { get { return this.strOfficeCode; } set { this.strOfficeCode = value; } }
        private string strDepartmentCode; public string DepartmentCode { get { return this.strDepartmentCode; } set { this.strDepartmentCode = value; } }
        private string strPositionCode; public string PositionCode { get { return this.strPositionCode; } set { this.strPositionCode = value; } }
        //Create  Constructor :Nattapong N.
        public UserBelongingData(string nOfficeCode, string nDepartmentCode, string nPositionCode)
        {
            this.strOfficeCode = nOfficeCode;
            this.strDepartmentCode = nDepartmentCode;
            this.strPositionCode = nPositionCode;
        }
    }
    /// <summary>
    /// DO for transaction header
    /// </summary>
    [Serializable]
    public class TransHeaderDo
    {
        private string _strScreenID;
        private string _strLanguage;
        private string _strMainOfficeCode;

        public string ScreenID
        {
            get { return this._strScreenID; }
            set { this._strScreenID = value; }
        }
        public string Language
        {
            get { return this._strLanguage; }
            set { this._strLanguage = value; }
        }
        public string MainOfficeCode
        {
            get { return this._strMainOfficeCode; }
            set { this._strMainOfficeCode = value; }
        }
    }
    /// <summary>
    /// DO for user's permission
    /// </summary>
    [Serializable]
    public class UserPermissionDataDo
    {
        public UserPermissionDataDo()
        {
        }
        public UserPermissionDataDo(string nObjectID, int nFunctionID, string nObjectType)
        {
            this.ObjectID = nObjectID;
            this.FunctionID = nFunctionID;
            this.ObjectType = nObjectType;

        }
        private string strObjectID;
        private int iFunctionID;
        private string strObjectType;


        public string ObjectID
        {
            get { return this.strObjectID; }
            set { this.strObjectID = value; }
        }
        public int FunctionID
        {
            get { return this.iFunctionID; }
            set { this.iFunctionID = value; }
        }
        public string ObjectType
        {
            get { return this.strObjectType; }
            set { this.strObjectType = value; }
        }
    }
    /// <summary>
    /// DO for office information
    /// </summary>
    [Serializable]
    public class OfficeDataDo
    {
        
        private string strOfficeCode;
        private string strOfficeName;
        private string strOfficeNameEN;
        private string strOfficeNameJP;
        private string strOfficeNameLC;
        private string strFunctionLogistic;
        private string strFunctionSecurity;
        private string strFunctionSale;
        private string strFunctionAdmin;
        private string strFunctionBilling;
        private string strFunctionQuotation;
        private string strFunctionDebtTracing;
        

        public string OfficeCode
        {
            get { return this.strOfficeCode; }
            set { this.strOfficeCode = value; }
        }
         [LanguageMapping]
        public string OfficeName
        {
            get { return this.strOfficeName; }
            set { this.strOfficeName = value; }
        }
        public string OfficeNameEN
        {
            get { return this.strOfficeNameEN; }
            set { this.strOfficeNameEN = value; }
        }
        public string OfficeNameJP
        {
            get { return this.strOfficeNameJP; }
            set { this.strOfficeNameJP = value; }
        }
        public string OfficeNameLC
        {
            get { return this.strOfficeNameLC; }
            set { this.strOfficeNameLC = value; }
        }
        public string FunctionLogistic
        {
            get { return this.strFunctionLogistic; }
            set { this.strFunctionLogistic = value; }
        }
        public string FunctionSecurity
        {
            get { return this.strFunctionSecurity; }
            set { this.strFunctionSecurity = value; }
        }
        public string FunctionSale
        {
            get { return this.strFunctionSale; }
            set { this.strFunctionSale = value; }
        }
        public string FunctionAdmin
        {
            get { return this.strFunctionAdmin; }
            set { this.strFunctionAdmin = value; }
        }
        public string FunctionBilling
        {
            get { return this.strFunctionBilling; }
            set { this.strFunctionBilling = value; }
        }
        public string FunctionQuatation
        {
            get { return this.strFunctionQuotation; }
            set { this.strFunctionQuotation = value; }
        }
        public string FunctionDebtTracing
        {
            get { return this.strFunctionDebtTracing; }
            set { this.strFunctionDebtTracing = value; }
        }
        public string OfficeCodeName
        {
            get
            {
                return Common.Util.CommonUtil.TextCodeName(this.OfficeCode, this.OfficeName);
            }
        }
    }
    /// <summary>
    /// DO for operation information
    /// </summary>
    [Serializable]
    public class OperationDataDo
    {
        private DateTime dtProcessDateTime;
        private int iTransactionType;
        private string strGUID;

        public DateTime ProcessDateTime
        {
            get { return this.dtProcessDateTime; }
            set { this.dtProcessDateTime = value; }
        }
        public int TransactionType
        {
            get { return this.iTransactionType; }
            set { this.iTransactionType = value; }
        }
        public string GUID
        {
            get { return this.strGUID; }
            set { this.strGUID = value; }
        }
    }
    /// <summary>
    /// DO for user information
    /// </summary>
    [Serializable]
    public class UserDataDo
    {
        private string strEmpNo; public string EmpNo { get { return this.strEmpNo; } set { this.strEmpNo = value; } }
        private string strEmpFirstNameEN; public string EmpFirstNameEN { get { return this.strEmpFirstNameEN; } set { this.strEmpFirstNameEN = value; } }
        private string strEmpLastNameEN; public string EmpLastNameEN { get { return this.strEmpLastNameEN; } set { this.strEmpLastNameEN = value; } }
        private string strEmpFirstNameLC; public string EmpFirstNameLC { get { return this.strEmpFirstNameLC; } set { this.strEmpFirstNameLC = value; } }
        private string strEmpLastNameLC; public string EmpLastNameLC { get { return this.strEmpLastNameLC; } set { this.strEmpLastNameLC = value; } }
        private string strEmailAddress; public string EmailAddress { get { return this.strEmailAddress; } set { this.strEmailAddress = value; } }
        private string strMainOfficeCode; public string MainOfficeCode { get { return this.strMainOfficeCode; } set { this.strMainOfficeCode = value; } }
        private string strMainDepartmentCode; public string MainDepartmentCode { get { return this.strMainDepartmentCode; } set { this.strMainDepartmentCode = value; } }

        public string EmpFullName
        {
            get
            {
                CommonUtil.LANGUAGE_LIST lang = CommonUtil.CurrentLanguage(false);
                if (lang == CommonUtil.LANGUAGE_LIST.LANGUAGE_3)
                    return CommonUtil.TextFullName(this.EmpFirstNameLC, this.EmpLastNameLC);
                else
                    return CommonUtil.TextFullName(this.EmpFirstNameEN, this.EmpLastNameEN);
            }
        }

    }
    /// <summary>
    /// DO for search information
    /// </summary>
    [Serializable]
    public class CommonSearchDo
    {
        private string _ContractCode;

        public string ContractCode
        {
            get { return this._ContractCode; }
            set {
                    this._ContractCode = value;
                    if (!CommonUtil.IsNullOrEmpty(value))
                    {
                        this._ProjectCode = null; 
                    }
                }
        }

        private string _ProjectCode;

        public string ProjectCode
        {
            get { return this._ProjectCode; }
            set { 
                    this._ProjectCode = value;
                    if (!CommonUtil.IsNullOrEmpty(value))
                    {
                        this._ContractCode = null;
                    }
                }
        }

        public string InvoiceNo { get; set; }
    }

}