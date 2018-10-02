

//  jQuery
/// <reference path="../../Scripts/jquery-1.5.1-vsdoc.js"/>

// dhtmlx grid
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxgrid.js"/>
/// <reference path = "../../Content/js/dhtmlxgrid/ext/dhtmlxgrid_data.js"/>
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxgridcell.js"/>
/// <reference path = "../../Content/js/dhtmlxgrid/ext/dhtmlxgrid_pgn.js"/>
/// <reference path = "../../Scripts/Base/Master.js" />
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Common/Dialog.js"/>
/// <reference path="../../Scripts/Billing/Dialog.js"/>
/// <reference path="../../Scripts/json2.js" />


/// <reference path = "../../Scripts/Base/GridControl.js" />

var pageRow = 0;
var BLS030_GridBillingType;



// Main
$(document).ready(function () {

    initialGridOnload();
    $("#btnSearchBillingTarget").click(function () {
        $("#dlgBLS030").OpenCMS470Dialog("BLS030");
    });
    $("#btnRetrieveBilling").click(retrieve_billing_click);
    $("#btnAutoTransferInfo").click(function () { $("#dlgBLS031").OpenBLS031Dialog("BLS031"); });
    $("#btnCreditCardInfo").click(function () { $("#dlgBLS032").OpenBLS032Dialog("BLS032"); });
    $("#btnAddBillingType").click(add_billing_type_click);
    $("#PaymentMethod").change(change_payment_method);


    SpecialGridControl(BLS030_GridBillingType, ["Remove"]);

    BindOnLoadedEvent(BLS030_GridBillingType, function () {
        var colInx = BLS030_GridBillingType.getColIndexById('Remove');
        for (var i = 0; i < BLS030_GridBillingType.getRowsNum(); i++) {
            var rowId = BLS030_GridBillingType.getRowId(i);
            GenerateRemoveButton(BLS030_GridBillingType, "btnRemoveEmail", rowId, "Remove", true);

            // binding grid button event 
            BindGridButtonClickEvent("btnRemoveEmail", rowId, BtnRemoveBillTypeClick);

        }
        BLS030_GridBillingType.setSizes();
    });
    enabledGridBillingType();



    BLS030_GridBillingType.enableAutoHeight(true);




    // Narupon
    if (BLS030_ViewBag != undefined &&
    ////BLS030_ViewBag.ContractProjectCode == "" &&
            BLS030_ViewBag.BillingClientCode != "" &&
            BLS030_ViewBag.BillingTargetRunningNo != ""
        ) {
        setInitialState();

        ////$("#ContractCodeProjectCode").val(BLS030_ViewBag.ContractProjectCode);
        $("#BillingTargetCode").val(BLS030_ViewBag.BillingClientCode);
        $("#BillingTargetRunningNo").val(BLS030_ViewBag.BillingTargetRunningNo)



    }
    else {
        setInitialState();
    }

    $("#VATUnchargedFlag").attr("disabled", true);
    $('#PaymentMethod').val("1");
});

function change_payment_method() {
    var paymentMethod = $("#PaymentMethod").val();
    if (paymentMethod == C_PAYMENT_METHOD_AUTO_TRANSFER) {
        $("#divAutoTransferInformation").show();
        $("#divCreditCardInformation").hide()
    }
    else if (paymentMethod == C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER) {
        $("#divAutoTransferInformation").hide();
        $("#divCreditCardInformation").show()
    }
    else {
        $("#divAutoTransferInformation").hide();
        $("#divCreditCardInformation").hide()
    }
}



function BLS031Object() {
    var obj = {
        ContractCode: $("#ContractCodeProjectCode").val()
            , BillingClientCode: $("#BillingTargetCode").val()
            , BillingClientNameEN: $("#FullNameEN").val()
            , BillingClientNameLC: $("#FullNameLC").val()
            , BillingOCC: $("#BillingOCC").val()
            , BankCode: $("#BankCode").val()
            , BankBranchCode: $("#BankBranchCode").val()
            , AccountNo: $("#AccountNo").val()
            , AccountName: $("#AccountName").val()
            , AccountType: $("#AccountType").val()
            , AutoTransferDate: $("#AutoTransferDate").val()
            , LastestResult: $("#LastestResult").val()
    };
    return { "doAutoTransfer": obj };
}

