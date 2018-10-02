/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>

$(document).ready(function () {
    InitialControlCTS180_06();
    InitialEventCTS180_06();
    VisibleDocumentSectionCTS180_06(false);
});

function InitialEventCTS180_06() {
    $("#CHF_UseSignaturePicture").click(function () {
        if ($(this).prop("checked") == true) {
            $("#CHF_EmployeeName").val("");
            $("#CHF_EmployeePosition").val("");

            $("#CHF_EmployeeName").attr("disabled", true);
            $("#CHF_EmployeePosition").attr("disabled", true);
        }
        else {
            $("#CHF_EmployeeName").attr("disabled", false);
            $("#CHF_EmployeePosition").attr("disabled", false);
        }
    });
}

function InitialControlCTS180_06() {
    $("#CHF_OldContractFee").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#CHF_NewContractFee").BindNumericBox(12, 2, 0, 999999999999.99);

    $("#CHF_ChangeContractFeeDate").InitialDate();
    $("#CHF_ReturnToOriginalFeeDate").InitialDate();
}

function InitialInputDataCTS180_06() {
    $("#divChangeFeeMemo").clearForm();
}

function BindDocumentDataCTS180_06(result) {
    if (result != undefined && result.length == 2) {
        var contDoc = result[0];
        var chgFee = result[1];

        $("#CHF_ContractCodeShort").val(contDoc.ContractCodeShort);
        $("#CHF_ContractTargetName").val(contDoc.ContractTargetNameEN);
        $("#CHF_RealCustomerName").val(contDoc.RealCustomerNameEN);
        $("#CHF_SiteNameEN").val(contDoc.SiteNameEN);
        $("#CHF_SiteNameLC").val(contDoc.SiteNameLC);
        $("#CHF_SiteAddress").val(contDoc.SiteAddressEN);

        $("#CHF_OldContractFee").SetNumericCurrency(chgFee.OldContractFeeCurrencyType);
        $("#CHF_OldContractFee").val(chgFee.OldContractFeeForShow);
        $("#CHF_NewContractFee").SetNumericCurrency(chgFee.NewContractFeeCurrencyType);
        $("#CHF_NewContractFee").val(chgFee.NewContractFeeForShow);

        $("#CHF_ChangeContractFeeDate").val(ConvertDateToTextFormat(ConvertDateObject(chgFee.ChangeContractFeeDate)));
        $("#CHF_ReturnToOriginalFeeDate").val(ConvertDateToTextFormat(ConvertDateObject(chgFee.ReturnToOriginalFeeDate)));
        $("#CHF_CustomerSignatureName").val(chgFee.CustomerSignatureName);

        $("#CHF_UseSignaturePicture").attr("checked", contDoc.SECOMSignatureFlag);
        if (contDoc.SECOMSignatureFlag == true) {
            $("#CHF_EmployeeName").attr("disabled", true);
            $("#CHF_EmployeePosition").attr("disabled", true);
        }
        else {
            $("#CHF_EmployeeName").val(contDoc.EmpName);
            $("#CHF_EmployeePosition").val(contDoc.EmpPosition);
        }
    }
}

function SetDocumentSectionModeCTS180_06(isView) {
    $("#divChangeFeeMemo").SetViewMode(isView);
}

function VisibleDocumentSectionCTS180_06(isVisible) {
    if (isVisible) {
        $("#divChangeFeeMemo").show();
    }
    else {
        $("#divChangeFeeMemo").hide();
    }
}

function SetRegisterStateCTS180_06() {
    $("#CHF_ContractCodeShort").attr("readonly", true);
    $("#CHF_ContractTargetName").attr("readonly", true);

    $("#CHF_OldContractFee").attr("readonly", true);
    $("#CHF_OldContractFee").NumericCurrency().attr("disabled", true);
}

function GetDocumentDataCTS180_06() {
    var objCTS180_06 = {
        ContractCodeShort: $("#CHF_ContractCodeShort").val(),
        ContractTargetNameLC: $("#CHF_ContractTargetName").val(),
        RealCustomerNameLC: $("#CHF_RealCustomerName").val(),
        SiteNameEN: $("#CHF_SiteNameEN").val(),
        SiteNameLC: $("#CHF_SiteNameLC").val(),
        SiteAddressLC: $("#CHF_SiteAddress").val(),

        OldContractFeeCurrencyType: $("#CHF_OldContractFee").NumericCurrencyValue(),
        OldContractFee: $("#CHF_OldContractFee").NumericValue(),
        NewContractFeeCurrencyType: $("#CHF_NewContractFee").NumericCurrencyValue(),
        NewContractFee: $("#CHF_NewContractFee").NumericValue(),

        ChangeContractFeeDate: $("#CHF_ChangeContractFeeDate").val(),
        ReturnToOriginalFeeDate: $("#CHF_ReturnToOriginalFeeDate").val(),
        CustomerSignatureName: $("#CHF_CustomerSignatureName").val(),
        SECOMSignatureFlag: $("#CHF_UseSignaturePicture").prop("checked"),
        EmpName: $("#CHF_EmployeeName").val(),
        EmpPosition: $("#CHF_EmployeePosition").val()
    };

    return objCTS180_06;
}