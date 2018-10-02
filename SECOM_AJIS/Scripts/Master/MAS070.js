/// <reference path = "../../Scripts/jquery-1.5.1-vsdoc.js" />
/// <reference path = "../../Scripts/Base/GridControl.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxcommon.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxgrid.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxgridcell.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/ext/dhtmlxgrid_pgn.js" />

var newMode = false;
var detailMode = false;
var objMAS070;
var objMAS070_bel;
var mygrid;
var mygrid2;
var addBelMode = false;
var editBelMode = false;
var belRowID;
var delBelRowID;

var delBelongingList = [];

var btn_employee_detail = "btnEmployeeDetail";
var btn_belonging_detail = "btnBelongingDetail";
var btn_belonging_remove = "btnBelongingRemove";

var selectedEmp;
var selectedBel;

$(document).ready(function () {
    $("#EmailAddress").attr("maxlength", c_email_length);
    $("#Search_Result").hide();
    $("#Employee_Detail").hide();
    $("#Belonging_Detail").hide();
    InitialDateFromToControl("#MAS070_EmployeeDetail #StartDate", "#MAS070_EmployeeDetail #EndDate");
    InitialDateFromToControl("#Belonging_Detail #ValidFrom", "#Belonging_Detail #ValidTo");
    $("#txtEmployeeNameSearch").InitialAutoComplete("/Master/MAS070_GetEmployeeName");

    $("#btnSearch").click(employeeSearch);
    $("#btnClear").click(clearSearchEmployee);
    $("#btnNew").click(newEmployee);
    $("#DeleteFlag").click(toggleEmployeeDetail);
    $("#btnAddUpd").click(addUpdateBelonging);
    $("#btnCancelBel").click(cancelBelonging);
    $("#btnRegisBelonging").click(function () {
        registerNewBelonging();
        addBelMode = true;
    });

    if (permission.ADD == "False") {
        $("#btnNew").attr("disabled", true);
    }

    $('#ChangePasswordFlag').change(toggleChangePasswordFlag);
    innitialAllGrid();
});

function innitialAllGrid() {
    //mygrid = $("#grid_result").InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/Master/MAS070_InitGrid");
    mygrid = $("#grid_result").InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/Master/MAS070_InitGrid");

    SpecialGridControl(mygrid, ["Edit"]);

    mygrid.attachEvent("onBeforeSelect", function (new_row, old_row) {
        return !(detailMode || newMode);
    });
    mygrid.attachEvent("onBeforeSorting", function (ind, type, direction) {
        return !(detailMode || newMode);
    });
    mygrid.attachEvent("onBeforePageChanged", function (ind, count) {
        return !(detailMode || newMode);
    });

    BindOnLoadedEvent(mygrid, function (gen_ctrl) {
        if (mygrid.getRowsNum() != 0) {
            for (var i = 0; i < mygrid.getRowsNum(); i++) {
                var row_id = mygrid.getRowId(i);

                if (gen_ctrl == true) {
                    if (newMode || detailMode) {
                        GenerateEditButton(mygrid, btn_employee_detail, row_id, "Edit", false);
                    } else {
                        GenerateEditButton(mygrid, btn_employee_detail, row_id, "Edit", true);
                    }
                    //GenerateEditButton(mygrid, btn_employee_detail, row_id, "Detail", true);
                }

                BindGridButtonClickEvent(btn_employee_detail, row_id, function (rid) {
                    mygrid.selectRow(mygrid.getRowIndex(rid));
                    if (!newMode && !detailMode) {
                        selectedEmp = rid;
                        var EmpNo = mygrid.cells(rid, mygrid.getColIndexById('EmpNo')).getValue();
                        employeeSearchDetail(EmpNo);
                        detailMode = true;
                    }
                });
            }
            mygrid.setSizes();
        }
    });

    mygrid2 = $("#belGrid_result").InitialGrid(ROWS_PER_PAGE_FOR_VIEWPAGE, true, "/Master/MAS070_InitBelGrid");

    SpecialGridControl(mygrid2, ["Edit", "Remove"]);

    mygrid2.attachEvent("onBeforeSelect", function (new_row, old_row) {
        return !(addBelMode || editBelMode);
    });
    mygrid2.attachEvent("onBeforeSorting", function (ind, type, direction) {
        return !(addBelMode || editBelMode);
    });
    mygrid2.attachEvent("onBeforePageChanged", function (ind, count) {
        return !(addBelMode || editBelMode);
    });

    BindOnLoadedEvent(mygrid2, function (gen_ctrl) {
        if (mygrid2.getRowsNum() != 0) {
            for (var i = 0; i < mygrid2.getRowsNum(); i++) {
                var row_id = mygrid2.getRowId(i);

                if (gen_ctrl == true) {
                    //if (permission.EDIT == "False" && !newMode) {
                    if ((permission.EDIT == "False" && !newMode) || addBelMode || editBelMode) {
                        GenerateEditButton(mygrid2, btn_belonging_detail, row_id, "Edit", false);
                        GenerateRemoveButton(mygrid2, btn_belonging_remove, row_id, "Remove", false);
                    } else {
                        GenerateEditButton(mygrid2, btn_belonging_detail, row_id, "Edit", true);
                        GenerateRemoveButton(mygrid2, btn_belonging_remove, row_id, "Remove", true);
                    }

                    var modMode = mygrid2.cells2(i, mygrid2.getColIndexById('ModifyMode')).getValue();
                    if (modMode == null || modMode == "" || modMode == undefined) {
                        mygrid2.cells2(i, mygrid2.getColIndexById('ModifyMode')).setValue("NONE");
                    }

                    if (mygrid2.cells2(i, mygrid2.getColIndexById('ApprovalAuthorityPersonFlag')).getValue() == 1
                    || mygrid2.cells2(i, mygrid2.getColIndexById('ApprovalAuthorityPersonFlag')).getValue() == c_yes) {
                        mygrid2.cells2(i, mygrid2.getColIndexById('ApprovalAuthorityPersonFlag')).setValue(c_yes);
                    } else {
                        mygrid2.cells2(i, mygrid2.getColIndexById('ApprovalAuthorityPersonFlag')).setValue(c_no);
                    }

                    if (mygrid2.cells2(i, mygrid2.getColIndexById('DepPersonInchargeFlag')).getValue() == 1
                    || mygrid2.cells2(i, mygrid2.getColIndexById('DepPersonInchargeFlag')).getValue() == c_yes) {
                        mygrid2.cells2(i, mygrid2.getColIndexById('DepPersonInchargeFlag')).setValue(c_yes);
                    } else {
                        mygrid2.cells2(i, mygrid2.getColIndexById('DepPersonInchargeFlag')).setValue(c_no);
                    }
                }

                BindGridButtonClickEvent(btn_belonging_detail, row_id, function (rid) {
                    mygrid2.selectRow(mygrid2.getRowIndex(rid));
                    if (!addBelMode && !editBelMode) {
                        selectedBel = rid;
                        eventBelongingDetail(rid);
                    }
                });
                BindGridButtonClickEvent(btn_belonging_remove, row_id, function (rid) {
                    if (!addBelMode && !editBelMode) {
                        eventBelongingDelete(rid);
                    }
                });
            }
            mygrid2.setSizes();
        }
    });
}

