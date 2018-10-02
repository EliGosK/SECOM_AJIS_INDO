/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.7.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/control_events.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>
/// <reference path="../../Scripts/Base/DateTimePicker.js"/>
/// <reference path="../../Scripts/Base/object/ajax_method.js"/>

/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxgridcell.js" /> 
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js" />
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxgrid.js" />
/// <reference path="../../Content/js/dhtmlxgrid/ext/dhtmlxgrid_data.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/ext/dhtmlxgrid_pgn.js"/>
/// <reference path="../json.js" />
/// <reference path="../json2.js" />
/// <reference path="../Base/object/command_event.js" />

var cboInstallationReport = "#cboInstallationReportType";
var ISS100_Screen = {
    divDailySearch: "#divDailyBasicInfo",
    divMonthSearch: "#divMonthBasicInfo"
};

var ISS100_process = {
    SelectProcess_OnChanged: function () {
        $(cboInstallationReport).change(function () {
            master_event.LockWindow(true);
            if ($(cboInstallationReport).val() == "") {
                $(ISS100_Screen.divDailySearch).hide();
                $(ISS100_Screen.divMonthSearch).hide();
                clearSearch();
            }
            else if ($(cboInstallationReport).val() == 0) {
                $(ISS100_Screen.divMonthSearch).hide();
                $(ISS100_Screen.divDailySearch).show();
                clearSearch();
            }
            else if ($(cboInstallationReport).val() == 1) {
                $(ISS100_Screen.divDailySearch).hide();
                $(ISS100_Screen.divMonthSearch).show();
                clearSearch();
            }
        
            master_event.LockWindow(false);
        });

    },
    InitialScreen: function () {
        master_event.LockWindow(true);
        $("#btnDownloadDaily").click(sendDataExcel);
        $("#btnDownloadMonth").click(sendDataExcelMontly);
        $("#btnClear").click(clearSearch);
        $("#btnClearMonth").click(clearSearch);
        InitialDateFromToControl("#PaidDateFrom", "#PaidDateTo");

        InitialDateFromToControl("#ReceiveDateFrom", "#ReceiveDateTo");
        InitialDateFromToControl("#CompleteDateFrom", "#CompleteDateTo");
        InitialDateFromToControl("#ExpectedStartDateFrom", "#ExpectedStartDateTo");
        InitialDateFromToControl("#ExpectedCompleteDateFrom", "#ExpectedCompleteDateTo");

        //$(ISS100_Screen.divDailySearch).show();
        master_event.LockWindow(false);
    }
};

$(document).ready(function () {
    ISS100_process.InitialScreen();
    ISS100_process.SelectProcess_OnChanged();
});

function sendDataExcel() {

    master_event.LockWindow(true);

    $("#divDailyBasicInfo").find(".highlight").toggleClass("highlight", false);

    var obj = {
        PaidDateFrom: $("#PaidDateFrom").val(),
        PaidDateTo: $("#PaidDateTo").val(),
        SubcontractorCode: $("#SubcontractorCode").val()
    };

    ajax_method.CallScreenController("/Installation/ISS100_ExportExcelData", obj, function (result, controls, isWarning) {
        if (result != undefined) {
            download_method.CallDownloadController("ifDownload", "/Installation/ISS100_Download", null);
        }
        else if (controls != undefined) {

            VaridateCtrl(controls, controls);
        }
        master_event.LockWindow(false);
    }, false);

}

function clearSearch() {
    ClearDateFromToControl("#PaidDateFrom", "#PaidDateTo");

    $("#SubcontractorCode").val("");
    $("#InstallationType")[0].selectedIndex = 0;
    $("#ContractCode").val("");
    $("#SiteName").val("");
    $("#SubcontractorCodeMonthly").val("");
    $("#ProductName").val("");
    $("#BuildingType")[0].selectedIndex = 0;
    VaridateCtrl(["SubcontractorCode", "InstallationType", "ContractCode", "SiteName", "SubcontractorCodeMonthly", "ProductName", "BuildingType"], null);
    ClearDateFromToControl("#ReceiveDateFrom", "#ReceiveDateTo");
    ClearDateFromToControl("#CompleteDateFrom", "#CompleteDateTo");
    ClearDateFromToControl("#ExpectedStartDateFrom", "#ExpectedStartDateTo");
    ClearDateFromToControl("#ExpectedCompleteDateFrom", "#ExpectedCompleteDateTo");

}

function sendDataExcelMontly() {

    master_event.LockWindow(true);

    $("#divMonthBasicInfo").find(".highlight").toggleClass("highlight", false);

    var obj = {
        ReportType: $("#InstallationType").val(),
        ReceiveDateFrom: $("#ReceiveDateFrom").val(),
        ReceiveDateTo: $("#ReceiveDateTo").val(),
        CompleteDateFrom: $("#CompleteDateFrom").val(),
        CompleteDateTo: $("#CompleteDateTo").val(),
        ExpectedStartDateFrom: $("#ExpectedStartDateFrom").val(),
        ExpectedStartDateTo: $("#ExpectedStartDateTo").val(),
        ExpectedCompleteDateFrom: $("#ExpectedCompleteDateFrom").val(),
        ExpectedCompleteDateTo: $("#ExpectedCompleteDateTo").val(),
        ContractCode: $("#ContractCode").val(),
        SiteName: $("#SiteName").val(),
        SubContractorCode: $("#SubcontractorCodeMonthly").val(),
        ProductName: $("#ProductName").val(),
        BuildingType: $("#BuildingType").val()

    };

    ajax_method.CallScreenController("/Installation/ISS100_ExportExcelDataMonthly", obj, function (result, controls, isWarning) {
        if (result != undefined) {
            download_method.CallDownloadController("ifDownload", "/Installation/ISS100_Download", null);
        }
        else if (controls != undefined) {
            VaridateCtrl(controls, controls);
        }
        master_event.LockWindow(false);
    }, false);
//    if (obj.DateFrom == "" || obj.DateTo == "") {
//        var messageParam = { "module": "Installation", "code": "MSG5125", "param": "" };
//        call_ajax_method("/Shared/GetMessage", messageParam, function (data) {
//            OpenWarningDialog(data.Message);
//        });

//        master_event.LockWindow(false);
//        return;
//    }

//    ajax_method.CallScreenController("/Installation/ISS100_ExportExcelDataMonthly", obj, function (result, controls, isWarning) {
//        if (result != undefined) {
//            download_method.CallDownloadController("ifDownload", "/Installation/ISS100_Download", null);
//        }

//        master_event.LockWindow(false);
//    }, false);


}
