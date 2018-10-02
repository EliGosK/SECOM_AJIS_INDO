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

var IVS281 = {};

IVS281.CtrlID = {
    divSearch: "#divSearch",
    frmSearch: "#frmSearch",
    cboStockReportType: "#cboStockReportType",
    txtSlipNoFrom: "#txtSlipNoFrom",
    txtSlipNoTo: "#txtSlipNoTo",
    txtStockOutDateFrom: "#txtStockOutDateFrom",
    txtStockOutDateTo: "#txtStockOutDateTo",
    txtContractCode: "#txtContractCode",
    txtOperateDateFrom: "#txtOperateDateFrom",
    txtOperateDateTo: "#txtOperateDateTo",
    txtCustomerName: "#txtCustomerName",
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
    txtStockOutDateSelected: "#txtStockOutDateSelected",
    txtTransferTypeSelected: "#txtTransferTypeSelected",
    txtTotalQty: "#txtTotalQty",
    txtTotalAmount: "#txtTotalAmount"
};

IVS281.Grids = {
    grdSearchResult: null,
    grdSearchResultIsBindEvent: false,
    grdSearchResult_DeleteAllRows: function () {
        if (IVS281.Grids.grdSearchResultIsBindEvent) {
            DeleteAllRow(IVS281.Grids.grdSearchResult);
        }
    },
    grdSearchResultColumns: {
        No: "No",
        SlipNo: "SlipNo",
        StockOutDate: "StockOutDate",
        Contract: "Contract",
        OperateDate: "OperateDate",
        CustName: "CustName",
        TransferType: "TransferType",
        DetailButton: "DetailButton",
        SelectButton: "SelectButton",
        SelectedFlag: "SelectedFlag"
    },

    grdDetail: null,
    grdDetailIsBindEvent: false,
    grdDetail_DeleteAllRows: function () {
        if (IVS281.Grids.grdDetailIsBindEvent) {
            DeleteAllRow(IVS281.Grids.grdDetail);
        }
    },
    grdDetailColumns: {
        No: "No",
        InstrumentCode: "InstrumentCode",
        Qty: "Qty",
        Cost: "Cost",
        Total: "Total",
        SlipNo: "SlipNo",
        StockOutDate: "StockOutDate",
        CostUsD: "CostUsD",
        TotalUsD: "TotalUsD"
    }
};

