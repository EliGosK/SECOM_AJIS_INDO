/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>

/*--- Main ---*/
var mode = "";
var gridCancelContractCTS110 = null;
var gridRemovalInstallFeeCTS110 = null;
var gridRemovalFeeBillingCTS110 = null;
var isInitial = false;

$(document).ready(function () {
    _isp1 = (_isp1.toUpperCase() == "TRUE") ? true : false;
    InitialControl();
    InitialEvent();
    SetInitialState();
});

function RegisterCommandControl() {
    SetRegisterCommand(true, command_register_click);
    SetResetCommand(true, command_reset_click);
    SetConfirmCommand(false, null);
    SetBackCommand(false, null);
}

function ConfirmCommandControl() {
    SetRegisterCommand(false, null);
    SetResetCommand(false, null);
    SetConfirmCommand(true, command_confirm_click);
    SetBackCommand(true, command_back_click);
}

function HideCommandControl() {
    SetRegisterCommand(false, null);
    SetResetCommand(false, null);
    SetConfirmCommand(false, null);
    SetBackCommand(false, null);
}

function InitialControl() {
    $("#dpCancelDate").InitialDate();
    InitialDateFromToControl("#dpPeriodFrom", "#dpPeriodTo");

    $("#txtFee").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#txtTax").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#txtNormalFee").BindNumericBox(12, 2, 0, 999999999999.99);

    $("#TotalSlideAmt").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#TotalReturnAmt").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#TotalBillingAmt").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#TotalAmtAfterCounterBalance").BindNumericBox(12, 2, 0, 999999999999.99);

    $("#TotalSlideAmtUsd").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#TotalReturnAmtUsd").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#TotalBillingAmtUsd").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#TotalAmtAfterCounterBalanceUsd").BindNumericBox(12, 2, 0, 999999999999.99);
}

function InitialEvent() {
    $("#ddlFeeType").change(free_type_change);
    $("#btnAdd").click(add_button_click);
    $("#btnCancel").click(cancel_button_click);
    $("#btnNew").click(new_button_click);

    $("#btnRetrieveBillingTarget").click(retrieve_billing_target_button_click);
    $("#btnRetrieveBillingClient").click(retrieve_billing_client_button_click);
    
    $("#btnCancelBilling").click(cancel_billing_button_click);

    $("#rdoTargetCode").click(target_code_click);
    $("#rdoClientCode").click(client_code_click);

    $("#btnSearchBillingClient").click(function () {
        $("#dlgCTS110").OpenCMS270Dialog();
    });

    $("#btnCopy").click(copy_button_click);
    $("#btnNewEdit").click(new_edit_button_click);
    $("#btnClearBillingTarget").click(clear_bill_target_button_click);
    $("#btnAddUpdate").click(add_update_button_click);

    InitialTrimTextEvent(["txtApproveNo1", "txtApproveNo2"]);
}

function SetInitialState() {
    isInitial = true;

    $("#divBillingTargetDetailSection").hide();
    $("#divBillingTargetSection").hide();

    ClearBillingDetail(true);
    $("#divCancelContract").clearForm();
    $("#divCondition").clearForm();

    ReGenerateHandlingTypeCombo();

    $("#divProcessCounterBalance input[type='radio']").attr("disabled", true);
    $("#divProcessCounterBalanceUsd input[type='radio']").attr("disabled", true);

    //Set maxlenght for TextArea control
    $("#txtRemark").SetMaxLengthTextArea(100);
    $("#OtherRemarks").SetMaxLengthTextArea(4000); //(1600);

    if ($("#ContractStatus").val() == $("#C_CONTRACT_STATUS_BEF_START").val()) {
        $("#divStopType").attr("disabled", true);
        $("#divRequireStopType").hide();

        $("#ddlCancelReason").attr("disabled", false);
        $("#divCancelReason").show();
    }
    else {
        $("#ddlCancelReason").attr("disabled", true);
        $("#divCancelReason").hide();

        $("#divStopType").attr("disabled", false);
        $("#divRequireStopType").show();
    }

    //default CancelDate
    var currentDate = new Date();
    $("#dpCancelDate").datepicker("setDate", currentDate);
    $("#dpCancelDate").val(ConvertDateToTextFormat(ConvertDateObject(currentDate)));

    InitialCancelContractConditionGridData();
    InitialRemovalInstallationFeeGridData();
    InitialRemovalFeeBillingGridData();

    $("#btnNew").attr("disabled", true);
    RegisterCommandControl();
}

function SetResetState() {
    SetInitialState();

    SetContractBasicInfoMode(false);
    SetCancelContractMode(false);
    SetCancelContractConditionMode(false);
    SetBillingTargetSectionMode(false);
}


function InitialCancelContractConditionGridData() {
    gridCancelContractCTS110 = $("#gridCancelContractCondition").LoadDataToGridWithInitial(0, false, false, "/Contract/CTS110_GetCancelContractConditionDetail",
    "", "CTS110_CancelContractConditionGridData", false);

    SpecialGridControl(gridCancelContractCTS110, ["Remove"]);

    BindOnLoadedEvent(gridCancelContractCTS110,
        function (gen_ctrl) {
            gridCancelContractCTS110.setColumnHidden(gridCancelContractCTS110.getColIndexById('FeeTypeCode'), true);
            gridCancelContractCTS110.setColumnHidden(gridCancelContractCTS110.getColIndexById('HandlingTypeCode'), true);
            gridCancelContractCTS110.setColumnHidden(gridCancelContractCTS110.getColIndexById('Sequence'), true);

            if (CheckFirstRowIsEmpty(gridCancelContractCTS110, false) == false) {

                for (var i = 0; i < gridCancelContractCTS110.getRowsNum(); i++) {
                    var removeColinx = gridCancelContractCTS110.getColIndexById('Remove');
                    var row_id = gridCancelContractCTS110.getRowId(i);

                    if (gen_ctrl == true) {
                        //gridCancelContractCTS110.cells2(i, removeColinx).setValue(GenerateHtmlButton("btnRemove", row_id, "Remove", true));
                        GenerateRemoveButton(gridCancelContractCTS110, "btnRemove", row_id, "Remove", true);
                    }

                    /* --- Set Event --- */
                    /* ----------------- */
                    BindGridButtonClickEvent("btnRemove", row_id,
                        function (row_id) {
                            RemoveDataCancelCond(row_id);
                        }
                    );
                    /* ----------------- */
                }
            }

            gridCancelContractCTS110.setSizes();

            call_ajax_method_json("/Contract/CTS110_GetCancelContractCondition", "",
                function (result, controls) {
                    if (result != undefined) {
                        $("#divConditionMemo").bindJSON(result);

                        if (result.ProcessAfterCounterBalanceType == $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_REFUND").val()) {
                            $("#divProcessCounterBalance #Refund").attr('checked', true);
                        }
                        else if (result.ProcessAfterCounterBalanceType == $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_RECEIVE").val()) {
                            $("#divProcessCounterBalance #ReceiveAsRevenue").attr('checked', true);
                        }
                        else if (result.ProcessAfterCounterBalanceType == $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_BILL").val()) {
                            $("#divProcessCounterBalance #Bill").attr('checked', true);
                        }
                        else if (result.ProcessAfterCounterBalanceType == $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_EXEMPT").val()) {
                            $("#divProcessCounterBalance #Exempt").attr('checked', true);
                        }

                        if (result.ProcessAfterCounterBalanceTypeUsd == $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_REFUND").val()) {
                            $("#divProcessCounterBalanceUsd #RefundUsd").attr('checked', true);
                        }
                        else if (result.ProcessAfterCounterBalanceTypeUsd == $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_RECEIVE").val()) {
                            $("#divProcessCounterBalanceUsd #ReceiveAsRevenueUsd").attr('checked', true);
                        }
                        else if (result.ProcessAfterCounterBalanceTypeUsd == $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_BILL").val()) {
                            $("#divProcessCounterBalanceUsd #BillUsd").attr('checked', true);
                        }
                        else if (result.ProcessAfterCounterBalanceTypeUsd == $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_EXEMPT").val()) {
                            $("#divProcessCounterBalanceUsd #ExemptUsd").attr('checked', true);
                        }

                        //CalculateTotalAmount();
                        $("#TotalSlideAmt").val(result.TotalSlideAmt);
                        $("#TotalReturnAmt").val(result.TotalReturnAmt);
                        $("#TotalBillingAmt").val(result.TotalBillingAmt);
                        $("#TotalAmtAfterCounterBalance").val(result.TotalAmtAfterCounterBalance);

                        $("#TotalSlideAmtUsd").val(result.TotalSlideAmtUsd);
                        $("#TotalReturnAmtUsd").val(result.TotalReturnAmtUsd);
                        $("#TotalBillingAmtUsd").val(result.TotalBillingAmtUsd);
                        $("#TotalAmtAfterCounterBalanceUsd").val(result.TotalAmtAfterCounterBalanceUsd);

                        SetEnableProcessAfterCounterBal();
                    }
                }
            );
        }
    );

}

