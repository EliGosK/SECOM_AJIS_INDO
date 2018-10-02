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

var IVS286 = {};

IVS286.LastSearchParam = null;

IVS286.CtrlID = {
    divSearch: "#divSearch",
    frmSearch: "#frmSearch",
    cboStockReportType: "#cboStockReportType",
    txtContractCode: "#txtContractCode",
    txtProcessDate: "#txtProcessDate",
    btnSearch: "#btnSearch",
    btnClear: "#btnClear",

    divSearchResult: "#divSearchResult",
    divSearchResultGrid: "#divSearchResultGrid",
    chkSelectAll: "#chkSelectAll",
    btnDownload: "#btnDownload",
    btnDownloadSummary: "#btnDownloadSummary",

    divDetail: "#divDetail",
    divDetailGrid: "#divDetailGrid",
    txtDtlContractCode: "#txtDtlContractCode",
    txtDtlCustomer: "#txtDtlCustomer",
    txtDtlSiteName: "#txtDtlSiteName",
    divDetailGrid: "#divDetailGrid",
    txtTotalQty: "#txtTotalQty",
    txtTotalAmount: "#txtTotalAmount"
};

IVS286.Grids = {
    grdSearchResult: null,
    grdSearchResultIsBindEvent: false,
    grdSearchResult_DeleteAllRows: function () {
        if (IVS286.Grids.grdSearchResultIsBindEvent) {
            DeleteAllRow(IVS286.Grids.grdSearchResult);
        }
    },
    grdSearchResultColumns: {
        No: "No",
        ContractCodeShort: "ContractCodeShort",
        OperateDate: "OperateDate",
        Customer: "Customer",
        SiteName: "SiteName",
        TransferDate: "TransferDate",
        Qty: "Qty",
        Amount: "AmountUsD",
        DetailButton: "DetailButton",
        SelectButton: "SelectButton",
        SelectedFlag: "SelectedFlag",
        ContractCode: "ContractCode"
    },

    grdDetail: null,
    grdDetailIsBindEvent: false,
    grdDetail_DeleteAllRows: function () {
        if (IVS286.Grids.grdDetailIsBindEvent) {
            DeleteAllRow(IVS286.Grids.grdDetail);
        }
    },
    grdDetailColumns: {
        No: "No",
        InstrumentCode: "EquipmentCode",
        Qty: "Qty",
        UnitPrice: "UnitPrice",
        Amount: "Amount"
    }
};

