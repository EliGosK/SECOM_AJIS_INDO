
var CMS480 = {

    ScreenMode: {
        Search: 0,
        Edit: 1,
        Confirm: 2
    },

    GridColumnID: {
        SearchResult: {
            MonthYear: "ReportMonthYear",
            BillingCode: "BillingCode",
            ReceivedAmount: "ReceiveAmount", ReceivedAmountID: "txtReceiveAmount",
            IncomeRentalFee: "IncomeRentalFee", IncomeRentalFeeID: "txtIncomeRentalFee",
            AccAdvanceReceive: "AccumulatedReceiveAmount", AccAdvanceReceiveID: "txtAccumulatedReceiveAmount",
            AccUnpaid: "AccumulatedUnpaid", AccUnpaidID: "txtAccumulatedUnpaid",
            IncomeVat: "IncomeVat", IncomeVatID: "txtIncomeVat",
            UnpaidPeriod: "UnpaidPeriod", UnpaidPeriodID: "txtUnpaidPeriod",
            IncomeDate: "IncomeDate", IncomeDateID: "txtIncomeDate",
            TaxRate: "TaxRate",
            MonthlyBillingAmount: "MonthlyBillingAmount"
        }
    },

    Grids: {
        grdSearch: null
    },

    Vars: {
        objSearchResult: null
    },

    jQuery: {
        SearchSection: {
            divSearchSection: function () { return $("#divSearchCarryOverAndProfit"); },
            divSearchCriteria: function () { return $("#divSearchCarryOverAndProfitCriteria"); },
            divSearchResultGrid: function () { return $("#divSearchCarryOverAndProfitGrid"); },
            txtSearchContractCode: function () { return $("#txtSearchContractCode"); },
            txtSearchBillingOCC: function () { return $("#txtSearchBillingOCC"); },
            cboSearchProductType: function () { return $("#cboSearchProductType"); },
            cboSearchMonth: function () { return $("#cboSearchMonth"); },
            cboSearchYear: function () { return $("#cboSearchYear"); },
            btnSearch: function () { return $("#btnSearch"); },
            btnClear: function () { return $("#btnClear"); },
            btnEdit: function () { return $("#btnEdit"); },
            formSearch: function () { return $("#formSearch"); }
        }
    },

    EventHandlers: {
        btnSearch_click: function () {
            CMS480.jQuery.SearchSection.btnEdit().hide();
            CMS480.jQuery.SearchSection.btnSearch().SetDisabled(true);
            master_event.LockWindow(true);

            var params = {
                ReportYear: CMS480.jQuery.SearchSection.cboSearchYear().val(),
                ReportMonth: CMS480.jQuery.SearchSection.cboSearchMonth().val(),
                ProductType: CMS480.jQuery.SearchSection.cboSearchProductType().val(),
                ContractCode: CMS480.jQuery.SearchSection.txtSearchContractCode().val(),
                BillingOCC: CMS480.jQuery.SearchSection.txtSearchBillingOCC().val()
            };

            ajax_method.CallScreenController("/Common/CMS480_SearchManageCarryOverProfit", params, function (result, controls) {

                if (result != null) {
                    if (result.IsSuccess == true) {
                        CMS480.Vars.objSearchResult = result;

                        if (result.ResultData != null && result.ResultData != undefined && result.ResultData.length != 0) {
                            CMS480.jQuery.SearchSection.btnEdit().show();
                            CMS480.jQuery.SearchSection.btnEdit().SetDisabled(false);
                        }

                        CMS480.jQuery.SearchSection.divSearchResultGrid().show();
                        master_event.ScrollWindow("#divSearchCarryOverAndProfitGrid");

                        CMS480.Functions.LoadDataToGrid(result, false);
                        //GridControl.LockGrid(CMS480.Grids.grdSearch);
                    }
                    else if (controls != undefined) {
                        VaridateCtrl(["cboSearchProductType", "cboSearchMonth", "cboSearchYear"], controls);
                    }
                }

                CMS480.jQuery.SearchSection.btnSearch().SetDisabled(false);
                master_event.LockWindow(false);
            });


            return false;
        },

        btnClear_click: function () {
            CMS480.jQuery.SearchSection.divSearchCriteria().clearForm();
            CMS480.jQuery.SearchSection.divSearchResultGrid().clearForm();

            DeleteAllRow(CMS480.Grids.grdSearch);
            CMS480.Functions.SetScreenMode(CMS480.ScreenMode.Search);
            return false;
        },

        btnEdit_click: function () {
            CMS480.Functions.SetScreenMode(CMS480.ScreenMode.Edit);

            CMS480.jQuery.SearchSection.btnEdit().SetDisabled(true);

            CMS480.Functions.LoadDataToGrid(CMS480.Vars.objSearchResult, true);
            //GridControl.UnlockGrid(CMS480.Grids.grdSearch);
            return false;
        },

        cmdRegister_Click: function () {
            var params = CMS480.Functions.GetCarryOverProfitForUpdate();

            ajax_method.CallScreenController("/common/CMS480_ValidateManageCarryOverProfit", params, function (result, controls) {

                //IVS020.jQuery.RegisterSection.divRegisterGrid().find(".highlight").toggleClass("highlight", false);
                //VaridateCtrl(controls, controls);

                if (result != null && result.IsSuccess) {
                    CMS480.Functions.SetEnabledSearchSection(false);
                    CMS480.Functions.SetEnabledGrid(CMS480.Grids.grdSearch, false);
                    //GridControl.LockGrid(CMS480.Grids.grdSearch);
                    CMS480.Functions.SetScreenMode(CMS480.ScreenMode.Confirm);
                }
            });
        },

        cmdReset_Click: function () {
            CMS480.Functions.SetScreenMode(CMS480.ScreenMode.Search);
            CMS480.EventHandlers.btnClear_click();
        },

        cmdBack_Click: function () {
            CMS480.Functions.SetEnabledSearchSection(true);
            CMS480.Functions.SetEnabledGrid(CMS480.Grids.grdSearch, true);
            //GridControl.UnlockGrid(CMS480.Grids.grdSearch);
            CMS480.Functions.SetScreenMode(CMS480.ScreenMode.Edit);
        },

        cmdConfirm_Click: function () {
            master_event.LockWindow(true);
            var params = {
                param: CMS480.Functions.GetCarryOverProfitForUpdate(),
                productTypeCode: CMS480.jQuery.SearchSection.cboSearchProductType().val()
            };

            ajax_method.CallScreenController("/Common/CMS480_UpdateManageCarryOverProfit", params, function (result, controls) {

                //IVS020.jQuery.RegisterSection.divRegisterGrid().find(".highlight").toggleClass("highlight", false);
                //VaridateCtrl(controls, controls);

                if (result != null && result.IsSuccess) {
                    master_event.LockWindow(false);
                    CMS480.Functions.SetEnabledSearchSection(true);
                    CMS480.Functions.SetScreenMode(CMS480.ScreenMode.Search);
                    CMS480.EventHandlers.btnClear_click();
                }
            });
        }

    },

    Functions: {
        InitializeGrid: function () {
            CMS480.Grids.grdSearch = CMS480.jQuery.SearchSection.divSearchResultGrid().InitialGrid(
                0, false, "/Common/CMS480_InitialSearchGrid");

            GridControl.LockGrid(CMS480.Grids.grdSearch);

            SpecialGridControl(CMS480.Grids.grdSearch, [
                CMS480.GridColumnID.SearchResult.ReceivedAmount,
                CMS480.GridColumnID.SearchResult.IncomeRentalFee,
                CMS480.GridColumnID.SearchResult.AccAdvanceReceive,
                CMS480.GridColumnID.SearchResult.AccUnpaid,
                CMS480.GridColumnID.SearchResult.IncomeVat,
                CMS480.GridColumnID.SearchResult.UnpaidPeriod,
                CMS480.GridColumnID.SearchResult.IncomeDate
            ]);
        },

        InitializeScreen: function () {
            CMS480.jQuery.SearchSection.btnSearch().click(CMS480.EventHandlers.btnSearch_click);
            CMS480.jQuery.SearchSection.btnClear().click(CMS480.EventHandlers.btnClear_click);
            CMS480.jQuery.SearchSection.btnEdit().click(CMS480.EventHandlers.btnEdit_click);

            CMS480.jQuery.SearchSection.divSearchSection().css("visibility", "visible");
        },

        SetScreenMode: function (mode) {
            if (mode == CMS480.ScreenMode.Search) {
                CMS480.jQuery.SearchSection.divSearchSection().show();
                CMS480.jQuery.SearchSection.divSearchResultGrid().hide();
                CMS480.jQuery.SearchSection.btnEdit().hide();

                register_command.SetCommand(null);
                reset_command.SetCommand(null);
                confirm_command.SetCommand(null);
                back_command.SetCommand(null);

            } else if (mode == CMS480.ScreenMode.Edit) {
                CMS480.jQuery.SearchSection.divSearchSection().show();
                CMS480.jQuery.SearchSection.divSearchResultGrid().show();
                CMS480.jQuery.SearchSection.btnEdit().show();

                register_command.SetCommand(CMS480.EventHandlers.cmdRegister_Click);
                reset_command.SetCommand(CMS480.EventHandlers.cmdReset_Click);
                confirm_command.SetCommand(null);
                back_command.SetCommand(null);

            } else if (mode == CMS480.ScreenMode.Confirm) {
                CMS480.jQuery.SearchSection.divSearchSection().show();
                CMS480.jQuery.SearchSection.divSearchResultGrid().show();
                CMS480.jQuery.SearchSection.btnEdit().show();

                register_command.SetCommand(null);
                reset_command.SetCommand(null);
                confirm_command.SetCommand(CMS480.EventHandlers.cmdConfirm_Click);
                back_command.SetCommand(CMS480.EventHandlers.cmdBack_Click);

            }
        },

        SetEnabledSearchSection: function (enabled) {
            CMS480.jQuery.SearchSection.cboSearchProductType().SetDisabled(!enabled);
            CMS480.jQuery.SearchSection.cboSearchMonth().SetDisabled(!enabled);
            CMS480.jQuery.SearchSection.cboSearchYear().SetDisabled(!enabled);
            CMS480.jQuery.SearchSection.txtSearchBillingOCC().SetDisabled(!enabled);
            CMS480.jQuery.SearchSection.txtSearchContractCode().SetDisabled(!enabled);
            CMS480.jQuery.SearchSection.btnSearch().SetDisabled(!enabled);
            CMS480.jQuery.SearchSection.btnClear().SetDisabled(!enabled);
        },

        GetCarryOverProfitForUpdate: function () {
            if (CMS480.Grids.grdSearch == undefined) {
                return null;
            }

            var grid = CMS480.Grids.grdSearch;
            var arrTmp = new Array();

            if (!CheckFirstRowIsEmpty(grid)) {
                for (var i = 0; i < grid.getRowsNum(); i++) {
                    if (!grid.cells2(i, grid.getColIndexById(CMS480.GridColumnID.SearchResult.MonthYear)).getAttribute("parent-header")) {
                        var row_id = grid.getRowId(i);

                        var monthYear = grid.cells2(i, grid.getColIndexById(CMS480.GridColumnID.SearchResult.MonthYear)).getValue();
                        var billingCode = grid.cells2(i, grid.getColIndexById(CMS480.GridColumnID.SearchResult.BillingCode)).getValue();

                        var txtReceiveAmountID = GenerateGridControlID(CMS480.GridColumnID.SearchResult.ReceivedAmountID, row_id);
                        var txtIncomeRentalFeeID = GenerateGridControlID(CMS480.GridColumnID.SearchResult.IncomeRentalFeeID, row_id);
                        var txtAccAdvanceReceiveID = GenerateGridControlID(CMS480.GridColumnID.SearchResult.AccAdvanceReceiveID, row_id);
                        var txtAccUnpaidID = GenerateGridControlID(CMS480.GridColumnID.SearchResult.AccUnpaidID, row_id);
                        var txtIncomeVatID = GenerateGridControlID(CMS480.GridColumnID.SearchResult.IncomeVatID, row_id);
                        var txtUnpaidPeriodID = GenerateGridControlID(CMS480.GridColumnID.SearchResult.UnpaidPeriodID, row_id);
                        var txtIncomeDateID = GenerateGridControlID(CMS480.GridColumnID.SearchResult.IncomeDateID, row_id);

                        var obj = {
                            ReportMonthYear: monthYear,
                            BillingCode: billingCode,
                            ReceiveAmount: $("#" + txtReceiveAmountID).NumericValue(),
                            ReceiveAmountCurrencyType: $("#" + txtReceiveAmountID).NumericCurrencyValue(),
                            IncomeRentalFee: $("#" + txtIncomeRentalFeeID).NumericValue(),
                            IncomeRentalFeeCurrencyType: $("#" + txtIncomeRentalFeeID).NumericCurrencyValue(),
                            AccumulatedReceiveAmount: $("#" + txtAccAdvanceReceiveID).NumericValue(),
                            AccumulatedReceiveAmountCurrencyType: $("#" + txtAccAdvanceReceiveID).NumericCurrencyValue(),
                            AccumulatedUnpaid: $("#" + txtAccUnpaidID).NumericValue(),
                            AccumulatedUnpaidCurrencyType: $("#" + txtAccUnpaidID).NumericCurrencyValue(),
                            IncomeVat: $("#" + txtIncomeVatID).NumericValue(),
                            IncomeVatCurrencyType: $("#" + txtIncomeVatID).NumericCurrencyValue(),
                            UnpaidPeriod: $("#" + txtUnpaidPeriodID).NumericValue(),
                            IncomeDate: $("#" + txtIncomeDateID).val()
                        };
                        arrTmp.push(obj);
                    }
                }
            }

            if (arrTmp.length > 0)
                return arrTmp;
            else
                return null;
        },

        SetEnabledGrid: function (grid, enabled) {
            if (grid == undefined) {
                return;
            }

            if (!CheckFirstRowIsEmpty(grid)) {
                for (var i = 0; i < grid.getRowsNum(); i++) {
                    if (!grid.cells2(i, grid.getColIndexById(CMS480.GridColumnID.SearchResult.MonthYear)).getAttribute("parent-header")) {
                        var row_id = grid.getRowId(i);
                        var txtReceiveAmountID = GenerateGridControlID(CMS480.GridColumnID.SearchResult.ReceivedAmountID, row_id);
                        var txtIncomeRentalFeeID = GenerateGridControlID(CMS480.GridColumnID.SearchResult.IncomeRentalFeeID, row_id);
                        var txtAccAdvanceReceiveID = GenerateGridControlID(CMS480.GridColumnID.SearchResult.AccAdvanceReceiveID, row_id);
                        var txtAccUnpaidID = GenerateGridControlID(CMS480.GridColumnID.SearchResult.AccUnpaidID, row_id);
                        var txtIncomeVatID = GenerateGridControlID(CMS480.GridColumnID.SearchResult.IncomeVatID, row_id);
                        var txtUnpaidPeriodID = GenerateGridControlID(CMS480.GridColumnID.SearchResult.UnpaidPeriodID, row_id);
                        var txtIncomeDateID = GenerateGridControlID(CMS480.GridColumnID.SearchResult.IncomeDateID, row_id);

                        $("#" + txtReceiveAmountID).SetDisabled(!enabled);
                        $("#" + txtReceiveAmountID + "CurrencyType").SetDisabled(!enabled);
                        $("#" + txtIncomeRentalFeeID).SetDisabled(!enabled);
                        $("#" + txtIncomeRentalFeeID + "CurrencyType").SetDisabled(!enabled);
                        $("#" + txtAccAdvanceReceiveID).SetDisabled(!enabled);
                        $("#" + txtAccAdvanceReceiveID + "CurrencyType").SetDisabled(!enabled);
                        $("#" + txtAccUnpaidID).SetDisabled(!enabled);
                        $("#" + txtAccUnpaidID + "CurrencyType").SetDisabled(!enabled);
                        $("#" + txtIncomeVatID).SetDisabled(!enabled);
                        $("#" + txtIncomeVatID + "CurrencyType").SetDisabled(!enabled);
                        $("#" + txtUnpaidPeriodID).SetDisabled(!enabled);
                        $("#" + txtIncomeDateID).SetDisabled(!enabled);
                        $("#" + txtIncomeDateID).EnableDatePicker(enabled);
                    }
                }
            }
        },

        LoadDataToGrid: function (result, editable) {
            var grid = CMS480.Grids.grdSearch;
            var data = result.ResultData;

            DeleteAllRow(grid);

            if (data == null || data == undefined || data.length == 0) {
                return;
            }

            CheckFirstRowIsEmpty(grid, true);

            if (data.length > 1000) {

                //                var msg = "Search result more than 1,000, please input more criteria.";

                //                ajax_method.CallScreenController("/Common/CMS480_GetMessage", null, function (result, controls) {

                //                    if (result != null) {
                //                        msg = result.Message;
                //                    }
                //                });

                var params = new Array();
                params.push("1000");

                var obj = {
                    module: "Common",
                    code: "MSG0052",
                    param: params
                };

                call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
                    //OpenYesNoMessageDialog(result.Code, result.Message

                    var rowEmptyID = '_empty_' + grid._OwnerId;
                    grid.addRow(grid.uid(), ["<div style='float:left;' id=" + rowEmptyID + " mode='empty'>" + result.Message + "</div>"], grid.getRowsNum());
                    grid.setColspan(grid.getRowId(0), 0, grid.getColumnsNum());
                    grid.setSizes();
                });

                return;
            }

            for (var i = 0; i < data.length; i++) {
                var row_id = null;

                row_id = grid.uid();
                grid.addRow(row_id
                    , CMS480.Functions.grdSearch_NewData(
                        data[i].ReportMonthYear,
                        data[i].BillingCodeShow
                    )
                    , grid.getRowsNum()
                );


                //GenerateNumericBox2(grid, CMS480.GridColumnID.SearchResult.ReceivedAmountID,
                //    row_id, CMS480.GridColumnID.SearchResult.ReceivedAmount,
                //    data[i].ReceiveAmountShow, 9, 2, -999999999.99, 999999999.99, false, editable, "97%");

                //GenerateNumericBox2(grid, CMS480.GridColumnID.SearchResult.IncomeRentalFeeID,
                //    row_id, CMS480.GridColumnID.SearchResult.IncomeRentalFee,
                //    data[i].IncomeRentalFeeShow, 9, 2, -999999999.99, 999999999.99, false, editable, "97%");

                //GenerateNumericBox2(grid, CMS480.GridColumnID.SearchResult.AccAdvanceReceiveID,
                //    row_id, CMS480.GridColumnID.SearchResult.AccAdvanceReceive,
                //    data[i].AccumulatedReceiveAmountShow, 9, 2, -999999999.99, 999999999.99, false, editable, "97%");

                //GenerateNumericBox2(grid, CMS480.GridColumnID.SearchResult.AccUnpaidID,
                //    row_id, CMS480.GridColumnID.SearchResult.AccUnpaid,
                //    data[i].AccumulatedUnpaidShow, 9, 2, -999999999.99, 999999999.99, false, editable, "97%");

                //GenerateNumericBox2(grid, CMS480.GridColumnID.SearchResult.IncomeVatID,
                //    row_id, CMS480.GridColumnID.SearchResult.IncomeVat,
                //    data[i].IncomeVatShow, 9, 2, -999999999.99, 999999999.99, false, editable, "97%");

                GenerateNumericBox2(grid, CMS480.GridColumnID.SearchResult.UnpaidPeriodID,
                    row_id, CMS480.GridColumnID.SearchResult.UnpaidPeriod,
                    data[i].UnpaidPeriodShow, 12, 2, -999999999999.99, 999999999999.99, false, editable, "97%");

                GenerateNumericCurrencyControl(grid, CMS480.GridColumnID.SearchResult.ReceivedAmountID, row_id, "ReceiveAmount", data[i].ReceiveAmountCurrencyType, data[i].ReceiveAmountShow, editable);
                GenerateNumericCurrencyControl(grid, CMS480.GridColumnID.SearchResult.IncomeRentalFeeID, row_id, "IncomeRentalFee", data[i].IncomeRentalFeeCurrencyType, data[i].IncomeRentalFeeShow, editable);
                GenerateNumericCurrencyControl(grid, CMS480.GridColumnID.SearchResult.AccAdvanceReceiveID, row_id, "AccumulatedReceiveAmount", data[i].AccumulatedReceiveAmountCurrencyType, data[i].AccumulatedReceiveAmountShow, editable);
                GenerateNumericCurrencyControl(grid, CMS480.GridColumnID.SearchResult.AccUnpaidID, row_id, "AccumulatedUnpaid", data[i].AccumulatedUnpaidCurrencyType, data[i].AccumulatedUnpaidShow, editable);
                GenerateNumericCurrencyControl(grid, CMS480.GridColumnID.SearchResult.IncomeVatID, row_id, "IncomeVat", data[i].IncomeVatCurrencyType, data[i].IncomeVatShow, editable);

                GenerateGridDateTimePicker(grid, row_id, CMS480.GridColumnID.SearchResult.IncomeDateID,
                    CMS480.GridColumnID.SearchResult.IncomeDate,
                    data[i].IncomeDateStr,
                    editable)

                var TaxRateColId = grid.getColIndexById(CMS480.GridColumnID.SearchResult.TaxRate);
                grid.cells(row_id, TaxRateColId).setValue(data[i].TaxRate);

                var MonthlyBillingAmountColId = grid.getColIndexById(CMS480.GridColumnID.SearchResult.MonthlyBillingAmount);
                grid.cells(row_id, MonthlyBillingAmountColId).setValue(data[i].MonthlyBillingAmount);

                var ReceivedAmountCtrlId = "#" + GenerateGridControlID(CMS480.GridColumnID.SearchResult.ReceivedAmountID, row_id);
                $(ReceivedAmountCtrlId).change(function () {
                    if (!$(ReceivedAmountCtrlId).prop("readonly")) {
                        var ctrl_row_id = GetGridRowIDFromControl(this);
                        CMS480.Functions.CalculateIncomeVAT(ctrl_row_id);
                    }
                });

                var AccUnpaidCtrlID = "#" + GenerateGridControlID(CMS480.GridColumnID.SearchResult.AccUnpaidID, row_id);
                $(AccUnpaidCtrlID).change(function () {
                    if (!$(AccUnpaidCtrlID).prop("readonly")) {
                        var ctrl_row_id = GetGridRowIDFromControl(this);
                        CMS480.Functions.CalculateUnpaidPeriod(ctrl_row_id);
                    }
                });

                //                var AccAdvanceReceiveCtrlId = "#" + GenerateGridControlID(CMS480.GridColumnID.SearchResult.AccAdvanceReceiveID, row_id);
                //                $(AccAdvanceReceiveCtrlId).blur(function () {
                //                    CalculateAmount(row_id, InstGrid.cells(row_id, InstGrid.getColIndexById("InstrumentCode")).getValue(), $(UnitPriceID).NumericValue(), null);
                //                });

            }

            grid.setSizes();
        },

        grdSearch_NewData: function (ReportMonthYear, BillingCode) {
            return [
                String(ReportMonthYear),
                String(BillingCode)
            ];
        },

        CalculateIncomeVAT: function (row_id) {
            var ReceivedAmountCtrlId = "#" + GenerateGridControlID(CMS480.GridColumnID.SearchResult.ReceivedAmountID, row_id);
            var ReceivedAmountValue = $(ReceivedAmountCtrlId).NumericValue();

            var TaxRateColId = CMS480.Grids.grdSearch.getColIndexById(CMS480.GridColumnID.SearchResult.TaxRate)
            var TaxRateValue = CMS480.Grids.grdSearch.cells(row_id, TaxRateColId).getValue();

            var IncomeVatIDCtrlId = "#" + GenerateGridControlID(CMS480.GridColumnID.SearchResult.IncomeVatID, row_id);
            $(IncomeVatIDCtrlId).val(SetNumericValue(ReceivedAmountValue * TaxRateValue, 2));
        },

        CalculateUnpaidPeriod: function (row_id) {
            var AccUnpaidCtrlID = "#" + GenerateGridControlID(CMS480.GridColumnID.SearchResult.AccUnpaidID, row_id);
            var AccUnPaidValue = $(AccUnpaidCtrlID).NumericValue();

            var MonthlyBillingAmountColId = CMS480.Grids.grdSearch.getColIndexById(CMS480.GridColumnID.SearchResult.MonthlyBillingAmount)
            var MonthlyBillingAmountValue = CMS480.Grids.grdSearch.cells(row_id, MonthlyBillingAmountColId).getValue();

            if (MonthlyBillingAmountValue > 0) {
                var UnpaidPeriodCtrlId = "#" + GenerateGridControlID(CMS480.GridColumnID.SearchResult.UnpaidPeriodID, row_id);
                $(UnpaidPeriodCtrlId).val(SetNumericValue(AccUnPaidValue / MonthlyBillingAmountValue, 2));
            }
        }
    }
}

function GenerateNumericCurrencyControl(grid, id, row_id, col_id, currency, textboxValue, enable) {
    var col = grid.getColIndexById(col_id);
    var fid = GenerateGridControlID(id, row_id);
    ToTalInstrumentAmountID = id + "_" + row_id;

    var obj = {
        id: fid,
        currency: currency,
        textboxValue: textboxValue,
        enable: enable
    };
    ajax_method.CallScreenController("/common/CMS480_GenerateCurrencyNumericTextBox", obj, function (result, controls, isWarning) {
        var txt = result;
        grid.cells(row_id, col).setValue(txt);
        $(fid).BindNumericBox(12, 2, 0, 999999999999.99);
        $(fid).NumericCurrency().attr('disabled', true);

        $(fid).focus(function () {
            grid.selectRowById(row_id);
        });
    });
}

$(document).ready(function () {
    CMS480.Functions.InitializeGrid();
    CMS480.Functions.InitializeScreen();
    CMS480.Functions.SetScreenMode(CMS480.ScreenMode.Search);
});