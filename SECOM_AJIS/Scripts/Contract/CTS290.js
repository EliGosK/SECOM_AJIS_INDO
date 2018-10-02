/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/ext/dhtmlxgrid_nxml.js"/>

/*--- Main ---*/
var pageRow;
var gridSearchResultCTS290;

$(document).ready(function () {
    $("#divLastChangeType").hide();

    InitialEvent();
    SetInitialState();

    pageRow = $("#PageRow").val();
    gridSearchResultCTS290 = $("#gridSearchResult").InitialGrid(pageRow, true, "/Contract/CTS290_InitialGrid")
});

function InitialEvent() {
    $("#btnSearch").click(search_button_click);
    $("#btnClear").click(clear_button_click);
    $("#btnDownload").click(download_form);
    //$("#formDownload").submit(download_form);
}

function SetInitialState() {
    //$("#divSpecifySearchCondition").clearForm();
    $("#divSearchResultList").hide();

    $("#WarrantyMonthFrom").val($("#DefaultMonth").val());
    $("#WarrantyYearFrom").val($("#DefaultYear").val());

    $("#WarrantyMonthTo").val($("#DefaultMonth").val());
    $("#WarrantyYearTo").val($("#DefaultYear").val());

    $("#OperationOffice").val("");
    $("#SaleContractOffice").val("");
}

function search_button_click() {
    $("#btnSearch").attr("disabled", true);
    master_event.LockWindow(true);

    var objCond = {
        ExpireWarrantyMonthFrom: $("#WarrantyMonthFrom").val(),
        ExpireWarrantyYearFrom: $("#WarrantyYearFrom").val(),
        ExpireWarrantyMonthTo: $("#WarrantyMonthTo").val(),
        ExpireWarrantyYearTo: $("#WarrantyYearTo").val(),
        OperationOfficeCode: $("#OperationOffice").val(),
        SaleContractOfficeCode: $("#SaleContractOffice").val()
    }

    $("#gridSearchResult").LoadDataToGrid(gridSearchResultCTS290, pageRow, true, "/Contract/CTS290_GetSearchResultData", objCond, "CTS290_SearchResultGridData", false,
        function (result, controls, isWarning) {
            if (controls != undefined) {
                /* --- Highlight Text --- */
                /* --------------------- */
                VaridateCtrl(["WarrantyMonthFrom", "WarrantyYearFrom", "WarrantyMonthTo", "WarrantyYearTo"], controls);

                master_event.LockWindow(false);
                $("#btnSearch").attr("disabled", false);
                return;
            }
            else if (isWarning == undefined) {
                $("#divSpecifySearchCondition").ResetToNormalControl();

                $("#divSearchResultList").show();
                //document.getElementById('divSearchResultList').scrollIntoView();
                master_event.ScrollWindow("#divSearchResultList", false);

                if (CheckFirstRowIsEmpty(gridSearchResultCTS290, false)) {
                    $("#btnDownload").attr("disabled", true);
                }
                else {
                    $("#btnDownload").attr("disabled", false);
                }
            }

            master_event.LockWindow(false);
            $("#btnSearch").attr("disabled", false);
        }
        , function () {
            $("#divSearchResultList").show();
        });

    //gridSearchResultCTS290.setSizes();
}

function clear_button_click() {
    CloseWarningDialog();
    SetInitialState();
}

function download_form() {
    var url = "/Contract/CTS290_DownloadAsCSV";
//    if (url.indexOf("k=") < 0) {

//        var key = ajax_method.GetKeyURL(null);
//        if (key != "") {
//            if (url.indexOf("?") > 0) {
//                url = url + "&k=" + key;
//            }
//            else {
//                url = url + "?k=" + key;
//            }

//        }
//    }

//    url=generate_url(url);
//    $("#ifDownload").get(0).src = url;

    download_method.CallDownloadController("ifDownload", url, null);
}

//function download_button_click() {
//    var csvResult = '';
//    if (CheckFirstRowIsEmpty(gridSearchResultCTS290, false) == false) {
//        gridSearchResultCTS290.enableCSVHeader(true);
//        csvResult = gridSearchResultCTS290.serializeToCSV(true);
//    }

//    var obj = { strCSVResult: csvResult };
//    call_ajax_method_json("/Contract/CTS290_DownloadAsCSV", obj,
//        function (result) {
//            if (result != undefined) {

//            }
//        }
//    );

//}


function cts290_period_custom(a, b, order) {
    var lst = [a, b];
    var dateA = GetDateFromPeriod(a);
    var dateB = GetDateFromPeriod(b);

    if (order == "asc") {
        if (dateA[0] > dateB[0]) {
            return 1;
        }
        else if (dateA[0] = dateB[0]) {
            if (dateA[1] > dateB[1]) {
                return 1;
            }
        }
            
        return -1;
    }
    else {
        if (dateA[0] > dateB[0]) {
            return -1;
        }
        else if (dateA[0] = dateB[0]) {
            if (dateA[1] > dateB[1]) {
                return -1;
            }
        }

        return 1;
    }
}

function GetDateFromPeriod(period) {
    var mm;
    var yy;
    var date = [null, null];

    if (period != null && period.length >= 8) {
        mm = period.substring(0, 3);
        yy = period.substring(4, 9);
        date[0] = new Date(yy + "/" + mm + "/" + 1);

        var b_idx = period.indexOf("<BR>");
        if (b_idx > 0) {
            var toDate = period.substring(b_idx + 4, period.length);

            mm = toDate.substring(0, 3);
            yy = toDate.substring(4, 9);
            date[1] = new Date(yy + "/" + mm + "/" + 1);
        }
        else {
            date[1] = new Date("1000/01/01");
        }
    }
    else {
        date[0] = new Date("1000/01/01");
        date[1] = new Date("1000/01/01");
    }

    return date;
}
