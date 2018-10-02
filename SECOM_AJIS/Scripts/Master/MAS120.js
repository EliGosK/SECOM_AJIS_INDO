/// <reference path = "../../Scripts/jquery-1.5.1-vsdoc.js" />
/// <reference path = "../../Scripts/Base/GridControl.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxcommon.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxgrid.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxgridcell.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/ext/dhtmlxgrid_pgn.js" />
/// <reference path = "../../Scripts/Base/object/ajax_method.js" />
/// <reference path = "../../Scripts/Base/object/master_event.js" />

var mygrid = null;
var btnGridEdit = "btnGridEdit";
var mode;
var isSelected = false;
var selectedRowId;
var OldShelfTypeCode;
$(document).ready(function () {

    $("#Result_List").hide();
    $("#Result_Detail").hide();

    $("#InstrumentCode").attr("readonly", true);
    $("#InstrumentName").attr("readonly", true);
    $("#AreaCode").attr("readonly", true);

    $("#btnNew").attr("disabled", !permission.ADD);

    $("#ShelfTypeCode").change(changeShelfTypeCode);

    $("#btnSearch").click(function () {
        //$("#btnSearch").attr("disabled", true);
        searchShelf();
    });
    $("#btnClear").click(function () {
        $("#MAS120_Search").clearForm();
        $("#Result_List").hide();
        mygrid.clearAll();
        $("#Result_Detail").hide();
    });
    $("#btnNew").click(function () {
        newShelf();
        mode = "new";
    });
    $("#DeleteFlag").click(function () {
        if (permission.EDIT) {
            setDisableControlForDeleteFlag($("#DeleteFlag").prop("checked"));

            $("#ShelfNo").attr("readonly", true);
            if ($("#ShelfTypeCode").val() != MAS120_Constant.C_INV_SHELF_TYPE_NORMAL) {
                $("#ShelfTypeCode").attr("disabled", true);
            }
            else {
                var obj = { ShelfNo: $("#ShelfNo").val() };
                call_ajax_method_json("/Master/MAS120_CheckShelfNo", obj, function (result, controls) {
                    if (result) {
                        $("#ShelfTypeCode").find("option").each(function () {
                            if ($(this).val() == MAS120_Constant.C_INV_SHELF_TYPE_PROJECT)
                                $(this).attr("disabled", true);
                        });
                    }
                    else {
                        $("#ShelfTypeCode").attr("disabled", true);
                    }
                });
            }


        }
    });

    $("#Search_Criteria input[id=txtShelfName]").InitialAutoComplete("/Master/GetShelfName");
    $("#Result_Detail input[id=ShelfName]").InitialAutoComplete("/Master/GetShelfName");

    mygrid = $("#grid_result").InitialGrid(ROWS_PER_PAGE_FOR_VIEWPAGE, true, "/Master/MAS120_InitGrid");

    SpecialGridControl(mygrid, ["Edit"]);

    mygrid.attachEvent("onBeforeSelect"
        , function (new_row, old_row) {
            if (isSelected)
                return false;
            else
                return true;
        }
    );
    mygrid.attachEvent("onBeforeSorting"
        , function (ind, type, direction) {
            if (isSelected)
                return false;
            else
                return true;
        }
    );
    mygrid.attachEvent("onBeforePageChanged"
        , function (ind, count) {
            if (isSelected)
                return false;
            else
                return true;
        }
    );

    BindOnLoadedEvent(mygrid
        , function (gen_ctrl) {
            if (mygrid.getRowsNum() != 0) {
                for (var i = 0; i < mygrid.getRowsNum(); i++) {
                    var row_id = mygrid.getRowId(i);

                    if (gen_ctrl == true) {
                        GenerateEditButton(mygrid, btnGridEdit, row_id, "Edit", true);
                    }

                    BindGridButtonClickEvent(btnGridEdit
                        , row_id
                        , function (rid) {
                            btnGridEditClick(rid)
                        }
                    );
                }
            }
        }
    );
});

