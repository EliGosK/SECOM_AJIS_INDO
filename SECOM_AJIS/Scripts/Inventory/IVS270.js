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

var IVS270 = {};
var currencyTotal = 0;

IVS270.CtrlID = {
    divProject: "#divProject",
    frmProject: "#frmProject",
    txtProjectCode: "#txtProjectCode",
    btnProjectRetrieve: "#btnProjectRetrieve",
    btnProjectCancel: "#btnProjectCancel",
    txtProjectName: "#txtProjectName",
    txtProjectManager: "#txtProjectManager",
    txtProjectAddress: "#txtProjectAddress",

    divSearchInstrument: "#divSearchInstrument",
    frmSearchInstrument: "#frmSearchInstrument",
    txtOffice: "#txtOffice",
    txtLocation: "#txtLocation",
    txtInstrumentCode: "#txtInstrumentCode",
    txtInstrumentName: "#txtInstrumentName",
    cboInstrumentArea: "#cboInstrumentArea",
    txtShelfNoFrom: "#txtShelfNoFrom",
    txtShelfNoTo: "#txtShelfNoTo",
    btnSearch: "#btnSearch",
    btnClear: "#btnClear",
    divSearchInstrumentResultGrid: "#divSearchInstrumentResultGrid",

    divStockOutDetail: "#divStockOutDetail",
    txtMemo: "#txtMemo",
    divStockOutInstGrid: "#divStockOutInstGrid",

    divShowSlip: "#divShowSlip",
    txtShowSlipNo: "#txtShowSlipNo",
    btnDownloadSlip: "#btnDownloadSlip",
    btnNewRegister: "#btnNewRegister"
};

IVS270.GridInstrumentResultColId = {
    Instrumentcode: "Instrumentcode",
    InstrumentName: "InstrumentName",
    AreaCodeName: "AreaCodeName",
    ShelfNo: "ShelfNo",
    InstrumentQty: "InstrumentQty",
    SelectButton: "SelectButton",
    AreaCode: "AreaCode",
    AreaNameShort: "AreaNameShort",
    UnitPrice: "UnitPrice"
};

IVS270.GridStockOutInstColId = {
    InstrumentCode: "InstrumentCode",
    InstrumentName: "InstrumentName",
    AreaNameShort: "AreaNameShort",
    ShelfNo: "ShelfNo",
    CurrentStockQty: "CurrentStockQty",
    StockOutQty: "StockOutQty",
    StockOutAmt: "StockOutAmt",
    RemoveButton: "RemoveButton",
    AreaCode: "AreaCode",
    InputOrder: "InputOrder",
    UnitPrice: "UnitPrice"
};

