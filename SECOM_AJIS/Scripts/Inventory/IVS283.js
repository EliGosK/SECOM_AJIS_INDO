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

var IVS283 = {};

IVS283.CtrlID = {
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

IVS283.Grids = {
    grdSearchResult: null,
    grdSearchResultIsBindEvent: false,
    grdSearchResult_DeleteAllRows: function () {
        if (IVS283.Grids.grdSearchResultIsBindEvent) {
            DeleteAllRow(IVS283.Grids.grdSearchResult);
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

IVS283.EventHandlers = {
    grdSearchResult_OnLoadedData: function (gen_ctrl) {
        var grid = IVS283.Grids.grdSearchResult;

        for (var i = 0; i < grid.getRowsNum(); i++) {
            var row_id = grid.getRowId(i);

            var isSelected = grid.cells(row_id, grid.getColIndexById(IVS283.Grids.grdSearchResultColumns.SelectedFlag)).getValue();
            grid.cells(row_id, grid.getColIndexById(IVS283.Grids.grdSearchResultColumns.SelectButton))
                .setValue(GenerateCheckBox2(IVS283.Grids.grdSearchResultColumns.SelectButton, row_id, "", true, isSelected));

            BindGridCheckBoxClickEvent(IVS283.Grids.grdSearchResultColumns.SelectButton, row_id, IVS283.EventHandlers.chkSelect_OnClick);
        }

        $(IVS283.CtrlID.chkSelectAll).click(IVS283.EventHandlers.chkSelectAll_OnClick);
        grid.setSizes();
    },

    chkSelect_OnClick: function (row_id, checked) {
        var grid = IVS283.Grids.grdSearchResult;
        grid.cells(row_id, grid.getColIndexById(IVS283.Grids.grdSearchResultColumns.SelectedFlag)).setValue(checked ? "1" : "");

        var checkedAll = $(IVS283.CtrlID.chkSelectAll).prop("checked");
        if (checkedAll && !checked) {
            $(IVS283.CtrlID.chkSelectAll).attr("checked", false);
        }
    },

    chkSelectAll_OnClick: function () {
        var checked = $(IVS283.CtrlID.chkSelectAll).prop("checked");
        var grid = IVS283.Grids.grdSearchResult;
        if (!CheckFirstRowIsEmpty(grid)) {
            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);
                var ctrl = "#" + GenerateGridControlID(IVS283.Grids.grdSearchResultColumns.SelectButton, row_id);
                $(ctrl).attr("checked", checked);
                grid.cells(row_id, grid.getColIndexById(IVS283.Grids.grdSearchResultColumns.SelectedFlag)).setValue(checked ? "1" : "");
            }
        }
    },

    btnSearch_OnClick: function () {
        master_event.LockWindow(true);
        
        var obj = {
            ReportType: $(IVS283.CtrlID.cboStockReportType).val(),
            InstrumentCode: $(IVS283.CtrlID.txtInstrumentCode).val(),
            InstrumentCodeSelected: null,
            YearMonth: $(IVS283.CtrlID.cboProcessDate).val()
        };

        $(IVS283.CtrlID.divSearchResultGrid).LoadDataToGrid(IVS283.Grids.grdSearchResult, ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/inventory/IVS283_SearchData", obj, "dtInstrumentForMovementReport", false, function (res) {
            $(IVS283.CtrlID.chkSelectAll).attr("checked", false);
            master_event.ScrollWindow(IVS283.CtrlID.divSearchResult);

            master_event.LockWindow(false);
        });

        return false;
    },

    btnClear_OnClick: function () {
        $(IVS283.CtrlID.frmSearch).clearForm();
        DeleteAllRow(IVS283.Grids.grdSearchResult);
        $(IVS283.CtrlID.chkSelectAll).attr("checked", false);
        return false;
    },

    btnDownload_OnClick: function () {
        master_event.LockWindow(true);

        var obj = {
            ReportType: $(IVS283.CtrlID.cboStockReportType).val(),
            InstrumentCode: $(IVS283.CtrlID.txtInstrumentCode).val(),
            InstrumentCodeSelected: [],
            YearMonth: $(IVS283.CtrlID.cboProcessDate).val()
        };

        var grid = IVS283.Grids.grdSearchResult;
        if (!CheckFirstRowIsEmpty(grid)) {
            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);
                var checked = grid.cells(row_id, grid.getColIndexById(IVS283.Grids.grdSearchResultColumns.SelectedFlag)).getValue();
                if (checked == "1") {
                    obj.InstrumentCodeSelected.push(grid.cells(row_id, grid.getColIndexById(IVS283.Grids.grdSearchResultColumns.InstrumentCode)).getValue());
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

        ajax_method.CallScreenController("/Inventory/IVS283_GenerateReport", obj, function (result, controls, isWarning) {
            if (result != undefined) {
                download_method.CallDownloadController("ifDownload", "/Inventory/IVS283_Download", null);
            }

            master_event.LockWindow(false);
        }, false);
    }
}


IVS283.InitializeGrid = function () {

    IVS283.Grids.grdSearchResult = $(IVS283.CtrlID.divSearchResultGrid).InitialGrid(
        ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/inventory/IVS283_InitialSearchResultGrid"
        , function () {
            BindOnLoadedEvent(IVS283.Grids.grdSearchResult, IVS283.EventHandlers.grdSearchResult_OnLoadedData);
            $(IVS283.CtrlID.chkSelectAll).click(IVS283.EventHandlers.chkSelectAll_OnClick);
            SpecialGridControl(IVS283.Grids.grdSearchResult, [IVS283.Grids.grdSearchResultColumns.SelectButton]);
            IVS283.Grids.grdSearchResultIsBindEvent = true;
        }
    );

};


IVS283.InitializeScreen = function () {
    $(IVS283.CtrlID.btnSearch).click(IVS283.EventHandlers.btnSearch_OnClick);
    $(IVS283.CtrlID.btnClear).click(IVS283.EventHandlers.btnClear_OnClick);
    $(IVS283.CtrlID.btnDownload).click(IVS283.EventHandlers.btnDownload_OnClick);

    $(IVS283.CtrlID.divSearch).css("visibility", "inherit");
    $(IVS283.CtrlID.divSearchResult).css("visibility", "inherit");
};

IVS283.ResetScreen = function () {
    $(IVS283.CtrlID.divSearch).show();

    $(IVS283.CtrlID.divProject).clearForm();
    $(IVS283.CtrlID.divSearchResult).clearForm();

    IVS283.Grids.grdSearchResult_DeleteAllRows();

    $(IVS283.CtrlID.divSearch).find(".highlight").toggleClass("highlight", false);
    $(IVS283.CtrlID.divSearchResult).find(".highlight").toggleClass("highlight", false);
};

$(document).ready(function () {
    IVS283.InitializeGrid();
    IVS283.InitializeScreen();
    IVS283.ResetScreen();
});