IVS286.EventHandlers = {
    grdSearchResult_OnLoadedData: function (gen_ctrl) {
        var grid = IVS286.Grids.grdSearchResult;

        for (var i = 0; i < grid.getRowsNum(); i++) {
            var row_id = grid.getRowId(i);
            if (gen_ctrl) {
                GenerateDetailButton(IVS286.Grids.grdSearchResult, IVS286.Grids.grdSearchResultColumns.DetailButton, row_id, IVS286.Grids.grdSearchResultColumns.DetailButton, true);
            }

            var isSelected = grid.cells(row_id, grid.getColIndexById(IVS286.Grids.grdSearchResultColumns.SelectedFlag)).getValue();
            grid.cells(row_id, grid.getColIndexById(IVS286.Grids.grdSearchResultColumns.SelectButton))
                .setValue(GenerateCheckBox2(IVS286.Grids.grdSearchResultColumns.SelectButton, row_id, "", true, isSelected));

            BindGridButtonClickEvent(IVS286.Grids.grdSearchResultColumns.DetailButton, row_id, IVS286.EventHandlers.grdSearchResult_OnDetail);
            BindGridCheckBoxClickEvent(IVS286.Grids.grdSearchResultColumns.SelectButton, row_id, IVS286.EventHandlers.chkSelect_OnClick);
        }

        $(IVS286.CtrlID.chkSelectAll).click(IVS286.EventHandlers.chkSelectAll_OnClick);
        grid.setSizes();
    },

    grdSearchResult_OnDetail: function (row_id) {
        var grid = IVS286.Grids.grdSearchResult;
        var row_index = grid.getRowIndex(row_id);
        grid.selectRow(row_index);

        var contractcode = grid.cells(row_id, grid.getColIndexById(IVS286.Grids.grdSearchResultColumns.ContractCodeShort)).getValue();
        var customer = grid.cells(row_id, grid.getColIndexById(IVS286.Grids.grdSearchResultColumns.Customer)).getValue();
        var sitename = grid.cells(row_id, grid.getColIndexById(IVS286.Grids.grdSearchResultColumns.SiteName)).getValue();
        var totalQty = grid.cells(row_id, grid.getColIndexById(IVS286.Grids.grdSearchResultColumns.Qty)).getValue();
        var totalAmount = grid.cells(row_id, grid.getColIndexById(IVS286.Grids.grdSearchResultColumns.Amount)).getValue();
        var obj = {
            ReportType: IVS286.LastSearchParam.ReportType,
            ContractCode: contractcode,
            ProcessDate: IVS286.LastSearchParam.ProcessDate
        };

        $(IVS286.CtrlID.txtDtlContractCode).val(contractcode);
        $(IVS286.CtrlID.txtDtlCustomer).val(customer);
        $(IVS286.CtrlID.txtDtlSiteName).val(sitename);

        $(IVS286.CtrlID.txtTotalQty).text(SetNumericText(totalQty, 0));
        $(IVS286.CtrlID.txtTotalAmount).text('US$' + SetNumericText(totalAmount, 2));

        $(IVS286.CtrlID.divDetailGrid).LoadDataToGrid(IVS286.Grids.grdDetail, ROWS_PER_PAGE_FOR_VIEWPAGE, true, "/inventory/IVS286_GetDetail", obj, "dtInProcessReportDetail", false
            , function (res) {
                var grid = IVS286.Grids.grdDetail;

                master_event.ScrollWindow(IVS286.CtrlID.divDetail);
            }
            , function (res) {
                $(IVS286.CtrlID.divDetail).show();
            }
        );

        return false;
    },

    chkSelect_OnClick: function (row_id, checked) {
        var grid = IVS286.Grids.grdSearchResult;
        grid.cells(row_id, grid.getColIndexById(IVS286.Grids.grdSearchResultColumns.SelectedFlag)).setValue(checked ? "1" : "");

        var checkedAll = $(IVS286.CtrlID.chkSelectAll).prop("checked");
        if (checkedAll && !checked) {
            $(IVS286.CtrlID.chkSelectAll).attr("checked", false);
        }
    },

    chkSelectAll_OnClick: function () {
        var checked = $(IVS286.CtrlID.chkSelectAll).prop("checked");
        var grid = IVS286.Grids.grdSearchResult;
        if (!CheckFirstRowIsEmpty(grid)) {
            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);
                var ctrl = "#" + GenerateGridControlID(IVS286.Grids.grdSearchResultColumns.SelectButton, row_id);
                $(ctrl).attr("checked", checked);
                grid.cells(row_id, grid.getColIndexById(IVS286.Grids.grdSearchResultColumns.SelectedFlag)).setValue(checked ? "1" : "");
            }
        }
    },

    btnSearch_OnClick: function () {
        master_event.LockWindow(true);

        var obj = {
            ReportType: $(IVS286.CtrlID.cboStockReportType).val(),
            ContractCode: $(IVS286.CtrlID.txtContractCode).val(),
            ContractCodeSelected: null,
            ProcessDate: $(IVS286.CtrlID.txtProcessDate).val()
        };

        IVS286.LastSearchParam = obj;

        $(IVS286.CtrlID.divSearchResultGrid).LoadDataToGrid(IVS286.Grids.grdSearchResult, ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/inventory/IVS286_SearchData", obj, "dtInProcessReport", false, function (res) {
            $(IVS286.CtrlID.chkSelectAll).attr("checked", false);
            master_event.ScrollWindow(IVS286.CtrlID.divSearchResult);

            master_event.LockWindow(false);
        });

        return false;
    },

    btnClear_OnClick: function () {
        $(IVS286.CtrlID.frmSearch).clearForm();
        DeleteAllRow(IVS286.Grids.grdSearchResult);
        DeleteAllRow(IVS286.Grids.grdDetail);
        $(IVS286.CtrlID.divDetail).hide();
        $(IVS286.CtrlID.chkSelectAll).attr("checked", false);

        IVS286.LastSearchParam = null;

        return false;
    },

    btnDownload_OnClick: function () {
        master_event.LockWindow(true);

        var obj = {
            ReportType: IVS286.LastSearchParam.ReportType,
            ContractCode: IVS286.LastSearchParam.ContractCode,
            ContractCodeSelected: [],
            ProcessDate: IVS286.LastSearchParam.ProcessDate
        };

        var grid = IVS286.Grids.grdSearchResult;
        if (!CheckFirstRowIsEmpty(grid)) {
            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);
                var checked = grid.cells(row_id, grid.getColIndexById(IVS286.Grids.grdSearchResultColumns.SelectedFlag)).getValue();
                if (checked == "1") {
                    obj.ContractCodeSelected.push(grid.cells(row_id, grid.getColIndexById(IVS286.Grids.grdSearchResultColumns.ContractCode)).getValue());
                }
            }
        }

        if (obj.ContractCodeSelected.length <= 0) {
            var messageParam = { "module": "Common", "code": "MSG0161", "param": "" };
            call_ajax_method("/Shared/GetMessage", messageParam, function (data) {
                OpenWarningDialog(data.Message);
            });

            master_event.LockWindow(false);
            return;
        }

        ajax_method.CallScreenController("/Inventory/IVS286_GenerateReport", obj, function (result, controls, isWarning) {
            if (result != undefined) {
                download_method.CallDownloadController("ifDownload", "/Inventory/IVS286_Download", null);
            }

            master_event.LockWindow(false);
        }, false);
    },
    
    btnDownloadSummary_OnClick: function () {
        master_event.LockWindow(true);

        var obj = {
            ReportType: IVS286.LastSearchParam.ReportType,
            ContractCode: IVS286.LastSearchParam.ContractCode,
            ContractCodeSelected: [],
            ProcessDate: IVS286.LastSearchParam.ProcessDate
        };

        var grid = IVS286.Grids.grdSearchResult;
        if (!CheckFirstRowIsEmpty(grid)) {
            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);
                var checked = grid.cells(row_id, grid.getColIndexById(IVS286.Grids.grdSearchResultColumns.SelectedFlag)).getValue();
                if (checked == "1") {
                    obj.ContractCodeSelected.push(grid.cells(row_id, grid.getColIndexById(IVS286.Grids.grdSearchResultColumns.ContractCode)).getValue());
                }
            }
        }

        if (obj.ContractCodeSelected.length <= 0) {
            var messageParam = { "module": "Common", "code": "MSG0161", "param": "" };
            call_ajax_method("/Shared/GetMessage", messageParam, function (data) {
                OpenWarningDialog(data.Message);
            });

            master_event.LockWindow(false);
            return;
        }

        ajax_method.CallScreenController("/Inventory/IVS286_GenerateReportSummary", obj, function (result, controls, isWarning) {
            if (result != undefined) {
                download_method.CallDownloadController("ifDownload", "/Inventory/IVS286_Download", null);
            }

            master_event.LockWindow(false);
        }, false);
    }
}


