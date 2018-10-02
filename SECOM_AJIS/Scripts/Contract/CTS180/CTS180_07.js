/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>

$(document).ready(function () {
    InitialControlCTS180_07();
    InitialEventCTS180_07();
    VisibleDocumentSectionCTS180_07(false);
});

function InitialEventCTS180_07() {
    $("#QC_FeeType").change(free_type_changeCTS180_07);
    $("#QC_Add").click(add_button_clickCTS180_07);
    $("#QC_Cancel").click(cancel_button_clickCTS180_07);

    $("#QC_UseSignaturePicture").click(function () {
        if ($(this).prop("checked") == true) {
            $("#QC_EmployeeName").val("");
            $("#QC_EmployeePosition").val("");

            $("#QC_EmployeeName").attr("disabled", true);
            $("#QC_EmployeePosition").attr("disabled", true);
        }
        else {
            $("#QC_EmployeeName").attr("disabled", false);
            $("#QC_EmployeePosition").attr("disabled", false);
        }
    });
}

function InitialControlCTS180_07() {
    $("#QC_StartServiceDate").InitialDate();
    $("#QC_CancelDate").InitialDate();
    InitialDateFromToControl("#QC_PeriodFrom", "#QC_PeriodTo");

    $("#QC_Fee").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#QC_Tax").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#QC_NormalFee").BindNumericBox(12, 2, 0, 999999999999.99);

    $("#QC_TotalSlideAmt").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#QC_TotalReturnAmt").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#QC_TotalBillingAmt").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#QC_TotalAmtAfterCounterBalance").BindNumericBox(12, 2, 0, 999999999999.99);

    $("#QC_TotalSlideAmtUsd").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#QC_TotalReturnAmtUsd").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#QC_TotalBillingAmtUsd").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#QC_TotalAmtAfterCounterBalanceUsd").BindNumericBox(12, 2, 0, 999999999999.99);

    $("#QC_Remark").SetMaxLengthTextArea(100);
    $("#QC_OtherRemarks").SetMaxLengthTextArea(4000); //(1600);
}

function InitialInputDataCTS180_07() {
    $("#divQuotationCancelContract").clearForm();
    ClearDateFromToControl("#QC_PeriodFrom", "#QC_PeriodTo");

    ReGenerateHandlingTypeComboCTS180_07();

    $("#divBillExemp").attr("disabled", true);
    $("#divRefundReceive").attr("disabled", true);
}

function BindDocumentDataCTS180_07(result) {
    if (result != undefined && result.length == 2) {
        var contDoc = result[0];
        var qoutCanCont = result[1];

        $("#QC_ContractCodeShort").val(contDoc.ContractCodeShort);
        $("#QC_ContractTargetName").val(contDoc.ContractTargetNameEN);
        $("#QC_StartServiceDate").val(ConvertDateToTextFormat(ConvertDateObject(qoutCanCont.StartServiceDate)));
        $("#QC_CancelDate").val(ConvertDateToTextFormat(ConvertDateObject(qoutCanCont.CancelContractDate)));

        //        $("#QC_TotalSlideAmt").val(qoutCanCont.TotalSlideAmt);
        //        $("#QC_TotalReturnAmt").val(qoutCanCont.TotalReturnAmt);
        //        $("#QC_TotalBillingAmt").val(qoutCanCont.TotalBillingAmt);
        //        $("#QC_TotalAmtAfterCounterBalance").val(qoutCanCont.TotalAmtAfterCounterBalance);

        $("#QC_OtherRemarks").val(qoutCanCont.OtherRemarks);


        //$("#QC_UseSignaturePicture").attr("checked", contDoc.SECOMSignatureFlag);
        //$("#QC_EmployeeName").val(contDoc.EmpName);
        //$("#QC_EmployeePosition").val(contDoc.EmpPosition);

        $("#QC_UseSignaturePicture").attr("checked", contDoc.SECOMSignatureFlag);
        if (contDoc.SECOMSignatureFlag == true) {
            $("#QC_EmployeeName").attr("disabled", true);
            $("#QC_EmployeePosition").attr("disabled", true);
        }
        else {
            $("#QC_EmployeeName").val(contDoc.EmpName);
            $("#QC_EmployeePosition").val(contDoc.EmpPosition);
        }

        $("#QC_TotalSlideAmtUsd").SetNumericCurrency(C_CURRENCY_US);
        $("#QC_TotalReturnAmtUsd").SetNumericCurrency(C_CURRENCY_US);
        $("#QC_TotalBillingAmtUsd").SetNumericCurrency(C_CURRENCY_US);
        $("#QC_TotalAmtAfterCounterBalanceUsd").SetNumericCurrency(C_CURRENCY_US);

        InitialQuotationCancelContractGridData();

    }
}

