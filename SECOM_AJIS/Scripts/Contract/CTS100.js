/*--- Main ---*/
$(document).ready(function () {
    $("#divLastChangeType").hide();

    InitialControl();
    InitialEvent();
    SetInitialState();
});

function RegisterCommandControl() {
    SetRegisterCommand(true, command_register_click);
    SetResetCommand(true, command_reset_click);
    SetConfirmCommand(false, null);
    SetBackCommand(false, null);
}

function ConfirmCommandControl() {
    SetRegisterCommand(false, null);
    SetResetCommand(false, null);
    SetConfirmCommand(true, command_confirm_click);
    SetBackCommand(true, command_back_click);
}

function HideCommandControl() {
    SetRegisterCommand(false, null);
    SetResetCommand(false, null);
    SetConfirmCommand(false, null);
    SetBackCommand(false, null);
}

function InitialControl() {
    $("#dpStopDate").InitialDate();
    $("#dpOperationDate").InitialDate();
    $("#txtStopFee").BindNumericBox(12, 2, 0, 999999999999.99);
}

function InitialEvent() {
    InitialTrimTextEvent([
        "txtApproveNo1",
        "txtApproveNo2"
    ]);

    $("#btnRetrieve").click(retrieve_button_click);
    $("#btnClear").click(function () {
        clear_button_click(true);
    });
}

function SetInitialState() {
    $("#divStopService").clearForm();

    //$("#dpStopDate").val(ConvertDateToTextFormat($("#CurrentDate").val()));
    var currentDate = new Date();
    $("#dpStopDate").datepicker("setDate", currentDate);
    $("#dpStopDate").val(ConvertDateToTextFormat(ConvertDateObject(currentDate)));

    $("#txtStopFee").val("0.00");

    if ($("#ContractCodeShort").val() == '') {
        $("#divContractBasicInfo").hide();
        $("#divStopService").hide();
        $("#txtSpecifyContractCode").attr("readonly", false);
        $("#btnRetrieve").attr("disabled", false);

        HideCommandControl();
    }
    else {
        SetRegisterState();
    }
}

function SetRegisterState() {
    $("#divContractBasicInfo").show();
    $("#divStopService").show();
    $("#txtSpecifyContractCode").attr("readonly", true);
    $("#btnRetrieve").attr("disabled", true);

    if ($("#HasStopFee").val() == 1) {
        $("#txtStopFee").attr("readonly", false);
        $("#txtStopFee").NumericCurrency().attr("disabled", false);
    }
    else {
        $("#txtStopFee").attr("readonly", true);
        $("#txtStopFee").NumericCurrency().attr("disabled", true);
    }

    RegisterCommandControl();
}

function SetSpecifyContractCodeMode(isview) {
    if (isview) {
        $("#divSpecifyContractCode").SetViewMode(true);
    }
    else {
        $("#divSpecifyContractCode").SetViewMode(false);
    }
}

function SetContractBasicInfoMode(isview) {
    if (isview) {
        $("#divContractBasicInfo").SetViewMode(true);
    }
    else {
        $("#divContractBasicInfo").SetViewMode(false);
        $("#ContractTargetCustomerImportant").attr("disabled", true);
    }
}

function SetStopServiceMode(isview) {
    if (isview) {
        $("#divStopService").SetViewMode(true);
    }
    else {
        $("#divStopService").SetViewMode(false);
    }
}

function command_register_click() {
    command_control.CommandControlMode(false);

    /* --- Set Parameter --- */
    var obj = {
        ChangeImplementDate: $("#dpStopDate").val(),
        StopCancelReasonType: $("#ddlStopReason").val(),
        ContractFeeOnStop: $("#txtStopFee").NumericValue(),
        ContractFeeOnStopCurrencyType: $("#txtStopFee").NumericCurrencyValue(),
        ExpectedResumeDate: $("#dpOperationDate").val(),
        ApproveNo1: $("#txtApproveNo1").val()
    };

    call_ajax_method_json("/Contract/CTS100_RegisterStopData", obj, doAfterRegister);

    //command_control.CommandControlMode(true);
}

function doAfterRegister(result, controls) {
    if (controls != undefined) {
        /* --- Higlight Text --- */
        /* --------------------- */
        VaridateCtrl(["dpStopDate",
                        "ddlStopReason",
                        "txtStopFee",
                        "dpOperationDate",
                        "txtApproveNo1"], controls);

        return;
    }
    else if (result != undefined) {
        /* --- Set View Mode --- */
        /* --------------------- */
        SetSpecifyContractCodeMode(true);
        SetContractBasicInfoMode(true);
        SetStopServiceMode(true);

        ConfirmCommandControl();
    }
}