function eXcell_yesNoCell(cell) {
    this.base = eXcell_edn;
    this.base(cell);
    this.setValue = function (val) {
        if (!val || val.toString()._dhx_trim() == "") {
            val = "0";
        }

        if (val > 0) {
            this.cell.innerHTML = "√";
        } else {
            this.cell.innerHTML = "-";
        }
    };
}
eXcell_yesNoCell.prototype = new eXcell_edn;

function setEnableToEmployeeGrid(enable) {
    if ($("#grid_result").length > 0) {
        if (mygrid.getRowsNum() != 0) {
            for (var i = 0; i < mygrid.getRowsNum(); i++) {
                var row_id = mygrid.getRowId(i);
                EnableGridButton(mygrid, btn_employee_detail, row_id, "Edit", enable);
            }
        }
    }

    //    var removeCol = mygrid.getColIndexById("Detail");
    //    mygrid.setColumnHidden(removeCol, !enable);
    //    mygrid.setSizes();

    // Akat K. : this is a really bad solution if there are better solution please tell me.
    //    $("#grid_result").hide();
    //    $("#grid_result").show();
}

function setEnableToBelongingGrid(enable) {
    if ($("#belGrid_result").length > 0) {
        if (mygrid2.getRowsNum() != 0) {
            for (var i = 0; i < mygrid2.getRowsNum(); i++) {
                var row_id = mygrid2.getRowId(i);
                EnableGridButton(mygrid2, btn_belonging_detail, row_id, "Edit", enable);
                EnableGridButton(mygrid2, btn_belonging_remove, row_id, "Remove", enable);
            }
        }
    }

    //    var removeCol2 = mygrid2.getColIndexById("Detail");
    //    mygrid2.setColumnHidden(removeCol2, !enable);
    //    removeCol2 = mygrid2.getColIndexById("Remove");
    //    mygrid2.setColumnHidden(removeCol2, !enable);
    //    mygrid2.setSizes();

    // Akat K. : this is a really bad solution if there are better solution please tell me.
    //    $("#belGrid_result").hide();
    //    $("#belGrid_result").show();
}

function eventBelongingDetail(rid) {
    //mygrid2.cells2(id, mygrid2.getColIndexById('OfficeCode')).getValue()
    $("#MAS070_BelongingDetail #EmpNo").val($("#MAS070_EmployeeDetail #EmpNo").val());
    $("#OfficeCode").val(mygrid2.cells(rid, mygrid2.getColIndexById('OfficeCode')).getValue());
    $("#DepartmentCode").val(mygrid2.cells(rid, mygrid2.getColIndexById('DepartmentCode')).getValue());
    $("#PositionCode").val(mygrid2.cells(rid, mygrid2.getColIndexById('PositionCode')).getValue());
    $("#BelongingID").val(mygrid2.cells(rid, mygrid2.getColIndexById('BelongingID')).getValue());
    if (mygrid2.cells(rid, mygrid2.getColIndexById('DepPersonInchargeFlag')).getValue() == c_yes || 
        mygrid2.cells(rid, mygrid2.getColIndexById('DepPersonInchargeFlag')).getValue() == 1) {
        $("#DepPersonInchargeFlag").attr("checked", true);
    } else {
        $("#DepPersonInchargeFlag").attr("checked", false);
    }
    if (mygrid2.cells(rid, mygrid2.getColIndexById('ApprovalAuthorityPersonFlag')).getValue() == 1 ||
        mygrid2.cells(rid, mygrid2.getColIndexById('ApprovalAuthorityPersonFlag')).getValue() == c_yes) {
        $("#ApprovalAuthorityPersonFlag").attr("checked", true);
    } else {
        $("#ApprovalAuthorityPersonFlag").attr("checked", false);
    }
    //    if (mygrid2.cells(rid, mygrid2.getColIndexById('MainDepartmentFlag')).getValue() == "1") {
    if (mygrid2.cells(rid, mygrid2.getColIndexById('MainDepartmentFlag')).getValue() == "√") {
        $("#MainDepartmentFlag").attr("checked", true);
    } else {
        $("#MainDepartmentFlag").attr("checked", false);
    }

    //    $("#MAS070_BelongingDetail #ValidFrom").val(mygrid2.cells(rid, mygrid2.getColIndexById('ValidFrom')).getValue());
    //    $("#MAS070_BelongingDetail #ValidTo").val(mygrid2.cells(rid, mygrid2.getColIndexById('ValidTo')).getValue());
    SetDateFromToData("#MAS070_BelongingDetail #ValidFrom", "#MAS070_BelongingDetail #ValidTo",
        mygrid2.cells(rid, mygrid2.getColIndexById('ValidFrom')).getValue(), mygrid2.cells(rid, mygrid2.getColIndexById('ValidTo')).getValue());
    $("#MAS070_BelongingDetail #ValidFrom").datepicker("getDate");
    $("#MAS070_BelongingDetail #ValidTo").datepicker("getDate");

    if (mygrid2.cells(rid, mygrid2.getColIndexById('ModifyMode')).getValue() != "ADD") {
        $("#OfficeCode").attr("disabled", true);
    }

    //    var tempDateStr = mygrid2.cells(rid, mygrid2.getColIndexById('ValidFrom')).getValue();
    //    tempDateStr = tempDateStr.replace("-", " ");
    //    var tempDateFrom = new Date(tempDateStr);
    //    $("#MAS070_BelongingDetail #ValidFrom").val(tempDateStr);
    //    tempDateStr = mygrid2.cells(rid, mygrid2.getColIndexById('ValidTo')).getValue();
    //    tempDateStr = tempDateStr.replace("-", " ");
    //    var tempDateTo = new Date(tempDateStr);
    //    $("#MAS070_BelongingDetail #ValidTo").val(tempDateStr);
    //    SetDateFromToData("#MAS070_BelongingDetail #ValidFrom", "#MAS070_BelongingDetail #ValidTo", tempDateFrom, tempDateTo);

    $("#MAS070_EmployeeDetail input").attr("readonly", true);
    $("#MAS070_EmployeeDetail button").attr("disabled", true);
    $("#StartDate").EnableDatePicker(false);
    $("#EndDate").EnableDatePicker(false);

    $("#IncidentNotificationFlag").attr("disabled", true);
    $("#btnRegisBelonging").attr("disabled", true);
    $("#DeleteFlag").attr("disabled", true);
    $("#Belonging_Detail").show();
    SetCancelCommand(false, cancelEmployeeData);
    SetConfirmCommand(false, saveEmployeeInformation);

    $("#belGrid_result input").attr("readonly", true);
    //$("#belGrid_result button").attr("disabled", true);
    if (!CheckFirstRowIsEmpty(mygrid2)) {
        setEnableToBelongingGrid(false);
    }

    editBelMode = true;
    belRowID = rid;
}

