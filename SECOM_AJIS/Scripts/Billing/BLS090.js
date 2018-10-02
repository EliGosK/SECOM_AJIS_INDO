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



var gridSearchResult;


var BLS090 = {
    ScreenMode: {
        Register: "RegisterMode",
        Confirm: "ConfirmMode",
        Finish: "FinishMode",
        NewRegister: "NewRegisterMode",
        Reset: "Reset"
    }
};

$(document).ready(function () {
    InitialGrid();
    InitialEvent();
    InitialPage();
});

function InitialGrid() {
    if ($.find("#BLS090_ResultListGrid").length > 0) {
        gridSearchResult = $("#BLS090_ResultListGrid").InitialGrid(0, false, "/Billing/BLS090_InitialSearchResultGrid", function () {

            SpecialGridControl(gridSearchResult, ["NewMonthlyBillingAmount"]);
            BindOnLoadedEvent(gridSearchResult, function () {
                //var colInx = gridSearchResult.getColIndexById('BtnRemove');
                for (var i = 0; i < gridSearchResult.getRowsNum(); i++) {
                    var rowId = gridSearchResult.getRowId(i);

                    // generate numeric text box in grid // NewMonthlyBillingAmount ==> Fee
                    var newFeeColIndex = gridSearchResult.getColIndexById("NewMonthlyBillingAmount");
                    //var val = GetValueFromLinkType(gridSearchResult, i, newFeeColIndex);
                    //GenerateNumericBox2(gridSearchResult, "txtNewBillingFee", rowId, "NewMonthlyBillingAmount", val, 8, 2, 0, 99999999, true, true);

                    //var txt_id = "#" + GenerateGridControlID("txtNewBillingFee", rowId);

                    //$(txt_id).blur(function () {
                    //    var all = GetUserAdjustData();

                    //    var row_idx = gridSearchResult.getRowsNum() - 1;
                    //    var row_id_lastrow = gridSearchResult.getRowId(row_idx);
                    //    var lastrow_fee = "#" + GenerateGridControlID("txtNewBillingFee", row_id_lastrow);

                    //    var val = all.totalFee;
                    //    var newVal = (new Number(val)).numberFormat("#,###.##");

                    //    $(lastrow_fee).val(newVal);
                    //});

                }

                gridSearchResult.setSizes();
            });
        });
    }
}