function BLS032Object() {

    var strMonth = $("#ExpMonth").val();
    if ($("#ExpMonth").val() < 10) {
        strMonth = strMonth.substring(1);
    }

    var obj = {
        ContractCode: $("#ContractCodeProjectCode").val()
            , BillingClientCode: $("#BillingTargetCode").val()
            , BillingClientNameEN: $("#FullNameEN").val()
            , BillingClientNameLC: $("#FullNameLC").val()
            , BillingOCC: $("#BillingOCC").val()
            , CreditCardCompanyCode: $("#CreditCardCompanyCode").val()
            , CreditCardType: $("#CreditCardType").val()
            , CreditCardNo: $("#CreditCardNo").val()
            , ExpMonth: strMonth
            , ExpYear: $("#ExpYear").val()
            , CardName: $("#CardName").val()
    };
    return { "doCredit": obj };
}

function BLS031Response(result) {

    $("#dlgBLS031").CloseDialog();

    var strAutoTransferDate = result.AutoTransferDate;
    if (strAutoTransferDate == "1") {
        strAutoTransferDate = strAutoTransferDate + "st";
    }
    else if (strAutoTransferDate == "2") {
        strAutoTransferDate = strAutoTransferDate + "nd";
    }
    else if (strAutoTransferDate == "3") {
        strAutoTransferDate = strAutoTransferDate + "rd";
    }
    else {
        strAutoTransferDate = strAutoTransferDate + "th";
    }

    $("#AccountName").val(result.AccountName);
    $("#BankName").val(result.BankName);
    $("#BankBranchName").val(result.BankBranchName);
    $("#AccountTypeName").val(result.AccountTypeName);
    $("#AccountNo").val(result.AccountNo);

    $("#AutoTransferDate").val(result.AutoTransferDate);
    $("#AutoTransferDateForView").val(strAutoTransferDate);


    //$("#LastestResult").val(result.LastestResult);

    $("#BankCode").val(result.BankCode);
    $("#BankBranchCode").val(result.BankBranchCode);
    $("#AccountType").val(result.AccountType);

}

function BLS032Response(result) {

    $("#dlgBLS032").CloseDialog();

    var strExpiredMonth = result.ExpMonth;
    if (result.ExpMonth < 10) {
        strExpiredMonth = "0" + result.ExpMonth;
    }

    $("#CreditCardTypeName").val(result.CreditCardTypeName);
    $("#CardName").val(result.CardName);
    $("#CreditCardCompanyName").val(result.CreditCardCompanyName);
    $("#CreditCardNo").val(result.CreditCardNo);
    $("#ExpMonth").val(strExpiredMonth);
    $("#ExpYear").val(result.ExpYear);
    $("#CardHolderName").val(result.CardName);
    $("#CreditCardType").val(result.CreditCardType);
    $("#CreditCardCompanyCode").val(result.CreditCardCompanyCode);
    $("#CardName").val(result.CardName);

}

function initialGridOnload() {
    BLS030_GridBillingType = $("#gridBillingType").InitialGrid(pageRow, false, "/Billing/BLS030_InitialGridBillingType");
}


function BLS030_ClearValueHiddenFiled() {
    $("#AutoTransferDate").val("");
    $("#BankCode").val("");
    $("#BankBranchCode").val("");
    $("#AccountType").val("");
    $("#CreditCardType").val("");
    $("#CreditCardCompanyCode").val("");
}

