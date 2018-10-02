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

var IVS290 = {};

IVS290.CtrlID = {
    divSearch: "#divSearch",
    frmSearch: "#frmSearch",
    txtSlipNoFrom: "#txtSlipNoFrom",
    txtSlipNoTo: "#txtSlipNoTo",
    txtTransferDateFrom: "#txtTransferDateFrom",
    txtTransferDateTo: "#txtTransferDateTo",
    btnSearch: "#btnSearch",
    btnClear: "#btnClear",

    divSearchResult: "#divSearchResult",
    divSearchResultGrid: "#divSearchResultGrid",
    chkSelectAll: "#chkSelectAll",
    btnDownload: "#btnDownload",
    btnDownloadSummary: "#btnDownloadSummary",

    divDetail: "#divDetail",
    divDetailGrid: "#divDetailGrid",
    txtSlipNoSelected: "#txtSlipNoSelected",
    txtTransferDateSelected: "#txtTransferDateSelected"
};

IVS290.Grids = {
    grdSearchResult: null,
    grdSearchResultIsBindEvent: false,
    grdSearchResult_DeleteAllRows: function () {
        if (IVS290.Grids.grdSearchResultIsBindEvent) {
            DeleteAllRow(IVS290.Grids.grdSearchResult);
        }
    },
    grdSearchResultColumns: {
        No: "No",
        SlipNo: "SlipNo",
        TransferDate: "TransferDate",
        Qty: "Qty",
        Amount: "Amount",
        DetailButton: "DetailButton",
        SelectButton: "SelectButton",
        SelectedFlag: "SelectedFlag"
    },

    grdDetail: null,
    grdDetailIsBindEvent: false,
    grdDetail_DeleteAllRows: function () {
        if (IVS290.Grids.grdDetailIsBindEvent) {
            DeleteAllRow(IVS290.Grids.grdDetail);
        }
    },
    grdDetailColumns: {
        No: "No",
        InstrumentCode: "InstrumentCode",
        Qty: "Qty",
        Cost: "Cost",
        Total: "Total",
        SlipNo: "SlipNo",
        StockOutDate: "StockOutDate"
    }
};

