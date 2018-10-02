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
var isSelected = false;
var selectedRowId;

$(document).ready(function () {

    $("#Result_List").hide();
    $("#Result_Detail").hide();
    $("#SubContractorCode").attr("readonly", true);
    $("#btnNew").attr("disabled", !permission.ADD);

    $("#NoInstallStaff").BindNumericBox(4, 0, 0, 9999);
    InitialNumericInputTextBox(["PhoneNo"]);

    $("#btnSearch").click(function () {
        //$("#btnSearch").attr("disabled", true);
        searchSubcontractor();
    });
    $("#btnClear").click(function () {
        $("#MAS110_SearchCriteria").clearForm();
        $("#Result_List").hide();
        mygrid.clearAll();
        $("#Result_Detail").hide();
        $("#MAS110_ResultDetail").clearForm();
    });
    $("#btnNew").click(function () {
        call_ajax_method_json("/Master/MAS110_Authority", null, function (result) {
            if (result != null) {
                modeOfComfirmCommand = "new";
                newSubcontractor();
                $("#COCompanyCode").attr("readonly", false);
                $("#InstallationTeam").attr("readonly", false);
            }
        });
    });
    $("#DeleteFlag").click(function () {
        if (permission.EDIT) {
            setDisableControlForDeleteFlag($("#DeleteFlag").prop("checked"));
        }
    });
    $("#SubInstallationFlag").click(function () {
        setDisableControlForSubInstallationFlag(!$("#SubInstallationFlag").prop("checked"));
    });
    $("#SubMaintenanceFlag").click(function () {
        setDisableControlForSubMaintenanceFlag(!$("#SubMaintenanceFlag").prop("checked"));
    });

    $("#Search_Criteria input[id=SubcontractorNameSearch]").InitialAutoComplete("/Master/GetSubcontractorName");
    $("#Result_Detail input[id=SubContractorNameEN]").InitialAutoComplete("/Master/GetSubcontractorNameEN");
    $("#Result_Detail input[id=SubContractorNameLC]").InitialAutoComplete("/Master/GetSubcontractorNameLC");
    $("#Result_Detail input[id=AddressEN]").InitialAutoComplete("/Master/GetSubcontractorAddressEN");
    $("#Result_Detail input[id=AddressLC]").InitialAutoComplete("/Master/GetSubcontractorAddressLC");

    mygrid = $("#grid_result").InitialGrid(ROWS_PER_PAGE_FOR_VIEWPAGE, true, "/Master/MAS110_InitGrid");

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

function searchSubcontractor() {

    //########### Modify by Siripoj S. 13-06-2012
    // disable search button
    $("#btnSearch").attr("disabled", true);
    master_event.LockWindow(true);

    $("#Result_Detail").hide();
    if ($("#grid_result").length > 0) {
        var param = CreateObjectData($("#MAS110_SearchCriteria").serialize());
        $("#grid_result").LoadDataToGrid(mygrid, ROWS_PER_PAGE_FOR_VIEWPAGE
            , false
            , "/Master/MAS110_Search"
            , param
            , "doSubcontractor"
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
                    //setTimeout("document.getElementById('Result_List').scrollIntoView()", 1500);
                }
            }
        );
    }
}

function newSubcontractor() {
    setDisableSearchSection(true);
    setDisableGridButton(true);
    $("#MAS110_ResultDetail").clearForm();
    $("#DeleteFlag").attr("disabled", true);
    $("#Result_Detail").show();
    if (permission.ADD) { setDisableControlForDeleteFlag(false); }
    SetConfirmCommand(true, confirmCommand);
    SetCancelCommand(true, cancelCommand);
}

