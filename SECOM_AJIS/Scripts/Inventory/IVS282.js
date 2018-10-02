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

var IVS282 = {};

IVS282.CtrlID = {
    divSearch: "#divSearch",
    frmSearch: "#frmSearch",
    cboStockReportType: "#cboStockReportType",
    txtSlipNoFrom: "#txtSlipNoFrom",
    txtSlipNoTo: "#txtSlipNoTo",
    txtReturnDateFrom: "#txtReturnDateFrom",
    txtReturnDateTo: "#txtReturnDateTo",
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
    txtReturnDateSelected: "#txtReturnDateSelected"
};

IVS282.Grids = {
    grdSearchResult: null,
    grdSearchResultIsBindEvent: false,
    grdSearchResult_DeleteAllRows: function () {
        if (IVS282.Grids.grdSearchResultIsBindEvent) {
            DeleteAllRow(IVS282.Grids.grdSearchResult);
        }
    },
    grdSearchResultColumns: {
        No: "No",
        SlipNo: "SlipNo",
        ReturnDate: "StockOutDate",
        Contract: "Contract",
        OperateDate: "OperateDate",
        CustName: "CustName",
        DetailButton: "DetailButton",
        SelectButton: "SelectButton",
        SelectedFlag: "SelectedFlag"
    },

    grdDetail: null,
    grdDetailIsBindEvent: false,
    grdDetail_DeleteAllRows: function () {
        if (IVS282.Grids.grdDetailIsBindEvent) {
            DeleteAllRow(IVS282.Grids.grdDetail);
        }
    },
    grdDetailColumns: {
        No: "No",
        InstrumentCode: "InstrumentCode",
        Qty: "Qty",
        Cost: "Cost",
        Total: "Total",
        SlipNo: "SlipNo",
        ReturnDate: "ReturnDate"
    }
};