IVS290.EventHandlers = {
    grdSearchResult_OnLoadedData: function (gen_ctrl) {
        var grid = IVS290.Grids.grdSearchResult;

        for (var i = 0; i < grid.getRowsNum(); i++) {
            var row_id = grid.getRowId(i);
            if (gen_ctrl) {
                GenerateDetailButton(IVS290.Grids.grdSearchResult, IVS290.Grids.grdSearchResultColumns.DetailButton, row_id, IVS290.Grids.grdSearchResultColumns.DetailButton, true);
            }

            var isSelected = grid.cells(row_id, grid.getColIndexById(IVS290.Grids.grdSearchResultColumns.SelectedFlag)).getValue();
            grid.cells(row_id, grid.getColIndexById(IVS290.Grids.grdSearchResultColumns.SelectButton))
                .setValue(GenerateCheckBox2(IVS290.Grids.grdSearchResultColumns.SelectButton, row_id, "", true, isSelected));

            BindGridButtonClickEvent(IVS290.Grids.grdSearchResultColumns.DetailButton, row_id, IVS290.EventHandlers.grdSearchResult_OnDetail);
            BindGridCheckBoxClickEvent(IVS290.Grids.grdSearchResultColumns.SelectButton, row_id, IVS290.EventHandlers.chkSelect_OnClick);
        }

        $(IVS290.CtrlID.chkSelectAll).click(IVS290.EventHandlers.chkSelectAll_OnClick);
        grid.setSizes();
    },

    grdSearchResult_OnDetail: function (row_id) {
        var grid = IVS290.Grids.grdSearchResult;
        var row_index = grid.getRowIndex(row_id);
        grid.selectRow(row_index);

        var slipNo = grid.cells(row_id, grid.getColIndexById(IVS290.Grids.grdSearchResultColumns.SlipNo)).getValue();
        var TransferDate = grid.cells(row_id, grid.getColIndexById(IVS290.Grids.grdSearchResultColumns.TransferDate)).getValue();

        var obj = {
            slipNo: [slipNo]
        };

        $(IVS290.CtrlID.txtSlipNoSelected).val(slipNo);
        $(IVS290.CtrlID.txtTransferDateSelected).val(TransferDate);

        $(IVS290.CtrlID.divDetailGrid).LoadDataToGrid(IVS290.Grids.grdDetail, ROWS_PER_PAGE_FOR_VIEWPAGE, true, "/inventory/IVS290_GetDetail", obj, "dtBufferLossReportDetail", false
            , function (res) {
                master_event.ScrollWindow(IVS290.CtrlID.divDetail);
            }
            , function (res) {
                $(IVS290.CtrlID.divDetail).show();
            }
        );

        return false;
    },

    chkSelect_OnClick: function (row_id, checked) {
        var grid = IVS290.Grids.grdSearchResult;
        grid.cells(row_id, grid.getColIndexById(IVS290.Grids.grdSearchResultColumns.SelectedFlag)).setValue(checked ? "1" : "");

        var checkedAll = $(IVS290.CtrlID.chkSelectAll).prop("checked");
        if (checkedAll && !checked) {
            $(IVS290.CtrlID.chkSelectAll).attr("checked", false);
        }
    },

    chkSelectAll_OnClick: function () {
        var checked = $(IVS290.CtrlID.chkSelectAll).prop("checked");
        var grid = IVS290.Grids.grdSearchResult;
        if (!CheckFirstRowIsEmpty(grid)) {
            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);
                var ctrl = "#" + GenerateGridControlID(IVS290.Grids.grdSearchResultColumns.SelectButton, row_id);
                $(ctrl).attr("checked", checked);
                grid.cells(row_id, grid.getColIndexById(IVS290.Grids.grdSearchResultColumns.SelectedFlag)).setValue(checked ? "1" : "");
            }
        }
    },

    grdDetail_OnLoadedData: function (gen_ctrl) {
        var grid = IVS290.Grids.grdDetail;

        grid.setSizes();
    },

    btnSearch_OnClick: function () {
        master_event.LockWindow(true);

        var obj = {
            SlipNoStart: $(IVS290.CtrlID.txtSlipNoFrom).val(),
            SlipNoEnd: $(IVS290.CtrlID.txtSlipNoTo).val(),
            TransferDateStart: $(IVS290.CtrlID.txtTransferDateFrom).val(),
            TransferDateEnd: $(IVS290.CtrlID.txtTransferDateTo).val()
        };

        $(IVS290.CtrlID.divSearchResultGrid).LoadDataToGrid(IVS290.Grids.grdSearchResult, ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/inventory/IVS290_SearchSlip", obj, "dtBufferLossReportHeader", false, function (res) {
            $(IVS290.CtrlID.divDetail).hide();
            $(IVS290.CtrlID.chkSelectAll).attr("checked", false);
            master_event.ScrollWindow(IVS290.CtrlID.divSearchResult);

            master_event.LockWindow(false);
        });

        return false;
    },

    btnClear_OnClick: function () {
        $(IVS290.CtrlID.frmSearch).clearForm();
        DeleteAllRow(IVS290.Grids.grdSearchResult);
        DeleteAllRow(IVS290.Grids.grdDetail);
        $(IVS290.CtrlID.divDetail).hide();
        $(IVS290.CtrlID.chkSelectAll).attr("checked", false);

        ClearDateFromToControl(IVS290.CtrlID.txtTransferDateFrom, IVS290.CtrlID.txtTransferDateTo);

        return false;
    },

    btnDownload_OnClick: function () {
        master_event.LockWindow(true);

        var obj = {
            slipNo: []
        };

        var grid = IVS290.Grids.grdSearchResult;
        if (!CheckFirstRowIsEmpty(grid)) {
            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);
                var checked = grid.cells(row_id, grid.getColIndexById(IVS290.Grids.grdSearchResultColumns.SelectedFlag)).getValue();
                if (checked == "1") {
                    obj.slipNo.push(grid.cells(row_id, grid.getColIndexById(IVS290.Grids.grdSearchResultColumns.SlipNo)).getValue());
                }
            }
        }

        if (obj.slipNo.length <= 0) {
            var messageParam = { "module": "Common", "code": "MSG0161", "param": "" };
            call_ajax_method("/Shared/GetMessage", messageParam, function (data) {
                OpenWarningDialog(data.Message);
            });

            master_event.LockWindow(false);
            return;
        }

        ajax_method.CallScreenController("/Inventory/IVS290_GenerateReport", obj, function (result, controls, isWarning) {
            if (result != undefined) {
                download_method.CallDownloadController("ifDownload", "/Inventory/IVS290_Download", null);
            }

            master_event.LockWindow(false);
        }, false);
    },

    btnDownloadSummary_OnClick: function () {
        master_event.LockWindow(true);

        var obj = {
            slipNo: []
        };

        var grid = IVS290.Grids.grdSearchResult;
        for (var i = 0; i < grid.getRowsNum(); i++) {
            var row_id = grid.getRowId(i);
            var checked = grid.cells(row_id, grid.getColIndexById(IVS290.Grids.grdSearchResultColumns.SelectedFlag)).getValue();
            if (checked == "1") {
                obj.slipNo.push(grid.cells(row_id, grid.getColIndexById(IVS290.Grids.grdSearchResultColumns.SlipNo)).getValue());
            }
        }

        if (obj.slipNo.length <= 0) {
            var messageParam = { "module": "Common", "code": "MSG0161", "param": "" };
            call_ajax_method("/Shared/GetMessage", messageParam, function (data) {
                OpenWarningDialog(data.Message);
            });

            master_event.LockWindow(false);
            return;
        }

        ajax_method.CallScreenController("/Inventory/IVS290_GenerateReportSummary", obj, function (result, controls, isWarning) {
            if (result != undefined) {
                download_method.CallDownloadController("ifDownload", "/Inventory/IVS290_Download", null);
            }

            master_event.LockWindow(false);
        }, false);
    }
}