function SetDocumentSectionModeCTS180_07(isView) {
    $("#divQuotationCancelContract").SetViewMode(isView);

    if (isView) {
        $("#divConditionQuotCancel").hide();

        if (gridQuotationCancelContractCTS180 != undefined) {
            var removeCol = gridQuotationCancelContractCTS180.getColIndexById("Remove");
            gridQuotationCancelContractCTS180.setColumnHidden(removeCol, true);
            gridQuotationCancelContractCTS180.setSizes();

            $("#gridQuotationCancelContract").hide();
            $("#gridQuotationCancelContract").show();
        }
    }
    else {
        $("#divConditionQuotCancel").show();

        if (gridQuotationCancelContractCTS180 != undefined) {
            var removeCol = gridQuotationCancelContractCTS180.getColIndexById("Remove");
            gridQuotationCancelContractCTS180.setColumnHidden(removeCol, false);
            gridQuotationCancelContractCTS180.setSizes();

            $("#gridQuotationCancelContract").hide();
            $("#gridQuotationCancelContract").LoadDataToGrid(gridQuotationCancelContractCTS180, 0, false, "/Contract/CTS180_RefreshCancelContractMemoDetail",
                                                "", "CTS110_CancelContractConditionGridData", false, null, null);
            $("#gridQuotationCancelContract").show();
        }
    }
}

function VisibleDocumentSectionCTS180_07(isVisible) {
    if (isVisible) {
        $("#divQuotationCancelContract").show();
    }
    else {
        $("#divQuotationCancelContract").hide();
    }
}

function SetRegisterStateCTS180_07() {
    $("#QC_ContractCodeShort").attr("readonly", true);
    $("#QC_ContractTargetName").attr("readonly", true);

    $("#QC_TotalSlideAmt").attr("readonly", true);
    $("#QC_TotalSlideAmt").NumericCurrency().attr("disabled", true);
    $("#QC_TotalReturnAmt").attr("readonly", true);
    $("#QC_TotalReturnAmt").NumericCurrency().attr("disabled", true);
    $("#QC_TotalBillingAmt").attr("readonly", true);
    $("#QC_TotalBillingAmt").NumericCurrency().attr("disabled", true);
    $("#QC_TotalAmtAfterCounterBalance").attr("readonly", true);
    $("#QC_TotalAmtAfterCounterBalance").NumericCurrency().attr("disabled", true);

    $("#QC_TotalSlideAmtUsd").attr("readonly", true);
    $("#QC_TotalSlideAmtUsd").NumericCurrency().attr("disabled", true);
    $("#QC_TotalReturnAmtUsd").attr("readonly", true);
    $("#QC_TotalReturnAmtUsd").NumericCurrency().attr("disabled", true);
    $("#QC_TotalBillingAmtUsd").attr("readonly", true);
    $("#QC_TotalBillingAmtUsd").NumericCurrency().attr("disabled", true);
    $("#QC_TotalAmtAfterCounterBalanceUsd").attr("readonly", true);
    $("#QC_TotalAmtAfterCounterBalanceUsd").NumericCurrency().attr("disabled", true);
}

