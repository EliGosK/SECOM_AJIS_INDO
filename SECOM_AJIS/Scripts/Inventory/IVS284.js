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

var IVS284 = {};

IVS284.LastSearchParam = null;

IVS284.CtrlID = {
    divSearch: "#divSearch",
    frmSearch: "#frmSearch",
    cboStockReportType: "#cboStockReportType",
    txtContractCode: "#txtContractCode",
    cboProcessDate: "#cboProcessDate",
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
    txtDtlOCC: "#txtDtlOCC",
    txtDtlCustomer: "#txtDtlCustomer",
    txtDtlSiteName: "#txtDtlSiteName",
    txtDtlOperateDate: "#txtDtlOperateDate",
    txtDtlTransferDate: "#txtDtlTransferDate",
    txtTotalQty: "#txtTotalQty",
    txtTotalAmount: "#txtTotalAmount"
};

IVS284.Grids = {
    grdSearchResult: null,
    grdSearchResultIsBindEvent: false,
    grdSearchResult_DeleteAllRows: function () {
        if (IVS284.Grids.grdSearchResultIsBindEvent) {
            DeleteAllRow(IVS284.Grids.grdSearchResult);
        }
    },
    grdSearchResultColumns: {
        No: "No",
        ContractCodeShort: "ContractCodeShort",
        OCC: "OCC",
        OperateDate: "OperateDate",
        CustName: "CustName",
        SiteName: "SiteName",
        TransferDate: "TransferDate",
        Qty: "Qty",
        Amount: "Amount",
        AmountUsD: "AmountUsD",
        DetailButton: "DetailButton",
        SelectButton: "SelectButton",
        SelectedFlag: "SelectedFlag",
        ContractCode: "ContractCode"
    },

    grdDetail: null,
    grdDetailIsBindEvent: false,
    grdDetail_DeleteAllRows: function () {
        if (IVS284.Grids.grdDetailIsBindEvent) {
            DeleteAllRow(IVS284.Grids.grdDetail);
        }
    },
    grdDetailColumns: {
        No: "No",
        InstrumentCode: "InstrumentCode",
        Qty: "Qty",
        UnitPrice: "UnitPrice",
        Amount: "Amount",
        AmountUsD: "AmountUsD"
    }
};

