var CTS140_09 = {
    mygridCTS140: null,

    InitialControl: function () {
        $("#btnAddCancelContract").click(CTS140_09.BtnAddCancelClick);
        $("#btnCancel").click(CTS140_09.ClearControl);

        InitialDateFromToControl("#StartPeriodDateContract", "#EndPeriodDateContract");

        $("#FeeAmount").BindNumericBox(12, 2, 0, 999999999999.99);
        $("#TaxAmount").BindNumericBox(12, 2, 0, 999999999999.99);
        $("#NormalFeeAmount").BindNumericBox(12, 2, 0, 999999999999.99);

        $("#TotalSlideAmt").BindNumericBox(12, 2, 0, 999999999999.99);
        $("#TotalRefundAmt").BindNumericBox(12, 2, 0, 999999999999.99);
        $("#TotalBillingAmt").BindNumericBox(12, 2, 0, 999999999999.99);
        $("#TotalAmtAfterCounterBalanceType").BindNumericBox(12, 2, 0, 999999999999.99);

        $("#TotalSlideAmtUsd").BindNumericBox(12, 2, 0, 999999999999.99);
        $("#TotalRefundAmtUsd").BindNumericBox(12, 2, 0, 999999999999.99);
        $("#TotalBillingAmtUsd").BindNumericBox(12, 2, 0, 999999999999.99);
        $("#TotalAmtAfterCounterBalanceTypeUsd").BindNumericBox(12, 2, 0, 999999999999.99);

        $("#FeeType").change(CTS140_09.FeeTypeChange);

        //Set maxlenght for TextArea control
        $("#Remark").SetMaxLengthTextArea(100);
        $("#OtherRemarks").SetMaxLengthTextArea(4000); //(1600);

        CTS140_09.ReGenerateHandlingTypeCombo();
        CTS140_09.GetCancelContractMemo();
        CTS140_09.GetCancelGrid();
    },
    SetSectionMode: function (isView) {
        $("#divCancelContractCondition").SetViewMode(isView);

        //gridCancelContract
        if (isView) {
            $("#divCondition").hide();

            if (CTS140_09.mygridCTS140 != undefined) {
                var removeCol = CTS140_09.mygridCTS140.getColIndexById("Remove");
                CTS140_09.mygridCTS140.setColumnHidden(removeCol, true);
                CTS140_09.mygridCTS140.setSizes();
            }
        }
        else {
            $("#divCondition").show();

            if (CTS140_09.mygridCTS140 != undefined) {
                var removeCol = CTS140_09.mygridCTS140.getColIndexById("Remove");
                CTS140_09.mygridCTS140.setColumnHidden(removeCol, false);
                CTS140_09.mygridCTS140.setSizes();

                $("#gridCancelContract").LoadDataToGrid(CTS140_09.mygridCTS140, 0, false, "/Contract/CTS140_GetCancelGrid",
                                                "", "CTS140_DOCancelContractGrid", false, null, null);
            }

            $("#divProcessCounterBalance input[type='radio']").attr("disabled", false);

            var totalAmtAfterCounterBalance = parseInt($("#TotalAmtAfterCounterBalanceType").NumericValue());
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

            var totalAmtAfterCounterBalanceUsd = parseInt($("#TotalAmtAfterCounterBalanceTypeUsd").NumericValue());
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
    },
    DisabledSection: function (isDisabled) {
        //Disable mygridCTS140
        if (CTS140_09.mygridCTS140 != undefined) {
            if (CheckFirstRowIsEmpty(CTS140_09.mygridCTS140, false) == false) {
                CTS140_09.BindCancelContractGrid(false);
            }
        }
        $("#divCancelContractCondition").SetEnableView(!isDisabled);
    },

    ReGenerateHandlingTypeCombo: function () {
        var billingType = $("#FeeType option:selected").val();
        var doCancelContractCondition = {
            BillingType: billingType
        };
        call_ajax_method_json('/Contract/CTS140_InitialBillingType', doCancelContractCondition,
            function (result, controls) {
                regenerate_combo("#HandlingType", result);
            });
    },
    GetCancelGrid: function () {
        $("#gridCancelContract").html("");
        CTS140_09.mygridCTS140 = $("#gridCancelContract").LoadDataToGridWithInitial(0, false, false, "/Contract/CTS140_GetCancelGrid", "", "CTS140_DOCancelContractGrid", false);
        SpecialGridControl(CTS140_09.mygridCTS140, ["Remove"]);
        BindOnLoadedEvent(CTS140_09.mygridCTS140, function (gen_ctrl) {
            if (CTS140_09.mygridCTS140 == true) {
                CTS140_09.mygridCTS140.setColumnHidden(CTS140_09.mygridCTS140.getColIndexById('BillingType'), true);
                CTS140_09.mygridCTS140.setColumnHidden(CTS140_09.mygridCTS140.getColIndexById('HandlingType'), true);
                CTS140_09.mygridCTS140.setColumnHidden(CTS140_09.mygridCTS140.getColIndexById('Sequence'), true);
            }

            CTS140_09.BindCancelContractGrid(true);
        });

        CTS140_09.GetCancelGridTotal();
    },
    BindCancelContractGrid: function (isEnbBtn) {
        var removeColinx = CTS140_09.mygridCTS140.getColIndexById('Remove');
        var periodColinx = CTS140_09.mygridCTS140.getColIndexById('PeriodString');

        var row_id;
        for (var i = 0; i < CTS140_09.mygridCTS140.getRowsNum(); i++) {
            row_id = CTS140_09.mygridCTS140.getRowId(i);

            GenerateRemoveButton(CTS140_09.mygridCTS140, "btnRemoveBilling", row_id, "Remove", isEnbBtn);
            BindGridButtonClickEvent("btnRemoveBilling", row_id,
                function (row_id) {
                    CTS140_09.mygridCTS140.selectRow(CTS140_09.mygridCTS140.getRowIndex(row_id));

                    var obj = {
                        module: "Common",
                        code: "MSG0097"
                    };
                    call_ajax_method_json("/Shared/GetMessage", obj,
                        function (result) {
                            OpenYesNoMessageDialog(result.Code, result.Message,
                                function () {
                                    var seqCol = CTS140_09.mygridCTS140.getColIndexById("Sequence");
                                    var seq = CTS140_09.mygridCTS140.cells(row_id, seqCol).getValue();
                                    var obj = { strSequence: seq };
                                    call_ajax_method('/Contract/CTS140_RemoveCancel', obj, //doCancelContractCondition,
                                        function (result, controls) {
                                            if (result == undefined) {
                                                CTS140_09.GetCancelGrid();
                                            }
                                        });
                                }
                            , null);
                        }
                    );
                }
            );
        }
    },
    GetCancelGridTotal: function () {
        call_ajax_method_json('/Contract/CTS140_GetCancelGridTotal', "",
            function (result, controls) {
                if (result != undefined) {
                    //$("#divRefundReceiveRevenue").show();
                    //$("#divBillExempt").show();
                    //$("#divRefundReceiveRevenue").removeAttr("disabled");
                    //$("#divBillExempt").removeAttr("disabled");

                    $("#TotalSlideAmt").val(result.TotalSlideAmt);
                    $("#TotalRefundAmt").val(result.TotalRefundAmt);
                    $("#TotalBillingAmt").val(result.TotalBillingAmt);
                    $("#TotalAmtAfterCounterBalanceType").val(result.TotalAmtAfterCounterBalanceType);

                    $("#TotalSlideAmtUsd").val(result.TotalSlideAmtUsd);
                    $("#TotalRefundAmtUsd").val(result.TotalRefundAmtUsd);
                    $("#TotalBillingAmtUsd").val(result.TotalBillingAmtUsd);
                    $("#TotalAmtAfterCounterBalanceTypeUsd").val(result.TotalAmtAfterCounterBalanceTypeUsd);

                    $("#divProcessCounterBalance input[type='radio']").attr("disabled", false);

                    var totalAmtAfterCounterBalance = parseInt($("#TotalAmtAfterCounterBalanceType").NumericValue());
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

                    var totalAmtAfterCounterBalanceUsd = parseInt($("#TotalAmtAfterCounterBalanceTypeUsd").NumericValue());
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

                    //$("#rdoRefund").removeAttr("checked");
                    //$("#rdoReceiveRevenue").removeAttr("checked");
                    //$("#rdoBill").removeAttr("checked");
                    //$("#rdoExempt").removeAttr("checked");

                    //var refund = parseFloat($("#TotalRefundAmt").NumericValue());
                    //var billing = parseFloat($("#TotalBillingAmt").NumericValue());
                    //if (refund > billing) {
                    //    //$("#divBillExempt").hide();
                    //    $("#divBillExempt").attr("disabled", true);
                    //    $("#rdoRefund").attr("checked", "checked");
                    //}
                    //else if (refund < billing) {
                    //    //$("#divRefundReceiveRevenue").hide();
                    //    $("#divRefundReceiveRevenue").attr("disabled", true);
                    //    $("#rdoBill").attr("checked", "checked");
                    //}
                    //else {
                    //    $("#divBillExempt").attr("disabled", true);
                    //    $("#divRefundReceiveRevenue").attr("disabled", true);
                    //}
                }
            });
    },

    FeeTypeChange: function () {
        $("#divCondition").ResetToNormalControl();
        if ($("#FeeType option:selected").val() == "") {
            $("#FeeAmount").attr("readonly", false);
            $("#FeeAmount").NumericCurrency().attr("disabled", false);

            $("#TaxAmount").attr("readonly", false);
            $("#TaxAmount").NumericCurrency().attr("disabled", false);

            $("#ContractCode_CounterBalance").attr("readonly", false);
            $("#StartPeriodDateContract").attr("readonly", false);
            $("#EndPeriodDateContract").attr("readonly", false);
            $("#Remark").attr("readonly", false);

            $("#NormalFeeAmount").attr("readonly", false);
            $("#NormalFeeAmount").NumericCurrency().attr("disabled", false);
        }
        if ($("#FeeType option:selected").val() == objCTS140.C_BILLING_TYPE_CONTRACT_FEE ||
            $("#FeeType option:selected").val() == objCTS140.C_BILLING_TYPE_MAINTENANCE_FEE ||
            $("#FeeType option:selected").val() == objCTS140.C_BILLING_TYPE_OTHER_FEE) {

            $("#FeeAmount").attr("readonly", false);
            $("#FeeAmount").NumericCurrency().attr("disabled", false);

            $("#TaxAmount").attr("readonly", false);
            $("#TaxAmount").NumericCurrency().attr("disabled", false);

            $("#ContractCode_CounterBalance").attr("readonly", false);
            $("#StartPeriodDateContract").attr("readonly", false);
            $("#EndPeriodDateContract").attr("readonly", false);
            $("#Remark").attr("readonly", false);

            $("#NormalFeeAmount").attr("readonly", true);
            $("#NormalFeeAmount").NumericCurrency().attr("disabled", true);

            $("#NormalFeeAmount").val("");
        }
        if ($("#FeeType option:selected").val() == objCTS140.C_BILLING_TYPE_DEPOSIT_FEE) {
            $("#FeeAmount").attr("readonly", false);
            $("#FeeAmount").NumericCurrency().attr("disabled", false);

            $("#TaxAmount").attr("readonly", false);
            $("#TaxAmount").NumericCurrency().attr("disabled", false);

            $("#ContractCode_CounterBalance").attr("readonly", false);
            $("#Remark").attr("readonly", false);
            $("#StartPeriodDateContract").attr("readonly", true);
            $("#EndPeriodDateContract").attr("readonly", true);

            $("#NormalFeeAmount").attr("readonly", true);
            $("#NormalFeeAmount").NumericCurrency().attr("disabled", true);

            $("#StartPeriodDateContract").val("");
            $("#EndPeriodDateContract").val("");
            $("#NormalFeeAmount").val("");
        }
        if ($("#FeeType option:selected").val() == objCTS140.C_BILLING_TYPE_REMOVAL_INSTALLATION_FEE) {
            $("#FeeAmount").attr("readonly", false);
            $("#FeeAmount").NumericCurrency().attr("disabled", false);

            $("#TaxAmount").attr("readonly", false);
            $("#TaxAmount").NumericCurrency().attr("disabled", false);

            $("#Remark").attr("readonly", false);

            $("#NormalFeeAmount").attr("readonly", false);
            $("#NormalFeeAmount").NumericCurrency().attr("disabled", false);

            $("#StartPeriodDateContract").attr("readonly", true);
            $("#EndPeriodDateContract").attr("readonly", true);
            $("#ContractCode_CounterBalance").attr("readonly", true);

            $("#StartPeriodDateContract").val("");
            $("#EndPeriodDateContract").val("");
            $("#ContractCode_CounterBalance").val("");
        }
        if ($("#FeeType option:selected").val() == objCTS140.C_BILLING_TYPE_CANCEL_CONTRACT_FEE) {
            $("#FeeAmount").attr("readonly", false);
            $("#FeeAmount").NumericCurrency().attr("disabled", false);

            $("#Remark").attr("readonly", false);

            $("#TaxAmount").attr("readonly", true);
            $("#TaxAmount").NumericCurrency().attr("disabled", true);

            $("#StartPeriodDateContract").attr("readonly", true);
            $("#EndPeriodDateContract").attr("readonly", true);
            $("#ContractCode_CounterBalance").attr("readonly", true);

            $("#NormalFeeAmount").attr("readonly", true);
            $("#NormalFeeAmount").NumericCurrency().attr("disabled", true);

            $("#TaxAmount").val("");
            $("#StartPeriodDateContract").val("");
            $("#EndPeriodDateContract").val("");
            $("#ContractCode_CounterBalance").val("");
            $("#NormalFeeAmount").val("");
        }
        if ($("#FeeType option:selected").val() == objCTS140.C_BILLING_TYPE_CHANGE_INSTALLATION_FEE) {
            $("#FeeAmount").attr("readonly", false);
            $("#FeeAmount").NumericCurrency().attr("disabled", false);

            $("#TaxAmount").attr("readonly", false);
            $("#TaxAmount").NumericCurrency().attr("disabled", false);

            $("#StartPeriodDateContract").attr("readonly", false);
            $("#Remark").attr("readonly", false);
            $("#ContractCode_CounterBalance").attr("readonly", true);
            $("#EndPeriodDateContract").attr("readonly", true);

            $("#NormalFeeAmount").attr("readonly", true);
            $("#NormalFeeAmount").NumericCurrency().attr("disabled", true);

            $("#ContractCode_CounterBalance").val("");
            $("#EndPeriodDateContract").val("");
            $("#NormalFeeAmount").val("");
        }
        if ($("#FeeType option:selected").val() == objCTS140.C_BILLING_TYPE_CARD_FEE) {
            $("#FeeAmount").attr("readonly", false);
            $("#FeeAmount").NumericCurrency().attr("disabled", false);

            $("#TaxAmount").attr("readonly", false);
            $("#TaxAmount").NumericCurrency().attr("disabled", false);

            $("#Remark").attr("readonly", false);
            $("#ContractCode_CounterBalance").attr("readonly", true);
            $("#StartPeriodDateContract").attr("readonly", true);
            $("#EndPeriodDateContract").attr("readonly", true);

            $("#NormalFeeAmount").attr("readonly", true);
            $("#NormalFeeAmount").NumericCurrency().attr("disabled", true);

            $("#ContractCode_CounterBalance").val("");
            $("#StartPeriodDateContract").val("");
            $("#EndPeriodDateContract").val("");
            $("#NormalFeeAmount").val("");
        }

        CTS140_09.ReGenerateHandlingTypeCombo();
    },
    BtnAddCancelClick: function () {
        var obj = CTS140.GetValidateObject();
        call_ajax_method_json('/Contract/CTS140_ValidateAddCancelRequireField', obj,
            function (result, controls) {
                if (controls != undefined) {
                    VaridateCtrl(["FeeType",
                                    "FeeAmount",
                                    "ContractCode_CounterBalance",
                                    "HandlingType",
                                    "TaxAmount",
                                    "NormalFeeAmount",
                                    "StartPeriodDateContract",
                                    "EndPeriodDateContract",
                                    "Remark"], controls);
                    return;
                }
                else if (result == undefined) {
                    call_ajax_method('/Contract/CTS140_AddCancel', obj,
                        function (result, controls) {
                            if (controls != undefined) {
                                VaridateCtrl(["FeeType",
                                        "FeeAmount",
                                        "ContractCode_CounterBalance",
                                        "HandlingType",
                                        "TaxAmount",
                                        "NormalFeeAmount",
                                        "StartPeriodDateContract",
                                        "EndPeriodDateContract",
                                        "Remark"], controls);
                                return;
                            }
                            else if (result == undefined) {
                                CTS140_09.GetCancelGrid();
                                CTS140_09.ClearControl();
                                CTS140_09.FeeTypeChange();
                            }
                        });

                }
            });
    },

    ClearControl: function () {
        CloseWarningDialog();
        $("#divCondition").clearForm();
        ClearDateFromToControl("#StartPeriodDateContract", "#EndPeriodDateContract");

        $("#FeeAmount").removeAttr("readonly");
        $("#FeeAmount").NumericCurrency().attr("disabled", false);

        $("#TaxAmount").removeAttr("readonly");
        $("#TaxAmount").NumericCurrency().attr("disabled", false);

        $("#NormalFeeAmount").removeAttr("readonly");
        $("#NormalFeeAmount").NumericCurrency().attr("disabled", false);

        $("#ContractCode_CounterBalance").removeAttr("readonly");
        $("#StartPeriodDateContract").removeAttr("readonly");
        $("#EndPeriodDateContract").removeAttr("readonly");
        $("#Remark").removeAttr("readonly");
    },

    GetCancelContractMemo: function () {
        call_ajax_method('/Contract/CTS140_GetCancelContractMemo', null, function (result, controls) {
            if (result != null) {
                $("#OtherRemarks").val(result.OtherRemarks);
            }
        });
    }
}



//function BindCancelContractCondition() {

//    call_ajax_method_json('/Contract/BindCancelContractCondition_CTS140', "",
//    function (result, controls) {
//        if (result != undefined) {
//            objCTS140_09.C_BILLING_TYPE_CONTRACT_FEE = result.C_BILLING_TYPE_CONTRACT_FEE;
//            objCTS140_09.C_BILLING_TYPE_MAINTENANCE_FEE = result.C_BILLING_TYPE_MAINTENANCE_FEE;
//            objCTS140_09.C_BILLING_TYPE_DEPOSIT_FEE = result.C_BILLING_TYPE_DEPOSIT_FEE;
//            objCTS140_09.C_BILLING_TYPE_REMOVAL_INSTALLATION_FEE = result.C_BILLING_TYPE_REMOVAL_INSTALLATION_FEE;
//            objCTS140_09.C_BILLING_TYPE_CANCEL_CONTRACT_FEE = result.C_BILLING_TYPE_CANCEL_CONTRACT_FEE;
//            objCTS140_09.C_BILLING_TYPE_CHANGE_INSTALLATION_FEE = result.C_BILLING_TYPE_CHANGE_INSTALLATION_FEE;
//            objCTS140_09.C_BILLING_TYPE_CARD_FEE = result.C_BILLING_TYPE_CARD_FEE;
//            objCTS140_09.C_BILLING_TYPE_OTHER_FEE = result.C_BILLING_TYPE_OTHER_FEE;
//            objCTS140_09.C_HANDLING_TYPE_BILL_UNPAID_FEE = result.C_HANDLING_TYPE_BILL_UNPAID_FEE;
//            objCTS140_09.C_HANDLING_TYPE_EXEMPT_UNPAID_FEE = result.C_HANDLING_TYPE_EXEMPT_UNPAID_FEE;
//            objCTS140_09.C_HANDLING_TYPE_RECEIVE_AS_REVENUE = result.C_HANDLING_TYPE_RECEIVE_AS_REVENUE;
//            objCTS140_09.C_HANDLING_TYPE_REFUND = result.C_HANDLING_TYPE_REFUND;
//            objCTS140_09.C_HANDLING_TYPE_SLIDE = result.C_HANDLING_TYPE_SLIDE;

//            objCTS140_09.C_PROC_AFT_COUNTER_BALANCE_TYPE_BILL = result.C_PROC_AFT_COUNTER_BALANCE_TYPE_BILL;
//            objCTS140_09.C_PROC_AFT_COUNTER_BALANCE_TYPE_EXEMPT = result.C_PROC_AFT_COUNTER_BALANCE_TYPE_EXEMPT;
//            objCTS140_09.C_PROC_AFT_COUNTER_BALANCE_TYPE_RECEIVE = result.C_PROC_AFT_COUNTER_BALANCE_TYPE_RECEIVE;
//            objCTS140_09.C_PROC_AFT_COUNTER_BALANCE_TYPE_REFUND = result.C_PROC_AFT_COUNTER_BALANCE_TYPE_REFUND;

//            //            if (result.ProcessAfterCounterBalanceType == objCTS140_09.C_PROC_AFT_COUNTER_BALANCE_TYPE_RECEIVE ||
//            //            result.ProcessAfterCounterBalanceType == objCTS140_09.C_PROC_AFT_COUNTER_BALANCE_TYPE_REFUND) {
//            //                $("#divRefundReceiveRevenue").show();
//            //                $("#divBillExempt").hide();
//            //            }

//            //            if (result.ProcessAfterCounterBalanceType == objCTS140_09.C_PROC_AFT_COUNTER_BALANCE_TYPE_BILL ||
//            //            result.ProcessAfterCounterBalanceType == objCTS140_09.C_PROC_AFT_COUNTER_BALANCE_TYPE_EXEMPT) {

//            //                $("#divRefundReceiveRevenue").hide();
//            //                $("#divBillExempt").show();
//            //            }
//            if (result.ProcessAfterCounterBalanceType == objCTS140_09.C_PROC_AFT_COUNTER_BALANCE_TYPE_REFUND) {
//                $("#rdoRefund").attr('checked', true);
//            }
//            else if (result.ProcessAfterCounterBalanceType == objCTS140_09.C_PROC_AFT_COUNTER_BALANCE_TYPE_RECEIVE) {
//                $("#rdoReceiveRevenue").attr('checked', true);
//            }
//            else if (result.ProcessAfterCounterBalanceType == objCTS140_09.C_PROC_AFT_COUNTER_BALANCE_TYPE_BILL) {
//                $("#rdoBill").attr('checked', true);
//            }
//            else if (result.ProcessAfterCounterBalanceType == objCTS140_09.C_PROC_AFT_COUNTER_BALANCE_TYPE_EXEMPT) {
//                $("#rdoExempt").attr('checked', true);
//            }

//            $("#OtherRemarks").val(result.OtherRemarks);

//            $("#CancelContractDate").val(result.CancelContractDate);
//            $("#RemovalInstallationCompleteDate").val(result.RemovalInstallationCompleteDate);
//        }
//    });
//}