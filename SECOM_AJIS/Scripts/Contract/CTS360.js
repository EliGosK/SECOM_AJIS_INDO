/// <reference path = "../../Scripts/jquery-1.5.1-vsdoc.js" />
/// <reference path = "../../Scripts/Base/GridControl.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxcommon.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxgrid.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxgridcell.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/ext/dhtmlxgrid_pgn.js" />

var cts360_grid;

$(document).ready(function () {
    InitialDateFromToControl("#dateByRolePeriodFrom", "#dateByRolePeriodTo");
    InitialDateFromToControl("#dateSearchPeriodFrom", "#dateSearchPeriodTo");

    InitialTrimTextEvent(["txtApproveNo"]);

//    $("#dateByRolePeriodFrom").val(c_datefrom);
//    $("#dateSearchPeriodFrom").val(c_datefrom);
//    $("#dateByRolePeriodTo").val(c_dateto);
//    $("#dateSearchPeriodTo").val(c_dateto);

    $("#btnSearchByRole").click(clickSearchByRole);
    $("#btnClearByRole").click(clickClearByRole);
    $("#btnSearchAR").click(clickSearchAR);
    $("#btnClearAR").click(clickClearAR);

    innitialGrid();

    $("#ContractStatusAll").attr("checked", "checked");
    $("#ContractTypeAll").attr("checked", "checked");

//    $("#chkARStatusHandling").click(ARStatusHandling_click);
//    $("#chkARStatusCompleted").click(ARStatusCompleted_click);

    $("#txtCustomerGroup").InitialAutoComplete("/Master/GetGroupName");
    $("#txtCustomerName").InitialAutoComplete("/Master/GetCustName");
    $("#txtContractPurchaserName").InitialAutoComplete("/Master/GetCustName");
    $("#txtSiteName").InitialAutoComplete("/Master/GetSiteName");
    $("#txtRequester").InitialAutoComplete("/Master/MAS070_GetEmployeeName");
    $("#txtApprover").InitialAutoComplete("/Master/MAS070_GetEmployeeName");
    $("#txtAuditor").InitialAutoComplete("/Master/MAS070_GetEmployeeName");

//    $("#ContractStatusUnimplemented").click(ContractStatusUnimplementedClick);
//    $("#ContractStatusImplemented").click(ContractStatusImplementedClick);
//    $("#ContractStatusStopService").click(ContractStatusStopServiceClick);
//    $("#ContractStatusCancelFinish").click(ContractStatusCancelFinishClick);
//    $("#ContractStatusAll").click(ContractStatusAllClick);

//    $("#ContractTypeAL").click(ContractTypeALClick);
//    $("#ContractTypeSales").click(ContractTypeSalesClick);
//    $("#ContractTypeMaintenance").click(ContractTypeMaintenanceClick);
//    $("#ContractTypeSentryGuard").click(ContractTypeSentryGuardClick);
//    $("#ContractTypeAll").click(ContractTypeAllClick);

    $("#Search_Result").hide();

    if (screen_mode == "Requester" || screen_mode == "Approver" || screen_mode == "Auditor") {
        $("#cboARRole").val(default_role);
        $("#cboARStatus").val(default_status);
        $("#cboARSpecifyPeriod").val(default_period);
        SetDateFromToData("#dateByRolePeriodFrom", "#dateByRolePeriodTo", c_datefrom, c_dateto)
        ARListByRole();
    }
    if (screen_mode == "Office") {
        $("#cboAROffice").val(ar_office_code);
        //SetDateFromToData("#dateSearchPeriodFrom", "#dateSearchPeriodTo", c_datefrom, c_dateto)
        ARListBySearch();
    }
    if (screen_mode == "Search") {
        searchAR();
    }
});

