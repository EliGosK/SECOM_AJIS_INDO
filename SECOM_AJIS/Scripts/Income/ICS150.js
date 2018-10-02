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

var IDPaymentDate = "#PaymentDate";

var ICS150_process = {

    InitialScreen: function () {
        master_event.LockWindow(true);
        $(IDPaymentDate).InitialDate();
        $("#btnDownload").click(sendDataExcel);
        $("#btnClear").click(clearSearch);
        master_event.LockWindow(false);
    },
    SelectProcessDate_OnChanged: function () {
        $(IDPaymentDate).on("BlurAfterSetDate", function () {
            PaymentDateChanged();
        });
    }
};

$(document).ready(function () {
    ICS150_process.InitialScreen();
    ICS150_process.SelectProcessDate_OnChanged();
});


function PaymentDateChanged() {
    var prevPaymentDate = $(IDPaymentDate).data("previous-val");
    var PaymentDate = $(IDPaymentDate).val();

    if (prevPaymentDate != PaymentDate) {
        master_event.LockWindow(true);
        var obj = {
            paymentDate: PaymentDate,
            createBy: $("#CreateBy").val()
        };
        $(IDPaymentDate).data("previous-val", obj.paymentDate);
        ajax_method.CallScreenController("/Income/ICS150_GetGroupName", obj, function (data) {
            regenerate_combo("#GroupName", data);
            master_event.LockWindow(false);
        });
    }
}


function sendDataExcel() {

    master_event.LockWindow(true);

    var obj = {
        PaymentDate: $(IDPaymentDate).val(),
        GroupName: $("#GroupName").val(),
        CreateBy: $("#CreateBy").val()
    };


    //    if (obj.PaymentDate == "" || obj.GroupName == "") {
    //        var messageParam = { "module": "Income", "code": "MSG7127", "param": "" };
    //        call_ajax_method("/Shared/GetMessage", messageParam, function (data) {
    //            OpenWarningDialog(data.Message);
    //        });

    //        master_event.LockWindow(false);
    //        return;
    //    }


    ajax_method.CallScreenController("/Income/ICS150_ExportExcelData", obj, function (result, controls, isWarning) {
        if (result != undefined) {
            download_method.CallDownloadController("ifDownload", "/Income/ICS150_Download", null);
        }
        else if (controls != undefined) {

            VaridateCtrl(controls, controls);
        }

    }, false);
    master_event.LockWindow(false);

}


function clearSearch() {
    ClearDateFromToControl(IDPaymentDate);
    $("#GroupName").empty();
    var newOption = $('<OPTION value="" selected>----Select----</OPTION>');
    $('#GroupName').append(newOption);

}