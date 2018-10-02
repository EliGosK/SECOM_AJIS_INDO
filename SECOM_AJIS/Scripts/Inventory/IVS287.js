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

var IVS287 = {};

IVS287.CtrlID = {
    divSearch: "#divSearch",
    frmSearch: "#frmSearch",
    cboStockReportType: "#cboStockReportType",
    txtInstrumentCode: "#txtInstrumentCode",
    btnSearch: "#btnSearch",
    btnClear: "#btnClear",

    divSearchResult: "#divSearchResult",
    divSearchResultGrid: "#divSearchResultGrid",
    chkSelectAll: "#chkSelectAll",
    btnDownload: "#btnDownload"
};

IVS287.Grids = {
    grdSearchResult: null,
    grdSearchResultIsBindEvent: false,
    grdSearchResult_DeleteAllRows: function () {
        if (IVS287.Grids.grdSearchResultIsBindEvent) {
            DeleteAllRow(IVS287.Grids.grdSearchResult);
        }
    },
    grdSearchResultColumns: {
        No: "No",
        InstrumentCode: "InstrumentCode",
        Qty: "Qty",
        Cost: "Cost",
        Total: "Total",
        SelectButton: "SelectButton",
        SelectedFlag: "SelectedFlag"
    }
};

IVS287.EventHandlers = {
    grdSearchResult_OnLoadedData: function (gen_ctrl) {
        var grid = IVS287.Grids.grdSearchResult;

        for (var i = 0; i < grid.getRowsNum(); i++) {
            var row_id = grid.getRowId(i);

            var isSelected = grid.cells(row_id, grid.getColIndexById(IVS287.Grids.grdSearchResultColumns.SelectedFlag)).getValue();
            grid.cells(row_id, grid.getColIndexById(IVS287.Grids.grdSearchResultColumns.SelectButton))
                .setValue(GenerateCheckBox2(IVS287.Grids.grdSearchResultColumns.SelectButton, row_id, "", true, isSelected));

            BindGridCheckBoxClickEvent(IVS287.Grids.grdSearchResultColumns.SelectButton, row_id, IVS287.EventHandlers.chkSelect_OnClick);
        }

        $(IVS287.CtrlID.chkSelectAll).click(IVS287.EventHandlers.chkSelectAll_OnClick);
        grid.setSizes();
    },

    chkSelect_OnClick: function (row_id, checked) {
        var grid = IVS287.Grids.grdSearchResult;
        grid.cells(row_id, grid.getColIndexById(IVS287.Grids.grdSearchResultColumns.SelectedFlag)).setValue(checked ? "1" : "");

        var checkedAll = $(IVS287.CtrlID.chkSelectAll).prop("checked");
        if (checkedAll && !checked) {
            $(IVS287.CtrlID.chkSelectAll).attr("checked", false);
        }
    },

    chkSelectAll_OnClick: function () {
        var checked = $(IVS287.CtrlID.chkSelectAll).prop("checked");
        var grid = IVS287.Grids.grdSearchResult;
        if (!CheckFirstRowIsEmpty(grid)) {
            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);
                var ctrl = "#" + GenerateGridControlID(IVS287.Grids.grdSearchResultColumns.SelectButton, row_id);
                $(ctrl).attr("checked", checked);
                grid.cells(row_id, grid.getColIndexById(IVS287.Grids.grdSearchResultColumns.SelectedFlag)).setValue(checked ? "1" : "");
            }
        }
    },

    btnSearch_OnClick: function () {
        master_event.LockWindow(true);
        
        var obj = {
            ReportType: $(IVS287.CtrlID.cboStockReportType).val(),
            InstrumentCode: $(IVS287.CtrlID.txtInstrumentCode).val(),
            InstrumentCodeSelected: null
        };

        $(IVS287.CtrlID.divSearchResultGrid).LoadDataToGrid(IVS287.Grids.grdSearchResult, ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/inventory/IVS287_SearchData", obj, "dtStockListReport", false, function (res) {
            $(IVS287.CtrlID.chkSelectAll).attr("checked", false);
            master_event.ScrollWindow(IVS287.CtrlID.divSearchResult);

            master_event.LockWindow(false);
        });

        return false;
    },

    btnClear_OnClick: function () {
        $(IVS287.CtrlID.frmSearch).clearForm();
        DeleteAllRow(IVS287.Grids.grdSearchResult);
        $(IVS287.CtrlID.chkSelectAll).attr("checked", false);
        return false;
    },

    btnDownload_OnClick: function () {
        master_event.LockWindow(true);

        var obj = {
            ReportType: $(IVS287.CtrlID.cboStockReportType).val(),
            InstrumentCode: $(IVS287.CtrlID.txtInstrumentCode).val(),
            InstrumentCodeSelected: []
        };

        var grid = IVS287.Grids.grdSearchResult;
        if (!CheckFirstRowIsEmpty(grid)) {
            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);
                var checked = grid.cells(row_id, grid.getColIndexById(IVS287.Grids.grdSearchResultColumns.SelectedFlag)).getValue();
                if (checked == "1") {
                    obj.InstrumentCodeSelected.push(grid.cells(row_id, grid.getColIndexById(IVS287.Grids.grdSearchResultColumns.InstrumentCode)).getValue());
                }
            }
        }

        if (obj.InstrumentCodeSelected.length <= 0) {
            var messageParam = { "module": "Common", "code": "MSG0161", "param": "" };
            call_ajax_method("/Shared/GetMessage", messageParam, function (data) {
                OpenWarningDialog(data.Message);
            });

            master_event.LockWindow(false);
            return;
        }

        ajax_method.CallScreenController("/Inventory/IVS287_GenerateReport", obj, function (result, controls, isWarning) {
            if (result != undefined) {
                download_method.CallDownloadController("ifDownload", "/Inventory/IVS287_Download", null);
            }

            master_event.LockWindow(false);
        }, false);
    }
}