function eventBelongingDelete(rid) {
    delBelRowID = rid;
    var msgprm = { "module": "Common", "code": "MSG0141" };  //param: c_remove };
    call_ajax_method("/Shared/GetMessage", msgprm, function (data, ctrl) {
        OpenYesNoMessageDialog(data.Code, data.Message, function () {
            if (mygrid2.cells(delBelRowID, mygrid2.getColIndexById('ModifyMode')).getValue() == "ADD") {
                DeleteRow(mygrid2, delBelRowID);
            } else {
                delBelongingList.push({
                    OfficeCode: mygrid2.cells(delBelRowID, mygrid2.getColIndexById('OfficeCode')).getValue(),
                    DepartmentCode: mygrid2.cells(delBelRowID, mygrid2.getColIndexById('DepartmentCode')).getValue(),
                    DepPersonInchargeFlag: mygrid2.cells(delBelRowID, mygrid2.getColIndexById('DepPersonInchargeFlag')).getValue() == "Yes" ? true : false,
                    BelongingID: mygrid2.cells(delBelRowID, mygrid2.getColIndexById('BelongingID')).getValue()
                });
                DeleteRow(mygrid2, delBelRowID);
            }
        });
    });
}

function cancelEmployeeData() {
    var msgprm = { "module": "Common", "code": "MSG0140" }; //, param: c_cancel };
    call_ajax_method("/Shared/GetMessage", msgprm, function (data, controls) {
        OpenYesNoMessageDialog(data.Code, data.Message, function () {
            //$("#Search_Result").hide();
            $("#Employee_Detail").hide();
            $("#Belonging_Detail").hide();
            SetCancelCommand(false, cancelEmployeeData);
            SetConfirmCommand(false, saveEmployeeInformation);

            $("#MAS070_Search input").attr("readonly", false);
            $("#MAS070_Search button").attr("disabled", false);
            //$("#Search_Result button").attr("disabled", false);
            setEnableToEmployeeGrid(true);
            if (permission.ADD == "False") {
                $("#btnNew").attr("disabled", true);
            }

            VaridateCtrl(["OfficeCode",
                    "DepartmentCode",
                    "PositionCode"], []);

            delBelongingList = [];

            addBelMode = false;
            editBelMode = false;

            detailMode = false;
            newMode = false;
        });
    });
}

function clearSearchEmployee() {
    //4.1	Clear textbox in “Search user” section
    $("#txtEmployeeCodeSearch").val("");
    $("#txtEmployeeNameSearch").val("");

    //4.2	Clear data in “Result list” section
    if (mygrid != null) {
        DeleteAllRow(mygrid);
    }

    //4.3	Show only "Search user" section
    $("#Search_Result").hide();
    $("#Employee_Detail").hide();
    SetCancelCommand(false, cancelEmployeeData);
    SetConfirmCommand(false, saveEmployeeInformation);
}

function newEmployee() {
    //5.1	Disable “Search user” and “Result list” section
    $("#MAS070_Search input").attr("readonly", true);
    $("#MAS070_Search button").attr("disabled", true);
    //$("#Search_Result button").attr("disabled", true);
    setEnableToEmployeeGrid(false);

    //5.2	Clear textbox in “Maintain user” section
    clearEmployeeDetail();
    $("#DeleteFlag").attr("disabled", true);

    //5.3	Show “Maintain user” section
    $("#Employee_Detail").show();
    $("#MAS070_EmployeeDetail #EmpNo").focus();
    SetCancelCommand(true, cancelEmployeeData);
    SetConfirmCommand(true, saveEmployeeInformation);

    $("#MAS070_EmployeeDetail #EmpNo").attr("readonly", false);

    //loadBelongingGridData("", false);
    if (mygrid2 != null) {
        //mygrid2.clearAll();
        DeleteAllRow(mygrid2)
    }

    $('#change-password-flag-area').hide();
    $('#password-area').show();
    $('#currentPassword').val('');
    $('#Password').val('');
    newMode = true;
}

function clearEmployeeDetail() {
    $("#MAS070_EmployeeDetail").clearForm();
    ClearDateFromToControl("#MAS070_EmployeeDetail #StartDate", "#MAS070_EmployeeDetail #EndDate");
    $("#MAS070_BelongingDetail").clearForm();
    ClearDateFromToControl("#Belonging_Detail #ValidFrom", "#Belonging_Detail #ValidTo");

    $("#DeleteFlag").attr("checked", false);
    $("#MainDepartmentFlag").attr("checked", false);
    $("#ApprovalAuthorityPersonFlag").attr("checked", false);
    $("#IncidentNotificationFlag").attr("checked", false);
    $("#DepPersonInchargeFlag").attr("checked", false);
    toggleEmployeeDetail();
}

function toggleEmployeeDetail() {
    if ($("#DeleteFlag").prop("checked")) {
        disableEmployeeDetail();
    } else {
        enableEmployeeDetail();
    }
}

function disableEmployeeDetail() {
    $("#MAS070_EmployeeDetail input").attr("readonly", true);
    $("#MAS070_EmployeeDetail button").attr("disabled", true);
    $("#MAS070_EmployeeDetail #StartDate").EnableDatePicker(false);
    $("#MAS070_EmployeeDetail #EndDate").EnableDatePicker(false);
    $("#IncidentNotificationFlag").attr("disabled", true);


    $("#belGrid_result input").attr("readonly", true);
    //$("#belGrid_result button").attr("disabled", true);
    if (!CheckFirstRowIsEmpty(mygrid2)) {
        setEnableToBelongingGrid(false);
    }
    $("#MAS070_BelongingDetail input").attr("readonly", true);
    $("#MAS070_BelongingDetail button").attr("disabled", true);
    $("#btnRegisBelonging").attr("disabled", true);

    if (permission.DELETE == "True") {
        $("#DeleteFlag").attr("disabled", false);
    } else {
        $("#DeleteFlag").attr("disabled", true);
    }
}