function InitialEvent() {

    $("#btnRetrieve").click(function () {

        $("#btnRetrieve").attr("disabled", true);
        $("#ContractCode").attr("readonly", true);

        // toUpperCase
        var contractCode_UpperCase = $("#ContractCode").val().toUpperCase();
        $("#ContractCode").val(contractCode_UpperCase) ;
        var obj = { ContractCode: $("#ContractCode").val() };
        ajax_method.CallScreenController("/Billing/BLS090_GetHeader", obj, function (result, controls) {
            if (result != undefined) {

                $("#formContractInfo").bindJSON(result);

                //---- deatail---
                $("#BLS090_ResultListGrid").LoadDataToGrid(gridSearchResult, 0, false, "/Billing/BLS090_GetDetail", obj, "dtBillingBasicForRentalList", false,
                    function (result2, controls) { // post-load
                        if (result2 != undefined) {
                            if (CheckFirstRowIsEmpty(gridSearchResult) == false) {
                                SetScreenMode(BLS090.ScreenMode.Register);


                                // Add #Total Row
                                var lblTotal = "<div style='text-align: right;'>" + $("#lblTotal").val() + " </div>";
                                AddNewRow(gridSearchResult, [lblTotal,
                                                               "",
                                                               result.TotalFee,   // Fee (sum)
                                                               //result.TotalFee,   // Fee (sum)
                                                               "0.00",   // New fee
                                                               ""]); // ToJson

                                 //Add by Jirawat Jannet @ 2016-08-29
                                 //Update last column from only numeric text to curency combobox and numeric textbox

                                for (var i = 0 ; i < gridSearchResult.getRowsNum() - 1; i++) {
                                    var rowId = gridSearchResult.getRowId(i);

                                    GenerateNumericCurrencyControl(gridSearchResult, 'txtNewBillingFee', rowId, 'NewMonthlyBillingAmount'
                                                                        , result.details[i].MonthlyBillingAmount, result.details[i].MonthlyBillingAmountCurrencyType, true);


                                    var txt_id = "#" + GenerateGridControlID("txtNewBillingFee", rowId);

                                    $(txt_id).blur(function () {
                                        var all = GetUserAdjustData();

                                        var row_idx = gridSearchResult.getRowsNum() - 1;
                                        var row_id_lastrow = gridSearchResult.getRowId(row_idx);
                                        var lastrow_fee = "#" + GenerateGridControlID("txtNewBillingFee", row_id_lastrow);

                                        var val = all.totalFee;
                                        var newVal = (new Number(val)).numberFormat("#,###.##");

                                        $(lastrow_fee).val(newVal);
                                    });
                                }

                                //End add

                                var row_idx = gridSearchResult.getRowsNum() - 1;
                                var row_id = gridSearchResult.getRowId(row_idx);
                                gridSearchResult.setColspan(row_id, 0, 2);

                                var val = result.TotalFeeForDisplay;
                                var valCurrency = result.ContractFeeCurrencyType;
                                //GenerateNumericBox2(gridSearchResult, "txtNewBillingFee", row_id, "NewMonthlyBillingAmount", val, 8, 2, 0, 99999999, true, false);
                                //GenerateNumericBoxWithUnit("txtNewBillingFee", row_id, val, '140', valCurrency, true, true, false);
                                GenerateNumericCurrencyControl(gridSearchResult, 'txtNewBillingFee', row_id, 'NewMonthlyBillingAmount', val, valCurrency, true);

                            } else {
                                reset_command.SetCommand(command_reset_click);
                            }
                        }
                        else if (controls != undefined) {
                            VaridateCtrl(controls, controls);
                            // For prevent click this button more than one time
                            $("#btnRetrieve").attr("disabled", false);
                            $("#ContractCode").attr("readonly", false);
                        }
                        else {
                            // For prevent click this button more than one time
                            $("#btnRetrieve").attr("disabled", false);
                            $("#ContractCode").attr("readonly", false);
                        }

                    },
                    function (result, controls, isWarning) { // pre-load
                        if (isWarning == undefined) {
                            $("#divSearchResult").show();
                        }
                    });

            }
            else if (controls != undefined) {
                VaridateCtrl(controls, controls);

                $("#btnRetrieve").attr("disabled", false);
                $("#ContractCode").attr("readonly", false);
                $("#formContractInfo").clearForm();
            }
            else {
                $("#btnRetrieve").attr("disabled", false);
                $("#ContractCode").attr("readonly", false);
                $("#formContractInfo").clearForm();
                VaridateCtrl(controls, controls);
            }
        });
    });
}

// Creat by Jirawat Jannet 2016-08-23
function GenerateNumericCurrencyControl(grid, id, row_id, col_id, value, currency, readOnly) {
    var col = grid.getColIndexById(col_id);
    var fid = GenerateGridControlID(id, row_id);

    var obj = {
        id: fid,
        value: value,
        currency: currency,
        readOnly: readOnly
    };
    ajax_method.CallScreenController("/Billing/BLS090_GenerateCurrencyNumericTextBox", obj, function (result, controls, isWarning) {
        var txt = result;
        grid.cells(row_id, col).setValue(txt);
        fid = "#" + fid;
        //$(fid).parent().parent().css({ "text-overflow": "clip" });
        //$(fid).BindNumericBox(before, dec, min, max, defaultmin);
        var amtVal = parseFloat($(fid).val().replace(',',''));
        $(fid).val(amtVal.toFixed(2));
        $(fid).setComma();

        //$(fid).focus(function () {
        //    grid.selectRowById(row_id);
        //});
    });

    //var val = parseFloat(grid.cells(row_id, 2).getValue());
    //var Show = BLS090_ViewBag.Currency + ' ' + val.toFixed(2);
    //grid.cells(row_id, 2).setValue(Show);

    var obj2 = {
        currencyCode: currency
    };
    ajax_method.CallScreenController("/Billing/BLS090_GetCurrencyDisplay", obj2, function (result, controls, isWarning) {
        var txt = result;
        var val = parseFloat(grid.cells(row_id, 2).getValue());
        var Show = txt + ' ' + val.toFixed(2);
        grid.cells(row_id, 2).setValue(Show);

        
    });
}

