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

var ICS084InvoiceGrid;
var ICS084ConfirmMessageList;

var ICS084 = {
    ScreenMode: {
        Register: "RegisterMode",
        Confirm: "ConfirmMode",
        Finish: "FinishMode"
    }
};

$(document).ready(function () {
    //Call Only one
    ICS084InitialPage();
    ICS084InitialBindingEvent();

    ICS084SetScreenMode(ICS084.ScreenMode.Register);
    ICS084InitialGrid();
});


function ICS084InitialGrid() {
    if ($.find("#TargetInvoiceGrid").length > 0) {
        var obj = {
            ScreenCaller: $("#ics084ScreenCaller").val(),
            PaymentTransNo: $("#ics084PaymentTransNo").val(),
            BillingTargetCode: $("#ics084BillingTargetCodeLongFormat").val(),
            InvoiceNo: $("#ics084InvoiceNo").val(),
            FirstPaymentAmountCurrencyType: $('#FirstPaymentAmount').NumericCurrencyValue()
        };
        ICS084InvoiceGrid = $("#TargetInvoiceGrid").LoadDataToGridWithInitial(0, false, false, "/Income/ICS084_GetInvoiceGrid", obj, "doUnpaidInvoice", false);
        ICS084InvoiceGrid.enableTooltips("false,true,true,true,true,true,false,false");
        ICS084BindingGridEvent();
    }
}

function ICS084BindingGridEvent() {
    SpecialGridControl(ICS084InvoiceGrid, ["Select", "MatchPaymentAmount", "WhtAmount","txtKeyInMatchAmountIncWHT", "txtKeyInWHTAmount"]);
    BindOnLoadedEvent(ICS084InvoiceGrid, function () {

        var colSelect = ICS084InvoiceGrid.getColIndexById('SelectInvoice');
        var colMatch = ICS084InvoiceGrid.getColIndexById('KeyInMatchAmountIncWHT');
        var colWht = ICS084InvoiceGrid.getColIndexById('KeyInWHTAmount');
        //var colUnpaidInvoice = ics081resultGrid.getColIndexById('UnpaidInvoice');
        //var colUnpaidDetail = ics081resultGrid.getColIndexById('UnpaidDetail');
        var colIsToMatchableProcess = ICS084InvoiceGrid.getColIndexById('IsToMatchableProcess');

        var selectID = "SelectInvoice";
        var matchID = "KeyInMatchAmountIncWHT";
        var whtID = "KeyInWHTAmount";
        var matchVal;
        var whtval;

        // Add by Jirawat Jannet on 2016-11-14
        // Data for Generate Currency textbox control
        var gntbcDatas = [];


        for (var i = 0; i < ICS084InvoiceGrid.getRowsNum(); i++) {
            var row_id = ICS084InvoiceGrid.getRowId(i);

            // Gen Checkbox
            ICS084InvoiceGrid.cells(row_id, colSelect).setValue(GenerateCheckBox(selectID, row_id, "checked", true));


            if (ICS084InvoiceGrid.cells(row_id, colIsToMatchableProcess).getValue() == true) {
                BindGridCheckBoxClickEvent(selectID, row_id, function (rid, checked) {
                    selectTargetInvoiceChange(rid, checked);
                });
            }
            else {
                $("#" + GenerateGridControlID(selectID, row_id)).SetDisabled(true);
            }


            // Gen numeric text box
            matchVal = GetValueFromLinkType(ICS084InvoiceGrid, i, colMatch);
            // Comment by Jirawat Jannet on 2016-10-27
            //GenerateNumericBox2(ICS084InvoiceGrid, "txtKeyInMatchAmountIncWHT", row_id, matchID, matchVal, 10, 2, 0, 999999999999.99, false, false);
            // Add by Jirawat Jannet on 2016-10-27
            // Comment by Jirawat Jannet on 2016-11-14 : Cause too slow render
            //GenerateNumericTextboxWithCurrency(ICS084InvoiceGrid, "txtKeyInMatchAmountIncWHT", row_id, matchID, matchVal, ICS084ScreenObject.C_CURRENCY_LOCAL);

            whtval = GetValueFromLinkType(ICS084InvoiceGrid, i, colWht);
            // Comment by Jirawat Jannet on 2016-10-27
            //GenerateNumericBox2(ICS084InvoiceGrid, "txtKeyInWHTAmount", row_id, whtID, whtval, 10, 2, 0, 999999999999.99, false, false);
            // Add by Jirawat Jannet on 2016-10-27
            // Comment by Jirawat Jannet on 2016-11-14 : Cause too slow render
            //GenerateNumericTextboxWithCurrency(ICS084InvoiceGrid, "txtKeyInWHTAmount", row_id, whtID, whtval, ICS084ScreenObject.C_CURRENCY_LOCAL);

            // Add by Jirawat Jannet on 2016-11-14
            var gntbcData = {
                rowId: row_id,
                id1: 'txtKeyInMatchAmountIncWHT',
                id2: 'txtKeyInWHTAmount',
                colId1: matchID,
                colId2: whtID,
                val1: matchVal,
                val2: whtval,
                currency1: ICS084ScreenObject.C_CURRENCY_LOCAL,
                currency2: ICS084ScreenObject.C_CURRENCY_LOCAL,
                enabled: false
            }
            // Add by Jirawat Jannet
            gntbcDatas.push(gntbcData);
        }
        // Add by Jirawat Jannet
        GenerateNumericTexboxWithCurrencyControls(ICS084InvoiceGrid, gntbcDatas);
        ICS084InvoiceGrid.setSizes();
    });
}