function enableEmployeeDetail() {
    $("#MAS070_EmployeeDetail input").attr("readonly", false);
    $("#MAS070_EmployeeDetail button").attr("disabled", false);
    $("#MAS070_EmployeeDetail #StartDate").EnableDatePicker(true);
    $("#MAS070_EmployeeDetail #EndDate").EnableDatePicker(true);
    $("#IncidentNotificationFlag").attr("disabled", false);

    if (detailMode) {
        $("#EmpNo").attr("readonly", true);
    }

    $("#belGrid_result input").attr("readonly", false);
    //$("#belGrid_result button").attr("disabled", false);
    if (!CheckFirstRowIsEmpty(mygrid2)) {
        setEnableToBelongingGrid(true);
    }
    $("#MAS070_BelongingDetail input").attr("readonly", false);
    $("#MAS070_BelongingDetail button").attr("disabled", false);
    $("#btnRegisBelonging").attr("disabled", false);
}

// BELONGING ########################################################################################################################################
function registerNewBelonging() {
    if (!addBelMode && !editBelMode) {
        //11.1	Disable control in “Maintain user” section
        $("#MAS070_EmployeeDetail input").attr("readonly", true);
        $("#MAS070_EmployeeDetail button").attr("disabled", true);
        $("#IncidentNotificationFlag").attr("disabled", true);
        $("#btnRegisBelonging").attr("disabled", true);
        $("#DeleteFlag").attr("disabled", true);

        $("#MAS070_EmployeeDetail #StartDate").EnableDatePicker(false);
        $("#MAS070_EmployeeDetail #EndDate").EnableDatePicker(false);

        $("#belGrid_result input").attr("readonly", true);
        //$("#belGrid_result button").attr("disabled", true);
        if (!CheckFirstRowIsEmpty(mygrid2)) {
            setEnableToBelongingGrid(false);
        }

        //11.2	Clear data in all input control in “Belonging information” section
        $("#MAS070_BelongingDetail").clearForm();
        $("#BelongingID").val("");
        selectedBel = undefined;
        ClearDateFromToControl("#Belonging_Detail #ValidFrom", "#Belonging_Detail #ValidTo");
        $("#MainDepartmentFlag").attr("checked", false);
        $("#ApprovalAuthorityPersonFlag").attr("checked", false);

        //11.3	Set dropdownlist index=0
        // default is select
        //$("#PositionCode").val(default_position);
        $("#PositionCode").val("");
        $("#OfficeCode").attr("disabled", false);

        //11.4	Show “Belonging information” section
        $("#Belonging_Detail").show();
        SetCancelCommand(false, cancelEmployeeData);
        SetConfirmCommand(false, saveEmployeeInformation);

        $("#MAS070_BelongingDetail #EmpNo").val($("#MAS070_EmployeeDetail #EmpNo").val());
        addBelMode = true;
    }
}

function addUpdateBelonging() {
    var parameter = {
        EmpNo: $("#EmpNo").val(),
        BelongingID: $("#BelongingID").val(),
        MainDepartmentFlag: $("#MainDepartmentFlag").prop("checked"),
        OfficeCode: $("#OfficeCode").val(),
        DepartmentCode: $("#DepartmentCode").val(),
        PositionCode: $("#PositionCode").val(),
        DepPersonInchargeFlag: $("#DepPersonInchargeFlag").prop("checked"),
        ApprovalAuthorityPersonFlag: $("#ApprovalAuthorityPersonFlag").prop("checked"),
        StartDate: $("#ValidFrom").val(),
        EndDate: $("#ValidTo").val(),
        delBelList: delBelongingList
    };
    ajax_method.CallScreenController(
        '/Master/MAS070_ValidateBelonging',
        parameter,
        function (result, controls) {
            if (controls != undefined) {
                VaridateCtrl(["OfficeCode",
                                "DepartmentCode",
                                "PositionCode"], controls);
            }

            if (result != undefined) {
                checkDuplicateAndMainDepartment();
            }
        }
    );

}

function checkDuplicateAndMainDepartment() {
    var err_mainDepart = false;
    //var err_depPerson = false;
    var err_dupBel = false;

    var addMainDepart = $("#MainDepartmentFlag").prop("checked") ? "1" : "0";
    var addDepPersonInCharge = $("#DepPersonInchargeFlag").prop("checked") ? 1 : 0;
    var addOffice = $("#OfficeCode").val();
    var addDepartment = $("#DepartmentCode").val();
    var addPosition = $("#PositionCode").val();

    var addBelongingID = $("#BelongingID").val();

    for (var i = 0; i < mygrid2.getRowsNum(); i++) {
        var checkBel = mygrid2.getRowId(i);
        if (checkBel == selectedBel) {
            continue;
        }

        var belongingID = mygrid2.cells2(i, mygrid2.getColIndexById('BelongingID')).getValue();
        var mainDepart = mygrid2.cells2(i, mygrid2.getColIndexById('MainDepartmentFlag')).getValue() == "√";

        if (addMainDepart == "1" && mainDepart) {
            err_mainDepart = true;
            break;
        }

        var DepPersonInCharge = mygrid2.cells2(i, mygrid2.getColIndexById('DepPersonInchargeFlag')).getValue() == c_yes;
        var officeCode = mygrid2.cells2(i, mygrid2.getColIndexById('OfficeCode')).getValue();
        var departmentCode = mygrid2.cells2(i, mygrid2.getColIndexById('DepartmentCode')).getValue();

        //        if (addDepPersonInCharge == 1) {
        //            if (addOffice == officeCode && addDepartment == departmentCode && DepPersonInCharge) {
        //                err_depPerson = true;
        //                break;
        //            }
        //        }

        var positionCode = mygrid2.cells2(i, mygrid2.getColIndexById('PositionCode')).getValue();
        if (addOffice == officeCode && addDepartment == departmentCode && addPosition == positionCode) {
            err_dupBel = true;
            break;
        }
    }

    if (err_mainDepart) {
        var param = { "module": "Master", "code": "MSG1014" };
        call_ajax_method("/Shared/GetMessage", param, function (data, controls) {
            OpenWarningDialog(data.Message);
            //OpenErrorMessageDialog(data.code, data.Message);
        });
        return;
    }

    //    if (err_depPerson) {
    //        var param = { "module": "Master", "code": "MSG1038" };
    //        call_ajax_method("/Shared/GetMessage", param, function (data, controls) {
    //            OpenErrorMessageDialog(data.code, data.Message);
    //        });
    //        return;
    //    }

    if (err_dupBel) {
        var param = { "module": "Master", "code": "MSG1011" };
        call_ajax_method("/Shared/GetMessage", param, function (data, controls) {
            OpenWarningDialog(data.Message);
            //OpenErrorMessageDialog(data.code, data.Message);
        });
        return;
    }

    if (addBelMode) {
        addBelonging();
    }
    if (editBelMode) {
        editBelonging();
    }
}

