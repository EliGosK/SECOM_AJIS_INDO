// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
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

TransferQty = "TransQty";

$(document).ready(function () {
    initScreen();
    initButton();
    initEvent();
});
function initEvent() {
}

function initButton() {
    //    $("#NewRegister").click(function () {
    //        initScreen();
    //    });
    $("#btnClear").click(function () {
        initScreen();
    });

    $("#btnRetrieve").click(function () {
        master_event.LockWindow(true);   
        var obj = { SlipNo: $("#SlipNo").val() };
        call_ajax_method_json("/inventory/IVS070_SearchInventorySlip", { Cond: obj }, function (result, controls) {
            if (controls != undefined)
                VaridateCtrl(["SlipNo"], controls);
            if (result != null && result != "") {
                $("#Source").val(result.SourceOfficeName);
                $("#Destination").val(result.DestinationOfficeName);
                $("#memo").val(result.Memo);
                $("InstGrid").LoadDataToGrid(InstGrid, ROWS_PER_PAGE_FOR_VIEWPAGE, false, "/inventory/IVS070_SearchInventorySlipDetail", { SlipNo: $("#SlipNo").val() }, "doInventorySlipDetail", false, function () {
                    $("#SlipNo").SetDisabled(true);
                    $("#btnRetrieve").SetDisabled(true);
                    master_event.LockWindow(false);                   

                    SetCancelCommand(true, cmdCancel);
                    SetConfirmCommand(true, cmdConfirm);

                }, "");
                $("#ReceiveDetail").show();
            }
        });
    });
}

function AddNewRowForTotal(grid, data) {
    /// <summary>Method to add new row to grid</summary>
    /// <param name="grid" type="object">Grid object</param>
    /// <param name="data" type="Array">Data in each column</param>

    grid.addRow(grid.uid(),
                    data,
                    grid.getRowsNum() - 1);
    grid.setSizes();
}
//function initGrid(objForInstGrid) {
//    $("#divInstGrid").show();

//    InstGrid = $("#InstGrid").LoadDataToGridWithInitial(ROWS_PER_PAGE_FOR_SEARCHPAGE, false, false, "/inventory/IVS050_SearchInventoryInstrumentList"
//        , objForInstGrid, "dtSearchInstrumentListResult", false, false);
//}

function addCommas(nStr) {
    nStr += '';
    x = nStr.split('.');
    x1 = x[0];
    x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }
    return x1 + x2;
}

function chkNum(ele) {
    var num = parseFloat(ele);
    //  addCommas(num.toFixed(2));
    return addCommas(num.toFixed(2));
}

function initScreen() {

    call_ajax_method_json("/inventory/IVS070_initParam", "", function (result, controls) {
        $("#SlipNo").val("");
        $("#Source").val("");
        $("#Destination").val("");
        $("#memo").val("");
        SetConfirmCommand(false, null);
        SetCancelCommand(false, null);
        if ($("#InstGrid").length > 0) {
            InstGrid = $("#InstGrid").InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/inventory/IVS070_InstGrid");
        }
        $("#ReceiveDetail").hide();

    });
}

function cmdConfirm() {
    command_control.CommandControlMode(false);    
    var InstList = new Array();
    for (var i = 0; i < InstGrid.getRowsNum(); i++) {
        var row_id = InstGrid.getRowId(i);
        var Inst = {
            InstrumentCode: InstGrid.cells(row_id, InstGrid.getColIndexById("InstrumentCode")).getValue(),
            InstrumentName: InstGrid.cells(row_id, InstGrid.getColIndexById("InstrumentName")).getValue(),
            TransferQty: InstGrid.cells(row_id, InstGrid.getColIndexById("TransferQty")).getValue(),
            DestinationAreaCode: InstGrid.cells(row_id, InstGrid.getColIndexById("DestinationAreaCode")).getValue(),
            DestinationShelfNo: InstGrid.cells(row_id, InstGrid.getColIndexById("DestinationShelfNo")).getValue(),
            row_id: row_id
        };
        InstList.push(Inst);
    }


    var obj = { StockInInstrument: InstList
    };

    call_ajax_method_json("/inventory/IVS070_cmdConfirm", obj, function (result, controls) {
        if (result == null) {
            var obj = {
                module: "Inventory",
                code: "MSG4013"
            };

            call_ajax_method("/Shared/GetMessage", obj, function (result) {
                OpenOkCancelDialog(result.Code, result.Message, function () {
                    command_control.CommandControlMode(false);                    
                    call_ajax_method_json("/inventory/IVS070_cmdConfirm_part2", "", function (result, controls) {
                        if (result) {
                            var obj = {
                                module: "Inventory",
                                code: "MSG4019"
                            };
                            call_ajax_method("/Shared/GetMessage", obj, function (result) {
                                OpenInformationMessageDialog(result.Code, result.Message,
                                function () {
                                    $("#SlipNo").SetDisabled(false);
                                    $("#btnRetrieve").SetDisabled(false);
                                    initScreen();
                                }, null);
                            });
                        }
                    });
                }, function () {
                    SetCancelCommand(true, cmdCancel);
                    SetConfirmCommand(true, cmdConfirm);
                });
            });
        }
        else {
            InstGrid.selectRowById(result);
        }
    });

    command_control.CommandControlMode(true);
}
function cmdCancel() {
    // JS Call Message

    var obj = {
        module: "Common",
        code: "MSG0038"
    };
    call_ajax_method("/Shared/GetMessage", obj, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message,
        function () {
            $("#SlipNo").SetDisabled(false);
            $("#btnRetrieve").SetDisabled(false);
            initScreen();
        },
        null);
    });

}