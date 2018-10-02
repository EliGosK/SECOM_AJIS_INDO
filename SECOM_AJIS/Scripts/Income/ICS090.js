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

var ics090_var = {
    isRetrieveInvoice: false,
    invoicePaymentStatus: ""
};

//This screen have no screen mode.

ics090_var.controls = {
    invoiceNo: "#InvoiceNo",

    billingClientNameEN: "#BillingClientNameEN",
    billingClientNameLC: "#BillingClientNameLC",
    invoiceAmountIncTax: "#InvoiceAmountIncTax",
    currentPaymentStatus: "#CurrentPaymentStatus",
    paymentStatusAfterChange: "#PaymentStatusAfterChange",
    correctionReason: "#CorrectionReason",
    approveNo: "#ApproveNo",

    btnRetrieve: "#BtnRetrieve",

    divInvoiceNo: "#DivInvoiceNo",
    divCorrectionReason: "#DivCorrectionReason",
    divApproveNo: "#DivApproveNo",
    
    divInputInvoice: "#DivInputInvoice",
    divInvoiceDetail: "#DivInvoiceDetail"
};



var ics090_process = {
    InitialScreen: function () {
        ics090_process.BidingEvent();
    },

    BidingEvent: function () {
        $(ics090_var.controls.btnRetrieve).click(ics090_process.CmdRetrieve_OnClick);
        $(ics090_var.controls.correctionReason).change(ics090_process.CorrectionReason_OnChange);

        //Screen command
        confirmCancel_command.SetCommand(ics090_process.CmdConfirmCancel_OnClick);
        reset_command.SetCommand(ics090_process.CmdReset_OnClick);
    },

    CmdRetrieve_OnClick: function () {
        $(ics090_var.controls.btnRetrieve).SetDisabled(true);

        //Retrive invoice information and display on screen
        var obj = { InvoiceNo: $(ics090_var.controls.invoiceNo).val() };

        ajax_method.CallScreenController("/Income/ICS090_GetInvoice", obj, function (result, controls) {
            if (result != undefined) {
                //Success
                ics090_var.isRetrieveInvoice = true;

                //Display invoice info
                $(ics090_var.controls.billingClientNameEN).val(result.doInvoice.BillingClientNameEN);
                $(ics090_var.controls.billingClientNameLC).val(result.doInvoice.BillingClientNameLC);
                $(ics090_var.controls.invoiceAmountIncTax).val((new Number(result.doInvoice.BillingAmountIncVATGridDisplay)).numberFormat("#,##0.00"));
                $(ics090_var.controls.invoiceAmountIncTax).NumericCurrency().val(result.doInvoice.BillingAmountIncVATGridDisplayCurrencyType);

                $(ics090_var.controls.currentPaymentStatus).val(result.PaymentStatus);
                $(ics090_var.controls.approveNo).val("");

                //Set disable control
                $(ics090_var.controls.invoiceNo).SetDisabled(true);
                $(ics090_var.controls.btnRetrieve).SetDisabled(true);

                //Filter correction reason
                ics090_process.FilterCorrectionReason(result.doInvoice.InvoicePaymentStatus);
            }
            else {
                ics090_var.isRetrieveInvoice = false;
                $(ics090_var.controls.btnRetrieve).SetDisabled(false);

                if (controls != undefined) {
                    VaridateCtrl(controls, controls);
                    $(ics090_var.controls.divInvoiceDetail).clearForm();
                }
            }
        });
    },

    FilterCorrectionReason: function (invoicePaymentStatus) {
        ics090_var.invoicePaymentStatus = invoicePaymentStatus;
        var obj = {
            paymentStatus: ics090_var.invoicePaymentStatus
        };
        ajax_method.CallScreenController("/Income/ICS090_GetCorrectionReason", obj, function (data) {
            regenerate_combo(ics090_var.controls.correctionReason, data);

            //Auto select 
            var itemCount = $(ics090_var.controls.correctionReason).children().length;
            if (itemCount == 2) {
                $("select" + ics090_var.controls.correctionReason).attr('selectedIndex', 1);
            }
        });
    },

    CorrectionReason_OnChange: function () {
        if ($(ics090_var.controls.correctionReason).val() == "") {
            $(ics090_var.controls.paymentStatusAfterChange).val("");
        }
        else {
            var obj = {
                paymentStatus: ics090_var.invoicePaymentStatus,
                correctionReason: $(ics090_var.controls.correctionReason).val()
            };
            ajax_method.CallScreenController("/Income/ICS090_GetNextPaymentStatus", obj, function (result, controls) {
                if (result != undefined) {
                    $(ics090_var.controls.paymentStatusAfterChange).val(result);
                }
                else {
                    $(ics090_var.controls.paymentStatusAfterChange).val("");
                }
            });
        }
    },

    CmdConfirmCancel_OnClick: function () {
        var ctrl = new Array();
        var controlname = "";

        if ($(ics090_var.controls.invoiceNo).val() == "" || ics090_var.isRetrieveInvoice == false) {
            var lblInvoiceNo = $(ics090_var.controls.divInvoiceNo).html();
            ctrl.push(ics090_var.controls.invoiceNo.substring(1, ics090_var.controls.invoiceNo.length));
            controlname += lblInvoiceNo;
        }
        if ($(ics090_var.controls.correctionReason).val() == "") {
            var lblCorrectionReason = $(ics090_var.controls.divCorrectionReason).html();
            ctrl.push(ics090_var.controls.correctionReason.substring(1, ics090_var.controls.correctionReason.length));
            controlname += (controlname == "" ? "" : ",") + lblCorrectionReason;
        }
        if ($(ics090_var.controls.approveNo).val() == "") {
            var lblApproveNo = $(ics090_var.controls.divApproveNo).html();
            ctrl.push(ics090_var.controls.approveNo.substring(1, ics090_var.controls.approveNo.length));
            controlname += (controlname == "" ? "" : ",") + lblApproveNo;
        }


        if (controlname != "") {
            //Show warning dialog
            var objMsg = { module: "Common", code: "MSG0007", param: controlname };
            call_ajax_method_json("/Shared/GetMessage", objMsg, function (resultMsg) {
                OpenWarningDialog(resultMsg.Message);
                VaridateCtrl(ctrl, ctrl);
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
                        command_control.CommandControlMode(false);

                        //Confirm cancel
                        var obj = {
                            InvoiceNo: $(ics090_var.controls.invoiceNo).val(),
                            CorrectionReason: $(ics090_var.controls.correctionReason).val(),
                            ApproveNo: $(ics090_var.controls.approveNo).val()
                        };
                        ajax_method.CallScreenController("/Income/ICS090_cmdConfirmCancel", obj, function (result, controls, isWarning) {
                            if (result != undefined) {
                                // Success
                                var objMsg = { module: "Income", code: "MSG7008" };
                                call_ajax_method_json("/Shared/GetMessage", objMsg, function (resultMsg) {
                                    OpenInformationMessageDialog(resultMsg.Code, resultMsg.Message, function () {
                                        //Show pdf file on new window
                                        if (result == "PDF") {
                                            var key = ajax_method.GetKeyURL(null);
                                            var url = ajax_method.GenerateURL("/Income/ICS090_DisplayReport?k=" + key);
                                            window.open(url, "", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
                                        }

                                        //Same as reset screen
                                        ics090_process.CmdReset_OnClick();
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
        $(ics090_var.controls.divInputInvoice).clearForm();
        ics090_process.FilterCorrectionReason(null);

        //Enable control
        $(ics090_var.controls.invoiceNo).SetDisabled(false);
        $(ics090_var.controls.btnRetrieve).SetDisabled(false);

        //Clear flag
        ics090_var.isRetrieveInvoice = false;
    }
};



$(document).ready(function () {
    ics090_process.InitialScreen();
});