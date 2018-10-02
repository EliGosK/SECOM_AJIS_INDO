/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>

$(document).ready(function () {
    InitialControlCTS180_03();
    InitialEventCTS180_03();
    VisibleDocumentSectionCTS180_03(false);
});

function InitialEventCTS180_03() {
    $("#CM_UseSignaturePicture").click(function () {
        if ($(this).prop("checked") == true) {
            $("#CM_EmployeeName").val("");
            $("#CM_EmployeePosition").val("");

            $("#CM_EmployeeName").attr("disabled", true);
            $("#CM_EmployeePosition").attr("disabled", true);
        }
        else {
            $("#CM_EmployeeName").attr("disabled", false);
            $("#CM_EmployeePosition").attr("disabled", false);
        }
    });
}

function InitialControlCTS180_03() {
    $("#CM_ChangeDate").InitialDate();
    $("#CM_OldContractFee").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#CM_NewContractFee").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#CM_ChangeContent").SetMaxLengthTextArea(100);
}

function InitialInputDataCTS180_03() {
    $("#divChangeMemo").clearForm();
}

function BindDocumentDataCTS180_03(result) {
    if (result != undefined && result.length == 2) {
        var contDoc = result[0];
        var chgMemoNtc = result[1];

        $("#CM_ContractCodeShort").val(contDoc.ContractCodeShort);
        $("#CM_ContractTargetName").val(contDoc.ContractTargetNameEN);
        $("#CM_RealCustomerName").val(contDoc.RealCustomerNameEN);
        $("#CM_SiteNameEN").val(contDoc.SiteNameEN);
        $("#CM_SiteNameLC").val(contDoc.SiteNameLC);
        $("#CM_SiteAddress").val(contDoc.SiteAddressEN);
        $("#CM_ChangeDate").val(ConvertDateToTextFormat(ConvertDateObject(chgMemoNtc.EffectiveDate)));

        if (contDoc.DocumentCode == $("#C_DOCUMENT_CODE_CHANGE_MEMO").val()) {
            $("#CM_OldContractFee").SetNumericCurrency(chgMemoNtc.OldContractFeeCurrencyType);
            $("#CM_OldContractFee").val(chgMemoNtc.OldContractFeeForShow);
            $("#CM_NewContractFee").SetNumericCurrency(chgMemoNtc.NewContractFeeCurrencyType);
            $("#CM_NewContractFee").val(chgMemoNtc.NewContractFeeForShow);

            $("#CM_CustomerSignatureName").val(chgMemoNtc.CustomerSignatureName);
        }

        $("#CM_ChangeContent").val(chgMemoNtc.ChangeContent);

        //$("#CM_UseSignaturePicture").attr("checked", contDoc.SECOMSignatureFlag);
        //$("#CM_EmployeeName").val(contDoc.EmpName);
        //$("#CM_EmployeePosition").val(contDoc.EmpPosition);
        $("#CM_UseSignaturePicture").attr("checked", contDoc.SECOMSignatureFlag);
        if (contDoc.SECOMSignatureFlag == true) {
            $("#CM_EmployeeName").attr("disabled", true);
            $("#CM_EmployeePosition").attr("disabled", true);
        }
        else {
            $("#CM_EmployeeName").val(contDoc.EmpName);
            $("#CM_EmployeePosition").val(contDoc.EmpPosition);
        }
    }
}

function SetDocumentSectionModeCTS180_03(isView) {
    $("#divChangeMemo").SetViewMode(isView);
}

function VisibleDocumentSectionCTS180_03(isVisible) {
    if (isVisible) {
        $("#divChangeMemo").show();
    }
    else {
        $("#divChangeMemo").hide();
    }
}

function SetRegisterStateCTS180_03(docCode) {
    $("#CM_ContractCodeShort").attr("readonly", true);
    $("#CM_ContractTargetName").attr("readonly", true);
    $("#CM_OldContractFee").attr("readonly", true);
    $("#CM_NewContractFee").attr("readonly", true);
    $("#CM_CustomerSignatureName").attr("readonly", docCode == $("#C_DOCUMENT_CODE_CHANGE_NOTICE").val());
}

function GetDocumentDataCTS180_03() {
    var objCTS180_03 = {
        ContractCodeShort: $("#CM_ContractCodeShort").val(),
        ContractTargetNameLC: $("#CM_ContractTargetName").val(),
        RealCustomerNameLC: $("#CM_RealCustomerName").val(),
        SiteNameEN: $("#CM_SiteNameEN").val(),
        SiteNameLC: $("#CM_SiteNameLC").val(),
        SiteAddressLC: $("#CM_SiteAddress").val(),
        EffectiveDate: $("#CM_ChangeDate").val(),
        OldContractFeeCurrencyType: $("#CM_OldContractFee").NumericCurrencyValue(),
        OldContractFee: $("#CM_OldContractFee").NumericValue(),
        NewContractFeeCurrencyType: $("#CM_NewContractFee").NumericCurrencyValue(),
        NewContractFee: $("#CM_NewContractFee").NumericValue(),
        CustomerSignatureName: $("#CM_CustomerSignatureName").val(),
        ChangeContent: $("#CM_ChangeContent").val(),
        SECOMSignatureFlag: $("#CM_UseSignaturePicture").prop("checked"),
        EmpName: $("#CM_EmployeeName").val(),
        EmpPosition: $("#CM_EmployeePosition").val()
    };

    return objCTS180_03;
}