function addBelonging() {
    addBelMode = false;
    addBelongingToGrid(mygrid2.getRowsNum(), "ADD");
    $("#Belonging_Detail").hide();
    SetCancelCommand(true, cancelEmployeeData);
    SetConfirmCommand(true, saveEmployeeInformation);
    after_addEditBelonging();
}

function editBelonging() {
    editBelMode = false;
    var rinx = mygrid2.getRowIndex(belRowID);

//    if (mygrid2.cells(belRowID, mygrid2.getColIndexById('ModifyMode')).getValue() == "NONE") {
//        var ModifyMode = "EDIT";
//    } else {
//        var ModifyMode = mygrid2.cells(belRowID, mygrid2.getColIndexById('ModifyMode')).getValue();
//    }
    var ModifyMode = "EDIT";
    editBelongingGrid(rinx, ModifyMode);

    $("#Belonging_Detail").hide();
    SetCancelCommand(true, cancelEmployeeData);
    SetConfirmCommand(true, saveEmployeeInformation);
    after_addEditBelonging();
}

function after_addEditBelonging() {
    $("#belGrid_result input").attr("readonly", false);
    //$("#belGrid_result button").attr("disabled", false);
    if (!CheckFirstRowIsEmpty(mygrid2)) {
        setEnableToBelongingGrid(true);
    }
    $("#MAS070_EmployeeDetail input").attr("readonly", false);
    $("#MAS070_EmployeeDetail button").attr("disabled", false);
    $("#IncidentNotificationFlag").attr("disabled", false);
    $("#btnRegisBelonging").attr("disabled", false);
    if (permission.DELETE == "True" && !newMode) {
        $("#DeleteFlag").attr("disabled", false);
    } else {
        $("#DeleteFlag").attr("disabled", true);
    }
    $("#MAS070_EmployeeDetail #StartDate").EnableDatePicker(true);
    $("#MAS070_EmployeeDetail #EndDate").EnableDatePicker(true);
}

function cancelBelonging() {
    //13.1	Enable control in “Maintain user” section
    $("#MAS070_EmployeeDetail input").attr("readonly", false);
    $("#MAS070_EmployeeDetail button").attr("disabled", false);
    $("#IncidentNotificationFlag").attr("disabled", false);
    $("#btnRegisBelonging").attr("disabled", false);
    if (permission.DELETE == "True") {
        $("#DeleteFlag").attr("disabled", false);
    } else {
        $("#DeleteFlag").attr("disabled", true);
    }
    $("#MAS070_EmployeeDetail #StartDate").EnableDatePicker(true);
    $("#MAS070_EmployeeDetail #EndDate").EnableDatePicker(true);

    //detailMode
    if (detailMode) {
        $("#EmpNo").attr("readonly", true);
    }

    $("#belGrid_result input").attr("readonly", false);
    //$("#belGrid_result button").attr("disabled", false);
    if (!CheckFirstRowIsEmpty(mygrid2)) {
        setEnableToBelongingGrid(true);
    }

    //13.2	Hide “Belonging information” section
    $("#Belonging_Detail").hide();
    SetCancelCommand(true, cancelEmployeeData);
    SetConfirmCommand(true, saveEmployeeInformation);

    VaridateCtrl(["OfficeCode",
                    "DepartmentCode",
                    "PositionCode"], []);

    addBelMode = false;
    editBelMode = false;
}

// MODIFY DATA IN GRID ##############################################################################################################################

function addNewEmpToGrid() {
    CheckFirstRowIsEmpty(mygrid, true);
    AddNewRow(mygrid, [
    //0 // Remove Column
    //,
        $("#MAS070_EmployeeDetail #EmpNo").val()
        , "(1) " + $("#EmpFirstNameEN").val() + " " + $("#EmpLastNameEN").val() + "<br/>(2) " + $("#EmpFirstNameLC").val() + " " + $("#EmpLastNameLC").val()
        , $("#EmailAddress").val() == "" ? "" : $("#EmailAddress").val() + c_email_suffix
        , ""
    ]);

    var row_idx = mygrid.getRowsNum() - 1;
    var row_id = mygrid.getRowId(row_idx);
    GenerateEditButton(mygrid, btn_employee_detail, row_id, "Edit", true);

    $("#Search_Result").show();

    //Set grid to last page
    mygrid.changePage(Math.ceil(mygrid.getRowsNum() / ROWS_PER_PAGE_FOR_VIEWPAGE));
}
function editEmpInGrid() {
    mygrid.cells(selectedEmp, mygrid.getColIndexById('EmpNo')).setValue($("#MAS070_EmployeeDetail #EmpNo").val());
    mygrid.cells(selectedEmp, mygrid.getColIndexById('EmpFullName')).setValue(
        "(1) " + $("#EmpFirstNameEN").val() + " " + $("#EmpLastNameEN").val() + "<br/>(2) " + $("#EmpFirstNameLC").val() + " " + $("#EmpLastNameLC").val()
    );

    var email = $("#EmailAddress").val();
    if (email != "") {
        email = email + c_email_suffix;
    }
    mygrid.cells(selectedEmp, mygrid.getColIndexById('EmailAddress')).setValue(email);
}

