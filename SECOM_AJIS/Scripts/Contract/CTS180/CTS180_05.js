/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>

$(document).ready(function () {
    InitialControlCTS180_05();
    InitialEventCTS180_05();
    VisibleDocumentSectionCTS180_05(false);
});

function InitialEventCTS180_05() {
    $("#CC_FeeType").change(free_type_changeCTS180_05);
    $("#CC_Add").click(add_button_clickCTS180_05);
    $("#CC_Cancel").click(cancel_button_clickCTS180_05);

    //Add by Jutarat A. on 29082012
    //$("#CC_Refund").click(function () {
    //    DisabledTransferBilling(false, true);
    //    ClearTransferBilling();
    //});
    //$("#CC_ReceiveAsRevenue").click(function () {
    //    DisabledTransferBilling(false, true);
    //    ClearTransferBilling();
    //});
    //$("#CC_Bill").click(function () {
    //    DisabledTransferBilling(false, false);
    //});
    //$("#CC_Exempt").click(function () {
    //    DisabledTransferBilling(true, false);
    //    ClearTransferBilling();
    //});
    //End Add

    //$("#CC_AutoNone").click(function () { $("#CC_AutoTransferBillingAmount").attr("readonly", true); ClearTransferBillingAmount(); });
    //$("#CC_AutoAll").click(function () { $("#CC_AutoTransferBillingAmount").attr("readonly", true); ClearTransferBillingAmount(); });
    //$("#CC_AutoPartial").click(function () { $("#CC_AutoTransferBillingAmount").attr("readonly", false); });

    $("#divProcessCounterBalance #CC_Refund").click(function () {
        if ($(this).prop("checked") == true) {
            $("#CC_BankNone").attr('disabled', true);
            $("#CC_BankAll").attr('disabled', true);
            $("#CC_BankPartial").attr('disabled', true);

            $("#CC_BankTransferBillingAmount").attr("readonly", true);
            $("#CC_BankTransferBillingAmount").NumericCurrency().attr("disabled", true);
        }
    });
    $("#divProcessCounterBalance #CC_ReceiveAsRevenue").click(function () {
        if ($(this).prop("checked") == true) {
            $("#CC_BankNone").attr('disabled', true);
            $("#CC_BankAll").attr('disabled', true);
            $("#CC_BankPartial").attr('disabled', true);

            $("#CC_BankTransferBillingAmount").attr("readonly", true);
            $("#CC_BankTransferBillingAmount").NumericCurrency().attr("disabled", true);
        }
    });
    $("#divProcessCounterBalance #CC_Bill").click(function () {
        if ($(this).prop("checked") == true) {
            $("#CC_BankNone").attr('disabled', false);
            $("#CC_BankAll").attr('disabled', false);
            $("#CC_BankPartial").attr('disabled', false);

            if ($("#CC_BankPartial").prop("checked") == true) {
                $("#CC_BankTransferBillingAmount").attr("readonly", false);
                $("#CC_BankTransferBillingAmount").NumericCurrency().attr("disabled", false);
            }
            else {
                $("#CC_BankTransferBillingAmount").attr("readonly", true);
                $("#CC_BankTransferBillingAmount").NumericCurrency().attr("disabled", true);
            }
        }
    });
    $("#divProcessCounterBalance #CC_Exempt").click(function () {
        if ($(this).prop("checked") == true) {
            $("#CC_BankNone").attr('disabled', true);
            $("#CC_BankAll").attr('disabled', true);
            $("#CC_BankPartial").attr('disabled', true);

            $("#CC_BankTransferBillingAmount").attr("readonly", true);
            $("#CC_BankTransferBillingAmount").NumericCurrency().attr("disabled", true);
        }
    });

    $("#divProcessCounterBalanceUsd #CC_RefundUsd").click(function () {
        if ($(this).prop("checked") == true) {
            $("#CC_BankNoneUsd").attr('disabled', true);
            $("#CC_BankAllUsd").attr('disabled', true);
            $("#CC_BankPartialUsd").attr('disabled', true);

            $("#CC_BankTransferBillingAmountUsd").attr("readonly", true);
            $("#CC_BankTransferBillingAmountUsd").NumericCurrency().attr("disabled", true);
        }
    });
    $("#divProcessCounterBalanceUsd #CC_ReceiveAsRevenueUsd").click(function () {
        if ($(this).prop("checked") == true) {
            $("#CC_BankNoneUsd").attr('disabled', true);
            $("#CC_BankAllUsd").attr('disabled', true);
            $("#CC_BankPartialUsd").attr('disabled', true);

            $("#CC_BankTransferBillingAmountUsd").attr("readonly", true);
            $("#CC_BankTransferBillingAmountUsd").NumericCurrency().attr("disabled", true);
        }
    });
    $("#divProcessCounterBalanceUsd #CC_BillUsd").click(function () {
        if ($(this).prop("checked") == true) {
            $("#CC_BankNoneUsd").attr('disabled', false);
            $("#CC_BankAllUsd").attr('disabled', false);
            $("#CC_BankPartialUsd").attr('disabled', false);

            if ($("#CC_BankPartialUsd").prop("checked") == true) {
                $("#CC_BankTransferBillingAmountUsd").attr("readonly", false);
                $("#CC_BankTransferBillingAmountUsd").NumericCurrency().attr("disabled", false);
            }
            else {
                $("#CC_BankTransferBillingAmountUsd").attr("readonly", true);
                $("#CC_BankTransferBillingAmountUsd").NumericCurrency().attr("disabled", true);
            }
        }
    });
    $("#divProcessCounterBalanceUsd #CC_ExemptUsd").click(function () {
        if ($(this).prop("checked") == true) {
            $("#CC_BankNoneUsd").attr('disabled', true);
            $("#CC_BankAllUsd").attr('disabled', true);
            $("#CC_BankPartialUsd").attr('disabled', true);

            $("#CC_BankTransferBillingAmountUsd").attr("readonly", true);
            $("#CC_BankTransferBillingAmountUsd").NumericCurrency().attr("disabled", true);
        }
    });

    $("#CC_BankNone").click(function () {
        $("#CC_BankTransferBillingAmount").attr("readonly", true);
        $("#CC_BankTransferBillingAmount").NumericCurrency().attr("disabled", true);

        ClearTransferBillingAmount();
    });
    $("#CC_BankAll").click(function () {
        $("#CC_BankTransferBillingAmount").attr("readonly", true);
        $("#CC_BankTransferBillingAmount").NumericCurrency().attr("disabled", true);

        ClearTransferBillingAmount();
    });
    $("#CC_BankPartial").click(function () {
        $("#CC_BankTransferBillingAmount").attr("readonly", false);
        $("#CC_BankTransferBillingAmount").NumericCurrency().attr("disabled", false);
    });

    $("#CC_BankNoneUsd").click(function () {
        $("#CC_BankTransferBillingAmountUsd").attr("readonly", true);
        $("#CC_BankTransferBillingAmountUsd").NumericCurrency().attr("disabled", true);

        ClearTransferBillingAmount();
    });
    $("#CC_BankAllUsd").click(function () {
        $("#CC_BankTransferBillingAmountUsd").attr("readonly", true);
        $("#CC_BankTransferBillingAmountUsd").NumericCurrency().attr("disabled", true);

        ClearTransferBillingAmount();
    });
    $("#CC_BankPartialUsd").click(function () {
        $("#CC_BankTransferBillingAmountUsd").attr("readonly", false);
        $("#CC_BankTransferBillingAmountUsd").NumericCurrency().attr("disabled", false);
    });

    $("#CC_UseSignaturePicture").click(function() {
        if ($(this).prop("checked") == true) {
            $("#CC_EmployeeName").val("");
            $("#CC_EmployeePosition").val("");

            $("#CC_EmployeeName").attr("disabled", true);
            $("#CC_EmployeePosition").attr("disabled", true);
        }
        else {
            $("#CC_EmployeeName").attr("disabled", false);
            $("#CC_EmployeePosition").attr("disabled", false);
        }
    });
}

