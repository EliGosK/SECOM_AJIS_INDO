var strC_ISSUE_INV_TIME_AT_START ;
var objBLS030;
// Main
$(document).ready(function () {

    FirstLoadScreen();

    //---------------- Initial Even ------------------
    $("#btnRetrieve").click(retrieve_billing_click);  
    $("#btnSearchBillingClient").click(function () {
        $("#dlgBLS010").OpenCMS270Dialog();
    });
    $("#btnNewEditBillingClient").click(new_edit_button_click);
    $("#btnRegisterBillingBasic").click(registerBillingBasic_click);
    $("#IssueInvTime").change(SelectIssueInvTimeChange);
    InitialNumericInputTextBox(["IssueInvMonth"], false);
    //Set max length for TextArea
    $("#Memo").SetMaxLengthTextArea(4000);

    // tt
    $("#RealBillingClientAddressEN").SetMaxLengthTextArea(1600, 3);
    $("#RealBillingClientAddressLC").SetMaxLengthTextArea(1600, 3);

    //$("#BillingOfficeCode").val(objBLS010.BillingOfficeCboValue); //Modify by Jirawat Jannet @ 2016-08-16
    //$("#SignatureType").val(objBLS010Conts.C_SIG_TYPE_HAVE); //Modify by Jirawat Jannet @ 2016-08-16
    //$("#DocLanguage").val(objBLS010Conts.C_DOC_LANGUAGE_TH); //Modify by Jirawat Jannet @ 2016-08-16
    //$("#IssueReceiptTiming").val(objBLS010Conts.C_ISSUE_REC_TIME_AFTER_PAYMENT); //Modify by Jirawat Jannet @ 2016-08-16
    //$("#ShowInvWHTFlag").attr("checked", true);
}
);
function InitialScreenControl() {

    //---------------- Clear Data ---------------------
    $("#divBillingTargetInfo").clearForm();
    $("#divSpecifyCode").clearForm();
    $("#divInvoiceInfo").clearForm();
    $("#divResultOfRegisterBillingTarget").clearForm();

    // tt
    $("#divBillingClientInfo").clearForm();



    //------------ Initial Combo Value-----------------
    /*$("#IssueInvTime").val(objBLS010Conts.C_ISSUE_INV_TIME_BEFORE);
    $("#IssueInvDate").val(objBLS010Conts.C_ISSUE_INV_DATE_01);
    $("#InvFormatType").val(objBLS010Conts.C_INV_FORMAT_FOLLOW_BILLING);
    $("#SignatureType").val(objBLS010Conts.C_SIG_TYPE_NO);
    $("#SeparateInvType").val(objBLS010Conts.C_SEP_INV_EACH_CONTRACT);
    $("#DocLanguage").val(objBLS010Conts.C_DOC_LANGUAGE_TH);
    $("#ShowDueDate").val(objBLS010Conts.C_SHOW_DUEDATE_7);
    $("#IssueReceiptTiming").val(objBLS010Conts.C_ISSUE_REC_TIME_AFTER_PAYMENT);
    $("#ShowAccType").val(objBLS010Conts.C_SHOW_BANK_ACC_SHOW);
    $("#WhtDeductionType").val(objBLS010Conts.C_DEDUCT_TYPE_NOT_DEDUCT);
    $("#ShowIssueDate").val(objBLS010Conts.C_SHOW_ISSUE_DATE_CHRISTIAN);
    $("#BillingOfficeCode").val(objBLS010.BillingOfficeCboValue);*/
    
    // tt
    $("#IssueInvMonth").val("");

    
    //------------- Set Disable Section ----------------------
    SetDisableDivdivResultOfRegisterBillingTarget(false);
    SetDisabledivBillingTargetInfo(true);
    SetDisabledivInvoiceInfo(true, true); 
    
    // tt
    SetDisabledivBillingClientInfo(true);

}
function InitialCommandButton(step) {
    if (step == 0) {
        register_command.SetCommand(register_billing_click);
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
        register_command.SetCommand(null);
        reset_command.SetCommand(null);
        confirm_command.SetCommand(null);
        back_command.SetCommand(null);
    }
}
function FirstLoadScreen() {

    //------------ Hide Section -----------------------
    $("#divResultOfRegisterBillingTarget").hide();

    InitialScreenControl();
    //InitialComboValue(0);
    InitialCommandButton(0);
    InitialCommandButton(2);
}