IVS282.EventHandlers = {
    grdSearchResult_OnLoadedData: function (gen_ctrl) {
        var grid = IVS282.Grids.grdSearchResult;

        for (var i = 0; i < grid.getRowsNum(); i++) {
            var row_id = grid.getRowId(i);
            if (gen_ctrl) {
                GenerateDetailButton(IVS282.Grids.grdSearchResult, IVS282.Grids.grdSearchResultColumns.DetailButton, row_id, IVS282.Grids.grdSearchResultColumns.DetailButton, true);
            }

            var isSelected = grid.cells(row_id, grid.getColIndexById(IVS282.Grids.grdSearchResultColumns.SelectedFlag)).getValue();
            grid.cells(row_id, grid.getColIndexById(IVS282.Grids.grdSearchResultColumns.SelectButton))
                .setValue(GenerateCheckBox2(IVS282.Grids.grdSearchResultColumns.SelectButton, row_id, "", true, isSelected));

            BindGridButtonClickEvent(IVS282.Grids.grdSearchResultColumns.DetailButton, row_id, IVS282.EventHandlers.grdSearchResult_OnDetail);
            BindGridCheckBoxClickEvent(IVS282.Grids.grdSearchResultColumns.SelectButton, row_id, IVS282.EventHandlers.chkSelect_OnClick);
        }

        $(IVS282.CtrlID.chkSelectAll).click(IVS282.EventHandlers.chkSelectAll_OnClick);
        grid.setSizes();
    },

    grdSearchResult_OnDetail: function (row_id) {
        var grid = IVS282.Grids.grdSearchResult;
        var row_index = grid.getRowIndex(row_id);
        grid.selectRow(row_index);

        var slipNo = grid.cells(row_id, grid.getColIndexById(IVS282.Grids.grdSearchResultColumns.SlipNo)).getValue();
        var ReturnDate = grid.cells(row_id, grid.getColIndexById(IVS282.Grids.grdSearchResultColumns.ReturnDate)).getValue();

        var obj = {
            slipNo: [slipNo]
        };

        $(IVS282.CtrlID.txtSlipNoSelected).val(slipNo);
        $(IVS282.CtrlID.txtReturnDateSelected).val(ReturnDate);

        $(IVS282.CtrlID.divDetailGrid).LoadDataToGrid(IVS282.Grids.grdDetail, ROWS_PER_PAGE_FOR_VIEWPAGE, true, "/inventory/IVS282_GetDetail", obj, "dtReturnReportDetail", false
            , function (res) {
                master_event.ScrollWindow(IVS282.CtrlID.divDetail);
            }
            , function (res) {
                $(IVS282.CtrlID.divDetail).show();
            }
        );

        return false;
    },

    chkSelect_OnClick: function (row_id, checked) {
        var grid = IVS282.Grids.grdSearchResult;
        grid.cells(row_id, grid.getColIndexById(IVS282.Grids.grdSearchResultColumns.SelectedFlag)).setValue(checked ? "1" : "");

        var checkedAll = $(IVS282.CtrlID.chkSelectAll).prop("checked");
        if (checkedAll && !checked) {
            $(IVS282.CtrlID.chkSelectAll).attr("checked", false);
        }
    },

    chkSelectAll_OnClick: function () {
        var checked = $(IVS282.CtrlID.chkSelectAll).prop("checked");
        var grid = IVS282.Grids.grdSearchResult;
        if (!CheckFirstRowIsEmpty(grid)) {
            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);
                var ctrl = "#" + GenerateGridControlID(IVS282.Grids.grdSearchResultColumns.SelectButton, row_id);
                $(ctrl).attr("checked", checked);
                grid.cells(row_id, grid.getColIndexById(IVS282.Grids.grdSearchResultColumns.SelectedFlag)).setValue(checked ? "1" : "");
            }
        }
    },

    grdDetail_OnLoadedData: function (gen_ctrl) {
        var grid = IVS282.Grids.grdDetail;

        grid.setSizes();
    },

    btnSearch_OnClick: function () {
        master_event.LockWindow(true);
        
        var obj = {
            ReportType: $(IVS282.CtrlID.cboStockReportType).val(),
            SlipNoStart: $(IVS282.CtrlID.txtSlipNoFrom).val(),
            SlipNoEnd: $(IVS282.CtrlID.txtSlipNoTo).val(),
            ReturnDateStart: $(IVS282.CtrlID.txtReturnDateFrom).val(),
            ReturnDateEnd: $(IVS282.CtrlID.txtReturnDateTo).val(),
            ContractCode: $(IVS282.CtrlID.txtContractCode).val(),
            OperateDateStart: $(IVS282.CtrlID.txtOperateDateFrom).val(),
            OperateDateEnd: $(IVS282.CtrlID.txtOperateDateTo).val(),
            CustName: $(IVS282.CtrlID.txtCustomerName).val()
        };

        $(IVS282.CtrlID.divSearchResultGrid).LoadDataToGrid(IVS282.Grids.grdSearchResult, ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/inventory/IVS282_SearchSlip", obj, "dtReturnReportHeader", false, function (res) {
            $(IVS282.CtrlID.divDetail).hide();
            $(IVS282.CtrlID.chkSelectAll).attr("checked", false);
            master_event.ScrollWindow(IVS282.CtrlID.divSearchResult);

            master_event.LockWindow(false);
        });

        return false;
    },

    btnClear_OnClick: function () {
        $(IVS282.CtrlID.frmSearch).clearForm();
        DeleteAllRow(IVS282.Grids.grdSearchResult);
        DeleteAllRow(IVS282.Grids.grdDetail);
        $(IVS282.CtrlID.divDetail).hide();
        $(IVS282.CtrlID.chkSelectAll).attr("checked", false);

        ClearDateFromToControl(IVS282.CtrlID.txtReturnDateFrom, IVS282.CtrlID.txtReturnDateTo);

        return false;
    },

    btnDownload_OnClick: function () {
        master_event.LockWindow(true);

        var obj = {
            slipNo: []
        };

        var grid = IVS282.Grids.grdSearchResult;
        if (!CheckFirstRowIsEmpty(grid)) {
            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);
                var checked = grid.cells(row_id, grid.getColIndexById(IVS282.Grids.grdSearchResultColumns.SelectedFlag)).getValue();
                if (checked == "1") {
                    obj.slipNo.push(grid.cells(row_id, grid.getColIndexById(IVS282.Grids.grdSearchResultColumns.SlipNo)).getValue());
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

        ajax_method.CallScreenController("/Inventory/IVS282_GenerateReport", obj, function (result, controls, isWarning) {
            if (result != undefined) {
                download_method.CallDownloadController("ifDownload", "/Inventory/IVS282_Download", null);
            }

            master_event.LockWindow(false);
        }, false);
    },

    btnDownloadSummary_OnClick: function () {
        master_event.LockWindow(true);

        var obj = {
            slipNo: []
        };

        var grid = IVS282.Grids.grdSearchResult;
        for (var i = 0; i < grid.getRowsNum(); i++) {
            var row_id = grid.getRowId(i);
            var checked = grid.cells(row_id, grid.getColIndexById(IVS282.Grids.grdSearchResultColumns.SelectedFlag)).getValue();
            if (checked == "1") {
                obj.slipNo.push(grid.cells(row_id, grid.getColIndexById(IVS282.Grids.grdSearchResultColumns.SlipNo)).getValue());
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

        ajax_method.CallScreenController("/Inventory/IVS282_GenerateReportSummary", obj, function (result, controls, isWarning) {
            if (result != undefined) {
                download_method.CallDownloadController("ifDownload", "/Inventory/IVS282_Download", null);
            }

            master_event.LockWindow(false);
        }, false);
    }
}


IVS282.InitializeGrid = function () {

    IVS282.Grids.grdSearchResult = $(IVS282.CtrlID.divSearchResultGrid).InitialGrid(
        ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/inventory/IVS282_InitialSearchResultGrid"
        , function () {
            BindOnLoadedEvent(IVS282.Grids.grdSearchResult, IVS282.EventHandlers.grdSearchResult_OnLoadedData);
            $(IVS282.CtrlID.chkSelectAll).click(IVS282.EventHandlers.chkSelectAll_OnClick);
            SpecialGridControl(IVS282.Grids.grdSearchResult, [IVS282.Grids.grdSearchResultColumns.DetailButton, IVS282.Grids.grdSearchResultColumns.SelectButton]);
            IVS282.Grids.grdSearchResultIsBindEvent = true;
        }
    );

    IVS282.Grids.grdDetail = $(IVS282.CtrlID.divDetailGrid).InitialGrid(
        ROWS_PER_PAGE_FOR_VIEWPAGE, true, "/inventory/IVS282_InitialDetailGrid"
        , function () {
            BindOnLoadedEvent(IVS282.Grids.grdDetail, IVS282.EventHandlers.grdDetail_OnLoadedData);
            IVS282.Grids.grdDetailIsBindEvent = true;
        }
    );

};


IVS282.InitializeScreen = function () {
    InitialDateFromToControl(IVS282.CtrlID.txtReturnDateFrom, IVS282.CtrlID.txtReturnDateTo);
    InitialDateFromToControl(IVS282.CtrlID.txtOperateDateFrom, IVS282.CtrlID.txtOperateDateTo);

    $(IVS282.CtrlID.btnSearch).click(IVS282.EventHandlers.btnSearch_OnClick);
    $(IVS282.CtrlID.btnClear).click(IVS282.EventHandlers.btnClear_OnClick);
    $(IVS282.CtrlID.btnDownload).click(IVS282.EventHandlers.btnDownload_OnClick);
    $(IVS282.CtrlID.btnDownloadSummary).click(IVS282.EventHandlers.btnDownloadSummary_OnClick);

    $(IVS282.CtrlID.divSearch).css("visibility", "inherit");
    $(IVS282.CtrlID.divSearchResult).css("visibility", "inherit");
    $(IVS282.CtrlID.divDetail).css("visibility", "inherit");
};

IVS282.ResetScreen = function () {
    $(IVS282.CtrlID.divSearch).show();
    $(IVS282.CtrlID.divDetail).hide();

    $(IVS282.CtrlID.divSearchResult).clearForm();
    $(IVS282.CtrlID.divDetail).clearForm();

    IVS282.Grids.grdSearchResult_DeleteAllRows();
    IVS282.Grids.grdDetail_DeleteAllRows();

    $(IVS282.CtrlID.divSearch).find(".highlight").toggleClass("highlight", false);
    $(IVS282.CtrlID.divSearchResult).find(".highlight").toggleClass("highlight", false);
    $(IVS282.CtrlID.divDetail).find(".highlight").toggleClass("highlight", false);
};

$(document).ready(function () {
    IVS282.InitializeGrid();
    IVS282.InitializeScreen();
    IVS282.ResetScreen();
});
