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

var InstGrid = null;
var currencyTotal = 0;

var StockGrid = null;
StockOutQty = "StockOutQty";
btnCal = "BtnCal";
isTotalRow = false;
calRowId = 0;
$(document).ready(function () {
    $("#txtStockOutDate").InitialDate();
    $("#txtStockOutDate").SetMinDate(IVS090_Constant.MINDATE);
    $("#txtStockOutDate").SetMaxDate(IVS090_Constant.MAXDATE);
    $("#memo").SetMaxLengthTextArea(1000);

    initScreen();
    initButton();
    initEvent();
});

function initScreen() {
    ajax_method.CallScreenController("/inventory/IVS090_InitParam", "", function (result, controls) {
        if (result) {
            $("#TransferStock").SetViewMode(false);
            $("#MainSection").SetViewMode(false);

            $("#searchSection").clearForm();
            $("#TransferStock").clearForm();
            SetRegisterCommand(false, null);
            SetResetCommand(false, null);
            SetConfirmCommand(false, null);
            SetBackCommand(false, null);

            isTotalRow = false;

            $("#TransferStock").hide();

            $("#ShowSlip").hide();

            $("#divInstGrid").show();

            $("#txtStockOutDate").SetDate(new Date);
        }
    });

    if ($("#InstGrid").length > 0) {
        InstGrid = $("#InstGrid").InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/inventory/IVS090_InstGrid");

        SpecialGridControl(InstGrid, ["Select"]);
        BindOnLoadedEvent(InstGrid, function (gen_ctrl) {
            if (CheckFirstRowIsEmpty(InstGrid, false) == false) {
                for (var i = 0; i < InstGrid.getRowsNum(); i++) {
                    var row_id = InstGrid.getRowId(i);
                    if (gen_ctrl == true) {
                        GenerateAddButton(InstGrid, "btnSelect", row_id, "Select", true);
                    }

                    BindGridButtonClickEvent("btnSelect", row_id, function (rid) {
                        var selInstCode = InstGrid.cells(rid, InstGrid.getColIndexById("Instrumentcode")).getValue();
                        var selInstName = InstGrid.cells(rid, InstGrid.getColIndexById("InstrumentName")).getValue();
                        var selAreaCode = InstGrid.cells(rid, InstGrid.getColIndexById("AreaCode")).getValue();
                        var selAreaCodeName = InstGrid.cells(rid, InstGrid.getColIndexById("AreaCodeName")).getValue();
                        var selInstQty = InstGrid.cells(rid, InstGrid.getColIndexById("InstrumentQty")).getValue();
                        var selShelfNo = InstGrid.cells(rid, InstGrid.getColIndexById("ShelfNo")).getValue();

                        var obj = { Instrumentcode: selInstCode,
                            InstrumentName: selInstName,
                            AreaCode: selAreaCode,
                            AreaCodeName: selAreaCodeName,
                            InstrumentQty: selInstQty,
                            ShelfNo: selShelfNo
                        , row_id: 0
                        };

                        ajax_method.CallScreenController("/inventory/IVS090_beforeAddElem", { Cond: obj }, function (result, controls) {
                            if (result) {
                                CheckFirstRowIsEmpty(StockGrid, true);
                                if (StockGrid.getRowsNum() < 2)
                                    //AddNewRow(StockGrid, [selInstCode, selInstName, selAreaCodeName, selInstQty.toString(), "", "0", "", "", selAreaCode, selShelfNo]);
                                    AddNewRow(StockGrid, [ConvertBlockHtml(selInstCode), ConvertBlockHtml(selInstName), selAreaCodeName, selInstQty.toString(), "", "0.00", "", "", selAreaCode, selShelfNo]); //Modify by Jutarat A. on 28112013
                                else
                                    //AddNewRowForTotal(StockGrid, [selInstCode, selInstName, selAreaCodeName, selInstQty.toString(), "", "0", "", "", selAreaCode, selShelfNo]);
                                    AddNewRowForTotal(StockGrid, [ConvertBlockHtml(selInstCode), ConvertBlockHtml(selInstName), selAreaCodeName, selInstQty.toString(), "", "0", "", "", selAreaCode, selShelfNo]); //Modify by Jutarat A. on 28112013

                                var row_idx = StockGrid.getRowsNum() - 1;
                                if (isTotalRow)
                                    row_idx--;

                                var row_id = StockGrid.getRowId(row_idx);

                                SetRegisterCommand(true, cmdRegister);
                                SetResetCommand(true, cmdReset);
                                SetConfirmCommand(false, null);
                                SetBackCommand(false, null);

                                obj.row_id = row_id;
                                ajax_method.CallScreenController("/inventory/IVS090_UpdateRowIDElem", obj, function (result, controls) {
                                    //alert("Updateed");
                                });

                                GenerateNumericBox2(StockGrid, StockOutQty, row_id, "StockOutQty", "", 5, 0, 0, 99999, true, true);
                                GenerateRemoveButton(StockGrid, "btnRemove", row_id, "Remove", true);
                                SpecialGridControl(StockGrid, ["StockOutQty"]);
                                SpecialGridControl(StockGrid, ["Remove"]);
                                BindGridButtonClickEvent("btnRemove", row_id, function (rid) {

                                    ajax_method.CallScreenController("/inventory/IVS090_DelElem", {
                                        Instrumentcode: StockGrid.cells(rid, StockGrid.getColIndexById("Instrumentcode")).getValue()
                                                            , AreaCode: StockGrid.cells(rid, StockGrid.getColIndexById("AreaCode")).getValue()
                                    }, function (result, controls) {
                                        if (result) {
                                            DeleteRow(StockGrid, rid);
                                            if (StockGrid.getRowsNum() == 1 && isTotalRow == true) {
                                                DeleteRow(StockGrid, calRowId);
                                                isTotalRow = false;
                                                calRowId = 0;
                                            }
                                        }

                                    });
                                });
                                var TransQtyID = "#" + GenerateGridControlID(StockOutQty, row_id);

                                if (isTotalRow == false) {
                                    AddNewRow(StockGrid, [_lblTotal, "", "", "", "", "", "", "", "", ""]);
                                    row_idx = StockGrid.getRowsNum() - 1;
                                    row_id = StockGrid.getRowId(row_idx);
//                                    StockGrid.cells(row_id, StockGrid.getColIndexById("Remove")).setValue(GenerateHtmlButton(btnCal, row_id, "Calculate", true));
                                    GenerateCalculateButton(StockGrid, btnCal, row_id, "Remove", true);
                                    StockGrid.setRowColor(row_id, "#ffffff");
                                    calRowId = row_id;
//                                    BindGridHtmlButtonClickEvent(btnCal, row_id, function (rid) {
//                                        btnCalClick();
//                                    });
                                    BindGridButtonClickEvent(btnCal, row_id, function (rid) {
                                        btnCalClick();
                                    });
                                    StockGrid.setColspan(row_id, 0, 4);
                                    StockGrid.setColspan(row_id, 4, 2);
                                    isTotalRow = true;
                                }

                                StockGrid.setSizes();
                            }
                        });


                        $("#TransferStock").show();

                    });
                }
            }
        });
    }

    if ($("#EliminateInst").length > 0) {
        StockGrid = $("#EliminateInst").InitialGrid(0, false, "/inventory/IVS090_TransferStockInstGrid");
    }
}

