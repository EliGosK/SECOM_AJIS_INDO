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
/// <reference path="../../Content/js/dhtmlxgrid/_dhtmlxcommon.js" />
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js" />


var gridSearchResult;

$(document).ready(function () {

    // Initial grid
    if ($.find("#IVS170_StockDifferenceListGrid").length > 0) {
        gridSearchResult = $("#IVS170_StockDifferenceListGrid").InitialGrid(0, false, "/Inventory/IVS170_InitialSearchResultGrid");

        // mygrid.setRowColor("row1","red");
        BindOnLoadedEvent(gridSearchResult, function () {

            // remove class odd_dhx_secom
            //$("#IVS170_StockDifferenceListGrid .odd_dhx_secom").removeClass("odd_dhx_secom").addClass("ev_dhx_secom");

            var diffCol = gridSearchResult.getColIndexById('DiffQty');

            for (var i = 0; i < gridSearchResult.getRowsNum(); i++) {
                if (CheckFirstRowIsEmpty(gridSearchResult, false) == false) {
                    var rowId = gridSearchResult.getRowId(i);
                    var diffVal = gridSearchResult.cells2(i, diffCol).getValue();

                    diffVal = parseInt(diffVal);
                    if (diffVal < 0) {
                        gridSearchResult.setRowColor(rowId, "#F9C8CF");  // F9C8CF = Pink
                    }
                    else if (diffVal > 0) {
                        gridSearchResult.setRowColor(rowId, "#F9C8CF"); // C7F7C9 = Green , F9C8CF = Pink
                    }
                    else {
                        gridSearchResult.setRowColor(rowId, "#E3EFFF"); // #E3EFFF = Blue
                    }
                }
            }

            

        });

    }

    // Event binding
    $("#btnSearch").click(IVS170_Search);
    $("#btnDownload").click(IVS170_Download);
    $("#btnClear").click(Clear);

    $("#InstrumentCode").InitialAutoComplete("/Master/GetInstrumentCodeAll", null, true);

    // Initial page
    InitialPage();

});

function InitialPage() {
    $("#divStockDiffList").hide();
}

function IVS170_Search() {

    // For prevent click this button more than one time
    $("#btnSearch").attr("disabled", true);
    master_event.LockWindow(true);

    //Remove red hilight
    $("#ShelfNoFrom").removeClass("highlight");
    $("#ShelfNoTo").removeClass("highlight");

    var obj = CreateObjectData($("#formSearchCriteria").serialize());
    obj.OfficeText = $("#OfficeCode option:selected").text()
    obj.LocationText = $("#LocationCode option:selected").text()
    obj.AreaText = $("#AreaCode option:selected").text()
    $("#IVS170_StockDifferenceListGrid").LoadDataToGrid(gridSearchResult, 0, false, "/Inventory/IVS170_SearchResponse", obj, "dtStockCheckingList", false,
                    function (result, controls, isWarning) { // post-load
                        // For prevent click this button more than one time
                        $("#btnSearch").attr("disabled", false);
                        master_event.LockWindow(false);

                        if (controls != undefined) {
                            VaridateCtrl(["ShelfNoFrom", "ShelfNoTo", "CheckingYearMonth"], controls);
                        }
                        else if (result != undefined) {
                            if (CheckFirstRowIsEmpty(gridSearchResult, false) == false) {
                                // Set grouping 
                                var groupingKeyInx = gridSearchResult.getColIndexById("GroupingKey");
                                gridSearchResult.groupBy(groupingKeyInx);
                            }

                            // == tt

                            $(".odd_dhx_secom").removeClass("odd_dhx_secom");
                        }
                    },
                    function (result, controls, isWarning) { // pre-load
                        if (isWarning == undefined) {
                            $("#divStockDiffList").show();


                        }
                    });
}

function IVS170_Download() {

    // For prevent click this button more than one time
    $("#btnDownload").attr("disabled", true);
    master_event.LockWindow(true);

    //Remove red hilight
    $("#ShelfNoFrom").removeClass("highlight");
    $("#ShelfNoTo").removeClass("highlight");

    var obj = CreateObjectData($("#formSearchCriteria").serialize());
    obj.OfficeText = $("#OfficeCode option:selected").text()
    obj.LocationText = $("#LocationCode option:selected").text()
    obj.AreaText = $("#AreaCode option:selected").text()
    ajax_method.CallScreenController("/Inventory/IVS170_GenerateReport", obj, function (result, controls, isWarning) {
        if (result != undefined) {
            download_method.CallDownloadController("ifDownload", "/Inventory/IVS170_Download", null);
        }

        $("#btnDownload").attr("disabled", false);
        master_event.LockWindow(false);
    }, false);
}

function Clear() {
    $("#divStockDiffList").hide();
    $("#divSearchCriteria").clearForm();
    $("#CheckingYearMonth").removeClass("highlight");

    CloseWarningDialog();
}