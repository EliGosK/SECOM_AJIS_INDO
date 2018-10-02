/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/control_events.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>
/// <reference path="../../Scripts/Base/DateTimePicker.js"/>

/// <reference path="../Base/object/ajax_method.js" />
/// <reference path="../Base/object/command_event.js" />
/// <reference path="../Base/object/master_event.js" />

var ICS110 = {

    WHTNo: null,
    LastSearchPaymentParams: null,

    ScreenMode: {
        Current: "",
        Register: 0,
        Confirm: 1,
        ShowWHTNo: 2
    },

    GridColumnID: {
        MatchWHTDetail: {
            PaymentTransNo: "PaymentTransNo",
            PaymentDate: "PaymentDate",
            Payer: "Payer",
            VATRegistantName: "VATRegistantName",
            InvoiceNo: "InvoiceNo",
            ContractCode: "ContractCode",
            WHTAmountShow: "WHTAmountShow", // add by jirawat jannet on 2016-10-28
            WHTAmount: "WHTAmount",
            RemoveButton: "RemoveButton",
            WHTAmountCurrencyType: 'WHTAmountCurrencyType' // add by jirawat jannet on 2016-10-28
        },
        SearchPayment: {
            No: "No",
            PaymentTransNo: "PaymentTransNo",
            PaymentDate: "PaymentDate",
            Payer: "Payer",
            VATRegistantName: "VATRegistantName",
            InvoiceNo: "InvoiceNo",
            ContractCode: "ContractCode",
            WHTAmountShow: "WHTAmountShow", // add by jirawat jannet on 2016-10-28
            WHTAmount: "WHTAmount",
            SelectButton: "SelectButton",
            SelectedFlag: "SelectedFlag",
            ToJson: "ToJson"
        }
    },

    CtrlID: {
        divMatchWHT: "#divMatchWHT",
        formMatchWHT: "#formMatchWHT",
        txtWHTNo: "#txtWHTNo",
        txtMatchingStatus: "#txtMatchingStatus",
        txtWHTAmount: "#txtWHTAmount",
        txtDocumentDate: "#txtDocumentDate",
        txtWHTMatchingDate: "#txtWHTMatchingDate",
        txtMatchedAmount: "#txtMatchedAmount",
        btnRetrievePayment: "#btnRetrievePayment",
        divMatchWHTDetailGrid: "#divMatchWHTDetailGrid",

        divShowWHTNo: "#divShowWHTNo",
        txtShowWHTNo: "#txtShowWHTNo",
        btnNewRegister: "#btnNewRegister",

        divSearchPaymentDialog: "#divSearchPaymentDialog",
        formSearchWHT: "#formSearchWHT",
        txtPaymentDateFrom: "#txtPaymentDateFrom",
        txtPaymentDateTo: "#txtPaymentDateTo",
        txtPaymentTransNo: "#txtPaymentTransNo",
        txtPayerName: "#txtPayerName",
        txtVATRegistantName: "#txtVATRegistantName",
        txtInvoiceNo: "#txtInvoiceNo",
        txtContractCode: "#txtContractCode",
        txtIDNo: "#txtIDNo",
        btnSearchPayment: "#btnSearchPayment",
        btnClearPayment: "#btnClearPayment",
        btnAddPayment: "#btnAddPayment",
        divSearchPaymentResult: "#divSearchPaymentResult",
        divSearchPaymentResultGrid: "#divSearchPaymentResultGrid",
        chkSelectAll: "#chkSelectAll"
    },

    Grids: {
        grdMatchWHTDetail: null,
        grdSearchPaymentResult: null
    },

    EventHandlers: {
        grdMatchWHTDetail_OnLoadedData: function () {
            var grid = ICS110.Grids.grdMatchWHTDetail;

            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);

                grid.GenerateRemoveButton(row_id);
            }
            grid.setSizes();
        },

        grdSearchPaymentResult_OnLoadedData: function (gen_ctrl) {
            var grid = ICS110.Grids.grdSearchPaymentResult;

            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);
                var paymenttransno = grid.cells(row_id, grid.getColIndexById(ICS110.GridColumnID.SearchPayment.PaymentTransNo)).getValue();
                var enabled = !ICS110.IsSelectedPayment(paymenttransno);
                var isSelected = grid.cells(row_id, grid.getColIndexById(ICS110.GridColumnID.SearchPayment.SelectedFlag)).getValue();
                var checkbox = GenerateCheckBox2(ICS110.GridColumnID.SearchPayment.SelectButton, row_id, "", enabled, isSelected || !enabled);
                grid.cells(row_id, grid.getColIndexById(ICS110.GridColumnID.SearchPayment.SelectButton)).setValue(checkbox);

                BindGridCheckBoxClickEvent(ICS110.GridColumnID.SearchPayment.SelectButton, row_id, ICS110.EventHandlers.chkSelect_OnClick);
            }

            $(ICS110.CtrlID.chkSelectAll).click(ICS110.EventHandlers.chkSelectAll_OnClick);
            grid.setSizes();
        },

        afterGridInitilized: function (grid) {
            grid.Initialized = true;

            if (ICS110.Grids.grdMatchWHTDetail
                && ICS110.Grids.grdMatchWHTDetail.Initialized
                && ICS110.Grids.grdSearchPaymentResult
                && ICS110.Grids.grdSearchPaymentResult.Initialized) {
                ICS110.SetScreenMode(ICS110.ScreenMode.Register);

                setTimeout(function () {
                    call_ajax_method_json("/income/ICS110_GetLoadingWHTNo", null, function (result, controls) {
                        if (result) {
                            ICS110.LoadWHTData(result);
                        }
                    });
                }, 500);
            }
        },

        chkSelect_OnClick: function (row_id, checked) {
            var grid = ICS110.Grids.grdSearchPaymentResult;
            grid.cells(row_id, grid.getColIndexById(ICS110.GridColumnID.SearchPayment.SelectedFlag)).setValue(checked ? "1" : "");

            var checkedAll = $(ICS110.CtrlID.chkSelectAll).prop("checked");
            if (checkedAll && !checked) {
                $(ICS110.CtrlID.chkSelectAll).attr("checked", false);
            }
        },

        chkSelectAll_OnClick: function () {
            var checked = $(ICS110.CtrlID.chkSelectAll).prop("checked");
            var grid = ICS110.Grids.grdSearchPaymentResult;
            if (!CheckFirstRowIsEmpty(grid)) {
                for (var i = 0; i < grid.getRowsNum(); i++) {
                    var row_id = grid.getRowId(i);
                    var ctrl = "#" + GenerateGridControlID(ICS110.GridColumnID.SearchPayment.SelectButton, row_id);
                    if (!$(ctrl).attr("disabled")) {
                        $(ctrl).attr("checked", checked);
                        grid.cells(row_id, grid.getColIndexById(ICS110.GridColumnID.SearchPayment.SelectedFlag)).setValue(checked ? "1" : "");
                    }
                }
            }
        },

        btnRetrievePayment_OnClick: function () {
            var event = {
                OK: function () {
                    ICS110.AddSelectedPayment();
                    $(ICS110.CtrlID.divSearchPaymentDialog).CloseDialog();
                }
            };

            $(ICS110.CtrlID.divSearchPaymentDialog).OpenPopupDialog2(900, 600, event, null,
                function (event, ui, callback) {
                    ICS110.RefreshSearchPayment(callback);
                },
                function () {
                    $(ICS110.CtrlID.txtPaymentDateFrom).datepicker("hide");
                    $(ICS110.CtrlID.txtPaymentDateTo).datepicker("hide");
                }
            );
        },

        btnSearchPayment_OnClick: function () {
            var params = $(ICS110.CtrlID.formSearchWHT).serializeObject2();

            params.WHTNo = ICS110.WHTNo;

            $(ICS110.CtrlID.btnSearchPayment).attr("disabled", true);
            master_event.LockWindow(true);

            ICS110.LastSearchPaymentParams = params;

            $(ICS110.CtrlID.divSearchPaymentResultGrid).LoadDataToGrid(
                ICS110.Grids.grdSearchPaymentResult,
                ROWS_PER_PAGE_FOR_SEARCHPAGE,
                true,
                "/Income/ICS110_SearchPayment",
                params,
                "doPaymentForWHT",
                false,
                function (result, controls, isWarning) { //post-load
                    $(ICS110.CtrlID.btnSearchPayment).removeAttr("disabled");
                    master_event.LockWindow(false);

                    if (result != undefined) {
                        $(ICS110.CtrlID.divSearchPaymentResult).show();
                    }
                },
                function (result, controls, isWarning) { //pre-load
                    if (isWarning == undefined) {
                        $(ICS110.CtrlID.divSearchPaymentResult).show();
                    }
                }
            );
        },

        txtWHTAmount_OnChange: function () {
            ICS110.RefreshMatchingStatus();
        },

        btnClearPayment_OnClick: function () {
            ICS110.ClearSearchPaymentDialog();
            ICS110.LastSearchPaymentParams = null;
            return false;
        },

        btnNewRegister_OnClick: function () {
            ICS110.ClearMatchWHT();
            ICS110.SetScreenMode(ICS110.ScreenMode.Register);
        },

        CommandRegister: function () {
            command_control.CommandControlMode(false);

            var obj = {
                WHTNo: $(ICS110.CtrlID.txtWHTNo).val(),
                Amount: $(ICS110.CtrlID.txtWHTAmount).NumericValue(),
                AmountCurrencyType: $(ICS110.CtrlID.txtWHTAmount).NumericCurrencyValue(),
                DocumentDate: $(ICS110.CtrlID.txtDocumentDate).val(),
                WHTMatchingDate: $(ICS110.CtrlID.txtWHTMatchingDate).val(),
                PaymentTransNoList: ICS110.GetTransNoList(),
                TotalMatchedAmount: $(ICS110.CtrlID.txtMatchedAmount).NumericValue(),
                TotalMatchedAmountCurrencyType: $(ICS110.CtrlID.txtMatchedAmount).NumericCurrencyValue()
            };

            call_ajax_method_json("/income/ICS110_RegisterWHT", obj, function (result, controls) {
                if (controls != undefined) {
                    VaridateCtrl(controls, controls);
                } else if (result) {
                    ICS110.SetScreenMode(ICS110.ScreenMode.Confirm);
                }

                command_control.CommandControlMode(true);
            });
        },

        CommandReset: function () {
            if (ICS110.WHTNo) {
                ICS110.LoadWHTData(ICS110.WHTNo);
            }
            else {
                ICS110.ClearMatchWHT();
            }
        },

        CommandConfirm: function () {
            command_control.CommandControlMode(false);

            call_ajax_method_json("/income/ICS110_ConfirmWHT", null, function (result, controls) {
                if (result) {
                    $(ICS110.CtrlID.txtShowWHTNo).val(result.WHTNo);
                    ICS110.SetScreenMode(ICS110.ScreenMode.ShowWHTNo);
                    master_event.ScrollWindow(ICS110.CtrlID.divShowWHTNo);
                }

                command_control.CommandControlMode(true);
            });

        },

        CommandBack: function () {
            ICS110.SetScreenMode(ICS110.ScreenMode.Register);
        }

    },

    LoadWHTData: function (whtno) {
        ICS110.WHTNo = whtno;

        var objParam = {
            WHTNo: whtno
        };

        call_ajax_method_json("/income/ICS110_GetWHTData", objParam, function (result, controls) {
            if (!result) {
                var param = { "module": "Common", "code": "MSG0001" };
                call_ajax_method("/Shared/GetMessage", param, function (data) {
                    /* ====== Open info dialog =====*/
                    OpenInformationMessageDialog(param.code, data.Message, function () {
                        ICS110.EventHandlers.btnNewRegister_OnClick();
                    });
                });
                return;
            }

            $(ICS110.CtrlID.txtWHTNo).val(result.WHTNo);
            $(ICS110.CtrlID.txtWHTAmount).val(SetNumericText(result.Amount, 2));
            $(ICS110.CtrlID.txtDocumentDate).val(ConvertDateToTextFormat(ConvertDateObject(result.DocumentDate)));
            $(ICS110.CtrlID.txtWHTMatchingDate).val(ConvertDateToTextFormat(ConvertDateObject(result.WHTMatchingDate)));

            $(ICS110.CtrlID.txtWHTMatchingDate).EnableDatePicker(false);

            var objDetailParam = {
                WHTNo: whtno
            };

            $(ICS110.CtrlID.divMatchWHTDetailGrid).LoadDataToGrid(
                ICS110.Grids.grdMatchWHTDetail,
                0,
                false,
                "/Income/ICS110_GetWHTDetail",
                objDetailParam,
                "doMatchWHTDetail",
                false,
                function () {
                    ICS110.RefreshMatchingStatus();
                },
                null
            );
        });
    },

    IsSelectedPayment: function (paymentTransNo) {
        var grid = ICS110.Grids.grdMatchWHTDetail;

        for (var i = 0; i < grid.getRowsNum(); i++) {
            var row_id = grid.getRowId(i);
            var transno = grid.cells(row_id, grid.getColIndexById(ICS110.GridColumnID.MatchWHTDetail.PaymentTransNo)).getValue();
            if (transno == paymentTransNo) {
                return true;
            }
        }
        return false;
    },

    ClearSearchPaymentDialog: function () {
        $(ICS110.CtrlID.divSearchPaymentDialog).clearForm();
        ClearDateFromToControl(ICS110.CtrlID.txtPaymentDateFrom, ICS110.CtrlID.txtPaymentDateTo);

        if (ICS110.Grids.grdSearchPaymentResult != null
            && !CheckFirstRowIsEmpty(ICS110.Grids.grdSearchPaymentResult)
            && ICS110.Grids.grdSearchPaymentResult.getRowsNum() > 0) {
            DeleteAllRow(ICS110.Grids.grdSearchPaymentResult);
            ICS110.Grids.grdSearchPaymentResult.setSizes();
        }

        $(ICS110.CtrlID.divSearchPaymentResult).hide();
    },

    ClearMatchWHT: function () {
        $(ICS110.CtrlID.divMatchWHT).clearForm();
        if (ICS110.Grids.grdMatchWHTDetail != null
            && !CheckFirstRowIsEmpty(ICS110.Grids.grdMatchWHTDetail)
            && ICS110.Grids.grdMatchWHTDetail.getRowsNum() > 0) {
            DeleteAllRow(ICS110.Grids.grdMatchWHTDetail);
            ICS110.Grids.grdMatchWHTDetail.setSizes();
        }

        $(ICS110.CtrlID.txtWHTMatchingDate).EnableDatePicker(true);
        ICS110.WHTNo = null;
    },

    InitializeGrid: function () {
        ICS110.Grids.grdMatchWHTDetail = $(ICS110.CtrlID.divMatchWHTDetailGrid).InitialGrid(
            0, false, "/income/ICS110_InitialMatchWHTDetail"
            , function () {
                BindOnLoadedEvent(ICS110.Grids.grdMatchWHTDetail, ICS110.EventHandlers.grdMatchWHTDetail_OnLoadedData);
                SpecialGridControl(ICS110.Grids.grdMatchWHTDetail, [ICS110.GridColumnID.MatchWHTDetail.RemoveButton]);
                ICS110.EventHandlers.afterGridInitilized(ICS110.Grids.grdMatchWHTDetail);
            }
        );

        ICS110.Grids.grdMatchWHTDetail.GenerateRemoveButton = function (row_id) {
            var grid = ICS110.Grids.grdMatchWHTDetail;

            GenerateRemoveButton(grid, ICS110.GridColumnID.MatchWHTDetail.RemoveButton, row_id, ICS110.GridColumnID.MatchWHTDetail.RemoveButton, true);
            BindGridButtonClickEvent(ICS110.GridColumnID.MatchWHTDetail.RemoveButton, row_id, function (rid) {
                DeleteRow(grid, rid);
                ICS110.RefreshMatchingStatus();
            });
        };

        ICS110.Grids.grdSearchPaymentResult = $(ICS110.CtrlID.divSearchPaymentResultGrid).InitialGrid(
            ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/income/ICS110_InitialSearchPaymentResultGrid"
            , function () {
                BindOnLoadedEvent(ICS110.Grids.grdSearchPaymentResult, ICS110.EventHandlers.grdSearchPaymentResult_OnLoadedData);
                $(ICS110.CtrlID.chkSelectAll).click(ICS110.EventHandlers.chkSelectAll_OnClick);
                SpecialGridControl(ICS110.Grids.grdSearchPaymentResult, [ICS110.GridColumnID.SearchPayment.SelectButton]);
                ICS110.EventHandlers.afterGridInitilized(ICS110.Grids.grdSearchPaymentResult);
            }
        );
    },

    InitializeScreen: function () {
        $(ICS110.CtrlID.txtWHTAmount).BindNumericBox(12, 2, 0, 999999999999.99, 0);
        $(ICS110.CtrlID.txtWHTAmount).change(ICS110.EventHandlers.txtWHTAmount_OnChange);

        $(ICS110.CtrlID.txtDocumentDate).InitialDate();
        $(ICS110.CtrlID.txtWHTMatchingDate).InitialDate();

        $(ICS110.CtrlID.txtMatchedAmount).BindNumericBox(12, 2, 0, 999999999999.99, 0);

        ClearDateFromToControl(ICS110.CtrlID.txtPaymentDateFrom, ICS110.CtrlID.txtPaymentDateTo);
        InitialDateFromToControl(ICS110.CtrlID.txtPaymentDateFrom, ICS110.CtrlID.txtPaymentDateTo);

        $(ICS110.CtrlID.btnRetrievePayment).click(ICS110.EventHandlers.btnRetrievePayment_OnClick);

        $(ICS110.CtrlID.btnSearchPayment).click(ICS110.EventHandlers.btnSearchPayment_OnClick);
        $(ICS110.CtrlID.btnClearPayment).click(ICS110.EventHandlers.btnClearPayment_OnClick);

        $(ICS110.CtrlID.btnNewRegister).click(ICS110.EventHandlers.btnNewRegister_OnClick);

        $(ICS110.CtrlID.divSearchPaymentResult).hide();
    },

    SetScreenMode: function (mode) {
        if (mode == ICS110.ScreenMode.Register) {
            register_command.SetCommand(ICS110.EventHandlers.CommandRegister);
            reset_command.SetCommand(ICS110.EventHandlers.CommandReset);
            confirm_command.SetCommand(null);
            back_command.SetCommand(null);
            ICS110.Grids.grdMatchWHTDetail.setColumnHidden(ICS110.Grids.grdMatchWHTDetail.getColIndexById(ICS110.GridColumnID.MatchWHTDetail.RemoveButton), false);
            $(ICS110.CtrlID.divMatchWHT).SetViewMode(false);
            $(ICS110.CtrlID.divShowWHTNo).hide();

            ICS110.ScreenMode.Current = mode;
        }
        else if (mode == ICS110.ScreenMode.Confirm) {
            register_command.SetCommand(null);
            reset_command.SetCommand(null);
            confirm_command.SetCommand(ICS110.EventHandlers.CommandConfirm);
            back_command.SetCommand(ICS110.EventHandlers.CommandBack);
            ICS110.Grids.grdMatchWHTDetail.setColumnHidden(ICS110.Grids.grdMatchWHTDetail.getColIndexById(ICS110.GridColumnID.MatchWHTDetail.RemoveButton), true);
            $(ICS110.CtrlID.divMatchWHT).SetViewMode(true);
            $(ICS110.CtrlID.divShowWHTNo).hide();

            ICS110.ScreenMode.Current = mode;
        }
        else if (mode == ICS110.ScreenMode.ShowWHTNo) {
            register_command.SetCommand(null);
            reset_command.SetCommand(null);
            confirm_command.SetCommand(null);
            back_command.SetCommand(null);
            ICS110.Grids.grdMatchWHTDetail.setColumnHidden(ICS110.Grids.grdMatchWHTDetail.getColIndexById(ICS110.GridColumnID.MatchWHTDetail.RemoveButton), true);
            $(ICS110.CtrlID.divMatchWHT).SetViewMode(true);
            $(ICS110.CtrlID.divShowWHTNo).show();

            ICS110.ScreenMode.Current = mode;
        }
    },
    // Add by Jirawat Janne ton 2016-11-15
    // Validate type of currency with selected data before add new data
    ValidateAddSelectedRow: function (selectedCurrencyType) {
        var gridMatch = ICS110.Grids.grdMatchWHTDetail;
        var clearEmptyRow = CheckFirstRowIsEmpty(gridMatch);

        if (!clearEmptyRow) {
            for (var i = 0; i < gridMatch.getRowsNum() ; i++) {
                var row_id = gridMatch.getRowId(i);
                var col_id = gridMatch.getColIndexById(ICS110.GridColumnID.MatchWHTDetail.WHTAmountCurrencyType);
                var currencyType = gridMatch.cells(row_id, col_id).getValue();

                if (selectedCurrencyType != currencyType) {
                    OpenWarningDialog(ErrorMessage.InvalidCurrencyType);
                    return false;
                }
                else
                    return true;
            }
        } else {
            return true;
        }
    },
    AddSelectedPayment: function () {
        var gridSelecting = ICS110.Grids.grdSearchPaymentResult;
        var gridMatch = ICS110.Grids.grdMatchWHTDetail;
        var clearEmptyRow = CheckFirstRowIsEmpty(gridMatch);
        var paymentDate;

        for (var i = 0; i < gridSelecting.getRowsNum(); i++) {
            var row_id = gridSelecting.getRowId(i);
            var checked = gridSelecting.cells(row_id, gridSelecting.getColIndexById(ICS110.GridColumnID.SearchPayment.SelectedFlag)).getValue() == "1";
            var paymenttransno = gridSelecting.cells(row_id, gridSelecting.getColIndexById(ICS110.GridColumnID.SearchPayment.PaymentTransNo)).getValue();
            var isSelected = ICS110.IsSelectedPayment(paymenttransno);

            if (checked && !isSelected) {
                if (clearEmptyRow) {
                    CheckFirstRowIsEmpty(gridMatch, true);
                    clearEmptyRow = false;
                }

                var chkboxid = GenerateGridControlID(ICS110.GridColumnID.SearchPayment.SelectButton, row_id);
                $("#" + chkboxid).prop("disabled", true);
                $("#" + chkboxid).prop("checked", true);

                var strPaymentData = gridSelecting.cells(row_id, gridSelecting.getColIndexById(ICS110.GridColumnID.SearchPayment.ToJson)).getValue();
                var objPaymentData = JSON.parse(htmlDecode(strPaymentData));

                // Add by Jirawat Jannet on 2016-11-15
                if (!ICS110.ValidateAddSelectedRow(objPaymentData.WHTAmountCurrencyType)) {
                    return;
                }

                AddNewRow(gridMatch, [
                    objPaymentData.PaymentTransNo,
                    objPaymentData.PaymentDate,
                    objPaymentData.Payer,
                    objPaymentData.VATRegistantName,
                    objPaymentData.InvoiceNo,
                    objPaymentData.ContractCode,
                    objPaymentData.WHTAmountShow,
                    '',
                    '',
                    objPaymentData.WHTAmount,
                    objPaymentData.WHTAmountCurrencyType
                ]);

                var newMatchRowNum = gridMatch.getRowsNum() - 1;
                newMatchRowId = gridMatch.getRowId(newMatchRowNum);
                gridMatch.GenerateRemoveButton(newMatchRowId);

                var tmpPaymentDate = $.datepicker.parseDate('mm/dd/yy', objPaymentData.PaymentDate);
                if (!paymentDate || paymentDate > tmpPaymentDate) {
                    paymentDate = tmpPaymentDate;
                }
            }
        }

        gridMatch.setSizes();
        $(ICS110.CtrlID.txtWHTMatchingDate).SetDate(paymentDate);
        ICS110.RefreshMatchingStatus();
    },

    GetTransNoList: function () {
        var result = [];
        var grid = ICS110.Grids.grdMatchWHTDetail;

        if (!CheckFirstRowIsEmpty(grid)) {
            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);
                var transno = grid.cells(row_id, grid.getColIndexById(ICS110.GridColumnID.MatchWHTDetail.PaymentTransNo)).getValue();
                result.push(transno);
            }
        }

        return result;
    },

    RefreshMatchingStatus: function () {
        var grid = ICS110.Grids.grdMatchWHTDetail;
        var WHTAmount = Math.round10(+($(ICS110.CtrlID.txtWHTAmount).NumericValue()), -2);
        var matchedamount = 0;
        var matchedAmountCurrencyType = '';

        if (!CheckFirstRowIsEmpty(grid)) {
            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);
                var amount = Math.round10(+(grid.cells(row_id, grid.getColIndexById(ICS110.GridColumnID.MatchWHTDetail.WHTAmount)).getValue()), -2);
                matchedAmountCurrencyType = grid.cells(row_id, grid.getColIndexById(ICS110.GridColumnID.MatchWHTDetail.WHTAmountCurrencyType)).getValue();
                matchedamount = Math.round10(matchedamount + amount, -2);
            }
        }

        if (matchedAmountCurrencyType == '') matchedAmountCurrencyType = C_CURRENCY_LOCAL;

        $(ICS110.CtrlID.txtMatchedAmount).val(SetNumericText(matchedamount, 2));
        $(ICS110.CtrlID.txtMatchedAmount).NumericCurrency().val(matchedAmountCurrencyType);

        if (WHTAmount == matchedamount) {
            $(ICS110.CtrlID.txtMatchingStatus).val(MATCHSTATUS_COMPLETED);
        }
        else {
            $(ICS110.CtrlID.txtMatchingStatus).val(MATCHSTATUS_PARTIAL);
        }
    },

    RefreshSearchPayment: function (callback) {
        if (ICS110.LastSearchPaymentParams && (
            ICS110.LastSearchPaymentParams.PaymentDateFrom
            || ICS110.LastSearchPaymentParams.PaymentDateTo
            || ICS110.LastSearchPaymentParams.PaymentTransNo
            || ICS110.LastSearchPaymentParams.PayerName
            || ICS110.LastSearchPaymentParams.VATRegistantName
            || ICS110.LastSearchPaymentParams.InvoiceNo
            || ICS110.LastSearchPaymentParams.ContractCode
            || ICS110.LastSearchPaymentParams.WHTNo
        )) {
            $(ICS110.CtrlID.divSearchPaymentResultGrid).LoadDataToGrid(
                ICS110.Grids.grdSearchPaymentResult,
                ROWS_PER_PAGE_FOR_SEARCHPAGE,
                true,
                "/Income/ICS110_SearchPayment",
                ICS110.LastSearchPaymentParams,
                "doPaymentForWHT",
                false,
                function (result, controls, isWarning) { //post-load
                    if (result != undefined) {
                        $(ICS110.CtrlID.divSearchPaymentResult).show();
                    }
                    if (callback) {
                        callback();
                    }
                },
                function (result, controls, isWarning) { //pre-load
                    if (isWarning == undefined) {
                        $(ICS110.CtrlID.divSearchPaymentResult).show();
                    }
                }
            );
        }
        else {
            if (callback) {
                callback();
            }
        }
    }

};

$(document).ready(function () {
    ICS110.InitializeGrid();
    ICS110.InitializeScreen();
});