function InitialPage() {
    $("#formContractInfo").clearForm();
    $("#divSearchResult").hide();
}



function SetScreenMode(mode) {
    if (mode == BLS090.ScreenMode.Register) {

        $("#divSearchCondition").show();
        $("#divSearchResult").show();

        register_command.SetCommand(command_register_click);
        reset_command.SetCommand(command_reset_click);
        confirm_command.SetCommand(null);
        back_command.SetCommand(null);

        $("#divSearchResult").SetViewMode(false);

        $("#ContractCode").attr("readonly", true);
        $("#btnRetrieve").attr("disabled", true);

    }
    else if (mode == BLS090.ScreenMode.Confirm) {

        $("#divSearchCondition").show();
        $("#divSearchResult").show();

        register_command.SetCommand(null);
        reset_command.SetCommand(null);
        confirm_command.SetCommand(command_confirm_click);
        back_command.SetCommand(command_back_click);

        $("#divSearchResult").SetViewMode(true);

        $("#ContractCode").attr("readonly", true);
        $("#btnRetrieve").attr("disabled", true);
    }
    else if (mode == BLS090.ScreenMode.Finish || mode == BLS090.ScreenMode.Reset) {

        $("#divSearchCondition").show();
        $("#divSearchResult").hide();

        register_command.SetCommand(null);
        reset_command.SetCommand(null);
        confirm_command.SetCommand(null);
        back_command.SetCommand(null);

        $("#ContractCode").attr("readonly", false);
        $("#btnRetrieve").attr("disabled", false);

        // Clear row
        DeleteAllRow(gridSearchResult);
        $("#divSearchCondition").clearForm();
    }


}

function command_register_click() {
    var all = GetUserAdjustData();
    var obj = all.detail;
    ajax_method.CallScreenController("/Billing/BLS090_Register", obj, function (result, controls, isWarning) {
        if (result != undefined) {
            if (result != "") {
                SetScreenMode(BLS090.ScreenMode.Confirm);
            }
        }
        else if (controls != undefined) {
            VaridateCtrl(controls, controls);
        }
    });
}

function command_confirm_click() {
    // Call ajax to save
    ajax_method.CallScreenController("/Billing/BLS090_Confirm", "",
        function (result, controls, isWarning) {
            if (result != undefined) {
                if (result != "") {

                    SetScreenMode(BLS090.ScreenMode.Finish);
                }
            }
            else if (controls != undefined) {
                // Validate again -> but not pass then set screen to input mode and hilight controls
                SetScreenMode(BLS090.ScreenMode.Register);
                VaridateCtrl(controls, controls);
            }
        });
}

function command_reset_click() {
    SetScreenMode(BLS090.ScreenMode.Reset);
}

function command_back_click() {
    SetScreenMode(BLS090.ScreenMode.Register);
}

function GetUserAdjustData() {

    var arr = new Array();
    var grid = gridSearchResult;

    var total = 0;

    if (CheckFirstRowIsEmpty(grid) == false) {
        for (var i = 0; i < grid.getRowsNum(); i++) {
            if (i < grid.getRowsNum() - 1) { // Not include Row# Total
                var row_id = grid.getRowId(i);
                var txtNewBillingFee_id = GenerateGridControlID("txtNewBillingFee", row_id);

                var strJson = grid.cells2(i, grid.getColIndexById("ToJson")).getValue();
                var data = JSON.parse(htmlDecode(strJson));

                var obj = {
                    txtNewBillingFeeID: txtNewBillingFee_id,
                    ContractCode: data.ContractCode,
                    BillingOCC: data.BillingOCC,
                    BillingClientCode: data.BillingClientCode,
                    MonthlyBillingAmount: data.MonthlyBillingAmount ,
                    NewBillingFee: $("#" + txtNewBillingFee_id).NumericValue(),
                    NewMonthlyBillingAmountCurrency: $("#" + txtNewBillingFee_id).NumericCurrencyValue(),
                    row_id: row_id
                };

                var decNewBillingFee = parseFloat(obj.NewBillingFee);
                total += (isNaN(decNewBillingFee) ? 0 : decNewBillingFee);

                arr.push(obj);
            }
        }
    }

    var retrunObj = { detail: arr, totalFee: total };

    return retrunObj;

}


