/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>

$(document).ready(function () {
    InitialControlCTS180_04();
    InitialEventCTS180_04();
    VisibleDocumentSectionCTS180_04(false);
});

function InitialEventCTS180_04() {
    $("#CF_UseSignaturePicture").click(function () {
        if ($(this).prop("checked") == true) {
            $("#CF_EmployeeName").val("");
            $("#CF_EmployeePosition").val("");

            $("#CF_EmployeeName").attr("disabled", true);
            $("#CF_EmployeePosition").attr("disabled", true);
        }
        else {
            $("#CF_EmployeeName").attr("disabled", false);
            $("#CF_EmployeePosition").attr("disabled", false);
        }
    });
}

function InitialControlCTS180_04() {
    $("#CF_RealInvestigationDate").InitialDate();
}

function InitialInputDataCTS180_04() {
    $("#divConfirmCurrent").clearForm();
}

function BindDocumentDataCTS180_04(result) {
    if (result != undefined && result.length == 2) {
        var contDoc = result[0];
        var cfrCurrInst = result[1];

        $("#CF_ContractCodeShort").val(contDoc.ContractCodeShort);
        $("#CF_ContractTargetName").val(contDoc.ContractTargetNameEN);
        $("#CF_RealCustomerName").val(contDoc.RealCustomerNameEN);
        $("#CF_SiteNameEN").val(contDoc.SiteNameEN);
        $("#CF_SiteNameLC").val(contDoc.SiteNameLC);
        $("#CF_SiteAddress").val(contDoc.SiteAddressEN);
        $("#CF_RealInvestigationDate").val(ConvertDateToTextFormat(ConvertDateObject(cfrCurrInst.RealInvestigationDate)));
        $("#CF_CustomerSignatureName").val(cfrCurrInst.CustomerSignatureName);

        //$("#CF_UseSignaturePicture").attr("checked", contDoc.SECOMSignatureFlag);
        //$("#CF_EmployeeName").val(contDoc.EmpName);
        //$("#CF_EmployeePosition").val(contDoc.EmpPosition);
        $("#CF_UseSignaturePicture").attr("checked", contDoc.SECOMSignatureFlag);
        if (contDoc.SECOMSignatureFlag == true) {
            $("#CF_EmployeeName").attr("disabled", true);
            $("#CF_EmployeePosition").attr("disabled", true);
        }
        else {
            $("#CF_EmployeeName").val(contDoc.EmpName);
            $("#CF_EmployeePosition").val(contDoc.EmpPosition);
        }
    }
}

function SetDocumentSectionModeCTS180_04(isView) {
    $("#divConfirmCurrent").SetViewMode(isView);
}

function VisibleDocumentSectionCTS180_04(isVisible) {
    if (isVisible) {
        $("#divConfirmCurrent").show();
    }
    else {
        $("#divConfirmCurrent").hide();
    }
}

function SetRegisterStateCTS180_04() {
    $("#CF_ContractCodeShort").attr("readonly", true);
    $("#CF_ContractTargetName").attr("readonly", true);
}

function GetDocumentDataCTS180_04() {
    var objCTS180_04 = {
        ContractCodeShort: $("#CF_ContractCodeShort").val(),
        ContractTargetNameLC: $("#CF_ContractTargetName").val(),
        RealCustomerNameLC: $("#CF_RealCustomerName").val(),
        SiteNameEN: $("#CF_SiteNameEN").val(),
        SiteNameLC: $("#CF_SiteNameLC").val(),
        SiteAddressLC: $("#CF_SiteAddress").val(),
        RealInvestigationDate: $("#CF_RealInvestigationDate").val(),
        CustomerSignatureName: $("#CF_CustomerSignatureName").val(),
        SECOMSignatureFlag: $("#CF_UseSignaturePicture").prop("checked"),
        EmpName: $("#CF_EmployeeName").val(),
        EmpPosition: $("#CF_EmployeePosition").val()
    };

    return objCTS180_04;
}