/// <reference path = "../../Scripts/jquery-1.5.1-vsdoc.js" />
/// <reference path = "../../Scripts/Base/GridControl.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxcommon.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxgrid.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxgridcell.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/ext/dhtmlxgrid_pgn.js" />
/// <reference path = "../../Scripts/Base/object/ajax_method.js" />
/// <reference path = "../../Scripts/Base/object/master_event.js" />

var mygrid = null;
var modeOfComfirmCommand;
var modeOfDlgBox;
var isSelected = false;
var selectedRowId;

$(document).ready(function () {

    $("#Result_List").hide();
    $("#Result_Detail").hide();

    $("#btnNew").attr("disabled", !permission.ADD);

    $("#InventoryFixedQuantity").BindNumericBox(10, 0, 0, 9999999999);

    $("#btnSearchInstrumentSearch").click(function () {

        modeOfDlgBox = "search";
        $('#dlgBox').OpenCMS170Dialog("CTS230");

        // enable serch customer button (set delay 3 sec.)
        setTimeout(
        function () {
            $("#btnSearchInstrumentSearch").attr("disabled", false);
        }, 1000);

    });
    $("#btnSearchInstrument").click(function () {

        //$("#btnSearchInstrument").attr("disabled", true);

        modeOfDlgBox = "detail";
        $('#dlgBox').OpenCMS170Dialog("CTS230");

        // enable serch customer button (set delay 3 sec.)
        setTimeout(
        function () {
            $("#btnSearchInstrument").attr("disabled", false);
        }, 1000);

    });
    $("#btnSearch").click(function () {
        //$("#btnSearch").attr("disabled", true);
        searchSafetyStock();
    });
    $("#btnClear").click(function () {
        $("#MAS130_SearchCriteria").clearForm();
        $("#Result_List").hide();
        mygrid.clearAll();
        $("#Result_Detail").hide();
        $("#MAS110_ResultDetail").clearForm();
    });
    $("#btnNew").click(function () {
        call_ajax_method_json("/Master/MAS130_Authority", null, function (result) {
            if (result != null) {
                modeOfComfirmCommand = "new";
                newSafetyStock();
            }
        });
    });

    $("#InstrumentCodeSearch").blur(function () {

        if ($("#InstrumentCodeSearch").val() != "") {
            var param = { InstrumentCodeSearch: $("#InstrumentCodeSearch").val() };
            ajax_method.CallScreenController("/Master/MAS130_GetInstrumentName", param,
            function (result) {
                if (result != undefined) {
                    $("#InstrumentNameSearch").val(result);
                }
            });
        }

    });

    mygrid = $("#grid_result").InitialGrid(ROWS_PER_PAGE_FOR_VIEWPAGE, true, "/Master/MAS130_InitGrid");

    SpecialGridControl(mygrid, ["Edit"]);

    mygrid.attachEvent("onBeforeSelect", function (new_row, old_row) { return !isSelected; });
    mygrid.attachEvent("onBeforeSorting", function (ind, type, direction) { return !isSelected; });
    mygrid.attachEvent("onBeforePageChanged", function (ind, count) { return !isSelected; });

    BindOnLoadedEvent(mygrid
        , function (gen_ctrl) {
            if (mygrid.getRowsNum() != 0) {
                for (var i = 0; i < mygrid.getRowsNum(); i++) {
                    var row_id = mygrid.getRowId(i);

                    if (gen_ctrl == true) {
                        GenerateEditButton(mygrid, "btnGridEdit", row_id, "Edit", true);
                    }

                    BindGridButtonClickEvent("btnGridEdit", row_id, function (rid) { btnGridEditClick(rid); });
                }
            }
        }
    );
});

    function searchSafetyStock() {

    //########### Modify by Siripoj S. 13-06-2012 #############//
    // disable search button
    $("#btnSearch").attr("disabled", true);
    master_event.LockWindow(true);

    $("#Result_Detail").hide();
    if ($("#grid_result").length > 0) {
        var param = { InstrumentCodeSearch: $("#InstrumentCodeSearch").val() };
        $("#grid_result").LoadDataToGrid(mygrid, ROWS_PER_PAGE_FOR_VIEWPAGE
            , false
            , "/Master/MAS130_Search"
            , param
            , "doSafetyStock"
            , false
            , function () {
                //$("#btnSearch").attr("disabled", false); 
                $("#btnSearch").attr("disabled", false);
                master_event.LockWindow(false);
             }
            , function (result, controls, isWarning) {
                if (result != undefined && isWarning == undefined) {
                    $("#Result_List").show();
                    master_event.ScrollWindow("#Result_List", false);
                } 
             }
        );
    }
}

function newSafetyStock() {
    $("#btnSearchInstrument").attr("disabled", false);
    setDisableSearchSection(true);
    setDisableGridButton(true);
    $("#MAS130_ResultDetail").clearForm();
    $("#Result_Detail").show();
    SetConfirmCommand(true, confirmCommand);
    SetCancelCommand(true, cancelCommand);
}

