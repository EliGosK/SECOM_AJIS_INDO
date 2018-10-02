//// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
///// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
///// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
///// <reference path="../../Scripts/modernizr-1.7.min.js"/>
///// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
///// <reference path="../../Scripts/Base/MessageDialog.js"/>
///// <reference path="../../Scripts/Base/Master.js"/>
///// <reference path="../Base/GridControl.js" />

///// <reference path="../Base/DateTimePicker.js" />
///// <reference path="../Base/control_events.js" />
var InstGrid = null;

var ElemGrid = null;
var ElemConfGrid = null;
var dtNewInstrument = null;
var totalTransferAmount = 0;
var currencyTotal = 0;

TransferQty = "TransQty";
btnCal = "BtnCal";
isTotalRow = false;
calRowId = 0;
$(document).ready(function () {
    initScreen();
    initGrid();
    initButton();

    $("#InstCode").InitialAutoComplete("/Master/GetInstrumentCodeAll", null, true);
});

function eXcell_transferQtyCell(cell) {
    this.base = eXcell_edn;
    this.base(cell);
    this.setValue = function (val) {
        this.cell.style.color = "Blue";
        this.cell.innerHTML = val;
    };
}
eXcell_transferQtyCell.prototype = new eXcell_edn;

function initButton() {
    $("#NewRegister").click(function () {
        initScreen();
        //        ElemGrid.setColumnHidden(ElemGrid.getColIndexById("Remove"), false);
        //        ElemGrid.setSizes();
        $("#LocationSelector").SetViewMode(false);
        $("#SourceLocation").attr("readonly", true);
        $("#DestinationLocation").attr("readonly", true);
        $("#Eliminate").SetViewMode(false);
        $("#Eliminate").clearForm();
    });
    $("#btnClear").click(function () {
        $("#Criteria").clearForm();
        DeleteAllRow(InstGrid);
    });

    $("#btnSearch").click(function () {
        var obj = { sourceLoc: SrcLoc,
            InstrumentName: $("#InstName").val(),
            InstrumentCode: $("#InstCode").val(),
            AreaCode: $("#InstArea").val()
        };

        //initGrid(obj);
        loadDataToGrid(obj);

        SetConfirmCommand(false, null);
        SetBackCommand(false, null);
    });

    $("#Download").click(function () {
        ajax_method.CallScreenController("/Inventory/IVS050_CheckExistFile", "", function (result) {
            if (result == 1) {
                var key = ajax_method.GetKeyURL(null);
                var url = ajax_method.GenerateURL("/Inventory/IVS050_DownloadPdfAndWriteLog?k=" + key);
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
function initGrid() {
    InstGrid = $("#InstGrid").InitialGrid(ROWS_PER_PAGE_FOR_VIEWPAGE, false, "/Inventory/IVS050_InstGrid");

    SpecialGridControl(InstGrid, ["Select"]);
    BindOnLoadedEvent(InstGrid, function () {
        for (var i = 0; i < InstGrid.getRowsNum(); i++) {
            var row_id = InstGrid.getRowId(i);
            //GenerateSelectButton(InstGrid, "btnSelect", row_id, "Select", true);
            GenerateAddButton(InstGrid, "btnSelect", row_id, "Select", true);
            BindGridButtonClickEvent("btnSelect", row_id, function (rid) {
                var selInstCode = InstGrid.cells(rid, InstGrid.getColIndexById("Instrumentcode")).getValue();
                var selInstName = InstGrid.cells(rid, InstGrid.getColIndexById("InstrumentName")).getValue();
                var selAreaCode = InstGrid.cells(rid, InstGrid.getColIndexById("AreaCode")).getValue();
                var selAreaCodeName = InstGrid.cells(rid, InstGrid.getColIndexById("AreaCodeName")).getValue();
                var selInstQty = InstGrid.cells(rid, InstGrid.getColIndexById("InstrumentQty")).getValue();
                var selShelfNo = InstGrid.cells(rid, InstGrid.getColIndexById("ShelfNo")).getValue();

                var obj = { InstrumentCode: selInstCode,
                    InstrumentName: selInstName,
                    AreaCode: selAreaCode,
                    AreaCodeName: selAreaCodeName,
                    InstrumentQty: selInstQty,
                    ShelfNo: selShelfNo,
                    row_id: 0
                };

                ajax_method.CallScreenController("/Inventory/IVS050_beforeAddElem", { Cond: obj }, function (result, controls) {
                    if (result) {
                        CheckFirstRowIsEmpty(ElemGrid, true);
                        if (ElemGrid.getRowsNum() < 2)
                            //AddNewRow(ElemGrid, [selInstCode, selInstName, selAreaCodeName, selInstQty, "", "", "", selAreaCode]);
                            AddNewRow(ElemGrid, [ConvertBlockHtml(selInstCode), ConvertBlockHtml(selInstName), selAreaCodeName, selInstQty, "", "", "", selAreaCode]); //Modify by Jutarat A. on 28112013
                        else
                            //AddNewRowForTotal(ElemGrid, [selInstCode, selInstName, selAreaCodeName, selInstQty, "", "", "", selAreaCode]);
                            AddNewRowForTotal(ElemGrid, [ConvertBlockHtml(selInstCode), ConvertBlockHtml(selInstName), selAreaCodeName, selInstQty, "", "", "", selAreaCode]); //Modify by Jutarat A. on 28112013

                        var row_idx = ElemGrid.getRowsNum() - 1;
                        if (isTotalRow)
                            row_idx--;

                        var row_id = ElemGrid.getRowId(row_idx);

                        obj.row_id = row_id;
                        ajax_method.CallScreenController("/Inventory/IVS050_UpdateRowIDElem", obj, function (result, controls) {
                            //alert("Updateed");
                        });

                        GenerateNumericBox2(ElemGrid, TransferQty, row_id, "TransferQty", "", 5, 0, 0, 99999, true, true);
                        GenerateRemoveButton(ElemGrid, "btnRemove", row_id, "Remove", true);
                        // Akat K. try copy from IVS090
                        SpecialGridControl(ElemGrid, ["TransferQty"]);
                        SpecialGridControl(ElemGrid, ["Remove"]);

                        BindGridButtonClickEvent("btnRemove", row_id, function (rid) {
                            ajax_method.CallScreenController("/Inventory/IVS050_DelElem", {
                                InstrumentCode: ElemGrid.cells(rid, ElemGrid.getColIndexById("Instrumentcode")).getValue()
                                                         , AreaCode: ElemGrid.cells(rid, ElemGrid.getColIndexById("AreaCode")).getValue()
                            }, function (result, controls) {
                                if (result) {
                                    DeleteRow(ElemGrid, rid);
                                    if (ElemGrid.getRowsNum() == 1 && isTotalRow == true) {
                                        DeleteRow(ElemGrid, calRowId);
                                        isTotalRow = false;
                                        calRowId = 0;
                                    }
                                    //btnCalClick();
                                }
                            });
                        });
                        var TransQtyID = "#" + GenerateGridControlID(TransferQty, row_id);

                        if (isTotalRow == false) {
                            AddNewRow(ElemGrid, [_lblTotal, "", "", "", "", "", "", ""]);
                            row_idx = ElemGrid.getRowsNum() - 1;
                            row_id = ElemGrid.getRowId(row_idx);

                            //                            ElemGrid.cells(row_id, ElemGrid.getColIndexById("Remove")).setValue(GenerateHtmlButton(btnCal, row_id, "cal", true));
                            //                            BindGridHtmlButtonClickEvent(btnCal, row_id, function (rid) {
                            //                                btnCalClick(undefined);
                            //                            });

                            calRowId = row_id;

                            GenerateCalculateButton(ElemGrid, btnCal, row_id, "Remove", true);
                            BindGridButtonClickEvent(btnCal, row_id, function (rid) {
                                btnCalClick(undefined);
                            });

                            ElemGrid.setColspan(row_id, 0, 4);
                            ElemGrid.setColspan(row_id, 4, 2);
                            isTotalRow = true;
                        }
                        ElemGrid.setSizes();
                        //btnCalClick();

                        SetRegisterCommand(true, cmdRegister);
                    }
                });
                $("#Eliminate").show();
                master_event.ScrollWindow("#Eliminate");
            });
        }
    });

    ElemGrid = null;
    ElemGrid = $("#EliminateInst").InitialGrid(0, false, "/Inventory/EliminateInstGrid");
    //SpecialGridControl(ElemGrid, ["Remove"]);
    BindOnLoadedEvent(ElemGrid, function () {
        for (var i = 0; i < ElemGrid.getRowsNum(); i++) {
            var row_id = ElemGrid.getRowId(i);

            GenerateNumericBox2(ElemGrid, TransferQty, row_id, "TransferQty", "", 6, 0, 0, 999999, true, true);
            GenerateRemoveButton(ElemGrid, "btnRemove", row_id, "Remove", true);

            BindGridButtonClickEvent("btnRemove", row_id, function (rid) {
                call_ajax_method_json("/inventory/IVS040_DelElem", {
                    InstrumentCode: ElemGrid.cells(rid, ElemGrid.getColIndexById("Instrumentcode")).getValue()
                                                         , AreaCode: ElemGrid.cells(rid, ElemGrid.getColIndexById("AreaCode")).getValue()
                }, function (result, controls) {
                    if (result) {
                        DeleteRow(ElemGrid, rid);
                        if (ElemGrid.getRowsNum() == 1 && isTotalRow == true) {
                            DeleteRow(ElemGrid, calRowId);
                            isTotalRow = false;
                            calRowId = 0;
                        }
                        //btnCalClick();
                    }
                });
            });

            ElemGrid.setSizes();
        }
    });

    ElemConfGrid = null;
    ElemConfGrid = $("#EliminateInstConfirm").InitialGrid(0, false, "/inventory/EliminateInstConfirmGrid");

}

function loadDataToGrid(objForInstGrid) {
    master_event.LockWindow(true);

    $("#divInstGrid").show();

    $("#InstGrid").LoadDataToGrid(InstGrid, ROWS_PER_PAGE_FOR_VIEWPAGE, false, "/inventory/IVS050_SearchInventoryInstrumentList", objForInstGrid, "dtSearchInstrumentListResult", false
            , function (result, controls, isWarning) {
                master_event.LockWindow(false);

                if (controls != undefined) {
                    //VaridateCtrl(["DestinationLocation ,SourceLocation"], controls);
                } else {
                    $("#SourceLocation").SetDisabled(true);
                    $("#DestinationLocation").SetDisabled(true);
                    SetResetCommand(true, cmdReset);
                }

            }
            , function (result, controls, isWarning) {
                //$("#Eliminate").show();
                //if (isWarning == undefined)
                //{ }
            });
}


function btnCalClick(afterCalFunc) {
    var lstElem = new Array();

    if (ElemGrid.getRowsNum() >= 2) {
        for (var i = 0; i < ElemGrid.getRowsNum() - 1; i++) {
            var row_id = ElemGrid.getRowId(i);
            var InstCode = ElemGrid.cells(row_id, ElemGrid.getColIndexById("Instrumentcode")).getValue();
            var AreaCode = ElemGrid.cells(row_id, ElemGrid.getColIndexById("AreaCode")).getValue();
            var TransQtyID = GenerateGridControlID(TransferQty, row_id);
            var TransferNumQty = $("#" + TransQtyID).NumericValue();

            var obj = {
                InstrumentCode: InstCode,
                AreaCode: AreaCode,
                TransferInstrumentQty: TransferNumQty,
                row_id: row_id,
                TransQtyID: TransQtyID
            };
            lstElem.push(obj);
        }
    }

    call_ajax_method_json("/Inventory/IVS050_Calculate", { Cond: lstElem, SourceLoc: $("#SourceLocation").val(), RegisterPress: (afterCalFunc != undefined) }, function (result, controls) {
        if (controls != undefined) {
            VaridateCtrl(controls, controls);
        }
        if (result != null) {
            var TotalNum = 0;
            for (var i = 0; i < ElemGrid.getRowsNum(); i++) {
                var rDX = ElemGrid.getRowId(i);

                for (var k = 0; k < result.length; k++) {
                    if (rDX == result[k].row_id) {
                        TotalNum = result[k].TransferAmount;

                        ElemGrid.cells(result[k].row_id, ElemGrid.getColIndexById("TransferingAmount")).setValue(result[k].TextTransferAmount);
                        ElemGrid.cells(result[k].row_id, ElemGrid.getColIndexById("SourceQty")).setValue(result[k].InstrumentQty);
                        currencyTotal = (result[k].TextTransferAmount).split(" ");
                        break;
                    }
                }
            }

            if (controls == undefined) {
                totalTransferAmount = chkNum(TotalNum.toString());
                ElemGrid.cells(calRowId, ElemGrid.getColIndexById("TransferQty")).setValue(currencyTotal[0] + ' ' + totalTransferAmount);
                if (afterCalFunc != undefined) {
                    afterCalFunc();
                }
            }
        }
    });

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

function initScreen() {
    if (InstGrid != null) {
        DeleteAllRow(InstGrid);
    }
    if (ElemGrid != null) {
        DeleteAllRow(ElemGrid);
    }
    if (ElemConfGrid != null) {
        DeleteAllRow(ElemConfGrid);
    }

    call_ajax_method_json("/inventory/IVS050_InitParam", "", function (result, controls) {
        if (result) {
            //            $("#Criteria").clearForm();
            // $("#InstGrid").InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/inventory/IVS050_instGrid");

            $("#Criteria").clearForm();

            SetRegisterCommand(false, null);
            SetResetCommand(false, null);
            SetConfirmCommand(false, null);
            SetBackCommand(false, null);

            $("#memo").SetMaxLengthTextArea(1000);

            //$("#searchSection").show();
            $("#Criteria").show();
            $("#SearchButton").show();
            $("#divInstGrid").show();

            $("#Eliminate").hide();
            $("#divGrid").show();
            $("#divConfirmGrid").hide();
            $("#ShowSlip").hide();
            //$("#divInstGrid").show();

            isTotalRow = false;
        }
    });
}

function cmdRegister() {
    btnCalClick(doRegister);
}

function doRegister() {
    command_control.CommandControlMode(false);

    var obj = { ApproveNo: $("#ApproveNo").val()
     , Memo: $("#memo").val()
     , SourceLoc: $("#SourceLocation").val()
    };

    $("#divGrid").ResetToNormalControl();

    call_ajax_method_json("/Inventory/IVS050_cmdReg", obj, function (result, controls) {
        command_control.CommandControlMode(true);

        if (controls != undefined) {
            VaridateCtrl(controls, controls);
        }

        if (result != undefined) {
            //6.3.1	Order data in selected list by “Instrument code”, “Area code”
            //$("#searchSection").hide();

            if (ElemConfGrid != null) {
                DeleteAllRow(ElemConfGrid);
            }


            $("#EliminateInstConfirm").LoadDataToGrid(ElemConfGrid, ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/inventory/IVS050_GetEliminateListForConfirm", obj, "IVS040INST", false
                , function (result, controls, isWarning) {
                    //ElemConfGrid = $("#EliminateInstConfirm").InitialGrid(0, false, "/inventory/EliminateInstConfirmGrid");

                    for (var i = 0; i < ElemConfGrid.getRowsNum() ; i++) {
                        var rDX = ElemConfGrid.getRowId(i);
                        var tmp = ElemConfGrid.cells(rDX, ElemConfGrid.getColIndexById("TransferAmount")).getValue();
                        ElemConfGrid.cells(rDX, ElemConfGrid.getColIndexById("TransferAmount")).setValue(currencyTotal[0] + ' ' + chkNum(tmp));
                    }

                    AddNewRow(ElemConfGrid, [_lblTotal, "", "", "", "", ""]);
                    row_idx = ElemConfGrid.getRowsNum() - 1;
                    row_id = ElemConfGrid.getRowId(row_idx);
                    ElemConfGrid.setColspan(row_id, 0, 4);
                    ElemConfGrid.setColspan(row_id, 4, 2);

                    ElemConfGrid.cells(row_id, ElemConfGrid.getColIndexById("TransferInstrumentQty")).setValue(currencyTotal[0] + ' ' + addCommas(totalTransferAmount));
                    ElemConfGrid.cells(row_id, ElemConfGrid.getColIndexById("TransferInstrumentQty")).setTextColor("black");

                    $("#Criteria").hide();
                    $("#SearchButton").hide();
                    $("#divInstGrid").hide();
                    $("#LocationSelector").SetViewMode(true);
                    $("#Eliminate").SetViewMode(true);

                    $("#divGrid").hide();
                    $("#divConfirmGrid").show();

                    //ElemGrid.setColumnHidden(ElemGrid.getColIndexById("Remove"), true);
                    //ElemGrid.setSizes();

                    SetRegisterCommand(false, null);
                    SetResetCommand(false, null);
                    SetConfirmCommand(true, cmdConfirm);
                    SetBackCommand(true, cmdBack);
                }
                , function (result, controls, isWarning) {
                    //$("#Eliminate").show();
                    if (isWarning == undefined)
                    { }
                });
        }
    });
}

function cmdConfirm() {
    var msgprm = {
        module: "Inventory",
        code: "MSG4115"
    };
    call_ajax_method("/Shared/GetMessage", msgprm, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message,
            function () {
                command_control.CommandControlMode(false);
                call_ajax_method_json("/inventory/IVS050_cmdConfirm", "", function (result, controls) {
                    command_control.CommandControlMode(true);

                    if (controls != undefined) {
                        VaridateCtrl(controls, controls);

                        if (result != null) {
                            for (var i = 0; i < ElemGrid.getRowsNum(); i++) {
                                var rDX = ElemGrid.getRowId(i);
                                for (var k = 0; k < result.length; k++) {
                                    if (rDX == result[k].row_id) {
                                        ElemGrid.cells(result[k].row_id, ElemGrid.getColIndexById("SourceQty")).setValue(result[k].InstrumentQty);
                                        break;
                                    }
                                }
                            }
                        }
                        // Akat K. : if error when confirm not go back to register screen
                        //cmdBack();
                    } else if (result == "toregister") {
                        // Akat K. : if error when confirm not go back to register screen
                        //cmdBack();
                    } else if (result != null && result != undefined) {
                        $("#Criteria").clearForm();
                        $("#ShowSlip").show();
                        master_event.ScrollWindow("#ShowSlip");
                        $("#Slipno").val(result);
                        SetRegisterCommand(false, null);
                        SetResetCommand(false, null);
                        SetConfirmCommand(false, null);
                        SetBackCommand(false, null);
                    }
                });
            },
            null);
    });
}

function cmdBack() {
    //$("#searchSection").show();
    $("#Criteria").show();
    $("#SearchButton").show();
    $("#divInstGrid").show();
    $("#LocationSelector").SetViewMode(false);
    $("#SourceLocation").attr("readonly", true);
    $("#DestinationLocation").attr("readonly", true);
    $("#Eliminate").SetViewMode(false);

    $("#divGrid").show();
    $("#divConfirmGrid").hide();

    //    ElemGrid.setColumnHidden(ElemGrid.getColIndexById("Remove"), false);
    //    ElemGrid.setSizes();
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
            //            ElemGrid.setColumnHidden(ElemGrid.getColIndexById("Remove"), false);
            //            ElemGrid.setSizes();
            $("#LocationSelector").SetViewMode(false);
            $("#SourceLocation").attr("readonly", true);
            $("#DestinationLocation").attr("readonly", true);
            $("#Eliminate").SetViewMode(false);
            $("#Eliminate").clearForm();
        },
        null);
    });
}