function InitialControlCTS180_05() {
    $("#CC_StartServiceDate").InitialDate();
    $("#CC_CancelDate").InitialDate();
    InitialDateFromToControl("#CC_PeriodFrom", "#CC_PeriodTo");

    $("#CC_Fee").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#CC_Tax").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#CC_NormalFee").BindNumericBox(12, 2, 0, 999999999999.99);

    $("#CC_TotalSlideAmt").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#CC_TotalReturnAmt").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#CC_TotalBillingAmt").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#CC_TotalAmtAfterCounterBalance").BindNumericBox(12, 2, 0, 999999999999.99);

    $("#CC_TotalSlideAmtUsd").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#CC_TotalReturnAmtUsd").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#CC_TotalBillingAmtUsd").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#CC_TotalAmtAfterCounterBalanceUsd").BindNumericBox(12, 2, 0, 999999999999.99);

    //$("#CC_AutoTransferBillingAmount").BindNumericBox(10, 2, 0, 9999999999.99);
    $("#CC_BankTransferBillingAmount").BindNumericBox(12, 2, 0, 999999999999.99);
    $("#CC_BankTransferBillingAmountUsd").BindNumericBox(12, 2, 0, 999999999999.99);

    $("#CC_Remark").SetMaxLengthTextArea(100);
    $("#CC_OtherRemarks").SetMaxLengthTextArea(4000); //(1600);
}

function InitialInputDataCTS180_05() {
    $("#divCancelContractMemo").clearForm();
    ClearDateFromToControl("#CC_PeriodFrom", "#CC_PeriodTo");

    ReGenerateHandlingTypeComboCTS180_05();

    //$("#divBillExemp").attr("disabled", true);
    //$("#divRefundReceive").attr("disabled", true);
    $("#divProcessCounterBalance input[type='radio']").attr("disabled", true);
    $("#divProcessCounterBalanceUsd input[type='radio']").attr("disabled", true);
}