function retrieve_billing_click() {

    $("#ContractCodeProjectCode").val($("#ContractCodeProjectCode").val().toUpperCase());
    $("#BillingTargetCode").val($("#BillingTargetCode").val().toUpperCase());
    $("#BillingTargetRunningNo").val($("#BillingTargetRunningNo").val().toUpperCase());


    var obj = { ContractProjectCodeShort: $("#ContractCodeProjectCode").val(),
        BillingClientCode: $("#BillingTargetCode").val(),
        BillingTargetRunningNo: $("#BillingTargetRunningNo").val()
    };

    ajax_method.CallScreenController("/Billing/BLS030_RetrieveData", obj, function (result, controls) {
        if (controls != undefined) {

            VaridateCtrl(["ContractCodeProjectCode", "BillingTargetCode", "BillingTargetRunningNo"], controls);

            return;
        }
        else if (result != undefined && result.doBillingTarget != null) {


            //var obj = { ProductTypeCode: "" };
            if (result.ProductTypeCode != undefined) {
                //obj.ProductTypeCode = result.ProductTypeCode;
                ajax_method.CallScreenController('/Billing/BLS030_GetBillingType', null, function (result, controls) {
                    if (result.List != null) {
                        regenerate_combo("#BillingTypeList", result);
                    }
                });
            }
            else {

            }


            $("#divBillingTargetForView").clearForm();
            $("#divAutoTransferInformation").clearForm();
            $("#divCreditCardInformation").clearForm();
            BLS030_ClearValueHiddenFiled();

            $("#divBillingTargetForView").bindJSON(result.doBillingTarget);
            $("#BillingOfficeCode").val(result.doBillingTarget.BillingOfficeCode + ": " + result.doBillingTarget.OfficeName);
            $("#DebtTracingOffice").val(result.doBillingTarget.BillingOfficeCode + ": " + result.doBillingTarget.OfficeName);
            $("#BillingTargetCodeView").val(result.doBillingTarget.BillingTargetCode_Short);

            $("#BillingTypeList").attr("disabled", false);
            $("#btnAddBillingType").attr("disabled", false);
            $("#PaymentMethod").attr("disabled", false);
            $("#SortingType").attr("readonly", false);

            $("#ContractCodeProjectCode").attr("readonly", true);
            $("#BillingTargetCode").attr("readonly", true);
            $("#BillingTargetRunningNo").attr("readonly", true);
            $("#btnRetrieveBilling").attr("disabled", true);
            $("#btnSearchBillingTarget").attr("disabled", true);

            InitialCommandButton(1);
            change_payment_method();
        }
        //else {  // comment by Narupon W.
        //    setInitialState();
        //}



    });

}

function setInitialState() {
    // --------------- Initial SCREEN ------------------
    //Set readonly control to support style on confirm state
    setReadOnlyControlForViewMode(true);

    $("#divBillingBasicInfo").SetViewMode(false);
    $("#divAutoTransferInformation").SetViewMode(false);
    $("#divCreditCardInformation").SetViewMode(false);

    InitialCommandButton(0);

    $("#divBillingBasicInfo").clearForm();
    $("#divAutoTransferInformation").clearForm();
    $("#divCreditCardInformation").clearForm();
    //--------------------------------------------------
    $("#ContractCodeProjectCode").attr("readonly", false);
    $("#BillingTargetCode").attr("readonly", false);
    $("#BillingTargetRunningNo").attr("readonly", false);
    $("#btnRetrieveBilling").attr("disabled", false);
    $("#btnSearchBillingTarget").attr("disabled", false);

    $("#BillingTypeList").attr("disabled", true);
    $("#btnAddBillingType").attr("disabled", true);
    $("#PaymentMethod").attr("disabled", true);
    $("#SortingType").attr("readonly", true);


    $("#divBillingBasicInfo").show();
    $("#divAutoTransferInformation").hide();
    $("#divCreditCardInformation").hide()


    $("#VATUnchargedFlag").attr("disabled", true);

}





function clear_installation_click() {

    // Get Message
    var obj = {
        module: "Common",
        code: "MSG0044"
    };

    ajax_method.CallScreenController("/Shared/GetMessage", obj, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message, clearAllScreen, function () {

        });

    });

}

