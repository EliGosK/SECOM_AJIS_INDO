/// <reference path = "../../Scripts/jquery-1.5.1-vsdoc.js" />
/// <reference path = "../../Scripts/Base/GridControl.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxcommon.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxgrid.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxgridcell.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/ext/dhtmlxgrid_pgn.js" />

/// <reference path = "../../Scripts/jquery.treeview.js" />
/// <reference path = "../../Scripts/tree/dhtmlxcommon.js" />
/// <reference path = "../../Scripts/tree/dhtmlxtree.js" />
/// <reference path = "../../Scripts/tree/ext/dhtmlxtree_start.js" />

var search_tree = null;
var set_tree = null;

var newMode = false;
var detailMode = false;
var newIndividual = false;
var isIndiviual = false;

var crPermissionGroupCode = "";
var PermissionGroupName = "";
var crOfficeCode = "";
var crDepartmentCode = "";
var crPositionCode = "";
var PermissionType = "";
var crPermissionIndividualCode = "";
var crPermissionIndividualName = "";
var UpdateDate = null;

var mygrid;
var mygrid2;

// tt
var mygrid_enable = true;

var empCount = 0;

var btn_permission_detail = "btnPermissionDetail";
var btn_employee_remove = "btnEmployeeRemove";

var delEmp = null;
var addEmp = null;
var norEmp = null;

$(document).ready(function () {


    initial_tree();
    //clear();
    $("#Search_Result").hide();
    $("#Maintain_Permission_Group").hide();
    $("#Employee_Info").hide();

    $("#txtUseInEmployeeName").attr("readonly", true);
    $("#txtUseInEmployeeCode").attr("readonly", true);
    $("#EmpFirstNameEN").attr("readonly", true);
    $("#PermissionGroupCode").attr("readonly", true);
    $("#PermissionType").attr("readonly", true);

    $("#chkPermissionOffice").attr("checked", true);
    $("#chkPermissionIndividual").attr("checked", true);
    checkEnableEmployeeSearch();

    $("#MAS080_PermissionInfo").clearForm();
    $("#MAS080_Office_Info").clearForm();
    $("#MAS080_Employee_Info").clearForm();
    $("#MAS080_SetPermission").clearForm();

    $("#btnSearch").click(search);
    $("#btnClear").click(clear);
    $("#chkPermissionIndividual").click(checkEnableEmployeeSearch);
    $("#chkPermissionOffice").click(checkPermissionOfficeSearch);
    if (permission_screen.EDIT == "True") {
        $("#btnNewIndividualPermission").click(newIndividualPermission);
    } else {
        $("#btnNewIndividualPermission").attr("disabled", true);
    }

    $("#btnAdd").click(addEmpToGrid);
    if (permission_screen.DELETE == "True") {
        $("#DeletePermission").click(toggleDeletePermission);
    } else {
        $("#DeletePermission").attr("disabled", true);
    }
    $("#txtUseInEmployeeCode").blur(getEmployeeNameForUseIn);
    //$("#txtUseInEmployeeCode").focusout(getEmployeeNameForUseIn);
    $("#EmpNo").blur(getEmployeeNameForIndividual);
    //$("#EmpNo").focusout(getEmployeeNameForIndividual);

    //    $("#btnCancel").click(confirmCancel);
    //    $("#btnConfirm").click(confirmPress);

    $("#txtPermissionGroupName").InitialAutoComplete("/Master/MAS080_GetPermissionGroupName");
    $("#txtUseInEmployeeCode").InitialAutoComplete("/Master/MAS080_EmployeeCode");

    if (permission_screen.ADD == "True") {
        $("#btnNewOfficePermission").click(newPermission);
    } else {
        $("#btnNewOfficePermission").attr("disabled", true);
    }

    //$("#cboModuleName").prepend("<option value='blank' selected='selected'>Please Select Module</option>");
    $("#cboModuleName").prepend("<option value='blank' style='display:none;'></option>");

    $("#cboModuleName").RelateControlEvent(changeModule_related);

    $("#cboModuleName").val("");
    changeModule_related();

    mygrid = $("#grid_result").InitialGrid(ROWS_PER_PAGE_FOR_VIEWPAGE, false, "/Master/MAS080_InitPermissionGrid");

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
                    GenerateEditButton(mygrid, btn_permission_detail, row_id, "Edit", mygrid_enable);

                }

                BindGridButtonClickEvent(btn_permission_detail, row_id, function (rid) {

                    // tt
                    mygrid_enable = false;
                    enable_grid_button(mygrid, "Edit", btn_permission_detail, false);

                    if (!newMode && !detailMode) {
                        crPermissionGroupCode = mygrid.cells(rid, mygrid.getColIndexById('crPermissionGroupCode')).getValue();
                        var permiss = mygrid.cells(rid, mygrid.getColIndexById('PermissionTypeCode')).getValue();

                        if (permiss == "0") {
                            PermissionType = c_permission_type_office;
                            isIndiviual = false;
                        } else {
                            isIndiviual = true;
                            PermissionType = c_permission_type_individual;
                        }
                        //htmlDecode
                        PermissionGroupName = htmlDecode(mygrid.cells(rid, mygrid.getColIndexById('crPermissionGroupName')).getValue());

                        var parmissIndiCode = mygrid.cells(rid, mygrid.getColIndexById('crPermissionIndividualCode')).getValue();
                        crPermissionIndividualCode = parmissIndiCode == "-" ? "" : parmissIndiCode;

                        var permissIndi = mygrid.cells(rid, mygrid.getColIndexById('crPermissionIndividualName')).getValue();
                        crPermissionIndividualName = permissIndi == "-" ? "" : htmlDecode(permissIndi);

                        crOfficeCode = mygrid.cells(rid, mygrid.getColIndexById('crOfficeCode')).getValue();
                        crDepartmentCode = mygrid.cells(rid, mygrid.getColIndexById('crDepartmentCode')).getValue();
                        crPositionCode = mygrid.cells(rid, mygrid.getColIndexById('crPositionCode')).getValue();
                        UpdateDate = mygrid.cells(rid, mygrid.getColIndexById('UpdateDate')).getValue();
                        getFunctionDetail(crPermissionGroupCode, crPermissionIndividualCode, isIndiviual);
                    }
                    mygrid.selectRow(mygrid.getRowIndex(rid));
                });
            }
            mygrid.setSizes();
        }
    });

    mygrid2 = $("#employee_grid").InitialGrid(0, true, "/Master/MAS080_InitEmployeeGrid");

    SpecialGridControl(mygrid2, ["Remove"]);
    BindOnLoadedEvent(mygrid2, function (gen_ctrl) {
        norEmp = [];
        if (mygrid2.getRowsNum() != 0) {
            for (var i = 0; i < mygrid2.getRowsNum(); i++) {
                var row_id = mygrid2.getRowId(i);

                if (gen_ctrl == true) {
                    if (permission_screen.EDIT == "True") {
                        GenerateRemoveButton(mygrid2, btn_employee_remove, row_id, "Remove", true);
                    } else {
                        GenerateRemoveButton(mygrid2, btn_employee_remove, row_id, "Remove", false);
                    }
                }

                BindGridButtonClickEvent(btn_employee_remove, row_id, removeEmployee);

                norEmp.push(mygrid2.cells(row_id, mygrid2.getColIndexById('EmpNo')).getValue());
            }
            mygrid2.setSizes();
        }
    });


});

