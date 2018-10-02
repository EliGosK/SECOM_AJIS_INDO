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

var StockGrid = null;
StockOutQty = "StockOutQty";

$(document).ready(function () {
    initScreen();
    initButton();
    initEvent();
});

function initScreen() {
    ajax_method.CallScreenController("/inventory/IVS100_InitParam", "", function (result, controls) {
        if (result) {       

            $("#SourceLocation").SetDisabled(false);

            $("#TransferStock").SetViewMode(false);
            $("#MainSection").SetViewMode(false);

            $("#searchSection").clearForm();
            $("#TransferStock").clearForm();
            $("#SourceLocation option:first").attr("selected", true);

            SetRegisterCommand(false, null);
            SetResetCommand(false, null);
            SetConfirmCommand(false, null);
            SetBackCommand(false, null);

            $("#memo").SetMaxLengthTextArea(1000);

            $("#TransferStock").hide();

            $("#ShowSlip").hide();

            $("#divInstGrid").show();
        }
    });

    if ($("#InstGrid").length > 0) {
        InstGrid = $("#InstGrid").InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/inventory/IVS100_InstGrid");
        
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

                        var obj = { InstrumentCode: selInstCode,
                            InstrumentName: selInstName,
                            AreaCode: selAreaCode,
                            AreaCodeName: selAreaCodeName,
                            InstrumentQty: selInstQty,
                            ShelfNo: selShelfNo
                        , row_id: 0
                        };

                        ajax_method.CallScreenController("/inventory/IVS100_beforeAddElem", { Cond: obj }, function (result, controls) {
                            if (result) {
                                CheckFirstRowIsEmpty(StockGrid, true);

                                //AddNewRow(StockGrid, [selInstCode, selInstName, selAreaCodeName, selInstQty, "", "", "", selAreaCode, selShelfNo]);
                                AddNewRow(StockGrid, [ConvertBlockHtml(selInstCode), ConvertBlockHtml(selInstName), selAreaCodeName, selInstQty, "", "", "", selAreaCode, selShelfNo]); //Modify by Jutarat A. on 28112013

                                var row_idx = StockGrid.getRowsNum() - 1;

                                var row_id = StockGrid.getRowId(row_idx);

                                SetRegisterCommand(true, cmdRegister);
                                SetResetCommand(true, cmdReset);
                                SetConfirmCommand(false, null);
                                SetBackCommand(false, null);

                                obj.row_id = row_id;
                                ajax_method.CallScreenController("/inventory/IVS100_UpdateRowIDElem", obj, function (result, controls) {
                                    //alert("Updateed");
                                });

                                GenerateNumericBox2(StockGrid, StockOutQty, row_id, "StockOutQty", "", 5, 0, 0, 99999, true, true);
                                GenerateRemoveButton(StockGrid, "btnRemove", row_id, "Remove", true);
                                SpecialGridControl(StockGrid, ["Remove"]);
                                SpecialGridControl(StockGrid, ["StockOutQty"]);
                                BindGridButtonClickEvent("btnRemove", row_id, function (rid) {

                                    ajax_method.CallScreenController("/inventory/IVS100_DelElem", {
                                        InstrumentCode: StockGrid.cells(rid, StockGrid.getColIndexById("Instrumentcode")).getValue()
                                                            , AreaCode: StockGrid.cells(rid, StockGrid.getColIndexById("AreaCode")).getValue()
                                    }, function (result, controls) {
                                        if (result) {
                                            DeleteRow(StockGrid, rid);
                                        }
                                    });
                                });
                                var TransQtyID = "#" + GenerateGridControlID(StockOutQty, row_id);

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
        StockGrid = $("#EliminateInst").InitialGrid(0, false, "/inventory/TransferStockInstGrid");
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

    $("#btnSearch").click(IVS100_Search);
}

function IVS100_Search() {
    master_event.LockWindow(true);
    $("#btnSearch").attr("disabled", true);

    var obj = { sourceLoc: $("#SourceLocation").val(),
        InstName: $("#InstName").val(),
        InstCode: $("#InstCode").val(),
        InstArea: $("#InstArea").val()
    };

    $("#InstGrid").LoadDataToGrid(InstGrid, ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/inventory/IVS100_SearchInventoryInstrument", obj, "dtSearchInstrumentListResult", false
        , function (result, controls, isWarning) {
            if (controls != undefined) {
                VaridateCtrl_AtLeast(["InstCode", "InstName", "InstArea"], controls);                
            }
            else if (result == undefined) {
                DeleteAllRow(InstGrid);
            }
            else if (isWarning == undefined) {
                master_event.ScrollWindow("#ResultSection");
                $("#SourceLocation").SetDisabled(true);
                SetResetCommand(true, cmdReset);
            }

            $("#btnSearch").attr("disabled", false);
            master_event.LockWindow(false);
        }
        , function (result, controls, isWarning) {
        });    
}

function initEvent() {
    $("#InstCode").InitialAutoComplete("/Master/GetInstrumentCodeAll", null, true);
}

function clear() {
    $("#searchSection").clearForm();
    //InstGrid = $("#InstGrid").InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/inventory/IVS100_InstGrid");
    DeleteAllRow(InstGrid);
}

function cmdRegister() {
    command_control.CommandControlMode(false);

    var InstList = new Array();
    for (var i = 0; i < StockGrid.getRowsNum(); i++) {
        var row_id = StockGrid.getRowId(i);
        var StockOutControlID = GenerateGridControlID(StockOutQty, row_id);

        var Inst = {
            InstrumentCode: StockGrid.cells(row_id, StockGrid.getColIndexById("Instrumentcode")).getValue(),
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
    

    var obj = { ApproveNo: $("#ApproveNo").val()
     , Memo: $("#memo").val()
     , SourceLoc: $("#SourceLocation").val()
     , RepairSubContractor: $("#SubContractor").val()
     , StockInInstrument: InstList
    };

    $("#divGrid").ResetToNormalControl();

    ajax_method.CallScreenController("/inventory/IVS100_cmdReg", obj, function (result, controls) {
        if (controls != undefined) {
            VaridateCtrl(controls, controls);
        }

        if (result != undefined) {
            for (var i = 0; i < result.length; i++) {
                var row_id = StockGrid.getRowId(i);
                StockGrid.cells(row_id, StockGrid.getColIndexById("InstrumentQty")).setValue(result[i].InstrumentQty.toString());
            }

            if (controls == undefined) {
                StockGrid.setColumnHidden(StockGrid.getColIndexById("Remove"), true);
                SetFitColumnForBackAction(StockGrid, "TempColumn");            
                $("#searchSection").clearForm();
                $("#searchSection").hide();
                $("#TransferStock").SetViewMode(true);
                $("#locationSection").SetViewMode(true);                

                SetRegisterCommand(false, null);
                SetResetCommand(false, null);
                SetConfirmCommand(true, cmdConfirm);
                SetBackCommand(true, cmdBack);
            }
        }
                
        command_control.CommandControlMode(true);
    });

}

function cmdConfirm() {
    command_control.CommandControlMode(false);

    ajax_method.CallScreenController("/inventory/IVS100_cmdConfirm", null, function (result, controls) {
        if (result != null && result != "") {
            if (!result || (result.length > 0 && controls != undefined)) {
                cmdBack();

                for (var i = 0; i < result.length; i++) {
                    var row_id = StockGrid.getRowId(i);
                    StockGrid.cells(row_id, StockGrid.getColIndexById("InstrumentQty")).setValue(result[i].InstrumentQty.toString());
                }

                StockGrid.setSizes();

                VaridateCtrl(controls, controls);
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
    ajax_method.CallScreenController("/Inventory/IVS100_CheckExistFile", "", function (result) {
        if (result == 1) {
            var key = ajax_method.GetKeyURL(null);
            var url = ajax_method.GenerateURL("/Inventory/IVS100_DownloadPdfAndWriteLog?k=" + key);
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