function BindDocumentDataCTS180_05(result) {
    if (result != undefined && result.length == 2) {
        var contDoc = result[0];
        var docCanCont = result[1];

        $("#CC_ContractCodeShort").val(contDoc.ContractCodeShort);
        $("#CC_ContractTargetName").val(contDoc.ContractTargetNameEN);
        $("#CC_StartServiceDate").val(ConvertDateToTextFormat(ConvertDateObject(docCanCont.StartServiceDate)));
        $("#CC_CancelDate").val(ConvertDateToTextFormat(ConvertDateObject(docCanCont.CancelContractDate)));
        $("#CC_RealCustomerName").val(contDoc.RealCustomerNameEN);
        $("#CC_SiteNameEN").val(contDoc.SiteNameEN);
        $("#CC_SiteNameLC").val(contDoc.SiteNameLC);
        $("#CC_SiteAddress").val(contDoc.SiteAddressEN);

        //        $("#CC_TotalSlideAmt").val(docCanCont.TotalSlideAmt);
        //        $("#CC_TotalReturnAmt").val(docCanCont.TotalReturnAmt);
        //        $("#CC_TotalBillingAmt").val(docCanCont.TotalBillingAmt);
        //        $("#CC_TotalAmtAfterCounterBalance").val(docCanCont.TotalAmtAfterCounterBalance);

        //DisabledTransferBilling(true, true); //Add by Jutarat A. on 29082012

        //if (docCanCont.ProcessAfterCounterBalanceTypeUsd == $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_REFUND").val()) {
        //    $("#CC_Refund").attr('checked', true);
        //}
        //else if (docCanCont.ProcessAfterCounterBalanceTypeUsd == $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_RECEIVE").val()) {
        //    $("#CC_ReceiveAsRevenue").attr('checked', true);
        //}
        //else if (docCanCont.ProcessAfterCounterBalanceType == $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_BILL").val()) {
        //    $("#CC_Bill").attr('checked', true);
        //    DisabledTransferBilling(false, true); //Add by Jutarat A. on 29082012
        //}
        //else if (docCanCont.ProcessAfterCounterBalanceType == $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_EXEMPT").val()) {
        //    $("#CC_Exempt").attr('checked', true);
        //}

        if (docCanCont.ProcessAfterCounterBalanceType == $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_REFUND").val()) {
            $("#divProcessCounterBalance #CC_Refund").attr('checked', true);
        }
        else if (docCanCont.ProcessAfterCounterBalanceType == $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_RECEIVE").val()) {
            $("#divProcessCounterBalance #CC_ReceiveAsRevenue").attr('checked', true);
        }
        else if (docCanCont.ProcessAfterCounterBalanceType == $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_BILL").val()) {
            $("#divProcessCounterBalance #CC_Bill").attr('checked', true);
        }
        else if (docCanCont.ProcessAfterCounterBalanceType == $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_EXEMPT").val()) {
            $("#divProcessCounterBalance #CC_Exempt").attr('checked', true);
        }

        if (docCanCont.ProcessAfterCounterBalanceTypeUsd == $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_REFUND").val()) {
            $("#divProcessCounterBalanceUsd #CC_RefundUsd").attr('checked', true);
        }
        else if (docCanCont.ProcessAfterCounterBalanceTypeUsd == $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_RECEIVE").val()) {
            $("#divProcessCounterBalanceUsd #CC_ReceiveAsRevenueUsd").attr('checked', true);
        }
        else if (docCanCont.ProcessAfterCounterBalanceTypeUsd == $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_BILL").val()) {
            $("#divProcessCounterBalanceUsd #CC_BillUsd").attr('checked', true);
        }
        else if (docCanCont.ProcessAfterCounterBalanceTypeUsd == $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_EXEMPT").val()) {
            $("#divProcessCounterBalanceUsd #CC_ExemptUsd").attr('checked', true);
        }


        $("#CC_AutoTransferBillingAmount").attr("readonly", true);
        if (docCanCont.AutoTransferBillingType == $("#C_AUTO_TRANSFER_BILLING_TYPE_NONE").val()) {
            $("#CC_AutoNone").attr('checked', true);
        }
        else if (docCanCont.AutoTransferBillingType == $("#C_AUTO_TRANSFER_BILLING_TYPE_ALL").val()) {
            $("#CC_AutoAll").attr('checked', true);
        }
        else if (docCanCont.AutoTransferBillingType == $("#C_AUTO_TRANSFER_BILLING_TYPE_PARTIAL").val()) {
            $("#CC_AutoPartial").attr('checked', true);

            if ($("#CC_Bill").prop('checked')) { //Add by Jutarat A. on 29082012
                $("#CC_AutoTransferBillingAmount").attr("readonly", false);
            }
        }

        $("#CC_BankTransferBillingAmount").attr("readonly", true);
        $("#CC_BankTransferBillingAmount").NumericCurrency().attr("disabled", true);

        if (docCanCont.BankTransferBillingType == $("#C_BANK_TRANSFER_BILLING_TYPE_NONE").val()) {
            $("#CC_BankNone").attr('checked', true);
        }
        else if (docCanCont.BankTransferBillingType == $("#C_BANK_TRANSFER_BILLING_TYPE_ALL").val()) {
            $("#CC_BankAll").attr('checked', true);
        }
        else if (docCanCont.BankTransferBillingType == $("#C_BANK_TRANSFER_BILLING_TYPE_PARTIAL").val()) {
            $("#CC_BankPartial").attr('checked', true);

            //if ($("#CC_Bill").prop('checked')) { //Add by Jutarat A. on 29082012
            //    $("#CC_BankTransferBillingAmount").attr("readonly", false);
            //    $("#CC_BankTransferBillingAmount").NumericCurrency().attr("disabled", false);
            //}
        }


        $("#CC_BankTransferBillingAmountUsd").attr("readonly", true);
        $("#CC_BankTransferBillingAmountUsd").NumericCurrency().attr("disabled", true);


        if (docCanCont.BankTransferBillingTypeUsd == $("#C_BANK_TRANSFER_BILLING_TYPE_NONE").val()) {
            $("#CC_BankNoneUsd").attr('checked', true);
        }
        else if (docCanCont.BankTransferBillingTypeUsd == $("#C_BANK_TRANSFER_BILLING_TYPE_ALL").val()) {
            $("#CC_BankAllUsd").attr('checked', true);
        }
        else if (docCanCont.BankTransferBillingTypeUsd == $("#C_BANK_TRANSFER_BILLING_TYPE_PARTIAL").val()) {
            $("#CC_BankPartialUsd").attr('checked', true);
        }

        if ($("#divProcessCounterBalance #CC_Bill").prop("checked") == true) {
            $("#CC_BankNone").attr('disabled', false);
            $("#CC_BankAll").attr('disabled', false);
            $("#CC_BankPartial").attr('disabled', false);

            if ($("#CC_BankPartial").prop("checked") == true) {
                $("#CC_BankTransferBillingAmount").attr("readonly", false);
                $("#CC_BankTransferBillingAmount").NumericCurrency().attr("disabled", false);
            }
            else {
                $("#CC_BankTransferBillingAmount").attr("readonly", true);
                $("#CC_BankTransferBillingAmount").NumericCurrency().attr("disabled", true);
            }
        }
        else {
            $("#CC_BankNone").attr('disabled', true);
            $("#CC_BankAll").attr('disabled', true);
            $("#CC_BankPartial").attr('disabled', true);

            $("#CC_BankTransferBillingAmount").attr("readonly", true);
            $("#CC_BankTransferBillingAmount").NumericCurrency().attr("disabled", true);
        }

        if ($("#divProcessCounterBalanceUsd #CC_BillUsd").prop("checked") == true) {
            $("#CC_BankNoneUsd").attr('disabled', false);
            $("#CC_BankAllUsd").attr('disabled', false);
            $("#CC_BankPartialUsd").attr('disabled', false);

            if ($("#CC_BankPartialUsd").prop("checked") == true) {
                $("#CC_BankTransferBillingAmountUsd").attr("readonly", false);
                $("#CC_BankTransferBillingAmountUsd").NumericCurrency().attr("disabled", false);
            }
            else {
                $("#CC_BankTransferBillingAmountUsd").attr("readonly", true);
                $("#CC_BankTransferBillingAmountUsd").NumericCurrency().attr("disabled", true);
            }
        }
        else {
            $("#CC_BankNoneUsd").attr('disabled', true);
            $("#CC_BankAllUsd").attr('disabled', true);
            $("#CC_BankPartialUsd").attr('disabled', true);

            $("#CC_BankTransferBillingAmountUsd").attr("readonly", true);
            $("#CC_BankTransferBillingAmountUsd").NumericCurrency().attr("disabled", true);
        }



        $("#CC_CustomerSignatureName").val(docCanCont.CustomerSignatureName);
        $("#CC_OtherRemarks").val(docCanCont.OtherRemarks);
        //$("#CC_AutoTransferBillingAmount").val(docCanCont.AutoTransferBillingAmtForShow);

        $("#CC_BankTransferBillingAmount").val(docCanCont.BankTransferBillingAmtForShow);
        $("#CC_BankTransferBillingAmountUsd").val(docCanCont.BankTransferBillingAmtUsdForShow);

        $("#CC_UseSignaturePicture").attr("checked", contDoc.SECOMSignatureFlag);
        if (contDoc.SECOMSignatureFlag == true) {
            $("#CC_EmployeeName").attr("disabled", true);
            $("#CC_EmployeePosition").attr("disabled", true);
        }
        else {
            $("#CC_EmployeeName").val(contDoc.EmpName);
            $("#CC_EmployeePosition").val(contDoc.EmpPosition);
        }

        $("#CC_TotalSlideAmtUsd").SetNumericCurrency(C_CURRENCY_US);
        $("#CC_TotalReturnAmtUsd").SetNumericCurrency(C_CURRENCY_US);
        $("#CC_TotalBillingAmtUsd").SetNumericCurrency(C_CURRENCY_US);
        $("#CC_TotalAmtAfterCounterBalanceUsd").SetNumericCurrency(C_CURRENCY_US);
        $("#CC_BankTransferBillingAmountUsd").SetNumericCurrency(C_CURRENCY_US);
        
        InitialCancelContractGridData();

    }
}

