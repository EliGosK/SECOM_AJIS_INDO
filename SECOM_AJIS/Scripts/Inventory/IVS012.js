/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../Base/GridControl.js" />

/// <reference path="../Base/DateTimePicker.js" />
/// <reference path="../Base/control_events.js" />
var InstGrid = null;

$(document).ready(function () {
    $("#txtVoucherDate").InitialDate();
    initScreen();
    initGrid();
    initButton();
    initEvent();
});
function initEvent() {
}

var InstrumentCost = "InstrumentCost";
var TotalRowId = null;
var ToTalInstrumentAmountID = "";

function initButton() {

    $("#btnRetrieve").click(function () {
        $("#InstrumentCostPrice input[type='text'],textarea").val("");
        var SlipNo = $("#SlipNo").val();
        var objSlipNo = { SlipNo: SlipNo };

        $("#InstrumentGrid").LoadDataToGrid(InstGrid, 0, false, "/inventory/RetrieveStockInSlip", objSlipNo, "doInventorySlipDetailList", false,
        function (result, controls, isWarning) {
            if (controls != undefined) {
                VaridateCtrl(["SlipNo"], controls);
            }

            if (result != null) {
                CalculateTotalAmount();
                call_ajax_method_json("/inventory/GetHeaderSlipDetail", objSlipNo, function (res, controls) {
                    if (res != null) {
                        $("#DetSlipNo").val(res.SlipNo);
                        $("#DetPorderNo").val(res.PurchaseOrderNo);
                        $("#DetStockInTypeCodeName").val(res.StockInTypeCodeName);
                        $("#DetSupOrderNo").val(res.DeliveryOrderNo);
                        $("#DetStockDate").val(ConvertDateToTextFormat(ConvertDateObject(res.StockInDate)));
                        $("#Detmemo").val(res.Memo);

                        $("#txtVoucherNo").val(res.VoucherID);
                        $("#txtVoucherDate").val(ConvertDateToTextFormat(ConvertDateObject(res.VoucherDate)));

                        $("#btnRetrieve").attr("disabled", true);
                        $("#SlipNo").attr("disabled", true);

                        SetRegisterCommand(true, cmdRegister);
                        SetResetCommand(true, cmdReset);
                        // $("#SpecifySlipNo").SetDisabled(true);
                    }
                    $("#InstrumentCostPrice").show();

                    $("#frmHeader input[type='text'],#frmHeader textarea").each(function () {
                        if (this.id != "txtVoucherNo" && this.id != "txtVoucherDate") {
                            $(this).SetDisabled(true);
                        }
                    });

                });
            }
        }, function () { });
    });

}