// Add by Jirawat Jannet on 2016-11-14
String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.replace(new RegExp(search, 'g'), replacement);
};

// Add by Jirawat Jannet on 2016-11-14
// Genereate multi textbox with currency control
function GenerateNumericTexboxWithCurrencyControls(grid, datas) {

    ajax_method.CallScreenController("/Income/GenerateCurrencyNumericTextBoxControl", null, function (result, controls, isWarning) {
        var txt = result;
        for (var i = 0; i < datas.length; i++) {
            var col1 = grid.getColIndexById(datas[i].colId1);
            var col2 = grid.getColIndexById(datas[i].colId2);
            var fid1 = GenerateGridControlID(datas[i].id1, datas[i].rowId);
            var fid2 = GenerateGridControlID(datas[i].id2, datas[i].rowId);

            var controlTxt1 = txt.replaceAll('{{id}}', fid1).replaceAll('{{value}}', datas[i].val1).replaceAll('{{currency}}', datas[i].currency1);
            var controlTxt2 = txt.replaceAll('{{id}}', fid2).replaceAll('{{value}}', datas[i].val2).replaceAll('{{currency}}', datas[i].currency2);

            grid.cells(datas[i].rowId, col1).setValue(controlTxt1);
            grid.cells(datas[i].rowId, col2).setValue(controlTxt2);

            fid1 = '#' + fid1;
            fid2 = '#' + fid2;

            $(fid1).BindNumericBox(12, 2, 0, 999999999999.99);
            $(fid2).BindNumericBox(12, 2, 0, 999999999999.99);

            var dVal1 = parseFloat($(fid1).val());
            var dVal2 = parseFloat($(fid2).val());

            $(fid1).val(accounting.toFixed(dVal1, 2));
            $(fid1).attr('readonly', !datas[i].enabled);
            $(fid1).NumericCurrency().attr('disabled', !datas[i].enabled);

            $(fid2).val(accounting.toFixed(dVal2, 2));
            $(fid2).attr('readonly', !datas[i].enabled);
            $(fid2).NumericCurrency().attr('disabled', !datas[i].enabled);

            //$(fid1).focus(function () {
            //    grid.selectRowById(datas[i].rowId);
            //});
            //$(fid2).focus(function () {
            //    grid.selectRowById(datas[i].rowId);
            //});
        }
        ICS084InvoiceGrid.setSizes();
    });
}