function clearAllScreen() {
    setInitialState();

}


function BtnRemoveBillTypeClick(row_id) {


    var selectedRowIndex = BLS030_GridBillingType.getRowIndex(row_id);
    var BillingType = BLS030_GridBillingType.cells2(selectedRowIndex, BLS030_GridBillingType.getColIndexById('BillingTypeCode')).getValue();
    var obj = { BillingTypeCode: BillingType }
    DeleteRow(BLS030_GridBillingType, row_id);
    ajax_method.CallScreenController("/Billing/BLS030_RemoveBillingTypeClick", obj, function (result, controls, isWarning) {


    });

}

function BtnRegisterClick() {
    var registerData_obj = {};
}



function BtnClearClick() {
    $("#EmailAddress").val("");

}



function InitialCommandButton(step) {
    if (step == 0) {
        SetRegisterCommand(false, null);
        SetResetCommand(false, null);
        SetApproveCommand(false, null);
        SetRejectCommand(false, null);
        SetReturnCommand(false, null);
        SetCloseCommand(false, null);
        SetConfirmCommand(false, null);
        SetBackCommand(false, null);
    }
    else if (step == 1) {
        SetRegisterCommand(true, command_register_click);
        SetResetCommand(true, command_reset_click);
        SetApproveCommand(false, null);
        SetRejectCommand(false, null);
        SetReturnCommand(false, null);
        SetCloseCommand(false, null);
        SetConfirmCommand(false, null);
        SetBackCommand(false, null);
    }
    else if (step == 2) {
        SetRegisterCommand(false, null);
        SetResetCommand(false, null);
        SetApproveCommand(false, null);
        SetRejectCommand(false, null);
        SetReturnCommand(false, null);
        SetCloseCommand(false, null);
        SetConfirmCommand(true, command_confirm_click);
        SetBackCommand(true, command_back_click);
    }
    else if (step == 3) {
        SetRegisterCommand(false, null);
        SetResetCommand(false, null);
        SetApproveCommand(false, null);
        SetRejectCommand(false, null);
        SetReturnCommand(false, null);
        SetCloseCommand(false, null);
        SetConfirmCommand(false, null);
        SetBackCommand(false, null);
    }
    else if (step == 4) {
        SetRegisterCommand(false, null);
        SetResetCommand(false, null);
        SetApproveCommand(true, command_approve_click);
        SetRejectCommand(true, command_reject_click);
        SetReturnCommand(true, command_return_click);
        SetCloseCommand(true, command_close_click);
        SetConfirmCommand(false, null);
        SetBackCommand(false, null);
    }
}

function command_back_click() {

    $("#BillingTypeInput").show();

    InitialCommandButton(1);

    //Set readonly control to support style on confirm state
    setReadOnlyControlForViewMode(true);

    $("#divBillingBasicInfo").SetViewMode(false);
    $("#divAutoTransferInformation").SetViewMode(false);
    $("#divCreditCardInformation").SetViewMode(false);

    enabledGridBillingType();
}

function command_register_click() {
    command_control.CommandControlMode(false);


    var obj = CreateObjectData($("#form1").serialize() + "&" + $("#form2").serialize() + "&" + $("#form3").serialize() + "&ContractProjectCodeShort=" + $("#ContractCodeProjectCode").val() + "&BillingClientCode=" + $("#BillingTargetCode").val() + "&BillingTargetRunningNo=" + $("#BillingTargetRunningNo").val());

    ajax_method.CallScreenController("/Billing/BLS030_ValidateBeforeRegister", obj, function (result, controls, isWarning) {
        command_control.CommandControlMode(true);
        if (controls != undefined) {
            /* --- Higlight Text --- */
            /* --------------------- */
            VaridateCtrl(["ContractCodeProjectCode"], controls);
            /* --------------------- */

            return;
        }
        else if (result == true && isWarning == undefined) {

            setConfirmState();
        }


    });
}