function searchShelf() {

    //########### Modify by Siripoj S. 13-06-2012 #############//
    // disable search button
    $("#btnSearch").attr("disabled", true);
    master_event.LockWindow(true);

    $("#Result_Detail").hide();
    if ($("#grid_result").length > 0) {
        var param = CreateObjectData($("#MAS120_Search").serialize());
        $("#grid_result").LoadDataToGrid(mygrid, ROWS_PER_PAGE_FOR_VIEWPAGE
            , false
            , "/Master/MAS120_Search"
            , param
            , "doShelf"
            , false
            , function () {
                //$("#btnSearch").attr("disabled", false);
                $("#btnSearch").attr("disabled", false);
                master_event.LockWindow(false);
            }
            , function (result, controls, isWarning) {
                if (result != undefined && isWarning == undefined) {
                    $("#Result_List").show();

                    // setTimeout("document.getElementById('Result_List').scrollIntoView()", 1500);
                    master_event.ScrollWindow("#Result_List", false);
                }
            }
        );
    }
}

function newShelf() {
    $("#ShelfTypeCode").find("option").each(function () {
        if ($(this).val() == MAS120_Constant.C_INV_SHELF_TYPE_NORMAL)
            $(this).attr("disabled", true);
    });
    setDisableSearchSection(true);
    setDisableGridButton(true);
    $("#MAS120_ShelfDetail").clearForm();
    $("#DeleteFlag").attr("disabled", true);
    $("#Result_Detail").show();
    if (permission.ADD) {
        setDisableControlForDeleteFlag(false);
    }
    SetConfirmCommand(true, confirmCommand);
    SetCancelCommand(true, cancelCommand);
}

function shelfSearchDetail(ShelfNo) {
    var param = { ShelfNo: ShelfNo };
    call_ajax_method_json('/Master/MAS120_GetShelfDetail'
        , param
        , function (result, controls) {
            if (result != undefined) {
                $("#MAS120_ShelfDetail").clearForm();
                $("#MAS120_ShelfDetail").bindJSON(result);
                $("#HiddenAreaCode").val(result.AreaCode);
                $("#UpdateDate").val(ConvertDateObject(result.UpdateDate));
                $("#UpdateBy").val(result.UpdateBy);
                $("#AreaCode").val(result.AreaCodeName);
                setDisableControlForDeleteFlag(!permission.EDIT);
                if (permission.EDIT) {
                    $("#ShelfNo").attr("readonly", true);
                }

                $("#DeleteFlag").attr("disabled", !permission.DEL);
                $("#Result_Detail").show();
                if (permission.EDIT || permission.DEL) {
                    SetConfirmCommand(true, confirmCommand);
                }
                SetCancelCommand(true, cancelCommand);
                OldShelfTypeCode = $("#ShelfTypeCode").val();
                if ($("#ShelfTypeCode").val() != MAS120_Constant.C_INV_SHELF_TYPE_NORMAL) {
                    $("#ShelfTypeCode").attr("disabled", true);


                }
                else {
                    var obj = { ShelfNo: $("#ShelfNo").val() };
                    call_ajax_method_json("/Master/MAS120_CheckShelfNo", obj, function (result, controls) {
                        if (result) {
                            $("#ShelfTypeCode").find("option").each(function () {
                                if ($(this).val() == MAS120_Constant.C_INV_SHELF_TYPE_PROJECT)
                                    $(this).attr("disabled", true);
                            });
                        }
                        else {
                            $("#ShelfTypeCode").attr("disabled", true);
                        }
                    });
                }


            }
        }
    );
}

function confirmCommand() {
    //$("#ShelfTypeCode").attr("disabled", false);
    SetConfirmCommand(false, confirmCommand);
    if (mode == "edit" && $("#DeleteFlag").prop("checked")) { mode = "delete"; }
    switch (mode) {
        case "new":
            confirmNew();
            break;
        case "edit":
            confirmEdit();
            break;
        case "delete":
            confirmDelete();
            break;
    }
//    if (mode == "edit" && $("#ShelfTypeCode").val() != MAS120_Constant.C_INV_SHELF_TYPE_NORMAL) {
//        $("#ShelfTypeCode").attr("disabled", true);


//    }
//    else {
//        var obj = { ShelfNo: $("#ShelfNo").val() };
//        call_ajax_method_json("/Master/MAS120_CheckShelfNo", obj, function (result, controls) {
//            if (result) {
//                $("#ShelfTypeCode").find("option").each(function () {
//                    if ($(this).val() == MAS120_Constant.C_INV_SHELF_TYPE_PROJECT)
//                        $(this).attr("disabled", true);
//                });
//            }
//            else {
//                $("#ShelfTypeCode").attr("disabled", true);
//            }
//        });
//    }


}

