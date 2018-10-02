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




var gridSearchResult;

var BLS080 = {
    ScreenMode: {
        Register: "RegisterMode",
        Confirm: "ConfirmMode",
        Finish: "FinishMode",
        NewRegister: "NewRegisterMode",
        Reset: "Reset"
    }
};

$(document).ready(function () {

    InitialGrid();
    InitialEvent();
    InitialPage();
});

function InitialPage() {

    // --- Date Picker ---
    InitialDateFromToControl("#AutoTranferDateFrom", "#AutoTranferDateTo");
    InitialDateFromToControl("#GeneateDateFrom", "#GeneateDateTo");

    $("#divSearchResult").hide();
}

function InitialGrid() {
    // Initial grid 1
    if ($.find("#BLS080_ResultListGrid").length > 0) {
        gridSearchResult = $("#BLS080_ResultListGrid").InitialGrid(ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/Billing/BLS080_InitialSearchResultGrid", function () {

            SpecialGridControl(gridSearchResult, ["BtnDownload"]);
            BindOnLoadedEvent(gridSearchResult, function () {
                //var colInx = gridCheckingDetail.getColIndexById('BtnRemove');
                for (var i = 0; i < gridSearchResult.getRowsNum(); i++) {
                    var rowId = gridSearchResult.getRowId(i);

                    GenerateDownloadButton(gridSearchResult, "btnDownload", rowId, "BtnDownload", true);
                    BindGridButtonClickEvent("btnDownload", rowId, Download_click);
                    
                }

                gridSearchResult.setSizes();
            });
        });
    }
}

function InitialEvent() {
    $("#btnSearch").click(BLS080_Search);
    $("#btnClear").click(BLS080_Clear);
}

function BLS080_Search() {
    // For prevent click this button more than one time
    $("#btnSearch").attr("disabled", true);
    master_event.LockWindow(true);

    var obj = $("#formSearchCondition").serializeObject2();
    $("#BLS080_ResultListGrid").LoadDataToGrid(gridSearchResult, ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/Billing/BLS080_SearchResponse", obj, "dtDownloadAutoTransferBankFile", false,
        function (result, controls) { // post-load

            // For prevent click this button more than one time
            $("#btnSearch").attr("disabled", false);
            master_event.LockWindow(false);

            if (controls != undefined) {
                VaridateCtrl(["SecomAccountID"], controls);
            }
        },
        function (result, controls, isWarning) { // pre-load
            if (isWarning == undefined) {
                $("#divSearchResult").show();
            }
        });
}

function BLS080_Clear() {
    $("#formSearchCondition").clearForm();
    $("#divSearchResult").hide();

    ClearDateFromToControl("#AutoTranferDateFrom", "#AutoTranferDateTo");
    ClearDateFromToControl("#GeneateDateFrom", "#GeneateDateTo");

}


function Download_click(row_id) {

    gridSearchResult.selectRow(gridSearchResult.getRowIndex(row_id));

    // Create JSON object from string JSON
    var strJson = gridSearchResult.cells2(gridSearchResult.getRowIndex(row_id), gridSearchResult.getColIndexById('ToJson')).getValue();
    strJson = htmlDecode(strJson);
    var data = JSON.parse(strJson);

    var obj = { fileName: data.FileName };

    ajax_method.CallScreenController("/Billing/BLS080_CheckExistFile", obj, function (result) {
        if (result == 1) {
            var key = ajax_method.GetKeyURL(null);
            // var url = ajax_method.GenerateURL("/Billing/BLS080_DownloadAndWriteLog?k=" + key + "&fileName=" + data.FileName);
            // window.open(url, "Report", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
            download_method.CallDownloadController("fDownload", "/Billing/BLS080_DownloadAndWriteLog", obj);
        }
        else {
            var param = { "module": "Common", "code": "MSG0112" };
            call_ajax_method("/Shared/GetMessage", param, function (data) {
                /* ====== Open info dialog =====*/
                OpenInformationMessageDialog(param.code, data.Message);
            });
        }

    }, false);
}