function initButton() {
    $("#NewRegister").click(function () {
        $("#searchSection").show();
        initScreen();
    });
    $("#btnClear").click(function () {
        clear();
    });
    $("#Download").click(function () {
        downloadDocumentReport();
    });

    $("#btnSearch").click(IVS090_Search);
}

function IVS090_Search() {
    master_event.LockWindow(true);
    $("#btnSearch").attr("disabled", true);

    var obj = { sourceLoc: $("#SourceLocationCode").val(),
        InstName: $("#InstName").val(),
        InstCode: $("#InstCode").val(),
        InstArea: $("#InstArea").val()
    };

    $("#InstGrid").LoadDataToGrid(InstGrid, ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/inventory/IVS090_SearchInventoryInstrument", obj, "dtSearchInstrumentListResult", false
            , function (result, controls, isWarning) {
                if (controls != undefined) {
                    VaridateCtrl_AtLeast(["InstCode", "InstName", "InstArea"], controls);
                }
                if (result == null) {
                    DeleteAllRow(InstGrid);
                }
                else if (isWarning == undefined) {
                    master_event.ScrollWindow("#ResultSection");
                    SetResetCommand(true, cmdReset);
                }

                $("#btnSearch").attr("disabled", false);
                master_event.LockWindow(false);
            }
            , function (result, controls, isWarning) {
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

function initEvent() {
    $("#InstCode").InitialAutoComplete("/Master/GetInstrumentCodeAll", null, true);
}

function clear() {
    $("#searchSection").clearForm();
    //InstGrid = $("#InstGrid").InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/inventory/IVS090_InstGrid");   
    DeleteAllRow(InstGrid);
}

function cmdRegister() {
    command_control.CommandControlMode(false);

    var InstList = new Array();
    if (StockGrid.getRowsNum() >= 2) {
        for (var i = 0; i < StockGrid.getRowsNum() - 1; i++) {
            var row_id = StockGrid.getRowId(i);
            var StockOutControlID = GenerateGridControlID(StockOutQty, row_id);
            var Inst = {
                Instrumentcode: StockGrid.cells(row_id, StockGrid.getColIndexById("Instrumentcode")).getValue(),
                InstrumentName: StockGrid.cells(row_id, StockGrid.getColIndexById("InstrumentName")).getValue(),
                AreaCode: StockGrid.cells(row_id, StockGrid.getColIndexById("AreaCode")).getValue(),
                AreaCodeName: StockGrid.cells(row_id, StockGrid.getColIndexById("AreaCodeName")).getValue(),
                InstrumentQty: StockGrid.cells(row_id, StockGrid.getColIndexById("InstrumentQty")).getValue(),
                StockOutQty: $("#" + GenerateGridControlID(StockOutQty, row_id)).NumericValue(),
                ShelfNo: StockGrid.cells(row_id, StockGrid.getColIndexById("ShelfNo")).getValue(),
                row_id: row_id,
                StockOutQty_id: StockOutControlID
            };
            InstList.push(Inst);
        }
    }


    var obj = { 
        ApproveNo: $("#ApproveNo").val()
        , Memo: $("#memo").val()
        , SourceLoc: $("#SourceLocationCode").val()
        , RepairSubContractor: $("#SubContractor").val()
        , StockInInstrument: InstList
        , StockOutDate: $("#txtStockOutDate").val()
    };

    $("#divGrid").ResetToNormalControl();

    ajax_method.CallScreenController("/inventory/IVS090_cmdReg", obj, function (result, controls) {
        if (controls != undefined) {
            VaridateCtrl(controls, controls);
        }

        if (result != undefined) {
            if (controls == undefined) {
                StockGrid.setColumnHidden(StockGrid.getColIndexById("Remove"), true);
                SetFitColumnForBackAction(StockGrid, "TempColumn");                

                var TotalNum = 0;
                for (var i = 0; i < StockGrid.getRowsNum(); i++) {
                    var rDX = StockGrid.getRowId(i);
                    for (var k = 0; k < result.length; k++) {
                        if (rDX == result[k].row_id) {
                            var Amnt = result[k].TransferAmount;
                            TotalNum += Amnt;
                            StockGrid.cells(result[k].row_id, StockGrid.getColIndexById("TransferingAmount")).setValue(result[k].TextTransferAmount);
                            currencyTotal = (result[k].TextTransferAmount).split(" ");
                            break;
                        }
                    }
                }
                StockGrid.cells(calRowId, StockGrid.getColIndexById("StockOutQty")).setValue(currencyTotal[0] + ' ' + chkNum(TotalNum.toString()));

                $("#searchSection").clearForm();
                $("#searchSection").hide();
                $("#TransferStock").SetViewMode(true);
                $("#locationSection").SetViewMode(true);

                for (var i = 0; i < result.length; i++) {
                    var row_id = StockGrid.getRowId(i);
                    StockGrid.cells(row_id, StockGrid.getColIndexById("InstrumentQty")).setValue(result[i].InstrumentQty.toString());
                }                

                SetRegisterCommand(false, null);
                SetResetCommand(false, null);
                SetConfirmCommand(true, cmdConfirm);
                SetBackCommand(true, cmdBack);
            }
            else {
                for (var i = 0; i < result.length; i++) {
                    var row_id = StockGrid.getRowId(i);
                    StockGrid.cells(row_id, StockGrid.getColIndexById("InstrumentQty")).setValue(result[i].InstrumentQty.toString());
                }
            }
        }

        command_control.CommandControlMode(true);
    });

}

function cmdConfirm() {
    var obj = {
        module: "Inventory",
        code: "MSG4113"
    };
    call_ajax_method("/Shared/GetMessage", obj, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message,
        function () {
            command_control.CommandControlMode(false);

            var InstList = new Array();
            if (StockGrid.getRowsNum() >= 2) {
                for (var i = 0; i < StockGrid.getRowsNum() - 1; i++) {
                    var row_id = StockGrid.getRowId(i);
                    var StockOutControlID = GenerateGridControlID(StockOutQty, row_id);
                    var Inst = {
                        Instrumentcode: StockGrid.cells(row_id, StockGrid.getColIndexById("Instrumentcode")).getValue(),
                        InstrumentName: StockGrid.cells(row_id, StockGrid.getColIndexById("InstrumentName")).getValue(),
                        AreaCode: StockGrid.cells(row_id, StockGrid.getColIndexById("AreaCode")).getValue(),
                        AreaCodeName: StockGrid.cells(row_id, StockGrid.getColIndexById("AreaCodeName")).getValue(),
                        InstrumentQty: StockGrid.cells(row_id, StockGrid.getColIndexById("InstrumentQty")).getValue(),
                        StockOutQty: $("#" + GenerateGridControlID(StockOutQty, row_id)).NumericValue(),
                        ShelfNo: StockGrid.cells(row_id, StockGrid.getColIndexById("ShelfNo")).getValue(),
                        TransferAmount: "0.00",
                        row_id: row_id,
                        StockOutQty_id: StockOutControlID
                    };

                    if (StockGrid.cells(row_id, StockGrid.getColIndexById("TransferingAmount")).getValue() != null && ((StockGrid.cells(row_id, StockGrid.getColIndexById("TransferingAmount")).getValue()).toString().indexOf('.', 0) == -1)) {
                        Inst.TransferAmount = StockGrid.cells(row_id, StockGrid.getColIndexById("TransferingAmount")).getValue().toString() + ".00";
                    }

                    InstList.push(Inst);
                }
            }

            var obj = { 
                ApproveNo: $("#ApproveNo").val()
                , Memo: $("#memo").val()
                , SourceLoc: $("#SourceLocationCode").val()
                , RepairSubContractor: $("#SubContractor").val()
                , StockInInstrument: InstList
                , StockOutDate: $("#txtStockOutDate").val()
            };

            ajax_method.CallScreenController("/inventory/IVS090_cmdConfirm", obj, function (result, controls) {
                if (controls != undefined) {
                    VaridateCtrl(controls, controls);
                }
                
                if (result != null && result != "") {
                    if (!result || (result.length > 0 && controls != undefined)) {
                        cmdBack();

                        for (var i = 0; i < result.length; i++) {
                            var row_id = StockGrid.getRowId(i);
                            StockGrid.cells(row_id, StockGrid.getColIndexById("InstrumentQty")).setValue(result[i].InstrumentQty.toString());
                        }

                        StockGrid.setSizes();
                    }
                    else {
                        $("#ShowSlip").show();
                        $("#Slipno").val(result);
                        SetRegisterCommand(false, null);
                        SetResetCommand(false, null);
                        SetConfirmCommand(false, null);
                        SetBackCommand(false, null);
                    }
                }
                command_control.CommandControlMode(true);
            });
        },
        null);
    });
}

function cmdBack() {
    StockGrid.setColumnHidden(StockGrid.getColIndexById("Remove"), false);
    SetFitColumnForBackAction(StockGrid, "TempColumn");  

    $("#searchSection").show();
    $("#TransferStock").SetViewMode(false);
    $("#locationSection").SetViewMode(false);

    StockGrid.setSizes();

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
            initScreen();
        },
        null);
    });
}