function InitialRemovalInstallationFeeGridData() {

    gridRemovalInstallFeeCTS110 = $("#gridRemovalInstallationFee").LoadDataToGridWithInitial(0, false, false, "/Contract/CTS110_GetRemovalInstallationFee",
    "", "CTS110_RemovalInstallationFeeGridData", false);

    SpecialGridControl(gridRemovalInstallFeeCTS110, ["Amount", "PaymentMethod"]);

    BindOnLoadedEvent(gridRemovalInstallFeeCTS110,
        function (gen_ctrl) {
            if (CheckFirstRowIsEmpty(gridRemovalInstallFeeCTS110, false) == false) {

                var row_idx = gridRemovalInstallFeeCTS110.getRowsNum() - 1;
                var row_id = gridRemovalInstallFeeCTS110.getRowId(row_idx);

                if (gen_ctrl == true) {
                    //gridRemovalInstallFeeCTS110.cells2(0, 1).setValue(GenerateNumericBoxWithUnit("Amount", row_id, "0", "100px", C_CURRENCY_UNIT, false, false));
                    //$("#Amount").BindNumericBox(10, 2, 0, 9999999999.99);
                    //GenerateNumericBoxWithUnit2(gridRemovalInstallFeeCTS110, "Amount", row_id, "Amount", "0", 12, 2, 0, 999999999999.99, false, "110px", C_CURRENCY_UNIT, false, false);
                    NumericTextBoxWithMultipleCurrency(gridRemovalInstallFeeCTS110, "Amount", row_id, "Amount", 0, C_CURRENCY_LOCAL, 12, 2, 0, 999999999999.99, 0, false, false);

                    gridRemovalInstallFeeCTS110.cells2(0, 2).setValue("<select id='PaymentMethod' name='PaymentMethod' style='width:195px;'><option value='0' selected='selected'>None</option></select>");

                    var obj = { id: "PaymentMethod" };
                    call_ajax_method_json("/Contract/CTS110_GetComboBoxPaymentMethod", obj,
                        function (result, controls) {
                            if (result != undefined) {
                                gridRemovalInstallFeeCTS110.cells2(0, 2).setValue(result);
                                $("#PaymentMethod").val($("#C_PAYMENT_METHOD_BANK_TRANSFER").val());
                            }
                        }
                    );
                }

                gridRemovalInstallFeeCTS110.setSizes();
            }

            //            gridRemovalInstallFeeCTS110.attachEvent("onRowSelect", function (id, ind) {
            //                var row_num = gridRemovalInstallFeeCTS110.getRowIndex(id);
            //                if (gridRemovalInstallFeeCTS110.cell.childNodes[0].tagName == "SELECT") {
            //                    var ddl = gridRemovalInstallFeeCTS110.cell.childNodes[0];
            //                    if (ddl.disabled == false) {  
            //                        ddl.focus();
            //                    }
            //                }
            //            });

            if (isInitial == true) {
                if ($("#ShowRemovalSection").val() == "True") {
                    if ($("#RemovalFeeAmount").val() > 0) {
                        $("#divBillingTargetSection").show();

                        var rowAmount = gridRemovalInstallFeeCTS110.getRowId(0);

                        if (gen_ctrl == true) {
                            ////gridRemovalInstallFeeCTS110.cells2(0, 1).setValue(GenerateNumericBoxWithUnit("Amount", rowAmount, $("#RemovalFeeAmount").val(), "100px", C_CURRENCY_UNIT, false, false));
                            //gridRemovalInstallFeeCTS110.cells2(0, 1).setValue(GenerateNumericBoxWithUnit("Amount", rowAmount, SetNumericValue($("#RemovalFeeAmount").val(), 2), "100px", C_CURRENCY_UNIT, false, false));
                            //$("#Amount").BindNumericBox(10, 2, 0, 9999999999.99);

                            //GenerateNumericBoxWithUnit2(gridRemovalInstallFeeCTS110, "Amount", rowAmount, "Amount", SetNumericValue($("#RemovalFeeAmount").val(), 2), 10, 2, 0, 9999999999.99, false, "110px", C_CURRENCY_UNIT, false, false);

                            var fee = SetNumericValue($("#RemovalFeeAmount").val(), 2);
                            var feeCurrencyType = $("#RemovalFeeAmount").NumericCurrencyValue();
                            NumericTextBoxWithMultipleCurrency(gridRemovalInstallFeeCTS110, "Amount", rowAmount, "Amount", fee, feeCurrencyType, 12, 2, 0, 999999999999.99, 0, false, false);

                        }
                    }
                }

                isInitial = false;
            }
        }
    );

}

function InitialRemovalFeeBillingGridData() {
    gridRemovalFeeBillingCTS110 = $("#gridRemovalFeeBillingTarget").LoadDataToGridWithInitial(0, false, false, "/Contract/CTS110_GetRemovalFeeBillingTarget",
    "", "CTS110_RemovalInstallationFeeGridData", false);

    SpecialGridControl(gridRemovalFeeBillingCTS110, ["Select", "Detail", "Remove"]);

    BindOnLoadedEvent(gridRemovalFeeBillingCTS110,
        function (gen_ctrl) {
            gridRemovalFeeBillingCTS110.setColumnHidden(gridRemovalFeeBillingCTS110.getColIndexById('Sequence'), true);
            gridRemovalFeeBillingCTS110.setColumnHidden(gridRemovalFeeBillingCTS110.getColIndexById('Status'), true);

            BindGridRemovalFeeBilling(true, gen_ctrl);
        }

    );
}

function BindGridRemovalFeeBilling(isEnabled, gen_ctrl) {
    var row_id;
    var billingOCC = "";

    if (CheckFirstRowIsEmpty(gridRemovalFeeBillingCTS110, false) == false) {

        for (var i = 0; i < gridRemovalFeeBillingCTS110.getRowsNum(); i++) {
            row_id = gridRemovalFeeBillingCTS110.getRowId(i);

            var selectColinx = gridRemovalFeeBillingCTS110.getColIndexById('Select');
            var detailColinx = gridRemovalFeeBillingCTS110.getColIndexById('Detail');
            var removeColinx = gridRemovalFeeBillingCTS110.getColIndexById('Remove');
            var occColinx = gridRemovalFeeBillingCTS110.getColIndexById('BillingOCC');
            billingOCC = gridRemovalFeeBillingCTS110.cells(row_id, occColinx).getValue();

            if (gen_ctrl == true) {
                gridRemovalFeeBillingCTS110.cells2(i, selectColinx).setValue(GenerateRadioButton("rdoSelect", row_id, "false", "Select", isEnabled));

                //gridRemovalFeeBillingCTS110.cells2(i, detailColinx).setValue(GenerateHtmlButton("btnDetail", row_id, "Detail", true));
                GenerateEditButton(gridRemovalFeeBillingCTS110, "btnDetail", row_id, "Detail", isEnabled);

                //gridRemovalFeeBillingCTS110.cells2(i, removeColinx).setValue(GenerateHtmlButton("btnRemove", row_id, "Remove", false));
                GenerateRemoveButton(gridRemovalFeeBillingCTS110, "btnRemoveBilling", row_id, "Remove", (billingOCC == "" && isEnabled));
            }
            
            BindGridRadioButtonClickEvent("rdoSelect", row_id, null);            
            
            BindGridButtonClickEvent("btnDetail", row_id,
                function (row_id) {
                    GetBillingTargetDetail(row_id);
                }
            );            
            
            BindGridButtonClickEvent("btnRemoveBilling", row_id,
                function (row_id) {
                    RemoveDataBillingTarget(row_id);
                }
            );

        }

        if (gridRemovalFeeBillingCTS110.getRowsNum() > 0) {
            row_id = gridRemovalFeeBillingCTS110.getRowId(0);

            var rdoSelect = GenerateGridControlID("rdoSelect", row_id);
            $("#" + rdoSelect).attr("checked", true);
        }

        gridRemovalFeeBillingCTS110.setSizes();
    }

    if (CheckFirstRowIsEmpty(gridRemovalFeeBillingCTS110, false) == false 
        && gridRemovalFeeBillingCTS110.getRowsNum() > 0 && billingOCC == "") {
        $("#btnNew").attr("disabled", true);
    }
    else {
        $("#btnNew").attr("disabled", false);
    }
}