function confirmNew() {
    command_control.CommandControlMode(false);
    var param = { ShelfNo: $("#ShelfNo").val(),
        ShelfName: $("#ShelfName").val(),
        ShelfTypeCode: $("#ShelfTypeCode").val(),
        DeleteFlag: $("#DeleteFlag").prop("checked")
    };
    call_ajax_method_json('/Master/MAS120_InsertShelf'
        , param
        , function (result, controls) {
            command_control.CommandControlMode(true);
            if (controls != undefined) {
                VaridateCtrl(["ShelfNo"
                             , "ShelfName"
                             , "ShelfTypeCode"], controls);
                SetConfirmCommand(true, confirmCommand);
            }
            else if (result == "ConfirmUpdate") {
                // Get Message
                var obj = {
                    module: "Master",
                    code: "MSG1055"
                };

                call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
                    OpenYesNoMessageDialog(result.Code, result.Message, confirmUpdateCaseDuplicate, function () {
                        SetConfirmCommand(true, confirmCommand);
                    });

                });
            }
            else if (result != undefined) {
                var param = { "module": "Common", "code": "MSG0046" };
                call_ajax_method_json("/Shared/GetMessage"
                    , param
                    , function (data) {
                        OpenInformationMessageDialog(data.Code, data.Message
                            , function () {
                                addDetailGrid(result);
                                setAllControlSaveSuccess();
                                mygrid.selectRow(mygrid.getRowsNum() - 1);
                            }
                        );
                    }
                );
            }
            else {
                SetConfirmCommand(true, confirmCommand);
            }
        }
    );
}

function confirmEdit() {
    command_control.CommandControlMode(false);
    var param = CreateObjectData($("#MAS120_ShelfDetail").serialize(), true);
    param.ShelfTypeCode = $("#ShelfTypeCode").val();
    param.AreaCode = $("#HiddenAreaCode").val();
    call_ajax_method_json('/Master/MAS120_UpdateShelf'
        , param
        , function (result, controls) {
            command_control.CommandControlMode(true);
            if (controls != undefined) {
                VaridateCtrl(["ShelfNo"
                             , "ShelfName"
                             , "ShelfTypeCode"], controls);
                SetConfirmCommand(true, confirmCommand);
            }
            if (result != undefined) {
                var param = { "module": "Common", "code": "MSG0046" };
                call_ajax_method_json("/Shared/GetMessage"
                    , param
                    , function (data) {
                        OpenInformationMessageDialog(data.Code, data.Message
                            , function () {
                                editDetailGrid(result);
                                setAllControlSaveSuccess();
                            }
                        );
                    }
                );
            }
        }
    );
}

function confirmUpdateCaseDuplicate() {
    command_control.CommandControlMode(false);
    var param = CreateObjectData($("#MAS120_ShelfDetail").serialize(), true);
    param.AreaCode = $("#HiddenAreaCode").val();
    call_ajax_method_json('/Master/MAS120_UpdateShelf'
        , param
        , function (result, controls) {
            command_control.CommandControlMode(true);
            if (controls != undefined) {
                VaridateCtrl(["ShelfNo"
                             , "ShelfName"
                             , "ShelfTypeCode"], controls);
                SetConfirmCommand(true, confirmCommand);
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
                                mygrid.selectRow(mygrid.getRowsNum() - 1);
                            }
                        );
                    }
                );
            }
        }
    );
}