function addBelongingToGrid(rowIndex, ModifyMode) {
    //    var apprAuth = $("#ApprovalAuthorityPersonFlag").prop("checked") ? "1" : "0";
    var apprAuth = $("#ApprovalAuthorityPersonFlag").prop("checked") ? c_yes : c_no;
    var mainDepart = $("#MainDepartmentFlag").prop("checked") ? "1" : "0";
//    var DepPersonInCharge = $("#DepPersonInchargeFlag").prop("checked") ? "1" : "0";
    var DepPersonInCharge = $("#DepPersonInchargeFlag").prop("checked") ? c_yes : c_no;
    var validFrom = $("#MAS070_BelongingDetail #ValidFrom").val() == "" ? "" : $("#MAS070_BelongingDetail #ValidFrom").datepicker("getDate");
    var validTo = $("#MAS070_BelongingDetail #ValidTo").val() == "" ? "" : $("#MAS070_BelongingDetail #ValidTo").datepicker("getDate");

    CheckFirstRowIsEmpty(mygrid2, true);

    AddNewRow(mygrid2, [
        rowIndex + 1
        , mainDepart
        , $("#OfficeCode option[selected='selected']").text()
        , $("#DepartmentCode option[selected='selected']").text()
        , $("#PositionCode option[selected='selected']").text()
        , DepPersonInCharge
        , apprAuth
        , validFrom
        , validTo
        , ""
        , ""
        , $("#OfficeCode").val()
        , $("#DepartmentCode").val()
        , $("#PositionCode").val()
        , $("#BelongingID").val()
        , ""
        , ModifyMode
    ]);

    var row_idx = mygrid2.getRowsNum() - 1;
    var row_id = mygrid2.getRowId(row_idx);

    GenerateEditButton(mygrid2, btn_belonging_detail, row_id, "Edit", true);
    GenerateRemoveButton(mygrid2, btn_belonging_remove, row_id, "Remove", true);
    mygrid2.setSizes();

    BindGridButtonClickEvent(btn_belonging_detail, row_id, function (rid) {
        mygrid2.selectRow(mygrid2.getRowIndex(rid));
        if (!addBelMode && !editBelMode) {
            selectedBel = rid;
            eventBelongingDetail(rid);
        }
    });
    BindGridButtonClickEvent(btn_belonging_remove, row_id, function (rid) {
        if (!addBelMode && !editBelMode) {
            eventBelongingDelete(rid);
        }
    });

}

function editBelongingGrid(rowIndex, ModifyMode) {
    //    var apprAuth = $("#ApprovalAuthorityPersonFlag").prop("checked") ? "1" : "0";
    var apprAuth = $("#ApprovalAuthorityPersonFlag").prop("checked") ? c_yes : c_no;
    var mainDepart = $("#MainDepartmentFlag").prop("checked") ? "1" : "0";
    //    var DepPersonInCharge = $("#DepPersonInchargeFlag").prop("checked") ? "1" : "0";
    var DepPersonInCharge = $("#DepPersonInchargeFlag").prop("checked") ? c_yes : c_no;
    var validFrom = $("#MAS070_BelongingDetail #ValidFrom").val() == "" ? "" : $("#MAS070_BelongingDetail #ValidFrom").datepicker("getDate");
    var validTo = $("#MAS070_BelongingDetail #ValidTo").val() == "" ? "" : $("#MAS070_BelongingDetail #ValidTo").datepicker("getDate");

    mygrid2.cells(selectedBel, mygrid2.getColIndexById('MainDepartmentFlag')).setValue(mainDepart);
    mygrid2.cells(selectedBel, mygrid2.getColIndexById('OfficeDisplay')).setValue($("#OfficeCode option[selected='selected']").text());
    mygrid2.cells(selectedBel, mygrid2.getColIndexById('DepartmentDisplay')).setValue($("#DepartmentCode option[selected='selected']").text());
    mygrid2.cells(selectedBel, mygrid2.getColIndexById('PositionDisplay')).setValue($("#PositionCode option[selected='selected']").text());
    mygrid2.cells(selectedBel, mygrid2.getColIndexById('DepPersonInchargeFlag')).setValue(DepPersonInCharge);
    mygrid2.cells(selectedBel, mygrid2.getColIndexById('ApprovalAuthorityPersonFlag')).setValue(apprAuth);
    mygrid2.cells(selectedBel, mygrid2.getColIndexById('ValidFrom')).setValue(validFrom);
    mygrid2.cells(selectedBel, mygrid2.getColIndexById('ValidTo')).setValue(validTo);
    mygrid2.cells(selectedBel, mygrid2.getColIndexById('OfficeCode')).setValue($("#OfficeCode").val());
    mygrid2.cells(selectedBel, mygrid2.getColIndexById('DepartmentCode')).setValue($("#DepartmentCode").val());
    mygrid2.cells(selectedBel, mygrid2.getColIndexById('PositionCode')).setValue($("#PositionCode").val());
    mygrid2.cells(selectedBel, mygrid2.getColIndexById('BelongingID')).setValue($("#BelongingID").val());
    mygrid2.cells(selectedBel, mygrid2.getColIndexById('ModifyMode')).setValue(ModifyMode);

//    DeleteRow(mygrid2, selectedBel);

//    CheckFirstRowIsEmpty(mygrid2, true);
//    mygrid2.addRow(mygrid2.uid(),
//        [rowIndex
//        , mainDepart
//        , $("#OfficeCode option[selected='selected']").text()
//        , $("#DepartmentCode option[selected='selected']").text()
//        , $("#PositionCode option[selected='selected']").text()
//        , DepPersonInCharge
//        , apprAuth
//        , validFrom
//        , validTo
//        , ""
//        , ""
//        , $("#OfficeCode").val()
//        , $("#DepartmentCode").val()
//        , $("#PositionCode").val()
//        , $("#BelongingID").val()
//        , ""
//        , ModifyMode],
//        rowIndex);

//    var row_idx = mygrid2.getRowsNum() - 1;
//    var row_id = mygrid2.getRowId(row_idx);

//    GenerateEditButton(mygrid2, btn_belonging_detail, row_id, "Edit", true);
//    GenerateRemoveButton(mygrid2, btn_belonging_remove, row_id, "Remove", true);
//    mygrid2.setSizes();

//    BindGridButtonClickEvent(btn_belonging_detail, row_id, function (rid) {
//        mygrid2.selectRow(mygrid2.getRowIndex(rid));
//        if (!addBelMode && !editBelMode) {
//            selectedBel = rid;
//            eventBelongingDetail(rid);
//        }
//    });
//    BindGridButtonClickEvent(btn_belonging_remove, row_id, function (rid) {
//        if (!addBelMode && !editBelMode) {
//            eventBelongingDelete(rid);
//        }
//    });
}

// GET DATA #########################################################################################################################################

function employeeSearch() {

    // disable search button
    $("#btnSearch").attr("disabled", true);
    master_event.LockWindow(true);

    var parameter = CreateObjectData($("#MAS070_Search").serialize());
    call_ajax_method(
        '/Master/MAS070_ValidateEmployeeSearch',
        parameter,
        function (result, controls) {
            if (result != undefined) {
                loadGridData();
                $("#Employee_Detail").hide();
                $("#Belonging_Detail").hide();

                SetCancelCommand(false, cancelEmployeeData);
                SetConfirmCommand(false, saveEmployeeInformation);
            }
            else {
                // enable search button
                $("#btnSearch").attr("disabled", false);
                master_event.LockWindow(false);
            }
        }
    );
}