function SetDisableDivdivResultOfRegisterBillingTarget(flag) {

    $("#BillingClientCode").attr("readonly", flag);
    $("#btnRetrieve").attr("disabled", flag);
    $("#btnSearchBillingClient").attr("disabled", flag);
}

function SetDisabledivBillingTargetInfo(flag) {
    $("#BillingOfficeCode").attr("disabled", flag);
    $("#ContactPersonName").attr("readonly", flag);
    $("#Memo").attr("readonly", flag);
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
    // Akat K. 2014-05-21 new combobox
    $("#PrintAdvanceDate").attr("disabled", flag);
    
    $("#WhtDeductionType").attr("disabled", retrieve);

    $("#ShowInvWHTFlag").attr("disabled", flag);



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


function registerBillingBasic_click() {   
    ajax_method.CallScreenControllerWithAuthority("/Billing/BLS030", objBLS030,false,null);
}

/*------ Even Click ------*/
/*---------------------------*/
function retrieve_billing_click() {

    //$("#divSpecifyCode").attr("disabled", true);
    var obj = { strBillingClientCode: $("#BillingClientCode").val() };

    ajax_method.CallScreenController("/Billing/BLS010_RetrieveData", obj,
        function (result, controls) {
            if (controls != undefined) {
                VaridateCtrl(["BillingClientCode"], controls);
                return;
            }
            else if (result != undefined) {
                SetScreenControl(result);
                InitialComboValue(1, result.CustTypeCode);
                InitialCommandButton(0);
                $("#btnNewEditBillingClient").attr("disabled", true);
            }
        });
}

function register_billing_click() {

    register_command.EnableCommand(false);

    var obj = CreateObjectData($("#form1").serialize());

    obj.WhtDeductionType = $("#WhtDeductionType").val();
    obj.NameEN = $("#FullNameEN").val();
    obj.NameLC = $("#FullNameLC").val();
    obj.RegionCode = $("#RegionCode").val();

    ajax_method.CallScreenController("/Billing/BLS010_RegisterData", obj,
        function (result, controls) {
            if (controls != undefined) {
                VaridateCtrl(["IssueInvTime", "BillingOfficeCode", "BillingTargetCode", "FullNameEN", "CustTypeName", "IssueInvMonth"], controls);
                return;
            }
            else if (result != undefined) {
                //SetScreenControl(result);
                SetControlAfterRegister();
                register_command.EnableCommand(true);
                $("#divIssueInvMonth").attr("style", "text-align: right");

            }
        });
}

function command_reset_click2() {   
    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message, ResetSessionBLS010, function () {

        });

    });

}

function command_reset_click() {
    ResetSessionBLS010();
    $("#btnNewEditBillingClient").attr("disabled", false);
    $("#CustTypeCode").val("");
    $("#CompanyTypeCode").val("");
    $("#BusinessTypeCode").val("");
    $("#RegionCode").val("");
}

function command_back_click() {
    SetControlBeforeRegister();
    setDisabledControlForViewMode(true);
}

function command_confirm_click() {
    confirm_command.EnableCommand(false);

    var obj = CreateObjectData($("#form1").serialize());
    obj.WhtDeductionType = $("#WhtDeductionType").val();
    obj.PayByChequeFlag = $("#PayByChequeFlag").prop("checked");
    obj.ShowInvWHTFlag = $('#ShowInvWHTFlag').prop('checked');
    
    ajax_method.CallScreenController("/Billing/BLS010_ConfirmBillingTargetData", obj,
        function (result, controls) {
            if (result != undefined) {
                objBLS030 = {
                    BillingClientCode: result.BillingClientCode,
                    BillingTargetRunningNo: result.BillingTargetNo
                };
                result.BillingClientCode;
                DisableConfirmCommand(false);
                SetControlAfterConfirm(result);
                confirm_command.EnableCommand(true);
                $("#btnRegisterBillingBasic").focus();
            }
        });
}

/*------ CMS270 Dialog ------*/
/*---------------------------*/
function CMS270Response(result_BillingClient) {
    //var obj = { billingClientData: result };
    var obj = result_BillingClient;
    $("#dlgBLS010").CloseDialog();
    if (obj == null) {
        return;
    }
    ajax_method.CallScreenController("/Billing/BLS010_ReturnMappingLanguage", obj,
        function (result, controls) {
            if (result != undefined) {
                //$("#divSpecifyCode").clearForm();
                SetScreenControl(result);
                //$("#BillingClientCode").val("");
                InitialComboValue(1, result.CustTypeCode);
                InitialCommandButton(0);

            }
            else {
                SetControlBeforeRegister();
               
            }
        });

}

