/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>

/*--- Main ---*/
var gridCancelContractCTS080;

$(document).ready(function () {
    $("#divLastChangeType").hide();

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
    $("#dpStartOperationDate").InitialDate();
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
    $("#btnRegisterNextDoc").click(register_next_doc_button_click);
}

function SetInitialState(reset) {
    if (reset == true) {
        $("#divCancelContractCondition").clearForm();
    }

    $("#divCondition").clearForm();
    $("#divSecomSignature").clearForm();
    $("#divResultOfRegister").clearForm();

    $("#divResultOfRegister").hide();

    ReGenerateHandlingTypeCombo();

    $("#divBillExemp").attr("disabled", true);
    $("#divRefundReceive").attr("disabled", true);

    //Set maxlenght for TextArea control
    $("#txtRemark").SetMaxLengthTextArea(100);
    $("#OtherRemarks").SetMaxLengthTextArea(4000); //(1600);

    InitialCancelContractConditionGridData();

    RegisterCommandControl();
}

function SetResetState() {
    SetInitialState(true);

    SetContractBasicInfoMode(false);
    SetCancelContractMode(false);
    SetCancelContractConditionMode(false);
}

function InitialCancelContractConditionGridData() {
    gridCancelContractCTS080 = $("#gridCancelContractCondition").LoadDataToGridWithInitial(0, false, false, "/Contract/CTS080_GetCancelContractConditionDetail",
    "", "CTS110_CancelContractConditionGridData", false);

    SpecialGridControl(gridCancelContractCTS080, ["Remove"]);

    BindOnLoadedEvent(gridCancelContractCTS080,
        function (gen_ctrl) {
            gridCancelContractCTS080.setColumnHidden(gridCancelContractCTS080.getColIndexById('FeeTypeCode'), true);
            gridCancelContractCTS080.setColumnHidden(gridCancelContractCTS080.getColIndexById('HandlingTypeCode'), true);
            gridCancelContractCTS080.setColumnHidden(gridCancelContractCTS080.getColIndexById('Sequence'), true);

            for (var i = 0; i < gridCancelContractCTS080.getRowsNum(); i++) {
                var removeColinx = gridCancelContractCTS080.getColIndexById('Remove');
                var row_id = gridCancelContractCTS080.getRowId(i);

                if (gen_ctrl == true) {
                    //gridCancelContractCTS080.cells2(i, removeColinx).setValue(GenerateHtmlButton("btnRemove", row_id, "Remove", true));
                    GenerateRemoveButton(gridCancelContractCTS080, "btnRemove", row_id, "Remove", true);
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

            gridCancelContractCTS080.setSizes();

            call_ajax_method_json("/Contract/CTS080_GetCancelContractCondition", "",
                function (result, controls) {
                    if (result != undefined) {
                        $("#divConditionMemo").bindJSON(result);

                        CalculateTotalAmount();
                    }
                }
            );
        }
    );

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

        if (gridCancelContractCTS080 != undefined) {
            var removeCol = gridCancelContractCTS080.getColIndexById("Remove");
            gridCancelContractCTS080.setColumnHidden(removeCol, true);
            gridCancelContractCTS080.setSizes();

            $("#gridCancelContractCondition").hide();
            $("#gridCancelContractCondition").show();
        }
    }
    else {
        $("#divCancelContractCondition").SetViewMode(false);
        $("#divCondition").show();

        if (gridCancelContractCTS080 != undefined) {
            var removeCol = gridCancelContractCTS080.getColIndexById("Remove");
            gridCancelContractCTS080.setColumnHidden(removeCol, false);
            gridCancelContractCTS080.setSizes();

            $("#gridCancelContractCondition").hide();
            $("#gridCancelContractCondition").LoadDataToGrid(gridCancelContractCTS080, 0, false, "/Contract/CTS080_RefreshCancelContractConditionDetail",
                                                "", "CTS110_CancelContractConditionGridData", false, null, null);
            $("#gridCancelContractCondition").show();
        }
    }
}

function SetResultOfRegisterMode(isview) {
    if (isview) {
        $("#divResultOfRegister").SetViewMode(true);
    }
    else {
        $("#divResultOfRegister").SetViewMode(false);
    }
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
    gridCancelContractCTS080.selectRow(gridCancelContractCTS080.getRowIndex(row_id));

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
                    var feeTypeCol = gridCancelContractCTS080.getColIndexById("FeeTypeCode");
                    var feeType = gridCancelContractCTS080.cells(row_id, feeTypeCol).getValue();

                    var seqCol = gridCancelContractCTS080.getColIndexById("Sequence");
                    var seq = gridCancelContractCTS080.cells(row_id, seqCol).getValue();

                    //Remove the seleted row from cancel contract condition list
                    DeleteRow(gridCancelContractCTS080, row_id);
                    gridCancelContractCTS080.setSizes();

                    var obj = { strSequence: seq };
                    call_ajax_method_json("/Contract/CTS080_RemoveDataCancelCond", obj,
                        function (result) {
                            if (result != undefined) {
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

function command_register_click() {
    command_control.CommandControlMode(false);

    call_ajax_method_json("/Contract/CTS080_RegisterCancelContractData", "",
        function (result, controls) {
            if (result != undefined) {
                /* --- Set View Mode --- */
                /* --------------------- */
                SetContractBasicInfoMode(true);
                SetCancelContractMode(true);
                SetCancelContractConditionMode(true);
                //SetResultOfRegisterMode(true);

                ConfirmCommandControl();
            }
        }
    );

    //command_control.CommandControlMode(true);
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
                    ajax_method.CallScreenControllerWithAuthority("/Contract/CTS080", obj, false, null);
                },
                null);
        }
    );
    /* ------------------- */
}

function command_confirm_click() {
    command_control.CommandControlMode(false);

    /* --- Set Parameter --- */
    var signatureFlag = null;
    if ($("#cnkUseSignaturePicture").prop("checked")) {
        signatureFlag = "1";
    }
    else {
        signatureFlag = "0";
    }

    var obj = {
        StartServiceDate: $("#dpStartOperationDate").val(),
        CancelContractDate: $("#dpCancelDate").val(),
        OtherRemarks: $("#OtherRemarks").val(),
        SECOMSignatureFlag: $("#cnkUseSignaturePicture").prop("checked"),
        EmpName: $("#txtEmployeeName").val(),
        EmpPosition: $("#txtEmployeePosition").val()
    };

    call_ajax_method_json("/Contract/CTS080_ConfirmRegisterCancelData", obj,
        function (result) {
            if (result != undefined) {
                var strDocNoResult = result[1];
                OpenInformationMessageDialog(result[0].Code, result[0].Message,
                    function () {
                        $("#divResultOfRegister").show();
                        $("#txtContractDocumentCode").val(strDocNoResult);
                        HideCommandControl();
                    }
                );
            }
        }
    );

    //command_control.CommandControlMode(true);
}

function command_back_click() {
    SetContractBasicInfoMode(false);
    SetCancelContractMode(false);
    SetCancelContractConditionMode(false);
    //SetResultOfRegisterMode(false);

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
        FeeAmount: $("#txtFee").NumericValue(),
        FeeAmountCurrencyType: $("#txtFee").NumericCurrencyValue(),
        TaxAmount: $("#txtTax").NumericValue(),
        TaxAmountCurrencyType: $("#txtTax").NumericCurrencyValue(),
        StartPeriodDate: $("#dpPeriodFrom").val(),
        EndPeriodDate: $("#dpPeriodTo").val(),
        Remark: $("#txtRemark").val(),
        ContractCode_CounterBalance: $("#txtContractCodeForSlideFee").val(),
        NormalFeeAmount: $("#txtNormalFee").NumericValue(),
        NormalFeeAmountCurrencyType: $("#txtNormalFee").NumericCurrencyValue()
    };

    call_ajax_method_json("/Contract/CTS080_ValidateAddCancelContractData", object,
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

                call_ajax_method_json("/Contract/CTS080_AddCancelContractData", object,
                    function (result) {
                        if (result != undefined) {
                            /* --- Check Empty Row --- */
                            /* ----------------------- */
                            CheckFirstRowIsEmpty(gridCancelContractCTS080, true);
                            /* ----------------------- */

                            /* --- Add new row --- */
                            /* ------------------- */
                            AddNewRow(gridCancelContractCTS080,
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

                            var removeColinx = gridCancelContractCTS080.getColIndexById('Remove');
                            var row_idx = gridCancelContractCTS080.getRowsNum() - 1;
                            var row_id = gridCancelContractCTS080.getRowId(row_idx);
                            //gridCancelContractCTS080.cells2(row_idx, removeColinx).setValue(GenerateHtmlButton("btnRemove", row_id, "Remove", true));
                            GenerateRemoveButton(gridCancelContractCTS080, "btnRemove", row_id, "Remove", true);

                            /* --- Set Event --- */
                            /* ----------------- */
                            BindGridButtonClickEvent("btnRemove", row_id,
                                function (row_id) {
                                    RemoveDataCancelCond(row_id, row_idx);
                                }
                            );
                            /* ----------------- */

                            gridCancelContractCTS080.setSizes();

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
    call_ajax_method_json("/Contract/CTS080_CalculateTotalAmount", "",
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
            }
        }
    );

}

function cancel_button_click() {
    CloseWarningDialog();
    InitialInputDataCancelCond();
}

function register_next_doc_button_click() {
    //SetResetState();
    var obj = { ContractCode: $("#ContractCodeShort").val() };
    ajax_method.CallScreenControllerWithAuthority("/Contract/CTS080", obj, false, null);
}