//Add by Jutarat A. on 29082012
function DisabledTransferBilling(isDisabled, isDisabledUsd) {
    //$("#CC_AutoNone").attr('disabled', isDisabled);
    //$("#CC_AutoAll").attr('disabled', isDisabled);
    //$("#CC_AutoPartial").attr('disabled', isDisabled);

    $("#CC_BankNone").attr('disabled', isDisabled);
    $("#CC_BankAll").attr('disabled', isDisabled);
    $("#CC_BankPartial").attr('disabled', isDisabled);

    $("#CC_BankNoneUsd").attr('disabled', isDisabledUsd);
    $("#CC_BankAllUsd").attr('disabled', isDisabledUsd);
    $("#CC_BankPartialUsd").attr('disabled', isDisabledUsd);

    //$("#CC_AutoTransferBillingAmount").attr("readonly", true);
    $("#CC_BankTransferBillingAmount").attr("readonly", true);
    $("#CC_BankTransferBillingAmount").NumericCurrency().attr("disabled", true);

    $("#CC_BankTransferBillingAmountUsd").attr("readonly", true);
    $("#CC_BankTransferBillingAmountUsd").NumericCurrency().attr("disabled", true);
}

function ClearTransferBilling() {
    //$("#CC_AutoNone").attr('checked', true);

    $("#CC_BankNone").attr('checked', true);
    $("#CC_BankNoneUsd").attr('checked', true);

    ClearTransferBillingAmount();
}

function ClearTransferBillingAmount() {
    //$("#CC_AutoTransferBillingAmount").val("");
    $("#CC_BankTransferBillingAmount").val("");
    $("#CC_BankTransferBillingAmountUsd").val("");
}
//End Add

