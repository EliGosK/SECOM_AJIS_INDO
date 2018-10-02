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
/// <reference path="../Base/object/ajax_method.js" />


var InstGrid;
var transInstGrid = null;
var dtNewInstrument = null;

TransferQty = "TransQty";
btnCal = "BtnCal";
isTotalRow = false;
calRowId = 0;

$(document).ready(function () {
    initScreen();
    initGrid();
    initButton();
    initEvent();

});
function initEvent() {
    $("#DestinationOffice").change(function () {
        var officeCode = $(this).val();

        if (officeCode == $("#HeadOfficeCode").val()) {
            $("#divInstArea").html(IVS060_InventoryAreaCbo.replace("{BlankID}", "InstArea"));
        }
        else if (officeCode == $("#SrinakarinOfficeCode").val()) {
            $("#divInstArea").html(IVS060_InventoryAreaSrinakarinCbo.replace("{BlankID}", "InstArea"));
        }
        else {
            $("#divInstArea").html(IVS060_InventoryAreaDepoCbo.replace("{BlankID}", "InstArea"));
        }
    });

    $("#InstCode").InitialAutoComplete("/Master/GetInstrumentCodeAll", null, true);
}

function initButton() {
    $("#NewRegister").click(function () {
        initScreen();

        if ($("#InstGrid").length > 0) {
            DeleteAllRow(InstGrid);
        }

        if ($("#TransInstGrid").length > 0) {
            DeleteAllRow(transInstGrid);
        }
    });
    $("#btnClear").click(function () {
        $("#btnSearch").SetDisabled(false);
        $("#searchSection").clearForm();

        if ($("#InstGrid").length > 0) {
            DeleteAllRow(InstGrid);
        }
    });

    $("#btnSearch").click(IVS060_Search);

    $("#Download").click(function () {
        ajax_method.CallScreenController("/Inventory/IVS060_CheckExistFile", "", function (result) {
            if (result == 1) {
                var key = ajax_method.GetKeyURL(null);
                var url = ajax_method.GenerateURL("/Inventory/IVS060_DownloadPdfAndWriteLog?k=" + key);
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


// search
function IVS060_Search() {
    master_event.LockWindow(true);
    $("#btnSearch").SetDisabled(true);

    var intArea = "";

    $("#InstArea").find("option").each(function () {
        if ($(this).val() != "") {
            if (intArea == "")
                intArea = $(this).val();
            else
                intArea = intArea + "," + $(this).val();
        }
    });

    intArea = "," + intArea + ",";

    var obj = { SourceOffice: $("#SourceOffice").val(),
        DestinationOffice: $("#DestinationOffice").val(),
        InstrumentName: $("#InstName").val(),
        InstrumentCode: $("#InstCode").val(),
        AreaCode: $("#InstArea").val()
    };

    //initGrid(obj);


    $("#InstGrid").LoadDataToGrid(InstGrid, ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/inventory/IVS060_SearchInventoryInstrumentList",
         { Cond: obj, SourceOffice: $("#SourceOffice").val(), IntAreaComboData: intArea },
    "dtSearchInstrumentListResult",
     false,
     function (result, controls, isWarning) {

         if (isWarning) {
             $("#btnSearch").SetDisabled(false);
             master_event.LockWindow(false);
         }   

         if (controls != undefined) {
             VaridateCtrl_AtLeast(["InstCode", "InstName", "InstArea", "SourceOffice", "DestinationOffice"], controls);
         } else {
             master_event.ScrollWindow("#ResultSection");
             $("#SourceOffice").SetDisabled(true);
             $("#DestinationOffice").SetDisabled(true);
             SetResetCommand(true, cmdReset);
         }
     }, function (result, controls, isWarning) { });

     $("#btnSearch").SetDisabled(false);
     master_event.LockWindow(false);

    SetConfirmCommand(false, null);
    SetBackCommand(false, null);
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
    // Initial grid
    if ($("#InstGrid").length > 0) {
        InstGrid = $("#InstGrid").InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/inventory/IVS060_InstGrid");
        SpecialGridControl(InstGrid, ["Select"]);
        BindOnLoadedEvent(InstGrid, function (gen_ctrl) {
            for (var i = 0; i < InstGrid.getRowsNum(); i++) {
                var row_id = InstGrid.getRowId(i);
                if (gen_ctrl == true) {
                    GenerateAddButton(InstGrid, "btnSelect", row_id, "Select", true);
                }

                BindGridButtonClickEvent("btnSelect", row_id, function (rid) {
                    $("#divGrid").show();
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
                        row_id: 0,
                        ShelfNo: selShelfNo
                    };

                    call_ajax_method_json("/inventory/IVS060_CheckBeforeAdd", { Cond: obj }, function (result, controls) {
                        if (result) {
                            CheckFirstRowIsEmpty(transInstGrid, true);
                            
                            //AddNewRow(transInstGrid, [selInstCode, selInstName, selAreaCodeName, selInstQty, "", "", "", selAreaCode, selShelfNo]);
                            AddNewRow(transInstGrid, [ConvertBlockHtml(selInstCode), ConvertBlockHtml(selInstName), selAreaCodeName, selInstQty, "", "", "", selAreaCode, selShelfNo]); //Modify by Jutarat A. on 28112013

                            var row_idx = transInstGrid.getRowsNum() - 1;
                            var row_id = transInstGrid.getRowId(row_idx);
                            obj.row_id = row_id;
                            call_ajax_method_json("/inventory/IVS060_UpdateRowIDTransfer", obj, function (result, controls) {
                                GenerateNumericBox2(transInstGrid, TransferQty, row_id, "TransferQty", "", 5, 0, 0, 99999, true, true);
                                GenerateRemoveButton(transInstGrid, "btnRemove", row_id, "Remove", true);

                                BindGridButtonClickEvent("btnRemove", row_id, function (rid) {

                                    call_ajax_method_json("/inventory/IVS060_DelElem", {
                                        InstrumentCode: transInstGrid.cells(rid, transInstGrid.getColIndexById("Instrumentcode")).getValue()
                                        , AreaCode: transInstGrid.cells(rid, transInstGrid.getColIndexById("AreaCode")).getValue()
                                    }, function (result, controls) {
                                        if (result) DeleteRow(transInstGrid, rid);

                                    });
                                });
                                var TransQtyID = "#" + GenerateGridControlID(TransferQty, row_id);

                                transInstGrid.setSizes();

                                $("#TransInstGrid").focus(); 

                                SetRegisterCommand(true, cmdRegister);
                                //SetResetCommand(true, cmdReset);
                            });
                        }
                    });
                    $("#TransInst").show();
                });
            }
        });
    }
    if ($("#TransInstGrid").length > 0) {
        transInstGrid = $("#TransInstGrid").InitialGrid(0, false, "/inventory/IVS060_TransGrid");
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

function initScreen() {
    ajax_method.CallScreenController("/inventory/IVS060_InitParam", "", function (result, controls) {
        if (result) {

            $("#SourceOffice option:first").attr("selected", true);
            $("#DestinationOffice option:first").attr("selected", true);

            var officeCode = $("#DestinationOffice").val();

            $("#memo").SetMaxLengthTextArea(1000);

            if (officeCode == $("#HeadOfficeCode").val()) {
                $("#divInstArea").html(IVS060_InventoryAreaCbo.replace("{BlankID}", "InstArea"));
            }
            else if (officeCode == $("#SrinakarinOfficeCode").val()) {
                $("#divInstArea").html(IVS060_InventoryAreaSrinakarinCbo.replace("{BlankID}", "InstArea"));
            }
            else {
                $("#divInstArea").html(IVS060_InventoryAreaDepoCbo.replace("{BlankID}", "InstArea"));
            }

            $("#memo").val("");
            $("#SourceOffice").SetDisabled(false);
            $("#btnSearch").SetDisabled(false);
            $("#DestinationOffice").SetDisabled(false);
            $("#searchSection").clearForm();
            $("#IVS060PAGE").SetViewMode(false);
            $("#searchSection").show();
            SetRegisterCommand(false, null);
            SetResetCommand(false, null);
            SetConfirmCommand(false, null);
            SetBackCommand(false, null);
            $("#TransInst").hide();
            $("#ShowSlip").hide();
        }
    });

    //Event grid binding

}

function cmdRegister() {
    command_control.CommandControlMode(false);
    var instrumentCodeIdx = transInstGrid.getColIndexById("Instrumentcode");
    var areaCodeIdx = transInstGrid.getColIndexById("AreaCode");
    var transfer = [];
    for (var i = 0; i < transInstGrid.getRowsNum(); i++) {
        var row_id = transInstGrid.getRowId(i);
        var transferControlID = GenerateGridControlID(TransferQty, row_id);

        transfer.push({
            controlID: transferControlID,
            instrumentCode: transInstGrid.cells(row_id, instrumentCodeIdx).getValue(),
            areaCode: transInstGrid.cells(row_id, areaCodeIdx).getValue(),
            transferQTY: $("#" + transferControlID).NumericValue()
        });
    }

    var obj = {
        memo: $("#memo").val(),
        transferQTYList: transfer
    };

    $("#divGrid").ResetToNormalControl();

    call_ajax_method_json("/inventory/IVS060_cmdReg", obj, function (result, controls) {
        if (controls != undefined) {
            VaridateCtrl(controls, controls);
        }

        if (result != undefined) {
            for (var i = 0; i < transInstGrid.getRowsNum(); i++) {
                var rDX = transInstGrid.getRowId(i);

                for (var k = 0; k < result.length; k++) {
                    if (rDX == result[k].row_id) {
                        transInstGrid.cells(result[k].row_id, transInstGrid.getColIndexById("InstrumentQty")).setValue(result[k].InstrumentQty.toString());
                        break;
                    }
                }
            }

            if (controls == undefined) {
                $("#searchSection").clearForm();

                $("#searchSection").hide();
                $("#IVS060PAGE").SetViewMode(true);

                SetRegisterCommand(false, null);
                SetResetCommand(false, null);
                SetConfirmCommand(true, cmdConfirm);
                SetBackCommand(true, cmdBack);

                //hideGridCol(InstGrid, "Select");
                hideGridCol(transInstGrid, "Remove");
                SetFitColumnForBackAction(transInstGrid, "TempColumn");

                //InstGrid.setSizes();
                transInstGrid.setSizes(); 
            }
        }

        command_control.CommandControlMode(true);
    });

}

function cmdConfirm() {
    command_control.CommandControlMode(false);

    call_ajax_method_json("/inventory/IVS060_cmdConfirm", "", function (result, controls) {
        if (result != null && result != "") {
            if (!result || (result.length > 0 && controls != undefined)) {
                cmdBack();

                VaridateCtrl(controls, controls);

                for (var i = 0; i < result.length; i++) {
                    var row_id = transInstGrid.getRowId(i);
                    transInstGrid.cells(row_id, transInstGrid.getColIndexById("InstrumentQty")).setValue(result[i].InstrumentQty.toString());
                }
            }
            else {
                $("#ShowSlip").show();
                $("#Slipno").val(result);
                $("#Slipno").focus();   
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
    //showGridCol(InstGrid, "Select");
    showGridCol(transInstGrid, "Remove");

    $("#searchSection").show();

    $("#IVS060PAGE").SetViewMode(false);

    //InstGrid.setSizes();
    transInstGrid.setSizes();

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

            if ($("#InstGrid").length > 0) {
                DeleteAllRow(InstGrid);
            }

            if ($("#TransInstGrid").length > 0) {
                DeleteAllRow(transInstGrid);
            }
        },
        null);
    });
}

function hideGridCol(grid, strCol) {
    grid.setColumnHidden(grid.getColIndexById(strCol), true);

}

function showGridCol(grid, strCol) {
    grid.setColumnHidden(grid.getColIndexById(strCol), false);
    grid.setSizes();
}