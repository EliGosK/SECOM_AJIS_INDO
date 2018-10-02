/*--- Main ---*/
$(document).ready(function () {
    InitialCommandControl();

    InitialControl();
    InitialEvent();
    SetInitialState();
});

function InitialCommandControl() {
    SetRegisterCommand(true, command_register_click);
    SetResetCommand(true, command_reset_click);
}

function InitialControl() {
    $("#dpCancelDate").InitialDate();
}

function InitialEvent() {
    $("#btnViewInstallationDetail").click(view_installation_detail_click);
    InitialTrimTextEvent(["txtApproveNo"]);
}

function SetInitialState() {
    //$("#btnViewInstallationDetail").attr("disabled", true); /* --- Implement in next phase --- */

    $("#divCancelSaleContract").clearForm();
    //$("#dpCancelDate").val(ConvertDateToTextFormat($("#CurrentDate").val()));

    var currentDate = new Date();
    $("#dpCancelDate").datepicker("setDate", currentDate);
    $("#dpCancelDate").val(ConvertDateToTextFormat(ConvertDateObject(currentDate)));
}

function SetSaleContractInfoMode(isview) {
    if (isview) {
        $("#divSaleContractBasicInfo").SetViewMode(true);
    }
    else {
        $("#divSaleContractBasicInfo").SetViewMode(false);
        $("#PurchaserCustomerImportant").attr("disabled", true);
    }
}

function SetCancelSaleContractMode(isview) {
    if (isview) {
        $("#divCancelSaleContract").SetViewMode(true);
    }
    else {
        $("#divCancelSaleContract").SetViewMode(false);
    }
}

function command_register_click() {
    command_control.CommandControlMode(false);

    /* --- Set Parameter --- */
    var obj = {
        CancelDate: $("#dpCancelDate").val(),
        CancelReasonType: $("#ddlCancelReason").val(),
        ApproveNo1: $("#txtApproveNo").val()
    };

    call_ajax_method_json("/Contract/CTS090_RegisterCancelData", obj, doAfterRegister);

    //command_control.CommandControlMode(true);
}

function doAfterRegister(result, controls) {
    if (controls != undefined) {
        /* --- Higlight Text --- */
        /* --------------------- */
        VaridateCtrl(["dpCancelDate",
                        "ddlCancelReason",
                        "txtApproveNo",
                        "LastChangeType",
                        "InstallationStatus"], controls);

        return;
    }
    else if (result != undefined) {
        /* --- Set View Mode --- */
        /* --------------------- */
        SetSaleContractInfoMode(true);
        SetCancelSaleContractMode(true);

        /* --- Set Command Button --- */
        /* -------------------------- */
        SetRegisterCommand(false, null);
        SetResetCommand(false, null);
        SetConfirmCommand(true, command_confirm_click);
        SetBackCommand(true, command_back_click);
        /* -------------------------- */
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
                    var obj = { ContractCode: $("#ContractCodeShort").val() };
                    ajax_method.CallScreenControllerWithAuthority("/Contract/CTS090", obj, false, null);
                },
                null);
        }
    );
     /* ------------------- */
}

function command_confirm_click() {
    command_control.CommandControlMode(false);

    /* --- Set Parameter --- */
    var obj = {
        CancelDate: $("#dpCancelDate").val(),
        CancelReasonType: $("#ddlCancelReason").val(),
        ApproveNo1: $("#txtApproveNo").val()
    };

    call_ajax_method_json("/Contract/CTS090_ConfirmRegisterCancelData", obj,
        function (result) {
            if (result != undefined) {
                OpenInformationMessageDialog(result.Code, result.Message, 
                    function () {
                        window.location.href = generate_url("/Common/CMS020");
                    }
                );
            }
        }
    );

    //command_control.CommandControlMode(true);
}

function command_back_click() {

    SetSaleContractInfoMode(false);
    SetCancelSaleContractMode(false);

    /* --- Set Command Button --- */
    /* -------------------------- */
    SetRegisterCommand(true, command_register_click);
    SetResetCommand(true, command_reset_click);
    SetConfirmCommand(false, null);
    SetBackCommand(false, null);
    /* -------------------------- */
}

function view_installation_detail_click() {
//    /* --- Implement in next phase --- */
//    var myurl = generate_url("/Common/CMS180") + "?strContractCode=" + $("#txtContractCode").val();
//    alert("Implement in next phase (CMS180) : " + myurl);

    var obj = { ContractCode: $("#ContractCodeShort").val() };
    ajax_method.CallScreenControllerWithAuthority("/Common/CMS180", obj, true, null);
}