function SetDocumentSectionModeCTS180_05(isView) {
    $("#divCancelContractMemo").SetViewMode(isView);

    if (isView) {
        $("#divConditionCancelCont").hide();

        if (gridCancelContractCTS180 != undefined) {
            var removeCol = gridCancelContractCTS180.getColIndexById("Remove");
            gridCancelContractCTS180.setColumnHidden(removeCol, true);
            gridCancelContractCTS180.setSizes();

            $("#gridCancelContractMemo").hide();
            $("#gridCancelContractMemo").show();
        }
    }
    else {
        $("#divConditionCancelCont").show();

        if (gridCancelContractCTS180 != undefined) {
            var removeCol = gridCancelContractCTS180.getColIndexById("Remove");
            gridCancelContractCTS180.setColumnHidden(removeCol, false);
            gridCancelContractCTS180.setSizes();

            $("#gridCancelContractMemo").hide();
            $("#gridCancelContractMemo").LoadDataToGrid(gridCancelContractCTS180, 0, false, "/Contract/CTS180_RefreshCancelContractMemoDetail",
                                                "", "CTS110_CancelContractConditionGridData", false, null, null);
            $("#gridCancelContractMemo").show();
        }

        if ($("#divProcessCounterBalance #CC_Bill").prop("checked") == true) {
            $("#CC_BankNone").attr('disabled', false);
            $("#CC_BankAll").attr('disabled', false);
            $("#CC_BankPartial").attr('disabled', false);

            if ($("#CC_BankPartial").prop("checked") == true) {
                $("#CC_BankTransferBillingAmount").attr("readonly", false);
                $("#CC_BankTransferBillingAmount").NumericCurrency().attr("disabled", false);
            }
            else {
                $("#CC_BankTransferBillingAmount").attr("readonly", true);
                $("#CC_BankTransferBillingAmount").NumericCurrency().attr("disabled", true);
            }
        }
        else {
            $("#CC_BankNone").attr('disabled', true);
            $("#CC_BankAll").attr('disabled', true);
            $("#CC_BankPartial").attr('disabled', true);

            $("#CC_BankTransferBillingAmount").attr("readonly", true);
            $("#CC_BankTransferBillingAmount").NumericCurrency().attr("disabled", true);
        }

        if ($("#divProcessCounterBalanceUsd #CC_BillUsd").prop("checked") == true) {
            $("#CC_BankNoneUsd").attr('disabled', false);
            $("#CC_BankAllUsd").attr('disabled', false);
            $("#CC_BankPartialUsd").attr('disabled', false);

            if ($("#CC_BankPartialUsd").prop("checked") == true) {
                $("#CC_BankTransferBillingAmountUsd").attr("readonly", false);
                $("#CC_BankTransferBillingAmountUsd").NumericCurrency().attr("disabled", false);
            }
            else {
                $("#CC_BankTransferBillingAmountUsd").attr("readonly", true);
                $("#CC_BankTransferBillingAmountUsd").NumericCurrency().attr("disabled", true);
            }
        }
        else {
            $("#CC_BankNoneUsd").attr('disabled', true);
            $("#CC_BankAllUsd").attr('disabled', true);
            $("#CC_BankPartialUsd").attr('disabled', true);

            $("#CC_BankTransferBillingAmountUsd").attr("readonly", true);
            $("#CC_BankTransferBillingAmountUsd").NumericCurrency().attr("disabled", true);
        }
    }
}

function VisibleDocumentSectionCTS180_05(isVisible) {
    if (isVisible) {
        $("#divCancelContractMemo").show();
    }
    else {
        $("#divCancelContractMemo").hide();
    }
}

function SetRegisterStateCTS180_05() {
    $("#CC_ContractCodeShort").attr("readonly", true);
    $("#CC_ContractTargetName").attr("readonly", true);

    //$("#CC_TotalSlideAmt").attr("readonly", true);
    //$("#CC_TotalReturnAmt").attr("readonly", true);
    //$("#CC_TotalBillingAmt").attr("readonly", true);
    //$("#CC_TotalAmtAfterCounterBalance").attr("readonly", true);

    //SetEnableProcessAfterCounterBalCTS180_05();
}