function employeeSearchDetail(EmpNo) {
    //6.1	Set dtEmployee to “Maintain user” section
    call_ajax_method(
        '/Master/MAS070_EmployeeSearchDetail/',
        { EmpNo: EmpNo },
        function (result, controls) {
            if (result != undefined) {
                $("#MAS070_Search input").attr("readonly", true);
                $("#Search_Result input").attr("readonly", true);
                $("#MAS070_Search button").attr("disabled", true);
                //$("#Search_Result button").attr("disabled", true);
                setEnableToEmployeeGrid(false);

                $("#MAS070_EmployeeDetail").clearForm();
                $("#MAS070_EmployeeDetail").bindJSON(result);

                result.StartDate = ConvertDateObject(result.StartDate);
                result.EndDate = ConvertDateObject(result.EndDate);
                //                $("#MAS070_EmployeeDetail #EndDate").val(ConvertDateToTextFormat(result.EndDate));
                //                $("#MAS070_EmployeeDetail #StartDate").val(ConvertDateToTextFormat(result.StartDate));

                SetDateFromToData("#MAS070_EmployeeDetail #StartDate", "#MAS070_EmployeeDetail #EndDate",
                    ConvertDateToTextFormat(result.StartDate), ConvertDateToTextFormat(result.EndDate));

                $("#MAS070_EmployeeDetail #StartDate").datepicker("getDate");
                $("#MAS070_EmployeeDetail #EndDate").datepicker("getDate");
                //                SetDateFromToData(
                //                    "#MAS070_EmployeeDetail #StartDate", "#MAS070_EmployeeDetail #EndDate",
                //                    new Date(ConvertDateObject(result.StartDate)), new Date(ConvertDateObject(result.EndDate)));

                $("#EmpUpdateDate").val(ConvertDateObject(result.UpdateDate));
                $("#EmpUpdateBy").val(result.UpdateBy);

                if (result.DeleteFlag == true) {
                    $("#DeleteFlag").attr("checked", true);
                } else {
                    $("#DeleteFlag").attr("checked", false);
                }

                if (result.IncidentNotificationFlag == true) {
                    $("#IncidentNotificationFlag").attr("checked", true);
                } else {
                    $("#IncidentNotificationFlag").attr("checked", false);
                }

                $('#change-password-flag-area').show();
                $('#password-area').hide();
                $('#currentPassword').val(result.Password);
                $('#Password').val(result.Password);

                //6.2	Set dtEmployee to “Belonging list” section
                loadBelongingGridData(result.EmpNo, true);

                //6.4	Disable [Employee code] text box
                $("#MAS070_EmployeeDetail #EmpNo").focus();
                $("#MAS070_EmployeeDetail #EmpNo").attr("readonly", true);

                //6.3	If DeleteFlag=True Then Disable all input control in “Maintain user” section except chkDeleteFlag
                if ($("#DeleteFlag").prop("checked")) {
                    disableEmployeeDetail();
                    SetConfirmCommand(false, saveEmployeeInformation);
                } else {
                    if (permission.EDIT == "True") {
                        enableEmployeeDetail();
                        $("#MAS070_EmployeeDetail #EmpNo").attr("readonly", true);
                        SetConfirmCommand(true, saveEmployeeInformation);
                    } else {
                        disableEmployeeDetail();
                        SetConfirmCommand(false, saveEmployeeInformation);
                    }
                }

                if (permission.DELETE == "True") {
                    $("#DeleteFlag").attr("disabled", false);
                } else {
                    $("#DeleteFlag").attr("disabled", true);
                }

                //6.5	Display “Maintain user” section
                $("#Employee_Detail").show();
                SetCancelCommand(true, cancelEmployeeData);
            }
        }
    );
}

function toggleChangePasswordFlag() {
    if ($('#ChangePasswordFlag').prop('checked')) {
        $('#Password').val('');
        $('#password-area').show();
    } else {
        $('#password-area').hide();
        $('#Password').val($('#currentPassword').val());
    }
}

// LOAD GRID ########################################################################################################################################

function loadGridData() {
    //3.2	Search Employee Data
    if ($("#grid_result").length > 0) {
        var parameter = CreateObjectData($("#MAS070_Search").serialize());

        $("#grid_result").LoadDataToGrid(mygrid, ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/Master/MAS070_EmployeeSearch", parameter, "dtEmployee", false,
        function () {
            // enable search button
            $("#btnSearch").attr("disabled", false);
            master_event.LockWindow(false);
        }
        ,
        function (result, controls, isWarning) {
            if (isWarning == undefined) {
                $("#Search_Result").show();
            }
        }
        );
    }

}

function loadBelongingGridData(EmpNo, doLoad) {
    if ($("#belGrid_result").length > 0) {
        var parameter = { EmpNo: EmpNo };

        if (doLoad) {
            $("#belGrid_result").LoadDataToGrid(mygrid2, ROWS_PER_PAGE_FOR_VIEWPAGE, false, "/Master/MAS070_SearchBelonging", parameter, "dtBelonging", false, null,
                function (result, controls, isWarning) {
                    if (isWarning == undefined) {
                        if (mygrid2.getRowsNum() == 0) {
                            $("#belGrid_result").hide();
                        } else {
                            $("#belGrid_result").show();
                        }
                    }
                }
            );
        }
    }

}

// SAVE SAVE SAVE !!!!! #############################################################################################################################

function saveEmployeeInformation() {

    DisableConfirmCommand(true);

    if ($("#DeleteFlag").prop("checked")) {
        saveEmployeeDelete();
    } else if (newMode) {
        var ocJson = ConfirmAll("ADD");
        saveEmployee(ocJson, true);
    } else if (detailMode) {
        var ocJson = ConfirmAll("EDIT");
        saveEmployee(ocJson, false);
    }
}