function SetContractBasicInfoMode(isview) {
    if (isview) {
        $("#divContractBasicInfo").SetViewMode(true);
    }
    else {
        $("#divContractBasicInfo").SetViewMode(false);
        $("#ContractTargetCustomerImportant").attr("disabled", true);
    }
}

function SetCancelContractMode(isview) {
    if (isview) {
        $("#divCancelContract").SetViewMode(true);
    }
    else {
        $("#divCancelContract").SetViewMode(false);
    }
}

function SetCancelContractConditionMode(isview) {
    if (isview) {
        $("#divCondition").hide();
        $("#divCancelContractCondition").SetViewMode(true);

        if (gridCancelContractCTS110 != undefined) {
            var removeCol = gridCancelContractCTS110.getColIndexById("Remove");
            gridCancelContractCTS110.setColumnHidden(removeCol, true);
            gridCancelContractCTS110.setSizes();

            $("#gridCancelContractCondition").hide();
            $("#gridCancelContractCondition").show();
        }
    }
    else {
        $("#divCancelContractCondition").SetViewMode(false);
        $("#divCondition").show();

        if (gridCancelContractCTS110 != undefined) {
            var removeCol = gridCancelContractCTS110.getColIndexById("Remove");
            gridCancelContractCTS110.setColumnHidden(removeCol, false);
            gridCancelContractCTS110.setSizes();

            $("#gridCancelContractCondition").hide();
            $("#gridCancelContractCondition").LoadDataToGrid(gridCancelContractCTS110, 0, false, "/Contract/CTS110_RefreshCancelContractConditionDetail",
                                                "", "CTS110_CancelContractConditionGridData", false, null, null);
            $("#gridCancelContractCondition").show();
        }

        SetEnableProcessAfterCounterBal();
    }
}

function SetBillingTargetSectionMode(isview) {
    if (isview) {
        $("#divBillingTargetSection").SetViewMode(true);

        gridRemovalInstallFeeCTS110.setSizes();
        if (gridRemovalFeeBillingCTS110 != undefined) {
            var selectCol = gridRemovalFeeBillingCTS110.getColIndexById("Select");
            var detailCol = gridRemovalFeeBillingCTS110.getColIndexById("Detail");
            var removeCol = gridRemovalFeeBillingCTS110.getColIndexById("Remove");
            gridRemovalFeeBillingCTS110.setColumnHidden(detailCol, true);
            gridRemovalFeeBillingCTS110.setColumnHidden(removeCol, true);
            gridRemovalFeeBillingCTS110.setSizes();

            $("#gridRemovalFeeBillingTarget").hide();
            $("#gridRemovalFeeBillingTarget").show();

            //$("#divBillingTargetDetailSection").hide();
        }
    }
    else {
        $("#divBillingTargetSection").SetViewMode(false);

        gridRemovalInstallFeeCTS110.setSizes();
        if (gridRemovalFeeBillingCTS110 != undefined) {
            var selectCol = gridRemovalFeeBillingCTS110.getColIndexById("Select");
            var detailCol = gridRemovalFeeBillingCTS110.getColIndexById("Detail");
            var removeCol = gridRemovalFeeBillingCTS110.getColIndexById("Remove");
            gridRemovalFeeBillingCTS110.setColumnHidden(detailCol, false);
            gridRemovalFeeBillingCTS110.setColumnHidden(removeCol, false);
            gridRemovalFeeBillingCTS110.setSizes();

            $("#gridRemovalFeeBillingTarget").hide();
            $("#gridRemovalFeeBillingTarget").LoadDataToGrid(gridRemovalFeeBillingCTS110, 0, false, "/Contract/CTS110_RefreshBillingTargetDetail",
                                                "", "CTS110_RemovalInstallationFeeGridData", false, null, null);
            $("#gridRemovalFeeBillingTarget").show();
        }

        if ($("#btnSearchBillingClient").prop("disabled")) {
            SetReadonlySpecifyCode(true);
        }

        if ($("#btnCopy").prop("disabled")) {
            SetReadonlyCopyName(true);
        }
    }
}

function SetReadonlySpecifyCode(isReadonly) {
    if (isReadonly) {
        $("#rdoTargetCode").attr("disabled", true);
        $("#rdoClientCode").attr("disabled", true);
        $("#BillingTargetCode").attr("readonly", true);
        $("#BillingClientCode").attr("readonly", true);

        $("#btnRetrieveBillingTarget").attr("disabled", true);
        $("#btnRetrieveBillingClient").attr("disabled", true);

        $("#btnSearchBillingClient").attr("disabled", true);
    }
    else {
        $("#rdoTargetCode").attr("disabled", false);
        $("#rdoClientCode").attr("disabled", false);
        $("#BillingTargetCode").attr("readonly", false);
        $("#BillingClientCode").attr("readonly", false);

        $("#btnRetrieveBillingTarget").attr("disabled", false);
        $("#btnRetrieveBillingClient").attr("disabled", true);

        $("#btnSearchBillingClient").attr("disabled", false);
    }

    if (_isp1) {
        $('#rdoTargetCode').attr('disabled', 'disabled');
        $('#rdoClientCode').attr('disabled', 'disabled');
        $('#BillingTargetCodeSearch').attr('readonly', 'readonly');

        if (!isReadonly) {
            var obj = $('input:radio[name=SpecifyCode]');
            obj.filter('[value=1]').attr('checked', true);
            client_code_click();
        }
    }
}

function SetReadonlyCopyName(isReadonly) {
    if (isReadonly) {
        $("#rdoContractTarget").attr("disabled", true);
        $("#rdoBranchOfContractTarget").attr("disabled", true);
        $("#rdoRealCustomer").attr("disabled", true);
        $("#rdoSite").attr("disabled", true);
        $("#btnCopy").attr("disabled", true);
    }
    else {
        $("#rdoContractTarget").attr("disabled", false);
        $("#rdoBranchOfContractTarget").attr("disabled", false);
        $("#rdoRealCustomer").attr("disabled", false);
        $("#rdoSite").attr("disabled", false);
        $("#btnCopy").attr("disabled", false);
    }
}

function SetBillingTargetDetailMode() {
    mode = "Update";

    $("#divBillingTargetDetailSection").show();

    SetReadonlySpecifyCode(true);
    SetReadonlyCopyName(true);

    if ($("#BillingOCC").val() == "") {
        $("#btnNewEdit").attr("disabled", false);
        $("#btnClearBillingTarget").attr("disabled", false);
        $("#BillingOffice").attr("disabled", false);
        $("#divRequireBillingOffice").show();
        $("#btnAddUpdate").attr("disabled", false);
    }
    else {
        $("#btnNewEdit").attr("disabled", true);
        $("#btnClearBillingTarget").attr("disabled", true);
        $("#BillingOffice").attr("disabled", true);
        $("#divRequireBillingOffice").hide();
        $("#btnAddUpdate").attr("disabled", true);
    }
}

function SetBillingTargetNewMode() {
    if (mode == "Update") {
        BindGridRemovalFeeBilling(true, true);
    }

    mode = "Add";

    $("#divBillingTargetDetailSection").show();

    SetReadonlySpecifyCode(false);
    SetReadonlyCopyName(false);
    $("#BillingClientCode").attr("readonly", true); //default

    $("#btnNewEdit").attr("disabled", false);
    $("#btnClearBillingTarget").attr("disabled", false);
    $("#BillingOffice").attr("disabled", false);
    $("#divRequireBillingOffice").show();
    $("#btnAddUpdate").attr("disabled", false);
}