function initGrid() {

    var tmpInstGrid = $("#InstrumentGrid").InitialGrid(0, false, "/inventory/IVS012_InstrumentGrid", function () {
        BindOnLoadedEvent(tmpInstGrid, function (gen_ctrl) {
            var defaultCurrency = C_CURRENCY_LOCAL;
            for (var i = 0; i < tmpInstGrid.getRowsNum() ; i++) {
                var row_id = tmpInstGrid.getRowId(i);
                if (gen_ctrl) {
                    var instrumentCurrency = tmpInstGrid.cells(row_id, tmpInstGrid.getColIndexById("InstrumentAmountCurrencyType")).getValue();
                    defaultCurrency = instrumentCurrency;
                    if (tmpInstGrid.cells(row_id, tmpInstGrid.getColIndexById("SourceAreaCode")).getValue() == AreaSample) {

                        GenerateNumericCurrencyControl(tmpInstGrid, InstrumentCost, row_id, "InstrumentCost", defaultCurrency, "0.00", true, CalculateTotalAmount);
                        //GenerateNumericBox2(tmpInstGrid, InstrumentCost, row_id, InstrumentCost, "0.00", 12, 2, 0, 999999999999.99, true, false);
                    }
                    else {
                        var amount = tmpInstGrid.cells(row_id, tmpInstGrid.getColIndexById("InstrumentAmount")).getValue();
                        amount = (amount ? SetNumericText(amount, 2) : "0.00");
                        

                        GenerateNumericCurrencyControl(tmpInstGrid, InstrumentCost, row_id, "InstrumentCost", defaultCurrency, amount, true, CalculateTotalAmount);
                        //GenerateNumericBox2(tmpInstGrid, InstrumentCost, row_id, InstrumentCost, amount, 12, 2, 0, 999999999999.99, true, true);
                    } 
                }
            }

            // Loop 2 times.
            // Modify to fix problem. it have come from InstrumentRowTotal was pushed over the edge of the screen.
            // i think, It takes a long time to generate InstrumentGrid, So system was calculated the height screen error.
            // Pachar S. 11102016
            for (var i = 0; i < tmpInstGrid.getRowsNum() ; i++) {
                var row_id = tmpInstGrid.getRowId(i);
                if (gen_ctrl) {
                    var instrumentCurrency = tmpInstGrid.cells(row_id, tmpInstGrid.getColIndexById("InstrumentAmountCurrencyType")).getValue();
                    defaultCurrency = instrumentCurrency;
                    if (tmpInstGrid.cells(row_id, tmpInstGrid.getColIndexById("SourceAreaCode")).getValue() == AreaSample) {

                        GenerateNumericCurrencyControl(tmpInstGrid, InstrumentCost, row_id, "InstrumentCost", defaultCurrency, "0.00", true, CalculateTotalAmount);
                        //GenerateNumericBox2(tmpInstGrid, InstrumentCost, row_id, InstrumentCost, "0.00", 12, 2, 0, 999999999999.99, true, false);
                    }
                    else {
                        var amount = tmpInstGrid.cells(row_id, tmpInstGrid.getColIndexById("InstrumentAmount")).getValue();
                        amount = (amount ? SetNumericText(amount, 2) : "0.00");


                        GenerateNumericCurrencyControl(tmpInstGrid, InstrumentCost, row_id, "InstrumentCost", defaultCurrency, amount, true, CalculateTotalAmount);
                        //GenerateNumericBox2(tmpInstGrid, InstrumentCost, row_id, InstrumentCost, amount, 12, 2, 0, 999999999999.99, true, true);
                    }
                }
            }
            //

            GenerateCurrencyHeaderControl(tmpInstGrid, "InstrumentGrid", "EquipmentTotal", 4, InstrumentCost, defaultCurrency);

            if (gen_ctrl) {
                $("#InstrumentGrid_grid input:text[name^=" + InstrumentCost + "]")
                .blur(function () {
                    $(this).val(SetNumericText($(this).NumericValue(), 2));
                    CalculateTotalAmount();
                });
            }

            if (gen_ctrl) {
                AddNewRow(tmpInstGrid, ["", "Total", "0", "", "0", "", ""]);
                TotalRowId = tmpInstGrid.getRowId(tmpInstGrid.getRowsNum() - 1);

                GenerateNumericCurrencyControl(tmpInstGrid, InstrumentCost, TotalRowId, InstrumentCost, defaultCurrency, "0.00", false, CalculateTotalAmount);
                //GenerateNumericBox2(tmpInstGrid, InstrumentCost, TotalRowId, InstrumentCost, "0.00", 12, 2, 0, 999999999999.99, true, false);
            }
        });

        SpecialGridControl(tmpInstGrid, [InstrumentCost]);

        InstGrid = tmpInstGrid;

        if (PreloadSlilpNo) {
            $("#SlipNo").val(PreloadSlilpNo);
            PreloadSlilpNo = null;
            $("#btnRetrieve").click();
        }
    });

}

function initScreen() {

    $("#SpecifySlipNo").SetViewMode(false);
    $("#InstrumentCostPrice").SetViewMode(false);
    $("#InstrumentCostPrice").hide();
    $("#SlipNo").val("");
    $("#btnRetrieve").attr("disabled", false);
    $("#SlipNo").attr("disabled", false);
    $("#InstrumentCostPrice input[type='text'],textarea").val("");
    SetRegisterCommand(false, null);
    SetResetCommand(false, null);
    SetConfirmCommand(false, null);
    SetBackCommand(false, null);
}

