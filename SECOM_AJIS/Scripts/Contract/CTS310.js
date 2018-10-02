/// <reference path = "../../Scripts/jquery-1.5.1-vsdoc.js" />
/// <reference path = "../../Scripts/Base/GridControl.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxcommon.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxgrid.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxgridcell.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/ext/dhtmlxgrid_pgn.js" />

var mygrid;

$(document).ready(function () {
    InitialDateFromToControl("#datePeriodFrom", "#datePeriodTo");

    $("#btnSearchByRole").click(clickSearchByRole);
    $("#btnClearByRole").click(clickClearByRole);
    $("#btnSearchIncident").click(clickSearchIncident);
    $("#btnClearIncident").click(clickClearIncident);

    //$("#chkIncidentStatusHandling").click(IncidentStatusHandling_click);
    //$("#chkIncidentStatusCompleted").click(IncidentStatusCompleted_click);

    $("#chkIncidentStatusHandling").val(c_handling);
    $("#chkIncidentStatusCompleted").val(c_complete);

    $("#txtContractTarget").InitialAutoComplete("/Master/GetCustName");
    $("#txtCustomerName").InitialAutoComplete("/Master/GetCustName");
    $("#txtCustomerGroup").InitialAutoComplete("/Master/GetGroupName");
    $("#txtSiteName").InitialAutoComplete("/Master/GetSiteName");
    $("#txtRegistrant").InitialAutoComplete("/Master/MAS070_GetEmployeeName");
    $("#txtControlChief").InitialAutoComplete("/Master/MAS070_GetEmployeeName");
    $("#txtChief").InitialAutoComplete("/Master/MAS070_GetEmployeeName");
    $("#txtCorrespondent").InitialAutoComplete("/Master/MAS070_GetEmployeeName");
    $("#txtAssistant").InitialAutoComplete("/Master/MAS070_GetEmployeeName");

    innitialGrid();

    $("#ContractStatusAll").attr("checked", "checked");
    $("#ContractTypeAll").attr("checked", "checked");

//    $("#ContractStatusAll").click(ContractStatusAllClick);
//    $("#ContractStatusUnimplemented").click(ContractStatusUnimplementedClick);
//    $("#ContractStatusImplemented").click(ContractStatusImplementedClick);
//    $("#ContractStatusStopService").click(ContractStatusStopServiceClick);
//    $("#ContractStatusCancelFinish").click(ContractStatusCancelFinishClick);

//    $("#ContractTypeAll").click(ContractTypeAllClick);
//    $("#ContractTypeAL").click(ContractTypeALClick);
//    $("#ContractTypeSales").click(ContractTypeSalesClick);
//    $("#ContractTypeMaintenance").click(ContractTypeMaintenanceClick);
//    $("#ContractTypeSentryGuard").click(ContractTypeSentryGuardClick);

    $("#Search_Result").hide();


    if (screen_mode == "Correspondent" || screen_mode == "Chief" || screen_mode == "Admin") {
        $("#cboIncidentRole").val(default_role);
        $("#cboDueDate").val(default_duedate);
        $("#cboIncidentStatus").val(default_status);

        if (screen_mode == "Admin") {
            $('#cboIncidentRole >option').remove();

            var parameter = {}
            call_ajax_method("/Contract/GetAdminCombo", parameter, function (data, ctrl) {
                regenerate_combo("#cboIncidentRole", data);
            });
        }
        incidentListByRole();
    }
    if (screen_mode == "Office") {
        $("#cboIncidentOffice").val(incident_office_code);
        incidentListBySearch();
    }
    if (screen_mode == "Search") {
        searchIncident();
    }
});