function InitialInputDataCancelCond() {
    //Initial input data on Cancel contract condition section
    //    $("#divFeeTax").clearForm();
    //    $("#divNormalSlideFee").clearForm();
    //    $("#divPeriod").clearForm();
    //    $("#divRemark").clearForm();

    $("#divCondition").clearForm();
    ClearDateFromToControl("#dpPeriodFrom", "#dpPeriodTo");

    $("#txtFee").attr("readonly", false);
    $("#txtFee").NumericCurrency().attr("disabled", false);
    $("#txtTax").attr("readonly", false);
    $("#txtTax").NumericCurrency().attr("disabled", false);
    $("#txtNormalFee").attr("readonly", false);
    $("#txtNormalFee").NumericCurrency().attr("disabled", false);

    $("#txtContractCodeForSlideFee").attr("readonly", false);
    $("#dpPeriodFrom").attr("readonly", false);
    $("#dpPeriodTo").attr("readonly", false);
    $("#txtRemark").attr("readonly", false);
}

function RemoveDataCancelCond(row_id) {
    gridCancelContractCTS110.selectRow(gridCancelContractCTS110.getRowIndex(row_id));

    /* --- Get Message --- */
    /* ------------------- */
    var obj = {
        module: "Common",
        code: "MSG0097"
    };
    call_ajax_method_json("/Shared/GetMessage", obj,
        function (result) {
            OpenYesNoMessageDialog(result.Code, result.Message,
                function () {
                    var feeTypeCol = gridCancelContractCTS110.getColIndexById("FeeTypeCode");
                    var feeType = gridCancelContractCTS110.cells(row_id, feeTypeCol).getValue();

                    var handlingTypeCol = gridCancelContractCTS110.getColIndexById("HandlingTypeCode"); //Add by Jutarat A. on 17072012
                    var handlingType = gridCancelContractCTS110.cells(row_id, handlingTypeCol).getValue(); //Add by Jutarat A. on 17072012

                    var seqCol = gridCancelContractCTS110.getColIndexById("Sequence");
                    var seq = gridCancelContractCTS110.cells(row_id, seqCol).getValue();

                    //Remove the seleted row from cancel contract condition list
                    DeleteRow(gridCancelContractCTS110, row_id);
                    gridCancelContractCTS110.setSizes();

                    var obj = { strSequence: seq };
                    call_ajax_method_json("/Contract/CTS110_RemoveDataCancelCond", obj,
                        function (result) {
                            if (result != undefined) {
                                if (feeType == $("#C_BILLING_TYPE_REMOVAL_INSTALLATION_FEE").val()
                                    && handlingType == $("#C_HANDLING_TYPE_BILL_UNPAID_FEE").val()) { //Add by Jutarat A. on 17072012
                                
                                    //$("#Amount").val("0");
                                    var rowAmount = gridRemovalInstallFeeCTS110.getRowId(0);

                                    //gridRemovalInstallFeeCTS110.cells2(0, 1).setValue(GenerateNumericBoxWithUnit("Amount", rowAmount, "0", "100px", C_CURRENCY_UNIT, false, false));
                                    //$("#Amount").BindNumericBox(10, 2, 0, 9999999999.99);

                                    //GenerateNumericBoxWithUnit2(gridRemovalInstallFeeCTS110, "Amount", rowAmount, "Amount", "0", 10, 2, 0, 9999999999.99, false, "110px", C_CURRENCY_UNIT, false, false);
                                    NumericTextBoxWithMultipleCurrency(gridRemovalInstallFeeCTS110, "Amount", rowAmount, "Amount", 0, C_CURRENCY_LOCAL, 12, 2, 0, 999999999999.99, 0, false, false);
                                    
                                    $("#divBillingTargetDetailSection").hide();
                                    $("#divBillingTargetSection").hide();
                                }

                                CalculateTotalAmount();
                            }
                        }
                    );
                },
                null);
        }
    );
    /* ------------------- */
}

function RemoveDataBillingTarget(row_id) {
    gridRemovalFeeBillingCTS110.selectRow(gridRemovalFeeBillingCTS110.getRowIndex(row_id));

    /* --- Get Message --- */
    /* ------------------- */
    var obj = {
        module: "Common",
        code: "MSG0097"
    };
    call_ajax_method_json("/Shared/GetMessage", obj,
        function (result) {
            OpenYesNoMessageDialog(result.Code, result.Message,
                function () {
                    var seqCol = gridRemovalFeeBillingCTS110.getColIndexById("Sequence");
                    var seq = gridRemovalFeeBillingCTS110.cells(row_id, seqCol).getValue();

                    //Remove selected row from Billing target list
                    DeleteRow(gridRemovalFeeBillingCTS110, row_id);
                    gridRemovalFeeBillingCTS110.setSizes();

                    var obj = { strSequence: seq };
                    call_ajax_method_json("/Contract/CTS110_RemoveDataBillingTarget", obj,
                        function (result) {
                            if (result != undefined) {
                                cancel_billing_button_click();
                            }
                        }
                    );


                },
                null);
        }
    );
    /* ------------------- */
}

function GetBillingTargetDetail(row_id) {
    $("#btnNew").attr("disabled", true);
    gridRemovalFeeBillingCTS110.selectRow(gridRemovalFeeBillingCTS110.getRowIndex(row_id));

    var seqCol = gridRemovalFeeBillingCTS110.getColIndexById("Sequence");
    var seq = gridRemovalFeeBillingCTS110.cells(row_id, seqCol).getValue();

    var obj = { strSequence: seq };
    call_ajax_method_json("/Contract/CTS110_GetBillingTargetDetail", obj,
        function (result) {
            if (result != undefined) {
                BindGridRemovalFeeBilling(false, true);

                BindBillingDetail(result);
                SetBillingTargetDetailMode();

                CheckEnableBillingOffice();
            }
        }
    );
}

function BindBillingDetail(result) {
    $("#BillingOCC").val(result.BillingOCC);
    $("#BillingTargetCodeDetail").val(result.BillingTargetCodeDetail);
    $("#BillingClientCodeDetail").val(result.BillingClientCodeDetail);
    $("#FullNameEN").val(result.FullNameEN);
    $("#BranchNameEN").val(result.BranchNameEN);
    $("#AddressEN").val(result.AddressEN);
    $("#FullNameLC").val(result.FullNameLC);
    $("#BranchNameLC").val(result.BranchNameLC);
    $("#AddressLC").val(result.AddressLC);
    $("#Nationality").val(result.Nationality);
    $("#PhoneNo").val(result.PhoneNo);
    $("#BusinessType").val(result.BusinessType); //result.BusinessTypeName
    $("#IDNo").val(result.IDNo);

    if (result.BillingOfficeCode != null && result.BillingOfficeCode != "") {
        $("#BillingOffice").val(result.BillingOfficeCode);
    }
}

function command_register_click() {
    command_control.CommandControlMode(false);

    /* --- Set Parameter --- */
    var changeTypeVal = null;
    if ($("#rdoContractExpired").prop("checked")) {
        changeTypeVal = $("#rdoContractExpired").val();
    }
    else if ($("#rdoCancelContract").prop("checked")) {
        changeTypeVal = $("#rdoCancelContract").val();
    }

    var procAfterCounterBal = null;
    if ($("#divProcessCounterBalance #Refund").prop("checked")) {
        procAfterCounterBal = $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_REFUND").val();
    }
    else if ($("#divProcessCounterBalance #ReceiveAsRevenue").prop("checked")) {
        procAfterCounterBal = $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_RECEIVE").val();
    }
    else if ($("#divProcessCounterBalance #Bill").prop("checked")) {
        procAfterCounterBal = $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_BILL").val();
    }
    else if ($("#divProcessCounterBalance #Exempt").prop("checked")) {
        procAfterCounterBal = $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_EXEMPT").val();
    }

    var procAfterCounterBalUsd = null;
    if ($("#divProcessCounterBalanceUsd #RefundUsd").prop("checked")) {
        procAfterCounterBalUsd = $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_REFUND").val();
    }
    else if ($("#divProcessCounterBalanceUsd #ReceiveAsRevenueUsd").prop("checked")) {
        procAfterCounterBalUsd = $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_RECEIVE").val();
    }
    else if ($("#divProcessCounterBalanceUsd #BillUsd").prop("checked")) {
        procAfterCounterBalUsd = $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_BILL").val();
    }
    else if ($("#divProcessCounterBalanceUsd #ExemptUsd").prop("checked")) {
        procAfterCounterBalUsd = $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_EXEMPT").val();
    }

    var removalRow = GetRemovalRowSequence();

    var obj = {
        ChangeType: changeTypeVal,
        ChangeImplementDate: $("#dpCancelDate").val(),
        StopCancelReasonType: $("#ddlCancelReason").val(),
        ApproveNo1: $("#txtApproveNo1").val(),
        ApproveNo2: $("#txtApproveNo2").val(),
        ProcessAfterCounterBalanceType: procAfterCounterBal,
        ProcessAfterCounterBalanceTypeUsd: procAfterCounterBalUsd,
        IsShowBillingTagetDetail: $("#divBillingTargetDetailSection").is(':visible'),
        RemovalRowSequence: removalRow
    };

    call_ajax_method_json("/Contract/CTS110_RegisterCancelContractData", obj, doAfterRegister);

    //command_control.CommandControlMode(true);
}

