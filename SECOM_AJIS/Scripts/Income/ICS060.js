/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/control_events.js"/>
/// <reference path="../../Scripts/Base/DateTimePicker.js"/>
/// <reference path="../Base/object/ajax_method.js" />
/// <reference path="../Base/object/command_event.js" />
/// <reference path="../Base/object/master_event.js" />

var ics060_var = {
    isRetrieveReceipt: false
};

//This screen have no screen mode.

ics060_var.controls = {
    receiptNo: "#ReceiptNo",
    taxInvoiceNo: "#TaxInvoiceNo",
    receiptIssueDate: "#ReceiptIssueDate",
    billingClientNameEN: "#BillingClientNameEN",
    billingClientNameLC: "#BillingClientNameLC",
    billingClientAddressEN: "#BillingClientAddressEN",
    billingClientAddressLC: "#BillingClientAddressLC",
    invoiceNo: "#InvoiceNo",
    paymentDate: "#PaymentDate",
    paymentMatchingDate: "#PaymentMatchingDate",
    billingTotalAmountIncVAT: "#BillingTotalAmountIncVAT",
    vatAmount: "#VatAmount",
    whtAmount: "#WhtAmount",
    advanceIssueReceipt: "#AdvanceIssueReceipt",
    cancelMethod: "#CancelMethod",
    cancelMethod_Require: "#CancelMethod_Require",

    btnRetrieve: "#BtnRetrieve",

    divReceiptNo: "#DivReceiptNo",
    divCancelMethod: "#DivCancelMethod",
    divInputReceipt: "#DivInputReceipt",
    divReceiptDetail: "#DivReceiptDetail"
};