/*------ MAS030 Dialog ------*/
/*---------------------------*/
var doBillingCustomerObject = null;
function new_edit_button_click() {
    $("#btnNewEditBillingClient").attr("disabled", true);
    
    ajax_method.CallScreenController("/Billing/BLS010_GetTempBillingClientData", "",
        function (result) {

            if (result != undefined) {
                doBillingCustomerObject = result;
            }
            else {
                doBillingCustomerObject = CreateObjectData($("#form1").serialize());
                doBillingCustomerObject.NameEN = $("#FullNameEN").val();
                doBillingCustomerObject.NameLC = $("#FullNameLC").val();
                doBillingCustomerObject.RegionCode = $("#RegionCode").val();
            }
            $("#dlgBLS010").OpenMAS030Dialog("BLS010");
        });
        $("#btnNewEditBillingClient").attr("disabled", false);
}
function MAS030Object() {
    return doBillingCustomerObject;
}
function MAS030Response(result_BillingClient) {
    //var obj = { billingClientData: result };
    var obj = result_BillingClient;
    $("#dlgBLS010").CloseDialog();
    if (obj != undefined) {

        ajax_method.CallScreenController("/Billing/BLS010_SetScreenParameterBillingtargetInfo", obj,null);        
        SetScreenControl(obj);
        $("#BillingClientCode").val("");
        InitialComboValue(1, obj.CustTypeCode);
        InitialCommandButton(0);
        $("#BillingClientCode").val("");
        $("#BillingClientCodeView").val("");
    }
    
}

function SetScreenControl(result) {
    SetDisableDivdivResultOfRegisterBillingTarget(true);
    SetDisabledivBillingTargetInfo(false);

    $("#divBillingTargetInfo").bindJSON(result);
    $("#CustTypeCode").val(result.CustTypeCode);
    $("#CompanyTypeCode").val(result.CompanyTypeCode);
    $("#BusinessTypeCode").val(result.BusinessTypeCode);
    $("#RegionCode").val(result.RegionCode);

    $("#BillingClientCodeView").val(result.BillingClientCode);
    $("#BillingClientCode").val(result.BillingClientCode);
    $("#ShowInvWHTFlag").attr("checked", result.ShowInvWHTFlag);
    $("#PayByChequeFlag").attr("checked", result.PayByChequeFlag);
    
    //set enable invoice info section
    SetDisabledivInvoiceInfo(false, true); 

    //set value for control
    //$("#ShowInvWHTFlag").prop(false);
    if (result.CustTypeCode == objBLS010Conts.C_CUST_TYPE_JURISTIC && (result.IDNo != null && result.IDNo != "")) {
        $("#WhtDeductionType").val(objBLS010Conts.C_DEDUCT_TYPE_DEDUCT);
    }
    else {
        $("#WhtDeductionType").val(objBLS010Conts.C_DEDUCT_TYPE_NOT_DEDUCT);
    }

    // tt
    SetDisabledivBillingClientInfo(false);
    $("#RealBillingClientNameEN").val(result.FullNameEN);
    $("#RealBillingClientAddressEN").val(result.AddressEN);
    $("#RealBillingClientNameLC").val(result.FullNameLC);
    $("#RealBillingClientAddressLC").val(result.AddressLC);
}

function SetControlAfterRegister() {
    InitialCommandButton(1);
    setDisabledControlForViewMode(false);
    $("#divBillingTargetInfo").SetViewMode(true);
    $("#divInvoiceInfo").SetViewMode(true);
    $("#divSpecifyCode").hide();

    $("#divBillingClientInfo").SetViewMode(true);
    
}

function SetControlBeforeRegister() {
    $("#divSpecifyCode").show();    
    InitialCommandButton(0);
    $("#divBillingTargetInfo").SetViewMode(false);
    $("#divInvoiceInfo").SetViewMode(false);
    //$("#ShowInvWHTFlag").attr("disabled", true);
    $("#ShowInvWHTFlag").attr("disabled", false); // edit by jirawat jannet

    $("#divBillingClientInfo").SetViewMode(false);
}
function SetControlAfterConfirm(obj) {
    $("#divResultOfRegisterBillingTarget").show();
    $("#BillingTargetCode").val(obj.BillingTargetCode);
    InitialCommandButton(2);
    //SetControlAfterRegister();
    DisableConfirmCommand(false);
}

