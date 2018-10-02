/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>

/* --- Variables --- */
var mygrid;

$(document).ready(function () {

    if ($.find("#grid_result").length > 0) {
        mygrid = $("#grid_result").InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/Common/CMS240_InitialGrid");
    }

    // intial event
    $("#MonthYear").change(cbo_MonthYear_change);

    intialPage();

    // refresh grid every 30 sec.
    var refreshId = setInterval(reload, 30000);

});

function intialPage() {
    $("#Search_Result").hide();
    SetPurgeCommand(true, purge_log_click);
    enable_purge_button(false);
}

function reload() {
   
    cbo_MonthYear_change();
}

function cbo_MonthYear_change() {
    // 1. Load/Set Purge status
    // 2. Load Last fail result staus (grid) if Purge status == failed


    if ($("#MonthYear").val() == "") {
        $("#Search_Result").hide();
        enable_purge_button(false);
    }
    else {
        // Load Purge data
        var param = { MonthYear: $("#MonthYear").val() };
        call_ajax_method_json("/Common/CMS240_GetStatus", param, function (result) {
            if (result != undefined) {

                var isEnablePurgeButton = (result.SuspendFlag == true);

                if (result.xml == "" || result.xml == null) {
                    $("#Status").val("");
                    $("#Search_Result").hide();

                    if (result.IsExistInTransLog == false) {
                        enable_purge_button(false);
                    }
                    else {
                        enable_purge_button(isEnablePurgeButton);
                    }

                }
                else {

                    $("#Status").val(result.PurgeStatusName);
                    $("#grid_result").LoadDataToGrid(mygrid, ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/Common/CMS240_GetPurgeLogData", "", "CMS240_PurgeLogDataDetail", false, function () { enable_purge_button(isEnablePurgeButton); }, function (result) { $("#Search_Result").show(); });

                }
            }
        });




    }


}

function enable_purge_button(enable) {
    var disabled = !enable;
    DisablePurgeCommand(disabled);
}

function purge_log_click() {

    var paramMessage = { "module": "Common", "code": "MSG0149", "param": "purge log" };
    call_ajax_method("/Shared/GetMessage", paramMessage, function (data) {

        OpenOkCancelDialog(data.Code, data.Message, purge_log);

    });

}

function purge_log() {

    $("#Status").val($("#CMS240_lblProcessingStatus").val());
    call_ajax_method_json("/Common/CMS240_PurgeLog", "", function (result) {

        if (result != undefined) {

            $("#Status").val(result.PurgeStatusName);

            if (result.IsPurgeSucceeded == true) {
                enable_purge_button(false);
            }
            else {
                enable_purge_button(true);
            }
        }

    });


}