function GenerateNumericTextboxWithCurrency(grid, id, row_id, col_id, value, currency, enabled) {
    var col = grid.getColIndexById(col_id);
    var fid = GenerateGridControlID(id, row_id);

    var obj = {
        id: fid,
        value: value,
        currency: currency
    };
    ajax_method.CallScreenController("/Income/GenerateCurrencyNumericTextBox", obj, function (result, controls, isWarning) {
        var txt = result;
        grid.cells(row_id, col).setValue(txt);
        fid = "#" + fid;
        $(fid).BindNumericBox(12, 2, 0, 999999999999.99);
        var dVal = parseFloat($(fid).val());

        $(fid).val(accounting.toFixed(dVal, 2));
        $(fid).attr('readonly', !enabled);
        $(fid).NumericCurrency().attr('disabled', !enabled);

        $(fid).focus(function () {
            grid.selectRowById(row_id);
        });
    });
}

function selectTargetInvoiceChange(rid, checked) {
    var colCurrencyType = ICS084InvoiceGrid.getColIndexById('InvoiceAmountCurrencyType');
    var colPaidCurrencyType = ICS084InvoiceGrid.getColIndexById('PaidAmountCurrencyType');
    var currencyType = ICS084InvoiceGrid.cells(rid, colCurrencyType).getValue();
    var paidCurrencyType = ICS084InvoiceGrid.cells(rid, colPaidCurrencyType).getValue();

    var txtKeyInMatchAmountIncWHT_ID = GenerateGridControlID("txtKeyInMatchAmountIncWHT", rid);
    var txtKeyInWHTAmount_ID = GenerateGridControlID("txtKeyInWHTAmount", rid);

    if (checked) {
        //Add by Jutarat A. on 17042013
        var colMatch = ICS084InvoiceGrid.getColIndexById('MatchAmountIncWHT');
        var defaultMatch = SetNumericValue(ICS084InvoiceGrid.cells(rid, colMatch).getValue(), 2);
        $("#" + txtKeyInMatchAmountIncWHT_ID).val(defaultMatch);
        $("#" + txtKeyInMatchAmountIncWHT_ID).setComma(); 
        $("#" + txtKeyInMatchAmountIncWHT_ID).SetReadOnly(false);
        $("#" + txtKeyInMatchAmountIncWHT_ID).ResetToNormalControl();
        $("#" + txtKeyInMatchAmountIncWHT_ID).NumericCurrency().val(paidCurrencyType);
        $("#" + txtKeyInMatchAmountIncWHT_ID).NumericCurrency().SetDisabled(false);


        //if (currencyType == ICS084ScreenObject.C_CURRENCY_LOCAL) {
        //    $("#" + txtKeyInMatchAmountIncWHT_ID).NumericCurrency().SetDisabled(true);
        //} else {
        //    $("#" + txtKeyInMatchAmountIncWHT_ID).NumericCurrency().SetDisabled(false);
        //}
        $("#" + txtKeyInMatchAmountIncWHT_ID).NumericCurrency().SetDisabled(false);

        if ($("#WHTFlag").val() == "False") {
            $("#" + txtKeyInWHTAmount_ID).SetReadOnly(true);
            $("#" + txtKeyInWHTAmount_ID).val("");
            $("#" + txtKeyInWHTAmount_ID).NumericCurrency().SetDisabled(true);
            $("#" + txtKeyInWHTAmount_ID).NumericCurrency().val(currencyType);
        }
        else {
            var colWht = ICS084InvoiceGrid.getColIndexById('WHTAmountDefault');
            var defaultWht = SetNumericValue(ICS084InvoiceGrid.cells(rid, colWht).getValue(), 2);
            $("#" + txtKeyInWHTAmount_ID).val(defaultWht);
            $("#" + txtKeyInWHTAmount_ID).setComma();
            $("#" + txtKeyInWHTAmount_ID).SetReadOnly(false);
            $("#" + txtKeyInWHTAmount_ID).ResetToNormalControl();
            $("#" + txtKeyInWHTAmount_ID).NumericCurrency().val(currencyType);
            $("#" + txtKeyInWHTAmount_ID).NumericCurrency().SetDisabled(false);

            //if (currencyType == ICS084ScreenObject.C_CURRENCY_LOCAL) {
            //    $("#" + txtKeyInWHTAmount_ID).NumericCurrency().SetDisabled(true);
            //} else {
            //    $("#" + txtKeyInWHTAmount_ID).NumericCurrency().SetDisabled(false);
            //}
            $("#" + txtKeyInWHTAmount_ID).NumericCurrency().SetDisabled(false);
        }
    }
    else {
        $("#" + txtKeyInMatchAmountIncWHT_ID).SetReadOnly(true);
        $("#" + txtKeyInMatchAmountIncWHT_ID).NumericCurrency().SetDisabled(true);
        $("#" + txtKeyInWHTAmount_ID).SetReadOnly(true);
        $("#" + txtKeyInWHTAmount_ID).NumericCurrency().SetDisabled(true);
        $("#" + txtKeyInMatchAmountIncWHT_ID).val("");
        $("#" + txtKeyInMatchAmountIncWHT_ID).NumericCurrency().val(ICS084ScreenObject.C_CURRENCY_LOCAL);
        $("#" + txtKeyInWHTAmount_ID).val("");
        $("#" + txtKeyInWHTAmount_ID).NumericCurrency().val(ICS084ScreenObject.C_CURRENCY_LOCAL);
    }
}