function command_reset_click() {
    /* --- Get Message --- */
    /* ------------------- */
    var obj = {
        module: "Common",
        code: "MSG0038"
    };
    call_ajax_method_json("/Shared/GetMessage", obj,
        function (result) {
            OpenYesNoMessageDialog(result.Code, result.Message,
                function () {
                    //SetInitialState();

                    if ($("#ContractCodeShort").val() == "") {
                        $("#divSpecifyContractCode").clearForm();
                        $("#ContractCodeShort").val("");
                        SetInitialState();
                    }
                    else {
                        var obj = { ContractCode: $("#ContractCodeShort").val() };
                        ajax_method.CallScreenControllerWithAuthority("/Contract/CTS100", obj, false, null);
                    }
                },
                null);
        }
    );
    /* ------------------- */
}

function command_confirm_click() {
    command_control.CommandControlMode(false);

    /* --- Set Parameter --- */
    var objStopReason = CreateObjectData($("#formCTS100").serialize());

    var obj = {
        ChangeImplementDate: objStopReason.dpStopDate,
        StopCancelReasonType: objStopReason.ddlStopReason,
        ContractFeeOnStop: objStopReason.txtStopFee,
        ContractFeeOnStopCurrencyType: objStopReason.txtStopFeeCurrencyType,
        ExpectedResumeDate: objStopReason.dpOperationDate,
        ApproveNo1: objStopReason.txtApproveNo1,
        ApproveNo2: objStopReason.txtApproveNo2
    };

    call_ajax_method_json("/Contract/CTS100_ConfirmRegisterCancelData", obj,
        function (result) {
            if (result != undefined && result.length == 2) {
                OpenInformationMessageDialog(result[0].Code, result[0].Message,
                    function () {
//                        if (result[1] == "") {
//                            SetSpecifyContractCodeMode(false);
//                            SetContractBasicInfoMode(false);
//                            SetStopServiceMode(false);

//                            clear_button_click();
//                        }
//                        else {
//                            window.location.href = generate_url("/Common/CMS020");
//                        }

                        $("#divContractBasicInfo").clearForm();

                        SetSpecifyContractCodeMode(false);
                        SetContractBasicInfoMode(false);
                        SetStopServiceMode(false);

                        clear_button_click(false);
                    }
                );
            }
        }
    );

    //command_control.CommandControlMode(true);
}

function command_back_click() {
    SetSpecifyContractCodeMode(false);
    SetContractBasicInfoMode(false);
    SetStopServiceMode(false);

    RegisterCommandControl();
}

function retrieve_button_click() {
    $("#btnRetrieve").attr("disabled", true);
    $("#btnClear").attr("disabled", true);

    var obj = { strContractCode: $("#txtSpecifyContractCode").val() };
    call_ajax_method_json("/Contract/CTS100_RetrieveData", obj,
        function (result, controls) {
            if (controls != undefined) {
                /* --- Higlight Text --- */
                /* --------------------- */
                VaridateCtrl(["txtSpecifyContractCode"], controls);

                $("#divContractBasicInfo").clearForm();
                $("#divStopService").clearForm();
                SetInitialState();

                $("#btnRetrieve").attr("disabled", false);
                $("#btnClear").attr("disabled", false);
                return;
            }
            else if (result != undefined) {
                $("#divContractBasicInfo").bindJSON(result.doRentalContractBasicData);
                if (result.doRentalContractBasicData != null) {
                    $("#ContractTargetCustomerImportant").attr("checked", result.doRentalContractBasicData.ContractTargetCustomerImportant);
                }

                $("#HasStopFee").val(result.HasStopFee);

                SetRegisterState();
                $("#btnClear").attr("disabled", false);

                /* --- Set condition --- */
                SEARCH_CONDITION = {
                    ContractCode: obj.strContractCode
                };
                /* --------------------- */
            }
            else {
                $("#btnRetrieve").attr("disabled", false);
                $("#btnClear").attr("disabled", false);
            }
        }
    );
}

function clear_button_click(clearAll) {
    if ($("#ContractCodeShort").val() != "") {
        call_ajax_method_json("/Contract/CTS100_ClearData", { clearAll: clearAll }, null);
    }

    $("#divSpecifyContractCode").clearForm();
    $("#divContractBasicInfo").clearForm();
    $("#divStopService").clearForm();

    CloseWarningDialog();
    SetInitialState();

    if (clearAll == true) {
        SEARCH_CONDITION = null;
    }
}
