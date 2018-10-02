/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>

/*--- Main ---*/
var gridContractTargetPurchaserCTS130;
var gridRealCustomerCTS130;
var gridBillingTargetCTS130;
var customerSearchType = "";
var contractCode = "";
var doCustomerObject = null;

$(document).ready(function () {
    //InitModeSection();
    SetInitialState();
    InitialEvent();
});

function InitModeSection() {
    var step = [
    //"CTS130_02",
                "CTS130_03",
                "CTS130_04",
                "CTS130_05",
                "CTS130_06",
                "CTS130_07",
                "CTS130_08"
               ];

    CallMultiLoadScreen("Contract", [step], null);
}

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

function SetInitialState() {
    if ($("#ServiceTypeCode").val() == $("#C_SERVICE_TYPE_RENTAL").val()) {
        $("#divRentalContractBasicSection").show();
        $("#divSaleContractBasicSection").hide();
    }
    else if ($("#ServiceTypeCode").val() == $("#C_SERVICE_TYPE_SALE").val()) {
        $("#divRentalContractBasicSection").hide();
        $("#divSaleContractBasicSection").show();
    }

    contractCode = $("#ContractCodeShort").val();

    RegisterCommandControl();
}

function InitialEvent() {


}

function SetSectionMode(isView) {
    SetSectionModeCTS130_01(isView);
    SetSectionModeCTS130_02(isView);
    SetSectionModeCTS130_03(isView);
    SetSectionModeCTS130_04(isView);
    SetSectionModeCTS130_05(isView);
    SetSectionModeCTS130_06(isView);
    SetSectionModeCTS130_07(isView);
    SetSectionModeCTS130_08(isView);
}

function command_register_click() {
    //DisableRegisterCommand(true);
    command_control.CommandControlMode(false);

    /* --- Set Parameter --- */
    var obj = {
        ChangeNameReasonType: $("#ChangeReasonType").val(),
        BranchContractFlag: $("#PC_BranchContract").prop("checked"),
        BranchNameEN: $("#PC_BranchNameEnglish").val(),
        BranchNameLC: $("#PC_BranchNameLocal").val(),
        BranchAddressEN: $("#PC_BranchAddressEnglish").val(),
        BranchAddressLC: $("#PC_BranchAddressLocal").val(),
        ContractTargetSignerTypeCode: $("#PC_ContractSignerType").val(),
        ContactPoint: $("#ContactPointDetail").val(),
        IsShowBillingTagetDetail: $("#divBillingTargetDetailSection").is(':visible')
    };

    call_ajax_method_json("/Contract/CTS130_RegisterChangeNameAddress", obj, doAfterRegister);

    //DisableRegisterCommand(false);
}

function doAfterRegister(result, controls) {
    if (controls != undefined) {
        /* --- Higlight Text --- */
        /* --------------------- */
        VaridateCtrl(["ChangeReasonType",
                        "PC_CustomerCode",
                        "PC_NameEnglish",
                        "PC_NameLocal",
                        "PC_CustomerType",
                        "PC_BranchNameEnglish",
                        "PC_BranchNameLocal",
                        "PC_BranchAddressEnglish",
                        "PC_BranchAddressLocal",
                        "RC_CustomerCode",
                        "RC_NameEnglish",
                        "RC_NameLocal",
                        "RC_CustomerType",
                        "ST_SiteCode",
                        "ST_NameEnglish",
                        "ST_NameLocal",
                        "ST_AddressEnglish",
                        "ST_AddressLocal"], controls);
        return;
    }
    else if (result != undefined) {
        SetSectionMode(true);
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
                    //SetResetState();
                    var obj = { ContractCode: $("#ContractCodeShort").val() };
                    ajax_method.CallScreenControllerWithAuthority("/Contract/CTS130", obj, false, null);
                },
                null);
        }
    );
    /* ------------------- */
}

function SetResetState() {
    SetInitialState();
    SetInitialStateCTS130_04();
    SetInitialStateCTS130_05();
    SetInitialStateCTS130_06();
    SetInitialStateCTS130_07();
    SetInitialStateCTS130_08(false);

    SetSectionMode(false);
}

function command_confirm_click() {
    //DisableConfirmCommand(true);
    command_control.CommandControlMode(false);

    /* --- Set Parameter --- */
    var obj = {
        ChangeNameReasonType: $("#ChangeReasonType").val(),
        BranchContractFlag: $("#PC_BranchContract").prop("checked"),
        BranchNameEN: $("#PC_BranchNameEnglish").val(),
        BranchNameLC: $("#PC_BranchNameLocal").val(),
        BranchAddressEN: $("#PC_BranchAddressEnglish").val(),
        BranchAddressLC: $("#PC_BranchAddressLocal").val(),
        ContractTargetSignerTypeCode: $("#PC_ContractSignerType").val(),
        ContactPoint: $("#ContactPointDetail").val()
    };

    call_ajax_method_json("/Contract/CTS130_ConfirmRegisterChangeNameAddress", obj,
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

    //DisableConfirmCommand(false);
}

function command_back_click() {
    SetSectionMode(false);
    RegisterCommandControl();
}

/*------ MAS050 Dialog ------*/
/*---------------------------*/
function newedit_button_click_MAS050(result) {
    //if (result != undefined) {
        doCustomerObject = result;
    //}
    $("#dlgCTS130").OpenMAS050Dialog("CTS130");
}
function MAS050Object() {
    return {
        doCustomer: doCustomerObject
    };
}
function MAS050Response(result) {
    var obj = { customerData: result };
    $("#dlgCTS130").CloseDialog();

    if (customerSearchType == "PC") {
        $("#PC_CustomerCodeSpecify").val("");

        call_ajax_method_json("/Contract/CTS130_UpdateContractTargetPurchaserData", obj,
        function (result, controls) {
            if (result != undefined) {
                BindDataCTS130_04(obj.customerData);
                SetReadonlySpecifyCodeCTS130_04(true);
            }
        });

    }
    else if (customerSearchType == "RC") {
        $("#RC_CustomerCodeSpecify").val("");

        call_ajax_method_json("/Contract/CTS130_UpdateRealCustomerData", obj,
        function (result, controls) {
            if (result != undefined) {
                BindDataCTS130_05(obj.customerData);
                SetReadonlySpecifyCodeCTS130_05(true);
            }
        });
    }
}
/*--------------------------*/