
/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>

/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>

var hasParameter = true;
var isInitAllSection = false;
var lastCondition = null;

$(document).ready(function () {


    initialPage();

    //SetCallerQueiue(["divCTS300_01", "divCTS300_02", "divCTS300_03", "divCTS300_04"]);

//    CallCTS300_01(false);
//    CallCTS300_02(false);
//    CallCTS300_03(false);
//    CallCTS300_04(false);
//    CallCTS300_05(false);


});

function initialPage() {
    SetScreenToDefault();
}

function EnableRegisterSection() {
    SetRegisterCommand(true, registerSection_clicked);
    SetResetCommand(true, resetSection_clicked);
    
    SetConfirmCommand(false, null);
    SetBackCommand(false, null);
}

function EnableConfirmSection() {
    SetRegisterCommand(false, null);
    SetResetCommand(false, null);

    SetConfirmCommand(true, confirmSection_clicked);
    SetBackCommand(true, backSection_clicked);
}

function DisableRegisterSection() {
    SetRegisterCommand(false, null);
    SetResetCommand(false, null);

    SetConfirmCommand(false, null);
    SetBackCommand(false, null);
}

function registerSection_clicked() {
    DisableResetCommand(true);
    DisableRegisterCommand(true);
    var datDate = GetFormDataObject();
    ValidateToRegister(datDate);
}

function ValidateToRegister(obj_05) {
    var obj_01 = GetConditionDataObject();

    var obj = {
        RelevantType: obj_01.RelevantType,
        RelevantCode: obj_01.RelevantCode,
        ContractRelevant: obj_01.ContractRelevant,
        CustomerRelateType: obj_01.CustomerRelateType,
        SiteRelateType: obj_01.SiteRelateType,
        ReceivedDate: obj_05.ReceivedDate,
        ReceivedTime: obj_05.ReceivedTime,
        ReceivedMethod: obj_05.ReceivedMethod,
        ContactName: obj_05.ContactName,
        Department: obj_05.Department,
        IncidentTitle: obj_05.IncidentTitle,
        IncidentType: obj_05.IncidentType,
        ReasonType: obj_05.ReasonType,
        IsSpecialInfo: obj_05.IsSpecialInfo,
        IsImportance: obj_05.IsImportance,
        ReceivedDetail: obj_05.ReceivedDetail,
        DueDateDeadLineType: obj_05.DueDateDeadLineType,
        DueDate_Date: obj_05.DueDate_Date,
        DueDate_Time: obj_05.DueDate_Time,
        Deadline_Date: obj_05.Deadline_Date,
        Deadline_Until: obj_05.Deadline_Until,
        InChargeList: obj_05.InChargeList,
        HaveReasonType: obj_05.HaveReasonType
    };

    call_ajax_method_json("/Contract/CTS300_ValidateData", obj, function (result, controls) {
        if (controls != undefined) {
            ValidateSection_05(controls);
        }

        if (result != undefined) {
            SetToViewMode(true);
            EnableConfirmSection();
        }

        DisableRegisterCommand(false);
        DisableResetCommand(false);
    });
}

function RegisterData(obj_05) {
    var obj_01 = GetConditionDataObject();

    var obj = {
        RelevantType: obj_01.RelevantType,
        RelevantCode: obj_01.RelevantCode,
        ContractRelevant: obj_01.ContractRelevant,
        CustomerRelateType: obj_01.CustomerRelateType,
        SiteRelateType: obj_01.SiteRelateType,
        ReceivedDate: obj_05.ReceivedDate,
        ReceivedTime: obj_05.ReceivedTime,
        ReceivedMethod: obj_05.ReceivedMethod,
        ContactName: obj_05.ContactName,
        Department: obj_05.Department,
        IncidentTitle: obj_05.IncidentTitle,
        IncidentType: obj_05.IncidentType,
        ReasonType: obj_05.ReasonType,
        IsSpecialInfo: obj_05.IsSpecialInfo,
        IsImportance: obj_05.IsImportance,
        ReceivedDetail: obj_05.ReceivedDetail,
        DueDateDeadLineType: obj_05.DueDateDeadLineType,
        DueDate_Date: obj_05.DueDate_Date,
        DueDate_Time: obj_05.DueDate_Time,
        Deadline_Date: obj_05.Deadline_Date,
        Deadline_Until: obj_05.Deadline_Until,
        InChargeList: obj_05.InChargeList,
        HaveReasonType: obj_05.HaveReasonType
    };

    call_ajax_method_json("/Contract/CTS300_RegisIncidentRelevant", obj, function (result, controls) {
        if (controls != undefined) {
            ValidateSection_05(controls);
        }

        if ((result != undefined) && (result != null) && (result.IsCompleted)) {
            SetObject_06(result);

            //SetToViewMode(false);
            SetShow_06(true);
            DisableRegisterSection();
            master_event.ScrollWindow($('#divResultRegister'), false);
        }

        DisableRegisterCommand(false);
        DisableConfirmCommand(false); //Add by Jutarat A. on 27092012
        DisableBackCommand(false); //Add by Jutarat A. on 27092012
    });
}