function searchSafetyStockDetail(InstrumentCode) {
    var param = { InstrumentCode: InstrumentCode };
    call_ajax_method_json('/Master/MAS130_SearchDetail'
        , param
        , function (result, controls) {
            if (result != undefined) {
                $("#MAS130_ResultDetail").clearForm();
                $("#MAS130_ResultDetail").bindJSON(result);
                $("#UpdateBy").val(result.UpdateBy);
                $("#Result_Detail").show();
                SetConfirmCommand(permission.EDIT, confirmCommand);
                SetCancelCommand(true, cancelCommand);
            }
        }
    );
}

function confirmCommand() {
    DisableConfirmCommand(true);
    switch (modeOfComfirmCommand) {
        case "new":
            confirmNew();
            break;
        case "edit":
            $("#btnSearchInstrumentSearch").attr("disabled", true);
            confirmEdit();
            break;
    }
}

function cancelCommand() {
    var param = { "module": "Common", "code": "MSG0028", param: msgCancel };
    call_ajax_method_json("/Shared/GetMessage"
        , param
        , function (data, ctrl) {
            OpenYesNoMessageDialog(data.Code, data.Message
                , function () {
                    isSelected = false;
                    setDisableSearchSection(false);
                    setDisableGridButton(false);
                    SetConfirmCommand(false, confirmCommand);
                    SetCancelCommand(false, cancelCommand);
                    $("#Result_Detail").hide();
                }
                , function () {
                    SetConfirmCommand(true, confirmCommand);
                }
            );
        }
    );
}

function confirmNew() {

    //    var param = { InstrumentCode: $("#InstrumentCode").val(), InventoryFixedQuantity: $("#InventoryFixedQuantity").NumericValue()};
    //    call_ajax_method_json('/Master/MAS130_Insert'
    //                        , param
    //                        , function (result, controls) {
    //                            $("#btnSearchInstrumentSearch").attr("disabled", false);
    //                            DisableConfirmCommand(false);

    //                            if (controls != undefined) {
    //                                VaridateCtrl(["InstrumentCode"], controls);
    //                            }
    //                            else if (result != undefined) {
    //                                var param = { "module": "Common", "code": "MSG0046" };
    //                                call_ajax_method_json("/Shared/GetMessage"
    //                                    , param
    //                                    , function (data) {
    //                                        OpenInformationMessageDialog(data.code, data.Message
    //                                            , function () {
    //                                                addDetailGrid(result);
    //                                                setAllControlSaveSuccess();
    //                                                mygrid.changePage(Math.ceil(mygrid.getRowsNum() / ROWS_PER_PAGE_FOR_VIEWPAGE));
    //                                                mygrid.selectRow(mygrid.getRowsNum() - 1);
    //                                            }
    //                                        );
    //                                    }
    //                                );
    //                            }
    //                        }
    //                    );
    command_control.CommandControlMode(false);
    var param = { InstrumentCode: $("#InstrumentCode").val() };
    call_ajax_method_json('/Master/MAS130_SearchDetail'
            , param
            , function (result, controls) {
                command_control.CommandControlMode(true);
                if (result == undefined) {
                    var param = { InstrumentCode: $("#InstrumentCode").val(), InventoryFixedQuantity: $("#InventoryFixedQuantity").NumericValue(), UpdateBy: $("#UpdateBy").val() };
                    call_ajax_method_json('/Master/MAS130_Insert'
                        , param
                        , function (result, controls) {

                            DisableConfirmCommand(false);

                            if (controls != undefined) {
                                VaridateCtrl(["InstrumentCode"], controls);
                            }
                            if (result != undefined) {
                                var param = { "module": "Common", "code": "MSG0046" };
                                call_ajax_method_json("/Shared/GetMessage"
                                    , param
                                    , function (data) {
                                        OpenInformationMessageDialog(data.Code, data.Message
                                            , function () {
                                                addDetailGrid(result);
                                                setAllControlSaveSuccess();
                                                mygrid.changePage(Math.ceil(mygrid.getRowsNum() / ROWS_PER_PAGE_FOR_VIEWPAGE));
                                                mygrid.selectRow(mygrid.getRowsNum() - 1);
                                            }
                                        );
                                    }
                                );
                            }
                        }
                    );
                }
                else {
                    var param = { "module": "Master", "code": "MSG1037", param: $("#InstrumentCode").val() };
                    call_ajax_method_json("/Shared/GetMessage"
                        , param
                        , function (data) {
                            OpenInformationMessageDialog(data.Code, data.Message
                                , function () {
                                    SetConfirmCommand(true, confirmCommand);
                                }
                            );
                        }
                    );
                }
            }
        );
}