// tt
function enable_grid_button(grid, col_id, button_id, enable) {
    for (var i = 0; i < grid.getRowsNum(); i++) {
        var row_id = grid.getRowId(i);

        var thisid = GenerateGridControlID(button_id, row_id);
        thisid = "#" + thisid;

        var col = grid.getColIndexById(col_id);

        if ($.find(thisid).length > 0) {

            if (enable) {
                var title = grid.cells(row_id, col).getAttribute("defToolTip");
                $(thisid).attr("class", "row-image");
                $(thisid).attr("title", title);
            }
            else {
                $(thisid).attr("class", "row-image-disabled");
                $(thisid).attr("title", "");
            }


        }
    }

    grid.attachEvent("onAfterSorting", function (index, type, direction) {
        for (var i = 0; i < grid.getRowsNum(); i++) {
            var row_id = grid.getRowId(i);

            var thisid = GenerateGridControlID(button_id, row_id);
            thisid = "#" + thisid;

            var col = grid.getColIndexById(col_id);

            if ($.find(thisid).length > 0) {

                if (enable) {
                    var title = grid.cells(row_id, col).getAttribute("defToolTip");
                    $(thisid).attr("class", "row-image");
                    $(thisid).attr("title", title);
                }
                else {
                    $(thisid).attr("class", "row-image-disabled");
                    $(thisid).attr("title", "");
                }


            }
        }
    });


}

function removeEmployee(rid) {

    var empNo = mygrid2.cells(rid, mygrid2.getColIndexById('EmpNo')).getValue();

    if (addEmp != undefined) {
        for (var idx = 0; idx < addEmp.length; idx++) {
            if (addEmp[idx] == empNo) {
                addEmp.splice(idx, 1);
                break;
            }
        }
    }
    if (delEmp == undefined) {
        delEmp = [];
    }
    var found = false;
    for (var idx = 0; idx < delEmp.length; idx++) {
        if (delEmp[idx] == empNo) {
            found = true;
            break;
        }
    }
    if (found == false) {
        if (norEmp != undefined)
        {
            for (var idx = 0; idx < norEmp.length; idx++) {
                if (norEmp[idx] == empNo) {
                    found = true;
                    break;
                }    
            }
        }

        if (found == true)
            delEmp.push(empNo);
    }

    DeleteRow(mygrid2, rid);
}

function getURLForGetXML(urlGetXML) {
    if (urlGetXML.indexOf("k=") < 0) {
        var key = ajax_method.GetKeyURL();
        if (key != "") {
            if (urlGetXML.indexOf("?") > 0) {
                urlGetXML = urlGetXML + "&k=" + key;
            }
            else {
                urlGetXML = urlGetXML + "?k=" + key;
            }
        }
    }
    if (urlGetXML.indexOf("?") > 0) {
        urlGetXML = urlGetXML + "&ajax=1";
    }
    else {
        urlGetXML = urlGetXML + "?ajax=1";
    }

    return ajax_method.GenerateURL(urlGetXML);
}