IVS284.EventHandlers = {
    grdSearchResult_OnLoadedData: function (gen_ctrl) {
        var grid = IVS284.Grids.grdSearchResult;

        for (var i = 0; i < grid.getRowsNum(); i++) {
            var row_id = grid.getRowId(i);
            if (gen_ctrl) {
                GenerateDetailButton(IVS284.Grids.grdSearchResult, IVS284.Grids.grdSearchResultColumns.DetailButton, row_id, IVS284.Grids.grdSearchResultColumns.DetailButton, true);
            }

            var isSelected = grid.cells(row_id, grid.getColIndexById(IVS284.Grids.grdSearchResultColumns.SelectedFlag)).getValue();
            grid.cells(row_id, grid.getColIndexById(IVS284.Grids.grdSearchResultColumns.SelectButton))
                .setValue(GenerateCheckBox2(IVS284.Grids.grdSearchResultColumns.SelectButton, row_id, "", true, isSelected));

            BindGridButtonClickEvent(IVS284.Grids.grdSearchResultColumns.DetailButton, row_id, IVS284.EventHandlers.grdSearchResult_OnDetail);
            BindGridCheckBoxClickEvent(IVS284.Grids.grdSearchResultColumns.SelectButton, row_id, IVS284.EventHandlers.chkSelect_OnClick);
        }

        $(IVS284.CtrlID.chkSelectAll).click(IVS284.EventHandlers.chkSelectAll_OnClick);
        grid.setSizes();
    },

    grdSearchResult_OnDetail: function (row_id) {
        var grid = IVS284.Grids.grdSearchResult;
        var row_index = grid.getRowIndex(row_id);
        grid.selectRow(row_index);

        var contractcode = grid.cells(row_id, grid.getColIndexById(IVS284.Grids.grdSearchResultColumns.ContractCodeShort)).getValue();
        var occ = grid.cells(row_id, grid.getColIndexById(IVS284.Grids.grdSearchResultColumns.OCC)).getValue();
        var customer = grid.cells(row_id, grid.getColIndexById(IVS284.Grids.grdSearchResultColumns.CustName)).getValue();
        var sitename = grid.cells(row_id, grid.getColIndexById(IVS284.Grids.grdSearchResultColumns.SiteName)).getValue();
        var operatedate = grid.cells(row_id, grid.getColIndexById(IVS284.Grids.grdSearchResultColumns.OperateDate)).getValue();
        var transferdate = grid.cells(row_id, grid.getColIndexById(IVS284.Grids.grdSearchResultColumns.TransferDate)).getValue();
        var totalQty = grid.cells(row_id, grid.getColIndexById(IVS284.Grids.grdSearchResultColumns.Qty)).getValue();
        var totalAmount = grid.cells(row_id, grid.getColIndexById(IVS284.Grids.grdSearchResultColumns.AmountUsD)).getValue();

        var obj = {
            ReportType: IVS284.LastSearchParam.ReportType,
            ContractCode: contractcode + '/' + occ,
            ContractCodeSelected: null,
            YearMonth: IVS284.LastSearchParam.YearMonth
        };

        $(IVS284.CtrlID.txtDtlContractCode).val(contractcode);
        $(IVS284.CtrlID.txtDtlOCC).val(occ);
        $(IVS284.CtrlID.txtDtlCustomer).val(customer);
        $(IVS284.CtrlID.txtDtlSiteName).val(sitename);
        $(IVS284.CtrlID.txtDtlOperateDate).val(operatedate);
        $(IVS284.CtrlID.txtDtlTransferDate).val(transferdate);

        $(IVS284.CtrlID.txtTotalQty).text(SetNumericText(totalQty, 0));
        $(IVS284.CtrlID.txtTotalAmount).text("US$" + SetNumericText(totalAmount, 2));

        $(IVS284.CtrlID.divDetailGrid).LoadDataToGrid(IVS284.Grids.grdDetail, ROWS_PER_PAGE_FOR_VIEWPAGE, true, "/inventory/IVS284_GetDetail", obj, "dtInprocessToInstallReportDetail", false
            , function (res) {
                var grid = IVS284.Grids.grdDetail;

                master_event.ScrollWindow(IVS284.CtrlID.divDetail);
            }
            , function (res) {
                $(IVS284.CtrlID.divDetail).show();
            }
        );

        return false;
    },

    chkSelect_OnClick: function (row_id, checked) {
        var grid = IVS284.Grids.grdSearchResult;
        grid.cells(row_id, grid.getColIndexById(IVS284.Grids.grdSearchResultColumns.SelectedFlag)).setValue(checked ? "1" : "");

        var checkedAll = $(IVS284.CtrlID.chkSelectAll).prop("checked");
        if (checkedAll && !checked) {
            $(IVS284.CtrlID.chkSelectAll).attr("checked", false);
        }
    },

    chkSelectAll_OnClick: function () {
        var checked = $(IVS284.CtrlID.chkSelectAll).prop("checked");
        var grid = IVS284.Grids.grdSearchResult;
        if (!CheckFirstRowIsEmpty(grid)) {
            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);
                var ctrl = "#" + GenerateGridControlID(IVS284.Grids.grdSearchResultColumns.SelectButton, row_id);
                $(ctrl).attr("checked", checked);
                grid.cells(row_id, grid.getColIndexById(IVS284.Grids.grdSearchResultColumns.SelectedFlag)).setValue(checked ? "1" : "");
            }
        }
    },

    btnSearch_OnClick: function () {
        master_event.LockWindow(true);

        var obj = {
            ReportType: $(IVS284.CtrlID.cboStockReportType).val(),
            ContractCode: $(IVS284.CtrlID.txtContractCode).val(),
            ContractCodeSelected: null,
            YearMonth: $(IVS284.CtrlID.cboProcessDate).val()
        };

        IVS284.LastSearchParam = obj;

        $(IVS284.CtrlID.divSearchResultGrid).LoadDataToGrid(IVS284.Grids.grdSearchResult, ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/inventory/IVS284_SearchData", obj, "dtInprocessToInstallReport", false, function (res) {
            $(IVS284.CtrlID.chkSelectAll).attr("checked", false);
            master_event.ScrollWindow(IVS284.CtrlID.divSearchResult);

            master_event.LockWindow(false);
        });

        return false;
    },

    btnClear_OnClick: function () {
        $(IVS284.CtrlID.frmSearch).clearForm();
        DeleteAllRow(IVS284.Grids.grdSearchResult);
        $(IVS284.CtrlID.chkSelectAll).attr("checked", false);

        IVS284.LastSearchParam = null;

        return false;
    },

    btnDownload_OnClick: function () {
        master_event.LockWindow(true);

        var obj = {
            ReportType: $(IVS284.CtrlID.cboStockReportType).val(),
            ContractCode: $(IVS284.CtrlID.txtContractCode).val(),
            ContractCodeSelected: [],
            YearMonth: $(IVS284.CtrlID.cboProcessDate).val()
        };

        var grid = IVS284.Grids.grdSearchResult;
        if (!CheckFirstRowIsEmpty(grid)) {
            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);
                var checked = grid.cells(row_id, grid.getColIndexById(IVS284.Grids.grdSearchResultColumns.SelectedFlag)).getValue();
                if (checked == "1") {
                    var tmpcode = grid.cells(row_id, grid.getColIndexById(IVS284.Grids.grdSearchResultColumns.ContractCode)).getValue();
                    var tmpocc = grid.cells(row_id, grid.getColIndexById(IVS284.Grids.grdSearchResultColumns.OCC)).getValue();
                    obj.ContractCodeSelected.push(tmpcode + "/" + tmpocc);
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

        ajax_method.CallScreenController("/Inventory/IVS284_GenerateReport", obj, function (result, controls, isWarning) {
            if (result != undefined) {
                download_method.CallDownloadController("ifDownload", "/Inventory/IVS284_Download", null);
            }

            master_event.LockWindow(false);
        }, false);
    },

    btnDownloadSummary_OnClick: function () {
        master_event.LockWindow(true);

        var obj = {
            ReportType: $(IVS284.CtrlID.cboStockReportType).val(),
            ContractCode: $(IVS284.CtrlID.txtContractCode).val(),
            ContractCodeSelected: [],
            YearMonth: $(IVS284.CtrlID.cboProcessDate).val()
        };

        var grid = IVS284.Grids.grdSearchResult;
        if (!CheckFirstRowIsEmpty(grid)) {
            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);
                var checked = grid.cells(row_id, grid.getColIndexById(IVS284.Grids.grdSearchResultColumns.SelectedFlag)).getValue();
                if (checked == "1") {
                    var tmpcode = grid.cells(row_id, grid.getColIndexById(IVS284.Grids.grdSearchResultColumns.ContractCode)).getValue();
                    var tmpocc = grid.cells(row_id, grid.getColIndexById(IVS284.Grids.grdSearchResultColumns.OCC)).getValue();
                    obj.ContractCodeSelected.push(tmpcode + "/" + tmpocc);
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

        ajax_method.CallScreenController("/Inventory/IVS284_GenerateReportSummary", obj, function (result, controls, isWarning) {
            if (result != undefined) {
                download_method.CallDownloadController("ifDownload", "/Inventory/IVS284_Download", null);
            }

            master_event.LockWindow(false);
        }, false);
    }
}


IVS284.InitializeGrid = function () {

    IVS284.Grids.grdSearchResult = $(IVS284.CtrlID.divSearchResultGrid).InitialGrid(
        ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/inventory/IVS284_InitialSearchResultGrid"
        , function () {
            BindOnLoadedEvent(IVS284.Grids.grdSearchResult, IVS284.EventHandlers.grdSearchResult_OnLoadedData);
            $(IVS284.CtrlID.chkSelectAll).click(IVS284.EventHandlers.chkSelectAll_OnClick);
            SpecialGridControl(IVS284.Grids.grdSearchResult, [IVS284.Grids.grdSearchResultColumns.SelectButton]);
            IVS284.Grids.grdSearchResultIsBindEvent = true;
        }
    );

    IVS284.Grids.grdDetail = $(IVS284.CtrlID.divDetailGrid).InitialGrid(
        ROWS_PER_PAGE_FOR_VIEWPAGE, true, "/inventory/IVS284_InitialDetailGrid"
        , function () {
            BindOnLoadedEvent(IVS284.Grids.grdDetail, IVS284.EventHandlers.grdDetail_OnLoadedData);
            IVS284.Grids.grdDetailIsBindEvent = true;
        }
    );

};


IVS284.InitializeScreen = function () {
    $(IVS284.CtrlID.btnSearch).click(IVS284.EventHandlers.btnSearch_OnClick);
    $(IVS284.CtrlID.btnClear).click(IVS284.EventHandlers.btnClear_OnClick);
    $(IVS284.CtrlID.btnDownload).click(IVS284.EventHandlers.btnDownload_OnClick);
    $(IVS284.CtrlID.btnDownloadSummary).click(IVS284.EventHandlers.btnDownloadSummary_OnClick);

    $(IVS284.CtrlID.divSearch).css("visibility", "inherit");
    $(IVS284.CtrlID.divSearchResult).css("visibility", "inherit");
    $(IVS284.CtrlID.divDetail).css("visibility", "inherit");
};

IVS284.ResetScreen = function () {
    $(IVS284.CtrlID.divSearch).show();
    $(IVS284.CtrlID.divDetail).hide();

    $(IVS284.CtrlID.divSearchResult).clearForm();
    $(IVS284.CtrlID.divDetail).clearForm();

    IVS284.Grids.grdSearchResult_DeleteAllRows();
    IVS284.Grids.grdDetail_DeleteAllRows();

    $(IVS284.CtrlID.divSearch).find(".highlight").toggleClass("highlight", false);
    $(IVS284.CtrlID.divSearchResult).find(".highlight").toggleClass("highlight", false);
    $(IVS284.CtrlID.divDetail).find(".highlight").toggleClass("highlight", false);
};

$(document).ready(function () {
    IVS284.InitializeGrid();
    IVS284.InitializeScreen();
    IVS284.ResetScreen();
});
