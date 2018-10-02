/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/control_events.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>
/// <reference path="../../Scripts/Base/DateTimePicker.js"/>

/// <reference path="../Base/object/ajax_method.js" />
/// <reference path="../Base/object/command_event.js" />
/// <reference path="../Base/object/master_event.js" />
/// <reference path="../number-functions.js" />

var paymentGrid;
var paymentConfirmGrid;

var ICS010 = {
    ScreenMode: {
        Register: "RegisterMode",
        Confirm: "ConfirmMode",
        Finish: "FinishMode"
    }
};
var IsHaveReceipt;

$(document).ready(function () {
    //Call Only one
    InitialPage();
    InitialGrid();
    InitialBindingEvent();
    onChangePaymentDate();
    //Start Screen Mode
    SetScreenMode(ICS010.ScreenMode.Register);
});

//payment change
function onChangePaymentDate() {
    $("#PaymentDateNull").change(function () {
       
        var ctxt = $("#PaymentDateNull").val();
        var instance = $("#PaymentDateNull").data("datepicker");
        if (instance.selectedDay == 0
            && instance.selectedMonth == 0
            && instance.selectedYear == 0) {
            $("#PaymentDateNull").datepicker("setDate", ctxt);
        }

        var ddate = instance.selectedDay;
        if (ddate < 10)
            ddate = "0" + ddate;
        var dmonth = instance.selectedMonth + 1;
        if (dmonth < 10)
            dmonth = "0" + dmonth;
        var dyear = instance.selectedYear;

        dyear = dyear.toString().substr(2, 2);
        var txt = "" + dyear + dmonth + ddate;
        $("#GroupNameDate").val(txt);

    });
}


//UI Common
function InitialPage() {
    $("#PaymentDateNull").InitialDate();
    $("#PaymentAmount").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#Memo").SetMaxLengthTextArea(100);

    $("#PromissoryNoteDate").InitialDate();

    //InitialNumericInputTextBox(["PayerBankAccNo1", "PayerBankAccNo2", "PayerBankAccNo3", "PayerBankAccNo4"],false); // Comment by Jirawat Jannet
    InitialNumericInputTextBox(["PayerBankAccNo"], false);// Add by Jirawat Jannet
}

//Grid
function InitialGrid() {
    if ($.find("#PaymentGrid").length > 0) {
        paymentGrid = $("#PaymentGrid").InitialGrid(0, false, "/Income/ICS010_InitialPaymentGrid");
        BindingGridEvent();
    }
    if ($.find("#PaymentConfirmGrid").length > 0) {
        paymentConfirmGrid = $("#PaymentConfirmGrid").InitialGrid(0, false, "/Income/ICS010_InitialPaymentConfirmGrid");
    }
}

function RemovePayment(rowId) {
    obj = { rowIndex: paymentGrid.getRowIndex(rowId) }
    ajax_method.CallScreenController("/Income/ICS010_RemovePaymetList", obj, function (result, controls, isWarning) {
        if (result != undefined) {
            if (result == "1") {
                $("#PaymentGrid").LoadDataToGrid(paymentGrid, 0, false, "/Income/ICS010_GetPaymentList", "", "tbt_Payment", false);
                BindingGridEvent();
            }
        }
        else if (controls != undefined) {
            VaridateCtrl(controls, controls);
        }
    });
}


//Binding Event
function InitialBindingEvent() {
    $("#PaymentType").change(PaymentTypeChanged);
    $("#btnRetrieve").click(RetrieveReceiptData);
    $("#btnClear").click(ClearReceiveData);

    $("#btnAdd").click(Add);
    $("#btnCancel").click(ClearForm);
    $("#SendingBankCode").change(SendingBankChanged);
    $("#btnNextToRegisterPayment").click(NextToRegisterPayment);
}


