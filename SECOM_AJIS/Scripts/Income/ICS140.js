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
/// <reference path="../../Scripts/Base/download_method.js" />

/// <reference path="../Base/object/ajax_method.js" />
/// <reference path="../Base/object/command_event.js" />
/// <reference path="../Base/object/master_event.js" />

var ICS140 = {
    ROWS_PER_PAGE_FOR_DETAIL: 5,
    InputMode: false,

    GridColumnID: {
    },

    CtrlID: {
        divDTList: "#divDTList",
        cboBillingOffice: "#cboBillingOffice",
        txtSearchContractCode: "#txtSearchContractCode",
        txtSearchBillingClient: "#txtSearchBillingClient",
        txtSearchInvoiceNo: "#txtSearchInvoiceNo",
        chkFirstFeeFlag: "#chkFirstFeeFlag",
        chkShowLawsuit: "#chkShowLawsuit",
        chkShowBranch: "#chkShowBranch",
        chkShowNotDue: "#chkShowNotDue",
        chkShowPending: "#chkShowPending",
        chkShowOutStanding: "#chkShowOutStanding",
        btnSearch: "#btnSearch",
        btnClear: "#btnClear",
        divGridCustList: "#divGridCustList",

        divCustDebtInfoPanel: "#divCustDebtInfoPanel",
        divContacts: "#divContacts",
        txtBillingOffice: "#txtBillingOffice",
        txtBillingTargetCode: "#txtBillingTargetCode",
        txtBillingClientName: "#txtBillingClientName",
        txtContactPersonName: "#txtContactPersonName",
        txtPhoneNo: "#txtPhoneNo",

        divGridReturnedCheque: "#divGridReturnedCheque",

        divInvoiceList: "#divInvoiceList",
        divGridInvoiceList: "#divGridInvoiceList",
        chkSelectAll: "#chkSelectAll",

        divInvoiceDetail: "#divInvoiceDetail",
        divGridInvoiceDetail: "#divGridInvoiceDetail",

        divDebtTracingInput: "#divDebtTracingInput",
        txtCurrentStatus: "#txtCurrentStatus",
        cboInputResult: "#cboInputResult",
        txtNextCallDate: "#txtNextCallDate",
        txtInputEstimateDate: "#txtInputEstimateDate",
        txtInputAmount: "#txtInputAmount",
        txtInputAmountUsd: '#txtInputAmountUsd',
        cboInputPaymentMethod: "#cboInputPaymentMethod",
        cboInputPostponeReason: "#cboInputPostponeReason",
        txtRemark: "#txtRemark",
        btnSave: "#btnSave",
        btnCancel: "#btnCancel",

        divHistory: "#divHistory",
        divGridHistory: "#divGridHistory"
    },

    Grids: {
        grdCustList: null,
        grdCustListColumns: {
            BillingOfficeDisplay: "BillingOfficeDisplay",
            SelectButton: "SelectButton",
            BillingTargetCode: "BillingTargetCode",
            BillingTargetCodeShort: "BillingTargetCodeShort",
            BillingClientName: "BillingClientName",
            ContactPersonName: "ContactPersonName",
            PhoneNo: "PhoneNo",
            DebtTracingSubStatus: "DebtTracingSubStatus",
            IsHQBranch: "IsHQBranch",
            DebtTracingStatusDesc: "DebtTracingStatusDesc",
            BillingOfficeCode: "BillingOfficeCode",
            ServiceTypeCode: "ServiceTypeCode",
            DebtTracingSubStatus: "DebtTracingSubStatus",
            PaidButton: "PaidButton"
        },
        grdReturnedCheque: null,
        grdInvoiceList: null,
        grdInvoiceListColumns: {
            SelectButton: "SelectButton",
            FirstFeeFlagIcon: "FirstFeeFlagIcon",
            DetailButton: "DetailButton",
            BillingTargetCode: "BillingTargetCode",
            InvoiceNo: "InvoiceNo",
            InvoiceOCC: "InvoiceOCC",
            SelectedFlag: "SelectedFlag",
            FirstFeeFlag: "FirstFeeFlag"
        },
        grdInvoiceDetail: null,
        grdInvoiceDetailColumns: {
            FirstFeeFlagIcon: "FirstFeeFlagIcon",
            ContractCodeDisplay: "ContractCodeDisplay",
            ContractCodeShort: "ContractCodeShort",
            BillingOCC: "BillingOCC",
            FirstFeeFlag: "FirstFeeFlag"
        },
        grdHistory: null
    },

    EventHandlers: {
        grdCustList_OnLoadedData: function (gen_ctrl) {
            var grid = ICS140.Grids.grdCustList;

            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);

                if (gen_ctrl) {
                    var paidColIdx = grid.getColIndexById(ICS140.Grids.grdCustListColumns.PaidButton);
                    grid.cells(row_id, paidColIdx).setValue(GenerateHtmlButton(ICS140.Grids.grdCustListColumns.PaidButton, row_id, PaidButtonLabel, true));

                    GenerateSelectButton(grid, ICS140.Grids.grdCustListColumns.SelectButton, row_id, ICS140.Grids.grdCustListColumns.SelectButton, true);
                }

                /* ===== Bind event onClick to button ===== */
                BindGridHtmlButtonClickEvent(ICS140.Grids.grdCustListColumns.PaidButton, row_id, ICS140.EventHandlers.btnPaidButton_OnClick);

                BindGridButtonClickEvent(ICS140.Grids.grdCustListColumns.SelectButton, row_id, ICS140.EventHandlers.btnSelectButton_OnClick);
            }

            grid.setSizes();
        },

        grdReturnedCheque_OnLoadedData: function (gen_ctrl) {
            var grid = ICS140.Grids.grdReturnedCheque;

            grid.setSizes();
        },

        grdInvoiceList_OnLoadedData: function (gen_ctrl) {
            var grid = ICS140.Grids.grdInvoiceList;

            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);

                if (gen_ctrl) {
                    GenerateDetailButton(grid, ICS140.Grids.grdInvoiceListColumns.DetailButton, row_id, ICS140.Grids.grdInvoiceListColumns.DetailButton, true);
                }

                var firstfeeflag = grid.cells(row_id, grid.getColIndexById(ICS140.Grids.grdInvoiceListColumns.FirstFeeFlag)).getValue();
                if (firstfeeflag == "1") {
                    GenerateImageButtonToGrid(grid, ICS140.Grids.grdInvoiceListColumns.FirstFeeFlagIcon, row_id, ICS140.Grids.grdInvoiceListColumns.FirstFeeFlagIcon, true, "F.png", " ")
                    var imgFirstFeeFlag = GenerateGridControlID(ICS140.Grids.grdInvoiceListColumns.FirstFeeFlagIcon, row_id);
                    $("#" + imgFirstFeeFlag)
                        .attr("title", " ")
                        .css("cursor", "default")
                        .css("width", "16px")
                        .css("height", "16px");
                }

                var isSelected = grid.cells(row_id, grid.getColIndexById(ICS140.Grids.grdInvoiceListColumns.SelectedFlag)).getValue();
                grid.cells(row_id, grid.getColIndexById(ICS140.Grids.grdInvoiceListColumns.SelectButton))
                    .setValue(GenerateCheckBox2(ICS140.Grids.grdInvoiceListColumns.SelectButton, row_id, "", true, isSelected));

                BindGridCheckBoxClickEvent(ICS140.Grids.grdInvoiceListColumns.SelectButton, row_id, ICS140.EventHandlers.chkSelect_OnClick);
                BindGridButtonClickEvent(ICS140.Grids.grdInvoiceListColumns.DetailButton, row_id, ICS140.EventHandlers.btnInvoiceDetailButton_OnClick);
            }

            var selectColIdx = ICS140.Grids.grdInvoiceList.getColIndexById(ICS140.Grids.grdInvoiceListColumns.SelectButton);
            if (ICS140.InputMode == true) {
                $(ICS140.CtrlID.chkSelectAll).show();
                ICS140.Grids.grdInvoiceList.setColumnHidden(selectColIdx, false);
            }
            else {
                $(ICS140.CtrlID.chkSelectAll).hide();
                ICS140.Grids.grdInvoiceList.setColumnHidden(selectColIdx, true);
            }

            grid.setSizes();
        },

        grdInvoiceDetail_OnLoadedData: function (gen_ctrl) {
            var grid = ICS140.Grids.grdInvoiceDetail;

            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);

                if (gen_ctrl) {
                    var contractCode = grid.cells(row_id, grid.getColIndexById(ICS140.Grids.grdInvoiceDetailColumns.ContractCodeShort)).getValue();
                    var lnkContract = "<a href='#' name='ViewContract' row_id='" + row_id + "'>" + contractCode + "</a>";
                    grid.cells(row_id, grid.getColIndexById(ICS140.Grids.grdInvoiceDetailColumns.ContractCodeDisplay)).setValue(lnkContract);
                }

                var firstfeeflag = grid.cells(row_id, grid.getColIndexById(ICS140.Grids.grdInvoiceDetailColumns.FirstFeeFlag)).getValue();
                if (firstfeeflag == "1") {
                    GenerateImageButtonToGrid(grid, ICS140.Grids.grdInvoiceDetailColumns.FirstFeeFlagIcon, row_id, ICS140.Grids.grdInvoiceDetailColumns.FirstFeeFlagIcon, true, "F.png", " ")
                    var imgFirstFeeFlag = GenerateGridControlID(ICS140.Grids.grdInvoiceDetailColumns.FirstFeeFlagIcon, row_id);
                    $("#" + imgFirstFeeFlag)
                        .attr("title", " ")
                        .css("cursor", "default")
                        .css("width", "16px")
                        .css("height", "16px");
                }

            }

            $(ICS140.CtrlID.divInvoiceDetail)
                .find("a[name='ViewContract']")
                .unbind("click")
                .click(ICS140.EventHandlers.lnkContract_OnClick);

            grid.setSizes();
        },

        grdHistory_OnLoadedData: function (gen_ctrl) {
            var grid = ICS140.Grids.grdHistory;

            grid.setSizes();
        },

        cboBillingOffice_OnChange: function () {
            ICS140.RetriveCustList(); //Load data on-start            
        },

        btnSearch_OnClick: function () {
            $(ICS140.CtrlID.divCustDebtInfoPanel).hide();
            ICS140.RetriveCustList();
        },

        btnClear_OnClick: function () {
            ICS140.ClearScreen();
        },

        btnPaidButton_OnClick: function (row_id) {

            var obj = {
                module: "Common",
                code: "MSG0028",
                param: PaidConfirmParam
            };

            ajax_method.CallScreenController("/Shared/GetMessage", obj, function (result) {
                OpenOkCancelDialog(result.Code, result.Message
                    , function () {
                        master_event.LockWindow(true);

                        var grid = ICS140.Grids.grdCustList;
                        var row_index = grid.getRowIndex(row_id);
                        grid.selectRow(row_index);

                        var billingTargetCode = grid.cells(row_id, grid.getColIndexById(ICS140.Grids.grdCustListColumns.BillingTargetCode)).getValue();
                        var serviceTypeCode = grid.cells(row_id, grid.getColIndexById(ICS140.Grids.grdCustListColumns.ServiceTypeCode)).getValue();

                        var param = {
                            billingTargetCode: billingTargetCode,
                            serviceTypeCode: serviceTypeCode
                        };

                        ajax_method.CallScreenController("/income/ICS140_SavePaid", param, function (result, controls, isWarning) {
                            if (result != undefined) {
                                grid.cells(row_id, grid.getColIndexById(ICS140.Grids.grdCustListColumns.DebtTracingStatusDesc)).setValue(result.DebtTracingStatusDesc);
                                grid.cells(row_id, grid.getColIndexById(ICS140.Grids.grdCustListColumns.DebtTracingSubStatus)).setValue(result.DebtTracingSubStatus);

                                var ctrl = "#" + GenerateGridControlID(ICS140.Grids.grdCustListColumns.PaidButton, row_id);
                                $(ctrl).SetDisabled(true);
                            }
                            master_event.LockWindow(false);
                        }, false);
                    }
                    , function () {

                    }
                );
            });
        },

        btnSelectButton_OnClick: function (row_id) {
            master_event.LockWindow(true);

            var grid = ICS140.Grids.grdCustList;
            var row_index = grid.getRowIndex(row_id);
            grid.selectRow(row_index);

            var billingOffice = grid.cells(row_id, grid.getColIndexById(ICS140.Grids.grdCustListColumns.BillingOfficeDisplay)).getValue();
            var billingTargetCode = grid.cells(row_id, grid.getColIndexById(ICS140.Grids.grdCustListColumns.BillingTargetCode)).getValue();
            var billingOfficeCode = grid.cells(row_id, grid.getColIndexById(ICS140.Grids.grdCustListColumns.BillingOfficeCode)).getValue();
            var billingTargetCodeShort = grid.cells(row_id, grid.getColIndexById(ICS140.Grids.grdCustListColumns.BillingTargetCodeShort)).getValue();
            var billingClientName = grid.cells(row_id, grid.getColIndexById(ICS140.Grids.grdCustListColumns.BillingClientName)).getValue();
            var contactPerson = grid.cells(row_id, grid.getColIndexById(ICS140.Grids.grdCustListColumns.ContactPersonName)).getValue();
            var phoneNo = grid.cells(row_id, grid.getColIndexById(ICS140.Grids.grdCustListColumns.PhoneNo)).getValue();
            var status = grid.cells(row_id, grid.getColIndexById(ICS140.Grids.grdCustListColumns.DebtTracingSubStatus)).getValue();
            var isHQBranch = grid.cells(row_id, grid.getColIndexById(ICS140.Grids.grdCustListColumns.IsHQBranch)).getValue();
            var currentStatus = grid.cells(row_id, grid.getColIndexById(ICS140.Grids.grdCustListColumns.DebtTracingStatusDesc)).getValue();
            var serviceTypeCode = grid.cells(row_id, grid.getColIndexById(ICS140.Grids.grdCustListColumns.ServiceTypeCode)).getValue();
            var debtTracingSubStatus = grid.cells(row_id, grid.getColIndexById(ICS140.Grids.grdCustListColumns.DebtTracingSubStatus)).getValue();

            $(ICS140.CtrlID.txtBillingOffice).val(billingOffice);
            $(ICS140.CtrlID.txtBillingTargetCode).val(billingTargetCodeShort);
            $(ICS140.CtrlID.txtBillingTargetCode).data("FullCode", billingTargetCode);
            $(ICS140.CtrlID.txtBillingTargetCode).data("OfficeCode", billingOfficeCode);
            $(ICS140.CtrlID.txtBillingTargetCode).data("ServiceTypeCode", serviceTypeCode);
            $(ICS140.CtrlID.txtBillingClientName).val(billingClientName);
            $(ICS140.CtrlID.txtContactPersonName).val(contactPerson);
            $(ICS140.CtrlID.txtPhoneNo).val(phoneNo);

            $(ICS140.CtrlID.divContacts).SetViewMode(true);

            if ((IsHQUser === true && status == DebtTracingStatus.C_DEBT_TRACE_SUBSTATUS_HQ_OUTSTANDING)
                || (status == DebtTracingStatus.C_DEBT_TRACE_SUBSTATUS_BR_OUTSTANDING)) {
                ICS140.InputMode = true;
            }
            else {
                ICS140.InputMode = false;
            }

            //Unlock all result options.
            $(ICS140.CtrlID.cboInputResult).find("span option").unwrap();

            // branch is HQ, not allow to transfer to either Branch or HQ
            if (isHQBranch == "1") {
                $(ICS140.CtrlID.cboInputResult)
                    .find("option[value='" + DebtTracingResult.C_DEBT_TRACE_RESULT_TRANSFER_TO_BRANCH + "']")
                    .wrap("<span></span>");
                $(ICS140.CtrlID.cboInputResult)
                    .find("option[value='" + DebtTracingResult.C_DEBT_TRACE_RESULT_TRANSFER_TO_HQ + "']")
                    .wrap("<span></span>");
            }
            // Debt tracing handling by HQ, allow transfer to branch.
            else if (status.slice(0, 1) == "1") {
                $(ICS140.CtrlID.cboInputResult)
                    .find("option[value='" + DebtTracingResult.C_DEBT_TRACE_RESULT_TRANSFER_TO_HQ + "']")
                    .wrap("<span></span>");
            }
            else if (status.slice(0, 1) == "2") {
                // Debt tracing handling by Branch, allow HQ user to only transfer back to HQ.
                if (IsHQUser) {
                    $(ICS140.CtrlID.cboInputResult).find("option").each(function () {
                        if ($(this).val() != DebtTracingResult.C_DEBT_TRACE_RESULT_TRANSFER_TO_HQ) {
                            $(this).wrap("<span></span>");
                        }
                    });
                }
                // Debt tracing handling by Branch, allow transfer to HQ.
                else {
                    $(ICS140.CtrlID.cboInputResult)
                    .find("option[value='" + DebtTracingResult.C_DEBT_TRACE_RESULT_TRANSFER_TO_BRANCH + "']")
                    .wrap("<span></span>");
                }
            }

            if ($(ICS140.CtrlID.cboInputResult).children("option").length <= 0) {
                ICS140.InputMode = false;
            }

            //2016-01-13 : SA requested to allow input in every status.
            ICS140.InputMode = true;

            if (ICS140.InputMode == true) {
                $(ICS140.CtrlID.divDebtTracingInput).clearForm();
                $(ICS140.CtrlID.txtCurrentStatus).val(currentStatus);

                $(ICS140.CtrlID.divDebtTracingInput).show();
            }
            else {
                $(ICS140.CtrlID.divDebtTracingInput).hide();
            }


            $(ICS140.CtrlID.divInvoiceDetail).hide();
            $(ICS140.CtrlID.divCustDebtInfoPanel).show();

            var paramReturnedCheque = {
                billingTargetCode: billingTargetCode
            };

            $(ICS140.CtrlID.divGridReturnedCheque).LoadDataToGrid(
                ICS140.Grids.grdReturnedCheque,
                0,
                true,
                "/Income/ICS140_RetriveReturnedCheque",
                paramReturnedCheque,
                "doReturnedCheque",
                false,
                function (result, controls, isWarning) { //post-load
                    //master_event.LockWindow(false);
                    //master_event.ScrollWindow(ICS140.CtrlID.divCustDebtInfoPanel);
                },
                function () { //pre-load
                    $(ICS140.CtrlID.divGridReturnedCheque).show();
                }
            );

            var paramInvoiceList = {
                billingTargetCode: billingTargetCode,
                serviceTypeCode: serviceTypeCode,
                debtTracingSubStatus: debtTracingSubStatus
            };

            $(ICS140.CtrlID.divGridInvoiceList).LoadDataToGrid(
                ICS140.Grids.grdInvoiceList,
                ICS140.ROWS_PER_PAGE_FOR_DETAIL,
                true,
                "/Income/ICS140_RetriveInvoiceList",
                paramInvoiceList,
                "doDebtTracingInvoiceList",
                false,
                function (result, controls, isWarning) { //post-load
                    ICS140.RefreshResultMode();
                    master_event.LockWindow(false);
                    master_event.ScrollWindow(ICS140.CtrlID.divCustDebtInfoPanel);
                },
                function () { //pre-load
                    $(ICS140.CtrlID.divGridInvoiceList).show();
                }
            );

            var paramHistory = {
                billingTargetCode: billingTargetCode
            };

            $(ICS140.CtrlID.divGridHistory).LoadDataToGrid(
                ICS140.Grids.grdHistory,
                10,
                true,
                "/Income/ICS140_RetriveHistory",
                paramHistory,
                "doDebtTracingHistory",
                false,
                function (result, controls, isWarning) { //post-load
                    //master_event.LockWindow(false);
                    //master_event.ScrollWindow(ICS140.CtrlID.divCustDebtInfoPanel);
                },
                function () { //pre-load
                    $(ICS140.CtrlID.divGridHistory).show();
                }
            );
        },

        btnInvoiceDetailButton_OnClick: function (row_id) {
            master_event.LockWindow(true);

            var grid = ICS140.Grids.grdInvoiceList;
            var row_index = grid.getRowIndex(row_id);
            grid.selectRow(row_index);

            var billingTargetCode = grid.cells(row_id, grid.getColIndexById(ICS140.Grids.grdInvoiceListColumns.BillingTargetCode)).getValue();
            var invoiceNo = grid.cells(row_id, grid.getColIndexById(ICS140.Grids.grdInvoiceListColumns.InvoiceNo)).getValue();
            var invoiceOCC = grid.cells(row_id, grid.getColIndexById(ICS140.Grids.grdInvoiceListColumns.InvoiceOCC)).getValue();

            var param = {
                billingTargetCode: billingTargetCode,
                invoiceNo: invoiceNo,
                invoiceOCC: invoiceOCC
            }

            $(ICS140.CtrlID.divInvoiceDetail).show();

            $(ICS140.CtrlID.divGridInvoiceDetail).LoadDataToGrid(
                ICS140.Grids.grdInvoiceDetail,
                ICS140.ROWS_PER_PAGE_FOR_DETAIL,
                true,
                "/Income/ICS140_RetriveInvoiceDetail",
                param,
                "doDebtTracingInvoiceDetail",
                false,
                function (result, controls, isWarning) { //post-load
                    master_event.LockWindow(false);
                    //master_event.ScrollWindow(ICS140.CtrlID.divInvoiceDetail);
                },
                function () { //pre-load
                    $(ICS140.CtrlID.divGridInvoiceDetail).show();
                }
            );

        },

        chkSelect_OnClick: function (row_id, checked) {
            var grid = ICS140.Grids.grdInvoiceList;
            grid.cells(row_id, grid.getColIndexById(ICS140.Grids.grdInvoiceListColumns.SelectedFlag)).setValue(checked ? "1" : "");

            var checkedAll = $(ICS140.CtrlID.chkSelectAll).prop("checked");
            if (checkedAll && !checked) {
                $(ICS140.CtrlID.chkSelectAll).attr("checked", false);
            }
        },

        chkSelectAll_OnClick: function () {
            var checked = $(ICS140.CtrlID.chkSelectAll).is(":checked");
            var grid = ICS140.Grids.grdInvoiceList;
            if (!CheckFirstRowIsEmpty(grid)) {
                for (var i = 0; i < grid.getRowsNum(); i++) {
                    var row_id = grid.getRowId(i);
                    var ctrl = "#" + GenerateGridControlID(ICS140.Grids.grdInvoiceListColumns.SelectButton, row_id);
                    $(ctrl).attr("checked", checked);
                    grid.cells(row_id, grid.getColIndexById(ICS140.Grids.grdInvoiceListColumns.SelectedFlag)).setValue(checked ? "1" : "");
                }
            }
        },

        lnkContract_OnClick: function (ctrl) {
            var row_id = $(this).prop("row_id");
            if (!row_id) {
                return false;
            }

            var grid = ICS140.Grids.grdInvoiceDetail;
            var contractCode = grid.cells(row_id, grid.getColIndexById(ICS140.Grids.grdInvoiceDetailColumns.ContractCodeShort)).getValue();
            var billingOCC = grid.cells(row_id, grid.getColIndexById(ICS140.Grids.grdInvoiceDetailColumns.BillingOCC)).getValue();

            var obj = {
                ContractCode: contractCode,
                BillingOCC: billingOCC
            };

            ajax_method.CallScreenControllerWithAuthority("/Common/CMS450", obj, true);
        },

        cboInputResult_OnChange: function () {
            ICS140.RefreshResultMode();
        },

        btnSave_OnClick: function () {
            ICS140.SaveInput();
        },

        btnCancel_OnClick: function () {
            $(ICS140.CtrlID.divCustDebtInfoPanel).hide();
        }
    },

    RetriveCustList: function () {
        master_event.LockWindow(true);

        var params = {
            BillingOfficeCode: $(ICS140.CtrlID.cboBillingOffice).val(),
            ContractCode: $(ICS140.CtrlID.txtSearchContractCode).val(),
            BillingClientName: $(ICS140.CtrlID.txtSearchBillingClient).val(),
            InvoiceNo: $(ICS140.CtrlID.txtSearchInvoiceNo).val(),
            FirstFeeFlag: $(ICS140.CtrlID.chkFirstFeeFlag).is(":checked"),
            ShowLawsuit: $(ICS140.CtrlID.chkShowLawsuit).is(":checked"),
            ShowBranch: $(ICS140.CtrlID.chkShowBranch).is(":checked"),
            ShowNotDue: $(ICS140.CtrlID.chkShowNotDue).is(":checked"),
            ShowPending: $(ICS140.CtrlID.chkShowPending).is(":checked"),
            ShowOutstanding: $(ICS140.CtrlID.chkShowOutStanding).is(":checked")
        }

        if (!(params.BillingOfficeCode)) {
            params.BillingOfficeCode = null;
        }

        $(ICS140.CtrlID.divGridCustList).LoadDataToGrid(
            ICS140.Grids.grdCustList,
            ROWS_PER_PAGE_FOR_SEARCHPAGE,
            true,
            "/Income/ICS140_RetriveCustList",
            params,
            "doDebtTracingCustList",
            false,
            function (result, controls, isWarning) { //post-load
                master_event.LockWindow(false);
            },
            function () { //pre-load
                $(ICS140.CtrlID.divGridCustList).show();
            }
        );
    },

    RefreshResultMode: function () {
        var result = $(ICS140.CtrlID.cboInputResult).val();

        if (result == DebtTracingResult.C_DEBT_TRACE_RESULT_WAIT_MATCH) {
            $(ICS140.CtrlID.txtNextCallDate).EnableDatePicker(true);
            $(ICS140.CtrlID.txtInputEstimateDate).val(null).EnableDatePicker(false);
            $(ICS140.CtrlID.txtInputAmount).val(null).SetDisabled(true);
            $(ICS140.CtrlID.txtInputAmount).NumericCurrency().val(DebtTracingResult.C_CURRENCY_LOCAL).SetDisabled(true);
            $(ICS140.CtrlID.txtInputAmountUsd).val(null).SetDisabled(true);
            $(ICS140.CtrlID.txtInputAmountUsd).NumericCurrency().val(DebtTracingResult.C_CURRENCY_US).SetDisabled(true);
            $(ICS140.CtrlID.cboInputPaymentMethod).val(null).SetDisabled(true);
            $(ICS140.CtrlID.cboInputPostponeReason).val(null).SetDisabled(true);
            $(ICS140.CtrlID.txtRemark).SetDisabled(false);
        }
        else if (result == DebtTracingResult.C_DEBT_TRACE_RESULT_POSTPONE) {
            $(ICS140.CtrlID.txtNextCallDate).EnableDatePicker(true);
            $(ICS140.CtrlID.txtInputEstimateDate).val(null).EnableDatePicker(false);
            $(ICS140.CtrlID.txtInputAmount).val(null).SetDisabled(true);
            $(ICS140.CtrlID.txtInputAmount).NumericCurrency().val(DebtTracingResult.C_CURRENCY_LOCAL).SetDisabled(true);
            $(ICS140.CtrlID.txtInputAmountUsd).val(null).SetDisabled(true);
            $(ICS140.CtrlID.txtInputAmountUsd).NumericCurrency().val(DebtTracingResult.C_CURRENCY_US).SetDisabled(true);
            $(ICS140.CtrlID.cboInputPaymentMethod).val(null).SetDisabled(true);
            $(ICS140.CtrlID.cboInputPostponeReason).SetDisabled(false);
            $(ICS140.CtrlID.txtRemark).SetDisabled(false);
        }
        else if (result == DebtTracingResult.C_DEBT_TRACE_RESULT_WAIT_FOR_PAYMENT) {
            $(ICS140.CtrlID.txtNextCallDate).EnableDatePicker(true);
            $(ICS140.CtrlID.txtInputEstimateDate).EnableDatePicker(true);
            $(ICS140.CtrlID.txtInputAmount).SetDisabled(false);
            $(ICS140.CtrlID.txtInputAmount).NumericCurrency().SetDisabled(true);
            $(ICS140.CtrlID.txtInputAmountUsd).val(null).SetDisabled(false);
            $(ICS140.CtrlID.txtInputAmountUsd).NumericCurrency().val(DebtTracingResult.C_CURRENCY_US).SetDisabled(true);
            $(ICS140.CtrlID.cboInputPaymentMethod).SetDisabled(false);
            $(ICS140.CtrlID.cboInputPostponeReason).val(null).SetDisabled(true);
            $(ICS140.CtrlID.txtRemark).SetDisabled(false);
        }
        else if (result == DebtTracingResult.C_DEBT_TRACE_RESULT_TRANSFER_TO_BRANCH
            || result == DebtTracingResult.C_DEBT_TRACE_RESULT_TRANSFER_TO_HQ
            || result == DebtTracingResult.C_DEBT_TRACE_RESULT_LAWSUIT
        ) {
            $(ICS140.CtrlID.txtNextCallDate).val(null).EnableDatePicker(false);
            $(ICS140.CtrlID.txtInputEstimateDate).val(null).EnableDatePicker(false);
            $(ICS140.CtrlID.txtInputAmount).val(null).SetDisabled(true);
            $(ICS140.CtrlID.txtInputAmount).NumericCurrency().val(DebtTracingResult.C_CURRENCY_LOCAL).SetDisabled(true);
            $(ICS140.CtrlID.txtInputAmountUsd).val(null).SetDisabled(true);
            $(ICS140.CtrlID.txtInputAmountUsd).NumericCurrency().val(DebtTracingResult.C_CURRENCY_US).SetDisabled(true);
            $(ICS140.CtrlID.cboInputPaymentMethod).val(null).SetDisabled(true);
            $(ICS140.CtrlID.cboInputPostponeReason).val(null).SetDisabled(true);
            $(ICS140.CtrlID.txtRemark).SetDisabled(false);
        }
        else {
            $(ICS140.CtrlID.txtNextCallDate).val(null).EnableDatePicker(false);
            $(ICS140.CtrlID.txtInputEstimateDate).val(null).EnableDatePicker(false);
            $(ICS140.CtrlID.txtInputAmount).val(null).SetDisabled(true);
            $(ICS140.CtrlID.txtInputAmount).NumericCurrency().val(DebtTracingResult.C_CURRENCY_LOCAL).SetDisabled(true);
            $(ICS140.CtrlID.txtInputAmountUsd).val(null).SetDisabled(true);
            $(ICS140.CtrlID.txtInputAmountUsd).NumericCurrency().val(DebtTracingResult.C_CURRENCY_US).SetDisabled(true);
            $(ICS140.CtrlID.cboInputPaymentMethod).val(null).SetDisabled(true);
            $(ICS140.CtrlID.cboInputPostponeReason).val(null).SetDisabled(true);
            $(ICS140.CtrlID.txtRemark).val(null).SetDisabled(true);
        }
    },

    ClearScreen: function () {
        $(ICS140.CtrlID.divDTList).clearForm();

        $(ICS140.CtrlID.chkShowOutStanding).prop("checked", true);
        $(ICS140.CtrlID.divGridCustList).hide();
        $(ICS140.CtrlID.divCustDebtInfoPanel).hide();
        $(ICS140.CtrlID.divInvoiceDetail).hide();

    },

    InitializeGrid: function () {
        ICS140.Grids.grdCustList = $(ICS140.CtrlID.divGridCustList).InitialGrid(
            ROWS_PER_PAGE_FOR_SEARCHPAGE, true, "/income/ICS140_InitialGridCustList"
            , function () {
                BindOnLoadedEvent(ICS140.Grids.grdCustList, ICS140.EventHandlers.grdCustList_OnLoadedData);
            }
        );

        ICS140.Grids.grdReturnedCheque = $(ICS140.CtrlID.divGridReturnedCheque).InitialGrid(
            0, true, "/income/ICS140_InitialGridReturnedCheque"
            , function () {
                BindOnLoadedEvent(ICS140.Grids.grdReturnedCheque, ICS140.EventHandlers.grdReturnedCheque_OnLoadedData);
            }
        );

        ICS140.Grids.grdInvoiceList = $(ICS140.CtrlID.divGridInvoiceList).InitialGrid(
            ICS140.ROWS_PER_PAGE_FOR_DETAIL, true, "/income/ICS140_InitialInvoiceList"
            , function () {
                BindOnLoadedEvent(ICS140.Grids.grdInvoiceList, ICS140.EventHandlers.grdInvoiceList_OnLoadedData);
            }
        );

        ICS140.Grids.grdInvoiceDetail = $(ICS140.CtrlID.divGridInvoiceDetail).InitialGrid(
            ICS140.ROWS_PER_PAGE_FOR_DETAIL, true, "/income/ICS140_InitialInvoiceDetail"
            , function () {
                BindOnLoadedEvent(ICS140.Grids.grdInvoiceDetail, ICS140.EventHandlers.grdInvoiceDetail_OnLoadedData);
            }
        );

        ICS140.Grids.grdHistory = $(ICS140.CtrlID.divGridHistory).InitialGrid(
            10, true, "/income/ICS140_InitialHistory"
            , function () {
                BindOnLoadedEvent(ICS140.Grids.grdHistory, ICS140.EventHandlers.grdHistory_OnLoadedData);
            }
        );
    },

    SaveInput: function () {
        master_event.LockWindow(true);

        var param = {
            BillingTargetCode: $(ICS140.CtrlID.txtBillingTargetCode).data("FullCode"),
            BillingOfficeCode: $(ICS140.CtrlID.txtBillingTargetCode).data("OfficeCode"),
            ServiceTypeCode: $(ICS140.CtrlID.txtBillingTargetCode).data("ServiceTypeCode"),
            Result: $(ICS140.CtrlID.cboInputResult).val(),
            NextCallDate: $(ICS140.CtrlID.txtNextCallDate).val(),
            EstimateDate: $(ICS140.CtrlID.txtInputEstimateDate).val(),
            Amount: $(ICS140.CtrlID.txtInputAmount).NumericValue(),
            AmountUsd: $(ICS140.CtrlID.txtInputAmountUsd).NumericValue(),
            AmountCurrencyType: $(ICS140.CtrlID.txtInputAmount).NumericCurrencyValue(),
            PaymentMethod: $(ICS140.CtrlID.cboInputPaymentMethod).val(),
            PostponeReason: $(ICS140.CtrlID.cboInputPostponeReason).val(),
            Remark: $(ICS140.CtrlID.txtRemark).val(),
            InvoiceNoList: []
        };

        var grid = ICS140.Grids.grdInvoiceList;
        if (!CheckFirstRowIsEmpty(grid)) {
            for (var i = 0; i < grid.getRowsNum(); i++) {
                var row_id = grid.getRowId(i);
                var checked = grid.cells(row_id, grid.getColIndexById(ICS140.Grids.grdInvoiceListColumns.SelectedFlag)).getValue();
                if (checked == "1") {
                    param.InvoiceNoList.push(grid.cells(row_id, grid.getColIndexById(ICS140.Grids.grdInvoiceListColumns.InvoiceNo)).getValue());
                }
            }
        }

        // Not requrired to select invoice.
        //if (param.InvoiceNoList.length <= 0) {
        //    var messageParam = { "module": "Common", "code": "MSG0161", "param": "" };
        //    call_ajax_method("/Shared/GetMessage", messageParam, function (data) {
        //        OpenWarningDialog(data.Message);
        //        master_event.ScrollWindow(ICS140.CtrlID.divInvoiceList);
        //    });

        //    master_event.LockWindow(false);
        //    return;
        //}

        ajax_method.CallScreenController("/income/ICS140_SaveInput", param, function (result, controls, isWarning) {
            if (controls != undefined) {
                VaridateCtrl(["cboInputResult", "txtNextCallDate", "txtInputEstimateDate", "txtInputAmount", "txtInputAmountUsd", "cboInputPaymentMethod", "cboInputPostponeReason"], controls);
                master_event.LockWindow(false);
            }
            else if (result != undefined) {
                var obj = {
                    module: "Common",
                    code: "MSG0046",
                    param: ""
                };

                ajax_method.CallScreenController("/Shared/GetMessage", obj, function (result) {
                    master_event.LockWindow(false);
                    OpenInformationMessageDialog(result.Code, result.Message, function () {
                        $(ICS140.CtrlID.divCustDebtInfoPanel).hide();
                        ICS140.RetriveCustList();
                    });
                });
            }
        }, false);
    },

    InitializeScreen: function () {
        $(ICS140.CtrlID.btnSearch).click(ICS140.EventHandlers.btnSearch_OnClick);
        $(ICS140.CtrlID.btnClear).click(ICS140.EventHandlers.btnClear_OnClick);

        $(ICS140.CtrlID.divInvoiceList).on("click", ICS140.CtrlID.chkSelectAll, ICS140.EventHandlers.chkSelectAll_OnClick);

        $(ICS140.CtrlID.divContacts).SetViewMode(true);

        $(ICS140.CtrlID.cboInputResult).change(ICS140.EventHandlers.cboInputResult_OnChange);

        $(ICS140.CtrlID.txtNextCallDate).InitialDate();
        $(ICS140.CtrlID.txtNextCallDate).SetMinDate(1);

        $(ICS140.CtrlID.txtInputEstimateDate).InitialDate();
        $(ICS140.CtrlID.txtInputEstimateDate).SetMinDate(0);

        $(ICS140.CtrlID.txtInputAmount).BindNumericBox(12, 2, 0, 999999999999.99);
        $(ICS140.CtrlID.txtInputAmountUsd).BindNumericBox(12, 2, 0, 999999999999.99);

        $(ICS140.CtrlID.txtRemark).SetMaxLengthTextArea(250);

        $(ICS140.CtrlID.btnSave).click(ICS140.EventHandlers.btnSave_OnClick);
        $(ICS140.CtrlID.btnCancel).click(ICS140.EventHandlers.btnCancel_OnClick);

        ICS140.ClearScreen();
        ICS140.RefreshResultMode();
    }
};

$(document).ready(function () {
    ICS140.InitializeScreen();
    ICS140.InitializeGrid();
});