function SetEnableProcessAfterCounterBalCTS180_05() {
    ////$("#divRefundReceive").show();
    //$("#divRefundReceive").attr("disabled", false);

    ////$("#divBillExemp").show();
    //$("#divBillExemp").attr("disabled", false);

    //var totalReturnAmt = parseInt($("#CC_TotalReturnAmt").NumericValue());
    //var totalBillingAmt = parseInt($("#CC_TotalBillingAmt").NumericValue());

    //if (totalReturnAmt > totalBillingAmt) {
    //    //$("#divBillExemp").hide();
    //    $("#divBillExemp").attr("disabled", true);
    //    $("#divBillExemp").clearForm();
    //}
    //else if (totalReturnAmt < totalBillingAmt) {
    //    //$("#divRefundReceive").hide();
    //    $("#divRefundReceive").attr("disabled", true);
    //    $("#divRefundReceive").clearForm();
    //}
    //else {
    //    $("#divBillExemp").attr("disabled", true);
    //    $("#divBillExemp").clearForm();

    //    $("#divRefundReceive").attr("disabled", true);
    //    $("#divRefundReceive").clearForm();
    //}

    $("#divProcessCounterBalance input[type='radio']").attr("disabled", false);

    var totalAmtAfterCounterBalance = parseInt($("#CC_TotalAmtAfterCounterBalance").NumericValue());
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

    var totalAmtAfterCounterBalanceUsd = parseInt($("#CC_TotalAmtAfterCounterBalanceUsd").NumericValue());
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

function free_type_changeCTS180_05() {
    $("#CC_Tax").attr("readonly", false);
    $("#CC_Tax").NumericCurrency().attr("disabled", false);
    $("#CC_NormalFee").attr("readonly", false);
    $("#CC_NormalFee").NumericCurrency().attr("disabled", false);

    $("#CC_ContractCodeForSlideFee").attr("readonly", false);
    $("#CC_PeriodFrom").attr("readonly", false);
    $("#CC_PeriodTo").attr("readonly", false);

    if ($("#CC_FeeType").val() == $("#C_BILLING_TYPE_CONTRACT_FEE").val()
        || $("#CC_FeeType").val() == $("#C_BILLING_TYPE_MAINTENANCE_FEE").val()
        || $("#CC_FeeType").val() == $("#C_BILLING_TYPE_OTHER_FEE").val()) {

        $("#CC_NormalFee").attr("readonly", true);
        $("#CC_NormalFee").NumericCurrency().attr("disabled", true);

        $("#CC_NormalFee").clearForm();
    }
    else if ($("#CC_FeeType").val() == $("#C_BILLING_TYPE_DEPOSIT_FEE").val()) {
        $("#CC_NormalFee").attr("readonly", true);
        $("#CC_NormalFee").NumericCurrency().attr("disabled", true);

        $("#CC_PeriodFrom").attr("readonly", true);
        $("#CC_PeriodTo").attr("readonly", true);

        $("#CC_NormalFee").clearForm();
        $("#CC_PeriodFrom").clearForm();
        $("#CC_PeriodTo").clearForm();
    }
    else if ($("#CC_FeeType").val() == $("#C_BILLING_TYPE_REMOVAL_INSTALLATION_FEE").val()) {
        $("#CC_ContractCodeForSlideFee").attr("readonly", true);

        $("#CC_PeriodFrom").attr("readonly", true);
        $("#CC_PeriodTo").attr("readonly", true);

        $("#CC_ContractCodeForSlideFee").clearForm();
        $("#CC_PeriodFrom").clearForm();
        $("#CC_PeriodTo").clearForm();
    }
    else if ($("#CC_FeeType").val() == $("#C_BILLING_TYPE_CANCEL_CONTRACT_FEE").val()) {
        $("#CC_Tax").attr("readonly", true);
        $("#CC_Tax").NumericCurrency().attr("disabled", true);

        $("#CC_NormalFee").attr("readonly", true);
        $("#CC_NormalFee").NumericCurrency().attr("disabled", true);

        $("#CC_ContractCodeForSlideFee").attr("readonly", true);
        $("#CC_PeriodFrom").attr("readonly", true);
        $("#CC_PeriodTo").attr("readonly", true);

        $("#CC_Tax").clearForm();
        $("#CC_NormalFee").clearForm();
        $("#CC_ContractCodeForSlideFee").clearForm();
        $("#CC_PeriodFrom").clearForm();
        $("#CC_PeriodTo").clearForm();
    }
    else if ($("#CC_FeeType").val() == $("#C_BILLING_TYPE_CHANGE_INSTALLATION_FEE").val()) {
        $("#CC_NormalFee").attr("readonly", true);
        $("#CC_NormalFee").NumericCurrency().attr("disabled", true);

        $("#CC_ContractCodeForSlideFee").attr("readonly", true);
        $("#CC_PeriodTo").attr("readonly", true);

        $("#CC_NormalFee").clearForm();
        $("#CC_ContractCodeForSlideFee").clearForm();
        $("#CC_PeriodTo").clearForm();
    }
    else if ($("#CC_FeeType").val() == $("#C_BILLING_TYPE_CARD_FEE").val()) {
        $("#CC_NormalFee").attr("readonly", true);
        $("#CC_NormalFee").NumericCurrency().attr("disabled", true);

        $("#CC_ContractCodeForSlideFee").attr("readonly", true);
        $("#CC_PeriodFrom").attr("readonly", true);
        $("#CC_PeriodTo").attr("readonly", true);

        $("#CC_NormalFee").clearForm();
        $("#CC_ContractCodeForSlideFee").clearForm();
        $("#CC_PeriodFrom").clearForm();
        $("#CC_PeriodTo").clearForm();
    }

    ReGenerateHandlingTypeComboCTS180_05();
}

function ReGenerateHandlingTypeComboCTS180_05() {
    var obj = { strFeeType: $("#CC_FeeType").val() };
    call_ajax_method_json("/Contract/CTS110_GetHandlingType", obj,
        function (result) {
            if (result != undefined) {
                regenerate_combo("#CC_HandlingType", result);
            }
        }
    );
}

function InitialCancelContractGridData() {

    gridCancelContractCTS180 = $("#gridCancelContractMemo").LoadDataToGridWithInitial(0, false, false, "/Contract/CTS180_GetCancelContractMemoDetail",
    "", "CTS110_CancelContractConditionGridData", false);

    SpecialGridControl(gridCancelContractCTS180, ["Remove"]);

    BindOnLoadedEvent(gridCancelContractCTS180,
        function (gen_ctrl) {
            gridCancelContractCTS180.setColumnHidden(gridCancelContractCTS180.getColIndexById('FeeTypeCode'), true);
            gridCancelContractCTS180.setColumnHidden(gridCancelContractCTS180.getColIndexById('HandlingTypeCode'), true);
            gridCancelContractCTS180.setColumnHidden(gridCancelContractCTS180.getColIndexById('Sequence'), true);

            for (var i = 0; i < gridCancelContractCTS180.getRowsNum(); i++) {
                var removeColinx = gridCancelContractCTS180.getColIndexById('Remove');
                var row_id = gridCancelContractCTS180.getRowId(i);

                if (gen_ctrl == true) {
                    GenerateRemoveButton(gridCancelContractCTS180, "btnRemove", row_id, "Remove", true);
                }

                /* --- Set Event --- */
                /* ----------------- */
                BindGridButtonClickEvent("btnRemove", row_id,
                    function (row_id) {
                        RemoveDataCancelCondCTS180_05(row_id);
                    }
                );
                /* ----------------- */
            }

            gridCancelContractCTS180.setSizes();
            CalculateTotalAmountCTS180_05();
        }
    );

}

function CalculateTotalAmountCTS180_05() {
    call_ajax_method_json("/Contract/CTS180_CalculateTotalAmount", "",
        function (result) {
            if (result != undefined) {
                $("#CC_TotalSlideAmt").val(result[0]);
                $("#CC_TotalReturnAmt").val(result[1]);
                $("#CC_TotalBillingAmt").val(result[2]);
                $("#CC_TotalAmtAfterCounterBalance").val(result[3]);

                $("#CC_TotalSlideAmtUsd").val(result[4]);
                $("#CC_TotalReturnAmtUsd").val(result[5]);
                $("#CC_TotalBillingAmtUsd").val(result[6]);
                $("#CC_TotalAmtAfterCounterBalanceUsd").val(result[7]);

                SetEnableProcessAfterCounterBalCTS180_05();
            }
        }
    );

}

function cancel_button_clickCTS180_05() {
//    InitialInputDataCTS180_05();

//    $("#CC_Fee").attr("readonly", false);
//    $("#CC_Tax").attr("readonly", false);
//    $("#CC_NormalFee").attr("readonly", false);
//    $("#CC_ContractCodeForSlideFee").attr("readonly", false);
//    $("#CC_PeriodFrom").attr("readonly", false);
//    $("#CC_PeriodTo").attr("readonly", false);
//    $("#CC_Remark").attr("readonly", false);

    CloseWarningDialog();
    ClearCancelCondCTS180_05();
}

function add_button_clickCTS180_05() {
    var object = {
        CancelContractType: "CC",
        BillingType: $("#CC_FeeType").val(),
        HandlingType: $("#CC_HandlingType").val(),

        FeeAmountCurrencyType: $("#CC_Fee").NumericCurrencyValue(),
        FeeAmount: $("#CC_Fee").NumericValue(),

        TaxAmountCurrencyType: $("#CC_Tax").NumericCurrencyValue(),
        TaxAmount: $("#CC_Tax").NumericValue(),

        StartPeriodDate: $("#CC_PeriodFrom").val(),
        EndPeriodDate: $("#CC_PeriodTo").val(),
        Remark: $("#CC_Remark").val(),
        ContractCode_CounterBalance: $("#CC_ContractCodeForSlideFee").val(),

        NormalFeeAmountCurrencyType: $("#CC_NormalFee").NumericCurrencyValue(),
        NormalFeeAmount: $("#CC_NormalFee").NumericValue()
    };

    call_ajax_method_json("/Contract/CTS180_ValidateAddCancelContractData", object,
    function (result, controls) {
        if (controls != undefined) {
            /* --- Higlight Text --- */
            /* --------------------- */
            VaridateCtrl(["CC_FeeType",
                    "CC_HandlingType",
                    "CC_Fee",
                    "CC_Tax",
                    "CC_PeriodFrom",
                    "CC_PeriodTo",
                    "CC_Remark",
                    "CC_ContractCodeForSlideFee",
                    "CC_NormalFee"], controls);

        } else if (result != undefined) {

            call_ajax_method_json("/Contract/CTS180_AddCancelContractData", object,
                function (result) {
                    if (result != undefined) {
                        /* --- Check Empty Row --- */
                        /* ----------------------- */
                        CheckFirstRowIsEmpty(gridCancelContractCTS180, true);
                        /* ----------------------- */

                        /* --- Add new row --- */
                        /* ------------------- */
                        AddNewRow(gridCancelContractCTS180,
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

                        var removeColinx = gridCancelContractCTS180.getColIndexById('Remove');
                        var row_idx = gridCancelContractCTS180.getRowsNum() - 1;
                        var row_id = gridCancelContractCTS180.getRowId(row_idx);

                        GenerateRemoveButton(gridCancelContractCTS180, "btnRemove", row_id, "Remove", true);

                        /* --- Set Event --- */
                        /* ----------------- */
                        BindGridButtonClickEvent("btnRemove", row_id,
                            function (row_id) {
                                RemoveDataCancelCondCTS180_05(row_id, row_idx);
                            }
                        );
                        /* ----------------- */

                        gridCancelContractCTS180.setSizes();

                        CalculateTotalAmountCTS180_05();

                        //$("#divConditionCancelCont").clearForm(); //InitialInputDataCTS180_05();
                        //SetEnableProcessAfterCounterBalCTS180_05();
                        ClearCancelCondCTS180_05();
                    }
                }
            );

        }
    });
}

function ClearCancelCondCTS180_05() {
    $("#divConditionCancelCont").clearForm();
    ClearDateFromToControl("#CC_PeriodFrom", "#CC_PeriodTo");

    $("#CC_Fee").attr("readonly", false);
    $("#CC_Fee").NumericCurrency().attr("disabled", false);

    $("#CC_Tax").attr("readonly", false);
    $("#CC_Tax").NumericCurrency().attr("disabled", false);

    $("#CC_NormalFee").attr("readonly", false);
    $("#CC_NormalFee").NumericCurrency().attr("disabled", false);

    $("#CC_ContractCodeForSlideFee").attr("readonly", false);
    $("#CC_PeriodFrom").attr("readonly", false);
    $("#CC_PeriodTo").attr("readonly", false);
    $("#CC_Remark").attr("readonly", false);
}

function RemoveDataCancelCondCTS180_05(row_id) {
    gridCancelContractCTS180.selectRow(gridCancelContractCTS180.getRowIndex(row_id));

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
                    var feeTypeCol = gridCancelContractCTS180.getColIndexById("FeeTypeCode");
                    var feeType = gridCancelContractCTS180.cells(row_id, feeTypeCol).getValue();

                    var seqCol = gridCancelContractCTS180.getColIndexById("Sequence");
                    var seq = gridCancelContractCTS180.cells(row_id, seqCol).getValue();

                    //Remove the seleted row from cancel contract condition list
                    DeleteRow(gridCancelContractCTS180, row_id);
                    gridCancelContractCTS180.setSizes();

                    var obj = { strSequence: seq };
                    call_ajax_method_json("/Contract/CTS180_RemoveDataCancelCond", obj,
                        function (result) {
                            if (result != undefined) {
                                CalculateTotalAmountCTS180_05();
                                //SetEnableProcessAfterCounterBalCTS180_05();
                            }
                        }
                    );
                },
                null);
        }
    );
    /* ------------------- */
}