function free_type_changeCTS180_07() {
    $("#QC_Tax").attr("readonly", false);
    $("#QC_Tax").NumericCurrency().attr("disabled", false);
    $("#QC_NormalFee").attr("readonly", false);
    $("#QC_NormalFee").NumericCurrency().attr("disabled", false);

    $("#QC_ContractCodeForSlideFee").attr("readonly", false);
    $("#QC_PeriodFrom").attr("readonly", false);
    $("#QC_PeriodTo").attr("readonly", false);

    if ($("#QC_FeeType").val() == $("#C_BILLING_TYPE_CONTRACT_FEE").val()
        || $("#QC_FeeType").val() == $("#C_BILLING_TYPE_MAINTENANCE_FEE").val()
        || $("#QC_FeeType").val() == $("#C_BILLING_TYPE_OTHER_FEE").val()) {

        $("#QC_NormalFee").attr("readonly", true);
        $("#QC_NormalFee").NumericCurrency().attr("disabled", true);

        $("#QC_NormalFee").clearForm();
    }
    else if ($("#QC_FeeType").val() == $("#C_BILLING_TYPE_DEPOSIT_FEE").val()) {
        $("#QC_NormalFee").attr("readonly", true);
        $("#QC_NormalFee").NumericCurrency().attr("disabled", true);

        $("#QC_PeriodFrom").attr("readonly", true);
        $("#QC_PeriodTo").attr("readonly", true);

        $("#QC_NormalFee").clearForm();
        $("#QC_PeriodFrom").clearForm();
        $("#QC_PeriodTo").clearForm();
    }
    else if ($("#QC_FeeType").val() == $("#C_BILLING_TYPE_REMOVAL_INSTALLATION_FEE").val()) {
        $("#QC_ContractCodeForSlideFee").attr("readonly", true);
        $("#QC_PeriodFrom").attr("readonly", true);
        $("#QC_PeriodTo").attr("readonly", true);

        $("#QC_ContractCodeForSlideFee").clearForm();
        $("#QC_PeriodFrom").clearForm();
        $("#QC_PeriodTo").clearForm();
    }
    else if ($("#QC_FeeType").val() == $("#C_BILLING_TYPE_CANCEL_CONTRACT_FEE").val()) {
        $("#QC_Tax").attr("readonly", true);
        $("#QC_Tax").NumericCurrency().attr("disabled", true);

        $("#QC_NormalFee").attr("readonly", true);
        $("#QC_NormalFee").NumericCurrency().attr("disabled", true);

        $("#QC_ContractCodeForSlideFee").attr("readonly", true);
        $("#QC_PeriodFrom").attr("readonly", true);
        $("#QC_PeriodTo").attr("readonly", true);

        $("#QC_Tax").clearForm();
        $("#QC_NormalFee").clearForm();
        $("#QC_ContractCodeForSlideFee").clearForm();
        $("#QC_PeriodFrom").clearForm();
        $("#QC_PeriodTo").clearForm();
    }
    else if ($("#QC_FeeType").val() == $("#C_BILLING_TYPE_CHANGE_INSTALLATION_FEE").val()) {
        $("#QC_NormalFee").attr("readonly", true);
        $("#QC_NormalFee").NumericCurrency().attr("disabled", true);
        
        $("#QC_ContractCodeForSlideFee").attr("readonly", true);
        $("#QC_PeriodTo").attr("readonly", true);

        $("#QC_NormalFee").clearForm();
        $("#QC_ContractCodeForSlideFee").clearForm();
        $("#QC_PeriodTo").clearForm();
    }
    else if ($("#QC_FeeType").val() == $("#C_BILLING_TYPE_CARD_FEE").val()) {
        $("#QC_NormalFee").attr("readonly", true);
        $("#QC_NormalFee").NumericCurrency().attr("disabled", true);

        $("#QC_ContractCodeForSlideFee").attr("readonly", true);
        $("#QC_PeriodFrom").attr("readonly", true);
        $("#QC_PeriodTo").attr("readonly", true);

        $("#QC_NormalFee").clearForm();
        $("#QC_ContractCodeForSlideFee").clearForm();
        $("#QC_PeriodFrom").clearForm();
        $("#QC_PeriodTo").clearForm();
    }

    ReGenerateHandlingTypeComboCTS180_07();
}

function ReGenerateHandlingTypeComboCTS180_07() {
    var obj = { strFeeType: $("#QC_FeeType").val() };
    call_ajax_method_json("/Contract/CTS110_GetHandlingType", obj,
        function (result) {
            if (result != undefined) {
                regenerate_combo("#QC_HandlingType", result);
            }
        }
    );
}

