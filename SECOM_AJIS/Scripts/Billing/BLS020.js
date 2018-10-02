var objOffice = undefined;

$(document).ready(function () {
    FirstLoadScreen();

    //---------------- Initial Even ------------------    
    $("#btnRetrieve").click(retrieve_click);

    $("#btnSearchBillingTarget").click(function () {
        $("#dlgBLS020").OpenCMS470Dialog();
    });
    $("#IssueInvTime").change(SelectIssueInvTimeChange);
    InitialNumericInputTextBox(["IssueInvMonth"], false);

    //Set max length for TextArea
    $("#Memo").SetMaxLengthTextArea(4000);

    // tt
    $("#RealBillingClientAddressEN").SetMaxLengthTextArea(1600, 3);
    $("#RealBillingClientAddressLC").SetMaxLengthTextArea(1600, 3);

    $('#BillingOfficeCode').val(objBLS020.BillingOfficeCboValue);
    $("#ShowDueDate").val(objBLS020Conts.C_SHOW_DUEDATE_7);
}
);

function FirstLoadScreen() {

    //------------ Hide Section -----------------------
    //  $("#divResultOfRegisterBillingTarget").hide();

    InitialScreenControl();
    InitialCommandButton(2);
}

function InitialScreenControl() {
    if (objBLS020.BillingClientCode != undefined && objBLS020.BillingClientCode != "") {
        retrieve_click();
    }

    //---------------- Clear Data ---------------------
    $("#divBillingTargetInfo").clearForm();
    $("#divInvoiceInfo").clearForm();

    // tt
    $("#divBillingClientInfo").clearForm();

    //------------- Set Disable Section ----------------------
    SetDisabledivBillingTargetInfo(true, false);
    SetDisabledivInvoiceInfo(true, true);
    
    // tt
    SetDisabledivBillingClientInfo(true);

    // tt
    $("#IssueInvMonth").val("");
}

function InitialCommandButton(step) {
    if (step == 0) {
        register_command.SetCommand(register_billingTarget_click);
        reset_command.SetCommand(command_reset_click);
        confirm_command.SetCommand(null);
        back_command.SetCommand(null);
    }
    else if (step == 1) {
        register_command.SetCommand(null);
        reset_command.SetCommand(null);
        confirm_command.SetCommand(command_confirm_click);
        back_command.SetCommand(command_back_click);
    }
    else if (step == 2) {
        confirm_command.SetCommand(null);
        back_command.SetCommand(null);
        register_command.SetCommand(null);
        reset_command.SetCommand(null);
    }
}

function SetDisabledivBillingTargetInfo(flag, flag2) {
    $("#BillingOfficeCode").attr("disabled", flag);
    $("#ContactPersonName").attr("readonly", flag);
    $("#Memo").attr("readonly", flag);
    $("#BillingClientCode").attr("readonly", flag2);
    $("#BillingTargetNo").attr("readonly", flag2);
    $("#btnRetrieve").attr("disabled", flag2);
    $("#btnSearchBillingTarget").attr("disabled", flag2);
}

function SetDisabledivInvoiceInfo(flag, retrieve) {

    $("#IssueInvTime").attr("disabled", flag);
    $("#IssueInvMonth").attr("readonly", flag);
    $("#IssueInvDate").attr("disabled", flag);
    $("#InvFormatType").attr("disabled", flag);
    $("#SignatureType").attr("disabled", flag);
    $("#DocLanguage").attr("disabled", flag);
    $("#ShowDueDate").attr("disabled", flag);
    $("#IssueReceiptTiming").attr("disabled", flag);
    $("#ShowAccType").attr("disabled", flag);

    $("#WhtDeductionType").attr("disabled", retrieve);
    $("#ShowInvWHTFlag").attr("disabled", flag);

    // Akat K. 2014-05-21 new combobox
    $("#PrintAdvanceDate").attr("disabled", flag);

    $("#ShowIssueDate").attr("disabled", flag);
    $("#PayByChequeFlag").attr("disabled", flag);
    $("#SeparateInvType").attr("disabled", flag);
    $("#SuppleInvAddress").attr("readonly", flag);
}