function initial_tree() {
    search_tree = new dhtmlXTreeObject("search_tree", "100%", "100%", 0);
    search_tree.setImagePath("../../Scripts/tree/imgs/");
    search_tree.enableCheckBoxes(1);
    search_tree.enableThreeStateCheckboxes(true);
    search_tree.setXMLAutoLoading(getURLForGetXML("/Master/GetFuntionXml"));
    //search_tree.loadXML(getURLForGetXML("/Master/GetFuntionXml"));

    set_tree = new dhtmlXTreeObject("set_tree", "100%", "100%", 0);
    set_tree.setImagePath("../../Scripts/tree/imgs/");
    set_tree.enableCheckBoxes(1);
    set_tree.enableThreeStateCheckboxes(true);
    set_tree.setXMLAutoLoading(getURLForGetXML("/Master/GetFuntionXml"));
    //set_tree.loadXML(getURLForGetXML("/Master/GetFuntionXml"));

    set_tree.attachEvent("onCheck", function (id, state) {
        if (state == "1") {
            set_tree.setCheck(c_view + id.substring(1), "1");

            if (id.substring(0, 2) == c_planner) {
                set_tree.setCheck(c_operate + id.substring(2), "1");
            }
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

// DISABLE ENABLE CONTROL ###########################################################################################################################

function enableSearch() {
    var allId = search_tree.getAllChildless().split(",");
    var i = 0;
    for (i = 0; i < allId.length; i++) {
        search_tree.disableCheckbox(allId[i], 0);
    }

    allId = search_tree.getAllItemsWithKids().split(",");
    i = 0;
    for (i = 0; i < allId.length; i++) {
        search_tree.disableCheckbox(allId[i], 0);
    }

    $("#Search_Permission_Group select").attr("disabled", false);

    $("#btnSearch").attr("disabled", false);
    $("#btnClear").attr("disabled", false);

    if (permission_screen.ADD == "True") {
        $("#btnNewOfficePermission").attr("disabled", false);
    }

    $("#txtPermissionGroupName").attr("readonly", false);
    if ($("#chkPermissionIndividual").prop("checked")) {
        $("#txtUseInEmployeeCode").attr("readonly", false);
    }
    //$("#txtUseInEmployeeName").attr("readonly", false);
    $("#chkPermissionOffice").attr("disabled", false);
    $("#chkPermissionIndividual").attr("disabled", false);
}

function disableSearch() {
    var allId = search_tree.getAllChildless().split(",");
    var i = 0;
    for (i = 0; i < allId.length; i++) {
        search_tree.disableCheckbox(allId[i], 1);
    }

    allId = search_tree.getAllItemsWithKids().split(",");
    i = 0;
    for (i = 0; i < allId.length; i++) {
        search_tree.disableCheckbox(allId[i], 1);
    }

    $("#Search_Permission_Group select").attr("disabled", true);

    $("#btnSearch").attr("disabled", true);
    $("#btnClear").attr("disabled", true);
    $("#btnNewOfficePermission").attr("disabled", true);

    $("#txtPermissionGroupName").attr("readonly", true);
    $("#txtUseInEmployeeCode").attr("readonly", true);
    $("#txtUseInEmployeeName").attr("readonly", true);
    $("#chkPermissionOffice").attr("disabled", true);
    $("#chkPermissionIndividual").attr("disabled", true);
}

function setEnablePermissionGrid(enable) {
    if ($("#grid_result").length > 0) {
        if (mygrid.getRowsNum() != 0) {
            for (var i = 0; i < mygrid.getRowsNum(); i++) {
                var row_id = mygrid.getRowId(i);
                EnableGridButton(mygrid, btn_permission_detail, row_id, "Edit", enable);
            }
        }
    }

    //    var removeCol = mygrid.getColIndexById("Detail");
    //    mygrid.setColumnHidden(removeCol, !enable);
    //    mygrid.setSizes();

    //    // Akat K. : this is a really bad solution if there are better solution please tell me.
    //    $("#grid_result").hide();
    //    $("#grid_result").show();
}

function setEnableEmployeeGrid(enable) {
    if ($("#employee_grid").length > 0) {
        if (mygrid2.getRowsNum() != 0) {
            for (var i = 0; i < mygrid2.getRowsNum(); i++) {
                var row_id = mygrid2.getRowId(i);
                EnableGridButton(mygrid2, btn_employee_remove, row_id, "Remove", enable);
            }
        }
    }

    //    var removeCol2 = mygrid2.getColIndexById("Remove");
    //    mygrid2.setColumnHidden(removeCol2, !enable);
    //    mygrid2.setSizes();

    //    // Akat K. : this is a really bad solution if there are better solution please tell me.
    //    $("#employee_grid").hide();
    //    $("#employee_grid").show();
}

function checkEnableEmployeeSearch() {
    if ($("#chkPermissionIndividual").prop("checked")) {
        $("#txtUseInEmployeeCode").attr("readonly", false);
        //$("#chkPermissionOffice").attr("checked", false);
    } else {
        $("#txtUseInEmployeeCode").attr("readonly", true);
        $("#txtUseInEmployeeCode").val("");
        $("#txtUseInEmployeeName").val("");
        //$("#chkPermissionOffice").attr("checked", true);
    }
}

function checkPermissionOfficeSearch() {
    if ($("#chkPermissionOffice").prop("checked")) {
        //$("#txtUseInEmployeeCode").attr("readonly", true);
        //$("#chkPermissionIndividual").attr("checked", false);
    } else {
        //$("#txtUseInEmployeeCode").attr("readonly", false);
        //$("#chkPermissionIndividual").attr("checked", true);
    }
}

function toggleDeletePermission() {
    if ($("#DeletePermission").prop("checked")) {
        disablePermissionDetail();
    } else {
        enablePermissionDetail();
    }
}

function enablePermissionDetail() {
    if (permission_screen.EDIT == "True" || newMode) {
        enablePermissionDetailTree();
        //        var allId = set_tree.getAllChildless().split(",");
        //        var i = 0;
        //        for (i = 0; i < allId.length; i++) {
        //            set_tree.disableCheckbox(allId[i], 0);
        //        }

        //        allId = set_tree.getAllItemsWithKids().split(",");
        //        i = 0;
        //        for (i = 0; i < allId.length; i++) {
        //            set_tree.disableCheckbox(allId[i], 0);
        //        }

        $("#PermissionGroupName").attr("readonly", false);
        $("#PermissionIndividualName").attr("readonly", false);

        $("#EmpNo").attr("readonly", false);
        $("#btnAdd").attr("disabled", false);
        $("#MAS080_Employee_Info button").attr("disabled", false);
    }

    if (!isIndiviual && !newMode && permission_screen.EDIT == "True") {
        $("#btnNewIndividualPermission").attr("disabled", false);
    } else {
        $("#btnNewIndividualPermission").attr("disabled", true);
    }
}

function disablePermissionDetail() {
    var allId = set_tree.getAllChildless().split(",");
    var i = 0;
    for (i = 0; i < allId.length; i++) {
        set_tree.disableCheckbox(allId[i], 1);
    }

    allId = set_tree.getAllItemsWithKids().split(",");
    i = 0;
    for (i = 0; i < allId.length; i++) {
        set_tree.disableCheckbox(allId[i], 1);
    }

    $("#PermissionGroupName").attr("readonly", true);
    $("#PermissionIndividualName").attr("readonly", true);
    $("#btnNewIndividualPermission").attr("disabled", true);
    $("#EmpNo").attr("readonly", true);
    $("#btnAdd").attr("disabled", true);
    $("#MAS080_Employee_Info button").attr("disabled", true);
}

// ACTION BUTTON AND EVENT ##########################################################################################################################

function getEmployeeNameForUseIn() {

    if ($("#txtUseInEmployeeCode").prop("readonly") == false && $("#txtUseInEmployeeCode").val() != "") {

        call_ajax_method(
            '/Master/MAS080_GetEmpNameByEmpNo',
            { empNo: $("#txtUseInEmployeeCode").val() },
            function (result, controls) {
                $("#txtUseInEmployeeName").val(result);
            }
        );

    } else if ($("#txtUseInEmployeeCode").val() == "") {
        $("#txtUseInEmployeeName").val("");
    }


}

function getEmployeeNameForIndividual() {

    if ($("#EmpNo").prop("readonly") == false && $("#EmpNo").val() != "") {

        call_ajax_method(
            '/Master/MAS080_GetEmpNameByEmpNo',
            { empNo: $("#EmpNo").val() },
            function (result, controls) {
                $("#EmpFirstNameEN").val(result);
            }
        );

    }
    else if ($("#EmpNo").val() == "") {
        $("#EmpFirstNameEN").val("");
    }



}

function changeModule_related(istab, isblur) {
    var moduleID = $("#cboModuleName").val();
    if (moduleID != "blank" || moduleID != "") {
        $("#cboModuleName option[value='blank']").remove();
        if (search_tree == null)
            initial_tree();
        
        search_tree.deleteChildItems("0");
        search_tree.enableCheckBoxes(1);
        search_tree.enableThreeStateCheckboxes(true);
        search_tree.setXMLAutoLoading(getURLForGetXML("/Master/GetFuntionXml?moduleID=" + moduleID));
        search_tree.loadXML(getURLForGetXML("/Master/GetFuntionXml?moduleID=" + moduleID));
    }

}

function changeModule_change() {
    var moduleID = $("#cboModuleName").val();
    if (moduleID != "blank") {
        if (search_tree == null)
            initial_tree();
        search_tree.deleteChildItems("0");
        search_tree.enableCheckBoxes(1);
        search_tree.enableThreeStateCheckboxes(true);
        search_tree.setXMLAutoLoading(getURLForGetXML("/Master/GetFuntionXml?moduleID=" + moduleID));
        search_tree.loadXML(getURLForGetXML("/Master/GetFuntionXml?moduleID=" + moduleID));
    }
}

function changeModule() {
    var moduleID = $("#cboModuleName").val();
    if (moduleID != "blank") {
        if (search_tree == null)
            initial_tree();
        search_tree.deleteChildItems("0");
        search_tree.enableCheckBoxes(1);
        search_tree.enableThreeStateCheckboxes(true);
        search_tree.setXMLAutoLoading(getURLForGetXML("/Master/GetFuntionXml?moduleID=" + moduleID));
        search_tree.loadXML(getURLForGetXML("/Master/GetFuntionXml?moduleID=" + moduleID), function () { $("#btnClear").attr("disabled", false); });
    }
}

function confirmCancel() {
    var msgprm = { "module": "Common", "code": "MSG0140" }; // param: c_cancel };
    call_ajax_method("/Shared/GetMessage", msgprm, function (data, controls) {
        OpenYesNoMessageDialog(data.Code, data.Message, function () {

            // tt
            mygrid_enable = true;
            enable_grid_button(mygrid, "Edit", btn_permission_detail, true);


            cancelOperation();
        });
    });
}

// TO CONTROLLER ####################################################################################################################################

function search() {



    cancelOperation();

    // disable search button
    $("#btnSearch").attr("disabled", true);
    master_event.LockWindow(true);

    if ($("#grid_result").length > 0) {

        var parameter = {
            TypeOffice: $("#chkPermissionOffice").prop("checked"),
            TypeIndividual: $("#chkPermissionIndividual").prop("checked"),
            PermissionGroupName: $("#txtPermissionGroupName").val(),
            OfficeCode: $("#cboUseInOffice").val(),
            DepartmentCode: $("#cboUseInDepartment").val(),
            PositionCode: $("#cboUseInPosition").val(),
            ObjectFunction: search_tree.getAllChecked(),
            EmpNo: $("#txtUseInEmployeeCode").val()
        };

        $("#grid_result").LoadDataToGrid(mygrid, ROWS_PER_PAGE_FOR_VIEWPAGE, false, "/Master/MAS080_SearchPermission", parameter, "dtPermissionHeader", false,
            function () {
                // enable search button
                $("#btnSearch").attr("disabled", false);
                master_event.LockWindow(false);

                // Slide search result to top
                //document.getElementById("Search_Result").scrollIntoView();
                master_event.ScrollWindow("#Search_Result");

                set_tree.deleteChildItems("0");
                set_tree.enableCheckBoxes(1);
                set_tree.enableThreeStateCheckboxes(true);
                set_tree.setXMLAutoLoading(getURLForGetXML("/Master/GetFuntionXml"));
                set_tree.loadXML(getURLForGetXML("/Master/GetFuntionXml"));
            }
            ,
            function (result, controls, isWarning) {
                if (isWarning == undefined) {
                    $("#Search_Result").show();

                    // Scroll to search result
                    //$.scrollTo('#Search_Result', { speed: 2500 });
                }
            }
        );
    }
}

function getFunctionDetail(crPermissionGroupCode, crPermissionIndividualCode, isIndiviual) {
//    *****
//    set_tree.deleteChildItems("0");
//    set_tree.enableCheckBoxes(1);
//    set_tree.enableThreeStateCheckboxes(true);
//    set_tree.setXMLAutoLoading(getURLForGetXML("/Master/GetFuntionXml"));
//    set_tree.loadXML(getURLForGetXML("/Master/GetFuntionXml"));
//    *****

    if (isIndiviual) {
        loadEmployeeGrid(crPermissionGroupCode, crPermissionIndividualCode);
        call_ajax_method(
            '/Master/MAS080_GetFunction',
            { permissionGroupCode: crPermissionGroupCode, permissionIndividualCode: crPermissionIndividualCode },
            function (data, controls) {
                $.each(data, function (index, row) {
                    set_tree.setCheck(row.SelectedID, true);
                });
                setControlForEditPermission();

                $("#btnNewIndividualPermission").attr("disabled", true);
                if (permission_screen.EDIT == "False") {
                    disablePermissionDetail();
                } else {
                    disablePermissionOfCurrentPermissionGroup(crPermissionGroupCode);
                }

                $("#Employee_Info").show();
                $("#Set_Permission_Info").show();

                setEnablePermissionGrid(false);
                disableSearch();
                detailMode = true;
            }
        );
    } else {
        call_ajax_method(
            '/Master/MAS080_GetFunction',
            { permissionGroupCode: crPermissionGroupCode, permissionIndividualCode: crPermissionIndividualCode },
            function (data, controls) {
                $.each(data, function (index, row) {
                    set_tree.setCheck(row.SelectedID, true);
                });
                setControlForEditPermission();

                $("#btnNewIndividualPermission").attr("disabled", false);
                if (permission_screen.EDIT == "False") {
                    disablePermissionDetail();
                }

                $("#Employee_Info").hide();
                $("#Set_Permission_Info").show();

                setEnablePermissionGrid(false);
                disableSearch();
                detailMode = true;
            }
        );
    }
}

function disablePermissionOfCurrentPermissionGroup(crPermissionGroupCode) {
    call_ajax_method(
        '/Master/MAS080_GetFunction',
        { permissionGroupCode: crPermissionGroupCode, permissionIndividualCode: "" },
        function (data, controls) {
            $.each(data, function (index, row) {
                var objID = set_tree.getParentId(row.SelectedID);
                var moduleID = set_tree.getParentId(objID);

                set_tree.setCheck(row.SelectedID, 1);
                //set_tree.setCheck(objID, 1);
                //set_tree.setCheck(moduleID, 1);

                set_tree.disableCheckbox(row.SelectedID, 1);
                set_tree.disableCheckbox(objID, 1);
                set_tree.disableCheckbox(moduleID, 1);
            });
        }
    );
}

function enablePermissionDetailTree() {
    var allId = set_tree.getAllChildless().split(",");
    var i = 0;
    for (i = 0; i < allId.length; i++) {
        set_tree.disableCheckbox(allId[i], 0);
    }

    allId = set_tree.getAllItemsWithKids().split(",");
    i = 0;
    for (i = 0; i < allId.length; i++) {
        set_tree.disableCheckbox(allId[i], 0);
    }
}

function setControlForEditPermission() {
    $("#Maintain_Permission_Group").show();
    $("#Permission_Group_Info").show();
    $("#Office_Department_Info").show();
    SetCancelCommand(true, confirmCancel);
    if (permission_screen.EDIT == "True" || permission_screen.DELETE == "True") {
        SetConfirmCommand(true, confirmPress);
    } else {
        SetConfirmCommand(false, confirmPress);
    }

    $("#PermissionGroupCode").val(crPermissionGroupCode);
    $("#PermissionType").val(PermissionType);
    $("#PermissionGroupName").val(PermissionGroupName);
    $("#OfficeCode").val(crOfficeCode);
    $("#DepartmentCode").val(crDepartmentCode);
    $("#PositionCode").val(crPositionCode);
    $("#PermissionIndividualName").val(crPermissionIndividualName);

    if (permission_screen.DELETE == "True") {
        $("#DeletePermission").attr("disabled", false);
    }
    $("#delete_permission_ctrl").show();
    $("#PermissionGroupCode").attr("readonly", true);
    $("#PermissionGroupName").attr("readonly", true);
    $("#PermissionType").attr("readonly", true);
    $("#MAS080_Office_Info select").attr("disabled", true);
}

function loadEmployeeGrid(crPermissionGroupCode, crPermissionIndividualCode) {
    if ($("#employee_grid").length > 0) {

        var parameter = { permissionGroupCode: crPermissionGroupCode, permissionIndividualCode: crPermissionIndividualCode };

        $("#employee_grid").LoadDataToGrid(mygrid2, 0, false, "/Master/MAS080_GetEmpNo", parameter, "dtEmpNo", false);

    }
}

function clear() {
    $("#btnClear").attr("disabled", true);
    $("#Search_Result").hide();
    $("#Maintain_Permission_Group").hide();
    $("#Employee_Info").hide();
    $("#MAS080_Search").clearForm();
    $("#txtUseInEmployeeCode").attr("readonly", false);
    $("#chkPermissionOffice").attr("checked", true);
    $("#chkPermissionIndividual").attr("checked", true);

    SetCancelCommand(false, confirmCancel);
    SetConfirmCommand(false, confirmPress);

    var allId = search_tree.getAllChecked().split(",");
    var i = 0;
    for (i = 0; i < allId.length; i++) {
        search_tree.setCheck(allId[i], false);
    }

    allId = search_tree.getAllItemsWithKids().split(",");
    var i = 0;
    for (i = 0; i < allId.length; i++) {
        search_tree.closeAllItems(allId[i]);
    }

    allId = set_tree.getAllItemsWithKids().split(",");
    var i = 0;
    for (i = 0; i < allId.length; i++) {
        set_tree.closeAllItems(allId[i]);
    }

    changeModule();
    document.getElementById("Search_Permission_Group").scrollIntoView();
}

function newPermission() {
    newMode = true;
    isIndiviual = false;

    $("#Maintain_Permission_Group").show();
    $("#Permission_Group_Info").show();
    $("#Office_Department_Info").show();

    $("#MAS080_PermissionInfo").clearForm();
    $("#MAS080_Office_Info").clearForm();
    $("#MAS080_Employee_Info").clearForm();
    $("#MAS080_SetPermission").clearForm();

    $("#delete_permission_ctrl").hide();

    SetCancelCommand(true, confirmCancel);
    SetConfirmCommand(true, confirmPress);

    //$("#btnNewIndividualPermission").attr("disabled", true);
    $("#MAS080_Office_Info select").attr("disabled", false);
    enablePermissionDetail();

    $("#Employee_Info").hide();
    $("#Set_Permission_Info").show();

    set_tree.deleteChildItems("0");
    set_tree.enableCheckBoxes(1);
    set_tree.enableThreeStateCheckboxes(true);
    set_tree.setXMLAutoLoading(getURLForGetXML("/Master/GetFuntionXml"));
    set_tree.loadXML(getURLForGetXML("/Master/GetFuntionXml"));

//    var allId = set_tree.getAllChecked().split(",");
//    var i = 0;
//    for (i = 0; i < allId.length; i++) {
//        set_tree.setCheck(allId[i], false);
//    }
    setEnablePermissionGrid(false);
    disableSearch();

    $("#PermissionType").val("Office/Department");


    // tt
    mygrid_enable = false;
    enable_grid_button(mygrid, "Edit", btn_permission_detail, false);

    document.getElementById("Maintain_Permission_Group").scrollIntoView();


}

function cancelOperation() {
    newMode = false;
    detailMode = false;
    newIndividual = false;

    crPermissionGroupCode = null;
    crOfficeCode = null;
    crDepartmentCode = null;
    crPositionCode = null;
    crPermissionIndividualCode = null;
    crPermissionIndividualName = null;

    $("#Maintain_Permission_Group").hide();
    $("#EmpNo").val("");
    $("#EmpFirstNameEN").val("");
    SetCancelCommand(false, confirmCancel);
    SetConfirmCommand(false, confirmPress);

    $("#DeletePermission").attr("checked", false);
    toggleDeletePermission();
    setEnablePermissionGrid(true);
    if (permission_screen.EDIT == "True") {
        setEnableEmployeeGrid(true);
    } else {
        setEnableEmployeeGrid(false);
    }

    enableSearch();

    var allId = set_tree.getAllChecked().split(",");
    var i = 0;
    for (i = 0; i < allId.length; i++) {
        set_tree.setCheck(allId[i], false);
    }
    enablePermissionDetailTree();
}

function newIndividualPermission() {
    delEmp = [];
    addEmp = [];

    $("#MAS080_Office_Info select").attr("disabled", true);
    $("#btnNewIndividualPermission").attr("disabled", true);

    $("#Employee_Info").show();
    // Akat K. new individual must enable employee grid
    if (permission_screen.EDIT == "True") {
        setEnableEmployeeGrid(true);
    } else {
        setEnableEmployeeGrid(false);
    }

    disablePermissionOfCurrentPermissionGroup(crPermissionGroupCode);

    $("#PermissionIndividualName").val("");
    DeleteAllRow(mygrid2);
    newIndividual = true;
}

function addEmpToGrid() {
    var employeeCode = $("#EmpNo").val();
    var employeeName = $("#EmpFirstNameEN").val();

    if (employeeCode == undefined || employeeCode == "" || employeeName == undefined || employeeName == "") {
        return;
    }

    //    for (var i = 0; i < mygrid2.getRowsNum(); i++) {
    //        if (mygrid2.cells2(i, mygrid2.getColIndexById('EmpNo')).getValue() == employeeCode
    //            && mygrid2.cells2(i, mygrid2.getColIndexById('ModifyMode')).getValue() != "DEL") {
    //            var param = { "module": "Master", "code": "MSG1025" };
    //            call_ajax_method("/Shared/GetMessage", param, function (data, controls) {
    //                OpenWarningDialog(data.Message);
    //                //OpenErrorMessageDialog(data.code, data.Message);
    //            });
    //            return;
    //        }
    //    }

    var vv = new Array();
    if (CheckFirstRowIsEmpty(mygrid2) == false) {
        var codeCol = mygrid2.getColIndexById("EmpNo");
        var modeCol = mygrid2.getColIndexById("ModifyMode");

        for (var i = 0; i < mygrid2.getRowsNum(); i++) {
            var row_id = mygrid2.getRowId(i);

            var obj = {
                EmpNo: mygrid2.cells2(i, codeCol).getValue(),
                ModifyMode: mygrid2.cells2(i, modeCol).getValue()
            };
            vv.push(obj);
        }
    }

    call_ajax_method(
        '/Master/MAS080_CheckExistEmpNo',
        { current: vv, officeCode: $("#OfficeCode").val(), departmentCode: $("#DepartmentCode").val(), positionCode: $("#PositionCode").val(), empNo: employeeCode },
        function (data, controls) {
            if (data != undefined) {
                CheckFirstRowIsEmpty(mygrid2, true);

                AddNewRow(mygrid2, [
                        employeeCode
                        , employeeName
                        , ""
                        , "ADD"
                    ]);

                var row_id = mygrid2.getRowId(mygrid2.getRowsNum() - 1);
                GenerateRemoveButton(mygrid2, btn_employee_remove, row_id, "Remove", true);
                BindGridButtonClickEvent(btn_employee_remove, row_id, removeEmployee);

                mygrid2.setSizes();
                $("#EmpNo").val("");
                $("#EmpFirstNameEN").val("");

                if (addEmp == undefined) {
                    addEmp = [];
                }

                var found = false;
                if (norEmp != undefined) {
                    for (var idx = 0; idx < norEmp.length; idx++) {
                        if (norEmp[idx] == employeeCode) {
                            found = true;
                            break;
                        }
                    }
                }
                if (found == false) {
                    addEmp.push(employeeCode);
                }

                if (delEmp != undefined) {
                    for (var idx = 0; idx < delEmp.length; idx++) {
                        if (delEmp[idx] == employeeCode) {
                            delEmp.splice(idx, 1);
                            break;
                        }
                    }
                }
            }
        }
    );
}

function confirmPress() {

    // disable confrim button
    DisableConfirmCommand(true);


    var param = collectInformation();

    if (newMode) {
        callAjaxSave(param, '/Master/MAS080_InsertPermissionOffice');
    } else if (detailMode) {
        if (newIndividual) {
            call_ajax_method("/Master/MAS080_ValidateTypeInidividual", param, function (data, controls) {

                // enable confrim button
                DisableConfirmCommand(false);

                if (controls != undefined) {
                    VaridateCtrl(["PermissionGroupName",
                                "OfficeCode",
                                "DepartmentCode",
                                "PositionCode",
                                "PermissionIndividualName",
                                "EmpNo"], controls);
                } else {
                    if (empCount == 0) {
                        var msgprm = { "module": "Master", "code": "MSG1044" };
                        call_ajax_method("/Shared/GetMessage", msgprm, function (data, controls) {
                            OpenWarningDialog(data.Message);
                        });
                        return;
                    }

                    callAjaxSaveInsertIndividual(param, '/Master/MAS080_InsertPermissionIndividual');
                }
            });
        } else {

            // enable confrim button
            DisableConfirmCommand(false);

            //if ($("#chkPermissionIndividual").prop("checked")) {
            if (isIndiviual) {
                if ($("#DeletePermission").prop("checked")) {
                    var msgprm = { "module": "Master", "code": "MSG1017", param: $("#PermissionIndividualName").val() };
                    call_ajax_method("/Shared/GetMessage", msgprm, function (data, ctrl) {
                        OpenYesNoMessageDialog(data.Code, data.Message, function () {
                            deletePermissionTypeIndividual(param);
                        });
                    });
                } else {
                call_ajax_method("/Master/MAS080_ValidateTypeInidividual", param, function (data, controls) {
                    if (controls != undefined) {
                        VaridateCtrl(["PermissionGroupName",
                                "OfficeCode",
                                "DepartmentCode",
                                "PositionCode",
                                "PermissionIndividualName",
                                "EmpNo"], controls);
                    } else {
                        var cc = 0;
                        if (CheckFirstRowIsEmpty(mygrid2) == false) {
                            cc = mygrid2.getRowsNum();
                        }
                        if (empCount == 0 && cc == 0) {
                            var msgprm = { "module": "Master", "code": "MSG1044" };
                            call_ajax_method("/Shared/GetMessage", msgprm, function (data, controls) {
                                OpenWarningDialog(data.Message);
                            });
                            return;
                        }

                        callAjaxSave(param, '/Master/MAS080_EditPermissionTypeIndividual');
                    }
                });
                }
            } else {
                if ($("#DeletePermission").prop("checked")) {
                    var msgprm = { "module": "Master", "code": "MSG1016", param: $("#PermissionGroupName").val() };
                    call_ajax_method("/Shared/GetMessage", msgprm, function (data, ctrl) {
                        OpenYesNoMessageDialog(data.Code, data.Message, function () {
                            deletePermissionTypeOffice(param);
                        });
                    });
                } else {
                    callAjaxSave(param, '/Master/MAS080_EditPermissionTypeOffice');
                }
            }
        }
    }

    // enable confrim button
    DisableConfirmCommand(false);
}

function collectInformation() {
    var empNo = "";
    //if ($("#chkPermissionIndividual").prop("checked") || newIndividual) {
    if (isIndiviual || newIndividual) {
//        empCount = 0;
//        for (var i = 0; i < mygrid2.getRowsNum(); i++) {
//            if (mygrid2.cells2(i, mygrid2.getColIndexById('ModifyMode')).getValue() == "NONE"
//                || mygrid2.cells2(i, mygrid2.getColIndexById('ModifyMode')).getValue() == "ADD") {
//                empCount++;
//            }

//            if (mygrid2.cells2(i, mygrid2.getColIndexById('ModifyMode')).getValue() == "DEL" ||
//                mygrid2.cells2(i, mygrid2.getColIndexById('ModifyMode')).getValue() == "ADD") {
//                empNo += "," + mygrid2.cells2(i, mygrid2.getColIndexById('EmpNo')).getValue() +
//                            ":" + mygrid2.cells2(i, mygrid2.getColIndexById('ModifyMode')).getValue();
//            }
//        }
        if (addEmp != undefined)
        {
            for(var idx = 0; idx < addEmp.length; idx++)
            {
                empNo += "," + addEmp[idx] + ":" + "ADD";
                empCount++;
            }
        }
        if (delEmp != undefined)
        {
            for(var idx = 0; idx < delEmp.length; idx++)
            {
                empNo += "," + delEmp[idx] + ":" + "DEL";
                empCount++;
            }
        }
    }

    var permission = {
        TypeOffice: $("#chkPermissionOffice").prop("checked"),
        TypeIndividual: $("#chkPermissionIndividual").prop("checked"),
        PermissionGroupCode: crPermissionGroupCode,
        PermissionGroupName: $("#PermissionGroupName").val(),
        PermissionIndividualCode: crPermissionIndividualCode,
        PermissionIndividualName: $("#PermissionIndividualName").val(),
        OfficeCode: $("#OfficeCode").val(),
        DepartmentCode: $("#DepartmentCode").val(),
        PositionCode: $("#PositionCode").val(),
        UpdateDate: UpdateDate,
        EmpNo: empNo,
        ObjectFunction: set_tree.getAllChecked()
    };

    return permission; // return JSON.stringify(permission);
}

function callAjaxSave(param, url) {
    call_ajax_method_json(
        url,
        param,
        function (result, controls) {
            if (controls != undefined) {
                VaridateCtrl(["PermissionGroupName",
                                "OfficeCode",
                                "DepartmentCode",
                                "PositionCode"], controls);
            }

            if (result != undefined) {
                var param = { "module": "Common", "code": "MSG0046" };
                call_ajax_method("/Shared/GetMessage", param, function (data, ctrl) {
                    OpenInformationMessageDialog(data.code, data.Message);
                });
                newMode = false;
                detailMode = false;
                newIndividual = false;
                setEnablePermissionGrid(true);
                if (permission_screen.EDIT == "True") {
                    setEnableEmployeeGrid(true);
                } else {
                    setEnableEmployeeGrid(false);
                }

                enablePermissionDetailTree();
                enableSearch();
                clear();

                // tt
                mygrid_enable = true;
                enable_grid_button(mygrid, "Edit", btn_permission_detail, true);

            }
        }
    );
}

function callAjaxSaveInsertIndividual(param, url) {
    call_ajax_method_json(
        url,
        param,
        function (result, controls) {
            if (result != undefined) {
                var param = { "module": "Common", "code": "MSG0046" };
                call_ajax_method("/Shared/GetMessage", param, function (data, ctrl) {
                    OpenInformationMessageDialog(data.code, data.Message);
                });
                newMode = false;
                detailMode = false;
                newIndividual = false;
                setEnablePermissionGrid(true);
                if (permission_screen.EDIT == "True") {
                    setEnableEmployeeGrid(true);
                } else {
                    setEnableEmployeeGrid(false);
                }

                enablePermissionDetailTree();
                enableSearch();
                clear();

                // tt
                mygrid_enable = true;
                enable_grid_button(mygrid, "Edit", btn_permission_detail, true);

            }
        }
    );
}

function deletePermissionTypeOffice() {
    call_ajax_method(
        '/Master/MAS080_DeletePermissionTypeOffice',
        { permissionGroupCode: crPermissionGroupCode },
        function (data, ctrl) {
            if (data != undefined) {
                var param = { "module": "Common", "code": "MSG0047" };
                call_ajax_method("/Shared/GetMessage", param, function (data, ctrl) {
                    OpenInformationMessageDialog(data.code, data.Message);

                    newMode = false;
                    detailMode = false;
                    newIndividual = false;
                    setEnablePermissionGrid(true);
                    if (permission_screen.EDIT == "True") {
                        setEnableEmployeeGrid(true);
                    } else {
                        setEnableEmployeeGrid(false);
                    }

                    enableSearch();
                    clear();

                    // tt
                    mygrid_enable = true;
                    enable_grid_button(mygrid, "Edit", btn_permission_detail, true);
                });
            }
        }
    );
}

function deletePermissionTypeIndividual() {
    call_ajax_method(
        '/Master/MAS080_DeletePermissionTypeInidividual',
        { permissionGroupCode: crPermissionGroupCode, permissionIndividualCode: crPermissionIndividualCode },
        function (data, ctrl) {
            if (data != undefined) {
                var param = { "module": "Common", "code": "MSG0047" };
                call_ajax_method("/Shared/GetMessage", param, function (data, ctrl) {
                    OpenInformationMessageDialog(data.code, data.Message);

                    newMode = false;
                    detailMode = false;
                    newIndividual = false;
                    setEnablePermissionGrid(true);
                    if (permission_screen.EDIT == "True") {
                        setEnableEmployeeGrid(true);
                    } else {
                        setEnableEmployeeGrid(false);
                    }

                    enableSearch();
                    clear();

                    // tt
                    mygrid_enable = true;
                    enable_grid_button(mygrid, "Edit", btn_permission_detail, true);
                });
            }
        }
    );
}