function InitialQuotationCancelContractGridData() {

    gridQuotationCancelContractCTS180 = $("#gridQuotationCancelContract").LoadDataToGridWithInitial(0, false, false, "/Contract/CTS180_GetCancelContractMemoDetail",
    "", "CTS110_CancelContractConditionGridData", false);

    SpecialGridControl(gridQuotationCancelContractCTS180, ["Remove"]);

    BindOnLoadedEvent(gridQuotationCancelContractCTS180,
        function (gen_ctrl) {
            gridQuotationCancelContractCTS180.setColumnHidden(gridQuotationCancelContractCTS180.getColIndexById('FeeTypeCode'), true);
            gridQuotationCancelContractCTS180.setColumnHidden(gridQuotationCancelContractCTS180.getColIndexById('HandlingTypeCode'), true);
            gridQuotationCancelContractCTS180.setColumnHidden(gridQuotationCancelContractCTS180.getColIndexById('Sequence'), true);

            for (var i = 0; i < gridQuotationCancelContractCTS180.getRowsNum(); i++) {
                var removeColinx = gridQuotationCancelContractCTS180.getColIndexById('Remove');
                var row_id = gridQuotationCancelContractCTS180.getRowId(i);

                if (gen_ctrl == true) {
                    GenerateRemoveButton(gridQuotationCancelContractCTS180, "btnRemove", row_id, "Remove", true);
                }

                /* --- Set Event --- */
                /* ----------------- */
                BindGridButtonClickEvent("btnRemove", row_id,
                    function (row_id) {
                        RemoveDataCancelCondCTS180_07(row_id);
                    }
                );
                /* ----------------- */
            }

            gridQuotationCancelContractCTS180.setSizes();
            CalculateTotalAmountCTS180_07();
        }
    );

}

function CalculateTotalAmountCTS180_07() {
    call_ajax_method_json("/Contract/CTS180_CalculateTotalAmount", "",
        function (result) {
            if (result != undefined) {
                $("#QC_TotalSlideAmt").val(result[0]);
                $("#QC_TotalReturnAmt").val(result[1]);
                $("#QC_TotalBillingAmt").val(result[2]);
                $("#QC_TotalAmtAfterCounterBalance").val(result[3]);

                $("#QC_TotalSlideAmtUsd").val(result[4]);
                $("#QC_TotalReturnAmtUsd").val(result[5]);
                $("#QC_TotalBillingAmtUsd").val(result[6]);
                $("#QC_TotalAmtAfterCounterBalanceUsd").val(result[7]);
            }
        }
    );

}

function cancel_button_clickCTS180_07() {
//    InitialInputDataCTS180_07();

//    $("#QC_Fee").attr("readonly", false);
//    $("#QC_Tax").attr("readonly", false);
//    $("#QC_NormalFee").attr("readonly", false);
//    $("#QC_ContractCodeForSlideFee").attr("readonly", false);
//    $("#QC_PeriodFrom").attr("readonly", false);
//    $("#QC_PeriodTo").attr("readonly", false);
//    $("#QC_Remark").attr("readonly", false);

    CloseWarningDialog();
    ClearCancelCondCTS180_07();
}