function SetDisabledivBillingClientInfo(flag) {
    $("#RealBillingClientNameEN").attr("readonly", flag);
    $("#RealBillingClientAddressEN").attr("readonly", flag);
    $("#RealBillingClientNameLC").attr("readonly", flag);
    $("#RealBillingClientAddressLC").attr("readonly", flag);
}

function SetScreenControl(result) {

    SetDisabledivBillingTargetInfo(false, true);

    $("#divBillingTargetInfo").bindJSON(result);
    $("#divInvoiceInfo").bindJSON(result);
    if (result.BillingClientCode_Short != undefined) {
        $("#BillingClientCode").val(result.BillingClientCode_Short)
    }
    $("#ShowInvWHTFlag").attr("checked", result.ShowInvWHTFlag);
    $("#PayByChequeFlag").attr("checked", result.PayByChequeFlag);

    //set enable invoice info section
    SetDisabledivInvoiceInfo(false, true);

    //set value for control
    //$("#ShowInvWHTFlag").prop(false);

    result.BillingOfficeNameCode = result.BillingOfficeCode + ": " + result.OfficeName;
    if ($("#BillingOfficeCode option[value='" + result.BillingOfficeCode + "']").length == 0) {
        $('#BillingOfficeCode').append($('<option></option>').val(result.BillingOfficeCode).html(result.BillingOfficeNameCode))
        $('#BillingOfficeCode').val(result.BillingOfficeCode);
        $("#BillingOfficeCode").attr("disabled", true);
        objOffice = {
            BillingOfficeName: result.BillingOfficeNameCode,
            BillingOfficeCode: result.BillingOfficeCode
        };
    }
    else {
        $("#BillingOfficeCode").attr("disabled", false);
    }
    if (result.CustTypeCode == objBLS020Conts.C_CUST_TYPE_JURISTIC && (result.IDNo != null && result.IDNo != "")) {
        $("#WhtDeductionType").attr("disabled", false);
        $("#ShowInvWHTFlag").attr("disabled", false);
    }
    else {
        $("#WhtDeductionType").attr("disabled", true);
        //$("#ShowInvWHTFlag").attr("disabled", true);
    }

    // tt
    SetDisabledivBillingClientInfo(false);
    $("#divBillingClientInfo").bindJSON(result);
}

function SetControlAfterRegister() {
    $("#btnRetrieve").hide();
    $("#btnSearchBillingTarget").hide();

    InitialCommandButton(1);
    $("#divBillingTargetInfo").SetViewMode(true);
    $("#divInvoiceInfo").SetViewMode(true);

    $("#divBillingClientInfo").SetViewMode(true);
}

function SetControlBeforeRegister() {
    $("#btnRetrieve").show();
    $("#btnSearchBillingTarget").show();
    InitialCommandButton(0);
    $("#divBillingTargetInfo").SetViewMode(false);
    $("#divInvoiceInfo").SetViewMode(false);
    //$("#ShowInvWHTFlag").attr("disabled", true);

    $("#divBillingClientInfo").SetViewMode(false);
}

function SetControlAfterConfirm(obj) {

    InitialScreenControl();
    InitialCommandButton(2);
    //SetControlAfterRegister();
    DisableConfirmCommand(false);
}

//------------- Even Click  ----------------------

