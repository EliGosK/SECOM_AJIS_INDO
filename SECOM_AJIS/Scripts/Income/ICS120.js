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

var ICS120 = {

    GridColumnID: {
        SearchWHT: {
            No: "No",
            WHTNo: "WHTNo",
            PaymentTransNo: "PaymentTransNo",
            Payer: "Payer",
            VATRegistantName: "VATRegistantName",
            DocumentDate: "DocumentDate",
            Status: "Status",
            SelectButton: "SelectButton"
        }
    },

    CtrlID: {
        divSearchWHT: "#divSearchWHT",
        formSearchWHT: "#formSearchWHT",
        txtWHTNo: "#txtWHTNo",
        txtPaymentTransNo: "#txtPaymentTransNo",
        txtPayerName: "#txtPayerName",
        txtVATRegistantName: "#txtVATRegistantName",
        txtDocumentDateFrom: "#txtDocumentDateFrom",
        txtDocumentDateTo: "#txtDocumentDateTo",
        btnSearch: "#btnSearch",
        btnClear: "#btnClear",
        divSearchResultGrid: "#divSearchResultGrid"
    },

    Grids: {
        grdSearchResult: null
    },

    EventHandlers: {
        divSearchResult_OnLoadedData: function () {
            var grid = ICS120.Grids.grdSearchResult;

            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);
                grid.GenerateSelectButton(row_id);

                var status = grid.cells(row_id, grid.getColIndexById(ICS120.GridColumnID.SearchWHT.Status)).getValue();
                status = (status == "Completed" ? MATCHSTATUS_COMPLETED : MATCHSTATUS_PARTIAL);
                grid.cells(row_id, grid.getColIndexById(ICS120.GridColumnID.SearchWHT.Status)).setValue(status);
            }
            grid.setSizes();
        },

        btnSearch_OnClick: function () {
            var params = $(ICS120.CtrlID.formSearchWHT).serializeObject2();

            $(ICS120.CtrlID.btnSearch).attr("disabled", true);
            master_event.LockWindow(true);

            $(ICS120.CtrlID.divSearchResultGrid).LoadDataToGrid(
                ICS120.Grids.grdSearchResult,
                ROWS_PER_PAGE_FOR_SEARCHPAGE,
                true,
                "/Income/ICS120_SearchIncomeWHT",
                params,
                "doIncomeWHT",
                false,
                function (result, controls, isWarning) { //post-load
                    $(ICS120.CtrlID.btnSearch).removeAttr("disabled");
                    master_event.LockWindow(false);
                },
                null
            );
        },

        btnClear_OnClick: function () {
            ICS120.ClearScreen();
            return false;
        }
    },

    ClearScreen: function () {
        $(ICS120.CtrlID.divSearchWHT).clearForm();
        ClearDateFromToControl(ICS120.CtrlID.txtDocumentDateFrom, ICS120.CtrlID.txtDocumentDateTo);

        if (ICS120.Grids.grdSearchResult != null
            && !CheckFirstRowIsEmpty(ICS120.Grids.grdSearchResult)
            && ICS120.Grids.grdSearchResult.getRowsNum() > 0) {
            DeleteAllRow(ICS120.Grids.grdSearchResult);
            ICS120.Grids.grdSearchResult.setSizes();
        }
    },

    InitializeGrid: function () {
        ICS120.Grids.grdSearchResult = $(ICS120.CtrlID.divSearchResultGrid).InitialGrid(
            ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/income/ICS120_InitialSearchIncomeWHT"
            , function () {
                BindOnLoadedEvent(ICS120.Grids.grdSearchResult, ICS120.EventHandlers.divSearchResult_OnLoadedData);
                SpecialGridControl(ICS120.Grids.grdSearchResult, [ICS120.GridColumnID.SearchWHT.SelectButton]);
            }
        );

        ICS120.Grids.grdSearchResult.GenerateSelectButton = function (row_id) {
            var grid = ICS120.Grids.grdSearchResult;

            GenerateSelectButton(grid, ICS120.GridColumnID.SearchWHT.SelectButton, row_id, ICS120.GridColumnID.SearchWHT.SelectButton, true);
            BindGridButtonClickEvent(ICS120.GridColumnID.SearchWHT.SelectButton, row_id, function (rid) {
                var whtno = grid.cells(rid, grid.getColIndexById(ICS120.GridColumnID.SearchWHT.WHTNo)).getValue();
                var obj = {
                    LoadWHTNo: whtno 
                };
                ajax_method.CallScreenControllerWithAuthority("/Income/ICS110", obj, true);
            });
        };

    },

    InitializeScreen: function () {
        ClearDateFromToControl(ICS120.CtrlID.txtDocumentDateFrom, ICS120.CtrlID.txtDocumentDateTo);
        InitialDateFromToControl(ICS120.CtrlID.txtDocumentDateFrom, ICS120.CtrlID.txtDocumentDateTo);

        $(ICS120.CtrlID.btnSearch).click(ICS120.EventHandlers.btnSearch_OnClick);
        $(ICS120.CtrlID.btnClear).click(ICS120.EventHandlers.btnClear_OnClick);
    }
};

$(document).ready(function () {
    ICS120.InitializeGrid();
    ICS120.InitializeScreen();
});