function searchSubcontractorDetail(SubContractorCode) {
    var param = { SubContractorCode: SubContractorCode };
    call_ajax_method_json('/Master/MAS110_SearchDetail'
        , param
        , function (result, controls) {
            if (result != undefined) {
                $("#MAS110_ResultDetail").clearForm();
                $("#MAS110_ResultDetail").bindJSON(result);
                $("#SubInstallationFlag").attr("checked", result.SubInstallationFlag);
                $("#SubMaintenanceFlag").attr("checked", result.SubMaintenanceFlag);
                $("#UpdateBy").val(result.UpdateBy);
                setDisableControlForSubInstallationFlag(!$("#SubInstallationFlag").prop("checked"));
                setDisableControlForSubMaintenanceFlag(!$("#SubMaintenanceFlag").prop("checked"));
                setDisableControlForDeleteFlag(!permission.EDIT);
                $("#DeleteFlag").attr("disabled", !permission.DEL);
                $("#Result_Detail").show();
                if (permission.EDIT || permission.DEL) { SetConfirmCommand(true, confirmCommand); }
                SetCancelCommand(true, cancelCommand);
            }
        }
    );
}

function confirmCommand() {

    DisableConfirmCommand(true);
    if (modeOfComfirmCommand == "edit" && $("#DeleteFlag").prop("checked")) { modeOfComfirmCommand = "delete"; }
    switch (modeOfComfirmCommand) {
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

}

function cancelCommand() {
    var param = { "module": "Common", "code": "MSG0028", param: msgCancel };
    call_ajax_method_json("/Shared/GetMessage"
        , param
        , function (data, ctrl) {
            OpenYesNoMessageDialog(data.Code, data.Message
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
        }
    );
}

function confirmNew() {
    command_control.CommandControlMode(false);
    var param = CreateObjectData($("#MAS110_ResultDetail").serialize(), true);
    //param = pa
    call_ajax_method_json('/Master/MAS110_Insert'
        , param
        , function (result, controls) {
            command_control.CommandControlMode(true);
            DisableConfirmCommand(false);
            if (controls != undefined) {
                VaridateCtrl(["SubContractorCode"
                             , "COCompanyCode"
                             , "InstallationTeam"
                             , "SubContractorNameEN"
                             , "SubContractorNameLC"
                             , "RepresentSubContractorName"
                             , "AddressEN"
                             , "AddressLC"], controls);
                SetConfirmCommand(true, confirmCommand);
            }
            else if (result == "ConfirmUpdate") {
                // Get Message
                var obj = {
                    module: "Master",
                    code: "MSG1057"
                };

                call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
                    OpenYesNoMessageDialog(result.Code, result.Message, confirmUpdateCaseDuplicate, function () {

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
                                mygrid.setSizes();
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

function confirmEdit() {
    command_control.CommandControlMode(false);
    setDisableControlForSubInstallationFlag(false)
    setDisableControlForSubMaintenanceFlag(false)
    var param = CreateObjectData($("#MAS110_ResultDetail").serialize(), true);
    call_ajax_method_json('/Master/MAS110_Update'
        , param
        , function (result, controls) {
            command_control.CommandControlMode(true);
            DisableConfirmCommand(false);
            if (controls != undefined) {
                VaridateCtrl(["SubContractorCode"
                             , "COCompanyCode"
                             , "InstallationTeam"
                             , "SubContractorNameEN"
                             , "SubContractorNameLC"
                             , "RepresentSubContractorName"
                             , "AddressEN"
                             , "AddressLC"], controls);
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

function confirmUpdateCaseDuplicate() {

    var param = CreateObjectData($("#MAS110_ResultDetail").serialize(), true);
    call_ajax_method_json('/Master/MAS110_UpdateCaseDuplicate'
    , param
    , function (result, controls) {
        DisableConfirmCommand(false);
        if (controls != undefined) {
            VaridateCtrl(["SubContractorCode"
                             , "COCompanyCode"
                             , "InstallationTeam"
                             , "SubContractorNameEN"
                             , "SubContractorNameLC"
                             , "RepresentSubContractorName"
                             , "AddressEN"
                             , "AddressLC"], controls);
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
                                mygrid.setSizes();
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

function confirmDelete() {
    command_control.CommandControlMode(false);
    var param = { "module": "Master", "code": "MSG1030", param: $("#SubContractorCode").val() };
    call_ajax_method_json("/Shared/GetMessage"
        , param
        , function (data, ctrl) {
            command_control.CommandControlMode(true);
            OpenYesNoMessageDialog(data.Code, data.Message
                , function () {
                    DisableConfirmCommand(false);
                    setDisableControlForSubInstallationFlag(false)
                    setDisableControlForSubMaintenanceFlag(false)
                    setDisableControlForDeleteFlag(false);
                    var param = CreateObjectData($("#MAS110_ResultDetail").serialize(), true);
                    call_ajax_method_json('/Master/MAS110_Update'
                        , param
                        , function (result, controls) {
                            if (result != undefined) {
                                var param = { "module": "Master", "code": "MSG1032", param: $("#SubContractorCode").val() };
                                DeleteRow(mygrid, selectedRowId);
                                setAllControlSaveSuccess();
                                call_ajax_method_json("/Shared/GetMessage"
                                    , param
                                    , function (data) { OpenInformationMessageDialog(data.Code, data.Message, function () { }); }
                                );
                            }
                            else {
                                var param = { "module": "Master", "code": "MSG1031", param: $("#SubContractorCode").val() };
                                call_ajax_method_json("/Shared/GetMessage"
                                    , param
                                    , function (data) { OpenInformationMessageDialog(data.Code, data.Message, function () { SetConfirmCommand(true, confirmCommand); }); }
                                );
                            }
                            if (permission.EDIT) {
                                setDisableControlForDeleteFlag($("#DeleteFlag").prop("checked"));
                            }
                        }
                    );
                }
                , function () { SetConfirmCommand(true, confirmCommand); }
            );
        }
    );
}

function btnGridEditClick(rid) {
    mygrid.selectRow(mygrid.getRowIndex(rid));
    selectedRowId = rid;
    isSelected = true;
    setDisableSearchSection(true);
    setDisableGridButton(true);
    var subcontractorCode = mygrid.cells(rid, mygrid.getColIndexById('SubContractorCode')).getValue();
    searchSubcontractorDetail(subcontractorCode);
    modeOfComfirmCommand = "edit";

}

function addDetailGrid(result) {
    if (result != undefined) {
        CheckFirstRowIsEmpty(mygrid, true);
        var SubContractorName = "(1) " + $("#SubContractorNameEN").val() + "<br/>(2) " + $("#SubContractorNameLC").val();
        AddNewRow(mygrid,
            ["",
            result.SubContractorCode,
            $("#COCompanyCode").val(),
            $("#InstallationTeam").val(),
            SubContractorName,
            $("#SubInstallationFlag").prop("checked") ? MAS110_FlagDisplay.Yes : MAS110_FlagDisplay.No,
            $("#SubMaintenanceFlag").prop("checked") ? MAS110_FlagDisplay.Yes : MAS110_FlagDisplay.No,
            ""]);
        var row_id = mygrid.getRowId(mygrid.getRowsNum() - 1);
        GenerateEditButton(mygrid, "btnGridEdit", row_id, "Edit", true);
        BindGridButtonClickEvent("btnGridEdit", row_id, function (rid) { btnGridEditClick(rid); });
    }
}

function editDetailGrid() {
    var SubContractorName = "(1) " + $("#SubContractorNameEN").val() + "<br/>(2) " + $("#SubContractorNameLC").val();
    mygrid.cells2(mygrid.getRowIndex(selectedRowId), mygrid.getColIndexById('SubContractorCode')).setValue($("#SubContractorCode").val());
    mygrid.cells2(mygrid.getRowIndex(selectedRowId), mygrid.getColIndexById('COCompanyCode')).setValue($("#COCompanyCode").val());
    mygrid.cells2(mygrid.getRowIndex(selectedRowId), mygrid.getColIndexById('InstallationTeam')).setValue($("#InstallationTeam").val());
    mygrid.cells2(mygrid.getRowIndex(selectedRowId), mygrid.getColIndexById('SubContractorName')).setValue(SubContractorName);
    mygrid.cells2(mygrid.getRowIndex(selectedRowId), mygrid.getColIndexById('SubInstallationFlagDisplay')).setValue($("#SubInstallationFlag").prop("checked") ? MAS110_FlagDisplay.Yes : MAS110_FlagDisplay.No);
    mygrid.cells2(mygrid.getRowIndex(selectedRowId), mygrid.getColIndexById('SubMaintenanceFlagDisplay')).setValue($("#SubMaintenanceFlag").prop("checked") ? MAS110_FlagDisplay.Yes : MAS110_FlagDisplay.No);
}

function setAllControlSaveSuccess() {
    isSelected = false;
    $("#Result_List").show();
    $("#Result_Detail").hide();
    $("#MAS110_ResultDetail").clearForm();
    SetConfirmCommand(false, confirmCommand);
    SetCancelCommand(false, cancelCommand);
    setDisableGridButton(false);
    setDisableSearchSection(false);
    if (modeOfComfirmCommand = "delete") {
        setDisableControlForDeleteFlag(false);
        $("#DeleteFlag").attr("disabled", false);
    }
}

function setDisableControlForDeleteFlag(isDisable) {
    $("#MAS110_ResultDetail input").attr("readonly", isDisable);
    $("#MAS110_ResultDetail select").attr("disabled", isDisable);
    $("#AddressEN").attr("readonly", isDisable);
    $("#AddressLC").attr("readonly", isDisable);
    $("#Memo").attr("readonly", isDisable);
    $("#SubInstallationFlag").attr("disabled", isDisable);
    $("#SubMaintenanceFlag").attr("disabled", isDisable);
    $("#SubContractorCode").attr("readonly", true);
    if (!isDisable) {
        setDisableControlForSubInstallationFlag(!$("#SubInstallationFlag").prop("checked"));
        setDisableControlForSubMaintenanceFlag(!$("#SubMaintenanceFlag").prop("checked"));
    }
    if (modeOfComfirmCommand == "edit") {
        $("#COCompanyCode").attr("readonly", true);
        $("#InstallationTeam").attr("readonly", true);
    }
}

function setDisableControlForSubInstallationFlag(isDisable) {
    $("#InstallationSubcontractorLevel").attr("disabled", isDisable);
    $("#InstallationAccessLevel").attr("disabled", isDisable);
    $("#InstallationAlarmLevel").attr("disabled", isDisable);
    $("#InstallationOtherLevel").attr("disabled", isDisable);
    $("#InstallationCCTVLevel").attr("disabled", isDisable);
    $("#InstallationImageMonitorLevel").attr("disabled", isDisable);
    if (isDisable) {
        $("#InstallationSubcontractorLevel").val("");
        $("#InstallationAccessLevel").val("");
        $("#InstallationAlarmLevel").val("");
        $("#InstallationOtherLevel").val("");
        $("#InstallationCCTVLevel").val("");
        $("#InstallationImageMonitorLevel").val("");
    }
}

function setDisableControlForSubMaintenanceFlag(isDisable) {
    $("#MaintenanceSubcontractorLevel").attr("disabled", isDisable);
    $("#MaintenanceAccessLevel").attr("disabled", isDisable);
    $("#MaintenanceAlarmLevel").attr("disabled", isDisable);
    $("#MaintenanceOtherLevel").attr("disabled", isDisable);
    $("#MaintenanceCCTVLevel").attr("disabled", isDisable);
    $("#MaintenanceImageMonitorLevel").attr("disabled", isDisable);
    if (isDisable) {
        $("#MaintenanceSubcontractorLevel").val("");
        $("#MaintenanceAccessLevel").val("");
        $("#MaintenanceAlarmLevel").val("");
        $("#MaintenanceOtherLevel").val("");
        $("#MaintenanceCCTVLevel").val("");
        $("#MaintenanceImageMonitorLevel").val("");
    }
}

function setDisableSearchSection(isDisable) {
    $("#MAS110_SearchCriteria input").attr("readonly", isDisable);
    $("#MAS110_SearchCriteria button").attr("disabled", isDisable);
    $("#MAS110_SearchCriteria select").attr("disabled", isDisable);
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