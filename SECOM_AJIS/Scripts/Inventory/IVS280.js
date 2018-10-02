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

var IVS280 = {};

IVS280.CtrlID = {
    divSearch: "#divSearch",
    frmSearch: "#frmSearch",
    cboStockReportType: "#cboStockReportType",
    txtSlipNoFrom: "#txtSlipNoFrom",
    txtSlipNoTo: "#txtSlipNoTo",
    txtStockInDateFrom: "#txtStockInDateFrom",
    txtStockInDateTo: "#txtStockInDateTo",
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
    txtStockInDateSelected: "#txtStockInDateSelected",
    txtSupplierNameSelected: "#txtSupplierNameSelected",
    txtInvoiceNoSelected: "#txtInvoiceNoSelected",
    txtTotalQty: "#txtTotalQty",
    txtTotalAmount: "#txtTotalAmount"
};

IVS280.Grids = {
    grdSearchResult: null,
    grdSearchResultIsBindEvent: false,
    grdSearchResult_DeleteAllRows: function () {
        if (IVS280.Grids.grdSearchResultIsBindEvent) {
            DeleteAllRow(IVS280.Grids.grdSearchResult);
        }
    },
    grdSearchResultColumns: {
        No: "No",
        SlipNo: "SlipNo",
        StockInDate: "StockInDate",
        StockInType: "StockInType",
        SupplierNameEN: "SupplierNameEN",
        InvoiceNo: "InvoiceNo",
        DetailButton: "DetailButton",
        SelectButton: "SelectButton",
        SelectedFlag: "SelectedFlag"
    },

    grdDetail: null,
    grdDetailIsBindEvent: false,
    grdDetail_DeleteAllRows: function () {
        if (IVS280.Grids.grdDetailIsBindEvent) {
            DeleteAllRow(IVS280.Grids.grdDetail);
        }
    },
    grdDetailColumns: {
        No: "No",
        InstrumentCode: "InstrumentCode",
        Qty: "Qty",
        Cost: "Cost",
        TotalUsD: "TotalUsD"
    }
};

