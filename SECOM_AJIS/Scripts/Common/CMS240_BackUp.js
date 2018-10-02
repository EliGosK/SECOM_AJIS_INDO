/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>

/* --- Variables --- */
var mygrid;

/*--- Main ---*/
$(document).ready(function () {

    initial();

    /* --- Initial grid --- */
    if ($("#grid_result").length > 0) {
        mygrid = $("#grid_result").InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/Common/CMS240_InitialGrid");
    }

    //bind combo event
    $("#MonthYear").change(function () {
        getPurgeLogData();
    });

    //refresh page every 30 second
//    var refreshId = setInterval(function () {
//        var key = getKeyURL();
//        document.cookie = "sck=" + key + ";path=/";
//        window.location.href = window.location.href;
//    }, 30000);


    //$.ajaxSetup({ cache: false });

});

function initial() {
    
    //Clear Search Data
    $("#MonthYear").val("");
    $("#Status").val("");

    //Hide Result Section
    $("#Search_Result").hide();

    //Set default Purge Button (show but disable)
    SetEnableButton(false);
}

function getPurgeLogData() {


    var my = $("#MonthYear").val();

    if (my == "") {
        return;
    }
    
    var monthYear = '';
    if (my != '') {
        var xx = my.split(" ");
        var yy = xx[0].split("/");
        monthYear = yy[2] + "/" + yy[0] + "/" + yy[1];
    }

    var parameter = { "monthYear": monthYear };

    call_ajax_method(
        '/Common/CMS240_GetPurgeLogData',
        parameter,
        function (result, controls) {
            if (controls != undefined) {
                VaridateCtrl(["MonthYear"], controls);
                $("#MonthYear").focus();
                return;
            }else if (result != undefined) {

                $("#Status").val(result.PurgeStatusName);

                if (result.PurgeStatusCode == CMS240Data.BatchStatusFailed) {
                    getPurgeLogDetailData();
                } else if (result.PurgeStatusCode == CMS240Data.BatchStatusProcessing) {
                    $("#Search_Result").hide();
                    SetEnableButton(false);
                } else if (result.PurgeStatusCode == "") {
                    $("#Search_Result").hide();
                    SetEnableButton(true);
                } else { //Succeeded
                    $("#Search_Result").hide();
                    SetEnableButton(false);
                }
            }
        }
    );
}

function getPurgeLogDetailData() {
    $("#grid_result").LoadDataToGrid(mygrid, ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/Common/CMS240_GetPurgeLogDetailData", null, "CMS240_PurgeLogDataDetail", false,
        null, //post-load
        function (result, controls, isWarning) { //pre-load
            if (isWarning == undefined) {
                $("#Search_Result").show();
            }
        });

    $("#Search_Result").show();
    SetEnableButton(true);
}

function SetEnableButton(enable) {
    if (enable && CMS240Data.EnablePurgeButton == "True") {
        SetPurgeCommand(true, doPurgeAction);
        DisablePurgeCommand(false);
    } else {
        SetPurgeCommand(true, null);
        DisablePurgeCommand(true);
    }
}

function doPurgeAction() {
    var param = { "module": "Common", "code": "MSG0028", "param": "purge log" };
    call_ajax_method("/Shared/GetMessage", param, function (data) {

        /* ====== Open confirm dialog =====*/
        OpenOkCancelDialog(data.Code, data.Message, function () {
            SetEnableButton(false);
            $("#Status").val(CMS240Data.PurgeProcessingStatusName);
            purgeLog();
        }, null);

        return false;
    });
}

function purgeLog() {
    call_ajax_method(
        '/Common/CMS240_PurgeLog',
        null,
        function (result, controls) {
            if (result != undefined) {

                $("#Status").val(result.PurgeStatusName);

                if (result.IsPurgeSucceeded) { //success
                    $("#Search_Result").hide();
                    SetEnableButton(false);
                } else { //failed
                    getPurgeLogDetailData();
                }
            }
        }
    );
}