function doAfterRegister(result, controls) {
    if (controls != undefined) {
        /* --- Higlight Text --- */
        /* --------------------- */
        VaridateCtrl(["dpCancelDate",
                        "ddlCancelReason",
                        "txtApproveNo1"], controls);
        return;
    }
    else if (result != undefined) {
        /* --- Set View Mode --- */
        /* --------------------- */
        SetContractBasicInfoMode(true);
        SetCancelContractMode(true);
        SetCancelContractConditionMode(true);
        SetBillingTargetSectionMode(true);

        ConfirmCommandControl();
    }
}

function command_reset_click() {
    /* --- Get Message --- */
    /* ------------------- */
    var obj = {
        module: "Common",
        code: "MSG0038"
    };
    call_ajax_method_json("/Shared/GetMessage", obj,
        function (result) {
            OpenYesNoMessageDialog(result.Code, result.Message,
                function () {
                    //SetResetState();
                    var obj = { ContractCode: $("#ContractCodeShort").val() };
                    ajax_method.CallScreenControllerWithAuthority("/Contract/CTS110", obj, false, null);
                },
                null);
        }
    );
    /* ------------------- */
}

function command_confirm_click() {
    command_control.CommandControlMode(false);

    /* --- Set Parameter --- */
    var changeTypeVal = null;
    if ($("#rdoContractExpired").prop("checked")) {
        changeTypeVal = $("#rdoContractExpired").val();
    }
    else if ($("#rdoCancelContract").prop("checked")) {
        changeTypeVal = $("#rdoCancelContract").val();
    }

    var procAfterCounterBal = null;
    if ($("#divProcessCounterBalance #Refund").prop("checked")) {
        procAfterCounterBal = $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_REFUND").val();
    }
    else if ($("#divProcessCounterBalance #ReceiveAsRevenue").prop("checked")) {
        procAfterCounterBal = $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_RECEIVE").val();
    }
    else if ($("#divProcessCounterBalance #Bill").prop("checked")) {
        procAfterCounterBal = $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_BILL").val();
    }
    else if ($("#divProcessCounterBalance #Exempt").prop("checked")) {
        procAfterCounterBal = $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_EXEMPT").val();
    }

    var procAfterCounterBalUsd = null;
    if ($("#divProcessCounterBalanceUsd #RefundUsd").prop("checked")) {
        procAfterCounterBalUsd = $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_REFUND").val();
    }
    else if ($("#divProcessCounterBalanceUsd #ReceiveAsRevenueUsd").prop("checked")) {
        procAfterCounterBalUsd = $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_RECEIVE").val();
    }
    else if ($("#divProcessCounterBalanceUsd #BillUsd").prop("checked")) {
        procAfterCounterBalUsd = $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_BILL").val();
    }
    else if ($("#divProcessCounterBalanceUsd #ExemptUsd").prop("checked")) {
        procAfterCounterBalUsd = $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_EXEMPT").val();
    }

//    var removalRow = null;
//    for (var i = 0; i < gridRemovalFeeBillingCTS110.getRowsNum(); i++) {

//        var row_id = gridRemovalFeeBillingCTS110.getRowId(i);
//        var rdoSelect = GenerateGridControlID("rdoSelect", row_id);

//        if ($("#" + rdoSelect).prop("checked")) {
//            var seqCol = gridRemovalFeeBillingCTS110.getColIndexById("Sequence");
//            removalRow = gridRemovalFeeBillingCTS110.cells2(i, seqCol).getValue();
//        }
//    }

    var removalRow_id = gridRemovalInstallFeeCTS110.getRowId(0); //Add by Jutarat A. on 17072012
    var removalAmountCtrl = GenerateGridControlID("Amount", removalRow_id); //Add by Jutarat A. on 17072012

    var removalRow = GetRemovalRowSequence();

    var obj = {
        ChangeType: changeTypeVal,
        ChangeImplementDate: $("#dpCancelDate").val(),
        StopCancelReasonType: $("#ddlCancelReason").val(),
        ApproveNo1: $("#txtApproveNo1").val(),
        ApproveNo2: $("#txtApproveNo2").val(),

        ProcessAfterCounterBalanceType: procAfterCounterBal,
        ProcessAfterCounterBalanceTypeUsd: procAfterCounterBalUsd,

        OtherRemarks: $("#OtherRemarks").val(),

        //RemovalAmount: gridRemovalInstallFeeCTS110.cells2(0, 1).getValue(),
        RemovalAmount: $("#" + removalAmountCtrl).NumericValue(), //Modify by Jutarat A. on 17072012
        RemovalAmountCurrencyType: $("#" + removalAmountCtrl).NumericCurrencyValue(),

        PaymentMethod: $("#PaymentMethod").val(),
        RemovalRowSequence: removalRow
    };

    call_ajax_method_json("/Contract/CTS110_ConfirmRegisterCancelData", obj,
        function (result) {
            if (result != undefined) {
                OpenInformationMessageDialog(result.Code, result.Message,
                    function () {
                        window.location.href = generate_url("/Common/CMS020");
                    }
                );
            }
        }
    );

    //command_control.CommandControlMode(true);
}

function GetRemovalRowSequence() {
    var removalRow = null;
    for (var i = 0; i < gridRemovalFeeBillingCTS110.getRowsNum(); i++) {

        var row_id = gridRemovalFeeBillingCTS110.getRowId(i);
        var rdoSelect = GenerateGridControlID("rdoSelect", row_id);

        if ($("#" + rdoSelect).prop("checked")) {
            var seqCol = gridRemovalFeeBillingCTS110.getColIndexById("Sequence");
            removalRow = gridRemovalFeeBillingCTS110.cells2(i, seqCol).getValue();
        }
    }

    return removalRow;
}

function command_back_click() {
    SetContractBasicInfoMode(false);
    SetCancelContractMode(false);
    SetCancelContractConditionMode(false);
    SetBillingTargetSectionMode(false);

    RegisterCommandControl();
}

