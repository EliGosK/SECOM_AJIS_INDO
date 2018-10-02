/// <reference path = "../../Scripts/jquery-1.5.1-vsdoc.js" />
/// <reference path = "../../Scripts/Base/GridControl.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxcommon.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxgrid.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxgridcell.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/ext/dhtmlxgrid_pgn.js" />

var CTS340_grid;

$(document).ready(function () {
    $("#btnSelect").click(searchIncidentResult);
    $("#summaryPeriod").val(c_summary_thisweek);
    innitialGrid();
});

function innitialGrid() {
    var con = {
        summaryPeriod: $("#summaryPeriod").val()
    };

    CTS340_grid = $("#incident_result").LoadDataToGridWithInitial(ROWS_PER_PAGE_FOR_SEARCHPAGE, true, true, "/Contract/CTS340_searchIncident", con, "dtSummaryIncident", false);

    //if (permission.CTS310 == "True") {
    BindOnLoadedEvent(CTS340_grid, function () {
        var officeCodeIndex = CTS340_grid.getColIndexById('OfficeCode');
        var officeNameIndex = CTS340_grid.getColIndexById('Name');

        var totalIncidentIdx = CTS340_grid.getColIndexById('TotalIncident');
        var totalIncompleteIncidentIdx = CTS340_grid.getColIndexById('TotalIncompleteIncident');
        var totalOverIncidentIdx = CTS340_grid.getColIndexById('TotalOverIncident');
        var canIncidentIdx = CTS340_grid.getColIndexById('CanIncident');
        var canIncompleteIncidentIdx = CTS340_grid.getColIndexById('CanIncompleteIncident');
        var canOverIncidentIdx = CTS340_grid.getColIndexById('CanOverIncident');
        var claimIncidentIdx = CTS340_grid.getColIndexById('ClaimIncident');
        var claimIncompleteIncidentIdx = CTS340_grid.getColIndexById('ClaimIncompleteIncident');
        var claimOverIncidentIdx = CTS340_grid.getColIndexById('ClaimOverIncident');

        if (CTS340_grid.getRowsNum() != 0) {
            for (var i = 0; i < CTS340_grid.getRowsNum(); i++) {
                var row_id = CTS340_grid.getRowId(i);

                var officeCode = CTS340_grid.cells(row_id, officeCodeIndex).getValue();
                var officeName = CTS340_grid.cells(row_id, officeNameIndex).getValue();
                var officeHtml = CTS340_grid.cells(row_id, officeNameIndex).cell.innerHTML;
                var totalIncident = CTS340_grid.cells(row_id, totalIncidentIdx).getValue();
                var beforeLink = "";

                var tagA = "";
                if (officeHtml.indexOf("&nbsp;") != -1 || totalIncident == 0) {
                    //beforeLink = "&nbsp;&nbsp;";
                    //officeHtml = officeHtml.substr(12);
                    tagA = officeHtml;
                } else {
                    tagA = "<a href='#'>" + officeHtml + "<input type='hidden' name='callerID' value='" + c_cts310_caller + "'/><input type='hidden' name='incidentOfficeCode' value='" + officeCode + "'/></a>";
                }

                //var tagA = beforeLink + "<a href='#'>" + officeHtml + "<input type='hidden' name='callerID' value='" + c_cts310_caller + "'/><input type='hidden' name='incidentOfficeCode' value='" + officeCode + "'/></a>";
                //CTS340_grid.cells(row_id, officeNameIndex).setValue(tagA);
                CTS340_grid.cells(row_id, officeNameIndex).cell.innerHTML = tagA;

                CTS340_grid.cells(row_id, totalIncidentIdx).setBgColor("#ffffdd");
                CTS340_grid.cells(row_id, totalIncompleteIncidentIdx).setBgColor("#ffffdd");
                CTS340_grid.cells(row_id, totalOverIncidentIdx).setBgColor("#ffffdd");
                CTS340_grid.cells(row_id, canIncidentIdx).setBgColor("#ffddff");
                CTS340_grid.cells(row_id, canIncompleteIncidentIdx).setBgColor("#ffddff");
                CTS340_grid.cells(row_id, canOverIncidentIdx).setBgColor("#ffddff");
                CTS340_grid.cells(row_id, claimIncidentIdx).setBgColor("#ddffff");
                CTS340_grid.cells(row_id, claimIncompleteIncidentIdx).setBgColor("#ddffff");
                CTS340_grid.cells(row_id, claimOverIncidentIdx).setBgColor("#ddffff");
            }

            $("#incident_result a").each(function () {
                $(this).click(function () {
                    var obj = {
                        callerID: $(this).children("input:hidden[name=callerID]").val(),
                        incidentOfficeCode: $(this).children("input:hidden[name=incidentOfficeCode]").val()
                    };
                    ajax_method.CallScreenControllerWithAuthority("/Contract/CTS310", obj, true, "0");
                });
            });
        }
    });
    //}
}

function eXcell_reqInstallCell(cell) {
    this.base = eXcell_edn;
    this.base(cell);
    this.setValue = function (val) {
        if (!val || val.toString()._dhx_trim() == "") {
            val = 0;
        }

        if (val > 0) {
            this.cell.innerHTML = c_reqinst;
        } else {
            this.cell.innerHTML = "";
        }
    };
}
eXcell_reqInstallCell.prototype = new eXcell_edn;

function searchIncidentResult() {
    var con = {
        summaryPeriod: $("#summaryPeriod").val()
    };

    if ($("#incident_result").length > 0) {
        $("#incident_result").LoadDataToGrid(CTS340_grid, ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/Contract/CTS340_searchIncident", con, "dtSummaryIncident", false);
    }
}
