/// <reference path = "../../Scripts/jquery-1.5.1-vsdoc.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxcommon.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxgrid.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxgridcell.js" />
/// <reference path = "../../Content/js/dhtmlxgrid/ext/dhtmlxgrid_pgn.js" />
/// <reference path = "../../Scripts/Base/Master.js" />

var btn_group_detail = "btnGroupDetail";

$(document).ready(function () {
    $("#Result_List").hide();
    $("#Search_Form").clearForm();

    $("#OfficeInCharge").RelateControlEvent(officeInCharge_change);

    $("#GroupName").InitialAutoComplete("/Master/GetGroupName");

    $("#btnClear").click(function () {
        $("#Result_List").hide();
        $("#Search_Form").clearForm();

        $('#PersonInCharge >option').remove();

        var parameter = { filter: "" }
        call_ajax_method("/Common/GetPersonInChargeFirstElementAllCombo", parameter, function (data) {
            regenerate_combo("#PersonInCharge", data);
        });
    });

    $("#NumberOfCustomerFrom").BindNumericBox(7, 0, 0, 9999999);
    $("#NumberOfCustomerTo").BindNumericBox(7, 0, 0, 9999999);
    $("#NumberOfSiteFrom").BindNumericBox(7, 0, 0, 9999999);
    $("#NumberOfSiteTo").BindNumericBox(7, 0, 0, 9999999);

    $("#btnSearch").click(vaidateSearch);

    mygrid = $("#result_list_grid").InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/Common/CMS090_InitGrid");

    SpecialGridControl(mygrid, ["GroupDetail"]);
    BindOnLoadedEvent(mygrid, function (gen_ctrl) {
        if (mygrid.getRowsNum() != 0) {
            for (var i = 0; i < mygrid.getRowsNum(); i++) {
                var row_id = mygrid.getRowId(i);

                if (gen_ctrl == true) {
                    //                if (permission.CMS100 == "True") {
                    GenerateDetailButton(mygrid, btn_group_detail, row_id, "GroupDetail", true);
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
                //                    GenerateDetailButton(mygrid, btn_group_detail, row_id, "GroupDetail", false);
                //                }
            }
        }
    });
});

function officeInCharge_change(istab, isblur) {

    $('#PersonInCharge >option').remove();

    var parameter = { filter: $("#OfficeInCharge").val() }
    call_ajax_method("/Common/GetPersonInChargeFirstElementAllCombo", parameter, function (data) {
        regenerate_combo("#PersonInCharge", data);
    });


}

function vaidateSearch() {

    // disable search button
    $("#btnSearch").attr("disabled", true);

    var param = CreateObjectData($("#Search_Form").serialize(), true);

    call_ajax_method(
        '/Common/CMS090_ValidateSearch',
        param,
        function (result, controls) {
            if (result != undefined) {
                param = CreateObjectData($("#Search_Form").serialize());
                loadResultListGrid(param);
            }
            if (controls != undefined) {

                // enable search button
                $("#btnSearch").attr("disabled", false);

                VaridateCtrl(["NumberOfCustomerFrom",
                                "NumberOfCustomerTo",
                                "NumberOfSiteFrom",
                                "NumberOfSiteTo"], controls);
            }
        }
    );
}

function loadResultListGrid(param) {
    if ($("#result_list_grid").length > 0) {
        $("#result_list_grid").LoadDataToGrid(mygrid, ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/Common/CMS090_Search", param, "View_dtGroupList", false,
        function () {
            // enable search button
            $("#btnSearch").attr("disabled", false);
        },
            function (result, controls, isWarning) {
                if (isWarning == undefined) {
                    $("#Result_List").show();
                }
            }
        );
    }
}