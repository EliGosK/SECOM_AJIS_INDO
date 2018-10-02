/// <reference path = "../../Scripts/jquery-1.5.1-vsdoc.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxcommon.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxgrid.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxgridcell.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/ext/dhtmlxgrid_pgn.js" />

var mygrid;
var mygrid2;
var btn_group_detail = "btnGroupDetail";
var btn_site_detail = "btnSiteDetail";

$(document).ready(function () {

    /* ------- Disable for first release ------ */ // Narupon W. 10/Jan/2012
    //    $("#btnViewIncidentList").attr("disabled", true);
    //    $("#btnViewARList").attr("disabled", true);
    //    $("#btnRegistIncident").attr("disabled", true);
    //    $("#btnRegisAR").attr("disabled", true);
    /* ------- Disable for first release (end) ------ */


    $("#Group_List").show();
    $("#Site_List").show();

    $("#btnViewIncidentList").click(function () {

        // New windos CTS320 : Incident list

        var obj = {
            strIncidentRelevantType: $("#C_INCIDENT_RELEVANT_TYPE_CUSTOMER").val(),
            strIncidentRelevantCode: CMS080Data.CustCode
        };

        ajax_method.CallScreenControllerWithAuthority("/Contract/CTS320", obj, true);

    });

    $("#btnViewARList").click(function () {
        // New window CTS370 : AR list

        var obj = {
            strARRelevantType: $("#C_AR_RELEVANT_TYPE_CUSTOMER").val(),
            strARRelevantCode: CMS080Data.CustCode
        };

        ajax_method.CallScreenControllerWithAuthority("/Contract/CTS370", obj, true);
    });

    $("#btnRegistIncident").click(function () {
        // New window CTS300 : Register new incident

        var obj = {
            strIncidentRelevantType: $("#C_INCIDENT_RELEVANT_TYPE_CUSTOMER").val(),
            strIncidentRelevantCode: CMS080Data.CustCode
        };

        ajax_method.CallScreenControllerWithAuthority("/Contract/CTS300", obj, true);
    });

    $("#btnRegisAR").click(function () {
        // New window CTS350 : Register new AR

        var obj = {
            strARRelevantType: $("#C_AR_RELEVANT_TYPE_CUSTOMER").val(),
            strARRelevantCode: CMS080Data.CustCode
        };

        ajax_method.CallScreenControllerWithAuthority("/Contract/CTS350", obj, true);
    });

    mygrid = $("#group_list_grid").InitialGrid(ROWS_PER_PAGE_FOR_VIEWPAGE, true, "/Common/CMS080_InitGroupGrid");

    SpecialGridControl(mygrid, ["Detail"]);
    BindOnLoadedEvent(mygrid, function (gen_ctrl) {
        if (mygrid.getRowsNum() != 0) {
            for (var i = 0; i < mygrid.getRowsNum(); i++) {
                var row_id = mygrid.getRowId(i);

                if (gen_ctrl == true) {
                    //                if (permission.CMS100 == "True") {
                    GenerateDetailButton(mygrid, btn_group_detail, row_id, "Detail", true);
                }

                BindGridButtonClickEvent(btn_group_detail, row_id, function (rid) {
                    var group_code = mygrid.cells(rid, mygrid.getColIndexById('GroupCode')).getValue();
                    var parameter = {
                        GroupCode: group_code
                    };
                    ajax_method.CallScreenControllerWithAuthority('/Common/CMS100', parameter, true);
                    mygrid.selectRow(mygrid.getRowIndex(rid));
                });
                //                } else {
                //                    GenerateDetailButton(mygrid, btn_group_detail, row_id, "Detail", false);
                //                }
            }
        }
    });

    mygrid2 = $("#site_list_grid").InitialGrid(ROWS_PER_PAGE_FOR_VIEWPAGE, true, "/Common/CMS080_InitSiteGrid");

    SpecialGridControl(mygrid2, ["Detail"]);
    BindOnLoadedEvent(mygrid2, function (gen_ctrl) {
        if (mygrid2.getRowsNum() != 0) {
            for (var i = 0; i < mygrid2.getRowsNum(); i++) {
                var row_id = mygrid2.getRowId(i);

                if (gen_ctrl == true) {
                    //                if (permission.CMS280 == "True") {
                    GenerateDetailButton(mygrid2, btn_site_detail, row_id, "Detail", true);
                }

                BindGridButtonClickEvent(btn_site_detail, row_id, function (rid) {
                    var site_code = mygrid2.cells(rid, mygrid2.getColIndexById('SiteCode')).getValue();
                    var parameter = {
                        strSiteCode: site_code
                    };
                    ajax_method.CallScreenControllerWithAuthority('/Common/CMS280', parameter, true);
                    mygrid2.selectRow(mygrid2.getRowIndex(rid));
                });
                //                } else {
                //                    GenerateDetailButton(mygrid2, btn_site_detail, row_id, "Detail", false);
                //                }
            }
        }
    });

    loadGroupList();
    //loadSiteList();

    $("#Search_Criteria").SetEmptyViewData();
});

function loadGroupList() {
    //3.2	Search Employee Data
    if ($.find("#group_list_grid").length > 0) {
        var param = { "custCode": custCode };
        $("#group_list_grid").LoadDataToGrid(mygrid, ROWS_PER_PAGE_FOR_VIEWPAGE, true, "/Common/CMS080_CustomerGroupSearch", param, "dtCustomerGroupForView", false, loadSiteList,
            function (result, controls, isWarning) {
                if (isWarning == undefined) {
                    $("#Group_List").show();
                    loadSiteList();
                }
            }
        );
    }
}

function loadSiteList() {
    //3.2	Search Employee Data
    if ($("#site_list_grid").length > 0) {
        var param = { "custCode": custCode, "strCustomerRole": custRole };
        $("#site_list_grid").LoadDataToGrid(mygrid2, ROWS_PER_PAGE_FOR_VIEWPAGE, true, "/Common/CMS080_SiteSearch", param, "dtSiteList", false, null,
            function (result, controls, isWarning) {
                if (isWarning == undefined) {
                    $("#Site_List").show();
                }
            }
        );
    }
}