/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>

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
    $("#dpStartResumeOperationDate").InitialDate();

    $("#dpStartResumeOperationDate").SetMinDate(C_STARTRESUME_MINDATE);
    $("#dpStartResumeOperationDate").SetMaxDate(C_STARTRESUME_MAXDATE);

    $("#dpAdjustBillingTerm").InitialDate();
}

function InitialEvent() {
    InitialTrimTextEvent(["txtApproveNo"]);

    $("#btnRetrieve").click(retrieve_button_click);
    $("#btnClear").click(clear_button_click);
    $("#ddlStartType").change(start_type_change);
}

function SetInitialState() {
    $("#divTelephoneLineInformation").hide();
    $("#divRequireUserCode").hide();
    $("#divRequireApproveNo").hide();

    //$("#divStartResumeOperation").clearForm();
    //$("#dpStartResumeOperationDate").val(ConvertDateToTextFormat($("#CurrentDate").val()));
    //$("#dpAdjustBillingTerm").val(ConvertDateToTextFormat($("#LastDate").val()));
    $("#ddlStartType").val("");
    $("#dpStartResumeOperationDate").val("");
    $("#txtApproveNo").val("");
    $("#dpAdjustBillingTerm").val("");

    var currentDate = new Date();
    $("#dpStartResumeOperationDate").datepicker("setDate", currentDate);
    $("#dpStartResumeOperationDate").val(ConvertDateToTextFormat(ConvertDateObject(currentDate)));

    if ($("#ContractCodeShort").val() == '') {
        $("#divContractBasicInfo").hide();
        $("#divStartResumeOperation").hide();
        $("#divTelephoneLineInformation").hide();
        $("#txtSpecifyContractCode").attr("readonly", false);
        $("#btnRetrieve").attr("disabled", false);

        HideCommandControl();
    }
    else {
        ReGenerateStartTypeCombo();
        SetRegisterState();
    }
}