IVS286.InitializeGrid = function () {

    IVS286.Grids.grdSearchResult = $(IVS286.CtrlID.divSearchResultGrid).InitialGrid(
        ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/inventory/IVS286_InitialSearchResultGrid"
        , function () {
            BindOnLoadedEvent(IVS286.Grids.grdSearchResult, IVS286.EventHandlers.grdSearchResult_OnLoadedData);
            $(IVS286.CtrlID.chkSelectAll).click(IVS286.EventHandlers.chkSelectAll_OnClick);
            SpecialGridControl(IVS286.Grids.grdSearchResult, [IVS286.Grids.grdSearchResultColumns.SelectButton]);
            IVS286.Grids.grdSearchResultIsBindEvent = true;
        }
    );
    
    IVS286.Grids.grdDetail = $(IVS286.CtrlID.divDetailGrid).InitialGrid(
        ROWS_PER_PAGE_FOR_VIEWPAGE, true, "/inventory/IVS286_InitialDetailGrid"
        , function () {
            BindOnLoadedEvent(IVS286.Grids.grdDetail, IVS286.EventHandlers.grdDetail_OnLoadedData);
            IVS286.Grids.grdDetailIsBindEvent = true;
        }
    );

};


IVS286.InitializeScreen = function () {
    $(IVS286.CtrlID.btnSearch).click(IVS286.EventHandlers.btnSearch_OnClick);
    $(IVS286.CtrlID.btnClear).click(IVS286.EventHandlers.btnClear_OnClick);
    $(IVS286.CtrlID.btnDownload).click(IVS286.EventHandlers.btnDownload_OnClick);
    $(IVS286.CtrlID.btnDownloadSummary).click(IVS286.EventHandlers.btnDownloadSummary_OnClick);

    $(IVS286.CtrlID.divSearch).css("visibility", "inherit");
    $(IVS286.CtrlID.divSearchResult).css("visibility", "inherit");
    $(IVS286.CtrlID.divDetail).css("visibility", "inherit");
};

IVS286.ResetScreen = function () {
    $(IVS286.CtrlID.divSearch).show();
    $(IVS286.CtrlID.divDetail).hide();

    $(IVS286.CtrlID.divSearchResult).clearForm();
    $(IVS286.CtrlID.divDetail).clearForm();

    IVS286.Grids.grdSearchResult_DeleteAllRows();
    IVS286.Grids.grdDetail_DeleteAllRows();

    $(IVS286.CtrlID.divSearch).find(".highlight").toggleClass("highlight", false);
    $(IVS286.CtrlID.divSearchResult).find(".highlight").toggleClass("highlight", false);
    $(IVS286.CtrlID.divDetail).find(".highlight").toggleClass("highlight", false);
};

$(document).ready(function () {
    IVS286.InitializeGrid();
    IVS286.InitializeScreen();
    IVS286.ResetScreen();
});
