var processType = "";

$(document).ready(function () {
    InitialControlPropertyCTS220_01();
}); /// <reference path="CTS220_01.js" />


function InitialControlPropertyCTS220_01() {
    if ($("#PermissionAdd").val() == "False") {
        $("#rdoInsertSecurityOccurence").attr("disabled", true);
    }

    if ($("#PermissionEdit").val() == "False") {
        $("#rdoCorrectSecurityData").attr("disabled", true);
    }

    if ($("#PermissionDel").val() == "False") {
        $("#rdoDeleteOperatedOccurence").attr("disabled", true);
    }

    $("#btnProcess").click(function () { SelectProcessClick(); });
    $("#rdoCorrectSecurityData").click(function () { ChangeProcessType(); })
    $("#rdoInsertSecurityOccurence").click(function () { ChangeProcessType(); })
    $("#rdoDeleteOperatedOccurence").click(function () { ChangeProcessType(); })

    $("#divContractBasicSection").hide();
    $("#divProductSection").hide();
    $("#divChangeContractSection").hide();
    $("#divInsuranceSection").hide();
    $("#divContractDocumentSection").hide();
    $("#divFutureDateSection").hide();
    $("#divQuotationSection").hide();
    $("#divInstallationSection").hide();
}

function ChangeProcessType() {

    if ($("#rdoCorrectSecurityData").prop("checked") == true || $("#rdoInsertSecurityOccurence").prop("checked") == true) {
        $("#Occurence").attr("disabled", false);
        $("#Occurence").attr("readonly", false);
    }

    if ($("#rdoDeleteOperatedOccurence").prop("checked") == true) {
        $("#Occurence").attr("disabled", true);
        $("#Occurence").attr("readonly", true);
        $("#Occurence").val("");
    }
}

function SelectProcessClick() {

    var obj = "";
    if ($("#rdoCorrectSecurityData").prop("checked") == true)
        processType = "Correct";

    if ($("#rdoInsertSecurityOccurence").prop("checked") == true)
        processType = "Insert";

    if ($("#rdoDeleteOperatedOccurence").prop("checked") == true)
        processType = "Delete";

    if (processType == "Delete") {
        obj = { ContractCode: $("#ContractCode").val(), OCC: "none", ProcessType: processType }
    }
    else {
        obj = { ContractCode: $("#ContractCode").val(), OCC: $("#Occurence").val(), ProcessType: processType }
    }

    call_ajax_method_json('/Contract/SelectProcessClick_CTS220', obj,
        function (result, controls) {
            if (controls != undefined) {
                VaridateCtrl(["Occurence"], controls);
                return;
            }
            else if (result != false) {
                InitialSelectProcessMode();
                InitialDataMode();

                if (typeof (result) == "string") {
                    $("#Occurence").val(result);
                }
            }
        });

}

function InitialDataMode() {
    var step1 = ["CTS220_02"];

    var step2 = [
                "CTS220_03",
                "CTS220_04",
                "CTS220_05",
                "CTS220_06",
                "CTS220_07",
                "CTS220_08",
                "CTS220_09"
               ];

    InitModeSection([step2, step1]);
}

function InitialSelectProcessMode() {
    $("#divSpecifyProcessTypeSection").show();
    $("#divSpecifyProcessTypeSection").attr('disabled', false);
    $("#divSpecifyProcessTypeSection").attr('readonly', false);
    $('#divSpecifyProcessTypeSection :input').attr('disabled', false);
    $('#divSpecifyProcessTypeSection :input').attr('readonly', false);
    $('#ContractCode').attr('readonly', true);

    $("#divContractBasicSection").hide();
    $("#divProductSection").hide();
    $("#divChangeContractSection").hide();
    $("#divInsuranceSection").hide();
    $("#divContractDocumentSection").hide();
    $("#divFutureDateSection").hide();
    $("#divQuotationSection").hide();
    $("#divInstallationSection").hide();
}