function free_type_change() {
    $("#txtTax").attr("readonly", false);
    $("#txtTax").NumericCurrency().attr("disabled", false);
    $("#txtNormalFee").attr("readonly", false);
    $("#txtNormalFee").NumericCurrency().attr("disabled", false);

    $("#txtContractCodeForSlideFee").attr("readonly", false);
    $("#dpPeriodFrom").attr("readonly", false);
    $("#dpPeriodTo").attr("readonly", false);

    if ($("#ddlFeeType").val() == $("#C_BILLING_TYPE_CONTRACT_FEE").val()
        || $("#ddlFeeType").val() == $("#C_BILLING_TYPE_MAINTENANCE_FEE").val()
        || $("#ddlFeeType").val() == $("#C_BILLING_TYPE_OTHER_FEE").val()) {

        $("#txtNormalFee").attr("readonly", true);
        $("#txtNormalFee").NumericCurrency().attr("disabled", true);

        $("#txtNormalFee").clearForm();
    }
    else if ($("#ddlFeeType").val() == $("#C_BILLING_TYPE_DEPOSIT_FEE").val()) {
        $("#txtNormalFee").attr("readonly", true);
        $("#txtNormalFee").NumericCurrency().attr("disabled", true);

        $("#dpPeriodFrom").attr("readonly", true);
        $("#dpPeriodTo").attr("readonly", true);

        $("#txtNormalFee").clearForm();
        $("#dpPeriodFrom").clearForm();
        $("#dpPeriodTo").clearForm();
    }
    else if ($("#ddlFeeType").val() == $("#C_BILLING_TYPE_REMOVAL_INSTALLATION_FEE").val()) {
        $("#txtContractCodeForSlideFee").attr("readonly", true);
        $("#dpPeriodFrom").attr("readonly", true);
        $("#dpPeriodTo").attr("readonly", true);

        $("#txtContractCodeForSlideFee").clearForm();
        $("#dpPeriodFrom").clearForm();
        $("#dpPeriodTo").clearForm();

        //Modify by Jutarat A. on 19072012
//        if ($("#DefaultRemovalFee").val() > 0) {
//            $("#txtNormalFee").val(SetNumericValue($("#DefaultRemovalFee").val(), 2));
//            $("#txtNormalFee").setComma();
//            $("#txtNormalFee").attr("readonly", true);
//        }
//        else {
//            $("#txtNormalFee").val("");
//            $("#txtNormalFee").attr("readonly", false);
//        }
        if ($("#InstallationType").val() == $("#C_RENTAL_INSTALL_TYPE_REMOVE_ALL").val()
            || $("#InstallationType").val() == $("#C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL").val()) {

            $("#txtNormalFee").SetNumericCurrency($("#DefaultNormalRemovalFeeCurrencyType").val());
            $("#txtNormalFee").val(SetNumericValue($("#DefaultNormalRemovalFee").val(), 2));
            $("#txtNormalFee").setComma();

            $("#txtFee").SetNumericCurrency($("#DefaultOrderRemovalFeeCurrencyType").val());
            $("#txtFee").val(SetNumericValue($("#DefaultOrderRemovalFee").val(), 2));
            $("#txtFee").setComma();

            if ($("#InstallationType").val() == $("#C_RENTAL_INSTALL_TYPE_REMOVE_ALL").val()) {
                $("#txtNormalFee").attr("readonly", true);
                $("#txtNormalFee").NumericCurrency().attr("disabled", true);

            }
            else {
                $("#txtNormalFee").attr("readonly", true);
                $("#txtNormalFee").NumericCurrency().attr("disabled", true);

                $("#txtFee").attr("readonly", true);
                $("#txtFee").NumericCurrency().attr("disabled", true);

                //$("#txtTax").attr("readonly", true);
            }
        }
//        else {
//            $("#txtNormalFee").attr("readonly", true);
//        }
        //End Modify
    }
    else if ($("#ddlFeeType").val() == $("#C_BILLING_TYPE_CANCEL_CONTRACT_FEE").val()) {
        $("#txtTax").attr("readonly", true);
        $("#txtTax").NumericCurrency().attr("disabled", true);
        $("#txtNormalFee").attr("readonly", true);
        $("#txtNormalFee").NumericCurrency().attr("disabled", true);

        $("#txtContractCodeForSlideFee").attr("readonly", true);
        $("#dpPeriodFrom").attr("readonly", true);
        $("#dpPeriodTo").attr("readonly", true);

        $("#txtTax").clearForm();
        $("#txtNormalFee").clearForm();
        $("#txtContractCodeForSlideFee").clearForm();
        $("#dpPeriodFrom").clearForm();
        $("#dpPeriodTo").clearForm();
    }
    else if ($("#ddlFeeType").val() == $("#C_BILLING_TYPE_CHANGE_INSTALLATION_FEE").val()) {
        $("#txtNormalFee").attr("readonly", true);
        $("#txtNormalFee").NumericCurrency().attr("disabled", true);

        $("#txtContractCodeForSlideFee").attr("readonly", true);
        $("#dpPeriodTo").attr("readonly", true);

        $("#txtNormalFee").clearForm();
        $("#txtContractCodeForSlideFee").clearForm();
        $("#dpPeriodTo").clearForm();
    }
    else if ($("#ddlFeeType").val() == $("#C_BILLING_TYPE_CARD_FEE").val()) {
        $("#txtNormalFee").attr("readonly", true);
        $("#txtNormalFee").NumericCurrency().attr("disabled", true);

        $("#txtContractCodeForSlideFee").attr("readonly", true);
        $("#dpPeriodFrom").attr("readonly", true);
        $("#dpPeriodTo").attr("readonly", true);

        $("#txtNormalFee").clearForm();
        $("#txtContractCodeForSlideFee").clearForm();
        $("#dpPeriodFrom").clearForm();
        $("#dpPeriodTo").clearForm();
    }

    ReGenerateHandlingTypeCombo();
}

function ReGenerateHandlingTypeCombo() {
    var obj = { strFeeType: $("#ddlFeeType").val() };
    call_ajax_method_json("/Contract/CTS110_GetHandlingType", obj,
        function (result) {
            if (result != undefined) {
                regenerate_combo("#ddlHandlingType", result);
            }
        }
    );
}

function add_button_click() {
    var object = {
        BillingType: $("#ddlFeeType").val(),
        HandlingType: $("#ddlHandlingType").val(),

        FeeAmountCurrencyType: $("#txtFee").NumericCurrencyValue(),
        FeeAmount: $("#txtFee").NumericValue(),
        TaxAmountCurrencyType: $("#txtTax").NumericCurrencyValue(),
        TaxAmount: $("#txtTax").NumericValue(),

        StartPeriodDate: $("#dpPeriodFrom").val(),
        EndPeriodDate: $("#dpPeriodTo").val(),
        Remark: $("#txtRemark").val(),
        ContractCode_CounterBalance: $("#txtContractCodeForSlideFee").val(),

        NormalFeeAmountCurrencyType: $("#txtNormalFee").NumericCurrencyValue(),
        NormalFeeAmount: $("#txtNormalFee").NumericValue()
    };
    
    call_ajax_method_json("/Contract/CTS110_ValidateAddCancelContractData", object,
        function (result, controls) {
            if (controls != undefined) {
                /* --- Higlight Text --- */
                /* --------------------- */
                VaridateCtrl(["ddlFeeType",
                        "ddlHandlingType",
                        "txtFee",
                        "txtTax",
                        "dpPeriodFrom",
                        "dpPeriodTo",
                        "txtRemark",
                        "txtContractCodeForSlideFee",
                        "txtNormalFee"], controls);

            } else if (result != undefined) {

                call_ajax_method_json("/Contract/CTS110_AddCancelContractData", object,
                    function (result) {
                        if (result != undefined) {
                            /* --- Check Empty Row --- */
                            /* ----------------------- */
                            CheckFirstRowIsEmpty(gridCancelContractCTS110, true);
                            /* ----------------------- */

                            /* --- Add new row --- */
                            /* ------------------- */
                            AddNewRow(gridCancelContractCTS110,
                            [result.FeeType,
                                result.HandlingType,
                                result.Fee,
                                result.Tax,
                                result.Period,
                                result.Remark,
                                "",
                                result.FeeTypeCode,
                                result.HandlingTypeCode,
                                result.Sequence]);
                            /*-------------------*/

                            var removeColinx = gridCancelContractCTS110.getColIndexById('Remove');
                            var row_idx = gridCancelContractCTS110.getRowsNum() - 1;
                            var row_id = gridCancelContractCTS110.getRowId(row_idx);
                            //gridCancelContractCTS110.cells2(row_idx, removeColinx).setValue(GenerateHtmlButton("btnRemove", row_id, "Remove", true));
                            GenerateRemoveButton(gridCancelContractCTS110, "btnRemove", row_id, "Remove", true);

                            /* --- Set Event --- */
                            /* ----------------- */
                            BindGridButtonClickEvent("btnRemove", row_id,
                                function (row_id) {
                                    RemoveDataCancelCond(row_id, row_idx);
                                }
                            );
                            /* ----------------- */

                            gridCancelContractCTS110.setSizes();
                            if (result.FeeTypeCode == $("#C_BILLING_TYPE_REMOVAL_INSTALLATION_FEE").val()
                                && $("#txtFee").NumericValue() > 0
                                && result.HandlingTypeCode == $("#C_HANDLING_TYPE_BILL_UNPAID_FEE").val()) { //Add by Jutarat A. on 17072012

                                if (($("#InstallationType").val() == null)
                                    || ($("#InstallationType").val() != null 
                                        && $("#InstallationType").val() != $("#C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL").val())) { //Add by Jutarat A. on 19072012
                                
                                    //$("#Amount").val($("#txtFee").NumericValue());

                                    BindGridRemovalFeeBilling(true, true);
                                    $("#divBillingTargetSection").show();

                                    var rowAmount = gridRemovalInstallFeeCTS110.getRowId(0);

                                    //gridRemovalInstallFeeCTS110.cells2(0, 1).setValue(GenerateNumericBoxWithUnit("Amount", rowAmount, $("#txtFee").val(), "100px", C_CURRENCY_UNIT, false, false));
                                    //$("#Amount").BindNumericBox(10, 2, 0, 9999999999.99);

                                    //GenerateNumericBoxWithUnit2(gridRemovalInstallFeeCTS110, "Amount", rowAmount, "Amount", $("#txtFee").val(), 10, 2, 0, 9999999999.99, false, "110px", C_CURRENCY_UNIT, false, false);

                                    var fee = SetNumericValue($("#txtFee").val(), 2);
                                    var feeCurrencyType = $("#txtFee").NumericCurrencyValue();
                                    NumericTextBoxWithMultipleCurrency(gridRemovalInstallFeeCTS110, "Amount", rowAmount, "Amount", fee, feeCurrencyType, 12, 2, 0, 999999999999.99, 0, false, false);
                                }
                            }

                            CalculateTotalAmount();
                            InitialInputDataCancelCond();
                        }
                    }
                );

            }
        }
    );
}