function setConfirmState() {
    InitialCommandButton(2);

    $("#BillingTypeInput").hide();

    //Set readonly control to support style on confirm state
    setReadOnlyControlForViewMode(false);

    $("#divBillingBasicInfo").SetViewMode(true);
    $("#divAutoTransferInformation").SetViewMode(true);
    $("#divCreditCardInformation").SetViewMode(true);

    disabledGridBillingType();

}

function setSuccessRegisState() {
    // --------------- Initial SCREEN ------------------

    $("#BillingTypeInput").show();

    //Set readonly control to support style on confirm state
    setReadOnlyControlForViewMode(true);

    $("#divBillingBasicInfo").SetViewMode(false);
    $("#divAutoTransferInformation").SetViewMode(false);
    $("#divCreditCardInformation").SetViewMode(false);

    InitialCommandButton(0);
    setInitialState();

    DeleteAllRow(BLS030_GridBillingType);

    $("#VATUnchargedFlag").attr("disabled", true);

    // tt
    enabledGridBillingType();
}

function command_confirm_click() {
    command_control.CommandControlMode(false);

    ajax_method.CallScreenController("/Billing/BLS030_RegisterData", null, function (result, controls, isWarning) {
        command_control.CommandControlMode(true);
        if (controls != undefined) {
            /* --- Higlight Text --- */
            /* --------------------- */
            VaridateCtrl(["ContractCodeProjectCode"], controls);
            /* --------------------- */
            return;
        }
        else if (result != undefined && isWarning == undefined) {


            var obj = {
                module: "Common",
                code: "MSG0046",
                param: ""
            };

            ajax_method.CallScreenController("/Shared/GetMessage", obj, function (result) {
                //OpenWarningDialog(result.Message, result.Message, null);
                OpenInformationMessageDialog(result.Code, result.Message);
            });


            setSuccessRegisState();
        }
    });


}

function command_reset_click() {

    var obj = {
        module: "Common",
        code: "MSG0038"
    };

    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {

        OpenYesNoMessageDialog(result.Code, result.Message, function () {

            setInitialState();
            DeleteAllRow(BLS030_GridBillingType);

            $('#PaymentMethod').val("1");
        });

    });
}



function convertDatetoYMD(ctrl) {
    var ctxt = ctrl.val();
    if (ctxt != "") {
        var instance = ctrl.data("datepicker");
        var ddate = instance.selectedDay;
        if (ddate < 10)
            ddate = "0" + ddate;
        var dmonth = instance.selectedMonth + 1;
        if (dmonth < 10)
            dmonth = "0" + dmonth;
        var dyear = instance.selectedYear;

        var txt = "" + dyear + dmonth + ddate;
        return txt;
    }
}

function getCurrentDateFormatYMD() {
    var myNow = new Date();
    var ddate = myNow.getDate();
    if (ddate < 10)
        ddate = "0" + ddate;
    var dmonth = myNow.getMonth() + 1;
    if (dmonth < 10)
        dmonth = "0" + dmonth;
    var dyear = myNow.getFullYear();

    var txt = "" + dyear + dmonth + ddate;
    return txt;
}

function disabledGridBillingType() {
    //////// DISABLED BUTTON In EMAIL GRID ///////////
    colInx = BLS030_GridBillingType.getColIndexById("Remove")
    BLS030_GridBillingType.setColumnHidden(colInx, true);

    //////////////////////////////////////////////////
}

function enabledGridBillingType() {
    colInx = BLS030_GridBillingType.getColIndexById("Remove")
    BLS030_GridBillingType.setColumnHidden(colInx, false);

    SetFitColumnForBackAction(BLS030_GridBillingType, "TempColumn");
    //////////////////////////////////////////////////
}