function RetrieveReceiptData() {
    IsHaveReceipt = false;
    //Retrieve Receipt Data
    var obj = {
        ReceiptNo: $("#RefAdvanceReceiptNo").val(),
        PaymentType: $("#PaymentType").val(),
        Payment: {
            PaymentType: $("#PaymentType").val(),
            RefAdvanceReceiptNo: $.trim($("#RefAdvanceReceiptNo").val())
        }
    };
    ajax_method.CallScreenController("/Income/ICS010_GetReceipt", obj, function (result, controls) {
        if (result != undefined) {
            //Success
            IsHaveReceipt = true;
            $("#RefAdvanceReceiptNo").val(result.ReceiptNo);
            var newVal = (new Number(result.ReceiptAmount)).numberFormat("#,##0.00");
            $("#RefAdvanceReceiptAmount").val(newVal);
            $("#RefAdvanceReceiptAmount").NumericCurrency().val(result.ReceiptAmountCurrencyType);
            $("#RefInvoiceNo").val(result.InvoiceNo);
            $("#RefInvoiceOCC").val(result.InvoiceOCC);
            $("#Payer").val(result.BillingClientName);

            $("#RefAdvanceReceiptNo").attr("readonly", true);
            $("#btnRetrieve").attr("disabled", true);
            $("#btnClear").removeAttr("disabled");
        }
        else if (controls != undefined) {
            VaridateCtrl(controls, controls);

            //Clear input
            $("#RefAdvanceReceiptAmount").val("");
            $("#RefAdvanceReceiptAmount").NumericCurrency().val('1');
            $("#RefInvoiceNo").val("");
            $("#RefInvoiceOCC").val("");
            $("#Payer").val("");
        }
    });
}

function ClearReceiveData() {
    IsHaveReceipt = false;
    $("#RefAdvanceReceiptNo").val("");
    $("#RefAdvanceReceiptAmount").val("");
    $("#RefAdvanceReceiptAmount").NumericCurrency().val('1');
    $("#RefInvoiceNo").val("");
    $("#RefInvoiceOCC").val("");
    $("#Payer").val("");

    $("#RefAdvanceReceiptNo").removeAttr("readonly");
    $("#btnRetrieve").removeAttr("disabled");
    $("#btnClear").attr("disabled", true);
}

function BindingGridEvent() {
    //Binding
    SpecialGridControl(paymentGrid, ["Remove"]);
    BindOnLoadedEvent(paymentGrid, function () {
        var colInx = paymentGrid.getColIndexById('Remove');
        
        for (var i = 0; i < paymentGrid.getRowsNum(); i++) {
            var rowId = paymentGrid.getRowId(i);
            GenerateRemoveButton(paymentGrid, "btnRemove", rowId, "Remove", true);

            // binding grid button event 
            BindGridButtonClickEvent("btnRemove", rowId, RemovePayment);
        }
        paymentGrid.setSizes();
    });
}

function Add() {
//    if ($("#GroupNameInput").val() == "") {
//        $("#GroupNameInput_Require").show();
//        return false;
//    }
//    else {
//        $("#GroupNameInput_Require").hide();
//    
//    }
   
    var obj = GetInputPaymentData();


    ajax_method.CallScreenController("/Income/ICS010_AddPayment", obj, function (result, controls, isWarning) {
        if (result != undefined) {
            if (result == "1") {
                $("#PaymentGrid").LoadDataToGrid(paymentGrid, 0, false, "/Income/ICS010_GetPaymentList", "", "tbt_Payment", false);
                ClearForm();
            }
        }
        else if (controls != undefined) {
           
            VaridateCtrl(controls, controls);
        }
    });
}