function register_billingTarget_click() {

    register_command.EnableCommand(false);
    var obj = { BillingClientCode: $("#BillingClientCode").val(),
        BillingTargetNo: $("#BillingTargetNo").val(),
        BillingOfficeCode: $("#BillingOfficeCode").val(),
        IssueInvMonth: $("#IssueInvMonth").val()
    };

    ajax_method.CallScreenController("/Billing/BLS020_RegisterBillingTarge", obj,
        function (result, controls) {
            if (controls != undefined) {
                VaridateCtrl(["BillingClientCode", "BillingTargetNo", "BillingOfficeCode", "IssueInvMonth"], controls);
                return;
            }
            else if (result != undefined) {
                SetControlAfterRegister();
                register_command.EnableCommand(true);
                $("#divIssueInvMonth").attr("style", "text-align: right");
            }
        });
}
function retrieve_click() {
    var obj;
    if ((objBLS020.BillingClientCode != undefined && objBLS020.BillingClientCode != "")
    && (objBLS020.BillingTargetNo != undefined && objBLS020.BillingTargetNo != "")) {
        obj = objBLS020;
    }
    else {
        obj = { BillingClientCode: $("#BillingClientCode").val(),
            BillingTargetNo: $("#BillingTargetNo").val()
        };
    }


    ajax_method.CallScreenController("/Billing/BLS020_RetrieveBillingTargetData", obj,
        function (result, controls) {
            if (controls != undefined) {
                VaridateCtrl(["BillingClientCode", "BillingTargetNo"], controls);
                return;
            }
            else if (result != undefined) {
                SetScreenControl(result);
                //SelectIssueInvTimeChange(); // Narupon comment this line
                InitialCommandButton(0);
                objBLS020.BillingTargetCode = undefined;
                objBLS020.BillingClientCode = undefined;
                objBLS020.BillingTargetNo = undefined;
            }
        });
}
function command_confirm_click() {
    confirm_command.EnableCommand(false);
    var obj = CreateObjectData($("#form1").serialize());
    obj.BillingOfficeCode = $("#BillingOfficeCode").val();
    //obj.IssueInvTime = $("#IssueInvTime").val();
    //obj.Memo = $("#Memo").val();
    obj.PayByChequeFlag = $("#PayByChequeFlag").prop("checked");
    obj.ShowInvWHTFlag = $("#ShowInvWHTFlag").prop("checked");
    obj.WhtDeductionType = $("#WhtDeductionType").val();

    ajax_method.CallScreenController("/Billing/BLS020_ConfirmEditBillingTarget", obj,
        function (result, controls) {
            if (controls != undefined) {
                VaridateCtrl(["BillingClientCode", "BillingTargetNo", "BillingOfficeCode"], controls);
                return;
            }
            else if (result != undefined) {
                //SetScreenControl(result);
                //SetControlAfterConfirm(result);
                SetControlBeforeRegister();
                confirm_command.EnableCommand(true);
                
                // tt
                //InitialScreenControl();

                FirstLoadScreen();

                /// Clear office
                if (objOffice != undefined) {
                    //var strOption = '<option selected="selected" value="' + objOffice.BillingOfficeCode + '">' + objOffice.BillingOfficeName + '</option>';
                    //$('#BillingOfficeCode').empty().append(strOption);
                    $("#BillingOfficeCode option[value='" + objOffice.BillingOfficeCode + "']").remove();
                }
                objOffice = undefined;
                $("#BillingOfficeCode").val("");
            }
        });
}

function command_back_click() {
    SetControlBeforeRegister();
}
function command_reset_click() {
    ResetSessionBLS020();
}

function ResetSessionBLS020() {
    /* ajax_method.CallScreenController("/Billing/BLS020_ResetSession", "",
    function (result) {
    if (result != undefined) {
    FirstLoadScreen();
    }
    });*/

    /// Clear office
    if (objOffice != undefined) {
        $("#BillingOfficeCode option[value='" + objOffice.BillingOfficeCode + "']").remove();
    }
    objOffice = undefined;
    $("#BillingOfficeCode").val("");

    FirstLoadScreen();
}

/*------ CMS470 Dialog ------*/
/*---------------------------*/
function CMS470Response(result_BillingTarget) {
    //var obj = { billingClientData: result };
    var obj = result_BillingTarget;
    $("#dlgBLS020").CloseDialog();
    if (obj == null) {
        return;
    }
    else {
        //         SetScreenControl(obj);
        //         SelectIssueInvTimeChange();
        //         InitialCommandButton(0);
        //         objBLS020.BillingTargetCode = undefined;
        //         objBLS020.BillingClientCode = undefined;
        //         objBLS020.BillingTargetNo = undefined;
        $("#BillingClientCode").val(result_BillingTarget.BillingClientCode_Short);
        $("#BillingTargetNo").val(result_BillingTarget.BillingTargetNo);
        retrieve_click();
    }
}


function SelectIssueInvTimeChange() {
    if (objBLS020Conts.C_ISSUE_INV_TIME_AT_START == $("#IssueInvTime").val()) {
        $("#IssueInvMonth").val("0");
        $("#IssueInvMonth").attr("readonly", true);
    }
    else {
        $("#IssueInvMonth").val("1");
        $("#IssueInvMonth").attr("readonly", false);
    }
}