function innitialGrid() {
    var pageRow = 20;

    mygrid = $("#grid_result").InitialGrid(pageRow, true, "/Contract/CTS310_InitGrid");

    BindOnLoadedEvent(mygrid, function (gen_ctrl) {
        var idColIndex = mygrid.getColIndexById('IncidentID');
        var overNewIndex = mygrid.getColIndexById('OverNewImportant');
        var incidentNoIndex = mygrid.getColIndexById('IncidentNo');
        var corrIndex = mygrid.getColIndexById('CorrespondentDisplay');
        var statusIndex = mygrid.getColIndexById('IncidentStatusNameDisplay');

        if (mygrid.getRowsNum() != 0) {
            for (var i = 0; i < mygrid.getRowsNum(); i++) {
                var row_id = mygrid.getRowId(i);
                

                // Over!
                if (mygrid.cells(row_id, overNewIndex).getValue() == "o" || mygrid.cells(row_id, overNewIndex).getValue() == c_over) {
                    mygrid.setRowColor(row_id, "#ff9999");
                    //mygrid.setCellTextStyle(row_id, overNewIndex, "background-color: red; color: white; font-weight: bold;");
                } else {
                    //mygrid.setCellTextStyle(row_id, overNewIndex, "background-color: yellow; color: red; font-weight: bold;");
                    //mygrid.setRowColor(row_id, "#ffffff");
//                    mygrid.setCellTextStyle(row_id, overNewIndex, "font-weight: bold;");
                }

                // Correspondent
                var celldata = mygrid.cells(row_id, corrIndex).getValue();
                if (!celldata || celldata.toString()._dhx_trim() == "") {
                    celldata = "n ";
                }

                var cellPrefix = celldata.substr(0, 1);
                var cellData = celldata.substr(1, celldata.length - 1);

                if (cellPrefix == "y") {
                    mygrid.cells(row_id, corrIndex).setValue(cellData);
                    mygrid.setCellTextStyle(row_id, corrIndex, "background-color: yellow");
                } else if (cellPrefix == "n") {
                    mygrid.cells(row_id, corrIndex).setValue(cellData);
                }

                // Status
                celldata = mygrid.cells(row_id, statusIndex).getValue();
                if (!celldata || celldata.toString()._dhx_trim() == "") {
                    celldata = "n ";
                }

                cellPrefix = celldata.substr(0, 1);
                cellData = celldata.substr(1, celldata.length - 1);

                if (cellPrefix == "y") {
                    mygrid.cells(row_id, statusIndex).setValue(cellData);
                    mygrid.setCellTextStyle(row_id, statusIndex, "background-color: yellow");
                } else if (cellPrefix == "n") {
                    mygrid.cells(row_id, statusIndex).setValue(cellData);
                }

                if (gen_ctrl == true) {
                    var incidentID = mygrid.cells(row_id, idColIndex).getValue();
                    var incidentNo = mygrid.cells(row_id, incidentNoIndex).getValue();
                    var tagA = "<a href='#'>" + incidentNo + "<input type='hidden' name='incidentID' value='" + incidentID + "'/></a>";
                    mygrid.cells(row_id, incidentNoIndex).setValue(tagA);
                }
            }

            //if (gen_ctrl == true) {  //Comment by Jutarat A. on 21082012
            $("#grid_result a").each(function () {
                $(this).unbind("click"); //Add by Jutarat A. on 27082012
                $(this).click(function () {
                    var obj = {
                        strIncidentID: $(this).children("input:hidden[name=incidentID]").val()
                    };
                    ajax_method.CallScreenControllerWithAuthority("/Contract/CTS330", obj, true);
                });
            });
            //}

        }
    });
}

function eXcell_overNewImportantCell(cell) {
    this.base = eXcell_edn;
    this.base(cell);
    this.setValue = function (val) {
        if (!val || val.toString()._dhx_trim() == "") {
            val = "0";
        }

        if (val == "o") {
            this.cell.innerHTML = c_over;
            this.cell.style.color = "white";
            this.cell.style.backgroundColor = "red";
            this.cell.style.fontWeight = "bold";
        } else if (val == "n") {
            this.cell.innerHTML = c_new;
            this.cell.style.color = "red";
            this.cell.style.backgroundColor = "yellow";
            this.cell.style.fontWeight = "bold";
        } else if (val == "i") {
            this.cell.innerHTML = "★";
            this.cell.style.color = "white";
            this.cell.style.backgroundColor = "red";
        } else {
            this.cell.innerHTML = "";
        }
    };
}
eXcell_overNewImportantCell.prototype = new eXcell_edn;

function eXcell_ImportantCell(cell) {
    this.base = eXcell_edn;
    this.base(cell);
    this.setValue = function (val) {
        if (!val || val.toString()._dhx_trim() == "") {
            val = "0";
        }

        if (val == "i") {
            this.cell.innerHTML = "★";
            this.cell.style.color = "white";
            this.cell.style.backgroundColor = "red";
        } else {
            this.cell.innerHTML = "";
        }
    };
}
eXcell_ImportantCell.prototype = new eXcell_edn;

function incidentListByRole() {
    $("#Search_by_Role").show();
    $("#Search_by_Condition").hide();
    clickSearchByRole();
}

function incidentListBySearch() {
    $("#Search_by_Condition").show();
    $("#Search_by_Role").hide();
    clickSearchIncident();
}

function searchIncident() {
    $("#Search_by_Condition").show();
    $("#Search_by_Role").hide();
}

// COMBOBOX CLICK ###################################################################################################################################

//function ContractStatusAllClick() {
//    $("#ContractStatusUnimplemented").attr("checked", false);
//    $("#ContractStatusImplemented").attr("checked", false);
//    $("#ContractStatusStopService").attr("checked", false);
//    $("#ContractStatusCancelFinish").attr("checked", false);
//}