function ICS084InitialPage() {
    $("#BankFee").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#OtherExpense").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#OtherIncome").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#ExchangeLoss").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#ExchangeGain").BindNumericBox(12, 2, 0, 999999999999.99);
}

function ICS084InitialBindingEvent() {
    $("#SelectSpecialProcess").change(SpecialProcessChange);
}

function SpecialProcessChange() {
    if ($('#SelectSpecialProcess').is(':checked')) {
        //Enable
        $("#ApproveNo").SetReadOnly(false);
        if ($("#OtherIncomeRegisteredFlag").val() == "False") {
            $("#OtherIncome").SetReadOnly(false);
            $("#OtherIncome").NumericCurrency().SetDisabled(false);
        }
        if ($("#OtherExpenseRegisteredFlag").val() == "False") {
            $("#OtherExpense").SetReadOnly(false);
            $("#OtherExpense").NumericCurrency().SetDisabled(false);
        }
    }
    else {
        //Clear value
        $("#ApproveNo").val("");
        $("#OtherIncome").val("");
        $("#OtherExpense").val("");
        //Disable
        $("#ApproveNo").SetReadOnly(true);
        $("#OtherIncome").SetReadOnly(true);
        $("#OtherIncome").NumericCurrency().SetDisabled(true);
        $("#OtherExpense").SetReadOnly(true);
        $("#OtherExpense").NumericCurrency().SetDisabled(true);
    }
}