IVS287.InitializeGrid = function () {

    IVS287.Grids.grdSearchResult = $(IVS287.CtrlID.divSearchResultGrid).InitialGrid(
        ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/inventory/IVS287_InitialSearchResultGrid"
        , function () {
            BindOnLoadedEvent(IVS287.Grids.grdSearchResult, IVS287.EventHandlers.grdSearchResult_OnLoadedData);
            $(IVS287.CtrlID.chkSelectAll).click(IVS287.EventHandlers.chkSelectAll_OnClick);
            SpecialGridControl(IVS287.Grids.grdSearchResult, [IVS287.Grids.grdSearchResultColumns.SelectButton]);
            IVS287.Grids.grdSearchResultIsBindEvent = true;
        }
    );

};


IVS287.InitializeScreen = function () {
    $(IVS287.CtrlID.btnSearch).click(IVS287.EventHandlers.btnSearch_OnClick);
    $(IVS287.CtrlID.btnClear).click(IVS287.EventHandlers.btnClear_OnClick);
    $(IVS287.CtrlID.btnDownload).click(IVS287.EventHandlers.btnDownload_OnClick);

    $(IVS287.CtrlID.divSearch).css("visibility", "inherit");
    $(IVS287.CtrlID.divSearchResult).css("visibility", "inherit");
};

IVS287.ResetScreen = function () {
    $(IVS287.CtrlID.divSearch).show();

    $(IVS287.CtrlID.divProject).clearForm();
    $(IVS287.CtrlID.divSearchResult).clearForm();

    IVS287.Grids.grdSearchResult_DeleteAllRows();

    $(IVS287.CtrlID.divSearch).find(".highlight").toggleClass("highlight", false);
    $(IVS287.CtrlID.divSearchResult).find(".highlight").toggleClass("highlight", false);
};

$(document).ready(function () {
    IVS287.InitializeGrid();
    IVS287.InitializeScreen();
    IVS287.ResetScreen();
});