function resetSection_clicked() {
    DisableRegisterCommand(true);
    DisableResetCommand(true);
    doAskYesNo("Common", "MSG0038", null, function () {
        SetToViewMode(false);
        DisableRegisterSection();
        SetScreenToDefault(false);

        if (lastCondition != null) {
            CTS300_ContractLost = true;

            if (CTS300_gridContractTarget != null) {
                DeleteAllRow(CTS300_gridContractTarget);
            }

            var incRelevantType = GetIncidentRelevantType();
            if (incRelevantType == _incType_Customer) {
                $('#CustomerCode').val(lastCondition);
                RetrieveCustomer_clicked();
            }
            else if (incRelevantType == _incType_Site) {
                $('#SiteCode').val(lastCondition);
                RetrieveSite_clicked();
            }
            else if (incRelevantType == _incType_Project) {
                $('#ProjectCode').val(lastCondition);
                RetrieveProject_clicked();
            }
            else if (incRelevantType == _incType_Contract) {
                $('#UserCode_ContractCode').val(lastCondition);
                RetrieveContract_clicked();
            }
        }

        DisableRegisterCommand(false);
        DisableResetCommand(false);
    },
    function () {
        DisableRegisterCommand(false);
        DisableResetCommand(false);
    });
}

function confirmSection_clicked() {
    DisableConfirmCommand(true);
    DisableBackCommand(true); //Add by Jutarat A. on 27092012

    var datDate = GetFormDataObject();
    RegisterData(datDate);
}

function backSection_clicked() {
    DisableConfirmCommand(true);
    SetToViewMode(false);
    EnableRegisterSection();
    DisableConfirmCommand(false);
}

function SetToViewMode(viewMode) {
    SetViewMode_01(viewMode);
    SetViewMode_02(viewMode);
    SetViewMode_03(viewMode);
    SetViewMode_04(viewMode);
    SetViewMode_05(viewMode);
}

function SetScreenToDefault(isClearRelevantType) {
    CloseWarningDialog();
    DisableRegisterSection();
    SetToViewMode(false);
    SetDefault_01(isClearRelevantType);
    SetDefault_02();
    SetDefault_03();
    SetDefault_04();
    SetDefault_05();
    SetDefault_06();

    LoadParameter();
}

function GetIncidentRelevantType() {
    return $('input[name="IncidentRelevantType"]:checked').val();
}

function LoadParameter() {
    isInitAllSection = IsInit01 && IsInit02 && IsInit03 && IsInit04 && IsInit05 && IsInit06;

    if (hasParameter && isInitAllSection) {
        call_ajax_method_json("/Contract/CTS300_CheckParameter", "", function (result, controls) {
            if ((result != null) && (result.strIncidentRelevantType != null) && (result.strIncidentRelevantCode != null)) {
                hasParameter = true;
                SetObject_01(result);
                hasParameter = false;

                if (result.strIncidentRelevantType == _incType_Customer) {
                    RetrieveCustomer_clicked();
                } else if (result.strIncidentRelevantType == _incType_Site) {
                    RetrieveSite_clicked();
                } else if (result.strIncidentRelevantType == _incType_Project) {
                    RetrieveProject_clicked();
                } else if (result.strIncidentRelevantType == _incType_Contract) {
                    RetrieveContract_clicked();
                }
            } else {
                hasParameter = false;
            }
        });

    }
}

function doAlert(moduleCode, msgCode, paramObj) {
    var obj = {
        module: moduleCode,
        code: msgCode,
        param: paramObj
    };

    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        OpenWarningDialog(result.Message, result.Message, null);
    });
}

function doAlertWithMessage(msgCode, msgText) {
    //OpenWarningDialog(msgText);
    hasAlert = true;
    alertMsg = msgText;
}

function doAskYesNo(moduleCode, msgCode, paramObj, yesFunc, noFunc) {
    var obj = {
        module: moduleCode,
        code: msgCode,
        param: paramObj
    };

    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        if ((yesFunc != null) && (typeof (yesFunc) != "function")) {
            yesFunc = null;
        }

        if ((noFunc != null) && (typeof (noFunc) != "function")) {
            noFunc = null;
        }

        OpenYesNoMessageDialog(result.Code, result.Message, yesFunc, noFunc);
    });
}

//function CallCTS300_01(bClear, func) {
//    /// <summary>Method to create Incident relevant information section</summary>
//    /// <param name="clear" type="bool">Flag to clear section</param>
//    CallHtmlSection("#divCTS300_01", "Contract", "CTS300_01", "", bClear, func);
//}

//function CallCTS300_02(bClear, func) {
//    /// <summary>Method to create Project information section</summary>
//    /// <param name="clear" type="bool">Flag to clear section</param>
//    CallHtmlSection("#divCTS300_02", "Contract", "CTS300_02", "", bClear, func);
//}

//function CallCTS300_03(bClear, func) {
//    /// <summary>Method to create Customer relevant information section</summary>
//    /// <param name="clear" type="bool">Flag to clear section</param>
//    CallHtmlSection("#divCTS300_03", "Contract", "CTS300_03", "", bClear, func);
//}

//function CallCTS300_04(bClear, func) {
//    /// <summary>Method to create Site information section</summary>
//    /// <param name="clear" type="bool">Flag to clear section</param>
//    CallHtmlSection("#divCTS300_04", "Contract", "CTS300_04", "", bClear, func);
//}

//function CallCTS300_05(bClear, func) {
//    /// <summary>Method to create New incedent field information section</summary>
//    /// <param name="clear" type="bool">Flag to clear section</param>
//    CallHtmlSection("#divCTS300_05", "Contract", "CTS300_05", "", bClear, func);
//}