var ics060_process = {
    InitialScreen: function () {
        ics060_process.BidingEvent();
    },

    BidingEvent: function () {
        $(ics060_var.controls.btnRetrieve).click(ics060_process.CmdRetrieve_OnClick);

        //Screen command
        confirmCancel_command.SetCommand(ics060_process.CmdConfirmCancel_OnClick);
        reset_command.SetCommand(ics060_process.CmdReset_OnClick);
    },

    CmdRetrieve_OnClick: function () {
        $(ics060_var.controls.btnRetrieve).SetDisabled(true);

        //Retrive receipt information and display on screen
        var obj = { ReceiptNo: $(ics060_var.controls.receiptNo).val() };
        ajax_method.CallScreenController("/Income/ICS060_GetReceipt", obj, function (result, controls) {
            if (result != undefined && result.doReceipt != undefined) {
                //Success
                ics060_var.isRetrieveReceipt = true;


                //Display receipt info
                $(ics060_var.controls.taxInvoiceNo).val(result.doReceipt.TaxInvoiceNo),
                $(ics060_var.controls.receiptIssueDate).val(result.doReceipt.ReceiptDateDisplay),
                $(ics060_var.controls.billingClientNameEN).val(result.doReceipt.BillingClientNameEN);
                $(ics060_var.controls.billingClientNameLC).val(result.doReceipt.BillingClientNameLC);
                $(ics060_var.controls.billingClientAddressEN).val(result.doReceipt.BillingClientAddressEN);
                $(ics060_var.controls.billingClientAddressLC).val(result.doReceipt.BillingClientAddressLC);

                $(ics060_var.controls.invoiceNo).val(result.doReceipt.InvoiceNo);

                $(ics060_var.controls.billingTotalAmountIncVAT).val((new Number(result.doReceipt.ReceiptAmount)).numberFormat("#,##0.00"));
                $(ics060_var.controls.billingTotalAmountIncVAT).NumericCurrency().val(result.doReceipt.ReceiptAmountCurrencyType);

                $(ics060_var.controls.vatAmount).val((new Number(result.doReceipt.VATAmount)).numberFormat("#,##0.00"));
                $(ics060_var.controls.vatAmount).NumericCurrency().val(result.doReceipt.VatAmountCurrencyType);

                $(ics060_var.controls.whtAmount).val((new Number(result.doReceipt.WHTAmount)).numberFormat("#,##0.00"));
                $(ics060_var.controls.whtAmount).NumericCurrency().val(result.doReceipt.WHTAmountCurrencyType);

                $(ics060_var.controls.paymentDate).val(result.doReceipt.PaymentDateDisplay);
                $(ics060_var.controls.paymentMatchingDate).val(result.doReceipt.PaymentMatchingDateDisplay);

                if (result.doReceipt.AdvanceReceiptStatusFlag == true) {
                    //Is Advance receipt status
                    $(ics060_var.controls.advanceIssueReceipt).attr('checked', true);
                }
                else {
                    $(ics060_var.controls.advanceIssueReceipt).attr('checked', false);
                }

                //Enable control
                $(ics060_var.controls.cancelMethod).SetDisabled(false);
                $(ics060_var.controls.cancelMethod_Require).show();

                //Disable control
                $(ics060_var.controls.receiptNo).SetDisabled(true);
                $(ics060_var.controls.btnRetrieve).SetDisabled(true);
            }
            else {
                ics060_var.isRetrieveReceipt = false;
                $(ics060_var.controls.cancelMethod).SetDisabled(true);
                $(ics060_var.controls.cancelMethod_Require).hide();
                $(ics060_var.controls.btnRetrieve).SetDisabled(false);

                if (controls != undefined) {
                    VaridateCtrl(controls, controls);
                    $(ics060_var.controls.divReceiptDetail).clearForm();
                }
            }

            if (result != undefined && result.CancelMethodComboBoxModel != undefined) {
                regenerate_combo(ics060_var.controls.cancelMethod, result.CancelMethodComboBoxModel);
            }
        });
    },

    CmdConfirmCancel_OnClick: function () {
        var receiptNo = $(ics060_var.controls.receiptNo).val();
        var cancelMethod = $(ics060_var.controls.cancelMethod).val();

        if (receiptNo == "" || ics060_var.isRetrieveReceipt == false) {
            //Show warning dialog
            var lblReceiptNo = $(ics060_var.controls.divReceiptNo).html();
            var objMsg = { module: "Common", code: "MSG0007", param: [lblReceiptNo] };
            call_ajax_method_json("/Shared/GetMessage", objMsg, function (resultMsg) {
                OpenWarningDialog(resultMsg.Message);
            });
        }
        else if (cancelMethod == "") {
            //Show warning dialog
            var lblCancelMethod = $(ics060_var.controls.divCancelMethod).html();
            var objMsg = { module: "Common", code: "MSG0007", param: [lblCancelMethod] };
            call_ajax_method_json("/Shared/GetMessage", objMsg, function (resultMsg) {
                OpenWarningDialog(resultMsg.Message);
            });
        }
        else {
            var obj = {
                module: "Common",
                code: "MSG0028",
                param: [$("#btnCommandConfirmCancel").html()]
            };
            ajax_method.CallScreenController("/Shared/GetMessage", obj, function (result) {
                OpenYesNoMessageDialog(result.Code, result.Message,
                    function () {
                        //Confirm cancel
                        var objData = {
                            ReceiptNo: receiptNo,
                            CancelMethod: cancelMethod
                        };
                        ajax_method.CallScreenController("/Income/ICS060_cmdConfirmCancel", objData, function (result, controls, isWarning) {
                            if (result != undefined) {
                                // Success
                                var objMsg = { module: "Income", code: "MSG7008" };
                                call_ajax_method_json("/Shared/GetMessage", objMsg, function (resultMsg) {
                                    OpenInformationMessageDialog(resultMsg.Code, resultMsg.Message, function () {
                                        //Same as reset screen
                                        ics060_process.CmdReset_OnClick();
                                    });
                                });
                            }
                            else if (controls != undefined) {
                                VaridateCtrl(controls, controls);
                            }
                        });
                    }
                    , null);
            });
        }
    },

    CmdReset_OnClick: function () {
        //Reset all and get back as initial screen state
        $(ics060_var.controls.divInputReceipt).clearForm();
        //Enable control
        $(ics060_var.controls.receiptNo).SetDisabled(false);
        $(ics060_var.controls.btnRetrieve).SetDisabled(false);
        $(ics060_var.controls.cancelMethod).SetDisabled(true);
        $(ics060_var.controls.cancelMethod_Require).hide();

        var arr1 = {
            List: new Array()
        };
        var opt1 = {
            Value: $(ics060_var.controls.cancelMethod + " option:first").val(),
            Display: $(ics060_var.controls.cancelMethod + " option:first").text()
        };
        arr1.List.push(opt1);
        regenerate_combo(ics060_var.controls.cancelMethod, arr1);

        //Clear flag
        ics060_var.isRetrieveReceipt = false;
    }
};



$(document).ready(function () {
    ics060_process.InitialScreen();
});