function confirmDelete() {
    command_control.CommandControlMode(false);
    var param = { "module": "Master", "code": "MSG1033", param: $("#ShelfNo").val() };

    call_ajax_method_json("/Shared/GetMessage"
        , param
        , function (data, ctrl) {
            OpenYesNoMessageDialog(data.Code, data.Message
                , function () {
                    command_control.CommandControlMode(true);
                    var DeleteFlag = $("#DeleteFlag").prop("checked");
                    var param = CreateObjectData($("#MAS120_ShelfDetail").serialize() + "&DeleteFlag=" + DeleteFlag + "&ShelfTypeCode=" + $("#ShelfTypeCode").val(), true);
                    param.AreaCode = $("#HiddenAreaCode").val();
                    call_ajax_method_json('/Master/MAS120_UpdateShelf'
                        , param
                        , function (result, controls) {
                            if (result != undefined) {
                                var param = { "module": "Master", "code": "MSG1035", param: $("#ShelfNo").val() };
                                call_ajax_method_json("/Shared/GetMessage"
                                    , param
                                    , function (data) {
                                        OpenInformationMessageDialog(data.Code, data.Message
                                            , function () {
                                                DeleteRow(mygrid, selectedRowId);
                                                setAllControlSaveSuccess();
                                            }
                                        );
                                    }
                                );
                            }
                            else if (controls == null) {
                                var param = { "module": "Master", "code": "MSG1034", param: $("#ShelfNo").val() };
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
                            else {
                                SetConfirmCommand(true, confirmCommand);
                            }
                        }
                    );
                }
                , function () {
                    SetConfirmCommand(true, confirmCommand);
                }
            );
        }
    );

}

function cancelCommand() {

    var obj = {
        module: "Common",
        code: "MSG0140"
    };
    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message
                , function () {
                    isSelected = false;
                    $("#DeleteFlag").attr("disabled", false);
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
    });
}

function addDetailGrid(result) {
    if (result != undefined) {
        CheckFirstRowIsEmpty(mygrid, true);
        var strShelfTypeName = $("#ShelfTypeCode option:selected").text();
        strShelfTypeName = strShelfTypeName.substring(strShelfTypeName.indexOf(":") + 1)
        AddNewRow(mygrid,
            ["",
            result.ShelfNo,
            $("#ShelfName").val(),
            //$("#ShelfTypeCode").val(),
            strShelfTypeName,
            "",
            "",
            ""]);
        var row_id = mygrid.getRowId(mygrid.getRowsNum() - 1);
        GenerateEditButton(mygrid, btnGridEdit, row_id, "Edit", true);
        mygrid.setSizes();
        BindGridButtonClickEvent(btnGridEdit
            , row_id
            , function (rid) {
                btnGridEditClick(rid);
                mygrid.changePage(Math.ceil(mygrid.getRowsNum() / ROWS_PER_PAGE_FOR_VIEWPAGE));
            }
        );
    }
}

function editDetailGrid(result) {
    //mygrid.cells2(mygrid.getRowIndex(selectedRowId), mygrid.getColIndexById('ShelfNo')).setValue($("#ShelfNo").val());
    mygrid.cells2(mygrid.getRowIndex(selectedRowId), mygrid.getColIndexById('ShelfNo')).setValue(result.ShelfNo); //Modify by Jutarat A. on 28022013

    mygrid.cells2(mygrid.getRowIndex(selectedRowId), mygrid.getColIndexById('ShelfName')).setValue($("#ShelfName").val());
    mygrid.cells2(mygrid.getRowIndex(selectedRowId), mygrid.getColIndexById('ShelfTypeCode')).setValue($("#ShelfTypeCode").val());
    mygrid.cells2(mygrid.getRowIndex(selectedRowId), mygrid.getColIndexById('InstrumentName')).setValue($("#InstrumentName").val());
    mygrid.cells2(mygrid.getRowIndex(selectedRowId), mygrid.getColIndexById('AreaCode')).setValue($("#AreaCode").val());

    var strShelfTypeName = $("#ShelfTypeCode option:selected").text();
    strShelfTypeName = strShelfTypeName.substring(strShelfTypeName.indexOf(":") + 1);
    mygrid.cells2(mygrid.getRowIndex(selectedRowId), mygrid.getColIndexById('ShelfTypeCodeName')).setValue(strShelfTypeName);
    if (result.InstrumentCode == null || result.InstrumentCode == "") {
        mygrid.cells2(mygrid.getRowIndex(selectedRowId), mygrid.getColIndexById('InstrumentName')).setValue("");
    }
    if (result.AreaCode == null || result.AreaCode == "") {
        mygrid.cells2(mygrid.getRowIndex(selectedRowId), mygrid.getColIndexById('AreaCodeName')).setValue("");
        mygrid.cells2(mygrid.getRowIndex(selectedRowId), mygrid.getColIndexById('AreaCode')).setValue("");
    }

}