IVS280.EventHandlers = {
    grdSearchResult_OnLoadedData: function (gen_ctrl) {
        var grid = IVS280.Grids.grdSearchResult;

        for (var i = 0; i < grid.getRowsNum(); i++) {
            var row_id = grid.getRowId(i);
            if (gen_ctrl) {
                GenerateDetailButton(IVS280.Grids.grdSearchResult, IVS280.Grids.grdSearchResultColumns.DetailButton, row_id, IVS280.Grids.grdSearchResultColumns.DetailButton, true);
            }

            var isSelected = grid.cells(row_id, grid.getColIndexById(IVS280.Grids.grdSearchResultColumns.SelectedFlag)).getValue();
            grid.cells(row_id, grid.getColIndexById(IVS280.Grids.grdSearchResultColumns.SelectButton))
                .setValue(GenerateCheckBox2(IVS280.Grids.grdSearchResultColumns.SelectButton, row_id, "", true, isSelected));

            BindGridButtonClickEvent(IVS280.Grids.grdSearchResultColumns.DetailButton, row_id, IVS280.EventHandlers.grdSearchResult_OnDetail);
            BindGridCheckBoxClickEvent(IVS280.Grids.grdSearchResultColumns.SelectButton, row_id, IVS280.EventHandlers.chkSelect_OnClick);
        }

        $(IVS280.CtrlID.chkSelectAll).click(IVS280.EventHandlers.chkSelectAll_OnClick);
        grid.setSizes();
    },

    grdSearchResult_OnDetail: function (row_id) {
        var grid = IVS280.Grids.grdSearchResult;
        var row_index = grid.getRowIndex(row_id);
        grid.selectRow(row_index);

        var slipNo = grid.cells(row_id, grid.getColIndexById(IVS280.Grids.grdSearchResultColumns.SlipNo)).getValue();
        var stockInDate = grid.cells(row_id, grid.getColIndexById(IVS280.Grids.grdSearchResultColumns.StockInDate)).getValue();
        var supplierName = grid.cells(row_id, grid.getColIndexById(IVS280.Grids.grdSearchResultColumns.SupplierNameEN)).getValue();
        var invoiceNO = grid.cells(row_id, grid.getColIndexById(IVS280.Grids.grdSearchResultColumns.InvoiceNo)).getValue();

        var obj = {
            slipNo: [slipNo]
        };

        $(IVS280.CtrlID.txtSlipNoSelected).val(slipNo);
        $(IVS280.CtrlID.txtStockInDateSelected).val(stockInDate);
        $(IVS280.CtrlID.txtSupplierNameSelected).val(supplierName);
        $(IVS280.CtrlID.txtInvoiceNoSelected).val(invoiceNO);

        $(IVS280.CtrlID.divDetailGrid).LoadDataToGrid(IVS280.Grids.grdDetail, ROWS_PER_PAGE_FOR_VIEWPAGE, true, "/inventory/IVS280_GetDetail", obj, "dtInReportDetail", false
            , function (res) {
                var grid = IVS280.Grids.grdDetail;
                var totalAmount = 0;
                var totalQTy = 0;

                if (!CheckFirstRowIsEmpty(grid)) {
                    for (var i = 0; i < grid.getRowsNum(); i++) {
                        var row_id = grid.getRowId(i);
                        totalQTy += +(grid.cells(row_id, grid.getColIndexById(IVS280.Grids.grdDetailColumns.Qty)).getValue());
                        totalAmount += Math.round10(+(grid.cells(row_id, grid.getColIndexById(IVS280.Grids.grdDetailColumns.TotalUsD)).getValue()), -2);
                    }
                }

                $(IVS280.CtrlID.txtTotalQty).text(SetNumericText(totalQTy, 0));
                $(IVS280.CtrlID.txtTotalAmount).text("US$"+SetNumericText(totalAmount, 2));

                master_event.ScrollWindow(IVS280.CtrlID.divDetail);
            }
            , function (res) {
                $(IVS280.CtrlID.divDetail).show();
            }
        );

        return false;
    },

    chkSelect_OnClick: function (row_id, checked) {
        var grid = IVS280.Grids.grdSearchResult;
        grid.cells(row_id, grid.getColIndexById(IVS280.Grids.grdSearchResultColumns.SelectedFlag)).setValue(checked ? "1" : "");

        var checkedAll = $(IVS280.CtrlID.chkSelectAll).prop("checked");
        if (checkedAll && !checked) {
            $(IVS280.CtrlID.chkSelectAll).attr("checked", false);
        }
    },

    chkSelectAll_OnClick: function () {
        var checked = $(IVS280.CtrlID.chkSelectAll).prop("checked");
        var grid = IVS280.Grids.grdSearchResult;
        if (!CheckFirstRowIsEmpty(grid)) {
            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);
                var ctrl = "#" + GenerateGridControlID(IVS280.Grids.grdSearchResultColumns.SelectButton, row_id);
                $(ctrl).attr("checked", checked);
                grid.cells(row_id, grid.getColIndexById(IVS280.Grids.grdSearchResultColumns.SelectedFlag)).setValue(checked ? "1" : "");
            }
        }
    },

    grdDetail_OnLoadedData: function (gen_ctrl) {
        var grid = IVS280.Grids.grdDetail;

        grid.setSizes();
    },

    btnSearch_OnClick: function () {
        master_event.LockWindow(true);

        var obj = {
            ReportType: $(IVS280.CtrlID.cboStockReportType).val(),
            SlipNoStart: $(IVS280.CtrlID.txtSlipNoFrom).val(),
            SlipNoEnd: $(IVS280.CtrlID.txtSlipNoTo).val(),
            StockInDateStart: $(IVS280.CtrlID.txtStockInDateFrom).val(),
            StockInDateEnd: $(IVS280.CtrlID.txtStockInDateTo).val()
        };

        $(IVS280.CtrlID.divSearchResultGrid).LoadDataToGrid(IVS280.Grids.grdSearchResult, ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/inventory/IVS280_SearchSlip", obj, "dtInReportHeader", false, function (res) {
            $(IVS280.CtrlID.divDetail).hide();
            $(IVS280.CtrlID.chkSelectAll).attr("checked", false);
            master_event.ScrollWindow(IVS280.CtrlID.divSearchResult);

            master_event.LockWindow(false);
        });

        return false;
    },

    btnClear_OnClick: function () {
        $(IVS280.CtrlID.frmSearch).clearForm();
        DeleteAllRow(IVS280.Grids.grdSearchResult);
        DeleteAllRow(IVS280.Grids.grdDetail);
        $(IVS280.CtrlID.divDetail).hide();
        $(IVS280.CtrlID.chkSelectAll).attr("checked", false);

        ClearDateFromToControl(IVS280.CtrlID.txtStockInDateFrom, IVS280.CtrlID.txtStockInDateTo);

        return false;
    },

    btnDownload_OnClick: function () {
        master_event.LockWindow(true);

        var obj = {
            slipNo: []
        };

        var grid = IVS280.Grids.grdSearchResult;
        if (!CheckFirstRowIsEmpty(grid)) {
            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);
                var checked = grid.cells(row_id, grid.getColIndexById(IVS280.Grids.grdSearchResultColumns.SelectedFlag)).getValue();
                if (checked == "1") {
                    obj.slipNo.push(grid.cells(row_id, grid.getColIndexById(IVS280.Grids.grdSearchResultColumns.SlipNo)).getValue());
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

        ajax_method.CallScreenController("/Inventory/IVS280_GenerateReport", obj, function (result, controls, isWarning) {
            if (result != undefined) {
                download_method.CallDownloadController("ifDownload", "/Inventory/IVS280_Download", null);
            }

            master_event.LockWindow(false);
        }, false);
    },

    btnDownloadSummary_OnClick: function () {
        master_event.LockWindow(true);

        var obj = {
            slipNo: []
        };

        var grid = IVS280.Grids.grdSearchResult;
        if (!CheckFirstRowIsEmpty(grid)) {
            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);
                var checked = grid.cells(row_id, grid.getColIndexById(IVS280.Grids.grdSearchResultColumns.SelectedFlag)).getValue();
                if (checked == "1") {
                    obj.slipNo.push(grid.cells(row_id, grid.getColIndexById(IVS280.Grids.grdSearchResultColumns.SlipNo)).getValue());
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

        ajax_method.CallScreenController("/Inventory/IVS280_GenerateReportSummary", obj, function (result, controls, isWarning) {
            if (result != undefined) {
                download_method.CallDownloadController("ifDownload", "/Inventory/IVS280_Download", null);
            }

            master_event.LockWindow(false);
        }, false);
    }
}


IVS280.InitializeGrid = function () {

    IVS280.Grids.grdSearchResult = $(IVS280.CtrlID.divSearchResultGrid).InitialGrid(
        ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/inventory/IVS280_InitialSearchResultGrid"
        , function () {
            BindOnLoadedEvent(IVS280.Grids.grdSearchResult, IVS280.EventHandlers.grdSearchResult_OnLoadedData);
            $(IVS280.CtrlID.chkSelectAll).click(IVS280.EventHandlers.chkSelectAll_OnClick);
            SpecialGridControl(IVS280.Grids.grdSearchResult, [IVS280.Grids.grdSearchResultColumns.DetailButton, IVS280.Grids.grdSearchResultColumns.SelectButton]);
            IVS280.Grids.grdSearchResultIsBindEvent = true;
        }
    );

    IVS280.Grids.grdDetail = $(IVS280.CtrlID.divDetailGrid).InitialGrid(
        ROWS_PER_PAGE_FOR_VIEWPAGE, true, "/inventory/IVS280_InitialDetailGrid"
        , function () {
            BindOnLoadedEvent(IVS280.Grids.grdDetail, IVS280.EventHandlers.grdDetail_OnLoadedData);
            IVS280.Grids.grdDetailIsBindEvent = true;
        }
    );

};


IVS280.InitializeScreen = function () {
    InitialDateFromToControl(IVS280.CtrlID.txtStockInDateFrom, IVS280.CtrlID.txtStockInDateTo);

    $(IVS280.CtrlID.btnSearch).click(IVS280.EventHandlers.btnSearch_OnClick);
    $(IVS280.CtrlID.btnClear).click(IVS280.EventHandlers.btnClear_OnClick);
    $(IVS280.CtrlID.btnDownload).click(IVS280.EventHandlers.btnDownload_OnClick);
    $(IVS280.CtrlID.btnDownloadSummary).click(IVS280.EventHandlers.btnDownloadSummary_OnClick);

    $(IVS280.CtrlID.divSearch).css("visibility", "inherit");
    $(IVS280.CtrlID.divSearchResult).css("visibility", "inherit");
    $(IVS280.CtrlID.divDetail).css("visibility", "inherit");
};

IVS280.ResetScreen = function () {
    $(IVS280.CtrlID.divSearch).show();
    $(IVS280.CtrlID.divDetail).hide();

    $(IVS280.CtrlID.divSearchResult).clearForm();
    $(IVS280.CtrlID.divDetail).clearForm();

    IVS280.Grids.grdSearchResult_DeleteAllRows();
    IVS280.Grids.grdDetail_DeleteAllRows();

    $(IVS280.CtrlID.divSearch).find(".highlight").toggleClass("highlight", false);
    $(IVS280.CtrlID.divSearchResult).find(".highlight").toggleClass("highlight", false);
    $(IVS280.CtrlID.divDetail).find(".highlight").toggleClass("highlight", false);
};

$(document).ready(function () {
    IVS280.InitializeGrid();
    IVS280.InitializeScreen();
    IVS280.ResetScreen();
});