function innitialGrid() {
    var pageRow = 20;

    cts360_grid = $("#ar_result_list").InitialGrid(pageRow, true, "/Contract/CTS360_InitGrid");

    BindOnLoadedEvent(cts360_grid, function (gen_ctrl) {
        var idColIndex = cts360_grid.getColIndexById('RequestNo');
        var overNewIndex = cts360_grid.getColIndexById('OverNewImportant');
        var RequestApproveNoIndex = cts360_grid.getColIndexById('RequestApproveNo');
        var statusIndex = cts360_grid.getColIndexById('ARStatusNameDisplay');

        if (cts360_grid.getRowsNum() != 0) {
            for (var i = 0; i < cts360_grid.getRowsNum(); i++) {
                var row_id = cts360_grid.getRowId(i);

                if (cts360_grid.cells(row_id, overNewIndex).getValue() == "o" || cts360_grid.cells(row_id, overNewIndex).getValue() == c_over) {
                    cts360_grid.setRowColor(row_id, "#ff9999");
                } else {
                    cts360_grid.setRowColor(row_id, "ffffff");
                }

                var celldata = cts360_grid.cells(row_id, statusIndex).getValue();
                if (!celldata || celldata.toString()._dhx_trim() == "") {
                    celldata = "n ";
                }

                var cellPrefix = celldata.substr(0, 1);
                var cellData = celldata.substr(1, celldata.length - 1);

                if (cellPrefix == "y") {
                    cts360_grid.cells(row_id, statusIndex).setValue(cellData);
                    cts360_grid.setCellTextStyle(row_id, statusIndex, "background-color: yellow");
                } else if (cellPrefix == "n") {
                    cts360_grid.cells(row_id, statusIndex).setValue(cellData);
                }

                if (gen_ctrl == true) {
                    var tmpData = $($("<p>").html(cts360_grid.cells(row_id, RequestApproveNoIndex).getValue()).text());
                    var requestNo = tmpData.filter("RequestNo").attr("value");
                    var approveNo = tmpData.filter("ApproveNo").attr("value");
                    var contractCode = tmpData.filter("ContractCode").attr("value");

                    var tagARequestNo = "<a href='#'>" + requestNo + "<br/>" + approveNo + "<input type='hidden' name='reqno' value='" + requestNo + "'/></a>";
                    var tagAContractCode = "<a href='#'>" + contractCode + "<input type='hidden' name='contractCode' value='" + contractCode + "'/></a>";
                    cts360_grid.cells(row_id, RequestApproveNoIndex).setValue(tagARequestNo + "<br/><br/>" + tagAContractCode);
                }
            }

            //if (gen_ctrl == true) {  //Comment by Jutarat A. on 21082012
            $("#ar_result_list a:has(input:hidden[name=reqno])")
            .unbind("click")
            .click(function () {
                var obj = {
                    pRequestNo: $(this).children("input:hidden[name=reqno]").val()
                };
                ajax_method.CallScreenControllerWithAuthority("/Contract/CTS380", obj, true);
            });
            //}

            $("#ar_result_list a:has(input:hidden[name=contractCode])")
            .css("color", "red")
            .unbind("click")
            .click(function () {
                var obj = {
                    strContractCode: $(this).children("input:hidden[name=contractCode]").val()
                };
                ajax_method.CallScreenControllerWithAuthority("/Common/CMS190", obj, true);
            });

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

        //Comment by Jutarat A. on 04092012
//        if (val == "o") {
//            this.cell.innerHTML = c_over;
//            this.cell.style.color = "white";
//            this.cell.style.backgroundColor = "red";
//            this.cell.style.fontWeight = "bold";
//        } else 
        //End Comment
        
        if (val == "n") {
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

function ARListByRole() {
    $("#Search_by_Role").show();
    $("#Search_by_Condition").hide();
    clickSearchByRole();
}

function ARListBySearch() {
    $("#Search_by_Condition").show();
    $("#Search_by_Role").hide();
    clickSearchAR();
}

function searchAR() {
    $("#Search_by_Condition").show();
    $("#Search_by_Role").hide();
}

// BUTTON ACTION ####################################################################################################################################

function ARStatusHandling_click() {
    $("#chkARStatusCompleted").attr("checked", false);
}

function ARStatusCompleted_click() {
    $("#chkARStatusHandling").attr("checked", false);
}

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

//function ContractStatusAllClick() {
//    $("#ContractStatusUnimplemented").attr("checked", false);
//    $("#ContractStatusImplemented").attr("checked", false);
//    $("#ContractStatusStopService").attr("checked", false);
//    $("#ContractStatusCancelFinish").attr("checked", false);
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

//function ContractTypeAllClick() {
//    $("#ContractTypeAL").attr("checked", false);
//    $("#ContractTypeSales").attr("checked", false);
//    $("#ContractTypeMaintenance").attr("checked", false);
//    $("#ContractTypeSentryGuard").attr("checked", false);
//}

function clickClearByRole() {
    $("#CTS310_ByRole").clearForm();
    SetDateFromToData("#dateByRolePeriodFrom", "#dateByRolePeriodTo", c_datefrom, c_dateto);
    $("#cboARRole").val(default_role);
    $("#cboARStatus").val(default_status);
    $("#cboARSpecifyPeriod").val(default_period);
    DeleteAllRow(cts360_grid);
    $("#Search_Result").hide();
}

function clickClearAR() {
    $("#CTS360_SearchAR").clearForm();
    ClearDateFromToControl("#dateSearchPeriodFrom", "#dateSearchPeriodTo");
    //SetDateFromToData("#dateSearchPeriodFrom", "#dateSearchPeriodTo", c_datefrom, c_dateto);
    DeleteAllRow(cts360_grid);
    $("#Search_Result").hide();
}

// SEARCH FUNCTION ##################################################################################################################################

function clickSearchByRole() {
    //Add by Jutarat A. on 20120508
    $("#btnSearchByRole").attr("disabled", true);
    master_event.LockWindow(true);

    var con = {
        ARRole: $("#cboARRole").val(),
        ARStatus: $("#cboARStatus").val(),
        ARSpecifyPeriod: $("#cboARSpecifyPeriod").val(),
        ARSpecifyPeriodFrom: $("#dateByRolePeriodFrom").val(),
        ARSpecifyPeriodTo: $("#dateByRolePeriodTo").val()
    };

    if ($("#ar_result_list").length > 0) {
        $("#ar_result_list").LoadDataToGrid(cts360_grid, 20, true, "/Contract/CTS360_ARListByRole", con, "dtARList", false,
            function (result, controls, isWarning) {
                if (controls != undefined) {
                    VaridateCtrl(["cboARSpecifyPeriod",
                    "dateByRoldPeriodFrom",
                    "dateByRolePeriodTo"], controls);
                }

                //Add by Jutarat A. on 20120508
                master_event.LockWindow(false);
                $("#btnSearchByRole").attr("disabled", false);
            },
            function (result, controls, isWarning) {
                if (isWarning == undefined) {
                    $("#Search_Result").show();
                    master_event.ScrollWindow("#Search_Result", false);  //Add by Jutarat A. on 20120508
                }

                //Add by Jutarat A. on 20120508
                master_event.LockWindow(false);
                $("#btnSearchByRole").attr("disabled", false);
            }
        );
    }
}

function clickSearchAR() {
    //Add by Jutarat A. on 20120508
    $("#btnSearchAR").attr("disabled", true);
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
        QuotationTargetCode : $("#txtQuatationTargetCode").val(),
        ContractTargetPurchaserName : $("#txtContractPurchaserName").val(),
        ContractCode : $("#txtUserContractCode").val(),
        UserCode : $("#txtUserContractCode").val(),
        ContractOfficeCode : $("#cboContractOffice").val(),
        OperationOfficeCode : $("#cboOperationOffice").val(),
        ContractStatus : contract_status,
        ContractType : contract_type,
        CustomerName : $("#txtCustomerName").val(),
        CustomerGroupName : $("#txtCustomerGroup").val(),
        SiteName : $("#txtSiteName").val(),
        ProjectName : $("#txtProjectName").val(),
        RequestNo : $("#txtRequestNo").val(),
        ApproveNo : $("#txtApproveNo").val(),
        ARTitle : $("#txtARTitle").val(),
        ARType : $("#cboARType").val(),
        ARStatusHandling : $("#chkARStatusHandling").prop("checked") ? c_handling : "",
        ARStatusComplete : $("#chkARStatusCompleted").prop("checked") ? c_complete : "",
        AROfficeCode : $("#cboAROffice").val(),
        SpecfyPeriod : $("#cboSearchSpecifyPeriod").val(),
        SpecifyPeriodFrom : $("#dateSearchPeriodFrom").val(),
        SpecifyPeriodTo : $("#dateSearchPeriodTo").val(),
        Requester : $("#txtRequester").val(),
        Approver : $("#txtApprover").val(),
        Auditor : $("#txtAuditor").val()
    }

    if ($("#ar_result_list").length > 0) {
        $("#ar_result_list").LoadDataToGrid(cts360_grid, 20, true, "/Contract/CTS360_SearchAR", con, "dtARList", false,
            function (result, controls, isWarning) {
                if (controls != undefined) {
                    VaridateCtrl(["cboSearchSpecifyPeriod",
                    "dateSearchPeriodFrom",
                    "dateSearchPeriodTo"], controls);
                }

                //Add by Jutarat A. on 20120508
                master_event.LockWindow(false);
                $("#btnSearchAR").attr("disabled", false);
            },
            function (result, controls, isWarning) {
                if (isWarning == undefined) {
                    $("#Search_Result").show();
                    master_event.ScrollWindow("#Search_Result", false);  //Add by Jutarat A. on 20120508
                }

                //Add by Jutarat A. on 20120508
                master_event.LockWindow(false);
                $("#btnSearchAR").attr("disabled", false);
            }
        );
    }
}