IVS281.EventHandlers = {
    grdSearchResult_OnLoadedData: function (gen_ctrl) {
        var grid = IVS281.Grids.grdSearchResult;

        for (var i = 0; i < grid.getRowsNum(); i++) {
            var row_id = grid.getRowId(i);
            if (gen_ctrl) {
                GenerateDetailButton(IVS281.Grids.grdSearchResult, IVS281.Grids.grdSearchResultColumns.DetailButton, row_id, IVS281.Grids.grdSearchResultColumns.DetailButton, true);
            }

            var isSelected = grid.cells(row_id, grid.getColIndexById(IVS281.Grids.grdSearchResultColumns.SelectedFlag)).getValue();
            grid.cells(row_id, grid.getColIndexById(IVS281.Grids.grdSearchResultColumns.SelectButton))
                .setValue(GenerateCheckBox2(IVS281.Grids.grdSearchResultColumns.SelectButton, row_id, "", true, isSelected));

            BindGridButtonClickEvent(IVS281.Grids.grdSearchResultColumns.DetailButton, row_id, IVS281.EventHandlers.grdSearchResult_OnDetail);
            BindGridCheckBoxClickEvent(IVS281.Grids.grdSearchResultColumns.SelectButton, row_id, IVS281.EventHandlers.chkSelect_OnClick);
        }

        $(IVS281.CtrlID.chkSelectAll).click(IVS281.EventHandlers.chkSelectAll_OnClick);
        grid.setSizes();
    },

    grdSearchResult_OnDetail: function (row_id) {
        var grid = IVS281.Grids.grdSearchResult;
        var row_index = grid.getRowIndex(row_id);
        grid.selectRow(row_index);

        var slipNo = grid.cells(row_id, grid.getColIndexById(IVS281.Grids.grdSearchResultColumns.SlipNo)).getValue();
        var StockOutDate = grid.cells(row_id, grid.getColIndexById(IVS281.Grids.grdSearchResultColumns.StockOutDate)).getValue();
        var transferType = grid.cells(row_id, grid.getColIndexById(IVS281.Grids.grdSearchResultColumns.TransferType)).getValue();

        var obj = {
            slipNo: [slipNo]
        };

        $(IVS281.CtrlID.txtSlipNoSelected).val(slipNo);
        $(IVS281.CtrlID.txtStockOutDateSelected).val(StockOutDate);
        $(IVS281.CtrlID.txtTransferTypeSelected).val(transferType);

        $(IVS281.CtrlID.divDetailGrid).LoadDataToGrid(IVS281.Grids.grdDetail, ROWS_PER_PAGE_FOR_VIEWPAGE, true, "/inventory/IVS281_GetDetail", obj, "dtOutReportDetail", false
            , function (res) {
                var grid = IVS281.Grids.grdDetail;
                var totalAmount = 0;
                var totalQTy = 0;

                if (!CheckFirstRowIsEmpty(grid)) {
                    for (var i = 0; i < grid.getRowsNum(); i++) {
                        var row_id = grid.getRowId(i);
                        totalQTy += +(grid.cells(row_id, grid.getColIndexById(IVS281.Grids.grdDetailColumns.Qty)).getValue());
                        totalAmount += Math.round10(+(grid.cells(row_id, grid.getColIndexById(IVS281.Grids.grdDetailColumns.TotalUsD)).getValue()), -2);
                    }
                }

                $(IVS281.CtrlID.txtTotalQty).text(SetNumericText(totalQTy, 0));
                $(IVS281.CtrlID.txtTotalAmount).text("US$" + SetNumericText(totalAmount, 2));
                
                master_event.ScrollWindow(IVS281.CtrlID.divDetail);
            }
            , function (res) {
                $(IVS281.CtrlID.divDetail).show();
            }
        );

        return false;
    },

    chkSelect_OnClick: function (row_id, checked) {
        var grid = IVS281.Grids.grdSearchResult;
        grid.cells(row_id, grid.getColIndexById(IVS281.Grids.grdSearchResultColumns.SelectedFlag)).setValue(checked ? "1" : "");

        var checkedAll = $(IVS281.CtrlID.chkSelectAll).prop("checked");
        if (checkedAll && !checked) {
            $(IVS281.CtrlID.chkSelectAll).attr("checked", false);
        }
    },

    chkSelectAll_OnClick: function () {
        var checked = $(IVS281.CtrlID.chkSelectAll).prop("checked");
        var grid = IVS281.Grids.grdSearchResult;
        if (!CheckFirstRowIsEmpty(grid)) {
            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);
                var ctrl = "#" + GenerateGridControlID(IVS281.Grids.grdSearchResultColumns.SelectButton, row_id);
                $(ctrl).attr("checked", checked);
                grid.cells(row_id, grid.getColIndexById(IVS281.Grids.grdSearchResultColumns.SelectedFlag)).setValue(checked ? "1" : "");
            }
        }
    },

    grdDetail_OnLoadedData: function (gen_ctrl) {
        var grid = IVS281.Grids.grdDetail;

        grid.setSizes();
    },

    btnSearch_OnClick: function () {
        master_event.LockWindow(true);

        var obj = {
            ReportType: $(IVS281.CtrlID.cboStockReportType).val(),
            SlipNoStart: $(IVS281.CtrlID.txtSlipNoFrom).val(),
            SlipNoEnd: $(IVS281.CtrlID.txtSlipNoTo).val(),
            StockOutDateStart: $(IVS281.CtrlID.txtStockOutDateFrom).val(),
            StockOutDateEnd: $(IVS281.CtrlID.txtStockOutDateTo).val(),
            ContractCode: $(IVS281.CtrlID.txtContractCode).val(),
            OperateDateStart: $(IVS281.CtrlID.txtOperateDateFrom).val(),
            OperateDateEnd: $(IVS281.CtrlID.txtOperateDateTo).val(),
            CustName: $(IVS281.CtrlID.txtCustomerName).val()
        };

        $(IVS281.CtrlID.divSearchResultGrid).LoadDataToGrid(IVS281.Grids.grdSearchResult, ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/inventory/IVS281_SearchSlip", obj, "dtOutReportHeader", false, function (res) {
            $(IVS281.CtrlID.divDetail).hide();
            $(IVS281.CtrlID.chkSelectAll).attr("checked", false);
            master_event.ScrollWindow(IVS281.CtrlID.divSearchResult);

            master_event.LockWindow(false);
        });

        return false;
    },

    btnClear_OnClick: function () {
        $(IVS281.CtrlID.frmSearch).clearForm();
        DeleteAllRow(IVS281.Grids.grdSearchResult);
        DeleteAllRow(IVS281.Grids.grdDetail);
        $(IVS281.CtrlID.divDetail).hide();
        $(IVS281.CtrlID.chkSelectAll).attr("checked", false);

        ClearDateFromToControl(IVS281.CtrlID.txtStockOutDateFrom, IVS281.CtrlID.txtStockOutDateTo);

        return false;
    },

    btnDownload_OnClick: function () {
        master_event.LockWindow(true);

        var obj = {
            slipNo: []
        };

        var grid = IVS281.Grids.grdSearchResult;
        if (!CheckFirstRowIsEmpty(grid)) {
            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);
                var checked = grid.cells(row_id, grid.getColIndexById(IVS281.Grids.grdSearchResultColumns.SelectedFlag)).getValue();
                if (checked == "1") {
                    obj.slipNo.push(grid.cells(row_id, grid.getColIndexById(IVS281.Grids.grdSearchResultColumns.SlipNo)).getValue());
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

        ajax_method.CallScreenController("/Inventory/IVS281_GenerateReport", obj, function (result, controls, isWarning) {
            if (result != undefined) {
                download_method.CallDownloadController("ifDownload", "/Inventory/IVS281_Download", null);
            }

            master_event.LockWindow(false);
        }, false);
    },

    btnDownloadSummary_OnClick: function () {
        master_event.LockWindow(true);

        var obj = {
            slipNo: []
        };

        var grid = IVS281.Grids.grdSearchResult;
        for (var i = 0; i < grid.getRowsNum(); i++) {
            var row_id = grid.getRowId(i);
            var checked = grid.cells(row_id, grid.getColIndexById(IVS281.Grids.grdSearchResultColumns.SelectedFlag)).getValue();
            if (checked == "1") {
                obj.slipNo.push(grid.cells(row_id, grid.getColIndexById(IVS281.Grids.grdSearchResultColumns.SlipNo)).getValue());
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

        ajax_method.CallScreenController("/Inventory/IVS281_GenerateReportSummary", obj, function (result, controls, isWarning) {
            if (result != undefined) {
                download_method.CallDownloadController("ifDownload", "/Inventory/IVS281_Download", null);
            }

            master_event.LockWindow(false);
        }, false);
    }
}


IVS281.InitializeGrid = function () {

    IVS281.Grids.grdSearchResult = $(IVS281.CtrlID.divSearchResultGrid).InitialGrid(
        ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/inventory/IVS281_InitialSearchResultGrid"
        , function () {
            BindOnLoadedEvent(IVS281.Grids.grdSearchResult, IVS281.EventHandlers.grdSearchResult_OnLoadedData);
            $(IVS281.CtrlID.chkSelectAll).click(IVS281.EventHandlers.chkSelectAll_OnClick);
            SpecialGridControl(IVS281.Grids.grdSearchResult, [IVS281.Grids.grdSearchResultColumns.DetailButton, IVS281.Grids.grdSearchResultColumns.SelectButton]);
            IVS281.Grids.grdSearchResultIsBindEvent = true;
        }
    );

    IVS281.Grids.grdDetail = $(IVS281.CtrlID.divDetailGrid).InitialGrid(
        ROWS_PER_PAGE_FOR_VIEWPAGE, true, "/inventory/IVS281_InitialDetailGrid"
        , function () {
            BindOnLoadedEvent(IVS281.Grids.grdDetail, IVS281.EventHandlers.grdDetail_OnLoadedData);
            IVS281.Grids.grdDetailIsBindEvent = true;
        }
    );

};


IVS281.InitializeScreen = function () {
    InitialDateFromToControl(IVS281.CtrlID.txtStockOutDateFrom, IVS281.CtrlID.txtStockOutDateTo);
    InitialDateFromToControl(IVS281.CtrlID.txtOperateDateFrom, IVS281.CtrlID.txtOperateDateTo);

    $(IVS281.CtrlID.btnSearch).click(IVS281.EventHandlers.btnSearch_OnClick);
    $(IVS281.CtrlID.btnClear).click(IVS281.EventHandlers.btnClear_OnClick);
    $(IVS281.CtrlID.btnDownload).click(IVS281.EventHandlers.btnDownload_OnClick);
    $(IVS281.CtrlID.btnDownloadSummary).click(IVS281.EventHandlers.btnDownloadSummary_OnClick);

    $(IVS281.CtrlID.divSearch).css("visibility", "inherit");
    $(IVS281.CtrlID.divSearchResult).css("visibility", "inherit");
    $(IVS281.CtrlID.divDetail).css("visibility", "inherit");
};

IVS281.ResetScreen = function () {
    $(IVS281.CtrlID.divSearch).show();
    $(IVS281.CtrlID.divDetail).hide();

    $(IVS281.CtrlID.divSearchResult).clearForm();
    $(IVS281.CtrlID.divDetail).clearForm();

    IVS281.Grids.grdSearchResult_DeleteAllRows();
    IVS281.Grids.grdDetail_DeleteAllRows();

    $(IVS281.CtrlID.divSearch).find(".highlight").toggleClass("highlight", false);
    $(IVS281.CtrlID.divSearchResult).find(".highlight").toggleClass("highlight", false);
    $(IVS281.CtrlID.divDetail).find(".highlight").toggleClass("highlight", false);
};

$(document).ready(function () {
    IVS281.InitializeGrid();
    IVS281.InitializeScreen();
    IVS281.ResetScreen();
});