function SetRegisterState() {
    $("#divContractBasicInfo").show();
    $("#divStartResumeOperation").show();
    $("#txtSpecifyContractCode").attr("readonly", true);
    $("#btnRetrieve").attr("disabled", true);

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

function SetStartResumeOperationMode(isview) {
    if (isview) {
        $("#divStartResumeOperation").SetViewMode(true);
    }
    else {
        $("#divStartResumeOperation").SetViewMode(false);
    }
}

function SetTelephoneLineInformationMode(isview) {
    if (isview) {
        $("#divTelephoneLineInformation").SetViewMode(true);
    }
    else {
        $("#divTelephoneLineInformation").SetViewMode(false);
    }
}

function command_register_click() {
    command_control.CommandControlMode(false);

    /* --- Set Parameter --- */
    var obj = {
        StartType: $("#ddlStartType").val(),
        StartResumeOperationDate: $("#dpStartResumeOperationDate").val(),
        ApproveNo: $("#txtApproveNo").val(),
        AdjustBillingTerm: $("#dpAdjustBillingTerm").val(),
        UserCode: $("#txtUserCode").val(),
        LineTypeNormal: $("#ddlLineTypeNormal").val(),
        TelephoneOwnerNormal: $("#ddlTelephoneOwnerNormal").val(),
        TelephoneNoNormal: $("#txtTelephoneNoNormal").val(),
        LineTypeImage: $("#ddlLineTypeImage").val(),
        TelephoneOwnerImage: $("#ddlTelephoneOwnerImage").val(),
        TelephoneNoImage: $("#txtTelephoneNoImage").val(),
        LineTypeDisconnection: $("#ddlLineTypeDisconnection").val(),
        TelephoneOwnerDisconnection: $("#ddlTelephoneOwnerDisconnection").val(),
        TelephoneNoDisconnection: $("#txtTelephoneNoDisconnection").val()
    };

    call_ajax_method_json("/Contract/CTS070_RegisterStartResumeData", obj, doAfterRegister);

    //command_control.CommandControlMode(true);
}

function doAfterRegister(result, controls) {
    if (controls != undefined) {
        /* --- Higlight Text --- */
        /* --------------------- */
        VaridateCtrl(["ddlStartType",
                        "dpStartResumeOperationDate",
                        "txtApproveNo",
                        "txtUserCode",
                        "ddlLineTypeNormal",
                        "ddlTelephoneOwnerNormal",
                        "txtTelephoneNoNormal",
                        "dpAdjustBillingTerm"], controls); //Add (dpAdjustBillingTerm) by Jutarat A. on 30042013

        return;
    }
    else if (result != undefined) {
        /* --- Set View Mode --- */
        /* --------------------- */
        SetSpecifyContractCodeMode(true);
        SetContractBasicInfoMode(true);
        SetStartResumeOperationMode(true);
        SetTelephoneLineInformationMode(true);

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
                        ajax_method.CallScreenControllerWithAuthority("/Contract/CTS070", obj, false, null);
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
    var obj = {
        StartType: $("#ddlStartType").val(),
        StartResumeOperationDate: $("#dpStartResumeOperationDate").val(),
        ApproveNo: $("#txtApproveNo").val(),
        AdjustBillingTerm: $("#dpAdjustBillingTerm").val(),
        UserCode: $("#txtUserCode").val(),
        LineTypeNormal: $("#ddlLineTypeNormal").val(),
        TelephoneOwnerNormal: $("#ddlTelephoneOwnerNormal").val(),
        TelephoneNoNormal: $("#txtTelephoneNoNormal").val(),
        LineTypeImage: $("#ddlLineTypeImage").val(),
        TelephoneOwnerImage: $("#ddlTelephoneOwnerImage").val(),
        TelephoneNoImage: $("#txtTelephoneNoImage").val(),
        LineTypeDisconnection: $("#ddlLineTypeDisconnection").val(),
        TelephoneOwnerDisconnection: $("#ddlTelephoneOwnerDisconnection").val(),
        TelephoneNoDisconnection: $("#txtTelephoneNoDisconnection").val()
    };

    call_ajax_method_json("/Contract/CTS070_ConfirmRegisterStartResumeData", obj,
        function (result) {
            if (result != undefined && result.length == 2) {
                OpenInformationMessageDialog(result[0].Code, result[0].Message,
                    function () {
//                        if (result[1] == "") {
//                            SetSpecifyContractCodeMode(false);
//                            SetContractBasicInfoMode(false);
//                            SetStartResumeOperationMode(false);
//                            SetTelephoneLineInformationMode(false);

//                            clear_button_click();
//                        }
//                        else {
//                            window.location.href = generate_url("/Common/CMS020");
//                        }

                        $("#divContractBasicInfo").clearForm();

                        SetSpecifyContractCodeMode(false);
                        SetContractBasicInfoMode(false);
                        SetStartResumeOperationMode(false);
                        SetTelephoneLineInformationMode(false);

                        clear_button_click();
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
    SetStartResumeOperationMode(false);
    SetTelephoneLineInformationMode(false);

    RegisterCommandControl();
}

function retrieve_button_click() {
    $("#btnRetrieve").attr("disabled", true);
    $("#btnClear").attr("disabled", true);

    var obj = { strContractCode: $("#txtSpecifyContractCode").val() };
    call_ajax_method_json("/Contract/CTS070_RetrieveData", obj,
        function (result, controls) {
            if (controls != undefined) {
                /* --- Highlight Text --- */
                /* --------------------- */
                VaridateCtrl(["txtSpecifyContractCode"], controls);

                $("#divContractBasicInfo").clearForm();
                $("#divStartResumeOperation").clearForm();
                SetInitialState();

                $("#btnRetrieve").attr("disabled", false);
                $("#btnClear").attr("disabled", false);
                return;
            }
            else if (result != undefined) {
                $("#divContractBasicInfo").bindJSON(result.doRentalContractBasicData);
                if (result.doRentalContractBasicData != null) {
                    $("#ContractTargetCustomerImportant").attr("checked", result.doRentalContractBasicData.ContractTargetCustomerImportant);
                    $("#txtUserCode").val(result.doRentalContractBasicData.UserCode); //Add by Jutarat A. on 15082012
                    
                    
                }

                $("#ProductTypeCode").val(result.RentalContractBasicData.ProductTypeCode);
                
                ReGenerateStartTypeCombo();
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

function clear_button_click() {
    if ($("#ContractCodeShort").val() != "") {
        call_ajax_method_json("/Contract/CTS070_ClearData", "", function () {
            SEARCH_CONDITION = null;
        });
    }

    $("#divSpecifyContractCode").clearForm();
    $("#divContractBasicInfo").clearForm();
    $("#divStartResumeOperation").clearForm();

    CloseWarningDialog();
    SetInitialState();
}

function start_type_change() {
    $("#divTelephoneLineInformation").clearForm();

    $("#divTelephoneLineInformation").hide();
    $("#divRequireUserCode").hide();
    $("#divRequireApproveNo").hide();

    if ($("#ProductTypeCode").val() == $("#C_PROD_TYPE_AL").val()
        || $("#ProductTypeCode").val() == $("#C_PROD_TYPE_ONLINE").val()
        || $("#ProductTypeCode").val() == $("#C_PROD_TYPE_RENTAL_SALE").val()) {

        if ($("#ddlStartType").val() == $("#C_START_TYPE_NEW_START").val()) {
            $("#divTelephoneLineInformation").show();

            $("#ddlLineTypeNormal").val("");
            $("#ddlTelephoneOwnerNormal").val("");
            $("#ddlLineTypeImage").val("");
            $("#ddlTelephoneOwnerImage").val("");
            $("#ddlLineTypeDisconnection").val("");
            $("#ddlTelephoneOwnerDisconnection").val("");

            $("#divRequireUserCode").show();
        }
        //Add by Jutarat A. on 15082012
        else if ($("#ddlStartType").val() == $("#C_START_TYPE_ALTER_START").val()) {
            $("#divRequireApproveNo").show();
        }
        else if ($("#ddlStartType").val() == $("#C_START_TYPE_RESUME").val()) {
            $("#divRequireUserCode").show();
        }
        //End Add
    }

}

function ReGenerateStartTypeCombo() {
    call_ajax_method_json("/Contract/CTS070_GetStartType", "",
        function (result) {
            if (result != undefined) {
                regenerate_combo("#ddlStartType", result);
            }
        }
    );
}