function confirmEdit() {
    command_control.CommandControlMode(false);
    var param = { InstrumentCode: $("#InstrumentCode").val(), InventoryFixedQuantity: $("#InventoryFixedQuantity").NumericValue(), UpdateBy: $("#UpdateBy").val() };
    call_ajax_method_json('/Master/MAS130_Update'
        , param
        , function (result, controls) {
            command_control.CommandControlMode(true);
            if (controls != undefined) {
                VaridateCtrl(["InstrumentCode"], controls);
                SetConfirmCommand(true, confirmCommand);
            }
            if (result != undefined) {
                var param = { "module": "Common", "code": "MSG0046" };
                call_ajax_method_json("/Shared/GetMessage"
                    , param
                    , function (data) {
                        OpenInformationMessageDialog(data.Code, data.Message
                            , function () {
                                editDetailGrid();
                                setAllControlSaveSuccess();
                            }
                        );
                    }
                );
            }
        }
    );
}

function btnGridEditClick(rid) {
    mygrid.selectRow(mygrid.getRowIndex(rid));
    selectedRowId = rid;
    isSelected = true;
    setDisableSearchSection(true);
    setDisableGridButton(true);
    $("#btnSearchInstrument").attr("disabled", true);
    var instrumentCodeSelected = mygrid.cells(rid, mygrid.getColIndexById('InstrumentCode')).getValue();
    searchSafetyStockDetail(instrumentCodeSelected);
    modeOfComfirmCommand = "edit";
}

function addDetailGrid(result) {
    if (result != undefined) {
        CheckFirstRowIsEmpty(mygrid, true);
        AddNewRow(mygrid,
            ["",
            ConvertBlockHtml($("#InstrumentCode").val()), //$("#InstrumentCode").val(), //Modify by Jutarat A. on 28112013
            ConvertBlockHtml($("#InstrumentName").val()), //$("#InstrumentName").val(), //Modify by Jutarat A. on 28112013
            $("#InventoryFixedQuantity").NumericValue(),
            ""]);
        var row_id = mygrid.getRowId(mygrid.getRowsNum() - 1);
        GenerateEditButton(mygrid, "btnGridEdit", row_id, "Edit", true);
        mygrid.setSizes();
        BindGridButtonClickEvent("btnGridEdit"
            , row_id
            , function (rid) {
                btnGridEditClick(rid);
            }
        );
    }
}

function editDetailGrid() {
    mygrid.cells2(mygrid.getRowIndex(selectedRowId), mygrid.getColIndexById('InstrumentCode')).setValue($("#InstrumentCode").val());
    mygrid.cells2(mygrid.getRowIndex(selectedRowId), mygrid.getColIndexById('InstrumentName')).setValue($("#InstrumentName").val());
    mygrid.cells2(mygrid.getRowIndex(selectedRowId), mygrid.getColIndexById('InventoryFixedQuantity')).setValue($("#InventoryFixedQuantity").NumericValue());
}

function CMS170Response(newInst) {
    switch (modeOfDlgBox) {
        case "search":
            $("#InstrumentCodeSearch").val(newInst.InstrumentCode);
            $("#InstrumentNameSearch").val(newInst.InstrumentName);
            break;
        case "detail":
            $("#InstrumentCode").val(newInst.InstrumentCode);
            $("#InstrumentName").val(newInst.InstrumentName);
            break;
    }
    $("#dlgBox").CloseDialog();
}

function CMS170Object() {
    return { bExpTypeHas: true,
        bExpTypeNo: true,
        bProdTypeSale: true,
        bProdTypeAlarm: true,
        bInstTypeGen: true,
        bInstTypeMonitoring: true,
        bInstTypeMat: true
    };
}

function setAllControlSaveSuccess() {
    isSelected = false;
    $("#Result_List").show();
    $("#Result_Detail").hide();
    $("#MAS130_ResultDetail").clearForm();
    SetConfirmCommand(false, confirmCommand);
    SetCancelCommand(false, cancelCommand);
    setDisableGridButton(false);
    setDisableSearchSection(false);
}

function setDisableSearchSection(isDisable) {
    $("#MAS130_SearchCriteria input").attr("readonly", isDisable);
    $("#MAS130_SearchCriteria button").attr("disabled", isDisable);
    $("#MAS130_SearchCriteria select").attr("disabled", isDisable);
    if (!isDisable) {
        $("#btnNew").attr("disabled", !permission.ADD);
    }

    // iggnore
    $("#InstrumentNameSearch").attr("readonly", true);
}

function setDisableGridButton(isDisable) {
    var isEnable = !isDisable;
    if ($("#grid_result").length > 0) {
        if (mygrid.getRowsNum() != 0) {
            for (var i = 0; i < mygrid.getRowsNum(); i++) {
                var row_id = mygrid.getRowId(i);
                EnableGridButton(mygrid, "btnGridEdit", row_id, "Edit", isEnable);
            }
        }
        mygrid.attachEvent("onAfterSorting"
            , function (index, type, direction) {
                for (var i = 0; i < mygrid.getRowsNum(); i++) {
                    var row_id = mygrid.getRowId(i);
                    EnableGridButton(mygrid, "btnGridEdit", row_id, "Edit", isEnable);
                }
            }
        );
    }
}