function cmdRegister() {
    command_control.CommandControlMode(false);
    $("#InstrumentGrid").find(".highlight").toggleClass("highlight", false);

    var noprice_instcode = "";
    var InstArr = new Array();
    for (var i = 0; i < GetInstrumentCount(); i++) {
        var row_id = InstGrid.getRowId(i);
        var obj = {
            InstrumentCode: InstGrid.cells(row_id, InstGrid.getColIndexById("InstrumentCode")).getValue(),
            InstrumentTotalPrice: $("#" + GenerateGridControlID(InstrumentCost, row_id)).NumericValue(),
            InstrumentTotalPriceEnable: !$("#" + GenerateGridControlID(InstrumentCost, row_id)).prop("readonly"),
            InstrumentArea: InstGrid.cells(row_id, InstGrid.getColIndexById("SourceAreaCode")).getValue(),
            StockInQty: InstGrid.cells(row_id, InstGrid.getColIndexById("TransferQty")).getValue(),
            txtInstrumentTotalPrice: GenerateGridControlID(InstrumentCost, row_id),
            RunningNo: InstGrid.cells(row_id, InstGrid.getColIndexById("RunningNo")).getValue()
        };

        obj.InstrumentAmountCurrencyType = $("#" + ToTalInstrumentAmountID).NumericCurrencyValue();
        if (obj.InstrumentAmountCurrencyType == C_CURRENCY_LOCAL) {
            obj.InstrumentAmountCurrencyType = obj.InstrumentAmountCurrencyType;
            obj.InstrumentTotalPrice = obj.InstrumentTotalPrice;
            obj.InstrumentAmountUsd = null;

            if (!obj.InstrumentTotalPrice || obj.InstrumentTotalPrice == 0) {
                if (noprice_instcode) {
                    noprice_instcode += ",";
                }
                noprice_instcode += obj.InstrumentCode;
            }
            InstArr.push(obj);
        }
        else {
            obj.InstrumentAmountCurrencyType = obj.InstrumentAmountCurrencyType;
            obj.InstrumentAmountUsd = obj.InstrumentTotalPrice;;
            obj.InstrumentTotalPrice = null;

            if (!obj.InstrumentAmountUsd || obj.InstrumentAmountUsd == 0) {
                if (noprice_instcode) {
                    noprice_instcode += ",";
                }
                noprice_instcode += obj.InstrumentCode;
            }
            InstArr.push(obj);
        }            
    }

    if (noprice_instcode) {

        // OpenYesNoMessageDialog
        var message;
        var param = { "module": "Inventory", "code": "MSG4147", param: noprice_instcode };
        call_ajax_method_json("/Shared/GetMessage", param, function (data) {
            OpenYesNoMessageDialog(data.Code, data.Message, function () {
                doRegisterIVS012(InstArr);
            });
        });
    }
    else {
        doRegisterIVS012(InstArr);
    }

}

function doRegisterIVS012(InstArr) {
    var obj = {
        cond: InstArr,
        SlipNo: $("#DetSlipNo").val(),
        VoucherNo: $("#txtVoucherNo").val(),
        VoucherDate: $("#txtVoucherDate").val()
    }

    call_ajax_method_json("/inventory/RegisterIvs012", obj, function (results, controls) {
        if (controls != undefined) {
            VaridateCtrl(controls, controls);
        }

        // end if 
        if (results) {
            $("#SpecifySlipNo").SetViewMode(true);
            $("#InstrumentCostPrice").SetViewMode(true);
            SetRegisterCommand(false, null);
            SetResetCommand(false, null);
            SetConfirmCommand(true, cmdConfirm);
            SetBackCommand(true, cmdBack);
        }

        command_control.CommandControlMode(true);
    });
}

function cmdConfirm() {
    command_control.CommandControlMode(false);
    ajax_method.CallScreenController("/inventory/cmdConfirmIVS012", "", function (res, controls) {
        if (res) {
            var obj = {
                module: "Common",
                code: "MSG0046"
            };
            call_ajax_method("/Shared/GetMessage", obj, function (result) {
                OpenInformationMessageDialog(result.Code, result.Message,
                function () {
                    initScreen();
                },
                null);
            });
        }
        command_control.CommandControlMode(true);
    });
}
function cmdBack() {
    $("#SpecifySlipNo").SetViewMode(false);
    $("#InstrumentCostPrice").SetViewMode(false);
    SetRegisterCommand(true, cmdRegister);
    SetResetCommand(true, cmdReset);
    SetConfirmCommand(false, null);
    SetBackCommand(false, null);
}

