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
StockOutQty = "TransferQty";
DestinationShelfNo = "DestinationShelfNo";

$(document).ready(function () {
    initScreen();
    initButton();
    initEvent();

    $("#txtTransferDate").InitialDate();

    $("#txtTransferDate").SetMinDate(C_MINDATE);
    $("#txtTransferDate").SetMaxDate(C_MAXDATE);
});

function initScreen() {
    ajax_method.CallScreenController("/inventory/IVS080_InitParam", "", function (result, controls) {
        if (result) {
            $("#TransferStock").SetViewMode(false);
            $("#MainSection").SetViewMode(false);

            $("#searchSection").clearForm();
            $("#TransferStock").clearForm();

            $("#Office option:first").attr("selected", true);
            $("#SourceArea option:first").attr("selected", true);
            $("#DestinationArea option:first").attr("selected", true);

            SetRegisterCommand(false, null);
            SetResetCommand(false, null);
            SetConfirmCommand(false, null);
            SetBackCommand(false, null);

            $("#approve_require").hide();

            $("#memo").SetMaxLengthTextArea(1000);

            $("#Office").SetDisabled(false);
            $("#SourceArea").SetDisabled(false);
            $("#DestinationArea").SetDisabled(false);

            $("#TransferStock").hide();

            $("#ShowSlip").hide();

            $("#divInstGrid").show();

            $('#SourceArea >option').remove(":contains(':')");
            $('#DestinationArea >option').remove(":contains(':')");
        }
    });

    if ($("#InstGrid").length > 0) {
        InstGrid = $("#InstGrid").InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/inventory/IVS080_InstGrid");

        SpecialGridControl(InstGrid, ["Select"]);
        BindOnLoadedEvent(InstGrid, function (gen_ctrl) {
            if (CheckFirstRowIsEmpty(InstGrid, false) == false) {
                for (var i = 0; i < InstGrid.getRowsNum(); i++) {
                    var row_id = InstGrid.getRowId(i);

                    if (gen_ctrl == true) {
                        GenerateAddButton(InstGrid, "btnSelect", row_id, "Select", true);
                    }

                    BindGridButtonClickEvent("btnSelect", row_id, function (rid) {
                        var selInstCode = DecodeHtml(InstGrid.cells(rid, InstGrid.getColIndexById("Instrumentcode")).getValue());
                        var selInstName = DecodeHtml(InstGrid.cells(rid, InstGrid.getColIndexById("InstrumentName")).getValue());
                        var selShelfNo = DecodeHtml(InstGrid.cells(rid, InstGrid.getColIndexById("ShelfNo")).getValue());
                        var selInstQty = InstGrid.cells(rid, InstGrid.getColIndexById("InstrumentQty")).getValue();

                        var obj = { InstrumentCode: selInstCode,
                            InstrumentName: selInstName,
                            ShelfNo: selShelfNo,
                            InstrumentQty: selInstQty
                        , row_id: 0
                        };

                        ajax_method.CallScreenController("/inventory/IVS080_beforeAddElem", { Cond: obj, OfficeCode: $("#Office").val() }, function (result, controls) {
                            if (result) {
                                CheckFirstRowIsEmpty(StockGrid, true);

                                //AddNewRow(StockGrid, [selInstCode, selInstName, selShelfNo, selInstQty, "", "", "", ""]);
                                AddNewRow(StockGrid, [ConvertBlockHtml(selInstCode), ConvertBlockHtml(selInstName), selShelfNo, selInstQty, "", "", "", ""]); //Modify by Jutarat A. on 28112013

                                var row_idx = StockGrid.getRowsNum() - 1;

                                var row_id = StockGrid.getRowId(row_idx);

                                DisplayRequireApprove();
                                DisplayContractCodeInput();

                                $("#EliminateInst").focus();

                                SetRegisterCommand(true, cmdRegister);
                                SetResetCommand(true, cmdReset);
                                SetConfirmCommand(false, null);
                                SetBackCommand(false, null);

                                obj.row_id = row_id;
                                ajax_method.CallScreenController("/inventory/IVS080_UpdateRowIDElem", obj, function (result, controls) {
                                    //alert("Updateed");
                                });

                                GeneratTextBox(StockGrid, DestinationShelfNo, row_id, "DestinationShelfNo", "", 50, true);
                                GenerateNumericBox2(StockGrid, StockOutQty, row_id, "TransferQty", "", 5, 0, 0, 99999, true, true);

                                GenerateRemoveButton(StockGrid, "btnRemove", row_id, "Remove", true);
                                SpecialGridControl(StockGrid, ["Remove"]);
                                SpecialGridControl(StockGrid, ["DestinationShelfNo"]);
                                SpecialGridControl(StockGrid, ["TransferQty"]);

                                var DestinationID = "#" + GenerateGridControlID(DestinationShelfNo, row_id);

                                if (result.DestinationShelfNo != "") {
                                    $(DestinationID).val(result.DestinationShelfNo);
                                    $(DestinationID).SetDisabled(true);
                                }
                                else {
                                    $(DestinationID).val("");
                                    $(DestinationID).SetDisabled(false);
                                }

                                BindGridButtonClickEvent("btnRemove", row_id, function (rid) {

                                    ajax_method.CallScreenController("/inventory/IVS080_DelElem", {
                                        InstrumentCode: StockGrid.cells(rid, StockGrid.getColIndexById("Instrumentcode")).getValue()
                                                            , ShelfNo: StockGrid.cells(rid, StockGrid.getColIndexById("SourceShelfNo")).getValue()
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
        StockGrid = $("#EliminateInst").InitialGrid(0, false, "/inventory/IVS080_TransferStockInstGrid");
    }
}

function DisplayRequireApprove() {
    var chkShow = false;
    var source = $("#SourceArea").val();
    var destination = $("#DestinationArea").val();

    if (source == "0" && destination == "5")
        chkShow = true;
    else if (source == "1" && destination == "4")
        chkShow = true;
    else if (source == "2" && destination == "4")
        chkShow = true;
    else if (source == "3" && (destination == "5" || destination == "6"))
        chkShow = true;
    else if (source == "4" && (destination == "1" || destination == "2" || destination == "5"))
        chkShow = true;
    else if (source == "6" && (destination == "3" || destination == "5"))
        chkShow = true;

    if(chkShow == true)
        $("#approve_require").show();
    else
        $("#approve_require").hide();
}

function DisplayContractCodeInput() {
    var chkShow = false;
    var source = $("#SourceArea").val();
    var destination = $("#DestinationArea").val();

    if (source == "4" && destination == "5")
        chkShow = true;

    if (chkShow == true) {
        $("#lblContractCode").show();
        $("#ContractCode").show();
    }
    else {
        $("#lblContractCode").hide();
        $("#ContractCode").hide();
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

    $("#btnSearch").click(IVS080_Search);
}

function IVS080_Search() {
    master_event.LockWindow(true);
    $("#btnSearch").attr("disabled", true);

    var obj = { Office: $("#Office").val(),
        Location: $("#Location").val(),
        SourceArea: $("#SourceArea").val(),
        DestinationArea: $("#DestinationArea").val(),
        InstName: $("#InstName").val(),
        InstCode: $("#InstCode").val(),
        ShelfNoFrom: $("#ShelfNoFrom").val(),
        ShelfNoTo: $("#ShelfNoTo").val()
    };

    $("#InstGrid").LoadDataToGrid(InstGrid, ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/inventory/IVS080_SearchInventoryInstrument", obj, "dtSearchInstrumentListResult", false
            , function (result, controls, isWarning) {
                if (controls != undefined) {
                    VaridateCtrl_AtLeast(["Office", "SourceArea", "DestinationArea", "InstCode", "InstName", "ShelfNoFrom", "ShelfNoTo"], controls);
                }
                else if (result == undefined) {
                    DeleteAllRow(InstGrid);
                }
                else if (isWarning == undefined) {
                    master_event.ScrollWindow("#ResultSection");
                    $("#Office").SetDisabled(true);
                    $("#Location").SetDisabled(true);
                    $("#SourceArea").SetDisabled(true);
                    $("#DestinationArea").SetDisabled(true);                    

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

    $("#Office").change(function () {

        var officeCode = $(this).val();

        var chkDisable = false;

        if (officeCode == $("#HeadOfficeCode").val()) {
            $("#divSourceArea").html(IVS080_InventoryAreaCbo.replace("{BlankID}", "SourceArea"));
            $('#DestinationArea >option').remove(":contains(':')");

            $("#SourceArea").change(function () {
                hideDestArea(officeCode, $(this).val());
            });

            chkDisable = false;
        }
        else if (officeCode == $("#SrinakarinOfficeCode").val()) {
            $("#divSourceArea").html(IVS080_InventoryAreaSrinakarinCbo.replace("{BlankID}", "SourceArea"));
            $('#DestinationArea >option').remove(":contains(':')");

            $("#SourceArea").change(function () {
                hideDestArea(officeCode, $(this).val());
            });

            chkDisable = true;
        }
        else if (officeCode != '') {
            $("#divSourceArea").html(IVS080_InventoryAreaDepoCbo.replace("{BlankID}", "SourceArea"));
            $('#DestinationArea >option').remove(":contains(':')");

            $("#SourceArea").change(function () {
                hideDestArea(officeCode, $(this).val());
            });

            chkDisable = true;
        }
        else {
            $('#SourceArea >option').remove(":contains(':')");
            $('#DestinationArea >option').remove(":contains(':')");

            chkDisable = false;
        }

        if (chkDisable == true) {
            $("#ShelfNoFrom").SetDisabled(true);
            $("#ShelfNoTo").SetDisabled(true);

            $("#ShelfNoFrom").val("");
            $("#ShelfNoTo").val("");
        }
        else {
            $("#ShelfNoFrom").SetDisabled(false);
            $("#ShelfNoTo").SetDisabled(false);
        }
    });
}

function hideDestArea(officeCode, sourceAreaCode) {
    if (officeCode == $("#HeadOfficeCode").val()) {
        $("#divDestArea").html(IVS080_InventoryAreaCbo.replace("{BlankID}", "DestinationArea"));
    }
    else if (officeCode == $("#SrinakarinOfficeCode").val()) {
        $("#divDestArea").html(IVS080_InventoryAreaSrinakarinCbo.replace("{BlankID}", "DestinationArea"));
    }
    else if (officeCode != '') {
        $("#divDestArea").html(IVS080_InventoryAreaDepoCbo.replace("{BlankID}", "DestinationArea"));
    }

    if (sourceAreaCode == "0") {
        $('#DestinationArea >option').remove(":contains('0')");
        $('#DestinationArea >option').remove(":contains('1')");
        $('#DestinationArea >option').remove(":contains('2')");
        $('#DestinationArea >option').remove(":contains('3')");
        $('#DestinationArea >option').remove(":contains('4')");
        $('#DestinationArea >option').remove(":contains('6')");
    }
    else if (sourceAreaCode == "1") {
        $('#DestinationArea >option').remove(":contains('0')");
        $('#DestinationArea >option').remove(":contains('1')");
        $('#DestinationArea >option').remove(":contains('3')");
        $('#DestinationArea >option').remove(":contains('5')");
        $('#DestinationArea >option').remove(":contains('6')");
    }
    else if (sourceAreaCode == "2") {
        $('#DestinationArea >option').remove(":contains('0')");
        $('#DestinationArea >option').remove(":contains('2')");
        $('#DestinationArea >option').remove(":contains('3')");
        $('#DestinationArea >option').remove(":contains('5')");
        $('#DestinationArea >option').remove(":contains('6')");
    }
    else if (sourceAreaCode == "3") {
        $('#DestinationArea >option').remove(":contains('0')");
        $('#DestinationArea >option').remove(":contains('1')");
        $('#DestinationArea >option').remove(":contains('2')");
        $('#DestinationArea >option').remove(":contains('3')");
        $('#DestinationArea >option').remove(":contains('4')");
    }
    else if (sourceAreaCode == "4") {
        $('#DestinationArea >option').remove(":contains('0')");
        $('#DestinationArea >option').remove(":contains('3')");
        $('#DestinationArea >option').remove(":contains('4')");
        $('#DestinationArea >option').remove(":contains('6')");
    }
    else if (sourceAreaCode == "5") {
        $('#DestinationArea >option').remove(":contains('0')");
        $('#DestinationArea >option').remove(":contains('1')");
        $('#DestinationArea >option').remove(":contains('2')");
        $('#DestinationArea >option').remove(":contains('3')");
        $('#DestinationArea >option').remove(":contains('4')");
        $('#DestinationArea >option').remove(":contains('5')");
    }
    else if (sourceAreaCode == "6") {
        $('#DestinationArea >option').remove(":contains('0')");
        $('#DestinationArea >option').remove(":contains('1')");
        $('#DestinationArea >option').remove(":contains('2')");
        $('#DestinationArea >option').remove(":contains('4')");
        $('#DestinationArea >option').remove(":contains('6')");
    }
}

function clear() {
    $("#searchSection").clearForm();
    //InstGrid = $("#InstGrid").InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/inventory/IVS080_InstGrid");
    DeleteAllRow(InstGrid);
}

function cmdRegister() {
    command_control.CommandControlMode(false);

    var InstList = new Array();
    for (var i = 0; i < StockGrid.getRowsNum(); i++) {
        var row_id = StockGrid.getRowId(i);
        var StockOutControlID = GenerateGridControlID(StockOutQty, row_id);
        var DestShelfNoControlID = GenerateGridControlID(DestinationShelfNo, row_id);
        var Inst = {
            InstrumentCode: StockGrid.cells(row_id, StockGrid.getColIndexById("Instrumentcode")).getValue(),
            InstrumentName: StockGrid.cells(row_id, StockGrid.getColIndexById("InstrumentName")).getValue(),
            SourceShelfNo: StockGrid.cells(row_id, StockGrid.getColIndexById("SourceShelfNo")).getValue(),
            ShelfNo: StockGrid.cells(row_id, StockGrid.getColIndexById("SourceShelfNo")).getValue(),
            InstrumentQty: StockGrid.cells(row_id, StockGrid.getColIndexById("InstrumentQty")).getValue(),
            //DestinationShelfNo: $("#" + GenerateGridControlID(DestinationShelfNo, row_id)).NumericValue(),
            DestinationShelfNo: ($("#" + GenerateGridControlID(DestinationShelfNo, row_id)).NumericValue()).toUpperCase(), //Modify by Jutarat A. on 18022013
            TransferQty: $("#" + GenerateGridControlID(StockOutQty, row_id)).NumericValue(),
            StockOutQty_id: StockOutControlID,
            DestShelfNo_id: DestShelfNoControlID,
            row_id: row_id
        };
        InstList.push(Inst);
    }


    var obj = { Office: $("#Office").val(),
        Location: $("#Location").val(),
        SourceArea: $("#SourceArea").val(),
        DestinationArea: $("#DestinationArea").val(),
        InstName: $("#InstName").val(),
        InstCode: $("#InstCode").val(),
        ShelfNoFrom: $("#ShelfNoFrom").val(),
        ShelfNoTo: $("#ShelfNoTo").val(),
        ApproveNo: $("#ApproveNo").val(),
        Memo: $("#memo").val(),
        StockInInstrument: InstList,
        ContractCode: $("#ContractCode").val(),
        TransferDate: $("#txtTransferDate").val()
    };

    $("#divGrid").ResetToNormalControl();

    ajax_method.CallScreenController("/inventory/IVS080_cmdReg", obj, function (result, controls) {
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
                $("#areaSection").SetViewMode(true);

                for (var i = 0; i < result.length; i++) {
                    var row_id = StockGrid.getRowId(i);
                    StockGrid.cells(row_id, StockGrid.getColIndexById("InstrumentQty")).setValue(result[i].InstrumentQty.toString());
                }           

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

    ajax_method.CallScreenController("/inventory/IVS080_cmdConfirm", null, function (result, controls) {
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
    $("#areaSection").SetViewMode(false);

    StockGrid.setSizes();

    DisplayRequireApprove();
    DisplayContractCodeInput();

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
    ajax_method.CallScreenController("/Inventory/IVS080_CheckExistFile", "", function (result) {
        if (result == 1) {
            var key = ajax_method.GetKeyURL(null);
            var url = ajax_method.GenerateURL("/Inventory/IVS080_DownloadPdfAndWriteLog?k=" + key);
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

function GeneratTextBox(grid, id, row_id, col_id, value, max_length, endable) {
    var disabled_txt = "";
    if (endable == false)
        disabled_txt = "readonly='readonly'";

    var fid = GenerateGridControlID(id, row_id);
    var col = grid.getColIndexById(col_id);

    var txt = "<input id='" + fid + "' name='" + fid + "' style='width:85px;margin-left:-2px;' type='text' maxlength='" + max_length + "' value='" + value + "' " + disabled_txt + " />";

    grid.cells(row_id, col).setValue(txt);

    fid = "#" + fid;
    $(fid).focus(function () {
        grid.selectRowById(row_id);
    });
}