//function ContractStatusUnimplementedClick() {
//    $("#ContractStatusImplemented").attr("checked", false);
//    $("#ContractStatusStopService").attr("checked", false);
//    $("#ContractStatusCancelFinish").attr("checked", false);
//    $("#ContractStatusAll").attr("checked", false);

//}

//function ContractStatusImplementedClick() {
//    $("#ContractStatusUnimplemented").attr("checked", false);
//    $("#ContractStatusStopService").attr("checked", false);
//    $("#ContractStatusCancelFinish").attr("checked", false);
//    $("#ContractStatusAll").attr("checked", false);
//}

//function ContractStatusStopServiceClick() {
//    $("#ContractStatusUnimplemented").attr("checked", false);
//    $("#ContractStatusImplemented").attr("checked", false);
//    $("#ContractStatusCancelFinish").attr("checked", false);
//    $("#ContractStatusAll").attr("checked", false);
//}

//function ContractStatusCancelFinishClick() {
//    $("#ContractStatusUnimplemented").attr("checked", false);
//    $("#ContractStatusImplemented").attr("checked", false);
//    $("#ContractStatusStopService").attr("checked", false);
//    $("#ContractStatusAll").attr("checked", false);
//}

//function ContractTypeAllClick() {
//    $("#ContractTypeAL").attr("checked", false);
//    $("#ContractTypeSales").attr("checked", false);
//    $("#ContractTypeMaintenance").attr("checked", false);
//    $("#ContractTypeSentryGuard").attr("checked", false);
//}

//function ContractTypeALClick() {
//    $("#ContractTypeSales").attr("checked", false);
//    $("#ContractTypeMaintenance").attr("checked", false);
//    $("#ContractTypeSentryGuard").attr("checked", false);
//    $("#ContractTypeAll").attr("checked", false);
//}

//function ContractTypeSalesClick() {
//    $("#ContractTypeAL").attr("checked", false);
//    $("#ContractTypeMaintenance").attr("checked", false);
//    $("#ContractTypeSentryGuard").attr("checked", false);
//    $("#ContractTypeAll").attr("checked", false);
//}

//function ContractTypeMaintenanceClick() {
//    $("#ContractTypeAL").attr("checked", false);
//    $("#ContractTypeSales").attr("checked", false);
//    $("#ContractTypeSentryGuard").attr("checked", false);
//    $("#ContractTypeAll").attr("checked", false);
//}

//function ContractTypeSentryGuardClick() {
//    $("#ContractTypeAL").attr("checked", false);
//    $("#ContractTypeSales").attr("checked", false);
//    $("#ContractTypeMaintenance").attr("checked", false);
//    $("#ContractTypeAll").attr("checked", false);
//}

// SEARCH FUNCTION ##################################################################################################################################

function clickSearchByRole() {
    //Add by Jutarat A. on 20120508
    $("#btnSearchByRole").attr("disabled", true);
    master_event.LockWindow(true);

    var add_date = 0;
    if ($("#cboDueDate").val() == c_duedate_1week) {
        add_date = 7;
    } else if ($("#cboDueDate").val() == c_duedate_2week) {
        add_date = 14;
    } else if ($("#cboDueDate").val() == c_duedate_1month) {
        add_date = 30;
    } else {
        add_date = 0;
    }

    var con = {
        strIncidentRole : $("#cboIncidentRole").val(),
        strIncidentStatus : $("#cboIncidentStatus").val(),
        intAddDate : add_date
    };

    if ($("#grid_result").length > 0) {
        $("#grid_result").LoadDataToGrid(mygrid, 20, true, "/Contract/CTS310_searchByRole", con, "dtIncidentList", false, 
            function () {
                //Move by Jutarat A. on 20130326
                master_event.LockWindow(false);
                $("#btnSearchByRole").attr("disabled", false);
            }, //null,
            function (result, controls, isWarning) {
                if (isWarning == undefined) {
                    $("#Search_Result").show();
                    master_event.ScrollWindow("#Search_Result", false);  //Add by Jutarat A. on 20120508
                }

//                //Add by Jutarat A. on 20120508
//                master_event.LockWindow(false);
//                $("#btnSearchByRole").attr("disabled", false);
            }
        );
    }
}

