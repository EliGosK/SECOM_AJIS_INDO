/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/control_events.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>
/// <reference path="../../Scripts/Base/DateTimePicker.js"/>

/// <reference path="../Base/object/ajax_method.js" />
/// <reference path="../Base/object/command_event.js" />
/// <reference path="../Base/object/master_event.js" />


var IVS150_CheckingStatusHistoryGrid;
var IVS150_OfficeCheckingListGrid;

$(document).ready(function () {

    // Initial button
    $("#btnStartChecking").attr("disabled", true);
    $("#btnStopChecking").attr("disabled", true);

    // Hide grid before show
    $("#IVS150_CheckingStatusGrid").hide();
    //$("#IVS150_OfficeCheckingListGrid").hide();

    // Initial grid
    if ($.find("#IVS150_CheckingStatusGrid").length > 0) {
        IVS150_CheckingStatusHistoryGrid = $("#IVS150_CheckingStatusGrid").InitialGrid(0, true, "/Inventory/IVS150_InitialHistoryGrid");
    }

    if ($.find("#IVS150_OfficeCheckingListGrid").length > 0) {
        IVS150_OfficeCheckingListGrid = $("#IVS150_OfficeCheckingListGrid").LoadDataToGridWithInitial(0, false, false,
                                "/Inventory/IVS150_GetOfficeCheckingList",
                                "",
                                "dtOfficeCheckingList", false);
    }

    // Bind event
    $("#CheckingYear").change(Cbo_CheckingYear_Change);

    // Button click
    $("#btnStartChecking").click(StartChecking);
    $("#btnStopChecking").click(StopChecking);

    GridControl.DisableSelectionHighlight();

    // Initial page
    InitialPage();

});


function InitialPage() {
    var enableBtnStart = IVS150_ViewBag.EnableBtnStartingChecking == "1" ? true : false;
    var enableBtnStop = IVS150_ViewBag.EnableBtnStopChecking == "1" ? true : false;

    $("#btnStartChecking").attr("disabled", !enableBtnStart);
    $("#btnStopChecking").attr("disabled", !enableBtnStop);

    if ($("#CheckingYear >option").length > 1) {
        var val = $("#CheckingYear >option").get(1).value;
        $("#CheckingYear").val(val);
    }
    else {
        $("#CheckingYear").val("");
    }


    var year = $("#CheckingYear").val();
    LoadDataToGrid_CheckingStatus(year);

}

function InitscreenAfterAction() {

    ajax_method.CallScreenController("/Inventory/IVS150_GetEnableStartStopButton", "", function (result) {
        if (result != undefined) {
            $("#btnStartChecking").attr("disabled", !result.EnableBtnStartingChecking);
            $("#btnStopChecking").attr("disabled", !result.EnableBtnStopChecking);
        }
    });

    if ($("#CheckingYear >option").length > 1) {
        var val = $("#CheckingYear >option").get(1).value;
        $("#CheckingYear").val(val);
    }
    else {
        $("#CheckingYear").val("");
    }


    var year = $("#CheckingYear").val();
    LoadDataToGrid_CheckingStatus(year);
    LoadDataToGrid_OfficeCheckingList();
}

function LoadDataToGrid_CheckingStatus(year) {
    var param = { "Year": year };

    $("#IVS150_CheckingStatusGrid").LoadDataToGrid(IVS150_CheckingStatusHistoryGrid, 0, false, "/Inventory/IVS150_GetCheckingStatusHistory", param, "dtCheckingStatusList", false,
                    function () { $("#IVS150_CheckingStatusGrid").show(); }, // post-load
                    function (result, controls, isWarning) { // pre-load
                        $("#IVS150_CheckingStatusGrid").show();
                    });

}

function LoadDataToGrid_OfficeCheckingList() {
    $("#IVS150_OfficeCheckingListGrid").LoadDataToGrid(IVS150_OfficeCheckingListGrid, 0, false, "/Inventory/IVS150_GetOfficeCheckingList", "", "dtOfficeCheckingList", false,
                    function () {  // post-load
                        //$("#IVS150_OfficeCheckingListGrid").show(); 
                    },
                    function (result, controls, isWarning) { // pre-load
                        //$("#IVS150_OfficeCheckingListGrid").show();
                    });
}

function Cbo_CheckingYear_Change() {
    var year = $("#CheckingYear").val();
    LoadDataToGrid_CheckingStatus(year);
}

// StartChecking
function StartChecking() {

    var obj = {
        module: "Inventory",
        code: "MSG4072"
    };

    ajax_method.CallScreenController("/Shared/GetMessage", obj, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message,
            function () {
                var lastBtnStart = $("#btnStartChecking").prop("disabled");
                var lastBtnStop = $("#btnStopChecking").prop("disabled");

                $("#btnStartChecking").attr("disabled", true);
                $("#btnStopChecking").attr("disabled", true);

                ajax_method.CallScreenController("/Inventory/IVS150_StartChecking", "", function (result) {
                    if (result == "1") {
                        InitscreenAfterAction();
                    }
                    else {
                        $("#btnStartChecking").attr("disabled", lastBtnStart);
                        $("#btnStopChecking").attr("disabled", lastBtnStop);
                    }
                });
            }
        );
    });

}

// StopChecking
function StopChecking() {

    var obj = {
        module: "Inventory",
        code: "MSG4074"
    };

    ajax_method.CallScreenController("/Shared/GetMessage", obj, function (result) {
        OpenYesNoMessageDialog(result.Code, result.Message,
            function () {
                var lastBtnStart = $("#btnStartChecking").prop("disabled");
                var lastBtnStop = $("#btnStopChecking").prop("disabled");

                $("#btnStartChecking").attr("disabled", true);
                $("#btnStopChecking").attr("disabled", true);

                ajax_method.CallScreenController("/Inventory/IVS150_StopChecking", "", function (result) {
                    if (result == "1") {
                        InitscreenAfterAction();
                    }
                    else {
                        $("#btnStartChecking").attr("disabled", lastBtnStart);
                        $("#btnStopChecking").attr("disabled", lastBtnStop);
                    }
                });
            }
        );
    });

}