function ICS084SetScreenMode(mode) {
    if (mode == ICS084.ScreenMode.Register) {
        $("#divTargetInvoiceForPaymentMatching").SetViewMode(false);
        $("#divBalanceAfterProcessing").hide();
        $("#divBalanceAfterProcessing").NumericCurrency().hide();

        if ($("#BankFeeRegisteredFlag").val() == "True") {
            $("#BankFee").SetReadOnly(true);
        }

        if ($("#OtherIncomeRegisteredFlag").val() == "False" || $("#OtherExpenseRegisteredFlag").val() == "False") {
            $('#SelectSpecialProcess').SetDisabled(false);
        }
        else {
            $('#SelectSpecialProcess').SetDisabled(true);
        }
        SpecialProcessChange();

        register_command.SetCommand(cmdRegister);
        reset_command.SetCommand(cmdReset);
        confirm_command.SetCommand(null);
        back_command.SetCommand(null);

        //Call ics080.js
        if (typeof (ics080ShowBackButton) == "function") {
            ics080ShowBackButton();
        }
    }
    else if (mode == ICS084.ScreenMode.Confirm) {
        $("#divTargetInvoiceForPaymentMatching").SetViewMode(true);
        $("#divBalanceAfterProcessing").show();
        $("#divBalanceAfterProcessing").NumericCurrency().show();
        //SetFitColumnForBackAction(ICS084InvoiceGrid, "TempSpan");

        register_command.SetCommand(null);
        reset_command.SetCommand(null);
        confirm_command.SetCommand(cmdConfirm);
        back_command.SetCommand(cmdBack);

        //Call ics080.js
        if (typeof (ics080HideBackButton) == "function") {
            ics080HideBackButton();
        }
    }
    else if (mode == ICS084.ScreenMode.Finish) {
        register_command.SetCommand(null);
        reset_command.SetCommand(null);
        confirm_command.SetCommand(null);
        back_command.SetCommand(null);

        //Gogo mode 080  Call ics080.js
        if (typeof (ics080RefreshMatchableBalance) == "function") {
            ics080RefreshMatchableBalance();
        }
        if (typeof (SetScreenPageMode) == "function") {
            SetScreenPageMode(ICS080_Constant.ScreenPageMode.ICS080);
        }
    }
}

function cmdRegister() {
    //Clear require field
    $("#divTargetInvoiceForPaymentMatching").ResetToNormalControl();

    var obj = GetUserAdjustData();
    ajax_method.CallScreenController("/Income/ICS084_cmdRegister", obj, function (result, controls, isWarning) {
        if (result != undefined) {
            if (result.ResultFlag == "1") {
                //Show Balance After Process
                var newVal = (new Number(result.BalanceAfterProcessing)).numberFormat("#,##0.00");
                $("#BalanceAfterProcessing").val(newVal);
                $("#BalanceAfterProcessing").NumericCurrency().val(result.BalanceAfterProcessingCurrencyType);

                // Confirm warning message box before confirm mode
                ICS084ConfirmMessageList = result.ConfirmMessageID;
                ConfirmWarningMessageBox();
            }
        }
        else if (controls != undefined) {
            VaridateCtrl(controls, controls);
        }
    });
}

function ConfirmWarningMessageBox() {
    if (ICS084ConfirmMessageList == undefined || ICS084ConfirmMessageList.length == 0) {
        ICS084SetScreenMode(ICS084.ScreenMode.Confirm);
    }
    else {
        var msgID = ICS084ConfirmMessageList.shift();
        var param = { "module": "Income", "code": msgID };
        call_ajax_method("/Shared/GetMessage", param, function (data) {
            OpenYesNoMessageDialog(data.Code, data.Message, function () {
                ConfirmWarningMessageBox();
            }, null);
        });
    }
}

function cmdReset() {
    //Clear value
    $("#SelectSpecialProcess").attr('checked', false);
    $("#BankFee").val("");
    $("#ApproveNo").val("");
    $("#OtherIncome").val("");
    $("#OtherExpense").val("");
    $("#ExchangeLoss").val("");
    $("#ExchangeGain").val("");

    $("#OtherIncome").NumericCurrencyValue("");
    $("#OtherExpense").NumericCurrencyValue("");
    $("#ExchangeLoss").NumericCurrencyValue("");
    $("#ExchangeGain").NumericCurrencyValue("");

    //Disable
    $("#ApproveNo").SetReadOnly(true);
    $("#OtherIncome").SetReadOnly(true);
    $("#OtherExpense").SetReadOnly(true);

    $("#OtherIncome").NumericCurrency().SetDisabled(true);
    $("#OtherExpense").NumericCurrency().SetDisabled(true);

    //Clear require field
    $("#divTargetInvoiceForPaymentMatching").ResetToNormalControl();


    ICS084InitialGrid();
}

function cmdBack() {
    ICS084SetScreenMode(ICS084.ScreenMode.Register);
}