function CalculateTotalAmount() {
    //    var handlingTypeCol = gridCancelContractCTS110.getColIndexById("HandlingType");
    //    var feeCol = gridCancelContractCTS110.getColIndexById("Fee");
    //    var taxCol = gridCancelContractCTS110.getColIndexById("Tax");
    //    var handlingTypeCodeCol = gridCancelContractCTS110.getColIndexById("HandlingTypeCode");
    //    var slideAmount = 0;
    //    var refundAmount = 0;
    //    var billingAmount = 0;
    //    var counterBalAmount = 0;

    //    for (var i = 0; i < gridCancelContractCTS110.getRowsNum(); i++) {
    //        var iFee = 0;
    //        var iTax = 0;

    //        var fee = gridCancelContractCTS110.cells2(i, feeCol).getValue();
    //        if (fee != "") {
    //            iFee = parseFloat(fee);
    //        }

    //        var tax = gridCancelContractCTS110.cells2(i, taxCol).getValue();
    //        if (tax != "") {
    //            iTax = parseFloat(tax);
    //        }

    //        var handlingTypeCode = gridCancelContractCTS110.cells2(i, handlingTypeCodeCol).getValue();
    //        if (handlingTypeCode == $("#C_HANDLING_TYPE_SLIDE").val()) {
    //            slideAmount += (iFee + iTax);
    //        }
    //        else if (handlingTypeCode == $("#C_HANDLING_TYPE_REFUND").val()) {
    //            refundAmount += (iFee + iTax);
    //        }
    //        else if (handlingTypeCode == $("#C_HANDLING_TYPE_BILL_UNPAID_FEE").val()) {
    //            billingAmount += (iFee + iTax);
    //        }
    //    }

    //    counterBalAmount = refundAmount - billingAmount;

    //    $("#TotalSlideAmt").val(slideAmount);
    //    $("#TotalReturnAmt").val(refundAmount);
    //    $("#TotalBillingAmt").val(billingAmount);
    //    $("#TotalAmtAfterCounterBalance").val(counterBalAmount);  


    call_ajax_method_json("/Contract/CTS110_CalculateTotalAmount", "",
        function (result) {
            if (result != undefined) {
                $("#TotalSlideAmt").val(result[0]);
                $("#TotalReturnAmt").val(result[1]);
                $("#TotalBillingAmt").val(result[2]);
                $("#TotalAmtAfterCounterBalance").val(result[3]);

                $("#TotalSlideAmtUsd").val(result[4]);
                $("#TotalReturnAmtUsd").val(result[5]);
                $("#TotalBillingAmtUsd").val(result[6]);
                $("#TotalAmtAfterCounterBalanceUsd").val(result[7]);

                SetEnableProcessAfterCounterBal();
            }
        }
    );
}

function SetEnableProcessAfterCounterBal() {
    $("#divProcessCounterBalance input[type='radio']").attr("disabled", false);
    
    var totalAmtAfterCounterBalance = parseInt($("#TotalAmtAfterCounterBalance").NumericValue());
    if (totalAmtAfterCounterBalance > 0) {
        $("#divProcessCounterBalance li.r-type1").hide();
        $("#divProcessCounterBalance li.r-type2").hide();
        $("#divProcessCounterBalance li.r-type3").show();
        $("#divProcessCounterBalance li.r-type4").show();
    }
    else if (totalAmtAfterCounterBalance < 0) {
        $("#divProcessCounterBalance li.r-type1").show();
        $("#divProcessCounterBalance li.r-type2").show();
        $("#divProcessCounterBalance li.r-type3").hide();
        $("#divProcessCounterBalance li.r-type4").hide();
    }
    else {
        $("#divProcessCounterBalance li.r-type1").show();
        $("#divProcessCounterBalance li.r-type2").show();
        $("#divProcessCounterBalance li.r-type3").show();
        $("#divProcessCounterBalance li.r-type4").show();

        $("#divProcessCounterBalance input[type='radio']").attr("disabled", true);
        $("#divProcessCounterBalance").clearForm();
    }

    $("#divProcessCounterBalanceUsd input[type='radio']").attr("disabled", false);

    var totalAmtAfterCounterBalanceUsd = parseInt($("#TotalAmtAfterCounterBalanceUsd").NumericValue());
    if (totalAmtAfterCounterBalanceUsd > 0) {
        $("#divProcessCounterBalanceUsd li.r-type1").hide();
        $("#divProcessCounterBalanceUsd li.r-type2").hide();
        $("#divProcessCounterBalanceUsd li.r-type3").show();
        $("#divProcessCounterBalanceUsd li.r-type4").show();
    }
    else if (totalAmtAfterCounterBalanceUsd < 0) {
        $("#divProcessCounterBalanceUsd li.r-type1").show();
        $("#divProcessCounterBalanceUsd li.r-type2").show();
        $("#divProcessCounterBalanceUsd li.r-type3").hide();
        $("#divProcessCounterBalanceUsd li.r-type4").hide();
    }
    else {
        $("#divProcessCounterBalanceUsd li.r-type1").show();
        $("#divProcessCounterBalanceUsd li.r-type2").show();
        $("#divProcessCounterBalanceUsd li.r-type3").show();
        $("#divProcessCounterBalanceUsd li.r-type4").show();

        $("#divProcessCounterBalanceUsd input[type='radio']").attr("disabled", true);
        $("#divProcessCounterBalanceUsd").clearForm();
    }
}

function cancel_button_click() {
    CloseWarningDialog();
    InitialInputDataCancelCond();
}

function new_button_click() {
    $("#btnNew").attr("disabled", true);

    ClearBillingDetail(true);
    $("#divBillingTargetDetailSection").show();

    SetBillingTargetNewMode();
}

function DisableSectionAfterBindBillingDetail() {
    SetReadonlySpecifyCode(true);
    SetReadonlyCopyName(true);
}

function retrieve_billing_target_button_click() {
    var obj = {
        strBillingTargetCode: $("#BillingTargetCode").val()
    };

    call_ajax_method_json("/Contract/CTS110_RetrieveBillingTargetData", obj,
        function (result, controls) {
            if (controls != undefined) {
                VaridateCtrl(["BillingTargetCode"], controls);
            }
            if (result != undefined) {
                BindBillingDetail(result);

                DisableSectionAfterBindBillingDetail();

                //                if ($("#BillingTargetCode").val() != "") {
                //                    $("#BillingOffice").attr("disabled", true);
                //                }
                //                else {
                //                    $("#BillingOffice").attr("disabled", false);
                //                }
                CheckEnableBillingOffice();
            }
        }
    );
}
function retrieve_billing_client_button_click() {
    var obj = {
        strBillingClientCode: $("#BillingClientCode").val()
    };

    call_ajax_method_json("/Contract/CTS110_RetrieveBillingClientData", obj,
        function (result, controls) {
            if (controls != undefined) {
                VaridateCtrl(["BillingClientCode"], controls);
            }
            if (result != undefined) {
                BindBillingDetail(result);

                DisableSectionAfterBindBillingDetail();

                //                if ($("#BillingTargetCode").val() != "") {
                //                    $("#BillingOffice").attr("disabled", true);
                //                }
                //                else {
                //                    $("#BillingOffice").attr("disabled", false);
                //                }
                CheckEnableBillingOffice();
            }
        }
    );
}

