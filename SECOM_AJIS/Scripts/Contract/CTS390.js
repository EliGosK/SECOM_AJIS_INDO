/// <reference path = "../../Scripts/jquery-1.5.1-vsdoc.js" />
/// <reference path = "../../Scripts/Base/GridControl.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxcommon.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxgrid.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxgridcell.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/ext/dhtmlxgrid_pgn.js" />

var CTS390_grid;

$(document).ready(function () {
    $("#btnSelect").click(searchIncidentResult);
    innitialGrid();
});

function innitialGrid() {
    var con = { summaryPeriod: "" };

    CTS390_grid = $("#ar_result").LoadDataToGridWithInitial(ROWS_PER_PAGE_FOR_SEARCHPAGE, true, true, "/Contract/CTS390_searchAR", con, "dtSummaryAR", false);

//    if (permission.CTS360 == "True") {
    BindOnLoadedEvent(CTS390_grid, function () {
        var officeCodeIndex = CTS390_grid.getColIndexById('OfficeCode');
        var officeNameIndex = CTS390_grid.getColIndexById('OfficeName');
        var totalARIdx = CTS390_grid.getColIndexById('TotalAR');

        if (CTS390_grid.getRowsNum() != 0) {
            for (var i = 0; i < CTS390_grid.getRowsNum(); i++) {
                var row_id = CTS390_grid.getRowId(i);

                var officeCode = CTS390_grid.cells(row_id, officeCodeIndex).getValue();
                var officeName = CTS390_grid.cells(row_id, officeNameIndex).getValue();
                var officeHtml = CTS390_grid.cells(row_id, officeNameIndex).cell.innerHTML;
                var totalAR = CTS390_grid.cells(row_id, totalARIdx).getValue();
                var beforeLink = "";

                var tagA = "";

                if (officeHtml.indexOf("&nbsp;") != -1 || totalAR == 0) {
                    //beforeLink = "&nbsp;&nbsp;";
                    //officeHtml = officeHtml.substr(12);
                    tagA = officeHtml;
                } else {
                    tagA = "<a href='#'>" + officeName + "<input type='hidden' name='screenMode' value='" + c_cts360_screenmode + "'/><input type='hidden' name='AROfficeCode' value='" + officeCode + "'/></a>";
                }

                //var tagA = beforeLink + "<a href='#'>" + officeName + "<input type='hidden' name='screenMode' value='" + c_cts360_screenmode + "'/><input type='hidden' name='AROfficeCode' value='" + officeCode + "'/></a>";
                //CTS390_grid.cells(row_id, officeNameIndex).setValue(tagA);
                CTS390_grid.cells(row_id, officeNameIndex).cell.innerHTML = tagA;
            }

            $("#ar_result a").each(function () {
                $(this).click(function () {
                    var obj = {
                        screenMode: $(this).children("input:hidden[name=screenMode]").val(),
                        AROfficeCode: $(this).children("input:hidden[name=AROfficeCode]").val()
                    };
                    ajax_method.CallScreenControllerWithAuthority("/Contract/CTS360", obj, true, "0");
                });
            });
        }
    });
//    }
}

function searchIncidentResult() {
    var con = {
        summaryPeriod: $("#summaryPeriod").val()
    };

    if ($("#ar_result").length > 0) {
        $("#ar_result").LoadDataToGrid(CTS390_grid, ROWS_PER_PAGE_FOR_SEARCHPAGE, false, "/Contract/CTS390_searchAR", con, "dtSummaryAR", false, null,
            function (result, controls, isWarning) {
                if (isWarning == undefined) {
                    $("#ar_result").show();
                }
            }
        );
    }
}