function cmdReset() {
    var obj = {
        module: "Common",
        code: "MSG0038"
    };
    call_ajax_method("/Shared/GetMessage", obj, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message,
        function () {
            SetRegisterCommand(false, null);
            SetResetCommand(false, null);
            SetConfirmCommand(false, null);
            SetBackCommand(false, null);
            initScreen();
        },
        null);
    });
}

function GetInstrumentCount() {
    if (InstGrid) {
        return InstGrid.getRowsNum() - 1;
    }
}

function CalculateTotalAmount() {
    if (!TotalRowId) {
        return;
    }

    var totalAmount = 0;
    var totalQty = 0;

    for (var i = 0; i < GetInstrumentCount(); i++) {
        var row_id = InstGrid.getRowId(i);
        
        totalQty += InstGrid.cells(row_id, InstGrid.getColIndexById("TransferQty")).getValue();
        totalAmount += +($('#' + InstrumentCost + '_' + row_id).NumericValue());
        //totalAmount += +($("#" + GenerateGridControlID(InstrumentCost, row_id)).NumericValue());
    }

    // InstGrid.cells(TotalRowId, InstGrid.getColIndexById("InstrumentAmount")).setValue(totalAmount);
    InstGrid.cells(TotalRowId, InstGrid.getColIndexById("TransferQty")).setValue(totalQty);
    $("#" + GenerateGridControlID(InstrumentCost, TotalRowId)).val(SetNumericText(totalAmount, 2));
}

function GenerateNumericCurrencyControl(grid, id, row_id, col_id, currency, textboxValue, enable, funcTrigger) {
    var col = grid.getColIndexById(col_id);
    var fid = GenerateGridControlID(id, row_id);
    ToTalInstrumentAmountID = id + "_" + row_id;

    var obj = {
        id: fid,
        currency: currency,
        textboxValue: textboxValue,
        enable: enable
    };
    ajax_method.CallScreenController("/inventory/IVS012_GenerateCurrencyNumericTextBox", obj, function (result, controls, isWarning) {
        var txt = result;
        grid.cells(row_id, col).setValue(txt);
        fid = "#" + fid;
        $(fid).BindNumericBox(12, 2, 0, 999999999999.99);
        var dVal = parseFloat(($(fid).val()).replace(/ /g, "").replace(/,/g, ""));

        $(fid).val(accounting.toFixed(dVal, 2));
        $(fid).setComma();
        $(fid).NumericCurrency().attr('disabled', true);

        $(fid).focus(function () {
            grid.selectRowById(row_id);
        });

        $(fid).blur(function () {
            if (funcTrigger != null)
                funcTrigger();
        });

        $(fid).change(function () {
            if (funcTrigger != null)
                funcTrigger();
        });

        if (funcTrigger != null)
            funcTrigger();
    });
}

function GenerateCurrencyHeaderControl(grid, tableID, id, col_index, col_ID, currency) {
    //var col = grid.getColIndexById(col_index);
    //var fid = GenerateGridControlID(id, row_id);

    var obj = {
        id: id,
        currency: currency
    };
    ajax_method.CallScreenController("/inventory/IVS012_GenerateCurrencyCombobox", obj, function (result, controls, isWarning) {
        var txt = result;
        var headerText = $('#' + tableID).find("table:eq(0)").find("tr:eq(1)").find("td:eq(" + col_index + ")").html();
        $('#' + tableID).find("table:eq(0)").find("tr:eq(1)").find("td:eq(" + col_index + ")").html(result + ' ' + headerText);
       
        $('#' + id).change(function(){
            for(var i=0; i<grid.getRowsNum(); i++)
            {
                var row_id = grid.getRowId(i);
                $('#' + col_ID + '_' + row_id + 'CurrencyType').val($(this).val());
            }
        })
        
    });
}