function GetInputPaymentData() {
    var receiptNo = "";
    var receiptAmount = "";
    var receiptAmountCurrencyType = '';
    var invoiceNo = "";
    var invoiceOCC = "";

    var secomBankId = "";
    var secomBankName = "";
    var sendingBankName = "";
    var sendingBranchName = "";
    var payerbankaccno = "";

    if (IsHaveReceipt) {
        receiptNo = $.trim($("#RefAdvanceReceiptNo").val());
        receiptAmount = $("#RefAdvanceReceiptAmount").NumericValue();
        receiptAmountCurrencyType = $("#RefAdvanceReceiptAmount").NumericCurrency().val();
        invoiceNo = $("#RefInvoiceNo").val();
        invoiceOCC = $("#RefInvoiceOCC").val();
    }

    secomBankId = $("#SECOMAccountID").val();
    secomBankName = $("#SECOMAccountID option:selected").text();
    
    if ($('#SendingBankCode').get(0).selectedIndex > 0)
    {   
        sendingBankName =  $("#SendingBankCode option:selected").text();
    }
    if ($('#SendingBranchCode').get(0).selectedIndex > 0)
    {   
        sendingBranchName =  $("#SendingBranchCode option:selected").text();
    }
    
    //No format
    // Comment by Jirawat Jannet
    //payerbankaccno = $.trim($("#PayerBankAccNo1").val()) + $.trim($("#PayerBankAccNo2").val()) + $.trim($("#PayerBankAccNo3").val()) + $.trim($("#PayerBankAccNo4").val());
    // Add by Jirawat Jannet
    payerbankaccno = $.trim($("#PayerBankAccNo").val());
    var MatchRGroupName = "";
    var MatchRGroupNameInput = "";
    if ($("#GroupNameInput").val() == "") {
        MatchRGroupNameInput = "";
    }
    else {
        MatchRGroupNameInput = $.trim($("#GroupNameDate").val()) + $.trim($("#GroupNameInput").val());
    }
    var payment = {
        PaymentType: $("#PaymentType").val(),
        RefAdvanceReceiptNo: receiptNo,
        RefAdvanceReceiptAmount: receiptAmount,
        RefAdvanceReceiptAmountCurrencyType: receiptAmountCurrencyType,
        RefInvoiceNo: invoiceNo,
        RefInvoiceOCC: invoiceOCC,

        SECOMAccountID: secomBankId,
        PaymentAmount: $("#PaymentAmount").NumericValue(),
        PaymentAmountCurrencyType: $("#PaymentAmount").NumericCurrency().val(),
        PaymentDateNull: $("#PaymentDateNull").val(),
        
        DocNo: $("#PromissoryNoteNo").val(),
        DocDate: $("#PromissoryNoteDate").val(),

        Payer: $.trim($("#Payer").val()),
        SendingBankCode: $("#SendingBankCode").val(),
        SendingBranchCode: $("#SendingBranchCode").val(),
        PayerBankAccNo: payerbankaccno,

        TelNo: $("#TelNo").val(),
        Memo: $("#Memo").val(),

        SECOMBankFullName: secomBankName,
        SendingBankName: sendingBankName,
        SendingBranchName: sendingBranchName,

        WHTFlag: !($("#chkNoWHT").is(':checked')),
        MatchRGroupName: MatchRGroupNameInput
    };

    var returnObj =
    {
        Payment: payment
    }

    return returnObj;
}

function ClearForm() {
    $("#divRegisterPayment").clearForm();
    ClearReceiveData();
    CloseWarningDialog();

    PaymentTypeChanged();
}

function NextToRegisterPayment() {
    $("#PaymentGrid").LoadDataToGrid(paymentGrid, 0, false, "/Income/ICS010_cmdClearPayment", "", "tbt_Payment", false
            , function () {
                SetScreenMode(ICS010.ScreenMode.Register);
            }
            , null);
}

function SendingBankChanged() {
    var bankCode = $("#SendingBankCode").val();
    var obj = { bankCode: bankCode };

    ajax_method.CallScreenController("/Income/ICS010_GetBankBranch", obj, function (data) {
        regenerate_combo("#SendingBranchCode", data);
    });
}