function add_button_clickCTS180_07() {
    var object = {
        CancelContractType: "QC",
        BillingType: $("#QC_FeeType").val(),
        HandlingType: $("#QC_HandlingType").val(),

        FeeAmountCurrencyType: $("#QC_Fee").NumericCurrencyValue(),
        FeeAmount: $("#QC_Fee").NumericValue(),

        TaxAmountCurrencyType: $("#QC_Tax").NumericCurrencyValue(),
        TaxAmount: $("#QC_Tax").NumericValue(),

        StartPeriodDate: $("#QC_PeriodFrom").val(),
        EndPeriodDate: $("#QC_PeriodTo").val(),
        Remark: $("#QC_Remark").val(),
        ContractCode_CounterBalance: $("#QC_ContractCodeForSlideFee").val(),

        NormalFeeAmountCurrencyType: $("#QC_NormalFee").NumericCurrencyValue(),
        NormalFeeAmount: $("#QC_NormalFee").NumericValue()
    };

    call_ajax_method_json("/Contract/CTS180_ValidateAddCancelContractData", object,
    function (result, controls) {
        if (controls != undefined) {
            /* --- Higlight Text --- */
            /* --------------------- */
            VaridateCtrl(["QC_FeeType",
                    "QC_HandlingType",
                    "QC_Fee",
                    "QC_Tax",
                    "QC_PeriodFrom",
                    "QC_PeriodTo",
                    "QC_Remark",
                    "QC_ContractCodeForSlideFee",
                    "QC_NormalFee"], controls);

        } else if (result != undefined) {

            call_ajax_method_json("/Contract/CTS180_AddCancelContractData", object,
                function (result) {
                    if (result != undefined) {
                        /* --- Check Empty Row --- */
                        /* ----------------------- */
                        CheckFirstRowIsEmpty(gridQuotationCancelContractCTS180, true);
                        /* ----------------------- */

                        /* --- Add new row --- */
                        /* ------------------- */
                        AddNewRow(gridQuotationCancelContractCTS180,
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

                        var removeColinx = gridQuotationCancelContractCTS180.getColIndexById('Remove');
                        var row_idx = gridQuotationCancelContractCTS180.getRowsNum() - 1;
                        var row_id = gridQuotationCancelContractCTS180.getRowId(row_idx);

                        GenerateRemoveButton(gridQuotationCancelContractCTS180, "btnRemove", row_id, "Remove", true);

                        /* --- Set Event --- */
                        /* ----------------- */
                        BindGridButtonClickEvent("btnRemove", row_id,
                            function (row_id) {
                                RemoveDataCancelCondCTS180_07(row_id, row_idx);
                            }
                        );
                        /* ----------------- */

                        gridQuotationCancelContractCTS180.setSizes();

                        CalculateTotalAmountCTS180_07();

                        //$("#divConditionQuotCancel").clearForm(); //InitialInputDataCTS180_07();
                        ClearCancelCondCTS180_07();
                    }
                }
            );

        }
    });
}

function ClearCancelCondCTS180_07() {
    $("#divConditionQuotCancel").clearForm();
    ClearDateFromToControl("#QC_PeriodFrom", "#QC_PeriodTo");

    $("#QC_Fee").attr("readonly", false);
    $("#QC_Fee").NumericCurrency().attr("disabled", false);

    $("#QC_Tax").attr("readonly", false);
    $("#QC_Tax").NumericCurrency().attr("disabled", false);

    $("#QC_NormalFee").attr("readonly", false);
    $("#QC_NormalFee").NumericCurrency().attr("disabled", false);

    $("#QC_ContractCodeForSlideFee").attr("readonly", false);
    $("#QC_PeriodFrom").attr("readonly", false);
    $("#QC_PeriodTo").attr("readonly", false);
    $("#QC_Remark").attr("readonly", false);
}

function RemoveDataCancelCondCTS180_07(row_id) {
    gridQuotationCancelContractCTS180.selectRow(gridQuotationCancelContractCTS180.getRowIndex(row_id));

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
                    var feeTypeCol = gridQuotationCancelContractCTS180.getColIndexById("FeeTypeCode");
                    var feeType = gridQuotationCancelContractCTS180.cells(row_id, feeTypeCol).getValue();

                    var seqCol = gridQuotationCancelContractCTS180.getColIndexById("Sequence");
                    var seq = gridQuotationCancelContractCTS180.cells(row_id, seqCol).getValue();

                    //Remove the seleted row from cancel contract condition list
                    DeleteRow(gridQuotationCancelContractCTS180, row_id);
                    gridQuotationCancelContractCTS180.setSizes();

                    var obj = { strSequence: seq };
                    call_ajax_method_json("/Contract/CTS180_RemoveDataCancelCond", obj,
                        function (result) {
                            if (result != undefined) {
                                CalculateTotalAmountCTS180_07();
                            }
                        }
                    );
                },
                null);
        }
    );
    /* ------------------- */
}

function GetDocumentDataCTS180_07() {
    var objCTS180_07 = {
        ContractCodeShort: $("#QC_ContractCodeShort").val(),
        ContractTargetNameLC: $("#QC_ContractTargetName").val(),
        StartServiceDate: $("#QC_StartServiceDate").val(),
        CancelContractDate: $("#QC_CancelDate").val(),
        OtherRemarks: $("#QC_OtherRemarks").val(),
        SECOMSignatureFlag: $("#QC_UseSignaturePicture").prop("checked"),
        EmpName: $("#QC_EmployeeName").val(),
        EmpPosition: $("#QC_EmployeePosition").val()
    };

    return objCTS180_07;
}