IVS270.Grids = {
    grdInstrumentResult: null,
    grdInstrumentResultIsBindEvent: false,
    grdInstrumentResult_DeleteAllRows: function () {
        if (IVS270.Grids.grdInstrumentResultIsBindEvent) {
            DeleteAllRow(IVS270.Grids.grdInstrumentResult);
        }
    },

    grdStockOutInst: null,
    grdStockOutInstIsBindEvent: false,
    grdStockOutInst_DeleteAllRows: function () {
        if (IVS270.Grids.grdStockOutInstIsBindEvent) {
            DeleteAllRow(IVS270.Grids.grdStockOutInst);
            IVS270.Grids.grdStockOutInst_InitialSummaryRow();
        }
    },

    grdStockOutInst_NewData: function (InstrumentCode, InstrumentName, AreaNameShort, ShelfNo, CurrentStockQty, StockOutQty, StockOutAmt, RemoveButton, AreaCode, AreaCodeName, InputOrder, UnitPrice) {
        return [
            InstrumentCode,
            InstrumentName,
            AreaNameShort,
            ShelfNo,
            CurrentStockQty,
            StockOutQty,
            //StockOutAmt,
            "0.00",
            RemoveButton,
            "",
            AreaCode,
            AreaCodeName,
            InputOrder,
            UnitPrice
        ];
    },

    grdStockOutInst_NewDataSummary: function (InstrumentCode, InstrumentName, AreaNameShort, ShelfNo, CurrentStockQty, StockOutQty, StockOutAmt, RemoveButton, AreaCode, AreaCodeName, InputOrder) {
        return [
            InstrumentCode,
            InstrumentName,
            AreaNameShort,
            ShelfNo,
            CurrentStockQty,
            StockOutQty,
            //StockOutAmt,
            StockOutAmt,
            RemoveButton,
            "",
            AreaCode,
            AreaCodeName,
            InputOrder
        ];
    },

    grdStockOutInst_InitialSummaryRow: function () {
        var grid = IVS270.Grids.grdStockOutInst;

        CheckFirstRowIsEmpty(grid, true);
        var row_id = grid.uid();
        grid.addRow(row_id
            , IVS270.Grids.grdStockOutInst_NewDataSummary(IVS270_Constant.LabelTotalProjectStockOut, null, null, null, null, "0", null, "", null, null, 9999)
            , grid.getRowsNum());
        grid.setColspan(row_id, grid.getColIndexById(IVS270.GridStockOutInstColId.InstrumentCode), 5);
        grid.setCellTextStyle(row_id, grid.getColIndexById(IVS270.GridStockOutInstColId.InstrumentCode), 'text-align:right');

//        grid.cells2(grid.getRowsNum() - 1, grid.getColIndexById(IVS270.GridStockOutInstColId.RemoveButton))
//            .setValue(GenerateHtmlButton(IVS270.GridStockOutInstColId.RemoveButton, row_id, "Calculate", true));
//        BindGridHtmlButtonClickEvent(IVS270.GridStockOutInstColId.RemoveButton, row_id, function (rid) {
//            IVS270.EventHandlers.CalculateButton_OnClick(grid, rid);
//        });

        var row_idx = grid.getRowsNum() - 1;
        row_id = grid.getRowId(row_idx);

        GenerateCalculateButton(grid, IVS270.GridStockOutInstColId.RemoveButton, row_id, IVS270.GridStockOutInstColId.RemoveButton, true);
        BindGridButtonClickEvent(IVS270.GridStockOutInstColId.RemoveButton, row_id, function (rid) {
            IVS270.EventHandlers.CalculateButton_OnClick(grid, rid);
        });

    },

    grdStockOutInst_AddData: function (strInstrumentcode, strInstrumentName, strAreaCode, strAreaNameShort, strAreaCodeName, strShelfNo, intInstrumentQty, UnitPrice) {
        var grid = IVS270.Grids.grdStockOutInst;
        var row_index = grid.getRowsNum() - 1;
        if (row_index < 0)
        {
            row_index = 0;
            IVS270.Grids.grdStockOutInst_InitialSummaryRow();
        }
        var row_id = grid.uid();

        grid.addRow(row_id
            , IVS270.Grids.grdStockOutInst_NewData(strInstrumentcode, strInstrumentName, strAreaNameShort, strShelfNo, intInstrumentQty, "", "", "", strAreaCode, strAreaCodeName, row_index, UnitPrice)
            , row_index);
        grid.cells2(row_index, grid.getColIndexById(IVS270.GridStockOutInstColId.AreaNameShort)).setAttribute("title", strAreaCodeName);

        GenerateNumericBox2(grid, IVS270.GridStockOutInstColId.StockOutQty
            , row_id, IVS270.GridStockOutInstColId.StockOutQty
            , "", 5, 0, 0, 99999, false, true);
        GenerateRemoveButton(grid, IVS270.GridStockOutInstColId.RemoveButton, row_id, IVS270.GridStockOutInstColId.RemoveButton, true);

        BindGridButtonClickEvent(IVS270.GridStockOutInstColId.RemoveButton, row_id, function (rid) {
            IVS270.EventHandlers.RemoveButton_OnClick(grid, rid);
        });
        grid.setSizes();
    },

    grdStockOutInst_GetData: function () {
        var grid = IVS270.Grids.grdStockOutInst;
        var tmpArray = [];

        for (var i = 0; i < grid.getRowsNum() - 1; i++) {
            var row_id = grid.getRowId(i);
            //AreaCodeName
            var strTmpInstrumentcode = grid.cells2(i, grid.getColIndexById(IVS270.GridStockOutInstColId.InstrumentCode)).getValue();
            var strTmpAreaCode = grid.cells2(i, grid.getColIndexById(IVS270.GridStockOutInstColId.AreaCode)).getValue();
            var strTmpShelfNo = grid.cells2(i, grid.getColIndexById(IVS270.GridStockOutInstColId.ShelfNo)).getValue();
            var intTmpCurrentStockQTy = grid.cells2(i, grid.getColIndexById(IVS270.GridStockOutInstColId.CurrentStockQty)).getValue();
            var intInputOrder = grid.cells2(i, grid.getColIndexById(IVS270.GridStockOutInstColId.InputOrder)).getValue();
            var intUnitPrice = grid.cells2(i, grid.getColIndexById(IVS270.GridStockOutInstColId.UnitPrice)).getValue();
            
            var txtStockOutQtyCtrlID = GenerateGridControlID(IVS270.GridStockOutInstColId.StockOutQty, row_id);

            var obj = {
                OfficeCode: IVS270_Constant.InvHeadOfficeCode,
                LocationCode: IVS270_Constant.LocationCode,
                AreaCode: strTmpAreaCode,
                ShelfNo: strTmpShelfNo,
                InstrumentCode: strTmpInstrumentcode,
                CurrentStockQty: intTmpCurrentStockQTy,
                StockOutQty: $("#" + txtStockOutQtyCtrlID).NumericValue(),
                StockOutQtyCtrlID: txtStockOutQtyCtrlID,
                GridRowId: row_id,
                InputOrder: intInputOrder,
                UnitPrice: intUnitPrice
            };

            tmpArray.push(obj);
        }

        return tmpArray;
    },

    grdStockOutInst_GetDataConfirm: function () {
        var grid = IVS270.Grids.grdStockOutInst;
        var tmpArray = [];

        for (var i = 0; i < grid.getRowsNum() - 1; i++) {
            var row_id = grid.getRowId(i);
            //AreaCodeName
            var strTmpInstrumentcode = grid.cells2(i, grid.getColIndexById(IVS270.GridStockOutInstColId.InstrumentCode)).getValue();
            var strTmpAreaCode = grid.cells2(i, grid.getColIndexById(IVS270.GridStockOutInstColId.AreaCode)).getValue();
            var strTmpShelfNo = grid.cells2(i, grid.getColIndexById(IVS270.GridStockOutInstColId.ShelfNo)).getValue();
            var intTmpCurrentStockQTy = grid.cells2(i, grid.getColIndexById(IVS270.GridStockOutInstColId.CurrentStockQty)).getValue();
            var intInputOrder = grid.cells2(i, grid.getColIndexById(IVS270.GridStockOutInstColId.InputOrder)).getValue();
            var intStockOutAmt = (grid.cells2(i, grid.getColIndexById(IVS270.GridStockOutInstColId.StockOutAmt)).getValue()).split(" ");
            var intUnitPrice = grid.cells2(i, grid.getColIndexById(IVS270.GridStockOutInstColId.UnitPrice)).getValue();

            var txtStockOutQtyCtrlID = GenerateGridControlID(IVS270.GridStockOutInstColId.StockOutQty, row_id);
            if (intStockOutAmt[0] == "Rp.") {
                var intStockOutAmtCurrency = C_CURRENCY_LOCAL;
            }
            else {
                var intStockOutAmtCurrency = C_CURRENCY_US;
            }

            var obj = {
                OfficeCode: IVS270_Constant.InvHeadOfficeCode,
                LocationCode: IVS270_Constant.LocationCode,
                AreaCode: strTmpAreaCode,
                ShelfNo: strTmpShelfNo,
                InstrumentCode: strTmpInstrumentcode,
                CurrentStockQty: intTmpCurrentStockQTy,
                StockOutAmount: intStockOutAmt[1],
                StockOutAmountCurrencyType: intStockOutAmtCurrency,
                StockOutQty: $("#" + txtStockOutQtyCtrlID).NumericValue(),
                StockOutQtyCtrlID: txtStockOutQtyCtrlID,
                GridRowId: row_id,
                InputOrder: intInputOrder,
                UnitPrice: intUnitPrice
            };

            tmpArray.push(obj);
        }

        return tmpArray;
    },

    grdStockOutInst_UpdateData: function (data) {
        if (data == undefined || data == null) {
            return false;
        }

        var grid = IVS270.Grids.grdStockOutInst;

        //var intTotalStockOutQty = 0;
        //var docTotalStockOutAmount = 0;
        for (var i = 0; i < data.ResultData.length; i++) {
            var row_id = data.ResultData[i].GridRowId;
            var intCurrentStockQty = data.ResultData[i].CurrentStockQty;
            var intStockOutQty = data.ResultData[i].StockOutQty;
            var decStockOutAmount = data.ResultData[i].StockOutAmount;

            grid.cells(row_id, grid.getColIndexById(IVS270.GridStockOutInstColId.CurrentStockQty)).setValue(intCurrentStockQty);
            //grid.cells(row_id, grid.getColIndexById(IVS270.GridStockOutInstColId.StockOutAmt)).setValue(decStockOutAmount);
            grid.cells(row_id, grid.getColIndexById(IVS270.GridStockOutInstColId.StockOutAmt)).setValue(data.ResultData[i].TextTransferAmount);
            currencyTotal = (data.ResultData[i].TextTransferAmount).split(" ");

            //intTotalStockOutQty += intStockOutQty;
            //docTotalStockOutAmount += decStockOutAmount;
        }

        var intSummaryRow = grid.getRowsNum() - 1;
        grid.cells2(intSummaryRow, grid.getColIndexById(IVS270.GridStockOutInstColId.StockOutQty)).setValue(data.TotalQtyText);

        //Qty is NULL
        if (currencyTotal[0] == "-")
        {
            grid.cells2(intSummaryRow, grid.getColIndexById(IVS270.GridStockOutInstColId.StockOutAmt)).setValue(currencyTotal[0]);
        }
        else
        {
            grid.cells2(intSummaryRow, grid.getColIndexById(IVS270.GridStockOutInstColId.StockOutAmt)).setValue(currencyTotal[0] + ' ' + chkNum(data.TotalAmount));
        }  
    },

    IsSummaryRow: function(row_index) {
        var grid = IVS270.Grids.grdStockOutInst;
        if (grid.cells(row_index, grid.getColIndexById(IVS270.GridStockOutInstColId.InputOrder)).getValue() == 9999) {
            return true;
        }
        else {
            return false;
        }
    },

    SortStockOutByInstrument: function (a, b, order, aid, bid) {
        var sorting_cols = [
            IVS270.GridStockOutInstColId.InstrumentCode,
            IVS270.GridStockOutInstColId.AreaCode,
            IVS270.GridStockOutInstColId.ShelfNo
        ];

        var grid = IVS270.Grids.grdStockOutInst;

        // Summary Row
        if (IVS270.Grids.IsSummaryRow(aid)) {
            return 1;
        }
        if (IVS270.Grids.IsSummaryRow(bid)) {
            return -1;
        }

        var sorting = function (level) {
            if (level >= sorting_cols.length) {
                if (order == "asc")
                    return a > b ? 1 : -1;
                else
                    return a < b ? 1 : -1;
            }

            var v1 = grid.cells(aid, grid.getColIndexById(sorting_cols[level])).getValue();
            var v2 = grid.cells(bid, grid.getColIndexById(sorting_cols[level])).getValue();

            if (v1 == v2) {
                return sorting(level + 1);
            }

            if (order == "asc")
                return v1 > v2 ? 1 : -1;
            else
                return v1 < v2 ? 1 : -1;
        };

        return sorting(0);
    }

};

