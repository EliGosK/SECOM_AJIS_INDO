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

var IVS285 = {};

IVS285.CtrlID = {
    divSearch: "#divSearch",
    frmSearch: "#frmSearch",
    cboStockReportType: "#cboStockReportType",
    txtInstrumentCode: "#txtInstrumentCode",
    cboProcessDate: "#cboProcessDate",
    btnSearch: "#btnSearch",
    btnClear: "#btnClear",

    divSearchResult: "#divSearchResult",
    divSearchResultGrid: "#divSearchResultGrid",
    chkSelectAll: "#chkSelectAll",
    btnDownload: "#btnDownload"
};

IVS285.Grids = {
    grdSearchResult: null,
    grdSearchResultIsBindEvent: false,
    grdSearchResult_DeleteAllRows: function () {
        if (IVS285.Grids.grdSearchResultIsBindEvent) {
            DeleteAllRow(IVS285.Grids.grdSearchResult);
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

IVS285.EventHandlers = {
    grdSearchResult_OnLoadedData: function (gen_ctrl) {
        var grid = IVS285.Grids.grdSearchResult;

        for (var i = 0; i < grid.getRowsNum(); i++) {
            var row_id = grid.getRowId(i);

            var isSelected = grid.cells(row_id, grid.getColIndexById(IVS285.Grids.grdSearchResultColumns.SelectedFlag)).getValue();
            grid.cells(row_id, grid.getColIndexById(IVS285.Grids.grdSearchResultColumns.SelectButton))
                .setValue(GenerateCheckBox2(IVS285.Grids.grdSearchResultColumns.SelectButton, row_id, "", true, isSelected));

            BindGridCheckBoxClickEvent(IVS285.Grids.grdSearchResultColumns.SelectButton, row_id, IVS285.EventHandlers.chkSelect_OnClick);
        }

        $(IVS285.CtrlID.chkSelectAll).click(IVS285.EventHandlers.chkSelectAll_OnClick);
        grid.setSizes();
    },

    chkSelect_OnClick: function (row_id, checked) {
        var grid = IVS285.Grids.grdSearchResult;
        grid.cells(row_id, grid.getColIndexById(IVS285.Grids.grdSearchResultColumns.SelectedFlag)).setValue(checked ? "1" : "");

        var checkedAll = $(IVS285.CtrlID.chkSelectAll).prop("checked");
        if (checkedAll && !checked) {
            $(IVS285.CtrlID.chkSelectAll).attr("checked", false);
        }
    },

    chkSelectAll_OnClick: function () {
        var checked = $(IVS285.CtrlID.chkSelectAll).prop("checked");
        var grid = IVS285.Grids.grdSearchResult;
        if (!CheckFirstRowIsEmpty(grid)) {
            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);
                var ctrl = "#" + GenerateGridControlID(IVS285.Grids.grdSearchResultColumns.SelectButton, row_id);
                $(ctrl).attr("checked", checked);
                grid.cells(row_id, grid.getColIndexById(IVS285.Grids.grdSearchResultColumns.SelectedFlag)).setValue(checked ? "1" : "");
            }
        }
    },

    btnSearch_OnClick: function () {
        master_event.LockWindow(true);
        
        var obj = {
            ReportType: $(IVS285.CtrlID.cboStockReportType).val(),
            InstrumentCode: $(IVS285.CtrlID.txtInstrumentCode).val(),
            InstrumentCodeSelected: null,
            YearMonth: $(IVS285.CtrlID.cboProcessDate).val()
        };

        $(IVS285.CtrlID.divSearchResultGrid).LoadDataToGrid(IVS285.Grids.grdSearchResult, ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/inventory/IVS285_SearchData", obj, "dtPhysicalReport", false, function (res) {
            $(IVS285.CtrlID.chkSelectAll).attr("checked", false);
            master_event.ScrollWindow(IVS285.CtrlID.divSearchResult);

            master_event.LockWindow(false);
        });

        return false;
    },

    btnClear_OnClick: function () {
        $(IVS285.CtrlID.frmSearch).clearForm();
        DeleteAllRow(IVS285.Grids.grdSearchResult);
        $(IVS285.CtrlID.chkSelectAll).attr("checked", false);

        return false;
    },

    btnDownload_OnClick: function () {
        master_event.LockWindow(true);

        var obj = {
            ReportType: $(IVS285.CtrlID.cboStockReportType).val(),
            InstrumentCode: $(IVS285.CtrlID.txtInstrumentCode).val(),
            InstrumentCodeSelected: [],
            YearMonth: $(IVS285.CtrlID.cboProcessDate).val()
        };

        var grid = IVS285.Grids.grdSearchResult;
        if (!CheckFirstRowIsEmpty(grid)) {
            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);
                var checked = grid.cells(row_id, grid.getColIndexById(IVS285.Grids.grdSearchResultColumns.SelectedFlag)).getValue();
                if (checked == "1") {
                    obj.InstrumentCodeSelected.push(grid.cells(row_id, grid.getColIndexById(IVS285.Grids.grdSearchResultColumns.InstrumentCode)).getValue());
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

        ajax_method.CallScreenController("/Inventory/IVS285_GenerateReport", obj, function (result, controls, isWarning) {
            if (result != undefined) {
                download_method.CallDownloadController("ifDownload", "/Inventory/IVS285_Download", null);
            }

            master_event.LockWindow(false);
        }, false);
    }
}


IVS285.InitializeGrid = function () {

    IVS285.Grids.grdSearchResult = $(IVS285.CtrlID.divSearchResultGrid).InitialGrid(
        ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/inventory/IVS285_InitialSearchResultGrid"
        , function () {
            BindOnLoadedEvent(IVS285.Grids.grdSearchResult, IVS285.EventHandlers.grdSearchResult_OnLoadedData);
            $(IVS285.CtrlID.chkSelectAll).click(IVS285.EventHandlers.chkSelectAll_OnClick);
            SpecialGridControl(IVS285.Grids.grdSearchResult, [IVS285.Grids.grdSearchResultColumns.SelectButton]);
            IVS285.Grids.grdSearchResultIsBindEvent = true;
        }
    );

};


IVS285.InitializeScreen = function () {
    $(IVS285.CtrlID.btnSearch).click(IVS285.EventHandlers.btnSearch_OnClick);
    $(IVS285.CtrlID.btnClear).click(IVS285.EventHandlers.btnClear_OnClick);
    $(IVS285.CtrlID.btnDownload).click(IVS285.EventHandlers.btnDownload_OnClick);

    $(IVS285.CtrlID.divSearch).css("visibility", "inherit");
    $(IVS285.CtrlID.divSearchResult).css("visibility", "inherit");
};

IVS285.ResetScreen = function () {
    $(IVS285.CtrlID.divSearch).show();

    $(IVS285.CtrlID.divProject).clearForm();
    $(IVS285.CtrlID.divSearchResult).clearForm();

    IVS285.Grids.grdSearchResult_DeleteAllRows();

    $(IVS285.CtrlID.divSearch).find(".highlight").toggleClass("highlight", false);
    $(IVS285.CtrlID.divSearchResult).find(".highlight").toggleClass("highlight", false);
};

$(document).ready(function () {
    IVS285.InitializeGrid();
    IVS285.InitializeScreen();
    IVS285.ResetScreen();
});