function cmdConfirm() {
    var obj = GetUserAdjustData();
    ajax_method.CallScreenController("/Income/ICS084_cmdConfirm", obj, function (result, controls, isWarning) {
        if (result != undefined) {
            if (result == "1") {
                // Success
                var objMsg = {
                    module: "Income",
                    code: "MSG7008"
                };
                call_ajax_method_json("/Shared/GetMessage", objMsg, function (resultMsg) {
                    OpenInformationMessageDialog(resultMsg.Code, resultMsg.Message, function () {
                        ICS084SetScreenMode(ICS084.ScreenMode.Finish);
                    });
                });
            }
        }
        else if (controls != undefined) {
            VaridateCtrl(controls, controls);
        }
    });
}


function GetUserAdjustData() {
    var arrAdjust = new Array();
    if (CheckFirstRowIsEmpty(ICS084InvoiceGrid) == false) {
        for (var i = 0; i < ICS084InvoiceGrid.getRowsNum(); i++) {
            var row_id = ICS084InvoiceGrid.getRowId(i);
            var chkSelectInvoice_ID = GenerateGridControlID("SelectInvoice", row_id);

            if ($("#" + chkSelectInvoice_ID).is(':checked')) {
                var invoiceNo = ICS084InvoiceGrid.cells2(i, ICS084InvoiceGrid.getColIndexById("InvoiceNo")).getValue();
                var txtKeyInMatchAmountIncWHT_ID = GenerateGridControlID("txtKeyInMatchAmountIncWHT", row_id);
                var txtKeyInWHTAmount_ID = GenerateGridControlID("txtKeyInWHTAmount", row_id);
                var txtInvoiceAmountCurrencyType = ICS084InvoiceGrid.cells2(i, ICS084InvoiceGrid.getColIndexById("InvoiceAmountCurrencyType")).getValue();

                var obj = {
                    InvoiceNo: invoiceNo,
                    KeyInMatchAmountIncWHT: $("#" + txtKeyInMatchAmountIncWHT_ID).NumericValue(),
                    KeyInMatchAmountIncWHTCurrencyType: $("#" + txtKeyInMatchAmountIncWHT_ID).NumericCurrencyValue(),
                    KeyInWHTAmount: $("#" + txtKeyInWHTAmount_ID).NumericValue(),
                    KeyInWHTAmountCurrencyType: $("#" + txtKeyInWHTAmount_ID).NumericCurrencyValue(),
                    KeyInMatchAmountIncWHT_ID: txtKeyInMatchAmountIncWHT_ID,
                    KeyInWHTAmount_ID: txtKeyInWHTAmount_ID,
                    InvoiceAmountCurrencyType: txtInvoiceAmountCurrencyType
                };
                arrAdjust.push(obj);
            }
        }
    }
    var ObjData = {
        SpecialProcess: $("#SelectSpecialProcess").is(':checked'),
        ApproveNo: $("#ApproveNo").NumericValue(),
        BankFee: $("#BankFee").NumericValue(),
        BankFeeCurrencyType: $("#BankFee").NumericCurrencyValue(),
        OtherExpense: $("#OtherExpense").NumericValue(),
        OtherExpenseCurrencyType: $("#OtherExpense").NumericCurrencyValue(),
        OtherIncome: $("#OtherIncome").NumericValue(),
        OtherIncomeCurrencyType: $("#OtherIncome").NumericCurrencyValue(),
        ExchangeLoss: $('#ExchangeLoss').NumericValue(),
        ExchangeLossCurrencyType: $('#ExchangeLoss').NumericCurrencyValue(),
        ExchangeGain: $('#ExchangeGain').NumericValue(),
        ExchangeGainCurrencyType: $('#ExchangeGain').NumericCurrencyValue(),
        MatchInvoiceData: arrAdjust,
        FirstPaymentAmountCurrencyType: $('#FirstPaymentAmount').NumericCurrencyValue()
    };
    return ObjData;
}