function clickSearchIncident() {
    //Add by Jutarat A. on 20120508
    $("#btnSearchIncident").attr("disabled", true);
    master_event.LockWindow(true);

    var contract_status = ",";
    if ($("#ContractStatusUnimplemented").prop("checked")) {
        contract_status += c_unimplemented + ",";
    }
    if ($("#ContractStatusImplemented").prop("checked")) {
        contract_status += c_implemented + ",";
    }
    if ($("#ContractStatusStopService").prop("checked")) {
        contract_status += c_stopservice + ",";
    }
    if ($("#ContractStatusCancelFinish").prop("checked")) {
        contract_status += c_cancel + ",";
    }
    if (contract_status == ",") {
        contract_status = "";
    }

    var contract_type = ",";
    if ($("#ContractTypeAL").prop("checked")) {
        contract_type += c_al + ",";
    }
    if ($("#ContractTypeSales").prop("checked")) {
        contract_type += c_sales + ",";
    }
    if ($("#ContractTypeMaintenance").prop("checked")) {
        contract_type += c_maintenance + ",";
    }
    if ($("#ContractTypeSentryGuard").prop("checked")) {
        contract_type += c_sentryguard + ",";
    }
    if (contract_type == ",") {
        contract_type = "";
    }

    var con = {
        ContractTargetPurchaserName : $("#txtContractTarget").val(),
        ContractCode : $("#txtUserCode").val(),
        UserCode : $("#txtUserCode").val(),
        ContractOfficeCode : $("#ContractOffice").val(),
        OperationOfficeCode : $("#OperationOffice").val(),
        ContractStatus : contract_status,
        ContractType : contract_type,
        CustomerName : $("#txtCustomerName").val(),
        CustomerGroupName : $("#txtCustomerGroup").val(),
        SiteName : $("#txtSiteName").val(),
        ProjectName : $("#txtProjectName").val(),
        IncidentNo : $("#txtIncidentNo").val(),
        IncidentTitle : $("#txtIncidentTitle").val(),
        IncidentType: $("#cboIncidentType").val(),
        IncidentStatusHandling : $("#chkIncidentStatusHandling").prop("checked") ? c_handling : "",
        IncidentStatusComplete : $("#chkIncidentStatusCompleted").prop("checked") ? c_complete : "",
        IncidentOfficeCode : $("#cboIncidentOffice").val(),
        SpecfyPeriod : $("#cboSpecifiedPeriod").val(),
        SpecifyPeriodFrom: $("#datePeriodFrom").val(),
        SpecifyPeriodTo: $("#datePeriodTo").val(),
        Registrant : $("#txtRegistrant").val(),
        Correspondent: $("#txtCorrespondent").val(),
        ControlChief: $("#txtControlChief").val(),
        Chief : $("#txtChief").val(),
        Assistant : $("#txtAssistant").val()
    }


    call_ajax_method_json(
        '/Contract/CTS310_ValidateSearch',
        con,
        function (result, controls) {
            if (controls != undefined) {
                VaridateCtrl(["cboSpecifiedPeriod",
                                "datePeriodFrom",
                                "datePeriodTo"], controls);

                //Add by Jutarat A. on 20120508
                master_event.LockWindow(false);
                $("#btnSearchIncident").attr("disabled", false);

            } else if (result != undefined) {
                if ($("#grid_result").length > 0) {
                    $("#Search_Result").show();
                    $("#grid_result").LoadDataToGrid(mygrid, 20, true, "/Contract/CTS310_searchIncidentList", con, "dtIncidentList", false, 
                        function () {
                            //Move by Jutarat A. on 20130326
                            master_event.LockWindow(false);
                            $("#btnSearchIncident").attr("disabled", false);
                        }, //null,
                        function (result, controls, isWarning) {
                            if (isWarning == undefined) {
                                $("#Search_Result").show();
                                master_event.ScrollWindow("#Search_Result", false);  //Add by Jutarat A. on 20120508
                            }

//                            //Add by Jutarat A. on 20120508
//                            master_event.LockWindow(false);
//                            $("#btnSearchIncident").attr("disabled", false);
                        }
                    );
                }
            }
            else {
                //Add by Jutarat A. on 20130326
                master_event.LockWindow(false);
                $("#btnSearchIncident").attr("disabled", false);
            }
        }
    );
}

function clickClearByRole() {
    $("#cboIncidentRole").val(default_role);
    $("#cboDueDate").val(default_duedate);
    $("#cboIncidentStatus").val(default_status);
    $("#Search_Result").hide();
    DeleteAllRow(mygrid);
}

function clickClearIncident() {
    $("#CTS310_SearchIncident").clearForm();
    ClearDateFromToControl("#datePeriodFrom", "#datePeriodTo");
    $("#ContractStatusAll").attr("checked", "checked");
    $("#ContractTypeAll").attr("checked", "checked");
    $("#Search_Result").hide();
    DeleteAllRow(mygrid);
}

//function IncidentStatusHandling_click() {
//    $("#chkIncidentStatusCompleted").attr("checked", false);
//}

//function IncidentStatusCompleted_click() {
//    $("#chkIncidentStatusHandling").attr("checked", false);
//}