function cancel_billing_button_click() {
    CheckEnableNewBillingTarget();

    CloseWarningDialog();
    ClearBillingDetail(true);

    $("#divBillingTargetDetailSection").hide();

    BindGridRemovalFeeBilling(true, true);
}

function CheckEnableNewBillingTarget() {
    var billingOCC = "";
    for (var i = 0; i < gridRemovalFeeBillingCTS110.getRowsNum(); i++) {
        var row_id = gridRemovalFeeBillingCTS110.getRowId(i);
        var occColinx = gridRemovalFeeBillingCTS110.getColIndexById('BillingOCC');
        billingOCC = gridRemovalFeeBillingCTS110.cells(row_id, occColinx).getValue();
    }

    if (gridRemovalFeeBillingCTS110.getRowsNum() > 0 && billingOCC == "") {
        $("#btnNew").attr("disabled", true);
    }
    else {
        $("#btnNew").attr("disabled", false);
    }
}

function ClearBillingDetail(isClearBillingOffice) {
//    $("#BillingTargetCodeDetail").val("");
//    $("#BillingClientCodeDetail").val("");
//    $("#FullNameEN").val("");
//    $("#BranchNameEN").val("");
//    $("#AddressEN").val("");
//    $("#FullNameLC").val("");
//    $("#BranchNameLC").val("");
//    $("#AddressLC").val("");
//    $("#Nationality").val("");
//    $("#PhoneNo").val("");
//    $("#BusinessType").val("");
//    $("#IDNo").val("");

//    $("#BillingTargetCode").val("");
//    $("#BillingClientCode").val("");
//    $("#rdoTargetCode").attr('checked', true)
//    $("#rdoContractTarget").attr('checked', true)

//    if ($("#BillingOffice").prop("disabled") == false) {
//        $("#BillingOffice").val("");
//        $("#BillingOffice").removeClass("highlight");
//    }

    CloseWarningDialog();

    $("#divSpecifyCodeAndCopyName").clearForm();
    $("#divBillingTargetDetail").clearForm();
    $("#BillingOffice").removeClass("highlight");

    if (isClearBillingOffice) {
        $("#BillingOffice").val("");
    }

    $("#rdoTargetCode").attr('checked', true)
    $("#rdoContractTarget").attr('checked', true)
}

function target_code_click() {
    $("#BillingTargetCode").attr("readonly", false);
    $("#BillingTargetCode").ResetToNormalControl();

    $("#BillingClientCode").attr("readonly", true);
    $("#BillingClientCode").val("");

    $("#btnRetrieveBillingTarget").removeAttr("disabled");
    $("#btnRetrieveBillingClient").attr("disabled", "disabled");
}

function client_code_click() {
    $("#BillingClientCode").attr("readonly", false);
    $("#BillingClientCode").ResetToNormalControl();

    $("#BillingTargetCode").attr("readonly", true);
    $("#BillingTargetCode").val("");

    $("#btnRetrieveBillingTarget").attr("disabled", "disabled");
    $("#btnRetrieveBillingClient").removeAttr("disabled");
}

function CMS270Response(result) {
    var obj = { billingClientData: result };
    $("#dlgCTS110").CloseDialog();

    call_ajax_method_json("/Contract/CTS110_SearchBillingClient", obj,
        function (result, controls) {
            if (result != undefined) {
                BindBillingDetail(result);
                DisableSectionAfterBindBillingDetail();
            }
        }
    );

}

function copy_button_click() {
    var copyType = "0";
    if ($("#rdoContractTarget").prop("checked")) {
        copyType = $("#rdoContractTarget").val();
    }
    else if ($("#rdoBranchOfContractTarget").prop("checked")) {
        copyType = $("#rdoBranchOfContractTarget").val();
    }
    else if ($("#rdoRealCustomer").prop("checked")) {
        copyType = $("#rdoRealCustomer").val();
    }
    else if ($("#rdoSite").prop("checked")) {
        copyType = $("#rdoSite").val();
    }

    var obj = { strCopyType: copyType }

    call_ajax_method_json("/Contract/CTS110_CopyBillingTarget", obj,
        function (result, controls) {
            if (result != undefined) {
                BindBillingDetail(result);
                DisableSectionAfterBindBillingDetail();
            }
        }
    );
}

/*------ MAS030 Dialog ------*/
/*---------------------------*/
var doBillingCustomerObject = null;
function new_edit_button_click() {
    call_ajax_method_json("/Contract/CTS110_GetTempBillingClientData", "",
        function (result) {
            //if (result != undefined) {
                doBillingCustomerObject = result;
            //}
            $("#dlgCTS110").OpenMAS030Dialog("CTS110");
        });
}
function MAS030Object() {
    return doBillingCustomerObject;
}
function MAS030Response(result) {
    var obj = { billingClientData: result };
    $("#dlgCTS110").CloseDialog();

    call_ajax_method_json("/Contract/CTS110_SearchBillingClient", obj,
        function (result, controls) {
            if (result != undefined) {
                BindBillingDetail(result);

                //                SetReadonlySpecifyCode(false);
                //                SetReadonlyCopyName(false);
                //                $("#BillingOffice").attr("disabled", false);
                //                $("#btnAddUpdate").attr("disabled", false);

                if (result.BillingClientCodeDetail == null || result.BillingClientCodeDetail == "") {
                    $("#BillingClientCodeDetail").val("");
                    //$("#BillingOffice").attr("disabled", false); //Move to CheckEnableBillingOffice

                    if (result.BillingTargetCodeDetail != null && result.BillingTargetCodeDetail != "") {
                        $("#BillingTargetCodeDetail").val("");
                        $("#BillingOffice").val("");
                    }
                }

                CheckEnableBillingOffice();
            }
        }
    );
}
/*--------------------------*/

function CheckEnableBillingOffice() {
    $("#BillingOffice").attr("disabled", $("#BillingTargetCodeDetail").val() != "");
}

function clear_bill_target_button_click() {
    call_ajax_method_json("/Contract/CTS110_ClearBillingTarget", "",
        function (result, controls) {
            if (result != undefined) {
                ClearBillingDetail(false);

                SetReadonlySpecifyCode(false);
                SetReadonlyCopyName(false);
                //$("#btnClearBillingTarget").attr("disabled", true);
                $("#BillingClientCode").attr("readonly", true); //default

                CheckEnableBillingOffice();
            }
        }
    );
}

function add_update_button_click() {
    var obj = { strOperationMode: mode,
        strBillingOffice: $("#BillingOffice").val()
    };

    call_ajax_method_json("/Contract/CTS110_AddUpdateBillingTargetDetail", obj,
        function (result, controls) {
            if (controls != undefined) {
                VaridateCtrl(["BillingOffice"], controls);
            }
            else if (result != undefined) {
                //                gridRemovalFeeBillingCTS110 = $("#gridRemovalFeeBillingTarget").LoadDataToGridWithInitial(0, false, false, "/Contract/CTS110_RefreshBillingTargetDetail",
                //                                                "", "CTS110_RemovalInstallationFeeGridData", false);

                //                SpecialGridControl(gridRemovalFeeBillingCTS110, ["Select", "Detail", "Remove"]);
                //                BindGridRemovalFeeBilling();

                $("#gridRemovalFeeBillingTarget").LoadDataToGrid(gridRemovalFeeBillingCTS110, 0, false, "/Contract/CTS110_RefreshBillingTargetDetail",
                                                "", "CTS110_RemovalInstallationFeeGridData", false, null, null);

                BindGridRemovalFeeBilling(true, true);

                ClearBillingDetail(true);
                $("#divBillingTargetDetailSection").hide();
            }
        }
    );

}