function GetDocumentDataCTS180_05() {
    //var balType = null;
    //var balTypeUsd = null;

    //if ($("#CC_Refund").prop('checked')) {
    //    balTypeUsd = $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_REFUND").val();
    //}
    //else if ($("#CC_ReceiveAsRevenue").prop('checked')) {
    //    balTypeUsd = $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_RECEIVE").val();
    //}
    //else if ($("#CC_Bill").prop('checked')) {
    //    balType = $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_BILL").val();
    //}
    //else if ($("#CC_Exempt").prop('checked')) {
    //    balType = $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_EXEMPT").val();
    //}

    var procAfterCounterBal = null;
    if ($("#divProcessCounterBalance #CC_Refund").prop("checked")) {
        procAfterCounterBal = $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_REFUND").val();
    }
    else if ($("#divProcessCounterBalance #CC_ReceiveAsRevenue").prop("checked")) {
        procAfterCounterBal = $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_RECEIVE").val();
    }
    else if ($("#divProcessCounterBalance #CC_Bill").prop("checked")) {
        procAfterCounterBal = $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_BILL").val();
    }
    else if ($("#divProcessCounterBalance #CC_Exempt").prop("checked")) {
        procAfterCounterBal = $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_EXEMPT").val();
    }

    var procAfterCounterBalUsd = null;
    if ($("#divProcessCounterBalanceUsd #CC_RefundUsd").prop("checked")) {
        procAfterCounterBalUsd = $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_REFUND").val();
    }
    else if ($("#divProcessCounterBalanceUsd #CC_ReceiveAsRevenueUsd").prop("checked")) {
        procAfterCounterBalUsd = $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_RECEIVE").val();
    }
    else if ($("#divProcessCounterBalanceUsd #CC_BillUsd").prop("checked")) {
        procAfterCounterBalUsd = $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_BILL").val();
    }
    else if ($("#divProcessCounterBalanceUsd #CC_ExemptUsd").prop("checked")) {
        procAfterCounterBalUsd = $("#C_PROC_AFT_COUNTER_BALANCE_TYPE_EXEMPT").val();
    }

    //var autoBillType = null;
    //if ($("#CC_AutoNone").prop('checked')) {
    //    autoBillType = $("#C_AUTO_TRANSFER_BILLING_TYPE_NONE").val();
    //}
    //else if ($("#CC_AutoAll").prop('checked')) {
    //    autoBillType = $("#C_AUTO_TRANSFER_BILLING_TYPE_ALL").val();
    //}
    //else if ($("#CC_AutoPartial").prop('checked')) {
    //    autoBillType = $("#C_AUTO_TRANSFER_BILLING_TYPE_PARTIAL").val();
    //}

    var bankBillType = null;
    var bankBillTypeUsd = null;

    if ($("#CC_BankNone").prop('checked')) {
        bankBillType = $("#C_BANK_TRANSFER_BILLING_TYPE_NONE").val();
    }
    else if ($("#CC_BankAll").prop('checked')) {
        bankBillType = $("#C_BANK_TRANSFER_BILLING_TYPE_ALL").val();
    }
    else if ($("#CC_BankPartial").prop('checked')) {
        bankBillType = $("#C_BANK_TRANSFER_BILLING_TYPE_PARTIAL").val();
    }

    if ($("#CC_BankNoneUsd").prop('checked')) {
        bankBillTypeUsd = $("#C_BANK_TRANSFER_BILLING_TYPE_NONE").val();
    }
    else if ($("#CC_BankAllUsd").prop('checked')) {
        bankBillTypeUsd = $("#C_BANK_TRANSFER_BILLING_TYPE_ALL").val();
    }
    else if ($("#CC_BankPartialUsd").prop('checked')) {
        bankBillTypeUsd = $("#C_BANK_TRANSFER_BILLING_TYPE_PARTIAL").val();
    }

    var objCTS180_05 = {
        ContractCodeShort: $("#CC_ContractCodeShort").val(),
        ContractTargetNameLC: $("#CC_ContractTargetName").val(),
        StartServiceDate: $("#CC_StartServiceDate").val(),
        CancelContractDate: $("#CC_CancelDate").val(),
        RealCustomerNameLC: $("#CC_RealCustomerName").val(),
        SiteNameEN: $("#CC_SiteNameEN").val(),
        SiteNameLC: $("#CC_SiteNameLC").val(),
        SiteAddressLC: $("#CC_SiteAddress").val(),
        //ProcessAfterCounterBalanceType: balType,
        //AutoTransferBillingType: autoBillType,
        //BankTransferBillingType: bankBillType,
        CustomerSignatureName: $("#CC_CustomerSignatureName").val(),
        OtherRemarks: $("#CC_OtherRemarks").val(),
        //AutoTransferBillingAmt: $("#CC_AutoTransferBillingAmount").NumericValue(),

        ProcessAfterCounterBalanceType: procAfterCounterBal,
        BankTransferBillingType: bankBillType,
        BankTransferBillingAmt: $("#CC_BankTransferBillingAmount").NumericValue(),

        ProcessAfterCounterBalanceTypeUsd: procAfterCounterBalUsd,
        BankTransferBillingTypeUsd: bankBillTypeUsd,
        BankTransferBillingAmtUsd: $("#CC_BankTransferBillingAmountUsd").NumericValue(),

        SECOMSignatureFlag: $("#CC_UseSignaturePicture").prop("checked"),
        EmpName: $("#CC_EmployeeName").val(),
        EmpPosition: $("#CC_EmployeePosition").val()
    };

    return objCTS180_05;
}