function doAlert(moduleCode, msgCode, paramObj) {
    var obj = {
        module: moduleCode,
        code: msgCode,
        param: paramObj
    };

    ajax_method.CallScreenController("/Shared/GetMessage", obj, function (result) {
        OpenWarningDialog(result.Message, result.Message, null);
    });
}

function add_billing_type_click() {

    var obj = {
        BillingTypeCode: $("#BillingTypeList").val()
    };

    ajax_method.CallScreenController("/Billing/BLS030_AddBillingType", obj, function (result, controls) {
        if (controls != undefined || result == null) {

        }
        else if (result.length > 0) {
            var BillingTypeLst = [result[0].BillingTypeCode + ": " + result[0].BillingTypeName + "<br />  ",
                                "(1) " + result[0].BillingTypeNameEN + "<br /> (2) " + result[0].BillingTypeNameLC,
                                "",
                                "",
                                result[0].BillingTypeCode];

            CheckFirstRowIsEmpty(BLS030_GridBillingType, true);
            AddNewRow(BLS030_GridBillingType, BillingTypeLst);

            var colInx = BLS030_GridBillingType.getColIndexById('Remove');
            var rowId = BLS030_GridBillingType.getRowId(BLS030_GridBillingType.getRowsNum() - 1);
            GenerateRemoveButton(BLS030_GridBillingType, "btnRemoveEmail", rowId, "Remove", true);
            // binding grid button event 
            BindGridButtonClickEvent("btnRemoveEmail", rowId, BtnRemoveBillTypeClick);



            //            //=========== Protect Scrollbar appear ===================================
            //            BLS030_GridBillingType.setColumnHidden(colInx, true);
            //            SetFitColumnForBackAction(BLS030_GridBillingType, "TempColumn");
            //            BLS030_GridBillingType.setColumnHidden(colInx, false);
            //            //=======================================================================

            BLS030_GridBillingType.setSizes();
        }
    });


    $("#BillingTypeList").val("");


}

function CMS470Response(result_BillingTarget) {
    //var obj = { billingClientData: result };
    var obj = result_BillingTarget;
    $("#dlgBLS030").CloseDialog();
    if (obj == null) {
        return;
    }
    else {
        var str = obj.BillingTargetCode_Short.split("-")
        if (str.length == 2) {
            $("#BillingTargetCode").val(str[0]);
            $("#BillingTargetRunningNo").val(str[1]);
        }
    }


}

function setReadOnlyControlForViewMode(flag) {
    //Set readonly control to support style on confirm state

    //Billing basic information
    $("#ContractCodeProjectCode").attr("readonly", flag);
    $("#BillingTargetCode").attr("readonly", flag);
    $("#BillingTargetRunningNo").attr("readonly", flag);

    $("#BillingOfficeCode").attr("readonly", flag);
    $("#DebtTracingOffice").attr("readonly", flag);
    $("#BillingTargetCodeView").attr("readonly", flag);
    $("#CustTypeName").attr("readonly", flag);
    $("#FullNameEN").attr("readonly", flag);
    $("#BranchNameEN").attr("readonly", flag);
    $("#AddressEN").attr("readonly", flag);

    $("#FullNameLC").attr("readonly", flag);
    $("#BranchNameLC").attr("readonly", flag);
    $("#AddressLC").attr("readonly", flag);

    //Auto transfer
    $("#AccountName").attr("readonly", flag);
    $("#BankName").attr("readonly", flag);
    $("#BankBranchName").attr("readonly", flag);
    $("#AccountTypeName").attr("readonly", flag);
    $("#AccountNo").attr("readonly", flag);
    $("#AutoTransferDateForView").attr("readonly", flag);
    $("#LastestResultName").attr("readonly", flag);

    //Credit card
    $("#CreditCardTypeName").attr("readonly", flag);
    $("#CardName").attr("readonly", flag);
    $("#CreditCardCompanyName").attr("readonly", flag);
    $("#CreditCardNo").attr("readonly", flag);
    $("#ExpMonth").attr("readonly", flag);
    $("#ExpYear").attr("readonly", flag);
}