function btnGridEditClick(rid) {
    mygrid.selectRow(mygrid.getRowIndex(rid));
    selectedRowId = rid;
    isSelected = true;
    setDisableSearchSection(true);
    setDisableGridButton(true);
    var shelfNo = mygrid.cells(rid, mygrid.getColIndexById('ShelfNo')).getValue();
    shelfSearchDetail(shelfNo);
    $("#ShelfTypeCode").find("option").each(function () {
        if ($(this).val() == MAS120_Constant.C_INV_SHELF_TYPE_NORMAL)
            $(this).attr("disabled", false);
    });
    mode = "edit";

}

function setAllControlSaveSuccess() {
    isSelected = false;
    $("#Result_List").show();
    $("#Result_Detail").hide();
    $("#MAS120_ShelfDetail").clearForm();
    SetConfirmCommand(false, confirmCommand);
    SetCancelCommand(false, cancelCommand);
    setDisableGridButton(false);
    setDisableSearchSection(false);
    if (mode = "delete") {
        setDisableControlForDeleteFlag(false);
        $("#DeleteFlag").attr("disabled", false);
    }
}

function setDisableControlForDeleteFlag(isDisable) {
    $("#ShelfNo").attr("readonly", isDisable);
    $("#ShelfName").attr("readonly", isDisable);
    $("#ShelfTypeCode").attr("disabled", isDisable);
}

function setDisableSearchSection(isDisable) {
    $("#MAS120_Search input").attr("readonly", isDisable);
    $("#MAS120_Search button").attr("disabled", isDisable);
    $("#MAS120_Search select").attr("disabled", isDisable);
    if (!isDisable) {
        $("#btnNew").attr("disabled", !permission.ADD);
    }
}

function setDisableGridButton(isDisable) {
    var isEnable = !isDisable;
    if ($("#grid_result").length > 0) {
        if (mygrid.getRowsNum() != 0) {
            for (var i = 0; i < mygrid.getRowsNum(); i++) {
                var row_id = mygrid.getRowId(i);
                EnableGridButton(mygrid, btnGridEdit, row_id, "Edit", isEnable);
            }
        }
        mygrid.attachEvent("onAfterSorting"
            , function (index, type, direction) {
                for (var i = 0; i < mygrid.getRowsNum(); i++) {
                    var row_id = mygrid.getRowId(i);
                    EnableGridButton(mygrid, btnGridEdit, row_id, "Edit", isEnable);
                }
            }
        );
    }
}

function changeShelfTypeCode() {
    if (mode == "edit") {
        if (OldShelfTypeCode == $("#ShelfTypeCode").val()) {
            return true;
        }

        if ($("#ShelfTypeCode").val() == MAS120_Constant.C_INV_SHELF_TYPE_EMPTY) {

            var obj = { ShelfNo: $("#ShelfNo").val() };
            call_ajax_method_json("/Master/MAS120_CheckShelfNo", obj, function (result, controls) {
                if (result) {

                }
                else {
                    $("#ShelfTypeCode").val(OldShelfTypeCode);
                    doAlert("Master", "MSG1046", "");
                }
            });
        }
        else {
            $("#ShelfTypeCode").val(OldShelfTypeCode);
            doAlert("Master", "MSG1046", "");
        }
    }
}

function doAlert(moduleCode, msgCode, paramObj) {
    var obj = {
        module: moduleCode,
        code: msgCode,
        param: paramObj
    };

    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
        OpenWarningDialog(result.Message, result.Message, null);
    });
}