function saveEmployeeDelete() {
    var msgprm = { "module": "Common", "code": "MSG0142" }; // param: c_delete };
    call_ajax_method("/Shared/GetMessage", msgprm, function (data, ctrl) {
        OpenYesNoMessageDialog(data.Code, data.Message, function () {
            call_ajax_method(
                '/Master/MAS070_DeleteEmployee',
                { EmpNo: $("#MAS070_EmployeeDetail #EmpNo").val(), ModifyMode: "DEL", DeleteFlag: $("#DeleteFlag").prop("checked") },
                function (result, controls) {
                    if (result != undefined && result != "NP") {
                        detailMode = false;
                        newMode = false;

                        $("#MAS070_Search input").attr("readonly", false);
                        $("#Search_Result input").attr("readonly", false);
                        $("#MAS070_Search button").attr("disabled", false);
                        //$("#Search_Result button").attr("disabled", false);
                        setEnableToEmployeeGrid(true);
                        if (permission.ADD == "False") {
                            $("#btnNew").attr("disabled", true);
                        }

                        clearEmployeeDetail();
                        $("#Employee_Detail").hide();
                        $("#Belonging_Detail").hide();
                        SetCancelCommand(false, cancelEmployeeData);
                        SetConfirmCommand(false, saveEmployeeInformation);

                        var param = { "module": "Common", "code": "MSG0047" };
                        call_ajax_method("/Shared/GetMessage", param, function (data) {
                            OpenInformationMessageDialog(data.Code, data.Message);
                            DeleteRow(mygrid, selectedEmp);
                        });

                        delBelongingList = [];
                    }
                    else {
                        // enable confrim button
                        DisableConfirmCommand(false);
                    }
                }

            );
        },
        function () {
            // enable confrim button
            DisableConfirmCommand(false);
        }
        );
    });
}

function saveEmployee(employeeInfo, add) {
    call_ajax_method_json(
        '/Master/MAS070_SaveEmployee',
        employeeInfo,
        function (result, controls) {

            // enable confrim button
            DisableConfirmCommand(true);

            if (controls != undefined) {
                VaridateCtrl(["MAS070_EmployeeDetail #EmpNo",
                                "EmpFirstNameEN",
                                "EmpLastNameEN",
                                "EmpFirstNameLC",
                                "EmpLastNameLC"], controls);
                // enable confrim button
                DisableConfirmCommand(false);
            }

            if (result != undefined && result == "P") {
                detailMode = false;
                newMode = false;

                $("#MAS070_Search input").attr("readonly", false);
                $("#Search_Result input").attr("readonly", false);
                $("#MAS070_Search button").attr("disabled", false);
                //$("#Search_Result button").attr("disabled", false);

                if (add) {
                    addNewEmpToGrid();
                } else {
                    editEmpInGrid();
                }

                setEnableToEmployeeGrid(true);
                if (permission.ADD == "False") {
                    $("#btnNew").attr("disabled", true);
                }

                clearEmployeeDetail();
                $("#Employee_Detail").hide();
                $("#Belonging_Detail").hide();

                SetCancelCommand(false, cancelEmployeeData);
                SetConfirmCommand(false, saveEmployeeInformation);

                var param = { "module": "Common", "code": "MSG0046" };
                call_ajax_method("/Shared/GetMessage", param, function (data) {
                    OpenInformationMessageDialog(data.Code, data.Message);
                });

                delBelongingList = [];
            }

            // EmpNo already exist and deleted confirm that you want to save
            if (result != undefined && result == "CONF") {
                var msgprm = { "module": "Master", "code": "MSG1047" };
                call_ajax_method("/Shared/GetMessage", msgprm, function (data, ctrl) {
                    OpenYesNoMessageDialog(data.Code, data.Message, function () {
                        var ocJson = ConfirmAll("EDIT");
                        //saveEmployee(ocJson, false);
                        saveEmployee(ocJson, true);
                    },
                        function () { }
                    );
                });
            }
            else {
                // enable confrim button
                DisableConfirmCommand(false);
            }
        }
    );


}

function ConfirmAll(ModifyMode) {
    var belongingList = [];
    for (var i = 0; i < mygrid2.getRowsNum(); i++) {
        var apprAuth = mygrid2.cells2(i, mygrid2.getColIndexById('ApprovalAuthorityPersonFlag')).getValue() == c_yes;
        var mainDepart = mygrid2.cells2(i, mygrid2.getColIndexById('MainDepartmentFlag')).getValue() == "√";
        var DepPersonInCharge = mygrid2.cells2(i, mygrid2.getColIndexById('DepPersonInchargeFlag')).getValue() == c_yes;

        belongingList.push({
            DepPersonInchargeFlag: DepPersonInCharge,
            ApprovalAuthorityPersonFlag: apprAuth,
            MainDepartmentFlag: mainDepart,
            EmpNo: $("#MAS070_EmployeeDetail #EmpNo").val(),
            OfficeCode: mygrid2.cells2(i, mygrid2.getColIndexById('OfficeCode')).getValue(),
            DepartmentCode: mygrid2.cells2(i, mygrid2.getColIndexById('DepartmentCode')).getValue(),
            PositionCode: mygrid2.cells2(i, mygrid2.getColIndexById('PositionCode')).getValue(),
            BelongingID: mygrid2.cells2(i, mygrid2.getColIndexById('BelongingID')).getValue(),
            StartDate: mygrid2.cells2(i, mygrid2.getColIndexById('ValidFrom')).getValue(),
            EndDate: mygrid2.cells2(i, mygrid2.getColIndexById('ValidTo')).getValue(),
            UpdateDate: ConvertDateObject(mygrid2.cells2(i, mygrid2.getColIndexById('UpdateDate')).getValue()),
            ModifyMode: mygrid2.cells2(i, mygrid2.getColIndexById('ModifyMode')).getValue()
        });
    }

    var employee = {
        DeleteFlag: $("#DeleteFlag").prop("checked"),
        EmpNo: $("#MAS070_EmployeeDetail #EmpNo").val(),
        EmpFirstNameEN: $("#EmpFirstNameEN").val(),
        EmpLastNameEN: $("#EmpLastNameEN").val(),
        EmpFirstNameLC: $("#EmpFirstNameLC").val(),
        EmpLastNameLC: $("#EmpLastNameLC").val(),
        StartDate: $("#MAS070_EmployeeDetail #StartDate").val(),
        EndDate: $("#MAS070_EmployeeDetail #EndDate").val(),
        //EmailAddress: $("#EmailAddress").val() + $("#SecomAddress").val(),
        EmailAddress: $("#EmailAddress").val() == "" ? "" : $("#EmailAddress").val() + c_email_suffix,
        IncidentNotificationFlag: $("#IncidentNotificationFlag").prop("checked"),
        UpdateDate: $("#EmpUpdateDate").val(),
        UpdateBy: $("#EmpUpdateBy").val(),
        Password: $("#Password").val(),
        ChangePasswordFlag: $('#ChangePasswordFlag').prop('checked'),

        ModifyMode: ModifyMode
    }

    var employeeInfo = { "employee": employee, "belongingList": belongingList, delBelList: delBelongingList }

    //    return JSON.stringify(employeeInfo);
    return employeeInfo
}