function btnCalClick() {
    var lstElem = new Array();

    if (StockGrid.getRowsNum() >= 2) {
        for (var i = 0; i < StockGrid.getRowsNum() - 1; i++) {
            var row_id = StockGrid.getRowId(i);
            var StockOutControlID = GenerateGridControlID(StockOutQty, row_id);
            var Inst = {
                Instrumentcode: StockGrid.cells(row_id, StockGrid.getColIndexById("Instrumentcode")).getValue(),
                AreaCode: StockGrid.cells(row_id, StockGrid.getColIndexById("AreaCode")).getValue(),
                ShelfNo: StockGrid.cells(row_id, StockGrid.getColIndexById("ShelfNo")).getValue(),
                StockOutQty: $("#" + GenerateGridControlID(StockOutQty, row_id)).NumericValue(),
                row_id: row_id,
                StockOutQty_id: StockOutControlID
            };
            lstElem.push(Inst);
        }

        ajax_method.CallScreenController("/inventory/IVS090_Calculate", { Cond: lstElem, SourceLoc: $("#SourceLocationCode").val() }, function (result, controls) {
            if (controls != undefined) {
                VaridateCtrl(controls, controls);
            }

            if (result != null) {
                var TotalNum = 0;
                for (var i = 0; i < StockGrid.getRowsNum(); i++) {
                    var rDX = StockGrid.getRowId(i);
                    for (var k = 0; k < result.length; k++) {
                        if (rDX == result[k].row_id) {
                            var Amnt = result[k].TransferAmount;
                            TotalNum += Amnt;
                            //StockGrid.cells(result[k].row_id, StockGrid.getColIndexById("TransferingAmount")).setValue(Amnt.toString());
                            StockGrid.cells(result[k].row_id, StockGrid.getColIndexById("TransferingAmount")).setValue(result[k].TextTransferAmount);
                            currencyTotal = (result[k].TextTransferAmount).split(" ");
                            break;
                        }
                    }
                }
                StockGrid.cells(calRowId, StockGrid.getColIndexById("StockOutQty")).setValue(currencyTotal[0] + ' ' + chkNum(TotalNum.toString()));
            }
        });
    }

}

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