function PaymentTypeChanged() {
    var paymentype = $("#PaymentType").val();
    var obj = { paymentType: paymentype };

    ajax_method.CallScreenController("/Income/ICS010_GetSECOMAccount", obj, function (data) {
        regenerate_combo("#SECOMAccountID", data);

        if (paymentype == ICS010_ViewBag.PaymentTypePromissoryNote
            || paymentype == ICS010_ViewBag.PaymentTypeChequePostDate) {
            //Promissory note
            $("#PromissoryNoteNo_Require").show();
            $("#PromissoryNoteDate_Require").show();
            $("#SendingBankCode_Require").show();
            $("#SendingBranchCode_Require").show();
            
        }
        else {
            $("#PromissoryNoteNo_Require").hide();
            $("#PromissoryNoteDate_Require").hide();
            $("#SendingBankCode_Require").hide();
            $("#SendingBranchCode_Require").hide();
           
        }
    });
}


//Button Command
function cmdRegister() {
    ajax_method.CallScreenController("/Income/ICS010_cmdRegister", null, function (result, controls, isWarning) {
        if (result != undefined) {
            if (result == "1") {
                $("#PaymentConfirmGrid").LoadDataToGrid(paymentConfirmGrid, 0, false, "/Income/ICS010_GetPaymentListForConfirm", "", "tbt_Payment", false
                    , function () {
                        paymentConfirmGrid.setColumnHidden(paymentConfirmGrid.getColIndexById("PaymentTransNo"), true);

                        // goto confirm state
                        SetScreenMode(ICS010.ScreenMode.Confirm);
                    }
                    , function (result, controls, isWarning) {
                    });
            }
        }
        else if (controls != undefined) {
            VaridateCtrl(controls, controls);
        }
    });
}

function cmdConfirm() {
    ajax_method.CallScreenController("/Income/ICS010_cmdConfirm", null,
        function (result, controls, isWarning) {
            if (result != undefined) {
                if (result == "1") {
                    $("#PaymentConfirmGrid").LoadDataToGrid(paymentConfirmGrid, 0, false, "/Income/ICS010_GetPaymentListForConfirm", "", "tbt_Payment", false
                    , function (result, controls, isWarning) {
                        paymentConfirmGrid.setColumnHidden(paymentConfirmGrid.getColIndexById("PaymentTransNo"), false);
                        SetFitColumnForBackAction(paymentConfirmGrid, "TempSpan");

                        //Show
                        SetScreenMode(ICS010.ScreenMode.Finish);
                    }
                    , function (result, controls, isWarning) {
                    });
                }
            }
            else if (controls != undefined) {
                VaridateCtrl(controls, controls);
            }
        });
}


function cmdReset() {
    $("#PaymentGrid").LoadDataToGrid(paymentGrid, 0, false, "/Income/ICS010_cmdClearPayment", "", "tbt_Payment", false
            , function () {
                ClearForm();
                SetScreenMode(ICS010.ScreenMode.Register);
            }
            , null);
}

function cmdBack() {
    SetScreenMode(ICS010.ScreenMode.Register);
}

//General Method
function SetScreenMode(mode) {
    if (mode == ICS010.ScreenMode.Register) {
        $("#divInputPayment").show();
        $("#divNextRegister").hide();
        $("#divPaymentGrid").show();
        $("#divPaymentConfirmGrid").hide();
        $("#divRegisterPayment").SetViewMode(false);
        $("#btnClear").attr("disabled", true);

        register_command.SetCommand(cmdRegister);
        reset_command.SetCommand(cmdReset);
        confirm_command.SetCommand(null);
        back_command.SetCommand(null);
    }
    else if (mode == ICS010.ScreenMode.Confirm) {
        $("#divInputPayment").hide();
        $("#divPaymentGrid").hide();
        $("#divPaymentConfirmGrid").show();
        $("#divRegisterPayment").SetViewMode(true);

        register_command.SetCommand(null);
        reset_command.SetCommand(null);
        confirm_command.SetCommand(cmdConfirm);
        back_command.SetCommand(cmdBack);
    }
    else if (mode == ICS010.ScreenMode.Finish) {
        $("#divNextRegister").show();
        $("#divNextRegister").SetViewMode(false);

        register_command.SetCommand(null);
        reset_command.SetCommand(null);
        confirm_command.SetCommand(null);
        back_command.SetCommand(null);
    }
}