function ResetSessionBLS010() {
    ajax_method.CallScreenController("/Billing/BLS010_ResetSession", "",
        function (result) {
            if (result != undefined) {
                FirstLoadScreen();                
            }            
        });
    }

    function SelectIssueInvTimeChange() {
        if (objBLS010Conts.C_ISSUE_INV_TIME_AT_START == $("#IssueInvTime").val()) {
            $("#IssueInvMonth").val("0");
            $("#IssueInvMonth").attr("readonly", true);
        }
        else {
            $("#IssueInvMonth").val("1");
            $("#IssueInvMonth").attr("readonly", false);
        }
    }
    function InitialComboValue(step, CustTypeCode) {
        if (step == 1) {
            $("#IssueInvTime").val(objBLS010Conts.C_ISSUE_INV_TIME_BEFORE);
            $("#IssueInvDate").val(objBLS010Conts.C_ISSUE_INV_DATE_01);
            $("#InvFormatType").val(objBLS010Conts.C_INV_FORMAT_FOLLOW_BILLING);
            //$("#SignatureType").val(objBLS010Conts.C_SIG_TYPE_NO);
            //$("#SignatureType").val(objBLS010Conts.C_SIG_TYPE_HAVE); //Modify by Jutarat A. on 16102013
            $("#SeparateInvType").val(objBLS010Conts.C_SEP_INV_EACH_CONTRACT);
            //$("#DocLanguage").val(objBLS010Conts.C_DOC_LANGUAGE_TH);
            $("#ShowDueDate").val(objBLS010Conts.C_SHOW_DUEDATE_7);
            //$("#IssueReceiptTiming").val(objBLS010Conts.C_ISSUE_REC_TIME_AFTER_PAYMENT);
            $("#ShowAccType").val(objBLS010Conts.C_SHOW_BANK_ACC_SHOW);
            //$("#WhtDeductionType").val(objBLS010Conts.C_DEDUCT_TYPE_NOT_DEDUCT);
            $("#ShowIssueDate").val(objBLS010Conts.C_SHOW_ISSUE_DATE_CHRISTIAN);
            //$("#BillingOfficeCode").val(objBLS010.BillingOfficeCboValue);

            $("#IssueInvMonth").val("1");

            $("#BillingOfficeCode").val(objBLS010.BillingOfficeCboValue); //Modify by Jirawat Jannet @ 2016-08-16
            $("#SignatureType").val(objBLS010Conts.C_SIG_TYPE_HAVE); //Modify by Jirawat Jannet @ 2016-08-16
            $("#DocLanguage").val(objBLS010Conts.C_DOC_LANGUAGE_TH); //Modify by Jirawat Jannet @ 2016-08-16
            $("#IssueReceiptTiming").val(objBLS010Conts.C_ISSUE_REC_TIME_AFTER_PAYMENT); //Modify by Jirawat Jannet @ 2016-08-16


            if (CustTypeCode == '1' || CustTypeCode == '3')
                $("#ShowInvWHTFlag").attr("checked", false);
            else
                $("#ShowInvWHTFlag").attr("checked", true);
            //$("#ShowInvWHTFlag").attr("checked", true);
        }
        else if(step == 0) { 
             $("#IssueInvTime").val();
            $("#IssueInvDate").val();
            $("#InvFormatType").val();
            $("#SignatureType").val();
            $("#SeparateInvType").val();
            $("#DocLanguage").val();
            $("#ShowDueDate").val();
            $("#IssueReceiptTiming").val();
            $("#ShowAccType").val();
            $("#WhtDeductionType").val();
            $("#ShowIssueDate").val();
            $("#BillingOfficeCode").val();
            // Akat K. 2014-05-21 new combobox
            $("#PrintAdvanceDate").val();

            $("#IssueInvMonth").val("1");
            $("#ShowInvWHTFlag").attr("checked", false);
        }
    }

    function setDisabledControlForViewMode(flag) {
        $("#BillingClientCodeView").attr("readonly", flag);
        $("#CustTypeName").attr("readonly", flag);
        $("#CompanyTypeName").attr("readonly", flag);
        $("#FullNameEN").attr("readonly", flag);
        $("#BranchNameEN").attr("readonly", flag);
        $("#AddressEN").attr("readonly", flag);
        $("#FullNameLC").attr("readonly", flag);
        $("#BranchNameLC").attr("readonly", flag);
        $("#AddressLC").attr("readonly", flag);
        $("#Nationality").attr("readonly", flag);
        $("#PhoneNo").attr("readonly", flag);
        $("#IDNo").attr("readonly", flag);
        $("#BusinessTypeName").attr("readonly", flag);
    }