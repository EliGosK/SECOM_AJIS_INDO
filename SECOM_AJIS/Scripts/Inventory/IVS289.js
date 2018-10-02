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

var IVS289 = {};

IVS289.CtrlID = {
    divSearch: "#divSearch",
    frmSearch: "#frmSearch",
    txtSlipNoFrom: "#txtSlipNoFrom",
    txtSlipNoTo: "#txtSlipNoTo",
    txtTransferDateFrom: "#txtTransferDateFrom",
    txtTransferDateTo: "#txtTransferDateTo",
    cboTransferType: "#cboTransferType",
    btnSearch: "#btnSearch",
    btnClear: "#btnClear",

    lblResultHeaderSuffix: "#lblResultHeaderSuffix",
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

IVS289.Grids = {
    grdSearchResult: null,
    grdSearchResultIsBindEvent: false,
    grdSearchResult_DeleteAllRows: function () {
        if (IVS289.Grids.grdSearchResultIsBindEvent) {
            DeleteAllRow(IVS289.Grids.grdSearchResult);
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
        if (IVS289.Grids.grdDetailIsBindEvent) {
            DeleteAllRow(IVS289.Grids.grdDetail);
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

IVS289.EventHandlers = {
    grdSearchResult_OnLoadedData: function (gen_ctrl) {
        var grid = IVS289.Grids.grdSearchResult;

        for (var i = 0; i < grid.getRowsNum(); i++) {
            var row_id = grid.getRowId(i);
            if (gen_ctrl) {
                GenerateDetailButton(IVS289.Grids.grdSearchResult, IVS289.Grids.grdSearchResultColumns.DetailButton, row_id, IVS289.Grids.grdSearchResultColumns.DetailButton, true);
            }

            var isSelected = grid.cells(row_id, grid.getColIndexById(IVS289.Grids.grdSearchResultColumns.SelectedFlag)).getValue();
            grid.cells(row_id, grid.getColIndexById(IVS289.Grids.grdSearchResultColumns.SelectButton))
                .setValue(GenerateCheckBox2(IVS289.Grids.grdSearchResultColumns.SelectButton, row_id, "", true, isSelected));

            BindGridButtonClickEvent(IVS289.Grids.grdSearchResultColumns.DetailButton, row_id, IVS289.EventHandlers.grdSearchResult_OnDetail);
            BindGridCheckBoxClickEvent(IVS289.Grids.grdSearchResultColumns.SelectButton, row_id, IVS289.EventHandlers.chkSelect_OnClick);
        }

        $(IVS289.CtrlID.chkSelectAll).click(IVS289.EventHandlers.chkSelectAll_OnClick);
        grid.setSizes();
    },

    grdSearchResult_OnDetail: function (row_id) {
        var grid = IVS289.Grids.grdSearchResult;
        var row_index = grid.getRowIndex(row_id);
        grid.selectRow(row_index);

        var slipNo = grid.cells(row_id, grid.getColIndexById(IVS289.Grids.grdSearchResultColumns.SlipNo)).getValue();
        var TransferDate = grid.cells(row_id, grid.getColIndexById(IVS289.Grids.grdSearchResultColumns.TransferDate)).getValue();

        var obj = {
            slipNo: [slipNo]
        };

        $(IVS289.CtrlID.txtSlipNoSelected).val(slipNo);
        $(IVS289.CtrlID.txtTransferDateSelected).val(TransferDate);

        $(IVS289.CtrlID.divDetailGrid).LoadDataToGrid(IVS289.Grids.grdDetail, ROWS_PER_PAGE_FOR_VIEWPAGE, true, "/inventory/IVS289_GetDetail", obj, "dtEliminateReportDetail", false
            , function (res) {
                master_event.ScrollWindow(IVS289.CtrlID.divDetail);
            }
            , function (res) {
                $(IVS289.CtrlID.divDetail).show();
            }
        );

        return false;
    },

    chkSelect_OnClick: function (row_id, checked) {
        var grid = IVS289.Grids.grdSearchResult;
        grid.cells(row_id, grid.getColIndexById(IVS289.Grids.grdSearchResultColumns.SelectedFlag)).setValue(checked ? "1" : "");

        var checkedAll = $(IVS289.CtrlID.chkSelectAll).prop("checked");
        if (checkedAll && !checked) {
            $(IVS289.CtrlID.chkSelectAll).attr("checked", false);
        }
    },

    chkSelectAll_OnClick: function () {
        var checked = $(IVS289.CtrlID.chkSelectAll).prop("checked");
        var grid = IVS289.Grids.grdSearchResult;
        if (!CheckFirstRowIsEmpty(grid)) {
            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);
                var ctrl = "#" + GenerateGridControlID(IVS289.Grids.grdSearchResultColumns.SelectButton, row_id);
                $(ctrl).attr("checked", checked);
                grid.cells(row_id, grid.getColIndexById(IVS289.Grids.grdSearchResultColumns.SelectedFlag)).setValue(checked ? "1" : "");
            }
        }
    },

    grdDetail_OnLoadedData: function (gen_ctrl) {
        var grid = IVS289.Grids.grdDetail;

        grid.setSizes();
    },

    btnSearch_OnClick: function () {
        master_event.LockWindow(true);

        var obj = {
            SlipNoStart: $(IVS289.CtrlID.txtSlipNoFrom).val(),
            SlipNoEnd: $(IVS289.CtrlID.txtSlipNoTo).val(),
            TransferDateStart: $(IVS289.CtrlID.txtTransferDateFrom).val(),
            TransferDateEnd: $(IVS289.CtrlID.txtTransferDateTo).val(),
            TransferType: $(IVS289.CtrlID.cboTransferType).val(),
            TransferTypeText: $(IVS289.CtrlID.cboTransferType + " option:selected").text()
        };

        $(IVS289.CtrlID.divSearchResultGrid).LoadDataToGrid(IVS289.Grids.grdSearchResult, ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/inventory/IVS289_SearchSlip", obj, "dtEliminateReportHeader", false, function (res) {
            $(IVS289.CtrlID.divDetail).hide();
            $(IVS289.CtrlID.chkSelectAll).attr("checked", false);
            $(IVS289.CtrlID.lblResultHeaderSuffix).text((obj.TransferTypeText ? " (" + obj.TransferTypeText + ")" : ""));
            master_event.ScrollWindow(IVS289.CtrlID.divSearchResult);

            master_event.LockWindow(false);
        });

        return false;
    },

    btnClear_OnClick: function () {
        $(IVS289.CtrlID.frmSearch).clearForm();
        DeleteAllRow(IVS289.Grids.grdSearchResult);
        DeleteAllRow(IVS289.Grids.grdDetail);
        $(IVS289.CtrlID.divDetail).hide();
        $(IVS289.CtrlID.chkSelectAll).attr("checked", false);

        ClearDateFromToControl(IVS289.CtrlID.txtTransferDateFrom, IVS289.CtrlID.txtTransferDateTo);

        return false;
    },

    btnDownload_OnClick: function () {
        master_event.LockWindow(true);

        var obj = {
            slipNo: []
        };

        var grid = IVS289.Grids.grdSearchResult;
        if (!CheckFirstRowIsEmpty(grid)) {
            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);
                var checked = grid.cells(row_id, grid.getColIndexById(IVS289.Grids.grdSearchResultColumns.SelectedFlag)).getValue();
                if (checked == "1") {
                    obj.slipNo.push(grid.cells(row_id, grid.getColIndexById(IVS289.Grids.grdSearchResultColumns.SlipNo)).getValue());
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

        ajax_method.CallScreenController("/Inventory/IVS289_GenerateReport", obj, function (result, controls, isWarning) {
            if (result != undefined) {
                download_method.CallDownloadController("ifDownload", "/Inventory/IVS289_Download", null);
            }

            master_event.LockWindow(false);
        }, false);
    },

    btnDownloadSummary_OnClick: function () {
        master_event.LockWindow(true);

        var obj = {
            slipNo: []
        };

        var grid = IVS289.Grids.grdSearchResult;
        for (var i = 0; i < grid.getRowsNum(); i++) {
            var row_id = grid.getRowId(i);
            var checked = grid.cells(row_id, grid.getColIndexById(IVS289.Grids.grdSearchResultColumns.SelectedFlag)).getValue();
            if (checked == "1") {
                obj.slipNo.push(grid.cells(row_id, grid.getColIndexById(IVS289.Grids.grdSearchResultColumns.SlipNo)).getValue());
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

        ajax_method.CallScreenController("/Inventory/IVS289_GenerateReportSummary", obj, function (result, controls, isWarning) {
            if (result != undefined) {
                download_method.CallDownloadController("ifDownload", "/Inventory/IVS289_Download", null);
            }

            master_event.LockWindow(false);
        }, false);
    }
}


IVS289.InitializeGrid = function () {

    IVS289.Grids.grdSearchResult = $(IVS289.CtrlID.divSearchResultGrid).InitialGrid(
        ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/inventory/IVS289_InitialSearchResultGrid"
        , function () {
            BindOnLoadedEvent(IVS289.Grids.grdSearchResult, IVS289.EventHandlers.grdSearchResult_OnLoadedData);
            $(IVS289.CtrlID.chkSelectAll).click(IVS289.EventHandlers.chkSelectAll_OnClick);
            SpecialGridControl(IVS289.Grids.grdSearchResult, [IVS289.Grids.grdSearchResultColumns.DetailButton, IVS289.Grids.grdSearchResultColumns.SelectButton]);
            IVS289.Grids.grdSearchResultIsBindEvent = true;
        }
    );

    IVS289.Grids.grdDetail = $(IVS289.CtrlID.divDetailGrid).InitialGrid(
        ROWS_PER_PAGE_FOR_VIEWPAGE, true, "/inventory/IVS289_InitialDetailGrid"
        , function () {
            BindOnLoadedEvent(IVS289.Grids.grdDetail, IVS289.EventHandlers.grdDetail_OnLoadedData);
            IVS289.Grids.grdDetailIsBindEvent = true;
        }
    );

};


IVS289.InitializeScreen = function () {
    InitialDateFromToControl(IVS289.CtrlID.txtTransferDateFrom, IVS289.CtrlID.txtTransferDateTo);

    $(IVS289.CtrlID.btnSearch).click(IVS289.EventHandlers.btnSearch_OnClick);
    $(IVS289.CtrlID.btnClear).click(IVS289.EventHandlers.btnClear_OnClick);
    $(IVS289.CtrlID.btnDownload).click(IVS289.EventHandlers.btnDownload_OnClick);
    $(IVS289.CtrlID.btnDownloadSummary).click(IVS289.EventHandlers.btnDownloadSummary_OnClick);

    $(IVS289.CtrlID.divSearch).css("visibility", "inherit");
    $(IVS289.CtrlID.divSearchResult).css("visibility", "inherit");
    $(IVS289.CtrlID.divDetail).css("visibility", "inherit");
};

IVS289.ResetScreen = function () {
    $(IVS289.CtrlID.divSearch).show();
    $(IVS289.CtrlID.divDetail).hide();

    $(IVS289.CtrlID.divSearchResult).clearForm();
    $(IVS289.CtrlID.divDetail).clearForm();

    IVS289.Grids.grdSearchResult_DeleteAllRows();
    IVS289.Grids.grdDetail_DeleteAllRows();

    $(IVS289.CtrlID.divSearch).find(".highlight").toggleClass("highlight", false);
    $(IVS289.CtrlID.divSearchResult).find(".highlight").toggleClass("highlight", false);
    $(IVS289.CtrlID.divDetail).find(".highlight").toggleClass("highlight", false);
};

$(document).ready(function () {
    IVS289.InitializeGrid();
    IVS289.InitializeScreen();
    IVS289.ResetScreen();
});