function chkUndefined(grid) {
    var row_num = grid.getRowsNum();
    if (row_num == 1) {
        var attr = grid.getRowAttribute(grid.getRowId(0), "mode");
        if (attr == undefined) {
            return true;
        }
    }
    else if (row_num == 0)
        return true;

    return false;
}

function downloadDocumentReport() {
    ajax_method.CallScreenController("/Inventory/IVS090_CheckExistFile", "", function (result) {
        if (result == 1) {
            var key = ajax_method.GetKeyURL(null);
            var url = ajax_method.GenerateURL("/Inventory/IVS090_DownloadPdfAndWriteLog?k=" + key);
            window.open(url, "Report", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
        }
        else {
            var param = { "module": "Common", "code": "MSG0112" };
            call_ajax_method("/Shared/GetMessage", param, function (data) {
                /* ====== Open info dialog =====*/
                OpenInformationMessageDialog(param.code, data.Message);
            });
        }

    });
}

function VaridateCtrl_AtLeast(ctrl_lst, null_ctrl) {
    if (ctrl_lst != null) {
        for (var idx = 0; idx < ctrl_lst.length; idx++) {
            var ctrl = $("#" + ctrl_lst[idx]);
            if (ctrl.length > 0) {
                ctrl.removeClass("highlight");


                if (ctrl[0].tagName.toLowerCase() == "select") {
                    var unb = function () {
                        for (var i = 0; i < ctrl_lst.length; i++) {
                            $("#" + ctrl_lst[i]).removeClass("highlight");
                            $("#" + ctrl_lst[i]).unbind("change", unb);
                        }
                    };
                    ctrl.change(unb);
                }
                else {
                    var unb = function () {
                        for (var i = 0; i < ctrl_lst.length; i++) {
                            $("#" + ctrl_lst[i]).removeClass("highlight");
                            $("#" + ctrl_lst[i]).unbind("keyup", unb);
                        }
                        // $(this).removeClass("highlight");
                        //  $(this).unbind("keyup", unb);
                    };
                    ctrl.keyup(unb);


                }
            }
        }
    }
    if (null_ctrl != null) {
        for (var idx = 0; idx < null_ctrl.length; idx++) {
            if (null_ctrl[idx] != "") {
                var ctrl = $("#" + null_ctrl[idx]);
                if (ctrl.length > 0) {
                    ctrl.addClass("highlight");
                }
            }
        }
    }

}