IVS290.InitializeGrid = function () {

    IVS290.Grids.grdSearchResult = $(IVS290.CtrlID.divSearchResultGrid).InitialGrid(
        ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/inventory/IVS290_InitialSearchResultGrid"
        , function () {
            BindOnLoadedEvent(IVS290.Grids.grdSearchResult, IVS290.EventHandlers.grdSearchResult_OnLoadedData);
            $(IVS290.CtrlID.chkSelectAll).click(IVS290.EventHandlers.chkSelectAll_OnClick);
            SpecialGridControl(IVS290.Grids.grdSearchResult, [IVS290.Grids.grdSearchResultColumns.DetailButton, IVS290.Grids.grdSearchResultColumns.SelectButton]);
            IVS290.Grids.grdSearchResultIsBindEvent = true;
        }
    );

    IVS290.Grids.grdDetail = $(IVS290.CtrlID.divDetailGrid).InitialGrid(
        ROWS_PER_PAGE_FOR_VIEWPAGE, true, "/inventory/IVS290_InitialDetailGrid"
        , function () {
            BindOnLoadedEvent(IVS290.Grids.grdDetail, IVS290.EventHandlers.grdDetail_OnLoadedData);
            IVS290.Grids.grdDetailIsBindEvent = true;
        }
    );

};


IVS290.InitializeScreen = function () {
    InitialDateFromToControl(IVS290.CtrlID.txtTransferDateFrom, IVS290.CtrlID.txtTransferDateTo);

    $(IVS290.CtrlID.btnSearch).click(IVS290.EventHandlers.btnSearch_OnClick);
    $(IVS290.CtrlID.btnClear).click(IVS290.EventHandlers.btnClear_OnClick);
    $(IVS290.CtrlID.btnDownload).click(IVS290.EventHandlers.btnDownload_OnClick);
    $(IVS290.CtrlID.btnDownloadSummary).click(IVS290.EventHandlers.btnDownloadSummary_OnClick);

    $(IVS290.CtrlID.divSearch).css("visibility", "inherit");
    $(IVS290.CtrlID.divSearchResult).css("visibility", "inherit");
    $(IVS290.CtrlID.divDetail).css("visibility", "inherit");
};

IVS290.ResetScreen = function () {
    $(IVS290.CtrlID.divSearch).show();
    $(IVS290.CtrlID.divDetail).hide();

    $(IVS290.CtrlID.divSearchResult).clearForm();
    $(IVS290.CtrlID.divDetail).clearForm();

    IVS290.Grids.grdSearchResult_DeleteAllRows();
    IVS290.Grids.grdDetail_DeleteAllRows();

    $(IVS290.CtrlID.divSearch).find(".highlight").toggleClass("highlight", false);
    $(IVS290.CtrlID.divSearchResult).find(".highlight").toggleClass("highlight", false);
    $(IVS290.CtrlID.divDetail).find(".highlight").toggleClass("highlight", false);
};

$(document).ready(function () {
    IVS290.InitializeGrid();
    IVS290.InitializeScreen();
    IVS290.ResetScreen();
});