IVS270.EventHandlers = {

    grdInstrumentResult_OnLoadedData: function (gen_ctrl) {
        var grid = IVS270.Grids.grdInstrumentResult;

        for (var i = 0; i < grid.getRowsNum(); i++) {
            var row_id = grid.getRowId(i);
            if (gen_ctrl) {
                GenerateAddButton(grid, IVS270.GridInstrumentResultColId.SelectButton, row_id, IVS270.GridInstrumentResultColId.SelectButton, true);
            }
            BindGridButtonClickEvent(IVS270.GridInstrumentResultColId.SelectButton, row_id, function (rid) {
                IVS270.EventHandlers.SelectButton_OnClick(grid, rid);
            });
        }

        grid.setSizes();
    },

    grdStockOutInst_OnLoadedData: function () {
        var grid = IVS270.Grids.grdStockOutInst;
        grid.setSizes();
    },

    btnProjectRetrieve_OnClick: function () {
        $(IVS270.CtrlID.btnProjectRetrieve).SetDisabled(true);
        master_event.LockWindow(true);
        $(IVS270.CtrlID.divProject).find(".highlight").toggleClass("highlight", false);

        var params = {
            strProjectCode: $(IVS270.CtrlID.txtProjectCode).val()
        };

        ajax_method.CallScreenController("/inventory/IVS270_GetProjectInformation", params, function (result, controls) {
            if (result.IsSuccess == false) {
                VaridateCtrl(controls, controls);

                $(IVS270.CtrlID.txtProjectName).val(null);
                $(IVS270.CtrlID.txtProjectManager).val(null);
                $(IVS270.CtrlID.txtProjectAddress).val(null);

                $(IVS270.CtrlID.txtProjectCode).SetDisabled(false);
                $(IVS270.CtrlID.btnProjectRetrieve).SetDisabled(false);
                $(IVS270.CtrlID.btnProjectCancel).SetDisabled(true);
            } else if (result.IsSuccess == true) {
                $(IVS270.CtrlID.txtProjectCode).val(result.ProjectInformation.ProjectCode); //Add by Jutarat A. on 30052013
                $(IVS270.CtrlID.txtProjectName).val(result.ProjectInformation.ProjectName);
                $(IVS270.CtrlID.txtProjectManager).val(result.ProjectInformation.ProjectManagerName);
                $(IVS270.CtrlID.txtProjectAddress).val(result.ProjectInformation.ProjectAddress);

                $(IVS270.CtrlID.txtProjectCode).SetDisabled(true);
                $(IVS270.CtrlID.btnProjectRetrieve).SetDisabled(true);
                $(IVS270.CtrlID.btnProjectCancel).SetDisabled(false);
            } else {
                $(IVS270.CtrlID.btnProjectRetrieve).SetDisabled(false);
                $(IVS270.CtrlID.btnProjectCancel).SetDisabled(true);
            }
            master_event.LockWindow(false);
        });

        return false;
    },

    btnProjectCancel_OnClick: function () {
        $(IVS270.CtrlID.btnProjectCancel).SetDisabled(true);

        $(IVS270.CtrlID.divProject).clearForm();

        $(IVS270.CtrlID.txtProjectCode).SetDisabled(false);
        $(IVS270.CtrlID.btnProjectRetrieve).SetDisabled(false);
        $(IVS270.CtrlID.btnProjectCancel).SetDisabled(true);

        return false;
    },

    btnSearch_OnClick: function () {
        $(IVS270.CtrlID.btnSearch).SetDisabled(true);
        master_event.LockWindow(true);

        var param = $(IVS270.CtrlID.frmSearchInstrument).serializeObject2();

        if (param.AreaCode == "") {
            param.AreaCodeList = [];
            $(IVS270.CtrlID.cboInstrumentArea + " > option").each(function (index) {
                if (index > 0) {
                    param.AreaCodeList.push($(this).val());
                }
            });
        }

        $(IVS270.CtrlID.divSearchInstrumentResultGrid).LoadDataToGrid(
            IVS270.Grids.grdInstrumentResult, ROWS_PER_PAGE_FOR_SEARCHPAGE
            , false, "/inventory/IVS270_SearchInventoryInstrumentList"
            , param, "dtSearchInstrumentListResult", false
            , function (result, controls) {
                $(IVS270.CtrlID.btnSearch).SetDisabled(false);
                master_event.LockWindow(false);

                master_event.ScrollWindow(IVS270.CtrlID.divSearchInstrumentResultGrid);
            }
            , function (result, controls, isWarning) {
                //Do Nothing
            }
        );

        return false;
    },

    btnClear_OnClick: function () {
        $(IVS270.CtrlID.divSearchInstrument).clearForm();

        $(IVS270.CtrlID.txtOffice).val(IVS270_Constant.InvHeadOfficeName);
        $(IVS270.CtrlID.txtLocation).val(IVS270_Constant.LocationName);

        IVS270.Grids.grdInstrumentResult_DeleteAllRows();

        return false;
    },

    SelectButton_OnClick: function (grdInstrument, rid) {

        var row_index = grdInstrument.getRowIndex(rid);
        grdInstrument.selectRow(row_index);

        $(IVS270.CtrlID.divStockOutDetail).show();

        GridControl.LockGrid(grdInstrument);
        GridControl.SetDisabledButtonOnGrid(grdInstrument, IVS270.GridInstrumentResultColId.SelectButton, IVS270.GridInstrumentResultColId.SelectButton, true);

        register_command.SetCommand(IVS270.EventHandlers.cmdRegister_OnClick);
        reset_command.SetCommand(IVS270.EventHandlers.cmdReset_OnClick);
        confirm_command.SetCommand(null);
        back_command.SetCommand(null);

        var strInstrumentcode = grdInstrument.cells2(row_index, grdInstrument.getColIndexById(IVS270.GridInstrumentResultColId.Instrumentcode)).getValue();
        var strInstrumentName = grdInstrument.cells2(row_index, grdInstrument.getColIndexById(IVS270.GridInstrumentResultColId.InstrumentName)).getValue();
        var strAreaNameShort = grdInstrument.cells2(row_index, grdInstrument.getColIndexById(IVS270.GridInstrumentResultColId.AreaNameShort)).getValue();
        var strAreaCodeName = grdInstrument.cells2(row_index, grdInstrument.getColIndexById(IVS270.GridInstrumentResultColId.AreaCodeName)).getValue();
        var strAreaCode = grdInstrument.cells2(row_index, grdInstrument.getColIndexById(IVS270.GridInstrumentResultColId.AreaCode)).getValue();
        var strShelfNo = grdInstrument.cells2(row_index, grdInstrument.getColIndexById(IVS270.GridInstrumentResultColId.ShelfNo)).getValue();
        var intInstrumentQty = grdInstrument.cells2(row_index, grdInstrument.getColIndexById(IVS270.GridInstrumentResultColId.InstrumentQty)).getValue();
        var intUnitPrice = grdInstrument.cells2(row_index, grdInstrument.getColIndexById(IVS270.GridInstrumentResultColId.UnitPrice)).getValue();

        if (IVS270.IsSelectedInstrument(strInstrumentcode, strAreaCode, strShelfNo)) {
            var messageParam = { "module": "Inventory", "code": "MSG4005", "param": "" };
            call_ajax_method("/Shared/GetMessage", messageParam, function (data) {
                OpenWarningDialog(data.Message);

                GridControl.UnlockGrid(grdInstrument);
                GridControl.SetDisabledButtonOnGrid(grdInstrument, IVS270.GridInstrumentResultColId.SelectButton, IVS270.GridInstrumentResultColId.SelectButton, false);
            });
            return;
        }

        IVS270.Grids.grdStockOutInst_AddData(strInstrumentcode, strInstrumentName, strAreaCode, strAreaNameShort, strAreaCodeName, strShelfNo, intInstrumentQty, intUnitPrice);

        GridControl.UnlockGrid(grdInstrument);
        GridControl.SetDisabledButtonOnGrid(grdInstrument, IVS270.GridInstrumentResultColId.SelectButton, IVS270.GridInstrumentResultColId.SelectButton, false);

        master_event.ScrollWindow(IVS270.CtrlID.divStockOutDetail);

        //if(grdInstrument.getRowsNum() == 0)
        //{
        //    IVS270.Grids.grdStockOutInst_InitialSummaryRow();
        //}
    },

    CalculateButton_OnClick: function (grid, rid) {
        $(IVS270.CtrlID.divStockOutInstGrid).find(".highlight").toggleClass("highlight", false);

        GridControl.LockGrid(grid);
        GridControl.SetDisabledButtonOnGrid(grid, IVS270.GridStockOutInstColId.RemoveButton, IVS270.GridStockOutInstColId.RemoveButton, true);

        if (grid.getRowsNum() <= 1) {
            var messageParam = { "module": "Inventory", "code": "MSG4068", "param": "" };
            call_ajax_method("/Shared/GetMessage", messageParam, function (data) {
                OpenWarningDialog(data.Message);

                GridControl.UnlockGrid(grid);
                GridControl.SetDisabledButtonOnGrid(grid, IVS270.GridStockOutInstColId.RemoveButton, IVS270.GridStockOutInstColId.RemoveButton, false);
            });
            return;
        }

        var param = IVS270.Grids.grdStockOutInst_GetData();

        ajax_method.CallScreenController("/inventory/IVS270_CalculateAmount", param, function (result, controls) {
            VaridateCtrl(controls, controls);

            if (result != null) {
                if (result.ResultData != null) {
                    IVS270.Grids.grdStockOutInst_UpdateData(result);
                }
            }

            GridControl.UnlockGrid(grid);
            GridControl.SetDisabledButtonOnGrid(grid, IVS270.GridStockOutInstColId.RemoveButton, IVS270.GridStockOutInstColId.RemoveButton, false);
        });

    },

    RemoveButton_OnClick: function (grid, rid) {
        grid.deleteRow(rid);
        var row_idx = grid.getRowsNum() - 1;
        row_id = grid.getRowId(row_idx);
        if(row_idx == 0)
        {
            //DeleteAllRow(grid);
            grid.deleteRow(row_id)
        }
    },

    cmdRegister_OnClick: function () {
        $(IVS270.CtrlID.divStockOutDetail).find(".highlight").toggleClass("highlight", false);
        $(IVS270.CtrlID.divStockOutInstGrid).find(".highlight").toggleClass("highlight", false);

        var param = {
            IsRetrievePressed: $(IVS270.CtrlID.btnProjectRetrieve).prop("disabled"),
            ProjectCode: $(IVS270.CtrlID.txtProjectCode).val(),
            Memo: $(IVS270.CtrlID.txtMemo).val(),
            Details: IVS270.Grids.grdStockOutInst_GetData()
        };

        ajax_method.CallScreenController("/inventory/IVS270_RegisterProjectStockOut", param, function (result, controls) {
            VaridateCtrl(controls, controls);

            if (result != null) {
                if (result.ResultData != null) {
                    IVS270.Grids.grdStockOutInst_UpdateData(result);
                }

                if (result.IsSuccess == true) {
                    $(IVS270.CtrlID.divSearchInstrument).hide();

                    $(IVS270.CtrlID.divSearchInstrument).clearForm();
                    $(IVS270.CtrlID.txtOffice).val(IVS270_Constant.InvHeadOfficeName);
                    $(IVS270.CtrlID.txtLocation).val(IVS270_Constant.LocationName);

                    $(IVS270.CtrlID.divProject).SetViewMode(true);
                    $(IVS270.CtrlID.divStockOutDetail).SetViewMode(true);

                    if (IVS270.Grids.grdStockOutInst != null) {
                        var colBtnRemoveIdx = IVS270.Grids.grdStockOutInst.getColIndexById(IVS270.GridStockOutInstColId.RemoveButton);
                        IVS270.Grids.grdStockOutInst.setColumnHidden(colBtnRemoveIdx, true);
                    }

                    //                    IVS270.Grids.grdStockOutInst.setCustomSorting(IVS270.Grids.SortStockOutByInstrument, IVS270.Grids.grdStockOutInst.getColIndexById(IVS270.GridStockOutInstColId.InstrumentCode));
                    //                    IVS270.Grids.grdStockOutInst.sortRows(IVS270.Grids.grdStockOutInst.getColIndexById(IVS270.GridStockOutInstColId.InstrumentCode), undefined, "asc");

                    register_command.SetCommand(null);
                    reset_command.SetCommand(null);
                    confirm_command.SetCommand(IVS270.EventHandlers.cmdConfirm_OnClick);
                    back_command.SetCommand(IVS270.EventHandlers.cmdBack_OnClick);
                }
            }
        });

    },

    cmdReset_OnClick: function () {
        IVS270.InitialScreen();
    },

    cmdConfirm_OnClick: function () {
        $(IVS270.CtrlID.divStockOutDetail).find(".highlight").toggleClass("highlight", false);
        $(IVS270.CtrlID.divStockOutInstGrid).find(".highlight").toggleClass("highlight", false);

        var param = {
            ProjectCode: $(IVS270.CtrlID.txtProjectCode).val(),
            Memo: $(IVS270.CtrlID.txtMemo).val(),
            Details: IVS270.Grids.grdStockOutInst_GetDataConfirm()
        };

        ajax_method.CallScreenController("/inventory/IVS270_ConfirmProjectStockOut", param, function (result, controls) {
            VaridateCtrl(controls, controls);

            if (result != null && result.IsSuccess == true) {
                $(IVS270.CtrlID.txtShowSlipNo).val(result.InvSlipNo);

                //$(IVS270.CtrlID.divProject).hide();
                $(IVS270.CtrlID.divSearchInstrument).hide();
                $(IVS270.CtrlID.divStockOutDetail).show();
                $(IVS270.CtrlID.divShowSlip).show();

                register_command.SetCommand(null);
                reset_command.SetCommand(null);
                confirm_command.SetCommand(null);
                back_command.SetCommand(null);

                master_event.ScrollWindow(IVS270.CtrlID.divShowSlip);
            }
            //else {
            //    if (IVS270.EventHandlers.cmdBack_OnClick != undefined) {
            //        IVS270.EventHandlers.cmdBack_OnClick();
            //    }
            //}
        });

    },

    cmdBack_OnClick: function () {
        $(IVS270.CtrlID.divProject).SetViewMode(false);
        $(IVS270.CtrlID.divStockOutDetail).SetViewMode(false);

        if (IVS270.Grids.grdStockOutInst != null) {
            var colBtnRemoveIdx = IVS270.Grids.grdStockOutInst.getColIndexById(IVS270.GridStockOutInstColId.RemoveButton);
            IVS270.Grids.grdStockOutInst.setColumnHidden(colBtnRemoveIdx, false);

            // Akat K. add
            IVS270.Grids.grdStockOutInst.setSizes();
            SetFitColumnForBackAction(IVS270.Grids.grdStockOutInst, "TempColumn");
        }

        $(IVS270.CtrlID.divSearchInstrument).show();

        //        IVS270.Grids.grdStockOutInst.sortRows(IVS270.Grids.grdStockOutInst.getColIndexById(IVS270.GridStockOutInstColId.InputOrder), undefined, "asc");

        register_command.SetCommand(IVS270.EventHandlers.cmdRegister_OnClick);
        reset_command.SetCommand(IVS270.EventHandlers.cmdReset_OnClick);
        confirm_command.SetCommand(null);
        back_command.SetCommand(null);

    },

    btnDownloadSlip_OnClick: function () {
        var param = {
            strInvSlipNo: $(IVS270.CtrlID.txtShowSlipNo).val()
        };

        ajax_method.CallScreenController("/inventory/IVS270_DownloadDocument", param, function (result, controls) {
            if (result != null) {
                var key = ajax_method.GetKeyURL(null);
                var url = ajax_method.GenerateURL("/inventory/IVS270_DownloadPdfAndWriteLog?k=" + key + "&strDocumentNo=" + result.DocumentNo + "&documentOCC=" + result.DocumentOCC + "&strDocumentCode=" + result.DocumentCode + "&fileName=" + result.FilePath)
                window.open(url, "Report", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
            }
        });
    },

    btnNewRegister_OnClick: function () {
        IVS270.InitialScreen();
    }

};

IVS270.InitializeGrid = function () {

    IVS270.Grids.grdInstrumentResult = $(IVS270.CtrlID.divSearchInstrumentResultGrid).InitialGrid(
        ROWS_PER_PAGE_FOR_VIEWPAGE, false, "/inventory/IVS270_InitialInstrumentResult"
        , function () {
            BindOnLoadedEvent(IVS270.Grids.grdInstrumentResult, IVS270.EventHandlers.grdInstrumentResult_OnLoadedData);
            IVS270.Grids.grdInstrumentResultIsBindEvent = true;
        }
    );

    SpecialGridControl(IVS270.Grids.grdInstrumentResult, [IVS270.GridInstrumentResultColId.SelectButton]);

    IVS270.Grids.grdStockOutInst = $(IVS270.CtrlID.divStockOutInstGrid).InitialGrid(
        0, false, "/inventory/IVS270_InitialStockOutDetail"
        , function () {
            BindOnLoadedEvent(IVS270.Grids.grdStockOutInst, IVS270.EventHandlers.grdStockOutInst_OnLoadedData);
            IVS270.Grids.grdStockOutInstIsBindEvent = true;
            IVS270.Grids.grdStockOutInst_InitialSummaryRow();
        }
    );

    SpecialGridControl(IVS270.Grids.grdStockOutInst, [IVS270.GridStockOutInstColId.RemoveButton, IVS270.GridStockOutInstColId.StockOutQty]);

};

IVS270.IsSelectedInstrument = function (strInstrumentcode, strAreaCode, strShelfNo) {
    var grid = IVS270.Grids.grdStockOutInst;

    if (!CheckFirstRowIsEmpty(grid)) {
        for (var i = 0; i < grid.getRowsNum() - 1; i++) {
            var row_id = grid.getRowId(i);

            var strTmpInstrumentcode = grid.cells2(i, grid.getColIndexById(IVS270.GridStockOutInstColId.InstrumentCode)).getValue();
            var strTmpAreaCode = grid.cells2(i, grid.getColIndexById(IVS270.GridStockOutInstColId.AreaCode)).getValue();
            var strTmpShelfNo = grid.cells2(i, grid.getColIndexById(IVS270.GridStockOutInstColId.ShelfNo)).getValue();

            if (strTmpInstrumentcode == strInstrumentcode
                && strTmpAreaCode == strAreaCode
                && strTmpShelfNo == strShelfNo) 
            {
                return true;
            }
        }
    }

    return false;
};

IVS270.InitializeScreen = function () {
    $(IVS270.CtrlID.btnProjectRetrieve).click(IVS270.EventHandlers.btnProjectRetrieve_OnClick);
    $(IVS270.CtrlID.btnProjectCancel).click(IVS270.EventHandlers.btnProjectCancel_OnClick);
    $(IVS270.CtrlID.btnSearch).click(IVS270.EventHandlers.btnSearch_OnClick);
    $(IVS270.CtrlID.btnClear).click(IVS270.EventHandlers.btnClear_OnClick);
    $(IVS270.CtrlID.btnDownloadSlip).click(IVS270.EventHandlers.btnDownloadSlip_OnClick);
    $(IVS270.CtrlID.btnNewRegister).click(IVS270.EventHandlers.btnNewRegister_OnClick);

    $(IVS270.CtrlID.txtMemo).SetMaxLengthTextArea(1000);

    $(IVS270.CtrlID.txtShowSlipNo).SetDisabled(true);

    $(IVS270.CtrlID.txtProjectName).SetDisabled(true);
    $(IVS270.CtrlID.txtProjectManager).SetDisabled(true);
    $(IVS270.CtrlID.txtProjectAddress).SetDisabled(true);
    $(IVS270.CtrlID.txtOffice).SetDisabled(true);
    $(IVS270.CtrlID.txtLocation).SetDisabled(true);

    $(IVS270.CtrlID.txtInstrumentCode).InitialAutoComplete("/Master/GetInstrumentCodeAll", null, true);

    $(IVS270.CtrlID.divSearchInstrument).css("visibility", "inherit");
    $(IVS270.CtrlID.divStockOutDetail).css("visibility", "inherit");
    $(IVS270.CtrlID.divShowSlip).css("visibility", "inherit");
};

IVS270.InitialScreen = function () {
    $(IVS270.CtrlID.divProject).show();
    $(IVS270.CtrlID.divSearchInstrument).show();
    $(IVS270.CtrlID.divStockOutDetail).hide();
    $(IVS270.CtrlID.divShowSlip).hide();

    $(IVS270.CtrlID.divProject).clearForm();
    $(IVS270.CtrlID.divSearchInstrument).clearForm();
    $(IVS270.CtrlID.divStockOutDetail).clearForm();
    $(IVS270.CtrlID.divShowSlip).clearForm();

    $(IVS270.CtrlID.txtOffice).val(IVS270_Constant.InvHeadOfficeName);
    $(IVS270.CtrlID.txtLocation).val(IVS270_Constant.LocationName);

    $(IVS270.CtrlID.txtProjectCode).SetDisabled(false);
    $(IVS270.CtrlID.btnProjectRetrieve).SetDisabled(false);
    $(IVS270.CtrlID.btnProjectCancel).SetDisabled(true);

    register_command.SetCommand(IVS270.EventHandlers.cmdRegister_OnClick);
    reset_command.SetCommand(IVS270.EventHandlers.cmdReset_OnClick);
    confirm_command.SetCommand(null);
    back_command.SetCommand(null);

    IVS270.Grids.grdInstrumentResult_DeleteAllRows();
    IVS270.Grids.grdStockOutInst_DeleteAllRows();

    $(IVS270.CtrlID.divProject).find(".highlight").toggleClass("highlight", false);
    $(IVS270.CtrlID.divStockOutDetail).find(".highlight").toggleClass("highlight", false);
    $(IVS270.CtrlID.divStockOutInstGrid).find(".highlight").toggleClass("highlight", false);

    $(IVS270.CtrlID.divProject).SetViewMode(false);
    $(IVS270.CtrlID.divStockOutDetail).SetViewMode(false);

    if (IVS270.Grids.grdStockOutInst != null) {
        var colBtnRemoveIdx = IVS270.Grids.grdStockOutInst.getColIndexById(IVS270.GridStockOutInstColId.RemoveButton);
        IVS270.Grids.grdStockOutInst.setColumnHidden(colBtnRemoveIdx, false);
    }

};

$(document).ready(function () {
    IVS270.InitializeGrid();
    IVS270.InitializeScreen();
    IVS270.InitialScreen();
});

function chkNum(ele) {
    var num = parseFloat(ele);
    //  addCommas(num.toFixed(2));
    return addCommas(num.toFixed(2));
}
