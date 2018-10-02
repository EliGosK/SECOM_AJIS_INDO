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

var IVS288 = {};

IVS288.CtrlID = {
    divSearch: "#divSearch",
    frmSearch: "#frmSearch",
    txtSlipNoFrom: "#txtSlipNoFrom",
    txtSlipNoTo: "#txtSlipNoTo",
    txtTransferDateFrom: "#txtTransferDateFrom",
    txtTransferDateTo: "#txtTransferDateTo",
    txtContractCode: "#txtContractCode",
    cboSourceArea: "#cboSourceArea",
    cboDestinationArea: "#cboDestinationArea",
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

IVS288.Grids = {
    grdSearchResult: null,
    grdSearchResultIsBindEvent: false,
    grdSearchResult_DeleteAllRows: function () {
        if (IVS288.Grids.grdSearchResultIsBindEvent) {
            DeleteAllRow(IVS288.Grids.grdSearchResult);
        }
    },
    grdSearchResultColumns: {
        No: "No",
        SlipNo: "SlipNo",
        TransferDate: "TransferDate",
        Contract: "Contract",
        CustName: "CustName",
        DetailButton: "DetailButton",
        SelectButton: "SelectButton",
        SelectedFlag: "SelectedFlag"
    },

    grdDetail: null,
    grdDetailIsBindEvent: false,
    grdDetail_DeleteAllRows: function () {
        if (IVS288.Grids.grdDetailIsBindEvent) {
            DeleteAllRow(IVS288.Grids.grdDetail);
        }
    },
    grdDetailColumns: {
        No: "No",
        InstrumentCode: "InstrumentCode",
        Qty: "Qty",
        Cost: "Cost",
        Total: "Total",
        SlipNo: "SlipNo",
        TransferDate: "TransferDate"
    }
};

IVS288.EventHandlers = {
    grdSearchResult_OnLoadedData: function (gen_ctrl) {
        var grid = IVS288.Grids.grdSearchResult;

        for (var i = 0; i < grid.getRowsNum(); i++) {
            var row_id = grid.getRowId(i);
            if (gen_ctrl) {
                GenerateDetailButton(IVS288.Grids.grdSearchResult, IVS288.Grids.grdSearchResultColumns.DetailButton, row_id, IVS288.Grids.grdSearchResultColumns.DetailButton, true);
            }

            var isSelected = grid.cells(row_id, grid.getColIndexById(IVS288.Grids.grdSearchResultColumns.SelectedFlag)).getValue();
            grid.cells(row_id, grid.getColIndexById(IVS288.Grids.grdSearchResultColumns.SelectButton))
                .setValue(GenerateCheckBox2(IVS288.Grids.grdSearchResultColumns.SelectButton, row_id, "", true, isSelected));

            BindGridButtonClickEvent(IVS288.Grids.grdSearchResultColumns.DetailButton, row_id, IVS288.EventHandlers.grdSearchResult_OnDetail);
            BindGridCheckBoxClickEvent(IVS288.Grids.grdSearchResultColumns.SelectButton, row_id, IVS288.EventHandlers.chkSelect_OnClick);
        }

        $(IVS288.CtrlID.chkSelectAll).click(IVS288.EventHandlers.chkSelectAll_OnClick);
        grid.setSizes();
    },

    grdSearchResult_OnDetail: function (row_id) {
        var grid = IVS288.Grids.grdSearchResult;
        var row_index = grid.getRowIndex(row_id);
        grid.selectRow(row_index);

        var slipNo = grid.cells(row_id, grid.getColIndexById(IVS288.Grids.grdSearchResultColumns.SlipNo)).getValue();
        var TransferDate = grid.cells(row_id, grid.getColIndexById(IVS288.Grids.grdSearchResultColumns.TransferDate)).getValue();

        var obj = {
            slipNo: [slipNo]
        };

        $(IVS288.CtrlID.txtSlipNoSelected).val(slipNo);
        $(IVS288.CtrlID.txtTransferDateSelected).val(TransferDate);

        $(IVS288.CtrlID.divDetailGrid).LoadDataToGrid(IVS288.Grids.grdDetail, ROWS_PER_PAGE_FOR_VIEWPAGE, true, "/inventory/IVS288_GetDetail", obj, "dtChangeAreaReportDetail", false
            , function (res) {
                master_event.ScrollWindow(IVS288.CtrlID.divDetail);
            }
            , function (res) {
                $(IVS288.CtrlID.divDetail).show();
            }
        );

        return false;
    },

    chkSelect_OnClick: function (row_id, checked) {
        var grid = IVS288.Grids.grdSearchResult;
        grid.cells(row_id, grid.getColIndexById(IVS288.Grids.grdSearchResultColumns.SelectedFlag)).setValue(checked ? "1" : "");

        var checkedAll = $(IVS288.CtrlID.chkSelectAll).prop("checked");
        if (checkedAll && !checked) {
            $(IVS288.CtrlID.chkSelectAll).attr("checked", false);
        }
    },

    chkSelectAll_OnClick: function () {
        var checked = $(IVS288.CtrlID.chkSelectAll).prop("checked");
        var grid = IVS288.Grids.grdSearchResult;
        if (!CheckFirstRowIsEmpty(grid)) {
            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);
                var ctrl = "#" + GenerateGridControlID(IVS288.Grids.grdSearchResultColumns.SelectButton, row_id);
                $(ctrl).attr("checked", checked);
                grid.cells(row_id, grid.getColIndexById(IVS288.Grids.grdSearchResultColumns.SelectedFlag)).setValue(checked ? "1" : "");
            }
        }
    },

    grdDetail_OnLoadedData: function (gen_ctrl) {
        var grid = IVS288.Grids.grdDetail;

        grid.setSizes();
    },

    btnSearch_OnClick: function () {
        master_event.LockWindow(true);

        var obj = {
            SlipNoStart: $(IVS288.CtrlID.txtSlipNoFrom).val(),
            SlipNoEnd: $(IVS288.CtrlID.txtSlipNoTo).val(),
            TransferDateStart: $(IVS288.CtrlID.txtTransferDateFrom).val(),
            TransferDateEnd: $(IVS288.CtrlID.txtTransferDateTo).val(),
            ContractCode: $(IVS288.CtrlID.txtContractCode).val(),
            SourceAreaCode: $(IVS288.CtrlID.cboSourceArea).val(),
            SourceAreaText: $(IVS288.CtrlID.cboSourceArea + " option:selected").text(),
            DestinationAreaCode: $(IVS288.CtrlID.cboDestinationArea).val(),
            DestinationAreaText: $(IVS288.CtrlID.cboDestinationArea + " option:selected").text()
        };

        $(IVS288.CtrlID.divSearchResultGrid).LoadDataToGrid(IVS288.Grids.grdSearchResult, ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/inventory/IVS288_SearchSlip", obj, "dtChangeAreaReportHeader", false, function (res, controls) {
            if (controls != undefined) {
                VaridateCtrl(controls, controls);
            }

            $(IVS288.CtrlID.divDetail).hide();
            $(IVS288.CtrlID.chkSelectAll).attr("checked", false);
            master_event.ScrollWindow(IVS288.CtrlID.divSearchResult);

            master_event.LockWindow(false);
        });

        return false;
    },

    btnClear_OnClick: function () {
        $(IVS288.CtrlID.frmSearch).clearForm();
        DeleteAllRow(IVS288.Grids.grdSearchResult);
        DeleteAllRow(IVS288.Grids.grdDetail);
        $(IVS288.CtrlID.divDetail).hide();
        $(IVS288.CtrlID.chkSelectAll).attr("checked", false);

        ClearDateFromToControl(IVS288.CtrlID.txtTransferDateFrom, IVS288.CtrlID.txtTransferDateTo);

        IVS288.EventHandlers.OnAreaCodeChanged();

        return false;
    },

    btnDownload_OnClick: function () {
        master_event.LockWindow(true);

        var obj = {
            slipNo: []
        };

        var grid = IVS288.Grids.grdSearchResult;
        if (!CheckFirstRowIsEmpty(grid)) {
            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);
                var checked = grid.cells(row_id, grid.getColIndexById(IVS288.Grids.grdSearchResultColumns.SelectedFlag)).getValue();
                if (checked == "1") {
                    obj.slipNo.push(grid.cells(row_id, grid.getColIndexById(IVS288.Grids.grdSearchResultColumns.SlipNo)).getValue());
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

        ajax_method.CallScreenController("/Inventory/IVS288_GenerateReport", obj, function (result, controls, isWarning) {
            if (result != undefined) {
                download_method.CallDownloadController("ifDownload", "/Inventory/IVS288_Download", null);
            }

            master_event.LockWindow(false);
        }, false);
    },

    btnDownloadSummary_OnClick: function () {
        master_event.LockWindow(true);

        var obj = {
            slipNo: []
        };

        var grid = IVS288.Grids.grdSearchResult;
        for (var i = 0; i < grid.getRowsNum(); i++) {
            var row_id = grid.getRowId(i);
            var checked = grid.cells(row_id, grid.getColIndexById(IVS288.Grids.grdSearchResultColumns.SelectedFlag)).getValue();
            if (checked == "1") {
                obj.slipNo.push(grid.cells(row_id, grid.getColIndexById(IVS288.Grids.grdSearchResultColumns.SlipNo)).getValue());
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

        ajax_method.CallScreenController("/Inventory/IVS288_GenerateReportSummary", obj, function (result, controls, isWarning) {
            if (result != undefined) {
                download_method.CallDownloadController("ifDownload", "/Inventory/IVS288_Download", null);
            }

            master_event.LockWindow(false);
        }, false);
    },

    OnAreaCodeChanged: function () {
        var source = $(IVS288.CtrlID.cboSourceArea).val();
        var dest = $(IVS288.CtrlID.cboDestinationArea).val();

        $(IVS288.CtrlID.cboDestinationArea + " span option").unwrap();
        if (source != "") {
            $(IVS288.CtrlID.cboDestinationArea + " option[value='" + source + "']").wrap("<span style='display: none' />");
        }

        $(IVS288.CtrlID.cboSourceArea + " span option").unwrap();
        if (dest != "") {
            $(IVS288.CtrlID.cboSourceArea + " option[value='" + dest + "']").wrap("<span style='display: none' />");
        }
    }
}


IVS288.InitializeGrid = function () {

    IVS288.Grids.grdSearchResult = $(IVS288.CtrlID.divSearchResultGrid).InitialGrid(
        ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/inventory/IVS288_InitialSearchResultGrid"
        , function () {
            BindOnLoadedEvent(IVS288.Grids.grdSearchResult, IVS288.EventHandlers.grdSearchResult_OnLoadedData);
            $(IVS288.CtrlID.chkSelectAll).click(IVS288.EventHandlers.chkSelectAll_OnClick);
            SpecialGridControl(IVS288.Grids.grdSearchResult, [IVS288.Grids.grdSearchResultColumns.DetailButton, IVS288.Grids.grdSearchResultColumns.SelectButton]);
            IVS288.Grids.grdSearchResultIsBindEvent = true;
        }
    );

    IVS288.Grids.grdDetail = $(IVS288.CtrlID.divDetailGrid).InitialGrid(
        ROWS_PER_PAGE_FOR_VIEWPAGE, true, "/inventory/IVS288_InitialDetailGrid"
        , function () {
            BindOnLoadedEvent(IVS288.Grids.grdDetail, IVS288.EventHandlers.grdDetail_OnLoadedData);
            IVS288.Grids.grdDetailIsBindEvent = true;
        }
    );

};


IVS288.InitializeScreen = function () {
    InitialDateFromToControl(IVS288.CtrlID.txtTransferDateFrom, IVS288.CtrlID.txtTransferDateTo);

    $(IVS288.CtrlID.btnSearch).click(IVS288.EventHandlers.btnSearch_OnClick);
    $(IVS288.CtrlID.btnClear).click(IVS288.EventHandlers.btnClear_OnClick);
    $(IVS288.CtrlID.btnDownload).click(IVS288.EventHandlers.btnDownload_OnClick);
    $(IVS288.CtrlID.btnDownloadSummary).click(IVS288.EventHandlers.btnDownloadSummary_OnClick);

    $(IVS288.CtrlID.divSearch).css("visibility", "inherit");
    $(IVS288.CtrlID.divSearchResult).css("visibility", "inherit");
    $(IVS288.CtrlID.divDetail).css("visibility", "inherit");

    $(IVS288.CtrlID.cboSourceArea + "," + IVS288.CtrlID.cboDestinationArea).change(IVS288.EventHandlers.OnAreaCodeChanged);
};

IVS288.ResetScreen = function () {
    $(IVS288.CtrlID.divSearch).show();
    $(IVS288.CtrlID.divDetail).hide();

    $(IVS288.CtrlID.divSearchResult).clearForm();
    $(IVS288.CtrlID.divDetail).clearForm();

    IVS288.EventHandlers.OnAreaCodeChanged();

    IVS288.Grids.grdSearchResult_DeleteAllRows();
    IVS288.Grids.grdDetail_DeleteAllRows();

    $(IVS288.CtrlID.divSearch).find(".highlight").toggleClass("highlight", false);
    $(IVS288.CtrlID.divSearchResult).find(".highlight").toggleClass("highlight", false);
    $(IVS288.CtrlID.divDetail).find(".highlight").toggleClass("highlight", false);
};

$(document).ready(function () {
    IVS288.InitializeGrid();
    IVS288.InitializeScreen();
    IVS288.ResetScreen();
});
